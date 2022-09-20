using System.Collections.Generic;
using Vidly.Customs.Extensions.Models;

namespace Vidly.Models.DTO
{
  public class NewRentalDto
  {
    public NewRentalDto() 
    {
      TransactionCode = new TransactionCode()
      {
        PrimaryKey = "intRentalId",
        TableName = "rentals",
        PrefixCode = "RTL"
      };
    }

    public int CustomerId { get; set; }
    public List<int> MovieIds { get; set; }
    public TransactionCode TransactionCode { get; }

  }
}