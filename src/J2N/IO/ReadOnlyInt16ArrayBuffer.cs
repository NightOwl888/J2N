#nullable enable

namespace J2N.IO
{
    /// <summary>
    /// <see cref="Int16ArrayBuffer"/>, <see cref="ReadWriteInt16ArrayBuffer"/> and <see cref="ReadOnlyInt16ArrayBuffer"/>
    /// compose the implementation of array based short buffers.
    /// <para/>
    /// <see cref="ReadOnlyInt16ArrayBuffer"/> extends <see cref="Int16ArrayBuffer"/> with all the write
    /// methods throwing read only exception.
    /// <para/>
    /// This class is marked sealed for runtime performance.
    /// </summary>
    internal sealed class ReadOnlyInt16ArrayBuffer : Int16ArrayBuffer
    {
        internal static ReadOnlyInt16ArrayBuffer Copy(Int16ArrayBuffer other, int markOfOther)
        {
            return new ReadOnlyInt16ArrayBuffer(other.Capacity, other.backingArray, other.offset)
            {
                limit = other.Limit,
                position = other.Position,
                mark = markOfOther
            };
        }

        internal ReadOnlyInt16ArrayBuffer(int capacity, short[] backingArray, int arrayOffset)
            : base(capacity, backingArray, arrayOffset)
        { }

        public override Int16Buffer AsReadOnlyBuffer() => Duplicate();

        public override Int16Buffer Compact()
        {
            throw new ReadOnlyBufferException();
        }

        public override Int16Buffer Duplicate() => Copy(this, mark);

        public override bool IsReadOnly => true;

        protected override short[] ProtectedArray
        {
            get { throw new ReadOnlyBufferException(); }
        }

        protected override int ProtectedArrayOffset
        {
            get { throw new ReadOnlyBufferException(); }
        }

        protected override bool ProtectedHasArray => false;

        public override Int16Buffer Put(Int16Buffer buffer)
        {
            throw new ReadOnlyBufferException();
        }

        public override Int16Buffer Put(short value)
        {
            throw new ReadOnlyBufferException();
        }

        public override Int16Buffer Put(int index, short value)
        {
            throw new ReadOnlyBufferException();
        }

        public override sealed Int16Buffer Put(short[] source, int offset, int length)
        {
            throw new ReadOnlyBufferException();
        }

        public override Int16Buffer Slice()
        {
            return new ReadOnlyInt16ArrayBuffer(Remaining, backingArray, offset + position);
        }
    }
}
