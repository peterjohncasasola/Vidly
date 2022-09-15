using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Vidly.Customs.Extensions;
using Vidly.Customs.Extensions.Models;
using Vidly.Models;
using Vidly.Models.DTO;

namespace Vidly.Controllers.Api
{
  //[Route("api/customers")]
  public class CustomersController : ApiController
  {
    private AppDbContext DbContext { get; }

    public CustomersController() => DbContext = new AppDbContext();

    [HttpGet()]
    public async Task<IHttpActionResult> GetCustomers([FromUri] QueryObject query)
    {

       var customers = DbContext.Customers.Include(c => c.MembershipType).AsQueryable();
       
       var result = await customers.Filter(query).ToPaginateAsync(query);
       
       return Ok(result);

    }

    [ResponseType(typeof(Customer))]
    public Customer GetCustomer(int id)
    {
      var customer = DbContext.Customers
                .Include(c => c.MembershipType)
                .FirstOrDefault(c => c.Id == id);

      return customer;
    }

    [HttpPut]
    public async Task<IHttpActionResult> PutCustomer(int id, CustomerDto customerDto)
    {
      if (!ModelState.IsValid) return BadRequest(ModelState);

      var customer = await DbContext.Customers.Include(q => q.MembershipType).FirstOrDefaultAsync(c => c.Id == id);

      if (id != customerDto.Id)
        return BadRequest();
      
      if (await CustomerExists(id, customerDto))
        return BadRequest($"{customerDto.Name} is already exists");
      
      if (customer != null)
      {
        customer.Name = customerDto.Name;
        customer.Address = customerDto.Address;
        customer.IsSubscribedToNewsLetter = customerDto.IsSubscribedToNewsLetter;
        customer.BirthDate = customerDto.BirthDate;
        customer.MembershipTypeId = customerDto.MembershipTypeId;

        DbContext.Entry(customer).State = EntityState.Modified;
      }

      try
      {
        await DbContext.SaveChangesAsync();
      }
      catch (DbUpdateConcurrencyException)
      {
        if (!CustomerExists(id))
          return NotFound();

        throw;
      }

      return Ok(customer);
    }

    // POST: api/Customers
    [ResponseType(typeof(Customer))]
    [HttpPost]
    public async Task<IHttpActionResult> PostCustomer(CustomerDto customerDto)
    {
      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      if (await CustomerExists(customerDto))
        return BadRequest($"{customerDto.Name} is already exists");

      var customer = new Customer()
      {
        BirthDate = customerDto.BirthDate,
        MembershipTypeId = customerDto.MembershipTypeId,
        Name = customerDto.Name,
        Address = customerDto.Address,
        IsSubscribedToNewsLetter = customerDto.IsSubscribedToNewsLetter
      };

      DbContext.Customers.Add(customer);
      await DbContext.SaveChangesAsync();

      return Ok(GetCustomer(customer.Id));
    }

    // DELETE: api/Customers/5
    [ResponseType(typeof(Customer))]
    [HttpDelete]
    public async Task<IHttpActionResult> DeleteCustomer(int id)
    {
      if (await HasRentals(id))
        return BadRequest("Unable to delete. Has existing rental transactions");
      
      var customer = await DbContext.Customers.FindAsync(id);
      if (customer == null)
        return NotFound();

      DbContext.Customers.Remove(customer);
      await DbContext.SaveChangesAsync();

      return StatusCode(HttpStatusCode.NoContent);

    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        DbContext.Dispose();
      }
      base.Dispose(disposing);
    }

    private bool CustomerExists(int id) => DbContext.Customers.Any(e => e.Id == id);
    private async Task<bool> CustomerExists(CustomerDto customerDto) => await DbContext.Customers.AnyAsync(c => c.Name == customerDto.Name);
    private async Task<bool> CustomerExists(int id, CustomerDto customerDto) => await DbContext.Customers.AnyAsync(c => c.Name == customerDto.Name & c.Id != id);
    private async Task<bool> HasRentals(int id) => await DbContext.RentalDetails.AnyAsync(c => c.Rental.CustomerId == id && c.IsReturned == false);

  }
}