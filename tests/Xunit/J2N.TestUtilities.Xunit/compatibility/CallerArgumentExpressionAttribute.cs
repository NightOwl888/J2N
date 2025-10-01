#if NETFRAMEWORK || NETSTANDARD2_0
namespace System.Runtime.CompilerServices
{
    /// <summary>
    /// This is a polyfill of CallerExpressionArgument prior to .NET Core.
    /// It is supported as long as the language used is C# 10+.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
    internal sealed class CallerArgumentExpressionAttribute : Attribute
    {
        public CallerArgumentExpressionAttribute(string parameterName)
        {
            ParameterName = parameterName;
        }

        public string ParameterName { get; }
    }
}
#endif
