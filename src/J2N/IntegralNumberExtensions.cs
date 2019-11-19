using System;
using System.Runtime.CompilerServices;

namespace J2N
{
    /// <summary>
    /// Extensions to integral numbers such as <see cref="int"/> or <see cref="long"/>.
    /// </summary>
    public static class IntegralNumberExtensions
    {
        #region ToHexString

        /// <summary>
        /// Converts the current <see cref="char"/> into its hexadecimal string
        /// representation (in lowercase). The returned string is a 
        /// concatenation of characters from '0' to '9' and 'a' to 'f'.
        /// </summary>
        /// <param name="value">The integer to convert.</param>
        /// <returns>The hexadecimal string representation of <paramref name="value"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToHexString(this char value)
        {

            return ((int)value).ToString("x");
            
        }

        /// <summary>
        /// Converts the current <see cref="int"/> into its hexadecimal string
        /// representation (in lowercase). The returned string is a 
        /// concatenation of characters from '0' to '9' and 'a' to 'f'.
        /// </summary>
        /// <param name="value">The integer to convert.</param>
        /// <returns>The hexadecimal string representation of <paramref name="value"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToHexString(this int value)
        {
            return value.ToString("x");
        }

        /// <summary>
        /// Converts the current <see cref="long"/> into its hexadecimal string
        /// representation (in lowercase). The returned string is a 
        /// concatenation of characters from '0' to '9' and 'a' to 'f'.
        /// </summary>
        /// <param name="value">The integer to convert.</param>
        /// <returns>The hexadecimal string representation of <paramref name="value"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToHexString(this long value)
        {
            return value.ToString("x");
        }

        #endregion

        #region TripleShift

        /// <summary>
        /// Performs an unsigned bitwise right shift to the current <paramref name="number"/>.
        /// <para/>
        /// Usage Note: Replacement for Java triple shift (>>>) operator. The
        /// value is converted to <see cref="sbyte"/> first before doing the conversion,
        /// so the result is the same as it is for the Java byte, which is signed.
        /// </summary>
        /// <param name="number">Number to operate on.</param>
        /// <param name="bits">Ammount of bits to shift.</param>
        /// <returns>The resulting number from the shift operation as <see cref="int"/>.</returns>
        // See http://stackoverflow.com/a/6625912
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int TripleShift(this byte number, int bits)
        {
            sbyte num = (sbyte)number;
            if (num >= 0)
                return num >> bits;
            return (num >> bits) + (2 << ~bits);
        }

        /// <summary>
        /// Performs an unsigned bitwise right shift to the current <paramref name="number"/>.
        /// <para/>
        /// Usage Note: Replacement for Java triple shift (>>>) operator.
        /// </summary>
        /// <param name="number">Number to operate on.</param>
        /// <param name="bits">Ammount of bits to shift.</param>
        /// <returns>The resulting number from the shift operation as <see cref="int"/>.</returns>
        // See http://stackoverflow.com/a/6625912
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CLSCompliant(false)]
        public static int TripleShift(this sbyte number, int bits)
        {
            if (number >= 0)
                return number >> bits;
            return (number >> bits) + (2 << ~bits);
        }

        /// <summary>
        /// Performs an unsigned bitwise right shift to the current <paramref name="number"/>.
        /// <para/>
        /// Usage Note: Replacement for Java triple shift (>>>) operator.
        /// </summary>
        /// <param name="number">Number to operate on.</param>
        /// <param name="bits">Ammount of bits to shift.</param>
        /// <returns>The resulting number from the shift operation as <see cref="int"/>.</returns>
        // See http://stackoverflow.com/a/6625912
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int TripleShift(this char number, int bits)
        {
            if (number >= 0)
                return number >> bits;
            return (number >> bits) + (2 << ~bits);
        }

        /// <summary>
        /// Performs an unsigned bitwise right shift to the current <paramref name="number"/>.
        /// <para/>
        /// Usage Note: Replacement for Java triple shift (>>>) operator.
        /// </summary>
        /// <param name="number">Number to operate on.</param>
        /// <param name="bits">Ammount of bits to shift.</param>
        /// <returns>The resulting number from the shift operation as <see cref="short"/>.</returns>
        // See http://stackoverflow.com/a/6625912
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int TripleShift(this short number, int bits)
        {
            if (number >= 0)
                return number >> bits;
            return (number >> bits) + (2 << ~bits);
        }

        /// <summary>
        /// Performs an unsigned bitwise right shift to the current <paramref name="number"/>.
        /// <para/>
        /// Usage Note: Replacement for Java triple shift (>>>) operator.
        /// </summary>
        /// <param name="number">Number to operate on.</param>
        /// <param name="bits">Ammount of bits to shift.</param>
        /// <returns>The resulting number from the shift operation as <see cref="int"/>.</returns>
        // See http://stackoverflow.com/a/6625912
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int TripleShift(this int number, int bits)
        {
            if (number >= 0)
                return number >> bits;
            return (number >> bits) + (2 << ~bits);
        }

        /// <summary>
        /// Performs an unsigned bitwise right shift to the current <paramref name="number"/>.
        /// <para/>
        /// Usage Note: Replacement for Java triple shift (>>>) operator.
        /// </summary>
        /// <param name="number">Number to operate on.</param>
        /// <param name="bits">Ammount of bits to shift.</param>
        /// <returns>The resulting number from the shift operation as <see cref="long"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long TripleShift(this long number, int bits)
        {
            return (long)((ulong)number >> bits);
        }

        #endregion

    }
}
