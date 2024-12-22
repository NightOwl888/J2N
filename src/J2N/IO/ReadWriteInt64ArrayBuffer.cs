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
    /// <see cref="Int64ArrayBuffer"/>, <see cref="ReadWriteInt64ArrayBuffer"/> and <see cref="ReadOnlyInt64ArrayBuffer"/> compose
    /// the implementation of array based long buffers.
    /// <para/>
    /// <see cref="ReadWriteInt64ArrayBuffer"/> extends <see cref="Int64ArrayBuffer"/> with all the write methods.
    /// <para/>
    /// This class is marked final for runtime performance.
    /// </summary>
    internal sealed class ReadWriteInt64ArrayBuffer : Int64ArrayBuffer
    {
        internal static ReadWriteInt64ArrayBuffer Copy(Int64ArrayBuffer other, int markOfOther)
        {
            return new ReadWriteInt64ArrayBuffer(other.Capacity, other.backingArray, other.offset)
            {
                limit = other.Limit,
                position = other.Position,
                mark = markOfOther
            };
        }

        internal ReadWriteInt64ArrayBuffer(long[] array)
            : base(array)
        { }

        internal ReadWriteInt64ArrayBuffer(int capacity)
            : base(capacity)
        { }

        internal ReadWriteInt64ArrayBuffer(int capacity, long[] backingArray, int arrayOffset)
            : base(capacity, backingArray, arrayOffset)
        { }

        public override Int64Buffer AsReadOnlyBuffer() => ReadOnlyInt64ArrayBuffer.Copy(this, mark);

        public override Int64Buffer Compact()
        {
            System.Array.Copy(backingArray, position + offset, backingArray, offset,
                    Remaining);
            position = limit - position;
            limit = capacity;
            mark = UnsetMark;
            return this;
        }

        public override Int64Buffer Duplicate() => Copy(this, mark);

        public override bool IsReadOnly => false;

        protected override long[] ProtectedArray => backingArray;

        protected override int ProtectedArrayOffset => offset;

        protected override bool ProtectedHasArray => true;


        public override Int64Buffer Put(long value)
        {
            if (position == limit)
            {
                throw new BufferOverflowException();
            }
            backingArray[offset + position++] = value;
            return this;
        }

        public override Int64Buffer Put(int index, long value)
        {
            if (index < 0 || index >= limit)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
            backingArray[offset + index] = value;
            return this;
        }

        public override Int64Buffer Put(long[] source, int offset, int length) // J2N TODO: API - Rename startIndex instead of offset
        {
            if (source is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
            if (offset < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(offset, ExceptionArgument.offset);
            if (length < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(length, ExceptionArgument.length);
            if (offset > source.Length - length) // Checks for int overflow
                throw new ArgumentOutOfRangeException(nameof(length), SR.ArgumentOutOfRange_IndexLength);
            if (length > Remaining)
                throw new BufferOverflowException();

            System.Array.Copy(source, offset, backingArray, base.offset + position, length);
            position += length;
            return this;
        }

        public override Int64Buffer Slice()
        {
            return new ReadWriteInt64ArrayBuffer(Remaining, backingArray, offset + position);
        }
    }
}
