using NUnit.Framework;

namespace J2N
{
    public class TestIntegralExtensions : TestCase
    {
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
        public void TestToHexString_Int32()
        {
            assertEquals("ffffffd6", (-42).ToHexString());
            assertEquals("2a", (42).ToHexString());
            assertEquals("5f5e0ff", (99999999).ToHexString());
        }

        [Test]
        public void TestToHexString_Int64()
        {
            assertEquals("ffffffffffffffd6", (-42L).ToHexString());
            assertEquals("2a", (42L).ToHexString());
            assertEquals("16345785d89ffff", (99999999999999999L).ToHexString());
        }

        [Test]
        public void TestToHexString_Char()
        {
            assertEquals("ffd6", (unchecked((char)-42)).ToHexString());
            assertEquals("2a", ((char)42).ToHexString());
            assertEquals("270f", ((char)9999).ToHexString());
        }


        /*
        public void testTripleShift_Integer() throws Exception {

            int num = Integer.MIN_VALUE;
            assertEquals(134217728, num >>> 4);
            assertEquals(32768, num >>> 16);
            assertEquals(-2147483648, num >>> 64);

            num = -656565656;
            assertEquals(227400102, num >>> 4);
            assertEquals(55517, num >>> 16);
            assertEquals(-656565656, num >>> 64);

            num = Integer.MIN_VALUE >> 4;
            assertEquals(260046848, num >>> 4);
            assertEquals(63488, num >>> 16);
            assertEquals(-134217728, num >>> 64);

            num = Integer.MIN_VALUE >> 8;
            assertEquals(267911168, num >>> 4);
            assertEquals(65408, num >>> 16);
            assertEquals(-8388608, num >>> 64);

            num = 0;
            assertEquals(0, num >>> 4);
            assertEquals(0, num >>> 16);
            assertEquals(0, num >>> 64);

            num = 888888888;
            assertEquals(55555555, num >>> 4);
            assertEquals(13563, num >>> 16);
            assertEquals(888888888, num >>> 64);

            num = Integer.MAX_VALUE;
            assertEquals(134217727, num >>> 4);
            assertEquals(32767, num >>> 16);
            assertEquals(2147483647, num >>> 64);

        }
        */

        [Test]
        public void TestTripleShift_Int32()
        {
            int num = int.MinValue;
            assertEquals(134217728, num.TripleShift(4));
            assertEquals(32768, num.TripleShift(16));
            assertEquals(-2147483648, num.TripleShift(64));

            num = -656565656;
            assertEquals(227400102, num.TripleShift(4));
            assertEquals(55517, num.TripleShift(16));
            assertEquals(-656565656, num.TripleShift(64));

            num = int.MinValue >> 4;
            assertEquals(260046848, num.TripleShift(4));
            assertEquals(63488, num.TripleShift(16));
            assertEquals(-134217728, num.TripleShift(64));

            num = int.MinValue >> 8;
            assertEquals(267911168, num.TripleShift(4));
            assertEquals(65408, num.TripleShift(16));
            assertEquals(-8388608, num.TripleShift(64));

            num = 0;
            assertEquals(0, num.TripleShift(4));
            assertEquals(0, num.TripleShift(16));
            assertEquals(0, num.TripleShift(64));

            num = 888888888;
            assertEquals(55555555, num.TripleShift(4));
            assertEquals(13563, num.TripleShift(16));
            assertEquals(888888888, num.TripleShift(64));

            num = int.MaxValue;
            assertEquals(134217727, num.TripleShift(4));
            assertEquals(32767, num.TripleShift(16));
            assertEquals(2147483647, num.TripleShift(64));
        }

