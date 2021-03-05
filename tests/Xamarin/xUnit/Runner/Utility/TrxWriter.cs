using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using VsTestOutcome = Microsoft.VisualStudio.TestPlatform.ObjectModel.TestOutcome;
using VsTestOutcomeHelper = Microsoft.VisualStudio.TestPlatform.ObjectModel.TestOutcomeHelper;
using VsTestResult = Microsoft.VisualStudio.TestPlatform.ObjectModel.TestResult;

namespace Xunit.Runner
{
    /// <summary>
    /// A writer for outputting VSTest TRX reporting.
    /// </summary>
    public sealed class TrxWriter : IDisposable
    {
        private const string XmlNamespace = "http://microsoft.com/schemas/VisualStudio/TeamTest/2010";

        private const string TestListNotInAListId = "8c84fa94-04c1-424b-9868-57a2d4851a1d";
        private const string TestListNotInAListName = "Results Not in a List";
        private const string TestListAllLoadedResultsId = "19431567-8539-422a-85d7-44ee4e166bda";
        private const string TestListAllLoadedResultsName = "All Loaded Results";

        private const string TestType = "13cdc9d9-ddb5-4fa4-a97d-d965ccfc6d4b"; // TODO: Not sure what this means, but it appears to be constant in our case

        private readonly XmlWriter xml;
        private bool inDocument, inResults, inTestDefinitions, inTestEntries;

        public TrxWriter(TextWriter output)
        {
            xml = XmlWriter.Create(output, new XmlWriterSettings { Encoding = Encoding.UTF8 });
        }

        public void WriteStartDocument(Guid testRunId, string testRunName, string testRunUser) // Only allowed once
        {
            xml.WriteStartDocument(false);
            xml.WriteStartElement("TestRun", XmlNamespace);
            xml.WriteAttributeString("id", testRunId.ToString());
            xml.WriteAttributeString("name", testRunName);
            xml.WriteAttributeString("runUser", testRunUser);
            inDocument = true;
        }

        public void WriteStartTestResults()
        {
            if (!inDocument)
                throw new InvalidOperationException("WriteStartDocument() not called");
            if (inResults)
                throw new InvalidOperationException("Already in a Results element");

            xml.WriteStartElement("Results", XmlNamespace);
            inResults = true;
        }

        public void WriteTestResult(Guid executionId, VsTestResult testResult)
        {
            if (!inResults)
                throw new InvalidOperationException("WriteStartTestResults() not called");

            xml.WriteStartElement("UnitTestResult", XmlNamespace);
            xml.WriteAttributeString("executionId", executionId.ToString());
            xml.WriteAttributeString("testId", testResult.TestCase.Id.ToString());
            xml.WriteAttributeString("testName", testResult.DisplayName);
            xml.WriteAttributeString("computerName", testResult.ComputerName);
            xml.WriteAttributeString("duration", testResult.Duration.ToString("c", CultureInfo.InvariantCulture));
            xml.WriteAttributeString("startTime", testResult.StartTime.ToString("O", CultureInfo.InvariantCulture));
            xml.WriteAttributeString("endTime", testResult.EndTime.ToString("O", CultureInfo.InvariantCulture));
            xml.WriteAttributeString("testType", TestType);
            xml.WriteAttributeString("outcome", VsTestOutcomeHelper.GetOutcomeString(testResult.Outcome));
            xml.WriteAttributeString("testListId", TestListNotInAListId);
            xml.WriteAttributeString("relativeResultsDirectory", executionId.ToString());

            bool hasErrorMessage = !string.IsNullOrEmpty(testResult.ErrorMessage);
            bool hasStackTrace = !string.IsNullOrEmpty(testResult.ErrorStackTrace);
            bool hasStdOut = false;

            if (hasErrorMessage || hasStackTrace || hasStdOut)
            {
                xml.WriteStartElement("Output", XmlNamespace);

                if (hasErrorMessage || hasStackTrace)
                {
                    xml.WriteStartElement("ErrorInfo", XmlNamespace);

                    if (hasErrorMessage)
                    {
                        xml.WriteElementString("Message", XmlNamespace, testResult.ErrorMessage);
                    }
                    if (hasStackTrace)
                    {
                        xml.WriteElementString("StackTrace", XmlNamespace, testResult.ErrorStackTrace);
                    }

                    xml.WriteEndElement(); // ErrorInfo
                }
                if (hasStdOut)
                {
                    // TODO: Figure out how to include StdOut messages
                    //xml.WriteElementString("StdOut", XmlNamespace, ((Microsoft.VisualStudio.TestPlatform.ObjectModel.AttachmentSet)testResult.Attachments[0]).;
                }

                xml.WriteEndElement(); // Output
            }

            xml.WriteEndElement(); // UnitTestResult
        }

