using NUnit.Framework;
using System;

namespace J2N.IO
{
    public class TestHeapDoubleBuffer : TestDoubleBuffer
    {
        public override void SetUp()
        {
            base.SetUp();
            buf = DoubleBuffer.Allocate(BUFFER_LENGTH);
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
        public void TestAllocatedDoubleBuffer_IllegalArg()
        {
            try
            {
                DoubleBuffer.Allocate(-1);
                fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (ArgumentException e)
            {
                // expected
            }
        }
    }
}
