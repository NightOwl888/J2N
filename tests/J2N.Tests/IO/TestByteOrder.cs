using NUnit.Framework;

namespace J2N.IO
{
    [TestFixture]
    public class TestByteOrder
    {
        [Test]
        public void TestToString()
        {
            Assert.AreEqual("BigEndian", ByteOrder.BigEndian.ToString());
            Assert.AreEqual("LittleEndian", ByteOrder.LittleEndian.ToString());
        }

        [Test]
        public void TestNativeOrder()
        {
            ByteOrder o = ByteOrder.NativeOrder;
            Assert.IsTrue(o == ByteOrder.BigEndian || o == ByteOrder.LittleEndian);
        }
    }
}
