using System.Collections.Generic;

namespace Vidly.Models.ViewModels
{
  public class CustomerFormViewModel
  {
    public List<MembershipType> MembershipTypes { get; set; }
    public Customer Customer { get; set; }
  }
}