using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;
using Vidly.Customs.Extensions.Models;
using Vidly.Models;

namespace Vidly.Controllers.Api
{
    public class MembershipTypesController : Controller
    {
        private AppDbContext _dbContext;
        public MembershipTypesController()
        {
            _dbContext = new AppDbContext();
        }
        // GET
    }
}