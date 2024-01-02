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

using System;
#if FEATURE_SPAN
using System.Buffers;
#endif
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;


namespace J2N.Text
{
    using SR = J2N.Resources.Strings;

    /// <summary>
    /// Extensions to the <see cref="StringBuilder"/> class.
    /// </summary>
    public static class StringBuilderExtensions
    {
        private const int CharStackBufferSize = 64;
        private const int CharPoolBufferSize = 512;

        #region Append

        // This doesn't work right. See: https://stackoverflow.com/a/26885473
        // However, we can fallback on the Append(object) overload and it will call ToString().
        // To work around, call the charSequenceLabel explicitly : sb.IndexOf(charSequence: theCharSequence);
        /// <summary>
        /// Appends the given <see cref="ICharSequence"/> to this <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="text">This <see cref="StringBuilder"/>.</param>
        /// <param name="charSequence">The <see cref="ICharSequence"/> to append.</param>
        /// <returns>This <see cref="StringBuilder"/>, for chaining.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="text"/> is <c>null</c>.</exception>
        public static StringBuilder Append(this StringBuilder text, ICharSequence? charSequence)
        {
            if (text is null)
                throw new ArgumentNullException(nameof(text));

            // For null values, this is a no-op
            if (charSequence is null || !charSequence.HasValue)
                return text;

            if (charSequence is StringCharSequence str)
                return text.Append(str.Value);
            else if (charSequence is CharArrayCharSequence charArray)
                return text.Append(charArray.Value);
            else if (charSequence is StringBuilderCharSequence sb)
#if FEATURE_STRINGBUILDER_APPEND_STRINGBUILDER
                return text.Append(sb.Value);
#else
                return Append(text, sb.Value);
#endif
            else if (charSequence is StringBuffer stringBuffer)
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
#if FEATURE_ARRAYPOOL
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
#else
                text.Append(charSequence.ToString()); // .NET 4.0...don't care to optimize
#endif
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
                throw new ArgumentNullException(nameof(text));
            if (charSequence is null && (startIndex != 0 || charCount != 0))
                throw new ArgumentNullException(nameof(charSequence)); // J2N: Unlike Java, we are throwing an exception (to match .NET Core 3) rather than writing "null" to the StringBuilder
            if (startIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(startIndex), SR.ArgumentOutOfRange_NeedNonNegNum);
            if (charCount < 0)
                throw new ArgumentOutOfRangeException(nameof(charCount), SR.ArgumentOutOfRange_NeedNonNegNum);

            if (charSequence == null || !charSequence.HasValue)
                return text;

            if (startIndex > charSequence.Length - charCount) // Checks for int overflow
                throw new ArgumentOutOfRangeException(nameof(charCount), SR.ArgumentOutOfRange_IndexLength);

            if (charSequence is CharArrayCharSequence charArrayCharSequence)
                return text.Append(charArrayCharSequence.Value, startIndex, charCount);
            if (charSequence is StringBuilderCharSequence stringBuilderCharSequence)
                return text.Append(stringBuilderCharSequence.Value, startIndex, charCount);
            if (charSequence is StringCharSequence stringCharSequence)
                return text.Append(stringCharSequence.Value, startIndex, charCount);
            else if (charSequence is StringBuffer stringBuffer)
                return text.Append(stringBuffer.builder, startIndex, charCount);

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
#if FEATURE_ARRAYPOOL
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
#else
                // .NET 4.0...don't care to optimize
                int end = startIndex + charCount;
                for (int i = startIndex; i < end; i++)
                    text.Append(charSequence[i]);
#endif
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
        /// Usage Note: Unlike in Java, a <c>null</c> <paramref name="charSequence"/> won't append the string
        /// <c>"null"</c> to the <see cref="StringBuilder"/>. Instead, it is a no-op.
        /// </summary>
        /// <param name="text">This <see cref="StringBuilder"/>.</param>
        /// <param name="charSequence">The <see cref="StringBuilder"/> to append.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="text"/> is <c>null</c>.</exception>
        public static StringBuilder Append(this StringBuilder text, StringBuilder? charSequence)
        {
            if (text is null)
                throw new ArgumentNullException(nameof(text));

            if (charSequence is null)
                return text;

#if FEATURE_STRINGBUILDER_APPEND_STRINGBUILDER
            return text.Append(charSequence);
#elif FEATURE_ARRAYPOOL
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
#else
            return text.Append(charSequence.ToString()); // .NET 4.0...don't care to optimize
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
                throw new ArgumentNullException(nameof(text));
            if (charSequence is null && (startIndex != 0 || charCount != 0))
                throw new ArgumentNullException(nameof(charSequence)); // J2N: Unlike Java, we are throwing an exception (to match .NET Core 3) rather than writing "null" to the StringBuilder
            if (startIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(startIndex), SR.ArgumentOutOfRange_NeedNonNegNum);
            if (charCount < 0)
                throw new ArgumentOutOfRangeException(nameof(charCount), SR.ArgumentOutOfRange_NeedNonNegNum);
            if (charSequence is null)
                return text;
            if (startIndex > charSequence.Length - charCount) // Checks for int overflow
                throw new ArgumentOutOfRangeException(nameof(charCount), SR.ArgumentOutOfRange_IndexLength);

#if FEATURE_STRINGBUILDER_APPEND_STRINGBUILDER
            return text.Append(charSequence, startIndex, charCount);
#elif FEATURE_ARRAYPOOL
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
#else
            return text.Append(charSequence.ToString(startIndex, charCount)); // .NET 4.0...don't care to optimize
#endif
        }

#if FEATURE_SPAN
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
                throw new ArgumentNullException(nameof(text));

#if FEATURE_STRINGBUILDER_APPEND_CHARPTR
            unsafe
            {
                fixed (char* seq = charSequence)
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
#endif

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
                throw new ArgumentNullException(nameof(text));
            if (!Character.IsValidCodePoint(codePoint))
                throw new ArgumentException(J2N.SR.Format(SR.Argument_InvalidCodePoint, codePoint));

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
        public static int CompareToOrdinal(this StringBuilder? text, ICharSequence? value) // KEEP OVERLOADS FOR ICharSequence, char[], StringBuilder, and string IN SYNC
        {
            if (text is null) return (value is null || !value.HasValue) ? 0 : -1;
            if (value is null || !value.HasValue) return 1;

            if (value is StringBuilderCharSequence sb)
                return CompareToOrdinal(text, sb.Value);
            if (value is StringBuffer stringBuffer)
                return CompareToOrdinal(text, stringBuffer.builder);

            int length = Math.Min(text.Length, value.Length);
#if FEATURE_STRINGBUILDER_COPYTO_SPAN // If this method isn't supported, we are buffering to an array pool to get to the stack, anyway.
            bool usePool = length > CharStackBufferSize;
            char[]? arrayToReturnToPool = usePool ? ArrayPool<char>.Shared.Rent(length) : null;
            try
#elif FEATURE_ARRAYPOOL
            char[] textChars = ArrayPool<char>.Shared.Rent(length);
            try
#else
            char[] textChars = new char[length];
#endif
            {
#if FEATURE_STRINGBUILDER_COPYTO_SPAN
                Span<char> textChars = usePool ? arrayToReturnToPool : stackalloc char[length];
                text.CopyTo(0, textChars, length);
#else
                text.CopyTo(0, textChars, 0, length);
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
#if FEATURE_STRINGBUILDER_COPYTO_SPAN
            finally
            {
                if (arrayToReturnToPool != null)
                    ArrayPool<char>.Shared.Return(arrayToReturnToPool);
            }
#elif FEATURE_ARRAYPOOL
            finally
            {
                ArrayPool<char>.Shared.Return(textChars);
            }
#endif
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
        public static int CompareToOrdinal(this StringBuilder? text, char[]? value) // KEEP OVERLOADS FOR ICharSequence, char[], StringBuilder, and string IN SYNC
        {
            if (text is null) return (value is null) ? 0 : -1;
            if (value is null) return 1;

            int length = Math.Min(text.Length, value.Length);
#if FEATURE_STRINGBUILDER_COPYTO_SPAN // If this method isn't supported, we are buffering to an array pool to get to the stack, anyway.
            bool usePool = length > CharStackBufferSize;
            char[]? arrayToReturnToPool = usePool ? ArrayPool<char>.Shared.Rent(length) : null;
            try
#elif FEATURE_ARRAYPOOL
            char[] textChars = ArrayPool<char>.Shared.Rent(length);
            try
#else
            char[] textChars = new char[length];
#endif
            {
#if FEATURE_STRINGBUILDER_COPYTO_SPAN
                Span<char> textChars = usePool ? arrayToReturnToPool : stackalloc char[length];
                text.CopyTo(0, textChars, length);
#else
                text.CopyTo(0, textChars, 0, length);
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
#if FEATURE_STRINGBUILDER_COPYTO_SPAN
            finally
            {
                if (arrayToReturnToPool != null)
                    ArrayPool<char>.Shared.Return(arrayToReturnToPool);
            }
#elif FEATURE_ARRAYPOOL
            finally
            {
                ArrayPool<char>.Shared.Return(textChars);
            }
#endif
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
        public static int CompareToOrdinal(this StringBuilder? text, StringBuilder? value) // KEEP OVERLOADS FOR ICharSequence, char[], StringBuilder, and string IN SYNC
        {
            if (object.ReferenceEquals(text, value)) return 0;
            if (text is null) return (value is null) ? 0 : -1;
            if (value is null) return 1;

            int length = Math.Min(text.Length, value.Length);
#if FEATURE_STRINGBUILDER_COPYTO_SPAN // If this method isn't supported, we are buffering to an array pool to get to the stack, anyway.
            bool usePool = length > CharStackBufferSize;
            char[]? textArrayToReturnToPool = usePool ? ArrayPool<char>.Shared.Rent(length) : null;
            char[]? valueArrayToReturnToPool = usePool ? ArrayPool<char>.Shared.Rent(length) : null;
            try
#elif FEATURE_ARRAYPOOL
            char[] textChars = ArrayPool<char>.Shared.Rent(length);
            char[] valueChars = ArrayPool<char>.Shared.Rent(length);
            try
#else
            char[] textChars = new char[length];
            char[] valueChars = new char[length];
#endif
            {
#if FEATURE_STRINGBUILDER_COPYTO_SPAN
                Span<char> textChars = usePool ? textArrayToReturnToPool : stackalloc char[length];
                Span<char> valueChars = usePool ? valueArrayToReturnToPool : stackalloc char[length];
                text.CopyTo(0, textChars, length);
                value.CopyTo(0, valueChars, length);
#else
                text.CopyTo(0, textChars, 0, length);
                value.CopyTo(0, valueChars, 0, length);
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
#if FEATURE_STRINGBUILDER_COPYTO_SPAN
            finally
            {
                if (textArrayToReturnToPool != null)
                    ArrayPool<char>.Shared.Return(textArrayToReturnToPool);
                if (valueArrayToReturnToPool != null)
                    ArrayPool<char>.Shared.Return(valueArrayToReturnToPool);
            }
#elif FEATURE_ARRAYPOOL
            finally
            {
                ArrayPool<char>.Shared.Return(textChars);
                ArrayPool<char>.Shared.Return(valueChars);
            }
#endif
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
        public static int CompareToOrdinal(this StringBuilder? text, string? value) // KEEP OVERLOADS FOR ICharSequence, char[], StringBuilder, and string IN SYNC
        {
            if (text is null) return (value is null) ? 0 : -1;
            if (value is null) return 1;

            int length = Math.Min(text.Length, value.Length);
#if FEATURE_STRINGBUILDER_COPYTO_SPAN // If this method isn't supported, we are buffering to an array pool to get to the stack, anyway.
            bool usePool = length > CharStackBufferSize;
            char[]? arrayToReturnToPool = usePool ? ArrayPool<char>.Shared.Rent(length) : null;
            try
#elif FEATURE_ARRAYPOOL
            char[] textChars = ArrayPool<char>.Shared.Rent(length);
            try
#else
            char[] textChars = new char[length];
#endif
            {
#if FEATURE_STRINGBUILDER_COPYTO_SPAN
                Span<char> textChars = usePool ? arrayToReturnToPool : stackalloc char[length];
                text.CopyTo(0, textChars, length);
#else
                text.CopyTo(0, textChars, 0, length);
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
#if FEATURE_STRINGBUILDER_COPYTO_SPAN
            finally
            {
                if (arrayToReturnToPool != null)
                    ArrayPool<char>.Shared.Return(arrayToReturnToPool);
            }
#elif FEATURE_ARRAYPOOL
            finally
            {
                ArrayPool<char>.Shared.Return(textChars);
            }
#endif
        }

        #endregion CompareToOrdinal

        #region CopyTo

#if FEATURE_SPAN

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
                throw new ArgumentOutOfRangeException(nameof(count), J2N.SR.Format(SR.ArgumentOutOfRange_Generic_MustBeNonNegative, nameof(count), count));

            if ((uint)sourceIndex > (uint)text.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(sourceIndex), SR.ArgumentOutOfRange_IndexMustBeLessOrEqual);
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

#endif

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
                throw new ArgumentNullException(nameof(text));
            if (startIndex < 0 || startIndex > text.Length)
                throw new ArgumentOutOfRangeException(nameof(startIndex));
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), SR.ArgumentOutOfRange_NeedNonNegNum);

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
                throw new ArgumentNullException(nameof(text));
            if (value is null)
                throw new ArgumentNullException(nameof(value));
            if (startIndex < 0 || startIndex > text.Length)
                throw new ArgumentOutOfRangeException(nameof(startIndex));

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

#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private static int IndexOfOrdinal(StringBuilder text, string value, int startIndex)
        {
            int length = value.Length;
            if (length == 0)
                return 0;

            int textLength = text.Length;
#if FEATURE_STRINGBUILDER_COPYTO_SPAN // If this method isn't supported, we are buffering to an array pool to get to the stack, anyway.
            bool usePool = textLength > CharStackBufferSize;
            char[]? arrayToReturnToPool = usePool ? ArrayPool<char>.Shared.Rent(textLength) : null;
            try
#elif FEATURE_ARRAYPOOL
            char[] textChars = ArrayPool<char>.Shared.Rent(textLength);
            try
#else
            char[] textChars = new char[textLength];
#endif
            {
#if FEATURE_STRINGBUILDER_COPYTO_SPAN
                Span<char> textChars = usePool ? arrayToReturnToPool : stackalloc char[textLength];
                text.CopyTo(0, textChars, textLength);
#else
                text.CopyTo(0, textChars, 0, textLength);
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
#if FEATURE_STRINGBUILDER_COPYTO_SPAN
            finally
            {
                if (arrayToReturnToPool != null)
                    ArrayPool<char>.Shared.Return(arrayToReturnToPool);
            }
#elif FEATURE_ARRAYPOOL
            finally
            {
                ArrayPool<char>.Shared.Return(textChars);
            }
#endif
        }

#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private static int IndexOfOrdinalIgnoreCase(StringBuilder text, string value, int startIndex)
        {
            int length = value.Length;
            if (length == 0)
                return 0;

            int textLength = text.Length;
#if FEATURE_STRINGBUILDER_COPYTO_SPAN // If this method isn't supported, we are buffering to an array pool to get to the stack, anyway.
            bool usePool = textLength > CharStackBufferSize;
            char[]? arrayToReturnToPool = usePool ? ArrayPool<char>.Shared.Rent(textLength) : null;
            try
#elif FEATURE_ARRAYPOOL
            char[] textChars = ArrayPool<char>.Shared.Rent(textLength);
            try
#else
            char[] textChars = new char[textLength];
#endif
            {
#if FEATURE_STRINGBUILDER_COPYTO_SPAN
                Span<char> textChars = usePool ? arrayToReturnToPool : stackalloc char[textLength];
                text.CopyTo(0, textChars, textLength);
#else
                text.CopyTo(0, textChars, 0, textLength);
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
#if FEATURE_STRINGBUILDER_COPYTO_SPAN
            finally
            {
                if (arrayToReturnToPool != null)
                    ArrayPool<char>.Shared.Return(arrayToReturnToPool);
            }
#elif FEATURE_ARRAYPOOL
            finally
            {
                ArrayPool<char>.Shared.Return(textChars);
            }
#endif
        }

        #endregion IndexOf

        #region Insert

        /// <summary>
        /// Inserts the string representation of a specified sequence of Unicode characters into this instance at the specified character position.
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
                throw new ArgumentNullException(nameof(text));
            if (index < 0 || index > text.Length)
                throw new ArgumentOutOfRangeException(nameof(index));

            // For null values, this is a no-op
            if (charSequence is null || !charSequence.HasValue)
                return text;

            if (charSequence is StringCharSequence str)
                return text.Insert(index, str.Value);
            else if (charSequence is CharArrayCharSequence charArray)
                return text.Insert(index, charArray.Value);
            else if (charSequence is StringBuilderCharSequence sb)
                return text.Insert(index, sb.Value);
            else if (charSequence is StringBuffer stringBuffer)
                return text.Insert(index, stringBuffer.builder);

#if FEATURE_ARRAYPOOL
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
#else
            return text.Insert(index, charSequence.ToString()); // .NET 4.0 - don't care to optimize
#endif
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
        public static StringBuilder Insert(this StringBuilder text, int index, ICharSequence? charSequence, int startIndex, int charCount) // J2N TODO: API - extension method for StringBuilder
        {
            if (text is null)
                throw new ArgumentNullException(nameof(text));
            if (index < 0 || index > text.Length)
                throw new ArgumentOutOfRangeException(nameof(index));
            if (startIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(startIndex), SR.ArgumentOutOfRange_NeedNonNegNum);
            if (charCount < 0)
                throw new ArgumentOutOfRangeException(nameof(charCount), SR.ArgumentOutOfRange_NeedNonNegNum);
            if (charSequence is null && (startIndex != 0 || charCount != 0))
                throw new ArgumentNullException(nameof(charSequence)); // J2N: Unlike Java, we are throwing an exception (to match .NET Core 3) rather than writing "null" to the StringBuilder
            if (charSequence is null || !charSequence.HasValue)
                return text;
            if (startIndex > charSequence.Length - charCount) // Checks for int overflow
                throw new ArgumentOutOfRangeException(nameof(charCount), SR.ArgumentOutOfRange_IndexLength);

            if (charSequence is StringCharSequence stringCharSequence)
                return text.Insert(index, stringCharSequence.Value, startIndex, charCount);
            if (charSequence is CharArrayCharSequence charArrayCharSequence)
                return text.Insert(index, charArrayCharSequence.Value, startIndex, charCount);
            if (charSequence is StringBuilderCharSequence sbCharSequence)
                return text.Insert(index, sbCharSequence.Value, startIndex, charCount);
            if (charSequence is StringBuffer stringBuffer)
                return text.Insert(index, stringBuffer.builder, startIndex, charCount);

#if FEATURE_ARRAYPOOL
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
#else
            return Insert(text, index, charSequence.Subsequence(startIndex, charCount)); // .NET 4.0 - don't care to optimize
#endif
        }

        /// <summary>
        /// Inserts the string representation of a specified sequence of Unicode characters into this instance at the specified character position.
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
                throw new ArgumentNullException(nameof(text));
            if (index < 0 || index > text.Length)
                throw new ArgumentOutOfRangeException(nameof(index));
            if (charSequence is null)
                return text;

            // NOTE: This method will not be used for .NET Standard 2.1+ because
            // the overload already exists on StringBuilder.

#if FEATURE_ARRAYPOOL
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
#else
            return text.Insert(index, charSequence.ToString()); // .NET 4.0 - don't care to optimize.
#endif
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
                throw new ArgumentNullException(nameof(text));
            if (index < 0 || index > text.Length)
                throw new ArgumentOutOfRangeException(nameof(index));
            if (startIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(startIndex), SR.ArgumentOutOfRange_NeedNonNegNum);
            if (charCount < 0)
                throw new ArgumentOutOfRangeException(nameof(charCount), SR.ArgumentOutOfRange_NeedNonNegNum);
            if (charSequence is null && (startIndex != 0 || charCount != 0))
                throw new ArgumentNullException(nameof(charSequence)); // J2N: Unlike Java, we are throwing an exception (to match .NET Core 3) rather than writing "null" to the StringBuilder
            if (charSequence is null)
                return text;
            if (startIndex > charSequence.Length - charCount) // Checks for int overflow
                throw new ArgumentOutOfRangeException(nameof(charCount), SR.ArgumentOutOfRange_IndexLength);

            // J2N NOTE: We don't use Span<char> because this Insert() overload was added to StringBuilder prior to the CopyTo(int, Span<char> int) overload,
            // so this will never be called when Span<char> is supported unless called as a static method.

#if FEATURE_ARRAYPOOL
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
#else
            return text.Insert(index, charSequence.ToString(startIndex, charCount)); // .NET 4.0 - don't care to optimize
#endif
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
                throw new ArgumentNullException(nameof(text));
            if (index < 0 || index > text.Length)
                throw new ArgumentOutOfRangeException(nameof(index));
            if (startIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(startIndex), SR.ArgumentOutOfRange_NeedNonNegNum);
            if (charCount < 0)
                throw new ArgumentOutOfRangeException(nameof(charCount), SR.ArgumentOutOfRange_NeedNonNegNum);
            if (value is null && (startIndex != 0 || charCount != 0))
                throw new ArgumentNullException(nameof(value)); // J2N: Unlike Java, we are throwing an exception (to match .NET Core 3) rather than writing "null" to the StringBuilder
            if (value is null)
                return text;
            if (startIndex > value.Length - charCount) // Checks for int overflow
                throw new ArgumentOutOfRangeException(nameof(charCount), SR.ArgumentOutOfRange_IndexLength);

#if FEATURE_STRINGBUILDER_INSERT_READONLYSPAN
            return text.Insert(index, value.AsSpan(startIndex, charCount));
#elif FEATURE_ARRAYPOOL
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
#else
            return text.Insert(index, value.Substring(startIndex, charCount)); // .NET 4.0 - don't care to optimize
#endif
        }

#if FEATURE_SPAN
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
                throw new ArgumentNullException(nameof(text));
            if (index < 0 || index > text.Length)
                throw new ArgumentOutOfRangeException(nameof(index));
            if (charSequence.Length == 0)
                return text;

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
            return text;
        }
#endif

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
                throw new ArgumentNullException(nameof(text));
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index), string.Format(SR.ArgumentOutOfRange_Generic_MustBeNonNegative, nameof(index), index));
            if (!Character.IsValidCodePoint(codePoint))
                throw new ArgumentException(J2N.SR.Format(SR.Argument_InvalidCodePoint, codePoint));

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
        //        throw new ArgumentNullException(nameof(text));

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
        //        throw new ArgumentNullException(nameof(text));

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
                throw new ArgumentNullException(nameof(text));

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
                throw new ArgumentNullException(nameof(text));
            if (value is null)
                throw new ArgumentNullException(nameof(value));
            if (startIndex < 0 || startIndex > text.Length)
                throw new ArgumentOutOfRangeException(nameof(startIndex));

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

#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private static int LastIndexOfOrdinal(StringBuilder text, string value, int startIndex)
        {
            int textLength = text.Length;
#if FEATURE_STRINGBUILDER_COPYTO_SPAN // If this method isn't supported, we are buffering to an array pool to get to the stack, anyway.
            bool usePool = textLength > CharStackBufferSize;
            char[]? arrayToReturnToPool = usePool ? ArrayPool<char>.Shared.Rent(textLength) : null;
            try
#elif FEATURE_ARRAYPOOL
            char[] textChars = ArrayPool<char>.Shared.Rent(textLength);
            try
#else
            char[] textChars = new char[textLength];
#endif
            {
#if FEATURE_STRINGBUILDER_COPYTO_SPAN
                Span<char> textChars = usePool ? arrayToReturnToPool : stackalloc char[textLength];
                text.CopyTo(0, textChars, textLength);
#else
                text.CopyTo(0, textChars, 0, textLength);
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
#if FEATURE_STRINGBUILDER_COPYTO_SPAN
            finally
            {
                if (arrayToReturnToPool != null)
                    ArrayPool<char>.Shared.Return(arrayToReturnToPool);
            }
#elif FEATURE_ARRAYPOOL
            finally
            {
                ArrayPool<char>.Shared.Return(textChars);
            }
#endif
        }

#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private static int LastIndexOfOrdinalIgnoreCase(StringBuilder text, string value, int startIndex)
        {
            int textLength = text.Length;
#if FEATURE_STRINGBUILDER_COPYTO_SPAN // If this method isn't supported, we are buffering to an array pool to get to the stack, anyway.
            bool usePool = textLength > CharStackBufferSize;
            char[]? arrayToReturnToPool = usePool ? ArrayPool<char>.Shared.Rent(textLength) : null;
            try
#elif FEATURE_ARRAYPOOL
            char[] textChars = ArrayPool<char>.Shared.Rent(textLength);
            try
#else
            char[] textChars = new char[textLength];
#endif
            {
#if FEATURE_STRINGBUILDER_COPYTO_SPAN
                Span<char> textChars = usePool ? arrayToReturnToPool : stackalloc char[textLength];
                text.CopyTo(0, textChars, textLength);
#else
                text.CopyTo(0, textChars, 0, textLength);
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
#if FEATURE_STRINGBUILDER_COPYTO_SPAN
            finally
            {
                if (arrayToReturnToPool != null)
                    ArrayPool<char>.Shared.Return(arrayToReturnToPool);
            }
#elif FEATURE_ARRAYPOOL
            finally
            {
                ArrayPool<char>.Shared.Return(textChars);
            }
#endif
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
                throw new ArgumentNullException(nameof(text));
            if (newValue is null)
                throw new ArgumentNullException(nameof(newValue));
            if (startIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(startIndex), SR.ArgumentOutOfRange_NeedNonNegNum);
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), SR.ArgumentOutOfRange_NeedNonNegNum);
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
                    // replacing with more characters...need some room
                    text.Insert(startIndex, new char[-diff]);
                }
                var textIndexer = new ValueStringBuilderIndexer(text);
                try
                {
                    // copy the chars based on the new length
                    for (int i = 0; i < stringLength; i++)
                    {
                        textIndexer[i + startIndex] = newValue[i];
                    }
                }
                finally
                {
                    textIndexer.Dispose();
                }
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

#if FEATURE_SPAN
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
        /// and <see cref="J2N.Memory.MemoryExtensions.ReverseText(Span{char})"/> which
        /// don't require a <see cref="StringBuilder"/> instance.
        /// </summary>
        /// <param name="text">this <see cref="StringBuilder"/></param>
        /// <returns>A reference to this <see cref="StringBuilder"/>, for chaining.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="text"/> is <c>null</c>.</exception>
        /// <seealso cref="J2N.Text.StringExtensions.ReverseText(string)"/>
        /// <seealso cref="J2N.Memory.MemoryExtensions.ReverseText(Span{char})"/>
#else
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
        /// which doesn't require a <see cref="StringBuilder"/> instance.
        /// </summary>
        /// <param name="text">this <see cref="StringBuilder"/></param>
        /// <returns>A reference to this <see cref="StringBuilder"/>, for chaining.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="text"/> is <c>null</c>.</exception>
        /// <seealso cref="J2N.Text.StringExtensions.ReverseText(string)"/>
#endif
        public static StringBuilder Reverse(this StringBuilder text)
        {
            if (text is null)
                throw new ArgumentNullException(nameof(text));

            var forwardTextIndexer = new ValueStringBuilderIndexer(text, iterateForward: true);
            var reverseTextIndexer = new ValueStringBuilderIndexer(text, iterateForward: false);
            try
            {
                int count = text.Length;
                if (count == 0) return text;
                int start = 0;
                int end = count - 1;
                char startHigh = forwardTextIndexer[0];
                char endLow = reverseTextIndexer[end];
                bool allowStartSurrogate = true, allowEndSurrogate = true;
                while (start < end)
                {
                    char startLow = forwardTextIndexer[start + 1];
                    char endHigh = reverseTextIndexer[end - 1];
                    bool surrogateAtStart = allowStartSurrogate && startLow >= 0xdc00
                            && startLow <= 0xdfff && startHigh >= 0xd800
                            && startHigh <= 0xdbff;
                    if (surrogateAtStart && (count < 3))
                    {
                        return text;
                    }
                    bool surAtEnd = allowEndSurrogate && endHigh >= 0xd800
                            && endHigh <= 0xdbff && endLow >= 0xdc00
                            && endLow <= 0xdfff;
                    allowStartSurrogate = allowEndSurrogate = true;
                    if (surrogateAtStart == surAtEnd)
                    {
                        if (surrogateAtStart)
                        {
                            // both surrogates
                            reverseTextIndexer[end] = startLow;
                            reverseTextIndexer[end - 1] = startHigh;
                            forwardTextIndexer[start] = endHigh;
                            forwardTextIndexer[start + 1] = endLow;
                            startHigh = forwardTextIndexer[start + 2];
                            endLow = reverseTextIndexer[end - 2];
                            start++;
                            end--;
                        }
                        else
                        {
                            // neither surrogates
                            reverseTextIndexer[end] = startHigh;
                            forwardTextIndexer[start] = endLow;
                            startHigh = startLow;
                            endLow = endHigh;
                        }
                    }
                    else
                    {
                        if (surrogateAtStart)
                        {
                            // surrogate only at the front
                            reverseTextIndexer[end] = startLow;
                            forwardTextIndexer[start] = endLow;
                            endLow = endHigh;
                            allowStartSurrogate = false;
                        }
                        else
                        {
                            // surrogate only at the end
                            reverseTextIndexer[end] = startHigh;
                            forwardTextIndexer[start] = endHigh;
                            startHigh = startLow;
                            allowEndSurrogate = false;
                        }
                    }
                    start++;
                    end--;
                }
                if ((count & 1) == 1 && (!allowStartSurrogate || !allowEndSurrogate))
                {
                    reverseTextIndexer[end] = allowStartSurrogate ? endLow : startHigh;
                }
                return text;
            }
            finally
            {
                forwardTextIndexer.Dispose();
                reverseTextIndexer.Dispose();
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
                throw new ArgumentOutOfRangeException(nameof(startIndex), SR.ArgumentOutOfRange_NeedNonNegNum);
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length), SR.ArgumentOutOfRange_NeedNonNegNum);
            if (startIndex > text.Length - length) // Checks for int overflow
                throw new ArgumentOutOfRangeException(nameof(length), SR.ArgumentOutOfRange_IndexLength);

            return text.ToString(startIndex, length).AsCharSequence();
        }

        #endregion Subsequence
    }
}
