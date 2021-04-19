﻿using J2N.Numerics;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
#nullable enable

namespace J2N
{
    /// <summary>
    /// Additions to <see cref="System.Math"/>, implemented as extension methods.
    /// </summary>
    public static class MathExtensions
    {
        /// <summary>
        /// Bit mask to isolate the sign bit of a <see cref="float"/>.
        /// </summary>
        private const int SingleSignBitMask = unchecked((int)0x80000000);

        /// <summary>
        /// Bit mask to isolate the sign bit of a <see cref="double"/>.
        /// </summary>
        private const long DoubleSignBitMask = unchecked((long)0x8000000000000000L);

        /// <summary>
        /// Returns the signum function of the specified <see cref="int"/> value. (The
        /// return value is <c>-1</c> if the specified value is negative; <c>0</c> if the
        /// specified value is zero; and <c>1</c> if the specified value is positive.)
        /// <para/>
        /// This can be useful for testing the results of two <see cref="IComparable{T}.CompareTo(T)"/>
        /// methods against each other, since only the sign is guaranteed to be the same between implementations.
        /// </summary>
        /// <param name="value">The value whose signum has to be computed.</param>
        /// <returns>The signum function of the specified <see cref="int"/> value.</returns>
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif 
        public static int Signum(this int value)
        {
            // HD, Section 2-7
            return (value >> 31) | ((-value).TripleShift(31));
        }

        /// <summary>
        /// Returns the signum function of the specified <see cref="long"/> value. (The
        /// return value is <c>-1</c> if the specified value is negative; <c>0</c> if the
        /// specified value is zero; and <c>1</c> if the specified value is positive.)
        /// </summary>
        /// <param name="value">The value whose signum has to be computed.</param>
        /// <returns>The signum function of the specified <see cref="long"/> value.</returns>
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif 
        public static int Signum(this long value)
        {
            // HD, Section 2-7
            return (int)((value >> 63) | ((-value).TripleShift(63)));
        }

        /// <summary>
        /// Converts an angle measured in degrees to an approximately equivalent angle 
        /// measured in radians. The conversion from degrees to radians is generally inexact.
        /// </summary>
        /// <param name="degrees">An angle in degrees to convert to radians</param>
        /// <returns>The value in radians</returns>
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif 
        public static double ToRadians(this double degrees)
        {
            return degrees / 180 * Math.PI;
        }

        /// <summary>
        /// Converts an angle measured in degrees to an approximately equivalent angle 
        /// measured in radians. The conversion from degrees to radians is generally inexact.
        /// </summary>
        /// <param name="degrees">An angle in degrees to convert to radians</param>
        /// <returns>The value in radians</returns>
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif 
        public static decimal ToRadians(this decimal degrees)
        {
            return degrees / 180 * (decimal)Math.PI;
        }

        /// <summary>
        /// Converts an angle measured in degrees to an approximately equivalent angle 
        /// measured in radians. The conversion from degrees to radians is generally inexact.
        /// </summary>
        /// <param name="degrees">An angle in degrees to convert to radians</param>
        /// <returns>The value in radians</returns>
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif 
        public static double ToRadians(this int degrees)
        {
            return ((double)degrees) / 180 * Math.PI;
        }

        /// <summary>
        /// Converts an angle measured in radians to an approximately equivalent angle 
        /// measured in degrees. The conversion from radians to degrees is generally 
        /// inexact; users should not expect Cos((90.0).ToRadians()) to exactly equal 0.0.
        /// </summary>
        /// <param name="radians">An angle in radians to convert to radians</param>
        /// <returns>The value in radians</returns>
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif 
        public static double ToDegrees(this double radians)
        {
            return radians * 180 / Math.PI;
        }

        /// <summary>
        /// Converts an angle measured in radians to an approximately equivalent angle 
        /// measured in degrees. The conversion from radians to degrees is generally 
        /// inexact; users should not expect Cos((90.0).ToRadians()) to exactly equal 0.0.
        /// </summary>
        /// <param name="radians">An angle in radians to convert to radians</param>
        /// <returns>The value in radians</returns>
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif 
        public static decimal ToDegrees(this decimal radians)
        {
            return radians * 180 / (decimal)Math.PI;
        }

