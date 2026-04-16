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
    public abstract class INavigableCollection_Generic_Tests<T> : ICollection_Generic_Tests<T>
    {
        #region ICollection<T> Helper Methods

        protected override bool DuplicateValuesAllowed => false;

        #endregion ICollection<T> Helper Methods

        #region INavigableCollection<T> Helper Methods

        protected bool TryCreateTBetween(T lower, T upper, out T result, int seedStart = 10000)
        {
            SCG.IComparer<T> comparer = GetIComparer() ?? Comparer<T>.Default;
            int seedEnd = seedStart + 10000;
            for (int seed = seedStart; seed < seedEnd; seed++)
            {
                T candidate = CreateT(seed);
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

        protected bool TryCreateTLessThan(T value, out T result, int seedStart = 10000)
        {
            SCG.IComparer<T> comparer = GetIComparer() ?? Comparer<T>.Default;
            int seedEnd = seedStart + 10000;
            for (int seed = seedStart; seed < seedEnd; seed++)
            {
                T candidate = CreateT(seed);
                if (comparer.Compare(candidate, value) < 0)
                {
                    result = candidate;
                    return true;
                }
            }
            result = default;
            return false;
        }

        protected bool TryCreateTGreaterThan(T value, out T result, int seedStart = 10000)
        {
            SCG.IComparer<T> comparer = GetIComparer() ?? Comparer<T>.Default;
            int seedEnd = seedStart + 10000;
            for (int seed = seedStart; seed < seedEnd; seed++)
            {
                T candidate = CreateT(seed);
                if (comparer.Compare(candidate, value) > 0)
                {
                    result = candidate;
                    return true;
                }
            }
            result = default;
            return false;
        }

        #endregion INavigableCollection<T> Helper Methods

        #region First and Last

        // J2N: Added TryGetFirst and TryGetLast methods to replace Min and Max
        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void INavigableCollection_Generic_FirstAndLast(int count)
        {
            INavigableCollection<T> collection = (INavigableCollection<T>)GenericICollectionFactory(count);
            INavigableCollection<T> descendingCollection = collection.GetViewDescending();
            if (count >= 3)
            {
                List<T> expected = new List<T>(collection);
                expected.Sort(GetIComparer());

                AssertFirstLastMatch(expected, collection, count);

                expected.Reverse();

                AssertFirstLastMatch(expected, descendingCollection, count);
            }
            else if (count == 0)
            {
                AssertFirstLastMatchEmptyCollection(collection);
                AssertFirstLastMatchEmptyCollection(descendingCollection);
            }
        }

        private void AssertFirstLastMatch(List<T> expected, INavigableCollection<T> collection, int count)
        {
            Assert.True(collection.TryGetFirst(out T first));
            Assert.Equal(expected[0], first);

            if (IsReadOnly)
            {
                Assert.Throws<NotSupportedException>(() => collection.RemoveFirst(out _));
            }
            else
            {
                Assert.True(collection.RemoveFirst(out T removedFirst));
                Assert.Equal(first, removedFirst);
                Assert.False(collection.Contains(first));
                Assert.True(collection.TryGetFirst(out T newFirst));
                Assert.NotEqual(removedFirst, newFirst);
                collection.Add(removedFirst); // Restore the collection to its orginal state
                Assert.True(collection.Contains(removedFirst));
            }

            Assert.True(collection.TryGetLast(out T last));
            Assert.Equal(expected[count - 1], last);

            if (IsReadOnly)
            {
                Assert.Throws<NotSupportedException>(() => collection.RemoveLast(out _));
            }
            else
            {
                Assert.True(collection.RemoveLast(out T removedLast));
                Assert.Equal(last, removedLast);
                Assert.False(collection.Contains(last));
                Assert.True(collection.TryGetLast(out T newLast));
                Assert.NotEqual(removedLast, newLast);
                collection.Add(removedLast); // Restore the collection to its orginal state
                Assert.True(collection.Contains(removedLast));
            }
        }

        private void AssertFirstLastMatchEmptyCollection(INavigableCollection<T> collection)
        {
            Assert.False(collection.TryGetFirst(out T value));
            Assert.Equal(default(T), value);

            if (IsReadOnly)
            {
                Assert.Throws<NotSupportedException>(() => collection.RemoveFirst(out _));
            }
            else
            {
                Assert.False(collection.RemoveFirst(out value));
                Assert.Equal(default(T), value);
            }

            Assert.False(collection.TryGetLast(out value));
            Assert.Equal(default(T), value);

            if (IsReadOnly)
            {
                Assert.Throws<NotSupportedException>(() => collection.RemoveLast(out _));
            }
            else
            {
                Assert.False(collection.RemoveLast(out value));
                Assert.Equal(default(T), value);
            }
        }

        #endregion

        #region GetView

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void INavigableCollection_Generic_GetView_EntireSet(int count)
        {
            if (count > 0)
            {
                INavigableCollection<T> collection = (INavigableCollection<T>)GenericICollectionFactory(count);
                T firstElement = collection.ElementAt(0);
                T lastElement = collection.ElementAt(count - 1);
                INavigableCollection<T> view = collection.GetView(firstElement, lastElement);
                List<T> expected = GetViewExpected(collection, firstElement, true, lastElement, true, GetIComparer());
                Assert.Equal(expected.Count, view.Count);
                Assert.True(view.SequenceEqual(expected));
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void INavigableCollection_Generic_GetView_Inclusive_Inclusive_EntireSet(int count)
        {
            if (count > 0)
            {
                INavigableCollection<T> collection = (INavigableCollection<T>)GenericICollectionFactory(count);
                T firstElement = collection.ElementAt(0);
                T lastElement = collection.ElementAt(count - 1);
                INavigableCollection<T> view = collection.GetView(firstElement, true, lastElement, true);
                List<T> expected = GetViewExpected(collection, firstElement, true, lastElement, true, GetIComparer());
                Assert.Equal(expected.Count, view.Count);
                Assert.True(view.SequenceEqual(expected));
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void INavigableCollection_Generic_GetView_Inclusive_Exclusive_EntireSet(int count)
        {
            if (count > 0)
            {
                INavigableCollection<T> collection = (INavigableCollection<T>)GenericICollectionFactory(count);
                T firstElement = collection.ElementAt(0);
                T lastElement = collection.ElementAt(count - 1);
                INavigableCollection<T> view = collection.GetView(firstElement, true, lastElement, false);
                List<T> expected = GetViewExpected(collection, firstElement, true, lastElement, false, GetIComparer());
                Assert.Equal(expected.Count, view.Count);
                Assert.True(view.SequenceEqual(expected));
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void INavigableCollection_Generic_GetView_Exclusive_Inclusive_EntireSet(int count)
        {
            if (count > 0)
            {
                INavigableCollection<T> collection = (INavigableCollection<T>)GenericICollectionFactory(count);
                T firstElement = collection.ElementAt(0);
                T lastElement = collection.ElementAt(count - 1);
                INavigableCollection<T> view = collection.GetView(firstElement, false, lastElement, true);
                List<T> expected = GetViewExpected(collection, firstElement, false, lastElement, true, GetIComparer());
                Assert.Equal(expected.Count, view.Count);
                Assert.True(view.SequenceEqual(expected));
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void INavigableCollection_Generic_GetView_Exclusive_Exclusive_EntireSet(int count)
        {
            if (count > 0)
            {
                INavigableCollection<T> collection = (INavigableCollection<T>)GenericICollectionFactory(count);
                T firstElement = collection.ElementAt(0);
                T lastElement = collection.ElementAt(count - 1);
                INavigableCollection<T> view = collection.GetView(firstElement, false, lastElement, false);
                List<T> expected = GetViewExpected(collection, firstElement, false, lastElement, false, GetIComparer());
                Assert.Equal(expected.Count, view.Count);
                Assert.True(view.SequenceEqual(expected));
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void INavigableCollection_Generic_GetView_MiddleOfSet(int count)
        {
            if (count >= 3)
            {
                INavigableCollection<T> collection = (INavigableCollection<T>)GenericICollectionFactory(count);
                T firstElement = collection.ElementAt(1);
                T lastElement = collection.ElementAt(count - 2);
                INavigableCollection<T> view = collection.GetView(firstElement, lastElement);
                List<T> expected = GetViewExpected(collection, firstElement, true, lastElement, true, GetIComparer());
                Assert.Equal(expected.Count, view.Count);
                Assert.True(view.SequenceEqual(expected));
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void INavigableCollection_Generic_GetView_Inclusive_Inclusive_MiddleOfSet(int count)
        {
            if (count >= 3)
            {
                INavigableCollection<T> collection = (INavigableCollection<T>)GenericICollectionFactory(count);
                T firstElement = collection.ElementAt(1);
                T lastElement = collection.ElementAt(count - 2);
                INavigableCollection<T> view = collection.GetView(firstElement, true, lastElement, true);
                List<T> expected = GetViewExpected(collection, firstElement, true, lastElement, true, GetIComparer());
                Assert.Equal(expected.Count, view.Count);
                Assert.True(view.SequenceEqual(expected));
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void INavigableCollection_Generic_GetView_Inclusive_Exclusive_MiddleOfSet(int count)
        {
            if (count >= 3)
            {
                INavigableCollection<T> collection = (INavigableCollection<T>)GenericICollectionFactory(count);
                T firstElement = collection.ElementAt(1);
                T lastElement = collection.ElementAt(count - 2);
                INavigableCollection<T> view = collection.GetView(firstElement, true, lastElement, false);
                List<T> expected = GetViewExpected(collection, firstElement, true, lastElement, false, GetIComparer());
                Assert.Equal(expected.Count, view.Count);
                Assert.True(view.SequenceEqual(expected));
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void INavigableCollection_Generic_GetView_Exclusive_Inclusive_MiddleOfSet(int count)
        {
            if (count >= 3)
            {
                INavigableCollection<T> collection = (INavigableCollection<T>)GenericICollectionFactory(count);
                T firstElement = collection.ElementAt(1);
                T lastElement = collection.ElementAt(count - 2);
                INavigableCollection<T> view = collection.GetView(firstElement, false, lastElement, true);
                List<T> expected = GetViewExpected(collection, firstElement, false, lastElement, true, GetIComparer());
                Assert.Equal(expected.Count, view.Count);
                Assert.True(view.SequenceEqual(expected));
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void INavigableCollection_Generic_GetView_Exclusive_Exclusive_MiddleOfSet(int count)
        {
            if (count >= 3)
            {
                INavigableCollection<T> collection = (INavigableCollection<T>)GenericICollectionFactory(count);
                T firstElement = collection.ElementAt(1);
                T lastElement = collection.ElementAt(count - 2);
                INavigableCollection<T> view = collection.GetView(firstElement, false, lastElement, false);
                List<T> expected = GetViewExpected(collection, firstElement, false, lastElement, false, GetIComparer());
                Assert.Equal(expected.Count, view.Count);
                Assert.True(view.SequenceEqual(expected));
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void INavigableCollection_Generic_GetView_LowerValueGreaterThanUpperValue_ThrowsArgumentException(int count)
        {
            if (count >= 2)
            {
                SCG.IComparer<T> comparer = GetIComparer() ?? Comparer<T>.Default;
                INavigableCollection<T> collection = (INavigableCollection<T>)GenericICollectionFactory(count);
                T firstElement = collection.ElementAt(0);
                T lastElement = collection.ElementAt(count - 1);
                if (comparer.Compare(firstElement, lastElement) < 0)
                {
                    AssertExtensions.Throws<ArgumentException>(() => collection.GetView(lastElement, firstElement));
                }
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void INavigableCollection_Generic_GetView_Inclusive_Inclusive_LowerValueGreaterThanUpperValue_ThrowsArgumentException(int count)
        {
            if (count >= 2)
            {
                SCG.IComparer<T> comparer = GetIComparer() ?? Comparer<T>.Default;
                INavigableCollection<T> collection = (INavigableCollection<T>)GenericICollectionFactory(count);
                T firstElement = collection.ElementAt(0);
                T lastElement = collection.ElementAt(count - 1);
                if (comparer.Compare(firstElement, lastElement) < 0)
                {
                    AssertExtensions.Throws<ArgumentException>(() => collection.GetView(lastElement, true, firstElement, true));
                }
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void INavigableCollection_Generic_GetView_SubsequentOutOfRangeCall_ThrowsArgumentOutOfRangeException(int count)
        {
            if (count >= 3)
            {
                INavigableCollection<T> collection = (INavigableCollection<T>)GenericICollectionFactory(count);
                SCG.IComparer<T> comparer = GetIComparer() ?? Comparer<T>.Default;
                T firstElement = collection.ElementAt(0);
                T middleElement = collection.ElementAt(count / 2);
                T lastElement = collection.ElementAt(count - 1);
                if ((comparer.Compare(firstElement, middleElement) < 0) && (comparer.Compare(middleElement, lastElement) < 0))
                {
                    INavigableCollection<T> view = collection.GetView(firstElement, middleElement);
                    Assert.Throws<ArgumentOutOfRangeException>(() => view.GetView(middleElement, lastElement));
                }
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void INavigableCollection_Generic_GetView_Inclusive_Inclusive_SubsequentOutOfRangeCall_ThrowsArgumentOutOfRangeException(int count)
        {
            if (count >= 3)
            {
                INavigableCollection<T> collection = (INavigableCollection<T>)GenericICollectionFactory(count);
                SCG.IComparer<T> comparer = GetIComparer() ?? Comparer<T>.Default;
                T firstElement = collection.ElementAt(0);
                T middleElement = collection.ElementAt(count / 2);
                T lastElement = collection.ElementAt(count - 1);
                if ((comparer.Compare(firstElement, middleElement) < 0) && (comparer.Compare(middleElement, lastElement) < 0))
                {
                    INavigableCollection<T> view = collection.GetView(firstElement, middleElement);
                    Assert.Throws<ArgumentOutOfRangeException>(() => view.GetView(middleElement, true, lastElement, true));
                }
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void INavigableCollection_Generic_GetView_Empty_FirstLast(int count)
        {
            if (count < 4) return;

            INavigableCollection<T> collection = (INavigableCollection<T>)GenericICollectionFactory(count);
            Assert.Equal(count, collection.Count);

            T firstElement = collection.ElementAt(0);
            T secondElement = collection.ElementAt(1);

            if (!TryCreateTBetween(firstElement, secondElement, out T first, 10000) ||
                !TryCreateTBetween(first, secondElement, out T last, 20000))
            {
                // If we can't create two values between the first two elements, then we can't
                // create a view that is guaranteed to be empty, so skip the rest of the test
                return;
            }

            INavigableCollection<T> view = collection.GetView(first, last);
            Assert.Equal(0, view.Count);

            Assert.False(view.TryGetFirst(out T value));
            Assert.Equal(default(T), value);

            Assert.False(view.TryGetLast(out value));
            Assert.Equal(default(T), value);
        }

        #endregion GetView

        #region GetViewBefore

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void INavigableCollection_Generic_GetViewBefore_EntireSet(int count)
        {
            if (count > 0)
            {
                INavigableCollection<T> collection = (INavigableCollection<T>)GenericICollectionFactory(count);
                T firstElement = collection.ElementAt(0);
                T lastElement = collection.ElementAt(count - 1);
                INavigableCollection<T> view = collection.GetViewBefore(lastElement);
                List<T> expected = GetViewBeforeExpected(collection, lastElement, true, GetIComparer());
                Assert.Equal(expected.Count, view.Count);
                Assert.True(view.SequenceEqual(expected));
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void INavigableCollection_Generic_GetViewBefore_Inclusive_EntireSet(int count)
        {
            if (count > 0)
            {
                INavigableCollection<T> collection = (INavigableCollection<T>)GenericICollectionFactory(count);
                T firstElement = collection.ElementAt(0);
                T lastElement = collection.ElementAt(count - 1);
                INavigableCollection<T> view = collection.GetViewBefore(lastElement, true);
                List<T> expected = GetViewBeforeExpected(collection, lastElement, true, GetIComparer());
                Assert.Equal(expected.Count, view.Count);
                Assert.True(view.SequenceEqual(expected));
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void INavigableCollection_Generic_GetViewBefore_Exclusive_EntireSet(int count)
        {
            if (count > 0)
            {
                INavigableCollection<T> collection = (INavigableCollection<T>)GenericICollectionFactory(count);
                T firstElement = collection.ElementAt(0);
                T lastElement = collection.ElementAt(count - 1);
                INavigableCollection<T> view = collection.GetViewBefore(lastElement, false);
                List<T> expected = GetViewBeforeExpected(collection, lastElement, false, GetIComparer());
                Assert.Equal(expected.Count, view.Count);
                Assert.True(view.SequenceEqual(expected));
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void INavigableCollection_Generic_GetViewBefore_MiddleOfSet(int count)
        {
            if (count >= 3)
            {
                INavigableCollection<T> collection = (INavigableCollection<T>)GenericICollectionFactory(count);
                T firstElement = collection.ElementAt(0);
                T lastElement = collection.ElementAt(count - 2);
                INavigableCollection<T> view = collection.GetViewBefore(lastElement);
                List<T> expected = GetViewBeforeExpected(collection, lastElement, true, GetIComparer());
                Assert.Equal(expected.Count, view.Count);
                Assert.True(view.SequenceEqual(expected));
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void INavigableCollection_Generic_GetViewBefore_Inclusive_MiddleOfSet(int count)
        {
            if (count >= 3)
            {
                INavigableCollection<T> collection = (INavigableCollection<T>)GenericICollectionFactory(count);
                T firstElement = collection.ElementAt(0);
                T lastElement = collection.ElementAt(count - 2);
                INavigableCollection<T> view = collection.GetViewBefore(lastElement, true);
                List<T> expected = GetViewBeforeExpected(collection, lastElement, true, GetIComparer());
                Assert.Equal(expected.Count, view.Count);
                Assert.True(view.SequenceEqual(expected));
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void INavigableCollection_Generic_GetViewBefore_Exclusive_MiddleOfSet(int count)
        {
            if (count >= 3)
            {
                INavigableCollection<T> collection = (INavigableCollection<T>)GenericICollectionFactory(count);
                T firstElement = collection.ElementAt(0);
                T lastElement = collection.ElementAt(count - 2);
                INavigableCollection<T> view = collection.GetViewBefore(lastElement, false);
                List<T> expected = GetViewBeforeExpected(collection, lastElement, false, GetIComparer());
                Assert.Equal(expected.Count, view.Count);
                Assert.True(view.SequenceEqual(expected));
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void INavigableCollection_Generic_GetViewBefore_SubsequentOutOfRangeCall_ThrowsArgumentOutOfRangeException(int count)
        {
            if (count >= 3)
            {
                INavigableCollection<T> collection = (INavigableCollection<T>)GenericICollectionFactory(count);
                SCG.IComparer<T> comparer = GetIComparer() ?? Comparer<T>.Default;
                T firstElement = collection.ElementAt(0);
                T middleElement = collection.ElementAt(count / 2);
                T lastElement = collection.ElementAt(count - 1);
                if (comparer.Compare(middleElement, lastElement) < 0)
                {
                    // J2N: this was confirmed to match JDK behavior
                    INavigableCollection<T> view = collection.GetViewBefore(middleElement);
                    Assert.Throws<ArgumentOutOfRangeException>(() => view.GetViewBefore(lastElement));
                    Assert.Throws<ArgumentOutOfRangeException>(() => view.GetViewBefore(lastElement, inclusive: true));
                    Assert.Throws<ArgumentOutOfRangeException>(() => view.GetViewBefore(lastElement, inclusive: false));
                    Assert.NotNull(view.GetViewBefore(middleElement, inclusive: true));
                    Assert.NotNull(view.GetViewBefore(middleElement, inclusive: false));
                }
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void INavigableCollection_Generic_GetViewBefore_Empty_FirstLast(int count)
        {
            if (count < 4) return;

            INavigableCollection<T> collection = (INavigableCollection<T>)GenericICollectionFactory(count);
            Assert.Equal(count, collection.Count);

            T firstElement = collection.ElementAt(0);

            if (!TryCreateTLessThan(firstElement, out T first, 10000))
            {
                // If we can't create a value less than the first element, then we can't
                // create a view that is guaranteed to be empty, so skip the rest of the test
                return;
            }

            INavigableCollection<T> view = collection.GetViewBefore(first);
            Assert.Equal(0, view.Count);

            Assert.False(view.TryGetFirst(out T value));
            Assert.Equal(default(T), value);

            Assert.False(view.TryGetLast(out value));
            Assert.Equal(default(T), value);
        }

        #endregion GetViewBefore

        #region GetViewAfter

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void INavigableCollection_Generic_GetViewAfter_EntireSet(int count)
        {
            if (count > 0)
            {
                INavigableCollection<T> collection = (INavigableCollection<T>)GenericICollectionFactory(count);
                T firstElement = collection.ElementAt(0);
                T lastElement = collection.ElementAt(count - 1);
                INavigableCollection<T> view = collection.GetViewAfter(firstElement);
                List<T> expected = GetViewAfterExpected(collection, firstElement, true, GetIComparer());
                Assert.Equal(expected.Count, view.Count);
                Assert.True(view.SequenceEqual(expected));
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void INavigableCollection_Generic_GetViewAfter_Inclusive_EntireSet(int count)
        {
            if (count > 0)
            {
                INavigableCollection<T> collection = (INavigableCollection<T>)GenericICollectionFactory(count);
                T firstElement = collection.ElementAt(0);
                T lastElement = collection.ElementAt(count - 1);
                INavigableCollection<T> view = collection.GetViewAfter(firstElement, true);
                List<T> expected = GetViewAfterExpected(collection, firstElement, true, GetIComparer());
                Assert.Equal(expected.Count, view.Count);
                Assert.True(view.SequenceEqual(expected));
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void INavigableCollection_Generic_GetViewAfter_Exclusive_EntireSet(int count)
        {
            if (count > 0)
            {
                INavigableCollection<T> collection = (INavigableCollection<T>)GenericICollectionFactory(count);
                T firstElement = collection.ElementAt(0);
                T lastElement = collection.ElementAt(count - 1);
                INavigableCollection<T> view = collection.GetViewAfter(firstElement, false);
                List<T> expected = GetViewAfterExpected(collection, firstElement, false, GetIComparer());
                Assert.Equal(expected.Count, view.Count);
                Assert.True(view.SequenceEqual(expected));
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void INavigableCollection_Generic_GetViewAfter_MiddleOfSet(int count)
        {
            if (count >= 3)
            {
                INavigableCollection<T> collection = (INavigableCollection<T>)GenericICollectionFactory(count);
                T firstElement = collection.ElementAt(1);
                T lastElement = collection.ElementAt(count - 2);
                INavigableCollection<T> view = collection.GetViewAfter(firstElement);
                List<T> expected = GetViewAfterExpected(collection, firstElement, true, GetIComparer());
                Assert.Equal(expected.Count, view.Count);
                Assert.True(view.SequenceEqual(expected));
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void INavigableCollection_Generic_GetViewAfter_Inclusive_MiddleOfSet(int count)
        {
            if (count >= 3)
            {
                INavigableCollection<T> collection = (INavigableCollection<T>)GenericICollectionFactory(count);
                T firstElement = collection.ElementAt(1);
                T lastElement = collection.ElementAt(count - 2);
                INavigableCollection<T> view = collection.GetViewAfter(firstElement, true);
                List<T> expected = GetViewAfterExpected(collection, firstElement, true, GetIComparer());
                Assert.Equal(expected.Count, view.Count);
                Assert.True(view.SequenceEqual(expected));
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void INavigableCollection_Generic_GetViewAfter_Exclusive_MiddleOfSet(int count)
        {
            if (count >= 3)
            {
                INavigableCollection<T> collection = (INavigableCollection<T>)GenericICollectionFactory(count);
                T firstElement = collection.ElementAt(1);
                T lastElement = collection.ElementAt(count - 2);
                INavigableCollection<T> view = collection.GetViewAfter(firstElement, false);
                List<T> expected = GetViewAfterExpected(collection, firstElement, false, GetIComparer());
                Assert.Equal(expected.Count, view.Count);
                Assert.True(view.SequenceEqual(expected));
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void INavigableCollection_Generic_GetViewAfter_SubsequentOutOfRangeCall_ThrowsArgumentOutOfRangeException(int count)
        {
            if (count >= 3)
            {
                INavigableCollection<T> collection = (INavigableCollection<T>)GenericICollectionFactory(count);
                SCG.IComparer<T> comparer = GetIComparer() ?? Comparer<T>.Default;
                T firstElement = collection.ElementAt(0);
                T middleElement = collection.ElementAt(count / 2);
                T lastElement = collection.ElementAt(count - 1);
                if (comparer.Compare(firstElement, middleElement) < 0)
                {
                    // J2N: this was confirmed to match JDK behavior
                    INavigableCollection<T> view = collection.GetViewAfter(middleElement);
                    Assert.Throws<ArgumentOutOfRangeException>(() => view.GetViewAfter(firstElement));
                    Assert.Throws<ArgumentOutOfRangeException>(() => view.GetViewAfter(firstElement, inclusive: true));
                    Assert.Throws<ArgumentOutOfRangeException>(() => view.GetViewAfter(firstElement, inclusive: false));
                    Assert.NotNull(view.GetViewAfter(middleElement, inclusive: true));
                    Assert.NotNull(view.GetViewAfter(middleElement, inclusive: false));
                }
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void INavigableCollection_Generic_GetViewAfter_Empty_FirstLast(int count)
        {
            if (count < 4) return;

            INavigableCollection<T> collection = (INavigableCollection<T>)GenericICollectionFactory(count);
            Assert.Equal(count, collection.Count);

            T lastElement = collection.ElementAt(count - 1);

            if (!TryCreateTGreaterThan(lastElement, out T last, seedStart: 10000))
            {
                // If we can't create a value greater than the last element, then we can't
                // create a view that is guaranteed to be empty, so skip the rest of the test
                return;
            }

            INavigableCollection<T> view = collection.GetViewAfter(last);
            Assert.Equal(0, view.Count);

            Assert.False(view.TryGetFirst(out T value));
            Assert.Equal(default(T), value);

            Assert.False(view.TryGetLast(out value));
            Assert.Equal(default(T), value);
        }

        #endregion GetViewAfter

        #region Enumeration and Ordering

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void INavigableCollection_Generic_SetIsProperlySortedAccordingToComparer(int count)
        {
            INavigableCollection<T> collection = (INavigableCollection<T>)GenericICollectionFactory(count);
            List<T> expected = collection.ToList();
            expected.Sort(GetIComparer());
            int expectedIndex = 0;
            foreach (T value in collection)
                Assert.Equal(expected[expectedIndex++], value);
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void INavigableCollection_Generic_ReverseSetIsProperlySortedAccordingToComparer(int count)
        {
            INavigableCollection<T> collection = (INavigableCollection<T>)GenericICollectionFactory(count);
            List<T> expected = collection.ToList();
            expected.Sort(GetIComparer());
            expected.Reverse();
            int expectedIndex = 0;
            foreach (T value in collection.Reverse())
                Assert.Equal(expected[expectedIndex++], value);
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void INavigableCollection_Generic_GetViewDescending_IsProperlySortedAccordingToComparer(int count)
        {
            INavigableCollection<T> collection = (INavigableCollection<T>)GenericICollectionFactory(count);
            INavigableCollection<T> descendingCollection = collection.GetViewDescending();
            List<T> expected = collection.ToList();
            expected.Sort(GetIComparer());
            expected.Reverse();
            int expectedIndex = 0;
            foreach (T value in descendingCollection)
                Assert.Equal(expected[expectedIndex++], value);
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void INavigableCollection_Generic_GetViewDescending_GetViewDescending_IsProperlySortedAccordingToComparer(int count)
        {
            INavigableCollection<T> collection = (INavigableCollection<T>)GenericICollectionFactory(count);
            INavigableCollection<T> doubleDescendingCollection = collection.GetViewDescending().GetViewDescending();
            List<T> expected = collection.ToList();
            expected.Sort(GetIComparer());
            int expectedIndex = 0;
            foreach (T value in doubleDescendingCollection)
                Assert.Equal(expected[expectedIndex++], value);
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void INavigableCollection_Generic_GetViewDescending_HasComparerWithReversedBehavior(int count)
        {
            INavigableCollection<T> collection = (INavigableCollection<T>)GenericICollectionFactory(count);
            INavigableCollection<T> descendingCollection = collection.GetViewDescending();
            INavigableCollection<T> doubleDescendingCollection = collection.GetViewDescending().GetViewDescending();
            SCG.IComparer<T> originalComparer = collection.Comparer;
            SCG.IComparer<T> descendingComparer = descendingCollection.Comparer;
            SCG.IComparer<T> doubleDescendingComparer = doubleDescendingCollection.Comparer;

            List<T> values = collection.ToList();
            int limit = Math.Min(values.Count, 10);

            for (int i = 0; i < limit; i++)
            {
                for (int j = 0; j < limit; j++)
                {
                    T a = values[i];
                    T b = values[j];

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
        public void INavigableCollection_Generic_GetViewDescending_TestViewEnumerator()
        {
            int count = 200;
            INavigableCollection<T> collection = (INavigableCollection<T>)GenericICollectionFactory(count);
            T middleElement = collection.ElementAt(count / 2);
            T nextToLastElement = collection.ElementAt(count - 2);
            INavigableCollection<T> mySubSet = collection.GetView(middleElement, nextToLastElement);

            Assert.True(collection.Count > mySubSet.Count); //"not all elements were encountered"

            SCG.IEnumerable<T> en = mySubSet.Reverse();
            INavigableCollection<T> descending = mySubSet.GetViewDescending();

            // J2N: Added asserts for descending collection comparison
            using var descendingEnumerator = descending.GetEnumerator();
            foreach (T element in en)
            {
                Assert.True(descendingEnumerator.MoveNext());
                Assert.Equal(element, descendingEnumerator.Current);
            }
            Assert.False(descendingEnumerator.MoveNext());
        }

        #endregion Enumeration and Ordering

        #region TryGetPredecessor

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void INavigableCollection_Generic_TryGetPredecessor(int count)
        {
            INavigableCollection<T> collection = (INavigableCollection<T>)GenericICollectionFactory(count);
            SCG.IComparer<T> comparer = GetIComparer() ?? Comparer<T>.Default;

            foreach (T value in collection)
            {
                bool foundExpected = TryGetPredecessorExpected(collection, value, out T expectedValue, comparer);
                bool foundActual = collection.TryGetPredecessor(value, out T actualValue);

                Assert.Equal(foundExpected, foundActual);
                if (foundExpected)
                    Assert.Equal(expectedValue, actualValue);
            }

            // Descending view
            INavigableCollection<T> desc = collection.GetViewDescending();

            foreach (T value in desc)
            {
                bool foundExpected = TryGetPredecessorExpected(desc, value, out T expectedValue, ReverseComparer<T>.Create(comparer));
                bool foundActual = desc.TryGetPredecessor(value, out T actualValue);

                Assert.Equal(foundExpected, foundActual);
                if (foundExpected)
                    Assert.Equal(expectedValue, actualValue);
            }
        }

        #endregion TryGetPredecessor

        #region TryGetSuccessor

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void INavigableCollection_Generic_TryGetSuccessor(int count)
        {
            INavigableCollection<T> collection = (INavigableCollection<T>)GenericICollectionFactory(count);
            SCG.IComparer<T> comparer = GetIComparer() ?? Comparer<T>.Default;

            foreach (T value in collection)
            {
                bool foundExpected = TryGetSuccessorExpected(collection, value, out T expectedValue, comparer);
                bool foundActual = collection.TryGetSuccessor(value, out T actualValue);

                Assert.Equal(foundExpected, foundActual);
                if (foundExpected)
                    Assert.Equal(expectedValue, actualValue);
            }

            // Descending view
            INavigableCollection<T> desc = collection.GetViewDescending();

            foreach (T value in desc)
            {
                bool foundExpected = TryGetSuccessorExpected(desc, value, out T expectedValue, ReverseComparer<T>.Create(comparer));
                bool foundActual = desc.TryGetSuccessor(value, out T actualValue);

                Assert.Equal(foundExpected, foundActual);
                if (foundExpected)
                    Assert.Equal(expectedValue, actualValue);
            }
        }

        #endregion TryGetSuccessor

        #region TryGetFloor

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void INavigableCollection_Generic_TryGetFloor(int count)
        {
            INavigableCollection<T> collection = (INavigableCollection<T>)GenericICollectionFactory(count);
            SCG.IComparer<T> comparer = GetIComparer() ?? Comparer<T>.Default;

            foreach (T value in collection)
            {
                bool foundExpected = TryGetFloorExpected(collection, value, out T expectedValue, comparer);
                bool foundActual = collection.TryGetFloor(value, out T actualValue);

                Assert.Equal(foundExpected, foundActual);
                if (foundExpected)
                    Assert.Equal(expectedValue, actualValue);
            }

            INavigableCollection<T> desc = collection.GetViewDescending();

            foreach (T value in desc)
            {
                bool foundExpected = TryGetFloorExpected(desc, value, out T expectedValue, ReverseComparer<T>.Create(comparer));
                bool foundActual = desc.TryGetFloor(value, out T actualValue);

                Assert.Equal(foundExpected, foundActual);
                if (foundExpected)
                    Assert.Equal(expectedValue, actualValue);
            }
        }

        #endregion TryGetFloor

        #region TryGetCeiling

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void INavigableCollection_Generic_TryGetCeiling(int count)
        {
            INavigableCollection<T> collection = (INavigableCollection<T>)GenericICollectionFactory(count);
            SCG.IComparer<T> comparer = GetIComparer() ?? Comparer<T>.Default;

            foreach (T value in collection)
            {
                bool foundExpected = TryGetCeilingExpected(collection, value, out T expectedValue, comparer);
                bool foundActual = collection.TryGetCeiling(value, out T actualValue);

                Assert.Equal(foundExpected, foundActual);
                if (foundExpected)
                    Assert.Equal(expectedValue, actualValue);
            }

            INavigableCollection<T> desc = collection.GetViewDescending();

            foreach (T value in desc)
            {
                bool foundExpected = TryGetCeilingExpected(desc, value, out T expectedValue, ReverseComparer<T>.Create(comparer));
                bool foundActual = desc.TryGetCeiling(value, out T actualValue);

                Assert.Equal(foundExpected, foundActual);
                if (foundExpected)
                    Assert.Equal(expectedValue, actualValue);
            }
        }

        #endregion TryGetCeiling
    }
}
