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

using J2N.Numerics;
using J2N.Text;
using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

namespace J2N
{
    /// <summary>
    /// Extensions to the <see cref="double"/> structure.
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

        #region ToHexString

        /// <summary>
        /// Returns a hexadecimal string representation of the <see cref="double"/> argument. All characters mentioned below are ASCII characters.
        /// <list type="bullet">
        ///     <item><description>If the argument is <see cref="double.NaN"/>, the result is <see cref="NumberFormatInfo.NumberDecimalSeparator"/>
        ///         of the <paramref name="provider"/>.</description></item>
        ///     <item><description>Otherwise, the result is a string that represents the sign and magnitude of the argument. If the sign
        ///         is negative, it is prefixed by <see cref="NumberFormatInfo.NegativeSign"/> of the <paramref name="provider"/>; if the
        ///         sign is positive, no sign character appears in the result. As for the magnitude <i>m</i>: </description>
        ///         <list type="bullet">
        ///             <item><description>If <i>m</i> is positive infinity, it is represented by <see cref="NumberFormatInfo.PositiveInfinitySymbol"/> of the <paramref name="provider"/>;
        ///                 if <i>m</i> is negative infinity, it is represented by <see cref="NumberFormatInfo.NegativeInfinitySymbol"/> of the <paramref name="provider"/>.</description></item>
        ///             <item><description>If <i>m</i> is zero, it is represented by the string "0x0.0p0"; thus, negative zero produces the result
        ///                 "-0x0.0p0" and positive zero produces the result "0x0.0p0". The negative symbol is represented by <see cref="NumberFormatInfo.NegativeSign"/>
        ///                 and decimal separator character is represented by <see cref="NumberFormatInfo.NumberDecimalSeparator"/>.</description></item>
        ///             <item><description>If <i>m</i> is a <see cref="double"/> value with a normalized representation, substrings are used to represent the significand
        ///                 and exponent fields. The significand is represented by the characters "0x1" followed by <see cref="NumberFormatInfo.NumberDecimalSeparator"/>,
        ///                 followed by a lowercase hexadecimal representation of the rest of the significand as a fraction. Trailing zeros in the hexadecimal representation are
        ///                 removed unless all the digits are zero, in which case a single zero is used. Next, the exponent is represented by "p"
        ///                 followed by a decimal string of the unbiased exponent as if produced by a call to <see cref="int.ToString()"/> with invariant culture on the exponent value. </description></item>
        ///             <item><description>If <i>m</i> is a <see cref="double"/> value with a subnormal representation, the significand is represented by the characters "0x0"
        ///                 followed by <see cref="NumberFormatInfo.NumberDecimalSeparator"/>, followed by a hexadecimal representation of the rest of the significand as a fraction.
        ///                 Trailing zeros in the hexadecimal representation are removed. Next, the exponent is represented by "p-1022". Note that there must be at least one nonzero
        ///                 digit in a subnormal significand. </description></item>
        ///         </list>
        ///     </item>
        /// </list>
        /// <para/>
        /// The value of <see cref="NumberFormatInfo.NumberNegativePattern"/> of <paramref name="provider"/> is ignored.
        /// <para/>
        /// <h3>Examples (using <see cref="NumberFormatInfo.InvariantInfo"/>)</h3>
        /// <list type="table">
        ///     <listheader>
        ///         <term>Floating-point Value</term>
        ///         <term>Hexadecimal String</term>
        ///     </listheader>
        ///     <item>
        ///         <term>1.0</term>
        ///         <term>0x1.0p0</term>
        ///     </item>
        ///     <item>
        ///         <term>-1.0</term>
        ///         <term>-0x1.0p0</term>
        ///     </item>
        ///     <item>
        ///         <term>2.0</term>
        ///         <term>0x1.0p1</term>
        ///     </item>
        ///     <item>
        ///         <term>3.0</term>
        ///         <term>0x1.8p1</term>
        ///     </item>
        ///     <item>
        ///         <term>0.5</term>
        ///         <term>0x1.0p-1</term>
        ///     </item>
        ///     <item>
        ///         <term>0.25</term>
        ///         <term>0x1.0p-2</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="double.MaxValue"/></term>
        ///         <term>0x1.fffffffffffffp1023</term>
        ///     </item>
        ///     <item>
        ///         <term>Minimum Normal Value</term>
        ///         <term>0x1.0p-1022</term>
        ///     </item>
        ///     <item>
        ///         <term>Maximum Subnormal Value</term>
        ///         <term>0x0.fffffffffffffp-1022</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="double.Epsilon"/></term>
        ///         <term>0x0.0000000000001p-1022</term>
        ///     </item>
        /// </list>
        /// <para/>
        /// Usage Note: To exactly match the behavior of the JDK, use <see cref="NumberFormatInfo.InvariantInfo"/>
        /// for <paramref name="provider"/>.
        /// </summary>
        /// <param name="value">The double to be converted.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <returns>A hex string representing <paramref name="value"/>.</returns>
        public static string ToHexString(this double value, IFormatProvider? provider)
        {
            var info = NumberFormatInfo.GetInstance(provider);
            if (!value.IsFinite())
            {
                if (double.IsNaN(value))
                {
                    return info.NaNSymbol;
                }

                return value.IsNegative() ? info.NegativeInfinitySymbol : info.PositiveInfinitySymbol;
            }
            return ToHexString(value, info, upperCase: false);
        }

