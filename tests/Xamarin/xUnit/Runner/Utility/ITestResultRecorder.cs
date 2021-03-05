using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using System.IO;
using VsTestOutcome = Microsoft.VisualStudio.TestPlatform.ObjectModel.TestOutcome;

namespace Xunit.Runner
{
    public interface ITestResultRecorder : ITestExecutionRecorder
    {
        void WriteTestResult(TextWriter output, VsTestOutcome outcome, Xunit.ExecutionSummary summary);
    }
}