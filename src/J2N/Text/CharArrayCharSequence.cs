using System;
using System.Text;

namespace J2N.Text
{
    /// <summary>
    /// A wrapper struct that represents a <see cref="T:char[]"/> and implements <see cref="ICharSequence"/>.
    /// </summary>
    public struct CharArrayCharSequence : ICharSequence,
        IComparable<ICharSequence>, IComparable,
        IComparable<string>, IComparable<StringBuilder>, IComparable<char[]>,
        IEquatable<ICharSequence>,
        IEquatable<CharArrayCharSequence>, IEquatable<StringBuilderCharSequence>, IEquatable<StringCharSequence>,
        IEquatable<string>, IEquatable<StringBuilder>, IEquatable<char[]>
    {
        /// <summary>
        /// Initializes a new instance of <see cref="CharArrayCharSequence"/> with the provided <paramref name="value"/>.
        /// </summary>
        /// <param name="value">A <see cref="T:char[]"/> to wrap in a <see cref="ICharSequence"/>. The value may be <c>null</c>.</param>
        public CharArrayCharSequence(char[] value)
        {
            this.Value = value;
            this.HasValue = (value != null);
        }

        /// <summary>
        /// Gets the current <see cref="T:char[]"/> value.
        /// </summary>
        public char[] Value { get; }

        #region ICharSequence Members

        /// <summary>
        /// Gets a value indicating whether the current <see cref="CharArrayCharSequence"/>
        /// has a valid <see cref="T:char[]"/> value.
        /// </summary>
        public bool HasValue { get; }

        /// <summary>
        /// Gets the character at the specified index, with the first character
        /// having index zero.
        /// </summary>
        /// <param name="index">The index of the character to return.</param>
        /// <returns>The requested character.</returns>
        /// <exception cref="System.IndexOutOfRangeException">
        /// If <c>index &lt; 0</c> or <c>index</c> is greater than the
        /// length of this sequence.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// If the underlying value of this sequence is <c>null</c>.
        /// </exception>
        public char this[int index]
        {
            get
            {
                if (Value == null)
                    throw new InvalidOperationException($"Cannot index a null {nameof(CharArrayCharSequence)}.");
                return Value[index];
            }
        }

        /// <summary>
        /// Gets the number of characters in this sequence.
        /// </summary>
        public int Length
        {
            get { return (Value == null) ? 0 : Value.Length; }
        }

        /// <summary>
        /// Retrieves a sub-sequence from this instance.
        /// The sub-sequence starts at a specified character position and has a specified length.
        /// <para/>
        /// IMPORTANT: This method has .NET semantics, that is, the second parameter is a length,
        /// not an exclusive end index as it would be in Java. To translate from Java to .NET,
        /// callers must account for this by subtracting (end - start) for the <paramref name="length"/>.
        /// </summary>
        /// <param name="startIndex">
        /// The start index of the sub-sequence. It is inclusive, that
        /// is, the index of the first character that is included in the
        /// sub-sequence.
        /// </param>
        /// <param name="length">The number of characters to return in the sub-sequence.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startIndex"/> plus <paramref name="length"/> indicates a position not within this instance.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="startIndex"/> or <paramref name="length"/> is less than zero.
        /// </exception>
        public ICharSequence Subsequence(int startIndex, int length)
        {
            // From Apache Harmony String class
            if (Value == null || (startIndex == 0 && length == Value.Length))
            {
                return new CharArrayCharSequence(Value);
            }
            if (startIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(startIndex));
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length));
            if (startIndex + length > Value.Length)
                throw new ArgumentOutOfRangeException("", $"{nameof(startIndex)} + {nameof(length)} > {nameof(Length)}");

            char[] result = new char[length];
            for (int i = 0, j = startIndex; i < length; i++, j++)
                result[i] = Value[j];

