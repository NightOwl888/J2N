#region Copyright 2010 by Apache Harmony, Licensed under the Apache License, Version 2.0
/*  Licensed to the Apache Software Foundation (ASF) under one or more
 *  contributor license agreements.  See the NOTICE file distributed with
 *  this work for additional information regarding copyright ownership.
 *  The ASF licenses this file to You under the Apache License, Version 2.0
 *  (the "License"); you may not use this file except in compliance with
 *  the License.  You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 *  Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *  limitations under the License.
 */
#endregion

using J2N.Collections;
using NUnit.Framework;
using System;
using System.Globalization;
using System.Text;
using Double = J2N.Numerics.Double;
using Float = J2N.Numerics.Single;
using Integer = J2N.Numerics.Int32;
using Long = J2N.Numerics.Int64;

namespace J2N.Text
{
    /// <summary>
    /// Tests from Apache Harmony, StringBuilderTest.java
    /// </summary>
    public abstract partial class StringBuilderTestBase
    {
        /**
         * @tests java.lang.StringBuilder.StringBuilder()
         */
        [Test]
        public virtual void Test_Constructor()
        {
            TextBuilder sb = StringBuilderFactory();
            assertNotNull(sb);
            assertEquals(16, sb.Capacity);
        }

        /**
         * @tests java.lang.StringBuilder.StringBuilder(int)
         */
        [Test]
        public virtual void Test_ConstructorI()
        {
            TextBuilder sb = StringBuilderFactory(24);
            assertNotNull(sb);
            assertEquals(24, sb.Capacity);

            try
            {
                new StringBuilder(-1);
                fail("no exception");
            }
            catch (ArgumentOutOfRangeException) // NegativeArraySizeException
            {
                // Expected
            }

            assertNotNull(StringBuilderFactory(0));
        }

        /**
         * @tests java.lang.StringBuilder.StringBuilder(CharSequence)
         */
        //@SuppressWarnings("cast")
        [Test]
        public virtual void Test_ConstructorLjava_lang_CharSequence()
        {
            TextBuilder sb = StringBuilderFactory("fixture".AsCharSequence());
            assertEquals("fixture", sb.ToString());
            assertEquals("fixture".Length + 16, sb.Capacity);

            sb = StringBuilderFactory((ICharSequence)new StringBuffer("fixture"));
            assertEquals("fixture", sb.ToString());
            assertEquals("fixture".Length + 16, sb.Capacity);

            // J2N: Changed behavior to match .NET string overload to allow null
            sb = StringBuilderFactory((ICharSequence)null);
            assertEquals("", sb.ToString());
            //try
            //{
            //    OpenStringBuilderFactory(ICharSequence)null);
            //    fail("no NPE");
            //}
            //catch (ArgumentNullException) // J2N: Using ArgumentNullException instead of NullReferenceException
            //{
            //    // Expected
            //}
        }

        /**
         * @tests java.lang.StringBuilder.StringBuilder(String)
         */
        [Test]
        public virtual void Test_ConstructorLjava_lang_String()
        {
            TextBuilder sb = StringBuilderFactory("fixture");
            assertEquals("fixture", sb.ToString());
            assertEquals("fixture".Length + 16, sb.Capacity);

            // J2N: Changed behavior to match .NET string overload to allow null
            sb = StringBuilderFactory((string)null);
            assertEquals("", sb.ToString());
            //try
            //{
            //    OpenStringBuilderFactory(string)null);
            //    fail("no NPE");
            //}
            //catch (ArgumentNullException) // NullPointerException
            //{
            //}
        }

        /**
         * @tests java.lang.StringBuilder.StringBuilder(ReadOnlySpan<char>)
         */
        [Test]
        public virtual void Test_ConstructorLjava_lang_ReadOnlySpan()
        {
            TextBuilder sb = StringBuilderFactory("fixture".AsSpan());
            assertEquals("fixture", sb.ToString());
            assertEquals("fixture".Length + 16, sb.Capacity);

            // J2N: Changed behavior to match .NET string overload to allow null
            sb = StringBuilderFactory(ReadOnlySpan<char>.Empty);
            assertEquals("", sb.ToString());
            assertTrue(sb.AsSpan().IsEmpty);
            //try
            //{
            //    OpenStringBuilderFactory(string)null);
            //    fail("no NPE");
            //}
            //catch (ArgumentNullException) // NullPointerException
            //{
            //}
        }

        /**
         * @tests java.lang.StringBuilder.Append(boolean)
         */
        [Test]
        public void Test_appendZ()
        {
            TextBuilder sb = StringBuilderFactory();
            assertSame(sb, sb.Append(true));
            assertEquals("true", sb.ToString());
            sb.Length = (0);
            assertSame(sb, sb.Append(false));
            assertEquals("false", sb.ToString());
        }

        /**
         * @tests java.lang.StringBuilder.Append(char)
         */
        [Test]
        public void Test_appendC()
        {
            TextBuilder sb = StringBuilderFactory();
            assertSame(sb, sb.Append('a'));
            assertEquals("a", sb.ToString());
            sb.Length = (0);
            assertSame(sb, sb.Append('b'));
            assertEquals("b", sb.ToString());
        }

        /**
         * @tests java.lang.StringBuilder.Append(char[])
         */
        [Test]
        public void Test_append_C()
        {
            TextBuilder sb = StringBuilderFactory();
            assertSame(sb, sb.Append(new char[] { 'a', 'b' }));
            assertEquals("ab", sb.ToString());
            sb.Length = (0);
            assertSame(sb, sb.Append(new char[] { 'c', 'd' }));
            assertEquals("cd", sb.ToString());

            // J2N: In .NET, appending null is a no-op rather than something that should throw
            sb.Append((char[])null);
            assertEquals("cd", sb.ToString());

            //try
            //{
            //    sb.Append((char[])null);
            //    fail("no NPE");
            //}
            //catch (ArgumentNullException) // NullPointerException
            //{
            //    // Expected
            //}
        }

        /**
         * @tests java.lang.StringBuilder.Append(char[], int, int)
         */
        [Test]
        public void Test_append_CII()
        {
            TextBuilder sb = StringBuilderFactory();
            assertSame(sb, sb.Append(new char[] { 'a', 'b' }, 0, 2));
            assertEquals("ab", sb.ToString());
            sb.Length = (0);
            assertSame(sb, sb.Append(new char[] { 'c', 'd' }, 0, 2));
            assertEquals("cd", sb.ToString());

            sb.Length = (0);
            assertSame(sb, sb.Append(new char[] { 'a', 'b', 'c', 'd' }, 0, 2));
            assertEquals("ab", sb.ToString());

            sb.Length = (0);
            assertSame(sb, sb.Append(new char[] { 'a', 'b', 'c', 'd' }, 2, 2));
            assertEquals("cd", sb.ToString());

            sb.Length = (0);
            assertSame(sb, sb.Append(new char[] { 'a', 'b', 'c', 'd' }, 2, 0));
            assertEquals("", sb.ToString());

            try
            {
                sb.Append((char[])null, 0, 2);
                fail("no NPE");
            }
            catch (ArgumentNullException) // NullPointerException
            {
                // Expected
            }

            try
            {
                sb.Append(new char[] { 'a', 'b', 'c', 'd' }, -1, 2);
                fail("no IOOBE, negative offset");
            }
            catch (ArgumentOutOfRangeException) // IndexOutOfBoundsException
            {
                // Expected
            }

            try
            {
                sb.Append(new char[] { 'a', 'b', 'c', 'd' }, 0, -1);
                fail("no IOOBE, negative length");
            }
            catch (ArgumentOutOfRangeException) // IndexOutOfBoundsException
            {
                // Expected
            }

            try
            {
                sb.Append(new char[] { 'a', 'b', 'c', 'd' }, 2, 3);
                fail("no IOOBE, offset and length overflow");
            }
            catch (ArgumentOutOfRangeException) // IndexOutOfBoundsException
            {
                // Expected
            }
        }

        /**
         * @tests java.lang.StringBuilder.Append(CharSequence)
         */
        [Test]
        public void Test_appendLjava_lang_CharSequence()
        {
            TextBuilder sb = StringBuilderFactory();
            assertSame(sb, sb.Append((ICharSequence)"ab".AsCharSequence()));
            assertEquals("ab", sb.ToString());
            sb.Length = (0);
            assertSame(sb, sb.Append((ICharSequence)"cd".AsCharSequence()));
            assertEquals("cd", sb.ToString());
            sb.Length = (0);
            assertSame(sb, sb.Append((ICharSequence)null));
            //assertEquals("null", sb.ToString());
            assertEquals("", sb.ToString()); // J2N: Changed the behavior to be a no-op rather than appending the string "null"
        }

        /**
         * @tests java.lang.StringBuilder.Append(CharSequence, int, int)
         */
        //@SuppressWarnings("cast")
        [Test]
        public void Test_appendLjava_lang_CharSequenceII()
        {
            TextBuilder sb = StringBuilderFactory();
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
            //assertSame(sb, sb.Append((ICharSequence)null, 0, 2 - 0)); // J2N: Changed the behavior to throw an exception (to match .NET Core 3.0's Append(StringBuilder,int,int) overload) rather than appending the string "null"
            try
            {
                sb.Append((ICharSequence)null, 0, 2 - 0);
                fail("no ArgumentOutOfRangeException, offset and length overflow");
            }
            catch (ArgumentNullException)
            {
                // Expected
            }
            //assertEquals("nu", sb.ToString());
            assertEquals("", sb.ToString()); // J2N: Changed the behavior to be a no-op rather than appending the string "null"
        }

