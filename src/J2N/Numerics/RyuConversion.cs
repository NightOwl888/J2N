// Copyright 2018 Ulf Adams
//
// The contents of this file may be used under the terms of the Apache License,
// Version 2.0.
//
//    (See accompanying file LICENSE-Apache or copy at
//     http://www.apache.org/licenses/LICENSE-2.0)
//
// Unless required by applicable law or agreed to in writing, this software
// is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
// KIND, either express or implied.
//
// This code was initially a mechanical convertion of C to C# by Tornhoof based
// on source from https://github.com/ulfjack/ryu/blob/master/ryu/f2s.c  
// and made available in the expandable details at https://github.com/dotnet/runtime/issues/10939
// 
// The code was then modified to improve public names, support .NET 4.51+ and to make the
// output more like that of the JDK.  

using System;

namespace J2N.Numerics {


    /// <summary>
    /// Contains floating point to string conversions that are implemented using the Ryu
    /// algorithm as documented at https://github.com/ulfjack/ryu  A goal of the public methods
    /// in this class was to have the conversion happen in a way that is consisiten with the
    /// output of such conversions performed via a JDK.
    /// </summary>
    public class RyuConversion {
        private const int FLOAT_MANTISSA_BITS = 23;
        private const int FLOAT_EXPONENT_BITS = 8;

        private const int FLOAT_POW5_INV_BITCOUNT = 59;

        private static readonly ulong[] FLOAT_POW5_INV_SPLIT =
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

        private const int FLOAT_POW5_BITCOUNT = 61;

        private static readonly ulong[] FLOAT_POW5_SPLIT =
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

        private static uint pow5Factor(uint value) {
            uint count = 0;
            for (; ; )
            {
                uint q = value / 5;
                uint r = value % 5;
                if (r != 0) break;

                value = q;
                ++count;
            }

            return count;
        }

        private static bool multipleOfPowerOf5(uint value, uint p) {
            return pow5Factor(value) >= p;
        }

        private static bool multipleOfPowerOf2(uint value, uint p) {
            // return __builtin_ctz(value) >= p;
            return (value & ((1u << (int)p) - 1)) == 0;
        }

