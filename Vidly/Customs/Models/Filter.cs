namespace Vidly.Customs.Models
{
  public class Filter
  {
    public string PropertyName { get; set; }
    public string Operation { get; set; }
    public object Value { get; set; }
  }
}