        internal static string ToHexString(this double value, NumberFormatInfo info, bool upperCase)
        {
            /*
             * Reference: http://en.wikipedia.org/wiki/IEEE_754
             */
            long bitValue = BitConversion.DoubleToInt64Bits(value);

            bool negative = (bitValue & unchecked((long)0x8000000000000000L)) != 0;
            // mask exponent bits and shift down
            long exponent = (bitValue & 0x7FF0000000000000L).TripleShift(52);
            // mask significand bits and shift up
            long significand = bitValue & 0x000FFFFFFFFFFFFFL;


            if (exponent == 0 && significand == 0)
            {
                if (upperCase)
                    return string.Concat(negative ? info.NegativeSign : "", "0X0", info.NumberDecimalSeparator, "0P0"); //$NON-NLS-1$ //$NON-NLS-2$
                else
                    return string.Concat(negative ? info.NegativeSign : "", "0x0", info.NumberDecimalSeparator, "0p0"); //$NON-NLS-1$ //$NON-NLS-2$
            }

#if FEATURE_SPAN
            ValueStringBuilder hexString = new ValueStringBuilder(stackalloc char[32]);
#else
            StringBuilder hexString = new StringBuilder(10);
#endif
            if (negative)
            {
                hexString.Append(info.NegativeSign);
            }
            hexString.Append(upperCase ? "0X" : "0x"); //$NON-NLS-1$

            if (exponent == 0)
            { // denormal (subnormal) value
                hexString.Append('0'); //$NON-NLS-1$
                hexString.Append(info.NumberDecimalSeparator);
                // significand is 52-bits, so there can be 13 hex digits
                int fractionDigits = 13;
                // remove trailing hex zeros, so long.ToHexString() won't print
                // them
                while ((significand != 0) && ((significand & 0xF) == 0))
                {
                    significand = significand.TripleShift(4);
                    fractionDigits--;
                }
                string hexSignificand = significand.ToString(upperCase ? "X" : "x", info);

                // if there are digits left, then insert some '0' chars first
                if (significand != 0 && fractionDigits > hexSignificand.Length)
                {
                    int digitDiff = fractionDigits - hexSignificand.Length;
                    while (digitDiff-- != 0)
                    {
                        hexString.Append('0');
                    }
                }
                hexString.Append(hexSignificand);
                hexString.Append(upperCase ? "P-1022" : "p-1022"); //$NON-NLS-1$
            }
            else
            { // normal value
                hexString.Append('1'); //$NON-NLS-1$
                hexString.Append(info.NumberDecimalSeparator);
                // significand is 52-bits, so there can be 13 hex digits
                int fractionDigits = 13;
                // remove trailing hex zeros, so long.ToHexString() won't print
                // them
                while ((significand != 0) && ((significand & 0xF) == 0))
                {
                    significand = significand.TripleShift(4);
                    fractionDigits--;
                }
                string hexSignificand = significand.ToString(upperCase ? "X" : "x", info);

                // if there are digits left, then insert some '0' chars first
                if (significand != 0 && fractionDigits > hexSignificand.Length)
                {
                    int digitDiff = fractionDigits - hexSignificand.Length;
                    while (digitDiff-- != 0)
                    {
                        hexString.Append('0');
                    }
                }

                hexString.Append(hexSignificand);
                hexString.Append(upperCase ? "P" : "p"); //$NON-NLS-1$
                // remove exponent's 'bias' and convert to a string
                hexString.Append((exponent - 1023).ToString(CultureInfo.InvariantCulture));
            }
            return hexString.ToString();
        }

        #endregion ToHexString
    }
}
