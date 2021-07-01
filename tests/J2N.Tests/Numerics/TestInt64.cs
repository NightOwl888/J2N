using J2N.Globalization;
using J2N.Text;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;
using System.Text;

namespace J2N.Numerics
{
    public class TestInt64 : TestCase
    {
        /**
        * @tests java.lang.Long#byteValue()
        */
        [Test]
        public void Test_byteValue()
        {
            // Test for method byte java.lang.Long.byteValue()
            Int64 l = new Int64(127);
            assertEquals("Returned incorrect byte value", 127, l.ToByte());
            assertEquals("Returned incorrect byte value", -1, (sbyte)new Int64(long.MaxValue)
                    .ToByte()); // J2N: cast required to change the result to negative
        }

        /**
         * @tests java.lang.Long#compareTo(java.lang.Long)
         */
        [Test]
        public void Test_compareToLjava_lang_Long()
        {
            // Test for method int java.lang.Long.compareTo(java.lang.Long)
            assertTrue("-2 compared to 1 gave non-negative answer", new Int64(-2L)
                    .CompareTo(new Int64(1L)) < 0);
            assertEquals("-2 compared to -2 gave non-zero answer", 0, new Int64(-2L)
                    .CompareTo(new Int64(-2L)));
            assertTrue("3 compared to 2 gave non-positive answer", new Int64(3L)
                    .CompareTo(new Int64(2L)) > 0);

            //try
            //{
            //    new Int64(0).CompareTo(null);
            //    fail("No NPE");
            //}
            //catch (NullPointerException e)
            //{
            //}

            // J2N: Return 1 when comparing to null to match other .NET classes
            assertEquals(1, new Int64(0).CompareTo(null));
        }

        /**
         * @tests java.lang.Long#compareTo(Object)
         */
        [Test]
        public void Test_compareTo_Object()
        {
            // Test for method int java.lang.Long.compareTo(java.lang.Long)
            assertTrue("-2 compared to 1 gave non-negative answer", new Int64(-2L)
                    .CompareTo((object)new Int64(1L)) < 0);
            assertEquals("-2 compared to -2 gave non-zero answer", 0, new Int64(-2L)
                    .CompareTo((object)new Int64(-2L)));
            assertTrue("3 compared to 2 gave non-positive answer", new Int64(3L)
                    .CompareTo((object)new Int64(2L)) > 0);

            //try
            //{
            //    new Int64(0).CompareTo(null);
            //    fail("No NPE");
            //}
            //catch (NullPointerException e)
            //{
            //}

            // J2N: Return 1 when comparing to null to match other .NET classes
            assertEquals(1, new Int64(0).CompareTo((object)null));

            // J2N: Check to ensure exception is thrown when there is a type mismatch
            Assert.Throws<ArgumentException>(() => new Int64(0).CompareTo((object)4));
        }

        // J2N: Moved to CharSequences
        ///**
        // * @tests java.lang.Long#decode(java.lang.String)
        // */
        //[Test]
        //public void Test_decodeLjava_lang_String2()
        //{
        //    // Test for method java.lang.Long
        //    // java.lang.Long.decode(java.lang.String)
        //    assertEquals("Returned incorrect value for hex string", 255L, Int64.Decode(
        //            "0xFF").GetInt64Value());
        //    assertEquals("Returned incorrect value for dec string", -89000L, Int64.Decode(
        //            "-89000").GetInt64Value());
        //    assertEquals("Returned incorrect value for 0 decimal", 0, Int64.Decode("0")
        //            .GetInt64Value());
        //    assertEquals("Returned incorrect value for 0 hex", 0, Int64.Decode("0x0")
        //            .GetInt64Value());
        //    assertTrue(
        //            "Returned incorrect value for most negative value decimal",
        //            Int64.Decode("-9223372036854775808").GetInt64Value() == unchecked((long)0x8000000000000000L));
        //    assertTrue(
        //            "Returned incorrect value for most negative value hex",
        //            Int64.Decode("-0x8000000000000000").GetInt64Value() == unchecked((long)0x8000000000000000L));
        //    assertTrue(
        //            "Returned incorrect value for most positive value decimal",
        //            Int64.Decode("9223372036854775807").GetInt64Value() == 0x7fffffffffffffffL);
        //    assertTrue(
        //            "Returned incorrect value for most positive value hex",
        //            Int64.Decode("0x7fffffffffffffff").GetInt64Value() == 0x7fffffffffffffffL);
        //    assertTrue("Failed for 07654321765432", Int64.Decode("07654321765432")
        //            .GetInt64Value() == /*07654321765432l*/ 538536569626); // J2N: Octal literals not supported in C#, converted to decimal

        //    bool exception = false;
        //    try
        //    {
        //        Int64
        //                .Decode("999999999999999999999999999999999999999999999999999999");
        //    }
        //    catch (OverflowException e) // J2N: .NET throws OverflowException rather than FormatException in this case
        //    {
        //        // Correct
        //        exception = true;
        //    }
        //    assertTrue("Failed to throw exception for value > ilong", exception);

        //    exception = false;
        //    try
        //    {
        //        Int64.Decode("9223372036854775808");
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
        //        Int64.Decode("-9223372036854775809");
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
        //        Int64.Decode("0x8000000000000000");
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
        //        Int64.Decode("-0x8000000000000001");
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
        //        Int64.Decode("42325917317067571199");
        //    }
        //    catch (OverflowException e) // J2N: .NET throws OverflowException rather than FormatException in this case
        //    {
        //        // Correct
        //        exception = true;
        //    }
        //    assertTrue("Failed to throw exception for 42325917317067571199",
        //            exception);
        //}

        /////**
        ////* @tests java.lang.Long#getLong(java.lang.String)
        ////*/
        ////[Test]
        ////public void Test_getLongLjava_lang_String()
        ////{
        ////    // Test for method java.lang.Long
        ////    // java.lang.Long.getLong(java.lang.String)
        ////    Properties tProps = new Properties();
        ////    tProps.put("testLong", "99");
        ////    System.setProperties(tProps);
        ////    assertTrue("returned incorrect Long", Long.getLong("testLong").equals(
        ////            new Int64(99)));
        ////    assertNull("returned incorrect default Long",
        ////            Long.getLong("ff"));
        ////}

        /////**
        //// * @tests java.lang.Long#getLong(java.lang.String, long)
        //// */
        ////[Test]
        ////public void Test_getLongLjava_lang_StringJ()
        ////{
        ////    // Test for method java.lang.Long
        ////    // java.lang.Long.getLong(java.lang.String, long)
        ////    Properties tProps = new Properties();
        ////    tProps.put("testLong", "99");
        ////    System.setProperties(tProps);
        ////    assertTrue("returned incorrect Long", Long.getLong("testLong", 4L)
        ////            .equals(new Int64(99)));
        ////    assertTrue("returned incorrect default Long", Long.getLong("ff", 4L)
        ////            .equals(new Int64(4)));
        ////}

        /////**
        //// * @tests java.lang.Long#getLong(java.lang.String, java.lang.Long)
        //// */
        ////[Test]
        ////public void Test_getLongLjava_lang_StringLjava_lang_Long()
        ////{
        ////    // Test for method java.lang.Long
        ////    // java.lang.Long.getLong(java.lang.String, java.lang.Long)
        ////    Properties tProps = new Properties();
        ////    tProps.put("testLong", "99");
        ////    System.setProperties(tProps);
        ////    assertTrue("returned incorrect Long", Long.getLong("testLong",
        ////            new Int64(4)).equals(new Int64(99)));
        ////    assertTrue("returned incorrect default Long", Long.getLong("ff",
        ////            new Int64(4)).equals(new Int64(4)));
        ////}

        // J2N: Moved to CharSequences
        ///**
        // * @tests java.lang.Long#parseLong(java.lang.String)
        // */
        //[Test]
        //public void Test_parseLongLjava_lang_String2()
        //{
        //    // Test for method long java.lang.Long.parseLong(java.lang.String)

        //    long l = Int64.Parse("89000000005", J2N.Text.StringFormatter.InvariantCulture);
        //    assertEquals("Parsed to incorrect long value", 89000000005L, l);
        //    assertEquals("Returned incorrect value for 0", 0, Int64.Parse("0", J2N.Text.StringFormatter.InvariantCulture));
        //    assertTrue("Returned incorrect value for most negative value", Int64
        //            .Parse("-9223372036854775808", J2N.Text.StringFormatter.InvariantCulture) == unchecked((long)0x8000000000000000L));
        //    assertTrue("Returned incorrect value for most positive value", Int64
        //            .Parse("9223372036854775807", J2N.Text.StringFormatter.InvariantCulture) == 0x7fffffffffffffffL);

        //    bool exception = false;
        //    try
        //    {
        //        Int64.Parse("9223372036854775808", J2N.Text.StringFormatter.InvariantCulture);
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
        //        Int64.Parse("-9223372036854775809", J2N.Text.StringFormatter.InvariantCulture);
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
        // * @tests java.lang.Long#parseLong(java.lang.String, int)
        // */
        //[Test]
        //public void Test_parseLongLjava_lang_StringI()
        //{
        //    // Test for method long java.lang.Long.parseLong(java.lang.String, int)
        //    assertEquals("Returned incorrect value",
        //            100000000L, Int64.Parse("100000000", 10));
        //    assertEquals("Returned incorrect value from hex string", 68719476735L, Int64.Parse(
        //            "FFFFFFFFF", 16));
        //    assertTrue("Returned incorrect value from octal string: "
        //            + Int64.Parse("77777777777", J2N.Text.StringFormatter.InvariantCulture), Int64.Parse("77777777777",
        //            8) == 8589934591L);
        //    assertEquals("Returned incorrect value for 0 hex", 0, Int64
        //            .Parse("0", 16));
        //    assertTrue("Returned incorrect value for most negative value hex", Int64
        //            .Parse("-8000000000000000", 16) == unchecked((long)0x8000000000000000L));
        //    assertTrue("Returned incorrect value for most positive value hex", Int64
        //            .Parse("7fffffffffffffff", 16) == 0x7fffffffffffffffL);
        //    assertEquals("Returned incorrect value for 0 decimal", 0, Int64.Parse(
        //            "0", 10));
        //    assertTrue(
        //            "Returned incorrect value for most negative value decimal",
        //            Int64.Parse("-9223372036854775808", 10) == unchecked((long)0x8000000000000000L));
        //    assertTrue(
        //            "Returned incorrect value for most positive value decimal",
        //            Int64.Parse("9223372036854775807", 10) == 0x7fffffffffffffffL);

        //    bool exception = false;
        //    try
        //    {
        //        Int64.Parse("999999999999", 8);
        //    }
        //    catch (FormatException e)
        //    {
        //        // correct
        //        exception = true;
        //    }
        //    assertTrue("Failed to throw exception when passed invalid string",
        //            exception);

        //    exception = false;
        //    try
        //    {
        //        Int64.Parse("9223372036854775808", 10);
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
        //        Int64.Parse("-9223372036854775809", 10);
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
        //    //    Int64.Parse("8000000000000000", 16);
        //    //}
        //    //catch (OverflowException e) // J2N: .NET throws OverflowException rather than FormatException in this case
        //    //{
        //    //    // Correct
        //    //    exception = true;
        //    //}
        //    //assertTrue("Failed to throw exception for hex MAX_VALUE + 1", exception);


        //    assertEquals(1L, Int64.Parse("1", 16));
        //    assertEquals(-1L, Int64.Parse("ffffffffffffffff", 16));
        //    assertEquals(9223372036854775807L, Int64.Parse("7fffffffffffffff", 16));
        //    assertEquals(-9223372036854775808L, Int64.Parse("-8000000000000000", 16)); // Special case: In Java, we allow the negative sign for the smallest negative number
        //    assertEquals(-9223372036854775808L, Int64.Parse("8000000000000000", 16));  // In .NET, it should parse without the negative sign to the same value (in .NET the negative sign is not allowed)
        //    assertEquals(-9223372036854775807L, Int64.Parse("8000000000000001", 16));

        //    exception = false;
        //    try
        //    {
        //        Int64.Parse("-8000000000000001", 16);
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
        //        Int64.Parse("42325917317067571199", 10);
        //    }
        //    catch (OverflowException e) // J2N: .NET throws OverflowException rather than FormatException in this case
        //    {
        //        // Correct
        //        exception = true;
        //    }
        //    assertTrue("Failed to throw exception for 42325917317067571199",
        //            exception);
        //}

        /**
         * @tests java.lang.Long#toBinaryString(long)
         */
        [Test]
        public void Test_toBinaryStringJ()
        {
            // Test for method java.lang.String java.lang.Long.toBinaryString(long)
            assertEquals("Incorrect binary string returned", "11011001010010010000", Int64.ValueOf(
                    890000L).ToBinaryString());
            assertEquals("Incorrect binary string returned",

                                    "1000000000000000000000000000000000000000000000000000000000000000", Int64
                            .ValueOf(long.MinValue).ToBinaryString()
                            );
            assertEquals("Incorrect binary string returned",

                                    "111111111111111111111111111111111111111111111111111111111111111", Int64
                            .ValueOf(long.MaxValue).ToBinaryString()
                            );
        }

        /**
         * @tests java.lang.Long#toHexString(long)
         */
        [Test]
        public void Test_toHexStringJ()
        {
            // Test for method java.lang.String java.lang.Long.toHexString(long)
            assertEquals("Incorrect hex string returned", "54e0845", Int64.ValueOf(89000005L).ToHexString()
                    );
            assertEquals("Incorrect hex string returned", "8000000000000000", Int64.ValueOf(
                    long.MinValue).ToHexString());
            assertEquals("Incorrect hex string returned", "7fffffffffffffff", Int64.ValueOf(
                    long.MaxValue).ToHexString());
        }

        /**
         * @tests java.lang.Long#toOctalString(long)
         */
        [Test]
        public void Test_toOctalStringJ()
        {
            // Test for method java.lang.String java.lang.Long.toOctalString(long)
            assertEquals("Returned incorrect oct string", "77777777777", Int64.ValueOf(
                    8589934591L).ToOctalString());
            assertEquals("Returned incorrect oct string", "1000000000000000000000", Int64.ValueOf(
                    long.MinValue).ToOctalString());
            assertEquals("Returned incorrect oct string", "777777777777777777777", Int64.ValueOf(
                    long.MaxValue).ToOctalString());
        }

        /**
         * @tests java.lang.Long#toString()
         */
        [Test]
        public void Test_toString2()
        {
            // Test for method java.lang.String java.lang.Long.toString()
            Int64 l = new Int64(89000000005L);
            assertEquals("Returned incorrect String",
                    "89000000005", l.ToString());
            assertEquals("Returned incorrect String", "-9223372036854775808", new Int64(long.MinValue)
                    .ToString());
            assertEquals("Returned incorrect String", "9223372036854775807", new Int64(long.MaxValue)
                    .ToString());
        }

        /**
         * @tests java.lang.Long#toString(long)
         */
        [Test]
        public void Test_toStringJ2()
        {
            // Test for method java.lang.String java.lang.Long.toString(long)

            assertEquals("Returned incorrect String", "89000000005", Int64.ToString(89000000005L)
                    );
            assertEquals("Returned incorrect String", "-9223372036854775808", Int64.ToString(long.MinValue)
                    );
            assertEquals("Returned incorrect String", "9223372036854775807", Int64.ToString(long.MaxValue)
                    );
        }

        /**
         * @tests java.lang.Long#toString(long, int)
         */
        [Test]
        public void Test_toStringJI()
        {
            // Test for method java.lang.String java.lang.Long.toString(long, int)
            assertEquals("Returned incorrect dec string", "100000000", Int64.ToString(100000000L,
                    10));
            assertEquals("Returned incorrect hex string", "fffffffff", Int64.ToString(68719476735L,
                    16));
            assertEquals("Returned incorrect oct string", "77777777777", Int64.ToString(8589934591L,
                    8));
            assertEquals("Returned incorrect bin string",
                    "1111111111111111111111111111111111111111111", Int64.ToString(
                    8796093022207L, 2));
            assertEquals("Returned incorrect min string", "-9223372036854775808", Int64.ToString(
                    unchecked((long)0x8000000000000000L), 10));
            assertEquals("Returned incorrect max string", "9223372036854775807", Int64.ToString(
                    0x7fffffffffffffffL, 10));
            assertEquals("Returned incorrect min string", "-8000000000000000", Int64.ToString(
                    unchecked((long)0x8000000000000000L), 16));
            assertEquals("Returned incorrect max string", "7fffffffffffffff", Int64.ToString(
                    0x7fffffffffffffffL, 16));
        }

