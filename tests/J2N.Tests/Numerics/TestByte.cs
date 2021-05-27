using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace J2N.Numerics
{
    public class TestByte : TestCase
    {

        /**
     * @tests java.lang.Byte#valueOf(byte)
     */
        [Test]
        public void Test_valueOfB()
        {
            assertEquals(new Byte(byte.MinValue), Byte.ValueOf(byte.MinValue));
            assertEquals(new Byte(byte.MaxValue), Byte.ValueOf(byte.MaxValue));
            assertEquals(new Byte((byte)0), Byte.ValueOf((byte)0));

            byte b = byte.MinValue + 1;
            while (b < byte.MaxValue)
            {
                assertEquals(new Byte(b), Byte.ValueOf(b));
                assertSame(Byte.ValueOf(b), Byte.ValueOf(b));
                b++;
            }
        }

        /**
         * @tests java.lang.Byte#hashCode()
         */
        [Test]
        public void Test_hashCode()
        {
            assertEquals(1, new Byte((byte)1).GetHashCode());
            assertEquals(2, new Byte((byte)2).GetHashCode());
            assertEquals(0, new Byte((byte)0).GetHashCode());
            assertEquals(-1, (sbyte)new Byte(unchecked((byte)-1)).GetHashCode()); // J2N: cast required to change the result to negative
        }

        /**
         * @tests java.lang.Byte#Byte(String)
         */
        [Test]
        public void Test_ConstructorLjava_lang_String()
        {
            assertEquals(new Byte((byte)0), new Byte("0", J2N.Text.StringFormatter.InvariantCulture));
            assertEquals(new Byte((byte)1), new Byte("1", J2N.Text.StringFormatter.InvariantCulture));
            assertEquals(new Byte(unchecked((byte)-1)), new Byte("-1", J2N.Text.StringFormatter.InvariantCulture));

            try
            {
                new Byte("0x1", J2N.Text.StringFormatter.InvariantCulture);
                fail("Expected FormatException with hex string.");
            }
            catch (FormatException e)
            {
            }

            try
            {
                new Byte("9.2", J2N.Text.StringFormatter.InvariantCulture);
                fail("Expected FormatException with floating point string.");
            }
            catch (FormatException e)
            {
            }

            try
            {
                new Byte("", J2N.Text.StringFormatter.InvariantCulture);
                fail("Expected FormatException with empty string.");
            }
            catch (FormatException e)
            {
            }

            try
            {
                new Byte(null, J2N.Text.StringFormatter.InvariantCulture);
                fail("Expected FormatException with null string.");
            }
            catch (ArgumentNullException e) // J2N: .NET throws ArgumentNullException rather than FormatException in this case
            {
            }
        }

        /**
         * @tests java.lang.Byte#Byte(byte)
         */
        [Test]
        public void Test_ConstructorB()
        {
            assertEquals(1, new Byte((byte)1).GetByteValue());
            assertEquals(2, new Byte((byte)2).GetByteValue());
            assertEquals(0, new Byte((byte)0).GetByteValue());
            assertEquals(-1, (sbyte)new Byte(unchecked((byte)-1)).GetByteValue()); // J2N: cast required to change the result to negative
        }

        /**
         * @tests java.lang.Byte#byteValue()
         */
        [Test]
        public void Test_booleanValue()
        {
            assertEquals(1, new Byte((byte)1).GetByteValue());
            assertEquals(2, new Byte((byte)2).GetByteValue());
            assertEquals(0, new Byte((byte)0).GetByteValue());
            assertEquals(-1, (sbyte)new Byte(unchecked((byte)-1)).GetByteValue()); // J2N: cast required to change the result to negative
        }

        /**
         * @tests java.lang.Byte#equals(Object)
         */
        [Test]
        public void Test_equalsLjava_lang_Object()
        {
            assertEquals(new Byte((byte)0), Byte.ValueOf((byte)0));
            assertEquals(new Byte((byte)1), Byte.ValueOf((byte)1));
            assertEquals(new Byte(unchecked((byte)-1)), Byte.ValueOf(unchecked((byte)-1)));

            Byte fixture = new Byte((byte)25);
            assertEquals(fixture, fixture);
            assertFalse(fixture.Equals(null));
            assertFalse(fixture.Equals("Not a Byte"));
        }

        /**
         * @tests java.lang.Byte#toString()
         */
        [Test]
        public void Test_toString()
        {
            // J2N: We get a byte value converted to a string, so the result is always positive
            assertEquals("255", new Byte(unchecked((byte)-1)).ToString(J2N.Text.StringFormatter.InvariantCulture)); // assertEquals("-1", new Byte(unchecked((byte)-1)).ToString(J2N.Text.StringFormatter.InvariantCulture));
            assertEquals("0", new Byte((byte)0).ToString(J2N.Text.StringFormatter.InvariantCulture));
            assertEquals("1", new Byte((byte)1).ToString(J2N.Text.StringFormatter.InvariantCulture));
            assertEquals("255", new Byte((byte)0xFF).ToString(J2N.Text.StringFormatter.InvariantCulture)); // assertEquals("-1", new Byte((byte)0xFF).ToString(J2N.Text.StringFormatter.InvariantCulture));
        }

        /**
         * @tests java.lang.Byte#toString(byte)
         */
        [Test]
        public void Test_toStringB()
        {
            // J2N: We get a byte value converted to a string, so the result is always positive
            assertEquals("255", Byte.ToString(unchecked((byte)-1), J2N.Text.StringFormatter.InvariantCulture)); // assertEquals("-1", Byte.ToString(unchecked((byte)-1), J2N.Text.StringFormatter.InvariantCulture));
            assertEquals("0", Byte.ToString((byte)0, J2N.Text.StringFormatter.InvariantCulture));
            assertEquals("1", Byte.ToString((byte)1, J2N.Text.StringFormatter.InvariantCulture));
            assertEquals("255", Byte.ToString((byte)0xFF, J2N.Text.StringFormatter.InvariantCulture)); // assertEquals("-1", Byte.ToString((byte)0xFF, J2N.Text.StringFormatter.InvariantCulture));
        }

        /**
         * @tests java.lang.Byte#valueOf(String)
         */
        [Test]
        public void Test_valueOfLjava_lang_String()
        {
            assertEquals(new Byte((byte)0), Byte.ValueOf("0", J2N.Text.StringFormatter.InvariantCulture));
            assertEquals(new Byte((byte)1), Byte.ValueOf("1", J2N.Text.StringFormatter.InvariantCulture));
            assertEquals(new Byte(unchecked((byte)-1)), Byte.ValueOf("-1", J2N.Text.StringFormatter.InvariantCulture));

            try
            {
                Byte.ValueOf("0x1", J2N.Text.StringFormatter.InvariantCulture);
                fail("Expected FormatException with hex string.");
            }
            catch (FormatException e)
            {
            }

            try
            {
                Byte.ValueOf("9.2", J2N.Text.StringFormatter.InvariantCulture);
                fail("Expected FormatException with floating point string.");
            }
            catch (FormatException e)
            {
            }

            try
            {
                Byte.ValueOf("", J2N.Text.StringFormatter.InvariantCulture);
                fail("Expected FormatException with empty string.");
            }
            catch (FormatException e)
            {
            }

            try
            {
                Byte.ValueOf(null, J2N.Text.StringFormatter.InvariantCulture);
                fail("Expected FormatException with null string.");
            }
            catch (ArgumentNullException e) // J2N: .NET throws ArgumentNullException rather than FormatException in this case
            {
            }
        }

        /**
         * @tests java.lang.Byte#valueOf(String,int)
         */
        [Test]
        public void Test_valueOfLjava_lang_StringI()
        {
            assertEquals(new Byte((byte)0), Byte.ValueOf("0", 10));
            assertEquals(new Byte((byte)1), Byte.ValueOf("1", 10));
            assertEquals(new Byte(unchecked((byte)-1)), Byte.ValueOf("-1", 10));

            //must be consistent with Character.digit()
            assertEquals(Character.Digit('1', 2), Byte.ValueOf("1", 2).GetByteValue());
            assertEquals(Character.Digit('F', 16), Byte.ValueOf("F", 16).GetByteValue());

            try
            {
                Byte.ValueOf("0x1", 10);
                fail("Expected FormatException with hex string.");
            }
            catch (FormatException e)
            {
            }

            try
            {
                Byte.ValueOf("9.2", 10);
                fail("Expected FormatException with floating point string.");
            }
            catch (FormatException e)
            {
            }

            try
            {
                Byte.ValueOf("", 10);
                fail("Expected FormatException with empty string.");
            }
            catch (FormatException e)
            {
            }

            //try
            //{
            //    Byte.ValueOf(null, 10);
            //    fail("Expected FormatException with null string.");
            //}
            //catch (FormatException e)
            //{
            //}

            // J2N: Match .NET behavior and return 0 for a null string
            assertEquals(new Byte(0), Byte.ValueOf(null, 10));
        }

        /**
         * @tests java.lang.Byte#parseByte(String)
         */
        [Test]
        public void Test_parseByteLjava_lang_String()
        {
            assertEquals(0, Byte.Parse("0", J2N.Text.StringFormatter.InvariantCulture));
            assertEquals(1, Byte.Parse("1", J2N.Text.StringFormatter.InvariantCulture));
            assertEquals(-1, (sbyte)Byte.Parse("-1", J2N.Text.StringFormatter.InvariantCulture)); // J2N: Parse allowed, but cast required to change the result to negative

            try
            {
                Byte.Parse("0x1", J2N.Text.StringFormatter.InvariantCulture);
                fail("Expected FormatException with hex string.");
            }
            catch (FormatException e)
            {
            }

            try
            {
                Byte.Parse("9.2", J2N.Text.StringFormatter.InvariantCulture);
                fail("Expected FormatException with floating point string.");
            }
            catch (FormatException e)
            {
            }

            try
            {
                Byte.Parse("", J2N.Text.StringFormatter.InvariantCulture);
                fail("Expected FormatException with empty string.");
            }
            catch (FormatException e)
            {
            }

            try
            {
                Byte.Parse(null, J2N.Text.StringFormatter.InvariantCulture);
                fail("Expected FormatException with null string.");
            }
            catch (ArgumentNullException e) // J2N: .NET throws ArgumentNullException rather than FormatException in this case
            {
            }
        }

        /**
         * @tests java.lang.Byte#parseByte(String,int)
         */
        [Test]
        public void Test_parseByteLjava_lang_StringI()
        {
            assertEquals(0, Byte.Parse("0", 10));
            assertEquals(1, Byte.Parse("1", 10));
            assertEquals(-1, (sbyte)Byte.Parse("-1", 10)); // J2N: Parse allowed, but cast required to change the result to negative

            //must be consistent with Character.digit()
            assertEquals(Character.Digit('1', 2), Byte.Parse("1", 2));
            assertEquals(Character.Digit('F', 16), Byte.Parse("F", 16));

            try
            {
                Byte.Parse("0x1", 10);
                fail("Expected FormatException with hex string.");
            }
            catch (FormatException e)
            {
            }

            try
            {
                Byte.Parse("9.2", 10);
                fail("Expected FormatException with floating point string.");
            }
            catch (FormatException e)
            {
            }

            try
            {
                Byte.Parse("", 10);
                fail("Expected FormatException with empty string.");
            }
            catch (FormatException e)
            {
            }

            //try
            //{
            //    Byte.Parse(null, 10);
            //    fail("Expected FormatException with null string.");
            //}
            //catch (FormatException e)
            //{
            //}

            // J2N: Match .NET behavior and return 0 for a null string
            assertEquals(0, Byte.Parse(null, 10));
        }

        /**
         * @tests java.lang.Byte#decode(String)
         */
        [Test]
        public void Test_decodeLjava_lang_String()
        {
            assertEquals(new Byte((byte)0), Byte.Decode("0"));
            assertEquals(new Byte((byte)1), Byte.Decode("1"));
            assertEquals(new Byte(unchecked((byte)-1)), Byte.Decode("-1"));
            assertEquals(new Byte((byte)0xF), Byte.Decode("0xF"));
            assertEquals(new Byte((byte)0xF), Byte.Decode("#F"));
            assertEquals(new Byte((byte)0xF), Byte.Decode("0XF"));
            assertEquals(new Byte((byte)07), Byte.Decode("07"));

            try
            {
                Byte.Decode("9.2");
                fail("Expected FormatException with floating point string.");
            }
            catch (FormatException e)
            {
            }

            try
            {
                Byte.Decode("");
                fail("Expected FormatException with empty string.");
            }
            catch (FormatException e)
            {
            }

            try
            {
                Byte.Decode(null);
                //undocumented NPE, but seems consistent across JREs
                fail("Expected NullPointerException with null string.");
            }
            catch (ArgumentNullException e)
            {
            }
        }

        /**
         * @tests java.lang.Byte#doubleValue()
         */
        [Test]
        public void Test_doubleValue()
        {
            // J2N: We get a byte value converted to a double, so the result is always positive
            assertEquals(255D, new Byte(unchecked((byte)-1)).GetDoubleValue(), 0D); // assertEquals(-1D, new Byte(unchecked((byte)-1)).GetDoubleValue(), 0D);
            assertEquals(0D, new Byte((byte)0).GetDoubleValue(), 0D);
            assertEquals(1D, new Byte((byte)1).GetDoubleValue(), 0D);
        }

        /**
         * @tests java.lang.Byte#floatValue()
         */
        [Test]
        public void Test_floatValue()
        {
            // J2N: We get a byte value converted to a double, so the result is always positive
            assertEquals(255F, new Byte(unchecked((byte)-1)).GetSingleValue(), 0F); // assertEquals(-1F, new Byte(unchecked((byte)-1)).GetSingleValue(), 0F);
            assertEquals(0F, new Byte((byte)0).GetSingleValue(), 0F);
            assertEquals(1F, new Byte((byte)1).GetSingleValue(), 0F);
        }

        /**
         * @tests java.lang.Byte#intValue()
         */
        [Test]
        public void Test_intValue()
        {
            // J2N: We get a byte value converted to an int, so the result is always positive
            assertEquals(255, new Byte(unchecked((byte)-1)).GetInt32Value()); // assertEquals(-1, new Byte(unchecked((byte)-1)).GetInt32Value());
            assertEquals(0, new Byte((byte)0).GetInt32Value());
            assertEquals(1, new Byte((byte)1).GetInt32Value());
        }

        /**
         * @tests java.lang.Byte#longValue()
         */
        [Test]
        public void Test_longValue()
        {
            // J2N: We get a byte value converted to an int, so the result is always positive
            assertEquals(255L, new Byte(unchecked((byte)-1)).GetInt64Value()); // assertEquals(-1L, new Byte(unchecked((byte)-1)).GetInt64Value());
            assertEquals(0L, new Byte((byte)0).GetInt64Value());
            assertEquals(1L, new Byte((byte)1).GetInt64Value());
        }

        /**
         * @tests java.lang.Byte#shortValue()
         */
        [Test]
        public void Test_shortValue()
        {
            // J2N: We get a byte value converted to an int, so the result is always positive
            assertEquals(255, new Byte(unchecked((byte)-1)).GetInt16Value()); // assertEquals(-1, new Byte(unchecked((byte)-1)).GetInt16Value());
            assertEquals(0, new Byte((byte)0).GetInt16Value());
            assertEquals(1, new Byte((byte)1).GetInt16Value());
        }

        /**
         * @tests java.lang.Byte#compareTo(Byte)
         */
        [Test]
        public void Test_compareToLjava_lang_Byte()
        {
            //Byte min = new Byte(byte.MinValue);
            //Byte zero = new Byte((byte)0);
            //Byte max = new Byte(byte.MaxValue);

            //assertTrue(max.CompareTo(max) == 0);
            //assertTrue(min.CompareTo(min) == 0);
            //assertTrue(zero.CompareTo(zero) == 0);

            //assertTrue(max.CompareTo(zero) > 0);
            //assertTrue(max.CompareTo(min) > 0);

            //assertTrue(zero.CompareTo(max) < 0);
            //assertTrue(zero.CompareTo(min) > 0);

            //assertTrue(min.CompareTo(zero) < 0);
            //assertTrue(min.CompareTo(max) < 0);

            //try
            //{
            //    min.CompareTo(null);
            //    fail("No NPE");
            //}
            //catch (NullPointerException e)
            //{
            //}

            // J2N: Changed from using zero to median, as zero is already in use by byte.MinValue
            Byte min = new Byte(byte.MinValue);
            Byte median = new Byte((byte)sbyte.MaxValue);
            Byte max = new Byte(byte.MaxValue);

            assertTrue(max.CompareTo(max) == 0);
            assertTrue(min.CompareTo(min) == 0);
            assertTrue(median.CompareTo(median) == 0);

            assertTrue(max.CompareTo(median) > 0);
            assertTrue(max.CompareTo(min) > 0);

            assertTrue(median.CompareTo(max) < 0);
            assertTrue(median.CompareTo(min) > 0);

            assertTrue(min.CompareTo(median) < 0);
            assertTrue(min.CompareTo(max) < 0);

            // J2N: Return 1 when comparing to null to match other .NET classes
            assertEquals(1, min.CompareTo(null));
        }

        /**
         * @tests java.lang.Byte#Byte(byte)
         */
        [Test]
        public void Test_ConstructorB2()
        {
            // Test for method java.lang.Byte(byte)

            Byte b = new Byte((byte)127);
            assertTrue("Byte creation failed", b.GetByteValue() == (byte)127);
        }

        /**
         * @tests java.lang.Byte#Byte(java.lang.String)
         */
        [Test]
        public void Test_ConstructorLjava_lang_String2()
        {
            // Test for method java.lang.Byte(java.lang.String)

            Byte b = new Byte("127", J2N.Text.StringFormatter.InvariantCulture);
            Byte nb = new Byte("-128", J2N.Text.StringFormatter.InvariantCulture);
            assertTrue("Incorrect Byte Object created", b.GetByteValue() == (byte)127
                    && (nb.GetByteValue() == unchecked((byte)-128)));

        }

        /**
         * @tests java.lang.Byte#byteValue()
         */
        [Test]
        public void Test_byteValue()
        {
            // Test for method byte java.lang.Byte.byteValue()
            assertTrue("Returned incorrect byte value",
                    new Byte((byte)127).GetByteValue() == (byte)(127));
        }

        /**
         * @tests java.lang.Byte#compareTo(java.lang.Byte)
         */
        [Test]
        public void Test_compareToLjava_lang_Byte2()
        {
            // Test for method int java.lang.Byte.compareTo(java.lang.Byte)
            assertTrue("Comparison failed", new Byte((byte)1).CompareTo(new Byte((byte)2)) < 0);
            // J2N: Comparison is with byte, which is 254, not -2
            assertTrue("Comparison failed", new Byte((byte)1).CompareTo(new Byte(unchecked((byte)-2))) < 0); // assertTrue("Comparison failed", new Byte((byte)1).CompareTo(new Byte(unchecked((byte)-2))) > 0);
            assertEquals("Comparison failed", 0, new Byte((byte)1).CompareTo(new Byte((byte)1)));
        }

        /**
         * @tests java.lang.Byte#decode(java.lang.String)
         */
        [Test]
        public void Test_decodeLjava_lang_String2()
        {
            // Test for method java.lang.Byte
            // java.lang.Byte.decode(java.lang.String)
            assertTrue("String decoded incorrectly, wanted: 1 got: " + Byte.Decode("1").ToString(),
                    Byte.Decode("1").Equals(new Byte((byte)1)));
            assertTrue("String decoded incorrectly, wanted: -1 got: "
                    + Byte.Decode("-1").ToString(), Byte.Decode("-1").Equals(new Byte(unchecked((byte)-1))));
            assertTrue("String decoded incorrectly, wanted: 127 got: "
                    + Byte.Decode("127").ToString(), Byte.Decode("127")
                    .Equals(new Byte((byte)127)));
            assertTrue("String decoded incorrectly, wanted: -128 got: "
                    + Byte.Decode("-128").ToString(), Byte.Decode("-128").Equals(
                    new Byte(unchecked((byte)-128))));
            assertTrue("String decoded incorrectly, wanted: 127 got: "
                    + Byte.Decode("0x7f").ToString(), Byte.Decode("0x7f").Equals(
                    new Byte((byte)127)));
            assertTrue("String decoded incorrectly, wanted: -128 got: "
                    + Byte.Decode("-0x80").ToString(), Byte.Decode("-0x80").Equals(
                    new Byte(unchecked((byte)-128))));

            bool exception = false;
            try
            {
                //Byte.Decode("128");
                Byte.Decode("256"); // J2N: We allow parsing from sbyte.MinValue to byte.MaxValue for compatibility
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
                Byte.Decode("-129");
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
                //Byte.Decode("0x80");
                Byte.Decode("0x0100"); // J2N: We allow parsing from sbyte.MinValue to byte.MaxValue for compatibility
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
                Byte.Decode("-0x81");
            }
            catch (FormatException e)
            {
                // Correct
                exception = true;
            }
            assertTrue("Failed to throw exception for hex MIN_VALUE - 1", exception);
        }

        /**
         * @tests java.lang.Byte#doubleValue()
         */
        [Test]
        public void Test_doubleValue2()
        {
            assertEquals(127D, new Byte((byte)127).GetDoubleValue(), 0.0);
        }

        /**
         * @tests java.lang.Byte#equals(java.lang.Object)
         */
        [Test]
        public void Test_equalsLjava_lang_Object2()
        {
            // Test for method boolean java.lang.Byte.equals(java.lang.Object)
            Byte b1 = new Byte((byte)90);
            Byte b2 = new Byte((byte)90);
            Byte b3 = new Byte(unchecked((byte)-90));
            assertTrue("Equality test failed", b1.Equals(b2));
            assertTrue("Equality test failed", !b1.Equals(b3));
        }

        /**
         * @tests java.lang.Byte#floatValue()
         */
        [Test]
        public void Test_floatValue2()
        {
            assertEquals(127F, new Byte((byte)127).GetSingleValue(), 0.0);
        }

        /**
         * @tests java.lang.Byte#hashCode()
         */
        [Test]
        public void Test_hashCode2()
        {
            // Test for method int java.lang.Byte.hashCode()
            assertEquals("Incorrect hash returned", 127, new Byte((byte)127).GetHashCode());
        }

        /**
         * @tests java.lang.Byte#intValue()
         */
        [Test]
        public void Test_intValue2()
        {
            // Test for method int java.lang.Byte.intValue()
            assertEquals("Returned incorrect int value", 127, new Byte((byte)127).GetInt32Value());
        }

        /**
         * @tests java.lang.Byte#longValue()
         */
        [Test]
        public void Test_longValue2()
        {
            // Test for method long java.lang.Byte.longValue()
            assertEquals("Returned incorrect long value", 127L, new Byte((byte)127).GetInt64Value());
        }

        /**
         * @tests java.lang.Byte#parseByte(java.lang.String)
         */
        [Test]
        public void Test_parseByteLjava_lang_String2()
        {
            assertEquals((byte)127, Byte.Parse("127", J2N.Text.StringFormatter.InvariantCulture));
            assertEquals(unchecked((byte)-128), Byte.Parse("-128", J2N.Text.StringFormatter.InvariantCulture));
            assertEquals((byte)0, Byte.Parse("0", J2N.Text.StringFormatter.InvariantCulture));
            assertEquals((byte)0x80, Byte.Parse("-128", J2N.Text.StringFormatter.InvariantCulture));
            assertEquals((byte)0x7F, Byte.Parse("127", J2N.Text.StringFormatter.InvariantCulture));

            try
            {
                Byte.Parse("-1000", J2N.Text.StringFormatter.InvariantCulture);
                fail("No FormatException");
            }
            catch (OverflowException e) // J2N: .NET throws OverflowException rather than FormatException in this case
            {
            }

            try
            {
                //Byte.ParseByte("128");
                Byte.Parse("256", J2N.Text.StringFormatter.InvariantCulture); // J2N: We allow parsing from sbyte.MinValue to byte.MaxValue for compatibility
                fail("No FormatException");
            }
            catch (OverflowException e) // J2N: .NET throws OverflowException rather than FormatException in this case
            {
            }

            try
            {
                Byte.Parse("-129", J2N.Text.StringFormatter.InvariantCulture);
                fail("No FormatException");
            }
            catch (OverflowException e) // J2N: .NET throws OverflowException rather than FormatException in this case
            {
            }
        }

        /**
         * @tests java.lang.Byte#parseByte(java.lang.String, int)
         */
        [Test]
        public void Test_parseByteLjava_lang_StringI2()
        {
            // Test for method byte java.lang.Byte.parseByte(java.lang.String, int)
            byte b = Byte.Parse("127", 10);
            byte bn = Byte.Parse("-128", 10);
            assertTrue("Invalid parse of dec byte", b == (byte)127 && (bn == unchecked((byte)-128)));
            assertEquals("Failed to parse hex value", 10, Byte.Parse("A", 16));
            assertEquals("Returned incorrect value for 0 hex", 0, Byte.Parse("0", 16));
            assertTrue("Returned incorrect value for most negative value hex", Byte.Parse(
                    "-80", 16) == (byte)0x80);
            assertTrue("Returned incorrect value for most positive value hex", Byte.Parse("7f",
                    16) == 0x7f);
            assertEquals("Returned incorrect value for 0 decimal", 0, Byte.Parse("0", 10));
            assertTrue("Returned incorrect value for most negative value decimal", Byte.Parse(
                    "-128", 10) == (byte)0x80);
            assertTrue("Returned incorrect value for most positive value decimal", Byte.Parse(
                    "127", 10) == 0x7f);

            try
            {
                Byte.Parse("-1000", 10);
                fail("Failed to throw exception");
            }
            catch (OverflowException e) // J2N: .NET throws OverflowException rather than FormatException in this case
            {
            }

            try
            {
                //Byte.ParseByte("128", 10);
                Byte.Parse("256", 10); // J2N: We allow parsing from sbyte.MinValue to byte.MaxValue for compatibility
                fail("Failed to throw exception for MAX_VALUE + 1");
            }
            catch (OverflowException e) // J2N: .NET throws OverflowException rather than FormatException in this case
            {
            }

            try
            {
                Byte.Parse("-129", 10);
                fail("Failed to throw exception for MIN_VALUE - 1");
            }
            catch (OverflowException e) // J2N: .NET throws OverflowException rather than FormatException in this case
            {
            }

            try
            {
                //Byte.ParseByte("80", 16);
                Byte.Parse("100", 16); // J2N: We allow parsing from sbyte.MinValue to byte.MaxValue for compatibility
                fail("Failed to throw exception for hex MAX_VALUE + 1");
            }
            catch (OverflowException e) // J2N: .NET throws OverflowException rather than FormatException in this case
            {
            }

            assertEquals(1, Byte.Parse("1", 16));
            assertEquals(-1, (sbyte)Byte.Parse("-1", 16));

            assertEquals(sbyte.MaxValue, Byte.Parse("7f", 16));
            assertEquals(-128, (sbyte)Byte.Parse("-80", 16)); // Special case: In Java, we allow the negative sign for the smallest negative number
            assertEquals(-128, (sbyte)Byte.Parse("80", 16));  // In .NET, it should parse without the negative sign to the same value (in .NET the negative sign is not allowed)
            assertEquals(-127, (sbyte)Byte.Parse("81", 16));

            try
            {
                Byte.Parse("-81", 16);
                fail("Failed to throw exception for hex MIN_VALUE + 1");
            }
            catch (OverflowException e) // J2N: .NET throws OverflowException rather than FormatException in this case
            {
            }
        }

        /**
         * @tests java.lang.Byte#shortValue()
         */
        [Test]
        public void Test_shortValue2()
        {
            assertEquals((short)127, new Byte((byte)127).GetInt16Value());
        }

        /**
         * @tests java.lang.Byte#toString()
         */
        [Test]
        public void Test_toString2()
        {
            // J2N: We get a byte value converted to a string, so the result is always positive
            assertEquals("Returned incorrect String", "127", new Byte((byte)127).ToString(J2N.Text.StringFormatter.InvariantCulture));
            assertEquals("Returned incorrect String", "129", new Byte(unchecked((byte)-127)).ToString(J2N.Text.StringFormatter.InvariantCulture)); // assertEquals("Returned incorrect String", "-127", new Byte(unchecked((byte)-127)).ToString(J2N.Text.StringFormatter.InvariantCulture));
            assertEquals("Returned incorrect String", "128", new Byte(unchecked((byte)-128)).ToString(J2N.Text.StringFormatter.InvariantCulture)); // assertEquals("Returned incorrect String", "-128", new Byte(unchecked((byte)-128)).ToString(J2N.Text.StringFormatter.InvariantCulture));
        }

        /**
         * @tests java.lang.Byte#toString(byte)
         */
        [Test]
        public void Test_toStringB2()
        {
            // J2N: We get a byte value converted to a string, so the result is always positive
            assertEquals("Returned incorrect String", "127", Byte.ToString((byte)127, J2N.Text.StringFormatter.InvariantCulture));
            assertEquals("Returned incorrect String", "129", Byte.ToString(unchecked((byte)-127), J2N.Text.StringFormatter.InvariantCulture)); // assertEquals("Returned incorrect String", "-127", Byte.ToString(unchecked((byte)-127), J2N.Text.StringFormatter.InvariantCulture));
            assertEquals("Returned incorrect String", "128", Byte.ToString(unchecked((byte)-128), J2N.Text.StringFormatter.InvariantCulture)); // assertEquals("Returned incorrect String", "-128", Byte.ToString(unchecked((byte)-128), J2N.Text.StringFormatter.InvariantCulture));
        }

        /**
         * @tests java.lang.Byte#valueOf(java.lang.String)
         */
        [Test]
        public void Test_valueOfLjava_lang_String2()
        {
            assertEquals("Returned incorrect byte", 0, Byte.ValueOf("0", J2N.Text.StringFormatter.InvariantCulture).GetByteValue());
            assertEquals("Returned incorrect byte", 127, Byte.ValueOf("127", J2N.Text.StringFormatter.InvariantCulture).GetByteValue());
            assertEquals("Returned incorrect byte", -127, (sbyte)Byte.ValueOf("-127", J2N.Text.StringFormatter.InvariantCulture).GetByteValue()); // J2N: Parse allowed, but cast required to change the result to negative
            assertEquals("Returned incorrect byte", -128, (sbyte)Byte.ValueOf("-128", J2N.Text.StringFormatter.InvariantCulture).GetByteValue()); // J2N: Parse allowed, but cast required to change the result to negative

            try
            {
                //Byte.ValueOf("128");
                Byte.ValueOf("256", J2N.Text.StringFormatter.InvariantCulture); // J2N: We allow parsing from sbyte.MinValue to byte.MaxValue for compatibility
                fail("Failed to throw exception when passes value > byte");
            }
            catch (OverflowException e) // J2N: .NET throws OverflowException rather than FormatException in this case
            {
            }
        }

        /**
         * @tests java.lang.Byte#valueOf(java.lang.String, int)
         */
        [Test]
        public void Test_valueOfLjava_lang_StringI2()
        {
            assertEquals("Returned incorrect byte", 10, Byte.ValueOf("A", 16).GetByteValue());
            assertEquals("Returned incorrect byte", 127, Byte.ValueOf("127", 10).GetByteValue());
            assertEquals("Returned incorrect byte", -127, (sbyte)Byte.ValueOf("-127", 10).GetByteValue()); // J2N: Parse allowed, but cast required to change the result to negative
            assertEquals("Returned incorrect byte", -128, (sbyte)Byte.ValueOf("-128", 10).GetByteValue()); // J2N: Parse allowed, but cast required to change the result to negative
            assertEquals("Returned incorrect byte", 127, Byte.ValueOf("7f", 16).GetByteValue());
            assertEquals("Returned incorrect byte", -127, (sbyte)Byte.ValueOf("-7f", 16).GetByteValue()); // J2N: Parse allowed, but cast required to change the result to negative
            assertEquals("Returned incorrect byte", -128, (sbyte)Byte.ValueOf("-80", 16).GetByteValue()); // J2N: Parse allowed, but cast required to change the result to negative

            try
            {
                //Byte.ValueOf("128", 10);
                Byte.ValueOf("256", 10); // J2N: We allow parsing from sbyte.MinValue to byte.MaxValue for compatibility
                fail("Failed to throw exception when passes value > byte");
            }
            catch (OverflowException e) // J2N: .NET throws OverflowException rather than FormatException in this case
            {
            }
        }
    }
}
