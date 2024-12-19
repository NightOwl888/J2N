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
    /// <see cref="Int16ArrayBuffer"/>, <see cref="ReadWriteInt16ArrayBuffer"/> and <see cref="ReadOnlyInt16ArrayBuffer"/>
    /// compose the implementation of array based short buffers.
    /// <para/>
    /// <see cref="ReadWriteInt16ArrayBuffer"/> extends <see cref="Int16ArrayBuffer"/> with all the write
    /// methods.
    /// <para/>
    /// This class is marked sealed for runtime performance.
    /// </summary>
    internal sealed class ReadWriteInt16ArrayBuffer : Int16ArrayBuffer
    {
        internal static ReadWriteInt16ArrayBuffer Copy(Int16ArrayBuffer other,
            int markOfOther)
        {
            return new ReadWriteInt16ArrayBuffer(other.Capacity, other.backingArray, other.offset)
            {
                limit = other.Limit,
                position = other.Position,
                mark = markOfOther
            };
        }

        internal ReadWriteInt16ArrayBuffer(short[] array)
            : base(array)
        { }

        internal ReadWriteInt16ArrayBuffer(int capacity)
            : base(capacity)
        { }

        internal ReadWriteInt16ArrayBuffer(int capacity, short[] backingArray, int arrayOffset)
            : base(capacity, backingArray, arrayOffset)
        { }

        public override Int16Buffer AsReadOnlyBuffer() => ReadOnlyInt16ArrayBuffer.Copy(this, mark);

        public override Int16Buffer Compact()
        {
            System.Array.Copy(backingArray, position + offset, backingArray, offset,
                    Remaining);
            position = limit - position;
            limit = capacity;
            mark = UnsetMark;
            return this;
        }

        public override Int16Buffer Duplicate() => Copy(this, mark);

        public override bool IsReadOnly => false;


        protected override short[] ProtectedArray => backingArray;

        protected override int ProtectedArrayOffset => offset;

        protected override bool ProtectedHasArray => true;

        public override Int16Buffer Put(short value)
        {
            if (position == limit)
            {
                throw new BufferOverflowException();
            }
            backingArray[offset + position++] = value;
            return this;
        }

        public override Int16Buffer Put(int index, short value)
        {
            if (index < 0 || index >= limit)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
            backingArray[offset + index] = value;
            return this;
        }

        public override Int16Buffer Put(short[] source, int offset, int length)
        {
            ThrowHelper.ThrowIfNull(source, ExceptionArgument.source);

            int len = source.Length;
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset), SR.ArgumentOutOfRange_NeedNonNegNum);
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length), SR.ArgumentOutOfRange_NeedNonNegNum);
            if (offset > len - length) // Checks for int overflow
                throw new ArgumentOutOfRangeException(nameof(length), SR.ArgumentOutOfRange_IndexLength);
            if (length > Remaining)
                throw new BufferOverflowException();

            System.Array.Copy(source, offset, backingArray, base.offset + position, length);
            position += length;
            return this;
        }

        public override Int16Buffer Slice()
        {
            return new ReadWriteInt16ArrayBuffer(Remaining, backingArray, offset + position);
        }
    }
}
