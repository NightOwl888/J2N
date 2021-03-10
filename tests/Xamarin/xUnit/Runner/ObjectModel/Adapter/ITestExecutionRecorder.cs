// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter
{
    public interface ITestExecutionRecorder : IMessageLogger
    {
        void RecordAttachments(IList<AttachmentSet> attachmentSets);
        void RecordEnd(TestCase testCase, TestOutcome outcome);
        void RecordResult(TestResult testResult);
        void RecordStart(TestCase testCase);
    }
}