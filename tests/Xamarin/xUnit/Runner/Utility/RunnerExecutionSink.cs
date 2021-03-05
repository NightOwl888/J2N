using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Xunit.Abstractions;
using VsTestCase = Microsoft.VisualStudio.TestPlatform.ObjectModel.TestCase;
using VsTestOutcome = Microsoft.VisualStudio.TestPlatform.ObjectModel.TestOutcome;
using VsTestResult = Microsoft.VisualStudio.TestPlatform.ObjectModel.TestResult;
using VsTestResultMessage = Microsoft.VisualStudio.TestPlatform.ObjectModel.TestResultMessage;

namespace Xunit.Runner
{
    public class RunnerExecutionSink : TestMessageSink, IExecutionSink, IDisposable
    {
        private readonly TextWriter blameWriter;
        private readonly TextWriter resultWriter;
        readonly ITestResultRecorder recorder;
        readonly IDictionary<string, VsTestCase> testCasesMap;

        public RunnerExecutionSink(IDictionary<string, VsTestCase> testCasesMap, ITestResultRecorder recorder, TextWriter blameWriter, TextWriter resultWriter)
        {
            this.blameWriter = blameWriter ?? throw new ArgumentNullException(nameof(blameWriter));
            this.resultWriter = resultWriter ?? throw new ArgumentNullException(nameof(resultWriter));

            this.recorder = recorder ?? throw new ArgumentNullException(nameof(recorder));
            //this.logger = logger;
            this.testCasesMap = testCasesMap ?? throw new ArgumentNullException(nameof(testCasesMap));

            ExecutionSummary = new ExecutionSummary();

            Execution.TestAssemblyFinishedEvent += HandleTestAssemblyFinished;
            Execution.TestCaseFinishedEvent += HandleTestCaseFinished;
            Execution.TestCaseStartingEvent += HandleTestCaseStarting;

            Execution.TestFailedEvent += HandleTestFailed;
            Execution.TestPassedEvent += HandleTestPassed;
            Execution.TestSkippedEvent += HandleTestSkipped;
        }

        public ExecutionSummary ExecutionSummary { get; private set; }

        public ManualResetEvent Finished { get; } = new ManualResetEvent(initialState: false);

        public override void Dispose()
        {
            ((IDisposable)Finished).Dispose();
            base.Dispose();
        }

        TestCase FindTestCase(ITestCase testCase)
        {
            if (testCasesMap.TryGetValue(testCase.UniqueID, out var result))
                return result;

            //logger.LogError(testCase, "Result reported for unknown test case: {0}", testCase.DisplayName);
            return null;
        }

        private void TryAndReport(string actionDescription, string displayName, Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                //logger.LogError(testCase, "Error occured while {0} for test case {1}: {2}", actionDescription, displayName, ex);
            }
        }

        void HandleTestCaseStarting(MessageHandlerArgs<ITestCaseStarting> args)
        {
            var testCaseStarting = args.Message;
            blameWriter.Write(testCaseStarting.TestCase.DisplayName);
            var vsTestCase = FindTestCase(testCaseStarting.TestCase);
            if (vsTestCase != null)
                TryAndReport("RecordStart", testCaseStarting.TestCase.DisplayName, () => recorder.RecordStart(vsTestCase));
            //else
            //    logger.LogWarning(testCaseStarting.TestCase, "(Starting) Could not find VS test case for {0} (ID = {1})", testCaseStarting.TestCase.DisplayName, testCaseStarting.TestCase.UniqueID);

            //HandleCancellation(args);
        }

        void HandleTestCaseFinished(MessageHandlerArgs<ITestCaseFinished> args)
        {
            var testCaseFinished = args.Message;
            var vsTestCase = FindTestCase(testCaseFinished.TestCase);
            if (vsTestCase != null)
                TryAndReport("RecordEnd", testCaseFinished.TestCase.DisplayName, () => recorder.RecordEnd(vsTestCase, GetAggregatedTestOutcome(testCaseFinished)));
            //else
            //    logger.LogWarning(testCaseFinished.TestCase, "(Finished) Could not find VS test case for {0} (ID = {1})", testCaseFinished.TestCase.DisplayName, testCaseFinished.TestCase.UniqueID);

            //HandleCancellation(args);
        }

        void HandleTestAssemblyFinished(MessageHandlerArgs<ITestAssemblyFinished> args)
        {
            var assemblyFinished = args.Message;

            ExecutionSummary.Failed = assemblyFinished.TestsFailed;
            ExecutionSummary.Skipped = assemblyFinished.TestsSkipped;
            ExecutionSummary.Time = assemblyFinished.ExecutionTime;
            ExecutionSummary.Total = assemblyFinished.TestsRun;

            var outcome = GetAggregatedTestOutcome(assemblyFinished);

            TryAndReport("RecordEnd", "RecordEnd", () => recorder.WriteTestResult(resultWriter, outcome, ExecutionSummary));

            Finished.Set();

            //HandleCancellation(args);
        }


