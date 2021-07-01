using J2N.Text;
using System;
using System.Diagnostics;
using System.Globalization;

namespace J2N.Numerics
{
    using SR = J2N.Resources.Strings;

    /// <summary>
    /// The abstract superclass of the classes which represent numeric base types
    /// (that is <see cref="Byte"/>, <see cref="System.Int16"/>, <see cref="Int32"/>,
    /// <see cref="Int64"/>, <see cref="Single"/>, and <see cref="Double"/>).
    /// </summary>
#if FEATURE_SERIALIZABLE
    [Serializable]
#endif
    public abstract class Number : IFormattable
    {
        // From System.Convert

        // A typeof operation is fairly expensive (does a system call), so we'll cache these here
        // statically.  These are exactly lined up with the TypeCode, eg. ConvertType[TypeCode.Int16]
        // will give you the type of an short.
        internal static readonly Type?[] ConvertTypes = {
            null, //Type.GetType("System.Empty"), //typeof(System.Empty), // J2N: This type is internal, but isn't currently being used, anyway, so rather than using Reflection, we are setting to null for now
            typeof(object),
            typeof(System.DBNull),
            typeof(bool),
            typeof(char),
            typeof(sbyte),
            typeof(byte),
            typeof(short),
            typeof(ushort),
            typeof(int),
            typeof(uint),
            typeof(long),
            typeof(ulong),
            typeof(float),
            typeof(double),
            typeof(decimal),
            typeof(DateTime),
            typeof(object), // TypeCode is discontinuous so we need a placeholder.
            typeof(string)
        };

        // Need to special case Enum because typecode will be underlying type, e.g. Int32
        private static readonly Type EnumType = typeof(Enum);


        /// <summary>
        /// Returns this object's value as a <see cref="byte"/>. Might involve rounding and/or
        /// truncating the value, so it fits into a <see cref="byte"/>.
        /// <para/>
        /// Usage Note: This is similar to byteValue() in the JDK, however for an exact match
        /// use <see cref="ToSByte()"/>.
        /// </summary>
        /// <returns>the primitive <see cref="byte"/> value of this object.</returns>
        public virtual byte ToByte()
        {
            return (byte)ToInt32();
        }

        /// <summary>
        /// Returns this object's value as a <see cref="sbyte"/>. Might involve rounding and/or
        /// truncating the value, so it fits into a <see cref="sbyte"/>.
        /// <para/>
        /// Usage Note: This is the equivalent operation of byteValue() in the JDK.
        /// </summary>
        /// <returns>the primitive <see cref="sbyte"/> value of this object.</returns>
        [CLSCompliant(false)]
        public virtual sbyte ToSByte()
        {
            return (sbyte)ToInt32();
        }

        /// <summary>
        /// Returns this object's value as a <see cref="double"/>. Might involve rounding.
        /// <para/>
        /// Usage Note: This is the equivalent operation of doubleValue() in the JDK.
        /// </summary>
        /// <returns>the primitive <see cref="double"/> value of this object.</returns>
        public abstract double ToDouble();

        /// <summary>
        /// Returns this object's value as a <see cref="float"/>. Might involve rounding.
        /// <para/>
        /// Usage Note: This is the equivalent operation of floatValue() in the JDK.
        /// </summary>
        /// <returns>the primitive <see cref="float"/> value of this object.</returns>
        public abstract float ToSingle();

        /// <summary>
        /// Returns this object's value as an <see cref="int"/>. Might involve rounding and/or
        /// truncating the value, so it fits into an <see cref="int"/>.
        /// <para/>
        /// Usage Note: This is the equivalent operation of intValue() in the JDK.
        /// </summary>
        /// <returns>the primitive <see cref="int"/> value of this object.</returns>
        public abstract int ToInt32();

        /// <summary>
        /// Returns this object's value as a <see cref="long"/>. Might involve rounding and/or
        /// truncating the value, so it fits into a <see cref="long"/>.
        /// <para/>
        /// Usage Note: This is the equivalent operation of longValue() in the JDK.
        /// </summary>
        /// <returns>the primitive <see cref="long"/> value of this object.</returns>
        public abstract long ToInt64();

        /// <summary>
        /// Returns this object's value as a <see cref="short"/>. Might involve rounding and/or
        /// truncating the value, so it fits into a <see cref="short"/>.
        /// <para/>
        /// Usage Note: This is the equivalent operation of shortValue() in the JDK.
        /// </summary>
        /// <returns>the primitive <see cref="short"/> value of this object.</returns>
        public virtual short ToInt16()
        {
            return (short)ToInt32();
        }

        /// <summary>
        /// Converts the numeric value of this instance to its equivalent string representation.
        /// The format provider used is <see cref="StringFormatter.CurrentCulture"/> which contains formatting rules similar to Java.
        /// </summary>
        /// <returns>The string representation of the value of this instance.</returns>
        public override string ToString()
        {
            return ToString(null, StringFormatter.CurrentCulture);
        }

