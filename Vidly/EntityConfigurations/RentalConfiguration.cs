using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;
using Vidly.Models;

namespace Vidly.EntityConfigurations
{
  public class RentalConfiguration : EntityTypeConfiguration<Rental>
  {
    public RentalConfiguration()
    {
      HasKey(t => t.Id);

      HasRequired(r => r.Customer)
        .WithMany(c => c.Rentals)
        .HasForeignKey(r => r.CustomerId);
      
      HasMany(r => r.RentalDetails)
        .WithRequired(r => r.Rental)
        .HasForeignKey(r => r.RentalId);

      Property(t => t.DateRented).HasColumnName("dtmDateRented").IsRequired();
      Property(t => t.CustomerId).HasColumnName("intCustomerId").IsRequired();
      Property(t => t.Id).HasColumnName("intRentalId").IsRequired();
      Property(t => t.IsCompleted).HasColumnName("ysnCompleted").IsOptional();
      Property(t => t.DateCompleted).HasColumnName("dtmDateCompleted").IsOptional();
      Property(t => t.RentalCode).HasColumnName("strTransactionCode").IsRequired();
    }
  }
}