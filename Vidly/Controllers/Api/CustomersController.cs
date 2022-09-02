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
using Vidly.Models;
using Vidly.Models.DTO;

namespace Vidly.Controllers.Api
{
  [Route("api/customers")]
  public class CustomersController : ApiController
  {
    public AppDbContext DbContext { get; }

    public CustomersController() => DbContext = new AppDbContext();

    [HttpGet()]
    public IQueryable<Customer> GetCustomers()
    {
      return DbContext.Customers.Include(c => c.MembershipType);
    }
    
    [ResponseType(typeof(Customer))]
    public async Task<IHttpActionResult> GetCustomer(int id)
    {
      var customer = await DbContext.Customers.FindAsync(id);
      if (customer == null)
      {
        return NotFound();
      }

      return Ok(customer);
    }
    
    [HttpPut]
    public async Task<IHttpActionResult> PutCustomer(int id, CustomerDto customerDto)
    {
      if (!ModelState.IsValid) return BadRequest(ModelState);

      var customer = await DbContext.Customers.FindAsync(id);

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

      return StatusCode(HttpStatusCode.NoContent);
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
        MembershipTypeId = customerDto.Id,
        Name = customerDto.Name,
        Address = customerDto.Address
      };

      DbContext.Customers.Add(customer);
      await DbContext.SaveChangesAsync();

      return CreatedAtRoute("DefaultApi", new { id = customer.Id }, customer);
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