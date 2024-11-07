using NUnit.Framework;

namespace J2N
{
    public class TestIntegralNumberExtensions : TestCase
    {
        /**
         * @tests java.lang.Integer#toBinaryString(int)
         */
        [Test]
        public void Test_ToBinaryString_Char()
        {
            // Test for method java.lang.String
            // java.lang.Integer.toBinaryString(int)
            assertEquals("Incorrect string returned", "1111111111111111", (char.MaxValue).ToBinaryString());
            assertEquals("Incorrect string returned", "0", (char.MinValue).ToBinaryString());
            assertEquals("Incorrect string returned", "1000001", 'A'.ToBinaryString());
        }

        /**
         * @tests java.lang.Integer#toBinaryString(int)
         */
        [Test]
        public void Test_ToBinaryString_Int16()
        {
            // Test for method java.lang.String
            // java.lang.Integer.toBinaryString(int)
            assertEquals("Incorrect string returned", "111111111111111", (short.MaxValue).ToBinaryString());
            assertEquals("Incorrect string returned", "11111111111111111000000000000000", (short.MinValue).ToBinaryString());
        }

        /**
         * @tests java.lang.Integer#toBinaryString(int)
         */
        [Test]
        public void Test_ToBinaryString_Int32()
        {
            // Test for method java.lang.String
            // java.lang.Integer.toBinaryString(int)
            assertEquals("Incorrect string returned", "1111111111111111111111111111111", (int.MaxValue).ToBinaryString());
            assertEquals("Incorrect string returned", "10000000000000000000000000000000", (int.MinValue).ToBinaryString());
        }



        /**
         * @tests java.lang.Long#toBinaryString(long)
         */
        [Test]
        public void Test_ToBinaryString_Int64()
        {
            // Test for method java.lang.String java.lang.Long.toBinaryString(long)
            assertEquals("Incorrect binary string returned", "11011001010010010000", (890000L).ToBinaryString());
            assertEquals("Incorrect binary string returned",

                                    "1000000000000000000000000000000000000000000000000000000000000000", (long.MinValue).ToBinaryString()
                            );
            assertEquals("Incorrect binary string returned",

                                    "111111111111111111111111111111111111111111111111111111111111111", (long.MaxValue).ToBinaryString()
                            );
        }

        /*
          public void testToHexString_Int32() throws Exception {
              assertEquals("ffffffd6", Integer.toHexString(-42));
              assertEquals("2a", Integer.toHexString(42));
              assertEquals("5f5e0ff", Integer.toHexString(99999999));
          }

          public void testToHexString_Int64() throws Exception {
              assertEquals("ffffffffffffffd6", Long.toHexString(-42L));
              assertEquals("2a", Long.toHexString(42L));
              assertEquals("16345785d89ffff", Long.toHexString(99999999999999999L));
          }

          public void testToHexString_Char() throws Exception {
              assertEquals("ffd6", Integer.toHexString((char)-42));
              assertEquals("2a", Integer.toHexString((char)42));
              assertEquals("270f", Integer.toHexString((char)9999));
          }
        */

        [Test]
        public void TestToHexString_Char()
        {
            assertEquals("ffd6", (unchecked((char)-42)).ToHexString());
            assertEquals("0", ((char)0).ToHexString());
            assertEquals("2a", ((char)42).ToHexString());
            assertEquals("270f", ((char)9999).ToHexString());
        }

        // J2N: Observed behavior in Java is below, but
        // it isn't easy getting .NET to recognize the difference
        // between a negative byte that underflowed and one that is
        // above 127, as each one should have a different format.
        //[Test]
        //public void TestToHexString_Byte()
        //{
        //    assertEquals("ffd6", (unchecked((byte)-42)).ToHexString());
        //    assertEquals("0", ((byte)0).ToHexString());
        //    assertEquals("2a", ((byte)42).ToHexString());
        //    assertEquals("ffffffe9", ((byte)233).ToHexString());
        //}

        [Test]
        public void TestToHexString_Short()
        {
            assertEquals("ffffffd6", ((short)-42).ToHexString());
            assertEquals("0", ((short)0).ToHexString());
            assertEquals("2a", ((short)42).ToHexString());
            assertEquals("270f", ((short)9999).ToHexString());
        }

        [Test]
        public void TestToHexString_Int32()
        {
            assertEquals("ffffffd6", (-42).ToHexString());
            assertEquals("0", (0).ToHexString());
            assertEquals("2a", (42).ToHexString());
            assertEquals("5f5e0ff", (99999999).ToHexString());
        }

        [Test]
        public void TestToHexString_Int64()
        {
            assertEquals("ffffffffffffffd6", (-42L).ToHexString());
            assertEquals("0", (0L).ToHexString());
            assertEquals("2a", (42L).ToHexString());
            assertEquals("16345785d89ffff", (99999999999999999L).ToHexString());
        }

