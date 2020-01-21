using NUnit.Framework;

namespace J2N.Numerics
{
    public class TestBitOperation : TestCase
    {
        /**
         * @tests java.lang.Integer#bitCount(int)
         */
        [Test]
        public void Test_BitCount_Int32()
        {
            assertEquals(0, BitOperation.PopCount(0x0));
            assertEquals(1, BitOperation.PopCount(0x1));
            assertEquals(1, BitOperation.PopCount(0x2));
            assertEquals(2, BitOperation.PopCount(0x3));
            assertEquals(1, BitOperation.PopCount(0x4));
            assertEquals(2, BitOperation.PopCount(0x5));
            assertEquals(2, BitOperation.PopCount(0x6));
            assertEquals(3, BitOperation.PopCount(0x7));
            assertEquals(1, BitOperation.PopCount(0x8));
            assertEquals(2, BitOperation.PopCount(0x9));
            assertEquals(2, BitOperation.PopCount(0xA));
            assertEquals(3, BitOperation.PopCount(0xB));
            assertEquals(2, BitOperation.PopCount(0xC));
            assertEquals(3, BitOperation.PopCount(0xD));
            assertEquals(3, BitOperation.PopCount(0xE));
            assertEquals(4, BitOperation.PopCount(0xF));

            assertEquals(8, BitOperation.PopCount(0xFF));
            assertEquals(12, BitOperation.PopCount(0xFFF));
            assertEquals(16, BitOperation.PopCount(0xFFFF));
            assertEquals(20, BitOperation.PopCount(0xFFFFF));
            assertEquals(24, BitOperation.PopCount(0xFFFFFF));
            assertEquals(28, BitOperation.PopCount(0xFFFFFFF));
            assertEquals(32, BitOperation.PopCount(unchecked((int)0xFFFFFFFF)));
        }

        /**
         * @tests java.lang.Long#bitCount(long)
         */
        [Test]
        public void Test_BitCount_Int64()
        {
            assertEquals(0, BitOperation.PopCount(0x0L));
            assertEquals(1, BitOperation.PopCount(0x1L));
            assertEquals(1, BitOperation.PopCount(0x2L));
            assertEquals(2, BitOperation.PopCount(0x3L));
            assertEquals(1, BitOperation.PopCount(0x4L));
            assertEquals(2, BitOperation.PopCount(0x5L));
            assertEquals(2, BitOperation.PopCount(0x6L));
            assertEquals(3, BitOperation.PopCount(0x7L));
            assertEquals(1, BitOperation.PopCount(0x8L));
            assertEquals(2, BitOperation.PopCount(0x9L));
            assertEquals(2, BitOperation.PopCount(0xAL));
            assertEquals(3, BitOperation.PopCount(0xBL));
            assertEquals(2, BitOperation.PopCount(0xCL));
            assertEquals(3, BitOperation.PopCount(0xDL));
            assertEquals(3, BitOperation.PopCount(0xEL));
            assertEquals(4, BitOperation.PopCount(0xFL));

            assertEquals(8, BitOperation.PopCount(0xFFL));
            assertEquals(12, BitOperation.PopCount(0xFFFL));
            assertEquals(16, BitOperation.PopCount(0xFFFFL));
            assertEquals(20, BitOperation.PopCount(0xFFFFFL));
            assertEquals(24, BitOperation.PopCount(0xFFFFFFL));
            assertEquals(28, BitOperation.PopCount(0xFFFFFFFL));
            assertEquals(64, BitOperation.PopCount(unchecked((long)0xFFFFFFFFFFFFFFFFL)));
        }


        /**
         * @tests java.lang.Integer#highestOneBit(int)
         */
        [Test]
        public void Test_HighestOneBit_Int32()
        {
            assertEquals(0x08, BitOperation.HighestOneBit(0x0A));
            assertEquals(0x08, BitOperation.HighestOneBit(0x0B));
            assertEquals(0x08, BitOperation.HighestOneBit(0x0C));
            assertEquals(0x08, BitOperation.HighestOneBit(0x0F));
            assertEquals(0x80, BitOperation.HighestOneBit(0xFF));

            assertEquals(0x080000, BitOperation.HighestOneBit(0x0F1234));
            assertEquals(0x800000, BitOperation.HighestOneBit(0xFF9977));

            assertEquals(unchecked((int)0x80000000), BitOperation.HighestOneBit(unchecked((int)0xFFFFFFFF)));

            assertEquals(0, BitOperation.HighestOneBit(0));
            assertEquals(1, BitOperation.HighestOneBit(1));
            assertEquals(unchecked((int)0x80000000), BitOperation.HighestOneBit(-1));
        }

