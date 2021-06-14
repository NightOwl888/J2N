using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace J2N.Benchmarks
{
    [MemoryDiagnoser]
    public class BenchmarkCharacter
    {
        public static int Iterations = 100000;

        [Benchmark]
        public void Digit_Ascii_Hexadecimal_Calculated()
        {
            for (int i = 0; i < Iterations; i++)
            {
                for (int c = 0; c < 256; c++)
                {
                    var _ = Character.Digit(c, 16);
                }
            }
        }

        //[Benchmark]
        //public void Digit_Ascii_Hexadecimal_ReadOnlySpanLookup()
        //{
        //    for (int i = 0; i < Iterations; i++)
        //    {
        //        for (int c = 0; c < 256; c++)
        //        {
        //            var _ = Character.Digit2(c, 16);
        //        }
        //    }
        //}

        //[Benchmark]
        //public void Digit_Ascii_Hexadecimal_StaticArrayLookup()
        //{
        //    for (int i = 0; i < Iterations; i++)
        //    {
        //        for (int c = 0; c < 256; c++)
        //        {
        //            var _ = Character.Digit3(c, 16);
        //        }
        //    }
        //}
    }
}
