using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Vidly.Customs.Data_Annotations;

namespace Vidly.Models
{
  public class Customer
  {
    public Customer()
    {
    }

    private int CalculateAge(DateTime birthDate)
    {
      var age = DateTime.Now.Year - birthDate.Year;
      if (DateTime.Now.DayOfYear < birthDate.DayOfYear)
        age -= 1;

      return age;
    }
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(255)]
    public string Name { get; set; }

    [Display(Name = "Date of Birth")]
    [DisplayFormat(DataFormatString = "{0:MMMM d, yyyy}")]
    [DateRange]
    public DateTime BirthDate { get; set; }

    [Column(Order = 4)]
    public string Address { get; set; }

    [Display(Name = "Subscribe to Newsletter?")]  
    public bool IsSubscribedToNewsLetter { get; set; }

    [Display(Name = "Membership")]
    [Required]
    public int MembershipTypeId { get; set; }
    public MembershipType MembershipType { get; set; }
    public ICollection<Rental> Rentals { get; set; }
    [NotMapped] public int Age => CalculateAge(BirthDate);
  }

}