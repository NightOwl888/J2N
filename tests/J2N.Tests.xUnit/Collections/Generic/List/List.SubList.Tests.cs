using J2N.Collections.Generic;
using System;
using Xunit;
using SCG = System.Collections.Generic;

namespace J2N.Collections.Tests
{
    public class List_SubList_Tests_int : List_SubList_Tests<int>
    {
        protected override bool Enumerator_Empty_Current_UndefinedOperation_Throws => false;
        protected override bool DefaultValueAllowed => true;

        protected override int CreateT(int seed)
        {
            Random rand = new Random(seed);
            return rand.Next();
        }
    }

    public class List_SubList_Tests_string : List_SubList_Tests<string>
    {
        protected override bool Enumerator_Empty_Current_UndefinedOperation_Throws => false;
        protected override string CreateT(int seed)
        {
            int stringLength = seed % 10 + 5;
            Random rand = new Random(seed);
            byte[] bytes = new byte[stringLength];
            rand.NextBytes(bytes);
            return Convert.ToBase64String(bytes);
        }
    }

    public class List_SubList_Tests_string_ReadOnly : List_SubList_Tests<string>
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
#if FEATURE_READONLYCOLLECTION_ENUMERATOR_EMPTY_CURRENT_UNDEFINEDOPERATION_DOESNOTTHROW
        protected override bool Enumerator_Empty_Current_UndefinedOperation_Throws => false;
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

        protected override SCG.IEnumerable<ModifyEnumerable> GetModifyEnumerables(ModifyOperation operations) => new List<ModifyEnumerable>();
    }

    public class List_SubList_Tests_int_ReadOnly : List_SubList_Tests<int>
    {
        protected override int CreateT(int seed)
        {
            Random rand = new Random(seed);
            return rand.Next();
        }

        // J2N: See the comment in the root Directory.Build.targets file
#if FEATURE_READONLYCOLLECTION_ENUMERATOR_EMPTY_CURRENT_UNDEFINEDOPERATION_DOESNOTTHROW
        protected override bool Enumerator_Empty_Current_UndefinedOperation_Throws => false;
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

        protected override SCG.IEnumerable<ModifyEnumerable> GetModifyEnumerables(ModifyOperation operations) => new List<ModifyEnumerable>().GetView(0, 0);
    }

    public abstract class List_SubList_Tests<T> : List_Generic_Tests<T>
    {
        protected virtual bool CanAddDefaultValue => true;

        private List<T> OriginalList { get; set; }

        protected override List<T> GenericListFactory()
        {
            OriginalList = new List<T>();
            return OriginalList.GetView(0, 0);
        }

        protected override List<T> GenericListFactory(int count)
        {
            SCG.IEnumerable<T> toCreateFrom = CreateEnumerable(EnumerableType.List, null, count, 0, 0);
            OriginalList = new List<T>(new [] { default(T), default(T), default(T), default(T), default(T), });
            OriginalList.AddRange(toCreateFrom);
            OriginalList.AddRange(new[] { default(T), default(T), default(T), default(T), default(T), });
            return OriginalList.GetView(5, OriginalList.Count - 10); // We put 5 empty elements on each bound, so the total is Count - 10
        }

        public override void ICollection_Generic_Add_DefaultValue(int count)
        {
            //base.ICollection_Generic_Add_DefaultValue(count);
            // Adding an item to a SubList does nothing - it updates the parent.
            if (DefaultValueAllowed && !IsReadOnly && !AddRemoveClear_ThrowsNotSupported && CanAddDefaultValue)
            {
                SCG.ICollection<T> collection = GenericICollectionFactory(count);
                collection.Add(default(T));
                Assert.Equal(count + 1, collection.Count); // collection is also updated.
                Assert.Equal(count + 1 + 10, OriginalList.Count); // Account for the extra 10 items in parent list
            }
        }
    }
}
