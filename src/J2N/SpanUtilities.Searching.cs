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

using J2N.Globalization;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace J2N
{
    /// <summary>
    /// Provides specialized operations on <see cref="Span{T}"/> and <see cref="ReadOnlySpan{T}"/> instances that
    /// are not good candidates for extension methods.
    /// </summary>
    public static partial class SpanUtilities
    {
        #region IndexOf

        /// <summary>
        /// Reports the zero-based index of the first occurrence of the specified <paramref name="value"/> in
        /// the specified <paramref name="span"/>.
        /// </summary>
        /// <param name="span">The source span.</param>
        /// <param name="value">The value to seek within the source span.</param>
        /// <returns>The zero-based index position of <paramref name="value"/> from the start of
        /// the specified <paramref name="span"/> if that sequence of characters is found, or -1 if it is not.
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
        /// always return 0. This method also allows searches within an empty span.
        /// </remarks>
        public static int IndexOf(ReadOnlySpan<char> span, string value)
        {
            if (value is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.value);

            // To match the JDK, allow search for empty string
            if (value.Length == 0)
                return 0;

            // If value is longer than the remaining searchable range, impossible to match
            if (value.Length > span.Length)
                return -1;

            return System.MemoryExtensions.IndexOf(span, value); // J2N: Hardware optimized, where possible
        }

        /// <summary>
        /// Reports the zero-based index of the first occurrence of the specified <paramref name="value"/> in
        /// the specified <paramref name="span"/>.
        /// </summary>
        /// <param name="span">The source span.</param>
        /// <param name="value">The value to seek within the source span.</param>
        /// <returns>The zero-based index position of <paramref name="value"/> from the start of
        /// the specified <paramref name="span"/> if that sequence of characters is found, or -1 if it is not.
        /// If <paramref name="value"/> is <see cref="ReadOnlySpan{T}.Empty"/>, the return value
        /// is 0.</returns>
        /// <remarks>
        /// Index numbering starts from zero.
        /// <para/>
        /// This method performs an ordinal (culture-insensitive) search, where a character is considered
        /// equivalent to another character only if their Unicode scalar values are the same.
        /// <para/>
        /// To match the behavior of the JDK, this method allows searches for the empty span, which will
        /// always return 0. This method also allows searches within an empty span.
        /// </remarks>
        public static int IndexOf(ReadOnlySpan<char> span, ReadOnlySpan<char> value)
        {
            // To match the JDK, allow search for empty string
            if (value.IsEmpty)
                return 0;

            // If value is longer than the remaining searchable range, impossible to match
            if (value.Length > span.Length)
                return -1;

            return System.MemoryExtensions.IndexOf(span, value); // J2N: Hardware optimized, where possible
        }

        /// <summary>
        /// Reports the zero-based index of the first occurrence of the specified <paramref name="value"/>
        /// beginning at the specified <paramref name="startIndex"/> in the specified <paramref name="span"/>.
        /// </summary>
        /// <param name="span">The source span.</param>
        /// <param name="value">The value to seek within the source span.</param>
        /// <param name="startIndex">The search starting position.</param>
        /// <returns>The zero-based index position of <paramref name="value"/> from the specified index of
        /// <paramref name="span"/> if that sequence of characters is found, or -1 if it is not.
        /// If <paramref name="value"/> is empty, the return value is the effective start index
        /// after clamping.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <c>null</c>.</exception>
        /// <remarks>
        /// Index numbering starts from zero. The <paramref name="startIndex"/> parameter is clamped to
        /// the valid range of <paramref name="span"/>. Values less than zero are treated as zero, and values
        /// greater than the length of <paramref name="span"/> are treated as equal to the length of <paramref name="span"/>.
        /// If <paramref name="startIndex"/> equals the length of <paramref name="span"/>, this method
        /// returns -1 for non-empty searches. If <paramref name="value"/> is empty, this method returns
        /// the effective start index after clamping. The search is case-sensitive.
        /// <para/>
        /// This method performs an ordinal (culture-insensitive) search, where a character is considered
        /// equivalent to another character only if their Unicode scalar values are the same.
        /// <para/>
        /// To match the behavior of the JDK, this method allows searches for the empty string, which will
        /// always return <paramref name="startIndex"/>. This method also allows searches within an empty span.
        /// <para/>
        /// If <paramref name="startIndex"/> is less than zero, it is treated as zero. If it is greater
        /// than the length of <paramref name="span"/>, it is treated as equal to the length.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int IndexOf(ReadOnlySpan<char> span, string value, int startIndex)
        {
            if (value is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.value);

            return IndexOf(span, value.AsSpan(), startIndex);
        }

        /// <summary>
        /// Reports the zero-based index of the first occurrence of the specified <paramref name="value"/>
        /// beginning at the specified index in the specified <paramref name="span"/>.
        /// </summary>
        /// <param name="span">The source span.</param>
        /// <param name="value">The value to seek within the source span.</param>
        /// <param name="startIndex">The search starting position.</param>
        /// <returns>The zero-based index position of <paramref name="value"/> from the start of
        /// the <paramref name="span"/> if that sequence of characters is found, or -1 if it is not.
        /// If <paramref name="value"/> is empty, the return value is the effective start index
        /// after clamping.</returns>
        /// <remarks>
        /// Index numbering starts from zero. The <paramref name="startIndex"/> parameter is clamped to
        /// the valid range of <paramref name="span"/>. Values less than zero are treated as zero, and values
        /// greater than the length of <paramref name="span"/> are treated as equal to the length of <paramref name="span"/>.
        /// If <paramref name="startIndex"/> equals the length of <paramref name="span"/>, this method
        /// returns -1 for non-empty searches. If <paramref name="value"/> is empty, this method returns
        /// the effective start index after clamping. The search is case-sensitive.
        /// <para/>
        /// This method performs an ordinal (culture-insensitive) search, where a character is considered
        /// equivalent to another character only if their Unicode scalar values are the same.
        /// <para/>
        /// To match the behavior of the JDK, this method allows searches for the empty span, which will
        /// always return <paramref name="startIndex"/>. This method also allows searches within an empty span.
        /// <para/>
        /// If <paramref name="startIndex"/> is less than zero, it is treated as zero. If it is greater
        /// than the length of <paramref name="span"/>, it is treated as equal to the length.
        /// </remarks>
        public static int IndexOf(ReadOnlySpan<char> span, ReadOnlySpan<char> value, int startIndex)
        {
            if (startIndex < 0)
                startIndex = 0;
            if (startIndex > span.Length)
                startIndex = span.Length;

            // To match the JDK, allow search for empty string
            if (value.IsEmpty)
                return startIndex;

            // Once clamped to Length, there is no searchable content remaining.
            if (startIndex == span.Length)
                return -1;

            // If value is longer than the remaining searchable range, impossible to match
            if (value.Length > span.Length - startIndex)
                return -1;

            // J2N: Hardware optimized, where possible
            int actualIndex = System.MemoryExtensions.IndexOf(span.Slice(startIndex), value);
            return actualIndex >= 0 ? startIndex + actualIndex : -1; // Overflow prevented by clamping
        }

        /// <summary>
        /// Reports the zero-based index of the first occurrence of the specified <paramref name="value"/>
        /// in the specified <paramref name="span"/>.
        /// </summary>
        /// <param name="span">The source span.</param>
        /// <param name="value">The value to seek within the source span.</param>
        /// <param name="comparisonType">One of the enumeration values that determines how the <paramref name="span"/>
        /// and <paramref name="value"/> are compared.</param>
        /// <returns>The zero-based index position of <paramref name="value"/> from the start of
        /// the <paramref name="span"/> if that sequence of characters is found, or -1 if it is not.
        /// If <paramref name="value"/> is <see cref="string.Empty"/>, the return value
        /// is 0.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException"><paramref name="comparisonType"/> is not a
        /// <see cref="StringComparison"/> value.</exception>
        /// <remarks>
        /// Index numbering starts from zero.
        /// <para/>
        /// The <paramref name="comparisonType"/> parameter specifies to search for the <paramref name="value"/>
        /// parameter using the current or invariant culture, using a case-sensitive or case-insensitive search,
        /// and using word or ordinal comparison rules.
        /// <para/>
        /// To match the behavior of the JDK, this method allows searches for the empty string, which will
        /// always return 0. This method also allows searches within an empty span.
        /// <para/>
        /// On older platforms than .NET Core, this overload provides optimizations for
        /// <see cref="StringComparison.OrdinalIgnoreCase"/> over and above the System.Memory package.
        /// </remarks>
        public static int IndexOf(ReadOnlySpan<char> span, string value, StringComparison comparisonType)
        {
            if (value is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.value);

            // To match the JDK, allow search for empty string
            if (value.Length == 0)
                return 0;

            // If value is longer than the remaining searchable range, impossible to match
            if (value.Length > span.Length)
                return -1;

#if FEATURE_MEMORYEXTENSIONS_INDEXOF_STRINGCOMPARISON
            return System.MemoryExtensions.IndexOf(span, value, comparisonType); // J2N: Hardware optimized, where possible
#else
            CheckStringComparison(comparisonType);

            if (comparisonType == StringComparison.Ordinal)
            {
                return System.MemoryExtensions.IndexOf(span, value);
            }
            else if (comparisonType == StringComparison.OrdinalIgnoreCase)
            {
                // Allocation-free common case for .NET Framework.
                return Ordinal.IndexOfOrdinalIgnoreCase(span, value);
            }

            // Hack for platforms older than .NET Core, since this overload didn't exist.
            // J2N TODO: Optimize (this is rarely used)
            return IndexOfSlow(span, value, comparisonType);
#endif
        }

        /// <summary>
        /// Reports the zero-based index of the first occurrence of the specified <paramref name="value"/>
        /// in the specified <paramref name="span"/>.
        /// </summary>
        /// <param name="span">The source span.</param>
        /// <param name="value">The value to seek within the source span.</param>
        /// <param name="comparisonType">One of the enumeration values that determines how the <paramref name="span"/>
        /// and <paramref name="value"/> are compared.</param>
        /// <returns>The zero-based index position of <paramref name="value"/> from the start of
        /// the <paramref name="span"/> if that sequence of characters is found, or -1 if it is not.
        /// If <paramref name="value"/> is <see cref="ReadOnlySpan{T}.Empty"/>, the return value
        /// is 0.</returns>
        /// <exception cref="ArgumentException"><paramref name="comparisonType"/> is not a
        /// <see cref="StringComparison"/> value.</exception>
        /// <remarks>
        /// Index numbering starts from zero.
        /// <para/>
        /// The <paramref name="comparisonType"/> parameter specifies to search for the <paramref name="value"/>
        /// parameter using the current or invariant culture, using a case-sensitive or case-insensitive search,
        /// and using word or ordinal comparison rules.
        /// <para/>
        /// To match the behavior of the JDK, this method allows searches for the empty span, which will
        /// always return 0. This method also allows searches within an empty span.
        /// <para/>
        /// On older platforms than .NET Core, this overload provides optimizations for
        /// <see cref="StringComparison.OrdinalIgnoreCase"/> over and above the System.Memory package.
        /// </remarks>
        public static int IndexOf(ReadOnlySpan<char> span, ReadOnlySpan<char> value, StringComparison comparisonType)
        {
            // To match the JDK, allow search for empty string
            if (value.IsEmpty)
                return 0;

            // If value is longer than the remaining searchable range, impossible to match
            if (value.Length > span.Length)
                return -1;

#if FEATURE_MEMORYEXTENSIONS_INDEXOF_STRINGCOMPARISON
            return System.MemoryExtensions.IndexOf(span, value, comparisonType); // J2N: Hardware optimized, where possible
#else
            CheckStringComparison(comparisonType);

            if (comparisonType == StringComparison.Ordinal)
            {
                return System.MemoryExtensions.IndexOf(span, value);
            }
            else if (comparisonType == StringComparison.OrdinalIgnoreCase)
            {
                // Allocation-free common case for .NET Framework.
                return Ordinal.IndexOfOrdinalIgnoreCase(span, value);
            }

            // Hack for platforms older than .NET Core, since this overload didn't exist.
            // J2N TODO: Optimize (this is rarely used)
            return IndexOfSlow(span, value, comparisonType);
#endif
        }

        /// <summary>
        /// Reports the zero-based index of the first occurrence of the specified <paramref name="value"/>
        /// beginning at the specified index in the specified <paramref name="span"/>.
        /// </summary>
        /// <param name="span">The source span.</param>
        /// <param name="value">The value to seek within the source span.</param>
        /// <param name="startIndex">The search starting position.</param>
        /// <param name="comparisonType">One of the enumeration values that determines how the <paramref name="span"/>
        /// and <paramref name="value"/> are compared.</param>
        /// <returns>The zero-based index position of <paramref name="value"/> from the start of
        /// <paramref name="span"/> if that sequence of characters is found, or -1 if it is not.
        /// If <paramref name="value"/> is empty, the return value is the effective start index
        /// after clamping.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException"><paramref name="comparisonType"/> is not a
        /// <see cref="StringComparison"/> value.</exception>
        /// <remarks>
        /// Index numbering starts from zero. The <paramref name="startIndex"/> parameter is clamped to
        /// the valid range of the <paramref name="span"/>. Values less than zero are treated as zero, and values
        /// greater than the length of <paramref name="span"/> are treated as equal to the length of <paramref name="span"/>.
        /// If <paramref name="startIndex"/> equals the length of <paramref name="span"/>, this method
        /// returns -1 for non-empty searches. If <paramref name="value"/> is empty, this method returns
        /// the effective start index after clamping.
        /// <para/>
        /// The <paramref name="comparisonType"/> parameter specifies to search for the <paramref name="value"/>
        /// parameter using the current or invariant culture, using a case-sensitive or case-insensitive search,
        /// and using word or ordinal comparison rules.
        /// <para/>
        /// To match the behavior of the JDK, this method allows searches for the empty string, which will
        /// always return 0. This method also allows searches within an empty span.
        /// <para/>
        /// If <paramref name="startIndex"/> is less than zero, it is treated as zero. If it is greater
        /// than the length of <paramref name="span"/>, it is treated as equal to the length.
        /// <para/>
        /// On older platforms than .NET Core, this overload provides optimizations for
        /// <see cref="StringComparison.OrdinalIgnoreCase"/> over and above the System.Memory package.
        /// </remarks>
        public static int IndexOf(ReadOnlySpan<char> span, string value, int startIndex, StringComparison comparisonType)
        {
            if (value is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.value);

            if (startIndex < 0)
                startIndex = 0;
            if (startIndex > span.Length)
                startIndex = span.Length;

            // To match the JDK, allow search for empty string
            if (value.Length == 0)
                return startIndex;

            // Once clamped to Length, there is no searchable content remaining.
            if (startIndex == span.Length)
                return -1;

            // If value is longer than the remaining searchable range, impossible to match
            if (value.Length > span.Length - startIndex)
                return -1;

            ReadOnlySpan<char> toSearch = span.Slice(startIndex);
#if FEATURE_MEMORYEXTENSIONS_INDEXOF_STRINGCOMPARISON
            // J2N: Hardware optimized, where possible
            int actualIndex = System.MemoryExtensions.IndexOf(toSearch, value, comparisonType);
#else
            CheckStringComparison(comparisonType);
            int actualIndex;

            if (comparisonType == StringComparison.Ordinal)
            {
                actualIndex = System.MemoryExtensions.IndexOf(toSearch, value);
            }
            else if (comparisonType == StringComparison.OrdinalIgnoreCase)
            {
                // Allocation-free common case for .NET Framework.
                actualIndex = Ordinal.IndexOfOrdinalIgnoreCase(toSearch, value);
            }
            else
            {
                // Hack for platforms older than .NET Core, since this overload didn't exist.
                // J2N TODO: Optimize (this is rarely used)
                actualIndex = IndexOfSlow(toSearch, value, comparisonType);
            }
#endif
            return actualIndex >= 0 ? startIndex + actualIndex : -1; // Overflow prevented by clamping
        }

        /// <summary>
        /// Reports the zero-based index of the first occurrence of the specified <paramref name="value"/>
        /// beginning at the specified index in the specified <paramref name="span"/>.
        /// </summary>
        /// <param name="span">The source span.</param>
        /// <param name="value">The value to seek within the source span.</param>
        /// <param name="startIndex">The search starting position.</param>
        /// <param name="comparisonType">One of the enumeration values that determines how the <paramref name="span"/>
        /// and <paramref name="value"/> are compared.</param>
        /// <returns>The zero-based index position of <paramref name="value"/> from the start of
        /// the <paramref name="span"/> if that sequence of characters is found, or -1 if it is not.
        /// If <paramref name="value"/> is empty, the return value is the effective start index
        /// after clamping.</returns>
        /// <exception cref="ArgumentException"><paramref name="comparisonType"/> is not a
        /// <see cref="StringComparison"/> value.</exception>
        /// <remarks>
        /// Index numbering starts from zero. The <paramref name="startIndex"/> parameter is clamped to
        /// the valid range of the <paramref name="span"/>. Values less than zero are treated as zero, and values
        /// greater than the length of <paramref name="span"/> are treated as equal to the length of <paramref name="span"/>.
        /// If <paramref name="startIndex"/> equals the length of <paramref name="span"/>, this method
        /// returns -1 for non-empty searches. If <paramref name="value"/> is empty, this method returns
        /// the effective start index after clamping.
        /// <para/>
        /// The <paramref name="comparisonType"/> parameter specifies to search for the <paramref name="value"/>
        /// parameter using the current or invariant culture, using a case-sensitive or case-insensitive search,
        /// and using word or ordinal comparison rules.
        /// <para/>
        /// To match the behavior of the JDK, this method allows searches for the empty string, which will
        /// always return 0. This method also allows searches within an empty span.
        /// <para/>
        /// If <paramref name="startIndex"/> is less than zero, it is treated as zero. If it is greater
        /// than the length of <paramref name="span"/>, it is treated as equal to the length.
        /// <para/>
        /// On older platforms than .NET Core, this overload provides optimizations for
        /// <see cref="StringComparison.OrdinalIgnoreCase"/> over and above the System.Memory package.
        /// </remarks>
        public static int IndexOf(ReadOnlySpan<char> span, ReadOnlySpan<char> value, int startIndex, StringComparison comparisonType)
        {
            if (startIndex < 0)
                startIndex = 0;
            if (startIndex > span.Length)
                startIndex = span.Length;

            // To match the JDK, allow search for empty string
            if (value.IsEmpty)
                return startIndex;

            // Once clamped to Length, there is no searchable content remaining.
            if (startIndex == span.Length)
                return -1;

            // If value is longer than the remaining searchable range, impossible to match
            if (value.Length > span.Length - startIndex)
                return -1;

            ReadOnlySpan<char> toSearch = span.Slice(startIndex);
#if FEATURE_MEMORYEXTENSIONS_INDEXOF_STRINGCOMPARISON
            // J2N: Hardware optimized, where possible
            int actualIndex = System.MemoryExtensions.IndexOf(toSearch, value, comparisonType);
#else
            CheckStringComparison(comparisonType);
            int actualIndex;

            if (comparisonType == StringComparison.Ordinal)
            {
                actualIndex = System.MemoryExtensions.IndexOf(toSearch, value);
            }
            else if (comparisonType == StringComparison.OrdinalIgnoreCase)
            {
                // Allocation-free common case for .NET Framework.
                actualIndex = Ordinal.IndexOfOrdinalIgnoreCase(toSearch, value);
            }
            else
            {
                // Hack for platforms older than .NET Core, since this overload didn't exist.
                // J2N TODO: Optimize (this is rarely used)
                actualIndex = IndexOfSlow(toSearch, value, comparisonType);
            }
#endif
            return actualIndex >= 0 ? startIndex + actualIndex : -1; // Overflow prevented by clamping
        }

#if !FEATURE_MEMORYEXTENSIONS_INDEXOF_STRINGCOMPARISON
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static int IndexOfSlow(ReadOnlySpan<char> span, ReadOnlySpan<char> value, StringComparison comparisonType)
            => span.ToString().IndexOf(value.ToString(), comparisonType);

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static int IndexOfSlow(ReadOnlySpan<char> span, string value, StringComparison comparisonType)
            => span.ToString().IndexOf(value, comparisonType);
#endif
        #endregion IndexOf

        #region LastIndexOf

        /// <summary>
        /// Reports the zero-based index of the last occurrence of the specified <paramref name="value"/> in the
        /// specified <paramref name="span"/>.
        /// </summary>
        /// <param name="span">The source span.</param>
        /// <param name="value">The value to seek within the source span.</param>
        /// <returns>The zero-based starting index position of value if that string is found, or -1
        /// if it is not found. If <paramref name="value"/> is <see cref="string.Empty"/>,
        /// it returns the length of <paramref name="span"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <c>null</c>.</exception>
        /// <remarks>
        /// Index numbering starts from zero. That is, the first character in the string
        /// is at index zero and the last is at <paramref name="span"/>.Length - 1.
        /// <para/>
        /// This method begins searching at the last character position of the <paramref name="span"/>
        /// and proceeds backward toward the beginning until either <paramref name="value"/>
        /// is found or the first character position has been examined. The search is case-sensitive.
        /// <para/>
        /// This method performs an ordinal (culture-insensitive) search, where a character is
        /// considered equivalent to another character only if their Unicode scalar values are
        /// the same.
        /// <para/>
        /// To match the behavior of the JDK, this method allows searches for the empty string, which will
        /// always return the length of <paramref name="span"/>. This method also allows searches within an empty span.
        /// </remarks>
        public static int LastIndexOf(ReadOnlySpan<char> span, string value)
        {
            if (value is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.value);

            // To match the JDK, allow search for empty string
            if (value.Length == 0)
                return span.Length;

            // If value is longer than the remaining searchable range, impossible to match
            if (value.Length > span.Length)
                return -1;

            return System.MemoryExtensions.LastIndexOf(span, value.AsSpan()); // J2N: Hardware optimized, where possible
        }

        /// <summary>
        /// Reports the zero-based index of the last occurrence of the specified <paramref name="value"/> in the
        /// specified <paramref name="span"/>.
        /// </summary>
        /// <param name="span">The source span.</param>
        /// <param name="value">The value to seek within the source span.</param>
        /// <returns>The zero-based starting index position of value if that string is found, or -1
        /// if it is not found. If <paramref name="value"/> is <see cref="ReadOnlySpan{T}.Empty"/>,
        /// it returns the length of <paramref name="span"/>.</returns>
        /// <remarks>
        /// Index numbering starts from zero. That is, the first character in the string
        /// is at index zero and the last is at <paramref name="span"/>.Length - 1.
        /// <para/>
        /// This method begins searching at the last character position of the <paramref name="span"/>
        /// and proceeds backward toward the beginning until either <paramref name="value"/>
        /// is found or the first character position has been examined. The search is case-sensitive.
        /// <para/>
        /// This method performs an ordinal (culture-insensitive) search, where a character is
        /// considered equivalent to another character only if their Unicode scalar values are
        /// the same.
        /// <para/>
        /// To match the behavior of the JDK, this method allows searches for the empty string, which will
        /// always return the length of <paramref name="span"/>. This method also allows searches within an empty span.
        /// </remarks>
        public static int LastIndexOf(ReadOnlySpan<char> span, ReadOnlySpan<char> value)
        {
            // To match the JDK, allow search for empty string
            if (value.IsEmpty)
                return span.Length;

            // If value is longer than the remaining searchable range, impossible to match
            if (value.Length > span.Length)
                return -1;

            return System.MemoryExtensions.LastIndexOf(span, value); // J2N: Hardware optimized, where possible
        }

        /// <summary>
        /// Reports the zero-based index of the last occurrence of the specified <paramref name="value"/>
        /// beginning at the specified index in the specified <paramref name="span"/>.
        /// </summary>
        /// <param name="span">The source span.</param>
        /// <param name="value">The value to seek within the source span.</param>
        /// <param name="startIndex">The search starting position. The search proceeds from
        /// <paramref name="startIndex"/> toward the beginning of the specified <paramref name="span"/>.</param>
        /// <returns>The zero-based starting index position of value if that string is found, or -1
        /// if it is not found. If <paramref name="value"/> is <see cref="string.Empty"/>,
        /// it returns <paramref name="startIndex"/> if it is within the bounds of the span; otherwise,
        /// if <paramref name="startIndex"/> is greater than the length of <paramref name="span"/>, it
        /// returns the length of <paramref name="span"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <c>null</c>.</exception>
        /// <remarks>
        /// Index numbering starts from zero. That is, the first character in the string
        /// is at index zero and the last is at <paramref name="span"/>.Length - 1.
        /// <para/>
        /// The search begins at the effective starting position and proceeds backward until either
        /// <paramref name="value"/> is found or the first character position has been examined.
        /// The effective starting position is the lesser of <paramref name="startIndex"/> and
        /// <paramref name="span"/>.Length - <paramref name="value"/>.Length. For example, if
        /// <paramref name="startIndex"/> is <paramref name="span"/>.Length - 1 and <paramref name="value"/>
        /// contains a single character, the method searches every character from the last
        /// character in the <paramref name="span"/> to the beginning. The search is case-sensitive.
        /// <para/>
        /// This method performs an ordinal (culture-insensitive) search, where a character is
        /// considered equivalent to another character only if their Unicode scalar values are
        /// the same.
        /// <para/>
        /// To match the behavior of the JDK, this method allows searches for the empty string, which will
        /// always return the lesser of <paramref name="startIndex"/> or the length of <paramref name="span"/>.
        /// This method also allows searches within an empty span.
        /// <para/>
        /// If <paramref name="startIndex"/> is less than zero, the method returns -1. If it is greater than
        /// the length of <paramref name="span"/>, it is treated as equal to the length.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int LastIndexOf(ReadOnlySpan<char> span, string value, int startIndex)
        {
            if (value is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.value);

            return LastIndexOf(span, value.AsSpan(), startIndex);
        }

        /// <summary>
        /// Reports the zero-based index of the last occurrence of the specified <paramref name="value"/>
        /// beginning at the specified index in the specified <paramref name="span"/>.
        /// </summary>
        /// <param name="span">The source span.</param>
        /// <param name="value">The value to seek within the source span.</param>
        /// <param name="startIndex">The search starting position. The search proceeds from <paramref name="startIndex"/> toward the
        /// beginning of the <paramref name="span"/>.</param>
        /// <returns>The zero-based starting index position of value if that string is found, or -1
        /// if it is not found. If <paramref name="value"/> is <see cref="ReadOnlySpan{T}.Empty"/>,
        /// it returns <paramref name="startIndex"/> if it is within the bounds of the <paramref name="span"/>; otherwise, if
        /// <paramref name="startIndex"/> is greater than the length of <paramref name="span"/>, it returns
        /// the length of <paramref name="span"/>.</returns>
        /// <remarks>
        /// Index numbering starts from zero. That is, the first character in the string
        /// is at index zero and the last is at <paramref name="span"/>.Length - 1.
        /// <para/>
        /// The search begins at the effective starting position and proceeds backward until either
        /// <paramref name="value"/> is found or the first character position has been examined.
        /// The effective starting position is the lesser of <paramref name="startIndex"/> and
        /// <paramref name="span"/>.Length - <paramref name="value"/>.Length. For example, if
        /// <paramref name="startIndex"/> is <paramref name="span"/>.Length - 1 and <paramref name="value"/>
        /// contains a single character, the method searches every character from the last
        /// character in the <paramref name="span"/> to the beginning. The search is case-sensitive.
        /// <para/>
        /// This method performs an ordinal (culture-insensitive) search, where a character is
        /// considered equivalent to another character only if their Unicode scalar values are
        /// the same.
        /// <para/>
        /// To match the behavior of the JDK, this method allows searches for the empty string, which will
        /// always return the lesser of <paramref name="startIndex"/> or the length of <paramref name="span"/>.
        /// This method also allows searches within an empty span.
        /// <para/>
        /// If <paramref name="startIndex"/> is less than zero, the method returns -1. If it is greater than
        /// the length of <paramref name="span"/>, it is treated as equal to the length.
        /// </remarks>
        public static int LastIndexOf(ReadOnlySpan<char> span, ReadOnlySpan<char> value, int startIndex)
        {
            if (startIndex < 0)
                return -1; // JDK behavior
            if (startIndex > span.Length)
                startIndex = span.Length;

            // To match the JDK, allow search for empty string
            if (value.IsEmpty)
                return startIndex;

            // If value is longer than the remaining searchable range, impossible to match
            if (value.Length > span.Length)
                return -1;

            // Last valid starting position
            int maxStart = span.Length - value.Length;

            if (startIndex > maxStart)
                startIndex = maxStart;

            Debug.Assert(startIndex >= 0);

            // J2N: Hardware optimized, where possible
            return System.MemoryExtensions.LastIndexOf(span.Slice(0, startIndex + value.Length), value); // Overflow prevented by clamping and maxStart logic
        }

        /// <summary>
        /// Reports the zero-based index of the last occurrence of the specified <paramref name="value"/> in the
        /// specified <paramref name="span"/>.
        /// </summary>
        /// <param name="span">The source span.</param>
        /// <param name="value">The value to seek within the source span.</param>
        /// <param name="comparisonType">One of the enumeration values that determines how the <paramref name="span"/>
        /// and <paramref name="value"/> are compared.</param>
        /// <returns>The zero-based starting index position of value if that string is found, or -1
        /// if it is not found. If <paramref name="value"/> is <see cref="string.Empty"/>,
        /// it returns the length of <paramref name="span"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException"><paramref name="comparisonType"/> is not a
        /// <see cref="StringComparison"/> value.</exception>
        /// <remarks>
        /// Index numbering starts from zero. That is, the first character in the string
        /// is at index zero and the last is at <paramref name="span"/>.Length - 1.
        /// <para/>
        /// This method begins searching at the last character position of the <paramref name="span"/>
        /// and proceeds backward toward the beginning until either <paramref name="value"/>
        /// is found or the first character position has been examined.
        /// <para/>
        /// The <paramref name="comparisonType"/> parameter specifies to search for the <paramref name="value"/>
        /// parameter using the current or invariant culture, using a case-sensitive or case-insensitive search,
        /// and using word or ordinal comparison rules.
        /// <para/>
        /// To match the behavior of the JDK, this method allows searches for the empty string, which will
        /// always return the length of <paramref name="span"/>. This method also allows searches within an empty span.
        /// <para/>
        /// On older platforms than .NET Core, this overload provides optimizations for
        /// <see cref="StringComparison.OrdinalIgnoreCase"/> over and above the System.Memory package.
        /// </remarks>
        public static int LastIndexOf(ReadOnlySpan<char> span, string value, StringComparison comparisonType)
        {
            if (value is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.value);

            // To match the JDK, allow search for empty string
            if (value.Length == 0)
                return span.Length;

            // If value is longer than the remaining searchable range, impossible to match
            if (value.Length > span.Length)
                return -1;

#if FEATURE_MEMORYEXTENSIONS_LASTINDEXOF_STRINGCOMPARISON
            return System.MemoryExtensions.LastIndexOf(span, value, comparisonType); // J2N: Hardware optimized, where possible
#else
            CheckStringComparison(comparisonType);

            if (comparisonType == StringComparison.Ordinal)
            {
                return span.LastIndexOf(value);
            }
            else if (comparisonType == StringComparison.OrdinalIgnoreCase)
            {
                // Allocation-free common case for .NET Framework.
                return Ordinal.LastIndexOfOrdinalIgnoreCase(span, value);
            }

            // Hack for platforms older than .NET Core, since this overload didn't exist.
            // J2N TODO: Optimize (this is rarely used)
            return LastIndexOfSlow(span, value, comparisonType);
#endif
        }

        /// <summary>
        /// Reports the zero-based index of the last occurrence of the specified <paramref name="value"/> in the
        /// specified <paramref name="span"/>.
        /// </summary>
        /// <param name="span">The source span.</param>
        /// <param name="value">The value to seek within the source span.</param>
        /// <param name="comparisonType">One of the enumeration values that determines how the <paramref name="span"/>
        /// and <paramref name="value"/> are compared.</param>
        /// <returns>The zero-based starting index position of value if that string is found, or -1
        /// if it is not found. If <paramref name="value"/> is <see cref="ReadOnlySpan{T}.Empty"/>,
        /// it returns the length of <paramref name="span"/>.</returns>
        /// <exception cref="ArgumentException"><paramref name="comparisonType"/> is not a
        /// <see cref="StringComparison"/> value.</exception>
        /// <remarks>
        /// Index numbering starts from zero. That is, the first character in the string
        /// is at index zero and the last is at <paramref name="span"/>.Length - 1.
        /// <para/>
        /// This method begins searching at the last character position of the <paramref name="span"/>
        /// and proceeds backward toward the beginning until either <paramref name="value"/>
        /// is found or the first character position has been examined.
        /// <para/>
        /// The <paramref name="comparisonType"/> parameter specifies to search for the <paramref name="value"/>
        /// parameter using the current or invariant culture, using a case-sensitive or case-insensitive search,
        /// and using word or ordinal comparison rules.
        /// <para/>
        /// To match the behavior of the JDK, this method allows searches for the empty string, which will
        /// always return the length of <paramref name="span"/>. This method also allows searches within an empty span.
        /// <para/>
        /// On older platforms than .NET Core, this overload provides optimizations for
        /// <see cref="StringComparison.OrdinalIgnoreCase"/> over and above the System.Memory package.
        /// </remarks>
        public static int LastIndexOf(ReadOnlySpan<char> span, ReadOnlySpan<char> value, StringComparison comparisonType)
        {
            // To match the JDK, allow search for empty string
            if (value.IsEmpty)
                return span.Length;

            // If value is longer than the remaining searchable range, impossible to match
            if (value.Length > span.Length)
                return -1;

#if FEATURE_MEMORYEXTENSIONS_LASTINDEXOF_STRINGCOMPARISON
            return System.MemoryExtensions.LastIndexOf(span, value, comparisonType); // J2N: Hardware optimized, where possible
#else
            CheckStringComparison(comparisonType);

            if (comparisonType == StringComparison.Ordinal)
            {
                return span.LastIndexOf(value);
            }
            else if (comparisonType == StringComparison.OrdinalIgnoreCase)
            {
                // Allocation-free common case for .NET Framework.
                return Ordinal.LastIndexOfOrdinalIgnoreCase(span, value);
            }

            // Hack for platforms older than .NET Core, since this overload didn't exist.
            // J2N TODO: Optimize (this is rarely used)
            return LastIndexOfSlow(span, value, comparisonType);
#endif
        }

        /// <summary>
        /// Reports the zero-based index of the last occurrence of the specified <paramref name="value"/>
        /// beginning at the specified index in the specified <paramref name="span"/>.
        /// </summary>
        /// <param name="span">The source span.</param>
        /// <param name="value">The value to seek within the source span.</param>
        /// <param name="startIndex">The search starting position. The search proceeds from <paramref name="startIndex"/> toward the
        /// beginning of the <paramref name="span"/>.</param>
        /// <param name="comparisonType">One of the enumeration values that determines how the <paramref name="span"/>
        /// and <paramref name="value"/> are compared.</param>
        /// <returns>The zero-based starting index position of value if that string is found, or -1
        /// if it is not found. If <paramref name="value"/> is <see cref="string.Empty"/>,
        /// it returns <paramref name="startIndex"/> if it is within the bounds of the <paramref name="span"/>; otherwise, if
        /// <paramref name="startIndex"/> is greater than the length of <paramref name="span"/>, it returns
        /// the length of <paramref name="span"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException"><paramref name="comparisonType"/> is not a
        /// <see cref="StringComparison"/> value.</exception>
        /// <remarks>
        /// Index numbering starts from zero. That is, the first character in the string
        /// is at index zero and the last is at <paramref name="span"/>.Length - 1.
        /// <para/>
        /// The search begins at the effective starting position and proceeds backward until either
        /// <paramref name="value"/> is found or the first character position has been examined.
        /// The effective starting position is the lesser of <paramref name="startIndex"/> and
        /// <paramref name="span"/>.Length - <paramref name="value"/>.Length. For example, if
        /// <paramref name="startIndex"/> is <paramref name="span"/>.Length - 1 and <paramref name="value"/>
        /// contains a single character, the method searches every character from the last
        /// character in the string to the beginning.
        /// <para/>
        /// The <paramref name="comparisonType"/> parameter specifies to search for the <paramref name="value"/>
        /// parameter using the current or invariant culture, using a case-sensitive or case-insensitive search,
        /// and using word or ordinal comparison rules.
        /// <para/>
        /// To match the behavior of the JDK, this method allows searches for the empty string, which will
        /// always return the lesser of <paramref name="startIndex"/> or the length of <paramref name="span"/>.
        /// This method also allows searches within an empty span.
        /// <para/>
        /// If <paramref name="startIndex"/> is less than zero, the method returns -1. If it is greater than
        /// the length of <paramref name="span"/>, it is treated as equal to the length.
        /// <para/>
        /// On older platforms than .NET Core, this overload provides optimizations for
        /// <see cref="StringComparison.OrdinalIgnoreCase"/> over and above the System.Memory package.
        /// </remarks>
        public static int LastIndexOf(ReadOnlySpan<char> span, string value, int startIndex, StringComparison comparisonType)
        {
            if (value is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.value);

            if (startIndex < 0)
                return -1; // JDK behavior
            if (startIndex > span.Length)
                startIndex = span.Length;

            // To match the JDK, allow search for empty string
            if (value.Length == 0)
                return startIndex;

            // If value is longer than the remaining searchable range, impossible to match
            if (value.Length > span.Length)
                return -1;

            // Last valid starting position
            int maxStart = span.Length - value.Length;

            if (startIndex > maxStart)
                startIndex = maxStart;

            Debug.Assert(startIndex >= 0);

            ReadOnlySpan<char> toSearch = span.Slice(0, startIndex + value.Length); // Overflow prevented by clamping and maxStart logic
#if FEATURE_MEMORYEXTENSIONS_LASTINDEXOF_STRINGCOMPARISON
            return System.MemoryExtensions.LastIndexOf(toSearch, value.AsSpan(), comparisonType); // J2N: Hardware optimized, where possible
#else
            CheckStringComparison(comparisonType);

            if (comparisonType == StringComparison.Ordinal)
            {
                return toSearch.LastIndexOf(value);
            }
            else if (comparisonType == StringComparison.OrdinalIgnoreCase)
            {
                // Allocation-free common case for .NET Framework.
                return Ordinal.LastIndexOfOrdinalIgnoreCase(toSearch, value);
            }

            // Hack for platforms older than .NET Core, since this overload didn't exist.
            // J2N TODO: Optimize (this is rarely used)
            return LastIndexOfSlow(toSearch, value, comparisonType);
#endif
        }

        /// <summary>
        /// Reports the zero-based index of the last occurrence of the specified <paramref name="value"/>
        /// beginning at the specified index in the specified <paramref name="span"/>.
        /// </summary>
        /// <param name="span">The source span.</param>
        /// <param name="value">The value to seek within the source span.</param>
        /// <param name="startIndex">The search starting position. The search proceeds from <paramref name="startIndex"/> toward the
        /// beginning of the <paramref name="span"/>.</param>
        /// <param name="comparisonType">One of the enumeration values that determines how the <paramref name="span"/>
        /// and <paramref name="value"/> are compared.</param>
        /// <returns>The zero-based starting index position of value if that string is found, or -1
        /// if it is not found. If <paramref name="value"/> is <see cref="ReadOnlySpan{T}.Empty"/>,
        /// it returns <paramref name="startIndex"/> if it is within the bounds of the <paramref name="span"/>; otherwise, if
        /// <paramref name="startIndex"/> is greater than the length of <paramref name="span"/>, it returns
        /// the length of <paramref name="span"/>.</returns>
        /// <exception cref="ArgumentException"><paramref name="comparisonType"/> is not a
        /// <see cref="StringComparison"/> value.</exception>
        /// <remarks>
        /// Index numbering starts from zero. That is, the first character in the string
        /// is at index zero and the last is at <paramref name="span"/>.Length - 1.
        /// <para/>
        /// The search begins at the effective starting position and proceeds backward until either
        /// <paramref name="value"/> is found or the first character position has been examined.
        /// The effective starting position is the lesser of <paramref name="startIndex"/> and
        /// <paramref name="span"/>.Length - <paramref name="value"/>.Length. For example, if
        /// <paramref name="startIndex"/> is <paramref name="span"/>.Length - 1 and <paramref name="value"/>
        /// contains a single character, the method searches every character from the last
        /// character in the string to the beginning.
        /// <para/>
        /// The <paramref name="comparisonType"/> parameter specifies to search for the <paramref name="value"/>
        /// parameter using the current or invariant culture, using a case-sensitive or case-insensitive search,
        /// and using word or ordinal comparison rules.
        /// <para/>
        /// To match the behavior of the JDK, this method allows searches for the empty string, which will
        /// always return the lesser of <paramref name="startIndex"/> or the length of <paramref name="span"/>.
        /// This method also allows searches within an empty span.
        /// <para/>
        /// If <paramref name="startIndex"/> is less than zero, the method returns -1. If it is greater than
        /// the length of <paramref name="span"/>, it is treated as equal to the length.
        /// <para/>
        /// On older platforms than .NET Core, this overload provides optimizations for
        /// <see cref="StringComparison.OrdinalIgnoreCase"/> over and above the System.Memory package.
        /// </remarks>
        public static int LastIndexOf(ReadOnlySpan<char> span, ReadOnlySpan<char> value, int startIndex, StringComparison comparisonType)
        {
            if (startIndex < 0)
                return -1; // JDK behavior
            if (startIndex > span.Length)
                startIndex = span.Length;

            // To match the JDK, allow search for empty string
            if (value.IsEmpty)
                return startIndex;

            // If value is longer than the remaining searchable range, impossible to match
            if (value.Length > span.Length)
                return -1;

            // Last valid starting position
            int maxStart = span.Length - value.Length;

            if (startIndex > maxStart)
                startIndex = maxStart;

            Debug.Assert(startIndex >= 0);

            ReadOnlySpan<char> toSearch = span.Slice(0, startIndex + value.Length); // Overflow prevented by clamping and maxStart logic
#if FEATURE_MEMORYEXTENSIONS_LASTINDEXOF_STRINGCOMPARISON
            return System.MemoryExtensions.LastIndexOf(toSearch, value, comparisonType); // J2N: Hardware optimized, where possible
#else
            CheckStringComparison(comparisonType);

            if (comparisonType == StringComparison.Ordinal)
            {
                return toSearch.LastIndexOf(value);
            }
            else if (comparisonType == StringComparison.OrdinalIgnoreCase)
            {
                // Allocation-free common case for .NET Framework.
                return Ordinal.LastIndexOfOrdinalIgnoreCase(toSearch, value);
            }

            // Hack for platforms older than .NET Core, since this overload didn't exist.
            // J2N TODO: Optimize (this is rarely used)
            return LastIndexOfSlow(toSearch, value, comparisonType);
#endif
        }

#if !FEATURE_MEMORYEXTENSIONS_INDEXOF_STRINGCOMPARISON
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static int LastIndexOfSlow(ReadOnlySpan<char> span, ReadOnlySpan<char> value, StringComparison comparisonType)
            => span.ToString().LastIndexOf(value.ToString(), comparisonType);

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static int LastIndexOfSlow(ReadOnlySpan<char> span, string value, StringComparison comparisonType)
            => span.ToString().LastIndexOf(value, comparisonType);
#endif

        #endregion LastIndexOf

#if !FEATURE_MEMORYEXTENSIONS_INDEXOF_STRINGCOMPARISON
        private static void CheckStringComparison(StringComparison comparisonType)
        {
            if (comparisonType < StringComparison.CurrentCulture || comparisonType > StringComparison.OrdinalIgnoreCase)
                ThrowHelper.ThrowArgumentException(ExceptionResource.NotSupported_StringComparison, ExceptionArgument.comparisonType);
        }
#endif
    }
}
