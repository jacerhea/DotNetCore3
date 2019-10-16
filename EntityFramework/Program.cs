using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace EntityFramework
{
    class Program
    {
        static async Task Main(string[] args)
        {

            var optionsBuilder = new DbContextOptionsBuilder<NorthwindContext>();
                optionsBuilder.UseSqlServer("Data Source=localhost;Initial Catalog=Northwind;Trusted_Connection=True")
                .AddInterceptors(new HintCommandInterceptor());

            using var context = new NorthwindContext(optionsBuilder.Options);
            var chaOrders = context.Products
                .Where(product => product.ProductName.StartsWith("cha"))
                .ToList();


            // find the bug
            var chaFailedCall = context.Products
            //    .AsEnumerable()
                .Where(x => GetProductsStartingWithCha(x))
                .ToList();

            var orders =
              from o in context.Orders
              where o.ShipName == "Piccolo und mehr"
              select o;


            // async stream
            await foreach(var o in orders.AsAsyncEnumerable())
            {
                Console.WriteLine(o.ShipName);
            }

            // tags. 2.2 feature.
            var orderCallWithTag = context.Orders
                .TagWith("iz in yur databases")
              .Where(o => o.ShipCountry == "Austria")
              .ToList();
        }

        private static bool GetProductsStartingWithCha(Products product)
        {
            return product.ProductName.StartsWith("cha");
        }
    }

    public class HintCommandInterceptor : DbCommandInterceptor
    {
        public override InterceptionResult<DbDataReader> ReaderExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result)
        {
            // Manipulate the command text, etc. here...
            command.CommandText += " OPTION (OPTIMIZE FOR UNKNOWN)";
            return result;
        }
    }
}