        /**
         * @tests java.lang.Long#highestOneBit(long)
         */
        [Test]
        public void Test_HighestOneBit_Int64()
        {
            assertEquals(0x08L, BitOperation.HighestOneBit(0x0AL));
            assertEquals(0x08L, BitOperation.HighestOneBit(0x0BL));
            assertEquals(0x08L, BitOperation.HighestOneBit(0x0CL));
            assertEquals(0x08L, BitOperation.HighestOneBit(0x0FL));
            assertEquals(0x80L, BitOperation.HighestOneBit(0xFFL));

            assertEquals(0x080000L, BitOperation.HighestOneBit(0x0F1234L));
            assertEquals(0x800000L, BitOperation.HighestOneBit(0xFF9977L));

            assertEquals(unchecked((long)0x8000000000000000L), BitOperation.HighestOneBit(unchecked((long)0xFFFFFFFFFFFFFFFFL)));

            assertEquals(0L, BitOperation.HighestOneBit(0L));
            assertEquals(1L, BitOperation.HighestOneBit(1L));
            assertEquals(unchecked((long)0x8000000000000000L), BitOperation.HighestOneBit(-1L));
        }



        /**
         * @tests java.lang.BitOperationExtensions#lowestOneBit(int)
         */
        [Test]
        public void Test_LowestOneBit_Int32()
        {
            assertEquals(0x10, BitOperation.LowestOneBit(0xF0));

            assertEquals(0x10, BitOperation.LowestOneBit(0x90));
            assertEquals(0x10, BitOperation.LowestOneBit(0xD0));

            assertEquals(0x10, BitOperation.LowestOneBit(0x123490));
            assertEquals(0x10, BitOperation.LowestOneBit(0x1234D0));

            assertEquals(0x100000, BitOperation.LowestOneBit(0x900000));
            assertEquals(0x100000, BitOperation.LowestOneBit(0xD00000));

            assertEquals(0x40, BitOperation.LowestOneBit(0x40));
            assertEquals(0x40, BitOperation.LowestOneBit(0xC0));

            assertEquals(0x4000, BitOperation.LowestOneBit(0x4000));
            assertEquals(0x4000, BitOperation.LowestOneBit(0xC000));

            assertEquals(0x4000, BitOperation.LowestOneBit(unchecked((int)0x99994000)));
            assertEquals(0x4000, BitOperation.LowestOneBit(unchecked((int)0x9999C000)));

            assertEquals(0, BitOperation.LowestOneBit(0));
            assertEquals(1, BitOperation.LowestOneBit(1));
            assertEquals(1, BitOperation.LowestOneBit(-1));
        }

        /**
         * @tests java.lang.BitOperationExtensions#lowestOneBit(long)
         */
        [Test]
        public void Test_LowestOneBit_Int64()
        {
            assertEquals(0x10L, BitOperation.LowestOneBit(0xF0L));

            assertEquals(0x10L, BitOperation.LowestOneBit(0x90L));
            assertEquals(0x10L, BitOperation.LowestOneBit(0xD0L));

            assertEquals(0x10L, BitOperation.LowestOneBit(0x123490L));
            assertEquals(0x10L, BitOperation.LowestOneBit(0x1234D0L));

            assertEquals(0x100000L, BitOperation.LowestOneBit(0x900000L));
            assertEquals(0x100000L, BitOperation.LowestOneBit(0xD00000L));

            assertEquals(0x40L, BitOperation.LowestOneBit(0x40L));
            assertEquals(0x40L, BitOperation.LowestOneBit(0xC0L));

            assertEquals(0x4000L, BitOperation.LowestOneBit(0x4000L));
            assertEquals(0x4000L, BitOperation.LowestOneBit(0xC000L));

            assertEquals(0x4000L, BitOperation.LowestOneBit(0x99994000L));
            assertEquals(0x4000L, BitOperation.LowestOneBit(0x9999C000L));

            assertEquals(0L, BitOperation.LowestOneBit(0L));
            assertEquals(1L, BitOperation.LowestOneBit(1L));
            assertEquals(1L, BitOperation.LowestOneBit(-1L));
        }


