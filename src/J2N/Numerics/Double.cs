using J2N.Globalization;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using JCG = J2N.Collections.Generic;

namespace J2N.Numerics
{
    using SR = J2N.Resources.Strings;

    /// <inheritdoc/>
    public sealed class Double : Number, IComparable<Double>, IComparable
    {
        /// <summary>
        /// The value which the receiver represents.
        /// </summary>
        private readonly double value;

        /// <summary>
        /// Maximum exponent a finite <see cref="double"/> variable may have.
        /// </summary>
        public const int MaxExponent = 1023;

        /// <summary>
        /// Minimum exponent a normalized <see cref="double"/> variable may have.
        /// </summary>
        public const int MinExponent = -1022;

        /// <summary>
        /// The smallest positive normal value of type <see cref="double"/>, 2<sup>-1022</sup>.
        /// It is equal to the value <c>BitConversion.Int64BitsToDouble(0x0010000000000000L)</c>.
        /// </summary>
        public const double MinNormal = 2.2250738585072014E-308;

        /// <summary>
        /// The number of bits needed to represent a <see cref="double"/> in
        /// two's complement form.
        /// </summary>
        public const int Size = 64;

        /**
         * Constructs a new {@code Double} with the specified primitive double
         * value.
         * 
         * @param value
         *            the primitive double value to store in the new instance.
         */
        internal Double(double value) // J2N: This has been marked deprecated in JDK 16, so we are marking it internal
        {
            this.value = value;
        }

        // J2N: Removed other overloads because all of the constructors are deprecated in JDK 16

        /// <summary>
        /// Compares this instance to a specified <see cref="Double"/> and returns an indication of their relative values.
        /// </summary>
        /// <param name="value">A <see cref="Double"/> to compare, or <c>null</c>.</param>
        /// <returns>
        /// A signed integer that indicates the relative order of this instance and <paramref name="value"/>.
        /// <list type="table">
        ///     <listheader>
        ///         <term>Return Value</term>
        ///         <term>Description </term>
        ///     </listheader>
        ///     <item>
        ///         <term>Less than zero</term>
        ///         <term>This instance is less than <paramref name="value"/>.</term>
        ///     </item>
        ///     <item>
        ///         <term>Zero</term>
        ///         <term>This instance is equal to <paramref name="value"/>.</term>
        ///     </item>
        ///     <item>
        ///         <term>Greater than zero</term>
        ///         <term>This instance is greater than <paramref name="value"/>, or <paramref name="value"/> is <c>null</c>.</term>
        ///     </item>
        /// </list>
        /// </returns>
        /// <remarks>
        /// There are two special cases:
        /// <list type="table">
        ///     <item><description><see cref="double.NaN"/> is equal to <see cref="double.NaN"/> and it is greater
        ///     than any other double value, including <see cref="double.PositiveInfinity"/></description></item>
        ///     <item><description>+0.0d (positive zero) is greater than -0.0d (negative zero).</description></item>
        /// </list>
        /// <para/>
        /// This method implements the <see cref="IComparable{T}"/> interface and performs slightly better than the <see cref="CompareTo(object?)"/>
        /// method because it does not have to convert the <paramref name="value"/> parameter to an object.
        /// </remarks>
        public int CompareTo(Double? value)
        {
            if (value is null) return 1; // Using 1 if other is null as specified here: https://stackoverflow.com/a/4852537
            return Compare(this.value, value.value);
        }

