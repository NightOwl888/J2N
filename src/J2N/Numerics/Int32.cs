using J2N.Globalization;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace J2N.Numerics
{
    using SR = J2N.Resources.Strings;

    /// <inheritdoc/>
    public sealed class Int32 : Number, IComparable<Int32>
    {
        /**
         * Constant for the number of bits needed to represent a {@code int} in
         * two's complement form.
         *
         * @since 1.5
         */
        public const int SIZE = 32; // J2N: Rename BitCount?

        /**
         * The value which the receiver represents.
         */
        private readonly int value;


        /**
         * Constructs a new {@code Integer} with the specified primitive integer
         * value.
         * 
         * @param value
         *            the primitive integer value to store in the new instance.
         */
        public Int32(int value)
        {
            this.value = value;
        }

        ///**
        // * Constructs a new {@code Integer} from the specified string.
        // * 
        // * @param string
        // *            the string representation of an integer value.
        // * @throws NumberFormatException
        // *             if {@code string} can not be decoded into an integer value.
        // * @see #parseInt(String)
        // */
        //public Int32(string value)
        //    : this(Parse(value))
        //{
        //}

        /**
         * Constructs a new {@code Integer} from the specified string.
         * 
         * @param string
         *            the string representation of an integer value.
         * @throws NumberFormatException
         *             if {@code string} can not be decoded into an integer value.
         * @see #parseInt(String)
         */
        public Int32(string value, IFormatProvider? provider)
            : this(Parse(value, provider))
        {
        }

        /**
         * Constructs a new {@code Integer} from the specified string.
         * 
         * @param string
         *            the string representation of an integer value.
         * @throws NumberFormatException
         *             if {@code string} can not be decoded into an integer value.
         * @see #parseInt(String)
         */
        public Int32(string value, NumberStyle style, IFormatProvider? provider)
            : this(Parse(value, style, provider))
        {
        }

        /// <inheritdoc/>
        public override byte GetByteValue()
        {
            return (byte)value;
        }

        /**
         * Compares this object to the specified integer object to determine their
         * relative order.
         * 
         * @param object
         *            the integer object to compare this object to.
         * @return a negative value if the value of this integer is less than the
         *         value of {@code object}; 0 if the value of this integer and the
         *         value of {@code object} are equal; a positive value if the value
         *         of this integer is greater than the value of {@code object}.
         * @see java.lang.Comparable
         * @since 1.2
         */
        public int CompareTo(Int32? other)
        {
            if (other is null) return 1; // Using 1 if other is null as specified here: https://stackoverflow.com/a/4852537
            return value.CompareTo(other.value);
        }

        /**
         * Parses the specified string and returns a {@code Integer} instance if the
         * string can be decoded into an integer value. The string may be an
         * optional minus sign "-" followed by a hexadecimal ("0x..." or "#..."),
         * octal ("0..."), or decimal ("...") representation of an integer.
         * 
         * @param string
         *            a string representation of an integer value.
         * @return an {@code Integer} containing the value represented by
         *         {@code string}.
         * @throws NumberFormatException
         *             if {@code string} can not be parsed as an integer value.
         */
        public static Int32 Decode(string value)
        {
            if (value is null)
                throw new ArgumentNullException(nameof(value));

            int length = value.Length, i = 0;
            if (length == 0)
            {
                throw new FormatException(); // J2N TODO: Error message
            }
            char firstDigit = value[i];
            int sign = firstDigit == '-' ? -1 : 1;
            if (sign < 0)
            {
                if (length == 1)
                {
                    throw new FormatException(value); // J2N TODO: Error message
                }
                firstDigit = value[++i];
            }

            int @base = 10;
            if (firstDigit == '0')
            {
                if (++i == length)
                {
                    return ValueOf(0);
                }
                if ((firstDigit = value[i]) == 'x' || firstDigit == 'X')
                {
                    if (++i == length)
                    {
                        throw new FormatException(value); // J2N TODO: Error message
                    }
                    @base = 16;
                }
                else
                {
                    @base = 8;
                }
            }
            else if (firstDigit == '#')
            {
                if (++i == length)
                {
                    throw new FormatException(value); // J2N TODO: Error message
                }
                @base = 16;
            }

#if FEATURE_READONLYSPAN
            int result = ParseNumbers.StringToInt(value.AsSpan(), @base, flags: ParseNumbers.IsTight, sign, ref i, value.Length - i);
#else
            int result = Parse(value, i, @base, negative: sign < 0);
#endif

            return ValueOf(result);
        }


        /// <inheritdoc/>
        public override double GetDoubleValue()
        {
            return value;
        }

        /**
         * Compares this instance with the specified object and indicates if they
         * are equal. In order to be equal, {@code o} must be an instance of
         * {@code Integer} and have the same integer value as this object.
         * 
         * @param o
         *            the object to compare this integer with.
         * @return {@code true} if the specified object is equal to this
         *         {@code Integer}; {@code false} otherwise.
         */
        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            if (obj is null) return false;

            return (obj is Int32 other)
                && (value == other.value);
        }

        /// <inheritdoc/>
        public override float GetSingleValue()
        {
            return value;
        }

        // J2N: These access system properties, for which .NET has no equivalent
        /////**
        //// * Returns the {@code Integer} value of the system property identified by
        //// * {@code string}. Returns {@code null} if {@code string} is {@code null}
        //// * or empty, if the property can not be found or if its value can not be
        //// * parsed as an integer.
        //// * 
        //// * @param string
        //// *            the name of the requested system property.
        //// * @return the requested property's value as an {@code Integer} or
        //// *         {@code null}.
        //// */
        ////public static Int32 GetInt32(string value)
        ////{
        ////    if (value == null || string.length() == 0)
        ////    {
        ////        return null;
        ////    }
        ////    String prop = System.getProperty(value);
        ////    if (prop == null)
        ////    {
        ////        return null;
        ////    }
        ////    try
        ////    {
        ////        return decode(prop);
        ////    }
        ////    catch (NumberFormatException ex)
        ////    {
        ////        return null;
        ////    }
        ////}

        /////**
        //// * Returns the {@code Integer} value of the system property identified by
        //// * {@code string}. Returns the specified default value if {@code string} is
        //// * {@code null} or empty, if the property can not be found or if its value
        //// * can not be parsed as an integer.
        //// * 
        //// * @param string
        //// *            the name of the requested system property.
        //// * @param defaultValue
        //// *            the default value that is returned if there is no integer
        //// *            system property with the requested name.
        //// * @return the requested property's value as an {@code Integer} or the
        //// *         default value.
        //// */
        ////public static Integer getInteger(String string, int defaultValue)
        ////{
        ////    if (string == null || string.length() == 0)
        ////    {
        ////        return valueOf(defaultValue);
        ////    }
        ////    String prop = System.getProperty(string);
        ////    if (prop == null)
        ////    {
        ////        return valueOf(defaultValue);
        ////    }
        ////    try
        ////    {
        ////        return decode(prop);
        ////    }
        ////    catch (NumberFormatException ex)
        ////    {
        ////        return valueOf(defaultValue);
        ////    }
        ////}

        /////**
        //// * Returns the {@code Integer} value of the system property identified by
        //// * {@code string}. Returns the specified default value if {@code string} is
        //// * {@code null} or empty, if the property can not be found or if its value
        //// * can not be parsed as an integer.
        //// * 
        //// * @param string
        //// *            the name of the requested system property.
        //// * @param defaultValue
        //// *            the default value that is returned if there is no integer
        //// *            system property with the requested name.
        //// * @return the requested property's value as an {@code Integer} or the
        //// *         default value.
        //// */
        ////public static Integer getInteger(String string, Integer defaultValue)
        ////{
        ////    if (string == null || string.length() == 0)
        ////    {
        ////        return defaultValue;
        ////    }
        ////    String prop = System.getProperty(string);
        ////    if (prop == null)
        ////    {
        ////        return defaultValue;
        ////    }
        ////    try
        ////    {
        ////        return decode(prop);
        ////    }
        ////    catch (NumberFormatException ex)
        ////    {
        ////        return defaultValue;
        ////    }
        ////}


        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return value;
        }

        /**
         * Gets the primitive value of this int.
         * 
         * @return this object's primitive value.
         */
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
        // * Parses the specified string as a signed decimal integer value. The ASCII
        // * character \u002d ('-') is recognized as the minus sign.
        // * 
        // * @param string
        // *            the string representation of an integer value.
        // * @return the primitive integer value represented by {@code string}.
        // * @throws NumberFormatException
        // *             if {@code string} is {@code null}, has a length of zero or
        // *             can not be parsed as an integer value.
        // */
        //public static int Parse(string value) // J2N: Renamed from ParseInt()
        //{
        //    //return Convert.ToInt32(value, 10);
        //    return Parse(value, 10);
        //}

        /**
         * Parses the specified string as a signed decimal integer value. The ASCII
         * character \u002d ('-') is recognized as the minus sign.
         * 
         * @param string
         *            the string representation of an integer value.
         * @return the primitive integer value represented by {@code string}.
         * @throws NumberFormatException
         *             if {@code string} is {@code null}, has a length of zero or
         *             can not be parsed as an integer value.
         */
        public static int Parse(string s, IFormatProvider? provider) // J2N: Renamed from ParseInt()
        {
            //return Convert.ToInt32(value, 10);
            return Parse(s, NumberStyle.Integer, provider);
        }

        /**
         * Parses the specified string as a signed decimal integer value. The ASCII
         * character \u002d ('-') is recognized as the minus sign.
         * 
         * @param string
         *            the string representation of an integer value.
         * @return the primitive integer value represented by {@code string}.
         * @throws NumberFormatException
         *             if {@code string} is {@code null}, has a length of zero or
         *             can not be parsed as an integer value.
         */
        public static int Parse(string s, NumberStyle style, IFormatProvider? provider) // J2N: Renamed from ParseInt()
        {
            //return Convert.ToInt32(value, 10);
            return int.Parse(s, (NumberStyles)style, provider);
        }

        /// <summary>
        /// Parses the <see cref="string"/> argument as a signed long in the specified <paramref name="radix"/>, beginning at the
        /// specified <paramref name="startIndex"/> with the specified number of characters in <paramref name="length"/>. 
        /// <para/>
        /// Usage Note: This is similar to the <see cref="Convert.ToInt32(string?, int)"/> method, however it allows conversion
        /// of the value without allocating a substring. It also differs in that it allows the use of the ASCII character \u002d ('-')
        /// or \u002B ('+') in any <paramref name="radix"/> and allows any <paramref name="radix"/> value from
        /// <see cref="Character.MinRadix"/> to <see cref="Character.MaxRadix"/> inclusive.
        /// </summary>
        /// <param name="s">The <see cref="string"/> containing the <see cref="int"/> representation to be parsed.</param>
        /// <param name="startIndex">The zero-based starting character position of a region in <paramref name="s"/>.</param>
        /// <param name="length">The number of characters in the region to parse.</param>
        /// <param name="radix">The radix (or base) to use when parsing <paramref name="s"/>. The value must be in the range
        /// <see cref="Character.MinRadix"/> - <see cref="Character.MaxRadix"/> inclusive.</param>
        /// <returns>The signed <see cref="int"/> represented by the subsequence in the specified <paramref name="radix"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="s"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startIndex"/> or <paramref name="length"/> is less than zero.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="startIndex"/> and <paramref name="length"/> refer to a location outside of <paramref name="s"/>.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="radix"/> is less than <see cref="Character.MinRadix"/> or greater than <see cref="Character.MaxRadix"/>.
        /// </exception>
        /// <exception cref="FormatException">
        /// The range of characters in <paramref name="s"/> specified by <paramref name="startIndex"/> and <paramref name="length"/>
        /// contains a character that is not a valid digit in the base specified by <paramref name="radix"/>.
        /// The exception message indicates that there are no digits to convert if the first character specified by
        /// <paramref name="startIndex"/> is invalid; otherwise, the message indicates that the range of characters in
        /// <paramref name="s"/> specified by <paramref name="startIndex"/> and <paramref name="length"/> contains invalid trailing characters.
        /// <para/>
        /// -or-
        /// <para/>
        /// The range of characters in <paramref name="s"/> specified by <paramref name="startIndex"/> and <paramref name="length"/>
        /// contain only a the ASCII character \u002d ('-') or \u002B ('+') sign and/or hexadecimal prefix 0X or 0x with no digits.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="length"/> is zero.
        /// </exception>
        /// <exception cref="OverflowException">
        /// The range of characters in <paramref name="s"/> specified by <paramref name="startIndex"/> and <paramref name="length"/>
        /// represents a number that is less than <see cref="int.MinValue"/> or greater than <see cref="int.MaxValue"/>.
        /// </exception>
        /// <seealso cref="Parse(string, int)"/>
        public static int Parse(string s, int startIndex, int length, int radix)
        {
            if (s is null)
                throw new ArgumentNullException(nameof(s));
            if (startIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(startIndex), startIndex, SR.ArgumentOutOfRange_NeedNonNegNum);
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length), length, SR.ArgumentOutOfRange_NeedNonNegNum);
            if (startIndex > s.Length - length) // Checks for int overflow
                throw new ArgumentOutOfRangeException(nameof(length), SR.ArgumentOutOfRange_IndexLength);

