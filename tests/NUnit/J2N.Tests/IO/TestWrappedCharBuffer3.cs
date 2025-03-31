using NUnit.Framework;
using System;
using System.Text;

namespace J2N.IO
{
    internal class TestWrappedCharBuffer3 : TestReadOnlyCharBuffer
    {
        protected static readonly StringBuilder TEST_STRINGBUILDER = new StringBuilder("123456789abcdef12345");

        public override void SetUp()
        {
            base.SetUp();
            buf = CharBuffer.Wrap(TEST_STRINGBUILDER);
            baseBuf = buf;
        }

        public override void TearDown()
        {
            base.TearDown();
            baseBuf = null;
            buf = null;
        }

        [Test]
        public void TestWrappedCharSequence_IllegalArg()
        {
            StringBuilder str = TEST_STRINGBUILDER;
            try
            {
                CharBuffer.Wrap(str, -1, 0 - -1); // J2N: Corrected 3rd parameter
                fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (ArgumentOutOfRangeException e)
            {
                // expected
            }
            try
            {
                CharBuffer.Wrap(str, 21, 21 - 21); // J2N: Corrected 3rd parameter
                fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (ArgumentOutOfRangeException e)
            {
                // expected
            }
            try
            {
                CharBuffer.Wrap(str, 2, 1 - 2); // J2N: Corrected 3rd parameter
                fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (ArgumentOutOfRangeException e)
            {
                // expected
            }
            try
            {
                CharBuffer.Wrap(str, 0, 21 - 0); // J2N: Corrected 3rd parameter
                fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (ArgumentOutOfRangeException e)
            {
                // expected
            }
            try
            {
                CharBuffer.Wrap((String)null, -1, 21 - -1); // J2N: Corrected 3rd parameter
                fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (ArgumentNullException e)
            {
                // expected
            }
        }

        [Test]
        public override void TestArray()
        {
            try
            {
                var _ = buf.Array;
                fail("Should throw UnsupportedOperationException"); //$NON-NLS-1$
            }
            catch (NotSupportedException e)
            {
            }
        }

        [Test]
        public override void TestPutcharArrayintint()
        {
            char[] array = new char[1];
            try
            {
                buf.Put(array, 0, array.Length - 0); // J2N: Corrected 3rd parameter
                fail("Should throw ReadOnlyBufferException"); //$NON-NLS-1$
            }
            catch (ReadOnlyBufferException e)
            {
                // expected
            }
            try
            {
                buf.Put((char[])null, 0, 1 - 0); // J2N: Corrected 3rd parameter
                fail("Should throw NullPointerException"); //$NON-NLS-1$
            }
            catch (ArgumentNullException e)
            {
                // expected
            }
            try
            {
                buf.Put(new char[buf.Capacity + 1], 0, (buf.Capacity + 1) - 0); // J2N: Corrected 3rd parameter
                fail("Should throw BufferOverflowException"); //$NON-NLS-1$
            }
            catch (BufferOverflowException e)
            {
                // expected
            }
            try
            {
                buf.Put(array, -1, array.Length - -1); // J2N: Corrected 3rd parameter
                fail("Should throw IndexOutOfBoundsException"); //$NON-NLS-1$
            }
            catch (ArgumentOutOfRangeException e)
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
                fail("Should throw NullPointerException"); //$NON-NLS-1$
            }
            catch (ArgumentNullException e)
            {
                // expected
            }
            try
            {
                buf.Put(buf);
                fail("Should throw IllegalArgumentException"); //$NON-NLS-1$
            }
            catch (ArgumentException e)
            {
                // expected
            }
        }

    }
}
