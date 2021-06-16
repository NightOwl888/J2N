/*
 * Copyright (c) 1996, 2013, Oracle and/or its affiliates. All rights reserved.
 * DO NOT ALTER OR REMOVE COPYRIGHT NOTICES OR THIS FILE HEADER.
 *
 * This code is free software; you can redistribute it and/or modify it
 * under the terms of the GNU General Public License version 2 only, as
 * published by the Free Software Foundation.  Oracle designates this
 * particular file as subject to the "Classpath" exception as provided
 * by Oracle in the LICENSE file that accompanied this code.
 *
 * This code is distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License
 * version 2 for more details (a copy is included in the LICENSE file that
 * accompanied this code).
 *
 * You should have received a copy of the GNU General Public License version
 * 2 along with this work; if not, write to the Free Software Foundation,
 * Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 *
 * Please contact Oracle, 500 Oracle Parkway, Redwood Shores, CA 94065 USA
 * or visit www.oracle.com if you need additional information or have any
 * questions.
 */

using J2N.Text;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading;

namespace J2N.Numerics
{
    /// <summary>
    /// A class for converting between ASCII and decimal representations of a single
    /// or double precision floating point number. Most conversions are provided via
    /// static convenience methods, although a <code>IBinaryToASCIIConverter</code>
    /// instance may be obtained and reused.
    /// </summary>
    public class FloatingDecimal
    {

        /**
         * This class contains additional constants documenting limits of the
         * <code>double</code> type.
         *
         * @author Joseph D. Darcy
         */

        internal static class DoubleConsts
        {
            //public static final double POSITIVE_INFINITY = java.lang.Double.POSITIVE_INFINITY;
            //public static final double NEGATIVE_INFINITY = java.lang.Double.NEGATIVE_INFINITY;
            //public static final double NaN = java.lang.Double.NaN;
            //public static final double MAX_VALUE = java.lang.Double.MAX_VALUE;
            //public static final double MIN_VALUE = java.lang.Double.MIN_VALUE;

            /**
             * A constant holding the smallest positive normal value of type
             * <code>double</code>, 2<sup>-1022</sup>.  It is equal to the
             * value returned by
             * <code>Double.longBitsToDouble(0x0010000000000000L)</code>.
             *
             * @since 1.5
             */
            public const double MIN_NORMAL = 2.2250738585072014E-308;


            /**
             * The number of logical bits in the significand of a
             * <code>double</code> number, including the implicit bit.
             */
            public const int SIGNIFICAND_WIDTH = 53;

            /**
             * Maximum exponent a finite <code>double</code> number may have.
             * It is equal to the value returned by
             * <code>Math.ilogb(Double.MAX_VALUE)</code>.
             */
            public const int MAX_EXPONENT = 1023;

            /**
             * Minimum exponent a normalized <code>double</code> number may
             * have.  It is equal to the value returned by
             * <code>Math.ilogb(Double.MIN_NORMAL)</code>.
             */
            public const int MIN_EXPONENT = -1022;

            /**
             * The exponent the smallest positive <code>double</code>
             * subnormal value would have if it could be normalized.  It is
             * equal to the value returned by
             * <code>FpUtils.ilogb(Double.MIN_VALUE)</code>.
             */
            public const int MIN_SUB_EXPONENT = MIN_EXPONENT -
                                                           (SIGNIFICAND_WIDTH - 1);

            /**
             * Bias used in representing a <code>double</code> exponent.
             */
            public const int EXP_BIAS = 1023;

            /**
             * Bit mask to isolate the sign bit of a <code>double</code>.
             */
            public const long SIGN_BIT_MASK = unchecked((long)0x8000000000000000L);

            /**
             * Bit mask to isolate the exponent field of a
             * <code>double</code>.
             */
            public const long EXP_BIT_MASK = 0x7FF0000000000000L;

            /**
             * Bit mask to isolate the significand field of a
             * <code>double</code>.
             */
            public const long SIGNIF_BIT_MASK = 0x000FFFFFFFFFFFFFL;

            static DoubleConsts()
            {
                // verify bit masks cover all bit positions and that the bit
                // masks are non-overlapping
                Debug.Assert(((SIGN_BIT_MASK | EXP_BIT_MASK | SIGNIF_BIT_MASK) == ~0L) &&
                       (((SIGN_BIT_MASK & EXP_BIT_MASK) == 0L) &&
                        ((SIGN_BIT_MASK & SIGNIF_BIT_MASK) == 0L) &&
                        ((EXP_BIT_MASK & SIGNIF_BIT_MASK) == 0L)));
            }
        }




        /**
    * This class contains additional constants documenting limits of the
    * <code>float</code> type.
    *
    * @author Joseph D. Darcy
*/

        internal static class FloatConsts
        {
            //public static final float POSITIVE_INFINITY = java.lang.Float.POSITIVE_INFINITY;
            //public static final float NEGATIVE_INFINITY = java.lang.Float.NEGATIVE_INFINITY;
            //public static final float NaN = java.lang.Float.NaN;
            //public static final float MAX_VALUE = java.lang.Float.MAX_VALUE;
            //public static final float MIN_VALUE = java.lang.Float.MIN_VALUE;

            /**
             * A constant holding the smallest positive normal value of type
             * <code>float</code>, 2<sup>-126</sup>.  It is equal to the value
             * returned by <code>Float.intBitsToFloat(0x00800000)</code>.
             */
            public const float MIN_NORMAL = 1.17549435E-38f;

            /**
             * The number of logical bits in the significand of a
             * <code>float</code> number, including the implicit bit.
             */
            public const int SIGNIFICAND_WIDTH = 24;

            /**
             * Maximum exponent a finite <code>float</code> number may have.
             * It is equal to the value returned by
             * <code>Math.ilogb(Float.MAX_VALUE)</code>.
             */
            public const int MAX_EXPONENT = 127;

            /**
             * Minimum exponent a normalized <code>float</code> number may
             * have.  It is equal to the value returned by
             * <code>Math.ilogb(Float.MIN_NORMAL)</code>.
             */
            public const int MIN_EXPONENT = -126;

            /**
             * The exponent the smallest positive <code>float</code> subnormal
             * value would have if it could be normalized.  It is equal to the
             * value returned by <code>FpUtils.ilogb(Float.MIN_VALUE)</code>.
             */
            public const int MIN_SUB_EXPONENT = MIN_EXPONENT -
                                                           (SIGNIFICAND_WIDTH - 1);

            /**
             * Bias used in representing a <code>float</code> exponent.
             */
            public const int EXP_BIAS = 127;

            /**
             * Bit mask to isolate the sign bit of a <code>float</code>.
             */
            public const int SIGN_BIT_MASK = unchecked((int)0x80000000);

            /**
             * Bit mask to isolate the exponent field of a
             * <code>float</code>.
             */
            public const int EXP_BIT_MASK = 0x7F800000;

            /**
             * Bit mask to isolate the significand field of a
             * <code>float</code>.
             */
            public const int SIGNIF_BIT_MASK = 0x007FFFFF;

            static FloatConsts()
            {
                // verify bit masks cover all bit positions and that the bit
                // masks are non-overlapping
                Debug.Assert(((SIGN_BIT_MASK | EXP_BIT_MASK | SIGNIF_BIT_MASK) == ~0) &&
                       (((SIGN_BIT_MASK & EXP_BIT_MASK) == 0) &&
                        ((SIGN_BIT_MASK & SIGNIF_BIT_MASK) == 0) &&
                        ((EXP_BIT_MASK & SIGNIF_BIT_MASK) == 0)));
            }
        }


        


        //
        // Constants of the implementation;
        // most are IEEE-754 related.
        // (There are more really boring constants at the end.)
        //
        private const int EXP_SHIFT = DoubleConsts.SIGNIFICAND_WIDTH - 1;
        private const long FRACT_HOB = (1L << EXP_SHIFT); // assumed High-Order bit
        private const long EXP_ONE = ((long)DoubleConsts.EXP_BIAS) << EXP_SHIFT; // exponent of 1.0
        private const int MAX_SMALL_BIN_EXP = 62;
        private const int MIN_SMALL_BIN_EXP = -(63 / 3);
        private const int MAX_DECIMAL_DIGITS = 15;
        private const int MAX_DECIMAL_EXPONENT = 308;
        private const int MIN_DECIMAL_EXPONENT = -324;
        private const int BIG_DECIMAL_EXPONENT = 324; // i.e. abs(MIN_DECIMAL_EXPONENT)
        private const int MAX_NDIGITS = 1100;

        private const int SINGLE_EXP_SHIFT = FloatConsts.SIGNIFICAND_WIDTH - 1;
        private const int SINGLE_FRACT_HOB = 1 << SINGLE_EXP_SHIFT;
        private const int SINGLE_MAX_DECIMAL_DIGITS = 7;
        private const int SINGLE_MAX_DECIMAL_EXPONENT = 38;
        private const int SINGLE_MIN_DECIMAL_EXPONENT = -45;
        private const int SINGLE_MAX_NDIGITS = 200;

        private const int INT_DECIMAL_DIGITS = 9;

        /**
         * Converts a double precision floating point value to a <code>String</code>.
         *
         * @param d The double precision value.
         * @return The value converted to a <code>String</code>.
         */
        public static string ToJavaFormatString(double d)
        {
            return GetBinaryToASCIIConverter(d).ToJavaFormatString();
        }

        /**
         * Converts a single precision floating point value to a <code>String</code>.
         *
         * @param f The single precision value.
         * @return The value converted to a <code>String</code>.
         */
        public static string ToJavaFormatString(float f)
        {
            return GetBinaryToASCIIConverter(f).ToJavaFormatString();
        }

        /**
         * Appends a double precision floating point value to an <code>Appendable</code>.
         * @param d The double precision value.
         * @param buf The <code>Appendable</code> with the value appended.
         */
        public static void AppendTo(double d, IAppendable buf)
        {
            GetBinaryToASCIIConverter(d).AppendTo(buf);
        }

        /**
         * Appends a single precision floating point value to an <code>Appendable</code>.
         * @param f The single precision value.
         * @param buf The <code>Appendable</code> with the value appended.
         */
        public static void AppendTo(float f, IAppendable buf)
        {
            GetBinaryToASCIIConverter(f).AppendTo(buf);
        }

        /**
         * Converts a <code>String</code> to a double precision floating point value.
         *
         * @param s The <code>String</code> to convert.
         * @return The double precision value.
         * @throws NumberFormatException If the <code>String</code> does not
         * represent a properly formatted double precision value.
         */
        public static double ParseDouble(string s)
        {
            return ReadJavaFormatString(s).ToDouble();
        }

        /**
         * Converts a <code>String</code> to a single precision floating point value.
         *
         * @param s The <code>String</code> to convert.
         * @return The single precision value.
         * @throws NumberFormatException If the <code>String</code> does not
         * represent a properly formatted single precision value.
         */
        public static float ParseFloat(string s)
        {
            return ReadJavaFormatString(s).ToSingle();
        }

        /**
         * A converter which can process single or double precision floating point
         * values into an ASCII <code>String</code> representation.
         */
        public interface IBinaryToASCIIConverter
        {
            /**
             * Converts a floating point value into an ASCII <code>String</code>.
             * @return The value converted to a <code>String</code>.
             */
            string ToJavaFormatString();

            /**
             * Appends a floating point value to an <code>Appendable</code>.
             * @param buf The <code>Appendable</code> to receive the value.
             */
            void AppendTo(IAppendable buf);

            /**
             * Retrieves the decimal exponent most closely corresponding to this value.
             * @return The decimal exponent.
             */
            int GetDecimalExponent { get; }

            /**
             * Retrieves the value as an array of digits.
             * @param digits The digit array.
             * @return The number of valid digits copied into the array.
             */
            int GetDigits(char[] digits);

            /**
             * Indicates the sign of the value.
             * @return <code>value < 0.0</code>.
             */
            bool IsNegative { get; }

            /**
             * Indicates whether the value is either infinite or not a number.
             *
             * @return <code>true</code> if and only if the value is <code>NaN</code>
             * or infinite.
             */
            bool IsExceptional { get; }

            /**
             * Indicates whether the value was rounded up during the binary to ASCII
             * conversion.
             *
             * @return <code>true</code> if and only if the value was rounded up.
             */
            bool DigitsRoundedUp { get; }

            /**
             * Indicates whether the binary to ASCII conversion was exact.
             *
             * @return <code>true</code> if any only if the conversion was exact.
             */
            bool DecimalDigitsExact { get; }
        }

        /**
         * A <code>IBinaryToASCIIConverter</code> which represents <code>NaN</code>
         * and infinite values.
         */
        private class ExceptionalBinaryToASCIIBuffer : IBinaryToASCIIConverter
        {
            private readonly string image;
            private bool isNegative;

            public ExceptionalBinaryToASCIIBuffer(string image, bool isNegative)
            {
                this.image = image;
                this.isNegative = isNegative;
            }

            public virtual string ToJavaFormatString()
            {
                return image;
            }

            public virtual void AppendTo(IAppendable buf)
            {
                if (buf is StringBuilderCharSequence stringBuilderCharSequence && stringBuilderCharSequence.HasValue)
                {
                    stringBuilderCharSequence.Value.Append(image);
                }
                else if (buf is StringBuffer stringBuffer)
                {
                    stringBuffer.Append(image);
                }
                else
                {
                    buf.Append(image);
                    //assert false;
                }
            }

            public virtual int GetDecimalExponent => throw new InvalidOperationException("Exceptional value does not have an exponent");

            public virtual int GetDigits(char[] digits)
            {
                throw new InvalidOperationException("Exceptional value does not have digits");
            }

            public virtual bool IsNegative => isNegative;


            public virtual bool IsExceptional => true;