#if FEATURE_READONLYSPAN
            //return s != null ? ParseNumbers.StringToInt(s.Substring(startIndex, length).AsSpan(), radix, ParseNumbers.IsTight) : 0;
            return ParseNumbers.StringToInt(s.AsSpan(), radix, flags: ParseNumbers.NoSpace, sign: 1, ref startIndex, length);
#else
            return 0;
#endif
        }

        internal static int ParseUnsigned(string s, int startIndex, int length, int radix) // For testing purposes (actual method will eventually go on the UInt64 type when it is created)
        {
            if (s is null)
                throw new ArgumentNullException(nameof(s));
            if (startIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(startIndex), startIndex, SR.ArgumentOutOfRange_NeedNonNegNum);
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length), length, SR.ArgumentOutOfRange_NeedNonNegNum);
            if (startIndex > s.Length - length) // Checks for int overflow
                throw new ArgumentOutOfRangeException(nameof(length), SR.ArgumentOutOfRange_IndexLength);

#if FEATURE_READONLYSPAN
            return ParseNumbers.StringToInt(s.AsSpan(), radix, flags: ParseNumbers.NoSpace | ParseNumbers.TreatAsUnsigned, sign: 1, ref startIndex, length);
#else
            return 0;
#endif
        }

        internal static int ParseUnsigned(string s, int radix) // For testing purposes (actual method will eventually go on the UInt64 type when it is created)
        {
#if FEATURE_READONLYSPAN
            return s != null ?
                ParseNumbers.StringToInt(s.AsSpan(), radix, ParseNumbers.IsTight | ParseNumbers.TreatAsUnsigned) :
                0;
#else
            return 0;
#endif
        }

        /////**
        //// * Parses the specified string as a signed integer value using the specified
        //// * radix. The ASCII character \u002d ('-') is recognized as the minus sign.
        //// * 
        //// * @param string
        //// *            the string representation of an integer value.
        //// * @param radix
        //// *            the radix to use when parsing.
        //// * @return the primitive integer value represented by {@code string} using
        //// *         {@code radix}.
        //// * @throws NumberFormatException
        //// *             if {@code string} is {@code null} or has a length of zero,
        //// *             {@code radix < Character.MIN_RADIX},
        //// *             {@code radix > Character.MAX_RADIX}, or if {@code string}
        //// *             can not be parsed as an integer value.
        //// */
        ////public static int Parse(string s, int radix) // J2N: Renamed from ParseInt()
        ////{
        ////    if (s == null || radix < Character.MinRadix
        ////            || radix > Character.MaxRadix)
        ////    {
        ////        throw new FormatException();
        ////    }
        ////    int length = s.Length, i = 0;
        ////    if (length == 0)
        ////    {
        ////        throw new FormatException(s);
        ////    }
        ////    bool negative = s[i] == '-';
        ////    if (negative && ++i == length)
        ////    {
        ////        throw new FormatException(s);
        ////    }

        ////    return Parse(s, i, radix, negative);
        ////}

        /// <summary>
        /// J2N TODO: Docs
        /// </summary>
        /// <param name="s"></param>
        /// <param name="radix"></param>
        /// <returns></returns>
        public static int Parse(string s, int radix) // J2N: Renamed from ParseLong()
        {
#if FEATURE_READONLYSPAN
            return s != null ?
                ParseNumbers.StringToInt(s.AsSpan(), radix, ParseNumbers.IsTight) :
                0;
#else
            return 0; // J2N TODO: finish
#endif
        }

        private static int Parse(string value, int offset, int radix,
                bool negative)
        {
            int max = int.MinValue / radix;
            int result = 0, length = value.Length;
            while (offset < length)
            {
                int digit = Character.Digit(value[offset++], radix);
                if (digit == -1)
                {
                    throw new FormatException(value);
                }
                if (max > result)
                {
                    throw new FormatException(value);
                }
                int next = result * radix - digit;
                if (next > result)
                {
                    throw new FormatException(value);
                }
                result = next;
            }
            if (!negative)
            {
                result = -result;
                if (result < 0)
                {
                    throw new FormatException(value);
                }
            }
            return result;
        }

        /// <summary>
        /// Converts the string representation of a number to its 32-bit signed integer equivalent.
        /// A return value indicates whether the conversion succeeded.
        /// <para/>
        /// Usage Note: When porting from Java, note that this is a culture-sensitive method. Java uses the invariant
        /// culture in the Integer class, so call <see cref="TryParse(string?, NumberStyle, IFormatProvider?, out int)"/>
        /// and pass <see cref="NumberStyle.AllowLeadingSign"/> and <see cref="NumberFormatInfo.InvariantInfo"/> to exactly match the Java parsing behavior.
        /// <para/>
        /// Using that overload also allows you to normalize the exception behavior. The <see cref="Parse(string, IFormatProvider?)"/>
        /// method may throw <see cref="ArgumentNullException"/>, <see cref="OverflowException"/>, or <see cref="FormatException"/>,
        /// but Java only ever throws NumberFormatException in any of those cases. Using <see cref="TryParse(string?, NumberStyle, IFormatProvider?, out int)"/>
        /// means a <see cref="FormatException"/> can be thrown in any of those error condtions when it returns <c>false</c>.
        /// </summary>
        /// <param name="s">A string containing a number to convert.</param>
        /// <param name="result">When this method returns, contains the 32-bit signed integer value equivalent of
        /// the number contained in <paramref name="s"/>, if the conversion succeeded, or zero if the conversion failed.
        /// The conversion fails if the s parameter is <c>null</c> or <see cref="string.Empty"/>, is not of the correct format,
        /// or represents a number less than <see cref="int.MinValue"/> or greater than <see cref="int.MaxValue"/>. This parameter
        /// is passed uninitialized; any value originally supplied in <paramref name="result"/> will be overwritten.</param>
        /// <returns><c>true</c> if <paramref name="s"/> was converted successfully; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// The <see cref="TryParse(string?, out int)"/> method is like the <see cref="Parse(string, IFormatProvider?)"/> method,
        /// except the <see cref="TryParse(string?, out int)"/> method does not throw an exception if the
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
        /// information that can be present in <paramref name="s"/>, use the <see cref="TryParse(string?, NumberStyle, IFormatProvider?, out int)"/> method.
        /// <para/>
        /// The <paramref name="s"/> parameter is parsed using the formatting information in a <see cref="NumberFormatInfo"/>
        /// object initialized for the current system culture. For more information, see <see cref="NumberFormatInfo.CurrentInfo"/>.
        /// <para/>
        /// This overload of the <see cref="TryParse(string?, out int)"/> method interprets all digits in the <paramref name="s"/> parameter
        /// as decimal digits. To parse the string representation of a hexadecimal number, call the
        /// <see cref="TryParse(string?, NumberStyle, IFormatProvider?, out int)"/> overload.
        /// </remarks>
        /// <seealso cref="Parse(string, IFormatProvider?)"/>
        /// <seealso cref="Parse(string, int)"/>
        /// <seealso cref="Number.ToString()"/>
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif 
        public static bool TryParse([NotNullWhen(true)] string? s, out int result)
        {
            return int.TryParse(s, out result);
        }

