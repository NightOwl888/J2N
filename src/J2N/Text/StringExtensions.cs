#region Copyright 2010 by Apache Harmony, Licensed under the Apache License, Version 2.0
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

using J2N.Buffers;
using System;
using System.Buffers;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace J2N.Text
{
    using SR = J2N.Resources.Strings;

    /// <summary>
    /// Extenions to the <see cref="System.String"/> class.
    /// </summary>
    public static class StringExtensions
    {
        private const int CharStackBufferSize = 64;

        #region AsCharSequence

        /// <summary>
        /// Convenience method to wrap a string in a <see cref="StringCharSequence"/>
        /// so a <see cref="string"/> can be used as <see cref="ICharSequence"/> in .NET.
        /// </summary>
        public static ICharSequence AsCharSequence(this string? text)
        {
            return new StringCharSequence(text);
        }

        #endregion AsCharSequence

        #region CompareToOrdinal

        /// <summary>
        /// This method mimics the Java String.compareTo(CharSequence) method in that it
        /// <list type="number">
        ///     <item><description>Compares the strings using lexographic sorting rules</description></item>
        ///     <item><description>Performs a culture-insensitive comparison</description></item>
        /// </list>
        /// This method is a convenience to replace the .NET CompareTo method 
        /// on all strings, provided the logic does not expect specific values
        /// but is simply comparing them with <c>&gt;</c> or <c>&lt;</c>.
        /// </summary>
        /// <param name="str">This string.</param>
        /// <param name="value">The string to compare with.</param>
        /// <returns>
        /// An integer that indicates the lexical relationship between the two comparands.
        /// Less than zero indicates the comparison value is greater than the current string.
        /// Zero indicates the strings are equal.
        /// Greater than zero indicates the comparison value is less than the current string.
        /// </returns>
        public static int CompareToOrdinal(this string? str, ICharSequence? value) // KEEP OVERLOADS FOR ReadOnlySpan<char>, ICharSequence, char[], StringBuilder, and string IN SYNC
        {
            if (str is null) return (value is null || !value.HasValue) ? 0 : -1;
            if (value is null || !value.HasValue) return 1;
            if (value is StringCharSequence s && object.ReferenceEquals(str, s.Value)) return 0;

            if (value is StringBuilderCharSequence sb)
                return CompareToOrdinal(str, sb.Value);
            if (value is StringBuffer stringBuffer)
                return CompareToOrdinal(str, stringBuffer.builder);

            int length = Math.Min(str.Length, value.Length);
            int result;
            for (int i = 0; i < length; i++)
            {
                if ((result = str[i] - value[i]) != 0)
                    return result;
            }

            // At this point, we have compared all the characters in at least one string.
            // The longer string will be larger.
            return str.Length - value.Length;
        }

        /// <summary>
        /// This method mimics the Java String.compareTo(CharSequence) method in that it
        /// <list type="number">
        ///     <item><description>Compares the strings using lexographic sorting rules</description></item>
        ///     <item><description>Performs a culture-insensitive comparison</description></item>
        /// </list>
        /// This method is a convenience to replace the .NET CompareTo method 
        /// on all strings, provided the logic does not expect specific values
        /// but is simply comparing them with <c>&gt;</c> or <c>&lt;</c>.
        /// </summary>
        /// <param name="str">This string.</param>
        /// <param name="value">The string to compare with.</param>
        /// <returns>
        /// An integer that indicates the lexical relationship between the two comparands.
        /// Less than zero indicates the comparison value is greater than the current string.
        /// Zero indicates the strings are equal.
        /// Greater than zero indicates the comparison value is less than the current string.
        /// </returns>
        public static int CompareToOrdinal(this string? str, char[]? value) // KEEP OVERLOADS FOR ReadOnlySpan<char>, ICharSequence, char[], StringBuilder, and string IN SYNC
        {
            if (str is null) return (value is null) ? 0 : -1;
            if (value is null) return 1;

            unsafe
            {
                fixed (char* valuePtr = value)
                {
                    return CompareToOrdinalCore(str, valuePtr, value.Length);
                }
            }
        }

        /// <summary>
        /// This method mimics the Java String.compareTo(CharSequence) method in that it
        /// <list type="number">
        ///     <item><description>Compares the strings using lexographic sorting rules</description></item>
        ///     <item><description>Performs a culture-insensitive comparison</description></item>
        /// </list>
        /// This method is a convenience to replace the .NET CompareTo method 
        /// on all strings, provided the logic does not expect specific values
        /// but is simply comparing them with <c>&gt;</c> or <c>&lt;</c>.
        /// </summary>
        /// <param name="str">This string.</param>
        /// <param name="value">The string to compare with.</param>
        /// <returns>
        /// An integer that indicates the lexical relationship between the two comparands.
        /// Less than zero indicates the comparison value is greater than the current string.
        /// Zero indicates the strings are equal.
        /// Greater than zero indicates the comparison value is less than the current string.
        /// </returns>
        public static int CompareToOrdinal(this string? str, StringBuilder? value) // KEEP OVERLOADS FOR ReadOnlySpan<char>, ICharSequence, char[], StringBuilder, and string IN SYNC
        {
            if (str is null) return (value is null) ? 0 : -1;
            if (value is null) return 1;

            int length = Math.Min(str.Length, value.Length);
            char[]? arrayToReturnToPool = null;
            try
            {
#if FEATURE_STRINGBUILDER_COPYTO_SPAN // If this method isn't supported, we are buffering to an array pool to get to the stack, anyway.
                Span<char> valueChars = length > CharStackBufferSize
                    ? (arrayToReturnToPool = ArrayPool<char>.Shared.Rent(length))
                    : stackalloc char[length];
                value.CopyTo(0, valueChars, length);
#else
                Span<char> valueChars = arrayToReturnToPool = ArrayPool<char>.Shared.Rent(length);
                value.CopyTo(0, arrayToReturnToPool, 0, length);
#endif
                int result;
                for (int i = 0; i < length; i++)
                {
                    if ((result = str[i] - valueChars[i]) != 0)
                        return result;
                }

                // At this point, we have compared all the characters in at least one string.
                // The longer string will be larger.
                return str.Length - value.Length;
            }
            finally
            {
                ArrayPool<char>.Shared.ReturnIfNotNull(arrayToReturnToPool);
            }
        }

        /// <summary>
        /// This method mimics the Java String.compareTo(String) method in that it
        /// <list type="number">
        ///     <item><description>Compares the strings using lexographic sorting rules</description></item>
        ///     <item><description>Performs a culture-insensitive comparison</description></item>
        /// </list>
        /// This method is a convenience to replace the .NET CompareTo method 
        /// on all strings, provided the logic does not expect specific values
        /// but is simply comparing them with <c>&gt;</c> or <c>&lt;</c>.
        /// </summary>
        /// <param name="str">This string.</param>
        /// <param name="value">The string to compare with.</param>
        /// <returns>
        /// An integer that indicates the lexical relationship between the two comparands.
        /// Less than zero indicates the comparison value is greater than the current string.
        /// Zero indicates the strings are equal.
        /// Greater than zero indicates the comparison value is less than the current string.
        /// </returns>
        public static int CompareToOrdinal(this string? str, string? value) // KEEP OVERLOADS FOR ReadOnlySpan<char>, ICharSequence, char[], StringBuilder, and string IN SYNC
        {
            return string.CompareOrdinal(str, value);
        }

        /// <summary>
        /// This method mimics the Java String.compareTo(CharSequence) method in that it
        /// <list type="number">
        ///     <item><description>Compares the strings using lexographic sorting rules</description></item>
        ///     <item><description>Performs a culture-insensitive comparison</description></item>
        /// </list>
        /// This method is a convenience to replace the .NET CompareTo method 
        /// on all strings, provided the logic does not expect specific values
        /// but is simply comparing them with <c>&gt;</c> or <c>&lt;</c>.
        /// </summary>
        /// <param name="str">This string.</param>
        /// <param name="value">The string to compare with.</param>
        /// <returns>
        /// An integer that indicates the lexical relationship between the two comparands.
        /// Less than zero indicates the comparison value is greater than the current string.
        /// Zero indicates the strings are equal.
        /// Greater than zero indicates the comparison value is less than the current string.
        /// </returns>
        public static int CompareToOrdinal(this string? str, ReadOnlySpan<char> value) // KEEP OVERLOADS FOR ReadOnlySpan<char>, ICharSequence, char[], StringBuilder, and string IN SYNC
        {
            // J2N: Only consider whether the right side is empty if the left side is null. This is the equivalent of calling str.AsSpan() and then doing the comparison. See: https://github.com/NightOwl888/J2N/pull/122#discussion_r1850836158
            if (str is null) return value.IsEmpty ? 0 : -1;

            unsafe
            {
                fixed (char* valuePtr = &MemoryMarshal.GetReference(value))
                {
                    return CompareToOrdinalCore(str, valuePtr, value.Length);
                }
            }
        }

        private unsafe static int CompareToOrdinalCore(string str, char* value, int valueLength)
        {
            int length = Math.Min(str.Length, valueLength);
            int result;
            for (int i = 0; i < length; i++)
            {
                if ((result = str[i] - value[i]) != 0)
                    return result;
            }

            // At this point, we have compared all the characters in at least one string.
            // The longer string will be larger.
            return str.Length - valueLength;
        }

        ///// <summary>
        ///// This method mimics the Java String.compareToIgnoreCase(String) method in that it
        ///// <list type="number">
        /////     <item><description>Compares the strings using lexographic sorting rules</description></item>
        /////     <item><description>Performs a case-insensitive and culture-insensitive comparison</description></item>
        ///// </list>
        ///// This method is a convenience to replace the .NET CompareTo method 
        ///// on all strings, provided the logic does not expect specific values
        ///// but is simply comparing them with <c>&gt;</c> or <c>&lt;</c>.
        ///// </summary>
        ///// <param name="str">This string.</param>
        ///// <param name="value">The string to compare with.</param>
        ///// <returns>
        ///// An integer that indicates the lexical relationship between the two comparands.
        ///// Less than zero indicates the comparison value is greater than the current string.
        ///// Zero indicates the strings are equal.
        ///// Greater than zero indicates the comparison value is less than the current string.
        ///// </returns>
        //public static int CompareToOrdinalIgnoreCase(this string str, string value)
        //{
        //    return StringComparer.OrdinalIgnoreCase.Compare(str, value);
        //}

        #endregion CompareToOrdinal

        #region Contains

        /// <summary>
        /// Returns a value indicating whether a specified character occurs within this string.
        /// <para/>
        /// This overload is missing from the <see cref="string"/> class prior to .NET Standard 2.1,
        /// so it is being included here for convenience.
        /// </summary>
        /// <param name="input">The string in which to seek <paramref name="value"/>.</param>
        /// <param name="value">The character to seek.</param>
        /// <returns><c>true</c> if the <paramref name="value"/> parameter occurs within this string; otherwise, <c>false</c>.</returns>
        /// <remarks>This method performs an ordinal (case-sensitive and culture-insensitive) comparison.</remarks>
        /// <exception cref="ArgumentNullException"><paramref name="input"/> is <c>null</c>.</exception>
        public static bool Contains(this string input, char value) // For compatibility with < .NET Standard 2.1
        {
            if (input is null)
                throw new ArgumentNullException(nameof(input));

            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] == value)
                    return true;
            }
            return false;
        }

        #endregion

        #region ContentEquals

        /// <summary>
        /// Compares a <see cref="ICharSequence"/> to this <see cref="string"/> to determine if
        /// their contents are equal.
        /// <para/>
        /// This differs from <see cref="string.Equals(string, StringComparison)"/> in that it does not
        /// consider the <see cref="string"/> type to be part of the comparison - it will match for any character sequence
        /// that contains matching characters.
        /// <para/>
        /// The comparison is done using <see cref="StringComparison.Ordinal"/> comparison rules.
        /// </summary>
        /// <param name="text">This <see cref="string"/>.</param>
        /// <param name="charSequence">The character sequence to compare to.</param>
        /// <returns><c>true</c> if this <see cref="string"/> represents the same
        /// sequence of characters as the specified <paramref name="charSequence"/>; otherwise, <c>false</c>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ContentEquals(this string? text, ICharSequence? charSequence) // KEEP OVERLOADS FOR ReadOnlySpan<char>, ICharSequence, char[], StringBuilder, and string IN SYNC
        {
            return ContentEquals(text, charSequence, StringComparison.Ordinal);
        }

        /// <summary>
        /// Compares a <see cref="ICharSequence"/> to this <see cref="string"/> to determine if
        /// their contents are equal.
        /// <para/>
        /// This differs from <see cref="string.Equals(string, StringComparison)"/> in that it does not
        /// consider the <see cref="string"/> type to be part of the comparison - it will match for any character sequence
        /// that contains matching characters.
        /// <para/>
        /// The comparison is done using <see cref="StringComparison.Ordinal"/> comparison rules.
        /// </summary>
        /// <param name="text">This <see cref="string"/>.</param>
        /// <param name="charSequence">The character sequence to compare to.</param>
        /// <returns><c>true</c> if this <see cref="string"/> represents the same
        /// sequence of characters as the specified <paramref name="charSequence"/>; otherwise, <c>false</c>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ContentEquals(this string? text, char[]? charSequence) // KEEP OVERLOADS FOR ReadOnlySpan<char>, ICharSequence, char[], StringBuilder, and string IN SYNC
        {
            return ContentEquals(text, charSequence, StringComparison.Ordinal);
        }

        /// <summary>
        /// Compares a <see cref="ICharSequence"/> to this <see cref="string"/> to determine if
        /// their contents are equal.
        /// <para/>
        /// This differs from <see cref="string.Equals(string, StringComparison)"/> in that it does not
        /// consider the <see cref="string"/> type to be part of the comparison - it will match for any character sequence
        /// that contains matching characters.
        /// <para/>
        /// The comparison is done using <see cref="StringComparison.Ordinal"/> comparison rules.
        /// </summary>
        /// <param name="text">This <see cref="string"/>.</param>
        /// <param name="charSequence">The character sequence to compare to.</param>
        /// <returns><c>true</c> if this <see cref="string"/> represents the same
        /// sequence of characters as the specified <paramref name="charSequence"/>; otherwise, <c>false</c>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ContentEquals(this string? text, StringBuilder? charSequence) // KEEP OVERLOADS FOR ReadOnlySpan<char>, ICharSequence, char[], StringBuilder, and string IN SYNC
        {
            return ContentEquals(text, charSequence, StringComparison.Ordinal);
        }

        /// <summary>
        /// Compares a <see cref="ICharSequence"/> to this <see cref="string"/> to determine if
        /// their contents are equal.
        /// <para/>
        /// This differs from <see cref="string.Equals(string, StringComparison)"/> in that it does not
        /// consider the <see cref="string"/> type to be part of the comparison - it will match for any character sequence
        /// that contains matching characters.
        /// <para/>
        /// The comparison is done using <see cref="StringComparison.Ordinal"/> comparison rules.
        /// </summary>
        /// <param name="text">This <see cref="string"/>.</param>
        /// <param name="charSequence">The character sequence to compare to.</param>
        /// <returns><c>true</c> if this <see cref="string"/> represents the same
        /// sequence of characters as the specified <paramref name="charSequence"/>; otherwise, <c>false</c>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ContentEquals(this string? text, string? charSequence) // KEEP OVERLOADS FOR ReadOnlySpan<char>, ICharSequence, char[], StringBuilder, and string IN SYNC
        {
            return ContentEquals(text, charSequence, StringComparison.Ordinal);
        }

        /// <summary>
        /// Compares a <see cref="ReadOnlySpan{Char}"/> to this <see cref="string"/> to determine if
        /// their contents are equal.
        /// <para/>
        /// This differs from <see cref="string.Equals(string, StringComparison)"/> in that it does not
        /// consider the <see cref="string"/> type to be part of the comparison - it will match for any character sequence
        /// that contains matching characters.
        /// <para/>
        /// The comparison is done using <see cref="StringComparison.Ordinal"/> comparison rules.
        /// </summary>
        /// <param name="text">This <see cref="string"/>.</param>
        /// <param name="charSequence">The character sequence to compare to.</param>
        /// <returns><c>true</c> if this <see cref="string"/> represents the same
        /// sequence of characters as the specified <paramref name="charSequence"/>; otherwise, <c>false</c>.</returns>
        public static bool ContentEquals(this string? text, ReadOnlySpan<char> charSequence) // KEEP OVERLOADS FOR ReadOnlySpan<char>, ICharSequence, char[], StringBuilder, and string IN SYNC
        {
#pragma warning disable CA2265 // Do not compare Span<T> to null or default
            if (text is null)
                return charSequence == default;
#pragma warning restore CA2265 // Do not compare Span<T> to null or default

            int len = charSequence.Length;
            if (len != text.Length)
                return false;
            if (len == 0 && text.Length == 0)
                return true; // since both are empty strings

            return charSequence.Equals(text.AsSpan(), StringComparison.Ordinal);
        }

        /// <summary>
        /// Compares a <see cref="ICharSequence"/> to this <see cref="string"/> to determine if
        /// their contents are equal.
        /// <para/>
        /// This differs from <see cref="string.Equals(string, StringComparison)"/> in that it does not
        /// consider the <see cref="string"/> type to be part of the comparison - it will match for any character sequence
        /// that contains matching characters.
        /// </summary>
        /// <param name="text">This <see cref="string"/>.</param>
        /// <param name="charSequence">The character sequence to compare to.</param>
        /// <param name="comparisonType">One of the enumeration values that specifies the rules for the comparison.</param>
        /// <returns><c>true</c> if this <see cref="string"/> represents the same
        /// sequence of characters as the specified <paramref name="charSequence"/>; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="comparisonType"/> is not a <see cref="StringComparison"/> value.</exception>
        public static bool ContentEquals(this string? text, ICharSequence? charSequence, StringComparison comparisonType) // KEEP OVERLOADS FOR ReadOnlySpan<char>, ICharSequence, char[], StringBuilder, and string IN SYNC
        {
            if (text is null)
                return charSequence is null || !charSequence.HasValue;
            if (charSequence is null || !charSequence.HasValue)
                return false;

            if (charSequence is StringBuilderCharSequence sb)
                return ContentEquals(text, sb.Value, comparisonType);
            if (charSequence is StringBuffer stringBuffer)
                return ContentEquals(text, stringBuffer.builder, comparisonType);

            int len = charSequence.Length;
            if (len != text.Length)
                return false;
            if (len == 0 && text.Length == 0)
                return true; // since both are empty strings

            return RegionMatches(text, 0, charSequence, 0, len, comparisonType);
        }

        /// <summary>
        /// Compares a <see cref="ICharSequence"/> to this <see cref="string"/> to determine if
        /// their contents are equal.
        /// <para/>
        /// This differs from <see cref="string.Equals(string, StringComparison)"/> in that it does not
        /// consider the <see cref="string"/> type to be part of the comparison - it will match for any character sequence
        /// that contains matching characters.
        /// </summary>
        /// <param name="text">This <see cref="string"/>.</param>
        /// <param name="charSequence">The character sequence to compare to.</param>
        /// <param name="comparisonType">One of the enumeration values that specifies the rules for the comparison.</param>
        /// <returns><c>true</c> if this <see cref="string"/> represents the same
        /// sequence of characters as the specified <paramref name="charSequence"/>; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="comparisonType"/> is not a <see cref="StringComparison"/> value.</exception>
        public static bool ContentEquals(this string? text, char[]? charSequence, StringComparison comparisonType) // KEEP OVERLOADS FOR ReadOnlySpan<char>, ICharSequence, char[], StringBuilder, and string IN SYNC
        {
            if (text is null)
                return charSequence is null;
            if (charSequence is null)
                return false;

            int len = charSequence.Length;
            if (len != text.Length)
                return false;
            if (len == 0 && text.Length == 0)
                return true; // since both are empty strings

            return RegionMatches(text, 0, charSequence, 0, len, comparisonType);
        }

        /// <summary>
        /// Compares a <see cref="ICharSequence"/> to this <see cref="string"/> to determine if
        /// their contents are equal.
        /// <para/>
        /// This differs from <see cref="string.Equals(string, StringComparison)"/> in that it does not
        /// consider the <see cref="string"/> type to be part of the comparison - it will match for any character sequence
        /// that contains matching characters.
        /// </summary>
        /// <param name="text">This <see cref="string"/>.</param>
        /// <param name="charSequence">The character sequence to compare to.</param>
        /// <param name="comparisonType">One of the enumeration values that specifies the rules for the comparison.</param>
        /// <returns><c>true</c> if this <see cref="string"/> represents the same
        /// sequence of characters as the specified <paramref name="charSequence"/>; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="comparisonType"/> is not a <see cref="StringComparison"/> value.</exception>
        public static bool ContentEquals(this string? text, StringBuilder? charSequence, StringComparison comparisonType) // KEEP OVERLOADS FOR ReadOnlySpan<char>, ICharSequence, char[], StringBuilder, and string IN SYNC
        {
            if (text is null)
                return charSequence is null;
            if (charSequence is null)
                return false;

            int len = charSequence.Length;
            if (len != text.Length)
                return false;
            if (len == 0 && text.Length == 0)
                return true; // since both are empty strings

            return RegionMatches(text, 0, charSequence, 0, len, comparisonType);
        }

        /// <summary>
        /// Compares a <see cref="ICharSequence"/> to this <see cref="string"/> to determine if
        /// their contents are equal.
        /// <para/>
        /// This differs from <see cref="string.Equals(string, StringComparison)"/> in that it does not
        /// consider the <see cref="string"/> type to be part of the comparison - it will match for any character sequence
        /// that contains matching characters.
        /// </summary>
        /// <param name="text">This <see cref="string"/>.</param>
        /// <param name="charSequence">The character sequence to compare to.</param>
        /// <param name="comparisonType">One of the enumeration values that specifies the rules for the comparison.</param>
        /// <returns><c>true</c> if this <see cref="string"/> represents the same
        /// sequence of characters as the specified <paramref name="charSequence"/>; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="comparisonType"/> is not a <see cref="StringComparison"/> value.</exception>
        public static bool ContentEquals(this string? text, string? charSequence, StringComparison comparisonType) // KEEP OVERLOADS FOR ReadOnlySpan<char>, ICharSequence, char[], StringBuilder, and string IN SYNC
        {
            if (text is null)
                return charSequence is null;
            if (charSequence is null)
                return false;

            int len = charSequence.Length;
            if (len != text.Length)
                return false;
            if (len == 0 && text.Length == 0)
                return true; // since both are empty strings

            return RegionMatches(text, 0, charSequence, 0, len, comparisonType);
        }

        /// <summary>
        /// Compares a <see cref="ReadOnlySpan{Char}"/> to this <see cref="string"/> to determine if
        /// their contents are equal.
        /// <para/>
        /// This differs from <see cref="string.Equals(string, StringComparison)"/> in that it does not
        /// consider the <see cref="string"/> type to be part of the comparison - it will match for any character sequence
        /// that contains matching characters.
        /// </summary>
        /// <param name="text">This <see cref="string"/>.</param>
        /// <param name="charSequence">The character sequence to compare to.</param>
        /// <param name="comparisonType">One of the enumeration values that specifies the rules for the comparison.</param>
        /// <returns><c>true</c> if this <see cref="string"/> represents the same
        /// sequence of characters as the specified <paramref name="charSequence"/>; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="comparisonType"/> is not a <see cref="StringComparison"/> value.</exception>
        public static bool ContentEquals(this string? text, ReadOnlySpan<char> charSequence, StringComparison comparisonType) // KEEP OVERLOADS FOR ReadOnlySpan<char>, ICharSequence, char[], StringBuilder, and string IN SYNC
        {
#pragma warning disable CA2265 // Do not compare Span<T> to null or default
            if (text is null)
                return charSequence == default;
#pragma warning restore CA2265 // Do not compare Span<T> to null or default

            int len = charSequence.Length;
            if (len != text.Length)
                return false;
            if (len == 0 && text.Length == 0)
                return true; // since both are empty strings

            return charSequence.Equals(text.AsSpan(), comparisonType);
        }

        #endregion ContentEquals

        #region GetBytes

        /// <summary>
        /// Encodes this <see cref="string"/> into a sequence of bytes using the named
        /// <see cref="Encoding"/>, storing the result into a new byte array.
        /// </summary>
        /// <param name="text">This <see cref="string"/>.</param>
        /// <param name="encoding">A supported <see cref="Encoding"/>.</param>
        /// <returns>The resultant byte array.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="text"/> or <paramref name="encoding"/> is <c>null</c>.</exception>
        public static byte[] GetBytes(this string text, Encoding encoding)
        {
            if (text is null)
                throw new ArgumentNullException(nameof(text));
            if (encoding is null)
                throw new ArgumentNullException(nameof(encoding));

            return encoding.GetBytes(text);
        }

        #endregion

        #region IndexOf

        /// <summary>
        /// Returns the index within this string of the first occurrence of
        /// the specified character. If a character with value
        /// <paramref name="codePoint"/> occurs in the character sequence represented by
        /// this <see cref="string"/> object, then the index (in Unicode
        /// code units) of the first such occurrence is returned. For
        /// values of <paramref name="codePoint"/> in the range from 0 to 0xFFFF
        /// (inclusive), this is the smallest value <i>k</i> such that:
        /// <code>
        ///     this[(<i>k</i>] == <paramref name="codePoint"/>
        /// </code>
        /// is true. For other values of <paramref name="codePoint"/>, it is the
        /// smallest value <i>k</i> such that:
        /// <code>
        ///     this.CodePointAt(<i>k</i>) == <paramref name="codePoint"/>
        /// </code>
        /// is true. In either case, if no such character occurs in this
        /// string, then <c>-1</c> is returned.
        /// </summary>
        /// <param name="text">This <see cref="string"/>.</param>
        /// <param name="codePoint">A character (Unicode code point).</param>
        /// <returns>The index of the first occurrence of the character in the
        /// character sequence represented by this object, or
        /// <c>-1</c> if the character does not occur.
        /// </returns>
        /// <exception cref="ArgumentNullException">If <paramref name="text"/> is <c>null</c>.</exception>
        public static int IndexOf(this string text, int codePoint) // KEEP IN SYNC WITH MemoryExtensions.IndexOf()
        {
            return IndexOf(text, codePoint, 0);
        }

        /// <summary>
        /// Returns the index within this string of the first occurrence of the
        /// specified character, starting the search at the specified index.
        /// </summary>
        /// <remarks>
        /// If a character with value <paramref name="codePoint"/> occurs in the
        /// character sequence represented by this <see cref="string"/>
        /// object at an index no smaller than <paramref name="startIndex"/>, then
        /// the index of the first such occurrence is returned. For values
        /// of <paramref name="codePoint"/> in the range from 0 to 0xFFFF (inclusive),
        /// this is the smallest value <i>k</i> such that:
        /// <code>
        ///     (this[<i>k</i>] == <paramref name="codePoint"/>) &amp;&amp; (<i>k</i> &gt;= <paramref name="startIndex"/>)
        /// </code>
        /// is true. For other values of <code>ch</code>, it is the
        /// smallest value <i>k</i> such that:
        /// <code>
        ///     (this.CodePointAt(<i>k</i>) == <paramref name="codePoint"/>) &amp;&amp; (<i>k</i> &gt;= <paramref name="startIndex"/>)
        /// </code>
        /// is true. In either case, if no such character occurs in this
        /// string at or after position <paramref name="startIndex"/>, then
        /// <c>-1</c> is returned.
        /// <para/>
        /// There is no restriction on the value of <paramref name="startIndex"/>. If it
        /// is negative, it has the same effect as if it were zero: this entire
        /// string may be searched. If it is greater than the length of this
        /// string, it has the same effect as if it were equal to the length of
        /// this string: <c>-1</c> is returned.
        /// <para/>
        /// All indices are specified in <c>char</c> values
        /// (Unicode code units).
        /// </remarks>
        /// <param name="text">This <see cref="string"/>.</param>
        /// <param name="codePoint">A character (Unicode code point).</param>
        /// <param name="startIndex">The index to start the search from.</param>
        /// <returns>The index of the first occurrence of the character in the
        /// character sequence represented by this object that is greater
        /// than or equal to <paramref name="startIndex"/>, or <c>-1</c>
        /// if the character does not occur.
        /// </returns>
        /// <exception cref="ArgumentNullException">If <paramref name="text"/> is <c>null</c>.</exception>
        public static int IndexOf(this string text, int codePoint, int startIndex) // KEEP IN SYNC WITH MemoryExtensions.IndexOf()
        {
            if (text is null)
                throw new ArgumentNullException(nameof(text));

            if (startIndex < 0)
            {
                startIndex = 0;
            }
            else if (startIndex >= text.Length)
            {
                // Note: fromIndex might be near -1>>>1.
                return -1;
            }

            if (codePoint < Character.MinSupplementaryCodePoint)
            {
                // handle most cases here (ch is a BMP code point or a
                // negative value (invalid code point))
                if (codePoint >= Character.MinCodePoint)
                    return text.IndexOf((char)codePoint, startIndex);

                return -1;
            }
            else
            {
                return IndexOfSupplementary(text, codePoint, startIndex);
            }
        }

        /// <summary>
        /// Handles (rare) calls of indexOf with a supplementary character.
        /// </summary>
        private static int IndexOfSupplementary(this string text, int codePoint, int startIndex) // KEEP IN SYNC WITH MemoryExtensions.IndexOfSupplementary()
        {
            if (Character.IsValidCodePoint(codePoint))
            {
                Character.ToChars(codePoint, out char hi, out char lo); // J2N: Eliminated array allocation
                int max = text.Length - 1;
                for (int i = startIndex; i < max; i++)
                {
                    if (text[i] == hi && text[i + 1] == lo)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        #endregion IndexOf

        #region Intern

        /// <summary>
        /// Returns a canonical representation for the string object.
        /// <para/>
        /// A pool of strings, initially empty, is maintained privately.
        /// <para/>
        /// When the intern method is invoked, if the pool already contains a
        /// string equal to <paramref name="text"/> (this string) as determined by
        /// <see cref="string.CompareOrdinal(string, string)"/> method, then the string from the pool is
        /// returned. Otherwise, <paramref name="text"/> (this string) is added to the
        /// pool and a reference to <paramref name="text"/> (this string) is returned.
        /// </summary>
        /// <param name="text">This <see cref="string"/>.</param>
        /// <returns>A string that has the same contents of this string, but is guaranteed
        /// to be from a pool of unique strings.</returns>
        public static string Intern(this string text)
            => string.Intern(text);

        #endregion Intern

        #region LastIndexOf

        /// <summary>
        /// Returns the index within this string of the last occurrence of
        /// the specified character. For values of <paramref name="codePoint"/> in the
        /// range from 0 to 0xFFFF (inclusive), the index (in Unicode code
        /// units) returned is the largest value <i>k</i> such that:
        /// <code>
        ///     this[<i>k</i>] == <paramref name="codePoint"/>
        /// </code>
        /// is <c>true</c>. For other values of <paramref name="codePoint"/>, it is the
        /// largest value in <i>k</i> such that:
        /// <code>
        ///     this.CodePointAt(<i>k</i>) = <paramref name="codePoint"/>
        /// </code>
        /// is <c>true</c>. In either case, if no such character occurs in this
        /// string, then <c>-1</c> is returned.  The
        /// <paramref name="text"/> is searched backwards starting at the last character.
        /// </summary>
        /// <param name="text">This <see cref="string"/>.</param>
        /// <param name="codePoint">A character (Unicode code point).</param>
        /// <returns>
        /// The index of the last occurrence of the character in the
        /// character sequence represented by this object, or
        /// <c>-1</c> if the character does not occur.
        /// </returns>
        /// <exception cref="ArgumentNullException">If <paramref name="text"/> is <c>null</c>.</exception>
        public static int LastIndexOf(this string text, int codePoint) // KEEP IN SYNC WITH MemoryExtensions.LastIndexOf()
        {
            if (text is null)
                throw new ArgumentNullException(nameof(text));

            return LastIndexOf(text, codePoint, text.Length - 1);
        }

        /// <summary>
        /// Returns the index within this string of the last occurrence of
        /// the specified character, searching backward starting at the
        /// specified index. For values of <paramref name="codePoint"/> in the range
        /// from 0 to 0xFFFF (inclusive), the index returned is the largest
        /// value <i>k</i> such that:
        /// <code>
        ///     (this[<i>k</i>] == <paramref name="codePoint"/>) &amp;&amp; (<i>k</i> &lt;= <paramref name="startIndex"/>)
        /// </code>
        /// is <c>true</c>. For other values of <paramref name="codePoint"/>, it is the
        /// largest value <i>k</i> such that:
        /// <code>
        ///     (this.CodePointAt(<i>k</i>) == <paramref name="codePoint"/>) &amp;&amp; (<i>k</i> &lt;= <paramref name="startIndex"/>)
        /// </code>
        /// is <c>true</c>. In either case, if no such character occurs in this
        /// string at or before position <paramref name="startIndex"/>, then
        /// <c>-1</c> is returned.
        /// <para/>
        /// All indices are specified in <see cref="char"/> values
        /// (Unicode code units).
        /// </summary>
        /// <param name="text">This <see cref="string"/>.</param>
        /// <param name="codePoint">A character (Unicode code point).</param>
        /// <param name="startIndex">
        /// The index to start the search from. There is no
        /// restriction on the value of <paramref name="startIndex"/>. If it is
        /// greater than or equal to the length of this string, it has
        /// the same effect as if it were equal to one less than the
        /// length of this string: this entire string may be searched.
        /// If it is negative, it has the same effect as if it were <c>-1</c>:
        /// </param>
        /// <returns>
        /// The index of the last occurrence of the character in the
        /// character sequence represented by this object, or
        /// <c>-1</c> if the character does not occur.
        /// </returns>
        /// <exception cref="ArgumentNullException">If <paramref name="text"/> is <c>null</c>.</exception>
        public static int LastIndexOf(this string text, int codePoint, int startIndex) // KEEP IN SYNC WITH MemoryExtensions.LastIndexOf()
        {
            if (text is null)
                throw new ArgumentNullException(nameof(text));

            if (codePoint < Character.MinSupplementaryCodePoint)
            {
                // handle most cases here (ch is a BMP code point or a
                // negative value (invalid code point))
                if (codePoint >= Character.MinCodePoint)
                    return text.LastIndexOf((char)codePoint, Math.Min(startIndex, text.Length - 1));

                return -1;
            }
            else
            {
                return LastIndexOfSupplementary(text, codePoint, startIndex);
            }
        }

        /// <summary>
        /// Handles (rare) calls of lastIndexOf with a supplementary character.
        /// </summary>
        private static int LastIndexOfSupplementary(this string text, int codePoint, int startIndex) // KEEP IN SYNC WITH MemoryExtensions.LastIndexOfSupplementary()
        {
            if (Character.IsValidCodePoint(codePoint))
            {
                Character.ToChars(codePoint, out char hi, out char lo); // J2N: Eliminated array allocation
                int i = Math.Min(startIndex, text.Length - 2);
                for (; i >= 0; i--)
                {
                    if (text[i] == hi && text[i + 1] == lo)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        #endregion LastIndexOf

        #region RegionMatches

        /// <summary>
        /// Compares the specified <see cref="ICharSequence"/> to this string and compares the specified
        /// range of characters to determine if they are the same.
        /// </summary>
        /// <param name="text">This string.</param>
        /// <param name="thisStartIndex">The starting offset in this string.</param>
        /// <param name="other">The <see cref="ICharSequence"/> to compare.</param>
        /// <param name="otherStartIndex">The starting offset in the specified string.</param>
        /// <param name="length">The number of characters to compare.</param>
        /// <param name="comparisonType">One of the enumeration values that specifies the rules for the search.</param>
        /// <returns><c>true</c> if the ranges of characters are equal, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="text"/> or <paramref name="other"/> is <c>null</c></exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="comparisonType"/> is not a <see cref="StringComparison"/> value.</exception>
        public static bool RegionMatches(this string text, int thisStartIndex, ICharSequence other, int otherStartIndex, int length, StringComparison comparisonType) // KEEP OVERLOADS FOR ReadOnlySpan<char>, ICharSequence, char[], StringBuilder, and string IN SYNC
        {
            if (text is null)
                throw new ArgumentNullException(nameof(text));
            if (other is null || !other.HasValue)
                throw new ArgumentNullException(nameof(other));

            if (other is StringBuilderCharSequence sb)
                return RegionMatches(text, thisStartIndex, sb.Value!, otherStartIndex, length, comparisonType);
            if (other is StringBuffer stringBuffer)
                return RegionMatches(text, thisStartIndex, stringBuffer.builder, otherStartIndex, length, comparisonType);

            if (other.Length - otherStartIndex < length || otherStartIndex < 0)
                return false;
            if (thisStartIndex < 0 || text.Length - thisStartIndex < length)
                return false;
            if (length <= 0)
                return true;

            switch (comparisonType)
            {
                case StringComparison.Ordinal:
                    for (int i = 0; i < length; ++i)
                    {
                        if (text[thisStartIndex + i] != other[otherStartIndex + i])
                        {
                            return false;
                        }
                    }
                    return true;

                case StringComparison.OrdinalIgnoreCase:
                    int end = thisStartIndex + length;
                    char c1, c2;
                    var textInfo = CultureInfo.InvariantCulture.TextInfo;
                    while (thisStartIndex < end)
                    {
                        if ((c1 = text[thisStartIndex++]) != (c2 = other[otherStartIndex++])
                                && textInfo.ToUpper(c1) != textInfo.ToUpper(c2)
                                // Required for unicode that we test both cases
                                && textInfo.ToLower(c1) != textInfo.ToLower(c2))
                        {
                            return false;
                        }
                    }
                    return true;

                default:
                    ICharSequence sub2 = other.Subsequence(otherStartIndex, length);
                    return string.Compare(text, thisStartIndex, sub2.ToString(), 0, length, comparisonType) == 0;
            }
        }

        /// <summary>
        /// Compares the specified <see cref="T:char[]"/> to this string and compares the specified
        /// range of characters to determine if they are the same.
        /// </summary>
        /// <param name="text">This string.</param>
        /// <param name="thisStartIndex">The starting offset in this string.</param>
        /// <param name="other">The <see cref="T:char[]"/> to compare.</param>
        /// <param name="otherStartIndex">The starting offset in the specified string.</param>
        /// <param name="length">The number of characters to compare.</param>
        /// <param name="comparisonType">One of the enumeration values that specifies the rules for the search.</param>
        /// <returns><c>true</c> if the ranges of characters are equal, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="text"/> or <paramref name="other"/> is <c>null</c></exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="comparisonType"/> is not a <see cref="StringComparison"/> value.</exception>
        public static bool RegionMatches(this string text, int thisStartIndex, char[] other, int otherStartIndex, int length, StringComparison comparisonType) // KEEP OVERLOADS FOR ReadOnlySpan<char>, ICharSequence, char[], StringBuilder, and string IN SYNC
        {
            if (text is null)
                throw new ArgumentNullException(nameof(text));
            if (other is null)
                throw new ArgumentNullException(nameof(other));

            if (other.Length - otherStartIndex < length || otherStartIndex < 0)
                return false;
            if (thisStartIndex < 0 || text.Length - thisStartIndex < length)
                return false;
            if (length <= 0)
                return true;

            switch (comparisonType)
            {
                case StringComparison.Ordinal:
                    for (int i = 0; i < length; ++i)
                    {
                        if (text[thisStartIndex + i] != other[otherStartIndex + i])
                        {
                            return false;
                        }
                    }
                    return true;

                case StringComparison.OrdinalIgnoreCase:
                    int end = thisStartIndex + length;
                    char c1, c2;
                    var textInfo = CultureInfo.InvariantCulture.TextInfo;
                    while (thisStartIndex < end)
                    {
                        if ((c1 = text[thisStartIndex++]) != (c2 = other[otherStartIndex++])
                                && textInfo.ToUpper(c1) != textInfo.ToUpper(c2)
                                // Required for unicode that we test both cases
                                && textInfo.ToLower(c1) != textInfo.ToLower(c2))
                        {
                            return false;
                        }
                    }
                    return true;

                default:
                    string sub2 = new string(other, otherStartIndex, length);
                    return string.Compare(text, thisStartIndex, sub2, 0, length, comparisonType) == 0;
            }
        }

        /// <summary>
        /// Compares the specified <see cref="StringBuilder"/> to this string and compares the specified
        /// range of characters to determine if they are the same.
        /// </summary>
        /// <param name="text">This string.</param>
        /// <param name="thisStartIndex">The starting offset in this string.</param>
        /// <param name="other">The <see cref="StringBuilder"/> to compare.</param>
        /// <param name="otherStartIndex">The starting offset in the specified string.</param>
        /// <param name="length">The number of characters to compare.</param>
        /// <param name="comparisonType">One of the enumeration values that specifies the rules for the search.</param>
        /// <returns><c>true</c> if the ranges of characters are equal, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="text"/> or <paramref name="other"/> is <c>null</c></exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="comparisonType"/> is not a <see cref="StringComparison"/> value.</exception>
        public static bool RegionMatches(this string text, int thisStartIndex, StringBuilder other, int otherStartIndex, int length, StringComparison comparisonType) // KEEP OVERLOADS FOR ReadOnlySpan<char>, ICharSequence, char[], StringBuilder, and string IN SYNC
        {
            if (text is null)
                throw new ArgumentNullException(nameof(text));
            if (other is null)
                throw new ArgumentNullException(nameof(other));

            if (other.Length - otherStartIndex < length || otherStartIndex < 0)
                return false;
            if (thisStartIndex < 0 || text.Length - thisStartIndex < length)
                return false;
            if (length <= 0)
                return true;

            if (comparisonType == StringComparison.Ordinal || comparisonType == StringComparison.OrdinalIgnoreCase)
            {
                char[]? arrayToReturnToPool = null;
                try
                {
#if FEATURE_STRINGBUILDER_COPYTO_SPAN // If this method isn't supported, we are buffering to an array pool to get to the stack, anyway.
                    Span<char> otherChars = length > CharStackBufferSize
                        ? (arrayToReturnToPool = ArrayPool<char>.Shared.Rent(length))
                        : stackalloc char[length];
                    other.CopyTo(otherStartIndex, otherChars, length);
#else
                    Span<char> otherChars = arrayToReturnToPool = ArrayPool<char>.Shared.Rent(length);
                    other.CopyTo(otherStartIndex, arrayToReturnToPool, 0, length);
#endif
                    if (comparisonType == StringComparison.Ordinal)
                    {
                        for (int i = 0; i < length; ++i)
                        {
                            if (text[thisStartIndex + i] != otherChars[i])
                            {
                                return false;
                            }
                        }
                        return true;
                    }
                    else
                    {
                        int end = thisStartIndex + length;
                        int otherIndex = 0;
                        char c1, c2;
                        var textInfo = CultureInfo.InvariantCulture.TextInfo;
                        while (thisStartIndex < end)
                        {
                            if ((c1 = text[thisStartIndex++]) != (c2 = otherChars[otherIndex++])
                                && textInfo.ToUpper(c1) != textInfo.ToUpper(c2)
                                // Required for unicode that we test both cases
                                && textInfo.ToLower(c1) != textInfo.ToLower(c2))
                            {
                                return false;
                            }
                        }
                        return true;
                    }

                }
                finally
                {
                    ArrayPool<char>.Shared.ReturnIfNotNull(arrayToReturnToPool);
                }
            }
            else
            {
                // J2N TODO: Optimize other comparison options
                string sub2 = other.ToString(otherStartIndex, length);
                return string.Compare(text, thisStartIndex, sub2, 0, length, comparisonType) == 0;
            }
        }

        /// <summary>
        /// Compares the specified <see cref="string"/> to this string and compares the specified
        /// range of characters to determine if they are the same.
        /// </summary>
        /// <param name="text">This string.</param>
        /// <param name="thisStartIndex">The starting offset in this string.</param>
        /// <param name="other">The <see cref="string"/> to compare.</param>
        /// <param name="otherStartIndex">The starting offset in the specified string.</param>
        /// <param name="length">The number of characters to compare.</param>
        /// <param name="comparisonType">One of the enumeration values that specifies the rules for the search.</param>
        /// <returns><c>true</c> if the ranges of characters are equal, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="text"/> or <paramref name="other"/> is <c>null</c></exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="comparisonType"/> is not a <see cref="StringComparison"/> value.</exception>
        public static bool RegionMatches(this string text, int thisStartIndex, string other, int otherStartIndex, int length, StringComparison comparisonType) // KEEP OVERLOADS FOR ReadOnlySpan<char>, ICharSequence, char[], StringBuilder, and string IN SYNC
        {
            if (text is null)
                throw new ArgumentNullException(nameof(text));
            if (other is null)
                throw new ArgumentNullException(nameof(other));

            if (other.Length - otherStartIndex < length || otherStartIndex < 0)
                return false;
            if (thisStartIndex < 0 || text.Length - thisStartIndex < length)
                return false;
            if (length <= 0)
                return true;

            switch (comparisonType)
            {
                case StringComparison.Ordinal:
                    for (int i = 0; i < length; ++i)
                    {
                        if (text[thisStartIndex + i] != other[otherStartIndex + i])
                        {
                            return false;
                        }
                    }
                    return true;

                case StringComparison.OrdinalIgnoreCase:
                    int end = thisStartIndex + length;
                    char c1, c2;
                    var textInfo = CultureInfo.InvariantCulture.TextInfo;
                    while (thisStartIndex < end)
                    {
                        if ((c1 = text[thisStartIndex++]) != (c2 = other[otherStartIndex++])
                                && textInfo.ToUpper(c1) != textInfo.ToUpper(c2)
                                // Required for unicode that we test both cases
                                && textInfo.ToLower(c1) != textInfo.ToLower(c2))
                        {
                            return false;
                        }
                    }
                    return true;

                default:
                    return string.Compare(text, thisStartIndex, other, otherStartIndex, length, comparisonType) == 0;
            }
        }

        /// <summary>
        /// Compares the specified <see cref="ReadOnlySpan{Char}"/> to this string and compares the specified
        /// range of characters to determine if they are the same.
        /// </summary>
        /// <param name="text">This string.</param>
        /// <param name="thisStartIndex">The starting offset in this string.</param>
        /// <param name="other">The <see cref="ReadOnlySpan{Char}"/> to compare.</param>
        /// <param name="otherStartIndex">The starting offset in the specified string.</param>
        /// <param name="length">The number of characters to compare.</param>
        /// <param name="comparisonType">One of the enumeration values that specifies the rules for the search.</param>
        /// <returns><c>true</c> if the ranges of characters are equal, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="text"/> is <c>null</c></exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="comparisonType"/> is not a <see cref="StringComparison"/> value.</exception>
        public static bool RegionMatches(this string text, int thisStartIndex, ReadOnlySpan<char> other, int otherStartIndex, int length, StringComparison comparisonType) // KEEP OVERLOADS FOR ReadOnlySpan<char>, ICharSequence, char[], StringBuilder, and string IN SYNC
        {
            if (text is null)
                throw new ArgumentNullException(nameof(text));

            if (other.Length - otherStartIndex < length || otherStartIndex < 0)
                return false;
            if (thisStartIndex < 0 || text.Length - thisStartIndex < length)
                return false;
            if (length <= 0)
                return true;

            return text.AsSpan(thisStartIndex, length).Equals(other.Slice(otherStartIndex, length), comparisonType);
        }

        #endregion RegionMatches

        #region ReverseText

        /// <summary>
        /// Reverses the character sequence and returns a new string.
        /// If there are any surrogate pairs included in the
        /// sequence, these are treated as single characters for the
        /// reverse operation. Thus, the order of the high-low surrogates
        /// is never reversed.
        /// <para/>
        /// Let <c>n</c> be the character length of this character sequence
        /// (not the length in <see cref="char"/> values) just prior to
        /// execution of the <see cref="ReverseText(string)"/> method. Then the
        /// character at index <c>k</c> in the new character sequence is
        /// equal to the character at index <c>n-k-1</c> in the old
        /// character sequence.
        /// <para/>
        /// Note that the reverse operation may result in producing
        /// surrogate pairs that were unpaired low-surrogates and
        /// high-surrogates before the operation. For example, reversing
        /// "&#92;uDC00&#92;uD800" produces "&#92;uD800&#92;uDC00" which is
        /// a valid surrogate pair.
        /// <para/>
        /// Usage Note: This is the same operation as
        /// <see cref="StringBuilderExtensions.Reverse(StringBuilder)"/>
        /// (derived from Java's StringBuilder.reverse() method) but is more
        /// efficient because it doesn't allocate a new <see cref="StringBuilder"/>
        /// instance and will use the stack for strings 64 characters and less.
        /// </summary>
        /// <param name="text">This <see cref="string"/>.</param>
        /// <returns>The reversed string with non-reversed surrogate pairs.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="text"/> is
        /// <c>null</c>.</exception>
        /// <seealso cref="StringBuilderExtensions.Reverse(StringBuilder)"/>
        /// <seealso cref="J2N.MemoryExtensions.ReverseText(Span{char})"/>
        public static string ReverseText(this string text)
        {
            if (text is null)
                throw new ArgumentNullException(nameof(text));
            if (text.Length < 2) return text;

#if FEATURE_STRING_CREATE
            return string.Create(text.Length, text, (Span<char> chars, string str) =>
            {
                int length = str.Length;
                int index = 0;
                for (int i = length - 1; i >= 0; i--)
                {
                    char ch = str[i];
                    if (char.IsLowSurrogate(ch) && i > 0)
                    {
                        char ch2 = str[i - 1];
                        if (char.IsHighSurrogate(ch2))
                        {
                            chars[index++] = ch2;
                            chars[index++] = ch;
                            i--;
                            continue;
                        }
                    }
                    chars[index++] = ch;
                }
            });
#else
            int length = text.Length;
            var result = length <= CharStackBufferSize
                ? new ValueStringBuilder(stackalloc char[length])
                : new ValueStringBuilder(length);
            for (int i = length - 1; i >= 0; i--)
            {
                char ch = text[i];
                if (char.IsLowSurrogate(ch) && i > 0)
                {
                    char ch2 = text[i - 1];
                    if (char.IsHighSurrogate(ch2))
                    {
                        result.Append(ch2);
                        result.Append(ch);
                        i--;
                        continue;
                    }
                }
                result.Append(ch);
            }
            return result.ToString();
#endif
        }

        #endregion ReverseText

        #region StartsWith

        /// <summary>
        /// Compares the specified string to this string, starting at the specified
        /// <paramref name="startIndex"/>, to determine if the specified string is a prefix.
        /// </summary>
        /// <param name="text">This string.</param>
        /// <param name="prefix">The string to look for.</param>
        /// <param name="startIndex">The starting offset.</param>
        /// <param name="comparisonType">One of the enumeration values that specifies the rules for the search.</param>
        /// <returns><c>true</c> if the specified string occurs in this string at the specified offset, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="text"/> or <paramref name="prefix"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException"><paramref name="comparisonType"/> is not a <see cref="StringComparison"/> value.</exception>
        public static bool StartsWith(this string text, string prefix, int startIndex, StringComparison comparisonType)
        {
            if (text is null)
                throw new ArgumentNullException(nameof(text));
            if (prefix is null)
                throw new ArgumentNullException(nameof(prefix));

            return RegionMatches(text, startIndex, prefix, 0, prefix.Length, comparisonType);
        }

        #endregion StartsWith

        #region Subsequence

        /// <summary>
        /// Retrieves a sub-sequence from this instance.
        /// The sub-sequence starts at a specified character position and has a specified length.
        /// <para/>
        /// IMPORTANT: This method has .NET semantics, that is, the second parameter is a length,
        /// not an exclusive end index as it would be in Java.
        /// </summary>
        /// <param name="startIndex">
        /// The start index of the sub-sequence. It is inclusive, that
        /// is, the index of the first character that is included in the
        /// sub-sequence.
        /// </param>
        /// <param name="text">This <see cref="T:char[]"/>.</param>
        /// <param name="length">The number of characters to return in the sub-sequence.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startIndex"/> plus <paramref name="length"/> indicates a position not within this instance.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="startIndex"/> or <paramref name="length"/> is less than zero.
        /// </exception>
        public static ICharSequence Subsequence(this string? text, int startIndex, int length)
        {
            // From Apache Harmony String class
            if (text is null || (startIndex == 0 && length == text.Length))
            {
                return text.AsCharSequence();
            }
            if (startIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(startIndex), SR.ArgumentOutOfRange_NeedNonNegNum);
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length), SR.ArgumentOutOfRange_NeedNonNegNum);
            if (startIndex > text.Length - length) // Checks for int overflow
                throw new ArgumentOutOfRangeException(nameof(length), SR.ArgumentOutOfRange_IndexLength);

            return text.Substring(startIndex, length).AsCharSequence();
        }

        #endregion Subsequence
    }
}
