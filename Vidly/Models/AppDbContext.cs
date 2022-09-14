using System;
using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Vidly.Models
{
  public class AppDbContext : IdentityDbContext<ApplicationUser>
  {
        public AppDbContext() : base("Vidly")
        {
            this.Configuration.LazyLoadingEnabled = false;
        }
        
        public DbSet<MembershipType> MembershipTypes { get; set; }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Rental> Rentals { get; set; }
        public DbSet<RentalDetail> RentalDetails { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
          Database.SetInitializer<AppDbContext>(null);
          base.OnModelCreating(modelBuilder);
        }

    public static AppDbContext Create()
    {
      return new AppDbContext();
    }
  }
  
      
}