        /**
         * @tests java.lang.StringBuilder.Append(double)
         */
        [Test]
        public void Test_appendD() // J2N TODO: Invariant
        {
            TextBuilder sb = StringBuilderFactory();
            assertSame(sb, sb.Append(1D));
            assertEquals(Double.ToString(1D), sb.ToString());
            sb.Length = (0);
            assertSame(sb, sb.Append(0D));
            assertEquals(Double.ToString(0D), sb.ToString());
            sb.Length = (0);
            assertSame(sb, sb.Append(-1D));
            assertEquals(Double.ToString(-1D), sb.ToString());
            sb.Length = (0);
            assertSame(sb, sb.Append(double.NaN));
            assertEquals(Double.ToString(double.NaN), sb.ToString());
            sb.Length = (0);
            assertSame(sb, sb.Append(double.NegativeInfinity));
            assertEquals(Double.ToString(double.NegativeInfinity, CultureInfo.InvariantCulture), sb.ToString());
            sb.Length = (0);
            assertSame(sb, sb.Append(double.PositiveInfinity));
            assertEquals(Double.ToString(double.PositiveInfinity, CultureInfo.InvariantCulture), sb.ToString());
            sb.Length = (0);
            assertSame(sb, sb.Append(double.Epsilon));
            assertEquals(Double.ToString(double.Epsilon, CultureInfo.InvariantCulture), sb.ToString());
            sb.Length = (0);
            assertSame(sb, sb.Append(double.MaxValue));
            assertEquals(Double.ToString(double.MaxValue, CultureInfo.InvariantCulture), sb.ToString());
        }

        /**
         * @tests java.lang.StringBuilder.Append(float)
         */
        [Test]
        public void Test_appendF() // J2N TODO: Invariant
        {
            TextBuilder sb = StringBuilderFactory();
            assertSame(sb, sb.Append(1F));
            assertEquals(Float.ToString(1F), sb.ToString());
            sb.Length = (0);
            assertSame(sb, sb.Append(0F));
            assertEquals(Float.ToString(0F), sb.ToString());
            sb.Length = (0);
            assertSame(sb, sb.Append(-1F));
            assertEquals(Float.ToString(-1F), sb.ToString());
            sb.Length = (0);
            assertSame(sb, sb.Append(float.NaN));
            assertEquals(Float.ToString(float.NaN), sb.ToString());
            sb.Length = (0);
            assertSame(sb, sb.Append(float.NegativeInfinity));
            assertEquals(Float.ToString(float.NegativeInfinity, CultureInfo.InvariantCulture), sb.ToString());
            sb.Length = (0);
            assertSame(sb, sb.Append(float.PositiveInfinity));
            assertEquals(Float.ToString(float.PositiveInfinity, CultureInfo.InvariantCulture), sb.ToString());
            sb.Length = (0);
            assertSame(sb, sb.Append(float.Epsilon));
            assertEquals(Float.ToString(float.Epsilon, CultureInfo.InvariantCulture), sb.ToString());
            sb.Length = (0);
            assertSame(sb, sb.Append(float.MaxValue));
            assertEquals(Float.ToString(float.MaxValue, CultureInfo.InvariantCulture), sb.ToString());
        }

        /**
         * @tests java.lang.StringBuilder.Append(int)
         */
        [Test]
        public void Test_appendI() // J2N TODO: Invariant
        {
            TextBuilder sb = StringBuilderFactory();
            assertSame(sb, sb.Append(1));
            assertEquals(Integer.ToString(1), sb.ToString());
            sb.Length = (0);
            assertSame(sb, sb.Append(0));
            assertEquals(Integer.ToString(0), sb.ToString());
            sb.Length = (0);
            assertSame(sb, sb.Append(-1));
            assertEquals(Integer.ToString(-1), sb.ToString());
            sb.Length = (0);
            assertSame(sb, sb.Append(int.MinValue));
            assertEquals(Integer.ToString(int.MinValue), sb.ToString());
            sb.Length = (0);
            assertSame(sb, sb.Append(int.MaxValue));
            assertEquals(Integer.ToString(int.MaxValue), sb.ToString());
        }

        /**
         * @tests java.lang.StringBuilder.Append(long)
         */
        [Test]
        public void Test_appendL() // J2N TODO: Invariant
        {
            TextBuilder sb = StringBuilderFactory();
            assertSame(sb, sb.Append(1L));
            assertEquals(Long.ToString(1L), sb.ToString());
            sb.Length = (0);
            assertSame(sb, sb.Append(0L));
            assertEquals(Long.ToString(0L), sb.ToString());
            sb.Length = (0);
            assertSame(sb, sb.Append(-1L));
            assertEquals(Long.ToString(-1L), sb.ToString());
            sb.Length = (0);
            assertSame(sb, sb.Append(long.MinValue));
            assertEquals(Long.ToString(long.MinValue), sb.ToString());
            sb.Length = (0);
            assertSame(sb, sb.Append(long.MaxValue));
            assertEquals(Long.ToString(long.MaxValue), sb.ToString());
        }

        /**
         * @tests java.lang.StringBuilder.Append(Object)'
         */
        [Test]
        public void Test_appendLjava_lang_Object()
        {
            TextBuilder sb = StringBuilderFactory();
            assertSame(sb, sb.Append(Fixture.INSTANCE));
            assertEquals(Fixture.INSTANCE.ToString(), sb.ToString());

            sb.Length = (0);
            assertSame(sb, sb.Append((Object)null));
            //assertEquals("null", sb.ToString());
            assertEquals("", sb.ToString()); // J2N: Changed the behavior to be a no-op rather than appending the string "null"
        }

        /**
         * @tests java.lang.StringBuilder.Append(String)
         */
        [Test]
        public void Test_appendLjava_lang_String()
        {
            TextBuilder sb = StringBuilderFactory();
            assertSame(sb, sb.Append("ab"));
            assertEquals("ab", sb.ToString());
            sb.Length = (0);
            assertSame(sb, sb.Append("cd"));
            assertEquals("cd", sb.ToString());
            sb.Length = (0);
            assertSame(sb, sb.Append((string)null));
            //assertEquals("null", sb.ToString());
            assertEquals("", sb.ToString()); // J2N: Changed the behavior to be a no-op rather than appending the string "null"
        }

        /**
         * @tests java.lang.StringBuilder.Append(StringBuffer)
         */
        [Test]
        public void Test_appendLjava_lang_StringBuffer()
        {
            TextBuilder sb = StringBuilderFactory();
            assertSame(sb, sb.Append(new StringBuffer("ab")));
            assertEquals("ab", sb.ToString());
            sb.Length = (0);
            assertSame(sb, sb.Append(new StringBuffer("cd")));
            assertEquals("cd", sb.ToString());
            sb.Length = (0);
            assertSame(sb, sb.Append((StringBuffer)null));
            //assertEquals("null", sb.ToString());
            assertEquals("", sb.ToString()); // J2N: Changed the behavior to be a no-op rather than appending the string "null"
        }

        /**
         * @tests java.lang.StringBuilder.AppendCodePoint(int)'
         */
        [Test]
        public void Test_appendCodePointI()
        {
            TextBuilder sb = StringBuilderFactory();
            sb.AppendCodePoint(0x10000);
            assertEquals("\uD800\uDC00", sb.ToString());
            sb.Append("fixture");
            assertEquals("\uD800\uDC00fixture", sb.ToString());
            sb.AppendCodePoint(0x00010FFFF);
            assertEquals("\uD800\uDC00fixture\uDBFF\uDFFF", sb.ToString());
        }

        /**
         * @tests java.lang.StringBuilder.Capacity'
         */
        [Test]
        public void Test_capacity()
        {
            TextBuilder sb = StringBuilderFactory();
            assertEquals(16, sb.Capacity);
            sb.Append("0123456789ABCDEF0123456789ABCDEF");
            assertTrue(sb.Capacity > 16);
        }

        /**
         * @tests java.lang.StringBuilder.charAt(int)'
         */
        [Test]
        public void Test_charAtI()
        {
            const string fixture = "0123456789";
            TextBuilder sb = StringBuilderFactory(fixture);
            for (int i = 0; i < fixture.Length; i++)
            {
                assertEquals((char)('0' + i), sb[i]);
            }

            try
            {
                _ = sb[-1];
                fail("no IOOBE, negative index");
            }
            catch (IndexOutOfRangeException) // IndexOutOfBoundsException
            {
                // Expected
            }

            try
            {
                _ = sb[fixture.Length];
                fail("no IOOBE, equal to length");
            }
            catch (IndexOutOfRangeException) // IndexOutOfBoundsException
            {
            }

            try
            {
                _ = sb[fixture.Length + 1];
                fail("no IOOBE, greater than length");
            }
            catch (IndexOutOfRangeException) // IndexOutOfBoundsException
            {
            }
        }

        /**
         * @tests java.lang.StringBuilder.CodePointAt(int)
         */
        [Test]
        public void Test_codePointAtI()
        {
            TextBuilder sb = StringBuilderFactory("abc");
            assertEquals('a', sb.CodePointAt(0));
            assertEquals('b', sb.CodePointAt(1));
            assertEquals('c', sb.CodePointAt(2));

            sb = StringBuilderFactory("\uD800\uDC00");
            assertEquals(0x10000, sb.CodePointAt(0));
            assertEquals('\uDC00', sb.CodePointAt(1));

            sb = StringBuilderFactory();
            sb.Append("abc");
            try
            {
                sb.CodePointAt(-1);
                fail("No IOOBE on negative index.");
            }
            catch (ArgumentOutOfRangeException) // IndexOutOfBoundsException
            {

            }

            try
            {
                sb.CodePointAt(sb.Length);
                fail("No IOOBE on index equal to length.");
            }
            catch (ArgumentOutOfRangeException) // IndexOutOfBoundsException
            {

            }

            try
            {
                sb.CodePointAt(sb.Length + 1);
                fail("No IOOBE on index greater than length.");
            }
            catch (ArgumentOutOfRangeException) // IndexOutOfBoundsException
            {

            }
        }

