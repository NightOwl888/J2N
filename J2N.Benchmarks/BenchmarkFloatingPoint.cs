using BenchmarkDotNet.Attributes;
using J2N.Globalization;
using System.Globalization;

namespace J2N.Benchmarks
{
    [MemoryDiagnoser]
    public class BenchmarkFloatingPoint
    {
        public static int Iterations = 1000;
        public static string ShortString = "0x1.fffffep127";
        public static string LongString = "-0X000000000000000000000000000001abcdef.0000000000000000000000000001abefp00000000000000000000000000000000000000000004f";

        [Benchmark]
        public void Parse_Single_Hexadecimal_ShortString_DotNet()
        {
            for (int i = 0; i < Iterations; i++)
            {
                J2N.Numerics.Single.Parse(ShortString, NumberStyle.HexFloat | NumberStyle.AllowTypeSpecifier, NumberFormatInfo.InvariantInfo);
            }
        }

        //[Benchmark]
        //public void Parse_Single_Hexidecimal_ShortString_HexStringParser()
        //{
        //    for (int i = 0; i < Iterations; i++)
        //    {
        //        J2N.Numerics.HexStringParser.ParseSingle(ShortString, NumberStyle.HexFloat | NumberStyle.AllowTypeSpecifier, NumberFormatInfo.InvariantInfo);
        //    }
        //}

        [Benchmark]
        public void Parse_Single_Hexidecimal_ShortString_FloatingDecimal()
        {
            for (int i = 0; i < Iterations; i++)
            {
                J2N.Numerics.FloatingDecimal.ParseFloat(ShortString);
            }
        }

        [Benchmark]
        public void Parse_Double_Hexadecimal_ShortString_DotNet()
        {
            for (int i = 0; i < Iterations; i++)
            {
                J2N.Numerics.Double.Parse(ShortString, NumberStyle.HexFloat | NumberStyle.AllowTypeSpecifier, NumberFormatInfo.InvariantInfo);
            }
        }

        //[Benchmark]
        //public void Parse_Double_Hexidecimal_ShortString_HexStringParser()
        //{
        //    for (int i = 0; i < Iterations; i++)
        //    {
        //        J2N.Numerics.HexStringParser.ParseDouble(ShortString, NumberStyle.HexFloat | NumberStyle.AllowTypeSpecifier, NumberFormatInfo.InvariantInfo);
        //    }
        //}

        [Benchmark]
        public void Parse_Double_Hexidecimal_ShortString_FloatingDecimal()
        {
            for (int i = 0; i < Iterations; i++)
            {
                J2N.Numerics.FloatingDecimal.ParseDouble(ShortString);
            }
        }

        [Benchmark]
        public void Parse_Single_Hexadecimal_LongString_DotNet()
        {
            for (int i = 0; i < Iterations; i++)
            {
                J2N.Numerics.Single.Parse(LongString, NumberStyle.HexFloat | NumberStyle.AllowTypeSpecifier, NumberFormatInfo.InvariantInfo);
            }
        }

        //[Benchmark]
        //public void Parse_Single_Hexidecimal_LongString_HexStringParser()
        //{
        //    for (int i = 0; i < Iterations; i++)
        //    {
        //        J2N.Numerics.HexStringParser.ParseSingle(LongString, NumberStyle.HexFloat | NumberStyle.AllowTypeSpecifier, NumberFormatInfo.InvariantInfo);
        //    }
        //}

        [Benchmark]
        public void Parse_Single_Hexidecimal_LongString_FloatingDecimal()
        {
            for (int i = 0; i < Iterations; i++)
            {
                J2N.Numerics.FloatingDecimal.ParseFloat(LongString);
            }
        }

        [Benchmark]
        public void Parse_Double_Hexadecimal_LongString_DotNet()
        {
            for (int i = 0; i < Iterations; i++)
            {
                J2N.Numerics.Double.Parse(LongString, NumberStyle.HexFloat | NumberStyle.AllowTypeSpecifier, NumberFormatInfo.InvariantInfo);
            }
        }

        //[Benchmark]
        //public void Parse_Double_Hexidecimal_LongString_HexStringParser()
        //{
        //    for (int i = 0; i < Iterations; i++)
        //    {
        //        J2N.Numerics.HexStringParser.ParseDouble(LongString, NumberStyle.HexFloat | NumberStyle.AllowTypeSpecifier, NumberFormatInfo.InvariantInfo);
        //    }
        //}

        [Benchmark]
        public void Parse_Double_Hexidecimal_LongString_FloatingDecimal()
        {
            for (int i = 0; i < Iterations; i++)
            {
                J2N.Numerics.FloatingDecimal.ParseDouble(LongString);
            }
        }
    }
}
