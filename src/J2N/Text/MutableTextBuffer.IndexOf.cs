#region Copyright 2019-2026 by Shad Storhaug, Licensed under the Apache License, Version 2.0
/*  Licensed to the Apache Software Foundation (ASF) under one or more
 *  contributor license agreements.  See the NOTICE file distributed with
 *  this work for additional information regarding copyright ownership.
 *  The ASF licenses this file to You under the Apache License, Version 2.0
 *  (the "License"); you may not use this file except in compliance with
 *  the License.  You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 *  Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *  limitations under the License.
 */
#endregion

using System;

namespace J2N.Text
{
    public partial class MutableTextBuffer
    {
        /// <summary>
        /// Reports the zero-based index of the first occurrence of the specified Unicode character
        /// in this instance.
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
        /// Reports the zero-based index of the first occurrence of the specified string.
        /// </summary>
        /// <param name="value">The string to find.</param>
        /// <returns>The zero-based index position of <paramref name="value"/> from the start of
        /// the current instance if that sequence of characters is found, or -1 if it is not.
        /// If <paramref name="value"/> is <see cref="string.Empty"/>, the return value
        /// is 0.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <c>null</c>.</exception>
        /// <remarks>
        /// Index numbering starts from zero.
        /// <para/>
        /// This method performs an ordinal (culture-insensitive) search, where a character is considered
        /// equivalent to another character only if their Unicode scalar values are the same.
        /// <para/>
        /// To match the behavior of the JDK, this method allows searches for the empty string, which will
        /// always return 0.
        /// </remarks>
        public int IndexOf(string value) // Coverage for the JDK
            => SpanUtilities.IndexOf(m_Chars.AsSpan(0, m_Position), value);

        /// <summary>
        /// Reports the zero-based index of the first occurrence of the specified span.
        /// </summary>
        /// <param name="value">The span to find.</param>
        /// <returns>The zero-based index position of <paramref name="value"/> from the start of
        /// the current instance if that sequence of characters is found, or -1 if it is not.
        /// If <paramref name="value"/> is <see cref="ReadOnlySpan{T}.Empty"/>, the return value
        /// is 0.</returns>
        /// <remarks>
        /// Index numbering starts from zero.
        /// <para/>
        /// This method performs an ordinal (culture-insensitive) search, where a character is considered
        /// equivalent to another character only if their Unicode scalar values are the same.
        /// <para/>
        /// To match the behavior of the JDK, this method allows searches for the empty span, which will
        /// always return 0.
        /// </remarks>
        public int IndexOf(ReadOnlySpan<char> value) // Coverage for the JDK
            => SpanUtilities.IndexOf(m_Chars.AsSpan(0, m_Position), value);

        /// <summary>
        /// Reports the zero-based index of the first occurrence of the specified string beginning
        /// at the specified index in this instance.
        /// </summary>
        /// <param name="value">The string to find.</param>
        /// <param name="startIndex">The search starting position.</param>
        /// <returns>The zero-based index position of <paramref name="value"/> from the specified index of
        /// the current instance if that sequence of characters is found, or -1 if it is not.
        /// If <paramref name="value"/> is empty, the return value is the effective start index
        /// after clamping.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <c>null</c>.</exception>
        /// <remarks>
        /// Index numbering starts from zero. The <paramref name="startIndex"/> parameter is clamped to
        /// the valid range of the current instance. Values less than zero are treated as zero, and values
        /// greater than <see cref = "Length" /> are treated as equal to <see cref = "Length" />.
        /// If <paramref name="startIndex"/> equals <see cref = "Length" />, this method
        /// returns -1 for non-empty searches. If <paramref name="value"/> is empty, this method returns
        /// the effective start index after clamping. The search is case-sensitive.
        /// <para/>
        /// This method performs an ordinal (culture-insensitive) search, where a character is considered
        /// equivalent to another character only if their Unicode scalar values are the same.
        /// <para/>
        /// To match the behavior of the JDK, this method allows searches for the empty string, which will
        /// always return <paramref name="startIndex"/>.
        /// <para/>
        /// If <paramref name="startIndex"/> is less than zero, it is treated as zero. If it is greater
        /// than <see cref="Length"/>, it is treated as equal to <see cref="Length"/>.
        /// </remarks>
        public int IndexOf(string value, int startIndex) // Coverage for the JDK
            => SpanUtilities.IndexOf(m_Chars.AsSpan(0, m_Position), value, startIndex);

