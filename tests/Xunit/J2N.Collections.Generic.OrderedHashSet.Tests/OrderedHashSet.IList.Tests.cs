// Based on: https://github.com/dotnet/runtime/blob/v10.0.0-rc.2.25502.107/src/libraries/System.Collections/tests/Generic/OrderedDictionary/OrderedDictionary.IList.Tests.cs
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using J2N.Collections.Generic;
using J2N.TestUtilities.Xunit;
using System;
using System.Collections;
using System.Linq;
using Xunit;
using SCG = System.Collections.Generic;

namespace J2N.Collections.Tests
{
    public class OrderedHashSet_IList_Tests : IList_Generic_Tests<string>
    {
        protected override bool DefaultValueAllowed => true; // J2N: Allow nulls and defaults
        protected override bool DuplicateValuesAllowed => false;
        protected override bool DefaultValueWhenNotAllowed_Throws => false; // J2N: Normal behavior of IList<T>
        protected override bool Enumerator_Empty_UsesSingletonInstance => true;
        protected override bool Enumerator_Empty_Current_UndefinedOperation_Throws => true;
        protected override bool Enumerator_Empty_ModifiedDuringEnumeration_ThrowsInvalidOperationException => false;

        protected override string CreateT(int seed) =>
            CreateString(seed);

        protected override SCG.IList<string> GenericIListFactory() => new OrderedHashSet<string>();

        private string CreateString(int seed)
        {
            int stringLength = seed % 10 + 5;
            Random rand = new Random(seed);
            byte[] bytes1 = new byte[stringLength];
            rand.NextBytes(bytes1);
            return Convert.ToBase64String(bytes1);
        }

        [Fact]
        public void IList_Generic_IndexOfRequiresValueMatch()
        {
            var set = new OrderedHashSet<string>()
            {
                "a",
                "b",
                "c"
            };

            string value = set.GetAt(2);
            Assert.Equal(2, ((SCG.IList<string>)set).IndexOf(value));
            Assert.Equal(-1, ((SCG.IList<string>)set).IndexOf("d"));
        }
    }

    public class OrderedHashSet_IList_NonGeneric_Tests : IList_NonGeneric_Tests
    {
        protected override bool NullAllowed => true;
        protected override bool DuplicateValuesAllowed => false;
        protected override bool Enumerator_Empty_UsesSingletonInstance => true;
        protected override bool Enumerator_Empty_Current_UndefinedOperation_Throw => true;
        protected override bool Enumerator_Empty_ModifiedDuringEnumeration_ThrowsInvalidOperationException => false;
        protected override bool Enumerator_Current_UndefinedOperation_Throws => true; // J2N: Use IList behavior
        protected override bool SupportsSerialization => false;
        protected override Type ICollection_NonGeneric_CopyTo_ArrayOfEnumType_ThrowType => typeof(ArgumentException);
        protected override bool IList_Empty_CurrentAfterAdd_Throws => true;
        protected override bool IList_CurrentAfterAdd_Throws => false; // J2N: Same behavior as OrderedDictionary<TKey, TValue>
        protected override ModifyOperation ModifyEnumeratorThrows => ModifyOperation.Add | ModifyOperation.Insert | ModifyOperation.Remove | ModifyOperation.Clear;

        protected override object CreateT(int seed) =>
            CreateString(seed);

        protected override IList NonGenericIListFactory() => new OrderedHashSet<string>();

        private string CreateString(int seed)
        {
            int stringLength = seed % 10 + 5;
            Random rand = new Random(seed);
            byte[] bytes1 = new byte[stringLength];
            rand.NextBytes(bytes1);
            return Convert.ToBase64String(bytes1);
        }

        [Fact]
        public void Indexer_Set_WrongType_ThrowsException()
        {
            IList list = NonGenericIListFactory();
            list.Add("value");
            AssertExtensions.Throws<ArgumentException>("value", () => list[0] = 42);
        }

