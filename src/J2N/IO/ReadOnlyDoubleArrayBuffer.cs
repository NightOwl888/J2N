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

namespace J2N.IO
{
    /// <summary>
    /// <see cref="DoubleArrayBuffer"/>, <see cref="ReadWriteDoubleArrayBuffer"/>, and <see cref="ReadOnlyDoubleArrayBuffer"/>
    /// compose the implementation of array based float buffers.
    /// <para/>
    /// <see cref="ReadOnlyDoubleArrayBuffer"/> extends <see cref="DoubleArrayBuffer"/> with all the write
    /// methods throwing read only exception.
    /// <para/>
    /// This class is marked sealed for runtime performance.
    /// </summary>
    internal sealed class ReadOnlyDoubleArrayBuffer : DoubleArrayBuffer
    {
        internal static ReadOnlyDoubleArrayBuffer Copy(DoubleArrayBuffer other, int markOfOther)
        {
            return new ReadOnlyDoubleArrayBuffer(other.Capacity, other.backingArray, other.offset)
            {
                limit = other.Limit,
                position = other.Position,
                mark = markOfOther
            };
        }

        internal ReadOnlyDoubleArrayBuffer(int capacity, double[] backingArray, int arrayOffset)
            : base(capacity, backingArray, arrayOffset)
        {
        }

        public override DoubleBuffer AsReadOnlyBuffer() => Duplicate();

        public override DoubleBuffer Compact()
        {
            throw new ReadOnlyBufferException();
        }

        public override DoubleBuffer Duplicate() => Copy(this, mark);

        public override bool IsReadOnly => true;

        protected override double[] ProtectedArray
        {
            get { throw new ReadOnlyBufferException(); }
        }

        protected override int ProtectedArrayOffset
        {
            get { throw new ReadOnlyBufferException(); }
        }

        protected override bool ProtectedHasArray => false;

        public override DoubleBuffer Put(double value)
        {
            throw new ReadOnlyBufferException();
        }

        public override DoubleBuffer Put(int index, double value)
        {
            throw new ReadOnlyBufferException();
        }

        public override DoubleBuffer Put(DoubleBuffer buffer)
        {
            throw new ReadOnlyBufferException();
        }

        public override sealed DoubleBuffer Put(double[] source, int offset, int length)
        {
            throw new ReadOnlyBufferException();
        }

        public override DoubleBuffer Slice()
        {
            return new ReadOnlyDoubleArrayBuffer(Remaining, backingArray, offset + position);
        }
    }
}