        /**
         * @tests java.lang.Integer#numberOfLeadingZeros(int)
         */
        [Test]
        public void Test_NumberOfLeadingZeros_Int32()
        {
            assertEquals(32, BitOperation.LeadingZeroCount(0x0));
            assertEquals(31, BitOperation.LeadingZeroCount(0x1));
            assertEquals(30, BitOperation.LeadingZeroCount(0x2));
            assertEquals(30, BitOperation.LeadingZeroCount(0x3));
            assertEquals(29, BitOperation.LeadingZeroCount(0x4));
            assertEquals(29, BitOperation.LeadingZeroCount(0x5));
            assertEquals(29, BitOperation.LeadingZeroCount(0x6));
            assertEquals(29, BitOperation.LeadingZeroCount(0x7));
            assertEquals(28, BitOperation.LeadingZeroCount(0x8));
            assertEquals(28, BitOperation.LeadingZeroCount(0x9));
            assertEquals(28, BitOperation.LeadingZeroCount(0xA));
            assertEquals(28, BitOperation.LeadingZeroCount(0xB));
            assertEquals(28, BitOperation.LeadingZeroCount(0xC));
            assertEquals(28, BitOperation.LeadingZeroCount(0xD));
            assertEquals(28, BitOperation.LeadingZeroCount(0xE));
            assertEquals(28, BitOperation.LeadingZeroCount(0xF));
            assertEquals(27, BitOperation.LeadingZeroCount(0x10));
            assertEquals(24, BitOperation.LeadingZeroCount(0x80));
            assertEquals(24, BitOperation.LeadingZeroCount(0xF0));
            assertEquals(23, BitOperation.LeadingZeroCount(0x100));
            assertEquals(20, BitOperation.LeadingZeroCount(0x800));
            assertEquals(20, BitOperation.LeadingZeroCount(0xF00));
            assertEquals(19, BitOperation.LeadingZeroCount(0x1000));
            assertEquals(16, BitOperation.LeadingZeroCount(0x8000));
            assertEquals(16, BitOperation.LeadingZeroCount(0xF000));
            assertEquals(15, BitOperation.LeadingZeroCount(0x10000));
            assertEquals(12, BitOperation.LeadingZeroCount(0x80000));
            assertEquals(12, BitOperation.LeadingZeroCount(0xF0000));
            assertEquals(11, BitOperation.LeadingZeroCount(0x100000));
            assertEquals(8, BitOperation.LeadingZeroCount(0x800000));
            assertEquals(8, BitOperation.LeadingZeroCount(0xF00000));
            assertEquals(7, BitOperation.LeadingZeroCount(0x1000000));
            assertEquals(4, BitOperation.LeadingZeroCount(0x8000000));
            assertEquals(4, BitOperation.LeadingZeroCount(0xF000000));
            assertEquals(3, BitOperation.LeadingZeroCount(0x10000000));
            assertEquals(0, BitOperation.LeadingZeroCount(unchecked((int)0x80000000)));
            assertEquals(0, BitOperation.LeadingZeroCount(unchecked((int)0xF0000000)));

            assertEquals(1, BitOperation.LeadingZeroCount(int.MaxValue));
            assertEquals(0, BitOperation.LeadingZeroCount(int.MinValue));
        }