        /// <summary>
        /// Reports the zero-based index of the first occurrence of the specified span beginning
        /// at the specified index in this instance.
        /// </summary>
        /// <param name="value">The span to find.</param>
        /// <param name="startIndex">The search starting position.</param>
        /// <returns>The zero-based index position of <paramref name="value"/> from the start of
        /// the current instance if that sequence of characters is found, or -1 if it is not.
        /// If <paramref name="value"/> is empty, the return value is the effective start index
        /// after clamping.</returns>
        /// <remarks>
        /// Index numbering starts from zero. The <paramref name="startIndex"/> parameter is clamped to
        /// the valid range of the current instance. Values less than zero are treated as zero, and values
        /// greater than <see cref = "Length" /> are treated as equal to <see cref = "Length" />.
        /// If <paramref name="startIndex"/> equals <see cref = "Length" />, this method
        /// returns -1 for non-empty searches. If <paramref name="value"/> is empty, this method returns
        /// the effective start index after clamping. The search is case-sensitive.
        /// <para/>
        /// This method performs an ordinal (culture-insensitive) search, where a character is considered
        /// equivalent to another character only if their Unicode scalar values are the same.
        /// <para/>
        /// To match the behavior of the JDK, this method allows searches for the empty span, which will
        /// always return <paramref name="startIndex"/>.
        /// <para/>
        /// If <paramref name="startIndex"/> is less than zero, it is treated as zero. If it is greater
        /// than <see cref="Length"/>, it is treated as equal to <see cref="Length"/>.
        /// </remarks>
        public int IndexOf(ReadOnlySpan<char> value, int startIndex) // Coverage for the JDK
            => SpanUtilities.IndexOf(m_Chars.AsSpan(0, m_Position), value, startIndex);

        /// <summary>
        /// Reports the zero-based index of the first occurrence of the specified string.
        /// </summary>
        /// <param name="value">The string to find.</param>
        /// <param name="comparisonType">One of the enumeration values that determines how the current instance
        /// and <paramref name="value"/> are compared.</param>
        /// <returns>The zero-based index position of <paramref name="value"/> from the start of
        /// the current instance if that sequence of characters is found, or -1 if it is not.
        /// If <paramref name="value"/> is <see cref="string.Empty"/>, the return value
        /// is 0.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <c>null</c>.</exception>
        /// <remarks>
        /// Index numbering starts from zero.
        /// <para/>
        /// The <paramref name="comparisonType"/> parameter specifies to search for the <paramref name="value"/>
        /// parameter using the current or invariant culture, using a case-sensitive or case-insensitive search,
        /// and using word or ordinal comparison rules.
        /// <para/>
        /// To match the behavior of the JDK, this method allows searches for the empty string, which will
        /// always return 0.
        /// <para/>
        /// On older platforms than .NET Core, this overload provides optimizations for
        /// <see cref="StringComparison.OrdinalIgnoreCase"/> over and above the System.Memory package.
        /// </remarks>
        public int IndexOf(string value, StringComparison comparisonType)
            => SpanUtilities.IndexOf(m_Chars.AsSpan(0, m_Position), value, comparisonType);

        /// <summary>
        /// Reports the zero-based index of the first occurrence of the specified span.
        /// </summary>
        /// <param name="value">The span to find.</param>
        /// <param name="comparisonType">One of the enumeration values that determines how the current instance
        /// and <paramref name="value"/> are compared.</param>
        /// <returns>The zero-based index position of <paramref name="value"/> from the start of
        /// the current instance if that sequence of characters is found, or -1 if it is not.
        /// If <paramref name="value"/> is <see cref="ReadOnlySpan{T}.Empty"/>, the return value
        /// is 0.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <c>null</c>.</exception>
        /// <remarks>
        /// Index numbering starts from zero.
        /// <para/>
        /// The <paramref name="comparisonType"/> parameter specifies to search for the <paramref name="value"/>
        /// parameter using the current or invariant culture, using a case-sensitive or case-insensitive search,
        /// and using word or ordinal comparison rules.
        /// <para/>
        /// To match the behavior of the JDK, this method allows searches for the empty span, which will
        /// always return 0.
        /// <para/>
        /// On older platforms than .NET Core, this overload provides optimizations for
        /// <see cref="StringComparison.OrdinalIgnoreCase"/> over and above the System.Memory package.
        /// </remarks>
        public int IndexOf(ReadOnlySpan<char> value, StringComparison comparisonType)
            => SpanUtilities.IndexOf(m_Chars.AsSpan(0, m_Position), value, comparisonType);

