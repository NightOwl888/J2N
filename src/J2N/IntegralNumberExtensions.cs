using System;
using System.Runtime.CompilerServices;


namespace J2N
{
    /// <summary>
    /// Extensions to integral numbers such as <see cref="int"/> or <see cref="long"/>.
    /// </summary>
    public static class IntegralNumberExtensions
    {
        #region ToBinaryString

        /// <summary>
        /// Converts the specified <see cref="char"/> into its binary string representation. The
        /// returned string is a concatenation of '0' and '1' characters.
        /// </summary>
        /// <param name="value">The <see cref="char"/> to convert</param>
        /// <returns>The binary string representation of <paramref name="value"/>.</returns>
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif 
        public static string ToBinaryString(this char value) => Convert.ToString(value, 2);

        /// <summary>
        /// Converts the specified <see cref="short"/> into its binary string representation. The
        /// returned string is a concatenation of '0' and '1' characters.
        /// </summary>
        /// <param name="value">The <see cref="short"/> to convert</param>
        /// <returns>The binary string representation of <paramref name="value"/>.</returns>
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif 
        public static string ToBinaryString(this short value)
        {
            if (value > 0)
                return Convert.ToString(value, 2);
            return Convert.ToString((int)value, 2);
        }

        /// <summary>
        /// Converts the specified <see cref="int"/> into its binary string representation. The
        /// returned string is a concatenation of '0' and '1' characters.
        /// </summary>
        /// <param name="value">The <see cref="int"/> to convert</param>
        /// <returns>The binary string representation of <paramref name="value"/>.</returns>
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif 
        public static string ToBinaryString(this int value) => Convert.ToString(value, 2);

        /// <summary>
        /// Converts the specified <see cref="long"/> into its binary string representation. The
        /// returned string is a concatenation of '0' and '1' characters.
        /// </summary>
        /// <param name="value">The <see cref="long"/> to convert</param>
        /// <returns>The binary string representation of <paramref name="value"/>.</returns>
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif 
        public static string ToBinaryString(this long value) => Convert.ToString(value, 2);

        #endregion ToBinaryString

        #region ToHexString

        /// <summary>
        /// Converts the current <see cref="char"/> into its hexadecimal string
        /// representation (in lowercase). The returned string is a 
        /// concatenation of characters from '0' to '9' and 'a' to 'f'.
        /// </summary>
        /// <param name="value">The <see cref="char"/> to convert.</param>
        /// <returns>The hexadecimal string representation of <paramref name="value"/>.</returns>
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif 
        public static string ToHexString(this char value) => Convert.ToString(value, 16);

        /// <summary>
        /// Converts the current <see cref="short"/> into its hexadecimal string
        /// representation (in lowercase). The returned string is a 
        /// concatenation of characters from '0' to '9' and 'a' to 'f'.
        /// </summary>
        /// <param name="value">The <see cref="short"/> to convert.</param>
        /// <returns>The hexadecimal string representation of <paramref name="value"/>.</returns>
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif 
        public static string ToHexString(this short value)
        {
            if (value > 0)
                return Convert.ToString(value, 16);
            return Convert.ToString((int)value, 16);
        }

        /// <summary>
        /// Converts the current <see cref="int"/> into its hexadecimal string
        /// representation (in lowercase). The returned string is a 
        /// concatenation of characters from '0' to '9' and 'a' to 'f'.
        /// </summary>
        /// <param name="value">The <see cref="int"/> to convert.</param>
        /// <returns>The hexadecimal string representation of <paramref name="value"/>.</returns>
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif 
        public static string ToHexString(this int value) => Convert.ToString(value, 16);

        /// <summary>
        /// Converts the current <see cref="long"/> into its hexadecimal string
        /// representation (in lowercase). The returned string is a 
        /// concatenation of characters from '0' to '9' and 'a' to 'f'.
        /// </summary>
        /// <param name="value">The <see cref="long"/> to convert.</param>
        /// <returns>The hexadecimal string representation of <paramref name="value"/>.</returns>
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif 
        public static string ToHexString(this long value) => Convert.ToString(value, 16);

        #endregion

        #region ToOctalString

        /// <summary>
        /// Converts the specified <see cref="char"/> into its octal string representation. The
        /// returned string is a concatenation of characters from '0' to '7'.
        /// </summary>
        /// <param name="value">The <see cref="char"/> to convert.</param>
        /// <returns>The octal string representation of <paramref name="value"/>.</returns>
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif 
        public static string ToOctalString(this char value) => Convert.ToString(value, 8);

