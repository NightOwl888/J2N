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
using System.IO.MemoryMappedFiles;

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
                Order = other.Order
            };
        }

        /// <summary>
        /// Initializes a new instance of <see cref="ReadWriteMemoryMappedViewByteBuffer"/>
        /// with the specified <paramref name="accessor"/> and <paramref name="capacity"/>.
        /// </summary>
        /// <param name="accessor">A <see cref="MemoryMappedViewAccessor"/>.</param>
        /// <param name="capacity">The capacity of the buffer.</param>
        internal ReadWriteMemoryMappedViewByteBuffer(MemoryMappedViewAccessor accessor, int capacity)
            : base(accessor, capacity)
        { }

        /// <summary>
        /// Initializes a new instance of <see cref="ReadWriteMemoryMappedViewByteBuffer"/>
        /// with the specified <paramref name="accessor"/> and <paramref name="capacity"/>.
        /// </summary>
        /// <param name="accessor">A <see cref="MemoryMappedViewAccessor"/>.</param>
        /// <param name="capacity">The capacity of the buffer.</param>
        /// <param name="offset">The offset of the buffer.</param>
        internal ReadWriteMemoryMappedViewByteBuffer(MemoryMappedViewAccessor accessor, int capacity, int offset)
            : base(accessor, capacity, offset)
        { }

        public override ByteBuffer AsReadOnlyBuffer()
        {
            return ReadOnlyMemoryMappedViewByteBuffer.Copy(this, mark);
        }

        public override ByteBuffer Compact() => throw new NotSupportedException();

        public override ByteBuffer Duplicate() => Copy(this, mark);

        public override bool IsReadOnly => false;

        protected override byte[] ProtectedArray => throw new NotSupportedException();

        protected override int ProtectedArrayOffset => offset;

        protected override bool ProtectedHasArray => false;


        public override ByteBuffer Put(byte value)
        {
            accessor.Write(Ix(NextPutIndex()), value);
            return this;
        }

        public override ByteBuffer Put(int index, byte value)
        {
            accessor.Write(Ix(CheckIndex(index)), value);
            return this;
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
            ThrowHelper.ThrowIfNull(source, ExceptionArgument.source);
            if (offset < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(offset, ExceptionArgument.offset);
            if (length < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(length, ExceptionArgument.length);
            if (offset > source.Length - length) // Checks for int overflow
                throw new ArgumentOutOfRangeException(nameof(length), SR.ArgumentOutOfRange_IndexLength);
            if (length > Remaining)
                throw new BufferOverflowException();
            if (IsReadOnly)
                throw new ReadOnlyBufferException();

            // we need to check for 0-length writes, since 
            // WriteArray will throw an ArgumentOutOfRange exception if position is at
            // the end even when nothing is written
            if (length > 0)
            {
                accessor.WriteArray(Ix(NextPutIndex(length)), source, offset, length);
            }
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
            return PutInt32(BitConversion.SingleToInt32Bits(value));
        }

        public override ByteBuffer PutSingle(int index, float value)
        {
            return PutInt32(index, BitConversion.SingleToInt32Bits(value));
        }

        public override ByteBuffer PutInt32(int value)
        {
            Store(NextPutIndex(4), value);
            return this;
        }

        public override ByteBuffer PutInt32(int index, int value)
        {
            Store(CheckIndex(index, 4), value);
            return this;
        }

        public override ByteBuffer PutInt64(int index, long value)
        {
            Store(CheckIndex(index, 8), value);
            return this;
        }

        public override ByteBuffer PutInt64(long value)
        {
            Store(NextPutIndex(8), value);
            return this;
        }

        public override ByteBuffer PutInt16(int index, short value)
        {
            Store(CheckIndex(index, 2), value);
            return this;
        }

        public override ByteBuffer PutInt16(short value)
        {
            Store(NextPutIndex(2), value);
            return this;
        }

        public override ByteBuffer Slice()
        {
            return new ReadWriteMemoryMappedViewByteBuffer(accessor, Remaining, offset + position)
            {
                order = this.order
            };
        }
    }
}
