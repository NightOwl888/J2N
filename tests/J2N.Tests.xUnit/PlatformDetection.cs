using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace J2N
{
    public static class PlatformDetection
    {
        public static bool IsFullFramework =>
#if FEATURE_RUNTIMEINFORMATION
            RuntimeInformation.FrameworkDescription.StartsWith(".NET Framework", StringComparison.OrdinalIgnoreCase);
#else
            true;
#endif
    }
}
