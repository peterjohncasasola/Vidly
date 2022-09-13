using System;
using Microsoft.AspNetCore.WebUtilities;

namespace Vidly.Customs.Services
{
  public class UriService 
  {
    private readonly string _baseUri;
    public UriService(string baseUri)
    {
      _baseUri = baseUri;
    }

    public string GetPageUri(string route)
    {
      var endpoint = new Uri(string.Concat(_baseUri, route));
      return endpoint.ToString();
    }
    public Uri GetPageUri(PaginationFilter filter, string route)
    {
      var endpoint = new Uri(string.Concat(_baseUri, route));
      var modifiedUri = QueryHelpers.AddQueryString(endpoint.ToString(), "page", filter.PageNumber.ToString());
      modifiedUri = QueryHelpers.AddQueryString(modifiedUri, "pageSize", filter.PageSize.ToString());
      return new Uri(modifiedUri);
    }
  }

}

