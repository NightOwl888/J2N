// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.CompilerServices;


namespace J2N
{
    /// <summary>
    /// Converts base data types to an array of bytes, and an array of bytes to base data types.
    /// <para/>
    /// This class is a supplement to <see cref="System.BitConverter"/> to provide functionality similar to
    /// that in the JDK.
    /// <para/>
    /// Usage Note: When porting code from Java, we recommend using <see cref="BitConversion"/> methods exclusively, as
    /// <see cref="SingleToInt32Bits(float)"/> and <see cref="DoubleToInt64Bits(double)"/> are NOT
    /// equivalent to BitConverter.SingleToInt32Bits(float) (where implemented) and <see cref="BitConverter.DoubleToInt64Bits(double)"/>.
    /// In <see cref="SingleToInt32Bits(float)"/> and <see cref="DoubleToInt64Bits(double)"/>, all NaN values are normalized
    /// to a single value, but .NET has no similar built-in functionality.
    /// </summary>
    public static class BitConversion
    {
        /// <summary>
        /// Returns the <see cref="float"/> value corresponding to a given
        /// bit representation.
        /// The argument is considered to be a representation of a
        /// floating-point value according to the IEEE 754 floating-point
        /// "single format" bit layout.
        ///
        /// <para/>If the argument is <c>0x7f800000</c>, the result is positive
        /// infinity.
        ///
        /// <para/>If the argument is <c>0xff800000</c>, the result is negative
        /// infinity.
        ///
        /// <para/>If the argument is any value in the range
        /// <c>0x7f800001</c> through <c>0x7fffffff</c> or in
        /// the range <c>0xff800001</c> through
        /// <c>0xffffffff</c>, the result is a NaN.  No IEEE 754
        /// floating-point operation provided by .NET can distinguish
        /// between two NaN values of the same type with different bit
        /// patterns.  Distinct values of NaN are only distinguishable by
        /// use of the <see cref="SingleToRawInt32Bits(float)"/> method.
        ///
        /// <para/>In all other cases, let <i>s</i>, <i>e</i>, and <i>m</i> be three
        /// values that can be computed from the argument:
        ///
        /// <code>
        /// int s = ((bits &gt;&gt; 31) == 0) ? 1 : -1;
        /// int e = ((bits &gt;&gt; 23) &amp; 0xff);
        /// int m = (e == 0) ?
        ///                 (bits &amp; 0x7fffff) &lt;&lt; 1 :
        ///                 (bits &amp; 0x7fffff) | 0x800000;
        /// </code>
        ///
        /// Then the floating-point result equals the value of the mathematical
        /// expression <i>s</i>&#183;<i>m</i>&#183;2<sup><i>e</i>-150</sup>.
        ///
        /// <para/>Note that this method may not be able to return a
        /// <see cref="float.NaN"/> with exactly same bit pattern as the
        /// <see cref="int"/> argument.  IEEE 754 distinguishes between two
        /// kinds of NaNs, quiet NaNs and <i>signaling NaNs</i>.  The
        /// differences between the two kinds of NaN are generally not
        /// visible in .NET.  Arithmetic operations on signaling NaNs turn
        /// them into quiet NaNs with a different, but often similar, bit
        /// pattern.  However, on some processors merely copying a
        /// signaling NaN also performs that conversion.  In particular,
        /// copying a signaling NaN to return it to the calling method may
        /// perform this conversion.  So <see cref="Int32BitsToSingle(int)"/> may
        /// not be able to return a <see cref="float"/> with a signaling NaN
        /// bit pattern.  Consequently, for some <see cref="int"/> values,
        /// <c>BitConversion.SingleToRawInt32Bits(BitConversion.Int32BitsToSingle(start))</c> may
        /// <i>not</i> equal <c>start</c>.  Moreover, which
        /// particular bit patterns represent signaling NaNs is platform
        /// dependent; although all NaN bit patterns, quiet or signaling,
        /// must be in the NaN range identified above.
        /// 
        /// <para/>
        /// NOTE: This corresponds to Float.intBitsToFloat(int) in the JDK
        /// and BitConverter.Int32BitsToSingle(int) in .NET (where implemented).
        /// </summary>
        /// <param name="value">The integer to convert.</param>
        /// <returns>A single-precision floating-point value that represents the converted integer.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe float Int32BitsToSingle(int value)
        {
            return *((float*)&value);
        }

