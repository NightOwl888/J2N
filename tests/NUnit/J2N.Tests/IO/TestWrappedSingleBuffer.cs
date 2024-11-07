using NUnit.Framework;
using System;

namespace J2N.IO
{
    public class TestWrappedSingleBuffer : TestSingleBuffer
    {
        public override void SetUp()
        {
            base.SetUp();
            buf = SingleBuffer.Wrap(new float[BUFFER_LENGTH]);
            loadTestData1(buf);
            baseBuf = buf;
        }

        public override void TearDown()
        {
            base.TearDown();
            baseBuf = null;
            buf = null;
        }

        /**
         * @tests java.nio.CharBuffer#allocate(char[],int,int)
         * 
         */
        [Test]
        public void TestWrappedFloatBuffer_IllegalArg()
        {
            float[] array = new float[20];
            try
            {
                SingleBuffer.Wrap(array, -1, 0);
                fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (ArgumentOutOfRangeException e)
            {
                // expected
            }
            try
            {
                SingleBuffer.Wrap(array, 21, 0);
                fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (ArgumentOutOfRangeException e)
            {
                // expected
            }
            try
            {
                SingleBuffer.Wrap(array, 0, -1);
                fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (ArgumentOutOfRangeException e)
            {
                // expected
            }
            try
            {
                SingleBuffer.Wrap(array, 0, 21);
                fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (ArgumentOutOfRangeException e)
            {
                // expected
            }
            try
            {
                SingleBuffer.Wrap(array, int.MaxValue, 1);
                fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (ArgumentOutOfRangeException e)
            {
                // expected
            }
            try
            {
                SingleBuffer.Wrap(array, 1, int.MaxValue);
                fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (ArgumentOutOfRangeException e)
            {
                // expected
            }
            try
            {
                SingleBuffer.Wrap((float[])null, -1, 0);
                fail("Should throw NPE"); //$NON-NLS-1$
            }
            catch (ArgumentNullException e)
            {
            }

            SingleBuffer buf = SingleBuffer.Wrap(array, 2, 16);
            assertEquals(buf.Position, 2);
            assertEquals(buf.Limit, 18);
            assertEquals(buf.Capacity, 20);
        }
    }
}
