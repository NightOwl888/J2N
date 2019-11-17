using System;
using System.Diagnostics.CodeAnalysis;

namespace J2N.IO
{
    /// <summary>
    /// <see cref="Int16ArrayBuffer"/>, <see cref="ReadWriteInt16ArrayBuffer"/> and <see cref="ReadOnlyInt16ArrayBuffer"/>
    /// compose the implementation of array based short buffers.
    /// <para/>
    /// <see cref="ReadWriteInt16ArrayBuffer"/> extends <see cref="Int16ArrayBuffer"/> with all the write
    /// methods.
    /// <para/>
    /// This class is marked sealed for runtime performance.
    /// </summary>
    internal sealed class ReadWriteInt16ArrayBuffer : Int16ArrayBuffer
    {
        internal static ReadWriteInt16ArrayBuffer Copy(Int16ArrayBuffer other,
            int markOfOther)
        {
            return new ReadWriteInt16ArrayBuffer(other.Capacity, other.backingArray, other.offset)
            {
                limit = other.Limit,
                position = other.Position,
                mark = markOfOther
            };
        }

        internal ReadWriteInt16ArrayBuffer(short[] array)
            : base(array)
        { }

        internal ReadWriteInt16ArrayBuffer(int capacity)
            : base(capacity)
        { }

        internal ReadWriteInt16ArrayBuffer(int capacity, short[] backingArray, int arrayOffset)
            : base(capacity, backingArray, arrayOffset)
        { }

        public override Int16Buffer AsReadOnlyBuffer() => ReadOnlyInt16ArrayBuffer.Copy(this, mark);

        public override Int16Buffer Compact()
        {
            System.Array.Copy(backingArray, position + offset, backingArray, offset,
                    Remaining);
            position = limit - position;
            limit = capacity;
            mark = UnsetMark;
            return this;
        }

        public override Int16Buffer Duplicate() => Copy(this, mark);

        public override bool IsReadOnly => false;


        [SuppressMessage("Microsoft.Performance", "CA1819", Justification = "design requires some writable array properties")]
        protected override short[] ProtectedArray => backingArray;

        protected override int ProtectedArrayOffset => offset;

        protected override bool ProtectedHasArray => true;

        public override Int16Buffer Put(short value)
        {
            if (position == limit)
            {
                throw new BufferOverflowException();
            }
            backingArray[offset + position++] = value;
            return this;
        }

        public override Int16Buffer Put(int index, short value)
        {
            if (index < 0 || index >= limit)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
            backingArray[offset + index] = value;
            return this;
        }

        public override Int16Buffer Put(short[] source, int offset, int length)
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

        public override Int16Buffer Slice()
        {
            return new ReadWriteInt16ArrayBuffer(Remaining, backingArray, offset + position);
        }
    }
}
