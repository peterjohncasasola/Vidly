using System;
using System.Web.Mvc;
using Vidly.Customs.Extensions.Models;
using Vidly.Customs.Extensions.Services;

namespace Vidly.Customs.Extensions.Models
{
  public class Links
  {
    public Uri Next { get; set; }
    public Uri Last { get; set; }
    public Uri First { get; set; }
    public Uri Previous { get; set; }
  }
}

public class ResponseHelper : Controller
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

  //public static PaginatedResult ToPagedResponse(QueryObject query, int totalRecords, object result, string route = "")
  //{
  //  var pageSize = query.PageSize <= 0 ? totalRecords : query.PageSize;

  //  var totalPages = Convert.ToInt32(Math.Ceiling(((double)totalRecords / (double)pageSize)));

  //  var page = query.Page <= totalPages ? query.Page : totalPages;

  //  var to = page * pageSize;

  //  var from = ((to) - pageSize) + 1;

  //  return new PaginatedResult()
  //  {
  //    Data = result,
  //    Meta = new Meta()
  //    {
  //      Path = uriService.GetPageUri(route),
  //      CurrentPage = query.Page,
  //      LastPage = totalPages,
  //      TotalItems = totalRecords,
  //      From = from > totalRecords ? totalRecords : from,
  //      To = to > totalRecords ? totalRecords : to,
  //      PerPage = query.PageSize

  //    },
  //    Links = new Links()
  //    {
  //      Next =
  //      query.Page >= 1 && query.Page < totalPages
  //      ? uriService.GetPageUri(new PaginationFilter(query.Page + 1, query.PageSize), route)
  //          : null,
  //      Previous =
  //          query.Page - 1 >= 1 && query.Page <= totalPages
  //              ? uriService.GetPageUri(new PaginationFilter(query.Page - 1, query.PageSize), route)
  //              : null,
  //      First = uriService.GetPageUri(new PaginationFilter(1, query.PageSize), route),
  //      Last = uriService.GetPageUri(new PaginationFilter(totalPages, query.PageSize), route),
  //    },
  //  };
  //}
}