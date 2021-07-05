using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lucene.Net.Util;

namespace J2N.Benchmarks
{
    [MemoryDiagnoser]
    [MediumRunJob]
    public class BenchmarkCharacter
    {
        public static int Iterations = 1000000;

        //public static string ShortString = "Name";
        //public static string MaxStackString = new string('s', 256);
        //public static string MinHeapString = new string('h', 257);
        //public static string LargeHeapString = new string('l', 1025);

        private static char[] CharBuffer;
        private static int[] SurrogateCodePoints = new int[] { 65536, 65537, 65538, 65539, 65540, 65541, 65542, 65543, 65544, 65545 };

        //[IterationSetup]
        //public void IterationSetup()
        //{
        //    CharBuffer = new char[20];
        //}

        //[Benchmark]
        //public void ToChars_Harmony()
        //{
        //    for (int i = 0; i < Iterations; i++)
        //        for (int j = 0; j < SurrogateCodePoints.Length; j++)
        //            Character.ToChars_Baseline(SurrogateCodePoints[j], CharBuffer, j * 2);
        //}

        //[Benchmark]
        //public void ToChars_Lucene()
        //{
        //    for (int i = 0; i < Iterations; i++)
        //        for (int j = 0; j < SurrogateCodePoints.Length; j++)
        //            Character.ToChars(SurrogateCodePoints[j], CharBuffer, j * 2);
        //}

        //[Benchmark]
        //public void ToString_Heap_Baseline()
        //{
        //    for (int i = 0; i < Iterations; i++)
        //        Character.ToString_Baseline(new int[] { 'H', 'e', 'l', 'l',
        //            'o', 'W', 'o', 'r', 'l', 'd' }, 0, 10);
        //}

        //[Benchmark]
        //public void ToString_Stack_Measured()
        //{
        //    for (int i = 0; i < Iterations; i++)
        //        Character.ToString_Measured(new int[] { 'H', 'e', 'l', 'l',
        //            'o', 'W', 'o', 'r', 'l', 'd' }, 0, 10);
        //}

        [Benchmark]
        public void ToString_Stack_Estimated()
        {
            for (int i = 0; i < Iterations; i++)
                Character.ToString(new int[] { 'H', 'e', 'l', 'l',
                    'o', 'W', 'o', 'r', 'l', 'd' }, 0, 10);
        }

        [Benchmark]
        public void UnicodeUtil_NewString_Heap()
        {
            for (int i = 0; i < Iterations; i++)
                UnicodeUtil.NewString(new int[] { 'H', 'e', 'l', 'l',
                    'o', 'W', 'o', 'r', 'l', 'd' }, 0, 10);
        }

        //[Benchmark]
        //public void Digit_Ascii_Hexadecimal_Calculated()
        //{
        //    for (int i = 0; i < Iterations; i++)
        //    {
        //        for (int c = 0; c < 256; c++)
        //        {
        //            var _ = Character.Digit(c, 16);
        //        }
        //    }
        //}
        // }

        //[Benchmark]
        //public void Digit_Ascii_Hexadecimal_Calculated()
        //{
        //    for (int i = 0; i < Iterations; i++)
        //    {
        //        for (int c = 0; c < 256; c++)
        //        {
        //            var _ = Character.Digit(c, 16);
        //        }
        //    }
        //}

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
