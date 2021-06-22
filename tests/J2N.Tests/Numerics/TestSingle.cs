using J2N.Globalization;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace J2N.Numerics
{
    public class TestSingle : TestCase
    {
        private static readonly int[] rawBitsFor3_4eN38To38 = { 0x1394470, 0x2e7958c, 0x490bd77, 0x634ecd5,
            0x7e2280b, 0x98d5907, 0xb30af48, 0xcdcdb1a, 0xe8a08f0, 0x102c8b2d, 0x11d7adf8,
            0x1386ccbb, 0x15287fe9, 0x16d29fe4, 0x1883a3ee, 0x1a248cea, 0x1bcdb025, 0x1d808e17,
            0x1f20b19d, 0x20c8de04, 0x227b1585, 0x241ced73, 0x25c428d0, 0x27753303, 0x29193fe2,
            0x2abf8fdb, 0x2c6f73d1, 0x2e15a863, 0x2fbb127c, 0x3169d71a, 0x33122671, 0x34b6b00d,
            0x36645c10, 0x380eb98a, 0x39b267ec, 0x3b5f01e8, 0x3d0b6131, 0x3eae397d, 0x4059c7dc,
            0x42081cea, 0x43aa2424, 0x4554ad2d, 0x4704ec3c, 0x48a6274b, 0x4a4fb11e, 0x4c01ceb3,
            0x4da2425f, 0x4f4ad2f7, 0x50fd87b5, 0x529e74d1, 0x54461205, 0x55f79687, 0x579abe14,
            0x59416d99, 0x5af1c900, 0x5c971da0, 0x5e3ce508, 0x5fec1e4a, 0x619392ee, 0x633877a9,
            0x64e69594, 0x66901d7c, 0x683424dc, 0x69e12e12, 0x6b8cbccb, 0x6d2febfe, 0x6edbe6fe,
            0x7089705f, 0x722bcc76, 0x73d6bf94, 0x758637bc, 0x7727c5ac, 0x78d1b717, 0x7a83126e,
            0x7c23d70a, 0x7dcccccc, 0x7f7fffff };

        private static readonly string[] expectedStringFor3_4eN38To38 = { "3.4028235E-38", "3.4028235E-37",
            "3.4028233E-36", "3.4028234E-35", "3.4028236E-34", "3.4028236E-33",
            "3.4028234E-32", "3.4028234E-31", "3.4028233E-30", "3.4028236E-29",
            "3.4028235E-28", "3.4028235E-27", "3.4028233E-26", "3.4028235E-25",
            "3.4028233E-24", "3.4028235E-23", "3.4028236E-22", "3.4028235E-21",
            "3.4028236E-20", "3.4028236E-19", "3.4028236E-18", "3.4028235E-17",
            "3.4028236E-16", "3.4028234E-15", "3.4028234E-14", "3.4028235E-13",
            "3.4028234E-12", "3.4028235E-11", "3.4028236E-10", "3.4028234E-9", "3.4028236E-8",
            "3.4028236E-7", "3.4028235E-6", "3.4028235E-5", "3.4028233E-4", "0.0034028236",
            "0.034028236", "0.34028235", "3.4028234", "34.028236", "340.28235", "3402.8235",
            "34028.234", "340282.34", "3402823.5", "3.4028236E7", "3.40282336E8",
            "3.40282342E9", "3.40282348E10", "3.40282343E11", "3.40282337E12", "3.40282353E13",
            "3.4028234E14", "3.4028234E15", "3.40282356E16", "3.40282356E17", "3.40282356E18",
            "3.4028236E19", "3.4028235E20", "3.4028233E21", "3.4028235E22", "3.4028233E23",
            "3.4028236E24", "3.4028234E25", "3.4028233E26", "3.4028234E27", "3.4028235E28",
            "3.4028236E29", "3.4028233E30", "3.4028235E31", "3.4028233E32", "3.4028236E33",
            "3.4028236E34", "3.4028234E35", "3.4028236E36", "3.4028235E37", "3.4028235E38" };

        private static readonly int[] rawBitsFor1_17eN38To38 = { unchecked((int)0x80800000), unchecked((int)0x82200000), unchecked((int)0x83c80000),
            unchecked((int)0x857a0000), unchecked((int)0x871c4000), unchecked((int)0x88c35000), unchecked((int)0x8a742400), unchecked((int)0x8c189680), unchecked((int)0x8dbebc20), unchecked((int)0x8f6e6b28),
            unchecked((int)0x911502f9), unchecked((int)0x92ba43b7), unchecked((int)0x9468d4a5), unchecked((int)0x961184e7), unchecked((int)0x97b5e621), unchecked((int)0x99635fa9), unchecked((int)0x9b0e1bca),
            unchecked((int)0x9cb1a2bc), unchecked((int)0x9e5e0b6b), unchecked((int)0xa00ac723), unchecked((int)0xa1ad78ec), unchecked((int)0xa358d727), unchecked((int)0xa5078678), unchecked((int)0xa6a96816),
            unchecked((int)0xa853c21c), unchecked((int)0xaa045951), unchecked((int)0xaba56fa6), unchecked((int)0xad4ecb8f), unchecked((int)0xaf013f39), unchecked((int)0xb0a18f08), unchecked((int)0xb249f2ca),
            unchecked((int)0xb3fc6f7c), unchecked((int)0xb59dc5ae), unchecked((int)0xb7453719), unchecked((int)0xb8f684df), unchecked((int)0xba9a130c), unchecked((int)0xbc4097ce), unchecked((int)0xbdf0bdc2),
            unchecked((int)0xbf967699), unchecked((int)0xc13c1440), unchecked((int)0xc2eb1950), unchecked((int)0xc492efd2), unchecked((int)0xc637abc6), unchecked((int)0xc7e596b8), unchecked((int)0xc98f7e33),
            unchecked((int)0xcb335dc0), unchecked((int)0xcce0352f), unchecked((int)0xce8c213e), unchecked((int)0xd02f298d), unchecked((int)0xd1daf3f0), unchecked((int)0xd388d876), unchecked((int)0xd52b0e94),
            unchecked((int)0xd6d5d239), unchecked((int)0xd885a363), unchecked((int)0xda270c3c), unchecked((int)0xdbd0cf4b), unchecked((int)0xdd82818f), unchecked((int)0xdf2321f3), unchecked((int)0xe0cbea70),
            unchecked((int)0xe27ee50b), unchecked((int)0xe41f4f27), unchecked((int)0xe5c722f1), unchecked((int)0xe778ebad), unchecked((int)0xe91b934c), unchecked((int)0xeac2781f), unchecked((int)0xec731627),
            unchecked((int)0xee17edd8), unchecked((int)0xefbde94f), unchecked((int)0xf16d63a2), unchecked((int)0xf3145e45), unchecked((int)0xf4b975d7), unchecked((int)0xf667d34c), unchecked((int)0xf810e410),
            unchecked((int)0xf9b51d14), unchecked((int)0xfb626459), unchecked((int)0xfd0d7eb7), unchecked((int)0xfeb0de65) };

        private static readonly string[] expectedStringFor1_17eN38To38 = { "-1.17549435E-38",
            "-1.1754944E-37", "-1.17549435E-36", "-1.17549435E-35", "-1.1754944E-34",
            "-1.17549435E-33", "-1.17549435E-32", "-1.1754944E-31", "-1.17549435E-30",
            "-1.17549435E-29", "-1.1754944E-28", "-1.1754943E-27", "-1.17549435E-26",
            "-1.1754943E-25", "-1.1754944E-24", "-1.1754943E-23", "-1.1754944E-22",
            "-1.1754943E-21", "-1.1754943E-20", "-1.1754943E-19", "-1.1754944E-18",
            "-1.1754944E-17", "-1.1754943E-16", "-1.1754943E-15", "-1.1754944E-14",
            "-1.1754943E-13", "-1.1754944E-12", "-1.1754943E-11", "-1.1754943E-10",
            "-1.1754944E-9", "-1.1754944E-8", "-1.1754943E-7", "-1.1754944E-6",
            "-1.1754943E-5", "-1.1754943E-4", "-0.0011754944", "-0.011754943", "-0.117549434",
            "-1.1754943", "-11.754944", "-117.54944", "-1175.4944", "-11754.943", "-117549.44",
            "-1175494.4", "-1.1754944E7", "-1.17549432E8", "-1.1754944E9", "-1.17549435E10",
            "-1.17549433E11", "-1.17549433E12", "-1.17549438E13", "-1.17549438E14",
            "-1.1754943E15", "-1.17549432E16", "-1.17549432E17", "-1.17549434E18",
            "-1.1754944E19", "-1.1754944E20", "-1.1754943E21", "-1.1754943E22",
            "-1.1754944E23", "-1.17549434E24", "-1.1754943E25", "-1.1754943E26",
            "-1.17549434E27", "-1.1754943E28", "-1.1754944E29", "-1.1754943E30",
            "-1.1754943E31", "-1.1754944E32", "-1.1754943E33", "-1.1754944E34",
            "-1.1754944E35", "-1.1754944E36", "-1.1754943E37", "-1.1754943E38" };


        // J2N: Moved to CharSequences
        //private void doTestCompareRawBits(string originalFloatString, int expectedRawBits,
        //    string expectedString)
        //{
        //    doTestCompareRawBits(originalFloatString, NumberStyle.Float, expectedRawBits, expectedString);
        //}

        //// J2N specific - allow passing style through so we can test the edge cases for hex or float type suffix (i.e "1.23f") specifier
        //private void doTestCompareRawBits(string originalFloatString, NumberStyle style, int expectedRawBits,
        //    string expectedString)
        //{
        //    int rawBits;
        //    float result = Single.Parse(originalFloatString, style, J2N.Text.StringFormatter.InvariantCulture);
        //    rawBits = Single.SingleToInt32Bits(result);
        //    assertEquals("Original float(" + originalFloatString + ") Converted float(" + result
        //            + ") Expecting:" + Int32.ToHexString(expectedRawBits) + " Got: "
        //            + Int32.ToHexString(rawBits), expectedRawBits, rawBits);
        //}

        /**
         * @tests java.lang.Float#Float(float)
         */
        [Test]
        public void Test_ConstructorF()
        {
            // Test for method java.lang.Float(float)

            Single f = new Single(900.89f);
            assertTrue("Created incorrect float", f.GetSingleValue() == 900.89f);
        }

        /**
         * @tests java.lang.Float#Float(java.lang.String)
         */
        [Test]
        public void Test_ConstructorLjava_lang_String()
        {
            // J2N TODO: Move to Parse tests in CharSequences

            // Test for method java.lang.Float(java.lang.String)

            float f = Single.Parse("900.89", J2N.Text.StringFormatter.InvariantCulture);
            assertTrue("Created incorrect Float", f == 900.89f);
        }

        /**
         * @tests java.lang.Float#byteValue()
         */
        [Test]
        public void Test_byteValue()
        {
            // Test for method byte java.lang.Float.byteValue()
            Single f = new Single(0.46874f);
            Single f2 = new Single(90.8f);
            assertTrue("Returned incorrect byte value", f.GetByteValue() == 0 && f2.GetByteValue() == 90);
        }

        /**
         * @tests java.lang.Float#compareTo(java.lang.Float)
         * @tests java.lang.Float#compare(float, float)
         */
        [Test]
        public void Test_compare()
        {
            float[] values = new float[] { float.NegativeInfinity, -float.MaxValue, -2f,
                -float.Epsilon, -0f, 0f, float.Epsilon, 2f, float.MaxValue,
                float.PositiveInfinity, float.NaN }; // J2N NOTE: MIN_VALUE in Java is the same as Epsilon in .NET
            for (int i = 0; i < values.Length; i++)
            {
                float f1 = values[i];
                assertTrue("compare() should be equal: " + f1, Single.Compare(f1, f1) == 0);
                Single F1 = new Single(f1);
                assertTrue("compareTo() should be equal: " + f1, F1.CompareTo(F1) == 0);
                for (int j = i + 1; j < values.Length; j++)
                {
                    float f2 = values[j];

                    assertTrue("compare() " + f1 + " should be less " + f2,
                            Single.Compare(f1, f2) == -1);
                    assertTrue("compare() " + f2 + " should be greater " + f1, Single
                            .Compare(f2, f1) == 1);
                    Single F2 = new Single(f2);
                    assertTrue("compareTo() " + f1 + " should be less " + f2,
                            F1.CompareTo(F2) == -1);
                    assertTrue("compareTo() " + f2 + " should be greater " + f1,
                            F2.CompareTo(F1) == 1);
                }
            }

            //try
            //{
            //    new Single(0.0F).CompareTo(null);
            //    fail("No NPE");
            //}
            //catch (ArgumentNullException e)
            //{
            //}

            // J2N: Return 1 when comparing to null to match other .NET classes
            assertEquals(1, new Single(0.0F).CompareTo(null));
        }

        /**
         * @tests java.lang.Float#doubleValue()
         */
        [Test]
        public void Test_doubleValue()
        {
            // Test for method double java.lang.Float.doubleValue()
            assertTrue("Incorrect double value returned", Math.Abs(new Single(999999.999f)
                    .GetDoubleValue() - 999999.999d) < 1);
        }

        /**
         * @tests java.lang.Float#SingleToInt32Bits(float)
         */
        [Test]
        public void Test_SingleToInt32BitsF()
        {
            float f = 9876.2345f;
            int bits = Single.SingleToInt32Bits(f);
            float r = Single.Int32BitsToSingle(bits);
            assertTrue("Incorrect intBits returned", f == r);
        }

        /**
         * @tests java.lang.Float#floatToRawIntBits(float)
         */
        [Test]
        public void Test_floatToRawIntBitsF()
        {
            int i = 0x7fc004d2;
            float f = Single.Int32BitsToSingle(i);
            assertTrue("Wrong raw bits", Single.SingleToRawInt32Bits(f) == i);
        }

        /**
         * @tests java.lang.Float#floatValue()
         */
        [Test]
        public void Test_floatValue()
        {
            // Test for method float java.lang.Float.floatValue()
            Single f = new Single(87.657f);
            Single f2 = new Single(-0.876f);
            assertTrue("Returned incorrect floatValue", f.GetSingleValue() == 87.657f
                    && (f2.GetSingleValue() == -0.876f));

        }

        /**
         * @tests java.lang.Float#hashCode()
         */
        [Test]
        public void Test_hashCode()
        {
            // Test for method int java.lang.Float.hashCode()
            Single f = new Single(1908.8786f);
            assertTrue("Returned invalid hash code for 1908.8786f", f.GetHashCode() == Single
                    .SingleToInt32Bits(1908.8786f));

            f = new Single(-1.112f);
            assertTrue("Returned invalid hash code for -1.112", f.GetHashCode() == Single
                    .SingleToInt32Bits(-1.112f));

            f = new Single(0f);
            assertTrue("Returned invalid hash code for 0", f.GetHashCode() == Single.SingleToInt32Bits(0f));

        }

        /**
         * @tests java.lang.Float#Int32BitsToSingle(int)
         */
        [Test]
        public void Test_Int32BitsToSingleI()
        {
            float f = 9876.2345f;
            int bits = Single.SingleToInt32Bits(f);
            float r = Single.Int32BitsToSingle(bits);
            assertEquals("Incorrect intBits returned", f, r, 0F);
        }

        /**
         * @tests java.lang.Float#intValue()
         */
        [Test]
        public void Test_intValue()
        {
            // Test for method int java.lang.Float.intValue()
            Single f = new Single(0.46874f);
            Single f2 = new Single(90.8f);
            assertTrue("Returned incorrect int value", f.GetInt32Value() == 0 && f2.GetInt32Value() == 90);
        }

        /**
         * @tests java.lang.Float#isInfinite()
         */
        [Test]
        public void Test_isInfinite()
        {
            // Test for method boolean java.lang.Float.isInfinite()
            assertTrue("Infinity check failed",
                    (new Single(float.PositiveInfinity).IsInfinity() && new Single(
                            float.NegativeInfinity).IsInfinity())
                            && !(new Single(0.13131414f).IsInfinity()));
        }

        /**
         * @tests java.lang.Float#isInfinite(float)
         */
        [Test]
        public void Test_isInfiniteF()
        {
            // Test for method boolean java.lang.Float.isInfinite(float)

            assertTrue("Infinity check failed", Single.IsInfinity(float.PositiveInfinity)
                    && (Single.IsInfinity(float.NegativeInfinity)) && !(Single.IsInfinity(1.0f)));
        }

        /**
         * @tests java.lang.Float#isNaN()
         */
        [Test]
        public void Test_isNaN()
        {
            // Test for method boolean java.lang.Float.isNaN()
            assertTrue("NAN check failed", new Single(float.NaN).IsNaN()
                    && !(new Single(1.0f).IsNaN()));
        }

        /**
         * @tests java.lang.Float#isNaN(float)
         */
        [Test]
        public void Test_isNaNF()
        {
            // Test for method boolean java.lang.Float.isNaN(float)
            assertTrue("NaN check failed", Single.IsNaN(float.NaN) && !(Single.IsNaN(12.09f)));
        }

        /**
         * @tests java.lang.Float#longValue()
         */
        [Test]
        public void Test_longValue()
        {
            // Test for method long java.lang.Float.longValue()
            Single f = new Single(0.46874f);
            Single f2 = new Single(90.8f);
            assertTrue("Returned incorrect long value", f.GetInt64Value() == 0 && f2.GetInt64Value() == 90);
        }

        // J2N: Moved to CharSequences
        ///**
        // * @tests java.lang.Float#parseFloat(java.lang.String)
        // */
        //[Test]
        //public void Test_parseFloatLjava_lang_String()
        //{
        //    assertEquals("Incorrect float returned, expected zero.", 0.0, Single
        //            .Parse("7.0064923216240853546186479164495e-46", J2N.Text.StringFormatter.InvariantCulture), 0.0);
        //    assertEquals("Incorrect float returned, expected minimum float.", float.Epsilon, // J2N: In .NET float.Epsilon is the same as Float.MIN_VALUE in Java
        //            Single.Parse("7.0064923216240853546186479164496e-46", J2N.Text.StringFormatter.InvariantCulture), 0.0);

        //    doTestCompareRawBits(
        //            "0.000000000000000000000000000000000000011754942807573642917278829910357665133228589927589904276829631184250030649651730385585324256680905818939208984375",
        //            0x800000, "1.17549435E-38");
        //    doTestCompareRawBits(
        //            "0.00000000000000000000000000000000000001175494280757364291727882991035766513322858992758990427682963118425003064965173038558532425668090581893920898437499999f",
        //            NumberStyle.Float | NumberStyle.AllowTypeSpecifier, // J2N specific - must use AllowTypeSpecifier to specify this is valid
        //            0x7fffff, "1.1754942E-38");

        //    /* Test a set of regular floats with exponents from -38 to +38 */
        //    for (int i = 38; i > 3; i--)
        //    {
        //        String testString;
        //        testString = "3.4028234663852886e-" + i;
        //        doTestCompareRawBits(testString, rawBitsFor3_4eN38To38[38 - i],
        //                expectedStringFor3_4eN38To38[38 - i]);
        //    }
        //    doTestCompareRawBits("3.4028234663852886e-3", rawBitsFor3_4eN38To38[38 - 3],
        //            expectedStringFor3_4eN38To38[38 - 3]);
        //    doTestCompareRawBits("3.4028234663852886e-2", rawBitsFor3_4eN38To38[38 - 2],
        //            expectedStringFor3_4eN38To38[38 - 2]);
        //    doTestCompareRawBits("3.4028234663852886e-1", rawBitsFor3_4eN38To38[38 - 1],
        //            expectedStringFor3_4eN38To38[38 - 1]);
        //    doTestCompareRawBits("3.4028234663852886e-0", rawBitsFor3_4eN38To38[38 - 0],
        //            expectedStringFor3_4eN38To38[38 - 0]);
        //    doTestCompareRawBits("3.4028234663852886e+1", rawBitsFor3_4eN38To38[38 + 1],
        //            expectedStringFor3_4eN38To38[38 + 1]);
        //    doTestCompareRawBits("3.4028234663852886e+2", rawBitsFor3_4eN38To38[38 + 2],
        //            expectedStringFor3_4eN38To38[38 + 2]);
        //    doTestCompareRawBits("3.4028234663852886e+3", rawBitsFor3_4eN38To38[38 + 3],
        //            expectedStringFor3_4eN38To38[38 + 3]);
        //    doTestCompareRawBits("3.4028234663852886e+4", rawBitsFor3_4eN38To38[38 + 4],
        //            expectedStringFor3_4eN38To38[38 + 4]);
        //    doTestCompareRawBits("3.4028234663852886e+5", rawBitsFor3_4eN38To38[38 + 5],
        //            expectedStringFor3_4eN38To38[38 + 5]);
        //    doTestCompareRawBits("3.4028234663852886e+6", rawBitsFor3_4eN38To38[38 + 6],
        //            expectedStringFor3_4eN38To38[38 + 6]);

        //    for (int i = 7; i < 39; i++)
        //    {
        //        String testString;
        //        testString = "3.4028234663852886e+" + i;
        //        doTestCompareRawBits(testString, rawBitsFor3_4eN38To38[38 + i],
        //                expectedStringFor3_4eN38To38[38 + i]);
        //    }

        //    /* Test another set of regular floats with exponents from -38 to +38 */
        //    for (int i = 38; i > 3; i--)
        //    {
        //        String testString;
        //        testString = "-1.1754943508222875e-" + i;
        //        doTestCompareRawBits(testString, rawBitsFor1_17eN38To38[38 - i],
        //                expectedStringFor1_17eN38To38[38 - i]);
        //    }
        //    doTestCompareRawBits("-1.1754943508222875e-3", rawBitsFor1_17eN38To38[38 - 3],
        //            expectedStringFor1_17eN38To38[38 - 3]);
        //    doTestCompareRawBits("-1.1754943508222875e-2", rawBitsFor1_17eN38To38[38 - 2],
        //            expectedStringFor1_17eN38To38[38 - 2]);
        //    doTestCompareRawBits("-1.1754943508222875e-1", rawBitsFor1_17eN38To38[38 - 1],
        //            expectedStringFor1_17eN38To38[38 - 1]);
        //    doTestCompareRawBits("-1.1754943508222875e-0", rawBitsFor1_17eN38To38[38 - 0],
        //            expectedStringFor1_17eN38To38[38 - 0]);
        //    doTestCompareRawBits("-1.1754943508222875e+1", rawBitsFor1_17eN38To38[38 + 1],
        //            expectedStringFor1_17eN38To38[38 + 1]);
        //    doTestCompareRawBits("-1.1754943508222875e+2", rawBitsFor1_17eN38To38[38 + 2],
        //            expectedStringFor1_17eN38To38[38 + 2]);
        //    doTestCompareRawBits("-1.1754943508222875e+3", rawBitsFor1_17eN38To38[38 + 3],
        //            expectedStringFor1_17eN38To38[38 + 3]);
        //    doTestCompareRawBits("-1.1754943508222875e+4", rawBitsFor1_17eN38To38[38 + 4],
        //            expectedStringFor1_17eN38To38[38 + 4]);
        //    doTestCompareRawBits("-1.1754943508222875e+5", rawBitsFor1_17eN38To38[38 + 5],
        //            expectedStringFor1_17eN38To38[38 + 5]);
        //    doTestCompareRawBits("-1.1754943508222875e+6", rawBitsFor1_17eN38To38[38 + 6],
        //            expectedStringFor1_17eN38To38[38 + 6]);

        //    for (int i = 7; i < 39; i++)
        //    {
        //        String testString;
        //        testString = "-1.1754943508222875e+" + i;
        //        doTestCompareRawBits(testString, rawBitsFor1_17eN38To38[38 + i],
        //                expectedStringFor1_17eN38To38[38 + i]);
        //    }

        //    /* Test denormalized floats (floats with exponents <= -38 */
        //    doTestCompareRawBits("1.1012984643248170E-45", 1, "1.4E-45");
        //    doTestCompareRawBits("-1.1012984643248170E-45", unchecked((int)0x80000001), "-1.4E-45");
        //    doTestCompareRawBits("1.0E-45", 1, "1.4E-45");
        //    doTestCompareRawBits("-1.0E-45", unchecked((int)0x80000001), "-1.4E-45");
        //    doTestCompareRawBits("0.9E-45", 1, "1.4E-45");
        //    doTestCompareRawBits("-0.9E-45", unchecked((int)0x80000001), "-1.4E-45");
        //    doTestCompareRawBits("4.203895392974451e-45", 3, "4.2E-45");
        //    doTestCompareRawBits("-4.203895392974451e-45", unchecked((int)0x80000003), "-4.2E-45");
        //    doTestCompareRawBits("0.004E-45", 0, "0.0");
        //    doTestCompareRawBits("-0.004E-45", unchecked((int)0x80000000), "-0.0");

        //    /*
        //     * Test for large floats close to and greater than 3.4028235E38 and
        //     * -3.4028235E38
        //     */
        //    doTestCompareRawBits("1.2E+38", 0x7eb48e52, "1.2E38");
        //    doTestCompareRawBits("-1.2E+38", unchecked((int)0xfeb48e52), "-1.2E38");
        //    doTestCompareRawBits("3.2E+38", 0x7f70bdc2, "3.2E38");
        //    doTestCompareRawBits("-3.2E+38", unchecked((int)0xff70bdc2), "-3.2E38");
        //    doTestCompareRawBits("3.4E+38", 0x7f7fc99e, "3.4E38");
        //    doTestCompareRawBits("-3.4E+38", unchecked((int)0xff7fc99e), "-3.4E38");
        //    doTestCompareRawBits("3.4028234663852886E+38", 0x7f7fffff, "3.4028235E38");
        //    doTestCompareRawBits("-3.4028234663852886E+38", unchecked((int)0xff7fffff), "-3.4028235E38");
        //    doTestCompareRawBits("3.405E+38", 0x7f800000, "Infinity");
        //    doTestCompareRawBits("-3.405E+38", unchecked((int)0xff800000), "-Infinity");
        //    doTestCompareRawBits("3.41E+38", 0x7f800000, "Infinity");
        //    doTestCompareRawBits("-3.41E+38", unchecked((int)0xff800000), "-Infinity");
        //    doTestCompareRawBits("3.42E+38", 0x7f800000, "Infinity");
        //    doTestCompareRawBits("-3.42E+38", unchecked((int)0xff800000), "-Infinity");
        //    doTestCompareRawBits("1.0E+39", 0x7f800000, "Infinity");
        //    doTestCompareRawBits("-1.0E+39", unchecked((int)0xff800000), "-Infinity");
        //}

        // J2N: Moved to CharSequences
        ///**
        // * @tests java.lang.Float#parseFloat(java.lang.String)
        // */
        //[Test]
        //public void Test_parseFloat_LString_Unusual()
        //{
        //    float actual;

        //    actual = Single.Parse("0x00000000000000000000000000000000000000000.0000000000000000000000000000000000000p0000000000000000000000000000000000", NumberStyle.HexFloat, J2N.Text.StringFormatter.InvariantCulture);
        //    assertEquals("Returned incorrect value", 0.0f, actual, 0.0F);

        //    actual = Single.Parse("+0Xfffff.fffffffffffffffffffffffffffffffp+99F", NumberStyle.HexFloat | NumberStyle.AllowTypeSpecifier, J2N.Text.StringFormatter.InvariantCulture);
        //    assertEquals("Returned incorrect value", 6.64614E35f, actual, 0.0F);

        //    actual = Single.Parse("-0X.123456789abcdefp+99f", NumberStyle.HexFloat | NumberStyle.AllowTypeSpecifier, J2N.Text.StringFormatter.InvariantCulture);
        //    assertEquals("Returned incorrect value", -4.5072022E28f, actual, 0.0F);

        //    actual = Single.Parse("-0X123456789abcdef.p+1f", NumberStyle.HexFloat | NumberStyle.AllowTypeSpecifier, J2N.Text.StringFormatter.InvariantCulture);
        //    assertEquals("Returned incorrect value", -1.63971062E17f, actual, 0.0F);

        //    actual = Single.Parse("-0X000000000000000000000000000001abcdef.0000000000000000000000000001abefp00000000000000000000000000000000000000000004f", NumberStyle.HexFloat | NumberStyle.AllowTypeSpecifier, J2N.Text.StringFormatter.InvariantCulture);
        //    assertEquals("Returned incorrect value", -4.48585472E8f, actual, 0.0F);

        //    actual = Single.Parse("0X0.00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000001234p600f", NumberStyle.HexFloat | NumberStyle.AllowTypeSpecifier, J2N.Text.StringFormatter.InvariantCulture);
        //    assertEquals("Returned incorrect value", 5.907252E33f, actual, 0.0F);

        //    actual = Single.Parse("0x1.p9223372036854775807", NumberStyle.HexFloat, J2N.Text.StringFormatter.InvariantCulture);
        //    assertEquals("Returned incorrect value", float.PositiveInfinity, actual, 0.0F);

        //    actual = Single.Parse("0x1.p9223372036854775808", NumberStyle.HexFloat, J2N.Text.StringFormatter.InvariantCulture);
        //    assertEquals("Returned incorrect value", float.PositiveInfinity, actual, 0.0F);

        //    actual = Single.Parse("0x10.p9223372036854775808", NumberStyle.HexFloat, J2N.Text.StringFormatter.InvariantCulture);
        //    assertEquals("Returned incorrect value", float.PositiveInfinity, actual, 0.0F);

        //    actual = Single.Parse("0xabcd.ffffffffp+2000", NumberStyle.HexFloat, J2N.Text.StringFormatter.InvariantCulture);
        //    assertEquals("Returned incorrect value", float.PositiveInfinity, actual, 0.0F);

        //    actual = Single.Parse("0x1.p-9223372036854775808", NumberStyle.HexFloat, J2N.Text.StringFormatter.InvariantCulture);
        //    assertEquals("Returned incorrect value", 0.0f, actual, 0.0F);

        //    actual = Single.Parse("0x1.p-9223372036854775809", NumberStyle.HexFloat, J2N.Text.StringFormatter.InvariantCulture);
        //    assertEquals("Returned incorrect value", 0.0f, actual, 0.0F);

        //    actual = Single.Parse("0x.1p-9223372036854775809", NumberStyle.HexFloat, J2N.Text.StringFormatter.InvariantCulture);
        //    assertEquals("Returned incorrect value", 0.0f, actual, 0.0F);
        //}

        // J2N: Moved to CharSequences
        ///**
        // * @tests java.lang.Float#parseFloat(java.lang.String)
        // */
        //[Test]
        //public void Test_parseFloat_LString_NormalPositiveExponent()
        //{
        //    int[] expecteds = {
        //        0x3991a2b4,                0x43cc0247,                0x47909009,
        //        0x4ac0c009,                0x4e109005,                0x5140c005,
        //        0x5458d805,                0x57848402,                0x5a909002,
        //        0x5da8a802,                0x60c0c002,                0x63cccc02,
        //        0x66e4e402,                0x69f0f002,                0x6d048401,
        //        0x70109001,                0x73169601,                0x76810810,
        //        0x79840840,                0x7c8a08a0,                0x7f800000,
        //        0x7f800000,                0x7f800000,                0x7f800000,
        //        0x7f800000,
        //};

        //    for (int i = 0; i < expecteds.Length; i++)
        //    {
        //        int part = i * 6;
        //        String inputString = "0x" + part + "." + part + "0123456789abcdefp" + part;

        //        float actual = Single.Parse(inputString, NumberStyle.HexFloat, J2N.Text.StringFormatter.InvariantCulture);
        //        float expected = Single.Int32BitsToSingle(expecteds[i]);

        //        String expectedString = Int32.ToHexString(Single.SingleToInt32Bits(expected));
        //        String actualString = Int32.ToHexString(Single.SingleToInt32Bits(actual));
        //        String errorMsg = i + "th input string is:<" + inputString
        //        + ">.The expected result should be:<" + expectedString
        //        + ">, but was: <" + actualString + ">. ";

        //        assertEquals(errorMsg, expected, actual, 0.0F);
        //    }
        //}

        // J2N: Moved to CharSequences
        ///**
        // * @tests java.lang.Float#parseFloat(java.lang.String)
        // */
        //[Test]
        //public void Test_parseFloat_LString_NormalNegativeExponent()
        //{
        //    int[] expecteds = {
        //        0x3991a2b4,
        //        0x3d6e0247,
        //        0x3aa0a009,
        //        0x37848405,
        //        0x3420a005,
        //        0x30d4d405,
        //        0x2d848402,
        //        0x2a129202,
        //        0x26acac02,
        //        0x2346c602,
        //        0x1fe0e002,
        //        0x1c6eee02,
        //        0x19048401,
        //        0x15919101,
        //        0x12189801,
        //        0xf028828,
        //        0xb890890,
        //        0x80c88c8,
        //        0x4930930,
        //        0x1198998,
        //        0x28028,
        //        0x51c,
        //        0xb,
        //        0x0,
        //        0x0,
        //};

        //    for (int i = 0; i < expecteds.Length; i++)
        //    {
        //        int part = i * 7;
        //        String inputString = "0x" + part + "." + part + "0123456789abcdefp-" + part;

        //        float actual = Single.Parse(inputString, NumberStyle.HexFloat, J2N.Text.StringFormatter.InvariantCulture);
        //        float expected = Single.Int32BitsToSingle(expecteds[i]);

        //        String expectedString = Int32.ToHexString(Single.SingleToInt32Bits(expected));
        //        String actualString = Int32.ToHexString(Single.SingleToInt32Bits(actual));
        //        String errorMsg = i + "th input string is:<" + inputString
        //        + ">.The expected result should be:<" + expectedString
        //        + ">, but was: <" + actualString + ">. ";

        //        assertEquals(errorMsg, expected, actual, 0.0F);
        //    }
        //}

        // J2N: Moved to CharSequences
        ///**
        // * @tests java.lang.Float#parseFloat(java.lang.String)
        // */
        //[Test]
        //public void Test_parseFloat_LString_MaxNormalBoundary()
        //{
        //    int[] expecteds ={
        //        0x7f7fffff,
        //        0x7f7fffff,
        //        0x7f7fffff,
        //        0x7f800000,
        //        0x7f800000,
        //        0x7f800000,

        //        unchecked((int)0xff7fffff),
        //        unchecked((int)0xff7fffff),
        //        unchecked((int)0xff7fffff),
        //        unchecked((int)0xff800000),
        //        unchecked((int)0xff800000),
        //        unchecked((int)0xff800000),
        //};

        //    String[] inputs = {
        //        "0x1.fffffep127",
        //        "0x1.fffffe000000000000000000000000000000000000000000000001p127",
        //        "0x1.fffffeffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffp127",
        //        "0x1.ffffffp127",
        //        "0x1.ffffff000000000000000000000000000000000000000000000001p127",
        //        "0x1.ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffp127",

        //        "-0x1.fffffep127",
        //        "-0x1.fffffe000000000000000000000000000000000000000000000001p127",
        //        "-0x1.fffffeffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffp127",
        //        "-0x1.ffffffp127",
        //        "-0x1.ffffff000000000000000000000000000000000000000000000001p127",
        //        "-0x1.ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffp127",
        //};

        //    for (int i = 0; i < inputs.Length; i++)
        //    {
        //        //float actual = Single.Parse(inputs[i], J2N.Text.StringFormatter.InvariantCulture);
        //        float actual = Single.Parse(inputs[i], NumberStyle.HexFloat, J2N.Text.StringFormatter.InvariantCulture);
        //        float expected = Single.Int32BitsToSingle(expecteds[i]);

        //        String expectedString = Int32.ToHexString(Single.SingleToInt32Bits(expected));
        //        String actualString = Int32.ToHexString(Single.SingleToInt32Bits(actual));
        //        String errorMsg = i + "th input string is:<" + inputs[i]
        //        + ">.The expected result should be:<" + expectedString
        //        + ">, but was: <" + actualString + ">. ";

        //        assertEquals(errorMsg, expected, actual, 0.0F);
        //    }
        //}

        // J2N: Moved to CharSequences
        ///**
        // * @tests java.lang.Float#parseFloat(java.lang.String)
        // */
        //[Test]
        //public void Test_parseFloat_LString_MinNormalBoundary()
        //{
        //    int[] expecteds = {
        //        0x800000,
        //        0x800000,
        //        0x800000,
        //        0x800000,
        //        0x800001,
        //        0x800001,

        //        unchecked((int)0x80800000),
        //        unchecked((int)0x80800000),
        //        unchecked((int)0x80800000),
        //        unchecked((int)0x80800000),
        //        unchecked((int)0x80800001),
        //        unchecked((int)0x80800001),
        //};

        //    String[] inputs = {
        //        "0x1.0p-126",
        //        "0x1.00000000000000000000000000000000000000000000001p-126",
        //        "0x1.000000ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffp-126",
        //        "0x1.000001p-126",
        //        "0x1.000001000000000000000000000000000000000000000001p-126",
        //        "0x1.000001fffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffp-126",

        //        "-0x1.0p-126",
        //        "-0x1.00000000000000000000000000000000000000000000001p-126",
        //        "-0x1.000000ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffp-126",
        //        "-0x1.000001p-126",
        //        "-0x1.000001000000000000000000000000000000000000000001p-126",
        //        "-0x1.000001fffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffp-126",
        //};

        //    for (int i = 0; i < inputs.Length; i++)
        //    {
        //        float actual = Single.Parse(inputs[i], NumberStyle.HexFloat, J2N.Text.StringFormatter.InvariantCulture);
        //        float expected = Single.Int32BitsToSingle(expecteds[i]);

        //        String expectedString = Int32.ToHexString(Single.SingleToInt32Bits(expected));
        //        String actualString = Int32.ToHexString(Single.SingleToInt32Bits(actual));
        //        String errorMsg = i + "th input string is:<" + inputs[i]
        //        + ">.The expected result should be:<" + expectedString
        //        + ">, but was: <" + actualString + ">. ";

        //        assertEquals(errorMsg, expected, actual, 0.0F);
        //    }
        //}

        // J2N: Moved to CharSequences
        ///**
        // * @tests java.lang.Float#parseFloat(java.lang.String)
        // */
        //[Test]
        //public void Test_parseFloat_LString_MaxSubNormalBoundary()
        //{
        //    int[] expecteds = {
        //        0x7fffff,
        //        0x7fffff,
        //        0x7fffff,
        //        0x800000,
        //        0x800000,
        //        0x800000,

        //        unchecked((int)0x807fffff),
        //        unchecked((int)0x807fffff),
        //        unchecked((int)0x807fffff),
        //        unchecked((int)0x80800000),
        //        unchecked((int)0x80800000),
        //        unchecked((int)0x80800000),
        //};

        //    String[] inputs = {
        //        "0x0.fffffep-126",
        //        "0x0.fffffe000000000000000000000000000000000000000000000000000001p-126",
        //        "0x0.fffffefffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffp-126",
        //        "0x0.ffffffp-126",
        //        "0x0.ffffff0000000000000000000000000000000000000000000000000000001p-126",
        //        "0x0.ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffp-126",

        //        "-0x0.fffffep-126",
        //        "-0x0.fffffe000000000000000000000000000000000000000000000000000001p-126",
        //        "-0x0.fffffefffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffp-126",
        //        "-0x0.ffffffp-126",
        //        "-0x0.ffffff0000000000000000000000000000000000000000000000000000001p-126",
        //        "-0x0.ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffp-126",
        //};

        //    for (int i = 0; i < inputs.Length; i++)
        //    {
        //        float actual = Single.Parse(inputs[i], NumberStyle.HexFloat, J2N.Text.StringFormatter.InvariantCulture);
        //        float expected = Single.Int32BitsToSingle(expecteds[i]);

        //        String expectedString = Int32.ToHexString(Single.SingleToInt32Bits(expected));
        //        String actualString = Int32.ToHexString(Single.SingleToInt32Bits(actual));
        //        String errorMsg = i + "th input string is:<" + inputs[i]
        //        + ">.The expected result should be:<" + expectedString
        //        + ">, but was: <" + actualString + ">. ";

        //        assertEquals(errorMsg, expected, actual, 0.0F);
        //    }
        //}

        // J2N: Moved to CharSequences
        ///**
        // * @tests java.lang.Float#parseFloat(java.lang.String)
        // */
        //[Test]
        //public void Test_parseFloat_LString_MinSubNormalBoundary()
        //{
        //    int[] expecteds = {
        //        0x1,
        //        0x1,
        //        0x1,
        //        0x2,
        //        0x2,
        //        0x2,

        //        unchecked((int)0x80000001),
        //        unchecked((int)0x80000001),
        //        unchecked((int)0x80000001),
        //        unchecked((int)0x80000002),
        //        unchecked((int)0x80000002),
        //        unchecked((int)0x80000002),
        //};

        //    String[] inputs = {
        //        "0x0.000002p-126",
        //        "0x0.00000200000000000000000000000000000000000001p-126",
        //        "0x0.000002ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffp-126",
        //        "0x0.000003p-126",
        //        "0x0.000003000000000000000000000000000000000000001p-126",
        //        "0x0.000003ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffp-126",

        //        "-0x0.000002p-126",
        //        "-0x0.00000200000000000000000000000000000000000001p-126",
        //        "-0x0.000002ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffp-126",
        //        "-0x0.000003p-126",
        //        "-0x0.000003000000000000000000000000000000000000001p-126",
        //        "-0x0.000003ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffp-126",
        //};

        //    for (int i = 0; i < inputs.Length; i++)
        //    {
        //        float actual = Single.Parse(inputs[i], NumberStyle.HexFloat, J2N.Text.StringFormatter.InvariantCulture);
        //        float expected = Single.Int32BitsToSingle(expecteds[i]);

        //        String expectedString = Int32.ToHexString(Single.SingleToInt32Bits(expected));
        //        String actualString = Int32.ToHexString(Single.SingleToInt32Bits(actual));
        //        String errorMsg = i + "th input string is:<" + inputs[i]
        //        + ">.The expected result should be:<" + expectedString
        //        + ">, but was: <" + actualString + ">. ";

        //        assertEquals(errorMsg, expected, actual, 0.0F);
        //    }
        //}

        // J2N: Moved to CharSequences
        ///**
        // * @tests java.lang.Float#parseFloat(java.lang.String)
        // */
        //[Test]
        //public void Test_parseFloat_LString_ZeroBoundary()
        //{
        //    int[] expecteds = {
        //        0x0,
        //        0x0,
        //        0x0,
        //        0x0,
        //        0x1,
        //        0x1,

        //        unchecked((int)0x80000000),
        //        unchecked((int)0x80000000),
        //        unchecked((int)0x80000000),
        //        unchecked((int)0x80000000),
        //        unchecked((int)0x80000001),
        //        unchecked((int)0x80000001),
        //};

        //    String[] inputs = {
        //        "0x0.000000000000000p-126",
        //        "0x0.000000000000000000000000000000000000000000000001p-126",
        //        "0x0.000000fffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffp-126",
        //        "0x0.000001p-126",
        //        "0x0.000001000000000000000000000000000000000000000001p-126",
        //        "0x0.000001fffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffp-126",

        //        "-0x0.000000000000000p-126",
        //        "-0x0.000000000000000000000000000000000000000000000001p-126",
        //        "-0x0.000000fffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffp-126",
        //        "-0x0.000001p-126",
        //        "-0x0.000001000000000000000000000000000000000000000001p-126",
        //        "-0x0.000001fffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffp-126",
        //};

        //    for (int i = 0; i < inputs.Length; i++)
        //    {
        //        float actual = Single.Parse(inputs[i], NumberStyle.HexFloat, J2N.Text.StringFormatter.InvariantCulture);
        //        float expected = Single.Int32BitsToSingle(expecteds[i]);

        //        String expectedString = Int32.ToHexString(Single.SingleToInt32Bits(expected));
        //        String actualString = Int32.ToHexString(Single.SingleToInt32Bits(actual));
        //        String errorMsg = i + "th input string is:<" + inputs[i]
        //        + ">.The expected result should be:<" + expectedString
        //        + ">, but was: <" + actualString + ">. ";

        //        assertEquals(errorMsg, expected, actual, 0.0F);
        //    }
        //}

        /**
         * @tests java.lang.Float#parseFloat(java.lang.String)
         */
        [Test]
        public void Test_parseFloat_LString_Harmony6261()
        {
            // Regression test for HARMONY-6261
            float f = Single.Parse("2147483648", J2N.Text.StringFormatter.InvariantCulture);
            //assertEquals("2.14748365E9", Single.ToString(f, J2N.Text.StringFormatter.InvariantCulture)); // J2N: Changed from "2.1474836E9" to "2.14748365E9" to match JDK behavior
            assertEquals("2.1474836E9", Single.ToString(f, J2N.Text.StringFormatter.InvariantCulture));

            // J2N: Moved to CharSequences

            //// J2N specific - need to specify AllowTypeSpecifier to match numbers ending in "d" or "f" (case insensitive)
            //doTestCompareRawBits("123456790528.000000000000000f", NumberStyle.Float | NumberStyle.AllowTypeSpecifier, 0x51e5f4c9, "1.2345679E11");
            //doTestCompareRawBits("123456790528.000000000000000", 0x51e5f4c9, "1.2345679E11");
            //doTestCompareRawBits("8589934592", 0x50000000, "8.5899346E9");
            //doTestCompareRawBits("8606711808", 0x50004000, "8.606712E9");
        }

        /**
         * @tests java.lang.Float#shortValue()
         */
        [Test]
        public void Test_shortValue()
        {
            // Test for method short java.lang.Float.shortValue()
            Single f = new Single(0.46874f);
            Single f2 = new Single(90.8f);
            assertTrue("Returned incorrect short value", f.GetInt16Value() == 0
                    && f2.GetInt16Value() == 90);
        }

        /**
         * @tests java.lang.Float#toString()
         */
        [Test]
        public void Test_toString()
        {
            // Test for method java.lang.String java.lang.Float.toString()

            Test_toString(12.90898f, "12.90898");

            Test_toString(1.7014118346046924e+38F, "1.7014118E38");

            Test_toString(1E19F, "1.0E19");

            Test_toString(1E-36F, "1.0E-36");

            Test_toString(1.0E-38F, "1.0E-38");
        }

        /**
         * @tests java.lang.Float#toString(float)
         */
        [Test]
        public void Test_toStringF()
        {
            // Test for method java.lang.String java.lang.Float.toString(float)

            float ff;
            String answer;

            ff = 12.90898f;
            answer = "12.90898";
            assertTrue("Incorrect String representation want " + answer + ", got "
                    + Single.ToString(ff, J2N.Text.StringFormatter.InvariantCulture), Single.ToString(ff, J2N.Text.StringFormatter.InvariantCulture).Equals(answer));

            ff = float.MaxValue;
            answer = "3.4028235E38";
            assertTrue("Incorrect String representation want " + answer + ", got "
                    + Single.ToString(ff, J2N.Text.StringFormatter.InvariantCulture), Single.ToString(ff, J2N.Text.StringFormatter.InvariantCulture).Equals(answer));
        }

        /**
         * @tests java.lang.Float#valueOf(java.lang.String)
         */
        [Test]
        public void Test_valueOfLjava_lang_String()
        {
            // Test for method java.lang.Float
            // java.lang.Float.valueOf(java.lang.String)

            Single wanted = new Single(432.1235f);
            Single got = Single.ValueOf("432.1235", J2N.Text.StringFormatter.InvariantCulture);
            assertTrue("Incorrect float returned--wanted: " + wanted + " but got: " + got, got
                    .Equals(wanted));

            wanted = new Single(0f);
            got = Single.ValueOf("0", J2N.Text.StringFormatter.InvariantCulture);
            assertTrue("Incorrect float returned--wanted: " + wanted + " but got: " + got, got
                    .Equals(wanted));

            wanted = new Single(-1212.3232f);
            got = Single.ValueOf("-1212.3232", J2N.Text.StringFormatter.InvariantCulture);
            assertTrue("Incorrect float returned--wanted: " + wanted + " but got: " + got, got
                    .Equals(wanted));

            try
            {
                Single.ValueOf(null, J2N.Text.StringFormatter.InvariantCulture);
                fail("Expected Float.valueOf(null) to throw NPE.");
            }
            catch (ArgumentNullException ex)
            {
                // expected
            }

            try
            {
                Single.ValueOf("", J2N.Text.StringFormatter.InvariantCulture);
                fail("Expected Single.valueOf(\"\") to throw NFE");
            }
            catch (FormatException e)
            {
                // expected
            }

            Single posZero = Single.ValueOf("+0.0", J2N.Text.StringFormatter.InvariantCulture);
            Single negZero = Single.ValueOf("-0.0", J2N.Text.StringFormatter.InvariantCulture);
            assertFalse("Floattest0", posZero.Equals(negZero));

            // J2N: .NET specific - testing specific cultures should also parse negative zero correctly
            Single posZero_de = Single.ValueOf("+0,0", new CultureInfo("de-DE"));
            Single negZero_de = Single.ValueOf("-0,0", new CultureInfo("de-DE"));
            assertFalse("Floattest0", posZero_de.Equals(negZero_de));

            assertTrue("Floattest1", 0.0f == -0.0f);

            // Tests for float values by name.
            Single expectedNaN = new Single(float.NaN);

            Single posNaN = Single.ValueOf("NaN", CultureInfo.InvariantCulture); // J2N: Works in English, but need invariant to guarantee same behavior.
            assertTrue("Floattest2", posNaN.Equals(expectedNaN));

            Single posNaNSigned = Single.ValueOf("+NaN", CultureInfo.InvariantCulture); // J2N: Works in English, but need invariant to guarantee same behavior.
            assertTrue("Floattest3", posNaNSigned.Equals(expectedNaN));

            Single negNaNSigned = Single.ValueOf("-NaN", CultureInfo.InvariantCulture); // J2N: Works in English, but need invariant to guarantee same behavior.
            assertTrue("Floattest4", negNaNSigned.Equals(expectedNaN));

            Single posInfinite = Single.ValueOf("Infinity", CultureInfo.InvariantCulture); // J2N: Same behavior, but only if specifying invariant culture, other cultures throw FormatException in this case
            assertTrue("Floattest5", posInfinite.Equals(new Single(float.PositiveInfinity)));

            Single posInfiniteSigned = Single.ValueOf("+Infinity", CultureInfo.InvariantCulture); // J2N: Same behavior, but only if specifying invariant culture, other cultures throw FormatException in this case
            assertTrue("Floattest6", posInfiniteSigned.Equals(new Single(float.PositiveInfinity)));

            Single negInfiniteSigned = Single.ValueOf("-Infinity", CultureInfo.InvariantCulture); // J2N: Same behavior, but only if specifying invariant culture, other cultures throw FormatException in this case
            assertTrue("Floattest7", negInfiniteSigned.Equals(new Single(float.NegativeInfinity)));

            // test HARMONY-6641
            posInfinite = Single.ValueOf("320.0E+2147483647", J2N.Text.StringFormatter.InvariantCulture);
            assertEquals("Floattest8", float.PositiveInfinity, posInfinite, 0.0f);

            negZero = Single.ValueOf("-1.4E-2147483314", J2N.Text.StringFormatter.InvariantCulture);
            assertEquals("Floattest9", -0.0f, negZero, 0.0f);
        }

        private void Test_toString(float ff, String answer)
        {
            // Test for method java.lang.String java.lang.Double.toString(double)
            assertTrue("Incorrect String representation want " + answer + ", got ("
                    + Single.ToString(ff, J2N.Text.StringFormatter.InvariantCulture) + ")", Single.ToString(ff, J2N.Text.StringFormatter.InvariantCulture).Equals(answer));
            Single f = new Single(ff);
            assertTrue("Incorrect String representation want " + answer + ", got ("
                    + Single.ToString(f.GetSingleValue(), J2N.Text.StringFormatter.InvariantCulture) + ")", Single.ToString(f.GetSingleValue(), J2N.Text.StringFormatter.InvariantCulture).Equals(
                    answer));
            assertTrue("Incorrect String representation want " + answer + ", got (" + f.ToString(J2N.Text.StringFormatter.InvariantCulture)
                    + ")", f.ToString(J2N.Text.StringFormatter.InvariantCulture).Equals(answer));
        }

        /**
         * @tests java.lang.Float#compareTo(java.lang.Float)
         * @tests java.lang.Float#compare(float, float)
         */
        [Test]
        public void Test_compareToLjava_lang_Float()
        {
            // A selection of float values in ascending order.
            float[] values = new float[] { float.NegativeInfinity, -float.MaxValue, -2f,
                -float.Epsilon, -0f, 0f, float.Epsilon, 2f, float.MaxValue,
                float.PositiveInfinity, float.NaN }; // J2N NOTE: MIN_VALUE in Java is the same as Epsilon in .NET

            for (int i = 0; i < values.Length; i++)
            {
                float f1 = values[i];

                // Test that each value compares equal to itself; and each object is
                // equal to another object
                // like itself
                assertTrue("Assert 0: compare() should be equal: " + f1, Single.Compare(f1, f1) == 0);
                Single objFloat = new Single(f1);
                assertTrue("Assert 1: compareTo() should be equal: " + objFloat, objFloat
                        .CompareTo(objFloat) == 0);

                // Test that the Float-defined order is respected
                for (int j = i + 1; j < values.Length; j++)
                {
                    float f2 = values[j];

                    assertTrue("Assert 2: compare() " + f1 + " should be less " + f2, Single
                            .Compare(f1, f2) == -1);
                    assertTrue("Assert 3: compare() " + f2 + " should be greater " + f1, Single
                            .Compare(f2, f1) == 1);

                    Single F2 = new Single(f2);
                    assertTrue("Assert 4: compareTo() " + f1 + " should be less " + f2, objFloat
                            .CompareTo(F2) == -1);
                    assertTrue("Assert 5: compareTo() " + f2 + " should be greater " + f1, F2
                            .CompareTo(objFloat) == 1);
                }
            }
        }

        /**
         * @tests java.lang.Float#equals(java.lang.Object)
         */
        [Test]
        public void Test_equalsLjava_lang_Object()
        {
            Single f1 = new Single(8765.4321f);
            Single f2 = new Single(8765.4321f);
            Single f3 = new Single(-1.0f);
            assertTrue("Assert 0: Equality test failed", f1.Equals(f2) && !(f1.Equals(f3)));

#pragma warning disable CS1718 // Comparison made to same variable
            assertTrue("Assert 1: NaN should not be == Nan", float.NaN != float.NaN);
#pragma warning restore CS1718 // Comparison made to same variable
            assertTrue("Assert 2: NaN should not be == Nan", new Single(float.NaN).Equals(new Single(
                    float.NaN)));
            assertTrue("Assert 3: -0f should be == 0f", 0f == -0f);
            assertTrue("Assert 4: -0f should not be equals() 0f", !new Single(0f).Equals(new Single(
                    -0f)));

            f1 = new Single(1098.576f);
            f2 = new Single(1098.576f);
            f3 = new Single(1.0f);
            assertTrue("Equality test failed", f1.Equals(f2) && !(f1.Equals(f3)));

#pragma warning disable CS1718 // Comparison made to same variable
            assertTrue("NaN should not be == Nan", float.NaN != float.NaN);
#pragma warning restore CS1718 // Comparison made to same variable
            assertTrue("NaN should not be == Nan", new Single(float.NaN)
                    .Equals(new Single(float.NaN)));
            assertTrue("-0f should be == 0f", 0f == -0f);
            assertTrue("-0f should not be equals() 0f", !new Single(0f).Equals(new Single(-0f)));
        }

        /**
         * @tests java.lang.Float#toHexString(float)
         */
        [Test]
        public void Test_toHexStringF()
        {
            // the follow values comes from the Float Javadoc/Spec
            assertEquals("0x0.0p0", Single.ToHexString(0.0F));
            assertEquals("-0x0.0p0", Single.ToHexString(-0.0F));
            assertEquals("0x1.0p0", Single.ToHexString(1.0F));
            assertEquals("-0x1.0p0", Single.ToHexString(-1.0F));
            assertEquals("0x1.0p1", Single.ToHexString(2.0F));
            assertEquals("0x1.8p1", Single.ToHexString(3.0F));
            assertEquals("0x1.0p-1", Single.ToHexString(0.5F));
            assertEquals("0x1.0p-2", Single.ToHexString(0.25F));
            assertEquals("0x1.fffffep127", Single.ToHexString(float.MaxValue));
            assertEquals("0x0.000002p-126", Single.ToHexString(float.Epsilon)); // J2N: In .NET float.Epsilon is the same as Float.MIN_VALUE in Java

            // test edge cases
            assertEquals("NaN", Single.ToHexString(float.NaN));
            assertEquals("-Infinity", Single.ToHexString(float.NegativeInfinity));
            assertEquals("Infinity", Single.ToHexString(float.PositiveInfinity));

            // test various numbers
            assertEquals("-0x1.da8p6", Single.ToHexString(-118.625F));
            assertEquals("0x1.295788p23", Single.ToHexString(9743299.65F));
            assertEquals("0x1.295788p23", Single.ToHexString(9743299.65000F));
            assertEquals("0x1.295788p23", Single.ToHexString(9743299.650001234F));
            assertEquals("0x1.700d1p33", Single.ToHexString(12349743299.65000F));

            // test HARMONY-2132
            //assertEquals("0x1.01p10", Single.ToHexString(0x1.01p10f)); // .NET cannot represent this as a float literal
        }

        /**
         * @tests java.lang.Float#valueOf(float)
         */
        [Test]
        public void Test_valueOfF()
        {
            assertEquals(new Single(float.Epsilon), Single.ValueOf(float.Epsilon)); // J2N: In .NET float.Epsilon is the same as Float.MIN_VALUE in Java
            assertEquals(new Single(float.MaxValue), Single.ValueOf(float.MaxValue));
            assertEquals(new Single(0), Single.ValueOf(0));

            int s = -128;
            while (s < 128)
            {
                assertEquals(new Single(s), Single.ValueOf(s));
                assertEquals(new Single(s + 0.1F), Single.ValueOf(s + 0.1F));
                assertEquals(Single.ValueOf(s + 0.1F), Single.ValueOf(s + 0.1F));
                s++;
            }
        }


        public class CharSequences : TestCase
        {
            public abstract class ParseTestCase : TestCase
            {
                /*
                 * A String, float pair
                 */
                public class PairSD
                {
                    public String s;
                    public float f;
                    PairSD(String s, float f)
                    {
                        this.s = s;
                        this.f = f;
                    }
                }

                private static string[] badStrings = {
                    "",
                    "+",
                    "-",
                    "+e",
                    "-e",
                    "+e170",
                    "-e170",

                    // Make sure intermediate white space is not deleted.
                    "1234   e10",
                    "-1234   e10",

                    // Control characters in the interior of a string are not legal
                    "1\u0007e1",
                    "1e\u00071",

                    // NaN and infinity can't have trailing type suffices or exponents
                    "NaNf",
                    "NaNF",
                    "NaNd",
                    "NaND",
                    "-NaNf",
                    "-NaNF",
                    "-NaNd",
                    "-NaND",
                    "+NaNf",
                    "+NaNF",
                    "+NaNd",
                    "+NaND",
                    "Infinityf",
                    "InfinityF",
                    "Infinityd",
                    "InfinityD",
                    "-Infinityf",
                    "-InfinityF",
                    "-Infinityd",
                    "-InfinityD",
                    "+Infinityf",
                    "+InfinityF",
                    "+Infinityd",
                    "+InfinityD",

                    "NaNe10",
                    "-NaNe10",
                    "+NaNe10",
                    "Infinitye10",
                    "-Infinitye10",
                    "+Infinitye10",

                    // Non-ASCII digits are not recognized
                    "\u0661e\u0661", // 1e1 in Arabic-Indic digits
                    "\u06F1e\u06F1", // 1e1 in Extended Arabic-Indic digits
                    "\u0967e\u0967" // 1e1 in Devanagari digits
                };

                private static string[] badHexStrings = {
                    "",
                    "+",
                    "-",
                    "+p",
                    "-p",
                    "+p170",
                    "-p170",

                    // Make sure intermediate white space is not deleted.
                    "1234   p10",
                    "-1234   p10",

                    // Control characters in the interior of a string are not legal
                    "1\u0007p1",
                    "1p\u00071",

                    // NaN and infinity can't have trailing type suffices or exponents
                    "NaNf",
                    "NaNF",
                    "NaNd",
                    "NaND",
                    "-NaNf",
                    "-NaNF",
                    "-NaNd",
                    "-NaND",
                    "+NaNf",
                    "+NaNF",
                    "+NaNd",
                    "+NaND",
                    "Infinityf",
                    "InfinityF",
                    "Infinityd",
                    "InfinityD",
                    "-Infinityf",
                    "-InfinityF",
                    "-Infinityd",
                    "-InfinityD",
                    "+Infinityf",
                    "+InfinityF",
                    "+Infinityd",
                    "+InfinityD",

                    "NaNp10",
                    "-NaNp10",
                    "+NaNp10",
                    "Infinityp10",
                    "-Infinityp10",
                    "+Infinityp10",

                    // Non-ASCII digits are not recognized
                    "\u0661p\u0661", // 1e1 in Arabic-Indic digits
                    "\u06F1p\u06F1", // 1e1 in Extended Arabic-Indic digits
                    "\u0967p\u0967" // 1e1 in Devanagari digits
                };


                private static string[] goodStrings = {
                    "NaN",
                    "+NaN",
                    "-NaN",
                    "Infinity",
                    "+Infinity",
                    "-Infinity",
                    "1.1e-23f",
                    ".1e-23f",
                    "1e-23",
                    "1f",
                    "1",
                    "2",
                    "1234",
                    "-1234",
                    "+1234",
                    "2147483647",   // Integer.MAX_VALUE
                    "2147483648",
                    "-2147483648",  // Integer.MIN_VALUE
                    "-2147483649",

                    "16777215",
                    "16777216",     // 2^24
                    "16777217",

                    "-16777215",
                    "-16777216",    // -2^24
                    "-16777217",

                    "9007199254740991",
                    "9007199254740992",     // 2^53
                    "9007199254740993",

                    "-9007199254740991",
                    "-9007199254740992",    // -2^53
                    "-9007199254740993",

                    "9223372036854775807",
                    "9223372036854775808",  // Long.MAX_VALUE
                    "9223372036854775809",

                    "-9223372036854775808",
                    "-9223372036854775809", // Long.MIN_VALUE
                    "-9223372036854775810"
                };

                private static float[] goodStringExpecteds = new float[] {
                    float.NaN,
                    float.NaN,
                    float.NaN,
                    float.PositiveInfinity,
                    float.PositiveInfinity,
                    float.NegativeInfinity,
                    1.1E-23f,
                    1E-24f,
                    1E-23f,
                    1f,
                    1f,
                    2f,
                    1234f,
                    -1234f,
                    1234f,
                    2.1474836E+09f, // Integer.MAX_VALUE
                    2.1474836E+09f,
                    -2.1474836E+09f, // Integer.MIN_VALUE
                    -2.1474836E+09f,

                    16777215f,
                    16777216f,    // 2^24
                    16777216f,

                    -16777215f,
                    -16777216f,    // -2^24
                    -16777216f,

                    9.007199E+15f,
                    9.007199E+15f,     // 2^53
                    9.007199E+15f,

                    -9.007199E+15f,
                    -9.007199E+15f,     // -2^53
                    -9.007199E+15f,

                    9.223372E+18f,
                    9.223372E+18f,  // Long.MAX_VALUE
                    9.223372E+18f,

                    -9.223372E+18f,
                    -9.223372E+18f, // Long.MIN_VALUE
                    -9.223372E+18f,
                };

                private static string[] goodHexStrings = {
                    "NaN",
                    "+NaN",
                    "-NaN",
                    "Infinity",
                    "+Infinity",
                    "-Infinity",
                    "1.1p-23f",
                    ".1p-23f",
                    "1p-23",
                    "1f",
                    "1",
                    "2",
                    "1234",
                    "-1234",
                    "+1234",
                    "2147483647",   // Integer.MAX_VALUE
                    "2147483648",
                    "-2147483648",  // Integer.MIN_VALUE
                    "-2147483649",

                    "16777215",
                    "16777216",     // 2^24
                    "16777217",

                    "-16777215",
                    "-16777216",    // -2^24
                    "-16777217",

                    "9007199254740991",
                    "9007199254740992",     // 2^53
                    "9007199254740993",

                    "-9007199254740991",
                    "-9007199254740992",    // -2^53
                    "-9007199254740993",

                    "9223372036854775807",
                    "9223372036854775808",  // Long.MAX_VALUE
                    "9223372036854775809",

                    "-9223372036854775808",
                    "-9223372036854775809", // Long.MIN_VALUE
                    "-9223372036854775810"
                };

                private static float[] goodHexStringsExpecteds = new float[] {
                    float.NaN,
                    float.NaN,
                    float.NaN,
                    float.PositiveInfinity,
                    float.PositiveInfinity,
                    float.NegativeInfinity,
                    1.2665987E-07f,
                    7.450581E-09f,
                    1.1920929E-07f,
                    62f,
                    2f,
                    4f,
                    9320f,
                    -9320f,
                    9320f,
                    2.8585968E+11f,   // Integer.MAX_VALUE
                    2.8585968E+11f,
                    -2.8585968E+11f,  // Integer.MIN_VALUE
                    -2.8585968E+11f,

                    753853500f,
                    753853500f,     // 2^24
                    753853500f,

                    -753853500f,
                    -753853500f,    // -2^24
                    -753853500f,

                    2.0756585E+19f,
                    2.0756585E+19f,     // 2^53
                    2.0756585E+19f,

                    -2.0756585E+19f,
                    -2.0756585E+19f,    // -2^53
                    -2.0756585E+19f,

                    8.626439E+22f,
                    8.626439E+22f,  // Long.MAX_VALUE
                    8.626439E+22f,

                    -8.626439E+22f,
                    -8.626439E+22f, // Long.MIN_VALUE
                    -8.626439E+22f,
                };

                // J2N: The .NET parser doesn't remove the \u0001 or \u001f characters, so we are omitting them in tests
                private static string pad = " \t\n\r\f\u000b"; /* " \t\n\r\f\u0001\u000b\u001f"; */

                private static string[] paddedBadStrings = LoadPaddedBadStrings();
                private static string[] paddedBadHexStrings = LoadPaddedBadHexStrings();
                private static string[] paddedGoodStrings = LoadPaddedGoodStrings();
                private static string[] paddedGoodHexStrings = LoadPaddedGoodHexStrings();

                private static string[] LoadPaddedBadStrings()
                {
                    var result = new string[badStrings.Length];
                    for (int i = 0; i < badStrings.Length; i++)
                        result[i] = pad + badStrings[i] + pad;
                    return result;
                }

                private static string[] LoadPaddedBadHexStrings()
                {
                    var result = new string[badHexStrings.Length];
                    for (int i = 0; i < badHexStrings.Length; i++)
                        result[i] = pad + badHexStrings[i] + pad;
                    return result;
                }

                private static string[] LoadPaddedGoodStrings()
                {
                    var result = new string[goodStrings.Length];
                    for (int i = 0; i < goodStrings.Length; i++)
                        result[i] = pad + goodStrings[i] + pad;
                    return result;
                }

                private static string[] LoadPaddedGoodHexStrings()
                {
                    var result = new string[goodHexStrings.Length];
                    for (int i = 0; i < goodHexStrings.Length; i++)
                        result[i] = pad + goodHexStrings[i] + pad;
                    return result;
                }

                public static IEnumerable<TestCaseData> TestParse_CharSequence_NumberStyle_IFormatProvider_ForFloatStyle_Data
                {
                    get
                    {
                        // JDK 8

                        yield return new TestCaseData(float.Epsilon, "" + float.Epsilon, NumberStyle.Float, J2N.Text.StringFormatter.InvariantCulture);
                        yield return new TestCaseData(float.MaxValue, "" + float.MaxValue, NumberStyle.Float, J2N.Text.StringFormatter.InvariantCulture);

                        yield return new TestCaseData((float)10.0, "10", NumberStyle.Float, J2N.Text.StringFormatter.InvariantCulture);
                        yield return new TestCaseData((float)10.0, "10.0", NumberStyle.Float, J2N.Text.StringFormatter.InvariantCulture);
                        yield return new TestCaseData((float)10.01, "10.01", NumberStyle.Float, J2N.Text.StringFormatter.InvariantCulture);

                        yield return new TestCaseData((float)-10.0, "-10", NumberStyle.Float, J2N.Text.StringFormatter.InvariantCulture);
                        yield return new TestCaseData((float)-10.0, "-10.00", NumberStyle.Float, J2N.Text.StringFormatter.InvariantCulture);
                        yield return new TestCaseData((float)-10.01, "-10.01", NumberStyle.Float, J2N.Text.StringFormatter.InvariantCulture);

                        // bug 6358355
                        yield return new TestCaseData(/*0x1.000002p57f*/ 1.44115205E17f, "144115196665790480", NumberStyle.Float, J2N.Text.StringFormatter.InvariantCulture); // J2N: Hex literals not supported in .NET
                        yield return new TestCaseData(/*0x1.000002p57f*/ 1.44115205E17f, "144115196665790481", NumberStyle.Float, J2N.Text.StringFormatter.InvariantCulture); // J2N: Hex literals not supported in .NET
                        yield return new TestCaseData(0.05f, "0.050000002607703203", NumberStyle.Float, J2N.Text.StringFormatter.InvariantCulture);
                        yield return new TestCaseData(0.05f, "0.050000002607703204", NumberStyle.Float, J2N.Text.StringFormatter.InvariantCulture);
                        yield return new TestCaseData(0.05f, "0.050000002607703205", NumberStyle.Float, J2N.Text.StringFormatter.InvariantCulture);
                        yield return new TestCaseData(0.05f, "0.050000002607703206", NumberStyle.Float, J2N.Text.StringFormatter.InvariantCulture);
                        yield return new TestCaseData(0.05f, "0.050000002607703207", NumberStyle.Float, J2N.Text.StringFormatter.InvariantCulture);
                        yield return new TestCaseData(0.05f, "0.050000002607703208", NumberStyle.Float, J2N.Text.StringFormatter.InvariantCulture);
                        yield return new TestCaseData(0.050000004f, "0.050000002607703209", NumberStyle.Float, J2N.Text.StringFormatter.InvariantCulture);

                        for (int i = 0; i < goodStrings.Length; i++)
                        {
                            string inputString = goodStrings[i];
                            NumberStyle styles = NumberStyle.Float;
                            if (inputString.EndsWith("f", StringComparison.OrdinalIgnoreCase) || inputString.EndsWith("d", StringComparison.OrdinalIgnoreCase))
                                styles |= NumberStyle.AllowTypeSpecifier;
                            var provider = J2N.Text.StringFormatter.InvariantCulture;

                            yield return new TestCaseData(goodStringExpecteds[i], inputString, styles, provider);
                        }

                        for (int i = 0; i < paddedGoodStrings.Length; i++)
                        {
                            string inputString = paddedGoodStrings[i];
                            NumberStyle styles = NumberStyle.Float;
                            if (inputString.TrimEnd().EndsWith("f", StringComparison.OrdinalIgnoreCase) || inputString.TrimEnd().EndsWith("d", StringComparison.OrdinalIgnoreCase))
                                styles |= NumberStyle.AllowTypeSpecifier;
                            var provider = J2N.Text.StringFormatter.InvariantCulture;

                            // Pass through the same value - all we care about here is that the input is accepted without error
                            yield return new TestCaseData(goodStringExpecteds[i], inputString, styles, provider);
                        }


                        // Harmony (Test_parseFloatLjava_lang_String())

                        yield return new TestCaseData(0.0f, "7.0064923216240853546186479164495e-46", NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(float.Epsilon, "7.0064923216240853546186479164496e-46", NumberStyle.Float, NumberFormatInfo.InvariantInfo); // J2N: In .NET float.Epsilon is the same as Float.MIN_VALUE in Java

                        // Custom

                        yield return new TestCaseData(1.8f, "1.8e0", NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(1.8f, "1,8e0", NumberStyle.Float, new NumberFormatInfo { NumberDecimalSeparator = "," });
                        yield return new TestCaseData(1.8f, "1--8e0", NumberStyle.Float, new NumberFormatInfo { NumberDecimalSeparator = "--" });
                        yield return new TestCaseData(1.8f, "1.8", NumberStyle.Float, NumberFormatInfo.InvariantInfo);

                        // Negative sign format tests
                        yield return new TestCaseData(-1.8f, "-1.8e0", NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(-1.8f, "-1.8", NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(-1.0f, "-1.", NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(-1.8f, "- 1.8e0", NumberStyle.Float, new NumberFormatInfo { NumberNegativePattern = 2 });
                        yield return new TestCaseData(-1.8f, "- 1.8", NumberStyle.Float, new NumberFormatInfo { NumberNegativePattern = 2 });
                        yield return new TestCaseData(-1.0f, "- 1.", NumberStyle.Float, new NumberFormatInfo { NumberNegativePattern = 2 });

                        yield return new TestCaseData(-1.8f, "1.8e0-", NumberStyle.Float | NumberStyle.AllowTrailingSign, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(-1.8f, "1.8-", NumberStyle.Float | NumberStyle.AllowTrailingSign, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(-1.0f, "1.-", NumberStyle.Float | NumberStyle.AllowTrailingSign, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(-1.8f, "1.8e0 -", NumberStyle.Float | NumberStyle.AllowTrailingSign, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(-1.8f, "1.8 -", NumberStyle.Float | NumberStyle.AllowTrailingSign, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(-1.0f, "1. -", NumberStyle.Float | NumberStyle.AllowTrailingSign, NumberFormatInfo.InvariantInfo);

                        yield return new TestCaseData(-1.8f, "(1.8e0)", NumberStyle.Float | NumberStyle.AllowParentheses, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(-1.8f, "(1.8)", NumberStyle.Float | NumberStyle.AllowParentheses, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(-1.0f, "(1.)", NumberStyle.Float | NumberStyle.AllowParentheses, NumberFormatInfo.InvariantInfo);

                        // Constant values

                        yield return new TestCaseData(float.NaN, "NaN", NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(float.NaN, "+NaN", NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(float.NaN, "-NaN", NumberStyle.Float, NumberFormatInfo.InvariantInfo);

                        yield return new TestCaseData(float.PositiveInfinity, "Infinity", NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(float.PositiveInfinity, "+Infinity", NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(float.NegativeInfinity, "-Infinity", NumberStyle.Float, NumberFormatInfo.InvariantInfo);


                        // J2N TODO: If/when the integral type parsers are ported, use similar tests to these to confirm them
                        //yield return new TestCaseData(1.8f, "1.8e0u", NumberStyle.Float | NumberStyle.AllowTypeSpecifier, NumberFormatInfo.InvariantInfo);
                        //yield return new TestCaseData(1.8f, "1.8e0l", NumberStyle.Float | NumberStyle.AllowTypeSpecifier, NumberFormatInfo.InvariantInfo);
                        //yield return new TestCaseData(1.8f, "1.8e0L", NumberStyle.Float | NumberStyle.AllowTypeSpecifier, NumberFormatInfo.InvariantInfo);
                        //yield return new TestCaseData(1.8f, "1.8e0ul", NumberStyle.Float | NumberStyle.AllowTypeSpecifier, NumberFormatInfo.InvariantInfo);
                        //yield return new TestCaseData(1.8f, "1.8e0UL", NumberStyle.Float | NumberStyle.AllowTypeSpecifier, NumberFormatInfo.InvariantInfo);
                        //yield return new TestCaseData(1.8f, "1.8e0uL", NumberStyle.Float | NumberStyle.AllowTypeSpecifier, NumberFormatInfo.InvariantInfo);
                        //yield return new TestCaseData(1.8f, "1.8e0Ul", NumberStyle.Float | NumberStyle.AllowTypeSpecifier, NumberFormatInfo.InvariantInfo);
                        //yield return new TestCaseData(1.8f, "1.8e0lu", NumberStyle.Float | NumberStyle.AllowTypeSpecifier, NumberFormatInfo.InvariantInfo);
                        //yield return new TestCaseData(1.8f, "1.8e0LU", NumberStyle.Float | NumberStyle.AllowTypeSpecifier, NumberFormatInfo.InvariantInfo);
                        //yield return new TestCaseData(1.8f, "1.8e0lU", NumberStyle.Float | NumberStyle.AllowTypeSpecifier, NumberFormatInfo.InvariantInfo);
                        //yield return new TestCaseData(1.8f, "1.8e0Lu", NumberStyle.Float | NumberStyle.AllowTypeSpecifier, NumberFormatInfo.InvariantInfo);


                        // Decimal type specifier

                        yield return new TestCaseData(1.8f, "1.8e0m", NumberStyle.Float | NumberStyle.AllowTypeSpecifier, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(1.8f, "1.8e0M", NumberStyle.Float | NumberStyle.AllowTypeSpecifier, NumberFormatInfo.InvariantInfo);
                    }
                }

                public static IEnumerable<TestCaseData> TestParse_CharSequence_NumberStyle_IFormatProvider_ForHexFloatStyle_Data
                {
                    get
                    {
                        // JDK 8

                        for (int i = 0; i < goodHexStrings.Length; i++)
                        {
                            string inputString = goodHexStrings[i];
                            NumberStyle styles = NumberStyle.HexFloat;
                            if (inputString.TrimEnd().EndsWith("f", StringComparison.OrdinalIgnoreCase) || inputString.TrimEnd().EndsWith("d", StringComparison.OrdinalIgnoreCase))
                                styles |= NumberStyle.AllowTypeSpecifier;
                            var provider = J2N.Text.StringFormatter.InvariantCulture;

                            yield return new TestCaseData(goodHexStringsExpecteds[i], inputString, styles, provider);
                        }

                        //for (int i = 0; i < paddedGoodHexStrings.Length; i++)
                        //{
                        //    string inputString = paddedGoodHexStrings[i];
                        //    NumberStyle styles = NumberStyle.HexFloat;
                        //    if (inputString.TrimEnd().EndsWith("f", StringComparison.OrdinalIgnoreCase) || inputString.TrimEnd().EndsWith("d", StringComparison.OrdinalIgnoreCase))
                        //        styles |= NumberStyle.AllowTypeSpecifier;
                        //    var provider = J2N.Text.StringFormatter.InvariantCulture;

                        //    // Pass through the same value - all we care about here is that the input is accepted without error
                        //    yield return new TestCaseData(goodHexStringsExpecteds[i], inputString, styles, provider);
                        //}

                        // JDK 8 (ParseHexFloatingPoint.floatTests())

                        string[][] roundingTestCases = {
                            // Target float value       hard rouding version

                            new string[] {"0x1.000000p0",    "0x1.0000000000001p0"},

                            // Try some values that should round up to nextUp(1.0f)
                            new string[] {"0x1.000002p0",    "0x1.0000010000001p0"},
                            new string[] {"0x1.000002p0",    "0x1.00000100000008p0"},
                            new string[] {"0x1.000002p0",    "0x1.0000010000000fp0"},
                            new string[] {"0x1.000002p0",    "0x1.00000100000001p0"},
                            new string[] {"0x1.000002p0",    "0x1.00000100000000000000000000000000000000001p0"},
                            new string[] {"0x1.000002p0",    "0x1.0000010000000fp0"},

                            // Potential double rounding cases
                            new string[] {"0x1.000002p0",    "0x1.000002fffffffp0"},
                            new string[] {"0x1.000002p0",    "0x1.000002fffffff8p0"},
                            new string[] {"0x1.000002p0",    "0x1.000002ffffffffp0"},

                            new string[] {"0x1.000002p0",    "0x1.000002ffff0ffp0"},
                            new string[] {"0x1.000002p0",    "0x1.000002ffff0ff8p0"},
                            new string[] {"0x1.000002p0",    "0x1.000002ffff0fffp0"},


                            new string[] {"0x1.000000p0",    "0x1.000000fffffffp0"},
                            new string[] {"0x1.000000p0",    "0x1.000000fffffff8p0"},
                            new string[] {"0x1.000000p0",    "0x1.000000ffffffffp0"},

                            new string[] {"0x1.000000p0",    "0x1.000000ffffffep0"},
                            new string[] {"0x1.000000p0",    "0x1.000000ffffffe8p0"},
                            new string[] {"0x1.000000p0",    "0x1.000000ffffffefp0"},

                            // Float subnormal cases
                            new string[] {"0x0.000002p-126", "0x0.0000010000001p-126"},
                            new string[] {"0x0.000002p-126", "0x0.00000100000000000001p-126"},

                            new string[] {"0x0.000006p-126", "0x0.0000050000001p-126"},
                            new string[] {"0x0.000006p-126", "0x0.00000500000000000001p-126"},

                            new string[] {"0x0.0p-149",      "0x0.7ffffffffffffffp-149"},
                            new string[] {"0x1.0p-148",      "0x1.3ffffffffffffffp-148"},
                            new string[] {"0x1.cp-147",      "0x1.bffffffffffffffp-147"},

                            new string[] {"0x1.fffffcp-127", "0x1.fffffdffffffffp-127"},
                        };

                        string[] signs = { "", "-" };

                        for (int i = 0; i < roundingTestCases.Length; i++)
                        {
                            for (int j = 0; j < signs.Length; j++)
                            {
                                string expectedIn = signs[j] + roundingTestCases[i][0];
                                string resultIn = signs[j] + roundingTestCases[i][1];

                                yield return new TestCaseData(Single.Parse(expectedIn, NumberStyle.HexFloat, NumberFormatInfo.InvariantInfo),
                                    resultIn, NumberStyle.HexFloat, NumberFormatInfo.InvariantInfo);
                            }
                        }

                        // Harmony (Test_parseFloat_LString_MaxNormalBoundary())

                        int[] expecteds1 ={
                                0x7f7fffff,
                                0x7f7fffff,
                                0x7f7fffff,
                                0x7f800000,
                                0x7f800000,
                                0x7f800000,

                                unchecked((int)0xff7fffff),
                                unchecked((int)0xff7fffff),
                                unchecked((int)0xff7fffff),
                                unchecked((int)0xff800000),
                                unchecked((int)0xff800000),
                                unchecked((int)0xff800000),
                        };

                        string[] inputs1 = {
                            "0x1.fffffep127",
                            "0x1.fffffe000000000000000000000000000000000000000000000001p127",
                            "0x1.fffffeffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffp127",
                            "0x1.ffffffp127",
                            "0x1.ffffff000000000000000000000000000000000000000000000001p127",
                            "0x1.ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffp127",

                            "-0x1.fffffep127",
                            "-0x1.fffffe000000000000000000000000000000000000000000000001p127",
                            "-0x1.fffffeffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffp127",
                            "-0x1.ffffffp127",
                            "-0x1.ffffff000000000000000000000000000000000000000000000001p127",
                            "-0x1.ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffp127",
                        };

                        for (int i = 0; i < inputs1.Length; i++)
                        {
                            yield return new TestCaseData(BitConversion.Int32BitsToSingle(expecteds1[i]), inputs1[i], NumberStyle.HexFloat, J2N.Text.StringFormatter.InvariantCulture);
                        }

                        // Harmony (Test_parseFloat_LString_MaxSubNormalBoundary())

                        int[] expecteds2 = {
                            0x7fffff,
                            0x7fffff,
                            0x7fffff,
                            0x800000,
                            0x800000,
                            0x800000,

                            unchecked((int)0x807fffff),
                            unchecked((int)0x807fffff),
                            unchecked((int)0x807fffff),
                            unchecked((int)0x80800000),
                            unchecked((int)0x80800000),
                            unchecked((int)0x80800000),
                        };

                        string[] inputs2 = {
                            "0x0.fffffep-126",
                            "0x0.fffffe000000000000000000000000000000000000000000000000000001p-126",
                            "0x0.fffffefffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffp-126",
                            "0x0.ffffffp-126",
                            "0x0.ffffff0000000000000000000000000000000000000000000000000000001p-126",
                            "0x0.ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffp-126",

                            "-0x0.fffffep-126",
                            "-0x0.fffffe000000000000000000000000000000000000000000000000000001p-126",
                            "-0x0.fffffefffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffp-126",
                            "-0x0.ffffffp-126",
                            "-0x0.ffffff0000000000000000000000000000000000000000000000000000001p-126",
                            "-0x0.ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffp-126",
                        };

                        for (int i = 0; i < inputs2.Length; i++)
                        {
                            yield return new TestCaseData(BitConversion.Int32BitsToSingle(expecteds2[i]), inputs2[i], NumberStyle.HexFloat, J2N.Text.StringFormatter.InvariantCulture);
                        }

                        // Harmony (Test_parseFloat_LString_MinSubNormalBoundary())

                        int[] expecteds3 = {
                            0x1,
                            0x1,
                            0x1,
                            0x2,
                            0x2,
                            0x2,

                            unchecked((int)0x80000001),
                            unchecked((int)0x80000001),
                            unchecked((int)0x80000001),
                            unchecked((int)0x80000002),
                            unchecked((int)0x80000002),
                            unchecked((int)0x80000002),
                        };

                        string[] inputs3 = {
                            "0x0.000002p-126",
                            "0x0.00000200000000000000000000000000000000000001p-126",
                            "0x0.000002ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffp-126",
                            "0x0.000003p-126",
                            "0x0.000003000000000000000000000000000000000000001p-126",
                            "0x0.000003ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffp-126",

                            "-0x0.000002p-126",
                            "-0x0.00000200000000000000000000000000000000000001p-126",
                            "-0x0.000002ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffp-126",
                            "-0x0.000003p-126",
                            "-0x0.000003000000000000000000000000000000000000001p-126",
                            "-0x0.000003ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffp-126",
                        };

                        for (int i = 0; i < inputs3.Length; i++)
                        {
                            yield return new TestCaseData(BitConversion.Int32BitsToSingle(expecteds3[i]), inputs3[i], NumberStyle.HexFloat, J2N.Text.StringFormatter.InvariantCulture);
                        }

                        // Harmony (Test_parseFloat_LString_MinNormalBoundary())

                        int[] expecteds4 = {
                            0x800000,
                            0x800000,
                            0x800000,
                            0x800000,
                            0x800001,
                            0x800001,

                            unchecked((int)0x80800000),
                            unchecked((int)0x80800000),
                            unchecked((int)0x80800000),
                            unchecked((int)0x80800000),
                            unchecked((int)0x80800001),
                            unchecked((int)0x80800001),
                        };

                        string[] inputs4 = {
                            "0x1.0p-126",
                            "0x1.00000000000000000000000000000000000000000000001p-126",
                            "0x1.000000ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffp-126",
                            "0x1.000001p-126",
                            "0x1.000001000000000000000000000000000000000000000001p-126",
                            "0x1.000001fffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffp-126",

                            "-0x1.0p-126",
                            "-0x1.00000000000000000000000000000000000000000000001p-126",
                            "-0x1.000000ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffp-126",
                            "-0x1.000001p-126",
                            "-0x1.000001000000000000000000000000000000000000000001p-126",
                            "-0x1.000001fffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffp-126",
                        };
                        for (int i = 0; i < inputs4.Length; i++)
                        {
                            yield return new TestCaseData(BitConversion.Int32BitsToSingle(expecteds4[i]), inputs4[i], NumberStyle.HexFloat, J2N.Text.StringFormatter.InvariantCulture);
                        }

                        // Harmony (Test_parseFloat_LString_NormalNegativeExponent())

                        int[] expecteds5 = {
                            0x3991a2b4,
                            0x3d6e0247,
                            0x3aa0a009,
                            0x37848405,
                            0x3420a005,
                            0x30d4d405,
                            0x2d848402,
                            0x2a129202,
                            0x26acac02,
                            0x2346c602,
                            0x1fe0e002,
                            0x1c6eee02,
                            0x19048401,
                            0x15919101,
                            0x12189801,
                            0xf028828,
                            0xb890890,
                            0x80c88c8,
                            0x4930930,
                            0x1198998,
                            0x28028,
                            0x51c,
                            0xb,
                            0x0,
                            0x0,
                        };

                        for (int i = 0; i < expecteds5.Length; i++)
                        {
                            int part = i * 7;
                            string inputString = "0x" + part.ToString(NumberFormatInfo.InvariantInfo) + "."
                                + part.ToString(NumberFormatInfo.InvariantInfo) + "0123456789abcdefp-" + part.ToString(NumberFormatInfo.InvariantInfo);

                            yield return new TestCaseData(BitConversion.Int32BitsToSingle(expecteds5[i]), inputString, NumberStyle.HexFloat, J2N.Text.StringFormatter.InvariantCulture);
                        }

                        // Harmony (Test_parseFloat_LString_NormalPositiveExponent())

                        int[] expecteds6 = {
                            0x3991a2b4,                0x43cc0247,                0x47909009,
                            0x4ac0c009,                0x4e109005,                0x5140c005,
                            0x5458d805,                0x57848402,                0x5a909002,
                            0x5da8a802,                0x60c0c002,                0x63cccc02,
                            0x66e4e402,                0x69f0f002,                0x6d048401,
                            0x70109001,                0x73169601,                0x76810810,
                            0x79840840,                0x7c8a08a0,                0x7f800000,
                            0x7f800000,                0x7f800000,                0x7f800000,
                            0x7f800000,
                        };

                        for (int i = 0; i < expecteds6.Length; i++)
                        {
                            int part = i * 6;
                            string inputString = "0x" + part.ToString(NumberFormatInfo.InvariantInfo) + "."
                                + part.ToString(NumberFormatInfo.InvariantInfo) + "0123456789abcdefp" + part.ToString(NumberFormatInfo.InvariantInfo);

                            yield return new TestCaseData(BitConversion.Int32BitsToSingle(expecteds6[i]), inputString, NumberStyle.HexFloat, J2N.Text.StringFormatter.InvariantCulture);
                        }

                        // Harmony (Test_parseFloat_LString_Unusual())

                        yield return new TestCaseData(0.0f, "0x00000000000000000000000000000000000000000.0000000000000000000000000000000000000p0000000000000000000000000000000000", NumberStyle.HexFloat, J2N.Text.StringFormatter.InvariantCulture);
                        yield return new TestCaseData(6.64614E35f, "+0Xfffff.fffffffffffffffffffffffffffffffp+99F", NumberStyle.HexFloat | NumberStyle.AllowTypeSpecifier, J2N.Text.StringFormatter.InvariantCulture);
                        yield return new TestCaseData(-4.5072022E28f, "-0X.123456789abcdefp+99f", NumberStyle.HexFloat | NumberStyle.AllowTypeSpecifier, J2N.Text.StringFormatter.InvariantCulture);
                        yield return new TestCaseData(-1.63971062E17f, "-0X123456789abcdef.p+1f", NumberStyle.HexFloat | NumberStyle.AllowTypeSpecifier, J2N.Text.StringFormatter.InvariantCulture);
                        yield return new TestCaseData(-4.48585472E8f, "-0X000000000000000000000000000001abcdef.0000000000000000000000000001abefp00000000000000000000000000000000000000000004f", NumberStyle.HexFloat | NumberStyle.AllowTypeSpecifier, J2N.Text.StringFormatter.InvariantCulture);
                        yield return new TestCaseData(5.907252E33f, "0X0.00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000001234p600f", NumberStyle.HexFloat | NumberStyle.AllowTypeSpecifier, J2N.Text.StringFormatter.InvariantCulture);
                        yield return new TestCaseData(float.PositiveInfinity, "0x1.p9223372036854775807", NumberStyle.HexFloat, J2N.Text.StringFormatter.InvariantCulture);
                        yield return new TestCaseData(float.PositiveInfinity, "0x1.p9223372036854775808", NumberStyle.HexFloat, J2N.Text.StringFormatter.InvariantCulture);
                        yield return new TestCaseData(float.PositiveInfinity, "0x10.p9223372036854775808", NumberStyle.HexFloat, J2N.Text.StringFormatter.InvariantCulture);
                        yield return new TestCaseData(float.PositiveInfinity, "0xabcd.ffffffffp+2000", NumberStyle.HexFloat, J2N.Text.StringFormatter.InvariantCulture);
                        yield return new TestCaseData(0.0f, "0x1.p-9223372036854775808", NumberStyle.HexFloat, J2N.Text.StringFormatter.InvariantCulture);
                        yield return new TestCaseData(0.0f, "0x1.p-9223372036854775809", NumberStyle.HexFloat, J2N.Text.StringFormatter.InvariantCulture);
                        yield return new TestCaseData(0.0f, "0x.1p-9223372036854775809", NumberStyle.HexFloat, J2N.Text.StringFormatter.InvariantCulture);

                        // Harmony (Test_parseFloat_LString_ZeroBoundary())

                        int[] expecteds7 = {
                            0x0,
                            0x0,
                            0x0,
                            0x0,
                            0x1,
                            0x1,

                            unchecked((int)0x80000000),
                            unchecked((int)0x80000000),
                            unchecked((int)0x80000000),
                            unchecked((int)0x80000000),
                            unchecked((int)0x80000001),
                            unchecked((int)0x80000001),
                        };

                        string[] inputs7 = {
                            "0x0.000000000000000p-126",
                            "0x0.000000000000000000000000000000000000000000000001p-126",
                            "0x0.000000fffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffp-126",
                            "0x0.000001p-126",
                            "0x0.000001000000000000000000000000000000000000000001p-126",
                            "0x0.000001fffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffp-126",

                            "-0x0.000000000000000p-126",
                            "-0x0.000000000000000000000000000000000000000000000001p-126",
                            "-0x0.000000fffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffp-126",
                            "-0x0.000001p-126",
                            "-0x0.000001000000000000000000000000000000000000000001p-126",
                            "-0x0.000001fffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffp-126",
                        };

                        for (int i = 0; i < inputs7.Length; i++)
                        {
                            yield return new TestCaseData(BitConversion.Int32BitsToSingle(expecteds7[i]), inputs7[i], NumberStyle.HexFloat, J2N.Text.StringFormatter.InvariantCulture);
                        }

                        // Harmony (Test_parseFloatLjava_lang_String())

                        yield return new TestCaseData(0.0f, "7.0064923216240853546186479164495e-46", NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(float.Epsilon, "7.0064923216240853546186479164496e-46", NumberStyle.Float, NumberFormatInfo.InvariantInfo); // J2N: In .NET float.Epsilon is the same as Float.MIN_VALUE in Java

                        // Custom

                        yield return new TestCaseData(3.0f, "0x1.8p1", NumberStyle.HexFloat, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(3.0f, "0x1,8p1", NumberStyle.HexFloat, new NumberFormatInfo { NumberDecimalSeparator = "," });
                        yield return new TestCaseData(3.0f, "0x1--8p1", NumberStyle.HexFloat, new NumberFormatInfo { NumberDecimalSeparator = "--" });
                        yield return new TestCaseData(3.0f, "0x1.8", NumberStyle.HexFloat, NumberFormatInfo.InvariantInfo);

                        // Negative sign format tests
                        yield return new TestCaseData(-3.0f, "-0x1.8p1", NumberStyle.HexFloat, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(-3.0f, "-0x1.8", NumberStyle.HexFloat, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(-2.0f, "-0x1.", NumberStyle.HexFloat, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(-3.0f, "- 0x1.8p1", NumberStyle.HexFloat, new NumberFormatInfo { NumberNegativePattern = 2 });
                        yield return new TestCaseData(-3.0f, "- 0x1.8", NumberStyle.HexFloat, new NumberFormatInfo { NumberNegativePattern = 2 });
                        yield return new TestCaseData(-2.0f, "- 0x1.", NumberStyle.HexFloat, new NumberFormatInfo { NumberNegativePattern = 2 });

                        // Constant values

                        yield return new TestCaseData(float.NaN, "NaN", NumberStyle.HexFloat, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(float.NaN, "+NaN", NumberStyle.HexFloat, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(float.NaN, "-NaN", NumberStyle.HexFloat, NumberFormatInfo.InvariantInfo);

                        yield return new TestCaseData(float.PositiveInfinity, "Infinity", NumberStyle.HexFloat, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(float.PositiveInfinity, "+Infinity", NumberStyle.HexFloat, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(float.NegativeInfinity, "-Infinity", NumberStyle.HexFloat, NumberFormatInfo.InvariantInfo);
                    }
                }


                public static IEnumerable<TestCaseData> TestParse_CharSequence_NumberStyle_IFormatProvider_ForFloatStyleException_Data
                {
                    get
                    {
                        //JDK 8

                        for (int i = 0; i < badStrings.Length; i++)
                        {
                            string inputString = badStrings[i];
                            NumberStyle styles = NumberStyle.Float;
                            if (inputString.TrimEnd().EndsWith("f", StringComparison.OrdinalIgnoreCase) || inputString.TrimEnd().EndsWith("d", StringComparison.OrdinalIgnoreCase))
                                styles |= NumberStyle.AllowTypeSpecifier;
                            var provider = J2N.Text.StringFormatter.InvariantCulture;

                            // Pass through the same value - all we care about here is that the input is accepted without error
                            yield return new TestCaseData(typeof(FormatException), inputString, styles, provider, "Bad input not expected to be parsed.");
                        }

                        for (int i = 0; i < paddedBadStrings.Length; i++)
                        {
                            string inputString = paddedBadStrings[i];
                            NumberStyle styles = NumberStyle.Float;
                            if (inputString.TrimEnd().EndsWith("f", StringComparison.OrdinalIgnoreCase) || inputString.TrimEnd().EndsWith("d", StringComparison.OrdinalIgnoreCase) )
                                styles |= NumberStyle.AllowTypeSpecifier;
                            var provider = J2N.Text.StringFormatter.InvariantCulture;

                            // Pass through the same value - all we care about here is that the input is accepted without error
                            yield return new TestCaseData(typeof(FormatException), inputString, styles, provider, "Bad input not expected to be parsed.");
                        }

                        // Custom

                        yield return new TestCaseData(typeof(FormatException), "1.8e1-", NumberStyle.Float, NumberFormatInfo.InvariantInfo, "NumberStyle.Float doesn't allow trailing negative sign.");
                        yield return new TestCaseData(typeof(FormatException), "1.8-", NumberStyle.Float, NumberFormatInfo.InvariantInfo, "NumberStyle.Float doesn't allow trailing negative sign.");
                        yield return new TestCaseData(typeof(FormatException), "1.-", NumberStyle.Float, NumberFormatInfo.InvariantInfo, "NumberStyle.Float doesn't allow trailing negative sign.");
                        yield return new TestCaseData(typeof(FormatException), "1.8e1 -", NumberStyle.Float, NumberFormatInfo.InvariantInfo, "NumberStyle.Float doesn't allow trailing negative sign.");
                        yield return new TestCaseData(typeof(FormatException), "1.8 -", NumberStyle.Float, NumberFormatInfo.InvariantInfo, "NumberStyle.Float doesn't allow trailing negative sign.");
                        yield return new TestCaseData(typeof(FormatException), "1. -", NumberStyle.Float, NumberFormatInfo.InvariantInfo, "NumberStyle.Float doesn't allow trailing negative sign.");

                        yield return new TestCaseData(typeof(FormatException), "(1.8e1)", NumberStyle.Float, NumberFormatInfo.InvariantInfo, "NumberStyle.Float doesn't allow parentheses.");
                        yield return new TestCaseData(typeof(FormatException), "(1.8)", NumberStyle.Float, NumberFormatInfo.InvariantInfo, "NumberStyle.Float doesn't allow parentheses.");
                        yield return new TestCaseData(typeof(FormatException), "(1.)", NumberStyle.Float, NumberFormatInfo.InvariantInfo, "NumberStyle.Float doesn't allow parentheses.");

                        yield return new TestCaseData(typeof(FormatException), "1. -d", NumberStyle.Float | NumberStyle.AllowParentheses | NumberStyle.AllowTypeSpecifier, NumberFormatInfo.InvariantInfo, "Type specifier may not be after negative sign.");
                        yield return new TestCaseData(typeof(FormatException), "1.-d", NumberStyle.Float | NumberStyle.AllowParentheses | NumberStyle.AllowTypeSpecifier, NumberFormatInfo.InvariantInfo, "Type specifier may not be after negative sign.");
                        yield return new TestCaseData(typeof(FormatException), "(1.)d", NumberStyle.Float | NumberStyle.AllowParentheses | NumberStyle.AllowTypeSpecifier, NumberFormatInfo.InvariantInfo, "Type specifier may not be after negative sign.");

                        yield return new TestCaseData(typeof(FormatException), "(1.", NumberStyle.Float | NumberStyle.AllowParentheses | NumberStyle.AllowTypeSpecifier, NumberFormatInfo.InvariantInfo, "Not a complete set of parentheses.");
                        yield return new TestCaseData(typeof(FormatException), "1.)", NumberStyle.Float | NumberStyle.AllowParentheses | NumberStyle.AllowTypeSpecifier, NumberFormatInfo.InvariantInfo, "Not a complete set of parentheses.");

                        yield return new TestCaseData(typeof(FormatException), "1.8e0dd", NumberStyle.Float | NumberStyle.AllowTypeSpecifier, NumberFormatInfo.InvariantInfo, "Type specifier contains double characters.");
                        yield return new TestCaseData(typeof(FormatException), "1.8e0DD", NumberStyle.Float | NumberStyle.AllowTypeSpecifier, NumberFormatInfo.InvariantInfo, "Type specifier contains double characters.");
                        yield return new TestCaseData(typeof(FormatException), "1.8e0ff", NumberStyle.Float | NumberStyle.AllowTypeSpecifier, NumberFormatInfo.InvariantInfo, "Type specifier contains double characters.");
                        yield return new TestCaseData(typeof(FormatException), "1.8e0FF", NumberStyle.Float | NumberStyle.AllowTypeSpecifier, NumberFormatInfo.InvariantInfo, "Type specifier contains double characters.");
                        yield return new TestCaseData(typeof(FormatException), "1.8e0mm", NumberStyle.Float | NumberStyle.AllowTypeSpecifier, NumberFormatInfo.InvariantInfo, "Type specifier contains double characters.");
                        yield return new TestCaseData(typeof(FormatException), "1.8e0MM", NumberStyle.Float | NumberStyle.AllowTypeSpecifier, NumberFormatInfo.InvariantInfo, "Type specifier contains double characters.");


                        // J2N TODO: If/when the integral type parsers are ported, use similar tests to these to confirm them
                        //yield return new TestCaseData(typeof(FormatException), "1.8e0uu", NumberStyle.Float | NumberStyle.AllowTypeSpecifier, NumberFormatInfo.InvariantInfo, "Type specifier contains double characters.");
                        //yield return new TestCaseData(typeof(FormatException), "1.8e0UU", NumberStyle.Float | NumberStyle.AllowTypeSpecifier, NumberFormatInfo.InvariantInfo, "Type specifier contains double characters.");
                        //yield return new TestCaseData(typeof(FormatException), "1.8e0ll", NumberStyle.Float | NumberStyle.AllowTypeSpecifier, NumberFormatInfo.InvariantInfo, "Type specifier contains double characters.");
                        //yield return new TestCaseData(typeof(FormatException), "1.8e0LL", NumberStyle.Float | NumberStyle.AllowTypeSpecifier, NumberFormatInfo.InvariantInfo, "Type specifier contains double characters.");
                        //yield return new TestCaseData(typeof(FormatException), "1.8e0uU", NumberStyle.Float | NumberStyle.AllowTypeSpecifier, NumberFormatInfo.InvariantInfo, "Type specifier contains double characters.");
                        //yield return new TestCaseData(typeof(FormatException), "1.8e0Uu", NumberStyle.Float | NumberStyle.AllowTypeSpecifier, NumberFormatInfo.InvariantInfo, "Type specifier contains double characters.");
                        //yield return new TestCaseData(typeof(FormatException), "1.8e0lL", NumberStyle.Float | NumberStyle.AllowTypeSpecifier, NumberFormatInfo.InvariantInfo, "Type specifier contains double characters.");
                        //yield return new TestCaseData(typeof(FormatException), "1.8e0Ll", NumberStyle.Float | NumberStyle.AllowTypeSpecifier, NumberFormatInfo.InvariantInfo, "Type specifier contains double characters.");
                    }
                }

                public static IEnumerable<TestCaseData> TestParse_CharSequence_NumberStyle_IFormatProvider_ForHexFloatStyleException_Data
                {
                    get
                    {
                        //JDK 8

                        for (int i = 0; i < badHexStrings.Length; i++)
                        {
                            string inputString = badHexStrings[i];
                            NumberStyle styles = NumberStyle.HexFloat;
                            if (inputString.TrimEnd().EndsWith("f", StringComparison.OrdinalIgnoreCase) || inputString.TrimEnd().EndsWith("d", StringComparison.OrdinalIgnoreCase))
                                styles |= NumberStyle.AllowTypeSpecifier;
                            var provider = J2N.Text.StringFormatter.InvariantCulture;

                            // Pass through the same value - all we care about here is that the input is accepted without error
                            yield return new TestCaseData(typeof(FormatException), inputString, styles, provider, "Bad input not expected to be parsed.");
                        }

                        for (int i = 0; i < paddedBadHexStrings.Length; i++)
                        {
                            string inputString = paddedBadHexStrings[i];
                            NumberStyle styles = NumberStyle.HexFloat;
                            if (inputString.TrimEnd().EndsWith("f", StringComparison.OrdinalIgnoreCase) || inputString.TrimEnd().EndsWith("d", StringComparison.OrdinalIgnoreCase))
                                styles |= NumberStyle.AllowTypeSpecifier;
                            var provider = J2N.Text.StringFormatter.InvariantCulture;

                            // Pass through the same value - all we care about here is that the input is accepted without error
                            yield return new TestCaseData(typeof(FormatException), inputString, styles, provider, "Bad input not expected to be parsed.");
                        }

                        // Custom

                        yield return new TestCaseData(typeof(FormatException), "0x1.8p1-", NumberStyle.HexFloat, NumberFormatInfo.InvariantInfo, "NumberStyle.Float doesn't allow trailing negative sign.");
                        yield return new TestCaseData(typeof(FormatException), "0x1.8-", NumberStyle.HexFloat, NumberFormatInfo.InvariantInfo, "NumberStyle.Float doesn't allow trailing negative sign.");
                        yield return new TestCaseData(typeof(FormatException), "0x1.-", NumberStyle.HexFloat, NumberFormatInfo.InvariantInfo, "NumberStyle.Float doesn't allow trailing negative sign.");
                        yield return new TestCaseData(typeof(FormatException), "0x1.8p1 -", NumberStyle.HexFloat, NumberFormatInfo.InvariantInfo, "NumberStyle.Float doesn't allow trailing negative sign.");
                        yield return new TestCaseData(typeof(FormatException), "0x1.8 -", NumberStyle.HexFloat, NumberFormatInfo.InvariantInfo, "NumberStyle.Float doesn't allow trailing negative sign.");
                        yield return new TestCaseData(typeof(FormatException), "0x1. -", NumberStyle.HexFloat, NumberFormatInfo.InvariantInfo, "NumberStyle.Float doesn't allow trailing negative sign.");

                        yield return new TestCaseData(typeof(FormatException), "(0x1.8p1)", NumberStyle.HexFloat, NumberFormatInfo.InvariantInfo, "NumberStyle.HexFloat doesn't allow parentheses.");
                        yield return new TestCaseData(typeof(FormatException), "(0x1.8)", NumberStyle.HexFloat, NumberFormatInfo.InvariantInfo, "NumberStyle.HexFloat doesn't allow parentheses.");
                        yield return new TestCaseData(typeof(FormatException), "(0x1.)", NumberStyle.HexFloat, NumberFormatInfo.InvariantInfo, "NumberStyle.HexFloat doesn't allow parentheses.");

                        yield return new TestCaseData(typeof(ArgumentException), "0x1.8p1-", NumberStyle.HexFloat | NumberStyle.AllowTrailingSign, NumberFormatInfo.InvariantInfo, "NumberStyle.Float doesn't allow trailing negative sign.");
                        yield return new TestCaseData(typeof(ArgumentException), "0x1.8-", NumberStyle.HexFloat | NumberStyle.AllowTrailingSign, NumberFormatInfo.InvariantInfo, "NumberStyle.Float doesn't allow trailing negative sign.");
                        yield return new TestCaseData(typeof(ArgumentException), "0x1.-", NumberStyle.HexFloat | NumberStyle.AllowTrailingSign, NumberFormatInfo.InvariantInfo, "NumberStyle.Float doesn't allow trailing negative sign.");
                        yield return new TestCaseData(typeof(ArgumentException), "0x1.8p1 -", NumberStyle.HexFloat | NumberStyle.AllowTrailingSign, NumberFormatInfo.InvariantInfo, "NumberStyle.Float doesn't allow trailing negative sign.");
                        yield return new TestCaseData(typeof(ArgumentException), "0x1.8 -", NumberStyle.HexFloat | NumberStyle.AllowTrailingSign, NumberFormatInfo.InvariantInfo, "NumberStyle.Float doesn't allow trailing negative sign.");
                        yield return new TestCaseData(typeof(ArgumentException), "0x1. -", NumberStyle.HexFloat | NumberStyle.AllowTrailingSign, NumberFormatInfo.InvariantInfo, "NumberStyle.Float doesn't allow trailing negative sign.");

                        yield return new TestCaseData(typeof(ArgumentException), "(0x1.8p1)", NumberStyle.HexFloat | NumberStyle.AllowParentheses, NumberFormatInfo.InvariantInfo, "NumberStyle.HexFloat doesn't allow parentheses.");
                        yield return new TestCaseData(typeof(ArgumentException), "(0x1.8)", NumberStyle.HexFloat | NumberStyle.AllowParentheses, NumberFormatInfo.InvariantInfo, "NumberStyle.HexFloat doesn't allow parentheses.");
                        yield return new TestCaseData(typeof(ArgumentException), "(0x1.)", NumberStyle.HexFloat | NumberStyle.AllowParentheses, NumberFormatInfo.InvariantInfo, "NumberStyle.HexFloat doesn't allow parentheses.");
                    }
                }

                public static IEnumerable<TestCaseData> TestParse_CharSequence_NumberStyle_IFormatProvider_ForRawBits_Data
                {
                    get
                    {
                        // Harmony (Test_parseFloat_LString_Harmony6261())

                        yield return new TestCaseData(0x51e5f4c9, "1.2345679E11", "123456790528.000000000000000f", NumberStyle.Float | NumberStyle.AllowTypeSpecifier, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(0x51e5f4c9, "1.2345679E11", "123456790528.000000000000000", NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(0x50000000, "8.5899346E9", "8589934592", NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(0x50004000, "8.606712E9", "8606711808", NumberStyle.Float, NumberFormatInfo.InvariantInfo);

                        // Harmony (Test_parseFloatLjava_lang_String())

                        yield return new TestCaseData(0x800000, "1.17549435E-38",
                            "0.000000000000000000000000000000000000011754942807573642917278829910357665133228589927589904276829631184250030649651730385585324256680905818939208984375",
                            NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(0x7fffff, "1.1754942E-38",
                            "0.00000000000000000000000000000000000001175494280757364291727882991035766513322858992758990427682963118425003064965173038558532425668090581893920898437499999f",
                            NumberStyle.Float | NumberStyle.AllowTypeSpecifier, NumberFormatInfo.InvariantInfo);

                        /* Test a set of regular floats with exponents from -38 to +38 */
                        for (int i = 38; i > 3; i--)
                        {
                            string testString = "3.4028234663852886e-" + i;
                            yield return new TestCaseData(rawBitsFor3_4eN38To38[38 - i],
                                expectedStringFor3_4eN38To38[38 - i], testString, NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        }

                        yield return new TestCaseData(rawBitsFor3_4eN38To38[38 - 3],
                            expectedStringFor3_4eN38To38[38 - 3], "3.4028234663852886e-3", NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(rawBitsFor3_4eN38To38[38 - 2],
                            expectedStringFor3_4eN38To38[38 - 2], "3.4028234663852886e-2", NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(rawBitsFor3_4eN38To38[38 - 1],
                            expectedStringFor3_4eN38To38[38 - 1], "3.4028234663852886e-1", NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(rawBitsFor3_4eN38To38[38 - 0],
                            expectedStringFor3_4eN38To38[38 - 0], "3.4028234663852886e-0", NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(rawBitsFor3_4eN38To38[38 + 1],
                            expectedStringFor3_4eN38To38[38 + 1], "3.4028234663852886e+1", NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(rawBitsFor3_4eN38To38[38 + 2],
                            expectedStringFor3_4eN38To38[38 + 2], "3.4028234663852886e+2", NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(rawBitsFor3_4eN38To38[38 + 3],
                            expectedStringFor3_4eN38To38[38 + 3], "3.4028234663852886e+3", NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(rawBitsFor3_4eN38To38[38 + 4],
                            expectedStringFor3_4eN38To38[38 + 4], "3.4028234663852886e+4", NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(rawBitsFor3_4eN38To38[38 + 5],
                            expectedStringFor3_4eN38To38[38 + 5], "3.4028234663852886e+5", NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(rawBitsFor3_4eN38To38[38 + 6],
                            expectedStringFor3_4eN38To38[38 + 6], "3.4028234663852886e+6", NumberStyle.Float, NumberFormatInfo.InvariantInfo);

                        for (int i = 7; i < 39; i++)
                        {
                            string testString = "3.4028234663852886e+" + i;
                            yield return new TestCaseData(rawBitsFor3_4eN38To38[38 + i],
                                expectedStringFor3_4eN38To38[38 + i], testString, NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        }

                        /* Test another set of regular floats with exponents from -38 to +38 */
                        for (int i = 38; i > 3; i--)
                        {
                            string testString = "-1.1754943508222875e-" + i;
                            yield return new TestCaseData(rawBitsFor1_17eN38To38[38 - i],
                                expectedStringFor1_17eN38To38[38 - i], testString, NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        }

                        yield return new TestCaseData(rawBitsFor1_17eN38To38[38 - 3],
                            expectedStringFor1_17eN38To38[38 - 3], "-1.1754943508222875e-3", NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(rawBitsFor1_17eN38To38[38 - 2],
                            expectedStringFor1_17eN38To38[38 - 2], "-1.1754943508222875e-2", NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(rawBitsFor1_17eN38To38[38 - 1],
                            expectedStringFor1_17eN38To38[38 - 1], "-1.1754943508222875e-1", NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(rawBitsFor1_17eN38To38[38 - 0],
                            expectedStringFor1_17eN38To38[38 - 0], "-1.1754943508222875e-0", NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(rawBitsFor1_17eN38To38[38 + 1],
                            expectedStringFor1_17eN38To38[38 + 1], "-1.1754943508222875e+1", NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(rawBitsFor1_17eN38To38[38 + 2],
                            expectedStringFor1_17eN38To38[38 + 2], "-1.1754943508222875e+2", NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(rawBitsFor1_17eN38To38[38 + 3],
                            expectedStringFor1_17eN38To38[38 + 3], "-1.1754943508222875e+3", NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(rawBitsFor1_17eN38To38[38 + 4],
                            expectedStringFor1_17eN38To38[38 + 4], "-1.1754943508222875e+4", NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(rawBitsFor1_17eN38To38[38 + 5],
                            expectedStringFor1_17eN38To38[38 + 5], "-1.1754943508222875e+5", NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(rawBitsFor1_17eN38To38[38 + 6],
                            expectedStringFor1_17eN38To38[38 + 6], "-1.1754943508222875e+6", NumberStyle.Float, NumberFormatInfo.InvariantInfo);

                        for (int i = 7; i < 39; i++)
                        {
                            string testString = "-1.1754943508222875e+" + i;
                            yield return new TestCaseData(rawBitsFor1_17eN38To38[38 + i],
                                expectedStringFor1_17eN38To38[38 + i], testString, NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        }

                        /* Test denormalized floats (floats with exponents <= -38 */
                        yield return new TestCaseData(1, "1.4E-45", "1.1012984643248170E-45", NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(unchecked((int)0x80000001), "-1.4E-45", "-1.1012984643248170E-45", NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(1, "1.4E-45", "1.0E-45", NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(unchecked((int)0x80000001), "-1.4E-45", "-1.0E-45", NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(1, "1.4E-45", "0.9E-45", NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(unchecked((int)0x80000001), "-1.4E-45", "-0.9E-45", NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(3, "4.2E-45", "4.203895392974451e-45", NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(unchecked((int)0x80000003), "-4.2E-45", "-4.203895392974451e-45", NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(0, "0.0", "0.004E-45", NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(unchecked((int)0x80000000), "-0.0", "-0.004E-45", NumberStyle.Float, NumberFormatInfo.InvariantInfo);

                        /*
                         * Test for large floats close to and greater than 3.4028235E38 and
                         * -3.4028235E38
                         */
                        yield return new TestCaseData(0x7eb48e52, "1.2E38", "1.2E+38", NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(unchecked((int)0xfeb48e52), "-1.2E38", "-1.2E+38", NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(0x7f70bdc2, "3.2E38", "3.2E+38", NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(unchecked((int)0xff70bdc2), "-3.2E38", "-3.2E+38", NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(0x7f7fc99e, "3.4E38", "3.4E+38", NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(unchecked((int)0xff7fc99e), "-3.4E38", "-3.4E+38", NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(0x7f7fffff, "3.4028235E38", "3.4028234663852886E+38", NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(unchecked((int)0xff7fffff), "-3.4028235E38", "-3.4028234663852886E+38", NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(0x7f800000, "Infinity", "3.405E+38", NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(unchecked((int)0xff800000), "-Infinity", "-3.405E+38", NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(0x7f800000, "Infinity", "3.41E+38", NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(unchecked((int)0xff800000), "-Infinity", "-3.41E+38", NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(0x7f800000, "Infinity", "3.42E+38", NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(unchecked((int)0xff800000), "-Infinity", "-3.42E+38", NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(0x7f800000, "Infinity", "1.0E+39", NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(unchecked((int)0xff800000), "-Infinity", "-1.0E+39", NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                    }
                }
            }

            #region Parse_CharSequence_NumberStyle_IFormatProvider

            public abstract class Parse_CharSequence_NumberStyle_IFormatProvider : ParseTestCase
            {
                protected virtual bool IsNullableType => true;

                protected abstract float GetResult(string value, NumberStyle style, IFormatProvider provider);

                [TestCaseSource("TestParse_CharSequence_NumberStyle_IFormatProvider_ForFloatStyle_Data")]
                public void TestParse_CharSequence_NumberStyle_IFormatProvider_ForFloatStyle(float expected, string value, NumberStyle style, IFormatProvider provider)
                {
                    Assume.That(IsNullableType || (!IsNullableType && value != null), "null is not supported by this character sequence type.");

                    float actual = GetResult(value, style, provider);

                    string expectedString = "0x" + BitConversion.SingleToInt32Bits(expected).ToHexString();
                    string actualString = "0x" + BitConversion.SingleToInt32Bits(actual).ToHexString();
                    string errorMsg = $"input string is:<{value}>. "
                        + $"The expected result should be:<{expectedString}>, "
                        + $"but was: <{actualString}>. ";

                    assertEquals(errorMsg, expected, actual, 0.0F);
                }

                [TestCaseSource("TestParse_CharSequence_NumberStyle_IFormatProvider_ForHexFloatStyle_Data")]
                public void TestParse_CharSequence_NumberStyle_IFormatProvider_ForHexFloatStyle(float expected, string value, NumberStyle style, IFormatProvider provider)
                {
                    Assume.That(IsNullableType || (!IsNullableType && value != null), "null is not supported by this character sequence type.");

                    float actual = GetResult(value, style, provider);

                    string expectedString = "0x" + BitConversion.SingleToInt32Bits(expected).ToHexString();
                    string actualString = "0x" + BitConversion.SingleToInt32Bits(actual).ToHexString();
                    string errorMsg = $"input string is:<{value}>. "
                        + $"The expected result should be:<{expectedString}>, "
                        + $"but was: <{actualString}>. ";

                    assertEquals(errorMsg, expected, actual, 0.0F);
                }

                [TestCaseSource("TestParse_CharSequence_NumberStyle_IFormatProvider_ForFloatStyleException_Data")]
                public void TestParse_CharSequence_NumberStyle_IFormatProvider_ForFloatStyleException(Type expectedExceptionType, string value, NumberStyle style, IFormatProvider provider, string message)
                {
                    Assume.That(IsNullableType || (!IsNullableType && value != null), "null is not supported by this character sequence type.");

                    Assert.Throws(expectedExceptionType, () => GetResult(value, style, provider), message);
                }

                [TestCaseSource("TestParse_CharSequence_NumberStyle_IFormatProvider_ForHexFloatStyleException_Data")]
                public void TestParse_CharSequence_NumberStyle_IFormatProvider_ForHexFloatStyleException(Type expectedExceptionType, string value, NumberStyle style, IFormatProvider provider, string message)
                {
                    Assume.That(IsNullableType || (!IsNullableType && value != null), "null is not supported by this character sequence type.");

                    Assert.Throws(expectedExceptionType, () => GetResult(value, style, provider), message);
                }

                [TestCaseSource("TestParse_CharSequence_NumberStyle_IFormatProvider_ForRawBits_Data")]
                public void TestParse_CharSequence_NumberStyle_IFormatProvider_ForRawBits(int expectedRawBits, string expectedString, string value, NumberStyle style, IFormatProvider provider)
                {
                    Assume.That(IsNullableType || (!IsNullableType && value != null), "null is not supported by this character sequence type.");

                    int rawBits;
                    float result = GetResult(value, style, provider);
                    rawBits = BitConversion.SingleToInt32Bits(result);
                    assertEquals($"Original float({value.ToString(NumberFormatInfo.InvariantInfo)}) Converted float({result.ToString(NumberFormatInfo.InvariantInfo)}) "
                            + $" Expecting:{expectedRawBits.ToHexString()} Got: {rawBits.ToHexString()}", expectedRawBits, rawBits);
                }
            }

            public class Parse_String_NumberStyle_IFormatProvider : Parse_CharSequence_NumberStyle_IFormatProvider
            {
                protected override float GetResult(string value, NumberStyle style, IFormatProvider provider)
                {
                    return Single.Parse(value, style, provider);
                }
            }

#if FEATURE_READONLYSPAN
            public class Parse_ReadOnlySpan_NumberStyle_IFormatProvider : Parse_CharSequence_NumberStyle_IFormatProvider
            {
                protected override bool IsNullableType => false;

                protected override float GetResult(string value, NumberStyle style, IFormatProvider provider)
                {
                    return Single.Parse(value.AsSpan(), style, provider);
                }

                [Test]
                public void TestUnicodeSymbols()
                {
                    assertEquals(float.PositiveInfinity, GetResult("INFINITYe\u0661234", NumberStyle.Float, new NumberFormatInfo { PositiveInfinitySymbol = "Infinitye\u0661234" }));
                    assertEquals(float.NegativeInfinity, GetResult("NEGINFINITYe\u0661234", NumberStyle.Float, new NumberFormatInfo { NegativeInfinitySymbol = "NegInfinitye\u0661234" }));
                    assertEquals(float.NaN, GetResult("NANe\u0661234", NumberStyle.Float, new NumberFormatInfo { NaNSymbol = "NaNe\u0661234" }));
                }
            }
#endif
            #endregion Parse_CharSequence_NumberStyle_IFormatProvider

            #region Parse_CharSequence_IFormatProvider

            public abstract class Parse_CharSequence_IFormatProvider : ParseTestCase
            {
                protected virtual bool IsNullableType => true;

                protected abstract float GetResult(string value, IFormatProvider provider);

                [TestCaseSource("TestParse_CharSequence_NumberStyle_IFormatProvider_ForFloatStyle_Data")]
                public void TestParse_CharSequence_IFormatProvider_ForFloatStyle(float expected, string value, NumberStyle style, IFormatProvider provider)
                {
                    Assume.That(IsNullableType || (!IsNullableType && value != null), "null is not supported by this character sequence type.");
                    Assume.That((style & ~(NumberStyle.Float | NumberStyle.AllowThousands)) == 0, "Custom NumberStyles are not supported on this overload.");

                    float actual = GetResult(value, provider);

                    string expectedString = "0x" + BitConversion.SingleToInt32Bits(expected).ToHexString();
                    string actualString = "0x" + BitConversion.SingleToInt32Bits(actual).ToHexString();
                    string errorMsg = $"input string is:<{value}>. "
                        + $"The expected result should be:<{expectedString}>, "
                        + $"but was: <{actualString}>. ";

                    assertEquals(errorMsg, expected, actual, 0.0F);
                }

                [TestCaseSource("TestParse_CharSequence_NumberStyle_IFormatProvider_ForFloatStyleException_Data")]
                public void TestParse_CharSequence_NumberStyle_IFormatProvider_ForFloatStyleException(Type expectedExceptionType, string value, NumberStyle style, IFormatProvider provider, string message)
                {
                    Assume.That(IsNullableType || (!IsNullableType && value != null), "null is not supported by this character sequence type.");
                    Assume.That((style & ~(NumberStyle.Float | NumberStyle.AllowThousands)) == 0, "Custom NumberStyles are not supported on this overload.");

                    Assert.Throws(expectedExceptionType, () => GetResult(value, provider), message);
                }

                [TestCaseSource("TestParse_CharSequence_NumberStyle_IFormatProvider_ForRawBits_Data")]
                public void TestParse_CharSequence_NumberStyle_IFormatProvider_ForRawBits(int expectedRawBits, string expectedString, string value, NumberStyle style, IFormatProvider provider)
                {
                    Assume.That(IsNullableType || (!IsNullableType && value != null), "null is not supported by this character sequence type.");
                    Assume.That((style & ~(NumberStyle.Float | NumberStyle.AllowThousands)) == 0, "Custom NumberStyles are not supported on this overload.");

                    int rawBits;
                    float result = GetResult(value, provider);
                    rawBits = BitConversion.SingleToInt32Bits(result);
                    assertEquals($"Original float({value.ToString(NumberFormatInfo.InvariantInfo)}) Converted float({result.ToString(NumberFormatInfo.InvariantInfo)}) "
                            + $" Expecting:{expectedRawBits.ToHexString()} Got: {rawBits.ToHexString()}", expectedRawBits, rawBits);
                }
            }

            public class Parse_String_IFormatProvider : Parse_CharSequence_IFormatProvider
            {
                protected override float GetResult(string value, IFormatProvider provider)
                {
                    return Single.Parse(value, provider);
                }
            }

            #endregion Parse_CharSequence_IFormatProvider

            #region TryParse_CharSequence_NumberStyle_IFormatProvider_Single

            public abstract class TryParse_CharSequence_NumberStyle_IFormatProvider_Single_TestCase : ParseTestCase
            {
                protected virtual bool IsNullableType => true;

                protected abstract bool GetResult(string value, NumberStyle style, IFormatProvider provider, out float result);

                [TestCaseSource("TestParse_CharSequence_NumberStyle_IFormatProvider_ForFloatStyle_Data")]
                public void TestTryParse_CharSequence_NumberStyle_IFormatProvider_Single_ForFloatStyle(float expected, string value, NumberStyle style, IFormatProvider provider)
                {
                    Assume.That(IsNullableType || (!IsNullableType && value != null), "null is not supported by this character sequence type.");

                    assertTrue(GetResult(value, style, provider, out float actual));

                    string expectedString = "0x" + BitConversion.SingleToInt32Bits(expected).ToHexString();
                    string actualString = "0x" + BitConversion.SingleToInt32Bits(actual).ToHexString();
                    string errorMsg = $"input string is:<{value}>. "
                        + $"The expected result should be:<{expectedString}>, "
                        + $"but was: <{actualString}>. ";

                    assertEquals(errorMsg, expected, actual, 0.0D);
                }

                [TestCaseSource("TestParse_CharSequence_NumberStyle_IFormatProvider_ForHexFloatStyle_Data")]
                public void TestTryParse_CharSequence_NumberStyle_IFormatProvider_Single_ForHexFloatStyle(float expected, string value, NumberStyle style, IFormatProvider provider)
                {
                    Assume.That(IsNullableType || (!IsNullableType && value != null), "null is not supported by this character sequence type.");

                    assertTrue(GetResult(value, style, provider, out float actual));

                    string expectedString = "0x" + BitConversion.SingleToInt32Bits(expected).ToHexString();
                    string actualString = "0x" + BitConversion.SingleToInt32Bits(actual).ToHexString();
                    string errorMsg = $"input string is:<{value}>. "
                        + $"The expected result should be:<{expectedString}>, "
                        + $"but was: <{actualString}>. ";

                    assertEquals(errorMsg, expected, actual, 0.0D);
                }

                [TestCaseSource("TestParse_CharSequence_NumberStyle_IFormatProvider_ForFloatStyleException_Data")]
                public void TestTryParse_CharSequence_NumberStyle_IFormatProvider_Single_ForFloatStyleException(Type expectedExceptionType, string value, NumberStyle style, IFormatProvider provider, string message)
                {
                    Assume.That(IsNullableType || (!IsNullableType && value != null), "null is not supported by this character sequence type.");

                    assertFalse(message, GetResult(value, style, provider, out float actual));
                    assertEquals(0, actual);
                }

                [TestCaseSource("TestParse_CharSequence_NumberStyle_IFormatProvider_ForHexFloatStyleException_Data")]
                public void TestTryParse_CharSequence_NumberStyle_IFormatProvider_Single_ForHexFloatStyleException(Type expectedExceptionType, string value, NumberStyle style, IFormatProvider provider, string message)
                {
                    Assume.That(IsNullableType || (!IsNullableType && value != null), "null is not supported by this character sequence type.");

                    float actual = 0;
                    if (expectedExceptionType == typeof(FormatException) || expectedExceptionType.Equals(typeof(OverflowException)))
                    {
                        assertFalse(message, GetResult(value, style, provider, out actual));
                    }
                    else // Actual exception should be thrown
                    {
                        Assert.Throws(expectedExceptionType, () => GetResult(value, style, provider, out actual), message);
                    }
                    assertEquals(0, actual);
                }

                [TestCaseSource("TestParse_CharSequence_NumberStyle_IFormatProvider_ForRawBits_Data")]
                public void TestTryParse_CharSequence_NumberStyle_IFormatProvider_Single_ForRawBits(int expectedRawBits, string expectedString, string value, NumberStyle style, IFormatProvider provider)
                {
                    Assume.That(IsNullableType || (!IsNullableType && value != null), "null is not supported by this character sequence type.");

                    int rawBits;
                    assertTrue(GetResult(value, style, provider, out float result));
                    rawBits = BitConversion.SingleToInt32Bits(result);
                    assertEquals($"Original float({value.ToString(NumberFormatInfo.InvariantInfo)}) Converted float({result.ToString(NumberFormatInfo.InvariantInfo)}) "
                            + $" Expecting:{expectedRawBits.ToHexString()} Got: {rawBits.ToHexString()}", expectedRawBits, rawBits);
                }
            }

            public class TryParse_String_NumberStyle_IFormatProvider_Single : TryParse_CharSequence_NumberStyle_IFormatProvider_Single_TestCase
            {
                protected override bool GetResult(string value, NumberStyle style, IFormatProvider provider, out float result)
                {
                    return Single.TryParse(value, style, provider, out result);
                }
            }

#if FEATURE_READONLYSPAN
            public class TryParse_ReadOnlySpan_NumberStyle_IFormatProvider_Single : TryParse_CharSequence_NumberStyle_IFormatProvider_Single_TestCase
            {
                protected override bool IsNullableType => false;

                protected override bool GetResult(string value, NumberStyle style, IFormatProvider provider, out float result)
                {
                    return Single.TryParse(value.AsSpan(), style, provider, out result);
                }
            }
#endif

            #endregion TryParse_CharSequence_NumberStyle_IFormatProvider_Single

            #region TryParse_CharSequence_Single

            public abstract class TryParse_CharSequence_Single_TestCase : ParseTestCase
            {
                protected virtual bool IsNullableType => true;

                protected abstract bool GetResult(string value, out float result);

                [TestCaseSource("TestParse_CharSequence_NumberStyle_IFormatProvider_ForFloatStyle_Data")]
                public void TestTryParse_CharSequence_Single_ForFloatStyle(float expected, string value, NumberStyle style, IFormatProvider provider)
                {
                    Assume.That(IsNullableType || (!IsNullableType && value != null), "null is not supported by this character sequence type.");
                    Assume.That((style & ~(NumberStyle.Float | NumberStyle.AllowThousands)) == 0, "Custom NumberStyles are not supported on this overload.");
                    // NOTE: Assumption made here that this contains no culture sensitivity tests.
                    Assume.That(NumberFormatInfo.GetInstance(provider).Equals(NumberFormatInfo.InvariantInfo), "Non-invariant tests do not apply to this overload.");

                    using (var context = new CultureContext(CultureInfo.InvariantCulture))
                    {
                        assertTrue(GetResult(value, out float actual));

                        string expectedString = "0x" + BitConversion.SingleToInt32Bits(expected).ToHexString();
                        string actualString = "0x" + BitConversion.SingleToInt32Bits(actual).ToHexString();
                        string errorMsg = $"input string is:<{value}>. "
                            + $"The expected result should be:<{expectedString}>, "
                            + $"but was: <{actualString}>. ";

                        assertEquals(errorMsg, expected, actual, 0.0D);
                    }
                }

                [TestCaseSource("TestParse_CharSequence_NumberStyle_IFormatProvider_ForFloatStyleException_Data")]
                public void TestTryParse_CharSequence_Single_ForFloatStyleException(Type expectedExceptionType, string value, NumberStyle style, IFormatProvider provider, string message)
                {
                    Assume.That(IsNullableType || (!IsNullableType && value != null), "null is not supported by this character sequence type.");
                    Assume.That((style & ~(NumberStyle.Float | NumberStyle.AllowThousands)) == 0, "Custom NumberStyles are not supported on this overload.");
                    // NOTE: Assumption made here that this contains no culture sensitivity tests.
                    Assume.That(NumberFormatInfo.GetInstance(provider).Equals(NumberFormatInfo.InvariantInfo), "Non-invariant tests do not apply to this overload.");

                    using (var context = new CultureContext(CultureInfo.InvariantCulture))
                    {

                        assertFalse(message, GetResult(value, out float actual));
                        assertEquals(0, actual);
                    }
                }

                [TestCaseSource("TestParse_CharSequence_NumberStyle_IFormatProvider_ForRawBits_Data")]
                public void TestTryParse_CharSequence_Single_ForRawBits(int expectedRawBits, string expectedString, string value, NumberStyle style, IFormatProvider provider)
                {
                    Assume.That(IsNullableType || (!IsNullableType && value != null), "null is not supported by this character sequence type.");
                    Assume.That((style & ~(NumberStyle.Float | NumberStyle.AllowThousands)) == 0, "Custom NumberStyles are not supported on this overload.");
                    // NOTE: Assumption made here that this contains no culture sensitivity tests.
                    Assume.That(NumberFormatInfo.GetInstance(provider).Equals(NumberFormatInfo.InvariantInfo), "Non-invariant tests do not apply to this overload.");

                    using (var context = new CultureContext(CultureInfo.InvariantCulture))
                    {

                        int rawBits;
                        assertTrue(GetResult(value, out float result));
                        rawBits = BitConversion.SingleToInt32Bits(result);
                        assertEquals($"Original float({value.ToString(NumberFormatInfo.InvariantInfo)}) Converted float({result.ToString(NumberFormatInfo.InvariantInfo)}) "
                                + $" Expecting:{expectedRawBits.ToHexString()} Got: {rawBits.ToHexString()}", expectedRawBits, rawBits);
                    }
                }
            }

            public class TryParse_String_Single : TryParse_CharSequence_Single_TestCase
            {
                protected override bool GetResult(string value, out float result)
                {
                    return Single.TryParse(value, out result);
                }
            }

#if FEATURE_READONLYSPAN
            public class TryParse_ReadOnlySpan_Single : TryParse_CharSequence_Single_TestCase
            {
                protected override bool IsNullableType => false;

                protected override bool GetResult(string value, out float result)
                {
                    return Single.TryParse(value.AsSpan(), out result);
                }
            }
#endif

            #endregion TryParse_CharSequence_Single
        }
    }
}
