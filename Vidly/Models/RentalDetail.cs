using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vidly.Models
{
    public class RentalDetail
    {
        public int Id { get; set; }
        public int RentalId { get; set; }
        public Rental Rental { get; set; }
        public int MovieId { get; set; }
        public Movie Movie { get; set; }
        public bool IsReturned { get; set; } = false;
        public DateTime? DateReturned { get; set; }
    }
}