#region Copyright 2018 by Ulf Adams, Licensed under the Apache License, Version 2.0
// Copyright 2018 Ulf Adams
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using NUnit.Framework;
using RandomizedTesting.Generators;
using System;
using System.Globalization;

namespace J2N.Numerics
{
    public class TestRyuDouble : DoubleToStringTest
    {
        private const int NUM_RANDOM_TESTS = 100000;

        internal override string f(double value, RoundingMode roundingMode)
        {
            return RyuDouble.ToString(value, NumberFormatInfo.InvariantInfo, roundingMode);
        }

        //[Test]
        //public void TestToJavaFormatStringDoubleFixed_AgainstOldFloatingDecimalForTest()
        //{
        //    double[] d = new double[] {
        //        -5.9522650387500933e18, // dtoa() fast path
        //        0.872989018674569,      // dtoa() fast iterative - long
        //        1.1317400099603851e308  // dtoa() slow iterative
        //    };

        //    for (int i = 0; i < d.Length; i++)
        //    {
        //        OldFloatingDecimalForTest ofd = new OldFloatingDecimalForTest(d[i]);
        //        assertEquals($"Original value: {d[i].ToString("R")} or {d[i].ToHexString(NumberFormatInfo.InvariantInfo)} hexadecimal", expected: ofd.toJavaFormatString(), RyuDouble.ToString(d[i]));
        //    }
        //}

        //[Test]
        //public void TestToJavaFormatStringDoubleRandom_AgainstOldFloatingDecimalForTest()
        //{
        //    for (int i = 0; i < NUM_RANDOM_TESTS; i++)
        //    {
        //        double[] d = new double[] {
        //            Random.NextInt64(),
        //            Random.NextGaussian(),
        //            Random.NextDouble()*double.MaxValue
        //        };
        //        for (int j = 0; j < d.Length; j++)
        //        {
        //            OldFloatingDecimalForTest ofd = new OldFloatingDecimalForTest(d[j]);
        //            assertEquals($"Original value: {d[j].ToString("R")} or {d[j].ToHexString(NumberFormatInfo.InvariantInfo)} hexadecimal", expected: ofd.toJavaFormatString(), RyuDouble.ToString(d[j]));
        //        }
        //    }
        //}

        [Test]
        public void TestToJavaFormatStringDoubleFixed_AgainstFloatingDecimal()
        {
            double[] d = new double[] {
                -5.9522650387500933e18, // dtoa() fast path
                0.872989018674569,      // dtoa() fast iterative - long
                1.1317400099603851e308  // dtoa() slow iterative
            };

            for (int i = 0; i < d.Length; i++)
            {
                //assertEquals($"Original value: {d[i].ToString("R")} or {d[i].ToHexString(CultureInfo.InvariantCulture)} hexadecimal", expected: FloatingDecimal.ToJavaFormatString(d[i]), actual: RyuDouble.ToString(d[i]));

                // Check for round-trip
                assertEquals($"Failed to round trip: {d[i]:R} or {d[i].ToHexString(CultureInfo.InvariantCulture)} hexadecimal", BitConversion.DoubleToRawInt64Bits(d[i]), BitConversion.DoubleToRawInt64Bits(FloatingDecimal.ParseDouble(RyuDouble.ToString(d[i], NumberFormatInfo.InvariantInfo))));
            }
        }

        [Test]
        public void TestToJavaFormatStringDoubleRandom_AgainstFloatingDecimal()
        {
            //var x = RyuDouble.ToString(3.14159, NumberFormatInfo.InvariantInfo, roundingMode: RoundingMode.Conservative);
            //var x = RyuDouble.ToString(123456789, NumberFormatInfo.InvariantInfo, roundingMode: RoundingMode.Conservative);
            //var x = RyuDouble.ToString(double.MaxValue, NumberFormatInfo.InvariantInfo, roundingMode: RoundingMode.Conservative);

            for (int i = 0; i < NUM_RANDOM_TESTS; i++)
            {
                double[] d = new double[] {
                        Random.NextInt64(),
                        Random.NextGaussian(),
                        Random.NextDouble()*double.MaxValue
                    };
                for (int j = 0; j < d.Length; j++)
                {
                    OldFloatingDecimalForTest ofd = new OldFloatingDecimalForTest(d[j]);
                    //assertEquals($"Original value: {d[j].ToHexString()} or {d[j].ToHexString()} hexadecimal", ofd.toJavaFormatString(), RyuDouble.ToString(d[j]));

                    // Check for round-trip
                    assertEquals($"Failed to round trip: {d[j]:R} or {d[j].ToHexString(CultureInfo.InvariantCulture)} hexadecimal", BitConversion.DoubleToRawInt64Bits(d[j]), BitConversion.DoubleToRawInt64Bits(FloatingDecimal.ParseDouble(RyuDouble.ToString(d[j], NumberFormatInfo.InvariantInfo))));

                    // Check for round-trip against .NET
                    // NOTE: on .NET 5 and .NET Core 3+, round tripping works, however in other versions it is sometimes off by 1 bit.

                    //assertEquals($"Failed to round trip (.NET): {d[j].ToString("R")} or {d[j].ToHexString(CultureInfo.InvariantCulture)} hexadecimal", BitConversion.DoubleToRawInt64Bits(d[j]).ToBinaryString(), BitConversion.DoubleToRawInt64Bits(double.Parse(RyuDouble.ToString(d[j]), CultureInfo.InvariantCulture)).ToBinaryString());

                    // Check for round-trip against J2N.Numerics.Double
                    assertEquals($"Failed to round trip (.NET parser in J2N): {d[j]:R} or {d[j].ToHexString(CultureInfo.InvariantCulture)} hexadecimal", BitConversion.DoubleToRawInt64Bits(d[j]), BitConversion.DoubleToRawInt64Bits(Double.Parse(RyuDouble.ToString(d[j], NumberFormatInfo.InvariantInfo), CultureInfo.InvariantCulture)));

                    //assertEquals($"Failed to round trip (.NET parser in J2N): {d[j].ToString("R")} or {d[j].ToHexString(CultureInfo.InvariantCulture)} hexadecimal", BitConversion.DoubleToRawInt64Bits(d[j]), BitConversion.DoubleToRawInt64Bits(Double.Parse(RyuDouble.ToString(d[j], new NumberFormatInfo { NumberDecimalSeparator = ",,," }, RoundingMode.Conservative), new NumberFormatInfo { NumberDecimalSeparator = ",,," })));
                }
            }
        }

        [Test]
        public void TestFixedDoubleRoundTrip()
        {
            //double d = 1.1317400099603851e308;
            //double d = 1.234d;
            //double d = -6.180646536066727E+18;
            double d = 1.9189729408694876; // Regression that produced divide by zero exception

            string doubleString = RyuDouble.ToString(d, NumberFormatInfo.InvariantInfo);
            long expectedBits = BitConversion.DoubleToRawInt64Bits(d);

            // Check for round-trip
            assertEquals($"Failed to round trip: {d:R} or {d.ToHexString(NumberFormatInfo.InvariantInfo)} hexadecimal. The string: {doubleString}", expectedBits, BitConversion.DoubleToRawInt64Bits(FloatingDecimal.ParseDouble(doubleString)));

            // Check for round-trip against J2N.Numerics.Double
            assertEquals($"Failed to round trip (.NET parser in J2N): {d:R} or {d.ToHexString(NumberFormatInfo.InvariantInfo)} hexadecimal. The string: {doubleString}", expectedBits, BitConversion.DoubleToRawInt64Bits(Double.Parse(doubleString, CultureInfo.InvariantCulture)));
        }
    }
}