        /**
         * @tests java.lang.Integer#toOctalString(int)
         */
        [Test]
        public void Test_ToOctalString_Char()
        {
            // Test for method java.lang.String java.lang.Integer.toOctalString(int)
            // Spec states that the int arg is treated as unsigned
            assertEquals("Returned incorrect octal string", "177777", (char.MaxValue).ToOctalString());
            assertEquals("Returned incorrect octal string", "0", (char.MinValue).ToOctalString());
            assertEquals("Incorrect string returned", "101", 'A'.ToOctalString());
        }

        /**
         * @tests java.lang.Integer#toOctalString(int)
         */
        [Test]
        public void Test_ToOctalString_Int16()
        {
            // Test for method java.lang.String java.lang.Integer.toOctalString(int)
            // Spec states that the int arg is treated as unsigned
            assertEquals("Returned incorrect octal string", "77777", (short.MaxValue).ToOctalString());
            assertEquals("Returned incorrect octal string", "37777700000", (short.MinValue).ToOctalString());
        }

        /**
         * @tests java.lang.Integer#toOctalString(int)
         */
        [Test]
        public void Test_ToOctalString_Int32()
        {
            // Test for method java.lang.String java.lang.Integer.toOctalString(int)
            // Spec states that the int arg is treated as unsigned
            assertEquals("Returned incorrect octal string", "17777777777", (int.MaxValue).ToOctalString());
            assertEquals("Returned incorrect octal string", "20000000000", (int.MinValue).ToOctalString());
        }


        /**
         * @tests java.lang.Long#toOctalString(long)
         */
        [Test]
        public void Test_ToOctalString_Int64()
        {
            // Test for method java.lang.String java.lang.Long.toOctalString(long)
            assertEquals("Returned incorrect oct string", "77777777777", (8589934591L).ToOctalString());
            assertEquals("Returned incorrect oct string", "1000000000000000000000", (long.MinValue).ToOctalString());
            assertEquals("Returned incorrect oct string", "777777777777777777777", (long.MaxValue).ToOctalString());
        }



        /**
         * @tests java.lang.Integer#toString(int, int)
         */
        [Test]
        public void Test_ToString_Int32()
        {
            // Test for method java.lang.String java.lang.Integer.toString(int, int)
            assertEquals("Returned incorrect octal string", "17777777777", 
                    2147483647.ToString(8));
            assertTrue("Returned incorrect hex string--wanted 7fffffff but got: "
                    + 2147483647.ToString(16), 
                    2147483647.ToString(16).Equals("7fffffff"));
            assertEquals("Incorrect string returned", "1111111111111111111111111111111", 2147483647.ToString(2)
                    );
            assertEquals("Incorrect string returned", "2147483647", 2147483647.ToString(10));

            assertEquals("Returned incorrect octal string", "-17777777777", (-2147483647).ToString(8));
            assertTrue("Returned incorrect hex string--wanted -7fffffff but got: "
                    + (-2147483647).ToString(16), (-2147483647).ToString(16).Equals("-7fffffff"));
            assertEquals("Incorrect string returned",
                            "-1111111111111111111111111111111", (-2147483647).ToString(2));
            assertEquals("Incorrect string returned", "-2147483647", (-2147483647).ToString(10));

            assertEquals("Returned incorrect octal string", "-20000000000", (-2147483648).ToString(8));
            assertTrue("Returned incorrect hex string--wanted -80000000 but got: "
                    + (-2147483648).ToString(16), (-2147483648).ToString(16).Equals("-80000000"));
            assertEquals("Incorrect string returned",
                            "-10000000000000000000000000000000", (-2147483648).ToString(2));
            assertEquals("Incorrect string returned", "-2147483648", (-2147483648).ToString(10));
        }

        /**
         * @tests java.lang.Long#toString(long, int)
         */
        [Test]
        public void Test_ToString_Int64()
        {
            // Test for method java.lang.String java.lang.Long.toString(long, int)
            assertEquals("Returned incorrect dec string", "100000000", 100000000L.ToString(10));
            assertEquals("Returned incorrect hex string", "fffffffff", 68719476735L.ToString(16));
            assertEquals("Returned incorrect oct string", "77777777777", 8589934591L.ToString(8));
            assertEquals("Returned incorrect bin string",
                    "1111111111111111111111111111111111111111111", 8796093022207L.ToString(2));
            assertEquals("Returned incorrect min string", "-9223372036854775808", unchecked((long)0x8000000000000000L).ToString(10));
            assertEquals("Returned incorrect max string", "9223372036854775807", 0x7fffffffffffffffL.ToString(10));
            assertEquals("Returned incorrect min string", "-8000000000000000", unchecked((long)0x8000000000000000L).ToString(16));
            assertEquals("Returned incorrect max string", "7fffffffffffffff", 0x7fffffffffffffffL.ToString(16));
        }
    }
}
