using NUnit.Framework;
using System;
using System.Text;

namespace J2N.Text
{
    /// <summary>
    /// Tests from Apache Harmony, StringBufferTest.java
    /// </summary>
    public class TestStringBuffer : TestCase
    {
        /**
         * @tests java.lang.StringBuffer#setLength(int)
         */
        [Test]
        public void Test_setLengthI()
        {
            // Regression for HARMONY-90
            StringBuffer buffer = new StringBuffer("abcde");
            try
            {
                buffer.Length = (-1);
                fail("Assert 0: IndexOutOfBoundsException must be thrown");
            }
            catch (ArgumentOutOfRangeException e)
            {
                // expected
            }

            assertEquals("abcde", buffer.ToString());
            buffer.Length = (1);
            buffer.Append('f');
            assertEquals("af", buffer.ToString());

            buffer = new StringBuffer("abcde");
            assertEquals("cde", buffer.ToString(2, buffer.Length - 2));
            buffer.Length = (3);
            buffer.Append('f');
            assertEquals("abcf", buffer.ToString());

            buffer = new StringBuffer("abcde");
            buffer.Length = (2);
            try
            {
                var _ = buffer[3];
                fail("should throw IndexOutOfBoundsException");
            }
            catch (IndexOutOfRangeException e)
            {
                // Expected
            }

            buffer = new StringBuffer();
            buffer.Append("abcdefg");
            buffer.Length = (2);
            buffer.Length = (5);
            for (int i = 2; i < 5; i++)
            {
                assertEquals(0, buffer[i]);
            }

            buffer = new StringBuffer();
            buffer.Append("abcdefg");
            buffer.Delete(2, 4 - 2); // J2N: Corrected length parameter
            buffer.Length = (7);
            assertEquals('a', buffer[0]);
            assertEquals('b', buffer[1]);
            assertEquals('e', buffer[2]);
            assertEquals('f', buffer[3]);
            assertEquals('g', buffer[4]);
            for (int i = 5; i < 7; i++)
            {
                assertEquals(0, buffer[i]);
            }

            buffer = new StringBuffer();
            buffer.Append("abcdefg");
            buffer.Replace(2, 5 - 2, "z"); // J2N: Corrected count parameter
            buffer.Length = (7);
            for (int i = 5; i < 7; i++)
            {
                assertEquals(0, buffer[i]);
            }
        }

        /**
         * @tests java.lang.StringBuffer#toString()
         */
        [Test]
        public void Test_toString()
        {
            StringBuffer buffer = new StringBuffer();
            assertEquals("", buffer.ToString());

            buffer.Append("abcde");
            assertEquals("abcde", buffer.ToString());
            buffer.Length = (1000);
            byte[] bytes = buffer.ToString().getBytes("GB18030");
            for (int i = 5; i < bytes.Length; i++)
            {
                assertEquals(0, bytes[i]);
            }

            buffer.Length = (5);
            buffer.Append("fghij");
            assertEquals("abcdefghij", buffer.ToString());
        }

        /**
         * @tests StringBuffer.StringBuffer(CharSequence);
         */
        [Test]
        public void Test_Constructor_ICharSequence()
        {
            //try
            //{
            //    new StringBuffer((ICharSequence)null);
            //    fail("Assert 0: NPE must be thrown.");
            //}
            //catch (ArgumentNullException e) { }

            assertEquals(string.Empty, new StringBuffer((ICharSequence)null).ToString()); // J2N: To match .NET, a null in the constructor should return string.Empty

            assertEquals("Assert 1: must equal 'abc'.", "abc", new StringBuffer((ICharSequence)"abc".AsCharSequence()).ToString());
        }

        /**
         * @tests StringBuffer.StringBuffer(CharSequence);
         */
        [Test]
        public void Test_Constructor_CharArray()
        {
            //try
            //{
            //    new StringBuffer((ICharSequence)null);
            //    fail("Assert 0: NPE must be thrown.");
            //}
            //catch (ArgumentNullException e) { }

            assertEquals(string.Empty, new StringBuffer((char[])null).ToString()); // J2N: To match .NET, a null in the constructor should return string.Empty

            assertEquals("Assert 1: must equal 'abc'.", "abc", new StringBuffer((char[])"abc".ToCharArray()).ToString());
        }

        /**
         * @tests StringBuffer.StringBuffer(CharSequence);
         */
        [Test]
        public void Test_Constructor_StringBuilder()
        {
            //try
            //{
            //    new StringBuffer((ICharSequence)null);
            //    fail("Assert 0: NPE must be thrown.");
            //}
            //catch (ArgumentNullException e) { }

            assertEquals(string.Empty, new StringBuffer((StringBuilder)null).ToString()); // J2N: To match .NET, a null in the constructor should return string.Empty

            assertEquals("Assert 1: must equal 'abc'.", "abc", new StringBuffer((StringBuilder)new StringBuilder("abc")).ToString());
        }

        /**
         * @tests StringBuffer.StringBuffer(CharSequence);
         */
        [Test]
        public void Test_Constructor_String()
        {
            //try
            //{
            //    new StringBuffer((ICharSequence)null);
            //    fail("Assert 0: NPE must be thrown.");
            //}
            //catch (ArgumentNullException e) { }

            assertEquals(string.Empty, new StringBuffer((string)null).ToString()); // J2N: To match .NET, a null in the constructor should return string.Empty

            assertEquals("Assert 1: must equal 'abc'.", "abc", new StringBuffer((string)"abc").ToString());
        }

        //[Test]
        //public void Test_trimToSize()
        //{
        //    StringBuffer buffer = new StringBuffer(25);
        //    buffer.Append("abc");
        //    int origCapacity = buffer.Capacity;
        //    buffer.trimToSize();
        //    int trimCapacity = buffer.Capacity;
        //    assertTrue("Assert 0: capacity must be smaller.", trimCapacity < origCapacity);
        //    assertEquals("Assert 1: length must still be 3", 3, buffer.Length);
        //    assertEquals("Assert 2: value must still be 'abc'.", "abc", buffer.ToString());
        //}

        /**
         * @tests java.lang.StringBuffer.Append(CharSequence)
         */
        [Test]
        public void Test_Append_ICharSequence()
        {
            StringBuffer sb = new StringBuffer();
            assertSame(sb, sb.Append((ICharSequence)"ab".AsCharSequence()));
            assertEquals("ab", sb.ToString());
            sb.Length = (0);
            assertSame(sb, sb.Append((ICharSequence)"cd".AsCharSequence()));
            assertEquals("cd", sb.ToString());
            sb.Length = (0);
            assertSame(sb, sb.Append((ICharSequence)null));
            //assertEquals("null", sb.ToString());
            assertEquals(string.Empty, sb.ToString()); // J2N: To match .NET, appending null is a no-op
        }

