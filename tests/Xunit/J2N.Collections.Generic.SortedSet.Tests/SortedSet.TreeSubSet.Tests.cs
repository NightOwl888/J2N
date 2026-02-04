// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using J2N.Collections.Generic;
using J2N.TestUtilities.Xunit;
using System;
using System.Linq;
using Xunit;
using SCG = System.Collections.Generic;

namespace J2N.Collections.Tests
{
    public class SortedSet_TreeSubset_GetViewBetween_int_Tests : SortedSet_TreeSubset_int_Tests
    {
        protected override bool LowerBoundInclusive => true;

        protected override bool UpperBoundInclusive => true;


        protected override SCG.ISet<int> GenericISetFactory()
        {
            OriginalSet = new SortedSet<int>();
            return OriginalSet.GetViewBetween(LowerBound, UpperBound);
        }
    }

    public class SortedSet_TreeSubset_GetViewBetween_string_Tests : SortedSet_TreeSubset_string_Tests
    {
        protected override bool LowerBoundInclusive => true;

        protected override bool UpperBoundInclusive => true;

        protected override SCG.ISet<string> GenericISetFactory()
        {
            OriginalSet = new SortedSet<string>();
            return OriginalSet.GetViewBetween(LowerBound, UpperBound);
        }
    }

    public class SortedSet_TreeSubset_GetViewBetween_Inclusive_Inclusive_int_Tests : SortedSet_TreeSubset_int_Tests
    {
        protected override bool LowerBoundInclusive => true;

        protected override bool UpperBoundInclusive => true;


        protected override SCG.ISet<int> GenericISetFactory()
        {
            OriginalSet = new SortedSet<int>();
            return OriginalSet.GetViewBetween(LowerBound, LowerBoundInclusive, UpperBound, UpperBoundInclusive);
        }
    }

    public class SortedSet_TreeSubset_GetViewBetween_Inclusive_Inclusive_string_Tests : SortedSet_TreeSubset_string_Tests
    {
        protected override bool LowerBoundInclusive => true;

        protected override bool UpperBoundInclusive => true;

        protected override SCG.ISet<string> GenericISetFactory()
        {
            OriginalSet = new SortedSet<string>();
            return OriginalSet.GetViewBetween(LowerBound, LowerBoundInclusive, UpperBound, UpperBoundInclusive);
        }
    }

    public class SortedSet_TreeSubset_GetViewBetween_Inclusive_Exclusive_int_Tests : SortedSet_TreeSubset_int_Tests
    {
        protected override bool LowerBoundInclusive => true;

        protected override bool UpperBoundInclusive => false;


        protected override SCG.ISet<int> GenericISetFactory()
        {
            OriginalSet = new SortedSet<int>();
            return OriginalSet.GetViewBetween(LowerBound, LowerBoundInclusive, UpperBound, UpperBoundInclusive);
        }
    }

    public class SortedSet_TreeSubset_GetViewBetween_Inclusive_Exclusive_string_Tests : SortedSet_TreeSubset_string_Tests
    {
        protected override bool LowerBoundInclusive => true;

        protected override bool UpperBoundInclusive => false;

        protected override SCG.ISet<string> GenericISetFactory()
        {
            OriginalSet = new SortedSet<string>();
            return OriginalSet.GetViewBetween(LowerBound, LowerBoundInclusive, UpperBound, UpperBoundInclusive);
        }
    }

    public class SortedSet_TreeSubset_GetViewBetween_Exclusive_Inclusive_int_Tests : SortedSet_TreeSubset_int_Tests
    {
        protected override bool LowerBoundInclusive => false;

        protected override bool UpperBoundInclusive => true;


        protected override SCG.ISet<int> GenericISetFactory()
        {
            OriginalSet = new SortedSet<int>();
            return OriginalSet.GetViewBetween(LowerBound, LowerBoundInclusive, UpperBound, UpperBoundInclusive);
        }
    }

    public class SortedSet_TreeSubset_GetViewBetween_Exclusive_Inclusive_string_Tests : SortedSet_TreeSubset_string_Tests
    {
        protected override bool LowerBoundInclusive => false;

        protected override bool UpperBoundInclusive => true;

        protected override SCG.ISet<string> GenericISetFactory()
        {
            OriginalSet = new SortedSet<string>();
            return OriginalSet.GetViewBetween(LowerBound, LowerBoundInclusive, UpperBound, UpperBoundInclusive);
        }
    }

    public class SortedSet_TreeSubset_GetViewBetween_Exclusive_Exclusive_int_Tests : SortedSet_TreeSubset_int_Tests
    {
        protected override bool LowerBoundInclusive => false;

        protected override bool UpperBoundInclusive => false;


