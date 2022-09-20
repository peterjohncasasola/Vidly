using System;
using System.Threading.Tasks;
using Vidly.Customs.Extensions.Models;
using Vidly.Models;

namespace Vidly.Customs.Extensions.Helpers
{
    
    public static class EfHelper
    {
      public static async Task<string> GenerateTransactionCode(TransactionCode transactionCode)
        {
          using var context = new AppDbContext();

          var code = await context
            .Database
            .SqlQuery<string>("SELECT CONCAT(" +
                              $"'{transactionCode.PrefixCode}-{DateTime.Now.Year}-', " +
                              $"RIGHT('{transactionCode.NumberFormat}' + " +
                              $"CAST (ISNULL(MAX({transactionCode.PrimaryKey}) + 1, 1) " +
                              $"AS VARCHAR(20)),{transactionCode.NumberFormat.Length})) " +
                              $"FROM {transactionCode.TableName}")
            

            .FirstOrDefaultAsync();

          return code;
        }
        public static async Task<string> GenerateTransactionCode
        (
           string prefixCode
          ,string tableName
          ,string primaryKey = "Id"
          ,string numberFormat = "0000"
           )
        {
          using var context = new AppDbContext();
          var code = await context
            .Database
            .SqlQuery<string>("SELECT CONCAT(" +
                              $"'{prefixCode}-{DateTime.Now.Year}-', " +
                              $"RIGHT('{numberFormat}' + " +
                              $"CAST (ISNULL(MAX({primaryKey}) + 1, 1) AS VARCHAR(20)),{numberFormat.Length})) " +
                              $"FROM {tableName}")
            .FirstOrDefaultAsync();

          return code;
        }
    }
}