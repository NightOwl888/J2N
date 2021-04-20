using System;
#nullable enable

namespace J2N.IO
{
    using SR = J2N.Resources.Strings;

    /// <summary>
    /// <see cref="Int64ArrayBuffer"/>, <see cref="ReadWriteInt64ArrayBuffer"/> and <see cref="ReadOnlyInt64ArrayBuffer"/> compose
    /// the implementation of array based long buffers.
    /// <para/>
    /// <see cref="ReadWriteInt64ArrayBuffer"/> extends <see cref="Int64ArrayBuffer"/> with all the write methods.
    /// <para/>
    /// This class is marked final for runtime performance.
    /// </summary>
    internal sealed class ReadWriteInt64ArrayBuffer : Int64ArrayBuffer
    {
        internal static ReadWriteInt64ArrayBuffer Copy(Int64ArrayBuffer other, int markOfOther)
        {
            return new ReadWriteInt64ArrayBuffer(other.Capacity, other.backingArray, other.offset)
            {
                limit = other.Limit,
                position = other.Position,
                mark = markOfOther
            };
        }

        internal ReadWriteInt64ArrayBuffer(long[] array)
            : base(array)
        { }

        internal ReadWriteInt64ArrayBuffer(int capacity)
            : base(capacity)
        { }

        internal ReadWriteInt64ArrayBuffer(int capacity, long[] backingArray, int arrayOffset)
            : base(capacity, backingArray, arrayOffset)
        { }

        public override Int64Buffer AsReadOnlyBuffer() => ReadOnlyInt64ArrayBuffer.Copy(this, mark);

        public override Int64Buffer Compact()
        {
            System.Array.Copy(backingArray, position + offset, backingArray, offset,
                    Remaining);
            position = limit - position;
            limit = capacity;
            mark = UnsetMark;
            return this;
        }

        public override Int64Buffer Duplicate() => Copy(this, mark);

        public override bool IsReadOnly => false;

        protected override long[] ProtectedArray => backingArray;

        protected override int ProtectedArrayOffset => offset;

        protected override bool ProtectedHasArray => true;


        public override Int64Buffer Put(long value)
        {
            if (position == limit)
            {
                throw new BufferOverflowException();
            }
            backingArray[offset + position++] = value;
            return this;
        }

        public override Int64Buffer Put(int index, long value)
        {
            if (index < 0 || index >= limit)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
            backingArray[offset + index] = value;
            return this;
        }

        public override Int64Buffer Put(long[] source, int offset, int length)
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

            System.Array.Copy(source, offset, backingArray, base.offset + position, length);
            position += length;
            return this;
        }

        public override Int64Buffer Slice()
        {
            return new ReadWriteInt64ArrayBuffer(Remaining, backingArray, offset + position);
        }
    }
}
