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

using System.Runtime.CompilerServices;


namespace J2N
{
    /// <summary>
    /// Extensions to the <see cref="System.Double"/> class.
    /// </summary>
    public static class DoubleExtensions
    {
        #region IsFinite

        /// <summary>
        /// Determines whether the specified value is finite (zero, subnormal, or normal).
        /// </summary>
        /// <param name="d">A double-precision floating-point number.</param>
        /// <returns><c>true</c> if the value is finite (zero, subnormal or normal); otherwise <c>false</c>.</returns>
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif 
        public static unsafe bool IsFinite(this double d)
        {
            long bits = BitConversion.DoubleToRawInt64Bits(d);
            return (bits & 0x7FFFFFFFFFFFFFFF) < 0x7FF0000000000000;
        }

        #endregion

        #region IsInfinity

        /// <summary>
        /// Returns a value indicating whether the specified value evaluates to negative or positive infinity.
        /// </summary>
        /// <param name="d">A double-precision floating-point number.</param>
        /// <returns><c>true</c> if the value evaluates to <see cref="double.PositiveInfinity"/> or
        /// <see cref="double.NegativeInfinity"/>; otherwise, <c>false</c>.</returns>
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif 
        public static unsafe bool IsInfinity(this double d)
        {
            long bits = BitConversion.DoubleToRawInt64Bits(d);
            return (bits & 0x7FFFFFFFFFFFFFFF) == 0x7FF0000000000000;
        }

        #endregion IsInfinity

        #region IsNaN

        /// <summary>
        /// Returns a value that indicates whether the specified value is not a number
        /// (<see cref="double.NaN"/>).
        /// </summary>
        /// <param name="d">A double-precision floating-point number.</param>
        /// <returns><c>true</c> if the value evaluates to <see cref="double.NaN"/>;
        /// otherwise, <c>false</c>.</returns>
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif 
        public static bool IsNaN(this double d)
        {
            // A NaN will never equal itself so this is an
            // easy and efficient way to check for NaN.

#pragma warning disable CS1718
            return d != d;
#pragma warning restore CS1718
        }

        #endregion IsNaN

        #region IsNegative

        /// <summary>
        /// Determines whether the specified value is negative.
        /// </summary>
        /// <param name="d">A double-precision floating-point number.</param>
        /// <returns><c>true</c> if the value is negative; otherwise, <c>false</c>.</returns>

#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif 
        public static unsafe bool IsNegative(this double d)
        {
            return BitConversion.DoubleToRawInt64Bits(d) < 0;
        }

        #endregion IsNegative

        #region IsNegativeInfinity

        /// <summary>
        /// Returns a value indicating whether the specified number evaluates to negative
        /// infinity.
        /// </summary>
        /// <param name="d">A double-precision floating-point number.</param>
        /// <returns><c>true</c> if <paramref name="d"/> evaluates to <see cref="double.NegativeInfinity"/>;
        /// otherwise, <c>false</c>.</returns>
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif 
        public static unsafe bool IsNegativeInfinity(this double d)
        {
            return d == double.NegativeInfinity;
        }

        #endregion IsNegativeInfinity

        #region IsNegativeZero

        /// <summary>
        /// Gets a value indicating whether the current <see cref="double"/> has the value negative zero (<c>-0.0d</c>).
        /// While negative zero is supported by the <see cref="double"/> datatype in .NET, comparisons and string formatting ignore
        /// this feature. This method allows a simple way to check whether the current <see cref="double"/> has the value negative zero.
        /// </summary>
        /// <param name="d">A double-precision floating-point number.</param>
        /// <returns><c>true</c> if the current value represents negative zero; otherwise, <c>false</c>.</returns>
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif 
        public static bool IsNegativeZero(this double d)
        {
            return d == 0 && IsNegative(d);
        }

        #endregion IsNegativeZero

        #region IsNormal

        /// <summary>
        /// Determines whether the specified value is normal.
        /// </summary>
        /// <param name="d">A double-precision floating-point number.</param>
        /// <returns><c>true</c> if the value is normal; <c>false</c> otherwise.</returns>
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif 
        public static unsafe bool IsNormal(this double d)
        {
            long bits = BitConversion.DoubleToRawInt64Bits(d);
            bits &= 0x7FFFFFFFFFFFFFFF;
            return (bits < 0x7FF0000000000000) && (bits != 0) && ((bits & 0x7FF0000000000000) != 0);
        }

        #endregion IsNormal

        #region IsPositiveInfinity

        /// <summary>
        /// Returns a value indicating whether the specified number evaluates to positive infinity.
        /// </summary>
        /// <param name="d">A double-precision floating-point number.</param>
        /// <returns><c>true</c> if <paramref name="d"/> evaluates to <see cref="double.PositiveInfinity"/>; otherwise, <c>false</c>.</returns>
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif 
        public static bool IsPositiveInfinity(this double d)
        {
            return d == double.PositiveInfinity;
        }

        #endregion IsPositiveInfinity

        #region IsSubnormal

        /// <summary>
        /// Determines whether the specified value is subnormal.
        /// </summary>
        /// <param name="d">A double-precision floating-point number.</param>
        /// <returns><c>true</c> if the value is subnormal; <c>false</c> otherwise.</returns>
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif 
        public static unsafe bool IsSubnormal(this double d)
        {
            long bits = BitConversion.DoubleToRawInt64Bits(d);
            bits &= 0x7FFFFFFFFFFFFFFF;
            return (bits < 0x7FF0000000000000) && (bits != 0) && ((bits & 0x7FF0000000000000) == 0);
        }

        #endregion IsSubnormal

    }
}
