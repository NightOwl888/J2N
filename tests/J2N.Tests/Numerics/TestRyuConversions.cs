using NUnit.Framework;
using RandomizedTesting.Generators;
using System;

namespace J2N.Numerics {
    public class TestRyuConversions : TestCase {

        private const int NUM_RANDOM_TESTS = 100000;

        [Test]
        public void DecimalOnly() {
            float f = .123f;

            string s = RyuConversion.FloatToString(f);
            assertEquals(".123", s);
        }

        [Test]
        public void SmallNumber() {
            float f = 123.45f;

            string s = RyuConversion.FloatToString(f);
            assertEquals("123.45", s);
        }


        [Test]
        public void ScientificNotation1() {
            float f = 1e7f;

            string s = RyuConversion.FloatToString(f);
            assertEquals("1.0E7", s);
        }

        [Test]
        public void ScientificNotation2() {
            float f = 1e7f + 1;

            string s = RyuConversion.FloatToString(f);
            assertEquals("1.0000001E7", s);
        }


        [Test]
        public void LargestNonScientificNotation() {
            float f = 1e7f - 1;

            string s = RyuConversion.FloatToString(f);
            assertEquals("9999999", s);
        }

        /**
         * This example was chosed because it's one that failed under the old 
         * float to string approach.
         */
        [Test]
        public void Example1() {
            float f = 12.90898f;

            string s = RyuConversion.FloatToString(f);
            assertEquals("12.90898", s);
        }

        /**
         * This example was chosed because it's one that failed under the old 
         * float to string approach.
         */
        [Test]
        public void Example2() {
            float f = 1.0e19f;

            string s = RyuConversion.FloatToString(f);
            assertEquals("1.0E19", s);
        }

        /**
         * This example was chosed because it's one that failed under the old 
         * float to string approach.
         */
        [Test]
        public void Example3() {
            float f = 1.0E-36f;

            string s = RyuConversion.FloatToString(f);
            assertEquals("1.0E-36", s);
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
                assertEquals($"Original value: {f[i].ToString("R")} or {f[i].ToHexString()} hexadecimal", expected: ofd.toJavaFormatString(), RyuConversion.FloatToString(f[i]));
            }
        }

        [Test]
        public void TestToJavaFormatStringFloatRandom_AgainstOldFloatingDecimalForTest()
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
                    OldFloatingDecimalForTest ofd = new OldFloatingDecimalForTest(f[j]);
                    assertEquals($"Original value: {f[j].ToString("R")} or {f[j].ToHexString()} hexadecimal", expected: ofd.toJavaFormatString(), RyuConversion.FloatToString(f[j]));
                }
            }
        }

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
                assertEquals($"Original value: {f[i].ToString("R")} or {f[i].ToHexString()} hexadecimal", expected: FloatingDecimal.ToJavaFormatString(f[i]), actual: RyuConversion.FloatToString(f[i]));

                // Check for round-trip
                assertEquals($"Failed to round trip: {f[i].ToString("R")} or {f[i].ToHexString()} hexadecimal", BitConversion.SingleToRawInt32Bits(f[i]), BitConversion.SingleToRawInt32Bits(FloatingDecimal.ParseFloat(RyuConversion.FloatToString(f[i]))));
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
                    assertEquals($"Original value: {f[j].ToString("R")} or {f[j].ToHexString()} hexadecimal", expected: FloatingDecimal.ToJavaFormatString(f[j]), actual: RyuConversion.FloatToString(f[j]));

                    // Check for round-trip
                    assertEquals($"Failed to round trip: {f[j].ToString("R")} or {f[j].ToHexString()} hexadecimal", BitConversion.SingleToRawInt32Bits(f[j]), BitConversion.SingleToRawInt32Bits(FloatingDecimal.ParseFloat(RyuConversion.FloatToString(f[j]))));
                }
            }
        }
    }
}
