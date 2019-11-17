using NUnit.Framework;

namespace J2N
{
    [TestFixture]
    public class TestBitConversion
    {
        // Tests from Apache Harmony

        /**
         * @tests java.lang.Float#floatToIntBits(float)
         */
        [Test]
        public void Test_SingleToInt32BitsF()
        {
            float f = 9876.2345f;
            int bits = BitConversion.SingleToInt32Bits(f);
            float r = BitConversion.Int32BitsToSingle(bits);
            Assert.IsTrue(f == r, "Incorrect intBits returned");
        }

        /**
         * @tests java.lang.Float#floatToRawIntBits(float)
         */
        [Test]
        public void Test_SingleToRawInt32BitsF()
        {
            int i = 0x7fc004d2;
            float f = BitConversion.Int32BitsToSingle(i);
            Assert.IsTrue(BitConversion.SingleToRawInt32Bits(f) == i, "Wrong raw bits");
        }

        /**
         * @tests java.lang.Float#intBitsToFloat(int)
         */
        [Test]
        public void Test_Int32BitsToSingleI()
        {
            float f = 9876.2345f;
            int bits = BitConversion.SingleToInt32Bits(f);
            float r = BitConversion.Int32BitsToSingle(bits);
            Assert.AreEqual(f, r, 0F, "Incorrect intBits returned");
        }

        /**
         * @tests java.lang.Double#doubleToLongBits(double)
         */
        [Test]
        public void Test_DoubleToInt64BitsD()
        {
            // Test for method long java.lang.Double.doubleToLongBits(double)
            double d = double.MaxValue;
            long lbits = BitConversion.DoubleToInt64Bits(d);
            double r = BitConversion.Int64BitsToDouble(lbits);

            Assert.IsTrue(d == r, "Bit conversion failed");
        }

        /**
         * @tests java.lang.Double#doubleToRawLongBits(double)
         */
        [Test]
        public void Test_DoubleToRawInt64BitsD()
        {
            long l = 0x7ff80000000004d2L;
            double d = BitConversion.Int64BitsToDouble(l);
            Assert.IsTrue(BitConversion.DoubleToRawInt64Bits(d) == l, "Wrong raw bits");
        }

        /**
         * @tests java.lang.Double#longBitsToDouble(long)
         */
        [Test]
        public void Test_Int64BitsToDoubleJ()
        {
            // Test for method double java.lang.Double.longBitsToDouble(long)

            double d = double.MaxValue;
            long lbits = BitConversion.DoubleToInt64Bits(d);
            double r = BitConversion.Int64BitsToDouble(lbits);

            Assert.IsTrue(d == r, "Bit conversion failed");
        }
    }
}
