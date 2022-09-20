using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vidly.Models
{
  public class MovieGenre
  {
    public int Id { get; set; }
    public int MovieId { get; set; }
    public Movie Movie { get; set; }
    public int GenreId { get; set; }
    public Genre Genre { get; set; }
  }
}