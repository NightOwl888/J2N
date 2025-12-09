using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int IndexOf(char value) => ((ReadOnlySpan<char>)m_Chars.AsSpan(0, m_Position)).IndexOf(value);

        /// <summary>
        /// Searches for the index of the specified sequence of characters. The search for the
        /// sequence of characters starts at the specified offset and moves towards the end.
        /// </summary>
        /// <param name="value">The string to find.</param>
        /// <returns>The zero-based index position of <paramref name="value"/> from the start of
        /// the current instance if that sequence of characters is found, or -1 if it is not.
        /// If <paramref name="value"/> is <see cref="ReadOnlySpan{T}.Empty"/>, the return value
        /// is 0.</returns>
        /// <remarks>
        /// Index numbering starts from zero.
        /// <para/>
        /// This method performs an ordinal (culture-insensitive) search, where a character is considered
        /// equivalent to another character only if their Unicode scalar values are the same.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int IndexOf(ReadOnlySpan<char> value) // Coverage for the JDK
            => SpanUtilities.IndexOf(m_Chars.AsSpan(0, m_Position), value);

        /// <summary>
        /// Searches for the index of the specified sequence of characters. The search for the
        /// sequence of characters starts at the specified offset and moves towards the end.
        /// </summary>
        /// <param name="value">The string to find.</param>
        /// <param name="startIndex">The search starting position.</param>
        /// <returns>The zero-based index position of <paramref name="value"/> from the start of
        /// the current instance if that sequence of characters is found, or -1 if it is not.
        /// If <paramref name="value"/> is <see cref="ReadOnlySpan{T}.Empty"/>, the return value
        /// is <paramref name="startIndex"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="startIndex"/> is less than 0 (zero)
        /// or greater than <see cref="Length"/>.</exception>
        /// <remarks>
        /// Index numbering starts from zero.
        /// <para/>
        /// This method performs an ordinal (culture-insensitive) search, where a character is considered
        /// equivalent to another character only if their Unicode scalar values are the same.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int IndexOf(ReadOnlySpan<char> value, int startIndex) // Coverage for the JDK
            => SpanUtilities.IndexOf(m_Chars.AsSpan(0, m_Position), value, startIndex);

        /// <summary>
        /// Searches for the index of the specified sequence of characters. The search for the
        /// sequence of characters starts at the specified offset and moves towards the end.
        /// </summary>
        /// <param name="value">The sequence of characters to search for.</param>
        /// <param name="comparisonType">One of the enumeration values that specifies the rules for the search.</param>
        /// <returns>The zero-based index position of <paramref name="value"/> from the start of
        /// the current instance if that sequence of characters is found, or -1 if it is not.
        /// If <paramref name="value"/> is <see cref="ReadOnlySpan{T}.Empty"/>, the return value
        /// is 0.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="value"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException"><paramref name="comparisonType"/> is not a
        /// <see cref="StringComparison"/> value.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int IndexOf(ReadOnlySpan<char> value, StringComparison comparisonType)
            => SpanUtilities.IndexOf(m_Chars.AsSpan(0, m_Position), value, comparisonType);

        /// <summary>
        /// Searches for the index of the specified sequence of characters. The search for the
        /// sequence of characters starts at the specified offset and moves towards the end.
        /// </summary>
        /// <param name="value">The sequence of characters to search for.</param>
        /// <param name="startIndex">The search starting position.</param>
        /// <param name="comparisonType">One of the enumeration values that specifies the rules for the search.</param>
        /// <returns>The zero-based index position of <paramref name="value"/> from the start of
        /// the current instance if that sequence of characters is found, or -1 if it is not.
        /// If <paramref name="value"/> is <see cref="ReadOnlySpan{T}.Empty"/>, the return value
        /// is <paramref name="startIndex"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="startIndex"/> is less than 0 (zero)
        /// or greater than <see cref="Length"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="comparisonType"/> is not a
        /// <see cref="StringComparison"/> value.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int IndexOf(ReadOnlySpan<char> value, int startIndex, StringComparison comparisonType)
            => SpanUtilities.IndexOf(m_Chars.AsSpan(0, m_Position), value, startIndex, comparisonType);

        /// <summary>
        /// Searches for the index of the specified string. The search for the
        /// string starts at the specified offset and moves towards the end.
        /// </summary>
        /// <param name="value">The string to find.</param>
        /// <returns>The zero-based index position of <paramref name="value"/> from the start of
        /// the current instance if that sequence of characters is found, or -1 if it is not.
        /// If <paramref name="value"/> is <see cref="string.Empty"/>, the return value
        /// is 0.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="value"/> is <c>null</c>.</exception>
        /// <remarks>
        /// Index numbering starts from zero.
        /// <para/>
        /// This method performs an ordinal (culture-insensitive) search, where a character is considered
        /// equivalent to another character only if their Unicode scalar values are the same.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int IndexOf(string value) // Coverage for the JDK
            => SpanUtilities.IndexOf(m_Chars.AsSpan(0, m_Position), value);

        /// <summary>
        /// Searches for the index of the specified string. The search for the
        /// string starts at the specified offset and moves towards the end.
        /// </summary>
        /// <param name="value">The string to find.</param>
        /// <param name="startIndex">The search starting position.</param>
        /// <returns>The zero-based index position of <paramref name="value"/> from the start of
        /// the current instance if that sequence of characters is found, or -1 if it is not.
        /// If <paramref name="value"/> is <see cref="string.Empty"/>, the return value
        /// is <paramref name="startIndex"/>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="value"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="startIndex"/> is less than 0 (zero)
        /// or greater than <see cref="Length"/>.</exception>
        /// <remarks>
        /// Index numbering starts from zero.
        /// <para/>
        /// This method performs an ordinal (culture-insensitive) search, where a character is considered
        /// equivalent to another character only if their Unicode scalar values are the same.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int IndexOf(string value, int startIndex) // Coverage for the JDK
            => SpanUtilities.IndexOf(m_Chars.AsSpan(0, m_Position), value, startIndex);

        /// <summary>
        /// Searches for the index of the specified string. The search for the
        /// string starts at the specified offset and moves towards the end.
        /// </summary>
        /// <param name="value">The string to find.</param>
        /// <param name="comparisonType">One of the enumeration values that specifies the rules for the search.</param>
        /// <returns>The zero-based index position of <paramref name="value"/> from the start of
        /// the current instance if that sequence of characters is found, or -1 if it is not.
        /// If <paramref name="value"/> is <see cref="string.Empty"/>, the return value
        /// is 0.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="value"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException"><paramref name="comparisonType"/> is not a
        /// <see cref="StringComparison"/> value.</exception>
        ///  <remarks>
        /// Index numbering starts from zero.
        /// <para/>
        /// This method performs an ordinal (culture-insensitive) search, where a character is considered
        /// equivalent to another character only if their Unicode scalar values are the same.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int IndexOf(string value, StringComparison comparisonType)
            => SpanUtilities.IndexOf(m_Chars.AsSpan(0, m_Position), value, comparisonType);

        /// <summary>
        /// Searches for the index of the specified string. The search for the
        /// string starts at the specified offset and moves towards the end.
        /// </summary>
        /// <param name="value">The string to find.</param>
        /// <param name="startIndex">The search starting position.</param>
        /// <param name="comparisonType">One of the enumeration values that specifies the rules for the search.</param>
        /// <returns>The zero-based index position of <paramref name="value"/> from the start of
        /// the current instance if that sequence of characters is found, or -1 if it is not.
        /// If <paramref name="value"/> is <see cref="string.Empty"/>, the return value
        /// is <paramref name="startIndex"/>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="value"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="startIndex"/> is less than 0 (zero)
        /// or greater than <see cref="Length"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="comparisonType"/> is not a
        /// <see cref="StringComparison"/> value.</exception>
        ///  <remarks>
        /// Index numbering starts from zero.
        /// <para/>
        /// This method performs an ordinal (culture-insensitive) search, where a character is considered
        /// equivalent to another character only if their Unicode scalar values are the same.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int IndexOf(string value, int startIndex, StringComparison comparisonType)
            => SpanUtilities.IndexOf(m_Chars.AsSpan(0, m_Position), value, startIndex, comparisonType);

        /// <summary>
        /// Reports the zero-based index position of the last occurrence of a specified Unicode character
        /// within this instance.
        /// </summary>
        /// <param name="value">The Unicode character to seek.</param>
        /// <returns>The zero-based index of the last occurrence of the value in the
        /// this instance. If not found, returns -1.</returns>
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public int LastIndexOf(char value) => ((ReadOnlySpan<char>)m_Chars.AsSpan(0, m_Position)).LastIndexOf(value);

        /// <summary>
        /// Searches for the index of the specified sequence of characters. The search for the
        /// sequence of characters starts at the specified offset and moves towards the beginning.
        /// </summary>
        /// <param name="value">The sequence of characters to search for.</param>
        /// <returns>The zero-based starting index position of value if that string is found, or -1
        /// if it is not found. If <paramref name="value"/> is <see cref="ReadOnlySpan{T}.Empty"/>,
        /// it returns <see cref="Length"/>.</returns>
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int LastIndexOf(ReadOnlySpan<char> value) // Coverage for the JDK
            => SpanUtilities.LastIndexOf(m_Chars.AsSpan(0, m_Position), value);

        /// <summary>
        /// Searches for the index of the specified sequence of characters. The search for the
        /// sequence of characters starts at the specified offset and moves towards the beginning.
        /// </summary>
        /// <param name="value">The sequence of characters to search for.</param>
        /// <param name="startIndex">The search starting position. The search proceeds from
        /// <paramref name="startIndex"/> toward the beginning of this instance.</param>
        /// <returns>The zero-based starting index position of value if that string is found, or -1
        /// if it is not found. If <paramref name="value"/> is <see cref="ReadOnlySpan{T}.Empty"/>,
        /// it returns the lower value of either <paramref name="startIndex"/> or <see cref="Length"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <see cref="Length"/> is greater than zero and <paramref name="startIndex"/> is less
        /// than zero or greater than the length of the current instance.
        /// <para/>
        /// -or-
        /// <para/>
        /// <see cref="Length"/> is zero (0) and <paramref name="startIndex"/> is not zero.
        /// </exception>
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int LastIndexOf(ReadOnlySpan<char> value, int startIndex) // Coverage for the JDK
            => SpanUtilities.LastIndexOf(m_Chars.AsSpan(0, m_Position), value, startIndex);

        /// <summary>
        /// Searches for the index of the specified sequence of characters. The search for the
        /// sequence of characters starts at the specified offset and moves towards the beginning.
        /// </summary>
        /// <param name="value">The sequence of characters to search for.</param>
        /// <param name="comparisonType">One of the enumeration values that specifies the rules for the search.</param>
        /// <returns>The zero-based starting index position of value if that string is found, or -1
        /// if it is not found. If <paramref name="value"/> is <see cref="ReadOnlySpan{T}.Empty"/>,
        /// it returns <see cref="Length"/>.</returns>
        /// <exception cref="ArgumentException"><paramref name="comparisonType"/> is not a
        /// <see cref="StringComparison"/> value.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int LastIndexOf(ReadOnlySpan<char> value, StringComparison comparisonType)
            => SpanUtilities.LastIndexOf(m_Chars.AsSpan(0, m_Position), value, comparisonType);

        /// <summary>
        /// Searches for the index of the specified sequence of characters. The search for the
        /// sequence of characters starts at the specified offset and moves towards the beginning.
        /// </summary>
        /// <param name="value">The sequence of characters to search for.</param>
        /// <param name="startIndex">The starting offset.</param>
        /// <param name="comparisonType">One of the enumeration values that specifies the rules for the search.</param>
        /// <returns>The zero-based starting index position of value if that string is found, or -1
        /// if it is not found. If <paramref name="value"/> is <see cref="ReadOnlySpan{T}.Empty"/>,
        /// it returns the lower value of either <paramref name="startIndex"/> or <see cref="Length"/>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="value"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="startIndex"/> is less than 0 (zero) or
        /// greater than <see cref="Length"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="comparisonType"/> is not a
        /// <see cref="StringComparison"/> value.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int LastIndexOf(ReadOnlySpan<char> value, int startIndex, StringComparison comparisonType)
            => SpanUtilities.LastIndexOf(m_Chars.AsSpan(0, m_Position), value, startIndex, comparisonType);

        /// <summary>
        /// Searches for the index of the specified string. The search for the
        /// string starts at the specified offset and moves towards the beginning.
        /// </summary>
        /// <param name="value">The string to find.</param>
        /// <returns>The zero-based starting index position of value if that string is found, or -1
        /// if it is not found. If <paramref name="value"/> is <see cref="string.Empty"/>,
        /// it returns <see cref="Length"/>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="value"/> is <c>null</c>.</exception>
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int LastIndexOf(string value) // Coverage for the JDK
            => SpanUtilities.LastIndexOf(m_Chars.AsSpan(0, m_Position), value);

        /// <summary>
        /// Searches for the index of the specified string. The search for the
        /// string starts at the specified offset and moves towards the beginning.
        /// </summary>
        /// <param name="value">The string to find.</param>
        /// <param name="startIndex">The search starting position. The search proceeds from
        /// <paramref name="startIndex"/> toward the beginning of this instance.</param>
        /// <returns>The zero-based starting index position of value if that string is found, or -1
        /// if it is not found. If <paramref name="value"/> is <see cref="string.Empty"/>,
        /// it returns the lower value of either <paramref name="startIndex"/> or <see cref="Length"/>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="value"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <see cref="Length"/> is greater than zero and <paramref name="startIndex"/> is less
        /// than zero or greater than the length of the current instance.
        /// <para/>
        /// -or-
        /// <para/>
        /// <see cref="Length"/> is zero (0) and <paramref name="startIndex"/> is not zero.
        /// </exception>
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int LastIndexOf(string value, int startIndex) // Coverage for the JDK
            => SpanUtilities.LastIndexOf(m_Chars.AsSpan(0, m_Position), value, startIndex);

        /// <summary>
        /// Searches for the index of the specified string. The search for the
        /// string starts at the specified offset and moves towards the beginning.
        /// </summary>
        /// <param name="value">The string to find.</param>
        /// <param name="comparisonType">One of the enumeration values that specifies the rules for the search.</param>
        /// <returns>The zero-based starting index position of value if that string is found, or -1
        /// if it is not found. If <paramref name="value"/> is <see cref="string.Empty"/>,
        /// it returns <see cref="Length"/>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="value"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException"><paramref name="comparisonType"/> is not a
        /// <see cref="StringComparison"/> value.</exception>
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int LastIndexOf(string value, StringComparison comparisonType)
            => SpanUtilities.LastIndexOf(m_Chars.AsSpan(0, m_Position), value, comparisonType);

        /// <summary>
        /// Searches for the index of the specified string. The search for the
        /// string starts at the specified offset and moves towards the beginning.
        /// </summary>
        /// <param name="value">The string to find.</param>
        /// <param name="startIndex">The starting offset.</param>
        /// <param name="comparisonType">One of the enumeration values that specifies the rules for the search.</param>
        /// <returns>The zero-based starting index position of value if that string is found, or -1
        /// if it is not found. If <paramref name="value"/> is <see cref="string.Empty"/>,
        /// it returns the lower value of either <paramref name="startIndex"/> or <see cref="Length"/>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="value"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="startIndex"/> is less than 0 (zero) or
        /// greater than <see cref="Length"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="comparisonType"/> is not a
        /// <see cref="StringComparison"/> value.</exception>
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int LastIndexOf(string value, int startIndex, StringComparison comparisonType)
            => SpanUtilities.LastIndexOf(m_Chars.AsSpan(0, m_Position), value, startIndex, comparisonType);
    }
}
