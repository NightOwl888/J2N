using J2N.Collections.ObjectModel;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
#nullable enable

namespace J2N.Collections.Generic.Extensions
{
    internal class TestCollectionExtensions
    {
        [Test]
        public void Test_AsReadOnly_ICollection_List_DispatchesToReadOnlyList()
        {
            ICollection<int> collection = new List<int> { 1, 2 };

            var result = collection.AsReadOnly();

            Assert.IsInstanceOf<ReadOnlyList<int>>(result);
        }

        [Test]
        public void Test_AsReadOnly_ICollection_Set_DispatchesToReadOnlySet()
        {
            ICollection<int> collection = new HashSet<int> { 1, 2 };

            var result = collection.AsReadOnly();

            Assert.IsInstanceOf<ReadOnlySet<int>>(result);
        }

        [Test]
        public void Test_AsReadOnly_ICollection_Dictionary_DispatchesToReadOnlyDictionary()
        {
            ICollection<KeyValuePair<int, string>> collection =
                new Dictionary<int, string>
                {
                    [1] = "a"
                };

            var result = collection.AsReadOnly();

            Assert.IsInstanceOf<ReadOnlyDictionary<int, string>>(result);
        }

        [Test]
        public void Test_AsReadOnly_Fallback_UsesReadOnlyCollection()
        {
            var list = new LinkedList<int>();
            list.AddLast(1);
            list.AddLast(2);

            ICollection<int> collection = list;

            var result = collection.AsReadOnly();

            Assert.IsInstanceOf<ReadOnlyCollection<int>>(result);
        }

        [Test]
        public void Test_AsReadOnly_ThrowsOnNull()
        {
            ICollection<int>? collection = null;

            Assert.Throws<ArgumentNullException>(() => collection!.AsReadOnly());
        }

        [Test]
        public void Test_AsReadOnly_KeyValuePair_ThrowsOnNull()
        {
            ICollection<KeyValuePair<int, int>>? collection = null;

            Assert.Throws<ArgumentNullException>(() => collection!.AsReadOnly());
        }

        [Test]
        public void Test_AsReadOnlyCollection_Throws_WhenDynamicCodeNotSupported()
        {
            var list = new LinkedList<int>();
            list.AddLast(1);
            list.AddLast(2);

            ICollection<int> collection = list;

            if (!RuntimeFeature.IsDynamicCodeSupported)
            {
                Assert.Throws<PlatformNotSupportedException>(
                    () => collection.AsReadOnly());
            }
        }

        [Test]
        public void Test_AsReadOnly_PrefersSetOverList_WhenBothImplemented()
        {
            ICollection<int> collection = new OrderedHashSet<int> { 1, 2 };

            var result = collection.AsReadOnly();

            Assert.IsInstanceOf<ReadOnlySet<int>>(result);
        }
    }
}
