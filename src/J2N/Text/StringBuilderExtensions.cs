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
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;


namespace J2N.Text
{
    /// <summary>
    /// Extensions to the <see cref="StringBuilder"/> class.
    /// </summary>
    public static class StringBuilderExtensions
    {
        private const int CharStackBufferSize = 64;
        private const int CharPoolBufferSize = 512;

        #region Append

        /// <summary>
        /// Appends the given <see cref="ICharSequence"/> to this <see cref="StringBuilder"/>.
        /// <para/>
        /// <strong>Usage Note:</strong> The compiler will not automatically call this extension method because
        /// <see cref="StringBuilder.Append(object?)"/> will take precedence over the call. This can be
        /// overcome only by calling this static method explicitly:
        /// <code>
        /// J2N.Text.StringBuilderExtensions.Append(stringBuilder, charSequenceToInsert);
        /// </code>
        /// or by explicitly using the <paramref name="charSequence"/> label:
        /// <code>
        /// stringBuilder.Append(charSequence: charSequenceToInsert);
        /// </code>
        /// </summary>
        /// <param name="text">This <see cref="StringBuilder"/>.</param>
        /// <param name="charSequence">The <see cref="ICharSequence"/> to append.</param>
        /// <returns>This <see cref="StringBuilder"/>, for chaining.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="text"/> is <c>null</c>.</exception>
        public static StringBuilder Append(this StringBuilder text, ICharSequence? charSequence)
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            // For null values, this is a no-op
            if (charSequence is null || !charSequence.HasValue || charSequence.Length == 0)
                return text;

            if (charSequence is StringCharSequence str)
                return text.Append(str.Value);
            if (charSequence is CharArrayCharSequence charArray)
                return text.Append(charArray.Value);
            if (charSequence is StringBuilderCharSequence sb)
#if FEATURE_STRINGBUILDER_APPEND_STRINGBUILDER
                return text.Append(sb.Value);
#else
                return Append(text, sb.Value);
#endif
            if (charSequence is StringBuffer stringBuffer)
#if FEATURE_STRINGBUILDER_APPEND_STRINGBUILDER
                return text.Append(stringBuffer.builder);
#else
                return Append(text, stringBuffer.builder);
#endif

            int length = charSequence.Length;
            if (length <= CharStackBufferSize)
            {
                for (int i = 0; i < length; i++)
                {
                    text.Append(charSequence[i]);
                }
                return text;
            }

            return AppendSlow(text, charSequence);

            static StringBuilder AppendSlow(StringBuilder text, ICharSequence charSequence)
            {
                int start = 0;
                int remainingCount = charSequence.Length;

                char[] buffer = ArrayPool<char>.Shared.Rent(Math.Min(remainingCount, CharPoolBufferSize));
                try
                {
                    while (remainingCount > 0)
                    {
                        // Determine the chunk size for the current iteration
                        int chunkLength = Math.Min(remainingCount, CharPoolBufferSize);

                        // Copy the chunk to the buffer
                        int bufferIndex = 0;
                        int end = start + chunkLength;
                        for (int i = start; i < end; i++)
                        {
                            buffer[bufferIndex++] = charSequence[i];
                        }

                        text.Append(buffer, 0, chunkLength);

                        start += chunkLength;
                        remainingCount -= chunkLength;
                    }
                }
                finally
                {
                    ArrayPool<char>.Shared.Return(buffer);
                }
                return text;
            }
        }

        /// <summary>
        /// Appends the specified range of characters of the given <see cref="ICharSequence"/> to this <see cref="StringBuilder"/>.
        /// <para/>
        /// IMPORTANT: This method has .NET semantics, that is, the third parameter is a length,
        /// not an exclusive end index as it would be in Java. To translate from Java to .NET,
        /// callers must account for this by subtracting (end - start) for the <paramref name="charCount"/>.
        /// </summary>
        /// <param name="text">This <see cref="StringBuilder"/>.</param>
        /// <param name="charSequence">The <see cref="ICharSequence"/> to append.</param>
        /// <param name="startIndex">The starting position of the substring within <paramref name="charSequence"/>.</param>
        /// <param name="charCount">The number of characters in <paramref name="charSequence"/> to append.</param>
        /// <returns>This <see cref="StringBuilder"/>, for chaining.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="charSequence"/> is <c>null</c>, and
        /// <paramref name="startIndex"/> and <paramref name="charCount"/> are not zero.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startIndex"/> plus <paramref name="charCount"/> indicates a position not within <paramref name="charSequence"/>.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="startIndex"/> or <paramref name="charCount"/> is less than zero.
        /// </exception>
        public static StringBuilder Append(this StringBuilder text, ICharSequence? charSequence, int startIndex, int charCount)
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);
            if (charSequence is null && (startIndex != 0 || charCount != 0))
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.charSequence); // J2N: Unlike Java, we are throwing an exception (to match .NET Core 3) rather than writing "null" to the StringBuilder
            if (startIndex < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(startIndex, ExceptionArgument.startIndex);
            if (charCount < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(charCount, ExceptionArgument.charCount);

            if (charSequence == null || !charSequence.HasValue)
                return text;

            if (startIndex > charSequence.Length - charCount) // Checks for int overflow
                throw new ArgumentOutOfRangeException(nameof(charCount), SR.ArgumentOutOfRange_IndexCount);

            if (charSequence is CharArrayCharSequence charArrayCharSequence)
                return text.Append(charArrayCharSequence.Value, startIndex, charCount);
            if (charSequence is StringCharSequence stringCharSequence)
                return text.Append(stringCharSequence.Value, startIndex, charCount);
            if (charSequence is StringBuilderCharSequence stringBuilderCharSequence)
#if FEATURE_STRINGBUILDER_APPEND_STRINGBUILDER
                return text.Append(stringBuilderCharSequence.Value, startIndex, charCount);
#else
                return Append(text, stringBuilderCharSequence.Value, startIndex, charCount);
#endif
            if (charSequence is StringBuffer stringBuffer)
#if FEATURE_STRINGBUILDER_APPEND_STRINGBUILDER
                return text.Append(stringBuffer.builder, startIndex, charCount);
#else
                return Append(text, stringBuffer.builder, startIndex, charCount);
#endif

            int length = charCount;
            if (length <= CharStackBufferSize)
            {
                int end = startIndex + charCount;
                for (int i = startIndex; i < end; i++)
                {
                    text.Append(charSequence[i]);
                }
                return text;
            }

            return AppendSlow(text, charSequence, startIndex, charCount);

            static StringBuilder AppendSlow(StringBuilder text, ICharSequence charSequence, int startIndex, int charCount)
            {
                int start = startIndex;
                int remainingCount = charCount;

                char[] buffer = ArrayPool<char>.Shared.Rent(Math.Min(remainingCount, CharPoolBufferSize));
                try
                {
                    while (remainingCount > 0)
                    {
                        // Determine the chunk size for the current iteration
                        int chunkLength = Math.Min(remainingCount, CharPoolBufferSize);

                        // Copy the chunk to the buffer
                        int bufferIndex = 0;
                        int end = start + chunkLength;
                        for (int i = start; i < end; i++)
                        {
                            buffer[bufferIndex++] = charSequence[i];
                        }

                        text.Append(buffer, 0, chunkLength);

                        start += chunkLength;
                        remainingCount -= chunkLength;
                    }
                }
                finally
                {
                    ArrayPool<char>.Shared.Return(buffer);
                }
                return text;
            }
        }

        /// <summary>
        /// Appends the given <see cref="StringBuilder"/> to this <see cref="StringBuilder"/>.
        /// <para/>
        /// The characters of the <paramref name="charSequence"/> argument are appended,
        /// in order, to this sequence, increasing the
        /// length of this sequence by the length of <paramref name="charSequence"/>.
        /// <para/>
        /// <strong>Usage Note:</strong> The compiler will not automatically call this extension method because
        /// <see cref="StringBuilder.Append(object?)"/> will take precedence over the call. This can be
        /// overcome only by calling this static method explicitly:
        /// <code>
        /// J2N.Text.StringBuilderExtensions.Append(stringBuilder, stringBuilderToInsert);
        /// </code>
        /// or by explicitly using the <paramref name="charSequence"/> label:
        /// <code>
        /// stringBuilder.Append(charSequence: stringBuilderToInsert);
        /// </code>
        /// <para/>
        /// Also, unlike in Java, a <c>null</c> <paramref name="charSequence"/> won't append the string
        /// <c>"null"</c> to the <see cref="StringBuilder"/>. Instead, it is a no-op.
        /// </summary>
        /// <param name="text">This <see cref="StringBuilder"/>.</param>
        /// <param name="charSequence">The <see cref="StringBuilder"/> to append.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="text"/> is <c>null</c>.</exception>
        public static StringBuilder Append(this StringBuilder text, StringBuilder? charSequence)
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);
            if (charSequence is null || charSequence.Length == 0)
                return text;

#if FEATURE_STRINGBUILDER_APPEND_STRINGBUILDER
            return text.Append(charSequence);
#else
            int start = 0;
            int remainingCount = charSequence.Length;

            char[] buffer = ArrayPool<char>.Shared.Rent(Math.Min(remainingCount, CharPoolBufferSize));
            try
            {
                while (remainingCount > 0)
                {
                    // Determine the chunk size for the current iteration
                    int chunkLength = Math.Min(remainingCount, CharPoolBufferSize);

                    // Copy the chunk to the buffer
                    charSequence.CopyTo(start, buffer, 0, chunkLength);

                    text.Append(buffer, 0, chunkLength);

                    start += chunkLength;
                    remainingCount -= chunkLength;
                }
            }
            finally
            {
                ArrayPool<char>.Shared.Return(buffer);
            }
            return text;
