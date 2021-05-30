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

using NUnit.Framework;
using RandomizedTesting.Generators;
using System;
using System.Globalization;

namespace J2N.Numerics
{
    public class TestRyuSingle : FloatToStringTest
    {
        private const int NUM_RANDOM_TESTS = 100000;

        internal override string f(float value, RoundingMode roundingMode)
        {
            return RyuSingle.FloatToString(value, roundingMode);
        }

        [Test]
        public void TestDecimalOnly()
        {
            float f = .123f;

            string s = RyuSingle.FloatToString(f);
            //assertEquals(".123", s);
            assertEquals("0.123", s); // Expected behavior of JDK 1.8
        }

        [Test]
        public void TestSmallNumber()
        {
            float f = 123.45f;

            string s = RyuSingle.FloatToString(f);
            assertEquals("123.45", s); // Expected behavior of JDK 1.8
        }


        [Test]
        public void TestScientificNotation1()
        {
            float f = 1e7f;

            string s = RyuSingle.FloatToString(f);
            assertEquals("1.0E7", s); // Expected behavior of JDK 1.8
        }

        [Test]
        public void TestScientificNotation2()
        {
            float f = 1e7f + 1;

            string s = RyuSingle.FloatToString(f);
            assertEquals("1.0000001E7", s); // Expected behavior of JDK 1.8
        }


        [Test]
        public void TestLargestNonScientificNotation()
        {
            float f = 1e7f - 1;

            string s = RyuSingle.FloatToString(f);
            //assertEquals("9999999", s);
            assertEquals("9999999.0", s); // Expected behavior of JDK 1.8
        }

        /**
         * This example was chosed because it's one that failed under the old 
         * float to string approach.
         */
        [Test]
        public void TestExample1()
        {
            float f = 12.90898f;

            string s = RyuSingle.FloatToString(f);
            assertEquals("12.90898", s); // Expected behavior of JDK 1.8
        }

        /**
         * This example was chosed because it's one that failed under the old 
         * float to string approach.
         */
        [Test]
        public void TestExample2()
        {
            float f = 1.0e19f;

            string s = RyuSingle.FloatToString(f);
            assertEquals("1.0E19", s); // Expected behavior of JDK 1.8
        }

        /**
         * This example was chosed because it's one that failed under the old 
         * float to string approach.
         */
        [Test]
        public void TestExample3()
        {
            float f = 1.0E-36f;

            string s = RyuSingle.FloatToString(f);
            assertEquals("1.0E-36", s); // Expected behavior of JDK 1.8
        }

        [Test]
        public void TestToJavaFormatStringFloatFixed_AgainstOldFloatingDecimalForTest()
        {
            float[] f = new float[] {
                -9.8784166e8f, // dtoa() fast path
                0.70443946f,   // dtoa() fast iterative - int
                1.8254228e37f  // dtoa() slow iterative
            };

            for (int i = 0; i < f.Length; i++)
            {
                OldFloatingDecimalForTest ofd = new OldFloatingDecimalForTest(f[i]);
                assertEquals($"Original value: {f[i].ToString("R")} or {f[i].ToHexString()} hexadecimal", expected: ofd.toJavaFormatString(), RyuSingle.FloatToString(f[i]));
            }
        }

        //[Test]
        //public void TestToJavaFormatStringFloatRandom_AgainstOldFloatingDecimalForTest()
        //{
        //    for (int i = 0; i < NUM_RANDOM_TESTS; i++)
        //    {
        //        float[] f = new float[] {
        //            Random.Next(),
        //            (float)Random.NextGaussian(),
        //            Random.NextSingle()*float.MaxValue
        //        };
        //        for (int j = 0; j < f.Length; j++)
        //        {
        //            OldFloatingDecimalForTest ofd = new OldFloatingDecimalForTest(f[j]);
        //            assertEquals($"Original value: {f[j].ToString("R")} or {f[j].ToHexString()} hexadecimal", expected: ofd.toJavaFormatString(), RyuFloat.FloatToString(f[j]));
        //        }
        //    }
        //}

        [Test]
        public void TestToJavaFormatStringFloatFixed_AgainstFloatingDecimal()
        {
            float[] f = new float[] {
                -9.8784166e8f, // dtoa() fast path
                0.70443946f,   // dtoa() fast iterative - int
                1.8254228e37f  // dtoa() slow iterative
            };

            for (int i = 0; i < f.Length; i++)
            {
                //assertEquals($"Original value: {f[i].ToString("R")} or {f[i].ToHexString()} hexadecimal", expected: FloatingDecimal.ToJavaFormatString(f[i]), actual: RyuFloat.FloatToString(f[i]));

                // Check for round-trip
                assertEquals($"Failed to round trip: {f[i].ToString("R")} or {f[i].ToHexString()} hexadecimal", BitConversion.SingleToRawInt32Bits(f[i]), BitConversion.SingleToRawInt32Bits(FloatingDecimal.ParseFloat(RyuSingle.FloatToString(f[i]))));
            }
        }

        [Test]
        public void TestToJavaFormatStringFloatRandom_AgainstFloatingDecimal()
        {
            for (int i = 0; i < NUM_RANDOM_TESTS; i++)
            {
                float[] f = new float[] {
                    Random.Next(),
                    (float)Random.NextGaussian(),
                    Random.NextSingle()*float.MaxValue
                };
                for (int j = 0; j < f.Length; j++)
                {
                    //assertEquals($"Original value: {f[j].ToString("R")} or {f[j].ToHexString()} hexadecimal", expected: FloatingDecimal.ToJavaFormatString(f[j]), actual: RyuFloat.FloatToString(f[j], RoundingMode.RoundEven));

                    // Check for round-trip
                    assertEquals($"Failed to round trip: {f[j].ToString("R")} or {f[j].ToHexString()} hexadecimal", BitConversion.SingleToRawInt32Bits(f[j]), BitConversion.SingleToRawInt32Bits(FloatingDecimal.ParseFloat(RyuSingle.FloatToString(f[j], RoundingMode.RoundEven))));

                    // Check for round-trip (.NET)
                    assertEquals($"Failed to round trip (.NET): {f[j].ToString("R")} or {f[j].ToHexString()} hexadecimal", BitConversion.SingleToRawInt32Bits(f[j]), BitConversion.SingleToRawInt32Bits(float.Parse(RyuSingle.FloatToString(f[j], RoundingMode.RoundEven), CultureInfo.InvariantCulture)));
                }
            }
        }
    }
}