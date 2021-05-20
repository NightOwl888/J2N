using J2N.Globalization;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace J2N.Numerics
{
    /// <summary>
    /// The wrapper for the primitive type <see cref="byte"/>.
    /// </summary>
    public sealed class Byte : Number, IComparable<Byte>
    {
        /// <summary>
        /// The value which the receiver represents.
        /// </summary>
        private readonly byte value;


        ///**
        // * The number of bits needed to represent a {@code Byte} value in two's
        // * complement form.
        // * 
        // * @since 1.5
        // */
        //public static final int SIZE = 8;

        //    /**
        //     * The {@link Class} object that represents the primitive type {@code byte}.
        //     */
        //    @SuppressWarnings("unchecked")
        //public static final Class<Byte> TYPE = (Class<Byte>)new byte[0].getClass()
        //        .getComponentType();

        // Note: This can't be set to "byte.class", since *that* is
        // defined to be "java.lang.Byte.TYPE";

        /**
         * A cache of instances used by {@link #valueOf(byte)} and auto-boxing.
         */
        private static readonly Byte[] Cache = new Byte[256];

        /**
         * Constructs a new {@code Byte} with the specified primitive byte value.
         * 
         * @param value
         *            the primitive byte value to store in the new instance.
         */
        public Byte(byte value)
        {
            this.value = value;
        }

        ///**
        // * Constructs a new {@code Byte} from the specified string.
        // * 
        // * @param string
        // *            the string representation of a single byte value.
        // * @throws NumberFormatException
        // *             if {@code string} can not be decoded into a byte value.
        // * @see #parseByte(String)
        // */
        //public Byte(string s)
        //    : this(Parse(s))
        //{
        //}

        /**
         * Constructs a new {@code Byte} from the specified string.
         * 
         * @param string
         *            the string representation of a single byte value.
         * @throws NumberFormatException
         *             if {@code string} can not be decoded into a byte value.
         * @see #parseByte(String)
         */
        public Byte(string s, IFormatProvider? provider)
            : this(Parse(s, provider))
        {
        }

        /**
         * Constructs a new {@code Byte} from the specified string.
         * 
         * @param string
         *            the string representation of a single byte value.
         * @throws NumberFormatException
         *             if {@code string} can not be decoded into a byte value.
         * @see #parseByte(String)
         */
        public Byte(string s, NumberStyle style, IFormatProvider? provider)
            : this(Parse(s, style, provider))
        {
        }

        /**
         * Gets the primitive value of this byte.
         * 
         * @return this object's primitive value.
         */
        public override byte GetByteValue()
        {
            return value;
        }

        /**
         * Compares this object to the specified byte object to determine their
         * relative order.
         * 
         * @param object
         *            the byte object to compare this object to.
         * @return a negative value if the value of this byte is less than the value
         *         of {@code object}; 0 if the value of this byte and the value of
         *         {@code object} are equal; a positive value if the value of this
         *         byte is greater than the value of {@code object}.
         * @see java.lang.Comparable
         * @since 1.2
         */
        public int CompareTo(Byte? other)
        {
            if (other is null) return 1; // Using 1 if other is null as specified here: https://stackoverflow.com/a/4852537
            return value.CompareTo(other.value);
            //return value > other.value ? 1 : (value < other.value ? -1 : 0);
        }

        /**
         * Parses the specified string and returns a {@code Byte} instance if the
         * string can be decoded into a single byte value. The string may be an
         * optional minus sign "-" followed by a hexadecimal ("0x..." or "#..."),
         * octal ("0..."), or decimal ("...") representation of a byte.
         * 
         * @param string
         *            a string representation of a single byte value.
         * @return a {@code Byte} containing the value represented by {@code string}.
         * @throws NumberFormatException
         *             if {@code string} can not be parsed as a byte value.
         */
        public static Byte Decode(string stringValue)
        {
            int intValue = Int32.Decode(stringValue).GetInt32Value();
            byte result = (byte)intValue;
            if (result == intValue || (sbyte)result == intValue)
            {
                return ValueOf(result);
            }
            throw new FormatException();
        }

        /// <inheritdoc/>
        public override double GetDoubleValue()
        {
            return value;
        }

        /**
         * Compares this object with the specified object and indicates if they are
         * equal. In order to be equal, {@code object} must be an instance of
         * {@code Byte} and have the same byte value as this object.
         * 
         * @param object
         *            the object to compare this byte with.
         * @return {@code true} if the specified object is equal to this
         *         {@code Byte}; {@code false} otherwise.
         */

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            if (obj is null) return false;

            return ReferenceEquals(obj, this) || (obj is Byte other) // TODO: Check this logic
                        && (value == other.value);
        }

        /// <inheritdoc/>
        public override float GetSingleValue()
        {
            return value;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return value;
        }

        /// <inheritdoc/>
        public override int GetInt32Value()
        {
            return value;
        }

        /// <inheritdoc/>
        public override long GetInt64Value()
        {
            return value;
        }

        ///**
        // * Parses the specified string as a signed decimal byte value. The ASCII
        // * character \u002d ('-') is recognized as the minus sign.
        // * 
        // * @param string
        // *            the string representation of a single byte value.
        // * @return the primitive byte value represented by {@code string}.
        // * @throws NumberFormatException
        // *             if {@code string} is {@code null}, has a length of zero or
        // *             can not be parsed as a byte value.
        // */
        //public static byte Parse(string value) // J2N: Renamed from ParseByte()
        //{
        //    //int intValue = Convert.ToInt32(value, 10); //Int32.ParseInt32(value);
        //    int intValue = Int32.Parse(value);
        //    byte result = (byte)intValue;
        //    if (result == intValue || (sbyte)result == intValue) // J2N: Allow negative sbyte values for compatibility, even though we return byte rather than sbyte
        //    {
        //        return result;
        //    }
        //    throw new FormatException();
        //}

        /**
         * Parses the specified string as a signed decimal byte value. The ASCII
         * character \u002d ('-') is recognized as the minus sign.
         * 
         * @param string
         *            the string representation of a single byte value.
         * @return the primitive byte value represented by {@code string}.
         * @throws NumberFormatException
         *             if {@code string} is {@code null}, has a length of zero or
         *             can not be parsed as a byte value.
         */
        public static byte Parse(string value, IFormatProvider? provider) // J2N: Renamed from ParseByte()
        {
            return Parse(value, NumberStyle.Integer, provider);
        }

        /**
         * Parses the specified string as a signed decimal byte value. The ASCII
         * character \u002d ('-') is recognized as the minus sign.
         * 
         * @param string
         *            the string representation of a single byte value.
         * @return the primitive byte value represented by {@code string}.
         * @throws NumberFormatException
         *             if {@code string} is {@code null}, has a length of zero or
         *             can not be parsed as a byte value.
         */
        public static byte Parse(string value, NumberStyle style, IFormatProvider? provider) // J2N: Renamed from ParseByte()
        {
            //int intValue = Convert.ToInt32(value, 10); //Int32.ParseInt32(value);
            int intValue = Int32.Parse(value, style, provider);
            byte result = (byte)intValue;
            if (result == intValue || (sbyte)result == intValue) // J2N: Allow negative sbyte values for compatibility, even though we return byte rather than sbyte
            {
                return result;
            }
            throw new FormatException();
        }

        /**
         * Parses the specified string as a signed byte value using the specified
         * radix. The ASCII character \u002d ('-') is recognized as the minus sign.
         * 
         * @param string
         *            the string representation of a single byte value.
         * @param radix
         *            the radix to use when parsing.
         * @return the primitive byte value represented by {@code string} using
         *         {@code radix}.
         * @throws NumberFormatException
         *             if {@code string} is {@code null} or has a length of zero,
         *             {@code radix < Character.MIN_RADIX},
         *             {@code radix > Character.MAX_RADIX}, or if {@code string}
         *             can not be parsed as a byte value.
         */
        public static byte Parse(string value, int radix) // J2N: Renamed from ParseByte()
        {
            int intValue = Int32.Parse(value, radix);
            byte result = (byte)intValue;
            if (result == intValue || (sbyte)result == intValue) // J2N: Allow negative sbyte values for compatibility, even though we return byte rather than sbyte
            {
                return result;
            }
            throw new FormatException();
        }

        /// <summary>
        /// Converts the span representation of a number in a specified style and culture-specific format to its 8-bit signed
        /// integer equivalent. A return value indicates whether the conversion succeeded.
        /// <para/>
        /// Usage Note: When porting from Java, note that this is a culture-sensitive method. Java uses the invariant
        /// culture in the Integer class, so call <see cref="TryParse(ReadOnlySpan{char}, NumberStyle, IFormatProvider?, out byte)"/>
        /// and pass <see cref="NumberStyle.AllowLeadingSign"/> and <see cref="NumberFormatInfo.InvariantInfo"/> to exactly match the Java parsing behavior.
        /// <para/>
        /// Using that overload also allows you to normalize the exception behavior. The <see cref="Parse(string, IFormatProvider?)"/>
        /// method may throw <see cref="ArgumentNullException"/>, <see cref="OverflowException"/>, or <see cref="FormatException"/>,
        /// but Java only ever throws NumberFormatException in any of those cases. Using <see cref="TryParse(ReadOnlySpan{char}, NumberStyle, IFormatProvider?, out byte)"/>
        /// means a <see cref="FormatException"/> can be thrown in any of those error condtions when it returns <c>false</c>.
        /// </summary>
        /// <param name="s">A span containing the characters that represent the number to convert. For compatibility with Java's byte (which is an 8-bit signed byte) the
        /// value may be from <c>"-127"</c> to <c>"255"</c>. Negative numbers in this range will overflow and become high-order positive numbers.</param>
        /// <param name="result">When this method returns, contains the 8-bit signed integer value equivalent of
        /// the number contained in <paramref name="s"/>, if the conversion succeeded, or zero if the conversion failed.
        /// The conversion fails if the s parameter is <c>null</c> or <see cref="string.Empty"/>, is not of the correct format,
        /// or represents a number less than <see cref="byte.MinValue"/> or greater than <see cref="byte.MaxValue"/>. This parameter
        /// is passed uninitialized; any value originally supplied in <paramref name="result"/> will be overwritten.</param>
        /// <returns><c>true</c> if <paramref name="s"/> was converted successfully; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// The <see cref="TryParse(ReadOnlySpan{char}, out byte)"/> method is like the <see cref="Parse(ReadOnlySpan{char}, IFormatProvider?)"/> method,
        /// except the <see cref="TryParse(ReadOnlySpan{char}, out byte)"/> method does not throw an exception if the
        /// conversion fails. It eliminates the need to use exception handling to test for a <see cref="FormatException"/>
        /// in the event that <paramref name="s"/> is invalid and cannot be successfully parsed.
        /// <para/>
        /// The <paramref name="s"/> parameter contains a number of the form:
        /// <code>
        /// [ws][sign]digits[ws]
        /// </code>
        /// Items in square brackets ([ and ]) are optional. The following table describes each element.
        /// <list type="table">
        ///     <listheader>
        ///         <term>Element</term>
        ///         <term>Description</term>
        ///     </listheader>
        ///     <item>
        ///         <term><i>ws</i></term>
        ///         <term>Optional white space.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>sign</i></term>
        ///         <term>An optional sign.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>digits</i></term>
        ///         <term>A sequence of digits ranging from 0 to 9.</term>
        ///     </item>
        /// </list>
        /// <para/>
        /// The <paramref name="s"/> parameter is interpreted using the <see cref="NumberStyle.Integer"/> style.
        /// In addition to the decimal digits, only leading and trailing spaces together with a leading sign are
        /// allowed. To explicitly define the style elements together with the culture-specific formatting
        /// information that can be present in <paramref name="s"/>, use the <see cref="TryParse(ReadOnlySpan{char}, NumberStyle, IFormatProvider?, out byte)"/> method.
        /// <para/>
        /// The <paramref name="s"/> parameter is parsed using the formatting information in a <see cref="NumberFormatInfo"/>
        /// object initialized for the current system culture. For more information, see <see cref="NumberFormatInfo.CurrentInfo"/>.
        /// <para/>
        /// This overload of the <see cref="TryParse(string?, out byte)"/> method interprets all digits in the <paramref name="s"/> parameter
        /// as decimal digits. To parse the string representation of a hexadecimal number, call the
        /// <see cref="TryParse(string?, NumberStyle, IFormatProvider?, out byte)"/> overload.
        /// </remarks>
        /// <seealso cref="Parse(string, IFormatProvider?)"/>
        /// <seealso cref="TryParse(string?, NumberStyle, IFormatProvider?, out byte)"/>
        /// <seealso cref="Number.ToString()"/>
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif 
        public static bool TryParse([NotNullWhen(true)] string? s, out byte result) // Culture Sensitive!
        {
            return TryParse(s, NumberStyle.Integer, NumberFormatInfo.CurrentInfo, out result);
        }

#if FEATURE_READONLYSPAN
        /// <summary>
        /// Converts the span representation of a number in a specified style and culture-specific format to its 8-bit signed
        /// integer equivalent. A return value indicates whether the conversion succeeded.
        /// <para/>
        /// Usage Note: When porting from Java, note that this is a culture-sensitive method. Java uses the invariant
        /// culture in the Integer class, so call <see cref="TryParse(ReadOnlySpan{char}, NumberStyle, IFormatProvider?, out byte)"/>
        /// and pass <see cref="NumberStyle.AllowLeadingSign"/> and <see cref="NumberFormatInfo.InvariantInfo"/> to exactly match the Java parsing behavior.
        /// <para/>
        /// Using that overload also allows you to normalize the exception behavior. The <see cref="Parse(string, IFormatProvider?)"/>
        /// method may throw <see cref="ArgumentNullException"/>, <see cref="OverflowException"/>, or <see cref="FormatException"/>,
        /// but Java only ever throws NumberFormatException in any of those cases. Using <see cref="TryParse(ReadOnlySpan{char}, NumberStyle, IFormatProvider?, out byte)"/>
        /// means a <see cref="FormatException"/> can be thrown in any of those error condtions when it returns <c>false</c>.
        /// </summary>
        /// <param name="s">A span containing the characters that represent the number to convert. For compatibility with Java's byte (which is an 8-bit signed byte) the
        /// value may be from <c>"-127"</c> to <c>"255"</c>. Negative numbers in this range will overflow and become high-order positive numbers.</param>
        /// <param name="result">When this method returns, contains the 8-bit signed integer value equivalent of
        /// the number contained in <paramref name="s"/>, if the conversion succeeded, or zero if the conversion failed.
        /// The conversion fails if the s parameter is <c>null</c> or <see cref="string.Empty"/>, is not of the correct format,
        /// or represents a number less than <see cref="byte.MinValue"/> or greater than <see cref="byte.MaxValue"/>. This parameter
        /// is passed uninitialized; any value originally supplied in <paramref name="result"/> will be overwritten.</param>
        /// <returns><c>true</c> if <paramref name="s"/> was converted successfully; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// The <see cref="TryParse(ReadOnlySpan{char}, out byte)"/> method is like the <see cref="Parse(ReadOnlySpan{char}, IFormatProvider?)"/> method,
        /// except the <see cref="TryParse(ReadOnlySpan{char}, out byte)"/> method does not throw an exception if the
        /// conversion fails. It eliminates the need to use exception handling to test for a <see cref="FormatException"/>
        /// in the event that <paramref name="s"/> is invalid and cannot be successfully parsed.
        /// <para/>
        /// The <paramref name="s"/> parameter contains a number of the form:
        /// <code>
        /// [ws][sign]digits[ws]
        /// </code>
        /// Items in square brackets ([ and ]) are optional. The following table describes each element.
        /// <list type="table">
        ///     <listheader>
        ///         <term>Element</term>
        ///         <term>Description</term>
        ///     </listheader>
        ///     <item>
        ///         <term><i>ws</i></term>
        ///         <term>Optional white space.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>sign</i></term>
        ///         <term>An optional sign.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>digits</i></term>
        ///         <term>A sequence of digits ranging from 0 to 9.</term>
        ///     </item>
        /// </list>
        /// <para/>
        /// The <paramref name="s"/> parameter is interpreted using the <see cref="NumberStyle.Integer"/> style.
        /// In addition to the decimal digits, only leading and trailing spaces together with a leading sign are
        /// allowed. To explicitly define the style elements together with the culture-specific formatting
        /// information that can be present in <paramref name="s"/>, use the <see cref="TryParse(ReadOnlySpan{char}, NumberStyle, IFormatProvider?, out byte)"/> method.
        /// <para/>
        /// The <paramref name="s"/> parameter is parsed using the formatting information in a <see cref="NumberFormatInfo"/>
        /// object initialized for the current system culture. For more information, see <see cref="NumberFormatInfo.CurrentInfo"/>.
        /// <para/>
        /// This overload of the <see cref="TryParse(ReadOnlySpan{char}, out byte)"/> method interprets all digits in the <paramref name="s"/> parameter
        /// as decimal digits. To parse the string representation of a hexadecimal number, call the
        /// <see cref="TryParse(ReadOnlySpan{char}, NumberStyle, IFormatProvider?, out byte)"/> overload.
        /// </remarks>
        /// <seealso cref="Parse(ReadOnlySpan{char}, IFormatProvider?)"/>
        /// <seealso cref="TryParse(string?, out byte)"/>
        /// <seealso cref="Number.ToString()"/>
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif 
        public static bool TryParse(ReadOnlySpan<char> s, out byte result) // Culture Sensitive!
        {
            return TryParse(s, NumberStyle.Integer, NumberFormatInfo.CurrentInfo, out result);
        }
#endif

        /// <summary>
        /// Converts the span representation of a number in a specified style and culture-specific format to its 8-bit signed integer equivalent.
        /// A return value indicates whether the conversion succeeded.
        /// <para/>
        /// Usage Note: To exactly match Java, use <see cref="NumberStyle.AllowLeadingSign"/> for <paramref name="style"/> and
        /// <see cref="NumberFormatInfo.InvariantInfo"/> for <paramref name="provider"/>. We recommend factoring out
        /// exceptions when parsing, but if the Java code depends on exceptions, throw <see cref="FormatException"/>
        /// when this method returns <c>false</c>.
        /// </summary>
        /// <param name="s">A span containing the characters that represent the number to convert. The span is interpreted using
        /// the style specified by <paramref name="style"/>. For compatibility with Java's byte (which is an 8-bit signed byte) the
        /// value may be from <c>"-127"</c> to <c>"255"</c>. Negative numbers in this range will overflow and become high-order positive numbers.</param>
        /// <param name="style">A bitwise combination of enumeration values that indicates the style elements that can be present in <paramref name="s"/>.
        /// A typical value to specify is <see cref="NumberStyle.Integer"/>.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information about <paramref name="s"/>.</param>
        /// <param name="result">When this method returns, contains the 8-bit signed integer value equivalent of
        /// the number contained in <paramref name="s"/>, if the conversion succeeded, or zero if the conversion failed.
        /// The conversion fails if the s parameter is <c>null</c> or <see cref="string.Empty"/>, is not of the correct format,
        /// or represents a number less than <see cref="byte.MinValue"/> or greater than <see cref="byte.MaxValue"/>. This parameter
        /// is passed uninitialized; any value originally supplied in <paramref name="result"/> will be overwritten.</param>
        /// <returns><c>true</c> if <paramref name="s"/> was converted successfully; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="style"/> is not a <see cref="NumberStyle"/> value.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="style"/> is not a combination of <see cref="NumberStyle.AllowHexSpecifier"/> and <see cref="NumberStyle.HexNumber"/> values.
        /// </exception>
        /// <remarks>
        /// The <see cref="TryParse(ReadOnlySpan{char}, NumberStyle, IFormatProvider?, out byte)"/> method is like the <see cref="Parse(ReadOnlySpan{char}, NumberStyle, IFormatProvider?)"/> method,
        /// except the <see cref="TryParse(ReadOnlySpan{char}, NumberStyle, IFormatProvider?, out byte)"/> method does not throw an exception if the
        /// conversion fails. It eliminates the need to use exception handling to test for a <see cref="FormatException"/>
        /// in the event that <paramref name="s"/> is invalid and cannot be successfully parsed.
        /// <para/>
        /// The <paramref name="style"/> parameter defines the style elements (such as white space or a positive or negative sign)
        /// that are allowed in the <paramref name="s"/> parameter for the parse operation to succeed. It must be a combination of
        /// bit flags from the <see cref="NumberStyle"/> enumeration. Depending on the value of <paramref name="style"/>, the <paramref name="s"/>
        /// parameter may include the following elements:
        /// <code>
        /// [ws][$][sign][digits,]digits[.fractional_digits][e[sign]digits][ws]
        /// </code>
        /// Or, if the <paramref name="style"/> parameter includes <see cref="NumberStyle.AllowHexSpecifier"/>:
        /// <code>
        /// [ws]hexdigits[ws]
        /// </code>
        /// Items in square brackets ([ and ]) are optional. The following table describes each element.
        /// <list type="table">
        ///     <listheader>
        ///         <term>Element</term>
        ///         <term>Description</term>
        ///     </listheader>
        ///     <item>
        ///         <term><i>ws</i></term>
        ///         <term>Optional white space.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>$</i></term>
        ///         <term>A culture-specific currency symbol. Its position in the string is defined by the <see cref="NumberFormatInfo.CurrencyPositivePattern"/>
        ///         property of the <see cref="NumberFormatInfo"/> object returned by the <see cref="IFormatProvider.GetFormat(Type?)"/> method of the
        ///         <paramref name="provider"/> parameter. The currency symbol can appear in <paramref name="s"/> if style includes the
        ///         <see cref="NumberStyle.AllowCurrencySymbol"/> flag.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>sign</i></term>
        ///         <term>An optional sign.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>digits</i></term>
        ///         <term>A sequence of digits ranging from 0 to 9.</term>
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
        ///         <term><i>fractional_digits</i></term>
        ///         <term>One or more occurrences of the digit 0. Fractional digits can appear in <paramref name="s"/> only if style includes
        ///         the <see cref="NumberStyle.AllowDecimalPoint"/> flag.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>The 'e' or 'E' character, which indicates that the value is represented in exponential notation. The <paramref name="s"/>
        ///         parameter can represent a number in exponential notation if style includes the <see cref="NumberStyle.AllowExponent"/> flag.</i></term>
        ///         <term></term>
        ///     </item>
        ///     <item>
        ///         <term><i>hexdigits</i></term>
        ///         <term>A sequence of hexadecimal digits from 0 through f, or 0 through F.</term>
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
        ///         <term>The decimal point (.) and <i>fractional_digits</i> elements. However, <i>fractional_digits</i> must consist
        ///         of only one or more 0 digits or the method returns <c>false</c>.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowExponent"/></term>
        ///         <term>The <paramref name="s"/> parameter can also use exponential notation. If <paramref name="s"/> represents a
        ///         number in exponential notation, it must represent an integer within the range of the <see cref="byte"/> data type
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
        ///         <term><see cref="NumberStyle.AllowTrailingFloatType"/></term>
        ///         <term>The type suffix used in the literal identifier syntax of C# or Java.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.Currency"/></term>
        ///         <term>All elements. The <paramref name="s"/> parameter cannot represent a hexadecimal
        ///         number or a number in exponential notation.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.Float"/></term>
        ///         <term>The <i>ws</i> element at the beginning or end of <paramref name="s"/>, sign at
        ///         the beginning of <paramref name="s"/>, and the decimal point (.) symbol. The <paramref name="s"/>
        ///         parameter can also use exponential notation.</term>
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
        /// The <see cref="NumberStyle"/> enum is a match in both symbol and value for the .NET <see cref="NumberStyles"/> enum.
        /// Therefore, simply casting the value will convert it properly between the two in both directions.
        /// <para/>
        /// If the <see cref="NumberStyle.AllowHexSpecifier"/> flag is used, s must be a hexadecimal value without a prefix.
        /// For example, "C9AF3" parses successfully, but "0xC9AF3" does not. The only other flags that can be present in style
        /// are <see cref="NumberStyle.AllowLeadingWhite"/> and <see cref="NumberStyle.AllowTrailingWhite"/>. (The <see cref="NumberStyle"/>
        /// enumeration has a composite style, <see cref="NumberStyle.HexNumber"/>, that includes both white space flags.)
        /// <para/>
        /// The <paramref name="provider"/> parameter is an <see cref="IFormatProvider"/> implementation, such as a <see cref="CultureInfo"/> object,
        /// a <see cref="NumberFormatInfo"/> object or a <see cref="J2N.Text.StringFormatter"/> object, whose <see cref="IFormatProvider.GetFormat(Type?)"/>
        /// method returns a <see cref="NumberFormatInfo"/> object. The <see cref="NumberFormatInfo"/> object provides culture-specific information about
        /// the format of <paramref name="s"/>. If <paramref name="provider"/> is <c>null</c>, the <see cref="NumberFormatInfo"/> object for the current
        /// culture is used.
        /// </remarks>
        /// <seealso cref="Parse(string, NumberStyle, IFormatProvider?)"/>
        /// <seealso cref="TryParse(string?, out byte)"/>
        /// <seealso cref="Number.ToString()"/>
        /// <seealso cref="NumberStyle"/>
        public static bool TryParse([NotNullWhen(true)] string? s, NumberStyle style, IFormatProvider? provider, out byte result)
        {
            if (s == null)
            {
                result = 0;
                return false;
            }
#if FEATURE_READONLYSPAN
            return TryParse((ReadOnlySpan<char>)s, style, provider, out result);
#else
            if (Int32.TryParse(s, style, provider, out int intValue))
            {
                result = (byte)intValue;
                if (result == intValue || (sbyte)result == intValue) // J2N: Allow negative sbyte values for compatibility, even though we return byte rather than sbyte
                {
                    return true;
                }
            }
            result = default;
            return false;
#endif
        }

#if FEATURE_READONLYSPAN
        /// <summary>
        /// Converts the span representation of a number in a specified style and culture-specific format to its 8-bit signed integer equivalent.
        /// A return value indicates whether the conversion succeeded.
        /// <para/>
        /// Usage Note: To exactly match Java, use <see cref="NumberStyle.AllowLeadingSign"/> for <paramref name="style"/> and
        /// <see cref="NumberFormatInfo.InvariantInfo"/> for <paramref name="provider"/>. We recommend factoring out
        /// exceptions when parsing, but if the Java code depends on exceptions, throw <see cref="FormatException"/>
        /// when this method returns <c>false</c>.
        /// </summary>
        /// <param name="s">A span containing the characters that represent the number to convert. The span is interpreted using
        /// the style specified by <paramref name="style"/>. For compatibility with Java's byte (which is an 8-bit signed byte) the
        /// value may be from <c>"-127"</c> to <c>"255"</c>. Negative numbers in this range will overflow and become high-order positive numbers.</param>
        /// <param name="style">A bitwise combination of enumeration values that indicates the style elements that can be present in <paramref name="s"/>.
        /// A typical value to specify is <see cref="NumberStyle.Integer"/>.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information about <paramref name="s"/>.</param>
        /// <param name="result">When this method returns, contains the 8-bit signed integer value equivalent of
        /// the number contained in <paramref name="s"/>, if the conversion succeeded, or zero if the conversion failed.
        /// The conversion fails if the s parameter is <c>null</c> or <see cref="string.Empty"/>, is not of the correct format,
        /// or represents a number less than <see cref="sbyte.MinValue"/> or greater than <see cref="byte.MaxValue"/>. This parameter
        /// is passed uninitialized; any value originally supplied in <paramref name="result"/> will be overwritten.</param>
        /// <returns><c>true</c> if <paramref name="s"/> was converted successfully; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="style"/> is not a <see cref="NumberStyle"/> value.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="style"/> is not a combination of <see cref="NumberStyle.AllowHexSpecifier"/> and <see cref="NumberStyle.HexNumber"/> values.
        /// </exception>
        /// <remarks>
        /// The <see cref="TryParse(ReadOnlySpan{char}, NumberStyle, IFormatProvider?, out byte)"/> method is like the <see cref="Parse(ReadOnlySpan{char}, NumberStyle, IFormatProvider?)"/> method,
        /// except the <see cref="TryParse(ReadOnlySpan{char}, NumberStyle, IFormatProvider?, out byte)"/> method does not throw an exception if the
        /// conversion fails. It eliminates the need to use exception handling to test for a <see cref="FormatException"/>
        /// in the event that <paramref name="s"/> is invalid and cannot be successfully parsed.
        /// <para/>
        /// The <paramref name="style"/> parameter defines the style elements (such as white space or a positive or negative sign)
        /// that are allowed in the <paramref name="s"/> parameter for the parse operation to succeed. It must be a combination of
        /// bit flags from the <see cref="NumberStyle"/> enumeration. Depending on the value of <paramref name="style"/>, the <paramref name="s"/>
        /// parameter may include the following elements:
        /// <code>
        /// [ws][$][sign][digits,]digits[.fractional_digits][e[sign]digits][ws]
        /// </code>
        /// Or, if the <paramref name="style"/> parameter includes <see cref="NumberStyle.AllowHexSpecifier"/>:
        /// <code>
        /// [ws]hexdigits[ws]
        /// </code>
        /// Items in square brackets ([ and ]) are optional. The following table describes each element.
        /// <list type="table">
        ///     <listheader>
        ///         <term>Element</term>
        ///         <term>Description</term>
        ///     </listheader>
        ///     <item>
        ///         <term><i>ws</i></term>
        ///         <term>Optional white space.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>$</i></term>
        ///         <term>A culture-specific currency symbol. Its position in the string is defined by the <see cref="NumberFormatInfo.CurrencyPositivePattern"/>
        ///         property of the <see cref="NumberFormatInfo"/> object returned by the <see cref="IFormatProvider.GetFormat(Type?)"/> method of the
        ///         <paramref name="provider"/> parameter. The currency symbol can appear in <paramref name="s"/> if style includes the
        ///         <see cref="NumberStyle.AllowCurrencySymbol"/> flag.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>sign</i></term>
        ///         <term>An optional sign.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>digits</i></term>
        ///         <term>A sequence of digits ranging from 0 to 9.</term>
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
        ///         <term><i>fractional_digits</i></term>
        ///         <term>One or more occurrences of the digit 0. Fractional digits can appear in <paramref name="s"/> only if style includes
        ///         the <see cref="NumberStyle.AllowDecimalPoint"/> flag.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>The 'e' or 'E' character, which indicates that the value is represented in exponential notation. The <paramref name="s"/>
        ///         parameter can represent a number in exponential notation if style includes the <see cref="NumberStyle.AllowExponent"/> flag.</i></term>
        ///         <term></term>
        ///     </item>
        ///     <item>
        ///         <term><i>hexdigits</i></term>
        ///         <term>A sequence of hexadecimal digits from 0 through f, or 0 through F.</term>
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
        ///         <term>The decimal point (.) and <i>fractional_digits</i> elements. However, <i>fractional_digits</i> must consist
        ///         of only one or more 0 digits or the method returns <c>false</c>.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowExponent"/></term>
        ///         <term>The <paramref name="s"/> parameter can also use exponential notation. If <paramref name="s"/> represents a
        ///         number in exponential notation, it must represent an integer within the range of the <see cref="byte"/> data type
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
        ///         <term><see cref="NumberStyle.AllowTrailingFloatType"/></term>
        ///         <term>The type suffix used in the literal identifier syntax of C# or Java.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.Currency"/></term>
        ///         <term>All elements. The <paramref name="s"/> parameter cannot represent a hexadecimal
        ///         number or a number in exponential notation.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.Float"/></term>
        ///         <term>The <i>ws</i> element at the beginning or end of <paramref name="s"/>, sign at
        ///         the beginning of <paramref name="s"/>, and the decimal point (.) symbol. The <paramref name="s"/>
        ///         parameter can also use exponential notation.</term>
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
        /// The <see cref="NumberStyle"/> enum is a match in both symbol and value for the .NET <see cref="NumberStyles"/> enum.
        /// Therefore, simply casting the value will convert it properly between the two in both directions.
        /// <para/>
        /// If the <see cref="NumberStyle.AllowHexSpecifier"/> flag is used, s must be a hexadecimal value without a prefix.
        /// For example, "C9AF3" parses successfully, but "0xC9AF3" does not. The only other flags that can be present in style
        /// are <see cref="NumberStyle.AllowLeadingWhite"/> and <see cref="NumberStyle.AllowTrailingWhite"/>. (The <see cref="NumberStyle"/>
        /// enumeration has a composite style, <see cref="NumberStyle.HexNumber"/>, that includes both white space flags.)
        /// <para/>
        /// The <paramref name="provider"/> parameter is an <see cref="IFormatProvider"/> implementation, such as a <see cref="CultureInfo"/> object,
        /// a <see cref="NumberFormatInfo"/> object or a <see cref="J2N.Text.StringFormatter"/> object, whose <see cref="IFormatProvider.GetFormat(Type?)"/>
        /// method returns a <see cref="NumberFormatInfo"/> object. The <see cref="NumberFormatInfo"/> object provides culture-specific information about
        /// the format of <paramref name="s"/>. If <paramref name="provider"/> is <c>null</c>, the <see cref="NumberFormatInfo"/> object for the current
        /// culture is used.
        /// </remarks>
        /// <seealso cref="Parse(ReadOnlySpan{char}, NumberStyle, IFormatProvider?)"/>
        /// <seealso cref="TryParse(ReadOnlySpan{char}, out byte)"/>
        /// <seealso cref="Number.ToString()"/>
        /// <seealso cref="NumberStyle"/>
        public static bool TryParse(ReadOnlySpan<char> s, NumberStyle style, IFormatProvider? provider, out byte result)
        {
            if (Int32.TryParse(s, style, provider, out int intValue))
            {
                result = (byte)intValue;
                if (result == intValue || (sbyte)result == intValue) // J2N: Allow negative sbyte values for compatibility, even though we return byte rather than sbyte
                {
                    return true;
                }
            }
            result = default;
            return false;
        }
#endif

        /// <inheritdoc/>
        public override short GetInt16Value()
        {
            return value;
        }

        //    public override string ToString()
        //{
        //    return Integer.toString(value);
        //}

        /// <inheritdoc/>
        public override string ToString(string? format, IFormatProvider? provider)
        {
            return ToString(format, provider, value);
        }

        /**
         * Returns a string containing a concise, human-readable description of the
         * specified byte value.
         * 
         * @param value
         *            the byte to convert to a string.
         * @return a printable representation of {@code value}.
         */
        public static string ToString(byte value)
        {
            //return Integer.toString(value);
            return value.ToString(J2N.Text.StringFormatter.CurrentCulture);
        }

        /**
         * Returns a string containing a concise, human-readable description of the
         * specified byte value.
         * 
         * @param value
         *            the byte to convert to a string.
         * @return a printable representation of {@code value}.
         */
        public static string ToString(byte value, IFormatProvider? provider)
        {
            //return Integer.toString(value);
            return value.ToString(provider);
        }

        /////**
        //// * Parses the specified string as a signed decimal byte value.
        //// * 
        //// * @param string
        //// *            the string representation of a single byte value.
        //// * @return a {@code Byte} instance containing the byte value represented by
        //// *         {@code string}.
        //// * @throws NumberFormatException
        //// *             if {@code string} is {@code null}, has a length of zero or
        //// *             can not be parsed as a byte value.
        //// * @see #parseByte(String)
        //// */
        ////public static Byte ValueOf(string value)
        ////{
        ////    return ValueOf(Parse(value));
        ////}

        /**
         * Parses the specified string as a signed decimal byte value.
         * 
         * @param string
         *            the string representation of a single byte value.
         * @return a {@code Byte} instance containing the byte value represented by
         *         {@code string}.
         * @throws NumberFormatException
         *             if {@code string} is {@code null}, has a length of zero or
         *             can not be parsed as a byte value.
         * @see #parseByte(String)
         */
        public static Byte ValueOf(string s, IFormatProvider? provider)
        {
            return ValueOf(Parse(s, provider));
        }

        /**
         * Parses the specified string as a signed decimal byte value.
         * 
         * @param string
         *            the string representation of a single byte value.
         * @return a {@code Byte} instance containing the byte value represented by
         *         {@code string}.
         * @throws NumberFormatException
         *             if {@code string} is {@code null}, has a length of zero or
         *             can not be parsed as a byte value.
         * @see #parseByte(String)
         */
        public static Byte ValueOf(string s, NumberStyle style, IFormatProvider? provider)
        {
            return ValueOf(Parse(s, style, provider));
        }

        /// <summary>
        /// Parses the specified string as a signed or unsigned byte value using the specified
        /// radix.
        /// <para/>
        /// Usage Note: In Java, the range allowed is from -128 to 127, however for compatibility
        /// reasons we have extended the range from -128 to 255. The value of the returned instance
        /// will be from 0 to 255, so if a negative value is required it is up to the user to
        /// cast to <see cref="sbyte"/>.
        /// </summary>
        /// <param name="s">The string representation of a single <see cref="byte"/> or <see cref="sbyte"/> value.</param>
        /// <param name="radix">The radix to use when parsing. This is the same as <c>fromBase</c> in <see cref="Convert.ToInt32(string?, int)"/>,
        /// except the range is expanded from <see cref="Character.MinRadix"/> to <see cref="Character.MaxRadix"/>, whereas <see cref="Convert.ToInt32(string?, int)"/>
        /// only supports 2, 8, 10, or 16.</param>
        /// <returns>A <see cref="Byte"/> instance containing the <see cref="byte"/> value represented by
        /// <paramref name="s"/> using <paramref name="radix"/>.</returns>
        /// <exception cref="FormatException">
        /// <paramref name="s"/> is <c>null</c> or has a length of zero.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="radix"/> is less than <see cref="Character.MinRadix"/> or greater than <see cref="Character.MaxRadix"/>.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="s"/> cannot be parsed as a <see cref="byte"/> or <see cref="sbyte"/> value.
        /// </exception>
        /// <seealso cref="Parse(string, int)"/>
        public static Byte ValueOf(string s, int radix) // J2N TODO: Exception handling - throw ArgumentOutOfRangeException and ArgumentNullException? Accept null like Convert.ToInt32() does?
        {
            return ValueOf(Parse(s, radix));
        }

        /**
         * Returns a {@code Byte} instance for the specified byte value.
         * <p>
         * If it is not necessary to get a new {@code Byte} instance, it is
         * recommended to use this method instead of the constructor, since it
         * maintains a cache of instances which may result in better performance.
         * 
         * @param b
         *            the byte value to store in the instance.
         * @return a {@code Byte} instance containing {@code b}.
         * @since 1.5
         */
        public static Byte ValueOf(byte b)
        {
            lock (Cache)
            {
                int idx = b - byte.MinValue;
                Byte result = Cache[idx];
                return result ?? (Cache[idx] = new Byte(b));
            }
        }



        // J2N: Support implicit conversion

        /// <inheritdoc/>
        public static implicit operator byte(Byte value) => value.value;
        /// <inheritdoc/>
        public static implicit operator Byte(byte value) => ValueOf(value);
    }
}
