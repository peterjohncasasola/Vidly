import { Helpers } from './helpers.js';

let vm = {
  movieIds: []
};

$(document).ready(function () {

  $("#btn-new-rental-modal").on("click", function () {
    $('#new-rental-modal-dialog').modal('show');
    $("#btn-save").val("Save");
  });

  $("movie-rental-data-table").on("click", ".btn-remove", function () {
    $(this).closest("tr").remove();
    const id = $(this).data("id");

    const index = vm.movieIds.findIndex(movieId => movieId == id);
    vm.movieIds.splice(index, 1);
  });

  $("#newRental").on("submit", function (e) {
    e.preventDefault();
  });

  const query = "pageSize=15&orderBy=Name&searchBy=Name&comparison=contains&search=%QUERY%";

  const customers = new Bloodhound({
    datumTokenizer: Bloodhound.tokenizers.whitespace('name'),
    queryTokenizer: Bloodhound.tokenizers.whitespace,
    remote: {
      url: `/api/customers?${query}`,
      wildcard: '%QUERY%',
      filter: function (response) {
        return response.data;
      }
    },
  });


  $('#customer').typeahead({
    minLength: 3,
    highlight: true,
  },
    {
    name: 'customers',
    display: 'name',
    source: customers,
    limit: 10,
    templates: {
      empty: [
        '<div class="empty-message">',
        'unable to find any customer that match the current query',
        '</div>'
      ].join('\n'),
    },
  }).on("typeahead:select", function (e, customer) {
    e.preventDefault();
    console.log(e);
    vm.customerId = customer.id;
  });

  const movies = new Bloodhound({
    datumTokenizer: Bloodhound.tokenizers.obj.whitespace("name"),
    queryTokenizer: Bloodhound.tokenizers.whitespace,
    remote: {
      url: `/api/movies?${query}&onlyAvailable=true`,
      wildcard: '%QUERY%',
      filter: function (response) {
        return response.data;
      }
    },

  });

  $('#new-rental-modal-dialog').modal({
    backdrop: 'static',
    keyboard: false
  });
  
  movies.initialize();

  $('#movie').typeahead({
    minLength: 3,
    highlight: true,
    limit: 10
  }, {
    name: 'movies',
    display: "name",
    source: movies,
    templates: {
      empty: [
        '<div class="empty-message">',
        'unable to find any movie that match the current query',
        '</div>'
      ].join('\n'),
    }
  }).on("typeahead:select", function (e, movie) {
    let isAlready = vm.movieIds.some(id => id === movie.id);
    
    if (!isAlready) {
      $("#movie-rental-table-body").append(
        `
                <tr>
                  <td class="text-truncate">
                    ${movie.name}
                  </td>

                  <td class="text-truncate" style="max-width: 200px;">
                    ${new moment(movie.releaseDate).format("MM/DD/yyyy")}
                  </td>

                    <td class="text-truncate" style="max-width: 200px;">
                    ${movie.genre}
                  </td>

                  <td style="max-width: 150px;">
                    <button  class="btn btn-danger btn-sm btn-remove"  id="btn-delete-${movie.id}" data-id="${movie.id}">Remove</button>
                  </td>
                </tr>
              `
      );
      vm.movieIds.push(movie.id);
    }
    $("#movie").typeahead("val", "");
    return false;
  });

  $.validator.addMethod("validCustomer", function () {
    return vm.customerId && vm.customerId !== 0;
  }, "Please select a valid customer.");
  $.validator.addMethod("atLeastOneMovie", function () {
    return vm.movieIds.length > 0;
  }, "Please select at least one movie.");

  let validator = $("#newRental").validate({
    submitHandler: function (e) {
      e.preventDefault();
      $.ajax({
        url: "/api/rentals",
        method: "post",
        data: vm
      })
        .done(function () {
          toastr.success("Rentals successfully recorded.");
          $("#customer").typeahead("val", "");
          $("#movie").typeahead("val", "");
          $("#movies").empty();
          vm = { movieIds: [] };
          validator.resetForm();
          $("#movie-rental-data-table > tbody").empty();
        })
        .fail(function (request, message, error) {
          Helpers.handleException(request, message, error)
        });

      return false;
    }
  });
});