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
    /// <see cref="SingleArrayBuffer"/>, <see cref="ReadWriteSingleArrayBuffer"/>, and <see cref="ReadOnlySingleArrayBuffer"/>
    /// compose the implementation of array based float buffers.
    /// <para/>
    /// <see cref="SingleArrayBuffer"/> implements all the shared readonly methods and is extended
    /// by the other two classes.
    /// <para/>
    /// All methods are marked sealed for runtime performance.
    /// </summary>
    internal abstract class SingleArrayBuffer : SingleBuffer
    {
        protected internal readonly float[] backingArray;

        protected internal readonly int offset;

        internal SingleArrayBuffer(float[] array)
            : this(array.Length, array, 0)
        { }

        internal SingleArrayBuffer(int capacity)
            : this(capacity, new float[capacity], 0)
        { }

        internal SingleArrayBuffer(int capacity, float[] backingArray, int offset)
            : base(capacity)
        {
            this.backingArray = backingArray;
            this.offset = offset;
        }

        public override sealed float Get()
        {
            if (position == limit)
            {
                throw new BufferUnderflowException();
            }
            return backingArray[offset + position++];
        }

        public override sealed float Get(int index)
        {
            if ((uint)index >= (uint)limit)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
            return backingArray[offset + index];
        }

        /// <inheritdoc/>
        public override SingleBuffer Get(float[] destination)
        {
            if (destination is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.destination);
            return Get(destination.AsSpan());
        }

        public override sealed SingleBuffer Get(float[] destination, int offset, int length) // J2N TODO: API - Rename startIndex instead of offset
        {
            if (destination is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.destination);
            if (offset < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(offset, ExceptionArgument.offset);
            if (length < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(length, ExceptionArgument.length);
            if (offset > destination.Length - length) // Checks for int overflow
                ThrowHelper.ThrowArgumentOutOfRange_IndexLengthArray(offset, ExceptionArgument.offset, length);
            if (length > Remaining)
                throw new BufferUnderflowException();

            System.Array.Copy(backingArray, this.offset + position, destination, offset, length);
            position += length;
            return this;
        }

        public override SingleBuffer Get(Span<float> destination) // J2N specific
        {
            int length = destination.Length;
            if (length > Remaining)
                throw new BufferUnderflowException();

            backingArray.AsSpan(offset + position, length).CopyTo(destination);
            position += length;
            return this;
        }

        //public override sealed bool IsDirect => false;

        public override sealed ByteOrder Order => ByteOrder.NativeOrder;
    }
}