        /**
         * @tests java.lang.StringBuffer.Append(CharSequence)
         */
        [Test]
        public void Test_Append_CharArray()
        {
            StringBuffer sb = new StringBuffer();
            assertSame(sb, sb.Append((char[])"ab".ToCharArray()));
            assertEquals("ab", sb.ToString());
            sb.Length = (0);
            assertSame(sb, sb.Append((char[])"cd".ToCharArray()));
            assertEquals("cd", sb.ToString());
            sb.Length = (0);
            assertSame(sb, sb.Append((char[])null));
            //assertEquals("null", sb.ToString());
            assertEquals(string.Empty, sb.ToString()); // J2N: To match .NET, appending null is a no-op
        }

        /**
         * @tests java.lang.StringBuffer.Append(CharSequence)
         */
        [Test]
        public void Test_Append_StringBuilder()
        {
            StringBuffer sb = new StringBuffer();
            assertSame(sb, sb.Append((StringBuilder)new StringBuilder("ab")));
            assertEquals("ab", sb.ToString());
            sb.Length = (0);
            assertSame(sb, sb.Append((StringBuilder)new StringBuilder("cd")));
            assertEquals("cd", sb.ToString());
            sb.Length = (0);
            assertSame(sb, sb.Append((StringBuilder)null));
            //assertEquals("null", sb.ToString());
            assertEquals(string.Empty, sb.ToString()); // J2N: To match .NET, appending null is a no-op
        }

        /**
         * @tests java.lang.StringBuffer.Append(CharSequence)
         */
        [Test]
        public void Test_Append_String()
        {
            StringBuffer sb = new StringBuffer();
            assertSame(sb, sb.Append((string)"ab"));
            assertEquals("ab", sb.ToString());
            sb.Length = (0);
            assertSame(sb, sb.Append((string)"cd"));
            assertEquals("cd", sb.ToString());
            sb.Length = (0);
            assertSame(sb, sb.Append((string)null));
            //assertEquals("null", sb.ToString());
            assertEquals(string.Empty, sb.ToString()); // J2N: To match .NET, appending null is a no-op
        }

        /**
         * @tests java.lang.StringBuffer.Append(CharSequence)
         */
        [Test]
        public void Test_Append_ReadOnlySpan()
        {
            StringBuffer sb = new StringBuffer();
            assertSame(sb, sb.Append("ab".AsSpan()));
            assertEquals("ab", sb.ToString());
            sb.Length = (0);
            assertSame(sb, sb.Append("cd".AsSpan()));
            assertEquals("cd", sb.ToString());
            sb.Length = (0);
            assertSame(sb, sb.Append((ReadOnlySpan<char>)null));
            //assertEquals("null", sb.ToString());
            assertEquals(string.Empty, sb.ToString()); // J2N: To match .NET, appending null is a no-op
        }

        /**
         * @tests java.lang.StringBuffer.Append(CharSequence, int, int)
         */
        [Test]
        public void Test_Append_ICharSequence_Int32_Int32()
        {
            StringBuffer sb = new StringBuffer();
            assertSame(sb, sb.Append((ICharSequence)"ab".AsCharSequence(), 0, 2 - 0)); // J2N: Corrected 3rd parameter
            assertEquals("ab", sb.ToString());
            sb.Length = (0);
            assertSame(sb, sb.Append((ICharSequence)"cd".AsCharSequence(), 0, 2 - 0)); // J2N: Corrected 3rd parameter
            assertEquals("cd", sb.ToString());
            sb.Length = (0);
            assertSame(sb, sb.Append((ICharSequence)"abcd".AsCharSequence(), 0, 2 - 0)); // J2N: Corrected 3rd parameter
            assertEquals("ab", sb.ToString());
            sb.Length = (0);
            assertSame(sb, sb.Append((ICharSequence)"abcd".AsCharSequence(), 2, 4 - 2)); // J2N: Corrected 3rd parameter
            assertEquals("cd", sb.ToString());
            sb.Length = (0);
            //assertSame(sb, sb.Append((ICharSequence)null, 0, 2 - 0)); // J2N: Corrected 3rd parameter
            //assertEquals("nu", sb.ToString());

            assertSame(sb, sb.Append((ICharSequence)null, 0, 0)); // J2N: To match .NET behavior, this is only a no-op if and only if both int values are 0
            assertEquals(string.Empty, sb.ToString());
        }

        /**
         * @tests java.lang.StringBuffer.Append(CharSequence, int, int)
         */
        [Test]
        public void Test_Append_CharArray_Int32_Int32()
        {
            StringBuffer sb = new StringBuffer();
            assertSame(sb, sb.Append((char[])"ab".ToCharArray(), 0, 2 - 0)); // J2N: Corrected 3rd parameter
            assertEquals("ab", sb.ToString());
            sb.Length = (0);
            assertSame(sb, sb.Append((char[])"cd".ToCharArray(), 0, 2 - 0)); // J2N: Corrected 3rd parameter
            assertEquals("cd", sb.ToString());
            sb.Length = (0);
            assertSame(sb, sb.Append((char[])"abcd".ToCharArray(), 0, 2 - 0)); // J2N: Corrected 3rd parameter
            assertEquals("ab", sb.ToString());
            sb.Length = (0);
            assertSame(sb, sb.Append((char[])"abcd".ToCharArray(), 2, 4 - 2)); // J2N: Corrected 3rd parameter
            assertEquals("cd", sb.ToString());
            sb.Length = (0);
            //assertSame(sb, sb.Append((char[])null, 0, 2 - 0)); // J2N: Corrected 3rd parameter
            //assertEquals("nu", sb.ToString());

            assertSame(sb, sb.Append((char[])null, 0, 0)); // J2N: To match .NET behavior, this is only a no-op if and only if both int values are 0
            assertEquals(string.Empty, sb.ToString());
        }

