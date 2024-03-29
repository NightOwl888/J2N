﻿#region Copyright 2018 by Ulf Adams, Licensed under the Apache License, Version 2.0
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

using NUnit.Framework;
using System;

namespace J2N.Numerics
{
    public abstract class FloatToStringTest : TestCase
    {
        internal abstract string f(float value, RoundingMode roundingMode);

        private void assertF2sEquals(String expected, float value)
        {
            assertEquals(expected, f(value, RoundingMode.RoundEven));
            assertEquals(expected, f(value, RoundingMode.Conservative));
        }

        private void assertF2sEquals(String expectedRoundEven, String expectedConservative, float value)
        {
            assertEquals(expectedRoundEven, f(value, RoundingMode.RoundEven));
            assertEquals(expectedConservative, f(value, RoundingMode.Conservative));
        }

        [Test]
        public void simpleCases()
        {
            assertF2sEquals("0.0", 0);
            assertF2sEquals("-0.0", BitConversion.Int32BitsToSingle(unchecked((int)0x80000000)));
            assertF2sEquals("1.0", 1.0f);
            assertF2sEquals("-1.0", -1f);
            assertF2sEquals("NaN", float.NaN);
            assertF2sEquals("Infinity", float.PositiveInfinity);
            assertF2sEquals("-Infinity", float.NegativeInfinity);
        }

        [Test]
        public void switchToSubnormal()
        {
            assertF2sEquals("1.1754944E-38", BitConversion.Int32BitsToSingle(0x00800000));
        }

        /**
         * Floating point values in the range 1.0E-3 <= x < 1.0E7 have to be printed
         * without exponent. This test checks the values at those boundaries.
         */
        [Test]
        public void boundaryConditions()
        {
            // x = 1.0E7
            assertF2sEquals("1.0E7", 1.0E7f);
            // x < 1.0E7
            assertF2sEquals("9999999.0", 9999999.0f);
            // x = 1.0E-3
            assertF2sEquals("0.001", 0.001f);
            // x < 1.0E-3
            assertF2sEquals("9.999999E-4", 0.0009999999f);
        }

        [Test]
        public void minAndMax()
        {
            assertF2sEquals("3.4028235E38", BitConversion.Int32BitsToSingle(0x7f7fffff));
            assertF2sEquals("1.4E-45", BitConversion.Int32BitsToSingle(0x00000001));
        }

        [Test]
        public void roundingModeEven()
        {
            assertF2sEquals("3.355445E7", "3.3554448E7", 3.3554448E7f);
            assertF2sEquals("9.0E9", "8.999999E9", 8.999999E9f);
            assertF2sEquals("3.436672E10", "3.4366718E10", 3.4366717E10f);
        }

        [Test]
        public void roundingEvenIfTied()
        {
            assertF2sEquals("0.33007812", 0.33007812f);
        }

        [Test]
        public void looksLikePow5()
        {
            // These are all floating point numbers where the mantissa is a power of 5,
            // and the exponent is in the range such that q = 10.
            assertF2sEquals("6.7108864E17", BitConversion.Int32BitsToSingle(0x5D1502F9));
            assertF2sEquals("1.3421773E18", BitConversion.Int32BitsToSingle(0x5D9502F9));
            assertF2sEquals("2.6843546E18", BitConversion.Int32BitsToSingle(0x5E1502F9));
        }

        [Test]
        public void regressionTest()
        {
            assertF2sEquals("4.7223665E21", 4.7223665E21f);
            assertF2sEquals("8388608.0", 8388608.0f);
            assertF2sEquals("1.6777216E7", 1.6777216E7f);
            assertF2sEquals("3.3554436E7", 3.3554436E7f);
            assertF2sEquals("6.7131496E7", 6.7131496E7f);
            assertF2sEquals("1.9310392E-38", 1.9310392E-38f);
            assertF2sEquals("-2.47E-43", -2.47E-43f);
            assertF2sEquals("1.993244E-38", 1.993244E-38f);
            assertF2sEquals("4103.9004", 4103.9003f);
            assertF2sEquals("5.3399997E9", 5.3399997E9f);
            assertF2sEquals("6.0898E-39", 6.0898E-39f);
            assertF2sEquals("0.0010310042", 0.0010310042f);
            assertF2sEquals("2.882326E17", 2.8823261E17f);
            assertF2sEquals("7.038531E-26", 7.038531E-26f);
            assertF2sEquals("9.223404E17", 9.2234038E17f);
            assertF2sEquals("6.710887E7", 6.7108872E7f);
            assertF2sEquals("1.0E-44", 1.0E-44f);
            assertF2sEquals("2.816025E14", 2.816025E14f);
            assertF2sEquals("9.223372E18", 9.223372E18f);
            assertF2sEquals("1.5846086E29", 1.5846085E29f);
            assertF2sEquals("1.1811161E19", 1.1811161E19f);
            assertF2sEquals("5.368709E18", 5.368709E18f);
            assertF2sEquals("4.6143166E18", 4.6143165E18f);
            assertF2sEquals("0.007812537", 0.007812537f);
            assertF2sEquals("1.4E-45", 1.4E-45f);
            assertF2sEquals("1.18697725E20", 1.18697724E20f);
            assertF2sEquals("1.00014165E-36", 1.00014165E-36f);
            assertF2sEquals("200.0", 200f);
            assertF2sEquals("3.3554432E7", 3.3554432E7f);
        }
    }
}