        /// <summary>
        /// Converts an angle measured in radians to an approximately equivalent angle 
        /// measured in degrees. The conversion from radians to degrees is generally 
        /// inexact; users should not expect Cos((90.0).ToRadians()) to exactly equal 0.0.
        /// </summary>
        /// <param name="radians">An angle in radians to convert to radians</param>
        /// <returns>The value in radians</returns>
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif 
        public static double ToDegrees(this int radians)
        {
            return ((double)radians) * 180 / Math.PI;
        }

        /// <summary>
        /// Returns the floating-point number adjacent to the first
        /// argument in the direction of the second argument. If both
        /// arguments compare as equal a value equivalent to the second argument
        /// is returned.
        /// <para/>
        /// <para/>
        /// Special Cases:
        /// <list type="bullet">
        ///     <item><description>If either argument is <see cref="double.NaN"/>, the result is <see cref="double.NaN"/>.</description></item>
        ///     <item><description>If both arguments are signed zeros, a value equivalent to <paramref name="direction"/> is returned.</description></item>
        ///     <item><description>If <paramref name="start"/> is &#177;<see cref="double.MinValue"/> and <paramref name="direction"/> has a value
        ///         such that the result should have a smaller magnitude, then a zero with the same sign as <paramref name="start"/> is returned.</description></item>
        ///     <item><description>If <paramref name="start"/> is infinite and <paramref name="direction"/> has a value such that the result
        ///     should have a smaller magnitude, <see cref="double.MaxValue"/> with the same sign as <paramref name="start"/> is returned.</description></item>
        ///     <item><description>If <paramref name="start"/> is equal to &#177;<see cref="double.MaxValue"/> and <paramref name="direction"/>
        ///         has a value such that the result should have a larger magnitude, an infinity with same sign as <paramref name="start"/> is returned.</description></item>
        /// </list>
        /// </summary>
        /// <param name="start">The starting floating-point value.</param>
        /// <param name="direction">A value indicating which of <paramref name="start"/>'s neighbors or <paramref name="start"/> should be returned.</param>
        /// <returns>The floating-point number adjacent to <paramref name="start"/> in the direction of <paramref name="direction"/>.</returns>
        public static double NextAfter(this double start, double direction)
        {
            /*
             * The cases:
             *
             * NextAfter(+infinity, 0)  == MaxValue
             * NextAfter(+infinity, +infinity)  == +infinity
             * NextAfter(-infinity, 0)  == -MaxValue
             * NextAfter(-infinity, -infinity)  == -infinity
             *
             * are naturally handled without any additional testing
             */

            // First check for NaN values
            if (double.IsNaN(start) || double.IsNaN(direction))
            {
                // return a NaN derived from the input NaN(s)
                return start + direction;
            }
            else if (start == direction)
            {
                return direction;
            }
            else
            {        // start > direction or start < direction
                     // Add +0.0 to get rid of a -0.0 (+0.0 + -0.0 => +0.0)
                     // then bitwise convert start to integer.
                long transducer = BitConversion.DoubleToRawInt64Bits(start + 0.0d);

                /*
                 * IEEE 754 floating-point numbers are lexicographically
                 * ordered if treated as signed- magnitude integers .
                 * Since .NET's integers are two's complement,
                 * incrementing" the two's complement representation of a
                 * logically negative floating-point value *decrements*
                 * the signed-magnitude representation. Therefore, when
                 * the integer representation of a floating-point values
                 * is less than zero, the adjustment to the representation
                 * is in the opposite direction than would be expected at
                 * first .
                 */
                if (direction > start)
                { // Calculate next greater value
                    transducer = transducer + (transducer >= 0L ? 1L : -1L);
                }
                else
                { // Calculate next lesser value
                    Debug.Assert(direction < start);
                    if (transducer > 0L)
                        --transducer;
                    else
                        if (transducer < 0L)
                        ++transducer;
                    /*
                     * transducer==0, the result is -MinValue
                     *
                     * The transition from zero (implicitly
                     * positive) to the smallest negative
                     * signed magnitude value must be done
                     * explicitly.
                     */
                    else
                        transducer = DoubleSignBitMask | 1L;
                }

                return BitConversion.Int64BitsToDouble(transducer);
            }
        }

