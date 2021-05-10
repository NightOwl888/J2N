using System;

namespace J2N.Numerics
{
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

        ////    /**
        ////     * Constant for the number of bits needed to represent a {@code long} in
        ////     * two's complement form.
        ////     *
        ////     * @since 1.5
        ////     */
        ////    public static final int SIZE = 64;


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

        /**
         * Constructs a new {@code Long} from the specified string.
         * 
         * @param string
         *            the string representation of a long value.
         * @throws NumberFormatException
         *             if {@code string} can not be decoded into a long value.
         * @see #parseLong(String)
         */
        public Int64(string value)
            : this(ParseInt64(value))
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
                throw new FormatException();
            }
            char firstDigit = value[i];
            bool negative = firstDigit == '-';
            if (negative)
            {
                if (length == 1)
                {
                    throw new FormatException(value);
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
                        throw new FormatException(value);
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
                    throw new FormatException(value);
                }
                i++;
                @base = 16;
            }

            long result = Parse(value, i, @base, negative);
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
        public static long ParseInt64(string value)
        {
            return ParseInt64(value, 10);
        }

        /**
         * Parses the specified string as a signed long value using the specified
         * radix. The ASCII character \u002d ('-') is recognized as the minus sign.
         * 
         * @param string
         *            the string representation of a long value.
         * @param radix
         *            the radix to use when parsing.
         * @return the primitive long value represented by {@code string} using
         *         {@code radix}.
         * @throws NumberFormatException
         *             if {@code string} is {@code null} or has a length of zero,
         *             {@code radix < Character.MIN_RADIX},
         *             {@code radix > Character.MAX_RADIX}, or if {@code string}
         *             can not be parsed as a long value.
         */
        public static long ParseInt64(string value, int radix)
        {
            if (value is null || radix < Character.MinRadix
                    || radix > Character.MaxRadix)
            {
                throw new FormatException();
            }
            int length = value.Length, i = 0;
            if (length == 0)
            {
                throw new FormatException(value);
            }
            bool negative = value[i] == '-';
            if (negative && ++i == length)
            {
                throw new FormatException(value);
            }

            return Parse(value, i, radix, negative);
        }

        private static long Parse(string value, int offset, int radix,
                bool negative)
        {
            long max = long.MinValue / radix;
            long result = 0, length = value.Length;
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
                long next = result * radix - digit;
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
        public static Int64 ValueOf(string value)
        {
            return ValueOf(ParseInt64(value));
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
            return ValueOf(ParseInt64(value, radix));
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
    }
}