        /**
         * @tests java.lang.StringBuffer.Append(CharSequence, int, int)
         */
        [Test]
        public void Test_Append_StringBuilder_Int32_Int32()
        {
            StringBuffer sb = new StringBuffer();
            assertSame(sb, sb.Append((StringBuilder)new StringBuilder("ab"), 0, 2 - 0)); // J2N: Corrected 3rd parameter
            assertEquals("ab", sb.ToString());
            sb.Length = (0);
            assertSame(sb, sb.Append((StringBuilder)new StringBuilder("cd"), 0, 2 - 0)); // J2N: Corrected 3rd parameter
            assertEquals("cd", sb.ToString());
            sb.Length = (0);
            assertSame(sb, sb.Append((StringBuilder)new StringBuilder("abcd"), 0, 2 - 0)); // J2N: Corrected 3rd parameter
            assertEquals("ab", sb.ToString());
            sb.Length = (0);
            assertSame(sb, sb.Append((StringBuilder)new StringBuilder("abcd"), 2, 4 - 2)); // J2N: Corrected 3rd parameter
            assertEquals("cd", sb.ToString());
            sb.Length = (0);
            //assertSame(sb, sb.Append((StringBuilder)null, 0, 2 - 0)); // J2N: Corrected 3rd parameter
            //assertEquals("nu", sb.ToString());

            assertSame(sb, sb.Append((StringBuilder)null, 0, 0)); // J2N: To match .NET behavior, this is only a no-op if and only if both int values are 0
            assertEquals(string.Empty, sb.ToString());
        }

        /**
         * @tests java.lang.StringBuffer.Append(CharSequence, int, int)
         */
        [Test]
        public void Test_Append_String_Int32_Int32()
        {
            StringBuffer sb = new StringBuffer();
            assertSame(sb, sb.Append((string)"ab", 0, 2 - 0)); // J2N: Corrected 3rd parameter
            assertEquals("ab", sb.ToString());
            sb.Length = (0);
            assertSame(sb, sb.Append((string)"cd", 0, 2 - 0)); // J2N: Corrected 3rd parameter
            assertEquals("cd", sb.ToString());
            sb.Length = (0);
            assertSame(sb, sb.Append((string)"abcd", 0, 2 - 0)); // J2N: Corrected 3rd parameter
            assertEquals("ab", sb.ToString());
            sb.Length = (0);
            assertSame(sb, sb.Append((string)"abcd", 2, 4 - 2)); // J2N: Corrected 3rd parameter
            assertEquals("cd", sb.ToString());
            sb.Length = (0);
            //assertSame(sb, sb.Append((string)null, 0, 2 - 0)); // J2N: Corrected 3rd parameter
            //assertEquals("nu", sb.ToString());

            assertSame(sb, sb.Append((string)null, 0, 0)); // J2N: To match .NET behavior, this is only a no-op if and only if both int values are 0
            assertEquals(string.Empty, sb.ToString());
        }


        /**
         * @tests java.lang.StringBuffer.Append(char[], int, int)
         */
        [Test]
        public void Test_append_CII_2()
        {
            StringBuffer obj = new StringBuffer();
            try
            {
                obj.Append(new char[0], -1, -1);
                fail("ArrayIndexOutOfBoundsException expected");
            }
            catch (ArgumentOutOfRangeException e)
            {
                // expected
            }
        }

        /**
         * @tests java.lang.StringBuffer.Append(char[], int, int)
         */
        [Test]
        public void Test_append_CII_3()
        {
            StringBuffer obj = new StringBuffer();
            try
            {
                obj.Append((char[])null, -1, -1); // J2N: To match .NET, we are throwing ArgumentOutOfRangeException when either int is negative before null checking is done
                fail("ArgumentOutOfRangeException expected");
            }
            catch (ArgumentOutOfRangeException e)
            {
                // expected
            }

            try
            {
                obj.Append((char[])null, 0, 1); // J2N: To match .NET, we are throwing ArgumentNullException when either int has a postive value
                fail("ArgumentNullException expected");
            }
            catch (ArgumentNullException e)
            {
                // expected
            }

            try
            {
                obj.Append((char[])null, 1, 0); // J2N: To match .NET, we are throwing ArgumentNullException when either int has a postive value
                fail("ArgumentNullException expected");
            }
            catch (ArgumentNullException e)
            {
                // expected
            }

            int beforeLen = obj.Length;
            obj.Append((char[])null, 0, 0);
            assertEquals(beforeLen, obj.Length); // This should be a no-op
        }

        /**
         * @tests java.lang.StringBuffer.insert(int, CharSequence)
         */
        [Test]
        public void Test_Insert_Int32_ICharSequence()
        {
            string fixture = "0000";
            StringBuffer sb = new StringBuffer(fixture);
            assertSame(sb, sb.Insert(0, (ICharSequence)"ab".AsCharSequence()));
            assertEquals("ab0000", sb.ToString());
            assertEquals(6, sb.Length);

            sb = new StringBuffer(fixture);
            assertSame(sb, sb.Insert(2, (ICharSequence)"ab".AsCharSequence()));
            assertEquals("00ab00", sb.ToString());
            assertEquals(6, sb.Length);

            sb = new StringBuffer(fixture);
            assertSame(sb, sb.Insert(4, (ICharSequence)"ab".AsCharSequence()));
            assertEquals("0000ab", sb.ToString());
            assertEquals(6, sb.Length);

            sb = new StringBuffer(fixture);
            assertSame(sb, sb.Insert(4, (ICharSequence)null));
            //assertEquals("0000null", sb.ToString());
            //assertEquals(8, sb.Length);
            assertEquals("0000", sb.ToString()); // J2N: Inserting null is a no-op (different behavior than Java is intended)
            assertEquals(4, sb.Length);

            try
            {
                sb = new StringBuffer(fixture);
                sb.Insert(-1, (ICharSequence)"ab".AsCharSequence());
                fail("no IOOBE, negative index");
            }
            catch (ArgumentOutOfRangeException e)
            {
                // Expected
            }

            try
            {
                sb = new StringBuffer(fixture);
                sb.Insert(5, (ICharSequence)"ab".AsCharSequence());
                fail("no IOOBE, index too large index");
            }
            catch (ArgumentOutOfRangeException e)
            {
                // Expected
            }
        }

