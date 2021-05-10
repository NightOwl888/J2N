using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using JCG = J2N.Collections.Generic;

namespace J2N.Numerics
{
    /// <inheritdoc/>
    public sealed class Double : Number, IComparable<Double>
    {
        /**
         * The value which the receiver represents.
         */
        private readonly double value;

        ////    /**
        ////     * Constant for the maximum {@code double} value, (2 - 2<sup>-52</sup>) *
        ////     * 2<sup>1023</sup>.
        ////     */
        ////    public static final double MAX_VALUE = 1.79769313486231570e+308;

        ////    /**
        ////     * Constant for the minimum {@code double} value, 2<sup>-1074</sup>.
        ////     */
        ////    public static final double MIN_VALUE = 5e-324;

        ////    /* 4.94065645841246544e-324 gets rounded to 9.88131e-324 */

        ////    /**
        ////     * Constant for the Not-a-Number (NaN) value of the {@code double} type.
        ////     */
        ////    public static final double NaN = 0.0 / 0.0;

        ////    /**
        ////     * Constant for the Positive Infinity value of the {@code double} type.
        ////     */
        ////    public static final double POSITIVE_INFINITY = 1.0 / 0.0;

        ////    /**
        ////     * Constant for the Negative Infinity value of the {@code double} type.
        ////     */
        ////    public static final double NEGATIVE_INFINITY = -1.0 / 0.0;

        ////    /**
        ////     * The {@link Class} object that represents the primitive type {@code
        ////     * double}.
        ////     *
        ////     * @since 1.1
        ////     */
        ////    @SuppressWarnings("unchecked")
        ////public static final Class<Double> TYPE = (Class<Double>)new double[0]
        ////        .getClass().getComponentType();

        ////    // Note: This can't be set to "double.class", since *that* is
        // defined to be "java.lang.Double.TYPE";

        /**
         * Constant for the number of bits needed to represent a {@code double} in
         * two's complement form.
         *
         * @since 1.5
         */
        public const int SIZE = 64;

        /**
         * Constructs a new {@code Double} with the specified primitive double
         * value.
         * 
         * @param value
         *            the primitive double value to store in the new instance.
         */
        public Double(double value)
        {
            this.value = value;
        }

        /**
         * Constructs a new {@code Double} from the specified string.
         * 
         * @param string
         *            the string representation of a double value.
         * @throws NumberFormatException
         *             if {@code string} can not be decoded into a double value.
         * @see #parseDouble(String)
         */
        public Double(string value)
            : this(ParseDouble(value))
        {
        }

        /**
         * Constructs a new {@code Double} from the specified string.
         * 
         * @param string
         *            the string representation of a double value.
         * @throws NumberFormatException
         *             if {@code string} can not be decoded into a double value.
         * @see #parseDouble(String)
         */
        public Double(string value, IFormatProvider? provider)
            : this(ParseDouble(value, provider))
        {
        }

        /**
         * Compares this object to the specified double object to determine their
         * relative order. There are two special cases:
         * <ul>
         * <li>{@code Double.NaN} is equal to {@code Double.NaN} and it is greater
         * than any other double value, including {@code Double.POSITIVE_INFINITY};</li>
         * <li>+0.0d is greater than -0.0d</li>
         * </ul>
         * 
         * @param object
         *            the double object to compare this object to.
         * @return a negative value if the value of this double is less than the
         *         value of {@code object}; 0 if the value of this double and the
         *         value of {@code object} are equal; a positive value if the value
         *         of this double is greater than the value of {@code object}.
         * @throws NullPointerException
         *             if {@code object} is {@code null}.
         * @see java.lang.Comparable
         * @since 1.2
         */
        public int CompareTo(Double? other)
        {
            if (other is null) return 1; // Using 1 if other is null as specified here: https://stackoverflow.com/a/4852537

            return Compare(value, other.value);
        }

        /// <inheritdoc/>
        public override byte GetByteValue()
        {
            return (byte)value;
        }

        /**
         * Converts the specified double value to a binary representation conforming
         * to the IEEE 754 floating-point double precision bit layout. All
         * <em>Not-a-Number (NaN)</em> values are converted to a single NaN
         * representation ({@code 0x7ff8000000000000L}).
         * 
         * @param value
         *            the double value to convert.
         * @return the IEEE 754 floating-point double precision representation of
         *         {@code value}.
         * @see #doubleToRawLongBits(double)
         * @see #longBitsToDouble(long)
         */
        public static long DoubleToInt64Bits(double value)
        {
            return BitConversion.DoubleToInt64Bits(value);
        }

