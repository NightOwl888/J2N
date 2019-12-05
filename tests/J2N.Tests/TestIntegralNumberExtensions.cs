using NUnit.Framework;

namespace J2N
{
    public class TestIntegralNumberExtensions : TestCase
    {
        /**
         * @tests java.lang.Integer#bitCount(int)
         */
        [Test]
        public void Test_BitCount_Int32()
        {
            assertEquals(0, IntegralNumberExtensions.BitCount(0x0));
            assertEquals(1, IntegralNumberExtensions.BitCount(0x1));
            assertEquals(1, IntegralNumberExtensions.BitCount(0x2));
            assertEquals(2, IntegralNumberExtensions.BitCount(0x3));
            assertEquals(1, IntegralNumberExtensions.BitCount(0x4));
            assertEquals(2, IntegralNumberExtensions.BitCount(0x5));
            assertEquals(2, IntegralNumberExtensions.BitCount(0x6));
            assertEquals(3, IntegralNumberExtensions.BitCount(0x7));
            assertEquals(1, IntegralNumberExtensions.BitCount(0x8));
            assertEquals(2, IntegralNumberExtensions.BitCount(0x9));
            assertEquals(2, IntegralNumberExtensions.BitCount(0xA));
            assertEquals(3, IntegralNumberExtensions.BitCount(0xB));
            assertEquals(2, IntegralNumberExtensions.BitCount(0xC));
            assertEquals(3, IntegralNumberExtensions.BitCount(0xD));
            assertEquals(3, IntegralNumberExtensions.BitCount(0xE));
            assertEquals(4, IntegralNumberExtensions.BitCount(0xF));

            assertEquals(8, IntegralNumberExtensions.BitCount(0xFF));
            assertEquals(12, IntegralNumberExtensions.BitCount(0xFFF));
            assertEquals(16, IntegralNumberExtensions.BitCount(0xFFFF));
            assertEquals(20, IntegralNumberExtensions.BitCount(0xFFFFF));
            assertEquals(24, IntegralNumberExtensions.BitCount(0xFFFFFF));
            assertEquals(28, IntegralNumberExtensions.BitCount(0xFFFFFFF));
            assertEquals(32, IntegralNumberExtensions.BitCount(unchecked((int)0xFFFFFFFF)));
        }

        /**
         * @tests java.lang.Long#bitCount(long)
         */
        [Test]
        public void Test_BitCount_Int64()
        {
            assertEquals(0, IntegralNumberExtensions.BitCount(0x0L));
            assertEquals(1, IntegralNumberExtensions.BitCount(0x1L));
            assertEquals(1, IntegralNumberExtensions.BitCount(0x2L));
            assertEquals(2, IntegralNumberExtensions.BitCount(0x3L));
            assertEquals(1, IntegralNumberExtensions.BitCount(0x4L));
            assertEquals(2, IntegralNumberExtensions.BitCount(0x5L));
            assertEquals(2, IntegralNumberExtensions.BitCount(0x6L));
            assertEquals(3, IntegralNumberExtensions.BitCount(0x7L));
            assertEquals(1, IntegralNumberExtensions.BitCount(0x8L));
            assertEquals(2, IntegralNumberExtensions.BitCount(0x9L));
            assertEquals(2, IntegralNumberExtensions.BitCount(0xAL));
            assertEquals(3, IntegralNumberExtensions.BitCount(0xBL));
            assertEquals(2, IntegralNumberExtensions.BitCount(0xCL));
            assertEquals(3, IntegralNumberExtensions.BitCount(0xDL));
            assertEquals(3, IntegralNumberExtensions.BitCount(0xEL));
            assertEquals(4, IntegralNumberExtensions.BitCount(0xFL));

            assertEquals(8, IntegralNumberExtensions.BitCount(0xFFL));
            assertEquals(12, IntegralNumberExtensions.BitCount(0xFFFL));
            assertEquals(16, IntegralNumberExtensions.BitCount(0xFFFFL));
            assertEquals(20, IntegralNumberExtensions.BitCount(0xFFFFFL));
            assertEquals(24, IntegralNumberExtensions.BitCount(0xFFFFFFL));
            assertEquals(28, IntegralNumberExtensions.BitCount(0xFFFFFFFL));
            assertEquals(64, IntegralNumberExtensions.BitCount(unchecked((long)0xFFFFFFFFFFFFFFFFL)));
        }


