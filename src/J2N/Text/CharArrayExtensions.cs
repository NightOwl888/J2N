using System;
using System.Text;

namespace J2N.Text
{
    /// <summary>
    /// Extensions to <see cref="T:char[]"/> arrays.
    /// </summary>
    public static class CharArrayExtensions
    {
        /// <summary>
        /// Convenience method to wrap a string in a <see cref="CharArrayCharSequence"/>
        /// so a <see cref="T:char[]"/> can be used as <see cref="ICharSequence"/> in .NET.
        /// </summary>
        /// <param name="text">This <see cref="T:char[]"/>.</param>
        public static ICharSequence AsCharSequence(this char[] text)
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
        public static ICharSequence Subsequence(this char[] text, int startIndex, int length)
        {
            // From Apache Harmony String class
            if (text == null || (startIndex == 0 && length == text.Length))
            {
                return text.AsCharSequence();
            }
            if (startIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(startIndex));
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length));
            if (startIndex + length > text.Length)
                throw new ArgumentOutOfRangeException("", $"{nameof(startIndex)} + {nameof(length)} > {nameof(text.Length)}");

            char[] result = new char[length];
            for (int i = 0, j = startIndex; i < length; i++, j++)
                result[i] = text[j];

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
        public static int CompareToOrdinal(this char[] str, char[] value)
        {
            if (object.ReferenceEquals(str, value)) return 0;
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
        public static int CompareToOrdinal(this char[] str, StringBuilder value)
        {
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
        public static int CompareToOrdinal(this char[] str, string value)
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
        public static int CompareToOrdinal(this char[] str, ICharSequence value)
        {
            if (value is CharArrayCharSequence && object.ReferenceEquals(str, value)) return 0;
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
