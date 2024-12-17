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
    /// This class wraps a byte buffer to be a <see cref="char"/> buffer.
    /// </summary>
    /// <remarks>
    /// Implementation notice:
    /// <list type="bullet">
    ///     <item><description>
    ///         After a byte buffer instance is wrapped, it becomes privately owned by
    ///         the adapter. It must NOT be accessed outside the adapter any more.
    ///     </description></item>
    ///     <item><description>
    ///         The byte buffer's position and limit are NOT linked with the adapter.
    ///         The adapter extends <see cref="Buffer"/>, thus has its own position and limit.
    ///     </description></item>
    /// </list>
    /// </remarks>
    internal sealed class CharToByteBufferAdapter : CharBuffer
    {
        internal static CharBuffer Wrap(ByteBuffer byteBuffer)
        {
            return new CharToByteBufferAdapter(byteBuffer.Slice());
        }

        private readonly ByteBuffer byteBuffer;

        internal CharToByteBufferAdapter(ByteBuffer byteBuffer)
                : base((byteBuffer.Capacity >> 1))
        {
            this.byteBuffer = byteBuffer;
            this.byteBuffer.Clear();
        }

        //public int GetByteCapacity()
        //{
        //    if (byteBuffer is IDirectBuffer)
        //    {
        //        return ((DirectBuffer)byteBuffer).getByteCapacity();
        //    }
        //    Debug.Assert(false, byteBuffer);
        //    return -1;
        //}

        //public PlatformAddress getEffectiveAddress()
        //{
        //    if (byteBuffer instanceof DirectBuffer) {
        //        return ((DirectBuffer)byteBuffer).getEffectiveAddress();
        //    }
        //    assert false : byteBuffer;
        //    return null;
        //}

        //public PlatformAddress getBaseAddress()
        //{
        //    if (byteBuffer instanceof DirectBuffer) {
        //        return ((DirectBuffer)byteBuffer).getBaseAddress();
        //    }
        //    assert false : byteBuffer;
        //    return null;
        //}

        //public boolean isAddressValid()
        //{
        //    if (byteBuffer instanceof DirectBuffer) {
        //        return ((DirectBuffer)byteBuffer).isAddressValid();
        //    }
        //    assert false : byteBuffer;
        //    return false;
        //}

        //public void addressValidityCheck()
        //{
        //    if (byteBuffer instanceof DirectBuffer) {
        //        ((DirectBuffer)byteBuffer).addressValidityCheck();
        //    } else {
        //        assert false : byteBuffer;
        //    }
        //}

        //public void free()
        //{
        //    if (byteBuffer instanceof DirectBuffer) {
        //        ((DirectBuffer)byteBuffer).free();
        //    } else {
        //        assert false : byteBuffer;
        //    }
        //}

        public override CharBuffer AsReadOnlyBuffer()
        {
            return new CharToByteBufferAdapter(byteBuffer.AsReadOnlyBuffer())
            {
                limit = limit,
                position = position,
                mark = mark
            };
        }

        public override CharBuffer Compact()
        {
            if (byteBuffer.IsReadOnly)
            {
                throw new ReadOnlyBufferException();
            }
            byteBuffer.Limit = limit << 1;
            byteBuffer.Position = position << 1;
            byteBuffer.Compact();
            byteBuffer.Clear();
            position = limit - position;
            limit = capacity;
            mark = UnsetMark;
            return this;
        }

        public override CharBuffer Duplicate()
        {
            return new CharToByteBufferAdapter(byteBuffer.Duplicate())
            {
                limit = limit,
                position = position,
                mark = mark
            };
        }

        public override char Get()
        {
            if (position == limit)
            {
                throw new BufferUnderflowException();
            }
            return byteBuffer.GetChar(position++ << 1);
        }

        public override char Get(int index)
        {
            if (index < 0 || index >= limit)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
            return byteBuffer.GetChar(index << 1);
        }

        //public override bool IsDirect => byteBuffer.IsDirect;

        public override bool IsReadOnly => byteBuffer.IsReadOnly;

        public override ByteOrder Order => byteBuffer.Order;

        protected override char[] ProtectedArray
        {
            get { throw new NotSupportedException(); }
        }

        protected override int ProtectedArrayOffset
        {
            get { throw new NotSupportedException(); }
        }

        protected override bool ProtectedHasArray => false;

        public override CharBuffer Put(char value)
        {
            if (position == limit)
            {
                throw new BufferOverflowException();
            }
            byteBuffer.PutChar(position++ << 1, value);
            return this;
        }

        public override CharBuffer Put(int index, char value)
        {
            if (index < 0 || index >= limit)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
            byteBuffer.PutChar(index << 1, value);
            return this;
        }

        public override CharBuffer Slice()
        {
            byteBuffer.Limit = limit << 1;
            byteBuffer.Position = position << 1;
            CharBuffer result = new CharToByteBufferAdapter(byteBuffer.Slice());
            byteBuffer.Clear();
            return result;
        }

        public override CharBuffer Subsequence(int startIndex, int length)
        {
            if (startIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(startIndex), SR.ArgumentOutOfRange_NeedNonNegNum);
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length), SR.ArgumentOutOfRange_NeedNonNegNum);
            if (startIndex > Remaining - length) // Checks for int overflow
                throw new ArgumentOutOfRangeException(nameof(length), SR.ArgumentOutOfRange_IndexLength);

            CharBuffer result = Duplicate();
            result.Limit = position + startIndex + length;
            result.Position = position + startIndex;
            return result;
        }
    }
}
