using System;
using System.Runtime.CompilerServices;

namespace J2N
{
    /// <summary>
    /// Extensions to integral numbers such as <see cref="int"/> or <see cref="long"/>.
    /// </summary>
    public static class IntegralNumberExtensions
    {
        #region BitCount

        /// <summary>
        /// Returns the number of one-bits in the two's complement binary
        /// representation of the specified <paramref name="value"/>. This function is
        /// sometimes referred to as the <i>population count</i>.
        /// </summary>
        /// <param name="value">The <see cref="int"/> to examine.</param>
        /// <returns>The number of one-bits in the two's complement binary
        /// representation of the specified <paramref name="value"/>.</returns>
        public static int BitCount(this int value)
        {
            // Hacker's Delight, Figure 5-2
            value -= ((value.TripleShift(1)) & 0x55555555);
            value = (value & 0x33333333) + ((value.TripleShift(2)) & 0x33333333);
            value = (value + (value.TripleShift(4))) & 0x0f0f0f0f;
            value += (value.TripleShift(8));
            value += (value.TripleShift(16));
            return value & 0x3f;
        }

        /// <summary>
        /// Returns the number of one-bits in the two's complement binary
        /// representation of the specified <paramref name="value"/>. This function is
        /// sometimes referred to as the <i>population count</i>.
        /// </summary>
        /// <param name="value">The <see cref="long"/> to examine.</param>
        /// <returns>The number of one-bits in the two's complement binary
        /// representation of the specified <paramref name="value"/>.</returns>
        public static int BitCount(this long value)
        {
            // Hacker's Delight, Figure 5-14
            value -= ((value.TripleShift(1)) & 0x5555555555555555L);
            value = (value & 0x3333333333333333L) + ((value.TripleShift(2)) & 0x3333333333333333L);
            value = (value + (value.TripleShift(4))) & 0x0f0f0f0f0f0f0f0fL;
            value += (value.TripleShift(8));
            value += (value.TripleShift(16));
            value += (value.TripleShift(32));
            return (int)value & 0x7f;
        }

        #endregion BitCount

        #region NumberOfLeadingZeros

        /// <summary>
        /// Returns the number of zero bits preceding the highest-order
        /// ("leftmost") one-bit in the two's complement binary representation
        /// of the specified <paramref name="value"/>. Returns 32 if the
        /// specified value has no one-bits in its two's complement representation,
        /// in other words if it is equal to zero.
        /// <para/>
        /// Note that this method is closely related to the logarithm base 2.
        /// For all positive <see cref="int"/> values x:
        /// <list type="bullet">
        ///     <item><description>floor(log<sub>2</sub>(x)) = <c>31 - x.NumberOfLeadingZeros()</c></description></item>
        ///     <item><description>ceil(log<sub>2</sub>(x)) = <c>32 - (x - 1).NumberOfLeadingZeros()</c></description></item>
        /// </list>
        /// </summary>
        /// <param name="value">The <see cref="int"/> to examine.</param>
        /// <returns>The number of zero bits preceding the highest-order
        /// ("leftmost") one-bit in the two's complement binary representation
        /// of the specified <paramref name="value"/>, or 32 if the value
        /// is equal to zero.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int NumberOfLeadingZeros(this int value)
        {
            // Hacker's Delight, Figure 5-6
            if (value == 0)
                return 32;
            value |= value >> 1;
            value |= value >> 2;
            value |= value >> 4;
            value |= value >> 8;
            value |= value >> 16;
            return BitCount(~value);
        }

        /// <summary>
        /// Returns the number of zero bits preceding the highest-order
        /// ("leftmost") one-bit in the two's complement binary representation
        /// of the specified <paramref name="value"/>. Returns 64 if the
        /// specified value has no one-bits in its two's complement representation,
        /// in other words if it is equal to zero.
        /// <para/>
        /// Note that this method is closely related to the logarithm base 2.
        /// For all positive <see cref="long"/> values x:
        /// <list type="bullet">
        ///     <item><description>floor(log<sub>2</sub>(x)) = <c>63 - x.NumberOfLeadingZeros()</c></description></item>
        ///     <item><description>ceil(log<sub>2</sub>(x)) = <c>64 - (x - 1).NumberOfLeadingZeros()</c></description></item>
        /// </list>
        /// </summary>
        /// <param name="value">The <see cref="long"/> to examine.</param>
        /// <returns>The number of zero bits preceding the highest-order
        /// ("leftmost") one-bit in the two's complement binary representation
        /// of the specified <paramref name="value"/>, or 64 if the value
        /// is equal to zero.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int NumberOfLeadingZeros(this long value)
        {
            // Hacker's Delight, Figure 5-6
            if (value == 0)
                return 64;
            value |= value >> 1;
            value |= value >> 2;
            value |= value >> 4;
            value |= value >> 8;
            value |= value >> 16;
            value |= value >> 32;
            return BitCount(~value);
        }

