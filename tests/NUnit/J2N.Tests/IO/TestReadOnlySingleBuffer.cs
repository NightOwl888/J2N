using NUnit.Framework;
using System;

namespace J2N.IO
{
    public class TestReadOnlySingleBuffer : TestSingleBuffer
    {
        public override void SetUp()
        {
            base.SetUp();
            buf = buf.AsReadOnlyBuffer();
            baseBuf = buf;
        }

        public override void TearDown()
        {
            base.TearDown();
        }

        [Test]
        public override void TestIsReadOnly()
        {
            assertTrue(buf.IsReadOnly);
        }

        [Test]
        public override void TestHasArray()
        {
            assertFalse(buf.HasArray);
        }

        [Test]
        public override void TestArray()
        {
            try
            {
                var _ = buf.Array;
                fail("Should throw ReadOnlyBufferException"); //$NON-NLS-1$
            }
            catch (ReadOnlyBufferException e)
            {
                //expected
            }
        }

        [Test]
        public override void TestHashCode()
        {
            SingleBuffer duplicate = buf.Duplicate();
            assertEquals(buf.GetHashCode(), duplicate.GetHashCode());
        }

        [Test]
        public override void TestArrayOffset()
        {
            try
            {
                var _ = buf.ArrayOffset;
                fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (NotSupportedException e)
            {
                //expected
            }
        }

        [Test]
        public override void TestCompact()
        {
            try
            {
                buf.Compact();
                fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (ReadOnlyBufferException e)
            {
                // expected
            }
        }

        [Test]
        public override void TestPutfloat()
        {
            try
            {
                buf.Put(0);
                fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (ReadOnlyBufferException e)
            {
                // expected
            }
        }

        [Test]
        public override void TestPutfloatSpan() // J2N specific
        {
            Span<float> array = new float[1];
            try
            {
                buf.Put(array);
                fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (ReadOnlyBufferException e)
            {
                // expected
            }
            // J2N: Null converts to empty span, should not throw
            //try
            //{
            //    buf.Put((float[])null);
            //    fail("Should throw Exception"); //$NON-NLS-1$
            //}
            //catch (ArgumentNullException e)
            //{
            //    // expected
            //}
        }

        [Test]
        public override void TestPutfloatArray()
        {
            float[] array = new float[1];
            try
            {
                buf.Put(array);
                fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (ReadOnlyBufferException e)
            {
                // expected
            }
            try
            {
                buf.Put((float[])null);
                fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (ArgumentNullException e)
            {
                // expected
            }
        }

        [Test]
        public override void TestPutfloatArrayintint()
        {
            float[] array = new float[1];
            try
            {
                buf.Put(array, 0, array.Length);
                fail("Should throw ReadOnlyBufferException"); //$NON-NLS-1$
            }
            catch (ReadOnlyBufferException e)
            {
                // expected
            }
            try
            {
                buf.Put((float[])null, 0, 1);
                fail("Should throw ReadOnlyBufferException"); //$NON-NLS-1$
            }
            catch (ReadOnlyBufferException e)
            {
                // expected
            }
            try
            {
                buf.Put(new float[buf.Capacity + 1], 0, buf.Capacity + 1);
                fail("Should throw ReadOnlyBufferException"); //$NON-NLS-1$
            }
            catch (ReadOnlyBufferException e)
            {
                // expected
            }
            try
            {
                buf.Put(array, -1, array.Length);
                fail("Should throw ReadOnlyBufferException"); //$NON-NLS-1$
            }
            catch (ReadOnlyBufferException e)
            {
                // expected
            }
        }

        [Test]
        public override void TestPutSingleBuffer()
        {
            SingleBuffer other = SingleBuffer.Allocate(1);
            try
            {
                buf.Put(other);
                fail("Should throw ReadOnlyBufferException"); //$NON-NLS-1$
            }
            catch (ReadOnlyBufferException e)
            {
                // expected
            }
            try
            {
                buf.Put((SingleBuffer)null);
                fail("Should throw ReadOnlyBufferException"); //$NON-NLS-1$
            }
            catch (ReadOnlyBufferException e)
            {
                // expected
            }
            try
            {
                buf.Put(buf);
                fail("Should throw ReadOnlyBufferException"); //$NON-NLS-1$
            }
            catch (ReadOnlyBufferException e)
            {
                // expected
            }
        }

        [Test]
        public override void TestPutintfloat()
        {
            try
            {
                buf.Put(0, (float)0);
                fail("Should throw ReadOnlyBufferException"); //$NON-NLS-1$
            }
            catch (ReadOnlyBufferException e)
            {
                // expected
            }
            try
            {
                buf.Put(-1, (float)0);
                fail("Should throw ReadOnlyBufferException"); //$NON-NLS-1$
            }
            catch (ReadOnlyBufferException e)
            {
                // expected
            }
        }
    }
}
