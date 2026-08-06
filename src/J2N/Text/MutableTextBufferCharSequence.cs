#region Copyright 2019-2026 by Shad Storhaug, Licensed under the Apache License, Version 2.0
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
using System.Buffers;
using System.Diagnostics;
using System.Text;

namespace J2N.Text
{
    /// <summary>
    /// A wrapper class that represents a <see cref="MutableTextBuffer"/> and implements <see cref="ICharSequence"/>.
    /// </summary>
    internal sealed class MutableTextBufferCharSequence : ICharSequence, IAppendable, ISpanAppendable,
        IEquatable<ICharSequence?>, IComparable<ICharSequence?>, ISpannable<char>, ICopyable<char>, ISpanCopyable<char>
    {
        /// <summary>
        /// Initializes a new instance of <see cref="MutableTextBufferCharSequence"/> with the provided <paramref name="value"/>.
        /// </summary>
        /// <param name="value">A <see cref="MutableTextBuffer"/> to wrap in a <see cref="ICharSequence"/>. The value may be <see langword="null"/>.</param>
        public MutableTextBufferCharSequence(MutableTextBuffer? value)
        {
            Value = value;
            HasValue = (value != null);
        }

        /// <summary>
        /// Gets the current <see cref="MutableTextBuffer"/> value.
        /// </summary>
        // J2N NOTE: Exposing this property allows users access to MutableTextBuffer.RawChars.
        // However, we intentionally don't allow TextBuilder and PooledTextBuilder users to access the unused
        // portion of the array because it may be uninitialized and contain sensitive data from another system.
        // So, before we expose this, we either need specialized adapters (base classes? generics?) that don't
        // expose the Value property. For now, advanced users are directed to use ReadOnlySpan<char> and ReadOnlyMemory<char>
        // rather then porting Java code directly to ICharSequence. That approach is more complex, but usually
        // ends up with far better performance.
        internal MutableTextBuffer? Value { get; }

        #region ICharSequence Members
        /// <summary>
        /// Gets a value indicating whether the current <see cref="MutableTextBufferCharSequence"/>
        /// has a valid <see cref="MutableTextBuffer"/> value.
        /// </summary>
        public bool HasValue { get; }

        /// <summary>
        /// Gets the character at the specified index, with the first character
        /// having index zero.
        /// </summary>
        /// <param name="index">The index of the character to return.</param>
        /// <returns>The requested character.</returns>
        /// <exception cref="IndexOutOfRangeException">
        /// If <c>index &lt; 0</c> or <c>index</c> is greater than the
        /// length of this sequence.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// If the underlying value of this sequence is <see langword="null"/>.
        /// </exception>
        public char this[int index]
        {
            get
            {
                if (Value is null)
                    throw new InvalidOperationException(SR.Format(SR.InvalidOperation_CannotIndexNullObject, nameof(MutableTextBufferCharSequence)));
                return Value[index];
            }
        }

        /// <summary>
        /// Gets the number of characters in this sequence.
        /// </summary>
        public int Length => Value?.Length ?? 0;

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
                return new MutableTextBufferCharSequence(Value);
            }
            if (startIndex < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(startIndex, ExceptionArgument.startIndex);
            if (length < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(length, ExceptionArgument.length);
            if ((uint)startIndex + (uint)length > Value.Length)
                ThrowHelper.ThrowArgumentOutOfRange_IndexLengthString(startIndex, length);

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
            return Value?.ToString() ?? string.Empty;
        }

        #endregion

        #region IAppendable

        /// <summary>
        /// Appends the string representation of a specified <see cref="char"/> object to this instance.
        /// </summary>
        /// <param name="value">The UTF-16-encoded code unit to append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Enlarging the value of this instance would exceed <see cref="MutableTextBuffer.MaxCapacity"/>.</exception>
        /// <exception cref="InvalidOperationException"><see cref="Value"/> is <see langword="null"/>.</exception>
        public MutableTextBufferCharSequence Append(char value)
        {
            if (Value is null)
                throw new InvalidOperationException(SR.Format(SR.InvalidOperation_CannotEditNullObject, nameof(MutableTextBuffer)));

            Value.AppendInternal(value);
            return this;
        }

        /// <summary>
        /// Appends a copy of the specified string to this instance.
        /// </summary>
        /// <param name="value">The string to append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Enlarging the value of this instance would exceed <see cref="MutableTextBuffer.MaxCapacity"/>.</exception>
        /// <exception cref="InvalidOperationException"><see cref="Value"/> is <see langword="null"/>.</exception>
        public MutableTextBufferCharSequence Append(string? value)
        {
            // For null values, this is a no-op
            if (value != null)
            {
                if (Value is null)
                    throw new InvalidOperationException(SR.Format(SR.InvalidOperation_CannotEditNullObject, nameof(MutableTextBuffer)));

                Value.AppendInternal(value);
            }
            return this;
        }

        /// <summary>
        /// Appends a copy of a specified substring to this instance.
        /// </summary>
        /// <param name="value">The string that contains the substring to append.</param>
        /// <param name="startIndex">The starting position of the substring within <paramref name="value"/>.</param>
        /// <param name="count">The number of characters in <paramref name="value"/> to append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>, and 
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
        /// Enlarging the value of this instance would exceed <see cref="MutableTextBuffer.MaxCapacity"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException"><see cref="Value"/> is <see langword="null"/>.</exception>
        public MutableTextBufferCharSequence Append(string? value, int startIndex, int count)
        {
            if (Value is null)
                throw new InvalidOperationException(SR.Format(SR.InvalidOperation_CannotEditNullObject, nameof(MutableTextBuffer)));

            Value.AppendInternal(value, startIndex, count);
            return this;
        }

        /// <summary>
        /// Appends a copy of the specified string to this instance.
        /// </summary>
        /// <param name="value">The <see cref="StringBuilder"/> that contains the string to append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Enlarging the value of this instance would exceed <see cref="MutableTextBuffer.MaxCapacity"/>.</exception>
        /// <exception cref="InvalidOperationException"><see cref="Value"/> is <see langword="null"/>.</exception>
        public MutableTextBufferCharSequence Append(StringBuilder? value)
        {
            // For null values, this is a no-op
            if (value != null)
            {
                if (Value is null)
                    throw new InvalidOperationException(SR.Format(SR.InvalidOperation_CannotEditNullObject, nameof(MutableTextBuffer)));

                Value.AppendInternal(value);
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
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>, and
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
        /// Enlarging the value of this instance would exceed <see cref="MutableTextBuffer.MaxCapacity"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException"><see cref="Value"/> is <see langword="null"/>.</exception>
        public MutableTextBufferCharSequence Append(StringBuilder? value, int startIndex, int count)
        {
            if (Value is null)
                throw new InvalidOperationException(SR.Format(SR.InvalidOperation_CannotEditNullObject, nameof(MutableTextBuffer)));

            Value.AppendInternal(value, startIndex, count);
            return this;
        }

        /// <summary>
        /// Appends the string representation of the Unicode characters in a specified array to this instance.
        /// </summary>
        /// <param name="value">The array of characters to append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Enlarging the value of this instance would exceed <see cref="MutableTextBuffer.MaxCapacity"/>.</exception>
        /// <exception cref="InvalidOperationException"><see cref="Value"/> is <see langword="null"/>.</exception>
        public MutableTextBufferCharSequence Append(char[]? value)
        {
            // For null values, this is a no-op
            if (value != null)
            {
                if (Value is null)
                    throw new InvalidOperationException(SR.Format(SR.InvalidOperation_CannotEditNullObject, nameof(MutableTextBuffer)));

                Value.AppendInternal(value);
            }
            return this;
        }

        /// <summary>
        /// Appends the string representation of a specified subarray of Unicode characters to this instance.
        /// </summary>
        /// <param name="value">A character array.</param>
        /// <param name="startIndex">The starting position in <paramref name="value"/>.</param>
        /// <param name="count">The number of characters in <paramref name="value"/> to append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>, and 
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
        /// Enlarging the value of this instance would exceed <see cref="MutableTextBuffer.MaxCapacity"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException"><see cref="Value"/> is <see langword="null"/>.</exception>
        public MutableTextBufferCharSequence Append(char[]? value, int startIndex, int count)
        {
            if (Value is null)
                throw new InvalidOperationException(SR.Format(SR.InvalidOperation_CannotEditNullObject, nameof(MutableTextBuffer)));

            Value.AppendInternal(value, startIndex, count);
            return this;
        }

        /// <summary>
        /// Appends the string representation of the Unicode characters in a specified <see cref="ICharSequence"/> to this instance.
        /// </summary>
        /// <param name="value">The <see cref="ICharSequence"/> containing the characters to append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Enlarging the value of this instance would exceed <see cref="MutableTextBuffer.MaxCapacity"/>.</exception>
        /// <exception cref="InvalidOperationException"><see cref="Value"/> is <see langword="null"/>.</exception>
        public MutableTextBufferCharSequence Append(ICharSequence? value)
        {
            // For null values, this is a no-op
            if (value != null && value.HasValue)
            {
                if (Value is null)
                    throw new InvalidOperationException(SR.Format(SR.InvalidOperation_CannotEditNullObject, nameof(MutableTextBuffer)));

                Value.AppendInternal(value);
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
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>, and 
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
        /// Enlarging the value of this instance would exceed <see cref="MutableTextBuffer.MaxCapacity"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException"><see cref="Value"/> is <see langword="null"/>.</exception>
        public MutableTextBufferCharSequence Append(ICharSequence? value, int startIndex, int count)
        {
            // For null values, this is a no-op
            if (value != null && value.HasValue)
            {
                if (Value is null)
                    throw new InvalidOperationException(SR.Format(SR.InvalidOperation_CannotEditNullObject, nameof(MutableTextBuffer)));

                Value.AppendInternal(value, startIndex, count);
            }
            return this;
        }

        /// <summary>
        /// Appends the string representation of a specified <see cref="ReadOnlySpan{T}"/> of Unicode characters to this instance.
        /// </summary>
        /// <param name="value">The <see cref="ReadOnlySpan{T}"/> containing the characters to append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <exception cref="InvalidOperationException"><see cref="Value"/> is <see langword="null"/>.</exception>
        public MutableTextBufferCharSequence Append(ReadOnlySpan<char> value)
        {
            if (Value is null)
                throw new InvalidOperationException(SR.Format(SR.InvalidOperation_CannotEditNullObject, nameof(MutableTextBuffer)));

            Value.AppendInternal(value);
            return this;
        }

        IAppendable IAppendable.Append(char value) => Append(value);

        IAppendable IAppendable.Append(string? value) => Append(value);

        IAppendable IAppendable.Append(string? value, int startIndex, int count) => Append(value, startIndex, count);

        IAppendable IAppendable.Append(StringBuilder? value) => Append(value);

        IAppendable IAppendable.Append(StringBuilder? value, int startIndex, int count) => Append(value, startIndex, count);

        IAppendable IAppendable.Append(char[]? value) => Append(value);

        IAppendable IAppendable.Append(char[]? value, int startIndex, int count) => Append(value, startIndex, count);

        IAppendable IAppendable.Append(ICharSequence? value) => Append(value);

        IAppendable IAppendable.Append(ICharSequence? value, int startIndex, int count) => Append(value, startIndex, count);

        ISpanAppendable ISpanAppendable.Append(ReadOnlySpan<char> value) => Append(value);

        #endregion

        #region Equality Comparison

        /// <summary>
        /// Determines whether this char sequence is equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="other">An <see cref="ICharSequence"/> to compare to the current char sequence.</param>
        /// <returns><see langword="true"/> if <paramref name="other"/> is equal to the current char sequence; otherwise, <see langword="false"/>.</returns>
        public bool Equals(ICharSequence? other)
        {
            if (!HasValue)
                return other is null || !other.HasValue;
            if (other is null || !other.HasValue)
                return false;

            int len = Length;
            if (len != other.Length) return false;

            if (other is ISpannable<char> spannable)
            {
                return Value.AsSpan().SequenceEqual(spannable.AsSpan());
            }
            else if (other is StringBuilderCharSequence stringBuilderCharSequence)
            {
                return Equals(stringBuilderCharSequence.Value);
            }
            else if (other is SynchronizedTextBuilderCharSequence synchronizedTextBuilderCharSequence)
            {
                return Equals(synchronizedTextBuilderCharSequence);
            }
            else if (other is StringBuffer stringBuffer)
            {
                return Equals(stringBuffer);
            }

            ReadOnlySpan<char> thisSpan = Value.AsSpan();
            for (int i = 0; i < len; i++)
            {
                if (!thisSpan[i].Equals(other[i])) return false;
            }
            return true;
        }

        /// <summary>
        /// Determines whether this char sequence is equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="other">A <see cref="T:char[]"/> to compare to the current char sequence.</param>
        /// <returns><see langword="true"/> if <paramref name="other"/> is equal to the current char sequence; otherwise, <see langword="false"/>.</returns>
        internal bool Equals(char[]? other)
        {
            if (!HasValue)
                return other is null;
            if (other is null)
                return false;

            return Value!.AsSpan().SequenceEqual(other);
        }

        /// <summary>
        /// Determines whether this char sequence is equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="other">A <see cref="string"/> to compare to the current char sequence.</param>
        /// <returns><see langword="true"/> if <paramref name="other"/> is equal to the current char sequence; otherwise, <see langword="false"/>.</returns>
        internal bool Equals(ReadOnlySpan<char> other)
        {
            if (!HasValue)
                return false;

            return Value.AsSpan().SequenceEqual(other);
        }

        /// <summary>
        /// Determines whether this char sequence is equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="other">A <see cref="string"/> to compare to the current char sequence.</param>
        /// <returns><see langword="true"/> if <paramref name="other"/> is equal to the current char sequence; otherwise, <see langword="false"/>.</returns>
        internal bool Equals(string? other)
        {
            if (!HasValue)
                return other is null;
            if (other is null)
                return false;

            return Value!.AsSpan().SequenceEqual(other);
        }

        /// <summary>
        /// Determines whether this char sequence is equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="other">A <see cref="StringBuilder"/> to compare to the current char sequence.</param>
        /// <returns><see langword="true"/> if <paramref name="other"/> is equal to the current char sequence; otherwise, <see langword="false"/>.</returns>
        internal bool Equals(StringBuilder? other)
        {
            if (!HasValue)
                return other is null;
            if (other is null)
                return false;

            int len = Length;
            int otherLength = other.Length;
            if (len != otherLength) return false;

#if FEATURE_STRINGBUILDER_GETCHUNKS
            ReadOnlySpan<char> thisSpan = Value.AsSpan();

            int offset = 0;
            foreach (ReadOnlyMemory<char> otherChunk in other.GetChunks())
            {
                ReadOnlySpan<char> thisChunk = thisSpan.Slice(offset, otherChunk.Length);
                if (!otherChunk.Span.SequenceEqual(thisChunk))
                    return false;

                offset += otherChunk.Length;
            }
            Debug.Assert(offset == Length);
            return true;
#else
            char[]? buffer = ArrayPool<char>.Shared.Rent(otherLength);
            try
            {
                other.CopyTo(0, buffer, 0, otherLength);
                return Equals(buffer.AsSpan(0, otherLength));
            }
            finally
            {
                ArrayPool<char>.Shared.Return(buffer);
            }
#endif
        }

        internal bool Equals(StringBuffer? other)
        {
            if (!HasValue)
                return other is null;
            if (other is null)
                return false;

            int len = Length;
            int otherLength = other.Length;
            if (len != otherLength) return false;

#if FEATURE_STRINGBUILDER_GETCHUNKS
            lock (other.SyncRoot)
            {
                ReadOnlySpan<char> thisSpan = Value.AsSpan();

                int offset = 0;
                foreach (ReadOnlyMemory<char> otherChunk in other.GetChunks())
                {
                    ReadOnlySpan<char> thisChunk = thisSpan.Slice(offset, otherChunk.Length);
                    if (!otherChunk.Span.SequenceEqual(thisChunk))
                        return false;

                    offset += otherChunk.Length;
                }
                Debug.Assert(offset == Length);
            }
            return true;
#else
            char[]? buffer = ArrayPool<char>.Shared.Rent(otherLength);
            try
            {
                other.CopyTo(0, buffer, 0, otherLength);
                return Equals(buffer.AsSpan(0, otherLength));
            }
            finally
            {
                ArrayPool<char>.Shared.Return(buffer);
            }
#endif
        }

        internal bool Equals(SynchronizedTextBuilderCharSequence? other)
        {
            if (!HasValue)
                return other is null || !other.HasValue;
            if (other is null || !other.HasValue)
                return false;

            lock (other.SyncRoot)
                return Value.AsSpan().SequenceEqual(other.Value.AsSpan());
        }

        internal bool Equals(SynchronizedTextBuilder? other)
        {
            if (!HasValue)
                return other is null;
            if (other is null)
                return false;

            lock (other.SyncRoot)
                return Value.AsSpan().SequenceEqual(other.AsSpan());
        }

        /// <summary>
        /// Determines whether this char sequence is equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="other">An object to compare to the current char sequence.</param>
        /// <returns><see langword="true"/> if <paramref name="other"/> is equal to the current char sequence; otherwise, <see langword="false"/>.</returns>
        public override bool Equals(object? other)
        {
            if (other is null)
                return !HasValue;

            if (other is ISpannable<char> spannable)
            {
                if (!spannable.HasValue)
                    return !HasValue;
                else if (!HasValue)
                    return false;

                return Value.AsSpan().SequenceEqual(spannable.AsSpan());
            }

            if (other is string otherString)
                return Equals(otherString);
            else if (other is char[] otherCharArray)
                return Equals(otherCharArray);
            else if (other is StringBuilder otherStringBuilder)
                return Equals(otherStringBuilder);
            else if (other is ICharSequence otherCharSequence)
                return Equals(otherCharSequence);
            else if (other is SynchronizedTextBuilder otherSynchronizedTextBuilder)
                return Equals(otherSynchronizedTextBuilder);

            return Equals(other.ToString());
        }

        /// <summary>
        /// Gets the hash code for the current <see cref="ICharSequence"/>.
        /// </summary>
        /// <returns>Returns the hash code for <see cref="Value"/>. If <see cref="Value"/> is <see langword="null"/>, returns <see cref="int.MaxValue"/>.</returns>
        public override int GetHashCode()
        {
            // NOTE: For consistency, we match all char sequences to the same
            // hash code. This unfortunately means it won't match
            // against String, StringBuilder or char[]. But that only matters
            // if the types are put into the same hashtable.

            if (!HasValue)
                return int.MaxValue;

            ReadOnlySpan<char> value = Value.AsSpan();
            // From Apache Harmony
            int length = value.Length;
            if (length == 0)
                return 0;

            unchecked
            {
                int hash = 0;
                for (int i = 0; i < length; i++)
                {
                    hash = value[i] + ((hash << 5) - hash);
                }
                return hash;
            }
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
            if (!HasValue) return (other is null || !other.HasValue) ? 0 : -1;
            if (other is null || !other.HasValue) return 1;

            if (other is ISpannable<char> spannable)
            {
                return CompareTo(spannable.AsSpan());
            }
            else if (other is StringBuilderCharSequence stringBuilderCharSequence)
            {
                return CompareTo(stringBuilderCharSequence.Value);
            }
            else if (other is SynchronizedTextBuilderCharSequence synchronizedTextBuilderCharSequence)
            {
                return CompareTo(synchronizedTextBuilderCharSequence);
            }
            else if (other is StringBuffer stringBuffer)
            {
                return CompareTo(stringBuffer);
            }

            int length = Math.Min(Length, other.Length);
            int result;
            ReadOnlySpan<char> thisSpan = Value.AsSpan();
            for (int i = 0; i < length; i++)
            {
                if ((result = thisSpan[i] - other[i]) != 0)
                    return result;
            }

            // At this point, we have compared all the characters in at least one string.
            // The longer string will be larger.
            return thisSpan.Length - other.Length;
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
        internal int CompareTo(string? other)
        {
            if (!HasValue) return (other is null) ? 0 : -1;
            if (other is null) return 1;

            return Value.AsSpan().CompareTo(other, StringComparison.Ordinal);
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
        internal int CompareTo(StringBuilder? other)
        {
            if (!HasValue) return (other is null) ? 0 : -1;
            if (other is null) return 1;

#if FEATURE_STRINGBUILDER_GETCHUNKS
            ReadOnlySpan<char> thisSpan = Value.AsSpan();

            int result;
            int thisIndex = 0;
            int remaining = Math.Min(thisSpan.Length, other.Length);

            foreach (ReadOnlyMemory<char> chunk in other.GetChunks())
            {
                ReadOnlySpan<char> chunkSpan = chunk.Span;
                int count = Math.Min(remaining, chunkSpan.Length);

                for (int i = 0; i < count; i++, thisIndex++)
                {
                    if ((result = thisSpan[thisIndex] - chunkSpan[i]) != 0)
                        return result;
                }

                remaining -= count;
                if (remaining == 0)
                    break;
            }

            // At this point, we have compared all the characters in at least one string.
            // The longer string will be larger.
            return thisSpan.Length - other.Length;
#else
            char[] buffer = ArrayPool<char>.Shared.Rent(other.Length);
            try
            {
                other.CopyTo(0, buffer, 0, other.Length);
                return CompareTo(buffer.AsSpan(0, other.Length));
            }
            finally
            {
                ArrayPool<char>.Shared.Return(buffer);
            }
#endif
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
        internal int CompareTo(char[]? other)
        {
            if (!HasValue) return (other is null) ? 0 : -1;
            if (other is null) return 1;

            return Value.AsSpan().CompareTo(other, StringComparison.Ordinal);
        }

        /// <summary>
        /// Compares this instance with a specified <see cref="ReadOnlySpan{Char}"/> and indicates whether
        /// this instance precedes, follows, or appears in the same position in the sort order as the specified string.
        /// </summary>
        /// <param name="other">The <see cref="ReadOnlySpan{Char}"/> to compare with this instance.</param>
        /// <returns>
        /// An integer that indicates the lexical relationship between the two comparands.
        /// Less than zero indicates the comparison value is greater than the current string.
        /// Zero indicates the strings are equal.
        /// Greater than zero indicates the comparison value is less than the current string.
        /// </returns>
        internal int CompareTo(ReadOnlySpan<char> other)
        {
            if (!HasValue)
                return -1;

            return Value.AsSpan().CompareTo(other, StringComparison.Ordinal);
        }

        internal int CompareTo(StringBuffer? other)
        {
            if (!HasValue) return (other is null) ? 0 : -1;
            if (other is null) return 1;

#if FEATURE_STRINGBUILDER_GETCHUNKS
            ReadOnlySpan<char> thisSpan = Value.AsSpan();

            lock (other.SyncRoot)
            {
                int result;
                int thisIndex = 0;
                int remaining = Math.Min(thisSpan.Length, other.Length);

                foreach (ReadOnlyMemory<char> chunk in other.builder.GetChunks())
                {
                    ReadOnlySpan<char> chunkSpan = chunk.Span;
                    int count = Math.Min(remaining, chunkSpan.Length);

                    for (int i = 0; i < count; i++, thisIndex++)
                    {
                        if ((result = thisSpan[thisIndex] - chunkSpan[i]) != 0)
                            return result;
                    }

                    remaining -= count;
                    if (remaining == 0)
                        break;
                }

                // At this point, we have compared all the characters in at least one string.
                // The longer string will be larger.
                return thisSpan.Length - other.Length;
            }
#else
            char[] buffer = ArrayPool<char>.Shared.Rent(other.Length);
            try
            {
                other.CopyTo(0, buffer, 0, other.Length);
                return CompareTo(buffer.AsSpan(0, other.Length));
            }
            finally
            {
                ArrayPool<char>.Shared.Return(buffer);
            }
#endif
        }

        internal int CompareTo(SynchronizedTextBuilderCharSequence? other)
        {
            if (!HasValue) return (other is null || !other.HasValue) ? 0 : -1;
            if (other is null || !other.HasValue) return 1;

            lock (other.SyncRoot)
                return Value.AsSpan().CompareTo(other.Value.AsSpan(), StringComparison.Ordinal);
        }

        internal int CompareTo(SynchronizedTextBuilder? other)
        {
            if (!HasValue) return (other is null) ? 0 : -1;
            if (other is null) return 1;

            lock (other.SyncRoot)
                return Value.AsSpan().CompareTo(other.AsSpan(), StringComparison.Ordinal);
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
            if (other is null)
                return !HasValue ? 0 : 1;

            if (other is ISpannable<char> spannable)
            {
                if (!spannable.HasValue)
                    return !HasValue ? 0 : 1;
                else if (!HasValue)
                    return -1;

                return CompareTo(spannable.AsSpan());
            }

            if (other is string otherString)
                return CompareTo(otherString);
            else if (other is StringBuilder otherStringBuilder)
                return CompareTo(otherStringBuilder);
            else if (other is char[] otherCharArray)
                return CompareTo(otherCharArray);
            else if (other is ICharSequence otherCharSequence)
                return CompareTo(otherCharSequence);
            else if (other is SynchronizedTextBuilder otherSynchronizedTextBuilder)
                return CompareTo(otherSynchronizedTextBuilder);

            return Value.AsSpan().CompareTo(other.ToString(), StringComparison.Ordinal);
        }

        #endregion

        #region ISpanCopyable<char>

        /// <summary>
        /// Copies the characters from a specified segment of this instance to a destination <see cref="char"/> span.
        /// </summary>
        /// <param name="sourceIndex">The starting position in this instance where characters will be copied from.
        /// The index is zero-based.</param>
        /// <param name="destination">The writable span where characters will be copied.</param>
        /// <param name="count">The number of characters to be copied.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="sourceIndex"/> or <paramref name="count"/> is less than 0.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="sourceIndex"/> is greater than <see cref="Length"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="sourceIndex"/> + <paramref name="count"/> is greater than <see cref="Length"/>.
        /// </exception>
        public void CopyTo(int sourceIndex, Span<char> destination, int count)
        {
            Value?.CopyTo(sourceIndex, destination, count);
        }

        #endregion ISpanCopyable<char>

        #region ICopyable<char>

        /// <summary>
        /// Copies the characters from a specified segment of this instance to a specified segment of a destination
        /// <see cref="char"/> array.
        /// </summary>
        /// <param name="sourceIndex">The starting position in this instance where characters will be copied from.
        /// The index is zero-based.</param>
        /// <param name="destination">The array where characters will be copied.</param>
        /// <param name="destinationIndex">The starting position in <paramref name="destination"/> where characters will be copied.
        /// The index is zero-based.</param>
        /// <param name="count">The number of characters to be copied.</param>
        /// <exception cref="ArgumentNullException"><paramref name="destination"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="sourceIndex"/>, <paramref name="destinationIndex"/>, or <paramref name="count"/>, is less than zero.
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
        /// <paramref name="destinationIndex"/> + <paramref name="count"/> is greater than the length of <paramref name="destination"/>.
        /// </exception>
        public void CopyTo(int sourceIndex, char[] destination, int destinationIndex, int count)
        {
            Value?.CopyTo(sourceIndex, destination, destinationIndex, count);
        }

        #endregion ICopyable<char>

        #region ISpannable<char> Members

        ReadOnlySpan<char> ISpannable<char>.AsSpan() => Value.AsSpan();

        ReadOnlySpan<char> ISpannable<char>.AsSpan(int start) => Value.AsSpan(start);

        ReadOnlySpan<char> ISpannable<char>.AsSpan(int start, int length) => Value.AsSpan(start, length);

        #endregion ISpannable<char> Members
    }
}
