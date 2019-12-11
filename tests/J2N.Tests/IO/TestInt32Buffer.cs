using NUnit.Framework;
using System;

namespace J2N.IO
{
    // This class was sourced from the Apache Harmony project
    // https://svn.apache.org/repos/asf/harmony/enhanced/java/trunk/
    // Original name: IntBufferTest.java

    /// <summary>
    /// Tests java.nio.Int32Buffer
    /// </summary>
    public class TestInt32Buffer : AbstractBufferTest
    {
        protected const int SMALL_TEST_LENGTH = 5;

        protected const int BUFFER_LENGTH = 20;

        protected Int32Buffer buf;

        public override void SetUp()
        {
            buf = Int32Buffer.Allocate(BUFFER_LENGTH);
            loadTestData1(buf);
            baseBuf = buf;
        }

        public override void TearDown()
        {
            buf = null;
            baseBuf = null;
        }

        [Test]
        public virtual void TestArray()
        {
            int[] array = buf.Array;
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
            int[] array = buf.Array;
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
            Int32Buffer @readonly = buf.AsReadOnlyBuffer();
            assertNotSame(buf, @readonly);
            assertTrue(@readonly.IsReadOnly);
            assertEquals(buf.Position, @readonly.Position);
            assertEquals(buf.Limit, @readonly.Limit);
            //assertEquals(buf.IsDirect, @readonly.IsDirect); // J2N: IsDirect not supported
            assertEquals(buf.Order, @readonly.Order);
            assertContentEquals(buf, @readonly);

            // readonly's position, mark, and limit should be independent to buf
            @readonly.Reset();
            assertEquals(@readonly.Position, 0);
            @readonly.Clear();
            assertEquals(buf.Position, buf.Limit);
            buf.Reset();
            assertEquals(buf.Position, 0);
        }

        [Test]
        public virtual void TestCompact()
        {
            // case: buffer is full
            buf.Clear();
            buf.Mark();
            loadTestData1(buf);
            Int32Buffer ret = buf.Compact();
            assertSame(ret, buf);
            assertEquals(buf.Position, buf.Capacity);
            assertEquals(buf.Limit, buf.Capacity);
            assertContentLikeTestData1(buf, 0, 0, buf.Capacity);
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
            assertContentLikeTestData1(buf, 0, 0, buf.Capacity);
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
            assertContentLikeTestData1(buf, 0, 1, 4);
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

            // normal cases
            assertTrue(buf.Capacity > 5);
            buf.Clear();
            Int32Buffer other = Int32Buffer.Allocate(buf.Capacity);
            loadTestData1(other);
            assertEquals(0, buf.CompareTo(other));
            assertEquals(0, other.CompareTo(buf));
            buf.Position = (1);
            assertTrue(buf.CompareTo(other) > 0);
            assertTrue(other.CompareTo(buf) < 0);
            other.Position = (2);
            assertTrue(buf.CompareTo(other) < 0);
            assertTrue(other.CompareTo(buf) > 0);
            buf.Position = (2);
            other.Limit = (5);
            assertTrue(buf.CompareTo(other) > 0);
            assertTrue(other.CompareTo(buf) < 0);
        }

