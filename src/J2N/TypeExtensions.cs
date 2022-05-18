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

using System;
using System.IO;
using System.Linq;
using System.Reflection;


namespace J2N
{
    /// <summary>
    /// Extensions to the <see cref="System.Type"/> class.
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// Returns true if this type implements the specified generic interface.
        /// <para/>
        /// Examples:
        /// <code>
        /// bool result1 = this.GetType().ImplementsGenericInterface(typeof(IDictionary&lt;,&gt;);
        /// bool result2 = typeof(System.Collections.Generic.List&lt;int&gt;).ImplementsGenericInterface(typeof(IList&lt;&gt;);
        /// </code>
        /// </summary>
        /// <param name="target">This <see cref="Type"/>.</param>
        /// <param name="interfaceType">The type of generic inteface to check.</param>
        /// <returns><c>true</c> if the type implements the generic interface; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="target"/> is <c>null</c>.</exception>
        public static bool ImplementsGenericInterface(this Type target, Type interfaceType)
        {
            if (target is null)
                throw new ArgumentNullException(nameof(target));
            if (interfaceType is null)
                return false;

            return target.IsGenericType && target.GetGenericTypeDefinition().GetInterfaces().Any(
                x => x.IsGenericType && interfaceType.IsAssignableFrom(x.GetGenericTypeDefinition())
            );
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
        /// "class path" as the type that is passed, making it similar to the Class.getResourceAsStream()
        /// method in Java.
        /// </summary>
        /// <param name="type">A type in the same namespace as the resource.</param>
        /// <param name="name">The resource name to locate.</param>
        /// <returns>an open <see cref="Stream"/> that can be used to read the resource, or <c>null</c> if the resource cannot be found.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="type"/> or <paramref name="name"/> is <c>null</c>.</exception>
        public static Stream? FindAndGetManifestResourceStream(this Type type, string name)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            return AssemblyExtensions.FindAndGetManifestResourceStream(type.Assembly, type, name);
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
        /// <param name="type">A type in the same namespace as the resource.</param>
        /// <param name="name">The resource name to locate.</param>
        /// <returns>The resource, if found; if not found, returns <c>null</c>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="type"/> or <paramref name="name"/> is <c>null</c>.</exception>
        public static string? FindResource(this Type type, string name)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            return AssemblyExtensions.FindResource(type.Assembly, type, name);
        }

        /// <summary>
        /// Returns <c>true</c> if a type is either a reference type
        /// or is a <see cref="Nullable{T}"/> type.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns><c>true</c> if a type is either a reference type
        /// or is a <see cref="Nullable{T}"/> type; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="type"/> is <c>null</c>.</exception>
        internal static bool IsNullableType(this Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            // If this is not a value type, it is a reference type, so it is automatically nullable
            //  (NOTE: All forms of Nullable<T> are value types)
            if (!type.IsValueType)
                return true;

            // Report whether type is a form of the Nullable<> type
            return Nullable.GetUnderlyingType(type) != null;
        }
    }
}
