namespace J2N.IO
{
    public class TestReadOnlyWrappedCharBuffer1 : TestReadOnlyCharBuffer
    {
        public override void SetUp()
        {
            base.SetUp();
            buf = CharBuffer.Wrap(new char[BUFFER_LENGTH]);
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
