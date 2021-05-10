using J2N.Text;
using System;
using System.Globalization;

namespace J2N.Numerics
{
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
        /// <summary>
        /// Returns this object's value as a <see cref="byte"/>. Might involve rounding and/or
        /// truncating the value, so it fits into a <see cref="byte"/>.
        /// </summary>
        /// <returns>the primitive <see cref="byte"/> value of this object.</returns>
        public virtual byte GetByteValue() // J2N TODO: Rename To...() and drop "value"
        {
            return (byte)GetInt32Value();
        }

        /// <summary>
        /// Returns this object's value as a <see cref="double"/>. Might involve rounding.
        /// </summary>
        /// <returns>the primitive <see cref="double"/> value of this object.</returns>
        public abstract double GetDoubleValue(); // J2N TODO: Rename To...() and drop "value"

        /// <summary>
        /// Returns this object's value as a <see cref="float"/>. Might involve rounding.
        /// </summary>
        /// <returns>the primitive <see cref="float"/> value of this object.</returns>
        public abstract float GetSingleValue(); // J2N TODO: Rename To...() and drop "value"

        /// <summary>
        /// Returns this object's value as an <see cref="int"/>. Might involve rounding and/or
        /// truncating the value, so it fits into an <see cref="int"/>.
        /// </summary>
        /// <returns>the primitive <see cref="int"/> value of this object.</returns>
        public abstract int GetInt32Value(); // J2N TODO: Rename To...() and drop "value"

        /// <summary>
        /// Returns this object's value as a <see cref="long"/>. Might involve rounding and/or
        /// truncating the value, so it fits into a <see cref="long"/>.
        /// </summary>
        /// <returns>the primitive <see cref="long"/> value of this object.</returns>
        public abstract long GetInt64Value(); // J2N TODO: Rename To...() and drop "value"

        /// <summary>
        /// Returns this object's value as a <see cref="short"/>. Might involve rounding and/or
        /// truncating the value, so it fits into a <see cref="short"/>.
        /// </summary>
        /// <returns>the primitive <see cref="short"/> value of this object.</returns>
        public virtual short GetInt16Value() // J2N TODO: Rename To...() and drop "value"
        {
            return (short)GetInt32Value();
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
    }
}