        /**
         * @tests java.lang.StringBuffer.insert(int, CharSequence)
         */
        [Test]
        public void Test_Insert_Int32_CharArray()
        {
            string fixture = "0000";
            StringBuffer sb = new StringBuffer(fixture);
            assertSame(sb, sb.Insert(0, (char[])"ab".ToCharArray()));
            assertEquals("ab0000", sb.ToString());
            assertEquals(6, sb.Length);

            sb = new StringBuffer(fixture);
            assertSame(sb, sb.Insert(2, (char[])"ab".ToCharArray()));
            assertEquals("00ab00", sb.ToString());
            assertEquals(6, sb.Length);

            sb = new StringBuffer(fixture);
            assertSame(sb, sb.Insert(4, (char[])"ab".ToCharArray()));
            assertEquals("0000ab", sb.ToString());
            assertEquals(6, sb.Length);

            sb = new StringBuffer(fixture);
            assertSame(sb, sb.Insert(4, (char[])null));
            //assertEquals("0000null", sb.ToString());
            //assertEquals(8, sb.Length);
            assertEquals("0000", sb.ToString()); // J2N: Inserting null is a no-op (different behavior than Java is intended)
            assertEquals(4, sb.Length);

            try
            {
                sb = new StringBuffer(fixture);
                sb.Insert(-1, (char[])"ab".ToCharArray());
                fail("no IOOBE, negative index");
            }
            catch (ArgumentOutOfRangeException e)
            {
                // Expected
            }

            try
            {
                sb = new StringBuffer(fixture);
                sb.Insert(5, (char[])"ab".ToCharArray());
                fail("no IOOBE, index too large index");
            }
            catch (ArgumentOutOfRangeException e)
            {
                // Expected
            }
        }

        /**
         * @tests java.lang.StringBuffer.insert(int, CharSequence)
         */
        [Test]
        public void Test_Insert_Int32_StringBuilder()
        {
            string fixture = "0000";
            StringBuffer sb = new StringBuffer(fixture);
            assertSame(sb, sb.Insert(0, (StringBuilder)new StringBuilder("ab")));
            assertEquals("ab0000", sb.ToString());
            assertEquals(6, sb.Length);

            sb = new StringBuffer(fixture);
            assertSame(sb, sb.Insert(2, (StringBuilder)new StringBuilder("ab")));
            assertEquals("00ab00", sb.ToString());
            assertEquals(6, sb.Length);

            sb = new StringBuffer(fixture);
            assertSame(sb, sb.Insert(4, (StringBuilder)new StringBuilder("ab")));
            assertEquals("0000ab", sb.ToString());
            assertEquals(6, sb.Length);

            sb = new StringBuffer(fixture);
            assertSame(sb, sb.Insert(4, (StringBuilder)null));
            //assertEquals("0000null", sb.ToString());
            //assertEquals(8, sb.Length);
            assertEquals("0000", sb.ToString()); // J2N: Inserting null is a no-op (different behavior than Java is intended)
            assertEquals(4, sb.Length);

            try
            {
                sb = new StringBuffer(fixture);
                sb.Insert(-1, (StringBuilder)new StringBuilder("ab"));
                fail("no IOOBE, negative index");
            }
            catch (ArgumentOutOfRangeException e)
            {
                // Expected
            }

            try
            {
                sb = new StringBuffer(fixture);
                sb.Insert(5, (StringBuilder)new StringBuilder("ab"));
                fail("no IOOBE, index too large index");
            }
            catch (ArgumentOutOfRangeException e)
            {
                // Expected
            }
        }

        /**
         * @tests java.lang.StringBuffer.insert(int, CharSequence)
         */
        [Test]
        public void Test_Insert_Int32_String()
        {
            string fixture = "0000";
            StringBuffer sb = new StringBuffer(fixture);
            assertSame(sb, sb.Insert(0, (string)"ab"));
            assertEquals("ab0000", sb.ToString());
            assertEquals(6, sb.Length);

            sb = new StringBuffer(fixture);
            assertSame(sb, sb.Insert(2, (string)"ab"));
            assertEquals("00ab00", sb.ToString());
            assertEquals(6, sb.Length);

            sb = new StringBuffer(fixture);
            assertSame(sb, sb.Insert(4, (string)"ab"));
            assertEquals("0000ab", sb.ToString());
            assertEquals(6, sb.Length);

            sb = new StringBuffer(fixture);
            assertSame(sb, sb.Insert(4, (string)null));
            //assertEquals("0000null", sb.ToString());
            //assertEquals(8, sb.Length);
            assertEquals("0000", sb.ToString()); // J2N: Inserting null is a no-op (different behavior than Java is intended)
            assertEquals(4, sb.Length);

            try
            {
                sb = new StringBuffer(fixture);
                sb.Insert(-1, (string)"ab");
                fail("no IOOBE, negative index");
            }
            catch (ArgumentOutOfRangeException e)
            {
                // Expected
            }

            try
            {
                sb = new StringBuffer(fixture);
                sb.Insert(5, (string)"ab");
                fail("no IOOBE, index too large index");
            }
            catch (ArgumentOutOfRangeException e)
            {
                // Expected
            }
        }


        /**
         * @tests java.lang.StringBuffer.insert(int, CharSequence, int, int)
         */
        [Test]
        public void Test_Insert_Int32_ICharSequence_Int32_Int32()
        {
            string fixture = "0000";
            StringBuffer sb = new StringBuffer(fixture);
            assertSame(sb, sb.Insert(0, (ICharSequence)"ab".AsCharSequence(), 0, 2 - 0)); // J2N: Corrected 4th parameter
            assertEquals("ab0000", sb.ToString());
            assertEquals(6, sb.Length);

            sb = new StringBuffer(fixture);
            assertSame(sb, sb.Insert(0, (ICharSequence)"ab".AsCharSequence(), 0, 1 - 0)); // J2N: Corrected 4th parameter
            assertEquals("a0000", sb.ToString());
            assertEquals(5, sb.Length);

            sb = new StringBuffer(fixture);
            assertSame(sb, sb.Insert(2, (ICharSequence)"ab".AsCharSequence(), 0, 2 - 0)); // J2N: Corrected 4th parameter
            assertEquals("00ab00", sb.ToString());
            assertEquals(6, sb.Length);

            sb = new StringBuffer(fixture);
            assertSame(sb, sb.Insert(2, (ICharSequence)"ab".AsCharSequence(), 0, 1 - 0)); // J2N: Corrected 4th parameter
            assertEquals("00a00", sb.ToString());
            assertEquals(5, sb.Length);

            sb = new StringBuffer(fixture);
            assertSame(sb, sb.Insert(4, (ICharSequence)"ab".AsCharSequence(), 0, 2 - 0)); // J2N: Corrected 4th parameter
            assertEquals("0000ab", sb.ToString());
            assertEquals(6, sb.Length);

            sb = new StringBuffer(fixture);
            assertSame(sb, sb.Insert(4, (ICharSequence)"ab".AsCharSequence(), 0, 1 - 0)); // J2N: Corrected 4th parameter
            assertEquals("0000a", sb.ToString());
            assertEquals(5, sb.Length);

            sb = new StringBuffer(fixture);
            //assertSame(sb, sb.Insert(4, (ICharSequence)null, 0, 2));
            //assertEquals("0000nu", sb.ToString());
            //assertEquals(6, sb.Length);

            assertSame(sb, sb.Insert(4, (ICharSequence)null, 0, 0)); // J2N: To match .NET, this is only a no-op if both int parameters are 0
            assertEquals("0000", sb.ToString());
            assertEquals(4, sb.Length);


            try
            {
                sb = new StringBuffer(fixture);
                sb.Insert(-1, (ICharSequence)"ab".AsCharSequence(), 0, 2 - 0); // J2N: Corrected 4th parameter
                fail("no IOOBE, negative index");
            }
            catch (ArgumentOutOfRangeException e)
            {
                // Expected
            }

            try
            {
                sb = new StringBuffer(fixture);
                sb.Insert(5, (ICharSequence)"ab".AsCharSequence(), 0, 2 - 0); // J2N: Corrected 4th parameter
                fail("no IOOBE, index too large index");
            }
            catch (ArgumentOutOfRangeException e)
            {
                // Expected
            }

            try
            {
                sb = new StringBuffer(fixture);
                sb.Insert(5, (ICharSequence)"ab".AsCharSequence(), -1, 2 - -1); // J2N: Corrected 4th parameter
                fail("no IOOBE, negative offset");
            }
            catch (ArgumentOutOfRangeException e)
            {
                // Expected
            }

            try
            {
                sb = new StringBuffer(fixture);
                sb.Insert(5, new char[] { 'a', 'b' }.AsCharSequence(), 0, -1 - 0); // J2N: Corrected 4th parameter
                fail("no IOOBE, negative length");
            }
            catch (ArgumentOutOfRangeException e)
            {
                // Expected
            }

            try
            {
                sb = new StringBuffer(fixture);
                sb.Insert(5, new char[] { 'a', 'b' }.AsCharSequence(), 0, 3 - 0); // J2N: Corrected 4th parameter
                fail("no IOOBE, too long");
            }
            catch (ArgumentOutOfRangeException e)
            {
                // Expected
            }
        }