#endif
        }

        /// <summary>
        /// Appends the given <see cref="StringBuilder"/> to this <see cref="StringBuilder"/>.
        /// </summary>
        /// <summary>
        /// Appends a substring of the specified <paramref name="charSequence"/> to this <see cref="StringBuilder"/>.
        /// <para/>
        /// Characters of the argument <paramref name="charSequence"/>, starting at
        /// <paramref name="startIndex"/>, are appended, in order, to the contents of
        /// this sequence up to the specified <paramref name="charCount"/>. The length of this
        /// sequence is increased by the value of <paramref name="charCount"/>.
        /// <para/>
        /// Usage Note: Unlike in Java, a <c>null</c> <paramref name="charSequence"/> won't append the string
        /// <c>"null"</c> to the <see cref="StringBuilder"/>. Instead, it will throw an <see cref="ArgumentNullException"/>.
        /// </summary>
        /// <param name="text">This <see cref="StringBuilder"/>.</param>
        /// <param name="charSequence">The <see cref="StringBuilder"/> to append.</param>
        /// <param name="startIndex">The starting index of the subsequence to be appended.</param>
        /// <param name="charCount">The number of characters in <paramref name="charSequence"/> to append.</param>
        /// <returns>This <see cref="StringBuilder"/>, for chaining.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="text"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startIndex"/> plus <paramref name="charCount"/> indicates a position not within <paramref name="charSequence"/>.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="startIndex"/> or <paramref name="charCount"/> is less than zero.
        /// </exception>
        public static StringBuilder Append(this StringBuilder text, StringBuilder? charSequence, int startIndex, int charCount)
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);
#if FEATURE_STRINGBUILDER_APPEND_STRINGBUILDER
            return text.Append(charSequence, startIndex, charCount);
#else
            if (charSequence is null && (startIndex != 0 || charCount != 0))
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.charSequence); // J2N: Unlike Java, we are throwing an exception (to match .NET Core 3) rather than writing "null" to the StringBuilder
            if (startIndex < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(startIndex, ExceptionArgument.startIndex);
            if (charCount < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(charCount, ExceptionArgument.charCount);
            if (charSequence is null)
                return text;
            if (startIndex > charSequence.Length - charCount) // Checks for int overflow
                throw new ArgumentOutOfRangeException(nameof(charCount), SR.ArgumentOutOfRange_IndexCount);

            int start = startIndex;
            int remainingCount = charCount;

            char[] buffer = ArrayPool<char>.Shared.Rent(Math.Min(remainingCount, CharPoolBufferSize));
            try
            {
                while (remainingCount > 0)
                {
                    // Determine the chunk size for the current iteration
                    int chunkLength = Math.Min(remainingCount, CharPoolBufferSize);

                    // Copy the chunk to the buffer
                    charSequence.CopyTo(start, buffer, 0, chunkLength);

                    text.Append(buffer, 0, chunkLength);

                    start += chunkLength;
                    remainingCount -= chunkLength;
                }
            }
            finally
            {
                ArrayPool<char>.Shared.Return(buffer);
            }
            return text;
#endif
        }

        /// <summary>
        /// Appends the given <see cref="ReadOnlySpan{Char}"/> to this <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="text">This <see cref="StringBuilder"/>.</param>
        /// <param name="charSequence">The <see cref="ReadOnlySpan{Char}"/> to append.</param>
        /// <returns>This <see cref="StringBuilder"/>, for chaining.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="text"/> is <c>null</c>.</exception>
        // J2N: Added to cover the missing .NET API on older .NET target frameworks.
        public static StringBuilder Append(this StringBuilder text, ReadOnlySpan<char> charSequence)
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);
            if (charSequence.Length == 0)
                return text;

#if FEATURE_STRINGBUILDER_APPEND_CHARPTR
            unsafe
            {
                fixed (char* seq = &MemoryMarshal.GetReference(charSequence))
                {
                    text.Append(seq, charSequence.Length);
                }
            }
#else
            int startIndex = 0;
            int remainingCount = charSequence.Length;

            char[] buffer = ArrayPool<char>.Shared.Rent(Math.Min(remainingCount, CharPoolBufferSize));
            try
            {
                while (remainingCount > 0)
                {
                    // Determine the chunk size for the current iteration
                    int chunkLength = Math.Min(remainingCount, CharPoolBufferSize);

                    // Copy the chunk to the buffer
                    charSequence.Slice(startIndex, chunkLength).CopyTo(buffer);

                    text.Append(buffer, 0, chunkLength);

                    startIndex += chunkLength;
                    remainingCount -= chunkLength;
                }
            }
            finally
            {
                ArrayPool<char>.Shared.Return(buffer);
            }
