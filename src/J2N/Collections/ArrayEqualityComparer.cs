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
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;


namespace J2N.Collections
{
    /// <summary>
    /// Provides comparers that use structural equality rules for arrays similar to those in Java.
    /// </summary>
    /// <typeparam name="T"></typeparam>
#if FEATURE_SERIALIZABLE
    [Serializable]
#endif
    public abstract class ArrayEqualityComparer<T> : System.Collections.Generic.EqualityComparer<T>
    {
        /// <summary>
        /// Hidden default property that doesn't apply to this class.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new static IEqualityComparer<T>? Default { get; }

        /// <summary>
        /// Gets a structural equality comparer for the specified generic array type with comparison rules similar
        /// to the JDK's Arrays class.
        /// <para/>
        /// This provides a high-performance array comparison that is faster than the
        /// <see cref="System.Collections.StructuralComparisons.StructuralEqualityComparer"/>.
        /// </summary>
        public static IEqualityComparer<T[]> OneDimensional { get; } = OneDimensionalArrayEqualityComparer<T>.Default;
    }
}