        void HandleTestFailed(MessageHandlerArgs<ITestFailed> args)
        {
            blameWriter.Write(" [FAILED]");

            var testFailed = args.Message;
            var result = MakeVsTestResult(TestOutcome.Failed, testFailed);
            if (result != null)
            {
                result.ErrorMessage = ExceptionUtility.CombineMessages(testFailed);
                result.ErrorStackTrace = ExceptionUtility.CombineStackTraces(testFailed);

                TryAndReport("RecordResult (Fail)", testFailed.TestCase.DisplayName, () => recorder.RecordResult(result));
            }
            //else
            //    logger.LogWarning(testFailed.TestCase, "(Fail) Could not find VS test case for {0} (ID = {1})", testFailed.TestCase.DisplayName, testFailed.TestCase.UniqueID);

            //HandleCancellation(args);
        }

        void HandleTestPassed(MessageHandlerArgs<ITestPassed> args)
        {
            blameWriter.Write(" [PASSED]");

            var testPassed = args.Message;
            var result = MakeVsTestResult(TestOutcome.Passed, testPassed);
            if (result != null)
                TryAndReport("RecordResult (Pass)", testPassed.TestCase.DisplayName, () => recorder.RecordResult(result));
            //else
            //    logger.LogWarning(testPassed.TestCase, "(Pass) Could not find VS test case for {0} (ID = {1})", testPassed.TestCase.DisplayName, testPassed.TestCase.UniqueID);

            //HandleCancellation(args);
        }

        void HandleTestSkipped(MessageHandlerArgs<ITestSkipped> args)
        {
            blameWriter.Write(" [SKIPPED]");

            var testSkipped = args.Message;
            var result = MakeVsTestResult(TestOutcome.Skipped, testSkipped);
            if (result != null)
                TryAndReport("RecordResult (Skip)", testSkipped.TestCase.DisplayName, () => recorder.RecordResult(result));
            //else
            //    logger.LogWarning(testSkipped.TestCase, "(Skip) Could not find VS test case for {0} (ID = {1})", testSkipped.TestCase.DisplayName, testSkipped.TestCase.UniqueID);

            //HandleCancellation(args);
        }


        TestResult MakeVsTestResult(TestOutcome outcome, ITestResultMessage testResult)
            => MakeVsTestResult(outcome, testResult.TestCase, testResult.Test.DisplayName, (double)testResult.ExecutionTime, testResult.Output);

        TestResult MakeVsTestResult(TestOutcome outcome, ITestSkipped skippedResult)
            => MakeVsTestResult(outcome, skippedResult.TestCase, skippedResult.Test.DisplayName, (double)skippedResult.ExecutionTime, skippedResult.Reason);

        TestResult MakeVsTestResult(TestOutcome outcome, ITestCase testCase, string displayName, double executionTime = 0.0, string output = null)
        {
            var vsTestCase = FindTestCase(testCase);
            if (vsTestCase == null)
                return null;

            var result = new VsTestResult(vsTestCase)
            {
//#if !WINDOWS_UAP
//                ComputerName = Environment.MachineName,
//#endif
                DisplayName = displayName,
                Duration = TimeSpan.FromSeconds(executionTime),
                Outcome = outcome,
            };

            // Work around VS considering a test "not run" when the duration is 0
            if (result.Duration.TotalMilliseconds == 0)
                result.Duration = TimeSpan.FromMilliseconds(1);

            if (!string.IsNullOrEmpty(output))
                result.Messages.Add(new VsTestResultMessage(VsTestResultMessage.StandardOutCategory, output));

            return result;
        }

        VsTestOutcome GetAggregatedTestOutcome(ITestAssemblyFinished testAssemblyFinished)
        {
            if (testAssemblyFinished.TestsRun == 0)
                return VsTestOutcome.NotFound;
            else if (testAssemblyFinished.TestsFailed > 0)
                return VsTestOutcome.Failed;
            else if (testAssemblyFinished.TestsSkipped > 0)
                return VsTestOutcome.Skipped;
            else
                return VsTestOutcome.Passed;
        }

        VsTestOutcome GetAggregatedTestOutcome(ITestCaseFinished testCaseFinished)
        {
            if (testCaseFinished.TestsRun == 0)
                return VsTestOutcome.NotFound;
            else if (testCaseFinished.TestsFailed > 0)
                return VsTestOutcome.Failed;
            else if (testCaseFinished.TestsSkipped > 0)
                return VsTestOutcome.Skipped;
            else
                return VsTestOutcome.Passed;
        }
    }
}