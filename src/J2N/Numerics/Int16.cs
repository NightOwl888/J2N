using J2N.Globalization;
using System;
using System.Globalization;

namespace J2N.Numerics
{
    /// <inheritdoc/>
    public sealed class Int16 : Number, IComparable<Int16>
    {
        /**
     * The value which the receiver represents.
     */
        private readonly short value;

        /////**
        //// * Constant for the maximum {@code short} value, 2<sup>15</sup>-1.
        //// */
        ////public static final short MAX_VALUE = (short)0x7FFF;

        /////**
        //// * Constant for the minimum {@code short} value, -2<sup>15</sup>.
        //// */
        ////public static final short MIN_VALUE = (short)0x8000;

        /**
         * Constant for the number of bits needed to represent a {@code short} in
         * two's complement form.
         *
         * @since 1.5
         */
        public const int SIZE = 16; // J2N: Rename BitCount?

        //    /**
        //     * The {@link Class} object that represents the primitive type {@code
        //     * short}.
        //     */
        //    @SuppressWarnings("unchecked")
        //public static final Class<Short> TYPE = (Class<Short>)new short[0]
        //        .getClass().getComponentType();

        // Note: This can't be set to "short.class", since *that* is
        // defined to be "java.lang.Short.TYPE";


        ///**
        // * Constructs a new {@code Short} from the specified string.
        // *
        // * @param string
        // *            the string representation of a short value.
        // * @throws NumberFormatException
        // *             if {@code string} can not be decoded into a short value.
        // * @see #parseShort(String)
        // */
        //public Int16(string value)
        //    : this(Parse(value))
        //{
        //}

        /**
         * Constructs a new {@code Short} from the specified string.
         *
         * @param string
         *            the string representation of a short value.
         * @throws NumberFormatException
         *             if {@code string} can not be decoded into a short value.
         * @see #parseShort(String)
         */
        public Int16(string s, IFormatProvider? provider)
            : this(Parse(s, provider))
        {
        }

        /**
         * Constructs a new {@code Short} from the specified string.
         *
         * @param string
         *            the string representation of a short value.
         * @throws NumberFormatException
         *             if {@code string} can not be decoded into a short value.
         * @see #parseShort(String)
         */
        public Int16(string s, NumberStyle style, IFormatProvider? provider)
            : this(Parse(s, style, provider))
        {
        }

        /**
         * Constructs a new {@code Short} with the specified primitive short value.
         *
         * @param value
         *            the primitive short value to store in the new instance.
         */
        public Int16(short value)
        {
            this.value = value;
        }

        /// <inheritdoc/>
        public override byte GetByteValue()
        {
            return (byte)value;
        }

        /**
         * Compares this object to the specified short object to determine their
         * relative order.
         * 
         * @param object
         *            the short object to compare this object to.
         * @return a negative value if the value of this short is less than the
         *         value of {@code object}; 0 if the value of this short and the
         *         value of {@code object} are equal; a positive value if the value
         *         of this short is greater than the value of {@code object}.
         * @throws NullPointerException
         *             if {@code object} is null.
         * @see java.lang.Comparable
         * @since 1.2
         */
        public int CompareTo(Int16? other)
        {
            if (other is null) return 1; // Using 1 if other is null as specified here: https://stackoverflow.com/a/4852537

            return value > other.value ? 1 : (value < other.value ? -1 : 0);
        }

