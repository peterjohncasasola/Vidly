namespace Vidly.Migrations
{
  using System;
  using System.Data.Entity.Migrations;

  public partial class DatabaseSeeder : DbMigration
  {
    public override void Up()
    {
      Sql(@"INSERT INTO [dbo].[MembershipTypes]
        ([Name]
        ,[SignUpFee]
        ,[DurationInMonths]
        ,[DiscountRate])
         VALUES
       ('Membership 1'
       ,'1000'
       ,'12'
       ,'10')");

      Sql(@"INSERT INTO [dbo].[MembershipTypes]
        ([Name]
        ,[SignUpFee]
       ,[DurationInMonths]
       ,[DiscountRate])
        VALUES
       ('Membership  2'
       ,'1800'
       ,'30'
       ,'15')");

      for (var i = 50; i <= 100; i++)
      {
        Sql($@"INSERT INTO [dbo].[Customers] ([Name],[IsSubscribedToNewsLetter],[MembershipTypeId]) 
        VALUES('Customer {i.ToString()}',1 ,{new Random().Next(9, 10)})");
      }
    }


    public override void Down()
    {
    }
  }
}