        /// <summary>
        /// Converts the numeric value of this instance to its equivalent string representation, using the specified <paramref name="format"/>.
        /// The format provider used is <see cref="StringFormatter.CurrentCulture"/> which contains formatting rules similar to Java.
        /// </summary>
        /// <param name="format">A standard or custom numeric format string.</param>
        /// <returns>The string representation of the value of this instance as specified by <paramref name="format"/>.</returns>
        /// <exception cref="FormatException"><paramref name="format"/> is invalid or not supported.</exception>
        public virtual string ToString(string? format)
        {
            return ToString(format, StringFormatter.CurrentCulture);
        }

        /// <summary>
        /// Converts the numeric value of this instance to its equivalent string representation using the
        /// specified culture-specific format information.
        /// </summary>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <returns>The string representation of the value of this instance as specified by <paramref name="provider"/>.</returns>
        public virtual string ToString(IFormatProvider? provider)
        {
            return ToString(null, provider);
        }

        /// <summary>
        /// Converts the numeric value of this instance to its equivalent string representation using the
        /// specified format and culture-specific format information.
        /// </summary>
        /// <param name="format">A standard or custom numeric format string.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <returns>The string representation of the value of this instance as specified by <paramref name="format"/> and <paramref name="provider"/>.</returns>
        /// <exception cref="FormatException"><paramref name="format"/> is invalid or not supported.</exception>
        public abstract string ToString(string? format, IFormatProvider? provider);

        internal string ToString(string? format, IFormatProvider? provider, IFormattable value)
        {
            // Fast path: For standard .NET formatting using cultures, call IFormattable.ToString() to eliminate
            // boxing associated with string.Format().
            if (provider is null || provider is CultureInfo || provider is NumberFormatInfo)
            {
                return value.ToString(format, provider);
            }
            // Built-in .NET numeric types don't support custom format providers, so we resort
            // to using string.Format with some hacky format conversion in order to support them.
            return string.Format(provider, format is null ? "{0}" : "{0:" + format + '}', value);
        }


        // From System.Convert
        internal static object DefaultToType(IConvertible value, Type targetType, IFormatProvider? provider)
        {
            Debug.Assert(value != null, "[Convert.DefaultToType]value!=null");
            if (targetType == null)
            {
                throw new ArgumentNullException(nameof(targetType));
            }

            if (ReferenceEquals(value!.GetType(), targetType))
            {
                return value;
            }

            if (ReferenceEquals(targetType, ConvertTypes[(int)TypeCode.Boolean]))
                return value.ToBoolean(provider);
            if (ReferenceEquals(targetType, ConvertTypes[(int)TypeCode.Char]))
                return value.ToChar(provider);
            if (ReferenceEquals(targetType, ConvertTypes[(int)TypeCode.SByte]))
                return value.ToSByte(provider);
            if (ReferenceEquals(targetType, ConvertTypes[(int)TypeCode.Byte]))
                return value.ToByte(provider);
            if (ReferenceEquals(targetType, ConvertTypes[(int)TypeCode.Int16]))
                return value.ToInt16(provider);
            if (ReferenceEquals(targetType, ConvertTypes[(int)TypeCode.UInt16]))
                return value.ToUInt16(provider);
            if (ReferenceEquals(targetType, ConvertTypes[(int)TypeCode.Int32]))
                return value.ToInt32(provider);
            if (ReferenceEquals(targetType, ConvertTypes[(int)TypeCode.UInt32]))
                return value.ToUInt32(provider);
            if (ReferenceEquals(targetType, ConvertTypes[(int)TypeCode.Int64]))
                return value.ToInt64(provider);
            if (ReferenceEquals(targetType, ConvertTypes[(int)TypeCode.UInt64]))
                return value.ToUInt64(provider);
            if (ReferenceEquals(targetType, ConvertTypes[(int)TypeCode.Single]))
                return value.ToSingle(provider);
            if (ReferenceEquals(targetType, ConvertTypes[(int)TypeCode.Double]))
                return value.ToDouble(provider);
            if (ReferenceEquals(targetType, ConvertTypes[(int)TypeCode.Decimal]))
                return value.ToDecimal(provider);
            if (ReferenceEquals(targetType, ConvertTypes[(int)TypeCode.DateTime]))
                return value.ToDateTime(provider);
            if (ReferenceEquals(targetType, ConvertTypes[(int)TypeCode.String]))
                return value.ToString(provider);
            if (ReferenceEquals(targetType, ConvertTypes[(int)TypeCode.Object]))
                return (object)value;
            // Need to special case Enum because typecode will be underlying type, e.g. Int32
            if (ReferenceEquals(targetType, EnumType))
                return (Enum)value;
            if (ReferenceEquals(targetType, ConvertTypes[(int)TypeCode.DBNull]))
                throw new InvalidCastException(SR.InvalidCast_DBNull);
            if (ReferenceEquals(targetType, ConvertTypes[(int)TypeCode.Empty]))
                throw new InvalidCastException(SR.InvalidCast_Empty);

            throw new InvalidCastException(J2N.SR.Format(SR.InvalidCast_FromTo, value.GetType().FullName, targetType.FullName));
        }
    }
}
