using System;
using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Vidly.Models
{
  public class AppDbContext : IdentityDbContext <ApplicationUser>
  {
        public AppDbContext() : base("Vidly")
        {
            this.Configuration.LazyLoadingEnabled = false;
        }
        
        public DbSet<MembershipType> MembershipTypes { get; set; }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Movie> Movies { get; set; }
        // public DbSet<Rental> Rentals { get; set; }
        // public DbSet<RentalDetail> RentalDetails { get; set; }
        
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
          modelBuilder.Entity<IdentityRole>().ToTable("Roles");
          modelBuilder.Entity<IdentityUser>().ToTable("Users");
          modelBuilder.Entity<IdentityUserClaim>().ToTable("UserClaims");
          modelBuilder.Entity<IdentityUserLogin>().ToTable("UserLogins");
          modelBuilder.Entity<IdentityUserRole>().ToTable("UserRoles");
        }

    public static AppDbContext Create()
    {
      return new AppDbContext();
    }
  }
  
      
}