        /**
         * @tests java.lang.Long#valueOf(java.lang.String)
         */
        [Test]
        public void Test_valueOfLjava_lang_String2()
        {
            // Test for method java.lang.Long
            // java.lang.Long.valueOf(java.lang.String)
            assertEquals("Returned incorrect value", 100000000L, Int64.ValueOf("100000000", J2N.Text.StringFormatter.InvariantCulture)
                    .ToInt64());
            assertTrue("Returned incorrect value", Int64.ValueOf(
                    "9223372036854775807", J2N.Text.StringFormatter.InvariantCulture).ToInt64() == long.MaxValue);
            assertTrue("Returned incorrect value", Int64.ValueOf(
                    "-9223372036854775808", J2N.Text.StringFormatter.InvariantCulture).ToInt64() == long.MinValue);

            bool exception = false;
            try
            {
                Int64
                        .ValueOf("999999999999999999999999999999999999999999999999999999999999", J2N.Text.StringFormatter.InvariantCulture);
            }
            catch (OverflowException e) // J2N: .NET throws OverflowException rather than FormatException in this case
            {
                // correct
                exception = true;
            }
            assertTrue("Failed to throw exception when passed invalid string",
                    exception);

            exception = false;
            try
            {
                Int64.ValueOf("9223372036854775808", J2N.Text.StringFormatter.InvariantCulture);
            }
            catch (OverflowException e) // J2N: .NET throws OverflowException rather than FormatException in this case
            {
                // correct
                exception = true;
            }
            assertTrue("Failed to throw exception when passed invalid string",
                    exception);

            exception = false;
            try
            {
                Int64.ValueOf("-9223372036854775809", J2N.Text.StringFormatter.InvariantCulture);
            }
            catch (OverflowException e) // J2N: .NET throws OverflowException rather than FormatException in this case
            {
                // correct
                exception = true;
            }
            assertTrue("Failed to throw exception when passed invalid string",
                    exception);
        }

        /**
         * @tests java.lang.Long#valueOf(java.lang.String, int)
         */
        [Test]
        public void Test_valueOfLjava_lang_StringI()
        {
            // Test for method java.lang.Long
            // java.lang.Long.valueOf(java.lang.String, int)
            assertEquals("Returned incorrect value", 100000000L, Int64.ValueOf("100000000", 10)
                    .ToInt64());
            assertEquals("Returned incorrect value from hex string", 68719476735L, Int64.ValueOf(
                    "FFFFFFFFF", 16).ToInt64());
            assertTrue("Returned incorrect value from octal string: "
                    + Int64.ValueOf("77777777777", 8).ToString(), Int64.ValueOf(
                    "77777777777", 8).ToInt64() == 8589934591L);
            assertTrue("Returned incorrect value", Int64.ValueOf(
                    "9223372036854775807", 10).ToInt64() == long.MaxValue);
            assertTrue("Returned incorrect value", Int64.ValueOf(
                    "-9223372036854775808", 10).ToInt64() == long.MinValue);
            assertTrue("Returned incorrect value", Int64.ValueOf("7fffffffffffffff",
                    16).ToInt64() == long.MaxValue);
            assertTrue("Returned incorrect value", Int64.ValueOf(
                    "-8000000000000000", 16).ToInt64() == long.MinValue);

            bool exception = false;
            try
            {
                Int64.ValueOf("999999999999", 8);
            }
            catch (FormatException e)
            {
                // correct
                exception = true;
            }
            assertTrue("Failed to throw exception when passed invalid string",
                    exception);

            exception = false;
            try
            {
                Int64.ValueOf("9223372036854775808", 10);
            }
            catch (OverflowException e) // J2N: .NET throws OverflowException rather than FormatException in this case
            {
                // correct
                exception = true;
            }
            assertTrue("Failed to throw exception when passed invalid string",
                    exception);

            exception = false;
            try
            {
                Int64.ValueOf("-9223372036854775809", 10);
            }
            catch (OverflowException e) // J2N: .NET throws OverflowException rather than FormatException in this case
            {
                // correct
                exception = true;
            }
            assertTrue("Failed to throw exception when passed invalid string",
                    exception);
        }
        /**
         * @tests java.lang.Long#valueOf(long)
         */
        [Test]
        public void Test_valueOfJ()
        {
            assertEquals(new Int64(long.MinValue), Int64.ValueOf(long.MinValue));
            assertEquals(new Int64(long.MaxValue), Int64.ValueOf(long.MaxValue));
            assertEquals(new Int64(0), Int64.ValueOf(0));

            long lng = -128;
            while (lng < 128)
            {
                assertEquals(new Int64(lng), Int64.ValueOf(lng));
                assertSame(Int64.ValueOf(lng), Int64.ValueOf(lng));
                lng++;
            }
        }

        [Test]
        public void GetTypeCode_Invoke_ReturnsInt64()
        {
            assertEquals(TypeCode.Int64, Int64.ValueOf((long)1).GetTypeCode());
        }

        /**
         * @tests java.lang.Long#hashCode()
         */
        [Test]
        public void Test_hashCode()
        {
            assertEquals((int)(1L ^ (1L.TripleShift(32))), new Int64(1).GetHashCode());
            assertEquals((int)(2L ^ (2L.TripleShift(32))), new Int64(2).GetHashCode());
            assertEquals((int)(0L ^ (0L.TripleShift(32))), new Int64(0).GetHashCode());
            assertEquals((int)(-1L ^ ((-1L).TripleShift(32))), new Int64(-1).GetHashCode());
        }

        // J2N: Removed this overload because all of the constructors are deprecated in JDK 16
        ///**
        // * @tests java.lang.Long#Int64(String)
        // */
        //[Test]
        //public void Test_ConstructorLjava_lang_String()
        //{
        //    assertEquals(new Int64(0), new Int64("0", J2N.Text.StringFormatter.InvariantCulture));
        //    assertEquals(new Int64(1), new Int64("1", J2N.Text.StringFormatter.InvariantCulture));
        //    assertEquals(new Int64(-1), new Int64("-1", J2N.Text.StringFormatter.InvariantCulture));

        //    try
        //    {
        //        new Int64("0x1", J2N.Text.StringFormatter.InvariantCulture);
        //        fail("Expected NumberFormatException with hex string.");
        //    }
        //    catch (FormatException e) { }

        //    try
        //    {
        //        new Int64("9.2", J2N.Text.StringFormatter.InvariantCulture);
        //        fail("Expected NumberFormatException with floating point string.");
        //    }
        //    catch (FormatException e) { }

        //    try
        //    {
        //        new Int64("", J2N.Text.StringFormatter.InvariantCulture);
        //        fail("Expected NumberFormatException with empty string.");
        //    }
        //    catch (FormatException e) { }

        //    try
        //    {
        //        new Int64(null, J2N.Text.StringFormatter.InvariantCulture);
        //        fail("Expected NumberFormatException with null string.");
        //    }
        //    catch (ArgumentNullException e) { } // J2N: .NET throws ArgumentNullException rather than FormatException in this case
        //}

        /**
         * @tests java.lang.Long#Long
         */
        [Test]
        public void Test_ConstructorJ()
        {
            assertEquals(1, new Int64(1).ToInt32());
            assertEquals(2, new Int64(2).ToInt32());
            assertEquals(0, new Int64(0).ToInt32());
            assertEquals(-1, new Int64(-1).ToInt32());
        }

        /**
         * @tests java.lang.Long#byteValue()
         */
        [Test]
        public void Test_booleanValue()
        {
            assertEquals(1, new Int64(1).ToByte());
            assertEquals(2, new Int64(2).ToByte());
            assertEquals(0, new Int64(0).ToByte());
            assertEquals(-1, (sbyte)new Int64(-1).ToByte()); // J2N: cast required to change the result to negative
        }

        /**
         * @tests java.lang.Long#equals(Object)
         */
        [Test]
        public void Test_equalsLjava_lang_Object()
        {
            assertEquals((object)new Int64(0), (object)Int64.ValueOf(0));
            assertEquals((object)new Int64(1), (object)Int64.ValueOf(1));
            assertEquals((object)new Int64(-1), (object)Int64.ValueOf(-1));

            Int64 fixture = new Int64(25);
            assertEquals((object)fixture, (object)fixture);
            assertFalse(fixture.Equals((object)null));
            assertFalse(fixture.Equals((object)"Not a Long"));
        }

        /**
         * @tests java.lang.Long#equals(Object)
         */
        [Test]
        public void Test_equals_Int64()
        {
            // Implicit conversion
            assertEquals(new Int64(0), Int64.ValueOf(0));
            assertEquals(new Int64(1), Int64.ValueOf(1));
            assertEquals(new Int64(-1), Int64.ValueOf(-1));

            // Explicit
            assertTrue(new Int64(0).Equals(Int64.ValueOf(0)));
            assertTrue(new Int64(1).Equals(Int64.ValueOf(1)));
            assertTrue(new Int64(-1).Equals(Int64.ValueOf(-1)));

            Int64 fixture = new Int64(25);
            assertEquals(fixture, fixture);
            assertFalse(fixture.Equals((Int64)null));
            //assertFalse(fixture.Equals("Not a Long"));
        }

        /**
         * @tests java.lang.Long#toString()
         */
        [Test]
        public void Test_toString()
        {
            assertEquals("-1", new Int64(-1).ToString());
            assertEquals("0", new Int64(0).ToString());
            assertEquals("1", new Int64(1).ToString());
            assertEquals("-1", new Int64(unchecked((int)0xFFFFFFFF)).ToString());
        }

        /**
         * @tests java.lang.Long#toString
         */
        [Test]
        public void Test_toStringJ()
        {
            assertEquals("-1", Int64.ToString(-1));
            assertEquals("0", Int64.ToString(0));
            assertEquals("1", Int64.ToString(1));
            assertEquals("-1", Int64.ToString(unchecked((int)0xFFFFFFFF)));
        }

        /**
         * @tests java.lang.Long#valueOf(String)
         */
        [Test]
        public void Test_valueOfLjava_lang_String()
        {
            assertEquals(new Int64(0), Int64.ValueOf("0", J2N.Text.StringFormatter.InvariantCulture));
            assertEquals(new Int64(1), Int64.ValueOf("1", J2N.Text.StringFormatter.InvariantCulture));
            assertEquals(new Int64(-1), Int64.ValueOf("-1", J2N.Text.StringFormatter.InvariantCulture));

            try
            {
                Int64.ValueOf("0x1", J2N.Text.StringFormatter.InvariantCulture);
                fail("Expected NumberFormatException with hex string.");
            }
            catch (FormatException e) { }

            try
            {
                Int64.ValueOf("9.2", J2N.Text.StringFormatter.InvariantCulture);
                fail("Expected NumberFormatException with floating point string.");
            }
            catch (FormatException e) { }

            try
            {
                Int64.ValueOf("", J2N.Text.StringFormatter.InvariantCulture);
                fail("Expected NumberFormatException with empty string.");
            }
            catch (FormatException e) { }

            try
            {
                Int64.ValueOf(null, J2N.Text.StringFormatter.InvariantCulture);
                fail("Expected NumberFormatException with null string.");
            }
            catch (ArgumentNullException e) { } // J2N: .NET throws ArgumentNullException rather than FormatException in this case
        }

        /**
         * @tests java.lang.Long#valueOf(String,long)
         */
        [Test]
        public void Test_valueOfLjava_lang_StringJ()
        {
            assertEquals(new Int64(0), Int64.ValueOf("0", 10));
            assertEquals(new Int64(1), Int64.ValueOf("1", 10));
            assertEquals(new Int64(-1), Int64.ValueOf("-1", 10));

            //must be consistent with Character.digit()
            assertEquals(Character.Digit('1', 2), Int64.ValueOf("1", 2).ToByte());
            assertEquals(Character.Digit('F', 16), Int64.ValueOf("F", 16).ToByte());

            try
            {
                Int64.ValueOf("0x1", 10);
                fail("Expected NumberFormatException with hex string.");
            }
            catch (FormatException e) { }

            try
            {
                Int64.ValueOf("9.2", 10);
                fail("Expected NumberFormatException with floating point string.");
            }
            catch (FormatException e) { }

            try
            {
                Int64.ValueOf("", 10);
                fail("Expected NumberFormatException with empty string.");
            }
            catch (FormatException e) { } // J2N: .NET throws ArgumentOutOfRangeException rather than FormatException in this case, but since it is inconsistent with long.Parse() we are going with FormatException.

            //try
            //{
            //    Int64.ValueOf(null, 10);
            //    fail("Expected NumberFormatException with null string.");
            //}
            //catch (FormatException e) { }

            // J2N: Match .NET behavior and return 0 for a null string
            assertEquals(0, Int64.ValueOf(null, 10));
        }

        // J2N: Moved to CharSequences
        ///**
        // * @tests java.lang.Long#parseLong(String)
        // */
        //[Test]
        //public void Test_parseLongLjava_lang_String()
        //{
        //    assertEquals(0, Int64.Parse("0", J2N.Text.StringFormatter.InvariantCulture));
        //    assertEquals(1, Int64.Parse("1", J2N.Text.StringFormatter.InvariantCulture));
        //    assertEquals(-1, Int64.Parse("-1", J2N.Text.StringFormatter.InvariantCulture));

        //    try
        //    {
        //        Int64.Parse("0x1", J2N.Text.StringFormatter.InvariantCulture);
        //        fail("Expected NumberFormatException with hex string.");
        //    }
        //    catch (FormatException e) { }

        //    try
        //    {
        //        Int64.Parse("9.2", J2N.Text.StringFormatter.InvariantCulture);
        //        fail("Expected NumberFormatException with floating point string.");
        //    }
        //    catch (FormatException e) { }

        //    try
        //    {
        //        Int64.Parse("", J2N.Text.StringFormatter.InvariantCulture);
        //        fail("Expected NumberFormatException with empty string.");
        //    }
        //    catch (FormatException e) { }

        //    try
        //    {
        //        Int64.Parse(null, J2N.Text.StringFormatter.InvariantCulture);
        //        fail("Expected NumberFormatException with null string.");
        //    }
        //    catch (ArgumentNullException e) { } // J2N: .NET throws ArgumentNullException rather than FormatException in this case
        //}

        // J2N: Moved to CharSequences
        ///**
        // * @tests java.lang.Long#parseLong(String,long)
        // */
        //[Test]
        //public void Test_parseLongLjava_lang_StringJ()
        //{
        //    assertEquals(0, Int64.Parse("0", 10));
        //    assertEquals(1, Int64.Parse("1", 10));
        //    assertEquals(-1, Int64.Parse("-1", 10));

        //    //must be consistent with Character.digit()
        //    assertEquals(Character.Digit('1', 2), Int64.Parse("1", 2));
        //    assertEquals(Character.Digit('F', 16), Int64.Parse("F", 16));

        //    try
        //    {
        //        Int64.Parse("0x1", 10);
        //        fail("Expected NumberFormatException with hex string.");
        //    }
        //    catch (FormatException e) { }

        //    try
        //    {
        //        Int64.Parse("9.2", 10);
        //        fail("Expected NumberFormatException with floating point string.");
        //    }
        //    catch (FormatException e) { }

        //    try
        //    {
        //        Int64.Parse("", 10);
        //        fail("Expected NumberFormatException with empty string.");
        //    }
        //    catch (FormatException e) { } // J2N: .NET throws ArgumentOutOfRangeException rather than FormatException in this case, but since it is inconsistent with long.Parse() we are going with FormatException.

        //    //try
        //    //{
        //    //    Int64.Parse(null, 10);
        //    //    fail("Expected NumberFormatException with null string.");
        //    //}
        //    //catch (FormatException e) { }

        //    // J2N: Match .NET behavior where null will result in 0
        //    assertEquals(0, Int64.Parse(null, 10));
        //}

        // J2N: Moved to CharSequences
        ///**
        // * @tests java.lang.Long#decode(String)
        // */
        //[Test]
        //public void Test_decodeLjava_lang_String()
        //{
        //    assertEquals(new Int64(0), Int64.Decode("0"));
        //    assertEquals(new Int64(1), Int64.Decode("1"));
        //    assertEquals(new Int64(-1), Int64.Decode("-1"));
        //    assertEquals(new Int64(0xF), Int64.Decode("0xF"));
        //    assertEquals(new Int64(0xF), Int64.Decode("#F"));
        //    assertEquals(new Int64(0xF), Int64.Decode("0XF"));
        //    assertEquals(new Int64(07), Int64.Decode("07"));

        //    try
        //    {
        //        Int64.Decode("9.2");
        //        fail("Expected NumberFormatException with floating point string.");
        //    }
        //    catch (FormatException e) { }

        //    try
        //    {
        //        Int64.Decode("");
        //        fail("Expected NumberFormatException with empty string.");
        //    }
        //    catch (FormatException e) { }

        //    try
        //    {
        //        Int64.Decode(null);
        //        //undocumented NPE, but seems consistent across JREs
        //        fail("Expected NullPointerException with null string.");
        //    }
        //    catch (ArgumentNullException e) { }
        //}

        /**
         * @tests java.lang.Long#doubleValue()
         */
        [Test]
        public void Test_doubleValue()
        {
            assertEquals(-1D, new Int64(-1).ToDouble(), 0D);
            assertEquals(0D, new Int64(0).ToDouble(), 0D);
            assertEquals(1D, new Int64(1).ToDouble(), 0D);
        }

        /**
         * @tests java.lang.Long#floatValue()
         */
        [Test]
        public void Test_floatValue()
        {
            assertEquals(-1F, new Int64(-1).ToSingle(), 0F);
            assertEquals(0F, new Int64(0).ToSingle(), 0F);
            assertEquals(1F, new Int64(1).ToSingle(), 0F);
        }

