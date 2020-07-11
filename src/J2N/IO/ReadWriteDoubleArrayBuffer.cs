using System;

namespace J2N.IO
{
    using SR = J2N.Resources.Strings;

    /// <summary>
    /// <see cref="DoubleArrayBuffer"/>, <see cref="ReadWriteDoubleArrayBuffer"/>, and <see cref="ReadOnlyDoubleArrayBuffer"/>
    /// compose the implementation of array based float buffers.
    /// <para/>
    /// <see cref="ReadWriteDoubleArrayBuffer"/> extends <see cref="DoubleArrayBuffer"/> with all the write
    /// methods.
    /// <para/>
    /// This class is marked sealed for runtime performance.
    /// </summary>
    internal sealed class ReadWriteDoubleArrayBuffer : DoubleArrayBuffer
    {
        internal static ReadWriteDoubleArrayBuffer Copy(DoubleArrayBuffer other, int markOfOther)
        {
            return new ReadWriteDoubleArrayBuffer(other.Capacity, other.backingArray, other.offset)
            {
                limit = other.Limit,
                position = other.Position,
                mark = markOfOther
            };
        }

        internal ReadWriteDoubleArrayBuffer(double[] array)
            : base(array)
        { }

        internal ReadWriteDoubleArrayBuffer(int capacity)
            : base(capacity)
        { }

        internal ReadWriteDoubleArrayBuffer(int capacity, double[] backingArray, int arrayOffset)
            : base(capacity, backingArray, arrayOffset)
        { }

        public override DoubleBuffer AsReadOnlyBuffer() => ReadOnlyDoubleArrayBuffer.Copy(this, mark);

        public override DoubleBuffer Compact()
        {
            System.Array.Copy(backingArray, position + offset, backingArray, offset,
                    Remaining);
            position = limit - position;
            limit = capacity;
            mark = UnsetMark;
            return this;
        }

        public override DoubleBuffer Duplicate() => Copy(this, mark);

        public override bool IsReadOnly => false;

        protected override double[] ProtectedArray => backingArray;

        protected override int ProtectedArrayOffset => offset;

        protected override bool ProtectedHasArray => true;

        public override DoubleBuffer Put(double value)
        {
            if (position == limit)
            {
                throw new BufferOverflowException();
            }
            backingArray[offset + position++] = value;
            return this;
        }

        public override DoubleBuffer Put(int index, double value)
        {
            if (index < 0 || index >= limit)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
            backingArray[offset + index] = value;
            return this;
        }

        public override DoubleBuffer Put(double[] source, int offset, int length)
        {
            if (source == null)
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

            System.Array.Copy(source, offset, backingArray, base.offset + position, length);
            position += length;
            return this;
        }

        public override DoubleBuffer Slice()
        {
            return new ReadWriteDoubleArrayBuffer(Remaining, backingArray, offset + position);
        }
    }
}