#if FEATURE_READONLYSPAN
        /// <summary>
        /// Converts the span representation of a number in a specified style and culture-specific format to its 32-bit signed
        /// integer equivalent. A return value indicates whether the conversion succeeded.
        /// <para/>
        /// Usage Note: When porting from Java, note that this is a culture-sensitive method. Java uses the invariant
        /// culture in the Integer class, so call <see cref="TryParse(ReadOnlySpan{char}, NumberStyle, IFormatProvider?, out int)"/>
        /// and pass <see cref="NumberStyle.AllowLeadingSign"/> and <see cref="NumberFormatInfo.InvariantInfo"/> to exactly match the Java parsing behavior.
        /// <para/>
        /// Using that overload also allows you to normalize the exception behavior. The <see cref="Parse(string, IFormatProvider?)"/>
        /// method may throw <see cref="ArgumentNullException"/>, <see cref="OverflowException"/>, or <see cref="FormatException"/>,
        /// but Java only ever throws NumberFormatException in any of those cases. Using <see cref="TryParse(ReadOnlySpan{char}, NumberStyle, IFormatProvider?, out int)"/>
        /// means a <see cref="FormatException"/> can be thrown in any of those error condtions when it returns <c>false</c>.
        /// </summary>
        /// <param name="s">A span containing the characters that represent the number to convert.</param>
        /// <param name="result">When this method returns, contains the 32-bit signed integer value equivalent of
        /// the number contained in <paramref name="s"/>, if the conversion succeeded, or zero if the conversion failed.
        /// The conversion fails if the s parameter is <c>null</c> or <see cref="string.Empty"/>, is not of the correct format,
        /// or represents a number less than <see cref="int.MinValue"/> or greater than <see cref="int.MaxValue"/>. This parameter
        /// is passed uninitialized; any value originally supplied in <paramref name="result"/> will be overwritten.</param>
        /// <returns><c>true</c> if <paramref name="s"/> was converted successfully; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// The <see cref="TryParse(ReadOnlySpan{char}, out int)"/> method is like the <see cref="Parse(ReadOnlySpan{char}, IFormatProvider?)"/> method,
        /// except the <see cref="TryParse(ReadOnlySpan{char}, out int)"/> method does not throw an exception if the
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
        /// information that can be present in <paramref name="s"/>, use the <see cref="TryParse(ReadOnlySpan{char}, NumberStyle, IFormatProvider?, out int)"/> method.
        /// <para/>
        /// The <paramref name="s"/> parameter is parsed using the formatting information in a <see cref="NumberFormatInfo"/>
        /// object initialized for the current system culture. For more information, see <see cref="NumberFormatInfo.CurrentInfo"/>.
        /// <para/>
        /// This overload of the <see cref="TryParse(ReadOnlySpan{char}, out int)"/> method interprets all digits in the <paramref name="s"/> parameter
        /// as decimal digits. To parse the string representation of a hexadecimal number, call the
        /// <see cref="TryParse(ReadOnlySpan{char}, NumberStyle, IFormatProvider?, out int)"/> overload.
        /// </remarks>
        /// <seealso cref="Parse(ReadOnlySpan{char}, IFormatProvider?)"/>
        /// <seealso cref="TryParse(string?, out int)"/>
        /// <seealso cref="Number.ToString()"/>
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif 
        public static bool TryParse(ReadOnlySpan<char> s, out int result)
        {
            return int.TryParse(s, out result);
        }
