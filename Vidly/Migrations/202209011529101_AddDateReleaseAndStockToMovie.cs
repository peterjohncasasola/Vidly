namespace Vidly.Migrations
{
  using System.Data.Entity.Migrations;

  public partial class AddDateReleaseAndStockToMovie : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Movies", "DateRelease", c => c.DateTime(nullable: false));
            AddColumn("dbo.Movies", "Stock", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Movies", "Stock");
            DropColumn("dbo.Movies", "DateRelease");
        }
    }
}
