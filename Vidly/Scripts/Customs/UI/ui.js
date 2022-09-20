
export class UIHelper {
    
    static loadEventHandler(filterQuery, callback) {
        $("#input-search").keyup(function (e) {
            filterQuery.page = 1;
            filterQuery.search = e.target.value;
            if (e.key === 'Enter') callback();
        });

        $("#pagination-link").on("click", ".btn-page-number", function () {
            filterQuery.page = $(this).attr("data-page-id");
            callback();
        })

        $("#btn-search").click(function () {
            filterQuery.page = 1;
            callback();
        }); 
        
        $("#select-limit").on("change", function (e) {
            filterQuery.pageSize = e.target.value;
            callback()
        });

        $('#filter-by').change(function (e) {
            filterQuery.searchBy = e.target.value;
        });

        $("#data-table").on("click", ".btn-delete", function () {
            const button = $(this);
            $("#delete-id").val(button.attr("data-id"));
            $("#modal-delete-dialog").modal('show');
        });

      $('#modal-dialog').modal({
        backdrop: 'static',
        keyboard: false
      });

     
    }
    
    static renderPaginationLink(pagination) {
        const { totalPages, currentPage } = pagination;
        let output = ``;
        const lastPage = `
        <li class="page-item">
            <button  data-page-id="${totalPages}"  class="page-link btn-page-number">${totalPages}</button>
        </li>
      `;
        const firstPage = `
        <li class="page-item">
             <button data-page-id="1" class="page-link  btn-page-number">1</button>
        </li>
      `;
        const previousPage = `
            <li class="page-item ${currentPage <= 1 ? "disabled" : ""}">
                <button data-page-id="${currentPage - 1}"  class="page-link  btn-page-number">Previous</button>
            </li>
      `;

        const nextPage = `
            <li class="page-item ${currentPage === totalPages ? "disabled" : ""}">
                <button  data-page-id="${currentPage + 1}" class="page-link btn-page-number">Next</button>
            </li>`;

        const disabledPage = `
                <li class="page-item">
                <button class="page-link disabled">...</button>
                </li>`;

        if (totalPages > 4) {
            if (currentPage < 5) {
                output += previousPage;
                for (let i = 1; i <= 5; i++) {
                    output += `
                        <li class="page-item ${currentPage === i ? "active" : ""}">
                         <button data-page-id="${i}" class="page-link btn-page-number" id="btn-page-${i}">${i}</button>
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
                          <button data-page-id="${i}" class="page-link btn-page-number" >${i}</button>
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
                          <button data-page-id="${i}" class="page-link btn-page-number"
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
                    <button data-page-id="${i}" class="page-link btn-page-number" >${i}</button>
                    </li>`;
            }
            output += nextPage;
        }

        document.querySelector('#pagination-link').innerHTML = output;
    }
    
    static showModalOnEdit(callback = null) {
        $("#data-table").on("click", ".btn-edit", function () {
           const button = $(this);
          if (callback) {
            callback(button.attr("data-id"));
          }
        });
      
    }

 

    static RemoveRow(row) {
        $(row).closest('tr').remove();
    }
    
    static showLoading() {
        let spinner =
            `<div class="overlay-spinner text-center" id="loading-spinner">
                <div class="spinner-border text-primary">
                    <span class="sr-only">Loading...</span>
                </div> 
            </div>`;
      $('.table-responsive > #data-table').append(spinner);
    }
    
    static hideLoading() {
        let element = $("#loading-spinner");
        element.remove();
    }

  static (modalId = "modal-form") {
    $(`#${modalId}`).on("submit", function (e) {
      e.preventDefault();
    });
  }
}
    