            public virtual bool DigitsRoundedUp => throw new InvalidOperationException("Exceptional value is not rounded");

            public virtual bool DecimalDigitsExact => throw new InvalidOperationException("Exceptional value is not exact");
        }

        private const string INFINITY_REP = "Infinity";
        private static readonly int INFINITY_LENGTH = INFINITY_REP.Length;
        private const string NAN_REP = "NaN";
        private static readonly int NAN_LENGTH = NAN_REP.Length;

        private static readonly IBinaryToASCIIConverter B2AC_POSITIVE_INFINITY = new ExceptionalBinaryToASCIIBuffer(INFINITY_REP, false);
        private static readonly IBinaryToASCIIConverter B2AC_NEGATIVE_INFINITY = new ExceptionalBinaryToASCIIBuffer("-" + INFINITY_REP, true);
        private static readonly IBinaryToASCIIConverter B2AC_NOT_A_NUMBER = new ExceptionalBinaryToASCIIBuffer(NAN_REP, false);
        private static readonly IBinaryToASCIIConverter B2AC_POSITIVE_ZERO = new BinaryToASCIIBuffer(false, new char[] { '0' });
        private static readonly IBinaryToASCIIConverter B2AC_NEGATIVE_ZERO = new BinaryToASCIIBuffer(true, new char[] { '0' });

        /**
         * A buffered implementation of <code>IBinaryToASCIIConverter</code>.
         */
        internal class BinaryToASCIIBuffer : IBinaryToASCIIConverter
        {
            private bool isNegative;
            private int decExponent;
            private int firstDigitIndex;
            private int nDigits;
            private readonly char[] digits;
            private readonly char[] buffer = new char[26];

            //
            // The fields below provide additional information about the result of
            // the binary to decimal digits conversion done in dtoa() and roundup()
            // methods. They are changed if needed by those two methods.
            //

            // True if the dtoa() binary to decimal conversion was exact.
            private bool exactDecimalConversion = false;

            // True if the result of the binary to decimal conversion was rounded-up
            // at the end of the conversion process, i.e. roundUp() method was called.
            private bool decimalDigitsRoundedUp = false;

            /**
             * Default constructor; used for non-zero values,
             * <code>BinaryToASCIIBuffer</code> may be thread-local and reused
             */
            internal BinaryToASCIIBuffer()
            {
                this.digits = new char[20];
            }

            /**
             * Creates a specialized value (positive and negative zeros).
             */
            internal BinaryToASCIIBuffer(bool isNegative, char[] digits)
            {
                this.isNegative = isNegative;
                this.decExponent = 0;
                this.digits = digits;
                this.firstDigitIndex = 0;
                this.nDigits = digits.Length;
            }

            public virtual string ToJavaFormatString()
            {
                int len = GetChars(buffer);
                return new string(buffer, 0, len);
            }

            public virtual void AppendTo(IAppendable buf)
            {
                int len = GetChars(buffer);
                if (buf is StringBuilderCharSequence stringBuilderCharSequence && stringBuilderCharSequence.HasValue)
                {
                    stringBuilderCharSequence.Value.Append(buffer, 0, len);
                }
                else if (buf is StringBuffer stringBuffer)
                {
                    stringBuffer.Append(buffer, 0, len);
                }
                else
                {
                    buf.Append(buffer, 0, len);
                    //assert false;
                }
            }

            public virtual int GetDecimalExponent => decExponent;

            public virtual int GetDigits(char[] digits)
            {
                System.Array.Copy(this.digits, firstDigitIndex, digits, 0, this.nDigits);
                return this.nDigits;
            }

            public virtual bool IsNegative => isNegative;


            public virtual bool IsExceptional => false;


            public bool DigitsRoundedUp => decimalDigitsRoundedUp;


            public bool DecimalDigitsExact => exactDecimalConversion;


            internal void SetSign(bool isNegative) // TODO: Make IsNegative setter
            {
                this.isNegative = isNegative;
            }

            /**
             * This is the easy subcase --
             * all the significant bits, after scaling, are held in lvalue.
             * negSign and decExponent tell us what processing and scaling
             * has already been done. Exceptional cases have already been
             * stripped out.
             * In particular:
             * lvalue is a finite number (not Inf, nor NaN)
             * lvalue > 0L (not zero, nor negative).
             *
             * The only reason that we develop the digits here, rather than
             * calling on Long.toString() is that we can do it a little faster,
             * and besides want to treat trailing 0s specially. If Long.toString
             * changes, we should re-evaluate this strategy!
             */
            private void DevelopLongDigits(int decExponent, long lvalue, int insignificantDigits)
            {
                if (insignificantDigits != 0)
                {
                    // Discard non-significant low-order bits, while rounding,
                    // up to insignificant value.
                    long pow10 = FDBigInteger.LONG_5_POW[insignificantDigits] << insignificantDigits; // 10^i == 5^i * 2^i;
                    long residue = lvalue % pow10;
                    lvalue /= pow10;
                    decExponent += insignificantDigits;
                    if (residue >= (pow10 >> 1))
                    {
                        // round up based on the low-order bits we're discarding
                        lvalue++;
                    }
                }
                int digitno = digits.Length - 1;
                int c;
                if (lvalue <= int.MaxValue)
                {
                    Debug.Assert(lvalue > 0L, lvalue.ToString()); // lvalue <= 0
                                                                  // even easier subcase!
                                                                  // can do int arithmetic rather than long!
                    int ivalue = (int)lvalue;
                    c = ivalue % 10;
                    ivalue /= 10;
                    while (c == 0)
                    {
                        decExponent++;
                        c = ivalue % 10;
                        ivalue /= 10;
                    }
                    while (ivalue != 0)
                    {
                        digits[digitno--] = (char)(c + '0');
                        decExponent++;
                        c = ivalue % 10;
                        ivalue /= 10;
                    }
                    digits[digitno] = (char)(c + '0');
                }
                else
                {
                    // same algorithm as above (same bugs, too )
                    // but using long arithmetic.
                    c = (int)(lvalue % 10L);
                    lvalue /= 10L;
                    while (c == 0)
                    {
                        decExponent++;
                        c = (int)(lvalue % 10L);
                        lvalue /= 10L;
                    }
                    while (lvalue != 0L)
                    {
                        digits[digitno--] = (char)(c + '0');
                        decExponent++;
                        c = (int)(lvalue % 10L);
                        lvalue /= 10;
                    }
                    digits[digitno] = (char)(c + '0');
                }
                this.decExponent = decExponent + 1;
                this.firstDigitIndex = digitno;
                this.nDigits = this.digits.Length - digitno;
            }

            internal void Dtoa(int binExp, long fractBits, int nSignificantBits, bool isCompatibleFormat)
            {
                Debug.Assert(fractBits > 0); // fractBits here can't be zero or negative
                Debug.Assert((fractBits & FRACT_HOB) != 0); // Hi-order bit should be set
                                                            // Examine number. Determine if it is an easy case,
                                                            // which we can do pretty trivially using float/long conversion,
                                                            // or whether we must do real work.
                int tailZeros = fractBits.TrailingZeroCount();

                // number of significant bits of fractBits;
                int nFractBits = EXP_SHIFT + 1 - tailZeros;

                // reset flags to default values as dtoa() does not always set these
                // flags and a prior call to dtoa() might have set them to incorrect
                // values with respect to the current state.
                decimalDigitsRoundedUp = false;
                exactDecimalConversion = false;

                // number of significant bits to the right of the point.
                int nTinyBits = Math.Max(0, nFractBits - binExp - 1);
                if (binExp <= MAX_SMALL_BIN_EXP && binExp >= MIN_SMALL_BIN_EXP)
                {
                    // Look more closely at the number to decide if,
                    // with scaling by 10^nTinyBits, the result will fit in
                    // a long.
                    if ((nTinyBits < FDBigInteger.LONG_5_POW.Length) && ((nFractBits + N_5_BITS[nTinyBits]) < 64))
                    {
                        //
                        // We can do this:
                        // take the fraction bits, which are normalized.
                        // (a) nTinyBits == 0: Shift left or right appropriately
                        //     to align the binary point at the extreme right, i.e.
                        //     where a long int point is expected to be. The integer
                        //     result is easily converted to a string.
                        // (b) nTinyBits > 0: Shift right by EXP_SHIFT-nFractBits,
                        //     which effectively converts to long and scales by
                        //     2^nTinyBits. Then multiply by 5^nTinyBits to
                        //     complete the scaling. We know this won't overflow
                        //     because we just counted the number of bits necessary
                        //     in the result. The integer you get from this can
                        //     then be converted to a string pretty easily.
                        //
                        if (nTinyBits == 0)
                        {
                            int insignificant;
                            if (binExp > nSignificantBits)
                            {
                                insignificant = InsignificantDigitsForPow2(binExp - nSignificantBits - 1);
                            }
                            else
                            {
                                insignificant = 0;
                            }
                            if (binExp >= EXP_SHIFT)
                            {
                                fractBits <<= (binExp - EXP_SHIFT);
                            }
                            else
                            {
                                //fractBits >>>= (EXP_SHIFT - binExp);
                                fractBits = fractBits.TripleShift(EXP_SHIFT - binExp);
                            }
                            DevelopLongDigits(0, fractBits, insignificant);
                            return;
                        }
                        //
                        // The following causes excess digits to be printed
                        // out in the single-float case. Our manipulation of
                        // halfULP here is apparently not correct. If we
                        // better understand how this works, perhaps we can
                        // use this special case again. But for the time being,
                        // we do not.
                        // else {
                        //     fractBits >>>= EXP_SHIFT+1-nFractBits;
                        //     fractBits//= long5pow[ nTinyBits ];
                        //     halfULP = long5pow[ nTinyBits ] >> (1+nSignificantBits-nFractBits);
                        //     developLongDigits( -nTinyBits, fractBits, insignificantDigits(halfULP) );
                        //     return;
                        // }
                        //
                    }
                }
                //
                // This is the hard case. We are going to compute large positive
                // integers B and S and integer decExp, s.t.
                //      d = ( B / S )// 10^decExp
                //      1 <= B / S < 10
                // Obvious choices are:
                //      decExp = floor( log10(d) )
                //      B      = d// 2^nTinyBits// 10^max( 0, -decExp )
                //      S      = 10^max( 0, decExp)// 2^nTinyBits
                // (noting that nTinyBits has already been forced to non-negative)
                // I am also going to compute a large positive integer
                //      M      = (1/2^nSignificantBits)// 2^nTinyBits// 10^max( 0, -decExp )
                // i.e. M is (1/2) of the ULP of d, scaled like B.
                // When we iterate through dividing B/S and picking off the
                // quotient bits, we will know when to stop when the remainder
                // is <= M.
                //
                // We keep track of powers of 2 and powers of 5.
                //
                int decExp = estimateDecExp(fractBits, binExp);
                int B2, B5; // powers of 2 and powers of 5, respectively, in B
                int S2, S5; // powers of 2 and powers of 5, respectively, in S
                int M2, M5; // powers of 2 and powers of 5, respectively, in M

                B5 = Math.Max(0, -decExp);
                B2 = B5 + nTinyBits + binExp;

                S5 = Math.Max(0, decExp);
                S2 = S5 + nTinyBits;

                M5 = B5;
                M2 = B2 - nSignificantBits;

                //
                // the long integer fractBits contains the (nFractBits) interesting
                // bits from the mantissa of d ( hidden 1 added if necessary) followed
                // by (EXP_SHIFT+1-nFractBits) zeros. In the interest of compactness,
                // I will shift out those zeros before turning fractBits into a
                // FDBigInteger. The resulting whole number will be
                //      d * 2^(nFractBits-1-binExp).
                //
                //fractBits >>>= tailZeros;
                fractBits = fractBits.TripleShift(tailZeros);
                B2 -= nFractBits - 1;
                int common2factor = Math.Min(B2, S2);
                B2 -= common2factor;
                S2 -= common2factor;
                M2 -= common2factor;

                //
                // HACK!! For exact powers of two, the next smallest number
                // is only half as far away as we think (because the meaning of
                // ULP changes at power-of-two bounds) for this reason, we
                // hack M2. Hope this works.
                //
                if (nFractBits == 1)
                {
                    M2 -= 1;
                }

                if (M2 < 0)
                {
                    // oops.
                    // since we cannot scale M down far enough,
                    // we must scale the other values up.
                    B2 -= M2;
                    S2 -= M2;
                    M2 = 0;
                }
                //
                // Construct, Scale, iterate.
                // Some day, we'll write a stopping test that takes
                // account of the asymmetry of the spacing of floating-point
                // numbers below perfect powers of 2
                // 26 Sept 96 is not that day.
                // So we use a symmetric test.
                //
                int ndigit = 0;
                bool low, high;
                long lowDigitDifference;
                int q;

                //
                // Detect the special cases where all the numbers we are about
                // to compute will fit in int or long integers.
                // In these cases, we will avoid doing FDBigInteger arithmetic.
                // We use the same algorithms, except that we "normalize"
                // our FDBigIntegers before iterating. This is to make division easier,
                // as it makes our fist guess (quotient of high-order words)
                // more accurate!
                //
                // Some day, we'll write a stopping test that takes
                // account of the asymmetry of the spacing of floating-point
                // numbers below perfect powers of 2
                // 26 Sept 96 is not that day.
                // So we use a symmetric test.
                //
                // binary digits needed to represent B, approx.
                int Bbits = nFractBits + B2 + ((B5 < N_5_BITS.Length) ? N_5_BITS[B5] : (B5 * 3));

                // binary digits needed to represent 10*S, approx.
                int tenSbits = S2 + 1 + (((S5 + 1) < N_5_BITS.Length) ? N_5_BITS[(S5 + 1)] : ((S5 + 1) * 3));
                if (Bbits < 64 && tenSbits < 64)
                {
                    if (Bbits < 32 && tenSbits < 32)
                    {
                        // wa-hoo! They're all ints!
                        int b = ((int)fractBits * FDBigInteger.SMALL_5_POW[B5]) << B2;
                        int s = FDBigInteger.SMALL_5_POW[S5] << S2;
                        int m = FDBigInteger.SMALL_5_POW[M5] << M2;
                        int tens = s * 10;
                        //
                        // Unroll the first iteration. If our decExp estimate
                        // was too high, our first quotient will be zero. In this
                        // case, we discard it and decrement decExp.
                        //
                        ndigit = 0;
                        q = b / s;
                        b = 10 * (b % s);
                        m *= 10;
                        low = (b < m);
                        high = (b + m > tens);
                        Debug.Assert(q < 10, q.ToString()); // excessively large digit
                        if ((q == 0) && !high)
                        {
                            // oops. Usually ignore leading zero.
                            decExp--;
                        }
                        else
                        {
                            digits[ndigit++] = (char)('0' + q);
                        }
                        //
                        // HACK! Java spec sez that we always have at least
                        // one digit after the . in either F- or E-form output.
                        // Thus we will need more than one digit if we're using
                        // E-form
                        //
                        if (!isCompatibleFormat || decExp < -3 || decExp >= 8)
                        {
                            high = low = false;
                        }
                        while (!low && !high)
                        {
                            q = b / s;
                            b = 10 * (b % s);
                            m *= 10;
                            Debug.Assert(q < 10, q.ToString()); // excessively large digit
                            if (m > 0L)
                            {
                                low = (b < m);
                                high = (b + m > tens);
                            }
                            else
                            {
                                // hack -- m might overflow!
                                // in this case, it is certainly > b,
                                // which won't
                                // and b+m > tens, too, since that has overflowed
                                // either!
                                low = true;
                                high = true;
                            }
                            digits[ndigit++] = (char)('0' + q);
                        }
                        lowDigitDifference = (b << 1) - tens;
                        exactDecimalConversion = (b == 0);
                    }
                    else
                    {

                        // still good! they're all longs!
                        long b = (fractBits * FDBigInteger.LONG_5_POW[B5]) << B2;
                        long s = FDBigInteger.LONG_5_POW[S5] << S2;
                        long m = FDBigInteger.LONG_5_POW[M5] << M2;
                        long tens = s * 10L;
                        //
                        // Unroll the first iteration. If our decExp estimate
                        // was too high, our first quotient will be zero. In this
                        // case, we discard it and decrement decExp.
                        //
                        ndigit = 0;
                        q = (int)(b / s);
                        b = 10L * (b % s);
                        m *= 10L;
                        low = (b < m);
                        high = (b + m > tens);
                        Debug.Assert(q < 10, q.ToString()); // excessively large digit
                        if ((q == 0) && !high)
                        {
                            // oops. Usually ignore leading zero.
                            decExp--;
                        }
                        else
                        {
                            digits[ndigit++] = (char)('0' + q);
                        }
                        //
                        // HACK! Java spec sez that we always have at least
                        // one digit after the . in either F- or E-form output.
                        // Thus we will need more than one digit if we're using
                        // E-form
                        //
                        if (!isCompatibleFormat || decExp < -3 || decExp >= 8)
                        {
                            high = low = false;
                        }
                        while (!low && !high)
                        {
                            q = (int)(b / s);
                            b = 10 * (b % s);
                            m *= 10;
                            Debug.Assert(q < 10, q.ToString());  // excessively large digit
                            if (m > 0L)
                            {
                                low = (b < m);
                                high = (b + m > tens);
                            }
                            else
                            {
                                // hack -- m might overflow!
                                // in this case, it is certainly > b,
                                // which won't
                                // and b+m > tens, too, since that has overflowed
                                // either!
                                low = true;
                                high = true;
                            }
                            digits[ndigit++] = (char)('0' + q);
                        }
                        lowDigitDifference = (b << 1) - tens;
                        exactDecimalConversion = (b == 0);
                    }
                }
                else
                {
                    //
                    // We really must do FDBigInteger arithmetic.
                    // Fist, construct our FDBigInteger initial values.
                    //
                    FDBigInteger Sval = FDBigInteger.ValueOfPow52(S5, S2);
                    int shiftBias = Sval.GetNormalizationBias();
                    Sval = Sval.LeftShift(shiftBias); // normalize so that division works better

                    FDBigInteger Bval = FDBigInteger.ValueOfMulPow52(fractBits, B5, B2 + shiftBias);
                    FDBigInteger Mval = FDBigInteger.ValueOfPow52(M5 + 1, M2 + shiftBias + 1);

                    FDBigInteger tenSval = FDBigInteger.ValueOfPow52(S5 + 1, S2 + shiftBias + 1); //Sval.mult( 10 );
                                                                                                  //
                                                                                                  // Unroll the first iteration. If our decExp estimate
                                                                                                  // was too high, our first quotient will be zero. In this
                                                                                                  // case, we discard it and decrement decExp.
                                                                                                  //
                    ndigit = 0;
                    q = Bval.QuoRemIteration(Sval);
                    low = (Bval.Cmp(Mval) < 0);
                    high = tenSval.AddAndCmp(Bval, Mval) <= 0;

                    Debug.Assert(q < 10, q.ToString()); // excessively large digit
                    if ((q == 0) && !high)
                    {
                        // oops. Usually ignore leading zero.
                        decExp--;
                    }
                    else
                    {
                        digits[ndigit++] = (char)('0' + q);
                    }
                    //
                    // HACK! Java spec sez that we always have at least
                    // one digit after the . in either F- or E-form output.
                    // Thus we will need more than one digit if we're using
                    // E-form
                    //
                    if (!isCompatibleFormat || decExp < -3 || decExp >= 8)
                    {
                        high = low = false;
                    }
                    while (!low && !high)
                    {
                        q = Bval.QuoRemIteration(Sval);
                        Debug.Assert(q < 10, q.ToString());  // excessively large digit
                        Mval = Mval.MultBy10(); //Mval = Mval.mult( 10 );
                        low = (Bval.Cmp(Mval) < 0);
                        high = tenSval.AddAndCmp(Bval, Mval) <= 0;
                        digits[ndigit++] = (char)('0' + q);
                    }
                    if (high && low)
                    {
                        Bval = Bval.LeftShift(1);
                        lowDigitDifference = Bval.Cmp(tenSval);
                    }
                    else
                    {
                        lowDigitDifference = 0L; // this here only for flow analysis!
                    }
                    //
                    exactDecimalConversion = (Bval.Cmp(FDBigInteger.ZERO) == 0);
                }
                this.decExponent = decExp + 1;
                this.firstDigitIndex = 0;
                this.nDigits = ndigit;
                //
                // Last digit gets rounded based on stopping condition.
                //
                if (high)
                {
                    if (low)
                    {
                        if (lowDigitDifference == 0L)
                        {
                            // it's a tie!
                            // choose based on which digits we like.
                            if ((digits[firstDigitIndex + nDigits - 1] & 1) != 0)
                            {
                                RoundUp();
                            }
                        }
                        else if (lowDigitDifference > 0)
                        {
                            RoundUp();
                        }
                    }
                    else
                    {
                        RoundUp();
                    }
                }
            }