#endif
            return text;
        }

        #endregion Append

        #region AppendCodePoint

        /// <summary>
        /// Appends the string representation of the <paramref name="codePoint"/>
        /// argument to this sequence.
        /// <para>
        /// The argument is appended to the contents of this sequence.
        /// The length of this sequence increases by <see cref="Character.CharCount(int)"/>.
        /// </para>
        /// <para>
        /// The overall effect is exactly as if the argument were
        /// converted to a <see cref="char"/> array by the method
        /// <see cref="Character.ToChars(int)"/> and the character in that array
        /// were then <see cref="StringBuilder.Append(char[])">appended</see> to this 
        /// <see cref="StringBuilder"/>.
        /// </para>
        /// </summary>
        /// <param name="text">This <see cref="StringBuilder"/>.</param>
        /// <param name="codePoint">A Unicode code point</param>
        /// <returns>This <see cref="StringBuilder"/>, for chaining.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="text"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException"><paramref name="codePoint"/> is not a valid Unicode code point.</exception>
        public static StringBuilder AppendCodePoint(this StringBuilder text, int codePoint)
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);
            if (!Character.IsValidCodePoint(codePoint))
                ThrowHelper.ThrowArgumentException_Argument_InvalidCodePoint(codePoint);

            if (codePoint < Character.MinSupplementaryCodePoint)
            {
                text.Append((char)codePoint);
                return text;
            }

            return AppendCodePointSlow(text, codePoint);

            static StringBuilder AppendCodePointSlow(StringBuilder text, int codePoint)
            {
                Character.ToChars(codePoint, out char high, out char low);
#if FEATURE_STRINGBUILDER_APPEND_CHARPTR
                Span<char> ch = stackalloc char[2];
                ch[0] = high;
                ch[1] = low;
                text.Append(ch);
#else
                text.Append(high);
                text.Append(low);
#endif
                return text;
            }
        }

        #endregion AppendCodePoint

        #region AsCharSequence

        /// <summary>
        /// Convenience method to wrap a string in a <see cref="StringBuilderCharSequence"/>
        /// so a <see cref="StringBuilder"/> can be used as <see cref="ICharSequence"/> in .NET.
        /// </summary>
        public static ICharSequence AsCharSequence(this StringBuilder? text)
        {
            return new StringBuilderCharSequence(text);
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
        /// <param name="text">This string.</param>
        /// <param name="value">The string to compare with.</param>
        /// <returns>
        /// An integer that indicates the lexical relationship between the two comparands.
        /// Less than zero indicates the comparison value is greater than the current string.
        /// Zero indicates the strings are equal.
        /// Greater than zero indicates the comparison value is less than the current string.
        /// </returns>
        public static int CompareToOrdinal(this StringBuilder? text, ICharSequence? value) // KEEP OVERLOADS FOR ReadOnlySpan<char>, ICharSequence, char[], StringBuilder, and string IN SYNC
        {
            if (text is null) return (value is null || !value.HasValue) ? 0 : -1;
            if (value is null || !value.HasValue) return 1;

            if (value is StringBuilderCharSequence sb)
                return CompareToOrdinal(text, sb.Value);
            if (value is StringBuffer stringBuffer)
                return CompareToOrdinal(text, stringBuffer.builder);

            int length = Math.Min(text.Length, value.Length);
            char[]? arrayToReturnToPool = null;
            try
            {
#if FEATURE_STRINGBUILDER_COPYTO_SPAN // If this method isn't supported, we are buffering to an array pool to get to the stack, anyway.
                Span<char> textChars = length > CharStackBufferSize
                    ? (arrayToReturnToPool = ArrayPool<char>.Shared.Rent(length))
                    : stackalloc char[length];
                text.CopyTo(0, textChars, length);
#else
                Span<char> textChars = arrayToReturnToPool = ArrayPool<char>.Shared.Rent(length);
                text.CopyTo(0, arrayToReturnToPool, 0, length);
#endif
                int result;
                for (int i = 0; i < length; i++)
                {
                    if ((result = textChars[i] - value[i]) != 0)
                        return result;
                }

                // At this point, we have compared all the characters in at least one string.
                // The longer string will be larger.
                return text.Length - value.Length;
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
        /// <param name="text">This string.</param>
        /// <param name="value">The string to compare with.</param>
        /// <returns>
        /// An integer that indicates the lexical relationship between the two comparands.
        /// Less than zero indicates the comparison value is greater than the current string.
        /// Zero indicates the strings are equal.
        /// Greater than zero indicates the comparison value is less than the current string.
        /// </returns>
        public static int CompareToOrdinal(this StringBuilder? text, char[]? value) // KEEP OVERLOADS FOR ReadOnlySpan<char>, ICharSequence, char[], StringBuilder, and string IN SYNC
        {
            if (text is null) return (value is null) ? 0 : -1;
            if (value is null) return 1;

            unsafe
            {
                fixed (char* valuePtr = value)
                {
                    return CompareToOrdinalCore(text, valuePtr, value.Length);
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
        /// <param name="text">This string.</param>
        /// <param name="value">The string to compare with.</param>
        /// <returns>
        /// An integer that indicates the lexical relationship between the two comparands.
        /// Less than zero indicates the comparison value is greater than the current string.
        /// Zero indicates the strings are equal.
        /// Greater than zero indicates the comparison value is less than the current string.
        /// </returns>
        public static int CompareToOrdinal(this StringBuilder? text, StringBuilder? value) // KEEP OVERLOADS FOR ReadOnlySpan<char>, ICharSequence, char[], StringBuilder, and string IN SYNC
        {
            if (object.ReferenceEquals(text, value)) return 0;
            if (text is null) return (value is null) ? 0 : -1;
            if (value is null) return 1;

            int length = Math.Min(text.Length, value.Length);
            char[]? textArrayToReturnToPool = null;
            char[]? valueArrayToReturnToPool = null;
            try
            {
#if FEATURE_STRINGBUILDER_COPYTO_SPAN // If this method isn't supported, we are buffering to an array pool to get to the stack, anyway.
                Span<char> textChars = length > CharStackBufferSize
                    ? (textArrayToReturnToPool = ArrayPool<char>.Shared.Rent(length))
                    : stackalloc char[length];
                Span<char> valueChars = length > CharStackBufferSize
                    ? (valueArrayToReturnToPool = ArrayPool<char>.Shared.Rent(length))
                    : stackalloc char[length];
                text.CopyTo(0, textChars, length);
                value.CopyTo(0, valueChars, length);
#else
                Span<char> textChars = textArrayToReturnToPool = ArrayPool<char>.Shared.Rent(length);
                Span<char> valueChars = valueArrayToReturnToPool = ArrayPool<char>.Shared.Rent(length);
                text.CopyTo(0, textArrayToReturnToPool, 0, length);
                value.CopyTo(0, valueArrayToReturnToPool, 0, length);
#endif
                int result;
                for (int i = 0; i < length; i++)
                {
                    if ((result = textChars[i] - valueChars[i]) != 0)
                        return result;
                }

                // At this point, we have compared all the characters in at least one string.
                // The longer string will be larger.
                return text.Length - value.Length;
            }
            finally
            {
                ArrayPool<char>.Shared.ReturnIfNotNull(textArrayToReturnToPool);
                ArrayPool<char>.Shared.ReturnIfNotNull(valueArrayToReturnToPool);
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
        /// <param name="text">This string.</param>
        /// <param name="value">The string to compare with.</param>
        /// <returns>
        /// An integer that indicates the lexical relationship between the two comparands.
        /// Less than zero indicates the comparison value is greater than the current string.
        /// Zero indicates the strings are equal.
        /// Greater than zero indicates the comparison value is less than the current string.
        /// </returns>
        public static int CompareToOrdinal(this StringBuilder? text, string? value) // KEEP OVERLOADS FOR ReadOnlySpan<char>, ICharSequence, char[], StringBuilder, and string IN SYNC
        {
            if (text is null) return (value is null) ? 0 : -1;
            if (value is null) return 1;

            unsafe
            {
                fixed (char* valuePtr = value)
                {
                    return CompareToOrdinalCore(text, valuePtr, value.Length);
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
        /// <param name="text">This string.</param>
        /// <param name="value">The string to compare with.</param>
        /// <returns>
        /// An integer that indicates the lexical relationship between the two comparands.
        /// Less than zero indicates the comparison value is greater than the current string.
        /// Zero indicates the strings are equal.
        /// Greater than zero indicates the comparison value is less than the current string.
        /// </returns>
        public static int CompareToOrdinal(this StringBuilder? text, ReadOnlySpan<char> value) // KEEP OVERLOADS FOR ReadOnlySpan<char>, ICharSequence, char[], StringBuilder, and string IN SYNC
        {
#pragma warning disable CA2265 // Do not compare Span<T> to null or default
            if (text is null) return (value == default) ? 0 : -1;
            if (value == default) return 1;
#pragma warning restore CA2265 // Do not compare Span<T> to null or default

            unsafe
            {
                fixed (char* valuePtr = &MemoryMarshal.GetReference(value))
                {
                    return CompareToOrdinalCore(text, valuePtr, value.Length);
                }
            }
        }

        private unsafe static int CompareToOrdinalCore(StringBuilder text, char* value, int valueLength)
        {
            int length = Math.Min(text.Length, valueLength);
            char[]? arrayToReturnToPool = null;
            try
            {
#if FEATURE_STRINGBUILDER_COPYTO_SPAN // If this method isn't supported, we are buffering to an array pool to get to the stack, anyway.
                Span<char> textChars = length > CharStackBufferSize
                    ? (arrayToReturnToPool = ArrayPool<char>.Shared.Rent(length))
                    : stackalloc char[length];
                text.CopyTo(0, textChars, length);
#else
                Span<char> textChars = arrayToReturnToPool = ArrayPool<char>.Shared.Rent(length);
                text.CopyTo(0, arrayToReturnToPool, 0, length);
#endif
                int result;
                for (int i = 0; i < length; i++)
                {
                    if ((result = textChars[i] - value[i]) != 0)
                        return result;
                }

                // At this point, we have compared all the characters in at least one string.
                // The longer string will be larger.
                return text.Length - valueLength;
            }
            finally
            {
                ArrayPool<char>.Shared.ReturnIfNotNull(arrayToReturnToPool);
            }
        }

        #endregion CompareToOrdinal

        #region CopyTo

        /// <summary>
        /// Copies the characters from a specified segment of this instance to a destination <see cref="char"/> span.
        /// </summary>
        /// <param name="text">This <see cref="StringBuilder"/>.</param>
        /// <param name="sourceIndex">The starting position in this instance where characters will be copied from. The index is zero-based.</param>
        /// <param name="destination">The writable span where characters will be copied.</param>
        /// <param name="count">The number of characters to be copied.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="sourceIndex"/> or <paramref name="count"/> is less than zero.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="sourceIndex"/> is greater than the length of this instance.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="sourceIndex"/> + <paramref name="count"/> is greater than the length of this instance.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="count"/> is greater than the length of <paramref name="destination"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException"><paramref name="text"/> is <c>null</c>.</exception>
        // J2N: Added to cover the missing .NET API on older .NET target frameworks.
        public static void CopyTo(this StringBuilder text, int sourceIndex, Span<char> destination, int count)
        {
            if (count < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(count, ExceptionArgument.count);
            }

            if ((uint)sourceIndex > (uint)text.Length)
            {
                ThrowHelper.ThrowArgumentOutOfRange_ArgumentOutOfRange_IndexString(sourceIndex, ExceptionArgument.sourceIndex);
            }

            if (sourceIndex > text.Length - count)
            {
                throw new ArgumentException(SR.Arg_LongerThanSrcString);
            }

            if (count > destination.Length)
            {
                throw new ArgumentException(SR.Argument_DestinationTooShort);
            }

            int startIndex = sourceIndex;

            // J2N NOTE: We don't use GetChunks() because it wasn't added until after StringBuilder got its own CopyTo(int, Span<char> int) overload,
            // so this will never be called when GetChunks() is supported unless called as a static method.

            int destinationIndex = 0;
            int remainingCount = count;

            char[] buffer = ArrayPool<char>.Shared.Rent(Math.Min(remainingCount, CharPoolBufferSize));
            try
            {
                while (remainingCount > 0)
                {
                    // Determine the chunk size for the current iteration
                    int chunkLength = Math.Min(remainingCount, CharPoolBufferSize);

                    // Copy the chunk to the buffer
                    text.CopyTo(startIndex, buffer, 0, chunkLength);

                    // Copy from the buffer to the destination Span<char>
                    buffer.AsSpan(0, chunkLength).CopyTo(destination.Slice(destinationIndex, chunkLength));

                    startIndex += chunkLength;
                    destinationIndex += chunkLength;
                    remainingCount -= chunkLength;
                }
            }
            finally
            {
                ArrayPool<char>.Shared.Return(buffer);
            }
        }

        #endregion

        #region Delete

        /// <summary>
        /// Deletes a sequence of characters specified by <paramref name="startIndex"/> and <paramref name="count"/>.
        /// Shifts any remaining characters to the left.
        /// <para/>
        /// IMPORTANT: This method has .NET semantics. That is, the <paramref name="count"/> parameter is a count rather than
        /// an exclusive end index. To translate from Java, use <c>end - start</c> for <paramref name="count"/>.
        /// <para/>
        /// This method differs from <see cref="StringBuilder.Remove(int, int)"/> in that it will automatically
        /// adjust the <paramref name="count"/> if <c><paramref name="startIndex"/> + <paramref name="count"/> > <see cref="StringBuilder.Length"/></c>
        /// to <c><see cref="StringBuilder.Length"/> - <paramref name="startIndex"/>.</c>, provided it is not bounded by <see cref="StringBuilder.MaxCapacity"/>.
        /// </summary>
        /// <param name="text">This <see cref="StringBuilder"/>.</param>
        /// <param name="startIndex">The start index in <paramref name="text"/>.</param>
        /// <param name="count">The number of characters to delete in <paramref name="text"/>.</param>
        /// <returns>This <see cref="StringBuilder"/>, for chaining.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startIndex"/> or <paramref name="count"/> is less than zero.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="startIndex"/> is greater than <see cref="StringBuilder.Length"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">If <paramref name="text"/> is <c>null</c>.</exception>
        public static StringBuilder Delete(this StringBuilder text, int startIndex, int count)
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);
            if ((uint)startIndex > (uint)text.Length)
                ThrowHelper.ThrowArgumentOutOfRange_ArgumentOutOfRange_IndexString(startIndex, ExceptionArgument.startIndex);
            if (count < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(count, ExceptionArgument.count);

            if (startIndex + count > text.Length)
                count = text.Length - startIndex;
            if (count > 0)
                text.Remove(startIndex, count);

            return text;
        }

        #endregion Delete

        #region IndexOf

        // J2N: Removed this overload to prevent it from accidentally selecting culture sensitivity when it is not desired
        ///// <summary>
        ///// Searches for the first index of the specified character. The search for
        ///// the character starts at the beginning and moves towards the end.
        ///// <para/>
        ///// Usage Note: This method has .NET semantics - it uses the current culture to compare the string.
        ///// To match Java, use the <see cref="IndexOf(StringBuilder, string, StringComparison)"/> overload
        ///// with <see cref="StringComparison.Ordinal"/>.
        ///// </summary>
        ///// <param name="text">This <see cref="StringBuilder"/>.</param>
        ///// <param name="value">The string to find.</param>
        ///// <returns>The index of the specified character, or <c>-1</c> if the character isn't found.</returns>
        ///// <exception cref="ArgumentNullException">If <paramref name="text"/> or <paramref name="value"/> is <c>null</c>.</exception>
        //public static int IndexOf(this StringBuilder text, string value)
        //{
        //    return IndexOf(text, value, 0, StringComparison.CurrentCulture);
        //}

        // J2N: Removed this overload to prevent it from accidentally selecting culture sensitivity when it is not desired
        ///// <summary>
        ///// Searches for the index of the specified character. The search for the
        ///// character starts at the specified offset and moves towards the end.
        ///// <para/>
        ///// Usage Note: This method has .NET semantics - it uses the current culture to compare the string.
        ///// To match Java, use the <see cref="IndexOf(StringBuilder, string, int, StringComparison)"/> overload
        ///// with <see cref="StringComparison.Ordinal"/>.
        ///// </summary>
        ///// <param name="text">This <see cref="StringBuilder"/>.</param>
        ///// <param name="value">The string to find.</param>
        ///// <param name="startIndex">The starting offset.</param>
        ///// <returns>The index of the specified character, or <c>-1</c> if the character isn't found.</returns>
        ///// <exception cref="ArgumentNullException">If <paramref name="text"/> or <paramref name="value"/> is <c>null</c>.</exception>
        //public static int IndexOf(this StringBuilder text, string value, int startIndex)
        //{
        //    return IndexOf(text, value, startIndex, StringComparison.CurrentCulture);
        //}

        /// <summary>
        /// Searches for the index of the specified character. The search for the
        /// character starts at the specified offset and moves towards the end.
        /// </summary>
        /// <param name="text">This <see cref="StringBuilder"/>.</param>
        /// <param name="value">The string to find.</param>
        /// <param name="comparisonType">One of the enumeration values that specifies the rules for the search.</param>
        /// <returns>The index of the specified character, or <c>-1</c> if the character isn't found.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="text"/> or <paramref name="value"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException"><paramref name="comparisonType"/> is not a <see cref="StringComparison"/> value.</exception>
        public static int IndexOf(this StringBuilder text, string value, StringComparison comparisonType)
        {
            return IndexOf(text, value, 0, comparisonType);
        }

        /// <summary>
        /// Searches for the index of the specified character. The search for the
        /// character starts at the specified offset and moves towards the end.
        /// </summary>
        /// <param name="text">This <see cref="StringBuilder"/>.</param>
        /// <param name="value">The string to find.</param>
        /// <param name="startIndex">The starting offset.</param>
        /// <param name="comparisonType">One of the enumeration values that specifies the rules for the search.</param>
        /// <returns>The index of the specified character, or <c>-1</c> if the character isn't found.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="text"/> or <paramref name="value"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="startIndex"/> is less than 0 (zero) or greater than the length of this <see cref="StringBuilder"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="comparisonType"/> is not a <see cref="StringComparison"/> value.</exception>
        public static int IndexOf(this StringBuilder text, string value, int startIndex, StringComparison comparisonType)
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);
            if (value is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.value);
            if ((uint)startIndex > (uint)text.Length)
                ThrowHelper.ThrowArgumentOutOfRange_ArgumentOutOfRange_IndexString(startIndex, ExceptionArgument.startIndex);

            if (value.Length == 0)
                return 0;

            switch (comparisonType)
            {
                case StringComparison.Ordinal:
                    return IndexOfOrdinal(text, value, startIndex);
                case StringComparison.OrdinalIgnoreCase:
                    return IndexOfOrdinalIgnoreCase(text, value, startIndex);
                default:
                    // J2N TODO: Optimize other StringComparison options better
                    int length = text.Length;
                    int start = Math.Min(startIndex, length - 1);
                    int result = text.ToString(start, length - start).IndexOf(value, comparisonType);
                    return (result == -1) ? -1 : result + start;
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int IndexOfOrdinal(StringBuilder text, string value, int startIndex)
        {
            int length = value.Length;
            if (length == 0)
                return 0;

            int textLength = text.Length;
            char[]? arrayToReturnToPool = null;
            try
            {
#if FEATURE_STRINGBUILDER_COPYTO_SPAN // If this method isn't supported, we are buffering to an array pool to get to the stack, anyway.
                Span<char> textChars = textLength > CharStackBufferSize
                    ? (arrayToReturnToPool = ArrayPool<char>.Shared.Rent(textLength))
                    : stackalloc char[textLength];
                text.CopyTo(0, textChars, textLength);
#else
                Span<char> textChars = arrayToReturnToPool = ArrayPool<char>.Shared.Rent(textLength);
                text.CopyTo(0, arrayToReturnToPool, 0, textLength);
#endif
                int maxSearchLength = (textLength - length) + 1;
                char firstChar = value[0];
                int index;
                for (int i = startIndex; i < maxSearchLength; ++i)
                {
                    if (textChars[i] == firstChar)
                    {
                        index = 1;
                        while ((index < length) && (textChars[i + index] == value[index]))
                            ++index;

                        if (index == length)
                            return i;
                    }
                }
                return -1;
            }
            finally
            {
                ArrayPool<char>.Shared.ReturnIfNotNull(arrayToReturnToPool);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int IndexOfOrdinalIgnoreCase(StringBuilder text, string value, int startIndex)
        {
            int length = value.Length;
            if (length == 0)
                return 0;

            int textLength = text.Length;
            char[]? arrayToReturnToPool = null;
            try
            {
#if FEATURE_STRINGBUILDER_COPYTO_SPAN // If this method isn't supported, we are buffering to an array pool to get to the stack, anyway.
                Span<char> textChars = textLength > CharStackBufferSize
                    ? (arrayToReturnToPool = ArrayPool<char>.Shared.Rent(textLength))
                    : stackalloc char[textLength];
                text.CopyTo(0, textChars, textLength);
#else
                Span<char> textChars = arrayToReturnToPool = ArrayPool<char>.Shared.Rent(textLength);
                text.CopyTo(0, arrayToReturnToPool, 0, textLength);
#endif
                int maxSearchLength = (textLength - length) + 1;
                char firstChar = value[0], c1, c2;
                var textInfo = CultureInfo.InvariantCulture.TextInfo;
                int index;
                for (int i = startIndex; i < maxSearchLength; ++i)
                {
                    if (textChars[i] == firstChar)
                    {
                        index = 1;
                        while ((index < length) &&
                            ((c1 = textChars[i + index]) == (c2 = value[index]) ||
                            textInfo.ToUpper(c1) == textInfo.ToUpper(c2) ||
                            // Required for unicode that we test both cases
                            textInfo.ToLower(c1) == textInfo.ToLower(c2)))
                        {
                            ++index;
                        }

                        if (index == length)
                            return i;
                    }
                }
                return -1;
            }
            finally
            {
                ArrayPool<char>.Shared.ReturnIfNotNull(arrayToReturnToPool);
            }
        }

        #endregion IndexOf

        #region Insert

        /// <summary>
        /// Inserts the string representation of a specified sequence of Unicode characters into this instance at the specified character position.
        /// <para/>
        /// <strong>Usage Note:</strong> The compiler will not automatically call this extension method because
        /// <see cref="StringBuilder.Insert(int, object?)"/> will take precedence over the call. This can be
        /// overcome only by calling this static method explicitly:
        /// <code>
        /// J2N.Text.StringBuilderExtensions.Insert(stringBuilder, 2, charSequenceToInsert);
        /// </code>
        /// or by explicitly using the <paramref name="charSequence"/> label:
        /// <code>
        /// stringBuilder.Insert(2, charSequence: charSequenceToInsert);
        /// </code>
        /// </summary>
        /// <param name="text">This <see cref="StringBuilder"/>.</param>
        /// <param name="index">The position in this instance where insertion begins.</param>
        /// <param name="charSequence">The character sequence to insert.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="text"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than zero or greater than the length of this instance.
        /// <para/>
        /// -or-
        /// <para/>
        /// Enlarging the value of this instance would exceed <see cref="StringBuilder.MaxCapacity"/>.
        /// </exception>
        public static StringBuilder Insert(this StringBuilder text, int index, ICharSequence? charSequence)
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);
            if ((uint)index > (uint)text.Length)
                throw new ArgumentOutOfRangeException(nameof(index), SR.ArgumentOutOfRange_IndexMustBeLessOrEqual);

            // For null values, this is a no-op
            if (charSequence is null || !charSequence.HasValue || charSequence.Length == 0)
                return text;

            if (charSequence is StringCharSequence str)
                return text.Insert(index, str.Value);
            if (charSequence is CharArrayCharSequence charArray)
                return text.Insert(index, charArray.Value);
            if (charSequence is StringBuilderCharSequence sb)
                return Insert(text, index, sb.Value);
            if (charSequence is StringBuffer stringBuffer)
                return Insert(text, index, stringBuffer.builder);

            int start = 0;
            int remainingCount = charSequence.Length;
            int insertIndex = index;

            char[] buffer = ArrayPool<char>.Shared.Rent(Math.Min(remainingCount, CharPoolBufferSize));
            try
            {
                while (remainingCount > 0)
                {
                    // Determine the chunk size for the current iteration
                    int chunkLength = Math.Min(remainingCount, CharPoolBufferSize);

                    // Copy the chunk to the buffer
                    int bufferIndex = 0;
                    int end = start + chunkLength;
                    for (int i = start; i < end; i++)
                    {
                        buffer[bufferIndex++] = charSequence[i];
                    }

                    text.Insert(insertIndex, buffer, 0, chunkLength);

                    start += chunkLength;
                    remainingCount -= chunkLength;
                    insertIndex += chunkLength;
                }
            }
            finally
            {
                ArrayPool<char>.Shared.Return(buffer);
            }
            return text;
        }

        /// <summary>
        /// Inserts the string representation of a specified subarray of Unicode characters into this instance at the specified character position.
        /// <para/>
        /// IMPORTANT: This method has .NET semantics. That is, the <paramref name="charCount"/> parameter is a count, not an exclusive end index as would be the
        /// case in Java. To translate from Java, use <c>end - start</c> to resolve <paramref name="charCount"/>.
        /// </summary>
        /// <param name="text">This <see cref="StringBuilder"/>.</param>
        /// <param name="index">The position in this instance where insertion begins.</param>
        /// <param name="charSequence">A character sequence.</param>
        /// <param name="startIndex">The starting index within <paramref name="charSequence"/>.</param>
        /// <param name="charCount">The number of characters to insert.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="text"/> is <c>null</c>.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="charSequence"/> is <c>null</c>, and <paramref name="startIndex"/> and <paramref name="charCount"/> are not zero.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/>, <paramref name="startIndex"/> or <paramref name="charCount"/> is less than zero.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="index"/> is greater than the length of this instance.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="startIndex"/> plus <paramref name="charCount"/> is not a position within <paramref name="charSequence"/>.
        /// <para/>
        /// -or-
        /// <para/>
        /// Enlarging the value of this instance would exceed <see cref="StringBuilder.MaxCapacity"/>.
        /// </exception>
        public static StringBuilder Insert(this StringBuilder text, int index, ICharSequence? charSequence, int startIndex, int charCount)
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);
            if ((uint)index > (uint)text.Length)
                throw new ArgumentOutOfRangeException(nameof(index), SR.ArgumentOutOfRange_IndexMustBeLessOrEqual);
            if (startIndex < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(startIndex, ExceptionArgument.startIndex);
            if (charCount < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(charCount, ExceptionArgument.charCount);
            if (charSequence is null && (startIndex != 0 || charCount != 0))
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.charSequence); // J2N: Unlike Java, we are throwing an exception (to match .NET Core 3) rather than writing "null" to the StringBuilder
            if (charSequence is null || !charSequence.HasValue)
                return text;
            if (startIndex > charSequence.Length - charCount) // Checks for int overflow
                throw new ArgumentOutOfRangeException(nameof(charCount), SR.ArgumentOutOfRange_IndexCount);

            if (charSequence is StringCharSequence stringCharSequence)
                return text.Insert(index, stringCharSequence.Value, startIndex, charCount);
            if (charSequence is CharArrayCharSequence charArrayCharSequence)
                return text.Insert(index, charArrayCharSequence.Value, startIndex, charCount);
            if (charSequence is StringBuilderCharSequence sbCharSequence)
                return text.Insert(index, sbCharSequence.Value, startIndex, charCount);
            if (charSequence is StringBuffer stringBuffer)
                return text.Insert(index, stringBuffer.builder, startIndex, charCount);

            int start = startIndex;
            int remainingCount = charCount;
            int insertIndex = index;

            char[] buffer = ArrayPool<char>.Shared.Rent(Math.Min(remainingCount, CharPoolBufferSize));
            try
            {
                while (remainingCount > 0)
                {
                    // Determine the chunk size for the current iteration
                    int chunkLength = Math.Min(remainingCount, CharPoolBufferSize);

                    // Copy the chunk to the buffer
                    int bufferIndex = 0;
                    int end = start + chunkLength;
                    for (int i = start; i < end; i++)
                    {
                        buffer[bufferIndex++] = charSequence[i];
                    }

                    text.Insert(insertIndex, buffer, 0, chunkLength);

                    start += chunkLength;
                    remainingCount -= chunkLength;
                    insertIndex += chunkLength;
                }
            }
            finally
            {
                ArrayPool<char>.Shared.Return(buffer);
            }
            return text;
        }

        /// <summary>
        /// Inserts the string representation of a specified sequence of Unicode characters into this instance at the specified character position.
        /// <para/>
        /// <strong>Usage Note:</strong> The compiler will not automatically call this extension method because
        /// <see cref="StringBuilder.Insert(int, object?)"/> will take precedence over the call. This can be
        /// overcome only by calling this static method explicitly:
        /// <code>
        /// J2N.Text.StringBuilderExtensions.Insert(stringBuilder, 2, stringBuilderToInsert);
        /// </code>
        /// or by explicitly using the <paramref name="charSequence"/> label:
        /// <code>
        /// stringBuilder.Insert(2, charSequence: stringBuilderToInsert);
        /// </code>
        /// </summary>
        /// <param name="text">This <see cref="StringBuilder"/>.</param>
        /// <param name="index">The position in this instance where insertion begins.</param>
        /// <param name="charSequence">The character sequence to insert.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="text"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than zero or greater than the length of this instance.
        /// <para/>
        /// -or-
        /// <para/>
        /// Enlarging the value of this instance would exceed <see cref="StringBuilder.MaxCapacity"/>.
        /// </exception>
        public static StringBuilder Insert(this StringBuilder text, int index, StringBuilder? charSequence)
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);
            if ((uint)index > (uint)text.Length)
                throw new ArgumentOutOfRangeException(nameof(index), SR.ArgumentOutOfRange_IndexMustBeLessOrEqual);
            if (charSequence is null || charSequence.Length == 0)
                return text;

            int start = 0;
            int remainingCount = charSequence.Length;
            int insertIndex = index;

            char[] buffer = ArrayPool<char>.Shared.Rent(Math.Min(remainingCount, CharPoolBufferSize));
            try
            {
                while (remainingCount > 0)
                {
                    // Determine the chunk size for the current iteration
                    int chunkLength = Math.Min(remainingCount, CharPoolBufferSize);

                    // Copy the chunk to the buffer
                    charSequence.CopyTo(start, buffer, 0, chunkLength);

                    text.Insert(insertIndex, buffer, 0, chunkLength);

                    start += chunkLength;
                    remainingCount -= chunkLength;
                    insertIndex += chunkLength;
                }
            }
            finally
            {
                ArrayPool<char>.Shared.Return(buffer);
            }
            return text;
        }

        /// <summary>
        /// Inserts the string representation of a specified subarray of Unicode characters into this instance at the specified character position.
        /// <para/>
        /// IMPORTANT: This method has .NET semantics. That is, the <paramref name="charCount"/> parameter is a count, not an exclusive end index as would be the
        /// case in Java. To translate from Java, use <c>end - start</c> to resolve <paramref name="charCount"/>.
        /// </summary>
        /// <param name="text">This <see cref="StringBuilder"/>.</param>
        /// <param name="index">The position in this instance where insertion begins.</param>
        /// <param name="charSequence">A character array.</param>
        /// <param name="startIndex">The starting index within <paramref name="charSequence"/>.</param>
        /// <param name="charCount">The number of characters to insert.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="text"/> is <c>null</c>.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="charSequence"/> is <c>null</c>, and <paramref name="startIndex"/> and <paramref name="charCount"/> are not zero.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/>, <paramref name="startIndex"/> or <paramref name="charCount"/> is less than zero.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="index"/> is greater than the length of this instance.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="startIndex"/> plus <paramref name="charCount"/> is not a position within <paramref name="charSequence"/>.
        /// <para/>
        /// -or-
        /// <para/>
        /// Enlarging the value of this instance would exceed <see cref="StringBuilder.MaxCapacity"/>.
        /// </exception>
        public static StringBuilder Insert(this StringBuilder text, int index, StringBuilder? charSequence, int startIndex, int charCount)
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);
            if ((uint)index > (uint)text.Length)
                throw new ArgumentOutOfRangeException(nameof(index), SR.ArgumentOutOfRange_IndexMustBeLessOrEqual);
            if (startIndex < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(startIndex, ExceptionArgument.startIndex);
            if (charCount < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(charCount, ExceptionArgument.charCount);
            if (charSequence is null && (startIndex != 0 || charCount != 0))
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.charSequence); // J2N: Unlike Java, we are throwing an exception (to match .NET Core 3) rather than writing "null" to the StringBuilder
            if (charSequence is null)
                return text;
            if (startIndex > charSequence.Length - charCount) // Checks for int overflow
                throw new ArgumentOutOfRangeException(nameof(charCount), SR.ArgumentOutOfRange_IndexCount);

            int start = startIndex;
            int remainingCount = charCount;
            int insertIndex = index;

            char[] buffer = ArrayPool<char>.Shared.Rent(Math.Min(remainingCount, CharPoolBufferSize));
            try
            {
                while (remainingCount > 0)
                {
                    // Determine the chunk size for the current iteration
                    int chunkLength = Math.Min(remainingCount, CharPoolBufferSize);

                    // Copy the chunk to the buffer
                    charSequence.CopyTo(start, buffer, 0, chunkLength);

                    text.Insert(insertIndex, buffer, 0, chunkLength);

                    start += chunkLength;
                    remainingCount -= chunkLength;
                    insertIndex += chunkLength;
                }
            }
            finally
            {
                ArrayPool<char>.Shared.Return(buffer);
            }
            return text;
        }

        /// <summary>
        /// Inserts the string representation of a specified subarray of Unicode characters into this instance at the specified character position.
        /// <para/>
        /// IMPORTANT: This method has .NET semantics. That is, the <paramref name="charCount"/> parameter is a count, not an exclusive end index as would be the
        /// case in Java. To translate from Java, use <c>end - start</c> to resolve <paramref name="charCount"/>.
        /// </summary>
        /// <param name="text">This <see cref="StringBuilder"/>.</param>
        /// <param name="index">The position in this instance where insertion begins.</param>
        /// <param name="value">A character array.</param>
        /// <param name="startIndex">The starting index within <paramref name="value"/>.</param>
        /// <param name="charCount">The number of characters to insert.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="text"/> is <c>null</c>.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="value"/> is <c>null</c>, and <paramref name="startIndex"/> and <paramref name="charCount"/> are not zero.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/>, <paramref name="startIndex"/> or <paramref name="charCount"/> is less than zero.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="index"/> is greater than the length of this instance.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="startIndex"/> plus <paramref name="charCount"/> is not a position within <paramref name="value"/>.
        /// <para/>
        /// -or-
        /// <para/>
        /// Enlarging the value of this instance would exceed <see cref="StringBuilder.MaxCapacity"/>.
        /// </exception>
        public static StringBuilder Insert(this StringBuilder text, int index, string? value, int startIndex, int charCount)
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);
            if ((uint)index > (uint)text.Length)
                throw new ArgumentOutOfRangeException(nameof(index), SR.ArgumentOutOfRange_IndexMustBeLessOrEqual);
            if (startIndex < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(startIndex, ExceptionArgument.startIndex);
            if (charCount < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(charCount, ExceptionArgument.charCount);
            if (value is null)
            {
                if (startIndex != 0 || charCount != 0)
                    ThrowHelper.ThrowArgumentNullException(ExceptionArgument.value); // J2N: Unlike Java, we are throwing an exception (to match .NET Core 3) rather than writing "null" to the StringBuilder
                return text;
            }
            if (startIndex > value.Length - charCount) // Checks for int overflow
                throw new ArgumentOutOfRangeException(nameof(charCount), SR.ArgumentOutOfRange_IndexCount);

#if FEATURE_STRINGBUILDER_INSERT_READONLYSPAN // If this method isn't supported, we are buffering to an array pool to get to the stack, anyway.
            return text.Insert(index, value.AsSpan(startIndex, charCount));
#else
            int start = startIndex;
            int remainingCount = charCount;
            int insertIndex = index;

            char[] buffer = ArrayPool<char>.Shared.Rent(Math.Min(remainingCount, CharPoolBufferSize));
            try
            {
                while (remainingCount > 0)
                {
                    // Determine the chunk size for the current iteration
                    int chunkLength = Math.Min(remainingCount, CharPoolBufferSize);

                    // Copy the chunk to the buffer
                    value.CopyTo(start, buffer, 0, chunkLength);

                    text.Insert(insertIndex, buffer, 0, chunkLength);

                    start += chunkLength;
                    remainingCount -= chunkLength;
                    insertIndex += chunkLength;
                }
            }
            finally
            {
                ArrayPool<char>.Shared.Return(buffer);
            }
            return text;
#endif
        }

        /// <summary>
        /// Inserts the sequence of characters into this <see cref="StringBuilder"/>
        /// at the specified character position.
        /// </summary>
        /// <param name="text">This <see cref="StringBuilder"/>.</param>
        /// <param name="index">The position in this instance where insertion begins.</param>
        /// <param name="charSequence">The <see cref="ReadOnlySpan{Char}"/> to insert.</param>
        /// <returns>This <see cref="StringBuilder"/>, for chaining.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="text"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than zero or greater
        /// than the length of this instance.</exception>
        // J2N: Added to cover the missing .NET API on older .NET target frameworks.
        public static StringBuilder Insert(this StringBuilder text, int index, ReadOnlySpan<char> charSequence)
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);
#if FEATURE_STRINGBUILDER_INSERT_READONLYSPAN
            return text.Insert(index, charSequence);
#else
            if ((uint)index > (uint)text.Length)
                throw new ArgumentOutOfRangeException(nameof(index), SR.ArgumentOutOfRange_IndexMustBeLessOrEqual);

            if (charSequence.Length > 0)
            {
                int startIndex = 0;
                int remainingCount = charSequence.Length;
                int insertIndex = index;

                char[] buffer = ArrayPool<char>.Shared.Rent(Math.Min(remainingCount, CharPoolBufferSize));
                try
                {
                    while (remainingCount > 0)
                    {
                        // Determine the chunk size for the current iteration
                        int chunkLength = Math.Min(remainingCount, CharPoolBufferSize);

                        // Copy the chunk to the buffer
                        charSequence.Slice(startIndex, chunkLength).CopyTo(buffer);

                        text.Insert(insertIndex, buffer, 0, chunkLength);

                        startIndex += chunkLength;
                        remainingCount -= chunkLength;
                        insertIndex += chunkLength;
                    }
                }
                finally
                {
                    ArrayPool<char>.Shared.Return(buffer);
                }
            }
            return text;
#endif
        }

        #endregion

        #region InsertCodePoint

        /// <summary>
        /// Insert the string representation of the <paramref name="codePoint"/>
        /// argument to this sequence at <paramref name="index"/>.
        /// <para>
        /// The argument is inserted into to the contents of this sequence.
        /// The length of this sequence increases by <see cref="Character.CharCount(int)"/>.
        /// </para>
        /// <para>
        /// The overall effect is exactly as if the argument were
        /// converted to a <see cref="char"/> array by the method
        /// <see cref="Character.ToChars(int)"/> and the character in that array
        /// were then <see cref="StringBuilder.Insert(int, char[])">inserted</see> into this
        /// <see cref="StringBuilder"/>.
        /// </para>
        /// </summary>
        /// <param name="text">This <see cref="StringBuilder"/>.</param>
        /// <param name="index">The position in this instance where insertion begins.</param>
        /// <param name="codePoint">A Unicode code point.</param>
        /// <returns>This <see cref="StringBuilder"/>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="text"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than zero or greater
        /// than the length of this instance.</exception>
        /// <exception cref="ArgumentException"><paramref name="codePoint"/> is not a valid Unicode code point.</exception>
        public static StringBuilder InsertCodePoint(this StringBuilder text, int index, int codePoint)
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);
            if (index < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(index, ExceptionArgument.index);
            if (!Character.IsValidCodePoint(codePoint))
                ThrowHelper.ThrowArgumentException_Argument_InvalidCodePoint(codePoint);

            if (codePoint < Character.MinSupplementaryCodePoint)
            {
                text.Insert(index, (char)codePoint);
                return text;
            }

            return InsertCodePointSlow(text, index, codePoint);

            static StringBuilder InsertCodePointSlow(StringBuilder text, int index, int codePoint)
            {
                Character.ToChars(codePoint, out char high, out char low);
#if FEATURE_STRINGBUILDER_APPEND_CHARPTR
                Span<char> ch = stackalloc char[2];
                ch[0] = high;
                ch[1] = low;
                text.Insert(index, ch);
#else
                text.Insert(index, high);
                text.Insert(index + 1, low);
#endif
                return text;
            }
        }

        #endregion InsertCodePoint

        #region LastIndexOf

        // J2N: Removed this overload to prevent it from accidentally selecting culture sensitivity when it is not desired
        ///// <summary>
        ///// Searches for the last index of the specified character. The search for
        ///// the character starts at the end and moves towards the beginning.
        ///// <para/>
        ///// Usage Note: This method has .NET semantics - it uses the current culture to compare the string.
        ///// To match Java, use the <see cref="LastIndexOf(StringBuilder, string, StringComparison)"/> overload
        ///// with <see cref="StringComparison.Ordinal"/>.
        ///// </summary>
        ///// <param name="text">This <see cref="StringBuilder"/>.</param>
        ///// <param name="value">The string to find.</param>
        ///// <returns>The index of the specified character, <c>-1</c> if the character isn't found.</returns>
        ///// <exception cref="ArgumentNullException">If <paramref name="text"/> or <paramref name="value"/> is <c>null</c>.</exception>
        //public static int LastIndexOf(this StringBuilder text, string value)
        //{
        //    if (text is null)
        //        ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);
        //    return LastIndexOf(text, value, text.Length, StringComparison.CurrentCulture);
        //}

        // J2N: Removed this overload to prevent it from accidentally selecting culture sensitivity when it is not desired
        ///// <summary>
        ///// Searches for the index of the specified character. The search for the
        ///// character starts at the specified offset and moves towards the beginning.
        ///// <para/>
        ///// Usage Note: This method has .NET semantics - it uses the current culture to compare the string.
        ///// To match Java, use the <see cref="LastIndexOf(StringBuilder, string, int, StringComparison)"/> overload
        ///// with <see cref="StringComparison.Ordinal"/>.
        ///// </summary>
        ///// <param name="text">This <see cref="StringBuilder"/>.</param>
        ///// <param name="value">The string to find.</param>
        ///// <param name="startIndex">The starting offset.</param>
        ///// <returns>The index of the specified character, <c>-1</c> if the character isn't found.</returns>
        ///// <exception cref="ArgumentNullException">If <paramref name="text"/> or <paramref name="value"/> is <c>null</c>.</exception>
        //public static int LastIndexOf(this StringBuilder text, string value, int startIndex)
        //{
        //    if (text is null)
        //        ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);
        //    return LastIndexOf(text, value, startIndex, StringComparison.CurrentCulture);
        //}

        /// <summary>
        /// Searches for the index of the specified character. The search for the
        /// character starts at the specified offset and moves towards the beginning.
        /// </summary>
        /// <param name="text">This <see cref="StringBuilder"/>.</param>
        /// <param name="value">The string to find.</param>
        /// <param name="comparisonType">One of the enumeration values that specifies the rules for the search.</param>
        /// <returns>The index of the specified character, <c>-1</c> if the character isn't found.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="text"/> or <paramref name="value"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException"><paramref name="comparisonType"/> is not a <see cref="StringComparison"/> value.</exception>
        public static int LastIndexOf(this StringBuilder text, string value, StringComparison comparisonType)
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);
            return LastIndexOf(text, value, text.Length - 1, comparisonType);
        }

        /// <summary>
        /// Searches for the index of the specified character. The search for the
        /// character starts at the specified offset and moves towards the beginning.
        /// </summary>
        /// <param name="text">This <see cref="StringBuilder"/>.</param>
        /// <param name="value">The string to find.</param>
        /// <param name="startIndex">The starting offset.</param>
        /// <param name="comparisonType">One of the enumeration values that specifies the rules for the search.</param>
        /// <returns>The index of the specified character, <c>-1</c> if the character isn't found.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="text"/> or <paramref name="value"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="startIndex"/> is less than 0 (zero) or greater than the length of this <see cref="StringBuilder"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="comparisonType"/> is not a <see cref="StringComparison"/> value.</exception>
        public static int LastIndexOf(this StringBuilder text, string value, int startIndex, StringComparison comparisonType)
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);
            if (value is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.value);
            if ((uint)startIndex > (uint)text.Length)
                ThrowHelper.ThrowArgumentOutOfRange_ArgumentOutOfRange_IndexString(startIndex, ExceptionArgument.startIndex);

            if (value.Length == 0)
                return text.Length;

            switch (comparisonType)
            {
                case StringComparison.Ordinal:
                    return LastIndexOfOrdinal(text, value, startIndex);
                case StringComparison.OrdinalIgnoreCase:
                    return LastIndexOfOrdinalIgnoreCase(text, value, startIndex);
                default:
                    // J2N TODO: Optimize other StringComparison options better
                    int start = Math.Min(startIndex, text.Length - 1);
                    return text.ToString(0, start + 1).LastIndexOf(value, comparisonType);
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int LastIndexOfOrdinal(StringBuilder text, string value, int startIndex)
        {
            int textLength = text.Length;
            char[]? arrayToReturnToPool = null;
            try
            {
#if FEATURE_STRINGBUILDER_COPYTO_SPAN // If this method isn't supported, we are buffering to an array pool to get to the stack, anyway.
                Span<char> textChars = textLength > CharStackBufferSize
                    ? (arrayToReturnToPool = ArrayPool<char>.Shared.Rent(textLength))
                    : stackalloc char[textLength];
                text.CopyTo(0, textChars, textLength);
#else
                Span<char> textChars = arrayToReturnToPool = ArrayPool<char>.Shared.Rent(textLength);
                text.CopyTo(0, arrayToReturnToPool, 0, textLength);
#endif
                int subCount = value.Length;

                if (subCount <= textLength && startIndex >= 0)
                {
                    if (subCount > 0)
                    {
                        if (startIndex > textLength - subCount)
                        {
                            startIndex = textLength - subCount; // count and subCount are both >= 1
                        }
                        char firstChar = value[0];
                        while (true)
                        {
                            int i = startIndex;
                            bool found = false;
                            for (; i >= 0; --i)
                            {
                                if (textChars[i] == firstChar)
                                {
                                    found = true;
                                    break;
                                }
                            }
                            if (!found)
                            {
                                return -1;
                            }
                            int o1 = i, o2 = 0;
                            while (++o2 < subCount && textChars[++o1] == value[o2])
                            {
                                // Intentionally empty
                            }
                            if (o2 == subCount)
                            {
                                return i;
                            }
                            startIndex = i - 1;
                        }
                    }
                    return startIndex < textLength ? startIndex : textLength;
                }
                return -1;
            }
            finally
            {
                ArrayPool<char>.Shared.ReturnIfNotNull(arrayToReturnToPool);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int LastIndexOfOrdinalIgnoreCase(StringBuilder text, string value, int startIndex)
        {
            int textLength = text.Length;
            char[]? arrayToReturnToPool = null;
            try
            {
#if FEATURE_STRINGBUILDER_COPYTO_SPAN // If this method isn't supported, we are buffering to an array pool to get to the stack, anyway.
                Span<char> textChars = textLength > CharStackBufferSize
                    ? (arrayToReturnToPool = ArrayPool<char>.Shared.Rent(textLength))
                    : stackalloc char[textLength];
                text.CopyTo(0, textChars, textLength);
#else
                Span<char> textChars = arrayToReturnToPool = ArrayPool<char>.Shared.Rent(textLength);
                text.CopyTo(0, arrayToReturnToPool, 0, textLength);
#endif
                int subCount = value.Length;

                if (subCount <= textLength && startIndex >= 0)
                {
                    if (subCount > 0)
                    {
                        if (startIndex > textLength - subCount)
                        {
                            startIndex = textLength - subCount; // count and subCount are both >= 1
                        }
                        char firstChar = value[0], c1, c2;
                        var textInfo = CultureInfo.InvariantCulture.TextInfo;
                        while (true)
                        {
                            int i = startIndex;
                            bool found = false;
                            for (; i >= 0; --i)
                            {
                                if (textChars[i] == firstChar)
                                {
                                    found = true;
                                    break;
                                }
                            }
                            if (!found)
                            {
                                return -1;
                            }
                            int o1 = i, o2 = 0;
                            while (++o2 < subCount && 
                                ((c1 = textChars[++o1]) == (c2 = value[o2]) ||
                                textInfo.ToUpper(c1) == textInfo.ToUpper(c2) ||
                                // Required for unicode that we test both cases
                                textInfo.ToLower(c1) == textInfo.ToLower(c2)))
                            {
                                // Intentionally empty
                            }
                            if (o2 == subCount)
                            {
                                return i;
                            }
                            startIndex = i - 1;
                        }
                    }
                    return startIndex < textLength ? startIndex : textLength;
                }
                return -1;
            }
            finally
            {
                ArrayPool<char>.Shared.ReturnIfNotNull(arrayToReturnToPool);
            }
        }

        #endregion LastIndexOf

        #region Replace

        /// <summary>
        /// Replaces the specified subsequence in this builder with the specified
        /// string, <paramref name="newValue"/>. The substring begins at the specified
        /// <paramref name="startIndex"/> and ends to the character at 
        /// <c><paramref name="count"/> - <paramref name="startIndex"/></c> or
        /// to the end of the sequence if no such character exists. First the
        /// characters in the substring ar removed and then the specified 
        /// <paramref name="newValue"/> is inserted at <paramref name="startIndex"/>.
        /// This <see cref="StringBuilder"/> will be lengthened to accommodate the
        /// specified <paramref name="newValue"/> if necessary.
        /// <para/>
        /// IMPORTANT: This method has .NET semantics. That is, the <paramref name="count"/> parameter is a count
        /// rather than an exclusive end index. To translate from Java, use <c>end - start</c>
        /// to resolve the <paramref name="count"/> parameter.
        /// </summary>
        /// <param name="text">This <see cref="StringBuilder"/>.</param>
        /// <param name="startIndex">The inclusive begin index in <paramref name="text"/>.</param>
        /// <param name="count">The number of characters to replace.</param>
        /// <param name="newValue">The replacement string.</param>
        /// <returns>This <see cref="StringBuilder"/> builder.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startIndex"/> or <paramref name="count"/> is less than zero.
        /// <para/>
        /// -or-
        /// <para/>
        /// Enlarging the value of this instance would exceed <see cref="StringBuilder.MaxCapacity"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">If <paramref name="text"/> or <paramref name="newValue"/> is <c>null</c>.</exception>
        public static StringBuilder Replace(this StringBuilder text, int startIndex, int count, string newValue)
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);
            if (newValue is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.newValue);
            if (startIndex < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(startIndex, ExceptionArgument.startIndex);
            if (count < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(count, ExceptionArgument.count);
            if (text.MaxCapacity > 0 && startIndex > text.MaxCapacity - count)
                throw new ArgumentOutOfRangeException(nameof(count), 
                    $"{nameof(startIndex)}: {startIndex} + {nameof(count)}: {count} > {nameof(text.MaxCapacity)}: {text.MaxCapacity}");

            int end = startIndex + count;
            if (end > text.Length)
            {
                end = text.Length;
            }
            if (end > startIndex)
            {
                int stringLength = newValue.Length;
                int diff = end - startIndex - stringLength;
                if (diff > 0)
                { // replacing with fewer characters
                    text.Remove(startIndex, diff);
                }
                else if (diff < 0)
                {
#if FEATURE_STRINGBUILDER_INSERT_READONLYSPAN
                    // We just need to add some blank space which will be written to later.
                    // We reuse the allocation by either putting it on the stack or using the array pool.
                    char[]? arrayToReturnToPool = null;
                    try
                    {
                        int diffLength = -diff;
                        Span<char> diffChars = diffLength > CharStackBufferSize
                            ? (arrayToReturnToPool = ArrayPool<char>.Shared.Rent(diffLength)).AsSpan(0, diffLength)
                            : stackalloc char[diffLength];

                        // replacing with more characters...need some room
                        text.Insert(startIndex, diffChars);
                    }
                    finally
                    {
                        ArrayPool<char>.Shared.ReturnIfNotNull(arrayToReturnToPool);
                    }
#else
                    // replacing with more characters...need some room
                    text.Insert(startIndex, new char[-diff]);
#endif
                }
                // copy the chars based on the new length
#if FEATURE_STRINGBUILDER_GETCHUNKS
                var textEnumerator = text.GetChunks();
                ReadOnlyMemory<char> textChunk = default;
                int lowerBound = 0, upperBound = -1;
                for (int i = 0; i < stringLength; i++)
                {
                    while (i + startIndex > upperBound)
                    {
                        lowerBound += textChunk.Length;
                        textEnumerator.MoveNext();
                        textChunk = textEnumerator.Current;
                        upperBound += textChunk.Length;
                    }
                    unsafe
                    {
                        using var handle = textChunk.Pin();
                        char* pointer = (char*)handle.Pointer;
                        pointer[i + startIndex - lowerBound] = newValue[i];
                    }
                }
#else
                for (int i = 0; i < stringLength; i++)
                {
                    text[i + startIndex] = newValue[i];
                }
#endif
                return text;
            }
            if (startIndex == end)
            {
                text.Insert(startIndex, newValue);
                return text;
            }
            return text;
        }

        /// <summary>
        /// Replaces the specified subsequence in this builder with the specified
        /// string, <paramref name="newValue"/>. The substring begins at the specified
        /// <paramref name="startIndex"/> and ends to the character at 
        /// <c><paramref name="count"/> - <paramref name="startIndex"/></c> or
        /// to the end of the sequence if no such character exists. First the
        /// characters in the substring ar removed and then the specified 
        /// <paramref name="newValue"/> is inserted at <paramref name="startIndex"/>.
        /// This <see cref="StringBuilder"/> will be lengthened to accommodate the
        /// specified <paramref name="newValue"/> if necessary.
        /// <para/>
        /// IMPORTANT: This method has .NET semantics. That is, the <paramref name="count"/> parameter is a count
        /// rather than an exclusive end index. To translate from Java, use <c>end - start</c>
        /// to resolve the <paramref name="count"/> parameter.
        /// </summary>
        /// <param name="text">This <see cref="StringBuilder"/>.</param>
        /// <param name="startIndex">The inclusive begin index in <paramref name="text"/>.</param>
        /// <param name="count">The number of characters to replace.</param>
        /// <param name="newValue">The replacement string.</param>
        /// <returns>This <see cref="StringBuilder"/> builder.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startIndex"/> or <paramref name="count"/> is less than zero.
        /// <para/>
        /// -or-
        /// <para/>
        /// Enlarging the value of this instance would exceed <see cref="StringBuilder.MaxCapacity"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">If <paramref name="text"/> is <c>null</c>.</exception>
        public static StringBuilder Replace(this StringBuilder text, int startIndex, int count, ReadOnlySpan<char> newValue)
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);
            if (startIndex < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(startIndex, ExceptionArgument.startIndex);
            if (count < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(count, ExceptionArgument.count);
            if (text.MaxCapacity > 0 && startIndex > text.MaxCapacity - count)
                throw new ArgumentOutOfRangeException(nameof(count),
                    $"{nameof(startIndex)}: {startIndex} + {nameof(count)}: {count} > {nameof(text.MaxCapacity)}: {text.MaxCapacity}");

            int end = startIndex + count;
            if (end > text.Length)
            {
                end = text.Length;
            }
            if (end > startIndex)
            {
                int stringLength = newValue.Length;
                int diff = end - startIndex - stringLength;
                if (diff > 0)
                { // replacing with fewer characters
                    text.Remove(startIndex, diff);
                }
                else if (diff < 0)
                {
#if FEATURE_STRINGBUILDER_INSERT_READONLYSPAN
                    // We just need to add some blank space which will be written to later.
                    // We reuse the allocation by either putting it on the stack or using the array pool.
                    char[]? arrayToReturnToPool = null;
                    try
                    {
                        int diffLength = -diff;
                        Span<char> diffChars = diffLength > CharStackBufferSize
                            ? (arrayToReturnToPool = ArrayPool<char>.Shared.Rent(diffLength)).AsSpan(0, diffLength)
                            : stackalloc char[diffLength];

                        // replacing with more characters...need some room
                        text.Insert(startIndex, diffChars);
                    }
                    finally
                    {
                        ArrayPool<char>.Shared.ReturnIfNotNull(arrayToReturnToPool);
                    }
#else
                    // replacing with more characters...need some room
                    text.Insert(startIndex, new char[-diff]);
#endif
                }
                // copy the chars based on the new length
#if FEATURE_STRINGBUILDER_GETCHUNKS
                var textEnumerator = text.GetChunks();
                ReadOnlyMemory<char> textChunk = default;
                int lowerBound = 0, upperBound = -1;
                for (int i = 0; i < stringLength; i++)
                {
                    while (i + startIndex > upperBound)
                    {
                        lowerBound += textChunk.Length;
                        textEnumerator.MoveNext();
                        textChunk = textEnumerator.Current;
                        upperBound += textChunk.Length;
                    }
                    unsafe
                    {
                        using var handle = textChunk.Pin();
                        char* pointer = (char*)handle.Pointer;
                        pointer[i + startIndex - lowerBound] = newValue[i];
                    }
                }
#else
                for (int i = 0; i < stringLength; i++)
                {
                    text[i + startIndex] = newValue[i];
                }
#endif
                return text;
            }
            if (startIndex == end)
            {
                text.Insert(startIndex, newValue);
                return text;
            }
            return text;
        }

        #endregion Replace

        #region Reverse

        /// <summary>
        /// Causes this character sequence to be replaced by the reverse of
        /// the sequence. If there are any surrogate pairs included in the
        /// sequence, these are treated as single characters for the
        /// reverse operation. Thus, the order of the high-low surrogates
        /// is never reversed.
        /// <para/>
        /// IMPORTANT: This operation is done in-place. Although a <see cref="StringBuilder"/>
        /// is returned, it is the SAME instance as the one that is passed in.
        /// <para/>
        /// Let <c>n</c> be the character length of this character sequence
        /// (not the length in <see cref="char"/> values) just prior to
        /// execution of the <see cref="Reverse(StringBuilder)"/> method. Then the
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
        /// Usage Note: This is the same operation as Java's StringBuilder.reverse()
        /// method. However, J2N also provides <see cref="J2N.Text.StringExtensions.ReverseText(string)"/>
        /// and <see cref="J2N.MemoryExtensions.ReverseText(Span{char})"/> which
        /// don't require a <see cref="StringBuilder"/> instance.
        /// </summary>
        /// <param name="text">this <see cref="StringBuilder"/></param>
        /// <returns>A reference to this <see cref="StringBuilder"/>, for chaining.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="text"/> is <c>null</c>.</exception>
        /// <seealso cref="J2N.Text.StringExtensions.ReverseText(string)"/>
        /// <seealso cref="J2N.MemoryExtensions.ReverseText(Span{char})"/>
        public static StringBuilder Reverse(this StringBuilder text)
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            int length = text.Length;
            if (length <= 1) return text;

            char[]? arrayToReturnToPool = null;
            try
            {
#if FEATURE_STRINGBUILDER_COPYTO_SPAN // If this method isn't supported, we are buffering to an array pool to get to the stack, anyway.
                Span<char> textChars = length > CharStackBufferSize
                    ? (arrayToReturnToPool = ArrayPool<char>.Shared.Rent(length))
                    : stackalloc char[length];
                text.CopyTo(0, textChars, length);
#else
                Span<char> textChars = arrayToReturnToPool = ArrayPool<char>.Shared.Rent(length);
                text.CopyTo(0, arrayToReturnToPool, 0, length);
#endif
                text.Length = 0;
                for (int i = length - 1; i >= 0; i--)
                {
                    char ch = textChars[i];
                    if (char.IsLowSurrogate(ch) && i > 0)
                    {
                        char ch2 = textChars[i - 1];
                        if (char.IsHighSurrogate(ch2))
                        {
                            text.Append(ch2);
                            text.Append(ch);
                            i--;
                            continue;
                        }
                    }
                    text.Append(ch);
                }
                return text;
            }
            finally
            {
                ArrayPool<char>.Shared.ReturnIfNotNull(arrayToReturnToPool);
            }
        }

        #endregion Reverse

        #region Subsequence

        /// <summary>
        /// Retrieves a sub-sequence from this instance.
        /// The sub-sequence starts at a specified character position and has a specified length.
        /// <para/>
        /// IMPORTANT: This method has .NET semantics, that is, the <paramref name="length"/> parameter is a length,
        /// not an exclusive end index as it would be in Java.
        /// </summary>
        /// <param name="startIndex">
        /// The start index of the sub-sequence. It is inclusive, that
        /// is, the index of the first character that is included in the
        /// sub-sequence.
        /// </param>
        /// <param name="text">This <see cref="StringBuilder"/>.</param>
        /// <param name="length">The number of characters to return in the sub-sequence.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startIndex"/> plus <paramref name="length"/> indicates a position not within this instance.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="startIndex"/> or <paramref name="length"/> is less than zero.
        /// </exception>
        public static ICharSequence Subsequence(this StringBuilder? text, int startIndex, int length)
        {
            // From Apache Harmony String class
            if (text is null || (startIndex == 0 && length == text.Length))
            {
                return text.AsCharSequence();
            }
            if (startIndex < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(startIndex, ExceptionArgument.startIndex);
            if (length < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(length, ExceptionArgument.length);
            if (startIndex > text.Length - length) // Checks for int overflow
                ThrowHelper.ThrowArgumentOutOfRange_IndexLengthString(startIndex, length);

            return text.ToString(startIndex, length).AsCharSequence();
        }

        #endregion Subsequence

        #region TryAsSpan

#if FEATURE_STRINGBUILDER_GETCHUNKS

        /// <summary>
        /// Gets a <see cref="ReadOnlySpan{Char}"/> from a <see cref="StringBuilder"/> if it
        /// contains a contiguous block memory corresponding with the location specified by
        /// <paramref name="startIndex"/> and <paramref name="length"/>.
        /// </summary>
        /// <param name="text">This <see cref="StringBuilder"/>.</param>
        /// <param name="startIndex">The inclusive first index in the range.</param>
        /// <param name="length">The number of characters to retrieve.</param>
        /// <param name="span">Upon successful return of this method, contains a
        /// <see cref="ReadOnlySpan{Char}"/> corresponding to the specified
        /// <paramref name="startIndex"/> and <paramref name="length"/>.</param>
        /// <returns><c>true</c> if the <see cref="StringBuilder"/> contains a chunk of memory
        /// of <paramref name="length"/> starting at the index <paramref name="startIndex"/>; othewise, <c>false</c>.
        /// Note that the <c>false</c> case may occur frequently in practice and it is recommended to have a fallback 
        /// approach using <see cref="StringBuilder.CopyTo(int, char[], int, int)"/>,
        /// <see cref="StringBuilder.CopyTo(int, Span{char}, int)"/>, or <see cref="StringBuilder.ToString(int, int)"/>.</returns>
        internal static bool TryAsSpan(this StringBuilder text, int startIndex, int length, out ReadOnlySpan<char> span)
        {
            Debug.Assert(text != null);
            Debug.Assert(startIndex >= 0);
            Debug.Assert(length >= 0);

            span = default;

            if (length == 0)
            {
                return true;
            }

            int offset = 0;

            foreach (var chunk in text.GetChunks())
            {
                int chunkLength = chunk.Length;

                // If the startIndex is before this chunk, we can return false immediately
                if (startIndex - offset < 0)
                {
                    return false;
                }

                // If the startIndex is within this chunk
                if (startIndex - offset < chunkLength)
                {
                    int relativeStartIndex = startIndex - offset;

                    // Check if the entire requested range is within this chunk
                    if (chunkLength - relativeStartIndex >= length)
                    {
                        span = chunk.Span.Slice(relativeStartIndex, length);
                        return true;
                    }
                    else
                    {
                        // If the range exceeds the current chunk, return false
                        return false;
                    }
                }

                offset += chunkLength;
            }

            // If the loop ends and no valid span was found, return false
            return false;
        }

#endif

        #endregion TryAsSpan
    }
}
