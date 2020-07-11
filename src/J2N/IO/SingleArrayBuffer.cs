using System;

namespace J2N.IO
{
    using SR = J2N.Resources.Strings;

    /// <summary>
    /// <see cref="SingleArrayBuffer"/>, <see cref="ReadWriteSingleArrayBuffer"/>, and <see cref="ReadOnlySingleArrayBuffer"/>
    /// compose the implementation of array based float buffers.
    /// <para/>
    /// <see cref="SingleArrayBuffer"/> implements all the shared readonly methods and is extended
    /// by the other two classes.
    /// <para/>
    /// All methods are marked sealed for runtime performance.
    /// </summary>
    internal abstract class SingleArrayBuffer : SingleBuffer
    {
        protected internal readonly float[] backingArray;

        protected internal readonly int offset;

        internal SingleArrayBuffer(float[] array)
            : this(array.Length, array, 0)
        { }

        internal SingleArrayBuffer(int capacity)
            : this(capacity, new float[capacity], 0)
        { }

        internal SingleArrayBuffer(int capacity, float[] backingArray, int offset)
            : base(capacity)
        {
            this.backingArray = backingArray;
            this.offset = offset;
        }

        public override sealed float Get()
        {
            if (position == limit)
            {
                throw new BufferUnderflowException();
            }
            return backingArray[offset + position++];
        }

        public override sealed float Get(int index)
        {
            if (index < 0 || index >= limit)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
            return backingArray[offset + index];
        }

        public override sealed SingleBuffer Get(float[] destination, int offset, int length)
        {
            if (destination == null)
                throw new ArgumentNullException(nameof(destination));

            int len = destination.Length;
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset), SR.ArgumentOutOfRange_NeedNonNegNum);
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length), SR.ArgumentOutOfRange_NeedNonNegNum);
            if (offset > len - length) // Checks for int overflow
                throw new ArgumentOutOfRangeException(nameof(length), SR.ArgumentOutOfRange_IndexLength);
            if (length > Remaining)
                throw new BufferUnderflowException();

            System.Array.Copy(backingArray, this.offset + position, destination, offset, length);
            position += length;
            return this;
        }

        //public override sealed bool IsDirect => false;

        public override sealed ByteOrder Order => ByteOrder.NativeOrder;
    }
}
