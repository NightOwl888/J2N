// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.VisualStudio.TestPlatform.ObjectModel
{
    using System;

    /// <summary>
    /// Helper methods for working with the TestOutcome enum.
    /// </summary>
    public static class TestOutcomeHelper
    {
        /// <summary>
        /// Converts the outcome into its localized string representation.
        /// </summary>
        /// <param name="outcome">The outcome to get the string for.</param>
        /// <returns>The localized outcome string.</returns>
        public static string GetOutcomeString(TestOutcome outcome)
        {
            string result;

            switch (outcome)
            {
                case TestOutcome.None:
                    result = "None";
                    break;
                case TestOutcome.Passed:
                    result = "Passed";
                    break;
                case TestOutcome.Failed:
                    result = "Failed";
                    break;
                case TestOutcome.Skipped:
                    result = "Skipped";
                    break;
                case TestOutcome.NotFound:
                    result = "NotFound";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(outcome));
            }

            return result;
        }
    }
}
