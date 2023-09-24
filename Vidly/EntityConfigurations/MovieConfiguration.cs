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

      Property(t => t.Id).HasColumnName("Id");
      Property(t => t.Name).HasColumnName("Name").IsRequired();
      Property(t => t.DateRelease).HasColumnName("DateRelease").IsRequired();
      Property(m => m.Genre).HasColumnName("Genre").IsRequired();
      Property(m => m.Stock).HasColumnName("Stock").IsRequired();
      Property(m => m.MinimumRequiredAge).HasColumnName("MinimumRequiredAge");
    }
  }
}