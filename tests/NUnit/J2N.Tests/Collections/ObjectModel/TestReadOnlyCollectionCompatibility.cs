using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace J2N.Collections.ObjectModel
{
    internal class TestReadOnlyCollectionCompatibility
    {
        [Test]
        public void Test_ReadOnlyList_ICollectionBehavior()
        {
            var list = new List<int> { 1, 2, 3 };
            var wrapper = new ReadOnlyList<int>(list);

            AssertIsReadOnlyCollection(
                wrapper,
                list,
                existingItem: 1,
                missingItem: 99,
                newItem: 4);
        }

        [Test]
        public void Test_ReadOnlySet_ICollectionBehavior()
        {
            var set = new HashSet<int> { 1, 2, 3 };
            var wrapper = new ReadOnlySet<int>(set);

            AssertIsReadOnlyCollection(
                wrapper,
                set,
                existingItem: 1,
                missingItem: 99,
                newItem: 4);
        }

        [Test]
        public void Test_ReadOnlyDictionary_ICollectionBehavior()
        {
            var dict = new Dictionary<int, string>
            {
                [1] = "a",
                [2] = "b"
            };

            ICollection<KeyValuePair<int, string>> wrapper =
                new ReadOnlyDictionary<int, string>(dict);

            AssertIsReadOnlyCollection(
                wrapper,
                dict,
                existingItem: new KeyValuePair<int, string>(1, "a"),
                missingItem: new KeyValuePair<int, string>(99, "z"),
                newItem: new KeyValuePair<int, string>(3, "c"));
        }

        [Test]
        public void Test_ReadOnlyCollection_Fallback_ICollectionBehavior()
        {
            var list = new LinkedList<int>();
            list.AddLast(1);
            list.AddLast(2);
            list.AddLast(3);

            ICollection<int> collection = list;

            var wrapper = new ReadOnlyCollection<int>(collection);

            AssertIsReadOnlyCollection(
                wrapper,
                collection,
                existingItem: 1,
                missingItem: 99,
                newItem: 4);
        }


        public static void AssertIsReadOnlyCollection<T>(
           ICollection<T> collection,
           ICollection<T> underlying,
           T existingItem,
           T missingItem,
           T newItem)
        {
            // IsReadOnly
            Assert.IsTrue(collection.IsReadOnly);

            // Count mirrors underlying
            Assert.AreEqual(underlying.Count, collection.Count);

            // Contains
            Assert.IsTrue(collection.Contains(existingItem));
            Assert.IsFalse(collection.Contains(missingItem));

            // Enumeration
            CollectionAssert.AreEquivalent(underlying, collection);

            // CopyTo
            T[] array = new T[collection.Count];
            collection.CopyTo(array, 0);
            CollectionAssert.AreEquivalent(underlying, array);

            // Mutators throw
            Assert.Throws<NotSupportedException>(() => collection.Add(newItem));
            Assert.Throws<NotSupportedException>(() => collection.Remove(existingItem));
            Assert.Throws<NotSupportedException>(() => collection.Clear());

            // Underlying mutation is reflected
            underlying.Add(newItem);
            Assert.IsTrue(collection.Contains(newItem));
            Assert.AreEqual(underlying.Count, collection.Count);
        }
    }
}
