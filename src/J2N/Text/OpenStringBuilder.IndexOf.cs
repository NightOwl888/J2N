using System;

namespace J2N.Text
{
    public partial class OpenStringBuilder
    {
        /// <summary>
        /// Reports the zero-based index of the first occurrence of the specified Unicode character
        /// in this string.
        /// </summary>
        /// <param name="value">A Unicode character to seek.</param>
        /// <returns>The zero-based index position of <paramref name="value"/> if that character
        /// is found, or -1 if it is not.</returns>
        /// <remarks>
        /// Index numbering starts from zero.
        /// <para/>
        /// This method performs an ordinal (culture-insensitive) search, where a character is considered
        /// equivalent to another character only if their Unicode scalar values are the same. 
        /// </remarks>
        public int IndexOf(char value) => new ReadOnlySpan<char>(m_Chars, 0, m_Position).IndexOf(value);

        /// <summary>
        /// Searches for the index of the specified character. The search for the
        /// character starts at the specified offset and moves towards the end.
        /// </summary>
        /// <param name="value">The string to find.</param>
        /// <param name="comparisonType">One of the enumeration values that specifies the rules for the search.</param>
        /// <returns>The index of the specified character, or <c>-1</c> if the character isn't found.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="value"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException"><paramref name="comparisonType"/> is not a
        /// <see cref="StringComparison"/> value.</exception>
        public int IndexOf(ReadOnlySpan<char> value, StringComparison comparisonType) // Coverage for the JDK
            => new ReadOnlySpan<char>(m_Chars, 0, m_Position).IndexOf(value, comparisonType);

        /// <summary>
        /// Searches for the index of the specified character. The search for the
        /// character starts at the specified offset and moves towards the end.
        /// </summary>
        /// <param name="value">The string to find.</param>
        /// <param name="startIndex">The starting offset.</param>
        /// <param name="comparisonType">One of the enumeration values that specifies the rules for the search.</param>
        /// <returns>The index of the specified character, or <c>-1</c> if the character isn't found.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="startIndex"/> is less than 0 (zero)
        /// or greater than <see cref="Length"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="comparisonType"/> is not a
        /// <see cref="StringComparison"/> value.</exception>
        public int IndexOf(ReadOnlySpan<char> value, int startIndex, StringComparison comparisonType) // Coverage for the JDK
        {
            if ((uint)startIndex > (uint)Length)
                ThrowHelper.ThrowArgumentOutOfRange_ArgumentOutOfRange_IndexString(startIndex, ExceptionArgument.startIndex);

            return new ReadOnlySpan<char>(m_Chars, startIndex, m_Position - startIndex).IndexOf(value, comparisonType) + startIndex;
        }

        /// <summary>
        /// Searches for the index of the specified character. The search for the
        /// character starts at the specified offset and moves towards the end.
        /// </summary>
        /// <param name="value">The string to find.</param>
        /// <param name="comparisonType">One of the enumeration values that specifies the rules for the search.</param>
        /// <returns>The index of the specified character, or <c>-1</c> if the character isn't found.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="value"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException"><paramref name="comparisonType"/> is not a
        /// <see cref="StringComparison"/> value.</exception>
        public int IndexOf(string value, StringComparison comparisonType) // Coverage for the JDK
        {
            if (value is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.value);

            return new ReadOnlySpan<char>(m_Chars, 0, m_Position).IndexOf(value.AsSpan(), comparisonType);
        }

        /// <summary>
        /// Searches for the index of the specified character. The search for the
        /// character starts at the specified offset and moves towards the end.
        /// </summary>
        /// <param name="value">The string to find.</param>
        /// <param name="startIndex">The starting offset.</param>
        /// <param name="comparisonType">One of the enumeration values that specifies the rules for the search.</param>
        /// <returns>The index of the specified character, or <c>-1</c> if the character isn't found.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="value"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="startIndex"/> is less than 0 (zero)
        /// or greater than <see cref="Length"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="comparisonType"/> is not a
        /// <see cref="StringComparison"/> value.</exception>
        public int IndexOf(string value, int startIndex, StringComparison comparisonType) // Coverage for the JDK
        {
            if (value is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.value);
            if ((uint)startIndex > (uint)Length)
                ThrowHelper.ThrowArgumentOutOfRange_ArgumentOutOfRange_IndexString(startIndex, ExceptionArgument.startIndex);

            return new ReadOnlySpan<char>(m_Chars, startIndex, m_Position - startIndex).IndexOf(value.AsSpan(), comparisonType) + startIndex;
        }