        /**
         * @tests java.lang.Integer#numberOfTrailingZeros(int)
         */
        [Test]
        public void Test_NumberOfTrailingZeros_Int32()
        {
            assertEquals(32, BitOperation.TrailingZeroCount(0x0));
            assertEquals(31, BitOperation.TrailingZeroCount(int.MinValue));
            assertEquals(0, BitOperation.TrailingZeroCount(int.MaxValue));

            assertEquals(0, BitOperation.TrailingZeroCount(0x1));
            assertEquals(3, BitOperation.TrailingZeroCount(0x8));
            assertEquals(0, BitOperation.TrailingZeroCount(0xF));

            assertEquals(4, BitOperation.TrailingZeroCount(0x10));
            assertEquals(7, BitOperation.TrailingZeroCount(0x80));
            assertEquals(4, BitOperation.TrailingZeroCount(0xF0));

            assertEquals(8, BitOperation.TrailingZeroCount(0x100));
            assertEquals(11, BitOperation.TrailingZeroCount(0x800));
            assertEquals(8, BitOperation.TrailingZeroCount(0xF00));

            assertEquals(12, BitOperation.TrailingZeroCount(0x1000));
            assertEquals(15, BitOperation.TrailingZeroCount(0x8000));
            assertEquals(12, BitOperation.TrailingZeroCount(0xF000));

            assertEquals(16, BitOperation.TrailingZeroCount(0x10000));
            assertEquals(19, BitOperation.TrailingZeroCount(0x80000));
            assertEquals(16, BitOperation.TrailingZeroCount(0xF0000));

            assertEquals(20, BitOperation.TrailingZeroCount(0x100000));
            assertEquals(23, BitOperation.TrailingZeroCount(0x800000));
            assertEquals(20, BitOperation.TrailingZeroCount(0xF00000));

            assertEquals(24, BitOperation.TrailingZeroCount(0x1000000));
            assertEquals(27, BitOperation.TrailingZeroCount(0x8000000));
            assertEquals(24, BitOperation.TrailingZeroCount(0xF000000));

            assertEquals(28, BitOperation.TrailingZeroCount(0x10000000));
            assertEquals(31, BitOperation.TrailingZeroCount(unchecked((int)0x80000000)));
            assertEquals(28, BitOperation.TrailingZeroCount(unchecked((int)0xF0000000)));
        }




        /**
         * @tests java.lang.Long#numberOfLeadingZeros(long)
         */
        [Test]
        public void Test_NumberOfLeadingZeros_Int64()
        {
            assertEquals(64L, BitOperation.LeadingZeroCount(0x0L));
            assertEquals(63L, BitOperation.LeadingZeroCount(0x1L));
            assertEquals(62L, BitOperation.LeadingZeroCount(0x2L));
            assertEquals(62L, BitOperation.LeadingZeroCount(0x3L));
            assertEquals(61L, BitOperation.LeadingZeroCount(0x4L));
            assertEquals(61L, BitOperation.LeadingZeroCount(0x5L));
            assertEquals(61L, BitOperation.LeadingZeroCount(0x6L));
            assertEquals(61L, BitOperation.LeadingZeroCount(0x7L));
            assertEquals(60L, BitOperation.LeadingZeroCount(0x8L));
            assertEquals(60L, BitOperation.LeadingZeroCount(0x9L));
            assertEquals(60L, BitOperation.LeadingZeroCount(0xAL));
            assertEquals(60L, BitOperation.LeadingZeroCount(0xBL));
            assertEquals(60L, BitOperation.LeadingZeroCount(0xCL));
            assertEquals(60L, BitOperation.LeadingZeroCount(0xDL));
            assertEquals(60L, BitOperation.LeadingZeroCount(0xEL));
            assertEquals(60L, BitOperation.LeadingZeroCount(0xFL));
            assertEquals(59L, BitOperation.LeadingZeroCount(0x10L));
            assertEquals(56L, BitOperation.LeadingZeroCount(0x80L));
            assertEquals(56L, BitOperation.LeadingZeroCount(0xF0L));
            assertEquals(55L, BitOperation.LeadingZeroCount(0x100L));
            assertEquals(52L, BitOperation.LeadingZeroCount(0x800L));
            assertEquals(52L, BitOperation.LeadingZeroCount(0xF00L));
            assertEquals(51L, BitOperation.LeadingZeroCount(0x1000L));
            assertEquals(48L, BitOperation.LeadingZeroCount(0x8000L));
            assertEquals(48L, BitOperation.LeadingZeroCount(0xF000L));
            assertEquals(47L, BitOperation.LeadingZeroCount(0x10000L));
            assertEquals(44L, BitOperation.LeadingZeroCount(0x80000L));
            assertEquals(44L, BitOperation.LeadingZeroCount(0xF0000L));
            assertEquals(43L, BitOperation.LeadingZeroCount(0x100000L));
            assertEquals(40L, BitOperation.LeadingZeroCount(0x800000L));
            assertEquals(40L, BitOperation.LeadingZeroCount(0xF00000L));
            assertEquals(39L, BitOperation.LeadingZeroCount(0x1000000L));
            assertEquals(36L, BitOperation.LeadingZeroCount(0x8000000L));
            assertEquals(36L, BitOperation.LeadingZeroCount(0xF000000L));
            assertEquals(35L, BitOperation.LeadingZeroCount(0x10000000L));
            assertEquals(32L, BitOperation.LeadingZeroCount(0x80000000L)); // J2N: Changed test to match observed behavior in JDK (32L rather than 0L)
            assertEquals(32L, BitOperation.LeadingZeroCount(0xF0000000L)); // J2N: Changed test to match observed behavior in JDK (32L rather than 0L)

            assertEquals(1L, BitOperation.LeadingZeroCount(long.MaxValue));
            assertEquals(0L, BitOperation.LeadingZeroCount(long.MinValue));
        }