            // add one to the least significant digit.
            // in the unlikely event there is a carry out, deal with it.
            // assert that this will only happen where there
            // is only one digit, e.g. (float)1e-44 seems to do it.
            //
            private void RoundUp()
            {
                int i = (firstDigitIndex + nDigits - 1);
                int q = digits[i];
                if (q == '9')
                {
                    while (q == '9' && i > firstDigitIndex)
                    {
                        digits[i] = '0';
                        q = digits[--i];
                    }
                    if (q == '9')
                    {
                        // carryout! High-order 1, rest 0s, larger exp.
                        decExponent += 1;
                        digits[firstDigitIndex] = '1';
                        return;
                    }
                    // else fall through.
                }
                digits[i] = (char)(q + 1);
                decimalDigitsRoundedUp = true;
            }

            /**
             * Estimate decimal exponent. (If it is small-ish,
             * we could double-check.)
             *
             * First, scale the mantissa bits such that 1 <= d2 < 2.
             * We are then going to estimate
             *          log10(d2) ~=~  (d2-1.5)/1.5 + log(1.5)
             * and so we can estimate
             *      log10(d) ~=~ log10(d2) + binExp * log10(2)
             * take the floor and call it decExp.
             */
            internal static int estimateDecExp(long fractBits, int binExp)
            {
                double d2 = BitConversion.Int64BitsToDouble(EXP_ONE | (fractBits & DoubleConsts.SIGNIF_BIT_MASK));
                double d = (d2 - 1.5D) * 0.289529654D + 0.176091259 + (double)binExp * 0.301029995663981;
                long dBits = BitConversion.DoubleToRawInt64Bits(d);  //can't be NaN here so use raw
                int exponent = (int)((dBits & DoubleConsts.EXP_BIT_MASK) >> EXP_SHIFT) - DoubleConsts.EXP_BIAS;
                bool isNegative = (dBits & DoubleConsts.SIGN_BIT_MASK) != 0; // discover sign
                if (exponent >= 0 && exponent < 52)
                { // hot path
                    long mask = DoubleConsts.SIGNIF_BIT_MASK >> exponent;
                    int r = (int)(((dBits & DoubleConsts.SIGNIF_BIT_MASK) | FRACT_HOB) >> (EXP_SHIFT - exponent));
                    return isNegative ? (((mask & dBits) == 0L) ? -r : -r - 1) : r;
                }
                else if (exponent < 0)
                {
                    return (((dBits & ~DoubleConsts.SIGN_BIT_MASK) == 0) ? 0 :
                            ((isNegative) ? -1 : 0));
                }
                else
                { //if (exponent >= 52)
                    return (int)d;
                }
            }

            private static int InsignificantDigits(int insignificant)
            {
                int i;
                for (i = 0; insignificant >= 10L; i++)
                {
                    insignificant /= 10; // J2N: Converted to int - not sure whether long makes a difference here
                }
                return i;
            }

            /**
             * Calculates
             * <pre>
             * insignificantDigitsForPow2(v) == insignificantDigits(1L<<v)
             * </pre>
             */
            private static int InsignificantDigitsForPow2(int p2)
            {
                if (p2 > 1 && p2 < insignificantDigitsNumber.Length)
                {
                    return insignificantDigitsNumber[p2];
                }
                return 0;
            }

            /**
             *  If insignificant==(1L << ixd)
             *  i = insignificantDigitsNumber[idx] is the same as:
             *  int i;
             *  for ( i = 0; insignificant >= 10L; i++ )
             *         insignificant /= 10L;
             */
            private static readonly int[] insignificantDigitsNumber = {
            0, 0, 0, 0, 1, 1, 1, 2, 2, 2, 3, 3, 3, 3,
            4, 4, 4, 5, 5, 5, 6, 6, 6, 6, 7, 7, 7,
            8, 8, 8, 9, 9, 9, 9, 10, 10, 10, 11, 11, 11,
            12, 12, 12, 12, 13, 13, 13, 14, 14, 14,
            15, 15, 15, 15, 16, 16, 16, 17, 17, 17,
            18, 18, 18, 19
        };

            // approximately ceil( log2( long5pow[i] ) )
            private static readonly int[] N_5_BITS = {
                0,
                3,
                5,
                7,
                10,
                12,
                14,
                17,
                19,
                21,
                24,
                26,
                28,
                31,
                33,
                35,
                38,
                40,
                42,
                45,
                47,
                49,
                52,
                54,
                56,
                59,
                61,
        };

            private int GetChars(char[] result)
            {
                Debug.Assert(nDigits <= 19, nDigits.ToString()); // generous bound on size of nDigits
                int i = 0;
                if (isNegative)
                {
                    result[0] = '-';
                    i = 1;
                }
                if (decExponent > 0 && decExponent < 8)
                {
                    // print digits.digits.
                    int charLength = Math.Min(nDigits, decExponent);
                    Array.Copy(digits, firstDigitIndex, result, i, charLength);
                    i += charLength;
                    if (charLength < decExponent)
                    {
                        charLength = decExponent - charLength;
                        result.Fill(i, charLength, '0'); // J2N: Corrected length
                        i += charLength;
                        result[i++] = '.';
                        result[i++] = '0';
                    }
                    else
                    {
                        result[i++] = '.';
                        if (charLength < nDigits)
                        {
                            int t = nDigits - charLength;
                            Array.Copy(digits, firstDigitIndex + charLength, result, i, t);
                            i += t;
                        }
                        else
                        {
                            result[i++] = '0';
                        }
                    }
                }
                else if (decExponent <= 0 && decExponent > -3)
                {
                    result[i++] = '0';
                    result[i++] = '.';
                    if (decExponent != 0)
                    {
                        result.Fill(i, (i - decExponent) - i, '0'); // J2N: Corrected length
                        i -= decExponent;
                    }
                    Array.Copy(digits, firstDigitIndex, result, i, nDigits);
                    i += nDigits;
                }
                else
                {
                    result[i++] = digits[firstDigitIndex];
                    result[i++] = '.';
                    if (nDigits > 1)
                    {
                        Array.Copy(digits, firstDigitIndex + 1, result, i, nDigits - 1);
                        i += nDigits - 1;
                    }
                    else
                    {
                        result[i++] = '0';
                    }
                    result[i++] = 'E';
                    int e;
                    if (decExponent <= 0)
                    {
                        result[i++] = '-';
                        e = -decExponent + 1;
                    }
                    else
                    {
                        e = decExponent - 1;
                    }
                    // decExponent has 1, 2, or 3, digits
                    if (e <= 9)
                    {
                        result[i++] = (char)(e + '0');
                    }
                    else if (e <= 99)
                    {
                        result[i++] = (char)(e / 10 + '0');
                        result[i++] = (char)(e % 10 + '0');
                    }
                    else
                    {
                        result[i++] = (char)(e / 100 + '0');
                        e %= 100;
                        result[i++] = (char)(e / 10 + '0');
                        result[i++] = (char)(e % 10 + '0');
                    }
                }
                return i;
            }

        }

