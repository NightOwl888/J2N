using System;

namespace J2N.CodeGeneration
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    internal sealed class CodeGenerationGenerateForwarderAttribute : Attribute
    {
    }
}
