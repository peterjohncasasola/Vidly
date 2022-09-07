using System.Data.Entity;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Vidly.Models;

namespace Vidly.Controllers
{
  public class MoviesController : Controller
    {
      private readonly AppDbContext _dbContext;

        public MoviesController()
        {
          _dbContext = new AppDbContext();
        }
        // GET: Movies
        public async Task<ActionResult> Index()
        {
          var movies = await _dbContext.Movies.ToListAsync();
            return View(movies);
        }

        // GET: Movies/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
              return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            
            var movie = await _dbContext.Movies.FindAsync(id);
            if (movie == null)
              return HttpNotFound();
            
            return View(movie);
        }

        public ActionResult New()
        {
            return View("MovieForm");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Submit(Movie movie)
        {
          // if (!ModelState.IsValid) return View("MovieForm", movie);

          if (movie.Id > 0)
          {
            var movieInDb = await _dbContext.Movies.SingleAsync(c => c.Id == movie.Id);
            movieInDb.Genre = movie.Genre;
            movieInDb.DateRelease = movie.DateRelease;
            movieInDb.Stock = movie.Stock;
            movieInDb.Name = movie.Name;
          }
          else
          {
            _dbContext.Movies.Add(movie);
          }

          await _dbContext.SaveChangesAsync();
          return RedirectToAction("Index", "Movies");

        }

    // GET: Movies/Edit/5
    public async Task<ActionResult> Edit(int? id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      var movie = await _dbContext.Movies.FindAsync(id);
        if (movie == null)
        {
          return HttpNotFound();
        }
        return View("MovieForm", movie);
    }


        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
              return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var movie = await _dbContext.Movies.FindAsync(id);
            if (movie == null)
              return HttpNotFound();

            return View(movie);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var movie = await _dbContext.Movies.FindAsync(id);
            _dbContext.Movies.Remove(movie);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _dbContext.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
