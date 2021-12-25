using J2N.Globalization;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace J2N.Text
{
    public class TestStringFormatter : TestCase
    {
        // Note: Other StringFormatter tests are currently done by proxy through
        // TestArrays.

        [Test]
        public void TestFormatCollections()
        {
            var set = new HashSet<IDictionary<string, string>>
            {
                new Dictionary<string, string> { { "1", "one" }, { "2", "two" }, { "3", "three" } },
                new Dictionary<string, string> { { "4", "four" }, { "5", "five" }, { "6", "six" } },
                new Dictionary<string, string> { { "7", "seven" }, { "8", "eight" }, { "9", "nine" } },
            };
            var setExpected = "[{1=one, 2=two, 3=three}, {4=four, 5=five, 6=six}, {7=seven, 8=eight, 9=nine}]";

            Assert.AreEqual(setExpected, string.Format(StringFormatter.InvariantCulture, "{0}", set));

            var map = new Dictionary<string, IDictionary<int, double>>
            {
                { "first", new Dictionary<int, double> { { 1, 1.23 }, { 2, 2.23 }, { 3, 3.23 } } },
                { "second", new Dictionary<int, double> { { 4, 1.24 }, { 5, 2.24 }, { 6, 3.24 } } },
                { "third", new Dictionary<int, double> { { 7, 1.25 }, { 8, 2.25 }, { 9, 3.25 } } },
            };
            var mapExpectedPortuguese = "{first={1=1,23, 2=2,23, 3=3,23}, second={4=1,24, 5=2,24, 6=3,24}, third={7=1,25, 8=2,25, 9=3,25}}";
            var mapExpectedUSEnglish = "{first={1=1.23, 2=2.23, 3=3.23}, second={4=1.24, 5=2.24, 6=3.24}, third={7=1.25, 8=2.25, 9=3.25}}";

            Assert.AreEqual(mapExpectedPortuguese, string.Format(new StringFormatter(new CultureInfo("pt")), "{0}", map));
            Assert.AreEqual(mapExpectedUSEnglish, string.Format(new StringFormatter(new CultureInfo("en-US")), "{0}", map));

            var array = new List<Dictionary<string, string>>[]
            {
                new List<Dictionary<string, string>> {
                    new Dictionary<string, string> { { "foo", "bar" }, { "foobar", "barfoo" } }
                },
                new List<Dictionary<string, string>> {
                    new Dictionary<string, string> { { "orange", "yellow" }, { "red", "black" } },
                    new Dictionary<string, string> { { "rain", "snow" }, { "sleet", "sunshine" } }
                },
            };
            var arrayExpected = "[[{foo=bar, foobar=barfoo}], [{orange=yellow, red=black}, {rain=snow, sleet=sunshine}]]";

            // NOTE: The cast to object here is required in order to force string.Format to call the right overload.
            Assert.AreEqual(arrayExpected, string.Format(StringFormatter.InvariantCulture, "{0}", (object)array));
        }

        [Test]
        public void TestCurrentCultureChange()
        {
            var map = new Dictionary<string, IDictionary<int, double>>
            {
                { "first", new Dictionary<int, double> { { 1, 1.23 }, { 2, 2.23 }, { 3, 3.23 } } },
                { "second", new Dictionary<int, double> { { 4, 1.24 }, { 5, 2.24 }, { 6, 3.24 } } },
                { "third", new Dictionary<int, double> { { 7, 1.25 }, { 8, 2.25 }, { 9, 3.25 } } },
            };
            var mapExpectedPortuguese = "{first={1=1,23, 2=2,23, 3=3,23}, second={4=1,24, 5=2,24, 6=3,24}, third={7=1,25, 8=2,25, 9=3,25}}";
            var mapExpectedUSEnglish = "{first={1=1.23, 2=2.23, 3=3.23}, second={4=1.24, 5=2.24, 6=3.24}, third={7=1.25, 8=2.25, 9=3.25}}";

            var formatter = StringFormatter.CurrentCulture;

            using (var context = new CultureContext("en-US"))
            {
                Assert.AreEqual(mapExpectedUSEnglish, string.Format(formatter, "{0}", map));
            }
            using (var context = new CultureContext("pt"))
            {
                Assert.AreEqual(mapExpectedPortuguese, string.Format(formatter, "{0}", map));
            }
        }

        [Test]
        public void TestCurrentUICultureChange()
        {
            var map = new Dictionary<string, IDictionary<int, double>>
            {
                { "first", new Dictionary<int, double> { { 1, 1.23 }, { 2, 2.23 }, { 3, 3.23 } } },
                { "second", new Dictionary<int, double> { { 4, 1.24 }, { 5, 2.24 }, { 6, 3.24 } } },
                { "third", new Dictionary<int, double> { { 7, 1.25 }, { 8, 2.25 }, { 9, 3.25 } } },
            };
            var mapExpectedPortuguese = "{first={1=1,23, 2=2,23, 3=3,23}, second={4=1,24, 5=2,24, 6=3,24}, third={7=1,25, 8=2,25, 9=3,25}}";
            var mapExpectedUSEnglish = "{first={1=1.23, 2=2.23, 3=3.23}, second={4=1.24, 5=2.24, 6=3.24}, third={7=1.25, 8=2.25, 9=3.25}}";

            var formatter = StringFormatter.CurrentUICulture;

            using (var context = new CultureContext("zh", "en-US"))
            {
                Assert.AreEqual(mapExpectedUSEnglish, string.Format(formatter, "{0}", map));
            }
            using (var context = new CultureContext("zh", "pt"))
            {
                Assert.AreEqual(mapExpectedPortuguese, string.Format(formatter, "{0}", map));
            }
        }

        [Test]
        public void TestNegativeZero_Float()
        {
            assertEquals("-0.0", string.Format(StringFormatter.InvariantCulture, "{0}", -0.0f));
            assertEquals("0.0", string.Format(StringFormatter.InvariantCulture, "{0}", 0.0f));
            assertEquals("-0,0", string.Format(new StringFormatter(new CultureInfo("fr-FR")), "{0}", -0.0f));
        }

        [Test]
        public void TestNegativeZero_Double()
        {
            assertEquals("-0.0", string.Format(StringFormatter.InvariantCulture, "{0}", -0.0d));
            assertEquals("0.0", string.Format(StringFormatter.InvariantCulture, "{0}", 0.0d));
            assertEquals("-0,0", string.Format(new StringFormatter(new CultureInfo("fr-FR")), "{0}", -0.0d));
        }

        [Test]
        public void TestDecimalPlaces_Float()
        {
            // J2N TODO: The observed behavior for this test in Java 8 is "13.987655". However, we see
            // 7 decimal places in Lucene.NET tests. Not sure which is correct, but leaving alone as it works in Lucene.NET now.
            // But need to revisit to find out why the behavior is different between Java 6 (Lucene) and Java 8 (ICU4J) tests.
            //assertEquals("13.9876543", string.Format(StringFormatter.InvariantCulture, "{0}", 13.9876543210987f));
            assertEquals("13.987655", string.Format(StringFormatter.InvariantCulture, "{0}", 13.9876543210987f));
            assertEquals("22.0", string.Format(StringFormatter.InvariantCulture, "{0}", 22f));
        }

        [Test]
        public void TestDecimalPlaces_Double()
        {
            assertEquals("22.0", string.Format(StringFormatter.InvariantCulture, "{0}", 22d));

            assertEquals("22,0", string.Format(new StringFormatter(new CultureInfo("fr-FR")), "{0}", 22d));
        }

        [Test]
        public void TestIFormatProvider_Double()
        {
            assertEquals("22.0", string.Format((IFormatProvider)StringFormatter.InvariantCulture, "{0}", 22d));

            assertEquals("22,0", string.Format(new StringFormatter((IFormatProvider)new CultureInfo("fr-FR")), "{0}", 22d));
        }

        [Test]
        public void Test_StandardFormat_Double()
        {
            assertEquals("22.0000000", string.Format(StringFormatter.InvariantCulture, "{0:F7}", 22d));
            assertEquals("22.0", string.Format(StringFormatter.InvariantCulture, "{0:N1}", 22d));
            assertEquals("22.5", string.Format(StringFormatter.InvariantCulture, "{0:N1}", 22.45678d));
        }

        [Test]
        public void TestBoolean()
        {
            assertEquals("true", string.Format(StringFormatter.InvariantCulture, "{0}", true));
            assertEquals("false", string.Format(StringFormatter.InvariantCulture, "{0}", false));
        }


        [Test]
        public void TestArray()
        {
            assertEquals("[1, 2, 3, 4, 5, 6, 7]", string.Format(StringFormatter.InvariantCulture, "{0}", new int[] { 1, 2, 3, 4, 5, 6, 7 }));
            assertEquals("[1, 2, 3, 4, 5, 6, 7]", string.Format(StringFormatter.InvariantCulture, "{0}", (System.Array)new string[] { "1", "2", "3", "4", "5", "6", "7" }));
        }

        private enum State
        {
            SETREADER, // consumer set a reader input either via ctor or via reset(Reader)
            RESET, // consumer has called reset()
            INCREMENT, // consumer is consuming, has called IncrementToken() == true
            INCREMENT_FALSE, // consumer has called IncrementToken() which returned false
            END, // consumer has called end() to perform end of stream operations
            CLOSE // consumer has called close() to release any resources
        }

        [Test]
        public void TestEnum()
        {
            var state = State.RESET;

            var actual = string.Format(StringFormatter.InvariantCulture, "IncrementToken() called while in wrong state: {0}", state);

            assertEquals("IncrementToken() called while in wrong state: RESET", actual);
        }

        /// <summary>
        /// Ensures J format will work on <see cref="sbyte"/>, <see cref="byte"/>, <see cref="short"/>, <see cref="int"/>, and <see cref="long"/>
        /// when using <see cref="StringFormatter"/>.
        /// </summary>
        [Test]
        public void Test_J_Format_SupportedIntegralTypes()
        {
            string[] formats = new string[] { "j", "J", "j4", "J4", "", null };
            object[] numbers = new object[] { (sbyte)123, (byte)123, (short)1234, (int)12345, (long)123456 };
            string[] expecteds = new string[] { "123", "123", "1234", "12345", "123456" };

            for (int i = 0; i < expecteds.Length; i++)
            {
                for (int j = 0; j < formats.Length; j++)
                {
                    if (formats[j] != null && formats[j].StartsWith("J", StringComparison.OrdinalIgnoreCase))
                    {
                        // Without StringFormatter, Java format should not be supported
                        Assert.Throws<FormatException>(() => string.Format("{0:" + formats[j] + "}", numbers[i]));
                    }

                    assertEquals(expecteds[i], string.Format(J2N.Text.StringFormatter.InvariantCulture, "{0:" + formats[j] + "}", numbers[i]));

                    // Culture should have no effect on integral types
                    assertEquals(expecteds[i], string.Format(J2N.Text.StringFormatter.CurrentCulture, "{0:" + formats[j] + "}", numbers[i]));
                }

#if NET46_OR_GREATER || NETCOREAPP
                // String interpolation
                FormattableString message = $"{numbers[i]:j}"; // Dynamic formats are unsupported, so we hardcode "j"
                assertEquals(expecteds[i], message.ToString(J2N.Text.StringFormatter.InvariantCulture));
#endif
            }
        }

        [Test]
        public void Test_Mixed_SupportedAndUnsupportedTypes()
        {
            int i = 12345;
            uint ui = 98765;
            DateTime date = new DateTime(1970, 1, 1);
            J2N.Numerics.Single f = 98.76543f;
            float f2 = f;

            string actual = string.Format(J2N.Text.StringFormatter.InvariantCulture, "The quick {0:J} brown fox {1:N} jumped over {2:f} the lazy {3:X} dog. {4:x}", i, ui, date, f, f2);
            string expected = "The quick 12345 brown fox 98,765.00 jumped over Thursday, 01 January 1970 00:00 the lazy 0X1.8B0FCCP6 dog. 0x1.8b0fccp6";
            assertEquals(expected, actual);
        }

    }
}