#endif

        /// <summary>
        /// Converts the string representation of a number in a specified style and culture-specific format to its
        /// 32-bit signed integer equivalent. A return value indicates whether the conversion succeeded.
        /// <para/>
        /// Usage Note: To exactly match Java, use <see cref="NumberStyle.AllowLeadingSign"/> for <paramref name="style"/> and
        /// <see cref="NumberFormatInfo.InvariantInfo"/> for <paramref name="provider"/>. We recommend factoring out
        /// exceptions when parsing, but if the Java code depends on exceptions, throw <see cref="FormatException"/>
        /// when this method returns <c>false</c>.
        /// </summary>
        /// <param name="s">A string containing a number to convert. The string is interpreted using the style specified by <paramref name="style"/>.</param>
        /// <param name="style">A bitwise combination of enumeration values that indicates the style elements that can be present in <paramref name="s"/>.
        /// A typical value to specify is <see cref="NumberStyle.Integer"/>.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information about <paramref name="s"/>.</param>
        /// <param name="result">When this method returns, contains the 32-bit signed integer value equivalent of
        /// the number contained in <paramref name="s"/>, if the conversion succeeded, or zero if the conversion failed.
        /// The conversion fails if the s parameter is <c>null</c> or <see cref="string.Empty"/>, is not of the correct format,
        /// or represents a number less than <see cref="int.MinValue"/> or greater than <see cref="int.MaxValue"/>. This parameter
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
        /// The <see cref="TryParse(string?, NumberStyle, IFormatProvider?, out int)"/> method is like the <see cref="Parse(string, NumberStyle, IFormatProvider?)"/> method,
        /// except the <see cref="TryParse(string?, NumberStyle, IFormatProvider?, out int)"/> method does not throw an exception if the
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
        ///         number in exponential notation, it must represent an integer within the range of the <see cref="int"/> data type
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
        /// <seealso cref="TryParse(string?, out int)"/>
        /// <seealso cref="Number.ToString()"/>
        /// <seealso cref="NumberStyle"/>
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool TryParse([NotNullWhen(true)] string? s, NumberStyle style, IFormatProvider? provider, out int result)
        {
            return int.TryParse(s, (NumberStyles)style, provider, out result);
        }


