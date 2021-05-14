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
