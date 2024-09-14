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
using System.Runtime.CompilerServices;


namespace J2N.Collections.Generic.Extensions
{
    /// <summary>
    /// Extensions to the <see cref="IDictionary{TKey, TValue}"/> interface.
    /// </summary>
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Returns a read-only <see cref="J2N.Collections.ObjectModel.ReadOnlyDictionary{TKey, TValue}"/> wrapper for the current dictionary.
        /// </summary>
        /// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
        /// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
        /// <param name="collection">The collection to make read-only.</param>
        /// <returns>An object that acts as a read-only wrapper around the current <see cref="IDictionary{TKey, TValue}"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is <c>null</c>.</exception>
        /// <remarks>
        /// To prevent any modifications to the <see cref="IDictionary{TKey, TValue}"/> object, expose it only through this wrapper.
        /// A <see cref="J2N.Collections.ObjectModel.ReadOnlyDictionary{TKey, TValue}"/> object does not expose methods that modify the collection. However, if
        /// changes are made to the underlying <see cref="IDictionary{TKey, TValue}"/> object, the read-only collection reflects those changes.
        /// <para/>
        /// This method is an O(1) operation.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static J2N.Collections.ObjectModel.ReadOnlyDictionary<TKey, TValue> AsReadOnly<TKey, TValue>(this IDictionary<TKey, TValue> collection)
        {
            return new J2N.Collections.ObjectModel.ReadOnlyDictionary<TKey, TValue>(collection);
        }
    }
}
