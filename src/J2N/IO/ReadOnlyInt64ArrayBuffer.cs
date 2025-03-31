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
    /// <see cref="ReadOnlyInt64ArrayBuffer"/> extends <see cref="Int64ArrayBuffer"/> with all the write methods
    /// throwing read only exception.
    /// <para/>
    /// This class is marked sealed for runtime performance.
    /// </summary>
    internal sealed class ReadOnlyInt64ArrayBuffer : Int64ArrayBuffer
    {
        internal static ReadOnlyInt64ArrayBuffer Copy(Int64ArrayBuffer other, int markOfOther)
        {
            return new ReadOnlyInt64ArrayBuffer(other.Capacity, other.backingArray, other.offset)
            {
                limit = other.Limit,
                position = other.Position,
                mark = markOfOther
            };
        }

        internal ReadOnlyInt64ArrayBuffer(int capacity, long[] backingArray, int arrayOffset)
            : base(capacity, backingArray, arrayOffset)
        { }

        public override Int64Buffer AsReadOnlyBuffer() => Duplicate();

        public override Int64Buffer Compact()
        {
            throw new ReadOnlyBufferException();
        }

        public override Int64Buffer Duplicate() => Copy(this, mark);

        public override bool IsReadOnly => true;

        protected override long[] ProtectedArray
        {
            get { throw new ReadOnlyBufferException(); }
        }

        protected override int ProtectedArrayOffset
        {
            get { throw new ReadOnlyBufferException(); }
        }

        protected override bool ProtectedHasArray => false;

        public override Int64Buffer Put(long value)
        {
            throw new ReadOnlyBufferException();
        }

        public override Int64Buffer Put(int index, long value)
        {
            throw new ReadOnlyBufferException();
        }

        public override Int64Buffer Put(Int64Buffer buffer)
        {
            throw new ReadOnlyBufferException();
        }

        public override Int64Buffer Put(long[] source, int offset, int length)
        {
            throw new ReadOnlyBufferException();
        }

        public override Int64Buffer Put(ReadOnlySpan<long> source) // J2N specific
        {
            throw new ReadOnlyBufferException();
        }

        public override Int64Buffer Slice()
        {
            return new ReadOnlyInt64ArrayBuffer(Remaining, backingArray, offset + position);
        }
    }
}
