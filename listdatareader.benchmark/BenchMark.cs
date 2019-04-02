using System;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System.Data;
using System.Data.SqlClient;
using umbe.data;
using BenchmarkDotNet.Configs;

namespace listdatareader.benchmark
{
    [Config(typeof(Config))]
    public class BenchMark
    {
        private List<Order> _list;

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
        [GlobalSetup]
        public void Setup(){
            _list = GetOrders();
            using(var conn = new SqlConnection("Server=localhost;Database=Test;User Id=<username>;Password=<password>")){
                conn.Open();

                var cmd = new SqlCommand(sqlTable, conn);
                cmd.ExecuteNonQuery();
            }            
        }
        
        [Benchmark]
        public void IDataReader(){
            using(var conn = new SqlConnection("Server=localhost;Database=Test;User Id=<username>;Password=<password>")){
                conn.Open();
                
                var listdatareader = new ListDataReader<Order>(_list);

                using(var bulk = new SqlBulkCopy(conn)){
                    bulk.DestinationTableName = "Orders";
                    bulk.WriteToServer(listdatareader);
                }
            
            }
        }
        
        [Benchmark]
        public void DataTable(){

            var dt = new DataTable();
            dt.Columns.Add(new DataColumn("Id", typeof(int)));
            dt.Columns.Add(new DataColumn("CustomerName", typeof(string)));
            dt.Columns.Add(new DataColumn("Total", typeof(decimal)));
            dt.Columns.Add(new DataColumn("Date", typeof(DateTime)));

            foreach(var order in _list){
                var dr = dt.NewRow();
                dr["Id"] = order.Id;
                dr["CustomerName"] = order.CustomerName;
                dr["Total"] = order.Total;
                dr["Date"] = order.Date;

                dt.Rows.Add(dr);
            }
            
            using(var conn = new SqlConnection("Server=localhost;Database=Test;User Id=<username>;Password=<password>")){
                conn.Open();
                
                using(var bulk = new SqlBulkCopy(conn)){
                    bulk.DestinationTableName = "Orders";
                    bulk.WriteToServer(dt);
                }
            
            }

        }

        private class Config : ManualConfig
        {
            public Config(){
                this.Options = ConfigOptions.DisableOptimizationsValidator;
            }
        }
    }
}