        /// <summary>
        /// Reports the zero-based index position of the last occurrence of a specified Unicode character
        /// within this instance.
        /// </summary>
        /// <param name="value">The Unicode character to seek.</param>
        /// <returns>The zero-based index of the last occurrence of the value in the
        /// span. If not found, returns -1.</returns>
        /// <remarks>
        /// Index numbering starts from zero. That is, the first character in the string
        /// is at index zero and the last is at <see cref="Length"/> - 1.
        /// <para/>
        /// This method begins searching at the last character position of this instance
        /// and proceeds backward toward the beginning until either <paramref name="value"/>
        /// is found or the first character position has been examined. The search is case-sensitive.
        /// <para/>
        /// This method performs an ordinal (culture-insensitive) search, where a character is
        /// considered equivalent to another character only if their Unicode scalar values are
        /// the same.
        /// </remarks>
        public int LastIndexOf(char value) => new ReadOnlySpan<char>(m_Chars, 0, m_Position).LastIndexOf(value);

        /// <summary>
        /// Searches for the index of the specified character. The search for the
        /// character starts at the specified offset and moves towards the beginning.
        /// </summary>
        /// <param name="value">The string to find.</param>
        /// <param name="comparisonType">One of the enumeration values that specifies the rules for the search.</param>
        /// <returns>The index of the specified character, <c>-1</c> if the character isn't found.</returns>
        /// <exception cref="ArgumentException"><paramref name="comparisonType"/> is not a
        /// <see cref="StringComparison"/> value.</exception>
        public int LastIndexOf(ReadOnlySpan<char> value, StringComparison comparisonType) // Coverage for the JDK
            => new ReadOnlySpan<char>(m_Chars, 0, m_Position).LastIndexOf(value, comparisonType);

        /// <summary>
        /// Searches for the index of the specified character. The search for the
        /// character starts at the specified offset and moves towards the beginning.
        /// </summary>
        /// <param name="value">The string to find.</param>
        /// <param name="startIndex">The starting offset.</param>
        /// <param name="comparisonType">One of the enumeration values that specifies the rules for the search.</param>
        /// <returns>The index of the specified character, <c>-1</c> if the character isn't found.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="value"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="startIndex"/> is less than 0 (zero) or
        /// greater than <see cref="Length"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="comparisonType"/> is not a
        /// <see cref="StringComparison"/> value.</exception>
        public int LastIndexOf(ReadOnlySpan<char> value, int startIndex, StringComparison comparisonType)
        {
            if ((uint)startIndex > (uint)Length)
                ThrowHelper.ThrowArgumentOutOfRange_ArgumentOutOfRange_IndexString(startIndex, ExceptionArgument.startIndex);

            return new ReadOnlySpan<char>(m_Chars, 0, startIndex + 1).LastIndexOf(value, comparisonType);
        }

        /// <summary>
        /// Searches for the index of the specified character. The search for the
        /// character starts at the specified offset and moves towards the beginning.
        /// </summary>
        /// <param name="value">The string to find.</param>
        /// <param name="comparisonType">One of the enumeration values that specifies the rules for the search.</param>
        /// <returns>The index of the specified character, <c>-1</c> if the character isn't found.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="value"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException"><paramref name="comparisonType"/> is not a
        /// <see cref="StringComparison"/> value.</exception>
        public int LastIndexOf(string value, StringComparison comparisonType)
        {
            if (value is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.value);

            return new ReadOnlySpan<char>(m_Chars, 0, m_Position).LastIndexOf(value.AsSpan(), comparisonType);
        }

        /// <summary>
        /// Searches for the index of the specified character. The search for the
        /// character starts at the specified offset and moves towards the beginning.
        /// </summary>
        /// <param name="value">The string to find.</param>
        /// <param name="startIndex">The starting offset.</param>
        /// <param name="comparisonType">One of the enumeration values that specifies the rules for the search.</param>
        /// <returns>The index of the specified character, <c>-1</c> if the character isn't found.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="value"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="startIndex"/> is less than 0 (zero) or
        /// greater than <see cref="Length"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="comparisonType"/> is not a
        /// <see cref="StringComparison"/> value.</exception>
        public int LastIndexOf(string value, int startIndex, StringComparison comparisonType)
        {
            if (value is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.value);
            if ((uint)startIndex > (uint)Length)
                ThrowHelper.ThrowArgumentOutOfRange_ArgumentOutOfRange_IndexString(startIndex, ExceptionArgument.startIndex);

            return new ReadOnlySpan<char>(m_Chars, 0, startIndex + 1).LastIndexOf(value.AsSpan(), comparisonType);
        }
    }
}
