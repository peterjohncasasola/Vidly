namespace Vidly.Customs.Extensions.Models
{
  public class PaginatedResult
  {
    public Meta Meta { get; set; }
    public Links Links { get; set; }
    public object Data { get; set; }
  }
}