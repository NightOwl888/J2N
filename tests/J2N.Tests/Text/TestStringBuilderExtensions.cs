using NUnit.Framework;
using System;
using System.Text;

namespace J2N.Text
{
    public class TestStringBuilderExtensions : TestCase
    {
        [Test]
        public void TestAppend()
        {
            StringBuilder target = new StringBuilder("This is a test");
            ICharSequence string1 = ". Text to add.".ToCharSequence();
            ICharSequence string2 = " Some more text to add.".ToCharSequence();

            // NOTE: We must use the label, or the compiler will choose the
            // Append(object) overload. That overload works, but is not as efficient
            // as this for CharArrayCharSequence.
            target.Append(charSequence: string1);
            Assert.AreEqual("This is a test. Text to add.", target.ToString());

            target.Append(string2, 6, string2.Length - 6);
            Assert.AreEqual("This is a test. Text to add.more text to add.", target.ToString());
        }

        [Test]
        public virtual void TestAppendCodePointBmp()
        {
            var sb = new StringBuilder("foo bar");
            int codePoint = 97; // a

            sb.AppendCodePoint(codePoint);

            Assert.AreEqual("foo bara", sb.ToString());
        }

        [Test]
        public virtual void TestAppendCodePointUnicode()
        {
            var sb = new StringBuilder("foo bar");
            int codePoint = 3594; // ช

            sb.AppendCodePoint(codePoint);

            Assert.AreEqual("foo barช", sb.ToString());
        }

        [Test]
        public virtual void TestAppendCodePointUTF16Surrogates()
        {
            var sb = new StringBuilder("foo bar");
            int codePoint = 176129; // '\uD86C', '\uDC01' (𫀁)

            sb.AppendCodePoint(codePoint);

            Assert.AreEqual("foo bar𫀁", sb.ToString());
        }

        [Test]
        public virtual void TestAppendCodePointTooHigh()
        {
            var sb = new StringBuilder("foo bar");
            int codePoint = Character.MaxCodePoint + 1;

            Assert.Throws<ArgumentException>(() => sb.AppendCodePoint(codePoint));
        }

        [Test]
        public virtual void TestAppendCodePointTooLow()
        {
            var sb = new StringBuilder("foo bar");
            int codePoint = Character.MinCodePoint - 1;

            Assert.Throws<ArgumentException>(() => sb.AppendCodePoint(codePoint));
        }

        [Test]
        public void TestCompareToOrdinal()
        {
            StringBuilder target = null;
            string compareTo = "Alpine";

            Assert.Greater(0, target.CompareToOrdinal(compareTo.ToCharArray()));
            Assert.Greater(0, target.CompareToOrdinal(new StringBuilder(compareTo)));
            Assert.Greater(0, target.CompareToOrdinal(compareTo));
            Assert.Greater(0, target.CompareToOrdinal(new CharArrayCharSequence(compareTo.ToCharArray())));
            Assert.Greater(0, target.CompareToOrdinal(new StringBuilderCharSequence(new StringBuilder(compareTo))));
            Assert.Greater(0, target.CompareToOrdinal(new StringCharSequence(compareTo)));

            target = new StringBuilder("Alpha");

            Assert.Greater(0, target.CompareToOrdinal(compareTo.ToCharArray()));
            Assert.Greater(0, target.CompareToOrdinal(new StringBuilder(compareTo)));
            Assert.Greater(0, target.CompareToOrdinal(compareTo));
            Assert.Greater(0, target.CompareToOrdinal(new CharArrayCharSequence(compareTo.ToCharArray())));
            Assert.Greater(0, target.CompareToOrdinal(new StringBuilderCharSequence(new StringBuilder(compareTo))));
            Assert.Greater(0, target.CompareToOrdinal(new StringCharSequence(compareTo)));

            compareTo = "Alpha";

            Assert.AreEqual(0, target.CompareToOrdinal(compareTo.ToCharArray()));
            Assert.AreEqual(0, target.CompareToOrdinal(new StringBuilder(compareTo)));
            Assert.AreEqual(0, target.CompareToOrdinal(compareTo));
            Assert.AreEqual(0, target.CompareToOrdinal(new CharArrayCharSequence(compareTo.ToCharArray())));
            Assert.AreEqual(0, target.CompareToOrdinal(new StringBuilderCharSequence(new StringBuilder(compareTo))));
            Assert.AreEqual(0, target.CompareToOrdinal(new StringCharSequence(compareTo)));

            compareTo = "Alp";

            Assert.Less(0, target.CompareToOrdinal(compareTo.ToCharArray()));
            Assert.Less(0, target.CompareToOrdinal(new StringBuilder(compareTo)));
            Assert.Less(0, target.CompareToOrdinal(compareTo));
            Assert.Less(0, target.CompareToOrdinal(new CharArrayCharSequence(compareTo.ToCharArray())));
            Assert.Less(0, target.CompareToOrdinal(new StringBuilderCharSequence(new StringBuilder(compareTo))));
            Assert.Less(0, target.CompareToOrdinal(new StringCharSequence(compareTo)));


            Assert.Less(0, target.CompareToOrdinal((char[])null));
            Assert.Less(0, target.CompareToOrdinal((StringBuilder)null));
            Assert.Less(0, target.CompareToOrdinal((string)null));
            Assert.Less(0, target.CompareToOrdinal(new CharArrayCharSequence(null)));
            Assert.Less(0, target.CompareToOrdinal(new StringBuilderCharSequence(null)));
            Assert.Less(0, target.CompareToOrdinal(new StringCharSequence(null)));
        }

