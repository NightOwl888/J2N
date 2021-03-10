using Android.App;
using Android.OS;
using Android.Runtime;
using System;
using System.IO;
using Xunit.Runner;

namespace J2N.Tests.Android.xUnit
{
    [Instrumentation(Name = "app.tests.TestInstrumentation")]
    public class TestInstrumentation : Instrumentation
    {
        public TestInstrumentation(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer)
        {
        }

        /// <summary>
        /// Gets or sets the path of the test results XML file.
        /// </summary>
        protected string TestResultFilePath { get; set; } = Path.Combine(Application.Context.GetExternalFilesDir("").AbsolutePath, Settings.TestResultFileName);

        /// <summary>
        /// Gets or sets the path of the blame sequence log file.
        /// This file outputs the name of each test method before it starts running and
        /// the test result after it is complete. If the test runner crashes, it will
        /// the last test method name in the file will be the source of the crash.
        /// </summary>
        protected string BlameCrashFilePath { get; set; } = Path.Combine(Application.Context.GetExternalFilesDir("").AbsolutePath, Settings.BlameFileName);

        public override void OnCreate(Bundle arguments)
        {
            base.OnCreate(arguments);
            Start();
        }

        public override void OnStart()
        {
            base.OnStart();

            var results = new Bundle();
            int failed = 0;
            try
            {
                results.PutString("blame-crash-path", ToAdbPath(BlameCrashFilePath));
                results.PutString("xunit2-results-path", ToAdbPath(TestResultFilePath));

                Xunit.ExecutionSummary result;

                using (var blameWriter = new StreamWriter(BlameCrashFilePath, append: false, encoding: System.Text.Encoding.UTF8))
                using (var resultWriter = new StreamWriter(TestResultFilePath, append: false, encoding: System.Text.Encoding.UTF8))
                {
                    var runner = new Xunit2TestAssemblyRunner(blameWriter, resultWriter);
                    result = runner.RunTests(Settings.TargetAssembly);
                }

                int run = 0, passed = 0, skipped = 0, inconclusive = 0;
                run += 1;
                inconclusive += 0;
                failed += result.Failed;
                passed += result.Total - (result.Skipped + result.Failed);
                skipped += result.Skipped;
                //if (result.Failed > 0)
                //{
                //    //Log.Error(TAG, "Test '{0}' failed: {1}", result.FullName, result.Message);
                //    // Avoid Java.Lang.NullPointerException: println needs a message
                //    // if (!String.IsNullOrEmpty(result.StackTrace))
                //    //     Log.Error(TAG, result.StackTrace);
                //    results.PutString("failure: " + result.FullName,
                //            result.Message + "\n" + result.StackTrace);
                //    //Log.Error(TAG, "  "); // makes it easier to read the failures in logcat output
                //}

                results.PutInt("run", run);
                results.PutInt("passed", passed);
                results.PutInt("failed", failed);
                results.PutInt("skipped", skipped);
                results.PutInt("inconclusive", inconclusive);
                results.PutInt("total", result.Total);

                TimeSpan delta = TimeSpan.FromSeconds(Convert.ToDouble(result.Time)); //testResult.EndTime - testResult.StartTime;
                string totalElapsed = string.Format("{0:00}:{1:00}:{2:00}", delta.Hours, delta.Minutes, delta.Seconds);
                results.PutString("elapsed", totalElapsed);
            }
            catch (Exception e)
            {
                //Log.Error(TAG, "Error: {0}", e);
                results.PutString("error", e.ToString());
            }

            Finish(failed == 0 ? Result.Ok : Result.Canceled, results);
        }

        // On some Android targets, the external storage directory is "emulated",
        // in which case the paths used on-device by the application are *not*
        // paths that can be used off-device with `adb pull`.
        // For example, `Contxt.GetExternalFilesDir()` may return `/storage/emulated/foo`,
        // but `adb pull /storage/emulated/foo` will *fail*; instead, we may need
        // `adb pull /mnt/shell/emulated/foo`.
        // The `$EMULATED_STORAGE_SOURCE` and `$EMULATED_STORAGE_TARGET` environment
        // variables control the "on-device" (`$EMULATED_STORAGE_TARGET`) and
        // "off-device" (`$EMULATED_STORAGE_SOURCE`) directory prefixes
        string ToAdbPath(string path)
        {
            var source = System.Environment.GetEnvironmentVariable("EMULATED_STORAGE_SOURCE");
            var target = System.Environment.GetEnvironmentVariable("EMULATED_STORAGE_TARGET");

            if (!string.IsNullOrEmpty(source) && !string.IsNullOrEmpty(target) && path.StartsWith(target, StringComparison.Ordinal))
            {
                return path.Replace(target, source);
            }
            return path;
        }
    }
}