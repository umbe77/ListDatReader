using System;
using System.Collections.Generic;
using Xunit;
using umbe.data;

namespace listdatareader.test
{
    public class TypedGetTest
    {

        private List<Order> _list;
        public TypedGetTest(){
            _list = new List<Order>{
                new Order{
                    Id = 1,
                    CustomerName = "Roberto",
                    Total = 12.3M,
                    IsActive = false
                }
            };
        }

        [Fact]
        public void GetString()
        {
            var dr = new ListDataReader<Order>(_list);
            var pIndex = dr.GetOrdinal("CustomerName");

            var countLine = 0;
            while(dr.Read()){
                countLine++;
                var p = dr.GetString(pIndex);
                Assert.Equal("Roberto", p);
                Assert.IsType<string>(p);
            }

            Assert.Equal(1, countLine);
        }

        [Fact]
        public void GetInt32()
        {
            var dr = new ListDataReader<Order>(_list);
            var pIndex = dr.GetOrdinal("Id");

            var countLine = 0;
            while(dr.Read()){
                countLine++;
                var p = dr.GetInt32(pIndex);
                Assert.Equal(1, p);
                Assert.IsType<int>(p);
            }

            Assert.Equal(1, countLine);
        }

        [Fact]
        public void GetDecimal()
        {
            var dr = new ListDataReader<Order>(_list);
            var pIndex = dr.GetOrdinal("Total");

            var countLine = 0;
            while(dr.Read()){
                countLine++;
                var p = dr.GetDecimal(pIndex);
                Assert.Equal(12.3M, p);
                Assert.IsType<decimal>(p);
            }

            Assert.Equal(1, countLine);
        }


        [Fact]
        public void GetBool()
        {
            var dr = new ListDataReader<Order>(_list);
            var pIndex = dr.GetOrdinal("IsActive");

            var countLine = 0;
            while(dr.Read()){
                countLine++;
                var p = dr.GetBoolean(pIndex);
                Assert.False(p);
                Assert.IsType<bool>(p);
            }

            Assert.Equal(1, countLine);
        }

        [Fact]
        public void GetNotString()
        {
            var dr = new ListDataReader<Order>(_list);
            var pIndex = dr.GetOrdinal("Id");

            var countLine = 0;
            while(dr.Read()){
                countLine++;
                Assert.Throws<InvalidCastException>(() => dr.GetString(pIndex));
            }
        }
    }
}