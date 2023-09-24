using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;
using Vidly.Models;

namespace Vidly.EntityConfigurations
{
  public class CustomerConfiguration : EntityTypeConfiguration<Customer>
  {
    public CustomerConfiguration()
    {
      HasKey(q => q.Id);
      HasRequired(t => t.MembershipType);

      ToTable("Customers");

      Property(m => m.Id).HasColumnName("Id");
      Property(t => t.MembershipTypeId).HasColumnName("MembershipTypeId");
      Property(m => m.Name).HasColumnName("Name").IsRequired();
      Property(t => t.IsSubscribedToNewsLetter).HasColumnName("IsSubscribedToNewsLetter");
      Property(t => t.Address).HasColumnName("Address");
      Property(t => t.BirthDate).HasColumnName("DateOfBirth");

      HasMany(t => t.Rentals);
    }
  }
}