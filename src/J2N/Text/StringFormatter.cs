using System;
using System.Globalization;

namespace J2N.Text
{
    /// <summary>
    /// Provides number and boolean formatting rules similar to how they are done in Java.
    /// <list type="bullet">
    ///     <item><description><see cref="float"/> and <see cref="double"/> values are displayed with a minimum of 1 fractional digit.</description></item>
    ///     <item><description><see cref="float"/> values with fractional digits are displayed to 7 decimals.</description></item>
    ///     <item><description><see cref="float"/> and <see cref="double"/> negative zeros are displayed with the same rules as the
    ///         current culture's <see cref="NumberFormatInfo.NumberNegativePattern"/> and <see cref="NumberFormatInfo.NegativeSign"/>.</description></item>
    ///     <item><description><see cref="bool"/> values are lowercased to <c>true</c> and <c>false</c>, rather than the default .NET <c>True</c> and <c>False</c>.</description></item>
    /// </list>
    /// </summary>
    public class StringFormatter : IFormatProvider, ICustomFormatter
    {
        private const float FloatNegativeZero = -0.0f;
        private const double DoubleNegativeZero = -0.0d;

        private readonly CultureInfo culture;

        /// <summary>
        /// Initializes a new instance of <see cref="StringFormatter"/>.
        /// </summary>
        public StringFormatter()
            : this(null)
        { }

        /// <summary>
        /// Initializes a new instance of <see cref="StringFormatter"/> with the specified <paramref name="culture"/>.
        /// </summary>
        /// <param name="culture">A <see cref="CultureInfo"/> that specifies the rules that will be used for formatting.
        /// If <c>null</c>, the <see cref="CultureInfo.CurrentCulture"/> will be used.</param>
        public StringFormatter(CultureInfo culture)
        {
            this.culture = culture ?? CultureInfo.CurrentCulture;
        }

        /// <summary>
        /// Gets the format provider.
        /// </summary>
        /// <param name="formatType">The format type that is requested.</param>
        /// <returns>The requested format provider, or <c>null</c> if it is not applicable.</returns>
        public object GetFormat(Type formatType)
        {
            if (typeof(ICustomFormatter).Equals(formatType))
                return this;
            return null;
        }

        /// <summary>
        /// Formats the <paramref name="arg"/> with rules similar to Java.
        /// <list type="bullet">
        ///     <item><description><see cref="float"/> and <see cref="double"/> values are displayed with a minimum of 1 fractional digit.</description></item>
        ///     <item><description><see cref="float"/> values with fractional digits are displayed to 7 decimals.</description></item>
        ///     <item><description><see cref="float"/> and <see cref="double"/> negative zeros are displayed with the same rules as the
        ///         current culture's <see cref="NumberFormatInfo.NumberNegativePattern"/> and <see cref="NumberFormatInfo.NegativeSign"/>.</description></item>
        ///     <item><description><see cref="bool"/> values are lowercased to <c>"true"</c> and <c>"false"</c>, rather than the default .NET "True" and "False".</description></item>
        /// </list>
        /// </summary>
        /// <param name="format">The format. To utilize this formatter, use <c>"{0}"</c> or <c>"{0:J}"</c>, otherwise it will be bypassed.</param>
        /// <param name="arg">The object to format.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// <returns>A string representing the formatted value, or <c>null</c> when this formatter is not applicable.</returns>
        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            if (!this.Equals(formatProvider))
                return null;

            // Set default format specifier             
            if (string.IsNullOrEmpty(format))
                format = "J";

            if (!(format == "J" || format == "j"))
                return null;

            if (arg is double d)
                return FormatDouble(d, culture.NumberFormat);
            else if (arg is float f)
                return FormatSingle(f, culture.NumberFormat);
            else if (arg is bool b)
                return FormatBoolean(b);

            return null;
        }

        private static string FormatNegativeZero(NumberFormatInfo numberFormat)
        {
            switch (numberFormat.NumberNegativePattern)
            {
                case 0: // (1,234.00)
                    return "(0.0)";
                case 2: // - 1,234.00
                    return numberFormat.NegativeSign + " 0.0";
                case 3: // 1,234.00-
                    return "0.0" + numberFormat.NegativeSign;
                case 4: // 1,234.00 -
                    return "0.0 " + numberFormat.NegativeSign;
                default: // (1): -1,234.00
                    return numberFormat.NegativeSign + "0.0";
            }
        }

        private static string FormatDouble(double d, NumberFormatInfo numberFormat)
        {
            if ((long)d == d)
            {
                // Special case: negative zero
                if (d == 0 && BitConversion.DoubleToRawInt64Bits(d) == BitConversion.DoubleToRawInt64Bits(DoubleNegativeZero))
                    return FormatNegativeZero(numberFormat);

                // Special case: When we have an integer value,
                // the standard .NET formatting removes the decimal point
                // and everything to the right. But we need to always
                // have at least 1 decimal place to match Java.
                return d.ToString("0.0", numberFormat);
            }

            return d.ToString("R", numberFormat);
        }

        private static string FormatSingle(float f, NumberFormatInfo numberFormat)
        {
            if ((int)f == f)
            {
                // Special case: negative zero
                if (f == 0 && BitConversion.SingleToRawInt32Bits(f) == BitConversion.SingleToRawInt32Bits(FloatNegativeZero))
                    return FormatNegativeZero(numberFormat);

                // Special case: When we have an integer value,
                // the standard .NET formatting removes the decimal point
                // and everything to the right. But we need to always
                // have at least 1 decimal place to match Java.
                return f.ToString("0.0", numberFormat);
            }

            // J2N NOTE: Although the MSDN documentation says that 
            // round-trip on float will be limited to 7 decimals, it appears
            // not to be the case. Also, when specifying "0.0######", we only
            // get a result to 6 decimal places maximum. So, we must round before
            // doing a round-trip format to guarantee 7 decimal places.
            return Math.Round(f, 7).ToString("R", numberFormat);
        }

        private static string FormatBoolean(bool b)
        {
            return b ? "true" : "false";
        }
    }
}
