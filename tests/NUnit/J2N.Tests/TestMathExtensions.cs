using NUnit.Framework;

namespace J2N
{
    public class TestMathExtensions : TestCase
    {
        /**
         * @tests java.lang.Integer#signum(int)
         */
        [Test]
        public void Test_Signum_Int32()
        {
            for (int i = -128; i < 0; i++)
            {
                assertEquals(-1, MathExtensions.Signum(i));
            }
            assertEquals(0, MathExtensions.Signum(0));
            for (int i = 1; i <= 127; i++)
            {
                assertEquals(1, MathExtensions.Signum(i));
            }
        }

        /**
         * @tests java.lang.Long#signum(long)
         */
        [Test]
        public void Test_Signum_Int64()
        {
            for (int i = -128; i < 0; i++)
            {
                assertEquals(-1, MathExtensions.Signum(i));
            }
            assertEquals(0, MathExtensions.Signum(0));
            for (int i = 1; i <= 127; i++)
            {
                assertEquals(1, MathExtensions.Signum(i));
            }
        }

        /**
         * @tests java.lang.Math#toRadians(double)
         */
        [Test]
        public void Test_ToRadians_Double()
        {
            for (double d = 500; d >= 0; d -= 1.0)
            {
                double converted = MathExtensions.ToDegrees(MathExtensions.ToRadians(d));
                assertTrue("Converted number not equal to original. d = " + d,
                        converted >= d * 0.99999999 && converted <= d * 1.00000001);
            }
        }

        /**
         * @tests java.lang.Math#toDegrees(double)
         */
        [Test]
        public void Test_ToDegrees_Double()
        {
            for (double d = 500; d >= 0; d -= 1.0)
            {
                double converted = MathExtensions.ToRadians(MathExtensions.ToDegrees(d));
                assertTrue("Converted number not equal to original. d = " + d,
                        converted >= d * 0.99999999 && converted <= d * 1.00000001);
            }
        }

        /**
         * @tests java.lang.Math#toRadians(double)
         */
        [Test]
        public void Test_ToRadians_Decimal()
        {
            for (decimal d = 500; d >= 0; d -= 1.0M)
            {
                decimal converted = MathExtensions.ToDegrees(MathExtensions.ToRadians(d));
                assertTrue("Converted number not equal to original. d = " + d,
                        converted >= d * 0.99999999M && converted <= d * 1.00000001M);
            }
        }

        /**
         * @tests java.lang.Math#toDegrees(double)
         */
        [Test]
        public void Test_ToDegrees_Decimal()
        {
            for (decimal d = 500; d >= 0; d -= 1.0M)
            {
                decimal converted = MathExtensions.ToRadians(MathExtensions.ToDegrees(d));
                assertTrue("Converted number not equal to original. d = " + d,
                        converted >= d * 0.99999999M && converted <= d * 1.00000001M);
            }
        }

        /**
         * @tests java.lang.Math#toRadians(double)
         */
        [Test]
        public void Test_ToRadians_Int32()
        {
            for (int d = 500; d >= 0; d -= 1)
            {
                double converted = MathExtensions.ToDegrees(MathExtensions.ToRadians(d));
                assertTrue("Converted number not equal to original. d = " + d,
                        converted >= d * 0.99999999 && converted <= d * 1.00000001);
            }
        }

        /**
         * @tests java.lang.Math#toDegrees(double)
         */
        [Test]
        public void Test_ToDegrees_Int32()
        {
            for (int d = 500; d >= 0; d -= 1)
            {
                double converted = MathExtensions.ToRadians(MathExtensions.ToDegrees(d));
                assertTrue("Converted number not equal to original. d = " + d,
                        converted >= d * 0.99999999 && converted <= d * 1.00000001);
            }
        }
    }
}