        /// <summary>
        /// Reports the zero-based index of the first occurrence of the specified string beginning
        /// at the specified index in this instance.
        /// </summary>
        /// <param name="value">The string to find.</param>
        /// <param name="startIndex">The search starting position.</param>
        /// <param name="comparisonType">One of the enumeration values that determines how the current instance
        /// and <paramref name="value"/> are compared.</param>
        /// <returns>The zero-based index position of <paramref name="value"/> from the start of
        /// the current instance if that sequence of characters is found, or -1 if it is not.
        /// If <paramref name="value"/> is empty, the return value is the effective start index
        /// after clamping.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException"><paramref name="comparisonType"/> is not a
        /// <see cref="StringComparison"/> value.</exception>
        /// <remarks>
        /// Index numbering starts from zero. The <paramref name="startIndex"/> parameter is clamped to
        /// the valid range of the current instance. Values less than zero are treated as zero, and values
        /// greater than <see cref = "Length" /> are treated as equal to <see cref = "Length" />.
        /// If <paramref name="startIndex"/> equals <see cref = "Length" />, this method
        /// returns -1 for non-empty searches. If <paramref name="value"/> is empty, this method returns
        /// the effective start index after clamping.
        /// <para/>
        /// The <paramref name="comparisonType"/> parameter specifies to search for the <paramref name="value"/>
        /// parameter using the current or invariant culture, using a case-sensitive or case-insensitive search,
        /// and using word or ordinal comparison rules.
        /// <para/>
        /// To match the behavior of the JDK, this method allows searches for the empty string, which will
        /// always return 0.
        /// <para/>
        /// If <paramref name="startIndex"/> is less than zero, it is treated as zero. If it is greater
        /// than <see cref="Length"/>, it is treated as equal to <see cref="Length"/>.
        /// <para/>
        /// On older platforms than .NET Core, this overload provides optimizations for
        /// <see cref="StringComparison.OrdinalIgnoreCase"/> over and above the System.Memory package.
        /// </remarks>
        public int IndexOf(string value, int startIndex, StringComparison comparisonType)
            => SpanUtilities.IndexOf(m_Chars.AsSpan(0, m_Position), value, startIndex, comparisonType);

        /// <summary>
        /// Reports the zero-based index of the first occurrence of the specified span beginning
        /// at the specified index in this instance.
        /// </summary>
        /// <param name="value">The span to find.</param>
        /// <param name="startIndex">The search starting position.</param>
        /// <param name="comparisonType">One of the enumeration values that determines how the current instance
        /// and <paramref name="value"/> are compared.</param>
        /// <returns>The zero-based index position of <paramref name="value"/> from the start of
        /// the current instance if that sequence of characters is found, or -1 if it is not.
        /// If <paramref name="value"/> is empty, the return value is the effective start index
        /// after clamping.</returns>
        /// <exception cref="ArgumentException"><paramref name="comparisonType"/> is not a
        /// <see cref="StringComparison"/> value.</exception>
        /// <remarks>
        /// Index numbering starts from zero. The <paramref name="startIndex"/> parameter is clamped to
        /// the valid range of the current instance. Values less than zero are treated as zero, and values
        /// greater than <see cref = "Length" /> are treated as equal to <see cref = "Length" />.
        /// If <paramref name="startIndex"/> equals <see cref = "Length" />, this method
        /// returns -1 for non-empty searches. If <paramref name="value"/> is empty, this method returns
        /// the effective start index after clamping.
        /// <para/>
        /// This method performs an ordinal (culture-insensitive) search, where a character is considered
        /// equivalent to another character only if their Unicode scalar values are the same.
        /// <para/>
        /// To match the behavior of the JDK, this method allows searches for the empty span, which will
        /// always return 0.
        /// <para/>
        /// If <paramref name="startIndex"/> is less than zero, it is treated as zero. If it is greater
        /// than <see cref="Length"/>, it is treated as equal to <see cref="Length"/>.
        /// <para/>
        /// On older platforms than .NET Core, this overload provides optimizations for
        /// <see cref="StringComparison.OrdinalIgnoreCase"/> over and above the System.Memory package.
        /// </remarks>
        public int IndexOf(ReadOnlySpan<char> value, int startIndex, StringComparison comparisonType)
            => SpanUtilities.IndexOf(m_Chars.AsSpan(0, m_Position), value, startIndex, comparisonType);

