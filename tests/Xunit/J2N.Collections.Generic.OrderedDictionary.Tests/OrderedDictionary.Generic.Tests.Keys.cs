﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Xunit;
using JCG = J2N.Collections.Generic;

namespace J2N.Collections.Tests
{
    public class OrderedDictionary_Generic_Tests_Keys : ICollection_Generic_Tests<string>
    {
        protected override bool Enumerator_Empty_UsesSingletonInstance => true;
        protected override bool Enumerator_Empty_Current_UndefinedOperation_Throws => true;
        protected override bool DefaultValueAllowed => true;
        protected override bool DuplicateValuesAllowed => false;
        protected override bool IsReadOnly => true;
        protected override IEnumerable<ModifyEnumerable> GetModifyEnumerables(ModifyOperation operations)=> new List<ModifyEnumerable>();
        protected override ICollection<string> GenericICollectionFactory() => new JCG.OrderedDictionary<string, string>().Keys;

        protected override ICollection<string> GenericICollectionFactory(int count)
        {
            JCG.OrderedDictionary<string, string> dictionary = new();
            int seed = 13453;
            for (int i = 0; i < count; i++)
            {
                dictionary.Add(CreateT(seed++), CreateT(seed++));
            }

            return dictionary.Keys;
        }

        protected override string CreateT(int seed)
        {
            int stringLength = seed % 10 + 5;
            Random rand = new Random(seed);
            byte[] bytes = new byte[stringLength];
            rand.NextBytes(bytes);
            return Convert.ToBase64String(bytes);
        }

        protected override Type ICollection_Generic_CopyTo_IndexLargerThanArrayCount_ThrowType => typeof(ArgumentOutOfRangeException);

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void OrderedDictionary_Generic_KeyCollection_GetEnumerator(int count)
        {
            JCG.OrderedDictionary<string, string> dictionary = new();
            int seed = 13453;
            while (dictionary.Count < count)
            {
                dictionary.Add(CreateT(seed++), CreateT(seed++));
            }

            dictionary.Keys.GetEnumerator();
        }
    }

    public class OrderedDictionary_Generic_Tests_Keys_AsICollection : ICollection_NonGeneric_Tests
    {
        protected override bool NullAllowed => true;
        protected override bool DuplicateValuesAllowed => false;
        protected override bool IsReadOnly => true;
        protected override bool Enumerator_Empty_UsesSingletonInstance => true;
        protected override bool Enumerator_Empty_Current_UndefinedOperation_Throw => true;
        protected override IEnumerable<ModifyEnumerable> GetModifyEnumerables(ModifyOperation operations) => new List<ModifyEnumerable>();
        protected override ICollection NonGenericICollectionFactory() => new JCG.OrderedDictionary<string, string>().Keys;
        protected override bool SupportsSerialization => false;
        protected override Type ICollection_NonGeneric_CopyTo_ArrayOfEnumType_ThrowType => typeof(ArgumentException);

        protected override Type ICollection_NonGeneric_CopyTo_IndexLargerThanArrayCount_ThrowType => typeof(ArgumentOutOfRangeException);

        protected override ICollection NonGenericICollectionFactory(int count)
        {
            JCG.OrderedDictionary<string, string> dictionary = new();
            int seed = 13453;
            for (int i = 0; i < count; i++)
            {
                dictionary.Add(CreateT(seed++), CreateT(seed++));
            }

            return dictionary.Keys;
        }

        private string CreateT(int seed)
        {
            int stringLength = seed % 10 + 5;
            Random rand = new Random(seed);
            byte[] bytes = new byte[stringLength];
            rand.NextBytes(bytes);
            return Convert.ToBase64String(bytes);
        }

        protected override void AddToCollection(ICollection collection, int numberOfItemsToAdd) => Debug.Fail("Read only");

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void OrderedDictionary_Generic_KeyCollection_CopyTo_ExactlyEnoughSpaceInTypeCorrectArray(int count)
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
    }
}
