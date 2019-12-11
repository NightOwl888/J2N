namespace J2N.IO
{
    public class TestReadOnlyHeapInt32Buffer : TestReadOnlyInt32Buffer
    {
        public override void SetUp()
        {
            base.SetUp();
            buf = Int32Buffer.Allocate(BUFFER_LENGTH);
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
