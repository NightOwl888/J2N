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
    /// <see cref="Int16ArrayBuffer"/>, <see cref="ReadWriteInt16ArrayBuffer"/> and <see cref="ReadOnlyInt16ArrayBuffer"/>
    /// compose the implementation of array based short buffers.
    /// <para/>
    /// <see cref="ReadOnlyInt16ArrayBuffer"/> extends <see cref="Int16ArrayBuffer"/> with all the write
    /// methods throwing read only exception.
    /// <para/>
    /// This class is marked sealed for runtime performance.
    /// </summary>
    internal sealed class ReadOnlyInt16ArrayBuffer : Int16ArrayBuffer
    {
        internal static ReadOnlyInt16ArrayBuffer Copy(Int16ArrayBuffer other, int markOfOther)
        {
            return new ReadOnlyInt16ArrayBuffer(other.Capacity, other.backingArray, other.offset)
            {
                limit = other.Limit,
                position = other.Position,
                mark = markOfOther
            };
        }

        internal ReadOnlyInt16ArrayBuffer(int capacity, short[] backingArray, int arrayOffset)
            : base(capacity, backingArray, arrayOffset)
        { }

        public override Int16Buffer AsReadOnlyBuffer() => Duplicate();

        public override Int16Buffer Compact()
        {
            throw new ReadOnlyBufferException();
        }

        public override Int16Buffer Duplicate() => Copy(this, mark);

        public override bool IsReadOnly => true;

        protected override short[] ProtectedArray
        {
            get { throw new ReadOnlyBufferException(); }
        }

        protected override int ProtectedArrayOffset
        {
            get { throw new ReadOnlyBufferException(); }
        }

        protected override bool ProtectedHasArray => false;

        public override Int16Buffer Put(Int16Buffer buffer)
        {
            throw new ReadOnlyBufferException();
        }

        public override Int16Buffer Put(short value)
        {
            throw new ReadOnlyBufferException();
        }

        public override Int16Buffer Put(int index, short value)
        {
            throw new ReadOnlyBufferException();
        }

        public override Int16Buffer Put(short[] source, int offset, int length)
        {
            throw new ReadOnlyBufferException();
        }

        public override Int16Buffer Put(ReadOnlySpan<short> source) // J2N specific
        {
            throw new ReadOnlyBufferException();
        }

        public override Int16Buffer Slice()
        {
            return new ReadOnlyInt16ArrayBuffer(Remaining, backingArray, offset + position);
        }
    }
}
