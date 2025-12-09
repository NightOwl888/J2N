using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace J2N
{
    public class TestSpanUtilities : TestCase
    {
        /**
         * @tests java.lang.StringBuilder.IndexOf(String)
         */
        [Test]
        public void Test_IndexOf_String()
        {
            ReadOnlySpan<char> fixture = "0123456789".AsSpan();

            assertEquals(0, SpanUtilities.IndexOf(fixture, "0")); // J2N NOTE: defaults to Ordinal comparison
            assertEquals(0, SpanUtilities.IndexOf(fixture, "012"));
            assertEquals(-1, SpanUtilities.IndexOf(fixture, "02"));
            assertEquals(8, SpanUtilities.IndexOf(fixture, "89"));

            try
            {
                SpanUtilities.IndexOf(fixture, (string)null);
                fail("no NPE");
            }
            catch (ArgumentNullException) // NullPointerException
            {
                // Expected
            }

            // J2N specific: tests for special cases
            assertEquals(0, SpanUtilities.IndexOf(fixture, "")); // Empty string is found at beginning
            assertEquals(-1, SpanUtilities.IndexOf("".AsSpan(), "foo")); // Empty string search (value.Length > span.Length)
            assertEquals(0, SpanUtilities.IndexOf("".AsSpan(), "")); // Empty string search for empty string
        }

        /**
         * @tests java.lang.StringBuilder.IndexOf(String, int)
         */
        [Test]
        public void Test_IndexOf_String_Int32()
        {
            ReadOnlySpan<char> fixture = "0123456789".AsSpan();

            assertEquals(0, SpanUtilities.IndexOf(fixture, "0")); // J2N NOTE: defaults to Ordinal comparison
            assertEquals(0, SpanUtilities.IndexOf(fixture, "012"));
            assertEquals(-1, SpanUtilities.IndexOf(fixture, "02"));
            assertEquals(8, SpanUtilities.IndexOf(fixture, "89"));

            assertEquals(0, SpanUtilities.IndexOf(fixture, "0", 0));
            assertEquals(0, SpanUtilities.IndexOf(fixture, "012", 0));
            assertEquals(-1, SpanUtilities.IndexOf(fixture, "02", 0));
            assertEquals(8, SpanUtilities.IndexOf(fixture, "89", 0));

            assertEquals(-1, SpanUtilities.IndexOf(fixture, "0", 5));
            assertEquals(-1, SpanUtilities.IndexOf(fixture, "012", 5));
            assertEquals(-1, SpanUtilities.IndexOf(fixture, "02", 0));
            assertEquals(8, SpanUtilities.IndexOf(fixture, "89", 5));

            try
            {
                SpanUtilities.IndexOf(fixture, (string)null, 0);
                fail("no NPE");
            }
            catch (ArgumentNullException) // NullPointerException
            {
                // Expected
            }


            // J2N specific: tests for special cases
            assertEquals(5, SpanUtilities.IndexOf(fixture, "", 5)); // Empty string is found at beginning
            assertEquals(-1, SpanUtilities.IndexOf("".AsSpan(), "foo", 0)); // Empty string search with value (value.Length > span.Length)
            assertEquals(0, SpanUtilities.IndexOf("".AsSpan(), "", 0)); // Empty string search for empty string
        }


        /**
         * @tests java.lang.StringBuilder.LastIndexOf(String)
         */
        [Test]
        public void Test_LastIndexOf_String()
        {
            ReadOnlySpan<char> fixture = "0123456789".AsSpan();

            assertEquals(0, SpanUtilities.LastIndexOf(fixture, "0")); // J2N NOTE: OpenStringBuilder defaults to Ordinal comparison
            assertEquals(0, SpanUtilities.LastIndexOf(fixture, "012"));
            assertEquals(-1, SpanUtilities.LastIndexOf(fixture, "02"));
            assertEquals(8, SpanUtilities.LastIndexOf(fixture, "89"));

            try
            {
                SpanUtilities.LastIndexOf(fixture, (string)null);
                fail("no NPE");
            }
            catch (ArgumentNullException) // NullPointerException
            {
                // Expected
            }
        }

        /**
         * @tests java.lang.StringBuilder.LastIndexOf(String, int)
         */
        [Test]
        public void Test_LastIndexOf_String_Int32()
        {
            ReadOnlySpan<char> fixture = "0123456789".AsSpan();

            assertEquals(0, SpanUtilities.LastIndexOf(fixture, "0")); // J2N NOTE: OpenStringBuilder defaults to Ordinal comparison
            assertEquals(0, SpanUtilities.LastIndexOf(fixture, "012"));
            assertEquals(-1, SpanUtilities.LastIndexOf(fixture, "02"));
            assertEquals(8, SpanUtilities.LastIndexOf(fixture, "89"));

            // J2N: Fixed bugs in Harmony test because it was calling the wrong overload for all of the tests.
            // For this group, we pass fixture.Length to simulate searching the entire span.
            assertEquals(0, SpanUtilities.LastIndexOf(fixture, "0", fixture.Length));
            assertEquals(0, SpanUtilities.LastIndexOf(fixture, "012", fixture.Length));
            assertEquals(-1, SpanUtilities.LastIndexOf(fixture, "02", fixture.Length));
            assertEquals(8, SpanUtilities.LastIndexOf(fixture, "89", fixture.Length));

            //assertEquals(-1, SpanUtilities.LastIndexOf(fixture, "0", 5));
            //assertEquals(-1, SpanUtilities.LastIndexOf(fixture, "012", 5));
            //assertEquals(-1, SpanUtilities.LastIndexOf(fixture, "02", 0));
            //assertEquals(8, SpanUtilities.LastIndexOf(fixture, "89", 5));

            // For this group, we need to change the values to show actual behavior of LastIndexOf with a starting index.
            assertEquals(0, SpanUtilities.LastIndexOf(fixture, "0", 5));     // 0 ≤ 5
            assertEquals(0, SpanUtilities.LastIndexOf(fixture, "012", 5));   // starts at 0
            assertEquals(-1, SpanUtilities.LastIndexOf(fixture, "02", 5));   // nonexistent
            assertEquals(-1, SpanUtilities.LastIndexOf(fixture, "89", 5));   // start at 8 > 5, so skipped

            try
            {
                SpanUtilities.LastIndexOf(fixture, (string)null, 0);
                fail("no NPE");
            }
            catch (ArgumentNullException) // NullPointerException
            {
                // Expected
            }
        }
    }
}
