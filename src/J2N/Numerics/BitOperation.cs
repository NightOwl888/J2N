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
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace J2N.Numerics
{
    /// <summary>
    /// Extensions to <see cref="int"/> and <see cref="long"/> to perform bitwise
    /// operations.
    /// </summary>
    public static class BitOperation
    {
        // C# no-alloc optimization that directly wraps the data section of the dll (similar to string constants)
        // https://github.com/dotnet/roslyn/pull/24621

#if !FEATURE_NUMERICBITOPERATIONS
        private static ReadOnlySpan<byte> TrailingZeroCountDeBruijn => // 32
        [
            00, 01, 28, 02, 29, 14, 24, 03,
            30, 22, 20, 15, 25, 17, 04, 08,
            31, 27, 13, 23, 21, 19, 16, 07,
            26, 12, 18, 06, 11, 05, 10, 09
        ];
#endif

        private static ReadOnlySpan<byte> Log2DeBruijn => // 32
        [
            00, 09, 01, 10, 13, 21, 02, 29,
            11, 14, 16, 18, 22, 25, 03, 30,
            08, 12, 20, 28, 15, 17, 24, 07,
            19, 27, 23, 06, 26, 05, 04, 31
        ];

        #region BitCount

        /// <summary>
        /// Returns the population count (number of set bits) of an <see cref="int"/> mask.
        /// Similar in behavior to the x86 instruction POPCNT.
        /// <para/>
        /// Usage Note: This is the same operation as Integer.bitCount() in the JDK.
        /// </summary>
        /// <param name="value">The <see cref="int"/> to examine.</param>
        /// <returns>The number of one-bits in the two's complement binary
        /// representation of the specified <paramref name="value"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        /// Returns the population count (number of set bits) of an <see cref="uint"/> mask.
        /// Similar in behavior to the x86 instruction POPCNT.
        /// <para/>
        /// Usage Note: This is a similar operation as Integer.bitCount() in the JDK.
        /// </summary>
        /// <param name="value">The <see cref="uint"/> to examine.</param>
        /// <returns>The number of one-bits in the two's complement binary
        /// representation of the specified <paramref name="value"/>.</returns>
        //[Intrinsic]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CLSCompliant(false)]
        public static int PopCount(this uint value)
        {
#if FEATURE_NUMERICBITOPERATIONS
            return System.Numerics.BitOperations.PopCount(value);
#else
            const uint c1 = 0x_55555555u;
            const uint c2 = 0x_33333333u;
            const uint c3 = 0x_0F0F0F0Fu;
            const uint c4 = 0x_01010101u;

            value -= (value >> 1) & c1;
            value = (value & c2) + ((value >> 2) & c2);
            value = (((value + (value >> 4)) & c3) * c4) >> 24;

            return (int)value;
#endif
        }

        /// <summary>
        /// Returns the population count (number of set bits) of a <see cref="long"/> mask.
        /// Similar in behavior to the x86 instruction POPCNT.
        /// <para/>
        /// Usage Note: This is the same operation as Long.bitCount() in the JDK.
        /// </summary>
        /// <param name="value">The <see cref="long"/> to examine.</param>
        /// <returns>The number of one-bits in the two's complement binary
        /// representation of the specified <paramref name="value"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

        /// <summary>
        /// Returns the population count (number of set bits) of a <see cref="ulong"/> mask.
        /// Similar in behavior to the x86 instruction POPCNT.
        /// <para/>
        /// Usage Note: This is the same operation as Long.bitCount() in the JDK.
        /// </summary>
        /// <param name="value">The <see cref="ulong"/> to examine.</param>
        /// <returns>The number of one-bits in the two's complement binary
        /// representation of the specified <paramref name="value"/>.</returns>
        //[Intrinsic]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CLSCompliant(false)]
        public static int PopCount(this ulong value)
        {
#if FEATURE_NUMERICBITOPERATIONS
            return System.Numerics.BitOperations.PopCount(value);
#else
            const uint c1 = 0x_55555555u;
            const uint c2 = 0x_33333333u;
            const uint c3 = 0x_0F0F0F0Fu;
            const uint c4 = 0x_01010101u;

            value -= (value >> 1) & c1;
            value = (value & c2) + ((value >> 2) & c2);
            value = (((value + (value >> 4)) & c3) * c4) >> 24;

            return (int)value;
#endif
        }

        /// <summary>
        /// Returns the population count (number of bits set) of a mask.
        /// Similar in behavior to the x86 instruction POPCNT.
        /// </summary>
        /// <param name="value">The value.</param>
        //[Intrinsic]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CLSCompliant(false)]
        public static int PopCount(this nuint value)
        {
#if FEATURE_BITOPERATIONS_UINTPTR
            return System.Numerics.BitOperations.PopCount(value);
#else
            if (IntPtr.Size == 8) // 64-bit process
            {
                return PopCount((ulong)value);
            }
            else
            {
                return PopCount((uint)value);
            }
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
            return 31 ^ Log2SoftwareFallback((uint)value);
#endif
        }

        /// <summary>
        /// Returns the number of zero bits preceding the highest-order
        /// ("leftmost") one-bit in the two's complement binary representation
        /// of the specified <paramref name="value"/>. Returns 32 if the
        /// specified value has no one-bits in its two's complement representation,
        /// in other words if it is equal to zero.
        /// <para/>
        /// Note that this method is closely related to the logarithm base 2.
        /// For all positive <see cref="uint"/> values x:
        /// <list type="bullet">
        ///     <item><description>floor(log<sub>2</sub>(x)) = <c>31 - x.LeadingZeroCount()</c></description></item>
        ///     <item><description>ceil(log<sub>2</sub>(x)) = <c>32 - (x - 1).LeadingZeroCount()</c></description></item>
        /// </list>
        /// <para/>
        /// Usage Note: This is a similar operation as Integer.numberOfLeadingZeros() in the JDK.
        /// </summary>
        /// <param name="value">The <see cref="uint"/> to examine.</param>
        /// <returns>The number of zero bits preceding the highest-order
        /// ("leftmost") one-bit in the two's complement binary representation
        /// of the specified <paramref name="value"/>, or 32 if the value
        /// is equal to zero.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CLSCompliant(false)]
        public static int LeadingZeroCount(this uint value)
        {
            if (value == 0)
                return 32;

#if FEATURE_NUMERICBITOPERATIONS
            return System.Numerics.BitOperations.LeadingZeroCount(value);
#else
            return 31 ^ Log2SoftwareFallback(value);
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

        /// <summary>
        /// Returns the number of zero bits preceding the highest-order
        /// ("leftmost") one-bit in the two's complement binary representation
        /// of the specified <paramref name="value"/>. Returns 64 if the
        /// specified value has no one-bits in its two's complement representation,
        /// in other words if it is equal to zero.
        /// <para/>
        /// Note that this method is closely related to the logarithm base 2.
        /// For all positive <see cref="ulong"/> values x:
        /// <list type="bullet">
        ///     <item><description>floor(log<sub>2</sub>(x)) = <c>63 - x.LeadingZeroCount()</c></description></item>
        ///     <item><description>ceil(log<sub>2</sub>(x)) = <c>64 - (x - 1).LeadingZeroCount()</c></description></item>
        /// </list>
        /// <para/>
        /// Usage Note: This is a similar operation as Long.numberOfLeadingZeros() in the JDK.
        /// </summary>
        /// <param name="value">The <see cref="ulong"/> to examine.</param>
        /// <returns>The number of zero bits preceding the highest-order
        /// ("leftmost") one-bit in the two's complement binary representation
        /// of the specified <paramref name="value"/>, or 64 if the value
        /// is equal to zero.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CLSCompliant(false)]
        public static int LeadingZeroCount(this ulong value)
        {
            if (value == 0)
                return 64;

#if FEATURE_NUMERICBITOPERATIONS
            return System.Numerics.BitOperations.LeadingZeroCount(value);
#else
            uint hi = (uint)(value >> 32);

            if (hi == 0)
            {
                return 32 + LeadingZeroCount((uint)value);
            }

            return LeadingZeroCount(hi);
#endif
        }

        /// <summary>
        /// Count the number of leading zero bits in a mask.
        /// Similar in behavior to the x86 instruction LZCNT.
        /// </summary>
        /// <param name="value">The value.</param>
        //[Intrinsic]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CLSCompliant(false)]
        public static int LeadingZeroCount(this nuint value)
        {
#if FEATURE_BITOPERATIONS_UINTPTR
            return System.Numerics.BitOperations.LeadingZeroCount(value);
#else
            if (IntPtr.Size == 8) // 64-bit process
            {
                return LeadingZeroCount((ulong)value);
            }
            else
            {
                return LeadingZeroCount((uint)value);
            }
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
        /// Similar in behavior to the x86 instruction TZCNT.
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
            // uint.MaxValue >> 27 is always in range [0 - 31] so we use Unsafe.AddByteOffset to avoid bounds check
            return Unsafe.AddByteOffset(
                // Using deBruijn sequence, k=2, n=5 (2^5=32) : 0b_0000_0111_0111_1100_1011_0101_0011_0001u
                ref MemoryMarshal.GetReference(TrailingZeroCountDeBruijn),
                // uint|long -> IntPtr cast on 32-bit platforms does expensive overflow checks not needed here
                (IntPtr)(int)(((value & (uint)-(int)value) * 0x077CB531u) >> 27)); // Multi-cast mitigates redundant conv.u8
#endif
        }

        /// <summary>
        /// Returns the number of zero bits following the lowest-order ("rightmost")
        /// one-bit in the two's complement binary representation of the specified
        /// <paramref name="value"/>. Returns 32 if the specified value has no
        /// one-bits in its two's complement representation, in other words if it is
        /// equal to zero.
        /// <para/>
        /// Similar in behavior to the x86 instruction TZCNT.
        /// <para/>
        /// Usage Note: This is the same operation as Integer.numberOfTrailingZeros() in the JDK.
        /// </summary>
        /// <param name="value">The <see cref="uint"/> to examine.</param>
        /// <returns>The number of zero bits following the lowest-order ("rightmost")
        /// one-bit in the two's complement binary representation of the
        /// specified <paramref name="value"/>, or 32 if the value is equal
        /// to zero.</returns>
        //[Intrinsic]
        [CLSCompliant(false)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int TrailingZeroCount(this uint value)
        {
            if (value == 0)
                return 32;
#if FEATURE_NUMERICBITOPERATIONS
            return System.Numerics.BitOperations.TrailingZeroCount(value);
#else
            // uint.MaxValue >> 27 is always in range [0 - 31] so we use Unsafe.AddByteOffset to avoid bounds check
            return Unsafe.AddByteOffset(
                // Using deBruijn sequence, k=2, n=5 (2^5=32) : 0b_0000_0111_0111_1100_1011_0101_0011_0001u
                ref MemoryMarshal.GetReference(TrailingZeroCountDeBruijn),
                // uint|long -> IntPtr cast on 32-bit platforms does expensive overflow checks not needed here
                (IntPtr)(int)(((value & (uint)-(int)value) * 0x077CB531u) >> 27)); // Multi-cast mitigates redundant conv.u8
#endif
        }

        /// <summary>
        /// Returns the number of zero bits following the lowest-order ("rightmost")
        /// one-bit in the two's complement binary representation of the specified
        /// <paramref name="value"/>. Returns 64 if the specified value has no
        /// one-bits in its two's complement representation, in other words if it is
        /// equal to zero.
        /// <para/>
        /// Similar in behavior to the x86 instruction TZCNT.
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
            uint lo = (uint)(ulong)value;

            if (lo == 0)
            {
                return 32 + TrailingZeroCount((uint)(((ulong)value) >> 32));
            }

            return TrailingZeroCount(lo);
#endif
        }

        /// <summary>
        /// Returns the number of zero bits following the lowest-order ("rightmost")
        /// one-bit in the two's complement binary representation of the specified
        /// <paramref name="value"/>. Returns 64 if the specified value has no
        /// one-bits in its two's complement representation, in other words if it is
        /// equal to zero.
        /// <para/>
        /// Similar in behavior to the x86 instruction TZCNT.
        /// </summary>
        /// <param name="value">The <see cref="ulong"/> to examine.</param>
        /// <returns>The number of zero bits following the lowest-order ("rightmost")
        /// one-bit in the two's complement binary representation of the
        /// specified <paramref name="value"/>, or 64 if the value is equal
        /// to zero.</returns>
        //[Intrinsic]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CLSCompliant(false)]
        public static int TrailingZeroCount(this ulong value)
        {
            if (value == 0)
                return 64;
#if FEATURE_NUMERICBITOPERATIONS
            return System.Numerics.BitOperations.TrailingZeroCount(value);
#else
            uint lo = (uint)value;

            if (lo == 0)
            {
                return 32 + TrailingZeroCount((uint)(value >> 32));
            }

            return TrailingZeroCount(lo);
#endif
        }

        /// <summary>
        /// Count the number of trailing zero bits in a mask.
        /// Similar in behavior to the x86 instruction TZCNT.
        /// </summary>
        /// <param name="value">The value.</param>
        //[Intrinsic]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int TrailingZeroCount(this nint value)
        {
#if FEATURE_BITOPERATIONS_UINTPTR
            return System.Numerics.BitOperations.TrailingZeroCount(value);
#else
            if (IntPtr.Size == 8) // 64-bit process
            {
                return TrailingZeroCount((ulong)(nuint)value);
            }
            else
            {
                return TrailingZeroCount((uint)(nuint)value);
            }
#endif
        }

        /// <summary>
        /// Count the number of trailing zero bits in a mask.
        /// Similar in behavior to the x86 instruction TZCNT.
        /// </summary>
        /// <param name="value">The value.</param>
        //[Intrinsic]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CLSCompliant(false)]
        public static int TrailingZeroCount(this nuint value)
        {
#if FEATURE_BITOPERATIONS_UINTPTR
            return System.Numerics.BitOperations.TrailingZeroCount(value);
#else
            if (IntPtr.Size == 8) // 64-bit process
            {
                return TrailingZeroCount((ulong)value);
            }
            else
            {
                return TrailingZeroCount((uint)value);
            }
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

        #region IsPow2

        /// <summary>
        /// Evaluate whether a given integral value is a power of 2.
        /// </summary>
        /// <param name="value">The value.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPow2(this int value) => (value & (value - 1)) == 0 && value > 0;

        /// <summary>
        /// Evaluate whether a given integral value is a power of 2.
        /// </summary>
        /// <param name="value">The value.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CLSCompliant(false)]
        public static bool IsPow2(this uint value) => (value & (value - 1)) == 0 && value != 0;

        /// <summary>
        /// Evaluate whether a given integral value is a power of 2.
        /// </summary>
        /// <param name="value">The value.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPow2(this long value) => (value & (value - 1)) == 0 && value > 0;

        /// <summary>
        /// Evaluate whether a given integral value is a power of 2.
        /// </summary>
        /// <param name="value">The value.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CLSCompliant(false)]
        public static bool IsPow2(this ulong value) => (value & (value - 1)) == 0 && value != 0;

        /// <summary>
        /// Evaluate whether a given integral value is a power of 2.
        /// </summary>
        /// <param name="value">The value.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPow2(this nint value) => (value & (value - 1)) == 0 && value > 0;

        /// <summary>
        /// Evaluate whether a given integral value is a power of 2.
        /// </summary>
        /// <param name="value">The value.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CLSCompliant(false)]
        public static bool IsPow2(this nuint value) => (value & (value - 1)) == 0 && value != 0;

        #endregion IsPow2

        #region Log2

        /// <summary>
        /// Returns the integer (floor) log of the specified value, base 2.
        /// Note that by convention, input value 0 returns 0 since log(0) is undefined.
        /// </summary>
        /// <param name="value">The value.</param>
        //[Intrinsic]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CLSCompliant(false)]
        public static int Log2(this int value)
        {
#if FEATURE_NUMERICBITOPERATIONS
            return System.Numerics.BitOperations.Log2((uint)value);
#else
            // Fallback contract is 0->0
            return Log2SoftwareFallback((uint)value);
#endif
        }

        /// <summary>
        /// Returns the integer (floor) log of the specified value, base 2.
        /// Note that by convention, input value 0 returns 0 since log(0) is undefined.
        /// </summary>
        /// <param name="value">The value.</param>
        //[Intrinsic]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CLSCompliant(false)]
        public static int Log2(this uint value)
        {
#if FEATURE_NUMERICBITOPERATIONS
            return System.Numerics.BitOperations.Log2(value);
#else
            // Fallback contract is 0->0
            return Log2SoftwareFallback(value);
#endif
        }

        /// <summary>
        /// Returns the integer (floor) log of the specified value, base 2.
        /// Note that by convention, input value 0 returns 0 since log(0) is undefined.
        /// </summary>
        /// <param name="value">The value.</param>
        //[Intrinsic]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CLSCompliant(false)]
        public static int Log2(this long value)
        {
#if FEATURE_NUMERICBITOPERATIONS
            return System.Numerics.BitOperations.Log2((ulong)value);
#else
            uint hi = (uint)(((ulong)value) >> 32);

            if (hi == 0)
            {
                return Log2((uint)(ulong)value);
            }

            return 32 + Log2(hi);
#endif
        }

        /// <summary>
        /// Returns the integer (floor) log of the specified value, base 2.
        /// Note that by convention, input value 0 returns 0 since log(0) is undefined.
        /// </summary>
        /// <param name="value">The value.</param>
        //[Intrinsic]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CLSCompliant(false)]
        public static int Log2(this ulong value)
        {
#if FEATURE_NUMERICBITOPERATIONS
            return System.Numerics.BitOperations.Log2(value);
#else
            uint hi = (uint)(value >> 32);

            if (hi == 0)
            {
                return Log2((uint)value);
            }

            return 32 + Log2(hi);
#endif
        }

        /// <summary>
        /// Returns the integer (floor) log of the specified value, base 2.
        /// Note that by convention, input value 0 returns 0 since log(0) is undefined.
        /// </summary>
        /// <param name="value">The value.</param>
        //[Intrinsic]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CLSCompliant(false)]
        public static int Log2(this nuint value)
        {
#if FEATURE_BITOPERATIONS_UINTPTR
            return System.Numerics.BitOperations.Log2(value);
#else
            if (IntPtr.Size == 8) // 64-bit process
            {
                return Log2((ulong)value);
            }
            else
            {
                return Log2((uint)value);
            }
#endif
        }

#if !FEATURE_NUMERICBITOPERATIONS
        /// <summary>
        /// Returns the integer (floor) log of the specified value, base 2.
        /// Note that by convention, input value 0 returns 0 since Log(0) is undefined.
        /// Does not directly use any hardware intrinsics, nor does it incur branching.
        /// </summary>
        /// <param name="value">The value.</param>
        private static int Log2SoftwareFallback(uint value)
        {
            // No AggressiveInlining due to large method size
            // Has conventional contract 0->0 (Log(0) is undefined)

            // Fill trailing zeros with ones, eg 00010010 becomes 00011111
            value |= value >> 01;
            value |= value >> 02;
            value |= value >> 04;
            value |= value >> 08;
            value |= value >> 16;

            // Using deBruijn sequence, k=2, n=5 (2^5=32) : 0b_0000_0111_1100_0100_1010_1100_1101_1101u
            return Log2DeBruijn[(int)((value * 0x07C4ACDDu) >> 27)];
        }
#endif

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
        public static short ReverseBytes(this short value)
        {
            int high = (value >> 8) & 0xFF;
            int low = (value & 0xFF) << 8;
            return (short)(low | high);
        }

        /// <summary>
        /// Returns the value obtained by reversing the order of the bytes in the
        /// two's complement representation of the specified <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The <see cref="int"/> value for which to reverse the byte order.</param>
        /// <returns>The value obtained by reversing the bytes in the specified
        /// <paramref name="value"/>.</returns>
        public static int ReverseBytes(this int value)
        {
            return ((value >>> 24)) |
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
                (value >>> 8) & 0x00ff00ff00ff00ffL;
            return (value << 48) |
                ((value & 0xffff0000L) << 16) |
                ((value >>> 16) & 0xffff0000L) |
                (value >>> 48);
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
        /// <para/>
        /// Similar in behavior to the x86 instruction ROL.
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
        /// <para/>
        /// Similar in behavior to the x86 instruction ROL.
        /// </summary>
        /// <param name="value">The <see cref="uint"/> value to rotate left.</param>
        /// <param name="distance">The number of bits to rotate.</param>
        /// <returns>The value obtained by rotating the two's complement binary
        /// representation of the specified <paramref name="value"/> left by the
        /// specified number of bits.</returns>
        //[Intrinsic]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CLSCompliant(false)]
        public static uint RotateLeft(this uint value, int distance)
        {
            if (distance == 0)
                return value;
            /*
             * According to JLS3, 15.19, the right operand of a shift is always
             * implicitly masked with 0x1F, which the negation of 'distance' is
             * taking advantage of.
             */
#if FEATURE_NUMERICBITOPERATIONS
            return System.Numerics.BitOperations.RotateLeft(value, distance);
#else
            return (value << distance) | (value >> (32 - distance));
#endif
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
        /// <para/>
        /// Similar in behavior to the x86 instruction ROL.
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
        /// <para/>
        /// Similar in behavior to the x86 instruction ROL.
        /// </summary>
        /// <param name="value">The <see cref="ulong"/> value to rotate left.</param>
        /// <param name="distance">The number of bits to rotate.</param>
        /// <returns>The value obtained by rotating the two's complement binary
        /// representation of the specified <paramref name="value"/> left by the
        /// specified number of bits.</returns>
        //[Intrinsic]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CLSCompliant(false)]
        public static ulong RotateLeft(this ulong value, int distance)
        {
            if (distance == 0)
                return value;
            /*
             * According to JLS3, 15.19, the right operand of a shift is always
             * implicitly masked with 0x3F, which the negation of 'distance' is
             * taking advantage of.
             */
#if FEATURE_NUMERICBITOPERATIONS
            return System.Numerics.BitOperations.RotateLeft(value, distance);
#else
            return (value << distance) | (value >> (64 - distance));
#endif
        }

        /// <summary>
        /// Rotates the specified value left by the specified number of bits.
        /// Similar in behavior to the x86 instruction ROL.
        /// </summary>
        /// <param name="value">The value to rotate.</param>
        /// <param name="distance">The number of bits to rotate by.
        /// Any value outside the range [0..31] is treated as congruent mod 32 on a 32-bit process,
        /// and any value outside the range [0..63] is treated as congruent mod 64 on a 64-bit process.</param>
        /// <returns>The rotated value.</returns>
        //[Intrinsic]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CLSCompliant(false)]
        public static nuint RotateLeft(this nuint value, int distance)
        {
#if FEATURE_BITOPERATIONS_UINTPTR
            return System.Numerics.BitOperations.RotateLeft(value, distance);
#else
            if (IntPtr.Size == 8) // 64-bit process
            {
                return (nuint)RotateLeft((ulong)value, distance);
            }
            else
            {
                return (nuint)RotateLeft((uint)value, distance);
            }
#endif
        }

        #endregion RotateLeft

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
        /// <para/>
        /// Similar in behavior to the x86 instruction ROR.
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
        /// <para/>
        /// Similar in behavior to the x86 instruction ROR.
        /// </summary>
        /// <param name="value">The <see cref="uint"/> value to rotate right.</param>
        /// <param name="distance">The number of bits to rotate.</param>
        /// <returns>The value obtained by rotating the two's complement binary
        /// representation of the specified <paramref name="value"/> right by the
        /// specified number of bits.</returns>
        //[Intrinsic]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CLSCompliant(false)]
        public static uint RotateRight(this uint value, int distance)
        {
            if (distance == 0)
                return value;
            /*
             * According to JLS3, 15.19, the right operand of a shift is always
             * implicitly masked with 0x1F, which the negation of 'distance' is
             * taking advantage of.
             */
#if FEATURE_NUMERICBITOPERATIONS
            return System.Numerics.BitOperations.RotateRight(value, distance);
#else
            return (value >> distance) | (value << (32 - distance));
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
        /// <para/>
        /// Similar in behavior to the x86 instruction ROR.
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
        /// <para/>
        /// Similar in behavior to the x86 instruction ROR.
        /// </summary>
        /// <param name="value">The <see cref="ulong"/> value to rotate right.</param>
        /// <param name="distance">The number of bits to rotate.</param>
        /// <returns>The value obtained by rotating the two's complement binary
        /// representation of the specified <paramref name="value"/> right by the
        /// specified number of bits.</returns>
        //[Intrinsic]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CLSCompliant(false)]
        public static ulong RotateRight(this ulong value, int distance)
        {
            if (distance == 0)
                return value;
            /*
             * According to JLS3, 15.19, the right operand of a shift is always
             * implicitly masked with 0x3F, which the negation of 'distance' is
             * taking advantage of.
             */
#if FEATURE_NUMERICBITOPERATIONS
            return System.Numerics.BitOperations.RotateRight(value, distance);
#else
            return (value >> distance) | (value << (64 - distance));
#endif
        }

        /// <summary>
        /// Rotates the specified value right by the specified number of bits.
        /// Similar in behavior to the x86 instruction ROR.
        /// </summary>
        /// <param name="value">The value to rotate.</param>
        /// <param name="distance">The number of bits to rotate by.
        /// Any value outside the range [0..31] is treated as congruent mod 32 on a 32-bit process,
        /// and any value outside the range [0..63] is treated as congruent mod 64 on a 64-bit process.</param>
        /// <returns>The rotated value.</returns>
        //[Intrinsic]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CLSCompliant(false)]
        public static nuint RotateRight(this nuint value, int distance)
        {
#if FEATURE_BITOPERATIONS_UINTPTR
            return System.Numerics.BitOperations.RotateRight(value, distance);
#else
            if (IntPtr.Size == 8) // 64-bit process
            {
                return (nuint)RotateRight((ulong)value, distance);
            }
            else
            {
                return (nuint)RotateRight((uint)value, distance);
            }
#endif
        }

        #endregion RotateRight

        #region RoundUpToPowerOf2

        /// <summary>Round the given integral value up to a power of 2.</summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The smallest power of 2 which is greater than or equal to <paramref name="value"/>.
        /// If <paramref name="value"/> is 0 or the result overflows, returns 0.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int RoundUpToPowerOf2(this int value)
        {
            if (value <= 0)
                return 0;

            // Cap check: max power of two for signed int is 1 << 30
            if (value > (1 << 30))
                return 0;

#if FEATURE_BITOPERATIONS_ROUNDUPTOPOWEROF2
            return (int)System.Numerics.BitOperations.RoundUpToPowerOf2((uint)value);
#elif FEATURE_NUMERICBITOPERATIONS
            if (IntPtr.Size == 8) // 64-bit process
            {
                return (int)(0x1_0000_0000ul >> LeadingZeroCount(value - 1));
            }
            else
            {
                int shift = 32 - LeadingZeroCount(value - 1);
                return (int)(1u ^ (uint)(shift >> 5)) << shift;
            }
#else
            // Based on https://graphics.stanford.edu/~seander/bithacks.html#RoundUpPowerOf2
            --value;
            value |= value >> 1;
            value |= value >> 2;
            value |= value >> 4;
            value |= value >> 8;
            value |= value >> 16;
            return value + 1;
#endif
        }

        /// <summary>Round the given integral value up to a power of 2.</summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The smallest power of 2 which is greater than or equal to <paramref name="value"/>.
        /// If <paramref name="value"/> is 0 or the result overflows, returns 0.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CLSCompliant(false)]
        public static uint RoundUpToPowerOf2(this uint value)
        {
#if FEATURE_BITOPERATIONS_ROUNDUPTOPOWEROF2
            return System.Numerics.BitOperations.RoundUpToPowerOf2(value);
#elif FEATURE_NUMERICBITOPERATIONS
            if (IntPtr.Size == 8) // 64-bit process
            {
                return (uint)(0x1_0000_0000ul >> LeadingZeroCount(value - 1));
            }
            else
            {
                int shift = 32 - LeadingZeroCount(value - 1);
                return (1u ^ (uint)(shift >> 5)) << shift;
            }
#else
            // Based on https://graphics.stanford.edu/~seander/bithacks.html#RoundUpPowerOf2
            --value;
            value |= value >> 1;
            value |= value >> 2;
            value |= value >> 4;
            value |= value >> 8;
            value |= value >> 16;
            return value + 1;
#endif
        }

        /// <summary>
        /// Round the given integral value up to a power of 2.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The smallest power of 2 which is greater than or equal to <paramref name="value"/>.
        /// If <paramref name="value"/> is 0 or the result overflows, returns 0.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long RoundUpToPowerOf2(this long value)
        {
            if (value <= 0)
                return 0;

            // Cap check: max power of 2 for long is 1L << 62
            if (value > (1L << 62))
                return 0;

#if FEATURE_BITOPERATIONS_ROUNDUPTOPOWEROF2
            return (long)System.Numerics.BitOperations.RoundUpToPowerOf2((ulong)value);
#elif FEATURE_NUMERICBITOPERATIONS
            int shift = 64 - LeadingZeroCount(value - 1);
            return (long)(1ul ^ (ulong)(shift >> 6)) << shift;
#else
            // Based on https://graphics.stanford.edu/~seander/bithacks.html#RoundUpPowerOf2
            --value;
            value |= value >> 1;
            value |= value >> 2;
            value |= value >> 4;
            value |= value >> 8;
            value |= value >> 16;
            value |= value >> 32;
            return value + 1;
#endif
        }

        /// <summary>
        /// Round the given integral value up to a power of 2.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The smallest power of 2 which is greater than or equal to <paramref name="value"/>.
        /// If <paramref name="value"/> is 0 or the result overflows, returns 0.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CLSCompliant(false)]
        public static ulong RoundUpToPowerOf2(this ulong value)
        {
#if FEATURE_BITOPERATIONS_ROUNDUPTOPOWEROF2
            return System.Numerics.BitOperations.RoundUpToPowerOf2(value);
#elif FEATURE_NUMERICBITOPERATIONS
            int shift = 64 - LeadingZeroCount(value - 1);
            return (1ul ^ (ulong)(shift >> 6)) << shift;
#else
            // Based on https://graphics.stanford.edu/~seander/bithacks.html#RoundUpPowerOf2
            --value;
            value |= value >> 1;
            value |= value >> 2;
            value |= value >> 4;
            value |= value >> 8;
            value |= value >> 16;
            value |= value >> 32;
            return value + 1;
#endif
        }

        /// <summary>
        /// Round the given integral value up to a power of 2.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The smallest power of 2 which is greater than or equal to <paramref name="value"/>.
        /// If <paramref name="value"/> is 0 or the result overflows, returns 0.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CLSCompliant(false)]
        public static nuint RoundUpToPowerOf2(this nuint value)
        {
            if (IntPtr.Size == 8) // 64-bit process
            {
                return (nuint)RoundUpToPowerOf2((ulong)value);
            }
            else
            {
                return (nuint)RoundUpToPowerOf2((uint)value);
            }
        }

        #endregion RoundUpToPowerOf2

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

        #endregion TripleShift
    }
}
