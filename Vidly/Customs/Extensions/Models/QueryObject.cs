namespace Vidly.Customs.Extensions.Models
{
  public class QueryObject
  {
    public string SortBy { get; set; } = "desc";
    public string OrderBy { get; set; } = "Id";
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 15;
    public bool Active { get; set; } = true;
    public string SearchBy { get; set; } = "";
    public string Search { get; set; } = "";
    public string Comparison { get; set; } = "eq";
    public string Fields { get; set; } = "*";
  }

}