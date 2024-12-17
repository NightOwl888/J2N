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
    /// A wrapper class that represents a <see cref="System.Text.StringBuilder"/> and implements <see cref="ICharSequence"/>.
    /// </summary>
    public class StringBuilderCharSequence : ICharSequence, IAppendable, 
        IComparable<ICharSequence>, IComparable,
        IComparable<string>, IComparable<StringBuilder>, IComparable<char[]>,
        IEquatable<ICharSequence>,
        IEquatable<CharArrayCharSequence>, IEquatable<StringBuilderCharSequence>, IEquatable<StringCharSequence>,
        IEquatable<string>, IEquatable<StringBuilder>, IEquatable<char[]>, ISpanAppendable
    {
        private const int CharStackBufferSize = 64;

        /// <summary>
        /// Initializes a new instance of <see cref="StringBuilderCharSequence"/> with a new backing <see cref="StringBuilder"/>.
        /// </summary>
        public StringBuilderCharSequence()
        {
            this.Value = new StringBuilder();
            this.HasValue = true;
        }


        /// <summary>
        /// Initializes a new instance of <see cref="StringBuilderCharSequence"/> with the provided <paramref name="value"/>.
        /// </summary>
        /// <param name="value">A <see cref="StringBuilder"/> to wrap in a <see cref="ICharSequence"/>. The value may be <c>null</c>.</param>
        public StringBuilderCharSequence(StringBuilder? value)
        {
            this.Value = value;
            this.HasValue = (value != null);
        }

        /// <summary>
        /// Gets the current <see cref="StringBuilder"/> value.
        /// </summary>
        public StringBuilder? Value { get; }

        #region ICharSequence

        /// <summary>
        /// Gets a value indicating whether the current <see cref="StringBuilderCharSequence"/>
        /// has a valid <see cref="StringBuilder"/> value.
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
                    throw new InvalidOperationException(J2N.SR.Format(SR.InvalidOperation_CannotIndexNullObject, nameof(StringBuilderCharSequence)));
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
                return new StringBuilderCharSequence(Value);
            }
            if (startIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(startIndex), SR.ArgumentOutOfRange_NeedNonNegNum);
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length), SR.ArgumentOutOfRange_NeedNonNegNum);
            if (startIndex > Value.Length - length) // Checks for int overflow
                throw new ArgumentOutOfRangeException(nameof(length), SR.ArgumentOutOfRange_IndexLength);

            // NOTE: This benchmarked slightly faster than
            // return new StringCharSequence(Value.ToString(startIndex, length));
            char[] result = new char[length];
            Value.CopyTo(startIndex, result, 0, length);
            return new CharArrayCharSequence(result);
        }

        /// <summary>
        /// Returns a string with the same characters in the same order as in this
        /// sequence.
        /// </summary>
        /// <returns>A string based on this sequence.</returns>
        public override string ToString()
        {
            return (Value is null) ? string.Empty : Value.ToString();
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
        public static bool operator ==(StringBuilderCharSequence? csq1, StringBuilderCharSequence? csq2)
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
        public static bool operator !=(StringBuilderCharSequence? csq1, StringBuilderCharSequence? csq2)
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
        public static bool operator ==(StringBuilderCharSequence? csq1, StringBuilder? csq2)
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
        public static bool operator !=(StringBuilderCharSequence? csq1, StringBuilder? csq2)
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
        public static bool operator ==(StringBuilder? csq1, StringBuilderCharSequence? csq2)
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
        public static bool operator !=(StringBuilder? csq1, StringBuilderCharSequence? csq2)
        {
            return !(csq1 == csq2);
        }

        #endregion

        #region Equality

        /// <summary>
        /// Determines whether this <see cref="StringBuilderCharSequence"/> is equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="other">An <see cref="ICharSequence"/> to compare to the current <see cref="StringBuilderCharSequence"/>.</param>
        /// <returns><c>true</c> if <paramref name="other"/> is equal to the current <see cref="StringBuilderCharSequence"/>; otherwise, <c>false</c>.</returns>
        public bool Equals(ICharSequence? other)
        {
            if (!HasValue)
                return other is null || !other.HasValue;
            if (other is null || !other.HasValue)
                return false;

            if (other.Length != Length) return false;

            if (other is StringBuilderCharSequence stringBuilderCharSequence)
                return Equals(stringBuilderCharSequence.Value);
            if (other is StringBuffer stringBuffer)
                return Equals(stringBuffer.builder);

            return CharSequenceComparer.Ordinal.Equals(this, other);
        }

        /// <summary>
        /// Determines whether this <see cref="StringBuilderCharSequence"/> is equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="other">A <see cref="CharArrayCharSequence"/> to compare to the current <see cref="StringBuilderCharSequence"/>.</param>
        /// <returns><c>true</c> if <paramref name="other"/> is equal to the current <see cref="StringBuilderCharSequence"/>; otherwise, <c>false</c>.</returns>
        public bool Equals(CharArrayCharSequence? other)
        {
            if (!HasValue)
                return other is null || !other.HasValue;
            if (other is null || !other.HasValue)
                return false;

            return Equals(other.Value);
        }

        /// <summary>
        /// Determines whether this <see cref="StringBuilderCharSequence"/> is equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="other">A <see cref="StringBuilderCharSequence"/> to compare to the current <see cref="StringBuilderCharSequence"/>.</param>
        /// <returns><c>true</c> if <paramref name="other"/> is equal to the current <see cref="StringBuilderCharSequence"/>; otherwise, <c>false</c>.</returns>
        public bool Equals(StringBuilderCharSequence? other)
        {
            if (!HasValue)
                return other is null || !other.HasValue;
            if (other is null || !other.HasValue)
                return false;

            return Equals(other.Value);
        }

        /// <summary>
        /// Determines whether this <see cref="StringBuilderCharSequence"/> is equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="other">A <see cref="StringCharSequence"/> to compare to the current <see cref="StringBuilderCharSequence"/>.</param>
        /// <returns><c>true</c> if <paramref name="other"/> is equal to the current <see cref="StringBuilderCharSequence"/>; otherwise, <c>false</c>.</returns>
        public bool Equals(StringCharSequence? other)
        {
            if (!HasValue)
                return other is null || !other.HasValue;
            if (other is null || !other.HasValue)
                return false;

            return Equals(other.Value);
        }

        /// <summary>
        /// Determines whether this <see cref="StringBuilderCharSequence"/> is equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="other">A <see cref="string"/> to compare to the current <see cref="StringBuilderCharSequence"/>.</param>
        /// <returns><c>true</c> if <paramref name="other"/> is equal to the current <see cref="StringBuilderCharSequence"/>; otherwise, <c>false</c>.</returns>
        public bool Equals(string? other)
        {
            var value = Value;
            if (value is null)
                return other is null;
            if (other is null)
                return false;

            int len = Length;
            int otherLength = other.Length;
            if (len != otherLength) return false;

            char[]? thisArrayToReturnToPool = null;
            try
            {
#if FEATURE_STRINGBUILDER_COPYTO_SPAN // If this method isn't supported, we are buffering to an array pool to get to the stack, anyway.
                Span<char> chars = len > CharStackBufferSize
                    ? (thisArrayToReturnToPool = ArrayPool<char>.Shared.Rent(len))
                    : stackalloc char[len];
                value.CopyTo(0, chars, len);
#else
                Span<char> chars = thisArrayToReturnToPool = ArrayPool<char>.Shared.Rent(len);
                value.CopyTo(0, thisArrayToReturnToPool, 0, len);
#endif
                for (int i = 0; i < len; i++)
                {
                    if (!chars[i].Equals(other[i])) return false;
                }
                return true;
            }
            finally
            {
                ArrayPool<char>.Shared.ReturnIfNotNull(thisArrayToReturnToPool);
            }
        }

        /// <summary>
        /// Determines whether this <see cref="StringBuilderCharSequence"/> is equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="other">A <see cref="StringBuilder"/> to compare to the current <see cref="StringBuilderCharSequence"/>.</param>
        /// <returns><c>true</c> if <paramref name="other"/> is equal to the current <see cref="StringBuilderCharSequence"/>; otherwise, <c>false</c>.</returns>
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

            char[]? thisArrayToReturnToPool = null;
            char[]? otherArrayToReturnToPool = null;
            try
            {
#if FEATURE_STRINGBUILDER_COPYTO_SPAN // If this method isn't supported, we are buffering to an array pool to get to the stack, anyway.
                Span<char> chars = len > CharStackBufferSize
                    ? (thisArrayToReturnToPool = ArrayPool<char>.Shared.Rent(len))
                    : stackalloc char[len];
                Span<char> otherChars = otherLength > CharStackBufferSize
                    ? (otherArrayToReturnToPool = ArrayPool<char>.Shared.Rent(otherLength))
                    : stackalloc char[otherLength];
                value.CopyTo(0, chars, len);
                other.CopyTo(0, otherChars, otherLength);
#else
                Span<char> chars = thisArrayToReturnToPool = ArrayPool<char>.Shared.Rent(len);
                Span<char> otherChars = otherArrayToReturnToPool = ArrayPool<char>.Shared.Rent(len);
                value.CopyTo(0, thisArrayToReturnToPool, 0, len);
                other.CopyTo(0, otherArrayToReturnToPool, 0, otherLength);
#endif
                for (int i = 0; i < len; i++)
                {
                    if (!chars[i].Equals(otherChars[i])) return false;
                }
                return true;
            }
            finally
            {
                ArrayPool<char>.Shared.ReturnIfNotNull(thisArrayToReturnToPool);
                ArrayPool<char>.Shared.ReturnIfNotNull(otherArrayToReturnToPool);
            }
        }

        /// <summary>
        /// Determines whether this <see cref="StringBuilderCharSequence"/> is equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="other">A <see cref="T:char[]"/> to compare to the current <see cref="StringBuilderCharSequence"/>.</param>
        /// <returns><c>true</c> if <paramref name="other"/> is equal to the current <see cref="StringBuilderCharSequence"/>; otherwise, <c>false</c>.</returns>
        public bool Equals(char[]? other)
        {
            var value = Value;
            if (value is null)
                return other is null;
            if (other is null)
                return false;

            int len = Length;
            int otherLength = other.Length;
            if (len != otherLength) return false;

            char[]? thisArrayToReturnToPool = null;
            try
            {
#if FEATURE_STRINGBUILDER_COPYTO_SPAN // If this method isn't supported, we are buffering to an array pool to get to the stack, anyway.
                Span<char> chars = len > CharStackBufferSize
                    ? (thisArrayToReturnToPool = ArrayPool<char>.Shared.Rent(len))
                    : stackalloc char[len];
                value.CopyTo(0, chars, len);
#else
                Span<char> chars = thisArrayToReturnToPool = ArrayPool<char>.Shared.Rent(len);
                value.CopyTo(0, thisArrayToReturnToPool, 0, len);
#endif
                for (int i = 0; i < len; i++)
                {
                    if (!chars[i].Equals(other[i])) return false;
                }
                return true;
            }
            finally
            {
                ArrayPool<char>.Shared.ReturnIfNotNull(thisArrayToReturnToPool);
            }
        }

        /// <summary>
        /// Determines whether this <see cref="StringBuilderCharSequence"/> is equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="other">An object to compare to the current <see cref="StringBuilderCharSequence"/>.</param>
        /// <returns><c>true</c> if <paramref name="other"/> is equal to the current <see cref="StringBuilderCharSequence"/>; otherwise, <c>false</c>.</returns>
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

        #region IComparable<T>

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
            if (!HasValue) return (other is null) ? 0 : -1;
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

            return Value.CompareToOrdinal(other.ToString());
        }

        #endregion

        #region IAppendable

        /// <summary>
        /// Appends the string representation of a specified <see cref="char"/> object to this instance.
        /// </summary>
        /// <param name="value">The UTF-16-encoded code unit to append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Enlarging the value of this instance would exceed <see cref="StringBuilder.MaxCapacity"/>.</exception>
        /// <exception cref="InvalidOperationException"><see cref="Value"/> is <c>null</c>.</exception>
        public StringBuilderCharSequence Append(char value)
        {
            if (Value is null)
                throw new InvalidOperationException(J2N.SR.Format(SR.InvalidOperation_CannotEditNullObject, nameof(StringBuilder)));

            Value.Append(value);
            return this;
        }

        /// <summary>
        /// Appends a copy of the specified string to this instance.
        /// </summary>
        /// <param name="value">The string to append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Enlarging the value of this instance would exceed <see cref="StringBuilder.MaxCapacity"/>.</exception>
        /// <exception cref="InvalidOperationException"><see cref="Value"/> is <c>null</c>.</exception>
        public StringBuilderCharSequence Append(string? value)
        {
            if (Value is null)
                throw new InvalidOperationException(J2N.SR.Format(SR.InvalidOperation_CannotEditNullObject, nameof(StringBuilder)));

            Value.Append(value);
            return this;
        }

        /// <summary>
        /// Appends a copy of a specified substring to this instance.
        /// </summary>
        /// <param name="value">The <see cref="StringBuilder"/> that contains the substring to append.</param>
        /// <param name="startIndex">The starting position of the substring within <paramref name="value"/>.</param>
        /// <param name="count">The number of characters in <paramref name="value"/> to append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <c>null</c>, and 
        /// <paramref name="startIndex"/> and <paramref name="count"/> are not zero.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="count"/> less than zero.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="startIndex"/> less than zero.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="startIndex"/> + <paramref name="count"/> is greater than the length of <paramref name="value"/>.
        /// <para/>
        /// -or-
        /// <para/>
        /// Enlarging the value of this instance would exceed <see cref="StringBuilder.MaxCapacity"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException"><see cref="Value"/> is <c>null</c>.</exception>
        public StringBuilderCharSequence Append(string? value, int startIndex, int count)
        {
            if (Value is null)
                throw new InvalidOperationException(J2N.SR.Format(SR.InvalidOperation_CannotEditNullObject, nameof(StringBuilder)));

            Value.Append(value, startIndex, count);
            return this;
        }

        /// <summary>
        /// Appends a copy of the specified string to this instance.
        /// </summary>
        /// <param name="value">The <see cref="StringBuilder"/> that contains the string to append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Enlarging the value of this instance would exceed <see cref="StringBuilder.MaxCapacity"/>.</exception>
        /// <exception cref="InvalidOperationException"><see cref="Value"/> is <c>null</c>.</exception>
        public StringBuilderCharSequence Append(StringBuilder? value)
        {
            if (Value is null)
                throw new InvalidOperationException(J2N.SR.Format(SR.InvalidOperation_CannotEditNullObject, nameof(StringBuilder)));

            if (value != null)
            {
#if FEATURE_STRINGBUILDER_APPEND_STRINGBUILDER
                Value.Append(value);
#else
                Value.Append(charSequence: value);
#endif
            }
            return this;
        }

        /// <summary>
        /// Appends a copy of a specified substring to this instance.
        /// </summary>
        /// <param name="value">The <see cref="StringBuilder"/> that contains the substring to append.</param>
        /// <param name="startIndex">The starting position of the substring within <paramref name="value"/>.</param>
        /// <param name="count">The number of characters in <paramref name="value"/> to append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <c>null</c>, and
        /// <paramref name="startIndex"/> and <paramref name="count"/> are not zero.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="count"/> less than zero.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="startIndex"/> less than zero.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="startIndex"/> + <paramref name="count"/> is greater than the length of <paramref name="value"/>.
        /// <para/>
        /// -or-
        /// <para/>
        /// Enlarging the value of this instance would exceed <see cref="StringBuilder.MaxCapacity"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException"><see cref="Value"/> is <c>null</c>.</exception>
        public StringBuilderCharSequence Append(StringBuilder? value, int startIndex, int count)
        {
            if (Value is null)
                throw new InvalidOperationException(J2N.SR.Format(SR.InvalidOperation_CannotEditNullObject, nameof(StringBuilder)));

            if (value != null)
                Value.Append(value, startIndex, count);
            return this;
        }

        /// <summary>
        /// Appends the string representation of the Unicode characters in a specified array to this instance.
        /// </summary>
        /// <param name="value">The array of characters to append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Enlarging the value of this instance would exceed <see cref="StringBuilder.MaxCapacity"/>.</exception>
        /// <exception cref="InvalidOperationException"><see cref="Value"/> is <c>null</c>.</exception>
        public StringBuilderCharSequence Append(char[]? value)
        {
            if (Value is null)
                throw new InvalidOperationException(J2N.SR.Format(SR.InvalidOperation_CannotEditNullObject, nameof(StringBuilder)));

            Value.Append(value);
            return this;
        }

        /// <summary>
        /// Appends the string representation of a specified subarray of Unicode characters to this instance.
        /// </summary>
        /// <param name="value">A character array.</param>
        /// <param name="startIndex">The starting position in <paramref name="value"/>.</param>
        /// <param name="count">The number of characters in <paramref name="value"/> to append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <c>null</c>, and 
        /// <paramref name="startIndex"/> and <paramref name="count"/> are not zero.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="count"/> less than zero.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="startIndex"/> less than zero.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="startIndex"/> + <paramref name="count"/> is greater than the length of <paramref name="value"/>.
        /// <para/>
        /// -or-
        /// <para/>
        /// Enlarging the value of this instance would exceed <see cref="StringBuilder.MaxCapacity"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException"><see cref="Value"/> is <c>null</c>.</exception>
        public StringBuilderCharSequence Append(char[]? value, int startIndex, int count)
        {
            if (Value is null)
                throw new InvalidOperationException(J2N.SR.Format(SR.InvalidOperation_CannotEditNullObject, nameof(StringBuilder)));

            Value.Append(value, startIndex, count);
            return this;
        }

        /// <summary>
        /// Appends the string representation of the Unicode characters in a specified <see cref="ICharSequence"/> to this instance.
        /// </summary>
        /// <param name="value">The <see cref="ICharSequence"/> containing the characters to append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Enlarging the value of this instance would exceed <see cref="StringBuilder.MaxCapacity"/>.</exception>
        /// <exception cref="InvalidOperationException"><see cref="Value"/> is <c>null</c>.</exception>
        public StringBuilderCharSequence Append(ICharSequence? value)
        {
            // For null values, this is a no-op
            if (value != null && value.HasValue)
            {
                if (Value is null)
                    throw new InvalidOperationException(J2N.SR.Format(SR.InvalidOperation_CannotEditNullObject, nameof(StringBuilder)));

                if (value is StringCharSequence stringCharSequence)
                    Value.Append(stringCharSequence.Value); // Already checked for null above
                else if (value is StringBuilderCharSequence stringBuilderCharSequence)
#if FEATURE_STRINGBUILDER_APPEND_STRINGBUILDER
                    Value.Append(stringBuilderCharSequence.Value); // Already checked for null above
#else
                    Value.Append(charSequence: stringBuilderCharSequence.Value); // Already checked for null above
#endif
                else if (value is CharArrayCharSequence charArrayCharSequence)
                    Value.Append(charArrayCharSequence.Value); // Already checked for null above
                else if (value is StringBuffer stringBuffer)
#if FEATURE_STRINGBUILDER_APPEND_STRINGBUILDER
                    Value.Append(stringBuffer.builder);
#else
                    Value.Append(charSequence: stringBuffer.builder);
#endif
                else
                    Value.Append(charSequence: value);
            }
            return this;
        }

        /// <summary>
        /// Appends the string representation of a specified <see cref="ICharSequence"/> of Unicode characters to this instance.
        /// </summary>
        /// <param name="value">The <see cref="ICharSequence"/> containing the characters to append.</param>
        /// <param name="startIndex">The starting position in <paramref name="value"/>.</param>
        /// <param name="count">The number of characters in <paramref name="value"/> to append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <c>null</c>, and 
        /// <paramref name="startIndex"/> and <paramref name="count"/> are not zero.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="count"/> less than zero.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="startIndex"/> less than zero.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="startIndex"/> + <paramref name="count"/> is greater than the length of <paramref name="value"/>.
        /// <para/>
        /// -or-
        /// <para/>
        /// Enlarging the value of this instance would exceed <see cref="StringBuilder.MaxCapacity"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException"><see cref="Value"/> is <c>null</c>.</exception>
        public StringBuilderCharSequence Append(ICharSequence? value, int startIndex, int count)
        {
            // For null values, this is a no-op
            if (value != null && value.HasValue)
            {
                if (Value is null)
                    throw new InvalidOperationException(J2N.SR.Format(SR.InvalidOperation_CannotEditNullObject, nameof(StringBuilder)));

                if (value is StringCharSequence stringCharSequence)
                    Value.Append(stringCharSequence.Value, startIndex, count); // Already checked for null above
                else if (value is StringBuilderCharSequence stringBuilderCharSequence)
                    Value.Append(stringBuilderCharSequence.Value, startIndex, count); // Already checked for null above
                else if (value is CharArrayCharSequence charArrayCharSequence)
                    Value.Append(charArrayCharSequence.Value, startIndex, count); // Already checked for null above
                else if (value is StringBuffer stringBuffer)
                    Value.Append(stringBuffer.builder, startIndex, count);
                else
                    Value.Append(value, startIndex, count);
            }
            return this;
        }

        /// <summary>
        /// Appends the string representation of a specified <see cref="ReadOnlySpan{T}"/> of Unicode characters to this instance.
        /// </summary>
        /// <param name="value">The <see cref="ReadOnlySpan{T}"/> containing the characters to append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <exception cref="InvalidOperationException"><see cref="Value"/> is <c>null</c>.</exception>
        public StringBuilderCharSequence Append(ReadOnlySpan<char> value)
        {
            if (Value is null)
                throw new InvalidOperationException(J2N.SR.Format(SR.InvalidOperation_CannotEditNullObject, nameof(StringBuilder)));

            Value.Append(value);
            return this;
        }

        IAppendable IAppendable.Append(char value) => this.Append(value);

        IAppendable IAppendable.Append(string? value) => this.Append(value);

        IAppendable IAppendable.Append(string? value, int startIndex, int count) => this.Append(value, startIndex, count);

        IAppendable IAppendable.Append(StringBuilder? value) => this.Append(value);

        IAppendable IAppendable.Append(StringBuilder? value, int startIndex, int count) => this.Append(value, startIndex, count);

        IAppendable IAppendable.Append(char[]? value) => this.Append(value);

        IAppendable IAppendable.Append(char[]? value, int startIndex, int count) => this.Append(value, startIndex, count);

        IAppendable IAppendable.Append(ICharSequence? value) => this.Append(value);

        IAppendable IAppendable.Append(ICharSequence? value, int startIndex, int count) => this.Append(value, startIndex, count);

        ISpanAppendable ISpanAppendable.Append(ReadOnlySpan<char> value) => this.Append(value);

        #endregion
    }

}
