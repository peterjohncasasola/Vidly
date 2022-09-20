using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;
using Vidly.Customs.Extensions.Models;

namespace Vidly.Models
{
    public class Rental
    {
        public Rental()
        {
          RentalDetails = new List<RentalDetail>();
        }
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Customer is required")]
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
        public DateTime DateRented { get; set; }
        public ICollection<RentalDetail> RentalDetails { get; set; }
        [Required]
        public string RentalCode { get; set; }

        public bool? IsCompleted { get; set; }
        public DateTime? DateCompleted { get; set; }
    }
}