        private static uint mulShift(uint m, ulong factor, int shift) {
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

        // Returns e == 0 ? 1 : ceil(log_2(5^e)).
        private static uint pow5bits(int e) {
            // This approximation works up to the point that the multiplication overflows at e = 3529.
            // If the multiplication were done in 64 bits, it would fail at 5^4004 which is just greater
            // than 2^9297.

            return (((uint)e * 1217359) >> 19) + 1;
        }

        // Returns floor(log_10(2^e)).
        private static int log10Pow2(int e) {
            // The first value this approximation fails for is 2^1651 which is just greater than 10^297.
            return (int)(((uint)e * 78913) >> 18);
        }

        // Returns floor(log_10(5^e)).
        private static int log10Pow5(int e) {
            // The first value this approximation fails for is 5^2621 which is just greater than 10^1832.
            return (int)(((uint)e * 732923) >> 20);
        }

        private static uint mulPow5InvDivPow2(uint m, uint q, int j) {
            return mulShift(m, FLOAT_POW5_INV_SPLIT[q], j);
        }

        private static uint mulPow5divPow2(uint m, uint i, int j) {
            return mulShift(m, FLOAT_POW5_SPLIT[i], j);
        }

        private static uint decimalLength(uint v) {
            // Function precondition: v is not a 10-digit number.
            // (9 digits are sufficient for round-tripping.)
            if (v >= 100000000) return 9;

            if (v >= 10000000) return 8;

            if (v >= 1000000) return 7;

            if (v >= 100000) return 6;

            if (v >= 10000) return 5;

            if (v >= 1000) return 4;

            if (v >= 100) return 3;

            if (v >= 10) return 2;

            return 1;
        }

        // A floating decimal representing m * 10^e.
        private struct floating_decimal_32 {
            public uint mantissa;
            public int exponent;
        }

        private static floating_decimal_32 f2d(uint ieeeMantissa, uint ieeeExponent) {
            uint bias = (1u << (FLOAT_EXPONENT_BITS - 1)) - 1;

            int e2;
            uint m2;
            if (ieeeExponent == 0) {
                // We subtract 2 so that the bounds computation has 2 additional bits.
                e2 = (int)(1 - bias - FLOAT_MANTISSA_BITS - 2);
                m2 = ieeeMantissa;
            } else {
                e2 = (int)(ieeeExponent - bias - FLOAT_MANTISSA_BITS - 2);
                m2 = (1u << FLOAT_MANTISSA_BITS) | ieeeMantissa;
            }

            bool even = (m2 & 1) == 0;
            bool acceptBounds = even;


            // Step 2: Determine the interval of legal decimal representations.
            uint mv = 4 * m2;
            uint mp = 4 * m2 + 2;
            // Implicit bool -> int conversion. True is 1, false is 0.
            bool mmShift = ieeeMantissa != 0 || ieeeExponent <= 1;
            uint mm = (uint)(4 * m2 - 1 - (mmShift ? 1 : 0));

            // Step 3: Convert to a decimal power base using 64-bit arithmetic.
            uint vr, vp, vm;
            int e10;
            bool vmIsTrailingZeros = false;
            bool vrIsTrailingZeros = false;
            byte lastRemovedDigit = 0;
            if (e2 >= 0) {
                uint q = (uint)log10Pow2(e2);
                e10 = (int)q;
                int k = (int)(FLOAT_POW5_INV_BITCOUNT + pow5bits((int)q) - 1);
                int i = (int)(-e2 + q + k);
                vr = mulPow5InvDivPow2(mv, q, i);
                vp = mulPow5InvDivPow2(mp, q, i);
                vm = mulPow5InvDivPow2(mm, q, i);
                if (q != 0 && (vp - 1) / 10 <= vm / 10) {
                    // We need to know one removed digit even if we are not going to loop below. We could use
                    // q = X - 1 above, except that would require 33 bits for the result, and we've found that
                    // 32-bit arithmetic is faster even on 64-bit machines.
                    int l = (int)(FLOAT_POW5_INV_BITCOUNT + pow5bits((int)(q - 1)) - 1);
                    lastRemovedDigit = (byte)mulPow5InvDivPow2(mv, q - 1, (int)(-e2 + q - 1 + l) % 10);
                }

                if (q <= 9) {
                    // The largest power of 5 that fits in 24 bits is 5^10, but q <= 9 seems to be safe as well.
                    // Only one of mp, mv, and mm can be a multiple of 5, if any.
                    if (mv % 5 == 0)
                        vrIsTrailingZeros = multipleOfPowerOf5(mv, q);
                    else if (acceptBounds)
                        vmIsTrailingZeros = multipleOfPowerOf5(mm, q);
                    else
                        vp -= (uint)(multipleOfPowerOf5(mp, q) ? 1 : 0);
                }
            } else {
                uint q = (uint)log10Pow5(-e2);
                e10 = (int)q + e2;
                int i = (int)(-e2 - q);
                int k = (int)(pow5bits(i) - FLOAT_POW5_BITCOUNT);
                int j = (int)(q - k);
                vr = mulPow5divPow2(mv, (uint)i, j);
                vp = mulPow5divPow2(mp, (uint)i, j);
                vm = mulPow5divPow2(mm, (uint)i, j);
                if (q != 0 && (vp - 1) / 10 <= vm / 10) {
                    j = (int)(q - 1 - (pow5bits(i + 1) - FLOAT_POW5_BITCOUNT));
                    lastRemovedDigit = (byte)(mulPow5divPow2(mv, (uint)i + 1, j) % 10);
                }

                if (q <= 1) {
                    // {vr,vp,vm} is trailing zeros if {mv,mp,mm} has at least q trailing 0 bits.
                    // mv = 4 * m2, so it always has at least two trailing 0 bits.
                    vrIsTrailingZeros = true;
                    if (acceptBounds)
                        vmIsTrailingZeros = mmShift;
                    else
                        --vp;
                } else if (q < 31) {
                    // TODO(ulfjack): Use a tighter bound here.
                    vrIsTrailingZeros = multipleOfPowerOf2(mv, q - 1);
                }
            }

            // Step 4: Find the shortest decimal representation in the interval of legal representations.
            uint removed = 0;
            uint output;
            if (vmIsTrailingZeros || vrIsTrailingZeros) {
                // General case, which happens rarely (~4.0%).
                while (vp / 10 > vm / 10) {
                    vmIsTrailingZeros &= vm - vm / 10 * 10 == 0;
                    vrIsTrailingZeros &= lastRemovedDigit == 0;
                    lastRemovedDigit = (byte)(vr % 10);
                    vr /= 10;
                    vp /= 10;
                    vm /= 10;
                    ++removed;
                }

                if (vmIsTrailingZeros)
                    while (vm % 10 == 0) {
                        vrIsTrailingZeros &= lastRemovedDigit == 0;
                        lastRemovedDigit = (byte)(vr % 10);
                        vr /= 10;
                        vp /= 10;
                        vm /= 10;
                        ++removed;
                    }

                if (vrIsTrailingZeros && lastRemovedDigit == 5 && vr % 2 == 0) lastRemovedDigit = 4;

                // We need to take vr + 1 if vr is outside bounds or we need to round up.
                output = (uint)(vr +
                                 (vr == vm && (!acceptBounds || !vmIsTrailingZeros) || lastRemovedDigit >= 5
                                     ? 1
                                     : 0));
            } else {
                // Specialized for the common case (~96.0%). Percentages below are relative to this.
                // Loop iterations below (approximately):
                // 0: 13.6%, 1: 70.7%, 2: 14.1%, 3: 1.39%, 4: 0.14%, 5+: 0.01%
                while (vp / 10 > vm / 10) {
                    lastRemovedDigit = (byte)(vr % 10);
                    vr /= 10;
                    vp /= 10;
                    vm /= 10;
                    ++removed;
                }

                // We need to take vr + 1 if vr is outside bounds or we need to round up.
                output = (uint)(vr + (vr == vm || lastRemovedDigit >= 5 ? 1 : 0));
            }

            int exp = (int)(e10 + removed);


            floating_decimal_32 fd;
            fd.exponent = exp;
            fd.mantissa = output;
            return fd;
        }


        //Modified to conform to Apache Harmony use of scientific notation.
        private static int to_chars(floating_decimal_32 v, bool sign, float f, char[] result) {
            // Step 5: Print the decimal representation.
            int index = 0;
            if (sign) result[index++] = '-';

            uint output = v.mantissa;
            uint olength = decimalLength(output);

            //These thresholds based on Apache Harmony NumberConverter class.
            if (f >= 1e7f || f <= -1e7f || (f > -1e-3f && f < 1e-3f)) {
                //Print using scientific notation

                for (uint i = 0; i < olength - 1; ++i) {
                    uint c = output % 10;
                    output /= 10;
                    result[(int)(index + olength - i)] = (char)('0' + c);
                }

                result[index] = (char)('0' + output % 10);

                if (olength > 1) {
                    result[index + 1] = '.';
                    index += (int)(olength + 1);
                } else {
                    index++;
                    result[index++] = '.';
                    result[index++] = '0';
                }

                // Print the exponent.
                result[index++] = 'E';
                int exp = (int)(v.exponent + olength - 1);

                if (exp < 0) {
                    result[index++] = '-';
                    exp = -exp;
                }

                if (exp >= 100)
                    result[index++] = (char)('0' + exp / 100);

                if (exp >= 10)
                    result[index++] = (char)('0' + exp / 10 % 10);

                result[index++] = (char)('0' + exp % 10);

            } else {
                //Print w/o scientifc notation.
                if (v.exponent > 0) {
                    throw new Exception("Positive exponent is unexpected.");
                }


                int length = (int)olength;
                if (v.exponent < 0) {
                    length++;              // make space for period
                }

                for (int i = 0; i < length; i++) {           //i is the char index
                    uint c = output % 10;
                    output /= 10;
                    if (v.exponent != 0 && i == -1 * v.exponent) {
                        result[(int)(index + length - i - 1)] = '.';
                        i++;
                        if (-1 * v.exponent == olength) {
                            break;      //we're done.  leads with a .
                        }
                    }

                    result[(int)(index + length - i - 1)] = (char)('0' + c);
                }

                index = (int)length;
            }


            return index;
        }


        private static int f2s_buffered_n(float f, char[] result) {
            // Step 1: Decode the floating-point number, and unify normalized and subnormal cases.
            uint bits = float_to_bits(f);


            // Decode bits into sign, mantissa, and exponent.
            bool ieeeSign = ((bits >> (FLOAT_MANTISSA_BITS + FLOAT_EXPONENT_BITS)) & 1) != 0;
            uint ieeeMantissa = bits & ((1u << FLOAT_MANTISSA_BITS) - 1);
            uint ieeeExponent = (bits >> FLOAT_MANTISSA_BITS) & ((1u << FLOAT_EXPONENT_BITS) - 1);

            // Case distinction; exit early for the easy cases.
            if (ieeeExponent == (1u << FLOAT_EXPONENT_BITS) - 1u || ieeeExponent == 0 && ieeeMantissa == 0)
                return copy_special_str(result, ieeeSign, ieeeExponent == 0, ieeeMantissa == 0);

            floating_decimal_32 v = f2d(ieeeMantissa, ieeeExponent);
            return to_chars(v, ieeeSign, f, result);
        }

        private static int copy_special_str(char[] result, bool sign, bool exponent, bool mantissa) {
            if (mantissa) {
                result[0] = 'N';
                result[1] = 'a';
                result[2] = 'N';
                return 3;
            }

            if (sign) result[0] = '-';

            if (exponent) {
                result[1] = 'I';
                result[2] = 'n';
                result[3] = 'f';
                result[4] = 'i';
                result[5] = 'n';
                result[6] = 'i';
                result[7] = 't';
                result[8] = 'y';
                return 9;
            }

            result[1] = '0';
            result[2] = 'E';
            result[3] = '0';
            return 4;
        }


        /// <summary>
        /// Converts a float to a string with an output consistent with how a Java
        /// JVM would output the string.
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public static string FloatToString(float f) {
            char[] result = new char[16];
            int count = f2s_buffered_n(f, result);

            char[] chars = new char[count];

            for (int i = 0; i < count; i++) {       //Initially tried using Buffer.BlockCopy but was having issue with it so went with loop.
                chars[i] = result[i];
            }
            return new string(chars);
        }

        private static uint float_to_bits(float f) {
            uint bits = 0;
            byte[] bytes = BitConverter.GetBytes(f);
            bits = BitConverter.ToUInt32(bytes, 0);
            return bits;
        }
    }
}