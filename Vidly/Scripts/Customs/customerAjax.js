
import { QueryObject as FilterQuery } from './Models/queryObject.js';
import { Customer } from './Models/customer.js';
import { UI } from './UI/ui.js';
import {Helpers} from "./helpers.js";

const form = document.querySelector("#modal-form");
const btnModal = document.querySelector("#btn-show-modal");
const table = document.querySelector("#data-table");
let currentPage = 1;
let filterQuery = new FilterQuery(currentPage, 10);


$(document).ready(function () {
  UI.loadEvents(filterQuery, getCustomers);
  UI.showModalOnEdit(getCustomer);
  getMembershipTypes();
  
  $("#modal-form").validate({
    errorClass: "label-error",
    rules: {
      name: {
        required: true,
        minlength: 3,
      },
      birthdate: {
        required: true,
      },
      membershipType: {
        required: true,
      }
    },
    submitHandler: function () {
      const name = document.querySelector("#customer-name").value,
        address = document.querySelector("#customer-address").value,
        membershipTypeId = $("#membership-type").val(),
        birthDate = document.querySelector("#birthdate").value,
        id = document.querySelector("#customer-id").value,
        isSubscribedToNewsLetter = $("#checkbox-subscribe").is(":checked");
        const customer = new Customer(id, name, address, membershipTypeId, birthDate, isSubscribedToNewsLetter);
        saveCustomer(customer);
        return false;
    }
  });
});


window.addEventListener('load', () => {
  getCustomers();
})

btnModal.addEventListener('click', () => {
  formClear();
  $('#modal-label').text('New Customer');
  $('#modal-dialog').modal('show');
  $("#btn-submit").val("Save");
})

form.addEventListener('submit', (e) => {
  e.preventDefault();
});

function saveCustomer(customer) {
  if (!customer.id) {
    customer.id = 0;
    $.ajax({
      url: "/api/customers",
      type: 'POST',
      contentType: "application/json;charset=UTF-8",
      data: JSON.stringify(customer),
      success: function (response, message, xhr) {
        $(".toast-body").text(`${response.name} successfully added`);
        getCustomers();
        $("#notification").show();
        $('#modal-dialog').modal('hide');
        setTimeout(() => {
          $("#notification").hide();
        },5000)
      },
      error: function (request, message, error) {
        console.log(request,message);
        Helpers.handleException(request, message, error);
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
        getCustomers();
        setTimeout(() => {
          $("#notification").hide();
        },5000)
      },
      error: function (request, message, error) {
        Helpers.handleException(request, message, error);
      }
    })
  }
 }
 
function appendToTable(customer) {

  if ($("#data-table-tbody").length > 0) {
  $("#data-table").append("<tbody></tbody>");
    $("#data-table tbody").remove();
  }

  $("#data-table-tbody").append(
    buildTableRow(customer));
}

function buildTableRow(customer) {
  return `
    <tr>
      <td class="text-truncate" style="max-width: 400px">
        ${customer.name}
      </td>
      <td class="text-truncate" style="max-width: 300px;">
        ${customer.address ?? ''}
      </td>
      <td class="text-truncate" style="max-width: 150px;">
        ${ Helpers.formatDate(customer.birthDate)}
      </td>
      <td class="text-truncate" style="max-width: 200px;">
        ${customer.membershipType?.name}
      </td>
      <td style="max-width: 150px;">
        <a class="btn ms-1 btn-info btn-sm btn-edit" data-id="${customer.id}"><i class="fa fa-solid fa-pencil"></i></a> 
        <a class="btn btn-danger btn-sm btn-delete" data-id="${customer.id}"><i class="fa fa-solid fa-trash"></i></a>
      </td>
    </tr>
  `;
}



function getCustomers() {
  $.ajax({
    url: `/api/customers?${Helpers.setQueryParams(filterQuery)}`,
    type: 'GET',
    dataType: 'json',
    success: function ({meta,data}) {
      filterQuery.page = meta.currentPage;
      UI.renderPaginationLink(meta);
      customerListSuccess(data);
    },
    error: function (request, message, error) {
      Helpers.handleException(request, message, error);
    }
  });
}

function getMembershipTypes() {

  $.ajax({
    url: `/api/membership-types`,
    type: 'GET',
    dataType: 'json',
    success: function (response) {
      let output = `<option value="">Select Membership Type</option>`;

      if (response) {
        response.forEach((membershipType) => {
          output += `<option value="${membershipType.id}">${membershipType.name}</option>`
        });
      }
      $("#membership-type").append(output);
    },
    error: function (request, message, error) {
      Helpers.handleException(request, message, error);
    }
  });
}




function customerListSuccess(products) {
  document.querySelector('#data-table-body').innerHTML = '';
  UI.showLoading()
  // Iterate over the collection of data
  $.each(products, function (index, product) {
    productAddRow(product);
  });
  
  setTimeout(() => UI.hideLoading(), 500);
}

function getCustomer(id) {
  $("#customer-id").val(id);
  $("#btn-submit").val('Update');
  
  $.ajax({
    url: `/api/customers/${id}`,
    type: 'GET',
    dataType: 'json',
    success: function (response) {
      customerToFields(response);
      $("#modal-label").text(response.name);
      $("#btn-submit").text("Update");
      $("#modal-dialog").modal('show');
    },
    error: function (request, message, error) {
      Helpers.handleException(request, message, error);
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
      getCustomers();
      $(".toast-body").text(`Successfully deleted.`);
      $("#notification").show();
      $("#modal-delete-dialog").modal('hide');
      setTimeout(() => {
        $("#notification").hide();
      },5000)
    },
    error: function (request, message, error) {
      Helpers.handleException(request, message, error);
    }
  });
})


function productAddRow(product) {
  // Append row to <table>
  $("#data-table-body").append(
    buildTableRow(product));
}

function formClear() {
  document.querySelector("#customer-name").value = '';
  document.querySelector("#customer-address").value = '';
  document.querySelector("#membership-type").value = '';
  document.querySelector("#birthdate").value = '';
  document.querySelector("#customer-id").value = '';
  $("#checkbox-subscribe").prop("checked", false);
}

function customerToFields(customer) {
  console.log(customer);
  document.querySelector("#customer-name").value = customer.name;
  document.querySelector("#customer-address").value = customer.address;
  document.querySelector("#membership-type").value = customer.membershipTypeId;
  document.querySelector("#birthdate").value = new Date(customer.birthDate).toISOString().slice(0, 10);
  document.querySelector("#customer-id").value = customer.id;
  $("#checkbox-subscribe").prop('checked', customer.isSubscribedToNewsLetter);
}
