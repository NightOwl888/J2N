using System;
using Xunit;
using Xunit.Sdk;

namespace J2N.TestUtilities.Xunit
{
    /// <summary>
    /// This works with the stock build of xunit 2.x. It does not align directly
    /// with the upstream Arcade code at https://github.com/dotnet/arcade/tree/release/9.0/src/Microsoft.DotNet.XUnitExtensions,
    /// but provides similar functionality without having to somehow dredge up Microsoft's custom fork of xunit.
    /// </summary>
    [XunitTestCaseDiscoverer("J2N.TestUtilities.Xunit.ConditionalFactDiscoverer", "J2N.TestUtilities.Xunit")]
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class ConditionalFactAttribute : FactAttribute
    {
        public Type? CalleeType { get; }
        public string[] ConditionMemberNames { get; }

        public ConditionalFactAttribute(Type calleeType, params string[] conditionMemberNames)
        {
            CalleeType = calleeType;
            ConditionMemberNames = conditionMemberNames ?? [];
        }

        public ConditionalFactAttribute(params string[] conditionMemberNames)
        {
            ConditionMemberNames = conditionMemberNames ?? [];
        }
    }
}