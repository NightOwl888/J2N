// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Text;


namespace J2N.Text
{
    using SR = J2N.Resources.Strings;

    /// <summary>
    /// A thread-safe, mutable sequence of characters.
    /// A string buffer is like a <see cref="string"/>, but can be modified. At any
    /// point in time it contains some particular sequence of characters, but
    /// the length and content of the sequence can be changed through certain
    /// method calls.
    /// <para/>
    /// String buffers are safe for use by multiple threads. The methods
    /// are synchronized where necessary so that all the operations on any
    /// particular instance behave as if they occur in some serial order
    /// that is consistent with the order of the method calls made by each of
    /// the individual threads involved.
    /// <para/>
    /// The principal operations on a <see cref="StringBuffer"/> are the
    /// <see cref="Append(string)"/> and <see cref="Insert(int, string)"/> methods, which are
    /// overloaded so as to accept data of any type. Each effectively
    /// converts a given datum to a string and then appends or inserts the
    /// characters of that string to the string buffer. The
    /// <see cref="Append(string)"/> method always adds these characters at the end
    /// of the buffer; the <see cref="Insert(int, string)"/> method adds the characters at
    /// a specified point.
    /// <para/>
    /// For example, if <c>z</c> refers to a string buffer object
    /// whose current contents are "<c>start</c>", then
    /// the method call <c>z.Append("le")</c> would cause the string
    /// buffer to contain "<c>startle</c>", whereas
    /// <c>z.Insert(4, "le")</c> would alter the string buffer to
    /// contain "<c>starlet</c>".
    /// <para/>
    /// In general, if sb refers to an instance of a <see cref="StringBuffer"/>,
    /// then <c>sb.Append(x)</c> has the same effect as
    /// <c>sb.Insert(sb.Length,&#160;x)</c>.
    /// <para/>
    /// Whenever an operation occurs involving a source sequence (such as
    /// appending or inserting from a source sequence) this class synchronizes
    /// only on the string buffer performing the operation, not on the source.
    /// <para/>
    /// Every string buffer has a capacity. As long as the length of the
    /// character sequence contained in the string buffer does not exceed
    /// the capacity, it is not necessary to allocate a new internal
    /// buffer array. If the internal buffer overflows, it is
    /// automatically made larger.
    /// <para/>
    /// Usage Note: The <see cref="StringBuilder"/> class should generally be used in preference to
    /// this one, as it supports all of the same operations but it is faster, as
    /// it performs no synchronization.
    /// <para/>
    /// <see cref="StringBuffer"/> takes a naive approach to locking, and is intended
    /// mainly as a drop in replacement for Java's <c>StringBuffer</c> class.
    /// </summary>
    /// <seealso cref="StringBuilder"/>
    /// <seealso cref="string"/>
#if FEATURE_SERIALIZABLE_STRINGS
    [Serializable]
#endif
    public sealed class StringBuffer : IAppendable, ICharSequence
    {
        private const int DefaultCapacity = 16;

        internal readonly StringBuilder builder; // internal for testing

#if FEATURE_SERIALIZABLE_STRINGS
        [NonSerialized]
#else
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0044:Add readonly modifier", Justification = "not readonly for serilization")]
#endif
        private object syncRoot = new object(); // not readonly for serilization

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="StringBuffer"/> class with an
        /// initial capacity of 16 characters.
        /// </summary>
        public StringBuffer()
            : this(capacity: DefaultCapacity)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringBuffer"/> class using the specified capacity.
        /// </summary>
        /// <param name="capacity">The suggested starting size of this instance.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is less than zero.</exception>
        public StringBuffer(int capacity)
            : this(string.Empty, capacity)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringBuffer"/> class using the specified string.
        /// </summary>
        /// <param name="value">The string used to initialize the value of the instance.
        /// If <paramref name="value"/> is <c>null</c>, the new <see cref="StringBuffer"/> will
        /// contain the empty string (that is, it contains <see cref="string.Empty"/>).</param>
        public StringBuffer(string? value)
            : this(value, (value?.Length ?? 0) + DefaultCapacity)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringBuffer"/> class using the
        /// specified character sequence.
        /// </summary>
        /// <param name="value">The character sequence used to initialize the value of the instance.
        /// If <paramref name="value"/> is <c>null</c>, the new <see cref="StringBuffer"/> will
        /// contain the empty string (that is, it contains <see cref="string.Empty"/>).</param>
        public StringBuffer(ICharSequence? value)
            : this(value, (value?.Length ?? 0) + DefaultCapacity)
        { }

        // .NET Gaps

        /// <summary>
        /// Initializes a new instance of the <see cref="StringBuffer"/> class
        /// using the specified character sequence and capacity.
        /// </summary>
        /// <param name="value">The string used to initialize the value of the instance.
        /// If <paramref name="value"/> is <c>null</c>, the new <see cref="StringBuffer"/> will
        /// contain the empty string (that is, it contains <see cref="string.Empty"/>).</param>
        /// <param name="capacity">The suggested starting size of the <see cref="StringBuffer"/>.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is less than zero.</exception>
        public StringBuffer(ICharSequence? value, int capacity)
            : this(value, 0, value?.Length ?? 0, capacity)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringBuffer"/> class
        /// from the specified subsequence and capacity.
        /// </summary>
        /// <param name="value">The string used to initialize the value of the instance.
        /// If <paramref name="value"/> is <c>null</c>, the new <see cref="StringBuffer"/> will
        /// contain the empty string (that is, it contains <see cref="string.Empty"/>).</param>
        /// <param name="startIndex">The position within <paramref name="value"/> where the substring begins.</param>
        /// <param name="length">The number of characters in the substring.</param>
        /// <param name="capacity">The suggested starting size of the <see cref="StringBuffer"/>.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="capacity"/> is less than zero.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="startIndex"/> plus <paramref name="length"/> is not a position within <paramref name="value"/>.
        /// </exception>
        public StringBuffer(ICharSequence? value, int startIndex, int length, int capacity)
        {
            if (capacity < 0)
                throw new ArgumentOutOfRangeException(nameof(capacity), SR.ArgumentOutOfRange_NeedNonNegNum);
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length), SR.ArgumentOutOfRange_NeedNonNegNum);
            if (startIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(startIndex), SR.ArgumentOutOfRange_NeedNonNegNum);
            Contract.EndContractBlock();

            string stringValue;
            if (value is null)
            {
                stringValue = string.Empty;
            }
            else
            {
                if (startIndex > value.Length - length)
                    throw new ArgumentOutOfRangeException(nameof(length));
                stringValue = value.Subsequence(startIndex, length).ToString();
            }