        [Test]
        public virtual void TestReverse()
        {
            var sb = new StringBuilder("foo 𝌆 bar𫀁mañana");

            sb.Reverse();

            Assert.AreEqual("anañam𫀁rab 𝌆 oof", sb.ToString());
        }

        [Test]
        public void TestSubsequence()
        {
            StringBuilder target = new StringBuilder("This is a test");

            Assert.AreEqual("This is a test", target.Subsequence(0, target.Length));
            Assert.AreEqual("is a", target.Subsequence(5, 4));
            Assert.AreEqual("", target.Subsequence(4, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => target.Subsequence(-1, 10));
            Assert.Throws<ArgumentOutOfRangeException>(() => target.Subsequence(3, -2));
            Assert.Throws<ArgumentOutOfRangeException>(() => target.Subsequence(target.Length, 1));

            StringBuilder emptyTarget = new StringBuilder();

            Assert.Throws<ArgumentOutOfRangeException>(() => emptyTarget.Subsequence(0, 1));

            StringBuilder nullTarget = null;

            Assert.IsFalse(nullTarget.Subsequence(6, 10).HasValue); // Null target will always return null subsequence
        }

        [Test]
        public void TestToCharSequence()
        {
            StringBuilder target = new StringBuilder("This is a test");

            var result = target.ToCharSequence();

            Assert.IsNotNull(result);
            Assert.AreEqual(typeof(StringBuilderCharSequence), result.GetType());
        }

        #region Apache Harmony Tests

        private void reverseTest(String org, String rev, String back)
        {
            // create non-shared StringBuilder
            StringBuilder sb = new StringBuilder(org);
            sb.Reverse();
            String reversed = sb.ToString();
            assertEquals(rev, reversed);
            // create non-shared StringBuilder
            sb = new StringBuilder(reversed);
            sb.Reverse();
            reversed = sb.ToString();
            assertEquals(back, reversed);

            // test algorithm when StringBuilder is shared
            sb = new StringBuilder(org);
            String copy = sb.ToString();
            assertEquals(org, copy);
            sb.Reverse();
            reversed = sb.ToString();
            assertEquals(rev, reversed);
            sb = new StringBuilder(reversed);
            copy = sb.ToString();
            assertEquals(rev, copy);
            sb.Reverse();
            reversed = sb.ToString();
            assertEquals(back, reversed);
        }

        /**
         * @tests java.lang.StringBuilder.reverse()
         */
        [Test]
        public void Test_Reverse()
        {
            String fixture = "0123456789";
            StringBuilder sb = new StringBuilder(fixture);
            assertSame(sb, sb.Reverse());
            assertEquals("9876543210", sb.ToString());

            sb = new StringBuilder("012345678");
            assertSame(sb, sb.Reverse());
            assertEquals("876543210", sb.ToString());

            sb.Length = (1);
            assertSame(sb, sb.Reverse());
            assertEquals("8", sb.ToString());

            sb.Length = (0);
            assertSame(sb, sb.Reverse());
            assertEquals("", sb.ToString());

            String str;
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
        }

        /**
         * @tests java.lang.StringBuilder.codePointCount(int, int)
         */
        [Test]
        public void Test_CodePointCountII()
        {
            assertEquals(1, new StringBuilder("\uD800\uDC00").CodePointCount(0, 2));
            assertEquals(1, new StringBuilder("\uD800\uDC01").CodePointCount(0, 2));
            assertEquals(1, new StringBuilder("\uD801\uDC01").CodePointCount(0, 2));
            assertEquals(1, new StringBuilder("\uDBFF\uDFFF").CodePointCount(0, 2));

            assertEquals(3, new StringBuilder("a\uD800\uDC00b").CodePointCount(0, 4));
            assertEquals(4, new StringBuilder("a\uD800\uDC00b\uD800").CodePointCount(0, 5));

            StringBuilder sb = new StringBuilder();
            sb.Append("abc");
            try
            {
                sb.CodePointCount(-1, 2);
                fail("No IOOBE for negative begin index.");
            }
#pragma warning disable 168
            catch (ArgumentOutOfRangeException e)
#pragma warning restore 168
            {

            }

            try
            {
                sb.CodePointCount(0, 4);
                fail("No IOOBE for end index that's too large.");
            }
#pragma warning disable 168
            catch (ArgumentOutOfRangeException e)
#pragma warning restore 168
            {

            }

            try
            {
                sb.CodePointCount(3, 2);
                fail("No IOOBE for begin index larger than end index.");
            }
#pragma warning disable 168
            catch (ArgumentOutOfRangeException e)
#pragma warning restore 168
            {

            }
        }

        /**
         * @tests java.lang.StringBuilder.codePointAt(int)
         */
        [Test]
        public void Test_CodePointAtI()
        {
            StringBuilder sb = new StringBuilder("abc");
            assertEquals('a', sb.CodePointAt(0));
            assertEquals('b', sb.CodePointAt(1));
            assertEquals('c', sb.CodePointAt(2));

            sb = new StringBuilder("\uD800\uDC00");
            assertEquals(0x10000, sb.CodePointAt(0));
            assertEquals('\uDC00', sb.CodePointAt(1));

            sb = new StringBuilder();
            sb.Append("abc");
            try
            {
                sb.CodePointAt(-1);
                fail("No IOOBE on negative index.");
            }
#pragma warning disable 168
            catch (ArgumentOutOfRangeException e)
#pragma warning restore 168
            {

            }

            try
            {
                sb.CodePointAt(sb.Length);
                fail("No IOOBE on index equal to length.");
            }
#pragma warning disable 168
            catch (ArgumentOutOfRangeException e)
#pragma warning restore 168
            {

            }

            try
            {
                sb.CodePointAt(sb.Length + 1);
                fail("No IOOBE on index greater than length.");
            }
#pragma warning disable 168
            catch (ArgumentOutOfRangeException e)
#pragma warning restore 168
            {

            }
        }

        /**
         * @tests java.lang.StringBuilder.indexOf(String)
         */
        [Test]
        public void Test_IndexOfLSystem_String()
        {
            String fixture = "0123456789";
            StringBuilder sb = new StringBuilder(fixture);
            assertEquals(0, sb.IndexOf("0"));
            assertEquals(0, sb.IndexOf("012"));
            assertEquals(-1, sb.IndexOf("02"));
            assertEquals(8, sb.IndexOf("89"));

            try
            {
                sb.IndexOf(null);
                fail("no NPE");
            }
#pragma warning disable 168
            catch (ArgumentNullException e)
#pragma warning restore 168
            {
                // Expected
            }
        }

        /**
         * @tests java.lang.StringBuilder.indexOf(String, int)
         */
        [Test]
        public void Test_IndexOfStringInt()
        {
            String fixture = "0123456789";
            StringBuilder sb = new StringBuilder(fixture);
            assertEquals(0, sb.IndexOf("0"));
            assertEquals(0, sb.IndexOf("012"));
            assertEquals(-1, sb.IndexOf("02"));
            assertEquals(8, sb.IndexOf("89"));

            assertEquals(0, sb.IndexOf("0"), 0);
            assertEquals(0, sb.IndexOf("012"), 0);
            assertEquals(-1, sb.IndexOf("02"), 0);
            assertEquals(8, sb.IndexOf("89"), 0);

            assertEquals(-1, sb.IndexOf("0"), 5);
            assertEquals(-1, sb.IndexOf("012"), 5);
            assertEquals(-1, sb.IndexOf("02"), 0);
            assertEquals(8, sb.IndexOf("89"), 5);

            try
            {
                sb.IndexOf(null, 0);
                fail("no NPE");
            }
#pragma warning disable 168
            catch (ArgumentNullException e)
#pragma warning restore 168
            {
                // Expected
            }
        }

        #endregion Apache Harmony Tests
    }
}
