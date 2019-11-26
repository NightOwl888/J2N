using NUnit.Framework;

namespace J2N
{
    public class TestBitConversion : TestCase
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


        public class JDKFloatBitwiseConversion
        {

            /**
             * A constant holding the smallest positive normal value of type
             * <code>float</code>, 2<sup>-126</sup>.  It is equal to the value
             * returned by <code>Float.intBitsToFloat(0x00800000)</code>.
             */
            public const float MIN_NORMAL = 1.17549435E-38f;

            /**
             * The number of logical bits in the significand of a
             * <code>float</code> number, including the implicit bit.
             */
            public const int SIGNIFICAND_WIDTH = 24;

            /**
             * Maximum exponent a finite <code>float</code> number may have.
             * It is equal to the value returned by
             * <code>Math.ilogb(Float.MAX_VALUE)</code>.
             */
            public const int MAX_EXPONENT = 127;

            /**
             * Minimum exponent a normalized <code>float</code> number may
             * have.  It is equal to the value returned by
             * <code>Math.ilogb(Float.MIN_NORMAL)</code>.
             */
            public const int MIN_EXPONENT = -126;

            /**
             * The exponent the smallest positive <code>float</code> subnormal
             * value would have if it could be normalized.  It is equal to the
             * value returned by <code>FpUtils.ilogb(Float.MIN_VALUE)</code>.
             */
            public const int MIN_SUB_EXPONENT = MIN_EXPONENT -
                                                           (SIGNIFICAND_WIDTH - 1);

            /**
             * Bias used in representing a <code>float</code> exponent.
             */
            public const int EXP_BIAS = 127;

            /**
             * Bit mask to isolate the sign bit of a <code>float</code>.
             */
            public const int SIGN_BIT_MASK = unchecked((int)0x80000000);

            /**
             * Bit mask to isolate the exponent field of a
             * <code>float</code>.
             */
            public const int EXP_BIT_MASK = 0x7F800000;

            /**
             * Bit mask to isolate the significand field of a
             * <code>float</code>.
             */
            public const int SIGNIF_BIT_MASK = 0x007FFFFF;

            [Test]
            public void Test_Constants_Sanity()
            {
                // verify bit masks cover all bit positions and that the bit
                // masks are non-overlapping
                assertTrue(((SIGN_BIT_MASK | EXP_BIT_MASK | SIGNIF_BIT_MASK) == ~0) &&
                       (((SIGN_BIT_MASK & EXP_BIT_MASK) == 0) &&
                        ((SIGN_BIT_MASK & SIGNIF_BIT_MASK) == 0) &&
                        ((EXP_BIT_MASK & SIGNIF_BIT_MASK) == 0)));
            }

            [Test]
            public void Test_SingleToInt32Bits_NaN()
            {
                for (int i = 0; i < SIGNIFICAND_WIDTH - 1; i++)
                {
                    int x = 1 << i;

                    // Strip out sign and exponent bits
                    int y = x & SIGNIF_BIT_MASK;

                    float[] values = {
                        BitConversion.Int32BitsToSingle(EXP_BIT_MASK | y),
                        BitConversion.Int32BitsToSingle(SIGN_BIT_MASK | EXP_BIT_MASK | y)
                    };

                    foreach (float value in values)
                    {
                        assertTrue("Invalid input " + y + "yielded non-NaN: " + value, float.IsNaN(value));
                        int converted = BitConversion.SingleToInt32Bits(value);
                        assertTrue(string.Format("Non-canoncial NaN bits returned: {0:x8}", converted), 0x7fc00000 == converted);
                    }

                    //testNanCase(1 << i);
                }
            }

            [Test]
            public void Test_SingleToInt32Bits_PositiveInfinity()
            {
                assertTrue("Bad conversion for +infinity.", BitConversion.SingleToInt32Bits(float.PositiveInfinity) == 0x7F800000);
            }

            [Test]
            public void Test_SingleToInt32Bits_NegativeInfinity()
            {
                assertTrue("Bad conversion for -infinity.", BitConversion.SingleToInt32Bits(float.NegativeInfinity) == unchecked((int)0xFF800000));
            }

            
        }

