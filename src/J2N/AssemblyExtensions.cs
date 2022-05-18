#region Copyright 2019-2021 by Shad Storhaug, Licensed under the Apache License, Version 2.0
/*  Licensed to the Apache Software Foundation (ASF) under one or more
 *  contributor license agreements.  See the NOTICE file distributed with
 *  this work for additional information regarding copyright ownership.
 *  The ASF licenses this file to You under the Apache License, Version 2.0
 *  (the "License"); you may not use this file except in compliance with
 *  the License.  You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 *  Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *  limitations under the License.
 */
#endregion

using J2N.Collections.Concurrent;
using J2N.Collections.Generic;
using System;
using System.Diagnostics.CodeAnalysis;
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
        // Use LurchTable as an LRU cache. We keep a maximum of 256 entries in memory before truncating and using
        // access order ensures that the most recently accessed items are kept near the top of the cache.
        private static readonly LurchTable<TypeAndResource, string> resourceCache = new LurchTable<TypeAndResource, string>(LurchTableOrder.Access, 256);

        /// <summary>
        /// Uses the assembly name + '.' + suffix to determine whether any resources begin with the concatenation.
        /// If not, the assembly name will be truncated at the '.' beginning from the right side of the string
        /// until a base name is found.
        /// </summary>
        /// <param name="assembly">This <see cref="Assembly"/>.</param>
        /// <param name="suffix">A suffix to use on the assembly name to limit the possible resource names to match.
        /// This value can be <c>null</c> to match any resource name in the assembly.</param>
        /// <returns>A base name if found, otherwise <see cref="string.Empty"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="assembly"/> is <c>null</c>.</exception>
        public static string GetManifestResourceBaseName(this Assembly assembly, string suffix)
        {
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));

            var resourceNames = assembly.GetManifestResourceNames();
            string assemblyName = assembly.GetName().Name!;
            string baseName = string.IsNullOrEmpty(suffix) ? assemblyName : assemblyName + '.' + suffix;
            int dotIndex = -1;
            do
            {
                if (resourceNames.Any(resName => resName.StartsWith(baseName, StringComparison.Ordinal)))
                {
                    return baseName;
                }

                dotIndex = assemblyName.LastIndexOf('.');
                if (dotIndex > -1 && dotIndex < assemblyName.Length - 1)
                {
                    assemblyName = assemblyName.Substring(0, dotIndex);
                    baseName = string.IsNullOrEmpty(suffix) ? assemblyName : assemblyName + '.' + suffix;
                }
            } while (dotIndex > -1);

            // No match
            return string.Empty;
        }

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
        /// "class path" as the type that is passed, making it similar to the <c>Class.getResourceAsStream()</c>
        /// method in Java.
        /// </summary>
        /// <param name="assembly">This assembly.</param>
        /// <param name="name">The resource name to locate.</param>
        /// <returns>An open <see cref="Stream"/> that can be used to read the resource, or <c>null</c> if the resource cannot be found.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="assembly"/> or <paramref name="name"/> is <c>null</c>.</exception>
        public static Stream? FindAndGetManifestResourceStream(this Assembly assembly, string name)
        {
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            string? resourceName = FindResource(assembly, name);
            if (string.IsNullOrEmpty(resourceName))
            {
                return null;
            }

            return assembly.GetManifestResourceStream(resourceName);
        }

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
        /// "class path" as the type that is passed, making it similar to the <c>Class.getResourceAsStream()</c>
        /// method in Java.
        /// </summary>
        /// <param name="assembly">This assembly.</param>
        /// <param name="type">A type in the same namespace as the resource.</param>
        /// <param name="name">The resource name to locate.</param>
        /// <returns>An open <see cref="Stream"/> that can be used to read the resource, or <c>null</c> if the resource cannot be found.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="assembly"/>, <paramref name="type"/> or <paramref name="name"/> is <c>null</c>.</exception>
        /// <seealso cref="TypeExtensions.FindAndGetManifestResourceStream(Type, string)"/>
        public static Stream? FindAndGetManifestResourceStream(this Assembly assembly, Type type, string name)
        {
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            string? resourceName = FindResource(assembly, type, name);
            if (string.IsNullOrEmpty(resourceName))
            {
                return null;
            }

            return assembly.GetManifestResourceStream(resourceName);
        }

        /// <summary>
        /// Aggressively searches to find a resource based on a resource name.
        /// Attempts to find the resource file by prepending the assembly name
        /// with the resource name and then removing the segements of the
        /// name from left to right until a match is found.
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
        /// "class path" as the type that is passed, making it similar to the <c>Class.getResource()</c>
        /// method in Java.
        /// </summary>
        /// <param name="assembly">This assembly.</param>
        /// <param name="name">The resource name to locate.</param>
        /// <returns>The resource, if found; if not found, returns <c>null</c>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="assembly"/> or <paramref name="name"/> is <c>null</c>.</exception>
        public static string? FindResource(this Assembly assembly, string name)
        {
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            var key = new TypeAndResource(null, name);
            return resourceCache.GetOrAdd(key, (key) =>
            {
                string[] resourceNames = assembly.GetManifestResourceNames();
                string? resourceName = resourceNames.Where(x => x.Equals(name)).FirstOrDefault();

                // If resourceName is not null, we have an exact match, don't search
                if (resourceName == null)
                {
                    string assemblyName = assembly.GetName().Name!;
                    int lastDot = assemblyName.LastIndexOf('.');
                    do
                    {
                        // Search by assembly name only
                        string resourceToFind = string.Concat(assemblyName, ".", name);
                        TryFindResource(resourceNames, null, resourceToFind, name, out resourceName);

                        // Continue searching by removing sections after the . from the assembly name
                        // until we have a match.
                        lastDot = assemblyName.LastIndexOf('.');
                        assemblyName = lastDot >= 0 ? assemblyName.Substring(0, lastDot) : null!;

                    } while (assemblyName != null && resourceName == null);

                    if (resourceName == null)
                    {
                        // Try again without using the assembly name
                        TryFindResource(resourceNames, null, name, name, out resourceName);
                    }
                }

                return resourceName!; // Null return okay
            });
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
        /// <exception cref="ArgumentNullException">If <paramref name="assembly"/>, <paramref name="type"/> or <paramref name="name"/> is <c>null</c>.</exception>
        public static string? FindResource(this Assembly assembly, Type type, string name)
        {
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            TypeAndResource key =  new TypeAndResource(type, name);
            return resourceCache.GetOrAdd(key, (key) =>
            {
                string[] resourceNames = assembly.GetManifestResourceNames();
                string? resourceName = resourceNames.Where(x => x.Equals(name, StringComparison.Ordinal)).FirstOrDefault();

                // If resourceName is not null, we have an exact match, don't search
                if (resourceName == null)
                {
                    string assemblyName = type.Assembly.GetName().Name!;
                    string namespaceName = type.Namespace!;

                    // Search by assembly + namespace
                    string resourceToFind = string.Concat(namespaceName, ".", name);
                    if (!TryFindResource(resourceNames, assemblyName, resourceToFind, name, out resourceName))
                    {
                        string? found1 = resourceName;

                        // Search by namespace only
                        if (!TryFindResource(resourceNames, null, resourceToFind, name, out resourceName))
                        {
                            string? found2 = resourceName;

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
                return resourceName!; // Null return okay
            });
        }

        private static bool TryFindResource(string[] resourceNames, [AllowNull, MaybeNull] string prefix, string resourceName, string exactResourceName, [MaybeNullWhen(false)] out string result)
        {
            if (!resourceNames.Contains(resourceName))
            {
                string? nameToFind = null;
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

        private struct TypeAndResource : IEquatable<TypeAndResource>
        {
            private readonly Type? type;
            private readonly string name;

            public TypeAndResource(Type? type, string name)
            {
                this.type = type;
                this.name = name;
            }

            public override bool Equals(object? obj)
            {
                if (obj is TypeAndResource other)
                    return Equals(other);

                return false;
            }

            public bool Equals(TypeAndResource other)
            {
#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable CS8604 // Possible null reference argument. We allow null values.
                return EqualityComparer<Type>.Default.Equals(this.type, other.type) &&
                    EqualityComparer<string>.Default.Equals(this.name, other.name);
#pragma warning restore CS8604 // Possible null reference argument.
#pragma warning restore IDE0079 // Remove unnecessary suppression
            }

            public override int GetHashCode()
            {
#pragma warning disable CS8604 // Possible null reference argument. We allow null values.
                return EqualityComparer<Type>.Default.GetHashCode(this.type) ^ EqualityComparer<string>.Default.GetHashCode(this.name);
#pragma warning restore CS8604 // Possible null reference argument.
            }
        }
    }
}