        private static readonly ThreadLocal<BinaryToASCIIBuffer> threadLocalBinaryToASCIIBuffer =
                new ThreadLocal<BinaryToASCIIBuffer>(() => new BinaryToASCIIBuffer());

        private static BinaryToASCIIBuffer GetBinaryToASCIIBuffer()
        {
            return threadLocalBinaryToASCIIBuffer.Value;
        }

        /**
         * A converter which can process an ASCII <code>String</code> representation
         * of a single or double precision floating point value into a
         * <code>float</code> or a <code>double</code>.
         */
        internal interface IASCIIToBinaryConverter
        {

            double ToDouble();

            float ToSingle();

        }

        /**
         * A <code>ASCIIToBinaryConverter</code> container for a <code>double</code>.
         */
        internal class PreparedASCIIToBinaryBuffer : IASCIIToBinaryConverter
        {
            private readonly double doubleVal;
            private readonly float floatVal;

            public PreparedASCIIToBinaryBuffer(double doubleVal, float floatVal)
            {
                this.doubleVal = doubleVal;
                this.floatVal = floatVal;
            }

            public virtual double ToDouble()
            {
                return doubleVal;
            }


            public virtual float ToSingle()
            {
                return floatVal;
            }
        }

        private static readonly IASCIIToBinaryConverter A2BC_POSITIVE_INFINITY = new PreparedASCIIToBinaryBuffer(double.PositiveInfinity, float.PositiveInfinity);
        private static readonly IASCIIToBinaryConverter A2BC_NEGATIVE_INFINITY = new PreparedASCIIToBinaryBuffer(double.NegativeInfinity, float.NegativeInfinity);
        private static readonly IASCIIToBinaryConverter A2BC_NOT_A_NUMBER = new PreparedASCIIToBinaryBuffer(double.NaN, float.NaN);
        private static readonly IASCIIToBinaryConverter A2BC_POSITIVE_ZERO = new PreparedASCIIToBinaryBuffer(0.0d, 0.0f);
        private static readonly IASCIIToBinaryConverter A2BC_NEGATIVE_ZERO = new PreparedASCIIToBinaryBuffer(-0.0d, -0.0f);

        /**
         * A buffered implementation of <code>ASCIIToBinaryConverter</code>.
         */
        internal class ASCIIToBinaryBuffer : IASCIIToBinaryConverter
        {
            private bool isNegative;
            private int decExponent;
            private char[] digits;
            private int nDigits;

            internal ASCIIToBinaryBuffer(bool negSign, int decExponent, char[]
            digits, int n)
            {
                this.isNegative = negSign;
                this.decExponent = decExponent;
                this.digits = digits;
                this.nDigits = n;
            }

            /**
             * Takes a FloatingDecimal, which we presumably just scanned in,
             * and finds out what its value is, as a double.
             *
             * AS A SIDE EFFECT, SET roundDir TO INDICATE PREFERRED
             * ROUNDING DIRECTION in case the result is really destined
             * for a single-precision float.
             */
            public virtual double ToDouble()
            {
                int kDigits = Math.Min(nDigits, MAX_DECIMAL_DIGITS + 1);
                //
                // convert the lead kDigits to a long integer.
                //
                // (special performance hack: start to do it using int)
                int iValue = (int)digits[0] - (int)'0';
                int iDigits = Math.Min(kDigits, INT_DECIMAL_DIGITS);
                for (int i = 1; i < iDigits; i++)
                {
                    iValue = iValue * 10 + (int)digits[i] - (int)'0';
                }
                long lValue = (long)iValue;
                for (int i = iDigits; i < kDigits; i++)
                {
                    lValue = lValue * 10L + (long)((int)digits[i] - (int)'0');
                }
                double dValue = (double)lValue;
                int exp = decExponent - kDigits;
                //
                // lValue now contains a long integer with the value of
                // the first kDigits digits of the number.
                // dValue contains the (double) of the same.
                //

                if (nDigits <= MAX_DECIMAL_DIGITS)
                {
                    //
                    // possibly an easy case.
                    // We know that the digits can be represented
                    // exactly. And if the exponent isn't too outrageous,
                    // the whole thing can be done with one operation,
                    // thus one rounding error.
                    // Note that all our constructors trim all leading and
                    // trailing zeros, so simple values (including zero)
                    // will always end up here
                    //
                    if (exp == 0 || dValue == 0.0)
                    {
                        return (isNegative) ? -dValue : dValue; // small floating integer
                    }
                    else if (exp >= 0)
                    {
                        if (exp <= MAX_SMALL_TEN)
                        {
                            //
                            // Can get the answer with one operation,
                            // thus one roundoff.
                            //
                            double rValue = dValue * SMALL_10_POW[exp];
                            return (isNegative) ? -rValue : rValue;
                        }
                        int slop = MAX_DECIMAL_DIGITS - kDigits;
                        if (exp <= MAX_SMALL_TEN + slop)
                        {
                            //
                            // We can multiply dValue by 10^(slop)
                            // and it is still "small" and exact.
                            // Then we can multiply by 10^(exp-slop)
                            // with one rounding.
                            //
                            dValue *= SMALL_10_POW[slop];
                            double rValue = dValue * SMALL_10_POW[exp - slop];
                            return (isNegative) ? -rValue : rValue;
                        }
                        //
                        // Else we have a hard case with a positive exp.
                        //
                    }
                    else
                    {
                        if (exp >= -MAX_SMALL_TEN)
                        {
                            //
                            // Can get the answer in one division.
                            //
                            double rValue = dValue / SMALL_10_POW[-exp];
                            return (isNegative) ? -rValue : rValue;
                        }
                        //
                        // Else we have a hard case with a negative exp.
                        //
                    }
                }

                //
                // Harder cases:
                // The sum of digits plus exponent is greater than
                // what we think we can do with one error.
                //
                // Start by approximating the right answer by,
                // naively, scaling by powers of 10.
                //
                if (exp > 0)
                {
                    if (decExponent > MAX_DECIMAL_EXPONENT + 1)
                    {
                        //
                        // Lets face it. This is going to be
                        // Infinity. Cut to the chase.
                        //
                        return (isNegative) ? double.NegativeInfinity : double.PositiveInfinity;
                    }
                    if ((exp & 15) != 0)
                    {
                        dValue *= SMALL_10_POW[exp & 15];
                    }
                    if ((exp >>= 4) != 0)
                    {
                        int j;
                        for (j = 0; exp > 1; j++, exp >>= 1)
                        {
                            if ((exp & 1) != 0)
                            {
                                dValue *= BIG_10_POW[j];
                            }
                        }
                        //
                        // The reason for the weird exp > 1 condition
                        // in the above loop was so that the last multiply
                        // would get unrolled. We handle it here.
                        // It could overflow.
                        //
                        double t = dValue * BIG_10_POW[j];
                        if (double.IsInfinity(t))
                        {
                            //
                            // It did overflow.
                            // Look more closely at the result.
                            // If the exponent is just one too large,
                            // then use the maximum finite as our estimate
                            // value. Else call the result infinity
                            // and punt it.
                            // ( I presume this could happen because
                            // rounding forces the result here to be
                            // an ULP or two larger than
                            // Double.MAX_VALUE ).
                            //
                            t = dValue / 2.0;
                            t *= BIG_10_POW[j];
                            if (double.IsInfinity(t))
                            {
                                return (isNegative) ? double.NegativeInfinity : double.PositiveInfinity;
                            }
                            t = double.MaxValue;
                        }
                        dValue = t;
                    }
                }
                else if (exp < 0)
                {
                    exp = -exp;
                    if (decExponent < MIN_DECIMAL_EXPONENT - 1)
                    {
                        //
                        // Lets face it. This is going to be
                        // zero. Cut to the chase.
                        //
                        return (isNegative) ? -0.0 : 0.0;
                    }
                    if ((exp & 15) != 0)
                    {
                        dValue /= SMALL_10_POW[exp & 15];
                    }
                    if ((exp >>= 4) != 0)
                    {
                        int j;
                        for (j = 0; exp > 1; j++, exp >>= 1)
                        {
                            if ((exp & 1) != 0)
                            {
                                dValue *= TINY_10_POW[j];
                            }
                        }
                        //
                        // The reason for the weird exp > 1 condition
                        // in the above loop was so that the last multiply
                        // would get unrolled. We handle it here.
                        // It could underflow.
                        //
                        double t = dValue * TINY_10_POW[j];
                        if (t == 0.0)
                        {
                            //
                            // It did underflow.
                            // Look more closely at the result.
                            // If the exponent is just one too small,
                            // then use the minimum finite as our estimate
                            // value. Else call the result 0.0
                            // and punt it.
                            // ( I presume this could happen because
                            // rounding forces the result here to be
                            // an ULP or two less than
                            // Double.MIN_VALUE ).
                            //
                            t = dValue * 2.0;
                            t *= TINY_10_POW[j];
                            if (t == 0.0)
                            {
                                return (isNegative) ? -0.0 : 0.0;
                            }
                            t = double.Epsilon; // J2N: MIN_VALUE in Java is equivalent to Epsilon in .NET
                        }
                        dValue = t;
                    }
                }

                //
                // dValue is now approximately the result.
                // The hard part is adjusting it, by comparison
                // with FDBigInteger arithmetic.
                // Formulate the EXACT big-number result as
                // bigD0 * 10^exp
                //
                if (nDigits > MAX_NDIGITS)
                {
                    nDigits = MAX_NDIGITS + 1;
                    digits[MAX_NDIGITS] = '1';
                }
                FDBigInteger bigD0 = new FDBigInteger(lValue, digits, kDigits, nDigits);
                exp = decExponent - nDigits;

                long ieeeBits = BitConversion.DoubleToRawInt64Bits(dValue); // IEEE-754 bits of double candidate
                int B5 = Math.Max(0, -exp); // powers of 5 in bigB, value is not modified inside correctionLoop
                int D5 = Math.Max(0, exp); // powers of 5 in bigD, value is not modified inside correctionLoop
                bigD0 = bigD0.MultByPow52(D5, 0);
                bigD0.MakeImmutable();   // prevent bigD0 modification inside correctionLoop
                FDBigInteger bigD = null;
                int prevD2 = 0;

            //correctionLoop:
                while (true)
                {
                    // here ieeeBits can't be NaN, Infinity or zero
                    int binexp = (int)(ieeeBits.TripleShift(EXP_SHIFT));
                    long bigBbits = ieeeBits & DoubleConsts.SIGNIF_BIT_MASK;
                    if (binexp > 0)
                    {
                        bigBbits |= FRACT_HOB;
                    }
                    else
                    { // Normalize denormalized numbers.
                        Debug.Assert(bigBbits != 0L, bigBbits.ToString()); // doubleToBigInt(0.0)
                        int leadingZeros = bigBbits.LeadingZeroCount();
                        int shift = leadingZeros - (63 - EXP_SHIFT);
                        bigBbits <<= shift;
                        binexp = 1 - shift;
                    }
                    binexp -= DoubleConsts.EXP_BIAS;
                    int lowOrderZeros = bigBbits.TrailingZeroCount();
                    //bigBbits >>>= lowOrderZeros;
                    bigBbits = bigBbits.TripleShift(lowOrderZeros);
                    int bigIntExp = binexp - EXP_SHIFT + lowOrderZeros;
                    int bigIntNBits = EXP_SHIFT + 1 - lowOrderZeros;

                    //
                    // Scale bigD, bigB appropriately for
                    // big-integer operations.
                    // Naively, we multiply by powers of ten
                    // and powers of two. What we actually do
                    // is keep track of the powers of 5 and
                    // powers of 2 we would use, then factor out
                    // common divisors before doing the work.
                    //
                    int B2 = B5; // powers of 2 in bigB
                    int D2 = D5; // powers of 2 in bigD
                    int Ulp2;   // powers of 2 in halfUlp.
                    if (bigIntExp >= 0)
                    {
                        B2 += bigIntExp;
                    }
                    else
                    {
                        D2 -= bigIntExp;
                    }
                    Ulp2 = B2;
                    // shift bigB and bigD left by a number s. t.
                    // halfUlp is still an integer.
                    int hulpbias;
                    if (binexp <= -DoubleConsts.EXP_BIAS)
                    {
                        // This is going to be a denormalized number
                        // (if not actually zero).
                        // half an ULP is at 2^-(DoubleConsts.EXP_BIAS+EXP_SHIFT+1)
                        hulpbias = binexp + lowOrderZeros + DoubleConsts.EXP_BIAS;
                    }
                    else
                    {
                        hulpbias = 1 + lowOrderZeros;
                    }
                    B2 += hulpbias;
                    D2 += hulpbias;
                    // if there are common factors of 2, we might just as well
                    // factor them out, as they add nothing useful.
                    int common2 = Math.Min(B2, Math.Min(D2, Ulp2));
                    B2 -= common2;
                    D2 -= common2;
                    Ulp2 -= common2;
                    // do multiplications by powers of 5 and 2
                    FDBigInteger bigB = FDBigInteger.ValueOfMulPow52(bigBbits, B5, B2);
                    if (bigD == null || prevD2 != D2)
                    {
                        bigD = bigD0.LeftShift(D2);
                        prevD2 = D2;
                    }
                    //
                    // to recap:
                    // bigB is the scaled-big-int version of our floating-point
                    // candidate.
                    // bigD is the scaled-big-int version of the exact value
                    // as we understand it.
                    // halfUlp is 1/2 an ulp of bigB, except for special cases
                    // of exact powers of 2
                    //
                    // the plan is to compare bigB with bigD, and if the difference
                    // is less than halfUlp, then we're satisfied. Otherwise,
                    // use the ratio of difference to halfUlp to calculate a fudge
                    // factor to add to the floating value, then go 'round again.
                    //
                    FDBigInteger diff;
                    int cmpResult;
                    bool overvalue;
                    if ((cmpResult = bigB.Cmp(bigD)) > 0)
                    {
                        overvalue = true; // our candidate is too big.
                        diff = bigB.LeftInplaceSub(bigD); // bigB is not user further - reuse
                        if ((bigIntNBits == 1) && (bigIntExp > -DoubleConsts.EXP_BIAS + 1))
                        {
                            // candidate is a normalized exact power of 2 and
                            // is too big (larger than Double.MIN_NORMAL). We will be subtracting.
                            // For our purposes, ulp is the ulp of the
                            // next smaller range.
                            Ulp2 -= 1;
                            if (Ulp2 < 0)
                            {
                                // rats. Cannot de-scale ulp this far.
                                // must scale diff in other direction.
                                Ulp2 = 0;
                                diff = diff.LeftShift(1);
                            }
                        }
                    }
                    else if (cmpResult < 0)
                    {
                        overvalue = false; // our candidate is too small.
                        diff = bigD.RightInplaceSub(bigB); // bigB is not user further - reuse
                    }
                    else
                    {
                        // the candidate is exactly right!
                        // this happens with surprising frequency
                        break; // correctionLoop;
                    }
                    cmpResult = diff.CmpPow52(B5, Ulp2);
                    if ((cmpResult) < 0)
                    {
                        // difference is small.
                        // this is close enough
                        break; // correctionLoop;
                    }
                    else if (cmpResult == 0)
                    {
                        // difference is exactly half an ULP
                        // round to some other value maybe, then finish
                        if ((ieeeBits & 1) != 0)
                        { // half ties to even
                            ieeeBits += overvalue ? -1 : 1; // nextDown or nextUp
                        }
                        break; // correctionLoop;
                    }
                    else
                    {
                        // difference is non-trivial.
                        // could scale addend by ratio of difference to
                        // halfUlp here, if we bothered to compute that difference.
                        // Most of the time ( I hope ) it is about 1 anyway.
                        ieeeBits += overvalue ? -1 : 1; // nextDown or nextUp
                        if (ieeeBits == 0 || ieeeBits == DoubleConsts.EXP_BIT_MASK)
                        { // 0.0 or Double.POSITIVE_INFINITY
                            break; // correctionLoop; // oops. Fell off end of range.
                        }
                        continue; // try again.
                    }

                }

                if (isNegative)
                {
                    ieeeBits |= DoubleConsts.SIGN_BIT_MASK;
                }
                return BitConversion.Int64BitsToDouble(ieeeBits);
            }

