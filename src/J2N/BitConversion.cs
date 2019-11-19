using System;
using System.Runtime.CompilerServices;

namespace J2N
{
    /// <summary>
    /// Converts base data types to an array of bytes, and an array of bytes to base data types.
    /// <para/>
    /// This class is a supplement to <see cref="System.BitConverter"/> to provide functionality similar to
    /// that in the JDK.
    /// </summary>
    public static class BitConversion // J2N TODO: Benchmarks - check the benefits of using unsafe pointers vs 2 method calls to BitConverter
    {
        /// <summary>
        /// NOTE: This was intBitsToFloat() in the JDK
        /// </summary>
        public static float Int32BitsToSingle(int value)
        {
            return BitConverter.ToSingle(BitConverter.GetBytes(value), 0);
        }

        /// <summary>
        /// NOTE: This was floatToRawIntBits() in the JDK
        /// </summary>
        public static int SingleToRawInt32Bits(float value)
        {
            // TODO: does this handle NaNs the same?
            return BitConverter.ToInt32(BitConverter.GetBytes(value), 0);
        }

        /// <summary>
        /// NOTE: This was floatToIntBits() in the JDK
        /// </summary>
        public static int SingleToInt32Bits(float value)
        {
            if (float.IsNaN(value))
            {
                return 0x7fc00000;
            }

            // TODO it is claimed that this could be faster
            return BitConverter.ToInt32(BitConverter.GetBytes(value), 0);
        }

        /// <summary>
        /// NOTE: This was floatToLongBits() in the JDK
        /// </summary>
        public static long SingleToInt64Bits(float value)
        {
            return BitConverter.ToInt64(BitConverter.GetBytes(value), 0);
        }

        /// <summary>
        /// NOTE: This was doubleToRawLongBits() in the JDK
        /// </summary>
        public static long DoubleToRawInt64Bits(double value)
        {
            return BitConverter.DoubleToInt64Bits(value);
        }

        /// <summary>
        /// NOTE: This was doubleToLongBits() in the JDK
        /// </summary>
        public static long DoubleToInt64Bits(double value)
        {
            if (double.IsNaN(value))
            {
                return 0x7ff8000000000000L;
            }

            return BitConverter.DoubleToInt64Bits(value);
        }

        /// <summary>
        /// NOTE: This was longBitsToDouble() in the JDK
        /// </summary>
        public static double Int64BitsToDouble(long value)
        {
            return BitConverter.Int64BitsToDouble(value);
        }

        ///// <summary>
        ///// NOTE: This was intBitsToFloat() in the JDK
        ///// </summary>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static unsafe float Int32BitsToSingle(int value)
        //{
        //    return *((float*)&value);
        //}

        ///// <summary>
        ///// NOTE: This was floatToRawIntBits() in the JDK
        ///// </summary>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static unsafe int SingleToRawInt32Bits(float value)
        //{
        //    return *((int*)&value);
        //}

        /////// <summary>
        /////// NOTE: This was intBitsToFloat() in the JDK
        /////// </summary>
        ////public static float Int32BitsToSingle(int value)
        ////{
        ////    return BitConverter.ToSingle(BitConverter.GetBytes(value), 0);
        ////}

        /////// <summary>
        /////// NOTE: This was floatToRawIntBits() in the JDK
        /////// </summary>
        ////public static int SingleToRawInt32Bits(float value)
        ////{
        ////    // TODO: does this handle NaNs the same?
        ////    return BitConverter.ToInt32(BitConverter.GetBytes(value), 0);
        ////}

        ///// <summary>
        ///// NOTE: This was floatToIntBits() in the JDK
        ///// </summary>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static unsafe int SingleToInt32Bits(float value)
        //{
        //    if (float.IsNaN(value))
        //    {
        //        return 0x7fc00000;
        //    }

        //    // TODO it is claimed that this could be faster
        //    //return BitConverter.ToInt32(BitConverter.GetBytes(value), 0);
        //    return *((int*)&value);
        //}

        ///// <summary>
        ///// NOTE: This was floatToLongBits() in the JDK
        ///// </summary>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static unsafe long SingleToInt64Bits(float value)
        //{
        //    //return BitConverter.ToInt64(BitConverter.GetBytes(value), 0);
        //    return *((long*)&value);
        //}

        ///// <summary>
        ///// NOTE: This was doubleToRawLongBits() in the JDK
        ///// </summary>
        ////public static long DoubleToRawInt64Bits(double value)
        ////{
        ////    return BitConverter.DoubleToInt64Bits(value);
        ////}
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static unsafe long DoubleToRawInt64Bits(double value)
        //{
        //    return *((long*)&value);
        //}

        ///// <summary>
        ///// NOTE: This was doubleToLongBits() in the JDK
        ///// </summary>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static unsafe long DoubleToInt64Bits(double value)
        //{
        //    if (double.IsNaN(value))
        //    {
        //        return 0x7ff8000000000000L;
        //    }

        //    return *((long*)&value);
        //    //return BitConverter.DoubleToInt64Bits(value);
        //}

        ///// <summary>
        ///// NOTE: This was longBitsToDouble() in the JDK
        ///// </summary>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static unsafe double Int64BitsToDouble(long value)
        //{
        //    return *((double*)&value);
        //    //return BitConverter.Int64BitsToDouble(value);
        //}
    }
}
