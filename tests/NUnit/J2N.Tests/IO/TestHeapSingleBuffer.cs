using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace J2N.IO
{
    public class TestHeapSingleBuffer : TestSingleBuffer
    {
        public override void SetUp()
        {
            base.SetUp();
            buf = SingleBuffer.Allocate(BUFFER_LENGTH);
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
        public void TestAllocatedFloatBuffer_IllegalArg()
        {
            try
            {
                SingleBuffer.Allocate(-1);
                fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (ArgumentException e)
            {
                // expected
            }
        }
    }
}
