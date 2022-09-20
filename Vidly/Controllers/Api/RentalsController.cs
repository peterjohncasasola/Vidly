using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Routing;
using Vidly.Customs.Extensions;
using Vidly.Customs.Extensions.Helpers;
using Vidly.Customs.Extensions.Models;
using Vidly.Models;
using Vidly.Models.DTO;

namespace Vidly.Controllers.Api
{
  [RoutePrefix("api/rentals")]
  public class RentalsController : ApiController
  {
    private readonly AppDbContext _db = new();
    
    [HttpGet]
    public async Task<IHttpActionResult> GetRentals([FromUri]   
      QueryObject query,
      [FromUri] DateRange dateRange,
      bool isCompleted = false, string filterDateBy = ""
      )
    {
      var rentals = _db.Rentals.Include(r => r.Customer);

      rentals = rentals.Where(r => r.IsCompleted == isCompleted);

      if (!string.IsNullOrEmpty(filterDateBy.Trim()))
      {
        var dateTo = dateRange.DateTo.AddDays(1);
        
        rentals = filterDateBy.ToUpper().Trim() switch
        {
          
          "COMPLETED" => rentals.Where(q => q.DateCompleted >= dateRange.DateFrom 
                                            && q.DateCompleted <= dateTo),

          "RENTED" => rentals.Where(q => q.DateRented >= dateRange.DateFrom 
                                         && q.DateRented <= dateTo),
          _ => rentals
        };
      }

      var result = await rentals.Filter(query).ToPaginateAsync(query);
      
      return Ok(result);
    }

    #region GetRentalById
    [Route("{id}")]
    [HttpGet]
    public async Task<IHttpActionResult> GetRentalById(int id)
    {
      try
      {
        var rental = await _db.Rentals
          .Include(t => t.Customer)
          .SingleAsync(t => t.Id == id);

        if (rental == null)
          return NotFound();

        await _db.RentalDetails
          .Where(r => r.RentalId == rental.Id)
          .Include(t => t.Movie)
          .LoadAsync();

        return Ok(rental);
      }
      catch (Exception e)
      {
        Console.WriteLine(e);
        throw;
      }
    }


    #endregion

    [HttpPost]
    public async Task<IHttpActionResult> PostRental(NewRentalDto rentalDto)
    {
      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      try
      {
        
        var customer = await _db.Customers.FindAsync(rentalDto.CustomerId);
        if (customer == null)
          return BadRequest("Customer not found");

        var movies = await _db.Movies.Where(m => rentalDto.MovieIds.Contains(m.Id)).ToListAsync();
        var rentalDetails = new List<RentalDetail>();

        if (movies.Count == 0) return BadRequest("Please add at least 1 movie");

        var rentalCode = await EfHelper.GenerateTransactionCode("RTL", "rentals", "intRentalId");
        var rental = new Rental()
        {
          RentalCode = rentalCode,
          CustomerId = rentalDto.CustomerId,
          DateRented = DateTime.Now,
          IsCompleted = false,
        };

        foreach (var movie in movies)
        {
          if (movie.Stock == 0)
            return BadRequest($"{movie.Name}- ({movie.Genre}) is not available");

          if (movie.MinimumRequiredAge > customer.Age)
            return BadRequest($"{customer.Name} does not meet the required age of movie {movie.Name}");

          movie.Stock--;
          
          var rentedMovie = new RentalDetail()
          {
            Movie = movie,
            Rental = rental
          };

          rentalDetails.Add(rentedMovie);
        }

        _db.RentalDetails.AddRange(rentalDetails);

        await _db.SaveChangesAsync();



        return Ok(rentalDetails);

      }
      catch (Exception e)
      {
        throw new Exception(e.Message);
      }
      
    }

    [Route("{id}")]
    [HttpPut]
    public async Task<IHttpActionResult> PutRental(int id, RentalDetailDto rentalDetailDto)
    {
      if (!ModelState.IsValid) return BadRequest(ModelState);

      var rental = await _db.Rentals.FindAsync(id);
      if (rental == null)
        return NotFound();
      
      try
      {
        _db.Configuration.AutoDetectChangesEnabled = false;

        foreach (var entity in 
                 rentalDetailDto
                   .RentalDetailIds
                   .Select(rentalDetailId => new RentalDetail()
                   {
                     Id = rentalDetailId,
                     DateReturned = DateTime.Now,
                     IsReturned = true,
                   }))  
        {
          _db.RentalDetails.Attach(entity);
          _db.Entry(entity).Property(e => e.IsReturned).IsModified = true;
          _db.Entry(entity).Property(e => e.DateReturned).IsModified = true;
        }


        await _db.SaveChangesAsync();

        
      }
      catch (DbUpdateConcurrencyException)
      {
        return NotFound();
      }
      finally
      {
        _db.Configuration.AutoDetectChangesEnabled = true;
      }

      var isNotCompleted = await _db.RentalDetails
        .AnyAsync(q => q.IsReturned == false && q.RentalId == id);

      if (!isNotCompleted)
      {
        rental.IsCompleted = true;
        rental.DateCompleted = DateTime.Now;
      }

      await _db.SaveChangesAsync();

      return Ok(rental);
    }


    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        _db.Dispose();
      }
      base.Dispose(disposing);
    }

    private bool RentalExists(int id)
    {
      return _db.Rentals.Count(e => e.Id == id) > 0;
    }
  }
  

}