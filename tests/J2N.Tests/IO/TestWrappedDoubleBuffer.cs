using NUnit.Framework;
using System;

namespace J2N.IO
{
    public class TestWrappedDoubleBuffer : TestDoubleBuffer
    {
        public override void SetUp()
        {
            base.SetUp();
            buf = DoubleBuffer.Wrap(new double[BUFFER_LENGTH]);
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
        public void TestWrappedDoubleuffer_IllegalArg()
        {
            double[] array = new double[20];
            try
            {
                DoubleBuffer.Wrap(array, -1, 0);
                fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (ArgumentOutOfRangeException e)
            {
                // expected
            }
            try
            {
                DoubleBuffer.Wrap(array, 21, 0);
                fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (ArgumentOutOfRangeException e)
            {
                // expected
            }
            try
            {
                DoubleBuffer.Wrap(array, 0, -1);
                fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (ArgumentOutOfRangeException e)
            {
                // expected
            }
            try
            {
                DoubleBuffer.Wrap(array, 0, 21);
                fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (ArgumentOutOfRangeException e)
            {
                // expected
            }
            try
            {
                DoubleBuffer.Wrap(array, int.MaxValue, 1);
                fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (ArgumentOutOfRangeException e)
            {
                // expected
            }
            try
            {
                DoubleBuffer.Wrap(array, 1, int.MaxValue);
                fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (ArgumentOutOfRangeException e)
            {
                // expected
            }
            try
            {
                DoubleBuffer.Wrap((double[])null, -1, 0);
                fail("Should throw NPE"); //$NON-NLS-1$
            }
            catch (ArgumentNullException e)
            {
            }

            DoubleBuffer buf = DoubleBuffer.Wrap(array, 2, 16);
            assertEquals(buf.Position, 2);
            assertEquals(buf.Limit, 18);
            assertEquals(buf.Capacity, 20);
        }
    }
}
