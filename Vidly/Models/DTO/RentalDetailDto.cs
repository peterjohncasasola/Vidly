using System.Collections.Generic;

namespace Vidly.Models.DTO
{
  public class RentalDetailDto
  {
    public RentalDetailDto()
    {
      
    }

    public int RentalId { get; set; }
    public List<int> RentalDetailIds { get; set; }

  }
}