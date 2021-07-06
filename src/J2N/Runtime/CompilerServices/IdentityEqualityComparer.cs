#region Copyright 2019-2021 by Shad Storhaug, Licensed under the Apache License, Version 2.0
/*Licensed to the Apache Software Foundation (ASF) under one or more
 *  contributor license agreements.  See the NOTICE file distributed with
 *  this work for additional information regarding copyright ownership.
 *  The ASF licenses this file to You under the Apache License, Version 2.0
 *  (the "License"); you may not use this file except in compliance with
 *  the License.  You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 *Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *  limitations under the License.
 */
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;


namespace J2N.Runtime.CompilerServices
{
    /// <summary>
    /// An equality comparer that compares objects for reference equality.
    /// <para/>
    /// The comparison is made by calling <see cref="object.ReferenceEquals(object, object)"/>
    /// rather than by calling the <see cref="Object.Equals(object)"/> method.
    /// </summary>
    /// <typeparam name="T">The type of objects to compare. Must be a reference type.</typeparam>
    public sealed class IdentityEqualityComparer<T> : IEqualityComparer<T>, IEqualityComparer
        where T : class
    {
        /// <summary>
        /// Gets the default instance of the
        /// <see cref="IdentityEqualityComparer{T}"/> class.
        /// </summary>
        /// <value>A <see cref="IdentityEqualityComparer{T}"/> instance.</value>
        public static IdentityEqualityComparer<T> Default { get; } = new IdentityEqualityComparer<T>();

        private IdentityEqualityComparer() { } // Singleton instance only

        /// <inheritdoc />
        public bool Equals(T? left, T? right)
        {
            return object.ReferenceEquals(left, right);
        }

        /// <inheritdoc />
        public int GetHashCode(T value)
        {
            return RuntimeHelpers.GetHashCode(value);
        }

        /// <inheritdoc />
        public new bool Equals(object? left, object? right)
        {
            return object.ReferenceEquals(left, right);
        }

        /// <inheritdoc />
        public int GetHashCode(object value)
        {
            return RuntimeHelpers.GetHashCode(value);
        }
    }
}