        /// <summary>
        /// Reports the zero-based index position of the last occurrence of a specified Unicode character
        /// within this instance.
        /// </summary>
        /// <param name="value">The Unicode character to seek.</param>
        /// <returns>The zero-based index of the last occurrence of the value in the
        /// span. If not found, returns -1.</returns>
        /// <remarks>
        /// Index numbering starts from zero. That is, the first character in the current instance
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
        /// Reports the zero-based index of the last occurrence of the specified string in
        /// this instance.
        /// </summary>
        /// <param name="value">The string to find.</param>
        /// <returns>The zero-based starting index position of value if that string is found, or -1
        /// if it is not found. If <paramref name="value"/> is <see cref="string.Empty"/>,
        /// it returns <see cref="Length"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <c>null</c>.</exception>
        /// <remarks>
        /// Index numbering starts from zero. That is, the first character in the current instance
        /// is at index zero and the last is at <see cref="Length"/> - 1.
        /// <para/>
        /// This method begins searching at the last character position of this instance
        /// and proceeds backward toward the beginning until either <paramref name="value"/>
        /// is found or the first character position has been examined. The search is case-sensitive.
        /// <para/>
        /// This method performs an ordinal (culture-insensitive) search, where a character is
        /// considered equivalent to another character only if their Unicode scalar values are
        /// the same.
        /// <para/>
        /// To match the behavior of the JDK, this method allows searches for the empty string, which will
        /// always return <see cref="Length"/>.
        /// </remarks>
        public int LastIndexOf(string value) // Coverage for the JDK
            => SpanUtilities.LastIndexOf(m_Chars.AsSpan(0, m_Position), value);

        /// <summary>
        /// Reports the zero-based index of the last occurrence of the specified string in
        /// this instance.
        /// </summary>
        /// <param name="value">The span to find.</param>
        /// <returns>The zero-based starting index position of value if that span is found, or -1
        /// if it is not found. If <paramref name="value"/> is <see cref="ReadOnlySpan{Char}.Empty"/>,
        /// it returns <see cref="Length"/>.</returns>
        /// <remarks>
        /// Index numbering starts from zero. That is, the first character in the current instance
        /// is at index zero and the last is at <see cref="Length"/> - 1.
        /// <para/>
        /// This method begins searching at the last character position of this instance
        /// and proceeds backward toward the beginning until either <paramref name="value"/>
        /// is found or the first character position has been examined. The search is case-sensitive.
        /// <para/>
        /// This method performs an ordinal (culture-insensitive) search, where a character is
        /// considered equivalent to another character only if their Unicode scalar values are
        /// the same.
        /// <para/>
        /// To match the behavior of the JDK, this method allows searches for the empty span, which will
        /// always return <see cref="Length"/>.
        /// </remarks>
        public int LastIndexOf(ReadOnlySpan<char> value) // Coverage for the JDK
            => SpanUtilities.LastIndexOf(m_Chars.AsSpan(0, m_Position), value);

