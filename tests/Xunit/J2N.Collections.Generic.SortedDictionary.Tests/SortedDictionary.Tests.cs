// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using J2N.Collections.Generic;
using J2N.TestUtilities.Xunit;
using System;
using System.Collections;
using Xunit;
using SCG = System.Collections.Generic;

namespace J2N.Collections.Tests
{
    public class SortedDictionary_IDictionary_NonGeneric_Tests : IDictionary_NonGeneric_Tests
    {
        #region IDictionary Helper Methods
        protected override bool Enumerator_Empty_UsesSingletonInstance => true;
        protected override bool Enumerator_Empty_ModifiedDuringEnumeration_ThrowsInvalidOperationException => false;

        protected override bool NullAllowed => true; // J2N allows null keys

        protected override IDictionary NonGenericIDictionaryFactory()
        {
            return new SortedDictionary<string, string>();
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
        protected override object CreateTValue(int seed)
        {
            return CreateTKey(seed);
        }

        #endregion

        #region IDictionary tests

        [Fact]
        public void IDictionary_NonGeneric_ItemSet_NullValueWhenDefaultValueIsNonNull()
        {
            IDictionary dictionary = new SortedDictionary<string, int>();
            Assert.Throws<ArgumentNullException>(() => dictionary[GetNewKey(dictionary)] = null);
        }

        [Fact]
        public void IDictionary_NonGeneric_ItemSet_KeyOfWrongType()
        {
            if (!IsReadOnly)
            {
                IDictionary dictionary = new SortedDictionary<string, string>();
                AssertExtensions.Throws<ArgumentException>("key", () => dictionary[23] = CreateTValue(12345));
                Assert.Empty(dictionary);
            }
        }

        [Fact]
        public void IDictionary_NonGeneric_ItemSet_ValueOfWrongType()
        {
            if (!IsReadOnly)
            {
                IDictionary dictionary = new SortedDictionary<string, string>();
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
                IDictionary dictionary = new SortedDictionary<string, string>();
                object missingKey = 23;
                AssertExtensions.Throws<ArgumentException>("key", () => dictionary.Add(missingKey, CreateTValue(12345)));
                Assert.Empty(dictionary);
            }
        }

        [Fact]
        public void IDictionary_NonGeneric_Add_ValueOfWrongType()
        {
            if (!IsReadOnly)
            {
                IDictionary dictionary = new SortedDictionary<string, string>();
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
                IDictionary dictionary = new SortedDictionary<string, int>();
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
                IDictionary dictionary = new SortedDictionary<string, int>();
                Assert.False(dictionary.Contains(1));
            }
        }

        [Fact]
        public void CantAcceptDuplicateKeysFromSourceDictionary()
        {
            Dictionary<string, int> source = new Dictionary<string, int> { { "a", 1 }, { "A", 1 } };
            AssertExtensions.Throws<ArgumentException>(null, () => new SortedDictionary<string, int>(source, StringComparer.OrdinalIgnoreCase));
        }

        #endregion

        #region ICollection tests

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void ICollection_NonGeneric_CopyTo_ArrayOfIncorrectKeyValuePairType(int count)
        {
            ICollection collection = NonGenericICollectionFactory(count);
            SCG.KeyValuePair<string, int>[] array = new SCG.KeyValuePair<string, int>[count * 3 / 2];
            AssertExtensions.Throws<ArgumentException>("array", null, () => collection.CopyTo(array, 0));
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void ICollection_NonGeneric_CopyTo_ArrayOfCorrectKeyValuePairType(int count)
        {
            ICollection collection = NonGenericICollectionFactory(count);
            SCG.KeyValuePair<string, string>[] array = new SCG.KeyValuePair<string, string>[count];
            collection.CopyTo(array, 0);
            int i = 0;
            foreach (object obj in collection)
                Assert.Equal(array[i++], obj);
        }

        #endregion
    }

    public class SortedDictionary_Tests
    {
        [Fact]
        public void TryAdd_AddsItem()
        {
            var dictionary = new SortedDictionary<string, string>();
            Assert.True(dictionary.TryAdd("a", "b"));

            Assert.True(dictionary.ContainsKey("a"));
        }

        [Fact]
        public void TryAdd_ItemAlreadyExists_DoesNotAddItem()
        {
            var dictionary = new SortedDictionary<string, string>();
            dictionary.Add("a", "b");

            Assert.False(dictionary.TryAdd("a", "c"));
            Assert.True(dictionary.TryGetValue("a", out string value));
            Assert.Equal("b", value);
        }

        [Fact]
        public void SortedDictionary_Generic_GetViewBetween_FirstLast()
        {
            SortedDictionary<int, int> dictionary = new()
            {
                [1] = 100,
                [3] = 300,
                [5] = 500,
                [7] = 700,
                [9] = 900,
            };
            SortedDictionary<int, int> view = dictionary.GetViewBetween(4, 8);

            Assert.True(dictionary.ContainsKey(1));
            Assert.True(dictionary.ContainsKey(3));
            Assert.True(dictionary.ContainsKey(5));
            Assert.True(dictionary.ContainsKey(7));
            Assert.True(dictionary.ContainsKey(9));

            Assert.False(view.ContainsKey(1));
            Assert.False(view.ContainsKey(3));
            Assert.True(view.ContainsKey(5));
            Assert.True(view.ContainsKey(7));
            Assert.False(view.ContainsKey(9));

            Assert.Equal(1, dictionary.FirstKey);
            Assert.Equal(9, dictionary.LastKey);

            Assert.Equal(5, view.FirstKey);
            Assert.Equal(7, view.LastKey);

            Assert.True(dictionary.TryGetFirst(out int key, out int value));
            Assert.Equal(1, key);
            Assert.Equal(100, value);

            Assert.True(dictionary.TryGetLast(out key, out value));
            Assert.Equal(9, key);
            Assert.Equal(900, value);

            Assert.True(view.TryGetFirst(out key, out value));
            Assert.Equal(5, key);
            Assert.Equal(500, value);

            Assert.True(view.TryGetLast(out key, out value));
            Assert.Equal(7, key);
            Assert.Equal(700, value);
        }

        // J2N: Added First and Last properties to replace Min and Max
        [Fact]
        public void SortedDictionary_Generic_GetViewBetween_FirstLast_Exhaustive()
        {
            SortedDictionary<int, int> dictionary = new()
            {
                [7] = 700,
                [11] = 1100,
                [3] = 300,
                [1] = 100,
                [5] = 500,
                [9] = 900,
                [13] = 1300,
            };
            for (int i = 0; i < 14; i++)
            {
                for (int j = i; j < 14; j++)
                {
                    SortedDictionary<int, int> view = dictionary.GetViewBetween(i, j);

                    if (j < i || (j == i && i % 2 == 0))
                    {
                        Assert.Equal(default(int), view.FirstKey);
                        Assert.Equal(default(int), view.LastKey);

                        Assert.False(view.TryGetFirst(out _, out _));
                        Assert.False(view.TryGetLast(out _, out _));
                    }
                    else
                    {
                        Assert.Equal(i + ((i + 1) % 2), view.FirstKey);
                        Assert.Equal(j - ((j + 1) % 2), view.LastKey);

                        Assert.True(view.TryGetFirst(out int key, out int value));
                        Assert.Equal(i + ((i + 1) % 2), key);
                        Assert.Equal((i + ((i + 1) % 2)) * 100, value);
                        Assert.True(view.TryGetLast(out key, out value));
                        Assert.Equal(j - ((j + 1) % 2), key);
                        Assert.Equal((j - ((j + 1) % 2)) * 100, value);
                    }
                }
            }
        }
    }
}
