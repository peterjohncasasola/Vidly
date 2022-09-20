using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;
using Vidly.Models;

namespace Vidly.EntityConfigurations
{
  public class MembershipTypeConfiguration : EntityTypeConfiguration<MembershipType>
  {
    public MembershipTypeConfiguration()
    {
      HasKey(q => q.Id);
      HasMany(t => t.Customers);
    }
  }
}