        /**
         * @tests java.lang.Integer#highestOneBit(int)
         */
        [Test]
        public void Test_HighestOneBit_Int32()
        {
            assertEquals(0x08, IntegralNumberExtensions.HighestOneBit(0x0A));
            assertEquals(0x08, IntegralNumberExtensions.HighestOneBit(0x0B));
            assertEquals(0x08, IntegralNumberExtensions.HighestOneBit(0x0C));
            assertEquals(0x08, IntegralNumberExtensions.HighestOneBit(0x0F));
            assertEquals(0x80, IntegralNumberExtensions.HighestOneBit(0xFF));

            assertEquals(0x080000, IntegralNumberExtensions.HighestOneBit(0x0F1234));
            assertEquals(0x800000, IntegralNumberExtensions.HighestOneBit(0xFF9977));

            assertEquals(unchecked((int)0x80000000), IntegralNumberExtensions.HighestOneBit(unchecked((int)0xFFFFFFFF)));

            assertEquals(0, IntegralNumberExtensions.HighestOneBit(0));
            assertEquals(1, IntegralNumberExtensions.HighestOneBit(1));
            assertEquals(unchecked((int)0x80000000), IntegralNumberExtensions.HighestOneBit(-1));
        }

        /**
         * @tests java.lang.Long#highestOneBit(long)
         */
        [Test]
        public void Test_HighestOneBit_Int64()
        {
            assertEquals(0x08L, IntegralNumberExtensions.HighestOneBit(0x0AL));
            assertEquals(0x08L, IntegralNumberExtensions.HighestOneBit(0x0BL));
            assertEquals(0x08L, IntegralNumberExtensions.HighestOneBit(0x0CL));
            assertEquals(0x08L, IntegralNumberExtensions.HighestOneBit(0x0FL));
            assertEquals(0x80L, IntegralNumberExtensions.HighestOneBit(0xFFL));

            assertEquals(0x080000L, IntegralNumberExtensions.HighestOneBit(0x0F1234L));
            assertEquals(0x800000L, IntegralNumberExtensions.HighestOneBit(0xFF9977L));

            assertEquals(unchecked((long)0x8000000000000000L), IntegralNumberExtensions.HighestOneBit(unchecked((long)0xFFFFFFFFFFFFFFFFL)));

            assertEquals(0L, IntegralNumberExtensions.HighestOneBit(0L));
            assertEquals(1L, IntegralNumberExtensions.HighestOneBit(1L));
            assertEquals(unchecked((long)0x8000000000000000L), IntegralNumberExtensions.HighestOneBit(-1L));
        }

        

        /**
         * @tests java.lang.IntegralNumberExtensions#lowestOneBit(int)
         */
        [Test]
        public void Test_LowestOneBit_Int32()
        {
            assertEquals(0x10, IntegralNumberExtensions.LowestOneBit(0xF0));

            assertEquals(0x10, IntegralNumberExtensions.LowestOneBit(0x90));
            assertEquals(0x10, IntegralNumberExtensions.LowestOneBit(0xD0));

            assertEquals(0x10, IntegralNumberExtensions.LowestOneBit(0x123490));
            assertEquals(0x10, IntegralNumberExtensions.LowestOneBit(0x1234D0));

            assertEquals(0x100000, IntegralNumberExtensions.LowestOneBit(0x900000));
            assertEquals(0x100000, IntegralNumberExtensions.LowestOneBit(0xD00000));

            assertEquals(0x40, IntegralNumberExtensions.LowestOneBit(0x40));
            assertEquals(0x40, IntegralNumberExtensions.LowestOneBit(0xC0));

            assertEquals(0x4000, IntegralNumberExtensions.LowestOneBit(0x4000));
            assertEquals(0x4000, IntegralNumberExtensions.LowestOneBit(0xC000));

            assertEquals(0x4000, IntegralNumberExtensions.LowestOneBit(unchecked((int)0x99994000)));
            assertEquals(0x4000, IntegralNumberExtensions.LowestOneBit(unchecked((int)0x9999C000)));

            assertEquals(0, IntegralNumberExtensions.LowestOneBit(0));
            assertEquals(1, IntegralNumberExtensions.LowestOneBit(1));
            assertEquals(1, IntegralNumberExtensions.LowestOneBit(-1));
        }

