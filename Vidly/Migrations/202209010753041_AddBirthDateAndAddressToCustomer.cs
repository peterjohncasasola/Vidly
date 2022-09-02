namespace Vidly.Migrations
{
  using System.Data.Entity.Migrations;

  public partial class AddBirthDateAndAddressToCustomer : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Customers", "DateOfBirth", c => c.DateTime(nullable: false));
            AddColumn("dbo.Customers", "Address", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Customers", "Address");
            DropColumn("dbo.Customers", "DateOfBirth");
        }
    }
}
