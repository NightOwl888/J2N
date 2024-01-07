using NUnit.Framework;
using System;

namespace J2N
{
    public class TestMemoryExtensions : TestCase
    {
        private const string hw1 = "HelloWorld";
        private const string TestStringSupplementary = "李红：不，那不是杂志。那是字典。𠳕";

#if FEATURE_SPAN

        /**
         * @tests java.lang.String#indexOf(int)
         */
        [Test]
        public void Test_IndexOf_ReadOnlySpan_Int32()
        {
            // Test for method int java.lang.String.indexOf(int)
            assertEquals("Invalid index returned", 1, hw1.AsSpan().IndexOf((int)'e'));
            assertEquals("Invalid index returned", 1, "a\ud800\udc00".AsSpan().IndexOf(0x10000));

            MemoryExtensions.IndexOf(TestStringSupplementary.AsSpan(), "𠳕".CodePointAt(0));
        }

        /**
         * @tests java.lang.String#indexOf(int, int)
         */
        [Test] // J2N: Verify we can use "startIndex" by slicing the Span
        public void Test_IndexOf_ReadOnlySpan_Int32_Int32()
        {
            // Test for method int java.lang.String.indexOf(int, int)
            assertEquals("Invalid character index returned", 5, hw1.AsSpan(2).IndexOf((int)'W') + 2);
            assertEquals("Invalid index returned", 2, "ab\ud800\udc00".AsSpan(1).IndexOf(0x10000) + 1);
        }

        /**
         * @tests java.lang.String#lastIndexOf(int)
         */
        [Test]
        public void Test_LastIndexOf_ReadOnlySpan_Int32()
        {
            // Test for method int java.lang.String.lastIndexOf(int)
            assertEquals("Failed to return correct index", 5, hw1.AsSpan().LastIndexOf((int)'W'));
            assertEquals("Returned index for non-existent char", -1, hw1.AsSpan()
                    .LastIndexOf((int)'Z'));
            assertEquals("Failed to return correct index", 1, "a\ud800\udc00".AsSpan()
                    .LastIndexOf(0x10000));
        }

        /**
         * @tests java.lang.String#lastIndexOf(int, int)
         */
        [Test] // J2N: Verify we can use "startIndex" by slicing the Span
        public void Test_LastIndexOf_ReadOnlySpan_Int32_Int32()
        {
            // Test for method int java.lang.String.lastIndexOf(int, int)
            assertEquals("Failed to return correct index", 5, hw1.AsSpan(0, 6 + 1).LastIndexOf((int)'W'));
            assertEquals("Returned index for char out of specified range", -1, hw1.AsSpan(0, 4 + 1)
                    .LastIndexOf((int)'W'));
            assertEquals("Returned index for non-existent char", -1, hw1.AsSpan(0, 9 + 1)
                    .LastIndexOf((int)'Z'));
        }


        /**
         * @tests java.lang.String#indexOf(int)
         */
        [Test]
        public void Test_IndexOf_Span_Int32()
        {
            // Test for method int java.lang.String.indexOf(int)
            assertEquals("Invalid index returned", 1, hw1.ToCharArray().AsSpan().IndexOf((int)'e'));
            assertEquals("Invalid index returned", 1, "a\ud800\udc00".ToCharArray().AsSpan().IndexOf(0x10000));

            MemoryExtensions.IndexOf(TestStringSupplementary.ToCharArray().AsSpan(), "𠳕".CodePointAt(0));
        }

        /**
         * @tests java.lang.String#indexOf(int, int)
         */
        [Test] // J2N: Verify we can use "startIndex" by slicing the Span
        public void Test_IndexOf_Span_Int32_Int32()
        {
            // Test for method int java.lang.String.indexOf(int, int)
            assertEquals("Invalid character index returned", 5, hw1.ToCharArray().AsSpan(2).IndexOf((int)'W') + 2);
            assertEquals("Invalid index returned", 2, "ab\ud800\udc00".ToCharArray().AsSpan(1).IndexOf(0x10000) + 1);
        }

        /**
         * @tests java.lang.String#lastIndexOf(int)
         */
        [Test]
        public void Test_LastIndexOf_Span_Int32()
        {
            // Test for method int java.lang.String.lastIndexOf(int)
            assertEquals("Failed to return correct index", 5, hw1.ToCharArray().AsSpan().LastIndexOf((int)'W'));
            assertEquals("Returned index for non-existent char", -1, hw1.ToCharArray().AsSpan()
                    .LastIndexOf((int)'Z'));
            assertEquals("Failed to return correct index", 1, "a\ud800\udc00".ToCharArray().AsSpan()
                    .LastIndexOf(0x10000));
        }

