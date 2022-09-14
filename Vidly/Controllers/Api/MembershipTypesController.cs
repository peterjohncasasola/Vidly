using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;
using Vidly.Customs.Extensions;
using Vidly.Customs.Extensions.Models;
using Vidly.Models;

namespace Vidly.Controllers.Api
{
    [System.Web.Http.Route("api/membership-types")]
    public class MembershipTypesController : ApiController
    {
        private readonly AppDbContext _dbContext;
        public MembershipTypesController()
        {
            _dbContext = new AppDbContext();
        }

        // GET: api/Movies
        public async Task<IHttpActionResult> GetList()
        {
          var movies = await _dbContext.MembershipTypes.ToListAsync();
          return Ok(movies);
        }




    // GET
  }
}