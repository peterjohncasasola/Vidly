﻿using System.ComponentModel.DataAnnotations;

namespace Vidly.Migrations
{
  public class MembershipType
  {
    public int Id { get; set; }
    [Required]

    [Display(Name = "Membership Type")]
    public string Name { get; set; }
    public short SignUpFee { get; set; }
    public byte DurationInMonths { get; set; }
    public byte DiscountRate { get; set; }
  }
}