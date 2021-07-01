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
    /// Extensions to the <see cref="System.Single"/> class.
    /// </summary>
    public static class SingleExtensions
    {
        #region IsFinite

        /// <summary>
        /// Determines whether the specified value is finite (zero, subnormal, or normal).
        /// </summary>
        /// <param name="f">A single-precision floating-point number.</param>
        /// <returns><c>true</c> if the value is finite (zero, subnormal or normal); otherwise <c>false</c>.</returns>
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif 
        public static unsafe bool IsFinite(this float f)
        {
            int bits = BitConversion.SingleToRawInt32Bits(f);
            return (bits & 0x7FFFFFFF) < 0x7F800000;
        }

        #endregion

        #region IsInfinity

        /// <summary>
        /// Returns a value indicating whether the specified value evaluates to negative or positive infinity.
        /// </summary>
        /// <param name="f">A single-precision floating-point number.</param>
        /// <returns><c>true</c> if the value evaluates to <see cref="float.PositiveInfinity"/> or
        /// <see cref="float.NegativeInfinity"/>; otherwise, <c>false</c>.</returns>
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif 
        public static unsafe bool IsInfinity(this float f)
        {
            int bits = BitConversion.SingleToRawInt32Bits(f);
            return (bits & 0x7FFFFFFF) == 0x7F800000;
        }

        #endregion IsInfinity

        #region IsNaN

        /// <summary>
        /// Returns a value that indicates whether the specified value is not a number
        /// (<see cref="float.NaN"/>).
        /// </summary>
        /// <param name="f">A single-precision floating-point number.</param>
        /// <returns><c>true</c> if the value evaluates to <see cref="float.NaN"/>;
        /// otherwise, <c>false</c>.</returns>
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif 
        public static bool IsNaN(this float f)
        {
            // A NaN will never equal itself so this is an
            // easy and efficient way to check for NaN.

#pragma warning disable CS1718
            return f != f;
#pragma warning restore CS1718
        }

        #endregion IsNaN

        #region IsNegative

        /// <summary>
        /// Determines whether the specified value is negative.
        /// </summary>
        /// <param name="f">A single-precision floating-point number.</param>
        /// <returns><c>true</c> if the value is negative; otherwise, <c>false</c>.</returns>

#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif 
        public static unsafe bool IsNegative(this float f)
        {
            return BitConversion.SingleToRawInt32Bits(f) < 0;
        }

        #endregion IsNegative

        #region IsNegativeInfinity

        /// <summary>
        /// Returns a value indicating whether the specified number evaluates to negative
        /// infinity.
        /// </summary>
        /// <param name="f">A single-precision floating-point number.</param>
        /// <returns><c>true</c> if <paramref name="f"/> evaluates to <see cref="float.NegativeInfinity"/>;
        /// otherwise, <c>false</c>.</returns>
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif 
        public static unsafe bool IsNegativeInfinity(this float f)
        {
            return f == float.NegativeInfinity;
        }

        #endregion IsNegativeInfinity

        #region IsNegativeZero

        /// <summary>
        /// Gets a value indicating whether the current <see cref="float"/> has the value negative zero (<c>-0.0f</c>).
        /// While negative zero is supported by the <see cref="float"/> datatype in .NET, comparisons and string formatting ignore
        /// this feature. This method allows a simple way to check whether the current <see cref="float"/> has the value negative zero.
        /// </summary>
        /// <param name="f">A single-precision floating-point number.</param>
        /// <returns><c>true</c> if the current value represents negative zero; otherwise, <c>false</c>.</returns>
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif 
        public static bool IsNegativeZero(this float f)
        {
            return f == 0 && IsNegative(f);
        }

        #endregion IsNegativeZero

        #region IsNormal

        /// <summary>
        /// Determines whether the specified value is normal.
        /// </summary>
        /// <param name="f">A single-precision floating-point number.</param>
        /// <returns><c>true</c> if the value is normal; <c>false</c> otherwise.</returns>
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif 
        public static unsafe bool IsNormal(this float f)
        {
            int bits = BitConversion.SingleToRawInt32Bits(f);
            bits &= 0x7FFFFFFF;
            return (bits < 0x7F800000) && (bits != 0) && ((bits & 0x7F800000) != 0);
        }

        #endregion IsNormal

        #region IsPositiveInfinity

        /// <summary>
        /// Returns a value indicating whether the specified number evaluates to positive infinity.
        /// </summary>
        /// <param name="f">A single-precision floating-point number.</param>
        /// <returns><c>true</c> if <paramref name="f"/> evaluates to <see cref="float.PositiveInfinity"/>; otherwise, <c>false</c>.</returns>
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif 
        public static bool IsPositiveInfinity(this float f)
        {
            return f == float.PositiveInfinity;
        }

        #endregion IsPositiveInfinity

        #region IsSubnormal

        /// <summary>
        /// Determines whether the specified value is subnormal.
        /// </summary>
        /// <param name="f">A single-precision floating-point number.</param>
        /// <returns><c>true</c> if the value is subnormal; <c>false</c> otherwise.</returns>
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif 
        public static unsafe bool IsSubnormal(this float f)
        {
            int bits = BitConversion.SingleToRawInt32Bits(f);
            bits &= 0x7FFFFFFF;
            return (bits < 0x7F800000) && (bits != 0) && ((bits & 0x7F800000) == 0);
        }

        #endregion IsSubnormal
    }
}
