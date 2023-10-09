using System;

namespace J2N
{
    public static class PlatformDetection
    {
        private static readonly bool isXamarinAndroid = LoadIsXamarinAndroid();

        private static bool LoadIsXamarinAndroid()
        {
#if NETFRAMEWORK
            return false;
#else
            var appPath = Environment.GetEnvironmentVariable("HOME");
            if (appPath is null) return false;
            string testPath = "J2N.Tests.Android";
#if NETSTANDARD2_0
            return appPath.Contains(testPath);
#else
            return appPath.Contains(testPath, StringComparison.Ordinal);
#endif
#endif
        }

        public static string BaseDirectory
        {
            get
            {
#if FEATURE_APPCONTEXT_BASEDIRECTORY
                return AppContext.BaseDirectory;
#else
                return AppDomain.CurrentDomain.BaseDirectory;
#endif
            }
        }

        public static bool IsXamarinAndroid => isXamarinAndroid;
    }
}
