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
    /// This class wraps a byte buffer to be a <see cref="int"/> buffer.
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
    internal sealed class Int32ToByteBufferAdapter : Int32Buffer
    {
        internal static Int32Buffer Wrap(ByteBuffer byteBuffer)
        {
            return new Int32ToByteBufferAdapter(byteBuffer.Slice());
        }

        private readonly ByteBuffer byteBuffer;

        internal Int32ToByteBufferAdapter(ByteBuffer byteBuffer)
            : base((byteBuffer.Capacity >> 2))
        {
            this.byteBuffer = byteBuffer;
            this.byteBuffer.Clear();
        }

        //public int GetByteCapacity()
        //{
        //    if (byteBuffer is IDirectBuffer)
        //    {
        //        return ((IDirectBuffer)byteBuffer).GetByteCapacity();
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

        public override Int32Buffer AsReadOnlyBuffer()
        {
            return new Int32ToByteBufferAdapter(byteBuffer.AsReadOnlyBuffer())
            {
                limit = limit,
                position = position,
                mark = mark
            };
        }

        public override Int32Buffer Compact()
        {
            if (byteBuffer.IsReadOnly)
            {
                throw new ReadOnlyBufferException();
            }
            byteBuffer.Limit = limit << 2;
            byteBuffer.Position = position << 2;
            byteBuffer.Compact();
            byteBuffer.Clear();
            position = limit - position;
            limit = capacity;
            mark = UnsetMark;
            return this;
        }

        public override Int32Buffer Duplicate()
        {
            return new Int32ToByteBufferAdapter(byteBuffer.Duplicate())
            {
                limit = limit,
                position = position,
                mark = mark
            };
        }

        public override int Get()
        {
            if (position == limit)
            {
                throw new BufferUnderflowException();
            }
            return byteBuffer.GetInt32(position++ << 2);
        }

        public override int Get(int index)
        {
            if (index < 0 || index >= limit)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
            return byteBuffer.GetInt32(index << 2);
        }

        //public override bool IsDirect => byteBuffer.IsDirect;

        public override bool IsReadOnly => byteBuffer.IsReadOnly;

        public override ByteOrder Order => byteBuffer.Order;

        [SuppressMessage("Microsoft.Performance", "CA1819", Justification = "design requires some writable array properties")]
        protected override int[] ProtectedArray
        {
            get { throw new NotSupportedException(); }
        }

        protected override int ProtectedArrayOffset
        {
            get { throw new NotSupportedException(); }
        }

        protected override bool ProtectedHasArray => false;

        public override Int32Buffer Put(int value)
        {
            if (position == limit)
            {
                throw new BufferOverflowException();
            }
            byteBuffer.PutInt32(position++ << 2, value);
            return this;
        }

        public override Int32Buffer Put(int index, int value)
        {
            if (index < 0 || index >= limit)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
            byteBuffer.PutInt32(index << 2, value);
            return this;
        }

        public override Int32Buffer Slice()
        {
            byteBuffer.Limit = limit << 2;
            byteBuffer.Position = position << 2;
            Int32Buffer result = new Int32ToByteBufferAdapter(byteBuffer.Slice());
            byteBuffer.Clear();
            return result;
        }
    }
}
