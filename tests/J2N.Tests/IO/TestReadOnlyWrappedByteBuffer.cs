using NUnit.Framework;

namespace J2N.IO
{
    public class TestReadOnlyWrappedByteBuffer : TestWrappedByteBuffer
    {
        public override void SetUp()
        {
            base.SetUp();
            buf = buf.AsReadOnlyBuffer();
            baseBuf = buf;
        }

        public override void TearDown()
        {
            base.TearDown();
        }

        [Test]
        public override void TestIsReadOnly()
        {
            assertTrue(buf.IsReadOnly);
        }

        [Test]
        public override void TestHasArray()
        {
            assertFalse(buf.HasArray);
        }

        [Test]
        public override void TestHashCode()
        {
            base.readOnlyHashCode();
        }
    }
}
