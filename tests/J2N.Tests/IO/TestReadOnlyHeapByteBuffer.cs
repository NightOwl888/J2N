using NUnit.Framework;

namespace J2N.IO
{
    // This class was sourced from the Apache Harmony project
    // https://svn.apache.org/repos/asf/harmony/enhanced/java/trunk/

    public class TestReadOnlyHeapByteBuffer : TestByteBuffer
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
