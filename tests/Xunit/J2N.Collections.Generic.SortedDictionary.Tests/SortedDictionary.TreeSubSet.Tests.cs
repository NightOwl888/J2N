using J2N.Collections.Generic;
using System;
using System.Linq;
using Xunit;
using SCG = System.Collections.Generic;

namespace J2N.Collections.Tests
{
    public class SortedDictionary_TreeSubset_GetViewBetween_int_int_Tests : SortedDictionary_TreeSubset_int_int_Tests
    {
        protected override bool LowerBoundInclusive => true;
        protected override bool UpperBoundInclusive => true;


        protected override SCG.IDictionary<int, int> GenericIDictionaryFactory()
        {
            OriginalDictionary = new SortedDictionary<int, int>();
            return OriginalDictionary.GetViewBetween(LowerBound, UpperBound);
        }
    }

    public class SortedDictionary_TreeSubset_GetViewBetween_string_string_Tests : SortedDictionary_TreeSubset_string_string_Tests
    {
        protected override bool LowerBoundInclusive => true;

        protected override bool UpperBoundInclusive => true;

        protected override SCG.IDictionary<string, string> GenericIDictionaryFactory()
        {
            OriginalDictionary = new SortedDictionary<string, string>();
            return OriginalDictionary.GetViewBetween(LowerBound, UpperBound);
        }
    }


    public class SortedDictionary_TreeSubset_GetViewBetween_Inclusive_Inclusive_int_int_Tests : SortedDictionary_TreeSubset_int_int_Tests
    {
        protected override bool LowerBoundInclusive => true;

        protected override bool UpperBoundInclusive => true;

        protected override SCG.IDictionary<int, int> GenericIDictionaryFactory()
        {
            OriginalDictionary = new SortedDictionary<int, int>();
            return OriginalDictionary.GetViewBetween(LowerBound, LowerBoundInclusive, UpperBound, UpperBoundInclusive);
        }
    }

    public class SortedDictionary_TreeSubset_GetViewBetween_Inclusive_Inclusive_string_string_Tests : SortedDictionary_TreeSubset_string_string_Tests
    {
        protected override bool LowerBoundInclusive => true;

        protected override bool UpperBoundInclusive => true;

        protected override SCG.IDictionary<string, string> GenericIDictionaryFactory()
        {
            OriginalDictionary = new SortedDictionary<string, string>();
            return OriginalDictionary.GetViewBetween(LowerBound, LowerBoundInclusive, UpperBound, UpperBoundInclusive);
        }
    }

    public class SortedDictionary_TreeSubset_GetViewBetween_Inclusive_Exclusive_int_int_Tests : SortedDictionary_TreeSubset_int_int_Tests
    {
        protected override bool LowerBoundInclusive => true;

        protected override bool UpperBoundInclusive => false;

        protected override SCG.IDictionary<int, int> GenericIDictionaryFactory()
        {
            OriginalDictionary = new SortedDictionary<int, int>();
            return OriginalDictionary.GetViewBetween(LowerBound, LowerBoundInclusive, UpperBound, UpperBoundInclusive);
        }
    }

    public class SortedDictionary_TreeSubset_GetViewBetween_Inclusive_Exclusive_string_string_Tests : SortedDictionary_TreeSubset_string_string_Tests
    {
        protected override bool LowerBoundInclusive => true;

        protected override bool UpperBoundInclusive => false;

        protected override SCG.IDictionary<string, string> GenericIDictionaryFactory()
        {
            OriginalDictionary = new SortedDictionary<string, string>();
            return OriginalDictionary.GetViewBetween(LowerBound, LowerBoundInclusive, UpperBound, UpperBoundInclusive);
        }
    }

    public class SortedDictionary_TreeSubset_GetViewBetween_Exclusive_Inclusive_int_int_Tests : SortedDictionary_TreeSubset_int_int_Tests
    {
        protected override bool LowerBoundInclusive => false;

        protected override bool UpperBoundInclusive => true;

