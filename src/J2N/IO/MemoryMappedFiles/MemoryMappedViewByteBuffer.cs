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

using J2N.Buffers.Binary;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO.MemoryMappedFiles;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.AccessControl;

#pragma warning disable CS9191 // The ref parameter is equivalent to in

namespace J2N.IO.MemoryMappedFiles
{
    /// <summary>
    /// A byte buffer whose content is a memory-mapped region of a file.
    /// 
    /// <para/>Mapped byte buffers are created via the
    /// <see cref="MemoryMappedFileExtensions.CreateViewByteBuffer(MemoryMappedFile, long, long, MemoryMappedFileAccess)"/>
    /// extension method. This class extends the <see cref="ByteBuffer"/> class with
    /// operations that are specific to memory-mapped file regions.
    /// 
    /// <para/>A memory-mapped byte buffer and the file mapping that it represents remain
    /// valid until it is disposed.
    /// 
    /// <para/>The content of a memory-mapped byte buffer can change at any time, for example
    /// if the content of the corresponding region of the mapped file is changed by
    /// this program or another.  Whether or not such changes occur, and when they
    /// occur, is operating-system dependent and therefore unspecified.
    /// </summary>
    public abstract class MemoryMappedViewByteBuffer : ByteBuffer, IDisposable
    {
        /// <summary>
        /// The <see cref="MemoryMappedDirectAccessorReference"/> from a <see cref="MemoryMappedFile"/>.
        /// </summary>
        internal readonly MemoryMappedDirectAccessorReference accessor;

        /// <summary>
        /// The current offset.
        /// </summary>
        internal readonly int offset;

        /// <summary>
        /// Whether or not this is a clone. Only the main view will be able to dispose the accessor.
        /// </summary>
        internal bool isClone;

