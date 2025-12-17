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

        protected override SCG.ISet<T> GenericISetFactory()
        {
            return new SortedSet<T>();
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

        #region GetViewBetween

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
                Assert.Equal(setLength, view.Count);
                Assert.True(set.SetEquals(view));
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedSet_Generic_GetViewBetween_MiddleOfSet(int setLength)
        {
            if (setLength >= 3)
            {
                SCG.IComparer<T> comparer = GetIComparer() ?? Comparer<T>.Default;
                SortedSet<T> set = (SortedSet<T>)GenericISetFactory(setLength);
                T firstElement = set.ElementAt(1);
                T lastElement = set.ElementAt(setLength - 2);

                SCG.List<T> expected = new SCG.List<T>(setLength - 2);
                foreach (T value in set)
                    if (comparer.Compare(value, firstElement) >= 0 && comparer.Compare(value, lastElement) <= 0)
                        expected.Add(value);

                SortedSet<T> view = set.GetViewBetween(firstElement, lastElement);
                Assert.Equal(expected.Count, view.Count);
                Assert.True(view.SetEquals(expected));
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
                    AssertExtensions.Throws<ArgumentException>("lowerValue", /*null,*/ () => set.GetViewBetween(lastElement, firstElement));
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
        public void SortedSet_Generic_GetViewBetween_Empty_MinMax(int setLength)
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
        }

        #endregion

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

        [Fact]
        public void SortedSet_Generic_TestSubSetEnumerator()
        {
            SortedSet<int> sortedSet = new SortedSet<int>();
            for (int i = 0; i < 10000; i++)
            {
                if (!sortedSet.Contains(i))
                    sortedSet.Add(i);
            }
            SortedSet<int> mySubSet = sortedSet.GetViewBetween(45, 90);

            Assert.Equal(46, mySubSet.Count); //"not all elements were encountered"

            SCG.IEnumerable<int> en = mySubSet.Reverse();
            Assert.True(mySubSet.SetEquals(en)); //"Expected to be the same set."
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
        }

        [Fact]
        public void SortedSet_GetSpanAlternateLookup_GetViewBetween_MatchesSet()
        {
            var set = new SortedSet<string>(StringComparer.Ordinal);
            for (int i = 0; i < 10; i++)
                set.Add(i.ToString());

            var lookup = set.GetSpanAlternateLookup<char>();

            // Inclusive
            var setView = set.GetViewBetween("3", "6");
            var lookupView = lookup.GetViewBetween("3".AsSpan(), "6".AsSpan());

            Assert.Equal(setView.ToArray(), lookupView.ToArray());

            // Exclusive
            setView = set.GetViewBetween("3", false, "6", false);
            lookupView = lookup.GetViewBetween("3".AsSpan(), false, "6".AsSpan(), false);

            Assert.Equal(setView.ToArray(), lookupView.ToArray());
        }

        [Fact]
        public void SortedSet_GetSpanAlternateLookup_WorksOnRootSet()
        {
            var set = new SortedSet<string>(StringComparer.Ordinal);
            AssertSpanLookupMatchesRootSet(set);
        }

        [Fact]
        public void SortedSet_GetSpanAlternateLookup_WorksOnView()
        {
            var set = new SortedSet<string>(StringComparer.Ordinal);
            for (int i = 0; i < 10; i++)
                set.Add(i.ToString("D2"));

            var view = set.GetViewBetween("02", "07");

            AssertSpanLookupMatchesView(view, 2, 7);
            AssertSpanLookupRejectsOutOfRangeValues(view, 1, 8);
        }


        [Fact]
        public void SortedSet_GetSpanAlternateLookup_WorksOnNestedView()
        {
            var set = new SortedSet<string>(StringComparer.Ordinal);
            for (int i = 0; i < 10; i++)
                set.Add(i.ToString("D2"));

            var view1 = set.GetViewBetween("02", "08");
            var view2 = view1.GetViewBetween("03", "06");

            AssertSpanLookupMatchesView(view2, 3, 6);
            AssertSpanLookupRejectsOutOfRangeValues(view2, 2, 7);
        }

        [Fact]
        public void SortedSet_GetSpanAlternateLookup_WorksOnDeeplyNestedViews()
        {
            var set = new SortedSet<string>(StringComparer.Ordinal);
            for (int i = 0; i < 20; i++)
                set.Add(i.ToString("D2"));

            var v1 = set.GetViewBetween("01", "18");
            var v2 = v1.GetViewBetween("03", "15");
            var v3 = v2.GetViewBetween("05", "10");

            AssertSpanLookupMatchesView(v3, 5, 10);
            AssertSpanLookupRejectsOutOfRangeValues(v3, 4, 11);
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
                set.Comparer.Equals(StringComparer.CurrentCulture))
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

            // Test TryGetPredecessor and TryGetSuccessor
            for (int i = 0; i < 5; i++)
            {
                string item = i.ToString();
                Assert.Equal(set.TryGetPredecessor(item, out string predecessor),
                    lookup.TryGetPredecessor(item.AsSpan(), out string spanPredecessor));
                Assert.Equal(predecessor, spanPredecessor);

                Assert.Equal(set.TryGetSuccessor(item, out string successor),
                    lookup.TryGetSuccessor(item.AsSpan(), out string spanSuccessor));
                Assert.Equal(successor, spanSuccessor);
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
                    set.TryGetPredecessor(s, out var p1),
                    lookup.TryGetPredecessor(s.AsSpan(), out var p2));
                Assert.Equal(p1, p2);

                Assert.Equal(
                    set.TryGetSuccessor(s, out var s1),
                    lookup.TryGetSuccessor(s.AsSpan(), out var s2));
                Assert.Equal(s1, s2);
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

        private static void AssertSpanLookupRejectsOutOfRangeValues(SortedSet<string> set, int below, int above)
        {
            var lookup = set.GetSpanAlternateLookup<char>();

            string low = below.ToString("D2");
            string high = above.ToString("D2");

            Assert.Throws<ArgumentOutOfRangeException>(() => set.Add(low));
            Assert.Throws<ArgumentOutOfRangeException>(() => lookup.Add(low.AsSpan()));

            Assert.Throws<ArgumentOutOfRangeException>(() => set.Add(high));
            Assert.Throws<ArgumentOutOfRangeException>(() => lookup.Add(high.AsSpan()));

            Assert.Throws<ArgumentOutOfRangeException>(() => set.GetViewBetween(low, high));
            Assert.Throws<ArgumentOutOfRangeException>(() => lookup.GetViewBetween(low.AsSpan(), high.AsSpan()));
            Assert.Throws<ArgumentOutOfRangeException>(() => set.GetViewBetween(low, false, high, false));
            Assert.Throws<ArgumentOutOfRangeException>(() => lookup.GetViewBetween(low.AsSpan(), false, high.AsSpan(), false));
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
    }
}