        protected override SCG.IDictionary<int, int> GenericIDictionaryFactory()
        {
            OriginalDictionary = new SortedDictionary<int, int>();
            return OriginalDictionary.GetViewBetween(LowerBound, LowerBoundInclusive, UpperBound, UpperBoundInclusive);
        }
    }

    public class SortedDictionary_TreeSubset_GetViewBetween_Exclusive_Inclusive_string_string_Tests : SortedDictionary_TreeSubset_string_string_Tests
    {
        protected override bool LowerBoundInclusive => false;

        protected override bool UpperBoundInclusive => true;

        protected override SCG.IDictionary<string, string> GenericIDictionaryFactory()
        {
            OriginalDictionary = new SortedDictionary<string, string>();
            return OriginalDictionary.GetViewBetween(LowerBound, LowerBoundInclusive, UpperBound, UpperBoundInclusive);
        }
    }

    public class SortedDictionary_TreeSubset_GetViewBetween_Exclusive_Exclusive_int_int_Tests : SortedDictionary_TreeSubset_int_int_Tests
    {
        protected override bool LowerBoundInclusive => false;

        protected override bool UpperBoundInclusive => false;

        protected override SCG.IDictionary<int, int> GenericIDictionaryFactory()
        {
            OriginalDictionary = new SortedDictionary<int, int>();
            return OriginalDictionary.GetViewBetween(LowerBound, LowerBoundInclusive, UpperBound, UpperBoundInclusive);
        }
    }

    public class SortedDictionary_TreeSubset_GetViewBetween_Exclusive_Exclusive_string_string_Tests : SortedDictionary_TreeSubset_string_string_Tests
    {
        protected override bool LowerBoundInclusive => false;

        protected override bool UpperBoundInclusive => false;

        protected override SCG.IDictionary<string, string> GenericIDictionaryFactory()
        {
            OriginalDictionary = new SortedDictionary<string, string>();
            return OriginalDictionary.GetViewBetween(LowerBound, LowerBoundInclusive, UpperBound, UpperBoundInclusive);
        }
    }


    public class SortedDictionary_TreeSubset_GetViewBefore_int_int_Tests : SortedDictionary_TreeSubset_int_int_Tests
    {
        protected override bool LowerBoundInclusive => true;

        protected override bool UpperBoundInclusive => true;


        protected override SCG.IDictionary<int, int> GenericIDictionaryFactory()
        {
            OriginalDictionary = new SortedDictionary<int, int>();
            return OriginalDictionary.GetViewBefore(UpperBound);
        }
    }

    public class SortedDictionary_TreeSubset_GetViewBefore_string_string_Tests : SortedDictionary_TreeSubset_string_string_Tests
    {
        protected override bool LowerBoundInclusive => true;

        protected override bool UpperBoundInclusive => true;

        protected override SCG.IDictionary<string, string> GenericIDictionaryFactory()
        {
            OriginalDictionary = new SortedDictionary<string, string>();
            return OriginalDictionary.GetViewBefore(UpperBound);
        }
    }

    public class SortedDictionary_TreeSubset_GetViewBefore_Inclusive_int_int_Tests : SortedDictionary_TreeSubset_int_int_Tests
    {
        protected override bool LowerBoundInclusive => true;

        protected override bool UpperBoundInclusive => true;


        protected override SCG.IDictionary<int, int> GenericIDictionaryFactory()
        {
            OriginalDictionary = new SortedDictionary<int, int>();
            return OriginalDictionary.GetViewBefore(UpperBound, UpperBoundInclusive);
        }
    }

    public class SortedDictionary_TreeSubset_GetViewBefore_Inclusive_string_string_Tests : SortedDictionary_TreeSubset_string_string_Tests
    {
        protected override bool LowerBoundInclusive => true;

        protected override bool UpperBoundInclusive => true;

        protected override SCG.IDictionary<string, string> GenericIDictionaryFactory()
        {
            OriginalDictionary = new SortedDictionary<string, string>();
            return OriginalDictionary.GetViewBefore(UpperBound, UpperBoundInclusive);
        }
    }

