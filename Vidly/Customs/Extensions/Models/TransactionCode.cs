using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vidly.Customs.Extensions.Models
{
  public class TransactionCode
  {
    public string PrefixCode { get; set; }
    public string TableName { get; set; }
    public string PrimaryKey { get; set; } = "Id";
    public string NumberFormat { get; set; } = "0000";


  }
}