        /**
         * Parses the specified string and returns a {@code Short} instance if the
         * string can be decoded into a short value. The string may be an optional
         * minus sign "-" followed by a hexadecimal ("0x..." or "#..."), octal
         * ("0..."), or decimal ("...") representation of a short.
         *
         * @param string
         *            a string representation of a short value.
         * @return a {@code Short} containing the value represented by
         *         {@code string}.
         * @throws NumberFormatException
         *             if {@code string} can not be parsed as a short value.
         */
        public static Int16 Decode(string value)
        {
            int intValue = Int32.Decode(value).GetInt32Value();
            short result = (short)intValue;
            if (result == intValue)
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
         * Compares this instance with the specified object and indicates if they
         * are equal. In order to be equal, {@code object} must be an instance of
         * {@code Short} and have the same short value as this object.
         *
         * @param object
         *            the object to compare this short with.
         * @return {@code true} if the specified object is equal to this
         *         {@code Short}; {@code false} otherwise.
         */
        public override bool Equals(object? obj)
        {
            return (obj is Int16)

                        && (value == ((Int16)obj).value);
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

        /////**
        //// * Parses the specified string as a signed decimal short value. The ASCII
        //// * character \u002d ('-') is recognized as the minus sign.
        //// *
        //// * @param string
        //// *            the string representation of a short value.
        //// * @return the primitive short value represented by {@code string}.
        //// * @throws NumberFormatException
        //// *             if {@code string} is {@code null}, has a length of zero or
        //// *             can not be parsed as a short value.
        //// */
        ////public static short Parse(string value)
        ////{
        ////    return Parse(value, 10);
        ////}

        /**
         * Parses the specified string as a signed decimal short value. The ASCII
         * character \u002d ('-') is recognized as the minus sign.
         *
         * @param string
         *            the string representation of a short value.
         * @return the primitive short value represented by {@code string}.
         * @throws NumberFormatException
         *             if {@code string} is {@code null}, has a length of zero or
         *             can not be parsed as a short value.
         */
        public static short Parse(string s, IFormatProvider? provider) // J2N: Renamed from ParseShort()
        {
            return short.Parse(s, provider);
        }

        /**
         * Parses the specified string as a signed decimal short value. The ASCII
         * character \u002d ('-') is recognized as the minus sign.
         *
         * @param string
         *            the string representation of a short value.
         * @return the primitive short value represented by {@code string}.
         * @throws NumberFormatException
         *             if {@code string} is {@code null}, has a length of zero or
         *             can not be parsed as a short value.
         */
        public static short Parse(string s, NumberStyle style, IFormatProvider? provider) // J2N: Renamed from ParseShort()
        {
            return short.Parse(s, (NumberStyles)style, provider);
        }


        /**
         * Parses the specified string as a signed short value using the specified
         * radix. The ASCII character \u002d ('-') is recognized as the minus sign.
         *
         * @param string
         *            the string representation of a short value.
         * @param radix
         *            the radix to use when parsing.
         * @return the primitive short value represented by {@code string} using
         *         {@code radix}.
         * @throws NumberFormatException
         *             if {@code string} is {@code null} or has a length of zero,
         *             {@code radix < Character.MIN_RADIX},
         *             {@code radix > Character.MAX_RADIX}, or if {@code string}
         *             can not be parsed as a short value.
         */
        public static short Parse(string s, int radix) // J2N: Renamed from ParseShort()
        {
            int intValue = Int32.Parse(s, radix);
            short result = (short)intValue;
            if (result == intValue)
            {
                return result;
            }
            throw new FormatException();
        }

        /**
         * Gets the primitive value of this short.
         *
         * @return this object's primitive value.
         */
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
         * specified short value with radix 10.
         *
         * @param value
         *             the short to convert to a string.
         * @return a printable representation of {@code value}.
         */
        public static string ToString(short value)
        {
            return Int32.ToString(value);
        }

        /**
 * Returns a string containing a concise, human-readable description of the
 * specified short value with radix 10.
 *
 * @param value
 *             the short to convert to a string.
 * @return a printable representation of {@code value}.
 */
        public static string ToString(short value, IFormatProvider? provider)
        {
            return Int32.ToString(value, provider);
        }

        ///**
        // * Parses the specified string as a signed decimal short value.
        // *
        // * @param string
        // *            the string representation of a short value.
        // * @return a {@code Short} instance containing the short value represented
        // *         by {@code string}.
        // * @throws NumberFormatException
        // *             if {@code string} is {@code null}, has a length of zero or
        // *             can not be parsed as a short value.
        // * @see #parseShort(String)
        // */
        //public static Int16 ValueOf(string value)
        //{
        //    return ValueOf(Parse(value));
        //}

        /**
         * Parses the specified string as a signed decimal short value.
         *
         * @param string
         *            the string representation of a short value.
         * @return a {@code Short} instance containing the short value represented
         *         by {@code string}.
         * @throws NumberFormatException
         *             if {@code string} is {@code null}, has a length of zero or
         *             can not be parsed as a short value.
         * @see #parseShort(String)
         */
        public static Int16 ValueOf(string value, IFormatProvider? provider)
        {
            return ValueOf(Parse(value, provider));
        }

        /**
         * Parses the specified string as a signed decimal short value.
         *
         * @param string
         *            the string representation of a short value.
         * @return a {@code Short} instance containing the short value represented
         *         by {@code string}.
         * @throws NumberFormatException
         *             if {@code string} is {@code null}, has a length of zero or
         *             can not be parsed as a short value.
         * @see #parseShort(String)
         */
        public static Int16 ValueOf(string value, NumberStyle style, IFormatProvider? provider)
        {
            return ValueOf(Parse(value, style, provider));
        }

        /**
         * Parses the specified string as a signed short value using the specified
         * radix.
         *
         * @param string
         *            the string representation of a short value.
         * @param radix
         *            the radix to use when parsing.
         * @return a {@code Short} instance containing the short value represented
         *         by {@code string} using {@code radix}.
         * @throws NumberFormatException
         *             if {@code string} is {@code null} or has a length of zero,
         *             {@code radix < Character.MIN_RADIX},
         *             {@code radix > Character.MAX_RADIX}, or if {@code string}
         *             can not be parsed as a short value.
         * @see #parseShort(String, int)
         */
        public static Int16 ValueOf(string value, int radix)
        {
            return ValueOf(Parse(value, radix));
        }

        /**
         * Reverses the bytes of the specified short.
         * 
         * @param s
         *            the short value for which to reverse bytes.
         * @return the reversed value.
         * @since 1.5
         */
        public static short ReverseBytes(short s) // J2N TODO: Move implementation to BitOperation
        {

            int high = (s >> 8) & 0xFF;
            int low = (s & 0xFF) << 8;
            return (short)(low | high);
        }

        /**
         * Returns a {@code Short} instance for the specified short value.
         * <p>
         * If it is not necessary to get a new {@code Short} instance, it is
         * recommended to use this method instead of the constructor, since it
         * maintains a cache of instances which may result in better performance.
         *
         * @param s
         *            the short value to store in the instance.
         * @return a {@code Short} instance containing {@code s}.
         * @since 1.5
         */
        public static Int16 ValueOf(short s)
        {
            if (s < -128 || s > 127)
            {
                return new Int16(s);
            }
            return ValueOfCache.CACHE[s + 128];
        }

        private class ValueOfCache
        {
            /**
             * A cache of instances used by {@link Short#valueOf(short)} and auto-boxing.
             */
            internal static readonly Int16[] CACHE = LoadCache();

            private static Int16[] LoadCache()
            {
                var cache = new Int16[256];

                for (short i = -128; i <= 127; i++)
                {
                    cache[i + 128] = new Int16(i);
                }
                return cache;

            }
        }

        // J2N: Support implicit conversion

        /// <inheritdoc/>
        public static implicit operator short(Int16 value) => value.value;
        /// <inheritdoc/>
        public static implicit operator Int16(short value) => ValueOf(value);
    }
}
