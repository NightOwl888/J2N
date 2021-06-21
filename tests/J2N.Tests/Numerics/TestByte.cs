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
    public class TestByte : TestCase
    {

        /**
     * @tests java.lang.Byte#valueOf(byte)
     */
        [Test]
        public void Test_valueOfB()
        {
            assertEquals(new Byte(byte.MinValue), Byte.GetValueOf(byte.MinValue));
            assertEquals(new Byte(byte.MaxValue), Byte.GetValueOf(byte.MaxValue));
            assertEquals(new Byte((byte)0), Byte.GetValueOf((byte)0));

            byte b = byte.MinValue + 1;
            while (b < byte.MaxValue)
            {
                assertEquals(new Byte(b), Byte.GetValueOf(b));
                assertSame(Byte.GetValueOf(b), Byte.GetValueOf(b));
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

        // J2N: Removed this overload because all of the constructors are deprecated in JDK 16
        ///**
        // * @tests java.lang.Byte#Byte(String)
        // */
        //[Test]
        //public void Test_ConstructorLjava_lang_String()
        //{
        //    assertEquals(new Byte((byte)0), new Byte("0", J2N.Text.StringFormatter.InvariantCulture));
        //    assertEquals(new Byte((byte)1), new Byte("1", J2N.Text.StringFormatter.InvariantCulture));
        //    assertEquals(new Byte(unchecked((byte)-1)), new Byte("-1", J2N.Text.StringFormatter.InvariantCulture));

        //    try
        //    {
        //        new Byte("0x1", J2N.Text.StringFormatter.InvariantCulture);
        //        fail("Expected FormatException with hex string.");
        //    }
        //    catch (FormatException e)
        //    {
        //    }

        //    try
        //    {
        //        new Byte("9.2", J2N.Text.StringFormatter.InvariantCulture);
        //        fail("Expected FormatException with floating point string.");
        //    }
        //    catch (FormatException e)
        //    {
        //    }

        //    try
        //    {
        //        new Byte("", J2N.Text.StringFormatter.InvariantCulture);
        //        fail("Expected FormatException with empty string.");
        //    }
        //    catch (FormatException e)
        //    {
        //    }

        //    try
        //    {
        //        new Byte(null, J2N.Text.StringFormatter.InvariantCulture);
        //        fail("Expected FormatException with null string.");
        //    }
        //    catch (ArgumentNullException e) // J2N: .NET throws ArgumentNullException rather than FormatException in this case
        //    {
        //    }
        //}

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
            assertEquals(new Byte((byte)0), Byte.GetValueOf((byte)0));
            assertEquals(new Byte((byte)1), Byte.GetValueOf((byte)1));
            assertEquals(new Byte(unchecked((byte)-1)), Byte.GetValueOf(unchecked((byte)-1)));

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
            assertEquals(new Byte((byte)0), Byte.GetValueOf("0", J2N.Text.StringFormatter.InvariantCulture));
            assertEquals(new Byte((byte)1), Byte.GetValueOf("1", J2N.Text.StringFormatter.InvariantCulture));
            assertEquals(new Byte(unchecked((byte)-1)), Byte.GetValueOf("-1", J2N.Text.StringFormatter.InvariantCulture));

            try
            {
                Byte.GetValueOf("0x1", J2N.Text.StringFormatter.InvariantCulture);
                fail("Expected FormatException with hex string.");
            }
            catch (FormatException e)
            {
            }

            try
            {
                Byte.GetValueOf("9.2", J2N.Text.StringFormatter.InvariantCulture);
                fail("Expected FormatException with floating point string.");
            }
            catch (FormatException e)
            {
            }

            try
            {
                Byte.GetValueOf("", J2N.Text.StringFormatter.InvariantCulture);
                fail("Expected FormatException with empty string.");
            }
            catch (FormatException e)
            {
            }

            try
            {
                Byte.GetValueOf(null, J2N.Text.StringFormatter.InvariantCulture);
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
            assertEquals(new Byte((byte)0), Byte.GetValueOf("0", 10));
            assertEquals(new Byte((byte)1), Byte.GetValueOf("1", 10));
            assertEquals(new Byte(unchecked((byte)-1)), Byte.GetValueOf("-1", 10));

            //must be consistent with Character.digit()
            assertEquals(Character.Digit('1', 2), Byte.GetValueOf("1", 2).GetByteValue());
            assertEquals(Character.Digit('F', 16), Byte.GetValueOf("F", 16).GetByteValue());

            try
            {
                Byte.GetValueOf("0x1", 10);
                fail("Expected FormatException with hex string.");
            }
            catch (FormatException e)
            {
            }

            try
            {
                Byte.GetValueOf("9.2", 10);
                fail("Expected FormatException with floating point string.");
            }
            catch (FormatException e)
            {
            }

            try
            {
                Byte.GetValueOf("", 10);
                fail("Expected FormatException with empty string.");
            }
            catch (FormatException e)
            {
            }

            //try
            //{
            //    Byte.GetValueOf(null, 10);
            //    fail("Expected FormatException with null string.");
            //}
            //catch (FormatException e)
            //{
            //}

            // J2N: Match .NET behavior and return 0 for a null string
            assertEquals(new Byte(0), Byte.GetValueOf(null, 10));
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

        // J2N: Moved to CharSequences
        ///**
        // * @tests java.lang.Byte#parseByte(String,int)
        // */
        //[Test]
        //public void Test_parseByteLjava_lang_StringI()
        //{
        //    assertEquals(0, Byte.Parse("0", 10));
        //    assertEquals(1, Byte.Parse("1", 10));
        //    assertEquals(-1, (sbyte)Byte.Parse("-1", 10)); // J2N: Parse allowed, but cast required to change the result to negative

        //    //must be consistent with Character.digit()
        //    assertEquals(Character.Digit('1', 2), Byte.Parse("1", 2));
        //    assertEquals(Character.Digit('F', 16), Byte.Parse("F", 16));

        //    try
        //    {
        //        Byte.Parse("0x1", 10);
        //        fail("Expected FormatException with hex string.");
        //    }
        //    catch (FormatException e)
        //    {
        //    }

        //    try
        //    {
        //        Byte.Parse("9.2", 10);
        //        fail("Expected FormatException with floating point string.");
        //    }
        //    catch (FormatException e)
        //    {
        //    }

        //    try
        //    {
        //        Byte.Parse("", 10);
        //        fail("Expected FormatException with empty string.");
        //    }
        //    catch (FormatException e)
        //    {
        //    }

        //    //try
        //    //{
        //    //    Byte.Parse(null, 10);
        //    //    fail("Expected FormatException with null string.");
        //    //}
        //    //catch (FormatException e)
        //    //{
        //    //}

        //    // J2N: Match .NET behavior and return 0 for a null string
        //    assertEquals(0, Byte.Parse(null, 10));
        //}

        // J2N: Moved to CharSequences
        ///**
        // * @tests java.lang.Byte#decode(String)
        // */
        //[Test]
        //public void Test_decodeLjava_lang_String()
        //{
        //    assertEquals(new Byte((byte)0), Byte.Decode("0"));
        //    assertEquals(new Byte((byte)1), Byte.Decode("1"));
        //    assertEquals(new Byte(unchecked((byte)-1)), Byte.Decode("-1"));
        //    assertEquals(new Byte((byte)0xF), Byte.Decode("0xF"));
        //    assertEquals(new Byte((byte)0xF), Byte.Decode("#F"));
        //    assertEquals(new Byte((byte)0xF), Byte.Decode("0XF"));
        //    assertEquals(new Byte((byte)07), Byte.Decode("07"));

        //    try
        //    {
        //        Byte.Decode("9.2");
        //        fail("Expected FormatException with floating point string.");
        //    }
        //    catch (FormatException e)
        //    {
        //    }

        //    try
        //    {
        //        Byte.Decode("");
        //        fail("Expected FormatException with empty string.");
        //    }
        //    catch (FormatException e)
        //    {
        //    }

        //    try
        //    {
        //        Byte.Decode(null);
        //        //undocumented NPE, but seems consistent across JREs
        //        fail("Expected NullPointerException with null string.");
        //    }
        //    catch (ArgumentNullException e)
        //    {
        //    }
        //}

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

        // J2N: Removed this overload because all of the constructors are deprecated in JDK 16
        ///**
        // * @tests java.lang.Byte#Byte(java.lang.String)
        // */
        //[Test]
        //public void Test_ConstructorLjava_lang_String2()
        //{
        //    // Test for method java.lang.Byte(java.lang.String)

        //    Byte b = new Byte("127", J2N.Text.StringFormatter.InvariantCulture);
        //    Byte nb = new Byte("-128", J2N.Text.StringFormatter.InvariantCulture);
        //    assertTrue("Incorrect Byte Object created", b.GetByteValue() == (byte)127
        //            && (nb.GetByteValue() == unchecked((byte)-128)));

        //}

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

        // J2N: Moved to CharSequences
        ///**
        // * @tests java.lang.Byte#decode(java.lang.String)
        // */
        //[Test]
        //public void Test_decodeLjava_lang_String2()
        //{
        //    // Test for method java.lang.Byte
        //    // java.lang.Byte.decode(java.lang.String)
        //    assertTrue("String decoded incorrectly, wanted: 1 got: " + Byte.Decode("1").ToString(),
        //            Byte.Decode("1").Equals(new Byte((byte)1)));
        //    assertTrue("String decoded incorrectly, wanted: -1 got: "
        //            + Byte.Decode("-1").ToString(), Byte.Decode("-1").Equals(new Byte(unchecked((byte)-1))));
        //    assertTrue("String decoded incorrectly, wanted: 127 got: "
        //            + Byte.Decode("127").ToString(), Byte.Decode("127")
        //            .Equals(new Byte((byte)127)));
        //    assertTrue("String decoded incorrectly, wanted: -128 got: "
        //            + Byte.Decode("-128").ToString(), Byte.Decode("-128").Equals(
        //            new Byte(unchecked((byte)-128))));
        //    assertTrue("String decoded incorrectly, wanted: 127 got: "
        //            + Byte.Decode("0x7f").ToString(), Byte.Decode("0x7f").Equals(
        //            new Byte((byte)127)));
        //    assertTrue("String decoded incorrectly, wanted: -128 got: "
        //            + Byte.Decode("-0x80").ToString(), Byte.Decode("-0x80").Equals(
        //            new Byte(unchecked((byte)-128))));

        //    bool exception = false;
        //    try
        //    {
        //        //Byte.Decode("128");
        //        Byte.Decode("256"); // J2N: We allow parsing from sbyte.MinValue to byte.MaxValue for compatibility
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
        //        Byte.Decode("-129");
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
        //        //Byte.Decode("0x80");
        //        Byte.Decode("0x0100"); // J2N: We allow parsing from sbyte.MinValue to byte.MaxValue for compatibility
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
        //        Byte.Decode("-0x81");
        //    }
        //    catch (OverflowException e) // J2N: .NET throws OverflowException rather than FormatException in this case
        //    {
        //        // Correct
        //        exception = true;
        //    }
        //    assertTrue("Failed to throw exception for hex MIN_VALUE - 1", exception);
        //}

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

        // J2N: Moved to CharSequences
        ///**
        // * @tests java.lang.Byte#parseByte(java.lang.String, int)
        // */
        //[Test]
        //public void Test_parseByteLjava_lang_StringI2()
        //{
        //    // Test for method byte java.lang.Byte.parseByte(java.lang.String, int)
        //    byte b = Byte.Parse("127", 10);
        //    byte bn = Byte.Parse("-128", 10);
        //    assertTrue("Invalid parse of dec byte", b == (byte)127 && (bn == unchecked((byte)-128)));
        //    assertEquals("Failed to parse hex value", 10, Byte.Parse("A", 16));
        //    assertEquals("Returned incorrect value for 0 hex", 0, Byte.Parse("0", 16));
        //    assertTrue("Returned incorrect value for most negative value hex", Byte.Parse(
        //            "-80", 16) == (byte)0x80);
        //    assertTrue("Returned incorrect value for most positive value hex", Byte.Parse("7f",
        //            16) == 0x7f);
        //    assertEquals("Returned incorrect value for 0 decimal", 0, Byte.Parse("0", 10));
        //    assertTrue("Returned incorrect value for most negative value decimal", Byte.Parse(
        //            "-128", 10) == (byte)0x80);
        //    assertTrue("Returned incorrect value for most positive value decimal", Byte.Parse(
        //            "127", 10) == 0x7f);

        //    try
        //    {
        //        Byte.Parse("-1000", 10);
        //        fail("Failed to throw exception");
        //    }
        //    catch (OverflowException e) // J2N: .NET throws OverflowException rather than FormatException in this case
        //    {
        //    }

        //    try
        //    {
        //        //Byte.ParseByte("128", 10);
        //        Byte.Parse("256", 10); // J2N: We allow parsing from sbyte.MinValue to byte.MaxValue for compatibility
        //        fail("Failed to throw exception for MAX_VALUE + 1");
        //    }
        //    catch (OverflowException e) // J2N: .NET throws OverflowException rather than FormatException in this case
        //    {
        //    }

        //    try
        //    {
        //        Byte.Parse("-129", 10);
        //        fail("Failed to throw exception for MIN_VALUE - 1");
        //    }
        //    catch (OverflowException e) // J2N: .NET throws OverflowException rather than FormatException in this case
        //    {
        //    }

        //    try
        //    {
        //        //Byte.ParseByte("80", 16);
        //        Byte.Parse("100", 16); // J2N: We allow parsing from sbyte.MinValue to byte.MaxValue for compatibility
        //        fail("Failed to throw exception for hex MAX_VALUE + 1");
        //    }
        //    catch (OverflowException e) // J2N: .NET throws OverflowException rather than FormatException in this case
        //    {
        //    }

        //    assertEquals(1, Byte.Parse("1", 16));
        //    assertEquals(-1, (sbyte)Byte.Parse("-1", 16));

        //    assertEquals(sbyte.MaxValue, Byte.Parse("7f", 16));
        //    assertEquals(-128, (sbyte)Byte.Parse("-80", 16)); // Special case: In Java, we allow the negative sign for the smallest negative number
        //    assertEquals(-128, (sbyte)Byte.Parse("80", 16));  // In .NET, it should parse without the negative sign to the same value (in .NET the negative sign is not allowed)
        //    assertEquals(-127, (sbyte)Byte.Parse("81", 16));

        //    try
        //    {
        //        Byte.Parse("-81", 16);
        //        fail("Failed to throw exception for hex MIN_VALUE + 1");
        //    }
        //    catch (OverflowException e) // J2N: .NET throws OverflowException rather than FormatException in this case
        //    {
        //    }
        //}

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
            assertEquals("Returned incorrect byte", 0, Byte.GetValueOf("0", J2N.Text.StringFormatter.InvariantCulture).GetByteValue());
            assertEquals("Returned incorrect byte", 127, Byte.GetValueOf("127", J2N.Text.StringFormatter.InvariantCulture).GetByteValue());
            assertEquals("Returned incorrect byte", -127, (sbyte)Byte.GetValueOf("-127", J2N.Text.StringFormatter.InvariantCulture).GetByteValue()); // J2N: Parse allowed, but cast required to change the result to negative
            assertEquals("Returned incorrect byte", -128, (sbyte)Byte.GetValueOf("-128", J2N.Text.StringFormatter.InvariantCulture).GetByteValue()); // J2N: Parse allowed, but cast required to change the result to negative

            try
            {
                //Byte.GetValueOf("128");
                Byte.GetValueOf("256", J2N.Text.StringFormatter.InvariantCulture); // J2N: We allow parsing from sbyte.MinValue to byte.MaxValue for compatibility
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
            assertEquals("Returned incorrect byte", 10, Byte.GetValueOf("A", 16).GetByteValue());
            assertEquals("Returned incorrect byte", 127, Byte.GetValueOf("127", 10).GetByteValue());
            assertEquals("Returned incorrect byte", -127, (sbyte)Byte.GetValueOf("-127", 10).GetByteValue()); // J2N: Parse allowed, but cast required to change the result to negative
            assertEquals("Returned incorrect byte", -128, (sbyte)Byte.GetValueOf("-128", 10).GetByteValue()); // J2N: Parse allowed, but cast required to change the result to negative
            assertEquals("Returned incorrect byte", 127, Byte.GetValueOf("7f", 16).GetByteValue());
            assertEquals("Returned incorrect byte", -127, (sbyte)Byte.GetValueOf("-7f", 16).GetByteValue()); // J2N: Parse allowed, but cast required to change the result to negative
            assertEquals("Returned incorrect byte", -128, (sbyte)Byte.GetValueOf("-80", 16).GetByteValue()); // J2N: Parse allowed, but cast required to change the result to negative

            try
            {
                //Byte.GetValueOf("128", 10);
                Byte.GetValueOf("256", 10); // J2N: We allow parsing from sbyte.MinValue to byte.MaxValue for compatibility
                fail("Failed to throw exception when passes value > byte");
            }
            catch (OverflowException e) // J2N: .NET throws OverflowException rather than FormatException in this case
            {
            }
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

                        yield return new TestCaseData((byte)+100, "+100", 10);
                        yield return new TestCaseData(unchecked((byte)-100), "-100", 10);

                        yield return new TestCaseData((byte)0, "+0", 10);
                        yield return new TestCaseData((byte)0, "-0", 10);
                        yield return new TestCaseData((byte)0, "+00000", 10);
                        yield return new TestCaseData((byte)0, "-00000", 10);

                        yield return new TestCaseData((byte)0, "0", 10);
                        yield return new TestCaseData((byte)1, "1", 10);
                        yield return new TestCaseData((byte)9, "9", 10);

                        // Harmony (Test_parseByteLjava_lang_String())

                        yield return new TestCaseData((byte)0, "0", 10);
                        yield return new TestCaseData((byte)1, "1", 10);
                        yield return new TestCaseData(unchecked((byte)-1), "-1", 10);

                        // Harmony (Test_parseByteLjava_lang_String2())

                        yield return new TestCaseData((byte)127, "127", 10);
                        yield return new TestCaseData(unchecked((byte)-128), "-128", 10);
                        yield return new TestCaseData((byte)0, "0", 10);
                        yield return new TestCaseData((byte)0x80, "-128", 10);
                        yield return new TestCaseData((byte)0x7F, "127", 10);

                        // Harmony (Test_parseByteLjava_lang_StringI())

                        yield return new TestCaseData((byte)0, "0", 10);
                        yield return new TestCaseData((byte)1, "1", 10);
                        yield return new TestCaseData(unchecked((byte)-1), "-1", 10);

                        //must be consistent with Character.digit()
                        yield return new TestCaseData((byte)Character.Digit('1', 2), "1", 2);
                        yield return new TestCaseData((byte)Character.Digit('F', 16), "F", 16);

                        yield return new TestCaseData((byte)0, null, 10); // J2N: Match .NET behavior where null will result in 0

                        // Harmony (Test_parseByteLjava_lang_StringI2())

                        yield return new TestCaseData((byte)127, "127", 10);
                        yield return new TestCaseData(unchecked((byte)-128), "-128", 10);
                        yield return new TestCaseData((byte)10, "A", 16);
                        yield return new TestCaseData((byte)0, "0", 16);
                        yield return new TestCaseData((byte)0x80, "-80", 16);
                        yield return new TestCaseData((byte)0x7f, "7f", 16);
                        yield return new TestCaseData((byte)0, "0", 10);
                        yield return new TestCaseData((byte)0x80, "-128", 10);
                        yield return new TestCaseData((byte)0x7f, "127", 10);

                        // .NET 5

                        string[] testValues = { null, null, null, null, "10", "100", "1011", "ff", "0xff", "77", "11", "11111111" };
                        int[] testBases = { 10, 2, 8, 16, 10, 10, 2, 16, 16, 8, 2, 2 };
                        byte[] expectedValues = { 0, 0, 0, 0, 10, 100, 11, 255, 255, 63, 3, 255 };
                        for (int i = 0; i < testValues.Length; i++)
                        {
                            yield return new TestCaseData(expectedValues[i], testValues[i], testBases[i]);
                        }

                        // Custom

                        yield return new TestCaseData((byte)1, "1", 16);
                        yield return new TestCaseData(unchecked((byte)-1), "-1", 16);
                        yield return new TestCaseData((byte)sbyte.MaxValue, "7f", 16);
                        yield return new TestCaseData(unchecked((byte)-128), "-80", 16); // Special case: In Java, we allow the negative sign for the smallest negative number
                        yield return new TestCaseData(unchecked((byte)-128), "80", 16);  // In .NET, it should parse without the negative sign to the same value (in .NET the negative sign is not allowed)
                        yield return new TestCaseData(unchecked((byte)-127), "81", 16);

                        // Surrogate Pairs (.NET only supports ASCII, but Java supports these)

                        yield return new TestCaseData((byte)99, "𑃹𝟫", 10);
                        yield return new TestCaseData((byte)53, "𝟓𝟑", 10);
                        yield return new TestCaseData((byte)49, "𑁪𑁯", 10);

                        // Non-decimal needs to be tested separately because they go through
                        // a separate execution path
                        yield return new TestCaseData((byte)153, "𑃹𝟫", 16);
                        yield return new TestCaseData((byte)83, "𝟓𝟑", 16);
                        yield return new TestCaseData((byte)73, "𑁪𑁯", 16);
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

                        // Harmony (Test_parseByteLjava_lang_String())

                        yield return new TestCaseData(typeof(FormatException), "0x1", 10);
                        yield return new TestCaseData(typeof(FormatException), "9.2", 10);
                        yield return new TestCaseData(typeof(FormatException), "", 10); // J2N: .NET throws ArgumentOutOfRangeException rather than FormatException in this case, but since it is inconsistent with long.Parse() we are going with FormatException.
                        //yield return new TestCaseData(typeof(ArgumentNullException), null, 10); // J2N: Match .NET behavior where null will result in 0

                        // Harmony (Test_parseByteLjava_lang_String2())

                        yield return new TestCaseData(typeof(OverflowException), "-1000", 10);
                        yield return new TestCaseData(typeof(OverflowException), /*"128"*/ "256", 10); // J2N: We allow parsing from sbyte.MinValue to byte.MaxValue for compatibility
                        yield return new TestCaseData(typeof(OverflowException), "-129", 10);

                        // Harmony (Test_parseByteLjava_lang_StringI())

                        yield return new TestCaseData(typeof(FormatException), "0x1", 10);
                        yield return new TestCaseData(typeof(FormatException), "9.2", 10);
                        yield return new TestCaseData(typeof(FormatException), "", 10);
                        //yield return new TestCaseData(typeof(ArgumentNullException), null, 10); // J2N: Match .NET behavior where null will result in 0

                        // Harmony (Test_parseByteLjava_lang_StringI2())

                        yield return new TestCaseData(typeof(OverflowException), "-1000", 10);
                        yield return new TestCaseData(typeof(OverflowException), /*"128"*/ "256", 10); // J2N: We allow parsing from sbyte.MinValue to byte.MaxValue for compatibility
                        yield return new TestCaseData(typeof(OverflowException), "-129", 10);
                        yield return new TestCaseData(typeof(OverflowException), /*"80"*/ "100", 16); // J2N: We allow parsing from sbyte.MinValue to byte.MaxValue for compatibility
                        yield return new TestCaseData(typeof(OverflowException), "-81", 16);

                        // .NET 5

                        string[] overflowValues = { "256", "111111111", "ffffe", "7777777" /*, "-1"*/ }; // J2N allows from sbyte.MinValue to byte.MaxValue for compatibility with Java
                        int[] overflowBases = { 10, 2, 16, 8 /*, 10*/ };
                        for (int i = 0; i < overflowValues.Length; i++)
                        {
                            yield return new TestCaseData(typeof(OverflowException), overflowValues[i], overflowBases[i]);
                        }

                        string[] formatExceptionValues = { "fffg", "0xxfff", "8", "112", "!56" };
                        int[] formatExceptionBases = { 16, 16, 8, 2, 10 };
                        for (int i = 0; i < formatExceptionValues.Length; i++)
                        {
                            yield return new TestCaseData(typeof(FormatException), formatExceptionValues[i], formatExceptionBases[i]);
                        }
                    }
                }

                public static IEnumerable<TestCaseData> TestParse_CharSequence_Int32_Int32_Int32_Data
                {
                    get
                    {
                        // JDK 8

                        yield return new TestCaseData((byte)0, "test-00000", 4, 10 - 4, 10);
                        yield return new TestCaseData(unchecked((byte)-123), "test-123", 4, 4, 10);
                        yield return new TestCaseData((byte)123, "xx123yy", 2, 3, 10);
                        yield return new TestCaseData((byte)15, "xxFyy", 2, 3 - 2, 16);

                        yield return new TestCaseData((byte)123, "xx12345yy", 2, 3, 10);

                        // Harmony (Test_parseByteLjava_lang_String())

                        yield return new TestCaseData((byte)0, "0", 0, 1, 10);
                        yield return new TestCaseData((byte)1, "1", 0, 1, 10);
                        yield return new TestCaseData(unchecked((byte)-1), "-1", 0, 2, 10);

                        // Harmony (Test_parseByteLjava_lang_String2())

                        yield return new TestCaseData((byte)127, "127", 0, 3, 10);
                        yield return new TestCaseData(unchecked((byte)-128), "-128", 0, 4, 10);
                        yield return new TestCaseData((byte)0, "0", 0, 1, 10);
                        yield return new TestCaseData((byte)0x80, "-128", 0, 4, 10);
                        yield return new TestCaseData((byte)0x7F, "127", 0, 3, 10);

                        // Harmony (Test_parseByteLjava_lang_StringI())

                        yield return new TestCaseData((byte)0, "0", 0, 1, 10);
                        yield return new TestCaseData((byte)1, "1", 0, 1, 10);
                        yield return new TestCaseData(unchecked((byte)-1), "-1", 0, 2, 10);

                        //must be consistent with Character.digit()
                        yield return new TestCaseData((byte)Character.Digit('1', 2), "1", 0, 1, 2);
                        yield return new TestCaseData((byte)Character.Digit('F', 16), "F", 0, 1, 16);

                        //yield return new TestCaseData((byte)0, null, 0, 1, 10); // Unlike non-indexed overload, throw instead of returning zero

                        // Harmony (Test_parseByteLjava_lang_StringI2())

                        yield return new TestCaseData((byte)127, "127", 0, 3, 10);
                        yield return new TestCaseData(unchecked((byte)-128), "-128", 0, 4, 10);
                        yield return new TestCaseData((byte)10, "A", 0, 1, 16);
                        yield return new TestCaseData((byte)0, "0", 0, 1, 16);
                        yield return new TestCaseData((byte)0x80, "-80", 0, 3, 16);
                        yield return new TestCaseData((byte)0x7f, "7f", 0, 2, 16);
                        yield return new TestCaseData((byte)0, "0", 0, 1, 10);
                        yield return new TestCaseData((byte)0x80, "-128", 0, 4, 10);
                        yield return new TestCaseData((byte)0x7f, "127", 0, 3, 10);

                        // .NET 5

                        string[] testValues = { /*null, null, null, null,*/ "10", "100", "1011", "ff", "0xff", "77", "11", "11111111" };
                        int[] testBases = { /*10, 2, 8, 16,*/ 10, 10, 2, 16, 16, 8, 2, 2 };
                        byte[] expectedValues = { /*0, 0, 0, 0,*/ 10, 100, 11, 255, 255, 63, 3, 255 };
                        for (int i = 0; i < testValues.Length; i++)
                        {
                            yield return new TestCaseData(expectedValues[i], testValues[i], 0, testValues[i].Length, testBases[i]);
                        }

                        // Custom

                        yield return new TestCaseData((byte)1, "1", 0, 1, 16);
                        yield return new TestCaseData(unchecked((byte)-1), "-1", 0, 2, 16);
                        yield return new TestCaseData((byte)sbyte.MaxValue, "7f", 0, 2, 16);
                        yield return new TestCaseData(unchecked((byte)-128), "-80", 0, 3, 16); // Special case: In Java, we allow the negative sign for the smallest negative number
                        yield return new TestCaseData(unchecked((byte)-128), "80", 0, 2, 16);  // In .NET, it should parse without the negative sign to the same value (in .NET the negative sign is not allowed)
                        yield return new TestCaseData(unchecked((byte)-127), "81", 0, 2, 16);

                        // Surrogate Pairs (.NET only supports ASCII, but Java supports these)

                        yield return new TestCaseData((byte)99, "𑃹𝟫", 0, 4, 10);
                        yield return new TestCaseData((byte)53, "𝟓𝟑", 0, 4, 10);
                        yield return new TestCaseData((byte)49, "𑁪𑁯", 0, 4, 10);

                        // Non-decimal needs to be tested separately because they go through
                        // a separate execution path
                        yield return new TestCaseData((byte)153, "𑃹𝟫", 0, 4, 16);
                        yield return new TestCaseData((byte)83, "𝟓𝟑", 0, 4, 16);
                        yield return new TestCaseData((byte)73, "𑁪𑁯", 0, 4, 16);
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

                        // Harmony (Test_parseByteLjava_lang_String())

                        yield return new TestCaseData(typeof(FormatException), "0x1", 0, 3, 10);
                        yield return new TestCaseData(typeof(FormatException), "9.2", 0, 3, 10);
                        yield return new TestCaseData(typeof(FormatException), "", 0, 0, 10); // J2N: .NET throws ArgumentOutOfRangeException rather than FormatException in this case, but since it is inconsistent with long.Parse() we are going with FormatException.
                        yield return new TestCaseData(typeof(ArgumentNullException), null, 0, 1, 10); // Unlike non-indexed overload, throw instead of returning zero

                        // Harmony (Test_parseByteLjava_lang_String2())

                        yield return new TestCaseData(typeof(OverflowException), "-1000", 0, 5, 10);
                        yield return new TestCaseData(typeof(OverflowException), /*"128"*/ "256", 0, 3, 10); // J2N: We allow parsing from sbyte.MinValue to byte.MaxValue for compatibility
                        yield return new TestCaseData(typeof(OverflowException), "-129", 0, 4, 10);

                        // Harmony (Test_parseByteLjava_lang_StringI())

                        yield return new TestCaseData(typeof(FormatException), "0x1", 0, 3, 10);
                        yield return new TestCaseData(typeof(FormatException), "9.2", 0, 3, 10);
                        yield return new TestCaseData(typeof(FormatException), "", 0, 0, 10);
                        yield return new TestCaseData(typeof(ArgumentNullException), null, 0, 1, 10); // Unlike non-indexed overload, throw instead of returning zero

                        // Harmony (Test_parseByteLjava_lang_StringI2())

                        yield return new TestCaseData(typeof(OverflowException), "-1000", 0, 5, 10);
                        yield return new TestCaseData(typeof(OverflowException), /*"128"*/ "256", 0, 3, 10); // J2N: We allow parsing from sbyte.MinValue to byte.MaxValue for compatibility
                        yield return new TestCaseData(typeof(OverflowException), "-129", 0, 4, 10);
                        yield return new TestCaseData(typeof(OverflowException), /*"80"*/ "100", 0, 3, 16); // J2N: We allow parsing from sbyte.MinValue to byte.MaxValue for compatibility
                        yield return new TestCaseData(typeof(OverflowException), "-81", 0, 3, 16);

                        // .NET 5

                        string[] overflowValues = { "256", "111111111", "ffffe", "7777777" /*, "-1"*/ }; // J2N allows from sbyte.MinValue to byte.MaxValue for compatibility with Java
                        int[] overflowBases = { 10, 2, 16, 8 /*, 10*/ };
                        for (int i = 0; i < overflowValues.Length; i++)
                        {
                            yield return new TestCaseData(typeof(OverflowException), overflowValues[i], 0, overflowValues[i].Length, overflowBases[i]);
                        }

                        string[] formatExceptionValues = { "fffg", "0xxfff", "8", "112", "!56" };
                        int[] formatExceptionBases = { 16, 16, 8, 2, 10 };
                        for (int i = 0; i < formatExceptionValues.Length; i++)
                        {
                            yield return new TestCaseData(typeof(FormatException), formatExceptionValues[i], 0, formatExceptionValues[i].Length, formatExceptionBases[i]);
                        }

                        // Custom

                        yield return new TestCaseData(typeof(FormatException), "xx  34567yy", 2, 5, 10); // spaces in range are not allowed
                    }
                }
            }

            #endregion ParseTestCase

            // Radix-based parsing

            #region Parse_CharSequence_Int32

            public abstract class Parse_CharSequence_Int32_TestCase : ParseTestCase
            {
                protected abstract byte GetResult(string value, int radix);

                [TestCaseSource("TestParse_CharSequence_Int32_Data")]
                public virtual void TestParse_CharSequence_Int32(byte expected, string value, int radix)
                {
                    var actual = GetResult(value, radix);
                    assertEquals($"Byte.Parse(string, out byte) failed. String: \"{value}\" Result: {actual}", expected, actual);
                }

                [TestCaseSource("TestParse_CharSequence_Int32_ForException_Data")]
                public virtual void TestParse_CharSequence_Int32_ForException(Type expectedExceptionType, string value, int radix)
                {
                    Assert.Throws(expectedExceptionType, () => GetResult(value, radix));
                }
            }

            public class Parse_String_Int32 : Parse_CharSequence_Int32_TestCase
            {
                protected override byte GetResult(string value, int radix)
                {
                    return Byte.Parse(value, radix);
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

                protected abstract byte GetResult(string value, int startIndex, int length, int radix);


                [TestCaseSource("TestParse_CharSequence_Int32_Int32_Int32_Data")]

                public virtual void TestParse_CharSequence_Int32_Int32_Int32(byte expected, string value, int startIndex, int length, int radix)
                {
                    Assume.That(IsNullableType || (!IsNullableType && value != null), "null is not supported by this character sequence type.");

                    var actual = GetResult(value, startIndex, length, radix);
                    assertEquals($"Byte.Parse(string, int, int, int) failed. Expected: {expected} String: \"{value}\", startIndex: {startIndex}, length: {length} radix: {radix} Result: {actual}", expected, actual);
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
                protected override byte GetResult(string value, int startIndex, int length, int radix)
                {
                    return Byte.Parse(value, startIndex, length, radix);
                }
            }

            public class Parse_CharArray_Int32_Int32_Int32_Int32 : Parse_CharSequence_Int32_Int32_Int32_TestCase
            {
                protected override byte GetResult(string value, int startIndex, int length, int radix)
                {
                    return Byte.Parse(value is null ? null : value.ToCharArray(), startIndex, length, radix);
                }
            }

            public class Parse_StringBuilder_Int32_Int32_Int32_Int32 : Parse_CharSequence_Int32_Int32_Int32_TestCase
            {
                protected override byte GetResult(string value, int startIndex, int length, int radix)
                {
                    // NOTE: Although technically the .NET StringBuilder will accept null and convert it to an empty string, we
                    // want to test the method for null here.
                    return Byte.Parse(value is null ? null : new StringBuilder(value), startIndex, length, radix);
                }
            }

            public class Parse_ICharSequence_Int32_Int32_Int32_Int32 : Parse_CharSequence_Int32_Int32_Int32_TestCase
            {
                protected override byte GetResult(string value, int startIndex, int length, int radix)
                {
                    return Byte.Parse(value.AsCharSequence(), startIndex, length, radix);
                }
            }

#if FEATURE_READONLYSPAN
            public class Parse_ReadOnlySpan_Int32_Int32_Int32_Int32 : Parse_CharSequence_Int32_Int32_Int32_TestCase
            {
                protected override bool IsNullableType => false;

                protected override byte GetResult(string value, int startIndex, int length, int radix)
                {
                    return Byte.Parse(value.AsSpan(), startIndex, length, radix);
                }
            }
#endif

            #endregion Parse_CharSequence_Int32_Int32_Int32

            #region TryParse_CharSequence_Int32

            public abstract class TryParse_CharSequence_Int32_TestCase : ParseTestCase
            {
                protected abstract bool GetResult(string value, int radix, out byte result);

                [TestCaseSource("TestParse_CharSequence_Int32_Data")]
                public virtual void TestParse_CharSequence_Int32(byte expected, string value, int radix)
                {
                    assertTrue(GetResult(value, radix, out byte actual));
                    assertEquals($"Byte.TryParse(string, out byte) failed. String: \"{value}\" Result: {actual}", expected, actual);
                }

                [TestCaseSource("TestParse_CharSequence_Int32_ForException_Data")]
                public virtual void TestParse_CharSequence_Int32_ForException(Type expectedExceptionType, string value, int radix)
                {
                    assertFalse(GetResult(value, radix, out byte actual));
                    assertEquals(0, actual);
                }
            }

            public class TryParse_String_Int32 : TryParse_CharSequence_Int32_TestCase
            {
                protected override bool GetResult(string value, int radix, out byte result)
                {
                    return Byte.TryParse(value, radix, out result);
                }
            }

            // J2N: ReadOnlySpan<char> not supported at this time on this overload (not supported in .NET anyway)
            //#if FEATURE_READONLYSPAN
            //            public class TryParse_ReadOnlySpan_Int32 : TryParse_CharSequence_Int32_TestCase
            //            {
            //                protected override bool GetResult(string s, int radix, out byte result)
            //                {
            //                    return Byte.TryParse(s.AsSpan(), radix, out result);
            //                }
            //            }
            //#endif

            #endregion Parse_CharSequence_Int32

            #region TryParse_CharSequence_Int32_Int32_Int32

            public abstract class TryParse_CharSequence_Int32_Int32_Int32_TestCase : ParseTestCase
            {
                protected virtual bool IsNullableType => true;

                protected abstract bool GetResult(string value, int startIndex, int length, int radix, out byte result);


                [TestCaseSource("TestParse_CharSequence_Int32_Int32_Int32_Data")]

                public virtual void TestParse_CharSequence_Int32_Int32_Int32(byte expected, string value, int startIndex, int length, int radix)
                {
                    Assume.That(IsNullableType || (!IsNullableType && value != null), "null is not supported by this character sequence type.");

                    assertTrue(GetResult(value, startIndex, length, radix, out byte actual));
                    assertEquals($"Int32.Parse(string, int, int, int) failed. Expected: {expected} String: \"{value}\", startIndex: {startIndex}, length: {length} radix: {radix} Result: {actual}", expected, actual);
                }

                [TestCaseSource("TestParse_CharSequence_Int32_Int32_Int32_ForException_Data")]
                public virtual void TestParse_CharSequence_Int32_Int32_Int32_ForException(Type expectedExceptionType, string value, int startIndex, int length, int radix)
                {
                    Assume.That(IsNullableType || (!IsNullableType && value != null), "null is not supported by this character sequence type.");

                    assertFalse(GetResult(value, startIndex, length, radix, out byte actual));
                    assertEquals(0, actual);
                }
            }

            public class TryParse_String_Int32_Int32_Int32_Int32 : TryParse_CharSequence_Int32_Int32_Int32_TestCase
            {
                protected override bool GetResult(string value, int startIndex, int length, int radix, out byte result)
                {
                    return Byte.TryParse(value, startIndex, length, radix, out result);
                }
            }

            public class TryParse_CharArray_Int32_Int32_Int32_Int32 : TryParse_CharSequence_Int32_Int32_Int32_TestCase
            {
                protected override bool GetResult(string value, int startIndex, int length, int radix, out byte result)
                {
                    return Byte.TryParse(value is null ? null : value.ToCharArray(), startIndex, length, radix, out result);
                }
            }

            public class TryParse_StringBuilder_Int32_Int32_Int32_Int32 : TryParse_CharSequence_Int32_Int32_Int32_TestCase
            {
                protected override bool GetResult(string value, int startIndex, int length, int radix, out byte result)
                {
                    // NOTE: Although technically the .NET StringBuilder will accept null and convert it to an empty string, we
                    // want to test the method for null here.
                    return Byte.TryParse(value is null ? null : new StringBuilder(value), startIndex, length, radix, out result);
                }
            }

            public class TryParse_ICharSequence_Int32_Int32_Int32_Int32 : TryParse_CharSequence_Int32_Int32_Int32_TestCase
            {
                protected override bool GetResult(string value, int startIndex, int length, int radix, out byte result)
                {
                    return Byte.TryParse(value.AsCharSequence(), startIndex, length, radix, out result);
                }
            }

#if FEATURE_READONLYSPAN
            public class TryParse_ReadOnlySpan_Int32_Int32_Int32_Int32 : TryParse_CharSequence_Int32_Int32_Int32_TestCase
            {
                protected override bool IsNullableType => false;

                protected override bool GetResult(string value, int startIndex, int length, int radix, out byte result)
                {
                    return Byte.TryParse(value.AsSpan(), startIndex, length, radix, out result);
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

                        yield return new TestCaseData("" + byte.MinValue, byte.MinValue);
                        yield return new TestCaseData("" + byte.MaxValue, byte.MaxValue);

                        yield return new TestCaseData("10", (byte)10);
                        yield return new TestCaseData("0x10", (byte)16);
                        yield return new TestCaseData("0X10", (byte)16);
                        yield return new TestCaseData("010", (byte)8);
                        yield return new TestCaseData("#10", (byte)16);

                        yield return new TestCaseData("+10", (byte)10);
                        yield return new TestCaseData("+0x10", (byte)16);
                        yield return new TestCaseData("+0X10", (byte)16);
                        yield return new TestCaseData("+010", (byte)8);
                        yield return new TestCaseData("+#10", (byte)16);

                        yield return new TestCaseData("-10", unchecked((byte)-10));
                        yield return new TestCaseData("-0x10", unchecked((byte)-16));
                        yield return new TestCaseData("-0X10", unchecked((byte)-16));
                        yield return new TestCaseData("-010", unchecked((byte)-8));
                        yield return new TestCaseData("-#10", unchecked((byte)-16));

                        yield return new TestCaseData(Convert.ToString((int)byte.MinValue, 10), byte.MinValue);
                        yield return new TestCaseData(Convert.ToString((int)byte.MaxValue, 10), byte.MaxValue);

                        // Harmony (Test_decodeLjava_lang_String())

                        yield return new TestCaseData("0", (byte)0);
                        yield return new TestCaseData("1", (byte)1);
                        yield return new TestCaseData("-1", unchecked((byte)-1));
                        yield return new TestCaseData("0xF", (byte)0xF);
                        yield return new TestCaseData("#F", (byte)0xF);
                        yield return new TestCaseData("0XF", (byte)0xF);
                        yield return new TestCaseData("07", (byte)07); // J2N: Technically, .NET doesn't recognize octal literals, but this is the same decimal value

                        // Harmony (Test_decodeLjava_lang_String2())

                        yield return new TestCaseData("1", (byte)1);
                        yield return new TestCaseData("-1", unchecked((byte)-1));
                        yield return new TestCaseData("127", (byte)127);
                        yield return new TestCaseData("-128", unchecked((byte)-128));
                        yield return new TestCaseData("0x7f", (byte)127);
                        yield return new TestCaseData("-0x80", unchecked((byte)-128));
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

                        yield return new TestCaseData(typeof(OverflowException), Convert.ToString((int)sbyte.MinValue - 1, 10), "Out of range"); // J2N: For compatibility, we allow parsing from the sbyte.MinValue
                        yield return new TestCaseData(typeof(OverflowException), Convert.ToString((int)byte.MaxValue + 1, 10), "Out of range");

                        yield return new TestCaseData(typeof(FormatException), "", "Empty String");

                        // Harmony (Test_decodeLjava_lang_String())

                        yield return new TestCaseData(typeof(FormatException), "9.2", "Expected FormatException with floating point string.");
                        yield return new TestCaseData(typeof(FormatException), "", "Expected FormatException with empty string.");
                        //undocumented NPE, but seems consistent across JREs
                        yield return new TestCaseData(typeof(ArgumentNullException), null, "Expected NullPointerException with null string.");

                        // Harmony (Test_decodeLjava_lang_String2())

                        yield return new TestCaseData(typeof(OverflowException), /*"128"*/ "256", "Failed to throw exception for MAX_VALUE + 1"); // J2N: We allow parsing from sbyte.MinValue to byte.MaxValue for compatibility
                        yield return new TestCaseData(typeof(OverflowException), "-129", "Failed to throw exception for MIN_VALUE - 1");
                        yield return new TestCaseData(typeof(OverflowException), /*"0x80"*/ "0x0100", "Failed to throw exception for hex MAX_VALUE + 1"); // J2N: We allow parsing from sbyte.MinValue to byte.MaxValue for compatibility
                        yield return new TestCaseData(typeof(OverflowException), "-0x81", "Failed to throw exception for hex MIN_VALUE - 1");

                        // Custom

                        yield return new TestCaseData(typeof(OverflowException), "0xffff", "Negative not allowed without negative sign"); // -1 - negative values are not allowed per the docs
                    }
                }
            }

            #endregion DecodeTestCase

            #region Decode_CharSequence

            public abstract class Decode_CharSequence_TestCase : DecodeTestCase
            {
                protected abstract Byte GetResult(string value);

                [TestCaseSource("TestDecode_CharSequence_Data")]
                public virtual void TestDecode_CharSequence(string value, byte expected)
                {
                    var actual = GetResult(value);
                    assertEquals($"Byte.Decode(string) failed. String: \"{value}\" Result: {actual}", expected, actual);
                }

                [TestCaseSource("TestDecode_CharSequence_ForException_Data")]
                public virtual void TestDecode_CharSequence_ForException(Type expectedExceptionType, string value, string message)
                {
                    Assert.Throws(expectedExceptionType, () => GetResult(value), message);
                }
            }

            public class Decode_String : Decode_CharSequence_TestCase
            {
                protected override Byte GetResult(string value)
                {
                    return Byte.Decode(value);
                }
            }

            // J2N: ReadOnlySpan<char> not supported at this time on this overload
            //#if FEATURE_READONLYSPAN
            //            public class Decode_ReadOnlySpan : Decode_CharSequence_TestCase
            //            {
            //                protected override Byte GetResult(string s)
            //                {
            //                    return Byte.Decode(s.AsSpan());
            //                }
            //            }
            //#endif

            #endregion Decode_CharSequence

            #region TryDecode_CharSequence

            public abstract class TryDecode_CharSequence_TestCase : DecodeTestCase
            {
                protected abstract bool GetResult(string value, out Byte result);

                [TestCaseSource("TestDecode_CharSequence_Data")]
                public virtual void TestTryDecode_CharSequence(string value, byte expected)
                {
                    assertTrue(GetResult(value, out Byte actual));
                    assertEquals($"Int16.TryDecode(string, out Int16) failed. String: \"{value}\" Result: {actual}", new Byte(expected), actual);
                }

                [TestCaseSource("TestDecode_CharSequence_ForException_Data")]
                public virtual void TestTryDecode_CharSequence_ForException(Type expectedExceptionType, string value, string message)
                {
                    assertFalse(GetResult(value, out Byte actual));
                    assertEquals(null, actual);
                }
            }

            public class TryDecode_String : TryDecode_CharSequence_TestCase
            {
                protected override bool GetResult(string value, out Byte result)
                {
                    return Byte.TryDecode(value, out result);
                }
            }

            // J2N: ReadOnlySpan<char> not supported at this time on this overload
            //#if FEATURE_READONLYSPAN
            //            public class TryDecode_ReadOnlySpan : TryDecode_CharSequence_TestCase
            //            {
            //                protected override bool GetResult(string s, out Byte result)
            //                {
            //                    return Byte.TryDecode(s.AsSpan(), out result);
            //                }
            //            }
            //#endif

            #endregion TryDecode_CharSequence
        }
    }
}
