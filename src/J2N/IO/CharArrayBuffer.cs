using J2N.Text;
using System;

namespace J2N.IO
{
    /// <summary>
    /// <see cref="CharArrayBuffer"/>, <see cref="ReadWriteCharArrayBuffer"/> and <see cref="ReadOnlyCharArrayBuffer"/> compose
    /// the implementation of array based char buffers.
    /// <para/>
    /// <see cref="CharArrayBuffer"/> implements all the shared readonly methods and is extended by
    /// the other two classes.
    /// <para/>
    /// All methods are marked sealed for runtime performance.
    /// </summary>
    internal abstract class CharArrayBuffer : CharBuffer
    {
        protected internal readonly char[] backingArray;

        protected internal readonly int offset;

        internal CharArrayBuffer(char[] array)
            : this(array.Length, array, 0)
        {
        }

        internal CharArrayBuffer(int capacity)
            : this(capacity, new char[capacity], 0)
        {
        }

        internal CharArrayBuffer(int capacity, char[] backingArray, int offset)
            : base(capacity)
        {
            this.backingArray = backingArray;
            this.offset = offset;
        }

        public override sealed char Get()
        {
            if (position == limit)
            {
                throw new BufferUnderflowException();
            }
            return backingArray[offset + position++];
        }

        public override sealed char Get(int index)
        {
            if (index < 0 || index >= limit)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
            return backingArray[offset + index];
        }

        public override sealed CharBuffer Get(char[] destination, int offset, int length)
        {
            if (destination == null)
                throw new ArgumentNullException(nameof(destination));

            int len = destination.Length;
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset));
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length));
            if ((long)offset + (long)length > len)
                throw new ArgumentOutOfRangeException(string.Empty, $"{nameof(offset)} + {nameof(length)} > {nameof(destination.Length)}");
            if (length > Remaining)
                throw new BufferUnderflowException();

            System.Array.Copy(backingArray, this.offset + position, destination, offset, length);
            position += length;
            return this;
        }

        //public override sealed bool IsDirect => false;

        public override sealed ByteOrder Order => ByteOrder.NativeOrder;

        public override sealed CharBuffer Subsequence(int startIndex, int length)
        {
            if (startIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(startIndex));
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length));
            if (startIndex + length > Remaining)
                throw new ArgumentOutOfRangeException("", $"{nameof(startIndex)} + {nameof(length)} > {nameof(Length)}");

            CharBuffer result = Duplicate();
            result.Limit = position + startIndex + length;
            result.Position = position + startIndex;
            return result;
        }

        public override sealed string ToString()
        {
            return new string(backingArray, offset + position, Remaining);
        }
    }
}
