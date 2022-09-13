namespace Vidly.Customs.Models
{
  public class Meta
  {
    public int? CurrentPage { get; set; }
    public int? LastPage { get; set; }
    public int? From { get; set; }
    public int? To { get; set; }
    public int? PerPage { get; set; }
    public string Path { get; set; }
    public int TotalItems { get; set; }
    public int TotalPages { get; set; }
  }
}