        /**
         * @tests java.lang.StringBuilder.CodePointBefore(int)
         */
        [Test]
        public void Test_codePointBeforeI()
        {
            TextBuilder sb = StringBuilderFactory("abc");
            assertEquals('a', sb.CodePointBefore(1));
            assertEquals('b', sb.CodePointBefore(2));
            assertEquals('c', sb.CodePointBefore(3));

            sb = StringBuilderFactory("\uD800\uDC00");
            assertEquals(0x10000, sb.CodePointBefore(2));
            assertEquals('\uD800', sb.CodePointBefore(1));

            sb = StringBuilderFactory();
            sb.Append("abc");

            try
            {
                sb.CodePointBefore(0);
                fail("No IOOBE on zero index.");
            }
            catch (ArgumentOutOfRangeException) // IndexOutOfBoundsException
            {

            }

            try
            {
                sb.CodePointBefore(-1);
                fail("No IOOBE on negative index.");
            }
            catch (ArgumentOutOfRangeException) // IndexOutOfBoundsException
            {

            }

            try
            {
                sb.CodePointBefore(sb.Length + 1);
                fail("No IOOBE on index greater than length.");
            }
            catch (ArgumentOutOfRangeException) // IndexOutOfBoundsException
            {

            }
        }

        /**
         * @tests java.lang.StringBuilder.CodePointCount(int, int)
         */
        [Test]
        public void Test_codePointCountII()
        {
            assertEquals(1, StringBuilderFactory("\uD800\uDC00").CodePointCount(0, 2 - 0));
            assertEquals(1, StringBuilderFactory("\uD800\uDC01").CodePointCount(0, 2 - 0));
            assertEquals(1, StringBuilderFactory("\uD801\uDC01").CodePointCount(0, 2 - 0));
            assertEquals(1, StringBuilderFactory("\uDBFF\uDFFF").CodePointCount(0, 2 - 0));

            assertEquals(3, StringBuilderFactory("a\uD800\uDC00b").CodePointCount(0, 4 - 0));
            assertEquals(4, StringBuilderFactory("a\uD800\uDC00b\uD800").CodePointCount(0, 5 - 0));

            TextBuilder sb = StringBuilderFactory();
            sb.Append("abc");
            try
            {
                sb.CodePointCount(-1, 2 - -1);
                fail("No IOOBE for negative begin index.");
            }
            catch (ArgumentOutOfRangeException) // IndexOutOfBoundsException
            {

            }

            try
            {
                sb.CodePointCount(0, 4 - 0);
                fail("No IOOBE for end index that's too large.");
            }
            catch (ArgumentOutOfRangeException) // IndexOutOfBoundsException
            {

            }

            try
            {
                sb.CodePointCount(3, 2 - 3);
                fail("No IOOBE for begin index larger than end index (negative length).");
            }
            catch (ArgumentOutOfRangeException) // IndexOutOfBoundsException
            {

            }
        }

        /**
         * @tests java.lang.StringBuilder.Delete(int, int)
         */
        [Test]
        public void Test_deleteII()
        {
            const string fixture = "0123456789";
            TextBuilder sb = StringBuilderFactory(fixture);
            assertSame(sb, sb.Delete(0, 0 - 0));
            assertEquals(fixture, sb.ToString());
            assertSame(sb, sb.Delete(5, 5 - 5));
            assertEquals(fixture, sb.ToString());
            assertSame(sb, sb.Delete(0, 1 - 0));
            assertEquals("123456789", sb.ToString());
            assertEquals(9, sb.Length);
            assertSame(sb, sb.Delete(0, sb.Length - 0));
            assertEquals("", sb.ToString());
            assertEquals(0, sb.Length);

            sb = StringBuilderFactory(fixture);
            assertSame(sb, sb.Delete(0, 11 - 0));
            assertEquals("", sb.ToString());
            assertEquals(0, sb.Length - 0);

            try
            {
                StringBuilderFactory(fixture).Delete(-1, 2 - -1);
                fail("no SIOOBE, negative start");
            }
            catch (ArgumentOutOfRangeException) // StringIndexOutOfBoundsException
            {
                // Expected
            }

            try
            {
                StringBuilderFactory(fixture).Delete(11, 12 - 11);
                fail("no SIOOBE, start too far");
            }
            catch (ArgumentOutOfRangeException) // StringIndexOutOfBoundsException
            {
                // Expected
            }

            try
            {
                StringBuilderFactory(fixture).Delete(13, 12 - 13);
                fail("no SIOOBE, start larger than end (negative length)");
            }
            catch (ArgumentOutOfRangeException) // StringIndexOutOfBoundsException
            {
                // Expected
            }

            // HARMONY 6212
            sb = StringBuilderFactory();
            sb.Append("abcde");
            String str = sb.ToString();
            sb.Delete(0, sb.Length - 0);
            sb.Append("YY");
            assertEquals("abcde", str);
            assertEquals("YY", sb.ToString());
        }

        /**
         * @tests java.lang.StringBuilder.deleteCharAt(int)
         */
        [Test]
        public void Test_deleteCharAtI()
        {
            const string fixture = "0123456789";
            TextBuilder sb = StringBuilderFactory(fixture);
            assertSame(sb, sb.RemoveAt(0));
            assertEquals("123456789", sb.ToString());
            assertEquals(9, sb.Length);
            sb = StringBuilderFactory(fixture);
            assertSame(sb, sb.RemoveAt(5));
            assertEquals("012346789", sb.ToString());
            assertEquals(9, sb.Length);
            sb = StringBuilderFactory(fixture);
            assertSame(sb, sb.RemoveAt(9));
            assertEquals("012345678", sb.ToString());
            assertEquals(9, sb.Length);

            try
            {
                StringBuilderFactory(fixture).RemoveAt(-1);
                fail("no SIOOBE, negative index");
            }
            catch (ArgumentOutOfRangeException) // StringIndexOutOfBoundsException
            {
                // Expected
            }

            try
            {
                StringBuilderFactory(fixture).RemoveAt(fixture.Length);
                fail("no SIOOBE, index equals length");
            }
            catch (ArgumentOutOfRangeException) // StringIndexOutOfBoundsException
            {
                // Expected
            }

            try
            {
                StringBuilderFactory(fixture).RemoveAt(fixture.Length + 1);
                fail("no SIOOBE, index exceeds length");
            }
            catch (ArgumentOutOfRangeException) // StringIndexOutOfBoundsException
            {
                // Expected
            }
        }

        /**
         * @tests java.lang.StringBuilder.EnsureCapacity(int)'
         */
        [Test]
        public virtual void Test_ensureCapacityI()
        {
            TextBuilder sb = StringBuilderFactory(5);
            assertEquals(5, sb.Capacity);
            sb.EnsureCapacity(10);
            assertEquals(12, sb.Capacity);
            sb.EnsureCapacity(26);
            assertEquals(26, sb.Capacity);
            sb.EnsureCapacity(55);
            assertEquals(55, sb.Capacity);
        }

        /**
         * @tests java.lang.StringBuilder.getChars(int, int, char[], int)'
         */
        [Test]
        public void Test_getCharsII_CI()
        {
            const string fixture = "0123456789";
            TextBuilder sb = StringBuilderFactory(fixture);
            char[] dst = new char[10];
            //sb.getChars(0, 10, dst, 0);
            sb.CopyTo(0, dst, 0, 10 - 0);
            assertTrue(Arrays.Equals(fixture.ToCharArray(), dst));

            dst.Fill('\0');
            //sb.getChars(0, 5, dst, 0);
            sb.CopyTo(0, dst, 0, 5);
            char[] fixtureChars = new char[10];
            //fixture.getChars(0, 5, fixtureChars, 0);
            fixture.CopyTo(0, fixtureChars, 0, 5 - 0);
            assertTrue(Arrays.Equals(fixtureChars, dst));

            dst.Fill('\0');
            fixtureChars.Fill('\0');
            //sb.getChars(0, 5, dst, 5);
            sb.CopyTo(0, dst, 5, 5);
            //fixture.getChars(0, 5, fixtureChars, 5);
            fixture.CopyTo(0, fixtureChars, 5, 5 - 0);
            assertTrue(Arrays.Equals(fixtureChars, dst));

            dst.Fill('\0');
            fixtureChars.Fill('\0');
            //sb.getChars(5, 10, dst, 1);
            sb.CopyTo(5, dst, 1, 10 - 5);
            //fixture.getChars(5, 10, fixtureChars, 1);
            fixture.CopyTo(5, fixtureChars, 1, 10 - 5);
            assertTrue(Arrays.Equals(fixtureChars, dst));

            try
            {
                //sb.getChars(0, 10, null, 0);
                sb.CopyTo(0, null, 0, 10 - 0);
                fail("no NPE");
            }
            catch (ArgumentNullException) // NullPointerException
            {
                // Expected
            }

            try
            {
                //sb.getChars(-1, 10, dst, 0);
                sb.CopyTo(-1, dst, 0, 10 - -1);
                fail("no IOOBE, srcBegin negative");
            }
            catch (ArgumentOutOfRangeException) // IndexOutOfBoundsException
            {
                // Expected
            }

            try
            {
                //sb.getChars(0, 10, dst, -1);
                sb.CopyTo(0, dst, -1, 10 - -1);
                fail("no IOOBE, dstBegin negative");
            }
            catch (ArgumentOutOfRangeException) // IndexOutOfBoundsException
            {
                // Expected
            }

            try
            {
                //sb.getChars(5, 4, dst, 0);
                sb.CopyTo(5, dst, 0, 4 - 5);
                fail("no IOOBE, srcBegin > srcEnd");
            }
            catch (ArgumentOutOfRangeException) // IndexOutOfBoundsException
            {
                // Expected
            }

            try
            {
                //sb.getChars(0, 11, dst, 0);
                sb.CopyTo(0, dst, 0, 11 - 0);
                fail("no IOOBE, srcEnd > length");
            }
            catch (ArgumentException) // IndexOutOfBoundsException
            {
                // Expected
            }

            try
            {
                //sb.getChars(0, 10, dst, 5);
                sb.CopyTo(0, dst, 5, 10 - 0);
                fail("no IOOBE, dstBegin and src size too large for what's left in dst");
            }
            catch (ArgumentException) // IndexOutOfBoundsException
            {
                // Expected
            }
        }

