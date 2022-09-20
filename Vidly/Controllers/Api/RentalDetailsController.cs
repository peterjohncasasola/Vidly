using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Vidly.Customs.Extensions;
using Vidly.Customs.Extensions.Models;
using Vidly.Models;

namespace Vidly.Controllers.Api
{
  [RoutePrefix("api/rental/details")]
  public class RentalDetailsController : ApiController
  {
    private readonly AppDbContext _db = new();


    [HttpGet]
    public async Task<IHttpActionResult> GetRentalDetails([FromUri]
      QueryObject query,
      [FromUri] DateRange dateRange,
      bool isReturned = false, string filterDateBy = ""
    )
    {
      var rentals = _db.RentalDetails.Include(r => r.Movie)
        .Include(r => r.Rental)
        .Include(r => r.Rental.Customer);

      rentals = rentals.Where(r => r.IsReturned == isReturned);

      if (!string.IsNullOrEmpty(filterDateBy.Trim()))
      {
        var dateTo = dateRange.DateTo.AddDays(1);
        rentals = filterDateBy.ToUpper().Trim() switch
        {
          "RETURNED" => rentals.Where(q => q.DateReturned >= dateRange.DateFrom && q.DateReturned <= dateTo),
          "RENTED" => rentals.Where(q => q.Rental.DateRented >= dateRange.DateFrom && q.Rental.DateRented <= dateTo),
          _ => rentals
        };
      }

      var result = await rentals.Filter(query).ToPaginateAsync(query);

      return Ok(result);
    }

    // GET: api/RentalDetails/5
    [ResponseType(typeof(RentalDetail))]
    [HttpGet]
    [Route("{id}")]
    public async Task<IHttpActionResult> GetRentalDetailById(int id)
    {
      var rentalDetail = await _db.RentalDetails
        .Where(r => r.Id == id)
        .Include(t => t.Movie)
        .SingleAsync();

      if (rentalDetail == null)
        return NotFound();
     
      return Ok(rentalDetail);
    }

    [ResponseType(typeof(RentalDetail))]
    [HttpGet]
    [Route()]
    public async Task<IHttpActionResult> GetRentalDetailsByRentalId(int rentalId)
    {
      var rentalDetail = await _db.RentalDetails
        .Where(r => r.RentalId == rentalId)
        .Include(t => t.Movie)
        .SingleAsync();

      if (rentalDetail == null)
        return NotFound();

      return Ok(rentalDetail);
    }

    [ResponseType(typeof(void))]
    [Route("{id}")]
    [HttpPut]
    public async Task<IHttpActionResult> PutRentalDetail(int id)
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

          await _db.SaveChangesAsync();
        }

        var isNotCompleted = await _db.RentalDetails
          .AnyAsync(r => r.IsReturned == false &&
                         r.RentalId == rentalDetail.RentalId);

        if (!isNotCompleted)
        {
          var rental = await _db.Rentals.SingleAsync(q => q.Id == rentalDetail.RentalId);
          rental.DateCompleted = DateTime.Now;
          rental.IsCompleted = true;
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

    private bool RentalDetailExists(int id)
    {
      return _db.RentalDetails.Count(e => e.Id == id) > 0;
    }
  }
}