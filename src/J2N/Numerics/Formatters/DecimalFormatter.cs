using System;

namespace J2N.Numerics.Formatters
{
    internal readonly struct DecimalFormatter : INumberFormatter<decimal>
    {
        public bool TryFormat(decimal value, ReadOnlySpan<char> format, IFormatProvider provider, Span<char> destination, out int charsWritten)
            => DotNetNumber.TryFormatDecimal(value, format, provider, destination, out charsWritten);
    }
}
