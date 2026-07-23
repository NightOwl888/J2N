using J2N.CodeGeneration;
using System;
using System.ComponentModel;
using System.Globalization;

namespace J2N.Text
{
    internal partial class MutableTextBuffer
    {
        /// <summary>
        /// culture-aware default behavior. Particularly useful on APIs that do not
        /// accept <see cref="IFormatProvider"/>, <see cref="StringComparison"/> or other
        /// culture-aware settings.
        /// </summary>
        internal bool useInvariantDefaults = false;

        /// <summary>
        /// Gets or sets a value indicating whether culture-sensitive operations use
        /// invariant defaults when the caller does not explicitly specify culture-
        /// specific behavior.
        /// </summary>
        /// <value>
        /// <see langword="false"/> to use the .NET default behavior of using the current
        /// culture for culture-sensitive operations; <see langword="true"/> to use
        /// invariant defaults instead. The default is <see langword="false"/>.
        /// </value>
        /// <remarks>
        /// This setting affects culture-sensitive operations that rely on default
        /// formatting, parsing, casing, comparison, or other culture-specific behavior
        /// when the caller does not explicitly provide a culture, format provider,
        /// comparison option, or equivalent setting.
        /// <para/>
        /// Setting this property to <see langword="true"/> is recommended when porting
        /// Java applications that expect locale-independent behavior. Java APIs commonly
        /// use locale-independent defaults for operations such as numeric formatting,
        /// whereas .NET APIs generally use the current culture by default.
        /// <para/>
        /// This setting has no effect on operations where the caller explicitly supplies
        /// the culture-specific option to use, such as an
        /// <see cref="IFormatProvider"/>, <see cref="CultureInfo"/>, or
        /// <see cref="StringComparison"/> value.
        /// </remarks>
        [CodeGenerationIgnore]
        public bool UseInvariantDefaults
        {
            get => useInvariantDefaults;
            init => useInvariantDefaults = value;
        }

        /// <summary>
        /// Gets the default <see cref="CultureInfo"/> instance for the current <see cref="MutableTextBuffer"/>.
        /// <para/>
        /// This value will be respected as the default setting when the caller does not provide a choice or
        /// is calling a culture-sensitve API without an option to override culture.
        /// </summary>
        /// <remarks>
        /// Subclasses may honor this setting or ignore it.
        /// </remarks>
        [CodeGenerationIgnore]
        public CultureInfo DefaultCulture => useInvariantDefaults ? CultureInfo.InvariantCulture : CultureInfo.CurrentCulture;

        /// <summary>
        /// Gets the default <see cref="NumberFormatInfo"/> instance for the current <see cref="MutableTextBuffer"/>.
        /// <para/>
        /// This value will be respected as the default setting when the caller does not provide a choice or
        /// is calling a culture-sensitve API without an option to override culture.
        /// </summary>
        /// <remarks>
        /// Subclasses may honor this setting or ignore it.
        /// </remarks>
        [CodeGenerationIgnore]
        public NumberFormatInfo DefaultNumberFormatInfo => useInvariantDefaults ? NumberFormatInfo.InvariantInfo : NumberFormatInfo.CurrentInfo;

        /// <summary>
        /// Gets the default <see cref="DateTimeFormatInfo"/> instance for the current <see cref="MutableTextBuffer"/>.
        /// <para/>
        /// This value will be respected as the default setting when the caller does not provide a choice or
        /// is calling a culture-sensitve API without an option to override culture.
        /// </summary>
        /// <remarks>
        /// Subclasses may honor this setting or ignore it.
        /// </remarks>
        [CodeGenerationIgnore]
        public DateTimeFormatInfo DefaultDateTimeFormatInfo => useInvariantDefaults ? DateTimeFormatInfo.InvariantInfo : DateTimeFormatInfo.CurrentInfo;

        /// <summary>
        /// Gets the default <see cref="StringFormatter"/> instance for the current <see cref="MutableTextBuffer"/>.
        /// <para/>
        /// This value will be respected as the default setting when the caller does not provide a choice or
        /// is calling a culture-sensitve API without an option to override culture.
        /// </summary>
        /// <remarks>
        /// Subclasses may honor this setting or ignore it.
        /// </remarks>
        [CodeGenerationIgnore]
        public StringFormatter DefaultStringFormatter => useInvariantDefaults ? StringFormatter.InvariantCulture : StringFormatter.CurrentCulture;

        /// <summary>
        /// Gets the default <see cref="StringComparison"/> setting for the current <see cref="MutableTextBuffer"/>.
        /// <para/>
        /// This value will be respected as the default setting when the caller does not provide a choice or
        /// is calling a culture-sensitve API without an option to override culture.
        /// </summary>
        /// <remarks>
        /// Subclasses may honor this setting or ignore it.
        /// </remarks>
        [CodeGenerationIgnore]
        public StringComparison DefaultStringComparison => useInvariantDefaults ? StringComparison.Ordinal : StringComparison.CurrentCulture;
    }
}