        /// <summary>
        /// Returns the floating-point number adjacent to the first
        /// argument in the direction of the second argument. If both
        /// arguments compare as equal a value equivalent to the second argument
        /// is returned.
        /// <para/>
        /// <para/>
        /// Special Cases:
        /// <list type="bullet">
        ///     <item><description>If either argument is <see cref="float.NaN"/>, the result is <see cref="float.NaN"/>.</description></item>
        ///     <item><description>If both arguments are signed zeros, a value equivalent to <paramref name="direction"/> is returned.</description></item>
        ///     <item><description>If <paramref name="start"/> is &#177;<see cref="float.MinValue"/> and <paramref name="direction"/> has a value
        ///         such that the result should have a smaller magnitude, then a zero with the same sign as <paramref name="start"/> is returned.</description></item>
        ///     <item><description>If <paramref name="start"/> is infinite and <paramref name="direction"/> has a value such that the result
        ///     should have a smaller magnitude, <see cref="float.MaxValue"/> with the same sign as <paramref name="start"/> is returned.</description></item>
        ///     <item><description>If <paramref name="start"/> is equal to &#177;<see cref="float.MaxValue"/> and <paramref name="direction"/>
        ///         has a value such that the result should have a larger magnitude, an infinity with same sign as <paramref name="start"/> is returned.</description></item>
        /// </list>
        /// </summary>
        /// <param name="start">The starting floating-point value.</param>
        /// <param name="direction">A value indicating which of <paramref name="start"/>'s neighbors or <paramref name="start"/> should be returned.</param>
        /// <returns>The floating-point number adjacent to <paramref name="start"/> in the direction of <paramref name="direction"/>.</returns>
        public static float NextAfter(this float start, double direction)
        {
            /*
             * The cases:
             *
             * NextAfter(+infinity, 0)  == MaxValue
             * NextAfter(+infinity, +infinity)  == +infinity
             * NextAfter(-infinity, 0)  == -MaxValue
             * NextAfter(-infinity, -infinity)  == -infinity
             *
             * are naturally handled without any additional testing
             */

            // First check for NaN values
            if (float.IsNaN(start) || double.IsNaN(direction))
            {
                // return a NaN derived from the input NaN(s)
                return start + (float)direction;
            }
            else if (start == direction)
            {
                return (float)direction;
            }
            else
            {        // start > direction or start < direction
                     // Add +0.0 to get rid of a -0.0 (+0.0 + -0.0 => +0.0)
                     // then bitwise convert start to integer.
                int transducer = BitConversion.SingleToRawInt32Bits(start + 0.0f);

                /*
                 * IEEE 754 floating-point numbers are lexicographically
                 * ordered if treated as signed- magnitude integers .
                 * Since .NET's integers are two's complement,
                 * incrementing" the two's complement representation of a
                 * logically negative floating-point value *decrements*
                 * the signed-magnitude representation. Therefore, when
                 * the integer representation of a floating-point values
                 * is less than zero, the adjustment to the representation
                 * is in the opposite direction than would be expected at
                 * first.
                 */
                if (direction > start)
                {// Calculate next greater value
                    transducer = transducer + (transducer >= 0 ? 1 : -1);
                }
                else
                { // Calculate next lesser value
                    Debug.Assert(direction < start);
                    if (transducer > 0)
                        --transducer;
                    else
                        if (transducer < 0)
                        ++transducer;
                    /*
                     * transducer==0, the result is -MinValue
                     *
                     * The transition from zero (implicitly
                     * positive) to the smallest negative
                     * signed magnitude value must be done
                     * explicitly.
                     */
                    else
                        transducer = SingleSignBitMask | 1;
                }

                return BitConversion.Int32BitsToSingle(transducer);
            }
        }

        /// <summary>
        /// Returns the floating-point value adjacent to <paramref name="value"/> in
        /// the direction of positive infinity. This method is
        /// semantically equivalent to <c>NextAfter(value, double.PositiveInfinity)</c>;
        /// however, a <see cref="NextUp(double)"/>
        /// implementation may run faster than its equivalent
        /// <see cref="NextAfter(double, double)"/> call.
        /// <para/>
        /// Special Cases:
        /// <list type="bullet">
        ///     <item><description>If the argument is <see cref="double.NaN"/>, the result is <see cref="double.NaN"/>.</description></item>
        ///     <item><description>If the argument is <see cref="double.PositiveInfinity"/>, the result is <see cref="double.PositiveInfinity"/>.</description></item>
        ///     <item><description>If the argument is zero, the result is <see cref="double.MinValue"/>.</description></item>
        /// </list>
        /// </summary>
        /// <param name="value">The starting floating-point value.</param>
        /// <returns>The adjacent floating-point value closer to positive infinity.</returns>
        public static double NextUp(this double value)
        {
            if (double.IsNaN(value) || value == double.PositiveInfinity)
                return value;
            else
            {
                value += 0.0d;
                return BitConversion.Int64BitsToDouble(BitConversion.DoubleToRawInt64Bits(value) +
                                               ((value >= 0.0d) ? +1L : -1L));
            }
        }