        /**
         * @tests java.lang.StringBuffer.insert(int, CharSequence, int, int)
         */
        [Test]
        public void Test_Insert_Int32_CharArray_Int32_Int32()
        {
            string fixture = "0000";
            StringBuffer sb = new StringBuffer(fixture);
            assertSame(sb, sb.Insert(0, (char[])"ab".ToCharArray(), 0, 2 - 0)); // J2N: Corrected 4th parameter
            assertEquals("ab0000", sb.ToString());
            assertEquals(6, sb.Length);

            sb = new StringBuffer(fixture);
            assertSame(sb, sb.Insert(0, (char[])"ab".ToCharArray(), 0, 1 - 0)); // J2N: Corrected 4th parameter
            assertEquals("a0000", sb.ToString());
            assertEquals(5, sb.Length);

            sb = new StringBuffer(fixture);
            assertSame(sb, sb.Insert(2, (char[])"ab".ToCharArray(), 0, 2 - 0)); // J2N: Corrected 4th parameter
            assertEquals("00ab00", sb.ToString());
            assertEquals(6, sb.Length);

            sb = new StringBuffer(fixture);
            assertSame(sb, sb.Insert(2, (char[])"ab".ToCharArray(), 0, 1 - 0)); // J2N: Corrected 4th parameter
            assertEquals("00a00", sb.ToString());
            assertEquals(5, sb.Length);

            sb = new StringBuffer(fixture);
            assertSame(sb, sb.Insert(4, (char[])"ab".ToCharArray(), 0, 2 - 0)); // J2N: Corrected 4th parameter
            assertEquals("0000ab", sb.ToString());
            assertEquals(6, sb.Length);

            sb = new StringBuffer(fixture);
            assertSame(sb, sb.Insert(4, (char[])"ab".ToCharArray(), 0, 1 - 0)); // J2N: Corrected 4th parameter
            assertEquals("0000a", sb.ToString());
            assertEquals(5, sb.Length);

            sb = new StringBuffer(fixture);
            //assertSame(sb, sb.Insert(4, (char[])null, 0, 2));
            //assertEquals("0000nu", sb.ToString());
            //assertEquals(6, sb.Length);

            assertSame(sb, sb.Insert(4, (char[])null, 0, 0)); // J2N: To match .NET, this is only a no-op if both int parameters are 0
            assertEquals("0000", sb.ToString());
            assertEquals(4, sb.Length);


            try
            {
                sb = new StringBuffer(fixture);
                sb.Insert(-1, (char[])"ab".ToCharArray(), 0, 2 - 0); // J2N: Corrected 4th parameter
                fail("no IOOBE, negative index");
            }
            catch (ArgumentOutOfRangeException e)
            {
                // Expected
            }

            try
            {
                sb = new StringBuffer(fixture);
                sb.Insert(5, (char[])"ab".ToCharArray(), 0, 2 - 0); // J2N: Corrected 4th parameter
                fail("no IOOBE, index too large index");
            }
            catch (ArgumentOutOfRangeException e)
            {
                // Expected
            }

            try
            {
                sb = new StringBuffer(fixture);
                sb.Insert(5, (char[])"ab".ToCharArray(), -1, 2 - -1); // J2N: Corrected 4th parameter
                fail("no IOOBE, negative offset");
            }
            catch (ArgumentOutOfRangeException e)
            {
                // Expected
            }

            try
            {
                sb = new StringBuffer(fixture);
                sb.Insert(5, new char[] { 'a', 'b' }, 0, -1 - 0); // J2N: Corrected 4th parameter
                fail("no IOOBE, negative length");
            }
            catch (ArgumentOutOfRangeException e)
            {
                // Expected
            }

            try
            {
                sb = new StringBuffer(fixture);
                sb.Insert(5, new char[] { 'a', 'b' }, 0, 3 - 0); // J2N: Corrected 4th parameter
                fail("no IOOBE, too long");
            }
            catch (ArgumentOutOfRangeException e)
            {
                // Expected
            }
        }