        /*
          public void testTripleShift_Long() throws Exception {
              long num = Long.MIN_VALUE;
              assertEquals(576460752303423488L, num >>> 4);
              assertEquals(140737488355328L, num >>> 16);
              assertEquals(-9223372036854775808L, num >>> 64);

              num = -656565656;
              assertEquals(1152921504565811622L, num >>> 4);
              assertEquals(281474976700637L, num >>> 16);
              assertEquals(-656565656, num >>> 64);

              num = Long.MIN_VALUE >> 4;
              assertEquals(1116892707587883008L, num >>> 4);
              assertEquals(272678883688448L, num >>> 16);
              assertEquals(-576460752303423488L, num >>> 64);

              num = Long.MIN_VALUE >> 8;
              assertEquals(1150669704793161728L, num >>> 4);
              assertEquals(280925220896768L, num >>> 16);
              assertEquals(-36028797018963968L, num >>> 64);

              num = 0;
              assertEquals(0, num >>> 4);
              assertEquals(0, num >>> 16);
              assertEquals(0, num >>> 64);

              num = 888888888;
              assertEquals(55555555L, num >>> 4);
              assertEquals(13563L, num >>> 16);
              assertEquals(888888888L, num >>> 64);

              num = Long.MAX_VALUE;
              assertEquals(576460752303423487L, num >>> 4);
              assertEquals(140737488355327L, num >>> 16);
              assertEquals(9223372036854775807L, num >>> 64);
          }
        */

        [Test]
        public void TestTripleShift_Int64()
        {
            long num = long.MinValue;
            assertEquals(576460752303423488L, num.TripleShift(4));
            assertEquals(140737488355328L, num.TripleShift(16));
            assertEquals(-9223372036854775808L, num.TripleShift(64));

            num = -656565656;
            assertEquals(1152921504565811622L, num.TripleShift(4));
            assertEquals(281474976700637L, num.TripleShift(16));
            assertEquals(-656565656, num.TripleShift(64));

            num = long.MinValue >> 4;
            assertEquals(1116892707587883008L, num.TripleShift(4));
            assertEquals(272678883688448L, num.TripleShift(16));
            assertEquals(-576460752303423488L, num.TripleShift(64));

            num = long.MinValue >> 8;
            assertEquals(1150669704793161728L, num.TripleShift(4));
            assertEquals(280925220896768L, num.TripleShift(16));
            assertEquals(-36028797018963968L, num.TripleShift(64));

            num = 0;
            assertEquals(0, num.TripleShift(4));
            assertEquals(0, num.TripleShift(16));
            assertEquals(0, num.TripleShift(64));

            num = 888888888;
            assertEquals(55555555L, num.TripleShift(4));
            assertEquals(13563L, num.TripleShift(16));
            assertEquals(888888888L, num.TripleShift(64));

            num = long.MaxValue;
            assertEquals(576460752303423487L, num.TripleShift(4));
            assertEquals(140737488355327L, num.TripleShift(16));
            assertEquals(9223372036854775807L, num.TripleShift(64));
        }

        /*
        public void testTripleShift_Short() throws Exception {
              short num = Short.MIN_VALUE;
              assertEquals(268433408, num >>> 4);
              assertEquals(67108352, num >>> 6);
              assertEquals(1048568, num >>> 12);

              num = -6565;
              assertEquals(268435045, num >>> 4);
              assertEquals(67108761, num >>> 6);
              assertEquals(1048574, num >>> 12);

              num = Short.MIN_VALUE >> 4;
              assertEquals(268435328, num >>> 4);
              assertEquals(67108832, num >>> 6);
              assertEquals(1048575, num >>> 12);

              num = Short.MIN_VALUE >> 8;
              assertEquals(268435448, num >>> 4);
              assertEquals(67108862, num >>> 6);
              assertEquals(1048575, num >>> 12);

              num = 0;
              assertEquals(0, num >>> 4);
              assertEquals(0, num >>> 6);
              assertEquals(0, num >>> 12);

              num = 8888;
              assertEquals(555, num >>> 4);
              assertEquals(138, num >>> 6);
              assertEquals(2, num >>> 12);

              num = Short.MAX_VALUE;
              assertEquals(2047, num >>> 4);
              assertEquals(511, num >>> 6);
              assertEquals(7, num >>> 12);
          }
        */

