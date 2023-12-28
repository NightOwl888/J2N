using NUnit.Framework;
using System;

namespace J2N.Memory
{
    public class TestMemoryExtensions : TestCase
    {
#if FEATURE_SPAN
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