        /**
         * @tests java.lang.IntegralNumberExtensions#lowestOneBit(long)
         */
        [Test]
        public void Test_LowestOneBit_Int64()
        {
            assertEquals(0x10L, IntegralNumberExtensions.LowestOneBit(0xF0L));

            assertEquals(0x10L, IntegralNumberExtensions.LowestOneBit(0x90L));
            assertEquals(0x10L, IntegralNumberExtensions.LowestOneBit(0xD0L));

            assertEquals(0x10L, IntegralNumberExtensions.LowestOneBit(0x123490L));
            assertEquals(0x10L, IntegralNumberExtensions.LowestOneBit(0x1234D0L));

            assertEquals(0x100000L, IntegralNumberExtensions.LowestOneBit(0x900000L));
            assertEquals(0x100000L, IntegralNumberExtensions.LowestOneBit(0xD00000L));

            assertEquals(0x40L, IntegralNumberExtensions.LowestOneBit(0x40L));
            assertEquals(0x40L, IntegralNumberExtensions.LowestOneBit(0xC0L));

            assertEquals(0x4000L, IntegralNumberExtensions.LowestOneBit(0x4000L));
            assertEquals(0x4000L, IntegralNumberExtensions.LowestOneBit(0xC000L));

            assertEquals(0x4000L, IntegralNumberExtensions.LowestOneBit(0x99994000L));
            assertEquals(0x4000L, IntegralNumberExtensions.LowestOneBit(0x9999C000L));

            assertEquals(0L, IntegralNumberExtensions.LowestOneBit(0L));
            assertEquals(1L, IntegralNumberExtensions.LowestOneBit(1L));
            assertEquals(1L, IntegralNumberExtensions.LowestOneBit(-1L));
        }


