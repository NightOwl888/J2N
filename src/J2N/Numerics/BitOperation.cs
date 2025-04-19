#region Copyright 2010 by Apache Harmony, Licensed under the Apache License, Version 2.0
/*  Licensed to the Apache Software Foundation (ASF) under one or more
 *  contributor license agreements.  See the NOTICE file distributed with
 *  this work for additional information regarding copyright ownership.
 *  The ASF licenses this file to You under the Apache License, Version 2.0
 *  (the "License"); you may not use this file except in compliance with
 *  the License.  You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 *  Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *  limitations under the License.
 */
#endregion

using System;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;


namespace J2N.Numerics
{
    /// <summary>
    /// Extensions to <see cref="int"/> and <see cref="long"/> to perform bitwise
    /// operations.
    /// </summary>
    public static class BitOperation
    {
        #region BitCount

        /// <summary>
        /// Returns the population count (number of set bits) of an <see cref="int"/> mask.
        /// <para/>
        /// Usage Note: This is the same operation as Integer.bitCount() in the JDK.
        /// </summary>
        /// <param name="value">The <see cref="int"/> to examine.</param>
        /// <returns>The number of one-bits in the two's complement binary
        /// representation of the specified <paramref name="value"/>.</returns>
        public static int PopCount(this int value)
        {
#if FEATURE_NUMERICBITOPERATIONS
            return System.Numerics.BitOperations.PopCount((uint)value);
#else
            // Hacker's Delight, Figure 5-2
            value -= ((value >>> 1) & 0x55555555);
            value = (value & 0x33333333) + ((value >>> 2) & 0x33333333);
            value = (value + (value >>> 4)) & 0x0f0f0f0f;
            value += (value >>> 8);
            value += (value >>> 16);
            return value & 0x3f;
#endif
        }

