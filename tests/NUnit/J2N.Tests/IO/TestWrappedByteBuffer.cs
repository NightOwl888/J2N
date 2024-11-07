using NUnit.Framework;
using System;

namespace J2N.IO
{
    public class TestWrappedByteBuffer : TestByteBuffer
    {
        public override void SetUp()
        {
            base.SetUp();
            buf = ByteBuffer.Wrap(new byte[BUFFER_LENGTH]);
            baseBuf = buf;
        }

        public override void TearDown()
        {
            base.TearDown();
            buf = null;
            baseBuf = null;
        }

        /**
         * @tests java.nio.ByteBuffer#allocate(byte[],int,int)
         * 
         */
        [Test]
        public void TestWrappedByteBuffer_IllegalArg()
        {
            byte[] array = new byte[BUFFER_LENGTH];
            try
            {
                ByteBuffer.Wrap(array, -1, 0);
                fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (ArgumentOutOfRangeException e)
            {
                // expected
            }
            try
            {
                ByteBuffer.Wrap(array, BUFFER_LENGTH + 1, 0);
                fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (ArgumentOutOfRangeException e)
            {
                // expected
            }
            try
            {
                ByteBuffer.Wrap(array, 0, -1);
                fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (ArgumentOutOfRangeException e)
            {
                // expected
            }
            try
            {
                ByteBuffer.Wrap(array, 0, BUFFER_LENGTH + 1);
                fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (ArgumentOutOfRangeException e)
            {
                // expected
            }
            try
            {
                ByteBuffer.Wrap(array, 1, int.MaxValue);
                fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (ArgumentOutOfRangeException e)
            {
                // expected
            }
            try
            {
                ByteBuffer.Wrap(array, int.MaxValue, 1);
                fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (ArgumentOutOfRangeException e)
            {
                // expected
            }
            try
            {
                ByteBuffer.Wrap((byte[])null, 1, int.MaxValue);
                fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (ArgumentNullException e)
            {
                // expected
            }
        }

        //[Test]
        //public void TestIsDirect() // J2N: IsDirect not supported
        //{
        //    assertFalse(buf.IsDirect);
        //}

        [Test]
        public override void TestHasArray()
        {
            assertTrue(buf.HasArray);
        }

        [Test]
        public override void TestIsReadOnly()
        {
            assertFalse(buf.IsReadOnly);
        }
    }
}