        /**
         * @tests java.lang.Integer#numberOfLeadingZeros(int)
         */
        [Test]
        public void Test_NumberOfLeadingZeros_Int32()
        {
            assertEquals(32, IntegralNumberExtensions.NumberOfLeadingZeros(0x0));
            assertEquals(31, IntegralNumberExtensions.NumberOfLeadingZeros(0x1));
            assertEquals(30, IntegralNumberExtensions.NumberOfLeadingZeros(0x2));
            assertEquals(30, IntegralNumberExtensions.NumberOfLeadingZeros(0x3));
            assertEquals(29, IntegralNumberExtensions.NumberOfLeadingZeros(0x4));
            assertEquals(29, IntegralNumberExtensions.NumberOfLeadingZeros(0x5));
            assertEquals(29, IntegralNumberExtensions.NumberOfLeadingZeros(0x6));
            assertEquals(29, IntegralNumberExtensions.NumberOfLeadingZeros(0x7));
            assertEquals(28, IntegralNumberExtensions.NumberOfLeadingZeros(0x8));
            assertEquals(28, IntegralNumberExtensions.NumberOfLeadingZeros(0x9));
            assertEquals(28, IntegralNumberExtensions.NumberOfLeadingZeros(0xA));
            assertEquals(28, IntegralNumberExtensions.NumberOfLeadingZeros(0xB));
            assertEquals(28, IntegralNumberExtensions.NumberOfLeadingZeros(0xC));
            assertEquals(28, IntegralNumberExtensions.NumberOfLeadingZeros(0xD));
            assertEquals(28, IntegralNumberExtensions.NumberOfLeadingZeros(0xE));
            assertEquals(28, IntegralNumberExtensions.NumberOfLeadingZeros(0xF));
            assertEquals(27, IntegralNumberExtensions.NumberOfLeadingZeros(0x10));
            assertEquals(24, IntegralNumberExtensions.NumberOfLeadingZeros(0x80));
            assertEquals(24, IntegralNumberExtensions.NumberOfLeadingZeros(0xF0));
            assertEquals(23, IntegralNumberExtensions.NumberOfLeadingZeros(0x100));
            assertEquals(20, IntegralNumberExtensions.NumberOfLeadingZeros(0x800));
            assertEquals(20, IntegralNumberExtensions.NumberOfLeadingZeros(0xF00));
            assertEquals(19, IntegralNumberExtensions.NumberOfLeadingZeros(0x1000));
            assertEquals(16, IntegralNumberExtensions.NumberOfLeadingZeros(0x8000));
            assertEquals(16, IntegralNumberExtensions.NumberOfLeadingZeros(0xF000));
            assertEquals(15, IntegralNumberExtensions.NumberOfLeadingZeros(0x10000));
            assertEquals(12, IntegralNumberExtensions.NumberOfLeadingZeros(0x80000));
            assertEquals(12, IntegralNumberExtensions.NumberOfLeadingZeros(0xF0000));
            assertEquals(11, IntegralNumberExtensions.NumberOfLeadingZeros(0x100000));
            assertEquals(8, IntegralNumberExtensions.NumberOfLeadingZeros(0x800000));
            assertEquals(8, IntegralNumberExtensions.NumberOfLeadingZeros(0xF00000));
            assertEquals(7, IntegralNumberExtensions.NumberOfLeadingZeros(0x1000000));
            assertEquals(4, IntegralNumberExtensions.NumberOfLeadingZeros(0x8000000));
            assertEquals(4, IntegralNumberExtensions.NumberOfLeadingZeros(0xF000000));
            assertEquals(3, IntegralNumberExtensions.NumberOfLeadingZeros(0x10000000));
            assertEquals(0, IntegralNumberExtensions.NumberOfLeadingZeros(unchecked((int)0x80000000)));
            assertEquals(0, IntegralNumberExtensions.NumberOfLeadingZeros(unchecked((int)0xF0000000)));

            assertEquals(1, IntegralNumberExtensions.NumberOfLeadingZeros(int.MaxValue));
            assertEquals(0, IntegralNumberExtensions.NumberOfLeadingZeros(int.MinValue));
        }