    public class SortedDictionary_TreeSubset_GetViewBefore_Exclusive_int_int_Tests : SortedDictionary_TreeSubset_int_int_Tests
    {
        protected override bool LowerBoundInclusive => true;

        protected override bool UpperBoundInclusive => false;


        protected override SCG.IDictionary<int, int> GenericIDictionaryFactory()
        {
            OriginalDictionary = new SortedDictionary<int, int>();
            return OriginalDictionary.GetViewBefore(UpperBound, UpperBoundInclusive);
        }
    }

    public class SortedDictionary_TreeSubset_GetViewBefore_Exclusive_string_string_Tests : SortedDictionary_TreeSubset_string_string_Tests
    {
        protected override bool LowerBoundInclusive => true;

        protected override bool UpperBoundInclusive => false;

        protected override SCG.IDictionary<string, string> GenericIDictionaryFactory()
        {
            OriginalDictionary = new SortedDictionary<string, string>();
            return OriginalDictionary.GetViewBefore(UpperBound, UpperBoundInclusive);
        }
    }


    public class SortedDictionary_TreeSubset_GetViewAfter_int_int_Tests : SortedDictionary_TreeSubset_int_int_Tests
    {
        protected override bool LowerBoundInclusive => true;

        protected override bool UpperBoundInclusive => true;


        protected override SCG.IDictionary<int, int> GenericIDictionaryFactory()
        {
            OriginalDictionary = new SortedDictionary<int, int>();
            return OriginalDictionary.GetViewAfter(LowerBound);
        }
    }

    public class SortedDictionary_TreeSubset_GetViewAfter_string_string_Tests : SortedDictionary_TreeSubset_string_string_Tests
    {
        protected override bool LowerBoundInclusive => true;

        protected override bool UpperBoundInclusive => true;

        protected override SCG.IDictionary<string, string> GenericIDictionaryFactory()
        {
            OriginalDictionary = new SortedDictionary<string, string>();
            return OriginalDictionary.GetViewAfter(LowerBound);
        }
    }

    public class SortedDictionary_TreeSubset_GetViewAfter_Inclusive_int_int_Tests : SortedDictionary_TreeSubset_int_int_Tests
    {
        protected override bool LowerBoundInclusive => true;

        protected override bool UpperBoundInclusive => true;


        protected override SCG.IDictionary<int, int> GenericIDictionaryFactory()
        {
            OriginalDictionary = new SortedDictionary<int, int>();
            return OriginalDictionary.GetViewAfter(LowerBound, LowerBoundInclusive);
        }
    }

    public class SortedDictionary_TreeSubset_GetViewAfter_Inclusive_string_string_Tests : SortedDictionary_TreeSubset_string_string_Tests
    {
        protected override bool LowerBoundInclusive => true;

        protected override bool UpperBoundInclusive => true;

        protected override SCG.IDictionary<string, string> GenericIDictionaryFactory()
        {
            OriginalDictionary = new SortedDictionary<string, string>();
            return OriginalDictionary.GetViewAfter(LowerBound, LowerBoundInclusive);
        }
    }

    public class SortedDictionary_TreeSubset_GetViewAfter_Exclusive_int_int_Tests : SortedDictionary_TreeSubset_int_int_Tests
    {
        protected override bool LowerBoundInclusive => false;

        protected override bool UpperBoundInclusive => true;


        protected override SCG.IDictionary<int, int> GenericIDictionaryFactory()
        {
            OriginalDictionary = new SortedDictionary<int, int>();
            return OriginalDictionary.GetViewAfter(LowerBound, LowerBoundInclusive);
        }
    }

    public class SortedDictionary_TreeSubset_GetViewAfter_Exclusive_string_string_Tests : SortedDictionary_TreeSubset_string_string_Tests
    {
        protected override bool LowerBoundInclusive => false;

        protected override bool UpperBoundInclusive => true;

        protected override SCG.IDictionary<string, string> GenericIDictionaryFactory()
        {
            OriginalDictionary = new SortedDictionary<string, string>();
            return OriginalDictionary.GetViewAfter(LowerBound, LowerBoundInclusive);
        }
    }


