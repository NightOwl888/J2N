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
using J2N.TestUtilities.Xunit;
using System;
using System.Linq;
using Xunit;
using SCG = System.Collections.Generic;
using static J2N.Collections.Tests.NavigableCollectionHelper;

namespace J2N.Collections.Tests
{
    public abstract class INavigableDictionary_Generic_Tests<TKey, TValue> : IDictionary_Generic_Tests<TKey, TValue>
    {
        #region INavigableDictionary<TKey, TValue> Helper Methods

        protected virtual SCG.IComparer<SCG.KeyValuePair<TKey, TValue>> GetIComparerOrDefault()
        {
            return GetIComparer() ?? new KeyValuePairComparer<TKey, TValue>(Comparer<TKey>.Default);
        }

        protected bool TryCreateTBetween(SCG.KeyValuePair<TKey, TValue> lower, SCG.KeyValuePair<TKey, TValue> upper, out SCG.KeyValuePair<TKey, TValue> result, int seedStart = 10000)
        {
            SCG.IComparer<SCG.KeyValuePair<TKey, TValue>> comparer = GetIComparerOrDefault();
            int seedEnd = seedStart + 10000;
            for (int seed = seedStart; seed < seedEnd; seed++)
            {
                SCG.KeyValuePair<TKey, TValue> candidate = CreateT(seed);
                if (comparer.Compare(candidate, lower) > 0 &&
                    comparer.Compare(candidate, upper) < 0)
                {
                    result = candidate;
                    return true;
                }
            }
            result = default;
            return false;
        }

        protected bool TryCreateTLessThan(SCG.KeyValuePair<TKey, TValue> value, out SCG.KeyValuePair<TKey, TValue> result, int seedStart = 10000)
        {
            SCG.IComparer<SCG.KeyValuePair<TKey, TValue>> comparer = GetIComparerOrDefault();
            int seedEnd = seedStart + 10000;
            for (int seed = seedStart; seed < seedEnd; seed++)
            {
                SCG.KeyValuePair<TKey, TValue> candidate = CreateT(seed);
                if (comparer.Compare(candidate, value) < 0)
                {
                    result = candidate;
                    return true;
                }
            }
            result = default;
            return false;
        }

        protected bool TryCreateTGreaterThan(SCG.KeyValuePair<TKey, TValue> value, out SCG.KeyValuePair<TKey, TValue> result, int seedStart = 10000)
        {
            SCG.IComparer<SCG.KeyValuePair<TKey, TValue>> comparer = GetIComparerOrDefault();
            int seedEnd = seedStart + 10000;
            for (int seed = seedStart; seed < seedEnd; seed++)
            {
                SCG.KeyValuePair<TKey, TValue> candidate = CreateT(seed);
                if (comparer.Compare(candidate, value) > 0)
                {
                    result = candidate;
                    return true;
                }
            }
            result = default;
            return false;
        }

        #endregion INavigableDictionary<TKey, TValue> Helper Methods

        #region First and Last

        // J2N: Added TryGetFirst and TryGetLast methods to replace Min and Max
        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void INavigableDictionary_Generic_FirstAndLast(int count)
        {
            INavigableDictionary<TKey, TValue> dictionary = (INavigableDictionary<TKey, TValue>)GenericIDictionaryFactory(count);
            INavigableDictionary<TKey, TValue> descendingDictionary = dictionary.GetViewDescending();
            if (count >= 3)
            {
                List<SCG.KeyValuePair<TKey, TValue>> expected = dictionary.ToList();
                expected.Sort(GetIComparerOrDefault());

                AssertFirstLastMatch(expected, dictionary, count);

                expected.Reverse();

                AssertFirstLastMatch(expected, descendingDictionary, count);
            }
            else if (count == 0)
            {
                AssertFirstLastMatchEmptyCollection(dictionary);
                AssertFirstLastMatchEmptyCollection(descendingDictionary);
            }
        }

        private void AssertFirstLastMatch(List<SCG.KeyValuePair<TKey, TValue>> expected, INavigableDictionary<TKey, TValue> dictionary, int count)
        {
            Assert.True(dictionary.TryGetFirst(out TKey firstKey, out TValue firstValue));
            Assert.Equal(expected[0].Key, firstKey);
            Assert.Equal(expected[0].Value, firstValue);

            if (IsReadOnly)
            {
                Assert.Throws<NotSupportedException>(() => dictionary.RemoveFirst(out _, out _));
            }
            else
            {
                Assert.True(dictionary.RemoveFirst(out TKey removedFirstKey, out TValue removedFirstValue));
                Assert.Equal(firstKey, removedFirstKey);
                Assert.False(dictionary.ContainsKey(firstKey));
                Assert.True(dictionary.TryGetFirst(out TKey newFirstKey, out _));
                Assert.NotEqual(removedFirstKey, newFirstKey);
                dictionary.Add(removedFirstKey, removedFirstValue); // Restore the collection to its orginal state
                Assert.True(dictionary.ContainsKey(removedFirstKey));
            }

            Assert.True(dictionary.TryGetLast(out TKey lastKey, out TValue lastValue));
            Assert.Equal(expected[count - 1].Key, lastKey);
            Assert.Equal(expected[count - 1].Value, lastValue);

            if (IsReadOnly)
            {
                Assert.Throws<NotSupportedException>(() => dictionary.RemoveLast(out _, out _));
            }
            else
            {
                Assert.True(dictionary.RemoveLast(out TKey removedLastKey, out TValue removedLastValue));
                Assert.Equal(lastKey, removedLastKey);
                Assert.False(dictionary.ContainsKey(lastKey));
                Assert.True(dictionary.TryGetLast(out TKey newLast, out _));
                Assert.NotEqual(removedLastKey, newLast);
                dictionary.Add(removedLastKey, removedLastValue); // Restore the collection to its orginal state
                Assert.True(dictionary.ContainsKey(removedLastKey));
            }
        }