        /**
         * @tests java.lang.Integer#numberOfTrailingZeros(int)
         */
        [Test]
        public void Test_NumberOfTrailingZeros_Int32()
        {
            assertEquals(32, IntegralNumberExtensions.NumberOfTrailingZeros(0x0));
            assertEquals(31, IntegralNumberExtensions.NumberOfTrailingZeros(int.MinValue));
            assertEquals(0, IntegralNumberExtensions.NumberOfTrailingZeros(int.MaxValue));

            assertEquals(0, IntegralNumberExtensions.NumberOfTrailingZeros(0x1));
            assertEquals(3, IntegralNumberExtensions.NumberOfTrailingZeros(0x8));
            assertEquals(0, IntegralNumberExtensions.NumberOfTrailingZeros(0xF));

            assertEquals(4, IntegralNumberExtensions.NumberOfTrailingZeros(0x10));
            assertEquals(7, IntegralNumberExtensions.NumberOfTrailingZeros(0x80));
            assertEquals(4, IntegralNumberExtensions.NumberOfTrailingZeros(0xF0));

            assertEquals(8, IntegralNumberExtensions.NumberOfTrailingZeros(0x100));
            assertEquals(11, IntegralNumberExtensions.NumberOfTrailingZeros(0x800));
            assertEquals(8, IntegralNumberExtensions.NumberOfTrailingZeros(0xF00));

            assertEquals(12, IntegralNumberExtensions.NumberOfTrailingZeros(0x1000));
            assertEquals(15, IntegralNumberExtensions.NumberOfTrailingZeros(0x8000));
            assertEquals(12, IntegralNumberExtensions.NumberOfTrailingZeros(0xF000));

            assertEquals(16, IntegralNumberExtensions.NumberOfTrailingZeros(0x10000));
            assertEquals(19, IntegralNumberExtensions.NumberOfTrailingZeros(0x80000));
            assertEquals(16, IntegralNumberExtensions.NumberOfTrailingZeros(0xF0000));

            assertEquals(20, IntegralNumberExtensions.NumberOfTrailingZeros(0x100000));
            assertEquals(23, IntegralNumberExtensions.NumberOfTrailingZeros(0x800000));
            assertEquals(20, IntegralNumberExtensions.NumberOfTrailingZeros(0xF00000));

            assertEquals(24, IntegralNumberExtensions.NumberOfTrailingZeros(0x1000000));
            assertEquals(27, IntegralNumberExtensions.NumberOfTrailingZeros(0x8000000));
            assertEquals(24, IntegralNumberExtensions.NumberOfTrailingZeros(0xF000000));

            assertEquals(28, IntegralNumberExtensions.NumberOfTrailingZeros(0x10000000));
            assertEquals(31, IntegralNumberExtensions.NumberOfTrailingZeros(unchecked((int)0x80000000)));
            assertEquals(28, IntegralNumberExtensions.NumberOfTrailingZeros(unchecked((int)0xF0000000)));
        }




