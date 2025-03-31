﻿using NUnit.Framework;
using System;

namespace J2N.IO
{
    public class TestReadOnlyInt64Buffer : TestInt64Buffer
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
            Int64Buffer duplicate = buf.Duplicate();
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
        public override void TestPutlong()
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
        public override void TestPutlongSpan() // J2N specific
        {
            Span<long> array = new long[1];
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
            //    buf.Put((long[])null);
            //    fail("Should throw Exception"); //$NON-NLS-1$
            //}
            //catch (ArgumentNullException e)
            //{
            //    // expected
            //}
        }

        [Test]
        public override void TestPutlongArray()
        {
            long[] array = new long[1];
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
                buf.Put((long[])null);
                fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (ArgumentNullException e)
            {
                // expected
            }
        }

        [Test]
        public override void TestPutlongArrayintint()
        {
            long[] array = new long[1];
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
                buf.Put((long[])null, 0, 1);
                fail("Should throw ReadOnlyBufferException"); //$NON-NLS-1$
            }
            catch (ReadOnlyBufferException e)
            {
                // expected
            }
            try
            {
                buf.Put(new long[buf.Capacity + 1], 0, buf.Capacity + 1);
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
        public override void TestPutInt64Buffer()
        {
            Int64Buffer other = Int64Buffer.Allocate(1);
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
                buf.Put((Int64Buffer)null);
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
        public override void TestPutintlong()
        {
            try
            {
                buf.Put(0, (long)0);
                fail("Should throw ReadOnlyBufferException"); //$NON-NLS-1$
            }
            catch (ReadOnlyBufferException e)
            {
                // expected
            }
            try
            {
                buf.Put(-1, (long)0);
                fail("Should throw ReadOnlyBufferException"); //$NON-NLS-1$
            }
            catch (ReadOnlyBufferException e)
            {
                // expected
            }
        }
    }
}
