// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using J2N.Collections.Generic;
using System;
using SCG = System.Collections.Generic;
using Xunit;

namespace J2N.Collections.Tests
{
    public class SortedSet_Generic_Tests_string : SortedSet_Generic_Tests<string>
    {
        protected override string CreateT(int seed)
        {
            int stringLength = seed % 10 + 5;
            Random rand = new Random(seed);
            byte[] bytes = new byte[stringLength];
            rand.NextBytes(bytes);
            return Convert.ToBase64String(bytes);
        }
    }

    public class SortedSet_Generic_Tests_int : SortedSet_Generic_Tests<int>
    {
        protected override int CreateT(int seed)
        {
            Random rand = new Random(seed);
            return rand.Next();
        }

        protected override bool DefaultValueAllowed => true;

        [Fact]
        public void SortedSet_Generic_GetView_FirstLast()
        {
            var set = (SortedSet<int>)CreateSortedSet(new[] { 1, 3, 5, 7, 9 }, 5, 5);
            SortedSet<int> view = set.GetView(4, 8);

            Assert.True(set.Contains(1));
            Assert.True(set.Contains(3));
            Assert.True(set.Contains(5));
            Assert.True(set.Contains(7));
            Assert.True(set.Contains(9));

            Assert.False(view.Contains(1));
            Assert.False(view.Contains(3));
            Assert.True(view.Contains(5));
            Assert.True(view.Contains(7));
            Assert.False(view.Contains(9));

            Assert.True(set.TryGetFirst(out int value));
            Assert.Equal(1, value);

            Assert.True(set.TryGetLast(out value));
            Assert.Equal(9, value);

            Assert.True(view.TryGetFirst(out value));
            Assert.Equal(5, value);

            Assert.True(view.TryGetLast(out value));
            Assert.Equal(7, value);

            Assert.True(set.RemoveFirst(out value));
            Assert.Equal(1, value);
            Assert.True(set.TryGetFirst(out value));
            Assert.Equal(3, value);
            Assert.True(view.TryGetFirst(out value));
            Assert.Equal(5, value);

            Assert.True(view.RemoveFirst(out value));
            Assert.Equal(5, value);
            Assert.True(view.TryGetFirst(out value));
            Assert.Equal(7, value);

            Assert.True(set.RemoveLast(out value));
            Assert.Equal(9, value);
            Assert.True(set.TryGetLast(out value));
            Assert.Equal(7, value);
            Assert.True(view.TryGetLast(out value));
            Assert.Equal(7, value);

            Assert.True(view.RemoveLast(out value));
            Assert.Equal(7, value);
            Assert.Equal(0, view.Count);
            Assert.False(view.TryGetFirst(out value));
            Assert.Equal(0, value);
            Assert.False(view.TryGetLast(out value));
            Assert.Equal(0, value);
            Assert.False(view.RemoveFirst(out value));
            Assert.Equal(0, value);
            Assert.False(view.RemoveLast(out value));
            Assert.Equal(0, value);

            Assert.Equal(1, set.Count);
            Assert.True(set.Contains(3));
        }

        [Fact]
        public void SortedSet_Generic_IntersectWith_SupersetEnumerableWithDups()
        {
            var set = (SortedSet<int>)CreateSortedSet(new[] { 1, 3, 5, 7, 9 }, 5, 5);
            set.IntersectWith(new[] { 5, 7, 3, 7, 11, 7, 5, 2 });

            Assert.Equal(new[] { 3, 5, 7 }, set);
        }

        [Fact]
        public void SortedSet_Generic_GetView_MinMax_Exhaustive()
        {
            var set = (SortedSet<int>)CreateSortedSet(new[] { 7, 11, 3, 1, 5, 9, 13 }, 7, 7);
            for (int i = 0; i < 14; i++)
            {
                for (int j = i; j < 14; j++)
                {
                    SortedSet<int> view = set.GetView(i, j);

                    if (j < i || (j == i && i % 2 == 0))
                    {
                        Assert.Equal(default(int), view.Min);
                        Assert.Equal(default(int), view.Max);
                    }
                    else
                    {
                        Assert.Equal(i + ((i + 1) % 2), view.Min);
                        Assert.Equal(j - ((j + 1) % 2), view.Max);
                    }
                }
            }
        }

        // J2N: Added TryGetFirst and TryGetLast methods to replace Min and Max
        [Fact]
        public void SortedSet_Generic_GetView_FirstLast_Exhaustive()
        {
            var set = (SortedSet<int>)CreateSortedSet(new[] { 7, 11, 3, 1, 5, 9, 13 }, 7, 7);
            for (int i = 0; i < 14; i++)
            {
                for (int j = i; j < 14; j++)
                {
                    SortedSet<int> view = set.GetView(i, j);

                    if (j < i || (j == i && i % 2 == 0))
                    {
                        Assert.False(view.TryGetFirst(out _));
                        Assert.False(view.TryGetLast(out _));
                    }
                    else
                    {
                        Assert.True(view.TryGetFirst(out int value));
                        Assert.Equal(i + ((i + 1) % 2), value);

                        Assert.True(view.TryGetLast(out value));
                        Assert.Equal(j - ((j + 1) % 2), value);
                    }
                }
            }
        }
    }