        /**
         * @tests java.lang.Long#numberOfLeadingZeros(long)
         */
        [Test]
        public void Test_NumberOfLeadingZeros_Int64()
        {
            assertEquals(64L, IntegralNumberExtensions.NumberOfLeadingZeros(0x0L));
            assertEquals(63L, IntegralNumberExtensions.NumberOfLeadingZeros(0x1L));
            assertEquals(62L, IntegralNumberExtensions.NumberOfLeadingZeros(0x2L));
            assertEquals(62L, IntegralNumberExtensions.NumberOfLeadingZeros(0x3L));
            assertEquals(61L, IntegralNumberExtensions.NumberOfLeadingZeros(0x4L));
            assertEquals(61L, IntegralNumberExtensions.NumberOfLeadingZeros(0x5L));
            assertEquals(61L, IntegralNumberExtensions.NumberOfLeadingZeros(0x6L));
            assertEquals(61L, IntegralNumberExtensions.NumberOfLeadingZeros(0x7L));
            assertEquals(60L, IntegralNumberExtensions.NumberOfLeadingZeros(0x8L));
            assertEquals(60L, IntegralNumberExtensions.NumberOfLeadingZeros(0x9L));
            assertEquals(60L, IntegralNumberExtensions.NumberOfLeadingZeros(0xAL));
            assertEquals(60L, IntegralNumberExtensions.NumberOfLeadingZeros(0xBL));
            assertEquals(60L, IntegralNumberExtensions.NumberOfLeadingZeros(0xCL));
            assertEquals(60L, IntegralNumberExtensions.NumberOfLeadingZeros(0xDL));
            assertEquals(60L, IntegralNumberExtensions.NumberOfLeadingZeros(0xEL));
            assertEquals(60L, IntegralNumberExtensions.NumberOfLeadingZeros(0xFL));
            assertEquals(59L, IntegralNumberExtensions.NumberOfLeadingZeros(0x10L));
            assertEquals(56L, IntegralNumberExtensions.NumberOfLeadingZeros(0x80L));
            assertEquals(56L, IntegralNumberExtensions.NumberOfLeadingZeros(0xF0L));
            assertEquals(55L, IntegralNumberExtensions.NumberOfLeadingZeros(0x100L));
            assertEquals(52L, IntegralNumberExtensions.NumberOfLeadingZeros(0x800L));
            assertEquals(52L, IntegralNumberExtensions.NumberOfLeadingZeros(0xF00L));
            assertEquals(51L, IntegralNumberExtensions.NumberOfLeadingZeros(0x1000L));
            assertEquals(48L, IntegralNumberExtensions.NumberOfLeadingZeros(0x8000L));
            assertEquals(48L, IntegralNumberExtensions.NumberOfLeadingZeros(0xF000L));
            assertEquals(47L, IntegralNumberExtensions.NumberOfLeadingZeros(0x10000L));
            assertEquals(44L, IntegralNumberExtensions.NumberOfLeadingZeros(0x80000L));
            assertEquals(44L, IntegralNumberExtensions.NumberOfLeadingZeros(0xF0000L));
            assertEquals(43L, IntegralNumberExtensions.NumberOfLeadingZeros(0x100000L));
            assertEquals(40L, IntegralNumberExtensions.NumberOfLeadingZeros(0x800000L));
            assertEquals(40L, IntegralNumberExtensions.NumberOfLeadingZeros(0xF00000L));
            assertEquals(39L, IntegralNumberExtensions.NumberOfLeadingZeros(0x1000000L));
            assertEquals(36L, IntegralNumberExtensions.NumberOfLeadingZeros(0x8000000L));
            assertEquals(36L, IntegralNumberExtensions.NumberOfLeadingZeros(0xF000000L));
            assertEquals(35L, IntegralNumberExtensions.NumberOfLeadingZeros(0x10000000L));
            assertEquals(32L, IntegralNumberExtensions.NumberOfLeadingZeros(0x80000000L)); // J2N: Changed test to match observed behavior in JDK (32L rather than 0L)
            assertEquals(32L, IntegralNumberExtensions.NumberOfLeadingZeros(0xF0000000L)); // J2N: Changed test to match observed behavior in JDK (32L rather than 0L)

            assertEquals(1L, IntegralNumberExtensions.NumberOfLeadingZeros(long.MaxValue));
            assertEquals(0L, IntegralNumberExtensions.NumberOfLeadingZeros(long.MinValue));
        }

        /**
         * @tests java.lang.Long#numberOfTrailingZeros(long)
         */
        [Test]
        public void Test_NumberOfTrailingZeros_Int64()
        {
            assertEquals(64L, IntegralNumberExtensions.NumberOfTrailingZeros(0x0L));
            assertEquals(63L, IntegralNumberExtensions.NumberOfTrailingZeros(long.MinValue));
            assertEquals(0L, IntegralNumberExtensions.NumberOfTrailingZeros(long.MaxValue));

            assertEquals(0L, IntegralNumberExtensions.NumberOfTrailingZeros(0x1L));
            assertEquals(3L, IntegralNumberExtensions.NumberOfTrailingZeros(0x8L));
            assertEquals(0L, IntegralNumberExtensions.NumberOfTrailingZeros(0xFL));

            assertEquals(4L, IntegralNumberExtensions.NumberOfTrailingZeros(0x10L));
            assertEquals(7L, IntegralNumberExtensions.NumberOfTrailingZeros(0x80L));
            assertEquals(4L, IntegralNumberExtensions.NumberOfTrailingZeros(0xF0L));

            assertEquals(8L, IntegralNumberExtensions.NumberOfTrailingZeros(0x100L));
            assertEquals(11L, IntegralNumberExtensions.NumberOfTrailingZeros(0x800L));
            assertEquals(8L, IntegralNumberExtensions.NumberOfTrailingZeros(0xF00L));

            assertEquals(12L, IntegralNumberExtensions.NumberOfTrailingZeros(0x1000L));
            assertEquals(15L, IntegralNumberExtensions.NumberOfTrailingZeros(0x8000L));
            assertEquals(12L, IntegralNumberExtensions.NumberOfTrailingZeros(0xF000L));

            assertEquals(16L, IntegralNumberExtensions.NumberOfTrailingZeros(0x10000L));
            assertEquals(19L, IntegralNumberExtensions.NumberOfTrailingZeros(0x80000L));
            assertEquals(16L, IntegralNumberExtensions.NumberOfTrailingZeros(0xF0000L));

            assertEquals(20L, IntegralNumberExtensions.NumberOfTrailingZeros(0x100000L));
            assertEquals(23L, IntegralNumberExtensions.NumberOfTrailingZeros(0x800000L));
            assertEquals(20L, IntegralNumberExtensions.NumberOfTrailingZeros(0xF00000L));

            assertEquals(24L, IntegralNumberExtensions.NumberOfTrailingZeros(0x1000000L));
            assertEquals(27L, IntegralNumberExtensions.NumberOfTrailingZeros(0x8000000L));
            assertEquals(24L, IntegralNumberExtensions.NumberOfTrailingZeros(0xF000000L));

            assertEquals(28L, IntegralNumberExtensions.NumberOfTrailingZeros(0x10000000L));
            assertEquals(31L, IntegralNumberExtensions.NumberOfTrailingZeros(0x80000000L));
            assertEquals(28L, IntegralNumberExtensions.NumberOfTrailingZeros(0xF0000000L));
        }