        #endregion NumberOfLeadingZeros

        #region NumberOfTrailingZeros

        /// <summary>
        /// Returns the number of zero bits following the lowest-order ("rightmost")
        /// one-bit in the two's complement binary representation of the specified
        /// <paramref name="value"/>. Returns 32 if the specified value has no
        /// one-bits in its two's complement representation, in other words if it is
        /// equal to zero.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>the number of zero bits following the lowest-order ("rightmost")
        /// one-bit in the two's complement binary representation of the
        /// specified <paramref name="value"/>, or 32 if the value is equal
        /// to zero.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int NumberOfTrailingZeros(this int value)
        {
            if (value == 0)
                return 32;
            return BitCount((value & -value) - 1);
        }

        /// <summary>
        /// Returns the number of zero bits following the lowest-order ("rightmost")
        /// one-bit in the two's complement binary representation of the specified
        /// <paramref name="value"/>. Returns 64 if the specified value has no
        /// one-bits in its two's complement representation, in other words if it is
        /// equal to zero.
        /// </summary>
        /// <param name="value">The <see cref="long"/> to examine.</param>
        /// <returns>The number of zero bits following the lowest-order ("rightmost")
        /// one-bit in the two's complement binary representation of the
        /// specified <paramref name="value"/>, or 64 if the value is equal
        /// to zero.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int NumberOfTrailingZeros(this long value)
        {
            if (value == 0)
                return 64;
            return BitCount((value & -value) - 1);
        }

        #endregion NumberOfTrailingZeros

        #region HighestOneBit

        /// <summary>
        /// Returns an <see cref="int"/> value with at most a single one-bit, in the
        /// position of the highest-order ("leftmost") one-bit in the specified
        /// <paramref name="value"/>. Returns zero if the specified value has no
        /// one-bits in its two's complement binary representation, that is, if it
        /// is equal to zero.
        /// </summary>
        /// <param name="value">The <see cref="int"/> to examine.</param>
        /// <returns>An <see cref="int"/> value with a single one-bit, in the position
        /// of the highest-order one-bit in the specified <paramref name="value"/>, or zero if
        /// the specified <paramref name="value"/> is itself equal to zero.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int HighestOneBit(this int value) // Harmony
        {
            value |= (value >> 1);
            value |= (value >> 2);
            value |= (value >> 4);
            value |= (value >> 8);
            value |= (value >> 16);
            return (value & ~(value.TripleShift(1)));
        }

        /// <summary>
        /// Returns a <see cref="long"/> value with at most a single one-bit, in the
        /// position of the highest-order ("leftmost") one-bit in the specified
        /// <paramref name="value"/>.  Returns zero if the specified <paramref name="value"/> has no
        /// one-bits in its two's complement binary representation, that is, if it
        /// is equal to zero.
        /// </summary>
        /// <param name="value">The <see cref="long"/> to examine.</param>
        /// <returns>A <see cref="long"/> value with a single one-bit, in the position
        /// of the highest-order one-bit in the specified <paramref name="value"/>, or zero if
        /// the specified <paramref name="value"/> is itself equal to zero.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long HighestOneBit(this long value)
        {
            value |= (value >> 1);
            value |= (value >> 2);
            value |= (value >> 4);
            value |= (value >> 8);
            value |= (value >> 16);
            value |= (value >> 32);
            return (value & ~(value.TripleShift(1)));
        }

        #endregion HighestOneBit

        #region LowestOneBit