        /// <summary>
        /// Compares this instance to a specified object and returns an indication of their relative values.
        /// </summary>
        /// <param name="value">An object to compare, or <c>null</c>.</param>
        /// <returns>
        /// A signed integer that indicates the relative order of this instance and <paramref name="value"/>.
        /// <list type="table">
        ///     <listheader>
        ///         <term>Return Value</term>
        ///         <term>Description </term>
        ///     </listheader>
        ///     <item>
        ///         <term>Less than zero</term>
        ///         <term>This instance is less than <paramref name="value"/>.</term>
        ///     </item>
        ///     <item>
        ///         <term>Zero</term>
        ///         <term>This instance is equal to <paramref name="value"/>.</term>
        ///     </item>
        ///     <item>
        ///         <term>Greater than zero</term>
        ///         <term>This instance is greater than <paramref name="value"/>, or <paramref name="value"/> is <c>null</c>.</term>
        ///     </item>
        /// </list>
        /// </returns>
        /// <exception cref="ArgumentException"><paramref name="value"/> is not a <see cref="Double"/>.</exception>
        /// <remarks>
        /// <paramref name="value"/> must be <c>null</c> or an instance of <see cref="Double"/>; otherwise, an exception is thrown.
        /// <para/>
        /// Any instance of <see cref="Double"/>, regardless of its value, is considered greater than <c>null</c>.
        /// <para/>
        /// There are two special cases:
        /// <list type="table">
        ///     <item><description><see cref="double.NaN"/> is equal to <see cref="double.NaN"/> and it is greater
        ///     than any other double value, including <see cref="double.PositiveInfinity"/></description></item>
        ///     <item><description>+0.0d (positive zero) is greater than -0.0d (negative zero).</description></item>
        /// </list>
        /// <para/>
        /// This method is implemented to support the <see cref="IComparable"/> interface.
        /// </remarks>
        public int CompareTo(object? value)
        {
            if (value is null) return 1; // Using 1 if other is null as specified here: https://stackoverflow.com/a/4852537
            if (!(value is Double other))
                throw new ArgumentException(SR.Arg_MustBeDouble);
            return Compare(this.value, other.value);
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

        #region Parse_CharSequence_IFormatProvider

        /// <summary>
        /// Converts the string representation of a number in a specified style and culture-specific format to
        /// its double-precision floating-point number equivalent.
        /// <para/>
        /// Usage Note: To approximately match the behavior of the JDK for decimal numbers, use <see cref="NumberStyle.Float"/> combined with
        /// <see cref="NumberStyle.AllowTypeSpecifier"/> for <c>style</c> and <see cref="NumberFormatInfo.InvariantInfo"/> for <paramref name="provider"/>.
        /// To approximately match the behavior of the JDK for hexadecimal numbers, use <see cref="NumberStyle.HexFloat"/> combined with
        /// <see cref="NumberStyle.AllowTypeSpecifier"/> for <c>style</c> and <see cref="NumberFormatInfo.InvariantInfo"/> for <paramref name="provider"/>.
        /// These options will parse any number that is supported by Java, but the hex specifier, exponent, and type suffix are all optional.
        /// </summary>
        /// <param name="s">A string that contains a number to convert.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information about <paramref name="s"/>.</param>
        /// <returns>A double-precision floating-point number equivalent to the numeric value or symbol specified in <paramref name="s"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="s"/> is <c>null</c>.</exception>
        /// <exception cref="FormatException"><paramref name="s"/> does not represent a number in a valid format.</exception>
        /// <remarks>
        /// In .NET Core 3.0 and later, values that are too large to represent are rounded to <see cref="double.PositiveInfinity"/> or
        /// <see cref="double.NegativeInfinity"/> as required by the IEEE 754 specification. In prior versions, including .NET Framework, parsing a
        /// value that was too large to represent resulted in failure. This implementation uses the new parser, but patches it back to .NET Framework 4.0.
        /// <para/>
        /// This overload is typically used to convert text that can be formatted in a variety of ways to a <see cref="double"/> value. For example,
        /// it can be used to convert the text entered by a user into an HTML text box to a numeric value.
        /// <para/>
        /// The <paramref name="s"/> parameter is interpreted using a combination of the <see cref="NumberStyle.Float"/> and <see cref="NumberStyle.AllowThousands"/> flags.
        /// The <paramref name="s"/> parameter can contain <see cref="NumberFormatInfo.PositiveInfinitySymbol"/>, <see cref="NumberFormatInfo.NegativeInfinitySymbol"/>, or
        /// <see cref="NumberFormatInfo.NaNSymbol"/> for the culture specified by <paramref name="provider"/>, or it can contain a string of the form:
        /// <para/>
        /// [ws][sign]integral-digits[.[fractional-digits]][E[sign]exponential-digits][ws]
        /// <para/>
        /// Elements framed in square brackets ([ and ]) are optional. Elements that contain the term "digits" consist of a series of numeric characters
        /// ranging from 0 to 9. The following table describes each element.
        /// <para/>
        /// <list type="table">
        ///     <listheader>
        ///         <term>Element</term>
        ///         <term>Description</term>
        ///     </listheader>
        ///     <item>
        ///         <term><i>ws</i></term>
        ///         <term>A series of white-space characters.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>sign</i></term>
        ///         <term>A negative sign symbol (-) or a positive sign symbol (+).</term>
        ///     </item>
        ///     <item>
        ///         <term><i>integral-digits</i></term>
        ///         <term>A series of digits ranging from 0 to 9 that specify the integral part of the number. Runs of <i>integral-digits</i> can be partitioned
        ///         by a group-separator symbol. For example, in some cultures a comma (,) separates groups of thousands. The <i>integral-digits</i> element can be absent
        ///         if the string contains the <i>fractional-digits</i> element.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>.</i></term>
        ///         <term>A culture-specific decimal point symbol.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>fractional-digits</i>
        ///         </term>
        ///         <term>A series of digits ranging from 0 to 9 that specify the fractional part of the number.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>e</i></term>
        ///         <term>The 'e' or 'E' character, which indicates that the value is represented in exponential notation.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>exponential-digits</i></term>
        ///         <term>A sequence of decimal digits from 0 through 9 that specify an exponent.</term>
        ///     </item>
        /// </list>
        /// <para/>
        /// For more information about numeric formats, see the <a href="https://docs.microsoft.com/en-us/dotnet/standard/base-types/formatting-types">
        /// Formatting Types</a> topic.
        /// <para/>
        /// The <paramref name="provider"/> parameter is an <see cref="IFormatProvider"/> implementation, such as a <see cref="CultureInfo"/> object,
        /// a <see cref="NumberFormatInfo"/> object or a <see cref="J2N.Text.StringFormatter"/> object, whose <see cref="IFormatProvider.GetFormat(Type?)"/>
        /// method returns a <see cref="NumberFormatInfo"/> object. The <see cref="NumberFormatInfo"/> object provides culture-specific information about
        /// the format of <paramref name="s"/>. When the <see cref="Parse(string, IFormatProvider)"/> method is invoked, it calls the provider parameter's
        /// <see cref="IFormatProvider.GetFormat(Type?)"/> method and passes it a <see cref="Type"/> object that represents the <see cref="NumberFormatInfo"/> type.
        /// The <see cref="IFormatProvider.GetFormat(Type?)"/> method then returns the <see cref="NumberFormatInfo"/> object that provides information about the
        /// format of the <paramref name="s"/> parameter. There are three ways to use the <paramref name="provider"/> parameter to supply custom formatting
        /// information to the parse operation:
        /// <list type="bullet">
        ///     <item><description>You can pass a <see cref="CultureInfo"/> object that represents the culture that supplies formatting information. Its
        ///     <see cref="IFormatProvider.GetFormat(Type?)"/> method returns the <see cref="NumberFormatInfo"/> object that provides numeric formatting
        ///     information for that culture.</description></item>
        ///     <item><description>You can pass the actual <see cref="NumberFormatInfo"/> object that provides numeric formatting information. (Its
        ///     implementation of <see cref="IFormatProvider.GetFormat(Type?)"/> just returns itself.)</description></item>
        ///     <item><description>You can pass a custom object that implements <see cref="IFormatProvider"/>. Its <see cref="IFormatProvider.GetFormat(Type?)"/>
        ///     method instantiates and returns the <see cref="NumberFormatInfo"/> object that provides formatting information.</description></item>
        /// </list>
        /// If <paramref name="provider"/> is <c>null</c> or a <see cref="NumberFormatInfo"/> object cannot be obtained, the <see cref="NumberFormatInfo"/>
        /// object for the current culture is used.
        /// <para/>
        /// If <paramref name="s"/> is out of range of the <see cref="double"/> data type, the method returns <see cref="double.NegativeInfinity"/> if
        /// <paramref name="s"/> is less than <see cref="double.MinValue"/> and <see cref="double.PositiveInfinity"/> if <paramref name="s"/> is greater
        /// than <see cref="double.MaxValue"/>.
        /// <para/>
        /// If a separator is encountered in the <paramref name="s"/> parameter during a parse operation, and the applicable currency or number
        /// decimal and group separators are the same, the parse operation assumes that the separator is a decimal separator rather than a group
        /// separator. For more information about separators, see <see cref="NumberFormatInfo.CurrencyDecimalSeparator"/>,
        /// <see cref="NumberFormatInfo.NumberDecimalSeparator"/>, <see cref="NumberFormatInfo.CurrencyGroupSeparator"/>, and
        /// <see cref="NumberFormatInfo.NumberGroupSeparator"/>.
        /// <para/>
        /// Some examples of <paramref name="s"/> are "100", "-123,456,789", "123.45e+6", "+500", "5e2", "3.1416", "600.", "-.123", and "-Infinity".
        /// </remarks>
        /// <seealso cref="ValueOf(string, IFormatProvider?)"/>
        /// <seealso cref="TryParse(string, NumberStyle, IFormatProvider?, out double)"/>
        public static double Parse(string s, IFormatProvider? provider) // J2N: Renamed from ParseDouble()
        {
            return Parse(s, NumberStyle.Float | NumberStyle.AllowThousands, provider);
        }

        #endregion Parse_CharSequence_IFormatProvider

        #region TryParse_CharSequence_Double

        /// <summary>
        /// Converts the string representation of a number to
        /// its double-precision floating-point number equivalent. A return value indicates whether the
        /// conversion succeeded or failed.
        /// <para/>
        /// Usage Note: To approximately match the behavior of the JDK for decimal numbers, use <see cref="NumberStyle.Float"/> combined with
        /// <see cref="NumberStyle.AllowTypeSpecifier"/> for <c>style</c> and <see cref="NumberFormatInfo.InvariantInfo"/> for <c>provider</c>.
        /// To approximately match the behavior of the JDK for hexadecimal numbers, use <see cref="NumberStyle.HexFloat"/> combined with
        /// <see cref="NumberStyle.AllowTypeSpecifier"/> for <c>style</c> and <see cref="NumberFormatInfo.InvariantInfo"/> for <c>provider</c>.
        /// These options will parse any number that is supported by Java, but the hex specifier, exponent, and type suffix are all optional.
        /// </summary>
        /// <param name="s">A string representing a number to convert.</param>
        /// <param name="result">When this method returns, contains the double-precision floating-point number equivalent to the
        /// numeric value or symbol contained in <paramref name="s"/>, if the conversion succeeded, or zero if the conversion failed.
        /// The conversion fails if the <paramref name="s"/> parameter is <c>null</c> or <see cref="string.Empty"/>.
        /// This parameter is passed uninitialized; any value originally supplied in result will be overwritten.</param>
        /// <returns><c>true</c> if <paramref name="s"/> was converted successfully; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// In .NET Core 3.0 and later, values that are too large to represent are rounded to <see cref="double.PositiveInfinity"/> or
        /// <see cref="double.NegativeInfinity"/> as required by the IEEE 754 specification. In prior versions, including .NET Framework, parsing a
        /// value that was too large to represent resulted in failure. This implementation uses the new parser, but patches it back to .NET Framework 4.0
        /// and upgrades it to support Java's hexadecimal floating point format.
        /// <para/>
        /// This overload differs from the <see cref="Parse(string, NumberStyle, IFormatProvider?)"/> method by returning a Boolean value that indicates
        /// whether the parse operation succeeded instead of returning the parsed numeric value. It eliminates the need to use exception handling to test
        /// for a <see cref="FormatException"/> in the event that <paramref name="s"/> is invalid and cannot be successfully parsed.
        /// <para/>
        /// The <paramref name="s"/> parameter can contain <see cref="NumberFormatInfo.PositiveInfinitySymbol"/>, <see cref="NumberFormatInfo.NegativeInfinitySymbol"/>, or
        /// <see cref="NumberFormatInfo.NaNSymbol"/> (the string comparison will use <see cref="StringComparison.OrdinalIgnoreCase"/> comparison rules), or a string of the form:
        /// <para/>
        /// [ws][sign][integral-digits,]integral-digits[.[fractional-digits]][e[sign]exponential-digits][ws]
        /// <para/>
        /// Elements framed in square brackets ([ and ]) are optional. Elements that contain the term "digits" consist of a series of numeric characters
        /// ranging from 0 to 9. The following table describes each element.
        /// <para/>
        /// <list type="table">
        ///     <listheader>
        ///         <term>Element</term>
        ///         <term>Description</term>
        ///     </listheader>
        ///     <item>
        ///         <term><i>ws</i></term>
        ///         <term>A series of white-space characters.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>sign</i></term>
        ///         <term>A negative sign symbol (-) or a positive sign symbol (+).</term>
        ///     </item>
        ///     <item>
        ///         <term><i>integral-digits</i></term>
        ///         <term>A series of digits ranging from 0 to 9 that specify the integral part of the number. The <i>integral-digits</i> element can be absent
        ///         if the string contains the <i>fractional-digits</i> element.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>,</i></term>
        ///         <term>A culture-specific group separator symbol.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>.</i></term>
        ///         <term>A culture-specific decimal point symbol.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>fractional-digits</i>
        ///         </term>
        ///         <term>A series of digits ranging from 0 to 9 that specify the fractional part of the number.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>e</i></term>
        ///         <term>The 'e' or 'E' character, which indicates that the value is represented in exponential (scientific) notation.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>exponential-digits</i></term>
        ///         <term>A sequence of decimal digits from 0 through 9 that specify an exponent.</term>
        ///     </item>
        /// </list>
        /// <para/>
        /// For more information about numeric formats, see the <a href="https://docs.microsoft.com/en-us/dotnet/standard/base-types/formatting-types">
        /// Formatting Types</a> topic.
        /// <para/>
        /// The <paramref name="s"/> parameter is interpreted using a combination of the <see cref="NumberStyle.Float"/> and <see cref="NumberStyle.AllowThousands"/>
        /// flags. This means that white space and thousands separators are allowed but currency symbols are not. To explicitly define the
        /// elements (such as currency symbols, thousands separators, and white space) that can be present in <paramref name="s"/>, use the
        /// <see cref="TryParse(string?, NumberStyle, IFormatProvider?, out double)"/> method overload.
        /// <para/>
        /// The <paramref name="s"/> parameter is parsed using the formatting information in a <see cref="NumberFormatInfo"/> object that is initialized for the
        /// current system culture. For more information, see <see cref="NumberFormatInfo.CurrentInfo"/>. To parse a string using the formatting information of
        /// some other specified culture, use the <see cref="TryParse(string?, NumberStyle, IFormatProvider?, out double)"/> method overload.
        /// <para/>
        /// If you pass the <see cref="TryParse(string?, out double)"/> method a string that is created by calling the <see cref="ToString(double, IFormatProvider?)"/>
        /// method and pass <see cref="J2N.Text.StringFormatter.InvariantCulture"/>, the original <see cref="double"/> value is returned. However, because of a loss of precision,
        /// the values using other implementations of <see cref="IFormatProvider"/> may not be equal.
        /// <para/>
        /// If <paramref name="s"/> is out of range of the <see cref="double"/> data type, the method returns <see cref="double.NegativeInfinity"/> if
        /// <paramref name="s"/> is less than <see cref="double.MinValue"/> and <see cref="double.PositiveInfinity"/> if <paramref name="s"/> is greater
        /// than <see cref="double.MaxValue"/>.
        /// <para/>
        /// If a separator is encountered in the <paramref name="s"/> parameter during a parse operation, and the applicable currency or number
        /// decimal and group separators are the same, the parse operation assumes that the separator is a decimal separator rather than a group
        /// separator. For more information about separators, see <see cref="NumberFormatInfo.CurrencyDecimalSeparator"/>,
        /// <see cref="NumberFormatInfo.NumberDecimalSeparator"/>, <see cref="NumberFormatInfo.CurrencyGroupSeparator"/>, and
        /// <see cref="NumberFormatInfo.NumberGroupSeparator"/>.
        /// </remarks>
        /// <seealso cref="Parse(string, IFormatProvider?)"/>
        /// <seealso cref="ValueOf(string, IFormatProvider?)"/>
        public static bool TryParse([NotNullWhen(true)] string? s, out double result)
        {
            if (s == null)
            {
                result = 0;
                return false;
            }

            return DotNetNumber.TryParseDouble(s, NumberStyle.Float | NumberStyle.AllowThousands, NumberFormatInfo.CurrentInfo, out result);
        }

#if FEATURE_READONLYSPAN

        /// <summary>
        /// Converts the string representation of a number in a character span to its double-precision floating-point number equivalent.
        /// A return value indicates whether the conversion succeeded or failed.
        /// <para/>
        /// Usage Note: To approximately match the behavior of the JDK for decimal numbers, use <see cref="NumberStyle.Float"/> combined with
        /// <see cref="NumberStyle.AllowTypeSpecifier"/> for <c>style</c> and <see cref="NumberFormatInfo.InvariantInfo"/> for <c>provider</c>.
        /// To approximately match the behavior of the JDK for hexadecimal numbers, use <see cref="NumberStyle.HexFloat"/> combined with
        /// <see cref="NumberStyle.AllowTypeSpecifier"/> for <c>style</c> and <see cref="NumberFormatInfo.InvariantInfo"/> for <c>provider</c>.
        /// These options will parse any number that is supported by Java, but the hex specifier, exponent, and type suffix are all optional.
        /// </summary>
        /// <param name="s">A string representing a number to convert.</param>
        /// <param name="result">When this method returns, contains the double-precision floating-point number equivalent to the
        /// numeric value or symbol contained in <paramref name="s"/>, if the conversion succeeded, or zero if the conversion failed.
        /// The conversion fails if the <paramref name="s"/> parameter is <c>null</c> or <see cref="string.Empty"/>.
        /// This parameter is passed uninitialized; any value originally supplied in result will be overwritten.</param>
        /// <returns><c>true</c> if <paramref name="s"/> was converted successfully; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// In .NET Core 3.0 and later, values that are too large to represent are rounded to <see cref="double.PositiveInfinity"/> or
        /// <see cref="double.NegativeInfinity"/> as required by the IEEE 754 specification. In prior versions, including .NET Framework, parsing a
        /// value that was too large to represent resulted in failure. This implementation uses the new parser, but patches it back to .NET Framework 4.0
        /// and upgrades it to support Java's hexadecimal floating point format.
        /// <para/>
        /// This overload differs from the <see cref="Parse(string, NumberStyle, IFormatProvider?)"/> method by returning a Boolean value that indicates
        /// whether the parse operation succeeded instead of returning the parsed numeric value. It eliminates the need to use exception handling to test
        /// for a <see cref="FormatException"/> in the event that <paramref name="s"/> is invalid and cannot be successfully parsed.
        /// <para/>
        /// The <paramref name="s"/> parameter can contain <see cref="NumberFormatInfo.PositiveInfinitySymbol"/>, <see cref="NumberFormatInfo.NegativeInfinitySymbol"/>, or
        /// <see cref="NumberFormatInfo.NaNSymbol"/> (the string comparison will use <see cref="StringComparison.OrdinalIgnoreCase"/> comparison rules), or a string of the form:
        /// <para/>
        /// [ws][sign][integral-digits,]integral-digits[.[fractional-digits]][e[sign]exponential-digits][ws]
        /// <para/>
        /// Elements framed in square brackets ([ and ]) are optional. Elements that contain the term "digits" consist of a series of numeric characters
        /// ranging from 0 to 9. The following table describes each element.
        /// <para/>
        /// <list type="table">
        ///     <listheader>
        ///         <term>Element</term>
        ///         <term>Description</term>
        ///     </listheader>
        ///     <item>
        ///         <term><i>ws</i></term>
        ///         <term>A series of white-space characters.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>sign</i></term>
        ///         <term>A negative sign symbol (-) or a positive sign symbol (+).</term>
        ///     </item>
        ///     <item>
        ///         <term><i>integral-digits</i></term>
        ///         <term>A series of digits ranging from 0 to 9 that specify the integral part of the number. The <i>integral-digits</i> element can be absent
        ///         if the string contains the <i>fractional-digits</i> element.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>,</i></term>
        ///         <term>A culture-specific group separator symbol.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>.</i></term>
        ///         <term>A culture-specific decimal point symbol.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>fractional-digits</i>
        ///         </term>
        ///         <term>A series of digits ranging from 0 to 9 that specify the fractional part of the number.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>e</i></term>
        ///         <term>The 'e' or 'E' character, which indicates that the value is represented in exponential (scientific) notation.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>exponential-digits</i></term>
        ///         <term>A sequence of decimal digits from 0 through 9 that specify an exponent.</term>
        ///     </item>
        /// </list>
        /// <para/>
        /// For more information about numeric formats, see the <a href="https://docs.microsoft.com/en-us/dotnet/standard/base-types/formatting-types">
        /// Formatting Types</a> topic.
        /// <para/>
        /// The <paramref name="s"/> parameter is interpreted using a combination of the <see cref="NumberStyle.Float"/> and <see cref="NumberStyle.AllowThousands"/>
        /// flags. This means that white space and thousands separators are allowed but currency symbols are not. To explicitly define the
        /// elements (such as currency symbols, thousands separators, and white space) that can be present in <paramref name="s"/>, use the
        /// <see cref="TryParse(string?, NumberStyle, IFormatProvider?, out double)"/> method overload.
        /// <para/>
        /// The <paramref name="s"/> parameter is parsed using the formatting information in a <see cref="NumberFormatInfo"/> object that is initialized for the
        /// current system culture. For more information, see <see cref="NumberFormatInfo.CurrentInfo"/>. To parse a string using the formatting information of
        /// some other specified culture, use the <see cref="TryParse(string?, NumberStyle, IFormatProvider?, out double)"/> method overload.
        /// <para/>
        /// If you pass the <see cref="TryParse(ReadOnlySpan{char}, out double)"/> method a string that is created by calling the <see cref="ToString(double, IFormatProvider?)"/>
        /// method and pass <see cref="J2N.Text.StringFormatter.InvariantCulture"/>, the original <see cref="double"/> value is returned. However, because of a loss of precision,
        /// the values using other implementations of <see cref="IFormatProvider"/> may not be equal.
        /// <para/>
        /// If <paramref name="s"/> is out of range of the <see cref="double"/> data type, the method returns <see cref="double.NegativeInfinity"/> if
        /// <paramref name="s"/> is less than <see cref="double.MinValue"/> and <see cref="double.PositiveInfinity"/> if <paramref name="s"/> is greater
        /// than <see cref="double.MaxValue"/>.
        /// <para/>
        /// If a separator is encountered in the <paramref name="s"/> parameter during a parse operation, and the applicable currency or number
        /// decimal and group separators are the same, the parse operation assumes that the separator is a decimal separator rather than a group
        /// separator. For more information about separators, see <see cref="NumberFormatInfo.CurrencyDecimalSeparator"/>,
        /// <see cref="NumberFormatInfo.NumberDecimalSeparator"/>, <see cref="NumberFormatInfo.CurrencyGroupSeparator"/>, and
        /// <see cref="NumberFormatInfo.NumberGroupSeparator"/>.
        /// </remarks>
        /// <seealso cref="Parse(ReadOnlySpan{char}, NumberStyle, IFormatProvider?)"/>
        /// <seealso cref="ValueOf(string?, IFormatProvider?)"/>
        public static bool TryParse(ReadOnlySpan<char> s, out double result)
        {
            return DotNetNumber.TryParseDouble(s, NumberStyle.Float | NumberStyle.AllowThousands, NumberFormatInfo.CurrentInfo, out result);
        }

#endif

        #endregion TryParse_CharSequence_Double

        #region Parse_CharSequence_NumberStyle_IFormatProvider

        /// <summary>
        /// Converts the string representation of a number in a specified style and culture-specific format to
        /// its double-precision floating-point number equivalent.
        /// <para/>
        /// Usage Note: To approximately match the behavior of the JDK for decimal numbers, use <see cref="NumberStyle.Float"/> combined with
        /// <see cref="NumberStyle.AllowTypeSpecifier"/> for <paramref name="style"/> and <see cref="NumberFormatInfo.InvariantInfo"/> for <paramref name="provider"/>.
        /// To approximately match the behavior of the JDK for hexadecimal numbers, use <see cref="NumberStyle.HexFloat"/> combined with
        /// <see cref="NumberStyle.AllowTypeSpecifier"/> for <paramref name="style"/> and <see cref="NumberFormatInfo.InvariantInfo"/> for <paramref name="provider"/>.
        /// These options will parse any number that is supported by Java, but the hex specifier, exponent, and type suffix are all optional.
        /// </summary>
        /// <param name="s">A string that contains a number to convert.</param>
        /// <param name="style">A bitwise combination of enumeration values that indicates the style elements
        /// that can be present in <paramref name="s"/>. Typical values to specify is <see cref="NumberStyle.Float"/>
        /// combined with <see cref="NumberStyle.AllowThousands"/> or, in the case of hexadecimal numbers,
        /// <see cref="NumberStyle.HexFloat"/>.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information about <paramref name="s"/>.</param>
        /// <returns>A double-precision floating-point number equivalent to the numeric value or symbol specified in <paramref name="s"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="s"/> is <c>null</c>.</exception>
        /// <exception cref="FormatException"><paramref name="s"/> does not represent a numeric value.</exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="style"/> is not a <see cref="NumberStyle"/> value.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="style"/> is not a combination of <see cref="NumberStyle.AllowHexSpecifier"/>, <see cref="NumberStyle.HexFloat"/>
        /// and <see cref="NumberStyle.AllowTypeSpecifier"/> values.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="style"/> has <see cref="NumberStyle.AllowHexSpecifier"/> and <see cref="NumberStyle.AllowTypeSpecifier"/> values
        /// but not <see cref="NumberStyle.AllowExponent"/>. When type suffix is supplied in a hexadecimal number, exponent (prefixed with a
        /// p or P) is required.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="style"/> is a combination of <see cref="NumberStyle.AllowCurrencySymbol"/> and <see cref="NumberStyle.AllowTypeSpecifier"/>.
        /// </exception>
        /// <remarks>
        /// In .NET Core 3.0 and later, values that are too large to represent are rounded to <see cref="double.PositiveInfinity"/> or
        /// <see cref="double.NegativeInfinity"/> as required by the IEEE 754 specification. In prior versions, including .NET Framework, parsing a
        /// value that was too large to represent resulted in failure. This implementation uses the new parser, but patches it back to .NET Framework 4.0
        /// and upgrades it to support Java's hexadecimal floating point format.
        /// <para/>
        /// The <paramref name="style"/> parameter defines the style elements (such as white space, thousands separators, and currency symbols) that
        /// are allowed in the s parameter for the parse operation to succeed. It must be a combination of bit flags from the <see cref="NumberStyle"/> enumeration.
        /// <para/>
        /// The <paramref name="s"/> parameter can contain <see cref="NumberFormatInfo.PositiveInfinitySymbol"/>, <see cref="NumberFormatInfo.NegativeInfinitySymbol"/>, or
        /// <see cref="NumberFormatInfo.NaNSymbol"/> for the culture specified by <paramref name="provider"/>. Depending on the value of <paramref name="style"/>,
        /// it can also take one of the following forms:
        /// <para/>
        /// [ws] [$] [sign][integral-digits,]integral-digits[.[fractional-digits]][E[sign]exponential-digits][type][ws]
        /// <para/>
        /// Or, if <paramref name="style"/> includes <see cref="NumberStyle.AllowHexSpecifier"/>:
        /// <para/>
        /// [ws] [$] [sign][hexdigits,]hexdigits[.[fractional-hexdigits]][P[sign]exponential-digits][type][ws]
        /// <para/>
        /// Elements framed in square brackets ([ and ]) are optional. The following table describes each element.
        /// <para/>
        /// <list type="table">
        ///     <listheader>
        ///         <term>Element</term>
        ///         <term>Description</term>
        ///     </listheader>
        ///     <item>
        ///         <term><i>ws</i></term>
        ///         <term>A series of white-space characters. White space can appear at the beginning of <paramref name="s"/> if style includes the
        ///             <see cref="NumberStyle.AllowLeadingWhite"/> flag, and it can appear at the end of <paramref name="s"/> if style includes the
        ///             <see cref="NumberStyle.AllowTrailingWhite"/> flag.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>$</i></term>
        ///         <term>A culture-specific currency symbol. Its position in the string is defined by the <see cref="NumberFormatInfo.CurrencyNegativePattern"/>
        ///             and <see cref="NumberFormatInfo.CurrencyPositivePattern"/> properties of the <see cref="NumberFormatInfo"/> object returned by the
        ///             <see cref="IFormatProvider.GetFormat(Type?)"/> method of the <paramref name="provider"/> parameter. The current culture's currency symbol can
        ///             appear in <paramref name="s"/> if style includes the <see cref="NumberStyle.AllowCurrencySymbol"/> flag.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>sign</i></term>
        ///         <term>A negative sign symbol (-) or a positive sign symbol (+). The sign can appear at the beginning of <paramref name="s"/> if
        ///             <paramref name="style"/> includes the <see cref="NumberStyle.AllowLeadingSign"/> flag, and it can appear at the end of <paramref name="s"/>
        ///             if <paramref name="style"/> includes the <see cref="NumberStyle.AllowTrailingSign"/> flag. Parentheses can be used in <paramref name="s"/>
        ///             to indicate a negative value if <paramref name="style"/> includes the <see cref="NumberStyle.AllowParentheses"/> flag.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>integral-digits</i></term>
        ///         <term>A series of digits ranging from 0 to 9 that specify the integral part of the number. The <i>integral-digits</i> element can be absent
        ///             if the string contains the <i>fractional-digits</i> element.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>hexdigits</i></term>
        ///         <term>A sequence of hexadecimal digits from 0 through f, or 0 through F that specify the integral part of the number. The <i>hexdigits</i>
        ///         element can be absent if the string contains the <i>fractional-hexdigits</i> element. Hexadecimal digits can appear in <paramref name="s"/>
        ///         if <paramref name="style"/> includes the <see cref="NumberStyle.AllowHexSpecifier"/> flag.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>,</i></term>
        ///         <term>A culture-specific thousands separator. The thousands separator of the culture specified by <paramref name="provider"/>
        ///         can appear in <paramref name="s"/> if <paramref name="style"/> includes the <see cref="NumberStyle.AllowThousands"/> flag.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>.</i></term>
        ///         <term>A culture-specific decimal point symbol. The decimal point symbol of the culture specified by <paramref name="provider"/>
        ///         can appear in <paramref name="s"/> if <paramref name="style"/> includes the <see cref="NumberStyle.AllowDecimalPoint"/> flag.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>fractional-digits</i>
        ///         </term>
        ///         <term>A series of digits ranging from 0 to 9 that specify the fractional part of the number. Fractional digits can appear in
        ///         <paramref name="s"/> if <paramref name="style"/> includes the <see cref="NumberStyle.AllowDecimalPoint"/> flag.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>fractional-hexdigits</i>
        ///         </term>
        ///         <term>A sequence of hexadecimal digits from 0 through f, or 0 through F that specify the fractional part of the number.
        ///         Fractional digits can appear in <paramref name="s"/> if <paramref name="style"/> includes both the
        ///         <see cref="NumberStyle.AllowDecimalPoint"/> and <see cref="NumberStyle.AllowHexSpecifier"/> flags.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>e</i></term>
        ///         <term>The 'e' or 'E' character, which indicates that the value is represented in exponential notation. The <paramref name="s"/>
        ///         parameter can represent a number in exponential notation if style includes the <see cref="NumberStyle.AllowExponent"/> flag.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>p</i></term>
        ///         <term>The 'p' or 'P' character, which indicates that the hexadecimal value is represented in exponential notation. The <paramref name="s"/>
        ///         parameter can represent a hexadecimal number in exponential notation if style includes both the <see cref="NumberStyle.AllowExponent"/>
        ///         and <see cref="NumberStyle.AllowHexSpecifier"/> flags.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>exponential-digits</i></term>
        ///         <term>A sequence of decimal digits from 0 through 9 that specify an exponent.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>type</i></term>
        ///         <term>The 'f', 'F', 'd', 'D', 'm' or 'M' character, which is the
        ///         [real type suffix](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/lexical-structure#real-literals)
        ///         of the number as specified in the C# language specification. The type suffix can appear in <paramref name="s"/> if <paramref name="style"/>
        ///         includes the <see cref="NumberStyle.AllowTypeSpecifier"/> flag. If <paramref name="style"/> includes the <see cref="NumberStyle.AllowHexSpecifier"/>
        ///         flag, specifying the <see cref="NumberStyle.AllowExponent"/> is required. Including an exponent in the string (prefixed with 'p' or 'P') is
        ///         required for the type suffix characters 'f', 'F', 'd', and 'D' since they would otherwise be interpreted as hexadecimal integral or fractional digits.</term>
        ///     </item>
        /// </list>
        /// <para/>
        /// NOTE: Any terminating NUL (U+0000) characters in <paramref name="s"/> are ignored by the parsing operation, regardless of
        /// the value of the <paramref name="style"/> argument.
        /// <para/>
        /// A string with decimal digits only (which corresponds to the <see cref="NumberStyle.None"/> flag) always parses successfully.
        /// Most of the remaining <see cref="NumberStyle"/> members control elements that may be but are not required to be present in
        /// this input string. The following table indicates how individual <see cref="NumberStyle"/> members affect the elements that may
        /// be present in <paramref name="s"/>.
        /// <list type="table">
        ///     <listheader>
        ///         <term>Non-composite NumberStyle values</term>
        ///         <term>Elements permitted in <paramref name="s"/> in addition to digits</term>
        ///     </listheader>
        ///     <item>
        ///         <term><see cref="NumberStyle.None"/></term>
        ///         <term>Decimal digits only.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowDecimalPoint"/></term>
        ///         <term>The decimal point (.) and <i>fractional-digits</i> elements. However, <i>fractional-digits</i> must consist
        ///         of only one or more 0 digits or the method returns <c>false</c>.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowExponent"/></term>
        ///         <term>The <paramref name="s"/> parameter can also use exponential notation. If <paramref name="s"/> represents a
        ///         number in exponential notation, it must represent an integer within the range of the <see cref="long"/> data type
        ///         without a non-zero, fractional component.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowLeadingWhite"/></term>
        ///         <term>The <i>ws</i> element at the beginning of <paramref name="s"/>.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowTrailingWhite"/></term>
        ///         <term>The <i>ws</i> element at the end of <paramref name="s"/>.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowLeadingSign"/></term>
        ///         <term>A sign can appear before <i>digits</i>.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowTrailingSign"/></term>
        ///         <term>A sign can appear after <i>digits</i>.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowParentheses"/></term>
        ///         <term>The <i>sign</i> element in the form of parentheses enclosing the numeric value.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowThousands"/></term>
        ///         <term>The thousands separator (,) element.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowCurrencySymbol"/></term>
        ///         <term>The <i>$</i> element.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowTypeSpecifier"/></term>
        ///         <term>The literal <i>type</i> suffix used in the literal identifier syntax of C# or Java.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.Currency"/></term>
        ///         <term>All elements. The <paramref name="s"/> parameter cannot represent a hexadecimal
        ///         number or a number in exponential notation (prefixed with an 'e' or 'E').</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.Float"/></term>
        ///         <term>The <i>ws</i> element at the beginning or end of <paramref name="s"/>, sign at
        ///         the beginning of <paramref name="s"/>, and the decimal point (.) symbol. The <paramref name="s"/>
        ///         parameter can also use exponential notation (prefixed with an 'e' or 'E').</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.HexFloat"/></term>
        ///         <term>The <i>ws</i> element at the beginning or end of <paramref name="s"/>, sign at
        ///         the beginning of <paramref name="s"/>, and the decimal point (.) symbol. The <paramref name="s"/>
        ///         parameter can also use exponential notation (prefixed with a 'p' or 'P').</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.Number"/></term>
        ///         <term>The <i>ws</i>, <i>sign</i>, thousands separator (,), and decimal point (.) elements.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.Any"/></term>
        ///         <term>All styles, except <paramref name="s"/> cannot represent a hexadecimal number.</term>
        ///     </item>
        /// </list>
        /// <para/>
        /// The <see cref="NumberStyle"/> enum can be converted to the .NET <see cref="NumberStyles"/> enum by using the
        /// <see cref="NumberStyleExtensions.ToNumberStyles(NumberStyle)"/> extension method.
        /// Similarly, <see cref="NumberStyles"/> enum can be converted to the J2N <see cref="NumberStyle"/> enum by using
        /// the <see cref="NumberStyleExtensions.ToNumberStyle(NumberStyles)"/> extension method.
        /// <para/>
        /// If the <see cref="NumberStyle.AllowHexSpecifier"/> flag is used, <paramref name="s"/> must be a hexadecimal value which may include an optional 0x or 0X prefix.
        /// For example, "0xC9AF3.8BD4C". This flag may be combined with any of the flags specified in <see cref="NumberStyle.Float"/> and also
        /// <see cref="NumberStyle.AllowTypeSpecifier"/>. (The <see cref="NumberStyle"/> enumeration has a composite style, <see cref="NumberStyle.HexFloat"/>,
        /// that includes all of those flags except <see cref="NumberStyle.AllowTypeSpecifier"/>.)
        /// <para/>
        /// The <paramref name="provider"/> parameter is an <see cref="IFormatProvider"/> implementation, such as a <see cref="CultureInfo"/> object,
        /// a <see cref="NumberFormatInfo"/> object or a <see cref="J2N.Text.StringFormatter"/> object, whose <see cref="IFormatProvider.GetFormat(Type?)"/>
        /// method returns a <see cref="NumberFormatInfo"/> object. The <see cref="NumberFormatInfo"/> object provides culture-specific information about
        /// the format of <paramref name="s"/>. When the <see cref="Parse(string, IFormatProvider)"/> method is invoked, it calls the provider parameter's
        /// <see cref="IFormatProvider.GetFormat(Type?)"/> method and passes it a <see cref="Type"/> object that represents the <see cref="NumberFormatInfo"/> type.
        /// The <see cref="IFormatProvider.GetFormat(Type?)"/> method then returns the <see cref="NumberFormatInfo"/> object that provides information about the
        /// format of the <paramref name="s"/> parameter. There are three ways to use the <paramref name="provider"/> parameter to supply custom formatting
        /// information to the parse operation:
        /// <list type="bullet">
        ///     <item><description>You can pass a <see cref="CultureInfo"/> object that represents the culture that supplies formatting information. Its
        ///     <see cref="IFormatProvider.GetFormat(Type?)"/> method returns the <see cref="NumberFormatInfo"/> object that provides numeric formatting
        ///     information for that culture.</description></item>
        ///     <item><description>You can pass the actual <see cref="NumberFormatInfo"/> object that provides numeric formatting information. (Its
        ///     implementation of <see cref="IFormatProvider.GetFormat(Type?)"/> just returns itself.)</description></item>
        ///     <item><description>You can pass a custom object that implements <see cref="IFormatProvider"/>. Its <see cref="IFormatProvider.GetFormat(Type?)"/>
        ///     method instantiates and returns the <see cref="NumberFormatInfo"/> object that provides formatting information.</description></item>
        /// </list>
        /// If <paramref name="provider"/> is <c>null</c> or a <see cref="NumberFormatInfo"/> object cannot be obtained, the <see cref="NumberFormatInfo"/>
        /// object for the current culture is used.
        /// <para/>
        /// If <paramref name="s"/> is out of range of the <see cref="double"/> data type, the method returns <see cref="double.NegativeInfinity"/> if
        /// <paramref name="s"/> is less than <see cref="double.MinValue"/> and <see cref="double.PositiveInfinity"/> if <paramref name="s"/> is greater
        /// than <see cref="double.MaxValue"/>.
        /// <para/>
        /// If a separator is encountered in the <paramref name="s"/> parameter during a parse operation, and the applicable currency or number
        /// decimal and group separators are the same, the parse operation assumes that the separator is a decimal separator rather than a group
        /// separator. For more information about separators, see <see cref="NumberFormatInfo.CurrencyDecimalSeparator"/>,
        /// <see cref="NumberFormatInfo.NumberDecimalSeparator"/>, <see cref="NumberFormatInfo.CurrencyGroupSeparator"/>, and
        /// <see cref="NumberFormatInfo.NumberGroupSeparator"/>.
        /// </remarks>
        /// <seealso cref="ValueOf(string, NumberStyle, IFormatProvider?)"/>
        /// <seealso cref="TryParse(string, NumberStyle, IFormatProvider?, out double)"/>
        public static double Parse(string s, NumberStyle style, IFormatProvider? provider) // J2N: Renamed from ParseDouble()
        {
            if (s is null)
                throw new ArgumentNullException(nameof(s));
            NumberStyleExtensions.ValidateParseStyleFloatingPoint(style);

            return DotNetNumber.ParseDouble(s, style, NumberFormatInfo.GetInstance(provider));
        }

#if FEATURE_READONLYSPAN
        /// <summary>
        /// Converts a character span that contains the string representation of a number in a specified style and culture-specific format to
        /// its double-precision floating-point number equivalent.
        /// <para/>
        /// Usage Note: To approximately match the behavior of the JDK for decimal numbers, use <see cref="NumberStyle.Float"/> combined with
        /// <see cref="NumberStyle.AllowTypeSpecifier"/> for <paramref name="style"/> and <see cref="NumberFormatInfo.InvariantInfo"/> for <paramref name="provider"/>.
        /// To approximately match the behavior of the JDK for hexadecimal numbers, use <see cref="NumberStyle.HexFloat"/> combined with
        /// <see cref="NumberStyle.AllowTypeSpecifier"/> for <paramref name="style"/> and <see cref="NumberFormatInfo.InvariantInfo"/> for <paramref name="provider"/>.
        /// These options will parse any number that is supported by Java, but the hex specifier, exponent, and type suffix are all optional.
        /// </summary>
        /// <param name="s">A string that contains a number to convert.</param>
        /// <param name="style">A bitwise combination of enumeration values that indicates the style elements
        /// that can be present in <paramref name="s"/>. Typical values to specify is <see cref="NumberStyle.Float"/>
        /// combined with <see cref="NumberStyle.AllowThousands"/> or, in the case of hexadecimal numbers,
        /// <see cref="NumberStyle.HexFloat"/>.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information about <paramref name="s"/>.</param>
        /// <returns>A double-precision floating-point number equivalent to the numeric value or symbol specified in <paramref name="s"/>.</returns>
        /// <exception cref="FormatException"><paramref name="s"/> does not represent a numeric value.</exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="style"/> is not a <see cref="NumberStyle"/> value.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="style"/> is not a combination of <see cref="NumberStyle.AllowHexSpecifier"/>, <see cref="NumberStyle.HexFloat"/>
        /// and <see cref="NumberStyle.AllowTypeSpecifier"/> values.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="style"/> has <see cref="NumberStyle.AllowHexSpecifier"/> and <see cref="NumberStyle.AllowTypeSpecifier"/> values
        /// but not <see cref="NumberStyle.AllowExponent"/>. When type suffix is supplied in a hexadecimal number, exponent (prefixed with a
        /// p or P) is required.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="style"/> is a combination of <see cref="NumberStyle.AllowCurrencySymbol"/> and <see cref="NumberStyle.AllowTypeSpecifier"/>.
        /// </exception>
        /// <remarks>
        /// In .NET Core 3.0 and later, values that are too large to represent are rounded to <see cref="double.PositiveInfinity"/> or
        /// <see cref="double.NegativeInfinity"/> as required by the IEEE 754 specification. In prior versions, including .NET Framework, parsing a
        /// value that was too large to represent resulted in failure. This implementation uses the new parser, but patches it back to .NET Framework 4.0
        /// and upgrades it to support Java's hexadecimal floating point format.
        /// <para/>
        /// The <paramref name="style"/> parameter defines the style elements (such as white space, thousands separators, and currency symbols) that
        /// are allowed in the s parameter for the parse operation to succeed. It must be a combination of bit flags from the <see cref="NumberStyle"/> enumeration.
        /// <para/>
        /// The <paramref name="s"/> parameter can contain <see cref="NumberFormatInfo.PositiveInfinitySymbol"/>, <see cref="NumberFormatInfo.NegativeInfinitySymbol"/>, or
        /// <see cref="NumberFormatInfo.NaNSymbol"/> for the culture specified by <paramref name="provider"/>. Depending on the value of <paramref name="style"/>,
        /// it can also take one of the following forms:
        /// <para/>
        /// [ws] [$] [sign][integral-digits,]integral-digits[.[fractional-digits]][E[sign]exponential-digits][type][ws]
        /// <para/>
        /// Or, if <paramref name="style"/> includes <see cref="NumberStyle.AllowHexSpecifier"/>:
        /// <para/>
        /// [ws] [$] [sign][hexdigits,]hexdigits[.[fractional-hexdigits]][P[sign]exponential-digits][type][ws]
        /// <para/>
        /// Elements framed in square brackets ([ and ]) are optional. The following table describes each element.
        /// <para/>
        /// <list type="table">
        ///     <listheader>
        ///         <term>Element</term>
        ///         <term>Description</term>
        ///     </listheader>
        ///     <item>
        ///         <term><i>ws</i></term>
        ///         <term>A series of white-space characters. White space can appear at the beginning of <paramref name="s"/> if style includes the
        ///             <see cref="NumberStyle.AllowLeadingWhite"/> flag, and it can appear at the end of <paramref name="s"/> if style includes the
        ///             <see cref="NumberStyle.AllowTrailingWhite"/> flag.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>$</i></term>
        ///         <term>A culture-specific currency symbol. Its position in the string is defined by the <see cref="NumberFormatInfo.CurrencyNegativePattern"/>
        ///             and <see cref="NumberFormatInfo.CurrencyPositivePattern"/> properties of the <see cref="NumberFormatInfo"/> object returned by the
        ///             <see cref="IFormatProvider.GetFormat(Type?)"/> method of the <paramref name="provider"/> parameter. The current culture's currency symbol can
        ///             appear in <paramref name="s"/> if style includes the <see cref="NumberStyle.AllowCurrencySymbol"/> flag.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>sign</i></term>
        ///         <term>A negative sign symbol (-) or a positive sign symbol (+). The sign can appear at the beginning of <paramref name="s"/> if
        ///             <paramref name="style"/> includes the <see cref="NumberStyle.AllowLeadingSign"/> flag, and it can appear at the end of <paramref name="s"/>
        ///             if <paramref name="style"/> includes the <see cref="NumberStyle.AllowTrailingSign"/> flag. Parentheses can be used in <paramref name="s"/>
        ///             to indicate a negative value if <paramref name="style"/> includes the <see cref="NumberStyle.AllowParentheses"/> flag.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>integral-digits</i></term>
        ///         <term>A series of digits ranging from 0 to 9 that specify the integral part of the number. The <i>integral-digits</i> element can be absent
        ///             if the string contains the <i>fractional-digits</i> element.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>hexdigits</i></term>
        ///         <term>A sequence of hexadecimal digits from 0 through f, or 0 through F that specify the integral part of the number. The <i>hexdigits</i>
        ///         element can be absent if the string contains the <i>fractional-hexdigits</i> element. Hexadecimal digits can appear in <paramref name="s"/>
        ///         if <paramref name="style"/> includes the <see cref="NumberStyle.AllowHexSpecifier"/> flag.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>,</i></term>
        ///         <term>A culture-specific thousands separator. The thousands separator of the culture specified by <paramref name="provider"/>
        ///         can appear in <paramref name="s"/> if <paramref name="style"/> includes the <see cref="NumberStyle.AllowThousands"/> flag.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>.</i></term>
        ///         <term>A culture-specific decimal point symbol. The decimal point symbol of the culture specified by <paramref name="provider"/>
        ///         can appear in <paramref name="s"/> if <paramref name="style"/> includes the <see cref="NumberStyle.AllowDecimalPoint"/> flag.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>fractional-digits</i>
        ///         </term>
        ///         <term>A series of digits ranging from 0 to 9 that specify the fractional part of the number. Fractional digits can appear in
        ///         <paramref name="s"/> if <paramref name="style"/> includes the <see cref="NumberStyle.AllowDecimalPoint"/> flag.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>fractional-hexdigits</i>
        ///         </term>
        ///         <term>A sequence of hexadecimal digits from 0 through f, or 0 through F that specify the fractional part of the number.
        ///         Fractional digits can appear in <paramref name="s"/> if <paramref name="style"/> includes both the
        ///         <see cref="NumberStyle.AllowDecimalPoint"/> and <see cref="NumberStyle.AllowHexSpecifier"/> flags.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>e</i></term>
        ///         <term>The 'e' or 'E' character, which indicates that the value is represented in exponential notation. The <paramref name="s"/>
        ///         parameter can represent a number in exponential notation if style includes the <see cref="NumberStyle.AllowExponent"/> flag.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>p</i></term>
        ///         <term>The 'p' or 'P' character, which indicates that the hexadecimal value is represented in exponential notation. The <paramref name="s"/>
        ///         parameter can represent a hexadecimal number in exponential notation if style includes both the <see cref="NumberStyle.AllowExponent"/>
        ///         and <see cref="NumberStyle.AllowHexSpecifier"/> flags.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>exponential-digits</i></term>
        ///         <term>A sequence of decimal digits from 0 through 9 that specify an exponent.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>type</i></term>
        ///         <term>The 'f', 'F', 'd', 'D', 'm' or 'M' character, which is the
        ///         [real type suffix](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/lexical-structure#real-literals)
        ///         of the number as specified in the C# language specification. The type suffix can appear in <paramref name="s"/> if <paramref name="style"/>
        ///         includes the <see cref="NumberStyle.AllowTypeSpecifier"/> flag. If <paramref name="style"/> includes the <see cref="NumberStyle.AllowHexSpecifier"/>
        ///         flag, specifying the <see cref="NumberStyle.AllowExponent"/> is required. Including an exponent in the string (prefixed with 'p' or 'P') is
        ///         required for the type suffix characters 'f', 'F', 'd', and 'D' since they would otherwise be interpreted as hexadecimal integral or fractional digits.</term>
        ///     </item>
        /// </list>
        /// <para/>
        /// NOTE: Any terminating NUL (U+0000) characters in <paramref name="s"/> are ignored by the parsing operation, regardless of
        /// the value of the <paramref name="style"/> argument.
        /// <para/>
        /// A string with decimal digits only (which corresponds to the <see cref="NumberStyle.None"/> flag) always parses successfully.
        /// Most of the remaining <see cref="NumberStyle"/> members control elements that may be but are not required to be present in
        /// this input string. The following table indicates how individual <see cref="NumberStyle"/> members affect the elements that may
        /// be present in <paramref name="s"/>.
        /// <list type="table">
        ///     <listheader>
        ///         <term>Non-composite NumberStyle values</term>
        ///         <term>Elements permitted in <paramref name="s"/> in addition to digits</term>
        ///     </listheader>
        ///     <item>
        ///         <term><see cref="NumberStyle.None"/></term>
        ///         <term>Decimal digits only.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowDecimalPoint"/></term>
        ///         <term>The decimal point (.) and <i>fractional-digits</i> elements. However, <i>fractional-digits</i> must consist
        ///         of only one or more 0 digits or the method returns <c>false</c>.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowExponent"/></term>
        ///         <term>The <paramref name="s"/> parameter can also use exponential notation. If <paramref name="s"/> represents a
        ///         number in exponential notation, it must represent an integer within the range of the <see cref="long"/> data type
        ///         without a non-zero, fractional component.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowLeadingWhite"/></term>
        ///         <term>The <i>ws</i> element at the beginning of <paramref name="s"/>.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowTrailingWhite"/></term>
        ///         <term>The <i>ws</i> element at the end of <paramref name="s"/>.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowLeadingSign"/></term>
        ///         <term>A sign can appear before <i>digits</i>.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowTrailingSign"/></term>
        ///         <term>A sign can appear after <i>digits</i>.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowParentheses"/></term>
        ///         <term>The <i>sign</i> element in the form of parentheses enclosing the numeric value.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowThousands"/></term>
        ///         <term>The thousands separator (,) element.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowCurrencySymbol"/></term>
        ///         <term>The <i>$</i> element.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowTypeSpecifier"/></term>
        ///         <term>The literal <i>type</i> suffix used in the literal identifier syntax of C# or Java.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.Currency"/></term>
        ///         <term>All elements. The <paramref name="s"/> parameter cannot represent a hexadecimal
        ///         number or a number in exponential notation (prefixed with an 'e' or 'E').</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.Float"/></term>
        ///         <term>The <i>ws</i> element at the beginning or end of <paramref name="s"/>, sign at
        ///         the beginning of <paramref name="s"/>, and the decimal point (.) symbol. The <paramref name="s"/>
        ///         parameter can also use exponential notation (prefixed with an 'e' or 'E').</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.HexFloat"/></term>
        ///         <term>The <i>ws</i> element at the beginning or end of <paramref name="s"/>, sign at
        ///         the beginning of <paramref name="s"/>, and the decimal point (.) symbol. The <paramref name="s"/>
        ///         parameter can also use exponential notation (prefixed with a 'p' or 'P').</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.Number"/></term>
        ///         <term>The <i>ws</i>, <i>sign</i>, thousands separator (,), and decimal point (.) elements.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.Any"/></term>
        ///         <term>All styles, except <paramref name="s"/> cannot represent a hexadecimal number.</term>
        ///     </item>
        /// </list>
        /// <para/>
        /// The <see cref="NumberStyle"/> enum can be converted to the .NET <see cref="NumberStyles"/> enum by using the
        /// <see cref="NumberStyleExtensions.ToNumberStyles(NumberStyle)"/> extension method.
        /// Similarly, <see cref="NumberStyles"/> enum can be converted to the J2N <see cref="NumberStyle"/> enum by using
        /// the <see cref="NumberStyleExtensions.ToNumberStyle(NumberStyles)"/> extension method.
        /// <para/>
        /// If the <see cref="NumberStyle.AllowHexSpecifier"/> flag is used, <paramref name="s"/> must be a hexadecimal value which may include an optional 0x or 0X prefix.
        /// For example, "0xC9AF3.8BD4C". This flag may be combined with any of the flags specified in <see cref="NumberStyle.Float"/> and also
        /// <see cref="NumberStyle.AllowTypeSpecifier"/>. (The <see cref="NumberStyle"/> enumeration has a composite style, <see cref="NumberStyle.HexFloat"/>,
        /// that includes all of those flags except <see cref="NumberStyle.AllowTypeSpecifier"/>.)
        /// <para/>
        /// The <paramref name="provider"/> parameter is an <see cref="IFormatProvider"/> implementation, such as a <see cref="CultureInfo"/> object,
        /// a <see cref="NumberFormatInfo"/> object or a <see cref="J2N.Text.StringFormatter"/> object, whose <see cref="IFormatProvider.GetFormat(Type?)"/>
        /// method returns a <see cref="NumberFormatInfo"/> object. The <see cref="NumberFormatInfo"/> object provides culture-specific information about
        /// the format of <paramref name="s"/>. When the <see cref="Parse(string, IFormatProvider)"/> method is invoked, it calls the provider parameter's
        /// <see cref="IFormatProvider.GetFormat(Type?)"/> method and passes it a <see cref="Type"/> object that represents the <see cref="NumberFormatInfo"/> type.
        /// The <see cref="IFormatProvider.GetFormat(Type?)"/> method then returns the <see cref="NumberFormatInfo"/> object that provides information about the
        /// format of the <paramref name="s"/> parameter. There are three ways to use the <paramref name="provider"/> parameter to supply custom formatting
        /// information to the parse operation:
        /// <list type="bullet">
        ///     <item><description>You can pass a <see cref="CultureInfo"/> object that represents the culture that supplies formatting information. Its
        ///     <see cref="IFormatProvider.GetFormat(Type?)"/> method returns the <see cref="NumberFormatInfo"/> object that provides numeric formatting
        ///     information for that culture.</description></item>
        ///     <item><description>You can pass the actual <see cref="NumberFormatInfo"/> object that provides numeric formatting information. (Its
        ///     implementation of <see cref="IFormatProvider.GetFormat(Type?)"/> just returns itself.)</description></item>
        ///     <item><description>You can pass a custom object that implements <see cref="IFormatProvider"/>. Its <see cref="IFormatProvider.GetFormat(Type?)"/>
        ///     method instantiates and returns the <see cref="NumberFormatInfo"/> object that provides formatting information.</description></item>
        /// </list>
        /// If <paramref name="provider"/> is <c>null</c> or a <see cref="NumberFormatInfo"/> object cannot be obtained, the <see cref="NumberFormatInfo"/>
        /// object for the current culture is used.
        /// <para/>
        /// If <paramref name="s"/> is out of range of the <see cref="double"/> data type, the method returns <see cref="double.NegativeInfinity"/> if
        /// <paramref name="s"/> is less than <see cref="double.MinValue"/> and <see cref="double.PositiveInfinity"/> if <paramref name="s"/> is greater
        /// than <see cref="double.MaxValue"/>.
        /// <para/>
        /// If a separator is encountered in the <paramref name="s"/> parameter during a parse operation, and the applicable currency or number
        /// decimal and group separators are the same, the parse operation assumes that the separator is a decimal separator rather than a group
        /// separator. For more information about separators, see <see cref="NumberFormatInfo.CurrencyDecimalSeparator"/>,
        /// <see cref="NumberFormatInfo.NumberDecimalSeparator"/>, <see cref="NumberFormatInfo.CurrencyGroupSeparator"/>, and
        /// <see cref="NumberFormatInfo.NumberGroupSeparator"/>.
        /// </remarks>
        /// <seealso cref="ValueOf(ReadOnlySpan{char}, NumberStyle, IFormatProvider?)"/>
        /// <seealso cref="TryParse(ReadOnlySpan{char}, NumberStyle, IFormatProvider?, out double)"/>
        public static double Parse(ReadOnlySpan<char> s, NumberStyle style, IFormatProvider? provider) // J2N: Renamed from ParseDouble()
        {
            NumberStyleExtensions.ValidateParseStyleFloatingPoint(style);

            return DotNetNumber.ParseDouble(s, style, NumberFormatInfo.GetInstance(provider));
        }
#endif

        #endregion Parse_CharSequence_NumberStyle_IFormatProvider

        #region TryParse_CharSequence_NumberStyle_IFormatProvider_Double

        /// <summary>
        /// Converts the string representation of a number in a specified style and culture-specific format to
        /// its double-precision floating-point number equivalent. A return value indicates whether the
        /// conversion succeeded or failed.
        /// <para/>
        /// Usage Note: To approximately match the behavior of the JDK for decimal numbers, use <see cref="NumberStyle.Float"/> combined with
        /// <see cref="NumberStyle.AllowTypeSpecifier"/> for <paramref name="style"/> and <see cref="NumberFormatInfo.InvariantInfo"/> for <paramref name="provider"/>.
        /// To approximately match the behavior of the JDK for hexadecimal numbers, use <see cref="NumberStyle.HexFloat"/> combined with
        /// <see cref="NumberStyle.AllowTypeSpecifier"/> for <paramref name="style"/> and <see cref="NumberFormatInfo.InvariantInfo"/> for <paramref name="provider"/>.
        /// These options will parse any number that is supported by Java, but the hex specifier, exponent, and type suffix are all optional.
        /// </summary>
        /// <param name="s">A string representing a number to convert.</param>
        /// <param name="style">A bitwise combination of enumeration values that indicates the style elements
        /// that can be present in <paramref name="s"/>. Typical values to specify is <see cref="NumberStyle.Float"/>
        /// combined with <see cref="NumberStyle.AllowThousands"/> or, in the case of hexadecimal numbers,
        /// <see cref="NumberStyle.HexFloat"/>.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information about <paramref name="s"/>.</param>
        /// <param name="result">When this method returns, contains the double-precision floating-point number equivalent to the
        /// numeric value or symbol contained in <paramref name="s"/>, if the conversion succeeded, or zero if the conversion failed.
        /// The conversion fails if the <paramref name="s"/> parameter is <c>null</c> or <see cref="string.Empty"/>, is not in a format
        /// compliant with <paramref name="style"/>, or if <paramref name="style"/> is not a valid combination of <see cref="NumberStyle"/>
        /// enumeration constants. This parameter is passed uninitialized; any value originally supplied in result will be overwritten.</param>
        /// <returns><c>true</c> if <paramref name="s"/> was converted successfully; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="style"/> is not a <see cref="NumberStyle"/> value.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="style"/> is not a combination of <see cref="NumberStyle.AllowHexSpecifier"/>, <see cref="NumberStyle.HexFloat"/>
        /// and <see cref="NumberStyle.AllowTypeSpecifier"/> values.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="style"/> has <see cref="NumberStyle.AllowHexSpecifier"/> and <see cref="NumberStyle.AllowTypeSpecifier"/> values
        /// but not <see cref="NumberStyle.AllowExponent"/>. When type suffix is supplied in a hexadecimal number, exponent (prefixed with a
        /// p or P) is required.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="style"/> is a combination of <see cref="NumberStyle.AllowCurrencySymbol"/> and <see cref="NumberStyle.AllowTypeSpecifier"/>.
        /// </exception>
        /// <remarks>
        /// In .NET Core 3.0 and later, values that are too large to represent are rounded to <see cref="double.PositiveInfinity"/> or
        /// <see cref="double.NegativeInfinity"/> as required by the IEEE 754 specification. In prior versions, including .NET Framework, parsing a
        /// value that was too large to represent resulted in failure. This implementation uses the new parser, but patches it back to .NET Framework 4.0
        /// and upgrades it to support Java's hexadecimal floating point format.
        /// <para/>
        /// This overload differs from the <see cref="Parse(string, NumberStyle, IFormatProvider?)"/> method by returning a Boolean value that indicates
        /// whether the parse operation succeeded instead of returning the parsed numeric value. It eliminates the need to use exception handling to test
        /// for a <see cref="FormatException"/> in the event that <paramref name="s"/> is invalid and cannot be successfully parsed.
        /// <para/>
        /// The <paramref name="style"/> parameter defines the style elements (such as white space, thousands separators, and currency symbols) that
        /// are allowed in the s parameter for the parse operation to succeed. It must be a combination of bit flags from the <see cref="NumberStyle"/> enumeration.
        /// <para/>
        /// The <paramref name="s"/> parameter can contain <see cref="NumberFormatInfo.PositiveInfinitySymbol"/>, <see cref="NumberFormatInfo.NegativeInfinitySymbol"/>, or
        /// <see cref="NumberFormatInfo.NaNSymbol"/> for the culture specified by <paramref name="provider"/>. Depending on the value of <paramref name="style"/>,
        /// it can also take one of the following forms:
        /// <para/>
        /// [ws] [$] [sign][integral-digits,]integral-digits[.[fractional-digits]][E[sign]exponential-digits][type][ws]
        /// <para/>
        /// Or, if <paramref name="style"/> includes <see cref="NumberStyle.AllowHexSpecifier"/>:
        /// <para/>
        /// [ws] [$] [sign][hexdigits,]hexdigits[.[fractional-hexdigits]][P[sign]exponential-digits][type][ws]
        /// <para/>
        /// Elements framed in square brackets ([ and ]) are optional. The following table describes each element.
        /// <para/>
        /// <list type="table">
        ///     <listheader>
        ///         <term>Element</term>
        ///         <term>Description</term>
        ///     </listheader>
        ///     <item>
        ///         <term><i>ws</i></term>
        ///         <term>A series of white-space characters. White space can appear at the beginning of <paramref name="s"/> if style includes the
        ///             <see cref="NumberStyle.AllowLeadingWhite"/> flag, and it can appear at the end of <paramref name="s"/> if style includes the
        ///             <see cref="NumberStyle.AllowTrailingWhite"/> flag.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>$</i></term>
        ///         <term>A culture-specific currency symbol. Its position in the string is defined by the <see cref="NumberFormatInfo.CurrencyNegativePattern"/>
        ///             and <see cref="NumberFormatInfo.CurrencyPositivePattern"/> properties of the <see cref="NumberFormatInfo"/> object returned by the
        ///             <see cref="IFormatProvider.GetFormat(Type?)"/> method of the <paramref name="provider"/> parameter. The current culture's currency symbol can
        ///             appear in <paramref name="s"/> if style includes the <see cref="NumberStyle.AllowCurrencySymbol"/> flag.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>sign</i></term>
        ///         <term>A negative sign symbol (-) or a positive sign symbol (+). The sign can appear at the beginning of <paramref name="s"/> if
        ///             <paramref name="style"/> includes the <see cref="NumberStyle.AllowLeadingSign"/> flag, and it can appear at the end of <paramref name="s"/>
        ///             if <paramref name="style"/> includes the <see cref="NumberStyle.AllowTrailingSign"/> flag. Parentheses can be used in <paramref name="s"/>
        ///             to indicate a negative value if <paramref name="style"/> includes the <see cref="NumberStyle.AllowParentheses"/> flag.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>integral-digits</i></term>
        ///         <term>A series of digits ranging from 0 to 9 that specify the integral part of the number. The <i>integral-digits</i> element can be absent
        ///             if the string contains the <i>fractional-digits</i> element.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>hexdigits</i></term>
        ///         <term>A sequence of hexadecimal digits from 0 through f, or 0 through F that specify the integral part of the number. The <i>hexdigits</i>
        ///         element can be absent if the string contains the <i>fractional-hexdigits</i> element. Hexadecimal digits can appear in <paramref name="s"/>
        ///         if <paramref name="style"/> includes the <see cref="NumberStyle.AllowHexSpecifier"/> flag.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>,</i></term>
        ///         <term>A culture-specific thousands separator. The thousands separator of the culture specified by <paramref name="provider"/>
        ///         can appear in <paramref name="s"/> if <paramref name="style"/> includes the <see cref="NumberStyle.AllowThousands"/> flag.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>.</i></term>
        ///         <term>A culture-specific decimal point symbol. The decimal point symbol of the culture specified by <paramref name="provider"/>
        ///         can appear in <paramref name="s"/> if <paramref name="style"/> includes the <see cref="NumberStyle.AllowDecimalPoint"/> flag.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>fractional-digits</i>
        ///         </term>
        ///         <term>A series of digits ranging from 0 to 9 that specify the fractional part of the number. Fractional digits can appear in
        ///         <paramref name="s"/> if <paramref name="style"/> includes the <see cref="NumberStyle.AllowDecimalPoint"/> flag.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>fractional-hexdigits</i>
        ///         </term>
        ///         <term>A sequence of hexadecimal digits from 0 through f, or 0 through F that specify the fractional part of the number.
        ///         Fractional digits can appear in <paramref name="s"/> if <paramref name="style"/> includes both the
        ///         <see cref="NumberStyle.AllowDecimalPoint"/> and <see cref="NumberStyle.AllowHexSpecifier"/> flags.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>e</i></term>
        ///         <term>The 'e' or 'E' character, which indicates that the value is represented in exponential notation. The <paramref name="s"/>
        ///         parameter can represent a number in exponential notation if style includes the <see cref="NumberStyle.AllowExponent"/> flag.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>p</i></term>
        ///         <term>The 'p' or 'P' character, which indicates that the hexadecimal value is represented in exponential notation. The <paramref name="s"/>
        ///         parameter can represent a hexadecimal number in exponential notation if style includes both the <see cref="NumberStyle.AllowExponent"/>
        ///         and <see cref="NumberStyle.AllowHexSpecifier"/> flags.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>exponential-digits</i></term>
        ///         <term>A sequence of decimal digits from 0 through 9 that specify an exponent.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>type</i></term>
        ///         <term>The 'f', 'F', 'd', 'D', 'm' or 'M' character, which is the
        ///         [real type suffix](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/lexical-structure#real-literals)
        ///         of the number as specified in the C# language specification. The type suffix can appear in <paramref name="s"/> if <paramref name="style"/>
        ///         includes the <see cref="NumberStyle.AllowTypeSpecifier"/> flag. If <paramref name="style"/> includes the <see cref="NumberStyle.AllowHexSpecifier"/>
        ///         flag, specifying the <see cref="NumberStyle.AllowExponent"/> is required. Including an exponent in the string (prefixed with 'p' or 'P') is
        ///         required for the type suffix characters 'f', 'F', 'd', and 'D' since they would otherwise be interpreted as hexadecimal integral or fractional digits.</term>
        ///     </item>
        /// </list>
        /// <para/>
        /// NOTE: Any terminating NUL (U+0000) characters in <paramref name="s"/> are ignored by the parsing operation, regardless of
        /// the value of the <paramref name="style"/> argument.
        /// <para/>
        /// A string with decimal digits only (which corresponds to the <see cref="NumberStyle.None"/> flag) always parses successfully.
        /// Most of the remaining <see cref="NumberStyle"/> members control elements that may be but are not required to be present in
        /// this input string. The following table indicates how individual <see cref="NumberStyle"/> members affect the elements that may
        /// be present in <paramref name="s"/>.
        /// <list type="table">
        ///     <listheader>
        ///         <term>Non-composite NumberStyle values</term>
        ///         <term>Elements permitted in <paramref name="s"/> in addition to digits</term>
        ///     </listheader>
        ///     <item>
        ///         <term><see cref="NumberStyle.None"/></term>
        ///         <term>Decimal digits only.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowDecimalPoint"/></term>
        ///         <term>The decimal point (.) and <i>fractional-digits</i> elements. However, <i>fractional-digits</i> must consist
        ///         of only one or more 0 digits or the method returns <c>false</c>.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowExponent"/></term>
        ///         <term>The <paramref name="s"/> parameter can also use exponential notation. If <paramref name="s"/> represents a
        ///         number in exponential notation, it must represent an integer within the range of the <see cref="long"/> data type
        ///         without a non-zero, fractional component.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowLeadingWhite"/></term>
        ///         <term>The <i>ws</i> element at the beginning of <paramref name="s"/>.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowTrailingWhite"/></term>
        ///         <term>The <i>ws</i> element at the end of <paramref name="s"/>.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowLeadingSign"/></term>
        ///         <term>A sign can appear before <i>digits</i>.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowTrailingSign"/></term>
        ///         <term>A sign can appear after <i>digits</i>.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowParentheses"/></term>
        ///         <term>The <i>sign</i> element in the form of parentheses enclosing the numeric value.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowThousands"/></term>
        ///         <term>The thousands separator (,) element.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowCurrencySymbol"/></term>
        ///         <term>The <i>$</i> element.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowTypeSpecifier"/></term>
        ///         <term>The literal <i>type</i> suffix used in the literal identifier syntax of C# or Java.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.Currency"/></term>
        ///         <term>All elements. The <paramref name="s"/> parameter cannot represent a hexadecimal
        ///         number or a number in exponential notation (prefixed with an 'e' or 'E').</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.Float"/></term>
        ///         <term>The <i>ws</i> element at the beginning or end of <paramref name="s"/>, sign at
        ///         the beginning of <paramref name="s"/>, and the decimal point (.) symbol. The <paramref name="s"/>
        ///         parameter can also use exponential notation (prefixed with an 'e' or 'E').</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.HexFloat"/></term>
        ///         <term>The <i>ws</i> element at the beginning or end of <paramref name="s"/>, sign at
        ///         the beginning of <paramref name="s"/>, and the decimal point (.) symbol. The <paramref name="s"/>
        ///         parameter can also use exponential notation (prefixed with a 'p' or 'P').</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.Number"/></term>
        ///         <term>The <i>ws</i>, <i>sign</i>, thousands separator (,), and decimal point (.) elements.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.Any"/></term>
        ///         <term>All styles, except <paramref name="s"/> cannot represent a hexadecimal number.</term>
        ///     </item>
        /// </list>
        /// <para/>
        /// The <see cref="NumberStyle"/> enum can be converted to the .NET <see cref="NumberStyles"/> enum by using the
        /// <see cref="NumberStyleExtensions.ToNumberStyles(NumberStyle)"/> extension method.
        /// Similarly, <see cref="NumberStyles"/> enum can be converted to the J2N <see cref="NumberStyle"/> enum by using
        /// the <see cref="NumberStyleExtensions.ToNumberStyle(NumberStyles)"/> extension method.
        /// <para/>
        /// If the <see cref="NumberStyle.AllowHexSpecifier"/> flag is used, <paramref name="s"/> must be a hexadecimal value which may include an optional 0x or 0X prefix.
        /// For example, "0xC9AF3.8BD4C". This flag may be combined with any of the flags specified in <see cref="NumberStyle.Float"/> and also
        /// <see cref="NumberStyle.AllowTypeSpecifier"/>. (The <see cref="NumberStyle"/> enumeration has a composite style, <see cref="NumberStyle.HexFloat"/>,
        /// that includes all of those flags except <see cref="NumberStyle.AllowTypeSpecifier"/>.)
        /// <para/>
        /// The <paramref name="provider"/> parameter is an <see cref="IFormatProvider"/> implementation, such as a <see cref="CultureInfo"/> object,
        /// a <see cref="NumberFormatInfo"/> object or a <see cref="J2N.Text.StringFormatter"/> object, whose <see cref="IFormatProvider.GetFormat(Type?)"/>
        /// method returns a <see cref="NumberFormatInfo"/> object. The <see cref="NumberFormatInfo"/> object provides culture-specific information about
        /// the format of <paramref name="s"/>. When the <see cref="Parse(string, IFormatProvider)"/> method is invoked, it calls the provider parameter's
        /// <see cref="IFormatProvider.GetFormat(Type?)"/> method and passes it a <see cref="Type"/> object that represents the <see cref="NumberFormatInfo"/> type.
        /// The <see cref="IFormatProvider.GetFormat(Type?)"/> method then returns the <see cref="NumberFormatInfo"/> object that provides information about the
        /// format of the <paramref name="s"/> parameter. There are three ways to use the <paramref name="provider"/> parameter to supply custom formatting
        /// information to the parse operation:
        /// <list type="bullet">
        ///     <item><description>You can pass a <see cref="CultureInfo"/> object that represents the culture that supplies formatting information. Its
        ///     <see cref="IFormatProvider.GetFormat(Type?)"/> method returns the <see cref="NumberFormatInfo"/> object that provides numeric formatting
        ///     information for that culture.</description></item>
        ///     <item><description>You can pass the actual <see cref="NumberFormatInfo"/> object that provides numeric formatting information. (Its
        ///     implementation of <see cref="IFormatProvider.GetFormat(Type?)"/> just returns itself.)</description></item>
        ///     <item><description>You can pass a custom object that implements <see cref="IFormatProvider"/>. Its <see cref="IFormatProvider.GetFormat(Type?)"/>
        ///     method instantiates and returns the <see cref="NumberFormatInfo"/> object that provides formatting information.</description></item>
        /// </list>
        /// If <paramref name="provider"/> is <c>null</c> or a <see cref="NumberFormatInfo"/> object cannot be obtained, the <see cref="NumberFormatInfo"/>
        /// object for the current culture is used.
        /// <para/>
        /// If <paramref name="s"/> is out of range of the <see cref="double"/> data type, the method returns <see cref="double.NegativeInfinity"/> if
        /// <paramref name="s"/> is less than <see cref="double.MinValue"/> and <see cref="double.PositiveInfinity"/> if <paramref name="s"/> is greater
        /// than <see cref="double.MaxValue"/>.
        /// <para/>
        /// If a separator is encountered in the <paramref name="s"/> parameter during a parse operation, and the applicable currency or number
        /// decimal and group separators are the same, the parse operation assumes that the separator is a decimal separator rather than a group
        /// separator. For more information about separators, see <see cref="NumberFormatInfo.CurrencyDecimalSeparator"/>,
        /// <see cref="NumberFormatInfo.NumberDecimalSeparator"/>, <see cref="NumberFormatInfo.CurrencyGroupSeparator"/>, and
        /// <see cref="NumberFormatInfo.NumberGroupSeparator"/>.
        /// </remarks>
        public static bool TryParse([NotNullWhen(true)] string? s, NumberStyle style, IFormatProvider? provider, out double result)
        {
            NumberStyleExtensions.ValidateParseStyleFloatingPoint(style);

            if (s == null)
            {
                result = 0;
                return false;
            }

            return DotNetNumber.TryParseDouble(s, style, NumberFormatInfo.GetInstance(provider), out result);
        }

#if FEATURE_READONLYSPAN
        /// <summary>
        /// Converts the span representation of a number in a specified style and culture-specific format to
        /// its double-precision floating-point number equivalent. A return value indicates whether the
        /// conversion succeeded or failed.
        /// <para/>
        /// Usage Note: To approximately match the behavior of the JDK for decimal numbers, use <see cref="NumberStyle.Float"/> combined with
        /// <see cref="NumberStyle.AllowTypeSpecifier"/> for <paramref name="style"/> and <see cref="NumberFormatInfo.InvariantInfo"/> for <paramref name="provider"/>.
        /// To approximately match the behavior of the JDK for hexadecimal numbers, use <see cref="NumberStyle.HexFloat"/> combined with
        /// <see cref="NumberStyle.AllowTypeSpecifier"/> for <paramref name="style"/> and <see cref="NumberFormatInfo.InvariantInfo"/> for <paramref name="provider"/>.
        /// These options will parse any number that is supported by Java, but the hex specifier, exponent, and type suffix are all optional.
        /// </summary>
        /// <param name="s">A read-only character span that contains the number to convert. The span is interpreted using the style specified
        /// by <paramref name="style"/>.</param>
        /// <param name="style">A bitwise combination of enumeration values that indicates the style elements
        /// that can be present in <paramref name="s"/>. Typical values to specify is <see cref="NumberStyle.Float"/>
        /// combined with <see cref="NumberStyle.AllowThousands"/> or, in the case of hexadecimal numbers,
        /// <see cref="NumberStyle.HexFloat"/>.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information about <paramref name="s"/>.</param>
        /// <param name="result">When this method returns, contains the double-precision floating-point number equivalent to the
        /// numeric value or symbol contained in <paramref name="s"/>, if the conversion succeeded, or zero if the conversion failed.
        /// The conversion fails if the <paramref name="s"/> parameter is <c>null</c> or <see cref="string.Empty"/>, is not in a format
        /// compliant with <paramref name="style"/>, or if <paramref name="style"/> is not a valid combination of <see cref="NumberStyle"/>
        /// enumeration constants. This parameter is passed uninitialized; any value originally supplied in result will be overwritten.</param>
        /// <returns><c>true</c> if <paramref name="s"/> was converted successfully; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="style"/> is not a <see cref="NumberStyle"/> value.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="style"/> is not a combination of <see cref="NumberStyle.AllowHexSpecifier"/>, <see cref="NumberStyle.HexFloat"/>
        /// and <see cref="NumberStyle.AllowTypeSpecifier"/> values.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="style"/> has <see cref="NumberStyle.AllowHexSpecifier"/> and <see cref="NumberStyle.AllowTypeSpecifier"/> values
        /// but not <see cref="NumberStyle.AllowExponent"/>. When type suffix is supplied in a hexadecimal number, exponent (prefixed with a
        /// p or P) is required.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="style"/> is a combination of <see cref="NumberStyle.AllowCurrencySymbol"/> and <see cref="NumberStyle.AllowTypeSpecifier"/>.
        /// </exception>
        /// <remarks>
        /// In .NET Core 3.0 and later, values that are too large to represent are rounded to <see cref="double.PositiveInfinity"/> or
        /// <see cref="double.NegativeInfinity"/> as required by the IEEE 754 specification. In prior versions, including .NET Framework, parsing a
        /// value that was too large to represent resulted in failure. This implementation uses the new parser, but patches it back to .NET Framework 4.0
        /// and upgrades it to support Java's hexadecimal floating point format.
        /// <para/>
        /// This overload differs from the <see cref="Parse(string, NumberStyle, IFormatProvider?)"/> method by returning a Boolean value that indicates
        /// whether the parse operation succeeded instead of returning the parsed numeric value. It eliminates the need to use exception handling to test
        /// for a <see cref="FormatException"/> in the event that <paramref name="s"/> is invalid and cannot be successfully parsed.
        /// <para/>
        /// The <paramref name="style"/> parameter defines the style elements (such as white space, thousands separators, and currency symbols) that
        /// are allowed in the s parameter for the parse operation to succeed. It must be a combination of bit flags from the <see cref="NumberStyle"/> enumeration.
        /// <para/>
        /// The <paramref name="s"/> parameter can contain <see cref="NumberFormatInfo.PositiveInfinitySymbol"/>, <see cref="NumberFormatInfo.NegativeInfinitySymbol"/>, or
        /// <see cref="NumberFormatInfo.NaNSymbol"/> for the culture specified by <paramref name="provider"/>. Depending on the value of <paramref name="style"/>,
        /// it can also take one of the following forms:
        /// <para/>
        /// [ws] [$] [sign][integral-digits,]integral-digits[.[fractional-digits]][E[sign]exponential-digits][type][ws]
        /// <para/>
        /// Or, if <paramref name="style"/> includes <see cref="NumberStyle.AllowHexSpecifier"/>:
        /// <para/>
        /// [ws] [$] [sign][hexdigits,]hexdigits[.[fractional-hexdigits]][P[sign]exponential-digits][type][ws]
        /// <para/>
        /// Elements framed in square brackets ([ and ]) are optional. The following table describes each element.
        /// <para/>
        /// <list type="table">
        ///     <listheader>
        ///         <term>Element</term>
        ///         <term>Description</term>
        ///     </listheader>
        ///     <item>
        ///         <term><i>ws</i></term>
        ///         <term>A series of white-space characters. White space can appear at the beginning of <paramref name="s"/> if style includes the
        ///             <see cref="NumberStyle.AllowLeadingWhite"/> flag, and it can appear at the end of <paramref name="s"/> if style includes the
        ///             <see cref="NumberStyle.AllowTrailingWhite"/> flag.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>$</i></term>
        ///         <term>A culture-specific currency symbol. Its position in the string is defined by the <see cref="NumberFormatInfo.CurrencyNegativePattern"/>
        ///             and <see cref="NumberFormatInfo.CurrencyPositivePattern"/> properties of the <see cref="NumberFormatInfo"/> object returned by the
        ///             <see cref="IFormatProvider.GetFormat(Type?)"/> method of the <paramref name="provider"/> parameter. The current culture's currency symbol can
        ///             appear in <paramref name="s"/> if style includes the <see cref="NumberStyle.AllowCurrencySymbol"/> flag.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>sign</i></term>
        ///         <term>A negative sign symbol (-) or a positive sign symbol (+). The sign can appear at the beginning of <paramref name="s"/> if
        ///             <paramref name="style"/> includes the <see cref="NumberStyle.AllowLeadingSign"/> flag, and it can appear at the end of <paramref name="s"/>
        ///             if <paramref name="style"/> includes the <see cref="NumberStyle.AllowTrailingSign"/> flag. Parentheses can be used in <paramref name="s"/>
        ///             to indicate a negative value if <paramref name="style"/> includes the <see cref="NumberStyle.AllowParentheses"/> flag.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>integral-digits</i></term>
        ///         <term>A series of digits ranging from 0 to 9 that specify the integral part of the number. The <i>integral-digits</i> element can be absent
        ///             if the string contains the <i>fractional-digits</i> element.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>hexdigits</i></term>
        ///         <term>A sequence of hexadecimal digits from 0 through f, or 0 through F that specify the integral part of the number. The <i>hexdigits</i>
        ///         element can be absent if the string contains the <i>fractional-hexdigits</i> element. Hexadecimal digits can appear in <paramref name="s"/>
        ///         if <paramref name="style"/> includes the <see cref="NumberStyle.AllowHexSpecifier"/> flag.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>,</i></term>
        ///         <term>A culture-specific thousands separator. The thousands separator of the culture specified by <paramref name="provider"/>
        ///         can appear in <paramref name="s"/> if <paramref name="style"/> includes the <see cref="NumberStyle.AllowThousands"/> flag.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>.</i></term>
        ///         <term>A culture-specific decimal point symbol. The decimal point symbol of the culture specified by <paramref name="provider"/>
        ///         can appear in <paramref name="s"/> if <paramref name="style"/> includes the <see cref="NumberStyle.AllowDecimalPoint"/> flag.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>fractional-digits</i>
        ///         </term>
        ///         <term>A series of digits ranging from 0 to 9 that specify the fractional part of the number. Fractional digits can appear in
        ///         <paramref name="s"/> if <paramref name="style"/> includes the <see cref="NumberStyle.AllowDecimalPoint"/> flag.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>fractional-hexdigits</i>
        ///         </term>
        ///         <term>A sequence of hexadecimal digits from 0 through f, or 0 through F that specify the fractional part of the number.
        ///         Fractional digits can appear in <paramref name="s"/> if <paramref name="style"/> includes both the
        ///         <see cref="NumberStyle.AllowDecimalPoint"/> and <see cref="NumberStyle.AllowHexSpecifier"/> flags.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>e</i></term>
        ///         <term>The 'e' or 'E' character, which indicates that the value is represented in exponential notation. The <paramref name="s"/>
        ///         parameter can represent a number in exponential notation if style includes the <see cref="NumberStyle.AllowExponent"/> flag.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>p</i></term>
        ///         <term>The 'p' or 'P' character, which indicates that the hexadecimal value is represented in exponential notation. The <paramref name="s"/>
        ///         parameter can represent a hexadecimal number in exponential notation if style includes both the <see cref="NumberStyle.AllowExponent"/>
        ///         and <see cref="NumberStyle.AllowHexSpecifier"/> flags.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>exponential-digits</i></term>
        ///         <term>A sequence of decimal digits from 0 through 9 that specify an exponent.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>type</i></term>
        ///         <term>The 'f', 'F', 'd', 'D', 'm' or 'M' character, which is the
        ///         [real type suffix](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/lexical-structure#real-literals)
        ///         of the number as specified in the C# language specification. The type suffix can appear in <paramref name="s"/> if <paramref name="style"/>
        ///         includes the <see cref="NumberStyle.AllowTypeSpecifier"/> flag. If <paramref name="style"/> includes the <see cref="NumberStyle.AllowHexSpecifier"/>
        ///         flag, specifying the <see cref="NumberStyle.AllowExponent"/> is required. Including an exponent in the string (prefixed with 'p' or 'P') is
        ///         required for the type suffix characters 'f', 'F', 'd', and 'D' since they would otherwise be interpreted as hexadecimal integral or fractional digits.</term>
        ///     </item>
        /// </list>
        /// <para/>
        /// NOTE: Any terminating NUL (U+0000) characters in <paramref name="s"/> are ignored by the parsing operation, regardless of
        /// the value of the <paramref name="style"/> argument.
        /// <para/>
        /// A string with decimal digits only (which corresponds to the <see cref="NumberStyle.None"/> flag) always parses successfully.
        /// Most of the remaining <see cref="NumberStyle"/> members control elements that may be but are not required to be present in
        /// this input string. The following table indicates how individual <see cref="NumberStyle"/> members affect the elements that may
        /// be present in <paramref name="s"/>.
        /// <list type="table">
        ///     <listheader>
        ///         <term>Non-composite NumberStyle values</term>
        ///         <term>Elements permitted in <paramref name="s"/> in addition to digits</term>
        ///     </listheader>
        ///     <item>
        ///         <term><see cref="NumberStyle.None"/></term>
        ///         <term>Decimal digits only.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowDecimalPoint"/></term>
        ///         <term>The decimal point (.) and <i>fractional-digits</i> elements. However, <i>fractional-digits</i> must consist
        ///         of only one or more 0 digits or the method returns <c>false</c>.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowExponent"/></term>
        ///         <term>The <paramref name="s"/> parameter can also use exponential notation. If <paramref name="s"/> represents a
        ///         number in exponential notation, it must represent an integer within the range of the <see cref="long"/> data type
        ///         without a non-zero, fractional component.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowLeadingWhite"/></term>
        ///         <term>The <i>ws</i> element at the beginning of <paramref name="s"/>.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowTrailingWhite"/></term>
        ///         <term>The <i>ws</i> element at the end of <paramref name="s"/>.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowLeadingSign"/></term>
        ///         <term>A sign can appear before <i>digits</i>.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowTrailingSign"/></term>
        ///         <term>A sign can appear after <i>digits</i>.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowParentheses"/></term>
        ///         <term>The <i>sign</i> element in the form of parentheses enclosing the numeric value.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowThousands"/></term>
        ///         <term>The thousands separator (,) element.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowCurrencySymbol"/></term>
        ///         <term>The <i>$</i> element.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowTypeSpecifier"/></term>
        ///         <term>The literal <i>type</i> suffix used in the literal identifier syntax of C# or Java.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.Currency"/></term>
        ///         <term>All elements. The <paramref name="s"/> parameter cannot represent a hexadecimal
        ///         number or a number in exponential notation (prefixed with an 'e' or 'E').</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.Float"/></term>
        ///         <term>The <i>ws</i> element at the beginning or end of <paramref name="s"/>, sign at
        ///         the beginning of <paramref name="s"/>, and the decimal point (.) symbol. The <paramref name="s"/>
        ///         parameter can also use exponential notation (prefixed with an 'e' or 'E').</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.HexFloat"/></term>
        ///         <term>The <i>ws</i> element at the beginning or end of <paramref name="s"/>, sign at
        ///         the beginning of <paramref name="s"/>, and the decimal point (.) symbol. The <paramref name="s"/>
        ///         parameter can also use exponential notation (prefixed with a 'p' or 'P').</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.Number"/></term>
        ///         <term>The <i>ws</i>, <i>sign</i>, thousands separator (,), and decimal point (.) elements.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.Any"/></term>
        ///         <term>All styles, except <paramref name="s"/> cannot represent a hexadecimal number.</term>
        ///     </item>
        /// </list>
        /// <para/>
        /// The <see cref="NumberStyle"/> enum can be converted to the .NET <see cref="NumberStyles"/> enum by using the
        /// <see cref="NumberStyleExtensions.ToNumberStyles(NumberStyle)"/> extension method.
        /// Similarly, <see cref="NumberStyles"/> enum can be converted to the J2N <see cref="NumberStyle"/> enum by using
        /// the <see cref="NumberStyleExtensions.ToNumberStyle(NumberStyles)"/> extension method.
        /// <para/>
        /// If the <see cref="NumberStyle.AllowHexSpecifier"/> flag is used, <paramref name="s"/> must be a hexadecimal value which may include an optional 0x or 0X prefix.
        /// For example, "0xC9AF3.8BD4C". This flag may be combined with any of the flags specified in <see cref="NumberStyle.Float"/> and also
        /// <see cref="NumberStyle.AllowTypeSpecifier"/>. (The <see cref="NumberStyle"/> enumeration has a composite style, <see cref="NumberStyle.HexFloat"/>,
        /// that includes all of those flags except <see cref="NumberStyle.AllowTypeSpecifier"/>.)
        /// <para/>
        /// The <paramref name="provider"/> parameter is an <see cref="IFormatProvider"/> implementation, such as a <see cref="CultureInfo"/> object,
        /// a <see cref="NumberFormatInfo"/> object or a <see cref="J2N.Text.StringFormatter"/> object, whose <see cref="IFormatProvider.GetFormat(Type?)"/>
        /// method returns a <see cref="NumberFormatInfo"/> object. The <see cref="NumberFormatInfo"/> object provides culture-specific information about
        /// the format of <paramref name="s"/>. When the <see cref="Parse(string, IFormatProvider)"/> method is invoked, it calls the provider parameter's
        /// <see cref="IFormatProvider.GetFormat(Type?)"/> method and passes it a <see cref="Type"/> object that represents the <see cref="NumberFormatInfo"/> type.
        /// The <see cref="IFormatProvider.GetFormat(Type?)"/> method then returns the <see cref="NumberFormatInfo"/> object that provides information about the
        /// format of the <paramref name="s"/> parameter. There are three ways to use the <paramref name="provider"/> parameter to supply custom formatting
        /// information to the parse operation:
        /// <list type="bullet">
        ///     <item><description>You can pass a <see cref="CultureInfo"/> object that represents the culture that supplies formatting information. Its
        ///     <see cref="IFormatProvider.GetFormat(Type?)"/> method returns the <see cref="NumberFormatInfo"/> object that provides numeric formatting
        ///     information for that culture.</description></item>
        ///     <item><description>You can pass the actual <see cref="NumberFormatInfo"/> object that provides numeric formatting information. (Its
        ///     implementation of <see cref="IFormatProvider.GetFormat(Type?)"/> just returns itself.)</description></item>
        ///     <item><description>You can pass a custom object that implements <see cref="IFormatProvider"/>. Its <see cref="IFormatProvider.GetFormat(Type?)"/>
        ///     method instantiates and returns the <see cref="NumberFormatInfo"/> object that provides formatting information.</description></item>
        /// </list>
        /// If <paramref name="provider"/> is <c>null</c> or a <see cref="NumberFormatInfo"/> object cannot be obtained, the <see cref="NumberFormatInfo"/>
        /// object for the current culture is used.
        /// <para/>
        /// If <paramref name="s"/> is out of range of the <see cref="double"/> data type, the method returns <see cref="double.NegativeInfinity"/> if
        /// <paramref name="s"/> is less than <see cref="double.MinValue"/> and <see cref="double.PositiveInfinity"/> if <paramref name="s"/> is greater
        /// than <see cref="double.MaxValue"/>.
        /// <para/>
        /// If a separator is encountered in the <paramref name="s"/> parameter during a parse operation, and the applicable currency or number
        /// decimal and group separators are the same, the parse operation assumes that the separator is a decimal separator rather than a group
        /// separator. For more information about separators, see <see cref="NumberFormatInfo.CurrencyDecimalSeparator"/>,
        /// <see cref="NumberFormatInfo.NumberDecimalSeparator"/>, <see cref="NumberFormatInfo.CurrencyGroupSeparator"/>, and
        /// <see cref="NumberFormatInfo.NumberGroupSeparator"/>.
        /// </remarks>
        public static bool TryParse(ReadOnlySpan<char> s, NumberStyle style, IFormatProvider? provider, out double result)
        {
            NumberStyleExtensions.ValidateParseStyleFloatingPoint(style);
            return DotNetNumber.TryParseDouble(s, style, NumberFormatInfo.GetInstance(provider), out result);
        }
#endif

        #endregion TryParse_CharSequence_NumberStyle_IFormatProvider_Double

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
            provider ??= CultureInfo.CurrentCulture;

            if (CultureInfo.InvariantCulture.NumberFormat.Equals(provider.GetFormat(typeof(NumberFormatInfo))))
            {
                //return FloatingDecimal.ToJavaFormatString(d); // J2N TODO: Culture
                return RyuDouble.DoubleToString(d, RoundingMode.Conservative); // J2N: Conservative rounding is closer to the JDK
            }

            return d.ToString(provider);
            //return org.apache.harmony.luni.util.NumberConverter.convert(d);
        }