        /**
         * Converts the specified double value to a binary representation conforming
         * to the IEEE 754 floating-point double precision bit layout.
         * <em>Not-a-Number (NaN)</em> values are preserved.
         * 
         * @param value
         *            the double value to convert.
         * @return the IEEE 754 floating-point double precision representation of
         *         {@code value}.
         * @see #doubleToLongBits(double)
         * @see #longBitsToDouble(long)
         */
        public static long DoubleToRawInt64Bits(double value)
        {
            return BitConversion.DoubleToRawInt64Bits(value);
        }

        /**
         * Gets the primitive value of this double.
         * 
         * @return this object's primitive value.
         */
        public override double GetDoubleValue()
        {
            return value;
        }

        /**
         * Compares this object with the specified object and indicates if they are
         * equal. In order to be equal, {@code object} must be an instance of
         * {@code Double} and the bit pattern of its double value is the same as
         * this object's.
         * 
         * @param object
         *            the object to compare this double with.
         * @return {@code true} if the specified object is equal to this
         *         {@code Double}; {@code false} otherwise.
         */
        public override bool Equals(object? obj)
        {
            if (obj is null) return false;

            return (obj == this)
                || (obj is Double other)
                && (BitConversion.DoubleToInt64Bits(this.value) == BitConversion.DoubleToInt64Bits(other.value));
        }

        /// <inheritdoc/>
        public override float GetSingleValue()
        {
            return (float)value;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            long v = BitConversion.DoubleToInt64Bits(value);
            return (int)(v ^ (v.TripleShift(32)));
        }

        /// <inheritdoc/>
        public override int GetInt32Value()
        {
            return (int)value;
        }

        /**
         * Indicates whether this object represents an infinite value.
         * 
         * @return {@code true} if the value of this double is positive or negative
         *         infinity; {@code false} otherwise.
         */
        public bool IsInfinity()
        {
            return IsInfinity(value);
        }

        /**
         * Indicates whether the specified double represents an infinite value.
         * 
         * @param d
         *            the double to check.
         * @return {@code true} if the value of {@code d} is positive or negative
         *         infinity; {@code false} otherwise.
         */
        public static bool IsInfinity(double d)
        {
            return double.IsInfinity(d);
            //return (d == POSITIVE_INFINITY) || (d == NEGATIVE_INFINITY);
        }

        /**
         * Indicates whether this object is a <em>Not-a-Number (NaN)</em> value.
         * 
         * @return {@code true} if this double is <em>Not-a-Number</em>;
         *         {@code false} if it is a (potentially infinite) double number.
         */
        public bool IsNaN()
        {
            return IsNaN(value);
        }

        /**
         * Indicates whether the specified double is a <em>Not-a-Number (NaN)</em>
         * value.
         * 
         * @param d
         *            the double value to check.
         * @return {@code true} if {@code d} is <em>Not-a-Number</em>;
         *         {@code false} if it is a (potentially infinite) double number.
         */
        public static bool IsNaN(double d)
        {
            return double.IsNaN(d);
            //return d != d;
        }

        /**
         * Converts the specified IEEE 754 floating-point double precision bit
         * pattern to a Java double value.
         * 
         * @param bits
         *            the IEEE 754 floating-point double precision representation of
         *            a double value.
         * @return the double value converted from {@code bits}.
         * @see #doubleToLongBits(double)
         * @see #doubleToRawLongBits(double)
         */
        public static double Int64BitsToDouble(long bits)
        {
            return BitConversion.Int64BitsToDouble(bits);
        }

        /// <inheritdoc/>
        public override long GetInt64Value()
        {
            return (long)value;
        }

        /**
         * Parses the specified string as a double value.
         * 
         * @param string
         *            the string representation of a double value.
         * @return the primitive double value represented by {@code string}.
         * @throws NumberFormatException
         *             if {@code string} is {@code null}, has a length of zero or
         *             can not be parsed as a double value.
         */
        public static double ParseDouble(string s)
        {
            return ParseDouble(s, J2N.Text.StringFormatter.CurrentCulture);

            //return FloatingPointParser.ParseDouble(s, J2N.Text.StringFormatter.CurrentCulture);
            //return org.apache.harmony.luni.util.FloatingPointParser
            //        .parseDouble(string);
        }