        /**
         * @tests java.lang.StringBuffer.insert(int, CharSequence, int, int)
         */
        [Test]
        public void Test_Insert_Int32_StringBuilder_Int32_Int32()
        {
            string fixture = "0000";
            StringBuffer sb = new StringBuffer(fixture);
            assertSame(sb, sb.Insert(0, (StringBuilder)new StringBuilder("ab"), 0, 2 - 0)); // J2N: Corrected 4th parameter
            assertEquals("ab0000", sb.ToString());
            assertEquals(6, sb.Length);

            sb = new StringBuffer(fixture);
            assertSame(sb, sb.Insert(0, (StringBuilder)new StringBuilder("ab"), 0, 1 - 0)); // J2N: Corrected 4th parameter
            assertEquals("a0000", sb.ToString());
            assertEquals(5, sb.Length);

            sb = new StringBuffer(fixture);
            assertSame(sb, sb.Insert(2, (StringBuilder)new StringBuilder("ab"), 0, 2 - 0)); // J2N: Corrected 4th parameter
            assertEquals("00ab00", sb.ToString());
            assertEquals(6, sb.Length);

            sb = new StringBuffer(fixture);
            assertSame(sb, sb.Insert(2, (StringBuilder)new StringBuilder("ab"), 0, 1 - 0)); // J2N: Corrected 4th parameter
            assertEquals("00a00", sb.ToString());
            assertEquals(5, sb.Length);

            sb = new StringBuffer(fixture);
            assertSame(sb, sb.Insert(4, (StringBuilder)new StringBuilder("ab"), 0, 2 - 0)); // J2N: Corrected 4th parameter
            assertEquals("0000ab", sb.ToString());
            assertEquals(6, sb.Length);

            sb = new StringBuffer(fixture);
            assertSame(sb, sb.Insert(4, (StringBuilder)new StringBuilder("ab"), 0, 1 - 0)); // J2N: Corrected 4th parameter
            assertEquals("0000a", sb.ToString());
            assertEquals(5, sb.Length);

            sb = new StringBuffer(fixture);
            //assertSame(sb, sb.Insert(4, (StringBuilder)null, 0, 2));
            //assertEquals("0000nu", sb.ToString());
            //assertEquals(6, sb.Length);

            assertSame(sb, sb.Insert(4, (StringBuilder)null, 0, 0)); // J2N: To match .NET, this is only a no-op if both int parameters are 0
            assertEquals("0000", sb.ToString());
            assertEquals(4, sb.Length);


            try
            {
                sb = new StringBuffer(fixture);
                sb.Insert(-1, (StringBuilder)new StringBuilder("ab"), 0, 2 - 0); // J2N: Corrected 4th parameter
                fail("no IOOBE, negative index");
            }
            catch (ArgumentOutOfRangeException e)
            {
                // Expected
            }

            try
            {
                sb = new StringBuffer(fixture);
                sb.Insert(5, (StringBuilder)new StringBuilder("ab"), 0, 2 - 0); // J2N: Corrected 4th parameter
                fail("no IOOBE, index too large index");
            }
            catch (ArgumentOutOfRangeException e)
            {
                // Expected
            }

            try
            {
                sb = new StringBuffer(fixture);
                sb.Insert(5, (StringBuilder)new StringBuilder("ab"), -1, 2 - -1); // J2N: Corrected 4th parameter
                fail("no IOOBE, negative offset");
            }
            catch (ArgumentOutOfRangeException e)
            {
                // Expected
            }

            try
            {
                sb = new StringBuffer(fixture);
                sb.Insert(5, new StringBuilder().Append(new char[] { 'a', 'b' }), 0, -1 - 0); // J2N: Corrected 4th parameter
                fail("no IOOBE, negative length");
            }
            catch (ArgumentOutOfRangeException e)
            {
                // Expected
            }

            try
            {
                sb = new StringBuffer(fixture);
                sb.Insert(5, new StringBuilder().Append(new char[] { 'a', 'b' }), 0, 3 - 0); // J2N: Corrected 4th parameter
                fail("no IOOBE, too long");
            }
            catch (ArgumentOutOfRangeException e)
            {
                // Expected
            }
        }

        /**
         * @tests java.lang.StringBuffer.insert(int, CharSequence, int, int)
         */
        [Test]
        public void Test_Insert_Int32_String_Int32_Int32()
        {
            string fixture = "0000";
            StringBuffer sb = new StringBuffer(fixture);
            assertSame(sb, sb.Insert(0, (string)"ab", 0, 2 - 0)); // J2N: Corrected 4th parameter
            assertEquals("ab0000", sb.ToString());
            assertEquals(6, sb.Length);

            sb = new StringBuffer(fixture);
            assertSame(sb, sb.Insert(0, (string)"ab", 0, 1 - 0)); // J2N: Corrected 4th parameter
            assertEquals("a0000", sb.ToString());
            assertEquals(5, sb.Length);

            sb = new StringBuffer(fixture);
            assertSame(sb, sb.Insert(2, (string)"ab", 0, 2 - 0)); // J2N: Corrected 4th parameter
            assertEquals("00ab00", sb.ToString());
            assertEquals(6, sb.Length);

            sb = new StringBuffer(fixture);
            assertSame(sb, sb.Insert(2, (string)"ab", 0, 1 - 0)); // J2N: Corrected 4th parameter
            assertEquals("00a00", sb.ToString());
            assertEquals(5, sb.Length);

            sb = new StringBuffer(fixture);
            assertSame(sb, sb.Insert(4, (string)"ab", 0, 2 - 0)); // J2N: Corrected 4th parameter
            assertEquals("0000ab", sb.ToString());
            assertEquals(6, sb.Length);

            sb = new StringBuffer(fixture);
            assertSame(sb, sb.Insert(4, (string)"ab", 0, 1 - 0)); // J2N: Corrected 4th parameter
            assertEquals("0000a", sb.ToString());
            assertEquals(5, sb.Length);

            sb = new StringBuffer(fixture);
            //assertSame(sb, sb.Insert(4, (string)null, 0, 2));
            //assertEquals("0000nu", sb.ToString());
            //assertEquals(6, sb.Length);

            assertSame(sb, sb.Insert(4, (string)null, 0, 0)); // J2N: To match .NET, this is only a no-op if both int parameters are 0
            assertEquals("0000", sb.ToString());
            assertEquals(4, sb.Length);


            try
            {
                sb = new StringBuffer(fixture);
                sb.Insert(-1, (string)"ab", 0, 2 - 0); // J2N: Corrected 4th parameter
                fail("no IOOBE, negative index");
            }
            catch (ArgumentOutOfRangeException e)
            {
                // Expected
            }

            try
            {
                sb = new StringBuffer(fixture);
                sb.Insert(5, (string)"ab", 0, 2 - 0); // J2N: Corrected 4th parameter
                fail("no IOOBE, index too large index");
            }
            catch (ArgumentOutOfRangeException e)
            {
                // Expected
            }

            try
            {
                sb = new StringBuffer(fixture);
                sb.Insert(5, (string)"ab", -1, 2 - -1); // J2N: Corrected 4th parameter
                fail("no IOOBE, negative offset");
            }
            catch (ArgumentOutOfRangeException e)
            {
                // Expected
            }

            try
            {
                sb = new StringBuffer(fixture);
                sb.Insert(5, new string(new char[] { 'a', 'b' }), 0, -1 - 0); // J2N: Corrected 4th parameter
                fail("no IOOBE, negative length");
            }
            catch (ArgumentOutOfRangeException e)
            {
                // Expected
            }

            try
            {
                sb = new StringBuffer(fixture);
                sb.Insert(5, new string(new char[] { 'a', 'b' }), 0, 3 - 0); // J2N: Corrected 4th parameter
                fail("no IOOBE, too long");
            }
            catch (ArgumentOutOfRangeException e)
            {
                // Expected
            }
        }

