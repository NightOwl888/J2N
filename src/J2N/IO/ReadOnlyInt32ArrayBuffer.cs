﻿using System.Diagnostics.CodeAnalysis;


namespace J2N.IO
{
    /// <summary>
    /// <see cref="Int32ArrayBuffer"/>, <see cref="ReadWriteInt32ArrayBuffer"/> and <see cref="ReadOnlyInt32ArrayBuffer"/> compose
    /// the implementation of array based <see cref="int"/> buffers.
    /// <para/>
    /// <see cref="ReadOnlyInt32ArrayBuffer"/> extends <see cref="Int32ArrayBuffer"/> with all the write methods
    /// throwing read only exception.
    /// <para/>
    /// This class is marked sealed for runtime performance.
    /// </summary>
    internal sealed class ReadOnlyInt32ArrayBuffer : Int32ArrayBuffer
    {
        internal static ReadOnlyInt32ArrayBuffer Copy(Int32ArrayBuffer other, int markOfOther)
        {
            return new ReadOnlyInt32ArrayBuffer(other.Capacity, other.backingArray, other.offset)
            {
                limit = other.Limit,
                position = other.Position,
                mark = markOfOther
            };
        }

        internal ReadOnlyInt32ArrayBuffer(int capacity, int[] backingArray, int arrayOffset)
            : base(capacity, backingArray, arrayOffset)
        { }

        public override Int32Buffer AsReadOnlyBuffer() => Duplicate();

        public override Int32Buffer Compact()
        {
            throw new ReadOnlyBufferException();
        }

        public override Int32Buffer Duplicate() => Copy(this, mark);

        public override bool IsReadOnly => true;

        [SuppressMessage("Microsoft.Performance", "CA1819", Justification = "design requires some writable array properties")]
        protected override int[] ProtectedArray
        {
            get { throw new ReadOnlyBufferException(); }
        }

        protected override int ProtectedArrayOffset
        {
            get { throw new ReadOnlyBufferException(); }
        }

        protected override bool ProtectedHasArray => false;

        public override Int32Buffer Put(int value)
        {
            throw new ReadOnlyBufferException();
        }

        public override Int32Buffer Put(int index, int value)
        {
            throw new ReadOnlyBufferException();
        }

        public override Int32Buffer Put(Int32Buffer buffer)
        {
            throw new ReadOnlyBufferException();
        }

        public override sealed Int32Buffer Put(int[] source, int offset, int length)
        {
            throw new ReadOnlyBufferException();
        }

        public override Int32Buffer Slice()
        {
            return new ReadOnlyInt32ArrayBuffer(Remaining, backingArray, offset + position);
        }
    }
}
