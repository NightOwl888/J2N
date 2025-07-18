﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections;
using System.Collections.Generic;
using J2N.TestUtilities.Xunit;
using Xunit;
using JCG = J2N.Collections.Generic;

namespace J2N.Collections.Tests
{
    public class OrderedDictionary_IDictionary_NonGeneric_Tests : IDictionary_NonGeneric_Tests
    {
        #region IDictionary Helper Methods
        protected override bool Enumerator_Current_UndefinedOperation_Throws => false;
        protected override bool Enumerator_Empty_Current_UndefinedOperation_Throw => true;
        protected override bool Enumerator_Empty_UsesSingletonInstance => true;
        protected override bool Enumerator_Empty_ModifiedDuringEnumeration_ThrowsInvalidOperationException => false;
        protected override ModifyOperation ModifyEnumeratorThrows => ModifyOperation.Add | ModifyOperation.Insert | ModifyOperation.Remove | ModifyOperation.Clear;
        protected override bool SupportsSerialization => false;
        protected override bool NullAllowed => true;

        protected override IDictionary NonGenericIDictionaryFactory()
        {
            return new JCG.OrderedDictionary<string, string>();
        }

        /// <summary>
        /// Creates an object that is dependent on the seed given. The object may be either
        /// a value type or a reference type, chosen based on the value of the seed.
        /// </summary>
        protected override object CreateTKey(int seed)
        {
            int stringLength = seed % 10 + 5;
            Random rand = new Random(seed);
            byte[] bytes = new byte[stringLength];
            rand.NextBytes(bytes);
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// Creates an object that is dependent on the seed given. The object may be either
        /// a value type or a reference type, chosen based on the value of the seed.
        /// </summary>
        protected override object CreateTValue(int seed) => CreateTKey(seed);

        #endregion

        #region Ordering tests

        [Fact]
        public void Ordering_AddInsertRemoveClear_ExpectedOrderResults()
        {
            JCG.OrderedDictionary<int, int> d = new();

            d.Add(1, 1);
            d.Add(2, 2);
            d.Add(3, 3);
            Assert.Equal(new[] { 1, 2, 3 }, d.Keys);
            Assert.Equal(new[] { 1, 2, 3 }, d.Values);
            Assert.Equal(new[] { KeyValuePairHelpers.Create(1, 1), KeyValuePairHelpers.Create(2, 2), KeyValuePairHelpers.Create(3, 3) }, d);

            d.Remove(2);
            Assert.Equal(new[] { 1, 3 }, d.Keys);
            Assert.Equal(new[] { 1, 3 }, d.Values);
            Assert.Equal(new[] { KeyValuePairHelpers.Create(1, 1), KeyValuePairHelpers.Create(3, 3) }, d);

            d.Add(4, 4);
            Assert.Equal(new[] { 1, 3, 4 }, d.Keys);
            Assert.Equal(new[] { 1, 3, 4 }, d.Values);
            Assert.Equal(new[] { KeyValuePairHelpers.Create(1, 1), KeyValuePairHelpers.Create(3, 3), KeyValuePairHelpers.Create(4, 4) }, d);

            d.Insert(0, 5, 5);
            Assert.Equal(new[] { 5, 1, 3, 4 }, d.Keys);
            Assert.Equal(new[] { 5, 1, 3, 4 }, d.Values);
            Assert.Equal(new[] { KeyValuePairHelpers.Create(5, 5), KeyValuePairHelpers.Create(1, 1), KeyValuePairHelpers.Create(3, 3), KeyValuePairHelpers.Create(4, 4) }, d);

            d.RemoveAt(2);
            Assert.Equal(new[] { 5, 1, 4 }, d.Keys);
            Assert.Equal(new[] { 5, 1, 4 }, d.Values);
            Assert.Equal(new[] { KeyValuePairHelpers.Create(5, 5), KeyValuePairHelpers.Create(1, 1), KeyValuePairHelpers.Create(4, 4) }, d);

            d.Add(6, 6);
            Assert.Equal(new[] { 5, 1, 4, 6 }, d.Keys);
            Assert.Equal(new[] { 5, 1, 4, 6 }, d.Values);
            Assert.Equal(new[] { KeyValuePairHelpers.Create(5, 5), KeyValuePairHelpers.Create(1, 1), KeyValuePairHelpers.Create(4, 4), KeyValuePairHelpers.Create(6, 6) }, d);

            d.Clear();
            Assert.Empty(d.Keys);
            Assert.Empty(d.Values);
            Assert.Empty(d);

            d.Add(7, 7);
            d.Add(9, 9);
            d.Add(8, 8);
            Assert.Equal(new[] { 7, 9, 8 }, d.Keys);
            Assert.Equal(new[] { 7, 9, 8 }, d.Values);
            Assert.Equal(new[] { KeyValuePairHelpers.Create(7, 7), KeyValuePairHelpers.Create(9, 9), KeyValuePairHelpers.Create(8, 8) }, d);
        }

        #endregion

        #region IDictionary tests

        [Fact]
        public void IDictionary_NonGeneric_ItemSet_NullValueWhenDefaultValueIsNonNull()
        {
            IDictionary dictionary = new JCG.OrderedDictionary<string, int>();
            Assert.Throws<ArgumentNullException>(() => dictionary[GetNewKey(dictionary)] = null);
        }

        [Fact]
        public void IDictionary_NonGeneric_ItemGet_KeyOfWrongType()
        {
            IDictionary dictionary = new JCG.OrderedDictionary<string, string>();
            dictionary.Add("key", "value");
            Assert.Null(dictionary[42]);
            Assert.Null(dictionary[KeyValuePairHelpers.Create("key", "value")]);
        }

        [Fact]
        public void IDictionary_NonGeneric_ItemSet_KeyOfWrongType()
        {
            if (!IsReadOnly)
            {
                IDictionary dictionary = new JCG.OrderedDictionary<string, string>();
                AssertExtensions.Throws<ArgumentException>("key", () => dictionary[23] = CreateTValue(12345));
                Assert.Empty(dictionary);
            }
        }

        [Fact]
        public void IDictionary_NonGeneric_ItemSet_ValueOfWrongType()
        {
            if (!IsReadOnly)
            {
                IDictionary dictionary = new JCG.OrderedDictionary<string, string>();
                object missingKey = GetNewKey(dictionary);
                AssertExtensions.Throws<ArgumentException>("value", () => dictionary[missingKey] = 324);
                Assert.Empty(dictionary);
            }
        }

        [Fact]
        public void IDictionary_NonGeneric_Add_KeyOfWrongType()
        {
            if (!IsReadOnly)
            {
                IDictionary dictionary = new JCG.OrderedDictionary<string, string>();
                object missingKey = 23;
                AssertExtensions.Throws<ArgumentException>("key", () => dictionary.Add(missingKey, CreateTValue(12345)));
                Assert.Empty(dictionary);

                dictionary = new JCG.OrderedDictionary<string, int>();
                AssertExtensions.Throws<ArgumentNullException>("value", () => dictionary.Add("key", null));
                Assert.Empty(dictionary);
            }
        }

        [Fact]
        public void IDictionary_NonGeneric_Add_ValueOfWrongType()
        {
            if (!IsReadOnly)
            {
                IDictionary dictionary = new JCG.OrderedDictionary<string, string>();
                object missingKey = GetNewKey(dictionary);
                AssertExtensions.Throws<ArgumentException>("value", () => dictionary.Add(missingKey, 324));
                Assert.Empty(dictionary);
            }
        }

        [Fact]
        public void IDictionary_NonGeneric_Add_NullValueWhenDefaultTValueIsNonNull()
        {
            if (!IsReadOnly)
            {
                IDictionary dictionary = new JCG.OrderedDictionary<string, int>();
                object missingKey = GetNewKey(dictionary);
                Assert.Throws<ArgumentNullException>(() => dictionary.Add(missingKey, null));
                Assert.Empty(dictionary);
            }
        }

        [Fact]
        public void IDictionary_NonGeneric_Contains_KeyOfWrongType()
        {
            if (!IsReadOnly)
            {
                IDictionary dictionary = new JCG.OrderedDictionary<string, int>();
                Assert.False(dictionary.Contains(1));
            }
        }

        [Fact]
        public void IDictionary_NonGeneric_Remove_KeyOfWrongType()
        {
            if (!IsReadOnly)
            {
                IDictionary dictionary = new JCG.OrderedDictionary<string, int>();
                dictionary.Remove(1); // ignored
                Assert.Empty(dictionary);
            }
        }

        [Fact]
        public void CantAcceptDuplicateKeysFromSourceDictionary()
        {
            Dictionary<string, int> source = new Dictionary<string, int> { { "a", 1 }, { "A", 1 } };
            // J2N: null key throws a different ArgumentException without "key" as the parameter name
            AssertExtensions.Throws<ArgumentException>(/*"key",*/ () => new JCG.OrderedDictionary<string, int>(source, StringComparer.OrdinalIgnoreCase));
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public override void IDictionary_NonGeneric_IDictionaryEnumerator_Current_AfterEndOfEnumerable_UndefinedBehavior(int count)
        {
            if (count != 0)
            {
                // Different undefined behavior for IDictionary.GetEnumerator when empty than for ICollection.GetEnumerator,
                // as the former doesn't use a singleton.
                base.IDictionary_NonGeneric_IDictionaryEnumerator_Current_AfterEndOfEnumerable_UndefinedBehavior(count);
            }
        }

        #endregion

        #region ICollection tests

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void ICollection_NonGeneric_CopyTo_ArrayOfIncorrectKeyValuePairType(int count)
        {
            ICollection collection = NonGenericICollectionFactory(count);
            KeyValuePair<string, int>[] array = new KeyValuePair<string, int>[count * 3 / 2];
            AssertExtensions.Throws<ArgumentException>("array", null, () => collection.CopyTo(array, 0));
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void ICollection_NonGeneric_CopyTo_ArrayOfCorrectKeyValuePairType(int count)
        {
            ICollection collection = NonGenericICollectionFactory(count);
            KeyValuePair<string, string>[] array = new KeyValuePair<string, string>[count];
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
