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
using System.Buffers.Binary;
using System.Runtime.InteropServices;

#pragma warning disable CS9191 // The ref parameter is equivalent to in

namespace J2N.IO
{
    /// <summary>
    /// <see cref="HeapByteBuffer"/>, <see cref="ReadWriteHeapByteBuffer"/> and <see cref="ReadOnlyHeapByteBuffer"/> compose
    /// the implementation of array based <see cref="byte"/> buffers.
    /// <para/>
    /// <see cref="HeapByteBuffer"/> implements all the shared readonly methods and is extended by
    /// the other two classes.
    /// <para/>
    /// All methods are sealed for runtime performance.
    /// </summary>
    internal abstract class HeapByteBuffer : ByteBuffer
    {
        protected internal readonly byte[] backingArray;

        protected internal readonly int offset;

        internal HeapByteBuffer(byte[] backingArray)
            : this(backingArray, backingArray.Length, 0)
        { }

        internal HeapByteBuffer(int capacity)
            : this(new byte[capacity], capacity, 0)
        { }

        internal HeapByteBuffer(byte[] backingArray, int capacity, int offset)
            : base(capacity)
        {
            this.backingArray = backingArray;
            this.offset = offset;

            if (offset + capacity > backingArray.Length)
            {
                throw new ArgumentOutOfRangeException("", $"{nameof(offset)} + {nameof(capacity)} > {nameof(backingArray.Length)}");
            }
        }

        /// <inheritdoc/>
        public override ByteBuffer Get(byte[] destination)
        {
            if (destination is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.destination);
            return Get(destination.AsSpan());
        }

        /*
         * Override ByteBuffer.get(byte[], int, int) to improve performance.
         * 
         * (non-Javadoc)
         * 
         * @see java.nio.ByteBuffer#get(byte[], int, int)
         */

        /// <seealso cref="ByteBuffer.Get(byte[], int, int)"/>
        public override sealed ByteBuffer Get(byte[] destination, int offset, int length) // J2N TODO: API - Rename startIndex instead of offset
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

            System.Array.Copy(backingArray, this.offset + position, destination, offset, length);
            position += length;
            return this;
        }

        public override sealed ByteBuffer Get(Span<byte> destination) // J2N specific
        {
            int length = destination.Length;
            if (length > Remaining)
                throw new BufferUnderflowException();

            backingArray.AsSpan(offset + position, length).CopyTo(destination);
            position += length;
            return this;
        }

        public override sealed byte Get()
        {
            if (position == limit)
            {
                throw new BufferUnderflowException();
            }
            return backingArray[offset + position++];
        }


        public override sealed byte Get(int index)
        {
            if ((uint)index >= (uint)limit)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
            return backingArray[offset + index];
        }

        public override sealed double GetDouble()
        {
            return BitConversion.Int64BitsToDouble(GetInt64());
        }

        public override sealed double GetDouble(int index)
        {
            return BitConversion.Int64BitsToDouble(GetInt64(index));
        }

        public override sealed float GetSingle()
        {
            return BitConversion.Int32BitsToSingle(GetInt32());
        }

        public override sealed float GetSingle(int index)
        {
            return BitConversion.Int32BitsToSingle(GetInt32(index));
        }

        public override sealed int GetInt32()
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

        public override sealed int GetInt32(int index)
        {
            int newIndex = index + sizeof(int);
            if (index < 0 || (uint)newIndex > (uint)limit) // J2N: Added check for overflowing integer
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.index);
            }
            return LoadInt32(index);
        }

        public override sealed long GetInt64()
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

        public override sealed long GetInt64(int index)
        {
            int newIndex = index + sizeof(long);
            if (index < 0 || (uint)newIndex > (uint)limit) // J2N: Added check for overflowing integer
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.index);
            }
            return LoadInt64(index);
        }

        public override sealed short GetInt16()
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

        public override sealed short GetInt16(int index)
        {
            int newIndex = index + sizeof(short);
            if (index < 0 || (uint)newIndex > (uint)limit) // J2N: Added check for overflowing integer
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.index);
            }
            return LoadInt16(index);
        }

        //public override sealed bool IsDirect
        //{
        //    get { return false; }
        //}

        protected int LoadInt32(int index)
        {
            int baseOffset = offset + index;
            int bytes = MemoryMarshal.Read<int>(backingArray.AsSpan(baseOffset, sizeof(int)));
            if (order == Endianness.BigEndian)
            {
                bytes = BinaryPrimitives.ReverseEndianness(bytes);
            }
            return bytes;
        }

        protected long LoadInt64(int index)
        {
            int baseOffset = offset + index;
            long bytes = MemoryMarshal.Read<long>(backingArray.AsSpan(baseOffset, sizeof(long)));
            if (order == Endianness.BigEndian)
            {
                bytes = BinaryPrimitives.ReverseEndianness(bytes);
            }
            return bytes;
        }

        protected short LoadInt16(int index)
        {
            int baseOffset = offset + index;
            short bytes = MemoryMarshal.Read<short>(backingArray.AsSpan(baseOffset, sizeof(short)));
            if (order == Endianness.BigEndian)
            {
                bytes = BinaryPrimitives.ReverseEndianness(bytes);
            }
            return bytes;
        }

        protected void Store(int index, int value)
        {
            int baseOffset = offset + index;
            if (order == Endianness.BigEndian)
            {
                value = BinaryPrimitives.ReverseEndianness(value);
            }
            MemoryMarshal.Write<int>(backingArray.AsSpan(baseOffset, sizeof(int)), ref value);
        }

        protected void Store(int index, long value)
        {
            int baseOffset = offset + index;
            if (order == Endianness.BigEndian)
            {
                value = BinaryPrimitives.ReverseEndianness(value);
            }
            MemoryMarshal.Write<long>(backingArray.AsSpan(baseOffset, sizeof(long)), ref value);
        }

        protected void Store(int index, short value)
        {
            int baseOffset = offset + index;
            if (order == Endianness.BigEndian)
            {
                value = BinaryPrimitives.ReverseEndianness(value);
            }
            MemoryMarshal.Write<short>(backingArray.AsSpan(baseOffset, sizeof(short)), ref value);
        }

        public override sealed CharBuffer AsCharBuffer()
        {
            return CharToByteBufferAdapter.Wrap(this);
        }

        public override sealed DoubleBuffer AsDoubleBuffer()
        {
            return DoubleToByteBufferAdapter.Wrap(this);
        }

        public override sealed SingleBuffer AsSingleBuffer()
        {
            return SingleToByteBufferAdapter.Wrap(this);
        }

        public override sealed Int32Buffer AsInt32Buffer()
        {
            return Int32ToByteBufferAdapter.Wrap(this);
        }

        public override sealed Int64Buffer AsInt64Buffer()
        {
            return Int64ToByteBufferAdapter.Wrap(this);
        }

        public override sealed Int16Buffer AsInt16Buffer()
        {
            return Int16ToByteBufferAdapter.Wrap(this);
        }

        public override sealed char GetChar()
        {
            return (char)GetInt16();
        }

        public override sealed char GetChar(int index)
        {
            return (char)GetInt16(index);
        }

        public override sealed ByteBuffer PutChar(char value)
        {
            return PutInt16((short)value);
        }

        public override sealed ByteBuffer PutChar(int index, char value)
        {
            return PutInt16(index, (short)value);
        }
    }
}
