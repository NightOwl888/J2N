using Android.App;
using Android.Runtime;
using NUnit.Xamarin.Android;
using System;
using System.IO;

namespace J2N.Tests.Android
{
    [Instrumentation(Name = "app.tests.TestInstrumentation")]
    public class TestInstrumentation : TestSuiteInstrumentation
    {
        public TestInstrumentation(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer)
        {
            TestResultFilePath = Path.Combine(Application.Context.GetExternalFilesDir("").AbsolutePath, Settings.TestResultFileName);
            BlameCrashFilePath = Path.Combine(Application.Context.GetExternalFilesDir("").AbsolutePath, Settings.BlameFileName);
        }

        protected override void AddTests()
        {
            AddTest(Settings.TargetAssembly);
        }
    }
}