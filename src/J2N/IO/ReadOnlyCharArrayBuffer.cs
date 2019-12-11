using System;
using System.Diagnostics.CodeAnalysis;

namespace J2N.IO
{
    /// <summary>
    /// <see cref="CharArrayBuffer"/>, <see cref="ReadWriteCharArrayBuffer"/> and <see cref="ReadOnlyCharArrayBuffer"/> compose
    /// the implementation of array based char buffers.
    /// <para/>
    /// <see cref="ReadOnlyCharArrayBuffer"/> extends <see cref="CharArrayBuffer"/> with all the write methods
    /// throwing read only exception.
    /// <para/>
    /// This class is marked sealed for runtime performance.
    /// </summary>
    internal sealed class ReadOnlyCharArrayBuffer : CharArrayBuffer
    {
        internal static ReadOnlyCharArrayBuffer Copy(CharArrayBuffer other, int markOfOther)
        {
            return new ReadOnlyCharArrayBuffer(other.Capacity, other.backingArray, other.offset)
            {
                limit = other.Limit,
                position = other.Position,
                mark = markOfOther
            };
        }

        internal ReadOnlyCharArrayBuffer(int capacity, char[] backingArray, int arrayOffset)
            : base(capacity, backingArray, arrayOffset)
        { }

        public override CharBuffer AsReadOnlyBuffer() => Duplicate();

        public override CharBuffer Compact()
        {
            throw new ReadOnlyBufferException();
        }

        public override CharBuffer Duplicate() => Copy(this, mark);

        public override bool IsReadOnly => true;

        [SuppressMessage("Microsoft.Performance", "CA1819", Justification = "design requires some writable array properties")]
        protected override char[] ProtectedArray
        {
            get { throw new ReadOnlyBufferException(); }
        }

        protected override int ProtectedArrayOffset
        {
            get { throw new ReadOnlyBufferException(); }
        }

        protected override bool ProtectedHasArray => false;

        public override CharBuffer Put(char value)
        {
            throw new ReadOnlyBufferException();
        }

        public override CharBuffer Put(int index, char value)
        {
            throw new ReadOnlyBufferException();
        }

        public override sealed CharBuffer Put(char[] source, int offset, int length)
        {
            throw new ReadOnlyBufferException();
        }

        public override sealed CharBuffer Put(CharBuffer src)
        {
            throw new ReadOnlyBufferException();
        }

        public override CharBuffer Put(string source, int startIndex, int length)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            int len = source.Length;
            if (startIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(startIndex));
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length));
            if ((long)startIndex + (long)length > len)
                throw new ArgumentOutOfRangeException(string.Empty, $"{nameof(startIndex)} + {nameof(length)} > {nameof(source.Length)}");

            throw new ReadOnlyBufferException();
        }

        public override CharBuffer Slice()
        {
            return new ReadOnlyCharArrayBuffer(Remaining, backingArray, offset + position);
        }
    }
}