        public void WriteEndTestResults()
        {
            if (!inResults)
                throw new InvalidOperationException("WriteStartTestResults() not called");

            xml.WriteEndElement(); // Results
            inResults = false;
        }

        public void WriteStartTestDefinitions()
        {
            if (!inDocument)
                throw new InvalidOperationException("WriteStartDocument() not called");
            if (inTestDefinitions)
                throw new InvalidOperationException("Already in a TestDefinitions element");

            xml.WriteStartElement("TestDefinitions", XmlNamespace);
            inTestDefinitions = true;
        }

        public void WriteTestDefinition(Guid executionId, VsTestResult testResult)
        {
            if (!inTestDefinitions)
                throw new InvalidOperationException("WriteStartTestDefinitions() not called");

            // TODO: See if we can get the class name passed into TestResult
            string className = testResult.TestCase.FullyQualifiedName;
            int lastDot;
            if ((lastDot = testResult.TestCase.FullyQualifiedName.LastIndexOf('.')) != -1)
            {
                className = testResult.TestCase.FullyQualifiedName.Substring(0, lastDot);
            }

            xml.WriteStartElement("UnitTest", XmlNamespace);
            xml.WriteAttributeString("name", testResult.DisplayName);
            xml.WriteAttributeString("storage", testResult.TestCase.CodeFilePath);
            xml.WriteAttributeString("id", testResult.TestCase.Id.ToString());

            xml.WriteStartElement("Execution", XmlNamespace);
            xml.WriteAttributeString("id", executionId.ToString());
            xml.WriteEndElement(); // Execution

            xml.WriteStartElement("TestMethod", XmlNamespace);
            xml.WriteAttributeString("codeBase", testResult.TestCase.CodeFilePath);
            xml.WriteAttributeString("adapterTypeName", testResult.TestCase.ExecutorUri.ToString());
            xml.WriteAttributeString("className", className);
            xml.WriteAttributeString("name", testResult.TestCase.DisplayName);
            xml.WriteEndElement(); // TestMethod

            if (testResult.TestCase.Traits.Any())
            {
                xml.WriteStartElement("Properties", XmlNamespace);
                foreach (var trait in testResult.TestCase.Traits)
                {
                    xml.WriteStartElement("Property", XmlNamespace);

                    xml.WriteElementString("Key", XmlNamespace, trait.Name);
                    xml.WriteElementString("Value", XmlNamespace, trait.Value);

                    xml.WriteEndElement(); // Property
                }
                xml.WriteEndElement(); // Properties
            }

            // TODO: TestCategory

            xml.WriteEndElement(); // UnitTest
        }

        public void WriteEndTestDefinitions()
        {
            if (!inTestDefinitions)
                throw new InvalidOperationException("WriteStartTestDefinitions() not called");

            xml.WriteEndElement();
            inTestDefinitions = false;
        }

        public void WriteStartTestEntries()
        {
            if (!inDocument)
                throw new InvalidOperationException("WriteStartDocument() not called");
            if (inTestEntries)
                throw new InvalidOperationException("Already in a TestEntries element");

            xml.WriteStartElement("TestEntries", XmlNamespace);
            inTestEntries = true;
        }

