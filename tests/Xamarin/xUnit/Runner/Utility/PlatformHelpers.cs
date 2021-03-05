using Android.Content.Res;
using System;
using System.IO;
using System.Linq;

namespace Xunit.Runner
{
    static class PlatformHelpers
    {
        public static void TerminateWithSuccess()
        {
            Environment.Exit(0);
        }

        public static Stream ReadConfigJson(string assemblyName)
        {
            //var assets = Assets.List(string.Empty);
            //if (assets.Contains($"{assemblyName}.xunit.runner.json"))
            //    return Assets.Open($"{assemblyName}.xunit.runner.json");

            //if (assets.Contains("xunit.runner.json"))
            //    return Assets.Open("xunit.runner.json");

            return null;
        }

        public static AssetManager Assets { get; set; }
    }
}