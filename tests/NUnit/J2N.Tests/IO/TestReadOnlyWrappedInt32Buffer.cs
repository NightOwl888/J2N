namespace J2N.IO
{
    public class TestReadOnlyWrappedInt32Buffer : TestReadOnlyInt32Buffer
    {
        public override void SetUp()
        {
            base.SetUp();
            buf = Int32Buffer.Wrap(new int[BUFFER_LENGTH]);
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
