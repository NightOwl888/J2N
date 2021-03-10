using Android.App;
using System.IO;
using System.Reflection;

namespace J2N.Tests.Android
{
    public static class Settings
    {
        public static readonly Assembly TargetAssembly = typeof(global::J2N.TestTypeExtensions).Assembly;

        public static readonly string TestResultFileName = "TestResults.xml";

        public static readonly string BlameFileName = "Sequence.txt";
    }
}