#if FEATURE_READONLYSPAN
        /// <summary>
        /// Converts the span representation of a number in a specified style and culture-specific format to its 32-bit signed integer equivalent.
        /// A return value indicates whether the conversion succeeded.
        /// <para/>
        /// Usage Note: To exactly match Java, use <see cref="NumberStyle.AllowLeadingSign"/> for <paramref name="style"/> and
        /// <see cref="NumberFormatInfo.InvariantInfo"/> for <paramref name="provider"/>. We recommend factoring out
        /// exceptions when parsing, but if the Java code depends on exceptions, throw <see cref="FormatException"/>
        /// when this method returns <c>false</c>.
        /// </summary>
        /// <param name="s">A span containing the characters that represent the number to convert. The span is interpreted using
        /// the style specified by <paramref name="style"/>.</param>
        /// <param name="style">A bitwise combination of enumeration values that indicates the style elements that can be present in <paramref name="s"/>.
        /// A typical value to specify is <see cref="NumberStyle.Integer"/>.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information about <paramref name="s"/>.</param>
        /// <param name="result">When this method returns, contains the 32-bit signed integer value equivalent of
        /// the number contained in <paramref name="s"/>, if the conversion succeeded, or zero if the conversion failed.
        /// The conversion fails if the s parameter is <c>null</c> or <see cref="string.Empty"/>, is not of the correct format,
        /// or represents a number less than <see cref="int.MinValue"/> or greater than <see cref="int.MaxValue"/>. This parameter
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
        /// The <see cref="TryParse(ReadOnlySpan{char}, NumberStyle, IFormatProvider?, out int)"/> method is like the <see cref="Parse(ReadOnlySpan{char}, NumberStyle, IFormatProvider?)"/> method,
        /// except the <see cref="TryParse(ReadOnlySpan{char}, NumberStyle, IFormatProvider?, out int)"/> method does not throw an exception if the
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
        ///         number in exponential notation, it must represent an integer within the range of the <see cref="int"/> data type
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
        /// <seealso cref="TryParse(ReadOnlySpan{char}, out int)"/>
        /// <seealso cref="Number.ToString()"/>
        /// <seealso cref="NumberStyle"/>
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif 
        public static bool TryParse(ReadOnlySpan<char> s, NumberStyle style, IFormatProvider? provider, out int result)
        {
            return int.TryParse(s, (NumberStyles)style, provider, out result);
        }
