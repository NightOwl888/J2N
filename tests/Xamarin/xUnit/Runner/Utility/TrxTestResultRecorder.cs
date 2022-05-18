using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using VsTestCase = Microsoft.VisualStudio.TestPlatform.ObjectModel.TestCase;
using VsTestOutcome = Microsoft.VisualStudio.TestPlatform.ObjectModel.TestOutcome;
using VsTestResult = Microsoft.VisualStudio.TestPlatform.ObjectModel.TestResult;

namespace Xunit.Runner
{
    /// <summary>
    /// A collector of test results that can be used to write a VSTest TRX report.
    /// </summary>
    public class TrxTestResultRecorder : ITestResultRecorder
    {
        private int passed, failed, skipped, inconclusive, total;
        private readonly DateTime creationTime;
        private readonly DateTime queuingTime;
        private DateTime startTime = DateTime.MinValue;
        private DateTime endTime = DateTime.MinValue;
        private readonly Guid executionId;
        private readonly string executionPath;
        private readonly IDictionary<Guid, VsTestResult> testResults = new Dictionary<Guid, VsTestResult>();
        private readonly HashSet<Guid> uniqueTestIDs = new HashSet<Guid>(70000);

        public TrxTestResultRecorder()
        {
            creationTime = DateTime.UtcNow;
            queuingTime = creationTime;
            executionId = Guid.NewGuid();
            var tempPath = Path.GetTempPath();
            executionPath = Path.Combine(tempPath, executionId.ToString());
            new DirectoryInfo(executionPath).Create();
        }

        public void RecordAttachments(IList<AttachmentSet> attachmentSets)
        {
            throw new NotImplementedException();
        }

        public void RecordEnd(VsTestCase testCase, VsTestOutcome outcome)
        {
            switch (outcome)
            {
                case VsTestOutcome.Passed:
                    Interlocked.Increment(ref passed);
                    break;
                case VsTestOutcome.Failed:
                    Interlocked.Increment(ref failed);
                    break;
                case VsTestOutcome.Skipped:
                    Interlocked.Increment(ref skipped);
                    break;
                case VsTestOutcome.NotFound:
                case VsTestOutcome.None:
                    Interlocked.Increment(ref inconclusive);
                    break;
            }
        }

        public void RecordResult(VsTestResult testResult)
        {
            var testExecutionId = Guid.NewGuid();
            var testCaseId = testResult.TestCase.Id;
            if (!uniqueTestIDs.Contains(testCaseId))
            {
                testResults.Add(testExecutionId, testResult);
                uniqueTestIDs.Add(testCaseId);
            }
        }

        public void RecordStart(VsTestCase testCase)
        {
            if (startTime == DateTime.MinValue) startTime = DateTime.UtcNow;
            Interlocked.Increment(ref total);
        }

        public void SendMessage(TestMessageLevel testMessageLevel, string message)
        {
            throw new NotImplementedException();
        }

        public void WriteTestResult(TextWriter output, VsTestOutcome outcome, Xunit.ExecutionSummary summary)
        {
            if (endTime == DateTime.MinValue) endTime = DateTime.UtcNow;

            using (var trx = new TrxWriter(output))
            {
                trx.WriteStartDocument(
                    testRunId: Guid.NewGuid(),
                    testRunName: startTime.ToString(CultureInfo.InvariantCulture),
                    testRunUser: "");

                trx.WriteTimes(creationTime, queuingTime, startTime, endTime);

                // Write results
                if (testResults.Count > 0)
                {
                    trx.WriteStartTestResults();
                    foreach (var pair in testResults)
                    {
                        trx.WriteTestResult(pair.Key, pair.Value);
                    }
                    trx.WriteEndTestResults();
                }

                // Write Test Definitions
                if (testResults.Count > 0)
                {
                    trx.WriteStartTestDefinitions();
                    foreach (var pair in testResults)
                    {
                        trx.WriteTestDefinition(pair.Key, pair.Value);
                    }
                    trx.WriteEndTestDefinitions();
                }

                // Write Test Entries
                if (testResults.Count > 0)
                {
                    trx.WriteStartTestEntries();
                    foreach (var pair in testResults)
                    {
                        trx.WriteTestEntry(pair.Key, pair.Value);
                    }
                    trx.WriteEndTestEntries();
                }

                trx.WriteResultSummary(outcome, summary);

                trx.WriteEndDocument();
            }
        }
    }
}