﻿using System;
using System.Diagnostics.CodeAnalysis;


namespace J2N.IO
{
    /// <summary>
    /// This class wraps a byte buffer to be a <see cref="double"/> buffer.
    /// <para/>
    /// Implementation notice:
    /// <list type="bullet">
    ///     <item><description>After a byte buffer instance is wrapped, it becomes privately owned by
    ///     the adapter. It must NOT be accessed outside the adapter any more.</description></item>
    ///     <item><description>The byte buffer's position and limit are NOT linked with the adapter.
    ///     The adapter extends <see cref="Buffer"/>, thus has its own position and limit.</description></item>
    /// </list>
    /// </summary>
    internal sealed class DoubleToByteBufferAdapter : DoubleBuffer
    {
        internal static DoubleBuffer Wrap(ByteBuffer byteBuffer)
        {
            return new DoubleToByteBufferAdapter(byteBuffer.Slice());
        }

        private readonly ByteBuffer byteBuffer;

        internal DoubleToByteBufferAdapter(ByteBuffer byteBuffer)
            : base((byteBuffer.Capacity >> 3))
        {
            this.byteBuffer = byteBuffer;
            this.byteBuffer.Clear();
        }

        //public int GetByteCapacity()
        //{
        //    if (byteBuffer is DirectBuffer) {
        //        return ((DirectBuffer)byteBuffer).GetByteCapacity();
        //    }
        //    return -1;
        //}

        //public PlatformAddress getEffectiveAddress()
        //{
        //    if (byteBuffer is DirectBuffer) {
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

        public override DoubleBuffer AsReadOnlyBuffer()
        {
            return new DoubleToByteBufferAdapter(byteBuffer.AsReadOnlyBuffer())
            {
                limit = limit,
                position = position,
                mark = mark
            };
        }

        public override DoubleBuffer Compact()
        {
            if (byteBuffer.IsReadOnly)
            {
                throw new ReadOnlyBufferException();
            }
            byteBuffer.SetLimit(limit << 3);
            byteBuffer.SetPosition(position << 3);
            byteBuffer.Compact();
            byteBuffer.Clear();
            position = limit - position;
            limit = capacity;
            mark = UnsetMark;
            return this;
        }
        public override DoubleBuffer Duplicate()
        {
            return new DoubleToByteBufferAdapter(byteBuffer.Duplicate())
            {
                limit = limit,
                position = position,
                mark = mark
            };
        }
        public override double Get()
        {
            if (position == limit)
            {
                throw new BufferUnderflowException();
            }
            return byteBuffer.GetDouble(position++ << 3);
        }
        public override double Get(int index)
        {
            if (index < 0 || index >= limit)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
            return byteBuffer.GetDouble(index << 3);
        }

        //public override bool IsDirect => byteBuffer.IsDirect;

        public override bool IsReadOnly => byteBuffer.IsReadOnly;

        public override ByteOrder Order => byteBuffer.Order;

        [SuppressMessage("Microsoft.Performance", "CA1819", Justification = "design requires some writable array properties")]
        protected override double[] ProtectedArray
        {
            get { throw new NotSupportedException(); }
        }

        protected override int ProtectedArrayOffset
        {
            get { throw new NotSupportedException(); }
        }

        protected override bool ProtectedHasArray => false;

        public override DoubleBuffer Put(double value)
        {
            if (position == limit)
            {
                throw new BufferOverflowException();
            }
            byteBuffer.PutDouble(position++ << 3, value);
            return this;
        }

        public override DoubleBuffer Put(int index, double value)
        {
            if (index < 0 || index >= limit)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
            byteBuffer.PutDouble(index << 3, value);
            return this;
        }

        public override DoubleBuffer Slice()
        {
            byteBuffer.SetLimit(limit << 3);
            byteBuffer.SetPosition(position << 3);
            DoubleBuffer result = new DoubleToByteBufferAdapter(byteBuffer.Slice());
            byteBuffer.Clear();
            return result;
        }
    }
}
