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

#if FEATURE_TYPEINFO

using System;
using System.Linq;
using System.Reflection;

namespace J2N
{
    /// <summary>
    /// Extensions to the <see cref="System.Reflection.TypeInfo"/> class.
    /// </summary>
    public static class TypeInfoExtensions
    {
        /// <summary>
        /// Returns true if this type implements the specified generic interface.
        /// <para/>
        /// Examples:
        /// <code>
        /// bool result1 = this.GetType().GetTypeInfo().ImplementsGenericInterface(typeof(IDictionary&lt;,&gt;);
        /// bool result2 = typeof(System.Collections.Generic.List&lt;int&gt;).GetTypeInfo().ImplementsGenericInterface(typeof(IList&lt;&gt;);
        /// </code>
        /// </summary>
        /// <param name="target">This <see cref="TypeInfo"/>.</param>
        /// <param name="interfaceType">The type of generic inteface to check.</param>
        /// <returns><c>true</c> if the type implements the generic interface; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="target"/> or <paramref name="interfaceType"/> is <c>null</c>.</exception>
        public static bool ImplementsGenericInterface(this TypeInfo target, Type interfaceType)
        {
            ThrowHelper.ThrowIfNull(target, ExceptionArgument.target);
            ThrowHelper.ThrowIfNull(interfaceType, ExceptionArgument.interfaceType);

            return target.IsGenericType && target.GetGenericTypeDefinition().GetInterfaces().Any(
                x => x.IsGenericType && interfaceType.IsAssignableFrom(x.GetGenericTypeDefinition())
            );
        }
    }
}
#endif