            /**
             * Takes a FloatingDecimal, which we presumably just scanned in,
             * and finds out what its value is, as a float.
             * This is distinct from doubleValue() to avoid the extremely
             * unlikely case of a double rounding error, wherein the conversion
             * to double has one rounding error, and the conversion of that double
             * to a float has another rounding error, IN THE WRONG DIRECTION,
             * ( because of the preference to a zero low-order bit ).
             */
            public virtual float ToSingle()
            {
                int kDigits = Math.Min(nDigits, SINGLE_MAX_DECIMAL_DIGITS + 1);
                //
                // convert the lead kDigits to an integer.
                //
                int iValue = (int)digits[0] - (int)'0';
                for (int i = 1; i < kDigits; i++)
                {
                    iValue = iValue * 10 + (int)digits[i] - (int)'0';
                }
                float fValue = (float)iValue;
                int exp = decExponent - kDigits;
                //
                // iValue now contains an integer with the value of
                // the first kDigits digits of the number.
                // fValue contains the (float) of the same.
                //

                if (nDigits <= SINGLE_MAX_DECIMAL_DIGITS)
                {
                    //
                    // possibly an easy case.
                    // We know that the digits can be represented
                    // exactly. And if the exponent isn't too outrageous,
                    // the whole thing can be done with one operation,
                    // thus one rounding error.
                    // Note that all our constructors trim all leading and
                    // trailing zeros, so simple values (including zero)
                    // will always end up here.
                    //
                    if (exp == 0 || fValue == 0.0f)
                    {
                        return (isNegative) ? -fValue : fValue; // small floating integer
                    }
                    else if (exp >= 0)
                    {
                        if (exp <= SINGLE_MAX_SMALL_TEN)
                        {
                            //
                            // Can get the answer with one operation,
                            // thus one roundoff.
                            //
                            fValue *= SINGLE_SMALL_10_POW[exp];
                            return (isNegative) ? -fValue : fValue;
                        }
                        int slop = SINGLE_MAX_DECIMAL_DIGITS - kDigits;
                        if (exp <= SINGLE_MAX_SMALL_TEN + slop)
                        {
                            //
                            // We can multiply fValue by 10^(slop)
                            // and it is still "small" and exact.
                            // Then we can multiply by 10^(exp-slop)
                            // with one rounding.
                            //
                            fValue *= SINGLE_SMALL_10_POW[slop];
                            fValue *= SINGLE_SMALL_10_POW[exp - slop];
                            return (isNegative) ? -fValue : fValue;
                        }
                        //
                        // Else we have a hard case with a positive exp.
                        //
                    }
                    else
                    {
                        if (exp >= -SINGLE_MAX_SMALL_TEN)
                        {
                            //
                            // Can get the answer in one division.
                            //
                            fValue /= SINGLE_SMALL_10_POW[-exp];
                            return (isNegative) ? -fValue : fValue;
                        }
                        //
                        // Else we have a hard case with a negative exp.
                        //
                    }
                }
                else if ((decExponent >= nDigits) && (nDigits + decExponent <= MAX_DECIMAL_DIGITS))
                {
                    //
                    // In double-precision, this is an exact floating integer.
                    // So we can compute to double, then shorten to float
                    // with one round, and get the right answer.
                    //
                    // First, finish accumulating digits.
                    // Then convert that integer to a double, multiply
                    // by the appropriate power of ten, and convert to float.
                    //
                    long lValue = (long)iValue;
                    for (int i = kDigits; i < nDigits; i++)
                    {
                        lValue = lValue * 10L + (long)((int)digits[i] - (int)'0');
                    }
                    double dValue2 = (double)lValue;
                    exp = decExponent - nDigits;
                    dValue2 *= SMALL_10_POW[exp];
                    fValue = (float)dValue2;
                    return (isNegative) ? -fValue : fValue;

                }
                //
                // Harder cases:
                // The sum of digits plus exponent is greater than
                // what we think we can do with one error.
                //
                // Start by approximating the right answer by,
                // naively, scaling by powers of 10.
                // Scaling uses doubles to avoid overflow/underflow.
                //
                double dValue = fValue;
                if (exp > 0)
                {
                    if (decExponent > SINGLE_MAX_DECIMAL_EXPONENT + 1)
                    {
                        //
                        // Lets face it. This is going to be
                        // Infinity. Cut to the chase.
                        //
                        return (isNegative) ? float.NegativeInfinity : float.PositiveInfinity;
                    }
                    if ((exp & 15) != 0)
                    {
                        dValue *= SMALL_10_POW[exp & 15];
                    }
                    if ((exp >>= 4) != 0)
                    {
                        int j;
                        for (j = 0; exp > 0; j++, exp >>= 1)
                        {
                            if ((exp & 1) != 0)
                            {
                                dValue *= BIG_10_POW[j];
                            }
                        }
                    }
                }
                else if (exp < 0)
                {
                    exp = -exp;
                    if (decExponent < SINGLE_MIN_DECIMAL_EXPONENT - 1)
                    {
                        //
                        // Lets face it. This is going to be
                        // zero. Cut to the chase.
                        //
                        return (isNegative) ? -0.0f : 0.0f;
                    }
                    if ((exp & 15) != 0)
                    {
                        dValue /= SMALL_10_POW[exp & 15];
                    }
                    if ((exp >>= 4) != 0)
                    {
                        int j;
                        for (j = 0; exp > 0; j++, exp >>= 1)
                        {
                            if ((exp & 1) != 0)
                            {
                                dValue *= TINY_10_POW[j];
                            }
                        }
                    }
                }
                fValue = Math.Max(float.Epsilon, Math.Min(float.MaxValue, (float)dValue)); // J2N: MIN_VALUE in Java is equivalent to Epsilon in .NET

                //
                // fValue is now approximately the result.
                // The hard part is adjusting it, by comparison
                // with FDBigInteger arithmetic.
                // Formulate the EXACT big-number result as
                // bigD0 * 10^exp
                //
                if (nDigits > SINGLE_MAX_NDIGITS)
                {
                    nDigits = SINGLE_MAX_NDIGITS + 1;
                    digits[SINGLE_MAX_NDIGITS] = '1';
                }
                FDBigInteger bigD0 = new FDBigInteger(iValue, digits, kDigits, nDigits);
                exp = decExponent - nDigits;

                int ieeeBits = BitConversion.SingleToRawInt32Bits(fValue); // IEEE-754 bits of float candidate
                int B5 = Math.Max(0, -exp); // powers of 5 in bigB, value is not modified inside correctionLoop
                int D5 = Math.Max(0, exp); // powers of 5 in bigD, value is not modified inside correctionLoop
                bigD0 = bigD0.MultByPow52(D5, 0);
                bigD0.MakeImmutable();   // prevent bigD0 modification inside correctionLoop
                FDBigInteger bigD = null;
                int prevD2 = 0;

            //correctionLoop:
                while (true)
                {
                    // here ieeeBits can't be NaN, Infinity or zero
                    int binexp = ieeeBits.TripleShift(SINGLE_EXP_SHIFT);
                    int bigBbits = ieeeBits & FloatConsts.SIGNIF_BIT_MASK;
                    if (binexp > 0)
                    {
                        bigBbits |= SINGLE_FRACT_HOB;
                    }
                    else
                    { // Normalize denormalized numbers.
                        Debug.Assert(bigBbits != 0, bigBbits.ToString()); // floatToBigInt(0.0)
                        int leadingZeros = bigBbits.LeadingZeroCount();
                        int shift = leadingZeros - (31 - SINGLE_EXP_SHIFT);
                        bigBbits <<= shift;
                        binexp = 1 - shift;
                    }
                    binexp -= FloatConsts.EXP_BIAS;
                    int lowOrderZeros = bigBbits.TrailingZeroCount();
                    //bigBbits >>>= lowOrderZeros;
                    bigBbits = bigBbits.TripleShift(lowOrderZeros);
                    int bigIntExp = binexp - SINGLE_EXP_SHIFT + lowOrderZeros;
                    int bigIntNBits = SINGLE_EXP_SHIFT + 1 - lowOrderZeros;

                    //
                    // Scale bigD, bigB appropriately for
                    // big-integer operations.
                    // Naively, we multiply by powers of ten
                    // and powers of two. What we actually do
                    // is keep track of the powers of 5 and
                    // powers of 2 we would use, then factor out
                    // common divisors before doing the work.
                    //
                    int B2 = B5; // powers of 2 in bigB
                    int D2 = D5; // powers of 2 in bigD
                    int Ulp2;   // powers of 2 in halfUlp.
                    if (bigIntExp >= 0)
                    {
                        B2 += bigIntExp;
                    }
                    else
                    {
                        D2 -= bigIntExp;
                    }
                    Ulp2 = B2;
                    // shift bigB and bigD left by a number s. t.
                    // halfUlp is still an integer.
                    int hulpbias;
                    if (binexp <= -FloatConsts.EXP_BIAS)
                    {
                        // This is going to be a denormalized number
                        // (if not actually zero).
                        // half an ULP is at 2^-(FloatConsts.EXP_BIAS+SINGLE_EXP_SHIFT+1)
                        hulpbias = binexp + lowOrderZeros + FloatConsts.EXP_BIAS;
                    }
                    else
                    {
                        hulpbias = 1 + lowOrderZeros;
                    }
                    B2 += hulpbias;
                    D2 += hulpbias;
                    // if there are common factors of 2, we might just as well
                    // factor them out, as they add nothing useful.
                    int common2 = Math.Min(B2, Math.Min(D2, Ulp2));
                    B2 -= common2;
                    D2 -= common2;
                    Ulp2 -= common2;
                    // do multiplications by powers of 5 and 2
                    FDBigInteger bigB = FDBigInteger.ValueOfMulPow52(bigBbits, B5, B2);
                    if (bigD == null || prevD2 != D2)
                    {
                        bigD = bigD0.LeftShift(D2);
                        prevD2 = D2;
                    }
                    //
                    // to recap:
                    // bigB is the scaled-big-int version of our floating-point
                    // candidate.
                    // bigD is the scaled-big-int version of the exact value
                    // as we understand it.
                    // halfUlp is 1/2 an ulp of bigB, except for special cases
                    // of exact powers of 2
                    //
                    // the plan is to compare bigB with bigD, and if the difference
                    // is less than halfUlp, then we're satisfied. Otherwise,
                    // use the ratio of difference to halfUlp to calculate a fudge
                    // factor to add to the floating value, then go 'round again.
                    //
                    FDBigInteger diff;
                    int cmpResult;
                    bool overvalue;
                    if ((cmpResult = bigB.Cmp(bigD)) > 0)
                    {
                        overvalue = true; // our candidate is too big.
                        diff = bigB.LeftInplaceSub(bigD); // bigB is not user further - reuse
                        if ((bigIntNBits == 1) && (bigIntExp > -FloatConsts.EXP_BIAS + 1))
                        {
                            // candidate is a normalized exact power of 2 and
                            // is too big (larger than Float.MIN_NORMAL). We will be subtracting.
                            // For our purposes, ulp is the ulp of the
                            // next smaller range.
                            Ulp2 -= 1;
                            if (Ulp2 < 0)
                            {
                                // rats. Cannot de-scale ulp this far.
                                // must scale diff in other direction.
                                Ulp2 = 0;
                                diff = diff.LeftShift(1);
                            }
                        }
                    }
                    else if (cmpResult < 0)
                    {
                        overvalue = false; // our candidate is too small.
                        diff = bigD.RightInplaceSub(bigB); // bigB is not user further - reuse
                    }
                    else
                    {
                        // the candidate is exactly right!
                        // this happens with surprising frequency
                        break; // correctionLoop;
                    }
                    cmpResult = diff.CmpPow52(B5, Ulp2);
                    if ((cmpResult) < 0)
                    {
                        // difference is small.
                        // this is close enough
                        break; // correctionLoop;
                    }
                    else if (cmpResult == 0)
                    {
                        // difference is exactly half an ULP
                        // round to some other value maybe, then finish
                        if ((ieeeBits & 1) != 0)
                        { // half ties to even
                            ieeeBits += overvalue ? -1 : 1; // nextDown or nextUp
                        }
                        break; // correctionLoop;
                    }
                    else
                    {
                        // difference is non-trivial.
                        // could scale addend by ratio of difference to
                        // halfUlp here, if we bothered to compute that difference.
                        // Most of the time ( I hope ) it is about 1 anyway.
                        ieeeBits += overvalue ? -1 : 1; // nextDown or nextUp
                        if (ieeeBits == 0 || ieeeBits == FloatConsts.EXP_BIT_MASK)
                        { // 0.0 or Float.POSITIVE_INFINITY
                            break; // correctionLoop; // oops. Fell off end of range.
                        }
                        continue; // try again.
                    }

                }
                if (isNegative)
                {
                    ieeeBits |= FloatConsts.SIGN_BIT_MASK;
                }
                return BitConversion.Int32BitsToSingle(ieeeBits);
            }


