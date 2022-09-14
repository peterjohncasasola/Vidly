using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Vidly.Customs.Extensions;
using Vidly.Customs.Extensions.Models;
using Vidly.Models;

namespace Vidly.Controllers.Api
{
  public class MoviesController : ApiController
  {
    private readonly AppDbContext _db;

    public MoviesController()
    {
      _db = new AppDbContext();
    }

    // GET: api/Movies
    public async Task<IHttpActionResult> GetMovies([FromUri] QueryObject query, bool onlyAvailable = false)
    {
      IQueryable data = null;
      var movies = _db.Movies.AsQueryable();

      if (onlyAvailable) movies = movies.Where(m => m.Stock > 0);

      if (!string.IsNullOrEmpty(query.Search.Trim()) && !string.IsNullOrEmpty(query.SearchBy.Trim()))
        movies = movies.Where(query);

      var totalRecords = await movies.CountAsync();

      movies = movies.SortBy(query).Paginate(query);

      if (query.Page > 0)
        movies = movies.Paginate(query);

      if (!string.IsNullOrEmpty(query.Fields.Trim()))
        data = movies.SelectColumns(query.Fields);

      return Ok(ResponseHelper.ToPagedResponse(query, totalRecords, data ?? movies));

    }

    // GET: api/Movies/5
    [ResponseType(typeof(Movie))]
    public async Task<IHttpActionResult> GetMovie(int id)
    {
      var movie = await _db.Movies.FindAsync(id);
      if (movie == null)
      {
        return NotFound();
      }

      return Ok(movie);
    }

    // PUT: api/Movies/5
    [ResponseType(typeof(void))]
    public async Task<IHttpActionResult> PutMovie(int id, Movie movie)
    {
      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      if (await MovieExists(id, movie))
        return BadRequest($"{movie.Name} is already exists");

      if (id != movie.Id)
        return BadRequest();

      _db.Entry(movie).State = EntityState.Modified;

      try
      {
        await _db.SaveChangesAsync();
      }
      catch (DbUpdateConcurrencyException)
      {
        if (!MovieExists(id))
        {
          return NotFound();
        }
        else
        {
          throw;
        }
      }

      return StatusCode(HttpStatusCode.NoContent);
    }

    // POST: api/Movies
    [ResponseType(typeof(Movie))]
    public async Task<IHttpActionResult> PostMovie(Movie movie)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      if (await MovieExists(movie))
        return BadRequest($"{movie.Name} is already exists");

      _db.Movies.Add(movie);
      await _db.SaveChangesAsync();

      return CreatedAtRoute("DefaultApi", new { id = movie.Id }, movie);
    }

    // DELETE: api/Movies/5
    [ResponseType(typeof(Movie))]
    public async Task<IHttpActionResult> DeleteMovie(int id)
    {
      var movie = await _db.Movies.FindAsync(id);
      if (movie == null)
      {
        return NotFound();
      }

      _db.Movies.Remove(movie);
      await _db.SaveChangesAsync();

      return Ok(movie);
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        _db.Dispose();
      }
      base.Dispose(disposing);
    }

    private bool MovieExists(int id)
    {
      return _db.Movies.Count(e => e.Id == id) > 0;
    }
    
    private async Task<bool> MovieExists(Movie movie) => await _db.Customers.AnyAsync(m => m.Name == movie.Name);
    private async Task<bool> MovieExists(int id, Movie movie) => await _db.Movies.AnyAsync(m => m.Name == movie.Name & m.Id != id);

  }
}