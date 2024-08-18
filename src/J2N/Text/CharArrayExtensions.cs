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
using System.Runtime.InteropServices;
using System.Text;


namespace J2N.Text
{
    using SR = J2N.Resources.Strings;

    /// <summary>
    /// Extensions to <see cref="T:char[]"/> arrays.
    /// </summary>
    public static class CharArrayExtensions
    {
        private const int CharStackBufferSize = 64;

        /// <summary>
        /// Convenience method to wrap a string in a <see cref="CharArrayCharSequence"/>
        /// so a <see cref="T:char[]"/> can be used as <see cref="ICharSequence"/> in .NET.
        /// </summary>
        /// <param name="text">This <see cref="T:char[]"/>.</param>
        public static ICharSequence AsCharSequence(this char[]? text)
        {
            return new CharArrayCharSequence(text);
        }

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
        public static ICharSequence Subsequence(this char[]? text, int startIndex, int length)
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

            char[] result = new char[length];
            Array.Copy(text, startIndex, result, 0, length);

            return new CharArrayCharSequence(result);
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
        public static int CompareToOrdinal(this char[]? str, char[]? value)
        {
            if (object.ReferenceEquals(str, value)) return 0;
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
        public static int CompareToOrdinal(this char[]? str, StringBuilder? value)
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
        public static int CompareToOrdinal(this char[]? str, string? value)
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
        public static int CompareToOrdinal(this char[]? str, ICharSequence? value)
        {
            if (str == null) return (value is null || !value.HasValue) ? 0 : -1;
            if (value == null || !value.HasValue) return 1;
            if (value is CharArrayCharSequence ca && object.ReferenceEquals(str, ca.Value)) return 0;

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
        public static int CompareToOrdinal(this char[]? str, ReadOnlySpan<char> value)
        {
            if (str is null) return (value == default) ? 0 : -1;
            if (value == default) return 1;

            unsafe
            {
                fixed (char* valuePtr = &MemoryMarshal.GetReference(value))
                {
                    return CompareToOrdinalCore(str, valuePtr, value.Length);
                }
            }
        }

        private unsafe static int CompareToOrdinalCore(char[] str, char* value, int valueLength)
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
    }
}
