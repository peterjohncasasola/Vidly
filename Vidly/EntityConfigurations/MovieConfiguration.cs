using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;
using Vidly.Models;

namespace Vidly.EntityConfigurations
{
  public class MovieConfiguration : EntityTypeConfiguration<Movie>
  {
    public MovieConfiguration()
    {
      HasKey(q => q.Id);

      Property(t => t.Id).HasColumnName("intMovieId");
      Property(t => t.Name).HasColumnName("strName").IsRequired();
      Property(t => t.DateRelease).HasColumnName("dtmDateRelease").IsRequired();
      Property(m => m.Genre).HasColumnName("strGenre").IsRequired();
      Property(m => m.Stock).HasColumnName("intStock").IsRequired();
      Property(m => m.MinimumRequiredAge).HasColumnName("intMinimumRequiredAge");
    }
  }
}