        public void WriteTestEntry(Guid executionId, VsTestResult testResult)
        {
            if (!inTestEntries)
                throw new InvalidOperationException("WriteStartTestEntries() not called");

            xml.WriteStartElement("TestEntry", XmlNamespace);
            xml.WriteAttributeString("testId", testResult.TestCase.Id.ToString());
            xml.WriteAttributeString("executionId", executionId.ToString());
            xml.WriteAttributeString("testListId", TestListNotInAListId);
            xml.WriteEndElement(); // TestEntry
        }

        public void WriteEndTestEntries()
        {
            if (!inTestEntries)
                throw new InvalidOperationException("WriteStartTestEntries() not called");

            xml.WriteEndElement();
            inTestEntries = false;

            // Write extra TestLists element, which typically goes right after the Entries element
            xml.WriteStartElement("TestLists", XmlNamespace);

            xml.WriteStartElement("TestList", XmlNamespace);
            xml.WriteAttributeString("name", TestListNotInAListName);
            xml.WriteAttributeString("id", TestListNotInAListId);
            xml.WriteEndElement(); // TestList

            xml.WriteStartElement("TestList", XmlNamespace);
            xml.WriteAttributeString("name", TestListAllLoadedResultsName);
            xml.WriteAttributeString("id", TestListAllLoadedResultsId);
            xml.WriteEndElement(); // TestList

            xml.WriteEndElement(); // TestLists
        }

        public void WriteTimes(DateTime creation, DateTime queuing, DateTime start, DateTime finish) // Only allowed once
        {
            if (!inDocument)
                throw new InvalidOperationException("WriteStartDocument() not called");

            xml.WriteStartElement("Times", XmlNamespace);
            xml.WriteAttributeString("creation", creation.ToString("O", CultureInfo.InvariantCulture));
            xml.WriteAttributeString("queuing", queuing.ToString("O", CultureInfo.InvariantCulture));
            xml.WriteAttributeString("start", start.ToString("O", CultureInfo.InvariantCulture));
            xml.WriteAttributeString("finish", finish.ToString("O", CultureInfo.InvariantCulture));
            xml.WriteEndElement(); // Times
        }

        public void WriteResultSummary(VsTestOutcome outcome, Xunit.ExecutionSummary executionSummary)
        {
            if (!inDocument)
                throw new InvalidOperationException("WriteStartDocument() not called");

            xml.WriteStartElement("ResultSummary", XmlNamespace);
            xml.WriteAttributeString("outcome", VsTestOutcomeHelper.GetOutcomeString(outcome));

            xml.WriteStartElement("Counters", XmlNamespace);
            xml.WriteAttributeString("total", executionSummary.Total.ToString(CultureInfo.InvariantCulture));
            xml.WriteAttributeString("executed", (executionSummary.Total - executionSummary.Skipped).ToString(CultureInfo.InvariantCulture));
            xml.WriteAttributeString("passed", (executionSummary.Total - (executionSummary.Failed + executionSummary.Skipped)).ToString(CultureInfo.InvariantCulture));
            xml.WriteAttributeString("inconclusive", executionSummary.Skipped.ToString(CultureInfo.InvariantCulture));
            xml.WriteAttributeString("failed", executionSummary.Failed.ToString(CultureInfo.InvariantCulture));
            xml.WriteAttributeString("error", executionSummary.Errors.ToString(CultureInfo.InvariantCulture));
            xml.WriteEndElement(); // Counters

            xml.WriteEndElement(); // ResultSummary
        }

        public void WriteEndDocument()
        {
            if (!inDocument)
                throw new InvalidOperationException("WriteStartDocument() not called");

            xml.WriteEndDocument(); // TestRun
            inDocument = false;
        }

        public void Dispose()
        {
            xml.Dispose();
        }
    }
}