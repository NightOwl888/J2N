using J2N.Text;
using NUnit.Framework;
using RandomizedTesting.Generators;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace J2N.Numerics
{
    public class TestFloatingDecimal : TestCase
    {
        //private enum ResultType
        //{
        //    RESULT_EXCEPTION,
        //    RESULT_PRINT
        //}

        //private static readonly ResultType RESULT_TYPE = ResultType.RESULT_PRINT;
        private const int NUM_RANDOM_TESTS = 100000;

        //private static readonly Random RANDOM = new Random();

        //private static void result(string message)
        //{
        //    switch (RESULT_TYPE)
        //    {
        //        case ResultType.RESULT_EXCEPTION:
        //            throw new Exception(message);
        //        case ResultType.RESULT_PRINT:
        //            Console.Error.WriteLine(message);
        //            break;
        //        default:
        //            Debug.Assert(false);
        //            break;
        //    }
        //}

        private static int check(String test, Object expected, Object actual)
        {
            assertEquals(test, expected, actual);
            return 0;

            //int failures = 0;
            //if (!actual.Equals(expected))
            //{
            //    failures++;
            //    result("Test " + test + " expected " + expected + " but obtained " + actual);
            //}
            //return failures;
        }

        [Test]
        public void TestAppendToDouble()
        {
            Console.WriteLine("  testAppendToDouble");
            int failures = 0;

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
                    StringBuilderCharSequence sb = new StringBuilderCharSequence();
                    ofd.appendTo(sb);
                    String oldString = sb.ToString();
                    sb = new StringBuilderCharSequence();
                    FloatingDecimal.AppendTo(d[j], sb);
                    String newString = sb.ToString();
                    if (!oldString.Equals(newString))
                    {

                    }

                    failures += check("testAppendToDouble", oldString, newString);
                }
            }

            //return failures;
        }

        [Test]
        public void TestAppendToFloat()
        {
            Console.WriteLine("  testAppendToFloat");
            int failures = 0;

            for (int i = 0; i < NUM_RANDOM_TESTS; i++)
            {
                float[] f = new float[] {
                Random.NextInt64(),
                (float)Random.NextGaussian(),
                Random.NextSingle()*float.MaxValue
            };
                for (int j = 0; j < f.Length; j++)
                {
                    OldFloatingDecimalForTest ofd = new OldFloatingDecimalForTest(f[j]);
                    StringBuilderCharSequence sb = new StringBuilderCharSequence();
                    ofd.appendTo(sb);
                    String oldString = sb.ToString();
                    sb = new StringBuilderCharSequence();
                    FloatingDecimal.AppendTo(f[j], sb);
                    String newString = sb.ToString();
                    failures += check("testAppendToFloat", oldString, newString);
                }
            }

            //return failures;
        }

        //[Test]
        //public void TestAppendTo()
        //{
        //    Console.WriteLine("testAppendTo");
        //    int failures = 0;

        //    failures += testAppendToDouble();
        //    failures += testAppendToFloat();

        //    //return failures;
        //}

        [Test]
        public void TestParseDouble()
        {
            Console.WriteLine("  testParseDouble");
            int failures = 0;

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
                    String javaFormatString = ofd.toJavaFormatString();
                    ofd = OldFloatingDecimalForTest.readJavaFormatString(javaFormatString);
                    double oldDouble = ofd.doubleValue();
                    string newString = FloatingDecimal.ToJavaFormatString(d[j]);
                    double newDouble = FloatingDecimal.ParseDouble(javaFormatString);
                    failures += check("testParseDouble", oldDouble, newDouble);
                }
            }

            //return failures;
        }

        [Test]
        public void TestParseFloat()
        {
            Console.WriteLine("  testParseFloat");
            int failures = 0;

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
                    String javaFormatString = ofd.toJavaFormatString();
                    ofd = OldFloatingDecimalForTest.readJavaFormatString(javaFormatString);
                    float oldFloat = ofd.floatValue();
                    float newFloat = FloatingDecimal.ParseFloat(javaFormatString);
                    failures += check("testParseFloat", oldFloat, newFloat);
                }
            }

            //return failures;
        }

        //[Test]
        //public void TestParse()
        //{
        //    Console.WriteLine("testParse");
        //    int failures = 0;

        //    failures += testParseDouble();
        //    failures += testParseFloat();

        //    return failures;
        //}

        [Test]
        public void TestToJavaFormatStringDoubleFixed()
        {
            Console.WriteLine("    testToJavaFormatStringDoubleFixed");
            int failures = 0;

            double[] d = new double[] {
                -5.9522650387500933e18, // dtoa() fast path
                0.872989018674569,      // dtoa() fast iterative - long
                1.1317400099603851e308  // dtoa() slow iterative
            };

            for (int i = 0; i < d.Length; i++)
            {
                OldFloatingDecimalForTest ofd = new OldFloatingDecimalForTest(d[i]);
                failures += check($"Original value: {d[i].ToHexString()} or {d[i].ToHexString()} hexadecimal", ofd.toJavaFormatString(), FloatingDecimal.ToJavaFormatString(d[i]));

                // Check for round-trip
                assertEquals($"Failed to round trip: {d[i].ToString("R")} or {d[i].ToHexString()} hexadecimal", BitConversion.DoubleToRawInt64Bits(d[i]), BitConversion.DoubleToRawInt64Bits(FloatingDecimal.ParseDouble(FloatingDecimal.ToJavaFormatString(d[i]))));

                // Check for round-trip against .NET
                assertEquals($"Failed to round trip (.NET): {d[i].ToString("R")} or {d[i].ToHexString()} hexadecimal", BitConversion.DoubleToRawInt64Bits(d[i]), BitConversion.DoubleToRawInt64Bits(double.Parse(FloatingDecimal.ToJavaFormatString(d[i]), CultureInfo.InvariantCulture)));
            }

            //return failures;
        }

        [Test]
        [Ignore(".NET Framework, .NET Core 2.x and Xamarin.Android are off by 1 bit when round tripping using the .NET parser in certain cases, likely due to the documented issue that we need at least 17 decimal digits to be round-trippable")]
        public void TestToJavaFormatStringDoubleRandom()
        {
            Console.WriteLine("    testToJavaFormatStringDoubleRandom");
            int failures = 0;

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
                    failures += check($"Original value: {d[j].ToHexString()} or {d[j].ToHexString()} hexadecimal", ofd.toJavaFormatString(), FloatingDecimal.ToJavaFormatString(d[j]));

                    // Check for round-trip
                    assertEquals($"Failed to round trip: {d[j].ToString("R")} or {d[j].ToHexString()} hexadecimal", BitConversion.DoubleToRawInt64Bits(d[j]), BitConversion.DoubleToRawInt64Bits(FloatingDecimal.ParseDouble(FloatingDecimal.ToJavaFormatString(d[j]))));

                    // Check for round-trip against .NET
                    assertEquals($"Failed to round trip (.NET): {d[j].ToString("R")} or {d[j].ToHexString()} hexadecimal", BitConversion.DoubleToRawInt64Bits(d[j]).ToBinaryString(), BitConversion.DoubleToRawInt64Bits(double.Parse(FloatingDecimal.ToJavaFormatString(d[j]), CultureInfo.InvariantCulture)).ToBinaryString());
                }
            }

            //return failures;
        }

        //[Test]
        //public int testToJavaFormatStringDouble()
        //{
        //    Console.WriteLine("  testToJavaFormatStringDouble");
        //    int failures = 0;
        //    failures += testToJavaFormatStringDoubleFixed();
        //    failures += testToJavaFormatStringDoubleRandom();
        //    return failures;
        //}

        [Test]
        public void TestToJavaFormatStringFloatFixed()
        {
            Console.WriteLine("    testToJavaFormatStringFloatFixed");
            int failures = 0;

            float[] f = new float[] {
                -9.8784166e8f, // dtoa() fast path
                0.70443946f,   // dtoa() fast iterative - int
                1.8254228e37f  // dtoa() slow iterative
            };

            for (int i = 0; i < f.Length; i++)
            {
                OldFloatingDecimalForTest ofd = new OldFloatingDecimalForTest(f[i]);
                failures += check($"Original value: {f[i].ToHexString()} or {f[i].ToHexString()} hexadecimal", ofd.toJavaFormatString(), FloatingDecimal.ToJavaFormatString(f[i]));

                // Check for round-trip
                assertEquals($"Failed to round trip: {f[i].ToString("R")} or {f[i].ToHexString()} hexadecimal", BitConversion.SingleToRawInt32Bits(f[i]), BitConversion.SingleToRawInt32Bits(FloatingDecimal.ParseFloat(FloatingDecimal.ToJavaFormatString(f[i]))));

                // Check for round-trip against .NET
                assertEquals($"Failed to round trip (.NET): {f[i].ToString("R")} or {f[i].ToHexString()} hexadecimal", BitConversion.SingleToRawInt32Bits(f[i]), BitConversion.SingleToRawInt32Bits(float.Parse(FloatingDecimal.ToJavaFormatString(f[i]), CultureInfo.InvariantCulture)));
            }

            //return failures;
        }

        [Test]
        public void TestToJavaFormatStringFloatRandom()
        {
            Console.WriteLine("    testToJavaFormatStringFloatRandom");
            int failures = 0;

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
                    failures += check($"Original value: {f[j].ToHexString()} or {f[j].ToHexString()} hexadecimal", ofd.toJavaFormatString(), FloatingDecimal.ToJavaFormatString(f[j]));

                    // Check for round-trip
                    assertEquals($"Failed to round trip: {f[j].ToString("R")} or {f[j].ToHexString()} hexadecimal", BitConversion.SingleToRawInt32Bits(f[j]), BitConversion.SingleToRawInt32Bits(FloatingDecimal.ParseFloat(FloatingDecimal.ToJavaFormatString(f[j]))));

                    // Check for round-trip against .NET
                    assertEquals($"Failed to round trip (.NET): {f[j].ToString("R")} or {f[j].ToHexString()} hexadecimal", BitConversion.SingleToRawInt32Bits(f[j]), BitConversion.SingleToRawInt32Bits(float.Parse(FloatingDecimal.ToJavaFormatString(f[j]), CultureInfo.InvariantCulture)));
                }
            }

            //return failures;
        }

        //[Test]
        //public int testToJavaFormatStringFloat()
        //{
        //    Console.WriteLine("  testToJavaFormatStringFloat");
        //    int failures = 0;

        //    failures += testToJavaFormatStringFloatFixed();
        //    failures += testToJavaFormatStringFloatRandom();

        //    return failures;
        //}

        //[Test]
        //public int testToJavaFormatString()
        //{
        //    Console.WriteLine("testToJavaFormatString");
        //    int failures = 0;

        //    failures += testToJavaFormatStringDouble();
        //    failures += testToJavaFormatStringFloat();

        //    return failures;
        //}

        //public static void main(String[] args)
        //{
        //    int failures = 0;

        //    failures += testAppendTo();
        //    failures += testParse();
        //    failures += testToJavaFormatString();

        //    if (failures != 0)
        //    {
        //        throw new RuntimeException("" + failures + " failures while testing FloatingDecimal");
        //    }
        //}

        public static readonly NumberFormatInfo NumberNegativePattern0 = new NumberFormatInfo { NumberNegativePattern = 0 };
        public static readonly NumberFormatInfo NumberNegativePattern1 = new NumberFormatInfo { NumberNegativePattern = 1, NegativeSign = "-" };
        public static readonly NumberFormatInfo NumberNegativePattern1_Sign_neg = new NumberFormatInfo { NumberNegativePattern = 1, NegativeSign = "neg" };
        public static readonly NumberFormatInfo NumberNegativePattern2 = new NumberFormatInfo { NumberNegativePattern = 2, NegativeSign = "-" };
        public static readonly NumberFormatInfo NumberNegativePattern2_Sign_neg = new NumberFormatInfo { NumberNegativePattern = 2, NegativeSign = "neg" };
        public static readonly NumberFormatInfo NumberNegativePattern3 = new NumberFormatInfo { NumberNegativePattern = 3, NegativeSign = "-" };
        public static readonly NumberFormatInfo NumberNegativePattern3_Sign_neg = new NumberFormatInfo { NumberNegativePattern = 3, NegativeSign = "neg" };
        public static readonly NumberFormatInfo NumberNegativePattern4 = new NumberFormatInfo { NumberNegativePattern = 4, NegativeSign = "-" };
        public static readonly NumberFormatInfo NumberNegativePattern4_Sign_neg = new NumberFormatInfo { NumberNegativePattern = 4, NegativeSign = "neg" };

        public static IEnumerable<TestCaseData> IsNegativeTestData
        {
            get
            {
                yield return new TestCaseData(NumberNegativePattern0, "(0.00)", true);
                yield return new TestCaseData(NumberNegativePattern0, "(0.00", false);
                yield return new TestCaseData(NumberNegativePattern0, "0.00)", false);
                yield return new TestCaseData(NumberNegativePattern0, "-0.00", false);
                yield return new TestCaseData(NumberNegativePattern0, "- 0.00", false);

                yield return new TestCaseData(NumberNegativePattern1, "-0.00", true);
                yield return new TestCaseData(NumberNegativePattern1, "- 0.00", false);
                yield return new TestCaseData(NumberNegativePattern1, "(0.00)", false);
                yield return new TestCaseData(NumberNegativePattern1, "0.00-", false);
                yield return new TestCaseData(NumberNegativePattern1, "0.00 -", false);

                yield return new TestCaseData(NumberNegativePattern1_Sign_neg, "neg0.00", true);
                yield return new TestCaseData(NumberNegativePattern1_Sign_neg, "neg 0.00", false);

                yield return new TestCaseData(NumberNegativePattern2, "- 0.00", true);
                yield return new TestCaseData(NumberNegativePattern2, "-0.00", false);
                yield return new TestCaseData(NumberNegativePattern2, "(0.00)", false);

                yield return new TestCaseData(NumberNegativePattern2_Sign_neg, "neg 0.00", true);
                yield return new TestCaseData(NumberNegativePattern2_Sign_neg, "neg0.00", false);

                yield return new TestCaseData(NumberNegativePattern3, "0.00-", true);
                yield return new TestCaseData(NumberNegativePattern3, "0.00 -", false);
                yield return new TestCaseData(NumberNegativePattern3, "(0.00)", false);
                yield return new TestCaseData(NumberNegativePattern3, "-0.00", false);
                yield return new TestCaseData(NumberNegativePattern3, "- 0.00", false);

                yield return new TestCaseData(NumberNegativePattern3_Sign_neg, "0.00neg", true);
                yield return new TestCaseData(NumberNegativePattern3_Sign_neg, "0.00 neg", false);

                yield return new TestCaseData(NumberNegativePattern4, "0.00-", false);
                yield return new TestCaseData(NumberNegativePattern4, "0.00 -", true);
                yield return new TestCaseData(NumberNegativePattern4, "(0.00)", false);
                yield return new TestCaseData(NumberNegativePattern4, "-0.00", false);
                yield return new TestCaseData(NumberNegativePattern4, "- 0.00", false);

                yield return new TestCaseData(NumberNegativePattern4_Sign_neg, "0.00neg", false);
                yield return new TestCaseData(NumberNegativePattern4_Sign_neg, "0.00 neg", true);
            }
        }

        [Test] // J2N specific
        [TestCaseSource("IsNegativeTestData")]
        public void TestIsNegative(IFormatProvider provider, string valueToTest, bool expected)
        {
            assertEquals(expected, FloatingDecimal.IsNegative(valueToTest, provider));
        }
    }
}
