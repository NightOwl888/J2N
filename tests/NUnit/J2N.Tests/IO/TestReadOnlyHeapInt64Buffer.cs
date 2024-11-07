namespace J2N.IO
{
    public class TestReadOnlyHeapInt64Buffer : TestReadOnlyInt64Buffer
    {
        public override void SetUp()
        {
            base.SetUp();
            buf = Int64Buffer.Allocate(BUFFER_LENGTH);
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
