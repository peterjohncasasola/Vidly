using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using Vidly.EntityConfigurations;

namespace Vidly.Models
{
  public class AppDbContext : IdentityDbContext<ApplicationUser>
  {
        public AppDbContext() : base("Vidly")
        {
          Configuration.LazyLoadingEnabled = false;
        }
        
        public DbSet<MembershipType> MembershipTypes { get; set; }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Rental> Rentals { get; set; }
        public DbSet<RentalDetail> RentalDetails { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
          Database.SetInitializer<AppDbContext>(null);

          modelBuilder.Entity<MovieGenre>().HasKey(t => new { t.Id });

          modelBuilder.Configurations.Add(new CustomerConfiguration());
          modelBuilder.Configurations.Add(new MovieConfiguration());
          
          modelBuilder.Configurations.Add(new RentalConfiguration());
          modelBuilder.Configurations.Add(new RentalDetailConfiguration());

          base.OnModelCreating(modelBuilder);
        }

    public static AppDbContext Create() => new AppDbContext();
  }
  
      
}