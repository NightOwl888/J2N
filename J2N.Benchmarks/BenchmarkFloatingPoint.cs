using BenchmarkDotNet.Attributes;
using J2N.Globalization;
using System.Globalization;

namespace J2N.Benchmarks
{
    [MemoryDiagnoser]
    [MediumRunJob]
    public class BenchmarkFloatingPoint
    {

        public static int Iterations = 100000;
        public static string ShortHexString = "0x1.fffffep127";
        public static string LongHexString = "-0X000000000000000000000000000001abcdef.0000000000000000000000000001abefp00000000000000000000000000000000000000000004f";

        public static double SmallDouble = 3.14d;
        public static double LargeDouble = 3.14159265358979323846264338327950288419716939937510582097494459230781640628620899862803482534211706798214808651328230664709384460955058223172535940812848111745028410270193852110555964462294895493038196d;

        public static float SmallSingle = 3.14f;
        public static float LargeSingle = 3.14159265358979323846264338327950288419716939937510582097494459230781640628620899862803482534211706798214808651328230664709384460955058223172535940812848111745028410270193852110555964462294895493038196f;


        [Benchmark]
        public void Format_Double_Small_Ryu_Conservative()
        {
            for (int i = 0; i < Iterations; i++)
                J2N.Numerics.RyuDouble.ToString(SmallDouble, NumberFormatInfo.InvariantInfo, Numerics.RoundingMode.Conservative);
        }


        [Benchmark]
        public void Format_Double_Small_DotNet()
        {
            for (int i = 0; i < Iterations; i++)
                SmallDouble.ToString(NumberFormatInfo.InvariantInfo);
        }

        [Benchmark]
        public void Format_Double_Small_J2N_J_Format()
        {
            for (int i = 0; i < Iterations; i++)
                J2N.Numerics.Double.ToString(SmallDouble, NumberFormatInfo.InvariantInfo);
        }

        [Benchmark]
        public void Format_Double_Small_J2N_G_Format()
        {
            for (int i = 0; i < Iterations; i++)
                J2N.Numerics.Double.ToString(SmallDouble, "G", NumberFormatInfo.InvariantInfo);
        }


        [Benchmark]
        public void Format_Double_Large_Ryu_Conservative()
        {
            for (int i = 0; i < Iterations; i++)
                J2N.Numerics.RyuDouble.ToString(LargeDouble, NumberFormatInfo.InvariantInfo, Numerics.RoundingMode.Conservative);
        }

        [Benchmark]
        public void Format_Double_Large_DotNet()
        {
            for (int i = 0; i < Iterations; i++)
                LargeDouble.ToString(NumberFormatInfo.InvariantInfo);
        }

        [Benchmark]
        public void Format_Double_Large_J2N_J_Format()
        {
            for (int i = 0; i < Iterations; i++)
                J2N.Numerics.Double.ToString(LargeDouble, NumberFormatInfo.InvariantInfo);
        }

        [Benchmark]
        public void Format_Double_Large_J2N_G_Format()
        {
            for (int i = 0; i < Iterations; i++)
                J2N.Numerics.Double.ToString(LargeDouble, "G", NumberFormatInfo.InvariantInfo);
        }


        [Benchmark]
        public void Format_Double_MaxValue_Ryu_Conservative()
        {
            for (int i = 0; i < Iterations; i++)
                J2N.Numerics.RyuDouble.ToString(double.MaxValue, NumberFormatInfo.InvariantInfo, Numerics.RoundingMode.Conservative);
        }

        [Benchmark]
        public void Format_Double_MaxValue_DotNet()
        {
            for (int i = 0; i < Iterations; i++)
                double.MaxValue.ToString(NumberFormatInfo.InvariantInfo);
        }

        [Benchmark]
        public void Format_Double_MaxValue_J2N_J_Format()
        {
            for (int i = 0; i < Iterations; i++)
                J2N.Numerics.Double.ToString(double.MaxValue, NumberFormatInfo.InvariantInfo);
        }

