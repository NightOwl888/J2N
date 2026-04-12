// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using J2N.Collections.Generic;
using J2N.TestUtilities.Xunit;
using System;
using System.Linq;
using System.Reflection;
using Xunit;
using SCG = System.Collections.Generic;

namespace J2N.Collections.Tests
{
    /// <summary>
    /// Contains tests that ensure the correctness of the SortedSet class.
    /// </summary>
    public abstract class SortedSet_Generic_Tests<T> : ISet_Generic_Tests<T>
    {
        #region ISet<T> Helper Methods

        // J2N: Added virtual properties to control inclusivity of bounds in GetView tests
        private bool? _isDescending;
        protected bool IsDescending => _isDescending ??= IsReverseIComparer(GetIComparer() ?? Comparer<T>.Default);

        protected virtual bool LowerBoundInclusive => true;
        protected virtual bool UpperBoundInclusive => true;

        protected virtual bool FirstInclusive => IsDescending ? UpperBoundInclusive : LowerBoundInclusive;

        protected virtual bool LastInclusive => IsDescending ? LowerBoundInclusive : UpperBoundInclusive;

        protected override SCG.ISet<T> GenericISetFactory()
        {
            return new SortedSet<T>();
        }

        protected static bool IsReverseIComparer(SCG.IComparer<T> comparer)
        {
            return comparer is ReverseComparer<T>;
        }

        private SCG.IComparer<T> GetForwardIComparer()
        {
            SCG.IComparer<T> comparer = GetIComparer() ?? Comparer<T>.Default;

            if (comparer is ReverseComparer<T> reverse)
                return reverse.InnerComparer;

            return comparer;
        }

        #endregion

        #region Constructors

        [Fact]
        public void SortedSet_Generic_Constructor()
        {
            SortedSet<T> set = new SortedSet<T>();
            Assert.Empty(set);
        }

        [Fact]
        public void SortedSet_Generic_Constructor_IComparer()
        {
            SCG.IComparer<T> comparer = GetIComparer();
            SortedSet<T> set = new SortedSet<T>(comparer);
            Assert.Equal(comparer ?? Comparer<T>.Default, set.Comparer);
        }

        [Theory]
        [MemberData(nameof(EnumerableTestData))]
        public void SortedSet_Generic_Constructor_IEnumerable(EnumerableType enumerableType, int setLength, int enumerableLength, int numberOfMatchingElements, int numberOfDuplicateElements)
        {
            _ = setLength;
            _ = numberOfMatchingElements;
            SCG.IEnumerable<T> enumerable = CreateEnumerable(enumerableType, null, enumerableLength, 0, numberOfDuplicateElements);
            SortedSet<T> set = new SortedSet<T>(enumerable);
            Assert.True(set.SetEquals(enumerable));
        }

        [Fact]
        public void SortedSet_Generic_Constructor_IEnumerable_Null()
        {
            Assert.Throws<ArgumentNullException>(() => new SortedSet<T>((SCG.IEnumerable<T>)null));
            Assert.Throws<ArgumentNullException>(() => new SortedSet<T>((SCG.IEnumerable<T>)null, Comparer<T>.Default));
        }

        [Theory]
        [MemberData(nameof(EnumerableTestData))]
        public void SortedSet_Generic_Constructor_IEnumerable_IComparer_Netcoreapp(EnumerableType enumerableType, int setLength, int enumerableLength, int numberOfMatchingElements, int numberOfDuplicateElements)
        {
            _ = setLength;
            _ = numberOfMatchingElements;
            _ = numberOfDuplicateElements;
            SCG.IEnumerable<T> enumerable = CreateEnumerable(enumerableType, null, enumerableLength, 0, 0);
            SortedSet<T> set = new SortedSet<T>(enumerable, GetIComparer());
            Assert.True(set.SetEquals(enumerable));
        }

        [Theory]
        [MemberData(nameof(EnumerableTestData))]
        public void SortedSet_Generic_Constructor_IEnumerable_IComparer_NullComparer_Netcoreapp(EnumerableType enumerableType, int setLength, int enumerableLength, int numberOfMatchingElements, int numberOfDuplicateElements)
        {
            _ = setLength;
            _ = numberOfMatchingElements;
            _ = numberOfDuplicateElements;
            SCG.IEnumerable<T> enumerable = CreateEnumerable(enumerableType, null, enumerableLength, 0, 0);
            SortedSet<T> set = new SortedSet<T>(enumerable, comparer: null);
            Assert.True(set.SetEquals(enumerable));
        }

        #endregion

        #region Max and Min

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedSet_Generic_MaxAndMin(int setLength)
        {
            SortedSet<T> set = (SortedSet<T>)GenericISetFactory(setLength);
            if (setLength > 0)
            {
                List<T> expected = set.ToList();
                expected.Sort(GetIComparer());
                Assert.Equal(expected[0], set.Min);
                Assert.Equal(expected[setLength - 1], set.Max);
            }
            else
            {
                Assert.Equal(default(T), set.Min);
                Assert.Equal(default(T), set.Max);
            }
        }

        #endregion

        #region First and Last

        // J2N: Added First and Last properties to replace Min and Max
        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedSet_Generic_FirstAndLast(int setLength)
        {
            SortedSet<T> set = (SortedSet<T>)GenericISetFactory(setLength);
            SortedSet<T> descendingSet = set.GetViewDescending();
            if (setLength > 0)
            {
                List<T> expected = set.ToList();
                expected.Sort(GetIComparer());

                AssertFirstLastMatch(expected, set, setLength);

                expected.Reverse();

                AssertFirstLastMatch(expected, descendingSet, setLength);
            }
            else
            {
                Assert.Equal(default(T), set.First);
                Assert.Equal(default(T), set.Last);

                Assert.False(set.TryGetFirst(out T value));
                Assert.Equal(default(T), value);

                Assert.False(set.TryGetLast(out value));
                Assert.Equal(default(T), value);
            }

            static void AssertFirstLastMatch(List<T> expected, SortedSet< T> set, int setLength)
            {
                Assert.Equal(expected[0], set.First);
                Assert.Equal(expected[setLength - 1], set.Last);

                Assert.True(set.TryGetFirst(out T value));
                Assert.Equal(expected[0], value);

                Assert.True(set.TryGetLast(out value));
                Assert.Equal(expected[setLength - 1], value);
            }
        }

        #endregion

        #region GetViewBetween