        protected override SCG.ISet<int> GenericISetFactory()
        {
            OriginalSet = new SortedSet<int>();
            return OriginalSet.GetViewBetween(LowerBound, LowerBoundInclusive, UpperBound, UpperBoundInclusive);
        }
    }

    public class SortedSet_TreeSubset_GetViewBetween_Exclusive_Exclusive_string_Tests : SortedSet_TreeSubset_string_Tests
    {
        protected override bool LowerBoundInclusive => false;

        protected override bool UpperBoundInclusive => false;

        protected override SCG.ISet<string> GenericISetFactory()
        {
            OriginalSet = new SortedSet<string>();
            return OriginalSet.GetViewBetween(LowerBound, LowerBoundInclusive, UpperBound, UpperBoundInclusive);
        }
    }


    public class SortedSet_TreeSubset_GetViewBefore_int_Tests : SortedSet_TreeSubset_int_Tests
    {
        protected override bool LowerBoundInclusive => true;

        protected override bool UpperBoundInclusive => true;


        protected override SCG.ISet<int> GenericISetFactory()
        {
            OriginalSet = new SortedSet<int>();
            return OriginalSet.GetViewBefore(UpperBound);
        }
    }

    public class SortedSet_TreeSubset_GetViewBefore_string_Tests : SortedSet_TreeSubset_string_Tests
    {
        protected override bool LowerBoundInclusive => true;

        protected override bool UpperBoundInclusive => true;

        protected override SCG.ISet<string> GenericISetFactory()
        {
            OriginalSet = new SortedSet<string>();
            return OriginalSet.GetViewBefore(UpperBound);
        }
    }

    public class SortedSet_TreeSubset_GetViewBefore_Inclusive_int_Tests : SortedSet_TreeSubset_int_Tests
    {
        protected override bool LowerBoundInclusive => true;

        protected override bool UpperBoundInclusive => true;


        protected override SCG.ISet<int> GenericISetFactory()
        {
            OriginalSet = new SortedSet<int>();
            return OriginalSet.GetViewBefore(UpperBound, UpperBoundInclusive);
        }
    }

    public class SortedSet_TreeSubset_GetViewBefore_Inclusive_string_Tests : SortedSet_TreeSubset_string_Tests
    {
        protected override bool LowerBoundInclusive => true;

        protected override bool UpperBoundInclusive => true;

        protected override SCG.ISet<string> GenericISetFactory()
        {
            OriginalSet = new SortedSet<string>();
            return OriginalSet.GetViewBefore(UpperBound, UpperBoundInclusive);
        }
    }

    public class SortedSet_TreeSubset_GetViewBefore_Exclusive_int_Tests : SortedSet_TreeSubset_int_Tests
    {
        protected override bool LowerBoundInclusive => true;

        protected override bool UpperBoundInclusive => false;


        protected override SCG.ISet<int> GenericISetFactory()
        {
            OriginalSet = new SortedSet<int>();
            return OriginalSet.GetViewBefore(UpperBound, UpperBoundInclusive);
        }
    }

    public class SortedSet_TreeSubset_GetViewBefore_Exclusive_string_Tests : SortedSet_TreeSubset_string_Tests
    {
        protected override bool LowerBoundInclusive => true;

        protected override bool UpperBoundInclusive => false;

        protected override SCG.ISet<string> GenericISetFactory()
        {
            OriginalSet = new SortedSet<string>();
            return OriginalSet.GetViewBefore(UpperBound, UpperBoundInclusive);
        }
    }

    public class SortedSet_TreeSubset_GetViewAfter_int_Tests : SortedSet_TreeSubset_int_Tests
    {
        protected override bool LowerBoundInclusive => true;

        protected override bool UpperBoundInclusive => true;


        protected override SCG.ISet<int> GenericISetFactory()
        {
            OriginalSet = new SortedSet<int>();
            return OriginalSet.GetViewAfter(LowerBound);
        }
    }

    public class SortedSet_TreeSubset_GetViewAfter_string_Tests : SortedSet_TreeSubset_string_Tests
    {
        protected override bool LowerBoundInclusive => true;

        protected override bool UpperBoundInclusive => true;

        protected override SCG.ISet<string> GenericISetFactory()
        {
            OriginalSet = new SortedSet<string>();
            return OriginalSet.GetViewAfter(LowerBound);
        }
    }

    public class SortedSet_TreeSubset_GetViewAfter_Inclusive_int_Tests : SortedSet_TreeSubset_int_Tests
    {
        protected override bool LowerBoundInclusive => true;

        protected override bool UpperBoundInclusive => true;