        /**
         * @tests java.lang.StringBuffer.insert(int, char)
         */
        [Test]
        public void Test_insertIC()
        {
            StringBuffer obj = new StringBuffer();
            try
            {
                obj.Insert(-1, ' ');
                fail("ArrayIndexOutOfBoundsException expected");
            }
            catch (ArgumentOutOfRangeException e)
            {
                // expected
            }
        }

        /**
         * @tests java.lang.StringBuffer.appendCodePoint(int)'
         */
        [Test]
        public void Test_appendCodePointI()
        {
            StringBuffer sb = new StringBuffer();
            sb.AppendCodePoint(0x10000);
            assertEquals("\uD800\uDC00", sb.ToString());
            sb.Append("fixture");
            assertEquals("\uD800\uDC00fixture", sb.ToString());
            sb.AppendCodePoint(0x00010FFFF);
            assertEquals("\uD800\uDC00fixture\uDBFF\uDFFF", sb.ToString());
        }

        /**
         * @tests java.lang.StringBuffer.codePointAt(int)
         */
        [Test]
        public void Test_codePointAtI()
        {
            StringBuffer sb = new StringBuffer("abc");
            assertEquals('a', sb.CodePointAt(0));
            assertEquals('b', sb.CodePointAt(1));
            assertEquals('c', sb.CodePointAt(2));

            sb = new StringBuffer("\uD800\uDC00");
            assertEquals(0x10000, sb.CodePointAt(0));
            assertEquals('\uDC00', sb.CodePointAt(1));

            try
            {
                sb.CodePointAt(-1);
                fail("No IOOBE on negative index.");
            }
            catch (ArgumentOutOfRangeException e)
            {

            }

            try
            {
                sb.CodePointAt(sb.Length);
                fail("No IOOBE on index equal to length.");
            }
            catch (ArgumentOutOfRangeException e)
            {

            }

            try
            {
                sb.CodePointAt(sb.Length + 1);
                fail("No IOOBE on index greater than length.");
            }
            catch (ArgumentOutOfRangeException e)
            {

            }
        }

        /**
         * @tests java.lang.StringBuffer.codePointBefore(int)
         */
        [Test]
        public void Test_codePointBeforeI()
        {
            StringBuffer sb = new StringBuffer("abc");
            assertEquals('a', sb.CodePointBefore(1));
            assertEquals('b', sb.CodePointBefore(2));
            assertEquals('c', sb.CodePointBefore(3));

            sb = new StringBuffer("\uD800\uDC00");
            assertEquals(0x10000, sb.CodePointBefore(2));
            assertEquals('\uD800', sb.CodePointBefore(1));

            try
            {
                sb.CodePointBefore(0);
                fail("No IOOBE on zero index.");
            }
            catch (ArgumentOutOfRangeException e)
            {

            }

            try
            {
                sb.CodePointBefore(-1);
                fail("No IOOBE on negative index.");
            }
            catch (ArgumentOutOfRangeException e)
            {

            }

            try
            {
                sb.CodePointBefore(sb.Length + 1);
                fail("No IOOBE on index greater than length.");
            }
            catch (ArgumentOutOfRangeException e)
            {

            }
        }

        /**
         * @tests java.lang.StringBuffer.codePointCount(int, int)
         */
        [Test]
        public void Test_codePointCountII()
        {
            assertEquals(1, new StringBuffer("\uD800\uDC00").CodePointCount(0, 2 - 0)); // J2N: Corrected length parameter
            assertEquals(1, new StringBuffer("\uD800\uDC01").CodePointCount(0, 2 - 0)); // J2N: Corrected length parameter
            assertEquals(1, new StringBuffer("\uD801\uDC01").CodePointCount(0, 2 - 0)); // J2N: Corrected length parameter
            assertEquals(1, new StringBuffer("\uDBFF\uDFFF").CodePointCount(0, 2 - 0)); // J2N: Corrected length parameter

            assertEquals(3, new StringBuffer("a\uD800\uDC00b").CodePointCount(0, 4 - 0)); // J2N: Corrected length parameter
            assertEquals(4, new StringBuffer("a\uD800\uDC00b\uD800").CodePointCount(0, 5 - 0)); // J2N: Corrected length parameter

            StringBuffer sb = new StringBuffer("abc");
            try
            {
                sb.CodePointCount(-1, 2 - -1); // J2N: Corrected length parameter
                fail("No IOOBE for negative begin index.");
            }
            catch (ArgumentOutOfRangeException e)
            {

            }

            try
            {
                sb.CodePointCount(0, 4 - 0); // J2N: Corrected length parameter
                fail("No IOOBE for end index that's too large.");
            }
            catch (ArgumentOutOfRangeException e)
            {

            }

            try
            {
                sb.CodePointCount(3, 2 - 3); // J2N: Corrected length parameter
                fail("No IOOBE for begin index larger than end index.");
            }
            catch (ArgumentOutOfRangeException e)
            {

            }
        }

        /**
         * @tests java.lang.StringBuffer.getChars(int, int, char[], int)
         */
        [Test]
        public void Test_getCharsII_CI()
        {
            StringBuffer obj = new StringBuffer();
            try
            {
                //obj.getChars(0, 0, new char[0], -1);
                obj.CopyTo(0, new char[0], -1, 0);
                fail("ArrayIndexOutOfBoundsException expected");
            }
            catch (ArgumentOutOfRangeException e)
            {
                // expected
            }
        }