        /// <summary>
        /// Returns an <see cref="int"/> value with at most a single one-bit, in the
        /// position of the lowest-order ("rightmost") one-bit in the specified
        /// <paramref name="value"/>. Returns zero if the specified value has no
        /// one-bits in its two's complement binary representation, that is, if it
        /// is equal to zero.
        /// </summary>
        /// <param name="value">The <see cref="int"/> to examine.</param>
        /// <returns>An <see cref="int"/> value with a single one-bit, in the position
        /// of the lowest-order one-bit in the specified <paramref name="value"/>, or zero if
        /// the specified <paramref name="value"/> is itself equal to zero.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int LowestOneBit(this int value)
        {
            // From Hacker's Delight, Section 2-1
            return value & -value;
        }

        /// <summary>
        /// Returns a <see cref="long"/> value with at most a single one-bit, in the
        /// position of the lowest-order ("rightmost") one-bit in the specified
        /// <see cref="long"/> value.  Returns zero if the specified value has no
        /// one-bits in its two's complement binary representation, that is, if it
        /// is equal to zero.
        /// </summary>
        /// <param name="value">The <see cref="long"/> to examine.</param>
        /// <returns>A <see cref="long"/> value with a single one-bit, in the position
        /// of the lowest-order one-bit in the specified <paramref name="value"/>, or zero if
        /// the specified <paramref name="value"/> is itself equal to zero.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long LowestOneBit(this long value)
        {
            // From Hacker's Delight, Section 2-1
            return value & -value;
        }

        #endregion LowestOneBit

        #region Reverse

        /// <summary>
        /// Returns the value obtained by reversing the order of the bits in the
        /// two's complement binary representation of the specified <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The <see cref="int"/> value for which to reverse the bit order.</param>
        /// <returns>The value obtained by reversing order of the bits in the
        /// specified <paramref name="value"/>.</returns>
        public static int Reverse(this int value)
        {
            // From Hacker's Delight, 7-1, Figure 7-1
            value = (value & 0x55555555) << 1 | (value.TripleShift(1)) & 0x55555555;
            value = (value & 0x33333333) << 2 | (value.TripleShift(2)) & 0x33333333;
            value = (value & 0x0f0f0f0f) << 4 | (value.TripleShift(4)) & 0x0f0f0f0f;
            value = (value << 24) | ((value & 0xff00) << 8) |
                ((value.TripleShift(8)) & 0xff00) | (value.TripleShift(24));
            return value;
        }

        /// <summary>
        /// Returns the value obtained by reversing the order of the bits in the
        /// two's complement binary representation of the specified <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The <see cref="long"/> value for which to reverse the bit order.</param>
        /// <returns>The value obtained by reversing order of the bits in the
        /// specified <paramref name="value"/>.</returns>
        public static long Reverse(this long value)
        {
            // From Hacker's Delight, 7-1, Figure 7-1
            value = (value & 0x5555555555555555L) << 1 | (value.TripleShift(1)) & 0x5555555555555555L;
            value = (value & 0x3333333333333333L) << 2 | (value.TripleShift(2)) & 0x3333333333333333L;
            value = (value & 0x0f0f0f0f0f0f0f0fL) << 4 | (value.TripleShift(4)) & 0x0f0f0f0f0f0f0f0fL;
            value = (value & 0x00ff00ff00ff00ffL) << 8 | (value.TripleShift(8)) & 0x00ff00ff00ff00ffL;
            value = (value << 48) | ((value & 0xffff0000L) << 16) |
                ((value.TripleShift(16)) & 0xffff0000L) | (value.TripleShift(48));
            return value;
        }

        #endregion

        #region ReverseBytes

        /// <summary>
        /// Returns the value obtained by reversing the order of the bytes in the
        /// two's complement representation of the specified <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The <see cref="int"/> value for which to reverse the byte order.</param>
        /// <returns>The value obtained by reversing the bytes in the specified
        /// <paramref name="value"/>.</returns>
        public static int ReverseBytes(this int value)
        {
            return ((value.TripleShift(24))) |
                   ((value >> 8) & 0xFF00) |
                   ((value << 8) & 0xFF0000) |
                   ((value << 24));
        }

        /// <summary>
        /// Returns the value obtained by reversing the order of the bytes in the
        /// two's complement representation of the specified <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The <see cref="long"/> value for which to reverse the byte order.</param>
        /// <returns>The value obtained by reversing the bytes in the specified
        /// <paramref name="value"/>.</returns>
        public static long ReverseBytes(this long value)
        {
            value = (value & 0x00ff00ff00ff00ffL) << 8 |
                (value.TripleShift(8)) & 0x00ff00ff00ff00ffL;
            return (value << 48) |
                ((value & 0xffff0000L) << 16) |
                ((value.TripleShift(16)) & 0xffff0000L) |
                (value.TripleShift(48));
        }

