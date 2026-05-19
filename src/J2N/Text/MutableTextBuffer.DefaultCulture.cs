using J2N.CodeGeneration;
using System;
using System.ComponentModel;
using System.Globalization;

namespace J2N.Text
{
    public partial class MutableTextBuffer
    {
        /// <summary>
        /// culture-aware default behavior. Particularly useful on APIs that do not
        /// accept <see cref="IFormatProvider"/>, <see cref="StringComparison"/> or other
        /// culture-aware settings.
        /// </summary>
        internal bool useInvariantDefaults = false;

        /// <summary>
        /// Gets or sets a flag indicating to use invariant default settings when not otherwise specified by the user.
        /// This setting affects culture-aware features such as formatting and comparing.
        /// </summary>
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
        public StringComparison DefaultStringComparison => useInvariantDefaults ? StringComparison.Ordinal : StringComparison.CurrentCulture;
    }
}