        [Fact]
        public void Add_WrongType_ThrowsException()
        {
            IList list = NonGenericIListFactory();
            list.Add("value");
            AssertExtensions.Throws<ArgumentException>("value", () => list.Add(42));
            Assert.Equal(1, list.Count);
        }

        [Fact]
        public void Insert_WrongType_ThrowsException()
        {
            IList list = NonGenericIListFactory();
            list.Insert(0, "value");
            AssertExtensions.Throws<ArgumentException>("value", () => list.Insert(0, 42));
            Assert.Equal(1, list.Count);
        }

        #region Ordering tests

        // Based on: https://github.com/dotnet/runtime/blob/v10.0.0-rc.2.25502.107/src/libraries/System.Collections/tests/Generic/OrderedDictionary/OrderedDictionary.Tests.cs#L47-L95

        [Fact]
        public void Ordering_AddInsertRemoveClear_ExpectedOrderResults()
        {
            OrderedHashSet<int> d = [];

            d.Add(1);
            d.Add(2);
            d.Add(3);
            Assert.Equal(new[] { 1, 2, 3 }, d);

            d.Remove(2);
            Assert.Equal(new[] { 1, 3 }, d);

            d.Add(4);
            Assert.Equal(new[] { 1, 3, 4 }, d);

            d.Insert(0, 5);
            Assert.Equal(new[] { 5, 1, 3, 4 }, d);

            d.RemoveAt(2);
            Assert.Equal(new[] { 5, 1, 4 }, d);

            d.Add(6);
            Assert.Equal(new[] { 5, 1, 4, 6 }, d);

            d.Clear();
            Assert.Empty(d);

            d.Add(7);
            d.Add(9);
            d.Add(8);
            Assert.Equal(new[] { 7, 9, 8 }, d);
        }

        #endregion

        #region IList tests

        // Based on: https://github.com/dotnet/runtime/blob/v10.0.0-rc.2.25502.107/src/libraries/System.Collections/tests/Generic/OrderedDictionary/OrderedDictionary.Tests.cs#L99-L220

        [Fact]
        public void IList_NonGeneric_ItemSet_NullValueWhenDefaultValueIsNonNull()
        {
            IList set = new OrderedHashSet<int>();
            Assert.Throws<ArgumentNullException>(() => set[0] = null);
        }

        [Fact]
        public void IList_NonGeneric_Add_NullValueWhenDefaultTIsNonNull()
        {
            if (!IsReadOnly)
            {
                IList set = new OrderedHashSet<int>();
                Assert.Throws<ArgumentNullException>(() => set.Add(null));
                Assert.Empty(set);
            }
        }

        [Fact]
        public void IList_NonGeneric_Contains_WrongType()
        {
            if (!IsReadOnly)
            {
                IList set = new OrderedHashSet<string>();
                Assert.False(set.Contains(1));
            }
        }

        [Fact]
        public void IList_NonGeneric_Remove_WrongType()
        {
            if (!IsReadOnly)
            {
                IList set = new OrderedHashSet<string>();
                set.Remove(1); // ignored
                Assert.Empty(set);
            }
        }

        #endregion

        #region ICollection tests

        // Based on: https://github.com/dotnet/runtime/blob/v10.0.0-rc.2.25502.107/src/libraries/System.Collections/tests/Generic/OrderedDictionary/OrderedDictionary.Tests.cs#L222-L247

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void ICollection_NonGeneric_CopyTo_ArrayOfIncorrectType(int count)
        {
            ICollection collection = NonGenericICollectionFactory(count);
            int[] array = new int[count * 3 / 2];
            AssertExtensions.Throws<ArgumentException>("array", null, () => collection.CopyTo(array, 0));
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void ICollection_NonGeneric_CopyTo_ArrayOfCorrectType(int count)
        {
            ICollection collection = NonGenericICollectionFactory(count);
            string[] array = new string[count];
            collection.CopyTo(array, 0);
            int i = 0;
            foreach (object obj in collection)
            {
                Assert.Equal(array[i++], obj);
            }
        }

        #endregion
    }
}