        /// <summary>
        /// Initializes a new instance of <see cref="MemoryMappedViewByteBuffer"/>
        /// with the specified <paramref name="accessor"/> and <paramref name="capacity"/>.
        /// </summary>
        /// <param name="accessor">A <see cref="MemoryMappedDirectAccessorReference"/>.</param>
        /// <param name="capacity">The capacity of the buffer.</param>
        internal MemoryMappedViewByteBuffer(MemoryMappedDirectAccessorReference accessor, int capacity)
            : base(capacity)
        {
            this.accessor = accessor;
            this.isClone = false;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="MemoryMappedViewByteBuffer"/>
        /// with the specified <paramref name="accessor"/> and <paramref name="capacity"/>.
        /// </summary>
        /// <param name="accessor">A <see cref="MemoryMappedDirectAccessorReference"/>.</param>
        /// <param name="capacity">The capacity of the buffer.</param>
        /// <param name="offset">The offset of the buffer.</param>
        internal MemoryMappedViewByteBuffer(MemoryMappedDirectAccessorReference accessor, int capacity, int offset)
            : this(accessor, capacity)
        {
            this.offset = offset;
        }

        /// <inheritdoc/>
        public override ByteBuffer Get(byte[] destination)
        {
            if (destination is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.destination);
            return Get(destination.AsSpan());
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
        // Implementation provided by Vincent Van Den Berghe: http://git.net/ml/general/2017-02/msg31639.html
        public override ByteBuffer Get(byte[] destination, int offset, int length) // J2N TODO: API - Rename startIndex instead of offset
        {
            if (destination is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.destination);
            if (offset < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(offset, ExceptionArgument.offset);
            if (length < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(length, ExceptionArgument.length);
            if (offset > destination.Length - length) // Checks for int overflow
                ThrowHelper.ThrowArgumentOutOfRange_IndexLengthArray(offset, ExceptionArgument.offset, length);
            if (length > Remaining)
                throw new BufferUnderflowException();

            //// we need to check for 0-length reads, since 
            //// ReadArray will throw an ArgumentOutOfRange exception if position is at
            //// the end even when nothing is read
            //if (length > 0)
            //{
            //    accessor.ReadArray(Ix(NextGetIndex(length)), destination, offset, length);
            //}

            accessor.AsSpan(this.offset + position, length).CopyTo(destination.AsSpan(offset, length));
            position += length;
            return this;
        }

        /// <summary>
        /// Reads bytes from the current position into the specified span,
        /// and increases the position by the number of bytes read.
        /// <para/>
        /// The <see cref="Span{Byte}.Length"/> property is used to determine
        /// how many bytes to read.
        /// </summary>
        /// <param name="destination">The target span, sliced to the proper position, if necessary.</param>
        /// <returns>This buffer.</returns>
        /// <exception cref="BufferUnderflowException">If <see cref="Span{Byte}.Length"/> is greater than
        /// <see cref="Buffer.Remaining"/>.</exception>
        public override ByteBuffer Get(Span<byte> destination) // J2N specific
        {
            int length = destination.Length;
            if (length > Remaining)
                throw new BufferUnderflowException();

            accessor.AsSpan(offset + position, length).CopyTo(destination);
            position += length;
            return this;
        }

        /// <summary>
        /// Returns the byte at the current position and increases the position by 1.
        /// </summary>
        /// <returns>The byte at the current position.</returns>
        /// <exception cref="BufferUnderflowException">If the position is equal or greater than limit.</exception>
        public override byte Get()
        {
            if (position == limit)
            {
                throw new BufferUnderflowException();
            }
            return accessor[offset + position++];
        }

        /// <summary>
        /// Returns the byte at the specified index and does not change the position.
        /// </summary>
        /// <param name="index">The index, must not be negative and less than limit.</param>
        /// <returns>The byte at the specified index.</returns>
        /// <exception cref="ArgumentOutOfRangeException">If index is invalid.</exception>
        public override byte Get(int index)
        {
            if ((uint)index >= (uint)limit)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
            return accessor[offset + index];
        }

        /// <summary>
        /// Returns the <see cref="double"/> at the current position and increases the position by 8.
        /// <para/>
        /// The 8 bytes starting from the current position are composed into a <see cref="double"/>
        /// according to the current byte order and returned.
        /// </summary>
        /// <returns>The <see cref="double"/> at the current position.</returns>
        /// <exception cref="BufferUnderflowException">If the position is greater than <c>limit - 8</c>.</exception>
        public override sealed double GetDouble()
        {
            return BitConversion.Int64BitsToDouble(GetInt64());
        }

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
        public override sealed double GetDouble(int index)
        {
            return BitConversion.Int64BitsToDouble(GetInt64(index));
        }

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
        public override sealed float GetSingle()
        {
            return BitConversion.Int32BitsToSingle(GetInt32());
        }

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
        public override sealed float GetSingle(int index)
        {
            return BitConversion.Int32BitsToSingle(GetInt32(index));
        }

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
        public override int GetInt32()
        {
            int newPosition = position + sizeof(int);
            if (newPosition < 0)
                throw new ArgumentOutOfRangeException(nameof(Position)); // J2N TODO: Change this to InvalidOperationException, since it is not an argument
            if (newPosition > limit)
                throw new BufferUnderflowException();

            int result = LoadInt32(position);
            position = newPosition;
            return result;
        }

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
        public override int GetInt32(int index)
        {
            int newIndex = index + sizeof(int);
            if (index < 0 || (uint)newIndex > (uint)limit) // J2N: Added check for overflowing integer
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.index);
            }
            return LoadInt32(index);
        }

        /// <summary>
        /// Returns the <see cref="long"/> at the specified index and increases the position by 8.
        /// <para/>
        /// NOTE: This was getLong() in the JDK
        /// </summary>
        /// <returns>The <see cref="long"/> at the specified index.</returns>
        /// <exception cref="ArgumentOutOfRangeException">If index is invalid.</exception>
        public override long GetInt64()
        {
            int newPosition = position + sizeof(long);
            if (newPosition < 0) // J2N: Added check for overflowing integer
                throw new ArgumentOutOfRangeException(nameof(Position)); // J2N TODO: Change this to InvalidOperationException, since it is not an argument
            if (newPosition > limit)
                throw new BufferUnderflowException();

            long result = LoadInt64(position);
            position = newPosition;
            return result;
        }

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
        public override long GetInt64(int index)
        {
            int newIndex = index + sizeof(long);
            if (index < 0 || (uint)newIndex > (uint)limit) // J2N: Added check for overflowing integer
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.index);
            }
            return LoadInt64(index);
        }

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
        public override short GetInt16()
        {
            int newPosition = position + sizeof(short);
            if (newPosition < 0) // J2N: Added check for overflowing integer
                throw new ArgumentOutOfRangeException(nameof(Position)); // J2N TODO: Change this to InvalidOperationException, since it is not an argument
            if (newPosition > limit)
                throw new BufferUnderflowException();

            short result = LoadInt16(position);
            position = newPosition;
            return result;
        }

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
        public override short GetInt16(int index)
        {
            int newIndex = index + sizeof(short);
            if (index < 0 || (uint)newIndex > (uint)limit) // J2N: Added check for overflowing integer
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.index);
            }
            return LoadInt16(index);
        }

        //public override bool IsDirect => false; // J2N: IsDirect Not supported

        /// <summary>
        /// Reads the <see cref="int"/> at the specified index using the byte order
        /// specified by <see cref="ByteBuffer.Order"/>. No validation is done on
        /// <paramref name="index"/>.
        /// </summary>
        /// <param name="index">The index to begin reading bytes.</param>
        /// <returns>The <see cref="int"/> at the specified index.</returns>
        protected int LoadInt32(int index)
        {
            int baseOffset = offset + index;
            int bytes = MemoryMarshal.Read<int>(accessor.AsSpan(baseOffset, sizeof(int)));
            if (!IsRequestedEndianness)
            {
                bytes = BinaryPrimitive.ReverseEndianness(bytes);
            }
            return bytes;
        }

        /// <summary>
        /// Reads the <see cref="long"/> at the specified index using the byte order
        /// specified by <see cref="ByteBuffer.Order"/>. No validation is done on
        /// <paramref name="index"/>.
        /// </summary>
        /// <param name="index">The index to begin reading bytes.</param>
        /// <returns>The <see cref="long"/> at the specified index.</returns>
        protected long LoadInt64(int index)
        {
            int baseOffset = offset + index;
            long bytes = MemoryMarshal.Read<long>(accessor.AsSpan(baseOffset, sizeof(long)));
            if (!IsRequestedEndianness)
            {
                bytes = BinaryPrimitive.ReverseEndianness(bytes);
            }
            return bytes;
        }

        /// <summary>
        /// Reads the <see cref="short"/> at the specified index using the byte order
        /// specified by <see cref="ByteBuffer.Order"/>. No validation is done on
        /// <paramref name="index"/>.
        /// </summary>
        /// <param name="index">The index to begin reading bytes.</param>
        /// <returns>The <see cref="short"/> at the specified index.</returns>
        protected short LoadInt16(int index)
        {
            int baseOffset = offset + index;
            short bytes = MemoryMarshal.Read<short>(accessor.AsSpan(baseOffset, sizeof(short)));
            if (!IsRequestedEndianness)
            {
                bytes = BinaryPrimitive.ReverseEndianness(bytes);
            }
            return bytes;
        }

        /// <summary>
        /// Writes the <see cref="int"/> at the specified index using the byte order
        /// specified by <see cref="ByteBuffer.Order"/>. No validation is done on
        /// <paramref name="index"/>.
        /// </summary>
        /// <param name="index">The index to begin writing bytes.</param>
        /// <param name="value">The value to write.</param>
        /// <returns>The <see cref="int"/> at the specified index.</returns>
        protected void Store(int index, int value)
        {
            int baseOffset = offset + index;
            if (!IsRequestedEndianness)
            {
                value = BinaryPrimitive.ReverseEndianness(value);
            }
            MemoryMarshal.Write<int>(accessor.AsSpan(baseOffset, sizeof(int)), ref value);
        }

        /// <summary>
        /// Writes the <see cref="long"/> at the specified index using the byte order
        /// specified by <see cref="ByteBuffer.Order"/>. No validation is done on
        /// <paramref name="index"/>.
        /// </summary>
        /// <param name="index">The index to begin writing bytes.</param>
        /// <param name="value">The value to write.</param>
        /// <returns>The <see cref="long"/> at the specified index.</returns>
        protected void Store(int index, long value)
        {
            int baseOffset = offset + index;
            if (!IsRequestedEndianness)
            {
                value = BinaryPrimitive.ReverseEndianness(value);
            }
            MemoryMarshal.Write<long>(accessor.AsSpan(baseOffset, sizeof(long)), ref value);
        }

        /// <summary>
        /// Writes the <see cref="short"/> at the specified index using the byte order
        /// specified by <see cref="ByteBuffer.Order"/>. No validation is done on
        /// <paramref name="index"/>.
        /// </summary>
        /// <param name="index">The index to begin writing bytes.</param>
        /// <param name="value">The value to write.</param>
        /// <returns>The <see cref="short"/> at the specified index.</returns>
        protected void Store(int index, short value)
        {
            int baseOffset = offset + index;
            if (!IsRequestedEndianness)
            {
                value = BinaryPrimitive.ReverseEndianness(value);
            }
            MemoryMarshal.Write<short>(accessor.AsSpan(baseOffset, sizeof(short)), ref value);
        }

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
        public override sealed CharBuffer AsCharBuffer()
        {
            return CharToByteBufferAdapter.Wrap(this);
        }

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
        public override sealed DoubleBuffer AsDoubleBuffer()
        {
            return DoubleToByteBufferAdapter.Wrap(this);
        }

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
        public override sealed SingleBuffer AsSingleBuffer()
        {
            return SingleToByteBufferAdapter.Wrap(this);
        }

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
        public override sealed Int32Buffer AsInt32Buffer()
        {
            return Int32ToByteBufferAdapter.Wrap(this);
        }

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
        public override sealed Int64Buffer AsInt64Buffer()
        {
            return Int64ToByteBufferAdapter.Wrap(this);
        }

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
        public override sealed Int16Buffer AsInt16Buffer()
        {
            return Int16ToByteBufferAdapter.Wrap(this);
        }

        /// <summary>
        /// Returns the <see cref="char"/> at the current position and increases the position by 2.
        /// <para/>
        /// The 2 bytes starting at the current position are composed into a char
        /// according to the current byte order and returned.
        /// </summary>
        /// <returns>The <see cref="char"/> at the current position.</returns>
        /// <exception cref="BufferUnderflowException">If the position is greater than <c>limit - 2</c>.</exception>
        public override sealed char GetChar()
        {
            return (char)GetInt16();
        }

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
        public override sealed char GetChar(int index)
        {
            return (char)GetInt16(index);
        }

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
        public override sealed ByteBuffer PutChar(char value)
        {
            return PutInt16((short)value);
        }

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
        public override sealed ByteBuffer PutChar(int index, char value)
        {
            return PutInt16(index, (short)value);
        }

        /// <summary>
        /// Writes all changes of the buffer to the mapped file.
        /// </summary>
        public virtual void Flush()
        {
            accessor.Flush();
        }

        /// <summary>
        /// Cleans up all resources associated with this <see cref="MemoryMappedViewByteBuffer"/>.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Cleans up all resources associated with this <see cref="MemoryMappedViewByteBuffer"/>.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                accessor.Dispose();
            }
        }
    }
}