        [Test]
        public virtual void TestDuplicate()
        {
            buf.Clear();
            buf.Mark();
            buf.Position = (buf.Limit);

            // duplicate's contents should be the same as buf
            Int32Buffer duplicate = buf.Duplicate();
            assertNotSame(buf, duplicate);
            assertEquals(buf.Position, duplicate.Position);
            assertEquals(buf.Limit, duplicate.Limit);
            assertEquals(buf.IsReadOnly, duplicate.IsReadOnly);
            //assertEquals(buf.IsDirect, duplicate.IsDirect); // J2N: IsDirect not supported
            assertEquals(buf.Order, duplicate.Order);
            assertContentEquals(buf, duplicate);

            // duplicate's position, mark, and limit should be independent to buf
            duplicate.Reset();
            assertEquals(duplicate.Position, 0);
            duplicate.Clear();
            assertEquals(buf.Position, buf.Limit);
            buf.Reset();
            assertEquals(buf.Position, 0);

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
            Int32Buffer @readonly = buf.AsReadOnlyBuffer();
            assertTrue(buf.Equals(@readonly));
            Int32Buffer duplicate = buf.Duplicate();
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
         * Class under test for int get()
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
         * Class under test for java.nio.Int32Buffer get(int[])
         */
        [Test]
        public virtual void TestGetintArray()
        {
            int[] array = new int[1];
            buf.Clear();
            for (int i = 0; i < buf.Capacity; i++)
            {
                assertEquals(buf.Position, i);
                Int32Buffer ret = buf.Get(array);
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
            try
            {
                buf.Get((int[])null);
                fail("Should throw NPE"); //$NON-NLS-1$
            }
            catch (ArgumentNullException e)
            {
                // expected
            }
        }

        /*
         * Class under test for java.nio.Int32Buffer get(int[], int, int)
         */
        [Test]
        public virtual void TestGetintArrayintint()
        {
            buf.Clear();
            int[] array = new int[buf.Capacity];

            try
            {
                buf.Get(new int[buf.Capacity + 1], 0, buf.Capacity + 1);
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
                buf.Get((int[])null, 2, -1);
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
            Int32Buffer ret = buf.Get(array, 0, array.Length);
            assertEquals(buf.Position, buf.Capacity);
            assertContentEquals(buf, array, 0, array.Length);
            assertSame(ret, buf);
        }

        /*
         * Class under test for int get(int)
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
        public virtual void TestHasArray()
        {
            assertNotNull(buf.Array);
        }

        [Test]
        public virtual void TestHashCode()
        {
            buf.Clear();
            Int32Buffer @readonly = buf.AsReadOnlyBuffer();
            Int32Buffer duplicate = buf.Duplicate();
            assertTrue(buf.GetHashCode() == @readonly.GetHashCode());

            assertTrue(buf.Capacity > 5);
            duplicate.Position = (buf.Capacity / 2);
            assertTrue(buf.GetHashCode() != duplicate.GetHashCode());
        }

        //[Test]
        //public virtual void TestIsDirect() // J2N: IsDirect not supported
        //{
        //    assertFalse(buf.IsDirect);
        //}

        [Test]
        public virtual void TestOrder()
        {
            var _ = buf.Order;
            assertEquals(ByteOrder.NativeOrder, buf.Order);
        }

        /*
         * Class under test for java.nio.Int32Buffer put(int)
         */
        [Test]
        public virtual void TestPutint()
        {
            buf.Clear();
            for (int i = 0; i < buf.Capacity; i++)
            {
                assertEquals(buf.Position, i);
                Int32Buffer ret = buf.Put((int)i);
                assertEquals(buf.Get(i), (int)i);
                assertSame(ret, buf);
            }
            try
            {
                buf.Put(0);
                fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (BufferOverflowException e)
            {
                // expected
            }
        }

        /*
         * Class under test for java.nio.Int32Buffer put(int[])
         */
        [Test]
        public virtual void TestPutintArray()
        {
            int[] array = new int[1];
            buf.Clear();
            for (int i = 0; i < buf.Capacity; i++)
            {
                assertEquals(buf.Position, i);
                array[0] = (int)i;
                Int32Buffer ret = buf.Put(array);
                assertEquals(buf.Get(i), (int)i);
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
                buf.Position=(buf.Limit);
                buf.Put((int[])null);
                fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (ArgumentNullException e)
            {
                // expected
            }
        }

        /*
         * Class under test for java.nio.Int32Buffer put(int[], int, int)
         */
        [Test]
        public virtual void TestPutintArrayintint()
        {
            buf.Clear();
            int[] array = new int[buf.Capacity];
            try
            {
                buf.Put(new int[buf.Capacity + 1], 0, buf.Capacity + 1);
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
                buf.Put((int[])null, 0, -1);
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
            Int32Buffer ret = buf.Put(array, 0, array.Length);
            assertEquals(buf.Position, buf.Capacity);
            assertContentEquals(buf, array, 0, array.Length);
            assertSame(ret, buf);
        }

        /*
         * Class under test for java.nio.Int32Buffer put(java.nio.Int32Buffer)
         */
        [Test]
        public virtual void TestPutInt32Buffer()
        {
            Int32Buffer other = Int32Buffer.Allocate(buf.Capacity);
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
                buf.Put(Int32Buffer.Allocate(buf.Capacity + 1));
                fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (BufferOverflowException e)
            {
                // expected
            }
            try
            {
                buf.Flip();
                buf.Put((Int32Buffer)null);
                fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (ArgumentNullException e)
            {
                // expected
            }

            loadTestData2(other);
            other.Clear();
            buf.Clear();
            Int32Buffer ret = buf.Put(other);
            assertEquals(other.Position, other.Capacity);
            assertEquals(buf.Position, buf.Capacity);
            assertContentEquals(other, buf);
            assertSame(ret, buf);
        }

        /*
         * Class under test for java.nio.Int32Buffer put(int, int)
         */
        [Test]
        public virtual void TestPutintint()
        {
            buf.Clear();
            for (int i = 0; i < buf.Capacity; i++)
            {
                assertEquals(buf.Position, 0);
                Int32Buffer ret = buf.Put(i, (int)i);
                assertEquals(buf.Get(i), (int)i);
                assertSame(ret, buf);
            }
            try
            {
                buf.Put(-1, 0);
                fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (ArgumentOutOfRangeException e)
            {
                // expected
            }
            try
            {
                buf.Put(buf.Limit, 0);
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
            buf.Position=(1);
            buf.Limit=(buf.Capacity - 1);

            Int32Buffer slice = buf.Slice();
            assertEquals(buf.IsReadOnly, slice.IsReadOnly);
            // assertEquals(buf.IsDirect, slice.IsDirect); // J2N: IsDirect not supported
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
                assertContentLikeTestData1(buf, 1, 0, slice.Capacity);
                buf.Put(2, 500);
                assertEquals(slice.Get(1), 500);
            }
        }

        [Test]
        public virtual void TestToString()
        {
            String str = buf.ToString();
            assertTrue(str.IndexOf("Int32") >= 0 || str.IndexOf("int") >= 0);
            assertTrue(str.IndexOf("" + buf.Position) >= 0);
            assertTrue(str.IndexOf("" + buf.Limit) >= 0);
            assertTrue(str.IndexOf("" + buf.Capacity) >= 0);
        }

        internal void loadTestData1(int[] array, int offset, int length)
        {
            for (int i = 0; i < length; i++)
            {
                array[offset + i] = (int)i;
            }
        }

        internal void loadTestData2(int[] array, int offset, int length)
        {
            for (int i = 0; i < length; i++)
            {
                array[offset + i] = (int)length - i;
            }
        }

        internal void loadTestData1(Int32Buffer buf)
        {
            buf.Clear();
            for (int i = 0; i < buf.Capacity; i++)
            {
                buf.Put(i, (int)i);
            }
        }

        internal void loadTestData2(Int32Buffer buf)
        {
            buf.Clear();
            for (int i = 0; i < buf.Capacity; i++)
            {
                buf.Put(i, (int)buf.Capacity - i);
            }
        }

        void assertContentEquals(Int32Buffer buf, int[] array,
              int offset, int length)
        {
            for (int i = 0; i < length; i++)
            {
                assertEquals(buf.Get(i), array[offset + i]);
            }
        }

        void assertContentEquals(Int32Buffer buf, Int32Buffer other)
        {
            assertEquals(buf.Capacity, other.Capacity);
            for (int i = 0; i < buf.Capacity; i++)
            {
                assertEquals(buf.Get(i), other.Get(i));
            }
        }

        void assertContentLikeTestData1(Int32Buffer buf,
              int startIndex, int startValue, int length)
        {
            int value = startValue;
            for (int i = 0; i < length; i++)
            {
                assertEquals(buf.Get(startIndex + i), value);
                value = value + 1;
            }
        }
    }
}
