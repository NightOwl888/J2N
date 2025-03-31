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
    /// <see cref="ReadOnlyHeapByteBuffer"/> extends <see cref="HeapByteBuffer"/> with all the write methods
    /// throwing read only exception.
    /// <para/>
    /// This class is marked sealed for runtime performance.
    /// </summary>
    internal sealed class ReadOnlyHeapByteBuffer : HeapByteBuffer
    {
        internal static ReadOnlyHeapByteBuffer Copy(HeapByteBuffer other, int markOfOther)
        {
            return new ReadOnlyHeapByteBuffer(other.backingArray, other.Capacity, other.offset)
            {
                limit = other.Limit,
                position = other.Position,
                mark = markOfOther,
                Order = other.Order
            };
        }
        internal ReadOnlyHeapByteBuffer(byte[] backingArray, int capacity, int arrayOffset)
            : base(backingArray, capacity, arrayOffset)
        { }

        public override ByteBuffer AsReadOnlyBuffer() => Copy(this, mark);

        public override ByteBuffer Compact()
        {
            throw new ReadOnlyBufferException();
        }

        public override ByteBuffer Duplicate() => Copy(this, mark);

        public override bool IsReadOnly => true;

        protected override byte[] ProtectedArray
        {
            get { throw new ReadOnlyBufferException(); }
        }
        protected override int ProtectedArrayOffset
        {
            get { throw new ReadOnlyBufferException(); }
        }

        protected override bool ProtectedHasArray => false;

        public override ByteBuffer Put(byte value)
        {
            throw new ReadOnlyBufferException();
        }

        public override ByteBuffer Put(int index, byte value)
        {
            throw new ReadOnlyBufferException();
        }

        public override ByteBuffer Put(byte[] source, int offset, int length)
        {
            throw new ReadOnlyBufferException();
        }

        public override ByteBuffer Put(ReadOnlySpan<byte> source) // J2N specific
        {
            throw new ReadOnlyBufferException();
        }

        public override ByteBuffer PutDouble(double value)
        {
            throw new ReadOnlyBufferException();
        }

        public override ByteBuffer PutDouble(int index, double value)
        {
            throw new ReadOnlyBufferException();
        }

        public override ByteBuffer PutSingle(float value)
        {
            throw new ReadOnlyBufferException();
        }

        public override ByteBuffer PutSingle(int index, float value)
        {
            throw new ReadOnlyBufferException();
        }

        public override ByteBuffer PutInt32(int value)
        {
            throw new ReadOnlyBufferException();
        }

        public override ByteBuffer PutInt32(int index, int value)
        {
            throw new ReadOnlyBufferException();
        }

        public override ByteBuffer PutInt64(int index, long value)
        {
            throw new ReadOnlyBufferException();
        }

        public override ByteBuffer PutInt64(long value)
        {
            throw new ReadOnlyBufferException();
        }

        public override ByteBuffer PutInt16(int index, short value)
        {
            throw new ReadOnlyBufferException();
        }

        public override ByteBuffer PutInt16(short value)
        {
            throw new ReadOnlyBufferException();
        }

        public override ByteBuffer Put(ByteBuffer buffer)
        {
            throw new ReadOnlyBufferException();
        }

        public override ByteBuffer Slice()
        {
            return new ReadOnlyHeapByteBuffer(backingArray, Remaining, offset + position)
            {
                order = this.order
            };
        }
    }
}
