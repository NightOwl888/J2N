using Android.App;
using Android.Content.PM;
using Android.OS;
using NUnit.Framework.Api;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Runner;
using NUnit.Runner.Services;
using System;
using System.Collections.Generic;
using System.IO;

namespace J2N.Tests.Android
{
    [Activity(Label = "@string/app_name", Icon = "@drawable/icon", Theme = "@style/AppTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsApplicationActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            // This will load all tests within the current project
            var nunit = new NUnit.Runner.App();

            // If you want to add tests in another assembly
            //nunit.AddTestAssembly(typeof(SomeTests).Assembly);
            //var anassembly = typeof(global::Lucene.Net.Util.Test2BPagedBytes).Assembly;
            nunit.AddTestAssembly(Settings.TargetAssembly);

            // Available options for testing
            nunit.Options = new TestOptions
            {
                // If True, the tests will run automatically when the app starts
                // otherwise you must run them manually.
                //AutoRun = true,

                // If True, the application will terminate automatically after running the tests.
                //TerminateAfterExecution = true,

                // Information about the tcp listener host and port.
                // For now, send result as XML to the listening server.
                //TcpWriterParameters = new TcpWriterInfo("192.168.0.108", 13000),

                // Creates a NUnit Xml result file on the host file system using PCLStorage library.
                CreateXmlResultFile = true,

                // Choose a different path for the xml result file
                ResultFilePath = Path.Combine(Application.Context.GetExternalFilesDir("").AbsolutePath, Settings.TestResultFileName)
            };

            LoadApplication(nunit);


            //// Use the runner directly with BlameListener to detect which tests crash the runner
            //string blameFilePath = Path.Combine(Application.Context.GetExternalFilesDir("").AbsolutePath, Settings.BlameFileName);
            //using (var blameWriter = new StreamWriter(blameFilePath, append: false, encoding: System.Text.Encoding.UTF8))
            //{
            //    NUnitTestAssemblyRunner runner = new NUnitTestAssemblyRunner(new DefaultTestAssemblyBuilder());
            //    runner.Load(Settings.TargetAssembly, new Dictionary<string, object>());

            //    var filter = new MyFilter();
            //    //var filter = TestFilter.Empty;


            //    ITestResult result = runner.Run(new BlameListener(TestListener.NULL, blameWriter), filter);
            //    var startTime = DateTime.Now;
            //    var resultsXml = new NUnit3XmlOutputWriter(startTime);
            //    resultsXml.WriteResultFile(result, nunit.Options.ResultFilePath);
            //}
        }
    }

    public class MyFilter : TestFilter
    {
        public override TNode AddToXml(TNode parentNode, bool recursive)
        {
            return TestFilter.Empty.AddToXml(parentNode, recursive);
        }
        public override bool Match(ITest test)
        {
            return (test.MethodName ?? "").Equals("TestPlatformDetection");
        }
    }

    public class BlameListener : ITestListener
    {
        private readonly ITestListener innerListener;
        private readonly TextWriter Writer;

        public BlameListener(ITestListener innerListener, TextWriter writer)
        {
            this.Writer = writer ?? throw new ArgumentNullException(nameof(writer));
            this.innerListener = innerListener ?? TestListener.NULL;
        }

        public void TestFinished(ITestResult result)
        {
            if (!(result.Test is TestSuite))
            {
                Writer.Write(": [");
                Writer.Write(result.ResultState.Status.ToString().ToUpperInvariant());
                Writer.Write("]");
                Writer.WriteLine();
            }

            innerListener.TestFinished(result);
        }
        public void TestOutput(TestOutput testOutput)
        {
            innerListener.TestOutput(testOutput);
        }
        public void TestStarted(ITest test)
        {
            if (test is TestMethod)
            {
                if (!(test.Method.MethodInfo is null))
                {
                    Writer.Write(test.Method.MethodInfo.ReflectedType.ToString());
                    Writer.Write('.');
                }
                Writer.Write(test.Name);
                Writer.Flush(); // Sometimes the test fails before it completes crashing the runtime, it's good to have the name of the test in the output
            }

            innerListener.TestStarted(test);
        }
    }
}