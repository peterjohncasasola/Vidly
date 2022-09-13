//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Data.Entity;
//using System.Data.Entity.Infrastructure;
//using System.Linq;
//using System.Net;
//using System.Net.Http;
//using System.Threading.Tasks;
//using System.Web.Http;
//using System.Web.Http.Description;
//using Vidly.Customs.Extensions;
//using Vidly.Customs.Extensions.Models;
//using Vidly.Models;
//using Vidly.Models.DTO;

//namespace Vidly.Controllers.Api
//{
//  [Route("api/rentals")]
//  public class RentalsController : ApiController
//  {
//    private readonly AppDbContext _db = new();
    
//    public IHttpActionResult GetRentals([FromUri] QueryObject query)
//    {
//      var rentals = _db.Rentals;

//      if (!string.IsNullOrEmpty(query.Fields.Trim()))
//        return Ok(rentals.SelectColumns(query.Fields));

//      return Ok(rentals);
//    }
//    public async Task<IHttpActionResult> GetRental(int id)
//    {
//      var rental = await _db.Rentals.FindAsync(id);

//      if (rental == null)
//      {
//        return NotFound();
//      }

//      return Ok(rental);
//    }
    
//    public async Task<IHttpActionResult> PostRental(NewRentalDto rentalDto)
//    {
//      if (!ModelState.IsValid)
//      {
//        return BadRequest(ModelState);
//      }

//      var rental = new Rental()
//      {
//        CustomerId = rentalDto.CustomerId,
//        DateRented = DateTime.Now,
//      };

//      var customer = await _db.Customers.FindAsync(rentalDto.CustomerId);
//      if (customer == null)
//      {
//        return BadRequest("Customer not found");
//      }

//      var movies = await _db.Movies.Where(m => rentalDto.MovieIds.Contains(m.Id) && m.Stock > 0).ToListAsync();
//      var rentailDetails = new List<RentalDetail>();

//      if (movies.Count == 0) return BadRequest("Movies are not available");

//      _db.Rentals.Add(rental);

//      foreach (var movie in movies) 
//        if (movie.Stock == 0)
//          return BadRequest($"{movie.Name} is not available");
//        else
//        {
//          movie.Stock--;
//          var rentedMovie = new RentalDetail()
//          {
//            Movie = movie,
//            Rental = rental
//          };
//          rentailDetails.Add(rentedMovie);
//        }

//      _db.RentalDetails.AddRange(rentailDetails);

//      await _db.SaveChangesAsync();

//      return CreatedAtRoute("DefaultApi", new { id = rental.Id }, rentalDto);
//    }

//    protected override void Dispose(bool disposing)
//    {
//      if (disposing)
//      {
//        _db.Dispose();
//      }
//      base.Dispose(disposing);
//    }

//    private bool RentalExists(int id)
//    {
//      return _db.Rentals.Count(e => e.Id == id) > 0;
//    }
//  }
//}