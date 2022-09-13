using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using Vidly.Customs.Data_Annotations;

namespace Vidly.Models.DTO
{
  public class CustomerDto
  {
    public int Id { get; set; }

    [Required]
    [StringLength(255)]
    public string Name { get; set; }

    [DateRange]
    public DateTime? BirthDate { get; set; }

    public string Address { get; set; }
    public bool IsSubscribedToNewsLetter { get; set; }
    [Required]
    public int MembershipTypeId { get; set; }
  }
}