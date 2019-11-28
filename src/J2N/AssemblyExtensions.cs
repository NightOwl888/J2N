using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Reflection;

namespace J2N
{
    /// <summary>
    /// Extensions to the <see cref="System.Reflection.Assembly"/> class.
    /// </summary>
    public static class AssemblyExtensions
    {
        private static ConcurrentDictionary<TypeAndResource, string> resourceCache = new ConcurrentDictionary<TypeAndResource, string>();

        /// <summary>
        /// Aggressively searches for a resource and, if found, returns an open <see cref="Stream"/>
        /// where it can be read.
        /// <para/>
        /// The search attempts to find the resource starting at the location of the current
        /// class and attempts every combination of starting namespace and or starting assembly name
        /// (split on <c>.</c>) concatenated with the <paramref name="name"/>. For example, if the
        /// type passed is in the namespace <c>Foo.Bar</c> in an assembly named <c>Faz.Baz</c>
        /// and the name passed is <c>res.txt</c>, the following locations are searched in this order:
        /// <code>
        /// 1. res.txt
        /// 2. Faz.Baz.Foo.Bar.res.txt
        /// 3. Foo.Bar.res.txt
        /// 4. Faz.Baz.res.txt
        /// </code>
        /// <para/>
        /// Usage Note: This method effectively treats embedded resources as being in the same
        /// "class path" as the type that is passed, making it similar to the Class.getResourceAsStream()
        /// method in Java.
        /// </summary>
        /// <param name="assembly">This assembly.</param>
        /// <param name="type">A type in the same namespace as the resource.</param>
        /// <param name="name">The resource name to locate.</param>
        /// <returns>An open <see cref="Stream"/> that can be used to read the resource, or <c>null</c> if the resource cannot be found.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="assembly"/> or <paramref name="type"/> is <c>null</c>.</exception>
        /// <seealso cref="TypeExtensions.FindAndGetManifestResourceStream(Type, string)"/>
        public static Stream FindAndGetManifestResourceStream(this Assembly assembly, Type type, string name)
        {
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            string resourceName = FindResource(assembly, type, name);
            if (string.IsNullOrEmpty(resourceName))
            {
                return null;
            }

            return assembly.GetManifestResourceStream(resourceName);
        }

        /// <summary>
        /// Aggressively searches to find a resource based on a <see cref="Type"/> and resource name.
        /// <para/>
        /// The search attempts to find the resource starting at the location of the current
        /// class and attempts every combination of starting namespace and or starting assembly name
        /// (split on <c>.</c>) concatenated with the <paramref name="name"/>. For example, if the
        /// type passed is in the namespace <c>Foo.Bar</c> in an assembly named <c>Faz.Baz</c>
        /// and the name passed is <c>res.txt</c>, the following locations are searched in this order:
        /// <code>
        /// 1. res.txt
        /// 2. Faz.Baz.Foo.Bar.res.txt
        /// 3. Foo.Bar.res.txt
        /// 4. Faz.Baz.res.txt
        /// </code>
        /// <para/>
        /// Usage Note: This method effectively treats embedded resources as being in the same
        /// "class path" as the type that is passed, making it similar to the Class.getResource()
        /// method in Java.
        /// </summary>
        /// <param name="assembly">This assembly.</param>
        /// <param name="type">A type in the same namespace as the resource.</param>
        /// <param name="name">The resource name to locate.</param>
        /// <returns>The resource, if found; if not found, returns <c>null</c>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="assembly"/> or <paramref name="type"/> is <c>null</c>.</exception>
        public static string FindResource(this Assembly assembly, Type type, string name)
        {
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            TypeAndResource key = new TypeAndResource(type, name);
            if (!resourceCache.TryGetValue(key, out string resourceName))
            {
                string[] resourceNames = assembly.GetManifestResourceNames();
                resourceName = resourceNames.Where(x => x.Equals(name, StringComparison.Ordinal)).FirstOrDefault();

                // If resourceName is not null, we have an exact match, don't search
                if (resourceName == null)
                {
                    string assemblyName = type.GetTypeInfo().Assembly.GetName().Name;
                    string namespaceName = type.GetTypeInfo().Namespace;

                    // Search by assembly + namespace
                    string resourceToFind = string.Concat(namespaceName, ".", name);
                    if (!TryFindResource(resourceNames, assemblyName, resourceToFind, name, out resourceName))
                    {
                        string found1 = resourceName;

                        // Search by namespace only
                        if (!TryFindResource(resourceNames, null, resourceToFind, name, out resourceName))
                        {
                            string found2 = resourceName;

                            // Search by assembly name only
                            resourceToFind = string.Concat(assemblyName, ".", name);
                            if (!TryFindResource(resourceNames, null, resourceToFind, name, out resourceName))
                            {
                                // Take the first match of multiple, if there are any
                                resourceName = found1 ?? found2 ?? resourceName;
                            }
                        }
                    }
                }

                resourceCache[key] = resourceName;
            }

            return resourceName;
        }

        private static bool TryFindResource(string[] resourceNames, string prefix, string resourceName, string exactResourceName, out string result)
        {
            if (!resourceNames.Contains(resourceName))
            {
                string nameToFind = null;
                while (resourceName.Length > 0 && resourceName.Contains('.') && (!(string.IsNullOrEmpty(prefix)) || resourceName.Equals(exactResourceName, StringComparison.Ordinal)))
                {
                    nameToFind = string.IsNullOrEmpty(prefix)
                        ? resourceName
                        : string.Concat(prefix, ".", resourceName);
                    string[] matches = resourceNames.Where(x => x.EndsWith(nameToFind, StringComparison.Ordinal)).ToArray();
                    if (matches.Length == 1)
                    {
                        result = matches[0]; // Exact match
                        return true;
                    }
                    else if (matches.Length > 1)
                    {
                        result = matches[0]; // First of many
                        return false;
                    }

                    resourceName = resourceName.Substring(resourceName.IndexOf('.') + 1);
                }
                result = null; // No match
                return false;
            }

            result = resourceName;
            return true;
        }

        private class TypeAndResource : IEquatable<TypeAndResource>
        {
            private readonly Type type;
            private readonly string name;

            public TypeAndResource(Type type, string name)
            {
                this.type = type;
                this.name = name;
            }

            public override bool Equals(object obj)
            {
                if (!(obj is TypeAndResource))
                    return false;

                var other = obj as TypeAndResource;
                return this.type.Equals(other.type)
                    && this.name.Equals(other.name, StringComparison.Ordinal);
            }

            public bool Equals(TypeAndResource other)
            {
                if (other == null)
                    return false;

                return this.type.Equals(other.type)
                    && this.name.Equals(other.name, StringComparison.Ordinal);
            }

            public override int GetHashCode()
            {
                return this.type.GetHashCode() ^ this.name.GetHashCode();
            }
        }
    }
}
