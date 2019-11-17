using System;
using System.Collections.Generic;
using System.Text;

namespace J2N.IO
{
    /// <summary>
    /// <see cref="DoubleArrayBuffer"/>, <see cref="ReadWriteDoubleArrayBuffer"/>, and <see cref="ReadOnlyDoubleArrayBuffer"/>
    /// compose the implementation of array based float buffers.
    /// <para/>
    /// <see cref="DoubleArrayBuffer"/> implements all the shared readonly methods and is extended
    /// by the other two classes.
    /// <para/>
    /// All methods are marked sealed for runtime performance.
    /// </summary>
    internal abstract class DoubleArrayBuffer : DoubleBuffer
    {
        protected internal readonly double[] backingArray;

        protected internal readonly int offset;

        internal DoubleArrayBuffer(double[] array)
            : this(array.Length, array, 0)
        { }

        internal DoubleArrayBuffer(int capacity)
            : this(capacity, new double[capacity], 0)
        { }

        internal DoubleArrayBuffer(int capacity, double[] backingArray, int offset)
            : base(capacity)
        {
            this.backingArray = backingArray;
            this.offset = offset;
        }

        public override sealed double Get()
        {
            if (position == limit)
            {
                throw new BufferUnderflowException();
            }
            return backingArray[offset + position++];
        }

        public override sealed double Get(int index)
        {
            if (index < 0 || index >= limit)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
            return backingArray[offset + index];
        }

        public override sealed DoubleBuffer Get(double[] destination, int offset, int length)
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
    }
}
