using NUnit.Framework;
using System;

namespace J2N.IO
{
    public class TestWrappedCharBuffer1 : TestCharBuffer
    {
        public override void SetUp()
        {
            base.SetUp();
            buf = CharBuffer.Wrap(new char[BUFFER_LENGTH]);
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
        public void TestWrappedCharBuffer_IllegalArg()
        {
            char[] array = new char[BUFFER_LENGTH];
            try
            {
                CharBuffer.Wrap(array, -1, 0);
                fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (ArgumentOutOfRangeException e)
            {
                // expected
            }
            try
            {
                CharBuffer.Wrap(array, BUFFER_LENGTH + 1, 0);
                fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (ArgumentOutOfRangeException e)
            {
                // expected
            }
            try
            {
                CharBuffer.Wrap(array, 0, -1);
                fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (ArgumentOutOfRangeException e)
            {
                // expected
            }
            try
            {
                CharBuffer.Wrap(array, 0, BUFFER_LENGTH + 1);
                fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (ArgumentOutOfRangeException e)
            {
                // expected
            }
            try
            {
                CharBuffer.Wrap(array, int.MaxValue, 1);
                fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (ArgumentOutOfRangeException e)
            {
                // expected
            }
            try
            {
                CharBuffer.Wrap(array, 1, int.MaxValue);
                fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (ArgumentOutOfRangeException e)
            {
                // expected
            }
            try
            {
                CharBuffer.Wrap((char[])null, -1, 0);
                fail("Should throw NPE"); //$NON-NLS-1$
            }
            catch (ArgumentNullException e)
            {
            }
        }
    }
}