        #endregion

        #region RotateLeft

        /// <summary>
        /// Returns the value obtained by rotating the two's complement binary
        /// representation of the specified <paramref name="value"/> left by the
        /// specified number of bits.  (Bits shifted out of the left hand, or
        /// high-order, side reenter on the right, or low-order.)
        /// <para/>
        /// Note that left rotation with a negative distance is equivalent to
        /// right rotation: <c>val.RotateLeft(-distance) == val.RotateRight(distance)</c>.
        /// Note also that rotation by any multiple of 32 is a
        /// no-op, so all but the last five bits of the rotation distance can be
        /// ignored, even if the distance is negative:
        /// <c>val.RotateLeft(distance) == val.RotateLeft(distance &amp; 0x1F)</c>.
        /// </summary>
        /// <param name="value">The <see cref="int"/> value to rotate left.</param>
        /// <param name="distance">The number of bits to rotate.</param>
        /// <returns>The value obtained by rotating the two's complement binary
        /// representation of the specified <paramref name="value"/> left by the
        /// specified number of bits.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int RotateLeft(this int value, int distance)
        {
            if (distance == 0)
            {
                return value;
            }
            /*
             * According to JLS3, 15.19, the right operand of a shift is always
             * implicitly masked with 0x1F, which the negation of 'distance' is
             * taking advantage of.
             */
            return ((value << distance) | (value.TripleShift(-distance)));
        }

        /// <summary>
        /// Returns the value obtained by rotating the two's complement binary
        /// representation of the specified <paramref name="value"/> left by the
        /// specified number of bits.  (Bits shifted out of the left hand, or
        /// high-order, side reenter on the right, or low-order.)
        /// <para/>
        /// Note that left rotation with a negative distance is equivalent to
        /// right rotation: <c>val.RotateLeft(-distance) == val.RotateRight(distance)</c>
        /// Note also that rotation by any multiple of 64 is a
        /// no-op, so all but the last six bits of the rotation distance can be
        /// ignored, even if the distance is negative:
        /// <c>val.RotateLeft(distance) == val.RotateLeft(distance &amp; 0x3F)</c>.
        /// </summary>
        /// <param name="value">The <see cref="long"/> value to rotate left.</param>
        /// <param name="distance">The number of bits to rotate.</param>
        /// <returns>The value obtained by rotating the two's complement binary
        /// representation of the specified <paramref name="value"/> left by the
        /// specified number of bits.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long RotateLeft(this long value, int distance)
        {
            if (distance == 0)
            {
                return value;
            }
            /*
             * According to JLS3, 15.19, the right operand of a shift is always
             * implicitly masked with 0x3F, which the negation of 'distance' is
             * taking advantage of.
             */
            return ((value << distance) | (value.TripleShift(-distance)));
        }

        #endregion

        #region RotateRight

        /// <summary>
        /// Returns the value obtained by rotating the two's complement binary
        /// representation of the specified <paramref name="value"/> right by the
        /// specified number of bits.  (Bits shifted out of the right hand, or
        /// low-order, side reenter on the left, or high-order.)
        /// <para/>
        /// Note that right rotation with a negative distance is equivalent to
        /// left rotation: <c>val.RotateRight(-distance) == val.RotateLeft(distance)</c>.
        /// Note also that rotation by any multiple of 32 is a
        /// no-op, so all but the last five bits of the rotation distance can be
        /// ignored, even if the distance is negative:
        /// <c>val.RotateRight(distance) == val.RotateLeft(distance &amp; 0x1F)</c>.
        /// </summary>
        /// <param name="value">The <see cref="int"/> value to rotate right.</param>
        /// <param name="distance">The number of bits to rotate.</param>
        /// <returns>The value obtained by rotating the two's complement binary
        /// representation of the specified <paramref name="value"/> right by the
        /// specified number of bits.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int RotateRight(this int value, int distance)
        {
            if (distance == 0)
            {
                return value;
            }
            /*
             * According to JLS3, 15.19, the right operand of a shift is always
             * implicitly masked with 0x1F, which the negation of 'distance' is
             * taking advantage of.
             */
            return ((value.TripleShift(distance)) | (value << (-distance)));
        }

