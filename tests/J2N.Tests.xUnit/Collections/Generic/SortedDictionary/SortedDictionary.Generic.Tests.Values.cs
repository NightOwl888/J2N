﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using J2N.Collections.Generic;
using System;
using System.Collections;
using System.Diagnostics;
using Xunit;
using SCG = System.Collections.Generic;

namespace J2N.Collections.Tests
{
    public class SortedDictionary_Generic_Tests_Values : ICollection_Generic_Tests<string>
    {
        protected override bool Enumerator_Empty_UsesSingletonInstance => true;
        protected override bool Enumerator_Empty_Current_UndefinedOperation_Throws => true;
        protected override bool DefaultValueAllowed => true;
        protected override bool DuplicateValuesAllowed => true;
        protected override bool IsReadOnly => true;

        protected override SCG.IEnumerable<ModifyEnumerable> GetModifyEnumerables(ModifyOperation operations) => new List<ModifyEnumerable>();

        protected override SCG.ICollection<string> GenericICollectionFactory()
        {
            return new SortedDictionary<string, string>().Values;
        }

        protected override SCG.ICollection<string> GenericICollectionFactory(int count)
        {
            SortedDictionary<string, string> list = new SortedDictionary<string, string>();
            int seed = 13453;
            for (int i = 0; i < count; i++)
                list.Add(CreateT(seed++), CreateT(seed++));
            return list.Values;
        }

        protected override string CreateT(int seed)
        {
            int stringLength = seed % 10 + 5;
            Random rand = new Random(seed);
            byte[] bytes = new byte[stringLength];
            rand.NextBytes(bytes);
            return Convert.ToBase64String(bytes);
        }

        [Fact]
        public void SortedDictionary_Generic_ValueCollection_Constructor_NullDictionary()
        {
            Assert.Throws<ArgumentNullException>(() => new SortedDictionary<string, string>.ValueCollection(null));
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedDictionary_Generic_ValueCollection_GetEnumerator(int count)
        {
            SortedDictionary<string, string> dictionary = new SortedDictionary<string, string>();
            int seed = 13453;
            while (dictionary.Count < count)
                dictionary.Add(CreateT(seed++), CreateT(seed++));
            dictionary.Values.GetEnumerator();
        }
    }

    public class SortedDictionary_Generic_Tests_Values_AsICollection : ICollection_NonGeneric_Tests
    {
        protected override bool NullAllowed => true;
        protected override bool DuplicateValuesAllowed => true;
        protected override bool IsReadOnly => true;
        protected override bool Enumerator_Empty_UsesSingletonInstance => true;
        protected override bool Enumerator_Current_UndefinedOperation_Throws => true;
        protected override SCG.IEnumerable<ModifyEnumerable> GetModifyEnumerables(ModifyOperation operations) => new List<ModifyEnumerable>();
        protected override bool SupportsSerialization => false;

        protected override ICollection NonGenericICollectionFactory()
        {
            return (ICollection)(new SortedDictionary<string, string>().Values);
        }

        protected override ICollection NonGenericICollectionFactory(int count)
        {
            SortedDictionary<string, string> list = new SortedDictionary<string, string>();
            int seed = 13453;
            for (int i = 0; i < count; i++)
                list.Add(CreateT(seed++), CreateT(seed++));
            return (ICollection)(list.Values);
        }

        private string CreateT(int seed)
        {
            int stringLength = seed % 10 + 5;
            Random rand = new Random(seed);
            byte[] bytes = new byte[stringLength];
            rand.NextBytes(bytes);
            return Convert.ToBase64String(bytes);
        }

        protected override void AddToCollection(ICollection collection, int numberOfItemsToAdd)
        {
            Debug.Assert(false);
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public override void ICollection_NonGeneric_CopyTo_ArrayOfIncorrectValueType(int count)
        {
            ICollection collection = NonGenericICollectionFactory(count);
            float[] array = new float[count * 3 / 2];
            Assert.Throws<InvalidCastException>(() => collection.CopyTo(array, 0));
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SortedDictionary_Generic_ValueCollection_CopyTo_ExactlyEnoughSpaceInTypeCorrectArray(int count)
        {
            ICollection collection = NonGenericICollectionFactory(count);
            string[] array = new string[count];
            collection.CopyTo(array, 0);
            int i = 0;
            foreach (object obj in collection)
                Assert.Equal(array[i++], obj);
        }
    }
}
