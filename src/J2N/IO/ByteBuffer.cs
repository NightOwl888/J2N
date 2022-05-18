#region Copyright 2010 by Apache Harmony, Licensed under the Apache License, Version 2.0
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
    using SR = J2N.Resources.Strings;

    /////     <item><description><see cref="AllocateDirect(int)"/> a memory block and create a direct
    /////     buffer based on it</description></item>

    /// <summary>
    /// A buffer for <see cref="byte"/>s.
    /// <para/>
    /// A byte buffer can be created in either one of the following ways:
    /// <list type="bullet">
    ///     <item><description><see cref="Allocate(int)"/> a new byte array and create a
    ///     buffer based on it</description></item>
    ///     <item><description><see cref="Wrap(byte[])"/> an existing byte array to create a new buffer</description></item>
    /// </list>
    /// </summary>
    public abstract class ByteBuffer : Buffer, IComparable<ByteBuffer>
    {
        /// <summary>
        /// Creates a byte buffer based on a newly allocated byte array.
        /// </summary>
        /// <param name="capacity">The capacity of the new buffer</param>
        /// <returns>The created byte buffer.</returns>
        /// <exception cref="ArgumentOutOfRangeException">If the <c>capacity &lt; 0</c>.</exception>
        public static ByteBuffer Allocate(int capacity)
        {
            if (capacity < 0)
                throw new ArgumentOutOfRangeException(nameof(capacity));
            return new ReadWriteHeapByteBuffer(capacity);
        }

        // J2N: Not implemented
        ///// <summary>
        ///// Creates a direct byte buffer based on a newly allocated memory block. (NOT IMPLEMENTED)
        ///// </summary>
        ///// <param name="capacity">The capacity of the new buffer</param>
        ///// <returns>The new byte buffer</returns>
        ///// <exception cref="ArgumentOutOfRangeException">If the <c>capacity &lt; 0</c>.</exception>
        //public static ByteBuffer AllocateDirect(int capacity)
        //{
        //    throw new NotImplementedException();
        //    //return new DirectByteBuffer(capacity);
        //}

        /// <summary>
        /// Creates a new byte buffer by wrapping the given byte array.
        /// <para/>
        /// Calling this method has the same effect as
        /// <c>Wrap(array, 0, array.Length)</c>.
        /// </summary>
        /// <param name="array">The byte array which the new buffer will be based on</param>
        /// <returns>The new byte buffer</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="array"/> is <c>null</c>.</exception>
        public static ByteBuffer Wrap(byte[] array)
        {
            if (array is null)
                throw new ArgumentNullException(nameof(array));

            return new ReadWriteHeapByteBuffer(array);
        }

        /// <summary>
        /// Creates a new byte buffer by wrapping the given byte array.
        /// <para/>
        /// The new buffer's position will be <paramref name="startIndex"/>, limit will be
        /// <c>start + len</c>, capacity will be the length of the array.
        /// </summary>
        /// <param name="array">The byte array which the new buffer will be based on.</param>
        /// <param name="startIndex">
        /// The start index, must not be negative and not greater than <c>array.Length</c>.
        /// </param>
        /// <param name="length">
        /// The length, must not be negative and not greater than
        /// <c>array.Length - start</c>.
        /// </param>
        /// <returns>The new byte buffer</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startIndex"/> plus <paramref name="length"/> indicates a position not within this instance.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="startIndex"/> or <paramref name="length"/> is less than zero.
        /// </exception>
        /// <exception cref="ArgumentNullException">If <paramref name="array"/> is <c>null</c>.</exception>
        public static ByteBuffer Wrap(byte[] array, int startIndex, int length)
        {
            if (array is null)
                throw new ArgumentNullException(nameof(array));

            int actualLength = array.Length;
            if (startIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(startIndex), SR.ArgumentOutOfRange_NeedNonNegNum);
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length), SR.ArgumentOutOfRange_NeedNonNegNum);
            if (startIndex > actualLength - length) // Checks for int overflow
                throw new ArgumentOutOfRangeException(nameof(length), SR.ArgumentOutOfRange_IndexLength);

            return new ReadWriteHeapByteBuffer(array)
            {
                position = startIndex,
                limit = startIndex + length
            };
        }

        /// <summary>
        /// The byte order of this buffer, default is <see cref="ByteOrder.BigEndian"/>.
        /// </summary>
        internal Endianness order = Endianness.BigEndian;

        /// <summary>
        /// Constructs a <see cref="ByteBuffer"/> with given capacity.
        /// </summary>
        /// <param name="capacity">The capacity of the buffer.</param>
        internal ByteBuffer(int capacity)
            : base(capacity)
        {
        }

        /// <summary>
        /// Returns the byte array which this buffer is based on, if there is one.
        /// </summary>
        /// <returns>The byte array which this buffer is based on.</returns>
        /// <exception cref="ReadOnlyBufferException">If this buffer is based on a read-only array.</exception>
        /// <exception cref="NotSupportedException">If this buffer is not based on an array.</exception>
        [SuppressMessage("Microsoft.Performance", "CA1819", Justification = "design requires some writable array properties")]
        public byte[] Array
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
        /// <exception cref="ReadOnlyBufferException">If this buffer is based on a read-only array.</exception>
        /// <exception cref="NotSupportedException">If this buffer is not based on an array.</exception>
        public int ArrayOffset
        {
            get { return ProtectedArrayOffset; }
        }

        ///// The new buffer is direct if this byte buffer is direct.

        /// <summary>
        /// Returns a <see cref="char"/> buffer which is based on the remaining content of this
        /// byte buffer.
        /// <para/>
        /// The new buffer's position is zero, its limit and capacity is the number
        /// of remaining bytes divided by two, and its mark is not set. The new
        /// buffer's read-only property and byte order are the same as this buffer's.
        /// <para/>
        /// The new buffer shares its content with this buffer, which means either
        /// buffer's change of content will be visible to the other. The two buffer's
        /// position, limit and mark are independent.
        /// </summary>
        /// <returns>A <see cref="char"/> buffer which is based on the content of this byte buffer.</returns>
        public abstract CharBuffer AsCharBuffer();

        ///// The new buffer is direct if this byte buffer is direct.

        /// <summary>
        /// Returns a <see cref="double"/> buffer which is based on the remaining content of this
        /// byte buffer.
        /// <para/>
        /// The new buffer's position is zero, its limit and capacity is the number
        /// of remaining bytes divided by eight, and its mark is not set. The new
        /// buffer's read-only property and byte order are the same as this buffer's.
        /// <para/>
        /// The new buffer shares its content with this buffer, which means either
        /// buffer's change of content will be visible to the other. The two buffer's
        /// position, limit and mark are independent.
        /// </summary>
        /// <returns>A <see cref="double"/> buffer which is based on the content of this byte buffer.</returns>
        public abstract DoubleBuffer AsDoubleBuffer();

        ///// The new buffer is direct if this byte buffer is direct.

        /// <summary>
        /// Returns a <see cref="float"/> buffer which is based on the remaining content of this
        /// byte buffer.
        /// <para/>
        /// The new buffer's position is zero, its limit and capacity is the number
        /// of remaining bytes divided by four, and its mark is not set. The new
        /// buffer's read-only property and byte order are the same as this buffer's.
        /// <para/>
        /// The new buffer shares its content with this buffer, which means either
        /// buffer's change of content will be visible to the other. The two buffer's
        /// position, limit and mark are independent.
        /// </summary>
        /// <returns>A <see cref="float"/> buffer which is based on the content of this byte buffer.</returns>
        public abstract SingleBuffer AsSingleBuffer();

        ///// The new buffer is direct if this byte buffer is direct.

        /// <summary>
        /// Returns a <see cref="int"/> buffer which is based on the remaining content of this byte
        /// buffer.
        /// <para/>
        /// The new buffer's position is zero, its limit and capacity is the number
        /// of remaining bytes divided by four, and its mark is not set. The new
        /// buffer's read-only property and byte order are the same as this buffer's.
        /// <para/>
        /// The new buffer shares its content with this buffer, which means either
        /// buffer's change of content will be visible to the other. The two buffer's
        /// position, limit and mark are independent.
        /// </summary>
        /// <returns>A <see cref="int"/> buffer which is based on the content of this byte buffer.</returns>
        public abstract Int32Buffer AsInt32Buffer();

        ///// The new buffer is direct if this byte buffer is direct.

        /// <summary>
        /// Returns a <see cref="long"/> buffer which is based on the remaining content of this
        /// byte buffer.
        /// <para/>
        /// The new buffer's position is zero, its limit and capacity is the number
        /// of remaining bytes divided by eight, and its mark is not set. The new
        /// buffer's read-only property and byte order are the same as this buffer's.
        /// <para/>
        /// The new buffer shares its content with this buffer, which means either
        /// buffer's change of content will be visible to the other. The two buffer's
        /// position, limit and mark are independent.
        /// </summary>
        /// <returns>A <see cref="long"/> buffer which is based on the content of this byte buffer.</returns>
        public abstract Int64Buffer AsInt64Buffer();

        ///// The new buffer is direct if this byte buffer is direct.

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
        public abstract ByteBuffer AsReadOnlyBuffer();

        ///// The new buffer is direct if this byte buffer is direct.

        /// <summary>
        /// Returns a <see cref="short"/> buffer which is based on the remaining content of this
        /// byte buffer.
        /// </summary>
        /// <remarks>
        /// The new buffer's position is zero, its limit and capacity is the number
        /// of remaining bytes divided by two, and its mark is not set. The new
        /// buffer's read-only property and byte order are the same as this buffer's.
        /// <para/>
        /// The new buffer shares its content with this buffer, which means either
        /// buffer's change of content will be visible to the other. The two buffer's
        /// position, limit and mark are independent.
        /// </remarks>
        /// <returns>A <see cref="short"/> buffer which is based on the content of this byte buffer.</returns>
        public abstract Int16Buffer AsInt16Buffer();

        /// <summary>
        /// Compacts this byte buffer.
        /// <para/>
        /// The remaining bytes will be moved to the head of the
        /// buffer, starting from position zero. Then the position is set to
        /// <see cref="Buffer.Remaining"/>; the limit is set to capacity; the mark is
        /// cleared.
        /// </summary>
        /// <returns>This buffer.</returns>
        /// <exception cref="ReadOnlyBufferException">If no changes may be made to the contents of this buffer.</exception>
        public abstract ByteBuffer Compact();

        /// <summary>
        /// Compares the remaining bytes of this buffer to another byte buffer's
        /// remaining bytes.
        /// </summary>
        /// <param name="other">another byte buffer.</param>
        /// <returns>
        /// a negative value if this is less than <c>other</c>; 0 if this
        /// equals to <c>other</c>; a positive value if this is greater
        /// than <c>other</c>.
        /// </returns>
        public virtual int CompareTo(ByteBuffer? other)
        {
            if (other is null) return 1; // Using 1 if other is null as specified here: https://stackoverflow.com/a/4852537

            int compareRemaining = (Remaining < other.Remaining) ? Remaining
                    : other.Remaining;
            int thisPos = position;
            int otherPos = other.position;
            byte thisByte, otherByte;
            while (compareRemaining > 0)
            {
                thisByte = Get(thisPos);
                otherByte = other.Get(otherPos);
                if (thisByte != otherByte)
                {
                    // NOTE: IN .NET comparison should return
                    // the diff, not be hard coded to 1/-1
                    return thisByte - otherByte;
                    //return thisByte < otherByte ? -1 : 1;
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
        /// as this buffer's. The duplicated buffer's read-only property and byte
        /// order are the same as this buffer's too.
        /// <para/>
        /// The new buffer shares its content with this buffer, which means either
        /// buffer's change of content will be visible to the other. The two buffer's
        /// position, limit and mark are independent.
        /// </summary>
        /// <returns>a duplicated buffer that shares its content with this buffer.</returns>
        public abstract ByteBuffer Duplicate();

        /// <summary>
        /// Checks whether this byte buffer is equal to another object.
        /// <para/>
        /// If <paramref name="other"/> is not a byte buffer then <c>false</c> is returned. Two
        /// byte buffers are equal if and only if their remaining bytes are exactly
        /// the same. Position, limit, capacity and mark are not considered.
        /// </summary>
        /// <param name="other">The object to compare with this byte buffer.</param>
        /// <returns>
        /// <c>true</c> if this byte buffer is equal to <paramref name="other"/>,
        /// <c>false</c> otherwise.
        /// </returns>
        public override bool Equals(object? other)
        {
            if (other is null || !(other is ByteBuffer otherBuffer))
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
        /// Returns the byte at the current position and increases the position by 1.
        /// </summary>
        /// <returns>The byte at the current position.</returns>
        /// <exception cref="BufferUnderflowException">If the position is equal or greater than limit.</exception>
        public abstract byte Get();

        /// <summary>
        /// Reads bytes from the current position into the specified byte array and
        /// increases the position by the number of bytes read.
        /// <para/>
        /// Calling this method has the same effect as
        /// <c>Get(destination, 0, destination.Length)</c>.
        /// </summary>
        /// <param name="destination">The destination byte array.</param>
        /// <returns>This buffer.</returns>
        /// <exception cref="BufferUnderflowException">If <c>dest.Length</c> is greater than <see cref="Buffer.Remaining"/>.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="destination"/> is <c>null</c>.</exception>
        public virtual ByteBuffer Get(byte[] destination)
        {
            if (destination is null)
                throw new ArgumentNullException(nameof(destination));

            return Get(destination, 0, destination.Length);
        }

        /// <summary>
        /// Reads bytes from the current position into the specified byte array,
        /// starting at the specified offset, and increases the position by the
        /// number of bytes read.
        /// </summary>
        /// <param name="destination">The target byte array.</param>
        /// <param name="offset">
        /// The offset of the byte array, must not be negative and
        /// not greater than <c>destination.Length</c>.</param>
        /// <param name="length">
        /// The number of bytes to read, must not be negative and not
        /// greater than <c>destination.Length - offset</c>
        /// </param>
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
        public virtual ByteBuffer Get(byte[] destination, int offset, int length)
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
        /// Returns the byte at the specified index and does not change the position.
        /// 
        /// </summary>
        /// <param name="index">The index, must not be negative and less than limit.</param>
        /// <returns>The byte at the specified index.</returns>
        /// <exception cref="ArgumentOutOfRangeException">If index is invalid.</exception>
        public abstract byte Get(int index);

        /// <summary>
        /// Returns the <see cref="char"/> at the current position and increases the position by 2.
        /// <para/>
        /// The 2 bytes starting at the current position are composed into a char
        /// according to the current byte order and returned.
        /// </summary>
        /// <returns>The <see cref="char"/> at the current position.</returns>
        /// <exception cref="BufferUnderflowException">If the position is greater than <c>limit - 2</c>.</exception>
        public abstract char GetChar();

        /// <summary>
        /// Returns the <see cref="char"/> at the specified index.
        /// <para/>
        /// The 2 bytes starting from the specified index are composed into a char
        /// according to the current byte order and returned. The position is not
        /// changed.
        /// </summary>
        /// <param name="index">The index, must not be negative and equal or less than <c>limit - 2</c>.</param>
        /// <returns>The <see cref="char"/> at the specified index.</returns>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="index"/> is invalid.</exception>
        public abstract char GetChar(int index);

        /// <summary>
        /// Returns the <see cref="double"/> at the current position and increases the position by 8.
        /// <para/>
        /// The 8 bytes starting from the current position are composed into a <see cref="double"/>
        /// according to the current byte order and returned.
        /// </summary>
        /// <returns>The <see cref="double"/> at the current position.</returns>
        /// <exception cref="BufferUnderflowException">If the position is greater than <c>limit - 8</c>.</exception>
        public abstract double GetDouble();

        /// <summary>
        /// Returns the <see cref="double"/> at the specified index.
        /// <para/>
        /// The 8 bytes starting at the specified index are composed into a <see cref="double"/>
        /// according to the current byte order and returned. The position is not
        /// changed.
        /// </summary>
        /// <param name="index">The index, must not be negative and equal or less than <c>limit - 8</c>.</param>
        /// <returns>The <see cref="double"/> at the specified index.</returns>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="index"/> is invalid.</exception>
        public abstract double GetDouble(int index);

        /// <summary>
        /// Returns the <see cref="float"/> at the current position and increases the position by 4.
        /// <para/>
        /// The 4 bytes starting at the current position are composed into a <see cref="float"/>
        /// according to the current byte order and returned.
        /// <para/>
        /// NOTE: This was getFloat() in the JDK
        /// </summary>
        /// <returns>The <see cref="float"/> at the current position.</returns>
        /// <exception cref="BufferUnderflowException">If the position is greater than <c>limit - 4</c>.</exception>
        public abstract float GetSingle();

        /// <summary>
        /// Returns the <see cref="float"/> at the specified index.
        /// <para/>
        /// The 4 bytes starting at the specified index are composed into a <see cref="float"/>
        /// according to the current byte order and returned. The position is not
        /// changed.
        /// <para/>
        /// NOTE: This was getFloat() in the JDK
        /// </summary>
        /// <param name="index">The index, must not be negative and equal or less than <c>limit - 4</c>.</param>
        /// <returns>The <see cref="float"/> at the specified index.</returns>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="index"/> is invalid.</exception>
        public abstract float GetSingle(int index);

        /// <summary>
        /// Returns the <see cref="int"/> at the current position and increases the position by 4.
        /// <para/>
        /// The 4 bytes starting at the current position are composed into a <see cref="int"/>
        /// according to the current byte order and returned.
        /// <para/>
        /// NOTE: This was getInt() in the JDK
        /// </summary>
        /// <returns>The <see cref="int"/> at the current position.</returns>
        /// <exception cref="BufferUnderflowException">If the position is greater than <c>limit - 4</c>.</exception>
        public abstract int GetInt32();

        /// <summary>
        /// Returns the <see cref="int"/> at the specified index.
        /// <para/>
        /// The 4 bytes starting at the specified index are composed into a <see cref="int"/>
        /// according to the current byte order and returned. The position is not
        /// changed.
        /// <para/>
        /// NOTE: This was getInt() in the JDK
        /// </summary>
        /// <param name="index">The index, must not be negative and equal or less than <c>limit - 4</c>.</param>
        /// <returns>The <see cref="int"/> at the specified index.</returns>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="index"/> is invalid.</exception>
        public abstract int GetInt32(int index);

        /// <summary>
        /// Returns the <see cref="long"/> at the current position and increases the position by 8.
        /// <para/>
        /// The 8 bytes starting at the current position are composed into a <see cref="long"/>
        /// according to the current byte order and returned.
        /// <para/>
        /// NOTE: This was getLong() in the JDK
        /// </summary>
        /// <returns>The <see cref="long"/> at the current position.</returns>
        /// <exception cref="BufferUnderflowException">If the position is greater than <c>limit - 8</c>.</exception>
        public abstract long GetInt64();


        /// <summary>
        /// Returns the <see cref="long"/> at the specified index.
        /// <para/>
        /// The 8 bytes starting at the specified index are composed into a <see cref="long"/>
        /// according to the current byte order and returned. The position is not
        /// changed.
        /// <para/>
        /// NOTE: This was getLong() in the JDK
        /// </summary>
        /// <param name="index">The index, must not be negative and equal or less than <c>limit - 8</c>.</param>
        /// <returns>The <see cref="long"/> at the specified index.</returns>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="index"/> is invalid.</exception>
        public abstract long GetInt64(int index);

        /// <summary>
        /// Returns the <see cref="short"/> at the current position and increases the position by 2.
        /// <para/>
        /// The 2 bytes starting at the current position are composed into a <see cref="short"/>
        /// according to the current byte order and returned.
        /// <para/>
        /// NOTE: This was getShort() in the JDK
        /// </summary>
        /// <returns>The <see cref="short"/> at the current position.</returns>
        /// <exception cref="BufferUnderflowException">If the position is greater than <c>limit - 2</c>.</exception>
        public abstract short GetInt16();


        /// <summary>
        /// Returns the <see cref="short"/> at the specified index.
        /// <para/>
        /// The 2 bytes starting at the specified index are composed into a <see cref="short"/>
        /// according to the current byte order and returned. The position is not
        /// changed.
        /// <para/>
        /// NOTE: This was getShort() in the JDK
        /// </summary>
        /// <param name="index">The index, must not be negative and equal or less than <c>limit - 2</c>.</param>
        /// <returns>The <see cref="short"/> at the specified index.</returns>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="index"/> is invalid.</exception>
        public abstract short GetInt16(int index);

        /// <summary>
        /// Indicates whether this buffer is based on a byte array and provides
        /// read/write access.
        /// </summary>
        public bool HasArray => ProtectedHasArray;

        /// <summary>
        /// Calculates this buffer's hash code from the remaining chars. The
        /// position, limit, capacity and mark don't affect the hash code.
        /// </summary>
        /// <returns>The hash code calculated from the remaining bytes.</returns>
        public override int GetHashCode()
        {
            int myPosition = position;
            int hash = 0;
            while (myPosition < limit)
            {
                hash += Get(myPosition++);
            }
            return hash;
        }

        ///// <summary>
        ///// Indicates whether this buffer is direct.
        ///// </summary>
        //public abstract bool IsDirect { get; }

        /// <summary>
        /// Returns the byte order used by this buffer when converting bytes from/to
        /// other primitive types.
        /// <para/>
        /// The default byte order of byte buffer is always
        /// <see cref="ByteOrder.BigEndian"/>.
        /// </summary>
        public ByteOrder Order
        {
            get => order == Endianness.BigEndian ? ByteOrder.BigEndian : ByteOrder.LittleEndian;
            set => SetOrder(value);
        }

        /// <summary>
        /// Sets the byte order of this buffer.
        /// </summary>
        /// <param name="byteOrder">The byte order to set.</param>
        /// <returns>This buffer.</returns>
        public ByteBuffer SetOrder(ByteOrder byteOrder)
        {
            order = byteOrder == ByteOrder.BigEndian ? Endianness.BigEndian
                : Endianness.LittleEndian;
            return this;
        }


        /// <summary>
        /// Child class implements this method to realize <see cref="Array"/>.
        /// </summary>
        /// <seealso cref="Array"/>
        [SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "Design requires some array properties")]
        protected abstract byte[] ProtectedArray { get; }

        /// <summary>
        /// Child class implements this method to realize <see cref="ArrayOffset"/>.
        /// </summary>
        protected abstract int ProtectedArrayOffset { get; }

        /// <summary>
        /// Child class implements this method to realize <seealso cref="HasArray"/>.
        /// </summary>
        protected abstract bool ProtectedHasArray { get; }


        /// <summary>
        /// Writes the given byte to the current position and increases the position
        /// by 1.
        /// </summary>
        /// <param name="b">The byte to write.</param>
        /// <returns>This buffer.</returns>
        /// <exception cref="BufferOverflowException">If position is equal or greater than limit.</exception>
        /// <exception cref="ReadOnlyBufferException">If no changes may be made to the contents of this buffer.</exception>
        public abstract ByteBuffer Put(byte b);

        /// <summary>
        /// Writes bytes in the given byte array to the current position and
        /// increases the position by the number of bytes written.
        /// <para/>
        /// Calling this method has the same effect as
        /// <c>Put(source, 0, source.Length)</c>.
        /// </summary>
        /// <param name="source">The source byte array.</param>
        /// <returns>This buffer.</returns>
        /// <exception cref="BufferOverflowException">If <see cref="Buffer.Remaining"/> is less than <c>src.Length</c>.</exception>
        /// <exception cref="ReadOnlyBufferException">If no changes may be made to the contents of this buffer.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="source"/> is <c>null</c>.</exception>
        public ByteBuffer Put(byte[] source)
        {
            if (source is null)
                throw new ArgumentNullException(nameof(source));

            return Put(source, 0, source.Length);
        }

        /// <summary>
        /// Writes bytes in the given byte array, starting from the specified offset,
        /// to the current position and increases the position by the number of bytes
        /// written.
        /// </summary>
        /// <param name="source">The source byte array.</param>
        /// <param name="offset">
        /// the offset of byte array, must not be negative and not greater
        /// than <c>source.Length</c>.
        /// </param>
        /// <param name="length">
        /// the number of bytes to write, must not be negative and not
        /// greater than <c>source.Length - offset</c>.
        /// </param>
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
        public virtual ByteBuffer Put(byte[] source, int offset, int length)
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
        /// Writes all the remaining bytes of the <paramref name="source"/> byte buffer to this
        /// buffer's current position, and increases both buffers' position by the
        /// number of bytes copied.
        /// </summary>
        /// <param name="source">The source byte buffer.</param>
        /// <returns>This buffer.</returns>
        /// <exception cref="BufferOverflowException">If <c>source.Remaining</c> is greater than this buffer's <see cref="Buffer.Remaining"/>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="source"/> is this buffer.</exception>
        /// <exception cref="ReadOnlyBufferException">If no changes may be made to the contents of this buffer.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="source"/> is <c>null</c>.</exception>
        public virtual ByteBuffer Put(ByteBuffer source)
        {
            if (source is null)
                throw new ArgumentNullException(nameof(source));
            if (ReferenceEquals(source, this))
                throw new ArgumentException(J2N.SR.Format(SR.Argument_MustNotBeThis, nameof(source), nameof(source)));
            if (source.Remaining > Remaining)
                throw new BufferOverflowException();
            if (IsReadOnly)
                throw new ReadOnlyBufferException(); // J2N: Harmony has a bug - it shouldn't read the source and change its position unless this buffer is writable

            byte[] contents = new byte[source.Remaining];
            source.Get(contents);
            Put(contents);
            return this;
        }

        /// <summary>
        /// Write a <see cref="byte"/> to the specified index of this buffer without changing the
        /// position.
        /// </summary>
        /// <param name="index">The index, must not be negative and less than the limit.</param>
        /// <param name="value">The <see cref="byte"/> to write.</param>
        /// <returns>This buffer.</returns>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="index"/> is invalid.</exception>
        /// <exception cref="ReadOnlyBufferException">If no changes may be made to the contents of this buffer.</exception>
        public abstract ByteBuffer Put(int index, byte value);

        /// <summary>
        /// Writes the given <see cref="char"/> to the current position and increases the position
        /// by 2.
        /// <para/>
        /// The <see cref="char"/> is converted to bytes using the current byte order.
        /// </summary>
        /// <param name="value">The <see cref="char"/> to write.</param>
        /// <returns>This buffer.</returns>
        /// <exception cref="BufferOverflowException">If position is greater than <c>limit - 2</c>.</exception>
        /// <exception cref="ReadOnlyBufferException">If no changes may be made to the contents of this buffer.</exception>
        public abstract ByteBuffer PutChar(char value);

        /// <summary>
        /// Writes the given <see cref="char"/> to the specified index of this buffer.
        /// <para/>
        /// The <see cref="char"/> is converted to bytes using the current byte order. The position
        /// is not changed.
        /// </summary>
        /// <param name="index">The index, must not be negative and equal or less than <c>limit - 2</c>.</param>
        /// <param name="value">The <see cref="char"/> to write.</param>
        /// <returns>This buffer.</returns>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="index"/> is invalid.</exception>
        /// <exception cref="ReadOnlyBufferException">If no changes may be made to the contents of this buffer.</exception>
        public abstract ByteBuffer PutChar(int index, char value);

        /// <summary>
        /// Writes the given <see cref="double"/> to the current position and increases the position
        /// by 8.
        /// <para/>
        /// The <see cref="double"/> is converted to bytes using the current byte order.
        /// </summary>
        /// <param name="value">The <see cref="double"/> to write.</param>
        /// <returns>This buffer.</returns>
        /// <exception cref="BufferOverflowException">If position is greater than <c>limit - 8</c>.</exception>
        /// <exception cref="ReadOnlyBufferException">If no changes may be made to the contents of this buffer.</exception>
        public abstract ByteBuffer PutDouble(double value);

        /// <summary>
        /// Writes the given <see cref="double"/> to the specified index of this buffer.
        /// <para/>
        /// The <see cref="double"/> is converted to bytes using the current byte order. The
        /// position is not changed.
        /// </summary>
        /// <param name="index">The index, must not be negative and equal or less than <c>limit - 8</c>.</param>
        /// <param name="value">The <see cref="double"/> to write.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="index"/> is invalid.</exception>
        /// <exception cref="ReadOnlyBufferException">If no changes may be made to the contents of this buffer.</exception>
        public abstract ByteBuffer PutDouble(int index, double value);

        /// <summary>
        /// Writes the given <see cref="float"/> to the current position and increases the position
        /// by 4.
        /// <para/>
        /// The <see cref="float"/> is converted to bytes using the current byte order.
        /// <para/>
        /// NOTE: This was putSingle() in the JDK
        /// </summary>
        /// <param name="value">The <see cref="float"/> to write.</param>
        /// <returns>This buffer.</returns>
        /// <exception cref="BufferOverflowException">If position is greater than <c>limit - 4</c>.</exception>
        /// <exception cref="ReadOnlyBufferException">If no changes may be made to the contents of this buffer.</exception>
        public abstract ByteBuffer PutSingle(float value);

        /// <summary>
        /// Writes the given <see cref="float"/> to the specified index of this buffer.
        /// <para/>
        /// The <see cref="float"/> is converted to bytes using the current byte order. The
        /// position is not changed.
        /// <para/>
        /// NOTE: This was putSingle() in the JDK
        /// </summary>
        /// <param name="index">The index, must not be negative and equal or less than <c>limit - 4</c>.</param>
        /// <param name="value">The <see cref="float"/> to write.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="index"/> is invalid.</exception>
        /// <exception cref="ReadOnlyBufferException">If no changes may be made to the contents of this buffer.</exception>
        public abstract ByteBuffer PutSingle(int index, float value);

        /// <summary>
        /// Writes the given <see cref="int"/> to the current position and increases the position by
        /// 4.
        /// <para/>
        /// The <see cref="int"/> is converted to bytes using the current byte order.
        /// <para/>
        /// NOTE: This was putInt() in the JDK
        /// </summary>
        /// <param name="value">The <see cref="int"/> to write.</param>
        /// <returns>This buffer.</returns>
        /// <exception cref="BufferOverflowException">If position is greater than <c>limit - 4</c>.</exception>
        /// <exception cref="ReadOnlyBufferException">If no changes may be made to the contents of this buffer.</exception>
        public abstract ByteBuffer PutInt32(int value);

        /// <summary>
        /// Writes the given <see cref="int"/> to the specified index of this buffer.
        /// <para/>
        /// The <see cref="int"/> is converted to bytes using the current byte order. The position
        /// is not changed.
        /// <para/>
        /// NOTE: This was putInt() in the JDK
        /// </summary>
        /// <param name="index">The index, must not be negative and equal or less than <c>limit - 4</c>.</param>
        /// <param name="value">The <see cref="int"/> to write.</param>
        /// <returns>This buffer.</returns>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="index"/> is invalid.</exception>
        /// <exception cref="ReadOnlyBufferException">If no changes may be made to the contents of this buffer.</exception>
        public abstract ByteBuffer PutInt32(int index, int value);

        /// <summary>
        /// Writes the given <see cref="long"/> to the current position and increases the position
        /// by 8.
        /// <para/>
        /// The <see cref="long"/> is converted to bytes using the current byte order.
        /// <para/>
        /// NOTE: This was putLong() in the JDK
        /// </summary>
        /// <param name="value">The <see cref="long"/> to write.</param>
        /// <returns>This buffer.</returns>
        /// <exception cref="BufferOverflowException">If position is greater than <c>limit - 8</c>.</exception>
        /// <exception cref="ReadOnlyBufferException">If no changes may be made to the contents of this buffer.</exception>
        public abstract ByteBuffer PutInt64(long value);

        /// <summary>
        /// Writes the given <see cref="long"/> to the specified index of this buffer.
        /// <para/>
        /// The <see cref="long"/> is converted to bytes using the current byte order. The position
        /// is not changed.
        /// <para/>
        /// NOTE: This was putLong() in the JDK
        /// </summary>
        /// <param name="index">The index, must not be negative and equal or less than <c>limit - 8</c>.</param>
        /// <param name="value">The <see cref="long"/> to write.</param>
        /// <returns>This buffer.</returns>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="index"/> is invalid.</exception>
        /// <exception cref="ReadOnlyBufferException">If no changes may be made to the contents of this buffer.</exception>
        public abstract ByteBuffer PutInt64(int index, long value);

        /// <summary>
        /// Writes the given <see cref="short"/> to the current position and increases the position
        /// by 2.
        /// <para/>
        /// The <see cref="short"/> is converted to bytes using the current byte order.
        /// <para/>
        /// NOTE: This was putShort() in the JDK
        /// </summary>
        /// <param name="value">The <see cref="short"/> to write.</param>
        /// <returns>This buffer.</returns>
        /// <exception cref="BufferOverflowException">If position is greater than <c>limit - 2</c>.</exception>
        /// <exception cref="ReadOnlyBufferException">If no changes may be made to the contents of this buffer.</exception>
        public abstract ByteBuffer PutInt16(short value);

        /// <summary>
        /// Writes the given <see cref="short"/> to the specified index of this buffer.
        /// <para/>
        /// The <see cref="short"/> is converted to bytes using the current byte order. The
        /// position is not changed.
        /// <para/>
        /// NOTE: This was putShort() in the JDK
        /// </summary>
        /// <param name="index">The index, must not be negative and equal or less than <c>limit - 2</c>.</param>
        /// <param name="value">The <see cref="short"/> to write.</param>
        /// <returns>This buffer.</returns>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="index"/> is invalid.</exception>
        /// <exception cref="ReadOnlyBufferException">If no changes may be made to the contents of this buffer.</exception>
        public abstract ByteBuffer PutInt16(int index, short value);

        /// <summary>
        /// Returns a sliced buffer that shares its content with this buffer.
        /// <para/>
        /// The sliced buffer's capacity will be this buffer's
        /// <see cref="Buffer.Remaining"/>, and it's zero position will correspond to
        /// this buffer's current position. The new buffer's position will be 0,
        /// limit will be its capacity, and its mark is cleared. The new buffer's
        /// read-only property and byte order are the same as this buffer's.
        /// <para/>
        /// The new buffer shares its content with this buffer, which means either
        /// buffer's change of content will be visible to the other. The two buffer's
        /// position, limit and mark are independent.
        /// </summary>
        /// <returns>A sliced buffer that shares its content with this buffer.</returns>
        public abstract ByteBuffer Slice();

        /// <summary>
        /// Returns a string representing the state of this byte buffer.
        /// </summary>
        /// <returns>A string representing the state of this byte buffer.</returns>
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
