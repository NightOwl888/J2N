using J2N.Text;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace J2N.IO
{
    /// <summary>
    /// Tests java.nio.CharBuffer
    /// </summary>
    public class TestCharBuffer : AbstractBufferTest
    {
        protected const int SMALL_TEST_LENGTH = 5;

        protected const int BUFFER_LENGTH = 20;

        protected CharBuffer buf;

        private static char[] chars = "123456789a".ToCharArray();

        public override void SetUp()
        {

            char[] charscopy = new char[chars.Length];
            System.Array.Copy(chars, 0, charscopy, 0, chars.Length);
            buf = CharBuffer.Wrap(charscopy);
            baseBuf = buf;
        }

        public override void TearDown()
        {
            buf = null;
            baseBuf = null;
        }

        // *** Added for .NET ***

        [Test]
        public virtual void TestWrap()
        {
            string s = "A test string to test with.";
            char[] ca = s.ToCharArray();
            StringBuilder sb = new StringBuilder(s);
            ICharSequence cs = s.AsCharSequence();

            // Control - char[] was part of the original Java implementation
            // and used length rather than end index. So we will check that our conversion
            // of string, StringBuilder, and ICharSequence limit is correct using char[] as a baseline.


            // String
            Assert.AreEqual(CharBuffer.Wrap(ca).Limit, CharBuffer.Wrap(s).Limit);
            Assert.AreEqual(CharBuffer.Wrap(ca, 6, 10).Limit, CharBuffer.Wrap(s, 6, 10).Limit);

            // StringBuilder
            Assert.AreEqual(CharBuffer.Wrap(ca).Limit, CharBuffer.Wrap(sb).Limit);
            Assert.AreEqual(CharBuffer.Wrap(ca, 6, 10).Limit, CharBuffer.Wrap(sb, 6, 10).Limit);

            // ICharSequence
            Assert.AreEqual(CharBuffer.Wrap(ca).Limit, CharBuffer.Wrap(cs).Limit);
            Assert.AreEqual(CharBuffer.Wrap(ca, 6, 10).Limit, CharBuffer.Wrap(cs, 6, 10).Limit);
        }


        // *** End Added for .NET ***


        [Test]
        public virtual void TestArray()
        {
            char[] array = buf.Array;
            assertContentEquals(buf, array, buf.ArrayOffset, buf.Capacity);

            loadTestData1(array, buf.ArrayOffset, buf.Capacity);
            assertContentEquals(buf, array, buf.ArrayOffset, buf.Capacity);

            loadTestData2(array, buf.ArrayOffset, buf.Capacity);
            assertContentEquals(buf, array, buf.ArrayOffset, buf.Capacity);

            loadTestData1(buf);
            assertContentEquals(buf, array, buf.ArrayOffset, buf.Capacity);

            loadTestData2(buf);
            assertContentEquals(buf, array, buf.ArrayOffset, buf.Capacity);
        }

        [Test]
        public virtual void TestArrayOffset()
        {
            char[] array = buf.Array;
            assertContentEquals(buf, array, buf.ArrayOffset, buf.Capacity);

            loadTestData1(array, buf.ArrayOffset, buf.Capacity);
            assertContentEquals(buf, array, buf.ArrayOffset, buf.Capacity);

            loadTestData2(array, buf.ArrayOffset, buf.Capacity);
            assertContentEquals(buf, array, buf.ArrayOffset, buf.Capacity);

            loadTestData1(buf);
            assertContentEquals(buf, array, buf.ArrayOffset, buf.Capacity);

            loadTestData2(buf);
            assertContentEquals(buf, array, buf.ArrayOffset, buf.Capacity);
        }

        [Test]
        public virtual void TestAsReadOnlyBuffer()
        {
            buf.Clear();
            buf.Mark();
            buf.Position = (buf.Limit);

            // readonly's contents should be the same as buf
            CharBuffer @readonly = buf.AsReadOnlyBuffer();
            assertNotSame(buf, @readonly);
            assertTrue(@readonly.IsReadOnly);
            assertEquals(buf.Position, @readonly.Position);
            assertEquals(buf.Limit, @readonly.Limit);
            //assertEquals(buf.IsDirect, @readonly.IsDirect); // J2N: IsDirect not supported
            assertEquals(buf.Order, @readonly.Order);
            assertEquals(buf.Capacity, @readonly.Capacity);
            assertContentEquals(buf, @readonly);

            // readonly's position, mark, and limit should be independent to buf
            @readonly.Reset();
            assertEquals(@readonly.Position, 0);
            @readonly.Clear();
            assertEquals(buf.Position, buf.Limit);
            buf.Reset();
            assertEquals(buf.Position, 0);

            buf.Clear();
            int originalPosition = (buf.Position + buf.Limit) / 2;
            buf.Position = (originalPosition);
            buf.Mark();
            buf.Position = (buf.Limit);

            // readonly's contents should be the same as buf
            @readonly = buf.AsReadOnlyBuffer();
            assertNotSame(buf, @readonly);
            assertTrue(@readonly.IsReadOnly);
            assertEquals(buf.Position, @readonly.Position);
            assertEquals(buf.Limit, @readonly.Limit);
            //assertEquals(buf.IsDirect, @readonly.IsDirect); // J2N: IsDirect not supported
            assertEquals(buf.Order, @readonly.Order);
            assertEquals(buf.Capacity, @readonly.Capacity);
            assertContentEquals(buf, @readonly);

            // readonly's position, mark, and limit should be independent to buf
            @readonly.Reset();
            assertEquals(@readonly.Position, originalPosition);
            @readonly.Clear();
            assertEquals(buf.Position, buf.Limit);
            buf.Reset();
            assertEquals(buf.Position, originalPosition);
        }

        [Test]
        public virtual void TestCompact()
        {
            // case: buffer is full
            buf.Clear();
            buf.Mark();
            loadTestData1(buf);
            CharBuffer ret = buf.Compact();
            assertSame(ret, buf);
            assertEquals(buf.Position, buf.Capacity);
            assertEquals(buf.Limit, buf.Capacity);
            assertContentLikeTestData1(buf, 0, (char)0, buf.Capacity);
            try
            {
                buf.Reset();
                fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (InvalidMarkException e)
            {
                // expected
            }

            // case: buffer is empty
            buf.Position = (0);
            buf.Limit = (0);
            buf.Mark();
            ret = buf.Compact();
            assertSame(ret, buf);
            assertEquals(buf.Position, 0);
            assertEquals(buf.Limit, buf.Capacity);
            assertContentLikeTestData1(buf, 0, (char)0, buf.Capacity);
            try
            {
                buf.Reset();
                fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (InvalidMarkException e)
            {
                // expected
            }

            // case: normal
            assertTrue(buf.Capacity > 5);
            buf.Position = (1);
            buf.Limit = (5);
            buf.Mark();
            ret = buf.Compact();
            assertSame(ret, buf);
            assertEquals(buf.Position, 4);
            assertEquals(buf.Limit, buf.Capacity);
            assertContentLikeTestData1(buf, 0, (char)1, 4);
            try
            {
                buf.Reset();
                fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (InvalidMarkException e)
            {
                // expected
            }
        }

        [Test]
        public virtual void TestCompareTo()
        {
            // compare to self
            assertEquals(0, buf.CompareTo(buf));

            assertTrue(buf.Capacity > SMALL_TEST_LENGTH);
            buf.Clear();
            CharBuffer other = CharBuffer.Allocate(buf.Capacity);
            other.Put(buf);
            other.Clear();
            buf.Clear();
            assertEquals(0, buf.CompareTo(other));
            assertEquals(0, other.CompareTo(buf));
            buf.Position = (1);
            assertTrue(buf.CompareTo(other) > 0);
            assertTrue(other.CompareTo(buf) < 0);
            other.Position = (2);
            assertTrue(buf.CompareTo(other) < 0);
            assertTrue(other.CompareTo(buf) > 0);
            buf.Position = (2);
            assertTrue(buf.CompareTo(other) == 0);
            assertTrue(other.CompareTo(buf) == 0);
            other.Limit = (SMALL_TEST_LENGTH);
            assertTrue(buf.CompareTo(other) > 0);
            assertTrue(other.CompareTo(buf) < 0);

            // J2N: Cover null for .NET. See: https://stackoverflow.com/a/4852537
            assertEquals(1, buf.CompareTo(null));
        }

        [Test]
        public virtual void TestDuplicate()
        {
            // mark the position 0
            buf.Clear();
            buf.Mark();
            buf.Position = (buf.Limit);

            // duplicate's contents should be the same as buf
            CharBuffer duplicate = buf.Duplicate();
            assertNotSame(buf, duplicate);
            assertEquals(buf.Position, duplicate.Position);
            assertEquals(buf.Limit, duplicate.Limit);
            assertEquals(buf.IsReadOnly, duplicate.IsReadOnly);
            //assertEquals(buf.IsDirect, duplicate.IsDirect); // J2N: IsDirect not supported
            assertEquals(buf.Order, duplicate.Order);
            assertEquals(buf.Capacity, duplicate.Capacity);
            assertContentEquals(buf, duplicate);

            // duplicate's position, mark, and limit should be independent to
            // buf
            duplicate.Reset();
            assertEquals(duplicate.Position, 0);
            duplicate.Clear();
            assertEquals(buf.Position, buf.Limit);
            buf.Reset();
            assertEquals(buf.Position, 0);

            // mark another position
            buf.Clear();
            int originalPosition = (buf.Position + buf.Limit) / 2;
            buf.Position = (originalPosition);
            buf.Mark();
            buf.Position = (buf.Limit);

            // duplicate's contents should be the same as buf
            duplicate = buf.Duplicate();
            assertNotSame(buf, duplicate);
            assertEquals(buf.Position, duplicate.Position);
            assertEquals(buf.Limit, duplicate.Limit);
            assertEquals(buf.IsReadOnly, duplicate.IsReadOnly);
            //assertEquals(buf.IsDirect, duplicate.IsDirect); // J2N: IsDirect not supported
            assertEquals(buf.Order, duplicate.Order);
            assertEquals(buf.Capacity, duplicate.Capacity);
            assertContentEquals(buf, duplicate);

            // duplicate's position, mark, and limit should be independent to
            // buf
            duplicate.Reset();
            assertEquals(duplicate.Position, originalPosition);
            duplicate.Clear();
            assertEquals(buf.Position, buf.Limit);
            buf.Reset();
            assertEquals(buf.Position, originalPosition);

            // duplicate share the same content with buf
            if (!duplicate.IsReadOnly)
            {
                loadTestData1(buf);
                assertContentEquals(buf, duplicate);
                loadTestData2(duplicate);
                assertContentEquals(buf, duplicate);
            }
        }

        [Test]
        public virtual void TestEquals()
        {
            // equal to self
            assertTrue(buf.Equals(buf));
            CharBuffer @readonly = buf.AsReadOnlyBuffer();
            assertTrue(buf.Equals(@readonly));
            CharBuffer duplicate = buf.Duplicate();
            assertTrue(buf.Equals(duplicate));

            // always false, if type mismatch
            assertFalse(buf.Equals(true));

            assertTrue(buf.Capacity > 5);

            buf.SetLimit(buf.Capacity).SetPosition(0);
            @readonly.SetLimit(@readonly.Capacity).SetPosition(1);
            assertFalse(buf.Equals(@readonly));

            buf.SetLimit(buf.Capacity - 1).SetPosition(0);
            duplicate.SetLimit(duplicate.Capacity).SetPosition(0);
            assertFalse(buf.Equals(duplicate));
        }

        /*
         * Class under test for char get()
         */
        [Test]
        public virtual void TestGet()
        {
            buf.Clear();
            for (int i = 0; i < buf.Capacity; i++)
            {
                assertEquals(buf.Position, i);
                assertEquals(buf.Get(), buf.Get(i));
            }
            try
            {
                buf.Get();
                fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (BufferUnderflowException e)
            {
                // expected
            }
        }

        /*
         * Class under test for java.nio.CharBuffer get(char[])
         */
        [Test]
        public virtual void TestGetcharArray()
        {
            char[] array = new char[1];
            buf.Clear();
            for (int i = 0; i < buf.Capacity; i++)
            {
                assertEquals(buf.Position, i);
                CharBuffer ret = buf.Get(array);
                assertEquals(array[0], buf.Get(i));
                assertSame(ret, buf);
            }
            try
            {
                buf.Get(array);
                fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (BufferUnderflowException e)
            {
                // expected
            }
        }

        /*
         * Class under test for java.nio.CharBuffer get(char[], int, int)
         */
        [Test]
        public virtual void TestGetcharArrayintint()
        {
            buf.Clear();
            char[] array = new char[buf.Capacity];

            try
            {
                buf.Get(new char[buf.Capacity + 1], 0, buf.Capacity + 1);
                fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (BufferUnderflowException e)
            {
                // expected
            }
            assertEquals(buf.Position, 0);
            try
            {
                buf.Get(array, -1, array.Length);
                fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (ArgumentOutOfRangeException e)
            {
                // expected
            }
            buf.Get(array, array.Length, 0);
            try
            {
                buf.Get(array, array.Length + 1, 1);
                fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (ArgumentOutOfRangeException e)
            {
                // expected
            }
            assertEquals(buf.Position, 0);
            try
            {
                buf.Get(array, 2, -1);
                fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (ArgumentOutOfRangeException e)
            {
                // expected
            }
            try
            {
                buf.Get((char[])null, 2, -1);
                fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (ArgumentNullException e)
            {
                // expected
            }
            try
            {
                buf.Get(array, 2, array.Length);
                fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (ArgumentOutOfRangeException e)
            {
                // expected
            }
            try
            {
                buf.Get(array, 1, int.MaxValue);
                fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (ArgumentOutOfRangeException e)
            {
                // expected
            }
            try
            {
                buf.Get(array, int.MaxValue, 1);
                fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (ArgumentOutOfRangeException e)
            {
                // expected
            }
            assertEquals(buf.Position, 0);

            buf.Clear();
            CharBuffer ret = buf.Get(array, 0, array.Length);
            assertEquals(buf.Position, buf.Capacity);
            assertContentEquals(buf, array, 0, array.Length);
            assertSame(ret, buf);
        }

        /*
         * Class under test for char get(int)
         */
        [Test]
        public virtual void TestGetint()
        {
            buf.Clear();
            for (int i = 0; i < buf.Capacity; i++)
            {
                assertEquals(buf.Position, i);
                assertEquals(buf.Get(), buf.Get(i));
            }
            try
            {
                buf.Get(-1);
                fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (ArgumentOutOfRangeException e)
            {
                // expected
            }
            try
            {
                buf.Get(buf.Limit);
                fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (ArgumentOutOfRangeException e)
            {
                // expected
            }
        }

        [Test]
        public virtual void TestHashCode()
        {
            buf.Clear();
            loadTestData1(buf);
            CharBuffer @readonly = buf.AsReadOnlyBuffer();
            CharBuffer duplicate = buf.Duplicate();
            assertTrue(buf.GetHashCode() == @readonly.GetHashCode());
            assertTrue(buf.Capacity > SMALL_TEST_LENGTH);
            duplicate.Position = (buf.Capacity / 2);
            assertTrue(buf.GetHashCode() != duplicate.GetHashCode());
        }

        /*
         * Class under test for java.nio.CharBuffer put(char)
         */
        [Test]
        public virtual void TestPutchar()
        {
            buf.Clear();
            for (int i = 0; i < buf.Capacity; i++)
            {
                assertEquals(buf.Position, i);
                CharBuffer ret = buf.Put((char)i);
                assertEquals(buf.Get(i), (char)i);
                assertSame(ret, buf);
            }
            try
            {
                buf.Put((char)0);
                fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (BufferOverflowException e)
            {
                // expected
            }
        }

        /*
         * Class under test for java.nio.CharBuffer put(char[])
         */
        [Test]
        public virtual void TestPutcharArray()
        {
            char[] array = new char[1];

            buf.Clear();
            for (int i = 0; i < buf.Capacity; i++)
            {
                assertEquals(buf.Position, i);
                array[0] = (char)i;
                CharBuffer ret = buf.Put(array);
                assertEquals(buf.Get(i), (char)i);
                assertSame(ret, buf);
            }
            try
            {
                buf.Put(array);
                fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (BufferOverflowException e)
            {
                // expected
            }
            try
            {
                buf.Put((char[])null);
                fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (ArgumentNullException e)
            {
                // expected
            }
        }

        /*
         * Class under test for java.nio.CharBuffer put(char[], int, int)
         */
        [Test]
        public virtual void TestPutcharArrayintint()
        {
            buf.Clear();
            char[] array = new char[buf.Capacity];
            try
            {
                buf.Put((char[])null, 0, 1);
                fail("Should throw NullPointerException"); //$NON-NLS-1$
            }
            catch (ArgumentNullException e)
            {
                // expected
            }
            try
            {
                buf.Put(new char[buf.Capacity + 1], 0, buf.Capacity + 1);
                fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (BufferOverflowException e)
            {
                // expected
            }
            assertEquals(buf.Position, 0);
            try
            {
                buf.Put(array, -1, array.Length);
                fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (ArgumentOutOfRangeException e)
            {
                // expected
            }
            try
            {
                buf.Put(array, array.Length + 1, 0);
                fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (ArgumentOutOfRangeException e)
            {
                // expected
            }
            buf.Put(array, array.Length, 0);
            assertEquals(buf.Position, 0);
            try
            {
                buf.Put(array, 0, -1);
                fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (ArgumentOutOfRangeException e)
            {
                // expected
            }
            try
            {
                buf.Put((char[])null, 0, -1);
                fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (ArgumentNullException e)
            {
                // expected
            }
            try
            {
                buf.Put(array, 2, array.Length);
                fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (ArgumentOutOfRangeException e)
            {
                // expected
            }
            try
            {
                buf.Put(array, int.MaxValue, 1);
                fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (ArgumentOutOfRangeException e)
            {
                // expected
            }
            try
            {
                buf.Put(array, 1, int.MaxValue);
                fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (ArgumentOutOfRangeException e)
            {
                // expected
            }
            assertEquals(buf.Position, 0);

            loadTestData2(array, 0, array.Length);
            CharBuffer ret = buf.Put(array, 0, array.Length);
            assertEquals(buf.Position, buf.Capacity);
            assertContentEquals(buf, array, 0, array.Length);
            assertSame(ret, buf);
        }

        /*
         * Class under test for java.nio.CharBuffer put(java.nio.CharBuffer)
         */
        [Test]
        public virtual void TestPutCharBuffer()
        {
            CharBuffer other = CharBuffer.Allocate(buf.Capacity);

            try
            {
                buf.Put((CharBuffer)null);
                fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (ArgumentNullException e)
            {
                // expected
            }
            try
            {
                buf.Put(buf);
                fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (ArgumentException e)
            {
                // expected
            }
            try
            {
                buf.Put(CharBuffer.Allocate(buf.Capacity + 1));
                fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (BufferOverflowException e)
            {
                // expected
            }
            try
            {
                buf.Flip();
                buf.Put((CharBuffer)null);
                fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (ArgumentNullException e)
            {
                // expected
            }

            loadTestData2(other);
            other.Clear();
            buf.Clear();
            CharBuffer ret = buf.Put(other);
            assertEquals(other.Position, other.Capacity);
            assertEquals(buf.Position, buf.Capacity);
            assertContentEquals(other, buf);
            assertSame(ret, buf);
        }

        /*
         * Class under test for java.nio.CharBuffer put(int, char)
         */
        [Test]
        public virtual void TestPutintchar()
        {
            buf.Clear();
            for (int i = 0; i < buf.Capacity; i++)
            {
                assertEquals(buf.Position, 0);
                CharBuffer ret = buf.Put(i, (char)i);
                assertEquals(buf.Get(i), (char)i);
                assertSame(ret, buf);
            }
            try
            {
                buf.Put(-1, (char)0);
                fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (ArgumentOutOfRangeException e)
            {
                // expected
            }
            try
            {
                buf.Put(buf.Limit, (char)0);
                fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (ArgumentOutOfRangeException e)
            {
                // expected
            }
        }

        [Test]
        public virtual void TestSlice()
        {
            assertTrue(buf.Capacity > 5);
            buf.Position = (1);
            buf.Limit = (buf.Capacity - 1);

            CharBuffer slice = buf.Slice();
            assertEquals(buf.IsReadOnly, slice.IsReadOnly);
            //assertEquals(buf.IsDirect, slice.IsDirect); // J2N: IsDirect not supported
            assertEquals(buf.Order, slice.Order);
            assertEquals(slice.Position, 0);
            assertEquals(slice.Limit, buf.Remaining);
            assertEquals(slice.Capacity, buf.Remaining);
            try
            {
                slice.Reset();
                fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (InvalidMarkException e)
            {
                // expected
            }

            // slice share the same content with buf
            if (!slice.IsReadOnly)
            {
                loadTestData1(slice);
                assertContentLikeTestData1(buf, 1, (char)0, slice.Capacity);
                buf.Put(2, (char)500);
                assertEquals(slice.Get(1), 500);
            }
        }

        [Test]
        public virtual void TestToString()
        {
            String expected = "";
            for (int i = buf.Position; i < buf.Limit; i++)
            {
                expected += buf.Get(i);
            }
            String str = buf.ToString();
            assertEquals(expected, str);
        }

        [Test]
        public virtual void TestCharAt()
        {
            for (int i = 0; i < buf.Remaining; i++)
            {
                assertEquals(buf.Get(buf.Position + i), buf[i]);
            }
            try
            {
                var _ = buf[-1];
                fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (ArgumentOutOfRangeException e)
            {
                // expected
            }
            try
            {
                var _ = buf[buf.Remaining];
                fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (ArgumentOutOfRangeException e)
            {
                // expected
            }
        }

        [Test]
        public virtual void TestLength()
        {
            assertEquals(buf.Length, buf.Remaining);
        }

        [Test]
        public virtual void TestSubSequence()
        {
            try
            {
                buf.Subsequence(-1, buf.Length - -1); // J2N: end - start
                fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (ArgumentOutOfRangeException e)
            {
                // expected
            }
            try
            {
                buf.Subsequence(buf.Length + 1, (buf.Length + 1) - (buf.Length + 1)); // J2N: end - start
                fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (ArgumentOutOfRangeException e)
            {
                // expected
            }
            assertEquals(buf.Subsequence(buf.Length, buf.Length - buf.Length).Length, 0); // J2N: end - start
            try
            {
                buf.Subsequence(1, 0 - 1); // J2N: end - start
                fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (ArgumentOutOfRangeException e)
            {
                // expected
            }
            try
            {
                buf.Subsequence(1, (buf.Length + 1) - 1); // J2N: end - start
                fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (ArgumentOutOfRangeException e)
            {
                // expected
            }

            assertEquals(buf.Subsequence(0, buf.Length - 0).ToString(), buf // J2N: end - start
                    .ToString());

            if (buf.Length >= 2)
            {
                assertEquals(buf.Subsequence(1, (buf.Length - 1) - 1).ToString(), buf // J2N: end - start
                        .ToString().Substring(1, (buf.Length - 1) - 1)); // J2N: end - start
            }
        }

        [Test]
        public virtual void TestPutString()
        {
            String str = " ";

            buf.Clear();
            for (int i = 0; i < buf.Capacity; i++)
            {
                assertEquals(buf.Position, i);
                str = "" + (char)i;
                CharBuffer ret = buf.Put(str);
                assertEquals(buf.Get(i), (char)i);
                assertSame(ret, buf);
            }
            try
            {
                buf.Put(str);
                fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (BufferOverflowException e)
            {
                // expected
            }
            try
            {
                buf.Put((String)null);
                fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (ArgumentNullException e)
            {
                // expected
            }
        }

        [Test]
        public virtual void TestPutStringintint()
        {
            buf.Clear();
            String str = new string(new char[buf.Capacity]);

            // Throw a BufferOverflowException and no character is transfered to
            // CharBuffer
            try
            {
                buf.Put(new string(new char[buf.Capacity + 1]), 0, (buf.Capacity + 1) - 0); // J2N: end - start
                fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (BufferOverflowException e)
            {
                // expected
            }
            try
            {
                buf.Put((String)null, 0, (buf.Capacity + 1) - 0); // J2N: end - start
                fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (ArgumentNullException e)
            {
                // expected
            }
            assertEquals(0, buf.Position);

            buf.Clear();
            try
            {
                buf.Put(str, -1, str.Length - -1); // J2N: end - start
                fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (ArgumentOutOfRangeException e)
            {
                // expected
            }
            try
            {
                buf.Put(str, str.Length + 1, (str.Length + 2) - (str.Length + 1)); // J2N: end - start
                fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (ArgumentOutOfRangeException e)
            {
                // expected
            }
            try
            {
                buf.Put((String)null, -1, 0 - -1); // J2N: end - start
                fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (ArgumentNullException e)
            {
                // expected
            }
            buf.Put(str, str.Length, str.Length - str.Length); // J2N: end - start
            assertEquals(buf.Position, 0);
            try
            {
                buf.Put(str, 2, 1 - 2); // J2N: end - start
                fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (ArgumentOutOfRangeException e)
            {
                // expected
            }
            try
            {
                buf.Put(str, 2, (str.Length + 1) - 2); // J2N: end - start
                fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (ArgumentOutOfRangeException e)
            {
                // expected
            }
            assertEquals(buf.Position, 0);

            char[] array = new char[buf.Capacity];
            loadTestData2(array, 0, array.Length);
            str = new string(array);

            CharBuffer ret = buf.Put(str, 0, str.Length - 0); // J2N: end - start
            assertEquals(buf.Position, buf.Capacity);
            assertContentEquals(buf, str.ToCharArray(), 0, str.Length);
            assertSame(ret, buf);
        }

        internal void loadTestData1(char[] array, int offset, int length)
        {
            for (int i = 0; i < length; i++)
            {
                array[offset + i] = (char)i;
            }
        }

        internal void loadTestData2(char[] array, int offset, int length)
        {
            for (int i = 0; i < length; i++)
            {
                array[offset + i] = (char)(length - i);
            }
        }

        internal void loadTestData1(CharBuffer buf)
        {
            buf.Clear();
            for (int i = 0; i < buf.Capacity; i++)
            {
                buf.Put(i, (char)i);
            }
        }

        internal void loadTestData2(CharBuffer buf)
        {
            buf.Clear();
            for (int i = 0; i < buf.Capacity; i++)
            {
                buf.Put(i, (char)(buf.Capacity - i));
            }
        }

        private void assertContentEquals(CharBuffer buf, char[] array, int offset,
                int length)
        {
            for (int i = 0; i < length; i++)
            {
                assertEquals(buf.Get(i), array[offset + i]);
            }
        }

        private void assertContentEquals(CharBuffer buf, CharBuffer other)
        {
            assertEquals(buf.Capacity, other.Capacity);
            for (int i = 0; i < buf.Capacity; i++)
            {
                assertEquals(buf.Get(i), other.Get(i));
            }
        }

        private void assertContentLikeTestData1(CharBuffer buf, int startIndex,
                char startValue, int length)
        {
            char value = startValue;
            for (int i = 0; i < length; i++)
            {
                assertEquals(buf.Get(startIndex + i), value);
                value = (char)(value + 1);
            }
        }

        [Test]
        public virtual void TestAppendSelf()
        {
            CharBuffer cb = CharBuffer.Allocate(10);
            CharBuffer cb2 = cb.Duplicate();
            cb.Append(cb);
            assertEquals(10, cb.Position);
            cb.Clear();
            assertEquals(cb2, cb);

            cb.Put("abc");
            cb2 = cb.Duplicate();
            cb.Append(cb);
            assertEquals(10, cb.Position);
            cb.Clear();
            cb2.Clear();
            assertEquals(cb2, cb);

            cb.Put("edfg");
            cb.Clear();
            cb2 = cb.Duplicate();
            cb.Append(cb);
            assertEquals(10, cb.Position);
            cb.Clear();
            cb2.Clear();
            assertEquals(cb, cb2);
        }

        [Test]
        public virtual void TestAppendOverFlow()
        {
            CharBuffer cb = CharBuffer.Allocate(1);
            ICharSequence cs = "String".AsCharSequence();
            cb.Put('A');
            try
            {
                cb.Append('C');
                fail("should throw BufferOverflowException.");
            }
            catch (BufferOverflowException ex)
            {
                // expected;
            }
            try
            {
                cb.Append(cs);
                fail("should throw BufferOverflowException.");
            }
            catch (BufferOverflowException ex)
            {
                // expected;
            }
            try
            {
                cb.Append(cs, 1, 2 - 1); // J2N: end - start
                fail("should throw BufferOverflowException.");
            }
            catch (BufferOverflowException ex)
            {
                // expected;
            }
        }

        [Test]
        public virtual void TestReadOnlyMap()
        {
            CharBuffer cb = CharBuffer.Wrap("ABCDE").AsReadOnlyBuffer();
            ICharSequence cs = "String".AsCharSequence();
            try
            {
                cb.Append('A');
                fail("should throw ReadOnlyBufferException.");
            }
            catch (ReadOnlyBufferException ex)
            {
                // expected;
            }
            try
            {
                cb.Append(cs);
                fail("should throw ReadOnlyBufferException.");
            }
            catch (ReadOnlyBufferException ex)
            {
                // expected;
            }
            try
            {
                cb.Append(cs, 1, 2 - 1); // J2N: end - start
                fail("should throw ReadOnlyBufferException.");
            }
            catch (ReadOnlyBufferException ex)
            {
                // expected;
            }
            cb.Append(cs, 1, 1 - 1); // J2N: end - start
        }

        [Test]
        public virtual void TestAppendCNormal()
        {
            CharBuffer cb = CharBuffer.Allocate(2);
            cb.Put('A');
            assertSame(cb, cb.Append('B'));
            assertEquals('B', cb.Get(1));
        }

        // *** Added for .NET ***

        [Test]
        public virtual void TestAppendCharArrayNormal()
        {
            CharBuffer cb = CharBuffer.Allocate(10);
            cb.Put('A');
            assertSame(cb, cb.Append("String".ToCharArray()));
            assertEquals("AString", cb.Flip().ToString());
            cb.Append((char[])null);
            assertEquals("null", cb.Flip().ToString());
        }

        [Test]
        public virtual void TestAppendCharArrayIINormal()
        {
            CharBuffer cb = CharBuffer.Allocate(10);
            cb.Put('A');
            assertSame(cb, cb.Append("String".ToCharArray(), 1, 3 - 1)); // J2N: end - start
            assertEquals("Atr", cb.Flip().ToString());

            cb.Append((char[])null, 0, 1 - 0); // J2N: end - start
            assertEquals("n", cb.Flip().ToString());
        }

        [Test]
        public virtual void TestAppendCharArrayII_IllegalArgument()
        {
            CharBuffer cb = CharBuffer.Allocate(10);
            cb.Append("String".ToCharArray(), 0, 0 - 0); // J2N: end - start
            cb.Append("String".ToCharArray(), 2, 2 - 2); // J2N: end - start
            try
            {
                cb.Append("String".ToCharArray(), -1, 1 - -1); // J2N: end - start
                fail("should throw IndexOutOfBoundsException.");
            }
            catch (ArgumentOutOfRangeException ex)
            {
                // expected;
            }
            try
            {
                cb.Append("String".ToCharArray(), -1, -1 - -1); // J2N: end - start
                fail("should throw IndexOutOfBoundsException.");
            }
            catch (ArgumentOutOfRangeException ex)
            {
                // expected;
            }
            try
            {
                cb.Append("String".ToCharArray(), 3, 2 - 3); // J2N: end - start
                fail("should throw IndexOutOfBoundsException.");
            }
            catch (ArgumentOutOfRangeException ex)
            {
                // expected;
            }
            try
            {
                cb.Append("String".ToCharArray(), 3, 0 - 3); // J2N: end - start
                fail("should throw IndexOutOfBoundsException.");
            }
            catch (ArgumentOutOfRangeException ex)
            {
                // expected;
            }
            try
            {
                cb.Append("String".ToCharArray(), 3, 110 - 3); // J2N: end - start
                fail("should throw IndexOutOfBoundsException.");
            }
            catch (ArgumentOutOfRangeException ex)
            {
                // expected;
            }
        }

        [Test]
        public virtual void TestAppendStringBuilderNormal()
        {
            CharBuffer cb = CharBuffer.Allocate(10);
            cb.Put('A');
            assertSame(cb, cb.Append(new StringBuilder("String")));
            assertEquals("AString", cb.Flip().ToString());
            cb.Append((StringBuilder)null);
            assertEquals("null", cb.Flip().ToString());
        }

        [Test]
        public virtual void TestAppendStringBuilderIINormal()
        {
            CharBuffer cb = CharBuffer.Allocate(10);
            cb.Put('A');
            assertSame(cb, cb.Append(new StringBuilder("String"), 1, 3 - 1)); // J2N: end - start
            assertEquals("Atr", cb.Flip().ToString());

            cb.Append((StringBuilder)null, 0, 1 - 0); // J2N: end - start
            assertEquals("n", cb.Flip().ToString());
        }

        [Test]
        public virtual void TestAppendStringBuilderII_IllegalArgument()
        {
            CharBuffer cb = CharBuffer.Allocate(10);
            cb.Append(new StringBuilder("String"), 0, 0 - 0); // J2N: end - start
            cb.Append(new StringBuilder("String"), 2, 2 - 2); // J2N: end - start
            try
            {
                cb.Append(new StringBuilder("String"), -1, 1 - -1); // J2N: end - start
                fail("should throw IndexOutOfBoundsException.");
            }
            catch (ArgumentOutOfRangeException ex)
            {
                // expected;
            }
            try
            {
                cb.Append(new StringBuilder("String"), -1, -1 - -1); // J2N: end - start
                fail("should throw IndexOutOfBoundsException.");
            }
            catch (ArgumentOutOfRangeException ex)
            {
                // expected;
            }
            try
            {
                cb.Append(new StringBuilder("String"), 3, 2 - 3); // J2N: end - start
                fail("should throw IndexOutOfBoundsException.");
            }
            catch (ArgumentOutOfRangeException ex)
            {
                // expected;
            }
            try
            {
                cb.Append(new StringBuilder("String"), 3, 0 - 3); // J2N: end - start
                fail("should throw IndexOutOfBoundsException.");
            }
            catch (ArgumentOutOfRangeException ex)
            {
                // expected;
            }
            try
            {
                cb.Append(new StringBuilder("String"), 3, 110 - 3); // J2N: end - start
                fail("should throw IndexOutOfBoundsException.");
            }
            catch (ArgumentOutOfRangeException ex)
            {
                // expected;
            }
        }

        [Test]
        public virtual void TestAppendStringNormal()
        {
            CharBuffer cb = CharBuffer.Allocate(10);
            cb.Put('A');
            assertSame(cb, cb.Append("String"));
            assertEquals("AString", cb.Flip().ToString());
            cb.Append((string)null);
            assertEquals("null", cb.Flip().ToString());
        }

        [Test]
        public virtual void TestAppendStringIINormal()
        {
            CharBuffer cb = CharBuffer.Allocate(10);
            cb.Put('A');
            assertSame(cb, cb.Append("String", 1, 3 - 1)); // J2N: end - start
            assertEquals("Atr", cb.Flip().ToString());

            cb.Append((string)null, 0, 1 - 0); // J2N: end - start
            assertEquals("n", cb.Flip().ToString());
        }

        [Test]
        public virtual void TestAppendStringII_IllegalArgument()
        {
            CharBuffer cb = CharBuffer.Allocate(10);
            cb.Append("String", 0, 0 - 0); // J2N: end - start
            cb.Append("String", 2, 2 - 0); // J2N: end - start
            try
            {
                cb.Append("String", -1, 1 - -1); // J2N: end - start
                fail("should throw IndexOutOfBoundsException.");
            }
            catch (ArgumentOutOfRangeException ex)
            {
                // expected;
            }
            try
            {
                cb.Append("String", -1, -1 - -1); // J2N: end - start
                fail("should throw IndexOutOfBoundsException.");
            }
            catch (ArgumentOutOfRangeException ex)
            {
                // expected;
            }
            try
            {
                cb.Append("String", 3, 2 - 3); // J2N: end - start
                fail("should throw IndexOutOfBoundsException.");
            }
            catch (ArgumentOutOfRangeException ex)
            {
                // expected;
            }
            try
            {
                cb.Append("String", 3, 0 - 3); // J2N: end - start
                fail("should throw IndexOutOfBoundsException.");
            }
            catch (ArgumentOutOfRangeException ex)
            {
                // expected;
            }
            try
            {
                cb.Append("String", 3, 110 - 3); // J2N: end - start
                fail("should throw IndexOutOfBoundsException.");
            }
            catch (ArgumentOutOfRangeException ex)
            {
                // expected;
            }
        }

        // *** End Added for .NET ***


        [Test]
        public virtual void TestAppendCharSequenceNormal()
        {
            CharBuffer cb = CharBuffer.Allocate(10);
            cb.Put('A');
            assertSame(cb, cb.Append("String".AsCharSequence()));
            assertEquals("AString", cb.Flip().ToString());
            cb.Append((ICharSequence)null);
            assertEquals("null", cb.Flip().ToString());
        }

        [Test]
        public virtual void TestAppendCharSequenceIINormal()
        {
            CharBuffer cb = CharBuffer.Allocate(10);
            cb.Put('A');
            assertSame(cb, cb.Append("String".AsCharSequence(), 1, 3 - 1)); // J2N: end - start
            assertEquals("Atr", cb.Flip().ToString());

            cb.Append((ICharSequence)null, 0, 1 - 0); // J2N: end - start
            assertEquals("n", cb.Flip().ToString());
        }

        [Test]
        public virtual void TestAppendCharSequenceII_IllegalArgument()
        {
            CharBuffer cb = CharBuffer.Allocate(10);
            cb.Append("String".AsCharSequence(), 0, 0 - 0); // J2N: end - start
            cb.Append("String".AsCharSequence(), 2, 2 - 2); // J2N: end - start
            try
            {
                cb.Append("String".AsCharSequence(), -1, 1 - -1); // J2N: end - start
                fail("should throw IndexOutOfBoundsException.");
            }
            catch (ArgumentOutOfRangeException ex)
            {
                // expected;
            }
            try
            {
                cb.Append("String".AsCharSequence(), -1, -1 - -1); // J2N: end - start
                fail("should throw IndexOutOfBoundsException.");
            }
            catch (ArgumentOutOfRangeException ex)
            {
                // expected;
            }
            try
            {
                cb.Append("String".AsCharSequence(), 3, 2 - 3); // J2N: end - start
                fail("should throw IndexOutOfBoundsException.");
            }
            catch (ArgumentOutOfRangeException ex)
            {
                // expected;
            }
            try
            {
                cb.Append("String".AsCharSequence(), 3, 0 - 3); // J2N: end - start
                fail("should throw IndexOutOfBoundsException.");
            }
            catch (ArgumentOutOfRangeException ex)
            {
                // expected;
            }
            try
            {
                cb.Append("String".AsCharSequence(), 3, 110 - 3); // J2N: end - start
                fail("should throw IndexOutOfBoundsException.");
            }
            catch (ArgumentOutOfRangeException ex)
            {
                // expected;
            }
        }

        [Test]
        public virtual void TestReadCharBuffer()
        {
            CharBuffer source = CharBuffer.Wrap("String");
            CharBuffer target = CharBuffer.Allocate(10);
            assertEquals(6, source.Read(target));
            assertEquals("String", target.Flip().ToString());
            // return -1 when nothing to read
            assertEquals(-1, source.Read(target));
            // NullPointerException
            try
            {
                assertEquals(-1, source.Read(null));
                fail("should throw NullPointerException.");
            }
            catch (ArgumentNullException ex)
            {
                // expected;
            }

        }

        [Test]
        public virtual void TestReadReadOnly()
        {
            CharBuffer source = CharBuffer.Wrap("String");
            CharBuffer target = CharBuffer.Allocate(10).AsReadOnlyBuffer();
            try
            {
                source.Read(target);
                fail("should throw ReadOnlyBufferException.");
            }
            catch (ReadOnlyBufferException ex)
            {
                // expected;
            }
            // if target has no remaining, needn't to check the isReadOnly
            target.Flip();
            assertEquals(0, source.Read(target));
        }

        [Test]
        public virtual void TestReadOverflow()
        {
            CharBuffer source = CharBuffer.Wrap("String");
            CharBuffer target = CharBuffer.Allocate(1);
            assertEquals(1, source.Read(target));
            assertEquals("S", target.Flip().ToString());
            assertEquals(1, source.Position);
        }

        [Test]
        public virtual void TestReadSelf()
        {
            CharBuffer source = CharBuffer.Wrap("abuffer");
            try
            {
                source.Read(source);
                fail("should throw IAE.");
            }
            catch (ArgumentException e)
            {
                //expected
            }
        }

        [Test]
        public virtual void TestRead_scenario1()
        {
            char[]
            charArray = new char[] { 'a', 'b' };
            CharBuffer charBuffer = CharBuffer.Wrap(charArray);
            try
            {
                charBuffer.Read(charBuffer);
                fail("should throw IllegalArgumentException");
            }
            catch (ArgumentException e)
            {
                // expected
            }
            charBuffer.Put(charArray);
            assertEquals(-1, charBuffer.Read(charBuffer));
        }

        [Test]
        public virtual void TestRead_scenario2()
        {
            CharBuffer charBufferA = CharBuffer.Allocate(0);
            CharBuffer allocateBuffer = CharBuffer.Allocate(1);
            CharBuffer charBufferB = CharBuffer.Wrap(allocateBuffer);
            assertEquals(-1, charBufferA.Read(charBufferB));

            allocateBuffer.Append(allocateBuffer);
            charBufferB = CharBuffer.Wrap(allocateBuffer);
            assertEquals(-1, charBufferA.Read(charBufferB));
        }

        //[Test]
        //public virtual void TestIsDirect() // J2N: IsDirect not supported
        //{
        //    assertFalse(buf.IsDirect);
        //}

        [Test]
        public virtual void TestHasArray()
        {
            assertTrue(buf.HasArray);
        }

        [Test]
        public virtual void TestOrder()
        {
            assertEquals(ByteOrder.NativeOrder, buf.Order);
        }

        [Test]
        public override void TestIsReadOnly()
        {
            assertFalse(buf.IsReadOnly);
        }
    }
}