        /// <summary>
        /// Returns the floating-point value adjacent to <paramref name="value"/> in
        /// the direction of positive infinity. This method is
        /// semantically equivalent to <c>NextAfter(value, float.PositiveInfinity)</c>;
        /// however, a <see cref="NextUp(float)"/>
        /// implementation may run faster than its equivalent
        /// <see cref="NextAfter(float, double)"/> call.
        /// <para/>
        /// Special Cases:
        /// <list type="bullet">
        ///     <item><description>If the argument is <see cref="float.NaN"/>, the result is <see cref="float.NaN"/>.</description></item>
        ///     <item><description>If the argument is <see cref="float.PositiveInfinity"/>, the result is <see cref="float.PositiveInfinity"/>.</description></item>
        ///     <item><description>If the argument is zero, the result is <see cref="float.MinValue"/>.</description></item>
        /// </list>
        /// </summary>
        /// <param name="value">The starting floating-point value.</param>
        /// <returns>The adjacent floating-point value closer to positive infinity.</returns>
        public static float NextUp(this float value)
        {
            if (float.IsNaN(value) || value == float.PositiveInfinity)
                return value;
            else
            {
                value += 0.0f;
                return BitConversion.Int32BitsToSingle(BitConversion.SingleToRawInt32Bits(value) +
                                            ((value >= 0.0f) ? +1 : -1));
            }
        }

        /// <summary>
        /// Returns the floating-point value adjacent to <paramref name="value"/> in
        /// the direction of negative infinity.  This method is
        /// semantically equivalent to <c>NextAfter(value, double.NegativeInfinity)</c>; however, a
        /// <see cref="NextDown(double)"/> implementation may run faster than its
        /// equivalent <see cref="NextAfter(double, double)"/> call.
        /// <para/>
        /// Special Cases:
        /// <list type="bullet">
        ///     <item><description>If the argument is <see cref="double.NaN"/>, the result is <see cref="double.NaN"/>.</description></item>
        ///     <item><description>If the argument is <see cref="double.NegativeInfinity"/>, the result is <see cref="double.NegativeInfinity"/>.</description></item>
        ///     <item><description>If the argument is zero, the result is <see cref="double.MinValue"/>.</description></item>
        /// </list>
        /// </summary>
        /// <param name="value">The starting floating-point value.</param>
        /// <returns>The adjacent floating-point value closer to negative infinity.</returns>
        public static double NextDown(this double value)
        {
            if (double.IsNaN(value) || value == double.NegativeInfinity)
                return value;
            else
            {
                if (value == 0.0)
                    return -double.MinValue;
                else
                    return BitConversion.Int64BitsToDouble(BitConversion.DoubleToRawInt64Bits(value) +
                                                   ((value > 0.0d) ? -1L : +1L));
            }
        }

        /// <summary>
        /// Returns the floating-point value adjacent to <paramref name="value"/> in
        /// the direction of negative infinity.  This method is
        /// semantically equivalent to <c>NextAfter(value, float.NegativeInfinity)</c>; however, a
        /// <see cref="NextDown(float)"/> implementation may run faster than its
        /// equivalent <see cref="NextAfter(float, double)"/> call.
        /// <para/>
        /// Special Cases:
        /// <list type="bullet">
        ///     <item><description>If the argument is <see cref="float.NaN"/>, the result is <see cref="float.NaN"/>.</description></item>
        ///     <item><description>If the argument is <see cref="float.NegativeInfinity"/>, the result is <see cref="float.NegativeInfinity"/>.</description></item>
        ///     <item><description>If the argument is zero, the result is <see cref="float.MinValue"/>.</description></item>
        /// </list>
        /// </summary>
        /// <param name="value">The starting floating-point value.</param>
        /// <returns>The adjacent floating-point value closer to negative infinity.</returns>
        public static float NextDown(this float value)
        {
            if (float.IsNaN(value) || value == float.NegativeInfinity)
                return value;
            else
            {
                if (value == 0.0f)
                    return -float.MinValue;
                else
                    return BitConversion.Int32BitsToSingle(BitConversion.SingleToRawInt32Bits(value) +
                                                ((value > 0.0f) ? -1 : +1));
            }
        }
    }
}
