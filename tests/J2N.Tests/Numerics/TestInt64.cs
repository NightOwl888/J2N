using NUnit.Framework;
using System;

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
            assertEquals("Returned incorrect byte value", 127, l.GetByteValue());
            assertEquals("Returned incorrect byte value", -1, (sbyte)new Int64(long.MaxValue)
                    .GetByteValue()); // J2N: cast required to change the result to negative
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
         * @tests java.lang.Long#decode(java.lang.String)
         */
        [Test]
        public void Test_decodeLjava_lang_String2()
        {
            // Test for method java.lang.Long
            // java.lang.Long.decode(java.lang.String)
            assertEquals("Returned incorrect value for hex string", 255L, Int64.Decode(
                    "0xFF").GetInt64Value());
            assertEquals("Returned incorrect value for dec string", -89000L, Int64.Decode(
                    "-89000").GetInt64Value());
            assertEquals("Returned incorrect value for 0 decimal", 0, Int64.Decode("0")
                    .GetInt64Value());
            assertEquals("Returned incorrect value for 0 hex", 0, Int64.Decode("0x0")
                    .GetInt64Value());
            assertTrue(
                    "Returned incorrect value for most negative value decimal",
                    Int64.Decode("-9223372036854775808").GetInt64Value() == unchecked((long)0x8000000000000000L));
            assertTrue(
                    "Returned incorrect value for most negative value hex",
                    Int64.Decode("-0x8000000000000000").GetInt64Value() == unchecked((long)0x8000000000000000L));
            assertTrue(
                    "Returned incorrect value for most positive value decimal",
                    Int64.Decode("9223372036854775807").GetInt64Value() == 0x7fffffffffffffffL);
            assertTrue(
                    "Returned incorrect value for most positive value hex",
                    Int64.Decode("0x7fffffffffffffff").GetInt64Value() == 0x7fffffffffffffffL);
            assertTrue("Failed for 07654321765432", Int64.Decode("07654321765432")
                    .GetInt64Value() == /*07654321765432l*/ 538536569626); // J2N: Octal literals not supported in C#, converted to decimal

            bool exception = false;
            try
            {
                Int64
                        .Decode("999999999999999999999999999999999999999999999999999999");
            }
            catch (FormatException e)
            {
                // Correct
                exception = true;
            }
            assertTrue("Failed to throw exception for value > ilong", exception);

            exception = false;
            try
            {
                Int64.Decode("9223372036854775808");
            }
            catch (FormatException e)
            {
                // Correct
                exception = true;
            }
            assertTrue("Failed to throw exception for MAX_VALUE + 1", exception);

            exception = false;
            try
            {
                Int64.Decode("-9223372036854775809");
            }
            catch (FormatException e)
            {
                // Correct
                exception = true;
            }
            assertTrue("Failed to throw exception for MIN_VALUE - 1", exception);

            exception = false;
            try
            {
                Int64.Decode("0x8000000000000000");
            }
            catch (FormatException e)
            {
                // Correct
                exception = true;
            }
            assertTrue("Failed to throw exception for hex MAX_VALUE + 1", exception);

            exception = false;
            try
            {
                Int64.Decode("-0x8000000000000001");
            }
            catch (FormatException e)
            {
                // Correct
                exception = true;
            }
            assertTrue("Failed to throw exception for hex MIN_VALUE - 1", exception);

            exception = false;
            try
            {
                Int64.Decode("42325917317067571199");
            }
            catch (FormatException e)
            {
                // Correct
                exception = true;
            }
            assertTrue("Failed to throw exception for 42325917317067571199",
                    exception);
        }

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

        /**
         * @tests java.lang.Long#parseLong(java.lang.String)
         */
        [Test]
        public void Test_parseLongLjava_lang_String2()
        {
            // Test for method long java.lang.Long.parseLong(java.lang.String)

            long l = Int64.ParseInt64("89000000005");
            assertEquals("Parsed to incorrect long value", 89000000005L, l);
            assertEquals("Returned incorrect value for 0", 0, Int64.ParseInt64("0"));
            assertTrue("Returned incorrect value for most negative value", Int64
                    .ParseInt64("-9223372036854775808") == unchecked((long)0x8000000000000000L));
            assertTrue("Returned incorrect value for most positive value", Int64
                    .ParseInt64("9223372036854775807") == 0x7fffffffffffffffL);

            bool exception = false;
            try
            {
                Int64.ParseInt64("9223372036854775808");
            }
            catch (FormatException e)
            {
                // Correct
                exception = true;
            }
            assertTrue("Failed to throw exception for MAX_VALUE + 1", exception);

            exception = false;
            try
            {
                Int64.ParseInt64("-9223372036854775809");
            }
            catch (FormatException e)
            {
                // Correct
                exception = true;
            }
            assertTrue("Failed to throw exception for MIN_VALUE - 1", exception);
        }

        /**
         * @tests java.lang.Long#parseLong(java.lang.String, int)
         */
        [Test]
        public void Test_parseLongLjava_lang_StringI()
        {
            // Test for method long java.lang.Long.parseLong(java.lang.String, int)
            assertEquals("Returned incorrect value",
                    100000000L, Int64.ParseInt64("100000000", 10));
            assertEquals("Returned incorrect value from hex string", 68719476735L, Int64.ParseInt64(
                    "FFFFFFFFF", 16));
            assertTrue("Returned incorrect value from octal string: "
                    + Int64.ParseInt64("77777777777"), Int64.ParseInt64("77777777777",
                    8) == 8589934591L);
            assertEquals("Returned incorrect value for 0 hex", 0, Int64
                    .ParseInt64("0", 16));
            assertTrue("Returned incorrect value for most negative value hex", Int64
                    .ParseInt64("-8000000000000000", 16) == unchecked((long)0x8000000000000000L));
            assertTrue("Returned incorrect value for most positive value hex", Int64
                    .ParseInt64("7fffffffffffffff", 16) == 0x7fffffffffffffffL);
            assertEquals("Returned incorrect value for 0 decimal", 0, Int64.ParseInt64(
                    "0", 10));
            assertTrue(
                    "Returned incorrect value for most negative value decimal",
                    Int64.ParseInt64("-9223372036854775808", 10) == unchecked((long)0x8000000000000000L));
            assertTrue(
                    "Returned incorrect value for most positive value decimal",
                    Int64.ParseInt64("9223372036854775807", 10) == 0x7fffffffffffffffL);

            bool exception = false;
            try
            {
                Int64.ParseInt64("999999999999", 8);
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
                Int64.ParseInt64("9223372036854775808", 10);
            }
            catch (FormatException e)
            {
                // Correct
                exception = true;
            }
            assertTrue("Failed to throw exception for MAX_VALUE + 1", exception);

            exception = false;
            try
            {
                Int64.ParseInt64("-9223372036854775809", 10);
            }
            catch (FormatException e)
            {
                // Correct
                exception = true;
            }
            assertTrue("Failed to throw exception for MIN_VALUE - 1", exception);

            exception = false;
            try
            {
                Int64.ParseInt64("8000000000000000", 16);
            }
            catch (FormatException e)
            {
                // Correct
                exception = true;
            }
            assertTrue("Failed to throw exception for hex MAX_VALUE + 1", exception);

            exception = false;
            try
            {
                Int64.ParseInt64("-8000000000000001", 16);
            }
            catch (FormatException e)
            {
                // Correct
                exception = true;
            }
            assertTrue("Failed to throw exception for hex MIN_VALUE + 1", exception);

            exception = false;
            try
            {
                Int64.ParseInt64("42325917317067571199", 10);
            }
            catch (FormatException e)
            {
                // Correct
                exception = true;
            }
            assertTrue("Failed to throw exception for 42325917317067571199",
                    exception);
        }

        /**
         * @tests java.lang.Long#toBinaryString(long)
         */
        [Test]
        public void Test_toBinaryStringJ()
        {
            // Test for method java.lang.String java.lang.Long.toBinaryString(long)
            assertEquals("Incorrect binary string returned", "11011001010010010000", Int64.ToBinaryString(
                    890000L));
            assertEquals("Incorrect binary string returned",

                                    "1000000000000000000000000000000000000000000000000000000000000000", Int64
                            .ToBinaryString(long.MinValue)
                            );
            assertEquals("Incorrect binary string returned",

                                    "111111111111111111111111111111111111111111111111111111111111111", Int64
                            .ToBinaryString(long.MaxValue)
                            );
        }

        /**
         * @tests java.lang.Long#toHexString(long)
         */
        [Test]
        public void Test_toHexStringJ()
        {
            // Test for method java.lang.String java.lang.Long.toHexString(long)
            assertEquals("Incorrect hex string returned", "54e0845", Int64.ToHexString(89000005L)
                    );
            assertEquals("Incorrect hex string returned", "8000000000000000", Int64.ToHexString(
                    long.MinValue));
            assertEquals("Incorrect hex string returned", "7fffffffffffffff", Int64.ToHexString(
                    long.MaxValue));
        }

        /**
         * @tests java.lang.Long#toOctalString(long)
         */
        [Test]
        public void Test_toOctalStringJ()
        {
            // Test for method java.lang.String java.lang.Long.toOctalString(long)
            assertEquals("Returned incorrect oct string", "77777777777", Int64.ToOctalString(
                    8589934591L));
            assertEquals("Returned incorrect oct string", "1000000000000000000000", Int64.ToOctalString(
                    long.MinValue));
            assertEquals("Returned incorrect oct string", "777777777777777777777", Int64.ToOctalString(
                    long.MaxValue));
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
            assertEquals("Returned incorrect value", 100000000L, Int64.ValueOf("100000000")
                    .GetInt64Value());
            assertTrue("Returned incorrect value", Int64.ValueOf(
                    "9223372036854775807").GetInt64Value() == long.MaxValue);
            assertTrue("Returned incorrect value", Int64.ValueOf(
                    "-9223372036854775808").GetInt64Value() == long.MinValue);

            bool exception = false;
            try
            {
                Int64
                        .ValueOf("999999999999999999999999999999999999999999999999999999999999");
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
                Int64.ValueOf("9223372036854775808");
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
                Int64.ValueOf("-9223372036854775809");
            }
            catch (FormatException e)
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
                    .GetInt64Value());
            assertEquals("Returned incorrect value from hex string", 68719476735L, Int64.ValueOf(
                    "FFFFFFFFF", 16).GetInt64Value());
            assertTrue("Returned incorrect value from octal string: "
                    + Int64.ValueOf("77777777777", 8).ToString(), Int64.ValueOf(
                    "77777777777", 8).GetInt64Value() == 8589934591L);
            assertTrue("Returned incorrect value", Int64.ValueOf(
                    "9223372036854775807", 10).GetInt64Value() == long.MaxValue);
            assertTrue("Returned incorrect value", Int64.ValueOf(
                    "-9223372036854775808", 10).GetInt64Value() == long.MinValue);
            assertTrue("Returned incorrect value", Int64.ValueOf("7fffffffffffffff",
                    16).GetInt64Value() == long.MaxValue);
            assertTrue("Returned incorrect value", Int64.ValueOf(
                    "-8000000000000000", 16).GetInt64Value() == long.MinValue);

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
                Int64.ValueOf("-9223372036854775809", 10);
            }
            catch (FormatException e)
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

        /**
         * @tests java.lang.Long#Int64(String)
         */
        [Test]
        public void Test_ConstructorLjava_lang_String()
        {
            assertEquals(new Int64(0), new Int64("0"));
            assertEquals(new Int64(1), new Int64("1"));
            assertEquals(new Int64(-1), new Int64("-1"));

            try
            {
                new Int64("0x1");
                fail("Expected NumberFormatException with hex string.");
            }
            catch (FormatException e) { }

            try
            {
                new Int64("9.2");
                fail("Expected NumberFormatException with floating point string.");
            }
            catch (FormatException e) { }

            try
            {
                new Int64("");
                fail("Expected NumberFormatException with empty string.");
            }
            catch (FormatException e) { }

            try
            {
                new Int64(null);
                fail("Expected NumberFormatException with null string.");
            }
            catch (FormatException e) { }
        }

        /**
         * @tests java.lang.Long#Long
         */
        [Test]
        public void Test_ConstructorJ()
        {
            assertEquals(1, new Int64(1).GetInt32Value());
            assertEquals(2, new Int64(2).GetInt32Value());
            assertEquals(0, new Int64(0).GetInt32Value());
            assertEquals(-1, new Int64(-1).GetInt32Value());
        }

        /**
         * @tests java.lang.Long#byteValue()
         */
        [Test]
        public void Test_booleanValue()
        {
            assertEquals(1, new Int64(1).GetByteValue());
            assertEquals(2, new Int64(2).GetByteValue());
            assertEquals(0, new Int64(0).GetByteValue());
            assertEquals(-1, (sbyte)new Int64(-1).GetByteValue()); // J2N: cast required to change the result to negative
        }

        /**
         * @tests java.lang.Long#equals(Object)
         */
        [Test]
        public void Test_equalsLjava_lang_Object()
        {
            assertEquals(new Int64(0), Int64.ValueOf(0));
            assertEquals(new Int64(1), Int64.ValueOf(1));
            assertEquals(new Int64(-1), Int64.ValueOf(-1));

            Int64 fixture = new Int64(25);
            assertEquals(fixture, fixture);
            assertFalse(fixture.Equals(null));
            assertFalse(fixture.Equals("Not a Long"));
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
            assertEquals(new Int64(0), Int64.ValueOf("0"));
            assertEquals(new Int64(1), Int64.ValueOf("1"));
            assertEquals(new Int64(-1), Int64.ValueOf("-1"));

            try
            {
                Int64.ValueOf("0x1");
                fail("Expected NumberFormatException with hex string.");
            }
            catch (FormatException e) { }

            try
            {
                Int64.ValueOf("9.2");
                fail("Expected NumberFormatException with floating point string.");
            }
            catch (FormatException e) { }

            try
            {
                Int64.ValueOf("");
                fail("Expected NumberFormatException with empty string.");
            }
            catch (FormatException e) { }

            try
            {
                Int64.ValueOf(null);
                fail("Expected NumberFormatException with null string.");
            }
            catch (FormatException e) { }
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
            assertEquals(Character.Digit('1', 2), Int64.ValueOf("1", 2).GetByteValue());
            assertEquals(Character.Digit('F', 16), Int64.ValueOf("F", 16).GetByteValue());

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
            catch (FormatException e) { }

            try
            {
                Int64.ValueOf(null, 10);
                fail("Expected NumberFormatException with null string.");
            }
            catch (FormatException e) { }
        }

        /**
         * @tests java.lang.Long#parseLong(String)
         */
        [Test]
        public void Test_parseLongLjava_lang_String()
        {
            assertEquals(0, Int64.ParseInt64("0"));
            assertEquals(1, Int64.ParseInt64("1"));
            assertEquals(-1, Int64.ParseInt64("-1"));

            try
            {
                Int64.ParseInt64("0x1");
                fail("Expected NumberFormatException with hex string.");
            }
            catch (FormatException e) { }

            try
            {
                Int64.ParseInt64("9.2");
                fail("Expected NumberFormatException with floating point string.");
            }
            catch (FormatException e) { }

            try
            {
                Int64.ParseInt64("");
                fail("Expected NumberFormatException with empty string.");
            }
            catch (FormatException e) { }

            try
            {
                Int64.ParseInt64(null);
                fail("Expected NumberFormatException with null string.");
            }
            catch (FormatException e) { }
        }

        /**
         * @tests java.lang.Long#parseLong(String,long)
         */
        [Test]
        public void Test_parseLongLjava_lang_StringJ()
        {
            assertEquals(0, Int64.ParseInt64("0", 10));
            assertEquals(1, Int64.ParseInt64("1", 10));
            assertEquals(-1, Int64.ParseInt64("-1", 10));

            //must be consistent with Character.digit()
            assertEquals(Character.Digit('1', 2), Int64.ParseInt64("1", 2));
            assertEquals(Character.Digit('F', 16), Int64.ParseInt64("F", 16));

            try
            {
                Int64.ParseInt64("0x1", 10);
                fail("Expected NumberFormatException with hex string.");
            }
            catch (FormatException e) { }

            try
            {
                Int64.ParseInt64("9.2", 10);
                fail("Expected NumberFormatException with floating point string.");
            }
            catch (FormatException e) { }

            try
            {
                Int64.ParseInt64("", 10);
                fail("Expected NumberFormatException with empty string.");
            }
            catch (FormatException e) { }

            try
            {
                Int64.ParseInt64(null, 10);
                fail("Expected NumberFormatException with null string.");
            }
            catch (FormatException e) { }
        }

        /**
         * @tests java.lang.Long#decode(String)
         */
        [Test]
        public void Test_decodeLjava_lang_String()
        {
            assertEquals(new Int64(0), Int64.Decode("0"));
            assertEquals(new Int64(1), Int64.Decode("1"));
            assertEquals(new Int64(-1), Int64.Decode("-1"));
            assertEquals(new Int64(0xF), Int64.Decode("0xF"));
            assertEquals(new Int64(0xF), Int64.Decode("#F"));
            assertEquals(new Int64(0xF), Int64.Decode("0XF"));
            assertEquals(new Int64(07), Int64.Decode("07"));

            try
            {
                Int64.Decode("9.2");
                fail("Expected NumberFormatException with floating point string.");
            }
            catch (FormatException e) { }

            try
            {
                Int64.Decode("");
                fail("Expected NumberFormatException with empty string.");
            }
            catch (FormatException e) { }

            try
            {
                Int64.Decode(null);
                //undocumented NPE, but seems consistent across JREs
                fail("Expected NullPointerException with null string.");
            }
            catch (ArgumentNullException e) { }
        }

        /**
         * @tests java.lang.Long#doubleValue()
         */
        [Test]
        public void Test_doubleValue()
        {
            assertEquals(-1D, new Int64(-1).GetDoubleValue(), 0D);
            assertEquals(0D, new Int64(0).GetDoubleValue(), 0D);
            assertEquals(1D, new Int64(1).GetDoubleValue(), 0D);
        }

        /**
         * @tests java.lang.Long#floatValue()
         */
        [Test]
        public void Test_floatValue()
        {
            assertEquals(-1F, new Int64(-1).GetSingleValue(), 0F);
            assertEquals(0F, new Int64(0).GetSingleValue(), 0F);
            assertEquals(1F, new Int64(1).GetSingleValue(), 0F);
        }

        /**
         * @tests java.lang.Long#intValue()
         */
        [Test]
        public void Test_intValue()
        {
            assertEquals(-1, new Int64(-1).GetInt32Value());
            assertEquals(0, new Int64(0).GetInt32Value());
            assertEquals(1, new Int64(1).GetInt32Value());
        }

        /**
         * @tests java.lang.Long#longValue()
         */
        [Test]
        public void Test_longValue()
        {
            assertEquals(-1L, new Int64(-1).GetInt64Value());
            assertEquals(0L, new Int64(0).GetInt64Value());
            assertEquals(1L, new Int64(1).GetInt64Value());
        }

        /**
         * @tests java.lang.Long#shortValue()
         */
        [Test]
        public void Test_shortValue()
        {
            assertEquals(-1, new Int64(-1).GetInt16Value());
            assertEquals(0, new Int64(0).GetInt16Value());
            assertEquals(1, new Int64(1).GetInt16Value());
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
            assertEquals(64, Int64.NumberOfLeadingZeros(0x0L));
            assertEquals(63, Int64.NumberOfLeadingZeros(0x1));
            assertEquals(62, Int64.NumberOfLeadingZeros(0x2));
            assertEquals(62, Int64.NumberOfLeadingZeros(0x3));
            assertEquals(61, Int64.NumberOfLeadingZeros(0x4));
            assertEquals(61, Int64.NumberOfLeadingZeros(0x5));
            assertEquals(61, Int64.NumberOfLeadingZeros(0x6));
            assertEquals(61, Int64.NumberOfLeadingZeros(0x7));
            assertEquals(60, Int64.NumberOfLeadingZeros(0x8));
            assertEquals(60, Int64.NumberOfLeadingZeros(0x9));
            assertEquals(60, Int64.NumberOfLeadingZeros(0xA));
            assertEquals(60, Int64.NumberOfLeadingZeros(0xB));
            assertEquals(60, Int64.NumberOfLeadingZeros(0xC));
            assertEquals(60, Int64.NumberOfLeadingZeros(0xD));
            assertEquals(60, Int64.NumberOfLeadingZeros(0xE));
            assertEquals(60, Int64.NumberOfLeadingZeros(0xF));
            assertEquals(59, Int64.NumberOfLeadingZeros(0x10));
            assertEquals(56, Int64.NumberOfLeadingZeros(0x80));
            assertEquals(56, Int64.NumberOfLeadingZeros(0xF0));
            assertEquals(55, Int64.NumberOfLeadingZeros(0x100));
            assertEquals(52, Int64.NumberOfLeadingZeros(0x800));
            assertEquals(52, Int64.NumberOfLeadingZeros(0xF00));
            assertEquals(51, Int64.NumberOfLeadingZeros(0x1000));
            assertEquals(48, Int64.NumberOfLeadingZeros(0x8000));
            assertEquals(48, Int64.NumberOfLeadingZeros(0xF000));
            assertEquals(47, Int64.NumberOfLeadingZeros(0x10000));
            assertEquals(44, Int64.NumberOfLeadingZeros(0x80000));
            assertEquals(44, Int64.NumberOfLeadingZeros(0xF0000));
            assertEquals(43, Int64.NumberOfLeadingZeros(0x100000));
            assertEquals(40, Int64.NumberOfLeadingZeros(0x800000));
            assertEquals(40, Int64.NumberOfLeadingZeros(0xF00000));
            assertEquals(39, Int64.NumberOfLeadingZeros(0x1000000));
            assertEquals(36, Int64.NumberOfLeadingZeros(0x8000000));
            assertEquals(36, Int64.NumberOfLeadingZeros(0xF000000));
            assertEquals(35, Int64.NumberOfLeadingZeros(0x10000000));
            assertEquals(32, Int64.NumberOfLeadingZeros(0x80000000)); // J2N: Changed test to match observed behavior in JDK (32 rather than 0)
            assertEquals(32, Int64.NumberOfLeadingZeros(0xF0000000)); // J2N: Changed test to match observed behavior in JDK (32 rather than 0)

            assertEquals(1, Int64.NumberOfLeadingZeros(long.MaxValue));
            assertEquals(0, Int64.NumberOfLeadingZeros(long.MinValue));
        }

        /**
         * @tests java.lang.Long#numberOfTrailingZeros(long)
         */
        [Test]
        public void Test_numberOfTrailingZerosJ()
        {
            assertEquals(64, Int64.NumberOfTrailingZeros(0x0));
            assertEquals(63, Int64.NumberOfTrailingZeros(long.MinValue));
            assertEquals(0, Int64.NumberOfTrailingZeros(long.MaxValue));

            assertEquals(0, Int64.NumberOfTrailingZeros(0x1));
            assertEquals(3, Int64.NumberOfTrailingZeros(0x8));
            assertEquals(0, Int64.NumberOfTrailingZeros(0xF));

            assertEquals(4, Int64.NumberOfTrailingZeros(0x10));
            assertEquals(7, Int64.NumberOfTrailingZeros(0x80));
            assertEquals(4, Int64.NumberOfTrailingZeros(0xF0));

            assertEquals(8, Int64.NumberOfTrailingZeros(0x100));
            assertEquals(11, Int64.NumberOfTrailingZeros(0x800));
            assertEquals(8, Int64.NumberOfTrailingZeros(0xF00));

            assertEquals(12, Int64.NumberOfTrailingZeros(0x1000));
            assertEquals(15, Int64.NumberOfTrailingZeros(0x8000));
            assertEquals(12, Int64.NumberOfTrailingZeros(0xF000));

            assertEquals(16, Int64.NumberOfTrailingZeros(0x10000));
            assertEquals(19, Int64.NumberOfTrailingZeros(0x80000));
            assertEquals(16, Int64.NumberOfTrailingZeros(0xF0000));

            assertEquals(20, Int64.NumberOfTrailingZeros(0x100000));
            assertEquals(23, Int64.NumberOfTrailingZeros(0x800000));
            assertEquals(20, Int64.NumberOfTrailingZeros(0xF00000));

            assertEquals(24, Int64.NumberOfTrailingZeros(0x1000000));
            assertEquals(27, Int64.NumberOfTrailingZeros(0x8000000));
            assertEquals(24, Int64.NumberOfTrailingZeros(0xF000000));

            assertEquals(28, Int64.NumberOfTrailingZeros(0x10000000));
            assertEquals(31, Int64.NumberOfTrailingZeros(0x80000000));
            assertEquals(28, Int64.NumberOfTrailingZeros(0xF0000000));
        }

        /**
         * @tests java.lang.Long#bitCount(long)
         */
        [Test]
        public void Test_bitCountJ()
        {
            assertEquals(0, Int64.BitCount(0x0));
            assertEquals(1, Int64.BitCount(0x1));
            assertEquals(1, Int64.BitCount(0x2));
            assertEquals(2, Int64.BitCount(0x3));
            assertEquals(1, Int64.BitCount(0x4));
            assertEquals(2, Int64.BitCount(0x5));
            assertEquals(2, Int64.BitCount(0x6));
            assertEquals(3, Int64.BitCount(0x7));
            assertEquals(1, Int64.BitCount(0x8));
            assertEquals(2, Int64.BitCount(0x9));
            assertEquals(2, Int64.BitCount(0xA));
            assertEquals(3, Int64.BitCount(0xB));
            assertEquals(2, Int64.BitCount(0xC));
            assertEquals(3, Int64.BitCount(0xD));
            assertEquals(3, Int64.BitCount(0xE));
            assertEquals(4, Int64.BitCount(0xF));

            assertEquals(8, Int64.BitCount(0xFF));
            assertEquals(12, Int64.BitCount(0xFFF));
            assertEquals(16, Int64.BitCount(0xFFFF));
            assertEquals(20, Int64.BitCount(0xFFFFF));
            assertEquals(24, Int64.BitCount(0xFFFFFF));
            assertEquals(28, Int64.BitCount(0xFFFFFFF));
            assertEquals(64, Int64.BitCount(unchecked((long)0xFFFFFFFFFFFFFFFFL)));
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
    }
}
