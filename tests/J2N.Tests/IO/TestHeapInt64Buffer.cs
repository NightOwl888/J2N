using NUnit.Framework;
using System;

namespace J2N.IO
{
    public class TestHeapInt64Buffer : TestInt64Buffer
    {
        public override void SetUp()
        {
            base.SetUp();
            buf = Int64Buffer.Allocate(BUFFER_LENGTH);
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
        public void TestAllocatedLongBuffer_IllegalArg()
        {
            try
            {
                Int64Buffer.Allocate(-1);
                fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (ArgumentException e)
            {
                // expected
            }
        }
    }
}