        /// <summary>
        /// Returns a representation of the specified floating-point value
        /// according to the IEEE 754 floating-point "single format" bit
        /// layout, preserving Not-a-Number (NaN) values.
        ///
        /// <para/>Bit 31 (the bit that is selected by the mask
        /// <c>0x80000000</c>) represents the sign of the floating-point
        /// number.
        /// Bits 30-23 (the bits that are selected by the mask
        /// <c>0x7f800000</c>) represent the exponent.
        /// Bits 22-0 (the bits that are selected by the mask
        /// <c>0x007fffff</c>) represent the significand (sometimes called
        /// the mantissa) of the floating-point number.
        ///
        /// <para/>If the argument is positive infinity, the result is
        /// <c>0x7f800000</c>.
        ///
        /// <para/>If the argument is negative infinity, the result is
        /// <c>0xff800000</c>.
        ///
        /// <para/>If the argument is NaN, the result is the integer representing
        /// the actual NaN value.  Unlike the <see cref="SingleToInt32Bits(float)"/>
        /// method, <see cref="SingleToRawInt32Bits(float)"/> does not collapse all the
        /// bit patterns encoding a NaN to a single "canonical"
        /// NaN value.
        ///
        /// <para/>In all cases, the result is an integer that, when given to the
        /// <see cref="Int32BitsToSingle(int)"/> method, will produce a
        /// floating-point value the same as the argument to
        /// <see cref="SingleToRawInt32Bits(float)"/>.
        /// 
        /// <para/>
        /// NOTE: This corresponds to Float.floatToRawIntBits(float) in the JDK
        /// and BitConverter.SingleToInt32Bits(float) in .NET (where implemented).
        /// </summary>
        /// <param name="value">A floating-point number.</param>
        /// <returns>The bits that represent the floating-point number.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe int SingleToRawInt32Bits(float value)
        {
            return *((int*)&value);
        }

        /// <summary>
        /// Returns a representation of the specified floating-point value
        /// according to the IEEE 754 floating-point "single format" bit
        /// layout.
        ///
        /// <para/>Bit 31 (the bit that is selected by the mask
        /// <c>0x80000000</c>) represents the sign of the floating-point
        /// number.
        /// Bits 30-23 (the bits that are selected by the mask
        /// <c>0x7f800000</c>) represent the exponent.
        /// Bits 22-0 (the bits that are selected by the mask
        /// <c>0x007fffff</c>) represent the significand (sometimes called
        /// the mantissa) of the floating-point number.
        ///
        /// <para/>If the argument is positive infinity, the result is
        /// <c>0x7f800000</c>.
        ///
        /// <para/>If the argument is negative infinity, the result is
        /// <c>0xff800000</c>.
        ///
        /// <para/>If the argument is NaN, the result is <c>0x7fc00000</c>.
        ///
        /// <para/>In all cases, the result is an integer that, when given to the
        /// <see cref="Int32BitsToSingle(int)"/> method, will produce a floating-point
        /// value the same as the argument to <see cref="SingleToInt32Bits(float)"/>
        /// (except all NaN values are collapsed to a single
        /// "canonical" NaN value).
        /// 
        /// <para/>
        /// NOTE: This corresponds to Float.floatToIntBits() in the JDK.
        /// There is no corresponding method in .NET.
        /// </summary>
        /// <param name="value">A floating-point number.</param>
        /// <returns>The bits that represent the floating-point number.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe int SingleToInt32Bits(float value)
        {
            if (float.IsNaN(value))
                return 0x7fc00000;

            return *((int*)&value);
        }

