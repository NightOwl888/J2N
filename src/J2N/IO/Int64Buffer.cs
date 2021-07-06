#region Copyright 2010 by Apache Harmony, Licensed under the Apache License, Version 2.0
/*Licensed to the Apache Software Foundation (ASF) under one or more
 *  contributor license agreements.  See the NOTICE file distributed with
 *  this work for additional information regarding copyright ownership.
 *  The ASF licenses this file to You under the Apache License, Version 2.0
 *  (the "License"); you may not use this file except in compliance with
 *  the License.  You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 *Unless required by applicable law or agreed to in writing, software
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
    using SR = J2N.Resources.Strings;

    /// <summary>
    /// A buffer of <see cref="long"/>s.
    /// <para/>
    /// A long buffer can be created in either of the following ways:
    /// <list type="bullet">
    ///     <item><description><see cref="Allocate(int)"/> a new long array and create a buffer
    ///         based on it</description></item>
    ///     <item><description><see cref="Wrap(long[])"/> an existing long array to create a new
    ///         buffer</description></item>
    ///     <item><description>Use <see cref="ByteBuffer.AsInt64Buffer"/> to create a <see cref="int"/>
    ///         buffer based on a byte buffer.</description></item>
    /// </list>
    /// </summary>
    public abstract class Int64Buffer : Buffer, IComparable<Int64Buffer>
    {
        /// <summary>
        /// Creates a long buffer based on a newly allocated long array.
        /// </summary>
        /// <param name="capacity">the capacity of the new buffer.</param>
        /// <returns>the created long buffer.</returns>
        /// <exception cref="ArgumentOutOfRangeException">if <paramref name="capacity"/> is less than zero.</exception>
        public static Int64Buffer Allocate(int capacity)
        {
            if (capacity < 0)
                throw new ArgumentException(nameof(capacity));

            return new ReadWriteInt64ArrayBuffer(capacity);
        }

        /// <summary>
        /// Creates a new <see cref="long"/> buffer by wrapping the given <see cref="long"/> array.
        /// <para/>
        /// Calling this method has the same effect as
        /// <c>Wrap(array, 0, array.Length)</c>.
        /// </summary>
        /// <param name="array">The <see cref="long"/> array which the new buffer will be based on.</param>
        /// <returns>The created <see cref="long"/> buffer.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="array"/> is <c>null</c>.</exception>
        public static Int64Buffer Wrap(long[] array)
        {
            if (array is null)
                throw new ArgumentNullException(nameof(array));

            return Wrap(array, 0, array.Length);
        }

        /// <summary>
        /// Creates a new <see cref="long"/> buffer by wrapping the given <see cref="long"/> array.
        /// <para/>
        /// The new buffer's position will be <paramref name="startIndex"/>, limit will be
        /// <c><paramref name="startIndex"/> + <paramref name="length"/></c>, capacity will
        /// be the length of the array.
        /// </summary>
        /// <param name="array">The <see cref="long"/> array which the new buffer will be based on.</param>
        /// <param name="startIndex">The start index, must not be negative and not greater than <c>array.Length</c>.</param>
        /// <param name="length">The length, must not be negative and not greater than
        /// <c>array.Length - <paramref name="startIndex"/></c>.</param>
        /// <returns>The created <see cref="long"/> buffer.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startIndex"/> plus <paramref name="length"/> indicates a position not within this instance.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="startIndex"/> or <paramref name="length"/> is less than zero.
        /// </exception>
        /// <exception cref="ArgumentNullException">If <paramref name="array"/> is <c>null</c>.</exception>
        public static Int64Buffer Wrap(long[] array, int startIndex, int length)
        {
            if (array is null)
                throw new ArgumentNullException(nameof(array));

            int len = array.Length;
            if (startIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(startIndex), SR.ArgumentOutOfRange_NeedNonNegNum);
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length), SR.ArgumentOutOfRange_NeedNonNegNum);
            if (startIndex > len - length) // Checks for int overflow
                throw new ArgumentOutOfRangeException(nameof(length), SR.ArgumentOutOfRange_IndexLength);

            return new ReadWriteInt64ArrayBuffer(array)
            {
                position = startIndex,
                limit = startIndex + length
            };
        }


        /// <summary>
        /// Constructs a <see cref="Int64Buffer"/> with given capacity.
        /// </summary>
        internal Int64Buffer(int capacity)
            : base(capacity)
        {
        }

        /// <summary>
        /// Gets the <see cref="long"/> array which this buffer is based on, if there is one.
        /// </summary>
        /// <returns>The byte array which this buffer is based on.</returns>
        /// <exception cref="ReadOnlyBufferException">If this buffer is based on an array, but it is read-only.</exception>
        /// <exception cref="NotSupportedException">If this buffer is not based on an array.</exception>
        [SuppressMessage("Microsoft.Performance", "CA1819", Justification = "design requires some writable array properties")]
        public long[] Array
        {
            get { return ProtectedArray; }
        }

        /// <summary>
        /// Returns the offset of the byte array which this buffer is based on, if
        /// there is one.
        /// <para/>
        /// The offset is the index of the array which corresponds to the zero
        /// position of the buffer.
        /// </summary>
        /// <exception cref="ReadOnlyBufferException">If this buffer is based on an array, but it is read-only.</exception>
        /// <exception cref="NotSupportedException">If this buffer is not based on an array.</exception>
        public int ArrayOffset
        {
            get { return ProtectedArrayOffset; }
        }

        /// <summary>
        /// Returns a read-only buffer that shares its content with this buffer.
        /// <para/>
        /// The returned buffer is guaranteed to be a new instance, even if this
        /// buffer is read-only itself. The new buffer's position, limit, capacity
        /// and mark are the same as this buffer.
        /// <para/>
        /// The new buffer shares its content with this buffer, which means this
        /// buffer's change of content will be visible to the new buffer. The two
        /// buffer's position, limit and mark are independent.
        /// </summary>
        /// <returns>A read-only version of this buffer.</returns>
        public abstract Int64Buffer AsReadOnlyBuffer();

        /// <summary>
        /// Compacts this <see cref="Int64Buffer"/>.
        /// <para/>
        /// The remaining <see cref="int"/>s will be moved to the head of the buffer, starting from
        /// position zero. Then the position is set to <see cref="Buffer.Remaining"/>; the
        /// limit is set to capacity; the mark is cleared.
        /// </summary>
        /// <returns>This buffer.</returns>
        /// <exception cref="ReadOnlyBufferException">If no changes may be made to the contents of this buffer.</exception>
        public abstract Int64Buffer Compact();

        /// <summary>
        /// Compares the remaining <see cref="long"/>s of this buffer to another <see cref="Int64Buffer"/>'s
        /// remaining <see cref="long"/>s.
        /// </summary>
        /// <param name="other">Another <see cref="Int64Buffer"/>.</param>
        /// <returns>A negative value if this is less than <paramref name="other"/>; 0 if this
        /// equals to <paramref name="other"/>; a positive value if this is greater
        /// than <paramref name="other"/>.</returns>
        public int CompareTo(Int64Buffer? other)
        {
            if (other is null) return 1; // Using 1 if other is null as specified here: https://stackoverflow.com/a/4852537

            int compareRemaining = (Remaining < other.Remaining) ? Remaining
                : other.Remaining;
            int thisPos = position;
            int otherPos = other.position;
            long thisLong, otherLong;
            while (compareRemaining > 0)
            {
                thisLong = Get(thisPos);
                otherLong = other.Get(otherPos);
                if (thisLong != otherLong)
                {
                    // NOTE: IN .NET comparison should return
                    // the diff, not be hard coded to 1/-1
                    //return (int)(thisLong - otherLong);
                    return thisLong < otherLong ? -1 : 1; // Disallow overflow to skew results
                }
                thisPos++;
                otherPos++;
                compareRemaining--;
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
        public abstract Int64Buffer Duplicate();

        /// <summary>
        /// Checks whether this <see cref="Int64Buffer"/> is equal to another object.
        /// <para/>
        /// If <paramref name="other"/> is not a <see cref="Int64Buffer"/> then <c>false</c> is returned. Two
        /// <see cref="Int64Buffer"/>s are equal if and only if their remaining <see cref="long"/>s are exactly the
        /// same. Position, limit, capacity and mark are not considered.
        /// </summary>
        /// <param name="other">The object to compare with this <see cref="Int64Buffer"/>.</param>
        /// <returns><c>true</c> if this <see cref="Int64Buffer"/> is equal to <paramref name="other"/>,
        /// <c>false</c> otherwise.</returns>
        public override bool Equals(object? other)
        {
            if (other is null || !(other is Int64Buffer otherBuffer))
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
            while (equalSoFar && (myPosition < limit))
            {
                equalSoFar = Get(myPosition++) == otherBuffer.Get(otherPosition++);
            }

            return equalSoFar;
        }

        /// <summary>
        /// Returns the <see cref="long"/> at the current position and increases the position by 1.
        /// </summary>
        /// <returns>The <see cref="long"/> at the current position.</returns>
        /// <exception cref="BufferUnderflowException">If the position is equal or greater than limit.</exception>
        public abstract long Get();

        /// <summary>
        /// Reads <see cref="long"/>s from the current position into the specified int array and
        /// increases the position by the number of <see cref="long"/>s read.
        /// <para/>
        /// Calling this method has the same effect as
        /// <c>Get(destination, 0, destination.Length)</c>.
        /// </summary>
        /// <param name="destination">The destination <see cref="long"/> array.</param>
        /// <returns>This buffer.</returns>
        /// <exception cref="BufferUnderflowException">If <c>destination.Length</c> is greater than <see cref="Buffer.Remaining"/>.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="destination"/> is <c>null</c>.</exception>
        public virtual Int64Buffer Get(long[] destination)
        {
            if (destination is null)
                throw new ArgumentNullException(nameof(destination));

            return Get(destination, 0, destination.Length);
        }

        /// <summary>
        /// Reads <see cref="long"/>s from the current position into the specified <see cref="long"/> array,
        /// starting from the specified offset, and increases the position by the
        /// number of <see cref="long"/>s read.
        /// </summary>
        /// <param name="destination">The target <see cref="long"/> array.</param>
        /// <param name="offset">the offset of the <see cref="long"/> array, must not be negative and not
        /// greater than <c>destination.Length</c>.</param>
        /// <param name="length">The number of <see cref="long"/>s to read, must be no less than zero and not
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
        public virtual Int64Buffer Get(long[] destination, int offset, int length)
        {
            if (destination is null)
                throw new ArgumentNullException(nameof(destination));

            int len = destination.Length;
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset), SR.ArgumentOutOfRange_NeedNonNegNum);
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length), SR.ArgumentOutOfRange_NeedNonNegNum);
            if (offset > len - length) // Checks for int overflow
                throw new ArgumentOutOfRangeException(nameof(length), SR.ArgumentOutOfRange_IndexLength);

            if (length > Remaining)
            {
                throw new BufferUnderflowException();
            }
            for (int i = offset; i < offset + length; i++)
            {
                destination[i] = Get();
            }
            return this;
        }

        /// <summary>
        /// Returns an <see cref="long"/> at the specified <paramref name="index"/>; the position is not changed.
        /// </summary>
        /// <param name="index">The index, must not be negative and less than limit.</param>
        /// <returns>The <see cref="long"/> at the specified index.</returns>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="index"/> is invalid.</exception>
        public abstract long Get(int index);

        /// <summary>
        /// Indicates whether this buffer is based on a <see cref="long"/> array and is read/write.
        /// <para/>
        /// Returns <c>true</c> if this buffer is based on a <see cref="long"/> array and provides
        /// read/write access, <c>false</c> otherwise.
        /// </summary>
        public bool HasArray
        {
            get { return ProtectedHasArray; }
        }

        /// <summary>
        /// Calculates this buffer's hash code from the remaining chars. The
        /// position, limit, capacity and mark don't affect the hash code.
        /// </summary>
        /// <returns>The hash code calculated from the remaining <see cref="long"/>s.</returns>
        public override int GetHashCode()
        {
            int myPosition = position;
            int hash = 0;
            long l;
            while (myPosition < limit)
            {
                l = Get(myPosition++);
                hash = hash + ((int)l) ^ ((int)(l >> 32));
            }
            return hash;
        }

        ///// <summary>
        ///// Indicates whether this buffer is direct. A direct buffer will try its
        ///// best to take advantage of native memory APIs and it may not stay in the
        ///// heap, so it is not affected by garbage collection.
        ///// <para/>
        ///// A <see cref="long"/> buffer is direct if it is based on a byte buffer and the byte
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
        protected abstract long[] ProtectedArray { get; }

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
        /// Writes the given <see cref="long"/> to the current position and increases the position
        /// by 1.
        /// </summary>
        /// <param name="source">The <see cref="long"/> to write.</param>
        /// <returns>This buffer.</returns>
        /// <exception cref="BufferOverflowException">If position is equal or greater than limit.</exception>
        /// <exception cref="ReadOnlyBufferException">If no changes may be made to the contents of this buffer.</exception>
        public abstract Int64Buffer Put(long source);

        /// <summary>
        /// Writes <see cref="long"/>s from the given <see cref="long"/> array to the current position and
        /// increases the position by the number of <see cref="long"/>s written.
        /// <para/>
        /// Calling this method has the same effect as
        /// <c>Put(source, 0, source.Length)</c>.
        /// </summary>
        /// <param name="source">The source <see cref="long"/> array.</param>
        /// <returns>This buffer.</returns>
        /// <exception cref="BufferOverflowException">If <see cref="Buffer.Remaining"/> is less than <c>source.Length</c>.</exception>
        /// <exception cref="ReadOnlyBufferException">If no changes may be made to the contents of this buffer.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="source"/> is <c>null</c>.</exception>
        public Int64Buffer Put(long[] source)
        {
            if (source is null)
                throw new ArgumentNullException(nameof(source));

            return Put(source, 0, source.Length);
        }

        /// <summary>
        /// Writes <see cref="long"/>s from the given int array, starting from the specified offset,
        /// to the current position and increases the position by the number of <see cref="long"/>s
        /// written.
        /// </summary>
        /// <param name="source">The source <see cref="long"/> array.</param>
        /// <param name="offset">The offset of <see cref="long"/> array, must not be negative and not greater than <c>source.Length</c>.</param>
        /// <param name="length">The number of <see cref="long"/>s to write, must be no less than zero and not
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
        public virtual Int64Buffer Put(long[] source, int offset, int length)
        {
            if (source is null)
                throw new ArgumentNullException(nameof(source));

            int len = source.Length;
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset), SR.ArgumentOutOfRange_NeedNonNegNum);
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length), SR.ArgumentOutOfRange_NeedNonNegNum);
            if (offset > len - length) // Checks for int overflow
                throw new ArgumentOutOfRangeException(nameof(length), SR.ArgumentOutOfRange_IndexLength);
            if (length > Remaining)
                throw new BufferOverflowException();

            for (int i = offset; i < offset + length; i++)
            {
                Put(source[i]);
            }
            return this;
        }

        /// <summary>
        /// Writes all the remaining <see cref="long"/>s of the <paramref name="source"/> <see cref="Int64Buffer"/> to this
        /// buffer's current position, and increases both buffers' position by the
        /// number of <see cref="long"/>s copied.
        /// </summary>
        /// <param name="source">The source <see cref="Int32Buffer"/>.</param>
        /// <returns>This buffer.</returns>
        /// <exception cref="BufferOverflowException">If <c>source.Remaining</c> is greater than this buffer's <see cref="Buffer.Remaining"/>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="source"/> is this buffer.</exception>
        /// <exception cref="ReadOnlyBufferException">If no changes may be made to the contents of this buffer.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="source"/> is <c>null</c>.</exception>
        public virtual Int64Buffer Put(Int64Buffer source)
        {
            if (source is null)
                throw new ArgumentNullException(nameof(source));
            if (ReferenceEquals(source, this))
                throw new ArgumentException(J2N.SR.Format(SR.Argument_MustNotBeThis, nameof(source), nameof(source)));
            if (source.Remaining > Remaining)
                throw new BufferOverflowException();
            if (IsReadOnly)
                throw new ReadOnlyBufferException(); // J2N: Harmony has a bug - it shouldn't read the source and change its position unless this buffer is writable

            long[] contents = new long[source.Remaining];
            source.Get(contents);
            Put(contents);
            return this;
        }

        /// <summary>
        /// Write a <see cref="long"/> to the specified <paramref name="index"/> of this buffer; the position is not
        /// changed.
        /// </summary>
        /// <param name="index">The index, must not be negative and less than the limit.</param>
        /// <param name="value">The <see cref="long"/> to write.</param>
        /// <returns>This buffer.</returns>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="index"/> is invalid.</exception>
        /// <exception cref="ReadOnlyBufferException">If no changes may be made to the contents of this buffer.</exception>
        public abstract Int64Buffer Put(int index, long value);

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
        public abstract Int64Buffer Slice();

        /// <summary>
        /// Returns a string represents of the state of this <see cref="Int64Buffer"/>.
        /// </summary>
        /// <returns>A string represents of the state of this <see cref="Int64Buffer"/>.</returns>
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
