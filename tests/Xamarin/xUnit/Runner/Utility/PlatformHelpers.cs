using Android.Content.Res;
using System;
using System.Diagnostics.CodeAnalysis;
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

        [SuppressMessage("Style", "IDE0034:Remove unused parameter", Justification = "May be useful someday")]
        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "May be useful someday")]
        [SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression", Justification = "IDE0034 and IDE0060 don't fire on all target frameworks")]
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