        /**
         * @tests java.lang.StringBuffer.offsetByCodePoints(int, int)'
         */
        [Test]
        public void Test_offsetByCodePointsII()
        {
            int result = new StringBuffer("a\uD800\uDC00b").OffsetByCodePoints(0, 2);
            assertEquals(3, result);

            result = new StringBuffer("abcd").OffsetByCodePoints(3, -1);
            assertEquals(2, result);

            result = new StringBuffer("a\uD800\uDC00b").OffsetByCodePoints(0, 3);
            assertEquals(4, result);

            result = new StringBuffer("a\uD800\uDC00b").OffsetByCodePoints(3, -1);
            assertEquals(1, result);

            result = new StringBuffer("a\uD800\uDC00b").OffsetByCodePoints(3, 0);
            assertEquals(3, result);

            result = new StringBuffer("\uD800\uDC00bc").OffsetByCodePoints(3, 0);
            assertEquals(3, result);

            result = new StringBuffer("a\uDC00bc").OffsetByCodePoints(3, -1);
            assertEquals(2, result);

            result = new StringBuffer("a\uD800bc").OffsetByCodePoints(3, -1);
            assertEquals(2, result);

            StringBuffer sb = new StringBuffer("abc");
            try
            {
                sb.OffsetByCodePoints(-1, 1);
                fail("No IOOBE for negative index.");
            }
            catch (ArgumentOutOfRangeException e)
            {

            }

            try
            {
                sb.OffsetByCodePoints(0, 4);
                fail("No IOOBE for offset that's too large.");
            }
            catch (ArgumentOutOfRangeException e)
            {

            }

            try
            {
                sb.OffsetByCodePoints(3, -4);
                fail("No IOOBE for offset that's too small.");
            }
            catch (ArgumentOutOfRangeException e)
            {

            }

            try
            {
                sb.OffsetByCodePoints(3, 1);
                fail("No IOOBE for index that's too large.");
            }
            catch (ArgumentOutOfRangeException e)
            {

            }

            try
            {
                sb.OffsetByCodePoints(4, -1);
                fail("No IOOBE for index that's too large.");
            }
            catch (ArgumentOutOfRangeException e)
            {

            }
        }

        /**
         * @tests {@link java.lang.StringBuffer#indexOf(String, int)}
         */
        [Test]
        public void Test_IndexOfStringInt()
        {
            string fixture = "0123456789";
            StringBuffer sb = new StringBuffer(fixture);
            assertEquals(0, sb.IndexOf("0", StringComparison.Ordinal));
            assertEquals(0, sb.IndexOf("012", StringComparison.Ordinal));
            assertEquals(-1, sb.IndexOf("02", StringComparison.Ordinal));
            assertEquals(8, sb.IndexOf("89", StringComparison.Ordinal));

            assertEquals(0, sb.IndexOf("0", StringComparison.Ordinal), 0);
            assertEquals(0, sb.IndexOf("012", StringComparison.Ordinal), 0);
            assertEquals(-1, sb.IndexOf("02", StringComparison.Ordinal), 0);
            assertEquals(8, sb.IndexOf("89", StringComparison.Ordinal), 0);

            assertEquals(-1, sb.IndexOf("0", StringComparison.Ordinal), 5);
            assertEquals(-1, sb.IndexOf("012", StringComparison.Ordinal), 5);
            assertEquals(-1, sb.IndexOf("02", StringComparison.Ordinal), 0);
            assertEquals(8, sb.IndexOf("89", StringComparison.Ordinal), 5);

            try
            {
                sb.IndexOf(null, 0, StringComparison.Ordinal);
                fail("Should throw a NullPointerExceptionE");
            }
            catch (ArgumentNullException e)
            {
                // Expected
            }
        }

        /**
         * @tests {@link java.lang.StringBuffer#lastIndexOf(String, int)}
         */
        [Test]
        public void Test_lastIndexOfLjava_lang_StringI()
        {
            string fixture = "0123456789";
            StringBuffer sb = new StringBuffer(fixture);
            assertEquals(0, sb.LastIndexOf("0", StringComparison.Ordinal));
            assertEquals(0, sb.LastIndexOf("012", StringComparison.Ordinal));
            assertEquals(-1, sb.LastIndexOf("02", StringComparison.Ordinal));
            assertEquals(8, sb.LastIndexOf("89", StringComparison.Ordinal));

            assertEquals(0, sb.LastIndexOf("0", StringComparison.Ordinal), 0);
            assertEquals(0, sb.LastIndexOf("012", StringComparison.Ordinal), 0);
            assertEquals(-1, sb.LastIndexOf("02", StringComparison.Ordinal), 0);
            assertEquals(8, sb.LastIndexOf("89", StringComparison.Ordinal), 0);

            assertEquals(-1, sb.LastIndexOf("0", StringComparison.Ordinal), 5);
            assertEquals(-1, sb.LastIndexOf("012", StringComparison.Ordinal), 5);
            assertEquals(-1, sb.LastIndexOf("02", StringComparison.Ordinal), 0);
            assertEquals(8, sb.LastIndexOf("89", StringComparison.Ordinal), 5);

            try
            {
                sb.LastIndexOf(null, 0, StringComparison.Ordinal);
                fail("Should throw a NullPointerException");
            }
            catch (ArgumentNullException e)
            {
                // Expected
            }
        }

        //// comparator for StringBuffer objects
        //private static final SerializableAssert STRING_BUFFER_COMPARATOR = new SerializableAssert()
        //{
        //    public void assertDeserialized(Serializable initial,
        //            Serializable deserialized)
        //    {

//        StringBuffer init = (StringBuffer)initial;
//        StringBuffer desr = (StringBuffer)deserialized;

//        // serializable fields are: 'count', 'shared', 'value'
//        // serialization of 'shared' is not verified
//        // 'count' + 'value' should result in required string
//        assertEquals("toString", init.ToString(), desr.ToString());
//    }
//};

#if FEATURE_SERIALIZABLE_STRINGS
        /**
         * @tests serialization/deserialization.
         */
        [Test]
        public void TestSerializationSelf()
        {

            //SerializationTest.verifySelf(new StringBuffer("0123456789"),
            //            STRING_BUFFER_COMPARATOR);

            var target = new StringBuffer("0123456789");

            var clone = Clone(target);

            assertNotSame(target, clone);
            assertNotSame(target.builder, clone.builder);
            assertTrue(target.Equals(clone));

            clone.Append("5");
            assertFalse(target.Equals(clone));
            assertFalse(target.Length == clone.Length);
        }
#endif

        /////**
        //// * @tests serialization/deserialization compatibility with RI.
        //// */
        ////[Test]
        ////public void TestSerializationCompatibility()
        ////{

        ////    SerializationTest.verifyGolden(this, new StringBuffer("0123456789"),
        ////                STRING_BUFFER_COMPARATOR);
        ////}
    }
}