        /// <summary>
        /// Returns the population count (number of set bits) of a <see cref="long"/> mask.
        /// <para/>
        /// Usage Note: This is the same operation as Long.bitCount() in the JDK.
        /// </summary>
        /// <param name="value">The <see cref="long"/> to examine.</param>
        /// <returns>The number of one-bits in the two's complement binary
        /// representation of the specified <paramref name="value"/>.</returns>
        public static int PopCount(this long value)
        {
#if FEATURE_NUMERICBITOPERATIONS
            return System.Numerics.BitOperations.PopCount((ulong)value);
#else
            // Hacker's Delight, Figure 5-14
            value -= ((value >>> 1) & 0x5555555555555555L);
            value = (value & 0x3333333333333333L) + ((value >>> 2) & 0x3333333333333333L);
            value = (value + (value >>> 4)) & 0x0f0f0f0f0f0f0f0fL;
            value += (value >>> 8);
            value += (value >>> 16);
            value += (value >>> 32);
            return (int)value & 0x7f;
#endif
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
        ///     <item><description>floor(log<sub>2</sub>(x)) = <c>31 - x.LeadingZeroCount()</c></description></item>
        ///     <item><description>ceil(log<sub>2</sub>(x)) = <c>32 - (x - 1).LeadingZeroCount()</c></description></item>
        /// </list>
        /// <para/>
        /// Usage Note: This is the same operation as Integer.numberOfLeadingZeros() in the JDK.
        /// </summary>
        /// <param name="value">The <see cref="int"/> to examine.</param>
        /// <returns>The number of zero bits preceding the highest-order
        /// ("leftmost") one-bit in the two's complement binary representation
        /// of the specified <paramref name="value"/>, or 32 if the value
        /// is equal to zero.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int LeadingZeroCount(this int value)
        {
            if (value == 0)
                return 32;

#if FEATURE_NUMERICBITOPERATIONS
            return System.Numerics.BitOperations.LeadingZeroCount((uint)value);
#else
            // Hacker's Delight, Figure 5-6
            value |= value >> 1;
            value |= value >> 2;
            value |= value >> 4;
            value |= value >> 8;
            value |= value >> 16;
            return PopCount(~value);
#endif
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
        ///     <item><description>floor(log<sub>2</sub>(x)) = <c>63 - x.LeadingZeroCount()</c></description></item>
        ///     <item><description>ceil(log<sub>2</sub>(x)) = <c>64 - (x - 1).LeadingZeroCount()</c></description></item>
        /// </list>
        /// <para/>
        /// Usage Note: This is the same operation as Long.numberOfLeadingZeros() in the JDK.
        /// </summary>
        /// <param name="value">The <see cref="long"/> to examine.</param>
        /// <returns>The number of zero bits preceding the highest-order
        /// ("leftmost") one-bit in the two's complement binary representation
        /// of the specified <paramref name="value"/>, or 64 if the value
        /// is equal to zero.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int LeadingZeroCount(this long value)
        {
            if (value == 0)
                return 64;

#if FEATURE_NUMERICBITOPERATIONS
            return System.Numerics.BitOperations.LeadingZeroCount((ulong)value);
#else
            // Hacker's Delight, Figure 5-6
            value |= value >> 1;
            value |= value >> 2;
            value |= value >> 4;
            value |= value >> 8;
            value |= value >> 16;
            value |= value >> 32;
            return PopCount(~value);
#endif
        }

        #endregion NumberOfLeadingZeros

        #region NumberOfTrailingZeros

        /// <summary>
        /// Returns the number of zero bits following the lowest-order ("rightmost")
        /// one-bit in the two's complement binary representation of the specified
        /// <paramref name="value"/>. Returns 32 if the specified value has no
        /// one-bits in its two's complement representation, in other words if it is
        /// equal to zero.
        /// <para/>
        /// Usage Note: This is the same operation as Integer.numberOfTrailingZeros() in the JDK.
        /// </summary>
        /// <param name="value">The <see cref="int"/> to examine.</param>
        /// <returns>The number of zero bits following the lowest-order ("rightmost")
        /// one-bit in the two's complement binary representation of the
        /// specified <paramref name="value"/>, or 32 if the value is equal
        /// to zero.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int TrailingZeroCount(this int value)
        {
            if (value == 0)
                return 32;
#if FEATURE_NUMERICBITOPERATIONS
            return System.Numerics.BitOperations.TrailingZeroCount((uint)value);
#else
            return PopCount((value & -value) - 1);
#endif
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
        public static int TrailingZeroCount(this long value)
        {
            if (value == 0)
                return 64;
#if FEATURE_NUMERICBITOPERATIONS
            return System.Numerics.BitOperations.TrailingZeroCount((ulong)value);
#else
            return PopCount((value & -value) - 1);
#endif
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
            return (value & ~(value >>> 1));
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
            return (value & ~(value >>> 1));
        }

        #endregion HighestOneBit

        #region Log2

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int Log2(this uint value)
        {
#if FEATURE_NUMERICBITOPERATIONS
            return System.Numerics.BitOperations.Log2(value);
#else
            // The 0->0 contract is fulfilled by setting the LSB to 1.
            // Log(1) is 0, and setting the LSB for values > 1 does not change the log2 result.
            value |= 1;
            return 31 ^ LeadingZeroCount((int)value);
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int Log2(this ulong value)
        {
#if FEATURE_NUMERICBITOPERATIONS
            return System.Numerics.BitOperations.Log2(value);
#else
            value |= 1;
            return 63 ^ LeadingZeroCount((long)value);
#endif
        }

        #endregion Log2

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
            value = (value & 0x55555555) << 1 | (value >>> 1) & 0x55555555;
            value = (value & 0x33333333) << 2 | (value >>> 2) & 0x33333333;
            value = (value & 0x0f0f0f0f) << 4 | (value >>> 4) & 0x0f0f0f0f;
            value = (value << 24) | ((value & 0xff00) << 8) |
                ((value >>> 8) & 0xff00) | (value >>> 24);
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
            value = (value & 0x5555555555555555L) << 1 | (value >>> 1) & 0x5555555555555555L;
            value = (value & 0x3333333333333333L) << 2 | (value >>> 2) & 0x3333333333333333L;
            value = (value & 0x0f0f0f0f0f0f0f0fL) << 4 | (value >>> 4) & 0x0f0f0f0f0f0f0f0fL;
            value = (value & 0x00ff00ff00ff00ffL) << 8 | (value >>> 8) & 0x00ff00ff00ff00ffL;
            value = (value << 48) | ((value & 0xffff0000L) << 16) |
                ((value >>> 16) & 0xffff0000L) | (value >>> 48);
            return value;
        }

        #endregion

        #region ReverseBytes

        /// <summary>
        /// Returns the value obtained by reversing the order of the bytes in the
        /// two's complement representation of the specified <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The <see cref="short"/> value for which to reverse the byte order.</param>
        /// <returns>The value obtained by reversing the bytes in the specified
        /// <paramref name="value"/>.</returns>
        [Obsolete("Use BinaryPrimitives.ReverseEndianness(short) instead.")]
        public static short ReverseBytes(this short value)
            => BinaryPrimitives.ReverseEndianness(value);

        /// <summary>
        /// Returns the value obtained by reversing the order of the bytes in the
        /// two's complement representation of the specified <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The <see cref="int"/> value for which to reverse the byte order.</param>
        /// <returns>The value obtained by reversing the bytes in the specified
        /// <paramref name="value"/>.</returns>
        [Obsolete("Use BinaryPrimitives.ReverseEndianness(int) instead.")]
        public static int ReverseBytes(this int value)
            => BinaryPrimitives.ReverseEndianness(value);

        /// <summary>
        /// Returns the value obtained by reversing the order of the bytes in the
        /// two's complement representation of the specified <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The <see cref="long"/> value for which to reverse the byte order.</param>
        /// <returns>The value obtained by reversing the bytes in the specified
        /// <paramref name="value"/>.</returns>
        [Obsolete("Use BinaryPrimitives.ReverseEndianness(long) instead.")]
        public static long ReverseBytes(this long value)
            => BinaryPrimitives.ReverseEndianness(value);

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
                return value;
            /*
             * According to JLS3, 15.19, the right operand of a shift is always
             * implicitly masked with 0x1F, which the negation of 'distance' is
             * taking advantage of.
             */
#if FEATURE_NUMERICBITOPERATIONS
            return (int)System.Numerics.BitOperations.RotateLeft((uint)value, distance);
#else
            return ((value << distance) | (value >>> -distance));
#endif
        }

        internal static uint RotateLeft(this uint value, int distance) // J2N TODO: API - Make public once we have implementations for all methods accepting uint and ulong
            => (value << distance) | (value >> (32 - distance));

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
                return value;
            /*
             * According to JLS3, 15.19, the right operand of a shift is always
             * implicitly masked with 0x3F, which the negation of 'distance' is
             * taking advantage of.
             */
#if FEATURE_NUMERICBITOPERATIONS
            return (long)System.Numerics.BitOperations.RotateLeft((ulong)value, distance);
#else
            return ((value << distance) | (value >>> -distance));
#endif
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
                return value;
            /*
             * According to JLS3, 15.19, the right operand of a shift is always
             * implicitly masked with 0x1F, which the negation of 'distance' is
             * taking advantage of.
             */
#if FEATURE_NUMERICBITOPERATIONS
            return (int)System.Numerics.BitOperations.RotateRight((uint)value, distance);
#else
            return ((value >>> distance) | (value << (-distance)));
#endif
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
                return value;
            /*
             * According to JLS3, 15.19, the right operand of a shift is always
             * implicitly masked with 0x3F, which the negation of 'distance' is
             * taking advantage of.
             */
#if FEATURE_NUMERICBITOPERATIONS
            return (long)System.Numerics.BitOperations.RotateRight((ulong)value, distance);
#else
            return ((value >>> distance) | (value << -distance));
#endif
        }

        #endregion

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
