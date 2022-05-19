using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
using Xunit.Abstractions;

namespace Xunit.Runner
{
    /// <summary>
    /// Writes a log of each executed test, flushing the name to the <see cref="TextWriter"/> before
    /// each test run. The status is appended if the test completed. A name without a status indicates
    /// the runner crashed during the execution of that test.
    /// <para/>
    /// This execution sink requires that the tests be run sequentially.
    /// </summary>
    [SuppressMessage("Design", "CA1063:Implement IDisposable Correctly", Justification = "Following xUnit's conventions")]
    public class BlameCrashExecutionSink : TestMessageSink, IExecutionSink, IDisposable
    {
        private readonly TextWriter blameWriter;
        private bool completed;

        public BlameCrashExecutionSink(TextWriter blameWriter)
        {
            this.blameWriter = blameWriter ?? throw new ArgumentNullException(nameof(blameWriter));

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

        void HandleTestCaseStarting(MessageHandlerArgs<ITestCaseStarting> args)
        {
            completed = false;
            var testCaseStarting = args.Message;
            blameWriter.Write(testCaseStarting.TestCase.DisplayName);
            blameWriter.Flush(); // Flush the name to the output in case of a crash
        }

        void HandleTestCaseFinished(MessageHandlerArgs<ITestCaseFinished> args)
        {
            if (completed) return;
            blameWriter.Write(" [ERROR]");
            completed = true;
        }

        void HandleTestAssemblyFinished(MessageHandlerArgs<ITestAssemblyFinished> args)
        {
            var assemblyFinished = args.Message;

            ExecutionSummary.Failed = assemblyFinished.TestsFailed;
            ExecutionSummary.Skipped = assemblyFinished.TestsSkipped;
            ExecutionSummary.Time = assemblyFinished.ExecutionTime;
            ExecutionSummary.Total = assemblyFinished.TestsRun;

            Finished.Set();
        }

        void HandleTestFailed(MessageHandlerArgs<ITestFailed> args)
        {
            blameWriter.Write(" [FAILED]");
            completed = true;
        }

        void HandleTestPassed(MessageHandlerArgs<ITestPassed> args)
        {
            blameWriter.Write(" [PASSED]");
            completed = true;
        }

        void HandleTestSkipped(MessageHandlerArgs<ITestSkipped> args)
        {
            blameWriter.Write(" [SKIPPED]");
            completed = true;
        }
    }
}