        protected override SCG.ISet<int> GenericISetFactory()
        {
            OriginalSet = new SortedSet<int>();
            return OriginalSet.GetViewAfter(LowerBound, LowerBoundInclusive);
        }
    }

    public class SortedSet_TreeSubset_GetViewAfter_Inclusive_string_Tests : SortedSet_TreeSubset_string_Tests
    {
        protected override bool LowerBoundInclusive => true;

        protected override bool UpperBoundInclusive => true;

        protected override SCG.ISet<string> GenericISetFactory()
        {
            OriginalSet = new SortedSet<string>();
            return OriginalSet.GetViewAfter(LowerBound, LowerBoundInclusive);
        }
    }

    public class SortedSet_TreeSubset_GetViewAfter_Exclusive_int_Tests : SortedSet_TreeSubset_int_Tests
    {
        protected override bool LowerBoundInclusive => false;

        protected override bool UpperBoundInclusive => true;


        protected override SCG.ISet<int> GenericISetFactory()
        {
            OriginalSet = new SortedSet<int>();
            return OriginalSet.GetViewAfter(LowerBound, LowerBoundInclusive);
        }
    }

    public class SortedSet_TreeSubset_GetViewAfter_Exclusive_string_Tests : SortedSet_TreeSubset_string_Tests
    {
        protected override bool LowerBoundInclusive => false;

        protected override bool UpperBoundInclusive => true;

        protected override SCG.ISet<string> GenericISetFactory()
        {
            OriginalSet = new SortedSet<string>();
            return OriginalSet.GetViewAfter(LowerBound, LowerBoundInclusive);
        }
    }


    public abstract class SortedSet_TreeSubset_int_Tests : SortedSet_TreeSubset_Tests<int>
    {
        protected override int LowerBound => int.MinValue;

        protected override int UpperBound => int.MaxValue;

        protected override bool DefaultValueAllowed => true;


        protected override int CreateT(int seed)
        {
            Random rand = new Random(seed);
            while (true)
            {
                int candidate = rand.Next();

                // J2N: don't allow the value to be the lower or upper bound
                if (candidate == LowerBound || candidate == UpperBound)
                {
                    continue;
                }
                return candidate;
            }
        }
    }

    public abstract class SortedSet_TreeSubset_string_Tests : SortedSet_TreeSubset_Tests<string>
    {
        protected override string LowerBound => 0.ToString().PadLeft(10);

        protected override string UpperBound => int.MaxValue.ToString().PadLeft(10);

        protected override bool CanAddDefaultValue => false;

        protected override bool DefaultValueAllowed => false;

        protected override string CreateT(int seed)
        {
            string candidate = seed.ToString().PadLeft(10);
            // J2N: don't allow the value to be the lower or upper bound
            if (candidate == LowerBound)
                return (seed + 1).ToString().PadLeft(10);
            else if (candidate == UpperBound)
                return (seed - 1).ToString().PadLeft(10);

            return candidate;
        }

        public override void ICollection_Generic_Remove_DefaultValueContainedInCollection(int count)
        {
            if (!IsReadOnly && !AddRemoveClear_ThrowsNotSupported && DefaultValueAllowed && !Enumerable.Contains(InvalidValues, default(string)))
            {
                int seed = count * 21;
                SCG.ICollection<string> collection = GenericICollectionFactory(count);
                Assert.False(collection.Remove(default(string)));
            }
        }

        public override void ICollection_Generic_Contains_DefaultValueOnCollectionContainingDefaultValue(int count)
        {
            if (DefaultValueAllowed && !IsReadOnly && !AddRemoveClear_ThrowsNotSupported)
            {
                SCG.ICollection<string> collection = GenericICollectionFactory(count);
                AssertExtensions.Throws<ArgumentOutOfRangeException>("item", /*null,*/ () => collection.Add(default(string)));
            }
        }
    }

    public abstract class SortedSet_TreeSubset_Tests<T> : SortedSet_Generic_Tests<T>
    {
        protected abstract override bool LowerBoundInclusive { get; }
        protected abstract T LowerBound { get; }
        protected abstract override bool UpperBoundInclusive { get; }
        protected abstract T UpperBound { get; }
        protected virtual bool CanAddDefaultValue => true;

        protected SortedSet<T> OriginalSet { get; set; }


        protected abstract override SCG.ISet<T> GenericISetFactory();

