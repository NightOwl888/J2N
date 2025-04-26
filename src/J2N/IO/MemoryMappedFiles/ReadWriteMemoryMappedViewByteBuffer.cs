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
using System.Diagnostics;
using System.IO.MemoryMappedFiles;
using System.Threading;

namespace J2N.IO.MemoryMappedFiles
{
    /// <summary>
    /// <see cref="MemoryMappedViewByteBuffer"/>, <see cref="ReadWriteMemoryMappedViewByteBuffer"/> and <see cref="ReadOnlyMemoryMappedViewByteBuffer"/> compose
    /// the implementation of array based byte buffers.
    /// <para/>
    /// <see cref="ReadWriteMemoryMappedViewByteBuffer"/> extends <see cref="MemoryMappedViewByteBuffer"/> with all the write methods.
    /// <para/>
    /// This class is marked sealed for runtime performance.
    /// </summary>
    internal sealed class ReadWriteMemoryMappedViewByteBuffer : MemoryMappedViewByteBuffer
    {
        internal static ReadWriteMemoryMappedViewByteBuffer Copy(MemoryMappedViewByteBuffer other, int markOfOther)
        {
            return new ReadWriteMemoryMappedViewByteBuffer(other.accessor, other.Capacity, other.offset)
            {
                limit = other.Limit,
                position = other.Position,
                mark = markOfOther,
                Order = other.Order,
                isClone = true,
            };
        }

        /// <summary>
        /// Initializes a new instance of <see cref="ReadWriteMemoryMappedViewByteBuffer"/>
        /// with the specified <paramref name="accessor"/> and <paramref name="capacity"/>.
        /// </summary>
        /// <param name="accessor">A <see cref="MemoryMappedDirectAccessorReference"/>.</param>
        /// <param name="capacity">The capacity of the buffer.</param>
        internal ReadWriteMemoryMappedViewByteBuffer(MemoryMappedDirectAccessorReference accessor, int capacity)
            : base(accessor, capacity)
        { }

        /// <summary>
        /// Initializes a new instance of <see cref="ReadWriteMemoryMappedViewByteBuffer"/>
        /// with the specified <paramref name="accessor"/> and <paramref name="capacity"/>.
        /// </summary>
        /// <param name="accessor">A <see cref="MemoryMappedDirectAccessorReference"/>.</param>
        /// <param name="capacity">The capacity of the buffer.</param>
        /// <param name="offset">The offset of the buffer.</param>
        internal ReadWriteMemoryMappedViewByteBuffer(MemoryMappedDirectAccessorReference accessor, int capacity, int offset)
            : base(accessor, capacity, offset)
        { }

        public override ByteBuffer AsReadOnlyBuffer()
        {
            EnsureOpen();
            return ReadOnlyMemoryMappedViewByteBuffer.Copy(this, mark);
        }

        public override ByteBuffer Compact()
        {
            EnsureOpen();
            accessor.AsSpan(position + offset, Remaining).CopyTo(accessor.AsSpan(offset, Remaining));
            position = limit - position;
            limit = capacity;
            mark = UnsetMark;
            return this;
        }

        public override ByteBuffer Duplicate()
        {
            EnsureOpen();
            return Copy(this, mark);
        }

        public override bool IsReadOnly => false;

        protected override byte[] ProtectedArray => throw new NotSupportedException();

        protected override int ProtectedArrayOffset => offset;

        protected override bool ProtectedHasArray => false;


        public override ByteBuffer Put(byte value)
        {
            EnsureOpen();
            if (position == limit)
            {
                throw new BufferOverflowException();
            }

            accessor[offset + position++] = value;
            return this;
        }

        public override ByteBuffer Put(int index, byte value)
        {
            EnsureOpen();
            if ((uint)index >= (uint)limit)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            accessor[offset + index] = value;
            return this;
        }