        /// <summary>
        /// Returns the value obtained by rotating the two's complement binary
        /// representation of the specified <see cref="long"/> value right by the
        /// specified number of bits.  (Bits shifted out of the right hand, or
        /// low-order, side reenter on the left, or high-order.)
        /// <para/>
        /// Note that right rotation with a negative distance is equivalent to
        /// left rotation: <c>val.RotateRight(-distance) == val.RotateLeft(distance)</c>.
        /// Note also that rotation by any multiple of 64 is a
        /// no-op, so all but the last six bits of the rotation distance can be
        /// ignored, even if the distance is negative: 
        /// <c>val.RotateRight(distance) == val.RotateRight(distance &amp; 0x3F)</c>.
        /// </summary>
        /// <param name="value">The <see cref="long"/> value to rotate right.</param>
        /// <param name="distance">The number of bits to rotate.</param>
        /// <returns>The value obtained by rotating the two's complement binary
        /// representation of the specified <paramref name="value"/> right by the
        /// specified number of bits.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long RotateRight(this long value, int distance)
        {
            if (distance == 0)
            {
                return value;
            }
            /*
             * According to JLS3, 15.19, the right operand of a shift is always
             * implicitly masked with 0x3F, which the negation of 'distance' is
             * taking advantage of.
             */
            return ((value.TripleShift(distance)) | (value << (-distance)));
        }

        #endregion

        #region ToBinaryString

        /// <summary>
        /// Converts the specified <see cref="char"/> into its binary string representation. The
        /// returned string is a concatenation of '0' and '1' characters.
        /// </summary>
        /// <param name="value">The <see cref="char"/> to convert</param>
        /// <returns>The binary string representation of <paramref name="value"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToBinaryString(this char value) => Convert.ToString(value, 2);

        /// <summary>
        /// Converts the specified <see cref="short"/> into its binary string representation. The
        /// returned string is a concatenation of '0' and '1' characters.
        /// </summary>
        /// <param name="value">The <see cref="short"/> to convert</param>
        /// <returns>The binary string representation of <paramref name="value"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToBinaryString(this short value)
        {
            if (value > 0)
                return Convert.ToString(value, 2);
            return Convert.ToString((int)value, 2);
        }

        /// <summary>
        /// Converts the specified <see cref="int"/> into its binary string representation. The
        /// returned string is a concatenation of '0' and '1' characters.
        /// </summary>
        /// <param name="value">The <see cref="int"/> to convert</param>
        /// <returns>The binary string representation of <paramref name="value"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToBinaryString(this int value) => Convert.ToString(value, 2);

        /// <summary>
        /// Converts the specified <see cref="long"/> into its binary string representation. The
        /// returned string is a concatenation of '0' and '1' characters.
        /// </summary>
        /// <param name="value">The <see cref="long"/> to convert</param>
        /// <returns>The binary string representation of <paramref name="value"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToBinaryString(this long value) => Convert.ToString(value, 2);

        #endregion ToBinaryString

        #region ToHexString

        /// <summary>
        /// Converts the current <see cref="char"/> into its hexadecimal string
        /// representation (in lowercase). The returned string is a 
        /// concatenation of characters from '0' to '9' and 'a' to 'f'.
        /// </summary>
        /// <param name="value">The <see cref="char"/> to convert.</param>
        /// <returns>The hexadecimal string representation of <paramref name="value"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToHexString(this char value) => Convert.ToString(value, 16);

        /// <summary>
        /// Converts the current <see cref="short"/> into its hexadecimal string
        /// representation (in lowercase). The returned string is a 
        /// concatenation of characters from '0' to '9' and 'a' to 'f'.
        /// </summary>
        /// <param name="value">The <see cref="short"/> to convert.</param>
        /// <returns>The hexadecimal string representation of <paramref name="value"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToHexString(this short value)
        {
            if (value > 0)
                return Convert.ToString(value, 16);
            return Convert.ToString((int)value, 16);
        }

        /// <summary>
        /// Converts the current <see cref="int"/> into its hexadecimal string
        /// representation (in lowercase). The returned string is a 
        /// concatenation of characters from '0' to '9' and 'a' to 'f'.
        /// </summary>
        /// <param name="value">The <see cref="int"/> to convert.</param>
        /// <returns>The hexadecimal string representation of <paramref name="value"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToHexString(this int value) => Convert.ToString(value, 16);