        public override void ICollection_Generic_Add_DefaultValue(int count)
        {
            // Adding an item to a TreeSubset does nothing - it updates the parent.
            if (DefaultValueAllowed && !IsReadOnly && !AddRemoveClear_ThrowsNotSupported && CanAddDefaultValue)
            {
                SCG.ICollection<T> collection = GenericICollectionFactory(count);
                collection.Add(default(T));
                Assert.Equal(count + 1, collection.Count); // collection is also updated.
                Assert.Equal(count + 1, OriginalSet.Count);
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedSet_TreeSubSet_Add_First(int count)
        {
            SortedSet<T> set = (SortedSet<T>)GenericISetFactory(count);
            T first = LowerBound;

            if (set.TryGetFirst(out T currentFirst))
            {
                Assert.NotEqual(first, currentFirst); // Sanity check - the collection should not contain first
            }

            Assert.NotEqual(first, set.First); // Sanity check - the collection should not contain first

            if (LowerBoundInclusive)
            {
                set.Add(first);
                Assert.Equal(count + 1, set.Count); // collection is also updated.
                Assert.Equal(count + 1, OriginalSet.Count);
                Assert.Equal(first, set.First);
            }
            else
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => set.Add(first));
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedSet_TreeSubSet_Add_Last(int count)
        {
            SortedSet<T> set = (SortedSet<T>)GenericISetFactory(count);
            T last = UpperBound;

            if (set.TryGetLast(out T currentLast))
            {
                Assert.NotEqual(last, currentLast); // Sanity check - the collection should not contain last
            }

            if (UpperBoundInclusive)
            {
                set.Add(last);
                Assert.Equal(count + 1, set.Count); // collection is also updated.
                Assert.Equal(count + 1, OriginalSet.Count);
                Assert.Equal(last, set.Last);
            }
            else
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => set.Add(last));
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedSet_TreeSubSet_Contains_First(int count)
        {
            SortedSet<T> set = (SortedSet<T>)GenericISetFactory(count);
            T first = LowerBound;

            if (set.TryGetFirst(out T currentFirst))
            {
                Assert.NotEqual(first, currentFirst); // Sanity check - the collection should not contain first
            }

            if (LowerBoundInclusive)
            {
                Assert.True(set.Add(first));
                Assert.Equal(count + 1, set.Count); // collection is also updated.
                Assert.Equal(count + 1, OriginalSet.Count);
                Assert.Equal(first, set.First);
                Assert.True(set.Contains(first));
            }
            else
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => set.Add(first));
                Assert.NotEqual(first, set.First);
                Assert.False(set.Contains(first));
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedSet_TreeSubSet_Contains_Last(int count)
        {
            SortedSet<T> set = (SortedSet<T>)GenericISetFactory(count);
            T last = UpperBound;

            if (set.TryGetLast(out T currentLast))
            {
                Assert.NotEqual(last, currentLast); // Sanity check - the collection should not contain last
            }

            if (UpperBoundInclusive)
            {
                Assert.True(set.Add(last));
                Assert.Equal(count + 1, set.Count); // collection is also updated.
                Assert.Equal(count + 1, OriginalSet.Count);
                Assert.Equal(last, set.Last);
                Assert.True(set.Contains(last));
            }
            else
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => set.Add(last));
                Assert.NotEqual(last, set.Last);
                Assert.False(set.Contains(last));
            }
        }


        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedSet_TreeSubSet_Remove_First(int count)
        {
            SortedSet<T> set = (SortedSet<T>)GenericISetFactory(count);
            T first = LowerBound;

            if (set.TryGetFirst(out T currentFirst))
            {
                Assert.NotEqual(first, currentFirst); // Sanity check - the collection should not contain first
            }

            if (LowerBoundInclusive)
            {
                Assert.True(set.Add(first));
                Assert.Equal(count + 1, set.Count); // collection is also updated.
                Assert.Equal(count + 1, OriginalSet.Count);
                Assert.Equal(first, set.First);
                Assert.True(set.Remove(first));
            }
            else
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => set.Add(first));
                Assert.False(set.Remove(first));
            }

            if (set.TryGetFirst(out currentFirst))
            {
                Assert.NotEqual(first, currentFirst);
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedSet_TreeSubSet_Remove_Last(int count)
        {
            SortedSet<T> set = (SortedSet<T>)GenericISetFactory(count);
            T last = UpperBound;

            if (set.TryGetLast(out T currentLast))
            {
                Assert.NotEqual(last, currentLast); // Sanity check - the collection should not contain last
            }

            if (UpperBoundInclusive)
            {
                Assert.True(set.Add(last));
                Assert.Equal(count + 1, set.Count); // collection is also updated.
                Assert.Equal(count + 1, OriginalSet.Count);
                Assert.Equal(last, set.Last);
                Assert.True(set.Remove(last));
            }
            else
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => set.Add(last));
                Assert.False(set.Remove(last));
            }

            if (set.TryGetLast(out currentLast))
            {
                Assert.NotEqual(last, currentLast);
            }
        }
    }
}