        [Benchmark]
        public void Format_Double_MaxValue_J2N_G_Format()
        {
            for (int i = 0; i < Iterations; i++)
                J2N.Numerics.Double.ToString(double.MaxValue, "G", NumberFormatInfo.InvariantInfo);
        }


        [Benchmark]
        public void Format_Single_Small_Ryu_Conservative()
        {
            for (int i = 0; i < Iterations; i++)
                J2N.Numerics.RyuSingle.ToString(SmallSingle, NumberFormatInfo.InvariantInfo, Numerics.RoundingMode.Conservative);
        }

        [Benchmark]
        public void Format_Single_Small_DotNet()
        {
            for (int i = 0; i < Iterations; i++)
                SmallSingle.ToString(NumberFormatInfo.InvariantInfo);
        }

        [Benchmark]
        public void Format_Single_Small_J2N_J_Format()
        {
            for (int i = 0; i < Iterations; i++)
                J2N.Numerics.Single.ToString(SmallSingle, NumberFormatInfo.InvariantInfo);
        }

        [Benchmark]
        public void Format_Single_Small_J2N_G_Format()
        {
            for (int i = 0; i < Iterations; i++)
                J2N.Numerics.Single.ToString(SmallSingle, "G", NumberFormatInfo.InvariantInfo);
        }


        [Benchmark]
        public void Format_Single_Large_Ryu_Conservative()
        {
            for (int i = 0; i < Iterations; i++)
                J2N.Numerics.RyuSingle.ToString(LargeSingle, NumberFormatInfo.InvariantInfo, Numerics.RoundingMode.Conservative);
        }

        [Benchmark]
        public void Format_Single_Large_DotNet()
        {
            for (int i = 0; i < Iterations; i++)
                LargeSingle.ToString(NumberFormatInfo.InvariantInfo);
        }

        [Benchmark]
        public void Format_Single_Large_J2N_J_Format()
        {
            for (int i = 0; i < Iterations; i++)
                J2N.Numerics.Single.ToString(LargeSingle, NumberFormatInfo.InvariantInfo);
        }

        [Benchmark]
        public void Format_Single_Large_J2N_G_Format()
        {
            for (int i = 0; i < Iterations; i++)
                J2N.Numerics.Single.ToString(LargeSingle, "G", NumberFormatInfo.InvariantInfo);
        }


        [Benchmark]
        public void Format_Single_MaxValue_Ryu_Conservative()
        {
            for (int i = 0; i < Iterations; i++)
                J2N.Numerics.RyuSingle.ToString(float.MaxValue, NumberFormatInfo.InvariantInfo, Numerics.RoundingMode.Conservative);
        }

        [Benchmark]
        public void Format_Single_MaxValue_DotNet()
        {
            for (int i = 0; i < Iterations; i++)
                float.MaxValue.ToString(NumberFormatInfo.InvariantInfo);
        }

        [Benchmark]
        public void Format_Single_MaxValue_J2N_J_Format()
        {
            for (int i = 0; i < Iterations; i++)
                J2N.Numerics.Single.ToString(float.MaxValue, NumberFormatInfo.InvariantInfo);
        }

        [Benchmark]
        public void Format_Single_MaxValue_J2N_G_Format()
        {
            for (int i = 0; i < Iterations; i++)
                J2N.Numerics.Single.ToString(float.MaxValue, "G", NumberFormatInfo.InvariantInfo);
        }




        //[Benchmark]
        //public void Parse_Single_Hexadecimal_ShortString_DotNet()
        //{
        //    for (int i = 0; i < Iterations; i++)
        //    {
        //        J2N.Numerics.Single.Parse(ShortHexString, NumberStyle.HexFloat | NumberStyle.AllowTypeSpecifier, NumberFormatInfo.InvariantInfo);
        //    }
        //}