        /// <summary>
        /// Reports the zero-based index of the last occurrence of the specified string beginning
        /// at the specified index in this instance.
        /// </summary>
        /// <param name="value">The string to find.</param>
        /// <param name="startIndex">The search starting position. The search proceeds from
        /// <paramref name="startIndex"/> toward the beginning of this instance.</param>
        /// <returns>The zero-based starting index position of value if that string is found, or -1
        /// if it is not found. If <paramref name="value"/> is <see cref="string.Empty"/>,
        /// it returns <paramref name="startIndex"/> if it is within the bounds of the current instance; otherwise,
        /// if <paramref name="startIndex"/> is greater than <see cref="Length"/>, it
        /// returns <see cref="Length"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <c>null</c>.</exception>
        /// <remarks>
        /// Index numbering starts from zero. That is, the first character in the current instance
        /// is at index zero and the last is at <see cref="Length"/> - 1.
        /// <para/>
        /// The search begins at the effective starting position and proceeds backward until either
        /// <paramref name="value"/> is found or the first character position has been examined.
        /// The effective starting position is the lesser of <paramref name="startIndex"/> and
        /// <see cref="Length"/> - <paramref name="value"/>.Length. For example, if
        /// <paramref name="startIndex"/> is <see cref="Length"/> - 1 and <paramref name="value"/>
        /// contains a single character, the method searches every character from the last
        /// character in the string to the beginning. The search is case-sensitive.
        /// <para/>
        /// This method performs an ordinal (culture-insensitive) search, where a character is
        /// considered equivalent to another character only if their Unicode scalar values are
        /// the same.
        /// <para/>
        /// To match the behavior of the JDK, this method allows searches for the empty string, which will
        /// always return the lesser of <paramref name="startIndex"/> or <see cref="Length"/>.
        /// <para/>
        /// If <paramref name="startIndex"/> is less than zero, the method returns -1. If it is greater than
        /// <see cref="Length"/>, it is treated as equal to <see cref="Length"/>.
        /// </remarks>
        public int LastIndexOf(string value, int startIndex) // Coverage for the JDK
            => SpanUtilities.LastIndexOf(m_Chars.AsSpan(0, m_Position), value, startIndex);

        /// <summary>
        /// Reports the zero-based index of the last occurrence of the specified span beginning
        /// at the specified index in this instance.
        /// </summary>
        /// <param name="value">The span to find.</param>
        /// <param name="startIndex">The search starting position. The search proceeds from startIndex toward the
        /// beginning of this instance.</param>
        /// <returns>The zero-based starting index position of value if that span is found, or -1
        /// if it is not found. If <paramref name="value"/> is <see cref="ReadOnlySpan{Char}.Empty"/>,
        /// it returns <paramref name="startIndex"/> if it is within the bounds of the current instance; otherwise,
        /// if <paramref name="startIndex"/> is greater than <see cref="Length"/>, it
        /// returns <see cref="Length"/>.</returns>
        /// <remarks>
        /// Index numbering starts from zero. That is, the first character in the current instance
        /// is at index zero and the last is at <see cref="Length"/> - 1.
        /// <para/>
        /// The search begins at the effective starting position and proceeds backward until either
        /// <paramref name="value"/> is found or the first character position has been examined.
        /// The effective starting position is the lesser of <paramref name="startIndex"/> and
        /// <see cref="Length"/> - <paramref name="value"/>.Length. For example, if
        /// <paramref name="startIndex"/> is <see cref="Length"/> - 1 and <paramref name="value"/>
        /// contains a single character, the method searches every character from the last
        /// character in the string to the beginning. The search is case-sensitive.
        /// <para/>
        /// This method performs an ordinal (culture-insensitive) search, where a character is
        /// considered equivalent to another character only if their Unicode scalar values are
        /// the same.
        /// <para/>
        /// To match the behavior of the JDK, this method allows searches for the empty span, which will
        /// always return the lesser of <paramref name="startIndex"/> or <see cref="Length"/>.
        /// <para/>
        /// If <paramref name="startIndex"/> is less than zero, the method returns -1. If it is greater than
        /// <see cref="Length"/>, it is treated as equal to <see cref="Length"/>.
        /// </remarks>
        public int LastIndexOf(ReadOnlySpan<char> value, int startIndex) // Coverage for the JDK
            => SpanUtilities.LastIndexOf(m_Chars.AsSpan(0, m_Position), value, startIndex);

