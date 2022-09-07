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
      IQueryable data = null;
      var customers = DbContext.Customers.Include(c => c.MembershipType).AsQueryable();
      
     if (!string.IsNullOrEmpty(query.Search.Trim()) && !string.IsNullOrEmpty(query.SearchBy.Trim())) 
       customers = customers.Where(query);
     
     var totalRecords = await customers.CountAsync();
     
      customers = customers.SortBy(query).Paginate(query);
     
     if (!string.IsNullOrEmpty(query.Fields.Trim()))
       data = customers.SelectColumns(query.Fields);

     return Ok(ResponseHelper.ToPagedResponse(query, totalRecords, data ?? customers));

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
      {
        return BadRequest();
      }

      if (customer != null)
      {
        customer.Name = customerDto.Name;
        customer.Address = customerDto.Address;
        customer.IsSubscribedToNewsLetter = customerDto.IsSubscribedToNewsLetter;
        customer.BirthDate = customerDto.BirthDate;

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
      {
        return BadRequest(ModelState);
      }

      var customer = new Customer()
      {
        BirthDate = customerDto.BirthDate,
        MembershipTypeId = customerDto.MembershipTypeId,
        Name = customerDto.Name,
        Address = customerDto.Address
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

    private bool CustomerExists(int id)
    {
      return DbContext.Customers.Count(e => e.Id == id) > 0;
    }
  }
}