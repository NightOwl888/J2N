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

namespace J2N.IO
{
    /// <summary>
    /// <see cref="HeapByteBuffer"/>, <see cref="ReadWriteHeapByteBuffer"/> and <see cref="ReadOnlyHeapByteBuffer"/> compose
    /// the implementation of array based byte buffers.
    /// <para/>
    /// <see cref="ReadWriteHeapByteBuffer"/> extends <see cref="HeapByteBuffer"/> with all the write methods.
    /// <para/>
    /// This class is marked sealed for runtime performance.
    /// </summary>
    internal sealed class ReadWriteHeapByteBuffer : HeapByteBuffer
    {
        internal static ReadWriteHeapByteBuffer Copy(HeapByteBuffer other, int markOfOther)
        {
            return new ReadWriteHeapByteBuffer(other.backingArray, other.Capacity, other.offset)
            {
                limit = other.Limit,
                position = other.Position,
                mark = markOfOther,
                Order = other.Order
            };
        }

        internal ReadWriteHeapByteBuffer(byte[] backingArray)
            : base(backingArray)
        { }

        internal ReadWriteHeapByteBuffer(int capacity)
            : base(capacity)
        { }

        internal ReadWriteHeapByteBuffer(byte[] backingArray, int capacity, int arrayOffset)
            : base(backingArray, capacity, arrayOffset)
        { }

        public override ByteBuffer AsReadOnlyBuffer()
        {
            return ReadOnlyHeapByteBuffer.Copy(this, mark);
        }

        public override ByteBuffer Compact()
        {
            System.Array.Copy(backingArray, position + offset, backingArray, offset,
                    Remaining);
            position = limit - position;
            limit = capacity;
            mark = UnsetMark;
            return this;
        }

        public override ByteBuffer Duplicate() => Copy(this, mark);

        public override bool IsReadOnly => false;

        protected override byte[] ProtectedArray => backingArray;

        protected override int ProtectedArrayOffset => offset;

        protected override bool ProtectedHasArray => true;


        public override ByteBuffer Put(byte value)
        {
            if (position == limit)
            {
                throw new BufferOverflowException();
            }
            backingArray[offset + position++] = value;
            return this;
        }

        public override ByteBuffer Put(int index, byte value)
        {
            if ((uint)index >= (uint)limit)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
            backingArray[offset + index] = value;
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

            System.Array.Copy(source, offset, backingArray, base.offset + position, length);
            position += length;
            return this;
        }

        public override ByteBuffer Put(ReadOnlySpan<byte> source) // J2N specific
        {
            int length = source.Length;
            if (length > Remaining)
                throw new BufferOverflowException();

            source.CopyTo(backingArray.AsSpan(offset + position, length));
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
            return PutInt32(BitConversion.SingleToInt32Bits(value));
        }

        public override ByteBuffer PutSingle(int index, float value)
        {
            return PutInt32(index, BitConversion.SingleToInt32Bits(value));
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
            return new ReadWriteHeapByteBuffer(backingArray, Remaining, offset + position)
            {
                order = this.order
            };
        }
    }
}