        /**
         * @tests java.lang.Integer#rotateLeft(int,int)
         */
        [Test]
        public void Test_RotateLeft_Int32()
        {
            assertEquals(0xF, IntegralNumberExtensions.RotateLeft(0xF, 0));
            assertEquals(0xF0, IntegralNumberExtensions.RotateLeft(0xF, 4));
            assertEquals(0xF00, IntegralNumberExtensions.RotateLeft(0xF, 8));
            assertEquals(0xF000, IntegralNumberExtensions.RotateLeft(0xF, 12));
            assertEquals(0xF0000, IntegralNumberExtensions.RotateLeft(0xF, 16));
            assertEquals(0xF00000, IntegralNumberExtensions.RotateLeft(0xF, 20));
            assertEquals(0xF000000, IntegralNumberExtensions.RotateLeft(0xF, 24));
            unchecked
            {
                assertEquals((int)0xF0000000, IntegralNumberExtensions.RotateLeft(0xF, 28));
                assertEquals((int)0xF0000000, IntegralNumberExtensions.RotateLeft((int)0xF0000000, 32));
            }
        }

        /**
         * @tests java.lang.Long#rotateLeft(long,long)
         */
        [Test]
        public void Test_RotateLeft_Int64()
        {
            assertEquals(0xF, IntegralNumberExtensions.RotateLeft(0xFL, 0));
            assertEquals(0xF0, IntegralNumberExtensions.RotateLeft(0xFL, 4));
            assertEquals(0xF00, IntegralNumberExtensions.RotateLeft(0xFL, 8));
            assertEquals(0xF000, IntegralNumberExtensions.RotateLeft(0xFL, 12));
            assertEquals(0xF0000, IntegralNumberExtensions.RotateLeft(0xFL, 16));
            assertEquals(0xF00000, IntegralNumberExtensions.RotateLeft(0xFL, 20));
            assertEquals(0xF000000, IntegralNumberExtensions.RotateLeft(0xFL, 24));
            assertEquals(0xF0000000L, IntegralNumberExtensions.RotateLeft(0xFL, 28));
            unchecked
            {
                assertEquals((long)0xF000000000000000L, IntegralNumberExtensions.RotateLeft((long)0xF000000000000000L, 64));
            }
        }

