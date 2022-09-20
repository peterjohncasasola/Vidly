using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;
using Vidly.Models;

namespace Vidly.EntityConfigurations
{
  public class RentalDetailConfiguration : EntityTypeConfiguration<RentalDetail>
  {
    public RentalDetailConfiguration()
    {
      HasKey(t => t.Id);

      HasRequired(t => t.Rental)
        .WithMany(t => t.RentalDetails)
        .HasForeignKey(t => t.RentalId);

      HasRequired(t => t.Movie)
        .WithMany(t => t.RentalDetails)
        .HasForeignKey(t => t.MovieId);

      Property(t => t.Id).HasColumnName("intRentalDetailId").IsRequired();
      Property(t => t.RentalId).HasColumnName("intRentalId").IsRequired();
      Property(t => t.MovieId).HasColumnName("intMovieId").IsRequired();
      Property(t => t.IsReturned).HasColumnName("ysnIsReturned").IsRequired();
      Property(t => t.DateReturned).HasColumnName("dtmDateReturned").IsOptional();

    }
  }
}