using NUnit.Framework;
using System;

namespace J2N.IO
{
    public class TestHeapCharBuffer : TestCharBuffer
    {
        public override void SetUp()
        {
            base.SetUp();
            buf = CharBuffer.Allocate(BUFFER_LENGTH);
            loadTestData1(buf);
            baseBuf = buf;
        }

        public override void TearDown()
        {
            base.TearDown();
            buf = null;
            baseBuf = null;
        }

        [Test]
        public void TestAllocatedCharBuffer_IllegalArg()
        {
            try
            {
                CharBuffer.Allocate(-1);
                fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (ArgumentException e)
            {
                // expected
            }
        }
    }
}
