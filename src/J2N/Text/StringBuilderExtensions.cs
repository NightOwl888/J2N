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
        /// Convenience method to wrap a string in a <see cref="StringBuilderCharSequence"/>
        /// so a <see cref="StringBuilder"/> can be used as <see cref="ICharSequence"/> in .NET.
        /// </summary>
        public static ICharSequence ToCharSequence(this StringBuilder text)
        {
            return new StringBuilderCharSequence(text);
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
        public static int CompareToOrdinal(this StringBuilder str, char[] value)
        {
            if (str == null) return -1;
            if (value == null) return 1;

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
        public static int CompareToOrdinal(this StringBuilder str, StringBuilder value)
        {
            if (object.ReferenceEquals(str, value)) return 0;
            if (str == null) return -1;
            if (value == null) return 1;

            // Materialize the string. It is faster to loop through
            // a string than a StringBuilder.
            string temp = value.ToString();

            int length = Math.Min(str.Length, temp.Length);
            int result;
            for (int i = 0; i < length; i++)
            {
                if ((result = str[i] - temp[i]) != 0)
                    return result;
            }

            // At this point, we have compared all the characters in at least one string.
            // The longer string will be larger.
            return str.Length - temp.Length;
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
        public static int CompareToOrdinal(this StringBuilder str, string value)
        {
            if (str == null) return -1;
            if (value == null) return 1;

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
        public static int CompareToOrdinal(this StringBuilder str, ICharSequence value)
        {
            if (value is StringBuilderCharSequence && object.ReferenceEquals(str, value)) return 0;
            if (str == null) return -1;
            if (value == null) return 1;
            if (!value.HasValue) return 1;

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
    }
}