            /**
             * All the positive powers of 10 that can be
             * represented exactly in double/float.
             */
            private static readonly double[] SMALL_10_POW = {
            1.0e0,
            1.0e1, 1.0e2, 1.0e3, 1.0e4, 1.0e5,
            1.0e6, 1.0e7, 1.0e8, 1.0e9, 1.0e10,
            1.0e11, 1.0e12, 1.0e13, 1.0e14, 1.0e15,
            1.0e16, 1.0e17, 1.0e18, 1.0e19, 1.0e20,
            1.0e21, 1.0e22
        };

            private static readonly float[] SINGLE_SMALL_10_POW = {
            1.0e0f,
            1.0e1f, 1.0e2f, 1.0e3f, 1.0e4f, 1.0e5f,
            1.0e6f, 1.0e7f, 1.0e8f, 1.0e9f, 1.0e10f
        };

            private static readonly double[] BIG_10_POW = {
            1e16, 1e32, 1e64, 1e128, 1e256 };
            private static readonly double[] TINY_10_POW = {
            1e-16, 1e-32, 1e-64, 1e-128, 1e-256 };

            private static readonly int MAX_SMALL_TEN = SMALL_10_POW.Length - 1;
            private static readonly int SINGLE_MAX_SMALL_TEN = SINGLE_SMALL_10_POW.Length - 1;

        }

        /**
         * Returns a <code>IBinaryToASCIIConverter</code> for a <code>double</code>.
         * The returned object is a <code>ThreadLocal</code> variable of this class.
         *
         * @param d The double precision value to convert.
         * @return The converter.
         */
        public static IBinaryToASCIIConverter GetBinaryToASCIIConverter(double d)
        {
            return GetBinaryToASCIIConverter(d, true);
        }

        /**
         * Returns a <code>IBinaryToASCIIConverter</code> for a <code>double</code>.
         * The returned object is a <code>ThreadLocal</code> variable of this class.
         *
         * @param d The double precision value to convert.
         * @param isCompatibleFormat
         * @return The converter.
         */
        internal static IBinaryToASCIIConverter GetBinaryToASCIIConverter(double d, bool isCompatibleFormat)
        {
            long dBits = BitConversion.DoubleToRawInt64Bits(d);
            bool isNegative = (dBits & DoubleConsts.SIGN_BIT_MASK) != 0; // discover sign
            long fractBits = dBits & DoubleConsts.SIGNIF_BIT_MASK;
            int binExp = (int)((dBits & DoubleConsts.EXP_BIT_MASK) >> EXP_SHIFT);
            // Discover obvious special cases of NaN and Infinity.
            if (binExp == (int)(DoubleConsts.EXP_BIT_MASK >> EXP_SHIFT))
            {
                if (fractBits == 0L)
                {
                    return isNegative ? B2AC_NEGATIVE_INFINITY : B2AC_POSITIVE_INFINITY;
                }
                else
                {
                    return B2AC_NOT_A_NUMBER;
                }
            }
            // Finish unpacking
            // Normalize denormalized numbers.
            // Insert assumed high-order bit for normalized numbers.
            // Subtract exponent bias.
            int nSignificantBits;
            if (binExp == 0)
            {
                if (fractBits == 0L)
                {
                    // not a denorm, just a 0!
                    return isNegative ? B2AC_NEGATIVE_ZERO : B2AC_POSITIVE_ZERO;
                }
                int leadingZeros = fractBits.LeadingZeroCount();
                int shift = leadingZeros - (63 - EXP_SHIFT);
                fractBits <<= shift;
                binExp = 1 - shift;
                nSignificantBits = 64 - leadingZeros; // recall binExp is  - shift count.
            }
            else
            {
                fractBits |= FRACT_HOB;
                nSignificantBits = EXP_SHIFT + 1;
            }
            binExp -= DoubleConsts.EXP_BIAS;
            BinaryToASCIIBuffer buf = GetBinaryToASCIIBuffer();
            buf.SetSign(isNegative);
            // call the routine that actually does all the hard work.
            buf.Dtoa(binExp, fractBits, nSignificantBits, isCompatibleFormat);
            return buf;
        }

        private static IBinaryToASCIIConverter GetBinaryToASCIIConverter(float f)
        {
            int fBits = BitConversion.SingleToRawInt32Bits(f);
            bool isNegative = (fBits & FloatConsts.SIGN_BIT_MASK) != 0;
            int fractBits = fBits & FloatConsts.SIGNIF_BIT_MASK;
            int binExp = (fBits & FloatConsts.EXP_BIT_MASK) >> SINGLE_EXP_SHIFT;
            // Discover obvious special cases of NaN and Infinity.
            if (binExp == (FloatConsts.EXP_BIT_MASK >> SINGLE_EXP_SHIFT))
            {
                if (fractBits == 0L)
                {
                    return isNegative ? B2AC_NEGATIVE_INFINITY : B2AC_POSITIVE_INFINITY;
                }
                else
                {
                    return B2AC_NOT_A_NUMBER;
                }
            }
            // Finish unpacking
            // Normalize denormalized numbers.
            // Insert assumed high-order bit for normalized numbers.
            // Subtract exponent bias.
            int nSignificantBits;
            if (binExp == 0)
            {
                if (fractBits == 0)
                {
                    // not a denorm, just a 0!
                    return isNegative ? B2AC_NEGATIVE_ZERO : B2AC_POSITIVE_ZERO;
                }
                int leadingZeros = fractBits.LeadingZeroCount();
                int shift = leadingZeros - (31 - SINGLE_EXP_SHIFT);
                fractBits <<= shift;
                binExp = 1 - shift;
                nSignificantBits = 32 - leadingZeros; // recall binExp is  - shift count.
            }
            else
            {
                fractBits |= SINGLE_FRACT_HOB;
                nSignificantBits = SINGLE_EXP_SHIFT + 1;
            }
            binExp -= FloatConsts.EXP_BIAS;
            BinaryToASCIIBuffer buf = GetBinaryToASCIIBuffer();
            buf.SetSign(isNegative);
            // call the routine that actually does all the hard work.
            buf.Dtoa(binExp, ((long)fractBits) << (EXP_SHIFT - SINGLE_EXP_SHIFT), nSignificantBits, true);
            return buf;
        }

        internal static IASCIIToBinaryConverter ReadJavaFormatString(string input)
        {
            bool isNegative = false;
            bool signSeen = false;
            int decExp;
            char c;

            //parseNumber:
            try
            {
                input = input.Trim(); // don't fool around with white space.
                                      // throws NullPointerException if null
                int len = input.Length;
                if (len == 0)
                {
                    throw new FormatException("empty String");
                }
                int i = 0;
                switch (input[i])
                {
                    case '-':
                        isNegative = true;
                        //FALLTHROUGH
                        i++;
                        signSeen = true;
                        break;
                    case '+':
                        i++;
                        signSeen = true;
                        break;
                }
                c = input[i];
                if (c == 'N')
                { // Check for NaN
                    if ((len - i) == NAN_LENGTH && input.IndexOf(NAN_REP, i, StringComparison.Ordinal) == i)
                    {
                        return A2BC_NOT_A_NUMBER;
                    }
                    // something went wrong, throw exception
                    //break; // parseNumber;
                    throw new FormatException("For input string: \"" + input + "\"");
                }
                else if (c == 'I')
                { // Check for Infinity strings
                    if ((len - i) == INFINITY_LENGTH && input.IndexOf(INFINITY_REP, i, StringComparison.Ordinal) == i)
                    {
                        return isNegative ? A2BC_NEGATIVE_INFINITY : A2BC_POSITIVE_INFINITY;
                    }
                    // something went wrong, throw exception
                    //break; // parseNumber;
                    throw new FormatException("For input string: \"" + input + "\"");
                }
                else if (c == '0')
                { // check for hexadecimal floating-point number
                    if (len > i + 1)
                    {
                        char ch = input[i + 1];
                        if (ch == 'x' || ch == 'X')
                        { // possible hex string
                            return ParseHexString(input);
                        }
                    }
                }  // look for and process decimal floating-point string

                char[] digits = new char[len];
                int nDigits = 0;
                bool decSeen = false;
                int decPt = 0;
                int nLeadZero = 0;
                int nTrailZero = 0;

                //skipLeadingZerosLoop:
                while (i < len)
                {
                    c = input[i];
                    if (c == '0')
                    {
                        nLeadZero++;
                    }
                    else if (c == '.')
                    {
                        if (decSeen)
                        {
                            // already saw one ., this is the 2nd.
                            throw new FormatException("multiple points");
                        }
                        decPt = i;
                        if (signSeen)
                        {
                            decPt -= 1;
                        }
                        decSeen = true;
                    }
                    else
                    {
                        break; //skipLeadingZerosLoop;
                    }
                    i++;
                }
                //digitLoop:
                while (i < len)
                {
                    c = input[i];
                    if (c >= '1' && c <= '9')
                    {
                        digits[nDigits++] = c;
                        nTrailZero = 0;
                    }
                    else if (c == '0')
                    {
                        digits[nDigits++] = c;
                        nTrailZero++;
                    }
                    else if (c == '.')
                    {
                        if (decSeen)
                        {
                            // already saw one ., this is the 2nd.
                            throw new FormatException("multiple points");
                        }
                        decPt = i;
                        if (signSeen)
                        {
                            decPt -= 1;
                        }
                        decSeen = true;
                    }
                    else
                    {
                        break; //digitLoop;
                    }
                    i++;
                }
                nDigits -= nTrailZero;
                //
                // At this point, we've scanned all the digits and decimal
                // point we're going to see. Trim off leading and trailing
                // zeros, which will just confuse us later, and adjust
                // our initial decimal exponent accordingly.
                // To review:
                // we have seen i total characters.
                // nLeadZero of them were zeros before any other digits.
                // nTrailZero of them were zeros after any other digits.
                // if ( decSeen ), then a . was seen after decPt characters
                // ( including leading zeros which have been discarded )
                // nDigits characters were neither lead nor trailing
                // zeros, nor point
                //
                //
                // special hack: if we saw no non-zero digits, then the
                // answer is zero!
                // Unfortunately, we feel honor-bound to keep parsing!
                //
                bool isZero = (nDigits == 0);
                if (isZero && nLeadZero == 0)
                {
                    // we saw NO DIGITS AT ALL,
                    // not even a crummy 0!
                    // this is not allowed.
                    //break parseNumber; // go throw exception
                    throw new FormatException("For input string: \"" + input + "\"");
                }
                //
                // Our initial exponent is decPt, adjusted by the number of
                // discarded zeros. Or, if there was no decPt,
                // then its just nDigits adjusted by discarded trailing zeros.
                //
                if (decSeen)
                {
                    decExp = decPt - nLeadZero;
                }
                else
                {
                    decExp = nDigits + nTrailZero;
                }

                //
                // Look for 'e' or 'E' and an optionally signed integer.
                //
                if ((i < len) && (((c = input[i]) == 'e') || (c == 'E')))
                {
                    int expSign = 1;
                    int expVal = 0;
                    int reallyBig = int.MaxValue / 10;
                    bool expOverflow = false;
                    switch (input[++i])
                    {
                        case '-':
                            expSign = -1;
                            //FALLTHROUGH
                            i++;
                            break;
                        case '+':
                            i++;
                            break;
                    }
                    int expAt = i;
                    //expLoop:
                    while (i < len)
                    {
                        if (expVal >= reallyBig)
                        {
                            // the next character will cause integer
                            // overflow.
                            expOverflow = true;
                        }
                        c = input[i++];
                        if (c >= '0' && c <= '9')
                        {
                            expVal = expVal * 10 + ((int)c - (int)'0');
                        }
                        else
                        {
                            i--;           // back up.
                            break; //expLoop; // stop parsing exponent.
                        }
                    }
                    int expLimit = BIG_DECIMAL_EXPONENT + nDigits + nTrailZero;
                    if (expOverflow || (expVal > expLimit))
                    {
                        //
                        // The intent here is to end up with
                        // infinity or zero, as appropriate.
                        // The reason for yielding such a small decExponent,
                        // rather than something intuitive such as
                        // expSign*Integer.MAX_VALUE, is that this value
                        // is subject to further manipulation in
                        // doubleValue() and floatValue(), and I don't want
                        // it to be able to cause overflow there!
                        // (The only way we can get into trouble here is for
                        // really outrageous nDigits+nTrailZero, such as 2 billion. )
                        //
                        decExp = expSign * expLimit;
                    }
                    else
                    {
                        // this should not overflow, since we tested
                        // for expVal > (MAX+N), where N >= abs(decExp)
                        decExp = decExp + expSign * expVal;
                    }

                    // if we saw something not a digit ( or end of string )
                    // after the [Ee][+-], without seeing any digits at all
                    // this is certainly an error. If we saw some digits,
                    // but then some trailing garbage, that might be ok.
                    // so we just fall through in that case.
                    // HUMBUG
                    if (i == expAt)
                    {
                        //break parseNumber; // certainly bad
                        throw new FormatException("For input string: \"" + input + "\"");
                    }
                }
                //
                // We parsed everything we could.
                // If there are leftovers, then this is not good input!
                //
                if (i < len &&
                    ((i != len - 1) ||
                    (input[i] != 'f' &&
                         input[i] != 'F' &&
                         input[i] != 'd' &&
                         input[i] != 'D')))
                {
                    //break parseNumber; // go throw exception
                    throw new FormatException("For input string: \"" + input + "\"");
                }
                if (isZero)
                {
                    return isNegative ? A2BC_NEGATIVE_ZERO : A2BC_POSITIVE_ZERO;
                }
                return new ASCIIToBinaryBuffer(isNegative, decExp, digits, nDigits);
            }
            catch (IndexOutOfRangeException e)
            {
                throw new FormatException("For input string: \"" + input + "\"", e);
            }
            throw new FormatException("For input string: \"" + input + "\"");
        }

