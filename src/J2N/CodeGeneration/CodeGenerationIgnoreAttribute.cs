using System;

namespace J2N.CodeGeneration
{
    /// <summary>
    /// Defines that a member will be ignored by code generators.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Constructor, AllowMultiple = false, Inherited = false)]
    internal sealed class CodeGenerationIgnoreAttribute : Attribute
    {
    }
}
