﻿#region Copyright 2010 by Apache Harmony, Licensed under the Apache License, Version 2.0
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
    /// <see cref="SingleArrayBuffer"/>, <see cref="ReadWriteSingleArrayBuffer"/> and <see cref="ReadOnlySingleArrayBuffer"/>
    /// compose the implementation of array based float buffers.
    /// <para/>
    /// <see cref="ReadWriteSingleArrayBuffer"/> extends <see cref="SingleArrayBuffer"/> with all the write
    /// methods.
    /// <para/>
    /// This class is marked sealed for runtime performance.
    /// </summary>
    internal sealed class ReadWriteSingleArrayBuffer : SingleArrayBuffer
    {
        internal static ReadWriteSingleArrayBuffer Copy(SingleArrayBuffer other, int markOfOther)
        {
            return new ReadWriteSingleArrayBuffer(other.Capacity, other.backingArray, other.offset)
            {
                limit = other.Limit,
                position = other.Position,
                mark = markOfOther
            };
        }

        internal ReadWriteSingleArrayBuffer(float[] array)
            : base(array)
        { }

        internal ReadWriteSingleArrayBuffer(int capacity)
            : base(capacity)
        { }

        internal ReadWriteSingleArrayBuffer(int capacity, float[] backingArray, int arrayOffset)
            : base(capacity, backingArray, arrayOffset)
        { }

        public override SingleBuffer AsReadOnlyBuffer() => ReadOnlySingleArrayBuffer.Copy(this, mark);

        public override SingleBuffer Compact()
        {
            System.Array.Copy(backingArray, position + offset, backingArray, offset,
                    Remaining);
            position = limit - position;
            limit = capacity;
            mark = UnsetMark;
            return this;
        }

        public override SingleBuffer Duplicate() => Copy(this, mark);

        public override bool IsReadOnly => false;

        protected override float[] ProtectedArray => backingArray;

        protected override int ProtectedArrayOffset => offset;

        protected override bool ProtectedHasArray => true;

        public override SingleBuffer Put(float value)
        {
            if (position == limit)
            {
                throw new BufferOverflowException();
            }
            backingArray[offset + position++] = value;
            return this;
        }

        public override SingleBuffer Put(int index, float value)
        {
            if ((uint)index >= (uint)limit)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
            backingArray[offset + index] = value;
            return this;
        }

        public override SingleBuffer Put(float[] source, int offset, int length) // J2N TODO: API - Rename startIndex instead of offset
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

        public override SingleBuffer Put(ReadOnlySpan<float> source) // J2N specific
        {
            int length = source.Length;
            if (length > Remaining)
                throw new BufferOverflowException();

            source.CopyTo(backingArray.AsSpan(offset + position, length));
            position += length;
            return this;
        }

        public override SingleBuffer Slice()
        {
            return new ReadWriteSingleArrayBuffer(Remaining, backingArray, offset + position);
        }
    }
}
