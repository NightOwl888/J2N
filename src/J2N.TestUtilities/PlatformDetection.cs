using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace J2N.TestUtilities
{
    public static class PlatformDetection
    {

        public static bool IsMonoRuntime => Type.GetType("Mono.RuntimeStructs") != null;
        public static bool IsNotMonoRuntime => !IsMonoRuntime;

        public static bool IsNativeAot => IsNotMonoRuntime && !IsReflectionEmitSupported;
        public static bool IsBrowser =>
#if FEATURE_RUNTIMEINFORMATION
            RuntimeInformation.IsOSPlatform(OSPlatform.Create("BROWSER"));
#else
            false;
#endif
        public static bool IsWasi =>
#if FEATURE_RUNTIMEINFORMATION
            RuntimeInformation.IsOSPlatform(OSPlatform.Create("WASI"));
#else
            false;
#endif
        public static bool IsAndroid =>
#if FEATURE_RUNTIMEINFORMATION
            RuntimeInformation.IsOSPlatform(OSPlatform.Create("ANDROID"));
#else
            false;
#endif
        public static bool IsNotAndroid => !IsAndroid;

        public static bool IsiOS =>
#if FEATURE_RUNTIMEINFORMATION
            RuntimeInformation.IsOSPlatform(OSPlatform.Create("IOS"));
#else
            false;
#endif
        public static bool IstvOS =>
#if FEATURE_RUNTIMEINFORMATION
            RuntimeInformation.IsOSPlatform(OSPlatform.Create("TVOS"));
#else
            false;
#endif
        public static bool IsMacCatalyst =>
#if FEATURE_RUNTIMEINFORMATION
            RuntimeInformation.IsOSPlatform(OSPlatform.Create("MACCATALYST"));
#else
            false;
#endif
        public static bool IsNotMacCatalyst => !IsMacCatalyst;

        public static bool IsMobile => IsBrowser || IsWasi || IsAppleMobile || IsAndroid;
        public static bool IsNotMobile => !IsMobile;

        public static bool IsAppleMobile => IsMacCatalyst || IsiOS || IstvOS;

        public static bool IsNetFramework =>
#if FEATURE_RUNTIMEINFORMATION
            RuntimeInformation.FrameworkDescription.StartsWith(".NET Framework", StringComparison.OrdinalIgnoreCase);
#else
            true;
#endif

        public static bool IsBinaryFormatterSupported => IsNotMobile && !IsNativeAot;

        public static bool IsPreciseGcSupported => !IsMonoRuntime;

        public static bool IsThreadingSupported => (!IsWasi && !IsBrowser) || IsWasmThreadingSupported;
        public static bool IsWasmThreadingSupported => IsBrowser && IsEnvironmentVariableTrue("IsBrowserThreadingSupported");

#if NETCOREAPP
        public static bool IsReflectionEmitSupported => RuntimeFeature.IsDynamicCodeSupported;
#else
        public static bool IsReflectionEmitSupported => true;
#endif
        public static bool IsNotReflectionEmitSupported => !IsReflectionEmitSupported;

        private static volatile Tuple<bool>? s_lazyNonZeroLowerBoundArraySupported;
        public static bool IsNonZeroLowerBoundArraySupported
        {
            get
            {
                if (s_lazyNonZeroLowerBoundArraySupported == null)
                {
                    bool nonZeroLowerBoundArraysSupported = false;
                    try
                    {
                        Array.CreateInstance(typeof(int), new int[] { 5 }, new int[] { 5 });
                        nonZeroLowerBoundArraysSupported = true;
                    }
                    catch (PlatformNotSupportedException)
                    {
                    }
                    s_lazyNonZeroLowerBoundArraySupported = Tuple.Create<bool>(nonZeroLowerBoundArraysSupported);
                }
                return s_lazyNonZeroLowerBoundArraySupported.Item1;
            }
        }

        private static readonly Lazy<bool> m_isInvariant = new Lazy<bool>(()
            => (bool?)Type.GetType("System.Globalization.GlobalizationMode")?.GetProperty("Invariant", BindingFlags.NonPublic | BindingFlags.Static)?.GetValue(null) == true);

        public static bool IsInvariantGlobalization => m_isInvariant.Value;
        public static bool IsNotInvariantGlobalization => !IsInvariantGlobalization;

        private static bool IsEnvironmentVariableTrue(string variableName)
        {
            if (!IsBrowser)
                return false;

            return Environment.GetEnvironmentVariable(variableName) is "true";
        }
    }
}
