﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using J2N.Collections.Generic;
using J2N.TestUtilities.Xunit;
using System;
using System.Collections;
using System.Linq;
using Xunit;
using SCG = System.Collections.Generic;

namespace J2N.Collections.Tests
{
    public class SortedSet_TreeSubset_Int_Tests : SortedSet_TreeSubset_Tests<int>
    {
        protected override int Min => int.MinValue;
        protected override int Max => int.MaxValue;

        protected override bool DefaultValueAllowed => true;

        protected override int CreateT(int seed)
        {
            Random rand = new Random(seed);
            return rand.Next();
        }
    }

    public class SortedSet_TreeSubset_String_Tests : SortedSet_TreeSubset_Tests<string>
    {
        protected override string Min => 0.ToString().PadLeft(10);
        protected override string Max => int.MaxValue.ToString().PadLeft(10);

        protected override bool CanAddDefaultValue => false;

        protected override string CreateT(int seed)
        {
            return seed.ToString().PadLeft(10);
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
        protected abstract T Min { get; }
        protected abstract T Max { get; }
        protected virtual bool CanAddDefaultValue => true;

        private SortedSet<T> OriginalSet { get; set; }

        protected override SCG.ISet<T> GenericISetFactory()
        {
            OriginalSet = new SortedSet<T>();
            return OriginalSet.GetViewBetween(Min, Max);
        }

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
    }
}
