using J2N.Globalization;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace J2N.Numerics
{
    using SR = J2N.Resources.Strings;

    /// <inheritdoc/>
    public sealed class Int64 : Number, IComparable<Int64>
    {
        /**
     * The value which the receiver represents.
     */
        private readonly long value;

        ////    /**
        ////     * Constant for the maximum {@code long} value, 2<sup>63</sup>-1.
        ////     */
        ////    public static final long MAX_VALUE = 0x7FFFFFFFFFFFFFFFL;

        ////    /**
        ////     * Constant for the minimum {@code long} value, -2<sup>63</sup>.
        ////     */
        ////    public static final long MIN_VALUE = 0x8000000000000000L;

        ////    /**
        ////     * The {@link Class} object that represents the primitive type {@code long}.
        ////     */
        ////    @SuppressWarnings("unchecked")
        ////public static final Class<Long> TYPE = (Class<Long>)new long[0].getClass()
        ////        .getComponentType();

        ////    // Note: This can't be set to "long.class", since *that* is
        ////    // defined to be "java.lang.Long.TYPE";

        /**
         * Constant for the number of bits needed to represent a {@code long} in
         * two's complement form.
         *
         * @since 1.5
         */
        public const int SIZE = 64; // J2N TODO: Rename BitCount? The BitCount method will be named PopCount() so there won't be a collision


        /**
         * Constructs a new {@code Long} with the specified primitive long value.
         * 
         * @param value
         *            the primitive long value to store in the new instance.
         */
        public Int64(long value)
        {
            this.value = value;
        }

        ///**
        // * Constructs a new {@code Long} from the specified string.
        // * 
        // * @param string
        // *            the string representation of a long value.
        // * @throws NumberFormatException
        // *             if {@code string} can not be decoded into a long value.
        // * @see #parseLong(String)
        // */
        //public Int64(string value)
        //    : this(Parse(value))
        //{

        //}

        /**
         * Constructs a new {@code Long} from the specified string.
         * 
         * @param string
         *            the string representation of a long value.
         * @throws NumberFormatException
         *             if {@code string} can not be decoded into a long value.
         * @see #parseLong(String)
         */
        public Int64(string value, IFormatProvider? provider)
            : this(Parse(value, provider))
        {
        }

        /**
         * Constructs a new {@code Long} from the specified string.
         * 
         * @param string
         *            the string representation of a long value.
         * @throws NumberFormatException
         *             if {@code string} can not be decoded into a long value.
         * @see #parseLong(String)
         */
        public Int64(string value, NumberStyle style, IFormatProvider? provider)
            : this(Parse(value, style, provider))
        {
        }

        /// <inheritdoc/>
        public override byte GetByteValue()
        {
            return (byte)value;
        }

        /**
         * Compares this object to the specified long object to determine their
         * relative order.
         * 
         * @param object
         *            the long object to compare this object to.
         * @return a negative value if the value of this long is less than the value
         *         of {@code object}; 0 if the value of this long and the value of
         *         {@code object} are equal; a positive value if the value of this
         *         long is greater than the value of {@code object}.
         * @see java.lang.Comparable
         * @since 1.2
         */
        public int CompareTo(Int64? other)
        {
            if (other is null) return 1; // Using 1 if other is null as specified here: https://stackoverflow.com/a/4852537
            return value > other.value ? 1 : (value < other.value ? -1 : 0);
        }

        /**
         * Parses the specified string and returns a {@code Long} instance if the
         * string can be decoded into a long value. The string may be an optional
         * minus sign "-" followed by a hexadecimal ("0x..." or "#..."), octal
         * ("0..."), or decimal ("...") representation of a long.
         * 
         * @param string
         *            a string representation of a long value.
         * @return a {@code Long} containing the value represented by {@code string}.
         * @throws NumberFormatException
         *             if {@code string} can not be parsed as a long value.
         */
        public static Int64 Decode(string value)
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
                    return ValueOf(0L);
                }
                if ((firstDigit = value[i]) == 'x' || firstDigit == 'X')
                {
                    if (i == length)
                    {
                        throw new FormatException(value); // J2N TODO: Error message
                    }
                    i++;
                    @base = 16;
                }
                else
                {
                    @base = 8;
                }
            }
            else if (firstDigit == '#')
            {
                if (i == length)
                {
                    throw new FormatException(value); // J2N TODO: Error message
                }
                i++;
                @base = 16;
            }
#if FEATURE_READONLYSPAN
            long result = ParseNumbers.StringToLong(value.AsSpan(), @base, flags: ParseNumbers.IsTight, sign, ref i, value.Length - i);
