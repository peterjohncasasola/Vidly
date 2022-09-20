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

      ToTable("tblCustomers");

      Property(m => m.Id).HasColumnName("intCustomerId");
      Property(t => t.MembershipTypeId).HasColumnName("intMembershipTypeId");
      Property(m => m.Name).HasColumnName("strName").IsRequired();
      Property(t => t.IsSubscribedToNewsLetter).HasColumnName("ysnSubscribedToNewsLetter");
      Property(t => t.Address).HasColumnName("strCompleteAddress");
      Property(t => t.BirthDate).HasColumnName("dtmDateOfBirth");

      HasMany(t => t.Rentals);
    }
  }
}