        /**
         * @tests java.lang.StringBuilder.IndexOf(String)
         */
        [Test]
        public void Test_indexOfLjava_lang_String()
        {
            const string fixture = "0123456789";
            TextBuilder sb = StringBuilderFactory(fixture);
            assertEquals(0, sb.IndexOf("0", StringComparison.Ordinal));
            assertEquals(0, sb.IndexOf("012", StringComparison.Ordinal));
            assertEquals(-1, sb.IndexOf("02", StringComparison.Ordinal));
            assertEquals(8, sb.IndexOf("89", StringComparison.Ordinal));

            try
            {
                sb.IndexOf((string)null, StringComparison.Ordinal);
                fail("no NPE");
            }
            catch (ArgumentNullException) // NullPointerException
            {
                // Expected
            }
        }

        /**
         * @tests java.lang.StringBuilder.IndexOf(String, int)
         */
        [Test]
        public void Test_IndexOfStringInt()
        {
            const string fixture = "0123456789";
            TextBuilder sb = StringBuilderFactory(fixture);
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
                sb.IndexOf((string)null, 0, StringComparison.Ordinal);
                fail("no NPE");
            }
            catch (ArgumentNullException) // NullPointerException
            {
                // Expected
            }
        }

        /**
         * @tests java.lang.StringBuilder.Insert(int, boolean)
         */
        [Test]
        public void Test_insertIZ()
        {
            const string fixture = "0000";
            TextBuilder sb = StringBuilderFactory(fixture);
            assertSame(sb, sb.Insert(0, true));
            assertEquals("true0000", sb.ToString());
            assertEquals(8, sb.Length);

            sb = StringBuilderFactory(fixture);
            assertSame(sb, sb.Insert(0, false));
            assertEquals("false0000", sb.ToString());
            assertEquals(9, sb.Length);

            sb = StringBuilderFactory(fixture);
            assertSame(sb, sb.Insert(2, false));
            assertEquals("00false00", sb.ToString());
            assertEquals(9, sb.Length);

            sb = StringBuilderFactory(fixture);
            assertSame(sb, sb.Insert(4, false));
            assertEquals("0000false", sb.ToString());
            assertEquals(9, sb.Length);

            try
            {
                sb = StringBuilderFactory(fixture);
                sb.Insert(-1, false);
                fail("no SIOOBE, negative index");
            }
            catch (ArgumentOutOfRangeException) // StringIndexOutOfBoundsException
            {
                // Expected
            }

            try
            {
                sb = StringBuilderFactory(fixture);
                sb.Insert(5, false);
                fail("no SIOOBE, index too large index");
            }
            catch (ArgumentOutOfRangeException) // StringIndexOutOfBoundsException
            {
                // Expected
            }
        }

        /**
         * @tests java.lang.StringBuilder.Insert(int, char)
         */
        [Test]
        public void Test_insertIC()
        {
            const string fixture = "0000";
            TextBuilder sb = StringBuilderFactory(fixture);
            assertSame(sb, sb.Insert(0, 'a'));
            assertEquals("a0000", sb.ToString());
            assertEquals(5, sb.Length);

            sb = StringBuilderFactory(fixture);
            assertSame(sb, sb.Insert(0, 'b'));
            assertEquals("b0000", sb.ToString());
            assertEquals(5, sb.Length);

            sb = StringBuilderFactory(fixture);
            assertSame(sb, sb.Insert(2, 'b'));
            assertEquals("00b00", sb.ToString());
            assertEquals(5, sb.Length);

            sb = StringBuilderFactory(fixture);
            assertSame(sb, sb.Insert(4, 'b'));
            assertEquals("0000b", sb.ToString());
            assertEquals(5, sb.Length);

            // FIXME this fails on Sun JRE 5.0_5
            //		try {
            //			sb = new StringBuilder(fixture);
            //			sb.Insert(-1, 'a');
            //			fail("no SIOOBE, negative index");
            //		} catch (StringIndexOutOfBoundsException e) {
            //			// Expected
            //		}

            /*
             * FIXME This fails on Sun JRE 5.0_5, but that seems like a bug, since
             * the 'insert(int, char[]) behaves this way.
             */
            //		try {
            //			sb = new StringBuilder(fixture);
            //			sb.Insert(5, 'a');
            //			fail("no SIOOBE, index too large index");
            //		} catch (StringIndexOutOfBoundsException e) {
            //			// Expected
            //		}
        }

        /**
         * @tests java.lang.StringBuilder.Insert(int, char)
         */
        [Test]
        public void Test_insertIC_2()
        {
            TextBuilder obj = StringBuilderFactory();
            try
            {
                obj.Insert(-1, '?');
                fail("ArrayIndexOutOfBoundsException expected");
            }
            catch (ArgumentOutOfRangeException) // ArrayIndexOutOfBoundsException
            {
                // expected
            }
        }

        /**
         * @tests java.lang.StringBuilder.Insert(int, char[])'
         */
        [Test]
        public void Test_insertI_C()
        {
            const string fixture = "0000";
            TextBuilder sb = StringBuilderFactory(fixture);
            assertSame(sb, sb.Insert(0, new char[] { 'a', 'b' }));
            assertEquals("ab0000", sb.ToString());
            assertEquals(6, sb.Length);

            sb = StringBuilderFactory(fixture);
            assertSame(sb, sb.Insert(2, new char[] { 'a', 'b' }));
            assertEquals("00ab00", sb.ToString());
            assertEquals(6, sb.Length);

            sb = StringBuilderFactory(fixture);
            assertSame(sb, sb.Insert(4, new char[] { 'a', 'b' }));
            assertEquals("0000ab", sb.ToString());
            assertEquals(6, sb.Length);

            /*
             * TODO This NPE is the behavior on Sun's JRE 5.0_5, but it's
             * undocumented. The assumption is that this method behaves like
             * String.valueOf(char[]), which does throw a NPE too, but that is also
             * undocumented.
             */

            // J2N: Changed behavior of null to a no-op to match .NET
            sb = StringBuilderFactory(fixture);
            assertSame(sb, sb.Insert(0, (char[])null));
            assertEquals("0000", sb.ToString());

            //try
            //{
            //    sb.Insert(0, (char[])null);
            //    fail("no NPE");
            //}
            //catch (ArgumentNullException) // NullPointerException
            //{
            //    // Expected
            //}

            try
            {
                sb = StringBuilderFactory(fixture);
                sb.Insert(-1, new char[] { 'a', 'b' });
                fail("no SIOOBE, negative index");
            }
            catch (ArgumentOutOfRangeException) // StringIndexOutOfBoundsException
            {
                // Expected
            }

            try
            {
                sb = StringBuilderFactory(fixture);
                sb.Insert(5, new char[] { 'a', 'b' });
                fail("no SIOOBE, index too large index");
            }
            catch (ArgumentOutOfRangeException) // StringIndexOutOfBoundsException
            {
                // Expected
            }
        }

