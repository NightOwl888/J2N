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

using J2N.Collections.Generic;
using System;
using SCG = System.Collections.Generic;
#nullable enable

namespace J2N.Collections.Tests
{
    public static class NavigableCollectionHelper
    {
        public static List<T> GetViewExpected<T>(IDistinctSortedCollection<T> collection, T from, bool fromInclusive, T to, bool toInclusive, SCG.IComparer<T>? comparer)
        {
            comparer ??= Comparer<T>.Default;
            List<T> expected = new List<T>(collection.Count);

            if (comparer.Compare(from, to) > 0)
                throw new ArgumentException("from must be less than or equal to to");

            if (fromInclusive && toInclusive)
            {
                foreach (T element in collection)
                    if (comparer.Compare(element, from) >= 0 &&
                        comparer.Compare(element, to) <= 0)
                        expected.Add(element);
            }
            else if (!fromInclusive && !toInclusive)
            {
                foreach (T element in collection)
                    if (comparer.Compare(element, from) > 0 &&
                        comparer.Compare(element, to) < 0)
                        expected.Add(element);
            }
            else if (!fromInclusive)
            {
                foreach (T element in collection)
                    if (comparer.Compare(element, from) > 0 &&
                        comparer.Compare(element, to) <= 0)
                        expected.Add(element);
            }
            else // !toInclusive
            {
                foreach (T element in collection)
                    if (comparer.Compare(element, from) >= 0 &&
                        comparer.Compare(element, to) < 0)
                        expected.Add(element);
            }

            return expected;
        }

        public static List<T> GetViewBeforeExpected<T>(IDistinctSortedCollection<T> collection, T to, bool inclusive, SCG.IComparer<T>? comparer)
        {
            comparer ??= Comparer<T>.Default;
            List<T> expected = new List<T>(collection.Count);

            if (inclusive)
            {
                foreach (T element in collection)
                    if (comparer.Compare(element, to) <= 0)
                        expected.Add(element);
            }
            else
            {
                foreach (T element in collection)
                    if (comparer.Compare(element, to) < 0)
                        expected.Add(element);
            }
            return expected;
        }

        public static List<T> GetViewAfterExpected<T>(IDistinctSortedCollection<T> collection, T from, bool inclusive, SCG.IComparer<T>? comparer)
        {
            comparer ??= Comparer<T>.Default;
            List<T> expected = new List<T>(collection.Count);

            if (inclusive)
            {
                foreach (T element in collection)
                    if (comparer.Compare(element, from) >= 0)
                        expected.Add(element);
            }
            else
            {
                foreach (T element in collection)
                    if (comparer.Compare(element, from) > 0)
                        expected.Add(element);
            }
            return expected;
        }

        public static bool TryGetPredecessorExpected<T>(IDistinctSortedCollection<T> collection, T element, out T result, SCG.IComparer<T>? comparer)
        {
            comparer ??= Comparer<T>.Default;
            result = default!;
            List<T> list = collection.ToList();
            for (int i = list.Count - 1; i >= 0; i--)
            {
                T current = list[i];
                if (comparer.Compare(current, element) < 0)
                {
                    result = current;
                    return true;
                }
            }
            return false;
        }

        public static bool TryGetSuccessorExpected<T>(IDistinctSortedCollection<T> collection, T element, out T result, SCG.IComparer<T>? comparer)
        {
            comparer ??= Comparer<T>.Default;
            result = default!;
            List<T> list = collection.ToList();
            for (int i = 0; i < list.Count; i++)
            {
                T current = list[i];
                if (comparer.Compare(current, element) > 0)
                {
                    result = current;
                    return true;
                }
            }
            return false;
        }

        public static bool TryGetFloorExpected<T>(IDistinctSortedCollection<T> collection, T element, out T result, SCG.IComparer<T>? comparer)
        {
            comparer ??= Comparer<T>.Default;
            result = default!;
            List<T> list = collection.ToList();
            for (int i = list.Count - 1; i >= 0; i--)
            {
                T current = list[i];
                if (comparer.Compare(current, element) <= 0)
                {
                    result = current;
                    return true;
                }
            }
            return false;
        }

        public static bool TryGetCeilingExpected<T>(IDistinctSortedCollection<T> collection, T element, out T result, SCG.IComparer<T>? comparer)
        {
            comparer ??= Comparer<T>.Default;
            result = default!;
            List<T> list = collection.ToList();
            for (int i = 0; i < list.Count; i++)
            {
                T current = list[i];
                if (comparer.Compare(current, element) >= 0)
                {
                    result = current;
                    return true;
                }
            }
            return false;
        }
    }
}