    public abstract class SortedDictionary_TreeSubset_int_int_Tests : SortedDictionary_TreeSubset_Tests<int, int>
    {
        protected override int LowerBound => int.MinValue;

        protected override int UpperBound => int.MaxValue;

        protected override bool DefaultValueAllowed => true;

        protected override SCG.KeyValuePair<int, int> CreateT(int seed)
        {
            return new SCG.KeyValuePair<int, int>(CreateTKey(seed), CreateTKey(seed + 500));
        }

        protected override int CreateTKey(int seed)
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

        protected override int CreateTValue(int seed)
        {
            return CreateTKey(seed);
        }
    }

    public abstract class SortedDictionary_TreeSubset_string_string_Tests : SortedDictionary_TreeSubset_Tests<string, string>
    {
        protected override string LowerBound => 0.ToString().PadLeft(10);
        protected override string UpperBound => int.MaxValue.ToString().PadLeft(10);

        protected override bool CanAddDefaultValue => false;

        protected override bool DefaultValueAllowed => false;

        protected override SCG.KeyValuePair<string, string> CreateT(int seed)
        {
            return new SCG.KeyValuePair<string, string>(CreateTKey(seed), CreateTKey(seed + 500));
        }

        protected override string CreateTKey(int seed)
        {
            string candidate = seed.ToString().PadLeft(10);
            // J2N: don't allow the value to be the lower or upper bound
            if (candidate == LowerBound)
                return (seed + 1).ToString().PadLeft(10);
            else if (candidate == UpperBound)
                return (seed - 1).ToString().PadLeft(10);

            return candidate;
        }

        protected override string CreateTValue(int seed)
        {
            return CreateTKey(seed);
        }

        public override void ICollection_Generic_Remove_DefaultValueContainedInCollection(int count)
        {
            if (!IsReadOnly && !AddRemoveClear_ThrowsNotSupported && DefaultValueAllowed && !Enumerable.Contains(InvalidValues, default(SCG.KeyValuePair<string, string>)))
            {
                int seed = count * 21;
                SCG.ICollection<SCG.KeyValuePair<string, string>> collection = GenericICollectionFactory(count);
                Assert.False(collection.Remove(default(SCG.KeyValuePair<string, string>)));
            }
        }
    }


    public abstract class SortedDictionary_TreeSubset_Tests<TKey, TValue> : SortedDictionary_Generic_Tests<TKey, TValue>
    {
        protected abstract override bool LowerBoundInclusive { get; }
        protected abstract TKey LowerBound { get; }
        protected abstract override bool UpperBoundInclusive { get; }
        protected abstract TKey UpperBound { get; }
        protected virtual bool CanAddDefaultValue => true;

        protected override bool DefaultValueAllowed => true;

        protected SortedDictionary<TKey, TValue> OriginalDictionary { get; set; }

        protected abstract override SCG.IDictionary<TKey, TValue> GenericIDictionaryFactory();

        public override void ICollection_Generic_Add_DefaultValue(int count)
        {
            // Adding an item to a TreeSubset does nothing - it updates the parent.
            if (DefaultValueAllowed && !IsReadOnly && !AddRemoveClear_ThrowsNotSupported && CanAddDefaultValue)
            {
                SCG.ICollection<SCG.KeyValuePair<TKey, TValue>> collection = GenericICollectionFactory(count);
                collection.Add(default(SCG.KeyValuePair<TKey, TValue>));
                Assert.Equal(count + 1, collection.Count); // collection is also updated.
                Assert.Equal(count + 1, OriginalDictionary.Count);
            }
        }

        public override void ICollection_Generic_Contains_DefaultValueOnCollectionContainingDefaultValue(int count)
        {
            if (DefaultValueAllowed && !IsReadOnly && !AddRemoveClear_ThrowsNotSupported && CanAddDefaultValue)
            {
                SCG.ICollection<SCG.KeyValuePair<TKey, TValue>> collection = GenericICollectionFactory(count);
                collection.Add(default(SCG.KeyValuePair<TKey, TValue>));
                Assert.True(collection.Contains(default(SCG.KeyValuePair<TKey, TValue>)));
            }
        }

