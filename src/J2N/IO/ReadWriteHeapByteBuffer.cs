using System;

namespace J2N.IO
{
    using SR = J2N.Resources.Strings;

    /// <summary>
    /// <see cref="HeapByteBuffer"/>, <see cref="ReadWriteHeapByteBuffer"/> and <see cref="ReadOnlyHeapByteBuffer"/> compose
    /// the implementation of array based byte buffers.
    /// <para/>
    /// <see cref="ReadWriteHeapByteBuffer"/> extends <see cref="HeapByteBuffer"/> with all the write methods.
    /// <para/>
    /// This class is marked sealed for runtime performance.
    /// </summary>
    internal sealed class ReadWriteHeapByteBuffer : HeapByteBuffer
    {
        internal static ReadWriteHeapByteBuffer Copy(HeapByteBuffer other, int markOfOther)
        {
            return new ReadWriteHeapByteBuffer(other.backingArray, other.Capacity, other.offset)
            {
                limit = other.Limit,
                position = other.Position,
                mark = markOfOther,
                Order = other.Order
            };
        }

        internal ReadWriteHeapByteBuffer(byte[] backingArray)
            : base(backingArray)
        { }

        internal ReadWriteHeapByteBuffer(int capacity)
            : base(capacity)
        { }

        internal ReadWriteHeapByteBuffer(byte[] backingArray, int capacity, int arrayOffset)
            : base(backingArray, capacity, arrayOffset)
        { }

        public override ByteBuffer AsReadOnlyBuffer()
        {
            return ReadOnlyHeapByteBuffer.Copy(this, mark);
        }

        public override ByteBuffer Compact()
        {
            System.Array.Copy(backingArray, position + offset, backingArray, offset,
                    Remaining);
            position = limit - position;
            limit = capacity;
            mark = UnsetMark;
            return this;
        }

        public override ByteBuffer Duplicate() => Copy(this, mark);

        public override bool IsReadOnly => false;

        protected override byte[] ProtectedArray => backingArray;

        protected override int ProtectedArrayOffset => offset;

        protected override bool ProtectedHasArray => true;


        public override ByteBuffer Put(byte value)
        {
            if (position == limit)
            {
                throw new BufferOverflowException();
            }
            backingArray[offset + position++] = value;
            return this;
        }

        public override ByteBuffer Put(int index, byte value)
        {
            if (index < 0 || index >= limit)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
            backingArray[offset + index] = value;
            return this;
        }

        /*
         * Override ByteBuffer.put(byte[], int, int) to improve performance.
         * 
         * (non-Javadoc)
         * 
         * @see java.nio.ByteBuffer#put(byte[], int, int)
         */

        public override ByteBuffer Put(byte[] source, int offset, int length)
        {
            if (source is null)
                throw new ArgumentNullException(nameof(source));

            int len = source.Length;
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset), SR.ArgumentOutOfRange_NeedNonNegNum);
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length), SR.ArgumentOutOfRange_NeedNonNegNum);
            if (offset > len - length) // Checks for int overflow
                throw new ArgumentOutOfRangeException(nameof(length), SR.ArgumentOutOfRange_IndexLength);
            if (length > Remaining)
                throw new BufferOverflowException();
            if (IsReadOnly)
                throw new ReadOnlyBufferException();

            System.Array.Copy(source, offset, backingArray, base.offset + position, length);
            position += length;
            return this;
        }

        public override ByteBuffer PutDouble(double value)
        {
            return PutInt64(BitConversion.DoubleToRawInt64Bits(value));
        }

        public override ByteBuffer PutDouble(int index, double value)
        {
            return PutInt64(index, BitConversion.DoubleToRawInt64Bits(value));
        }

        public override ByteBuffer PutSingle(float value)
        {
            return PutInt32(BitConversion.SingleToInt32Bits(value));
        }

        public override ByteBuffer PutSingle(int index, float value)
        {
            return PutInt32(index, BitConversion.SingleToInt32Bits(value));
        }

        public override ByteBuffer PutInt32(int value)
        {
            int newPosition = position + 4;
            if (newPosition < 0) // J2N: Added check for overflowing integer
                throw new ArgumentOutOfRangeException(nameof(Position));
            if (newPosition > limit)
                throw new BufferOverflowException();

            Store(position, value);
            position = newPosition;
            return this;
        }

        public override ByteBuffer PutInt32(int index, int value)
        {
            int newIndex = index + 4;
            if (index < 0 || newIndex > limit || newIndex < 0) // J2N: Added check for overflowing integer
                throw new ArgumentOutOfRangeException(nameof(index));

            Store(index, value);
            return this;
        }

        public override ByteBuffer PutInt64(int index, long value)
        {
            int newIndex = index + 8;
            if (index < 0 || newIndex > limit || newIndex < 0) // J2N: Added check for overflowing integer
                throw new ArgumentOutOfRangeException(nameof(index));

            Store(index, value);
            return this;
        }

        public override ByteBuffer PutInt64(long value)
        {
            int newPosition = position + 8;
            if (newPosition < 0) // J2N: Added check for overflowing integer
                throw new ArgumentOutOfRangeException(nameof(Position));
            if (newPosition > limit)
                throw new BufferOverflowException();

            Store(position, value);
            position = newPosition;
            return this;
        }

        public override ByteBuffer PutInt16(int index, short value)
        {
            int newIndex = index + 2;
            if (index < 0 || newIndex > limit || newIndex < 0) // J2N: Added check for overflowing integer
                throw new ArgumentOutOfRangeException(nameof(index));

            Store(index, value);
            return this;
        }

        public override ByteBuffer PutInt16(short value)
        {
            int newPosition = position + 2;
            if (newPosition < 0) // J2N: Added check for overflowing integer
                throw new ArgumentOutOfRangeException(nameof(Position));
            if (newPosition > limit)
                throw new BufferOverflowException();

            Store(position, value);
            position = newPosition;
            return this;
        }

        public override ByteBuffer Slice()
        {
            return new ReadWriteHeapByteBuffer(backingArray, Remaining, offset + position)
            {
                order = this.order
            };
        }
    }
}
