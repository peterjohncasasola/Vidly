using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Vidly.Customs.Extensions;
using Vidly.Customs.Extensions.Models;
using Vidly.Models;
using Vidly.Models.DTO;

namespace Vidly.Controllers.Api
{
  public class RentalsController : ApiController
  {
    private readonly AppDbContext _db = new();
    
    [HttpGet]
    public IHttpActionResult GetRentals([FromUri]   
      QueryObject query,
      [FromUri] DateRange dateRange,
      bool includeReturned = false, string filterDateBy = ""
      )
    {
      var rentals = _db.RentalDetails.Include(r => r.Movie)
        .Include(r => r.Rental)
        .Include(r => r.Rental.Customer);

      if (!string.IsNullOrEmpty(filterDateBy.Trim()))
      {
        var dateTo = dateRange.DateTo.AddDays(1);
        switch (filterDateBy.ToUpper().Trim())
        {
          case "RETURNED":
            rentals = rentals.Where(q => q.DateReturned >= dateRange.DateFrom && q.DateReturned <= dateTo);
            break;
          case "RENTED":
            rentals = rentals.Where(q => q.Rental.DateRented >= dateRange.DateFrom && q.Rental.DateRented <= dateTo);
            break;
          default:
            rentals = rentals;
            break;
        }
        
        if (!includeReturned)
          rentals = rentals.Where(r => r.IsReturned == false);
      }

      var result = rentals.Filter(query).ToPaginate(query);
      
      return Ok(result);
    }
    public async Task<IHttpActionResult> GetRental(int id)
    {
      var rental = await _db.Rentals.FindAsync(id);

      if (rental == null)
      {
        return NotFound();
      }

      return Ok(rental);
    }

    public async Task<IHttpActionResult> PostRental(NewRentalDto rentalDto)
    {
      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      var rental = new Rental()
      {
        CustomerId = rentalDto.CustomerId,
        DateRented = DateTime.Now,
      };

      var customer = await _db.Customers.FindAsync(rentalDto.CustomerId);
      if (customer == null)
        return BadRequest("Customer not found");

      var movies = await _db.Movies.Where(m => rentalDto.MovieIds.Contains(m.Id) && m.Stock > 0).ToListAsync();
      var rentalDetails = new List<RentalDetail>();

      if (movies.Count == 0) return BadRequest("Movies are not available");

      _db.Rentals.Add(rental);

      foreach (var movie in movies)
      {
        if (movie.Stock == 0)
          return BadRequest($"{movie.Name} is not available");

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

    [Route("api/rentals/details/{id}")]
    [HttpPut]
    public async Task<IHttpActionResult> PutRental(int id)
    {
      if (!ModelState.IsValid) return BadRequest(ModelState);
      var rentalDetail = await _db.RentalDetails.FindAsync(id);

      try
      {

        if (rentalDetail != null)
        {
          rentalDetail.DateReturned = DateTime.Now;
          rentalDetail.IsReturned = true;
          var rentedMovie = await _db.Movies.FindAsync(rentalDetail.MovieId);
          if (rentedMovie != null) rentedMovie.Stock++;
        }
        await _db.SaveChangesAsync();
      }
      catch (DbUpdateConcurrencyException)
      {
          return NotFound();
      }

      return Ok(rentalDetail);
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