        [Test]
        public void TestTripleShift_Int16()
        {
            short num = short.MinValue;
            assertEquals(268433408, num.TripleShift(4));
            assertEquals(67108352, num.TripleShift(6));
            assertEquals(1048568, num.TripleShift(12));

            num = -6565;
            assertEquals(268435045, num.TripleShift(4));
            assertEquals(67108761, num.TripleShift(6));
            assertEquals(1048574, num.TripleShift(12));

            num = short.MinValue >> 4;
            assertEquals(268435328, num.TripleShift(4));
            assertEquals(67108832, num.TripleShift(6));
            assertEquals(1048575, num.TripleShift(12));

            num = short.MinValue >> 8;
            assertEquals(268435448, num.TripleShift(4));
            assertEquals(67108862, num.TripleShift(6));
            assertEquals(1048575, num.TripleShift(12));

            num = 0;
            assertEquals(0, num.TripleShift(4));
            assertEquals(0, num.TripleShift(6));
            assertEquals(0, num.TripleShift(12));

            num = 8888;
            assertEquals(555, num.TripleShift(4));
            assertEquals(138, num.TripleShift(6));
            assertEquals(2, num.TripleShift(12));

            num = short.MaxValue;
            assertEquals(2047, num.TripleShift(4));
            assertEquals(511, num.TripleShift(6));
            assertEquals(7, num.TripleShift(12));
        }

        /*
        public void testTripleShift_Char() throws Exception {
              char num = Character.MIN_VALUE;
              assertEquals(0, num >>> 1);
              assertEquals(0, num >>> 3);
              assertEquals(0, num >>> 9);

              num = (char)-6565;
              assertEquals(29485, num >>> 1);
              assertEquals(7371, num >>> 3);
              assertEquals(115, num >>> 9);

              num = (char)-67;
              assertEquals(32734, num >>> 1);
              assertEquals(8183, num >>> 3);
              assertEquals(127, num >>> 9);

              num = (char)-1;
              assertEquals(32767, num >>> 1);
              assertEquals(8191, num >>> 3);
              assertEquals(127, num >>> 9);

              num = (char)0;
              assertEquals(0, num >>> 1);
              assertEquals(0, num >>> 3);
              assertEquals(0, num >>> 9);

              num = (char)8888;
              assertEquals(4444, num >>> 1);
              assertEquals(1111, num >>> 3);
              assertEquals(17, num >>> 9);

              num = Character.MAX_VALUE;
              assertEquals(32767, num >>> 1);
              assertEquals(8191, num >>> 3);
              assertEquals(127, num >>> 9);
          }
        */

        [Test]
        public void TestTripleShift_Char()
        {

            char num = char.MinValue;
            assertEquals(0, num.TripleShift(1));
            assertEquals(0, num.TripleShift(3));
            assertEquals(0, num.TripleShift(9));

            num = unchecked((char)-6565);
            assertEquals(29485, num.TripleShift(1));
            assertEquals(7371, num.TripleShift(3));
            assertEquals(115, num.TripleShift(9));

            num = unchecked((char)-67);
            assertEquals(32734, num.TripleShift(1));
            assertEquals(8183, num.TripleShift(3));
            assertEquals(127, num.TripleShift(9));

            num = unchecked((char)-1);
            assertEquals(32767, num.TripleShift(1));
            assertEquals(8191, num.TripleShift(3));
            assertEquals(127, num.TripleShift(9));

            num = (char)0;
            assertEquals(0, num.TripleShift(1));
            assertEquals(0, num.TripleShift(3));
            assertEquals(0, num.TripleShift(9));

            num = (char)8888;
            assertEquals(4444, num.TripleShift(1));
            assertEquals(1111, num.TripleShift(3));
            assertEquals(17, num.TripleShift(9));

            num = char.MaxValue;
            assertEquals(32767, num.TripleShift(1));
            assertEquals(8191, num.TripleShift(3));
            assertEquals(127, num.TripleShift(9));
        }