        /**
         * @tests java.lang.Integer#rotateRight(int,int)
         */
        [Test]
        public void Test_RotateRight_Int32()
        {
            assertEquals(0xF, IntegralNumberExtensions.RotateRight(0xF0, 4));
            assertEquals(0xF, IntegralNumberExtensions.RotateRight(0xF00, 8));
            assertEquals(0xF, IntegralNumberExtensions.RotateRight(0xF000, 12));
            assertEquals(0xF, IntegralNumberExtensions.RotateRight(0xF0000, 16));
            assertEquals(0xF, IntegralNumberExtensions.RotateRight(0xF00000, 20));
            assertEquals(0xF, IntegralNumberExtensions.RotateRight(0xF000000, 24));
            unchecked
            {
                assertEquals(0xF, IntegralNumberExtensions.RotateRight((int)0xF0000000, 28));
                assertEquals((int)0xF0000000, IntegralNumberExtensions.RotateRight((int)0xF0000000, 32));
                assertEquals((int)0xF0000000, IntegralNumberExtensions.RotateRight((int)0xF0000000, 0));
            }

        }

        /**
         * @tests java.lang.Long#rotateRight(long,long)
         */
        [Test]
        public void Test_RotateRight_Int64()
        {
            assertEquals(0xFL, IntegralNumberExtensions.RotateRight(0xF0L, 4));
            assertEquals(0xFL, IntegralNumberExtensions.RotateRight(0xF00L, 8));
            assertEquals(0xFL, IntegralNumberExtensions.RotateRight(0xF000L, 12));
            assertEquals(0xFL, IntegralNumberExtensions.RotateRight(0xF0000L, 16));
            assertEquals(0xFL, IntegralNumberExtensions.RotateRight(0xF00000L, 20));
            assertEquals(0xFL, IntegralNumberExtensions.RotateRight(0xF000000L, 24));
            assertEquals(0xFL, IntegralNumberExtensions.RotateRight(0xF0000000L, 28));
            unchecked
            {
                assertEquals((long)0xF000000000000000L, IntegralNumberExtensions.RotateRight((long)0xF000000000000000L, 64));
                assertEquals((long)0xF000000000000000L, IntegralNumberExtensions.RotateRight((long)0xF000000000000000L, 0));
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
                assertEquals((int)0xAABBCCDD, IntegralNumberExtensions.ReverseBytes((int)0xDDCCBBAA));
            }
            assertEquals(0x11223344, IntegralNumberExtensions.ReverseBytes(0x44332211));
            assertEquals(0x00112233, IntegralNumberExtensions.ReverseBytes(0x33221100));
            assertEquals(0x20000002, IntegralNumberExtensions.ReverseBytes(0x02000020));
        }

        /**
         * @tests java.lang.Long#reverseBytes(long)
         */
        [Test]
        public void Test_ReverseBytes_Int64()
        {
            unchecked
            {
                assertEquals((long)0xAABBCCDD00112233L, IntegralNumberExtensions.ReverseBytes(0x33221100DDCCBBAAL));
                assertEquals(0x1122334455667788L, IntegralNumberExtensions.ReverseBytes((long)0x8877665544332211L));
            }
            assertEquals(0x0011223344556677L, IntegralNumberExtensions.ReverseBytes(0x7766554433221100L));
            assertEquals(0x2000000000000002L, IntegralNumberExtensions.ReverseBytes(0x0200000000000020L));
        }


        /**
         * @tests java.lang.Integer#reverse(int)
         */
        [Test]
        public void Test_Reverse_Int32()
        {
            assertEquals(-1, IntegralNumberExtensions.Reverse(-1));
            assertEquals(unchecked((int)0x80000000), IntegralNumberExtensions.Reverse(1));
        }

        /**
         * @tests java.lang.Long#reverse(long)
         */
        [Test]
        public void Test_Reverse_Int64()
        {
            assertEquals(0L, IntegralNumberExtensions.Reverse(0L));
            assertEquals(-1L, IntegralNumberExtensions.Reverse(-1L));
            assertEquals(unchecked((long)0x8000000000000000L), IntegralNumberExtensions.Reverse(1L));
        }


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
