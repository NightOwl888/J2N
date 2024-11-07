using NUnit.Framework;
using System;

namespace J2N.IO
{
    public class TestHeapInt16Buffer : TestInt16Buffer
    {
        public override void SetUp()
        {
            base.SetUp();
            buf = Int16Buffer.Allocate(BUFFER_LENGTH);
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
        public void TestAllocatedShortBuffer_IllegalArg()
        {
            try
            {
                Int16Buffer.Allocate(-1);
                fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (ArgumentException e)
            {
                // expected
            }
        }
    }
}
