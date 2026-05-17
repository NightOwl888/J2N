#if !FEATURE_ISEXTERNALINIT
namespace System.Runtime.CompilerServices
{
    /// <summary>
    /// This is a compatibility patch for the compiler to support init properties on
    /// older target frameworks. It serves no other purpose and is internal to avoid
    /// conflicting with other libraries. Note that the System.Runtime.CompilerServices
    /// namespace declaration here is deliverate because that is where the compiler expects
    /// this type to exist.
    /// </summary>
    internal static class IsExternalInit
    {
    }
}
#endif