        /**
         * Parses the specified string as a double value.
         * 
         * @param string
         *            the string representation of a double value.
         * @return the primitive double value represented by {@code string}.
         * @throws NumberFormatException
         *             if {@code string} is {@code null}, has a length of zero or
         *             can not be parsed as a double value.
         */
        public static double ParseDouble(string s, IFormatProvider? provider)
        {
            // J2N: In .NET we don't throw on null, but return zero to match behavior of built-in parser.
            if (s is null)
                return 0.0d;

            s = s.Trim();
            if (s == string.Empty)
                throw new FormatException("The string was empty, which is not allowed."); // J2N TODO: Localize string

            if (provider is null ||
                CultureInfo.InvariantCulture.NumberFormat.Equals(provider.GetFormat(typeof(NumberFormatInfo))) ||
                FloatingDecimal.ParseAsHex(s))
            {
                return FloatingDecimal.ParseDouble(s); // J2N TODO: Culture
            }

            //return FloatingPointParser.ParseDouble(value, provider);
            //return org.apache.harmony.luni.util.FloatingPointParser
            //        .parseDouble(string);

            double result = double.Parse(s, provider); // J2N TODO: For now, fallback to .NET. We should respect the NumberFormatInfo settings in the Java parser/formatter, though.

            // .NET doesn't handle negative zero, so we need to do that here
            if (result == 0d && FloatingDecimal.IsNegative(s, provider))
                return -0.0d;

            return result;
        }

        ///// <summary>
        ///// Checks whether the given string is negative for the <paramref name="provider"/>. If <paramref name="provider"/>
        ///// is <c>null</c>, uses <see cref="CultureInfo.CurrentCulture"/>.
        ///// </summary>
        ///// <param name="s"></param>
        ///// <param name="provider"></param>
        ///// <returns></returns>
        //private static bool IsNegative(string s, IFormatProvider? provider)
        //{
        //    NumberFormatInfo numberFormat = provider is null ? CultureInfo.CurrentCulture.NumberFormat : (NumberFormatInfo)provider.GetFormat(typeof(NumberFormatInfo))!;
        //    string negativeSign = numberFormat.NegativeSign;
        //    switch (numberFormat.NumberNegativePattern)
        //    {
        //        case 0: // (1,234.00)
        //            return s[0] == '(' && s[s.Length - 1] == ')';
        //        case 2: // - 1,234.00
        //            if (s.Length < negativeSign.Length + 1) // space must be at the end of the sign
        //                return false;
        //            for (int i = 0; i < negativeSign.Length; i++)
        //                if (s[i] != negativeSign[i])
        //                    return false;
        //            if (s[negativeSign.Length] != ' ')
        //                return false;
        //            return true;
        //        case 3: // 1,234.00-
        //            if (s.Length < negativeSign.Length)
        //                return false;
        //            for (int i = s.Length - 1, j = negativeSign.Length - 1; j >= 0 ; i--, j--)
        //                if (s[i] != negativeSign[j])
        //                    return false;
        //            return true;
        //        case 4: // 1,234.00 -
        //            if (s.Length < negativeSign.Length + 1) // space must be at the beginning of the sign
        //                return false;
        //            for (int i = s.Length - 1, j = negativeSign.Length - 1; j >= 0; i--, j--)
        //                if (s[i] != negativeSign[j])
        //                    return false;
        //            if (s[s.Length - (negativeSign.Length + 1)] != ' ')
        //                return false;
        //            return true;
        //        default: // (1): -1,234.00
        //            if (s.Length < negativeSign.Length)
        //                return false;
        //            for (int i = 0; i < negativeSign.Length; i++)
        //                if (s[i] != negativeSign[i])
        //                    return false;
        //            return true;
        //    }
        //}

        ///*
        // * Answers true if the string should be parsed as a hex encoding.
        // * Assumes the string is trimmed.
        // */
        //private static bool ParseAsHex(string s)
        //{
        //    int length = s.Length;
        //    if (length < 2)
        //    {
        //        return false;
        //    }
        //    char first = s[0];
        //    char second = s[1];
        //    if (first == '+' || first == '-')
        //    {
        //        // Move along
        //        if (length < 3)
        //        {
        //            return false;
        //        }
        //        first = second;
        //        second = s[2];
        //    }
        //    return (first == '0') && (second == 'x' || second == 'X');
        //}

        /// <inheritdoc/>
        public override short GetInt16Value()
        {
            return (short)value;
        }

        //@Override
        //public String toString()
        //{
        //    //return Double.toString(value);
        //}

        /// <inheritdoc/>
        public override string ToString(string? format, IFormatProvider? provider)
        {
            return ToString(format, provider, value);
        }

