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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace J2N.Numerics
{
    internal class FloatingPointFormat
    {
        public static readonly FloatingPointFormat Float16 = new FloatingPointFormat(16, 5, 10);
        public static readonly FloatingPointFormat Float32 = new FloatingPointFormat(32, 8, 23);
        public static readonly FloatingPointFormat Float64 = new FloatingPointFormat(64, 11, 52);
        public static readonly FloatingPointFormat Float80 = new FloatingPointFormat(80, 15, 63);
        public static readonly FloatingPointFormat Float128 = new FloatingPointFormat(128, 15, 112);
        public static readonly FloatingPointFormat Float256 = new FloatingPointFormat(256, 19, 236);


        private readonly int totalBits;
        private readonly int exponentBits;
        private readonly int mantissaBits;

        private FloatingPointFormat(int totalBits, int exponentBits, int mantissaBits)
        {
            this.totalBits = totalBits;
            this.exponentBits = exponentBits;
            this.mantissaBits = mantissaBits;
        }

        public int TotalBits => totalBits;
        public int ExponentBits => exponentBits;
        public int MantissaBits => mantissaBits;
        public int Bias => (1 << (exponentBits - 1)) - 1;
    }
}
