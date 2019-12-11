using NUnit.Framework;
using System;

namespace J2N.IO
{
    public class TestWrappedInt32Buffer : TestInt32Buffer
    {
        public override void SetUp()
        {
            base.SetUp();
            buf = Int32Buffer.Wrap(new int[BUFFER_LENGTH]);
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
        public void TestWrappedIntBuffer_IllegalArg()
        {
            int[] array = new int[20];
            try
            {
                Int32Buffer.Wrap(array, -1, 0);
                fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (ArgumentOutOfRangeException e)
            {
                // expected
            }
            try
            {
                Int32Buffer.Wrap(array, 21, 0);
                fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (ArgumentOutOfRangeException e)
            {
                // expected
            }
            try
            {
                Int32Buffer.Wrap(array, 0, -1);
                fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (ArgumentOutOfRangeException e)
            {
                // expected
            }
            try
            {
                Int32Buffer.Wrap(array, 0, 21);
                fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (ArgumentOutOfRangeException e)
            {
                // expected
            }
            try
            {
                Int32Buffer.Wrap(array, int.MaxValue, 1);
                fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (ArgumentOutOfRangeException e)
            {
                // expected
            }
            try
            {
                Int32Buffer.Wrap(array, 1, int.MaxValue);
                fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (ArgumentOutOfRangeException e)
            {
                // expected
            }
            try
            {
                Int32Buffer.Wrap((int[])null, -1, 0);
                fail("Should throw NPE"); //$NON-NLS-1$
            }
            catch (ArgumentNullException e)
            {
            }

            Int32Buffer buf = Int32Buffer.Wrap(array, 2, 16);
            assertEquals(buf.Position, 2);
            assertEquals(buf.Limit, 18);
            assertEquals(buf.Capacity, 20);
        }
    }
}
