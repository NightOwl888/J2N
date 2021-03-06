using NUnit.Framework;
using System;

namespace J2N.IO
{
    public class TestHeapByteBuffer : TestByteBuffer
    {
        public override void SetUp()
        {
            base.SetUp();
            buf = ByteBuffer.Allocate(BUFFER_LENGTH);
            baseBuf = buf;
        }

        public override void TearDown()
        {
            base.TearDown();
            buf = null;
            baseBuf = null;
        }

        [Test]
        public void TestAllocatedByteBuffer_IllegalArg()
        {
            try
            {
                ByteBuffer.Allocate(-1);
                fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (ArgumentException e)
            {
                // expected 
            }
        }

        //[Test]
        //public override void TestIsDirect() // J2N: IsDirect not supported
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
