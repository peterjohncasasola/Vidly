using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Vidly.Models;
using Vidly.Models.ViewModels;

namespace Vidly.Controllers
{
  public class CustomersController : Controller
  {
    private readonly AppDbContext _dbContext;
    public CustomersController()
    {
      _dbContext = new AppDbContext();
    }

    // GET: Customers
    public ViewResult Index()
    {
      return View("AjaxView");
    }

    public async Task<ActionResult> Details(int? id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      var customer = await _dbContext.Customers.FindAsync(id);
      if (customer == null)
      {
        return HttpNotFound();
      }
      return View(customer);
    }

    public ActionResult New()
    {
      var memberShipTypes = _dbContext.MembershipTypes.ToList();

      var viewModel = new CustomerFormViewModel()
      {
        MembershipTypes = memberShipTypes,
      };

      return View("CustomerForm", viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Submit(Customer customer)
    {
      if (!ModelState.IsValid)
      {
        var viewModel = new CustomerFormViewModel
        {
          Customer = customer,
          MembershipTypes = _dbContext.MembershipTypes.ToList()
        };

        return View("CustomerForm", viewModel);
      }

      if (customer.Id > 0)
      {
        var customerInDb = await _dbContext.Customers.SingleAsync(c => c.Id == customer.Id);

        customerInDb.MembershipTypeId = customer.MembershipTypeId;
        customerInDb.Address = customer.Address;
        customerInDb.BirthDate = customer.BirthDate;
        customerInDb.Name = customer.Name;
        customerInDb.IsSubscribedToNewsLetter = customer.IsSubscribedToNewsLetter;

      }
      else
      {
        _dbContext.Customers.Add(customer);
      }

      await _dbContext.SaveChangesAsync();
      return RedirectToAction("Index", "Customers");

    }

    public async Task<ActionResult> Edit(int? id)
    {
      if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      
      var customer = await _dbContext.Customers.FindAsync(id);
      
      if (customer == null) return HttpNotFound();
      
      var memberShipTypes = _dbContext.MembershipTypes.ToList();
      var viewModel = new CustomerFormViewModel()
      {
        MembershipTypes = memberShipTypes,
        Customer = customer
      };

      return View("CustomerForm", viewModel);
    }

    // GET: Customers/Delete/5
    public async Task<ActionResult> Delete(int? id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      Customer customer = await _dbContext.Customers.FindAsync(id);
      if (customer == null)
      {
        return HttpNotFound();
      }
      return View(customer);
    }

    // POST: Customers/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> DeleteConfirmed(int id)
    {
      Customer customer = await _dbContext.Customers.FindAsync(id);
      _dbContext.Customers.Remove(customer);
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
