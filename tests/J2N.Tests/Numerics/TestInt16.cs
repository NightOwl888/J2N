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

        /**
         * @tests java.lang.Short#decode(java.lang.String)
         */
        [Test]
        public void Test_decodeLjava_lang_String2()
        {
            // Test for method java.lang.Short
            // java.lang.Short.decode(java.lang.String)
            assertTrue("Did not decode -1 correctly", Int16.Decode("-1")
                    .GetInt16Value() == (short)-1);
            assertTrue("Did not decode -100 correctly", Int16.Decode("-100")
                    .GetInt16Value() == (short)-100);
            assertTrue("Did not decode 23 correctly", Int16.Decode("23")
                    .GetInt16Value() == (short)23);
            assertTrue("Did not decode 0x10 correctly", Int16.Decode("0x10")
                    .GetInt16Value() == (short)16);
            assertTrue("Did not decode 32767 correctly", Int16.Decode("32767")
                    .GetInt16Value() == (short)32767);
            assertTrue("Did not decode -32767 correctly", Int16.Decode("-32767")
                    .GetInt16Value() == (short)-32767);
            assertTrue("Did not decode -32768 correctly", Int16.Decode("-32768")
                    .GetInt16Value() == (short)-32768);

            bool exception = false;
            try
            {
                Int16.Decode("123s");
            }
            catch (FormatException e)
            {
                // correct
                exception = true;
            }
            assertTrue("Did not throw FormatException decoding 123s",
                    exception);

            exception = false;
            try
            {
                Int16.Decode("32768");
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
                Int16.Decode("-32769");
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
                Int16.Decode("0x8000");
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
                Int16.Decode("-0x8001");
            }
            catch (FormatException e)
            {
                // Correct
                exception = true;
            }
            assertTrue("Failed to throw exception for hex MIN_VALUE - 1", exception);
        }

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

        /**
         * @tests java.lang.Short#parseShort(java.lang.String, int)
         */
        [Test]
        public void Test_parseShortLjava_lang_StringI2()
        {
            // Test for method short java.lang.Short.parseShort(java.lang.String,
            // int)
            bool aThrow = true;
            assertEquals("Incorrectly parsed hex string",
                    255, Int16.Parse("FF", 16));
            assertEquals("Incorrectly parsed oct string",
                    16, Int16.Parse("20", 8));
            assertEquals("Incorrectly parsed dec string",
                    20, Int16.Parse("20", 10));
            assertEquals("Incorrectly parsed bin string",
                    4, Int16.Parse("100", 2));
            assertEquals("Incorrectly parsed -hex string", -255, Int16
                    .Parse("-FF", 16));
            assertEquals("Incorrectly parsed -oct string",
                    -16, Int16.Parse("-20", 8));
            assertEquals("Incorrectly parsed -bin string", -4, Int16
                    .Parse("-100", 2));
            assertEquals("Returned incorrect value for 0 hex", 0, Int16.Parse("0",
                    16));
            assertTrue("Returned incorrect value for most negative value hex",
                    Int16.Parse("-8000", 16) == unchecked((short)0x8000));
            assertTrue("Returned incorrect value for most positive value hex",
                    Int16.Parse("7fff", 16) == 0x7fff);
            assertEquals("Returned incorrect value for 0 decimal", 0, Int16.Parse(
                    "0", 10));
            assertTrue("Returned incorrect value for most negative value decimal",
                    Int16.Parse("-32768", 10) == unchecked((short)0x8000));
            assertTrue("Returned incorrect value for most positive value decimal",
                    Int16.Parse("32767", 10) == 0x7fff);

            try
            {
                Int16.Parse("FF", 2);
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

            bool exception = false;
            try
            {
                Int16.Parse("10000000000", 10);
            }
            catch (OverflowException e) // J2N: .NET throws OverflowException rather than FormatException in this case
            {
                // Correct
                exception = true;
            }
            assertTrue(
                    "Failed to throw exception when passed string larger than 16 bits",
                    exception);

            exception = false;
            try
            {
                Int16.Parse("32768", 10);
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
                Int16.Parse("-32769", 10);
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
            //    Int16.Parse("8000", 16);
            //}
            //catch (OverflowException e) // J2N: .NET throws OverflowException rather than FormatException in this case
            //{
            //    // Correct
            //    exception = true;
            //}
            //assertTrue("Failed to throw exception for hex MAX_VALUE + 1", exception);

            assertEquals(1, Int16.Parse("1", 16));
            assertEquals(-1, Int16.Parse("ffff", 16));
            assertEquals(32767, Int16.Parse("7fff", 16));
            assertEquals(-32768, Int16.Parse("-8000", 16)); // Special case: In Java, we allow the negative sign for the smallest negative number
            assertEquals(-32768, Int16.Parse("8000", 16));  // In .NET, it should parse without the negative sign to the same value (in .NET the negative sign is not allowed)
            assertEquals(-32767, Int16.Parse("8001", 16));

            exception = false;
            try
            {
                Int16.Parse("-8001", 16);
            }
            catch (OverflowException e) // J2N: .NET throws OverflowException rather than FormatException in this case
            {
                // Correct
                exception = true;
            }
            assertTrue("Failed to throw exception for hex MIN_VALUE + 1", exception);
        }

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
            assertEquals("Returned incorrect short", -32768, Int16.ValueOf("-32768", J2N.Text.StringFormatter.InvariantCulture)
                    .GetInt16Value());
            assertEquals("Returned incorrect short", 32767, Int16.ValueOf("32767", J2N.Text.StringFormatter.InvariantCulture)
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
            assertEquals("Incorrectly parsed hex string", 255, Int16.ValueOf("FF", 16)
                    .GetInt16Value());
            assertEquals("Incorrectly parsed oct string", 16, Int16.ValueOf("20", 8)
                    .GetInt16Value());
            assertEquals("Incorrectly parsed dec string", 20, Int16.ValueOf("20", 10)
                    .GetInt16Value());
            assertEquals("Incorrectly parsed bin string", 4, Int16.ValueOf("100", 2)
                    .GetInt16Value());
            assertEquals("Incorrectly parsed -hex string", -255, Int16.ValueOf("-FF", 16)
                    .GetInt16Value());
            assertEquals("Incorrectly parsed -oct string", -16, Int16.ValueOf("-20", 8)
                    .GetInt16Value());
            assertEquals("Incorrectly parsed -bin string", -4, Int16.ValueOf("-100", 2)
                    .GetInt16Value());
            assertTrue("Did not decode 32767 correctly", Int16.ValueOf("32767", 10)
                    .GetInt16Value() == (short)32767);
            assertTrue("Did not decode -32767 correctly", Int16.ValueOf("-32767",
                    10).GetInt16Value() == (short)-32767);
            assertTrue("Did not decode -32768 correctly", Int16.ValueOf("-32768",
                    10).GetInt16Value() == (short)-32768);
            try
            {
                Int16.ValueOf("FF", 2);
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
                Int16.ValueOf("10000000000", 10);
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
            assertEquals(new Int16(short.MinValue), Int16.ValueOf(short.MinValue));
            assertEquals(new Int16(short.MaxValue), Int16.ValueOf(short.MaxValue));
            assertEquals(new Int16((short)0), Int16.ValueOf((short)0));

            short s = -128;
            while (s < 128)
            {
                assertEquals(new Int16(s), Int16.ValueOf(s));
                assertSame(Int16.ValueOf(s), Int16.ValueOf(s));
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

        /**
         * @tests java.lang.Short#Short(String)
         */
        [Test]
        public void Test_ConstructorLjava_lang_String()
        {
            assertEquals(new Int16((short)0), new Int16("0", J2N.Text.StringFormatter.InvariantCulture));
            assertEquals(new Int16((short)1), new Int16("1", J2N.Text.StringFormatter.InvariantCulture));
            assertEquals(new Int16((short)-1), new Int16("-1", J2N.Text.StringFormatter.InvariantCulture));

            try
            {
                new Int16("0x1", J2N.Text.StringFormatter.InvariantCulture);
                fail("Expected FormatException with hex string.");
            }
            catch (FormatException e) { }

            try
            {
                new Int16("9.2", J2N.Text.StringFormatter.InvariantCulture);
                fail("Expected FormatException with floating point string.");
            }
            catch (FormatException e) { }

            try
            {
                new Int16("", J2N.Text.StringFormatter.InvariantCulture);
                fail("Expected FormatException with empty string.");
            }
            catch (FormatException e) { }

            try
            {
                new Int16(null, J2N.Text.StringFormatter.InvariantCulture);
                fail("Expected FormatException with null string.");
            }
            catch (ArgumentNullException e) { } // J2N: .NET throws ArgumentNullException rather than FormatException in this case
        }

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
            assertEquals(new Int16((short)0), Int16.ValueOf((short)0));
            assertEquals(new Int16((short)1), Int16.ValueOf((short)1));
            assertEquals(new Int16((short)-1), Int16.ValueOf((short)-1));

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
            assertEquals(new Int16((short)0), Int16.ValueOf("0", J2N.Text.StringFormatter.InvariantCulture));
            assertEquals(new Int16((short)1), Int16.ValueOf("1", J2N.Text.StringFormatter.InvariantCulture));
            assertEquals(new Int16((short)-1), Int16.ValueOf("-1", J2N.Text.StringFormatter.InvariantCulture));

            try
            {
                Int16.ValueOf("0x1", J2N.Text.StringFormatter.InvariantCulture);
                fail("Expected FormatException with hex string.");
            }
            catch (FormatException e) { }

            try
            {
                Int16.ValueOf("9.2", J2N.Text.StringFormatter.InvariantCulture);
                fail("Expected FormatException with floating point string.");
            }
            catch (FormatException e) { }

            try
            {
                Int16.ValueOf("", J2N.Text.StringFormatter.InvariantCulture);
                fail("Expected FormatException with empty string.");
            }
            catch (FormatException e) { }

            try
            {
                Int16.ValueOf(null, J2N.Text.StringFormatter.InvariantCulture);
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
            assertEquals(new Int16((short)0), Int16.ValueOf("0", 10));
            assertEquals(new Int16((short)1), Int16.ValueOf("1", 10));
            assertEquals(new Int16((short)-1), Int16.ValueOf("-1", 10));

            //must be consistent with Character.digit()
            assertEquals(Character.Digit('1', 2), Int16.ValueOf("1", 2).GetByteValue());
            assertEquals(Character.Digit('F', 16), Int16.ValueOf("F", 16).GetByteValue());

            try
            {
                Int16.ValueOf("0x1", 10);
                fail("Expected FormatException with hex string.");
            }
            catch (FormatException e) { }

            try
            {
                Int16.ValueOf("9.2", 10);
                fail("Expected FormatException with floating point string.");
            }
            catch (FormatException e) { }

            try
            {
                Int16.ValueOf("", 10);
                fail("Expected FormatException with empty string.");
            }
            catch (FormatException e) { }

            //try
            //{
            //    Int16.ValueOf(null, 10);
            //    fail("Expected FormatException with null string.");
            //}
            //catch (FormatException e) { }

            // J2N: Match .NET behavior where null will result in 0
            assertEquals(0, Int16.ValueOf(null, 10));
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

        /**
         * @tests java.lang.Short#parseShort(String,int)
         */
        [Test]
        public void Test_parseShortLjava_lang_StringI()
        {
            assertEquals(0, Int16.Parse("0", 10));
            assertEquals(1, Int16.Parse("1", 10));
            assertEquals(-1, Int16.Parse("-1", 10));

            //must be consistent with Character.digit()
            assertEquals(Character.Digit('1', 2), Int16.Parse("1", 2));
            assertEquals(Character.Digit('F', 16), Int16.Parse("F", 16));

            try
            {
                Int16.Parse("0x1", 10);
                fail("Expected FormatException with hex string.");
            }
            catch (FormatException e) { }

            try
            {
                Int16.Parse("9.2", 10);
                fail("Expected FormatException with floating point string.");
            }
            catch (FormatException e) { }

            try
            {
                Int16.Parse("", 10);
                fail("Expected FormatException with empty string.");
            }
            catch (FormatException e) { }

            //try
            //{
            //    Int16.Parse(null, 10);
            //    fail("Expected FormatException with null string.");
            //}
            //catch (FormatException e) { }

            // J2N: Match .NET behavior where null will result in 0
            assertEquals(0, Int16.Parse(null, 10));
        }

        /**
         * @tests java.lang.Short#decode(String)
         */
        [Test]
        public void Test_decodeLjava_lang_String()
        {
            assertEquals(new Int16((short)0), Int16.Decode("0"));
            assertEquals(new Int16((short)1), Int16.Decode("1"));
            assertEquals(new Int16((short)-1), Int16.Decode("-1"));
            assertEquals(new Int16((short)0xF), Int16.Decode("0xF"));
            assertEquals(new Int16((short)0xF), Int16.Decode("#F"));
            assertEquals(new Int16((short)0xF), Int16.Decode("0XF"));
            assertEquals(new Int16((short)07), Int16.Decode("07"));

            try
            {
                Int16.Decode("9.2");
                fail("Expected FormatException with floating point string.");
            }
            catch (FormatException e) { }

            try
            {
                Int16.Decode("");
                fail("Expected FormatException with empty string.");
            }
            catch (FormatException e) { }

            try
            {
                Int16.Decode(null);
                //undocumented NPE, but seems consistent across JREs
                fail("Expected NullPointerException with null string.");
            }
            catch (ArgumentNullException e) { }
        }

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
    }
}
