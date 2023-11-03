#if FEATURE_SPAN && !NET6_0_OR_GREATER

namespace System
{
    /// <summary>
    /// Provides functionality to format the string representation of an object into
    /// a span.
    /// </summary>
    public interface ISpanFormattable : IFormattable
    {
        /// <summary>
        /// Tries to format the value of the current instance into the provided span of characters.
        /// </summary>
        /// <param name="destination">When this method returns, this instance's value formatted as a span of characters.</param>
        /// <param name="charsWritten">When this method returns, the number of characters that were written in destination.</param>
        /// <param name="format">A span containing the characters that represent a standard or custom format string
        /// that defines the acceptable format for destination.</param>
        /// <param name="provider">An optional object that supplies culture-specific formatting information for
        /// destination.</param>
        /// <returns><c>true</c> if the formatting was successful; otherwise, <c>false</c>.</returns>
        bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider);
    }
}

#endif
