using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Xunit;
using Xunit.Abstractions;
using VsTestCase = Microsoft.VisualStudio.TestPlatform.ObjectModel.TestCase;

namespace Xunit.Runner
{
    /// <summary>
    /// A basic sequential runner for xUnit that includes blame crash handling and
    /// TRX errror reporting.
    /// </summary>
    public class Xunit2TestAssemblyRunner
    {
        public const string ExecutorUri = "executor://xunit/XamarinTestRunner2/android";

        static readonly Uri uri = new Uri(ExecutorUri);

        private readonly TextWriter blameWriter;
        private readonly TextWriter resultsWriter;

        public Xunit2TestAssemblyRunner(TextWriter blameWriter, TextWriter resultsWriter)
        {
            this.blameWriter = blameWriter ?? throw new ArgumentNullException(nameof(blameWriter));
            this.resultsWriter = resultsWriter ?? throw new ArgumentNullException(nameof(resultsWriter));
        }

        public ExecutionSummary RunTests(Assembly assembly)
        {
            return RunTestsInAssembly(new AssemblyRunInfo
            {
                Assembly = assembly,
                AssemblyFileName = GetAssemblyFileName(assembly),
                Configuration = GetConfiguration(assembly.GetName().Name)
            });
        }


        static string GetAssemblyFileName(Assembly assembly)
        {
            var codeBase = assembly.CodeBase;
            return codeBase.Substring(7);
        }


        static List<ITestCase> GetTestCases(AssemblyRunInfo runInfo, ITestFrameworkDiscoverer discoverer)
        {
            string assemblyFileName = runInfo.AssemblyFileName;
            // Xunit needs the file name

            var configuration = runInfo.Configuration;
            var discoveryOptions = TestFrameworkOptions.ForDiscovery(configuration);

            using (var sink = new TestDiscoverySink())
            {
                discoverer.Find(includeSourceInformation: false, sink, discoveryOptions);
                sink.Finished.WaitOne();

                return sink.TestCases;
            }
        }


        static TestAssemblyConfiguration GetConfiguration(string assemblyName)
        {
            var stream = GetConfigurationStreamForAssembly(assemblyName);
            if (stream != null)
            {
                using (stream)
                {
                    return ConfigReader.Load(stream);
                }
            }

            return new TestAssemblyConfiguration();
        }

        static Stream GetConfigurationStreamForAssembly(string assemblyName)
        {
            // Android needs to read the config from its asset manager
            return PlatformHelpers.ReadConfigJson(assemblyName);
        }

