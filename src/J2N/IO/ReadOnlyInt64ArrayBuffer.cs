

namespace J2N.IO
{
    /// <summary>
    /// <see cref="Int64ArrayBuffer"/>, <see cref="ReadWriteInt64ArrayBuffer"/> and <see cref="ReadOnlyInt64ArrayBuffer"/> compose
    /// the implementation of array based long buffers.
    /// <para/>
    /// <see cref="ReadOnlyInt64ArrayBuffer"/> extends <see cref="Int64ArrayBuffer"/> with all the write methods
    /// throwing read only exception.
    /// <para/>
    /// This class is marked sealed for runtime performance.
    /// </summary>
    internal sealed class ReadOnlyInt64ArrayBuffer : Int64ArrayBuffer
    {
        internal static ReadOnlyInt64ArrayBuffer Copy(Int64ArrayBuffer other, int markOfOther)
        {
            return new ReadOnlyInt64ArrayBuffer(other.Capacity, other.backingArray, other.offset)
            {
                limit = other.Limit,
                position = other.Position,
                mark = markOfOther
            };
        }

        internal ReadOnlyInt64ArrayBuffer(int capacity, long[] backingArray, int arrayOffset)
            : base(capacity, backingArray, arrayOffset)
        { }

        public override Int64Buffer AsReadOnlyBuffer() => Duplicate();

        public override Int64Buffer Compact()
        {
            throw new ReadOnlyBufferException();
        }

        public override Int64Buffer Duplicate() => Copy(this, mark);

        public override bool IsReadOnly => true;

        protected override long[] ProtectedArray
        {
            get { throw new ReadOnlyBufferException(); }
        }

        protected override int ProtectedArrayOffset
        {
            get { throw new ReadOnlyBufferException(); }
        }

        protected override bool ProtectedHasArray => false;

        public override Int64Buffer Put(long value)
        {
            throw new ReadOnlyBufferException();
        }

        public override Int64Buffer Put(int index, long value)
        {
            throw new ReadOnlyBufferException();
        }

        public override Int64Buffer Put(Int64Buffer buffer)
        {
            throw new ReadOnlyBufferException();
        }

        public override sealed Int64Buffer Put(long[] source, int offset, int length)
        {
            throw new ReadOnlyBufferException();
        }

        public override Int64Buffer Slice()
        {
            return new ReadOnlyInt64ArrayBuffer(Remaining, backingArray, offset + position);
        }
    }
}