        // J2N: GetViewBetween has been superseded by GetView because the BCL named the parameters wrong for descending views.

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedSet_Generic_GetViewBetween_EntireSet(int setLength)
        {
            if (setLength > 0)
            {
                SortedSet<T> set = (SortedSet<T>)GenericISetFactory(setLength);
                T firstElement = set.ElementAt(0);
                T lastElement = set.ElementAt(setLength - 1);
                SortedSet<T> view = set.GetViewBetween(firstElement, lastElement);
                SCG.List<T> expected = GetExpectedView(set, firstElement, true, lastElement, true);
                Assert.Equal(expected.Count, view.Count);
                Assert.True(view.SequenceEqual(expected));
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedSet_Generic_GetViewBetween_Inclusive_Inclusive_EntireSet(int setLength)
        {
            if (setLength > 0)
            {
                SortedSet<T> set = (SortedSet<T>)GenericISetFactory(setLength);
                T firstElement = set.ElementAt(0);
                T lastElement = set.ElementAt(setLength - 1);
                SortedSet<T> view = set.GetViewBetween(firstElement, true, lastElement, true);
                SCG.List<T> expected = GetExpectedView(set, firstElement, true, lastElement, true);
                Assert.Equal(expected.Count, view.Count);
                Assert.True(view.SequenceEqual(expected));
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedSet_Generic_GetViewBetween_Inclusive_Exclusive_EntireSet(int setLength)
        {
            if (setLength > 0)
            {
                SortedSet<T> set = (SortedSet<T>)GenericISetFactory(setLength);
                T firstElement = set.ElementAt(0);
                T lastElement = set.ElementAt(setLength - 1);
                SortedSet<T> view = set.GetViewBetween(firstElement, true, lastElement, false);
                SCG.List<T> expected = GetExpectedView(set, firstElement, true, lastElement, false);
                Assert.Equal(expected.Count, view.Count);
                Assert.True(view.SequenceEqual(expected));
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedSet_Generic_GetViewBetween_Exclusive_Inclusive_EntireSet(int setLength)
        {
            if (setLength > 0)
            {
                SortedSet<T> set = (SortedSet<T>)GenericISetFactory(setLength);
                T firstElement = set.ElementAt(0);
                T lastElement = set.ElementAt(setLength - 1);
                SortedSet<T> view = set.GetViewBetween(firstElement, false, lastElement, true);
                SCG.List<T> expected = GetExpectedView(set, firstElement, false, lastElement, true);
                Assert.Equal(expected.Count, view.Count);
                Assert.True(view.SequenceEqual(expected));
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedSet_Generic_GetViewBetween_Exclusive_Exclusive_EntireSet(int setLength)
        {
            if (setLength > 0)
            {
                SortedSet<T> set = (SortedSet<T>)GenericISetFactory(setLength);
                T firstElement = set.ElementAt(0);
                T lastElement = set.ElementAt(setLength - 1);
                SortedSet<T> view = set.GetViewBetween(firstElement, false, lastElement, false);
                SCG.List<T> expected = GetExpectedView(set, firstElement, false, lastElement, false);
                Assert.Equal(expected.Count, view.Count);
                Assert.True(view.SequenceEqual(expected));
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedSet_Generic_GetViewBetween_MiddleOfSet(int setLength)
        {
            if (setLength >= 3)
            {
                SortedSet<T> set = (SortedSet<T>)GenericISetFactory(setLength);
                T firstElement = set.ElementAt(1);
                T lastElement = set.ElementAt(setLength - 2);

                SCG.List<T> expected = GetExpectedView(set, firstElement, true, lastElement, true);

                SortedSet<T> view = set.GetViewBetween(firstElement, lastElement);
                Assert.Equal(expected.Count, view.Count);
                Assert.True(view.SequenceEqual(expected));
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedSet_Generic_GetViewBetween_Inclusive_Inclusive_MiddleOfSet(int setLength)
        {
            if (setLength >= 3)
            {
                SortedSet<T> set = (SortedSet<T>)GenericISetFactory(setLength);
                T firstElement = set.ElementAt(1);
                T lastElement = set.ElementAt(setLength - 2);

                SCG.List<T> expected = GetExpectedView(set, firstElement, true, lastElement, true);

                SortedSet<T> view = set.GetViewBetween(firstElement, true, lastElement, true);
                Assert.Equal(expected.Count, view.Count);
                Assert.True(view.SequenceEqual(expected));
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedSet_Generic_GetViewBetween_Inclusive_Exclusive_MiddleOfSet(int setLength)
        {
            if (setLength >= 3)
            {
                SortedSet<T> set = (SortedSet<T>)GenericISetFactory(setLength);
                T firstElement = set.ElementAt(1);
                T lastElement = set.ElementAt(setLength - 2);

                SCG.List<T> expected = GetExpectedView(set, firstElement, true, lastElement, false);

                SortedSet<T> view = set.GetViewBetween(firstElement, true, lastElement, false);
                Assert.Equal(expected.Count, view.Count);
                Assert.True(view.SequenceEqual(expected));
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedSet_Generic_GetViewBetween_Exclusive_Inclusive_MiddleOfSet(int setLength)
        {
            if (setLength >= 3)
            {
                SortedSet<T> set = (SortedSet<T>)GenericISetFactory(setLength);
                T firstElement = set.ElementAt(1);
                T lastElement = set.ElementAt(setLength - 2);

                SCG.List<T> expected = GetExpectedView(set, firstElement, false, lastElement, true);

                SortedSet<T> view = set.GetViewBetween(firstElement, false, lastElement, true);
                Assert.Equal(expected.Count, view.Count);
                Assert.True(view.SequenceEqual(expected));
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedSet_Generic_GetViewBetween_Exclusive_Exclusive_MiddleOfSet(int setLength)
        {
            if (setLength >= 3)
            {
                SortedSet<T> set = (SortedSet<T>)GenericISetFactory(setLength);
                T firstElement = set.ElementAt(1);
                T lastElement = set.ElementAt(setLength - 2);

                SCG.List<T> expected = GetExpectedView(set, firstElement, false, lastElement, false);

                SortedSet<T> view = set.GetViewBetween(firstElement, false, lastElement, false);
                Assert.Equal(expected.Count, view.Count);
                Assert.True(view.SequenceEqual(expected));
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedSet_Generic_GetViewBetween_LowerValueGreaterThanUpperValue_ThrowsArgumentException(int setLength)
        {
            if (setLength >= 2)
            {
                SCG.IComparer<T> comparer = GetIComparer() ?? Comparer<T>.Default;
                SortedSet<T> set = (SortedSet<T>)GenericISetFactory(setLength);
                T firstElement = set.ElementAt(0);
                T lastElement = set.ElementAt(setLength - 1);
                if (comparer.Compare(firstElement, lastElement) < 0)
                {
                    ArgumentException ex = AssertExtensions.Throws<ArgumentException>(() => set.GetViewBetween(lastElement, firstElement));
                    string lowerArgumentName = IsDescending ? "upperValue" : "lowerValue";
                    string upperArgumentName = IsDescending ? "lowerValue" : "upperValue";
                    Assert.Equal(lowerArgumentName, ex.ParamName);
                    Assert.Contains(upperArgumentName, ex.Message);
                }
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedSet_Generic_GetViewBetween_SubsequentOutOfRangeCall_ThrowsArgumentOutOfRangeException(int setLength)
        {
            if (setLength >= 3)
            {
                SortedSet<T> set = (SortedSet<T>)GenericISetFactory(setLength);
                SCG.IComparer<T> comparer = GetIComparer() ?? Comparer<T>.Default;
                T firstElement = set.ElementAt(0);
                T middleElement = set.ElementAt(setLength / 2);
                T lastElement = set.ElementAt(setLength - 1);
                if ((comparer.Compare(firstElement, middleElement) < 0) && (comparer.Compare(middleElement, lastElement) < 0))
                {
                    SortedSet<T> view = set.GetViewBetween(firstElement, middleElement);
                    Assert.Throws<ArgumentOutOfRangeException>(() => view.GetViewBetween(middleElement, lastElement));
                }
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedSet_Generic_GetViewBetween_Empty_FirstLast(int setLength)
        {
            if (setLength < 4) return;

            SortedSet<T> set = (SortedSet<T>)GenericISetFactory(setLength);
            Assert.Equal(setLength, set.Count);

            T firstElement = set.ElementAt(0);
            T secondElement = set.ElementAt(1);
            T nextToLastElement = set.ElementAt(setLength - 2);
            T lastElement = set.ElementAt(setLength - 1);

            T[] items = set.ToArray();
            for (int i = 1; i < setLength - 1; i++)
            {
                set.Remove(items[i]);
            }
            Assert.Equal(2, set.Count);

            SortedSet<T> view = set.GetViewBetween(secondElement, nextToLastElement);
            Assert.Equal(0, view.Count);

            Assert.Equal(default(T), view.Min);
            Assert.Equal(default(T), view.Max);

            Assert.Equal(default(T), view.First);
            Assert.Equal(default(T), view.Last);

            Assert.False(view.TryGetFirst(out T value));
            Assert.Equal(default(T), value);

            Assert.False(view.TryGetLast(out value));
            Assert.Equal(default(T), value);
        }

        #endregion GetViewBetween

        #region GetView

        private SCG.List<T> GetExpectedView(SortedSet<T> set, T fromValue, bool fromInclusive, T toValue, bool toInclusive)
        {
            SCG.IComparer<T> comparer = GetIComparer() ?? Comparer<T>.Default;
            SCG.List<T> expected = new SCG.List<T>(set.Count);

            if (fromInclusive && toInclusive)
            {
                foreach (T value in set)
                    if (comparer.Compare(value, fromValue) >= 0 &&
                        comparer.Compare(value, toValue) <= 0)
                        expected.Add(value);
            }
            else if (!fromInclusive && !toInclusive)
            {
                foreach (T value in set)
                    if (comparer.Compare(value, fromValue) > 0 &&
                        comparer.Compare(value, toValue) < 0)
                        expected.Add(value);
            }
            else if (!fromInclusive)
            {
                foreach (T value in set)
                    if (comparer.Compare(value, fromValue) > 0 &&
                        comparer.Compare(value, toValue) <= 0)
                        expected.Add(value);
            }
            else // !toInclusive
            {
                foreach (T value in set)
                    if (comparer.Compare(value, fromValue) >= 0 &&
                        comparer.Compare(value, toValue) < 0)
                        expected.Add(value);
            }

            return expected;
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedSet_Generic_GetView_EntireSet(int setLength)
        {
            if (setLength > 0)
            {
                SortedSet<T> set = (SortedSet<T>)GenericISetFactory(setLength);
                T firstElement = set.ElementAt(0);
                T lastElement = set.ElementAt(setLength - 1);
                SortedSet<T> view = set.GetView(firstElement, lastElement);
                SCG.List<T> expected = GetExpectedView(set, firstElement, true, lastElement, true);
                Assert.Equal(expected.Count, view.Count);
                Assert.True(view.SequenceEqual(expected));
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedSet_Generic_GetView_Inclusive_Inclusive_EntireSet(int setLength)
        {
            if (setLength > 0)
            {
                SortedSet<T> set = (SortedSet<T>)GenericISetFactory(setLength);
                T firstElement = set.ElementAt(0);
                T lastElement = set.ElementAt(setLength - 1);
                SortedSet<T> view = set.GetView(firstElement, true, lastElement, true);
                SCG.List<T> expected = GetExpectedView(set, firstElement, true, lastElement, true);
                Assert.Equal(expected.Count, view.Count);
                Assert.True(view.SequenceEqual(expected));
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedSet_Generic_GetView_Inclusive_Exclusive_EntireSet(int setLength)
        {
            if (setLength > 0)
            {
                SortedSet<T> set = (SortedSet<T>)GenericISetFactory(setLength);
                T firstElement = set.ElementAt(0);
                T lastElement = set.ElementAt(setLength - 1);
                SortedSet<T> view = set.GetView(firstElement, true, lastElement, false);
                SCG.List<T> expected = GetExpectedView(set, firstElement, true, lastElement, false);
                Assert.Equal(expected.Count, view.Count);
                Assert.True(view.SequenceEqual(expected));
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedSet_Generic_GetView_Exclusive_Inclusive_EntireSet(int setLength)
        {
            if (setLength > 0)
            {
                SortedSet<T> set = (SortedSet<T>)GenericISetFactory(setLength);
                T firstElement = set.ElementAt(0);
                T lastElement = set.ElementAt(setLength - 1);
                SortedSet<T> view = set.GetView(firstElement, false, lastElement, true);
                SCG.List<T> expected = GetExpectedView(set, firstElement, false, lastElement, true);
                Assert.Equal(expected.Count, view.Count);
                Assert.True(view.SequenceEqual(expected));
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedSet_Generic_GetView_Exclusive_Exclusive_EntireSet(int setLength)
        {
            if (setLength > 0)
            {
                SortedSet<T> set = (SortedSet<T>)GenericISetFactory(setLength);
                T firstElement = set.ElementAt(0);
                T lastElement = set.ElementAt(setLength - 1);
                SortedSet<T> view = set.GetView(firstElement, false, lastElement, false);
                SCG.List<T> expected = GetExpectedView(set, firstElement, false, lastElement, false);
                Assert.Equal(expected.Count, view.Count);
                Assert.True(view.SequenceEqual(expected));
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedSet_Generic_GetView_MiddleOfSet(int setLength)
        {
            if (setLength >= 3)
            {
                SortedSet<T> set = (SortedSet<T>)GenericISetFactory(setLength);
                T firstElement = set.ElementAt(1);
                T lastElement = set.ElementAt(setLength - 2);

                SCG.List<T> expected = GetExpectedView(set, firstElement, true, lastElement, true);

                SortedSet<T> view = set.GetView(firstElement, lastElement);
                Assert.Equal(expected.Count, view.Count);
                Assert.True(view.SequenceEqual(expected));
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedSet_Generic_GetView_Inclusive_Inclusive_MiddleOfSet(int setLength)
        {
            if (setLength >= 3)
            {
                SortedSet<T> set = (SortedSet<T>)GenericISetFactory(setLength);
                T firstElement = set.ElementAt(1);
                T lastElement = set.ElementAt(setLength - 2);

                SCG.List<T> expected = GetExpectedView(set, firstElement, true, lastElement, true);

                SortedSet<T> view = set.GetView(firstElement, true, lastElement, true);
                Assert.Equal(expected.Count, view.Count);
                Assert.True(view.SequenceEqual(expected));
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedSet_Generic_GetView_Inclusive_Exclusive_MiddleOfSet(int setLength)
        {
            if (setLength >= 3)
            {
                SortedSet<T> set = (SortedSet<T>)GenericISetFactory(setLength);
                T firstElement = set.ElementAt(1);
                T lastElement = set.ElementAt(setLength - 2);

                SCG.List<T> expected = GetExpectedView(set, firstElement, true, lastElement, false);

                SortedSet<T> view = set.GetView(firstElement, true, lastElement, false);
                Assert.Equal(expected.Count, view.Count);
                Assert.True(view.SequenceEqual(expected));
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedSet_Generic_GetView_Exclusive_Inclusive_MiddleOfSet(int setLength)
        {
            if (setLength >= 3)
            {
                SortedSet<T> set = (SortedSet<T>)GenericISetFactory(setLength);
                T firstElement = set.ElementAt(1);
                T lastElement = set.ElementAt(setLength - 2);

                SCG.List<T> expected = GetExpectedView(set, firstElement, false, lastElement, true);

                SortedSet<T> view = set.GetView(firstElement, false, lastElement, true);
                Assert.Equal(expected.Count, view.Count);
                Assert.True(view.SequenceEqual(expected));
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedSet_Generic_GetView_Exclusive_Exclusive_MiddleOfSet(int setLength)
        {
            if (setLength >= 3)
            {
                SortedSet<T> set = (SortedSet<T>)GenericISetFactory(setLength);
                T firstElement = set.ElementAt(1);
                T lastElement = set.ElementAt(setLength - 2);

                SCG.List<T> expected = GetExpectedView(set, firstElement, false, lastElement, false);

                SortedSet<T> view = set.GetView(firstElement, false, lastElement, false);
                Assert.Equal(expected.Count, view.Count);
                Assert.True(view.SequenceEqual(expected));
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedSet_Generic_GetView_LowerValueGreaterThanUpperValue_ThrowsArgumentException(int setLength)
        {
            if (setLength >= 2)
            {
                SCG.IComparer<T> comparer = GetIComparer() ?? Comparer<T>.Default;
                SortedSet<T> set = (SortedSet<T>)GenericISetFactory(setLength);
                T firstElement = set.ElementAt(0);
                T lastElement = set.ElementAt(setLength - 1);
                if (comparer.Compare(firstElement, lastElement) < 0)
                {
                    const string fromArgumentName = "fromValue";
                    const string toArgumentName = "toValue";
                    string lowerArgumentName = IsDescending ? toArgumentName : fromArgumentName;
                    string upperArgumentName = IsDescending ? fromArgumentName : toArgumentName;

                    ArgumentException exception = AssertExtensions.Throws<ArgumentException>(() => set.GetView(lastElement, firstElement));
                    Assert.Equal(lowerArgumentName, exception.ParamName);
                    Assert.Contains(upperArgumentName, exception.Message);
                }
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedSet_Generic_GetView_Inclusive_Inclusive_LowerValueGreaterThanUpperValue_ThrowsArgumentException(int setLength)
        {
            if (setLength >= 2)
            {
                SCG.IComparer<T> comparer = GetIComparer() ?? Comparer<T>.Default;
                SortedSet<T> set = (SortedSet<T>)GenericISetFactory(setLength);
                T firstElement = set.ElementAt(0);
                T lastElement = set.ElementAt(setLength - 1);
                if (comparer.Compare(firstElement, lastElement) < 0)
                {
                    const string fromArgumentName = "fromValue";
                    const string toArgumentName = "toValue";
                    string lowerArgumentName = IsDescending ? toArgumentName : fromArgumentName;
                    string upperArgumentName = IsDescending ? fromArgumentName : toArgumentName;

                    ArgumentException exception = AssertExtensions.Throws<ArgumentException>(() => set.GetView(lastElement, true, firstElement, true));
                    Assert.Equal(lowerArgumentName, exception.ParamName);
                    Assert.Contains(upperArgumentName, exception.Message);
                }
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedSet_Generic_GetView_SubsequentOutOfRangeCall_ThrowsArgumentOutOfRangeException(int setLength)
        {
            if (setLength >= 3)
            {
                SortedSet<T> set = (SortedSet<T>)GenericISetFactory(setLength);
                SCG.IComparer<T> comparer = GetIComparer() ?? Comparer<T>.Default;
                T firstElement = set.ElementAt(0);
                T middleElement = set.ElementAt(setLength / 2);
                T lastElement = set.ElementAt(setLength - 1);
                if ((comparer.Compare(firstElement, middleElement) < 0) && (comparer.Compare(middleElement, lastElement) < 0))
                {
                    SortedSet<T> view = set.GetView(firstElement, middleElement);
                    Assert.Throws<ArgumentOutOfRangeException>("toValue", () => view.GetView(middleElement, lastElement));
                }
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedSet_Generic_GetView_Inclusive_Inclusive_SubsequentOutOfRangeCall_ThrowsArgumentOutOfRangeException(int setLength)
        {
            if (setLength >= 3)
            {
                SortedSet<T> set = (SortedSet<T>)GenericISetFactory(setLength);
                SCG.IComparer<T> comparer = GetIComparer() ?? Comparer<T>.Default;
                T firstElement = set.ElementAt(0);
                T middleElement = set.ElementAt(setLength / 2);
                T lastElement = set.ElementAt(setLength - 1);
                if ((comparer.Compare(firstElement, middleElement) < 0) && (comparer.Compare(middleElement, lastElement) < 0))
                {
                    SortedSet<T> view = set.GetView(firstElement, middleElement);
                    Assert.Throws<ArgumentOutOfRangeException>("toValue", () => view.GetView(middleElement, true, lastElement, true));
                }
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedSet_Generic_GetView_Empty_FirstLast(int setLength)
        {
            if (setLength < 4) return;

            SortedSet<T> set = (SortedSet<T>)GenericISetFactory(setLength);
            Assert.Equal(setLength, set.Count);

            T firstElement = set.ElementAt(0);
            T secondElement = set.ElementAt(1);
            T nextToLastElement = set.ElementAt(setLength - 2);
            T lastElement = set.ElementAt(setLength - 1);

            T[] items = set.ToArray();
            for (int i = 1; i < setLength - 1; i++)
            {
                set.Remove(items[i]);
            }
            Assert.Equal(2, set.Count);

            SortedSet<T> view = set.GetView(secondElement, nextToLastElement);
            Assert.Equal(0, view.Count);

            Assert.Equal(default(T), view.Min);
            Assert.Equal(default(T), view.Max);

            Assert.Equal(default(T), view.First);
            Assert.Equal(default(T), view.Last);

            Assert.False(view.TryGetFirst(out T value));
            Assert.Equal(default(T), value);

            Assert.False(view.TryGetLast(out value));
            Assert.Equal(default(T), value);
        }

        #endregion GetView

        #region GetViewBefore

        private SCG.List<T> GetExpectedViewBefore(SortedSet<T> set, T toValue, bool inclusive)
        {
            SCG.IComparer<T> comparer = GetIComparer() ?? Comparer<T>.Default;
            SCG.List<T> expected = new SCG.List<T>(set.Count);
            
            if (inclusive)
            {
                foreach (T value in set)
                    if (comparer.Compare(value, toValue) <= 0)
                        expected.Add(value);
            }
            else
            {
                foreach (T value in set)
                    if (comparer.Compare(value, toValue) < 0)
                        expected.Add(value);
            }
            return expected;
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedSet_Generic_GetViewBefore_EntireSet(int setLength)
        {
            if (setLength > 0)
            {
                SortedSet<T> set = (SortedSet<T>)GenericISetFactory(setLength);
                T firstElement = set.ElementAt(0);
                T lastElement = set.ElementAt(setLength - 1);
                SortedSet<T> view = set.GetViewBefore(lastElement);
                SCG.List<T> expected = GetExpectedViewBefore(set, lastElement, true);
                Assert.Equal(expected.Count, view.Count);
                Assert.True(view.SequenceEqual(expected));
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedSet_Generic_GetViewBefore_Inclusive_EntireSet(int setLength)
        {
            if (setLength > 0)
            {
                SortedSet<T> set = (SortedSet<T>)GenericISetFactory(setLength);
                T firstElement = set.ElementAt(0);
                T lastElement = set.ElementAt(setLength - 1);
                SortedSet<T> view = set.GetViewBefore(lastElement, true);
                SCG.List<T> expected = GetExpectedViewBefore(set, lastElement, true);
                Assert.Equal(expected.Count, view.Count);
                Assert.True(view.SequenceEqual(expected));
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedSet_Generic_GetViewBefore_Exclusive_EntireSet(int setLength)
        {
            if (setLength > 0)
            {
                SortedSet<T> set = (SortedSet<T>)GenericISetFactory(setLength);
                T firstElement = set.ElementAt(0);
                T lastElement = set.ElementAt(setLength - 1);
                SortedSet<T> view = set.GetViewBefore(lastElement, false);
                SCG.List<T> expected = GetExpectedViewBefore(set, lastElement, false);
                Assert.Equal(expected.Count, view.Count);
                Assert.True(view.SequenceEqual(expected));
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedSet_Generic_GetViewBefore_MiddleOfSet(int setLength)
        {
            if (setLength >= 3)
            {
                SortedSet<T> set = (SortedSet<T>)GenericISetFactory(setLength);
                T firstElement = set.ElementAt(0);
                T lastElement = set.ElementAt(setLength - 2);

                SCG.List<T> expected = GetExpectedViewBefore(set, lastElement, true);

                SortedSet<T> view = set.GetViewBefore(lastElement);
                Assert.Equal(expected.Count, view.Count);
                Assert.True(view.SequenceEqual(expected));
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedSet_Generic_GetViewBefore_Inclusive_MiddleOfSet(int setLength)
        {
            if (setLength >= 3)
            {
                SortedSet<T> set = (SortedSet<T>)GenericISetFactory(setLength);
                T firstElement = set.ElementAt(0);
                T lastElement = set.ElementAt(setLength - 2);

                SCG.List<T> expected = GetExpectedViewBefore(set, lastElement, true);

                SortedSet<T> view = set.GetViewBefore(lastElement, true);
                Assert.Equal(expected.Count, view.Count);
                Assert.True(view.SequenceEqual(expected));
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedSet_Generic_GetViewBefore_Exclusive_MiddleOfSet(int setLength)
        {
            if (setLength >= 3)
            {
                SortedSet<T> set = (SortedSet<T>)GenericISetFactory(setLength);
                T firstElement = set.ElementAt(0);
                T lastElement = set.ElementAt(setLength - 2);

                SCG.List<T> expected = GetExpectedViewBefore(set, lastElement, false);

                SortedSet<T> view = set.GetViewBefore(lastElement, false);
                Assert.Equal(expected.Count, view.Count);
                Assert.True(view.SequenceEqual(expected));
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedSet_Generic_GetViewBefore_SubsequentOutOfRangeCall_ThrowsArgumentOutOfRangeException(int setLength)
        {
            if (setLength >= 3)
            {
                SortedSet<T> set = (SortedSet<T>)GenericISetFactory(setLength);
                SCG.IComparer<T> comparer = GetForwardIComparer();
                T firstElement = set.ElementAt(0);
                T middleElement = set.ElementAt(setLength / 2);
                T lastElement = set.ElementAt(setLength - 1);
                if (comparer.Compare(middleElement, lastElement) < 0)
                {
                    // J2N: this was confirmed to match JDK behavior
                    SortedSet<T> view = set.GetViewBefore(middleElement);
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
        public void SortedSet_Generic_GetViewBefore_Empty_FirstLast(int setLength)
        {
            if (setLength < 4) return;

            SortedSet<T> set = (SortedSet<T>)GenericISetFactory(setLength);
            Assert.Equal(setLength, set.Count);

            T firstElement = set.ElementAt(0);
            T secondElement = set.ElementAt(1);
            T nextToLastElement = set.ElementAt(setLength - 2);
            T lastElement = set.ElementAt(setLength - 1);

            T[] items = set.ToArray();
            for (int i = 0; i < setLength - 1; i++)
            {
                set.Remove(items[i]);
            }
            Assert.Equal(1, set.Count);

            SortedSet<T> view = set.GetViewBefore(nextToLastElement);
            Assert.Equal(0, view.Count);

            Assert.Equal(default(T), view.Min);
            Assert.Equal(default(T), view.Max);

            Assert.Equal(default(T), view.First);
            Assert.Equal(default(T), view.Last);

            Assert.False(view.TryGetFirst(out T value));
            Assert.Equal(default(T), value);

            Assert.False(view.TryGetLast(out value));
            Assert.Equal(default(T), value);
        }

        #endregion GetViewBefore

        #region GetViewAfter

        private SCG.List<T> GetExpectedViewAfter(SortedSet<T> set, T fromValue, bool inclusive)
        {
            SCG.IComparer<T> comparer = GetIComparer() ?? Comparer<T>.Default;
            SCG.List<T> expected = new SCG.List<T>(set.Count);
            if (inclusive)
            {
                foreach (T value in set)
                    if (comparer.Compare(value, fromValue) >= 0)
                        expected.Add(value);
            }
            else
            {
                foreach (T value in set)
                    if (comparer.Compare(value, fromValue) > 0)
                        expected.Add(value);
            }
            return expected;
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedSet_Generic_GetViewAfter_EntireSet(int setLength)
        {
            if (setLength > 0)
            {
                SortedSet<T> set = (SortedSet<T>)GenericISetFactory(setLength);
                T firstElement = set.ElementAt(0);
                T lastElement = set.ElementAt(setLength - 1);
                SortedSet<T> view = set.GetViewAfter(firstElement);
                SCG.List<T> expected = GetExpectedViewAfter(set, firstElement, true);
                Assert.Equal(expected.Count, view.Count);
                Assert.True(view.SequenceEqual(expected));
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedSet_Generic_GetViewAfter_Inclusive_EntireSet(int setLength)
        {
            if (setLength > 0)
            {
                SortedSet<T> set = (SortedSet<T>)GenericISetFactory(setLength);
                T firstElement = set.ElementAt(0);
                T lastElement = set.ElementAt(setLength - 1);
                SortedSet<T> view = set.GetViewAfter(firstElement, true);
                SCG.List<T> expected = GetExpectedViewAfter(set, firstElement, true);
                Assert.Equal(expected.Count, view.Count);
                Assert.True(view.SequenceEqual(expected));
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedSet_Generic_GetViewAfter_Exclusive_EntireSet(int setLength)
        {
            if (setLength > 0)
            {
                SortedSet<T> set = (SortedSet<T>)GenericISetFactory(setLength);
                T firstElement = set.ElementAt(0);
                T lastElement = set.ElementAt(setLength - 1);
                SortedSet<T> view = set.GetViewAfter(firstElement, false);
                SCG.List<T> expected = GetExpectedViewAfter(set, firstElement, false);
                Assert.Equal(expected.Count, view.Count);
                Assert.True(view.SequenceEqual(expected));
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedSet_Generic_GetViewAfter_MiddleOfSet(int setLength)
        {
            if (setLength >= 3)
            {
                SortedSet<T> set = (SortedSet<T>)GenericISetFactory(setLength);
                T firstElement = set.ElementAt(1);
                T lastElement = set.ElementAt(setLength - 2);

                SCG.List<T> expected = GetExpectedViewAfter(set, firstElement, true);

                SortedSet<T> view = set.GetViewAfter(firstElement);
                Assert.Equal(expected.Count, view.Count);
                Assert.True(view.SequenceEqual(expected));
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedSet_Generic_GetViewAfter_Inclusive_MiddleOfSet(int setLength)
        {
            if (setLength >= 3)
            {
                SortedSet<T> set = (SortedSet<T>)GenericISetFactory(setLength);
                T firstElement = set.ElementAt(1);
                T lastElement = set.ElementAt(setLength - 2);

                SCG.List<T> expected = GetExpectedViewAfter(set, firstElement, true);

                SortedSet<T> view = set.GetViewAfter(firstElement, true);
                Assert.Equal(expected.Count, view.Count);
                Assert.True(view.SequenceEqual(expected));
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedSet_Generic_GetViewAfter_Exclusive_MiddleOfSet(int setLength)
        {
            if (setLength >= 3)
            {
                SortedSet<T> set = (SortedSet<T>)GenericISetFactory(setLength);
                T firstElement = set.ElementAt(1);
                T lastElement = set.ElementAt(setLength - 2);

                SCG.List<T> expected = GetExpectedViewAfter(set, firstElement, false);

                SortedSet<T> view = set.GetViewAfter(firstElement, false);
                Assert.Equal(expected.Count, view.Count);
                Assert.True(view.SequenceEqual(expected));
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedSet_Generic_GetViewAfter_SubsequentOutOfRangeCall_ThrowsArgumentOutOfRangeException(int setLength)
        {
            if (setLength >= 3)
            {
                SortedSet<T> set = (SortedSet<T>)GenericISetFactory(setLength);
                SCG.IComparer<T> comparer = GetForwardIComparer();
                T firstElement = set.ElementAt(0);
                T middleElement = set.ElementAt(setLength / 2);
                T lastElement = set.ElementAt(setLength - 1);
                if (comparer.Compare(firstElement, middleElement) < 0)
                {
                    // J2N: this was confirmed to match JDK behavior
                    SortedSet<T> view = set.GetViewAfter(middleElement);
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
        public void SortedSet_Generic_GetViewAfter_Empty_FirstLast(int setLength)
        {
            if (setLength < 4) return;

            SortedSet<T> set = (SortedSet<T>)GenericISetFactory(setLength);
            Assert.Equal(setLength, set.Count);

            T firstElement = set.ElementAt(0);
            T secondElement = set.ElementAt(1);
            T nextToLastElement = set.ElementAt(setLength - 2);
            T lastElement = set.ElementAt(setLength - 1);

            T[] items = set.ToArray();
            for (int i = 1; i < setLength; i++)
            {
                set.Remove(items[i]);
            }
            Assert.Equal(1, set.Count);

            SortedSet<T> view = set.GetViewAfter(secondElement);
            Assert.Equal(0, view.Count);

            Assert.Equal(default(T), view.Min);
            Assert.Equal(default(T), view.Max);

            Assert.Equal(default(T), view.First);
            Assert.Equal(default(T), view.Last);

            Assert.False(view.TryGetFirst(out T value));
            Assert.Equal(default(T), value);

            Assert.False(view.TryGetLast(out value));
            Assert.Equal(default(T), value);
        }

        #endregion GetViewAfter

        #region RemoveWhere

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedSet_Generic_RemoveWhere_AllElements(int setLength)
        {
            SortedSet<T> set = (SortedSet<T>)GenericISetFactory(setLength);
            int removedCount = set.RemoveWhere((value) => { return true; });
            Assert.Equal(setLength, removedCount);
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedSet_Generic_RemoveWhere_NoElements(int setLength)
        {
            SortedSet<T> set = (SortedSet<T>)GenericISetFactory(setLength);
            int removedCount = set.RemoveWhere((value) => { return false; });
            Assert.Equal(0, removedCount);
            Assert.Equal(setLength, set.Count);
        }

        [Fact]
        public void SortedSet_Generic_RemoveWhere_NullPredicate_ThrowsArgumentNullException()
        {
            SortedSet<T> set = (SortedSet<T>)GenericISetFactory();
            AssertExtensions.Throws<ArgumentNullException>("match", () => set.RemoveWhere(null));
        }

        #endregion

        #region Enumeration and Ordering

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedSet_Generic_SetIsProperlySortedAccordingToComparer(int setLength)
        {
            SortedSet<T> set = (SortedSet<T>)GenericISetFactory(setLength);
            List<T> expected = set.ToList();
            expected.Sort(GetIComparer());
            int expectedIndex = 0;
            foreach (T value in set)
                Assert.Equal(expected[expectedIndex++], value);
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedSet_Generic_ReverseSetIsProperlySortedAccordingToComparer(int setLength)
        {
            SortedSet<T> set = (SortedSet<T>)GenericISetFactory(setLength);
            List<T> expected = set.ToList();
            expected.Sort(GetIComparer());
            expected.Reverse();
            int expectedIndex = 0;
            foreach (T value in set.Reverse())
                Assert.Equal(expected[expectedIndex++], value);
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedSet_Generic_GetViewDescending_IsProperlySortedAccordingToComparer(int setLength)
        {
            SortedSet<T> set = (SortedSet<T>)GenericISetFactory(setLength);
            SortedSet<T> descendingSet = set.GetViewDescending();
            List<T> expected = set.ToList();
            expected.Sort(GetIComparer());
            expected.Reverse();
            int expectedIndex = 0;
            foreach (T value in descendingSet)
                Assert.Equal(expected[expectedIndex++], value);
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedSet_Generic_GetViewDescending_GetViewDescending_IsProperlySortedAccordingToComparer(int setLength)
        {
            SortedSet<T> set = (SortedSet<T>)GenericISetFactory(setLength);
            SortedSet<T> doubleDescendingSet = set.GetViewDescending().GetViewDescending();
            List<T> expected = set.ToList();
            expected.Sort(GetIComparer());
            int expectedIndex = 0;
            foreach (T value in doubleDescendingSet)
                Assert.Equal(expected[expectedIndex++], value);
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedSet_Generic_GetViewDescending_HasComparerWithReversedBehavior(int setLength)
        {
            SortedSet<T> set = (SortedSet<T>)GenericISetFactory(setLength);
            SortedSet<T> descendingSet = set.GetViewDescending();
            SortedSet<T> doubleDescendingSet = set.GetViewDescending().GetViewDescending();
            SCG.IComparer<T> originalComparer = set.Comparer;
            SCG.IComparer<T> descendingComparer = descendingSet.Comparer;
            SCG.IComparer<T> doubleDescendingComparer = doubleDescendingSet.Comparer;

            List<T> values = set.ToList();
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
        public void SortedSet_Generic_TestSubSetEnumerator()
        {
            SortedSet<int> sortedSet = new SortedSet<int>();
            for (int i = 0; i < 10000; i++)
            {
                if (!sortedSet.Contains(i))
                    sortedSet.Add(i);
            }
            SortedSet<int> mySubSet = sortedSet.GetView(45, 90);

            Assert.Equal(46, mySubSet.Count); //"not all elements were encountered"

            SCG.IEnumerable<int> en = mySubSet.Reverse();
            Assert.True(mySubSet.SetEquals(en)); //"Expected to be the same set."

            // J2N: Added asserts for descending set comparison
            SortedSet<int> descending = mySubSet.GetViewDescending();
            using var descendingEnumerator = descending.GetEnumerator();
            foreach (int element in en)
            {
                Assert.True(descendingEnumerator.MoveNext());
                Assert.Equal(element, descendingEnumerator.Current);
            }
            Assert.False(descendingEnumerator.MoveNext());
        }

        #endregion

        #region CopyTo

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedSet_Generic_CopyTo_WithoutIndex(int setLength)
        {
            SortedSet<T> set = (SortedSet<T>)GenericISetFactory(setLength);
            List<T> expected = set.ToList();
            expected.Sort(GetIComparer());
            T[] actual = new T[setLength];
            set.CopyTo(actual);
            Assert.Equal(expected, actual);
        }

        // J2N: Added to test descending set CopyTo method
        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedSet_Generic_CopyTo_GetViewDescending_WithoutIndex_PreservesReverseOrder(int setLength)
        {
            SortedSet<T> set = (SortedSet<T>)GenericISetFactory(setLength);
            SortedSet<T> descendingSet = set.GetViewDescending();
            List<T> expected = descendingSet.ToList();
            expected.Sort(GetIComparer());
            expected.Reverse();
            T[] actual = new T[setLength];
            descendingSet.CopyTo(actual);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedSet_Generic_CopyTo_WithValidFullCount(int setLength)
        {
            SortedSet<T> set = (SortedSet<T>)GenericISetFactory(setLength);
            List<T> expected = set.ToList();
            expected.Sort(GetIComparer());
            T[] actual = new T[setLength];
            set.CopyTo(actual, 0, setLength);
            Assert.Equal(expected, actual);
        }

        // J2N: Added to test descending set CopyTo method
        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedSet_Generic_CopyTo_GetViewDescending_WithValidFullCount_PreservesReverseOrder(int setLength)
        {
            SortedSet<T> set = (SortedSet<T>)GenericISetFactory(setLength);
            SortedSet<T> descendingSet = set.GetViewDescending();
            List<T> expected = descendingSet.ToList();
            expected.Sort(GetIComparer());
            expected.Reverse();
            T[] actual = new T[setLength];
            descendingSet.CopyTo(actual, 0, setLength);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedSet_Generic_CopyTo_NegativeCount_ThrowsArgumentOutOfRangeException(int setLength)
        {
            SortedSet<T> set = (SortedSet<T>)GenericISetFactory(setLength);
            T[] actual = new T[setLength];
            Assert.Throws<ArgumentOutOfRangeException>(() => set.CopyTo(actual, 0, -1));
            Assert.Throws<ArgumentOutOfRangeException>(() => set.CopyTo(actual, 0, int.MinValue));
        }

        #endregion

        #region CreateSetComparer

        [Fact]
        public void SetComparer_SetEqualsTests()
        {
            SCG.List<T> objects = new SCG.List<T>() { CreateT(1), CreateT(2), CreateT(3), CreateT(4), CreateT(5), CreateT(6) };

            var set = new SCG.HashSet<SortedSet<T>>()
            {
                new SortedSet<T> { objects[0], objects[1], objects[2] },
                new SortedSet<T> { objects[3], objects[4], objects[5] }
            };

            var noComparerSet = new SCG.HashSet<SortedSet<T>>()
            {
                new SortedSet<T> { objects[0], objects[1], objects[2] },
                new SortedSet<T> { objects[3], objects[4], objects[5] }
            };

            var comparerSet1 = new SCG.HashSet<SortedSet<T>>(SortedSet<T>.CreateSetComparer())
            {
                new SortedSet<T> { objects[0], objects[1], objects[2] },
                new SortedSet<T> { objects[3], objects[4], objects[5] }
            };

            var comparerSet2 = new SCG.HashSet<SortedSet<T>>(SortedSet<T>.CreateSetComparer())
            {
                new SortedSet<T> { objects[3], objects[4], objects[5] },
                new SortedSet<T> { objects[0], objects[1], objects[2] }
            };

            Assert.True(noComparerSet.SetEquals(set)); // Unlike .NET's SortedSet, ours is structurally equatable by default
            Assert.True(comparerSet1.SetEquals(set));
            Assert.True(comparerSet2.SetEquals(set));
        }
        #endregion

        #region GetSpanAlternateLookup

        [Fact]
        public void GetSpanAlternateLookup_FailsWhenIncompatible()
        {
            var hashSet = new SortedSet<string>(StringComparer.Ordinal);

            hashSet.GetSpanAlternateLookup<char>();
            Assert.True(hashSet.TryGetSpanAlternateLookup<char>(out _));

            Assert.Throws<InvalidOperationException>(() => hashSet.GetSpanAlternateLookup<byte>());
            Assert.Throws<InvalidOperationException>(() => hashSet.GetSpanAlternateLookup<string>());
            Assert.Throws<InvalidOperationException>(() => hashSet.GetSpanAlternateLookup<int>());

            Assert.False(hashSet.TryGetSpanAlternateLookup<byte>(out _));
            Assert.False(hashSet.TryGetSpanAlternateLookup<string>(out _));
            Assert.False(hashSet.TryGetSpanAlternateLookup<int>(out _));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        public void SortedSet_GetSpanAlternateLookup_OperationsMatchUnderlyingSet(int mode)
        {
            // Test with a variety of comparers to ensure that the alternate lookup is consistent with the underlying set
            SortedSet<string> set = new(mode switch
            {
                0 => StringComparer.Ordinal,
                1 => StringComparer.OrdinalIgnoreCase,
                2 => StringComparer.InvariantCulture,
                3 => StringComparer.InvariantCultureIgnoreCase,
                4 => StringComparer.CurrentCulture,
                5 => StringComparer.CurrentCultureIgnoreCase,
                _ => throw new ArgumentOutOfRangeException(nameof(mode))
            });

            AssertSpanLookupMatchesRootSet(set);
            set.Clear();
            AssertSpanLookupMatchesRootSet(set.GetViewDescending());
        }

        [Fact]
        public void SortedSet_GetSpanAlternateLookup_GetView_MatchesSet()
        {
            var set = new SortedSet<string>(StringComparer.Ordinal);
            for (int i = 0; i < 10; i++)
                set.Add(i.ToString());

            var lookup = set.GetSpanAlternateLookup<char>();

            AssertLookupMatchesSet(set, lookup, "3", "6");

            // Descending set/view
            var descendingSet = set.GetViewDescending();
            var descendingLookup = descendingSet.GetSpanAlternateLookup<char>();

            AssertLookupMatchesSet(descendingSet, descendingLookup, "6", "3");

            static void AssertLookupMatchesSet(SortedSet<string> set, SortedSet<string>.SpanAlternateLookup<char> lookup, string from, string to)
            {
                // Inclusive
                var setView = set.GetView(from, to);
                var lookupView = lookup.GetView(from.AsSpan(), to.AsSpan());

                Assert.Equal(setView.ToArray(), lookupView.ToArray());

                setView = set.GetView(from, true, to, true);
                lookupView = lookup.GetView(from.AsSpan(), true, to.AsSpan(), true);

                Assert.Equal(setView.ToArray(), lookupView.ToArray());

                // Exclusive
                setView = set.GetView(from, false, to, false);
                lookupView = lookup.GetView(from.AsSpan(), false, to.AsSpan(), false);

                Assert.Equal(setView.ToArray(), lookupView.ToArray());

                // Mixed
                setView = set.GetView(from, true, to, false);
                lookupView = lookup.GetView(from.AsSpan(), true, to.AsSpan(), false);

                Assert.Equal(setView.ToArray(), lookupView.ToArray());

                setView = set.GetView(from, false, to, true);
                lookupView = lookup.GetView(from.AsSpan(), false, to.AsSpan(), true);

                Assert.Equal(setView.ToArray(), lookupView.ToArray());
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedSet_GetSpanAlternateLookup_GetView_LowerValueGreaterThanUpperValue_ThrowsArgumentException_MatchingSet(int setLength)
        {
            if (setLength >= 2)
            {
                SCG.IComparer<string> comparer = StringComparer.Ordinal;
                var set = new SortedSet<string>(comparer);
                for (int i = 0; i < setLength; i++)
                    set.Add(i.ToString());

                Assert_LowerValueGreaterThanUpperValue_ThrowsArgumentException_MatchingSet(set, comparer, setLength);
                Assert_LowerValueGreaterThanUpperValue_ThrowsArgumentException_MatchingSet(set.GetViewDescending(), ReverseComparer<string>.Create(comparer), setLength);
            }

            static void Assert_LowerValueGreaterThanUpperValue_ThrowsArgumentException_MatchingSet(SortedSet<string> set, SCG.IComparer<string> comparer, int setLength)
            {
                string firstElement = set.ElementAt(0);
                string lastElement = set.ElementAt(setLength - 1);
                if (comparer.Compare(firstElement, lastElement) < 0)
                {
                    var lookup = set.GetSpanAlternateLookup<char>();

                    AssertExtensions.ThrowsSameArgumentException<ArgumentException>(
                        () => set.GetView(lastElement, firstElement),
                        () => lookup.GetView(lastElement.AsSpan(), firstElement.AsSpan()));

                    AssertExtensions.ThrowsSameArgumentException<ArgumentException>(
                        () => set.GetView(lastElement, fromInclusive: true, firstElement, toInclusive: true),
                        () => lookup.GetView(lastElement.AsSpan(), fromInclusive: true, firstElement.AsSpan(), toInclusive: true));

                    AssertExtensions.ThrowsSameArgumentException<ArgumentException>(
                       () => set.GetView(lastElement, fromInclusive: true, firstElement, toInclusive: false),
                       () => lookup.GetView(lastElement.AsSpan(), fromInclusive: true, firstElement.AsSpan(), toInclusive: false));

                    AssertExtensions.ThrowsSameArgumentException<ArgumentException>(
                       () => set.GetView(lastElement, fromInclusive: false, firstElement, toInclusive: true),
                       () => lookup.GetView(lastElement.AsSpan(), fromInclusive: false, firstElement.AsSpan(), toInclusive: true));

                    AssertExtensions.ThrowsSameArgumentException<ArgumentException>(
                       () => set.GetView(lastElement, fromInclusive: false, firstElement, toInclusive: false),
                       () => lookup.GetView(lastElement.AsSpan(), fromInclusive: false, firstElement.AsSpan(), toInclusive: false));
                }
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedSet_GetSpanAlternateLookup_GetView_SubsequentOutOfRangeCall_ThrowsArgumentOutOfRangeException_MatchingSet(int setLength)
        {
            if (setLength >= 3)
            {
                SCG.IComparer<string> comparer = StringComparer.Ordinal;
                var set = new SortedSet<string>(comparer);
                for (int i = 0; i < setLength; i++)
                    set.Add(i.ToString());

                Assert_SubsequentOutOfRangeCall_ThrowsArgumentOutOfRangeException_MatchingSet(set, comparer, setLength);
                Assert_SubsequentOutOfRangeCall_ThrowsArgumentOutOfRangeException_MatchingSet(set.GetViewDescending(), ReverseComparer<string>.Create(comparer), setLength);
            }

            static void Assert_SubsequentOutOfRangeCall_ThrowsArgumentOutOfRangeException_MatchingSet(SortedSet<string> set, SCG.IComparer<string> comparer, int setLength)
            {
                string firstElement = set.ElementAt(0);
                string middleElement = set.ElementAt(setLength / 2);
                string lastElement = set.ElementAt(setLength - 1);
                if ((comparer.Compare(firstElement, middleElement) < 0) && (comparer.Compare(middleElement, lastElement) < 0))
                {
                    SortedSet<string> view = set.GetView(firstElement, middleElement);
                    var lookup = view.GetSpanAlternateLookup<char>();

                    AssertExtensions.ThrowsSameArgumentException<ArgumentOutOfRangeException>(
                        () => view.GetView(middleElement, lastElement),
                        () => lookup.GetView(middleElement.AsSpan(), lastElement.AsSpan()));

                    AssertExtensions.ThrowsSameArgumentException<ArgumentOutOfRangeException>(
                        () => view.GetView(middleElement, fromInclusive: true, lastElement, toInclusive: true),
                        () => lookup.GetView(middleElement.AsSpan(), fromInclusive: true, lastElement.AsSpan(), toInclusive: true));

                    AssertExtensions.ThrowsSameArgumentException<ArgumentOutOfRangeException>(
                        () => view.GetView(middleElement, fromInclusive: true, lastElement, toInclusive: false),
                        () => lookup.GetView(middleElement.AsSpan(), fromInclusive: true, lastElement.AsSpan(), toInclusive: false));

                    AssertExtensions.ThrowsSameArgumentException<ArgumentOutOfRangeException>(
                        () => view.GetView(middleElement, fromInclusive: false, lastElement, toInclusive: true),
                        () => lookup.GetView(middleElement.AsSpan(), fromInclusive: false, lastElement.AsSpan(), toInclusive: true));

                    AssertExtensions.ThrowsSameArgumentException<ArgumentOutOfRangeException>(
                        () => view.GetView(middleElement, fromInclusive: false, lastElement, toInclusive: false),
                        () => lookup.GetView(middleElement.AsSpan(), fromInclusive: false, lastElement.AsSpan(), toInclusive: false));
                }
            }
        }

        [Fact]
        public void SortedSet_GetSpanAlternateLookup_GetViewBefore_MatchesSet()
        {
            var set = new SortedSet<string>(StringComparer.Ordinal);
            for (int i = 0; i < 10; i++)
                set.Add(i.ToString());

            var lookup = set.GetSpanAlternateLookup<char>();

            AssertLookupMatchesSet(set, lookup);

            // Descending set/view
            var descendingSet = set.GetViewDescending();
            var descendingLookup = descendingSet.GetSpanAlternateLookup<char>();

            AssertLookupMatchesSet(descendingSet, descendingLookup);

            static void AssertLookupMatchesSet(SortedSet<string> set, SortedSet<string>.SpanAlternateLookup<char> lookup)
            {
                // Inclusive
                var setView = set.GetViewBefore("6");
                var lookupView = lookup.GetViewBefore("6".AsSpan());

                Assert.Equal(setView.ToArray(), lookupView.ToArray());

                setView = set.GetViewBefore("6", true);
                lookupView = lookup.GetViewBefore("6".AsSpan(), true);

                Assert.Equal(setView.ToArray(), lookupView.ToArray());

                // Exclusive
                setView = set.GetViewBefore("6", false);
                lookupView = lookup.GetViewBefore("6".AsSpan(), false);

                Assert.Equal(setView.ToArray(), lookupView.ToArray());
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedSet_GetSpanAlternateLookup_GetViewBefore_SubsequentOutOfRangeCall_ThrowsArgumentOutOfRangeException(int setLength)
        {
            if (setLength >= 3)
            {
                SCG.IComparer<string> comparer = StringComparer.Ordinal;
                var set = new SortedSet<string>(comparer);
                for (int i = 0; i < setLength; i++)
                    set.Add(i.ToString());

                string firstElement = set.ElementAt(0);
                string middleElement = set.ElementAt(setLength / 2);
                string lastElement = set.ElementAt(setLength - 1);
                if ((comparer.Compare(firstElement, middleElement) < 0) && (comparer.Compare(middleElement, lastElement) < 0))
                {
                    // J2N: this was confirmed to match JDK behavior
                    SortedSet<string> view = set.GetView(firstElement, middleElement);
                    var lookup = view.GetSpanAlternateLookup<char>();
                    Assert.Throws<ArgumentOutOfRangeException>(() => lookup.GetViewBefore(lastElement.AsSpan()));
                    Assert.Throws<ArgumentOutOfRangeException>(() => lookup.GetViewBefore(lastElement.AsSpan(), inclusive: true));
                    Assert.Throws<ArgumentOutOfRangeException>(() => lookup.GetViewBefore(lastElement.AsSpan(), inclusive: false));
                    Assert.NotNull(lookup.GetViewBefore(middleElement.AsSpan(), inclusive: true));
                    Assert.NotNull(lookup.GetViewBefore(middleElement.AsSpan(), inclusive: false));
                }
            }
        }

        [Fact]
        public void SortedSet_GetSpanAlternateLookup_GetViewAfter_MatchesSet()
        {
            var set = new SortedSet<string>(StringComparer.Ordinal);
            for (int i = 0; i < 10; i++)
                set.Add(i.ToString());

            var lookup = set.GetSpanAlternateLookup<char>();

            AssertLookupMatchesSet(set, lookup);

            // Descending set/view
            var descendingSet = set.GetViewDescending();
            var descendingLookup = descendingSet.GetSpanAlternateLookup<char>();

            AssertLookupMatchesSet(descendingSet, descendingLookup);

            static void AssertLookupMatchesSet(SortedSet<string> set, SortedSet<string>.SpanAlternateLookup<char> lookup)
            {
                // Inclusive
                var setView = set.GetViewAfter("3");
                var lookupView = lookup.GetViewAfter("3".AsSpan());

                Assert.Equal(setView.ToArray(), lookupView.ToArray());

                setView = set.GetViewAfter("3", true);
                lookupView = lookup.GetViewAfter("3".AsSpan(), true);

                Assert.Equal(setView.ToArray(), lookupView.ToArray());

                // Exclusive
                setView = set.GetViewAfter("3", false);
                lookupView = lookup.GetViewAfter("3".AsSpan(), false);

                Assert.Equal(setView.ToArray(), lookupView.ToArray());
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedSet_GetSpanAlternateLookup_GetViewAfter_SubsequentOutOfRangeCall_ThrowsArgumentOutOfRangeException(int setLength)
        {
            if (setLength >= 3)
            {
                SCG.IComparer<string> comparer = StringComparer.Ordinal;
                var set = new SortedSet<string>(comparer);
                for (int i = 0; i < setLength; i++)
                    set.Add(i.ToString());

                string firstElement = set.ElementAt(0);
                string middleElement = set.ElementAt(setLength / 2);
                string lastElement = set.ElementAt(setLength - 1);
                if ((comparer.Compare(firstElement, middleElement) < 0) && (comparer.Compare(middleElement, lastElement) < 0))
                {
                    // J2N: this was confirmed to match JDK behavior
                    SortedSet<string> view = set.GetViewAfter(middleElement);
                    var lookup = view.GetSpanAlternateLookup<char>();
                    Assert.Throws<ArgumentOutOfRangeException>(() => lookup.GetViewAfter(firstElement.AsSpan()));
                    Assert.Throws<ArgumentOutOfRangeException>(() => lookup.GetViewAfter(firstElement.AsSpan(), inclusive: true));
                    Assert.Throws<ArgumentOutOfRangeException>(() => lookup.GetViewAfter(firstElement.AsSpan(), inclusive: false));
                    Assert.NotNull(lookup.GetViewAfter(middleElement.AsSpan(), inclusive: true));
                    Assert.NotNull(lookup.GetViewAfter(middleElement.AsSpan(), inclusive: false));
                }
            }
        }

        [Theory]
        [InlineData(true, true)]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(false, false)]
        public void SortedSet_GetSpanAlternateLookup_WorksOnView(
            bool fromInclusive,
            bool toInclusive)
        {
            var set = new SortedSet<string>(StringComparer.Ordinal);
            for (int i = 0; i < 10; i++)
                set.Add(i.ToString("D2"));

            // View: [02,07]
            var view = set.GetView("02", fromInclusive, "07", toInclusive);

            int minInclusive = fromInclusive ? 2 : 3;
            int maxInclusive = toInclusive ? 7 : 6;

            AssertSpanLookupMatchesView(view, minInclusive, maxInclusive);

            set.Clear();
            for (int i = 0; i < 10; i++)
                set.Add(i.ToString("D2"));

            AssertSpanLookupMatchesView(view.GetViewDescending(), minInclusive, maxInclusive);

            int actualLower = fromInclusive ? 1 : 2;
            int actualUpper = toInclusive ? 8 : 7;

            AssertSpanLookupRejectsOutOfRangeValues(
                view,
                actualLower,
                actualUpper,
                fromInclusive,
                toInclusive);
        }

        [Theory]
        [InlineData(true, true)]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(false, false)]
        public void SortedSet_GetSpanAlternateLookup_WorksOnNestedView(
            bool fromInclusive,
            bool toInclusive)
        {
            var set = new SortedSet<string>(StringComparer.Ordinal);
            for (int i = 0; i < 10; i++)
                set.Add(i.ToString("D2"));

            var view1 = set.GetView("02", fromInclusive, "08", toInclusive);
            var view2 = view1.GetView("03", fromInclusive, "06", toInclusive);

            int minInclusive = fromInclusive ? 3 : 4;
            int maxInclusive = toInclusive ? 6 : 5;

            AssertSpanLookupMatchesView(view2, minInclusive, maxInclusive);

            set.Clear();
            for (int i = 0; i < 10; i++)
                set.Add(i.ToString("D2"));

            AssertSpanLookupMatchesView(view2.GetViewDescending(), minInclusive, maxInclusive);

            int lowerReject = fromInclusive ? 2 : 3;
            int upperReject = toInclusive ? 7 : 6;

            AssertSpanLookupRejectsOutOfRangeValues(
                view2,
                lowerReject,
                upperReject,
                fromInclusive,
                toInclusive);
        }

        [Theory]
        [InlineData(true, true)]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(false, false)]
        public void SortedSet_GetSpanAlternateLookup_WorksOnDeeplyNestedViews(
            bool fromInclusive,
            bool toInclusive)
        {
            var set = new SortedSet<string>(StringComparer.Ordinal);
            for (int i = 0; i < 20; i++)
                set.Add(i.ToString("D2"));

            var v1 = set.GetView("01", fromInclusive, "18", toInclusive);
            var v2 = v1.GetView("03", fromInclusive, "15", toInclusive);
            var v3 = v2.GetView("05", fromInclusive, "10", toInclusive);

            int minInclusive = fromInclusive ? 5 : 6;
            int maxInclusive = toInclusive ? 10 : 9;

            AssertSpanLookupMatchesView(v3, minInclusive, maxInclusive);

            v3.Clear();
            for (int i = 0; i < 20; i++)
                set.Add(i.ToString("D2"));

            AssertSpanLookupMatchesView(v3.GetViewDescending(), minInclusive, maxInclusive);

            int lowerReject = fromInclusive ? 4 : 5;
            int upperReject = toInclusive ? 11 : 10;

            AssertSpanLookupRejectsOutOfRangeValues(
                v3,
                lowerReject,
                upperReject,
                fromInclusive,
                toInclusive);
        }
        private static void AssertSpanLookupMatchesRootSet(SortedSet<string> set)
        {
            var lookup = set.GetSpanAlternateLookup<char>();
            Assert.Same(set, lookup.Set);
            Assert.Same(lookup.Set, lookup.Set);

            // Add to the set and validate that the lookup reflects the changes
            Assert.True(set.Add("123"));
            Assert.True(lookup.Contains("123".AsSpan()));
            Assert.False(lookup.Add("123".AsSpan()));
            Assert.True(lookup.Remove("123".AsSpan()));
            Assert.False(set.Contains("123"));

            // Add via the lookup and validate that the set reflects the changes
            Assert.True(lookup.Add("123".AsSpan()));
            Assert.True(set.Contains("123"));
            lookup.TryGetValue("123".AsSpan(), out string value);
            Assert.Equal("123", value);
            Assert.False(lookup.Remove("321".AsSpan()));
            Assert.True(lookup.Remove("123".AsSpan()));

            // Ensure that case-sensitivity of the comparer is respected
            Assert.True(lookup.Add("a".AsSpan()));
            if (set.Comparer.Equals(StringComparer.Ordinal) ||
                set.Comparer.Equals(StringComparer.InvariantCulture) ||
                set.Comparer.Equals(StringComparer.CurrentCulture) ||
                set.Comparer.Equals(ReverseComparer<string>.Create(StringComparer.Ordinal)) ||
                set.Comparer.Equals(ReverseComparer<string>.Create(StringComparer.InvariantCulture)) ||
                set.Comparer.Equals(ReverseComparer<string>.Create(StringComparer.CurrentCulture)))
            {
                Assert.True(lookup.Add("A".AsSpan()));
                Assert.True(lookup.Remove("a".AsSpan()));
                Assert.False(lookup.Remove("a".AsSpan()));
                Assert.True(lookup.Remove("A".AsSpan()));
            }
            else
            {
                Assert.False(lookup.Add("A".AsSpan()));
                Assert.True(lookup.Remove("A".AsSpan()));
                Assert.False(lookup.Remove("a".AsSpan()));
                Assert.False(lookup.Remove("A".AsSpan()));
            }

            // Test the behavior of null vs "" in the set and lookup
            Assert.True(set.Add(null));
            Assert.True(set.Add(string.Empty));
            Assert.True(set.Contains(null));
            Assert.True(set.Contains(""));
            Assert.True(lookup.Contains("".AsSpan()));
            Assert.True(lookup.Remove("".AsSpan()));
            Assert.Equal(1, set.Count);
            Assert.False(lookup.Remove("".AsSpan()));
            Assert.True(set.Remove(null));
            Assert.Equal(0, set.Count);

            // Test adding multiple entries via the lookup
            for (int i = 0; i < 10; i++)
            {
                Assert.Equal(i, set.Count);
                Assert.True(lookup.Add(i.ToString().AsSpan()));
                Assert.False(lookup.Add(i.ToString().AsSpan()));
            }

            Assert.Equal(10, set.Count);

            // Test that the lookup and the set agree on what's in and not in
            for (int i = -1; i <= 10; i++)
            {
                Assert.Equal(set.TryGetValue(i.ToString(), out string dv), lookup.TryGetValue(i.ToString().AsSpan(), out string lv));
                Assert.Equal(dv, lv);
            }

            // Test removing multiple entries via the lookup
            for (int i = 9; i >= 0; i--)
            {
                Assert.True(lookup.Remove(i.ToString().AsSpan()));
                Assert.False(lookup.Remove(i.ToString().AsSpan()));
                Assert.Equal(i, set.Count);
            }


            // Add some sequential items again
            for (int i = 0; i < 5; i++)
            {
                Assert.Equal(i, set.Count);
                Assert.True(lookup.Add(i.ToString().AsSpan()));
            }

            // Test TryGetPredecessor, TryGetSuccessor,
            // TryGetFlor, TryGetCeiling
            for (int i = 0; i < 5; i++)
            {
                string item = i.ToString();
                Assert.Equal(set.TryGetPredecessor(item, out string predecessor),
                    lookup.TryGetPredecessor(item.AsSpan(), out string spanPredecessor));
                Assert.Equal(predecessor, spanPredecessor);

                Assert.Equal(set.TryGetSuccessor(item, out string successor),
                    lookup.TryGetSuccessor(item.AsSpan(), out string spanSuccessor));
                Assert.Equal(successor, spanSuccessor);

                Assert.Equal(set.TryGetFloor(item, out string floor),
                    lookup.TryGetFloor(item.AsSpan(), out string spanFloor));
                Assert.Equal(floor, spanFloor);

                Assert.Equal(set.TryGetCeiling(item, out string ceiling),
                    lookup.TryGetCeiling(item.AsSpan(), out string spanCeiling));
                Assert.Equal(ceiling, spanCeiling);
            }
        }
        private static void AssertSpanLookupMatchesView(SortedSet<string> set, int minInclusive, int maxInclusive)
        {
            var lookup = set.GetSpanAlternateLookup<char>();
            Assert.Same(set, lookup.Set);

            // in-range add/remove
            for (int i = minInclusive; i <= maxInclusive; i++)
            {
                string s = i.ToString("D2");

                //Assert.True(lookup.Add(s.AsSpan()));
                Assert.False(lookup.Add(s.AsSpan()));
                Assert.False(set.Add(s));

                Assert.True(lookup.Contains(s.AsSpan()));
                Assert.True(set.Contains(s));

                lookup.TryGetValue(s.AsSpan(), out string v1);
                Assert.Equal(s, v1);

                set.TryGetValue(s, out string v2);
                Assert.Equal(s, v2);
            }

            // predecessor / successor within range
            for (int i = minInclusive; i <= maxInclusive; i++)
            {
                string s = i.ToString("D2");

                Assert.Equal(
                    set.TryGetPredecessor(s, out var predecessor),
                    lookup.TryGetPredecessor(s.AsSpan(), out var spanPredecessor));
                Assert.Equal(predecessor, spanPredecessor);

                Assert.Equal(
                    set.TryGetSuccessor(s, out var successor),
                    lookup.TryGetSuccessor(s.AsSpan(), out var spanSuccessor));
                Assert.Equal(successor, spanSuccessor);

                Assert.Equal(
                    set.TryGetPredecessor(s, out var floor),
                    lookup.TryGetPredecessor(s.AsSpan(), out var spanFloor));
                Assert.Equal(floor, spanFloor);

                Assert.Equal(
                    set.TryGetSuccessor(s, out var ceiling),
                    lookup.TryGetSuccessor(s.AsSpan(), out var spanCeiling));
                Assert.Equal(ceiling, spanCeiling);
            }

            // in-range remove
            for (int i = maxInclusive; i >= minInclusive; i--)
            {
                string s = i.ToString("D2");
                Assert.True(lookup.Remove(s.AsSpan()));
                Assert.False(lookup.Remove(s.AsSpan()));

                Assert.False(set.Remove(s));
            }

            Assert.Equal(0, set.Count);
        }

        private static void AssertSpanLookupRejectsOutOfRangeValues(SortedSet<string> set, int below, int above, bool fromInclusive, bool toInclusive)
        {
            var lookup = set.GetSpanAlternateLookup<char>();

            string low = below.ToString("D2");
            string high = above.ToString("D2");

            Assert.Throws<ArgumentOutOfRangeException>(() => set.Add(low));
            Assert.Throws<ArgumentOutOfRangeException>(() => lookup.Add(low.AsSpan()));

            Assert.Throws<ArgumentOutOfRangeException>(() => set.Add(high));
            Assert.Throws<ArgumentOutOfRangeException>(() => lookup.Add(high.AsSpan()));

            Assert.Throws<ArgumentOutOfRangeException>(() => set.GetView(low, high));
            Assert.Throws<ArgumentOutOfRangeException>(() => lookup.GetView(low.AsSpan(), high.AsSpan()));
            Assert.Throws<ArgumentOutOfRangeException>(() => set.GetView(low, fromInclusive, high, toInclusive));
            Assert.Throws<ArgumentOutOfRangeException>(() => lookup.GetView(low.AsSpan(), fromInclusive, high.AsSpan(), toInclusive));
        }

        #endregion SpanAlternateLookup

        #region TryGetValue

        [Fact]
        public void SortedSet_Generic_TryGetValue_Contains()
        {
            T value = CreateT(1);
            SortedSet<T> set = new SortedSet<T> { value };
            T equalValue = CreateT(1);
            T actualValue;
            Assert.True(set.TryGetValue(equalValue, out actualValue));
            Assert.Equal(value, actualValue);
            if (!typeof(T).GetTypeInfo().IsValueType)
            {
#pragma warning disable xUnit2005 // Do not use identity check on value type
                Assert.Same((object)value, (object)actualValue);
#pragma warning restore xUnit2005 // Do not use identity check on value type
            }
        }

        [Fact]
        public void SortedSet_Generic_TryGetValue_Contains_OverwriteOutputParam()
        {
            T value = CreateT(1);
            SortedSet<T> set = new SortedSet<T> { value };
            T equalValue = CreateT(1);
            T actualValue = CreateT(2);
            Assert.True(set.TryGetValue(equalValue, out actualValue));
            Assert.Equal(value, actualValue);
            if (!typeof(T).GetTypeInfo().IsValueType)
            {
#pragma warning disable xUnit2005 // Do not use identity check on value type
                Assert.Same((object)value, (object)actualValue);
#pragma warning restore xUnit2005 // Do not use identity check on value type
            }
        }

        [Fact]
        public void SortedSet_Generic_TryGetValue_NotContains()
        {
            T value = CreateT(1);
            SortedSet<T> set = new SortedSet<T> { value };
            T equalValue = CreateT(2);
            T actualValue;
            Assert.False(set.TryGetValue(equalValue, out actualValue));
            Assert.Equal(default(T), actualValue);
        }

        [Fact]
        public void SortedSet_Generic_TryGetValue_NotContains_OverwriteOutputParam()
        {
            T value = CreateT(1);
            SortedSet<T> set = new SortedSet<T> { value };
            T equalValue = CreateT(2);
            T actualValue = equalValue;
            Assert.False(set.TryGetValue(equalValue, out actualValue));
            Assert.Equal(default(T), actualValue);
        }

        #endregion

        #region TryGetPredecessor

        private static bool TryGetPredecessorExpected(
            List<T> sorted,
            T value,
            SCG.IComparer<T> comparer,
            out T result)
        {
            result = default!;
            for (int i = sorted.Count - 1; i >= 0; i--)
            {
                if (comparer.Compare(sorted[i], value) < 0)
                {
                    result = sorted[i];
                    return true;
                }
            }
            return false;
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedSet_Generic_TryGetPredecessor(int setLength)
        {
            SortedSet<T> set = (SortedSet<T>)GenericISetFactory(setLength);
            var comparer = GetIComparer() ?? Comparer<T>.Default;

            List<T> expected = set.ToList();
            expected.Sort(comparer);

            foreach (T value in expected)
            {
                bool foundExpected = TryGetPredecessorExpected(expected, value, comparer, out T expectedValue);
                bool foundActual = set.TryGetPredecessor(value, out T actualValue);

                Assert.Equal(foundExpected, foundActual);
                if (foundExpected)
                    Assert.Equal(expectedValue, actualValue);
            }

            // Descending view
            SortedSet<T> desc = set.GetViewDescending();

            foreach (T value in expected)
            {
                bool foundExpected = TryGetSuccessorExpected(expected, value, comparer, out T expectedValue);
                bool foundActual = desc.TryGetPredecessor(value, out T actualValue);

                Assert.Equal(foundExpected, foundActual);
                if (foundExpected)
                    Assert.Equal(expectedValue, actualValue);
            }
        }

        #endregion TryGetPredecessor

        #region TryGetSuccessor

        private static bool TryGetSuccessorExpected(
            List<T> sorted,
            T value,
            SCG.IComparer<T> comparer,
            out T result)
        {
            result = default!;
            for (int i = 0; i < sorted.Count; i++)
            {
                if (comparer.Compare(sorted[i], value) > 0)
                {
                    result = sorted[i];
                    return true;
                }
            }
            return false;
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedSet_Generic_TryGetSuccessor(int setLength)
        {
            SortedSet<T> set = (SortedSet<T>)GenericISetFactory(setLength);
            var comparer = GetIComparer() ?? Comparer<T>.Default;

            List<T> expected = set.ToList();
            expected.Sort(comparer);

            foreach (T value in expected)
            {
                bool foundExpected = TryGetSuccessorExpected(expected, value, comparer, out T expectedValue);
                bool foundActual = set.TryGetSuccessor(value, out T actualValue);

                Assert.Equal(foundExpected, foundActual);
                if (foundExpected)
                    Assert.Equal(expectedValue, actualValue);
            }

            // Descending view
            SortedSet<T> desc = set.GetViewDescending();

            foreach (T value in expected)
            {
                bool foundExpected = TryGetPredecessorExpected(expected, value, comparer, out T expectedValue);
                bool foundActual = desc.TryGetSuccessor(value, out T actualValue);

                Assert.Equal(foundExpected, foundActual);
                if (foundExpected)
                    Assert.Equal(expectedValue, actualValue);
            }
        }

        #endregion TryGetSuccessor

        #region TryGetFloor

        private static bool TryGetFloorExpected(
            List<T> sorted,
            T value,
            SCG.IComparer<T> comparer,
            out T result)
        {
            result = default!;
            for (int i = sorted.Count - 1; i >= 0; i--)
            {
                if (comparer.Compare(sorted[i], value) <= 0)
                {
                    result = sorted[i];
                    return true;
                }
            }
            return false;
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedSet_Generic_TryGetFloor(int setLength)
        {
            SortedSet<T> set = (SortedSet<T>)GenericISetFactory(setLength);
            var comparer = GetIComparer() ?? Comparer<T>.Default;

            List<T> expected = set.ToList();
            expected.Sort(comparer);

            foreach (T value in expected)
            {
                bool foundExpected = TryGetFloorExpected(expected, value, comparer, out T expectedValue);
                bool foundActual = set.TryGetFloor(value, out T actualValue);

                Assert.Equal(foundExpected, foundActual);
                if (foundExpected)
                    Assert.Equal(expectedValue, actualValue);
            }

            SortedSet<T> desc = set.GetViewDescending();

            foreach (T value in expected)
            {
                bool foundExpected = TryGetCeilingExpected(expected, value, comparer, out T expectedValue);
                bool foundActual = desc.TryGetFloor(value, out T actualValue);

                Assert.Equal(foundExpected, foundActual);
                if (foundExpected)
                    Assert.Equal(expectedValue, actualValue);
            }
        }

        #endregion TryGetFloor

        #region TryGetCeiling

        private static bool TryGetCeilingExpected(
            List<T> sorted,
            T value,
            SCG.IComparer<T> comparer,
            out T result)
        {
            result = default!;
            for (int i = 0; i < sorted.Count; i++)
            {
                if (comparer.Compare(sorted[i], value) >= 0)
                {
                    result = sorted[i];
                    return true;
                }
            }
            return false;
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedSet_Generic_TryGetCeiling(int setLength)
        {
            SortedSet<T> set = (SortedSet<T>)GenericISetFactory(setLength);
            var comparer = GetIComparer() ?? Comparer<T>.Default;

            List<T> expected = set.ToList();
            expected.Sort(comparer);

            foreach (T value in expected)
            {
                bool foundExpected = TryGetCeilingExpected(expected, value, comparer, out T expectedValue);
                bool foundActual = set.TryGetCeiling(value, out T actualValue);

                Assert.Equal(foundExpected, foundActual);
                if (foundExpected)
                    Assert.Equal(expectedValue, actualValue);
            }

            SortedSet<T> desc = set.GetViewDescending();

            foreach (T value in expected)
            {
                bool foundExpected = TryGetFloorExpected(expected, value, comparer, out T expectedValue);
                bool foundActual = desc.TryGetCeiling(value, out T actualValue);

                Assert.Equal(foundExpected, foundActual);
                if (foundExpected)
                    Assert.Equal(expectedValue, actualValue);
            }
        }

        #endregion TryGetCeiling

    }
}
