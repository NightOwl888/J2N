using J2N.Globalization;
using J2N.Text;
using NUnit.Framework;
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
                    .ToByte());
            assertEquals("Returned incorrect byte value", 127, new Int32(127)
                    .ToByte());
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
         * @tests java.lang.Integer#compareTo(Object)
         */
        [Test]
        public void Test_compareTo_Object()
        {
            // Test for method int java.lang.Integer.compareTo(java.lang.Integer)
            assertTrue("-2 compared to 1 gave non-negative answer", new Int32(-2)
                    .CompareTo((object)new Int32(1)) < 0);
            assertEquals("-2 compared to -2 gave non-zero answer", 0, new Int32(-2)
                    .CompareTo((object)new Int32(-2)));
            assertTrue("3 compared to 2 gave non-positive answer", new Int32(3)
                    .CompareTo((object)new Int32(2)) > 0);

            //try
            //{
            //    new Int32(0).CompareTo(null);
            //    fail("No NPE");
            //}
            //catch (NullPointerException e)
            //{
            //}

            // J2N: Return 1 when comparing to null to match other .NET classes
            assertEquals(1, new Int32(0).CompareTo((object)null));

            // J2N: Check to ensure exception is thrown when there is a type mismatch
            Assert.Throws<ArgumentException>(() => new Int32(0).CompareTo((object)4));
        }

        // J2N: Moved to CharSequences
        ///**
        // * @tests java.lang.Integer#decode(java.lang.String)
        // */
        //[Test]
        //public void Test_decodeLjava_lang_String2()
        //{
        //    // Test for method java.lang.Integer
        //    // java.lang.Integer.decode(java.lang.String)
        //    assertEquals("Failed for 132233",
        //            132233, Int32.Decode("132233").GetInt32Value());
        //    assertEquals("Failed for 07654321",
        //            /*07654321*/ 2054353, Int32.Decode("07654321").GetInt32Value()); // J2N: Octal literals are not supported in C#, changed to decimal representation
        //    assertTrue("Failed for #1234567",
        //            Int32.Decode("#1234567").GetInt32Value() == 0x1234567);
        //    assertTrue("Failed for 0xdAd",
        //            Int32.Decode("0xdAd").GetInt32Value() == 0xdad);
        //    assertEquals("Failed for -23", -23, Int32.Decode("-23").GetInt32Value());
        //    assertEquals("Returned incorrect value for 0 decimal", 0, Int32
        //            .Decode("0").GetInt32Value());
        //    assertEquals("Returned incorrect value for 0 hex", 0, Int32.Decode("0x0")
        //            .GetInt32Value());
        //    assertTrue("Returned incorrect value for most negative value decimal",
        //            Int32.Decode("-2147483648").GetInt32Value() == unchecked((int)0x80000000));
        //    assertTrue("Returned incorrect value for most negative value hex",
        //            Int32.Decode("-0x80000000").GetInt32Value() == unchecked((int)0x80000000));
        //    assertTrue("Returned incorrect value for most positive value decimal",
        //            Int32.Decode("2147483647").GetInt32Value() == 0x7fffffff);
        //    assertTrue("Returned incorrect value for most positive value hex",
        //            Int32.Decode("0x7fffffff").GetInt32Value() == 0x7fffffff);

        //    bool exception = false;
        //    try
        //    {
        //        Int32.Decode("0a");
        //    }
        //    catch (FormatException e)
        //    {
        //        // correct
        //        exception = true;
        //    }
        //    assertTrue("Failed to throw FormatException for \"Oa\"",
        //            exception);

        //    exception = false;
        //    try
        //    {
        //        Int32.Decode("2147483648");
        //    }
        //    catch (OverflowException e) // J2N: .NET throws OverflowException rather than FormatException in this case
        //    {
        //        // Correct
        //        exception = true;
        //    }
        //    assertTrue("Failed to throw exception for MAX_VALUE + 1", exception);

        //    exception = false;
        //    try
        //    {
        //        Int32.Decode("-2147483649");
        //    }
        //    catch (OverflowException e) // J2N: .NET throws OverflowException rather than FormatException in this case
        //    {
        //        // Correct
        //        exception = true;
        //    }
        //    assertTrue("Failed to throw exception for MIN_VALUE - 1", exception);

        //    exception = false;
        //    try
        //    {
        //        Int32.Decode("0x80000000");
        //    }
        //    catch (OverflowException e) // J2N: .NET throws OverflowException rather than FormatException in this case
        //    {
        //        // Correct
        //        exception = true;
        //    }
        //    assertTrue("Failed to throw exception for hex MAX_VALUE + 1", exception);

        //    exception = false;
        //    try
        //    {
        //        Int32.Decode("-0x80000001");
        //    }
        //    catch (OverflowException e) // J2N: .NET throws OverflowException rather than FormatException in this case
        //    {
        //        // Correct
        //        exception = true;
        //    }
        //    assertTrue("Failed to throw exception for hex MIN_VALUE - 1", exception);

        //    exception = false;
        //    try
        //    {
        //        Int32.Decode("9999999999");
        //    }
        //    catch (OverflowException e) // J2N: .NET throws OverflowException rather than FormatException in this case
        //    {
        //        // Correct
        //        exception = true;
        //    }
        //    assertTrue("Failed to throw exception for 9999999999", exception);

        //    try
        //    {
        //        Int32.Decode("-");
        //        fail("Expected exception for -");
        //    }
        //    catch (FormatException e)
        //    {
        //        // Expected
        //    }

        //    try
        //    {
        //        Int32.Decode("0x");
        //        fail("Expected exception for 0x");
        //    }
        //    catch (FormatException e)
        //    {
        //        // Expected
        //    }

        //    try
        //    {
        //        Int32.Decode("#");
        //        fail("Expected exception for #");
        //    }
        //    catch (FormatException e)
        //    {
        //        // Expected
        //    }

        //    try
        //    {
        //        Int32.Decode("x123");
        //        fail("Expected exception for x123");
        //    }
        //    catch (FormatException e)
        //    {
        //        // Expected
        //    }

        //    try
        //    {
        //        Int32.Decode(null);
        //        fail("Expected exception for null");
        //    }
        //    catch (ArgumentNullException e)
        //    {
        //        // Expected
        //    }

        //    try
        //    {
        //        Int32.Decode("");
        //        fail("Expected exception for empty string");
        //    }
        //    catch (FormatException ex)
        //    {
        //        // Expected
        //    }

        //    try
        //    {
        //        Int32.Decode(" ");
        //        fail("Expected exception for single space");
        //    }
        //    catch (FormatException ex)
        //    {
        //        // Expected
        //    }

        //}

        /**
         * @tests java.lang.Integer#doubleValue()
         */
        [Test]
        public void Test_doubleValue2()
        {
            // Test for method double java.lang.Integer.doubleValue()
            assertEquals("Returned incorrect double value", 2147483647.0, new Int32(2147483647)
                    .ToDouble(), 0.0D);
            assertEquals("Returned incorrect double value", -2147483647.0, new Int32(-2147483647)
                    .ToDouble(), 0.0D);
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
            assertTrue("Equality test failed", i1.Equals((object)i2) && !(i1.Equals((object)i3)));
        }

        /**
        * @tests java.lang.Integer#equals(Int32)
        */
        [Test]
        public void Test_equals_Int32_2()
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
                    .ToSingle() == 65535.0f);
            assertTrue("Returned incorrect float value", new Int32(-65535)
                    .ToSingle() == -65535.0f);
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
            assertEquals("Returned incorrect int value", 8900, i.ToInt32());
        }

        /**
         * @tests java.lang.Integer#longValue()
         */
        [Test]
        public void Test_longValue2()
        {
            // Test for method long java.lang.Integer.longValue()
            Int32 i = new Int32(8900);
            assertEquals("Returned incorrect long value", 8900L, i.ToInt64());
        }

        // J2N: Moved to CharSequences
        ///**
        // * @tests java.lang.Integer#parseInt(java.lang.String)
        // */
        //[Test]
        //public void Test_parseIntLjava_lang_String2()
        //{
        //    // Test for method int java.lang.Integer.parseInt(java.lang.String)

        //    int i = Int32.Parse("-8900", J2N.Text.StringFormatter.InvariantCulture);
        //    assertEquals("Returned incorrect int", -8900, i);
        //    assertEquals("Returned incorrect value for 0", 0, Int32.Parse("0", J2N.Text.StringFormatter.InvariantCulture));
        //    assertTrue("Returned incorrect value for most negative value", Int32
        //            .Parse("-2147483648", J2N.Text.StringFormatter.InvariantCulture) == unchecked((int)0x80000000));
        //    assertTrue("Returned incorrect value for most positive value", Int32
        //            .Parse("2147483647", J2N.Text.StringFormatter.InvariantCulture) == 0x7fffffff);

        //    bool exception = false;
        //    try
        //    {
        //        Int32.Parse("999999999999", J2N.Text.StringFormatter.InvariantCulture);
        //    }
        //    catch (OverflowException e) // J2N: .NET throws OverflowException rather than FormatException in this case
        //    {
        //        // Correct
        //        exception = true;
        //    }
        //    assertTrue("Failed to throw exception for value > int", exception);

        //    exception = false;
        //    try
        //    {
        //        Int32.Parse("2147483648", J2N.Text.StringFormatter.InvariantCulture);
        //    }
        //    catch (OverflowException e) // J2N: .NET throws OverflowException rather than FormatException in this case
        //    {
        //        // Correct
        //        exception = true;
        //    }
        //    assertTrue("Failed to throw exception for MAX_VALUE + 1", exception);

        //    exception = false;
        //    try
        //    {
        //        Int32.Parse("-2147483649", J2N.Text.StringFormatter.InvariantCulture);
        //    }
        //    catch (OverflowException e) // J2N: .NET throws OverflowException rather than FormatException in this case
        //    {
        //        // Correct
        //        exception = true;
        //    }
        //    assertTrue("Failed to throw exception for MIN_VALUE - 1", exception);
        //}

        // J2N: Moved to CharSequences
        ///**
        // * @tests java.lang.Integer#parseInt(java.lang.String, int)
        // */
        //[Test]
        //public void Test_parseIntLjava_lang_StringI2()
        //{
        //    // Test for method int java.lang.Integer.parseInt(java.lang.String, int)
        //    assertEquals("Parsed dec val incorrectly",
        //            -8000, Int32.Parse("-8000", 10));
        //    assertEquals("Parsed hex val incorrectly",
        //            255, Int32.Parse("FF", 16));
        //    assertEquals("Parsed oct val incorrectly",
        //            16, Int32.Parse("20", 8));
        //    assertEquals("Returned incorrect value for 0 hex", 0, Int32.Parse("0",
        //            16));
        //    assertTrue("Returned incorrect value for most negative value hex",
        //            Int32.Parse("-80000000", 16) == unchecked((int)0x80000000));
        //    assertTrue("Returned incorrect value for most positive value hex",
        //            Int32.Parse("7fffffff", 16) == 0x7fffffff);
        //    assertEquals("Returned incorrect value for 0 decimal", 0, Int32.Parse(
        //            "0", 10));
        //    assertTrue("Returned incorrect value for most negative value decimal",
        //            Int32.Parse("-2147483648", 10) == unchecked((int)0x80000000));
        //    assertTrue("Returned incorrect value for most positive value decimal",
        //            Int32.Parse("2147483647", 10) == 0x7fffffff);

        //    bool exception = false;
        //    try
        //    {
        //        Int32.Parse("FFFF", 10);
        //    }
        //    catch (FormatException e)
        //    {
        //        // Correct
        //        exception = true;
        //    }
        //    assertTrue(
        //            "Failed to throw exception when passes hex string and dec parm",
        //            exception);

        //    exception = false;
        //    try
        //    {
        //        Int32.Parse("2147483648", 10);
        //    }
        //    catch (OverflowException e) // J2N: .NET throws OverflowException rather than FormatException in this case
        //    {
        //        // Correct
        //        exception = true;
        //    }
        //    assertTrue("Failed to throw exception for MAX_VALUE + 1", exception);

        //    exception = false;
        //    try
        //    {
        //        Int32.Parse("-2147483649", 10);
        //    }
        //    catch (OverflowException e) // J2N: .NET throws OverflowException rather than FormatException in this case
        //    {
        //        // Correct
        //        exception = true;
        //    }
        //    assertTrue("Failed to throw exception for MIN_VALUE - 1", exception);

        //    // J2N: MinValue is a special case and must allow both a positive and negative version to be compatible
        //    // with both .NET and Java
        //    //exception = false;
        //    //try
        //    //{
        //    //    Int32.Parse("80000000", 16);
        //    //}
        //    //catch (OverflowException e) // J2N: .NET throws OverflowException rather than FormatException in this case
        //    //{
        //    //    // Correct
        //    //    exception = true;
        //    //}
        //    //assertTrue("Failed to throw exception for hex MAX_VALUE + 1", exception);

        //    assertEquals(1, Int32.Parse("1", 16));
        //    assertEquals(-1, Int32.Parse("ffffffff", 16));
        //    assertEquals(2147483647, Int32.Parse("7fffffff", 16));
        //    assertEquals(-2147483648, Int32.Parse("-80000000", 16)); // Special case: In Java, we allow the negative sign for the smallest negative number
        //    assertEquals(-2147483648, Int32.Parse("80000000", 16));  // In .NET, it should parse without the negative sign to the same value (in .NET the negative sign is not allowed)
        //    assertEquals(-2147483647, Int32.Parse("80000001", 16));

        //    exception = false;
        //    try
        //    {
        //        Int32.Parse("-80000001", 16);
        //    }
        //    catch (OverflowException e) // J2N: .NET throws OverflowException rather than FormatException in this case
        //    {
        //        // Correct
        //        exception = true;
        //    }
        //    assertTrue("Failed to throw exception for hex MIN_VALUE + 1", exception);

        //    exception = false;
        //    try
        //    {
        //        Int32.Parse("9999999999", 10);
        //    }
        //    catch (OverflowException e) // J2N: .NET throws OverflowException rather than FormatException in this case
        //    {
        //        // Correct
        //        exception = true;
        //    }
        //    assertTrue("Failed to throw exception for 9999999999", exception);
        //}

        /**
         * @tests java.lang.Integer#shortValue()
         */
        [Test]
        public void Test_shortValue2()
        {
            // Test for method short java.lang.Integer.shortValue()
            Int32 i = new Int32(2147450880);
            assertEquals("Returned incorrect long value", -32768, i.ToInt16());
        }

        /**
         * @tests java.lang.Integer#toBinaryString(int)
         */
        [Test]
        public void Test_toBinaryStringI()
        {
            // Test for method java.lang.String
            // java.lang.Integer.toBinaryString(int)
            assertEquals("Incorrect string returned", "1111111111111111111111111111111", Int32.GetInstance(
                    int.MaxValue).ToBinaryString());
            assertEquals("Incorrect string returned", "10000000000000000000000000000000", Int32.GetInstance(
                    int.MinValue).ToBinaryString());
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
                        .GetInstance(i).ToHexString().Equals(hexvals[i]));
            }

            assertTrue("Returned incorrect hex string: "
                    + Int32.GetInstance(int.MaxValue).ToHexString(), Int32.GetInstance(
                    int.MaxValue).ToHexString().Equals("7fffffff"));
            assertTrue("Returned incorrect hex string: "
                    + Int32.GetInstance(int.MinValue).ToHexString(), Int32.GetInstance(
                    int.MinValue).ToHexString().Equals("80000000"));
        }

        /**
         * @tests java.lang.Integer#toOctalString(int)
         */
        [Test]
        public void Test_toOctalStringI()
        {
            // Test for method java.lang.String java.lang.Integer.toOctalString(int)
            // Spec states that the int arg is treated as unsigned
            assertEquals("Returned incorrect octal string", "17777777777", Int32.GetInstance(
                    int.MaxValue).ToOctalString());
            assertEquals("Returned incorrect octal string", "20000000000", Int32.GetInstance(
                    int.MinValue).ToOctalString());
        }

        /**
         * @tests java.lang.Integer#toString()
         */
        [Test]
        public void Test_toString2()
        {
            // Test for method java.lang.String java.lang.Integer.toString()

            Int32 i = new Int32(-80001);

            assertEquals("Returned incorrect String", "-80001", i.ToString(CultureInfo.InvariantCulture));
        }

        /**
         * @tests java.lang.Integer#toString(int)
         */
        [Test]
        public void Test_toStringI2()
        {
            // Test for method java.lang.String java.lang.Integer.toString(int)

            //assertEquals("Returned incorrect String", "-80765", Int32.ToString(-80765)
            //        );
            //assertEquals("Returned incorrect octal string", "2147483647", Int32.ToString(
            //        int.MaxValue));
            //assertEquals("Returned incorrect octal string", "-2147483647", Int32.ToString(
            //        -int.MaxValue));
            //assertEquals("Returned incorrect octal string", "-2147483648", Int32.ToString(
            //        int.MinValue));

            //// Test for HARMONY-6068
            //assertEquals("Returned incorrect octal String", "-1000", Int32.ToString(-1000));
            //assertEquals("Returned incorrect octal String", "1000", Int32.ToString(1000));
            //assertEquals("Returned incorrect octal String", "0", Int32.ToString(0));
            //assertEquals("Returned incorrect octal String", "708", Int32.ToString(708));
            //assertEquals("Returned incorrect octal String", "-100", Int32.ToString(-100));
            //assertEquals("Returned incorrect octal String", "-1000000008", Int32.ToString(-1000000008));
            //assertEquals("Returned incorrect octal String", "2000000008", Int32.ToString(2000000008));

            Test_toString(-80765, "-80765");
            Test_toString(int.MaxValue, "2147483647");
            Test_toString(-int.MaxValue, "-2147483647");
            Test_toString(int.MinValue, "-2147483648");

            // Test for HARMONY-6068
            Test_toString(-1000, "-1000");
            Test_toString(1000, "1000");
            Test_toString(0, "0");
            Test_toString(708, "708");
            Test_toString(-100, "-100");
            Test_toString(-1000000008, "-1000000008");
            Test_toString(2000000008, "2000000008");
        }

        // J2N: Moved to IntegralNumberExtensions
        ///**
        // * @tests java.lang.Integer#toString(int, int)
        // */
        //[Test]
        //public void Test_toStringII()
        //{
        //    // Test for method java.lang.String java.lang.Integer.toString(int, int)
        //    assertEquals("Returned incorrect octal string", "17777777777", Int32.ToString(
        //            2147483647, 8));
        //    assertTrue("Returned incorrect hex string--wanted 7fffffff but got: "
        //            + Int32.ToString(2147483647, 16), Int32.ToString(
        //            2147483647, 16).Equals("7fffffff"));
        //    assertEquals("Incorrect string returned", "1111111111111111111111111111111", Int32.ToString(2147483647, 2)
        //            );
        //    assertEquals("Incorrect string returned", "2147483647", Int32
        //            .ToString(2147483647, 10));

        //    assertEquals("Returned incorrect octal string", "-17777777777", Int32.ToString(
        //            -2147483647, 8));
        //    assertTrue("Returned incorrect hex string--wanted -7fffffff but got: "
        //            + Int32.ToString(-2147483647, 16), Int32.ToString(
        //            -2147483647, 16).Equals("-7fffffff"));
        //    assertEquals("Incorrect string returned",
        //                    "-1111111111111111111111111111111", Int32
        //            .ToString(-2147483647, 2));
        //    assertEquals("Incorrect string returned", "-2147483647", Int32.ToString(-2147483647,
        //            10));

        //    assertEquals("Returned incorrect octal string", "-20000000000", Int32.ToString(
        //            -2147483648, 8));
        //    assertTrue("Returned incorrect hex string--wanted -80000000 but got: "
        //            + Int32.ToString(-2147483648, 16), Int32.ToString(
        //            -2147483648, 16).Equals("-80000000"));
        //    assertEquals("Incorrect string returned",
        //                    "-10000000000000000000000000000000", Int32
        //            .ToString(-2147483648, 2));
        //    assertEquals("Incorrect string returned", "-2147483648", Int32.ToString(-2147483648,
        //            10));
        //}

        /**
         * @tests java.lang.Integer#valueOf(java.lang.String)
         */
        [Test]
        public void Test_valueOfLjava_lang_String2()
        {
            // Test for method java.lang.Integer
            // java.lang.Integer.valueOf(java.lang.String)
            assertEquals("Returned incorrect int", 8888888, Int32.GetInstance("8888888", J2N.Text.StringFormatter.InvariantCulture)
                    .ToInt32());
            assertTrue("Returned incorrect int", Int32.GetInstance("2147483647", J2N.Text.StringFormatter.InvariantCulture)
                    .ToInt32() == int.MaxValue);
            assertTrue("Returned incorrect int", Int32.GetInstance("-2147483648", J2N.Text.StringFormatter.InvariantCulture)
                    .ToInt32() == int.MinValue);

            bool exception = false;
            try
            {
                Int32.GetInstance("2147483648", J2N.Text.StringFormatter.InvariantCulture);
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
                Int32.GetInstance("-2147483649", J2N.Text.StringFormatter.InvariantCulture);
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
            assertEquals("Returned incorrect int for hex string", 255, Int32.GetInstance(
                    "FF", 16).ToInt32());
            assertEquals("Returned incorrect int for oct string", 16, Int32.GetInstance(
                    "20", 8).ToInt32());
            assertEquals("Returned incorrect int for bin string", 4, Int32.GetInstance(
                    "100", 2).ToInt32());

            assertEquals("Returned incorrect int for - hex string", -255, Int32.GetInstance(
                    "-FF", 16).ToInt32());
            assertEquals("Returned incorrect int for - oct string", -16, Int32.GetInstance(
                    "-20", 8).ToInt32());
            assertEquals("Returned incorrect int for - bin string", -4, Int32.GetInstance(
                    "-100", 2).ToInt32());
            assertTrue("Returned incorrect int", Int32.GetInstance("2147483647", 10)
                    .ToInt32() == int.MaxValue);
            assertTrue("Returned incorrect int", Int32.GetInstance("-2147483648", 10)
                    .ToInt32() == int.MinValue);
            assertTrue("Returned incorrect int", Int32.GetInstance("7fffffff", 16)
                    .ToInt32() == int.MaxValue);
            assertTrue("Returned incorrect int", Int32.GetInstance("-80000000", 16)
                    .ToInt32() == int.MinValue);

            bool exception = false;
            try
            {
                Int32.GetInstance("FF", 2);
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
                Int32.GetInstance("2147483648", 10);
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
                Int32.GetInstance("-2147483649", 10);
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
            //    Int32.GetInstance("80000000", 16);
            //}
            //catch (OverflowException e) // J2N: .NET throws OverflowException rather than FormatException in this case
            //{
            //    // Correct
            //    exception = true;
            //}
            //assertTrue("Failed to throw exception with hex MAX_VALUE + 1",
            //        exception);

            assertEquals(1, Int32.GetInstance("1", 16).ToInt32());
            assertEquals(-1, Int32.GetInstance("ffffffff", 16).ToInt32());
            assertEquals(2147483647, Int32.GetInstance("7fffffff", 16).ToInt32());
            assertEquals(-2147483648, Int32.GetInstance("-80000000", 16).ToInt32()); // Special case: In Java, we allow the negative sign for the smallest negative number
            assertEquals(-2147483648, Int32.GetInstance("80000000", 16).ToInt32());  // In .NET, it should parse without the negative sign to the same value (in .NET the negative sign is not allowed)
            assertEquals(-2147483647, Int32.GetInstance("80000001", 16).ToInt32());

            exception = false;
            try
            {
                Int32.GetInstance("-80000001", 16);
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
            assertEquals(new Int32(int.MinValue), Int32.GetInstance(int.MinValue));
            assertEquals(new Int32(int.MaxValue), Int32.GetInstance(int.MaxValue));
            assertEquals(new Int32(0), Int32.GetInstance(0));

            short s = -128;
            while (s < 128)
            {
                assertEquals(new Int32(s), Int32.GetInstance(s));
                assertSame(Int32.GetInstance(s), Int32.GetInstance(s));
                s++;
            }
        }

        [Test]
        public void GetTypeCode_Invoke_ReturnsInt32()
        {
            assertEquals(TypeCode.Int32, Int32.GetInstance(1).GetTypeCode());
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

        // J2N: Removed this overload because all constructors have been deprecated in JDK 16
        ///**
        // * @tests java.lang.Integer#Int32(String)
        // */
        //[Test]
        //public void Test_ConstructorLjava_lang_String()
        //{
        //    assertEquals(new Int32(0), new Int32("0", J2N.Text.StringFormatter.InvariantCulture));
        //    assertEquals(new Int32(1), new Int32("1", J2N.Text.StringFormatter.InvariantCulture));
        //    assertEquals(new Int32(-1), new Int32("-1", J2N.Text.StringFormatter.InvariantCulture));

        //    try
        //    {
        //        new Int32("0x1", J2N.Text.StringFormatter.InvariantCulture);
        //        fail("Expected FormatException with hex string.");
        //    }
        //    catch (FormatException e) { }

        //    try
        //    {
        //        new Int32("9.2", J2N.Text.StringFormatter.InvariantCulture);
        //        fail("Expected FormatException with floating point string.");
        //    }
        //    catch (FormatException e) { }

        //    try
        //    {
        //        new Int32("", J2N.Text.StringFormatter.InvariantCulture);
        //        fail("Expected FormatException with empty string.");
        //    }
        //    catch (FormatException e) { }

        //    try
        //    {
        //        new Int32(null, J2N.Text.StringFormatter.InvariantCulture);
        //        fail("Expected FormatException with null string.");
        //    }
        //    catch (ArgumentNullException e) { } // J2N: .NET throws ArgumentNullException rather than FormatException in this case
        //}

        /**
         * @tests java.lang.Integer#Integer
         */
        [Test]
        public void Test_ConstructorI()
        {
            assertEquals(1, new Int32(1).ToInt32());
            assertEquals(2, new Int32(2).ToInt32());
            assertEquals(0, new Int32(0).ToInt32());
            assertEquals(-1, new Int32(-1).ToInt32());

            Int32 i = new Int32(-89000);
            assertEquals("Incorrect Integer created", -89000, i.ToInt32());
        }

        /**
         * @tests java.lang.Integer#byteValue()
         */
        [Test]
        public void Test_booleanValue()
        {
            assertEquals(1, new Int32(1).ToByte());
            assertEquals(2, new Int32(2).ToByte());
            assertEquals(0, new Int32(0).ToByte());
            assertEquals(-1, (sbyte)new Int32(-1).ToByte()); // J2N: cast required to change the result to negative
        }

        /**
         * @tests java.lang.Integer#equals(Object)
         */
        [Test]
        public void Test_equalsLjava_lang_Object()
        {
            assertEquals((object)new Int32(0), (object)Int32.GetInstance(0));
            assertEquals((object)new Int32(1), (object)Int32.GetInstance(1));
            assertEquals((object)new Int32(-1), (object)Int32.GetInstance(-1));

            Int32 fixture = new Int32(25);
            assertEquals((object)fixture, (object)fixture);
            assertFalse(fixture.Equals((object)null));
            assertFalse(fixture.Equals((object)"Not a Integer"));
        }

        /**
        * @tests java.lang.Integer#equals(Object)
        */
        [Test]
        public void Test_equals_Int32()
        {
            // Implicit conversion
            assertEquals(new Int32(0), Int32.GetInstance(0));
            assertEquals(new Int32(1), Int32.GetInstance(1));
            assertEquals(new Int32(-1), Int32.GetInstance(-1));

            // Explicit
            assertTrue(new Int32(0).Equals(Int32.GetInstance(0)));
            assertTrue(new Int32(1).Equals(Int32.GetInstance(1)));
            assertTrue(new Int32(-1).Equals(Int32.GetInstance(-1)));

            Int32 fixture = new Int32(25);
            assertEquals(fixture, fixture);
            assertFalse(fixture.Equals((Int32)null));
            //assertFalse(fixture.Equals("Not a Integer"));
        }

        // J2N: Centralizes ToString()/TryFormat() logic to allow testing all overloads

        private void Test_toString(int ii, string answer)
        {
            // Test for method java.lang.String java.lang.Double.toString(double)
            assertTrue("Incorrect String representation want " + answer + ", got ("
                    + Int32.ToString(ii, null, J2N.Text.StringFormatter.InvariantCulture) + ")", Int32.ToString(ii, null, J2N.Text.StringFormatter.InvariantCulture).Equals(answer));
            Int32 i = new Int32(ii);
            assertTrue("Incorrect String representation want " + answer + ", got ("
                    + Int32.ToString(i.ToInt32(), null, J2N.Text.StringFormatter.InvariantCulture) + ")", Int32.ToString(i.ToInt32(), null, J2N.Text.StringFormatter.InvariantCulture).Equals(
                    answer));
            assertTrue("Incorrect String representation want " + answer + ", got (" + i.ToString(J2N.Text.StringFormatter.InvariantCulture)
                    + ")", i.ToString(J2N.Text.StringFormatter.InvariantCulture).Equals(answer));

#if FEATURE_SPAN
            Span<char> buffer = stackalloc char[64];
            assertTrue(i.TryFormat(buffer, out int charsWritten, ReadOnlySpan<char>.Empty, CultureInfo.InvariantCulture));
            string actual = buffer.Slice(0, charsWritten).ToString();
            assertEquals("Incorrect String representation want " + answer + ", got (" + actual + ")", answer, actual);

            assertTrue(Int32.TryFormat(ii, buffer, out charsWritten, provider: CultureInfo.InvariantCulture));
            actual = buffer.Slice(0, charsWritten).ToString();
            assertEquals("Incorrect String representation want " + answer + ", got (" + actual + ")", answer, actual);
#endif
        }

        /**
         * @tests java.lang.Integer#toString()
         */
        [Test]
        public void Test_toString() // J2N TODO: Culture tests
        {
            //assertEquals("-1", new Int32(-1).ToString());
            //assertEquals("0", new Int32(0).ToString());
            //assertEquals("1", new Int32(1).ToString());
            //assertEquals("-1", new Int32(unchecked((int)0xFFFFFFFF)).ToString());

            Test_toString((int)-1, "-1");
            Test_toString((int)0, "0");
            Test_toString((int)1, "1");
            Test_toString(unchecked((int)0xFFFFFFFF), "-1");
        }

        // J2N: Same values as above. No need to run again.
        ///**
        // * @tests java.lang.Integer#toString
        // */
        //[Test]
        //public void Test_toStringI() // J2N TODO: Culture tests
        //{
        //    assertEquals("-1", Int32.ToString(-1));
        //    assertEquals("0", Int32.ToString(0));
        //    assertEquals("1", Int32.ToString(1));
        //    assertEquals("-1", Int32.ToString(unchecked((int)0xFFFFFFFF)));
        //}

        /**
         * @tests java.lang.Integer#valueOf(String)
         */
        [Test]
        public void Test_valueOfLjava_lang_String()
        {
            assertEquals(new Int32(0), Int32.GetInstance("0", J2N.Text.StringFormatter.InvariantCulture));
            assertEquals(new Int32(1), Int32.GetInstance("1", J2N.Text.StringFormatter.InvariantCulture));
            assertEquals(new Int32(-1), Int32.GetInstance("-1", J2N.Text.StringFormatter.InvariantCulture));

            try
            {
                Int32.GetInstance("0x1", J2N.Text.StringFormatter.InvariantCulture);
                fail("Expected FormatException with hex string.");
            }
            catch (FormatException e) { }

            try
            {
                Int32.GetInstance("9.2", J2N.Text.StringFormatter.InvariantCulture);
                fail("Expected FormatException with floating point string.");
            }
            catch (FormatException e) { }

            try
            {
                Int32.GetInstance("", J2N.Text.StringFormatter.InvariantCulture);
                fail("Expected FormatException with empty string.");
            }
            catch (FormatException e) { }

            try
            {
                Int32.GetInstance(null, J2N.Text.StringFormatter.InvariantCulture);
                fail("Expected FormatException with null string.");
            }
            catch (ArgumentNullException e) { } // J2N: .NET throws ArgumentNullException rather than FormatException in this case
        }

        // J2N: Moved to CharSequences
        ///**
        // * @tests java.lang.Integer#valueOf(String,int)
        // */
        //[Test]
        //public void Test_valueOfLjava_lang_StringI()
        //{
        //    assertEquals(new Int32(0), Int32.GetInstance("0", 10));
        //    assertEquals(new Int32(1), Int32.GetInstance("1", 10));
        //    assertEquals(new Int32(-1), Int32.GetInstance("-1", 10));

        //    //must be consistent with Character.digit()
        //    assertEquals(Character.Digit('1', 2), Int32.GetInstance("1", 2).GetByteValue());
        //    assertEquals(Character.Digit('F', 16), Int32.GetInstance("F", 16).GetByteValue());

        //    try
        //    {
        //        Int32.GetInstance("0x1", 10);
        //        fail("Expected FormatException with hex string.");
        //    }
        //    catch (FormatException e) { }

        //    try
        //    {
        //        Int32.GetInstance("9.2", 10);
        //        fail("Expected FormatException with floating point string.");
        //    }
        //    catch (FormatException e) { }

        //    try
        //    {
        //        Int32.GetInstance("", 10);
        //        fail("Expected FormatException with empty string.");
        //    }
        //    catch (FormatException e) { }

        //    //try
        //    //{
        //    //    Int32.GetInstance(null, 10);
        //    //    fail("Expected FormatException with null string.");
        //    //}
        //    //catch (FormatException e) { }

        //    // J2N: Match .NET behavior and return 0 for a null string
        //    assertEquals(0, Int32.GetInstance(null, 10));
        //}

        // J2N: Moved to CharSequences
        ///**
        // * @tests java.lang.Integer#parseInt(String)
        // */
        //[Test]
        //public void Test_parseIntLjava_lang_String()
        //{
        //    assertEquals(0, Int32.Parse("0", J2N.Text.StringFormatter.InvariantCulture));
        //    assertEquals(1, Int32.Parse("1", J2N.Text.StringFormatter.InvariantCulture));
        //    assertEquals(-1, Int32.Parse("-1", J2N.Text.StringFormatter.InvariantCulture));

        //    try
        //    {
        //        Int32.Parse("0x1", J2N.Text.StringFormatter.InvariantCulture);
        //        fail("Expected FormatException with hex string.");
        //    }
        //    catch (FormatException e) { }

        //    try
        //    {
        //        Int32.Parse("9.2", J2N.Text.StringFormatter.InvariantCulture);
        //        fail("Expected FormatException with floating point string.");
        //    }
        //    catch (FormatException e) { }

        //    try
        //    {
        //        Int32.Parse("", J2N.Text.StringFormatter.InvariantCulture);
        //        fail("Expected FormatException with empty string.");
        //    }
        //    catch (FormatException e) { }

        //    try
        //    {
        //        Int32.Parse(null, J2N.Text.StringFormatter.InvariantCulture);
        //        fail("Expected FormatException with null string.");
        //    }
        //    catch (ArgumentNullException e) { } // J2N: .NET throws ArgumentNullException rather than FormatException in this case
        //}

        // J2N: Moved to CharSequences
        ///**
        // * @tests java.lang.Integer#parseInt(String,int)
        // */
        //[Test]
        //public void Test_parseIntLjava_lang_StringI()
        //{
        //    assertEquals(0, Int32.Parse("0", 10));
        //    assertEquals(1, Int32.Parse("1", 10));
        //    assertEquals(-1, Int32.Parse("-1", 10));

        //    //must be consistent with Character.digit()
        //    assertEquals(Character.Digit('1', 2), Int32.Parse("1", 2));
        //    assertEquals(Character.Digit('F', 16), Int32.Parse("F", 16));

        //    try
        //    {
        //        Int32.Parse("0x1", 10);
        //        fail("Expected FormatException with hex string.");
        //    }
        //    catch (FormatException e) { }

        //    try
        //    {
        //        Int32.Parse("9.2", 10);
        //        fail("Expected FormatException with floating point string.");
        //    }
        //    catch (FormatException e) { }

        //    try
        //    {
        //        Int32.Parse("", 10);
        //        fail("Expected FormatException with empty string.");
        //    }
        //    catch (FormatException e) { }

        //    //try
        //    //{
        //    //    Int32.Parse(null, 10);
        //    //    fail("Expected FormatException with null string.");
        //    //}
        //    //catch (FormatException e) { }

        //    // J2N: Match .NET behavior where null will result in 0
        //    assertEquals(0, Int32.Parse(null, 10));
        //}

        // J2N: Moved to CharSequences
        ///**
        // * @tests java.lang.Integer#decode(String)
        // */
        //[Test]
        //public void Test_decodeLjava_lang_String()
        //{
        //    assertEquals(new Int32(0), Int32.Decode("0"));
        //    assertEquals(new Int32(1), Int32.Decode("1"));
        //    assertEquals(new Int32(-1), Int32.Decode("-1"));
        //    assertEquals(new Int32(0xF), Int32.Decode("0xF"));
        //    assertEquals(new Int32(0xF), Int32.Decode("#F"));
        //    assertEquals(new Int32(0xF), Int32.Decode("0XF"));
        //    assertEquals(new Int32(07), Int32.Decode("07"));

        //    try
        //    {
        //        Int32.Decode("9.2");
        //        fail("Expected FormatException with floating point string.");
        //    }
        //    catch (FormatException e) { }

        //    try
        //    {
        //        Int32.Decode("");
        //        fail("Expected FormatException with empty string.");
        //    }
        //    catch (FormatException e) { }

        //    try
        //    {
        //        Int32.Decode(null);
        //        //undocumented NPE, but seems consistent across JREs
        //        fail("Expected NullPointerException with null string.");
        //    }
        //    catch (ArgumentNullException e) { }
        //}

        /**
         * @tests java.lang.Integer#doubleValue()
         */
        [Test]
        public void Test_doubleValue()
        {
            assertEquals(-1D, new Int32(-1).ToDouble(), 0D);
            assertEquals(0D, new Int32(0).ToDouble(), 0D);
            assertEquals(1D, new Int32(1).ToDouble(), 0D);
        }

        /**
         * @tests java.lang.Integer#floatValue()
         */
        [Test]
        public void Test_floatValue()
        {
            assertEquals(-1F, new Int32(-1).ToSingle(), 0F);
            assertEquals(0F, new Int32(0).ToSingle(), 0F);
            assertEquals(1F, new Int32(1).ToSingle(), 0F);
        }

        /**
         * @tests java.lang.Integer#intValue()
         */
        [Test]
        public void Test_intValue()
        {
            assertEquals(-1, new Int32(-1).ToInt32());
            assertEquals(0, new Int32(0).ToInt32());
            assertEquals(1, new Int32(1).ToInt32());
        }

        /**
         * @tests java.lang.Integer#longValue()
         */
        [Test]
        public void Test_longValue()
        {
            assertEquals(-1L, new Int32(-1).ToInt64());
            assertEquals(0L, new Int32(0).ToInt64());
            assertEquals(1L, new Int32(1).ToInt64());
        }

        /**
         * @tests java.lang.Integer#shortValue()
         */
        [Test]
        public void Test_shortValue()
        {
            assertEquals(-1, new Int32(-1).ToInt16());
            assertEquals(0, new Int32(0).ToInt16());
            assertEquals(1, new Int32(1).ToInt16());
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
            assertEquals(32, Int32.LeadingZeroCount(0x0));
            assertEquals(31, Int32.LeadingZeroCount(0x1));
            assertEquals(30, Int32.LeadingZeroCount(0x2));
            assertEquals(30, Int32.LeadingZeroCount(0x3));
            assertEquals(29, Int32.LeadingZeroCount(0x4));
            assertEquals(29, Int32.LeadingZeroCount(0x5));
            assertEquals(29, Int32.LeadingZeroCount(0x6));
            assertEquals(29, Int32.LeadingZeroCount(0x7));
            assertEquals(28, Int32.LeadingZeroCount(0x8));
            assertEquals(28, Int32.LeadingZeroCount(0x9));
            assertEquals(28, Int32.LeadingZeroCount(0xA));
            assertEquals(28, Int32.LeadingZeroCount(0xB));
            assertEquals(28, Int32.LeadingZeroCount(0xC));
            assertEquals(28, Int32.LeadingZeroCount(0xD));
            assertEquals(28, Int32.LeadingZeroCount(0xE));
            assertEquals(28, Int32.LeadingZeroCount(0xF));
            assertEquals(27, Int32.LeadingZeroCount(0x10));
            assertEquals(24, Int32.LeadingZeroCount(0x80));
            assertEquals(24, Int32.LeadingZeroCount(0xF0));
            assertEquals(23, Int32.LeadingZeroCount(0x100));
            assertEquals(20, Int32.LeadingZeroCount(0x800));
            assertEquals(20, Int32.LeadingZeroCount(0xF00));
            assertEquals(19, Int32.LeadingZeroCount(0x1000));
            assertEquals(16, Int32.LeadingZeroCount(0x8000));
            assertEquals(16, Int32.LeadingZeroCount(0xF000));
            assertEquals(15, Int32.LeadingZeroCount(0x10000));
            assertEquals(12, Int32.LeadingZeroCount(0x80000));
            assertEquals(12, Int32.LeadingZeroCount(0xF0000));
            assertEquals(11, Int32.LeadingZeroCount(0x100000));
            assertEquals(8, Int32.LeadingZeroCount(0x800000));
            assertEquals(8, Int32.LeadingZeroCount(0xF00000));
            assertEquals(7, Int32.LeadingZeroCount(0x1000000));
            assertEquals(4, Int32.LeadingZeroCount(0x8000000));
            assertEquals(4, Int32.LeadingZeroCount(0xF000000));
            assertEquals(3, Int32.LeadingZeroCount(0x10000000));
            assertEquals(0, Int32.LeadingZeroCount(unchecked((int)0x80000000)));
            assertEquals(0, Int32.LeadingZeroCount(unchecked((int)0xF0000000)));

            assertEquals(1, Int32.LeadingZeroCount(int.MaxValue));
            assertEquals(0, Int32.LeadingZeroCount(int.MinValue));
        }

        /**
         * @tests java.lang.Integer#numberOfTrailingZeros(int)
         */
        [Test]
        public void Test_numberOfTrailingZerosI()
        {
            assertEquals(32, Int32.TrailingZeroCount(0x0));
            assertEquals(31, Int32.TrailingZeroCount(int.MinValue));
            assertEquals(0, Int32.TrailingZeroCount(int.MaxValue));

            assertEquals(0, Int32.TrailingZeroCount(0x1));
            assertEquals(3, Int32.TrailingZeroCount(0x8));
            assertEquals(0, Int32.TrailingZeroCount(0xF));

            assertEquals(4, Int32.TrailingZeroCount(0x10));
            assertEquals(7, Int32.TrailingZeroCount(0x80));
            assertEquals(4, Int32.TrailingZeroCount(0xF0));

            assertEquals(8, Int32.TrailingZeroCount(0x100));
            assertEquals(11, Int32.TrailingZeroCount(0x800));
            assertEquals(8, Int32.TrailingZeroCount(0xF00));

            assertEquals(12, Int32.TrailingZeroCount(0x1000));
            assertEquals(15, Int32.TrailingZeroCount(0x8000));
            assertEquals(12, Int32.TrailingZeroCount(0xF000));

            assertEquals(16, Int32.TrailingZeroCount(0x10000));
            assertEquals(19, Int32.TrailingZeroCount(0x80000));
            assertEquals(16, Int32.TrailingZeroCount(0xF0000));

            assertEquals(20, Int32.TrailingZeroCount(0x100000));
            assertEquals(23, Int32.TrailingZeroCount(0x800000));
            assertEquals(20, Int32.TrailingZeroCount(0xF00000));

            assertEquals(24, Int32.TrailingZeroCount(0x1000000));
            assertEquals(27, Int32.TrailingZeroCount(0x8000000));
            assertEquals(24, Int32.TrailingZeroCount(0xF000000));

            assertEquals(28, Int32.TrailingZeroCount(0x10000000));
            assertEquals(31, Int32.TrailingZeroCount(unchecked((int)0x80000000)));
            assertEquals(28, Int32.TrailingZeroCount(unchecked((int)0xF0000000)));
        }

        /**
         * @tests java.lang.Integer#bitCount(int)
         */
        [Test]
        public void Test_bitCountI()
        {
            assertEquals(0, Int32.PopCount(0x0));
            assertEquals(1, Int32.PopCount(0x1));
            assertEquals(1, Int32.PopCount(0x2));
            assertEquals(2, Int32.PopCount(0x3));
            assertEquals(1, Int32.PopCount(0x4));
            assertEquals(2, Int32.PopCount(0x5));
            assertEquals(2, Int32.PopCount(0x6));
            assertEquals(3, Int32.PopCount(0x7));
            assertEquals(1, Int32.PopCount(0x8));
            assertEquals(2, Int32.PopCount(0x9));
            assertEquals(2, Int32.PopCount(0xA));
            assertEquals(3, Int32.PopCount(0xB));
            assertEquals(2, Int32.PopCount(0xC));
            assertEquals(3, Int32.PopCount(0xD));
            assertEquals(3, Int32.PopCount(0xE));
            assertEquals(4, Int32.PopCount(0xF));

            assertEquals(8, Int32.PopCount(0xFF));
            assertEquals(12, Int32.PopCount(0xFFF));
            assertEquals(16, Int32.PopCount(0xFFFF));
            assertEquals(20, Int32.PopCount(0xFFFFF));
            assertEquals(24, Int32.PopCount(0xFFFFFF));
            assertEquals(28, Int32.PopCount(0xFFFFFFF));
            assertEquals(32, Int32.PopCount(unchecked((int)0xFFFFFFFF)));
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

        public class CharSequences : TestCase
        {
            #region ParseTestCase

            public abstract class ParseTestCase
            {
                #region TestParse_CharSequence_Int32_Data

                public static IEnumerable<TestCaseData> TestParse_CharSequence_Int32_Data
                {
                    get
                    {
                        // JDK 8

                        yield return new TestCaseData(+100, "+100", 10);
                        yield return new TestCaseData(-100, "-100", 10);

                        yield return new TestCaseData(0, "+0", 10);
                        yield return new TestCaseData(0, "-0", 10);
                        yield return new TestCaseData(0, "+00000", 10);
                        yield return new TestCaseData(0, "-00000", 10);

                        yield return new TestCaseData(0, "0", 10);
                        yield return new TestCaseData(1, "1", 10);
                        yield return new TestCaseData(9, "9", 10);

                        // Harmony (Test_parseIntLjava_lang_String())

                        yield return new TestCaseData(0, "0", 10);
                        yield return new TestCaseData(1, "1", 10);
                        yield return new TestCaseData(-1, "-1", 10);
                        yield return new TestCaseData(0, null, 10); // .NET returns 0 in this case (JDK throws)

                        // Harmony (Test_parseIntLjava_lang_String2())

                        yield return new TestCaseData(-8900, "-8900", 10);
                        yield return new TestCaseData(0, "0", 10);
                        yield return new TestCaseData(unchecked((int)0x80000000), "-2147483648", 10);
                        yield return new TestCaseData(0x7fffffff, "2147483647", 10);

                        // Harmony (Test_parseIntLjava_lang_StringI())

                        yield return new TestCaseData(0, "0", 10);
                        yield return new TestCaseData(1, "1", 10);
                        yield return new TestCaseData(-1, "-1", 10);

                        //must be consistent with Character.digit()
                        yield return new TestCaseData(Character.Digit('1', 2), "1", 2);
                        yield return new TestCaseData(Character.Digit('F', 16), "F", 16);

                        yield return new TestCaseData(0, null, 10); // J2N: Match .NET behavior where null will result in 0

                        // Harmony (Test_parseIntLjava_lang_StringI2())

                        yield return new TestCaseData(-8000, "-8000", 10);
                        yield return new TestCaseData(255, "FF", 16);
                        yield return new TestCaseData(16, "20", 8);
                        yield return new TestCaseData(0, "0", 16);
                        yield return new TestCaseData(unchecked((int)0x80000000), "-80000000", 16);
                        yield return new TestCaseData(0x7fffffff, "7fffffff", 16);
                        yield return new TestCaseData(0, "0", 10);
                        yield return new TestCaseData(unchecked((int)0x80000000), "-2147483648", 10);
                        yield return new TestCaseData(0x7fffffff, "2147483647", 10);

                        // .NET 5

                        string[] testValues = { null, null, null, null, "7FFFFFFF", "2147483647", "17777777777", "1111111111111111111111111111111", "80000000", "-2147483648", "20000000000", "10000000000000000000000000000000", };
                        int[] testBases = { 10, 2, 8, 16, 16, 10, 8, 2, 16, 10, 8, 2, };
                        int[] expectedValues = { 0, 0, 0, 0, int.MaxValue, int.MaxValue, int.MaxValue, int.MaxValue, int.MinValue, int.MinValue, int.MinValue, int.MinValue, };
                        for (int i = 0; i < testValues.Length; i++)
                        {
                            yield return new TestCaseData(expectedValues[i], testValues[i], testBases[i]);
                        }

                        // Custom

                        yield return new TestCaseData(1, "1", 16);
                        yield return new TestCaseData(-1, "ffffffff", 16);
                        yield return new TestCaseData(2147483647, "7fffffff", 16);
                        yield return new TestCaseData(-2147483648, "-80000000", 16); // Special case: In Java, we allow the negative sign for the smallest negative number
                        yield return new TestCaseData(-2147483648, "80000000", 16);  // In .NET, it should parse without the negative sign to the same value (in .NET the negative sign is not allowed)
                        yield return new TestCaseData(-2147483647, "80000001", 16);

                        // Surrogate Pairs (.NET only supports ASCII, but Java supports these)

                        yield return new TestCaseData(999, "𝟗𑃹𝟫", 10);
                        yield return new TestCaseData(5783, "𝟓𝟕𝟖𝟑", 10);
                        yield return new TestCaseData(479, "𑁪𑁭𑁯", 10);

                        // Non-decimal needs to be tested separately because they go through
                        // a separate execution path
                        yield return new TestCaseData(2457, "𝟗𑃹𝟫", 16);
                        yield return new TestCaseData(22403, "𝟓𝟕𝟖𝟑", 16);
                        yield return new TestCaseData(1145, "𑁪𑁭𑁯", 16);
                    }
                }

                #endregion TestParse_CharSequence_Int32_Data

                #region TestParse_CharSequence_Int32_ForException_Data

                public static IEnumerable<TestCaseData> TestParse_CharSequence_Int32_ForException_Data
                {
                    get
                    {
                        // JDK 8

                        yield return new TestCaseData(typeof(FormatException), "", 10);
                        yield return new TestCaseData(typeof(FormatException), "\u0000", 10);
                        yield return new TestCaseData(typeof(FormatException), "\u002f", 10);
                        yield return new TestCaseData(typeof(FormatException), "+", 10);
                        yield return new TestCaseData(typeof(FormatException), "-", 10);
                        yield return new TestCaseData(typeof(FormatException), "++", 10);
                        yield return new TestCaseData(typeof(FormatException), "+-", 10);
                        yield return new TestCaseData(typeof(FormatException), "-+", 10);
                        yield return new TestCaseData(typeof(FormatException), "--", 10);
                        yield return new TestCaseData(typeof(FormatException), "++100", 10);
                        yield return new TestCaseData(typeof(FormatException), "--100", 10);
                        yield return new TestCaseData(typeof(FormatException), "+-6", 10);
                        yield return new TestCaseData(typeof(FormatException), "-+6", 10);
                        yield return new TestCaseData(typeof(FormatException), "*100", 10);

                        // Harmony (Test_parseIntLjava_lang_String())

                        yield return new TestCaseData(typeof(FormatException), "0x1", 10);
                        yield return new TestCaseData(typeof(FormatException), "9.2", 10);
                        yield return new TestCaseData(typeof(FormatException), "", 10);
                        //yield return new TestCaseData(typeof(ArgumentNullException), null, 10); // .NET returns 0 in this case

                        // Harmony (Test_parseIntLjava_lang_String2())

                        yield return new TestCaseData(typeof(OverflowException), "999999999999", 10);
                        yield return new TestCaseData(typeof(OverflowException), "2147483648", 10);
                        yield return new TestCaseData(typeof(OverflowException), "-2147483649", 10);

                        // Harmony (Test_parseIntLjava_lang_StringI())

                        yield return new TestCaseData(typeof(FormatException), "0x1", 10);
                        yield return new TestCaseData(typeof(FormatException), "9.2", 10);
                        yield return new TestCaseData(typeof(FormatException), "", 10); // J2N: .NET throws ArgumentOutOfRangeException rather than FormatException in this case, but since it is inconsistent with int.Parse() we are going with FormatException.
                        //yield return new TestCaseData(typeof(ArgumentNullException), null, 10); // J2N: Match .NET behavior where null will result in 0

                        // Harmony (Test_parseIntLjava_lang_StringI2())

                        yield return new TestCaseData(typeof(FormatException), "FFFF", 10);
                        yield return new TestCaseData(typeof(OverflowException), "2147483648", 10);
                        yield return new TestCaseData(typeof(OverflowException), "-2147483649", 10);
                        yield return new TestCaseData(typeof(OverflowException), "-80000001", 16);
                        yield return new TestCaseData(typeof(OverflowException), "9999999999", 10);

                        // .NET 5

                        string[] overflowValues = { "2147483648", "-2147483649", "111111111111111111111111111111111", "1FFFFffff", "777777777777" };
                        int[] overflowBases = { 10, 10, 2, 16, 8 };
                        for (int i = 0; i < overflowValues.Length; i++)
                        {
                            yield return new TestCaseData(typeof(OverflowException), overflowValues[i], overflowBases[i]);
                        }

                        string[] formatExceptionValues = { "12", "ffffffffffffffffffff" };
                        int[] formatExceptionBases = { 2, 8 };
                        for (int i = 0; i < formatExceptionValues.Length; i++)
                        {
                            yield return new TestCaseData(typeof(FormatException), formatExceptionValues[i], formatExceptionBases[i]);
                        }

                        string[] argumentExceptionValues = { "10", /*"11",*/ "abba" /*, "-ab"*/ }; // Negative signs are allowed, so last test would pass in Java
                        int[] argumentExceptionBases = { -1, /*3,*/ 0 /*, 16*/ };                  // Radix 3 is valid so 2nd test would pass in Java
                        for (int i = 0; i < argumentExceptionValues.Length; i++)
                        {
                            yield return new TestCaseData(typeof(ArgumentOutOfRangeException), argumentExceptionValues[i], argumentExceptionBases[i]);
                        }
                    }
                }

                #endregion TestParse_CharSequence_Int32_ForException_Data

                #region TestParse_CharSequence_Int32_Int32_Int32_Data

                public static IEnumerable<TestCaseData> TestParse_CharSequence_Int32_Int32_Int32_Data
                {
                    get
                    {
                        // JDK 8

                        yield return new TestCaseData(0, "test-00000", 4, 10 - 4, 10);
                        yield return new TestCaseData(-12345, "test-12345", 4, 10 - 4, 10);
                        yield return new TestCaseData(12345, "xx12345yy", 2, 7 - 2, 10);
                        yield return new TestCaseData(15, "xxFyy", 2, 3 - 2, 16);

                        yield return new TestCaseData(12345, "xx1234567yy", 2, 5, 10);

                        // Harmony (Test_parseIntLjava_lang_String())

                        yield return new TestCaseData(0, "0", 0, 1, 10);
                        yield return new TestCaseData(1, "1", 0, 1, 10);
                        yield return new TestCaseData(-1, "-1", 0, 2, 10);
                        //yield return new TestCaseData(0, null, 0, 1, 10); // J2N: Throw ArgumentNullException in this case

                        // Harmony (Test_parseIntLjava_lang_String2())

                        yield return new TestCaseData(-8900, "-8900", 0, 5, 10);
                        yield return new TestCaseData(0, "0", 0, 1, 10);
                        yield return new TestCaseData(unchecked((int)0x80000000), "-2147483648", 0, 11, 10);
                        yield return new TestCaseData(0x7fffffff, "2147483647", 0, 10, 10);

                        // Harmony (Test_parseIntLjava_lang_StringI())

                        yield return new TestCaseData(0, "0", 0, 1, 10);
                        yield return new TestCaseData(1, "1", 0, 1, 10);
                        yield return new TestCaseData(-1, "-1", 0, 2, 10);

                        //must be consistent with Character.digit()
                        yield return new TestCaseData(Character.Digit('1', 2), "1", 0, 1, 2);
                        yield return new TestCaseData(Character.Digit('F', 16), "F", 0, 1, 16);

                        //yield return new TestCaseData(0, null, 0, 1, 10); // J2N: Match Java behavior where null will result in ArgumentNullException

                        // Harmony (Test_parseIntLjava_lang_StringI2())

                        yield return new TestCaseData(-8000, "-8000", 0, 5, 10);
                        yield return new TestCaseData(255, "FF", 0, 2, 16);
                        yield return new TestCaseData(16, "20", 0, 2, 8);
                        yield return new TestCaseData(0, "0", 0, 1, 16);
                        yield return new TestCaseData(unchecked((int)0x80000000), "-80000000", 0, 9, 16);
                        yield return new TestCaseData(0x7fffffff, "7fffffff", 0, 8, 16);
                        yield return new TestCaseData(0, "0", 0, 1, 10);
                        yield return new TestCaseData(unchecked((int)0x80000000), "-2147483648", 0, 11, 10);
                        yield return new TestCaseData(0x7fffffff, "2147483647", 0, 10, 10);

                        // .NET 5

                        string[] testValues = { /*null, null, null, null,*/ "7FFFFFFF", "2147483647", "17777777777", "1111111111111111111111111111111", "80000000", "-2147483648", "20000000000", "10000000000000000000000000000000", };
                        int[] testBases = { /*10, 2, 8, 16,*/ 16, 10, 8, 2, 16, 10, 8, 2, };
                        int[] expectedValues = { /*0, 0, 0, 0,*/ int.MaxValue, int.MaxValue, int.MaxValue, int.MaxValue, int.MinValue, int.MinValue, int.MinValue, int.MinValue, };
                        for (int i = 0; i < testValues.Length; i++)
                        {
                            yield return new TestCaseData(expectedValues[i], testValues[i], 0, testValues[i].Length, testBases[i]);
                        }

                        // Custom

                        yield return new TestCaseData(1, "1", 0, 1, 16);
                        yield return new TestCaseData(-1, "ffffffff", 0, 8, 16);
                        yield return new TestCaseData(2147483647, "7fffffff", 0, 8, 16);
                        yield return new TestCaseData(-2147483648, "-80000000", 0, 9, 16); // Special case: In Java, we allow the negative sign for the smallest negative number
                        yield return new TestCaseData(-2147483648, "80000000", 0, 8, 16);  // In .NET, it should parse without the negative sign to the same value (in .NET the negative sign is not allowed)
                        yield return new TestCaseData(-2147483647, "80000001", 0, 8, 16);

                        // Surrogate Pairs (.NET only supports ASCII, but Java supports these)

                        yield return new TestCaseData(999, "𝟗𑃹𝟫", 0, 6, 10);
                        yield return new TestCaseData(5783, "𝟓𝟕𝟖𝟑", 0, 8, 10);
                        yield return new TestCaseData(479, "𑁪𑁭𑁯", 0, 6, 10);

                        // Non-decimal needs to be tested separately because they go through
                        // a separate execution path
                        yield return new TestCaseData(2457, "𝟗𑃹𝟫", 0, 6, 16);
                        yield return new TestCaseData(22403, "𝟓𝟕𝟖𝟑", 0, 8, 16);
                        yield return new TestCaseData(1145, "𑁪𑁭𑁯", 0, 6, 16);
                    }
                }

                #endregion TestParse_CharSequence_Int32_Int32_Int32_Data

                #region TestParse_CharSequence_Int32_Int32_Int32_ForException_Data

                public static IEnumerable<TestCaseData> TestParse_CharSequence_Int32_Int32_Int32_ForException_Data
                {
                    get
                    {
                        // JDK 8

                        yield return new TestCaseData(typeof(FormatException), "", 0, 0 - 0, 10); // J2N: .NET throws ArgumentOutOfRangeException rather than FormatException in this case, but since it is inconsistent with int.Parse() we are going with FormatException.
                        yield return new TestCaseData(typeof(FormatException), "+-6", 0, 3 - 0, 10);
                        yield return new TestCaseData(typeof(FormatException), "1000000", 7, 7 - 7, 10); // J2N: .NET throws ArgumentOutOfRangeException rather than FormatException in this case, but since it is inconsistent with int.Parse() we are going with FormatException.
                        yield return new TestCaseData(typeof(ArgumentOutOfRangeException), "1000000", 0, 2 - 0, Character.MaxRadix + 1);
                        yield return new TestCaseData(typeof(ArgumentOutOfRangeException), "1000000", 0, 2 - 0, Character.MinRadix - 1);

                        yield return new TestCaseData(typeof(ArgumentOutOfRangeException), "", 1, 1 - 1, 10);
                        yield return new TestCaseData(typeof(ArgumentOutOfRangeException), "1000000", 10, 4 - 10, 10);
                        yield return new TestCaseData(typeof(ArgumentOutOfRangeException), "1000000", 10, 2 - 10, Character.MaxRadix + 1);
                        yield return new TestCaseData(typeof(ArgumentOutOfRangeException), "1000000", 10, 2 - 10, Character.MinRadix - 1);
                        yield return new TestCaseData(typeof(ArgumentOutOfRangeException), "1000000", -1, 2 - -1, Character.MaxRadix + 1);
                        yield return new TestCaseData(typeof(ArgumentOutOfRangeException), "1000000", -1, 2 - -1, Character.MinRadix - 1);
                        yield return new TestCaseData(typeof(ArgumentOutOfRangeException), "-1", 0, 3 - 0, 10);
                        yield return new TestCaseData(typeof(ArgumentOutOfRangeException), "-1", 2, 3 - 2, 10);
                        yield return new TestCaseData(typeof(ArgumentOutOfRangeException), "-1", -1, 2 - -1, 10);

                        yield return new TestCaseData(typeof(ArgumentNullException), null, 0, 1 - 0, 10);
                        yield return new TestCaseData(typeof(ArgumentNullException), null, -1, 0 - -1, 100);
                        yield return new TestCaseData(typeof(ArgumentNullException), null, 0, 0 - 0, 10);
                        yield return new TestCaseData(typeof(ArgumentNullException), null, 0, -1 - 0, 10);
                        yield return new TestCaseData(typeof(ArgumentNullException), null, -1, -1 - -1, -1);

                        // Harmony (Test_parseIntLjava_lang_String())

                        yield return new TestCaseData(typeof(FormatException), "0x1", 0, 3, 10);
                        yield return new TestCaseData(typeof(FormatException), "9.2", 0, 3, 10);
                        yield return new TestCaseData(typeof(FormatException), "", 0, 0, 10);
                        yield return new TestCaseData(typeof(ArgumentNullException), null, 0, 1, 10); // Unlike non-indexed overload, throw instead of returning zero

                        // Harmony (Test_parseIntLjava_lang_String2())

                        yield return new TestCaseData(typeof(OverflowException), "999999999999", 0, 12, 10);
                        yield return new TestCaseData(typeof(OverflowException), "2147483648", 0, 10, 10);
                        yield return new TestCaseData(typeof(OverflowException), "-2147483649", 0, 11, 10);

                        // Harmony (Test_parseIntLjava_lang_StringI())

                        yield return new TestCaseData(typeof(FormatException), "0x1", 0, 3, 10);
                        yield return new TestCaseData(typeof(FormatException), "9.2", 0, 3, 10);
                        yield return new TestCaseData(typeof(FormatException), "", 0, 0, 10); // J2N: .NET throws ArgumentOutOfRangeException rather than FormatException in this case, but since it is inconsistent with int.Parse() we are going with FormatException.
                        yield return new TestCaseData(typeof(ArgumentNullException), null, 0, 1, 10); // Unlike non-indexed overload, throw instead of returning zero

                        // Harmony (Test_parseIntLjava_lang_StringI2())

                        yield return new TestCaseData(typeof(FormatException), "FFFF", 0, 4, 10);
                        yield return new TestCaseData(typeof(OverflowException), "2147483648", 0, 10, 10);
                        yield return new TestCaseData(typeof(OverflowException), "-2147483649", 0, 11, 10);
                        yield return new TestCaseData(typeof(OverflowException), "-80000001", 0, 9, 16);
                        yield return new TestCaseData(typeof(OverflowException), "9999999999", 0, 10, 10);

                        // .NET 5

                        string[] overflowValues = { "2147483648", "-2147483649", "111111111111111111111111111111111", "1FFFFffff", "777777777777" };
                        int[] overflowBases = { 10, 10, 2, 16, 8 };
                        for (int i = 0; i < overflowValues.Length; i++)
                        {
                            yield return new TestCaseData(typeof(OverflowException), overflowValues[i], 0, overflowValues[i].Length, overflowBases[i]);
                        }

                        string[] formatExceptionValues = { "12", "ffffffffffffffffffff" };
                        int[] formatExceptionBases = { 2, 8 };
                        for (int i = 0; i < formatExceptionValues.Length; i++)
                        {
                            yield return new TestCaseData(typeof(FormatException), formatExceptionValues[i], 0, formatExceptionValues[i].Length, formatExceptionBases[i]);
                        }

                        string[] argumentExceptionValues = { "10", /*"11",*/ "abba" /*, "-ab"*/ }; // Negative signs are allowed, so last test would pass in Java
                        int[] argumentExceptionBases = { -1, /*3,*/ 0 /*, 16*/ };                  // Radix 3 is valid so 2nd test would pass in Java
                        for (int i = 0; i < argumentExceptionValues.Length; i++)
                        {
                            yield return new TestCaseData(typeof(ArgumentOutOfRangeException), argumentExceptionValues[i], 0, argumentExceptionValues[i].Length, argumentExceptionBases[i]);
                        }

                        // Custom

                        yield return new TestCaseData(typeof(FormatException), "xx  34567yy", 2, 5, 10); // spaces in range are not allowed
                    }
                }

                #endregion TestParse_CharSequence_Int32_Int32_Int32_ForException_Data

                #region TestParse_CharSequence_NumberStyle_IFormatProvider_Data

                public static IEnumerable<TestCaseData> TestParse_CharSequence_NumberStyle_IFormatProvider_Data
                {
                    get
                    {
                        // JDK 8

                        yield return new TestCaseData(+100, "+100", NumberStyle.Integer, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(-100, "-100", NumberStyle.Integer, NumberFormatInfo.InvariantInfo);

                        yield return new TestCaseData(0, "+0", NumberStyle.Integer, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(0, "-0", NumberStyle.Integer, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(0, "+00000", NumberStyle.Integer, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(0, "-00000", NumberStyle.Integer, NumberFormatInfo.InvariantInfo);

                        yield return new TestCaseData(0, "0", NumberStyle.Integer, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(1, "1", NumberStyle.Integer, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(9, "9", NumberStyle.Integer, NumberFormatInfo.InvariantInfo);

                        // Harmony (Test_parseIntLjava_lang_String())

                        yield return new TestCaseData(0, "0", NumberStyle.Integer, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(1, "1", NumberStyle.Integer, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(-1, "-1", NumberStyle.Integer, NumberFormatInfo.InvariantInfo);
                        //yield return new TestCaseData(0, null, NumberStyle.Integer, NumberFormatInfo.InvariantInfo); // .NET returns 0 in this case (JDK throws)

                        // Harmony (Test_parseIntLjava_lang_String2())

                        yield return new TestCaseData(-8900, "-8900", NumberStyle.Integer, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(0, "0", NumberStyle.Integer, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(unchecked((int)0x80000000), "-2147483648", NumberStyle.Integer, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(0x7fffffff, "2147483647", NumberStyle.Integer, NumberFormatInfo.InvariantInfo);

                        // Harmony (Test_parseIntLjava_lang_StringI())

                        yield return new TestCaseData(0, "0", NumberStyle.Integer, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(1, "1", NumberStyle.Integer, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(-1, "-1", NumberStyle.Integer, NumberFormatInfo.InvariantInfo);

                        //yield return new TestCaseData(0, null, NumberStyle.Integer, NumberFormatInfo.InvariantInfo); // J2N: Match .NET behavior where null will result in 0

                        // Harmony (Test_parseIntLjava_lang_StringI2())

                        yield return new TestCaseData(-8000, "-8000", NumberStyle.Integer, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(255, "FF", NumberStyle.HexNumber, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(0, "0", NumberStyle.HexNumber, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(unchecked((int)0x80000000), /*"-80000000"*/ "80000000", NumberStyle.HexNumber, NumberFormatInfo.InvariantInfo); // 2's complement required
                        yield return new TestCaseData(0x7fffffff, "7fffffff", NumberStyle.HexNumber, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(0, "0", NumberStyle.Integer, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(unchecked((int)0x80000000), "-2147483648", NumberStyle.Integer, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(0x7fffffff, "2147483647", NumberStyle.Integer, NumberFormatInfo.InvariantInfo);

                        // Custom

                        yield return new TestCaseData(0x007b, "0x007b", NumberStyle.HexNumber, NumberFormatInfo.InvariantInfo); // J2N: Allow 0x to be specified in the string
                        yield return new TestCaseData(0x007b, "0x007bL", NumberStyle.HexNumber | NumberStyle.AllowTypeSpecifier, NumberFormatInfo.InvariantInfo); // J2N: Allow 0x to be specified in the string
                        yield return new TestCaseData(100, "64L", NumberStyle.HexNumber | NumberStyle.AllowTypeSpecifier, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(0x007b, "0x007bL  ", NumberStyle.HexNumber | NumberStyle.AllowTypeSpecifier, NumberFormatInfo.InvariantInfo); // J2N: Allow trailing whitespace

                        // Tests for AllowTypeSpecifier
                        yield return new TestCaseData(100, "100L", NumberStyle.Integer | NumberStyle.AllowTypeSpecifier, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(100, "100U", NumberStyle.Integer | NumberStyle.AllowTypeSpecifier, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(100, "100l", NumberStyle.Integer | NumberStyle.AllowTypeSpecifier, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(100, "100u", NumberStyle.Integer | NumberStyle.AllowTypeSpecifier, NumberFormatInfo.InvariantInfo);

                        yield return new TestCaseData(100, "100UL", NumberStyle.Integer | NumberStyle.AllowTypeSpecifier, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(100, "100ul", NumberStyle.Integer | NumberStyle.AllowTypeSpecifier, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(100, "100Ul", NumberStyle.Integer | NumberStyle.AllowTypeSpecifier, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(100, "100uL", NumberStyle.Integer | NumberStyle.AllowTypeSpecifier, NumberFormatInfo.InvariantInfo);

                        yield return new TestCaseData(100, "100LU", NumberStyle.Integer | NumberStyle.AllowTypeSpecifier, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(100, "100lu", NumberStyle.Integer | NumberStyle.AllowTypeSpecifier, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(100, "100Lu", NumberStyle.Integer | NumberStyle.AllowTypeSpecifier, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(100, "100lU", NumberStyle.Integer | NumberStyle.AllowTypeSpecifier, NumberFormatInfo.InvariantInfo);

                        yield return new TestCaseData(100, "100D", NumberStyle.Integer | NumberStyle.AllowTypeSpecifier, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(100, "100d", NumberStyle.Integer | NumberStyle.AllowTypeSpecifier, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(100, "100F", NumberStyle.Integer | NumberStyle.AllowTypeSpecifier, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(100, "100f", NumberStyle.Integer | NumberStyle.AllowTypeSpecifier, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(100, "100M", NumberStyle.Integer | NumberStyle.AllowTypeSpecifier, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(100, "100m", NumberStyle.Integer | NumberStyle.AllowTypeSpecifier, NumberFormatInfo.InvariantInfo);

                        yield return new TestCaseData(100, "100.0D", NumberStyle.Integer | NumberStyle.AllowDecimalPoint | NumberStyle.AllowTypeSpecifier, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(100, "100.0d", NumberStyle.Integer | NumberStyle.AllowDecimalPoint | NumberStyle.AllowTypeSpecifier, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(100, "100.0F", NumberStyle.Integer | NumberStyle.AllowDecimalPoint | NumberStyle.AllowTypeSpecifier, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(100, "100.0f", NumberStyle.Integer | NumberStyle.AllowDecimalPoint | NumberStyle.AllowTypeSpecifier, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(100, "100.0M", NumberStyle.Integer | NumberStyle.AllowDecimalPoint | NumberStyle.AllowTypeSpecifier, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(100, "100.0m", NumberStyle.Integer | NumberStyle.AllowDecimalPoint | NumberStyle.AllowTypeSpecifier, NumberFormatInfo.InvariantInfo);
                    }
                }

                #endregion TestParse_CharSequence_NumberStyle_IFormatProvider_Data

                #region TestParse_CharSequence_NumberStyle_IFormatProvider_ForException_Data

                public static IEnumerable<TestCaseData> TestParse_CharSequence_NumberStyle_IFormatProvider_ForException_Data
                {
                    get
                    {
                        // JDK 8

                        yield return new TestCaseData(typeof(FormatException), "", NumberStyle.Integer, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(typeof(FormatException), "\u0000", NumberStyle.Integer, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(typeof(FormatException), "\u002f", NumberStyle.Integer, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(typeof(FormatException), "+", NumberStyle.Integer, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(typeof(FormatException), "-", NumberStyle.Integer, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(typeof(FormatException), "++", NumberStyle.Integer, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(typeof(FormatException), "+-", NumberStyle.Integer, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(typeof(FormatException), "-+", NumberStyle.Integer, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(typeof(FormatException), "--", NumberStyle.Integer, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(typeof(FormatException), "++100", NumberStyle.Integer, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(typeof(FormatException), "--100", NumberStyle.Integer, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(typeof(FormatException), "+-6", NumberStyle.Integer, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(typeof(FormatException), "-+6", NumberStyle.Integer, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(typeof(FormatException), "*100", NumberStyle.Integer, NumberFormatInfo.InvariantInfo);

                        // Harmony (Test_parseIntLjava_lang_String())

                        yield return new TestCaseData(typeof(FormatException), "0x1", NumberStyle.Integer, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(typeof(FormatException), "9.2", NumberStyle.Integer, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(typeof(FormatException), "", NumberStyle.Integer, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(typeof(ArgumentNullException), null, NumberStyle.Integer, NumberFormatInfo.InvariantInfo);

                        // Harmony (Test_parseIntLjava_lang_String2())

                        yield return new TestCaseData(typeof(OverflowException), "999999999999", NumberStyle.Integer, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(typeof(OverflowException), "2147483648", NumberStyle.Integer, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(typeof(OverflowException), "-2147483649", NumberStyle.Integer, NumberFormatInfo.InvariantInfo);

                        // Harmony (Test_parseIntLjava_lang_StringI())

                        yield return new TestCaseData(typeof(FormatException), "0x1", NumberStyle.Integer, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(typeof(FormatException), "9.2", NumberStyle.Integer, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(typeof(FormatException), "", NumberStyle.Integer, NumberFormatInfo.InvariantInfo); // J2N: .NET throws ArgumentOutOfRangeException rather than FormatException in this case, but since it is inconsistent with int.Parse() we are going with FormatException.
                        yield return new TestCaseData(typeof(ArgumentNullException), null, NumberStyle.Integer, NumberFormatInfo.InvariantInfo);

                        // Harmony (Test_parseIntLjava_lang_StringI2())

                        yield return new TestCaseData(typeof(FormatException), "FFFF", NumberStyle.Integer, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(typeof(OverflowException), "2147483648", NumberStyle.Integer, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(typeof(OverflowException), "-2147483649", NumberStyle.Integer, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(typeof(OverflowException), /*"-80000001"*/ "100000000", NumberStyle.HexNumber, NumberFormatInfo.InvariantInfo); // J2N:  2's complement required
                        yield return new TestCaseData(typeof(OverflowException), "9999999999", NumberStyle.Integer, NumberFormatInfo.InvariantInfo);

                        // Custom

                        // Tests for AllowTypeSpecifier

                        yield return new TestCaseData(typeof(FormatException), "100L", NumberStyle.Integer, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(typeof(FormatException), "100U", NumberStyle.Integer, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(typeof(FormatException), "100l", NumberStyle.Integer, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(typeof(FormatException), "100u", NumberStyle.Integer, NumberFormatInfo.InvariantInfo);

                        yield return new TestCaseData(typeof(FormatException), "100UL", NumberStyle.Integer, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(typeof(FormatException), "100ul", NumberStyle.Integer, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(typeof(FormatException), "100Ul", NumberStyle.Integer, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(typeof(FormatException), "100uL", NumberStyle.Integer, NumberFormatInfo.InvariantInfo);

                        yield return new TestCaseData(typeof(FormatException), "100LU", NumberStyle.Integer, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(typeof(FormatException), "100lu", NumberStyle.Integer, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(typeof(FormatException), "100Lu", NumberStyle.Integer, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(typeof(FormatException), "100lU", NumberStyle.Integer, NumberFormatInfo.InvariantInfo);

                        // These are not valid ways of specifying long or unsigned types (they do not compile with a decimal point)
                        yield return new TestCaseData(typeof(FormatException), "100.0L", NumberStyle.Integer | NumberStyle.AllowDecimalPoint | NumberStyle.AllowTypeSpecifier, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(typeof(FormatException), "100.0U", NumberStyle.Integer | NumberStyle.AllowDecimalPoint | NumberStyle.AllowTypeSpecifier, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(typeof(FormatException), "100.0l", NumberStyle.Integer | NumberStyle.AllowDecimalPoint | NumberStyle.AllowTypeSpecifier, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(typeof(FormatException), "100.0u", NumberStyle.Integer | NumberStyle.AllowDecimalPoint | NumberStyle.AllowTypeSpecifier, NumberFormatInfo.InvariantInfo);

                        yield return new TestCaseData(typeof(FormatException), "100.0UL", NumberStyle.Integer | NumberStyle.AllowDecimalPoint | NumberStyle.AllowTypeSpecifier, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(typeof(FormatException), "100.0ul", NumberStyle.Integer | NumberStyle.AllowDecimalPoint | NumberStyle.AllowTypeSpecifier, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(typeof(FormatException), "100.0Ul", NumberStyle.Integer | NumberStyle.AllowDecimalPoint | NumberStyle.AllowTypeSpecifier, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(typeof(FormatException), "100.0uL", NumberStyle.Integer | NumberStyle.AllowDecimalPoint | NumberStyle.AllowTypeSpecifier, NumberFormatInfo.InvariantInfo);

                        yield return new TestCaseData(typeof(FormatException), "100.0LU", NumberStyle.Integer | NumberStyle.AllowDecimalPoint | NumberStyle.AllowTypeSpecifier, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(typeof(FormatException), "100.0lu", NumberStyle.Integer | NumberStyle.AllowDecimalPoint | NumberStyle.AllowTypeSpecifier, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(typeof(FormatException), "100.0Lu", NumberStyle.Integer | NumberStyle.AllowDecimalPoint | NumberStyle.AllowTypeSpecifier, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(typeof(FormatException), "100.0lU", NumberStyle.Integer | NumberStyle.AllowDecimalPoint | NumberStyle.AllowTypeSpecifier, NumberFormatInfo.InvariantInfo);

                        // We need to use AllowDecimalPoint for these to work.
                        yield return new TestCaseData(typeof(FormatException), "100.0D", NumberStyle.Integer | NumberStyle.AllowTypeSpecifier, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(typeof(FormatException), "100.0d", NumberStyle.Integer | NumberStyle.AllowTypeSpecifier, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(typeof(FormatException), "100.0F", NumberStyle.Integer | NumberStyle.AllowTypeSpecifier, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(typeof(FormatException), "100.0f", NumberStyle.Integer | NumberStyle.AllowTypeSpecifier, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(typeof(FormatException), "100.0M", NumberStyle.Integer | NumberStyle.AllowTypeSpecifier, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(typeof(FormatException), "100.0m", NumberStyle.Integer | NumberStyle.AllowTypeSpecifier, NumberFormatInfo.InvariantInfo);
                    }
                }

                #endregion TestParse_CharSequence_NumberStyle_IFormatProvider_ForException_Data
            }

            #endregion ParseTestCase

            // Radix-based parsing

            #region Parse_CharSequence_Int32

            public abstract class Parse_CharSequence_Int32_TestCase : ParseTestCase
            {
                protected abstract int GetResult(string value, int radix);


                [TestCaseSource(typeof(ParseTestCase), "TestParse_CharSequence_Int32_Data")]
                public virtual void TestParse_CharSequence_Int32(int expected, string value, int radix)
                {
                    var actual = GetResult(value, radix);
                    assertEquals($"Int32.Parse(string, IFormatProvider) failed. String: \"{value}\" Result: {actual}", expected, actual);
                }


                [TestCaseSource(typeof(ParseTestCase), "TestParse_CharSequence_Int32_ForException_Data")]
                public virtual void TestParse_CharSequence_Int32_ForException(Type expectedExceptionType, string value, int radix)
                {
                    Assert.Throws(expectedExceptionType, () => GetResult(value, radix));
                }
            }

            public class Parse_String_Int32 : Parse_CharSequence_Int32_TestCase
            {
                protected override int GetResult(string value, int radix)
                {
                    return Int32.Parse(value, radix);
                }
            }

            // J2N: ReadOnlySpan<char> not supported at this time on this overload (not supported in .NET anyway)
            //#if FEATURE_SPAN
            //            public class Parse_ReadOnlySpan_Int32 : Parse_CharSequence_Int32_TestCase
            //            {
            //                protected override int GetResult(string s, int radix)
            //                {
            //                    return Int32.Parse(s.AsSpan(), radix);
            //                }
            //            }
            //#endif

            #endregion Parse_CharSequence_Int32

            #region Parse_CharSequence_Int32_Int32_Int32

            public abstract class Parse_CharSequence_Int32_Int32_Int32_TestCase : ParseTestCase
            {
                protected virtual bool IsNullableType => true;

                protected abstract int GetResult(string value, int startIndex, int length, int radix);


                [TestCaseSource(typeof(ParseTestCase), "TestParse_CharSequence_Int32_Int32_Int32_Data")]

                public virtual void TestParse_CharSequence_Int32_Int32_Int32(int expected, string value, int startIndex, int length, int radix)
                {
                    Assume.That(IsNullableType || (!IsNullableType && value != null), "null is not supported by this character sequence type.");

                    var actual = GetResult(value, startIndex, length, radix);
                    assertEquals($"Int32.Parse(string, int, int, int) failed. Expected: {expected} String: \"{value}\", startIndex: {startIndex}, length: {length} radix: {radix} Result: {actual}", expected, actual);
                }


                [TestCaseSource(typeof(ParseTestCase), "TestParse_CharSequence_Int32_Int32_Int32_ForException_Data")]
                public virtual void TestParse_CharSequence_Int32_Int32_Int32_ForException(Type expectedExceptionType, string value, int startIndex, int length, int radix)
                {
                    Assume.That(IsNullableType || (!IsNullableType && value != null), "null is not supported by this character sequence type.");

                    Assert.Throws(expectedExceptionType, () => GetResult(value, startIndex, length, radix));
                }
            }

            public class Parse_String_Int32_Int32_Int32_Int32 : Parse_CharSequence_Int32_Int32_Int32_TestCase
            {
                protected override int GetResult(string value, int startIndex, int length, int radix)
                {
                    return Int32.Parse(value, startIndex, length, radix);
                }
            }

            public class Parse_CharArray_Int32_Int32_Int32_Int32 : Parse_CharSequence_Int32_Int32_Int32_TestCase
            {
                protected override int GetResult(string value, int startIndex, int length, int radix)
                {
                    return Int32.Parse(value is null ? null : value.ToCharArray(), startIndex, length, radix);
                }
            }

            public class Parse_StringBuilder_Int32_Int32_Int32_Int32 : Parse_CharSequence_Int32_Int32_Int32_TestCase
            {
                protected override int GetResult(string value, int startIndex, int length, int radix)
                {
                    // NOTE: Although technically the .NET StringBuilder will accept null and convert it to an empty string, we
                    // want to test the method for null here.
                    return Int32.Parse(value is null ? null : new StringBuilder(value), startIndex, length, radix);
                }
            }

            public class Parse_ICharSequence_Int32_Int32_Int32_Int32 : Parse_CharSequence_Int32_Int32_Int32_TestCase
            {
                protected override int GetResult(string value, int startIndex, int length, int radix)
                {
                    return Int32.Parse(value.AsCharSequence(), startIndex, length, radix);
                }
            }

#if FEATURE_SPAN
            public class Parse_ReadOnlySpan_Int32_Int32_Int32_Int32 : Parse_CharSequence_Int32_Int32_Int32_TestCase
            {
                protected override bool IsNullableType => false;

                protected override int GetResult(string value, int startIndex, int length, int radix)
                {
                    return Int32.Parse(value.AsSpan(), startIndex, length, radix);
                }
            }
#endif


            #endregion Parse_CharSequence_Int32_Int32_Int32

            #region TryParse_CharSequence_Int32_Int32_Int32

            public abstract class TryParse_CharSequence_Int32_Int32_Int32_TestCase : ParseTestCase
            {
                protected virtual bool IsNullableType => true;

                protected abstract bool GetResult(string value, int startIndex, int length, int radix, out int result);

                [TestCaseSource(typeof(ParseTestCase), "TestParse_CharSequence_Int32_Int32_Int32_Data")]

                public virtual void TestTryParse_CharSequence_Int32_Int32_Int32(int expected, string value, int startIndex, int length, int radix)
                {
                    Assume.That(IsNullableType || (!IsNullableType && value != null), "null is not supported by this character sequence type.");

                    assertTrue(GetResult(value, startIndex, length, radix, out int actual));
                    assertEquals($"Int32.TryParse(string, int, int, int, out int) failed. Expected: {expected} String: \"{value}\", startIndex: {startIndex}, length: {length} radix: {radix} Result: {actual}", expected, actual);
                }


                [TestCaseSource(typeof(ParseTestCase), "TestParse_CharSequence_Int32_Int32_Int32_ForException_Data")]
                public virtual void TestTryParse_CharSequence_Int32_Int32_Int32_ForException(Type expectedExceptionType, string value, int startIndex, int length, int radix)
                {
                    Assume.That(IsNullableType || (!IsNullableType && value != null), "null is not supported by this character sequence type.");

                    int actual = 0;
                    if (expectedExceptionType != typeof(ArgumentOutOfRangeException))
                    {
                        assertFalse(GetResult(value, startIndex, length, radix, out actual));
                    }
                    else // Actual exception should be thrown
                    {
                        Assert.Throws(expectedExceptionType, () => GetResult(value, startIndex, length, radix, out actual));
                    }
                    assertEquals(0, actual);
                }
            }

            public class TryParse_String_Int32_Int32_Int32_Int32 : TryParse_CharSequence_Int32_Int32_Int32_TestCase
            {
                protected override bool GetResult(string value, int startIndex, int length, int radix, out int result)
                {
                    return Int32.TryParse(value, startIndex, length, radix, out result);
                }
            }

            public class TryParse_CharArray_Int32_Int32_Int32_Int32 : TryParse_CharSequence_Int32_Int32_Int32_TestCase
            {
                protected override bool GetResult(string value, int startIndex, int length, int radix, out int result)
                {
                    return Int32.TryParse(value is null ? null : value.ToCharArray(), startIndex, length, radix, out result);
                }
            }

            public class TryParse_StringBuilder_Int32_Int32_Int32_Int32 : TryParse_CharSequence_Int32_Int32_Int32_TestCase
            {
                protected override bool GetResult(string value, int startIndex, int length, int radix, out int result)
                {
                    return Int32.TryParse(value is null ? null : new StringBuilder(value), startIndex, length, radix, out result);
                }
            }

            public class TryParse_ICharSequence_Int32_Int32_Int32_Int32 : TryParse_CharSequence_Int32_Int32_Int32_TestCase
            {
                protected override bool GetResult(string value, int startIndex, int length, int radix, out int result)
                {
                    return Int32.TryParse(value.AsCharSequence(), startIndex, length, radix, out result);
                }
            }

#if FEATURE_SPAN
            public class TryParse_ReadOnlySpan_Int32_Int32_Int32_Int32 : TryParse_CharSequence_Int32_Int32_Int32_TestCase
            {
                protected override bool IsNullableType => false;

                protected override bool GetResult(string value, int startIndex, int length, int radix, out int result)
                {
                    return Int32.TryParse(value.AsSpan(), startIndex, length, radix, out result);
                }
            }
#endif

            #endregion TryParse_CharSequence_Int32_Int32_Int32

            #region TryParse_CharSequence_Int32

            public abstract class TryParse_CharSequence_Int32_TestCase : ParseTestCase
            {
                protected abstract bool GetResult(string value, int radix, out int result);


                [TestCaseSource(typeof(ParseTestCase), "TestParse_CharSequence_Int32_Data")]
                public virtual void TestTryParse_CharSequence_Int32(int expected, string value, int radix)
                {
                    assertTrue(GetResult(value, radix, out int actual));
                    assertEquals($"Int32.TryParse(string, out int) failed. String: \"{value}\" Result: {actual}", expected, actual);
                }


                [TestCaseSource(typeof(ParseTestCase), "TestParse_CharSequence_Int32_ForException_Data")]
                public virtual void TestTryParse_CharSequence_Int32_ForException(Type expectedExceptionType, string value, int radix)
                {
                    int actual = 0;
                    if (expectedExceptionType != typeof(ArgumentOutOfRangeException))
                    {
                        assertFalse(GetResult(value, radix, out actual));
                    }
                    else // Actual exception should be thrown
                    {
                        Assert.Throws(expectedExceptionType, () => GetResult(value, radix, out actual));
                    }
                    assertEquals(0, actual);
                }
            }

            public class TryParse_String_Int32 : TryParse_CharSequence_Int32_TestCase
            {
                protected override bool GetResult(string value, int radix, out int result)
                {
                    return Int32.TryParse(value, radix, out result);
                }
            }

            // J2N: ReadOnlySpan<char> not supported at this time on this overload (not supported in .NET anyway)
            //#if FEATURE_SPAN
            //            public class TryParse_ReadOnlySpan_Int32 : TryParse_CharSequence_Int32_TestCase
            //            {
            //                protected override bool GetResult(string s, int radix, out int result)
            //                {
            //                    return Int32.TryParse(s.AsSpan(), radix, out result);
            //                }
            //            }
            //#endif

            #endregion Parse_CharSequence_Int32

            // Culture-aware parsing

            #region Parse_CharSequence_IFormatProvider

            public abstract class Parse_CharSequence_IFormatProvider_TestCase : ParseTestCase
            {
                protected virtual bool IsNullableType => true;

                protected abstract int GetResult(string s, IFormatProvider provider);


                [TestCaseSource(typeof(ParseTestCase), "TestParse_CharSequence_NumberStyle_IFormatProvider_Data")]
                public void TestParse_CharSequence_IFormatProvider(int expected, string value, NumberStyle style, IFormatProvider provider)
                {
                    Assume.That(IsNullableType || (!IsNullableType && value != null), "null is not supported by this character sequence type.");
                    Assume.That((style & ~NumberStyle.Integer) == 0, "Custom NumberStyles are not supported on this overload.");

                    var actual = GetResult(value, provider);
                    assertEquals($"Int32.Parse(string, NumberStyle, IFormatProvider) failed. Expected: {expected} String: \"{value}\", NumberStyle: {style}, provider: {provider} Result: {actual}", expected, actual);
                }

                [TestCaseSource(typeof(ParseTestCase), "TestParse_CharSequence_NumberStyle_IFormatProvider_ForException_Data")]
                public void TestParse_CharSequence_IFormatProvider_ForException(Type expectedExceptionType, string value, NumberStyle style, IFormatProvider provider)
                {
                    Assume.That(IsNullableType || (!IsNullableType && value != null), "null is not supported by this character sequence type.");
                    Assume.That((style & ~NumberStyle.Integer) == 0, "Custom NumberStyles are not supported on this overload.");

                    Assert.Throws(expectedExceptionType, () => GetResult(value, provider));
                }
            }

            public class Parse_String_IFormatProvider_TestCase : Parse_CharSequence_IFormatProvider_TestCase
            {
                protected override int GetResult(string s, IFormatProvider provider)
                {
                    return Int32.Parse(s, provider);
                }
            }

            #endregion Parse_CharSequence_IFormatProvider

            #region Parse_CharSequence_NumberStyle_IFormatProvider

            public abstract class Parse_CharSequence_NumberStyle_IFormatProvider_TestCase : ParseTestCase
            {
                protected virtual bool IsNullableType => true;

                protected abstract int GetResult(string s, NumberStyle style, IFormatProvider provider);


                [TestCaseSource(typeof(ParseTestCase), "TestParse_CharSequence_NumberStyle_IFormatProvider_Data")]
                public void TestParse_CharSequence_NumberStyle_IFormatProvider(int expected, string value, NumberStyle style, IFormatProvider provider)
                {
                    Assume.That(IsNullableType || (!IsNullableType && value != null), "null is not supported by this character sequence type.");

                    var actual = GetResult(value, style, provider);
                    assertEquals($"Int32.Parse(string, NumberStyle, IFormatProvider) failed. Expected: {expected} String: \"{value}\", NumberStyle: {style}, provider: {provider} Result: {actual}", expected, actual);
                }

                [TestCaseSource(typeof(ParseTestCase), "TestParse_CharSequence_NumberStyle_IFormatProvider_ForException_Data")]
                public void TestParse_CharSequence_NumberStyle_IFormatProvider_ForException(Type expectedExceptionType, string value, NumberStyle style, IFormatProvider provider)
                {
                    Assume.That(IsNullableType || (!IsNullableType && value != null), "null is not supported by this character sequence type.");

                    Assert.Throws(expectedExceptionType, () => GetResult(value, style, provider));
                }
            }

            public class Parse_String_NumberStyle_IFormatProvider_TestCase : Parse_CharSequence_NumberStyle_IFormatProvider_TestCase
            {
                protected override int GetResult(string s, NumberStyle style, IFormatProvider provider)
                {
                    return Int32.Parse(s, style, provider);
                }
            }

#if FEATURE_SPAN
            public class Parse_ReadOnlySpan_NumberStyle_IFormatProvider_TestCase : Parse_CharSequence_NumberStyle_IFormatProvider_TestCase
            {
                protected override bool IsNullableType => false;

                protected override int GetResult(string s, NumberStyle style, IFormatProvider provider)
                {
                    return Int32.Parse(s.AsSpan(), style, provider);
                }
            }
#endif

            #endregion Parse_CharSequence_NumberStyle_IFormatProvider

            #region TryParse_CharSequence_NumberStyle_IFormatProvider

            public abstract class TryParse_CharSequence_NumberStyle_IFormatProvider_TestCase : ParseTestCase
            {
                protected virtual bool IsNullableType => true;

                protected abstract bool GetResult(string s, NumberStyle style, IFormatProvider provider, out int result);


                [TestCaseSource(typeof(ParseTestCase), "TestParse_CharSequence_NumberStyle_IFormatProvider_Data")]
                public void TestTryParse_CharSequence_NumberStyle_IFormatProvider(int expected, string value, NumberStyle style, IFormatProvider provider)
                {
                    Assume.That(IsNullableType || (!IsNullableType && value != null), "null is not supported by this character sequence type.");

                    assertTrue(GetResult(value, style, provider, out int actual));
                    assertEquals($"Int32.Parse(string, NumberStyle, IFormatProvider) failed. Expected: {expected} String: \"{value}\", NumberStyle: {style}, provider: {provider} Result: {actual}", expected, actual);
                }

                [TestCaseSource(typeof(ParseTestCase), "TestParse_CharSequence_NumberStyle_IFormatProvider_ForException_Data")]
                public void TestTryParse_CharSequence_NumberStyle_IFormatProvider_ForException(Type expectedExceptionType, string value, NumberStyle style, IFormatProvider provider)
                {
                    Assume.That(IsNullableType || (!IsNullableType && value != null), "null is not supported by this character sequence type.");

                    int actual = 0;
                    if (expectedExceptionType != typeof(ArgumentException))
                    {
                        assertFalse(GetResult(value, style, provider, out actual));
                    }
                    else // Actual exception should be thrown
                    {
                        Assert.Throws(expectedExceptionType, () => GetResult(value, style, provider, out actual));
                    }
                    assertEquals(0, actual);
                }
            }

            public class TryParse_String_NumberStyle_IFormatProvider_TestCase : TryParse_CharSequence_NumberStyle_IFormatProvider_TestCase
            {
                protected override bool GetResult(string s, NumberStyle style, IFormatProvider provider, out int result)
                {
                    return Int32.TryParse(s, style, provider, out result);
                }
            }

#if FEATURE_SPAN
            public class TryParse_ReadOnlySpan_NumberStyle_IFormatProvider_TestCase : TryParse_CharSequence_NumberStyle_IFormatProvider_TestCase
            {
                protected override bool IsNullableType => false;

                protected override bool GetResult(string s, NumberStyle style, IFormatProvider provider, out int result)
                {
                    return Int32.TryParse(s.AsSpan(), style, provider, out result);
                }
            }
#endif

            #endregion Parse_CharSequence_NumberStyle_IFormatProvider

            #region TryParse_CharSequence

            public abstract class TryParse_CharSequence_TestCase : ParseTestCase
            {
                protected virtual bool IsNullableType => true;

                protected abstract bool GetResult(string s, out int result);


                [TestCaseSource(typeof(ParseTestCase), "TestParse_CharSequence_NumberStyle_IFormatProvider_Data")]
                public void TestTryParse_CharSequence(int expected, string value, NumberStyle style, IFormatProvider provider)
                {
                    Assume.That(IsNullableType || (!IsNullableType && value != null), "null is not supported by this character sequence type.");
                    Assume.That((style & ~NumberStyle.Integer) == 0, "Custom NumberStyles are not supported on this overload.");

                    assertTrue(GetResult(value, out int actual));
                    assertEquals($"Int32.Parse(string, NumberStyle, IFormatProvider) failed. Expected: {expected} String: \"{value}\", NumberStyle: {style}, provider: {provider} Result: {actual}", expected, actual);
                }

                [TestCaseSource(typeof(ParseTestCase), "TestParse_CharSequence_NumberStyle_IFormatProvider_ForException_Data")]
                public void TestTryParse_CharSequence_ForException(Type expectedExceptionType, string value, NumberStyle style, IFormatProvider provider)
                {
                    Assume.That(IsNullableType || (!IsNullableType && value != null), "null is not supported by this character sequence type.");
                    Assume.That((style & ~NumberStyle.Integer) == 0, "Custom NumberStyles are not supported on this overload.");

                    int actual = 0;
                    if (expectedExceptionType != typeof(ArgumentException))
                    {
                        assertFalse(GetResult(value, out actual));
                    }
                    else // Actual exception should be thrown
                    {
                        Assert.Throws(expectedExceptionType, () => GetResult(value, out actual));
                    }
                    assertEquals(0, actual);
                }
            }

            public class TryParse_String_TestCase : TryParse_CharSequence_TestCase
            {
                protected override bool GetResult(string s, out int result)
                {
                    return Int32.TryParse(s, out result);
                }
            }

#if FEATURE_SPAN
            public class TryParse_ReadOnlySpan_TestCase : TryParse_CharSequence_TestCase
            {
                protected override bool IsNullableType => false;

                protected override bool GetResult(string s, out int result)
                {
                    return Int32.TryParse(s.AsSpan(), out result);
                }
            }
#endif

            #endregion TryParse_CharSequence


            #region ParseUnsignedTestCase


            public class ParseUnsignedTestCase : TestCase
            {

                public static IEnumerable<TestCaseData> UnsignedIntTestCases
                {
                    get
                    {
                        // JDK 8

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

                public static IEnumerable<TestCaseData> ParseUnsigned_CharSequence_Int32_ForException_Data
                {
                    get
                    {
                        // JDK 8

                        //yield return new TestCaseData(typeof(ArgumentNullException), null, 10); // J2N: We mimic the .NET behavior by allowing null to be converted to 0
                        yield return new TestCaseData(typeof(FormatException), "", 10);
                        yield return new TestCaseData(typeof(OverflowException), "-1", 10);
                        yield return new TestCaseData(typeof(OverflowException), "9223372036854775807", 10); // long.MaxValue
                        yield return new TestCaseData(typeof(OverflowException), "4294967296", 10); // uint.MaxValue + 1
                    }
                }

                public static IEnumerable<TestCaseData> ParseUnsigned_CharSequence_Int32_Int32_Int32_ForException_Data
                {
                    get
                    {
                        // JDK 8

                        yield return new TestCaseData(typeof(FormatException), null, 10); // Expected because we are concatenating a string. In .NET concatenating null to a string is the same as empty string.
                        yield return new TestCaseData(typeof(FormatException), "", 10);
                        yield return new TestCaseData(typeof(OverflowException), "-1", 10);
                        yield return new TestCaseData(typeof(OverflowException), "9223372036854775807", 10); // long.MaxValue
                        yield return new TestCaseData(typeof(OverflowException), "4294967296", 10); // uint.MaxValue + 1
                    }
                }
            }

            #endregion ParseUnsignedTestCase

            #region ParseUnsigned_CharSequence_Int32

            public abstract class ParseUnsigned_CharSequence_Int32_TestCase : ParseUnsignedTestCase
            {
                protected abstract int GetResult(string s, int radix);

                [TestCaseSource(typeof(ParseUnsignedTestCase), "UnsignedIntTestCases")]
                public void TestParseUnsigned_CharSequence_Int32(uint value, int radix, string bigString, int expected)
                {
                    int actual = GetResult(bigString, radix);
                    assertEquals(expected, actual);
                }

                [TestCaseSource(typeof(ParseUnsignedTestCase), "ParseUnsigned_CharSequence_Int32_ForException_Data")]
                public void TestParseUnsigned_CharSequence_Int32_ForException(Type expectedExceptionType, string value, int radix)
                {
                    Assert.Throws(expectedExceptionType, () => GetResult(value, radix));
                }
            }

            public class ParseUnsigned_String_Int32 : ParseUnsigned_CharSequence_Int32_TestCase
            {
                protected override int GetResult(string s, int radix)
                {
                    return Int32.ParseUnsigned(s, radix);
                }
            }

            #endregion ParseUnsigned_CharSequence_Int32

            #region ParseUnsigned_CharSequence_Int32_Int32_Int32

            public abstract class ParseUnsigned_CharSequence_Int32_Int32_Int32_TestCase : ParseUnsignedTestCase
            {
                protected abstract int GetResult(string s, int startIndex, int length, int radix);

                [TestCaseSource(typeof(ParseUnsignedTestCase), "UnsignedIntTestCases")]
                public void TestParseUnsigned_CharSequence_Int32_Int32_Int32(uint value, int radix, string bigString, int expected)
                {
                    int actual = GetResult("prefix" + bigString + "suffix", "prefix".Length, bigString.Length, radix);
                    assertEquals(expected, actual);
                }

                [TestCaseSource(typeof(ParseUnsignedTestCase), "ParseUnsigned_CharSequence_Int32_Int32_Int32_ForException_Data")]
                public void TestParseUnsigned_CharSequence_Int32_Int32_Int32_ForException(Type expectedExceptionType, string value, int radix)
                {
                    Assert.Throws(expectedExceptionType, () => GetResult("prefix" + value + "suffix", "prefix".Length, value?.Length ?? 0, radix));
                }
            }

            public class ParseUnsigned_String_Int32_Int32_Int32 : ParseUnsigned_CharSequence_Int32_Int32_Int32_TestCase
            {
                protected override int GetResult(string s, int startIndex, int length, int radix)
                {
                    return Int32.ParseUnsigned(s, startIndex, length, radix);
                }
            }

            #endregion ParseUnsigned_CharSequence_Int32_Int32_Int32

            #region DecodeTestCase

            public abstract class DecodeTestCase
            {
                public static IEnumerable<TestCaseData> TestDecode_CharSequence_Data
                {
                    get
                    {
                        // JDK 8

                        yield return new TestCaseData("" + int.MinValue, int.MinValue);
                        yield return new TestCaseData("" + int.MaxValue, int.MaxValue);

                        yield return new TestCaseData("10", 10);
                        yield return new TestCaseData("0x10", 16);
                        yield return new TestCaseData("0X10", 16);
                        yield return new TestCaseData("010", 8);
                        yield return new TestCaseData("#10", 16);

                        yield return new TestCaseData("+10", 10);
                        yield return new TestCaseData("+0x10", 16);
                        yield return new TestCaseData("+0X10", 16);
                        yield return new TestCaseData("+010", 8);
                        yield return new TestCaseData("+#10", 16);

                        yield return new TestCaseData("-10", -10);
                        yield return new TestCaseData("-0x10", -16);
                        yield return new TestCaseData("-0X10", -16);
                        yield return new TestCaseData("-010", -8);
                        yield return new TestCaseData("-#10", -16);

                        yield return new TestCaseData(Convert.ToString(int.MinValue, 10), int.MinValue);
                        yield return new TestCaseData(Convert.ToString(int.MaxValue, 10), int.MaxValue);

                        // Harmony (Test_decodeLjava_lang_String())

                        yield return new TestCaseData("0", 0);
                        yield return new TestCaseData("1", 1);
                        yield return new TestCaseData("-1", -1);
                        yield return new TestCaseData("0xF", 0xF);
                        yield return new TestCaseData("#F", 0xF);
                        yield return new TestCaseData("0XF", 0xF);
                        yield return new TestCaseData("07", 07); // NOTE: Technically, .NET doesn't support octal literals, but this is the same value as decimal

                        // Harmony (Test_decodeLjava_lang_String2())

                        yield return new TestCaseData("132233", 132233);
                        yield return new TestCaseData("07654321", /*07654321*/ 2054353);
                        yield return new TestCaseData("#1234567", 0x1234567);
                        yield return new TestCaseData("0xdAd", 0xdad);
                        yield return new TestCaseData("-23", -23);
                        yield return new TestCaseData("0", 0);
                        yield return new TestCaseData("0x0", 0);
                        yield return new TestCaseData("-2147483648", unchecked((int)0x80000000));
                        yield return new TestCaseData("-0x80000000", unchecked((int)0x80000000));
                        yield return new TestCaseData("2147483647", 0x7fffffff);
                        yield return new TestCaseData("0x7fffffff", 0x7fffffff);
                    }
                }

                public static IEnumerable<TestCaseData> TestDecode_CharSequence_ForException_Data
                {
                    get
                    {
                        // JDK 8

                        yield return new TestCaseData(typeof(FormatException), "0x-10", "Integer.decode allows negative sign in wrong position.");
                        yield return new TestCaseData(typeof(FormatException), "0x+10", "Integer.decode allows positive sign in wrong position.");

                        yield return new TestCaseData(typeof(FormatException), "+", "Raw plus sign allowed.");
                        yield return new TestCaseData(typeof(FormatException), "-", "Raw minus sign allowed.");

                        yield return new TestCaseData(typeof(OverflowException), Convert.ToString((long)int.MinValue - 1, 10), "Out of range");
                        yield return new TestCaseData(typeof(OverflowException), Convert.ToString((long)int.MaxValue + 1, 10), "Out of range");

                        yield return new TestCaseData(typeof(FormatException), "", "Empty String");

                        // Harmony (Test_decodeLjava_lang_String())

                        yield return new TestCaseData(typeof(FormatException), "9.2", "Expected FormatException with floating point string.");
                        yield return new TestCaseData(typeof(FormatException), "", "Expected FormatException with empty string.");
                        //undocumented NPE, but seems consistent across JREs
                        yield return new TestCaseData(typeof(ArgumentNullException), null, "Expected NullPointerException with null string.");

                        // Harmony (Test_decodeLjava_lang_String2())

                        yield return new TestCaseData(typeof(FormatException), "0a", "Failed to throw FormatException for \"Oa\"");
                        yield return new TestCaseData(typeof(OverflowException), "2147483648", "Failed to throw exception for MAX_VALUE + 1");
                        yield return new TestCaseData(typeof(OverflowException), "-2147483649", "Failed to throw exception for MIN_VALUE - 1");
                        yield return new TestCaseData(typeof(OverflowException), "0x80000000", "Failed to throw exception for hex MAX_VALUE + 1");
                        yield return new TestCaseData(typeof(OverflowException), "-0x80000001", "Failed to throw exception for hex MIN_VALUE - 1");
                        yield return new TestCaseData(typeof(OverflowException), "9999999999", "Failed to throw exception for 9999999999");
                        yield return new TestCaseData(typeof(FormatException), "-", "Expected exception for -");
                        yield return new TestCaseData(typeof(FormatException), "0x", "Expected exception for 0x");
                        yield return new TestCaseData(typeof(FormatException), "#", "Expected exception for #");
                        yield return new TestCaseData(typeof(FormatException), "x123", "Expected exception for x123");
                        yield return new TestCaseData(typeof(ArgumentNullException), null, "Expected exception for null");
                        yield return new TestCaseData(typeof(FormatException), "", "Expected exception for empty string");
                        yield return new TestCaseData(typeof(FormatException), " ", "Expected exception for single space");

                        // Custom

                        yield return new TestCaseData(typeof(OverflowException), "0xffffffff", "Negative not allowed without negative sign"); // -1 - negative values are not allowed per the docs
                    }
                }
            }

            #endregion DecodeTestCase

            #region Decode_CharSequence

            public abstract class Decode_CharSequence_TestCase : DecodeTestCase
            {
                protected abstract Int32 GetResult(string value);

                [TestCaseSource(typeof(DecodeTestCase), "TestDecode_CharSequence_Data")]
                public virtual void TestDecode_CharSequence(string value, int expected)
                {
                    var actual = GetResult(value);
                    assertEquals($"Int32.Decode(string) failed. String: \"{value}\" Result: {actual}", expected, actual);
                }

                [TestCaseSource(typeof(DecodeTestCase), "TestDecode_CharSequence_ForException_Data")]
                public virtual void TestDecode_CharSequence_ForException(Type expectedExceptionType, string value, string message)
                {
                    Assert.Throws(expectedExceptionType, () => GetResult(value), message);
                }
            }

            public class Decode_String : Decode_CharSequence_TestCase
            {
                protected override Int32 GetResult(string value)
                {
                    return Int32.Decode(value);
                }
            }

            // J2N: ReadOnlySpan<char> not supported at this time on this overload
            //#if FEATURE_SPAN
            //            public class Decode_ReadOnlySpan : Decode_CharSequence_TestCase
            //            {
            //                protected override Int32 GetResult(string s)
            //                {
            //                    return Int32.Decode(s.AsSpan());
            //                }
            //            }
            //#endif

            #endregion Decode_CharSequence

            #region TryDecode_CharSequence

            public abstract class TryDecode_CharSequence_TestCase : DecodeTestCase
            {
                protected abstract bool GetResult(string value, out Int32 result);

                [TestCaseSource(typeof(DecodeTestCase), "TestDecode_CharSequence_Data")]
                public virtual void TestTryDecode_CharSequence(string value, int expected)
                {
                    assertTrue(GetResult(value, out Int32 actual));
                    assertEquals($"Int32.TryDecode(string, out Int32) failed. String: \"{value}\" Result: {actual}", new Int32(expected), actual);
                }

                [TestCaseSource(typeof(DecodeTestCase), "TestDecode_CharSequence_ForException_Data")]
                public virtual void TestTryDecode_CharSequence_ForException(Type expectedExceptionType, string value, string message)
                {
                    assertFalse(GetResult(value, out Int32 actual));
                    assertEquals(null, actual);
                }
            }

            public class TryDecode_String : TryDecode_CharSequence_TestCase
            {
                protected override bool GetResult(string value, out Int32 result)
                {
                    return Int32.TryDecode(value, out result);
                }
            }

            // J2N: ReadOnlySpan<char> not supported at this time on this overload
            //#if FEATURE_SPAN
            //            public class TryDecode_ReadOnlySpan : TryDecode_CharSequence_TestCase
            //            {
            //                protected override bool GetResult(string s, out Int32 result)
            //                {
            //                    return Int32.TryDecode(s.AsSpan(), out result);
            //                }
            //            }
            //#endif

            #endregion TryDecode_CharSequence
        }
    }
}
