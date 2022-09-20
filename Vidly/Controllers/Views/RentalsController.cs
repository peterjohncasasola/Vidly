using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Vidly.Controllers.Views
{
  public class RentalsController : Controller
  {
    // GET: Rentals
    public ActionResult Index()
    {
      return View("Index");
    }

    public ActionResult List()
    {
      return View("List");
    }

    public ActionResult New()
    {
      return View("New");
    }

  }
}
