using System;

namespace J2N.CodeGeneration
{
    /// <summary>
    /// Defines that a method, property getter, or property setter should skip being synchronized.
    /// This allows the caller to supply their own sychronziation if and only if it is required for
    /// methods that return System.Memory types or other types that need to be fully synchronized for
    /// the complete operation.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    internal sealed class CodeGenerationSkipSynchronizationAttribute : Attribute
    {
    }
}
