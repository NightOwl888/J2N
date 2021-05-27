﻿using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace J2N.Numerics
{
    public class TestInt32 : TestCase
    {
        /**
     * @tests java.lang.Integer#byteValue()
     */
        [Test]
        public void Test_byteValue()
        {
            // Test for method byte java.lang.Integer.GetByteValue()
            assertEquals("Returned incorrect byte value", -1, (sbyte)new Int32(65535) // J2N: cast required to change the result to negative
                    .GetByteValue());
            assertEquals("Returned incorrect byte value", 127, new Int32(127)
                    .GetByteValue());
        }

        /**
         * @tests java.lang.Integer#compareTo(java.lang.Integer)
         */
        [Test]
        public void Test_compareToLjava_lang_Int32()
        {
            // Test for method int java.lang.Integer.compareTo(java.lang.Integer)
            assertTrue("-2 compared to 1 gave non-negative answer", new Int32(-2)
                    .CompareTo(new Int32(1)) < 0);
            assertEquals("-2 compared to -2 gave non-zero answer", 0, new Int32(-2)
                    .CompareTo(new Int32(-2)));
            assertTrue("3 compared to 2 gave non-positive answer", new Int32(3)
                    .CompareTo(new Int32(2)) > 0);

            //try
            //{
            //    new Int32(0).CompareTo(null);
            //    fail("No NPE");
            //}
            //catch (NullPointerException e)
            //{
            //}

            // J2N: Return 1 when comparing to null to match other .NET classes
            assertEquals(1, new Int32(0).CompareTo(null));
        }

        /**
         * @tests java.lang.Integer#decode(java.lang.String)
         */
        [Test]
        public void Test_decodeLjava_lang_String2()
        {
            // Test for method java.lang.Integer
            // java.lang.Integer.decode(java.lang.String)
            assertEquals("Failed for 132233",
                    132233, Int32.Decode("132233").GetInt32Value());
            assertEquals("Failed for 07654321",
                    /*07654321*/ 2054353, Int32.Decode("07654321").GetInt32Value()); // J2N: Octal literals are not supported in C#, changed to decimal representation
            assertTrue("Failed for #1234567",
                    Int32.Decode("#1234567").GetInt32Value() == 0x1234567);
            assertTrue("Failed for 0xdAd",
                    Int32.Decode("0xdAd").GetInt32Value() == 0xdad);
            assertEquals("Failed for -23", -23, Int32.Decode("-23").GetInt32Value());
            assertEquals("Returned incorrect value for 0 decimal", 0, Int32
                    .Decode("0").GetInt32Value());
            assertEquals("Returned incorrect value for 0 hex", 0, Int32.Decode("0x0")
                    .GetInt32Value());
            assertTrue("Returned incorrect value for most negative value decimal",
                    Int32.Decode("-2147483648").GetInt32Value() == unchecked((int)0x80000000));
            assertTrue("Returned incorrect value for most negative value hex",
                    Int32.Decode("-0x80000000").GetInt32Value() == unchecked((int)0x80000000));
            assertTrue("Returned incorrect value for most positive value decimal",
                    Int32.Decode("2147483647").GetInt32Value() == 0x7fffffff);
            assertTrue("Returned incorrect value for most positive value hex",
                    Int32.Decode("0x7fffffff").GetInt32Value() == 0x7fffffff);

            bool exception = false;
            try
            {
                Int32.Decode("0a");
            }
            catch (FormatException e)
            {
                // correct
                exception = true;
            }
            assertTrue("Failed to throw FormatException for \"Oa\"",
                    exception);

            exception = false;
            try
            {
                Int32.Decode("2147483648");
            }
            catch (OverflowException e) // J2N: .NET throws OverflowException rather than FormatException in this case
            {
                // Correct
                exception = true;
            }
            assertTrue("Failed to throw exception for MAX_VALUE + 1", exception);

            exception = false;
            try
            {
                Int32.Decode("-2147483649");
            }
            catch (OverflowException e) // J2N: .NET throws OverflowException rather than FormatException in this case
            {
                // Correct
                exception = true;
            }
            assertTrue("Failed to throw exception for MIN_VALUE - 1", exception);

            // J2N: MinValue is a special case and must allow both a positive and negative version to be compatible
            // with both .NET and Java
            //exception = false;
            //try
            //{
            //    Int32.Decode("0x80000000");
            //}
            //catch (OverflowException e) // J2N: .NET throws OverflowException rather than FormatException in this case
            //{
            //    // Correct
            //    exception = true;
            //}
            //assertTrue("Failed to throw exception for hex MAX_VALUE + 1", exception);

            exception = false;
            try
            {
                Int32.Decode("-0x80000001");
            }
            catch (OverflowException e) // J2N: .NET throws OverflowException rather than FormatException in this case
            {
                // Correct
                exception = true;
            }
            assertTrue("Failed to throw exception for hex MIN_VALUE - 1", exception);

            exception = false;
            try
            {
                Int32.Decode("9999999999"); // J2N: .NET throws OverflowException rather than FormatException in this case
            }
            catch (OverflowException e)
            {
                // Correct
                exception = true;
            }
            assertTrue("Failed to throw exception for 9999999999", exception);

            try
            {
                Int32.Decode("-");
                fail("Expected exception for -");
            }
            catch (FormatException e)
            {
                // Expected
            }

            try
            {
                Int32.Decode("0x");
                fail("Expected exception for 0x");
            }
            catch (FormatException e)
            {
                // Expected
            }

            try
            {
                Int32.Decode("#");
                fail("Expected exception for #");
            }
            catch (FormatException e)
            {
                // Expected
            }

            try
            {
                Int32.Decode("x123");
                fail("Expected exception for x123");
            }
            catch (FormatException e)
            {
                // Expected
            }

            try
            {
                Int32.Decode(null);
                fail("Expected exception for null");
            }
            catch (ArgumentNullException e) // J2N: Support null to zero like in Convert.ToInt32?
            {
                // Expected
            }

            try
            {
                Int32.Decode("");
                fail("Expected exception for empty string");
            }
            catch (FormatException ex)
            {
                // Expected
            }

            try
            {
                Int32.Decode(" ");
                fail("Expected exception for single space");
            }
            catch (FormatException ex)
            {
                // Expected
            }

        }

        /**
         * @tests java.lang.Integer#doubleValue()
         */
        [Test]
        public void Test_doubleValue2()
        {
            // Test for method double java.lang.Integer.doubleValue()
            assertEquals("Returned incorrect double value", 2147483647.0, new Int32(2147483647)
                    .GetDoubleValue(), 0.0D);
            assertEquals("Returned incorrect double value", -2147483647.0, new Int32(-2147483647)
                    .GetDoubleValue(), 0.0D);
        }

        /**
         * @tests java.lang.Integer#equals(java.lang.Object)
         */
        [Test]
        public void Test_equalsLjava_lang_Object2()
        {
            // Test for method boolean java.lang.Integer.equals(java.lang.Object)
            Int32 i1 = new Int32(1000);
            Int32 i2 = new Int32(1000);
            Int32 i3 = new Int32(-1000);
            assertTrue("Equality test failed", i1.Equals(i2) && !(i1.Equals(i3)));
        }

        /**
         * @tests java.lang.Integer#floatValue()
         */
        [Test]
        public void Test_floatValue2()
        {
            // Test for method float java.lang.Integer.floatValue()
            assertTrue("Returned incorrect float value", new Int32(65535)
                    .GetSingleValue() == 65535.0f);
            assertTrue("Returned incorrect float value", new Int32(-65535)
                    .GetSingleValue() == -65535.0f);
        }

        ///**
        // * @tests java.lang.Integer#getInt32(java.lang.String)
        // */
        //[Test]
        //public void Test_getIntegerLjava_lang_String()
        //{
        //    // Test for method java.lang.Integer
        //    // java.lang.Integer.getInt32(java.lang.String)
        //    Properties tProps = new Properties();
        //    tProps.put("testInt", "99");
        //    System.setProperties(tProps);
        //    assertTrue("returned incorrect Integer", Int32.getInt32("testInt")
        //            .equals(new Int32(99)));
        //    assertNull("returned incorrect default Integer", Integer
        //            .getInt32("ff"));
        //}

        ///**
        // * @tests java.lang.Integer#getInt32(java.lang.String, int)
        // */
        //[Test]
        //public void Test_getIntegerLjava_lang_StringI()
        //{
        //    // Test for method java.lang.Integer
        //    // java.lang.Integer.getInt32(java.lang.String, int)
        //    Properties tProps = new Properties();
        //    tProps.put("testInt", "99");
        //    System.setProperties(tProps);
        //    assertTrue("returned incorrect Integer", Int32.getInt32("testInt",
        //            4).equals(new Int32(99)));
        //    assertTrue("returned incorrect default Integer", Int32.getInt32(
        //            "ff", 4).equals(new Int32(4)));
        //}

        ///**
        // * @tests java.lang.Integer#getInt32(java.lang.String, java.lang.Integer)
        // */
        //[Test]
        //public void Test_getIntegerLjava_lang_StringLjava_lang_Int32()
        //{
        //    // Test for method java.lang.Integer
        //    // java.lang.Integer.getInt32(java.lang.String, java.lang.Integer)
        //    Properties tProps = new Properties();
        //    tProps.put("testInt", "99");
        //    System.setProperties(tProps);
        //    assertTrue("returned incorrect Integer", Int32.getInt32("testInt",
        //            new Int32(4)).equals(new Int32(99)));
        //    assertTrue("returned incorrect default Integer", Int32.getInt32(
        //            "ff", new Int32(4)).equals(new Int32(4)));
        //}

        /**
         * @tests java.lang.Integer#hashCode()
         */
        [Test]
        public void Test_hashCode2()
        {
            // Test for method int java.lang.Integer.hashCode()

            Int32 i1 = new Int32(1000);
            Int32 i2 = new Int32(-1000);
            assertTrue("Returned incorrect hashcode", i1.GetHashCode() == 1000
                    && (i2.GetHashCode() == -1000));
        }

        /**
         * @tests java.lang.Integer#intValue()
         */
        [Test]
        public void Test_intValue2()
        {
            // Test for method int java.lang.Integer.intValue()

            Int32 i = new Int32(8900);
            assertEquals("Returned incorrect int value", 8900, i.GetInt32Value());
        }

        /**
         * @tests java.lang.Integer#longValue()
         */
        [Test]
        public void Test_longValue2()
        {
            // Test for method long java.lang.Integer.longValue()
            Int32 i = new Int32(8900);
            assertEquals("Returned incorrect long value", 8900L, i.GetInt64Value());
        }

        /**
         * @tests java.lang.Integer#parseInt(java.lang.String)
         */
        [Test]
        public void Test_parseIntLjava_lang_String2()
        {
            // Test for method int java.lang.Integer.parseInt(java.lang.String)

            int i = Int32.Parse("-8900", J2N.Text.StringFormatter.InvariantCulture);
            assertEquals("Returned incorrect int", -8900, i);
            assertEquals("Returned incorrect value for 0", 0, Int32.Parse("0", J2N.Text.StringFormatter.InvariantCulture));
            assertTrue("Returned incorrect value for most negative value", Int32
                    .Parse("-2147483648", J2N.Text.StringFormatter.InvariantCulture) == unchecked((int)0x80000000));
            assertTrue("Returned incorrect value for most positive value", Int32
                    .Parse("2147483647", J2N.Text.StringFormatter.InvariantCulture) == 0x7fffffff);

            bool exception = false;
            try
            {
                Int32.Parse("999999999999", J2N.Text.StringFormatter.InvariantCulture);
            }
            catch (OverflowException e) // J2N: .NET throws OverflowException rather than FormatException in this case
            {
                // Correct
                exception = true;
            }
            assertTrue("Failed to throw exception for value > int", exception);

            exception = false;
            try
            {
                Int32.Parse("2147483648", J2N.Text.StringFormatter.InvariantCulture);
            }
            catch (OverflowException e) // J2N: .NET throws OverflowException rather than FormatException in this case
            {
                // Correct
                exception = true;
            }
            assertTrue("Failed to throw exception for MAX_VALUE + 1", exception);

            exception = false;
            try
            {
                Int32.Parse("-2147483649", J2N.Text.StringFormatter.InvariantCulture);
            }
            catch (OverflowException e) // J2N: .NET throws OverflowException rather than FormatException in this case
            {
                // Correct
                exception = true;
            }
            assertTrue("Failed to throw exception for MIN_VALUE - 1", exception);
        }

        /**
         * @tests java.lang.Integer#parseInt(java.lang.String, int)
         */
        [Test]
        public void Test_parseIntLjava_lang_StringI2()
        {
            // Test for method int java.lang.Integer.parseInt(java.lang.String, int)
            assertEquals("Parsed dec val incorrectly",
                    -8000, Int32.Parse("-8000", 10));
            assertEquals("Parsed hex val incorrectly",
                    255, Int32.Parse("FF", 16));
            assertEquals("Parsed oct val incorrectly",
                    16, Int32.Parse("20", 8));
            assertEquals("Returned incorrect value for 0 hex", 0, Int32.Parse("0",
                    16));
            assertTrue("Returned incorrect value for most negative value hex",
                    Int32.Parse("-80000000", 16) == unchecked((int)0x80000000));
            assertTrue("Returned incorrect value for most positive value hex",
                    Int32.Parse("7fffffff", 16) == 0x7fffffff);
            assertEquals("Returned incorrect value for 0 decimal", 0, Int32.Parse(
                    "0", 10));
            assertTrue("Returned incorrect value for most negative value decimal",
                    Int32.Parse("-2147483648", 10) == unchecked((int)0x80000000));
            assertTrue("Returned incorrect value for most positive value decimal",
                    Int32.Parse("2147483647", 10) == 0x7fffffff);

            bool exception = false;
            try
            {
                Int32.Parse("FFFF", 10);
            }
            catch (FormatException e)
            {
                // Correct
                exception = true;
            }
            assertTrue(
                    "Failed to throw exception when passes hex string and dec parm",
                    exception);

            exception = false;
            try
            {
                Int32.Parse("2147483648", 10);
            }
            catch (OverflowException e) // J2N: .NET throws OverflowException rather than FormatException in this case
            {
                // Correct
                exception = true;
            }
            assertTrue("Failed to throw exception for MAX_VALUE + 1", exception);

            exception = false;
            try
            {
                Int32.Parse("-2147483649", 10);
            }
            catch (OverflowException e) // J2N: .NET throws OverflowException rather than FormatException in this case
            {
                // Correct
                exception = true;
            }
            assertTrue("Failed to throw exception for MIN_VALUE - 1", exception);

            // J2N: MinValue is a special case and must allow both a positive and negative version to be compatible
            // with both .NET and Java
            //exception = false;
            //try
            //{
            //    Int32.Parse("80000000", 16);
            //}
            //catch (OverflowException e) // J2N: .NET throws OverflowException rather than FormatException in this case
            //{
            //    // Correct
            //    exception = true;
            //}
            //assertTrue("Failed to throw exception for hex MAX_VALUE + 1", exception);

            assertEquals(1, Int32.Parse("1", 16));
            assertEquals(-1, Int32.Parse("ffffffff", 16));
            assertEquals(2147483647, Int32.Parse("7fffffff", 16));
            assertEquals(-2147483648, Int32.Parse("-80000000", 16)); // Special case: In Java, we allow the negative sign for the smallest negative number
            assertEquals(-2147483648, Int32.Parse("80000000", 16));  // In .NET, it should parse without the negative sign to the same value (in .NET the negative sign is not allowed)
            assertEquals(-2147483647, Int32.Parse("80000001", 16));

            exception = false;
            try
            {
                Int32.Parse("-80000001", 16);
            }
            catch (OverflowException e) // J2N: .NET throws OverflowException rather than FormatException in this case
            {
                // Correct
                exception = true;
            }
            assertTrue("Failed to throw exception for hex MIN_VALUE + 1", exception);

            exception = false;
            try
            {
                Int32.Parse("9999999999", 10);
            }
            catch (OverflowException e) // J2N: .NET throws OverflowException rather than FormatException in this case
            {
                // Correct
                exception = true;
            }
            assertTrue("Failed to throw exception for 9999999999", exception);
        }

        /**
         * @tests java.lang.Integer#shortValue()
         */
        [Test]
        public void Test_shortValue2()
        {
            // Test for method short java.lang.Integer.shortValue()
            Int32 i = new Int32(2147450880);
            assertEquals("Returned incorrect long value", -32768, i.GetInt16Value());
        }

        /**
         * @tests java.lang.Integer#toBinaryString(int)
         */
        [Test]
        public void Test_toBinaryStringI()
        {
            // Test for method java.lang.String
            // java.lang.Integer.toBinaryString(int)
            assertEquals("Incorrect string returned", "1111111111111111111111111111111", Int32.ToBinaryString(
                    int.MaxValue));
            assertEquals("Incorrect string returned", "10000000000000000000000000000000", Int32.ToBinaryString(
                    int.MinValue));
        }

        /**
         * @tests java.lang.Integer#toHexString(int)
         */
        [Test]
        public void Test_toHexStringI()
        {
            // Test for method java.lang.String java.lang.Integer.toHexString(int)

            String[] hexvals = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9",
                "a", "b", "c", "d", "e", "f" };

            for (int i = 0; i < 16; i++)
            {
                assertTrue("Incorrect string returned " + hexvals[i], Int32
                        .ToHexString(i).Equals(hexvals[i]));
            }

            assertTrue("Returned incorrect hex string: "
                    + Int32.ToHexString(int.MaxValue), Int32.ToHexString(
                    int.MaxValue).Equals("7fffffff"));
            assertTrue("Returned incorrect hex string: "
                    + Int32.ToHexString(int.MinValue), Int32.ToHexString(
                    int.MinValue).Equals("80000000"));
        }

        /**
         * @tests java.lang.Integer#toOctalString(int)
         */
        [Test]
        public void Test_toOctalStringI()
        {
            // Test for method java.lang.String java.lang.Integer.toOctalString(int)
            // Spec states that the int arg is treated as unsigned
            assertEquals("Returned incorrect octal string", "17777777777", Int32.ToOctalString(
                    int.MaxValue));
            assertEquals("Returned incorrect octal string", "20000000000", Int32.ToOctalString(
                    int.MinValue));
        }

        /**
         * @tests java.lang.Integer#toString()
         */
        [Test]
        public void Test_toString2()
        {
            // Test for method java.lang.String java.lang.Integer.toString()

            Int32 i = new Int32(-80001);

            assertEquals("Returned incorrect String", "-80001", i.ToString());
        }

        /**
         * @tests java.lang.Integer#toString(int)
         */
        [Test]
        public void Test_toStringI2()
        {
            // Test for method java.lang.String java.lang.Integer.toString(int)

            assertEquals("Returned incorrect String", "-80765", Int32.ToString(-80765)
                    );
            assertEquals("Returned incorrect octal string", "2147483647", Int32.ToString(
                    int.MaxValue));
            assertEquals("Returned incorrect octal string", "-2147483647", Int32.ToString(
                    -int.MaxValue));
            assertEquals("Returned incorrect octal string", "-2147483648", Int32.ToString(
                    int.MinValue));

            // Test for HARMONY-6068
            assertEquals("Returned incorrect octal String", "-1000", Int32.ToString(-1000));
            assertEquals("Returned incorrect octal String", "1000", Int32.ToString(1000));
            assertEquals("Returned incorrect octal String", "0", Int32.ToString(0));
            assertEquals("Returned incorrect octal String", "708", Int32.ToString(708));
            assertEquals("Returned incorrect octal String", "-100", Int32.ToString(-100));
            assertEquals("Returned incorrect octal String", "-1000000008", Int32.ToString(-1000000008));
            assertEquals("Returned incorrect octal String", "2000000008", Int32.ToString(2000000008));
        }

        /**
         * @tests java.lang.Integer#toString(int, int)
         */
        [Test]
        public void Test_toStringII()
        {
            // Test for method java.lang.String java.lang.Integer.toString(int, int)
            assertEquals("Returned incorrect octal string", "17777777777", Int32.ToString(
                    2147483647, 8));
            assertTrue("Returned incorrect hex string--wanted 7fffffff but got: "
                    + Int32.ToString(2147483647, 16), Int32.ToString(
                    2147483647, 16).Equals("7fffffff"));
            assertEquals("Incorrect string returned", "1111111111111111111111111111111", Int32.ToString(2147483647, 2)
                    );
            assertEquals("Incorrect string returned", "2147483647", Int32
                    .ToString(2147483647, 10));

            assertEquals("Returned incorrect octal string", "-17777777777", Int32.ToString(
                    -2147483647, 8));
            assertTrue("Returned incorrect hex string--wanted -7fffffff but got: "
                    + Int32.ToString(-2147483647, 16), Int32.ToString(
                    -2147483647, 16).Equals("-7fffffff"));
            assertEquals("Incorrect string returned",
                            "-1111111111111111111111111111111", Int32
                    .ToString(-2147483647, 2));
            assertEquals("Incorrect string returned", "-2147483647", Int32.ToString(-2147483647,
                    10));

            assertEquals("Returned incorrect octal string", "-20000000000", Int32.ToString(
                    -2147483648, 8));
            assertTrue("Returned incorrect hex string--wanted -80000000 but got: "
                    + Int32.ToString(-2147483648, 16), Int32.ToString(
                    -2147483648, 16).Equals("-80000000"));
            assertEquals("Incorrect string returned",
                            "-10000000000000000000000000000000", Int32
                    .ToString(-2147483648, 2));
            assertEquals("Incorrect string returned", "-2147483648", Int32.ToString(-2147483648,
                    10));
        }

        /**
         * @tests java.lang.Integer#valueOf(java.lang.String)
         */
        [Test]
        public void Test_valueOfLjava_lang_String2()
        {
            // Test for method java.lang.Integer
            // java.lang.Integer.valueOf(java.lang.String)
            assertEquals("Returned incorrect int", 8888888, Int32.ValueOf("8888888", J2N.Text.StringFormatter.InvariantCulture)
                    .GetInt32Value());
            assertTrue("Returned incorrect int", Int32.ValueOf("2147483647", J2N.Text.StringFormatter.InvariantCulture)
                    .GetInt32Value() == int.MaxValue);
            assertTrue("Returned incorrect int", Int32.ValueOf("-2147483648", J2N.Text.StringFormatter.InvariantCulture)
                    .GetInt32Value() == int.MinValue);

            bool exception = false;
            try
            {
                Int32.ValueOf("2147483648", J2N.Text.StringFormatter.InvariantCulture);
            }
            catch (OverflowException e) // J2N: .NET throws OverflowException rather than FormatException in this case
            {
                // Correct
                exception = true;
            }
            assertTrue("Failed to throw exception with MAX_VALUE + 1", exception);

            exception = false;
            try
            {
                Int32.ValueOf("-2147483649", J2N.Text.StringFormatter.InvariantCulture);
            }
            catch (OverflowException e) // J2N: .NET throws OverflowException rather than FormatException in this case
            {
                // Correct
                exception = true;
            }
            assertTrue("Failed to throw exception with MIN_VALUE - 1", exception);
        }

        /**
         * @tests java.lang.Integer#valueOf(java.lang.String, int)
         */
        [Test]
        public void Test_valueOfLjava_lang_StringI2()
        {
            // Test for method java.lang.Integer
            // java.lang.Integer.valueOf(java.lang.String, int)
            assertEquals("Returned incorrect int for hex string", 255, Int32.ValueOf(
                    "FF", 16).GetInt32Value());
            assertEquals("Returned incorrect int for oct string", 16, Int32.ValueOf(
                    "20", 8).GetInt32Value());
            assertEquals("Returned incorrect int for bin string", 4, Int32.ValueOf(
                    "100", 2).GetInt32Value());

            assertEquals("Returned incorrect int for - hex string", -255, Int32.ValueOf(
                    "-FF", 16).GetInt32Value());
            assertEquals("Returned incorrect int for - oct string", -16, Int32.ValueOf(
                    "-20", 8).GetInt32Value());
            assertEquals("Returned incorrect int for - bin string", -4, Int32.ValueOf(
                    "-100", 2).GetInt32Value());
            assertTrue("Returned incorrect int", Int32.ValueOf("2147483647", 10)
                    .GetInt32Value() == int.MaxValue);
            assertTrue("Returned incorrect int", Int32.ValueOf("-2147483648", 10)
                    .GetInt32Value() == int.MinValue);
            assertTrue("Returned incorrect int", Int32.ValueOf("7fffffff", 16)
                    .GetInt32Value() == int.MaxValue);
            assertTrue("Returned incorrect int", Int32.ValueOf("-80000000", 16)
                    .GetInt32Value() == int.MinValue);

            bool exception = false;
            try
            {
                Int32.ValueOf("FF", 2);
            }
            catch (FormatException e)
            {
                // Correct
                exception = true;
            }
            assertTrue(
                    "Failed to throw exception with hex string and base 2 radix",
                    exception);

            exception = false;
            try
            {
                Int32.ValueOf("2147483648", 10);
            }
            catch (OverflowException e) // J2N: .NET throws OverflowException rather than FormatException in this case
            {
                // Correct
                exception = true;
            }
            assertTrue("Failed to throw exception with MAX_VALUE + 1", exception);

            exception = false;
            try
            {
                Int32.ValueOf("-2147483649", 10);
            }
            catch (OverflowException e) // J2N: .NET throws OverflowException rather than FormatException in this case
            {
                // Correct
                exception = true;
            }
            assertTrue("Failed to throw exception with MIN_VALUE - 1", exception);

            // J2N: MinValue is a special case and must allow both a positive and negative version to be compatible
            // with both .NET and Java
            //exception = false;
            //try
            //{
            //    Int32.ValueOf("80000000", 16);
            //}
            //catch (OverflowException e) // J2N: .NET throws OverflowException rather than FormatException in this case
            //{
            //    // Correct
            //    exception = true;
            //}
            //assertTrue("Failed to throw exception with hex MAX_VALUE + 1",
            //        exception);

            assertEquals(1, Int32.ValueOf("1", 16).GetInt32Value());
            assertEquals(-1, Int32.ValueOf("ffffffff", 16).GetInt32Value());
            assertEquals(2147483647, Int32.ValueOf("7fffffff", 16).GetInt32Value());
            assertEquals(-2147483648, Int32.ValueOf("-80000000", 16).GetInt32Value()); // Special case: In Java, we allow the negative sign for the smallest negative number
            assertEquals(-2147483648, Int32.ValueOf("80000000", 16).GetInt32Value());  // In .NET, it should parse without the negative sign to the same value (in .NET the negative sign is not allowed)
            assertEquals(-2147483647, Int32.ValueOf("80000001", 16).GetInt32Value());

            exception = false;
            try
            {
                Int32.ValueOf("-80000001", 16);
            }
            catch (OverflowException e) // J2N: .NET throws OverflowException rather than FormatException in this case
            {
                // Correct
                exception = true;
            }
            assertTrue("Failed to throw exception with hex MIN_VALUE - 1",
                    exception);
        }

        /**
         * @tests java.lang.Integer#valueOf(byte)
         */
        [Test]
        public void Test_valueOfI()
        {
            assertEquals(new Int32(int.MinValue), Int32.ValueOf(int.MinValue));
            assertEquals(new Int32(int.MaxValue), Int32.ValueOf(int.MaxValue));
            assertEquals(new Int32(0), Int32.ValueOf(0));

            short s = -128;
            while (s < 128)
            {
                assertEquals(new Int32(s), Int32.ValueOf(s));
                assertSame(Int32.ValueOf(s), Int32.ValueOf(s));
                s++;
            }
        }

        /**
         * @tests java.lang.Integer#hashCode()
         */
        [Test]
        public void Test_hashCode()
        {
            assertEquals(1, new Int32(1).GetHashCode());
            assertEquals(2, new Int32(2).GetHashCode());
            assertEquals(0, new Int32(0).GetHashCode());
            assertEquals(-1, new Int32(-1).GetHashCode());
        }

        /**
         * @tests java.lang.Integer#Int32(String)
         */
        [Test]
        public void Test_ConstructorLjava_lang_String()
        {
            assertEquals(new Int32(0), new Int32("0", J2N.Text.StringFormatter.InvariantCulture));
            assertEquals(new Int32(1), new Int32("1", J2N.Text.StringFormatter.InvariantCulture));
            assertEquals(new Int32(-1), new Int32("-1", J2N.Text.StringFormatter.InvariantCulture));

            try
            {
                new Int32("0x1", J2N.Text.StringFormatter.InvariantCulture);
                fail("Expected FormatException with hex string.");
            }
            catch (FormatException e) { }

            try
            {
                new Int32("9.2", J2N.Text.StringFormatter.InvariantCulture);
                fail("Expected FormatException with floating point string.");
            }
            catch (FormatException e) { }

            try
            {
                new Int32("", J2N.Text.StringFormatter.InvariantCulture);
                fail("Expected FormatException with empty string.");
            }
            catch (FormatException e) { }

            try
            {
                new Int32(null, J2N.Text.StringFormatter.InvariantCulture);
                fail("Expected FormatException with null string.");
            }
            catch (ArgumentNullException e) { } // J2N: .NET throws ArgumentNullException rather than FormatException in this case
        }

        /**
         * @tests java.lang.Integer#Integer
         */
        [Test]
        public void Test_ConstructorI()
        {
            assertEquals(1, new Int32(1).GetInt32Value());
            assertEquals(2, new Int32(2).GetInt32Value());
            assertEquals(0, new Int32(0).GetInt32Value());
            assertEquals(-1, new Int32(-1).GetInt32Value());

            Int32 i = new Int32(-89000);
            assertEquals("Incorrect Integer created", -89000, i.GetInt32Value());
        }

        /**
         * @tests java.lang.Integer#byteValue()
         */
        [Test]
        public void Test_booleanValue()
        {
            assertEquals(1, new Int32(1).GetByteValue());
            assertEquals(2, new Int32(2).GetByteValue());
            assertEquals(0, new Int32(0).GetByteValue());
            assertEquals(-1, (sbyte)new Int32(-1).GetByteValue()); // J2N: cast required to change the result to negative
        }

        /**
         * @tests java.lang.Integer#equals(Object)
         */
        [Test]
        public void Test_equalsLjava_lang_Object()
        {
            assertEquals(new Int32(0), Int32.ValueOf(0));
            assertEquals(new Int32(1), Int32.ValueOf(1));
            assertEquals(new Int32(-1), Int32.ValueOf(-1));

            Int32 fixture = new Int32(25);
            assertEquals(fixture, fixture);
            assertFalse(fixture.Equals(null));
            assertFalse(fixture.Equals("Not a Integer"));
        }

        /**
         * @tests java.lang.Integer#toString()
         */
        [Test]
        public void Test_toString() // J2N TODO: Culture tests
        {
            assertEquals("-1", new Int32(-1).ToString());
            assertEquals("0", new Int32(0).ToString());
            assertEquals("1", new Int32(1).ToString());
            assertEquals("-1", new Int32(unchecked((int)0xFFFFFFFF)).ToString());
        }

        /**
         * @tests java.lang.Integer#toString
         */
        [Test]
        public void Test_toStringI() // J2N TODO: Culture tests
        {
            assertEquals("-1", Int32.ToString(-1));
            assertEquals("0", Int32.ToString(0));
            assertEquals("1", Int32.ToString(1));
            assertEquals("-1", Int32.ToString(unchecked((int)0xFFFFFFFF)));
        }

        /**
         * @tests java.lang.Integer#valueOf(String)
         */
        [Test]
        public void Test_valueOfLjava_lang_String()
        {
            assertEquals(new Int32(0), Int32.ValueOf("0", J2N.Text.StringFormatter.InvariantCulture));
            assertEquals(new Int32(1), Int32.ValueOf("1", J2N.Text.StringFormatter.InvariantCulture));
            assertEquals(new Int32(-1), Int32.ValueOf("-1", J2N.Text.StringFormatter.InvariantCulture));

            try
            {
                Int32.ValueOf("0x1", J2N.Text.StringFormatter.InvariantCulture);
                fail("Expected FormatException with hex string.");
            }
            catch (FormatException e) { }

            try
            {
                Int32.ValueOf("9.2", J2N.Text.StringFormatter.InvariantCulture);
                fail("Expected FormatException with floating point string.");
            }
            catch (FormatException e) { }

            try
            {
                Int32.ValueOf("", J2N.Text.StringFormatter.InvariantCulture);
                fail("Expected FormatException with empty string.");
            }
            catch (FormatException e) { }

            try
            {
                Int32.ValueOf(null, J2N.Text.StringFormatter.InvariantCulture);
                fail("Expected FormatException with null string.");
            }
            catch (ArgumentNullException e) { } // J2N: .NET throws ArgumentNullException rather than FormatException in this case
        }

        /**
         * @tests java.lang.Integer#valueOf(String,int)
         */
        [Test]
        public void Test_valueOfLjava_lang_StringI()
        {
            assertEquals(new Int32(0), Int32.ValueOf("0", 10));
            assertEquals(new Int32(1), Int32.ValueOf("1", 10));
            assertEquals(new Int32(-1), Int32.ValueOf("-1", 10));

            //must be consistent with Character.digit()
            assertEquals(Character.Digit('1', 2), Int32.ValueOf("1", 2).GetByteValue());
            assertEquals(Character.Digit('F', 16), Int32.ValueOf("F", 16).GetByteValue());

            try
            {
                Int32.ValueOf("0x1", 10);
                fail("Expected FormatException with hex string.");
            }
            catch (FormatException e) { }

            try
            {
                Int32.ValueOf("9.2", 10);
                fail("Expected FormatException with floating point string.");
            }
            catch (FormatException e) { }

            try
            {
                Int32.ValueOf("", 10);
                fail("Expected FormatException with empty string.");
            }
            catch (FormatException e) { }

            //try
            //{
            //    Int32.ValueOf(null, 10);
            //    fail("Expected FormatException with null string.");
            //}
            //catch (FormatException e) { }

            // J2N: Match .NET behavior and return 0 for a null string
            assertEquals(0, Int32.ValueOf(null, 10));
        }

        /**
         * @tests java.lang.Integer#parseInt(String)
         */
        [Test]
        public void Test_parseIntLjava_lang_String()
        {
            assertEquals(0, Int32.Parse("0", J2N.Text.StringFormatter.InvariantCulture));
            assertEquals(1, Int32.Parse("1", J2N.Text.StringFormatter.InvariantCulture));
            assertEquals(-1, Int32.Parse("-1", J2N.Text.StringFormatter.InvariantCulture));

            try
            {
                Int32.Parse("0x1", J2N.Text.StringFormatter.InvariantCulture);
                fail("Expected FormatException with hex string.");
            }
            catch (FormatException e) { }

            try
            {
                Int32.Parse("9.2", J2N.Text.StringFormatter.InvariantCulture);
                fail("Expected FormatException with floating point string.");
            }
            catch (FormatException e) { }

            try
            {
                Int32.Parse("", J2N.Text.StringFormatter.InvariantCulture);
                fail("Expected FormatException with empty string.");
            }
            catch (FormatException e) { }

            try
            {
                Int32.Parse(null, J2N.Text.StringFormatter.InvariantCulture);
                fail("Expected FormatException with null string.");
            }
            catch (ArgumentNullException e) { } // J2N: .NET throws ArgumentNullException rather than FormatException in this case
        }

        /**
         * @tests java.lang.Integer#parseInt(String,int)
         */
        [Test]
        public void Test_parseIntLjava_lang_StringI()
        {
            assertEquals(0, Int32.Parse("0", 10));
            assertEquals(1, Int32.Parse("1", 10));
            assertEquals(-1, Int32.Parse("-1", 10));

            //must be consistent with Character.digit()
            assertEquals(Character.Digit('1', 2), Int32.Parse("1", 2));
            assertEquals(Character.Digit('F', 16), Int32.Parse("F", 16));

            try
            {
                Int32.Parse("0x1", 10);
                fail("Expected FormatException with hex string.");
            }
            catch (FormatException e) { }

            try
            {
                Int32.Parse("9.2", 10);
                fail("Expected FormatException with floating point string.");
            }
            catch (FormatException e) { }

            try
            {
                Int32.Parse("", 10);
                fail("Expected FormatException with empty string.");
            }
            catch (FormatException e) { }

            //try
            //{
            //    Int32.Parse(null, 10);
            //    fail("Expected FormatException with null string.");
            //}
            //catch (FormatException e) { }

            // J2N: Match .NET behavior where null will result in 0
            assertEquals(0, Int32.Parse(null, 10));
        }

        /**
         * @tests java.lang.Integer#decode(String)
         */
        [Test]
        public void Test_decodeLjava_lang_String()
        {
            assertEquals(new Int32(0), Int32.Decode("0"));
            assertEquals(new Int32(1), Int32.Decode("1"));
            assertEquals(new Int32(-1), Int32.Decode("-1"));
            assertEquals(new Int32(0xF), Int32.Decode("0xF"));
            assertEquals(new Int32(0xF), Int32.Decode("#F"));
            assertEquals(new Int32(0xF), Int32.Decode("0XF"));
            assertEquals(new Int32(07), Int32.Decode("07"));

            try
            {
                Int32.Decode("9.2");
                fail("Expected FormatException with floating point string.");
            }
            catch (FormatException e) { }

            try
            {
                Int32.Decode("");
                fail("Expected FormatException with empty string.");
            }
            catch (FormatException e) { }

            try
            {
                Int32.Decode(null);
                //undocumented NPE, but seems consistent across JREs
                fail("Expected NullPointerException with null string.");
            }
            catch (ArgumentNullException e) { } // J2N TODO: Allow null to be converted to 0 to be consistent with Convert.ToInt32?
        }

        /**
         * @tests java.lang.Integer#doubleValue()
         */
        [Test]
        public void Test_doubleValue()
        {
            assertEquals(-1D, new Int32(-1).GetDoubleValue(), 0D);
            assertEquals(0D, new Int32(0).GetDoubleValue(), 0D);
            assertEquals(1D, new Int32(1).GetDoubleValue(), 0D);
        }

        /**
         * @tests java.lang.Integer#floatValue()
         */
        [Test]
        public void Test_floatValue()
        {
            assertEquals(-1F, new Int32(-1).GetSingleValue(), 0F);
            assertEquals(0F, new Int32(0).GetSingleValue(), 0F);
            assertEquals(1F, new Int32(1).GetSingleValue(), 0F);
        }

        /**
         * @tests java.lang.Integer#intValue()
         */
        [Test]
        public void Test_intValue()
        {
            assertEquals(-1, new Int32(-1).GetInt32Value());
            assertEquals(0, new Int32(0).GetInt32Value());
            assertEquals(1, new Int32(1).GetInt32Value());
        }

        /**
         * @tests java.lang.Integer#longValue()
         */
        [Test]
        public void Test_longValue()
        {
            assertEquals(-1L, new Int32(-1).GetInt64Value());
            assertEquals(0L, new Int32(0).GetInt64Value());
            assertEquals(1L, new Int32(1).GetInt64Value());
        }

        /**
         * @tests java.lang.Integer#shortValue()
         */
        [Test]
        public void Test_shortValue()
        {
            assertEquals(-1, new Int32(-1).GetInt16Value());
            assertEquals(0, new Int32(0).GetInt16Value());
            assertEquals(1, new Int32(1).GetInt16Value());
        }
        /**
         * @tests java.lang.Integer#highestOneBit(int)
         */
        [Test]
        public void Test_highestOneBitI()
        {
            assertEquals(0x08, Int32.HighestOneBit(0x0A));
            assertEquals(0x08, Int32.HighestOneBit(0x0B));
            assertEquals(0x08, Int32.HighestOneBit(0x0C));
            assertEquals(0x08, Int32.HighestOneBit(0x0F));
            assertEquals(0x80, Int32.HighestOneBit(0xFF));

            assertEquals(0x080000, Int32.HighestOneBit(0x0F1234));
            assertEquals(0x800000, Int32.HighestOneBit(0xFF9977));

            assertEquals(unchecked((int)0x80000000), Int32.HighestOneBit(unchecked((int)0xFFFFFFFF)));

            assertEquals(0, Int32.HighestOneBit(0));
            assertEquals(1, Int32.HighestOneBit(1));
            assertEquals(unchecked((int)0x80000000), Int32.HighestOneBit(-1));
        }

        /**
         * @tests java.lang.Integer#lowestOneBit(int)
         */
        [Test]
        public void Test_lowestOneBitI()
        {
            assertEquals(0x10, Int32.LowestOneBit(0xF0));

            assertEquals(0x10, Int32.LowestOneBit(0x90));
            assertEquals(0x10, Int32.LowestOneBit(0xD0));

            assertEquals(0x10, Int32.LowestOneBit(0x123490));
            assertEquals(0x10, Int32.LowestOneBit(0x1234D0));

            assertEquals(0x100000, Int32.LowestOneBit(0x900000));
            assertEquals(0x100000, Int32.LowestOneBit(0xD00000));

            assertEquals(0x40, Int32.LowestOneBit(0x40));
            assertEquals(0x40, Int32.LowestOneBit(0xC0));

            assertEquals(0x4000, Int32.LowestOneBit(0x4000));
            assertEquals(0x4000, Int32.LowestOneBit(0xC000));

            assertEquals(0x4000, Int32.LowestOneBit(unchecked((int)0x99994000)));
            assertEquals(0x4000, Int32.LowestOneBit(unchecked((int)0x9999C000)));

            assertEquals(0, Int32.LowestOneBit(0));
            assertEquals(1, Int32.LowestOneBit(1));
            assertEquals(1, Int32.LowestOneBit(-1));
        }
        /**
         * @tests java.lang.Integer#numberOfLeadingZeros(int)
         */
        [Test]
        public void Test_numberOfLeadingZerosI()
        {
            assertEquals(32, Int32.NumberOfLeadingZeros(0x0));
            assertEquals(31, Int32.NumberOfLeadingZeros(0x1));
            assertEquals(30, Int32.NumberOfLeadingZeros(0x2));
            assertEquals(30, Int32.NumberOfLeadingZeros(0x3));
            assertEquals(29, Int32.NumberOfLeadingZeros(0x4));
            assertEquals(29, Int32.NumberOfLeadingZeros(0x5));
            assertEquals(29, Int32.NumberOfLeadingZeros(0x6));
            assertEquals(29, Int32.NumberOfLeadingZeros(0x7));
            assertEquals(28, Int32.NumberOfLeadingZeros(0x8));
            assertEquals(28, Int32.NumberOfLeadingZeros(0x9));
            assertEquals(28, Int32.NumberOfLeadingZeros(0xA));
            assertEquals(28, Int32.NumberOfLeadingZeros(0xB));
            assertEquals(28, Int32.NumberOfLeadingZeros(0xC));
            assertEquals(28, Int32.NumberOfLeadingZeros(0xD));
            assertEquals(28, Int32.NumberOfLeadingZeros(0xE));
            assertEquals(28, Int32.NumberOfLeadingZeros(0xF));
            assertEquals(27, Int32.NumberOfLeadingZeros(0x10));
            assertEquals(24, Int32.NumberOfLeadingZeros(0x80));
            assertEquals(24, Int32.NumberOfLeadingZeros(0xF0));
            assertEquals(23, Int32.NumberOfLeadingZeros(0x100));
            assertEquals(20, Int32.NumberOfLeadingZeros(0x800));
            assertEquals(20, Int32.NumberOfLeadingZeros(0xF00));
            assertEquals(19, Int32.NumberOfLeadingZeros(0x1000));
            assertEquals(16, Int32.NumberOfLeadingZeros(0x8000));
            assertEquals(16, Int32.NumberOfLeadingZeros(0xF000));
            assertEquals(15, Int32.NumberOfLeadingZeros(0x10000));
            assertEquals(12, Int32.NumberOfLeadingZeros(0x80000));
            assertEquals(12, Int32.NumberOfLeadingZeros(0xF0000));
            assertEquals(11, Int32.NumberOfLeadingZeros(0x100000));
            assertEquals(8, Int32.NumberOfLeadingZeros(0x800000));
            assertEquals(8, Int32.NumberOfLeadingZeros(0xF00000));
            assertEquals(7, Int32.NumberOfLeadingZeros(0x1000000));
            assertEquals(4, Int32.NumberOfLeadingZeros(0x8000000));
            assertEquals(4, Int32.NumberOfLeadingZeros(0xF000000));
            assertEquals(3, Int32.NumberOfLeadingZeros(0x10000000));
            assertEquals(0, Int32.NumberOfLeadingZeros(unchecked((int)0x80000000)));
            assertEquals(0, Int32.NumberOfLeadingZeros(unchecked((int)0xF0000000)));

            assertEquals(1, Int32.NumberOfLeadingZeros(int.MaxValue));
            assertEquals(0, Int32.NumberOfLeadingZeros(int.MinValue));
        }

        /**
         * @tests java.lang.Integer#numberOfTrailingZeros(int)
         */
        [Test]
        public void Test_numberOfTrailingZerosI()
        {
            assertEquals(32, Int32.NumberOfTrailingZeros(0x0));
            assertEquals(31, Int32.NumberOfTrailingZeros(int.MinValue));
            assertEquals(0, Int32.NumberOfTrailingZeros(int.MaxValue));

            assertEquals(0, Int32.NumberOfTrailingZeros(0x1));
            assertEquals(3, Int32.NumberOfTrailingZeros(0x8));
            assertEquals(0, Int32.NumberOfTrailingZeros(0xF));

            assertEquals(4, Int32.NumberOfTrailingZeros(0x10));
            assertEquals(7, Int32.NumberOfTrailingZeros(0x80));
            assertEquals(4, Int32.NumberOfTrailingZeros(0xF0));

            assertEquals(8, Int32.NumberOfTrailingZeros(0x100));
            assertEquals(11, Int32.NumberOfTrailingZeros(0x800));
            assertEquals(8, Int32.NumberOfTrailingZeros(0xF00));

            assertEquals(12, Int32.NumberOfTrailingZeros(0x1000));
            assertEquals(15, Int32.NumberOfTrailingZeros(0x8000));
            assertEquals(12, Int32.NumberOfTrailingZeros(0xF000));

            assertEquals(16, Int32.NumberOfTrailingZeros(0x10000));
            assertEquals(19, Int32.NumberOfTrailingZeros(0x80000));
            assertEquals(16, Int32.NumberOfTrailingZeros(0xF0000));

            assertEquals(20, Int32.NumberOfTrailingZeros(0x100000));
            assertEquals(23, Int32.NumberOfTrailingZeros(0x800000));
            assertEquals(20, Int32.NumberOfTrailingZeros(0xF00000));

            assertEquals(24, Int32.NumberOfTrailingZeros(0x1000000));
            assertEquals(27, Int32.NumberOfTrailingZeros(0x8000000));
            assertEquals(24, Int32.NumberOfTrailingZeros(0xF000000));

            assertEquals(28, Int32.NumberOfTrailingZeros(0x10000000));
            assertEquals(31, Int32.NumberOfTrailingZeros(unchecked((int)0x80000000)));
            assertEquals(28, Int32.NumberOfTrailingZeros(unchecked((int)0xF0000000)));
        }

        /**
         * @tests java.lang.Integer#bitCount(int)
         */
        [Test]
        public void Test_bitCountI()
        {
            assertEquals(0, Int32.BitCount(0x0));
            assertEquals(1, Int32.BitCount(0x1));
            assertEquals(1, Int32.BitCount(0x2));
            assertEquals(2, Int32.BitCount(0x3));
            assertEquals(1, Int32.BitCount(0x4));
            assertEquals(2, Int32.BitCount(0x5));
            assertEquals(2, Int32.BitCount(0x6));
            assertEquals(3, Int32.BitCount(0x7));
            assertEquals(1, Int32.BitCount(0x8));
            assertEquals(2, Int32.BitCount(0x9));
            assertEquals(2, Int32.BitCount(0xA));
            assertEquals(3, Int32.BitCount(0xB));
            assertEquals(2, Int32.BitCount(0xC));
            assertEquals(3, Int32.BitCount(0xD));
            assertEquals(3, Int32.BitCount(0xE));
            assertEquals(4, Int32.BitCount(0xF));

            assertEquals(8, Int32.BitCount(0xFF));
            assertEquals(12, Int32.BitCount(0xFFF));
            assertEquals(16, Int32.BitCount(0xFFFF));
            assertEquals(20, Int32.BitCount(0xFFFFF));
            assertEquals(24, Int32.BitCount(0xFFFFFF));
            assertEquals(28, Int32.BitCount(0xFFFFFFF));
            assertEquals(32, Int32.BitCount(unchecked((int)0xFFFFFFFF)));
        }

        /**
         * @tests java.lang.Integer#rotateLeft(int,int)
         */
        [Test]
        public void Test_rotateLeftII()
        {
            assertEquals(0xF, Int32.RotateLeft(0xF, 0));
            assertEquals(0xF0, Int32.RotateLeft(0xF, 4));
            assertEquals(0xF00, Int32.RotateLeft(0xF, 8));
            assertEquals(0xF000, Int32.RotateLeft(0xF, 12));
            assertEquals(0xF0000, Int32.RotateLeft(0xF, 16));
            assertEquals(0xF00000, Int32.RotateLeft(0xF, 20));
            assertEquals(0xF000000, Int32.RotateLeft(0xF, 24));
            assertEquals(unchecked((int)0xF0000000), Int32.RotateLeft(0xF, 28));
            assertEquals(unchecked((int)0xF0000000), Int32.RotateLeft(unchecked((int)0xF0000000), 32));
        }

        /**
         * @tests java.lang.Integer#rotateRight(int,int)
         */
        [Test]
        public void Test_rotateRightII()
        {
            assertEquals(0xF, Int32.RotateRight(0xF0, 4));
            assertEquals(0xF, Int32.RotateRight(0xF00, 8));
            assertEquals(0xF, Int32.RotateRight(0xF000, 12));
            assertEquals(0xF, Int32.RotateRight(0xF0000, 16));
            assertEquals(0xF, Int32.RotateRight(0xF00000, 20));
            assertEquals(0xF, Int32.RotateRight(0xF000000, 24));
            assertEquals(0xF, Int32.RotateRight(unchecked((int)0xF0000000), 28));
            assertEquals(unchecked((int)0xF0000000), Int32.RotateRight(unchecked((int)0xF0000000), 32));
            assertEquals(unchecked((int)0xF0000000), Int32.RotateRight(unchecked((int)0xF0000000), 0));

        }

        /**
         * @tests java.lang.Integer#reverseBytes(int)
         */
        [Test]
        public void Test_reverseBytesI()
        {
            assertEquals(unchecked((int)0xAABBCCDD), Int32.ReverseBytes(unchecked((int)0xDDCCBBAA)));
            assertEquals(0x11223344, Int32.ReverseBytes(0x44332211));
            assertEquals(0x00112233, Int32.ReverseBytes(0x33221100));
            assertEquals(0x20000002, Int32.ReverseBytes(0x02000020));
        }

        /**
         * @tests java.lang.Integer#reverse(int)
         */
        [Test]
        public void Test_reverseI()
        {
            assertEquals(-1, Int32.Reverse(-1));
            assertEquals(unchecked((int)0x80000000), Int32.Reverse(1));
        }

        /**
         * @tests java.lang.Integer#signum(int)
         */
        [Test]
        public void Test_signumI()
        {
            for (int i = -128; i < 0; i++)
            {
                assertEquals(-1, Int32.Signum(i));
            }
            assertEquals(0, Int32.Signum(0));
            for (int i = 1; i <= 127; i++)
            {
                assertEquals(1, Int32.Signum(i));
            }
        }

        public class JDK8 : TestCase
        {
            [TestCase(+100, "+100")]
            [TestCase(-100, "-100")]

            [TestCase(0, "+0")]
            [TestCase(0, "-0")]
            [TestCase(0, "+00000")]
            [TestCase(0, "-00000")]

            [TestCase(0, "0")]
            [TestCase(1, "1")]
            [TestCase(9, "9")]

            public void TestParse_String_Int32(int expected, string value)
            {
                var actual = Int32.Parse(value, NumberFormatInfo.InvariantInfo);
                assertEquals($"Int32.Parse(string, IFormatProvider) failed. String: \"{value}\" Result: {actual}", expected, actual);
            }

            [TestCase(typeof(FormatException), "")]
            [TestCase(typeof(FormatException), "\u0000")]
            [TestCase(typeof(FormatException), "\u002f")]
            [TestCase(typeof(FormatException), "+")]
            [TestCase(typeof(FormatException), "-")]
            [TestCase(typeof(FormatException), "++")]
            [TestCase(typeof(FormatException), "+-")]
            [TestCase(typeof(FormatException), "-+")]
            [TestCase(typeof(FormatException), "--")]
            [TestCase(typeof(FormatException), "++100")]
            [TestCase(typeof(FormatException), "--100")]
            [TestCase(typeof(FormatException), "+-6")]
            [TestCase(typeof(FormatException), "-+6")]
            [TestCase(typeof(FormatException), "*100")]
            public void TestParse_String_Int32_ForException(Type expectedExceptionType, string value)
            {
                Assert.Throws(expectedExceptionType, () => Int32.Parse(value, NumberFormatInfo.InvariantInfo));
            }

            [TestCase(0, "test-00000", 4, 10 - 4, 10)]
            [TestCase(-12345, "test-12345", 4, 10 - 4, 10)]
            [TestCase(12345, "xx12345yy", 2, 7 - 2, 10)]
            [TestCase(15, "xxFyy", 2, 3 - 2, 16)]

            [TestCase(12345, "xx1234567yy", 2, 5, 10)]

            public void TestParse_String_Int32_Int32_Int32(int expected, string value, int startIndex, int length, int radix)
            {
                var actual = Int32.Parse(value, startIndex, length, radix);
                assertEquals($"Int32.Parse(string, int, int, int) failed. Expected: {expected} String: \"{value}\", startIndex: {startIndex}, length: {length} radix: {radix} Result: {actual}", expected, actual);
            }

            [TestCase(typeof(FormatException), "", 0, 0 - 0, 10)] // J2N: .NET throws ArgumentOutOfRangeException rather than FormatException in this case, but since it is inconsistent with int.Parse() we are going with FormatException.
            [TestCase(typeof(FormatException), "+-6", 0, 3 - 0, 10)]
            [TestCase(typeof(FormatException), "1000000", 7, 7 - 7, 10)] // J2N: .NET throws ArgumentOutOfRangeException rather than FormatException in this case, but since it is inconsistent with int.Parse() we are going with FormatException.
            [TestCase(typeof(ArgumentOutOfRangeException), "1000000", 0, 2 - 0, Character.MaxRadix + 1)]
            [TestCase(typeof(ArgumentOutOfRangeException), "1000000", 0, 2 - 0, Character.MinRadix - 1)]

            [TestCase(typeof(ArgumentOutOfRangeException), "", 1, 1 - 1, 10)]
            [TestCase(typeof(ArgumentOutOfRangeException), "1000000", 10, 4 - 10, 10)]
            [TestCase(typeof(ArgumentOutOfRangeException), "1000000", 10, 2 - 10, Character.MaxRadix + 1)]
            [TestCase(typeof(ArgumentOutOfRangeException), "1000000", 10, 2 - 10, Character.MinRadix - 1)]
            [TestCase(typeof(ArgumentOutOfRangeException), "1000000", -1, 2 - -1, Character.MaxRadix + 1)]
            [TestCase(typeof(ArgumentOutOfRangeException), "1000000", -1, 2 - -1, Character.MinRadix - 1)]
            [TestCase(typeof(ArgumentOutOfRangeException), "-1", 0, 3 - 0, 10)]
            [TestCase(typeof(ArgumentOutOfRangeException), "-1", 2, 3 - 2, 10)]
            [TestCase(typeof(ArgumentOutOfRangeException), "-1", -1, 2 - -1, 10)]

            [TestCase(typeof(ArgumentNullException), null, 0, 1 - 0, 10)]
            [TestCase(typeof(ArgumentNullException), null, -1, 0 - -1, 100)]
            [TestCase(typeof(ArgumentNullException), null, 0, 0 - 0, 10)]
            [TestCase(typeof(ArgumentNullException), null, 0, -1 - 0, 10)]
            [TestCase(typeof(ArgumentNullException), null, -1, -1 - -1, -1)]

            [TestCase(typeof(FormatException), "xx  34567yy", 2, 5, 10)] // spaces in range are not allowed
            public void TestParse_String_Int32_Int32_Int32_ForException(Type expectedExceptionType, string value, int startIndex, int length, int radix)
            {
                Assert.Throws(expectedExceptionType, () => Int32.Parse(value, startIndex, length, radix));
            }


            public static IEnumerable<TestCaseData> UnsignedIntTestCases
            {
                get
                {
                    yield return new TestCaseData((uint)0, 2, "0", 0);
                    yield return new TestCaseData((uint)0, 3, "0", 0);
                    yield return new TestCaseData((uint)0, 4, "0", 0);
                    yield return new TestCaseData((uint)0, 5, "0", 0);
                    yield return new TestCaseData((uint)0, 6, "0", 0);
                    yield return new TestCaseData((uint)0, 7, "0", 0);
                    yield return new TestCaseData((uint)0, 8, "0", 0);
                    yield return new TestCaseData((uint)0, 9, "0", 0);
                    yield return new TestCaseData((uint)0, 10, "0", 0);
                    yield return new TestCaseData((uint)0, 11, "0", 0);
                    yield return new TestCaseData((uint)0, 12, "0", 0);
                    yield return new TestCaseData((uint)0, 13, "0", 0);
                    yield return new TestCaseData((uint)0, 14, "0", 0);
                    yield return new TestCaseData((uint)0, 15, "0", 0);
                    yield return new TestCaseData((uint)0, 16, "0", 0);
                    yield return new TestCaseData((uint)0, 17, "0", 0);
                    yield return new TestCaseData((uint)0, 18, "0", 0);
                    yield return new TestCaseData((uint)0, 19, "0", 0);
                    yield return new TestCaseData((uint)0, 20, "0", 0);
                    yield return new TestCaseData((uint)0, 21, "0", 0);
                    yield return new TestCaseData((uint)0, 22, "0", 0);
                    yield return new TestCaseData((uint)0, 23, "0", 0);
                    yield return new TestCaseData((uint)0, 24, "0", 0);
                    yield return new TestCaseData((uint)0, 25, "0", 0);
                    yield return new TestCaseData((uint)0, 26, "0", 0);
                    yield return new TestCaseData((uint)0, 27, "0", 0);
                    yield return new TestCaseData((uint)0, 28, "0", 0);
                    yield return new TestCaseData((uint)0, 29, "0", 0);
                    yield return new TestCaseData((uint)0, 30, "0", 0);
                    yield return new TestCaseData((uint)0, 31, "0", 0);
                    yield return new TestCaseData((uint)0, 32, "0", 0);
                    yield return new TestCaseData((uint)0, 33, "0", 0);
                    yield return new TestCaseData((uint)0, 34, "0", 0);
                    yield return new TestCaseData((uint)0, 35, "0", 0);
                    yield return new TestCaseData((uint)0, 36, "0", 0);

                    yield return new TestCaseData((uint)1, 2, "1", 1);
                    yield return new TestCaseData((uint)1, 3, "1", 1);
                    yield return new TestCaseData((uint)1, 4, "1", 1);
                    yield return new TestCaseData((uint)1, 5, "1", 1);
                    yield return new TestCaseData((uint)1, 6, "1", 1);
                    yield return new TestCaseData((uint)1, 7, "1", 1);
                    yield return new TestCaseData((uint)1, 8, "1", 1);
                    yield return new TestCaseData((uint)1, 9, "1", 1);
                    yield return new TestCaseData((uint)1, 10, "1", 1);
                    yield return new TestCaseData((uint)1, 11, "1", 1);
                    yield return new TestCaseData((uint)1, 12, "1", 1);
                    yield return new TestCaseData((uint)1, 13, "1", 1);
                    yield return new TestCaseData((uint)1, 14, "1", 1);
                    yield return new TestCaseData((uint)1, 15, "1", 1);
                    yield return new TestCaseData((uint)1, 16, "1", 1);
                    yield return new TestCaseData((uint)1, 17, "1", 1);
                    yield return new TestCaseData((uint)1, 18, "1", 1);
                    yield return new TestCaseData((uint)1, 19, "1", 1);
                    yield return new TestCaseData((uint)1, 20, "1", 1);
                    yield return new TestCaseData((uint)1, 21, "1", 1);
                    yield return new TestCaseData((uint)1, 22, "1", 1);
                    yield return new TestCaseData((uint)1, 23, "1", 1);
                    yield return new TestCaseData((uint)1, 24, "1", 1);
                    yield return new TestCaseData((uint)1, 25, "1", 1);
                    yield return new TestCaseData((uint)1, 26, "1", 1);
                    yield return new TestCaseData((uint)1, 27, "1", 1);
                    yield return new TestCaseData((uint)1, 28, "1", 1);
                    yield return new TestCaseData((uint)1, 29, "1", 1);
                    yield return new TestCaseData((uint)1, 30, "1", 1);
                    yield return new TestCaseData((uint)1, 31, "1", 1);
                    yield return new TestCaseData((uint)1, 32, "1", 1);
                    yield return new TestCaseData((uint)1, 33, "1", 1);
                    yield return new TestCaseData((uint)1, 34, "1", 1);
                    yield return new TestCaseData((uint)1, 35, "1", 1);
                    yield return new TestCaseData((uint)1, 36, "1", 1);

                    yield return new TestCaseData((uint)10, 2, "1010", 10);
                    yield return new TestCaseData((uint)10, 3, "101", 10);
                    yield return new TestCaseData((uint)10, 4, "22", 10);
                    yield return new TestCaseData((uint)10, 5, "20", 10);
                    yield return new TestCaseData((uint)10, 6, "14", 10);
                    yield return new TestCaseData((uint)10, 7, "13", 10);
                    yield return new TestCaseData((uint)10, 8, "12", 10);
                    yield return new TestCaseData((uint)10, 9, "11", 10);
                    yield return new TestCaseData((uint)10, 10, "10", 10);
                    yield return new TestCaseData((uint)10, 11, "a", 10);
                    yield return new TestCaseData((uint)10, 12, "a", 10);
                    yield return new TestCaseData((uint)10, 13, "a", 10);
                    yield return new TestCaseData((uint)10, 14, "a", 10);
                    yield return new TestCaseData((uint)10, 15, "a", 10);
                    yield return new TestCaseData((uint)10, 16, "a", 10);
                    yield return new TestCaseData((uint)10, 17, "a", 10);
                    yield return new TestCaseData((uint)10, 18, "a", 10);
                    yield return new TestCaseData((uint)10, 19, "a", 10);
                    yield return new TestCaseData((uint)10, 20, "a", 10);
                    yield return new TestCaseData((uint)10, 21, "a", 10);
                    yield return new TestCaseData((uint)10, 22, "a", 10);
                    yield return new TestCaseData((uint)10, 23, "a", 10);
                    yield return new TestCaseData((uint)10, 24, "a", 10);
                    yield return new TestCaseData((uint)10, 25, "a", 10);
                    yield return new TestCaseData((uint)10, 26, "a", 10);
                    yield return new TestCaseData((uint)10, 27, "a", 10);
                    yield return new TestCaseData((uint)10, 28, "a", 10);
                    yield return new TestCaseData((uint)10, 29, "a", 10);
                    yield return new TestCaseData((uint)10, 30, "a", 10);
                    yield return new TestCaseData((uint)10, 31, "a", 10);
                    yield return new TestCaseData((uint)10, 32, "a", 10);
                    yield return new TestCaseData((uint)10, 33, "a", 10);
                    yield return new TestCaseData((uint)10, 34, "a", 10);
                    yield return new TestCaseData((uint)10, 35, "a", 10);
                    yield return new TestCaseData((uint)10, 36, "a", 10);

                    yield return new TestCaseData((uint)int.MaxValue - 1, 2, "1111111111111111111111111111110", 2147483646);
                    yield return new TestCaseData((uint)int.MaxValue - 1, 3, "12112122212110202100", 2147483646);
                    yield return new TestCaseData((uint)int.MaxValue - 1, 4, "1333333333333332", 2147483646);
                    yield return new TestCaseData((uint)int.MaxValue - 1, 5, "13344223434041", 2147483646);
                    yield return new TestCaseData((uint)int.MaxValue - 1, 6, "553032005530", 2147483646);
                    yield return new TestCaseData((uint)int.MaxValue - 1, 7, "104134211160", 2147483646);
                    yield return new TestCaseData((uint)int.MaxValue - 1, 8, "17777777776", 2147483646);
                    yield return new TestCaseData((uint)int.MaxValue - 1, 9, "5478773670", 2147483646);
                    yield return new TestCaseData((uint)int.MaxValue - 1, 10, "2147483646", 2147483646);
                    yield return new TestCaseData((uint)int.MaxValue - 1, 11, "a02220280", 2147483646);
                    yield return new TestCaseData((uint)int.MaxValue - 1, 12, "4bb2308a6", 2147483646);
                    yield return new TestCaseData((uint)int.MaxValue - 1, 13, "282ba4aa9", 2147483646);
                    yield return new TestCaseData((uint)int.MaxValue - 1, 14, "1652ca930", 2147483646);
                    yield return new TestCaseData((uint)int.MaxValue - 1, 15, "c87e66b6", 2147483646);
                    yield return new TestCaseData((uint)int.MaxValue - 1, 16, "7ffffffe", 2147483646);
                    yield return new TestCaseData((uint)int.MaxValue - 1, 17, "53g7f547", 2147483646);
                    yield return new TestCaseData((uint)int.MaxValue - 1, 18, "3928g3h0", 2147483646);
                    yield return new TestCaseData((uint)int.MaxValue - 1, 19, "27c57h31", 2147483646);
                    yield return new TestCaseData((uint)int.MaxValue - 1, 20, "1db1f926", 2147483646);
                    yield return new TestCaseData((uint)int.MaxValue - 1, 21, "140h2d90", 2147483646);
                    yield return new TestCaseData((uint)int.MaxValue - 1, 22, "ikf5bf0", 2147483646);
                    yield return new TestCaseData((uint)int.MaxValue - 1, 23, "ebelf94", 2147483646);
                    yield return new TestCaseData((uint)int.MaxValue - 1, 24, "b5gge56", 2147483646);
                    yield return new TestCaseData((uint)int.MaxValue - 1, 25, "8jmdnkl", 2147483646);
                    yield return new TestCaseData((uint)int.MaxValue - 1, 26, "6oj8iom", 2147483646);
                    yield return new TestCaseData((uint)int.MaxValue - 1, 27, "5ehnck9", 2147483646);
                    yield return new TestCaseData((uint)int.MaxValue - 1, 28, "4clm98e", 2147483646);
                    yield return new TestCaseData((uint)int.MaxValue - 1, 29, "3hk7986", 2147483646);
                    yield return new TestCaseData((uint)int.MaxValue - 1, 30, "2sb6cs6", 2147483646);
                    yield return new TestCaseData((uint)int.MaxValue - 1, 31, "2d09uc0", 2147483646);
                    yield return new TestCaseData((uint)int.MaxValue - 1, 32, "1vvvvvu", 2147483646);
                    yield return new TestCaseData((uint)int.MaxValue - 1, 33, "1lsqtl0", 2147483646);
                    yield return new TestCaseData((uint)int.MaxValue - 1, 34, "1d8xqro", 2147483646);
                    yield return new TestCaseData((uint)int.MaxValue - 1, 35, "15v22ul", 2147483646);
                    yield return new TestCaseData((uint)int.MaxValue - 1, 36, "zik0zi", 2147483646);

                    yield return new TestCaseData((uint)int.MaxValue, 2, "1111111111111111111111111111111", 2147483647);
                    yield return new TestCaseData((uint)int.MaxValue, 3, "12112122212110202101", 2147483647);
                    yield return new TestCaseData((uint)int.MaxValue, 4, "1333333333333333", 2147483647);
                    yield return new TestCaseData((uint)int.MaxValue, 5, "13344223434042", 2147483647);
                    yield return new TestCaseData((uint)int.MaxValue, 6, "553032005531", 2147483647);
                    yield return new TestCaseData((uint)int.MaxValue, 7, "104134211161", 2147483647);
                    yield return new TestCaseData((uint)int.MaxValue, 8, "17777777777", 2147483647);
                    yield return new TestCaseData((uint)int.MaxValue, 9, "5478773671", 2147483647);
                    yield return new TestCaseData((uint)int.MaxValue, 10, "2147483647", 2147483647);
                    yield return new TestCaseData((uint)int.MaxValue, 11, "a02220281", 2147483647);
                    yield return new TestCaseData((uint)int.MaxValue, 12, "4bb2308a7", 2147483647);
                    yield return new TestCaseData((uint)int.MaxValue, 13, "282ba4aaa", 2147483647);
                    yield return new TestCaseData((uint)int.MaxValue, 14, "1652ca931", 2147483647);
                    yield return new TestCaseData((uint)int.MaxValue, 15, "c87e66b7", 2147483647);
                    yield return new TestCaseData((uint)int.MaxValue, 16, "7fffffff", 2147483647);
                    yield return new TestCaseData((uint)int.MaxValue, 17, "53g7f548", 2147483647);
                    yield return new TestCaseData((uint)int.MaxValue, 18, "3928g3h1", 2147483647);
                    yield return new TestCaseData((uint)int.MaxValue, 19, "27c57h32", 2147483647);
                    yield return new TestCaseData((uint)int.MaxValue, 20, "1db1f927", 2147483647);
                    yield return new TestCaseData((uint)int.MaxValue, 21, "140h2d91", 2147483647);
                    yield return new TestCaseData((uint)int.MaxValue, 22, "ikf5bf1", 2147483647);
                    yield return new TestCaseData((uint)int.MaxValue, 23, "ebelf95", 2147483647);
                    yield return new TestCaseData((uint)int.MaxValue, 24, "b5gge57", 2147483647);
                    yield return new TestCaseData((uint)int.MaxValue, 25, "8jmdnkm", 2147483647);
                    yield return new TestCaseData((uint)int.MaxValue, 26, "6oj8ion", 2147483647);
                    yield return new TestCaseData((uint)int.MaxValue, 27, "5ehncka", 2147483647);
                    yield return new TestCaseData((uint)int.MaxValue, 28, "4clm98f", 2147483647);
                    yield return new TestCaseData((uint)int.MaxValue, 29, "3hk7987", 2147483647);
                    yield return new TestCaseData((uint)int.MaxValue, 30, "2sb6cs7", 2147483647);
                    yield return new TestCaseData((uint)int.MaxValue, 31, "2d09uc1", 2147483647);
                    yield return new TestCaseData((uint)int.MaxValue, 32, "1vvvvvv", 2147483647);
                    yield return new TestCaseData((uint)int.MaxValue, 33, "1lsqtl1", 2147483647);
                    yield return new TestCaseData((uint)int.MaxValue, 34, "1d8xqrp", 2147483647);
                    yield return new TestCaseData((uint)int.MaxValue, 35, "15v22um", 2147483647);
                    yield return new TestCaseData((uint)int.MaxValue, 36, "zik0zj", 2147483647);

                    yield return new TestCaseData((uint)int.MaxValue + 1, 2, "10000000000000000000000000000000", -2147483648);
                    yield return new TestCaseData((uint)int.MaxValue + 1, 3, "12112122212110202102", -2147483648);
                    yield return new TestCaseData((uint)int.MaxValue + 1, 4, "2000000000000000", -2147483648);
                    yield return new TestCaseData((uint)int.MaxValue + 1, 5, "13344223434043", -2147483648);
                    yield return new TestCaseData((uint)int.MaxValue + 1, 6, "553032005532", -2147483648);
                    yield return new TestCaseData((uint)int.MaxValue + 1, 7, "104134211162", -2147483648);
                    yield return new TestCaseData((uint)int.MaxValue + 1, 8, "20000000000", -2147483648);
                    yield return new TestCaseData((uint)int.MaxValue + 1, 9, "5478773672", -2147483648);
                    yield return new TestCaseData((uint)int.MaxValue + 1, 10, "2147483648", -2147483648);
                    yield return new TestCaseData((uint)int.MaxValue + 1, 11, "a02220282", -2147483648);
                    yield return new TestCaseData((uint)int.MaxValue + 1, 12, "4bb2308a8", -2147483648);
                    yield return new TestCaseData((uint)int.MaxValue + 1, 13, "282ba4aab", -2147483648);
                    yield return new TestCaseData((uint)int.MaxValue + 1, 14, "1652ca932", -2147483648);
                    yield return new TestCaseData((uint)int.MaxValue + 1, 15, "c87e66b8", -2147483648);
                    yield return new TestCaseData((uint)int.MaxValue + 1, 16, "80000000", -2147483648);
                    yield return new TestCaseData((uint)int.MaxValue + 1, 17, "53g7f549", -2147483648);
                    yield return new TestCaseData((uint)int.MaxValue + 1, 18, "3928g3h2", -2147483648);
                    yield return new TestCaseData((uint)int.MaxValue + 1, 19, "27c57h33", -2147483648);
                    yield return new TestCaseData((uint)int.MaxValue + 1, 20, "1db1f928", -2147483648);
                    yield return new TestCaseData((uint)int.MaxValue + 1, 21, "140h2d92", -2147483648);
                    yield return new TestCaseData((uint)int.MaxValue + 1, 22, "ikf5bf2", -2147483648);
                    yield return new TestCaseData((uint)int.MaxValue + 1, 23, "ebelf96", -2147483648);
                    yield return new TestCaseData((uint)int.MaxValue + 1, 24, "b5gge58", -2147483648);
                    yield return new TestCaseData((uint)int.MaxValue + 1, 25, "8jmdnkn", -2147483648);
                    yield return new TestCaseData((uint)int.MaxValue + 1, 26, "6oj8ioo", -2147483648);
                    yield return new TestCaseData((uint)int.MaxValue + 1, 27, "5ehnckb", -2147483648);
                    yield return new TestCaseData((uint)int.MaxValue + 1, 28, "4clm98g", -2147483648);
                    yield return new TestCaseData((uint)int.MaxValue + 1, 29, "3hk7988", -2147483648);
                    yield return new TestCaseData((uint)int.MaxValue + 1, 30, "2sb6cs8", -2147483648);
                    yield return new TestCaseData((uint)int.MaxValue + 1, 31, "2d09uc2", -2147483648);
                    yield return new TestCaseData((uint)int.MaxValue + 1, 32, "2000000", -2147483648);
                    yield return new TestCaseData((uint)int.MaxValue + 1, 33, "1lsqtl2", -2147483648);
                    yield return new TestCaseData((uint)int.MaxValue + 1, 34, "1d8xqrq", -2147483648);
                    yield return new TestCaseData((uint)int.MaxValue + 1, 35, "15v22un", -2147483648);
                    yield return new TestCaseData((uint)int.MaxValue + 1, 36, "zik0zk", -2147483648);

                    yield return new TestCaseData(uint.MaxValue - 1, 2, "11111111111111111111111111111110", -2);
                    yield return new TestCaseData(uint.MaxValue - 1, 3, "102002022201221111202", -2);
                    yield return new TestCaseData(uint.MaxValue - 1, 4, "3333333333333332", -2);
                    yield return new TestCaseData(uint.MaxValue - 1, 5, "32244002423134", -2);
                    yield return new TestCaseData(uint.MaxValue - 1, 6, "1550104015502", -2);
                    yield return new TestCaseData(uint.MaxValue - 1, 7, "211301422352", -2);
                    yield return new TestCaseData(uint.MaxValue - 1, 8, "37777777776", -2);
                    yield return new TestCaseData(uint.MaxValue - 1, 9, "12068657452", -2);
                    yield return new TestCaseData(uint.MaxValue - 1, 10, "4294967294", -2);
                    yield return new TestCaseData(uint.MaxValue - 1, 11, "1904440552", -2);
                    yield return new TestCaseData(uint.MaxValue - 1, 12, "9ba461592", -2);
                    yield return new TestCaseData(uint.MaxValue - 1, 13, "535a79887", -2);
                    yield return new TestCaseData(uint.MaxValue - 1, 14, "2ca5b7462", -2);
                    yield return new TestCaseData(uint.MaxValue - 1, 15, "1a20dcd7e", -2);
                    yield return new TestCaseData(uint.MaxValue - 1, 16, "fffffffe", -2);
                    yield return new TestCaseData(uint.MaxValue - 1, 17, "a7ffda8g", -2);
                    yield return new TestCaseData(uint.MaxValue - 1, 18, "704he7g2", -2);
                    yield return new TestCaseData(uint.MaxValue - 1, 19, "4f5aff64", -2);
                    yield return new TestCaseData(uint.MaxValue - 1, 20, "3723ai4e", -2);
                    yield return new TestCaseData(uint.MaxValue - 1, 21, "281d55i2", -2);
                    yield return new TestCaseData(uint.MaxValue - 1, 22, "1fj8b182", -2);
                    yield return new TestCaseData(uint.MaxValue - 1, 23, "1606k7ia", -2);
                    yield return new TestCaseData(uint.MaxValue - 1, 24, "mb994ae", -2);
                    yield return new TestCaseData(uint.MaxValue - 1, 25, "hek2mgj", -2);
                    yield return new TestCaseData(uint.MaxValue - 1, 26, "dnchbnk", -2);
                    yield return new TestCaseData(uint.MaxValue - 1, 27, "b28jpdk", -2);
                    yield return new TestCaseData(uint.MaxValue - 1, 28, "8pfgih2", -2);
                    yield return new TestCaseData(uint.MaxValue - 1, 29, "76beige", -2);
                    yield return new TestCaseData(uint.MaxValue - 1, 30, "5qmcpqe", -2);
                    yield return new TestCaseData(uint.MaxValue - 1, 31, "4q0jto2", -2);
                    yield return new TestCaseData(uint.MaxValue - 1, 32, "3vvvvvu", -2);
                    yield return new TestCaseData(uint.MaxValue - 1, 33, "3aokq92", -2);
                    yield return new TestCaseData(uint.MaxValue - 1, 34, "2qhxjlg", -2);
                    yield return new TestCaseData(uint.MaxValue - 1, 35, "2br45q9", -2);
                    yield return new TestCaseData(uint.MaxValue - 1, 36, "1z141z2", -2);

                    yield return new TestCaseData(uint.MaxValue, 2, "11111111111111111111111111111111", -1);
                    yield return new TestCaseData(uint.MaxValue, 3, "102002022201221111210", -1);
                    yield return new TestCaseData(uint.MaxValue, 4, "3333333333333333", -1);
                    yield return new TestCaseData(uint.MaxValue, 5, "32244002423140", -1);
                    yield return new TestCaseData(uint.MaxValue, 6, "1550104015503", -1);
                    yield return new TestCaseData(uint.MaxValue, 7, "211301422353", -1);
                    yield return new TestCaseData(uint.MaxValue, 8, "37777777777", -1);
                    yield return new TestCaseData(uint.MaxValue, 9, "12068657453", -1);
                    yield return new TestCaseData(uint.MaxValue, 10, "4294967295", -1);
                    yield return new TestCaseData(uint.MaxValue, 11, "1904440553", -1);
                    yield return new TestCaseData(uint.MaxValue, 12, "9ba461593", -1);
                    yield return new TestCaseData(uint.MaxValue, 13, "535a79888", -1);
                    yield return new TestCaseData(uint.MaxValue, 14, "2ca5b7463", -1);
                    yield return new TestCaseData(uint.MaxValue, 15, "1a20dcd80", -1);
                    yield return new TestCaseData(uint.MaxValue, 16, "ffffffff", -1);
                    yield return new TestCaseData(uint.MaxValue, 17, "a7ffda90", -1);
                    yield return new TestCaseData(uint.MaxValue, 18, "704he7g3", -1);
                    yield return new TestCaseData(uint.MaxValue, 19, "4f5aff65", -1);
                    yield return new TestCaseData(uint.MaxValue, 20, "3723ai4f", -1);
                    yield return new TestCaseData(uint.MaxValue, 21, "281d55i3", -1);
                    yield return new TestCaseData(uint.MaxValue, 22, "1fj8b183", -1);
                    yield return new TestCaseData(uint.MaxValue, 23, "1606k7ib", -1);
                    yield return new TestCaseData(uint.MaxValue, 24, "mb994af", -1);
                    yield return new TestCaseData(uint.MaxValue, 25, "hek2mgk", -1);
                    yield return new TestCaseData(uint.MaxValue, 26, "dnchbnl", -1);
                    yield return new TestCaseData(uint.MaxValue, 27, "b28jpdl", -1);
                    yield return new TestCaseData(uint.MaxValue, 28, "8pfgih3", -1);
                    yield return new TestCaseData(uint.MaxValue, 29, "76beigf", -1);
                    yield return new TestCaseData(uint.MaxValue, 30, "5qmcpqf", -1);
                    yield return new TestCaseData(uint.MaxValue, 31, "4q0jto3", -1);
                    yield return new TestCaseData(uint.MaxValue, 32, "3vvvvvv", -1);
                    yield return new TestCaseData(uint.MaxValue, 33, "3aokq93", -1);
                    yield return new TestCaseData(uint.MaxValue, 34, "2qhxjlh", -1);
                    yield return new TestCaseData(uint.MaxValue, 35, "2br45qa", -1);
                    yield return new TestCaseData(uint.MaxValue, 36, "1z141z3", -1);
                }
            }

            [Test]
            [TestCaseSource("UnsignedIntTestCases")]
            public void TestParseUnsigned_String_Int32(uint value, int radix, string bigString, int expected)
            {
                int actual = Int32.ParseUnsigned(bigString, radix);
                assertEquals(expected, actual);
            }


            [Test]
            [TestCaseSource("UnsignedIntTestCases")]
            public void TestParseUnsigned_String_Int32_Int32_Int32(uint value, int radix, string bigString, int expected)
            {
                int actual = Int32.ParseUnsigned("prefix" + bigString + "suffix", "prefix".Length, bigString.Length, radix);
                assertEquals(expected, actual);
            }

            //[Test]
            //[TestCase(typeof(ArgumentNullException), null, 10)] // J2N: We mimic the .NET behavior by allowing null to be converted to 0
            [TestCase(typeof(FormatException), "", 10)]
            [TestCase(typeof(OverflowException), "-1", 10)]
            [TestCase(typeof(OverflowException), "9223372036854775807", 10)] // long.MaxValue
            [TestCase(typeof(OverflowException), "4294967296", 10)] // uint.MaxValue + 1
            public void TestParseUnsigned_String_Int32_ForException(Type expectedExceptionType, string value, int radix)
            {
                Assert.Throws(expectedExceptionType, () => Int32.ParseUnsigned(value, radix));
            }


            //[Test]
            [TestCase(typeof(FormatException), null, 10)] // Expected because we are concatenating a string. In .NET concatenating null to a string is the same as empty string.
            [TestCase(typeof(FormatException), "", 10)]
            [TestCase(typeof(OverflowException), "-1", 10)]
            [TestCase(typeof(OverflowException), "9223372036854775807", 10)] // long.MaxValue
            [TestCase(typeof(OverflowException), "4294967296", 10)] // uint.MaxValue + 1
            public void TestParseUnsigned_String_Int32_Int32_Int32_ForException(Type expectedExceptionType, string value, int radix)
            {
                Assert.Throws(expectedExceptionType, () => Int32.ParseUnsigned("prefix" + value + "suffix", "prefix".Length, value?.Length ?? 0, radix));
            }
        }
    }
}
