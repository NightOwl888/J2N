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

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace J2N.Numerics
{
    /// <summary>
    /// Implementation of Float and Double to String conversion.
    /// </summary>
    internal sealed class SlowConversion
    {
        private static readonly BigInteger TWO = 2;
        private static readonly BigInteger TEN = 2;

        private static bool DEBUG = false;
        private static readonly bool DEBUG_FLOAT = true;

        public static void Main(string[] args)
        {
            DEBUG = true;
            if (DEBUG_FLOAT)
            {
                float f = 0.33007812f;
                string result = floatToString(f);
                Console.WriteLine(result + " " + f);
            }
            else
            {
                double f = 1.1873267205539228E-308;
                string result = doubleToString(f);
                Console.WriteLine(result + " " + f);
            }
        }

        public static string floatToString(float value)
        {
            return floatToString(value, RoundingMode.RoundEven);
        }

        public static string floatToString(float value, RoundingMode roundingMode)
        {
            if (DEBUG) Console.WriteLine("VALUE=" + value);
            long bits = BitConversion.SingleToInt32Bits(value) & 0xffffffffL;
            return asString(bits, FloatingPointFormat.Float32, roundingMode);
        }

        public static string doubleToString(double value)
        {
            return doubleToString(value, RoundingMode.RoundEven);
        }

        public static string doubleToString(double value, RoundingMode roundingMode)
        {
            if (DEBUG) Console.WriteLine("VALUE=" + value);
            long bits = BitConversion.DoubleToInt64Bits(value);
            return asString(bits, FloatingPointFormat.Float64, roundingMode);
        }

        internal static string asString(long bits, FloatingPointFormat format, RoundingMode mode)
        {
            // Step 1: Decode the floating point number, and unify normalized and subnormal cases.
            //
            // The format of all IEEE numbers is S E* M*; we obtain M by masking the lower M bits, E by
            // shifting and masking, and S also by shifting and masking.
            int mantissaBits = format.MantissaBits;
            int exponentBits = format.ExponentBits;

            int ieeeExponent = (int)((bits.TripleShift(mantissaBits)) & ((1 << exponentBits) - 1));
            long ieeeMantissa = bits & ((1L << mantissaBits) - 1);
            bool sign = ((bits.TripleShift(mantissaBits + exponentBits)) & 1) != 0;
            bool even = (bits & 1) == 0;

            // Exit early if it's NaN, Infinity, or 0.
            if (ieeeExponent == ((1 << exponentBits) - 1))
            {
                // Handle the special cases where the exponent is all 1s indicating NaN or Infinity: if the
                // mantissa is non-zero, it's a NaN, otherwise it's +/-infinity.
                return (ieeeMantissa != 0) ? "NaN" : sign ? "-Infinity" : "Infinity";
            }
            else if ((ieeeExponent == 0) && (ieeeMantissa == 0))
            {
                // If the mantissa is 0, the code below would end up with a lower bound that is less than 0,
                // which throws off the char-by-char comparison. Instead, we exit here with the correct
                // string.
                return sign ? "-0.0" : "0.0";
            }

            // Compute the offset used by the IEEE format.
            int offset = (1 << (exponentBits - 1)) - 1;

            // Unify normalized and subnormal cases.
            int e2;
            long m2;
            if (ieeeExponent == 0)
            {
                e2 = 1 - offset - mantissaBits;
                m2 = ieeeMantissa;
            }
            else
            {
                e2 = ieeeExponent - offset - mantissaBits;
                m2 = ieeeMantissa | (1L << mantissaBits);
            }

            // Step 2: Determine the interval of legal decimal representations.
            long mv = 4 * m2;
            long mp = 4 * m2 + 2;
            long mm = 4 * m2 - (((m2 != (1L << mantissaBits)) || (ieeeExponent <= 1)) ? 2 : 1);
            e2 -= 2;

            // Step 3: Convert to a decimal power base using arbitrary-precision arithmetic.
            BigInteger vr, vp, vm;
            int e10;
            if (e2 >= 0)
            {
                vr = (BigInteger)mv << e2;
                vp = (BigInteger)mp << e2;
                vm = (BigInteger)mm << e2;
                e10 = 0;
            }
            else
            {
                BigInteger factor = BigInteger.Pow(5, -e2);
                vr = (BigInteger)mv * factor;
                vp = (BigInteger)mp * factor;
                vm = (BigInteger)mm * factor;
                e10 = e2;
            }

            // Step 4: Find the shortest decimal representation in the interval of legal representations.
            //
            // We do some extra work here in order to follow Float/Double.toString semantics. In particular,
            // that requires printing in scientific format if and only if the exponent is between -3 and 7,
            // and it requires printing at least two decimal digits.
            //
            // Above, we moved the decimal dot all the way to the right, so now we need to count digits to
            // figure out the correct exponent for scientific notation.
            int vpLength = vp.ToString().Length;
            e10 += vpLength - 1;
            bool scientificNotation = (e10 < -3) || (e10 >= 7);

            if (DEBUG)
            {
                Console.WriteLine("IN=" + bits.ToBinaryString());
                Console.WriteLine("   S=" + (sign ? "-" : "+") + " E=" + e2 + " M=" + m2);
                Console.WriteLine("E =" + e10);
                Console.WriteLine("V+=" + vp);
                Console.WriteLine("V =" + vr);
                Console.WriteLine("V-=" + vm);
            }

            if (!mode.AcceptUpperBound(even))
            {
                vp = vp - (BigInteger.One);
            }
            bool vmIsTrailingZeros = true;
            // Track if vr is tailing zeroes _after_ lastRemovedDigit.
            bool vrIsTrailingZeros = true;
            int removed = 0;
            int lastRemovedDigit = 0;
            while (!(vp / TEN).Equals(vm / TEN))
            {
                if (scientificNotation && vp.CompareTo(new BigInteger(100)) < 0)
                {
                    // Float/Double.toString semantics requires printing at least two digits.
                    break;
                }
                vmIsTrailingZeros &= vm % TEN /*).intValueExact()*/ == 0;
                vrIsTrailingZeros &= lastRemovedDigit == 0;
                lastRemovedDigit = (int)(vr % TEN) /*).intValueExact()*/;
                vp = vp / TEN;
                vr = vr / TEN;
                vm = vm / TEN;
                removed++;
            }
            if (vmIsTrailingZeros && mode.AcceptLowerBound(even))
            {
                while (vm % TEN /*).intValueExact()*/ == 0)
                {
                    if (scientificNotation && vp.CompareTo((BigInteger)100) < 0)
                    {
                        // Float/Double.toString semantics requires printing at least two digits.
                        break;
                    }
                    vrIsTrailingZeros &= lastRemovedDigit == 0;
                    lastRemovedDigit = (int)(vr % TEN) /*).intValueExact()*/;
                    vp = vp / TEN;
                    vr = vr / TEN;
                    vm = vm / TEN;
                    removed++;
                }
            }
            if (vrIsTrailingZeros && (lastRemovedDigit == 5) && (vr % TWO /*).intValueExact()*/ == 0))
            {
                // Round down not up if the number ends in X50000 and the number is even.
                lastRemovedDigit = 4;
            }
            string output = ((vr.CompareTo(vm) > 0) ? (lastRemovedDigit >= 5 ? vr + BigInteger.One : vr) : vp).ToString(CultureInfo.InvariantCulture);
            int olength = vpLength - removed;

            if (DEBUG)
            {
                Console.WriteLine("LRD=" + lastRemovedDigit);
                Console.WriteLine("VP=" + vp);
                Console.WriteLine("VR=" + vr);
                Console.WriteLine("VM=" + vm);
                Console.WriteLine("O=" + output);
                Console.WriteLine("OLEN=" + olength);
                Console.WriteLine("EXP=" + e10);
            }

            // Step 5: Print the decimal representation.
            // We follow Float/Double.toString semantics here.
            StringBuilder result = new StringBuilder();
            // Add the minus sign if the number is negative.
            if (sign)
            {
                result.Append('-');
            }

            if (scientificNotation)
            {
                result.Append(output[0]);
                result.Append('.');
                for (int i = 1; i < olength; i++)
                {
                    result.Append(output[i]);
                }
                if (olength == 1)
                {
                    result.Append('0');
                }
                result.Append('E');
                result.Append(e10);
                return result.ToString();
            }
            else
            {
                // Print leading 0s and '.' if applicable.
                for (int i = 0; i > e10; i--)
                {
                    result.Append('0');
                    if (i == 0)
                    {
                        result.Append(".");
                    }
                }
                // Print number and '.' if applicable.
                for (int i = 0; i < olength; i++)
                {
                    result.Append(output[i]);
                    if (e10 == 0)
                    {
                        result.Append('.');
                    }
                    e10--;
                }
                // Print trailing 0s and '.' if applicable.
                for (; e10 >= -1; e10--)
                {
                    result.Append('0');
                    if (e10 == 0)
                    {
                        result.Append('.');
                    }
                }
                return result.ToString();
            }
        }
    }
}
