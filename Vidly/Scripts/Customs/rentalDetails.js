import { QueryObject as FilterQuery } from './Models/queryObject.js';
import {UIHelper} from "./UI/ui.js";
import {Helpers} from "./helpers.js";

let currentPage = 1,
    filterQuery = new FilterQuery(currentPage, 15);

let filter = {
  isReturned: true,
  filterDateBy: "",
  dateFrom: null,
  dateTo: null,
}
    

$(document).ready(function () {
  UIHelper.loadEventHandler(filterQuery, getRentals);
  getRentals(filterQuery);
})



$("#input-status").on("change", function () {
  filter.isReturned = $(this).is(':checked');
  $("#label-status").text(filter.isReturned ? 'Showing Returned' : 'Showing For Returns');
 filterQuery.page = 1;
  getRentals(filterQuery);
 });

$("#filter-date-by").on("change", function (e) {
  filter.filterDateBy = e.target.value
  console.log(filter)
});

$("#dateFrom").on("change", function (e) {
  filter.dateFrom = e.target.value;
  console.log(filter)
});

$("#dateTo").on("change", function (e) {
  filter.dateTo = e.target.value;
  console.log(filter)
});


function saveRental(customer) {
  if (!customer.id) {
    customer.id = 0;
    $.ajax({
      url: "/api/rentals",
      type: 'POST',
      contentType: "application/json;charset=UTF-8",
      data: JSON.stringify(customer),
      success: function (response) {
        $(".toast-body").text(`Successfully saved.`);
        getRentals(filterQuery);
        $("#notification").show();
        $('#modal-dialog').modal('hide');
        setTimeout(() => {
          $("#notification").hide();
        }, 5000)
      },
      error: function (request, message, error) {
        Helpers.handleException(request, message, error)
      }
    });
  }
}




function appendToTable(customer) {

  if ($("#data-table-body").length > 0) {
    $("#data-table tbody").remove();
    $("#data-table").append("<tbody></tbody>");
  }

  $("#data-table-body").append(
    buildTableRow(customer));
}

function buildTableRow(data) {
  return `
    <tr>
      <td>
        ${data.rental.rentalCode ?? ''}
      </td>
      <td>
        ${data.rental.customer.name}
      </td>
      <td>
        ${data.movie.name}
      </td>
      <td>
        ${data.movie.genre}
      </td>

      <td>
        ${Helpers.formatDate(data.rental.dateRented)}
      </td>
      <td>
        ${Helpers.formatDate(data.dateReturned) ?? 'Not yet returned'}
      </td>
      <td>
        ${renderButton(data)}
      </td>
    </tr>
  `;
}

function renderButton(data) {
  if (!data.dateReturned) {
    return `<button class="btn btn-primary btn-sm js-delete" data-id="${data.id}">Return</button>`;
  }
  return 'Returned';
}


function getRentals() {
  $.ajax({
    url: `/api/rentals/details?${Helpers.setQueryParams(filterQuery)}&${Helpers.setQueryParams(filter)}`,
    type: 'GET',
    dataType: 'json',
    success: function ({ meta, data }) {
      filterQuery.page = meta.currentPage;
      
      UIHelper.renderPaginationLink(meta);
      
      customerListSuccess(data);
    },
    error: function (request, message, error) {
      Helpers.handleException(request, message, error)
    }
  });
}


function customerListSuccess(customers) {
  let output = '';
  document.querySelector('#rentals-table-body').innerHTML = '';
 
   $.each(customers, function (index, customer) {
    output += buildTableRow(customer);
  });
   $("#rentals-table-body").append(output);
}


function productAddRow(product) {
  // Append row to <table>
  $("#data-table-body").append(buildTableRow(product));
}


$(document).ready(function() {
  $("#data-table").on("click", ".js-delete", function () {
    const button = $(this);
    bootbox.confirm("Return this movie?", function (result) {
      if (result) {
        $.ajax({
          url: `/api/rentals/details/${button.attr("data-id")}`,
          method: "PUT",
          success: function (response) {
            getRentals(filterQuery);
            toastr.success(`Successfully returned`);
          }
        });
      }
    });
  });
})


