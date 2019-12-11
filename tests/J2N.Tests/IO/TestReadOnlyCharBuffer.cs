using NUnit.Framework;
using System;

namespace J2N.IO
{
    public class TestReadOnlyCharBuffer : TestCharBuffer
    {
        public override void SetUp()
        {
            base.SetUp();
            loadTestData1(buf);
            buf = buf.AsReadOnlyBuffer();
            baseBuf = buf;
        }

        public override void TearDown()
        {
            buf = null;
            baseBuf = null;
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
            }
        }

        [Test]
        public override void TestHashCode()
        {
            CharBuffer duplicate = buf.Duplicate();
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
        public override void TestPutchar()
        {
            try
            {
                buf.Put((char)0);
                fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (ReadOnlyBufferException e)
            {
                // expected
            }
        }

        [Test]
        public override void TestPutcharArray()
        {
            char[] array = new char[1];
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
                buf.Put((char[])null);
                fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (ArgumentNullException e)
            {
                // expected
            }
        }

        [Test]
        public override void TestPutcharArrayintint()
        {
            char[] array = new char[1];
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
                buf.Put((char[])null, 0, 1);
                fail("Should throw ReadOnlyBufferException"); //$NON-NLS-1$
            }
            catch (ReadOnlyBufferException e)
            {
                // expected
            }
            try
            {
                buf.Put(new char[buf.Capacity + 1], 0, buf.Capacity + 1);
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
        public override void TestPutCharBuffer()
        {
            CharBuffer other = CharBuffer.Allocate(1);
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
                buf.Put((CharBuffer)null);
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
        public override void TestPutintchar()
        {
            try
            {
                buf.Put(0, (char)0);
                fail("Should throw ReadOnlyBufferException"); //$NON-NLS-1$
            }
            catch (ReadOnlyBufferException e)
            {
                // expected
            }
            try
            {
                buf.Put(-1, (char)0);
                fail("Should throw ReadOnlyBufferException"); //$NON-NLS-1$
            }
            catch (ReadOnlyBufferException e)
            {
                // expected
            }
        }

        [Test]
        public override void TestPutStringintint()
        {
            buf.Clear();
            String str = "" + new char[buf.Capacity];
            try
            {
                buf.Put(str, 0, str.Length);
                fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (ReadOnlyBufferException e)
            {
                // expected
            }
            try
            {
                buf.Put((String)null, 0, 0);
                fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (ArgumentNullException e)
            {
                // expected
            }
            try
            {
                buf.Put(str, -1, str.Length);
                fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (ArgumentOutOfRangeException e)
            {
                // expected
            }
            String longStr = "" + new char[buf.Capacity + 1];
            try
            {
                buf.Put(longStr, 0, longStr.Length);
                fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (ReadOnlyBufferException e)
            {
                // expected
            }
        }

        [Test]
        public override void TestPutString()
        {
            String str = " ";
            try
            {
                buf.Put(str);
                fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (ReadOnlyBufferException e)
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
    }
}
