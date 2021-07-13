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


using System.IO.MemoryMappedFiles;


namespace J2N.IO.MemoryMappedFiles
{
    /// <summary>
    /// <see cref="MemoryMappedViewByteBuffer"/>, <see cref="ReadWriteMemoryMappedViewByteBuffer"/> and <see cref="ReadOnlyMemoryMappedViewByteBuffer"/> compose
    /// the implementation of memory-mapped byte buffers.
    /// <para/>
    /// <see cref="ReadOnlyMemoryMappedViewByteBuffer"/> extends <see cref="MemoryMappedViewByteBuffer"/> with all the write methods
    /// throwing read only exception.
    /// <para/>
    /// This class is marked sealed for runtime performance.
    /// </summary>
    internal sealed class ReadOnlyMemoryMappedViewByteBuffer : MemoryMappedViewByteBuffer
    {
        internal static ReadOnlyMemoryMappedViewByteBuffer Copy(MemoryMappedViewByteBuffer other, int markOfOther)
        {
            return new ReadOnlyMemoryMappedViewByteBuffer(other.accessor, other.Capacity, other.offset)
            {
                limit = other.Limit,
                position = other.Position,
                mark = markOfOther,
                Order = other.Order
            };
        }

        /// <summary>
        /// Initializes a new instance of <see cref="ReadOnlyMemoryMappedViewByteBuffer"/>
        /// with the specified <paramref name="accessor"/> and <paramref name="capacity"/>.
        /// </summary>
        /// <param name="accessor">A <see cref="MemoryMappedViewAccessor"/>.</param>
        /// <param name="capacity">The capacity of the buffer.</param>
        /// <param name="offset">The offset of the buffer.</param>
        internal ReadOnlyMemoryMappedViewByteBuffer(MemoryMappedViewAccessor accessor, int capacity, int offset)
            : base(accessor, capacity, offset)
        { }

        public override ByteBuffer AsReadOnlyBuffer() => Copy(this, mark);

        public override ByteBuffer Compact() => throw new ReadOnlyBufferException();

        public override ByteBuffer Duplicate() => Copy(this, mark);

        public override bool IsReadOnly => true;

        protected override byte[] ProtectedArray => throw new ReadOnlyBufferException();

        protected override int ProtectedArrayOffset => throw new ReadOnlyBufferException();

        protected override bool ProtectedHasArray => false;

        public override ByteBuffer Put(byte value) => throw new ReadOnlyBufferException();

        public override ByteBuffer Put(int index, byte value) => throw new ReadOnlyBufferException();

        public override ByteBuffer Put(byte[] source, int offset, int length) => throw new ReadOnlyBufferException();

        public override ByteBuffer PutDouble(double value) => throw new ReadOnlyBufferException();

        public override ByteBuffer PutDouble(int index, double value) => throw new ReadOnlyBufferException();

        public override ByteBuffer PutSingle(float value) => throw new ReadOnlyBufferException();

        public override ByteBuffer PutSingle(int index, float value) => throw new ReadOnlyBufferException();

        public override ByteBuffer PutInt32(int value) => throw new ReadOnlyBufferException();

        public override ByteBuffer PutInt32(int index, int value) => throw new ReadOnlyBufferException();

        public override ByteBuffer PutInt64(int index, long value) => throw new ReadOnlyBufferException();

        public override ByteBuffer PutInt64(long value) => throw new ReadOnlyBufferException();

        public override ByteBuffer PutInt16(int index, short value) => throw new ReadOnlyBufferException();

        public override ByteBuffer PutInt16(short value) => throw new ReadOnlyBufferException();

        public override ByteBuffer Put(ByteBuffer buffer) => throw new ReadOnlyBufferException();

        public override ByteBuffer Slice()
        {
            return new ReadOnlyMemoryMappedViewByteBuffer(accessor, Remaining, offset + position)
            {
                order = this.order
            };
        }
    }
}