        public override void IDictionary_Generic_Add_DefaultKey_DefaultValue(int count)
        {
            if (CanAddDefaultValue)
            {
                base.IDictionary_Generic_Add_DefaultKey_DefaultValue(count);
            }
        }

        public override void IDictionary_Generic_Add_DefaultKey_NonDefaultValue(int count)
        {
            if (CanAddDefaultValue)
            {
                base.IDictionary_Generic_Add_DefaultKey_NonDefaultValue(count);
            }
        }

        public override void IDictionary_Generic_ContainsKey_DefaultKeyNotContainedInDictionary(int count)
        {
            if (CanAddDefaultValue)
            {
                base.IDictionary_Generic_ContainsKey_DefaultKeyNotContainedInDictionary(count);
            }
        }

        public override void IDictionary_Generic_ItemGet_DefaultKey(int count)
        {
            if (CanAddDefaultValue)
            {
                base.IDictionary_Generic_ItemGet_DefaultKey(count);
            }
        }

        public override void IDictionary_Generic_ItemSet_DefaultKey(int count)
        {
            if (CanAddDefaultValue)
            {
                base.IDictionary_Generic_ItemSet_DefaultKey(count);
            }
        }

        public override void IDictionary_Generic_RemoveKey_DefaultKeyNotContainedInDictionary(int count)
        {
            if (CanAddDefaultValue)
            {
                base.IDictionary_Generic_RemoveKey_DefaultKeyNotContainedInDictionary(count);
            }
        }

        public override void IDictionary_Generic_TryGetValue_DefaultKeyNotContainedInDictionary(int count)
        {
            if (CanAddDefaultValue)
            {
                base.IDictionary_Generic_TryGetValue_DefaultKeyNotContainedInDictionary(count);
            }
        }