        /**
         * @tests java.lang.Long#numberOfTrailingZeros(long)
         */
        [Test]
        public void Test_NumberOfTrailingZeros_Int64()
        {
            assertEquals(64L, BitOperation.TrailingZeroCount(0x0L));
            assertEquals(63L, BitOperation.TrailingZeroCount(long.MinValue));
            assertEquals(0L, BitOperation.TrailingZeroCount(long.MaxValue));

            assertEquals(0L, BitOperation.TrailingZeroCount(0x1L));
            assertEquals(3L, BitOperation.TrailingZeroCount(0x8L));
            assertEquals(0L, BitOperation.TrailingZeroCount(0xFL));

            assertEquals(4L, BitOperation.TrailingZeroCount(0x10L));
            assertEquals(7L, BitOperation.TrailingZeroCount(0x80L));
            assertEquals(4L, BitOperation.TrailingZeroCount(0xF0L));

            assertEquals(8L, BitOperation.TrailingZeroCount(0x100L));
            assertEquals(11L, BitOperation.TrailingZeroCount(0x800L));
            assertEquals(8L, BitOperation.TrailingZeroCount(0xF00L));

            assertEquals(12L, BitOperation.TrailingZeroCount(0x1000L));
            assertEquals(15L, BitOperation.TrailingZeroCount(0x8000L));
            assertEquals(12L, BitOperation.TrailingZeroCount(0xF000L));

            assertEquals(16L, BitOperation.TrailingZeroCount(0x10000L));
            assertEquals(19L, BitOperation.TrailingZeroCount(0x80000L));
            assertEquals(16L, BitOperation.TrailingZeroCount(0xF0000L));

            assertEquals(20L, BitOperation.TrailingZeroCount(0x100000L));
            assertEquals(23L, BitOperation.TrailingZeroCount(0x800000L));
            assertEquals(20L, BitOperation.TrailingZeroCount(0xF00000L));

            assertEquals(24L, BitOperation.TrailingZeroCount(0x1000000L));
            assertEquals(27L, BitOperation.TrailingZeroCount(0x8000000L));
            assertEquals(24L, BitOperation.TrailingZeroCount(0xF000000L));

            assertEquals(28L, BitOperation.TrailingZeroCount(0x10000000L));
            assertEquals(31L, BitOperation.TrailingZeroCount(0x80000000L));
            assertEquals(28L, BitOperation.TrailingZeroCount(0xF0000000L));
        }

        /**
         * @tests java.lang.Integer#rotateLeft(int,int)
         */
        [Test]
        public void Test_RotateLeft_Int32()
        {
            assertEquals(0xF, BitOperation.RotateLeft(0xF, 0));
            assertEquals(0xF0, BitOperation.RotateLeft(0xF, 4));
            assertEquals(0xF00, BitOperation.RotateLeft(0xF, 8));
            assertEquals(0xF000, BitOperation.RotateLeft(0xF, 12));
            assertEquals(0xF0000, BitOperation.RotateLeft(0xF, 16));
            assertEquals(0xF00000, BitOperation.RotateLeft(0xF, 20));
            assertEquals(0xF000000, BitOperation.RotateLeft(0xF, 24));
            unchecked
            {
                assertEquals((int)0xF0000000, BitOperation.RotateLeft(0xF, 28));
                assertEquals((int)0xF0000000, BitOperation.RotateLeft((int)0xF0000000, 32));
            }
        }

