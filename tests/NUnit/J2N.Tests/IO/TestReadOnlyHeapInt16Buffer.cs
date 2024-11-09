namespace J2N.IO
{
    public class TestReadOnlyHeapInt16Buffer : TestReadOnlyInt16Buffer
    {
        public override void SetUp()
        {
            base.SetUp();
            buf = Int16Buffer.Allocate(BUFFER_LENGTH);
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