        /// <summary>
        /// Converts the current <see cref="long"/> into its hexadecimal string
        /// representation (in lowercase). The returned string is a 
        /// concatenation of characters from '0' to '9' and 'a' to 'f'.
        /// </summary>
        /// <param name="value">The <see cref="long"/> to convert.</param>
        /// <returns>The hexadecimal string representation of <paramref name="value"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToHexString(this long value) => Convert.ToString(value, 16);

        #endregion

        #region ToOctalString

        /// <summary>
        /// Converts the specified <see cref="char"/> into its octal string representation. The
        /// returned string is a concatenation of characters from '0' to '7'.
        /// </summary>
        /// <param name="value">The <see cref="char"/> to convert.</param>
        /// <returns>The octal string representation of <paramref name="value"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToOctalString(this char value) => Convert.ToString(value, 8);

        /// <summary>
        /// Converts the specified <see cref="short"/> into its octal string representation. The
        /// returned string is a concatenation of characters from '0' to '7'.
        /// </summary>
        /// <param name="value">The <see cref="short"/> to convert.</param>
        /// <returns>The octal string representation of <paramref name="value"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToOctalString(this short value)
        {
            if (value > 0)
                return Convert.ToString(value, 8);
            return Convert.ToString((int)value, 8);
        }

        /// <summary>
        /// Converts the specified <see cref="int"/> into its octal string representation. The
        /// returned string is a concatenation of characters from '0' to '7'.
        /// </summary>
        /// <param name="value">The <see cref="int"/> to convert.</param>
        /// <returns>The octal string representation of <paramref name="value"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToOctalString(this int value) => Convert.ToString(value, 8);

        /// <summary>
        /// Converts the specified <see cref="long"/> into its octal string representation. The
        /// returned string is a concatenation of characters from '0' to '7'.
        /// </summary>
        /// <param name="value">The <see cref="long"/> to convert.</param>
        /// <returns>The octal string representation of <paramref name="value"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToOctalString(this long value) => Convert.ToString(value, 8);

        #endregion

        #region ToString (radix)

