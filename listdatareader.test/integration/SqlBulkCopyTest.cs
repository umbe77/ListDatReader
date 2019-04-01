using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using umbe.data;
using Xunit;

namespace listdatareader.test.integration
{
    public class SqlBulkCopyTest
    {
        private const string sqlTable = @"
IF NOT EXISTS(SELECT 1 FROM sys.tables WHERE name = 'Orders')
BEGIN
    CREATE TABLE Orders (
        Id int,
        CustomerName nvarchar(50),
        Total decimal,
        Date datetime
    )
END
ELSE
BEGIN
    TRUNCATE TABLE Orders
END
";

        private const string sqlCount = @"
SELECT COUNT(*)
FROM Orders";

        [Fact]
        public void WriteWithBulk()
        {
            var orders = GetOrders();
            using(var conn = new SqlConnection("Server=localhost;Database=Test;User Id=<Username>;Password=<password>")){
                conn.Open();

                var cmd = new SqlCommand(sqlTable, conn);
                cmd.ExecuteNonQuery();

                var listdatareader = new ListDataReader<Order>(orders);

                using(var bulk = new SqlBulkCopy(conn)){
                    bulk.DestinationTableName = "Orders";
                    bulk.WriteToServer(listdatareader);
                }

                var cmdCount = new SqlCommand(sqlCount, conn);
                var count = (int)cmdCount.ExecuteScalar();
                Assert.Equal(1000000, count);
            }
        }

        private List<Order> GetOrders(){
            var list = new List<Order>();

            for(var i=0;i<1000000;++i){
                list.Add(new Order{
                    Id = i,
                    CustomerName = $"Roberto - {i}",
                    Total = i + 23.7M,
                    Date = DateTime.Now
                });
            }

            return list;
        }

    }
}