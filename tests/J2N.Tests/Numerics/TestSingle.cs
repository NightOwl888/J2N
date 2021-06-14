﻿using NUnit.Framework;
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

        private void doTestCompareRawBits(string originalFloatString, int expectedRawBits,
            string expectedString)
        {
            int rawBits;
            float result = Single.ParseSingle(originalFloatString, J2N.Text.StringFormatter.InvariantCulture);
            rawBits = Single.SingleToInt32Bits(result);
            assertEquals("Original float(" + originalFloatString + ") Converted float(" + result
                    + ") Expecting:" + Int32.ToHexString(expectedRawBits) + " Got: "
                    + Int32.ToHexString(rawBits), expectedRawBits, rawBits);
        }

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
            // Test for method java.lang.Float(java.lang.String)

            Single f = new Single("900.89");
            assertTrue("Created incorrect Float", f.GetSingleValue() == 900.89f);
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

        /**
         * @tests java.lang.Float#parseFloat(java.lang.String)
         */
        [Test]
        public void Test_parseFloatLjava_lang_String()
        {
            assertEquals("Incorrect float returned, expected zero.", 0.0, Single
                    .ParseSingle("7.0064923216240853546186479164495e-46", J2N.Text.StringFormatter.InvariantCulture), 0.0);
            assertEquals("Incorrect float returned, expected minimum float.", float.Epsilon, // J2N: In .NET float.Epsilon is the same as Float.MIN_VALUE in Java
                    Single.ParseSingle("7.0064923216240853546186479164496e-46", J2N.Text.StringFormatter.InvariantCulture), 0.0);

            doTestCompareRawBits(
                    "0.000000000000000000000000000000000000011754942807573642917278829910357665133228589927589904276829631184250030649651730385585324256680905818939208984375",
                    0x800000, "1.17549435E-38");
            doTestCompareRawBits(
                    "0.00000000000000000000000000000000000001175494280757364291727882991035766513322858992758990427682963118425003064965173038558532425668090581893920898437499999f",
                    0x7fffff, "1.1754942E-38");

            /* Test a set of regular floats with exponents from -38 to +38 */
            for (int i = 38; i > 3; i--)
            {
                String testString;
                testString = "3.4028234663852886e-" + i;
                doTestCompareRawBits(testString, rawBitsFor3_4eN38To38[38 - i],
                        expectedStringFor3_4eN38To38[38 - i]);
            }
            doTestCompareRawBits("3.4028234663852886e-3", rawBitsFor3_4eN38To38[38 - 3],
                    expectedStringFor3_4eN38To38[38 - 3]);
            doTestCompareRawBits("3.4028234663852886e-2", rawBitsFor3_4eN38To38[38 - 2],
                    expectedStringFor3_4eN38To38[38 - 2]);
            doTestCompareRawBits("3.4028234663852886e-1", rawBitsFor3_4eN38To38[38 - 1],
                    expectedStringFor3_4eN38To38[38 - 1]);
            doTestCompareRawBits("3.4028234663852886e-0", rawBitsFor3_4eN38To38[38 - 0],
                    expectedStringFor3_4eN38To38[38 - 0]);
            doTestCompareRawBits("3.4028234663852886e+1", rawBitsFor3_4eN38To38[38 + 1],
                    expectedStringFor3_4eN38To38[38 + 1]);
            doTestCompareRawBits("3.4028234663852886e+2", rawBitsFor3_4eN38To38[38 + 2],
                    expectedStringFor3_4eN38To38[38 + 2]);
            doTestCompareRawBits("3.4028234663852886e+3", rawBitsFor3_4eN38To38[38 + 3],
                    expectedStringFor3_4eN38To38[38 + 3]);
            doTestCompareRawBits("3.4028234663852886e+4", rawBitsFor3_4eN38To38[38 + 4],
                    expectedStringFor3_4eN38To38[38 + 4]);
            doTestCompareRawBits("3.4028234663852886e+5", rawBitsFor3_4eN38To38[38 + 5],
                    expectedStringFor3_4eN38To38[38 + 5]);
            doTestCompareRawBits("3.4028234663852886e+6", rawBitsFor3_4eN38To38[38 + 6],
                    expectedStringFor3_4eN38To38[38 + 6]);

            for (int i = 7; i < 39; i++)
            {
                String testString;
                testString = "3.4028234663852886e+" + i;
                doTestCompareRawBits(testString, rawBitsFor3_4eN38To38[38 + i],
                        expectedStringFor3_4eN38To38[38 + i]);
            }

            /* Test another set of regular floats with exponents from -38 to +38 */
            for (int i = 38; i > 3; i--)
            {
                String testString;
                testString = "-1.1754943508222875e-" + i;
                doTestCompareRawBits(testString, rawBitsFor1_17eN38To38[38 - i],
                        expectedStringFor1_17eN38To38[38 - i]);
            }
            doTestCompareRawBits("-1.1754943508222875e-3", rawBitsFor1_17eN38To38[38 - 3],
                    expectedStringFor1_17eN38To38[38 - 3]);
            doTestCompareRawBits("-1.1754943508222875e-2", rawBitsFor1_17eN38To38[38 - 2],
                    expectedStringFor1_17eN38To38[38 - 2]);
            doTestCompareRawBits("-1.1754943508222875e-1", rawBitsFor1_17eN38To38[38 - 1],
                    expectedStringFor1_17eN38To38[38 - 1]);
            doTestCompareRawBits("-1.1754943508222875e-0", rawBitsFor1_17eN38To38[38 - 0],
                    expectedStringFor1_17eN38To38[38 - 0]);
            doTestCompareRawBits("-1.1754943508222875e+1", rawBitsFor1_17eN38To38[38 + 1],
                    expectedStringFor1_17eN38To38[38 + 1]);
            doTestCompareRawBits("-1.1754943508222875e+2", rawBitsFor1_17eN38To38[38 + 2],
                    expectedStringFor1_17eN38To38[38 + 2]);
            doTestCompareRawBits("-1.1754943508222875e+3", rawBitsFor1_17eN38To38[38 + 3],
                    expectedStringFor1_17eN38To38[38 + 3]);
            doTestCompareRawBits("-1.1754943508222875e+4", rawBitsFor1_17eN38To38[38 + 4],
                    expectedStringFor1_17eN38To38[38 + 4]);
            doTestCompareRawBits("-1.1754943508222875e+5", rawBitsFor1_17eN38To38[38 + 5],
                    expectedStringFor1_17eN38To38[38 + 5]);
            doTestCompareRawBits("-1.1754943508222875e+6", rawBitsFor1_17eN38To38[38 + 6],
                    expectedStringFor1_17eN38To38[38 + 6]);

            for (int i = 7; i < 39; i++)
            {
                String testString;
                testString = "-1.1754943508222875e+" + i;
                doTestCompareRawBits(testString, rawBitsFor1_17eN38To38[38 + i],
                        expectedStringFor1_17eN38To38[38 + i]);
            }

            /* Test denormalized floats (floats with exponents <= -38 */
            doTestCompareRawBits("1.1012984643248170E-45", 1, "1.4E-45");
            doTestCompareRawBits("-1.1012984643248170E-45", unchecked((int)0x80000001), "-1.4E-45");
            doTestCompareRawBits("1.0E-45", 1, "1.4E-45");
            doTestCompareRawBits("-1.0E-45", unchecked((int)0x80000001), "-1.4E-45");
            doTestCompareRawBits("0.9E-45", 1, "1.4E-45");
            doTestCompareRawBits("-0.9E-45", unchecked((int)0x80000001), "-1.4E-45");
            doTestCompareRawBits("4.203895392974451e-45", 3, "4.2E-45");
            doTestCompareRawBits("-4.203895392974451e-45", unchecked((int)0x80000003), "-4.2E-45");
            doTestCompareRawBits("0.004E-45", 0, "0.0");
            doTestCompareRawBits("-0.004E-45", unchecked((int)0x80000000), "-0.0");

            /*
             * Test for large floats close to and greater than 3.4028235E38 and
             * -3.4028235E38
             */
            doTestCompareRawBits("1.2E+38", 0x7eb48e52, "1.2E38");
            doTestCompareRawBits("-1.2E+38", unchecked((int)0xfeb48e52), "-1.2E38");
            doTestCompareRawBits("3.2E+38", 0x7f70bdc2, "3.2E38");
            doTestCompareRawBits("-3.2E+38", unchecked((int)0xff70bdc2), "-3.2E38");
            doTestCompareRawBits("3.4E+38", 0x7f7fc99e, "3.4E38");
            doTestCompareRawBits("-3.4E+38", unchecked((int)0xff7fc99e), "-3.4E38");
            doTestCompareRawBits("3.4028234663852886E+38", 0x7f7fffff, "3.4028235E38");
            doTestCompareRawBits("-3.4028234663852886E+38", unchecked((int)0xff7fffff), "-3.4028235E38");
            doTestCompareRawBits("3.405E+38", 0x7f800000, "Infinity");
            doTestCompareRawBits("-3.405E+38", unchecked((int)0xff800000), "-Infinity");
            doTestCompareRawBits("3.41E+38", 0x7f800000, "Infinity");
            doTestCompareRawBits("-3.41E+38", unchecked((int)0xff800000), "-Infinity");
            doTestCompareRawBits("3.42E+38", 0x7f800000, "Infinity");
            doTestCompareRawBits("-3.42E+38", unchecked((int)0xff800000), "-Infinity");
            doTestCompareRawBits("1.0E+39", 0x7f800000, "Infinity");
            doTestCompareRawBits("-1.0E+39", unchecked((int)0xff800000), "-Infinity");
        }

        /**
         * @tests java.lang.Float#parseFloat(java.lang.String)
         */
        [Test]
        public void Test_parseFloat_LString_Unusual()
        {
            float actual;

            actual = Single.ParseSingle("0x00000000000000000000000000000000000000000.0000000000000000000000000000000000000p0000000000000000000000000000000000");
            assertEquals("Returned incorrect value", 0.0f, actual, 0.0F);

            actual = Single.ParseSingle("+0Xfffff.fffffffffffffffffffffffffffffffp+99F");
            assertEquals("Returned incorrect value", 6.64614E35f, actual, 0.0F);

            actual = Single.ParseSingle("-0X.123456789abcdefp+99f");
            assertEquals("Returned incorrect value", -4.5072022E28f, actual, 0.0F);

            actual = Single.ParseSingle("-0X123456789abcdef.p+1f");
            assertEquals("Returned incorrect value", -1.63971062E17f, actual, 0.0F);

            actual = Single.ParseSingle("-0X000000000000000000000000000001abcdef.0000000000000000000000000001abefp00000000000000000000000000000000000000000004f");
            assertEquals("Returned incorrect value", -4.48585472E8f, actual, 0.0F);

            actual = Single.ParseSingle("0X0.00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000001234p600f");
            assertEquals("Returned incorrect value", 5.907252E33f, actual, 0.0F);

            actual = Single.ParseSingle("0x1.p9223372036854775807");
            assertEquals("Returned incorrect value", float.PositiveInfinity, actual, 0.0F);

            actual = Single.ParseSingle("0x1.p9223372036854775808");
            assertEquals("Returned incorrect value", float.PositiveInfinity, actual, 0.0F);

            actual = Single.ParseSingle("0x10.p9223372036854775808");
            assertEquals("Returned incorrect value", float.PositiveInfinity, actual, 0.0F);

            actual = Single.ParseSingle("0xabcd.ffffffffp+2000");
            assertEquals("Returned incorrect value", float.PositiveInfinity, actual, 0.0F);

            actual = Single.ParseSingle("0x1.p-9223372036854775808");
            assertEquals("Returned incorrect value", 0.0f, actual, 0.0F);

            actual = Single.ParseSingle("0x1.p-9223372036854775809");
            assertEquals("Returned incorrect value", 0.0f, actual, 0.0F);

            actual = Single.ParseSingle("0x.1p-9223372036854775809");
            assertEquals("Returned incorrect value", 0.0f, actual, 0.0F);
        }

        /**
         * @tests java.lang.Float#parseFloat(java.lang.String)
         */
        [Test]
        public void Test_parseFloat_LString_NormalPositiveExponent()
        {
            int[] expecteds = {
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

            for (int i = 0; i < expecteds.Length; i++)
            {
                int part = i * 6;
                String inputString = "0x" + part + "." + part + "0123456789abcdefp" + part;

                float actual = Single.ParseSingle(inputString);
                float expected = Single.Int32BitsToSingle(expecteds[i]);

                String expectedString = Int32.ToHexString(Single.SingleToInt32Bits(expected));
                String actualString = Int32.ToHexString(Single.SingleToInt32Bits(actual));
                String errorMsg = i + "th input string is:<" + inputString
                + ">.The expected result should be:<" + expectedString
                + ">, but was: <" + actualString + ">. ";

                assertEquals(errorMsg, expected, actual, 0.0F);
            }
        }

        /**
         * @tests java.lang.Float#parseFloat(java.lang.String)
         */
        [Test]
        public void Test_parseFloat_LString_NormalNegativeExponent()
        {
            int[] expecteds = {
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

            for (int i = 0; i < expecteds.Length; i++)
            {
                int part = i * 7;
                String inputString = "0x" + part + "." + part + "0123456789abcdefp-" + part;

                float actual = Single.ParseSingle(inputString);
                float expected = Single.Int32BitsToSingle(expecteds[i]);

                String expectedString = Int32.ToHexString(Single.SingleToInt32Bits(expected));
                String actualString = Int32.ToHexString(Single.SingleToInt32Bits(actual));
                String errorMsg = i + "th input string is:<" + inputString
                + ">.The expected result should be:<" + expectedString
                + ">, but was: <" + actualString + ">. ";

                assertEquals(errorMsg, expected, actual, 0.0F);
            }
        }

        /**
         * @tests java.lang.Float#parseFloat(java.lang.String)
         */
        [Test]
        public void Test_parseFloat_LString_MaxNormalBoundary()
        {
            int[] expecteds ={
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

            String[] inputs = {
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

            for (int i = 0; i < inputs.Length; i++)
            {
                float actual = Single.ParseSingle(inputs[i]);
                float expected = Single.Int32BitsToSingle(expecteds[i]);

                String expectedString = Int32.ToHexString(Single.SingleToInt32Bits(expected));
                String actualString = Int32.ToHexString(Single.SingleToInt32Bits(actual));
                String errorMsg = i + "th input string is:<" + inputs[i]
                + ">.The expected result should be:<" + expectedString
                + ">, but was: <" + actualString + ">. ";

                assertEquals(errorMsg, expected, actual, 0.0F);
            }
        }

        /**
         * @tests java.lang.Float#parseFloat(java.lang.String)
         */
        [Test]
        public void Test_parseFloat_LString_MinNormalBoundary()
        {
            int[] expecteds = {
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

            String[] inputs = {
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

            for (int i = 0; i < inputs.Length; i++)
            {
                float actual = Single.ParseSingle(inputs[i]);
                float expected = Single.Int32BitsToSingle(expecteds[i]);

                String expectedString = Int32.ToHexString(Single.SingleToInt32Bits(expected));
                String actualString = Int32.ToHexString(Single.SingleToInt32Bits(actual));
                String errorMsg = i + "th input string is:<" + inputs[i]
                + ">.The expected result should be:<" + expectedString
                + ">, but was: <" + actualString + ">. ";

                assertEquals(errorMsg, expected, actual, 0.0F);
            }
        }

        /**
         * @tests java.lang.Float#parseFloat(java.lang.String)
         */
        [Test]
        public void Test_parseFloat_LString_MaxSubNormalBoundary()
        {
            int[] expecteds = {
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

            String[] inputs = {
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

            for (int i = 0; i < inputs.Length; i++)
            {
                float actual = Single.ParseSingle(inputs[i]);
                float expected = Single.Int32BitsToSingle(expecteds[i]);

                String expectedString = Int32.ToHexString(Single.SingleToInt32Bits(expected));
                String actualString = Int32.ToHexString(Single.SingleToInt32Bits(actual));
                String errorMsg = i + "th input string is:<" + inputs[i]
                + ">.The expected result should be:<" + expectedString
                + ">, but was: <" + actualString + ">. ";

                assertEquals(errorMsg, expected, actual, 0.0F);
            }
        }

        /**
         * @tests java.lang.Float#parseFloat(java.lang.String)
         */
        [Test]
        public void Test_parseFloat_LString_MinSubNormalBoundary()
        {
            int[] expecteds = {
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

            String[] inputs = {
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

            for (int i = 0; i < inputs.Length; i++)
            {
                float actual = Single.ParseSingle(inputs[i]);
                float expected = Single.Int32BitsToSingle(expecteds[i]);

                String expectedString = Int32.ToHexString(Single.SingleToInt32Bits(expected));
                String actualString = Int32.ToHexString(Single.SingleToInt32Bits(actual));
                String errorMsg = i + "th input string is:<" + inputs[i]
                + ">.The expected result should be:<" + expectedString
                + ">, but was: <" + actualString + ">. ";

                assertEquals(errorMsg, expected, actual, 0.0F);
            }
        }

        /**
         * @tests java.lang.Float#parseFloat(java.lang.String)
         */
        [Test]
        public void Test_parseFloat_LString_ZeroBoundary()
        {
            int[] expecteds = {
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

            String[] inputs = {
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

            for (int i = 0; i < inputs.Length; i++)
            {
                float actual = Single.ParseSingle(inputs[i]);
                float expected = Single.Int32BitsToSingle(expecteds[i]);

                String expectedString = Int32.ToHexString(Single.SingleToInt32Bits(expected));
                String actualString = Int32.ToHexString(Single.SingleToInt32Bits(actual));
                String errorMsg = i + "th input string is:<" + inputs[i]
                + ">.The expected result should be:<" + expectedString
                + ">, but was: <" + actualString + ">. ";

                assertEquals(errorMsg, expected, actual, 0.0F);
            }
        }

        /**
         * @tests java.lang.Float#parseFloat(java.lang.String)
         */
        [Test]
        public void Test_parseFloat_LString_Harmony6261()
        {
            // Regression test for HARMONY-6261
            float f = new Single("2147483648", J2N.Text.StringFormatter.InvariantCulture);
            //assertEquals("2.14748365E9", Single.ToString(f, J2N.Text.StringFormatter.InvariantCulture)); // J2N: Changed from "2.1474836E9" to "2.14748365E9" to match JDK behavior
            assertEquals("2.1474836E9", Single.ToString(f, J2N.Text.StringFormatter.InvariantCulture));

            doTestCompareRawBits("123456790528.000000000000000f", 0x51e5f4c9, "1.2345679E11");
            doTestCompareRawBits("123456790528.000000000000000", 0x51e5f4c9, "1.2345679E11");
            doTestCompareRawBits("8589934592", 0x50000000, "8.5899346E9");
            doTestCompareRawBits("8606711808", 0x50004000, "8.606712E9");
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
            Single got = Single.ValueOf("432.1235");
            assertTrue("Incorrect float returned--wanted: " + wanted + " but got: " + got, got
                    .Equals(wanted));

            wanted = new Single(0f);
            got = Single.ValueOf("0");
            assertTrue("Incorrect float returned--wanted: " + wanted + " but got: " + got, got
                    .Equals(wanted));

            wanted = new Single(-1212.3232f);
            got = Single.ValueOf("-1212.3232");
            assertTrue("Incorrect float returned--wanted: " + wanted + " but got: " + got, got
                    .Equals(wanted));

            //try
            //{
            //    Single.ValueOf(null);
            //    fail("Expected Float.valueOf(null) to throw NPE.");
            //}
            //catch (ArgumentNullException ex)
            //{
            //    // expected
            //}

            assertEquals(Single.ValueOf(0f), Single.ValueOf(null)); // J2N: Match .NET behavior so this is more of a drop-in replacement

            try
            {
                Single.ValueOf("");
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
            Single posZero_de = Single.ValueOf("+0.0", new CultureInfo("de-DE"));
            Single negZero_de = Single.ValueOf("-0.0", new CultureInfo("de-DE"));
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
    }
}
