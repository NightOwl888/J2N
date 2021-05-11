using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace J2N.Numerics
{
    internal sealed class NumberConverter
    {
        private int setCount; // number of times u and k have been gotten

        private int getCount; // number of times u and k have been set

        private readonly int[] uArray = new int[64];

        private int firstK;

        private readonly static double invLogOfTenBaseTwo = Math.Log(2.0)
                / Math.Log(10.0);

        private readonly static long[] TEN_TO_THE = LoadTenToThe();

        private static long[] LoadTenToThe()
        {
            var result = new long[20];
            result[0] = 1L;
            for (int i = 1; i < result.Length; ++i)
            {
                long previous = result[i - 1];
                result[i] = (previous << 1) + (previous << 3);
            }
            return result;
        }


        private static NumberConverter GetConverter()
        {
            return new NumberConverter();
        }

        public static string Convert(double input)
        {
            return GetConverter().ConvertD(input);
        }

        public static string Convert(float input)
        {
            return GetConverter().ConvertF(input);
        }

        public string ConvertD(double inputNumber)
        {
            int p = 1023 + 52; // the power offset (precision)
            long signMask = unchecked((long)0x8000000000000000L); // the mask to get the sign of
                                                 // the number
            long eMask = 0x7FF0000000000000L; // the mask to get the power bits
            long fMask = 0x000FFFFFFFFFFFFFL; // the mask to get the significand
                                              // bits

            long inputNumberBits = BitConversion.DoubleToInt64Bits(inputNumber);
            // the value of the sign... 0 is positive, ~0 is negative
            string signString = (inputNumberBits & signMask) == 0 ? "" : "-";
            // the value of the 'power bits' of the inputNumber
            int e = (int)((inputNumberBits & eMask) >> 52);
            // the value of the 'significand bits' of the inputNumber
            long f = inputNumberBits & fMask;
            bool mantissaIsZero = f == 0;
            int pow = 0, numBits = 52;

            if (e == 2047)
                return mantissaIsZero ? signString + "Infinity" : "NaN";
            if (e == 0)
            {
                if (mantissaIsZero)
                    return signString + "0.0";
                if (f == 1)
                    // special case to increase precision even though 2 *
                    // Double.MIN_VALUE is 1.0e-323
                    return signString + "4.9E-324";
                pow = 1 - p; // a denormalized number
                long ff = f;
                while ((ff & 0x0010000000000000L) == 0)
                {
                    ff = ff << 1;
                    numBits--;
                }
            }
            else
            {
                // 0 < e < 2047
                // a "normalized" number
                f = f | 0x0010000000000000L;
                pow = e - p;
            }

            if (-59 < pow && pow < 6 || (pow == -59 && !mantissaIsZero))
                LongDigitGenerator(f, pow, e == 0, mantissaIsZero, numBits);
            else
                BigIntDigitGenerator(f, pow, e == 0, mantissaIsZero,
                    numBits);

            if (inputNumber >= 1e7D || inputNumber <= -1e7D
            || (inputNumber > -1e-3D && inputNumber < 1e-3D))
                return signString + FreeFormatExponential();

            return signString + FreeFormat();
        }

        public string ConvertF(float inputNumber)
        {
            int p = 127 + 23; // the power offset (precision)
            int signMask = unchecked((int)0x80000000); // the mask to get the sign of the number
            int eMask = 0x7F800000; // the mask to get the power bits
            int fMask = 0x007FFFFF; // the mask to get the significand bits

            int inputNumberBits = BitConversion.SingleToInt32Bits(inputNumber);
            // the value of the sign... 0 is positive, ~0 is negative
            string signString = (inputNumberBits & signMask) == 0 ? "" : "-";
            // the value of the 'power bits' of the inputNumber
            int e = (inputNumberBits & eMask) >> 23;
            // the value of the 'significand bits' of the inputNumber
            int f = inputNumberBits & fMask;
            bool mantissaIsZero = f == 0;
            int pow, numBits = 23;

            if (e == 255)
                return mantissaIsZero ? signString + "Infinity" : "NaN";
            if (e == 0)
            {
                if (mantissaIsZero)
                    return signString + "0.0";
                pow = 1 - p; // a denormalized number
                if (f < 8)
                { // want more precision with smallest values
                    f = f << 2;
                    pow -= 2;
                }
                int ff = f;
                while ((ff & 0x00800000) == 0)
                {
                    ff = ff << 1;
                    numBits--;
                }
            }
            else
            {
                // 0 < e < 255
                // a "normalized" number
                f = f | 0x00800000;
                pow = e - p;
            }

            if (-59 < pow && pow < 35 || (pow == -59 && !mantissaIsZero))
                LongDigitGenerator(f, pow, e == 0, mantissaIsZero, numBits);
            else
                BigIntDigitGenerator(f, pow, e == 0, mantissaIsZero,
                        numBits);
            if (inputNumber >= 1e7f || inputNumber <= -1e7f
            || (inputNumber > -1e-3f && inputNumber < 1e-3f))
                return signString + FreeFormatExponential();

            return signString + FreeFormat();
        }

        private string FreeFormatExponential()
        {
            // corresponds to process "Free-Format Exponential"
            char[] formattedDecimal = new char[25];
            formattedDecimal[0] = (char)('0' + uArray[getCount++]);
            formattedDecimal[1] = '.';
            // the position the next character is to be inserted into
            // formattedDecimal
            int charPos = 2;

            int k = firstK;
            int expt = k;
            while (true)
            {
                k--;
                if (getCount >= setCount)
                    break;

                formattedDecimal[charPos++] = (char)('0' + uArray[getCount++]);
            }

            if (k == expt - 1)
                formattedDecimal[charPos++] = '0';
            formattedDecimal[charPos++] = 'E';
            return new string(formattedDecimal, 0, charPos)
                    + Int32.ToString(expt, J2N.Text.StringFormatter.InvariantCulture);
        }

        private string FreeFormat()
        {
            // corresponds to process "Free-Format"
            char[] formattedDecimal = new char[25];
            // the position the next character is to be inserted into
            // formattedDecimal
            int charPos = 0;
            int k = firstK;
            if (k < 0)
            {
                formattedDecimal[0] = '0';
                formattedDecimal[1] = '.';
                charPos += 2;
                for (int i = k + 1; i < 0; i++)
                    formattedDecimal[charPos++] = '0';
            }

            int U = uArray[getCount++];
            do
            {
                if (U != -1)
                    formattedDecimal[charPos++] = (char)('0' + U);
                else if (k >= -1)
                    formattedDecimal[charPos++] = '0';

                if (k == 0)
                    formattedDecimal[charPos++] = '.';

                k--;
                U = getCount < setCount ? uArray[getCount++] : -1;
            } while (U != -1 || k >= -1);
            return new string(formattedDecimal, 0, charPos);
        }

        //private void bigIntDigitGeneratorInstImpl(long f, int e,
        //        bool isDenormalized, bool mantissaIsZero, int p);

        #region From bigint.c in Apache Harmony

        // From cbigint.c in Apache Harmony
        //private ulong HIGH_IN_U64(ulong u64) => u64 >> 32;
        //private ulong LOW_IN_U64(ulong u64) => u64 & 0x00000000FFFFFFFF;

        //private static readonly BigInteger TEN_E1 = 0xA;
        //private static readonly BigInteger TEN_E2 = 0x64;
        //private static readonly BigInteger TEN_E3 = 0x3E8;
        //private static readonly BigInteger TEN_E4 = 0x2710;
        //private static readonly BigInteger TEN_E5 = 0x186A0;
        //private static readonly BigInteger TEN_E6 = 0xF4240;
        //private static readonly BigInteger TEN_E7 = 0x989680;
        //private static readonly BigInteger TEN_E8 = 0x5F5E100;
        //private static readonly BigInteger TEN_E9 = 0x3B9ACA00;
        //private static readonly BigInteger TEN_E19 = 0x8AC7230489E80000;

        //private BigInteger TIMES_TEN(BigInteger x) => (((x << 3) + ((x) << 1)));

        private static readonly BigInteger TEN = 10; 

        private void BigIntDigitGenerator(long f, int e, bool isDenormalized, bool mantissaIsZero, int p)
        {
            BigInteger R, S, M;
            if (e >= 0)
            {
                M = 1L << e;
                if (!mantissaIsZero)
                {
                    R = (BigInteger)f << (e + 1);
                    S = 2;
                }
                else
                {
                    R = (BigInteger)f << (e + 2);
                    S = 4;
                }
            }
            else
            {
                M = 1;
                if (isDenormalized || !mantissaIsZero)
                {
                    R = (BigInteger)f << 1;
                    S = (BigInteger)1L << (1 - e);
                }
                else
                {
                    R = (BigInteger)f << 2;
                    S = (BigInteger)1L << (2 - e);
                }
            }
            int k = (int)Math.Ceiling((e + p - 1) * invLogOfTenBaseTwo - 1e-10);

            if (k > 0)
            {
                //S = S * TEN_TO_THE[k];
                S = BigInteger.Pow(TEN, k);
            }
            else if (k < 0)
            {
                //long scale = TEN_TO_THE[-k];
                //R = R * scale;
                //M = M == 1 ? scale : M * scale;
                BigInteger scale = BigInteger.Pow(TEN, -k);
                R = R * scale;
                M = M == 1 ? scale : M * scale;
            }

            if (R + M > S)
            { // was M_plus
                firstK = k;
            }
            else
            {
                firstK = k - 1;
                R = R * 10;
                M = M * 10;
            }

            getCount = setCount = 0; // reset indices
            bool low, high;
            int U;
            BigInteger[] Si = new BigInteger[] { S, S << 1, S << 2, S << 3 };
            while (true)
            {
                // set U to be floor (R / S) and R to be the remainder
                // using a kind of "binary search" to find the answer.
                // It's a lot quicker than actually dividing since we know
                // the answer will be between 0 and 10
                U = 0;
                BigInteger remainder;
                for (int i = 3; i >= 0; i--)
                {
                    remainder = R - Si[i];
                    if (remainder >= 0)
                    {
                        R = remainder;
                        U += 1 << i;
                    }
                }

                low = R < M; // was M_minus
                high = R + M > S; // was M_plus

                if (low || high)
                    break;

                R = R * 10;
                M = M * 10;
                uArray[setCount++] = U;
            }
            if (low && !high)
                uArray[setCount++] = U;
            else if (high && !low)
                uArray[setCount++] = U + 1;
            else if ((R << 1) < S)
                uArray[setCount++] = U;
            else
                uArray[setCount++] = U + 1;
        }

        //private long TimesTenToTheEHighPrecision(ref ulong result, long length, int e)
        //{
        //    ulong overflow;
        //    int exp10 = e;

        //    if (e == 0)
        //        return length;

        //    /* Replace the current implementation which performs a
        //     * "multiplication" by 10 e number of times with an actual
        //     * multiplication. 10e19 is the largest exponent to the power of ten
        //     * that will fit in a 64-bit integer, and 10e9 is the largest exponent to
        //     * the power of ten that will fit in a 64-bit integer. Not sure where the
        //     * break-even point is between an actual multiplication and a
        //     * simpleAappendDecimalDigit() so just pick 10e3 as that point for
        //     * now.
        //     */
        //}

        //private long SimpleMultiplyHighPrecision64

        //private int SimpleAppendDecimalDigitHighPrecision(ulong[] arg1, long length, ulong digit)
        //{
        //    /* assumes digit is less than 32 bits */
        //    ulong arg;
        //    long index = 0;

        //    digit <<= 32;
        //    do
        //    {

        //    } while (++index < length);
        //}

        #endregion

        private void LongDigitGenerator(long f, int e, bool isDenormalized,
                bool mantissaIsZero, int p)
        {
            long R, S, M;
            if (e >= 0)
            {
                M = 1L << e;
                if (!mantissaIsZero)
                {
                    R = f << (e + 1);
                    S = 2;
                }
                else
                {
                    R = f << (e + 2);
                    S = 4;
                }
            }
            else
            {
                M = 1;
                if (isDenormalized || !mantissaIsZero)
                {
                    R = f << 1;
                    S = 1L << (1 - e);
                }
                else
                {
                    R = f << 2;
                    S = 1L << (2 - e);
                }
            }

            int k = (int)Math.Ceiling((e + p - 1) * invLogOfTenBaseTwo - 1e-10);

            if (k > 0)
            {
                S = S * TEN_TO_THE[k];
            }
            else if (k < 0)
            {
                long scale = TEN_TO_THE[-k];
                R = R * scale;
                M = M == 1 ? scale : M * scale;
            }

            if (R + M > S)
            { // was M_plus
                firstK = k;
            }
            else
            {
                firstK = k - 1;
                R = R * 10;
                M = M * 10;
            }

            getCount = setCount = 0; // reset indices
            bool low, high;
            int U;
            long[] Si = new long[] { S, S << 1, S << 2, S << 3 };
            while (true)
            {
                // set U to be floor (R / S) and R to be the remainder
                // using a kind of "binary search" to find the answer.
                // It's a lot quicker than actually dividing since we know
                // the answer will be between 0 and 10
                U = 0;
                long remainder;
                for (int i = 3; i >= 0; i--)
                {
                    remainder = R - Si[i];
                    if (remainder >= 0)
                    {
                        R = remainder;
                        U += 1 << i;
                    }
                }

                low = R < M; // was M_minus
                high = R + M > S; // was M_plus

                if (low || high)
                    break;

                R = R * 10;
                M = M * 10;
                uArray[setCount++] = U;
            }
            if (low && !high)
                uArray[setCount++] = U;
            else if (high && !low)
                uArray[setCount++] = U + 1;
            else if ((R << 1) < S)
                uArray[setCount++] = U;
            else
                uArray[setCount++] = U + 1;
        }
    }
}
