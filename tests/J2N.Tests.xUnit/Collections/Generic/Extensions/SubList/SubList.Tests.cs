// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using J2N.Collections.Generic;
using J2N.Collections.Generic.Extensions;
using System;
using Xunit;
using SCG = System.Collections.Generic;

namespace J2N.Collections.Tests
{
    public class SubList_Tests_int : SubList_Tests<int>
    {
        protected override bool DefaultValueAllowed => true;

        protected override int CreateT(int seed)
        {
            Random rand = new Random(seed);
            return rand.Next();
        }
    }

    public class SubList_Tests_string : SubList_Tests<string>
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

    public class SubList_Tests_string_ReadOnly : SubList_Tests<string>
    {
        protected override string CreateT(int seed)
        {
            int stringLength = seed % 10 + 5;
            Random rand = new Random(seed);
            byte[] bytes = new byte[stringLength];
            rand.NextBytes(bytes);
            return Convert.ToBase64String(bytes);
        }

        // J2N: See the comment in the root Directory.Build.targets file
#if !FEATURE_READONLYCOLLECTION_ENUMERATOR_EMPTY_CURRENT_UNDEFINEDOPERATION_DOESNOTTHROW
        protected override bool Enumerator_Empty_Current_UndefinedOperation_Throws => true;
#endif

        protected override bool IsReadOnly => true;

        protected override SCG.IList<string> GenericIListFactory(int setLength)
        {
            return GenericListFactory(setLength).AsReadOnly();
        }

        protected override SCG.IList<string> GenericIListFactory()
        {
            return GenericListFactory().AsReadOnly();
        }

        protected override SCG.IEnumerable<ModifyEnumerable> GetModifyEnumerables(ModifyOperation operations) => new SubList<ModifyEnumerable>(new List<ModifyEnumerable>(), 0, 0);
    }

    public class SubList_Tests_int_ReadOnly : SubList_Tests<int>
    {
        protected override int CreateT(int seed)
        {
            Random rand = new Random(seed);
            return rand.Next();
        }

        // J2N: See the comment in the root Directory.Build.targets file
#if !FEATURE_READONLYCOLLECTION_ENUMERATOR_EMPTY_CURRENT_UNDEFINEDOPERATION_DOESNOTTHROW
        protected override bool Enumerator_Empty_Current_UndefinedOperation_Throws => true;
#endif

        protected override bool IsReadOnly => true;

        protected override SCG.IList<int> GenericIListFactory(int setLength)
        {
            return GenericListFactory(setLength).AsReadOnly();
        }

        protected override SCG.IList<int> GenericIListFactory()
        {
            return GenericListFactory().AsReadOnly();
        }

        protected override SCG.IEnumerable<ModifyEnumerable> GetModifyEnumerables(ModifyOperation operations) => new List<ModifyEnumerable>();
    }

    public abstract class SubList_Tests<T> : IList_Generic_Tests<T>
    {
//        // J2N: See the comment in the root Directory.Build.targets file
//#if FEATURE_READONLYCOLLECTION_ENUMERATOR_EMPTY_CURRENT_UNDEFINEDOPERATION_DOESNOTTHROW
//        protected override bool Enumerator_Empty_Current_UndefinedOperation_Throws => false;
//#else
//        //protected override bool Enumerator_Empty_Current_UndefinedOperation_Throws => true;
//#endif

        private SCG.List<T> OriginalList { get; set; }

        #region IList<T> Helper Methods

        protected override SCG.IList<T> GenericIListFactory()
        {
            OriginalList = new SCG.List<T>();
            return new SubList<T>(OriginalList, 0, 0);
        }

        protected override SCG.IList<T> GenericIListFactory(int count)
        {
            return GenericListFactory(count);
        }

        #endregion

        #region SubList<T> Helper Methods

        internal virtual SubList<T> GenericListFactory()
        {
            OriginalList = new SCG.List<T>();
            return new SubList<T>(OriginalList, 0, 0);
        }

        internal virtual SubList<T> GenericListFactory(int count)
        {
            SCG.IEnumerable<T> toCreateFrom = CreateEnumerable(EnumerableType.List, null, count, 0, 0);
            OriginalList = new SCG.List<T>(new[] { default(T), default(T), default(T), default(T), default(T), });
            OriginalList.AddRange(toCreateFrom);
            OriginalList.AddRange(new[] { default(T), default(T), default(T), default(T), default(T), });
            return new SubList<T>(OriginalList, 5, OriginalList.Count - 10); // We put 5 empty elements on each bound, so the total is Count - 10
        }

        protected void VerifyList(SCG.IList<T> list, SCG.IList<T> expectedItems)
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
    }
}
