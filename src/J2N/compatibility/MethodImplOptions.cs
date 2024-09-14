using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if !FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING

namespace J2N
{
    internal static class MethodImplOptions
    {
        public const short Unmanaged = 4;
        public const short NoInlining = 8;
        public const short ForwardRef = 16;
        public const short Synchronized = 32;
        public const short NoOptimization = 64;
        public const short PreserveSig = 128;
        public const short AggressiveInlining = 256;
        public const short AggressiveOptimization = 512;
        public const short InternalCall = 4096;
    }
}

#endif