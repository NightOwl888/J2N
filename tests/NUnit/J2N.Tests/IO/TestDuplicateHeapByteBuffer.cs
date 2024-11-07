namespace J2N.IO
{
    public class TestDuplicateHeapByteBuffer : TestHeapByteBuffer
    {
        public override void SetUp()
        {
            base.SetUp();
            buf = buf.Duplicate();
            baseBuf = buf;
        }

        public override void TearDown()
        {
            base.TearDown();
        }
    }
}
