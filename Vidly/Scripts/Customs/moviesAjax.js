import {QueryObject} from "./Models/queryObject.js";
import { Helpers } from './helpers.js'


const form = document.querySelector("#modal-form");
const btnModal = document.querySelector("#btn-show-modal");
const table = document.querySelector("#table");
let currentPage = 1;
let queryParams = new QueryObject(currentPage, 15);

$("#input-search").keyup(function (e) {
  queryParams.search = e.target.value;
  if (e.key === 'Enter') getMovies(queryParams);
});

$("#btn-search").click(function () {
  if (queryParams.search) {
    getMovies(queryParams);
  }
});

$('#filter-by').change(function (e) {
  queryParams.searchBy = e.target.value;
  console.log(queryParams);
});


window.addEventListener('load', () => {
  getMovies(queryParams);
})

btnModal.addEventListener('click', () => {
  formClear();
  $('#modal-label').text('New Customer');
  $('#modal-dialog').modal('show');
  $("#btn-submit").text("Save");
})

form.addEventListener('submit', (e) => {
  e.preventDefault();
  const name = document.querySelector("#customer-name").value,
    address = document.querySelector("#customer-address").value,
    membershipTypeId = document.querySelector("#membership-type").value,
    birthDate = document.querySelector("#birthdate").value,
    id = document.querySelector("#customer-id").value

  const customer = new Customer(id,name, address, membershipTypeId, birthDate, true);
  saveCustomer(customer);
});


function saveCustomer(customer) {
  if (!customer.id) {
    customer.id = 0;
    $.ajax({
      url: "/api/customers",
      type: 'POST',
      contentType: "application/json;charset=UTF-8",
      data: JSON.stringify(customer),
      success: function (response) {
        $(".toast-body").text(`Successfully saved.`);
        getMovies(queryParams);
        $("#notification").show();
        $('#modal-dialog').modal('hide');
        setTimeout(() => {
          $("#notification").hide();
        },5000)
      },
      error: function (request, message, error) {
        handleException(request, message, error);
      }
    });
  } else {
    $.ajax({
      url: `/api/customers/${customer.id}`,
      type: 'PUT',
      contentType:
        "application/json;charset=UTF-8",
      data: JSON.stringify(customer),
      success: function (response) {
        $(".toast-body").text(`Successfully updated.`);
        $("#notification").show();
        $('#modal-dialog').modal('hide');
        getMovies(queryParams);
        setTimeout(() => {
          $("#notification").hide();
        },5000)
      },
      error: function (request, message, error) {
        handleException(request, message, error);
      }
    })
  }
 }
 
 
function addCustomer(customer) {
  appendToTable(customer);
  formClear();
}

function appendToTable(customer) {

  if ($("#customer-table tbody").length > 0) {
  $("#customer-table").append("<tbody></tbody>");
    $("#customer-table tbody").remove();
  }

  $("#customer-table tbody").append(
    buildTableRow(customer));
}

function buildTableRow(customer) {
  const output = `
    <tr>
      <td class="text-truncate" style="max-width: 400px">
        ${customer.name}
      </td>
      <td class="text-truncate" style="max-width: 300px;">
        ${customer.address ?? ''}
      </td>
      <td class="text-truncate" style="max-width: 150px;">
        ${ formatDate(customer.birthDate) }
      </td>
      <td class="text-truncate" style="max-width: 200px;">
        ${customer.membershipType?.name }
      </td>
      <td style="max-width: 150px;">
        <a onclick="getCustomer(${customer.id})" class="btn btn-info btn-sm px-3 btn-edit" data-id="${customer.id}">Edit</a>|
        <a onclick="showDelete(${customer.id})" class="btn btn-danger btn-sm px-3 btn-delete" data-id="${customer.id}">Delete</a>
      </td>
    </tr>
  `;
  return output;
}

function handleException(request, message, error) {
  let msg = "";
  msg += "Code: " + request.status + "\n";
  msg += "Text: " + request.statusText + "\n";
  if (request.responseJSON != null) {
    msg += "Message" + request.responseJSON.Message + "\n";
  }
  alert(msg);
}


function getMovies(params, onlyAvailable = false) {
  $.ajax({
    url: `/api/customers?${Helpers.setQueryParams(params)}&onlyAvailable=${onlyAvailable}`,
    type: 'GET',
    dataType: 'json',
    success: function ({meta,data}) {
      queryParams.page = meta.currentPage;
      renderPaginationLink(meta);
      customerListSuccess(data);
    },
    error: function (request, message, error) {
      handleException(request, message, error);
    }
  });
}
function customerListSuccess(products) {
  document.querySelector('#customer-table-body').innerHTML = '';
  // Iterate over the collection of data
  $.each(products, function (index, product) {
    productAddRow(product);
  });
}

function getCustomer(id) {
  $("#customer-id").val(id);
  $("#btn-submit").text('Update');
  
  $.ajax({
    url: `/api/movies/${id}`,
    type: 'GET',
    dataType: 'json',
    success: function (response) {
      customerToFields(response);
      $("#modal-label").text(response.name);
      $("#btn-submit").text("Update");
      $("#modal-dialog").modal('show');
    },
    error: function (request, message, error) {
      handleException(request, message, error);
    }
  });
}

$("#btn-delete").on('click',function () {
  let id = $("#delete-id").val();
  $.ajax({
    url: `/api/customers/${id}`,
    type: 'DELETE',
    dataType: 'json',
    success: function () {
      getMovies(queryParams);
      $(".toast-body").text(`Successfully deleted.`);
      $("#notification").show();
      $("#modal-delete-dialog").modal('hide');
      setTimeout(() => {
        $("#notification").hide();
      },5000)
    },
    error: function (request, message, error) {
      handleException(request, message, error);
    }
  });
})



function showDelete(id) {
  $("#delete-id").val(id);
  $("#modal-delete-dialog").modal('show');
}

function productAddRow(product) {
  // Append row to <table>
  $("#customer-table-body").append(
    buildTableRow(product));
}

function formClear() {
  document.querySelector("#customer-name").value = '';
  document.querySelector("#customer-address").value = '';
  document.querySelector("#membership-type").value = '';
  document.querySelector("#birthdate").value = '';
  document.querySelector("#customer-id").value = '';

}

function customerToFields(customer) {
  console.log(customer);
  document.querySelector("#customer-name").value = customer.name;
  document.querySelector("#customer-address").value = customer.address;
  document.querySelector("#membership-type").value = customer.membershipTypeId;
  document.querySelector("#birthdate").value = new Date(customer.birthDate).toISOString().slice(0, 10);
  document.querySelector("#customer-id").value = customer.id;
}
