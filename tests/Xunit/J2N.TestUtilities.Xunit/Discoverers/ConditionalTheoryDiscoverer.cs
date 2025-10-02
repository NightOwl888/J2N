using System;
using System.Collections.Generic;
using System.Linq;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace J2N.TestUtilities.Xunit
{
    /// <summary>
    /// This works with the stock build of xunit 2.x. It does not align directly
    /// with the upstream Arcade code at https://github.com/dotnet/arcade/tree/release/9.0/src/Microsoft.DotNet.XUnitExtensions.
    /// </summary>
    public class ConditionalTheoryDiscoverer : TheoryDiscoverer
    {
        public ConditionalTheoryDiscoverer(IMessageSink diagnosticMessageSink)
            : base(diagnosticMessageSink) { }

        public override IEnumerable<IXunitTestCase> Discover(
            ITestFrameworkDiscoveryOptions discoveryOptions,
            ITestMethod testMethod,
            IAttributeInfo theoryAttribute)
        {
            // Extract constructor args
            var ctorArgs = theoryAttribute.GetConstructorArguments().ToArray();
            Type? calleeType = null;
            string[] conditionMemberNames = [];

            if (ctorArgs.Length > 0)
            {
                if (ctorArgs[0] is Type t)
                {
                    calleeType = t;
                    if (ctorArgs.Length > 1 && ctorArgs[1] is string[] arr)
                        conditionMemberNames = arr;
                }
                else if (ctorArgs[0] is string[] arrOnly)
                {
                    conditionMemberNames = arrOnly;
                }
            }

            // Ask the base class to generate the normal theory test cases
            foreach (var rowCase in base.Discover(discoveryOptions, testMethod, theoryAttribute))
            {
                yield return new ConditionalTheoryTestCase(
                    DiagnosticMessageSink,
                    discoveryOptions.MethodDisplayOrDefault(),
                    discoveryOptions.MethodDisplayOptionsOrDefault(),
                    testMethod,
                    rowCase.TestMethodArguments,
                    calleeType,
                    conditionMemberNames);
            }
        }
    }
}
