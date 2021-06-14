using BenchmarkDotNet.Running;
using System;

namespace J2N.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<BenchmarkCharacter>();
            //Console.WriteLine("Hello World!");
        }
    }
}
