// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using J2N.Collections.Generic;
using System;
using Xunit;
using SCG = System.Collections.Generic;

namespace J2N.Collections.Tests
{
    /// <summary>
    /// Contains tests that ensure the correctness of the List class.
    /// </summary>
    public abstract partial class List_Generic_Tests<T> : IList_Generic_Tests<T>
    {
        #region IList<T> Helper Methods

        protected override SCG.IList<T> GenericIListFactory()
        {
            return GenericListFactory();
        }

        protected override SCG.IList<T> GenericIListFactory(int count)
        {
            return GenericListFactory(count);
        }

        #endregion

        #region List<T> Helper Methods

        protected virtual List<T> GenericListFactory()
        {
            return new List<T>();
        }

        protected virtual List<T> GenericListFactory(int count)
        {
            SCG.IEnumerable<T> toCreateFrom = CreateEnumerable(EnumerableType.List, null, count, 0, 0);
            return new List<T>(toCreateFrom);
        }

        protected void VerifyList(List<T> list, List<T> expectedItems)
        {
            Assert.Equal(expectedItems.Count, list.Count);

            //Only verify the indexer. List should be in a good enough state that we
            //do not have to verify consistency with any other method.
            for (int i = 0; i < list.Count; ++i)
            {
                Assert.True(list[i] == null ? expectedItems[i] == null : list[i].Equals(expectedItems[i]));
            }
        }

        #endregion

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void CopyTo_ArgumentValidity(int count)
        {
            List<T> list = GenericListFactory(count);
            AssertExtensions.Throws<ArgumentException>(null, () => list.CopyTo(0, new T[0], 0, count + 1));
            AssertExtensions.Throws<ArgumentException>(null, () => list.CopyTo(count, new T[0], 0, 1));
        }
    }
}