        /// <summary>
        /// Similar to <see cref="SingleToInt32Bits(float)"/>, but
        /// returns the result as <see cref="long"/> rather than <see cref="int"/>.
        /// 
        /// <para/>
        /// NOTE: This corresponds to BitConverter.SingleToInt64Bits in
        /// .NET (where implemented).
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe long SingleToInt64Bits(float value)
        {
            return *((long*)&value);
        }

        /// <summary>
        /// Returns the <see cref="double"/> value corresponding to a given
        /// bit representation.
        /// The argument is considered to be a representation of a
        /// floating-point value according to the IEEE 754 floating-point
        /// "double format" bit layout.
        ///
        /// <para/>If the argument is <c>0x7ff0000000000000L</c>, the result
        /// is positive infinity.
        ///
        /// <para/>If the argument is <c>0xfff0000000000000L</c>, the result
        /// is negative infinity.
        ///
        /// <para/>If the argument is any value in the range
        /// <c>0x7ff0000000000001L</c> through
        /// <c>0x7fffffffffffffffL</c> or in the range
        /// <c>0xfff0000000000001L</c> through
        /// <c>0xffffffffffffffffL</c>, the result is a NaN.  No IEEE
        /// 754 floating-point operation provided by .NET can distinguish
        /// between two NaN values of the same type with different bit
        /// patterns.  Distinct values of NaN are only distinguishable by
        /// use of the <see cref="DoubleToRawInt64Bits(double)"/> method.
        ///
        /// <para/>In all other cases, let <i>s</i>, <i>e</i>, and <i>m</i> be three
        /// values that can be computed from the argument:
        ///
        /// <code>
        /// int s = ((bits &gt;&gt; 63) == 0) ? 1 : -1;
        /// int e = (int)((bits &gt;&gt; 52) &amp; 0x7ffL);
        /// long m = (e == 0) ?
        ///                 (bits &amp; 0xfffffffffffffL) &lt;&lt; 1 :
        ///                 (bits &amp; 0xfffffffffffffL) | 0x10000000000000L;
        /// </code>
        ///
        /// Then the floating-point result equals the value of the mathematical
        /// expression <i>s</i>&#183;<i>m</i>&#183;2<sup><i>e</i>-1075</sup>.
        ///
        /// <para/>Note that this method may not be able to return a
        /// <see cref="double"/> NaN with exactly same bit pattern as the
        /// <see cref="long"/> argument.  IEEE 754 distinguishes between two
        /// kinds of NaNs, quiet NaNs and <i>signaling NaNs</i>.  The
        /// differences between the two kinds of NaN are generally not
        /// visible in .NET.  Arithmetic operations on signaling NaNs turn
        /// them into quiet NaNs with a different, but often similar, bit
        /// pattern.  However, on some processors merely copying a
        /// signaling NaN also performs that conversion.  In particular,
        /// copying a signaling NaN to return it to the calling method
        /// may perform this conversion.  So <see cref="Int64BitsToDouble(long)"/>
        /// may not be able to return a <see cref="double"/> with a
        /// signaling NaN bit pattern.  Consequently, for some
        /// <see cref="long"/> values,
        /// <c>BitConversion.DoubleToRawInt64Bits(BitConversion.Int64BitsToDouble(start))</c> may
        /// <i>not</i> equal <c>start</c>.  Moreover, which
        /// particular bit patterns represent signaling NaNs is platform
        /// dependent; although all NaN bit patterns, quiet or signaling,
        /// must be in the NaN range identified above.
        /// 
        /// <para/>
        /// NOTE: This corresponds to Double.longBitsToDouble() in the JDK
        /// and <see cref="BitConverter.Int64BitsToDouble(long)"/> in .NET.
        /// </summary>
        /// <param name="value">Any <see cref="long"/> integer.</param>
        /// <returns>The <see cref="double"/> floating-point value with
        /// the same bit pattern.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Int64BitsToDouble(long value)
        {
            return BitConverter.Int64BitsToDouble(value);
        }

