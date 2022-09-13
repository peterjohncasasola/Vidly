
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



const table = document.querySelector("#data-table");

let currentPage = 1,
    filterQuery = new FilterQuery(currentPage, 15),
    isReturned = false;

$(document).ready(function () {
  getCustomers(filterQuery);
});

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

$("#input-status").on("change", function () {
 isReturned = $(this).is(':checked');
 filterQuery.page = 1;
 getCustomers(filterQuery);
 });


function saveCustomer(customer) {
  if (!customer.id) {
    customer.id = 0;
    $.ajax({
      url: "/api/rentals",
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
        }, 5000)
      },
      error: function (request, message, error) {
        
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
        }, 5000)
      },
      error: function (request, message, error) {
        
      }
    })
  }
}



function addCustomer(customer) {
  appendToTable(customer);
  formClear();
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
        ${data.rental.customer.name}
      </td>
      <td>
        ${data.movie.name}
      </td>
      <td>
        ${data.movie.genre}
      </td>

      <td>
        ${formatDate(data.rental.dateRented)}
      </td>
      <td>
        ${formatDate(data.dateReturned) ?? 'Not yet returned'}
      </td>
      <td>
        ${renderButton(data)}
      </td>
    </tr>
  `;
}

function renderButton(data) {
  if (!data.isReturned) {
    return `<button class="btn btn-primary btn-sm px-3 js-delete" data-id="${data.id}">Return</button>`;
  }
  // return `<button disabled class="btn btn-primary btn-sm px-3 js-delete" data-id="${data.id}">Returned</button>`;
  return 'Returned';
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

function getCustomers(params) {
  params.isReturned = isReturned;
  $.ajax({
    url: `/api/rentals?${setQueryParams(params)}`,
    type: 'GET',
    dataType: 'json',
    success: function ({ meta, data }) {
      filterQuery.page = meta.currentPage;
      renderPaginationLink(meta);
      customerListSuccess(data);
    },
    error: function (request, message, error) {
      
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
      output += previousPage;
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
    else {
      const endLimit = totalPages - 5;
      if (currentPage > endLimit) {
        output += previousPage;
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
      else {
        output += previousPage;
        output += firstPage;
        output += disabledPage;
        for (let i = currentPage - 1; i <= currentPage + 1; i++) {
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
  else {
    output += previousPage;
    for (let i = 1; i <= totalPages; i++) {
      output += `
      <li class="page-item ${currentPage === i ? "active" : ""}" >
        <button onclick="paginateCustomers(${i})" class="page-link" >${i}</button>
        </li>`;
    }
    output += nextPage;
  }

  document.querySelector('#pagination-link').innerHTML = output;
}

function paginateCustomers(page) {
  filterQuery.page = page;
  getCustomers(filterQuery);

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
            getCustomers(filterQuery);
            toastr.success(`Successfully returned`);
          }
        });
      }
    });
  });
})