        /**
         * @tests java.lang.StringBuilder.Insert(int, char[], int, int)
         */
        [Test]
        public void Test_insertI_CII()
        {
            const string fixture = "0000";
            TextBuilder sb = StringBuilderFactory(fixture);
            assertSame(sb, sb.Insert(0, new char[] { 'a', 'b' }, 0, 2));
            assertEquals("ab0000", sb.ToString());
            assertEquals(6, sb.Length);

            sb = StringBuilderFactory(fixture);
            assertSame(sb, sb.Insert(0, new char[] { 'a', 'b' }, 0, 1));
            assertEquals("a0000", sb.ToString());
            assertEquals(5, sb.Length);

            sb = StringBuilderFactory(fixture);
            assertSame(sb, sb.Insert(2, new char[] { 'a', 'b' }, 0, 2));
            assertEquals("00ab00", sb.ToString());
            assertEquals(6, sb.Length);

            sb = StringBuilderFactory(fixture);
            assertSame(sb, sb.Insert(2, new char[] { 'a', 'b' }, 0, 1));
            assertEquals("00a00", sb.ToString());
            assertEquals(5, sb.Length);

            sb = StringBuilderFactory(fixture);
            assertSame(sb, sb.Insert(4, new char[] { 'a', 'b' }, 0, 2));
            assertEquals("0000ab", sb.ToString());
            assertEquals(6, sb.Length);

            sb = StringBuilderFactory(fixture);
            assertSame(sb, sb.Insert(4, new char[] { 'a', 'b' }, 0, 1));
            assertEquals("0000a", sb.ToString());
            assertEquals(5, sb.Length);

            /*
             * TODO This NPE is the behavior on Sun's JRE 5.0_5, but it's
             * undocumented. The assumption is that this method behaves like
             * String.valueOf(char[]), which does throw a NPE too, but that is also
             * undocumented.
             */

            try
            {
                sb.Insert(0, (char[])null, 0, 2);
                fail("no NPE");
            }
            catch (ArgumentNullException) // NullPointerException
            {
                // Expected
            }

            try
            {
                sb = StringBuilderFactory(fixture);
                sb.Insert(-1, new char[] { 'a', 'b' }, 0, 2);
                fail("no SIOOBE, negative index");
            }
            catch (ArgumentOutOfRangeException) // StringIndexOutOfBoundsException
            {
                // Expected
            }

            try
            {
                sb = StringBuilderFactory(fixture);
                sb.Insert(5, new char[] { 'a', 'b' }, 0, 2);
                fail("no SIOOBE, index too large index");
            }
            catch (ArgumentOutOfRangeException) // StringIndexOutOfBoundsException
            {
                // Expected
            }

            try
            {
                sb = StringBuilderFactory(fixture);
                sb.Insert(5, new char[] { 'a', 'b' }, -1, 2);
                fail("no SIOOBE, negative offset");
            }
            catch (ArgumentOutOfRangeException) // StringIndexOutOfBoundsException
            {
                // Expected
            }

            try
            {
                sb = StringBuilderFactory(fixture);
                sb.Insert(5, new char[] { 'a', 'b' }, 0, -1);
                fail("no SIOOBE, negative length");
            }
            catch (ArgumentOutOfRangeException) // StringIndexOutOfBoundsException
            {
                // Expected
            }

            try
            {
                sb = StringBuilderFactory(fixture);
                sb.Insert(5, new char[] { 'a', 'b' }, 0, 3);
                fail("no SIOOBE, too long");
            }
            catch (ArgumentOutOfRangeException) // StringIndexOutOfBoundsException
            {
                // Expected
            }
        }

        /**
         * @tests java.lang.StringBuilder.Insert(int, CharSequence)
         */
        [Test]
        public void Test_insertILjava_lang_CharSequence()
        {
            const string fixture = "0000";
            TextBuilder sb = StringBuilderFactory(fixture);
            assertSame(sb, sb.Insert(0, (ICharSequence)"ab".AsCharSequence()));
            assertEquals("ab0000", sb.ToString());
            assertEquals(6, sb.Length);

            sb = StringBuilderFactory(fixture);
            assertSame(sb, sb.Insert(2, (ICharSequence)"ab".AsCharSequence()));
            assertEquals("00ab00", sb.ToString());
            assertEquals(6, sb.Length);

            sb = StringBuilderFactory(fixture);
            assertSame(sb, sb.Insert(4, (ICharSequence)"ab".AsCharSequence()));
            assertEquals("0000ab", sb.ToString());
            assertEquals(6, sb.Length);

            sb = StringBuilderFactory(fixture);
            assertSame(sb, sb.Insert(4, (ICharSequence)null));
            //assertEquals("0000null", sb.ToString());
            //assertEquals(8, sb.Length);
            assertEquals("0000", sb.ToString()); // J2N: Changed behavior to make adding null a no-op to match .NET
            assertEquals(4, sb.Length);

            try
            {
                sb = StringBuilderFactory(fixture);
                sb.Insert(-1, (ICharSequence)"ab".AsCharSequence());
                fail("no IOOBE, negative index");
            }
            catch (ArgumentOutOfRangeException) // IndexOutOfBoundsException
            {
                // Expected
            }

            try
            {
                sb = StringBuilderFactory(fixture);
                sb.Insert(5, (ICharSequence)"ab".AsCharSequence());
                fail("no IOOBE, index too large index");
            }
            catch (ArgumentOutOfRangeException) // IndexOutOfBoundsException
            {
                // Expected
            }
        }

        /**
         * @tests java.lang.StringBuilder.Insert(int, CharSequence, int, int)
         */
        //@SuppressWarnings("cast")
        [Test]
        public void Test_insertILjava_lang_CharSequenceII()
        {
            const string fixture = "0000";
            TextBuilder sb = StringBuilderFactory(fixture);
            assertSame(sb, sb.Insert(0, (ICharSequence)"ab".AsCharSequence(), 0, 2 - 0));
            assertEquals("ab0000", sb.ToString());
            assertEquals(6, sb.Length);

            sb = StringBuilderFactory(fixture);
            assertSame(sb, sb.Insert(0, (ICharSequence)"ab".AsCharSequence(), 0, 1 - 0));
            assertEquals("a0000", sb.ToString());
            assertEquals(5, sb.Length);

            sb = StringBuilderFactory(fixture);
            assertSame(sb, sb.Insert(2, (ICharSequence)"ab".AsCharSequence(), 0, 2 - 0));
            assertEquals("00ab00", sb.ToString());
            assertEquals(6, sb.Length);

            sb = StringBuilderFactory(fixture);
            assertSame(sb, sb.Insert(2, (ICharSequence)"ab".AsCharSequence(), 0, 1 - 0));
            assertEquals("00a00", sb.ToString());
            assertEquals(5, sb.Length);

            sb = StringBuilderFactory(fixture);
            assertSame(sb, sb.Insert(4, (ICharSequence)"ab".AsCharSequence(), 0, 2 - 0));
            assertEquals("0000ab", sb.ToString());
            assertEquals(6, sb.Length);

            sb = StringBuilderFactory(fixture);
            assertSame(sb, sb.Insert(4, (ICharSequence)"ab".AsCharSequence(), 0, 1 - 0));
            assertEquals("0000a", sb.ToString());
            assertEquals(5, sb.Length);

            // J2N: Changed behavior to throw on null when either startIndex or count is non-zero to match .NET
            //sb = OpenStringBuilderFactory(fixture);
            //assertSame(sb, sb.Insert(4, (ICharSequence)null, 0, 2 - 0));
            //assertEquals("0000nu", sb.ToString());
            //assertEquals(6, sb.Length);

            try
            {
                sb = StringBuilderFactory(fixture);
                sb.Insert(4, (ICharSequence)null, 0, 2 - 0);
                fail("no ArgumentNullException when length is set");
            }
            catch (ArgumentNullException)
            {
                // Expected
            }

            try
            {
                sb = StringBuilderFactory(fixture);
                sb.Insert(4, (ICharSequence)null, 2, 0 - 2);
                fail("no ArgumentNullException when startIndex is set");
            }
            catch (ArgumentNullException)
            {
                // Expected
            }

            try
            {
                sb = StringBuilderFactory(fixture);
                sb.Insert(-1, (ICharSequence)"ab".AsCharSequence(), 0, 2 - 0);
                fail("no IOOBE, negative index");
            }
            catch (ArgumentOutOfRangeException) // IndexOutOfBoundsException
            {
                // Expected
            }

            try
            {
                sb = StringBuilderFactory(fixture);
                sb.Insert(5, (ICharSequence)"ab".AsCharSequence(), 0, 2 - 0);
                fail("no IOOBE, index too large index");
            }
            catch (ArgumentOutOfRangeException) // IndexOutOfBoundsException
            {
                // Expected
            }

            try
            {
                sb = StringBuilderFactory(fixture);
                sb.Insert(5, (ICharSequence)"ab".AsCharSequence(), -1, 2 - -1);
                fail("no IOOBE, negative offset");
            }
            catch (ArgumentOutOfRangeException) // IndexOutOfBoundsException
            {
                // Expected
            }

            try
            {
                sb = StringBuilderFactory(fixture);
                sb.Insert(5, new char[] { 'a', 'b' }, 0, -1 - 0);
                fail("no IOOBE, negative length");
            }
            catch (ArgumentOutOfRangeException) // IndexOutOfBoundsException
            {
                // Expected
            }

            try
            {
                sb = StringBuilderFactory(fixture);
                sb.Insert(5, new char[] { 'a', 'b' }, 0, 3 - 0);
                fail("no IOOBE, too long");
            }
            catch (ArgumentOutOfRangeException) // IndexOutOfBoundsException
            {
                // Expected
            }
        }

        /**
         * @tests java.lang.StringBuilder.Insert(int, double)
         */
        [Test]
        public void Test_insertID()
        {
            const string fixture = "0000";
            TextBuilder sb = StringBuilderFactory(fixture);
            assertSame(sb, sb.Insert(0, -1D));
            assertEquals("-1.00000", sb.ToString());
            assertEquals(8, sb.Length);

            sb = StringBuilderFactory(fixture);
            assertSame(sb, sb.Insert(0, 0D));
            assertEquals("0.00000", sb.ToString());
            assertEquals(7, sb.Length);

            sb = StringBuilderFactory(fixture);
            assertSame(sb, sb.Insert(2, 1D));
            assertEquals("001.000", sb.ToString());
            assertEquals(7, sb.Length);

            sb = StringBuilderFactory(fixture);
            assertSame(sb, sb.Insert(4, 2D));
            assertEquals("00002.0", sb.ToString());
            assertEquals(7, sb.Length);

            try
            {
                sb = StringBuilderFactory(fixture);
                sb.Insert(-1, 1D);
                fail("no IOOBE, negative index");
            }
            catch (ArgumentOutOfRangeException) // IndexOutOfBoundsException
            {
                // Expected
            }

            try
            {
                sb = StringBuilderFactory(fixture);
                sb.Insert(5, 1D);
                fail("no IOOBE, index too large index");
            }
            catch (ArgumentOutOfRangeException) // IndexOutOfBoundsException
            {
                // Expected
            }
        }

