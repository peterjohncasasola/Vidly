using System.Collections.Generic;
using Vidly.Migrations;

namespace Vidly.Models.ViewModels
{
  public class CustomerFormViewModel
  {
    public List<MembershipType> MembershipTypes { get; set; }
    public Customer Customer { get; set; }
  }
}