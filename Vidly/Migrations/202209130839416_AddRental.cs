namespace Vidly.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddRental : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RentalDetails",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RentalId = c.Int(nullable: false),
                        MovieId = c.Int(nullable: false),
                        IsReturned = c.Boolean(nullable: false),
                        DateReturned = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Movies", t => t.MovieId, cascadeDelete: true)
                .ForeignKey("dbo.Rentals", t => t.RentalId, cascadeDelete: true)
                .Index(t => t.RentalId)
                .Index(t => t.MovieId);
            
            CreateTable(
                "dbo.Rentals",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CustomerId = c.Int(nullable: false),
                        DateRented = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Customers", t => t.CustomerId, cascadeDelete: true)
                .Index(t => t.CustomerId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RentalDetails", "RentalId", "dbo.Rentals");
            DropForeignKey("dbo.Rentals", "CustomerId", "dbo.Customers");
            DropForeignKey("dbo.RentalDetails", "MovieId", "dbo.Movies");
            DropIndex("dbo.Rentals", new[] { "CustomerId" });
            DropIndex("dbo.RentalDetails", new[] { "MovieId" });
            DropIndex("dbo.RentalDetails", new[] { "RentalId" });
            DropTable("dbo.Rentals");
            DropTable("dbo.RentalDetails");
        }
    }
}
