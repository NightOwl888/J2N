#region Copyright 2018 by Ulf Adams, Licensed under the Apache License, Version 2.0
// Copyright 2018 Ulf Adams
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using J2N.Text;
using System;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace J2N.Numerics
{

    /// <summary>
    /// An implementation of Ryu for <see cref="float"/>.
    /// </summary>
    internal sealed partial class RyuSingle
    {
        private const int CharStackBufferSize = 32;

#if DEBUG
#pragma warning disable IDE0044 // Add readonly modifier
        private static bool DEBUG = false;
#pragma warning restore IDE0044 // Add readonly modifier
#endif

        private const int FLOAT_MANTISSA_BITS = 23;
        private const int FLOAT_MANTISSA_MASK = (1 << FLOAT_MANTISSA_BITS) - 1;

        private const int FLOAT_EXPONENT_BITS = 8;
        private const int FLOAT_EXPONENT_MASK = (1 << FLOAT_EXPONENT_BITS) - 1;
        private const int FLOAT_EXPONENT_BIAS = (1 << (FLOAT_EXPONENT_BITS - 1)) - 1;

        private const long LOG10_2_DENOMINATOR = 10000000L;
        private static readonly long LOG10_2_NUMERATOR = (long)(LOG10_2_DENOMINATOR * Math.Log10(2));

        private const long LOG10_5_DENOMINATOR = 10000000L;
        private static readonly long LOG10_5_NUMERATOR = (long)(LOG10_5_DENOMINATOR * Math.Log10(5));

        private const long LOG2_5_DENOMINATOR = 10000000L;
        private static readonly long LOG2_5_NUMERATOR = (long)(LOG2_5_DENOMINATOR * (Math.Log(5) / Math.Log(2)));

        private const int POS_TABLE_SIZE = 47;
        private const int INV_TABLE_SIZE = 31;

        #region Debug Tables
#if DEBUG
        // Only for debugging.
        private static readonly BigInteger[] POW5 = new BigInteger[POS_TABLE_SIZE] {
            BigInteger.Parse("1"),
            BigInteger.Parse("5"),
            BigInteger.Parse("25"),
            BigInteger.Parse("125"),
            BigInteger.Parse("625"),
            BigInteger.Parse("3125"),
            BigInteger.Parse("15625"),
            BigInteger.Parse("78125"),
            BigInteger.Parse("390625"),
            BigInteger.Parse("1953125"),
            BigInteger.Parse("9765625"),
            BigInteger.Parse("48828125"),
            BigInteger.Parse("244140625"),
            BigInteger.Parse("1220703125"),
            BigInteger.Parse("6103515625"),
            BigInteger.Parse("30517578125"),
            BigInteger.Parse("152587890625"),
            BigInteger.Parse("762939453125"),
            BigInteger.Parse("3814697265625"),
            BigInteger.Parse("19073486328125"),
            BigInteger.Parse("95367431640625"),
            BigInteger.Parse("476837158203125"),
            BigInteger.Parse("2384185791015625"),
            BigInteger.Parse("11920928955078125"),
            BigInteger.Parse("59604644775390625"),
            BigInteger.Parse("298023223876953125"),
            BigInteger.Parse("1490116119384765625"),
            BigInteger.Parse("7450580596923828125"),
            BigInteger.Parse("37252902984619140625"),
            BigInteger.Parse("186264514923095703125"),
            BigInteger.Parse("931322574615478515625"),
            BigInteger.Parse("4656612873077392578125"),
            BigInteger.Parse("23283064365386962890625"),
            BigInteger.Parse("116415321826934814453125"),
            BigInteger.Parse("582076609134674072265625"),
            BigInteger.Parse("2910383045673370361328125"),
            BigInteger.Parse("14551915228366851806640625"),
            BigInteger.Parse("72759576141834259033203125"),
            BigInteger.Parse("363797880709171295166015625"),
            BigInteger.Parse("1818989403545856475830078125"),
            BigInteger.Parse("9094947017729282379150390625"),
            BigInteger.Parse("45474735088646411895751953125"),
            BigInteger.Parse("227373675443232059478759765625"),
            BigInteger.Parse("1136868377216160297393798828125"),
            BigInteger.Parse("5684341886080801486968994140625"),
            BigInteger.Parse("28421709430404007434844970703125"),
            BigInteger.Parse("142108547152020037174224853515625"),
        };

        private static readonly BigInteger[] POW5_INV = new BigInteger[INV_TABLE_SIZE] { // J2N: Same as NEG_TABLE_SIZE in RyuDouble
            BigInteger.Parse("576460752303423489"), BigInteger.Parse("461168601842738791"),
            BigInteger.Parse("368934881474191033"), BigInteger.Parse("295147905179352826"),
            BigInteger.Parse("472236648286964522"), BigInteger.Parse("377789318629571618"),
            BigInteger.Parse("302231454903657294"), BigInteger.Parse("483570327845851670"),
            BigInteger.Parse("386856262276681336"), BigInteger.Parse("309485009821345069"),
            BigInteger.Parse("495176015714152110"), BigInteger.Parse("396140812571321688"),
            BigInteger.Parse("316912650057057351"), BigInteger.Parse("507060240091291761"),
            BigInteger.Parse("405648192073033409"), BigInteger.Parse("324518553658426727"),
            BigInteger.Parse("519229685853482763"), BigInteger.Parse("415383748682786211"),
            BigInteger.Parse("332306998946228969"), BigInteger.Parse("531691198313966350"),
            BigInteger.Parse("425352958651173080"), BigInteger.Parse("340282366920938464"),
            BigInteger.Parse("544451787073501542"), BigInteger.Parse("435561429658801234"),
            BigInteger.Parse("348449143727040987"), BigInteger.Parse("557518629963265579"),
            BigInteger.Parse("446014903970612463"), BigInteger.Parse("356811923176489971"),
            BigInteger.Parse("570899077082383953"), BigInteger.Parse("456719261665907162"),
            BigInteger.Parse("365375409332725730"),
        };
#endif
        #endregion Debug Tables


        private const int POW5_BITCOUNT = 61;
        private const int POW5_HALF_BITCOUNT = 31;

        private static readonly ulong[] POW5_SPLIT =
        {
            1152921504606846976ul, 1441151880758558720ul, 1801439850948198400ul, 2251799813685248000ul,
            1407374883553280000ul, 1759218604441600000ul, 2199023255552000000ul, 1374389534720000000ul,
            1717986918400000000ul, 2147483648000000000ul, 1342177280000000000ul, 1677721600000000000ul,
            2097152000000000000ul, 1310720000000000000ul, 1638400000000000000ul, 2048000000000000000ul,
            1280000000000000000ul, 1600000000000000000ul, 2000000000000000000ul, 1250000000000000000ul,
            1562500000000000000ul, 1953125000000000000ul, 1220703125000000000ul, 1525878906250000000ul,
            1907348632812500000ul, 1192092895507812500ul, 1490116119384765625ul, 1862645149230957031ul,
            1164153218269348144ul, 1455191522836685180ul, 1818989403545856475ul, 2273736754432320594ul,
            1421085471520200371ul, 1776356839400250464ul, 2220446049250313080ul, 1387778780781445675ul,
            1734723475976807094ul, 2168404344971008868ul, 1355252715606880542ul, 1694065894508600678ul,
            2117582368135750847ul, 1323488980084844279ul, 1654361225106055349ul, 2067951531382569187ul,
            1292469707114105741ul, 1615587133892632177ul, 2019483917365790221ul
        };

        private const int POW5_INV_BITCOUNT = 59;
        private const int POW5_INV_HALF_BITCOUNT = 31;

        private static readonly ulong[] POW5_INV_SPLIT =
        {
            576460752303423489ul, 461168601842738791ul, 368934881474191033ul, 295147905179352826ul,
            472236648286964522ul, 377789318629571618ul, 302231454903657294ul, 483570327845851670ul,
            386856262276681336ul, 309485009821345069ul, 495176015714152110ul, 396140812571321688ul,
            316912650057057351ul, 507060240091291761ul, 405648192073033409ul, 324518553658426727ul,
            519229685853482763ul, 415383748682786211ul, 332306998946228969ul, 531691198313966350ul,
            425352958651173080ul, 340282366920938464ul, 544451787073501542ul, 435561429658801234ul,
            348449143727040987ul, 557518629963265579ul, 446014903970612463ul, 356811923176489971ul,
            570899077082383953ul, 456719261665907162ul, 365375409332725730ul
        };

        //public static void Main(string[] args)
        //{
        //    float f = 0.33007812f;
        //    string result = ToString(f, RoundingMode.RoundEven);
        //    Console.WriteLine(result + " " + f);
        //}

        public static string ToString(float value, NumberFormatInfo info)
        {
            return ToString(value, info, RoundingMode.RoundEven, upperCase: true);
        }

        public static string ToString(float value, NumberFormatInfo info, bool upperCase)
        {
            return ToString(value, info, RoundingMode.RoundEven, upperCase);
        }

        public static string ToString(float value, NumberFormatInfo info, RoundingMode roundingMode)
        {
            return ToString(value, info, roundingMode, upperCase: true);
        }

        public static string ToString(float value, NumberFormatInfo info, RoundingMode roundingMode, bool upperCase)
        {
            var sb = new ValueStringBuilder(stackalloc char[CharStackBufferSize]);
            return FormatSingle(ref sb, value, info, roundingMode, upperCase) ?? sb.ToString();
        }

        public static string? FormatSingle(ref ValueStringBuilder sb, float value, NumberFormatInfo info, RoundingMode roundingMode, bool upperCase)
        {
            // Step 1: Decode the floating point number, and unify normalized and subnormal cases.
            // First, handle all the trivial cases.
            if (!value.IsFinite())
            {
                if (float.IsNaN(value))
                {
                    return info.NaNSymbol;
                }

                return value.IsNegative() ? info.NegativeInfinitySymbol : info.PositiveInfinitySymbol;
            }
            int bits = BitConversion.SingleToRawInt32Bits(value); // J2N: Since we have checked for NaN above, it is quicker to call SingleToRawInt32Bits
            if (bits == 0) return string.Concat("0", info.NumberDecimalSeparator, "0");
            if (bits == unchecked((int)0x80000000)) return string.Concat(info.NegativeSign, "0", info.NumberDecimalSeparator, "0");

            // Otherwise extract the mantissa and exponent bits and run the full algorithm.
            int ieeeExponent = (bits >> FLOAT_MANTISSA_BITS) & FLOAT_EXPONENT_MASK;
            int ieeeMantissa = bits & FLOAT_MANTISSA_MASK;
            // By default, the correct mantissa starts with a 1, except for denormal numbers.
            int e2;
            int m2;
            if (ieeeExponent == 0)
            {
                e2 = 1 - FLOAT_EXPONENT_BIAS - FLOAT_MANTISSA_BITS;
                m2 = ieeeMantissa;
            }
            else
            {
                e2 = ieeeExponent - FLOAT_EXPONENT_BIAS - FLOAT_MANTISSA_BITS;
                m2 = ieeeMantissa | (1 << FLOAT_MANTISSA_BITS);
            }

            bool sign = bits < 0;
#if DEBUG
            if (DEBUG)
            {
                Console.WriteLine("IN=" + bits.ToString());
                Console.WriteLine("   S=" + (sign ? "-" : "+") + " E=" + e2 + " M=" + m2);
            }
#endif

            // Step 2: Determine the interval of legal decimal representations.
            bool even = (m2 & 1) == 0;
            int mv = 4 * m2;
            int mp = 4 * m2 + 2;
            int mm = 4 * m2 - ((m2 != (1L << FLOAT_MANTISSA_BITS)) || (ieeeExponent <= 1) ? 2 : 1);
            e2 -= 2;

#if DEBUG
            if (DEBUG)
            {
                string sv, sp, sm;
                int e10Debug;
                if (e2 >= 0)
                {
                    sv = (new BigInteger(mv) << e2).ToString(CultureInfo.InvariantCulture);
                    sp = (new BigInteger(mp) << e2).ToString(CultureInfo.InvariantCulture);
                    sm = (new BigInteger(mm) << e2).ToString(CultureInfo.InvariantCulture);
                    e10Debug = 0;
                }
                else
                {
                    BigInteger factor = BigInteger.Pow(new BigInteger(5), -e2);
                    sv = (new BigInteger(mv) * factor).ToString(CultureInfo.InvariantCulture);
                    sp = (new BigInteger(mp) * factor).ToString(CultureInfo.InvariantCulture);
                    sm = (new BigInteger(mm) * factor).ToString(CultureInfo.InvariantCulture);
                    e10Debug = e2;
                }

                e10Debug += sp.Length - 1;

                Console.WriteLine("Exact values");
                Console.WriteLine("  m =" + mv);
                Console.WriteLine("  E =" + e10Debug);
                Console.WriteLine("  d+=" + sp);
                Console.WriteLine("  d =" + sv);
                Console.WriteLine("  d-=" + sm);
                Console.WriteLine("  e2=" + e2);
            }
#endif

            // Step 3: Convert to a decimal power base using 128-bit arithmetic.
            // -151 = 1 - 127 - 23 - 2 <= e_2 - 2 <= 254 - 127 - 23 - 2 = 102
            int dp, dv, dm;
            int e10;
            bool dpIsTrailingZeros, dvIsTrailingZeros, dmIsTrailingZeros;
            int lastRemovedDigit = 0;
            if (e2 >= 0)
            {
                // Compute m * 2^e_2 / 10^q = m * 2^(e_2 - q) / 5^q
                int q = (int)(e2 * LOG10_2_NUMERATOR / LOG10_2_DENOMINATOR);
                int k = POW5_INV_BITCOUNT + Pow5Bits(q) - 1;
                int i = -e2 + q + k;
                dv = (int)MulPow5InvDivPow2(mv, q, i);
                dp = (int)MulPow5InvDivPow2(mp, q, i);
                dm = (int)MulPow5InvDivPow2(mm, q, i);
                if (q != 0 && ((dp - 1) / 10 <= dm / 10))
                {
                    // We need to know one removed digit even if we are not going to loop below. We could use
                    // q = X - 1 above, except that would require 33 bits for the result, and we've found that
                    // 32-bit arithmetic is faster even on 64-bit machines.
                    int l = POW5_INV_BITCOUNT + Pow5Bits(q - 1) - 1;
                    lastRemovedDigit = (int)(MulPow5InvDivPow2(mv, q - 1, -e2 + q - 1 + l) % 10);
                }
                e10 = q;
#if DEBUG
                if (DEBUG)
                {
                    Console.WriteLine(mv + " * 2^" + e2 + " / 10^" + q);
                }
#endif

                dpIsTrailingZeros = Pow5Factor((int)mp) >= q;
                dvIsTrailingZeros = Pow5Factor((int)mv) >= q;
                dmIsTrailingZeros = Pow5Factor((int)mm) >= q;
            }
            else
            {
                // Compute m * 5^(-e_2) / 10^q = m * 5^(-e_2 - q) / 2^q
                int q = (int)(-e2 * LOG10_5_NUMERATOR / LOG10_5_DENOMINATOR);
                int i = -e2 - q;
                int k = Pow5Bits(i) - POW5_BITCOUNT;
                int j = q - k;
                dv = (int)MulPow5divPow2(mv, i, j);
                dp = (int)MulPow5divPow2(mp, i, j);
                dm = (int)MulPow5divPow2(mm, i, j);
                if (q != 0 && ((dp - 1) / 10 <= dm / 10))
                {
                    j = q - 1 - (Pow5Bits(i + 1) - POW5_BITCOUNT);
                    lastRemovedDigit = (int)(MulPow5divPow2(mv, i + 1, j) % 10);
                }
                e10 = q + e2; // Note: e2 and e10 are both negative here.
#if DEBUG
                if (DEBUG)
                {
                    Console.WriteLine(mv + " * 5^" + (-e2) + " / 10^" + q + " = " + mv + " * 5^" + (-e2 - q) + " / 2^" + q);
                }
#endif

                dpIsTrailingZeros = 1 >= q;
                dvIsTrailingZeros = (q < FLOAT_MANTISSA_BITS) && (mv & ((1 << (q - 1)) - 1)) == 0;
                dmIsTrailingZeros = (mm % 2 == 1 ? 0 : 1) >= q;
            }
#if DEBUG
            if (DEBUG)
            {
                Console.WriteLine("Actual values");
                Console.WriteLine("  d+=" + dp);
                Console.WriteLine("  d =" + dv);
                Console.WriteLine("  d-=" + dm);
                Console.WriteLine("  last removed=" + lastRemovedDigit);
                Console.WriteLine("  e10=" + e10);
                Console.WriteLine("  d+10=" + dpIsTrailingZeros);
                Console.WriteLine("  d   =" + dvIsTrailingZeros);
                Console.WriteLine("  d-10=" + dmIsTrailingZeros);
            }
#endif

            // Step 4: Find the shortest decimal representation in the interval of legal representations.
            //
            // We do some extra work here in order to follow Float/Double.toString semantics. In particular,
            // that requires printing in scientific format if and only if the exponent is between -3 and 7,
            // and it requires printing at least two decimal digits.
            //
            // Above, we moved the decimal dot all the way to the right, so now we need to count digits to
            // figure out the correct exponent for scientific notation.
            int dplength = DecimalLength(dp);
            int exp = e10 + dplength - 1;

            // Float.toString semantics requires using scientific notation if and only if outside this range.
            bool scientificNotation = !((exp >= -3) && (exp < 7));

            int removed = 0;
            if (dpIsTrailingZeros && !roundingMode.AcceptUpperBound(even))
            {
                dp--;
            }

            while (dp / 10 > dm / 10)
            {
                if ((dp < 100) && scientificNotation)
                {
                    // We print at least two digits, so we might as well stop now.
                    break;
                }
                dmIsTrailingZeros &= dm % 10 == 0;
                dp /= 10;
                lastRemovedDigit = dv % 10;
                dv /= 10;
                dm /= 10;
                removed++;
            }
            if (dmIsTrailingZeros && roundingMode.AcceptLowerBound(even))
            {
                while (dm % 10 == 0)
                {
                    if ((dp < 100) && scientificNotation)
                    {
                        // We print at least two digits, so we might as well stop now.
                        break;
                    }
                    dp /= 10;
                    lastRemovedDigit = dv % 10;
                    dv /= 10;
                    dm /= 10;
                    removed++;
                }
            }

            if (dvIsTrailingZeros && (lastRemovedDigit == 5) && (dv % 2 == 0))
            {
                // Round down not up if the number ends in X50000 and the number is even.
                lastRemovedDigit = 4;
            }
            int output = dv +
                ((dv == dm && !(dmIsTrailingZeros && roundingMode.AcceptLowerBound(even))) || (lastRemovedDigit >= 5) ? 1 : 0);
            int olength = dplength - removed;

#if DEBUG
            if (DEBUG)
            {
                Console.WriteLine("Actual values after loop");
                Console.WriteLine("  d+=" + dp);
                Console.WriteLine("  d =" + dv);
                Console.WriteLine("  d-=" + dm);
                Console.WriteLine("  last removed=" + lastRemovedDigit);
                Console.WriteLine("  e10=" + e10);
                Console.WriteLine("  d+10=" + dpIsTrailingZeros);
                Console.WriteLine("  d-10=" + dmIsTrailingZeros);
                Console.WriteLine("  output=" + output);
                Console.WriteLine("  output_length=" + olength);
                Console.WriteLine("  output_exponent=" + exp);
            }
#endif

            // Step 5: Print the decimal representation.
            // We follow Float.toString semantics here.
            //char[] result = new char[15];
            int index = 0;
            string negSign = info.NegativeSign, decimalSeparator = info.NumberDecimalSeparator;
            int negSignLength = (sign ? negSign.Length : 0), decimalSeparatorLength = decimalSeparator.Length;
            // For the exp + 1 >= olength case, we use the max length of 15. The value is derived from
            // the literal 11 + negSignLength + decimalSeparatorLength + 2.
            int bufferLength = Math.Min((exp < 0 ? dplength + 2 : (exp + 1 >= olength ? 11 : olength)) + negSignLength + decimalSeparatorLength + (scientificNotation ? 4 : 2), 13 + negSignLength + decimalSeparatorLength);

            unsafe
            {
                fixed (char* ptr = &MemoryMarshal.GetReference(sb.AppendSpan(bufferLength)))
                {
                    char* result = ptr;
                    WriteBuffer(result, ref index, output, olength, upperCase, sign, exp, scientificNotation, negSign, negSignLength, decimalSeparator, decimalSeparatorLength);
                }
                int excess = bufferLength - index;
                sb.Length -= excess;
                return null;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private unsafe static void WriteBuffer(char* result, ref int index, int output, int olength, bool upperCase, bool sign, int exp, bool scientificNotation, string negSign, int negSignLength, string decimalSeparator, int decimalSeparatorLength)
        {
            if (sign)
            {
                //result[index++] = '-';
                for (int i = 0; i < negSignLength; i++)
                {
                    result[index++] = negSign[i];
                }
            }

            if (scientificNotation)
            {
                // Print in the format x.xxxxxE-yy.
                for (int i = 0; i < olength - 1; i++)
                {
                    int c = output % 10; output /= 10;
                    //result[index + olength - i] = (char)('0' + c);
                    result[index + olength - i + (decimalSeparatorLength - 1)] = (char)('0' + c);
                }
                result[index] = (char)('0' + output % 10);
                //result[index + 1] = '.';
                //index += olength + 1;
                for (int i = 0; i < decimalSeparatorLength; i++)
                {
                    result[index + 1 + i] = decimalSeparator[i];
                }
                index += olength + decimalSeparatorLength;
                if (olength == 1)
                {
                    result[index++] = '0';
                }

                // Print 'E', the exponent sign, and the exponent, which has at most two digits.
                result[index++] = upperCase ? 'E' : 'e';
                if (exp < 0)
                {
                    result[index++] = '-';
                    exp = -exp;
                }
                if (exp >= 10)
                {
                    result[index++] = (char)('0' + exp / 10);
                }
                result[index++] = (char)('0' + exp % 10);
            }
            else
            {
                // Otherwise follow the Java spec for values in the interval [1E-3, 1E7).
                if (exp < 0)
                {
                    // Decimal dot is before any of the digits.
                    result[index++] = '0';
                    //result[index++] = '.';
                    for (int i = 0; i < decimalSeparatorLength; i++)
                    {
                        result[index++] = decimalSeparator[i];
                    }
                    for (int i = -1; i > exp; i--)
                    {
                        result[index++] = '0';
                    }
                    int current = index;
                    for (int i = 0; i < olength; i++)
                    {
                        result[current + olength - i - 1] = (char)('0' + output % 10);
                        output /= 10;
                        index++;
                    }
                }
                else if (exp + 1 >= olength)
                {
                    // Decimal dot is after any of the digits.
                    for (int i = 0; i < olength; i++)
                    {
                        result[index + olength - i - 1] = (char)('0' + output % 10);
                        output /= 10;
                    }
                    index += olength;
                    for (int i = olength; i < exp + 1; i++)
                    {
                        result[index++] = '0';
                    }
                    //result[index++] = '.';
                    for (int i = 0; i < decimalSeparatorLength; i++)
                    {
                        result[index++] = decimalSeparator[i];
                    }
                    result[index++] = '0';
                }
                else
                {
                    // Decimal dot is somewhere between the digits.
                    //int current = index + 1;
                    int current = index + decimalSeparatorLength;
                    for (int i = 0; i < olength; i++)
                    {
                        if (olength - i - 1 == exp)
                        {
                            //result[current + olength - i - 1] = '.';
                            //current--;
                            for (int j = 0; j < decimalSeparatorLength; j++)
                            {
                                result[current + olength - i - decimalSeparatorLength + j] = decimalSeparator[j];
                            }
                            current -= decimalSeparatorLength;
                        }
                        result[current + olength - i - 1] = (char)('0' + output % 10);
                        output /= 10;
                    }
                    //index += olength + 1;
                    index += olength + decimalSeparatorLength;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int Pow5Bits(int e)
        {
            return e == 0 ? 1 : (int)((e * LOG2_5_NUMERATOR + LOG2_5_DENOMINATOR - 1) / LOG2_5_DENOMINATOR);
        }

        /// <summary>
        /// Returns the exponent of the largest power of 5 that divides the given value, i.e., returns
        /// i such that value = 5^i * x, where x is an integer.
        /// </summary>
        private static int Pow5Factor(int value)
        {
            int count = 0;
            while (value > 0)
            {
                if (value % 5 != 0)
                {
                    return count;
                }
                value /= 5;
                count++;
            }
            throw new ArgumentException("" + value);
        }

        /// <summary>
        /// Compute the exact result of [m * 5^(-e_2) / 10^q] = [m * 5^(-e_2 - q) / 2^q]
        /// = [m * [5^(p - q)/2^k] / 2^(q - k)] = [m * POW5[i] / 2^j].
        /// </summary>
        private static ulong MulPow5divPow2(int m, int i, int j)
        {
            if (j - POW5_HALF_BITCOUNT < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(j));
            }
            return MulShift(m, POW5_SPLIT[i], j);
        }

        /// <summary>
        /// Compute the exact result of [m * 2^p / 10^q] = [m * 2^(p - q) / 5 ^ q]
        /// = [m * [2^k / 5^q] / 2^-(p - q - k)] = [m * POW5_INV[q] / 2^j].
        /// </summary>
        private static ulong MulPow5InvDivPow2(int m, int q, int j)
        {
            if (j - POW5_INV_HALF_BITCOUNT < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(j));
            }
            return MulShift(m, POW5_INV_SPLIT[q], j);
        }

        private static uint MulShift(int m, ulong factor, int shift)
        {
            // The casts here help MSVC to avoid calls to the __allmul library
            // function.
            uint factorLo = (uint)factor;
            uint factorHi = (uint)(factor >> 32);
            ulong bits0 = (ulong)m * factorLo;
            ulong bits1 = (ulong)m * factorHi;

            ulong sum = (bits0 >> 32) + bits1;
            ulong shiftedSum = sum >> (shift - 32);
            return (uint)shiftedSum;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] // J2N: Only called in one place
        private static int DecimalLength(int v)
        {
            int length = 10;
            int factor = 1000000000;
            for (; length > 0; length--)
            {
                if (v >= factor)
                {
                    break;
                }
                factor /= 10;
            }
            return length;
        }
    }
}