        /// <summary>
        /// Converts the specified <see cref="short"/> into its octal string representation. The
        /// returned string is a concatenation of characters from '0' to '7'.
        /// </summary>
        /// <param name="value">The <see cref="short"/> to convert.</param>
        /// <returns>The octal string representation of <paramref name="value"/>.</returns>
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif 
        public static string ToOctalString(this short value)
        {
            if (value > 0)
                return Convert.ToString(value, 8);
            return Convert.ToString((int)value, 8);
        }

        /// <summary>
        /// Converts the specified <see cref="int"/> into its octal string representation. The
        /// returned string is a concatenation of characters from '0' to '7'.
        /// </summary>
        /// <param name="value">The <see cref="int"/> to convert.</param>
        /// <returns>The octal string representation of <paramref name="value"/>.</returns>
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif 
        public static string ToOctalString(this int value) => Convert.ToString(value, 8);

        /// <summary>
        /// Converts the specified <see cref="long"/> into its octal string representation. The
        /// returned string is a concatenation of characters from '0' to '7'.
        /// </summary>
        /// <param name="value">The <see cref="long"/> to convert.</param>
        /// <returns>The octal string representation of <paramref name="value"/>.</returns>
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif 
        public static string ToOctalString(this long value) => Convert.ToString(value, 8);

        #endregion

        #region ToString (radix)

        /// <summary>
        /// Converts the specified <see cref="int"/> into a string representation based on the
        /// specified radix. The returned string is a concatenation of a minus sign
        /// if the number is negative and characters from '0' to '9' and 'a' to 'z',
        /// depending on the radix. If <paramref name="radix"/> is not in the interval defined
        /// by <see cref="Character.MinRadix"/> and <see cref="Character.MaxRadix"/> then 10 is
        /// used as the base for the conversion.
        /// </summary>
        /// <param name="value">The <see cref="int"/> to convert.</param>
        /// <param name="radix">The base to use for the conversion.</param>
        /// <returns>The string representation of <paramref name="value"/>.</returns>
        public static string ToString(this int value, int radix)
        {
            if (radix < Character.MinRadix || radix > Character.MaxRadix)
                radix = 10;
            if (value == 0)
                return "0"; //$NON-NLS-1$

            int count = 2, j = value;
            bool negative = value < 0;
            if (!negative)
            {
                count = 1;
                j = -value;
            }
            while ((value /= radix) != 0)
                count++;

            char[] buffer = new char[count];
            do
            {
                int ch = 0 - (j % radix);
                if (ch > 9)
                    ch = ch - 10 + 'a';
                else
                    ch += '0';
                buffer[--count] = (char)ch;
            } while ((j /= radix) != 0);
            if (negative)
            {
                buffer[0] = '-';
            }
            return new string(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// Converts the specified <see cref="long"/> value into a string representation based on
        /// the specified radix. The returned string is a concatenation of a minus
        /// sign if the number is negative and characters from '0' to '9' and 'a' to
        /// 'z', depending on the radix. If <paramref name="radix"/> is not in the interval
        /// defined by <see cref="Character.MinRadix"/> and <see cref="Character.MaxRadix"/>
        /// then 10 is used as the base for the conversion.
        /// </summary>
        /// <param name="value">The long to convert.</param>
        /// <param name="radix">The base to use for the conversion.</param>
        /// <returns>The string representation of <paramref name="value"/>.</returns>
        public static string ToString(this long value, int radix)
        {
            if (radix < Character.MinRadix || radix > Character.MaxRadix)
                radix = 10;
            if (value == 0)
                return "0"; //$NON-NLS-1$

            int count = 2;
            long j = value;
            bool negative = value < 0;
            if (!negative)
            {
                count = 1;
                j = -value;
            }
            while ((value /= radix) != 0)
                count++;

            char[] buffer = new char[count];
            do
            {
                int ch = 0 - (int)(j % radix);
                if (ch > 9)
                    ch = ch - 10 + 'a';
                else
                    ch += '0';
                buffer[--count] = (char)ch;
            } while ((j /= radix) != 0);
            if (negative)
                buffer[0] = '-';
            return new string(buffer, 0, buffer.Length);
        }

        #endregion ToString (radix)
    }
}
