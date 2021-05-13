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
    public sealed class Single : Number, IComparable<Single>
    {
        /// <summary>
        /// The value which the receiver represents.
        /// </summary>
        private readonly float value;

        /////// <summary>
        /////// Constant for the maximum <see cref="float"/> value, (2 - 2<sup>-23</sup>) * 2<sup>127</sup>.
        /////// </summary>
        ////public const float MAX_VALUE = 3.40282346638528860e+38f;

        /////**
        //// * Constant for the minimum <see cref="float"/> value, 2<sup>-149</sup>.
        //// */
        ////public const float MIN_VALUE = 1.40129846432481707e-45f;

        /////**
        //// * Constant for the Not-a-Number (NaN) value of the <see cref="float"/> type.
        //// */
        ////public const float NaN = float.NaN;

        /////**
        //// * Constant for the Positive Infinity value of the <see cref="float"/> type.
        //// */
        ////public const float PositiveInfinity = float.PositiveInfinity;

        /////**
        //// * Constant for the Negative Infinity value of the <see cref="float"/> type.
        //// */
        ////public const float NegativeInfinity = float.NegativeInfinity;

        ////    /**
        ////     * The {@link Class} object that represents the primitive type {@code
        ////     * float}.
        ////     *
        ////     * @since 1.1
        ////     */
        ////    @SuppressWarnings("unchecked")
        ////public static final Class<Float> TYPE = (Class<Float>)new float[0]
        ////        .getClass().getComponentType();

        ////// Note: This can't be set to "float.class", since *that* is
        ////// defined to be "java.lang.Float.TYPE";

        /**
         * Constant for the number of bits needed to represent a {@code float} in
         * two's complement form.
         *
         * @since 1.5
         */
        public const int SIZE = 32; // J2N TODO: Rename BitCount?

        /**
         * Constructs a new {@code Float} with the specified primitive float value.
         * 
         * @param value
         *            the primitive float value to store in the new instance.
         */
        public Single(float value)
        {
            this.value = value;
        }

        /**
         * Constructs a new {@code Float} with the specified primitive double value.
         * 
         * @param value
         *            the primitive double value to store in the new instance.
         */
        public Single(double value)
        {
            this.value = (float)value;
        }

        /**
         * Constructs a new {@code Float} from the specified string.
         * 
         * @param string
         *            the string representation of a float value.
         * @throws NumberFormatException
         *             if {@code string} can not be decoded into a float value.
         * @see #parseFloat(String)
         */
        public Single(string value)
            : this(ParseSingle(value))
        {
        }

        /**
         * Constructs a new {@code Float} from the specified string.
         * 
         * @param string
         *            the string representation of a float value.
         * @throws NumberFormatException
         *             if {@code string} can not be decoded into a float value.
         * @see #parseFloat(String)
         */
        public Single(string value, IFormatProvider? provider)
            : this(ParseSingle(value, provider))
        {
        }

        /**
         * Compares this object to the specified float object to determine their
         * relative order. There are two special cases:
         * <ul>
         * <li>{@code Float.NaN} is equal to {@code Float.NaN} and it is greater
         * than any other float value, including {@code Float.POSITIVE_INFINITY};</li>
         * <li>+0.0f is greater than -0.0f</li>
         * </ul>
         * 
         * @param object
         *            the float object to compare this object to.
         * @return a negative value if the value of this float is less than the
         *         value of {@code object}; 0 if the value of this float and the
         *         value of {@code object} are equal; a positive value if the value
         *         of this float is greater than the value of {@code object}.
         * @see java.lang.Comparable
         * @since 1.2
         */
        public int CompareTo(Single? other)
        {
            if (other is null) return 1; // Using 1 if other is null as specified here: https://stackoverflow.com/a/4852537
            return Compare(value, other.value);
        }

        /// <inheritdoc/>
        public override byte GetByteValue()
        {
            return (byte)value;
        }

        /// <inheritdoc/>
        public override double GetDoubleValue()
        {
            return value;
        }

        /**
         * Compares this instance with the specified object and indicates if they
         * are equal. In order to be equal, {@code object} must be an instance of
         * {@code Float} and have the same float value as this object.
         * 
         * @param object
         *            the object to compare this float with.
         * @return {@code true} if the specified object is equal to this
         *         {@code Float}; {@code false} otherwise.
         */
        public override bool Equals(object? obj)
        {
            if (obj is null) return false;

            return (ReferenceEquals(obj, this))
                || (obj is Single other)
                && (BitConversion.SingleToInt32Bits(this.value) == BitConversion.SingleToInt32Bits(other.value));
        }

        /**
         * Converts the specified float value to a binary representation conforming
         * to the IEEE 754 floating-point single precision bit layout. All
         * <em>Not-a-Number (NaN)</em> values are converted to a single NaN
         * representation ({@code 0x7ff8000000000000L}).
         * 
         * @param value
         *            the float value to convert.
         * @return the IEEE 754 floating-point single precision representation of
         *         {@code value}.
         * @see #floatToRawIntBits(float)
         * @see #intBitsToFloat(int)
         */
        public static int SingleToInt32Bits(float value)
        {
            return BitConversion.SingleToInt32Bits(value);
        }

        /**
         * Converts the specified float value to a binary representation conforming
         * to the IEEE 754 floating-point single precision bit layout.
         * <em>Not-a-Number (NaN)</em> values are preserved.
         * 
         * @param value
         *            the float value to convert.
         * @return the IEEE 754 floating-point single precision representation of
         *         {@code value}.
         * @see #floatToIntBits(float)
         * @see #intBitsToFloat(int)
         */
        public static int SingleToRawInt32Bits(float value)
        {
            return BitConversion.SingleToRawInt32Bits(value);
        }

        /**
         * Gets the primitive value of this float.
         * 
         * @return this object's primitive value.
         */
        public override float GetSingleValue()
        {
            return value;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return SingleToInt32Bits(value);
        }

        /**
         * Converts the specified IEEE 754 floating-point single precision bit
         * pattern to a Java float value.
         * 
         * @param bits
         *            the IEEE 754 floating-point single precision representation of
         *            a float value.
         * @return the float value converted from {@code bits}.
         * @see #floatToIntBits(float)
         * @see #floatToRawIntBits(float)
         */
        public static float Int32BitsToSingle(int value)
        {
            return BitConversion.Int32BitsToSingle(value);
        }

        /// <inheritdoc/>
        public override int GetInt32Value()
        {
            return (int)value;
        }

        /**
         * Indicates whether this object represents an infinite value.
         * 
         * @return {@code true} if the value of this float is positive or negative
         *         infinity; {@code false} otherwise.
         */
        public bool IsInfinity()
        {
            return IsInfinity(value);
        }

        /**
         * Indicates whether the specified float represents an infinite value.
         * 
         * @param f
         *            the float to check.
         * @return {@code true} if the value of {@code f} is positive or negative
         *         infinity; {@code false} otherwise.
         */
        public static bool IsInfinity(float f) // J2N: Do we need to expose this?
        {
            return float.IsInfinity(f);
            // return (f == POSITIVE_INFINITY) || (f == NEGATIVE_INFINITY);
        }

        /**
         * Indicates whether this object is a <em>Not-a-Number (NaN)</em> value.
         * 
         * @return {@code true} if this float is <em>Not-a-Number</em>;
         *         {@code false} if it is a (potentially infinite) float number.
         */
        public bool IsNaN()
        {
            return IsNaN(value);
        }

        /**
         * Indicates whether the specified float is a <em>Not-a-Number (NaN)</em>
         * value.
         * 
         * @param f
         *            the float value to check.
         * @return {@code true} if {@code f} is <em>Not-a-Number</em>;
         *         {@code false} if it is a (potentially infinite) float number.
         */
        public static bool IsNaN(float f) // J2N: Do we need to expose this?
        {
            return float.IsNaN(f);
            //return f != f;
        }

        /// <inheritdoc/>
        public override long GetInt64Value()
        {
            return (long)value;
        }

        /**
         * Parses the specified string as a float value.
         * 
         * @param string
         *            the string representation of a float value.
         * @return the primitive float value represented by {@code string}.
         * @throws NumberFormatException
         *             if {@code string} is {@code null}, has a length of zero or
         *             can not be parsed as a float value.
         * @see #valueOf(String)
         * @since 1.2
         */
        public static float ParseSingle(string s) // J2N: Rename Parse()
        {
            return ParseSingle(s, J2N.Text.StringFormatter.CurrentCulture);
            //return FloatingPointParser.ParseFloat(value, J2N.Text.StringFormatter.CurrentCulture);

            //return float.Parse(value, J2N.Text.StringFormatter.CurrentCulture); // J2N TODO: Is this right?
                                                                                //return org.apache.harmony.luni.util.FloatingPointParser
                                                                                //        .parseFloat(string);
        }

        /**
         * Parses the specified string as a float value.
         * 
         * @param string
         *            the string representation of a float value.
         * @return the primitive float value represented by {@code string}.
         * @throws NumberFormatException
         *             if {@code string} is {@code null}, has a length of zero or
         *             can not be parsed as a float value.
         * @see #valueOf(String)
         * @since 1.2
         */
        public static float ParseSingle(string s, IFormatProvider? provider) // J2N: Rename Parse()
        {
            // J2N: In .NET we don't throw on null, but return zero to match behavior of built-in parser.
            if (s is null)
                return 0.0f;

            provider ??= CultureInfo.CurrentCulture;

            s = s.Trim();
            if (s == string.Empty)
                throw new FormatException("The string was empty, which is not allowed."); // J2N TODO: Localize string

            provider ??= CultureInfo.CurrentCulture;

            if (CultureInfo.InvariantCulture.NumberFormat.Equals(provider.GetFormat(typeof(NumberFormatInfo))) ||
                ParseAsHex(s))
            {
                return FloatingDecimal.ParseFloat(s); // J2N TODO: Culture
            }

            //return FloatingPointParser.ParseDouble(value, provider);
            //return org.apache.harmony.luni.util.FloatingPointParser
            //        .parseDouble(string);

            float result = float.Parse(s, provider); // J2N TODO: For now, fallback to .NET. We should respect the NumberFormatInfo settings in the Java parser/formatter, though.

            // .NET doesn't handle negative zero, so we need to do that here
            if (result == 0f && FloatingDecimal.IsNegative(s, provider))
                return -0.0f;

            return result;

            //return FloatingPointParser.ParseFloat(value, provider);

            //return float.Parse(value, provider); // J2N TODO: Is this right?
            //return org.apache.harmony.luni.util.FloatingPointParser
            //        .parseFloat(string);
        }

        /*
         * Answers true if the string should be parsed as a hex encoding.
         * Assumes the string is trimmed.
         */
        private static bool ParseAsHex(string s)
        {
            int length = s.Length;
            if (length < 2)
            {
                return false;
            }
            char first = s[0];
            char second = s[1];
            if (first == '+' || first == '-')
            {
                // Move along
                if (length < 3)
                {
                    return false;
                }
                first = second;
                second = s[2];
            }
            return (first == '0') && (second == 'x' || second == 'X');
        }

        /// <inheritdoc/>
        public override short GetInt16Value()
        {
            return (short)value;
        }

        //public override string ToString()
        //{
        //    return Single.ToString(value);
        //}

        /// <inheritdoc/>
        public override string ToString(string? format, IFormatProvider? provider) // .NETified
        {
            return ToString(format, provider, value);
        }

        /**
         * Returns a string containing a concise, human-readable description of the
         * specified float value.
         * 
         * @param f
         *             the float to convert to a string.
         * @return a printable representation of {@code f}.
         */
        public static string ToString(float f)
        {
            return ToString(f, J2N.Text.StringFormatter.CurrentCulture);
            //return org.apache.harmony.luni.util.NumberConverter.convert(f);
        }

        /**
         * Returns a string containing a concise, human-readable description of the
         * specified float value.
         * 
         * @param f
         *             the float to convert to a string.
         * @return a printable representation of {@code f}.
         */
        public static string ToString(float f, IFormatProvider? provider)
        {
            //// Fast path: For standard .NET formatting using cultures, call IFormattable.ToString() to eliminate
            //// boxing associated with string.Format().
            //if (provider is null || provider is CultureInfo || provider is NumberFormatInfo)
            //{
            //    return f.ToString(provider);
            //}
            //// Built-in .NET numeric types don't support custom format providers, so we resort
            //// to using string.Format with some hacky format conversion in order to support them.
            //return string.Format(provider, "{0}", f);

            provider ??= CultureInfo.CurrentCulture;

            if (CultureInfo.InvariantCulture.NumberFormat.Equals(provider.GetFormat(typeof(NumberFormatInfo))))
            {
                //return FloatingDecimal.ToJavaFormatString(f); // J2N TODO: Culture
                return RyuConversion.FloatToString(f);
            }

            return f.ToString(provider);
            //return org.apache.harmony.luni.util.NumberConverter.convert(f);
        }

        /**
         * Parses the specified string as a float value.
         * 
         * @param string
         *            the string representation of a float value.
         * @return a {@code Float} instance containing the float value represented
         *         by {@code string}.
         * @throws NumberFormatException
         *             if {@code string} is {@code null}, has a length of zero or
         *             can not be parsed as a float value.
         * @see #parseFloat(String)
         */
        public static Single ValueOf(string value)
        {
            return ValueOf(ParseSingle(value));
        }

        /**
         * Parses the specified string as a float value.
         * 
         * @param string
         *            the string representation of a float value.
         * @return a {@code Float} instance containing the float value represented
         *         by {@code string}.
         * @throws NumberFormatException
         *             if {@code string} is {@code null}, has a length of zero or
         *             can not be parsed as a float value.
         * @see #parseFloat(String)
         */
        public static Single ValueOf(string value, IFormatProvider? provider)
        {
            return ValueOf(ParseSingle(value, provider));
        }

        /**
         * Compares the two specified float values. There are two special cases:
         * <ul>
         * <li>{@code Float.NaN} is equal to {@code Float.NaN} and it is greater
         * than any other float value, including {@code Float.POSITIVE_INFINITY};</li>
         * <li>+0.0f is greater than -0.0f</li>
         * </ul>
         * 
         * @param float1
         *            the first value to compare.
         * @param float2
         *            the second value to compare.
         * @return a negative value if {@code float1} is less than {@code float2};
         *         0 if {@code float1} and {@code float2} are equal; a positive
         *         value if {@code float1} is greater than {@code float2}.
         * @since 1.4
         */
        public static int Compare(float float1, float float2)
        {
            return JCG.Comparer<float>.Default.Compare(float1, float2);

            //// Non-zero, non-NaN checking.
            //if (float1 > float2)
            //{
            //    return 1;
            //}
            //if (float2 > float1)
            //{
            //    return -1;
            //}
            //if (float1 == float2 && 0.0f != float1)
            //{
            //    return 0;
            //}

            //// NaNs are equal to other NaNs and larger than any other float
            //if (IsNaN(float1))
            //{
            //    if (IsNaN(float2))
            //    {
            //        return 0;
            //    }
            //    return 1;
            //}
            //else if (IsNaN(float2))
            //{
            //    return -1;
            //}

            //// Deal with +0.0 and -0.0
            //int f1 = SingleToRawInt32Bits(float1);
            //int f2 = SingleToRawInt32Bits(float2);
            //// The below expression is equivalent to:
            //// (f1 == f2) ? 0 : (f1 < f2) ? -1 : 1
            //// because f1 and f2 are either 0 or Integer.MIN_VALUE
            //return (f1 >> 31) - (f2 >> 31);
        }

        /**
         * Returns a {@code Float} instance for the specified float value.
         * 
         * @param f
         *            the float value to store in the instance.
         * @return a {@code Float} instance containing {@code f}.
         * @since 1.5
         */
        public static Single ValueOf(float f)
        {
            return new Single(f);
        }

        /// <summary>
        /// Returns a hexadecimal string representation of the <see cref="float"/> argument. All characters mentioned below are ASCII characters.
        /// <list type="bullet">
        ///     <item><description>If the argument is <see cref="float.NaN"/>, the result is the string "NaN". </description></item>
        ///     <item><description>Otherwise, the result is a string that represents the sign and magnitude (absolute value) of the argument.
        ///         If the sign is negative, the first character of the result is '-' ('\u002D'); if the sign is positive, no sign character
        ///         appears in the result. As for the magnitude <i>m</i>: </description>
        ///         <list type="bullet">
        ///             <item><description>If <i>m</i> is infinity, it is represented by the string "Infinity"; thus, positive infinity produces the
        ///                 result "Infinity" and negative infinity produces the result "-Infinity". </description></item>
        ///             <item><description>If <i>m</i> is zero, it is represented by the string "0x0.0p0"; thus, negative zero produces the result
        ///                 "-0x0.0p0" and positive zero produces the result "0x0.0p0". </description></item>
        ///             <item><description>If <i>m</i> is a <see cref="float"/> value with a normalized representation, substrings are used to represent the
        ///                 significand and exponent fields. The significand is represented by the characters "0x1." followed by a lowercase
        ///                 hexadecimal representation of the rest of the significand as a fraction. Trailing zeros in the hexadecimal representation
        ///                 are removed unless all the digits are zero, in which case a single zero is used. Next, the exponent is represented by "p"
        ///                 followed by a decimal string of the unbiased exponent as if produced by a call to <see cref="int.ToString()"/> with invariant culture on the exponent value. </description></item>
        ///             <item><description>If <i>m</i> is a <see cref="float"/> value with a subnormal representation, the significand is represented by the characters "0x0."
        ///                 followed by a hexadecimal representation of the rest of the significand as a fraction. Trailing zeros in the hexadecimal
        ///                 representation are removed. Next, the exponent is represented by "p-126". Note that there must be at least one nonzero
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
        /// <param name="f">The <see cref="float"/> to be converted.</param>
        /// <returns>A hex string representing <paramref name="f"/>.</returns>
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif 
        public static string ToHexString(float f)
        {
            return f.ToHexString();
        }



        // J2N: Support implicit conversion

        /// <inheritdoc/>
        public static implicit operator float(Single value) => value.value;
        /// <inheritdoc/>
        public static implicit operator Single(float value) => ValueOf(value);
    }
}