        /// <summary>
        /// Reports the zero-based index of the last occurrence of the specified string in the
        /// this instance.
        /// </summary>
        /// <param name="value">The string to find.</param>
        /// <param name="comparisonType">One of the enumeration values that determines how the current instance
        /// and <paramref name="value"/> are compared.</param>
        /// <returns>The zero-based starting index position of value if that string is found, or -1
        /// if it is not found. If <paramref name="value"/> is <see cref="string.Empty"/>,
        /// it returns <see cref="Length"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException"><paramref name="comparisonType"/> is not a
        /// <see cref="StringComparison"/> value.</exception>
        /// <remarks>
        /// Index numbering starts from zero. That is, the first character in the current instance
        /// is at index zero and the last is at <see cref="Length"/> - 1.
        /// <para/>
        /// This method begins searching at the last character position of this instance
        /// and proceeds backward toward the beginning until either <paramref name="value"/>
        /// is found or the first character position has been examined.
        /// <para/>
        /// The <paramref name="comparisonType"/> parameter specifies to search for the <paramref name="value"/>
        /// parameter using the current or invariant culture, using a case-sensitive or case-insensitive search,
        /// and using word or ordinal comparison rules.
        /// <para/>
        /// To match the behavior of the JDK, this method allows searches for the empty string, which will
        /// always return <see cref="Length"/>.
        /// <para/>
        /// On older platforms than .NET Core, this overload provides optimizations for
        /// <see cref="StringComparison.OrdinalIgnoreCase"/> over and above the System.Memory package.
        /// </remarks>
        public int LastIndexOf(string value, StringComparison comparisonType)
            => SpanUtilities.LastIndexOf(m_Chars.AsSpan(0, m_Position), value, comparisonType);

        /// <summary>
        /// Reports the zero-based index of the last occurrence of the specified span in the
        /// this instance.
        /// </summary>
        /// <param name="value">The span to find.</param>
        /// <param name="comparisonType">One of the enumeration values that determines how the current instance
        /// and <paramref name="value"/> are compared.</param>
        /// <returns>The zero-based starting index position of value if that span is found, or -1
        /// if it is not found. If <paramref name="value"/> is <see cref="ReadOnlySpan{Char}.Empty"/>,
        /// it returns <see cref="Length"/>.</returns>
        /// <exception cref="ArgumentException"><paramref name="comparisonType"/> is not a
        /// <see cref="StringComparison"/> value.</exception>
        /// <remarks>
        /// Index numbering starts from zero. That is, the first character in the current instance
        /// is at index zero and the last is at <see cref="Length"/> - 1.
        /// <para/>
        /// This method begins searching at the last character position of this instance
        /// and proceeds backward toward the beginning until either <paramref name="value"/>
        /// is found or the first character position has been examined.
        /// <para/>
        /// The <paramref name="comparisonType"/> parameter specifies to search for the <paramref name="value"/>
        /// parameter using the current or invariant culture, using a case-sensitive or case-insensitive search,
        /// and using word or ordinal comparison rules.
        /// <para/>
        /// To match the behavior of the JDK, this method allows searches for the empty span, which will
        /// always return <see cref="Length"/>.
        /// <para/>
        /// On older platforms than .NET Core, this overload provides optimizations for
        /// <see cref="StringComparison.OrdinalIgnoreCase"/> over and above the System.Memory package.
        /// </remarks>
        public int LastIndexOf(ReadOnlySpan<char> value, StringComparison comparisonType)
            => SpanUtilities.LastIndexOf(m_Chars.AsSpan(0, m_Position), value, comparisonType);