        /**
         * Returns a string containing a concise, human-readable description of the
         * specified double value.
         * 
         * @param d
         *             the double to convert to a string.
         * @return a printable representation of {@code d}.
         */
        public static string ToString(double d)
        {
            return ToString(d, J2N.Text.StringFormatter.CurrentCulture);
            //return org.apache.harmony.luni.util.NumberConverter.convert(d);
        }

        /**
         * Returns a string containing a concise, human-readable description of the
         * specified double value.
         * 
         * @param d
         *             the double to convert to a string.
         * @return a printable representation of {@code d}.
         */
        public static string ToString(double d, IFormatProvider? provider)
        {
            if (provider is null || CultureInfo.InvariantCulture.NumberFormat.Equals(provider.GetFormat(typeof(NumberFormatInfo))))
            {
                //return J2N.Numerics.NumberConverter.Convert(f);
                return FloatingDecimal.ToJavaFormatString(d); // J2N TODO: Culture
            }

            return d.ToString(provider);
            //return org.apache.harmony.luni.util.NumberConverter.convert(d);
        }

        /**
         * Parses the specified string as a double value.
         * 
         * @param string
         *            the string representation of a double value.
         * @return a {@code Double} instance containing the double value represented
         *         by {@code string}.
         * @throws NumberFormatException
         *             if {@code string} is {@code null}, has a length of zero or
         *             can not be parsed as a double value.
         * @see #parseDouble(String)
         */
        public static Double ValueOf(string value)
        {
            return new Double(ParseDouble(value));
        }


        /**
         * Parses the specified string as a double value.
         * 
         * @param string
         *            the string representation of a double value.
         * @return a {@code Double} instance containing the double value represented
         *         by {@code string}.
         * @throws NumberFormatException
         *             if {@code string} is {@code null}, has a length of zero or
         *             can not be parsed as a double value.
         * @see #parseDouble(String)
         */
        public static Double ValueOf(string value, IFormatProvider? provider)
        {
            return new Double(ParseDouble(value, provider));
        }

        /**
         * Compares the two specified double values. There are two special cases:
         * <ul>
         * <li>{@code Double.NaN} is equal to {@code Double.NaN} and it is greater
         * than any other double value, including {@code Double.POSITIVE_INFINITY};</li>
         * <li>+0.0d is greater than -0.0d</li>
         * </ul>
         * 
         * @param double1
         *            the first value to compare.
         * @param double2
         *            the second value to compare.
         * @return a negative value if {@code double1} is less than {@code double2};
         *         0 if {@code double1} and {@code double2} are equal; a positive
         *         value if {@code double1} is greater than {@code double2}.
         */
        public static int Compare(double double1, double double2)
        {
            return JCG.Comparer<double>.Default.Compare(double1, double2);

            //// Non-zero, non-NaN checking.
            //if (double1 > double2)
            //{
            //    return 1;
            //}
            //if (double2 > double1)
            //{
            //    return -1;
            //}
            //if (double1 == double2 && 0.0d != double1)
            //{
            //    return 0;
            //}

            //// NaNs are equal to other NaNs and larger than any other double
            //if (double.IsNaN(double1))
            //{
            //    if (double.IsNaN(double2))
            //    {
            //        return 0;
            //    }
            //    return 1;
            //}
            //else if (double.IsNaN(double2))
            //{
            //    return -1;
            //}

            //// Deal with +0.0 and -0.0
            //long d1 = BitConversion.DoubleToRawInt64Bits(double1);
            //long d2 = BitConversion.DoubleToRawInt64Bits(double2);
            //// The below expression is equivalent to:
            //// (d1 == d2) ? 0 : (d1 < d2) ? -1 : 1
            //return (int)((d1 >> 63) - (d2 >> 63));
        }

        /**
         * Returns a {@code Double} instance for the specified double value.
         * 
         * @param d
         *            the double value to store in the instance.
         * @return a {@code Double} instance containing {@code d}.
         * @since 1.5
         */
        public static Double ValueOf(double d)
        {
            return new Double(d);
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
        /// <param name="d">The double to be converted.</param>
        /// <returns>A hex string representing <paramref name="d"/>.</returns>
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif 
        public static string ToHexString(double d)
        {
            return d.ToHexString();
        }

        // J2N: Support implicit conversion

        /// <inheritdoc/>
        public static implicit operator double(Double value) => value.value;
        /// <inheritdoc/>
        public static implicit operator Double(double value) => ValueOf(value);
    }
}
