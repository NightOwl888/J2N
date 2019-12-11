namespace J2N.IO
{
    public class TestReadOnlyWrappedSingleBuffer : TestReadOnlySingleBuffer
    {
        public override void SetUp()
        {
            base.SetUp();
            buf = SingleBuffer.Wrap(new float[BUFFER_LENGTH]);
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
