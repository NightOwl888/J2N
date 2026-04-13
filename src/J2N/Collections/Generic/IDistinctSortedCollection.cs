
#region Copyright 2019-2026 by Shad Storhaug, Licensed under the Apache License, Version 2.0
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

using System.Collections.Generic;

namespace J2N.Collections.Generic
{
    /// <summary>
    /// A contract that indicates a sorted collection of unique elements that is sorted
    /// by a <see cref="IComparer{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of element in the collection.</typeparam>
    /// <remarks>
    /// This interface can be used to identify collections that maintain a specific order of distinct elements,
    /// which can significantly impact performance of constructor or bulk operations of <see cref="SortedSet{T}"/>
    /// and <see cref="SortedDictionary{TKey, TValue}"/>.
    /// </remarks>
    public interface IDistinctSortedCollection<T> : ISortedCollection<T>
    {
    }
}
