using System;


namespace J2N.IO
{
    /// <summary>
    /// Defines byte order constants.
    /// </summary>
    public sealed class ByteOrder
    {
        /// <summary>
        /// This constant represents big endian.
        /// </summary>
        public static readonly ByteOrder BigEndian = new ByteOrder(nameof(BigEndian)); //$NON-NLS-1$

        /// <summary>
        /// This constant represents little endian.
        /// </summary>
        public static readonly ByteOrder LittleEndian = new ByteOrder(nameof(LittleEndian)); //$NON-NLS-1$

        /// <summary>
        /// Returns the current platform byte order.
        /// </summary>
        public static ByteOrder NativeOrder { get; private set; } = LoadNativeByteOrder();
        private static ByteOrder LoadNativeByteOrder()
        {
            // Read endianness from the current system.
            return BitConverter.IsLittleEndian ? LittleEndian : BigEndian;
        }

        private readonly string name;

        private ByteOrder(string name)
        {
            this.name = name;
        }

        /// <summary>
        /// Returns a string that describes this object.
        /// </summary>
        /// <returns>
        /// "BigEndian" for <see cref="ByteOrder.BigEndian"/> objects,
        /// "LittleEndian" for <see cref="ByteOrder.LittleEndian"/> objects.
        /// </returns>
        public override string ToString()
        {
            return name;
        }
    }
}
