namespace J2N.IO
{
    public class TestDuplicateWrappedByteBuffer : TestWrappedByteBuffer
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