            this.builder = new StringBuilder(stringValue, capacity);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringBuffer"/> class using the
        /// specified character sequence.
        /// </summary>
        /// <param name="value">The character sequence used to initialize the value of the instance.
        /// If <paramref name="value"/> is <c>null</c>, the new <see cref="StringBuffer"/> will
        /// contain the empty string (that is, it contains <see cref="string.Empty"/>).</param>
        public StringBuffer(StringBuilder? value)
            : this(value, (value?.Length ?? 0) + DefaultCapacity)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringBuffer"/> class
        /// using the specified string and capacity.
        /// </summary>
        /// <param name="value">The string used to initialize the value of the instance.
        /// If <paramref name="value"/> is <c>null</c>, the new <see cref="StringBuffer"/> will
        /// contain the empty string (that is, it contains <see cref="string.Empty"/>).</param>
        /// <param name="capacity">The suggested starting size of the <see cref="StringBuffer"/>.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is less than zero.</exception>
        public StringBuffer(StringBuilder? value, int capacity)
            : this(value, 0, value?.Length ?? 0, capacity)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringBuffer"/> class
        /// from the specified substring and capacity.
        /// </summary>
        /// <param name="value">The string used to initialize the value of the instance.
        /// If <paramref name="value"/> is <c>null</c>, the new <see cref="StringBuffer"/> will
        /// contain the empty string (that is, it contains <see cref="string.Empty"/>).</param>
        /// <param name="startIndex">The position within <paramref name="value"/> where the substring begins.</param>
        /// <param name="length">The number of characters in the substring.</param>
        /// <param name="capacity">The suggested starting size of the <see cref="StringBuffer"/>.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="capacity"/> is less than zero.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="startIndex"/> plus <paramref name="length"/> is not a position within <paramref name="value"/>.
        /// </exception>
        public StringBuffer(StringBuilder? value, int startIndex, int length, int capacity)
        {
            if (capacity < 0)
                throw new ArgumentOutOfRangeException(nameof(capacity), SR.ArgumentOutOfRange_NeedNonNegNum);
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length), SR.ArgumentOutOfRange_NeedNonNegNum);
            if (startIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(startIndex), SR.ArgumentOutOfRange_NeedNonNegNum);
            Contract.EndContractBlock();

            string stringValue;
            if (value is null)
            {
                stringValue = string.Empty;
            }
            else
            {
                if (startIndex > value.Length - length)
                    throw new ArgumentOutOfRangeException(nameof(length));
                stringValue = value.ToString(startIndex, length);
            }

            this.builder = new StringBuilder(stringValue, capacity);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringBuffer"/> class using the
        /// specified character sequence.
        /// </summary>
        /// <param name="value">The character sequence used to initialize the value of the instance.
        /// If <paramref name="value"/> is <c>null</c>, the new <see cref="StringBuffer"/> will
        /// contain the empty string (that is, it contains <see cref="string.Empty"/>).</param>
        public StringBuffer(char[]? value)
            : this(value, (value?.Length ?? 0) + DefaultCapacity)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringBuffer"/> class
        /// using the specified subsequence and capacity.
        /// </summary>
        /// <param name="value">The string used to initialize the value of the instance.
        /// If <paramref name="value"/> is <c>null</c>, the new <see cref="StringBuffer"/> will
        /// contain the empty string (that is, it contains <see cref="string.Empty"/>).</param>
        /// <param name="capacity">The suggested starting size of the <see cref="StringBuffer"/>.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is less than zero.</exception>
        public StringBuffer(char[]? value, int capacity)
            : this(value, 0, value?.Length ?? 0, capacity)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringBuffer"/> class
        /// from the specified character array and capacity.
        /// </summary>
        /// <param name="value">The string used to initialize the value of the instance.
        /// If <paramref name="value"/> is <c>null</c>, the new <see cref="StringBuffer"/> will
        /// contain the empty string (that is, it contains <see cref="string.Empty"/>).</param>
        /// <param name="startIndex">The position within <paramref name="value"/> where the substring begins.</param>
        /// <param name="length">The number of characters in the substring.</param>
        /// <param name="capacity">The suggested starting size of the <see cref="StringBuffer"/>.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="capacity"/> is less than zero.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="startIndex"/> plus <paramref name="length"/> is not a position within <paramref name="value"/>.
        /// </exception>
        public StringBuffer(char[]? value, int startIndex, int length, int capacity)
        {
            if (capacity < 0)
                throw new ArgumentOutOfRangeException(nameof(capacity), SR.ArgumentOutOfRange_NeedNonNegNum);
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length), SR.ArgumentOutOfRange_NeedNonNegNum);
            if (startIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(startIndex), SR.ArgumentOutOfRange_NeedNonNegNum);
            Contract.EndContractBlock();

            string stringValue;
            if (value is null)
            {
                stringValue = string.Empty;
            }
            else
            {
                if (startIndex > value.Length - length)
                    throw new ArgumentOutOfRangeException(nameof(length));
                stringValue = new string(value, startIndex, length);
            }

            this.builder = new StringBuilder(stringValue, capacity);
        }


        // .NET constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="StringBuffer"/> class that starts
        /// with a specified capacity and can grow to a specified maximum.
        /// </summary>
        /// <param name="capacity">The suggested starting size of the <see cref="StringBuffer"/>.</param>
        /// <param name="maxCapacity">The maximum number of characters the current string can contain.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="maxCapacity"/>is less than one, <paramref name="capacity"/> is less than zero,
        /// or <paramref name="capacity"/> is greater than <paramref name="maxCapacity"/>.</exception>
        public StringBuffer(int capacity, int maxCapacity)
        {
            this.builder = new StringBuilder(capacity, maxCapacity);
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="StringBuffer"/> class
        /// using the specified string and capacity.
        /// </summary>
        /// <param name="value">The string used to initialize the value of the instance.
        /// If <paramref name="value"/> is <c>null</c>, the new <see cref="StringBuffer"/> will
        /// contain the empty string (that is, it contains <see cref="string.Empty"/>).</param>
        /// <param name="capacity">The suggested starting size of the <see cref="StringBuffer"/>.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is less than zero.</exception>
        public StringBuffer(string? value, int capacity)
            : this(value, 0, value?.Length ?? 0, capacity)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringBuffer"/> class
        /// from the specified substring and capacity.
        /// </summary>
        /// <param name="value">The string used to initialize the value of the instance.
        /// If <paramref name="value"/> is <c>null</c>, the new <see cref="StringBuffer"/> will
        /// contain the empty string (that is, it contains <see cref="string.Empty"/>).</param>
        /// <param name="startIndex">The position within <paramref name="value"/> where the substring begins.</param>
        /// <param name="length">The number of characters in the substring.</param>
        /// <param name="capacity">The suggested starting size of the <see cref="StringBuffer"/>.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="capacity"/> is less than zero.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="startIndex"/> plus <paramref name="length"/> is not a position within <paramref name="value"/>.
        /// </exception>
        public StringBuffer(string? value, int startIndex, int length, int capacity)
        {
            this.builder = new StringBuilder(value, startIndex, length, capacity);
        }

        #endregion Constructors

        /// <summary>
        /// Gets an object that can be used to synchronize access to the <see cref="StringBuffer"/>.
        /// </summary>
        public object SyncRoot => syncRoot;


#if FEATURE_SERIALIZABLE_STRINGS
        [System.Runtime.Serialization.OnDeserialized]
        internal void OnDeserializedMethod(System.Runtime.Serialization.StreamingContext context)
        {
            syncRoot = new object();
        }
#endif

        /// <summary>
        /// Gets or sets the length of the current <see cref="StringBuffer"/> object.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">The value specified for a set
        /// operation is less than zero or greater than <see cref="MaxCapacity"/>.</exception>
        public int Length
        {
            get
            {
                lock (syncRoot)
                    return builder.Length;
            }
            set
            {
                lock (syncRoot)
                    builder.Length = value;
            }
        }

        /// <summary>
        /// Gets the maximum capacity of this instance.
        /// </summary>
        public int MaxCapacity
        {
            get
            {
                lock (syncRoot)
                    return builder.MaxCapacity;
            }
        }

        /// <summary>
        /// Gets or sets the maximum number of characters that can be contained
        /// in the memory allocated by the current instance.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The value specified for a set operation is less than the current length of this instance.
        /// <para/>
        /// -or-
        /// <para/>
        /// The value specified for a set operation is greater than the maximum capacity.
        /// </exception>
        public int Capacity
        {
            get
            {
                lock (syncRoot)
                    return builder.Capacity;
            }
            set
            {
                lock (syncRoot)
                    builder.Capacity = value;
            }
        }

        /// <summary>
        /// Gets or sets the character at the specified character position in this instance.
        /// </summary>
        /// <param name="index">The position of the character.</param>
        /// <returns>The Unicode character at position <paramref name="index"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is outside the bounds of this instance while setting a character.
        /// </exception>
        /// <exception cref="IndexOutOfRangeException">
        /// <paramref name="index"/> is outside the bounds of this instance while getting a character.
        /// </exception>
        public char this[int index]
        {
            get
            {
                lock (syncRoot)
                    return builder[index];
            }
            set
            {
                lock (syncRoot)
                    builder[index] = value;
            }
        }

        /// <summary>
        /// Returns the code point at <paramref name="index"/> in the specified sequence of
        /// character units. If the unit at <paramref name="index"/> is a high-surrogate unit,
        /// <c><paramref name="index"/> + 1</c> is less than the length of the sequence and the unit at
        /// <c><paramref name="index"/> + 1</c> is a low-surrogate unit, then the supplementary code
        /// point represented by the pair is returned; otherwise the <see cref="char"/>
        /// value at <paramref name="index"/> is returned.
        /// </summary>
        /// <param name="index">The position in this instance from which to retrieve the code
        /// point.</param>
        /// <returns>The Unicode code point or <see cref="char"/> value at <paramref name="index"/> in
        /// this instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is greater than or equal to <see cref="Length"/>.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="index"/> is less than zero.
        /// </exception>
        public int CodePointAt(int index)
        {
            lock (syncRoot)
                return builder.CodePointAt(index);
        }

        /// <summary>
        /// Returns the code point that precedes <paramref name="index"/> in the specified
        /// sequence of character units. If the unit at <c><paramref name="index"/> - 1</c> is a
        /// low-surrogate unit, <c><paramref name="index"/> - 2</c> is not negative and the unit at
        /// <c><paramref name="index"/> - 2</c> is a high-surrogate unit, then the supplementary code
        /// point represented by the pair is returned; otherwise the <see cref="char"/>
        /// value at <c><paramref name="index"/> - 1</c> is returned.
        /// </summary>
        /// <param name="index">The position in this instance following the code
        /// point that should be returned.</param>
        /// <returns>The Unicode code point or <see cref="char"/> value before <paramref name="index"/>
        /// in this instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException">If the <paramref name="index"/> is less than 1 or greater than
        /// <see cref="Length"/>.</exception>
        public int CodePointBefore(int index)
        {
            lock (syncRoot)
                return builder.CodePointBefore(index);
        }

        /// <summary>
        /// Returns the number of Unicode code points in the text range of the specified char sequence.
        /// The text range begins at the specified <paramref name="startIndex"/> and extends for the number of characters specified in <paramref name="length"/>. 
        /// Unpaired surrogates within the text range count as one code point each.
        /// <para/>
        /// IMPORTANT: This method has .NET semantics. That is, the second parameter is a length rather than an exclusive end index. To convert from
        /// Java, use <c>endIndex - startIndex</c> to obtain the length.
        /// </summary>
        /// <param name="startIndex">The index to the first char of the text range.</param>
        /// <param name="length">The number of characters to consider in the count from this instance.</param>
        /// <returns>The number of Unicode code points in the specified text range.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startIndex"/> plus <paramref name="length"/> indicates a position not within this instance.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="startIndex"/> or <paramref name="length"/> is less than zero.
        /// </exception>
        public int CodePointCount(int startIndex, int length)
        {
            lock (syncRoot)
                return builder.CodePointCount(startIndex, length);
        }

        /// <summary>
        /// Returns the index within the given char sequence that is offset from the given <paramref name="index"/> by
        /// <paramref name="codePointOffset"/> code points. Unpaired surrogates within the text range given by 
        /// <paramref name="index"/> and <paramref name="codePointOffset"/> count as one code point each.
        /// </summary>
        /// <param name="index">The index to be offset.</param>
        /// <param name="codePointOffset">The number of code points to look backwards or forwards; may
        /// be a negative or positive value.</param>
        /// <returns>The index within the char sequence, offset by <paramref name="codePointOffset"/> code points.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than zero or greater than the length of this instance.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="codePointOffset"/> is positive and the subsequence starting with <paramref name="index"/> has fewer than
        /// <paramref name="codePointOffset"/> code points.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="codePointOffset"/> is negative and the subsequence before <paramref name="index"/> has fewer than
        /// the absolute value of <paramref name="codePointOffset"/> code points.
        /// </exception>
        public int OffsetByCodePoints(int index, int codePointOffset)
        {
            lock (syncRoot)
                return builder.OffsetByCodePoints(index, codePointOffset);
        }



#if FEATURE_STRINGBUILDER_GETCHUNKS
        internal StringBuilder.ChunkEnumerator GetChunks() // J2N TODO: API: Make public (need to evaluate whether we need our own implementation of ChunkEnumerator and how to synchronize)
        {
            return builder.GetChunks();
        }
#endif


        #region Append

#if FEATURE_STRINGBUILDER_APPEND_READONLYSPAN

        /// <summary>
        /// Appends a copy of the specified <see cref="ReadOnlySpan{Char}"/> to this instance.
        /// </summary>
        /// <param name="value">The string to append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Enlarging the value of this instance would exceed <see cref="MaxCapacity"/>.</exception>
        public StringBuffer Append(ReadOnlySpan<char> value)
        {
            lock (syncRoot)
            {
                builder.Append(value);
                return this;
            }
        }
#endif
#if FEATURE_READONLYMEMORY

        /// <summary>
        /// Appends a copy of the specified <see cref="ReadOnlyMemory{Char}"/> to this instance.
        /// </summary>
        /// <param name="value">The string to append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Enlarging the value of this instance would exceed <see cref="MaxCapacity"/>.</exception>
        public StringBuffer Append(ReadOnlyMemory<char> value)
        {
            lock (syncRoot)
            {
                builder.Append(value);
                return this;
            }
        }
#endif

        /// <summary>
        /// Appends the string representation of a specified object to this instance.
        /// </summary>
        /// <param name="value">The object to append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Enlarging the value of this instance would exceed <see cref="MaxCapacity"/>.</exception>
        public StringBuffer Append(object? value)
        {
            lock (syncRoot)
            {
                builder.Append(value);
                return this;
            }
        }

        /// <summary>
        /// Appends a copy of the specified string to this instance.
        /// </summary>
        /// <param name="value">The string to append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Enlarging the value of this instance would exceed <see cref="MaxCapacity"/>.</exception>
        public StringBuffer Append(string? value)
        {
            lock (syncRoot)
            {
                builder.Append(value);
                return this;
            }
        }

        /// <summary>
        /// Appends a copy of a specified substring to this instance.
        /// <para/>
        /// IMPORTANT: This method has .NET semantics. That is, the fourth parameter is a count, not an exclusive end index as would be the
        /// case in Java. To translate from Java, use <c>end - start</c> to resolve <paramref name="count"/>.
        /// </summary>
        /// <param name="value">The string that contains the substring to append.</param>
        /// <param name="startIndex">The starting position of the substring within <paramref name="value"/>.</param>
        /// <param name="count">The number of characters in <paramref name="value"/> to append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <c>null</c>, and
        /// <paramref name="startIndex"/> and <paramref name="count"/> are not zero.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="count"/> is less than zero.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="startIndex"/> is less than zero.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="startIndex"/> + <paramref name="count"/> is greater than the length of <paramref name="value"/>.
        /// <para/>
        /// -or-
        /// <para/>
        /// Enlarging the value of this instance would exceed <see cref="MaxCapacity"/>.
        /// </exception>
        public StringBuffer Append(string? value, int startIndex, int count)
        {
            lock (syncRoot)
            {
                builder.Append(value, startIndex, count);
                return this;
            }
        }

        /// <summary>
        /// Appends the string representation of the Unicode characters in a specified <see cref="StringBuffer"/> to this instance.
        /// <para/>
        /// This method synchronizes on this (the destination)
        /// object but does not synchronize on the source <paramref name="value"/>.
        /// <para/>
        /// NOTE: Unlike the Java implementation, this method does not add the word <c>"null"</c> to the <see cref="StringBuffer"/>
        /// if <paramref name="value"/> is <c>null</c>. Instead, no operation is performed.
        /// </summary>
        /// <param name="value">The <see cref="StringBuffer"/> to append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Enlarging the value of this instance would exceed <see cref="MaxCapacity"/>.</exception>
        public StringBuffer Append(StringBuffer? value)
        {
            if (value is null)
                return this;

            lock (syncRoot)
            {
#if FEATURE_STRINGBUILDER_APPEND_STRINGBUILDER
                builder.Append(value.builder);
#else
                builder.Append(charSequence: value.builder);
#endif
                return this;
            }
        }

        /// <summary>
        /// Appends the string representation of a specified subarray of Unicode characters to this instance.
        /// <para/>
        /// IMPORTANT: This method has .NET semantics. That is, the fourth parameter is a count, not an exclusive end index as would be the
        /// case in Java. To translate from Java, use <c>end - start</c> to resolve <paramref name="charCount"/>.
        /// </summary>
        /// <param name="charSequence">The UTF-16-encoded code unit to append.</param>
        /// <param name="startIndex">The starting position in <paramref name="charSequence"/>.</param>
        /// <param name="charCount">The number of characters to append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="charSequence"/> is <c>null</c>, and
        /// <paramref name="startIndex"/> and <paramref name="charCount"/> are not zero.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="charCount"/> is less than zero.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="startIndex"/> is less than zero.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="startIndex"/> + <paramref name="charCount"/> is greater than the length of <paramref name="charSequence"/>.
        /// <para/>
        /// -or-
        /// <para/>
        /// Enlarging the value of this instance would exceed <see cref="MaxCapacity"/>.
        /// </exception>
        public StringBuffer Append(StringBuffer? charSequence, int startIndex, int charCount)
        {
            lock (syncRoot)
            {
                builder.Append(charSequence?.builder, startIndex, charCount);
                return this;
            }
        }

        /// <summary>
        /// Appends the string representation of the Unicode characters in a specified <see cref="StringBuilder"/> to this instance.
        /// <para/>
        /// NOTE: Unlike the Java implementation, this method does not add the word <c>"null"</c> to the <see cref="StringBuffer"/>
        /// if <paramref name="value"/> is <c>null</c>. Instead, no operation is performed.
        /// </summary>
        /// <param name="value">The <see cref="StringBuffer"/> to append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Enlarging the value of this instance would exceed <see cref="MaxCapacity"/>.</exception>
        public StringBuffer Append(StringBuilder? value)
        {
            if (value is null)
                return this;

            lock (syncRoot)
            {
#if FEATURE_STRINGBUILDER_APPEND_STRINGBUILDER
                builder.Append(value);
#else
                builder.Append(charSequence: value);
#endif
                return this;
            }
        }

        /// <summary>
        /// Appends the string representation of a specified subarray of Unicode characters to this instance.
        /// <para/>
        /// IMPORTANT: This method has .NET semantics. That is, the fourth parameter is a count, not an exclusive end index as would be the
        /// case in Java. To translate from Java, use <c>end - start</c> to resolve <paramref name="charCount"/>.
        /// </summary>
        /// <param name="charSequence">The UTF-16-encoded code unit to append.</param>
        /// <param name="startIndex">The starting position in <paramref name="charSequence"/>.</param>
        /// <param name="charCount">The number of characters to append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="charSequence"/> is <c>null</c>, and
        /// <paramref name="startIndex"/> and <paramref name="charCount"/> are not zero.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="charCount"/> is less than zero.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="startIndex"/> is less than zero.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="startIndex"/> + <paramref name="charCount"/> is greater than the length of <paramref name="charSequence"/>.
        /// <para/>
        /// -or-
        /// <para/>
        /// Enlarging the value of this instance would exceed <see cref="MaxCapacity"/>.
        /// </exception>
        public StringBuffer Append(StringBuilder? charSequence, int startIndex, int charCount)
        {
            lock (syncRoot)
            {
                builder.Append(charSequence, startIndex, charCount);
                return this;
            }
        }

        /// <summary>
        /// Appends the string representation of the Unicode characters in a specified sequence to this instance.
        /// <para/>
        /// This method synchronizes on this (the destination)
        /// object but does not synchronize on the source <paramref name="charSequence"/>.
        /// <para/>
        /// NOTE: Unlike the Java implementation, this method does not add the word <c>"null"</c> to the <see cref="StringBuffer"/>
        /// if <paramref name="charSequence"/> is <c>null</c>. Instead, no operation is performed.
        /// </summary>
        /// <param name="charSequence">The sequence of characters to append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Enlarging the value of this instance would exceed <see cref="MaxCapacity"/>.</exception>
        public StringBuffer Append(ICharSequence? charSequence)
        {
            // Note, synchronization achieved via other invocations
            if (charSequence is StringCharSequence stringCharSequence)
                return this.Append(stringCharSequence.Value);
            if (charSequence is StringBuffer stringBufferCharSequence)
                return this.Append(stringBufferCharSequence);

            lock (syncRoot)
            {
                builder.Append(charSequence: charSequence);
                return this;
            }
        }

        /// <summary>
        /// Appends the string representation of a specified subarray of Unicode characters to this instance.
        /// </summary>
        /// <param name="charSequence">The UTF-16-encoded code unit to append.</param>
        /// <param name="startIndex">The starting position in <paramref name="charSequence"/>.</param>
        /// <param name="charCount">The number of characters to append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="charSequence"/> is <c>null</c>, and
        /// <paramref name="startIndex"/> and <paramref name="charCount"/> are not zero.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="charCount"/> is less than zero.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="startIndex"/> is less than zero.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="startIndex"/> + <paramref name="charCount"/> is greater than the length of <paramref name="charSequence"/>.
        /// <para/>
        /// -or-
        /// <para/>
        /// Enlarging the value of this instance would exceed <see cref="MaxCapacity"/>.
        /// </exception>
        public StringBuffer Append(ICharSequence? charSequence, int startIndex, int charCount)
        {
            lock (syncRoot)
            {
                builder.Append(charSequence, startIndex, charCount);
                return this;
            }
        }

        /// <summary>
        /// Appends the string representation of the Unicode characters in a specified array to this instance.
        /// </summary>
        /// <param name="value">The array of characters to append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Enlarging the value of this instance would exceed <see cref="MaxCapacity"/>.</exception>
        public StringBuffer Append(char[]? value)
        {
            lock (syncRoot)
            {
                builder.Append(value);
                return this;
            }
        }

        /// <summary>
        /// Appends the string representation of a specified subarray of Unicode characters to this instance.
        /// </summary>
        /// <param name="value">The UTF-16-encoded code unit to append.</param>
        /// <param name="startIndex">The starting position in <paramref name="value"/>.</param>
        /// <param name="charCount">The number of characters to append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <c>null</c>, and
        /// <paramref name="startIndex"/> and <paramref name="charCount"/> are not zero.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="charCount"/> is less than zero.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="startIndex"/> is less than zero.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="startIndex"/> + <paramref name="charCount"/> is greater than the length of <paramref name="value"/>.
        /// <para/>
        /// -or-
        /// <para/>
        /// Enlarging the value of this instance would exceed <see cref="MaxCapacity"/>.
        /// </exception>
        public StringBuffer Append(char[]? value, int startIndex, int charCount)
        {
            lock (syncRoot)
            {
                builder.Append(value, startIndex, charCount);
                return this;
            }
        }

#if FEATURE_STRINGBUILDER_APPEND_CHARPTR
        /// <summary>
        /// Appends an array of Unicode characters starting at a specified address to this instance.
        /// </summary>
        /// <param name="value">A pointer to an array of characters.</param>
        /// <param name="valueCount">The number of characters in the array.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <exception cref="NullReferenceException"><paramref name="value"/> is a null pointer.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="valueCount"/> is less than zero.
        /// <para/>
        /// -or-
        /// <para/>
        /// Enlarging the value of this instance would exceed <see cref="MaxCapacity"/>.
        /// </exception>
        [CLSCompliant(false)]
        [System.Security.SecurityCritical]
        public unsafe StringBuffer Append(char* value, int valueCount)
        {
            lock (syncRoot)
            {
                builder.Append(value, valueCount);
                return this;
            }
        }
#endif

        /// <summary>
        /// Appends the string representation of a specified Boolean value to this instance.
        /// </summary>
        /// <param name="value">The UTF-16-encoded code unit to append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Enlarging the value of this instance would exceed <see cref="MaxCapacity"/>.</exception>
        public StringBuffer Append(bool value)
        {
            lock (syncRoot)
            {
                builder.Append(value);
                return this;
            }
        }

        /// <summary>
        /// Appends the string representation of a specified <see cref="char"/> object to this instance.
        /// </summary>
        /// <param name="value">The UTF-16-encoded code unit to append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Enlarging the value of this instance would exceed <see cref="MaxCapacity"/>.</exception>
        public StringBuffer Append(char value)
        {
            lock (syncRoot)
            {
                builder.Append(value);
                return this;
            }
        }

        /// <summary>
        /// Appends a specified number of copies of the string representation of a Unicode character to this instance.
        /// </summary>
        /// <param name="value">The UTF-16-encoded code unit to append.</param>
        /// <param name="repeatCount">The number of times to append <paramref name="value"/>.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="repeatCount"/> is less than zero.
        /// <para/>
        /// -or-
        /// <para/>
        /// Enlarging the value of this instance would exceed <see cref="MaxCapacity"/>.
        /// </exception>
        public StringBuffer Append(char value, int repeatCount)
        {
            lock (syncRoot)
            {
                builder.Append(value, repeatCount);
                return this;
            }
        }

        /// <summary>
        /// Appends the string representation of a specified 32-bit signed integer to this instance.
        /// </summary>
        /// <param name="value">The value to append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Enlarging the value of this instance would exceed <see cref="MaxCapacity"/>.</exception>
        public StringBuffer Append(int value)
        {
            lock (syncRoot)
            {
                builder.Append(value);
                return this;
            }
        }

        /// <summary>
        /// Appends the string representation of a specified 64-bit signed integer to this instance.
        /// </summary>
        /// <param name="value">The value to append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Enlarging the value of this instance would exceed <see cref="MaxCapacity"/>.</exception>
        public StringBuffer Append(long value)
        {
            lock (syncRoot)
            {
                builder.Append(value);
                return this;
            }
        }

        /// <summary>
        /// Appends the string representation of a specified single-precision floating-point number to this instance.
        /// </summary>
        /// <param name="value">The value to append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Enlarging the value of this instance would exceed <see cref="MaxCapacity"/>.</exception>
        public StringBuffer Append(float value)
        {
            lock (syncRoot)
            {
                builder.Append(value);
                return this;
            }
        }

        /// <summary>
        /// Appends the string representation of a specified double-precision floating-point number to this instance.
        /// </summary>
        /// <param name="value">The value to append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Enlarging the value of this instance would exceed <see cref="MaxCapacity"/>.</exception>
        public StringBuffer Append(double value)
        {
            lock (syncRoot)
            {
                builder.Append(value);
                return this;
            }
        }

        /// <summary>
        /// Appends the string representation of a specified decimal number to this instance.
        /// </summary>
        /// <param name="value">The value to append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Enlarging the value of this instance would exceed <see cref="MaxCapacity"/>.</exception>
        public StringBuffer Append(decimal value)
        {
            lock (syncRoot)
            {
                builder.Append(value);
                return this;
            }
        }

        /// <summary>
        /// Appends the string representation of a specified 8-bit unsigned integer to this instance.
        /// </summary>
        /// <param name="value">The value to append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Enlarging the value of this instance would exceed <see cref="MaxCapacity"/>.</exception>
        public StringBuffer Append(byte value)
        {
            lock (syncRoot)
            {
                builder.Append(value);
                return this;
            }
        }

        /// <summary>
        /// Appends the string representation of a specified 8-bit signed integer to this instance.
        /// </summary>
        /// <param name="value">The value to append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Enlarging the value of this instance would exceed <see cref="MaxCapacity"/>.</exception>
        [CLSCompliant(false)]
        public StringBuffer Append(sbyte value)
        {
            lock (syncRoot)
            {
                builder.Append(value);
                return this;
            }
        }

        /// <summary>
        /// Appends the string representation of a specified 16-bit signed integer to this instance.
        /// </summary>
        /// <param name="value">The value to append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Enlarging the value of this instance would exceed <see cref="MaxCapacity"/>.</exception>
        public StringBuffer Append(short value)
        {
            lock (syncRoot)
            {
                builder.Append(value);
                return this;
            }
        }

        /// <summary>
        /// Appends the string representation of a specified 32-bit unsigned integer to this instance.
        /// </summary>
        /// <param name="value">The value to append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Enlarging the value of this instance would exceed <see cref="MaxCapacity"/>.</exception>
        [CLSCompliant(false)]
        public StringBuffer Append(uint value)
        {
            lock (syncRoot)
            {
                builder.Append(value);
                return this;
            }
        }

        /// <summary>
        /// Appends the string representation of a specified 64-bit unsigned integer to this instance.
        /// </summary>
        /// <param name="value">The value to append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Enlarging the value of this instance would exceed <see cref="MaxCapacity"/>.</exception>
        [CLSCompliant(false)]
        public StringBuffer Append(ulong value)
        {
            lock (syncRoot)
            {
                builder.Append(value);
                return this;
            }
        }

        /// <summary>
        /// Appends the string representation of a specified 16-bit unsigned integer to this instance.
        /// </summary>
        /// <param name="value">The value to append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Enlarging the value of this instance would exceed <see cref="MaxCapacity"/>.</exception>
        [CLSCompliant(false)]
        public StringBuffer Append(ushort value)
        {
            lock (syncRoot)
            {
                builder.Append(value);
                return this;
            }
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
        /// <param name="codePoint">A Unicode code point</param>
        /// <returns>This <see cref="StringBuilder"/>, for chaining.</returns>
        public StringBuffer AppendCodePoint(int codePoint)
        {
            lock (syncRoot)
            {
                builder.AppendCodePoint(codePoint);
                return this;
            }
        }

        #endregion

        #region AppendFormat

        /// <summary>
        /// Appends the string returned by processing a composite format string,
        /// which contains zero or more format items, to this instance. Each
        /// format item is replaced by the string representation of a single argument.
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="arg0">An object to format.</param>
        /// <returns>A reference to this instance with <paramref name="format"/> appended.
        /// Each format item in <paramref name="format"/> is replaced by the string
        /// representation of <paramref name="arg0"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="format"/> is <c>null</c>.</exception>
        /// <exception cref="FormatException">
        /// <paramref name="format"/> is invalid.
        /// <para/>
        /// -or-
        /// <para/>
        /// The index of a format item is less than 0 (zero), or greater than or equal to 1 (one).
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">The length of the expanded string would exceed <see cref="MaxCapacity"/>.</exception>
        public StringBuffer AppendFormat(string format, object? arg0)
        {
            lock (syncRoot)
            {
                builder.AppendFormat(format, arg0);
                return this;
            }
        }

        /// <summary>
        /// Appends the string returned by processing a composite format string,
        /// which contains zero or more format items, to this instance. Each
        /// format item is replaced by the string representation of a single
        /// argument using a specified format provider.
        /// </summary>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <param name="format">A composite format string.</param>
        /// <param name="arg0">An object to format.</param>
        /// <returns>A reference to this instance with <paramref name="format"/> appended.
        /// After the append operation, this instance contains any data that existed before
        /// the operation, suffixed by a copy of <paramref name="format"/> in which any format specification 
        /// is replaced by the string representation of <paramref name="arg0"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="format"/> is <c>null</c>.</exception>
        /// <exception cref="FormatException">
        /// <paramref name="format"/> is invalid.
        /// <para/>
        /// -or-
        /// <para/>
        /// The index of a format item is less than 0 (zero), or greater than or equal to 1 (one).
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">The length of the expanded string would exceed <see cref="MaxCapacity"/>.</exception>
        public StringBuffer AppendFormat(IFormatProvider? provider, string format, object? arg0)
        {
            lock (syncRoot)
            {
                builder.AppendFormat(provider, format, arg0);
                return this;
            }
        }

        /// <summary>
        /// Appends the string returned by processing a composite format string,
        /// which contains zero or more format items, to this instance. Each
        /// format item is replaced by the string representation of either of two arguments.
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="arg0">The first object to format.</param>
        /// <param name="arg1">The second object to format.</param>
        /// <returns>A reference to this instance with <paramref name="format"/> appended.
        /// Each format item in <paramref name="format"/> is replaced by the string
        /// representation of the corresponding object argument.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="format"/> is <c>null</c>.</exception>
        /// <exception cref="FormatException">
        /// <paramref name="format"/> is invalid.
        /// <para/>
        /// -or-
        /// <para/>
        /// The index of a format item is less than 0 (zero), or greater than or equal to 2 (two).
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">The length of the expanded string would exceed <see cref="MaxCapacity"/>.</exception>
        public StringBuffer AppendFormat(string format, object? arg0, object? arg1)
        {
            lock (syncRoot)
            {
                builder.AppendFormat(format, arg0, arg1);
                return this;
            }
        }

        /// <summary>
        /// Appends the string returned by processing a composite format string,
        /// which contains zero or more format items, to this instance. Each
        /// format item is replaced by the string representation of either
        /// of two arguments using a specified format provider.
        /// </summary>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <param name="format">A composite format string.</param>
        /// <param name="arg0">The first object to format.</param>
        /// <param name="arg1">The second object to format.</param>
        /// <returns>A reference to this instance with <paramref name="format"/> appended.
        /// After the append operation, this instance contains any data that existed before
        /// the operation, suffixed by a copy of <paramref name="format"/> in which any format specification 
        /// is replaced by the string representation of the corresponding object argument.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="format"/> is <c>null</c>.</exception>
        /// <exception cref="FormatException">
        /// <paramref name="format"/> is invalid.
        /// <para/>
        /// -or-
        /// <para/>
        /// The index of a format item is less than 0 (zero), or greater than or equal to 2 (two).
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">The length of the expanded string would exceed <see cref="MaxCapacity"/>.</exception>
        public StringBuffer AppendFormat(IFormatProvider? provider, string format, object? arg0, object? arg1)
        {
            lock (syncRoot)
            {
                builder.AppendFormat(provider, format, arg0, arg1);
                return this;
            }
        }

        /// <summary>
        /// Appends the string returned by processing a composite format string,
        /// which contains zero or more format items, to this instance. Each
        /// format item is replaced by the string representation of any of three arguments.
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="arg0">The first object to format.</param>
        /// <param name="arg1">The second object to format.</param>
        /// <param name="arg2">The third object to format.</param>
        /// <returns>A reference to this instance with <paramref name="format"/> appended.
        /// Each format item in <paramref name="format"/> is replaced by the string
        /// representation of the corresponding object argument.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="format"/> is <c>null</c>.</exception>
        /// <exception cref="FormatException">
        /// <paramref name="format"/> is invalid.
        /// <para/>
        /// -or-
        /// <para/>
        /// The index of a format item is less than 0 (zero), or greater than or equal to 3 (three).
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">The length of the expanded string would exceed <see cref="MaxCapacity"/>.</exception>
        public StringBuffer AppendFormat(string format, object? arg0, object? arg1, object? arg2)
        {
            lock (syncRoot)
            {
                builder.AppendFormat(format, arg0, arg1, arg2);
                return this;
            }
        }

        /// <summary>
        /// Appends the string returned by processing a composite format string,
        /// which contains zero or more format items, to this instance. Each
        /// format item is replaced by the string representation of any
        /// of three arguments using a specified format provider.
        /// </summary>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <param name="format">A composite format string.</param>
        /// <param name="arg0">The first object to format.</param>
        /// <param name="arg1">The second object to format.</param>
        /// <param name="arg2">The third object to format.</param>
        /// <returns>A reference to this instance with <paramref name="format"/> appended.
        /// After the append operation, this instance contains any data that existed before
        /// the operation, suffixed by a copy of <paramref name="format"/> in which any format specification 
        /// is replaced by the string representation of the corresponding object argument.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="format"/> is <c>null</c>.</exception>
        /// <exception cref="FormatException">
        /// <paramref name="format"/> is invalid.
        /// <para/>
        /// -or-
        /// <para/>
        /// The index of a format item is less than 0 (zero), or greater than or equal to 3 (three).
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">The length of the expanded string would exceed <see cref="MaxCapacity"/>.</exception>
        public StringBuffer AppendFormat(IFormatProvider? provider, string format, object? arg0, object? arg1, object? arg2)
        {
            lock (syncRoot)
            {
                builder.AppendFormat(provider, format, arg0, arg1, arg2);
                return this;
            }
        }

        /// <summary>
        /// Appends the string returned by processing a composite format string,
        /// which contains zero or more format items, to this instance. Each format
        /// item is replaced by the string representation of a corresponding argument
        /// in a parameter array.
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">An array of objects to format.</param>
        /// <returns>A reference to this instance with <paramref name="format"/> appended.
        /// Each format item in <paramref name="format"/> is replaced by the string representation
        /// of the corresponding object argument.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="format"/> or <paramref name="args"/> is <c>null</c>.</exception>
        /// <exception cref="FormatException">
        /// <paramref name="format"/> is invalid.
        /// <para/>
        /// -or-
        /// <para/>
        /// The index of a format item is less than 0 (zero), or greater than or equal to the
        /// length of the <paramref name="args"/> array.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">The length of the expanded string would exceed <see cref="MaxCapacity"/>.</exception>
        public StringBuffer AppendFormat(string format, params object?[] args)
        {
            lock (syncRoot)
            {
                builder.AppendFormat(format, args);
                return this;
            }
        }

        /// <summary>
        /// Appends the string returned by processing a composite format string, which contains zero or
        /// more format items, to this instance. Each format item is replaced by the string representation
        /// of a corresponding argument in a parameter array using a specified format provider.
        /// </summary>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">An array of objects to format.</param>
        /// <returns>A reference to this instance with <paramref name="format"/> appended.
        /// After the append operation, this instance contains any data that existed before
        /// the operation, suffixed by a copy of <paramref name="format"/> where any format specification is
        /// replaced by the string representation of the corresponding object argument.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="format"/> is <c>null</c>.</exception>
        /// <exception cref="FormatException">
        /// <paramref name="format"/> is invalid.
        /// <para/>
        /// -or-
        /// <para/>
        /// The index of a format item is less than 0 (zero), or greater than or equal to the
        /// length of the <paramref name="args"/> array.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">The length of the expanded string would exceed <see cref="MaxCapacity"/>.</exception>
        public StringBuffer AppendFormat(IFormatProvider? provider, string format, params object?[] args)
        {
            lock (syncRoot)
            {
                builder.AppendFormat(provider, format, args);
                return this;
            }
        }

        #endregion

        #region AppendJoin

#if FEATURE_STRINGBUILDER_APPENDJOIN

        /// <summary>
        /// Concatenates the string representations of the elements in the
        /// provided array of objects, using the specified separator between
        /// each member, then appends the result to the current instance of
        /// the string builder.
        /// </summary>
        /// <param name="separator">The character to use as a separator.
        /// <paramref name="separator"/> is included in the joined strings
        /// only if <paramref name="values"/> has more than one element.</param>
        /// <param name="values">An array that contains the strings to
        /// concatenate and append to the current instance of the string builder.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        public StringBuffer AppendJoin(char separator, params object?[] values)
        {
            lock (syncRoot)
            {
                builder.AppendJoin(separator, values);
                return this;
            }
        }

        /// <summary>
        /// Concatenates the strings of the provided array, using the
        /// specified char separator between each string, then appends
        /// the result to the current instance of the string builder.
        /// </summary>
        /// <param name="separator">The character to use as a separator.
        /// <paramref name="separator"/> is included in the joined strings only if values
        /// has more than one element.</param>
        /// <param name="values">An array that contains the strings to
        /// concatenate and append to the current instance of the string builder.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        public StringBuffer AppendJoin(char separator, params string?[] values)
        {
            lock (syncRoot)
            {
                builder.AppendJoin(separator, values);
                return this;
            }
        }

        /// <summary>
        /// Concatenates the string representations of the elements in the
        /// provided array of objects, using the specified separator between
        /// each member, then appends the result to the current instance of
        /// the string builder.
        /// </summary>
        /// <param name="separator">The string to use as a separator.
        /// <paramref name="separator"/> is included in the joined strings
        /// only if <paramref name="values"/> has more than one element.</param>
        /// <param name="values">An array that contains the strings to
        /// concatenate and append to the current instance of the string builder.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        public StringBuffer AppendJoin(string separator, params object?[] values)
        {
            lock (syncRoot)
            {
                builder.AppendJoin(separator, values);
                return this;
            }
        }

        /// <summary>
        /// Concatenates the strings of the provided array, using the specified
        /// separator between each string, then appends the result to the current
        /// instance of the string builder.
        /// </summary>
        /// <param name="separator">The string to use as a separator.
        /// <paramref name="separator"/> is included in the joined strings only if values
        /// has more than one element.</param>
        /// <param name="values">An array that contains the strings to concatenate
        /// and append to the current instance of the string builder.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        public StringBuffer AppendJoin(string separator, params string?[] values)
        {
            lock (syncRoot)
            {
                builder.AppendJoin(separator, values);
                return this;
            }
        }

        /// <summary>
        /// Concatenates and appends the members of a collection,
        /// using the specified char separator between each member.
        /// </summary>
        /// <typeparam name="T">The type of the members of <paramref name="values"/>.</typeparam>
        /// <param name="separator">The character to use as a separator.
        /// <paramref name="separator"/> is included in the concatenated and appended strings
        /// only if values has more than one element.</param>
        /// <param name="values">A collection that contains the objects to concatenate
        /// and append to the current instance of the string builder.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        public StringBuffer AppendJoin<T>(char separator, IEnumerable<T> values)
        {
            lock (syncRoot)
            {
                builder.AppendJoin(separator, values);
                return this;
            }
        }

        /// <summary>
        /// Concatenates and appends the members of a collection,
        /// using the specified separator between each member.
        /// </summary>
        /// <typeparam name="T">The type of the members of <paramref name="values"/>.</typeparam>
        /// <param name="separator">The string to use as a separator.
        /// <paramref name="separator"/> is included in the concatenated and appended
        /// strings only if values has more than one element.</param>
        /// <param name="values">A collection that contains the objects to concatenate
        /// and append to the current instance of the string builder.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        public StringBuffer AppendJoin<T>(string? separator, IEnumerable<T> values)
        {
            lock (syncRoot)
            {
                builder.AppendJoin(separator, values);
                return this;
            }
        }
#endif

        #endregion

        #region AppendLine

        /// <summary>
        /// Appends the default line terminator to the end of the current <see cref="StringBuffer"/> object.
        /// </summary>
        /// <remarks>
        /// The default line terminator is the current value of the <see cref="Environment.NewLine"/> property.
        /// <para/>
        /// The capacity of this instance is adjusted as needed.
        /// </remarks>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Enlarging the value of this instance would exceed <see cref="MaxCapacity"/>.</exception>
        public StringBuffer AppendLine()
        {
            lock (syncRoot)
            {
                builder.AppendLine();
                return this;
            }
        }

        /// <summary>
        /// Appends a copy of the specified string followed by the default line
        /// terminator to the end of the current <see cref="StringBuffer"/> object.
        /// </summary>
        /// <remarks>
        /// The default line terminator is the current value of the <see cref="Environment.NewLine"/> property.
        /// <para/>
        /// The capacity of this instance is adjusted as needed.
        /// </remarks>
        /// <param name="value">The string to append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Enlarging the value of this instance would exceed <see cref="MaxCapacity"/>.</exception>
        public StringBuffer AppendLine(string? value)
        {
            lock (syncRoot)
            {
                builder.AppendLine(value);
                return this;
            }
        }

        #endregion

        #region Clear

        /// <summary>
        /// Removes all characters from the current <see cref="StringBuffer"/> instance.
        /// </summary>
        /// <returns>An object whose <see cref="Length"/> is 0 (zero).</returns>
        public StringBuffer Clear()
        {
            lock (syncRoot)
            {
                builder.Clear();
                return this;
            }
        }

        #endregion Clear

        #region CopyTo

        /// <summary>
        /// Copies the characters from a specified segment of this instance to a specified segment of a destination <see cref="char"/> array.
        /// </summary>
        /// <param name="sourceIndex">The starting position in this instance where characters will be copied from. The index is zero-based.</param>
        /// <param name="destination">The array where characters will be copied.</param>
        /// <param name="destinationIndex">The starting position in <paramref name="destination"/> where characters will be copied. The index is zero-based.</param>
        /// <param name="count">The number of characters to be copied.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="destination"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="sourceIndex"/>, <paramref name="destinationIndex"/>, or count, is less than zero.
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
        public void CopyTo(int sourceIndex, char[] destination, int destinationIndex, int count) // J2N: Use instead of GetChars()
        {
            lock (syncRoot)
                builder.CopyTo(sourceIndex, destination, destinationIndex, count);
        }

        #endregion

        #region Delete

        /// <summary>
        /// Deletes a sequence of characters specified by <paramref name="startIndex"/> and <paramref name="count"/>.
        /// Shifts any remaining characters to the left.
        /// <para/>
        /// Usage Note: This method has .NET semantics. That is, the 3rd parameter is a count rather than
        /// an exclusive end index. To translate from Java, use <c>end - start</c> for <paramref name="count"/>.
        /// <para/>
        /// This method differs from <see cref="StringBuilder.Remove(int, int)"/> in that it will automatically
        /// adjust the <paramref name="count"/> if <c><paramref name="startIndex"/> + <paramref name="count"/> > <see cref="Length"/></c>
        /// to <c><see cref="Length"/> - <paramref name="startIndex"/>.</c>, provided it is not bounded by <see cref="MaxCapacity"/>.
        /// </summary>
        /// <param name="startIndex">The start index in this instance.</param>
        /// <param name="count">The number of characters to delete in this instance.</param>
        /// <returns>This <see cref="StringBuilder"/>, for chaining.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startIndex"/> or <paramref name="count"/> is less than zero.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="startIndex"/> is greater than or equal to <see cref="Length"/>.
        /// </exception>
        public StringBuffer Delete(int startIndex, int count)
        {
            lock (syncRoot)
            {
                builder.Delete(startIndex, count);
                return this;
            }
        }

        #endregion Delete

        #region EnsureCapacity

        /// <summary>
        /// Ensures that the capacity of this instance of <see cref="StringBuffer"/>
        /// is at least the specified value.
        /// </summary>
        /// <param name="capacity">The minimum capacity to ensure.</param>
        /// <returns>The new capacity of this instance.</returns>
        public int EnsureCapacity(int capacity)
        {
            lock (syncRoot)
                return builder.EnsureCapacity(capacity);
        }

        #endregion

        #region Equals

#if FEATURE_STRINGBUILDER_EQUALS_READONLYSPAN

        /// <summary>
        /// Returns a value indicating whether this instance is equal to a specified object.
        /// </summary>
        /// <param name="span">An object to compare with this instance, or <c>null</c>.</param>
        /// <returns><c>true</c> if this instance and sb have equal string; otherwise, <c>false</c>.</returns>
        public bool Equals(ReadOnlySpan<char> span)
        {
            lock (syncRoot)
                return builder.Equals(span);
        }
#endif

        /// <summary>
        /// Returns a value indicating whether this instance is equal to a specified object.
        /// </summary>
        /// <param name="other">An object to compare with this instance, or <c>null</c>.</param>
        /// <returns><c>true</c> if this instance and sb have equal string, <see cref="Capacity"/>,
        /// and <see cref="MaxCapacity"/> values; otherwise, <c>false</c>.</returns>
        public bool Equals(StringBuilderCharSequence? other)
        {
            if (other is null || !other.HasValue)
                return false;

            return Equals(other.Value);
        }

        /// <summary>
        /// Returns a value indicating whether this instance is equal to a specified object.
        /// </summary>
        /// <param name="other">An object to compare with this instance, or <c>null</c>.</param>
        /// <returns><c>true</c> if this instance and sb have equal string, <see cref="Capacity"/>,
        /// and <see cref="MaxCapacity"/> values; otherwise, <c>false</c>.</returns>
        public bool Equals(StringBuilder? other)
        {
            lock (syncRoot)
                return builder.Equals(other);
        }

        /// <summary>
        /// Returns a value indicating whether this instance is equal to a specified object.
        /// </summary>
        /// <param name="other">An object to compare with this instance, or <c>null</c>.</param>
        /// <returns><c>true</c> if this instance and sb have equal string, <see cref="Capacity"/>,
        /// and <see cref="MaxCapacity"/> values; otherwise, <c>false</c>.</returns>
        public bool Equals(StringBuffer? other)
        {
            return Equals(other?.builder);
        }

        /// <summary>
        /// Returns a value indicating whether this instance is equal to a specified object.
        /// </summary>
        /// <param name="obj">An object to compare with this instance, or <c>null</c>.</param>
        /// <returns><c>true</c> if this instance and sb have equal string, <see cref="Capacity"/>,
        /// and <see cref="MaxCapacity"/> values; otherwise, <c>false</c>.</returns>
        public override bool Equals(object? obj)
        {
            if (obj is null) return false;

            if (obj is StringBuffer stringBuffer)
                return Equals(stringBuffer);
            if (obj is StringBuilder stringBuilder)
                return Equals(stringBuilder);
            if (obj is StringBuilderCharSequence stringBuilder1)
                return Equals(stringBuilder1);

            lock (syncRoot)
                return builder.Equals(obj);
        }

        #endregion Equals

        #region GetHashCode

        /// <summary>
        /// Returns a hash code for the builder.
        /// </summary>
        /// <returns>A hash code for the builder.</returns>
        public override int GetHashCode()
        {
            lock (syncRoot)
                return builder.GetHashCode();
        }

        #endregion GetHashCode

        #region Insert

#if FEATURE_STRINGBUILDER_INSERT_READONLYSPAN

        /// <summary>
        /// Inserts the string representation of a <see cref="ReadOnlySpan{Char}"/> value into this instance at the specified character position.
        /// </summary>
        /// <param name="index">The position in this instance where insertion begins.</param>
        /// <param name="value">The value to insert.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than zero or greater than the length of this instance.</exception>
        /// <exception cref="OutOfMemoryException">Enlarging the value of this instance would exceed <see cref="MaxCapacity"/>.</exception>
        public StringBuffer Insert(int index, ReadOnlySpan<char> value)
        {
            lock (syncRoot)
            {
                builder.Insert(index, value);
                return this;
            }
        }

#endif

        /// <summary>
        /// Inserts the string representation of a Boolean value into this instance at the specified character position.
        /// </summary>
        /// <param name="index">The position in this instance where insertion begins.</param>
        /// <param name="value">The value to insert.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than zero or greater than the length of this instance.</exception>
        /// <exception cref="OutOfMemoryException">Enlarging the value of this instance would exceed <see cref="MaxCapacity"/>.</exception>
        public StringBuffer Insert(int index, bool value)
        {
            lock (syncRoot)
            {
                builder.Insert(index, value);
                return this;
            }
        }

        /// <summary>
        /// Inserts the string representation of a specified 8-bit unsigned integer into this instance at the specified character position.
        /// </summary>
        /// <param name="index">The position in this instance where insertion begins.</param>
        /// <param name="value">The value to insert.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than zero or greater than the length of this instance.</exception>
        /// <exception cref="OutOfMemoryException">Enlarging the value of this instance would exceed <see cref="MaxCapacity"/>.</exception>
        public StringBuffer Insert(int index, byte value)
        {
            lock (syncRoot)
            {
                builder.Insert(index, value);
                return this;
            }
        }

        /// <summary>
        /// Inserts the string representation of a specified Unicode character into this instance at the specified character position.
        /// </summary>
        /// <param name="index">The position in this instance where insertion begins.</param>
        /// <param name="value">The character array to insert.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than zero or greater than the length of this instance.
        /// <para/>
        /// -or-
        /// <para/>
        /// Enlarging the value of this instance would exceed <see cref="MaxCapacity"/>.
        /// </exception>
        public StringBuffer Insert(int index, char value)
        {
            lock (syncRoot)
            {
                builder.Insert(index, value);
                return this;
            }
        }

        /// <summary>
        /// Inserts the string representation of a specified array of Unicode characters into this instance at the specified character position.
        /// </summary>
        /// <param name="index">The position in this instance where insertion begins.</param>
        /// <param name="value">The character array to insert.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than zero or greater than the length of this instance.
        /// <para/>
        /// -or-
        /// <para/>
        /// Enlarging the value of this instance would exceed <see cref="MaxCapacity"/>.
        /// </exception>
        public StringBuffer Insert(int index, char[]? value)
        {
            lock (syncRoot)
            {
                builder.Insert(index, value);
                return this;
            }
        }

        /// <summary>
        /// Inserts the string representation of a specified subarray of Unicode characters into this instance at the specified character position.
        /// </summary>
        /// <param name="index">The position in this instance where insertion begins.</param>
        /// <param name="value">A character array.</param>
        /// <param name="startIndex">The starting index within <paramref name="value"/>.</param>
        /// <param name="charCount">The number of characters to insert.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
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
        /// Enlarging the value of this instance would exceed <see cref="MaxCapacity"/>.
        /// </exception>
        public StringBuffer Insert(int index, char[]? value, int startIndex, int charCount)
        {
            lock (syncRoot)
            {
                builder.Insert(index, value, startIndex, charCount);
                return this;
            }
        }

        /// <summary>
        /// Inserts the string representation of a specified sequence of Unicode characters into this instance at the specified character position.
        /// </summary>
        /// <param name="index">The position in this instance where insertion begins.</param>
        /// <param name="charSequence">The character sequence to insert.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than zero or greater than the length of this instance.
        /// <para/>
        /// -or-
        /// <para/>
        /// Enlarging the value of this instance would exceed <see cref="MaxCapacity"/>.
        /// </exception>
        public StringBuffer Insert(int index, ICharSequence? charSequence)
        {
            if (charSequence is StringCharSequence stringCharSequence)
                return Insert(index, stringCharSequence?.Value);

            lock (syncRoot)
            {
                builder.Insert(index, charSequence: charSequence);
                return this;
            }
        }

        /// <summary>
        /// Inserts the string representation of a specified subarray of Unicode characters into this instance at the specified character position.
        /// <para/>
        /// IMPORTANT: This method has .NET semantics. That is, the fourth parameter is a count, not an exclusive end index as would be the
        /// case in Java. To translate from Java, use <c>end - start</c> to resolve <paramref name="charCount"/>.
        /// </summary>
        /// <param name="index">The position in this instance where insertion begins.</param>
        /// <param name="charSequence">A character array.</param>
        /// <param name="startIndex">The starting index within <paramref name="charSequence"/>.</param>
        /// <param name="charCount">The number of characters to insert.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
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
        /// Enlarging the value of this instance would exceed <see cref="MaxCapacity"/>.
        /// </exception>
        public StringBuffer Insert(int index, ICharSequence? charSequence, int startIndex, int charCount)
        {
            lock (syncRoot)
            {
                builder.Insert(index, charSequence, startIndex, charCount);
                return this;
            }
        }

        /// <summary>
        /// Inserts the string representation of a specified sequence of Unicode characters into this instance at the specified character position.
        /// </summary>
        /// <param name="index">The position in this instance where insertion begins.</param>
        /// <param name="charSequence">The character sequence to insert.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than zero or greater than the length of this instance.
        /// <para/>
        /// -or-
        /// <para/>
        /// Enlarging the value of this instance would exceed <see cref="MaxCapacity"/>.
        /// </exception>
        public StringBuffer Insert(int index, StringBuilder? charSequence)
        {
            lock (syncRoot)
            {
                builder.Insert(index, charSequence: charSequence);
                return this;
            }
        }

        /// <summary>
        /// Inserts the string representation of a specified subarray of Unicode characters into this instance at the specified character position.
        /// <para/>
        /// IMPORTANT: This method has .NET semantics. That is, the fourth parameter is a count, not an exclusive end index as would be the
        /// case in Java. To translate from Java, use <c>end - start</c> to resolve <paramref name="charCount"/>.
        /// </summary>
        /// <param name="index">The position in this instance where insertion begins.</param>
        /// <param name="charSequence">A character array.</param>
        /// <param name="startIndex">The starting index within <paramref name="charSequence"/>.</param>
        /// <param name="charCount">The number of characters to insert.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
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
        /// Enlarging the value of this instance would exceed <see cref="MaxCapacity"/>.
        /// </exception>
        public StringBuffer Insert(int index, StringBuilder? charSequence, int startIndex, int charCount)
        {
            lock (syncRoot)
            {
                builder.Insert(index, charSequence, startIndex, charCount);
                return this;
            }
        }

        /// <summary>
        /// Inserts one or more copies of a specified string into this instance at the specified character position.
        /// </summary>
        /// <param name="index">The position in this instance where insertion begins.</param>
        /// <param name="value">The string to insert.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than zero or greater than the length of this instance.
        /// <para/>
        /// -or-
        /// <para/>
        /// Enlarging the value of this instance would exceed <see cref="MaxCapacity"/>.
        /// </exception>
        public StringBuffer Insert(int index, string? value)
        {
            lock (syncRoot)
            {
                builder.Insert(index, value);
                return this;
            }
        }

        /// <summary>
        /// Inserts the string representation of a specified subarray of Unicode characters into this instance at the specified character position.
        /// <para/>
        /// IMPORTANT: This method has .NET semantics. That is, the fourth parameter is a count, not an exclusive end index as would be the
        /// case in Java. To translate from Java, use <c>end - start</c> to resolve <paramref name="charCount"/>.
        /// </summary>
        /// <param name="index">The position in this instance where insertion begins.</param>
        /// <param name="value">A character array.</param>
        /// <param name="startIndex">The starting index within <paramref name="value"/>.</param>
        /// <param name="charCount">The number of characters to insert.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
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
        /// Enlarging the value of this instance would exceed <see cref="MaxCapacity"/>.
        /// </exception>
        public StringBuffer Insert(int index, string? value, int startIndex, int charCount)
        {
            lock (syncRoot)
            {
                builder.Insert(index, value, startIndex, charCount);
                return this;
            }
        }

        /// <summary>
        /// Inserts a string into this instance at the specified character position.
        /// </summary>
        /// <param name="index">The position in this instance where insertion begins.</param>
        /// <param name="value">The string to insert.</param>
        /// <param name="count">The number of times to insert <paramref name="value"/>.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than zero or greater than the length of this instance.</exception>
        /// <exception cref="OutOfMemoryException">Enlarging the value of this instance would exceed <see cref="MaxCapacity"/>.</exception>
        public StringBuffer Insert(int index, string? value, int count)
        {
            lock (syncRoot)
            {
                builder.Insert(index, value, count);
                return this;
            }
        }

        /// <summary>
        /// Inserts the string representation of an object into this instance at the specified character position.
        /// </summary>
        /// <param name="index">The position in this instance where insertion begins.</param>
        /// <param name="value">The value to insert.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than zero or greater than the length of this instance.</exception>
        /// <exception cref="OutOfMemoryException">Enlarging the value of this instance would exceed <see cref="MaxCapacity"/>.</exception>
        public StringBuffer Insert(int index, object? value)
        {
            lock (syncRoot)
            {
                builder.Insert(index, value);
                return this;
            }
        }

        /// <summary>
        /// Inserts the string representation of a specified 32-bit signed integer into this instance at the specified character position.
        /// </summary>
        /// <param name="index">The position in this instance where insertion begins.</param>
        /// <param name="value">The value to insert.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than zero or greater than the length of this instance.</exception>
        /// <exception cref="OutOfMemoryException">Enlarging the value of this instance would exceed <see cref="MaxCapacity"/>.</exception>
        public StringBuffer Insert(int index, int value)
        {
            lock (syncRoot)
            {
                builder.Insert(index, value);
                return this;
            }
        }

        /// <summary>
        /// Inserts the string representation of a specified 64-bit signed integer into this instance at the specified character position.
        /// </summary>
        /// <param name="index">The position in this instance where insertion begins.</param>
        /// <param name="value">The value to insert.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than zero or greater than the length of this instance.</exception>
        /// <exception cref="OutOfMemoryException">Enlarging the value of this instance would exceed <see cref="MaxCapacity"/>.</exception>
        public StringBuffer Insert(int index, long value)
        {
            lock (syncRoot)
            {
                builder.Insert(index, value);
                return this;
            }
        }

        /// <summary>
        /// Inserts the string representation of a specified 16-bit signed integer into this instance at the specified character position.
        /// </summary>
        /// <param name="index">The position in this instance where insertion begins.</param>
        /// <param name="value">The value to insert.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than zero or greater than the length of this instance.</exception>
        /// <exception cref="OutOfMemoryException">Enlarging the value of this instance would exceed <see cref="MaxCapacity"/>.</exception>
        public StringBuffer Insert(int index, short value)
        {
            lock (syncRoot)
            {
                builder.Insert(index, value);
                return this;
            }
        }

        /// <summary>
        /// Inserts the string representation of a single-precision floating-point number into this instance at the specified character position.
        /// </summary>
        /// <param name="index">The position in this instance where insertion begins.</param>
        /// <param name="value">The value to insert.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than zero or greater than the length of this instance.</exception>
        /// <exception cref="OutOfMemoryException">Enlarging the value of this instance would exceed <see cref="MaxCapacity"/>.</exception>
        public StringBuffer Insert(int index, float value)
        {
            lock (syncRoot)
            {
                builder.Insert(index, value);
                return this;
            }
        }

        /// <summary>
        /// Inserts the string representation of a double-precision floating-point number into this instance at the specified character position.
        /// </summary>
        /// <param name="index">The position in this instance where insertion begins.</param>
        /// <param name="value">The value to insert.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than zero or greater than the length of this instance.</exception>
        /// <exception cref="OutOfMemoryException">Enlarging the value of this instance would exceed <see cref="MaxCapacity"/>.</exception>
        public StringBuffer Insert(int index, double value)
        {
            lock (syncRoot)
            {
                builder.Insert(index, value);
                return this;
            }
        }

        /// <summary>
        /// Inserts the string representation of a decimal number into this instance at the specified character position.
        /// </summary>
        /// <param name="index">The position in this instance where insertion begins.</param>
        /// <param name="value">The value to insert.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than zero or greater than the length of this instance.</exception>
        /// <exception cref="OutOfMemoryException">Enlarging the value of this instance would exceed <see cref="MaxCapacity"/>.</exception>
        public StringBuffer Insert(int index, decimal value)
        {
            lock (syncRoot)
            {
                builder.Insert(index, value);
                return this;
            }
        }

        /// <summary>
        /// Inserts the string representation of a specified 8-bit signed integer into this instance at the specified character position.
        /// </summary>
        /// <param name="index">The position in this instance where insertion begins.</param>
        /// <param name="value">The value to insert.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than zero or greater than the length of this instance.</exception>
        /// <exception cref="OutOfMemoryException">Enlarging the value of this instance would exceed <see cref="MaxCapacity"/>.</exception>
        [CLSCompliant(false)]
        public StringBuffer Insert(int index, sbyte value)
        {
            lock (syncRoot)
            {
                builder.Insert(index, value);
                return this;
            }
        }

        /// <summary>
        /// Inserts the string representation of a specified 16-bit unsigned integer into this instance at the specified character position.
        /// </summary>
        /// <param name="index">The position in this instance where insertion begins.</param>
        /// <param name="value">The value to insert.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than zero or greater than the length of this instance.</exception>
        /// <exception cref="OutOfMemoryException">Enlarging the value of this instance would exceed <see cref="MaxCapacity"/>.</exception>
        [CLSCompliant(false)]
        public StringBuffer Insert(int index, ushort value)
        {
            lock (syncRoot)
            {
                builder.Insert(index, value);
                return this;
            }
        }

        /// <summary>
        /// Inserts the string representation of a specified 32-bit unsigned integer into this instance at the specified character position.
        /// </summary>
        /// <param name="index">The position in this instance where insertion begins.</param>
        /// <param name="value">The value to insert.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than zero or greater than the length of this instance.</exception>
        /// <exception cref="OutOfMemoryException">Enlarging the value of this instance would exceed <see cref="MaxCapacity"/>.</exception>
        [CLSCompliant(false)]
        public StringBuffer Insert(int index, uint value)
        {
            lock (syncRoot)
            {
                builder.Insert(index, value);
                return this;
            }
        }

        /// <summary>
        /// Inserts the string representation of a specified 64-bit unsigned integer into this instance at the specified character position.
        /// </summary>
        /// <param name="index">The position in this instance where insertion begins.</param>
        /// <param name="value">The value to insert.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than zero or greater than the length of this instance.</exception>
        /// <exception cref="OutOfMemoryException">Enlarging the value of this instance would exceed <see cref="MaxCapacity"/>.</exception>
        [CLSCompliant(false)]
        public StringBuffer Insert(int index, ulong value)
        {
            lock (syncRoot)
            {
                builder.Insert(index, value);
                return this;
            }
        }

        #endregion Insert

        #region IndexOf

        /// <summary>
        /// Searches for the index of the specified character. The search for the
        /// character starts at the specified offset and moves towards the end.
        /// </summary>
        /// <param name="value">The string to find.</param>
        /// <param name="comparisonType">One of the enumeration values that specifies the rules for the search.</param>
        /// <returns>The index of the specified character, or <c>-1</c> if the character isn't found.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="value"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException"><paramref name="comparisonType"/> is not a <see cref="StringComparison"/> value.</exception>
        public int IndexOf(string value, StringComparison comparisonType)
        {
            lock (syncRoot)
                return builder.IndexOf(value, 0, comparisonType);
        }

        /// <summary>
        /// Searches for the index of the specified character. The search for the
        /// character starts at the specified offset and moves towards the end.
        /// </summary>
        /// <param name="value">The string to find.</param>
        /// <param name="startIndex">The starting offset.</param>
        /// <param name="comparisonType">One of the enumeration values that specifies the rules for the search.</param>
        /// <returns>The index of the specified character, or <c>-1</c> if the character isn't found.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="value"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="startIndex"/> is less than 0 (zero) or greater than the length of this <see cref="StringBuilder"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="comparisonType"/> is not a <see cref="StringComparison"/> value.</exception>
        public int IndexOf(string value, int startIndex, StringComparison comparisonType)
        {
            lock (syncRoot)
                return builder.IndexOf(value, startIndex, comparisonType);
        }

        #endregion IndexOf

        #region LastIndexOf

        /// <summary>
        /// Searches for the index of the specified character. The search for the
        /// character starts at the specified offset and moves towards the beginning.
        /// </summary>
        /// <param name="value">The string to find.</param>
        /// <param name="comparisonType">One of the enumeration values that specifies the rules for the search.</param>
        /// <returns>The index of the specified character, <c>-1</c> if the character isn't found.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="value"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException"><paramref name="comparisonType"/> is not a <see cref="StringComparison"/> value.</exception>
        public int LastIndexOf(string value, StringComparison comparisonType)
        {
            lock (syncRoot)
                return builder.LastIndexOf(value, builder.Length, comparisonType);
        }

        /// <summary>
        /// Searches for the index of the specified character. The search for the
        /// character starts at the specified offset and moves towards the beginning.
        /// </summary>
        /// <param name="value">The string to find.</param>
        /// <param name="startIndex">The starting offset.</param>
        /// <param name="comparisonType">One of the enumeration values that specifies the rules for the search.</param>
        /// <returns>The index of the specified character, <c>-1</c> if the character isn't found.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="value"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="startIndex"/> is less than 0 (zero) or greater than the length of this <see cref="StringBuilder"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="comparisonType"/> is not a <see cref="StringComparison"/> value.</exception>
        public int LastIndexOf(string value, int startIndex, StringComparison comparisonType)
        {
            lock (syncRoot)
                return builder.LastIndexOf(value, startIndex, comparisonType);
        }

        #endregion LastIndexOf

        #region Remove

        /// <summary>
        /// Removes the specified range of characters from this instance.
        /// <para/>
        /// Usage Note: This method differs from <see cref="Delete(int, int)"/> in strictness.
        /// This method throws exceptions if the bounds are not exact, where <see cref="Delete(int, int)"/>
        /// will make a best effort to try to correct the bounds to ensure the operation is successful.
        /// </summary>
        /// <param name="startIndex">The zero-based position in this instance where removal begins.</param>
        /// <param name="length">The number of characters to remove.</param>
        /// <returns>A reference to this instance after the excise operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="startIndex"/> or <paramref name="length"/> is less than zero,
        /// or <paramref name="startIndex"/> + <paramref name="length"/> is greater than the length of this instance.</exception>
        public StringBuffer Remove(int startIndex, int length)
        {
            lock (syncRoot)
            {
                builder.Remove(startIndex, length);
                return this;
            }
        }


        #endregion

        #region Replace

        /// <summary>
        /// Replaces the specified subsequence in this builder with the specified
        /// string, <paramref name="newValue"/>. The substring begins at the specified
        /// <paramref name="startIndex"/> and ends to the character at 
        /// <c><paramref name="count"/> - <paramref name="startIndex"/></c> or
        /// to the end of the sequence if no such character exists. First the
        /// characters in the substring ar removed and then the specified 
        /// <paramref name="newValue"/> is inserted at <paramref name="startIndex"/>.
        /// This <see cref="StringBuffer"/> will be lengthened to accommodate the
        /// specified <paramref name="newValue"/> if necessary.
        /// <para/>
        /// IMPORTANT: This method has .NET semantics. That is, the third parameter is a count
        /// rather than an exclusive end index. To translate from Java, use <c>end - start</c>
        /// to resolve the <paramref name="count"/> parameter.
        /// </summary>
        /// <param name="startIndex">The inclusive begin index in this instance.</param>
        /// <param name="count">The number of characters to replace.</param>
        /// <param name="newValue">The replacement string.</param>
        /// <returns>This <see cref="StringBuffer"/> builder.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startIndex"/> or <paramref name="count"/> is less than zero.
        /// <para/>
        /// -or-
        /// <para/>
        /// Enlarging the value of this instance would exceed <see cref="StringBuilder.MaxCapacity"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">If <paramref name="newValue"/> is <c>null</c>.</exception>
        public StringBuffer Replace(int startIndex, int count, string newValue)
        {
            lock (syncRoot)
            {
                builder.Replace(startIndex, count, newValue);
                return this;
            }
        }

        /// <summary>
        /// Replaces all occurrences of a specified character in this instance with another specified character.
        /// </summary>
        /// <param name="oldChar">The character to replace.</param>
        /// <param name="newChar">The character that replaces <paramref name="oldChar"/>.</param>
        /// <returns>A reference to this instance with <paramref name="oldChar"/> replaced by <paramref name="newChar"/>.</returns>
        public StringBuffer Replace(char oldChar, char newChar)
        {
            lock (syncRoot)
            {
                builder.Replace(oldChar, newChar);
                return this;
            }
        }

        /// <summary>
        /// Replaces, within a substring of this instance, all occurrences of a specified character with another specified character.
        /// </summary>
        /// <param name="oldChar">The character to replace.</param>
        /// <param name="newChar">The character that replaces <paramref name="oldChar"/>.</param>
        /// <param name="startIndex">The position in this instance where the substring begins.</param>
        /// <param name="count">The length of the substring.</param>
        /// <returns>A reference to this instance with <paramref name="oldChar"/> replaced by <paramref name="newChar"/>
        /// in the range from <paramref name="startIndex"/> to <paramref name="startIndex"/> + <paramref name="count"/> - 1.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startIndex"/> + <paramref name="count"/> is greater than the length of the value of this instance.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="startIndex"/> or <paramref name="count"/> is less than zero.
        /// </exception>
        public StringBuffer Replace(char oldChar, char newChar, int startIndex, int count)
        {
            lock (syncRoot)
            {
                builder.Replace(oldChar, newChar, startIndex, count);
                return this;
            }
        }

        /// <summary>
        /// Replaces all occurrences of a specified string in this instance with another specified string.
        /// </summary>
        /// <param name="oldValue">The string to replace.</param>
        /// <param name="newValue">The string that replaces <paramref name="oldValue"/>, or <c>null</c>.</param>
        /// <returns>A reference to this instance with all instances of <paramref name="oldValue"/> replaced by <paramref name="newValue"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="oldValue"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The length of <paramref name="oldValue"/> is zero.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Enlarging the value of this instance would exceed <see cref="MaxCapacity"/>.</exception>
        public StringBuffer Replace(string oldValue, string? newValue)
        {
            lock (syncRoot)
            {
                builder.Replace(oldValue, newValue);
                return this;
            }
        }

        /// <summary>
        /// Replaces, within a substring of this instance, all occurrences of a specified string with another specified string.
        /// </summary>
        /// <param name="oldValue">The string to replace.</param>
        /// <param name="newValue">The string that replaces <paramref name="oldValue"/>, or <c>null</c>.</param>
        /// <param name="startIndex">The position in this instance where the substring begins.</param>
        /// <param name="count">The length of the substring.</param>
        /// <returns>A reference to this instance with <paramref name="oldValue"/> replaced by <paramref name="newValue"/>
        /// in the range from <paramref name="startIndex"/> to <paramref name="startIndex"/> + <paramref name="count"/> - 1.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="oldValue"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The length of <paramref name="oldValue"/> is zero.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startIndex"/>  or <paramref name="count"/> is less than zero.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="startIndex"/>  plus <paramref name="count"/> indicates a character position not within this instance.
        /// <para/>
        /// -or-
        /// <para/>
        /// Enlarging the value of this instance would exceed <see cref="MaxCapacity"/>.
        /// </exception>
        public StringBuffer Replace(string oldValue, string? newValue, int startIndex, int count)
        {
            lock (syncRoot)
            {
                builder.Replace(oldValue, newValue, startIndex, count);
                return this;
            }
        }

        #endregion

        #region Reverse

        /// <summary>
        /// Causes this character sequence to be replaced by the reverse of
        /// the sequence. If there are any surrogate pairs included in the
        /// sequence, these are treated as single characters for the
        /// reverse operation. Thus, the order of the high-low surrogates
        /// is never reversed.
        /// <para/>
        /// IMPORTANT: This operation is done in-place. Although a <see cref="StringBuffer"/>
        /// is returned, it is a reference to the current instance.
        /// <para/>
        /// Let <c>n</c> be the character length of this character sequence
        /// (not the length in <see cref="char"/> values) just prior to
        /// execution of the <see cref="Reverse()"/> method. Then the
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
        /// <returns>A reference to this <see cref="StringBuffer"/>, for chaining.</returns>
        public StringBuffer Reverse()
        {
            lock (syncRoot)
            {
                builder.Reverse();
                return this;
            }
        }

        #endregion Reverse

        #region Subsequence

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
            lock (syncRoot)
                return builder.Subsequence(startIndex, length);
        }

        #endregion

        #region ToString

        /// <summary>
        /// Converts the value of this instance to a <see cref="string"/>.
        /// </summary>
        /// <returns>A string whose value is the same as this instance.</returns>
        public override string ToString()
        {
            lock (syncRoot)
                return builder.ToString();
        }

        /// <summary>
        /// Converts the value of a substring of this instance to a <see cref="string"/>.
        /// </summary>
        /// <param name="startIndex">The starting position of the substring in this instance.</param>
        /// <param name="length">The length of the substring.</param>
        /// <returns>A string whose value is the same as the specified substring of this instance.</returns>
        public string ToString(int startIndex, int length)
        {
            lock (syncRoot)
                return builder.ToString(startIndex, length);
        }

        #endregion ToString


        #region IAppendable Members

        IAppendable IAppendable.Append(char value) => Append(value);

        IAppendable IAppendable.Append(string? value) => Append(value);

        IAppendable IAppendable.Append(string? value, int startIndex, int count) => Append(value, startIndex, count);

        IAppendable IAppendable.Append(StringBuilder? value) => Append(value);

        IAppendable IAppendable.Append(StringBuilder? value, int startIndex, int count) => Append(value, startIndex, count);

        IAppendable IAppendable.Append(char[]? value) => Append(value);

        IAppendable IAppendable.Append(char[]? value, int startIndex, int count) => Append(value, startIndex, count);

        IAppendable IAppendable.Append(ICharSequence? value) => Append(value);

        IAppendable IAppendable.Append(ICharSequence? value, int startIndex, int count) => Append(value, startIndex, count);

        #endregion IAppendable Members

        #region ICharSequence Members

        char ICharSequence.this[int index] => this[index];

        bool ICharSequence.HasValue => true;

        int ICharSequence.Length => Length;

        ICharSequence ICharSequence.Subsequence(int startIndex, int length) => Subsequence(startIndex, length);

        string ICharSequence.ToString() => ToString();

        #endregion
    }
}