#endif

        /// <inheritdoc/>
        public override short GetInt16Value()
        {
            return (short)value;
        }


        /**
 * Converts the specified integer into its binary string representation. The
 * returned string is a concatenation of '0' and '1' characters.
 * 
 * @param i
 *            the integer to convert.
 * @return the binary string representation of {@code i}.
 */
        public static string ToBinaryString(int i)
        {
            return i.ToBinaryString();
            //int count = 1, j = i;

            //if (i < 0)
            //{
            //    count = 32;
            //}
            //else
            //{
            //    while ((j >>>= 1) != 0)
            //    {
            //        count++;
            //    }
            //}

            //char[] buffer = new char[count];
            //do
            //{
            //    buffer[--count] = (char)((i & 1) + '0');
            //    i >>>= 1;
            //} while (count > 0);
            //return new String(0, buffer.length, buffer);
        }

        /**
         * Converts the specified integer into its hexadecimal string
         * representation. The returned string is a concatenation of characters from
         * '0' to '9' and 'a' to 'f'.
         * 
         * @param i
         *            the integer to convert.
         * @return the hexadecimal string representation of {@code i}.
         */
        public static string ToHexString(int i)
        {
            return i.ToHexString();
            //int count = 1, j = i;

            //if (i < 0)
            //{
            //    count = 8;
            //}
            //else
            //{
            //    while ((j >>>= 4) != 0)
            //    {
            //        count++;
            //    }
            //}

            //char[] buffer = new char[count];
            //do
            //{
            //    int t = i & 15;
            //    if (t > 9)
            //    {
            //        t = t - 10 + 'a';
            //    }
            //    else
            //    {
            //        t += '0';
            //    }
            //    buffer[--count] = (char)t;
            //    i >>>= 4;
            //} while (count > 0);
            //return new String(0, buffer.length, buffer);
        }

        /**
         * Converts the specified integer into its octal string representation. The
         * returned string is a concatenation of characters from '0' to '7'.
         * 
         * @param i
         *            the integer to convert.
         * @return the octal string representation of {@code i}.
         */
        public static string ToOctalString(int i)
        {
            return i.ToOctalString();
            //int count = 1, j = i;

            //if (i < 0)
            //{
            //    count = 11;
            //}
            //else
            //{
            //    while ((j >>>= 3) != 0)
            //    {
            //        count++;
            //    }
            //}

            //char[] buffer = new char[count];
            //do
            //{
            //    buffer[--count] = (char)((i & 7) + '0');
            //    i >>>= 3;
            //} while (count > 0);
            //return new String(0, buffer.length, buffer);
        }





        /// <inheritdoc/>
        public override string ToString(string? format, IFormatProvider? provider) // .NETified...
        {
            return ToString(format, provider, value);
        }

        /**
         * Converts the specified integer into its decimal string representation.
         * The returned string is a concatenation of a minus sign if the number is
         * negative and characters from '0' to '9'.
         * 
         * @param value
         *            the integer to convert.
         * @return the decimal string representation of {@code value}.
         */
        public static string ToString(int value)
        {
            return value.ToString(J2N.Text.StringFormatter.CurrentCulture);

            //if (value == 0)
            //{
            //    return "0"; //$NON-NLS-1$
            //}

            //// Faster algorithm for smaller Integers
            //if (value < 1000 && value > -1000)
            //{
            //    char[] buffer = new char[4];
            //    int positive_value = value < 0 ? -value : value;
            //    int first_digit = 0;
            //    if (value < 0)
            //    {
            //        buffer[0] = '-';
            //        first_digit++;
            //    }
            //    int last_digit = first_digit;
            //    int quot = positive_value;
            //    do
            //    {
            //        int res = quot / 10;
            //        int digit_value = quot - ((res << 3) + (res << 1));
            //        digit_value += '0';
            //        buffer[last_digit++] = (char)digit_value;
            //        quot = res;
            //    } while (quot != 0);

            //    int count = last_digit--;
            //    do
            //    {
            //        char tmp = buffer[last_digit];
            //        buffer[last_digit--] = buffer[first_digit];
            //        buffer[first_digit++] = tmp;
            //    } while (first_digit < last_digit);
            //    return new String(0, count, buffer);
            //}
            //if (value == MIN_VALUE)
            //{
            //    return "-2147483648";//$NON-NLS-1$
            //}

            //char[] buffer = new char[11];
            //int positive_value = value < 0 ? -value : value;
            //byte first_digit = 0;
            //if (value < 0)
            //{
            //    buffer[0] = '-';
            //    first_digit++;
            //}
            //byte last_digit = first_digit;
            //byte count;
            //int number;
            //boolean start = false;
            //for (int i = 0; i < 9; i++)
            //{
            //    count = 0;
            //    if (positive_value < (number = decimalScale[i]))
            //    {
            //        if (start)
            //        {
            //            buffer[last_digit++] = '0';
            //        }
            //        continue;
            //    }

            //    if (i > 0)
            //    {
            //        number = (decimalScale[i] << 3);
            //        if (positive_value >= number)
            //        {
            //            positive_value -= number;
            //            count += 8;
            //        }
            //        number = (decimalScale[i] << 2);
            //        if (positive_value >= number)
            //        {
            //            positive_value -= number;
            //            count += 4;
            //        }
            //    }
            //    number = (decimalScale[i] << 1);
            //    if (positive_value >= number)
            //    {
            //        positive_value -= number;
            //        count += 2;
            //    }
            //    if (positive_value >= decimalScale[i])
            //    {
            //        positive_value -= decimalScale[i];
            //        count++;
            //    }
            //    if (count > 0 && !start)
            //    {
            //        start = true;
            //    }
            //    if (start)
            //    {
            //        buffer[last_digit++] = (char)(count + '0');
            //    }
            //}

            //buffer[last_digit++] = (char)(positive_value + '0');
            //count = last_digit--;
            //return new String(0, count, buffer);
        }

        /**
* Returns a string containing a concise, human-readable description of the
* specified byte value.
* 
* @param value
*            the byte to convert to a string.
* @return a printable representation of {@code value}.
*/
        public static string ToString(int value, IFormatProvider? provider)
        {
            //return Integer.toString(value);
            return value.ToString(provider);
        }

        /**
         * Converts the specified integer into a string representation based on the
         * specified radix. The returned string is a concatenation of a minus sign
         * if the number is negative and characters from '0' to '9' and 'a' to 'z',
         * depending on the radix. If {@code radix} is not in the interval defined
         * by {@code Character.MIN_RADIX} and {@code Character.MAX_RADIX} then 10 is
         * used as the base for the conversion.
         * 
         * @param i
         *            the integer to convert.
         * @param radix
         *            the base to use for the conversion.
         * @return the string representation of {@code i}.
         */
        public static string ToString(int i, int radix) // J2N: Unlike Convert.ToInt32, this supports "fromBase" (radix) up to 36
        {
            if (radix < Character.MinRadix || radix > Character.MaxRadix)
            {
                radix = 10;
            }
            if (i == 0)
            {
                return "0"; //$NON-NLS-1$
            }

            int count = 2, j = i;
            bool negative = i < 0;
            if (!negative)
            {
                count = 1;
                j = -i;
            }
            while ((i /= radix) != 0)
            {
                count++;
            }

            char[] buffer = new char[count];
            do
            {
                int ch = 0 - (j % radix);
                if (ch > 9)
                {
                    ch = ch - 10 + 'a';
                }
                else
                {
                    ch += '0';
                }
                buffer[--count] = (char)ch;
            } while ((j /= radix) != 0);
            if (negative)
            {
                buffer[0] = '-';
            }
            return new string(buffer, 0, buffer.Length);
        }


        /////**
        ////* Parses the specified string as a signed decimal integer value.
        ////* 
        ////* @param string
        ////*            the string representation of an integer value.
        ////* @return an {@code Integer} instance containing the integer value
        ////*         represented by {@code string}.
        ////* @throws NumberFormatException
        ////*             if {@code string} is {@code null}, has a length of zero or
        ////*             can not be parsed as an integer value.
        ////* @see #parseInt(String)
        ////*/
        ////public static Int32 ValueOf(string value)
        ////{
        ////    return ValueOf(Parse(value));
        ////}
        ///

        /**
        * Parses the specified string as a signed decimal integer value.
        * 
        * @param string
        *            the string representation of an integer value.
        * @return an {@code Integer} instance containing the integer value
        *         represented by {@code string}.
        * @throws NumberFormatException
        *             if {@code string} is {@code null}, has a length of zero or
        *             can not be parsed as an integer value.
        * @see #parseInt(String)
        */
        public static Int32 ValueOf(string value, IFormatProvider? provider)
        {
            return ValueOf(Parse(value, provider));
        }

        /**
        * Parses the specified string as a signed decimal integer value.
        * 
        * @param string
        *            the string representation of an integer value.
        * @return an {@code Integer} instance containing the integer value
        *         represented by {@code string}.
        * @throws NumberFormatException
        *             if {@code string} is {@code null}, has a length of zero or
        *             can not be parsed as an integer value.
        * @see #parseInt(String)
        */
        public static Int32 ValueOf(string value, NumberStyle style, IFormatProvider? provider)
        {
            return ValueOf(Parse(value, style, provider));
        }

        /**
         * Parses the specified string as a signed integer value using the specified
         * radix.
         * 
         * @param string
         *            the string representation of an integer value.
         * @param radix
         *            the radix to use when parsing.
         * @return an {@code Integer} instance containing the integer value
         *         represented by {@code string} using {@code radix}.
         * @throws NumberFormatException
         *             if {@code string} is {@code null} or has a length of zero,
         *             {@code radix < Character.MIN_RADIX},
         *             {@code radix > Character.MAX_RADIX}, or if {@code string}
         *             can not be parsed as an integer value.
         * @see #parseInt(String, int)
         */
        public static Int32 ValueOf(string value, int radix)
        {
            return ValueOf(Parse(value, radix));
        }


        /**
     * Determines the highest (leftmost) bit of the specified integer that is 1
     * and returns the bit mask value for that bit. This is also referred to as
     * the Most Significant 1 Bit. Returns zero if the specified integer is
     * zero.
     * 
     * @param i
     *            the integer to examine.
     * @return the bit mask indicating the highest 1 bit in {@code i}.
     * @since 1.5
     */
        public static int HighestOneBit(int i)
        {
            return i.HighestOneBit();
            //i |= (i >> 1);
            //i |= (i >> 2);
            //i |= (i >> 4);
            //i |= (i >> 8);
            //i |= (i >> 16);
            //return (i & ~(i >>> 1));
        }

        /**
         * Determines the lowest (rightmost) bit of the specified integer that is 1
         * and returns the bit mask value for that bit. This is also referred
         * to as the Least Significant 1 Bit. Returns zero if the specified integer
         * is zero.
         * 
         * @param i
         *            the integer to examine.
         * @return the bit mask indicating the lowest 1 bit in {@code i}.
         * @since 1.5
         */
        public static int LowestOneBit(int i)
        {
            return i.LowestOneBit();
            //return (i & (-i));
        }

        /**
         * Determines the number of leading zeros in the specified integer prior to
         * the {@link #highestOneBit(int) highest one bit}.
         *
         * @param i
         *            the integer to examine.
         * @return the number of leading zeros in {@code i}.
         * @since 1.5
         */
        public static int NumberOfLeadingZeros(int i) // J2N TODO: Change name, or eliminate?
        {
            return i.LeadingZeroCount();

            //i |= i >> 1;
            //i |= i >> 2;
            //i |= i >> 4;
            //i |= i >> 8;
            //i |= i >> 16;
            //return bitCount(~i);
        }

        /**
         * Determines the number of trailing zeros in the specified integer after
         * the {@link #lowestOneBit(int) lowest one bit}.
         *
         * @param i
         *            the integer to examine.
         * @return the number of trailing zeros in {@code i}.
         * @since 1.5
         */
        public static int NumberOfTrailingZeros(int i) // J2N TODO: Change name, or eliminate?
        {
            return i.TrailingZeroCount();
            //return bitCount((i & -i) - 1);
        }

        /**
         * Counts the number of 1 bits in the specified integer; this is also
         * referred to as population count.
         *
         * @param i
         *            the integer to examine.
         * @return the number of 1 bits in {@code i}.
         * @since 1.5
         */
        public static int BitCount(int i) // J2N TODO: Change name, or eliminate?
        {
            return i.PopCount();
            //i -= ((i >> 1) & 0x55555555);
            //i = (i & 0x33333333) + ((i >> 2) & 0x33333333);
            //i = (((i >> 4) + i) & 0x0F0F0F0F);
            //i += (i >> 8);
            //i += (i >> 16);
            //return (i & 0x0000003F);
        }

        /**
         * Rotates the bits of the specified integer to the left by the specified
         * number of bits.
         *
         * @param i
         *            the integer value to rotate left.
         * @param distance
         *            the number of bits to rotate.
         * @return the rotated value.
         * @since 1.5
         */
        public static int RotateLeft(int i, int distance)
        {
            return i.RotateLeft(distance);
            //if (distance == 0)
            //{
            //    return i;
            //}
            ///*
            // * According to JLS3, 15.19, the right operand of a shift is always
            // * implicitly masked with 0x1F, which the negation of 'distance' is
            // * taking advantage of.
            // */
            //return ((i << distance) | (i >>> (-distance)));
        }

        /**
         * Rotates the bits of the specified integer to the right by the specified
         * number of bits.
         *
         * @param i
         *            the integer value to rotate right.
         * @param distance
         *            the number of bits to rotate.
         * @return the rotated value.
         * @since 1.5
         */
        public static int RotateRight(int i, int distance)
        {
            return i.RotateRight(distance);
            //if (distance == 0)
            //{
            //    return i;
            //}
            ///*
            // * According to JLS3, 15.19, the right operand of a shift is always
            // * implicitly masked with 0x1F, which the negation of 'distance' is
            // * taking advantage of.
            // */
            //return ((i >>> distance) | (i << (-distance)));
        }

        /**
         * Reverses the order of the bytes of the specified integer.
         * 
         * @param i
         *            the integer value for which to reverse the byte order.
         * @return the reversed value.
         * @since 1.5
         */
        public static int ReverseBytes(int i)
        {
            return i.ReverseBytes();
            //int b3 = i >>> 24;
            //int b2 = (i >>> 8) & 0xFF00;
            //int b1 = (i & 0xFF00) << 8;
            //int b0 = i << 24;
            //return (b0 | b1 | b2 | b3);
        }

        /**
         * Reverses the order of the bits of the specified integer.
         * 
         * @param i
         *            the integer value for which to reverse the bit order.
         * @return the reversed value.
         * @since 1.5
         */
        public static int Reverse(int i)
        {
            return i.Reverse();
            //// From Hacker's Delight, 7-1, Figure 7-1
            //i = (i & 0x55555555) << 1 | (i >> 1) & 0x55555555;
            //i = (i & 0x33333333) << 2 | (i >> 2) & 0x33333333;
            //i = (i & 0x0F0F0F0F) << 4 | (i >> 4) & 0x0F0F0F0F;
            //return reverseBytes(i);
        }


        /**
     * Returns the value of the {@code signum} function for the specified
     * integer.
     * 
     * @param i
     *            the integer value to check.
     * @return -1 if {@code i} is negative, 1 if {@code i} is positive, 0 if
     *         {@code i} is zero.
     * @since 1.5
     */
        public static int Signum(int i)
        {
            return i.Signum();
        }


        /**
         * Returns a {@code Integer} instance for the specified integer value.
         * <p>
         * If it is not necessary to get a new {@code Integer} instance, it is
         * recommended to use this method instead of the constructor, since it
         * maintains a cache of instances which may result in better performance.
         *
         * @param i
         *            the integer value to store in the instance.
         * @return a {@code Integer} instance containing {@code i}.
         * @since 1.5
         */
        public static Int32 ValueOf(int i)
        {
            if (i < -128 || i > 127)
            {
                return new Int32(i);
            }
            return ValueOfCache.CACHE[i + 128];

        }

        static class ValueOfCache
        {
            /**
             * <p>
             * A cache of instances used by {@link Integer#valueOf(int)} and auto-boxing.
             */
            internal static readonly Int32[] CACHE = LoadCache();

            private static Int32[] LoadCache()
            {
                var cache = new Int32[256];
                for (int i = -128; i <= 127; i++)
                {
                    cache[i + 128] = new Int32(i);
                }
                return cache;
            }
        }





        // J2N: Support implicit conversion

        /// <inheritdoc/>
        public static implicit operator int(Int32 value) => value.value;
        /// <inheritdoc/>
        public static implicit operator Int32(int value) => ValueOf(value);
    }
}
