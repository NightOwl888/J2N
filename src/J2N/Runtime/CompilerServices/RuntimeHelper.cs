#region Copyright 2019-2024 by Shad Storhaug, Licensed under the Apache License, Version 2.0
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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace J2N.Runtime.CompilerServices
{
    internal static class RuntimeHelper
    {
        /// <summary>
        /// Returns a value that indicates whether the specified type is a reference type or
        /// a value type that contains references.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <returns><c>true</c> if the given type is a reference type or a value type that
        /// contains references; otherwise, <c>false</c>.</returns>
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool IsReferenceOrContainsReferences<T>()
#if FEATURE_RUNTIMEHELPERS_ISREFERENCETYPEORCONTAINSREFERENCES
            => RuntimeHelpers.IsReferenceOrContainsReferences<T>();
#else
            => ReferenceOrContainsReferencesHolder<T>.Value;
#endif

#if !FEATURE_RUNTIMEHELPERS_ISREFERENCETYPEORCONTAINSREFERENCES
        private static class ReferenceOrContainsReferencesHolder<T>
        {
            public static bool Value = LoadValue();

            private static bool LoadValue()
            {
                Type type = typeof(T);

                // If not a value type, it's a reference type
                if (!type.IsValueType)
                    return true;

                // Check if any fields in the value type are reference types or contain references
                return IsReferenceOrContainsReferences(type, new HashSet<Type>());
            }

            private static bool IsReferenceOrContainsReferences(Type type, HashSet<Type> processedTypes)
            {
                // If we've already processed this type for the current structure, skip it
                if (!processedTypes.Add(type))
                    return false;

                // If it's not a value type, it's a reference type
                if (!type.IsValueType)
                    return true;

                // Iterate through all fields of the type
                foreach (var field in type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
                {
                    // If the field is a reference type, or contains references, return true
                    if (IsReferenceOrContainsReferences(field.FieldType, processedTypes))
                        return true;
                }

                return false;
            }
        }
#endif
    }
}