        /// <summary>
        /// Returns a representation of the specified floating-point value
        /// according to the IEEE 754 floating-point "double
        /// format" bit layout.
        ///
        /// <para/>Bit 63 (the bit that is selected by the mask
        /// <c>0x8000000000000000L</c>) represents the sign of the
        /// floating-point number. Bits
        /// 62-52 (the bits that are selected by the mask
        /// <c>0x7ff0000000000000L</c>) represent the exponent. Bits 51-0
        /// (the bits that are selected by the mask
        /// <c>0x000fffffffffffffL</c>) represent the significand
        /// (sometimes called the mantissa) of the floating-point number.
        ///
        /// <para/>If the argument is positive infinity, the result is
        /// <c>0x7ff0000000000000L</c>.
        ///
        /// <para/>If the argument is negative infinity, the result is
        /// <c>0xfff0000000000000L</c>.
        ///
        /// <para/>If the argument is NaN, the result is
        /// <c>0x7ff8000000000000L</c>.
        ///
        /// <para/>In all cases, the result is a <see cref="long"/> integer that, when
        /// given to the <see cref="Int64BitsToDouble(long)"/> method, will produce a
        /// floating-point value the same as the argument to
        /// <see cref="DoubleToInt64Bits(double)"/> (except all NaN values are
        /// collapsed to a single "canonical" NaN value).
        /// 
        /// <para/>
        /// NOTE: This corresponds to Double.doubleToLongBits() in the JDK.
        /// There is no corresponding method in .NET.
        /// </summary>
        /// <param name="value">A <see cref="double"/> precision floating-point number.</param>
        /// <returns>The bits that represent the floating-point number.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long DoubleToInt64Bits(double value)
        {
            if (double.IsNaN(value))
                return 0x7ff8000000000000L;

            return BitConverter.DoubleToInt64Bits(value);
        }

        /// <summary>
        /// Returns a representation of the specified floating-point value
        /// according to the IEEE 754 floating-point "double
        /// format" bit layout, preserving Not-a-Number (NaN) values.
        ///
        /// <para/>Bit 63 (the bit that is selected by the mask
        /// <c>0x8000000000000000L</c>) represents the sign of the
        /// floating-point number. Bits
        /// 62-52 (the bits that are selected by the mask
        /// <c>0x7ff0000000000000L</c>) represent the exponent. Bits 51-0
        /// (the bits that are selected by the mask
        /// <c>0x000fffffffffffffL</c>) represent the significand
        /// (sometimes called the mantissa) of the floating-point number.
        ///
        /// <para/>If the argument is positive infinity, the result is
        /// <c>0x7ff0000000000000L</c>.
        ///
        /// <para/>If the argument is negative infinity, the result is
        /// <c>0xfff0000000000000L</c>.
        ///
        /// <para/>If the argument is NaN, the result is the <see cref="long"/>
        /// integer representing the actual NaN value.  Unlike the
        /// <see cref="DoubleToInt64Bits(double)"/> method,
        /// <see cref="DoubleToRawInt64Bits(double)"/> does not collapse all the bit
        /// patterns encoding a NaN to a single "canonical" NaN
        /// value.
        ///
        /// <para/>In all cases, the result is a <see cref="long"/> integer that,
        /// when given to the <see cref="Int64BitsToDouble(long)"/> method, will
        /// produce a floating-point value the same as the argument to
        /// <see cref="DoubleToRawInt64Bits(double)"/>.
        /// 
        /// <para/>
        /// NOTE: This corresponds to Double.doubleToRawLongBits() in the JDK
        /// and to <see cref="BitConverter.DoubleToInt64Bits(double)"/> in .NET.
        /// </summary>
        /// <param name="value">A <see cref="double"/> precision floating-point number.</param>
        /// <returns>The bits that represent the floating-point number.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long DoubleToRawInt64Bits(double value)
        {
            return BitConverter.DoubleToInt64Bits(value);
        }
    }
}
