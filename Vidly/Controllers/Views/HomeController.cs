﻿using System.Web.Mvc;

namespace Vidly.Controllers
{
    [AllowAnonymous]
  public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        
        public ActionResult NewRental()
        {
            return View("RentalView");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }
    }
}