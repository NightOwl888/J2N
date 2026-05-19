using System;

namespace J2N.CodeGeneration
{
    /// <summary>
    /// Defines that a method return type should be generated as a fluent API return style by code generators.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    internal sealed class CodeGenerationReturnsSelfAttribute : Attribute
    {
    }
}
