using System.Reflection;

namespace J2N.Tests.Android.xUnit
{
    public static class Settings
    {
        public static readonly Assembly TargetAssembly = typeof(global::J2N.AssertExtensions).Assembly;

        public static readonly string TestResultFileName = "TestResults.xml";

        public static readonly string BlameFileName = "Sequence.txt";
    }
}