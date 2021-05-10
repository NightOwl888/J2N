using J2N.Text;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace J2N.Numerics
{
    public class OldFloatingDecimalForTest
    {
        bool isExceptional;
        bool isNegative;
        int decExponent;
        char[] digits;
        int nDigits;
        int bigIntExp;
        int bigIntNBits;
        bool mustSetRoundDir = false;
        bool fromHex = false;
        int roundDir = 0; // set by doubleValue

        /*
         * The fields below provides additional information about the result of
         * the binary to decimal digits conversion done in dtoa() and roundup()
         * methods. They are changed if needed by those two methods.
         */

        // True if the dtoa() binary to decimal conversion was exact.
        bool exactDecimalConversion = false;

        // True if the result of the binary to decimal conversion was rounded-up
        // at the end of the conversion process, i.e. roundUp() method was called.
        bool decimalDigitsRoundedUp = false;

        private OldFloatingDecimalForTest(bool negSign, int decExponent, char[] digits, int n, bool e)
        {
            isNegative = negSign;
            isExceptional = e;
            this.decExponent = decExponent;
            this.digits = digits;
            this.nDigits = n;
        }

        /*
         * Constants of the implementation
         * Most are IEEE-754 related.
         * (There are more really boring constants at the end.)
         */
        static readonly long signMask = unchecked((long)0x8000000000000000L);
        static readonly long expMask = 0x7ff0000000000000L;
        static readonly long fractMask = ~(signMask | expMask);
        static readonly int expShift = 52;
        static readonly int expBias = 1023;
        static readonly long fractHOB = (1L << expShift); // assumed High-Order bit
        static readonly long expOne = ((long)expBias) << expShift; // exponent of 1.0
        static readonly int maxSmallBinExp = 62;
        static readonly int minSmallBinExp = -(63 / 3);
        static readonly int maxDecimalDigits = 15;
        static readonly int maxDecimalExponent = 308;
        static readonly int minDecimalExponent = -324;
        static readonly int bigDecimalExponent = 324; // i.e. abs(minDecimalExponent)

        static readonly long highbyte = unchecked((long)0xff00000000000000L);
        static readonly long highbit = unchecked((long)0x8000000000000000L);
        static readonly long lowbytes = ~highbyte;

        static readonly int singleSignMask = unchecked((int)0x80000000);
        static readonly int singleExpMask = 0x7f800000;
        static readonly int singleFractMask = ~(singleSignMask | singleExpMask);
        static readonly int singleExpShift = 23;
        static readonly int singleFractHOB = 1 << singleExpShift;
        static readonly int singleExpBias = 127;
        static readonly int singleMaxDecimalDigits = 7;
        static readonly int singleMaxDecimalExponent = 38;
        static readonly int singleMinDecimalExponent = -45;

        static readonly int intDecimalDigits = 9;


        /*
         * count number of bits from high-order 1 bit to low-order 1 bit,
         * inclusive.
         */
        private static int countBits(long v)
        {
            //
            // the strategy is to shift until we get a non-zero sign bit
            // then shift until we have no bits left, counting the difference.
            // we do byte shifting as a hack. Hope it helps.
            //
            if (v == 0L) return 0;

            while ((v & highbyte) == 0L)
            {
                v <<= 8;
            }
            while (v > 0L)
            { // i.e. while ((v&highbit) == 0L )
                v <<= 1;
            }

            int n = 0;
            while ((v & lowbytes) != 0L)
            {
                v <<= 8;
                n += 8;
            }
            while (v != 0L)
            {
                v <<= 1;
                n += 1;
            }
            return n;
        }

        /*
         * Keep big powers of 5 handy for future reference.
         */
        private static OldFDBigIntForTest[] b5p;

        private static object syncLock = new object();

        private static OldFDBigIntForTest big5pow(int p)
        {
            lock (syncLock)
            {
                Debug.Assert(p >= 0, p.ToString()); // negative power of 5
                if (b5p == null)
                {
                    b5p = new OldFDBigIntForTest[p + 1];
                }
                else if (b5p.Length <= p)
                {
                    OldFDBigIntForTest[] t = new OldFDBigIntForTest[p + 1];
                    Array.Copy(b5p, 0, t, 0, b5p.Length);
                    b5p = t;
                }
                if (b5p[p] != null)
                    return b5p[p];
                else if (p < small5pow.Length)
                    return b5p[p] = new OldFDBigIntForTest(small5pow[p]);
                else if (p < long5pow.Length)
                    return b5p[p] = new OldFDBigIntForTest(long5pow[p]);
                else
                {
                    // construct the value.
                    // recursively.
                    int q, r;
                    // in order to compute 5^p,
                    // compute its square root, 5^(p/2) and square.
                    // or, let q = p / 2, r = p -q, then
                    // 5^p = 5^(q+r) = 5^q * 5^r
                    q = p >> 1;
                    r = p - q;
                    OldFDBigIntForTest bigq = b5p[q];
                    if (bigq == null)
                        bigq = big5pow(q);
                    if (r < small5pow.Length)
                    {
                        return (b5p[p] = bigq.mult(small5pow[r]));
                    }
                    else
                    {
                        OldFDBigIntForTest bigr = b5p[r];
                        if (bigr == null)
                            bigr = big5pow(r);
                        return (b5p[p] = bigq.mult(bigr));
                    }
                }

            }
        }

        //
        // a common operation
        //
        private static OldFDBigIntForTest multPow52(OldFDBigIntForTest v, int p5, int p2)
        {
            if (p5 != 0)
            {
                if (p5 < small5pow.Length)
                {
                    v = v.mult(small5pow[p5]);
                }
                else
                {
                    v = v.mult(big5pow(p5));
                }
            }
            if (p2 != 0)
            {
                v.lshiftMe(p2);
            }
            return v;
        }

        //
        // another common operation
        //
        private static OldFDBigIntForTest constructPow52(int p5, int p2)
        {
            OldFDBigIntForTest v = new OldFDBigIntForTest(big5pow(p5));
            if (p2 != 0)
            {
                v.lshiftMe(p2);
            }
            return v;
        }

        /*
         * Make a floating double into a OldFDBigIntForTest.
         * This could also be structured as a OldFDBigIntForTest
         * constructor, but we'd have to build a lot of knowledge
         * about floating-point representation into it, and we don't want to.
         *
         * AS A SIDE EFFECT, THIS METHOD WILL SET THE INSTANCE VARIABLES
         * bigIntExp and bigIntNBits
         *
         */
        private OldFDBigIntForTest doubleToBigInt(double dval)
        {
            long lbits = BitConversion.DoubleToInt64Bits(dval) & ~signMask;
            int binexp = (int)(lbits.TripleShift(expShift));
            lbits &= fractMask;
            if (binexp > 0)
            {
                lbits |= fractHOB;
            }
            else
            {
                Debug.Assert(lbits != 0L, lbits.ToString()); // doubleToBigInt(0.0)
                binexp += 1;
                while ((lbits & fractHOB) == 0L)
                {
                    lbits <<= 1;
                    binexp -= 1;
                }
            }
            binexp -= expBias;
            int nbits = countBits(lbits);
            /*
             * We now know where the high-order 1 bit is,
             * and we know how many there are.
             */
            int lowOrderZeros = expShift + 1 - nbits;
            //lbits >>>= lowOrderZeros;
            lbits = lbits.TripleShift(lowOrderZeros);

            bigIntExp = binexp + 1 - nbits;
            bigIntNBits = nbits;
            return new OldFDBigIntForTest(lbits);
        }

        /*
         * Compute a number that is the ULP of the given value,
         * for purposes of addition/subtraction. Generally easy.
         * More difficult if subtracting and the argument
         * is a normalized a power of 2, as the ULP changes at these points.
         */
        private static double ulp(double dval, bool subtracting)
        {
            long lbits = BitConversion.DoubleToInt64Bits(dval) & ~signMask;
            int binexp = (int)(lbits.TripleShift(expShift));
            double ulpval;
            if (subtracting && (binexp >= expShift) && ((lbits & fractMask) == 0L))
            {
                // for subtraction from normalized, powers of 2,
                // use next-smaller exponent
                binexp -= 1;
            }
            if (binexp > expShift)
            {
                ulpval = BitConversion.Int64BitsToDouble(((long)(binexp - expShift)) << expShift);
            }
            else if (binexp == 0)
            {
                ulpval = double.Epsilon;
            }
            else
            {
                ulpval = BitConversion.Int64BitsToDouble(1L << (binexp - 1));
            }
            if (subtracting) ulpval = -ulpval;

            return ulpval;
        }

        /*
         * Round a double to a float.
         * In addition to the fraction bits of the double,
         * look at the class instance variable roundDir,
         * which should help us avoid double-rounding error.
         * roundDir was set in hardValueOf if the estimate was
         * close enough, but not exact. It tells us which direction
         * of rounding is preferred.
         */
        float stickyRound(double dval)
        {
            long lbits = BitConversion.DoubleToInt64Bits(dval);
            long binexp = lbits & expMask;
            if (binexp == 0L || binexp == expMask)
            {
                // what we have here is special.
                // don't worry, the right thing will happen.
                return (float)dval;
            }
            lbits += (long)roundDir; // hack-o-matic.
            return (float)BitConversion.Int64BitsToDouble(lbits);
        }


        /*
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
        private void developLongDigits(int decExponent, long lvalue, long insignificant)
        {
            char[] digits;
            int ndigits;
            int digitno;
            int c;
            //
            // Discard non-significant low-order bits, while rounding,
            // up to insignificant value.
            int i;
            for (i = 0; insignificant >= 10L; i++)
                insignificant /= 10L;
            if (i != 0)
            {
                long pow10 = long5pow[i] << i; // 10^i == 5^i * 2^i;
                long residue = lvalue % pow10;
                lvalue /= pow10;
                decExponent += i;
                if (residue >= (pow10 >> 1))
                {
                    // round up based on the low-order bits we're discarding
                    lvalue++;
                }
            }
            if (lvalue <= int.MaxValue)
            {
                Debug.Assert(lvalue > 0L, lvalue.ToString()); // lvalue <= 0
                                                              // even easier subcase!
                                                              // can do int arithmetic rather than long!
                int ivalue = (int)lvalue;
                ndigits = 10;
                digits = perThreadBuffer.Value;
                digitno = ndigits - 1;
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
                ndigits = 20;
                digits = perThreadBuffer.Value;
                digitno = ndigits - 1;
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
            char[] result;
            ndigits -= digitno;
            result = new char[ndigits];
            Array.Copy(digits, digitno, result, 0, ndigits);
            this.digits = result;
            this.decExponent = decExponent + 1;
            this.nDigits = ndigits;
        }

        //
        // add one to the least significant digit.
        // in the unlikely event there is a carry out,
        // deal with it.
        // assert that this will only happen where there
        // is only one digit, e.g. (float)1e-44 seems to do it.
        //
        private void roundup()
        {
            int i;
            int q = digits[i = (nDigits - 1)];
            if (q == '9')
            {
                while (q == '9' && i > 0)
                {
                    digits[i] = '0';
                    q = digits[--i];
                }
                if (q == '9')
                {
                    // carryout! High-order 1, rest 0s, larger exp.
                    decExponent += 1;
                    digits[0] = '1';
                    return;
                }
                // else fall through.
            }
            digits[i] = (char)(q + 1);
            decimalDigitsRoundedUp = true;
        }

        public bool digitsRoundedUp()
        {
            return decimalDigitsRoundedUp;
        }

        /*
         * FIRST IMPORTANT CONSTRUCTOR: DOUBLE
         */
        public OldFloatingDecimalForTest(double d)
        {
            long dBits = BitConversion.DoubleToInt64Bits(d);
            long fractBits;
            int binExp;
            int nSignificantBits;

            // discover and delete sign
            if ((dBits & signMask) != 0)
            {
                isNegative = true;
                dBits ^= signMask;
            }
            else
            {
                isNegative = false;
            }
            // Begin to unpack
            // Discover obvious special cases of NaN and Infinity.
            binExp = (int)((dBits & expMask) >> expShift);
            fractBits = dBits & fractMask;
            if (binExp == (int)(expMask >> expShift))
            {
                isExceptional = true;
                if (fractBits == 0L)
                {
                    digits = infinity;
                }
                else
                {
                    digits = notANumber;
                    isNegative = false; // NaN has no sign!
                }
                nDigits = digits.Length;
                return;
            }
            isExceptional = false;
            // Finish unpacking
            // Normalize denormalized numbers.
            // Insert assumed high-order bit for normalized numbers.
            // Subtract exponent bias.
            if (binExp == 0)
            {
                if (fractBits == 0L)
                {
                    // not a denorm, just a 0!
                    decExponent = 0;
                    digits = zero;
                    nDigits = 1;
                    return;
                }
                while ((fractBits & fractHOB) == 0L)
                {
                    fractBits <<= 1;
                    binExp -= 1;
                }
                nSignificantBits = expShift + binExp + 1; // recall binExp is  - shift count.
                binExp += 1;
            }
            else
            {
                fractBits |= fractHOB;
                nSignificantBits = expShift + 1;
            }
            binExp -= expBias;
            // call the routine that actually does all the hard work.
            dtoa(binExp, fractBits, nSignificantBits);
        }

        /*
         * SECOND IMPORTANT CONSTRUCTOR: SINGLE
         */
        public OldFloatingDecimalForTest(float f)
        {
            int fBits = BitConversion.SingleToInt32Bits(f);
            int fractBits;
            int binExp;
            int nSignificantBits;

            // discover and delete sign
            if ((fBits & singleSignMask) != 0)
            {
                isNegative = true;
                fBits ^= singleSignMask;
            }
            else
            {
                isNegative = false;
            }
            // Begin to unpack
            // Discover obvious special cases of NaN and Infinity.
            binExp = (fBits & singleExpMask) >> singleExpShift;
            fractBits = fBits & singleFractMask;
            if (binExp == (singleExpMask >> singleExpShift))
            {
                isExceptional = true;
                if (fractBits == 0L)
                {
                    digits = infinity;
                }
                else
                {
                    digits = notANumber;
                    isNegative = false; // NaN has no sign!
                }
                nDigits = digits.Length;
                return;
            }
            isExceptional = false;
            // Finish unpacking
            // Normalize denormalized numbers.
            // Insert assumed high-order bit for normalized numbers.
            // Subtract exponent bias.
            if (binExp == 0)
            {
                if (fractBits == 0)
                {
                    // not a denorm, just a 0!
                    decExponent = 0;
                    digits = zero;
                    nDigits = 1;
                    return;
                }
                while ((fractBits & singleFractHOB) == 0)
                {
                    fractBits <<= 1;
                    binExp -= 1;
                }
                nSignificantBits = singleExpShift + binExp + 1; // recall binExp is  - shift count.
                binExp += 1;
            }
            else
            {
                fractBits |= singleFractHOB;
                nSignificantBits = singleExpShift + 1;
            }
            binExp -= singleExpBias;
            // call the routine that actually does all the hard work.
            dtoa(binExp, ((long)fractBits) << (expShift - singleExpShift), nSignificantBits);
        }

        private void dtoa(int binExp, long fractBits, int nSignificantBits)
        {
            int nFractBits; // number of significant bits of fractBits;
            int nTinyBits;  // number of these to the right of the point.
            int decExp;

            // Examine number. Determine if it is an easy case,
            // which we can do pretty trivially using float/long conversion,
            // or whether we must do real work.
            nFractBits = countBits(fractBits);
            nTinyBits = Math.Max(0, nFractBits - binExp - 1);
            if (binExp <= maxSmallBinExp && binExp >= minSmallBinExp)
            {
                // Look more closely at the number to decide if,
                // with scaling by 10^nTinyBits, the result will fit in
                // a long.
                if ((nTinyBits < long5pow.Length) && ((nFractBits + n5bits[nTinyBits]) < 64))
                {
                    /*
                     * We can do this:
                     * take the fraction bits, which are normalized.
                     * (a) nTinyBits == 0: Shift left or right appropriately
                     *     to align the binary point at the extreme right, i.e.
                     *     where a long int point is expected to be. The integer
                     *     result is easily converted to a string.
                     * (b) nTinyBits > 0: Shift right by expShift-nFractBits,
                     *     which effectively converts to long and scales by
                     *     2^nTinyBits. Then multiply by 5^nTinyBits to
                     *     complete the scaling. We know this won't overflow
                     *     because we just counted the number of bits necessary
                     *     in the result. The integer you get from this can
                     *     then be converted to a string pretty easily.
                     */
                    long halfULP;
                    if (nTinyBits == 0)
                    {
                        if (binExp > nSignificantBits)
                        {
                            halfULP = 1L << (binExp - nSignificantBits - 1);
                        }
                        else
                        {
                            halfULP = 0L;
                        }
                        if (binExp >= expShift)
                        {
                            fractBits <<= (binExp - expShift);
                        }
                        else
                        {
                            fractBits = fractBits.TripleShift(expShift - binExp);
                        }
                        developLongDigits(0, fractBits, halfULP);
                        return;
                    }
                    /*
                     * The following causes excess digits to be printed
                     * out in the single-float case. Our manipulation of
                     * halfULP here is apparently not correct. If we
                     * better understand how this works, perhaps we can
                     * use this special case again. But for the time being,
                     * we do not.
                     * else {
                     *     fractBits >>>= expShift+1-nFractBits;
                     *     fractBits *= long5pow[ nTinyBits ];
                     *     halfULP = long5pow[ nTinyBits ] >> (1+nSignificantBits-nFractBits);
                     *     developLongDigits( -nTinyBits, fractBits, halfULP );
                     *     return;
                     * }
                     */
                }
            }
            /*
             * This is the hard case. We are going to compute large positive
             * integers B and S and integer decExp, s.t.
             *      d = ( B / S ) * 10^decExp
             *      1 <= B / S < 10
             * Obvious choices are:
             *      decExp = floor( log10(d) )
             *      B      = d * 2^nTinyBits * 10^max( 0, -decExp )
             *      S      = 10^max( 0, decExp) * 2^nTinyBits
             * (noting that nTinyBits has already been forced to non-negative)
             * I am also going to compute a large positive integer
             *      M      = (1/2^nSignificantBits) * 2^nTinyBits * 10^max( 0, -decExp )
             * i.e. M is (1/2) of the ULP of d, scaled like B.
             * When we iterate through dividing B/S and picking off the
             * quotient bits, we will know when to stop when the remainder
             * is <= M.
             *
             * We keep track of powers of 2 and powers of 5.
             */

            /*
             * Estimate decimal exponent. (If it is small-ish,
             * we could double-check.)
             *
             * First, scale the mantissa bits such that 1 <= d2 < 2.
             * We are then going to estimate
             *          log10(d2) ~=~  (d2-1.5)/1.5 + log(1.5)
             * and so we can estimate
             *      log10(d) ~=~ log10(d2) + binExp * log10(2)
             * take the floor and call it decExp.
             * FIXME -- use more precise constants here. It costs no more.
             */
            double d2 = BitConversion.Int64BitsToDouble(
                expOne | (fractBits & ~fractHOB));
            decExp = (int)Math.Floor(
                (d2 - 1.5D) * 0.289529654D + 0.176091259 + (double)binExp * 0.301029995663981);
            int B2, B5; // powers of 2 and powers of 5, respectively, in B
            int S2, S5; // powers of 2 and powers of 5, respectively, in S
            int M2, M5; // powers of 2 and powers of 5, respectively, in M
            int Bbits; // binary digits needed to represent B, approx.
            int tenSbits; // binary digits needed to represent 10*S, approx.
            OldFDBigIntForTest Sval, Bval, Mval;

            B5 = Math.Max(0, -decExp);
            B2 = B5 + nTinyBits + binExp;

            S5 = Math.Max(0, decExp);
            S2 = S5 + nTinyBits;

            M5 = B5;
            M2 = B2 - nSignificantBits;

            /*
             * the long integer fractBits contains the (nFractBits) interesting
             * bits from the mantissa of d ( hidden 1 added if necessary) followed
             * by (expShift+1-nFractBits) zeros. In the interest of compactness,
             * I will shift out those zeros before turning fractBits into a
             * OldFDBigIntForTest. The resulting whole number will be
             *      d * 2^(nFractBits-1-binExp).
             */
            fractBits = fractBits.TripleShift(expShift + 1 - nFractBits);
            B2 -= nFractBits - 1;
            int common2factor = Math.Min(B2, S2);
            B2 -= common2factor;
            S2 -= common2factor;
            M2 -= common2factor;

            /*
             * HACK!! For exact powers of two, the next smallest number
             * is only half as far away as we think (because the meaning of
             * ULP changes at power-of-two bounds) for this reason, we
             * hack M2. Hope this works.
             */
            if (nFractBits == 1)
                M2 -= 1;

            if (M2 < 0)
            {
                // oops.
                // since we cannot scale M down far enough,
                // we must scale the other values up.
                B2 -= M2;
                S2 -= M2;
                M2 = 0;
            }
            /*
             * Construct, Scale, iterate.
             * Some day, we'll write a stopping test that takes
             * account of the asymmetry of the spacing of floating-point
             * numbers below perfect powers of 2
             * 26 Sept 96 is not that day.
             * So we use a symmetric test.
             */
            char[] digits = this.digits = new char[18];
            int ndigit = 0;
            bool low, high;
            long lowDigitDifference;
            int q;

            /*
             * Detect the special cases where all the numbers we are about
             * to compute will fit in int or long integers.
             * In these cases, we will avoid doing OldFDBigIntForTest arithmetic.
             * We use the same algorithms, except that we "normalize"
             * our OldFDBigIntForTests before iterating. This is to make division easier,
             * as it makes our fist guess (quotient of high-order words)
             * more accurate!
             *
             * Some day, we'll write a stopping test that takes
             * account of the asymmetry of the spacing of floating-point
             * numbers below perfect powers of 2
             * 26 Sept 96 is not that day.
             * So we use a symmetric test.
             */
            Bbits = nFractBits + B2 + ((B5 < n5bits.Length) ? n5bits[B5] : (B5 * 3));
            tenSbits = S2 + 1 + (((S5 + 1) < n5bits.Length) ? n5bits[(S5 + 1)] : ((S5 + 1) * 3));
            if (Bbits < 64 && tenSbits < 64)
            {
                if (Bbits < 32 && tenSbits < 32)
                {
                    // wa-hoo! They're all ints!
                    int b = ((int)fractBits * small5pow[B5]) << B2;
                    int s = small5pow[S5] << S2;
                    int m = small5pow[M5] << M2;
                    int tens = s * 10;
                    /*
                     * Unroll the first iteration. If our decExp estimate
                     * was too high, our first quotient will be zero. In this
                     * case, we discard it and decrement decExp.
                     */
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
                    /*
                     * HACK! Java spec sez that we always have at least
                     * one digit after the . in either F- or E-form output.
                     * Thus we will need more than one digit if we're using
                     * E-form
                     */
                    if (decExp < -3 || decExp >= 8)
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
                    long b = (fractBits * long5pow[B5]) << B2;
                    long s = long5pow[S5] << S2;
                    long m = long5pow[M5] << M2;
                    long tens = s * 10L;
                    /*
                     * Unroll the first iteration. If our decExp estimate
                     * was too high, our first quotient will be zero. In this
                     * case, we discard it and decrement decExp.
                     */
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
                    /*
                     * HACK! Java spec sez that we always have at least
                     * one digit after the . in either F- or E-form output.
                     * Thus we will need more than one digit if we're using
                     * E-form
                     */
                    if (decExp < -3 || decExp >= 8)
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
                OldFDBigIntForTest ZeroVal = new OldFDBigIntForTest(0);
                OldFDBigIntForTest tenSval;
                int shiftBias;

                /*
                 * We really must do OldFDBigIntForTest arithmetic.
                 * Fist, construct our OldFDBigIntForTest initial values.
                 */
                Bval = multPow52(new OldFDBigIntForTest(fractBits), B5, B2);
                Sval = constructPow52(S5, S2);
                Mval = constructPow52(M5, M2);


                // normalize so that division works better
                Bval.lshiftMe(shiftBias = Sval.normalizeMe());
                Mval.lshiftMe(shiftBias);
                tenSval = Sval.mult(10);
                /*
                 * Unroll the first iteration. If our decExp estimate
                 * was too high, our first quotient will be zero. In this
                 * case, we discard it and decrement decExp.
                 */
                ndigit = 0;
                q = Bval.quoRemIteration(Sval);
                Mval = Mval.mult(10);
                low = (Bval.cmp(Mval) < 0);
                high = (Bval.add(Mval).cmp(tenSval) > 0);
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
                /*
                 * HACK! Java spec sez that we always have at least
                 * one digit after the . in either F- or E-form output.
                 * Thus we will need more than one digit if we're using
                 * E-form
                 */
                if (decExp < -3 || decExp >= 8)
                {
                    high = low = false;
                }
                while (!low && !high)
                {
                    q = Bval.quoRemIteration(Sval);
                    Mval = Mval.mult(10);
                    Debug.Assert(q < 10, q.ToString());  // excessively large digit
                    low = (Bval.cmp(Mval) < 0);
                    high = (Bval.add(Mval).cmp(tenSval) > 0);
                    digits[ndigit++] = (char)('0' + q);
                }
                if (high && low)
                {
                    Bval.lshiftMe(1);
                    lowDigitDifference = Bval.cmp(tenSval);
                }
                else
                {
                    lowDigitDifference = 0L; // this here only for flow analysis!
                }
                exactDecimalConversion = (Bval.cmp(ZeroVal) == 0);
            }
            this.decExponent = decExp + 1;
            this.digits = digits;
            this.nDigits = ndigit;
            /*
             * Last digit gets rounded based on stopping condition.
             */
            if (high)
            {
                if (low)
                {
                    if (lowDigitDifference == 0L)
                    {
                        // it's a tie!
                        // choose based on which digits we like.
                        if ((digits[nDigits - 1] & 1) != 0) roundup();
                    }
                    else if (lowDigitDifference > 0)
                    {
                        roundup();
                    }
                }
                else
                {
                    roundup();
                }
            }
        }

        public bool decimalDigitsExact()
        {
            return exactDecimalConversion;
        }

        public String toString()
        {
            // most brain-dead version
            StringBuffer result = new StringBuffer(nDigits + 8);
            if (isNegative) { result.Append('-'); }
            if (isExceptional)
            {
                result.Append(digits, 0, nDigits);
            }
            else
            {
                result.Append("0.");
                result.Append(digits, 0, nDigits);
                result.Append('e');
                result.Append(decExponent);
            }
            return result.ToString();
        }

        public string toJavaFormatString()
        {
            char[] result = perThreadBuffer.Value;
            int i = getChars(result);
            return new String(result, 0, i);
        }

        private int getChars(char[] result)
        {
            Debug.Assert(nDigits <= 19, nDigits.ToString()); // generous bound on size of nDigits
            int i = 0;
            if (isNegative) { result[0] = '-'; i = 1; }
            if (isExceptional)
            {
                Array.Copy(digits, 0, result, i, nDigits);
                i += nDigits;
            }
            else
            {
                if (decExponent > 0 && decExponent < 8)
                {
                    // print digits.digits.
                    int charLength = Math.Min(nDigits, decExponent);
                    Array.Copy(digits, 0, result, i, charLength);
                    i += charLength;
                    if (charLength < decExponent)
                    {
                        charLength = decExponent - charLength;
                        Array.Copy(zero, 0, result, i, charLength);
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
                            Array.Copy(digits, charLength, result, i, t);
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
                        Array.Copy(zero, 0, result, i, -decExponent);
                        i -= decExponent;
                    }
                    Array.Copy(digits, 0, result, i, nDigits);
                    i += nDigits;
                }
                else
                {
                    result[i++] = digits[0];
                    result[i++] = '.';
                    if (nDigits > 1)
                    {
                        Array.Copy(digits, 1, result, i, nDigits - 1);
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
            }
            return i;
        }

        // Per-thread buffer for string/stringbuffer conversion
        private static ThreadLocal<char[]> perThreadBuffer = new ThreadLocal<char[]>(() => new char[26]);


        public void appendTo(IAppendable buf)
        {
            char[] result = perThreadBuffer.Value;
            int i = getChars(result);
            if (buf is StringBuilderCharSequence stringBuilderCharSequence)
                stringBuilderCharSequence.Append(result, 0, i);
            else if (buf is StringBuffer stringBuffer)
                stringBuffer.Append(result, 0, i);
            else
                buf.Append(result, 0, i);
            //assert false;
        }

        public static OldFloatingDecimalForTest readJavaFormatString(String @in)
        {
            bool isNegative = false;
            bool signSeen = false;
            int decExp;
            char c;

            //parseNumber:
            try
            {
                @in = @in.Trim(); // don't fool around with white space.
                                  // throws NullPointerException if null
                int l = @in.Length;
                if (l == 0) throw new FormatException("empty String");
                int i = 0;
                switch (c = @in[i])
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

                // Check for NaN and Infinity strings
                c = @in[i];
                if (c == 'N' || c == 'I')
                { // possible NaN or infinity
                    bool potentialNaN = false;
                    char[] targetChars = null;  // char array of "NaN" or "Infinity"

                    if (c == 'N')
                    {
                        targetChars = notANumber;
                        potentialNaN = true;
                    }
                    else
                    {
                        targetChars = infinity;
                    }

                    // compare Input string to "NaN" or "Infinity"
                    int j = 0;
                    while (i < l && j < targetChars.Length)
                    {
                        if (@in[i] == targetChars[j])
                        {
                            i++; j++;
                        }
                        else // something is amiss, throw exception
                             //break parseNumber;
                            throw new FormatException("For input string: \"" + @in + "\"");
                    }

                    // For the candidate string to be a NaN or infinity,
                    // all characters in input string and target char[]
                    // must be matched ==> j must equal targetChars.length
                    // and i must equal l
                    if ((j == targetChars.Length) && (i == l))
                    { // return NaN or infinity
                        return (potentialNaN ? new OldFloatingDecimalForTest(double.NaN) // NaN has no sign
                                : new OldFloatingDecimalForTest(isNegative ?
                                                      double.NegativeInfinity :
                                                      double.PositiveInfinity));
                    }
                    else
                    { // something went wrong, throw exception
                      //break parseNumber;
                        throw new FormatException("For input string: \"" + @in + "\"");
                    }

                }
                else if (c == '0')
                { // check for hexadecimal floating-point number
                    if (l > i + 1)
                    {
                        char ch = @in[i + 1];
                        if (ch == 'x' || ch == 'X') // possible hex string
                            return parseHexString(@in);
                    }
                }  // look for and process decimal floating-point string

                char[] digits = new char[l];
                int nDigits = 0;
                bool decSeen = false;
                int decPt = 0;
                int nLeadZero = 0;
                int nTrailZero = 0;
                //digitLoop:
                while (i < l)
                {
                    switch (c = @in[i])
                    {
                        case '0':
                            if (nDigits > 0)
                            {
                                nTrailZero += 1;
                            }
                            else
                            {
                                nLeadZero += 1;
                            }
                            break; // out of switch.
                        case '1':
                        case '2':
                        case '3':
                        case '4':
                        case '5':
                        case '6':
                        case '7':
                        case '8':
                        case '9':
                            while (nTrailZero > 0)
                            {
                                digits[nDigits++] = '0';
                                nTrailZero -= 1;
                            }
                            digits[nDigits++] = c;
                            break; // out of switch.
                        case '.':
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
                            break; // out of switch.
                        default:
                            //break; // digitLoop;
                            goto digitLoop_break;
                    }
                    i++;
                }
            digitLoop_break: { }
                /*
                 * At this point, we've scanned all the digits and decimal
                 * point we're going to see. Trim off leading and trailing
                 * zeros, which will just confuse us later, and adjust
                 * our initial decimal exponent accordingly.
                 * To review:
                 * we have seen i total characters.
                 * nLeadZero of them were zeros before any other digits.
                 * nTrailZero of them were zeros after any other digits.
                 * if ( decSeen ), then a . was seen after decPt characters
                 * ( including leading zeros which have been discarded )
                 * nDigits characters were neither lead nor trailing
                 * zeros, nor point
                 */
                /*
                 * special hack: if we saw no non-zero digits, then the
                 * answer is zero!
                 * Unfortunately, we feel honor-bound to keep parsing!
                 */
                if (nDigits == 0)
                {
                    digits = zero;
                    nDigits = 1;
                    if (nLeadZero == 0)
                    {
                        // we saw NO DIGITS AT ALL,
                        // not even a crummy 0!
                        // this is not allowed.
                        //break parseNumber; // go throw exception
                        throw new FormatException("For input string: \"" + @in + "\"");
                    }

                }

                /* Our initial exponent is decPt, adjusted by the number of
                 * discarded zeros. Or, if there was no decPt,
                 * then its just nDigits adjusted by discarded trailing zeros.
                 */
                if (decSeen)
                {
                    decExp = decPt - nLeadZero;
                }
                else
                {
                    decExp = nDigits + nTrailZero;
                }

                /*
                 * Look for 'e' or 'E' and an optionally signed integer.
                 */
                if ((i < l) && (((c = @in[i]) == 'e') || (c == 'E')))
                {
                    int expSign = 1;
                    int expVal = 0;
                    int reallyBig = int.MaxValue / 10;
                    bool expOverflow = false;
                    switch (@in[++i])
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
                    while (i < l)
                    {
                        if (expVal >= reallyBig)
                        {
                            // the next character will cause integer
                            // overflow.
                            expOverflow = true;
                        }
                        switch (c = @in[i++])
                        {
                            case '0':
                            case '1':
                            case '2':
                            case '3':
                            case '4':
                            case '5':
                            case '6':
                            case '7':
                            case '8':
                            case '9':
                                expVal = expVal * 10 + ((int)c - (int)'0');
                                continue;
                            default:
                                i--;           // back up.
                                //break; // expLoop;
                                goto expLoop_break; // stop parsing exponent.

                        }
                    }
                expLoop_break: { }
                    int expLimit = bigDecimalExponent + nDigits + nTrailZero;
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
                        //break parseNumber; // certainly bad
                        throw new FormatException("For input string: \"" + @in + "\"");
                }
                /*
                 * We parsed everything we could.
                 * If there are leftovers, then this is not good input!
                 */
                if (i < l &&
                    ((i != l - 1) ||
                    (@in[i] != 'f' &&
                     @in[i] != 'F' &&
                     @in[i] != 'd' &&
                     @in[i] != 'D')))
                {
                    //break parseNumber; // go throw exception
                    throw new FormatException("For input string: \"" + @in + "\"");
                }

                return new OldFloatingDecimalForTest(isNegative, decExp, digits, nDigits, false);
            }
            catch (IndexOutOfRangeException e) { }
            throw new FormatException("For input string: \"" + @in + "\"");
        }

        /*
         * Take a FloatingDecimal, which we presumably just scanned in,
         * and find out what its value is, as a double.
         *
         * AS A SIDE EFFECT, SET roundDir TO INDICATE PREFERRED
         * ROUNDING DIRECTION in case the result is really destined
         * for a single-precision float.
         */

        public /*strictfp*/ double doubleValue()
        {
            int kDigits = Math.Min(nDigits, maxDecimalDigits + 1);
            long lValue;
            double dValue;
            double rValue, tValue;

            // First, check for NaN and Infinity values
            if (digits == infinity || digits == notANumber)
            {
                if (digits == notANumber)
                    return double.NaN;
                else
                    return (isNegative ? double.NegativeInfinity : double.PositiveInfinity);
            }
            else
            {
                if (mustSetRoundDir)
                {
                    roundDir = 0;
                }
                /*
                 * convert the lead kDigits to a long integer.
                 */
                // (special performance hack: start to do it using int)
                int iValue = (int)digits[0] - (int)'0';
                int iDigits = Math.Min(kDigits, intDecimalDigits);
                for (int i = 1; i < iDigits; i++)
                {
                    iValue = iValue * 10 + (int)digits[i] - (int)'0';
                }
                lValue = (long)iValue;
                for (int i = iDigits; i < kDigits; i++)
                {
                    lValue = lValue * 10L + (long)((int)digits[i] - (int)'0');
                }
                dValue = (double)lValue;
                int exp = decExponent - kDigits;
                /*
                 * lValue now contains a long integer with the value of
                 * the first kDigits digits of the number.
                 * dValue contains the (double) of the same.
                 */

                if (nDigits <= maxDecimalDigits)
                {
                    /*
                     * possibly an easy case.
                     * We know that the digits can be represented
                     * exactly. And if the exponent isn't too outrageous,
                     * the whole thing can be done with one operation,
                     * thus one rounding error.
                     * Note that all our constructors trim all leading and
                     * trailing zeros, so simple values (including zero)
                     * will always end up here
                     */
                    if (exp == 0 || dValue == 0.0)
                        return (isNegative) ? -dValue : dValue; // small floating integer
                    else if (exp >= 0)
                    {
                        if (exp <= maxSmallTen)
                        {
                            /*
                             * Can get the answer with one operation,
                             * thus one roundoff.
                             */
                            rValue = dValue * small10pow[exp];
                            if (mustSetRoundDir)
                            {
                                tValue = rValue / small10pow[exp];
                                roundDir = (tValue == dValue) ? 0
                                    : (tValue < dValue) ? 1
                                    : -1;
                            }
                            return (isNegative) ? -rValue : rValue;
                        }
                        int slop = maxDecimalDigits - kDigits;
                        if (exp <= maxSmallTen + slop)
                        {
                            /*
                             * We can multiply dValue by 10^(slop)
                             * and it is still "small" and exact.
                             * Then we can multiply by 10^(exp-slop)
                             * with one rounding.
                             */
                            dValue *= small10pow[slop];
                            rValue = dValue * small10pow[exp - slop];

                            if (mustSetRoundDir)
                            {
                                tValue = rValue / small10pow[exp - slop];
                                roundDir = (tValue == dValue) ? 0
                                    : (tValue < dValue) ? 1
                                    : -1;
                            }
                            return (isNegative) ? -rValue : rValue;
                        }
                        /*
                         * Else we have a hard case with a positive exp.
                         */
                    }
                    else
                    {
                        if (exp >= -maxSmallTen)
                        {
                            /*
                             * Can get the answer in one division.
                             */
                            rValue = dValue / small10pow[-exp];
                            tValue = rValue * small10pow[-exp];
                            if (mustSetRoundDir)
                            {
                                roundDir = (tValue == dValue) ? 0
                                    : (tValue < dValue) ? 1
                                    : -1;
                            }
                            return (isNegative) ? -rValue : rValue;
                        }
                        /*
                         * Else we have a hard case with a negative exp.
                         */
                    }
                }

                /*
                 * Harder cases:
                 * The sum of digits plus exponent is greater than
                 * what we think we can do with one error.
                 *
                 * Start by approximating the right answer by,
                 * naively, scaling by powers of 10.
                 */
                if (exp > 0)
                {
                    if (decExponent > maxDecimalExponent + 1)
                    {
                        /*
                         * Lets face it. This is going to be
                         * Infinity. Cut to the chase.
                         */
                        return (isNegative) ? double.NegativeInfinity : double.PositiveInfinity;
                    }
                    if ((exp & 15) != 0)
                    {
                        dValue *= small10pow[exp & 15];
                    }
                    if ((exp >>= 4) != 0)
                    {
                        int j;
                        for (j = 0; exp > 1; j++, exp >>= 1)
                        {
                            if ((exp & 1) != 0)
                                dValue *= big10pow[j];
                        }
                        /*
                         * The reason for the weird exp > 1 condition
                         * in the above loop was so that the last multiply
                         * would get unrolled. We handle it here.
                         * It could overflow.
                         */
                        double t = dValue * big10pow[j];
                        if (double.IsInfinity(t))
                        {
                            /*
                             * It did overflow.
                             * Look more closely at the result.
                             * If the exponent is just one too large,
                             * then use the maximum finite as our estimate
                             * value. Else call the result infinity
                             * and punt it.
                             * ( I presume this could happen because
                             * rounding forces the result here to be
                             * an ULP or two larger than
                             * Double.MAX_VALUE ).
                             */
                            t = dValue / 2.0;
                            t *= big10pow[j];
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
                    if (decExponent < minDecimalExponent - 1)
                    {
                        /*
                         * Lets face it. This is going to be
                         * zero. Cut to the chase.
                         */
                        return (isNegative) ? -0.0 : 0.0;
                    }
                    if ((exp & 15) != 0)
                    {
                        dValue /= small10pow[exp & 15];
                    }
                    if ((exp >>= 4) != 0)
                    {
                        int j;
                        for (j = 0; exp > 1; j++, exp >>= 1)
                        {
                            if ((exp & 1) != 0)
                                dValue *= tiny10pow[j];
                        }
                        /*
                         * The reason for the weird exp > 1 condition
                         * in the above loop was so that the last multiply
                         * would get unrolled. We handle it here.
                         * It could underflow.
                         */
                        double t = dValue * tiny10pow[j];
                        if (t == 0.0)
                        {
                            /*
                             * It did underflow.
                             * Look more closely at the result.
                             * If the exponent is just one too small,
                             * then use the minimum finite as our estimate
                             * value. Else call the result 0.0
                             * and punt it.
                             * ( I presume this could happen because
                             * rounding forces the result here to be
                             * an ULP or two less than
                             * Double.MIN_VALUE ).
                             */
                            t = dValue * 2.0;
                            t *= tiny10pow[j];
                            if (t == 0.0)
                            {
                                return (isNegative) ? -0.0 : 0.0;
                            }
                            t = double.MinValue;
                        }
                        dValue = t;
                    }
                }

                /*
                 * dValue is now approximately the result.
                 * The hard part is adjusting it, by comparison
                 * with OldFDBigIntForTest arithmetic.
                 * Formulate the EXACT big-number result as
                 * bigD0 * 10^exp
                 */
                OldFDBigIntForTest bigD0 = new OldFDBigIntForTest(lValue, digits, kDigits, nDigits);
                exp = decExponent - nDigits;

                //correctionLoop:
                while (true)
                {
                    /* AS A SIDE EFFECT, THIS METHOD WILL SET THE INSTANCE VARIABLES
                     * bigIntExp and bigIntNBits
                     */
                    OldFDBigIntForTest bigB = doubleToBigInt(dValue);

                    /*
                     * Scale bigD, bigB appropriately for
                     * big-integer operations.
                     * Naively, we multiply by powers of ten
                     * and powers of two. What we actually do
                     * is keep track of the powers of 5 and
                     * powers of 2 we would use, then factor out
                     * common divisors before doing the work.
                     */
                    int B2, B5; // powers of 2, 5 in bigB
                    int D2, D5; // powers of 2, 5 in bigD
                    int Ulp2;   // powers of 2 in halfUlp.
                    if (exp >= 0)
                    {
                        B2 = B5 = 0;
                        D2 = D5 = exp;
                    }
                    else
                    {
                        B2 = B5 = -exp;
                        D2 = D5 = 0;
                    }
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
                    if (bigIntExp + bigIntNBits <= -expBias + 1)
                    {
                        // This is going to be a denormalized number
                        // (if not actually zero).
                        // half an ULP is at 2^-(expBias+expShift+1)
                        hulpbias = bigIntExp + expBias + expShift;
                    }
                    else
                    {
                        hulpbias = expShift + 2 - bigIntNBits;
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
                    bigB = multPow52(bigB, B5, B2);
                    OldFDBigIntForTest bigD = multPow52(new OldFDBigIntForTest(bigD0), D5, D2);
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
                    OldFDBigIntForTest diff;
                    int cmpResult;
                    bool overvalue;
                    if ((cmpResult = bigB.cmp(bigD)) > 0)
                    {
                        overvalue = true; // our candidate is too big.
                        diff = bigB.sub(bigD);
                        if ((bigIntNBits == 1) && (bigIntExp > -expBias + 1))
                        {
                            // candidate is a normalized exact power of 2 and
                            // is too big. We will be subtracting.
                            // For our purposes, ulp is the ulp of the
                            // next smaller range.
                            Ulp2 -= 1;
                            if (Ulp2 < 0)
                            {
                                // rats. Cannot de-scale ulp this far.
                                // must scale diff in other direction.
                                Ulp2 = 0;
                                diff.lshiftMe(1);
                            }
                        }
                    }
                    else if (cmpResult < 0)
                    {
                        overvalue = false; // our candidate is too small.
                        diff = bigD.sub(bigB);
                    }
                    else
                    {
                        // the candidate is exactly right!
                        // this happens with surprising frequency
                        break;// correctionLoop;
                    }
                    OldFDBigIntForTest halfUlp = constructPow52(B5, Ulp2);
                    if ((cmpResult = diff.cmp(halfUlp)) < 0)
                    {
                        // difference is small.
                        // this is close enough
                        if (mustSetRoundDir)
                        {
                            roundDir = overvalue ? -1 : 1;
                        }
                        break;// correctionLoop;
                    }
                    else if (cmpResult == 0)
                    {
                        // difference is exactly half an ULP
                        // round to some other value maybe, then finish
                        dValue += 0.5 * ulp(dValue, overvalue);
                        // should check for bigIntNBits == 1 here??
                        if (mustSetRoundDir)
                        {
                            roundDir = overvalue ? -1 : 1;
                        }
                        break;// correctionLoop;
                    }
                    else
                    {
                        // difference is non-trivial.
                        // could scale addend by ratio of difference to
                        // halfUlp here, if we bothered to compute that difference.
                        // Most of the time ( I hope ) it is about 1 anyway.
                        dValue += ulp(dValue, overvalue);
                        if (dValue == 0.0 || dValue == double.PositiveInfinity)
                            break;// correctionLoop; // oops. Fell off end of range.
                        continue; // try again.
                    }

                }
                return (isNegative) ? -dValue : dValue;
            }
        }

        /*
         * Take a FloatingDecimal, which we presumably just scanned in,
         * and find out what its value is, as a float.
         * This is distinct from doubleValue() to avoid the extremely
         * unlikely case of a double rounding error, wherein the conversion
         * to double has one rounding error, and the conversion of that double
         * to a float has another rounding error, IN THE WRONG DIRECTION,
         * ( because of the preference to a zero low-order bit ).
         */

        public /*strictfp*/ float floatValue()
        {
            int kDigits = Math.Min(nDigits, singleMaxDecimalDigits + 1);
            int iValue;
            float fValue;

            // First, check for NaN and Infinity values
            if (digits == infinity || digits == notANumber)
            {
                if (digits == notANumber)
                    return float.NaN;
                else
                    return (isNegative ? float.NegativeInfinity : float.PositiveInfinity);
            }
            else
            {
                /*
                 * convert the lead kDigits to an integer.
                 */
                iValue = (int)digits[0] - (int)'0';
                for (int i = 1; i < kDigits; i++)
                {
                    iValue = iValue * 10 + (int)digits[i] - (int)'0';
                }
                fValue = (float)iValue;
                int exp = decExponent - kDigits;
                /*
                 * iValue now contains an integer with the value of
                 * the first kDigits digits of the number.
                 * fValue contains the (float) of the same.
                 */

                if (nDigits <= singleMaxDecimalDigits)
                {
                    /*
                     * possibly an easy case.
                     * We know that the digits can be represented
                     * exactly. And if the exponent isn't too outrageous,
                     * the whole thing can be done with one operation,
                     * thus one rounding error.
                     * Note that all our constructors trim all leading and
                     * trailing zeros, so simple values (including zero)
                     * will always end up here.
                     */
                    if (exp == 0 || fValue == 0.0f)
                        return (isNegative) ? -fValue : fValue; // small floating integer
                    else if (exp >= 0)
                    {
                        if (exp <= singleMaxSmallTen)
                        {
                            /*
                             * Can get the answer with one operation,
                             * thus one roundoff.
                             */
                            fValue *= singleSmall10pow[exp];
                            return (isNegative) ? -fValue : fValue;
                        }
                        int slop = singleMaxDecimalDigits - kDigits;
                        if (exp <= singleMaxSmallTen + slop)
                        {
                            /*
                             * We can multiply dValue by 10^(slop)
                             * and it is still "small" and exact.
                             * Then we can multiply by 10^(exp-slop)
                             * with one rounding.
                             */
                            fValue *= singleSmall10pow[slop];
                            fValue *= singleSmall10pow[exp - slop];
                            return (isNegative) ? -fValue : fValue;
                        }
                        /*
                         * Else we have a hard case with a positive exp.
                         */
                    }
                    else
                    {
                        if (exp >= -singleMaxSmallTen)
                        {
                            /*
                             * Can get the answer in one division.
                             */
                            fValue /= singleSmall10pow[-exp];
                            return (isNegative) ? -fValue : fValue;
                        }
                        /*
                         * Else we have a hard case with a negative exp.
                         */
                    }
                }
                else if ((decExponent >= nDigits) && (nDigits + decExponent <= maxDecimalDigits))
                {
                    /*
                     * In double-precision, this is an exact floating integer.
                     * So we can compute to double, then shorten to float
                     * with one round, and get the right answer.
                     *
                     * First, finish accumulating digits.
                     * Then convert that integer to a double, multiply
                     * by the appropriate power of ten, and convert to float.
                     */
                    long lValue = (long)iValue;
                    for (int i = kDigits; i < nDigits; i++)
                    {
                        lValue = lValue * 10L + (long)((int)digits[i] - (int)'0');
                    }
                    double dValue2 = (double)lValue;
                    exp = decExponent - nDigits;
                    dValue2 *= small10pow[exp];
                    fValue = (float)dValue2;
                    return (isNegative) ? -fValue : fValue;

                }
                /*
                 * Harder cases:
                 * The sum of digits plus exponent is greater than
                 * what we think we can do with one error.
                 *
                 * Start by weeding out obviously out-of-range
                 * results, then convert to double and go to
                 * common hard-case code.
                 */
                if (decExponent > singleMaxDecimalExponent + 1)
                {
                    /*
                     * Lets face it. This is going to be
                     * Infinity. Cut to the chase.
                     */
                    return (isNegative) ? float.NegativeInfinity : float.PositiveInfinity;
                }
                else if (decExponent < singleMinDecimalExponent - 1)
                {
                    /*
                     * Lets face it. This is going to be
                     * zero. Cut to the chase.
                     */
                    return (isNegative) ? -0.0f : 0.0f;
                }

                /*
                 * Here, we do 'way too much work, but throwing away
                 * our partial results, and going and doing the whole
                 * thing as double, then throwing away half the bits that computes
                 * when we convert back to float.
                 *
                 * The alternative is to reproduce the whole multiple-precision
                 * algorithm for float precision, or to try to parameterize it
                 * for common usage. The former will take about 400 lines of code,
                 * and the latter I tried without success. Thus the semi-hack
                 * answer here.
                 */
                mustSetRoundDir = !fromHex;
                double dValue = doubleValue();
                return stickyRound(dValue);
            }
        }


        /*
         * All the positive powers of 10 that can be
         * represented exactly in double/float.
         */
        private static readonly double[] small10pow = {
        1.0e0,
        1.0e1, 1.0e2, 1.0e3, 1.0e4, 1.0e5,
        1.0e6, 1.0e7, 1.0e8, 1.0e9, 1.0e10,
        1.0e11, 1.0e12, 1.0e13, 1.0e14, 1.0e15,
        1.0e16, 1.0e17, 1.0e18, 1.0e19, 1.0e20,
        1.0e21, 1.0e22
    };

        private static readonly float[] singleSmall10pow = {
        1.0e0f,
        1.0e1f, 1.0e2f, 1.0e3f, 1.0e4f, 1.0e5f,
        1.0e6f, 1.0e7f, 1.0e8f, 1.0e9f, 1.0e10f
    };

        private static readonly double[] big10pow = {
        1e16, 1e32, 1e64, 1e128, 1e256 };
        private static readonly double[] tiny10pow = {
        1e-16, 1e-32, 1e-64, 1e-128, 1e-256 };

        private static readonly int maxSmallTen = small10pow.Length - 1;
        private static readonly int singleMaxSmallTen = singleSmall10pow.Length - 1;

        private static readonly int[] small5pow = {
        1,
        5,
        5*5,
        5*5*5,
        5*5*5*5,
        5*5*5*5*5,
        5*5*5*5*5*5,
        5*5*5*5*5*5*5,
        5*5*5*5*5*5*5*5,
        5*5*5*5*5*5*5*5*5,
        5*5*5*5*5*5*5*5*5*5,
        5*5*5*5*5*5*5*5*5*5*5,
        5*5*5*5*5*5*5*5*5*5*5*5,
        5*5*5*5*5*5*5*5*5*5*5*5*5
    };


        private static readonly long[] long5pow = {
        1L,
        5L,
        5L*5,
        5L*5*5,
        5L*5*5*5,
        5L*5*5*5*5,
        5L*5*5*5*5*5,
        5L*5*5*5*5*5*5,
        5L*5*5*5*5*5*5*5,
        5L*5*5*5*5*5*5*5*5,
        5L*5*5*5*5*5*5*5*5*5,
        5L*5*5*5*5*5*5*5*5*5*5,
        5L*5*5*5*5*5*5*5*5*5*5*5,
        5L*5*5*5*5*5*5*5*5*5*5*5*5,
        5L*5*5*5*5*5*5*5*5*5*5*5*5*5,
        5L*5*5*5*5*5*5*5*5*5*5*5*5*5*5,
        5L*5*5*5*5*5*5*5*5*5*5*5*5*5*5*5,
        5L*5*5*5*5*5*5*5*5*5*5*5*5*5*5*5*5,
        5L*5*5*5*5*5*5*5*5*5*5*5*5*5*5*5*5*5,
        5L*5*5*5*5*5*5*5*5*5*5*5*5*5*5*5*5*5*5,
        5L*5*5*5*5*5*5*5*5*5*5*5*5*5*5*5*5*5*5*5,
        5L*5*5*5*5*5*5*5*5*5*5*5*5*5*5*5*5*5*5*5*5,
        5L*5*5*5*5*5*5*5*5*5*5*5*5*5*5*5*5*5*5*5*5*5,
        5L*5*5*5*5*5*5*5*5*5*5*5*5*5*5*5*5*5*5*5*5*5*5,
        5L*5*5*5*5*5*5*5*5*5*5*5*5*5*5*5*5*5*5*5*5*5*5*5,
        5L*5*5*5*5*5*5*5*5*5*5*5*5*5*5*5*5*5*5*5*5*5*5*5*5,
        5L*5*5*5*5*5*5*5*5*5*5*5*5*5*5*5*5*5*5*5*5*5*5*5*5*5,
    };

        // approximately ceil( log2( long5pow[i] ) )
        private static readonly int[] n5bits = {
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

        private static readonly char[] infinity = { 'I', 'n', 'f', 'i', 'n', 'i', 't', 'y' };
        private static readonly char[] notANumber = { 'N', 'a', 'N' };
        private static readonly char[] zero = { '0', '0', '0', '0', '0', '0', '0', '0' };


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
        internal static readonly Regex hexFloatPattern = new Regex(
            //1           234                      56                   7                      8      9
            "([-+])?0[xX](((" + XDIGIT + "+)\\.?)|((" + XDIGIT + "*)\\.(" + XDIGIT + "+)))[pP]([-+])?(" + DIGIT + "+)[fFdD]?",
            RegexOptions.Compiled);

        /*
         * Convert string s to a suitable floating decimal; uses the
         * double constructor and set the roundDir variable appropriately
         * in case the value is later converted to a float.
         */
        static OldFloatingDecimalForTest parseHexString(String s)
        {
            // Verify string is a member of the hexadecimal floating-point
            // string language.
            Match m = hexFloatPattern.Match(s);
            bool validInput = m.Success;

            if (!validInput)
            {
                // Input does not match pattern
                throw new FormatException("For input string: \"" + s + "\"");
            }
            else
            { // validInput
                /*
                 * We must isolate the sign, significand, and exponent
                 * fields.  The sign value is straightforward.  Since
                 * floating-point numbers are stored with a normalized
                 * representation, the significand and exponent are
                 * interrelated.
                 *
                 * After extracting the sign, we normalized the
                 * significand as a hexadecimal value, calculating an
                 * exponent adjust for any shifts made during
                 * normalization.  If the significand is zero, the
                 * exponent doesn't need to be examined since the output
                 * will be zero.
                 *
                 * Next the exponent in the input string is extracted.
                 * Afterwards, the significand is normalized as a *binary*
                 * value and the input value's normalized exponent can be
                 * computed.  The significand bits are copied into a
                 * double significand; if the string has more logical bits
                 * than can fit in a double, the extra bits affect the
                 * round and sticky bits which are used to round the final
                 * value.
                 */

                //  Extract significand sign
                Group group1 = m.Groups[1];
                double sign = ((group1 == null) || group1.Value.Equals("+")) ? 1.0 : -1.0;


                //  Extract Significand magnitude
                /*
                 * Based on the form of the significand, calculate how the
                 * binary exponent needs to be adjusted to create a
                 * normalized *hexadecimal* floating-point number; that
                 * is, a number where there is one nonzero hex digit to
                 * the left of the (hexa)decimal point.  Since we are
                 * adjusting a binary, not hexadecimal exponent, the
                 * exponent is adjusted by a multiple of 4.
                 *
                 * There are a number of significand scenarios to consider;
                 * letters are used in indicate nonzero digits:
                 *
                 * 1. 000xxxx       =>      x.xxx   normalized
                 *    increase exponent by (number of x's - 1)*4
                 *
                 * 2. 000xxx.yyyy =>        x.xxyyyy        normalized
                 *    increase exponent by (number of x's - 1)*4
                 *
                 * 3. .000yyy  =>   y.yy    normalized
                 *    decrease exponent by (number of zeros + 1)*4
                 *
                 * 4. 000.00000yyy => y.yy normalized
                 *    decrease exponent by (number of zeros to right of point + 1)*4
                 *
                 * If the significand is exactly zero, return a properly
                 * signed zero.
                 */

                String significandString = null;
                int signifLength = 0;
                int exponentAdjust = 0;
                {
                    int leftDigits = 0; // number of meaningful digits to
                                        // left of "decimal" point
                                        // (leading zeros stripped)
                    int rightDigits = 0; // number of digits to right of
                                         // "decimal" point; leading zeros
                                         // must always be accounted for
                    /*
                     * The significand is made up of either
                     *
                     * 1. group 4 entirely (integer portion only)
                     *
                     * OR
                     *
                     * 2. the fractional portion from group 7 plus any
                     * (optional) integer portions from group 6.
                     */
                    Group group4;
                    if ((group4 = m.Groups[4]) != null)
                    {  // Integer-only significand
                       // Leading zeros never matter on the integer portion
                        significandString = stripLeadingZeros(group4.Value);
                        leftDigits = significandString.Length;
                    }
                    else
                    {
                        // Group 6 is the optional integer; leading zeros
                        // never matter on the integer portion
                        String group6 = stripLeadingZeros(m.Groups[6].Value);
                        leftDigits = group6.Length;

                        // fraction
                        String group7 = m.Groups[7].Value;
                        rightDigits = group7.Length;

                        // Turn "integer.fraction" into "integer"+"fraction"
                        significandString =
                            ((group6 == null) ? "" : group6) + // is the null
                                                               // check necessary?
                            group7;
                    }

                    significandString = stripLeadingZeros(significandString);
                    signifLength = significandString.Length;

                    /*
                     * Adjust exponent as described above
                     */
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
                        return new OldFloatingDecimalForTest(sign * 0.0);
                    }
                }

                //  Extract Exponent
                /*
                 * Use an int to read in the exponent value; this should
                 * provide more than sufficient range for non-contrived
                 * inputs.  If reading the exponent in as an int does
                 * overflow, examine the sign of the exponent and
                 * significand to determine what to do.
                 */
                Group group8 = m.Groups[8];
                bool positiveExponent = (group8 == null) || group8.Value.Equals("+");
                long unsignedRawExponent;
                try
                {
                    unsignedRawExponent = Int32.ParseInt32(m.Groups[9].Value); // J2N TODO: TryParse
                }
                catch (FormatException e)
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
                    return new OldFloatingDecimalForTest(sign * (positiveExponent ?
                                                       double.PositiveInfinity : 0.0));
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
                int bitsCopied = 0;
                int nextShift = 0;
                long significand = 0L;
                // First iteration is different, since we only copy
                // from the leading significand bit; one more exponent
                // adjust will be needed...

                // IMPORTANT: make leadingDigit a long to avoid
                // surprising shift semantics!
                long leadingDigit = getHexDigit(significandString, 0);

                /*
                 * Left shift the leading digit (53 - (bit position of
                 * leading 1 in digit)); this sets the top bit of the
                 * significand to 1.  The nextShift value is adjusted
                 * to take into account the number of bit positions of
                 * the leadingDigit actually used.  Finally, the
                 * exponent is adjusted to normalize the significand
                 * as a binary value, not just a hex value.
                 */
                if (leadingDigit == 1)
                {
                    significand |= leadingDigit << 52;
                    nextShift = 52 - 4;
                    /* exponent += 0 */
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


                /*
                 * Now the exponent variable is equal to the normalized
                 * binary exponent.  Code below will make representation
                 * adjustments if the exponent is incremented after
                 * rounding (includes overflows to infinity) or if the
                 * result is subnormal.
                 */

                // Copy digit into significand until the significand can't
                // hold another full hex digit or there are no more input
                // hex digits.
                int i = 0;
                for (i = 1;
                    i < signifLength && nextShift >= 0;
                    i++)
                {
                    long currentDigit = getHexDigit(significandString, i);
                    significand |= (currentDigit << nextShift);
                    nextShift -= 4;
                }

                // After the above loop, the bulk of the string is copied.
                // Now, we must copy any partial hex digits into the
                // significand AND compute the round bit and start computing
                // sticky bit.

                if (i < signifLength)
                { // at least one hex input digit exists
                    long currentDigit = getHexDigit(significandString, i);

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
                        currentDigit = getHexDigit(significandString, i);
                        sticky = sticky || (currentDigit != 0);
                        i++;
                    }

                }
                // else all of string was seen, round and sticky are
                // correct as false.


                // Check for overflow and update exponent accordingly.

                if (exponent > FloatingDecimal.DoubleConsts.MAX_EXPONENT)
                {         // Infinite result
                          // overflow to properly signed infinity
                    return new OldFloatingDecimalForTest(sign * double.PositiveInfinity);
                }
                else
                {  // Finite return value
                    if (exponent <= FloatingDecimal.DoubleConsts.MAX_EXPONENT && // (Usually) normal result
                        exponent >= FloatingDecimal.DoubleConsts.MIN_EXPONENT)
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
                                         (long)FloatingDecimal.DoubleConsts.EXP_BIAS) <<
                                         (FloatingDecimal.DoubleConsts.SIGNIFICAND_WIDTH - 1))
                                       & FloatingDecimal.DoubleConsts.EXP_BIT_MASK) |
                            (FloatingDecimal.DoubleConsts.SIGNIF_BIT_MASK & significand);

                    }
                    else
                    {  // Subnormal or zero
                       // (exponent < DoubleConsts.MIN_EXPONENT)

                        if (exponent < (FloatingDecimal.DoubleConsts.MIN_SUB_EXPONENT - 1))
                        {
                            // No way to round back to nonzero value
                            // regardless of significand if the exponent is
                            // less than -1075.
                            return new OldFloatingDecimalForTest(sign * 0.0);
                        }
                        else
                        { //  -1075 <= exponent <= MIN_EXPONENT -1 = -1023
                            /*
                             * Find bit position to round to; recompute
                             * round and sticky bits, and shift
                             * significand right appropriately.
                             */

                            sticky = sticky || round;
                            round = false;

                            // Number of bits of significand to preserve is
                            // exponent - abs_min_exp +1
                            // check:
                            // -1075 +1074 + 1 = 0
                            // -1023 +1074 + 1 = 52

                            int bitsDiscarded = 53 -
                                ((int)exponent - FloatingDecimal.DoubleConsts.MIN_SUB_EXPONENT + 1);
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

                            significand = ((((long)(FloatingDecimal.DoubleConsts.MIN_EXPONENT - 1) + // subnorm exp.
                                              (long)FloatingDecimal.DoubleConsts.EXP_BIAS) <<
                                             (FloatingDecimal.DoubleConsts.SIGNIFICAND_WIDTH - 1))
                                           & FloatingDecimal.DoubleConsts.EXP_BIT_MASK) |
                                (FloatingDecimal.DoubleConsts.SIGNIF_BIT_MASK & significand);
                        }
                    }

                    // The significand variable now contains the currently
                    // appropriate exponent bits too.

                    /*
                     * Determine if significand should be incremented;
                     * making this determination depends on the least
                     * significant bit and the round and sticky bits.
                     *
                     * Round to nearest even rounding table, adapted from
                     * table 4.7 in "Computer Arithmetic" by IsraelKoren.
                     * The digit to the left of the "decimal" point is the
                     * least significant bit, the digits to the right of
                     * the point are the round and sticky bits
                     *
                     * Number       Round(x)
                     * x0.00        x0.
                     * x0.01        x0.
                     * x0.10        x0.
                     * x0.11        x1. = x0. +1
                     * x1.00        x1.
                     * x1.01        x1.
                     * x1.10        x1. + 1
                     * x1.11        x1. + 1
                     */
                    bool incremented = false;
                    bool leastZero = ((significand & 1L) == 0L);
                    if ((leastZero && round && sticky) ||
                        ((!leastZero) && round))
                    {
                        incremented = true;
                        significand++;
                    }

                    OldFloatingDecimalForTest fd = new OldFloatingDecimalForTest(MathExtensions.CopySign(
                                                                  BitConversion.Int64BitsToDouble(significand),
                                                                  sign));

                    /*
                     * Set roundingDir variable field of fd properly so
                     * that the input string can be properly rounded to a
                     * float value.  There are two cases to consider:
                     *
                     * 1. rounding to double discards sticky bit
                     * information that would change the result of a float
                     * rounding (near halfway case between two floats)
                     *
                     * 2. rounding to double rounds up when rounding up
                     * would not occur when rounding to float.
                     *
                     * For former case only needs to be considered when
                     * the bits rounded away when casting to float are all
                     * zero; otherwise, float round bit is properly set
                     * and sticky will already be true.
                     *
                     * The lower exponent bound for the code below is the
                     * minimum (normalized) subnormal exponent - 1 since a
                     * value with that exponent can round up to the
                     * minimum subnormal value and the sticky bit
                     * information must be preserved (i.e. case 1).
                     */
                    if ((exponent >= FloatingDecimal.FloatConsts.MIN_SUB_EXPONENT - 1) &&
                        (exponent <= FloatingDecimal.FloatConsts.MAX_EXPONENT))
                    {
                        // Outside above exponent range, the float value
                        // will be zero or infinity.

                        /*
                         * If the low-order 28 bits of a rounded double
                         * significand are 0, the double could be a
                         * half-way case for a rounding to float.  If the
                         * double value is a half-way case, the double
                         * significand may have to be modified to round
                         * the the right float value (see the stickyRound
                         * method).  If the rounding to double has lost
                         * what would be float sticky bit information, the
                         * double significand must be incremented.  If the
                         * double value's significand was itself
                         * incremented, the float value may end up too
                         * large so the increment should be undone.
                         */
                        if ((significand & 0xfffffffL) == 0x0L)
                        {
                            // For negative values, the sign of the
                            // roundDir is the same as for positive values
                            // since adding 1 increasing the significand's
                            // magnitude and subtracting 1 decreases the
                            // significand's magnitude.  If neither round
                            // nor sticky is true, the double value is
                            // exact and no adjustment is required for a
                            // proper float rounding.
                            if (round || sticky)
                            {
                                if (leastZero)
                                { // prerounding lsb is 0
                                  // If round and sticky were both true,
                                  // and the least significant
                                  // significand bit were 0, the rounded
                                  // significand would not have its
                                  // low-order bits be zero.  Therefore,
                                  // we only need to adjust the
                                  // significand if round XOR sticky is
                                  // true.
                                    if (round ^ sticky)
                                    {
                                        fd.roundDir = 1;
                                    }
                                }
                                else
                                { // prerounding lsb is 1
                                  // If the prerounding lsb is 1 and the
                                  // resulting significand has its
                                  // low-order bits zero, the significand
                                  // was incremented.  Here, we undo the
                                  // increment, which will ensure the
                                  // right guard and sticky bits for the
                                  // float rounding.
                                    if (round)
                                        fd.roundDir = -1;
                                }
                            }
                        }
                    }

                    fd.fromHex = true;
                    return fd;
                }
            }
        }

        private static readonly Regex LeadingZeros = new Regex("^0+", RegexOptions.Compiled);

        /**
         * Return <code>s</code> with any leading zeros removed.
         */
        static String stripLeadingZeros(String s)
        {
            //return s.replaceFirst("^0+", "");
            return LeadingZeros.Replace(s, "", 1);
        }

        /**
         * Extract a hexadecimal digit from position <code>position</code>
         * of string <code>s</code>.
         */
        static int getHexDigit(String s, int position)
        {
            int value = Character.Digit(s[position], 16);
            if (value <= -1 || value >= 16)
            {
                throw new InvalidOperationException("Unexpected failure of digit conversion of " +
                                         s[position]);
            }
            return value;
        }
    }
}
