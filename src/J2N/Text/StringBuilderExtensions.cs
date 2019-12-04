using System;
using System.Text;

namespace J2N.Text
{
    /// <summary>
    /// Extensions to the <see cref="StringBuilder"/> class.
    /// </summary>
    public static class StringBuilderExtensions
    {
        // This doesn't work right. See: https://stackoverflow.com/a/26885473
        // However, we can fallback on the Append(object) overload and it will call ToString().
        /// <summary>
        /// Appends the given <see cref="ICharSequence"/> to this <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="text">This <see cref="StringBuilder"/>.</param>
        /// <param name="charSequence">The <see cref="ICharSequence"/> to append.</param>
        /// <returns>This <see cref="StringBuilder"/>, for chaining.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="text"/> is <c>null</c>.</exception>
        public static StringBuilder Append(this StringBuilder text, ICharSequence charSequence)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));

            // For null values, this is a no-op
            if (charSequence != null && charSequence.HasValue)
            {
                if (charSequence is StringCharSequence)
                    text.Append(((StringCharSequence)charSequence).Value);
                else if (charSequence is CharArrayCharSequence)
                    text.Append(((CharArrayCharSequence)charSequence).Value);
                else // We don't need to check for StringBuilderCharSequence because we call ToString() on it anyway
                    text.Append(charSequence.ToString());
            }
            return text;
        }

        /// <summary>
        /// Appends the specified range of characters of the given <see cref="ICharSequence"/> to this <see cref="StringBuilder"/>.
        /// <para/>
        /// IMPORTANT: This method has .NET semantics, that is, the second parameter is a length,
        /// not an exclusive end index as it would be in Java. To translate from Java to .NET,
        /// callers must account for this by subtracting (end - start) for the <paramref name="count"/>.
        /// </summary>
        /// <param name="text">This <see cref="StringBuilder"/>.</param>
        /// <param name="charSequence">The <see cref="ICharSequence"/> to append.</param>
        /// <param name="startIndex">The starting position of the substring within <paramref name="charSequence"/>.</param>
        /// <param name="count">The number of characters in <paramref name="charSequence"/> to append.</param>
        /// <returns>This <see cref="StringBuilder"/>, for chaining.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="text"/> is <c>null</c>.</exception>
        public static StringBuilder Append(this StringBuilder text, ICharSequence charSequence, int startIndex, int count)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));

            if (startIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(startIndex));
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            // For null values, this is a no-op
            if (charSequence == null)
            {
                //text.Append("null"); // Java behavior
                return text;
            }

            int end = startIndex + count;
            for (int i = startIndex; i < end; i++)
                text.Append(charSequence[i]);
            return text;
        }

        /// <summary>
        /// Appends the string representation of the <paramref name="codePoint"/>
        /// argument to this sequence.
        /// 
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
        /// <param name="codePoint">a Unicode code point</param>
        /// <returns>a reference to this object.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="text"/> is <c>null</c>.</exception>
        public static StringBuilder AppendCodePoint(this StringBuilder text, int codePoint)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));

            text.Append(Character.ToChars(codePoint));
            return text;
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
        public static int CompareToOrdinal(this StringBuilder text, ICharSequence value)
        {
            if (value is StringBuilderCharSequence && object.ReferenceEquals(text, value)) return 0;
            if (text == null) return -1;
            if (value == null) return 1;
            if (!value.HasValue) return 1;

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
        public static int CompareToOrdinal(this StringBuilder text, char[] value)
        {
            if (text == null) return -1;
            if (value == null) return 1;

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
        public static int CompareToOrdinal(this StringBuilder text, StringBuilder value)
        {
            if (object.ReferenceEquals(text, value)) return 0;
            if (text == null) return -1;
            if (value == null) return 1;

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
        public static int CompareToOrdinal(this StringBuilder text, string value)
        {
            if (text == null) return -1;
            if (value == null) return 1;

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
        /// Searches for the first index of the specified character. The search for
        /// the character starts at the beginning and moves towards the end.
        /// </summary>
        /// <param name="text">This <see cref="StringBuilder"/>.</param>
        /// <param name="value">The string to find.</param>
        /// <returns>The index of the specified character, or <c>-1</c> if the character isn't found.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="text"/> or <paramref name="value"/> is <c>null</c>.</exception>
        public static int IndexOf(this StringBuilder text, string value)
        {
            return IndexOf(text, value, 0, StringComparison.Ordinal);
        }

        /// <summary>
        /// Searches for the index of the specified character. The search for the
        /// character starts at the specified offset and moves towards the end.
        /// </summary>
        /// <param name="text">This <see cref="StringBuilder"/>.</param>
        /// <param name="value">The string to find.</param>
        /// <param name="startIndex">The starting offset.</param>
        /// <returns>The index of the specified character, or <c>-1</c> if the character isn't found.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="text"/> or <paramref name="value"/> is <c>null</c>.</exception>
        public static int IndexOf(this StringBuilder text, string value, int startIndex)
        {
            return IndexOf(text, value, startIndex, StringComparison.Ordinal);
        }

        /// <summary>
        /// Searches for the index of the specified character. The search for the
        /// character starts at the specified offset and moves towards the end.
        /// </summary>
        /// <param name="text">This <see cref="StringBuilder"/>.</param>
        /// <param name="value">The string to find.</param>
        /// <param name="startIndex">The starting offset.</param>
        /// <param name="comparisonType"></param>
        /// <returns>The index of the specified character, or <c>-1</c> if the character isn't found.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="text"/> or <paramref name="value"/> is <c>null</c>.</exception>
        public static int IndexOf(this StringBuilder text, string value, int startIndex, StringComparison comparisonType)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            int length = value.Length;
            if (length == 0)
                return 0;
            int textLength = text.Length;

            // Tradeoff: materializing the string is generally faster,
            // but also requires additional RAM. So, if the string is going
            // to take up more than 16KB, we index into the StringBuilder
            // rather than a string.
            if (textLength <= 16384)
                return text.ToString().IndexOf(value, comparisonType);

            int maxSearchLength = (textLength - length) + 1;

            if (comparisonType == StringComparison.Ordinal)
            {
                int index;
                for (int i = startIndex; i < maxSearchLength; ++i)
                {
                    if (text[i] == value[0])
                    {
                        index = 1;
                        while ((index < length) && (text[i + index] == value[index]))
                            ++index;

                        if (index == length)
                            return i;
                    }
                }
            }
            else
            {
                for (int i = startIndex; i < maxSearchLength; ++i)
                {
                    if (text[i] == value[0] && text.ToString(i, length).Equals(value, comparisonType))
                        return i;
                }
            }

            return -1;
        }

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
        public static StringBuilder Reverse(this StringBuilder text)
        {
            bool hasSurrogate = false;
            int n = text.Length - 1;
            // Tradeoff: materializing the string is generally faster,
            // but also requires additional RAM. So, if the string is going
            // to take up more than 16KB, we index into the StringBuilder
            // rather than a string.
            bool materializeString = text.Length <= 16384;
            string readOnlyText = materializeString ? text.ToString() : null;
            for (int j = (n - 1) >> 1; j >= 0; --j)
            {
                char temp = materializeString ? readOnlyText[j] : text[j];
                char temp2 = materializeString ? readOnlyText[n - j] : text[n - j];
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
                    char c2 = materializeString ? readOnlyText[i] : text[i];
                    if (char.IsLowSurrogate(c2))
                    {
                        char c1 = materializeString ? readOnlyText[i + 1] : text[i];
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
        public static ICharSequence Subsequence(this StringBuilder text, int startIndex, int length)
        {
            // From Apache Harmony String class
            if (text == null || (startIndex == 0 && length == text.Length))
            {
                return text.ToCharSequence();
            }
            if (startIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(startIndex));
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length));
            if (startIndex + length > text.Length)
                throw new ArgumentOutOfRangeException("", $"{nameof(startIndex)} + {nameof(length)} > {nameof(text.Length)}");

            return text.ToString(startIndex, length).ToCharSequence();
        }

        /// <summary>
        /// Convenience method to wrap a string in a <see cref="StringBuilderCharSequence"/>
        /// so a <see cref="StringBuilder"/> can be used as <see cref="ICharSequence"/> in .NET.
        /// </summary>
        public static ICharSequence ToCharSequence(this StringBuilder text)
        {
            return new StringBuilderCharSequence(text);
        }
    }
}
