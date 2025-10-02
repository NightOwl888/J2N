using System;
using System.Linq;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace J2N.TestUtilities.Xunit
{
    /// <summary>
    /// This works with the stock build of xunit 2.x. It does not align directly
    /// with the upstream Arcade code at https://github.com/dotnet/arcade/tree/release/9.0/src/Microsoft.DotNet.XUnitExtensions,
    /// but provides similar functionality without having to somehow dredge up Microsoft's custom fork of xunit.
    /// </summary>
    public class ConditionalFactDiscoverer : FactDiscoverer
    {
        public ConditionalFactDiscoverer(IMessageSink diagnosticMessageSink)
            : base(diagnosticMessageSink)
        { }

        // Called during discovery; we construct a custom test case so it can evaluate at runtime
        protected override IXunitTestCase CreateTestCase(
            ITestFrameworkDiscoveryOptions discoveryOptions,
            ITestMethod testMethod,
            IAttributeInfo factAttribute)
        {
            // Keep the original constructor arguments so the test case can re-evaluate at runtime
            var ctorArgs = factAttribute.GetConstructorArguments().ToArray();

            return new ConditionalFactTestCase(
                DiagnosticMessageSink,
                discoveryOptions.MethodDisplayOrDefault(),
                discoveryOptions.MethodDisplayOptionsOrDefault(),
                testMethod,
                ctorArgs);
        }
    }
}
