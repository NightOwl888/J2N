#region Copyright 2019-2021 by Shad Storhaug, Licensed under the Apache License, Version 2.0
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
using System.Text;

namespace J2N.Text
{
    /// <summary>
    /// A wrapper class that represents a <see cref="string"/> and implements <see cref="ICharSequence"/>.
    /// </summary>
    public class StringCharSequence : ICharSequence, 
        IComparable<ICharSequence>, IComparable,
        IComparable<string>, IComparable<StringBuilder>, IComparable<char[]>,
        IEquatable<ICharSequence>,
        IEquatable<CharArrayCharSequence>, IEquatable<StringBuilderCharSequence>, IEquatable<StringCharSequence>, 
        IEquatable<string>, IEquatable<StringBuilder>, IEquatable<char[]>
    {
        private const int CharStackBufferSize = 64;

        /// <summary>
        /// Initializes a new instance of <see cref="StringCharSequence"/> with the provided <paramref name="value"/>.
        /// </summary>
        /// <param name="value">A <see cref="T:char[]"/> to wrap in a <see cref="ICharSequence"/>. The value may be <c>null</c>.</param>
        public StringCharSequence(string? value)
        {
            this.Value = value;
            this.HasValue = (value != null);
        }

        /// <summary>
        /// Gets the current <see cref="string"/> value.
        /// </summary>
        public string? Value { get; }

        #region ICharSequence Members

        /// <summary>
        /// Gets a value indicating whether the current <see cref="StringCharSequence"/>
        /// has a valid <see cref="string"/> value.
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
                if (Value is null)
                    throw new InvalidOperationException(J2N.SR.Format(SR.InvalidOperation_CannotIndexNullObject, nameof(StringCharSequence)));
                return Value[index];
            }
        }

        /// <summary>
        /// Gets the number of characters in this sequence.
        /// </summary>
        public int Length
        {
            get { return (Value is null) ? 0 : Value.Length; }
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
            if (Value is null || (startIndex == 0 && length == Value.Length))
            {
                return new StringCharSequence(Value);
            }
            if (startIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(startIndex), SR.ArgumentOutOfRange_NeedNonNegNum);
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length), SR.ArgumentOutOfRange_NeedNonNegNum);
            if (startIndex > Value.Length - length) // Checks for int overflow
                throw new ArgumentOutOfRangeException(nameof(length), SR.ArgumentOutOfRange_IndexLength);

            return new StringCharSequence(Value.Substring(startIndex, length));
        }

        /// <summary>
        /// Returns a string with the same characters in the same order as in this
        /// sequence.
        /// </summary>
        /// <returns>A string based on this sequence.</returns>
        public override string ToString()
        {
            return Value ?? string.Empty;
        }

        #endregion

        #region Operator Overloads

        /// <summary>
        /// Compares <paramref name="csq1"/> and <paramref name="csq2"/> for equality.
        /// Two character sequences are considered equal if they have the same characters
        /// in the same order.
        /// </summary>
        /// <param name="csq1">The first sequence.</param>
        /// <param name="csq2">The second sequence.</param>
        /// <returns><c>true</c> if <paramref name="csq1"/> and <paramref name="csq2"/> represent to the same instance; otherwise, <c>false</c>.</returns>
        public static bool operator ==(StringCharSequence? csq1, StringCharSequence? csq2)
        {
            if (csq1 is null || !csq1.HasValue)
                return csq2 is null || !csq2.HasValue;
            else if (csq2 is null || !csq2.HasValue)
                return false;

            return csq1.Equals(csq2);
        }

        /// <summary>
        /// Compares <paramref name="csq1"/> and <paramref name="csq2"/> for inequality.
        /// Two character sequences are considered equal if they have the same characters
        /// in the same order.
        /// </summary>
        /// <param name="csq1">The first sequence.</param>
        /// <param name="csq2">The second sequence.</param>
        /// <returns><c>true</c> if <paramref name="csq1"/> and <paramref name="csq2"/> do not represent to the same instance; otherwise, <c>false</c>.</returns>
        public static bool operator !=(StringCharSequence? csq1, StringCharSequence? csq2)
        {
            return !(csq1 == csq2);
        }

        /// <summary>
        /// Compares <paramref name="csq1"/> and <paramref name="csq2"/> for equality.
        /// Two character sequences are considered equal if they have the same characters
        /// in the same order.
        /// </summary>
        /// <param name="csq1">The first sequence.</param>
        /// <param name="csq2">The second sequence.</param>
        /// <returns><c>true</c> if <paramref name="csq1"/> and <paramref name="csq2"/> represent to the same instance; otherwise, <c>false</c>.</returns>
        public static bool operator ==(StringCharSequence? csq1, string? csq2)
        {
            if (csq1 is null || !csq1.HasValue)
                return csq2 is null;
            else if (csq2 is null)
                return !csq1.HasValue;

            return csq1.Equals(csq2);
        }

        /// <summary>
        /// Compares <paramref name="csq1"/> and <paramref name="csq2"/> for inequality.
        /// Two character sequences are considered equal if they have the same characters
        /// in the same order.
        /// </summary>
        /// <param name="csq1">The first sequence.</param>
        /// <param name="csq2">The second sequence.</param>
        /// <returns><c>true</c> if <paramref name="csq1"/> and <paramref name="csq2"/> do not represent to the same instance; otherwise, <c>false</c>.</returns>
        public static bool operator !=(StringCharSequence? csq1, string? csq2)
        {
            return !(csq1 == csq2);
        }

        /// <summary>
        /// Compares <paramref name="csq1"/> and <paramref name="csq2"/> for equality.
        /// Two character sequences are considered equal if they have the same characters
        /// in the same order.
        /// </summary>
        /// <param name="csq1">The first sequence.</param>
        /// <param name="csq2">The second sequence.</param>
        /// <returns><c>true</c> if <paramref name="csq1"/> and <paramref name="csq2"/> represent to the same instance; otherwise, <c>false</c>.</returns>
        public static bool operator ==(string? csq1, StringCharSequence? csq2)
        {
            if (csq1 is null)
                return csq2 is null || !csq2.HasValue;
            else if (csq2 is null || !csq2.HasValue)
                return false;

            return csq2.Equals(csq1);
        }

        /// <summary>
        /// Compares <paramref name="csq1"/> and <paramref name="csq2"/> for inequality.
        /// Two character sequences are considered equal if they have the same characters
        /// in the same order.
        /// </summary>
        /// <param name="csq1">The first sequence.</param>
        /// <param name="csq2">The second sequence.</param>
        /// <returns><c>true</c> if <paramref name="csq1"/> and <paramref name="csq2"/> do not represent to the same instance; otherwise, <c>false</c>.</returns>
        public static bool operator !=(string? csq1, StringCharSequence? csq2)
        {
            return !(csq1 == csq2);
        }

        #endregion

        #region Equality Comparison

        /// <summary>
        /// Determines whether this <see cref="StringCharSequence"/> is equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="other">An <see cref="ICharSequence"/> to compare to the current <see cref="StringCharSequence"/>.</param>
        /// <returns><c>true</c> if <paramref name="other"/> is equal to the current <see cref="StringCharSequence"/>; otherwise, <c>false</c>.</returns>
        public bool Equals(ICharSequence? other)
        {
            if (!HasValue)
                return other is null || !other.HasValue;
            if (other is null || !other.HasValue)
                return false;

            return CharSequenceComparer.Ordinal.Equals(this, other);
        }

        /// <summary>
        /// Determines whether this <see cref="StringCharSequence"/> is equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="other">A <see cref="StringCharSequence"/> to compare to the current <see cref="StringCharSequence"/>.</param>
        /// <returns><c>true</c> if <paramref name="other"/> is equal to the current <see cref="StringCharSequence"/>; otherwise, <c>false</c>.</returns>
        public bool Equals(StringCharSequence? other)
        {
            if (!HasValue)
                return other is null || !other.HasValue;
            if (other is null || !other.HasValue)
                return false;

            return Equals(other.Value);
        }

        /// <summary>
        /// Determines whether this <see cref="StringCharSequence"/> is equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="other">A <see cref="CharArrayCharSequence"/> to compare to the current <see cref="StringCharSequence"/>.</param>
        /// <returns><c>true</c> if <paramref name="other"/> is equal to the current <see cref="StringCharSequence"/>; otherwise, <c>false</c>.</returns>
        public bool Equals(CharArrayCharSequence? other)
        {
            if (!HasValue)
                return other is null || !other.HasValue;
            if (other is null || !other.HasValue)
                return false;

            return Equals(other.Value);
        }

        /// <summary>
        /// Determines whether this <see cref="StringCharSequence"/> is equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="other">A <see cref="StringBuilderCharSequence"/> to compare to the current <see cref="StringCharSequence"/>.</param>
        /// <returns><c>true</c> if <paramref name="other"/> is equal to the current <see cref="StringCharSequence"/>; otherwise, <c>false</c>.</returns>
        public bool Equals(StringBuilderCharSequence? other)
        {
            if (!HasValue)
                return other is null || !other.HasValue;
            if (other is null || !other.HasValue)
                return false;

            return Equals(other.Value);
        }

        /// <summary>
        /// Determines whether this <see cref="StringCharSequence"/> is equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="other">A <see cref="string"/> to compare to the current <see cref="StringCharSequence"/>.</param>
        /// <returns><c>true</c> if <paramref name="other"/> is equal to the current <see cref="StringCharSequence"/>; otherwise, <c>false</c>.</returns>
        public bool Equals(string? other)
        {
            var value = Value;
            if (value is null)
                return other is null;
            if (other is null)
                return false;

            int len = Length;
            if (len != other.Length) return false;
            for (int i = 0; i < len; i++)
            {
                if (!value[i].Equals(other[i])) return false;
            }
            return true;
        }

        /// <summary>
        /// Determines whether this <see cref="StringCharSequence"/> is equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="other">A <see cref="StringBuilder"/> to compare to the current <see cref="StringCharSequence"/>.</param>
        /// <returns><c>true</c> if <paramref name="other"/> is equal to the current <see cref="StringCharSequence"/>; otherwise, <c>false</c>.</returns>
        public bool Equals(StringBuilder? other)
        {
            var value = Value;
            if (value is null)
                return other is null;
            if (other is null)
                return false;

            int len = Length;
            int otherLength = other.Length;
            if (len != otherLength) return false;

            char[]? otherArrayToReturnToPool = null;
            try
            {
#if FEATURE_STRINGBUILDER_COPYTO_SPAN // If this method isn't supported, we are buffering to an array pool to get to the stack, anyway.
                Span<char> otherChars = otherLength > CharStackBufferSize
                    ? (otherArrayToReturnToPool = ArrayPool<char>.Shared.Rent(otherLength))
                    : stackalloc char[otherLength];
                other.CopyTo(0, otherChars, otherLength);
#else
                Span<char> otherChars = otherArrayToReturnToPool = ArrayPool<char>.Shared.Rent(otherLength);
                other.CopyTo(0, otherArrayToReturnToPool, 0, otherLength);
#endif
                for (int i = 0; i < len; i++)
                {
                    if (!value[i].Equals(otherChars[i])) return false;
                }
                return true;
            }
            finally
            {
                ArrayPool<char>.Shared.ReturnIfNotNull(otherArrayToReturnToPool);
            }
        }

        /// <summary>
        /// Determines whether this <see cref="StringCharSequence"/> is equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="other">A <see cref="T:char[]"/> to compare to the current <see cref="StringCharSequence"/>.</param>
        /// <returns><c>true</c> if <paramref name="other"/> is equal to the current <see cref="StringCharSequence"/>; otherwise, <c>false</c>.</returns>
        public bool Equals(char[]? other)
        {
            var value = Value;
            if (value is null)
                return other is null;
            if (other is null)
                return false;

            int len = other.Length;
            if (len != value.Length) return false;
            for (int i = 0; i < len; i++)
            {
                if (!value[i].Equals(other[i])) return false;
            }
            return true;
        }

        /// <summary>
        /// Determines whether this <see cref="StringCharSequence"/> is equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="other">An object to compare to the current <see cref="StringCharSequence"/>.</param>
        /// <returns><c>true</c> if <paramref name="other"/> is equal to the current <see cref="StringCharSequence"/>; otherwise, <c>false</c>.</returns>
        public override bool Equals(object? other)
        {
            if (other is null)
                return !HasValue;

            if (other is string otherString)
                return Equals(otherString);
            else if (other is StringBuilder otherStringBuilder)
                return Equals(otherStringBuilder);
            else if (other is char[] otherCharArray)
                return Equals(otherCharArray);
            else if (other is StringCharSequence otherStringCharSequence)
                return Equals(otherStringCharSequence.Value);
            else if (other is CharArrayCharSequence otherCharArrayCharSequence)
                return Equals(otherCharArrayCharSequence.Value);
            else if (other is StringBuilderCharSequence otherStringBuilderCharSequence)
                return Equals(otherStringBuilderCharSequence.Value);
            else if (other is StringBuffer stringBuffer)
                return Equals(stringBuffer.builder);
            else if (other is ICharSequence otherCharSequence)
                return Equals(otherCharSequence);

            return Equals(other.ToString());
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
        public int CompareTo(ICharSequence? other)
        {
            if (this.Value is null) return (other is null || !other.HasValue) ? 0 : -1;
            if (other is null) return 1;

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
        public int CompareTo(string? other)
        {
            if (this.Value is null) return (other is null) ? 0 : -1;
            if (other is null) return 1;

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
        public int CompareTo(StringBuilder? other)
        {
            if (this.Value is null) return (other is null) ? 0 : -1;
            if (other is null) return 1;

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
        public int CompareTo(char[]? other)
        {
            if (this.Value is null) return (other is null) ? 0 : -1;
            if (other is null) return 1;

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
        public int CompareTo(object? other)
        {
            if (this.Value is null) return (other is null) ? 0 : -1;
            if (other is null) return 1;

            if (other is string otherString)
                return CompareTo(otherString);
            else if (other is StringBuilder otherStringBuilder)
                return CompareTo(otherStringBuilder);
            else if (other is char[] otherCharArray)
                return CompareTo(otherCharArray);
            else if (other is StringCharSequence otherStringCharSequence)
                return CompareTo(otherStringCharSequence.Value);
            else if (other is CharArrayCharSequence otherCharArrayCharSequence)
                return CompareTo(otherCharArrayCharSequence.Value);
            else if (other is StringBuilderCharSequence otherStringBuilderCharSequence)
                return CompareTo(otherStringBuilderCharSequence.Value);
            else if (other is StringBuffer stringBuffer)
                return CompareTo(stringBuffer.builder);
            else if (other is ICharSequence otherCharSequence)
                return CompareTo(otherCharSequence);

            return this.Value.CompareToOrdinal(other.ToString());
        }

        #endregion
    }
}
