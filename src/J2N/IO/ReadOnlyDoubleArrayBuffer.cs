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