            return new CharArrayCharSequence(result);
        }

        /// <summary>
        /// Returns a string with the same characters in the same order as in this
        /// sequence.
        /// </summary>
        /// <returns>A string based on this sequence.</returns>
        public override string ToString()
        {
            return (Value == null) ? string.Empty : new string(Value);
        }

        #endregion

        #region Operator Overloads

        /// <summary>
        /// Compares <paramref name="csq1"/> and <paramref name="csq2"/> for reference equality.
        /// </summary>
        /// <param name="csq1">The first sequence.</param>
        /// <param name="csq2">The second sequence.</param>
        /// <returns><c>true</c> if <paramref name="csq1"/> and <paramref name="csq2"/> represent to the same instance; otherwise, <c>false</c>.</returns>
        public static bool operator ==(CharArrayCharSequence csq1, CharArrayCharSequence csq2)
        {
            return csq1.Value == csq2.Value;
        }

        /// <summary>
        /// Compares <paramref name="csq1"/> and <paramref name="csq2"/> for reference inequality.
        /// </summary>
        /// <param name="csq1">The first sequence.</param>
        /// <param name="csq2">The second sequence.</param>
        /// <returns><c>true</c> if <paramref name="csq1"/> and <paramref name="csq2"/> do not represent to the same instance; otherwise, <c>false</c>.</returns>
        public static bool operator !=(CharArrayCharSequence csq1, CharArrayCharSequence csq2)
        {
            return csq1.Value != csq2.Value;
        }

        /// <summary>
        /// Compares <paramref name="csq1"/> and <paramref name="csq2"/> for reference equality.
        /// </summary>
        /// <param name="csq1">The first sequence.</param>
        /// <param name="csq2">The second sequence.</param>
        /// <returns><c>true</c> if <paramref name="csq1"/> and <paramref name="csq2"/> represent to the same instance; otherwise, <c>false</c>.</returns>
        public static bool operator ==(CharArrayCharSequence csq1, char[] csq2)
        {
            return csq1.Value == csq2;
        }

        /// <summary>
        /// Compares <paramref name="csq1"/> and <paramref name="csq2"/> for reference inequality.
        /// </summary>
        /// <param name="csq1">The first sequence.</param>
        /// <param name="csq2">The second sequence.</param>
        /// <returns><c>true</c> if <paramref name="csq1"/> and <paramref name="csq2"/> do not represent to the same instance; otherwise, <c>false</c>.</returns>
        public static bool operator !=(CharArrayCharSequence csq1, char[] csq2)
        {
            return csq1.Value != csq2;
        }

        /// <summary>
        /// Compares <paramref name="csq1"/> and <paramref name="csq2"/> for reference equality.
        /// </summary>
        /// <param name="csq1">The first sequence.</param>
        /// <param name="csq2">The second sequence.</param>
        /// <returns><c>true</c> if <paramref name="csq1"/> and <paramref name="csq2"/> represent to the same instance; otherwise, <c>false</c>.</returns>
        public static bool operator ==(char[] csq1, CharArrayCharSequence csq2)
        {
            return csq1 == csq2.Value;
        }

        /// <summary>
        /// Compares <paramref name="csq1"/> and <paramref name="csq2"/> for reference inequality.
        /// </summary>
        /// <param name="csq1">The first sequence.</param>
        /// <param name="csq2">The second sequence.</param>
        /// <returns><c>true</c> if <paramref name="csq1"/> and <paramref name="csq2"/> do not represent to the same instance; otherwise, <c>false</c>.</returns>
        public static bool operator !=(char[] csq1, CharArrayCharSequence csq2)
        {
            return csq1 != csq2.Value;
        }

        #endregion

        #region Equality Comparison

        /// <summary>
        /// Determines whether this <see cref="CharArrayCharSequence"/> is equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="other">An <see cref="ICharSequence"/> to compare to the current <see cref="CharArrayCharSequence"/>.</param>
        /// <returns><c>true</c> if <paramref name="other"/> is equal to the current <see cref="CharArrayCharSequence"/>; otherwise, <c>false</c>.</returns>
        public bool Equals(ICharSequence other)
        {
            if (this.Value == null)
                return other == null || !other.HasValue;

            int len = this.Value.Length;
            if (len != other.Length) return false;
            for (int i = 0; i < len; i++)
            {
                if (!this.Value[i].Equals(other[i])) return false;
            }
            return true;
        }

        /// <summary>
        /// Determines whether this <see cref="CharArrayCharSequence"/> is equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="other">A <see cref="CharArrayCharSequence"/> to compare to the current <see cref="CharArrayCharSequence"/>.</param>
        /// <returns><c>true</c> if <paramref name="other"/> is equal to the current <see cref="CharArrayCharSequence"/>; otherwise, <c>false</c>.</returns>
        public bool Equals(CharArrayCharSequence other)
        {
            return this.Equals(other.Value);
        }

        /// <summary>
        /// Determines whether this <see cref="CharArrayCharSequence"/> is equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="other">A <see cref="StringBuilderCharSequence"/> to compare to the current <see cref="CharArrayCharSequence"/>.</param>
        /// <returns><c>true</c> if <paramref name="other"/> is equal to the current <see cref="CharArrayCharSequence"/>; otherwise, <c>false</c>.</returns>
        public bool Equals(StringBuilderCharSequence other)
        {
            return this.Equals(other.Value);
        }

        /// <summary>
        /// Determines whether this <see cref="CharArrayCharSequence"/> is equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="other">A <see cref="StringCharSequence"/> to compare to the current <see cref="CharArrayCharSequence"/>.</param>
        /// <returns><c>true</c> if <paramref name="other"/> is equal to the current <see cref="CharArrayCharSequence"/>; otherwise, <c>false</c>.</returns>
        public bool Equals(StringCharSequence other)
        {
            return this.Equals(other.Value);
        }

        /// <summary>
        /// Determines whether this <see cref="CharArrayCharSequence"/> is equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="other">A <see cref="string"/> to compare to the current <see cref="CharArrayCharSequence"/>.</param>
        /// <returns><c>true</c> if <paramref name="other"/> is equal to the current <see cref="CharArrayCharSequence"/>; otherwise, <c>false</c>.</returns>
        public bool Equals(string other)
        {
            if (this.Value == null)
                return other == null;

            int len = this.Value.Length;
            if (len != other.Length) return false;
            for (int i = 0; i < len; i++)
            {
                if (!this.Value[i].Equals(other[i])) return false;
            }
            return true;
        }

        /// <summary>
        /// Determines whether this <see cref="CharArrayCharSequence"/> is equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="other">A <see cref="StringBuilder"/> to compare to the current <see cref="CharArrayCharSequence"/>.</param>
        /// <returns><c>true</c> if <paramref name="other"/> is equal to the current <see cref="CharArrayCharSequence"/>; otherwise, <c>false</c>.</returns>
        public bool Equals(StringBuilder other)
        {
            if (this.Value == null)
                return other == null;

            // NOTE: This benchmarked to be faster than looping through the StringBuilder
            string temp = other.ToString();

            int len = this.Value.Length;
            if (len != temp.Length) return false;
            for (int i = 0; i < len; i++)
            {
                if (!this.Value[i].Equals(temp[i])) return false;
            }
            return true;
        }

        /// <summary>
        /// Determines whether this <see cref="CharArrayCharSequence"/> is equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="other">A <see cref="T:char[]"/> to compare to the current <see cref="CharArrayCharSequence"/>.</param>
        /// <returns><c>true</c> if <paramref name="other"/> is equal to the current <see cref="CharArrayCharSequence"/>; otherwise, <c>false</c>.</returns>
        public bool Equals(char[] other)
        {
            if (this.Value == null)
                return other == null;

            int len = this.Value.Length;
            if (len != other.Length) return false;
            for (int i = 0; i < len; i++)
            {
                if (!this.Value[i].Equals(other[i])) return false;
            }
            return true;
        }

        /// <summary>
        /// Determines whether this <see cref="CharArrayCharSequence"/> is equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="other">An object to compare to the current <see cref="CharArrayCharSequence"/>.</param>
        /// <returns><c>true</c> if <paramref name="other"/> is equal to the current <see cref="CharArrayCharSequence"/>; otherwise, <c>false</c>.</returns>
        public override bool Equals(object other)
        {
            if (other is string)
                return Equals(other as string);
            else if (other is StringBuilder)
                return Equals(other as StringBuilder);
            else if (other is char[])
                return Equals(other as char[]);
            else if (other is StringCharSequence)
                return Equals((StringCharSequence)other);
            else if (other is CharArrayCharSequence)
                return Equals((CharArrayCharSequence)other);
            else if (other is StringBuilderCharSequence)
                return Equals((StringBuilderCharSequence)other);
            else if (other is ICharSequence)
                return Equals((ICharSequence)other);

            return false;
        }

        /// <summary>
        /// Gets the hash code for the current <see cref="ICharSequence"/>.
        /// </summary>
        /// <returns>Returns the hash code for <see cref="Value"/>. If <see cref="Value"/> is <c>null</c>, returns <see cref="int.MaxValue"/>.</returns>
        public override int GetHashCode()
        {
            // NOTE: For consistency, we match all char sequences to the same
            // hash code. This unfortunately means it won't match
            // against String, StringBuilder or char[]. But that only matters
            // if the types are put into the same hashtable.
            return CharSequenceComparer.Ordinal.GetHashCode(this.Value);
        }

        #endregion

        #region IComparable Members

        /// <summary>
        /// Compares this instance with a specified <see cref="ICharSequence"/> object and indicates whether
        /// this instance precedes, follows, or appears in the same position in the sort order as the specified string.
        /// </summary>
        /// <param name="other">The <see cref="ICharSequence"/> to compare with this instance.</param>
        /// <returns>
        /// An integer that indicates the lexical relationship between the two comparands.
        /// Less than zero indicates the comparison value is greater than the current string.
        /// Zero indicates the strings are equal.
        /// Greater than zero indicates the comparison value is less than the current string.
        /// </returns>
        public int CompareTo(ICharSequence other)
        {
            return this.Value.CompareToOrdinal(other);
        }

        /// <summary>
        /// Compares this instance with a specified <see cref="string"/> object and indicates whether
        /// this instance precedes, follows, or appears in the same position in the sort order as the specified string.
        /// </summary>
        /// <param name="other">The <see cref="string"/> to compare with this instance.</param>
        /// <returns>
        /// An integer that indicates the lexical relationship between the two comparands.
        /// Less than zero indicates the comparison value is greater than the current string.
        /// Zero indicates the strings are equal.
        /// Greater than zero indicates the comparison value is less than the current string.
        /// </returns>
        public int CompareTo(string other)
        {
            return this.Value.CompareToOrdinal(other);
        }

        /// <summary>
        /// Compares this instance with a specified <see cref="StringBuilder"/> object and indicates whether
        /// this instance precedes, follows, or appears in the same position in the sort order as the specified string.
        /// </summary>
        /// <param name="other">The <see cref="StringBuilder"/> to compare with this instance.</param>
        /// <returns>
        /// An integer that indicates the lexical relationship between the two comparands.
        /// Less than zero indicates the comparison value is greater than the current string.
        /// Zero indicates the strings are equal.
        /// Greater than zero indicates the comparison value is less than the current string.
        /// </returns>
        public int CompareTo(StringBuilder other)
        {
            return this.Value.CompareToOrdinal(other);
        }

        /// <summary>
        /// Compares this instance with a specified <see cref="T:char[]"/> object and indicates whether
        /// this instance precedes, follows, or appears in the same position in the sort order as the specified string.
        /// </summary>
        /// <param name="other">The <see cref="T:char[]"/> to compare with this instance.</param>
        /// <returns>
        /// An integer that indicates the lexical relationship between the two comparands.
        /// Less than zero indicates the comparison value is greater than the current string.
        /// Zero indicates the strings are equal.
        /// Greater than zero indicates the comparison value is less than the current string.
        /// </returns>
        public int CompareTo(char[] other)
        {
            return this.Value.CompareToOrdinal(other);
        }

        /// <summary>
        /// Compares this instance with a specified <see cref="object"/> and indicates whether
        /// this instance precedes, follows, or appears in the same position in the sort order as the specified string.
        /// </summary>
        /// <param name="other">The <see cref="object"/> to compare with this instance.</param>
        /// <returns>
        /// An integer that indicates the lexical relationship between the two comparands.
        /// Less than zero indicates the comparison value is greater than the current string.
        /// Zero indicates the strings are equal.
        /// Greater than zero indicates the comparison value is less than the current string.
        /// </returns>
        public int CompareTo(object other)
        {
            if (this.Value == null) return -1;
            if (other == null) return 1;

            return this.Value.CompareToOrdinal(other.ToString());
        }

        #endregion
    }
}