        public override void SortedDictionary_Generic_RemoveKey_DefaultKeyNotContainedInDictionary(int count)
        {
            if (CanAddDefaultValue)
            {
                base.SortedDictionary_Generic_RemoveKey_DefaultKeyNotContainedInDictionary(count);
            }
        }


        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedDictionary_TreeSubSet_Add_FirstKey(int count)
        {
            SortedDictionary<TKey, TValue> dictionary = (SortedDictionary<TKey, TValue>)GenericIDictionaryFactory(count);
            TKey firstKey = LowerBound;

            if (dictionary.TryGetFirst(out TKey currentFirstKey, out _))
            {
                Assert.NotEqual(firstKey, currentFirstKey); // Sanity check - the collection should not contain first
            }

            if (LowerBoundInclusive)
            {
                dictionary.Add(firstKey, default(TValue));
                Assert.Equal(count + 1, dictionary.Count); // collection is also updated.
                Assert.Equal(count + 1, OriginalDictionary.Count);
                Assert.Equal(firstKey, dictionary.FirstKey);
            }
            else
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => dictionary.Add(firstKey, default(TValue)));
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedDictionary_TreeSubSet_Add_LastKey(int count)
        {
            SortedDictionary<TKey, TValue> dictionary = (SortedDictionary<TKey, TValue>)GenericIDictionaryFactory(count);
            TKey lastKey = UpperBound;

            if (dictionary.TryGetLast(out TKey currentLastKey, out _))
            {
                Assert.NotEqual(lastKey, currentLastKey); // Sanity check - the collection should not contain last
            }

            if (UpperBoundInclusive)
            {
                dictionary.Add(lastKey, default(TValue));
                Assert.Equal(count + 1, dictionary.Count); // collection is also updated.
                Assert.Equal(count + 1, OriginalDictionary.Count);
                Assert.Equal(lastKey, dictionary.LastKey);
            }
            else
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => dictionary.Add(lastKey, default(TValue)));
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedDictionary_TreeSubSet_ContainsKey_FirstKey(int count)
        {
            SortedDictionary<TKey, TValue> dictionary = (SortedDictionary<TKey, TValue>)GenericIDictionaryFactory(count);
            TKey firstKey = LowerBound;

            if (dictionary.TryGetFirst(out TKey currentFirstKey, out _))
            {
                Assert.NotEqual(firstKey, currentFirstKey); // Sanity check - the collection should not contain first
            }

            if (LowerBoundInclusive)
            {
                dictionary.Add(firstKey, default(TValue));
                Assert.Equal(count + 1, dictionary.Count); // collection is also updated.
                Assert.Equal(count + 1, OriginalDictionary.Count);
                Assert.Equal(firstKey, dictionary.FirstKey);
                Assert.True(dictionary.ContainsKey(firstKey));
            }
            else
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => dictionary.Add(firstKey, default(TValue)));
                Assert.NotEqual(firstKey, dictionary.FirstKey);
                Assert.False(dictionary.ContainsKey(firstKey));
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedDictionary_TreeSubSet_ContainsKey_LastKey(int count)
        {
            SortedDictionary<TKey, TValue> dictionary = (SortedDictionary<TKey, TValue>)GenericIDictionaryFactory(count);
            TKey lastKey = UpperBound;

            if (dictionary.TryGetLast(out TKey currentLastKey, out _))
            {
                Assert.NotEqual(lastKey, currentLastKey); // Sanity check - the collection should not contain last
            }

            if (UpperBoundInclusive)
            {
                dictionary.Add(lastKey, default(TValue));
                Assert.Equal(count + 1, dictionary.Count); // collection is also updated.
                Assert.Equal(count + 1, OriginalDictionary.Count);
                Assert.Equal(lastKey, dictionary.LastKey);
                Assert.True(dictionary.ContainsKey(lastKey));
            }
            else
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => dictionary.Add(lastKey, default(TValue)));
                Assert.NotEqual(lastKey, dictionary.LastKey);
                Assert.False(dictionary.ContainsKey(lastKey));
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedDictionary_TreeSubSet_Remove_FirstKey(int count)
        {
            SortedDictionary<TKey, TValue> dictionary = (SortedDictionary<TKey, TValue>)GenericIDictionaryFactory(count);
            TKey firstKey = LowerBound;

            if (dictionary.TryGetFirst(out TKey currentFirstKey, out _))
            {
                Assert.NotEqual(firstKey, currentFirstKey); // Sanity check - the collection should not contain first
            }

            if (LowerBoundInclusive)
            {
                dictionary.Add(firstKey, default(TValue));
                Assert.Equal(count + 1, dictionary.Count); // collection is also updated.
                Assert.Equal(count + 1, OriginalDictionary.Count);
                Assert.Equal(firstKey, dictionary.FirstKey);
                Assert.True(dictionary.Remove(firstKey));
            }
            else
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => dictionary.Add(firstKey, default(TValue)));
                Assert.False(dictionary.Remove(firstKey));
            }

            if (dictionary.TryGetFirst(out currentFirstKey, out _))
            {
                Assert.NotEqual(firstKey, currentFirstKey);
            }
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedDictionary_TreeSubSet_Remove_LastKey(int count)
        {
            SortedDictionary<TKey, TValue> dictionary = (SortedDictionary<TKey, TValue>)GenericIDictionaryFactory(count);
            TKey lastKey = UpperBound;

            if (dictionary.TryGetLast(out TKey currentLastKey, out _))
            {
                Assert.NotEqual(lastKey, currentLastKey); // Sanity check - the collection should not contain last
            }

            if (UpperBoundInclusive)
            {
                dictionary.Add(lastKey, default(TValue));
                Assert.Equal(count + 1, dictionary.Count); // collection is also updated.
                Assert.Equal(count + 1, OriginalDictionary.Count);
                Assert.Equal(lastKey, dictionary.LastKey);
                Assert.True(dictionary.Remove(lastKey));
            }
            else
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => dictionary.Add(lastKey, default(TValue)));
                Assert.False(dictionary.Remove(lastKey));
            }

            if (dictionary.TryGetLast(out currentLastKey, out _))
            {
                Assert.NotEqual(lastKey, currentLastKey);
            }
        }
    }
}