        /// <summary>
        /// Converts the specified <see cref="int"/> into a string representation based on the
        /// specified radix. The returned string is a concatenation of a minus sign
        /// if the number is negative and characters from '0' to '9' and 'a' to 'z',
        /// depending on the radix. If <paramref name="radix"/> is not in the interval defined
        /// by <see cref="Character.MinRadix"/> and <see cref="Character.MaxRadix"/> then 10 is
        /// used as the base for the conversion.
        /// </summary>
        /// <param name="value">The <see cref="int"/> to convert.</param>
        /// <param name="radix">The base to use for the conversion.</param>
        /// <returns>The string representation of <paramref name="value"/>.</returns>
        public static string ToString(this int value, int radix)
        {
            if (radix < Character.MinRadix || radix > Character.MaxRadix)
                radix = 10;
            if (value == 0)
                return "0"; //$NON-NLS-1$

            int count = 2, j = value;
            bool negative = value < 0;
            if (!negative)
            {
                count = 1;
                j = -value;
            }
            while ((value /= radix) != 0)
                count++;

            char[] buffer = new char[count];
            do
            {
                int ch = 0 - (j % radix);
                if (ch > 9)
                    ch = ch - 10 + 'a';
                else
                    ch += '0';
                buffer[--count] = (char)ch;
            } while ((j /= radix) != 0);
            if (negative)
            {
                buffer[0] = '-';
            }
            return new string(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// Converts the specified <see cref="long"/> value into a string representation based on
        /// the specified radix. The returned string is a concatenation of a minus
        /// sign if the number is negative and characters from '0' to '9' and 'a' to
        /// 'z', depending on the radix. If <paramref name="radix"/> is not in the interval
        /// defined by <see cref="Character.MinRadix"/> and <see cref="Character.MaxRadix"/>
        /// then 10 is used as the base for the conversion.
        /// </summary>
        /// <param name="value">The long to convert.</param>
        /// <param name="radix">The base to use for the conversion.</param>
        /// <returns>The string representation of <paramref name="value"/>.</returns>
        public static string ToString(this long value, int radix)
        {
            if (radix < Character.MinRadix || radix > Character.MaxRadix)
                radix = 10;
            if (value == 0)
                return "0"; //$NON-NLS-1$

            int count = 2;
            long j = value;
            bool negative = value < 0;
            if (!negative)
            {
                count = 1;
                j = -value;
            }
            while ((value /= radix) != 0)
                count++;

            char[] buffer = new char[count];
            do
            {
                int ch = 0 - (int)(j % radix);
                if (ch > 9)
                    ch = ch - 10 + 'a';
                else
                    ch += '0';
                buffer[--count] = (char)ch;
            } while ((j /= radix) != 0);
            if (negative)
                buffer[0] = '-';
            return new string(buffer, 0, buffer.Length);
        }

        #endregion ToString (radix)

        #region TripleShift

        /// <summary>
        /// Performs an unsigned bitwise right shift to the current <paramref name="number"/>.
        /// <para/>
        /// Usage Note: Replacement for Java triple shift (>>>) operator. The
        /// value is converted to <see cref="sbyte"/> first before doing the conversion,
        /// so the result is the same as it is for the Java byte, which is signed.
        /// </summary>
        /// <param name="number">Number to operate on.</param>
        /// <param name="bits">Ammount of bits to shift.</param>
        /// <returns>The resulting number from the shift operation as <see cref="int"/>.</returns>
        // See http://stackoverflow.com/a/6625912
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int TripleShift(this byte number, int bits)
        {
            return TripleShift((sbyte)number, bits);
        }

        /// <summary>
        /// Performs an unsigned bitwise right shift to the current <paramref name="number"/>.
        /// <para/>
        /// Usage Note: Replacement for Java triple shift (>>>) operator.
        /// </summary>
        /// <param name="number">Number to operate on.</param>
        /// <param name="bits">Ammount of bits to shift.</param>
        /// <returns>The resulting number from the shift operation as <see cref="int"/>.</returns>
        // See http://stackoverflow.com/a/6625912
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CLSCompliant(false)]
        public static int TripleShift(this sbyte number, int bits)
        {
            if (number >= 0)
                return number >> bits;
            return (number >> bits) + (2 << ~bits);
        }

        /// <summary>
        /// Performs an unsigned bitwise right shift to the current <paramref name="number"/>.
        /// <para/>
        /// Usage Note: Replacement for Java triple shift (>>>) operator.
        /// </summary>
        /// <param name="number">Number to operate on.</param>
        /// <param name="bits">Ammount of bits to shift.</param>
        /// <returns>The resulting number from the shift operation as <see cref="int"/>.</returns>
        // See http://stackoverflow.com/a/6625912
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int TripleShift(this char number, int bits)
        {
            if (number >= 0)
                return number >> bits;
            return (number >> bits) + (2 << ~bits);
        }

        /// <summary>
        /// Performs an unsigned bitwise right shift to the current <paramref name="number"/>.
        /// <para/>
        /// Usage Note: Replacement for Java triple shift (>>>) operator.
        /// </summary>
        /// <param name="number">Number to operate on.</param>
        /// <param name="bits">Ammount of bits to shift.</param>
        /// <returns>The resulting number from the shift operation as <see cref="short"/>.</returns>
        // See http://stackoverflow.com/a/6625912
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int TripleShift(this short number, int bits)
        {
            if (number >= 0)
                return number >> bits;
            return (number >> bits) + (2 << ~bits);
        }

        /// <summary>
        /// Performs an unsigned bitwise right shift to the current <paramref name="number"/>.
        /// <para/>
        /// Usage Note: Replacement for Java triple shift (>>>) operator.
        /// </summary>
        /// <param name="number">Number to operate on.</param>
        /// <param name="bits">Ammount of bits to shift.</param>
        /// <returns>The resulting number from the shift operation as <see cref="int"/>.</returns>
        // See http://stackoverflow.com/a/6625912
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int TripleShift(this int number, int bits)
        {
            if (number >= 0)
                return number >> bits;
            return (number >> bits) + (2 << ~bits);
        }

        /// <summary>
        /// Performs an unsigned bitwise right shift to the current <paramref name="number"/>.
        /// <para/>
        /// Usage Note: Replacement for Java triple shift (>>>) operator.
        /// </summary>
        /// <param name="number">Number to operate on.</param>
        /// <param name="bits">Ammount of bits to shift.</param>
        /// <returns>The resulting number from the shift operation as <see cref="long"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long TripleShift(this long number, int bits)
        {
            return (long)((ulong)number >> bits);
        }

        #endregion
    }
}
