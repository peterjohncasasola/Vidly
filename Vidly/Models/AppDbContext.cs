using System.Data.Entity;
using Vidly.Migrations;

namespace Vidly.Models
{
  public class AppDbContext : DbContext
    {
        public AppDbContext() : base("Vidly")
        {
            
        }
        
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<MembershipType> MembershipTypes { get; set; }
    }
}