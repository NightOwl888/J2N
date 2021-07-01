using J2N.Numerics;
using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

namespace J2N
{
    /// <summary>
    /// Extensions to the <see cref="float"/> structure.
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

        #region ToHexString

        /// <summary>
        /// Returns a hexadecimal string representation of the <see cref="float"/> argument. All characters mentioned below are ASCII characters.
        /// <list type="bullet">
        ///     <item><description>If the argument is <see cref="float.NaN"/>, the result is <see cref="NumberFormatInfo.NumberDecimalSeparator"/>
        ///         of the <paramref name="provider"/>. </description></item>
        ///     <item><description>Otherwise, the result is a string that represents the sign and magnitude of the argument. If the sign
        ///         is negative, it is prefixed by <see cref="NumberFormatInfo.NegativeSign"/> of the <paramref name="provider"/>; if the
        ///         sign is positive, no sign character appears in the result. As for the magnitude <i>m</i>: </description>
        ///         <list type="bullet">
        ///             <item><description>If <i>m</i> is positive infinity, it is represented by <see cref="NumberFormatInfo.PositiveInfinitySymbol"/> of the <paramref name="provider"/>;
        ///                 if <i>m</i> is negative infinity, it is represented by <see cref="NumberFormatInfo.NegativeInfinitySymbol"/> of the <paramref name="provider"/>.</description></item>
        ///             <item><description>If <i>m</i> is zero, it is represented by the string "0x0.0p0"; thus, negative zero produces the result
        ///                 "-0x0.0p0" and positive zero produces the result "0x0.0p0". The negative symbol is represented by <see cref="NumberFormatInfo.NegativeSign"/>
        ///                 and decimal separator character is represented by <see cref="NumberFormatInfo.NumberDecimalSeparator"/>.</description></item>
        ///             <item><description>If <i>m</i> is a <see cref="float"/> value with a normalized representation, substrings are used to represent the
        ///                 significand and exponent fields. The significand is represented by the characters "0x1" followed by <see cref="NumberFormatInfo.NumberDecimalSeparator"/>,
        ///                 followed by a lowercase hexadecimal representation of the rest of the significand as a fraction. Trailing zeros in the hexadecimal representation
        ///                 are removed unless all the digits are zero, in which case a single zero is used. Next, the exponent is represented by "p"
        ///                 followed by a decimal string of the unbiased exponent as if produced by a call to <see cref="int.ToString()"/> with invariant culture on the exponent value. </description></item>
        ///             <item><description>If <i>m</i> is a <see cref="float"/> value with a subnormal representation, the significand is represented by the characters "0x0"
        ///                 followed by <see cref="NumberFormatInfo.NumberDecimalSeparator"/>, followed by a hexadecimal representation of the rest of the significand as a fraction.
        ///                 Trailing zeros in the hexadecimal representation are removed. Next, the exponent is represented by "p-126". Note that there must be at least one nonzero
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
        ///         <term><see cref="float.MaxValue"/></term>
        ///         <term>0x1.fffffep127</term>
        ///     </item>
        ///     <item>
        ///         <term>Minimum Normal Value</term>
        ///         <term>0x1.0p-126</term>
        ///     </item>
        ///     <item>
        ///         <term>Maximum Subnormal Value</term>
        ///         <term>0x0.fffffep-126</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="float.Epsilon"/></term>
        ///         <term>0x0.000002p-126</term>
        ///     </item>
        /// </list>
        /// </summary>
        /// <param name="value">The <see cref="float"/> to be converted.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <returns>A hex string representing <paramref name="value"/>.</returns>
        public static string ToHexString(this float value, IFormatProvider? provider)
        {
            var info = NumberFormatInfo.GetInstance(provider);
            /*
             * Reference: http://en.wikipedia.org/wiki/IEEE_754
             */
            if (float.IsNaN(value))
            {
                return info.NaNSymbol; //$NON-NLS-1$
            }
            if (float.IsPositiveInfinity(value))
            {
                return info.PositiveInfinitySymbol; //$NON-NLS-1$
            }
            if (float.IsNegativeInfinity(value))
            {
                return info.NegativeInfinitySymbol; //$NON-NLS-1$
            }

            int bitValue = BitConversion.SingleToInt32Bits(value);

            bool negative = (bitValue & 0x80000000) != 0;
            // mask exponent bits and shift down
            int exponent = (bitValue & 0x7f800000).TripleShift(23);
            // mask significand bits and shift up
            // significand is 23-bits, so we shift to treat it like 24-bits
            int significand = (bitValue & 0x007FFFFF) << 1;

            if (exponent == 0 && significand == 0)
            {
                return string.Concat(negative ? info.NegativeSign : "", "0x0", info.NumberDecimalSeparator, "0p0"); //$NON-NLS-1$ //$NON-NLS-2$
            }

            StringBuilder hexString = new StringBuilder(10);
            if (negative)
            {
                hexString.Append(info.NegativeSign);
            }
            hexString.Append("0x"); //$NON-NLS-1$

            if (exponent == 0)
            { // denormal (subnormal) value
                hexString.Append('0'); //$NON-NLS-1$
                hexString.Append(info.NumberDecimalSeparator);
                // significand is 23-bits, so there can be 6 hex digits
                int fractionDigits = 6;
                // remove trailing hex zeros, so int.ToHexString() won't print
                // them
                while ((significand != 0) && ((significand & 0xF) == 0))
                {
                    //significand >>>= 4;
                    significand = significand.TripleShift(4);
                    fractionDigits--;
                }
                // this assumes int.ToHexString() returns lowercase characters
                string hexSignificand = significand.ToHexString();

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
                hexString.Append("p-126"); //$NON-NLS-1$
            }
            else
            { // normal value
                hexString.Append('1'); //$NON-NLS-1$
                hexString.Append(info.NumberDecimalSeparator);
                // significand is 23-bits, so there can be 6 hex digits
                int fractionDigits = 6;
                // remove trailing hex zeros, so Integer.toHexString() won't print
                // them
                while ((significand != 0) && ((significand & 0xF) == 0))
                {
                    //significand >>>= 4;
                    significand = significand.TripleShift(4);
                    fractionDigits--;
                }
                // this assumes int.ToHexString() returns lowercase characters
                string hexSignificand = significand.ToHexString();

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
                hexString.Append('p');
                // remove exponent's 'bias' and convert to a string
                hexString.Append((exponent - 127).ToString(CultureInfo.InvariantCulture));
            }
            return hexString.ToString();
        }

        #endregion ToHexString
    }
}
