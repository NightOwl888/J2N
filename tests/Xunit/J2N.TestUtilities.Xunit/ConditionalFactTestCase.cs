using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;
#nullable enable

namespace J2N.TestUtilities.Xunit
{
    /// <summary>
    /// This works with the stock build of xunit 2.x. It does not align directly
    /// with the upstream Arcade code at https://github.com/dotnet/arcade/tree/release/9.0/src/Microsoft.DotNet.XUnitExtensions,
    /// but provides similar functionality without having to somehow dredge up Microsoft's custom fork of xunit.
    /// </summary>
    public class ConditionalFactTestCase : XunitTestCase
    {
        object[]? conditionArguments;

        // Parameterless ctor required for de-serialization
        [Obsolete("Called by the de-serializer", error: true)]
        public ConditionalFactTestCase() { }

        public ConditionalFactTestCase(
            IMessageSink diagnosticMessageSink,
            TestMethodDisplay defaultMethodDisplay,
            TestMethodDisplayOptions defaultMethodDisplayOptions,
            ITestMethod testMethod,
            object[]? conditionArguments = null)
            : base(diagnosticMessageSink, defaultMethodDisplay, defaultMethodDisplayOptions, testMethod)
        {
            this.conditionArguments = conditionArguments;
        }

        public override void Serialize(IXunitSerializationInfo data)
        {
            base.Serialize(data);
            // serialize the condition arguments so RunAsync can re-evaluate (they must be serializable)
            // we store as object[]; xUnit serialization supports basic types & arrays
            data.AddValue("ConditionArguments", conditionArguments);
        }

        public override void Deserialize(IXunitSerializationInfo data)
        {
            base.Deserialize(data);
            conditionArguments = data.GetValue<object[]>("ConditionArguments");
        }

        public override async Task<RunSummary> RunAsync(
            IMessageSink diagnosticMessageSink,
            IMessageBus messageBus,
            object[] constructorArguments,
            ExceptionAggregator aggregator,
            CancellationTokenSource cancellationTokenSource)
        {
            // Re-evaluate the condition *at runtime*
            // This calls your existing helper that inspects the test method and condition args:
            string? skipReason = ConditionalTestDiscoverer.EvaluateSkipConditions(TestMethod, conditionArguments);

            if (skipReason != null)
            {
                var test = new XunitTest(this, DisplayName);

                if (!messageBus.QueueMessage(new TestStarting(test)))
                    cancellationTokenSource.Cancel();

                if (!messageBus.QueueMessage(new TestSkipped(test, skipReason)))
                    cancellationTokenSource.Cancel();

                if (!messageBus.QueueMessage(new TestFinished(test, 0, null)))
                    cancellationTokenSource.Cancel();

                return new RunSummary { Total = 1, Skipped = 1 };
            }


            // Condition allowed the test to run — defer to normal execution
            return await base.RunAsync(diagnosticMessageSink, messageBus, constructorArguments, aggregator, cancellationTokenSource);
        }
    }
}