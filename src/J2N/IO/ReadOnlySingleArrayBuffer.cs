namespace J2N.IO
{
    /// <summary>
    /// <see cref="SingleArrayBuffer"/>, <see cref="ReadWriteSingleArrayBuffer"/> and <see cref="ReadOnlySingleArrayBuffer"/>
    /// compose the implementation of array based float buffers.
    /// <para/>
    /// <see cref="ReadOnlySingleArrayBuffer"/> extends <see cref="SingleArrayBuffer"/> with all the write methods
    /// throwing read only exception.
    /// <para/>
    /// This class is marked sealed for runtime performance.
    /// </summary>
    internal sealed class ReadOnlySingleArrayBuffer : SingleArrayBuffer
    {
        internal static ReadOnlySingleArrayBuffer Copy(SingleArrayBuffer other, int markOfOther)
        {
            return new ReadOnlySingleArrayBuffer(other.Capacity, other.backingArray, other.offset)
            {
                limit = other.Limit,
                position = other.Position,
                mark = markOfOther
            };
        }

        internal ReadOnlySingleArrayBuffer(int capacity, float[] backingArray, int arrayOffset)
            : base(capacity, backingArray, arrayOffset)
        { }

        public override SingleBuffer AsReadOnlyBuffer() => Duplicate();

        public override SingleBuffer Compact()
        {
            throw new ReadOnlyBufferException();
        }

        public override SingleBuffer Duplicate() => Copy(this, mark);

        public override bool IsReadOnly => true;

        protected override float[] ProtectedArray
        {
            get { throw new ReadOnlyBufferException(); }
        }

        protected override int ProtectedArrayOffset
        {
            get { throw new ReadOnlyBufferException(); }
        }

        protected override bool ProtectedHasArray => false;

        public override SingleBuffer Put(float value)
        {
            throw new ReadOnlyBufferException();
        }

        public override SingleBuffer Put(int index, float value)
        {
            throw new ReadOnlyBufferException();
        }

        public override SingleBuffer Put(SingleBuffer buffer)
        {
            throw new ReadOnlyBufferException();
        }

        public override sealed SingleBuffer Put(float[] source, int offset, int length)
        {
            throw new ReadOnlyBufferException();
        }

        public override SingleBuffer Slice()
        {
            return new ReadOnlySingleArrayBuffer(Remaining, backingArray, offset + position);
        }
    }
}