        /**
         * @tests java.lang.StringBuilder.Insert(int, float)
         */
        [Test]
        public void Test_insertIF()
        {
            const string fixture = "0000";
            TextBuilder sb = StringBuilderFactory(fixture);
            assertSame(sb, sb.Insert(0, -1F));
            assertEquals("-1.00000", sb.ToString());
            assertEquals(8, sb.Length);

            sb = StringBuilderFactory(fixture);
            assertSame(sb, sb.Insert(0, 0F));
            assertEquals("0.00000", sb.ToString());
            assertEquals(7, sb.Length);

            sb = StringBuilderFactory(fixture);
            assertSame(sb, sb.Insert(2, 1F));
            assertEquals("001.000", sb.ToString());
            assertEquals(7, sb.Length);

            sb = StringBuilderFactory(fixture);
            assertSame(sb, sb.Insert(4, 2F));
            assertEquals("00002.0", sb.ToString());
            assertEquals(7, sb.Length);

            try
            {
                sb = StringBuilderFactory(fixture);
                sb.Insert(-1, 1F);
                fail("no IOOBE, negative index");
            }
            catch (ArgumentOutOfRangeException) // IndexOutOfBoundsException
            {
                // Expected
            }

            try
            {
                sb = StringBuilderFactory(fixture);
                sb.Insert(5, 1F);
                fail("no IOOBE, index too large index");
            }
            catch (ArgumentOutOfRangeException) // IndexOutOfBoundsException
            {
                // Expected
            }
        }

        /**
         * @tests java.lang.StringBuilder.Insert(int, int)
         */
        [Test]
        public void Test_insertII()
        {
            const string fixture = "0000";
            TextBuilder sb = StringBuilderFactory(fixture);
            assertSame(sb, sb.Insert(0, -1));
            assertEquals("-10000", sb.ToString());
            assertEquals(6, sb.Length);

            sb = StringBuilderFactory(fixture);
            assertSame(sb, sb.Insert(0, 0));
            assertEquals("00000", sb.ToString());
            assertEquals(5, sb.Length);

            sb = StringBuilderFactory(fixture);
            assertSame(sb, sb.Insert(2, 1));
            assertEquals("00100", sb.ToString());
            assertEquals(5, sb.Length);

            sb = StringBuilderFactory(fixture);
            assertSame(sb, sb.Insert(4, 2));
            assertEquals("00002", sb.ToString());
            assertEquals(5, sb.Length);

            try
            {
                sb = StringBuilderFactory(fixture);
                sb.Insert(-1, 1);
                fail("no IOOBE, negative index");
            }
            catch (ArgumentOutOfRangeException) // IndexOutOfBoundsException
            {
                // Expected
            }

            try
            {
                sb = StringBuilderFactory(fixture);
                sb.Insert(5, 1);
                fail("no IOOBE, index too large index");
            }
            catch (ArgumentOutOfRangeException) // IndexOutOfBoundsException
            {
                // Expected
            }
        }

        /**
         * @tests java.lang.StringBuilder.Insert(int, long)
         */
        [Test]
        public void Test_insertIJ()
        {
            const string fixture = "0000";
            TextBuilder sb = StringBuilderFactory(fixture);
            assertSame(sb, sb.Insert(0, -1L));
            assertEquals("-10000", sb.ToString());
            assertEquals(6, sb.Length);

            sb = StringBuilderFactory(fixture);
            assertSame(sb, sb.Insert(0, 0L));
            assertEquals("00000", sb.ToString());
            assertEquals(5, sb.Length);

            sb = StringBuilderFactory(fixture);
            assertSame(sb, sb.Insert(2, 1L));
            assertEquals("00100", sb.ToString());
            assertEquals(5, sb.Length);

            sb = StringBuilderFactory(fixture);
            assertSame(sb, sb.Insert(4, 2L));
            assertEquals("00002", sb.ToString());
            assertEquals(5, sb.Length);

            try
            {
                sb = StringBuilderFactory(fixture);
                sb.Insert(-1, 1L);
                fail("no IOOBE, negative index");
            }
            catch (ArgumentOutOfRangeException) // IndexOutOfBoundsException
            {
                // Expected
            }

            try
            {
                sb = StringBuilderFactory(fixture);
                sb.Insert(5, 1L);
                fail("no IOOBE, index too large index");
            }
            catch (ArgumentOutOfRangeException) // IndexOutOfBoundsException
            {
                // Expected
            }
        }

        /**
         * @tests java.lang.StringBuilder.Insert(int, Object)
         */
        [Test]
        public void Test_insertILjava_lang_Object()
        {
            const string fixture = "0000";
            TextBuilder sb = StringBuilderFactory(fixture);
            assertSame(sb, sb.Insert(0, Fixture.INSTANCE));
            assertEquals("fixture0000", sb.ToString());
            assertEquals(11, sb.Length);

            sb = StringBuilderFactory(fixture);
            assertSame(sb, sb.Insert(2, Fixture.INSTANCE));
            assertEquals("00fixture00", sb.ToString());
            assertEquals(11, sb.Length);

            sb = StringBuilderFactory(fixture);
            assertSame(sb, sb.Insert(4, Fixture.INSTANCE));
            assertEquals("0000fixture", sb.ToString());
            assertEquals(11, sb.Length);

            sb = StringBuilderFactory(fixture);
            assertSame(sb, sb.Insert(4, (Object)null));
            //assertEquals("0000null", sb.ToString());
            //assertEquals(8, sb.Length);
            assertEquals("0000", sb.ToString()); // J2N: Changed behavior to make adding null a no-op to match .NET
            assertEquals(4, sb.Length);

            try
            {
                sb = StringBuilderFactory(fixture);
                sb.Insert(-1, Fixture.INSTANCE);
                fail("no IOOBE, negative index");
            }
            catch (ArgumentOutOfRangeException) // IndexOutOfBoundsException
            {
                // Expected
            }

            try
            {
                sb = StringBuilderFactory(fixture);
                sb.Insert(5, Fixture.INSTANCE);
                fail("no IOOBE, index too large index");
            }
            catch (ArgumentOutOfRangeException) // IndexOutOfBoundsException
            {
                // Expected
            }
        }

        /**
         * @tests java.lang.StringBuilder.Insert(int, String)
         */
        [Test]
        public void Test_insertILjava_lang_String()
        {
            const string fixture = "0000";
            TextBuilder sb = StringBuilderFactory(fixture);
            assertSame(sb, sb.Insert(0, "fixture"));
            assertEquals("fixture0000", sb.ToString());
            assertEquals(11, sb.Length);

            sb = StringBuilderFactory(fixture);
            assertSame(sb, sb.Insert(2, "fixture"));
            assertEquals("00fixture00", sb.ToString());
            assertEquals(11, sb.Length);

            sb = StringBuilderFactory(fixture);
            assertSame(sb, sb.Insert(4, "fixture"));
            assertEquals("0000fixture", sb.ToString());
            assertEquals(11, sb.Length);

            sb = StringBuilderFactory(fixture);
            assertSame(sb, sb.Insert(4, (Object)null));
            //assertEquals("0000null", sb.ToString());
            //assertEquals(8, sb.Length);
            assertEquals("0000", sb.ToString()); // J2N: Changed behavior to make adding null a no-op to match .NET
            assertEquals(4, sb.Length);

            try
            {
                sb = StringBuilderFactory(fixture);
                sb.Insert(-1, "fixture");
                fail("no IOOBE, negative index");
            }
            catch (ArgumentOutOfRangeException) // IndexOutOfBoundsException
            {
                // Expected
            }

            try
            {
                sb = StringBuilderFactory(fixture);
                sb.Insert(5, "fixture");
                fail("no IOOBE, index too large index");
            }
            catch (ArgumentOutOfRangeException) // IndexOutOfBoundsException
            {
                // Expected
            }
        }

