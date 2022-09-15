namespace Vidly.Migrations
{
  using System;
  using System.Data.Entity.Migrations;
  using Vidly.Models;
  using Bogus;
  using System.Collections.Generic;
  public partial class DatabaseSeeder : DbMigration
  {
    public override void Up()
    {
      var dbContext = new AppDbContext();
      var faker = new Faker();

      //var membershipTypes = new List<MembershipType>()
      //   {
      //       new MembershipType()
      //       {
      //           Id = 1,
      //           Name = "Pay as You Go",
      //           DiscountRate = faker.Random.Byte(1, 20),
      //           DurationInMonths = faker.Random.Byte(1, 12),
      //           SignUpFee = faker.Random.Byte(50, 200)
      //       },
      //       new MembershipType()
      //       {
      //           Id = 2,
      //           Name = "Monthly",
      //           DiscountRate = faker.Random.Byte(1, 20),
      //           DurationInMonths = faker.Random.Byte(1, 12),
      //           SignUpFee = faker.Random.Byte(50, 200)
      //       },
      //       new MembershipType()
      //       {
      //           Id = 3,
      //           Name = "Quarterly",
      //           DiscountRate = faker.Random.Byte(1, 20),
      //           DurationInMonths = faker.Random.Byte(1, 12),
      //           SignUpFee = faker.Random.Byte(50, 200)
      //       }

      //   };

      //dbContext.MembershipTypes.AddRange(membershipTypes);
      //dbContext.SaveChanges();

      var customers = new Faker<Customer>()
          .RuleFor(c => c.Name, (f, u) => $"{f.Name.FirstName()} {f.Name.LastName()}")
          .RuleFor(c => c.Address, f => f.Address.FullAddress())
          .RuleFor(c => c.BirthDate, f => f.Date.Between(new DateTime(1950, 1, 1), DateTime.Now))
          .RuleFor(c => c.IsSubscribedToNewsLetter, f => f.Random.Bool())
          .RuleFor(c => c.MembershipTypeId, f => f.Random.Int(1, 3));

      dbContext.Customers.AddRange(customers.Generate(200000));


      //var dummy = new Faker<Movie>()
      //    .RuleFor(c => c.Name, (f, u) => f.Random.Word())
      //    .RuleFor(c => c.Stock, f => f.Random.Int(10, 200))
      //    .RuleFor(c => c.DateRelease, f => f.Date.Recent())
      //    .RuleFor(c => c.MinimumRequiredAge, f => f.Random.Int(0, 21))
      //    .RuleFor(c => c.Genre, f => f.Music.Genre());

      //dbContext.Movies.AddRange(dummy.Generate(1000));

      dbContext.SaveChanges();
    }

    public override void Down()
    {
    }
  }
}
