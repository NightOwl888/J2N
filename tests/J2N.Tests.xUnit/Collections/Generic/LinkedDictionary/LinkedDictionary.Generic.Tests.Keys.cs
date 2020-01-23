// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using J2N.Collections.Generic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Xunit;
using JCG = J2N.Collections.Generic;

namespace J2N.Collections.Tests
{
    public class LinkedDictionary_Generic_Tests_Keys : ICollection_Generic_Tests<string>
    {
        protected override bool DefaultValueAllowed => false;
        protected override bool DuplicateValuesAllowed => false;
        protected override bool IsReadOnly => true;
        protected override IEnumerable<ModifyEnumerable> GetModifyEnumerables(ModifyOperation operations) => new JCG.List<ModifyEnumerable>();
        protected override ICollection<string> GenericICollectionFactory()
        {
            return new LinkedDictionary<string, string>().Keys;
        }

        protected override ICollection<string> GenericICollectionFactory(int count)
        {
            LinkedDictionary<string, string> list = new LinkedDictionary<string, string>();
            int seed = 13453;
            for (int i = 0; i < count; i++)
                list.Add(CreateT(seed++), CreateT(seed++));
            return list.Keys;
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

        [Fact]
        public void LinkedDictionary_Generic_KeyCollection_Constructor_NullDictionary()
        {
            Assert.Throws<ArgumentNullException>(() => new LinkedDictionary<string, string>.KeyCollection(null));
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void LinkedDictionary_Generic_KeyCollection_GetEnumerator(int count)
        {
            LinkedDictionary<string, string> dictionary = new LinkedDictionary<string, string>();
            int seed = 13453;
            while (dictionary.Count < count)
                dictionary.Add(CreateT(seed++), CreateT(seed++));
            dictionary.Keys.GetEnumerator();
        }
    }

    public class LinkedDictionary_Generic_Tests_Keys_AsICollection : ICollection_NonGeneric_Tests
    {
        protected override bool NullAllowed => true;
        protected override bool DuplicateValuesAllowed => false;
        protected override bool IsReadOnly => true;
        protected override bool Enumerator_Current_UndefinedOperation_Throws => true;
        protected override Type ICollection_NonGeneric_CopyTo_ArrayOfEnumType_ThrowType => typeof(ArgumentException);
        protected override bool SupportsSerialization => false;

        protected override Type ICollection_NonGeneric_CopyTo_IndexLargerThanArrayCount_ThrowType => typeof(ArgumentOutOfRangeException);

        protected override ICollection NonGenericICollectionFactory()
        {
            return (ICollection)new LinkedDictionary<string, string>().Keys;
        }

        protected override ICollection NonGenericICollectionFactory(int count)
        {
            LinkedDictionary<string, string> list = new LinkedDictionary<string, string>();
            int seed = 13453;
            for (int i = 0; i < count; i++)
                list.Add(CreateT(seed++), CreateT(seed++));
            return (ICollection)list.Keys;
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

        protected override IEnumerable<ModifyEnumerable> GetModifyEnumerables(ModifyOperation operations) => new JCG.List<ModifyEnumerable>();

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void Dictionary_Generic_KeyCollection_CopyTo_ExactlyEnoughSpaceInTypeCorrectArray(int count)
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