        private void AssertFirstLastMatchEmptyCollection(INavigableDictionary<TKey, TValue> dictionary)
        {
            Assert.False(dictionary.TryGetFirst(out TKey key, out TValue value));
            Assert.Equal(default(TKey), key);
            Assert.Equal(default(TValue), value);

            if (IsReadOnly)
            {
                Assert.Throws<NotSupportedException>(() => dictionary.RemoveFirst(out _, out _));
            }
            else
            {
                Assert.False(dictionary.RemoveFirst(out key, out value));
                Assert.Equal(default(TKey), key);
                Assert.Equal(default(TValue), value);
            }

            Assert.False(dictionary.TryGetLast(out key, out value));
            Assert.Equal(default(TKey), key);
            Assert.Equal(default(TValue), value);

            if (IsReadOnly)
            {
                Assert.Throws<NotSupportedException>(() => dictionary.RemoveLast(out _, out _));
            }
            else
            {
                Assert.False(dictionary.RemoveLast(out key, out value));
                Assert.Equal(default(TKey), key);
                Assert.Equal(default(TValue), value);
            }
        }

        #endregion First and Last

        #region GetView

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void INavigableDictionary_Generic_GetView_EntireSet(int count)
        {
            if (count > 0)
            {
                INavigableDictionary<TKey, TValue> dictionary = (INavigableDictionary<TKey, TValue>)GenericIDictionaryFactory(count);
                SCG.KeyValuePair<TKey, TValue> firstElement = dictionary.ElementAt(0);
                SCG.KeyValuePair<TKey, TValue> lastElement = dictionary.ElementAt(count - 1);
                INavigableDictionary<TKey, TValue> view = dictionary.GetView(firstElement.Key, lastElement.Key);
                List<SCG.KeyValuePair<TKey, TValue>> expected = GetViewExpected(dictionary, firstElement, true, lastElement, true, GetIComparerOrDefault());
                Assert.Equal(expected.Count, view.Count);
                Assert.True(view.SequenceEqual(expected));
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void INavigableDictionary_Generic_GetView_Inclusive_Inclusive_EntireSet(int count)
        {
            if (count > 0)
            {
                INavigableDictionary<TKey, TValue> dictionary = (INavigableDictionary<TKey, TValue>)GenericIDictionaryFactory(count);
                SCG.KeyValuePair<TKey, TValue> firstElement = dictionary.ElementAt(0);
                SCG.KeyValuePair<TKey, TValue> lastElement = dictionary.ElementAt(count - 1);
                INavigableDictionary<TKey, TValue> view = dictionary.GetView(firstElement.Key, true, lastElement.Key, true);
                List<SCG.KeyValuePair<TKey, TValue>> expected = GetViewExpected(dictionary, firstElement, true, lastElement, true, GetIComparerOrDefault());
                Assert.Equal(expected.Count, view.Count);
                Assert.True(view.SequenceEqual(expected));
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void INavigableDictionary_Generic_GetView_Inclusive_Exclusive_EntireSet(int count)
        {
            if (count > 0)
            {
                INavigableDictionary<TKey, TValue> dictionary = (INavigableDictionary<TKey, TValue>)GenericIDictionaryFactory(count);
                SCG.KeyValuePair<TKey, TValue> firstElement = dictionary.ElementAt(0);
                SCG.KeyValuePair<TKey, TValue> lastElement = dictionary.ElementAt(count - 1);
                INavigableDictionary<TKey, TValue> view = dictionary.GetView(firstElement.Key, true, lastElement.Key, false);
                List<SCG.KeyValuePair<TKey, TValue>> expected = GetViewExpected(dictionary, firstElement, true, lastElement, false, GetIComparerOrDefault());
                Assert.Equal(expected.Count, view.Count);
                Assert.True(view.SequenceEqual(expected));
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void INavigableDictionary_Generic_GetView_Exclusive_Inclusive_EntireSet(int count)
        {
            if (count > 0)
            {
                INavigableDictionary<TKey, TValue> dictionary = (INavigableDictionary<TKey, TValue>)GenericIDictionaryFactory(count);
                SCG.KeyValuePair<TKey, TValue> firstElement = dictionary.ElementAt(0);
                SCG.KeyValuePair<TKey, TValue> lastElement = dictionary.ElementAt(count - 1);
                INavigableDictionary<TKey, TValue> view = dictionary.GetView(firstElement.Key, false, lastElement.Key, true);
                List<SCG.KeyValuePair<TKey, TValue>> expected = GetViewExpected(dictionary, firstElement, false, lastElement, true, GetIComparerOrDefault());
                Assert.Equal(expected.Count, view.Count);
                Assert.True(view.SequenceEqual(expected));
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void INavigableDictionary_Generic_GetView_Exclusive_Exclusive_EntireSet(int count)
        {
            if (count > 0)
            {
                INavigableDictionary<TKey, TValue> dictionary = (INavigableDictionary<TKey, TValue>)GenericIDictionaryFactory(count);
                SCG.KeyValuePair<TKey, TValue> firstElement = dictionary.ElementAt(0);
                SCG.KeyValuePair<TKey, TValue> lastElement = dictionary.ElementAt(count - 1);
                INavigableDictionary<TKey, TValue> view = dictionary.GetView(firstElement.Key, false, lastElement.Key, false);
                List<SCG.KeyValuePair<TKey, TValue>> expected = GetViewExpected(dictionary, firstElement, false, lastElement, false, GetIComparerOrDefault());
                Assert.Equal(expected.Count, view.Count);
                Assert.True(view.SequenceEqual(expected));
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void INavigableDictionary_Generic_GetView_MiddleOfSet(int count)
        {
            if (count >= 3)
            {
                INavigableDictionary<TKey, TValue> dictionary = (INavigableDictionary<TKey, TValue>)GenericIDictionaryFactory(count);
                SCG.KeyValuePair<TKey, TValue> firstElement = dictionary.ElementAt(1);
                SCG.KeyValuePair<TKey, TValue> lastElement = dictionary.ElementAt(count - 2);
                INavigableDictionary<TKey, TValue> view = dictionary.GetView(firstElement.Key, lastElement.Key);
                List<SCG.KeyValuePair<TKey, TValue>> expected = GetViewExpected(dictionary, firstElement, true, lastElement, true, GetIComparerOrDefault());
                Assert.Equal(expected.Count, view.Count);
                Assert.True(view.SequenceEqual(expected));
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void INavigableDictionary_Generic_GetView_Inclusive_Inclusive_MiddleOfSet(int count)
        {
            if (count >= 3)
            {
                INavigableDictionary<TKey, TValue> dictionary = (INavigableDictionary<TKey, TValue>)GenericIDictionaryFactory(count);
                SCG.KeyValuePair<TKey, TValue> firstElement = dictionary.ElementAt(1);
                SCG.KeyValuePair<TKey, TValue> lastElement = dictionary.ElementAt(count - 2);
                INavigableDictionary<TKey, TValue> view = dictionary.GetView(firstElement.Key, true, lastElement.Key, true);
                List<SCG.KeyValuePair<TKey, TValue>> expected = GetViewExpected(dictionary, firstElement, true, lastElement, true, GetIComparerOrDefault());
                Assert.Equal(expected.Count, view.Count);
                Assert.True(view.SequenceEqual(expected));
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void INavigableDictionary_Generic_GetView_Inclusive_Exclusive_MiddleOfSet(int count)
        {
            if (count >= 3)
            {
                INavigableDictionary<TKey, TValue> dictionary = (INavigableDictionary<TKey, TValue>)GenericIDictionaryFactory(count);
                SCG.KeyValuePair<TKey, TValue> firstElement = dictionary.ElementAt(1);
                SCG.KeyValuePair<TKey, TValue> lastElement = dictionary.ElementAt(count - 2);
                INavigableDictionary<TKey, TValue> view = dictionary.GetView(firstElement.Key, true, lastElement.Key, false);
                List<SCG.KeyValuePair<TKey, TValue>> expected = GetViewExpected(dictionary, firstElement, true, lastElement, false, GetIComparerOrDefault());
                Assert.Equal(expected.Count, view.Count);
                Assert.True(view.SequenceEqual(expected));
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void INavigableDictionary_Generic_GetView_Exclusive_Inclusive_MiddleOfSet(int count)
        {
            if (count >= 3)
            {
                INavigableDictionary<TKey, TValue> dictionary = (INavigableDictionary<TKey, TValue>)GenericIDictionaryFactory(count);
                SCG.KeyValuePair<TKey, TValue> firstElement = dictionary.ElementAt(1);
                SCG.KeyValuePair<TKey, TValue> lastElement = dictionary.ElementAt(count - 2);
                INavigableDictionary<TKey, TValue> view = dictionary.GetView(firstElement.Key, false, lastElement.Key, true);
                List<SCG.KeyValuePair<TKey, TValue>> expected = GetViewExpected(dictionary, firstElement, false, lastElement, true, GetIComparerOrDefault());
                Assert.Equal(expected.Count, view.Count);
                Assert.True(view.SequenceEqual(expected));
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void INavigableDictionary_Generic_GetView_Exclusive_Exclusive_MiddleOfSet(int count)
        {
            if (count >= 3)
            {
                INavigableDictionary<TKey, TValue> dictionary = (INavigableDictionary<TKey, TValue>)GenericIDictionaryFactory(count);
                SCG.KeyValuePair<TKey, TValue> firstElement = dictionary.ElementAt(1);
                SCG.KeyValuePair<TKey, TValue> lastElement = dictionary.ElementAt(count - 2);
                INavigableDictionary<TKey, TValue> view = dictionary.GetView(firstElement.Key, false, lastElement.Key, false);
                List<SCG.KeyValuePair<TKey, TValue>> expected = GetViewExpected(dictionary, firstElement, false, lastElement, false, GetIComparerOrDefault());
                Assert.Equal(expected.Count, view.Count);
                Assert.True(view.SequenceEqual(expected));
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void INavigableDictionary_Generic_GetView_LowerValueGreaterThanUpperValue_ThrowsArgumentException(int count)
        {
            if (count >= 2)
            {
                SCG.IComparer<SCG.KeyValuePair<TKey, TValue>> comparer = GetIComparerOrDefault();
                INavigableDictionary<TKey, TValue> dictionary = (INavigableDictionary<TKey, TValue>)GenericIDictionaryFactory(count);
                SCG.KeyValuePair<TKey, TValue> firstElement = dictionary.ElementAt(0);
                SCG.KeyValuePair<TKey, TValue> lastElement = dictionary.ElementAt(count - 1);
                if (comparer.Compare(firstElement, lastElement) < 0)
                {
                    AssertExtensions.Throws<ArgumentException>(() => dictionary.GetView(lastElement.Key, firstElement.Key));
                }
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void INavigableDictionary_Generic_GetView_Inclusive_Inclusive_LowerValueGreaterThanUpperValue_ThrowsArgumentException(int count)
        {
            if (count >= 2)
            {
                SCG.IComparer<SCG.KeyValuePair<TKey, TValue>> comparer = GetIComparerOrDefault();
                INavigableDictionary<TKey, TValue> dictionary = (INavigableDictionary<TKey, TValue>)GenericIDictionaryFactory(count);
                SCG.KeyValuePair<TKey, TValue> firstElement = dictionary.ElementAt(0);
                SCG.KeyValuePair<TKey, TValue> lastElement = dictionary.ElementAt(count - 1);
                if (comparer.Compare(firstElement, lastElement) < 0)
                {
                    AssertExtensions.Throws<ArgumentException>(() => dictionary.GetView(lastElement.Key, true, firstElement.Key, true));
                }
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void INavigableDictionary_Generic_GetView_SubsequentOutOfRangeCall_ThrowsArgumentOutOfRangeException(int count)
        {
            if (count >= 3)
            {
                INavigableDictionary<TKey, TValue> dictionary = (INavigableDictionary<TKey, TValue>)GenericIDictionaryFactory(count);
                SCG.IComparer<SCG.KeyValuePair<TKey, TValue>> comparer = GetIComparerOrDefault();
                SCG.KeyValuePair<TKey, TValue> firstElement = dictionary.ElementAt(0);
                SCG.KeyValuePair<TKey, TValue> middleElement = dictionary.ElementAt(count / 2);
                SCG.KeyValuePair<TKey, TValue> lastElement = dictionary.ElementAt(count - 1);
                if ((comparer.Compare(firstElement, middleElement) < 0) && (comparer.Compare(middleElement, lastElement) < 0))
                {
                    INavigableDictionary<TKey, TValue> view = dictionary.GetView(firstElement.Key, middleElement.Key);
                    Assert.Throws<ArgumentOutOfRangeException>(() => view.GetView(middleElement.Key, lastElement.Key));
                }
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void INavigableDictionary_Generic_GetView_Inclusive_Inclusive_SubsequentOutOfRangeCall_ThrowsArgumentOutOfRangeException(int count)
        {
            if (count >= 3)
            {
                INavigableDictionary<TKey, TValue> dictionary = (INavigableDictionary<TKey, TValue>)GenericIDictionaryFactory(count);
                SCG.IComparer<SCG.KeyValuePair<TKey, TValue>> comparer = GetIComparerOrDefault();
                SCG.KeyValuePair<TKey, TValue> firstElement = dictionary.ElementAt(0);
                SCG.KeyValuePair<TKey, TValue> middleElement = dictionary.ElementAt(count / 2);
                SCG.KeyValuePair<TKey, TValue> lastElement = dictionary.ElementAt(count - 1);
                if ((comparer.Compare(firstElement, middleElement) < 0) && (comparer.Compare(middleElement, lastElement) < 0))
                {
                    INavigableDictionary<TKey, TValue> view = dictionary.GetView(firstElement.Key, middleElement.Key);
                    Assert.Throws<ArgumentOutOfRangeException>(() => view.GetView(middleElement.Key, true, lastElement.Key, true));
                }
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void INavigableDictionary_Generic_GetView_Empty_FirstLast(int count)
        {
            if (count < 4) return;

            INavigableDictionary<TKey, TValue> dictionary = (INavigableDictionary<TKey, TValue>)GenericIDictionaryFactory(count);
            Assert.Equal(count, dictionary.Count);

            SCG.KeyValuePair<TKey, TValue> firstElement = dictionary.ElementAt(0);
            SCG.KeyValuePair<TKey, TValue> secondElement = dictionary.ElementAt(1);

            if (!TryCreateTBetween(firstElement, secondElement, out SCG.KeyValuePair<TKey, TValue> first, 10000) ||
                !TryCreateTBetween(first, secondElement, out SCG.KeyValuePair<TKey, TValue> last, 20000))
            {
                // If we can't create two values between the first two elements, then we can't
                // create a view that is guaranteed to be empty, so skip the rest of the test
                return;
            }

            INavigableDictionary<TKey, TValue> view = dictionary.GetView(first.Key, last.Key);
            Assert.Equal(0, view.Count);

            Assert.False(view.TryGetFirst(out TKey key, out TValue value));
            Assert.Equal(default(TKey), key);
            Assert.Equal(default(TValue), value);

            Assert.False(view.TryGetLast(out key, out value));
            Assert.Equal(default(TKey), key);
            Assert.Equal(default(TValue), value);
        }

        #endregion GetView

        #region GetViewBefore

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void INavigableDictionary_Generic_GetViewBefore_EntireSet(int count)
        {
            if (count > 0)
            {
                INavigableDictionary<TKey, TValue> dictionary = (INavigableDictionary<TKey, TValue>)GenericIDictionaryFactory(count);
                SCG.KeyValuePair<TKey, TValue> firstElement = dictionary.ElementAt(0);
                SCG.KeyValuePair<TKey, TValue> lastElement = dictionary.ElementAt(count - 1);
                INavigableDictionary<TKey, TValue> view = dictionary.GetViewBefore(lastElement.Key);
                List<SCG.KeyValuePair<TKey, TValue>> expected = GetViewBeforeExpected(dictionary, lastElement, true, GetIComparerOrDefault());
                Assert.Equal(expected.Count, view.Count);
                Assert.True(view.SequenceEqual(expected));
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void INavigableDictionary_Generic_GetViewBefore_Inclusive_EntireSet(int count)
        {
            if (count > 0)
            {
                INavigableDictionary<TKey, TValue> dictionary = (INavigableDictionary<TKey, TValue>)GenericIDictionaryFactory(count);
                SCG.KeyValuePair<TKey, TValue> firstElement = dictionary.ElementAt(0);
                SCG.KeyValuePair<TKey, TValue> lastElement = dictionary.ElementAt(count - 1);
                INavigableDictionary<TKey, TValue> view = dictionary.GetViewBefore(lastElement.Key, true);
                List<SCG.KeyValuePair<TKey, TValue>> expected = GetViewBeforeExpected(dictionary, lastElement, true, GetIComparerOrDefault());
                Assert.Equal(expected.Count, view.Count);
                Assert.True(view.SequenceEqual(expected));
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void INavigableDictionary_Generic_GetViewBefore_Exclusive_EntireSet(int count)
        {
            if (count > 0)
            {
                INavigableDictionary<TKey, TValue> dictionary = (INavigableDictionary<TKey, TValue>)GenericIDictionaryFactory(count);
                SCG.KeyValuePair<TKey, TValue> firstElement = dictionary.ElementAt(0);
                SCG.KeyValuePair<TKey, TValue> lastElement = dictionary.ElementAt(count - 1);
                INavigableDictionary<TKey, TValue> view = dictionary.GetViewBefore(lastElement.Key, false);
                List<SCG.KeyValuePair<TKey, TValue>> expected = GetViewBeforeExpected(dictionary, lastElement, false, GetIComparerOrDefault());
                Assert.Equal(expected.Count, view.Count);
                Assert.True(view.SequenceEqual(expected));
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void INavigableDictionary_Generic_GetViewBefore_MiddleOfSet(int count)
        {
            if (count >= 3)
            {
                INavigableDictionary<TKey, TValue> dictionary = (INavigableDictionary<TKey, TValue>)GenericIDictionaryFactory(count);
                SCG.KeyValuePair<TKey, TValue> firstElement = dictionary.ElementAt(1);
                SCG.KeyValuePair<TKey, TValue> lastElement = dictionary.ElementAt(count - 2);
                INavigableDictionary<TKey, TValue> view = dictionary.GetViewBefore(lastElement.Key);
                List<SCG.KeyValuePair<TKey, TValue>> expected = GetViewBeforeExpected(dictionary, lastElement, true, GetIComparerOrDefault());
                Assert.Equal(expected.Count, view.Count);
                Assert.True(view.SequenceEqual(expected));
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void INavigableDictionary_Generic_GetViewBefore_Inclusive_MiddleOfSet(int count)
        {
            if (count >= 3)
            {
                INavigableDictionary<TKey, TValue> dictionary = (INavigableDictionary<TKey, TValue>)GenericIDictionaryFactory(count);
                SCG.KeyValuePair<TKey, TValue> firstElement = dictionary.ElementAt(1);
                SCG.KeyValuePair<TKey, TValue> lastElement = dictionary.ElementAt(count - 2);
                INavigableDictionary<TKey, TValue> view = dictionary.GetViewBefore(lastElement.Key, true);
                List<SCG.KeyValuePair<TKey, TValue>> expected = GetViewBeforeExpected(dictionary, lastElement, true, GetIComparerOrDefault());
                Assert.Equal(expected.Count, view.Count);
                Assert.True(view.SequenceEqual(expected));
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void INavigableDictionary_Generic_GetViewBefore_Exclusive_MiddleOfSet(int count)
        {
            if (count >= 3)
            {
                INavigableDictionary<TKey, TValue> dictionary = (INavigableDictionary<TKey, TValue>)GenericIDictionaryFactory(count);
                SCG.KeyValuePair<TKey, TValue> firstElement = dictionary.ElementAt(1);
                SCG.KeyValuePair<TKey, TValue> lastElement = dictionary.ElementAt(count - 2);
                INavigableDictionary<TKey, TValue> view = dictionary.GetViewBefore(lastElement.Key, false);
                List<SCG.KeyValuePair<TKey, TValue>> expected = GetViewBeforeExpected(dictionary, lastElement, false, GetIComparerOrDefault());
                Assert.Equal(expected.Count, view.Count);
                Assert.True(view.SequenceEqual(expected));
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void INavigableDictionary_Generic_GetViewBefore_SubsequentOutOfRangeCall_ThrowsArgumentOutOfRangeException(int count)
        {
            if (count >= 3)
            {
                INavigableDictionary<TKey, TValue> dictionary = (INavigableDictionary<TKey, TValue>)GenericIDictionaryFactory(count);
                SCG.IComparer<SCG.KeyValuePair<TKey, TValue>> comparer = GetIComparerOrDefault();
                SCG.KeyValuePair<TKey, TValue> firstElement = dictionary.ElementAt(0);
                SCG.KeyValuePair<TKey, TValue> middleElement = dictionary.ElementAt(count / 2);
                SCG.KeyValuePair<TKey, TValue> lastElement = dictionary.ElementAt(count - 1);
                if (comparer.Compare(middleElement, lastElement) < 0)
                {
                    // J2N: this was confirmed to match JDK behavior
                    INavigableDictionary<TKey, TValue> view = dictionary.GetViewBefore(middleElement.Key);
                    Assert.Throws<ArgumentOutOfRangeException>(() => view.GetViewBefore(lastElement.Key));
                    Assert.Throws<ArgumentOutOfRangeException>(() => view.GetViewBefore(lastElement.Key, inclusive: true));
                    Assert.Throws<ArgumentOutOfRangeException>(() => view.GetViewBefore(lastElement.Key, inclusive: false));
                    Assert.NotNull(view.GetViewBefore(middleElement.Key, inclusive: true));
                    Assert.NotNull(view.GetViewBefore(middleElement.Key, inclusive: false));
                }
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void INavigableDictionary_Generic_GetViewBefore_Empty_FirstLast(int count)
        {
            if (count < 4) return;

            INavigableDictionary<TKey, TValue> dictionary = (INavigableDictionary<TKey, TValue>)GenericIDictionaryFactory(count);
            Assert.Equal(count, dictionary.Count);

            SCG.KeyValuePair<TKey, TValue> firstElement = dictionary.ElementAt(0);

            if (!TryCreateTLessThan(firstElement, out SCG.KeyValuePair<TKey, TValue> first, 10000))
            {
                // If we can't create a value less than the first element, then we can't
                // create a view that is guaranteed to be empty, so skip the rest of the test
                return;
            }

            INavigableDictionary<TKey, TValue> view = dictionary.GetViewBefore(first.Key);
            Assert.Equal(0, view.Count);

            Assert.False(view.TryGetFirst(out TKey key, out TValue value));
            Assert.Equal(default(TKey), key);
            Assert.Equal(default(TValue), value);

            Assert.False(view.TryGetLast(out key, out value));
            Assert.Equal(default(TKey), key);
            Assert.Equal(default(TValue), value);
        }

        #endregion GetViewBefore

        #region GetViewAfter

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void INavigableDictionary_Generic_GetViewAfter_EntireSet(int count)
        {
            if (count > 0)
            {
                INavigableDictionary<TKey, TValue> dictionary = (INavigableDictionary<TKey, TValue>)GenericIDictionaryFactory(count);
                SCG.KeyValuePair<TKey, TValue> firstElement = dictionary.ElementAt(0);
                SCG.KeyValuePair<TKey, TValue> lastElement = dictionary.ElementAt(count - 1);
                INavigableDictionary<TKey, TValue> view = dictionary.GetViewAfter(firstElement.Key);
                List<SCG.KeyValuePair<TKey, TValue>> expected = GetViewAfterExpected(dictionary, firstElement, true, GetIComparerOrDefault());
                Assert.Equal(expected.Count, view.Count);
                Assert.True(view.SequenceEqual(expected));
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void INavigableDictionary_Generic_GetViewAfter_Inclusive_EntireSet(int count)
        {
            if (count > 0)
            {
                INavigableDictionary<TKey, TValue> dictionary = (INavigableDictionary<TKey, TValue>)GenericIDictionaryFactory(count);
                SCG.KeyValuePair<TKey, TValue> firstElement = dictionary.ElementAt(0);
                SCG.KeyValuePair<TKey, TValue> lastElement = dictionary.ElementAt(count - 1);
                INavigableDictionary<TKey, TValue> view = dictionary.GetViewAfter(firstElement.Key, true);
                List<SCG.KeyValuePair<TKey, TValue>> expected = GetViewAfterExpected(dictionary, firstElement, true, GetIComparerOrDefault());
                Assert.Equal(expected.Count, view.Count);
                Assert.True(view.SequenceEqual(expected));
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void INavigableDictionary_Generic_GetViewAfter_Exclusive_EntireSet(int count)
        {
            if (count > 0)
            {
                INavigableDictionary<TKey, TValue> dictionary = (INavigableDictionary<TKey, TValue>)GenericIDictionaryFactory(count);
                SCG.KeyValuePair<TKey, TValue> firstElement = dictionary.ElementAt(0);
                SCG.KeyValuePair<TKey, TValue> lastElement = dictionary.ElementAt(count - 1);
                INavigableDictionary<TKey, TValue> view = dictionary.GetViewAfter(firstElement.Key, false);
                List<SCG.KeyValuePair<TKey, TValue>> expected = GetViewAfterExpected(dictionary, firstElement, false, GetIComparerOrDefault());
                Assert.Equal(expected.Count, view.Count);
                Assert.True(view.SequenceEqual(expected));
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void INavigableDictionary_Generic_GetViewAfter_MiddleOfSet(int count)
        {
            if (count >= 3)
            {
                INavigableDictionary<TKey, TValue> dictionary = (INavigableDictionary<TKey, TValue>)GenericIDictionaryFactory(count);
                SCG.KeyValuePair<TKey, TValue> firstElement = dictionary.ElementAt(1);
                SCG.KeyValuePair<TKey, TValue> lastElement = dictionary.ElementAt(count - 2);
                INavigableDictionary<TKey, TValue> view = dictionary.GetViewAfter(firstElement.Key);
                List<SCG.KeyValuePair<TKey, TValue>> expected = GetViewAfterExpected(dictionary, firstElement, true, GetIComparerOrDefault());
                Assert.Equal(expected.Count, view.Count);
                Assert.True(view.SequenceEqual(expected));
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void INavigableDictionary_Generic_GetViewAfter_Inclusive_MiddleOfSet(int count)
        {
            if (count >= 3)
            {
                INavigableDictionary<TKey, TValue> dictionary = (INavigableDictionary<TKey, TValue>)GenericIDictionaryFactory(count);
                SCG.KeyValuePair<TKey, TValue> firstElement = dictionary.ElementAt(1);
                SCG.KeyValuePair<TKey, TValue> lastElement = dictionary.ElementAt(count - 2);
                INavigableDictionary<TKey, TValue> view = dictionary.GetViewAfter(firstElement.Key, true);
                List<SCG.KeyValuePair<TKey, TValue>> expected = GetViewAfterExpected(dictionary, firstElement, true, GetIComparerOrDefault());
                Assert.Equal(expected.Count, view.Count);
                Assert.True(view.SequenceEqual(expected));
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void INavigableDictionary_Generic_GetViewAfter_Exclusive_MiddleOfSet(int count)
        {
            if (count >= 3)
            {
                INavigableDictionary<TKey, TValue> dictionary = (INavigableDictionary<TKey, TValue>)GenericIDictionaryFactory(count);
                SCG.KeyValuePair<TKey, TValue> firstElement = dictionary.ElementAt(1);
                SCG.KeyValuePair<TKey, TValue> lastElement = dictionary.ElementAt(count - 2);
                INavigableDictionary<TKey, TValue> view = dictionary.GetViewAfter(firstElement.Key, false);
                List<SCG.KeyValuePair<TKey, TValue>> expected = GetViewAfterExpected(dictionary, firstElement, false, GetIComparerOrDefault());
                Assert.Equal(expected.Count, view.Count);
                Assert.True(view.SequenceEqual(expected));
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void INavigableDictionary_Generic_GetViewAfter_SubsequentOutOfRangeCall_ThrowsArgumentOutOfRangeException(int count)
        {
            if (count >= 3)
            {
                INavigableDictionary<TKey, TValue> dictionary = (INavigableDictionary<TKey, TValue>)GenericIDictionaryFactory(count);
                SCG.IComparer<SCG.KeyValuePair<TKey, TValue>> comparer = GetIComparerOrDefault();
                SCG.KeyValuePair<TKey, TValue> firstElement = dictionary.ElementAt(0);
                SCG.KeyValuePair<TKey, TValue> middleElement = dictionary.ElementAt(count / 2);
                SCG.KeyValuePair<TKey, TValue> lastElement = dictionary.ElementAt(count - 1);
                if ((comparer.Compare(firstElement, middleElement) < 0) && (comparer.Compare(middleElement, lastElement) < 0))
                {
                    // J2N: this was confirmed to match JDK behavior
                    INavigableDictionary<TKey, TValue> view = dictionary.GetViewAfter(middleElement.Key);
                    Assert.Throws<ArgumentOutOfRangeException>(() => view.GetViewAfter(firstElement.Key));
                    Assert.Throws<ArgumentOutOfRangeException>(() => view.GetViewAfter(firstElement.Key, inclusive: true));
                    Assert.Throws<ArgumentOutOfRangeException>(() => view.GetViewAfter(firstElement.Key, inclusive: false));
                    Assert.NotNull(view.GetViewAfter(middleElement.Key, inclusive: true));
                    Assert.NotNull(view.GetViewAfter(middleElement.Key, inclusive: false));
                }
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void INavigableDictionary_Generic_GetViewAfter_Empty_FirstLast(int count)
        {
            if (count < 4) return;

            INavigableDictionary<TKey, TValue> dictionary = (INavigableDictionary<TKey, TValue>)GenericIDictionaryFactory(count);
            Assert.Equal(count, dictionary.Count);

            SCG.KeyValuePair<TKey, TValue> lastElement = dictionary.ElementAt(count - 1);

            if (!TryCreateTGreaterThan(lastElement, out SCG.KeyValuePair<TKey, TValue> last, seedStart: 10000))
            {
                // If we can't create a value greater than the last element, then we can't
                // create a view that is guaranteed to be empty, so skip the rest of the test
                return;
            }

            INavigableDictionary<TKey, TValue> view = dictionary.GetViewAfter(last.Key);
            Assert.Equal(0, view.Count);

            Assert.False(view.TryGetFirst(out TKey key, out TValue value));
            Assert.Equal(default(TKey), key);
            Assert.Equal(default(TValue), value);

            Assert.False(view.TryGetLast(out key, out value));
            Assert.Equal(default(TKey), key);
            Assert.Equal(default(TValue), value);
        }

        #endregion GetViewAfter

        #region Ordering And Enumeration

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void INavigableDictionary_Generic_DictionaryIsProperlySortedAccordingToComparer(int count)
        {
            INavigableDictionary<TKey, TValue> dictionary = (INavigableDictionary<TKey, TValue>)GenericIDictionaryFactory(count);
            List<SCG.KeyValuePair<TKey, TValue>> expected = dictionary.ToList();
            expected.Sort(GetIComparerOrDefault());
            int expectedIndex = 0;
            foreach (SCG.KeyValuePair<TKey, TValue> value in dictionary)
                Assert.Equal(expected[expectedIndex++], value);
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void INavigableDictionary_Generic_ReverseDictionaryIsProperlySortedAccordingToComparer(int count)
        {
            INavigableDictionary<TKey, TValue> dictionary = (INavigableDictionary<TKey, TValue>)GenericIDictionaryFactory(count);
            List<SCG.KeyValuePair<TKey, TValue>> expected = dictionary.ToList();
            expected.Sort(GetIComparerOrDefault());
            expected.Reverse();
            int expectedIndex = 0;
            foreach (SCG.KeyValuePair<TKey, TValue> value in dictionary.Reverse())
                Assert.Equal(expected[expectedIndex++], value);
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void INavigableDictionary_Generic_GetViewDescending_IsProperlySortedAccordingToComparer(int count)
        {
            INavigableDictionary<TKey, TValue> dictionary = (INavigableDictionary<TKey, TValue>)GenericIDictionaryFactory(count);
            INavigableDictionary<TKey, TValue> descendingDictionary = dictionary.GetViewDescending();
            List<SCG.KeyValuePair<TKey, TValue>> expected = dictionary.ToList();
            expected.Sort(GetIComparerOrDefault());
            expected.Reverse();
            int expectedIndex = 0;
            foreach (SCG.KeyValuePair<TKey, TValue> value in descendingDictionary)
                Assert.Equal(expected[expectedIndex++], value);
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void INavigableDictionary_Generic_GetViewDescending_GetViewDescending_IsProperlySortedAccordingToComparer(int count)
        {
            INavigableDictionary<TKey, TValue> dictionary = (INavigableDictionary<TKey, TValue>)GenericIDictionaryFactory(count);
            INavigableDictionary<TKey, TValue> doubleDescendingDictionary = dictionary.GetViewDescending().GetViewDescending();
            List<SCG.KeyValuePair<TKey, TValue>> expected = dictionary.ToList();
            expected.Sort(GetIComparerOrDefault());
            int expectedIndex = 0;
            foreach (SCG.KeyValuePair<TKey, TValue> value in doubleDescendingDictionary)
                Assert.Equal(expected[expectedIndex++], value);
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void INavigableDictionary_Generic_GetViewDescending_HasComparerWithReversedBehavior(int count)
        {
            INavigableDictionary<TKey, TValue> dictionary = (INavigableDictionary<TKey, TValue>)GenericIDictionaryFactory(count);
            INavigableDictionary<TKey, TValue> descendingDictionary = dictionary.GetViewDescending();
            INavigableDictionary<TKey, TValue> doubleDescendingDictionary = dictionary.GetViewDescending().GetViewDescending();
            SCG.IComparer<TKey> originalComparer = dictionary.Comparer;
            SCG.IComparer<TKey> descendingComparer = descendingDictionary.Comparer;
            SCG.IComparer<TKey> doubleDescendingComparer = doubleDescendingDictionary.Comparer;

            List<TKey> keys = dictionary.Keys.ToList();
            int limit = Math.Min(keys.Count, 10);

            for (int i = 0; i < limit; i++)
            {
                for (int j = 0; j < limit; j++)
                {
                    TKey a = keys[i];
                    TKey b = keys[j];

                    int original = originalComparer.Compare(a, b);

                    int reverse = descendingComparer.Compare(a, b);
                    int reverseSwapped = descendingComparer.Compare(b, a);

                    int forward = doubleDescendingComparer.Compare(a, b);
                    int forwardSwapped = doubleDescendingComparer.Compare(b, a);

                    // Core invariant: reversed ordering
                    Assert.Equal(Math.Sign(original), -Math.Sign(reverse));

                    // Symmetry invariant
                    Assert.Equal(Math.Sign(original), Math.Sign(reverseSwapped));

                    // Core invariant: double-reversed ordering (forward)
                    Assert.Equal(Math.Sign(original), Math.Sign(forward));

                    // Symmetry invariant (double-reversed)
                    Assert.Equal(Math.Sign(original), -Math.Sign(forwardSwapped));
                }
            }
        }

        [Fact]
        public void INavigableDictionary_Generic_TestViewEnumerator()
        {
            int count = 200;
            INavigableDictionary<TKey, TValue> dictionary = (INavigableDictionary<TKey, TValue>)GenericIDictionaryFactory(count);
            TKey middleElement = dictionary.ElementAt(count / 2).Key;
            TKey nextToLastElement = dictionary.ElementAt(count - 2).Key;
            INavigableDictionary<TKey, TValue> mySubSet = dictionary.GetView(middleElement, nextToLastElement);

            Assert.True(dictionary.Count > mySubSet.Count); //"not all elements were encountered"

            SCG.IEnumerable<SCG.KeyValuePair<TKey, TValue>> en = mySubSet.Reverse();
            INavigableDictionary<TKey, TValue> descending = mySubSet.GetViewDescending();

            // J2N: Added asserts for descending set comparison
            using var descendingEnumerator = descending.GetEnumerator();
            foreach (SCG.KeyValuePair<TKey, TValue> element in en)
            {
                Assert.True(descendingEnumerator.MoveNext());
                Assert.Equal(element, descendingEnumerator.Current);
            }
            Assert.False(descendingEnumerator.MoveNext());
        }

        #endregion Ordering And Enumeration

        #region TryGetPredecessor

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void INavigableDictionary_Generic_TryGetPredecessor(int count)
        {
            INavigableDictionary<TKey, TValue> dictionary = (INavigableDictionary<TKey, TValue>)GenericIDictionaryFactory(count);
            SCG.IComparer<SCG.KeyValuePair<TKey, TValue>> comparer = GetIComparerOrDefault();

            foreach (SCG.KeyValuePair<TKey, TValue> kvp in dictionary)
            {
                bool foundExpected = TryGetPredecessorExpected(dictionary, kvp, out var expectedKvp, comparer);
                bool foundActual = dictionary.TryGetPredecessor(kvp.Key, out TKey actualKey, out TValue actualValue);

                Assert.Equal(foundExpected, foundActual);
                if (foundExpected)
                {
                    Assert.Equal(expectedKvp.Key, actualKey);
                    Assert.Equal(expectedKvp.Value, actualValue);
                }
            }

            // Descending view
            INavigableDictionary<TKey, TValue> desc = dictionary.GetViewDescending();

            foreach (SCG.KeyValuePair<TKey, TValue> kvp in desc)
            {
                bool foundExpected = TryGetPredecessorExpected(desc, kvp, out var expectedKvp, ReverseComparer<SCG.KeyValuePair<TKey, TValue>>.Create(comparer));
                bool foundActual = desc.TryGetPredecessor(kvp.Key, out TKey actualKey, out TValue actualValue);

                Assert.Equal(foundExpected, foundActual);
                if (foundExpected)
                {
                    Assert.Equal(expectedKvp.Key, actualKey);
                    Assert.Equal(expectedKvp.Value, actualValue);
                }
            }
        }

        #endregion TryGetPredecessor

        #region TryGetSuccessor

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void INavigableDictionary_Generic_TryGetSuccessor(int count)
        {
            INavigableDictionary<TKey, TValue> dictionary = (INavigableDictionary<TKey, TValue>)GenericIDictionaryFactory(count);
            SCG.IComparer<SCG.KeyValuePair<TKey, TValue>> comparer = GetIComparerOrDefault();

            foreach (SCG.KeyValuePair<TKey, TValue> kvp in dictionary)
            {
                bool foundExpected = TryGetSuccessorExpected(dictionary, kvp, out var expectedKvp, comparer);
                bool foundActual = dictionary.TryGetSuccessor(kvp.Key, out TKey actualKey, out TValue actualValue);

                Assert.Equal(foundExpected, foundActual);
                if (foundExpected)
                {
                    Assert.Equal(expectedKvp.Key, actualKey);
                    Assert.Equal(expectedKvp.Value, actualValue);
                }
            }

            // Descending view
            INavigableDictionary<TKey, TValue> desc = dictionary.GetViewDescending();

            foreach (SCG.KeyValuePair<TKey, TValue> kvp in desc)
            {
                bool foundExpected = TryGetSuccessorExpected(desc, kvp, out var expectedKvp, ReverseComparer<SCG.KeyValuePair<TKey, TValue>>.Create(comparer));
                bool foundActual = desc.TryGetSuccessor(kvp.Key, out TKey actualKey, out TValue actualValue);

                Assert.Equal(foundExpected, foundActual);
                if (foundExpected)
                {
                    Assert.Equal(expectedKvp.Key, actualKey);
                    Assert.Equal(expectedKvp.Value, actualValue);
                }
            }
        }

        #endregion TryGetSuccessor

        #region TryGetFloor

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void INavigableDictionary_Generic_TryGetFloor(int count)
        {
            INavigableDictionary<TKey, TValue> dictionary = (INavigableDictionary<TKey, TValue>)GenericIDictionaryFactory(count);
            SCG.IComparer<SCG.KeyValuePair<TKey, TValue>> comparer = GetIComparerOrDefault();

            foreach (SCG.KeyValuePair<TKey, TValue> kvp in dictionary)
            {
                bool foundExpected = TryGetFloorExpected(dictionary, kvp, out var expectedKvp, comparer);
                bool foundActual = dictionary.TryGetFloor(kvp.Key, out TKey actualKey, out TValue actualValue);

                Assert.Equal(foundExpected, foundActual);
                if (foundExpected)
                {
                    Assert.Equal(expectedKvp.Key, actualKey);
                    Assert.Equal(expectedKvp.Value, actualValue);
                }
            }

            INavigableDictionary<TKey, TValue> desc = dictionary.GetViewDescending();

            foreach (SCG.KeyValuePair<TKey, TValue> kvp in desc)
            {
                bool foundExpected = TryGetFloorExpected(desc, kvp, out var expectedKvp, ReverseComparer<SCG.KeyValuePair<TKey, TValue>>.Create(comparer));
                bool foundActual = desc.TryGetFloor(kvp.Key, out TKey actualKey, out TValue actualValue);

                Assert.Equal(foundExpected, foundActual);
                if (foundExpected)
                {
                    Assert.Equal(expectedKvp.Key, actualKey);
                    Assert.Equal(expectedKvp.Value, actualValue);
                }
            }
        }

        #endregion TryGetFloor

        #region TryGetCeiling

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void INavigableDictionary_Generic_TryGetCeiling(int count)
        {
            INavigableDictionary<TKey, TValue> dictionary = (INavigableDictionary<TKey, TValue>)GenericIDictionaryFactory(count);
            SCG.IComparer<SCG.KeyValuePair<TKey, TValue>> comparer = GetIComparerOrDefault();

            foreach (SCG.KeyValuePair<TKey, TValue> value in dictionary)
            {
                bool foundExpected = TryGetCeilingExpected(dictionary, value, out var expectedKvp, comparer);
                bool foundActual = dictionary.TryGetCeiling(value.Key, out TKey actualKey, out TValue actualValue);

                Assert.Equal(foundExpected, foundActual);
                if (foundExpected)
                {
                    Assert.Equal(expectedKvp.Key, actualKey);
                    Assert.Equal(expectedKvp.Value, actualValue);
                }
            }

            INavigableDictionary<TKey, TValue> desc = dictionary.GetViewDescending();

            foreach (SCG.KeyValuePair<TKey, TValue> value in desc)
            {
                bool foundExpected = TryGetCeilingExpected(desc, value, out var expectedKvp, ReverseComparer<SCG.KeyValuePair<TKey, TValue>>.Create(comparer));
                bool foundActual = desc.TryGetCeiling(value.Key, out TKey actualKey, out TValue actualValue);

                Assert.Equal(foundExpected, foundActual);
                if (foundExpected)
                {
                    Assert.Equal(expectedKvp.Key, actualKey);
                    Assert.Equal(expectedKvp.Value, actualValue);
                }
            }
        }

        #endregion TryGetCeiling
    }
}