        ExecutionSummary RunTestsInAssembly(AssemblyRunInfo runInfo)
        {
            //var assemblyDisplayName = "(unknown assembly)";

            //try
            //{
                var assembly = new XunitProjectAssembly { AssemblyFilename = runInfo.AssemblyFileName };
                var assemblyFileName = runInfo.AssemblyFileName;
                //assemblyDisplayName = Path.GetFileNameWithoutExtension(assemblyFileName);
                //var configuration = runInfo.Configuration;
                //var shadowCopy = configuration.ShadowCopyOrDefault;

                var appDomain = assembly.Configuration.AppDomain ?? AppDomainSupport.Denied;
                //var longRunningSeconds = assembly.Configuration.LongRunningTestSecondsOrDefault;

                runInfo.Configuration.PreEnumerateTheories = true;

                //if (runSettings.DisableAppDomain)
                //    appDomain = AppDomainSupport.Denied;

//#if WINDOWS_UAP
//                // For AppX Apps, use the package location
//                assemblyFileName = Path.Combine(Windows.ApplicationModel.Package.Current.InstalledLocation.Path, Path.GetFileName(assemblyFileName));
//#endif

                //var diagnosticSink = DiagnosticMessageSink.ForDiagnostics(logger, assemblyDisplayName, runInfo.Configuration.DiagnosticMessagesOrDefault);
                //var diagnosticMessageSink = MessageSinkAdapter.Wrap(diagnosticSink);
                using (var controller = new XunitFrontController(appDomain, assemblyFileName/*, shadowCopy: shadowCopy, diagnosticMessageSink: diagnosticMessageSink*/))
                {
                    // Execute tests
                    var executionOptions = TestFrameworkOptions.ForExecution(runInfo.Configuration);
                    executionOptions.SetSynchronousMessageReporting(true);
                    executionOptions.SetDisableParallelization(true);


                    var testCases = GetTestCases(runInfo, controller);
                    var discoveredTestCases = GetVsTestCases(assemblyFileName, controller, testCases);
                    var testCasesMap = new Dictionary<string, VsTestCase>(StringComparer.Ordinal);
                    var uniqueTestCases = new List<ITestCase>();
                    foreach (var discoveredTestCase in discoveredTestCases)
                    {
                        var uniqueID = discoveredTestCase.UniqueID;
                        if (testCasesMap.ContainsKey(uniqueID))
                        {
                            //logger.LogWarning(filteredTestCase.TestCase, "Skipping test case with duplicate ID '{0}' ('{1}' and '{2}')", uniqueID, testCasesMap[uniqueID].DisplayName, filteredTestCase.VSTestCase.DisplayName);
                        }
                        else
                        {
                            testCasesMap.Add(uniqueID, discoveredTestCase.VSTestCase);
                            uniqueTestCases.Add(discoveredTestCase.TestCase);
                        }
                    }

                    var recorder = new TrxTestResultRecorder();
                    using (var resultsSink = new RunnerExecutionSink(testCasesMap, recorder, this.blameWriter, this.resultsWriter))
                    {
                        controller.RunTests(uniqueTestCases, resultsSink, executionOptions);
                        resultsSink.Finished.WaitOne();
                        return resultsSink.ExecutionSummary;
                    }

                    //var assembliesElement = new XElement("assemblies");
                    //var assemblyElement = new XElement("assembly");
                    //using (var executionSink = new DelegatingXmlCreationSink(new BlameCrashExecutionSink(blameWriter), assemblyElement))
                    //{
                    //    controller.RunTests(testCases, executionSink, executionOptions);
                    //    executionSink.Finished.WaitOne();

                    //    //assembliesElement.Add(new XAttribute("timestamp", DateTime.Now.ToString(CultureInfo.InvariantCulture)));
                    //    assembliesElement.Add(assemblyElement);
                    //    assembliesElement.Save(resultsWriter);

                    //    return executionSink.ExecutionSummary;
                    //}
                }
            //}
            //catch (Exception ex)
            //{
            //    logger.LogError("{0}: Catastrophic failure: {1}", assemblyDisplayName, ex);
            //}
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0059:Unnecessary assignment of a value", Justification = "Will add logging someday")]
        public static VsTestCase CreateVsTestCase(string source,
                                               TestCaseDescriptor descriptor)
        {
            try
            {
                var fqTestMethodName = $"{descriptor.ClassName}.{descriptor.MethodName}";
                var result = new VsTestCase(fqTestMethodName, uri, source) { DisplayName = Escape(descriptor.DisplayName) };

                result.Id = GuidFromString(uri + descriptor.UniqueID);
                result.CodeFilePath = descriptor.SourceFileName;
                result.LineNumber = descriptor.SourceLineNumber.GetValueOrDefault();

                //if (addTraitThunk != null)
                //{
                //    var traits = descriptor.Traits;

                //    foreach (var key in traits.Keys)
                //        foreach (var value in traits[key])
                //            addTraitThunk(result, key, value);
                //}

                return result;
            }
            catch (Exception ex)
            {
                //logger.LogErrorWithSource(source, "Error creating Visual Studio test case for {0}: {1}", descriptor.DisplayName, ex);
                return null;
            }
        }
        static string Escape(string value)
        {
            if (value == null)
                return string.Empty;

            return value.Replace("\r", "\\r").Replace("\n", "\\n").Replace("\t", "\\t");
        }

        readonly static HashAlgorithm Hasher = SHA1.Create();

        static Guid GuidFromString(string data)
        {
            var hash = Hasher.ComputeHash(Encoding.Unicode.GetBytes(data));
            var b = new byte[16];
            Array.Copy((Array)hash, (Array)b, 16);
            return new Guid(b);
        }

        static IList<DiscoveredTestCase> GetVsTestCases(string source, ITestFrameworkDiscoverer discoverer, List<ITestCase> testCases /*, VsExecutionDiscoverySink visitor, LoggerHelper logger, TestPlatformContext testPlatformContext*/)
        {
            var descriptorProvider = (discoverer as ITestCaseDescriptorProvider) ?? new DefaultTestCaseDescriptorProvider(discoverer);
            //var testCases = visitor.TestCases;
            var descriptors = descriptorProvider.GetTestCaseDescriptors(testCases, false);
            var results = new List<DiscoveredTestCase>(descriptors.Count);

            for (var idx = 0; idx < descriptors.Count; ++idx)
            {
                var testCase = new DiscoveredTestCase(source, descriptors[idx], testCases[idx]/*, logger, testPlatformContext*/);
                if (testCase.VSTestCase != null)
                    results.Add(testCase);
            }

            return results;
        }

        class DiscoveredTestCase
        {
            public string Name { get; }

            public string ClassName { get; }

            public string MethodName { get; }

            public IEnumerable<string> TraitNames { get; }

            public VsTestCase VSTestCase { get; }

            public ITestCase TestCase { get; }

            public string UniqueID { get; }

            public DiscoveredTestCase(string source, TestCaseDescriptor descriptor, ITestCase testCase /*, LoggerHelper logger, TestPlatformContext testPlatformContext*/)
            {
                Name = $"{descriptor.ClassName}.{descriptor.MethodName} ({descriptor.UniqueID})";
                ClassName = descriptor.ClassName;
                MethodName = descriptor.MethodName;
                TestCase = testCase;
                UniqueID = descriptor.UniqueID;
                VSTestCase = Xunit2TestAssemblyRunner.CreateVsTestCase(source, descriptor/*, logger, testPlatformContext*/);
                TraitNames = descriptor.Traits.Keys;
            }
        }
    }
}