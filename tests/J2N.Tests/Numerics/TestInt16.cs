using J2N.Text;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace J2N.Numerics
{
    public class TestInt16 : TestCase
    {
        private Int16 sp = new Int16((short)18000);
        private Int16 sn = new Int16((short)-19000);

        /**
         * @tests java.lang.Short#byteValue()
         */
        [Test]
        public void Test_byteValue()
        {
            // Test for method byte java.lang.Short.byteValue()
            assertEquals("Returned incorrect byte value", 0, new Int16(short.MinValue)
                    .GetByteValue());
            assertEquals("Returned incorrect byte value", -1, (sbyte)new Int16(short.MaxValue)
                    .GetByteValue()); // J2N: cast required to change the result to negative
        }

        /**
         * @tests java.lang.Short#compareTo(java.lang.Short)
         */
        [Test]
        public void Test_compareToLjava_lang_Short()
        {
            // Test for method int java.lang.Short.compareTo(java.lang.Short)
            Int16 s = new Int16((short)1);
            Int16 x = new Int16((short)3);
            assertTrue(
                    "Should have returned negative value when compared to greater short",
                    s.CompareTo(x) < 0);
            x = new Int16((short)-1);
            assertTrue(
                    "Should have returned positive value when compared to lesser short",
                    s.CompareTo(x) > 0);
            x = new Int16((short)1);
            assertEquals("Should have returned zero when compared to equal short",
                                 0, s.CompareTo(x));

            //try
            //{
            //    new Int16((short)0).CompareTo(null);
            //    fail("No NPE");
            //}
            //catch (ArgumentNullException e)
            //{
            //}

            // J2N: Return 1 when comparing to null to match other .NET classes
            assertEquals(1, new Int16(0).CompareTo(null));
        }

        // J2N: Moved to CharSequences
        ///**
        // * @tests java.lang.Short#decode(java.lang.String)
        // */
        //[Test]
        //public void Test_decodeLjava_lang_String2()
        //{
        //    // Test for method java.lang.Short
        //    // java.lang.Short.decode(java.lang.String)
        //    assertTrue("Did not decode -1 correctly", Int16.Decode("-1")
        //            .GetInt16Value() == (short)-1);
        //    assertTrue("Did not decode -100 correctly", Int16.Decode("-100")
        //            .GetInt16Value() == (short)-100);
        //    assertTrue("Did not decode 23 correctly", Int16.Decode("23")
        //            .GetInt16Value() == (short)23);
        //    assertTrue("Did not decode 0x10 correctly", Int16.Decode("0x10")
        //            .GetInt16Value() == (short)16);
        //    assertTrue("Did not decode 32767 correctly", Int16.Decode("32767")
        //            .GetInt16Value() == (short)32767);
        //    assertTrue("Did not decode -32767 correctly", Int16.Decode("-32767")
        //            .GetInt16Value() == (short)-32767);
        //    assertTrue("Did not decode -32768 correctly", Int16.Decode("-32768")
        //            .GetInt16Value() == (short)-32768);

        //    bool exception = false;
        //    try
        //    {
        //        Int16.Decode("123s");
        //    }
        //    catch (FormatException e)
        //    {
        //        // correct
        //        exception = true;
        //    }
        //    assertTrue("Did not throw FormatException decoding 123s",
        //            exception);

        //    exception = false;
        //    try
        //    {
        //        Int16.Decode("32768");
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
        //        Int16.Decode("-32769");
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
        //        Int16.Decode("0x8000");
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
        //        Int16.Decode("-0x8001");
        //    }
        //    catch (OverflowException e) // J2N: .NET throws OverflowException rather than FormatException in this case
        //    {
        //        // Correct
        //        exception = true;
        //    }
        //    assertTrue("Failed to throw exception for hex MIN_VALUE - 1", exception);
        //}

        /**
         * @tests java.lang.Short#parseShort(java.lang.String)
         */
        [Test]
        public void Test_parseShortLjava_lang_String2()
        {
            // Test for method short java.lang.Short.parseShort(java.lang.String)
            short sp = Int16.Parse("32746", J2N.Text.StringFormatter.InvariantCulture);
            short sn = Int16.Parse("-32746", J2N.Text.StringFormatter.InvariantCulture);

            assertTrue("Incorrect parse of short", sp == (short)32746
                    && (sn == (short)-32746));
            assertEquals("Returned incorrect value for 0", 0, Int16.Parse("0", J2N.Text.StringFormatter.InvariantCulture));
            assertTrue("Returned incorrect value for most negative value", Int16
                    .Parse("-32768", J2N.Text.StringFormatter.InvariantCulture) == unchecked((short)0x8000));
            assertTrue("Returned incorrect value for most positive value", Int16
                    .Parse("32767", J2N.Text.StringFormatter.InvariantCulture) == 0x7fff);

            bool exception = false;
            try
            {
                Int16.Parse("32768", J2N.Text.StringFormatter.InvariantCulture);
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
                Int16.Parse("-32769", J2N.Text.StringFormatter.InvariantCulture);
            }
            catch (OverflowException e) // J2N: .NET throws OverflowException rather than FormatException in this case
            {
                // Correct
                exception = true;
            }
            assertTrue("Failed to throw exception for MIN_VALUE - 1", exception);
        }

        // J2N: Moved to CharSequences
        ///**
        // * @tests java.lang.Short#parseShort(java.lang.String, int)
        // */
        //[Test]
        //public void Test_parseShortLjava_lang_StringI2()
        //{
        //    // Test for method short java.lang.Short.parseShort(java.lang.String,
        //    // int)
        //    bool aThrow = true;
        //    assertEquals("Incorrectly parsed hex string",
        //            255, Int16.Parse("FF", 16));
        //    assertEquals("Incorrectly parsed oct string",
        //            16, Int16.Parse("20", 8));
        //    assertEquals("Incorrectly parsed dec string",
        //            20, Int16.Parse("20", 10));
        //    assertEquals("Incorrectly parsed bin string",
        //            4, Int16.Parse("100", 2));
        //    assertEquals("Incorrectly parsed -hex string", -255, Int16
        //            .Parse("-FF", 16));
        //    assertEquals("Incorrectly parsed -oct string",
        //            -16, Int16.Parse("-20", 8));
        //    assertEquals("Incorrectly parsed -bin string", -4, Int16
        //            .Parse("-100", 2));
        //    assertEquals("Returned incorrect value for 0 hex", 0, Int16.Parse("0",
        //            16));
        //    assertTrue("Returned incorrect value for most negative value hex",
        //            Int16.Parse("-8000", 16) == unchecked((short)0x8000));
        //    assertTrue("Returned incorrect value for most positive value hex",
        //            Int16.Parse("7fff", 16) == 0x7fff);
        //    assertEquals("Returned incorrect value for 0 decimal", 0, Int16.Parse(
        //            "0", 10));
        //    assertTrue("Returned incorrect value for most negative value decimal",
        //            Int16.Parse("-32768", 10) == unchecked((short)0x8000));
        //    assertTrue("Returned incorrect value for most positive value decimal",
        //            Int16.Parse("32767", 10) == 0x7fff);

        //    try
        //    {
        //        Int16.Parse("FF", 2);
        //    }
        //    catch (FormatException e)
        //    {
        //        // Correct
        //        aThrow = false;
        //    }
        //    if (aThrow)
        //    {
        //        fail(
        //                "Failed to throw exception when passed hex string and base 2 radix");
        //    }

        //    bool exception = false;
        //    try
        //    {
        //        Int16.Parse("10000000000", 10);
        //    }
        //    catch (OverflowException e) // J2N: .NET throws OverflowException rather than FormatException in this case
        //    {
        //        // Correct
        //        exception = true;
        //    }
        //    assertTrue(
        //            "Failed to throw exception when passed string larger than 16 bits",
        //            exception);

        //    exception = false;
        //    try
        //    {
        //        Int16.Parse("32768", 10);
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
        //        Int16.Parse("-32769", 10);
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
        //    //    Int16.Parse("8000", 16);
        //    //}
        //    //catch (OverflowException e) // J2N: .NET throws OverflowException rather than FormatException in this case
        //    //{
        //    //    // Correct
        //    //    exception = true;
        //    //}
        //    //assertTrue("Failed to throw exception for hex MAX_VALUE + 1", exception);

        //    assertEquals(1, Int16.Parse("1", 16));
        //    assertEquals(-1, Int16.Parse("ffff", 16));
        //    assertEquals(32767, Int16.Parse("7fff", 16));
        //    assertEquals(-32768, Int16.Parse("-8000", 16)); // Special case: In Java, we allow the negative sign for the smallest negative number
        //    assertEquals(-32768, Int16.Parse("8000", 16));  // In .NET, it should parse without the negative sign to the same value (in .NET the negative sign is not allowed)
        //    assertEquals(-32767, Int16.Parse("8001", 16));

        //    exception = false;
        //    try
        //    {
        //        Int16.Parse("-8001", 16);
        //    }
        //    catch (OverflowException e) // J2N: .NET throws OverflowException rather than FormatException in this case
        //    {
        //        // Correct
        //        exception = true;
        //    }
        //    assertTrue("Failed to throw exception for hex MIN_VALUE + 1", exception);
        //}

        /**
         * @tests java.lang.Short#toString()
         */
        [Test]
        public void Test_toString2()
        {
            // Test for method java.lang.String java.lang.Short.toString()
            assertTrue("Invalid string returned", sp.ToString().Equals("18000")
                    && (sn.ToString().Equals("-19000")));
            assertEquals("Returned incorrect string", "32767", new Int16((short)32767)
                    .ToString());
            assertEquals("Returned incorrect string", "-32767", new Int16((short)-32767)
                    .ToString());
            assertEquals("Returned incorrect string", "-32768", new Int16((short)-32768)
                    .ToString());
        }

        /**
         * @tests java.lang.Short#toString(short)
         */
        [Test]
        public void Test_toStringS2()
        {
            // Test for method java.lang.String java.lang.Short.toString(short)
            assertEquals("Returned incorrect string", "32767", Int16.ToString((short)32767)
                    );
            assertEquals("Returned incorrect string", "-32767", Int16.ToString((short)-32767)
                    );
            assertEquals("Returned incorrect string", "-32768", Int16.ToString((short)-32768)
                    );
        }

        /**
         * @tests java.lang.Short#valueOf(java.lang.String)
         */
        [Test]
        public void Test_valueOfLjava_lang_String2()
        {
            // Test for method java.lang.Short
            // java.lang.Short.valueOf(java.lang.String)
            assertEquals("Returned incorrect short", -32768, Int16.GetValueOf("-32768", J2N.Text.StringFormatter.InvariantCulture)
                    .GetInt16Value());
            assertEquals("Returned incorrect short", 32767, Int16.GetValueOf("32767", J2N.Text.StringFormatter.InvariantCulture)
                    .GetInt16Value());
        }

        /**
         * @tests java.lang.Short#valueOf(java.lang.String, int)
         */
        [Test]
        public void Test_valueOfLjava_lang_StringI2()
        {
            // Test for method java.lang.Short
            // java.lang.Short.valueOf(java.lang.String, int)
            bool aThrow = true;
            assertEquals("Incorrectly parsed hex string", 255, Int16.GetValueOf("FF", 16)
                    .GetInt16Value());
            assertEquals("Incorrectly parsed oct string", 16, Int16.GetValueOf("20", 8)
                    .GetInt16Value());
            assertEquals("Incorrectly parsed dec string", 20, Int16.GetValueOf("20", 10)
                    .GetInt16Value());
            assertEquals("Incorrectly parsed bin string", 4, Int16.GetValueOf("100", 2)
                    .GetInt16Value());
            assertEquals("Incorrectly parsed -hex string", -255, Int16.GetValueOf("-FF", 16)
                    .GetInt16Value());
            assertEquals("Incorrectly parsed -oct string", -16, Int16.GetValueOf("-20", 8)
                    .GetInt16Value());
            assertEquals("Incorrectly parsed -bin string", -4, Int16.GetValueOf("-100", 2)
                    .GetInt16Value());
            assertTrue("Did not decode 32767 correctly", Int16.GetValueOf("32767", 10)
                    .GetInt16Value() == (short)32767);
            assertTrue("Did not decode -32767 correctly", Int16.GetValueOf("-32767",
                    10).GetInt16Value() == (short)-32767);
            assertTrue("Did not decode -32768 correctly", Int16.GetValueOf("-32768",
                    10).GetInt16Value() == (short)-32768);
            try
            {
                Int16.GetValueOf("FF", 2);
            }
            catch (FormatException e)
            {
                // Correct
                aThrow = false;
            }
            if (aThrow)
            {
                fail(
                        "Failed to throw exception when passed hex string and base 2 radix");
            }
            try
            {
                Int16.GetValueOf("10000000000", 10);
            }
            catch (OverflowException e) // J2N: .NET throws OverflowException rather than FormatException in this case
            {
                // Correct
                return;
            }
            fail(
                    "Failed to throw exception when passed string larger than 16 bits");
        }
        /**
         * @tests java.lang.Short#valueOf(byte)
         */
        [Test]
        public void Test_valueOfS()
        {
            assertEquals(new Int16(short.MinValue), Int16.GetValueOf(short.MinValue));
            assertEquals(new Int16(short.MaxValue), Int16.GetValueOf(short.MaxValue));
            assertEquals(new Int16((short)0), Int16.GetValueOf((short)0));

            short s = -128;
            while (s < 128)
            {
                assertEquals(new Int16(s), Int16.GetValueOf(s));
                assertSame(Int16.GetValueOf(s), Int16.GetValueOf(s));
                s++;
            }
        }

        /**
         * @tests java.lang.Short#hashCode()
         */
        [Test]
        public void Test_hashCode()
        {
            assertEquals(1, new Int16((short)1).GetHashCode());
            assertEquals(2, new Int16((short)2).GetHashCode());
            assertEquals(0, new Int16((short)0).GetHashCode());
            assertEquals(-1, new Int16((short)-1).GetHashCode());
        }

        // J2N: Removed this overload because all of the constructors are deprecated in JDK 16
        ///**
        // * @tests java.lang.Short#Short(String)
        // */
        //[Test]
        //public void Test_ConstructorLjava_lang_String()
        //{
        //    assertEquals(new Int16((short)0), new Int16("0", J2N.Text.StringFormatter.InvariantCulture));
        //    assertEquals(new Int16((short)1), new Int16("1", J2N.Text.StringFormatter.InvariantCulture));
        //    assertEquals(new Int16((short)-1), new Int16("-1", J2N.Text.StringFormatter.InvariantCulture));

        //    try
        //    {
        //        new Int16("0x1", J2N.Text.StringFormatter.InvariantCulture);
        //        fail("Expected FormatException with hex string.");
        //    }
        //    catch (FormatException e) { }

        //    try
        //    {
        //        new Int16("9.2", J2N.Text.StringFormatter.InvariantCulture);
        //        fail("Expected FormatException with floating point string.");
        //    }
        //    catch (FormatException e) { }

        //    try
        //    {
        //        new Int16("", J2N.Text.StringFormatter.InvariantCulture);
        //        fail("Expected FormatException with empty string.");
        //    }
        //    catch (FormatException e) { }

        //    try
        //    {
        //        new Int16(null, J2N.Text.StringFormatter.InvariantCulture);
        //        fail("Expected FormatException with null string.");
        //    }
        //    catch (ArgumentNullException e) { } // J2N: .NET throws ArgumentNullException rather than FormatException in this case
        //}

        /**
         * @tests java.lang.Short#Short(short)
         */
        [Test]
        public void Test_ConstructorS()
        {
            assertEquals(1, new Int16((short)1).GetInt16Value());
            assertEquals(2, new Int16((short)2).GetInt16Value());
            assertEquals(0, new Int16((short)0).GetInt16Value());
            assertEquals(-1, new Int16((short)-1).GetInt16Value());
        }

        /**
         * @tests java.lang.Short#byteValue()
         */
        [Test]
        public void Test_booleanValue()
        {
            assertEquals(1, new Int16((short)1).GetByteValue());
            assertEquals(2, new Int16((short)2).GetByteValue());
            assertEquals(0, new Int16((short)0).GetByteValue());
            assertEquals(-1, (sbyte)new Int16((short)-1).GetByteValue()); // J2N: cast required to change the result to negative
        }

        /**
         * @tests java.lang.Short#equals(Object)
         */
        [Test]
        public void Test_equalsLjava_lang_Object()
        {
            assertEquals(new Int16((short)0), Int16.GetValueOf((short)0));
            assertEquals(new Int16((short)1), Int16.GetValueOf((short)1));
            assertEquals(new Int16((short)-1), Int16.GetValueOf((short)-1));

            Int16 fixture = new Int16((short)25);
            assertEquals(fixture, fixture);
            assertFalse(fixture.Equals(null));
            assertFalse(fixture.Equals("Not a Short"));
        }

        /**
         * @tests java.lang.Short#toString()
         */
        [Test]
        public void Test_toString()
        {
            assertEquals("-1", new Int16((short)-1).ToString());
            assertEquals("0", new Int16((short)0).ToString());
            assertEquals("1", new Int16((short)1).ToString());
            assertEquals("-1", new Int16(unchecked((short)0xFFFF)).ToString());
        }

        /**
         * @tests java.lang.Short#toString(short)
         */
        [Test]
        public void Test_toStringS()
        {
            assertEquals("-1", Int16.ToString((short)-1));
            assertEquals("0", Int16.ToString((short)0));
            assertEquals("1", Int16.ToString((short)1));
            assertEquals("-1", Int16.ToString(unchecked((short)0xFFFF)));
        }

        /**
         * @tests java.lang.Short#valueOf(String)
         */
        [Test]
        public void Test_valueOfLjava_lang_String()
        {
            assertEquals(new Int16((short)0), Int16.GetValueOf("0", J2N.Text.StringFormatter.InvariantCulture));
            assertEquals(new Int16((short)1), Int16.GetValueOf("1", J2N.Text.StringFormatter.InvariantCulture));
            assertEquals(new Int16((short)-1), Int16.GetValueOf("-1", J2N.Text.StringFormatter.InvariantCulture));

            try
            {
                Int16.GetValueOf("0x1", J2N.Text.StringFormatter.InvariantCulture);
                fail("Expected FormatException with hex string.");
            }
            catch (FormatException e) { }

            try
            {
                Int16.GetValueOf("9.2", J2N.Text.StringFormatter.InvariantCulture);
                fail("Expected FormatException with floating point string.");
            }
            catch (FormatException e) { }

            try
            {
                Int16.GetValueOf("", J2N.Text.StringFormatter.InvariantCulture);
                fail("Expected FormatException with empty string.");
            }
            catch (FormatException e) { }

            try
            {
                Int16.GetValueOf(null, J2N.Text.StringFormatter.InvariantCulture);
                fail("Expected FormatException with null string.");
            }
            catch (ArgumentNullException e) { } // J2N: .NET throws ArgumentNullException rather than FormatException in this case
        }

        /**
         * @tests java.lang.Short#valueOf(String,int)
         */
        [Test]
        public void Test_valueOfLjava_lang_StringI()
        {
            assertEquals(new Int16((short)0), Int16.GetValueOf("0", 10));
            assertEquals(new Int16((short)1), Int16.GetValueOf("1", 10));
            assertEquals(new Int16((short)-1), Int16.GetValueOf("-1", 10));

            //must be consistent with Character.digit()
            assertEquals(Character.Digit('1', 2), Int16.GetValueOf("1", 2).GetByteValue());
            assertEquals(Character.Digit('F', 16), Int16.GetValueOf("F", 16).GetByteValue());

            try
            {
                Int16.GetValueOf("0x1", 10);
                fail("Expected FormatException with hex string.");
            }
            catch (FormatException e) { }

            try
            {
                Int16.GetValueOf("9.2", 10);
                fail("Expected FormatException with floating point string.");
            }
            catch (FormatException e) { }

            try
            {
                Int16.GetValueOf("", 10);
                fail("Expected FormatException with empty string.");
            }
            catch (FormatException e) { }

            //try
            //{
            //    Int16.GetValueOf(null, 10);
            //    fail("Expected FormatException with null string.");
            //}
            //catch (FormatException e) { }

            // J2N: Match .NET behavior where null will result in 0
            assertEquals(0, Int16.GetValueOf(null, 10));
        }

        /**
         * @tests java.lang.Short#parseShort(String)
         */
        [Test]
        public void Test_parseShortLjava_lang_String()
        {
            assertEquals(0, Int16.Parse("0", J2N.Text.StringFormatter.InvariantCulture));
            assertEquals(1, Int16.Parse("1", J2N.Text.StringFormatter.InvariantCulture));
            assertEquals(-1, Int16.Parse("-1", J2N.Text.StringFormatter.InvariantCulture));

            try
            {
                Int16.Parse("0x1", J2N.Text.StringFormatter.InvariantCulture);
                fail("Expected FormatException with hex string.");
            }
            catch (FormatException e) { }

            try
            {
                Int16.Parse("9.2", J2N.Text.StringFormatter.InvariantCulture);
                fail("Expected FormatException with floating point string.");
            }
            catch (FormatException e) { }

            try
            {
                Int16.Parse("", J2N.Text.StringFormatter.InvariantCulture);
                fail("Expected FormatException with empty string.");
            }
            catch (FormatException e) { }

            try
            {
                Int16.Parse(null, J2N.Text.StringFormatter.InvariantCulture);
                fail("Expected FormatException with null string.");
            }
            catch (ArgumentNullException e) { } // J2N: .NET throws ArgumentNullException rather than FormatException in this case
        }

        // J2N: Moved to CharSequences
        ///**
        // * @tests java.lang.Short#parseShort(String,int)
        // */
        //[Test]
        //public void Test_parseShortLjava_lang_StringI()
        //{
        //    assertEquals(0, Int16.Parse("0", 10));
        //    assertEquals(1, Int16.Parse("1", 10));
        //    assertEquals(-1, Int16.Parse("-1", 10));

        //    //must be consistent with Character.digit()
        //    assertEquals(Character.Digit('1', 2), Int16.Parse("1", 2));
        //    assertEquals(Character.Digit('F', 16), Int16.Parse("F", 16));

        //    try
        //    {
        //        Int16.Parse("0x1", 10);
        //        fail("Expected FormatException with hex string.");
        //    }
        //    catch (FormatException e) { }

        //    try
        //    {
        //        Int16.Parse("9.2", 10);
        //        fail("Expected FormatException with floating point string.");
        //    }
        //    catch (FormatException e) { }

        //    try
        //    {
        //        Int16.Parse("", 10);
        //        fail("Expected FormatException with empty string.");
        //    }
        //    catch (FormatException e) { }

        //    //try
        //    //{
        //    //    Int16.Parse(null, 10);
        //    //    fail("Expected FormatException with null string.");
        //    //}
        //    //catch (FormatException e) { }

        //    // J2N: Match .NET behavior where null will result in 0
        //    assertEquals(0, Int16.Parse(null, 10));
        //}

        // J2N: Moved to CharSequences
        ///**
        // * @tests java.lang.Short#decode(String)
        // */
        //[Test]
        //public void Test_decodeLjava_lang_String()
        //{
        //    assertEquals(new Int16((short)0), Int16.Decode("0"));
        //    assertEquals(new Int16((short)1), Int16.Decode("1"));
        //    assertEquals(new Int16((short)-1), Int16.Decode("-1"));
        //    assertEquals(new Int16((short)0xF), Int16.Decode("0xF"));
        //    assertEquals(new Int16((short)0xF), Int16.Decode("#F"));
        //    assertEquals(new Int16((short)0xF), Int16.Decode("0XF"));
        //    assertEquals(new Int16((short)07), Int16.Decode("07"));

        //    try
        //    {
        //        Int16.Decode("9.2");
        //        fail("Expected FormatException with floating point string.");
        //    }
        //    catch (FormatException e) { }

        //    try
        //    {
        //        Int16.Decode("");
        //        fail("Expected FormatException with empty string.");
        //    }
        //    catch (FormatException e) { }

        //    try
        //    {
        //        Int16.Decode(null);
        //        //undocumented NPE, but seems consistent across JREs
        //        fail("Expected NullPointerException with null string.");
        //    }
        //    catch (ArgumentNullException e) { }
        //}

        /**
         * @tests java.lang.Short#doubleValue()
         */
        [Test]
        public void Test_doubleValue()
        {
            assertEquals(-1D, new Int16((short)-1).GetDoubleValue(), 0D);
            assertEquals(0D, new Int16((short)0).GetDoubleValue(), 0D);
            assertEquals(1D, new Int16((short)1).GetDoubleValue(), 0D);
        }

        /**
         * @tests java.lang.Short#floatValue()
         */
        [Test]
        public void Test_floatValue()
        {
            assertEquals(-1F, new Int16((short)-1).GetSingleValue(), 0F);
            assertEquals(0F, new Int16((short)0).GetSingleValue(), 0F);
            assertEquals(1F, new Int16((short)1).GetSingleValue(), 0F);
        }

        /**
         * @tests java.lang.Short#intValue()
         */
        [Test]
        public void Test_intValue()
        {
            assertEquals(-1, new Int16((short)-1).GetInt32Value());
            assertEquals(0, new Int16((short)0).GetInt32Value());
            assertEquals(1, new Int16((short)1).GetInt32Value());
        }

        /**
         * @tests java.lang.Short#longValue()
         */
        [Test]
        public void Test_longValue()
        {
            assertEquals(-1L, new Int16((short)-1).GetInt64Value());
            assertEquals(0L, new Int16((short)0).GetInt64Value());
            assertEquals(1L, new Int16((short)1).GetInt64Value());
        }

        /**
         * @tests java.lang.Short#shortValue()
         */
        [Test]
        public void Test_shortValue()
        {
            assertEquals(-1, new Int16((short)-1).GetInt16Value());
            assertEquals(0, new Int16((short)0).GetInt16Value());
            assertEquals(1, new Int16((short)1).GetInt16Value());
        }

        /**
         * @tests java.lang.Short#reverseBytes(short)
         */
        [Test]
        public void Test_reverseBytesS()
        {
            assertEquals(unchecked((short)0xABCD), Int16.ReverseBytes(unchecked((short)0xCDAB)));
            assertEquals((short)0x1234, Int16.ReverseBytes((short)0x3412));
            assertEquals((short)0x0011, Int16.ReverseBytes((short)0x1100));
            assertEquals((short)0x2002, Int16.ReverseBytes((short)0x0220));
        }

        public class CharSequences : TestCase
        {
            #region ParseTestCase

            public abstract class ParseTestCase
            {
                public static IEnumerable<TestCaseData> TestParse_CharSequence_Int32_Data
                {
                    get
                    {
                        // JDK 8

                        yield return new TestCaseData((short)+100, "+100", 10);
                        yield return new TestCaseData((short)-100, "-100", 10);

                        yield return new TestCaseData((short)0, "+0", 10);
                        yield return new TestCaseData((short)0, "-0", 10);
                        yield return new TestCaseData((short)0, "+00000", 10);
                        yield return new TestCaseData((short)0, "-00000", 10);

                        yield return new TestCaseData((short)0, "0", 10);
                        yield return new TestCaseData((short)1, "1", 10);
                        yield return new TestCaseData((short)9, "9", 10);

                        // Harmony (Test_parseShortLjava_lang_String())

                        yield return new TestCaseData((short)0, "0", 10);
                        yield return new TestCaseData((short)1, "1", 10);
                        yield return new TestCaseData((short)-1, "-1", 10);
                        yield return new TestCaseData((short)0, null, 10); // J2N: .NET returns 0 in this case

                        // Harmony (Test_parseShortLjava_lang_String2())

                        yield return new TestCaseData((short)32746, "32746", 10);
                        yield return new TestCaseData((short)-32746, "-32746", 10);
                        yield return new TestCaseData((short)0, "0", 10);
                        yield return new TestCaseData(unchecked((short)0x8000), "-32768", 10);
                        yield return new TestCaseData((short)0x7fff, "32767", 10);

                        // Harmony (Test_parseShortLjava_lang_StringI())

                        yield return new TestCaseData((short)0, "0", 10);
                        yield return new TestCaseData((short)1, "1", 10);
                        yield return new TestCaseData((short)-1, "-1", 10);
                        yield return new TestCaseData((short)0, null, 10); // J2N: Match .NET behavior where null will result in 0

                        //must be consistent with Character.digit()
                        yield return new TestCaseData((short)Character.Digit('1', 2), "1", 2);
                        yield return new TestCaseData((short)Character.Digit('F', 16), "F", 16);

                        // Harmony (Test_parseShortLjava_lang_StringI2())

                        yield return new TestCaseData((short)255, "FF", 16);
                        yield return new TestCaseData((short)16, "20", 8);
                        yield return new TestCaseData((short)20, "20", 10);
                        yield return new TestCaseData((short)4, "100", 2);
                        yield return new TestCaseData((short)-255, "-FF", 16);
                        yield return new TestCaseData((short)-16, "-20", 8);
                        yield return new TestCaseData((short)-4, "-100", 2);
                        yield return new TestCaseData((short)0, "0", 16);
                        yield return new TestCaseData(unchecked((short)0x8000), "-8000", 16);
                        yield return new TestCaseData((short)0x7fff, "7fff", 16);
                        yield return new TestCaseData((short)0, "0", 10);
                        yield return new TestCaseData(unchecked((short)0x8000), "-32768", 10);
                        yield return new TestCaseData((short)0x7fff, "32767", 10);

                        // .NET 5

                        string[] testValues = { null, null, null, null, "7fff", "32767", "77777", "111111111111111", "8000", "-32768", "100000", "1000000000000000" };
                        int[] testBases = { 10, 2, 8, 16, 16, 10, 8, 2, 16, 10, 8, 2 };
                        short[] expectedValues = { 0, 0, 0, 0, short.MaxValue, short.MaxValue, short.MaxValue, short.MaxValue, short.MinValue, short.MinValue, short.MinValue, short.MinValue };
                        for (int i = 0; i < testValues.Length; i++)
                        {
                            yield return new TestCaseData(expectedValues[i], testValues[i], testBases[i]);
                        }

                        // Custom

                        yield return new TestCaseData((short)1, "1", 16);
                        yield return new TestCaseData((short)-1, "ffff", 16);
                        yield return new TestCaseData((short)32767, "7fff", 16);
                        yield return new TestCaseData((short)-32768, "-8000", 16); // Special case: In Java, we allow the negative sign for the smallest negative number
                        yield return new TestCaseData((short)-32768, "8000", 16);
                        yield return new TestCaseData((short)-32767, "8001", 16);

                        // Surrogate Pairs (.NET only supports ASCII, but Java supports these)

                        yield return new TestCaseData((short)999, "𝟗𑃹𝟫", 10);
                        yield return new TestCaseData((short)5783, "𝟓𝟕𝟖𝟑", 10);
                        yield return new TestCaseData((short)479, "𑁪𑁭𑁯", 10);

                        // Non-decimal needs to be tested separately because they go through
                        // a separate execution path
                        yield return new TestCaseData((short)2457, "𝟗𑃹𝟫", 16);
                        yield return new TestCaseData((short)22403, "𝟓𝟕𝟖𝟑", 16);
                        yield return new TestCaseData((short)1145, "𑁪𑁭𑁯", 16);
                    }
                }

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

                        // Harmony (Test_parseShortLjava_lang_String())

                        yield return new TestCaseData(typeof(FormatException), "0x1", 10);
                        yield return new TestCaseData(typeof(FormatException), "9.2", 10);
                        yield return new TestCaseData(typeof(FormatException), "", 10);
                        //yield return new TestCaseData(typeof(ArgumentNullException), null, 10); // J2N: .NET returns 0 in this case

                        // Harmony (Test_parseShortLjava_lang_String2())

                        yield return new TestCaseData(typeof(OverflowException), "32768", 10);
                        yield return new TestCaseData(typeof(OverflowException), "-32769", 10);

                        // Harmony (Test_parseShortLjava_lang_StringI())

                        yield return new TestCaseData(typeof(FormatException), "0x1", 10);
                        yield return new TestCaseData(typeof(FormatException), "9.2", 10);
                        yield return new TestCaseData(typeof(FormatException), "", 10); // J2N: .NET throws ArgumentOutOfRangeException rather than FormatException in this case, but since it is inconsistent with short.Parse() we are going with FormatException.
                        //yield return new TestCaseData(typeof(FormatException), null, 10); // J2N: Match .NET behavior where null will result in 0

                        // Harmony (Test_parseShortLjava_lang_StringI2())

                        yield return new TestCaseData(typeof(FormatException), "FF", 2);
                        yield return new TestCaseData(typeof(OverflowException), "10000000000", 10);
                        yield return new TestCaseData(typeof(OverflowException), "32768", 10);
                        yield return new TestCaseData(typeof(OverflowException), "-32769", 10);
                        yield return new TestCaseData(typeof(OverflowException), "-8001", 16);

                        // .NET 5

                        string[] overflowValues = { "32768", "-32769", "11111111111111111", "1FFFF", "777777" };
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

                public static IEnumerable<TestCaseData> TestParse_CharSequence_Int32_Int32_Int32_Data
                {
                    get
                    {
                        // JDK 8

                        yield return new TestCaseData((short)0, "test-00000", 4, 10 - 4, 10);
                        yield return new TestCaseData((short)-12345, "test-12345", 4, 10 - 4, 10);
                        yield return new TestCaseData((short)12345, "xx12345yy", 2, 7 - 2, 10);
                        yield return new TestCaseData((short)15, "xxFyy", 2, 3 - 2, 16);

                        yield return new TestCaseData((short)12345, "xx1234567yy", 2, 5, 10);


                        // Harmony (Test_parseShortLjava_lang_String())

                        yield return new TestCaseData((short)0, "0", 0, 1, 10);
                        yield return new TestCaseData((short)1, "1", 0, 1, 10);
                        yield return new TestCaseData((short)-1, "-1", 0, 2, 10);
                        //yield return new TestCaseData((short)0, null, 0, 1, 10); // Unlike non-indexed overload, throw instead of returning zero

                        // Harmony (Test_parseShortLjava_lang_String2())

                        yield return new TestCaseData((short)32746, "32746", 0, 5, 10);
                        yield return new TestCaseData((short)-32746, "-32746", 0, 6, 10);
                        yield return new TestCaseData((short)0, "0", 0, 1, 10);
                        yield return new TestCaseData(unchecked((short)0x8000), "-32768", 0, 6, 10);
                        yield return new TestCaseData((short)0x7fff, "32767", 0, 5, 10);

                        // Harmony (Test_parseShortLjava_lang_StringI())

                        yield return new TestCaseData((short)0, "0", 0, 1, 10);
                        yield return new TestCaseData((short)1, "1", 0, 1, 10);
                        yield return new TestCaseData((short)-1, "-1", 0, 2, 10);
                        //yield return new TestCaseData((short)0, null, 0, 1, 10); // Unlike non-indexed overload, throw instead of returning zero

                        //must be consistent with Character.digit()
                        yield return new TestCaseData((short)Character.Digit('1', 2), "1", 0, 1, 2);
                        yield return new TestCaseData((short)Character.Digit('F', 16), "F", 0, 1, 16);

                        // Harmony (Test_parseShortLjava_lang_StringI2())

                        yield return new TestCaseData((short)255, "FF", 0, 2, 16);
                        yield return new TestCaseData((short)16, "20", 0, 2, 8);
                        yield return new TestCaseData((short)20, "20", 0, 2, 10);
                        yield return new TestCaseData((short)4, "100", 0, 3, 2);
                        yield return new TestCaseData((short)-255, "-FF", 0, 3, 16);
                        yield return new TestCaseData((short)-16, "-20", 0, 3, 8);
                        yield return new TestCaseData((short)-4, "-100", 0, 4, 2);
                        yield return new TestCaseData((short)0, "0", 0, 1, 16);
                        yield return new TestCaseData(unchecked((short)0x8000), "-8000", 0, 5, 16);
                        yield return new TestCaseData((short)0x7fff, "7fff", 0, 4, 16);
                        yield return new TestCaseData((short)0, "0", 0, 1, 10);
                        yield return new TestCaseData(unchecked((short)0x8000), "-32768", 0, 6, 10);
                        yield return new TestCaseData((short)0x7fff, "32767", 0, 5, 10);

                        // .NET 5

                        string[] testValues = { /*null, null, null, null,*/ "7fff", "32767", "77777", "111111111111111", "8000", "-32768", "100000", "1000000000000000" };
                        int[] testBases = { /*10, 2, 8, 16,*/ 16, 10, 8, 2, 16, 10, 8, 2 };
                        short[] expectedValues = { /*0, 0, 0, 0,*/ short.MaxValue, short.MaxValue, short.MaxValue, short.MaxValue, short.MinValue, short.MinValue, short.MinValue, short.MinValue };
                        for (int i = 0; i < testValues.Length; i++)
                        {
                            yield return new TestCaseData(expectedValues[i], testValues[i], 0, testValues[i].Length, testBases[i]);
                        }

                        // Custom

                        yield return new TestCaseData((short)1, "1", 0, 1, 16);
                        yield return new TestCaseData((short)-1, "ffff", 0, 4, 16);
                        yield return new TestCaseData((short)32767, "7fff", 0, 4, 16);
                        yield return new TestCaseData((short)-32768, "-8000", 0, 5, 16); // Special case: In Java, we allow the negative sign for the smallest negative number
                        yield return new TestCaseData((short)-32768, "8000", 0, 4, 16);
                        yield return new TestCaseData((short)-32767, "8001", 0, 4, 16);

                        // Surrogate Pairs (.NET only supports ASCII, but Java supports these)

                        yield return new TestCaseData((short)999, "𝟗𑃹𝟫", 0, 6, 10);
                        yield return new TestCaseData((short)5783, "𝟓𝟕𝟖𝟑", 0, 8, 10);
                        yield return new TestCaseData((short)479, "𑁪𑁭𑁯", 0, 6, 10);

                        // Non-decimal needs to be tested separately because they go through
                        // a separate execution path
                        yield return new TestCaseData((short)2457, "𝟗𑃹𝟫", 0, 6, 16);
                        yield return new TestCaseData((short)22403, "𝟓𝟕𝟖𝟑", 0, 8, 16);
                        yield return new TestCaseData((short)1145, "𑁪𑁭𑁯", 0, 6, 16);
                    }
                }

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

                        // Harmony (Test_parseShortLjava_lang_String())

                        yield return new TestCaseData(typeof(FormatException), "0x1", 0, 3, 10);
                        yield return new TestCaseData(typeof(FormatException), "9.2", 0, 3, 10);
                        yield return new TestCaseData(typeof(FormatException), "", 0, 0, 10);
                        yield return new TestCaseData(typeof(ArgumentNullException), null, 0, 1, 10); // Unlike non-indexed overload, throw instead of returning zero

                        // Harmony (Test_parseShortLjava_lang_String2())

                        yield return new TestCaseData(typeof(OverflowException), "32768", 0, 5, 10);
                        yield return new TestCaseData(typeof(OverflowException), "-32769", 0, 6, 10);

                        // Harmony (Test_parseShortLjava_lang_StringI())

                        yield return new TestCaseData(typeof(FormatException), "0x1", 0, 3, 10);
                        yield return new TestCaseData(typeof(FormatException), "9.2", 0, 3, 10);
                        yield return new TestCaseData(typeof(FormatException), "", 0, 0, 10); // J2N: .NET throws ArgumentOutOfRangeException rather than FormatException in this case, but since it is inconsistent with short.Parse() we are going with FormatException.
                        yield return new TestCaseData(typeof(ArgumentNullException), null, 0, 1, 10); // Unlike non-indexed overload, throw instead of returning zero

                        // Harmony (Test_parseShortLjava_lang_StringI2())

                        yield return new TestCaseData(typeof(FormatException), "FF", 0, 2, 2);
                        yield return new TestCaseData(typeof(OverflowException), "10000000000", 0, 11, 10);
                        yield return new TestCaseData(typeof(OverflowException), "32768", 0, 5, 10);
                        yield return new TestCaseData(typeof(OverflowException), "-32769", 0, 6, 10);
                        yield return new TestCaseData(typeof(OverflowException), "-8001", 0, 5, 16);

                        // .NET 5

                        string[] overflowValues = { "32768", "-32769", "11111111111111111", "1FFFF", "777777" };
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
            }

            #endregion ParseTestCase

            #region Parse_CharSequence_Int32

            public abstract class Parse_CharSequence_Int32_TestCase : ParseTestCase
            {
                protected abstract short GetResult(string value, int radix);

                [TestCaseSource("TestParse_CharSequence_Int32_Data")]
                public virtual void TestParse_CharSequence_Int32(short expected, string value, int radix)
                {
                    var actual = GetResult(value, radix);
                    assertEquals($"Int16.Parse(string, IFormatProvider) failed. String: \"{value}\" Result: {actual}", expected, actual);
                }

                [TestCaseSource("TestParse_CharSequence_Int32_ForException_Data")]
                public virtual void TestParse_CharSequence_Int32_ForException(Type expectedExceptionType, string value, int radix)
                {
                    Assert.Throws(expectedExceptionType, () => GetResult(value, radix));
                }
            }

            public class Parse_String_Int32 : Parse_CharSequence_Int32_TestCase
            {
                protected override short GetResult(string value, int radix)
                {
                    return Int16.Parse(value, radix);
                }
            }

            // J2N: ReadOnlySpan<char> not supported at this time on this overload (not supported in .NET anyway)
            //#if FEATURE_READONLYSPAN
            //            public class Parse_ReadOnlySpan_Int32 : Parse_CharSequence_Int32_TestCase
            //            {
            //                protected override short GetResult(string s, int radix)
            //                {
            //                    return Int16.Parse(s.AsSpan(), radix);
            //                }
            //            }
            //#endif

            #endregion Parse_CharSequence_Int32

            #region Parse_CharSequence_Int32_Int32_Int32

            public abstract class Parse_CharSequence_Int32_Int32_Int32_TestCase : ParseTestCase
            {
                protected virtual bool IsNullableType => true;

                protected abstract short GetResult(string value, int startIndex, int length, int radix);


                [TestCaseSource("TestParse_CharSequence_Int32_Int32_Int32_Data")]

                public virtual void TestParse_CharSequence_Int32_Int32_Int32(short expected, string value, int startIndex, int length, int radix)
                {
                    Assume.That(IsNullableType || (!IsNullableType && value != null), "null is not supported by this character sequence type.");

                    var actual = GetResult(value, startIndex, length, radix);
                    assertEquals($"Int16.Parse(string, int, int, int) failed. Expected: {expected} String: \"{value}\", startIndex: {startIndex}, length: {length} radix: {radix} Result: {actual}", expected, actual);
                }

                [TestCaseSource("TestParse_CharSequence_Int32_Int32_Int32_ForException_Data")]
                public virtual void TestParse_CharSequence_Int32_Int32_Int32_ForException(Type expectedExceptionType, string value, int startIndex, int length, int radix)
                {
                    Assume.That(IsNullableType || (!IsNullableType && value != null), "null is not supported by this character sequence type.");

                    Assert.Throws(expectedExceptionType, () => GetResult(value, startIndex, length, radix));
                }
            }

            public class Parse_String_Int32_Int32_Int32_Int32 : Parse_CharSequence_Int32_Int32_Int32_TestCase
            {
                protected override short GetResult(string value, int startIndex, int length, int radix)
                {
                    return Int16.Parse(value, startIndex, length, radix);
                }
            }

            public class Parse_CharArray_Int32_Int32_Int32_Int32 : Parse_CharSequence_Int32_Int32_Int32_TestCase
            {
                protected override short GetResult(string value, int startIndex, int length, int radix)
                {
                    return Int16.Parse(value is null ? null : value.ToCharArray(), startIndex, length, radix);
                }
            }

            public class Parse_StringBuilder_Int32_Int32_Int32_Int32 : Parse_CharSequence_Int32_Int32_Int32_TestCase
            {
                protected override short GetResult(string value, int startIndex, int length, int radix)
                {
                    // NOTE: Although technically the .NET StringBuilder will accept null and convert it to an empty string, we
                    // want to test the method for null here.
                    return Int16.Parse(value is null ? null : new StringBuilder(value), startIndex, length, radix);
                }
            }

            public class Parse_ICharSequence_Int32_Int32_Int32_Int32 : Parse_CharSequence_Int32_Int32_Int32_TestCase
            {
                protected override short GetResult(string value, int startIndex, int length, int radix)
                {
                    return Int16.Parse(value.AsCharSequence(), startIndex, length, radix);
                }
            }

#if FEATURE_READONLYSPAN
            public class Parse_ReadOnlySpan_Int32_Int32_Int32_Int32 : Parse_CharSequence_Int32_Int32_Int32_TestCase
            {
                protected override bool IsNullableType => false;

                protected override short GetResult(string value, int startIndex, int length, int radix)
                {
                    return Int16.Parse(value.AsSpan(), startIndex, length, radix);
                }
            }
#endif

            #endregion Parse_CharSequence_Int32_Int32_Int32

            #region TryParse_CharSequence_Int32

            public abstract class TryParse_CharSequence_Int32_TestCase : ParseTestCase
            {
                protected abstract bool GetResult(string value, int radix, out short result);

                [TestCaseSource("TestParse_CharSequence_Int32_Data")]
                public virtual void TestParse_CharSequence_Int32(short expected, string value, int radix)
                {
                    assertTrue(GetResult(value, radix, out short actual));
                    assertEquals($"Int16.TryParse(string, out short) failed. String: \"{value}\" Result: {actual}", expected, actual);
                }

                [TestCaseSource("TestParse_CharSequence_Int32_ForException_Data")]
                public virtual void TestParse_CharSequence_Int32_ForException(Type expectedExceptionType, string value, int radix)
                {
                    assertFalse(GetResult(value, radix, out short actual));
                    assertEquals(0, actual);
                }
            }

            public class TryParse_String_Int32 : TryParse_CharSequence_Int32_TestCase
            {
                protected override bool GetResult(string value, int radix, out short result)
                {
                    return Int16.TryParse(value, radix, out result);
                }
            }

            // J2N: ReadOnlySpan<char> not supported at this time on this overload (not supported in .NET anyway)
            //#if FEATURE_READONLYSPAN
            //            public class TryParse_ReadOnlySpan_Int32 : TryParse_CharSequence_Int32_TestCase
            //            {
            //                protected override bool GetResult(string s, int radix, out short result)
            //                {
            //                    return Int16.TryParse(s.AsSpan(), radix, out result);
            //                }
            //            }
            //#endif

            #endregion Parse_CharSequence_Int32

            #region TryParse_CharSequence_Int32_Int32_Int32

            public abstract class TryParse_CharSequence_Int32_Int32_Int32_TestCase : ParseTestCase
            {
                protected virtual bool IsNullableType => true;

                protected abstract bool GetResult(string value, int startIndex, int length, int radix, out short result);


                [TestCaseSource("TestParse_CharSequence_Int32_Int32_Int32_Data")]

                public virtual void TestParse_CharSequence_Int32_Int32_Int32(short expected, string value, int startIndex, int length, int radix)
                {
                    Assume.That(IsNullableType || (!IsNullableType && value != null), "null is not supported by this character sequence type.");

                    assertTrue(GetResult(value, startIndex, length, radix, out short actual));
                    assertEquals($"Int16.TryParse(string, int, int, int, out short) failed. Expected: {expected} String: \"{value}\", startIndex: {startIndex}, length: {length} radix: {radix} Result: {actual}", expected, actual);
                }

                [TestCaseSource("TestParse_CharSequence_Int32_Int32_Int32_ForException_Data")]
                public virtual void TestParse_CharSequence_Int32_Int32_Int32_ForException(Type expectedExceptionType, string value, int startIndex, int length, int radix)
                {
                    Assume.That(IsNullableType || (!IsNullableType && value != null), "null is not supported by this character sequence type.");

                    assertFalse(GetResult(value, startIndex, length, radix, out short actual));
                    assertEquals(0, actual);
                }
            }

            public class TryParse_String_Int32_Int32_Int32_Int32 : TryParse_CharSequence_Int32_Int32_Int32_TestCase
            {
                protected override bool GetResult(string value, int startIndex, int length, int radix, out short result)
                {
                    return Int16.TryParse(value, startIndex, length, radix, out result);
                }
            }

            public class TryParse_CharArray_Int32_Int32_Int32_Int32 : TryParse_CharSequence_Int32_Int32_Int32_TestCase
            {
                protected override bool GetResult(string value, int startIndex, int length, int radix, out short result)
                {
                    return Int16.TryParse(value is null ? null : value.ToCharArray(), startIndex, length, radix, out result);
                }
            }

            public class TryParse_StringBuilder_Int32_Int32_Int32_Int32 : TryParse_CharSequence_Int32_Int32_Int32_TestCase
            {
                protected override bool GetResult(string value, int startIndex, int length, int radix, out short result)
                {
                    // NOTE: Although technically the .NET StringBuilder will accept null and convert it to an empty string, we
                    // want to test the method for null here.
                    return Int16.TryParse(value is null ? null : new StringBuilder(value), startIndex, length, radix, out result);
                }
            }

            public class TryParse_ICharSequence_Int32_Int32_Int32_Int32 : TryParse_CharSequence_Int32_Int32_Int32_TestCase
            {
                protected override bool GetResult(string value, int startIndex, int length, int radix, out short result)
                {
                    return Int16.TryParse(value.AsCharSequence(), startIndex, length, radix, out result);
                }
            }

#if FEATURE_READONLYSPAN
            public class TryParse_ReadOnlySpan_Int32_Int32_Int32_Int32 : TryParse_CharSequence_Int32_Int32_Int32_TestCase
            {
                protected override bool IsNullableType => false;

                protected override bool GetResult(string value, int startIndex, int length, int radix, out short result)
                {
                    return Int16.TryParse(value.AsSpan(), startIndex, length, radix, out result);
                }
            }
#endif

            #endregion TryParse_CharSequence_Int32_Int32_Int32


            #region DecodeTestCase

            public abstract class DecodeTestCase
            {
                public static IEnumerable<TestCaseData> TestDecode_CharSequence_Data
                {
                    get
                    {
                        // JDK 8

                        yield return new TestCaseData("" + short.MinValue, short.MinValue);
                        yield return new TestCaseData("" + short.MaxValue, short.MaxValue);

                        yield return new TestCaseData("10", (short)10);
                        yield return new TestCaseData("0x10", (short)16);
                        yield return new TestCaseData("0X10", (short)16);
                        yield return new TestCaseData("010", (short)8);
                        yield return new TestCaseData("#10", (short)16);

                        yield return new TestCaseData("+10", (short)10);
                        yield return new TestCaseData("+0x10", (short)16);
                        yield return new TestCaseData("+0X10", (short)16);
                        yield return new TestCaseData("+010", (short)8);
                        yield return new TestCaseData("+#10", (short)16);

                        yield return new TestCaseData("-10", (short)-10);
                        yield return new TestCaseData("-0x10", (short)-16);
                        yield return new TestCaseData("-0X10", (short)-16);
                        yield return new TestCaseData("-010", (short)-8);
                        yield return new TestCaseData("-#10", (short)-16);

                        yield return new TestCaseData(Convert.ToString((int)short.MinValue, 10), short.MinValue);
                        yield return new TestCaseData(Convert.ToString((int)short.MaxValue, 10), short.MaxValue);

                        // Harmony (Test_decodeLjava_lang_String())

                        yield return new TestCaseData("0", (short)0);
                        yield return new TestCaseData("1", (short)1);
                        yield return new TestCaseData("-1", (short)-1);
                        yield return new TestCaseData("0xF", (short)0xF);
                        yield return new TestCaseData("#F", (short)0xF);
                        yield return new TestCaseData("0XF", (short)0xF);
                        yield return new TestCaseData("07", (short)07);

                        // Harmony (Test_decodeLjava_lang_String2())

                        yield return new TestCaseData("-1", (short)-1);
                        yield return new TestCaseData("-100", (short)-100);
                        yield return new TestCaseData("23", (short)23);
                        yield return new TestCaseData("0x10", (short)16);
                        yield return new TestCaseData("32767", (short)32767);
                        yield return new TestCaseData("-32767", (short)-32767);
                        yield return new TestCaseData("-32768", (short)-32768);
                    }
                }

                public static IEnumerable<TestCaseData> TestDecode_CharSequence_ForException_Data
                {
                    get
                    {
                        // JDK 8

                        yield return new TestCaseData(typeof(FormatException), "0x-10", "Short.decode allows negative sign in wrong position.");
                        yield return new TestCaseData(typeof(FormatException), "0x+10", "Short.decode allows positive sign in wrong position.");

                        yield return new TestCaseData(typeof(FormatException), "+", "Raw plus sign allowed.");
                        yield return new TestCaseData(typeof(FormatException), "-", "Raw minus sign allowed.");

                        yield return new TestCaseData(typeof(OverflowException), Convert.ToString((int)short.MinValue - 1, 10), "Out of range");
                        yield return new TestCaseData(typeof(OverflowException), Convert.ToString((int)short.MaxValue + 1, 10), "Out of range");

                        yield return new TestCaseData(typeof(FormatException), "", "Empty String");

                        // Harmony (Test_decodeLjava_lang_String())

                        yield return new TestCaseData(typeof(FormatException), "9.2", "Expected FormatException with floating point string.");
                        yield return new TestCaseData(typeof(FormatException), "", "Expected FormatException with empty string.");
                        yield return new TestCaseData(typeof(ArgumentNullException), null, "Expected NullPointerException with null string."); //undocumented NPE, but seems consistent across JREs

                        // Harmony (Test_decodeLjava_lang_String2())

                        yield return new TestCaseData(typeof(FormatException), "123s", "Did not throw FormatException decoding 123s");
                        yield return new TestCaseData(typeof(OverflowException), "32768", "Failed to throw exception for MAX_VALUE + 1");
                        yield return new TestCaseData(typeof(OverflowException), "-32769", "Failed to throw exception for MIN_VALUE - 1");
                        yield return new TestCaseData(typeof(OverflowException), "0x8000", "Failed to throw exception for hex MAX_VALUE + 1");
                        yield return new TestCaseData(typeof(OverflowException), "-0x8001", "Failed to throw exception for hex MIN_VALUE - 1");

                        // Custom

                        yield return new TestCaseData(typeof(OverflowException), "0xffff", "Negative not allowed without negative sign"); // -1 - negative values are not allowed per the docs
                    }
                }
            }

            #endregion DecodeTestCase

            #region Decode_CharSequence

            public abstract class Decode_CharSequence_TestCase : DecodeTestCase
            {
                protected abstract Int16 GetResult(string value);

                [TestCaseSource("TestDecode_CharSequence_Data")]
                public virtual void TestDecode_CharSequence(string value, short expected)
                {
                    var actual = GetResult(value);
                    assertEquals($"Int16.Decode(string) failed. String: \"{value}\" Result: {actual}", expected, actual);
                }

                [TestCaseSource("TestDecode_CharSequence_ForException_Data")]
                public virtual void TestDecode_CharSequence_ForException(Type expectedExceptionType, string value, string message)
                {
                    Assert.Throws(expectedExceptionType, () => GetResult(value), message);
                }
            }

            public class Decode_String : Decode_CharSequence_TestCase
            {
                protected override Int16 GetResult(string value)
                {
                    return Int16.Decode(value);
                }
            }

            // J2N: ReadOnlySpan<char> not supported at this time on this overload
            //#if FEATURE_READONLYSPAN
            //            public class Decode_ReadOnlySpan : Decode_CharSequence_TestCase
            //            {
            //                protected override Int16 GetResult(string s)
            //                {
            //                    return Int16.Decode(s.AsSpan());
            //                }
            //            }
            //#endif

            #endregion Decode_CharSequence

            #region TryDecode_CharSequence

            public abstract class TryDecode_CharSequence_TestCase : DecodeTestCase
            {
                protected abstract bool GetResult(string value, out Int16 result);

                [TestCaseSource("TestDecode_CharSequence_Data")]
                public virtual void TestTryDecode_CharSequence(string value, short expected)
                {
                    assertTrue(GetResult(value, out Int16 actual));
                    assertEquals($"Int16.TryDecode(string, out Int16) failed. String: \"{value}\" Result: {actual}", new Int16(expected), actual);
                }

                [TestCaseSource("TestDecode_CharSequence_ForException_Data")]
                public virtual void TestTryDecode_CharSequence_ForException(Type expectedExceptionType, string value, string message)
                {
                    assertFalse(GetResult(value, out Int16 actual));
                    assertEquals(null, actual);
                }
            }

            public class TryDecode_String : TryDecode_CharSequence_TestCase
            {
                protected override bool GetResult(string value, out Int16 result)
                {
                    return Int16.TryDecode(value, out result);
                }
            }

            // J2N: ReadOnlySpan<char> not supported at this time on this overload
            //#if FEATURE_READONLYSPAN
            //            public class TryDecode_ReadOnlySpan : TryDecode_CharSequence_TestCase
            //            {
            //                protected override bool GetResult(string s, out Int16 result)
            //                {
            //                    return Int16.TryDecode(s.AsSpan(), out result);
            //                }
            //            }
            //#endif

            #endregion TryDecode_CharSequence
        }
    }
}
