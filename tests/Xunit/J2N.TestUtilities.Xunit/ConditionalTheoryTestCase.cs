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
    internal sealed class ConditionalTheoryTestCase : XunitTestCase
    {
        private Type? _calleeType;
        private string[] _conditionMemberNames = [];

        [Obsolete("Called by the de-serializer", error: true)]
        public ConditionalTheoryTestCase() { }

        public ConditionalTheoryTestCase(
            IMessageSink diagnosticMessageSink,
            TestMethodDisplay defaultMethodDisplay,
            TestMethodDisplayOptions defaultMethodDisplayOptions,
            ITestMethod testMethod,
            object[]? testMethodArguments,
            Type? calleeType,
            string[] conditionMemberNames)
            : base(diagnosticMessageSink, defaultMethodDisplay, defaultMethodDisplayOptions, testMethod, testMethodArguments)
        {
            _calleeType = calleeType;
            _conditionMemberNames = conditionMemberNames ?? [];
        }

        public override void Serialize(IXunitSerializationInfo data)
        {
            base.Serialize(data);
            data.AddValue(nameof(_calleeType), _calleeType, typeof(Type));
            data.AddValue(nameof(_conditionMemberNames), _conditionMemberNames, typeof(string[]));
        }

        public override void Deserialize(IXunitSerializationInfo data)
        {
            base.Deserialize(data);
            _calleeType = data.GetValue<Type?>(nameof(_calleeType));
            _conditionMemberNames = data.GetValue<string[]>(nameof(_conditionMemberNames)) ?? [];
        }

        public override async Task<RunSummary> RunAsync(
            IMessageSink diagnosticMessageSink,
            IMessageBus messageBus,
            object[] constructorArguments,
            ExceptionAggregator aggregator,
            CancellationTokenSource cancellationTokenSource)
        {
            // Build the conditionArguments the same way the discoverer expects:
            object?[] conditionArguments = new object?[] { _calleeType, _conditionMemberNames };

            // Evaluate skip conditions at runtime using your helper that accepts ITestMethod + object[]
            string? skipReason = ConditionalTestDiscoverer.EvaluateSkipConditions(TestMethod, conditionArguments);

            if (skipReason != null)
            {
                var test = new XunitTest(this, DisplayName);

                // Emit a full lifecycle so VS and other adapters count the skip correctly.
                if (!messageBus.QueueMessage(new TestStarting(test)))
                    cancellationTokenSource.Cancel();

                if (!messageBus.QueueMessage(new TestSkipped(test, skipReason)))
                    cancellationTokenSource.Cancel();

                if (!messageBus.QueueMessage(new TestFinished(test, 0, null)))
                    cancellationTokenSource.Cancel();

                return new RunSummary { Total = 1, Skipped = 1 };
            }

            // Condition allowed the test to run — defer to normal execution of the row.
            return await base.RunAsync(diagnosticMessageSink, messageBus, constructorArguments, aggregator, cancellationTokenSource);
        }
    }
}