        /**
         * @tests java.lang.String#lastIndexOf(int, int)
         */
        [Test] // J2N: Verify we can use "startIndex" by slicing the Span
        public void Test_LastIndexOf_Span_Int32_Int32()
        {
            // Test for method int java.lang.String.lastIndexOf(int, int)
            assertEquals("Failed to return correct index", 5, hw1.ToCharArray().AsSpan(0, 6 + 1).LastIndexOf((int)'W'));
            assertEquals("Returned index for char out of specified range", -1, hw1.ToCharArray().AsSpan(0, 4 + 1)
                    .LastIndexOf((int)'W'));
            assertEquals("Returned index for non-existent char", -1, hw1.ToCharArray().AsSpan(0, 9 + 1)
                    .LastIndexOf((int)'Z'));
        }


        private void reverseTest(string org, string rev, string back)
        {
            {
                Span<char> sb = stackalloc char[org.Length];
                org.AsSpan().CopyTo(sb);
                sb.ReverseText();
                string reversed = sb.ToString();
                assertEquals(rev, reversed);

                Span<char> sb2 = stackalloc char[reversed.Length];
                reversed.AsSpan().CopyTo(sb);
                sb.ReverseText();
                reversed = sb.ToString();
                assertEquals(back, reversed);
            }
        }

        [Test]
        public void Test_ReverseText_Span()
        {
            {
                string fixture = "0123456789";
                Span<char> sb = stackalloc char[fixture.Length];
                fixture.AsSpan().CopyTo(sb);
                sb.ReverseText();
                assertEquals("9876543210", sb.ToString());
            }

            {
                string fixture = "012345678";
                Span<char> sb = stackalloc char[fixture.Length];
                fixture.AsSpan().CopyTo(sb);
                sb.ReverseText();
                assertEquals("876543210", sb.ToString());
            }

            {
                string fixture = "8";
                Span<char> sb = stackalloc char[fixture.Length];
                fixture.AsSpan().CopyTo(sb);
                sb.ReverseText();
                assertEquals("8", sb.ToString());
            }

            {
                string fixture = "";
                Span<char> sb = stackalloc char[1];
                fixture.AsSpan().CopyTo(sb);
                sb.Slice(0, 0).ReverseText();
                assertEquals("", sb.Slice(0, 0).ToString());
            }


            string str;
            str = "a";
            reverseTest(str, str, str);

            str = "ab";
            reverseTest(str, "ba", str);

            str = "abcdef";
            reverseTest(str, "fedcba", str);

            str = "abcdefg";
            reverseTest(str, "gfedcba", str);

            str = "\ud800\udc00";
            reverseTest(str, str, str);

            str = "\udc00\ud800";
            reverseTest(str, "\ud800\udc00", "\ud800\udc00");

            str = "a\ud800\udc00";
            reverseTest(str, "\ud800\udc00a", str);

            str = "ab\ud800\udc00";
            reverseTest(str, "\ud800\udc00ba", str);

            str = "abc\ud800\udc00";
            reverseTest(str, "\ud800\udc00cba", str);

            str = "\ud800\udc00\udc01\ud801\ud802\udc02";
            reverseTest(str, "\ud802\udc02\ud801\udc01\ud800\udc00",
                    "\ud800\udc00\ud801\udc01\ud802\udc02");

            str = "\ud800\udc00\ud801\udc01\ud802\udc02";
            reverseTest(str, "\ud802\udc02\ud801\udc01\ud800\udc00", str);

            str = "\ud800\udc00\udc01\ud801a";
            reverseTest(str, "a\ud801\udc01\ud800\udc00",
                    "\ud800\udc00\ud801\udc01a");

            str = "a\ud800\udc00\ud801\udc01";
            reverseTest(str, "\ud801\udc01\ud800\udc00a", str);

            str = "\ud800\udc00\udc01\ud801ab";
            reverseTest(str, "ba\ud801\udc01\ud800\udc00",
                    "\ud800\udc00\ud801\udc01ab");

            str = "ab\ud800\udc00\ud801\udc01";
            reverseTest(str, "\ud801\udc01\ud800\udc00ba", str);

            str = "\ud800\udc00\ud801\udc01";
            reverseTest(str, "\ud801\udc01\ud800\udc00", str);

            str = "a\ud800\udc00z\ud801\udc01";
            reverseTest(str, "\ud801\udc01z\ud800\udc00a", str);

            str = "a\ud800\udc00bz\ud801\udc01";
            reverseTest(str, "\ud801\udc01zb\ud800\udc00a", str);

            str = "abc\ud802\udc02\ud801\udc01\ud800\udc00";
            reverseTest(str, "\ud800\udc00\ud801\udc01\ud802\udc02cba", str);

            str = "abcd\ud802\udc02\ud801\udc01\ud800\udc00";
            reverseTest(str, "\ud800\udc00\ud801\udc01\ud802\udc02dcba", str);

            str = new string('z', 1000) + "abcd\ud802\udc02\ud801\udc01\ud800\udc00" + new string('p', 3000) + "abcd\ud802\udc02\ud801\udc01\ud800\udc00";
            reverseTest(str, "\ud800\udc00\ud801\udc01\ud802\udc02dcba" + new string('p', 3000) + "\ud800\udc00\ud801\udc01\ud802\udc02dcba" + new string('z', 1000), str);
        }
#endif
    }
}