        public class JDKDoubleBitwiseConversion
        {
            /**
             * A constant holding the smallest positive normal value of type
             * <code>double</code>, 2<sup>-1022</sup>.  It is equal to the
             * value returned by
             * <code>Double.longBitsToDouble(0x0010000000000000L)</code>.
             *
             * @since 1.5
             */
            public const double MIN_NORMAL = 2.2250738585072014E-308;


            /**
             * The number of logical bits in the significand of a
             * <code>double</code> number, including the implicit bit.
             */
            public const int SIGNIFICAND_WIDTH = 53;

            /**
             * Maximum exponent a finite <code>double</code> number may have.
             * It is equal to the value returned by
             * <code>Math.ilogb(Double.MAX_VALUE)</code>.
             */
            public const int MAX_EXPONENT = 1023;

            /**
             * Minimum exponent a normalized <code>double</code> number may
             * have.  It is equal to the value returned by
             * <code>Math.ilogb(Double.MIN_NORMAL)</code>.
             */
            public const int MIN_EXPONENT = -1022;

            /**
             * The exponent the smallest positive <code>double</code>
             * subnormal value would have if it could be normalized.  It is
             * equal to the value returned by
             * <code>FpUtils.ilogb(Double.MIN_VALUE)</code>.
             */
            public const int MIN_SUB_EXPONENT = MIN_EXPONENT -
                                                           (SIGNIFICAND_WIDTH - 1);

            /**
             * Bias used in representing a <code>double</code> exponent.
             */
            public const int EXP_BIAS = 1023;

            /**
             * Bit mask to isolate the sign bit of a <code>double</code>.
             */
            public const long SIGN_BIT_MASK = unchecked((long)0x8000000000000000L);

            /**
             * Bit mask to isolate the exponent field of a
             * <code>double</code>.
             */
            public const long EXP_BIT_MASK = 0x7FF0000000000000L;

            /**
             * Bit mask to isolate the significand field of a
             * <code>double</code>.
             */
            public const long SIGNIF_BIT_MASK = 0x000FFFFFFFFFFFFFL;

            [Test]
            public void Test_Constants_Sanity()
            {
                // verify bit masks cover all bit positions and that the bit
                // masks are non-overlapping
                assertTrue(((SIGN_BIT_MASK | EXP_BIT_MASK | SIGNIF_BIT_MASK) == ~0) &&
                       (((SIGN_BIT_MASK & EXP_BIT_MASK) == 0) &&
                        ((SIGN_BIT_MASK & SIGNIF_BIT_MASK) == 0) &&
                        ((EXP_BIT_MASK & SIGNIF_BIT_MASK) == 0)));
            }

            [Test]
            public void Test_DoubleToInt64Bits_NaN()
            {
                for (int i = 0; i < SIGNIFICAND_WIDTH - 1; i++)
                {
                    long x = 1 << i;

                    // Strip out sign and exponent bits
                    long y = x & SIGNIF_BIT_MASK;

                    double[] values = {
                        BitConversion.Int64BitsToDouble(EXP_BIT_MASK | y),
                        BitConversion.Int64BitsToDouble(SIGN_BIT_MASK | EXP_BIT_MASK | y)
                    };

                    foreach (double value in values)
                    {
                        assertTrue("Invalid input " + y + "yielded non-NaN: " + value, double.IsNaN(value));
                        long converted = BitConversion.DoubleToInt64Bits(value);
                        assertTrue(string.Format("Non-canoncial NaN bits returned: {0:x8}", converted), 0x7ff8000000000000L == converted);
                    }

                    //testNanCase(1 << i);
                }
            }

            [Test]
            public void Test_DoubleToInt64Bits_PositiveInfinity()
            {
                assertTrue("Bad conversion for +infinity.", BitConversion.DoubleToInt64Bits(double.PositiveInfinity) == 0x7ff0000000000000L);
            }

            [Test]
            public void Test_DoubleToInt64Bits_NegativeInfinity()
            {
                assertTrue("Bad conversion for -infinity.", BitConversion.DoubleToInt64Bits(double.NegativeInfinity) == unchecked((long)0xfff0000000000000L));
            }
        }
    }
}
