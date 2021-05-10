using System;
using System.Collections.Generic;
using System.Linq;
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

        /**
         * Constructs a new {@code Byte} from the specified string.
         * 
         * @param string
         *            the string representation of a single byte value.
         * @throws NumberFormatException
         *             if {@code string} can not be decoded into a byte value.
         * @see #parseByte(String)
         */
        public Byte(string stringValue)
            : this(ParseByte(stringValue))
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
        public static byte ParseByte(string value) // J2N TODO: Rename Parse() - Byte seems redundant here
        {
            //int intValue = Convert.ToInt32(value, 10); //Int32.ParseInt32(value);
            int intValue = Int32.ParseInt32(value);
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
        public static byte ParseByte(string value, int radix) // J2N TODO: Rename Parse() - Byte seems redundant here
        {
            int intValue = Int32.ParseInt32(value, radix);
            byte result = (byte)intValue;
            if (result == intValue || (sbyte)result == intValue) // J2N: Allow negative sbyte values for compatibility, even though we return byte rather than sbyte
            {
                return result;
            }
            throw new FormatException();
        }

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
        public static Byte ValueOf(string value)
        {
            return ValueOf(ParseByte(value));
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
        /// <param name="value">The string representation of a single <see cref="byte"/> or <see cref="sbyte"/> value.</param>
        /// <param name="radix">The radix to use when parsing. This is the same as <c>fromBase</c> in <see cref="Convert.ToInt32(string?, int)"/>,
        /// except the range is expanded from <see cref="Character.MinRadix"/> to <see cref="Character.MaxRadix"/>, whereas <see cref="Convert.ToInt32(string?, int)"/>
        /// only supports 2, 8, 10, or 16.</param>
        /// <returns>A <see cref="Byte"/> instance containing the <see cref="byte"/> value represented by
        /// <paramref name="value"/> using <paramref name="radix"/>.</returns>
        /// <exception cref="FormatException">
        /// <paramref name="value"/> is <c>null</c> or has a length of zero.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="radix"/> is less than <see cref="Character.MinRadix"/> or greater than <see cref="Character.MaxRadix"/>.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="value"/> cannot be parsed as a <see cref="byte"/> or <see cref="sbyte"/> value.
        /// </exception>
        /// <seealso cref="ParseByte(string, int)"/>
        public static Byte ValueOf(string value, int radix) // J2N TODO: Exception handling - throw ArgumentOutOfRangeException and ArgumentNullException? Accept null like Convert.ToInt32() does?
        {
            return ValueOf(ParseByte(value, radix));
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
