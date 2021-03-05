using Android.App;
using Android.OS;
using Xunit.Sdk;
using Xunit.Runners.UI;
using Xunit.Runners.ResultChannels;
using System.IO;

using Android.Support.V7.App;
using Xunit.Runner;

namespace J2N.Tests.Android.xUnit
{
    [Activity(Label = "xUnit Android Runner", MainLauncher = true, Theme = "@android:style/Theme.Material.Light")]
    public class MainActivity : RunnerActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            // tests can be inside the main assembly
            //AddTestAssembly(Assembly.GetExecutingAssembly());
            AddTestAssembly(Settings.TargetAssembly);

            AddExecutionAssembly(typeof(ExtensibilityPointFactory).Assembly);
            // or in any reference assemblies

            //AddTestAssembly(typeof(PortableTests).Assembly);
            // or in any assembly that you load (since JIT is available)

#if false
            // you can use the default or set your own custom writer (e.g. save to web site and tweet it ;-)
            Writer = new TcpTextWriter ("10.0.1.2", 16384);
            // start running the test suites as soon as the application is loaded
            AutoStart = true;
            // crash the application (to ensure it's ended) and return to springboard
            TerminateAfterExecution = true;
#endif
            ResultChannel = new TrxResultChannel(Path.Combine(Application.Context.GetExternalFilesDir("").AbsolutePath, Settings.TestResultFileName));

            // you cannot add more assemblies once calling base
            base.OnCreate(bundle);
        }
    }
}





// J2N NOTE: This code is for actual debugging. Unfortunately, the above activity does not scale to 77,000 tests, so we needed our own
// runner to fix the scalability issues with it and to provde the lacking TestInstrumentation for Android and blame crash functionality.

//namespace J2N.Tests.Android.xUnit
//{
//    [Activity(Label = "xUnit Android Runner 2", Theme = "@android:style/Theme.Material.Light", MainLauncher = true)]
//    public class MainActivity : AppCompatActivity
//    {
//        protected override void OnCreate(Bundle savedInstanceState)
//        {
//            base.OnCreate(savedInstanceState);
//            //Xamarin.Essentials.Platform.Init(this, savedInstanceState);
//            //// Set our view from the "main" layout resource
//            //SetContentView(Resource.Layout.activity_main);

//            PlatformHelpers.Assets = Assets;


//            string blameFilePath = Path.Combine(Application.Context.GetExternalFilesDir("").AbsolutePath, Settings.BlameFileName);
//            string resultFilePath = Path.Combine(Application.Context.GetExternalFilesDir("").AbsolutePath, Settings.TestResultFileName);
//            using (var blameWriter = new StreamWriter(blameFilePath, append: false, encoding: System.Text.Encoding.UTF8))
//            using (var resultWriter = new StreamWriter(resultFilePath, append: false, encoding: System.Text.Encoding.UTF8))
//            {
//                var runner = new Xunit2TestAssemblyRunner(blameWriter, resultWriter);
//                var resultSummary = runner.RunTests(Settings.TargetAssembly);
//            }

//        }
//        //public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
//        //{
//        //    Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

//        //    base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
//        //}
//    }
//}