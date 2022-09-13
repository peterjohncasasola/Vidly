using System;
using System.Web.Mvc;
using Vidly.Customs.Models;

namespace Vidly.Customs.Helpers
{
  public class ResponseHelper
  {
    public static PaginatedResult ToPagedResponse(QueryObject query, int totalRecords, object result, string route = null)
    {
      var pageSize = query.PageSize <= 0 ? totalRecords : query.PageSize;

      var totalPages = Convert.ToInt32(Math.Ceiling(((double)totalRecords / (double)pageSize)));

      var page = query.Page <= totalPages ? query.Page : totalPages;

      var to = page * pageSize;
      var from = totalRecords > 0 ? ((to) - pageSize) + 1 : 0;

      return new PaginatedResult()
      {
        Data = result,
        Meta = new Meta()
        {
          Path = route,
          CurrentPage = page,
          LastPage = totalPages,
          TotalItems = totalRecords,
          TotalPages = (int) Math.Ceiling((decimal)((decimal)totalRecords / query.PageSize)),
          From = from > totalRecords ? totalRecords : from,
          To = to > totalRecords ? totalRecords : to,
          PerPage = pageSize

        }
      };
    }
  }
}