#else
            long result = Parse(value, i, @base, negative: sign < 0);
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
         * {@code Long} and have the same long value as this object.
         * 
         * @param o
         *            the object to compare this long with.
         * @return {@code true} if the specified object is equal to this
         *         {@code Long}; {@code false} otherwise.
         */
        public override bool Equals(object? obj)
        {
            if (obj is null) return false;

            return (obj is Int64 other)
                    && (value == other.value);
        }

        /// <inheritdoc/>
        public override float GetSingleValue()
        {
            return value;
        }

        /////**
        //// * Returns the {@code Long} value of the system property identified by
        //// * {@code string}. Returns {@code null} if {@code string} is {@code null}
        //// * or empty, if the property can not be found or if its value can not be
        //// * parsed as a long.
        //// * 
        //// * @param string
        //// *            the name of the requested system property.
        //// * @return the requested property's value as a {@code Long} or {@code null}.
        //// */
        ////public static Long getLong(String string)
        ////{
        ////    if (string == null || string.length() == 0)
        ////    {
        ////        return null;
        ////    }
        ////    String prop = System.getProperty(string);
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
        //// * Returns the {@code Long} value of the system property identified by
        //// * {@code string}. Returns the specified default value if {@code string} is
        //// * {@code null} or empty, if the property can not be found or if its value
        //// * can not be parsed as a long.
        //// * 
        //// * @param string
        //// *            the name of the requested system property.
        //// * @param defaultValue
        //// *            the default value that is returned if there is no long system
        //// *            property with the requested name.
        //// * @return the requested property's value as a {@code Long} or the default
        //// *         value.
        //// */
        ////public static Long getLong(String string, long defaultValue)
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
        //// * Returns the {@code Long} value of the system property identified by
        //// * {@code string}. Returns the specified default value if {@code string} is
        //// * {@code null} or empty, if the property can not be found or if its value
        //// * can not be parsed as a long.
        //// * 
        //// * @param string
        //// *            the name of the requested system property.
        //// * @param defaultValue
        //// *            the default value that is returned if there is no long system
        //// *            property with the requested name.
        //// * @return the requested property's value as a {@code Long} or the default
        //// *         value.
        //// */
        ////public static Long getLong(String string, Long defaultValue)
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
            return (int)(value ^ (value.TripleShift(32)));
        }

        /// <inheritdoc/>
        public override int GetInt32Value()
        {
            return (int)value;
        }

        /**
         * Gets the primitive value of this long.
         * 
         * @return this object's primitive value.
         */
        public override long GetInt64Value()
        {
            return value;
        }

        ///**
        // * Parses the specified string as a signed decimal long value. The ASCII
        // * character \u002d ('-') is recognized as the minus sign.
        // * 
        // * @param string
        // *            the string representation of a long value.
        // * @return the primitive long value represented by {@code string}.
        // * @throws NumberFormatException
        // *             if {@code string} is {@code null}, has a length of zero or
        // *             can not be parsed as a long value.
        // */
        //public static long Parse(string value) // J2N: Renamed from ParseLong()
        //{
        //    return Parse(value, 10);
        //}

        /**
         * Parses the specified string as a signed decimal long value. The ASCII
         * character \u002d ('-') is recognized as the minus sign.
         * 
         * @param string
         *            the string representation of a long value.
         * @return the primitive long value represented by {@code string}.
         * @throws NumberFormatException
         *             if {@code string} is {@code null}, has a length of zero or
         *             can not be parsed as a long value.
         */
        public static long Parse(string s, IFormatProvider? provider) // J2N: Renamed from ParseLong()
        {
            return Parse(s, NumberStyle.Integer, provider);
        }

        /**
         * Parses the specified string as a signed decimal long value. The ASCII
         * character \u002d ('-') is recognized as the minus sign.
         * 
         * @param string
         *            the string representation of a long value.
         * @return the primitive long value represented by {@code string}.
         * @throws NumberFormatException
         *             if {@code string} is {@code null}, has a length of zero or
         *             can not be parsed as a long value.
         */
        public static long Parse(string s, NumberStyle style , IFormatProvider? provider) // J2N: Renamed from ParseLong()
        {
            return long.Parse(s, (NumberStyles)style, provider); // J2N TODO: AllowTrailingTypeSpecifier
        }

        internal static long ParseUnsigned(string s, int startIndex, int length, int radix) // For testing purposes (actual method will eventually go on the UInt64 type when it is created)
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
            return ParseNumbers.StringToLong(s.AsSpan(), radix, flags: ParseNumbers.NoSpace | ParseNumbers.TreatAsUnsigned, sign: 1, ref startIndex, length);
#else
            return 0;
#endif
        }

        internal static long ParseUnsigned(string s, int radix) // For testing purposes (actual method will eventually go on the UInt64 type when it is created)
        {
#if FEATURE_READONLYSPAN
            return s != null ?
                ParseNumbers.StringToLong(s.AsSpan(), radix, ParseNumbers.IsTight | ParseNumbers.TreatAsUnsigned) :
                0;
#else
            return 0;
#endif
        }


        /////**
        //// * Parses the specified string as a signed long value using the specified
        //// * radix. The ASCII character \u002d ('-') is recognized as the minus sign.
        //// * 
        //// * @param string
        //// *            the string representation of a long value.
        //// * @param radix
        //// *            the radix to use when parsing.
        //// * @return the primitive long value represented by {@code string} using
        //// *         {@code radix}.
        //// * @throws NumberFormatException
        //// *             if {@code string} is {@code null} or has a length of zero,
        //// *             {@code radix < Character.MIN_RADIX},
        //// *             {@code radix > Character.MAX_RADIX}, or if {@code string}
        //// *             can not be parsed as a long value.
        //// */
        //////public static long Parse(string s, int radix) // J2N: Renamed from ParseLong()
        //////{
        //////    if (s is null || radix < Character.MinRadix
        //////            || radix > Character.MaxRadix)
        //////    {
        //////        throw new FormatException();
        //////    }
        //////    int length = s.Length, i = 0;
        //////    if (length == 0)
        //////    {
        //////        throw new FormatException(s);
        //////    }
        //////    bool negative = s[i] == '-';
        //////    if (negative && ++i == length)
        //////    {
        //////        throw new FormatException(s);
        //////    }

        //////    return Parse(s, i, radix, negative);
        //////}

        /// <summary>
        /// Parses the <see cref="string"/> argument as a signed long in the specified <paramref name="radix"/>, beginning at the
        /// specified <paramref name="startIndex"/> with the specified number of characters in <paramref name="length"/>. 
        /// <para/>
        /// Usage Note: This is similar to the <see cref="Convert.ToInt64(string?, int)"/> method, however it allows conversion
        /// of the value without allocating a substring. It also differs in that it allows the use of the ASCII character \u002d ('-')
        /// or \u002B ('+') in any <paramref name="radix"/>.
        /// </summary>
        /// <param name="s">The <see cref="string"/> containing the <see cref="long"/> representation to be parsed.</param>
        /// <param name="startIndex">The zero-based starting character position of a region in <paramref name="s"/>.</param>
        /// <param name="length">The number of characters in the region to parse.</param>
        /// <param name="radix">The radix (or base) to use when parsing <paramref name="s"/>. The value must be in the range
        /// <see cref="Character.MinRadix"/> - <see cref="Character.MaxRadix"/> inclusive.</param>
        /// <returns>The signed <see cref="long"/> represented by the subsequence in the specified <paramref name="radix"/>.</returns>
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
        /// represents a number that is less than <see cref="long.MinValue"/> or greater than <see cref="long.MaxValue"/>.
        /// </exception>
        /// <seealso cref="Parse(string, int)"/>
        public static long Parse(string s, int startIndex, int length, int radix)
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
            //return s != null ? ParseNumbers.StringToLong(s.Substring(startIndex, length).AsSpan(), radix, ParseNumbers.IsTight) : 0;
            return ParseNumbers.StringToLong(s.AsSpan(), radix, flags: ParseNumbers.NoSpace, sign: 1, ref startIndex, length);
