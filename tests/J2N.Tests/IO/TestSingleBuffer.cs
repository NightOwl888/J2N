using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace J2N.IO
{
    // This class was sourced from the Apache Harmony project
    // https://svn.apache.org/repos/asf/harmony/enhanced/java/trunk/
    // Original name: SingleBufferTest.java

    /// <summary>
    /// Tests java.nio.SingleBuffer
    /// </summary>
    public class TestSingleBuffer : AbstractBufferTest
    {
        protected const int SMALL_TEST_LENGTH = 5;

        protected const int BUFFER_LENGTH = 20;

        protected SingleBuffer buf;

        public override void SetUp()
        {
            buf = SingleBuffer.Allocate(BUFFER_LENGTH);
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
            float[] array = buf.Array;
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
            float[] array = buf.Array;
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
            SingleBuffer @readonly = buf.AsReadOnlyBuffer();
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
            SingleBuffer ret = buf.Compact();
            assertSame(ret, buf);
            assertEquals(buf.Position, buf.Capacity);
            assertEquals(buf.Limit, buf.Capacity);
            assertContentLikeTestData1(buf, 0, 0.0f, buf.Capacity);
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
            assertContentLikeTestData1(buf, 0, 0.0f, buf.Capacity);
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
            buf.Position=(1);
            buf.Limit=(5);
            buf.Mark();
            ret = buf.Compact();
            assertSame(ret, buf);
            assertEquals(buf.Position, 4);
            assertEquals(buf.Limit, buf.Capacity);
            assertContentLikeTestData1(buf, 0, 1.0f, 4);
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
            // J2N: Changed the behavior from Java because in .NET CompareTo is supposed to return 1 instead of throwing an exception when passed null
            //try
            //{
            //    buf.CompareTo(null);
            //    fail("Should throw NPE");
            //}
            //catch (ArgumentNullException e)
            //{
            //    // expected
            //}

            // J2N: Cover null for .NET. See: https://stackoverflow.com/a/4852537
            assertEquals(1, buf.CompareTo(null));

            // compare to self
            assertEquals(0, buf.CompareTo(buf));

            // normal cases
            assertTrue(buf.Capacity > 5);
            buf.Clear();
            SingleBuffer other = SingleBuffer.Allocate(buf.Capacity);
            loadTestData1(other);
            assertEquals(0, buf.CompareTo(other));
            assertEquals(0, other.CompareTo(buf));
            buf.Position=(1);
            assertTrue(buf.CompareTo(other) > 0);
            assertTrue(other.CompareTo(buf) < 0);
            other.Position=(2);
            assertTrue(buf.CompareTo(other) < 0);
            assertTrue(other.CompareTo(buf) > 0);
            buf.Position=(2);
            other.Limit=(5);
            assertTrue(buf.CompareTo(other) > 0);
            assertTrue(other.CompareTo(buf) < 0);

            SingleBuffer fbuffer1 = SingleBuffer.Wrap(new float[] { float.NaN });
            SingleBuffer fbuffer2 = SingleBuffer.Wrap(new float[] { float.NaN });
            SingleBuffer fbuffer3 = SingleBuffer.Wrap(new float[] { 42f });

            assertEquals("Failed equal comparison with NaN entry", 0, fbuffer1
                    .CompareTo(fbuffer2));
            assertEquals("Failed greater than comparison with NaN entry", 1, fbuffer3
                    .CompareTo(fbuffer1));
            assertEquals("Failed less than comparison with NaN entry", -1, fbuffer1 // J2N: Corrected this (Harmony) test to match JDK 7, which expects NaN to be symmetric. This also matches .NET float.CompareTo().
                    .CompareTo(fbuffer3));
        }

        [Test]
        public virtual void TestDuplicate()
        {
            buf.Clear();
            buf.Mark();
            buf.Position=(buf.Limit);

            // duplicate's contents should be the same as buf
            SingleBuffer duplicate = buf.Duplicate();
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
            SingleBuffer @readonly = buf.AsReadOnlyBuffer();
            assertTrue(buf.Equals(@readonly));
            SingleBuffer duplicate = buf.Duplicate();
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
         * Class under test for float get()
         */
        [Test]
        public virtual void TestGet()
        {
            buf.Clear();
            for (int i = 0; i < buf.Capacity; i++)
            {
                assertEquals(buf.Position, i);
                assertEquals(buf.Get(), buf.Get(i), 0.01);
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
         * Class under test for java.nio.SingleBuffer get(float[])
         */
        [Test]
        public virtual void TestGetfloatArray()
        {
            float[] array = new float[1];
            buf.Clear();
            for (int i = 0; i < buf.Capacity; i++)
            {
                assertEquals(buf.Position, i);
                SingleBuffer ret = buf.Get(array);
                assertEquals(array[0], buf.Get(i), 0.01);
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
                buf.Position=(buf.Limit);
                buf.Get((float[])null);
                fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (ArgumentNullException e)
            {
                // expected
            }
            buf.Get(new float[0]);
        }

        /*
         * Class under test for java.nio.SingleBuffer get(float[], int, int)
         */
        [Test]
        public virtual void TestGetfloatArrayintint()
        {
            buf.Clear();
            float[] array = new float[buf.Capacity];

            try
            {
                buf.Get(new float[buf.Capacity + 1], 0, buf.Capacity + 1);
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
                buf.Get((float[])null, 2, -1);
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
            SingleBuffer ret = buf.Get(array, 0, array.Length);
            assertEquals(buf.Position, buf.Capacity);
            assertContentEquals(buf, array, 0, array.Length);
            assertSame(ret, buf);
        }

        /*
         * Class under test for float get(int)
         */
        [Test]
        public virtual void TestGetint()
        {
            buf.Clear();
            for (int i = 0; i < buf.Capacity; i++)
            {
                assertEquals(buf.Position, i);
                assertEquals(buf.Get(), buf.Get(i), 0.01);
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
            SingleBuffer @readonly = buf.AsReadOnlyBuffer();
            SingleBuffer duplicate = buf.Duplicate();
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
            if (buf.HasArray)
            {
                assertEquals(ByteOrder.NativeOrder, buf.Order);
            }
        }

        /*
         * Class under test for java.nio.SingleBuffer put(float)
         */
        [Test]
        public virtual void TestPutfloat()
        {
            buf.Clear();
            for (int i = 0; i < buf.Capacity; i++)
            {
                assertEquals(buf.Position, i);
                SingleBuffer ret = buf.Put((float)i);
                assertEquals(buf.Get(i), (float)i, 0.0);
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
         * Class under test for java.nio.SingleBuffer put(float[])
         */
        [Test]
        public virtual void TestPutfloatArray()
        {
            float[] array = new float[1];
            buf.Clear();
            for (int i = 0; i < buf.Capacity; i++)
            {
                assertEquals(buf.Position, i);
                array[0] = (float)i;
                SingleBuffer ret = buf.Put(array);
                assertEquals(buf.Get(i), (float)i, 0.0);
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
                buf.Position = (buf.Limit);
                buf.Put((float[])null);
                fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (ArgumentNullException e)
            {
                // expected
            }
        }

        /*
         * Class under test for java.nio.SingleBuffer put(float[], int, int)
         */
        [Test]
        public virtual void TestPutfloatArrayintint()
        {
            buf.Clear();
            float[] array = new float[buf.Capacity];
            try
            {
                buf.Put(new float[buf.Capacity + 1], 0, buf.Capacity + 1);
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
                buf.Put((float[])null, 0, -1);
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
            SingleBuffer ret = buf.Put(array, 0, array.Length);
            assertEquals(buf.Position, buf.Capacity);
            assertContentEquals(buf, array, 0, array.Length);
            assertSame(ret, buf);
        }

        /*
         * Class under test for java.nio.SingleBuffer put(java.nio.SingleBuffer)
         */
        [Test]
        public virtual void TestPutSingleBuffer()
        {
            SingleBuffer other = SingleBuffer.Allocate(buf.Capacity);
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
                buf.Put(SingleBuffer.Allocate(buf.Capacity + 1));
                fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (BufferOverflowException e)
            {
                // expected
            }
            try
            {
                buf.Flip();
                buf.Put((SingleBuffer)null);
                fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (ArgumentNullException e)
            {
                // expected
            }
            buf.Clear();
            loadTestData2(other);
            other.Clear();
            buf.Clear();
            SingleBuffer ret = buf.Put(other);
            assertEquals(other.Position, other.Capacity);
            assertEquals(buf.Position, buf.Capacity);
            assertContentEquals(other, buf);
            assertSame(ret, buf);
        }

        /*
         * Class under test for java.nio.SingleBuffer put(int, float)
         */
        [Test]
        public virtual void TestPutintfloat()
        {
            buf.Clear();
            for (int i = 0; i < buf.Capacity; i++)
            {
                assertEquals(buf.Position, 0);
                SingleBuffer ret = buf.Put(i, (float)i);
                assertEquals(buf.Get(i), (float)i, 0.0);
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

            SingleBuffer slice = buf.Slice();
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
                assertContentLikeTestData1(buf, 1, 0, slice.Capacity);
                buf.Put(2, 500);
                assertEquals(slice.Get(1), 500, 0.0);
            }
        }

        [Test]
        public virtual void TestToString()
        {
            String str = buf.ToString();
            assertTrue(str.IndexOf("Single", StringComparison.InvariantCulture) >= 0 || str.IndexOf("float", StringComparison.InvariantCulture) >= 0);
            assertTrue(str.IndexOf("" + buf.Position, StringComparison.InvariantCulture) >= 0);
            assertTrue(str.IndexOf("" + buf.Limit, StringComparison.InvariantCulture) >= 0);
            assertTrue(str.IndexOf("" + buf.Capacity, StringComparison.InvariantCulture) >= 0);
        }

        internal void loadTestData1(float[] array, int offset, int length)
        {
            for (int i = 0; i < length; i++)
            {
                array[offset + i] = (float)i;
            }
        }

        internal void loadTestData2(float[] array, int offset, int length)
        {
            for (int i = 0; i < length; i++)
            {
                array[offset + i] = (float)length - i;
            }
        }

        internal void loadTestData1(SingleBuffer buf)
        {
            buf.Clear();
            for (int i = 0; i < buf.Capacity; i++)
            {
                buf.Put(i, (float)i);
            }
        }

        internal void loadTestData2(SingleBuffer buf)
        {
            buf.Clear();
            for (int i = 0; i < buf.Capacity; i++)
            {
                buf.Put(i, (float)buf.Capacity - i);
            }
        }

        void assertContentEquals(SingleBuffer buf, float[] array,
                int offset, int length)
        {
            for (int i = 0; i < length; i++)
            {
                assertEquals(buf.Get(i), array[offset + i], 0.01);
            }
        }

        void assertContentEquals(SingleBuffer buf, SingleBuffer other)
        {
            assertEquals(buf.Capacity, other.Capacity);
            for (int i = 0; i < buf.Capacity; i++)
            {
                assertEquals(buf.Get(i), other.Get(i), 0.01);
            }
        }

        void assertContentLikeTestData1(SingleBuffer buf,
                int startIndex, float startValue, int length)
        {
            float value = startValue;
            for (int i = 0; i < length; i++)
            {
                assertEquals(buf.Get(startIndex + i), value, 0.01);
                value = value + 1.0f;
            }
        }
    }
}