        /// <summary>
        /// Reports the zero-based index of the last occurrence of the specified string beginning
        /// at the specified index in this instance.
        /// </summary>
        /// <param name="value">The string to find.</param>
        /// <param name="startIndex">The search starting position. The search proceeds from <paramref name="startIndex"/> toward the
        /// beginning of this instance.</param>
        /// <param name="comparisonType">One of the enumeration values that determines how the current instance
        /// and <paramref name="value"/> are compared.</param>
        /// <returns>The zero-based starting index position of value if that string is found, or -1
        /// if it is not found. If <paramref name="value"/> is <see cref="string.Empty"/>,
        /// it returns <paramref name="startIndex"/> if it is within the bounds of the the current instance; otherwise, if
        /// <paramref name="startIndex"/> is greater than <see cref="Length"/>, it returns
        /// the <see cref="Length"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException"><paramref name="comparisonType"/> is not a
        /// <see cref="StringComparison"/> value.</exception>
        /// <remarks>
        /// Index numbering starts from zero. That is, the first character in the current instance
        /// is at index zero and the last is at <see cref="Length"/> - 1.
        /// <para/>
        /// The search begins at the effective starting position and proceeds backward until either
        /// <paramref name="value"/> is found or the first character position has been examined.
        /// The effective starting position is the lesser of <paramref name="startIndex"/> and
        /// <see cref="Length"/> - <paramref name="value"/>.Length. For example, if
        /// <paramref name="startIndex"/> is <see cref="Length"/> - 1 and <paramref name="value"/>
        /// contains a single character, the method searches every character from the last
        /// character in the string to the beginning.
        /// <para/>
        /// The <paramref name="comparisonType"/> parameter specifies to search for the <paramref name="value"/>
        /// parameter using the current or invariant culture, using a case-sensitive or case-insensitive search,
        /// and using word or ordinal comparison rules.
        /// <para/>
        /// To match the behavior of the JDK, this method allows searches for the empty string, which will
        /// always return the lesser of <paramref name="startIndex"/> or <see cref="Length"/>.
        /// <para/>
        /// If <paramref name="startIndex"/> is less than zero, the method returns -1. If it is greater than
        /// <see cref="Length"/>, it is treated as equal to <see cref="Length"/>.
        /// <para/>
        /// On older platforms than .NET Core, this overload provides optimizations for
        /// <see cref="StringComparison.OrdinalIgnoreCase"/> over and above the System.Memory package.
        /// </remarks>
        public int LastIndexOf(string value, int startIndex, StringComparison comparisonType)
            => SpanUtilities.LastIndexOf(m_Chars.AsSpan(0, m_Position), value, startIndex, comparisonType);

        /// <summary>
        /// Reports the zero-based index of the last occurrence of the specified span  beginning
        /// at the specified index in this instance.
        /// </summary>
        /// <param name="value">The span to find.</param>
        /// <param name="startIndex">The search starting position. The search proceeds from <paramref name="startIndex"/> toward the
        /// beginning of this instance.</param>
        /// <param name="comparisonType">One of the enumeration values that determines how the current instance
        /// and <paramref name="value"/> are compared.</param>
        /// <returns>The zero-based starting index position of value if that span is found, or -1
        /// if it is not found. If <paramref name="value"/> is <see cref="ReadOnlySpan{Char}.Empty"/>,
        /// it returns <paramref name="startIndex"/> if it is within the bounds of the the current instance; otherwise, if
        /// <paramref name="startIndex"/> is greater than <see cref="Length"/>, it returns
        /// the <see cref="Length"/>.</returns>
        /// <exception cref="ArgumentException"><paramref name="comparisonType"/> is not a
        /// <see cref="StringComparison"/> value.</exception>
        /// <remarks>
        /// Index numbering starts from zero. That is, the first character in the current instance
        /// is at index zero and the last is at <see cref="Length"/> - 1.
        /// <para/>
        /// The search begins at the effective starting position and proceeds backward until either
        /// <paramref name="value"/> is found or the first character position has been examined.
        /// The effective starting position is the lesser of <paramref name="startIndex"/> and
        /// <see cref="Length"/> - <paramref name="value"/>.Length. For example, if
        /// <paramref name="startIndex"/> is <see cref="Length"/> - 1 and <paramref name="value"/>
        /// contains a single character, the method searches every character from the last
        /// character in the string to the beginning.
        /// <para/>
        /// The <paramref name="comparisonType"/> parameter specifies to search for the <paramref name="value"/>
        /// parameter using the current or invariant culture, using a case-sensitive or case-insensitive search,
        /// and using word or ordinal comparison rules.
        /// <para/>
        /// To match the behavior of the JDK, this method allows searches for the empty span, which will
        /// always return the lesser of <paramref name="startIndex"/> or <see cref="Length"/>.
        /// <para/>
        /// If <paramref name="startIndex"/> is less than zero, the method returns -1. If it is greater than
        /// <see cref="Length"/>, it is treated as equal to <see cref="Length"/>.
        /// <para/>
        /// On older platforms than .NET Core, this overload provides optimizations for
        /// <see cref="StringComparison.OrdinalIgnoreCase"/> over and above the System.Memory package.
        /// </remarks>
        public int LastIndexOf(ReadOnlySpan<char> value, int startIndex, StringComparison comparisonType)
            => SpanUtilities.LastIndexOf(m_Chars.AsSpan(0, m_Position), value, startIndex, comparisonType);
    }
}
