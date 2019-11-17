using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace J2N.IO
{
    // This class was sourced from the Apache Harmony project
    // https://svn.apache.org/repos/asf/harmony/enhanced/java/trunk/
    // Original name: DoubleBufferTest.java

    /// <summary>
    /// Tests java.nio.DoubleBuffer
    /// </summary>
    public class TestDoubleBuffer : AbstractBufferTest
    {
        protected const int SMALL_TEST_LENGTH = 5;

        protected const int BUFFER_LENGTH = 20;

        protected DoubleBuffer buf;

        public override void SetUp()
        {
            buf = DoubleBuffer.Allocate(BUFFER_LENGTH);
            loadTestData1(buf);
            baseBuf = buf;
        }

        public override void TearDown()
        {
            buf = null;
            baseBuf = null;
        }

        /*
         * Test with bit sequences that represent the IEEE754 doubles Positive
         * infinity, negative infinity, and NaN.
         */
        [Test]
        public void TestNaNs()
        {
            long[] nans = new long[] { 0x7ff0000000000000L, unchecked((long)0xfff0000000000000L),
                0x7ff8000000000000L };
            for (int i = 0; i < nans.Length; i++)
            {
                long longBitsIn = nans[i];
                double dbl = BitConversion.Int64BitsToDouble(longBitsIn);
                long longBitsOut = BitConversion.DoubleToRawInt64Bits(dbl);
                // Sanity check
                assertTrue(longBitsIn == longBitsOut);

                // Store the double and retrieve it
                ByteBuffer buffer = ByteBuffer.Allocate(8);
                buffer.PutDouble(dbl);
                double bufDoubleOut = buffer.GetDouble(0);

                // Check the bits sequence was not normalized
                long bufLongOut = BitConversion.DoubleToRawInt64Bits(bufDoubleOut);
                assertTrue(longBitsIn == bufLongOut);
            }
        }

        [Test]
        public void TestArray()
        {
            double[] array = buf.Array;
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
        public void TestArrayOffset()
        {
            double[] array = buf.Array;
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
        public void TestAsReadOnlyBuffer()
        {
            buf.Clear();
            buf.Mark();
            buf.Position = (buf.Limit);

            // readonly's contents should be the same as buf
            DoubleBuffer @readonly = buf.AsReadOnlyBuffer();
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
        public void TestCompact()
        {
            // case: buffer is full
            buf.Clear();
            buf.Mark();
            loadTestData1(buf);
            DoubleBuffer ret = buf.Compact();
            assertSame(ret, buf);
            assertEquals(buf.Position, buf.Capacity);
            assertEquals(buf.Limit, buf.Capacity);
            assertContentLikeTestData1(buf, 0, 0.0, buf.Capacity);
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
            assertContentLikeTestData1(buf, 0, 0.0, buf.Capacity);
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
            assertContentLikeTestData1(buf, 0, 1.0, 4);
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
        public void TestCompareTo()
        {
            DoubleBuffer other = DoubleBuffer.Allocate(buf.Capacity);
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

            DoubleBuffer dbuffer1 = DoubleBuffer.Wrap(new double[] { double.NaN });
            DoubleBuffer dbuffer2 = DoubleBuffer.Wrap(new double[] { double.NaN });
            DoubleBuffer dbuffer3 = DoubleBuffer.Wrap(new double[] { 42d });

            assertEquals("Failed equal comparison with NaN entry", 0, dbuffer1
                    .CompareTo(dbuffer2));
            assertEquals("Failed greater than comparison with NaN entry", 1, dbuffer3
                    .CompareTo(dbuffer1));
            assertEquals("Failed less than comparison with NaN entry", -1, dbuffer1 // J2N: Corrected this (Harmony) test to match JDK 7, which expects NaN to be symmetric. This also matches .NET double.CompareTo().
                    .CompareTo(dbuffer3));
        }

        [Test]
        public void TestDuplicate()
        {
            buf.Clear();
            buf.Mark();
            buf.Position = (buf.Limit);

            // duplicate's contents should be the same as buf
            DoubleBuffer duplicate = buf.Duplicate();
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
            // FIXME
            if (!duplicate.IsReadOnly)
            {
                loadTestData1(buf);
                assertContentEquals(buf, duplicate);
                loadTestData2(duplicate);
                assertContentEquals(buf, duplicate);
            }
        }

        [Test]
        public void TestEquals()
        {
            // equal to self
            assertTrue(buf.Equals(buf));
            DoubleBuffer @readonly = buf.AsReadOnlyBuffer();
            assertTrue(buf.Equals(@readonly));
            DoubleBuffer duplicate = buf.Duplicate();
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
         * Class under test for double get()
         */
        [Test]
        public void TestGet()
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
         * Class under test for java.nio.DoubleBuffer get(double[])
         */
        [Test]
        public void TestGetdoubleArray()
        {
            double[] array = new double[1];
            buf.Clear();
            for (int i = 0; i < buf.Capacity; i++)
            {
                assertEquals(buf.Position, i);
                DoubleBuffer ret = buf.Get(array);
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
        }

        /*
         * Class under test for java.nio.DoubleBuffer get(double[], int, int)
         */
        [Test]
        public void TestGetdoubleArrayintint()
        {
            buf.Clear();
            double[] array = new double[buf.Capacity];

            try
            {
                buf.Get(new double[buf.Capacity + 1], 0, buf.Capacity + 1);
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
                buf.Get((double[])null, 0, -1);
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
            DoubleBuffer ret = buf.Get(array, 0, array.Length);
            assertEquals(buf.Position, buf.Capacity);
            assertContentEquals(buf, array, 0, array.Length);
            assertSame(ret, buf);
        }

        /*
         * Class under test for double get(int)
         */
        [Test]
        public void TestGetint()
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
        public void TestHasArray()
        {
            assertTrue(buf.HasArray);
        }

        [Test]
        public void TestHashCode()
        {
            buf.Clear();
            DoubleBuffer @readonly = buf.AsReadOnlyBuffer();
            DoubleBuffer duplicate = buf.Duplicate();
            assertTrue(buf.GetHashCode() == @readonly.GetHashCode());

            assertTrue(buf.Capacity > 5);
            duplicate.Position = (buf.Capacity / 2);
            assertTrue(buf.GetHashCode() != duplicate.GetHashCode());
        }

        //[Test]
        //public void TestIsDirect() // J2N: IsDirect not supported
        //{
        //    assertFalse(buf.IsDirect);
        //}

        [Test]
        public void TestOrder()
        {
            assertEquals(ByteOrder.NativeOrder, buf.Order);
        }

        /*
         * Class under test for java.nio.DoubleBuffer put(double)
         */
        [Test]
        public void TestPutdouble()
        {

            buf.Clear();
            for (int i = 0; i < buf.Capacity; i++)
            {
                assertEquals(buf.Position, i);
                DoubleBuffer ret = buf.Put((double)i);
                assertEquals(buf.Get(i), (double)i, 0.0);
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
         * Class under test for java.nio.DoubleBuffer put(double[])
         */
        [Test]
        public void TestPutdoubleArray()
        {
            double[] array = new double[1];

            buf.Clear();
            for (int i = 0; i < buf.Capacity; i++)
            {
                assertEquals(buf.Position, i);
                array[0] = (double)i;
                DoubleBuffer ret = buf.Put(array);
                assertEquals(buf.Get(i), (double)i, 0.0);
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
        }

        /*
         * Class under test for java.nio.DoubleBuffer put(double[], int, int)
         */
        [Test]
        public void TestPutdoubleArrayintint()
        {
            buf.Clear();
            double[] array = new double[buf.Capacity];

            try
            {
                buf.Put(new double[buf.Capacity + 1], 0, buf.Capacity + 1);
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
                buf.Put((double[])null, 0, -1);
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
            DoubleBuffer ret = buf.Put(array, 0, array.Length);
            assertEquals(buf.Position, buf.Capacity);
            assertContentEquals(buf, array, 0, array.Length);
            assertSame(ret, buf);
        }

        /*
         * Class under test for java.nio.DoubleBuffer put(java.nio.DoubleBuffer)
         */
        [Test]
        public void TestPutDoubleBuffer()
        {
            DoubleBuffer other = DoubleBuffer.Allocate(buf.Capacity);

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
                buf.Put(DoubleBuffer.Allocate(buf.Capacity + 1));
                fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (BufferOverflowException e)
            {
                // expected
            }

            loadTestData2(other);
            other.Clear();
            buf.Clear();
            DoubleBuffer ret = buf.Put(other);
            assertEquals(other.Position, other.Capacity);
            assertEquals(buf.Position, buf.Capacity);
            assertContentEquals(other, buf);
            assertSame(ret, buf);
        }

        /*
         * Class under test for java.nio.DoubleBuffer put(int, double)
         */
        [Test]
        public void TestPutintdouble()
        {
            buf.Clear();
            for (int i = 0; i < buf.Capacity; i++)
            {
                assertEquals(buf.Position, 0);
                DoubleBuffer ret = buf.Put(i, (double)i);
                assertEquals(buf.Get(i), (double)i, 0.0);
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
        public void TestSlice()
        {
            assertTrue(buf.Capacity > 5);
            buf.Position = (1);
            buf.Limit = (buf.Capacity - 1);

            DoubleBuffer slice = buf.Slice();
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
            // FIXME:
            if (!slice.IsReadOnly)
            {
                loadTestData1(slice);
                assertContentLikeTestData1(buf, 1, 0, slice.Capacity);
                buf.Put(2, 500);
                assertEquals(slice.Get(1), 500, 0.0);
            }
        }

        [Test]
        public void TestToString()
        {
            String str = buf.ToString();
            assertTrue(str.IndexOf("Double") >= 0 || str.IndexOf("double") >= 0);
            assertTrue(str.IndexOf("" + buf.Position) >= 0);
            assertTrue(str.IndexOf("" + buf.Limit) >= 0);
            assertTrue(str.IndexOf("" + buf.Capacity) >= 0);
        }

        void loadTestData1(double[] array, int offset, int length)
        {
            for (int i = 0; i < length; i++)
            {
                array[offset + i] = (double)i;
            }
        }

        void loadTestData2(double[] array, int offset, int length)
        {
            for (int i = 0; i < length; i++)
            {
                array[offset + i] = (double)length - i;
            }
        }

        void loadTestData1(DoubleBuffer buf)
        {
            buf.Clear();
            for (int i = 0; i < buf.Capacity; i++)
            {
                buf.Put(i, (double)i);
            }
        }

        void loadTestData2(DoubleBuffer buf)
        {
            buf.Clear();
            for (int i = 0; i < buf.Capacity; i++)
            {
                buf.Put(i, (double)buf.Capacity - i);
            }
        }

        private void assertContentEquals(DoubleBuffer buf, double[] array,
                int offset, int length)
        {
            for (int i = 0; i < length; i++)
            {
                assertEquals(buf.Get(i), array[offset + i], 0.01);
            }
        }

        private void assertContentEquals(DoubleBuffer buf, DoubleBuffer other)
        {
            assertEquals(buf.Capacity, other.Capacity);
            for (int i = 0; i < buf.Capacity; i++)
            {
                assertEquals(buf.Get(i), other.Get(i), 0.01);
            }
        }

        private void assertContentLikeTestData1(DoubleBuffer buf, int startIndex,
                double startValue, int length)
        {
            double value = startValue;
            for (int i = 0; i < length; i++)
            {
                assertEquals(buf.Get(startIndex + i), value, 0.01);
                value = value + 1.0;
            }
        }

    }
}