        private static class HexFloatPattern
        {
            /// <summary>
            /// Patch for the .NET Regex class because it only supports Unicode 3.0.1 and does not support the xDigit property.
            /// This brings it up to Unicode 10.0.
            /// </summary>
            private const string XDIGIT = "(?:[\\p{Nd}A-Fa-f\\u07C0-\\u07C9\\u0BE6\\u0DE6-\\u0DEF\\u1090-\\u1099\\u1946-\\u194F\\u19D0-\\u19D9\\u1A80-\\u1A89\\u1A90-\\u1A99\\u1B50-\\u1B59\\u1BB0-\\u1BB9\\u1C40-\\u1C49\\u1C50-\\u1C59\\uA620-\\uA629\\uA8D0-\\uA8D9\\uA900-\\uA909\\uA9D0-\\uA9D9\\uA9F0-\\uA9F9\\uAA50-\\uAA59\\uABF0-\\uABF9\\uFF21-\\uFF26\\uFF41-\\uFF46]|\\uD801[\\uDCA0-\\uDCA9]|\\uD804[\\uDC66-\\uDC6F]|\\uD804[\\uDCF0-\\uDCF9]|\\uD804[\\uDD36-\\uDD3F]|\\uD804[\\uDDD0-\\uDDD9]|\\uD804[\\uDEF0-\\uDEF9]|\\uD805[\\uDC50-\\uDC59]|\\uD805[\\uDCD0-\\uDCD9]|\\uD805[\\uDE50-\\uDE59]|\\uD805[\\uDEC0-\\uDEC9]|\\uD805[\\uDF30-\\uDF39]|\\uD806[\\uDCE0-\\uDCE9]|\\uD807[\\uDC50-\\uDC59]|\\uD807[\\uDD50-\\uDD59]|\\uD81A[\\uDE60-\\uDE69]|\\uD81A[\\uDF50-\\uDF59]|\\uD835[\\uDFCE-\\uDFFF]|\\uD83A[\\uDD50-\\uDD59])";

            /// <summary>
            /// Patch for the .NET Regex class because it only supports Unicode 3.0.1 and does not support the Digit property.
            /// This brings it up to Unicode 10.0.
            /// </summary>
            private const string DIGIT = "(?:[\\p{Nd}\\u07C0-\\u07C9\\u0BE6\\u0DE6-\\u0DEF\\u1090-\\u1099\\u1946-\\u194F\\u19D0-\\u19D9\\u1A80-\\u1A89\\u1A90-\\u1A99\\u1B50-\\u1B59\\u1BB0-\\u1BB9\\u1C40-\\u1C49\\u1C50-\\u1C59\\uA620-\\uA629\\uA8D0-\\uA8D9\\uA900-\\uA909\\uA9D0-\\uA9D9\\uA9F0-\\uA9F9\\uAA50-\\uAA59\\uABF0-\\uABF9]|\\uD801[\\uDCA0-\\uDCA9]|\\uD804[\\uDC66-\\uDC6F]|\\uD804[\\uDCF0-\\uDCF9]|\\uD804[\\uDD36-\\uDD3F]|\\uD804[\\uDDD0-\\uDDD9]|\\uD804[\\uDEF0-\\uDEF9]|\\uD805[\\uDC50-\\uDC59]|\\uD805[\\uDCD0-\\uDCD9]|\\uD805[\\uDE50-\\uDE59]|\\uD805[\\uDEC0-\\uDEC9]|\\uD805[\\uDF30-\\uDF39]|\\uD806[\\uDCE0-\\uDCE9]|\\uD807[\\uDC50-\\uDC59]|\\uD807[\\uDD50-\\uDD59]|\\uD81A[\\uDE60-\\uDE69]|\\uD81A[\\uDF50-\\uDF59]|\\uD835[\\uDFCE-\\uDFFF]|\\uD83A[\\uDD50-\\uDD59])";

            /// <summary>
            /// Grammar is compatible with hexadecimal floating-point constants
            /// described in section 6.4.4.2 of the C99 specification.
            /// </summary>
            internal static readonly Regex VALUE = new Regex(
                //1           234                      56                   7                      8      9
                "([-+])?0[xX](((" + XDIGIT + "+)\\.?)|((" + XDIGIT + "*)\\.(" + XDIGIT + "+)))[pP]([-+])?(" + DIGIT + "+)[fFdD]?",
                RegexOptions.Compiled);
        }

