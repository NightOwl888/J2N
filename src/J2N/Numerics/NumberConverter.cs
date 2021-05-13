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
                S = S * BigInteger.Pow(TEN, k);
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



        // From cbigint.c in Apache Harmony
        private static ulong LOW_IN_U64(ulong u64) => u64 >> 32;
        private static ulong HIGH_IN_U64(ulong u64) => u64 & 0x00000000FFFFFFFF;

        // J2N: Since we are dealing with an array type and not a pointer to a memory location,
        // we can normalize these to a single operation for both FROM_VAR adn FROM_PTR variants.
        private static ulong LOW_U32(ulong u64) => u64; // J2N TODO: Do low level op here
        private static ulong HIGH_U32(ulong u64) => u64; // J2N TODO: Do low level op here

        private static ulong TIMES_TEN(ulong u64) => (((u64 << 3) + ((u64) << 1)));

        /*NB the Number converter methods are synchronized so it is possible to
        *have global data for use by bigIntDigitGenerator */
        private const int RM_SIZE = 21;
        private const int STemp_SIZE = 22;

        private const ulong TEN_E1 = 0xA;
        private const ulong TEN_E2 = 0x64;
        private const ulong TEN_E3 = 0x3E8;
        private const ulong TEN_E4 = 0x2710;
        private const ulong TEN_E5 = 0x186A0;
        private const ulong TEN_E6 = 0xF4240;
        private const ulong TEN_E7 = 0x989680;
        private const ulong TEN_E8 = 0x5F5E100;
        private const ulong TEN_E9 = 0x3B9ACA00;
        private const ulong TEN_E19 = 0x8AC7230489E80000;

        private static readonly BigInteger TEN = 10; 

        //private void BigIntDigitGenerator(float f, int e, bool isDenormalized, bool mantissaIsZero, int p)
        //{
        //    int RLength, SLength, TempLength, mplus_Length, mminus_Length;
        //    int i;
        //    //int high, low, i; // J2N: high, low defined below
        //    //int k, firstK, U; // J2N: firstK defined at the class level, k and U defined below
        //    //int getCount, setCount; // J2N: Defined at the class level
        //    //int[] uArray; // J2N: Defined at the class level

        //    // J2N: Omitted j stuff because this is all in scope

        //    ulong[] R = new ulong[RM_SIZE], S = new ulong[STemp_SIZE], mplus = new ulong[RM_SIZE], mminus = new ulong[RM_SIZE],
        //        Temp = new ulong[STemp_SIZE];

        //    if (e >= 0)
        //    {
        //        *R = f; // J2N TODO: Not sure how to translate this - R is an array so not sure which element we are supposed to set. In the original code it was a pointer.
        //        *mplus = *mminus = 1; // J2N TODO: Not sure how to translate this - R is an array so not sure which element we are supposed to set. In the original code it was a pointer.
        //        SimpleShiftLeftHighPrecision(mminus, RM_SIZE, e);
                
        //        if (f != ((ulong)2 << (p - 1)))
        //        {
        //            SimpleShiftLeftHighPrecision(R, RM_SIZE, e + 1);
        //            *S = 2;
        //            /*
        //            * m+ = m+ << e results in 1.0e23 to be printed as
        //            * 0.9999999999999999E23
        //            * m+ = m+ << e+1 results in 1.0e23 to be printed as
        //            * 1.0e23 (caused too much rounding)
        //            *      470fffffffffffff = 2.0769187434139308E34
        //            *      4710000000000000 = 2.076918743413931E34
        //            */
        //            SimpleShiftLeftHighPrecision(mplus, RM_SIZE, e);
        //        }
        //        else
        //        {
        //            SimpleShiftLeftHighPrecision(R, RM_SIZE, e + 2);
        //            *S = 4;
        //            SimpleShiftLeftHighPrecision(mplus, RM_SIZE, e + 1);
        //        }
        //    }
        //    else
        //    {
        //        if (isDenormalized || (f != ((ulong)2 << (p - 1))))
        //        {
        //            *R = (ulong)f << 1;
        //            *S = (ulong)1L << (1 - e);
        //            SimpleShiftLeftHighPrecision(S, STemp_SIZE, 1 - e);
        //            *mplus = *mminus = 1;
        //        }
        //        else
        //        {
        //            *R = (ulong)f << 2;
        //            *S = (ulong)1L << (2 - e);
        //            SimpleShiftLeftHighPrecision(S, STemp_SIZE, 2 - e);
        //            *mplus = 2;
        //            *mminus = 1;
        //        }
        //    }
        //    int k = (int)Math.Ceiling((e + p - 1) * invLogOfTenBaseTwo - 1e-10);

        //    if (k > 0)
        //    {
        //        //// BigInteger code:
        //        //S = S * BigInteger.Pow(TEN, k);

        //        TimesTenToTheEHighPrecision(S, STemp_SIZE, k);
        //    }
        //    else if (k < 0)
        //    {
        //        //// BigInteger code:
        //        //R = R * BigInteger.Pow(TEN, -k);
        //        //mplus = mplus * BigInteger.Pow(TEN, -k);
        //        //mminus = mminus * BigInteger.Pow(TEN, -k);

        //        TimesTenToTheEHighPrecision(R, RM_SIZE, -k);
        //        TimesTenToTheEHighPrecision(mplus, RM_SIZE, -k);
        //        TimesTenToTheEHighPrecision(mminus, RM_SIZE, -k);
        //    }

        //    RLength = mplus_Length = mminus_Length = RM_SIZE;
        //    SLength = TempLength = STemp_SIZE;

        //    if (Temp.Length > RM_SIZE)
        //        Temp.Fill<ulong>(0L);
        //    Array.Copy(R, Temp, RM_SIZE);

        //    while (RLength > 1 && R[RLength - 1] == 0)
        //        --RLength;
        //    while (mplus_Length > 1 && mplus[mplus_Length - 1] == 0)
        //        --mplus_Length;
        //    while (mminus_Length > 1 && mminus[mminus_Length - 1] == 0)
        //        --mminus_Length;
        //    while (SLength > 1 && S[SLength - 1] == 0)
        //        --SLength;
        //    TempLength = (RLength > mplus_Length ? RLength : mplus_Length) + 1;
        //    AddHighPrecision(Temp, TempLength, mplus, mplus_Length);

        //    if (CompareHighPrecision(Temp, TempLength, S, SLength) >= 0)
        //    { // was M_plus
        //        firstK = k;
        //    }
        //    else
        //    {
        //        firstK = k - 1;
        //        SimpleAppendDecimalDigitHighPrecision(R, ++RLength, 0);
        //        SimpleAppendDecimalDigitHighPrecision(mplus, ++mplus_Length, 0);
        //        SimpleAppendDecimalDigitHighPrecision(mminus, ++mminus_Length, 0);
        //        while (RLength > 1 && R[RLength - 1] == 0)
        //            --RLength;
        //        while (mplus_Length > 1 && mplus[mplus_Length - 1] == 0)
        //            --mplus_Length;
        //        while (mminus_Length > 1 && mminus[mminus_Length - 1] == 0)
        //            --mminus_Length;
        //    }

        //    // J2N: class level variables are already in scope, so we omit the code dealing with that

        //    getCount = setCount = 0; // reset indices

        //    bool low, high;
        //    int U;
        //    while (true)
        //    {
        //        U = 0;
        //        for (i = 3; i >= 0; --i)
        //        {
        //            TempLength = SLength + 1;
        //            Temp[SLength] = 0;
        //            if (Temp.Length > SLength)
        //                Temp.Fill<ulong>(0L);
        //            Array.Copy(S, Temp, SLength);
        //            SimpleShiftLeftHighPrecision(Temp, TempLength, i);
        //            if (CompareHighPrecision(R, RLength, Temp, TempLength) >= 0)
        //            {
        //                SubtractHighPrecision(R, RLength, Temp, TempLength);
        //                U += 1 << i;
        //            }
        //        }

        //        low = CompareHighPrecision(R, RLength, mminus, mminus_Length) <= 0;

        //        if (Temp.Length > RLength)
        //            Temp.Fill<ulong>(0L);
        //        Array.Copy(R, Temp, RLength);
        //        TempLength = (RLength > mplus_Length ? RLength : mplus_Length) + 1;
        //        AddHighPrecision(Temp, TempLength, mplus, mplus_Length);

        //        high = CompareHighPrecision(Temp, TempLength, S, SLength) >= 0;

        //        if (low || high)
        //            break;

        //        SimpleAppendDecimalDigitHighPrecision(R, ++RLength, 0);
        //        SimpleAppendDecimalDigitHighPrecision(mplus, ++mplus_Length, 0);
        //        SimpleAppendDecimalDigitHighPrecision(mminus, ++mminus_Length, 0);
        //        while (RLength > 1 && R[RLength - 1] == 0)
        //            --RLength;
        //        while (mplus_Length > 1 && mplus[mplus_Length - 1] == 0)
        //            --mplus_Length;
        //        while (mminus_Length > 1 && mminus[mminus_Length - 1] == 0)
        //            --mminus_Length;
        //        uArray[setCount++] = U;
        //    }

        //    SimpleShiftLeftHighPrecision(R, ++RLength, 1);
        //    if (low && !high)
        //        uArray[setCount++] = U;
        //    else if (high && !low)
        //        uArray[setCount++] = U + 1;
        //    else if (CompareHighPrecision(R, RLength, S, SLength) < 0)
        //        uArray[setCount++] = U;
        //    else
        //        uArray[setCount++] = U + 1;
        //}

        // BigInteger can raise to the power of 10 to the e, but lacks the ability to specify length,
        // so we need to do longhand multiplication.
        internal static int TimesTenToTheEHighPrecision(ulong[] result, int length, int e)
        {
            /* assumes result can hold value */
            ulong overflow;
            int exp10 = e;

            if (e == 0)
                return length;
            /* bad O(n) way of doing it, but simple */
            /*
               do {
               overflow = simpleAppendDecimalDigitHighPrecision(result, length, 0);
               if (overflow)
               result[length++] = overflow;
               } while (--e);
             */
            /* Replace the current implementation which performs a
             * "multiplication" by 10 e number of times with an actual
             * multiplication. 10e19 is the largest exponent to the power of ten
             * that will fit in a 64-bit integer, and 10e9 is the largest exponent to
             * the power of ten that will fit in a 64-bit integer. Not sure where the
             * break-even point is between an actual multiplication and a
             * simpleAappendDecimalDigit() so just pick 10e3 as that point for
             * now.
             */
            while (exp10 >= 19)
            {
                overflow = SimpleMultiplyHighPrecision64(result, length, TEN_E19);
                if (overflow != 0)
                    result[length++] = overflow;
                exp10 -= 19;
            }
            while (exp10 >= 9)
            {
                overflow = SimpleMultiplyHighPrecision(result, length, TEN_E9);
                if (overflow != 0)
                    result[length++] = overflow;
                exp10 -= 9;
            }
            if (exp10 == 0)
            {
                return length;
            }
            else if (exp10 == 1)
            {
                overflow = SimpleAppendDecimalDigitHighPrecision(result, length, 0);
                if (overflow != 0)
                    result[length++] = overflow;
            }
            else if (exp10 == 2)
            {
                overflow = SimpleAppendDecimalDigitHighPrecision(result, length, 0);
                if (overflow != 0)
                    result[length++] = overflow;
                overflow = SimpleAppendDecimalDigitHighPrecision(result, length, 0); // J2N TODO: This seems strange repeating the exact same thing
                if (overflow != 0)
                    result[length++] = overflow;
            }
            else if (exp10 == 3)
            {
                overflow = SimpleMultiplyHighPrecision(result, length, TEN_E3);
                if (overflow != 0)
                    result[length++] = overflow;
            }
            else if (exp10 == 4)
            {
                overflow = SimpleMultiplyHighPrecision(result, length, TEN_E4);
                if (overflow != 0)
                    result[length++] = overflow;
            }
            else if (exp10 == 5)
            {
                overflow = SimpleMultiplyHighPrecision(result, length, TEN_E5);
                if (overflow != 0)
                    result[length++] = overflow;
            }
            else if (exp10 == 6)
            {
                overflow = SimpleMultiplyHighPrecision(result, length, TEN_E6);
                if (overflow != 0)
                    result[length++] = overflow;
            }
            else if (exp10 == 7)
            {
                overflow = SimpleMultiplyHighPrecision(result, length, TEN_E7);
                if (overflow != 0)
                    result[length++] = overflow;
            }
            else if (exp10 == 8)
            {
                overflow = SimpleMultiplyHighPrecision(result, length, TEN_E8);
                if (overflow != 0)
                    result[length++] = overflow;
            }
            return length;
        }

        private static ulong SimpleMultiplyHighPrecision64(ulong[] arg1, int length, ulong arg2)
        {
            ulong intermediate, pArg1, carry1, carry2, prod1, prod2, sum;
            //ulong index;
            ulong buf32; // Should be uint, but would need to cast


            sum = 0; // J2N specific - need to assign this before use.
            //index = 0;
            intermediate = 0;
            //pArg1 = arg1[index];
            carry1 = carry2 = 0;

            for (int index = 0; index < length; ++index)
            {
                pArg1 = arg1[index];

                if ((pArg1 != 0) || (intermediate != 0))
                {
                    prod1 = LOW_U32(arg2) * HIGH_U32(pArg1);
                    sum = intermediate + prod1;
                    if ((sum < prod1) || (sum < intermediate))
                    {
                        carry1 = 1;
                    }
                    else
                    {
                        carry1 = 0;
                    }
                }
                prod1 = LOW_U32(arg2) * HIGH_U32(pArg1);
                prod2 = HIGH_U32(arg2) * LOW_U32(pArg1);
                intermediate = carry2 + HIGH_IN_U64(sum) + prod1 + prod2;
                if ((intermediate < prod1) || (intermediate < prod2))
                {
                    carry2 = 1;
                }
                else
                {
                    carry2 = 0;
                }
                // LOW_U32_FROM_PTR (pArg1) = LOW_U32_FROM_VAR (sum); // J2N TODO: How do we translate this?
                buf32 = LOW_U32(pArg1);
                // HIGH_U32_FROM_PTR (pArg1) = LOW_U32_FROM_VAR (intermediate); // J2N TODO: How do we translate this?
                intermediate = carry1 + HIGH_IN_U64(intermediate)
                    + HIGH_U32(arg2) * buf32;
            }
            return intermediate;
        }

        private static ulong SimpleMultiplyHighPrecision(ulong[] arg1, int length, ulong arg2)
        {
            /* assumes arg2 only holds 32 bits of information */
            ulong product = 0;
            //int index = 0;

            for (int index = 0; index < length; ++index)
            {
                product = HIGH_IN_U64(product) + arg2 * LOW_U32(arg1[index]);
                // LOW_U32_FROM_PTR (arg1 + index) = LOW_U32_FROM_VAR (product); // J2N TODO: How do we translate this?
                product = HIGH_IN_U64(product) + arg2 * HIGH_U32(product);
            }
            return HIGH_U32(product);
        }

        private static ulong SimpleAppendDecimalDigitHighPrecision(ulong[] arg1, int length, ulong digit)
        {
            /* assumes digit is less than 32 bits */
            ulong arg;
            //int index = 0;

            digit <<= 32;
            for (int index = 0; index < length; ++index)
            {
                arg = LOW_IN_U64(arg1[index]);
                digit = HIGH_IN_U64(digit) + TIMES_TEN(arg);
                // LOW_U32_FROM_PTR (arg1 + index) = LOW_U32_FROM_VAR (digit); // J2N TODO: How do we translate this?

                arg = HIGH_IN_U64(arg1[index]);
                digit = HIGH_IN_U64(digit) + TIMES_TEN(arg);
                // HIGH_U32_FROM_PTR (arg1 + index) = LOW_U32_FROM_VAR (digit); // J2N TODO: How do we translate this?
            }

            return HIGH_U32(digit);
        }

        //private static void SimpleShiftLeftHighPrecision(ulong[] arg1, int length, int arg2)
        //{
        //    /* assumes length > 0 */
        //    int index, offset;
        //    if (arg2 >= 64)
        //    {
        //        offset = arg2 >> 6;
        //        index = length;

        //        while (--index - offset >= 0)
        //            arg1[index] = arg1[index - offset];
        //        do
        //        {
        //            arg1[index] = 0;
        //        } while (--index >= 0);

        //        arg2 &= 0x3F;
        //    }
        //    if (arg2 == 0)
        //        return;
        //    while (--length > 0)
        //    {
        //        arg1[length] = arg1[length] << arg2 | arg1[length - 1] >> (64 - arg2);
        //    }
        //    *arg1 <<= arg2; // J2N TODO: How do we translate this?
        //}

        private static int CompareHighPrecision(ulong[] arg1, int length1, ulong[] arg2, int length2)
        {
            while (--length1 >= 0 && arg1[length1] == 0) ;
            while (--length2 >= 0 && arg2[length2] == 0) ;

            if (length1 > length2)
                return 1;
            else if (length1 < length2)
                return -1;
            else if (length1 > -1)
            {
                do
                {
                    if (arg1[length1] > arg2[length1])
                        return 1;
                    else if (arg1[length1] < arg2[length1])
                        return -1;
                }
                while (--length1 >= 0);
            }

            return 0;
        }

        //private static bool SimpleAddHighPrecision(ulong[] arg1, int length, ulong arg2)
        //{
        //    /* assumes length > 0 */
        //    int index = 1;

        //    *arg1 += arg2; // J2N TODO: How do we translate this?
        //    if (arg2 <= *arg1) // J2N TODO: How do we translate this?
        //        return false;
        //    else if (length == 1)
        //        return true;

        //    while (++arg1[index] == 0 && ++index < length) ;

        //    return index == length;
        //}

        private static bool AddHighPrecision(ulong[] arg1, int length1, ulong[] arg2, int length2)
        {
            if (length1 == 0 || length2 == 0)
            {
                return false;
            }
            else if (length1 < length2)
            {
                length2 = length1;
            }

            /* addition is limited by length of arg1 as it this function is
             * storing the result in arg1 */
            /* fix for cc (GCC) 3.2 20020903 (Red Hat Linux 8.0 3.2-7): code generated does not
             * do the temp1 + temp2 + carry addition correct.  carry is 64 bit because gcc has
             * subtle issues when you mix 64 / 32 bit maths. */
            ulong temp1, temp2, temp3;
            ulong carry = 0;
            int index = 0;
            do
            {
                temp1 = arg1[index];
                temp2 = arg2[index];
                temp3 = temp1 + temp2;
                arg1[index] = temp3 + carry;
                if (arg2[index] < arg1[index])
                    carry = 0;
                else if (arg2[index] != arg1[index])
                    carry = 1;
            }
            while (++index < length2);
            if (carry == 0)
                return false;
            else if (index == length1)
                return true;

            while (++arg1[index] == 0 && ++index < length1) ;

            return index == length1;
        }

        //private static void SubtractHighPrecision(ulong[] arg1, int length1, ulong[] arg2, int length2)
        //{
        //    /* assumes arg1 > arg2 */
        //    int index;
        //    for (index = 0; index < length1; ++index)
        //        arg1[index] = ~arg1[index];
        //    SimpleAddHighPrecision(arg1, length1, 1);

        //    while (length2 > 0 && arg2[length2 - 1] == 0)
        //        --length2;

        //    AddHighPrecision(arg1, length1, arg2, length2);

        //    for (index = 0; index < length1; ++index)
        //        arg1[index] = ~arg1[index];
        //    SimpleAddHighPrecision(arg1, length1, 1);
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
