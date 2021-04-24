using J2N.Globalization;
using NUnit.Framework;
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
            // J2N TODO: The observed behavior for this test in Java 8 is "13.987755". However, we see
            // 7 decimal places in Lucene.NET tests. Not sure which is correct, but leaving alone as it works in Lucene.NET now.
            // But need to revisit to find out why the behavior is different between Java 6 (Lucene) and Java 8 (ICU4J) tests.
            //assertEquals("13.9876543", string.Format(StringFormatter.InvariantCulture, "{0}", 13.9876543210987f));
            assertEquals("22.0", string.Format(StringFormatter.InvariantCulture, "{0}", 22f));
        }

        [Test]
        public void TestDecimalPlaces_Double()
        {
            assertEquals("22.0", string.Format(StringFormatter.InvariantCulture, "{0}", 22d));
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
    }
}