#else
            return 0;
#endif
        }

        

        /// <summary>
        /// J2N TODO: Docs
        /// </summary>
        /// <param name="s"></param>
        /// <param name="radix"></param>
        /// <returns></returns>
        public static long Parse(string s, int radix) // J2N: Renamed from ParseLong()
        {
#if FEATURE_READONLYSPAN
            return s != null ?
                ParseNumbers.StringToLong(s.AsSpan(), radix, ParseNumbers.IsTight) :
                0;
#else
            return 0;
#endif

            //if (radix < Character.MinRadix || radix > Character.MaxRadix)
            //    throw new ArgumentOutOfRangeException(nameof(radix), SR.ArgumentOutOfRange_Radix);
            //if (s is null)
            //    return 0; // J2N: Match .NET behavior and return 0 when s is null

            ////if (s is null || radix < Character.MinRadix
            ////        || radix > Character.MaxRadix)
            ////{
            ////    throw new FormatException();
            ////}
            //int length = s.Length, i = 0;

            //// Get rid of the whitespace and then check that we've still got some digits to parse.
            //EatWhiteSpace(s, ref i);
            //if (i == length)
            //{
            //    throw new FormatException(SR.Format_EmptyInputString);
            //}

            //// Check for a sign
            //int sign = 1;
            //if (s[i] == '-')
            //{
            //    sign = -1;
            //    i++;
            //}
            //else if (s[i] == '+')
            //{
            //    i++;
            //}

            //// Consume the 0x if we're in base-16.
            //if ((radix == 16) && (i + 1 < length) && s[i] == '0')
            //{
            //    if (s[i + 1] == 'x' || s[i + 1] == 'X')
            //    {
            //        i += 2;
            //    }
            //}

            //// Check for no parseable digits
            //if (i == length)
            //{
            //    DotNetNumber.ThrowOverflowOrFormatException(DotNetNumber.ParsingStatus.Failed);
            //}

            //return Parse(s, offset: i, radix, negative: sign < 0);
        }

        /// <summary>
        /// J2N TODO: Docs
        /// </summary>
        /// <param name="s"></param>
        /// <param name="radix"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TryParse(string? s, int radix, out long result)
        {
            if (radix < Character.MinRadix || radix > Character.MaxRadix)
            {
                result = default;
                return false;
            }
            if (s is null)
            {
                result = 0; // J2N: Match .NET behavior and return 0 when s is null
                return true;
            }
            int length = s.Length, i = 0;

            // Get rid of the whitespace and then check that we've still got some digits to parse.
            EatWhiteSpace(s, ref i);
            if (i == length)
                if (length == 0)
            {
                result = default;
                return false;
            }

            // Check for a sign
            int sign = 1;
            if (s[i] == '-')
            {
                sign = -1;
                i++;
            }
            else if (s[i] == '+')
            {
                i++;
            }

            // Consume the 0x if we're in base-16.
            if ((radix == 16) && (i + 1 < length) && s[i] == '0')
            {
                if (s[i + 1] == 'x' || s[i + 1] == 'X')
                {
                    i += 2;
                }
            }

            // Check for no parseable digits
            if (i == length)
            {
                result = default;
                return false;
            }

            return TryParse(s, offset: i, radix, negative: sign < 0, out result);
        }

#if FEATURE_READONLYSPAN
        //public static bool TryParse(ReadOnlySpan<char> s, int radix, out long result)
        //{
        //    if (s.Length == 0 || radix < Character.MinRadix
        //        || radix > Character.MaxRadix)
        //    {
        //        result = default;
        //        return false;
        //    }
        //    unsafe
        //    {
        //        fixed (char* stringPointer = &MemoryMarshal.GetReference(value))
        //        {
        //            char* p = stringPointer;
        //            if (!TryParseNumber(ref p, p + value.Length, styles, ref number, info)
        //                || ((int)(p - stringPointer) < value.Length && !TrailingZeros(value, (int)(p - stringPointer))))
        //            {
        //                number.CheckConsistency();
        //                return false;
        //            }
        //        }
        //    }
        //}

        //private static unsafe bool TryParseNumber(ReadOnlySpan<char> s, int radix, out long result)
        //{
        //    fixed (char* stringPointer = &MemoryMarshal.GetReference(value))
        //    {
        //        char* p = stringPointer;
        //        if (!TryParseNumber(ref p, p + value.Length, styles, ref number, info)
        //            || ((int)(p - stringPointer) < value.Length && !TrailingZeros(value, (int)(p - stringPointer))))
        //        {
        //            number.CheckConsistency();
        //            return false;
        //        }
        //    }
        //}

        private static void EatWhiteSpace(ReadOnlySpan<char> s, ref int i)
        {
            int localIndex = i;
            for (; localIndex < s.Length && char.IsWhiteSpace(s[localIndex]); localIndex++) ;
            i = localIndex;
        }
