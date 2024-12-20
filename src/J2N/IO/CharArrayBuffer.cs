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
    /// <see cref="CharArrayBuffer"/>, <see cref="ReadWriteCharArrayBuffer"/> and <see cref="ReadOnlyCharArrayBuffer"/> compose
    /// the implementation of array based char buffers.
    /// <para/>
    /// <see cref="CharArrayBuffer"/> implements all the shared readonly methods and is extended by
    /// the other two classes.
    /// <para/>
    /// All methods are marked sealed for runtime performance.
    /// </summary>
    internal abstract class CharArrayBuffer : CharBuffer
    {
        protected internal readonly char[] backingArray;

        protected internal readonly int offset;

        internal CharArrayBuffer(char[] array)
            : this(array.Length, array, 0)
        {
        }

        internal CharArrayBuffer(int capacity)
            : this(capacity, new char[capacity], 0)
        {
        }

        internal CharArrayBuffer(int capacity, char[] backingArray, int offset)
            : base(capacity)
        {
            this.backingArray = backingArray;
            this.offset = offset;
        }

        public override sealed char Get()
        {
            if (position == limit)
            {
                throw new BufferUnderflowException();
            }
            return backingArray[offset + position++];
        }

        public override sealed char Get(int index)
        {
            if (index < 0 || index >= limit)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
            return backingArray[offset + index];
        }

        public override sealed CharBuffer Get(char[] destination, int offset, int length) // J2N TODO: API - Rename startIndex instead of offset
        {
            ThrowHelper.ThrowIfNull(destination, ExceptionArgument.destination);
            if (offset < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(offset, ExceptionArgument.offset);
            if (length < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(length, ExceptionArgument.length);
            if (offset > destination.Length - length) // Checks for int overflow
                throw new ArgumentOutOfRangeException(nameof(length), SR.ArgumentOutOfRange_IndexLength);
            if (length > Remaining)
                throw new BufferUnderflowException();

            System.Array.Copy(backingArray, this.offset + position, destination, offset, length);
            position += length;
            return this;
        }

        //public override sealed bool IsDirect => false;

        public override sealed ByteOrder Order => ByteOrder.NativeOrder;

        public override sealed CharBuffer Subsequence(int startIndex, int length)
        {
            if (startIndex < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(startIndex, ExceptionArgument.startIndex);
            if (length < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(length, ExceptionArgument.length);
            if (startIndex > Remaining - length) // Checks for int overflow
                throw new ArgumentOutOfRangeException(nameof(length), SR.ArgumentOutOfRange_IndexLength);

            CharBuffer result = Duplicate();
            result.Limit = position + startIndex + length;
            result.Position = position + startIndex;
            return result;
        }

        public override sealed string ToString()
        {
            return new string(backingArray, offset + position, Remaining);
        }
    }
}
