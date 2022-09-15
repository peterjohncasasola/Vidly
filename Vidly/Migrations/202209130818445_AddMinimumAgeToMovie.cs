namespace Vidly.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddMinimumAgeToMovie : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Movies", "MinimumRequiredAge", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Movies", "MinimumRequiredAge");
        }
    }
}
