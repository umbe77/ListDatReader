using System;
using BenchmarkDotNet.Running;

namespace listdatareader.benchmark
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<BenchMark>();
        }
    }
}
