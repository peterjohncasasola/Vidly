
class FilterQuery {
  constructor(page, pageSize, search, searchBy, comparison, orderBy, sortBy) {
    this.page = page;
    this.pageSize = pageSize;
    this.searchBy = searchBy;
    this.search = search;
    this.comparison = comparison;
    this.orderBy = orderBy;
    this.sortBy = sortBy;
  }
}

const form = document.querySelector("#modal-form");
const btnModal = document.querySelector("#btn-show-modal");
const table = document.querySelector("#customer-table");
let currentPage = 1;
let filterQuery = new FilterQuery(currentPage, 15);

$("#input-search").keyup(function (e) {
  filterQuery.page = 1;
  filterQuery.search = e.target.value;
  if (e.key === 'Enter') getCustomers(filterQuery);
});

$("#btn-search").click(function () {
  filterQuery.page = 1;
  if (filterQuery.search) {
    getCustomers(filterQuery);
  }
});

$('#filter-by').change(function (e) {
  filterQuery.searchBy = e.target.value;
  console.log(filterQuery);
});


window.addEventListener('load', () => {
  getCustomers(filterQuery);
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

class Customer {
  constructor(id, name, address, membershipTypeId, birthDate, isSubscribedToNewsLetter) {
    this.id = id;
    this.name = name;
    this.address = address;
    this.membershipTypeId = membershipTypeId;
    this.birthDate = birthDate;
    this.isSubscribedToNewsLetter = isSubscribedToNewsLetter;
  }
}

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
        getCustomers(filterQuery);
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
        getCustomers(filterQuery);
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
  return `
    <tr>
      <td class="text-truncate" style="max-width: 400px">
        ${customer.name}
      </td>
      <td class="text-truncate" style="max-width: 300px;">
        ${customer.address ?? ''}
      </td>
      <td class="text-truncate" style="max-width: 150px;">
        ${formatDate(customer.birthDate)}
      </td>
      <td class="text-truncate" style="max-width: 200px;">
        ${customer.membershipType?.name}
      </td>
      <td style="max-width: 150px;">
        <a onclick="getCustomer(${customer.id})" class="btn btn-info btn-sm px-3 btn-edit" data-id="${customer.id}">Edit</a>|
        <a onclick="showDelete(${customer.id})" class="btn btn-danger btn-sm px-3 btn-delete" data-id="${customer.id}">Delete</a>
      </td>
    </tr>
  `;
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

function searchCustomer(params) {
  $.ajax({
    url: `/api/customers?${setQueryParams(params)}`,
    type: 'GET',
    dataType: 'json',
    success: function (response) {
      currentPage = response.meta.currentPage;
      renderPaginationLink(response.meta);
      customerListSuccess(response.data);
    },
    error: function (request, message, error) {
      handleException(request, message, error);
    }
  });
}

function getCustomers(params) {
  
  $.ajax({
    url: `/api/customers?${setQueryParams(params)}`,
    type: 'GET',
    dataType: 'json',
    success: function ({meta,data}) {
      filterQuery.page = meta.currentPage;
      renderPaginationLink(meta);
      customerListSuccess(data);
    },
    error: function (request, message, error) {
      handleException(request, message, error);
    }
  });
}



function renderPaginationLink(pagination) {
  const { totalPages, currentPage } = pagination;
  let output = ``;
  const lastPage = `
      <li class="page-item">
        <button onclick="paginateCustomers(${totalPages})" class="page-link">${totalPages}</button>
        </li>
      `;
  const firstPage = `
      <li class="page-item">
        <button  onclick="paginateCustomers(${1})" class="page-link">1</button>
        </li>
      `;
  const previousPage = `
     <li class="page-item ${currentPage === 1 ? "disabled" : ""}">
        <button onclick="paginateCustomers(${currentPage - 1})" class="page-link">Previous</button>
        </li>
      `;

  const nextPage = `
      <li class="page-item ${currentPage === totalPages ? "disabled" : ""}">
        <button onclick="paginateCustomers(${currentPage + 1})" class="page-link">Next</button>
        </li>
      `;
  
  const disabledPage = `
      <li class="page-item">
        <button class="page-link disabled">...</button>
        </li>
      `;

  if (totalPages > 4) {
    if (currentPage < 5) {
      output+= previousPage;
      for (let i = 1; i <= 5; i++) {
        output += `
      <li class="page-item ${currentPage === i ? "active" : ""}">
        <button onclick="paginateCustomers(${i})" class="page-link" id="btn-page-${i}">${i}</button>
        </li>`;
      }
      output += disabledPage;
      output += lastPage;
      output += nextPage;
    }
    else  
    {
      const endLimit = totalPages - 5;
      if (currentPage > endLimit) {
        output+= previousPage;
        output += firstPage;
        output += disabledPage;
        for (let i = endLimit; i <= totalPages; i++) {
          output += `
            <li class="page-item ${currentPage === i ? "active" : ""}" > 
              <button onclick="paginateCustomers(${i})" class="page-link" >${i}</button>
              </li>`;
        }
        output += nextPage;

      }
      else 
      {
        output+= previousPage;
        output += firstPage;
        output += disabledPage;
        for (let i = currentPage -1; i <= currentPage + 1; i++) {
            output += `
            <li class="page-item ${currentPage === i ? "active" : ""}">
              <button onclick="paginateCustomers(${i})" class="page-link"
              >${i}</button>
            </li>`;     
        }
        output += disabledPage;
        output += lastPage;
        output += nextPage;
      }
    }
  }
  else 
  {
    output+= previousPage;
    for (let i = 1; i <= totalPages; i++) {
      output += `
      <li class="page-item ${currentPage === i ? "active" : ""}" >
        <button onclick="paginateCustomers(${i})" class="page-link" >${i}</button>
        </li>`;
    }
    output+=nextPage;
  }
  
  document.querySelector('#pagination-link').innerHTML = output;
}

function paginateCustomers(page) {
  filterQuery.page = page;
  getCustomers(filterQuery);
}

function formatDate(date, format = 'MM/DD/YYYY') {
  return date == null ? null : new moment(date).format(format);
}

function setQueryParams(query) {
  let queryParams = [];

  for (const key in query) {
    if (query[key] !== null && query[key] !== undefined && query[key] !== '' && query[key] !== 0) {
      if (Array.isArray(query[key])) {
        if (query[key].length > 0) {
          for (const val of query[key])
            if (val !== null && val !== undefined && val !== '' && val !== 0) queryParams.push(`${key}=${val}`);
        }
      } else {
        queryParams.push(`${key}=${query[key]}`);
      }
    }
  }
  return queryParams.join('&');
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
      getCustomers(filterQuery);
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