        /**
         * @tests java.lang.Long#rotateLeft(long,long)
         */
        [Test]
        public void Test_RotateLeft_Int64()
        {
            assertEquals(0xF, BitOperation.RotateLeft(0xFL, 0));
            assertEquals(0xF0, BitOperation.RotateLeft(0xFL, 4));
            assertEquals(0xF00, BitOperation.RotateLeft(0xFL, 8));
            assertEquals(0xF000, BitOperation.RotateLeft(0xFL, 12));
            assertEquals(0xF0000, BitOperation.RotateLeft(0xFL, 16));
            assertEquals(0xF00000, BitOperation.RotateLeft(0xFL, 20));
            assertEquals(0xF000000, BitOperation.RotateLeft(0xFL, 24));
            assertEquals(0xF0000000L, BitOperation.RotateLeft(0xFL, 28));
            unchecked
            {
                assertEquals((long)0xF000000000000000L, BitOperation.RotateLeft((long)0xF000000000000000L, 64));
            }
        }

        /**
         * @tests java.lang.Integer#rotateRight(int,int)
         */
        [Test]
        public void Test_RotateRight_Int32()
        {
            assertEquals(0xF, BitOperation.RotateRight(0xF0, 4));
            assertEquals(0xF, BitOperation.RotateRight(0xF00, 8));
            assertEquals(0xF, BitOperation.RotateRight(0xF000, 12));
            assertEquals(0xF, BitOperation.RotateRight(0xF0000, 16));
            assertEquals(0xF, BitOperation.RotateRight(0xF00000, 20));
            assertEquals(0xF, BitOperation.RotateRight(0xF000000, 24));
            unchecked
            {
                assertEquals(0xF, BitOperation.RotateRight((int)0xF0000000, 28));
                assertEquals((int)0xF0000000, BitOperation.RotateRight((int)0xF0000000, 32));
                assertEquals((int)0xF0000000, BitOperation.RotateRight((int)0xF0000000, 0));
            }

        }

        /**
         * @tests java.lang.Long#rotateRight(long,long)
         */
        [Test]
        public void Test_RotateRight_Int64()
        {
            assertEquals(0xFL, BitOperation.RotateRight(0xF0L, 4));
            assertEquals(0xFL, BitOperation.RotateRight(0xF00L, 8));
            assertEquals(0xFL, BitOperation.RotateRight(0xF000L, 12));
            assertEquals(0xFL, BitOperation.RotateRight(0xF0000L, 16));
            assertEquals(0xFL, BitOperation.RotateRight(0xF00000L, 20));
            assertEquals(0xFL, BitOperation.RotateRight(0xF000000L, 24));
            assertEquals(0xFL, BitOperation.RotateRight(0xF0000000L, 28));
            unchecked
            {
                assertEquals((long)0xF000000000000000L, BitOperation.RotateRight((long)0xF000000000000000L, 64));
                assertEquals((long)0xF000000000000000L, BitOperation.RotateRight((long)0xF000000000000000L, 0));
            }

        }


        /**
         * @tests java.lang.Integer#reverseBytes(int)
         */
        [Test]
        public void Test_ReverseBytes_Int32()
        {
            unchecked
            {
                assertEquals((int)0xAABBCCDD, BitOperation.ReverseBytes((int)0xDDCCBBAA));
            }
            assertEquals(0x11223344, BitOperation.ReverseBytes(0x44332211));
            assertEquals(0x00112233, BitOperation.ReverseBytes(0x33221100));
            assertEquals(0x20000002, BitOperation.ReverseBytes(0x02000020));
        }

        /**
         * @tests java.lang.Long#reverseBytes(long)
         */
        [Test]
        public void Test_ReverseBytes_Int64()
        {
            unchecked
            {
                assertEquals((long)0xAABBCCDD00112233L, BitOperation.ReverseBytes(0x33221100DDCCBBAAL));
                assertEquals(0x1122334455667788L, BitOperation.ReverseBytes((long)0x8877665544332211L));
            }
            assertEquals(0x0011223344556677L, BitOperation.ReverseBytes(0x7766554433221100L));
            assertEquals(0x2000000000000002L, BitOperation.ReverseBytes(0x0200000000000020L));
        }


        /**
         * @tests java.lang.Integer#reverse(int)
         */
        [Test]
        public void Test_Reverse_Int32()
        {
            assertEquals(-1, BitOperation.Reverse(-1));
            assertEquals(unchecked((int)0x80000000), BitOperation.Reverse(1));
        }

        /**
         * @tests java.lang.Long#reverse(long)
         */
        [Test]
        public void Test_Reverse_Int64()
        {
            assertEquals(0L, BitOperation.Reverse(0L));
            assertEquals(-1L, BitOperation.Reverse(-1L));
            assertEquals(unchecked((long)0x8000000000000000L), BitOperation.Reverse(1L));
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
