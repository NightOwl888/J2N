using System;
using System.Runtime.CompilerServices;

namespace J2N.Memory
{
    /// <summary>
    /// A ref struct that is used to store <see cref="int"/> values on the stack.
    /// Stores up to 32 <see cref="int"/> values via slot index. These values can be 0 - 65535.
    /// <para/>
    /// Use this when an array is too expensive and there aren't that many individual values to contend with.
    /// This instance will be allocated on the stack where it is declared, which limits its lifetime to within
    /// the same method call.
    /// </summary>
    internal ref struct ValueInt32Packer // KEEP IN SYNC WITH Int32Packer
    {
        public const int BitsPerSlot = 16; // Number of bits allocated for each slot
        internal const int SlotsPerULong = 64 / BitsPerSlot; // Number of slots per ulong variable
        public const int TotalSlots = 32; // Maximum number of slots
        public const int MinValue = 0;
        public const int MaxValue = (1 << BitsPerSlot) - 1; // Maximum value that can be stored with BitsPerSlot bits

        private ulong value1;
        private ulong value2;
        private ulong value3;
        private ulong value4;
        private ulong value5;
        private ulong value6;
        private ulong value7;
        private ulong value8;

#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        internal static ulong GetSlotMask(int slotIndex) // Internal for testing
        {
            int bitOffset = (slotIndex % SlotsPerULong) * BitsPerSlot;
            return (ulong)MaxValue << bitOffset;
        }

        public void SetValue(int slotIndex, int value)
        {
            if (slotIndex < 0 || slotIndex >= TotalSlots)
            {
                throw new ArgumentOutOfRangeException(nameof(slotIndex));
            }

            if (value < 0 || value > MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }

            ulong mask = GetSlotMask(slotIndex);

            switch (slotIndex / SlotsPerULong)
            {
                case 0:
                    value1 &= ~mask;
                    value1 |= (ulong)value << (slotIndex % SlotsPerULong) * BitsPerSlot;
                    break;
                case 1:
                    value2 &= ~mask;
                    value2 |= (ulong)value << (slotIndex % SlotsPerULong) * BitsPerSlot;
                    break;
                case 2:
                    value3 &= ~mask;
                    value3 |= (ulong)value << (slotIndex % SlotsPerULong) * BitsPerSlot;
                    break;
                case 3:
                    value4 &= ~mask;
                    value4 |= (ulong)value << (slotIndex % SlotsPerULong) * BitsPerSlot;
                    break;
                case 4:
                    value5 &= ~mask;
                    value5 |= (ulong)value << (slotIndex % SlotsPerULong) * BitsPerSlot;
                    break;
                case 5:
                    value6 &= ~mask;
                    value6 |= (ulong)value << (slotIndex % SlotsPerULong) * BitsPerSlot;
                    break;
                case 6:
                    value7 &= ~mask;
                    value7 |= (ulong)value << (slotIndex % SlotsPerULong) * BitsPerSlot;
                    break;
                case 7:
                    value8 &= ~mask;
                    value8 |= (ulong)value << (slotIndex % SlotsPerULong) * BitsPerSlot;
                    break;
            }
        }

        public int GetValue(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= TotalSlots)
            {
                throw new ArgumentOutOfRangeException(nameof(slotIndex));
            }

            ulong mask = GetSlotMask(slotIndex);

            switch (slotIndex / SlotsPerULong)
            {
                case 0:
                    return (int)((value1 & mask) >> (slotIndex % SlotsPerULong) * BitsPerSlot);
                case 1:
                    return (int)((value2 & mask) >> (slotIndex % SlotsPerULong) * BitsPerSlot);
                case 2:
                    return (int)((value3 & mask) >> (slotIndex % SlotsPerULong) * BitsPerSlot);
                case 3:
                    return (int)((value4 & mask) >> (slotIndex % SlotsPerULong) * BitsPerSlot);
                case 4:
                    return (int)((value5 & mask) >> (slotIndex % SlotsPerULong) * BitsPerSlot);
                case 5:
                    return (int)((value6 & mask) >> (slotIndex % SlotsPerULong) * BitsPerSlot);
                case 6:
                    return (int)((value7 & mask) >> (slotIndex % SlotsPerULong) * BitsPerSlot);
                case 7:
                    return (int)((value8 & mask) >> (slotIndex % SlotsPerULong) * BitsPerSlot);
                default:
                    return 0; // This should not happen
            }
        }

        public void PrintPackedValues()
        {
            Console.WriteLine($"Value 1: {Convert.ToString((long)value1, 2).PadLeft(64, '0')}");
            Console.WriteLine($"Value 2: {Convert.ToString((long)value2, 2).PadLeft(64, '0')}");
            Console.WriteLine($"Value 3: {Convert.ToString((long)value3, 2).PadLeft(64, '0')}");
            Console.WriteLine($"Value 4: {Convert.ToString((long)value4, 2).PadLeft(64, '0')}");
            Console.WriteLine($"Value 5: {Convert.ToString((long)value5, 2).PadLeft(64, '0')}");
            Console.WriteLine($"Value 6: {Convert.ToString((long)value6, 2).PadLeft(64, '0')}");
            Console.WriteLine($"Value 7: {Convert.ToString((long)value7, 2).PadLeft(64, '0')}");
            Console.WriteLine($"Value 8: {Convert.ToString((long)value8, 2).PadLeft(64, '0')}");
        }
    }
}
