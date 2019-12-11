namespace J2N.IO
{
    public class TestReadOnlyWrappedInt64Buffer : TestReadOnlyInt64Buffer
    {
        public override void SetUp()
        {
            base.SetUp();
            buf = Int64Buffer.Wrap(new long[BUFFER_LENGTH]);
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