    public class SortedSet_Generic_Tests_int_With_NullComparer : SortedSet_Generic_Tests_int
    {
        protected override SCG.IComparer<int> GetIComparer() => null;
    }

    //[OuterLoop]
    public class SortedSet_Generic_Tests_EquatableBackwardsOrder : SortedSet_Generic_Tests<EquatableBackwardsOrder>
    {
        protected override EquatableBackwardsOrder CreateT(int seed)
        {
            Random rand = new Random(seed);
            return new EquatableBackwardsOrder(rand.Next());
        }

        protected override SCG.ISet<EquatableBackwardsOrder> GenericISetFactory()
        {
            return new SortedSet<EquatableBackwardsOrder>();
        }
    }

    //[OuterLoop]
    public class SortedSet_Generic_Tests_int_With_Comparer_SameAsDefaultComparer : SortedSet_Generic_Tests<int>
    {
        protected override SCG.IEqualityComparer<int> GetIEqualityComparer()
        {
            return new Comparer_SameAsDefaultComparer();
        }

        protected override SCG.IComparer<int> GetIComparer()
        {
            return new Comparer_SameAsDefaultComparer();
        }

        protected override int CreateT(int seed)
        {
            Random rand = new Random(seed);
            return rand.Next();
        }

        protected override SCG.ISet<int> GenericISetFactory()
        {
            return new SortedSet<int>(new Comparer_SameAsDefaultComparer());
        }
    }

    //[OuterLoop]
    public class SortedSet_Generic_Tests_int_With_Comparer_HashCodeAlwaysReturnsZero : SortedSet_Generic_Tests<int>
    {
        protected override SCG.IEqualityComparer<int> GetIEqualityComparer()
        {
            return new Comparer_HashCodeAlwaysReturnsZero();
        }

        protected override SCG.IComparer<int> GetIComparer()
        {
            return new Comparer_HashCodeAlwaysReturnsZero();
        }

        protected override int CreateT(int seed)
        {
            Random rand = new Random(seed);
            return rand.Next();
        }

        protected override SCG.ISet<int> GenericISetFactory()
        {
            return new SortedSet<int>(new Comparer_HashCodeAlwaysReturnsZero());
        }
    }

    //[OuterLoop]
    public class SortedSet_Generic_Tests_int_With_Comparer_ModOfInt : SortedSet_Generic_Tests<int>
    {
        protected override SCG.IEqualityComparer<int> GetIEqualityComparer()
        {
            return new Comparer_ModOfInt(15000);
        }

        protected override SCG.IComparer<int> GetIComparer()
        {
            return new Comparer_ModOfInt(15000);
        }

        protected override int CreateT(int seed)
        {
            Random rand = new Random(seed);
            return rand.Next();
        }

        protected override SCG.ISet<int> GenericISetFactory()
        {
            return new SortedSet<int>(new Comparer_ModOfInt(15000));
        }
    }

    //[OuterLoop]
    public class SortedSet_Generic_Tests_int_With_Comparer_AbsOfInt : SortedSet_Generic_Tests<int>
    {
        protected override SCG.IEqualityComparer<int> GetIEqualityComparer()
        {
            return new Comparer_AbsOfInt();
        }

        protected override SCG.IComparer<int> GetIComparer()
        {
            return new Comparer_AbsOfInt();
        }

        protected override int CreateT(int seed)
        {
            Random rand = new Random(seed);
            return rand.Next();
        }

        protected override SCG.ISet<int> GenericISetFactory()
        {
            return new SortedSet<int>(new Comparer_AbsOfInt());
        }
    }


    public class SortedSet_Generic_Tests_AsGenericINavigableCollection_int : INavigableCollection_Generic_Tests<int>
    {
        protected override SCG.ICollection<int> GenericICollectionFactory()
        {
            return new SortedSet<int>();
        }

        protected override int CreateT(int seed)
        {
            Random rand = new Random(seed);
            return rand.Next();
        }
    }

    public class SortedSet_Generic_Tests_AsGenericINavigableCollection_string : INavigableCollection_Generic_Tests<string>
    {
        protected override SCG.ICollection<string> GenericICollectionFactory()
        {
            return new SortedSet<string>();
        }
        protected override string CreateT(int seed)
        {
            int stringLength = seed % 10 + 5;
            Random rand = new Random(seed);
            byte[] bytes = new byte[stringLength];
            rand.NextBytes(bytes);
            return Convert.ToBase64String(bytes);
        }
    }
}