        /*
        public void testTripleShift_Byte() throws Exception {
              byte num = Byte.MIN_VALUE;
              assertEquals(2147483584, num >>> 1);
              assertEquals(536870896, num >>> 3);
              assertEquals(67108862, num >>> 6);

              num = (byte)-131;
              assertEquals(62, num >>> 1);
              assertEquals(15, num >>> 3);
              assertEquals(1, num >>> 6);

              num = -67;
              assertEquals(2147483614, num >>> 1);
              assertEquals(536870903, num >>> 3);
              assertEquals(67108862, num >>> 6);

              num = -1;
              assertEquals(2147483647, num >>> 1);
              assertEquals(536870911, num >>> 3);
              assertEquals(67108863, num >>> 6);

              num = 0;
              assertEquals(0, num >>> 1);
              assertEquals(0, num >>> 3);
              assertEquals(0, num >>> 6);

              num = (byte)888;
              assertEquals(60, num >>> 1);
              assertEquals(15, num >>> 3);
              assertEquals(1, num >>> 6);

              num = Byte.MAX_VALUE;
              assertEquals(63, num >>> 1);
              assertEquals(15, num >>> 3);
              assertEquals(1, num >>> 6);
          }
        */

        [Test]
        public void TestTripleShift_Byte()
        {
            byte num = unchecked((byte)sbyte.MinValue);
            assertEquals(2147483584, num.TripleShift(1));
            assertEquals(536870896, num.TripleShift(3));
            assertEquals(67108862, num.TripleShift(6));

            num = unchecked((byte)-131);
            assertEquals(62, num.TripleShift(1));
            assertEquals(15, num.TripleShift(3));
            assertEquals(1, num.TripleShift(6));

            num = unchecked((byte)-67);
            assertEquals(2147483614, num.TripleShift(1));
            assertEquals(536870903, num.TripleShift(3));
            assertEquals(67108862, num.TripleShift(6));

            num = unchecked((byte)-1);
            assertEquals(2147483647, num.TripleShift(1));
            assertEquals(536870911, num.TripleShift(3));
            assertEquals(67108863, num.TripleShift(6));

            num = 0;
            assertEquals(0, num.TripleShift(1));
            assertEquals(0, num.TripleShift(3));
            assertEquals(0, num.TripleShift(6));

            num = unchecked((byte)888);
            assertEquals(60, num.TripleShift(1));
            assertEquals(15, num.TripleShift(3));
            assertEquals(1, num.TripleShift(6));

            num = (byte)sbyte.MaxValue;
            assertEquals(63, num.TripleShift(1));
            assertEquals(15, num.TripleShift(3));
            assertEquals(1, num.TripleShift(6));
        }

        [Test]
        public void TestTripleShift_SByte()
        {
            sbyte num = sbyte.MinValue;
            assertEquals(2147483584, num.TripleShift(1));
            assertEquals(536870896, num.TripleShift(3));
            assertEquals(67108862, num.TripleShift(6));

            num = unchecked((sbyte)-131);
            assertEquals(62, num.TripleShift(1));
            assertEquals(15, num.TripleShift(3));
            assertEquals(1, num.TripleShift(6));

            num = -67;
            assertEquals(2147483614, num.TripleShift(1));
            assertEquals(536870903, num.TripleShift(3));
            assertEquals(67108862, num.TripleShift(6));

            num = -1;
            assertEquals(2147483647, num.TripleShift(1));
            assertEquals(536870911, num.TripleShift(3));
            assertEquals(67108863, num.TripleShift(6));

            num = 0;
            assertEquals(0, num.TripleShift(1));
            assertEquals(0, num.TripleShift(3));
            assertEquals(0, num.TripleShift(6));

            num = unchecked((sbyte)888);
            assertEquals(60, num.TripleShift(1));
            assertEquals(15, num.TripleShift(3));
            assertEquals(1, num.TripleShift(6));

            num = sbyte.MaxValue;
            assertEquals(63, num.TripleShift(1));
            assertEquals(15, num.TripleShift(3));
            assertEquals(1, num.TripleShift(6));
        }
    }
}
