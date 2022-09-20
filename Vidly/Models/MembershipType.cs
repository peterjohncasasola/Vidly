using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Vidly.Models
{
  public class MembershipType
  {
    public MembershipType()
    {
      Customers = new List<Customer>();
    }
    public int Id { get; set; }
    [Required]

    [Display(Name = "Membership Type")]
    public string Name { get; set; }
    public short SignUpFee { get; set; }
    public byte DurationInMonths { get; set; }
    public byte DiscountRate { get; set; }
    public ICollection<Customer> Customers { get; set; }
  }
}