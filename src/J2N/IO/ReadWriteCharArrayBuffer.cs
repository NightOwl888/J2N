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
using System.Diagnostics.CodeAnalysis;

namespace J2N.IO
{
    /// <summary>
    /// <see cref="CharArrayBuffer"/>, <see cref="ReadWriteCharArrayBuffer"/> and <see cref="ReadOnlyCharArrayBuffer"/> compose
    /// the implementation of array based char buffers.
    /// <para/>
    /// <see cref="ReadWriteCharArrayBuffer"/> extends <see cref="CharArrayBuffer"/> with all the write methods.
    /// <para/>
    /// This class is marked sealed for runtime performance.
    /// </summary>
    internal sealed class ReadWriteCharArrayBuffer : CharArrayBuffer
    {
        internal static ReadWriteCharArrayBuffer Copy(CharArrayBuffer other, int markOfOther)
        {
            return new ReadWriteCharArrayBuffer(other.Capacity, other.backingArray, other.offset)
            {
                limit = other.Limit,
                position = other.Position,
                mark = markOfOther
            };
        }

        internal ReadWriteCharArrayBuffer(char[] array)
            : base(array)
        { }

        internal ReadWriteCharArrayBuffer(int capacity)
            : base(capacity)
        { }

        internal ReadWriteCharArrayBuffer(int capacity, char[] backingArray, int arrayOffset)
            : base(capacity, backingArray, arrayOffset)
        { }

        public override CharBuffer AsReadOnlyBuffer() => ReadOnlyCharArrayBuffer.Copy(this, mark);

        public override CharBuffer Compact()
        {
            System.Array.Copy(backingArray, position + offset, backingArray, offset,
                    Remaining);
            position = limit - position;
            limit = capacity;
            mark = UnsetMark;
            return this;
        }

        public override CharBuffer Duplicate() => Copy(this, mark);

        public override bool IsReadOnly => false;

        protected override char[] ProtectedArray => backingArray;

        protected override int ProtectedArrayOffset => offset;

        protected override bool ProtectedHasArray => true;

        public override CharBuffer Put(char value)
        {
            if (position == limit)
            {
                throw new BufferOverflowException();
            }
            backingArray[offset + position++] = value;
            return this;
        }

        public override CharBuffer Put(int index, char value)
        {
            if ((uint)index >= (uint)limit)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
            backingArray[offset + index] = value;
            return this;
        }

        public override CharBuffer Put(char[] source, int offset, int length) // J2N TODO: API - Rename startIndex instead of offset
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

        public override CharBuffer Slice()
        {
            return new ReadWriteCharArrayBuffer(Remaining, backingArray, offset + position);
        }
    }
}