        ////[Benchmark]
        ////public void Parse_Single_Hexidecimal_ShortString_HexStringParser()
        ////{
        ////    for (int i = 0; i < Iterations; i++)
        ////    {
        ////        J2N.Numerics.HexStringParser.ParseSingle(ShortHexString, NumberStyle.HexFloat | NumberStyle.AllowTypeSpecifier, NumberFormatInfo.InvariantInfo);
        ////    }
        ////}

        //[Benchmark]
        //public void Parse_Single_Hexidecimal_ShortString_FloatingDecimal()
        //{
        //    for (int i = 0; i < Iterations; i++)
        //    {
        //        J2N.Numerics.FloatingDecimal.ParseFloat(ShortHexString);
        //    }
        //}

        //[Benchmark]
        //public void Parse_Double_Hexadecimal_ShortString_DotNet()
        //{
        //    for (int i = 0; i < Iterations; i++)
        //    {
        //        J2N.Numerics.Double.Parse(ShortHexString, NumberStyle.HexFloat | NumberStyle.AllowTypeSpecifier, NumberFormatInfo.InvariantInfo);
        //    }
        //}

        ////[Benchmark]
        ////public void Parse_Double_Hexidecimal_ShortString_HexStringParser()
        ////{
        ////    for (int i = 0; i < Iterations; i++)
        ////    {
        ////        J2N.Numerics.HexStringParser.ParseDouble(ShortHexString, NumberStyle.HexFloat | NumberStyle.AllowTypeSpecifier, NumberFormatInfo.InvariantInfo);
        ////    }
        ////}

        //[Benchmark]
        //public void Parse_Double_Hexidecimal_ShortString_FloatingDecimal()
        //{
        //    for (int i = 0; i < Iterations; i++)
        //    {
        //        J2N.Numerics.FloatingDecimal.ParseDouble(ShortHexString);
        //    }
        //}

        //[Benchmark]
        //public void Parse_Single_Hexadecimal_LongString_DotNet()
        //{
        //    for (int i = 0; i < Iterations; i++)
        //    {
        //        J2N.Numerics.Single.Parse(LongHexString, NumberStyle.HexFloat | NumberStyle.AllowTypeSpecifier, NumberFormatInfo.InvariantInfo);
        //    }
        //}

        ////[Benchmark]
        ////public void Parse_Single_Hexidecimal_LongString_HexStringParser()
        ////{
        ////    for (int i = 0; i < Iterations; i++)
        ////    {
        ////        J2N.Numerics.HexStringParser.ParseSingle(LongHexString, NumberStyle.HexFloat | NumberStyle.AllowTypeSpecifier, NumberFormatInfo.InvariantInfo);
        ////    }
        ////}

        //[Benchmark]
        //public void Parse_Single_Hexidecimal_LongString_FloatingDecimal()
        //{
        //    for (int i = 0; i < Iterations; i++)
        //    {
        //        J2N.Numerics.FloatingDecimal.ParseFloat(LongHexString);
        //    }
        //}

        //[Benchmark]
        //public void Parse_Double_Hexadecimal_LongString_DotNet()
        //{
        //    for (int i = 0; i < Iterations; i++)
        //    {
        //        J2N.Numerics.Double.Parse(LongHexString, NumberStyle.HexFloat | NumberStyle.AllowTypeSpecifier, NumberFormatInfo.InvariantInfo);
        //    }
        //}

        ////[Benchmark]
        ////public void Parse_Double_Hexidecimal_LongString_HexStringParser()
        ////{
        ////    for (int i = 0; i < Iterations; i++)
        ////    {
        ////        J2N.Numerics.HexStringParser.ParseDouble(LongHexString, NumberStyle.HexFloat | NumberStyle.AllowTypeSpecifier, NumberFormatInfo.InvariantInfo);
        ////    }
        ////}

        //[Benchmark]
        //public void Parse_Double_Hexidecimal_LongString_FloatingDecimal()
        //{
        //    for (int i = 0; i < Iterations; i++)
        //    {
        //        J2N.Numerics.FloatingDecimal.ParseDouble(LongHexString);
        //    }
        //}
    }
}
