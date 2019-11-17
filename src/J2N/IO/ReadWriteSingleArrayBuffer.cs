using System;

namespace J2N.IO
{
    /// <summary>
    /// <see cref="SingleArrayBuffer"/>, <see cref="ReadWriteSingleArrayBuffer"/> and <see cref="ReadOnlySingleArrayBuffer"/>
    /// compose the implementation of array based float buffers.
    /// <para/>
    /// <see cref="ReadWriteSingleArrayBuffer"/> extends <see cref="SingleArrayBuffer"/> with all the write
    /// methods.
    /// <para/>
    /// This class is marked sealed for runtime performance.
    /// </summary>
    internal sealed class ReadWriteSingleArrayBuffer : SingleArrayBuffer
    {
        internal static ReadWriteSingleArrayBuffer Copy(SingleArrayBuffer other, int markOfOther)
        {
            return new ReadWriteSingleArrayBuffer(other.Capacity, other.backingArray, other.offset)
            {
                limit = other.Limit,
                position = other.Position,
                mark = markOfOther
            };
        }

        internal ReadWriteSingleArrayBuffer(float[] array)
            : base(array)
        { }

        internal ReadWriteSingleArrayBuffer(int capacity)
            : base(capacity)
        { }

        internal ReadWriteSingleArrayBuffer(int capacity, float[] backingArray, int arrayOffset)
            : base(capacity, backingArray, arrayOffset)
        { }

        public override SingleBuffer AsReadOnlyBuffer() => ReadOnlySingleArrayBuffer.Copy(this, mark);

        public override SingleBuffer Compact()
        {
            System.Array.Copy(backingArray, position + offset, backingArray, offset,
                    Remaining);
            position = limit - position;
            limit = capacity;
            mark = UnsetMark;
            return this;
        }

        public override SingleBuffer Duplicate() => Copy(this, mark);

        public override bool IsReadOnly => false;

        protected override float[] ProtectedArray => backingArray;

        protected override int ProtectedArrayOffset => offset;

        protected override bool ProtectedHasArray => true;

        public override SingleBuffer Put(float value)
        {
            if (position == limit)
            {
                throw new BufferOverflowException();
            }
            backingArray[offset + position++] = value;
            return this;
        }

        public override SingleBuffer Put(int index, float value)
        {
            if (index < 0 || index >= limit)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
            backingArray[offset + index] = value;
            return this;
        }

        public override SingleBuffer Put(float[] source, int offset, int length)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            int len = source.Length;
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset));
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length));
            if ((long)offset + (long)length > len)
                throw new ArgumentOutOfRangeException(string.Empty, $"{nameof(offset)} + {nameof(length)} > {nameof(source.Length)}");
            if (length > Remaining)
                throw new BufferOverflowException();

            System.Array.Copy(source, offset, backingArray, base.offset + position, length);
            position += length;
            return this;
        }

        public override SingleBuffer Slice()
        {
            return new ReadWriteSingleArrayBuffer(Remaining, backingArray, offset + position);
        }
    }
}
