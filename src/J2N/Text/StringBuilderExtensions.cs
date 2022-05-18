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
            if (charSequence != null && charSequence.HasValue)
            {
                if (charSequence is StringCharSequence str)
                    text.Append(str.Value);
                else if (charSequence is CharArrayCharSequence charArray)
                    text.Append(charArray.Value);
                else if (charSequence is StringBuilderCharSequence sb)
                    text.Append(sb.Value);
                else
                    text.Append(charSequence.ToString());
            }
            return text;
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

            int end = startIndex + charCount;
            for (int i = startIndex; i < end; i++)
                text.Append(charSequence[i]);
            return text;
        }

        // J2N: Excluding this overload because it is effectively the same as Append(object)
        // and the compiler chooses that overload by default, anyway.
        ///// <summary>
        ///// Appends the given <see cref="StringBuilder"/> to this <see cref="StringBuilder"/>.
        ///// <para/>
        ///// The characters of the <paramref name="charSequence"/> argument are appended,
        ///// in order, to this sequence, increasing the
        ///// length of this sequence by the length of <paramref name="charSequence"/>.
        ///// <para/>
        ///// Usage Note: Unlike in Java, a <c>null</c> <paramref name="charSequence"/> won't append the string
        ///// <c>"null"</c> to the <see cref="StringBuilder"/>. Instead, it is a no-op.
        ///// </summary>
        ///// <param name="text">This <see cref="StringBuilder"/>.</param>
        ///// <param name="charSequence">The <see cref="StringBuilder"/> to append.</param>
        ///// <exception cref="ArgumentNullException">If <paramref name="text"/> is <c>null</c>.</exception>
        //public static StringBuilder Append(this StringBuilder text, StringBuilder charSequence)
        //{
        //    if (text == null)
        //        throw new ArgumentNullException(nameof(text));

        //    if (charSequence != null)
        //        text.Append(charSequence.ToString());
        //    return text;
        //}

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
            if (charSequence == null)
                return text;
            if (startIndex > charSequence.Length - charCount) // Checks for int overflow
                throw new ArgumentOutOfRangeException(nameof(charCount), SR.ArgumentOutOfRange_IndexLength);

            return text.Append(charSequence.ToString(startIndex, charCount));
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
        public static StringBuilder AppendCodePoint(this StringBuilder text, int codePoint)
        {
            if (text is null)
                throw new ArgumentNullException(nameof(text));

            text.Append(Character.ToChars(codePoint));
            return text;
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
            if (value is StringBuilderCharSequence && object.ReferenceEquals(text, value)) return 0;
            if (text is null) return (value is null || !value.HasValue) ? 0 : -1;
            if (value is null || !value.HasValue) return 1;

            int length = Math.Min(text.Length, value.Length);
            int result;
            for (int i = 0; i < length; i++)
            {
                if ((result = text[i] - value[i]) != 0)
                    return result;
            }

            // At this point, we have compared all the characters in at least one string.
            // The longer string will be larger.
            return text.Length - value.Length;
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
            int result;
            for (int i = 0; i < length; i++)
            {
                if ((result = text[i] - value[i]) != 0)
                    return result;
            }

            // At this point, we have compared all the characters in at least one string.
            // The longer string will be larger.
            return text.Length - value.Length;
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
            if (text is null) return -1;
            if (value is null) return 1;

            // Materialize the string. It is faster to loop through
            // a string than a StringBuilder.
            return text.ToString().CompareToOrdinal(value.ToString());
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
            int result;
            for (int i = 0; i < length; i++)
            {
                if ((result = text[i] - value[i]) != 0)
                    return result;
            }

            // At this point, we have compared all the characters in at least one string.
            // The longer string will be larger.
            return text.Length - value.Length;
        }

        #endregion CompareToOrdinal

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
        public static StringBuilder Delete(this StringBuilder? text, int startIndex, int count)
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

            return comparisonType switch
            {
                StringComparison.Ordinal => IndexOfOrdinal(text, value, startIndex),
                StringComparison.OrdinalIgnoreCase => IndexOfOrdinalIgnoreCase(text, value, startIndex),
                _ => text.ToString().IndexOf(value, startIndex, comparisonType),
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
            int maxSearchLength = (textLength - length) + 1;
            char firstChar = value[0];
            int index;
            for (int i = startIndex; i < maxSearchLength; ++i)
            {
                if (text[i] == firstChar)
                {
                    index = 1;
                    while ((index < length) && (text[i + index] == value[index]))
                        ++index;

                    if (index == length)
                        return i;
                }
            }
            return -1;
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
            int maxSearchLength = (textLength - length) + 1;
            char firstChar = value[0], c1, c2;
            var textInfo = CultureInfo.InvariantCulture.TextInfo;
            int index;
            for (int i = startIndex; i < maxSearchLength; ++i)
            {
                if (text[i] == firstChar)
                {
                    index = 1;
                    while ((index < length) &&
                        ((c1 = text[i + index]) == (c2 = value[index]) ||
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
            if (charSequence != null && charSequence.HasValue)
            {
                if (charSequence is StringCharSequence str)
                    text.Insert(index, str.Value);
                else if (charSequence is CharArrayCharSequence charArray)
                    text.Insert(index, charArray.Value);
                else if (charSequence is StringBuilderCharSequence sb)
                    text.Insert(index, sb.Value);
                else
                    text.Insert(index, charSequence.ToString());
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

            if (charSequence is CharArrayCharSequence charArrayCharSequence)
                return text.Insert(index, charArrayCharSequence.Value, startIndex, charCount);

            return Insert(text, index, charSequence.Subsequence(startIndex, charCount));
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
            return text.Insert(index, charSequence.ToString());
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

            return text.Insert(index, charSequence.ToString(startIndex, charCount));
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

            return text.Insert(index, value.Substring(startIndex, charCount));
        }

        #endregion

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

            return LastIndexOf(text, value, text.Length, comparisonType);
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

            return comparisonType switch
            {
                StringComparison.Ordinal => LastIndexOfOrdinal(text, value, startIndex),
                StringComparison.OrdinalIgnoreCase => LastIndexOfOrdinalIgnoreCase(text, value, startIndex),
                _ => text.ToString().LastIndexOf(value, startIndex, comparisonType),
            };
        }

#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif 
        private static int LastIndexOfOrdinal(StringBuilder text, string value, int startIndex)
        {
            int textLength = text.Length;
            int subCount = value.Length;

            if (subCount <= textLength && startIndex >= 0)
            {
                if (subCount > 0)
                {
                    if (startIndex > textLength - subCount)
                    {
                        startIndex = textLength - subCount; // count and subCount are both >= 1
                    }
                    // TODO optimize charAt to direct array access
                    char firstChar = value[0];
                    while (true)
                    {
                        int i = startIndex;
                        bool found = false;
                        for (; i >= 0; --i)
                        {
                            if (text[i] == firstChar)
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
                        while (++o2 < subCount && text[++o1] == value[o2])
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

#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif 
        private static int LastIndexOfOrdinalIgnoreCase(StringBuilder text, string value, int startIndex)
        {
            int textLength = text.Length;
            int subCount = value.Length;

            if (subCount <= textLength && startIndex >= 0)
            {
                if (subCount > 0)
                {
                    if (startIndex > textLength - subCount)
                    {
                        startIndex = textLength - subCount; // count and subCount are both >= 1
                    }
                    // TODO optimize charAt to direct array access
                    char firstChar = value[0], c1, c2;
                    var textInfo = CultureInfo.InvariantCulture.TextInfo;
                    while (true)
                    {
                        int i = startIndex;
                        bool found = false;
                        for (; i >= 0; --i)
                        {
                            if (text[i] == firstChar)
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
                            ((c1 = text[++o1]) == (c2 = value[o2]) ||
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
                // copy the chars based on the new length
                for (int i = 0; i < stringLength; i++)
                {
                    text[i + startIndex] = newValue[i];
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
        /// execution of the <see cref="Reverse"/> method. Then the
        /// character at index <c>k</c> in the new character sequence is
        /// equal to the character at index <c>n-k-1</c> in the old
        /// character sequence.
        /// <para/>
        /// Note that the reverse operation may result in producing
        /// surrogate pairs that were unpaired low-surrogates and
        /// high-surrogates before the operation. For example, reversing
        /// "&#92;uDC00&#92;uD800" produces "&#92;uD800&#92;uDC00" which is
        /// a valid surrogate pair.
        /// </summary>
        /// <param name="text">this <see cref="StringBuilder"/></param>
        /// <returns>A reference to this <see cref="StringBuilder"/>, for chaining.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="text"/> is <c>null</c>.</exception>
        public static StringBuilder Reverse(this StringBuilder text)
        {
            if (text is null)
                throw new ArgumentNullException(nameof(text));

            bool hasSurrogate = false;
            int n = text.Length - 1;
            // Tradeoff: materializing the string is generally faster,
            // but also requires additional RAM. So, if the string is going
            // to take up more than 16KB, we index into the StringBuilder
            // rather than a string.
            bool materializeString = text.Length <= 16384;
            string? readOnlyText = materializeString ? text.ToString() : null;
            for (int j = (n - 1) >> 1; j >= 0; --j)
            {
                char temp = materializeString ? readOnlyText![j] : text[j];
                char temp2 = materializeString ? readOnlyText![n - j] : text[n - j];
                if (!hasSurrogate)
                {
                    hasSurrogate = (temp >= Character.MinSurrogate && temp <= Character.MaxSurrogate)
                        || (temp2 >= Character.MinSurrogate && temp2 <= Character.MaxSurrogate);
                }
                text[j] = temp2;
                text[n - j] = temp;
            }
            if (hasSurrogate)
            {
                readOnlyText = materializeString ? text.ToString() : null;
                // Reverse back all valid surrogate pairs
                for (int i = 0; i < text.Length - 1; i++)
                {
                    char c2 = materializeString ? readOnlyText![i] : text[i];
                    if (char.IsLowSurrogate(c2))
                    {
                        char c1 = materializeString ? readOnlyText![i + 1] : text[i];
                        if (char.IsHighSurrogate(c1))
                        {
                            text[i++] = c1;
                            text[i] = c2;
                        }
                    }
                }
            }

            return text;
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