        internal override ByteBuffer InternalPut(byte[] source)
        {
            if (source is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
            return Put(source.AsSpan());
        }

        /*
         * Override ByteBuffer.put(byte[], int, int) to improve performance.
         * 
         * (non-Javadoc)
         * 
         * @see java.nio.ByteBuffer#put(byte[], int, int)
         */

        public override ByteBuffer Put(byte[] source, int offset, int length) // J2N TODO: API - Rename startIndex instead of offset
        {
            EnsureOpen();
            if (source is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
            if (offset < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(offset, ExceptionArgument.offset);
            if (length < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(length, ExceptionArgument.length);
            if (offset > source.Length - length) // Checks for int overflow
                ThrowHelper.ThrowArgumentOutOfRange_IndexLengthArray(offset, ExceptionArgument.offset, length);
            if (length > Remaining)
                throw new BufferOverflowException();

            source.AsSpan(offset, length).CopyTo(accessor.AsSpan(base.offset + position, length));
            position += length;
            return this;
        }

        public override ByteBuffer Put(ReadOnlySpan<byte> source) // J2N specific
        {
            EnsureOpen();
            int length = source.Length;
            if (length > Remaining)
                throw new BufferOverflowException();

            source.CopyTo(accessor.AsSpan(offset + position, length));
            position += length;
            return this;
        }

        public override ByteBuffer PutDouble(double value)
        {
            return PutInt64(BitConversion.DoubleToRawInt64Bits(value));
        }

        public override ByteBuffer PutDouble(int index, double value)
        {
            return PutInt64(index, BitConversion.DoubleToRawInt64Bits(value));
        }

        public override ByteBuffer PutSingle(float value)
        {
            return PutInt32(BitConversion.SingleToRawInt32Bits(value));
        }

        public override ByteBuffer PutSingle(int index, float value)
        {
            return PutInt32(index, BitConversion.SingleToRawInt32Bits(value));
        }

        public override ByteBuffer PutInt32(int value)
        {
            int newPosition = position + sizeof(int);
            if ((uint)newPosition > (uint)limit) // J2N: Added check for overflowing integer
                throw new BufferOverflowException();

            Store(position, value);
            position = newPosition;
            return this;
        }

        public override ByteBuffer PutInt32(int index, int value)
        {
            int newIndex = index + sizeof(int);
            if (index < 0 || (uint)newIndex > (uint)limit) // J2N: Added check for overflowing integer
                throw new ArgumentOutOfRangeException(nameof(index));

            Store(index, value);
            return this;
        }

        public override ByteBuffer PutInt64(int index, long value)
        {
            int newIndex = index + sizeof(long);
            if (index < 0 || (uint)newIndex > (uint)limit) // J2N: Added check for overflowing integer
                throw new ArgumentOutOfRangeException(nameof(index));

            Store(index, value);
            return this;
        }

        public override ByteBuffer PutInt64(long value)
        {
            int newPosition = position + sizeof(long);
            if ((uint)newPosition > (uint)limit) // J2N: Added check for overflowing integer
                throw new BufferOverflowException();

            Store(position, value);
            position = newPosition;
            return this;
        }

        public override ByteBuffer PutInt16(int index, short value)
        {
            int newIndex = index + sizeof(short);
            if (index < 0 || (uint)newIndex > (uint)limit) // J2N: Added check for overflowing integer
                throw new ArgumentOutOfRangeException(nameof(index));

            Store(index, value);
            return this;
        }

        public override ByteBuffer PutInt16(short value)
        {
            int newPosition = position + sizeof(short);
            if ((uint)newPosition > (uint)limit) // J2N: Added check for overflowing integer
                throw new BufferOverflowException();

            Store(position, value);
            position = newPosition;
            return this;
        }

        public override ByteBuffer Slice()
        {
            EnsureOpen();
            return new ReadWriteMemoryMappedViewByteBuffer(accessor, Remaining, offset + position)
            {
                order = this.order,
                isClone = true,
            };
        }
    }
}