        /**
         * @tests java.lang.StringBuilder.LastIndexOf(String)
         */
        [Test]
        public void Test_lastIndexOfLjava_lang_String()
        {
            const string fixture = "0123456789";
            TextBuilder sb = StringBuilderFactory(fixture);
            assertEquals(0, sb.LastIndexOf("0", StringComparison.Ordinal));
            assertEquals(0, sb.LastIndexOf("012", StringComparison.Ordinal));
            assertEquals(-1, sb.LastIndexOf("02", StringComparison.Ordinal));
            assertEquals(8, sb.LastIndexOf("89", StringComparison.Ordinal));

            try
            {
                sb.LastIndexOf((string)null, StringComparison.Ordinal);
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
        public void Test_lastIndexOfLjava_lang_StringI()
        {
            const string fixture = "0123456789";
            TextBuilder sb = StringBuilderFactory(fixture);
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
                sb.LastIndexOf((string)null, 0, StringComparison.Ordinal);
                fail("no NPE");
            }
            catch (ArgumentNullException) // NullPointerException
            {
                // Expected
            }
        }

        /**
         * @tests java.lang.StringBuilder.Length
         */
        [Test]
        public void Test_length()
        {
            TextBuilder sb = StringBuilderFactory();
            assertEquals(0, sb.Length);
            sb.Append("0000");
            assertEquals(4, sb.Length);
        }

        /**
         * @tests java.lang.StringBuilder.OffsetByCodePoints(int, int)'
         */
        [Test]
        public void Test_offsetByCodePointsII()
        {
            int result = StringBuilderFactory("a\uD800\uDC00b").OffsetByCodePoints(0, 2);
            assertEquals(3, result);

            result = StringBuilderFactory("abcd").OffsetByCodePoints(3, -1);
            assertEquals(2, result);

            result = StringBuilderFactory("a\uD800\uDC00b").OffsetByCodePoints(0, 3);
            assertEquals(4, result);

            result = StringBuilderFactory("a\uD800\uDC00b").OffsetByCodePoints(3, -1);
            assertEquals(1, result);

            result = StringBuilderFactory("a\uD800\uDC00b").OffsetByCodePoints(3, 0);
            assertEquals(3, result);

            result = StringBuilderFactory("\uD800\uDC00bc").OffsetByCodePoints(3, 0);
            assertEquals(3, result);

            result = StringBuilderFactory("a\uDC00bc").OffsetByCodePoints(3, -1);
            assertEquals(2, result);

            result = StringBuilderFactory("a\uD800bc").OffsetByCodePoints(3, -1);
            assertEquals(2, result);

            TextBuilder sb = StringBuilderFactory();
            sb.Append("abc");
            try
            {
                sb.OffsetByCodePoints(-1, 1);
                fail("No IOOBE for negative index.");
            }
            catch (ArgumentOutOfRangeException) // IndexOutOfBoundsException
            {

            }

            try
            {
                sb.OffsetByCodePoints(0, 4);
                fail("No IOOBE for offset that's too large.");
            }
            catch (ArgumentOutOfRangeException) // IndexOutOfBoundsException
            {

            }

            try
            {
                sb.OffsetByCodePoints(3, -4);
                fail("No IOOBE for offset that's too small.");
            }
            catch (ArgumentOutOfRangeException) // IndexOutOfBoundsException
            {

            }

            try
            {
                sb.OffsetByCodePoints(3, 1);
                fail("No IOOBE for index that's too large.");
            }
            catch (ArgumentOutOfRangeException) // IndexOutOfBoundsException
            {

            }

            try
            {
                sb.OffsetByCodePoints(4, -1);
                fail("No IOOBE for index that's too large.");
            }
            catch (ArgumentOutOfRangeException) // IndexOutOfBoundsException
            {

            }
        }

        /**
         * @tests java.lang.StringBuilder.Replace(int, int, String)'
         */
        [Test]
        public void Test_replaceIILjava_lang_String()
        {
            const string fixture = "0000";
            TextBuilder sb = StringBuilderFactory(fixture);
            assertSame(sb, sb.Replace(1, 3 - 1, "11")); // J2N: Corrected 2nd parameter
            assertEquals("0110", sb.ToString());
            assertEquals(4, sb.Length);

            sb = StringBuilderFactory(fixture);
            assertSame(sb, sb.Replace(1, 2 - 1, "11")); // J2N: Corrected 2nd parameter
            assertEquals("01100", sb.ToString());
            assertEquals(5, sb.Length);

            sb = StringBuilderFactory(fixture);
            assertSame(sb, sb.Replace(4, 5 - 4, "11")); // J2N: Corrected 2nd parameter
            assertEquals("000011", sb.ToString());
            assertEquals(6, sb.Length);

            sb = StringBuilderFactory(fixture);
            assertSame(sb, sb.Replace(4, 6 - 4, "11")); // J2N: Corrected 2nd parameter
            assertEquals("000011", sb.ToString());
            assertEquals(6, sb.Length);

            // FIXME Undocumented NPE in Sun's JRE 5.0_5
            try
            {
                sb.Replace(1, 2 - 1, (string)null); // J2N: Corrected 2nd parameter
                fail("No NPE");
            }
            catch (ArgumentNullException) // NullPointerException
            {
                // Expected
            }

            try
            {
                sb = StringBuilderFactory(fixture);
                sb.Replace(-1, 2 - -1, "11"); // J2N: Corrected 2nd parameter
                fail("No SIOOBE, negative start");
            }
            catch (ArgumentOutOfRangeException) // StringIndexOutOfBoundsException
            {
                // Expected
            }

            try
            {
                sb = StringBuilderFactory(fixture);
                sb.Replace(5, 2 - 5, "11"); // J2N: Corrected 2nd parameter
                fail("No SIOOBE, start > length");
            }
            catch (ArgumentOutOfRangeException) // StringIndexOutOfBoundsException
            {
                // Expected
            }

            try
            {
                sb = StringBuilderFactory(fixture);
                sb.Replace(3, 2 - 3, "11"); // J2N: Corrected 2nd parameter
                fail("No SIOOBE, start > end");
            }
            catch (ArgumentOutOfRangeException) // StringIndexOutOfBoundsException
            {
                // Expected
            }

            // Regression for HARMONY-348
            TextBuilder buffer = StringBuilderFactory("1234567");
            buffer.Replace(2, 6 - 2, "XXX"); // J2N: Corrected 2nd parameter
            assertEquals("12XXX7", buffer.ToString());
        }

        /**
         * @tests java.lang.StringBuilder.Replace(int, int, String)'
         */
        [Test]
        public void Test_replaceIILjava_lang_ReadOnlySpan()
        {
            const string fixture = "0000";
            TextBuilder sb = StringBuilderFactory(fixture);
            assertSame(sb, sb.Replace(1, 3 - 1, "11".AsSpan())); // J2N: Corrected 2nd parameter
            assertEquals("0110", sb.ToString());
            assertEquals(4, sb.Length);

            sb = StringBuilderFactory(fixture);
            assertSame(sb, sb.Replace(1, 2 - 1, "11".AsSpan())); // J2N: Corrected 2nd parameter
            assertEquals("01100", sb.ToString());
            assertEquals(5, sb.Length);

            sb = StringBuilderFactory(fixture);
            assertSame(sb, sb.Replace(4, 5 - 4, "11".AsSpan())); // J2N: Corrected 2nd parameter
            assertEquals("000011", sb.ToString());
            assertEquals(6, sb.Length);

            sb = StringBuilderFactory(fixture);
            assertSame(sb, sb.Replace(4, 6 - 4, "11".AsSpan())); // J2N: Corrected 2nd parameter
            assertEquals("000011", sb.ToString());
            assertEquals(6, sb.Length);

            // J2N: null converts to an empty span, so no exception is thrown

            //// FIXME Undocumented NPE in Sun's JRE 5.0_5
            //try
            //{
            //    sb.Replace(1, 2 - 1, (string)null); // J2N: Corrected 2nd parameter
            //    fail("No NPE");
            //}
            //catch (ArgumentNullException) // NullPointerException
            //{
            //    // Expected
            //}

            try
            {
                sb = StringBuilderFactory(fixture);
                sb.Replace(-1, 2 - -1, "11".AsSpan()); // J2N: Corrected 2nd parameter
                fail("No SIOOBE, negative start");
            }
            catch (ArgumentOutOfRangeException) // StringIndexOutOfBoundsException
            {
                // Expected
            }

            try
            {
                sb = StringBuilderFactory(fixture);
                sb.Replace(5, 2 - 5, "11".AsSpan()); // J2N: Corrected 2nd parameter
                fail("No SIOOBE, start > length");
            }
            catch (ArgumentOutOfRangeException) // StringIndexOutOfBoundsException
            {
                // Expected
            }

            try
            {
                sb = StringBuilderFactory(fixture);
                sb.Replace(3, 2 - 3, "11".AsSpan()); // J2N: Corrected 2nd parameter
                fail("No SIOOBE, start > end");
            }
            catch (ArgumentOutOfRangeException) // StringIndexOutOfBoundsException
            {
                // Expected
            }

            // Regression for HARMONY-348
            TextBuilder buffer = StringBuilderFactory("1234567");
            buffer.Replace(2, 6 - 2, "XXX".AsSpan()); // J2N: Corrected 2nd parameter
            assertEquals("12XXX7", buffer.ToString());
        }

        private void reverseTest(String org, String rev, String back)
        {
            // create non-shared StringBuilder
            TextBuilder sb = StringBuilderFactory(org);
            sb.Reverse();
            String reversed = sb.ToString();
            assertEquals(rev, reversed);
            // create non-shared StringBuilder
            sb = StringBuilderFactory(reversed);
            sb.Reverse();
            reversed = sb.ToString();
            assertEquals(back, reversed);

            // test algorithm when StringBuilder is shared
            sb = StringBuilderFactory(org);
            String copy = sb.ToString();
            assertEquals(org, copy);
            sb.Reverse();
            reversed = sb.ToString();
            assertEquals(rev, reversed);
            sb = StringBuilderFactory(reversed);
            copy = sb.ToString();
            assertEquals(rev, copy);
            sb.Reverse();
            reversed = sb.ToString();
            assertEquals(back, reversed);
        }

        /**
         * @tests java.lang.StringBuilder.Reverse()
         */
        [Test]
        public void Test_reverse()
        {
            const string fixture = "0123456789";
            TextBuilder sb = StringBuilderFactory(fixture);
            assertSame(sb, sb.Reverse());
            assertEquals("9876543210", sb.ToString());

            sb = StringBuilderFactory("012345678");
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
         * @tests java.lang.StringBuilder.setCharAt(int, char)
         */
        [Test]
        public void Test_setCharAtIC()
        {
            const string fixture = "0000";
            TextBuilder sb = StringBuilderFactory(fixture);
            sb[0] = 'A';
            assertEquals("A000", sb.ToString());
            sb[1] = 'B';
            assertEquals("AB00", sb.ToString());
            sb[2] = 'C';
            assertEquals("ABC0", sb.ToString());
            sb[3] = 'D';
            assertEquals("ABCD", sb.ToString());

            try
            {
                sb[-1] = 'A';
                fail("No IOOBE, negative index");
            }
            catch (ArgumentOutOfRangeException) //IndexOutOfBoundsException
            {
                // Expected
            }

            try
            {
                sb[4] = 'A';
                fail("No IOOBE, index == length");
            }
            catch (ArgumentOutOfRangeException) //IndexOutOfBoundsException
            {
                // Expected
            }

            try
            {
                sb[5] = 'A';
                fail("No IOOBE, index > length");
            }
            catch (ArgumentOutOfRangeException) //IndexOutOfBoundsException
            {
                // Expected
            }
        }

        /**
         * @tests java.lang.StringBuilder.setLength(int)'
         */
        [Test]
        public void Test_setLengthI()
        {
            const string fixture = "0123456789";
            TextBuilder sb = StringBuilderFactory(fixture);
            sb.Length = (5);
            assertEquals(5, sb.Length);
            assertEquals("01234", sb.ToString());
            sb.Length = (6);
            assertEquals(6, sb.Length);
            assertEquals("01234\0", sb.ToString());
            sb.Length = (0);
            assertEquals(0, sb.Length);
            assertEquals("", sb.ToString());

            try
            {
                sb.Length = (-1);
                fail("No IOOBE, negative length.");
            }
            catch (ArgumentOutOfRangeException) //IndexOutOfBoundsException
            {
                // Expected
            }

            sb = StringBuilderFactory("abcde");
            assertEquals("abcde", sb.ToString());
            sb.Length = (1);
            sb.Append('g');
            assertEquals("ag", sb.ToString());

            sb = StringBuilderFactory("abcde");
            sb.Length = (3);
            sb.Append('g');
            assertEquals("abcg", sb.ToString());

            sb = StringBuilderFactory("abcde");
            sb.Length = (2);
            try
            {
                _ = sb[3];
                fail("should throw IndexOutOfBoundsException");
            }
            catch (IndexOutOfRangeException) //IndexOutOfBoundsException
            {
                // Expected
            }

            sb = StringBuilderFactory();
            sb.Append("abcdefg");
            sb.Length = (2);
            sb.Length = (5);
            for (int i = 2; i < 5; i++)
            {
                assertEquals(0, sb[i]);
            }

            sb = StringBuilderFactory();
            sb.Append("abcdefg");
            sb.Delete(2, 4 - 2); // J2N: Corrected 2nd parameter
            sb.Length = (7);
            assertEquals('a', sb[0]);
            assertEquals('b', sb[1]);
            assertEquals('e', sb[2]);
            assertEquals('f', sb[3]);
            assertEquals('g', sb[4]);
            for (int i = 5; i < 7; i++)
            {
                assertEquals(0, sb[i]);
            }

            sb = StringBuilderFactory();
            sb.Append("abcdefg");
            sb.Replace(2, 5 - 2, "z"); // J2N: Corrected 2nd parameter
            sb.Length = (7);
            for (int i = 5; i < 7; i++)
            {
                assertEquals(0, sb[i]);
            }
        }

        /**
         * @tests java.lang.StringBuilder.subSequence(int, int)
         */
        [Test]
        public void Test_subSequenceII()
        {
            const string fixture = "0123456789";
            TextBuilder sb = StringBuilderFactory(fixture);
            ICharSequence ss = sb.Subsequence(0, 5);
            assertEquals("01234", ss.ToString());

            ss = sb.Subsequence(0, 0);
            assertEquals("", ss.ToString());

            try
            {
                sb.Subsequence(-1, 1 - -1);
                fail("No IOOBE, negative start.");
            }
            catch (ArgumentOutOfRangeException) //IndexOutOfBoundsException
            {
                // Expected
            }

            try
            {
                sb.Subsequence(0, -1 - 0);
                fail("No IOOBE, negative end.");
            }
            catch (ArgumentOutOfRangeException) //IndexOutOfBoundsException
            {
                // Expected
            }

            try
            {
                sb.Subsequence(0, fixture.Length + 1 - 0);
                fail("No IOOBE, end > length.");
            }
            catch (ArgumentOutOfRangeException) //IndexOutOfBoundsException
            {
                // Expected
            }

            try
            {
                sb.Subsequence(3, 2 - 3);
                fail("No IOOBE, start > end.");
            }
            catch (ArgumentOutOfRangeException) //IndexOutOfBoundsException
            {
                // Expected
            }
        }

        /**
         * @tests java.lang.StringBuilder.substring(int)
         */
        [Test]
        public void Test_substringI()
        {
            const string fixture = "0123456789";
            TextBuilder sb = StringBuilderFactory(fixture);
            String ss = sb.ToString(0);
            assertEquals(fixture, ss);

            ss = sb.ToString(10);
            assertEquals("", ss);

            try
            {
                sb.ToString(-1);
                fail("No SIOOBE, negative start.");
            }
            catch (ArgumentOutOfRangeException) // StringIndexOutOfBoundsException
            {
                // Expected
            }

            try
            {
                sb.ToString(0, -1);
                fail("No SIOOBE, negative end.");
            }
            catch (ArgumentOutOfRangeException) // StringIndexOutOfBoundsException
            {
                // Expected
            }

            try
            {
                sb.ToString(fixture.Length + 1);
                fail("No SIOOBE, start > length.");
            }
            catch (ArgumentOutOfRangeException) // StringIndexOutOfBoundsException
            {
                // Expected
            }
        }

        /**
         * @tests java.lang.StringBuilder.substring(int, int)
         */
        [Test]
        public void Test_substringII()
        {
            const string fixture = "0123456789";
            TextBuilder sb = StringBuilderFactory(fixture);
            String ss = sb.ToString(0, 5 - 0);
            assertEquals("01234", ss);

            ss = sb.ToString(0, 0 - 0);
            assertEquals("", ss);

            try
            {
                sb.ToString(-1, 1 - -1);
                fail("No SIOOBE, negative start.");
            }
            catch (ArgumentOutOfRangeException) // StringIndexOutOfBoundsException
            {
                // Expected
            }

            try
            {
                sb.ToString(0, -1 - 0);
                fail("No SIOOBE, negative end.");
            }
            catch (ArgumentOutOfRangeException) // StringIndexOutOfBoundsException
            {
                // Expected
            }

            try
            {
                sb.ToString(0, fixture.Length + 1 - 0);
                fail("No SIOOBE, end > length.");
            }
            catch (ArgumentOutOfRangeException) // StringIndexOutOfBoundsException
            {
                // Expected
            }

            try
            {
                sb.ToString(3, 2 - 3);
                fail("No SIOOBE, start > end.");
            }
            catch (ArgumentOutOfRangeException) // StringIndexOutOfBoundsException
            {
                // Expected
            }
        }

        /**
         * @tests java.lang.StringBuilder.ToString()'
         */
        [Test]
        public void Test_toString()
        {
            const string fixture = "0123456789";
            TextBuilder sb = StringBuilderFactory(fixture);
            assertEquals(fixture, sb.ToString());

            sb.Length = (0);
            sb.Append("abcde");
            assertEquals("abcde", sb.ToString());
            sb.Length = (1000);
            byte[] bytes = sb.ToString().getBytes("GB18030");
            for (int i = 5; i < bytes.Length; i++)
            {
                assertEquals(0, bytes[i]);
            }

            sb.Length = (5);
            sb.Append("fghij");
            assertEquals("abcdefghij", sb.ToString());
        }

        /**
         * @tests java.lang.StringBuilder.trimToSize()'
         */
        [Test]
        public void Test_trimToSize()
        {
            const string fixture = "0123456789";
            TextBuilder sb = StringBuilderFactory(fixture);
            assertTrue(sb.Capacity > fixture.Length);
            assertEquals(fixture.Length, sb.Length);
            assertEquals(fixture, sb.ToString());
            int prevCapacity = sb.Capacity;
            sb.TrimExcess();
            assertTrue(prevCapacity > sb.Capacity);
            assertEquals(fixture.Length, sb.Length);
            assertEquals(fixture, sb.ToString());
        }

        //// comparator for StringBuilder objects
        //private static readonly SerializableAssert STRING_BILDER_COMPARATOR = new SerializableAssert()
        //{
        //        public void assertDeserialized(Serializable initial,
        //                Serializable deserialized)
        //{

        //    StringBuilder init = (StringBuilder)initial;
        //    StringBuilder desr = (StringBuilder)deserialized;

        //    assertEquals("toString", init.ToString(), desr.ToString());
        //}
        //    };

        ///**
        // * @tests serialization/deserialization.
        // */
        //public void testSerializationSelf() throws Exception
        //{

        //    SerializationTest.verifySelf(new StringBuilder("0123456789"),
        //                STRING_BILDER_COMPARATOR);
        //    }

        //    /**
        //     * @tests serialization/deserialization compatibility with RI.
        //     */
        //    public void testSerializationCompatibility() throws Exception
        //{

        //    SerializationTest.verifyGolden(this, new StringBuilder("0123456789"),
        //                STRING_BILDER_COMPARATOR);
        //    }

        private sealed class Fixture
        {
            internal static readonly Fixture INSTANCE = new Fixture();

            public override string ToString()
            {
                return "fixture";
            }
        }
    }
}
