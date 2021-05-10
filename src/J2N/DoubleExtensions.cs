using J2N.Numerics;
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
        private const double NegativeZero = -0.0d;

        /// <summary>
        /// Gets a value indicating whether the current <see cref="double"/> has the value negative zero (<c>-0.0d</c>).
        /// While negative zero is supported by the <see cref="double"/> datatype in .NET, comparisons and string formatting ignore
        /// this feature. This method allows a simple way to check whether the current <see cref="double"/> has the value negative zero.
        /// </summary>
        /// <param name="d">This <see cref="double"/>.</param>
        /// <returns><c>true</c> if the current value represents negative zero; otherwise, <c>false</c>.</returns>
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif 
        public static bool IsNegativeZero(this double d)
        {
            return (d == 0 && BitConversion.DoubleToRawInt64Bits(d) == BitConversion.DoubleToRawInt64Bits(NegativeZero));
        }

        /// <summary>
        /// Returns a hexadecimal string representation of the <see cref="double"/> argument. All characters mentioned below are ASCII characters.
        /// <list type="bullet">
        ///     <item><description>If the argument is <see cref="double.NaN"/>, the result is the string "NaN".</description></item>
        ///     <item><description>Otherwise, the result is a string that represents the sign and magnitude of the argument. If the sign
        ///         is negative, the first character of the result is '-' ('\u002D'); if the sign is positive, no sign character appears
        ///         in the result. As for the magnitude <i>m</i>: </description>
        ///         <list type="bullet">
        ///             <item><description>If <i>m</i> is infinity, it is represented by the string "Infinity"; thus, positive infinity produces
        ///                 the result "Infinity" and negative infinity produces the result "-Infinity".</description></item>
        ///             <item><description>If <i>m</i> is zero, it is represented by the string "0x0.0p0"; thus, negative zero produces the result
        ///                 "-0x0.0p0" and positive zero produces the result "0x0.0p0". </description></item>
        ///             <item><description>If <i>m</i> is a <see cref="double"/> value with a normalized representation, substrings are used to represent the significand
        ///                 and exponent fields. The significand is represented by the characters "0x1." followed by a lowercase hexadecimal
        ///                 representation of the rest of the significand as a fraction. Trailing zeros in the hexadecimal representation are
        ///                 removed unless all the digits are zero, in which case a single zero is used. Next, the exponent is represented by "p"
        ///                 followed by a decimal string of the unbiased exponent as if produced by a call to <see cref="int.ToString()"/> with invariant culture on the exponent value. </description></item>
        ///             <item><description>If <i>m</i> is a <see cref="double"/> value with a subnormal representation, the significand is represented by the characters "0x0."
        ///                 followed by a hexadecimal representation of the rest of the significand as a fraction. Trailing zeros in the hexadecimal
        ///                 representation are removed. Next, the exponent is represented by "p-1022". Note that there must be at least one nonzero
        ///                 digit in a subnormal significand. </description></item>
        ///         </list>
        ///     </item>
        /// </list>
        /// <h3>Examples</h3>
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
        /// </summary>
        /// <param name="value">The double to be converted.</param>
        /// <returns>A hex string representing <paramref name="value"/>.</returns>
        public static string ToHexString(this double value)
        {
            /*
             * Reference: http://en.wikipedia.org/wiki/IEEE_754
             */
            if (double.IsNaN(value))
            {
                return "NaN"; //$NON-NLS-1$
            }
            if (double.IsPositiveInfinity(value))
            {
                return "Infinity"; //$NON-NLS-1$
            }
            if (double.IsNegativeInfinity(value))
            {
                return "-Infinity"; //$NON-NLS-1$
            }

            long bitValue = BitConversion.DoubleToInt64Bits(value);

            bool negative = (bitValue & unchecked((long)0x8000000000000000L)) != 0;
            // mask exponent bits and shift down
            long exponent = (bitValue & 0x7FF0000000000000L).TripleShift(52);
            // mask significand bits and shift up
            long significand = bitValue & 0x000FFFFFFFFFFFFFL;

            if (exponent == 0 && significand == 0)
            {
                return (negative ? "-0x0.0p0" : "0x0.0p0"); //$NON-NLS-1$ //$NON-NLS-2$
            }

            StringBuilder hexString = new StringBuilder(10);
            if (negative)
            {
                hexString.Append("-0x"); //$NON-NLS-1$
            }
            else
            {
                hexString.Append("0x"); //$NON-NLS-1$
            }

            if (exponent == 0)
            { // denormal (subnormal) value
                hexString.Append("0."); //$NON-NLS-1$
                // significand is 52-bits, so there can be 13 hex digits
                int fractionDigits = 13;
                // remove trailing hex zeros, so long.ToHexString() won't print
                // them
                while ((significand != 0) && ((significand & 0xF) == 0))
                {
                    significand = significand.TripleShift(4);
                    fractionDigits--;
                }
                // this assumes long.ToHexString() returns lowercase characters
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
                hexString.Append("p-1022"); //$NON-NLS-1$
            }
            else
            { // normal value
                hexString.Append("1."); //$NON-NLS-1$
                // significand is 52-bits, so there can be 13 hex digits
                int fractionDigits = 13;
                // remove trailing hex zeros, so long.ToHexString() won't print
                // them
                while ((significand != 0) && ((significand & 0xF) == 0))
                {
                    significand = significand.TripleShift(4);
                    fractionDigits--;
                }
                // this assumes long.ToHexString() returns lowercase characters
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
                hexString.Append((exponent - 1023).ToString(CultureInfo.InvariantCulture));
            }
            return hexString.ToString();
        }
    }
}
