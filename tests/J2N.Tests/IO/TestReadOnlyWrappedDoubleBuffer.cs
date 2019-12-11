namespace J2N.IO
{
    public class TestReadOnlyWrappedDoubleBuffer : TestReadOnlyDoubleBuffer
    {
        public override void SetUp()
        {
            base.SetUp();
            buf = DoubleBuffer.Wrap(new double[BUFFER_LENGTH]);
            base.loadTestData1(buf);
            buf = buf.AsReadOnlyBuffer();
            baseBuf = buf;
        }

        public override void TearDown()
        {
            base.TearDown();
        }
    }
}
