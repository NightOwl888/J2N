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
            return t.GetTypeInfo().Assembly.FindAndGetManifestResourceStream(t, name);
        }
    }
}