        /**
         * Converts string s to a suitable floating decimal; uses the
         * double constructor and sets the roundDir variable appropriately
         * in case the value is later converted to a float.
         *
         * @param s The <code>String</code> to parse.
         */
        internal static IASCIIToBinaryConverter ParseHexString(string s)
        {
            // Verify string is a member of the hexadecimal floating-point
            // string language.
            Match m = HexFloatPattern.VALUE.Match(s);
            bool validInput = m.Success;
            if (!validInput)
            {
                // Input does not match pattern
                throw new FormatException("For input string: \"" + s + "\"");
            }
            else
            { // validInput
              //
              // We must isolate the sign, significand, and exponent
              // fields.  The sign value is straightforward.  Since
              // floating-point numbers are stored with a normalized
              // representation, the significand and exponent are
              // interrelated.
              //
              // After extracting the sign, we normalized the
              // significand as a hexadecimal value, calculating an
              // exponent adjust for any shifts made during
              // normalization.  If the significand is zero, the
              // exponent doesn't need to be examined since the output
              // will be zero.
              //
              // Next the exponent in the input string is extracted.
              // Afterwards, the significand is normalized as a *binary*
              // value and the input value's normalized exponent can be
              // computed.  The significand bits are copied into a
              // double significand; if the string has more logical bits
              // than can fit in a double, the extra bits affect the
              // round and sticky bits which are used to round the final
              // value.
              //
              //  Extract significand sign
                Group group1 = m.Groups[1];
                bool isNegative = group1.Success && group1.Value.Equals("-", StringComparison.Ordinal);

                //  Extract Significand magnitude
                //
                // Based on the form of the significand, calculate how the
                // binary exponent needs to be adjusted to create a
                // normalized//hexadecimal* floating-point number; that
                // is, a number where there is one nonzero hex digit to
                // the left of the (hexa)decimal point.  Since we are
                // adjusting a binary, not hexadecimal exponent, the
                // exponent is adjusted by a multiple of 4.
                //
                // There are a number of significand scenarios to consider;
                // letters are used in indicate nonzero digits:
                //
                // 1. 000xxxx       =>      x.xxx   normalized
                //    increase exponent by (number of x's - 1)*4
                //
                // 2. 000xxx.yyyy =>        x.xxyyyy        normalized
                //    increase exponent by (number of x's - 1)*4
                //
                // 3. .000yyy  =>   y.yy    normalized
                //    decrease exponent by (number of zeros + 1)*4
                //
                // 4. 000.00000yyy => y.yy normalized
                //    decrease exponent by (number of zeros to right of point + 1)*4
                //
                // If the significand is exactly zero, return a properly
                // signed zero.
                //

                string significandString = null;
                int signifLength = 0;
                int exponentAdjust = 0;
                {
                    int leftDigits = 0; // number of meaningful digits to
                                        // left of "decimal" point
                                        // (leading zeros stripped)
                    int rightDigits = 0; // number of digits to right of
                                         // "decimal" point; leading zeros
                                         // must always be accounted for
                                         //
                                         // The significand is made up of either
                                         //
                                         // 1. group 4 entirely (integer portion only)
                                         //
                                         // OR
                                         //
                                         // 2. the fractional portion from group 7 plus any
                                         // (optional) integer portions from group 6.
                                         //
                    Group group4;
                    if ((group4 = m.Groups[4]).Success)
                    {  // Integer-only significand
                       // Leading zeros never matter on the integer portion
                        significandString = StripLeadingZeros(group4.Value);
                        leftDigits = significandString.Length;
                    }
                    else
                    {
                        // Group 6 is the optional integer; leading zeros
                        // never matter on the integer portion
                        string group6 = StripLeadingZeros(m.Groups[6].Value);
                        leftDigits = group6.Length;

                        // fraction
                        string group7 = m.Groups[7].Value;
                        rightDigits = group7.Length;

                        // Turn "integer.fraction" into "integer"+"fraction"
                        significandString = group6 + group7;
                                //((group6 == null) ? "" : group6) + // is the null
                                //                                   // check necessary?
                                //        group7;
                    }

                    significandString = StripLeadingZeros(significandString);
                    signifLength = significandString.Length;

                    //
                    // Adjust exponent as described above
                    //
                    if (leftDigits >= 1)
                    {  // Cases 1 and 2
                        exponentAdjust = 4 * (leftDigits - 1);
                    }
                    else
                    {                // Cases 3 and 4
                        exponentAdjust = -4 * (rightDigits - signifLength + 1);
                    }

                    // If the significand is zero, the exponent doesn't
                    // matter; return a properly signed zero.

                    if (signifLength == 0)
                    { // Only zeros in input
                        return isNegative ? A2BC_NEGATIVE_ZERO : A2BC_POSITIVE_ZERO;
                    }
                }

                //  Extract Exponent
                //
                // Use an int to read in the exponent value; this should
                // provide more than sufficient range for non-contrived
                // inputs.  If reading the exponent in as an int does
                // overflow, examine the sign of the exponent and
                // significand to determine what to do.
                //
                Group group8 = m.Groups[8];
                bool positiveExponent = (!group8.Success) || group8.Value.Equals("+", StringComparison.Ordinal);
                if (!int.TryParse(m.Groups[9].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int unsignedRawExponent))
                {
                    // At this point, we know the exponent is
                    // syntactically well-formed as a sequence of
                    // digits.  Therefore, if an NumberFormatException
                    // is thrown, it must be due to overflowing int's
                    // range.  Also, at this point, we have already
                    // checked for a zero significand.  Thus the signs
                    // of the exponent and significand determine the
                    // final result:
                    //
                    //                      significand
                    //                      +               -
                    // exponent     +       +infinity       -infinity
                    //              -       +0.0            -0.0
                    return isNegative ?
                              (positiveExponent ? A2BC_NEGATIVE_INFINITY : A2BC_NEGATIVE_ZERO)
                            : (positiveExponent ? A2BC_POSITIVE_INFINITY : A2BC_POSITIVE_ZERO);

                }

                long rawExponent =
                        (positiveExponent ? 1L : -1L) * // exponent sign
                                unsignedRawExponent;            // exponent magnitude

                // Calculate partially adjusted exponent
                long exponent = rawExponent + exponentAdjust;

                // Starting copying non-zero bits into proper position in
                // a long; copy explicit bit too; this will be masked
                // later for normal values.

                bool round = false;
                bool sticky = false;
                int nextShift = 0;
                long significand = 0L;
                // First iteration is different, since we only copy
                // from the leading significand bit; one more exponent
                // adjust will be needed...

                // IMPORTANT: make leadingDigit a long to avoid
                // surprising shift semantics!
                long leadingDigit = GetHexDigit(significandString, 0);

                //
                // Left shift the leading digit (53 - (bit position of
                // leading 1 in digit)); this sets the top bit of the
                // significand to 1.  The nextShift value is adjusted
                // to take into account the number of bit positions of
                // the leadingDigit actually used.  Finally, the
                // exponent is adjusted to normalize the significand
                // as a binary value, not just a hex value.
                //
                if (leadingDigit == 1)
                {
                    significand |= leadingDigit << 52;
                    nextShift = 52 - 4;
                    // exponent += 0
                }
                else if (leadingDigit <= 3)
                { // [2, 3]
                    significand |= leadingDigit << 51;
                    nextShift = 52 - 5;
                    exponent += 1;
                }
                else if (leadingDigit <= 7)
                { // [4, 7]
                    significand |= leadingDigit << 50;
                    nextShift = 52 - 6;
                    exponent += 2;
                }
                else if (leadingDigit <= 15)
                { // [8, f]
                    significand |= leadingDigit << 49;
                    nextShift = 52 - 7;
                    exponent += 3;
                }
                else
                {
                    throw new InvalidOperationException("Result from digit conversion too large!");
                }
                // The preceding if-else could be replaced by a single
                // code block based on the high-order bit set in
                // leadingDigit.  Given leadingOnePosition,

                // significand |= leadingDigit << (SIGNIFICAND_WIDTH - leadingOnePosition);
                // nextShift = 52 - (3 + leadingOnePosition);
                // exponent += (leadingOnePosition-1);

                //
                // Now the exponent variable is equal to the normalized
                // binary exponent.  Code below will make representation
                // adjustments if the exponent is incremented after
                // rounding (includes overflows to infinity) or if the
                // result is subnormal.
                //

                // Copy digit into significand until the significand can't
                // hold another full hex digit or there are no more input
                // hex digits.
                int i = 0;
                for (i = 1;
                     i < signifLength && nextShift >= 0;
                     i++)
                {
                    long currentDigit = GetHexDigit(significandString, i);
                    significand |= (currentDigit << nextShift);
                    nextShift -= 4;
                }

                // After the above loop, the bulk of the string is copied.
                // Now, we must copy any partial hex digits into the
                // significand AND compute the round bit and start computing
                // sticky bit.

                if (i < signifLength)
                { // at least one hex input digit exists
                    long currentDigit = GetHexDigit(significandString, i);

                    // from nextShift, figure out how many bits need
                    // to be copied, if any
                    switch (nextShift)
                    { // must be negative
                        case -1:
                            // three bits need to be copied in; can
                            // set round bit
                            significand |= ((currentDigit & 0xEL) >> 1);
                            round = (currentDigit & 0x1L) != 0L;
                            break;

                        case -2:
                            // two bits need to be copied in; can
                            // set round and start sticky
                            significand |= ((currentDigit & 0xCL) >> 2);
                            round = (currentDigit & 0x2L) != 0L;
                            sticky = (currentDigit & 0x1L) != 0;
                            break;

                        case -3:
                            // one bit needs to be copied in
                            significand |= ((currentDigit & 0x8L) >> 3);
                            // Now set round and start sticky, if possible
                            round = (currentDigit & 0x4L) != 0L;
                            sticky = (currentDigit & 0x3L) != 0;
                            break;

                        case -4:
                            // all bits copied into significand; set
                            // round and start sticky
                            round = ((currentDigit & 0x8L) != 0);  // is top bit set?
                                                                   // nonzeros in three low order bits?
                            sticky = (currentDigit & 0x7L) != 0;
                            break;

                        default:
                            throw new InvalidOperationException("Unexpected shift distance remainder.");
                            // break;
                    }

                    // Round is set; sticky might be set.

                    // For the sticky bit, it suffices to check the
                    // current digit and test for any nonzero digits in
                    // the remaining unprocessed input.
                    i++;
                    while (i < signifLength && !sticky)
                    {
                        currentDigit = GetHexDigit(significandString, i);
                        sticky = sticky || (currentDigit != 0);
                        i++;
                    }

                }
                // else all of string was seen, round and sticky are
                // correct as false.

                // Float calculations
                int floatBits = isNegative ? FloatConsts.SIGN_BIT_MASK : 0;
                if (exponent >= FloatConsts.MIN_EXPONENT)
                {
                    if (exponent > FloatConsts.MAX_EXPONENT)
                    {
                        // Float.POSITIVE_INFINITY
                        floatBits |= FloatConsts.EXP_BIT_MASK;
                    }
                    else
                    {
                        int threshShift = DoubleConsts.SIGNIFICAND_WIDTH - FloatConsts.SIGNIFICAND_WIDTH - 1;
                        bool floatSticky = (significand & ((1L << threshShift) - 1)) != 0 || round || sticky;
                        int iValue = (int)(significand.TripleShift(threshShift));
                        if ((iValue & 3) != 1 || floatSticky)
                        {
                            iValue++;
                        }
                        floatBits |= (((((int)exponent) + (FloatConsts.EXP_BIAS - 1))) << SINGLE_EXP_SHIFT) + (iValue >> 1);
                    }
                }
                else
                {
                    if (exponent < FloatConsts.MIN_SUB_EXPONENT - 1)
                    {
                        // 0
                    }
                    else
                    {
                        // exponent == -127 ==> threshShift = 53 - 2 + (-149) - (-127) = 53 - 24
                        int threshShift = (int)((DoubleConsts.SIGNIFICAND_WIDTH - 2 + FloatConsts.MIN_SUB_EXPONENT) - exponent);
                        Debug.Assert(threshShift >= DoubleConsts.SIGNIFICAND_WIDTH - FloatConsts.SIGNIFICAND_WIDTH);
                        Debug.Assert(threshShift < DoubleConsts.SIGNIFICAND_WIDTH);
                        bool floatSticky = (significand & ((1L << threshShift) - 1)) != 0 || round || sticky;
                        int iValue = (int)(significand.TripleShift(threshShift));
                        if ((iValue & 3) != 1 || floatSticky)
                        {
                            iValue++;
                        }
                        floatBits |= iValue >> 1;
                    }
                }
                float fValue = BitConversion.Int32BitsToSingle(floatBits);

                // Check for overflow and update exponent accordingly.
                if (exponent > DoubleConsts.MAX_EXPONENT)
                {         // Infinite result
                          // overflow to properly signed infinity
                    return isNegative ? A2BC_NEGATIVE_INFINITY : A2BC_POSITIVE_INFINITY;
                }
                else
                {  // Finite return value
                    if (exponent <= DoubleConsts.MAX_EXPONENT && // (Usually) normal result
                            exponent >= DoubleConsts.MIN_EXPONENT)
                    {

                        // The result returned in this block cannot be a
                        // zero or subnormal; however after the
                        // significand is adjusted from rounding, we could
                        // still overflow in infinity.

                        // AND exponent bits into significand; if the
                        // significand is incremented and overflows from
                        // rounding, this combination will update the
                        // exponent correctly, even in the case of
                        // Double.MAX_VALUE overflowing to infinity.

                        significand = (((exponent +
                                (long)DoubleConsts.EXP_BIAS) <<
                                (DoubleConsts.SIGNIFICAND_WIDTH - 1))
                                & DoubleConsts.EXP_BIT_MASK) |
                                (DoubleConsts.SIGNIF_BIT_MASK & significand);

                    }
                    else
                    {  // Subnormal or zero
                       // (exponent < DoubleConsts.MIN_EXPONENT)

                        if (exponent < (DoubleConsts.MIN_SUB_EXPONENT - 1))
                        {
                            // No way to round back to nonzero value
                            // regardless of significand if the exponent is
                            // less than -1075.
                            return isNegative ? A2BC_NEGATIVE_ZERO : A2BC_POSITIVE_ZERO;
                        }
                        else
                        { //  -1075 <= exponent <= MIN_EXPONENT -1 = -1023
                          //
                          // Find bit position to round to; recompute
                          // round and sticky bits, and shift
                          // significand right appropriately.
                          //

                            sticky = sticky || round;
                            round = false;

                            // Number of bits of significand to preserve is
                            // exponent - abs_min_exp +1
                            // check:
                            // -1075 +1074 + 1 = 0
                            // -1023 +1074 + 1 = 52

                            int bitsDiscarded = 53 -
                                    ((int)exponent - DoubleConsts.MIN_SUB_EXPONENT + 1);
                            Debug.Assert(bitsDiscarded >= 1 && bitsDiscarded <= 53);

                            // What to do here:
                            // First, isolate the new round bit
                            round = (significand & (1L << (bitsDiscarded - 1))) != 0L;
                            if (bitsDiscarded > 1)
                            {
                                // create mask to update sticky bits; low
                                // order bitsDiscarded bits should be 1
                                long mask = ~((~0L) << (bitsDiscarded - 1));
                                sticky = sticky || ((significand & mask) != 0L);
                            }

                            // Now, discard the bits
                            significand = significand >> bitsDiscarded;

                            significand = ((((long)(DoubleConsts.MIN_EXPONENT - 1) + // subnorm exp.
                                    (long)DoubleConsts.EXP_BIAS) <<
                                    (DoubleConsts.SIGNIFICAND_WIDTH - 1))
                                    & DoubleConsts.EXP_BIT_MASK) |
                                    (DoubleConsts.SIGNIF_BIT_MASK & significand);
                        }
                    }

                    // The significand variable now contains the currently
                    // appropriate exponent bits too.

                    //
                    // Determine if significand should be incremented;
                    // making this determination depends on the least
                    // significant bit and the round and sticky bits.
                    //
                    // Round to nearest even rounding table, adapted from
                    // table 4.7 in "Computer Arithmetic" by IsraelKoren.
                    // The digit to the left of the "decimal" point is the
                    // least significant bit, the digits to the right of
                    // the point are the round and sticky bits
                    //
                    // Number       Round(x)
                    // x0.00        x0.
                    // x0.01        x0.
                    // x0.10        x0.
                    // x0.11        x1. = x0. +1
                    // x1.00        x1.
                    // x1.01        x1.
                    // x1.10        x1. + 1
                    // x1.11        x1. + 1
                    //
                    bool leastZero = ((significand & 1L) == 0L);
                    if ((leastZero && round && sticky) ||
                            ((!leastZero) && round))
                    {
                        significand++;
                    }

                    double value = isNegative ?
                            BitConversion.Int64BitsToDouble(significand | DoubleConsts.SIGN_BIT_MASK) :
                            BitConversion.Int64BitsToDouble(significand);

                    return new PreparedASCIIToBinaryBuffer(value, fValue);
                }
            }
        }

        /**
         * Returns <code>s</code> with any leading zeros removed.
         */
        private static string StripLeadingZeros(string s)
        {
            //        return  s.replaceFirst("^0+", "");
            if (s != string.Empty && s[0] == '0')
            {
                for (int i = 1; i < s.Length; i++)
                {
                    if (s[i] != '0')
                    {
                        return s.Substring(i);
                    }
                }
                return "";
            }
            return s;
        }

        /**
         * Extracts a hexadecimal digit from position <code>position</code>
         * of string <code>s</code>.
         */
        private static int GetHexDigit(string s, int position)
        {
            int value = Character.Digit(s[position], 16);
            if (value <= -1 || value >= 16)
            {
                throw new InvalidOperationException("Unexpected failure of digit conversion of " +
                                         s[position]);
            }
            return value;
        }



        /// <summary>
        /// Checks whether the given string is negative for the <paramref name="provider"/>. If <paramref name="provider"/>
        /// is <c>null</c>, uses <see cref="CultureInfo.CurrentCulture"/>. Assumes <paramref name="s"/> is trimmed.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        // J2N: Useful to check whether the string is negative after parsing in .NET because it doesn't automatically
        // handle negative zero.
        internal static bool IsNegative(string s, IFormatProvider provider)
        {
            NumberFormatInfo numberFormat = provider is null ? CultureInfo.CurrentCulture.NumberFormat : (NumberFormatInfo)provider.GetFormat(typeof(NumberFormatInfo));
            string negativeSign = numberFormat.NegativeSign;
            switch (numberFormat.NumberNegativePattern)
            {
                case 0: // (1,234.00)
                    return s[0] == '(' && s[s.Length - 1] == ')';
                case 2: // - 1,234.00
                    if (s.Length < negativeSign.Length + 1) // space must be at the end of the sign
                        return false;
                    for (int i = 0; i < negativeSign.Length; i++)
                        if (s[i] != negativeSign[i])
                            return false;
                    if (s[negativeSign.Length] != ' ')
                        return false;
                    return true;
                case 3: // 1,234.00-
                    if (s.Length < negativeSign.Length + 1) // Must be at least 1 digit (we need to ensure there is no space before the sign)
                        return false;
                    for (int i = s.Length - 1, j = negativeSign.Length - 1; j >= 0; i--, j--)
                        if (s[i] != negativeSign[j])
                            return false;
                    if (s[s.Length - (negativeSign.Length + 1)] == ' ')
                        return false;
                    return true;
                case 4: // 1,234.00 -
                    if (s.Length < negativeSign.Length + 1) // space must be at the beginning of the sign
                        return false;
                    for (int i = s.Length - 1, j = negativeSign.Length - 1; j >= 0; i--, j--)
                        if (s[i] != negativeSign[j])
                            return false;
                    if (s[s.Length - (negativeSign.Length + 1)] != ' ')
                        return false;
                    return true;
                default: // (1): -1,234.00
                    if (s.Length < negativeSign.Length + 1) // Must be at least 1 digit (we need to ensure there is no space after the sign)
                        return false;
                    for (int i = 0; i < negativeSign.Length; i++)
                        if (s[i] != negativeSign[i])
                            return false;
                    if (s[negativeSign.Length] == ' ')
                        return false;
                    return true;
            }
        }

        /*
         * Answers true if the string should be parsed as a hex encoding.
         * Assumes the string is trimmed.
         */
        // J2N: From Apache Harmony - useful to escape out before using .NET parsing,
        // which doesn't support hexadecimal.
        internal static bool ParseAsHex(string s)
        {
            int length = s.Length;
            if (length < 2)
            {
                return false;
            }
            char first = s[0];
            char second = s[1];
            if (first == '+' || first == '-')
            {
                // Move along
                if (length < 3)
                {
                    return false;
                }
                first = second;
                second = s[2];
            }
            return (first == '0') && (second == 'x' || second == 'X');
        }
    }
}
