using System;
using System.IO;
using System.Reflection;

namespace J2N
{
    public static class TypeExtensions
    {
        /// <summary>
        /// Locates resources in the same directory as this type
        /// </summary>
        public static Stream getResourceAsStream(this Type t, string name)
        {
#if FEATURE_TYPEEXTENSIONS_GETTYPEINFO
            return t.GetTypeInfo().Assembly.FindAndGetManifestResourceStream(t, name);
#else
            return t.Assembly.FindAndGetManifestResourceStream(t, name);
#endif
        }
    }
}