        /**
         * @tests java.lang.Long#intValue()
         */
        [Test]
        public void Test_intValue()
        {
            assertEquals(-1, new Int64(-1).ToInt32());
            assertEquals(0, new Int64(0).ToInt32());
            assertEquals(1, new Int64(1).ToInt32());
        }

        /**
         * @tests java.lang.Long#longValue()
         */
        [Test]
        public void Test_longValue()
        {
            assertEquals(-1L, new Int64(-1).ToInt64());
            assertEquals(0L, new Int64(0).ToInt64());
            assertEquals(1L, new Int64(1).ToInt64());
        }

        /**
         * @tests java.lang.Long#shortValue()
         */
        [Test]
        public void Test_shortValue()
        {
            assertEquals(-1, new Int64(-1).ToInt16());
            assertEquals(0, new Int64(0).ToInt16());
            assertEquals(1, new Int64(1).ToInt16());
        }
        /**
         * @tests java.lang.Long#highestOneBit(long)
         */
        [Test]
        public void Test_HighestOneBitJ()
        {
            assertEquals(0x08, Int64.HighestOneBit(0x0A));
            assertEquals(0x08, Int64.HighestOneBit(0x0B));
            assertEquals(0x08, Int64.HighestOneBit(0x0C));
            assertEquals(0x08, Int64.HighestOneBit(0x0F));
            assertEquals(0x80, Int64.HighestOneBit(0xFF));

            assertEquals(0x080000, Int64.HighestOneBit(0x0F1234));
            assertEquals(0x800000, Int64.HighestOneBit(0xFF9977));

            assertEquals(unchecked((long)0x8000000000000000L), Int64.HighestOneBit(unchecked((long)0xFFFFFFFFFFFFFFFFL)));

            assertEquals(0, Int64.HighestOneBit(0));
            assertEquals(1, Int64.HighestOneBit(1));
            assertEquals(unchecked((long)0x8000000000000000L), Int64.HighestOneBit(-1));
        }

        /**
         * @tests java.lang.Long#lowestOneBit(long)
         */
        [Test]
        public void Test_lowestOneBitJ()
        {
            assertEquals(0x10, Int64.LowestOneBit(0xF0));

            assertEquals(0x10, Int64.LowestOneBit(0x90));
            assertEquals(0x10, Int64.LowestOneBit(0xD0));

            assertEquals(0x10, Int64.LowestOneBit(0x123490));
            assertEquals(0x10, Int64.LowestOneBit(0x1234D0));

            assertEquals(0x100000, Int64.LowestOneBit(0x900000));
            assertEquals(0x100000, Int64.LowestOneBit(0xD00000));

            assertEquals(0x40, Int64.LowestOneBit(0x40));
            assertEquals(0x40, Int64.LowestOneBit(0xC0));

            assertEquals(0x4000, Int64.LowestOneBit(0x4000));
            assertEquals(0x4000, Int64.LowestOneBit(0xC000));

            assertEquals(0x4000, Int64.LowestOneBit(0x99994000));
            assertEquals(0x4000, Int64.LowestOneBit(0x9999C000));

            assertEquals(0, Int64.LowestOneBit(0));
            assertEquals(1, Int64.LowestOneBit(1));
            assertEquals(1, Int64.LowestOneBit(-1));
        }
        /**
         * @tests java.lang.Long#numberOfLeadingZeros(long)
         */
        [Test]
        public void Test_numberOfLeadingZerosJ()
        {
            assertEquals(64, Int64.LeadingZeroCount(0x0L));
            assertEquals(63, Int64.LeadingZeroCount(0x1));
            assertEquals(62, Int64.LeadingZeroCount(0x2));
            assertEquals(62, Int64.LeadingZeroCount(0x3));
            assertEquals(61, Int64.LeadingZeroCount(0x4));
            assertEquals(61, Int64.LeadingZeroCount(0x5));
            assertEquals(61, Int64.LeadingZeroCount(0x6));
            assertEquals(61, Int64.LeadingZeroCount(0x7));
            assertEquals(60, Int64.LeadingZeroCount(0x8));
            assertEquals(60, Int64.LeadingZeroCount(0x9));
            assertEquals(60, Int64.LeadingZeroCount(0xA));
            assertEquals(60, Int64.LeadingZeroCount(0xB));
            assertEquals(60, Int64.LeadingZeroCount(0xC));
            assertEquals(60, Int64.LeadingZeroCount(0xD));
            assertEquals(60, Int64.LeadingZeroCount(0xE));
            assertEquals(60, Int64.LeadingZeroCount(0xF));
            assertEquals(59, Int64.LeadingZeroCount(0x10));
            assertEquals(56, Int64.LeadingZeroCount(0x80));
            assertEquals(56, Int64.LeadingZeroCount(0xF0));
            assertEquals(55, Int64.LeadingZeroCount(0x100));
            assertEquals(52, Int64.LeadingZeroCount(0x800));
            assertEquals(52, Int64.LeadingZeroCount(0xF00));
            assertEquals(51, Int64.LeadingZeroCount(0x1000));
            assertEquals(48, Int64.LeadingZeroCount(0x8000));
            assertEquals(48, Int64.LeadingZeroCount(0xF000));
            assertEquals(47, Int64.LeadingZeroCount(0x10000));
            assertEquals(44, Int64.LeadingZeroCount(0x80000));
            assertEquals(44, Int64.LeadingZeroCount(0xF0000));
            assertEquals(43, Int64.LeadingZeroCount(0x100000));
            assertEquals(40, Int64.LeadingZeroCount(0x800000));
            assertEquals(40, Int64.LeadingZeroCount(0xF00000));
            assertEquals(39, Int64.LeadingZeroCount(0x1000000));
            assertEquals(36, Int64.LeadingZeroCount(0x8000000));
            assertEquals(36, Int64.LeadingZeroCount(0xF000000));
            assertEquals(35, Int64.LeadingZeroCount(0x10000000));
            assertEquals(32, Int64.LeadingZeroCount(0x80000000)); // J2N: Changed test to match observed behavior in JDK (32 rather than 0)
            assertEquals(32, Int64.LeadingZeroCount(0xF0000000)); // J2N: Changed test to match observed behavior in JDK (32 rather than 0)

            assertEquals(1, Int64.LeadingZeroCount(long.MaxValue));
            assertEquals(0, Int64.LeadingZeroCount(long.MinValue));
        }

        /**
         * @tests java.lang.Long#numberOfTrailingZeros(long)
         */
        [Test]
        public void Test_numberOfTrailingZerosJ()
        {
            assertEquals(64, Int64.TrailingZeroCount(0x0));
            assertEquals(63, Int64.TrailingZeroCount(long.MinValue));
            assertEquals(0, Int64.TrailingZeroCount(long.MaxValue));

            assertEquals(0, Int64.TrailingZeroCount(0x1));
            assertEquals(3, Int64.TrailingZeroCount(0x8));
            assertEquals(0, Int64.TrailingZeroCount(0xF));

            assertEquals(4, Int64.TrailingZeroCount(0x10));
            assertEquals(7, Int64.TrailingZeroCount(0x80));
            assertEquals(4, Int64.TrailingZeroCount(0xF0));

            assertEquals(8, Int64.TrailingZeroCount(0x100));
            assertEquals(11, Int64.TrailingZeroCount(0x800));
            assertEquals(8, Int64.TrailingZeroCount(0xF00));

            assertEquals(12, Int64.TrailingZeroCount(0x1000));
            assertEquals(15, Int64.TrailingZeroCount(0x8000));
            assertEquals(12, Int64.TrailingZeroCount(0xF000));

            assertEquals(16, Int64.TrailingZeroCount(0x10000));
            assertEquals(19, Int64.TrailingZeroCount(0x80000));
            assertEquals(16, Int64.TrailingZeroCount(0xF0000));

            assertEquals(20, Int64.TrailingZeroCount(0x100000));
            assertEquals(23, Int64.TrailingZeroCount(0x800000));
            assertEquals(20, Int64.TrailingZeroCount(0xF00000));

            assertEquals(24, Int64.TrailingZeroCount(0x1000000));
            assertEquals(27, Int64.TrailingZeroCount(0x8000000));
            assertEquals(24, Int64.TrailingZeroCount(0xF000000));

            assertEquals(28, Int64.TrailingZeroCount(0x10000000));
            assertEquals(31, Int64.TrailingZeroCount(0x80000000));
            assertEquals(28, Int64.TrailingZeroCount(0xF0000000));
        }

        /**
         * @tests java.lang.Long#bitCount(long)
         */
        [Test]
        public void Test_bitCountJ()
        {
            assertEquals(0, Int64.PopCount(0x0));
            assertEquals(1, Int64.PopCount(0x1));
            assertEquals(1, Int64.PopCount(0x2));
            assertEquals(2, Int64.PopCount(0x3));
            assertEquals(1, Int64.PopCount(0x4));
            assertEquals(2, Int64.PopCount(0x5));
            assertEquals(2, Int64.PopCount(0x6));
            assertEquals(3, Int64.PopCount(0x7));
            assertEquals(1, Int64.PopCount(0x8));
            assertEquals(2, Int64.PopCount(0x9));
            assertEquals(2, Int64.PopCount(0xA));
            assertEquals(3, Int64.PopCount(0xB));
            assertEquals(2, Int64.PopCount(0xC));
            assertEquals(3, Int64.PopCount(0xD));
            assertEquals(3, Int64.PopCount(0xE));
            assertEquals(4, Int64.PopCount(0xF));

            assertEquals(8, Int64.PopCount(0xFF));
            assertEquals(12, Int64.PopCount(0xFFF));
            assertEquals(16, Int64.PopCount(0xFFFF));
            assertEquals(20, Int64.PopCount(0xFFFFF));
            assertEquals(24, Int64.PopCount(0xFFFFFF));
            assertEquals(28, Int64.PopCount(0xFFFFFFF));
            assertEquals(64, Int64.PopCount(unchecked((long)0xFFFFFFFFFFFFFFFFL)));
        }

        /**
         * @tests java.lang.Long#rotateLeft(long,long)
         */
        [Test]
        public void Test_rotateLeftJI()
        {
            assertEquals(0xF, Int64.RotateLeft(0xF, 0));
            assertEquals(0xF0, Int64.RotateLeft(0xF, 4));
            assertEquals(0xF00, Int64.RotateLeft(0xF, 8));
            assertEquals(0xF000, Int64.RotateLeft(0xF, 12));
            assertEquals(0xF0000, Int64.RotateLeft(0xF, 16));
            assertEquals(0xF00000, Int64.RotateLeft(0xF, 20));
            assertEquals(0xF000000, Int64.RotateLeft(0xF, 24));
            assertEquals(0xF0000000L, Int64.RotateLeft(0xF, 28));
            assertEquals(unchecked((long)0xF000000000000000L), Int64.RotateLeft(unchecked((long)0xF000000000000000L), 64));
        }

        /**
         * @tests java.lang.Long#rotateRight(long,long)
         */
        [Test]
        public void Test_rotateRightJI()
        {
            assertEquals(0xF, Int64.RotateRight(0xF0, 4));
            assertEquals(0xF, Int64.RotateRight(0xF00, 8));
            assertEquals(0xF, Int64.RotateRight(0xF000, 12));
            assertEquals(0xF, Int64.RotateRight(0xF0000, 16));
            assertEquals(0xF, Int64.RotateRight(0xF00000, 20));
            assertEquals(0xF, Int64.RotateRight(0xF000000, 24));
            assertEquals(0xF, Int64.RotateRight(0xF0000000L, 28));
            assertEquals(unchecked((long)0xF000000000000000L), Int64.RotateRight(unchecked((long)0xF000000000000000L), 64));
            assertEquals(unchecked((long)0xF000000000000000L), Int64.RotateRight(unchecked((long)0xF000000000000000L), 0));

        }

        /**
         * @tests java.lang.Long#reverseBytes(long)
         */
        [Test]
        public void Test_reverseBytesJ()
        {
            assertEquals(unchecked((long)0xAABBCCDD00112233L), Int64.ReverseBytes(0x33221100DDCCBBAAL));
            assertEquals(0x1122334455667788L, Int64.ReverseBytes(unchecked((long)0x8877665544332211L)));
            assertEquals(0x0011223344556677L, Int64.ReverseBytes(0x7766554433221100L));
            assertEquals(0x2000000000000002L, Int64.ReverseBytes(0x0200000000000020L));
        }

        /**
         * @tests java.lang.Long#reverse(long)
         */
        [Test]
        public void Test_reverseJ()
        {
            assertEquals(0, Int64.Reverse(0));
            assertEquals(-1, Int64.Reverse(-1));
            assertEquals(unchecked((long)0x8000000000000000L), Int64.Reverse(1));
        }

        /**
         * @tests java.lang.Long#signum(long)
         */
        [Test]
        public void Test_signumJ()
        {
            for (int i = -128; i < 0; i++)
            {
                assertEquals(-1, Int64.Signum(i));
            }
            assertEquals(0, Int64.Signum(0));
            for (int i = 1; i <= 127; i++)
            {
                assertEquals(1, Int64.Signum(i));
            }
        }

        public class CharSequences : TestCase
        {
            #region ParseTestCase

            public abstract class ParseTestCase
            {
                // Radix-based parsing

                #region TestParse_CharSequence_Int32_Data

