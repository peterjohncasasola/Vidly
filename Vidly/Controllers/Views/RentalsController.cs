//using System;
//using System.Collections.Generic;
//using System.Data.Entity;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;
//using Vidly.Models;

//namespace Vidly.Controllers.Views
//{
//    public class RentalsController : Controller
//    {
//      private readonly AppDbContext _db = new AppDbContext();

//    // GET: Rentals
//        public ActionResult Index()
//        {
//          var rentals = _db.RentalDetails.Include(q => q.Movie)
//            .Include(q => q.Rental.Customer);
//          return View("Index", rentals);
//        }

//        public ActionResult New()
//        {
//          return View("New");
//        }

//  }
//}