#endif

        private static void EatWhiteSpace(string s, ref int i)
        {
            int localIndex = i;
            for (; localIndex < s.Length && char.IsWhiteSpace(s[localIndex]); localIndex++) ;
            i = localIndex;
        }

        //private static long Parse(string value, int offset, int radix,
        //        bool negative)
        //{
        //    long length = value.Length;
        //    if (!negative)
        //    {
        //        long max = long.MinValue / radix;
        //        long result = 0;
        //        while (offset < length)
        //        {
        //            int digit = Character.Digit(value[offset++], radix);
        //            if (digit == -1)
        //            {
        //                DotNetNumber.ThrowOverflowOrFormatException(DotNetNumber.ParsingStatus.Failed); // Extra junk at the end of the string
        //            }

        //            if (max > result)
        //            {
        //                //throw new FormatException(value);
        //                DotNetNumber.ThrowOverflowException(TypeCode.Int64); // J2N: Match .NET behavior here
        //            }
        //            long next = result * radix - digit;
        //            if (next > result)
        //            {
        //                //throw new FormatException(value);
        //                DotNetNumber.ThrowOverflowException(TypeCode.Int64); // J2N: Match .NET behavior here
        //            }
        //            result = next;
        //        }

        //        result = -result;
        //        if (result < 0)
        //        {
        //            //throw new FormatException(value);
        //            DotNetNumber.ThrowOverflowException(TypeCode.Int64); // J2N: Match .NET behavior here
        //        }
        //        return result;
        //    }
        //    else
        //    {
        //        ulong result = 0;
        //        ulong maxVal = 0xffffffffffffffff / (uint)radix;
        //        while (offset < length)
        //        {
        //            int digit = Character.Digit(value[offset++], radix);
        //            if (digit == -1)
        //            {
        //                DotNetNumber.ThrowOverflowOrFormatException(DotNetNumber.ParsingStatus.Failed); // Extra junk at the end of the string
        //            }
        //            // Check for overflows - this is sufficient & correct.
        //            if (result > maxVal || ((long)result) < 0)
        //            {
        //                DotNetNumber.ThrowOverflowException(TypeCode.Int64);
        //            }

        //            result = result * (ulong)radix + (ulong)digit;
        //        }

        //        if ((long)result < 0 && result != 0x8000000000000000)
        //        {
        //            DotNetNumber.ThrowOverflowException(TypeCode.Int64);
        //        }
        //        return (long)result * -1;
        //    }
        //}

        private static long Parse(string value, int offset, int radix,
        bool negative)
        {
            long length = value.Length;
            if (negative)
            {
                long max = long.MinValue / radix;
                long result = 0;
                while (offset < length)
                {
                    int digit = Character.Digit(value[offset++], radix);
                    if (digit == -1)
                    {
                        DotNetNumber.ThrowOverflowOrFormatException(DotNetNumber.ParsingStatus.Failed); // Extra junk at the end of the string
                    }

                    if (max > result)
                    {
                        //throw new FormatException(value);
                        DotNetNumber.ThrowOverflowException(TypeCode.Int64); // J2N: Match .NET behavior here
                    }
                    long next = result * radix - digit;
                    if (next > result)
                    {
                        //throw new FormatException(value);
                        DotNetNumber.ThrowOverflowException(TypeCode.Int64); // J2N: Match .NET behavior here
                    }
                    result = next;
                }
                return result;
            }
            else
            {
                ulong result = 0;
                ulong maxVal = 0xffffffffffffffff / (uint)radix;
                while (offset < length)
                {
                    int digit = Character.Digit(value[offset++], radix);
                    if (digit == -1)
                    {
                        DotNetNumber.ThrowOverflowOrFormatException(DotNetNumber.ParsingStatus.Failed); // Extra junk at the end of the string
                    }
                    // Check for overflows - this is sufficient & correct.
                    if (result > maxVal || ((long)result) < 0)
                    {
                        DotNetNumber.ThrowOverflowException(TypeCode.Int64);
                    }

                    result = result * (ulong)radix + (ulong)digit;
                }

                if ((long)result < 0 && result != 0x8000000000000000)
                {
                    DotNetNumber.ThrowOverflowException(TypeCode.Int64);
                }
                return (long)result;
            }
        }

        //private static long Parse(string value, int offset, int radix,
        //        bool negative)
        //{
        //    long max = long.MinValue / radix;
        //    long result = 0, length = value.Length;
        //    while (offset < length)
        //    {
        //        int digit = Character.Digit(value[offset++], radix);
        //        if (digit == -1)
        //        {
        //            //throw new FormatException(value);
        //            DotNetNumber.ThrowOverflowOrFormatException(DotNetNumber.ParsingStatus.Failed); // Extra junk at the end of the string
        //        }
        //        if (max > result)
        //        {
        //            //throw new FormatException(value);
        //            DotNetNumber.ThrowOverflowException(TypeCode.Int64); // J2N: Match .NET behavior here
        //        }
        //        long next = result * radix - digit;
        //        if (next > result)
        //        {
        //            //throw new FormatException(value);
        //            DotNetNumber.ThrowOverflowException(TypeCode.Int64); // J2N: Match .NET behavior here
        //        }
        //        result = next;
        //    }
        //    // J2N: Test what JDK does when passed both a negative sign and setting bit 63 to 1 and duplicate behavior
        //    if (!negative)
        //    {
        //        result = -result;
        //        if (result < 0)
        //        {
        //            //throw new FormatException(value);
        //            DotNetNumber.ThrowOverflowException(TypeCode.Int64); // J2N: Match .NET behavior here
        //        }
        //    }
        //    return result;
        //}

        private static bool TryParse(string value, int offset, int radix,
                bool negative, out long result)
        {
            result = default;
            long max = long.MinValue / radix;
            long length = value.Length;
            while (offset < length)
            {
                int digit = Character.Digit(value[offset++], radix);
                if (digit == -1)
                {
                    return false; // Extra junk at the end of the string
                }
                if (max > result)
                {
                    return false;
                }
                long next = result * radix - digit;
                if (next > result)
                {
                    return false;
                }
                result = next;
            }
            if (!negative)
            {
                result = -result;
                if (result < 0)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Converts the string representation of a number to its 64-bit signed integer equivalent.
        /// A return value indicates whether the conversion succeeded.
        /// <para/>
        /// Usage Note: When porting from Java, note that this is a culture-sensitive method. Java uses the invariant
        /// culture in the Integer class, so call <see cref="TryParse(string?, NumberStyle, IFormatProvider?, out long)"/>
        /// and pass <see cref="NumberStyle.AllowLeadingSign"/> and <see cref="NumberFormatInfo.InvariantInfo"/> to exactly match the Java parsing behavior.
        /// <para/>
        /// Using that overload also allows you to normalize the exception behavior. The <see cref="Parse(string, IFormatProvider?)"/>
        /// method may throw <see cref="ArgumentNullException"/>, <see cref="OverflowException"/>, or <see cref="FormatException"/>,
        /// but Java only ever throws NumberFormatException in any of those cases. Using <see cref="TryParse(string?, NumberStyle, IFormatProvider?, out long)"/>
        /// means a <see cref="FormatException"/> can be thrown in any of those error condtions when it returns <c>false</c>.
        /// </summary>
        /// <param name="s">A string containing a number to convert.</param>
        /// <param name="result">When this method returns, contains the 64-bit signed integer value equivalent of
        /// the number contained in <paramref name="s"/>, if the conversion succeeded, or zero if the conversion failed.
        /// The conversion fails if the s parameter is <c>null</c> or <see cref="string.Empty"/>, is not of the correct format,
        /// or represents a number less than <see cref="long.MinValue"/> or greater than <see cref="long.MaxValue"/>. This parameter
        /// is passed uninitialized; any value originally supplied in <paramref name="result"/> will be overwritten.</param>
        /// <returns><c>true</c> if <paramref name="s"/> was converted successfully; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// The <see cref="TryParse(string?, out long)"/> method is like the <see cref="Parse(string, IFormatProvider?)"/> method,
        /// except the <see cref="TryParse(string?, out long)"/> method does not throw an exception if the
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
        /// information that can be present in <paramref name="s"/>, use the <see cref="TryParse(string?, NumberStyle, IFormatProvider?, out long)"/> method.
        /// <para/>
        /// The <paramref name="s"/> parameter is parsed using the formatting information in a <see cref="NumberFormatInfo"/>
        /// object initialized for the current system culture. For more information, see <see cref="NumberFormatInfo.CurrentInfo"/>.
        /// <para/>
        /// This overload of the <see cref="TryParse(string?, out long)"/> method interprets all digits in the <paramref name="s"/> parameter
        /// as decimal digits. To parse the string representation of a hexadecimal number, call the
        /// <see cref="TryParse(string?, NumberStyle, IFormatProvider?, out long)"/> overload.
        /// </remarks>
        /// <seealso cref="Parse(string, IFormatProvider?)"/>
        /// <seealso cref="Parse(string, long)"/>
        /// <seealso cref="Number.ToString()"/>
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool TryParse([NotNullWhen(true)] string? s, out long result)
        {
            return long.TryParse(s, out result);
        }

#if FEATURE_READONLYSPAN
        /// <summary>
        /// Converts the span representation of a number in a specified style and culture-specific format to its 64-bit signed
        /// integer equivalent. A return value indicates whether the conversion succeeded.
        /// <para/>
        /// Usage Note: When porting from Java, note that this is a culture-sensitive method. Java uses the invariant
        /// culture in the Integer class, so call <see cref="TryParse(ReadOnlySpan{char}, NumberStyle, IFormatProvider?, out long)"/>
        /// and pass <see cref="NumberStyle.AllowLeadingSign"/> and <see cref="NumberFormatInfo.InvariantInfo"/> to exactly match the Java parsing behavior.
        /// <para/>
        /// Using that overload also allows you to normalize the exception behavior. The <see cref="Parse(string, IFormatProvider?)"/>
        /// method may throw <see cref="ArgumentNullException"/>, <see cref="OverflowException"/>, or <see cref="FormatException"/>,
        /// but Java only ever throws NumberFormatException in any of those cases. Using <see cref="TryParse(ReadOnlySpan{char}, NumberStyle, IFormatProvider?, out long)"/>
        /// means a <see cref="FormatException"/> can be thrown in any of those error condtions when it returns <c>false</c>.
        /// </summary>
        /// <param name="s">A span containing the characters that represent the number to convert.</param>
        /// <param name="result">When this method returns, contains the 64-bit signed integer value equivalent of
        /// the number contained in <paramref name="s"/>, if the conversion succeeded, or zero if the conversion failed.
        /// The conversion fails if the s parameter is <c>null</c> or <see cref="string.Empty"/>, is not of the correct format,
        /// or represents a number less than <see cref="long.MinValue"/> or greater than <see cref="long.MaxValue"/>. This parameter
        /// is passed uninitialized; any value originally supplied in <paramref name="result"/> will be overwritten.</param>
        /// <returns><c>true</c> if <paramref name="s"/> was converted successfully; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// The <see cref="TryParse(ReadOnlySpan{char}, out long)"/> method is like the <see cref="Parse(ReadOnlySpan{char}, IFormatProvider?)"/> method,
        /// except the <see cref="TryParse(ReadOnlySpan{char}, out long)"/> method does not throw an exception if the
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
        /// information that can be present in <paramref name="s"/>, use the <see cref="TryParse(ReadOnlySpan{char}, NumberStyle, IFormatProvider?, out long)"/> method.
        /// <para/>
        /// The <paramref name="s"/> parameter is parsed using the formatting information in a <see cref="NumberFormatInfo"/>
        /// object initialized for the current system culture. For more information, see <see cref="NumberFormatInfo.CurrentInfo"/>.
        /// <para/>
        /// This overload of the <see cref="TryParse(ReadOnlySpan{char}, out long)"/> method interprets all digits in the <paramref name="s"/> parameter
        /// as decimal digits. To parse the string representation of a hexadecimal number, call the
        /// <see cref="TryParse(ReadOnlySpan{char}, NumberStyle, IFormatProvider?, out long)"/> overload.
        /// </remarks>
        /// <seealso cref="Parse(ReadOnlySpan{char}, IFormatProvider?)"/>
        /// <seealso cref="TryParse(string?, out long)"/>
        /// <seealso cref="Number.ToString()"/>
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool TryParse(ReadOnlySpan<char> s, out long result)
        {
            return long.TryParse(s, out result);
        }
#endif

        /// <summary>
        /// Converts the string representation of a number in a specified style and culture-specific format to its
        /// 64-bit signed integer equivalent. A return value indicates whether the conversion succeeded.
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
        /// <param name="result">When this method returns, contains the 64-bit signed integer value equivalent of
        /// the number contained in <paramref name="s"/>, if the conversion succeeded, or zero if the conversion failed.
        /// The conversion fails if the s parameter is <c>null</c> or <see cref="string.Empty"/>, is not of the correct format,
        /// or represents a number less than <see cref="long.MinValue"/> or greater than <see cref="long.MaxValue"/>. This parameter
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
        /// The <see cref="TryParse(string?, NumberStyle, IFormatProvider?, out long)"/> method is like the <see cref="Parse(string, NumberStyle, IFormatProvider?)"/> method,
        /// except the <see cref="TryParse(string?, NumberStyle, IFormatProvider?, out long)"/> method does not throw an exception if the
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
        /// <seealso cref="TryParse(string?, out long)"/>
        /// <seealso cref="Number.ToString()"/>
        /// <seealso cref="NumberStyle"/>
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool TryParse([NotNullWhen(true)] string? s, NumberStyle style, IFormatProvider? provider, out long result)
        {
            // J2N TODO: Support NumberStyle.AllowTypeSuffix ("l" or "L")
            return long.TryParse(s, (NumberStyles)style, provider, out result);
        }


#if FEATURE_READONLYSPAN
        /// <summary>
        /// Converts the span representation of a number in a specified style and culture-specific format to its 64-bit signed integer equivalent.
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
        /// <param name="result">When this method returns, contains the 64-bit signed integer value equivalent of
        /// the number contained in <paramref name="s"/>, if the conversion succeeded, or zero if the conversion failed.
        /// The conversion fails if the s parameter is <c>null</c> or <see cref="string.Empty"/>, is not of the correct format,
        /// or represents a number less than <see cref="long.MinValue"/> or greater than <see cref="long.MaxValue"/>. This parameter
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
        /// The <see cref="TryParse(ReadOnlySpan{char}, NumberStyle, IFormatProvider?, out long)"/> method is like the <see cref="Parse(ReadOnlySpan{char}, NumberStyle, IFormatProvider?)"/> method,
        /// except the <see cref="TryParse(ReadOnlySpan{char}, NumberStyle, IFormatProvider?, out long)"/> method does not throw an exception if the
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
        /// <seealso cref="TryParse(ReadOnlySpan{char}, out long)"/>
        /// <seealso cref="Number.ToString()"/>
        /// <seealso cref="NumberStyle"/>
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool TryParse(ReadOnlySpan<char> s, NumberStyle style, IFormatProvider? provider, out long result)
        {
            // J2N TODO: Support NumberStyle.AllowTypeSuffix ("l" or "L")
            return long.TryParse(s, (NumberStyles)style, provider, out result);
        }
#endif

        /// <inheritdoc/>
        public override short GetInt16Value()
        {
            return (short)value;
        }

        /**
         * Converts the specified long value into its binary string representation.
         * The returned string is a concatenation of '0' and '1' characters.
         * 
         * @param l
         *            the long value to convert.
         * @return the binary string representation of {@code l}.
         */
        public static string ToBinaryString(long l)
        {
            return l.ToBinaryString();

            //int count = 1;
            //long j = l;

            //if (l < 0)
            //{
            //    count = 64;
            //}
            //else
            //{
            //    while ((j >>= 1) != 0)
            //    {
            //        count++;
            //    }
            //}

            //char[] buffer = new char[count];
            //do
            //{
            //    buffer[--count] = (char)((l & 1) + '0');
            //    l >>= 1;
            //} while (count > 0);
            //return new String(0, buffer.length, buffer);
        }

        /**
         * Converts the specified long value into its hexadecimal string
         * representation. The returned string is a concatenation of characters from
         * '0' to '9' and 'a' to 'f'.
         * 
         * @param l
         *            the long value to convert.
         * @return the hexadecimal string representation of {@code l}.
         */
        public static string ToHexString(long l)
        {
            return l.ToHexString();

            //int count = 1;
            //long j = l;

            //if (l < 0)
            //{
            //    count = 16;
            //}
            //else
            //{
            //    while ((j >>= 4) != 0)
            //    {
            //        count++;
            //    }
            //}

            //char[] buffer = new char[count];
            //do
            //{
            //    int t = (int)(l & 15);
            //    if (t > 9)
            //    {
            //        t = t - 10 + 'a';
            //    }
            //    else
            //    {
            //        t += '0';
            //    }
            //    buffer[--count] = (char)t;
            //    l >>= 4;
            //} while (count > 0);
            //return new String(0, buffer.length, buffer);
        }

        /**
         * Converts the specified long value into its octal string representation.
         * The returned string is a concatenation of characters from '0' to '7'.
         * 
         * @param l
         *            the long value to convert.
         * @return the octal string representation of {@code l}.
         */
        public static string ToOctalString(long l)
        {
            return l.ToOctalString();

            //int count = 1;
            //long j = l;

            //if (l < 0)
            //{
            //    count = 22;
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
            //    buffer[--count] = (char)((l & 7) + '0');
            //    l >>>= 3;
            //} while (count > 0);
            //return new String(0, buffer.length, buffer);
        }

        //@Override
        //public string toString()
        //{
        //    return Long.toString(value);
        //}

        /// <inheritdoc/>
        public override string ToString(string? format, IFormatProvider? provider)
        {
            return ToString(format, provider, value);
        }

        /**
         * Converts the specified long value into its decimal string representation.
         * The returned string is a concatenation of a minus sign if the number is
         * negative and characters from '0' to '9'.
         * 
         * @param l
         *            the long to convert.
         * @return the decimal string representation of {@code l}.
         */
        public static string ToString(long l)
        {
            return ToString(l, 10);
        }

        /**
         * Converts the specified long value into a string representation based on
         * the specified radix. The returned string is a concatenation of a minus
         * sign if the number is negative and characters from '0' to '9' and 'a' to
         * 'z', depending on the radix. If {@code radix} is not in the interval
         * defined by {@code Character.MIN_RADIX} and {@code Character.MAX_RADIX}
         * then 10 is used as the base for the conversion.
         * 
         * @param l
         *            the long to convert.
         * @param radix
         *            the base to use for the conversion.
         * @return the string representation of {@code l}.
         */
        public static string ToString(long l, int radix)
        {
            if (radix < Character.MinRadix || radix > Character.MaxRadix)
            {
                radix = 10;
            }
            if (l == 0)
            {
                return "0"; //$NON-NLS-1$
            }

            int count = 2;
            long j = l;
            bool negative = l < 0;
            if (!negative)
            {
                count = 1;
                j = -l;
            }
            while ((l /= radix) != 0)
            {
                count++;
            }

            char[] buffer = new char[count];
            do
            {
                int ch = 0 - (int)(j % radix);
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

        ///**
        // * Parses the specified string as a signed decimal long value.
        // * 
        // * @param string
        // *            the string representation of a long value.
        // * @return a {@code Long} instance containing the long value represented by
        // *         {@code string}.
        // * @throws NumberFormatException
        // *             if {@code string} is {@code null}, has a length of zero or
        // *             can not be parsed as a long value.
        // * @see #parseLong(String)
        // */
        //public static Int64 ValueOf(string value)
        //{
        //    return ValueOf(Parse(value));
        //}

        /**
         * Parses the specified string as a signed decimal long value.
         * 
         * @param string
         *            the string representation of a long value.
         * @return a {@code Long} instance containing the long value represented by
         *         {@code string}.
         * @throws NumberFormatException
         *             if {@code string} is {@code null}, has a length of zero or
         *             can not be parsed as a long value.
         * @see #parseLong(String)
         */
        public static Int64 ValueOf(string value, IFormatProvider? provider)
        {
            return ValueOf(Parse(value, provider));
        }

        /**
         * Parses the specified string as a signed decimal long value.
         * 
         * @param string
         *            the string representation of a long value.
         * @return a {@code Long} instance containing the long value represented by
         *         {@code string}.
         * @throws NumberFormatException
         *             if {@code string} is {@code null}, has a length of zero or
         *             can not be parsed as a long value.
         * @see #parseLong(String)
         */
        public static Int64 ValueOf(string value, NumberStyle style, IFormatProvider? provider)
        {
            return ValueOf(Parse(value, style, provider));
        }

        /**
         * Parses the specified string as a signed long value using the specified
         * radix.
         * 
         * @param string
         *            the string representation of a long value.
         * @param radix
         *            the radix to use when parsing.
         * @return a {@code Long} instance containing the long value represented by
         *         {@code string} using {@code radix}.
         * @throws NumberFormatException
         *             if {@code string} is {@code null} or has a length of zero,
         *             {@code radix < Character.MIN_RADIX},
         *             {@code radix > Character.MAX_RADIX}, or if {@code string}
         *             can not be parsed as a long value.
         * @see #parseLong(String, int)
         */
        public static Int64 ValueOf(string value, int radix)
        {
            return ValueOf(Parse(value, radix));
        }

        /**
         * Determines the highest (leftmost) bit of the specified long value that is
         * 1 and returns the bit mask value for that bit. This is also referred to
         * as the Most Significant 1 Bit. Returns zero if the specified long is
         * zero.
         * 
         * @param lng
         *            the long to examine.
         * @return the bit mask indicating the highest 1 bit in {@code lng}.
         * @since 1.5
         */
        public static long HighestOneBit(long lng)
        {
            return lng.HighestOneBit();
            //lng |= (lng >> 1);
            //lng |= (lng >> 2);
            //lng |= (lng >> 4);
            //lng |= (lng >> 8);
            //lng |= (lng >> 16);
            //lng |= (lng >> 32);
            //return (lng & ~(lng >>> 1));
        }

        /**
         * Determines the lowest (rightmost) bit of the specified long value that is
         * 1 and returns the bit mask value for that bit. This is also referred to
         * as the Least Significant 1 Bit. Returns zero if the specified long is
         * zero.
         * 
         * @param lng
         *            the long to examine.
         * @return the bit mask indicating the lowest 1 bit in {@code lng}.
         * @since 1.5
         */
        public static long LowestOneBit(long lng)
        {
            return lng.LowestOneBit();
            //return (lng & (-lng));
        }

        /**
         * Determines the number of leading zeros in the specified long value prior
         * to the {@link #highestOneBit(long) highest one bit}.
         *
         * @param lng
         *            the long to examine.
         * @return the number of leading zeros in {@code lng}.
         * @since 1.5
         */
        public static int NumberOfLeadingZeros(long lng)
        {
            return lng.LeadingZeroCount();
            //lng |= lng >> 1;
            //lng |= lng >> 2;
            //lng |= lng >> 4;
            //lng |= lng >> 8;
            //lng |= lng >> 16;
            //lng |= lng >> 32;
            //return bitCount(~lng);
        }

        /**
         * Determines the number of trailing zeros in the specified long value after
         * the {@link #lowestOneBit(long) lowest one bit}.
         *
         * @param lng
         *            the long to examine.
         * @return the number of trailing zeros in {@code lng}.
         * @since 1.5
         */
        public static int NumberOfTrailingZeros(long lng)
        {
            return lng.TrailingZeroCount();
            //return bitCount((lng & -lng) - 1);
        }

        /**
         * Counts the number of 1 bits in the specified long value; this is also
         * referred to as population count.
         *
         * @param lng
         *            the long to examine.
         * @return the number of 1 bits in {@code lng}.
         * @since 1.5
         */
        public static int BitCount(long lng)
        {
            return lng.PopCount();
            //lng = (lng & 0x5555555555555555L) + ((lng >> 1) & 0x5555555555555555L);
            //lng = (lng & 0x3333333333333333L) + ((lng >> 2) & 0x3333333333333333L);
            //// adjust for 64-bit integer
            //int i = (int)((lng >>> 32) + lng);
            //i = (i & 0x0F0F0F0F) + ((i >> 4) & 0x0F0F0F0F);
            //i = (i & 0x00FF00FF) + ((i >> 8) & 0x00FF00FF);
            //i = (i & 0x0000FFFF) + ((i >> 16) & 0x0000FFFF);
            //return i;
        }

        /**
         * Rotates the bits of the specified long value to the left by the specified
         * number of bits.
         *
         * @param lng
         *            the long value to rotate left.
         * @param distance
         *            the number of bits to rotate.
         * @return the rotated value.
         * @since 1.5
         */
        public static long RotateLeft(long lng, int distance)
        {
            return lng.RotateLeft(distance);
            //if (distance == 0)
            //{
            //    return lng;
            //}
            ///*
            // * According to JLS3, 15.19, the right operand of a shift is always
            // * implicitly masked with 0x3F, which the negation of 'distance' is
            // * taking advantage of.
            // */
            //return ((lng << distance) | (lng >>> (-distance)));
        }

        /**
         * <p>
         * Rotates the bits of the specified long value to the right by the
         * specified number of bits.
         *
         * @param lng
         *            the long value to rotate right.
         * @param distance
         *            the number of bits to rotate.
         * @return the rotated value.
         * @since 1.5
         */
        public static long RotateRight(long lng, int distance)
        {
            return lng.RotateRight(distance);
            //if (distance == 0)
            //{
            //    return lng;
            //}
            ///*
            // * According to JLS3, 15.19, the right operand of a shift is always
            // * implicitly masked with 0x3F, which the negation of 'distance' is
            // * taking advantage of.
            // */
            //return ((lng >>> distance) | (lng << (-distance)));
        }

        /**
         * Reverses the order of the bytes of the specified long value.
         * 
         * @param lng
         *            the long value for which to reverse the byte order.
         * @return the reversed value.
         * @since 1.5
         */
        public static long ReverseBytes(long lng)
        {
            return lng.ReverseBytes();
            //long b7 = lng >>> 56;
            //long b6 = (lng >>> 40) & 0xFF00L;
            //long b5 = (lng >>> 24) & 0xFF0000L;
            //long b4 = (lng >>> 8) & 0xFF000000L;
            //long b3 = (lng & 0xFF000000L) << 8;
            //long b2 = (lng & 0xFF0000L) << 24;
            //long b1 = (lng & 0xFF00L) << 40;
            //long b0 = lng << 56;
            //return (b0 | b1 | b2 | b3 | b4 | b5 | b6 | b7);
        }

        /**
         * Reverses the order of the bits of the specified long value.
         * 
         * @param lng
         *            the long value for which to reverse the bit order.
         * @return the reversed value.
         * @since 1.5
         */
        public static long Reverse(long lng)
        {
            return lng.Reverse();
            //// From Hacker's Delight, 7-1, Figure 7-1
            //lng = (lng & 0x5555555555555555L) << 1 | (lng >> 1)
            //        & 0x5555555555555555L;
            //lng = (lng & 0x3333333333333333L) << 2 | (lng >> 2)
            //        & 0x3333333333333333L;
            //lng = (lng & 0x0F0F0F0F0F0F0F0FL) << 4 | (lng >> 4)
            //        & 0x0F0F0F0F0F0F0F0FL;
            //return reverseBytes(lng);
        }

        /**
         * Returns the value of the {@code signum} function for the specified long
         * value.
         * 
         * @param lng
         *            the long value to check.
         * @return -1 if {@code lng} is negative, 1 if {@code lng} is positive, 0 if
         *         {@code lng} is zero.
         * @since 1.5
         */
        public static int Signum(long lng)
        {
            return lng.Signum();
            //return (lng == 0 ? 0 : (lng < 0 ? -1 : 1));
        }

        /**
         * Returns a {@code Long} instance for the specified long value.
         * <p>
         * If it is not necessary to get a new {@code Long} instance, it is
         * recommended to use this method instead of the constructor, since it
         * maintains a cache of instances which may result in better performance.
         *
         * @param lng
         *            the long value to store in the instance.
         * @return a {@code Long} instance containing {@code lng}.
         * @since 1.5
         */
        public static Int64 ValueOf(long lng)
        {
            if (lng < -128 || lng > 127)
            {
                return new Int64(lng);
            }
            return ValueOfCache.Cache[128 + (int)lng];
        }

        static class ValueOfCache
        {
            /**
             * <p>
             * A cache of instances used by {@link Long#valueOf(long)} and auto-boxing.
             */
            internal static readonly Int64[] Cache = LoadCache();

            private static Int64[] LoadCache()
            {
                var cache = new Int64[256];
                for (int i = -128; i <= 127; i++)
                    cache[i + 128] = new Int64(i);
                return cache;
            }

        }


        // J2N: Support implicit conversion

        /// <inheritdoc/>
        public static implicit operator long(Int64 value) => value.value;
        /// <inheritdoc/>
        public static implicit operator Int64(long value) => ValueOf(value);
    }
}
