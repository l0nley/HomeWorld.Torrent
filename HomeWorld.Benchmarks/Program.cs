using BenchmarkDotNet.Running;
using System;

namespace HomeWorld.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<CRC>();
            Console.ReadLine();
        }
    }
}
