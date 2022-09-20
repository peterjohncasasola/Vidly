import { QueryObject as FilterQuery } from './Models/queryObject.js';
import {UIHelper} from "./UI/ui.js";
import {Helpers} from "./helpers.js";

let currentPage = 1,
  filterQuery = new FilterQuery(currentPage, 15);

filterQuery.fields = 'Id, RentalCode, CustomerId, Customer.Name as CustomerName, DateRented, DateCompleted, IsCompleted';

let filter = {
  isCompleted: false,
  filterDateBy: "",
  dateFrom: null,
  dateTo: null,
}


let rentalDetails = [];

$(document).ready(function () {
  UIHelper.loadEventHandler(filterQuery, getRentals);
  getRentals();
  UIHelper.showModalOnEdit(getRental);
});



$("#input-status").on("change", function () {
  filter.isCompleted = $(this).is(':checked');
  $("#label-status").text(filter.isCompleted ? 'Showing Returned' : 'Showing For Returns');
 filterQuery.page = 1;
  getRentals();
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

function getRental(id) {
  $("#primary-id").val(id);
  $("#btn-submit").val('Update');

  $.ajax({
    url: `/api/rentals/${id}`,
    type: 'GET',
    dataType: 'json',
    success: function (response) {
      response.isCompleted ? $("#btn-save").prop('disabled', true) : $("#btn-save").prop('disabled', false);
      rentalDetails = response.rentalDetails;
      renderRentalDetails(rentalDetails)
      displayCustomerDetails(response);
      $("#modal-label").text(response.rentalCode);
      $("#btn-submit").text("Update");
      $("#modal-dialog").modal('show');
    },
    error: function (request, message, error) {
      Helpers.handleException(request, message, error);
    }
  });
}



function buildTableRow(data) {
  return `
    <tr>
      <td>
        <button class="btn btn-sm btn-link btn-edit ms-1" data-id="${data.id}">${data.rentalCode ?? ''}</button>
      </td>
      <td>
        ${data.customerName}
      </td>
      <td>
        ${Helpers.formatDate(data.dateRented, 'L LT')}
      </td>
      <td>
        ${Helpers.formatDate(data.dateCompleted, 'L LT') ?? 'Not yet completed'}
      </td>
    </tr>
  `;
}

function renderButton(data) {
  if (!data.isReturned && !data.dateReturned) {
    return `<button class="btn btn-primary btn-sm btn-delete" data-id="${data.id}">Return</button>`;
  }
  return 'Returned';
}

function renderCheckBox(data) {
  if (data.isReturned && data.dateReturned) {
    return `<input class="form-check-input select-checkbox" type="checkbox" checked onclick="return false;" data-check-id="0" style="min-width: 15px;" id="input-checkbox-${data.id}" />`;
  }
  else if (data.isReturned && !data.dateReturned) {
    return `<input class="form-check-input select-checkbox" data-check-id="${data.id}" checked  type="checkbox" style="min-width: 15px;" id="input-checkbox-${data.id}" />`;
  }

  else {
    return `<input class="form-check-input select-checkbox" data-check-id="${data.id}" type="checkbox" style="min-width: 15px;" id="input-checkbox-${data.id}" />`;
  }
}



$("#btn-save").on("click", function () {
  let payload = {
    rentalId: $("#primary-id").val(),
    rentalDetailIds: [],
  };
  $('#rental-details-data-table').find('tbody tr').each(function (index) {
    if ($(this).find("input[type='checkbox'][id^=input-checkbox]").prop('checked')) {
      let id = $(this).find("input[type='checkbox'][id^=input-checkbox]").attr('data-check-id');
      if (id > 0) {
        payload.rentalDetailIds.push(
          $(this).find("input[type='checkbox'][id^=input-checkbox]").attr('data-check-id')
        );
      }
    }
  });

  $.ajax({
    url: `/api/rentals/${payload.rentalId}`,
    type: 'PUT',
    contentType: "application/json;charset=UTF-8",
    data: JSON.stringify(payload),
    success: function (response) {
      $(".toast-body").text(`Successfully returned.`);
      getRental(payload.rentalId);
      $("#notification").show();
      setTimeout(() => {
        $("#notification").hide();
      }, 5000)
    },
    error: function (request, message, error) {
      Helpers.handleException(request, message, error)
    }
  });

});


$('#rental-details-data-table').on("click", ".select-checkbox", function () {
  
  const id = $(this).data("check-id");
  if (id > 0) {
    let index = rentalDetails.findIndex(rental => rental.id == id);
    rentalDetails[index].isReturned = !rentalDetails[index].isReturned;
  }
  
});

function getRentals() {
 
  $.ajax({
    url: `/api/rentals?${Helpers.setQueryParams(filterQuery)}&${Helpers.setQueryParams(filter)}`,
    type: 'GET',
    dataType: 'json',
    success: function ({ meta, data }) {
      filterQuery.page = meta.currentPage;
      
      UIHelper.renderPaginationLink(meta);
      
      renderRentalList(data);
    },
    error: function (request, message, error) {
      Helpers.handleException(request, message, error)
    }
  });
}

function displayCustomerDetails(data) {
  let output = '';
  output += `<thead class="fs-6 fw-normal" id="rental-header">
                <tr class="bg-light">
                  <th class="fs-6 fw-normal">
                    Date Rented :
                  </th>
                  <th  class="fs-6 fw-normal">
                    ${Helpers.formatDate(data.dateRented, 'L LT')}
                  </th>
                </tr>
                <tr class="bg-light">
                  <th class="fs-6 fw-normal">
                    Customer Name :
                  </th>
                  <th class="fs-6 fw-normal">
                    ${data.customer.name}
                  </th>
                </tr>
                <tr class="bg-light">
                  <th  class="fs-6 fw-normal">
                    Address:
                  </th>
                  <th class="fs-6 fw-normal">
                    ${data.customer.address}
                  </th>
                </tr>
               
                </thead>`;
  $("#rental-header").remove();
  $("#rental-header-data-table").append(output);
  
}

$("#search-movie").on("keyup", function (e) {
  e.preventDefault();
  let result = rentalDetails.filter(({ movie }) => {
    return movie.name.toLowerCase().includes(e.target.value.toLowerCase());
  });
  renderRentalDetails(result);
})



function movieToFields(movie) {
}


function renderRentalList(rentals) {
  let output = '';
  document.querySelector('#rentals-table-body').innerHTML = '';
  UIHelper.showLoading();
   $.each(rentals, function (index, rental) {
    output += buildTableRow(rental);
   });
  setTimeout(() => UIHelper.hideLoading(), 500);
  $("#rentals-table-body").append(output);

}

function renderRentalDetails(rentalDetails) {
  let output = '';
  document.querySelector('#rental-details-table-body').innerHTML = '';
  
  $.each(rentalDetails, function (index, data) {
    output += `
    <tr class="select-row">
      <td style="max-width: 25px;" class="select">
        <div class="form-check ms-1">
          ${renderCheckBox(data)}
        </div>
      </td>
      <td>
        ${data.movie.name}
      </td>
      <td>
        ${data.movie.genre}
      </td>
      <td>
        ${Helpers.formatDate(data.dateReturned, 'L LT') ?? 'Not yet returned'}
      </td>
      <td>
        ${renderButton(data)}
      </td>
     
    </tr>`;
  });
  $("#rental-details-table-body").append(output);
}

$('#selectAll').click(function (e) {
  $(this).closest('table').find('td input:checkbox').prop('checked', this.checked)
});

$("input[type=checkbox]").click(function () {
  console.log($(this));
  if (!$(this).prop("checked")) {
    $("#selectAll").prop("checked", false);
  }
});

$(document).ready(function() {
  $("#rental-details-data-table").on("click", ".btn-delete", function () {
    const id = $(this).attr("data-id");
    let row = $(this).parent().parent();
    var amount = $($(row).find("td")[4]).html();
    console.log(amount);
    bootbox.confirm("Return this movie?", function (result) {
      if (result) {
        $.ajax({
          url: `/api/rental/details/${id}`,
          method: "PUT",
          success: function (response) {
            $($(row).find("td")[3]).text(Helpers.formatDate(response.dateReturned, 'L LT'));
            $($(row).find("td")[4]).text('Returned');

            toastr.success(`Successfully returned`);
          }
        });
      }
    });
  });
})