                public static IEnumerable<TestCaseData> TestParse_CharSequence_Int32_Data
                {
                    get
                    {
                        // JDK 8

                        yield return new TestCaseData(+100L, "+100", 10);
                        yield return new TestCaseData(-100L, "-100", 10);

                        yield return new TestCaseData(0L, "+0", 10);
                        yield return new TestCaseData(0L, "-0", 10);
                        yield return new TestCaseData(0L, "+00000", 10);
                        yield return new TestCaseData(0L, "-00000", 10);

                        yield return new TestCaseData(0L, "0", 10);
                        yield return new TestCaseData(1L, "1", 10);
                        yield return new TestCaseData(9L, "9", 10);

                        // Harmony (Test_parseLongLjava_lang_String())

                        yield return new TestCaseData(0L, "0", 10);
                        yield return new TestCaseData(1L, "1", 10);
                        yield return new TestCaseData(-1L, "-1", 10);

                        yield return new TestCaseData(0L, null, 10); // J2N: Match .NET behavior and return 0 in this case

                        // Harmony (Test_parseLongLjava_lang_String2())

                        yield return new TestCaseData(89000000005L, "89000000005", 10);
                        yield return new TestCaseData(0L, "0", 10);
                        yield return new TestCaseData(unchecked((long)0x8000000000000000L), "-9223372036854775808", 10);
                        yield return new TestCaseData(0x7fffffffffffffffL, "9223372036854775807", 10);

                        // Harmony (Test_parseLongLjava_lang_StringI())

                        yield return new TestCaseData(100000000L, "100000000", 10);
                        yield return new TestCaseData(68719476735L, "FFFFFFFFF", 16);
                        yield return new TestCaseData(8589934591L, "77777777777", 8);
                        yield return new TestCaseData(0L, "0", 16);
                        yield return new TestCaseData(unchecked((long)0x8000000000000000L), "-8000000000000000", 16);
                        yield return new TestCaseData(0x7fffffffffffffffL, "7fffffffffffffff", 16);
                        yield return new TestCaseData(0L, "0", 10);
                        yield return new TestCaseData(unchecked((long)0x8000000000000000L), "-9223372036854775808", 10);
                        yield return new TestCaseData(0x7fffffffffffffffL, "9223372036854775807", 10);

                        // Harmony (Test_parseLongLjava_lang_StringJ())

                        yield return new TestCaseData(0L, "0", 10);
                        yield return new TestCaseData(1L, "1", 10);
                        yield return new TestCaseData(-1L, "-1", 10);

                        //must be consistent with Character.digit()
                        yield return new TestCaseData(Character.Digit('1', 2), "1", 2);
                        yield return new TestCaseData(Character.Digit('F', 16), "F", 16);

                        yield return new TestCaseData(0L, null, 10); // J2N: Match .NET behavior where null will result in 0

                        // .NET 5

                        string[] testValues = { null, null, null, null, "7FFFFFFFFFFFFFFF", "9223372036854775807", "777777777777777777777", "111111111111111111111111111111111111111111111111111111111111111", "8000000000000000", "-9223372036854775808", "1000000000000000000000", "1000000000000000000000000000000000000000000000000000000000000000" };
                        int[] testBases = { 10, 2, 8, 16, 16, 10, 8, 2, 16, 10, 8, 2 };
                        long[] expectedValues = { 0, 0, 0, 0, long.MaxValue, long.MaxValue, long.MaxValue, long.MaxValue, long.MinValue, long.MinValue, long.MinValue, long.MinValue };

                        for (int i = 0; i < testValues.Length; i++)
                        {
                            yield return new TestCaseData(expectedValues[i], testValues[i], testBases[i]);
                        }

                        // Custom

                        yield return new TestCaseData(1L, "1", 16);
                        yield return new TestCaseData(-1L, "ffffffffffffffff", 16);
                        yield return new TestCaseData(9223372036854775807L, "7fffffffffffffff", 16);
                        yield return new TestCaseData(-9223372036854775808L, "-8000000000000000", 16); // Special case: In Java, we allow the negative sign for the smallest negative number
                        yield return new TestCaseData(-9223372036854775808L, "8000000000000000", 16);  // In .NET, it should parse without the negative sign to the same value (in .NET the negative sign is not allowed)
                        yield return new TestCaseData(-9223372036854775807L, "8000000000000001", 16);

                        // Surrogate Pairs (.NET only supports ASCII, but Java supports these)

                        yield return new TestCaseData(999L, "𝟗𑃹𝟫", 10);
                        yield return new TestCaseData(5783L, "𝟓𝟕𝟖𝟑", 10);
                        yield return new TestCaseData(479L, "𑁪𑁭𑁯", 10);

                        // Non-decimal needs to be tested separately because they go through
                        // a separate execution path
                        yield return new TestCaseData(2457L, "𝟗𑃹𝟫", 16);
                        yield return new TestCaseData(22403L, "𝟓𝟕𝟖𝟑", 16);
                        yield return new TestCaseData(1145L, "𑁪𑁭𑁯", 16);
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

                        // Harmony (Test_parseLongLjava_lang_String())

                        yield return new TestCaseData(typeof(FormatException), "0x1", 10);
                        yield return new TestCaseData(typeof(FormatException), "9.2", 10);
                        yield return new TestCaseData(typeof(FormatException), "", 10);
                        //yield return new TestCaseData(typeof(FormatException), null, 10); // J2N: Match .NET behavior and return 0 in this case

                        // Harmony (Test_parseLongLjava_lang_String2())

                        yield return new TestCaseData(typeof(OverflowException), "9223372036854775808", 10);
                        yield return new TestCaseData(typeof(OverflowException), "-9223372036854775809", 10);

                        // Harmony (Test_parseLongLjava_lang_StringI())

                        yield return new TestCaseData(typeof(FormatException), "999999999999", 8);
                        yield return new TestCaseData(typeof(OverflowException), "9223372036854775808", 10);
                        yield return new TestCaseData(typeof(OverflowException), "-9223372036854775809", 10);
                        yield return new TestCaseData(typeof(OverflowException), "-8000000000000001", 16);
                        yield return new TestCaseData(typeof(OverflowException), "42325917317067571199", 10);

                        // Harmony (Test_parseLongLjava_lang_StringJ())

                        yield return new TestCaseData(typeof(FormatException), "0x1", 10);
                        yield return new TestCaseData(typeof(FormatException), "9.2", 10);
                        yield return new TestCaseData(typeof(FormatException), "", 10); // J2N: .NET throws ArgumentOutOfRangeException rather than FormatException in this case, but since it is inconsistent with long.Parse() we are going with FormatException.
                        //yield return new TestCaseData(typeof(ArgumentNullException), null, 10); // J2N: Match .NET behavior where null will result in 0

                        // .NET 5

                        string[] overflowValues = { "9223372036854775808", "-9223372036854775809", "11111111111111111111111111111111111111111111111111111111111111111", "1FFFFffffFFFFffff", "7777777777777777777777777" };
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

                        yield return new TestCaseData(0L, "test-00000", 4, 10 - 4, 10);
                        yield return new TestCaseData(-12345L, "test-12345", 4, 10 - 4, 10);
                        yield return new TestCaseData(12345L, "xx12345yy", 2, 7 - 2, 10);
                        yield return new TestCaseData(123456789012345L, "xx123456789012345yy", 2, 17 - 2, 10);
                        yield return new TestCaseData(15L, "xxFyy", 2, 3 - 2, 16);

                        yield return new TestCaseData(12345L, "xx1234567yy", 2, 5, 10);

                        // Harmony (Test_parseLongLjava_lang_String())

                        yield return new TestCaseData(0L, "0", 0, 1, 10);
                        yield return new TestCaseData(1L, "1", 0, 1, 10);
                        yield return new TestCaseData(-1L, "-1", 0, 2, 10);

                        //yield return new TestCaseData(0L, null, 0, 1, 10); // Unlike non-indexed overload, throw instead of returning zero

                        // Harmony (Test_parseLongLjava_lang_String2())

                        yield return new TestCaseData(89000000005L, "89000000005", 0, 11, 10);
                        yield return new TestCaseData(0L, "0", 0, 1, 10);
                        yield return new TestCaseData(unchecked((long)0x8000000000000000L), "-9223372036854775808", 0, 20, 10);
                        yield return new TestCaseData(0x7fffffffffffffffL, "9223372036854775807", 0, 19, 10);

                        // Harmony (Test_parseLongLjava_lang_StringI())

                        yield return new TestCaseData(100000000L, "100000000", 0, 9, 10);
                        yield return new TestCaseData(68719476735L, "FFFFFFFFF", 0, 9, 16);
                        yield return new TestCaseData(8589934591L, "77777777777", 0, 11, 8);
                        yield return new TestCaseData(0L, "0", 0, 1, 16);
                        yield return new TestCaseData(unchecked((long)0x8000000000000000L), "-8000000000000000", 0, 17, 16);
                        yield return new TestCaseData(0x7fffffffffffffffL, "7fffffffffffffff", 0, 16, 16);
                        yield return new TestCaseData(0L, "0", 0, 1, 10);
                        yield return new TestCaseData(unchecked((long)0x8000000000000000L), "-9223372036854775808", 0, 20, 10);
                        yield return new TestCaseData(0x7fffffffffffffffL, "9223372036854775807", 0, 19, 10);

                        // Harmony (Test_parseLongLjava_lang_StringJ())

                        yield return new TestCaseData(0L, "0", 0, 1, 10);
                        yield return new TestCaseData(1L, "1", 0, 1, 10);
                        yield return new TestCaseData(-1L, "-1", 0, 2, 10);

                        //must be consistent with Character.digit()
                        yield return new TestCaseData(Character.Digit('1', 2), "1", 0, 1, 2);
                        yield return new TestCaseData(Character.Digit('F', 16), "F", 0, 1, 16);

                        //yield return new TestCaseData(0L, null, 0, 1, 10); // Unlike non-indexed overload, throw instead of returning zero

                        // .NET 5

                        string[] testValues = { /*null, null, null, null,*/ "7FFFFFFFFFFFFFFF", "9223372036854775807", "777777777777777777777", "111111111111111111111111111111111111111111111111111111111111111", "8000000000000000", "-9223372036854775808", "1000000000000000000000", "1000000000000000000000000000000000000000000000000000000000000000" };
                        int[] testBases = { /*10, 2, 8, 16,*/ 16, 10, 8, 2, 16, 10, 8, 2 };
                        long[] expectedValues = { /*0, 0, 0, 0,*/ long.MaxValue, long.MaxValue, long.MaxValue, long.MaxValue, long.MinValue, long.MinValue, long.MinValue, long.MinValue };

                        for (int i = 0; i < testValues.Length; i++)
                        {
                            yield return new TestCaseData(expectedValues[i], testValues[i], 0, testValues[i].Length, testBases[i]);
                        }

                        // Custom

                        yield return new TestCaseData(1L, "1", 0, 1, 16);
                        yield return new TestCaseData(-1L, "ffffffffffffffff", 0, 16, 16);
                        yield return new TestCaseData(9223372036854775807L, "7fffffffffffffff", 0, 16, 16);
                        yield return new TestCaseData(-9223372036854775808L, "-8000000000000000", 0, 17, 16); // Special case: In Java, we allow the negative sign for the smallest negative number
                        yield return new TestCaseData(-9223372036854775808L, "8000000000000000", 0, 16, 16);  // In .NET, it should parse without the negative sign to the same value (in .NET the negative sign is not allowed)
                        yield return new TestCaseData(-9223372036854775807L, "8000000000000001", 0, 16, 16);

                        // Surrogate Pairs (.NET only supports ASCII, but Java supports these)

                        yield return new TestCaseData(999L, "𝟗𑃹𝟫", 0, 6, 10);
                        yield return new TestCaseData(5783L, "𝟓𝟕𝟖𝟑", 0, 8, 10);
                        yield return new TestCaseData(479L, "𑁪𑁭𑁯", 0, 6, 10);

                        // Non-decimal needs to be tested separately because they go through
                        // a separate execution path
                        yield return new TestCaseData(2457L, "𝟗𑃹𝟫", 0, 6, 16);
                        yield return new TestCaseData(22403L, "𝟓𝟕𝟖𝟑", 0, 8, 16);
                        yield return new TestCaseData(1145L, "𑁪𑁭𑁯", 0, 6, 16);
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

                        // Harmony (Test_parseLongLjava_lang_String())

                        yield return new TestCaseData(typeof(FormatException), "0x1", 0, 3, 10);
                        yield return new TestCaseData(typeof(FormatException), "9.2", 0, 3, 10);
                        yield return new TestCaseData(typeof(FormatException), "", 0, 0, 10);
                        yield return new TestCaseData(typeof(ArgumentNullException), null, 0, 1, 10); // Unlike non-indexed overload, throw instead of returning zero

                        // Harmony (Test_parseLongLjava_lang_String2())

                        yield return new TestCaseData(typeof(OverflowException), "9223372036854775808", 0, 19, 10);
                        yield return new TestCaseData(typeof(OverflowException), "-9223372036854775809", 0, 20, 10);

                        // Harmony (Test_parseLongLjava_lang_StringI())

                        yield return new TestCaseData(typeof(FormatException), "999999999999", 0, 12, 8);
                        yield return new TestCaseData(typeof(OverflowException), "9223372036854775808", 0, 19, 10);
                        yield return new TestCaseData(typeof(OverflowException), "-9223372036854775809", 0, 20, 10);
                        yield return new TestCaseData(typeof(OverflowException), "-8000000000000001", 0, 17, 16);
                        yield return new TestCaseData(typeof(OverflowException), "42325917317067571199", 0, 20, 10);

                        // Harmony (Test_parseLongLjava_lang_StringJ())

                        yield return new TestCaseData(typeof(FormatException), "0x1", 0, 3, 10);
                        yield return new TestCaseData(typeof(FormatException), "9.2", 0, 3, 10);
                        yield return new TestCaseData(typeof(FormatException), "", 0, 0, 10); // J2N: .NET throws ArgumentOutOfRangeException rather than FormatException in this case, but since it is inconsistent with long.Parse() we are going with FormatException.
                        yield return new TestCaseData(typeof(ArgumentNullException), null, 0, 1, 10); // Unlike non-indexed overload, throw instead of returning zero

                        // .NET 5

                        string[] overflowValues = { "9223372036854775808", "-9223372036854775809", "11111111111111111111111111111111111111111111111111111111111111111", "1FFFFffffFFFFffff", "7777777777777777777777777" };
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

                // Culture-sensitive parsing

                #region TestParse_CharSequence_NumberStyle_IFormatProvider_Data

                public static IEnumerable<TestCaseData> TestParse_CharSequence_NumberStyle_IFormatProvider_Data
                {
                    get
                    {
                        // JDK 8

                        yield return new TestCaseData(+100L, "+100", NumberStyle.Integer, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(-100L, "-100", NumberStyle.Integer, NumberFormatInfo.InvariantInfo);

                        yield return new TestCaseData(0L, "+0", NumberStyle.Integer, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(0L, "-0", NumberStyle.Integer, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(0L, "+00000", NumberStyle.Integer, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(0L, "-00000", NumberStyle.Integer, NumberFormatInfo.InvariantInfo);

                        yield return new TestCaseData(0L, "0", NumberStyle.Integer, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(1L, "1", NumberStyle.Integer, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(9L, "9", NumberStyle.Integer, NumberFormatInfo.InvariantInfo);

                        // Harmony (Test_parseLongLjava_lang_String())

                        yield return new TestCaseData(0L, "0", NumberStyle.Integer, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(1L, "1", NumberStyle.Integer, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(-1L, "-1", NumberStyle.Integer, NumberFormatInfo.InvariantInfo);

                        //yield return new TestCaseData(0L, null, NumberStyle.Integer, NumberFormatInfo.InvariantInfo); // J2N: Match .NET behavior and return 0 in this case

                        // Harmony (Test_parseLongLjava_lang_String2())

                        yield return new TestCaseData(89000000005L, "89000000005", NumberStyle.Integer, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(0L, "0", NumberStyle.Integer, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(unchecked((long)0x8000000000000000L), "-9223372036854775808", NumberStyle.Integer, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(0x7fffffffffffffffL, "9223372036854775807", NumberStyle.Integer, NumberFormatInfo.InvariantInfo);

                        // Harmony (Test_parseLongLjava_lang_StringI())

                        yield return new TestCaseData(100000000L, "100000000", NumberStyle.Integer, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(68719476735L, "FFFFFFFFF", NumberStyle.HexNumber, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(0L, "0", NumberStyle.HexNumber, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(unchecked((long)0x8000000000000000L), /*"-8000000000000000"*/ "8000000000000000", NumberStyle.HexNumber, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(0x7fffffffffffffffL, "7fffffffffffffff", NumberStyle.HexNumber, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(0L, "0", NumberStyle.Integer, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(unchecked((long)0x8000000000000000L), "-9223372036854775808", NumberStyle.Integer, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(0x7fffffffffffffffL, "9223372036854775807", NumberStyle.Integer, NumberFormatInfo.InvariantInfo);

                        // Harmony (Test_parseLongLjava_lang_StringJ())

                        yield return new TestCaseData(0L, "0", NumberStyle.Integer, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(1L, "1", NumberStyle.Integer, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(-1L, "-1", NumberStyle.Integer, NumberFormatInfo.InvariantInfo);

                        //yield return new TestCaseData(0L, null, NumberStyle.Integer, NumberFormatInfo.InvariantInfo); // J2N: Match .NET behavior where null will result in 0
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

                        // Harmony (Test_parseLongLjava_lang_String())

                        yield return new TestCaseData(typeof(FormatException), "0x1", NumberStyle.Integer, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(typeof(FormatException), "9.2", NumberStyle.Integer, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(typeof(FormatException), "", NumberStyle.Integer, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(typeof(ArgumentNullException), null, NumberStyle.Integer, NumberFormatInfo.InvariantInfo);

                        // Harmony (Test_parseLongLjava_lang_String2())

                        yield return new TestCaseData(typeof(OverflowException), "9223372036854775808", NumberStyle.Integer, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(typeof(OverflowException), "-9223372036854775809", NumberStyle.Integer, NumberFormatInfo.InvariantInfo);

                        // Harmony (Test_parseLongLjava_lang_StringI())

                        yield return new TestCaseData(typeof(OverflowException), "9223372036854775808", NumberStyle.Integer, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(typeof(OverflowException), "-9223372036854775809", NumberStyle.Integer, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(typeof(OverflowException), /*"-8000000000000001"*/ "10000000000000000", NumberStyle.HexNumber, NumberFormatInfo.InvariantInfo); // J2N: 2's complement required
                        yield return new TestCaseData(typeof(OverflowException), "42325917317067571199", NumberStyle.Integer, NumberFormatInfo.InvariantInfo);

                        // Harmony (Test_parseLongLjava_lang_StringJ())

                        yield return new TestCaseData(typeof(FormatException), "0x1", NumberStyle.Integer, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(typeof(FormatException), "9.2", NumberStyle.Integer, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(typeof(FormatException), "", NumberStyle.Integer, NumberFormatInfo.InvariantInfo); // J2N: .NET throws ArgumentOutOfRangeException rather than FormatException in this case, but since it is inconsistent with long.Parse() we are going with FormatException.
                        yield return new TestCaseData(typeof(ArgumentNullException), null, NumberStyle.Integer, NumberFormatInfo.InvariantInfo); // J2N: Match .NET behavior where null will result in 0
                    }
                }

                #endregion TestParse_CharSequence_NumberStyle_IFormatProvider_ForException_Data
            }

            #endregion ParseTestCase

            // Radix-based parsing

            #region Parse_CharSequence_Int32

            public abstract class Parse_CharSequence_Int32_TestCase : ParseTestCase
            {
                protected abstract long GetResult(string value, int radix);


                [TestCaseSource(typeof(ParseTestCase), "TestParse_CharSequence_Int32_Data")]
                public void TestParse_String_Int32(long expected, string value, int radix)
                {
                    var actual = GetResult(value, radix);
                    assertEquals($"Int64.Parse(string, IFormatProvider) failed. String: \"{value}\" Result: {actual}", expected, actual);
                }

                [TestCaseSource(typeof(ParseTestCase), "TestParse_CharSequence_Int32_ForException_Data")]
                public virtual void TestParse_CharSequence_Int32_ForException(Type expectedExceptionType, string value, int radix)
                {
                    Assert.Throws(expectedExceptionType, () => GetResult(value, radix));
                }
            }

            public class Parse_String_Int32 : Parse_CharSequence_Int32_TestCase
            {
                protected override long GetResult(string value, int radix)
                {
                    return Int64.Parse(value, radix);
                }
            }


            // J2N: ReadOnlySpan<char> not supported at this time on this overload (not supported in .NET anyway)
            //#if FEATURE_READONLYSPAN
            //            public class Parse_ReadOnlySpan_Int32 : Parse_CharSequence_Int32_TestCase
            //            {
            //                protected override long GetResult(string value, int radix)
            //                {
            //                    return Int64.Parse(value.AsSpan(), radix);
            //                }
            //            }
            //#endif

            #endregion Parse_CharSequence_Int32

            #region Parse_CharSequence_Int32_Int32_Int32

            public abstract class Parse_CharSequence_Int32_Int32_Int32_TestCase : ParseTestCase
            {
                protected virtual bool IsNullableType => true;

                protected abstract long GetResult(string s, int startIndex, int length, int radix);


                [TestCaseSource(typeof(ParseTestCase), "TestParse_CharSequence_Int32_Int32_Int32_Data")]
                public void TestParse_String_Int32_Int32_Int32(long expected, string value, int startIndex, int length, int radix)
                {
                    Assume.That(IsNullableType || (!IsNullableType && value != null), "null is not supported by this character sequence type.");

                    var actual = GetResult(value, startIndex, length, radix);
                    assertEquals($"Int64.Parse(string, int, int, int) failed. Expected: {expected} String: \"{value}\", startIndex: {startIndex}, length: {length} radix: {radix} Result: {actual}", expected, actual);
                }

                [TestCaseSource(typeof(ParseTestCase), "TestParse_CharSequence_Int32_Int32_Int32_ForException_Data")]
                public void TestParse_String_Int32_Int32_Int32_ForException(Type expectedExceptionType, string value, int startIndex, int length, int radix)
                {
                    Assume.That(IsNullableType || (!IsNullableType && value != null), "null is not supported by this character sequence type.");

                    Assert.Throws(expectedExceptionType, () => GetResult(value, startIndex, length, radix));
                }
            }

            public class Parse_String_Int32_Int32_Int32_Int32 : Parse_CharSequence_Int32_Int32_Int32_TestCase
            {
                protected override long GetResult(string value, int startIndex, int length, int radix)
                {
                    return Int64.Parse(value, startIndex, length, radix);
                }
            }

            public class Parse_CharArray_Int32_Int32_Int32_Int32 : Parse_CharSequence_Int32_Int32_Int32_TestCase
            {
                protected override long GetResult(string value, int startIndex, int length, int radix)
                {
                    return Int64.Parse(value is null ? null : value.ToCharArray(), startIndex, length, radix);
                }
            }

            public class Parse_StringBuilder_Int32_Int32_Int32_Int32 : Parse_CharSequence_Int32_Int32_Int32_TestCase
            {
                protected override long GetResult(string value, int startIndex, int length, int radix)
                {
                    // NOTE: Although technically the .NET StringBuilder will accept null and convert it to an empty string, we
                    // want to test the method for null here.
                    return Int64.Parse(value is null ? null : new StringBuilder(value), startIndex, length, radix);
                }
            }

            public class Parse_ICharSequence_Int32_Int32_Int32_Int32 : Parse_CharSequence_Int32_Int32_Int32_TestCase
            {
                protected override long GetResult(string value, int startIndex, int length, int radix)
                {
                    return Int64.Parse(value.AsCharSequence(), startIndex, length, radix);
                }
            }

#if FEATURE_READONLYSPAN
            public class Parse_ReadOnlySpan_Int32_Int32_Int32_Int32 : Parse_CharSequence_Int32_Int32_Int32_TestCase
            {
                protected override bool IsNullableType => false;

                protected override long GetResult(string value, int startIndex, int length, int radix)
                {
                    return Int64.Parse(value.AsSpan(), startIndex, length, radix);
                }
            }
#endif

            #endregion Parse_CharSequence_Int32_Int32_Int32

            #region TryParse_CharSequence_Int32

            public abstract class TryParse_CharSequence_Int32_TestCase : ParseTestCase
            {
                protected abstract bool GetResult(string value, int radix, out long result);


                [TestCaseSource(typeof(ParseTestCase), "TestParse_CharSequence_Int32_Data")]
                public void TestTryParse_String_Int32(long expected, string value, int radix)
                {
                    assertTrue(GetResult(value, radix, out long actual));
                    assertEquals($"Int64.Parse(string, IFormatProvider) failed. String: \"{value}\" Result: {actual}", expected, actual);
                }

                [TestCaseSource(typeof(ParseTestCase), "TestParse_CharSequence_Int32_ForException_Data")]
                public virtual void TestTryParse_CharSequence_Int32_ForException(Type expectedExceptionType, string value, int radix)
                {
                    assertFalse(GetResult(value, radix, out long actual));
                    assertEquals(0, actual);
                }
            }

            public class TryParse_String_Int32 : TryParse_CharSequence_Int32_TestCase
            {
                protected override bool GetResult(string value, int radix, out long result)
                {
                    return Int64.TryParse(value, radix, out result);
                }
            }


            // J2N: ReadOnlySpan<char> not supported at this time on this overload (not supported in .NET anyway)
            //#if FEATURE_READONLYSPAN
            //            public class TryParse_ReadOnlySpan_Int32 : TryParse_CharSequence_Int32_TestCase
            //            {
            //                protected override bool GetResult(string s, int radix, out long result)
            //                {
            //                    return Int64.TryParse(s.AsSpan(), radix, out result);
            //                }
            //            }
            //#endif

            #endregion TryParse_CharSequence_Int32

            #region TryParse_CharSequence_Int32_Int32_Int32

            public abstract class TryParse_CharSequence_Int32_Int32_Int32_TestCase : ParseTestCase
            {
                protected virtual bool IsNullableType => true;

                protected abstract bool GetResult(string value, int startIndex, int length, int radix, out long result);


                [TestCaseSource(typeof(ParseTestCase), "TestParse_CharSequence_Int32_Int32_Int32_Data")]
                public void TestTryParse_String_Int32_Int32_Int32(long expected, string value, int startIndex, int length, int radix)
                {
                    Assume.That(IsNullableType || (!IsNullableType && value != null), "null is not supported by this character sequence type.");

                    assertTrue(GetResult(value, startIndex, length, radix, out long actual));
                    assertEquals($"Int64.Parse(string, int, int, int) failed. Expected: {expected} String: \"{value}\", startIndex: {startIndex}, length: {length} radix: {radix} Result: {actual}", expected, actual);
                }

                [TestCaseSource(typeof(ParseTestCase), "TestParse_CharSequence_Int32_Int32_Int32_ForException_Data")]
                public void TestTryParse_String_Int32_Int32_Int32_ForException(Type expectedExceptionType, string value, int startIndex, int length, int radix)
                {
                    Assume.That(IsNullableType || (!IsNullableType && value != null), "null is not supported by this character sequence type.");

                    assertFalse(GetResult(value, startIndex, length, radix, out long actual));
                    assertEquals(0, actual);
                }
            }

            public class TryParse_String_Int32_Int32_Int32_Int32 : TryParse_CharSequence_Int32_Int32_Int32_TestCase
            {
                protected override bool GetResult(string value, int startIndex, int length, int radix, out long result)
                {
                    return Int64.TryParse(value, startIndex, length, radix, out result);
                }
            }

            public class TryParse_CharArray_Int32_Int32_Int32_Int32 : TryParse_CharSequence_Int32_Int32_Int32_TestCase
            {
                protected override bool GetResult(string value, int startIndex, int length, int radix, out long result)
                {
                    return Int64.TryParse(value is null ? null : value.ToCharArray(), startIndex, length, radix, out result);
                }
            }

            public class TryParse_StringBuilder_Int32_Int32_Int32_Int32 : TryParse_CharSequence_Int32_Int32_Int32_TestCase
            {
                protected override bool GetResult(string value, int startIndex, int length, int radix, out long result)
                {
                    return Int64.TryParse(value is null ? null : new StringBuilder(value), startIndex, length, radix, out result);
                }
            }

            public class TryParse_ICharSequence_Int32_Int32_Int32_Int32 : TryParse_CharSequence_Int32_Int32_Int32_TestCase
            {
                protected override bool GetResult(string value, int startIndex, int length, int radix, out long result)
                {
                    return Int64.TryParse(value.AsCharSequence(), startIndex, length, radix, out result);
                }
            }

#if FEATURE_READONLYSPAN
            public class TryParse_ReadOnlySpan_Int32_Int32_Int32_Int32 : TryParse_CharSequence_Int32_Int32_Int32_TestCase
            {
                protected override bool IsNullableType => false;

                protected override bool GetResult(string value, int startIndex, int length, int radix, out long result)
                {
                    return Int64.TryParse(value.AsSpan(), startIndex, length, radix, out result);
                }
            }
#endif

            #endregion TryParse_CharSequence_Int32_Int32_Int32

            // Culture-aware parsing

            #region Parse_CharSequence_IFormatProvider

            public abstract class Parse_CharSequence_IFormatProvider_TestCase : ParseTestCase
            {
                protected virtual bool IsNullableType => true;

                protected abstract long GetResult(string s, IFormatProvider provider);


                [TestCaseSource(typeof(ParseTestCase), "TestParse_CharSequence_NumberStyle_IFormatProvider_Data")]
                public void TestParse_CharSequence_IFormatProvider(long expected, string value, NumberStyle style, IFormatProvider provider)
                {
                    Assume.That(IsNullableType || (!IsNullableType && value != null), "null is not supported by this character sequence type.");
                    Assume.That((style & ~NumberStyle.Integer) == 0, "Custom NumberStyles are not supported on this overload.");

                    var actual = GetResult(value, provider);
                    assertEquals($"Int64.Parse(string, NumberStyle, IFormatProvider) failed. Expected: {expected} String: \"{value}\", NumberStyle: {style}, provider: {provider} Result: {actual}", expected, actual);
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
                protected override long GetResult(string s, IFormatProvider provider)
                {
                    return Int64.Parse(s, provider);
                }
            }

            #endregion Parse_CharSequence_IFormatProvider

            #region Parse_CharSequence_NumberStyle_IFormatProvider

            public abstract class Parse_CharSequence_NumberStyle_IFormatProvider_TestCase : ParseTestCase
            {
                protected virtual bool IsNullableType => true;

                protected abstract long GetResult(string s, NumberStyle style, IFormatProvider provider);


                [TestCaseSource(typeof(ParseTestCase), "TestParse_CharSequence_NumberStyle_IFormatProvider_Data")]
                public void TestParse_CharSequence_NumberStyle_IFormatProvider(long expected, string value, NumberStyle style, IFormatProvider provider)
                {
                    Assume.That(IsNullableType || (!IsNullableType && value != null), "null is not supported by this character sequence type.");

                    var actual = GetResult(value, style, provider);
                    assertEquals($"Int64.Parse(string, NumberStyle, IFormatProvider) failed. Expected: {expected} String: \"{value}\", NumberStyle: {style}, provider: {provider} Result: {actual}", expected, actual);
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
                protected override long GetResult(string s, NumberStyle style, IFormatProvider provider)
                {
                    return Int64.Parse(s, style, provider);
                }
            }

#if FEATURE_READONLYSPAN
            public class Parse_ReadOnlySpan_NumberStyle_IFormatProvider_TestCase : Parse_CharSequence_NumberStyle_IFormatProvider_TestCase
            {
                protected override bool IsNullableType => false;

                protected override long GetResult(string s, NumberStyle style, IFormatProvider provider)
                {
                    return Int64.Parse(s.AsSpan(), style, provider);
                }
            }
#endif

            #endregion Parse_CharSequence_NumberStyle_IFormatProvider

            #region TryParse_CharSequence_NumberStyle_IFormatProvider

            public abstract class TryParse_CharSequence_NumberStyle_IFormatProvider_TestCase : ParseTestCase
            {
                protected virtual bool IsNullableType => true;

                protected abstract bool GetResult(string s, NumberStyle style, IFormatProvider provider, out long result);


                [TestCaseSource(typeof(ParseTestCase), "TestParse_CharSequence_NumberStyle_IFormatProvider_Data")]
                public void TestParse_CharSequence_NumberStyle_IFormatProvider(long expected, string value, NumberStyle style, IFormatProvider provider)
                {
                    Assume.That(IsNullableType || (!IsNullableType && value != null), "null is not supported by this character sequence type.");

                    assertTrue(GetResult(value, style, provider, out long actual));
                    assertEquals($"Int64.Parse(string, NumberStyle, IFormatProvider) failed. Expected: {expected} String: \"{value}\", NumberStyle: {style}, provider: {provider} Result: {actual}", expected, actual);
                }

                [TestCaseSource(typeof(ParseTestCase), "TestParse_CharSequence_NumberStyle_IFormatProvider_ForException_Data")]
                public void TestParse_CharSequence_NumberStyle_IFormatProvider_ForException(Type expectedExceptionType, string value, NumberStyle style, IFormatProvider provider)
                {
                    Assume.That(IsNullableType || (!IsNullableType && value != null), "null is not supported by this character sequence type.");

                    long actual = 0;
                    if (expectedExceptionType != typeof(ArgumentException))
                    {
                        assertFalse(GetResult(value, style, provider, out actual));
                    }
                    else // Actual exception should be thrown
                    {
                        Assert.Throws(expectedExceptionType, () => GetResult(value, style, provider, out actual));
                    }
                    assertEquals(0L, actual);
                }
            }

            public class TryParse_String_NumberStyle_IFormatProvider_TestCase : TryParse_CharSequence_NumberStyle_IFormatProvider_TestCase
            {
                protected override bool GetResult(string s, NumberStyle style, IFormatProvider provider, out long result)
                {
                    return Int64.TryParse(s, style, provider, out result);
                }
            }

#if FEATURE_READONLYSPAN
            public class TryParse_ReadOnlySpan_NumberStyle_IFormatProvider_TestCase : TryParse_CharSequence_NumberStyle_IFormatProvider_TestCase
            {
                protected override bool IsNullableType => false;

                protected override bool GetResult(string s, NumberStyle style, IFormatProvider provider, out long result)
                {
                    return Int64.TryParse(s.AsSpan(), style, provider, out result);
                }
            }
#endif

            #endregion Parse_CharSequence_NumberStyle_IFormatProvider

            #region TryParse_CharSequence

            public abstract class TryParse_CharSequence_TestCase : ParseTestCase
            {
                protected virtual bool IsNullableType => true;

                protected abstract bool GetResult(string s, out long result);


                [TestCaseSource(typeof(ParseTestCase), "TestParse_CharSequence_NumberStyle_IFormatProvider_Data")]
                public void TestParse_CharSequence(long expected, string value, NumberStyle style, IFormatProvider provider)
                {
                    Assume.That(IsNullableType || (!IsNullableType && value != null), "null is not supported by this character sequence type.");
                    Assume.That((style & ~NumberStyle.Integer) == 0, "Custom NumberStyles are not supported on this overload.");

                    assertTrue(GetResult(value, out long actual));
                    assertEquals($"Int64.Parse(string, NumberStyle, IFormatProvider) failed. Expected: {expected} String: \"{value}\", NumberStyle: {style}, provider: {provider} Result: {actual}", expected, actual);
                }

                [TestCaseSource(typeof(ParseTestCase), "TestParse_CharSequence_NumberStyle_IFormatProvider_ForException_Data")]
                public void TestParse_CharSequence_ForException(Type expectedExceptionType, string value, NumberStyle style, IFormatProvider provider)
                {
                    Assume.That(IsNullableType || (!IsNullableType && value != null), "null is not supported by this character sequence type.");
                    Assume.That((style & ~NumberStyle.Integer) == 0, "Custom NumberStyles are not supported on this overload.");

                    long actual = 0;
                    if (expectedExceptionType != typeof(ArgumentException))
                    {
                        assertFalse(GetResult(value, out actual));
                    }
                    else // Actual exception should be thrown
                    {
                        Assert.Throws(expectedExceptionType, () => GetResult(value, out actual));
                    }
                    assertEquals(0L, actual);
                }
            }

            public class TryParse_String_TestCase : TryParse_CharSequence_TestCase
            {
                protected override bool GetResult(string s, out long result)
                {
                    return Int64.TryParse(s, out result);
                }
            }

#if FEATURE_READONLYSPAN
            public class TryParse_ReadOnlySpan_TestCase : TryParse_CharSequence_TestCase
            {
                protected override bool IsNullableType => false;

                protected override bool GetResult(string s, out long result)
                {
                    return Int64.TryParse(s.AsSpan(), out result);
                }
            }
#endif

            #endregion TryParse_CharSequence

            // testParseUnsignedLong()

            #region ParseUnsignedTestCase

            public class ParseUnsignedTestCase : TestCase
            {

                public static IEnumerable<TestCaseData> UnsignedLongTestCases
                {
                    get
                    {
                        // JDK 8
                        yield return new TestCaseData(0UL, 2, "0", 0L);
                        yield return new TestCaseData(0UL, 3, "0", 0L);
                        yield return new TestCaseData(0UL, 4, "0", 0L);
                        yield return new TestCaseData(0UL, 5, "0", 0L);
                        yield return new TestCaseData(0UL, 6, "0", 0L);
                        yield return new TestCaseData(0UL, 7, "0", 0L);
                        yield return new TestCaseData(0UL, 8, "0", 0L);
                        yield return new TestCaseData(0UL, 9, "0", 0L);
                        yield return new TestCaseData(0UL, 10, "0", 0L);
                        yield return new TestCaseData(0UL, 11, "0", 0L);
                        yield return new TestCaseData(0UL, 12, "0", 0L);
                        yield return new TestCaseData(0UL, 13, "0", 0L);
                        yield return new TestCaseData(0UL, 14, "0", 0L);
                        yield return new TestCaseData(0UL, 15, "0", 0L);
                        yield return new TestCaseData(0UL, 16, "0", 0L);
                        yield return new TestCaseData(0UL, 17, "0", 0L);
                        yield return new TestCaseData(0UL, 18, "0", 0L);
                        yield return new TestCaseData(0UL, 19, "0", 0L);
                        yield return new TestCaseData(0UL, 20, "0", 0L);
                        yield return new TestCaseData(0UL, 21, "0", 0L);
                        yield return new TestCaseData(0UL, 22, "0", 0L);
                        yield return new TestCaseData(0UL, 23, "0", 0L);
                        yield return new TestCaseData(0UL, 24, "0", 0L);
                        yield return new TestCaseData(0UL, 25, "0", 0L);
                        yield return new TestCaseData(0UL, 26, "0", 0L);
                        yield return new TestCaseData(0UL, 27, "0", 0L);
                        yield return new TestCaseData(0UL, 28, "0", 0L);
                        yield return new TestCaseData(0UL, 29, "0", 0L);
                        yield return new TestCaseData(0UL, 30, "0", 0L);
                        yield return new TestCaseData(0UL, 31, "0", 0L);
                        yield return new TestCaseData(0UL, 32, "0", 0L);
                        yield return new TestCaseData(0UL, 33, "0", 0L);
                        yield return new TestCaseData(0UL, 34, "0", 0L);
                        yield return new TestCaseData(0UL, 35, "0", 0L);
                        yield return new TestCaseData(0UL, 36, "0", 0L);

                        yield return new TestCaseData(1UL, 2, "1", 1L);
                        yield return new TestCaseData(1UL, 3, "1", 1L);
                        yield return new TestCaseData(1UL, 4, "1", 1L);
                        yield return new TestCaseData(1UL, 5, "1", 1L);
                        yield return new TestCaseData(1UL, 6, "1", 1L);
                        yield return new TestCaseData(1UL, 7, "1", 1L);
                        yield return new TestCaseData(1UL, 8, "1", 1L);
                        yield return new TestCaseData(1UL, 9, "1", 1L);
                        yield return new TestCaseData(1UL, 10, "1", 1L);
                        yield return new TestCaseData(1UL, 11, "1", 1L);
                        yield return new TestCaseData(1UL, 12, "1", 1L);
                        yield return new TestCaseData(1UL, 13, "1", 1L);
                        yield return new TestCaseData(1UL, 14, "1", 1L);
                        yield return new TestCaseData(1UL, 15, "1", 1L);
                        yield return new TestCaseData(1UL, 16, "1", 1L);
                        yield return new TestCaseData(1UL, 17, "1", 1L);
                        yield return new TestCaseData(1UL, 18, "1", 1L);
                        yield return new TestCaseData(1UL, 19, "1", 1L);
                        yield return new TestCaseData(1UL, 20, "1", 1L);
                        yield return new TestCaseData(1UL, 21, "1", 1L);
                        yield return new TestCaseData(1UL, 22, "1", 1L);
                        yield return new TestCaseData(1UL, 23, "1", 1L);
                        yield return new TestCaseData(1UL, 24, "1", 1L);
                        yield return new TestCaseData(1UL, 25, "1", 1L);
                        yield return new TestCaseData(1UL, 26, "1", 1L);
                        yield return new TestCaseData(1UL, 27, "1", 1L);
                        yield return new TestCaseData(1UL, 28, "1", 1L);
                        yield return new TestCaseData(1UL, 29, "1", 1L);
                        yield return new TestCaseData(1UL, 30, "1", 1L);
                        yield return new TestCaseData(1UL, 31, "1", 1L);
                        yield return new TestCaseData(1UL, 32, "1", 1L);
                        yield return new TestCaseData(1UL, 33, "1", 1L);
                        yield return new TestCaseData(1UL, 34, "1", 1L);
                        yield return new TestCaseData(1UL, 35, "1", 1L);
                        yield return new TestCaseData(1UL, 36, "1", 1L);

                        yield return new TestCaseData(10UL, 2, "1010", 10L);
                        yield return new TestCaseData(10UL, 3, "101", 10L);
                        yield return new TestCaseData(10UL, 4, "22", 10L);
                        yield return new TestCaseData(10UL, 5, "20", 10L);
                        yield return new TestCaseData(10UL, 6, "14", 10L);
                        yield return new TestCaseData(10UL, 7, "13", 10L);
                        yield return new TestCaseData(10UL, 8, "12", 10L);
                        yield return new TestCaseData(10UL, 9, "11", 10L);
                        yield return new TestCaseData(10UL, 10, "10", 10L);
                        yield return new TestCaseData(10UL, 11, "a", 10L);
                        yield return new TestCaseData(10UL, 12, "a", 10L);
                        yield return new TestCaseData(10UL, 13, "a", 10L);
                        yield return new TestCaseData(10UL, 14, "a", 10L);
                        yield return new TestCaseData(10UL, 15, "a", 10L);
                        yield return new TestCaseData(10UL, 16, "a", 10L);
                        yield return new TestCaseData(10UL, 17, "a", 10L);
                        yield return new TestCaseData(10UL, 18, "a", 10L);
                        yield return new TestCaseData(10UL, 19, "a", 10L);
                        yield return new TestCaseData(10UL, 20, "a", 10L);
                        yield return new TestCaseData(10UL, 21, "a", 10L);
                        yield return new TestCaseData(10UL, 22, "a", 10L);
                        yield return new TestCaseData(10UL, 23, "a", 10L);
                        yield return new TestCaseData(10UL, 24, "a", 10L);
                        yield return new TestCaseData(10UL, 25, "a", 10L);
                        yield return new TestCaseData(10UL, 26, "a", 10L);
                        yield return new TestCaseData(10UL, 27, "a", 10L);
                        yield return new TestCaseData(10UL, 28, "a", 10L);
                        yield return new TestCaseData(10UL, 29, "a", 10L);
                        yield return new TestCaseData(10UL, 30, "a", 10L);
                        yield return new TestCaseData(10UL, 31, "a", 10L);
                        yield return new TestCaseData(10UL, 32, "a", 10L);
                        yield return new TestCaseData(10UL, 33, "a", 10L);
                        yield return new TestCaseData(10UL, 34, "a", 10L);
                        yield return new TestCaseData(10UL, 35, "a", 10L);
                        yield return new TestCaseData(10UL, 36, "a", 10L);

                        // int.MaxValue - 1

                        yield return new TestCaseData(2147483646UL, 2, "1111111111111111111111111111110", 2147483646L);
                        yield return new TestCaseData(2147483646UL, 3, "12112122212110202100", 2147483646L);
                        yield return new TestCaseData(2147483646UL, 4, "1333333333333332", 2147483646L);
                        yield return new TestCaseData(2147483646UL, 5, "13344223434041", 2147483646L);
                        yield return new TestCaseData(2147483646UL, 6, "553032005530", 2147483646L);
                        yield return new TestCaseData(2147483646UL, 7, "104134211160", 2147483646L);
                        yield return new TestCaseData(2147483646UL, 8, "17777777776", 2147483646L);
                        yield return new TestCaseData(2147483646UL, 9, "5478773670", 2147483646L);
                        yield return new TestCaseData(2147483646UL, 10, "2147483646", 2147483646L);
                        yield return new TestCaseData(2147483646UL, 11, "a02220280", 2147483646L);
                        yield return new TestCaseData(2147483646UL, 12, "4bb2308a6", 2147483646L);
                        yield return new TestCaseData(2147483646UL, 13, "282ba4aa9", 2147483646L);
                        yield return new TestCaseData(2147483646UL, 14, "1652ca930", 2147483646L);
                        yield return new TestCaseData(2147483646UL, 15, "c87e66b6", 2147483646L);
                        yield return new TestCaseData(2147483646UL, 16, "7ffffffe", 2147483646L);
                        yield return new TestCaseData(2147483646UL, 17, "53g7f547", 2147483646L);
                        yield return new TestCaseData(2147483646UL, 18, "3928g3h0", 2147483646L);
                        yield return new TestCaseData(2147483646UL, 19, "27c57h31", 2147483646L);
                        yield return new TestCaseData(2147483646UL, 20, "1db1f926", 2147483646L);
                        yield return new TestCaseData(2147483646UL, 21, "140h2d90", 2147483646L);
                        yield return new TestCaseData(2147483646UL, 22, "ikf5bf0", 2147483646L);
                        yield return new TestCaseData(2147483646UL, 23, "ebelf94", 2147483646L);
                        yield return new TestCaseData(2147483646UL, 24, "b5gge56", 2147483646L);
                        yield return new TestCaseData(2147483646UL, 25, "8jmdnkl", 2147483646L);
                        yield return new TestCaseData(2147483646UL, 26, "6oj8iom", 2147483646L);
                        yield return new TestCaseData(2147483646UL, 27, "5ehnck9", 2147483646L);
                        yield return new TestCaseData(2147483646UL, 28, "4clm98e", 2147483646L);
                        yield return new TestCaseData(2147483646UL, 29, "3hk7986", 2147483646L);
                        yield return new TestCaseData(2147483646UL, 30, "2sb6cs6", 2147483646L);
                        yield return new TestCaseData(2147483646UL, 31, "2d09uc0", 2147483646L);
                        yield return new TestCaseData(2147483646UL, 32, "1vvvvvu", 2147483646L);
                        yield return new TestCaseData(2147483646UL, 33, "1lsqtl0", 2147483646L);
                        yield return new TestCaseData(2147483646UL, 34, "1d8xqro", 2147483646L);
                        yield return new TestCaseData(2147483646UL, 35, "15v22ul", 2147483646L);
                        yield return new TestCaseData(2147483646UL, 36, "zik0zi", 2147483646L);

                        // int.MaxValue

                        yield return new TestCaseData(2147483647UL, 2, "1111111111111111111111111111111", 2147483647L);
                        yield return new TestCaseData(2147483647UL, 3, "12112122212110202101", 2147483647L);
                        yield return new TestCaseData(2147483647UL, 4, "1333333333333333", 2147483647L);
                        yield return new TestCaseData(2147483647UL, 5, "13344223434042", 2147483647L);
                        yield return new TestCaseData(2147483647UL, 6, "553032005531", 2147483647L);
                        yield return new TestCaseData(2147483647UL, 7, "104134211161", 2147483647L);
                        yield return new TestCaseData(2147483647UL, 8, "17777777777", 2147483647L);
                        yield return new TestCaseData(2147483647UL, 9, "5478773671", 2147483647L);
                        yield return new TestCaseData(2147483647UL, 10, "2147483647", 2147483647L);
                        yield return new TestCaseData(2147483647UL, 11, "a02220281", 2147483647L);
                        yield return new TestCaseData(2147483647UL, 12, "4bb2308a7", 2147483647L);
                        yield return new TestCaseData(2147483647UL, 13, "282ba4aaa", 2147483647L);
                        yield return new TestCaseData(2147483647UL, 14, "1652ca931", 2147483647L);
                        yield return new TestCaseData(2147483647UL, 15, "c87e66b7", 2147483647L);
                        yield return new TestCaseData(2147483647UL, 16, "7fffffff", 2147483647L);
                        yield return new TestCaseData(2147483647UL, 17, "53g7f548", 2147483647L);
                        yield return new TestCaseData(2147483647UL, 18, "3928g3h1", 2147483647L);
                        yield return new TestCaseData(2147483647UL, 19, "27c57h32", 2147483647L);
                        yield return new TestCaseData(2147483647UL, 20, "1db1f927", 2147483647L);
                        yield return new TestCaseData(2147483647UL, 21, "140h2d91", 2147483647L);
                        yield return new TestCaseData(2147483647UL, 22, "ikf5bf1", 2147483647L);
                        yield return new TestCaseData(2147483647UL, 23, "ebelf95", 2147483647L);
                        yield return new TestCaseData(2147483647UL, 24, "b5gge57", 2147483647L);
                        yield return new TestCaseData(2147483647UL, 25, "8jmdnkm", 2147483647L);
                        yield return new TestCaseData(2147483647UL, 26, "6oj8ion", 2147483647L);
                        yield return new TestCaseData(2147483647UL, 27, "5ehncka", 2147483647L);
                        yield return new TestCaseData(2147483647UL, 28, "4clm98f", 2147483647L);
                        yield return new TestCaseData(2147483647UL, 29, "3hk7987", 2147483647L);
                        yield return new TestCaseData(2147483647UL, 30, "2sb6cs7", 2147483647L);
                        yield return new TestCaseData(2147483647UL, 31, "2d09uc1", 2147483647L);
                        yield return new TestCaseData(2147483647UL, 32, "1vvvvvv", 2147483647L);
                        yield return new TestCaseData(2147483647UL, 33, "1lsqtl1", 2147483647L);
                        yield return new TestCaseData(2147483647UL, 34, "1d8xqrp", 2147483647L);
                        yield return new TestCaseData(2147483647UL, 35, "15v22um", 2147483647L);
                        yield return new TestCaseData(2147483647UL, 36, "zik0zj", 2147483647L);

                        // int.MaxValue + 1

                        yield return new TestCaseData(2147483648UL, 2, "10000000000000000000000000000000", 2147483648L);
                        yield return new TestCaseData(2147483648UL, 3, "12112122212110202102", 2147483648L);
                        yield return new TestCaseData(2147483648UL, 4, "2000000000000000", 2147483648L);
                        yield return new TestCaseData(2147483648UL, 5, "13344223434043", 2147483648L);
                        yield return new TestCaseData(2147483648UL, 6, "553032005532", 2147483648L);
                        yield return new TestCaseData(2147483648UL, 7, "104134211162", 2147483648L);
                        yield return new TestCaseData(2147483648UL, 8, "20000000000", 2147483648L);
                        yield return new TestCaseData(2147483648UL, 9, "5478773672", 2147483648L);
                        yield return new TestCaseData(2147483648UL, 10, "2147483648", 2147483648L);
                        yield return new TestCaseData(2147483648UL, 11, "a02220282", 2147483648L);
                        yield return new TestCaseData(2147483648UL, 12, "4bb2308a8", 2147483648L);
                        yield return new TestCaseData(2147483648UL, 13, "282ba4aab", 2147483648L);
                        yield return new TestCaseData(2147483648UL, 14, "1652ca932", 2147483648L);
                        yield return new TestCaseData(2147483648UL, 15, "c87e66b8", 2147483648L);
                        yield return new TestCaseData(2147483648UL, 16, "80000000", 2147483648L);
                        yield return new TestCaseData(2147483648UL, 17, "53g7f549", 2147483648L);
                        yield return new TestCaseData(2147483648UL, 18, "3928g3h2", 2147483648L);
                        yield return new TestCaseData(2147483648UL, 19, "27c57h33", 2147483648L);
                        yield return new TestCaseData(2147483648UL, 20, "1db1f928", 2147483648L);
                        yield return new TestCaseData(2147483648UL, 21, "140h2d92", 2147483648L);
                        yield return new TestCaseData(2147483648UL, 22, "ikf5bf2", 2147483648L);
                        yield return new TestCaseData(2147483648UL, 23, "ebelf96", 2147483648L);
                        yield return new TestCaseData(2147483648UL, 24, "b5gge58", 2147483648L);
                        yield return new TestCaseData(2147483648UL, 25, "8jmdnkn", 2147483648L);
                        yield return new TestCaseData(2147483648UL, 26, "6oj8ioo", 2147483648L);
                        yield return new TestCaseData(2147483648UL, 27, "5ehnckb", 2147483648L);
                        yield return new TestCaseData(2147483648UL, 28, "4clm98g", 2147483648L);
                        yield return new TestCaseData(2147483648UL, 29, "3hk7988", 2147483648L);
                        yield return new TestCaseData(2147483648UL, 30, "2sb6cs8", 2147483648L);
                        yield return new TestCaseData(2147483648UL, 31, "2d09uc2", 2147483648L);
                        yield return new TestCaseData(2147483648UL, 32, "2000000", 2147483648L);
                        yield return new TestCaseData(2147483648UL, 33, "1lsqtl2", 2147483648L);
                        yield return new TestCaseData(2147483648UL, 34, "1d8xqrq", 2147483648L);
                        yield return new TestCaseData(2147483648UL, 35, "15v22un", 2147483648L);
                        yield return new TestCaseData(2147483648UL, 36, "zik0zk", 2147483648L);

                        yield return new TestCaseData(uint.MaxValue - 1UL, 2, "11111111111111111111111111111110", 4294967294L);
                        yield return new TestCaseData(uint.MaxValue - 1UL, 3, "102002022201221111202", 4294967294L);
                        yield return new TestCaseData(uint.MaxValue - 1UL, 4, "3333333333333332", 4294967294L);
                        yield return new TestCaseData(uint.MaxValue - 1UL, 5, "32244002423134", 4294967294L);
                        yield return new TestCaseData(uint.MaxValue - 1UL, 6, "1550104015502", 4294967294L);
                        yield return new TestCaseData(uint.MaxValue - 1UL, 7, "211301422352", 4294967294L);
                        yield return new TestCaseData(uint.MaxValue - 1UL, 8, "37777777776", 4294967294L);
                        yield return new TestCaseData(uint.MaxValue - 1UL, 9, "12068657452", 4294967294L);
                        yield return new TestCaseData(uint.MaxValue - 1UL, 10, "4294967294", 4294967294L);
                        yield return new TestCaseData(uint.MaxValue - 1UL, 11, "1904440552", 4294967294L);
                        yield return new TestCaseData(uint.MaxValue - 1UL, 12, "9ba461592", 4294967294L);
                        yield return new TestCaseData(uint.MaxValue - 1UL, 13, "535a79887", 4294967294L);
                        yield return new TestCaseData(uint.MaxValue - 1UL, 14, "2ca5b7462", 4294967294L);
                        yield return new TestCaseData(uint.MaxValue - 1UL, 15, "1a20dcd7e", 4294967294L);
                        yield return new TestCaseData(uint.MaxValue - 1UL, 16, "fffffffe", 4294967294L);
                        yield return new TestCaseData(uint.MaxValue - 1UL, 17, "a7ffda8g", 4294967294L);
                        yield return new TestCaseData(uint.MaxValue - 1UL, 18, "704he7g2", 4294967294L);
                        yield return new TestCaseData(uint.MaxValue - 1UL, 19, "4f5aff64", 4294967294L);
                        yield return new TestCaseData(uint.MaxValue - 1UL, 20, "3723ai4e", 4294967294L);
                        yield return new TestCaseData(uint.MaxValue - 1UL, 21, "281d55i2", 4294967294L);
                        yield return new TestCaseData(uint.MaxValue - 1UL, 22, "1fj8b182", 4294967294L);
                        yield return new TestCaseData(uint.MaxValue - 1UL, 23, "1606k7ia", 4294967294L);
                        yield return new TestCaseData(uint.MaxValue - 1UL, 24, "mb994ae", 4294967294L);
                        yield return new TestCaseData(uint.MaxValue - 1UL, 25, "hek2mgj", 4294967294L);
                        yield return new TestCaseData(uint.MaxValue - 1UL, 26, "dnchbnk", 4294967294L);
                        yield return new TestCaseData(uint.MaxValue - 1UL, 27, "b28jpdk", 4294967294L);
                        yield return new TestCaseData(uint.MaxValue - 1UL, 28, "8pfgih2", 4294967294L);
                        yield return new TestCaseData(uint.MaxValue - 1UL, 29, "76beige", 4294967294L);
                        yield return new TestCaseData(uint.MaxValue - 1UL, 30, "5qmcpqe", 4294967294L);
                        yield return new TestCaseData(uint.MaxValue - 1UL, 31, "4q0jto2", 4294967294L);
                        yield return new TestCaseData(uint.MaxValue - 1UL, 32, "3vvvvvu", 4294967294L);
                        yield return new TestCaseData(uint.MaxValue - 1UL, 33, "3aokq92", 4294967294L);
                        yield return new TestCaseData(uint.MaxValue - 1UL, 34, "2qhxjlg", 4294967294L);
                        yield return new TestCaseData(uint.MaxValue - 1UL, 35, "2br45q9", 4294967294L);
                        yield return new TestCaseData(uint.MaxValue - 1UL, 36, "1z141z2", 4294967294L);

                        yield return new TestCaseData(uint.MaxValue, 2, "11111111111111111111111111111111", 4294967295L);
                        yield return new TestCaseData(uint.MaxValue, 3, "102002022201221111210", 4294967295L);
                        yield return new TestCaseData(uint.MaxValue, 4, "3333333333333333", 4294967295L);
                        yield return new TestCaseData(uint.MaxValue, 5, "32244002423140", 4294967295L);
                        yield return new TestCaseData(uint.MaxValue, 6, "1550104015503", 4294967295L);
                        yield return new TestCaseData(uint.MaxValue, 7, "211301422353", 4294967295L);
                        yield return new TestCaseData(uint.MaxValue, 8, "37777777777", 4294967295L);
                        yield return new TestCaseData(uint.MaxValue, 9, "12068657453", 4294967295L);
                        yield return new TestCaseData(uint.MaxValue, 10, "4294967295", 4294967295L);
                        yield return new TestCaseData(uint.MaxValue, 11, "1904440553", 4294967295L);
                        yield return new TestCaseData(uint.MaxValue, 12, "9ba461593", 4294967295L);
                        yield return new TestCaseData(uint.MaxValue, 13, "535a79888", 4294967295L);
                        yield return new TestCaseData(uint.MaxValue, 14, "2ca5b7463", 4294967295L);
                        yield return new TestCaseData(uint.MaxValue, 15, "1a20dcd80", 4294967295L);
                        yield return new TestCaseData(uint.MaxValue, 16, "ffffffff", 4294967295L);
                        yield return new TestCaseData(uint.MaxValue, 17, "a7ffda90", 4294967295L);
                        yield return new TestCaseData(uint.MaxValue, 18, "704he7g3", 4294967295L);
                        yield return new TestCaseData(uint.MaxValue, 19, "4f5aff65", 4294967295L);
                        yield return new TestCaseData(uint.MaxValue, 20, "3723ai4f", 4294967295L);
                        yield return new TestCaseData(uint.MaxValue, 21, "281d55i3", 4294967295L);
                        yield return new TestCaseData(uint.MaxValue, 22, "1fj8b183", 4294967295L);
                        yield return new TestCaseData(uint.MaxValue, 23, "1606k7ib", 4294967295L);
                        yield return new TestCaseData(uint.MaxValue, 24, "mb994af", 4294967295L);
                        yield return new TestCaseData(uint.MaxValue, 25, "hek2mgk", 4294967295L);
                        yield return new TestCaseData(uint.MaxValue, 26, "dnchbnl", 4294967295L);
                        yield return new TestCaseData(uint.MaxValue, 27, "b28jpdl", 4294967295L);
                        yield return new TestCaseData(uint.MaxValue, 28, "8pfgih3", 4294967295L);
                        yield return new TestCaseData(uint.MaxValue, 29, "76beigf", 4294967295L);
                        yield return new TestCaseData(uint.MaxValue, 30, "5qmcpqf", 4294967295L);
                        yield return new TestCaseData(uint.MaxValue, 31, "4q0jto3", 4294967295L);
                        yield return new TestCaseData(uint.MaxValue, 32, "3vvvvvv", 4294967295L);
                        yield return new TestCaseData(uint.MaxValue, 33, "3aokq93", 4294967295L);
                        yield return new TestCaseData(uint.MaxValue, 34, "2qhxjlh", 4294967295L);
                        yield return new TestCaseData(uint.MaxValue, 35, "2br45qa", 4294967295L);
                        yield return new TestCaseData(uint.MaxValue, 36, "1z141z3", 4294967295L);

                        yield return new TestCaseData(long.MaxValue - 1UL, 2, "111111111111111111111111111111111111111111111111111111111111110", 9223372036854775806L);
                        yield return new TestCaseData(long.MaxValue - 1UL, 3, "2021110011022210012102010021220101220220", 9223372036854775806L);
                        yield return new TestCaseData(long.MaxValue - 1UL, 4, "13333333333333333333333333333332", 9223372036854775806L);
                        yield return new TestCaseData(long.MaxValue - 1UL, 5, "1104332401304422434310311211", 9223372036854775806L);
                        yield return new TestCaseData(long.MaxValue - 1UL, 6, "1540241003031030222122210", 9223372036854775806L);
                        yield return new TestCaseData(long.MaxValue - 1UL, 7, "22341010611245052052266", 9223372036854775806L);
                        yield return new TestCaseData(long.MaxValue - 1UL, 8, "777777777777777777776", 9223372036854775806L);
                        yield return new TestCaseData(long.MaxValue - 1UL, 9, "67404283172107811826", 9223372036854775806L);
                        yield return new TestCaseData(long.MaxValue - 1UL, 10, "9223372036854775806", 9223372036854775806L);
                        yield return new TestCaseData(long.MaxValue - 1UL, 11, "1728002635214590696", 9223372036854775806L);
                        yield return new TestCaseData(long.MaxValue - 1UL, 12, "41a792678515120366", 9223372036854775806L);
                        yield return new TestCaseData(long.MaxValue - 1UL, 13, "10b269549075433c36", 9223372036854775806L);
                        yield return new TestCaseData(long.MaxValue - 1UL, 14, "4340724c6c71dc7a6", 9223372036854775806L);
                        yield return new TestCaseData(long.MaxValue - 1UL, 15, "160e2ad3246366806", 9223372036854775806L);
                        yield return new TestCaseData(long.MaxValue - 1UL, 16, "7ffffffffffffffe", 9223372036854775806L);
                        yield return new TestCaseData(long.MaxValue - 1UL, 17, "33d3d8307b214007", 9223372036854775806L);
                        yield return new TestCaseData(long.MaxValue - 1UL, 18, "16agh595df825fa6", 9223372036854775806L);
                        yield return new TestCaseData(long.MaxValue - 1UL, 19, "ba643dci0ffeehg", 9223372036854775806L);
                        yield return new TestCaseData(long.MaxValue - 1UL, 20, "5cbfjia3fh26ja6", 9223372036854775806L);
                        yield return new TestCaseData(long.MaxValue - 1UL, 21, "2heiciiie82dh96", 9223372036854775806L);
                        yield return new TestCaseData(long.MaxValue - 1UL, 22, "1adaibb21dckfa6", 9223372036854775806L);
                        yield return new TestCaseData(long.MaxValue - 1UL, 23, "i6k448cf4192c1", 9223372036854775806L);
                        yield return new TestCaseData(long.MaxValue - 1UL, 24, "acd772jnc9l0l6", 9223372036854775806L);
                        yield return new TestCaseData(long.MaxValue - 1UL, 25, "64ie1focnn5g76", 9223372036854775806L);
                        yield return new TestCaseData(long.MaxValue - 1UL, 26, "3igoecjbmca686", 9223372036854775806L);
                        yield return new TestCaseData(long.MaxValue - 1UL, 27, "27c48l5b37oaoo", 9223372036854775806L);
                        yield return new TestCaseData(long.MaxValue - 1UL, 28, "1bk39f3ah3dmq6", 9223372036854775806L);
                        yield return new TestCaseData(long.MaxValue - 1UL, 29, "q1se8f0m04isa", 9223372036854775806L);
                        yield return new TestCaseData(long.MaxValue - 1UL, 30, "hajppbc1fc206", 9223372036854775806L);
                        yield return new TestCaseData(long.MaxValue - 1UL, 31, "bm03i95hia436", 9223372036854775806L);
                        yield return new TestCaseData(long.MaxValue - 1UL, 32, "7vvvvvvvvvvvu", 9223372036854775806L);
                        yield return new TestCaseData(long.MaxValue - 1UL, 33, "5hg4ck9jd4u36", 9223372036854775806L);
                        yield return new TestCaseData(long.MaxValue - 1UL, 34, "3tdtk1v8j6tpo", 9223372036854775806L);
                        yield return new TestCaseData(long.MaxValue - 1UL, 35, "2pijmikexrxp6", 9223372036854775806L);
                        yield return new TestCaseData(long.MaxValue - 1UL, 36, "1y2p0ij32e8e6", 9223372036854775806L);

                        yield return new TestCaseData(long.MaxValue + 0UL, 2, "111111111111111111111111111111111111111111111111111111111111111", 9223372036854775807L);
                        yield return new TestCaseData(long.MaxValue + 0UL, 3, "2021110011022210012102010021220101220221", 9223372036854775807L);
                        yield return new TestCaseData(long.MaxValue + 0UL, 4, "13333333333333333333333333333333", 9223372036854775807L);
                        yield return new TestCaseData(long.MaxValue + 0UL, 5, "1104332401304422434310311212", 9223372036854775807L);
                        yield return new TestCaseData(long.MaxValue + 0UL, 6, "1540241003031030222122211", 9223372036854775807L);
                        yield return new TestCaseData(long.MaxValue + 0UL, 7, "22341010611245052052300", 9223372036854775807L);
                        yield return new TestCaseData(long.MaxValue + 0UL, 8, "777777777777777777777", 9223372036854775807L);
                        yield return new TestCaseData(long.MaxValue + 0UL, 9, "67404283172107811827", 9223372036854775807L);
                        yield return new TestCaseData(long.MaxValue + 0UL, 10, "9223372036854775807", 9223372036854775807L);
                        yield return new TestCaseData(long.MaxValue + 0UL, 11, "1728002635214590697", 9223372036854775807L);
                        yield return new TestCaseData(long.MaxValue + 0UL, 12, "41a792678515120367", 9223372036854775807L);
                        yield return new TestCaseData(long.MaxValue + 0UL, 13, "10b269549075433c37", 9223372036854775807L);
                        yield return new TestCaseData(long.MaxValue + 0UL, 14, "4340724c6c71dc7a7", 9223372036854775807L);
                        yield return new TestCaseData(long.MaxValue + 0UL, 15, "160e2ad3246366807", 9223372036854775807L);
                        yield return new TestCaseData(long.MaxValue + 0UL, 16, "7fffffffffffffff", 9223372036854775807L);
                        yield return new TestCaseData(long.MaxValue + 0UL, 17, "33d3d8307b214008", 9223372036854775807L);
                        yield return new TestCaseData(long.MaxValue + 0UL, 18, "16agh595df825fa7", 9223372036854775807L);
                        yield return new TestCaseData(long.MaxValue + 0UL, 19, "ba643dci0ffeehh", 9223372036854775807L);
                        yield return new TestCaseData(long.MaxValue + 0UL, 20, "5cbfjia3fh26ja7", 9223372036854775807L);
                        yield return new TestCaseData(long.MaxValue + 0UL, 21, "2heiciiie82dh97", 9223372036854775807L);
                        yield return new TestCaseData(long.MaxValue + 0UL, 22, "1adaibb21dckfa7", 9223372036854775807L);
                        yield return new TestCaseData(long.MaxValue + 0UL, 23, "i6k448cf4192c2", 9223372036854775807L);
                        yield return new TestCaseData(long.MaxValue + 0UL, 24, "acd772jnc9l0l7", 9223372036854775807L);
                        yield return new TestCaseData(long.MaxValue + 0UL, 25, "64ie1focnn5g77", 9223372036854775807L);
                        yield return new TestCaseData(long.MaxValue + 0UL, 26, "3igoecjbmca687", 9223372036854775807L);
                        yield return new TestCaseData(long.MaxValue + 0UL, 27, "27c48l5b37oaop", 9223372036854775807L);
                        yield return new TestCaseData(long.MaxValue + 0UL, 28, "1bk39f3ah3dmq7", 9223372036854775807L);
                        yield return new TestCaseData(long.MaxValue + 0UL, 29, "q1se8f0m04isb", 9223372036854775807L);
                        yield return new TestCaseData(long.MaxValue + 0UL, 30, "hajppbc1fc207", 9223372036854775807L);
                        yield return new TestCaseData(long.MaxValue + 0UL, 31, "bm03i95hia437", 9223372036854775807L);
                        yield return new TestCaseData(long.MaxValue + 0UL, 32, "7vvvvvvvvvvvv", 9223372036854775807L);
                        yield return new TestCaseData(long.MaxValue + 0UL, 33, "5hg4ck9jd4u37", 9223372036854775807L);
                        yield return new TestCaseData(long.MaxValue + 0UL, 34, "3tdtk1v8j6tpp", 9223372036854775807L);
                        yield return new TestCaseData(long.MaxValue + 0UL, 35, "2pijmikexrxp7", 9223372036854775807L);
                        yield return new TestCaseData(long.MaxValue + 0UL, 36, "1y2p0ij32e8e7", 9223372036854775807L);

                        yield return new TestCaseData(long.MaxValue + 1UL, 2, "1000000000000000000000000000000000000000000000000000000000000000", -9223372036854775808L);
                        yield return new TestCaseData(long.MaxValue + 1UL, 3, "2021110011022210012102010021220101220222", -9223372036854775808L);
                        yield return new TestCaseData(long.MaxValue + 1UL, 4, "20000000000000000000000000000000", -9223372036854775808L);
                        yield return new TestCaseData(long.MaxValue + 1UL, 5, "1104332401304422434310311213", -9223372036854775808L);
                        yield return new TestCaseData(long.MaxValue + 1UL, 6, "1540241003031030222122212", -9223372036854775808L);
                        yield return new TestCaseData(long.MaxValue + 1UL, 7, "22341010611245052052301", -9223372036854775808L);
                        yield return new TestCaseData(long.MaxValue + 1UL, 8, "1000000000000000000000", -9223372036854775808L);
                        yield return new TestCaseData(long.MaxValue + 1UL, 9, "67404283172107811828", -9223372036854775808L);
                        yield return new TestCaseData(long.MaxValue + 1UL, 10, "9223372036854775808", -9223372036854775808L);
                        yield return new TestCaseData(long.MaxValue + 1UL, 11, "1728002635214590698", -9223372036854775808L);
                        yield return new TestCaseData(long.MaxValue + 1UL, 12, "41a792678515120368", -9223372036854775808L);
                        yield return new TestCaseData(long.MaxValue + 1UL, 13, "10b269549075433c38", -9223372036854775808L);
                        yield return new TestCaseData(long.MaxValue + 1UL, 14, "4340724c6c71dc7a8", -9223372036854775808L);
                        yield return new TestCaseData(long.MaxValue + 1UL, 15, "160e2ad3246366808", -9223372036854775808L);
                        yield return new TestCaseData(long.MaxValue + 1UL, 16, "8000000000000000", -9223372036854775808L);
                        yield return new TestCaseData(long.MaxValue + 1UL, 17, "33d3d8307b214009", -9223372036854775808L);
                        yield return new TestCaseData(long.MaxValue + 1UL, 18, "16agh595df825fa8", -9223372036854775808L);
                        yield return new TestCaseData(long.MaxValue + 1UL, 19, "ba643dci0ffeehi", -9223372036854775808L);
                        yield return new TestCaseData(long.MaxValue + 1UL, 20, "5cbfjia3fh26ja8", -9223372036854775808L);
                        yield return new TestCaseData(long.MaxValue + 1UL, 21, "2heiciiie82dh98", -9223372036854775808L);
                        yield return new TestCaseData(long.MaxValue + 1UL, 22, "1adaibb21dckfa8", -9223372036854775808L);
                        yield return new TestCaseData(long.MaxValue + 1UL, 23, "i6k448cf4192c3", -9223372036854775808L);
                        yield return new TestCaseData(long.MaxValue + 1UL, 24, "acd772jnc9l0l8", -9223372036854775808L);
                        yield return new TestCaseData(long.MaxValue + 1UL, 25, "64ie1focnn5g78", -9223372036854775808L);
                        yield return new TestCaseData(long.MaxValue + 1UL, 26, "3igoecjbmca688", -9223372036854775808L);
                        yield return new TestCaseData(long.MaxValue + 1UL, 27, "27c48l5b37oaoq", -9223372036854775808L);
                        yield return new TestCaseData(long.MaxValue + 1UL, 28, "1bk39f3ah3dmq8", -9223372036854775808L);
                        yield return new TestCaseData(long.MaxValue + 1UL, 29, "q1se8f0m04isc", -9223372036854775808L);
                        yield return new TestCaseData(long.MaxValue + 1UL, 30, "hajppbc1fc208", -9223372036854775808L);
                        yield return new TestCaseData(long.MaxValue + 1UL, 31, "bm03i95hia438", -9223372036854775808L);
                        yield return new TestCaseData(long.MaxValue + 1UL, 32, "8000000000000", -9223372036854775808L);
                        yield return new TestCaseData(long.MaxValue + 1UL, 33, "5hg4ck9jd4u38", -9223372036854775808L);
                        yield return new TestCaseData(long.MaxValue + 1UL, 34, "3tdtk1v8j6tpq", -9223372036854775808L);
                        yield return new TestCaseData(long.MaxValue + 1UL, 35, "2pijmikexrxp8", -9223372036854775808L);
                        yield return new TestCaseData(long.MaxValue + 1UL, 36, "1y2p0ij32e8e8", -9223372036854775808L);

                        yield return new TestCaseData((2UL ^ 64) - 1UL, 2, "1111111111111111111111111111111111111111111111111111111111111111", -1L);
                        yield return new TestCaseData((2UL ^ 64) - 1UL, 3, "11112220022122120101211020120210210211220", -1L);
                        yield return new TestCaseData((2UL ^ 64) - 1UL, 4, "33333333333333333333333333333333", -1L);
                        yield return new TestCaseData((2UL ^ 64) - 1UL, 5, "2214220303114400424121122430", -1L);
                        yield return new TestCaseData((2UL ^ 64) - 1UL, 6, "3520522010102100444244423", -1L);
                        yield return new TestCaseData((2UL ^ 64) - 1UL, 7, "45012021522523134134601", -1L);
                        yield return new TestCaseData((2UL ^ 64) - 1UL, 8, "1777777777777777777777", -1L);
                        yield return new TestCaseData((2UL ^ 64) - 1UL, 9, "145808576354216723756", -1L);
                        yield return new TestCaseData((2UL ^ 64) - 1UL, 10, "18446744073709551615", -1L);
                        yield return new TestCaseData((2UL ^ 64) - 1UL, 11, "335500516a429071284", -1L);
                        yield return new TestCaseData((2UL ^ 64) - 1UL, 12, "839365134a2a240713", -1L);
                        yield return new TestCaseData((2UL ^ 64) - 1UL, 13, "219505a9511a867b72", -1L);
                        yield return new TestCaseData((2UL ^ 64) - 1UL, 14, "8681049adb03db171", -1L);
                        yield return new TestCaseData((2UL ^ 64) - 1UL, 15, "2c1d56b648c6cd110", -1L);
                        yield return new TestCaseData((2UL ^ 64) - 1UL, 16, "ffffffffffffffff", -1L);
                        yield return new TestCaseData((2UL ^ 64) - 1UL, 17, "67979g60f5428010", -1L);
                        yield return new TestCaseData((2UL ^ 64) - 1UL, 18, "2d3fgb0b9cg4bd2f", -1L);
                        yield return new TestCaseData((2UL ^ 64) - 1UL, 19, "141c8786h1ccaagg", -1L);
                        yield return new TestCaseData((2UL ^ 64) - 1UL, 20, "b53bjh07be4dj0f", -1L);
                        yield return new TestCaseData((2UL ^ 64) - 1UL, 21, "5e8g4ggg7g56dif", -1L);
                        yield return new TestCaseData((2UL ^ 64) - 1UL, 22, "2l4lf104353j8kf", -1L);
                        yield return new TestCaseData((2UL ^ 64) - 1UL, 23, "1ddh88h2782i515", -1L);
                        yield return new TestCaseData((2UL ^ 64) - 1UL, 24, "l12ee5fn0ji1if", -1L);
                        yield return new TestCaseData((2UL ^ 64) - 1UL, 25, "c9c336o0mlb7ef", -1L);
                        yield return new TestCaseData((2UL ^ 64) - 1UL, 26, "7b7n2pcniokcgf", -1L);
                        yield return new TestCaseData((2UL ^ 64) - 1UL, 27, "4eo8hfam6fllmo", -1L);
                        yield return new TestCaseData((2UL ^ 64) - 1UL, 28, "2nc6j26l66rhof", -1L);
                        yield return new TestCaseData((2UL ^ 64) - 1UL, 29, "1n3rsh11f098rn", -1L);
                        yield return new TestCaseData((2UL ^ 64) - 1UL, 30, "14l9lkmo30o40f", -1L);
                        yield return new TestCaseData((2UL ^ 64) - 1UL, 31, "nd075ib45k86f", -1L);
                        yield return new TestCaseData((2UL ^ 64) - 1UL, 32, "fvvvvvvvvvvvv", -1L);
                        yield return new TestCaseData((2UL ^ 64) - 1UL, 33, "b1w8p7j5q9r6f", -1L);
                        yield return new TestCaseData((2UL ^ 64) - 1UL, 34, "7orp63sh4dphh", -1L);
                        yield return new TestCaseData((2UL ^ 64) - 1UL, 35, "5g24a25twkwff", -1L);
                        yield return new TestCaseData((2UL ^ 64) - 1UL, 36, "3w5e11264sgsf", -1L);
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
                        yield return new TestCaseData(typeof(OverflowException), "18446744073709551616", 10); // TWO.pow(64).toString()

                        // test case known at one time to fail
                        yield return new TestCaseData(typeof(OverflowException), "1234567890abcdef1", 16);
                        // smallest guard value to overflow: guard = 99 = 11*3*3, radix = 33
                        yield return new TestCaseData(typeof(OverflowException), "b1w8p7j5q9r6g", 33);
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
                        yield return new TestCaseData(typeof(OverflowException), "18446744073709551616", 10); // TWO.pow(64).toString()
                    }
                }
            }

            #endregion ParseUnsignedTestCase

            #region ParseUnsigned_CharSequence_Int32

            public abstract class ParseUnsigned_CharSequence_Int32_TestCase : ParseUnsignedTestCase
            {
                protected abstract long GetResult(string s, int radix);

                [TestCaseSource(typeof(ParseUnsignedTestCase), "UnsignedLongTestCases")]
                public void TestParseUnsigned_String_Int32(ulong value, int radix, string bigString, long expected)
                {
                    long actual = GetResult(bigString, radix);
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
                protected override long GetResult(string s, int radix)
                {
                    return Int64.ParseUnsigned(s, radix);
                }
            }

            #endregion ParseUnsigned_CharSequence_Int32

            #region ParseUnsigned_CharSequence_Int32_Int32_Int32

            public abstract class ParseUnsigned_CharSequence_Int32_Int32_Int32_TestCase : ParseUnsignedTestCase
            {
                protected abstract long GetResult(string s, int startIndex, int length, int radix);

                [TestCaseSource(typeof(ParseUnsignedTestCase), "UnsignedLongTestCases")]
                public void TestParseUnsigned_CharSequence_Int32_Int32_Int32(ulong value, int radix, string bigString, long expected)
                {
                    long actual = GetResult("prefix" + bigString + "suffix", "prefix".Length, bigString.Length, radix);
                    assertEquals(expected, actual);
                }

                [TestCaseSource(typeof(ParseUnsignedTestCase), "ParseUnsigned_CharSequence_Int32_Int32_Int32_ForException_Data")]
                public void TestParseUnsigned_String_Int32_Int32_Int32_ForException(Type expectedExceptionType, string value, int radix)
                {
                    Assert.Throws(expectedExceptionType, () => Int64.ParseUnsigned("prefix" + value + "suffix", "prefix".Length, value?.Length ?? 0, radix));
                }
            }

            public class ParseUnsigned_String_Int32_Int32_Int32 : ParseUnsigned_CharSequence_Int32_Int32_Int32_TestCase
            {
                protected override long GetResult(string s, int startIndex, int length, int radix)
                {
                    return Int64.ParseUnsigned(s, startIndex, length, radix);
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

                        yield return new TestCaseData("" + long.MinValue, long.MinValue);
                        yield return new TestCaseData("" + long.MaxValue, long.MaxValue);

                        yield return new TestCaseData("10", 10L);
                        yield return new TestCaseData("0x10", 16L);
                        yield return new TestCaseData("0X10", 16L);
                        yield return new TestCaseData("010", 8L);
                        yield return new TestCaseData("#10", 16L);

                        yield return new TestCaseData("+10", 10L);
                        yield return new TestCaseData("+0x10", 16L);
                        yield return new TestCaseData("+0X10", 16L);
                        yield return new TestCaseData("+010", 8L);
                        yield return new TestCaseData("+#10", 16L);

                        yield return new TestCaseData("-10", -10L);
                        yield return new TestCaseData("-0x10", -16L);
                        yield return new TestCaseData("-0X10", -16L);
                        yield return new TestCaseData("-010", -8L);
                        yield return new TestCaseData("-#10", -16L);

                        yield return new TestCaseData(Convert.ToString(long.MinValue, 10), long.MinValue);
                        yield return new TestCaseData(Convert.ToString(long.MaxValue, 10), long.MaxValue);

                        // Harmony (Test_decodeLjava_lang_String())

                        yield return new TestCaseData("0", 0L);
                        yield return new TestCaseData("1", 1L);
                        yield return new TestCaseData("-1", -1L);
                        yield return new TestCaseData("0xF", 0xFL);
                        yield return new TestCaseData("#F", 0xFL);
                        yield return new TestCaseData("0XF", 0xFL);
                        yield return new TestCaseData("07", 07L); // J2N: Technically, .NET doesn't recognize octal literals, but this is the same decimal value

                        // Harmony (Test_decodeLjava_lang_String2())

                        yield return new TestCaseData("0xFF", 255L);
                        yield return new TestCaseData("-89000", -89000L);
                        yield return new TestCaseData("0", 0L);
                        yield return new TestCaseData("0x0", 0L);
                        yield return new TestCaseData("-9223372036854775808", unchecked((long)0x8000000000000000L));
                        yield return new TestCaseData("-0x8000000000000000", unchecked((long)0x8000000000000000L));
                        yield return new TestCaseData("9223372036854775807", 0x7fffffffffffffffL);
                        yield return new TestCaseData("0x7fffffffffffffff", 0x7fffffffffffffffL);
                        yield return new TestCaseData("07654321765432", /*07654321765432l*/ 538536569626); // J2N: Octal literals not supported in C#, converted to decimal
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

                        yield return new TestCaseData(typeof(OverflowException), ((BigInteger)long.MinValue - 1).ToString(NumberFormatInfo.InvariantInfo), "Out of range");
                        yield return new TestCaseData(typeof(OverflowException), ((BigInteger)long.MaxValue + 1).ToString(NumberFormatInfo.InvariantInfo), "Out of range");

                        yield return new TestCaseData(typeof(FormatException), "", "Empty String");

                        // Harmony (Test_decodeLjava_lang_String())

                        yield return new TestCaseData(typeof(FormatException), "9.2", "Expected NumberFormatException with floating point string.");
                        yield return new TestCaseData(typeof(FormatException), "", "Expected NumberFormatException with empty string.");
                        //undocumented NPE, but seems consistent across JREs
                        yield return new TestCaseData(typeof(ArgumentNullException), null, "Expected NullPointerException with null string.");

                        // Harmony (Test_decodeLjava_lang_String2())

                        yield return new TestCaseData(typeof(OverflowException), "999999999999999999999999999999999999999999999999999999", "Failed to throw exception for value > ilong");
                        yield return new TestCaseData(typeof(OverflowException), "9223372036854775808", "Failed to throw exception for MAX_VALUE + 1");
                        yield return new TestCaseData(typeof(OverflowException), "-9223372036854775809", "Failed to throw exception for MIN_VALUE - 1");
                        yield return new TestCaseData(typeof(OverflowException), "0x8000000000000000", "Failed to throw exception for hex MAX_VALUE + 1");
                        yield return new TestCaseData(typeof(OverflowException), "-0x8000000000000001", "Failed to throw exception for hex MIN_VALUE - 1");
                        yield return new TestCaseData(typeof(OverflowException), "42325917317067571199", "Failed to throw exception for 42325917317067571199");

                        // Custom

                        yield return new TestCaseData(typeof(OverflowException), "0xffffffffffffffff", "Negative not allowed without negative sign"); // -1 - negative values are not allowed per the docs
                    }
                }
            }

            #endregion DecodeTestCase

            #region Decode_CharSequence

            public abstract class Decode_CharSequence_TestCase : DecodeTestCase
            {
                protected abstract Int64 GetResult(string value);

                [TestCaseSource(typeof(DecodeTestCase), "TestDecode_CharSequence_Data")]
                public virtual void TestDecode_CharSequence(string value, long expected)
                {
                    var actual = GetResult(value);
                    assertEquals($"Int64.Decode(string) failed. String: \"{value}\" Result: {actual}", expected, actual);
                }

                [TestCaseSource(typeof(DecodeTestCase), "TestDecode_CharSequence_ForException_Data")]
                public virtual void TestDecode_CharSequence_ForException(Type expectedExceptionType, string value, string message)
                {
                    Assert.Throws(expectedExceptionType, () => GetResult(value), message);
                }
            }

            public class Decode_String : Decode_CharSequence_TestCase
            {
                protected override Int64 GetResult(string value)
                {
                    return Int64.Decode(value);
                }
            }

            // J2N: ReadOnlySpan<char> not supported at this time on this overload
            //#if FEATURE_READONLYSPAN
            //            public class Decode_ReadOnlySpan : Decode_CharSequence_TestCase
            //            {
            //                protected override Int64 GetResult(string s)
            //                {
            //                    return Int64.Decode(s.AsSpan());
            //                }
            //            }
            //#endif

            #endregion Decode_CharSequence

            #region TryDecode_CharSequence

            public abstract class TryDecode_CharSequence_TestCase : DecodeTestCase
            {
                protected abstract bool GetResult(string value, out Int64 result);

                [TestCaseSource(typeof(DecodeTestCase), "TestDecode_CharSequence_Data")]
                public virtual void TestTryDecode_CharSequence(string value, long expected)
                {
                    assertTrue(GetResult(value, out Int64 actual));
                    assertEquals($"Int64.TryDecode(string, out Int64) failed. String: \"{value}\" Result: {actual}", new Int64(expected), actual);
                }

                [TestCaseSource(typeof(DecodeTestCase), "TestDecode_CharSequence_ForException_Data")]
                public virtual void TestTryDecode_CharSequence_ForException(Type expectedExceptionType, string value, string message)
                {
                    assertFalse(GetResult(value, out Int64 actual));
                    assertEquals(null, actual);
                }
            }

            public class TryDecode_String : TryDecode_CharSequence_TestCase
            {
                protected override bool GetResult(string value, out Int64 result)
                {
                    return Int64.TryDecode(value, out result);
                }
            }

            // J2N: ReadOnlySpan<char> not supported at this time on this overload
            //#if FEATURE_READONLYSPAN
            //            public class TryDecode_ReadOnlySpan : TryDecode_CharSequence_TestCase
            //            {
            //                protected override bool GetResult(string s, out Int64 result)
            //                {
            //                    return Int64.TryDecode(s.AsSpan(), out result);
            //                }
            //            }
            //#endif

            #endregion TryDecode_CharSequence
        }
    }
}