        ///**
        // * Parses the specified string as a double value.
        // * 
        // * @param string
        // *            the string representation of a double value.
        // * @return a {@code Double} instance containing the double value represented
        // *         by {@code string}.
        // * @throws NumberFormatException
        // *             if {@code string} is {@code null}, has a length of zero or
        // *             can not be parsed as a double value.
        // * @see #parseDouble(String)
        // */
        //public static Double ValueOf(string value)
        //{
        //    return new Double(ParseDouble(value));
        //}


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
            return new Double(Parse(value, provider));
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
        public static Double ValueOf(string value, NumberStyle style, IFormatProvider? provider)
        {
            return new Double(Parse(value, style, provider));
        }

        /// <summary>
        /// Compares the two specified <see cref="double"/> values. There are two special cases:
        /// <list type="table">
        ///     <item><description><see cref="double.NaN"/> is equal to <see cref="double.NaN"/> and it is greater
        ///     than any other double value, including <see cref="double.PositiveInfinity"/></description></item>
        ///     <item><description>+0.0d (positive zero) is greater than -0.0d (negative zero).</description></item>
        /// </list>
        /// </summary>
        /// <param name="doubleA">The first value to compare.</param>
        /// <param name="doubleB">The second value to compare.</param>
        /// <returns>
        /// A 32-bit signed integer that indicates the relationship between the two comparands.
        /// <list type="table">
        ///     <listheader>
        ///         <term>Return Value</term>
        ///         <term>Description </term>
        ///     </listheader>
        ///     <item>
        ///         <term>Less than zero</term>
        ///         <term><paramref name="doubleA"/> is less than <paramref name="doubleB"/>.</term>
        ///     </item>
        ///     <item>
        ///         <term>Zero</term>
        ///         <term><paramref name="doubleA"/> equal to <paramref name="doubleB"/>.</term>
        ///     </item>
        ///     <item>
        ///         <term>Greater than zero</term>
        ///         <term><paramref name="doubleA"/> is greater than <paramref name="doubleB"/>.</term>
        ///     </item>
        /// </list>
        /// </returns>
        public static int Compare(double doubleA, double doubleB)
        {
            return JCG.Comparer<double>.Default.Compare(doubleA, doubleB);
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
