
import { QueryObject as FilterQuery } from './Models/queryObject.js';
import { Movie } from './Models/movie.js';
import { UIHelper } from './UI/ui.js';
import {Helpers} from "./helpers.js";

const form = document.querySelector("#modal-form");
const btnModal = document.querySelector("#btn-show-modal");
const table = document.querySelector("#data-table");
let currentPage = 1;
let filterQuery = new FilterQuery(currentPage, 10);


$(document).ready(function () {
  UIHelper.loadEventHandler(filterQuery, getMovies);
  UIHelper.showModalOnEdit(getMovie);

  $("#modal-form").validate({
    errorClass: "label-error",
    rules: {
      name: {
        required: true,
        minlength: 3,
      },
      genre: "required",
      stock: {
        required: true,
        min: 0,
      },
      minimumAge: {
        required: true,
        min: 0,
      },
      dateReleased: {
        required: true,
        date: true,
        maxDate: true
      },
  
    },
    submitHandler: function () {
      const name = $("#movie-name").val(),
          genre = $("#movie-genre").val(),
          stock = $("#movie-stock").val(),
          dateRelease = $("#date-released").val(),
          minimumRequiredAge = $("#movie-min-age").val(),
          id = document.querySelector("#movie-id").value;
      const movie = new Movie(id, name, genre, stock, dateRelease, minimumRequiredAge);
      saveMovie(movie);
      return false;
    }
  });
});

$.validator.addMethod("maxDate", function(value, element) {
  let curDate = new Date();
  let inputDate = new Date(value);
  return inputDate < curDate;
  
}, "Invalid Date!");

window.addEventListener('load', () => {
  getMovies();
})

btnModal.addEventListener('click', () => {
  formClear();
  $('#modal-label').text('New Movie');
  $('#modal-dialog').modal('show');
  $("#btn-submit").val("Save");
})

form.addEventListener('submit', (e) => {
  e.preventDefault();
});

function saveMovie(movie) {
  if (!movie.id) {
    movie.id = 0;
    $.ajax({
      url: "/api/movies",
      type: 'POST',
      contentType: "application/json;charset=UTF-8",
      data: JSON.stringify(movie),
      success: function (response, message, xhr) {
        $(".toast-body").text(`${response.name} successfully added`);
        getMovies();
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
      url: `/api/movies/${movie.id}`,
      type: 'PUT',
      contentType:
          "application/json;charset=UTF-8",
      data: JSON.stringify(movie),
      success: function (response) {
        $(".toast-body").text(`Successfully updated.`);
        $("#notification").show();
        $('#modal-dialog').modal('hide');
        getMovies();
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

function buildTableRow(movie) {
  return `
    <tr>
      <td class="text-truncate" style="max-width: 400px">
        ${movie.name}
      </td>
      <td class="text-truncate" style="max-width: 300px;">
        ${movie.genre ?? ''}
      </td>
      <td class="text-truncate" style="max-width: 150px;">
        ${ Helpers.formatDate(movie.dateRelease)}
      </td>
      <td class="text-truncate" style="max-width: 200px;">
        ${movie.stock}
      </td>
      <td class="text-truncate" style="max-width: 200px;">
        ${movie.minimumRequiredAge}
      </td>
      <td style="max-width: 150px;">
        <a class="btn ms-1 btn-info btn-sm btn-edit" data-id="${movie.id}"><i class="fa fa-solid fa-pencil"></i></a> 
        <a class="btn btn-danger btn-sm btn-delete" data-id="${movie.id}"><i class="fa fa-solid fa-trash"></i></a>
      </td>
    </tr>
  `;
}



function getMovies() {
  $.ajax({
    url: `/api/movies?${Helpers.setQueryParams(filterQuery)}`,
    type: 'GET',
    dataType: 'json',
    success: function ({meta,data}) {
      filterQuery.page = meta.currentPage;
      UIHelper.renderPaginationLink(meta);
      customerListSuccess(data);
    },
    error: function (request, message, error) {
      Helpers.handleException(request, message, error);
    }
  });
}



function customerListSuccess(products) {
  document.querySelector('#data-table-body').innerHTML = '';
  UIHelper.showLoading()
  // Iterate over the collection of data
  $.each(products, function (index, product) {
    productAddRow(product);
  });

  setTimeout(() => UIHelper.hideLoading(), 500);
}

function getMovie(id) {
  $("#movie-id").val(id);
  $("#btn-submit").val('Update');

  $.ajax({
    url: `/api/movies/${id}`,
    type: 'GET',
    dataType: 'json',
    success: function (response) {
      movieToFields(response);
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
    url: `/api/movies/${id}`,
    type: 'DELETE',
    dataType: 'json',
    success: function () {
      getMovies();
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
  document.querySelector("#movie-name").value = '';
  document.querySelector("#movie-min-age").value = '';
  document.querySelector("#movie-stock").value = '';
  document.querySelector("#date-released").value = '';
  document.querySelector("#movie-genre").value = '';
}

function movieToFields(movie) {
  document.querySelector("#movie-name").value = movie.name;
  document.querySelector("#movie-min-age").value = movie.minimumRequiredAge;
  document.querySelector("#movie-stock").value = movie.stock;
  $("#date-released").val(Helpers.formatDate(movie.dateRelease, "mm/dd/yyyy"));
  document.querySelector("#movie-genre").value = movie.genre;
}
