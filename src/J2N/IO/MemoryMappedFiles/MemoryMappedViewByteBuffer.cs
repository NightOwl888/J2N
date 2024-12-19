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

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO.MemoryMappedFiles;

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
        /// The <see cref="MemoryMappedViewAccessor"/> from a <see cref="MemoryMappedFile"/>.
        /// </summary>
        protected internal readonly MemoryMappedViewAccessor accessor;

        /// <summary>
        /// The current offset.
        /// </summary>
        protected internal readonly int offset;

        /// <summary>
        /// Initializes a new instance of <see cref="MemoryMappedViewByteBuffer"/>
        /// with the specified <paramref name="accessor"/> and <paramref name="capacity"/>.
        /// </summary>
        /// <param name="accessor">A <see cref="MemoryMappedViewAccessor"/>.</param>
        /// <param name="capacity">The capacity of the buffer.</param>
        internal MemoryMappedViewByteBuffer(MemoryMappedViewAccessor accessor, int capacity)
            : base(capacity)
        {
            this.accessor = accessor;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="MemoryMappedViewByteBuffer"/>
        /// with the specified <paramref name="accessor"/> and <paramref name="capacity"/>.
        /// </summary>
        /// <param name="accessor">A <see cref="MemoryMappedViewAccessor"/>.</param>
        /// <param name="capacity">The capacity of the buffer.</param>
        /// <param name="offset">The offset of the buffer.</param>
        internal MemoryMappedViewByteBuffer(MemoryMappedViewAccessor accessor, int capacity, int offset)
            : this(accessor, capacity)
        {
            this.offset = offset;
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
        public override ByteBuffer Get(byte[] destination, int offset, int length)
        {
            ThrowHelper.ThrowIfNull(destination, ExceptionArgument.destination);

            int len = destination.Length;
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset), SR.ArgumentOutOfRange_NeedNonNegNum);
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length), SR.ArgumentOutOfRange_NeedNonNegNum);
            if (offset > len - length) // Checks for int overflow
                throw new ArgumentOutOfRangeException(nameof(length), SR.ArgumentOutOfRange_IndexLength);
            if (length > Remaining)
                throw new BufferUnderflowException();

            // we need to check for 0-length reads, since 
            // ReadArray will throw an ArgumentOutOfRange exception if position is at
            // the end even when nothing is read
            if (length > 0)
            {
                accessor.ReadArray(Ix(NextGetIndex(length)), destination, offset, length);
            }

            return this;
        }

        /// <summary>
        /// Returns the byte at the current position and increases the position by 1.
        /// </summary>
        /// <returns>The byte at the current position.</returns>
        /// <exception cref="BufferUnderflowException">If the position is equal or greater than limit.</exception>
        public override byte Get()
        {
            return accessor.ReadByte(Ix(NextGetIndex()));
        }

        /// <summary>
        /// Returns the byte at the specified index and does not change the position.
        /// </summary>
        /// <param name="index">The index, must not be negative and less than limit.</param>
        /// <returns>The byte at the specified index.</returns>
        /// <exception cref="ArgumentOutOfRangeException">If index is invalid.</exception>
        public override byte Get(int index)
        {
            return accessor.ReadByte(Ix(CheckIndex(index)));
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
            return LoadInt32(NextGetIndex(4));
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
            return LoadInt32(CheckIndex(index, 4));
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
            return LoadInt64(NextGetIndex(8));
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
            return LoadInt64(CheckIndex(index, 8));
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
            return LoadInt16(NextGetIndex(2));
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
            return LoadInt16(CheckIndex(index, 2));
        }

        //public override bool IsDirect => false; // J2N: IsDirect Not supported

        /// <summary>
        /// Reads the <see cref="int"/> at the specified index using the byte order
        /// specified by <see cref="ByteBuffer.Order"/>. No validation is done on
        /// <paramref name="index"/>.
        /// </summary>
        /// <param name="index">The index to begin reading bytes.</param>
        /// <returns>The <see cref="int"/> at the specified index.</returns>
        [SuppressMessage("Style", "IDE0054:Use compound assignment", Justification = "Aligning code style with Apache Harmony")]
        protected int LoadInt32(int index)
        {
            int baseOffset = offset + index;
            int bytes = 0;
            if (order == Endianness.BigEndian)
            {
                for (int i = 0; i < 4; i++)
                {
                    bytes = bytes << 8;
                    bytes = bytes | (accessor.ReadByte(baseOffset + i) & 0xFF);
                }
            }
            else
            {
                //for (int i = 3; i >= 0; i--)
                //{
                //    bytes = bytes << 8;
                //    bytes = bytes | (accessor.ReadByte(baseOffset + i) & 0xFF);
                //}
                bytes = accessor.ReadInt32(baseOffset);
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
        [SuppressMessage("Style", "IDE0054:Use compound assignment", Justification = "Aligning code style with Apache Harmony")]
        protected long LoadInt64(int index)
        {
            int baseOffset = offset + index;
            long bytes = 0;
            if (order == Endianness.BigEndian)
            {
                for (int i = 0; i < 8; i++)
                {
                    bytes = bytes << 8;
                    bytes = bytes | (uint)(accessor.ReadByte(baseOffset + i) & 0xFF);
                }
            }
            else
            {
                //for (int i = 7; i >= 0; i--)
                //{
                //    bytes = bytes << 8;
                //    bytes = bytes | (uint)(accessor.ReadByte(baseOffset + i) & 0xFF);
                //}
                bytes = accessor.ReadInt64(baseOffset);
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
            short bytes; // J2N: Unnecessary variable assignment
            if (order == Endianness.BigEndian)
            {
                bytes = (short)(accessor.ReadByte(baseOffset) << 8);
                bytes |= (short)(accessor.ReadByte(baseOffset + 1) & 0xFF);
            }
            else
            {
                //bytes = (short)(accessor.ReadByte(baseOffset + 1) << 8);
                //bytes |= (short)(accessor.ReadByte(baseOffset) & 0xFF);
                bytes = accessor.ReadInt16(baseOffset);
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
            if (order == Endianness.BigEndian)
            {
                for (int i = 3; i >= 0; i--)
                {
                    accessor.Write(baseOffset + i, (byte)(value & 0xFF));
                    value >>= 8;
                }
            }
            else
            {
                accessor.Write(baseOffset, value);
                //for (int i = 0; i <= 3; i++)
                //{
                //    backingArray[baseOffset + i] = (byte)(value & 0xFF);
                //    value >>= 8;
                //}
            }
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
            if (order == Endianness.BigEndian)
            {
                for (int i = 7; i >= 0; i--)
                {
                    accessor.Write(baseOffset + i, (byte)(value & 0xFF));
                    value >>= 8;
                }
            }
            else
            {
                accessor.Write(baseOffset, value);
                //for (int i = 0; i <= 7; i++)
                //{
                //    backingArray[baseOffset + i] = (byte)(value & 0xFF);
                //    value >>= 8;
                //}
            }
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
            if (order == Endianness.BigEndian)
            {
                accessor.Write(baseOffset, (byte)((value >> 8) & 0xFF));
                accessor.Write(baseOffset + 1, (byte)(value & 0xFF));
            }
            else
            {
                accessor.Write(baseOffset, value);
            }
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

        #region Private Helper Methods

        internal int Ix(int i)
        {
            return i + offset;
        }

        /// <summary>
        /// Checks the current position against the limit, throwing a
        /// <see cref="BufferUnderflowException"/> if it is not smaller than the limit, and then
        /// increments the position.
        /// </summary>
        /// <returns>The current position value, before it is incremented</returns>
        internal int NextGetIndex()
        {
            if (position >= limit)
            {
                throw new BufferUnderflowException();
            }
            return position++;
        }

        internal int NextGetIndex(int numberOfBytes)
        {
            if (limit - position < numberOfBytes)
            {
                throw new BufferUnderflowException();
            }
            int p = position;
            position += numberOfBytes;
            return p;
        }

        /// <summary>
        /// Checks the current position against the limit, throwing a <see cref="BufferOverflowException"/>
        /// if it is not smaller than the limit, and then
        /// increments the position.
        /// </summary>
        /// <returns>The current position value, before it is incremented</returns>
        internal int NextPutIndex()
        {
            if (position >= limit)
            {
                throw new BufferOverflowException();
            }
            return position++;
        }

        internal int NextPutIndex(int numberOfBytes)
        {
            if (limit - position < numberOfBytes)
            {
                throw new BufferOverflowException();
            }
            int p = position;
            position += numberOfBytes;
            return p;
        }

        /// <summary>
        /// Checks the given index against the limit, throwing an <see cref="ArgumentOutOfRangeException"/> 
        /// if it is not smaller than the limit or is smaller than zero.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        internal int CheckIndex(int index)
        {
            if ((index < 0) || (index >= limit))
                throw new ArgumentOutOfRangeException(nameof(index));

            return index;
        }

        internal int CheckIndex(int index, int numberOfBytes)
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index));
            if (numberOfBytes > limit - index)
                throw new ArgumentOutOfRangeException(nameof(numberOfBytes));

            return index;
        }

        #endregion Private Helper Methods
    }
}
