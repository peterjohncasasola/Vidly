using System;
using System.ComponentModel.DataAnnotations;
using Vidly.Customs.Data_Annotations;

namespace Vidly.Models
{
  public class Movie
  {
    [Key]
    public int Id { get; set; }
    
    [Required]
    [StringLength(255)]
    public string Name { get; set; }
    
    [Required]
    public string Genre { get; set; }

    [Display(Name = "Date Release")]
    [DateRange]
    [DisplayFormat(DataFormatString = "{0:MMMM d, yyyy}")]
    public DateTime DateRelease { get; set; }
    [Required]
    public int Stock { get; set; }
    [Display(Name = "Minimum Age")]
    public int MinimumRequiredAge { get; set; }
  }

  public enum Genre
  {
    Drama,
    Comedy,
    Action,
    Fantasy,
    Horror,
    Romance,
    Western,
    Thriller,
  }
}