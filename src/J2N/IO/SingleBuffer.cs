﻿#region Copyright 2010 by Apache Harmony, Licensed under the Apache License, Version 2.0
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
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace J2N.IO
{
    /// <summary>
    /// A buffer of <see cref="float"/>s.
    /// <para/>
    /// A short buffer can be created in either of the following ways:
    /// <list type="bullet">
    ///     <item><description><see cref="Allocate(int)"/> a new <see cref="float"/> array and create a buffer
    ///         based on it</description></item>
    ///     <item><description><see cref="Wrap(float[])"/> an existing <see cref="float"/> array to create a new
    ///         buffer</description></item>
    ///     <item><description>Use <see cref="ByteBuffer.AsSingleBuffer()"/> to create a <see cref="float"/>
    ///         buffer based on a byte buffer.</description></item>
    /// </list>
    /// </summary>
    public abstract class SingleBuffer : Buffer, IComparable<SingleBuffer>
    {
        /// <summary>
        /// Creates a <see cref="SingleBuffer"/> based on a newly allocated <see cref="float"/> array.
        /// </summary>
        /// <param name="capacity">The capacity of the new buffer.</param>
        /// <returns>The created <see cref="SingleBuffer"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="capacity"/> is less than zero.</exception>
        public static SingleBuffer Allocate(int capacity)
        {
            if (capacity < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(capacity, ExceptionArgument.capacity);
            return new ReadWriteSingleArrayBuffer(capacity);
        }

        /// <summary>
        /// Creates a new <see cref="SingleBuffer"/> by wrapping the given <see cref="float"/> array.
        /// <para/>
        /// Calling this method has the same effect as
        /// <c>Wrap(array, 0, array.Length)</c>.
        /// </summary>
        /// <param name="array">The <see cref="float"/> array which the new buffer will be based on.</param>
        /// <returns>The created <see cref="SingleBuffer"/>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="array"/> is <c>null</c>.</exception>
        public static SingleBuffer Wrap(float[] array)
        {
            if (array is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.array);
            return Wrap(array, 0, array.Length);
        }

        /// <summary>
        /// Creates a new <see cref="SingleBuffer"/> by wrapping the given <see cref="float"/> array.
        /// <para/>
        /// The new buffer's position will be <paramref name="startIndex"/>, limit will be
        /// <c>startIndex + length</c>, capacity will be the length of the array.
        /// </summary>
        /// <param name="array">The <see cref="float"/> array which the new buffer will be based on.</param>
        /// <param name="startIndex">The start index, must not be negative and not greater than
        /// <c>array.Length</c>.</param>
        /// <param name="length">The length, must not be negative and not greater than <c>array.Length - start</c>.</param>
        /// <returns>The created <see cref="SingleBuffer"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startIndex"/> plus <paramref name="length"/> indicates a position not within this instance.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="startIndex"/> or <paramref name="length"/> is less than zero.
        /// </exception>
        /// <exception cref="ArgumentNullException">If <paramref name="array"/> is <c>null</c>.</exception>
        public static SingleBuffer Wrap(float[] array, int startIndex, int length)
        {
            if (array is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.array);
            if (startIndex < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(startIndex, ExceptionArgument.startIndex);
            if (length < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(length, ExceptionArgument.length);
            if (startIndex > array.Length - length) // Checks for int overflow
                throw new ArgumentOutOfRangeException(nameof(length), SR.ArgumentOutOfRange_IndexLengthArray);

            return new ReadWriteSingleArrayBuffer(array)
            {
                position = startIndex,
                limit = startIndex + length
            };
        }

        /// <summary>
        /// Constructs a <see cref="SingleBuffer"/> with given capacity.
        /// </summary>
        /// <param name="capacity">The capacity of the buffer.</param>
        internal SingleBuffer(int capacity)
            : base(capacity)
        { }

        /// <summary>
        /// Gets the <see cref="float"/> array which this buffer is based on, if there is one.
        /// </summary>
        /// <exception cref="ReadOnlyBufferException">If this buffer is based on an array, but it is read-only.</exception>
        /// <exception cref="NotSupportedException">If this buffer is not based on an array.</exception>
        [SuppressMessage("Microsoft.Performance", "CA1819", Justification = "design requires some writable array properties")]
        public float[] Array => ProtectedArray;

        /// <summary>
        /// Gets the offset of the <see cref="float"/> array which this buffer is based on, if
        /// there is one.
        /// <para/>
        /// The offset is the index of the array corresponds to the zero position of
        /// the buffer.
        /// </summary>
        /// <exception cref="ReadOnlyBufferException">If this buffer is based on an array, but it is read-only.</exception>
        /// <exception cref="NotSupportedException">If this buffer is not based on an array.</exception>
        public int ArrayOffset => ProtectedArrayOffset;

        /// <summary>
        /// Returns a read-only buffer that shares its content with this buffer.
        /// <para/>
        /// The returned buffer is guaranteed to be a new instance, even this buffer
        /// is read-only itself. The new buffer's position, limit, capacity and mark
        /// are the same as this buffer's.
        /// <para/>
        /// The new buffer shares its content with this buffer, which means this
        /// buffer's change of content will be visible to the new buffer. The two
        /// buffer's position, limit and mark are independent.
        /// </summary>
        /// <returns>A read-only version of this buffer.</returns>
        public abstract SingleBuffer AsReadOnlyBuffer();

        /// <summary>
        /// Compacts this <see cref="SingleBuffer"/>.
        /// <para/>
        /// The remaining <see cref="float"/>s will be moved to the head of the buffer, starting from
        /// position zero. Then the position is set to <see cref="Buffer.Remaining"/>; the
        /// limit is set to capacity; the mark is cleared.
        /// </summary>
        /// <returns>This buffer.</returns>
        /// <exception cref="ReadOnlyBufferException">If no changes may be made to the contents of this buffer.</exception>
        public abstract SingleBuffer Compact();

        /// <summary>
        /// Compares the remaining <see cref="float"/>s of this buffer to another <see cref="SingleBuffer"/>'s
        /// remaining <see cref="float"/>s.
        /// </summary>
        /// <param name="other">Another <see cref="SingleBuffer"/>.</param>
        /// <returns>a negative value if this is less than <paramref name="other"/>; 0 if this
        /// equals to <paramref name="other"/>; a positive value if this is greater
        /// than <paramref name="other"/>.</returns>
        public virtual int CompareTo(SingleBuffer? other)
        {
            if (other is null) return 1; // Using 1 if other is null as specified here: https://stackoverflow.com/a/4852537

            int num = Math.Min(Remaining, other.Remaining);
            int pos_this = position;
            int pos_other = other.position;
            int result;

            for (int count = 0; count < num; count++)
            {
                if ((result = Get(pos_this++).CompareTo(other.Get(pos_other++))) != 0)
                    return result;
            }

            return Remaining - other.Remaining;
        }

        /// <summary>
        /// Returns a duplicated buffer that shares its content with this buffer.
        /// <para/>
        /// The duplicated buffer's position, limit, capacity and mark are the same
        /// as this buffer. The duplicated buffer's read-only property and byte order
        /// are the same as this buffer's.
        /// <para/>
        /// The new buffer shares its content with this buffer, which means either
        /// buffer's change of content will be visible to the other. The two buffer's
        /// position, limit and mark are independent.
        /// </summary>
        /// <returns>A duplicated buffer that shares its content with this buffer.</returns>
        public abstract SingleBuffer Duplicate();

        /// <summary>
        /// Checks whether this <see cref="SingleBuffer"/> is equal to another object.
        /// <para/>
        /// If <paramref name="other"/> is not a <see cref="SingleBuffer"/> then <c>false</c> is returned. Two
        /// <see cref="SingleBuffer"/>s are equal if and only if their remaining <see cref="float"/>s are exactly the
        /// same. Position, limit, capacity and mark are not considered.
        /// </summary>
        /// <param name="other">The object to compare with this <see cref="SingleBuffer"/>.</param>
        /// <returns><c>true</c> if this <see cref="SingleBuffer"/> is equal to <paramref name="other"/>,
        /// <c>false</c> otherwise.</returns>
        public override bool Equals(object? other)
        {
            if (other is null || !(other is SingleBuffer otherBuffer))
            {
                return false;
            }

            if (Remaining != otherBuffer.Remaining)
            {
                return false;
            }

            int myPosition = position;
            int otherPosition = otherBuffer.position;
            bool equalSoFar = true;
            float a, b;
            while (equalSoFar && (myPosition < limit))
            {
                a = Get(myPosition++);
                b = otherBuffer.Get(otherPosition++);
                equalSoFar = a == b || (float.IsNaN(a) && float.IsNaN(b));
            }

            return equalSoFar;
        }

        /// <summary>
        /// Returns the <see cref="float"/> at the current position and increases the position by 1.
        /// </summary>
        /// <returns>The <see cref="float"/> at the current position.</returns>
        /// <exception cref="BufferUnderflowException">If the position is equal or greater than limit.</exception>
        public abstract float Get();

        /// <summary>
        /// Reads <see cref="float"/>s from the current position into the specified int array and
        /// increases the position by the number of <see cref="float"/>s read.
        /// <para/>
        /// Calling this method has the same effect as
        /// <c>Get(destination, 0, destination.Length)</c>.
        /// </summary>
        /// <param name="destination">The destination <see cref="float"/> array.</param>
        /// <returns>This buffer.</returns>
        /// <exception cref="BufferUnderflowException">If <c>destination.Length</c> is greater than <see cref="Buffer.Remaining"/>.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="destination"/> is <c>null</c>.</exception>
        public virtual SingleBuffer Get(float[] destination)
        {
            if (destination is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.destination);
            return Get(destination, 0, destination.Length);
        }

        /// <summary>
        /// Reads <see cref="float"/>s from the current position into the specified <see cref="float"/> array,
        /// starting from the specified offset, and increases the position by the
        /// number of <see cref="float"/>s read.
        /// </summary>
        /// <param name="destination">The target <see cref="float"/> array.</param>
        /// <param name="offset">the offset of the <see cref="float"/> array, must not be negative and not
        /// greater than <c>destination.Length</c>.</param>
        /// <param name="length">The number of <see cref="float"/>s to read, must be no less than zero and not
        /// greater than <c>destination.Length - offset</c>.</param>
        /// <returns>This buffer.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="offset"/> plus <paramref name="length"/> indicates a position not within this instance.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="offset"/> or <paramref name="length"/> is less than zero.
        /// </exception>
        /// <exception cref="BufferUnderflowException">If <paramref name="length"/> is greater than <see cref="Buffer.Remaining"/>.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="destination"/> is <c>null</c>.</exception>
        public virtual SingleBuffer Get(float[] destination, int offset, int length) // J2N TODO: API - Rename startIndex instead of offset
        {
            if (destination is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.destination);
            if (offset < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(offset, ExceptionArgument.offset);
            if (length < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(length, ExceptionArgument.length);
            if (offset > destination.Length - length) // Checks for int overflow
                throw new ArgumentOutOfRangeException(nameof(length), SR.ArgumentOutOfRange_IndexLengthArray);
            if (length > Remaining)
                throw new BufferUnderflowException();

            for (int i = offset; i < offset + length; i++)
            {
                destination[i] = Get();
            }
            return this;
        }

        /// <summary>
        /// Returns an <see cref="float"/> at the specified <paramref name="index"/>; the position is not changed.
        /// </summary>
        /// <param name="index">The index, must not be negative and less than limit.</param>
        /// <returns>The <see cref="float"/> at the specified index.</returns>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="index"/> is invalid.</exception>
        public abstract float Get(int index);

        /// <summary>
        /// Indicates whether this buffer is based on a <see cref="float"/> array and is read/write.
        /// <para/>
        /// Returns <c>true</c> if this buffer is based on a <see cref="float"/> array and provides
        /// read/write access, <c>false</c> otherwise.
        /// </summary>
        public bool HasArray => ProtectedHasArray;

        /// <summary>
        /// Calculates this buffer's hash code from the remaining chars. The
        /// position, limit, capacity and mark don't affect the hash code.
        /// </summary>
        /// <returns>The hash code calculated from the remaining <see cref="float"/>s.</returns>
        public override int GetHashCode()
        {
            int myPosition = position;
            int hash = 0;
            while (myPosition < limit)
            {
                hash += BitConversion.SingleToInt32Bits(Get(myPosition++));
            }
            return hash;
        }

        ///// <summary>
        ///// Indicates whether this buffer is direct. A direct buffer will try its
        ///// best to take advantage of native memory APIs and it may not stay in the
        ///// heap, so it is not affected by garbage collection.
        ///// <para/>
        ///// An <see cref="int"/> buffer is direct if it is based on a byte buffer and the byte
        ///// buffer is direct.
        ///// <para/>
        ///// Returns <c>true</c> if this buffer is direct, <c>false</c> otherwise.
        ///// </summary>
        //public abstract bool IsDirect { get; }

        /// <summary>
        /// Gets the byte order used by this buffer when converting ints from/to
        /// bytes.
        /// <para/>
        /// If this buffer is not based on a byte buffer, then always return the
        /// platform's native byte order.
        /// </summary>
        public abstract ByteOrder Order { get; }

        /// <summary>
        /// Child class implements this property to realize <see cref="Array"/>.
        /// </summary>
        /// <seealso cref="Array"/>
        [SuppressMessage("Microsoft.Performance", "CA1819", Justification = "design requires some writable array properties")]
        protected abstract float[] ProtectedArray { get; }

        /// <summary>
        /// Child class implements this property to realize <see cref="ArrayOffset"/>.
        /// </summary>
        /// <seealso cref="ArrayOffset"/>
        protected abstract int ProtectedArrayOffset { get; }

        /// <summary>
        /// Child class implements this property to realize <see cref="HasArray"/>.
        /// </summary>
        /// <seealso cref="HasArray"/>
        protected abstract bool ProtectedHasArray { get; }

        /// <summary>
        /// Writes the given <see cref="float"/> to the current position and increases the position
        /// by 1.
        /// </summary>
        /// <param name="value">The <see cref="float"/> to write.</param>
        /// <returns>This buffer.</returns>
        /// <exception cref="BufferOverflowException">If position is equal or greater than limit.</exception>
        /// <exception cref="ReadOnlyBufferException">If no changes may be made to the contents of this buffer.</exception>
        public abstract SingleBuffer Put(float value);

        /// <summary>
        /// Writes <see cref="float"/>s from the given <see cref="float"/> array to the current position and
        /// increases the position by the number of <see cref="float"/>s written.
        /// <para/>
        /// Calling this method has the same effect as
        /// <c>Put(source, 0, source.Length)</c>.
        /// </summary>
        /// <param name="source">The source <see cref="float"/> array.</param>
        /// <returns>This buffer.</returns>
        /// <exception cref="BufferOverflowException">If <see cref="Buffer.Remaining"/> is less than <c>source.Length</c>.</exception>
        /// <exception cref="ReadOnlyBufferException">If no changes may be made to the contents of this buffer.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="source"/> is <c>null</c>.</exception>
        public SingleBuffer Put(float[] source)
        {
            if (source is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
            return Put(source, 0, source.Length);
        }

        /// <summary>
        /// Writes <see cref="float"/>s from the given int array, starting from the specified offset,
        /// to the current position and increases the position by the number of <see cref="float"/>s
        /// written.
        /// </summary>
        /// <param name="source">The source <see cref="float"/> array.</param>
        /// <param name="offset">The offset of <see cref="float"/> array, must not be negative and not greater than <c>source.Length</c>.</param>
        /// <param name="length">The number of <see cref="float"/>s to write, must be no less than zero and not
        /// greater than <c>source.Length - offset</c>.</param>
        /// <returns>This buffer.</returns>
        /// <exception cref="BufferOverflowException">If <see cref="Buffer.Remaining"/> is less than <paramref name="length"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="offset"/> plus <paramref name="length"/> indicates a position not within this instance.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="offset"/> or <paramref name="length"/> is less than zero.
        /// </exception>
        /// <exception cref="ReadOnlyBufferException">If no changes may be made to the contents of this buffer.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="source"/> is <c>null</c>.</exception>
        public virtual SingleBuffer Put(float[] source, int offset, int length) // J2N TODO: API - Rename startIndex instead of offset
        {
            if (source is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
            if (offset < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(offset, ExceptionArgument.offset);
            if (length < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(length, ExceptionArgument.length);
            if (offset > source.Length - length) // Checks for int overflow
                throw new ArgumentOutOfRangeException(nameof(length), SR.ArgumentOutOfRange_IndexLengthArray);
            if (length > Remaining)
                throw new BufferOverflowException();

            for (int i = offset; i < offset + length; i++)
            {
                Put(source[i]);
            }
            return this;
        }

        /// <summary>
        /// Writes all the remaining <see cref="float"/>s of the <paramref name="source"/> <see cref="SingleBuffer"/> to this
        /// buffer's current position, and increases both buffers' position by the
        /// number of <see cref="float"/>s copied.
        /// </summary>
        /// <param name="source">The source <see cref="SingleBuffer"/>.</param>
        /// <returns>This buffer.</returns>
        /// <exception cref="BufferOverflowException">If <c>source.Remaining</c> is greater than this buffer's <see cref="Buffer.Remaining"/>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="source"/> is this buffer.</exception>
        /// <exception cref="ReadOnlyBufferException">If no changes may be made to the contents of this buffer.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="source"/> is <c>null</c>.</exception>
        public virtual SingleBuffer Put(SingleBuffer source)
        {
            if (source is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
            if (ReferenceEquals(source, this))
                throw new ArgumentException(J2N.SR.Format(SR.Argument_MustNotBeThis, nameof(source), nameof(source)));
            if (source.Remaining > Remaining)
                throw new BufferOverflowException();
            if (IsReadOnly)
                throw new ReadOnlyBufferException(); // J2N: Harmony has a bug - it shouldn't read the source and change its position unless this buffer is writable

            float[] contents = new float[source.Remaining];
            source.Get(contents);
            Put(contents);
            return this;
        }

        /// <summary>
        /// Write a <see cref="float"/> to the specified <paramref name="index"/> of this buffer; the position is not
        /// changed.
        /// </summary>
        /// <param name="index">The index, must not be negative and less than the limit.</param>
        /// <param name="value">The <see cref="float"/> to write.</param>
        /// <returns>This buffer.</returns>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="index"/> is invalid.</exception>
        /// <exception cref="ReadOnlyBufferException">If no changes may be made to the contents of this buffer.</exception>
        public abstract SingleBuffer Put(int index, float value);

        /// <summary>
        /// Returns a sliced buffer that shares its content with this buffer.
        /// <para/>
        /// The sliced buffer's capacity will be this buffer's <see cref="Buffer.Remaining"/>,
        /// and its zero position will correspond to this buffer's current position.
        /// The new buffer's position will be 0, limit will be its capacity, and its
        /// mark is cleared. The new buffer's read-only property and byte order are
        /// same as this buffer's.
        /// <para/>
        /// The new buffer shares its content with this buffer, which means either
        /// buffer's change of content will be visible to the other. The two buffer's
        /// position, limit and mark are independent.
        /// </summary>
        /// <returns>A sliced buffer that shares its content with this buffer.</returns>
        public abstract SingleBuffer Slice();

        /// <summary>
        /// Returns a string represents of the state of this <see cref="SingleBuffer"/>.
        /// </summary>
        /// <returns>A string represents of the state of this <see cref="SingleBuffer"/>.</returns>
        public override string ToString()
        {
            StringBuilder buf = new StringBuilder();
            buf.Append(GetType().Name);
            buf.Append(", status: capacity="); //$NON-NLS-1$
            buf.Append(Capacity);
            buf.Append(" position="); //$NON-NLS-1$
            buf.Append(Position);
            buf.Append(" limit="); //$NON-NLS-1$
            buf.Append(Limit);
            return buf.ToString();
        }
    }
}
