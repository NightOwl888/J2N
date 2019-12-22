using J2N.Util;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace J2N.Collections.Generic
{
    public class TestListEqualityComparer : TestCase
    {
        private IList<int> list;
        private int[] array;
        private IList<IList<IList<int>>> nestedList;
        private IList<IList<IList<int>>> nestedEqualList;
        private IList<IList<IList<int>>> nestedUnequalList;

        public override void SetUp()
        {
            base.SetUp();
            list = new List<int> { 1, 2, 3, 4, 5 };
            array = new int[] { 1, 2, 3, 4, 5 };

            nestedList = new List<IList<IList<int>>>()
            {
                new List<IList<int>>
                {
                    new List<int> { 9, 8, 7, 6 },
                    new List<int> { 9, 8, 7, 5 },
                    new List<int> { 9, 8, 7, 4 },
                },
                new List<IList<int>>
                {
                    new List<int> { 10, 8, 7, 6 },
                    new List<int> { 10, 8, 7, 5 },
                    new List<int> { 10, 8, 7, 4 },
                },
                new List<IList<int>>
                {
                    new List<int> { 0, 3, 5, 7, 9 },
                    new List<int> { 0, 3, 5, 7, 10 },
                    new List<int> { 0, 3, 5, 7, 11 },
                },
            };
            nestedEqualList = new List<IList<IList<int>>>()
            {
                new List<IList<int>>
                {
                    new List<int> { 9, 8, 7, 6 },
                    new List<int> { 9, 8, 7, 5 },
                    new List<int> { 9, 8, 7, 4 },
                },
                new List<IList<int>>
                {
                    new List<int> { 10, 8, 7, 6 },
                    new List<int> { 10, 8, 7, 5 },
                    new List<int> { 10, 8, 7, 4 },
                },
                new List<IList<int>>
                {
                    new List<int> { 0, 3, 5, 7, 9 },
                    new List<int> { 0, 3, 5, 7, 10 },
                    new List<int> { 0, 3, 5, 7, 11 },
                },
            };
            nestedUnequalList = new List<IList<IList<int>>>()
            {
                new List<IList<int>>
                {
                    new List<int> { 9, 8, 7, 6 },
                    new List<int> { 9, 8, 7, 5 },
                    new List<int> { 9, 8, 7, 4 },
                },
                new List<IList<int>>
                {
                    new List<int> { 10, 8, 7, 6 },
                    new List<int> { 10, 8, 7, 5 },
                    new List<int> { 10, 8, 7, 4 },
                },
                new List<IList<int>>
                {
                    new List<int> { 0, 3, 5, 7, 9 },
                    new List<int> { 0, 3, 555, 7, 10 },
                    new List<int> { 0, 3, 5, 7, 11 },
                },
            };
        }


        [Test]
        public void TestEquality_Default()
        {
            assertTrue(ListEqualityComparer<int>.Default.Equals(list, array)); // Types are compatible - array implements IList<T>
            assertEquals(ListEqualityComparer<int>.Default.GetHashCode(list), ListEqualityComparer<int>.Default.GetHashCode(list));

            assertFalse(ListEqualityComparer<IList<IList<int>>>.Default.Equals(nestedList, nestedEqualList));
            Assert.AreNotEqual(ListEqualityComparer<IList<IList<int>>>.Default.GetHashCode(nestedList), ListEqualityComparer<IList<IList<int>>>.Default.GetHashCode(nestedEqualList));

            assertFalse(ListEqualityComparer<IList<IList<int>>>.Default.Equals(nestedList, nestedUnequalList));
            Assert.AreNotEqual(ListEqualityComparer<IList<IList<int>>>.Default.GetHashCode(nestedList), ListEqualityComparer<IList<IList<int>>>.Default.GetHashCode(nestedUnequalList));
        }

        [Test]
        public void TestEquality_Aggressive()
        {
            assertTrue(ListEqualityComparer<int>.Aggressive.Equals(list, array)); // Types are compatible - array implements IList<T>
            assertEquals(ListEqualityComparer<int>.Aggressive.GetHashCode(list), ListEqualityComparer<int>.Aggressive.GetHashCode(list));

            assertTrue(ListEqualityComparer<IList<IList<int>>>.Aggressive.Equals(nestedList, nestedEqualList));
            Assert.AreEqual(ListEqualityComparer<IList<IList<int>>>.Aggressive.GetHashCode(nestedList), ListEqualityComparer<IList<IList<int>>>.Aggressive.GetHashCode(nestedEqualList));

            assertFalse(ListEqualityComparer<IList<IList<int>>>.Aggressive.Equals(nestedList, nestedUnequalList));
            Assert.AreNotEqual(ListEqualityComparer<IList<IList<int>>>.Aggressive.GetHashCode(nestedList), ListEqualityComparer<IList<IList<int>>>.Aggressive.GetHashCode(nestedUnequalList));
        }

        [Test]
        public void TestEqualityList_Aggressive()
        {
            var control = new List<IDictionary<string, string>>
                    {
                        new Dictionary<string, string> { { "1", "one" }, { "2", "two" }, { "3", "three" } },
                        new Dictionary<string, string> { { "4", "four" }, { "5", "five" }, { "6", "six" } },
                        new Dictionary<string, string> { { "7", "seven" }, { "8", "eight" }, { "9", "nine" } },
                    };
            var equal = new List<IDictionary<string, string>>
                    {
                        new Dictionary<string, string> { { "1", "one" }, { "2", "two" }, { "3", "three" } },
                        new Dictionary<string, string> { { "4", "four" }, { "5", "five" }, { "6", "six" } },
                        new Dictionary<string, string> { { "7", "seven" }, { "8", "eight" }, { "9", "nine" } },
                    };
            var equalDifferentType = new IDictionary<string, string>[]
            {
                        new Dictionary<string, string> { { "1", "one" }, { "2", "two" }, { "3", "three" } },
                        new Dictionary<string, string> { { "4", "four" }, { "5", "five" }, { "6", "six" } },
                        new Dictionary<string, string> { { "7", "seven" }, { "8", "eight" }, { "9", "nine" } },
            };
            var equalDifferentOrder = new List<IDictionary<string, string>>
                    {
                        new Dictionary<string, string> { { "7", "seven" }, { "8", "eight" }, { "9", "nine" } },
                        new Dictionary<string, string> { { "1", "one" }, { "2", "two" }, { "3", "three" } },
                        new Dictionary<string, string> { { "4", "four" }, { "5", "five" }, { "6", "six" } },
                    };
            var level1EqualLevel2Unequal = new List<IDictionary<string, string>>
                    {
                        new Dictionary<string, string> { { "1", "one" }, { "2", "two" }, { "3", "three" } },
                        new Dictionary<string, string> { { "4", "four" }, { "5", "five" }, { "6", "six" } },
                        new Dictionary<string, string> { { "7", "seven" }, { "8", "eight" }, { "9", "nine99" } },
                    };

            Assert.AreEqual(ListEqualityComparer<IDictionary<string, string>>.Aggressive.GetHashCode(control), ListEqualityComparer<IDictionary<string, string>>.Aggressive.GetHashCode(control));
            Assert.IsTrue(ListEqualityComparer<IDictionary<string, string>>.Aggressive.Equals(control, control));

            Assert.AreEqual(ListEqualityComparer<IDictionary<string, string>>.Aggressive.GetHashCode(control), ListEqualityComparer<IDictionary<string, string>>.Aggressive.GetHashCode(equal));
            Assert.IsTrue(ListEqualityComparer<IDictionary<string, string>>.Aggressive.Equals(control, equal));

            Assert.AreEqual(ListEqualityComparer<IDictionary<string, string>>.Aggressive.GetHashCode(control), ListEqualityComparer<IDictionary<string, string>>.Aggressive.GetHashCode(equalDifferentType));
            Assert.IsTrue(ListEqualityComparer<IDictionary<string, string>>.Aggressive.Equals(control, equalDifferentType));

            // Lists and arrays are order-sensitive
            Assert.AreNotEqual(ListEqualityComparer<IDictionary<string, string>>.Aggressive.GetHashCode(control), ListEqualityComparer<IDictionary<string, string>>.Aggressive.GetHashCode(equalDifferentOrder));
            Assert.IsFalse(ListEqualityComparer<IDictionary<string, string>>.Aggressive.Equals(control, equalDifferentOrder));

            Assert.AreNotEqual(ListEqualityComparer<IDictionary<string, string>>.Aggressive.GetHashCode(control), ListEqualityComparer<IDictionary<string, string>>.Aggressive.GetHashCode(level1EqualLevel2Unequal));
            Assert.IsFalse(ListEqualityComparer<IDictionary<string, string>>.Aggressive.Equals(control, level1EqualLevel2Unequal));
        }

        [Test]
        public void TestEqualityListSimple_Aggressive()
        {
            var control = new List<IList<string>>
                    {
                        new List<string> { "one",  "two",  "three" },
                        new List<string> { "four",  "five", "six" } ,
                        new List<string> { "seven", "eight", "nine" },
                    };
            var equal = new List<IList<string>>
                    {
                        new List<string> { "one",  "two",  "three" },
                        new List<string> { "four",  "five", "six" } ,
                        new List<string> { "seven", "eight", "nine" },
                    };
            var equalDifferentType = new IList<string>[]
            {
                        new List<string> { "one",  "two",  "three" },
                        new List<string> { "four",  "five", "six" } ,
                        new List<string> { "seven", "eight", "nine" },
            };
            var equalDifferentOrder = new List<IList<string>>
                    {
                        new List<string> { "four",  "five", "six" } ,
                        new List<string> { "seven", "eight", "nine" },
                        new List<string> { "one",  "two",  "three" },
                    };
            var level1EqualLevel2Unequal = new List<IList<string>>
                    {
                        new List<string> { "one",  "two",  "three" },
                        new List<string> { "four",  "five", "six" } ,
                        new List<string> { "seven", "eight", "nine-nine" },
                    };

            Assert.AreEqual(ListEqualityComparer<IList<string>>.Aggressive.GetHashCode(control), ListEqualityComparer<IList<string>>.Aggressive.GetHashCode(control));
            Assert.IsTrue(ListEqualityComparer<IList<string>>.Aggressive.Equals(control, control));

            Assert.AreEqual(ListEqualityComparer<IList<string>>.Aggressive.GetHashCode(control), ListEqualityComparer<IList<string>>.Aggressive.GetHashCode(equal));
            Assert.IsTrue(ListEqualityComparer<IList<string>>.Aggressive.Equals(control, equal));

            Assert.AreEqual(ListEqualityComparer<IList<string>>.Aggressive.GetHashCode(control), ListEqualityComparer<IList<string>>.Aggressive.GetHashCode(equalDifferentType));
            Assert.IsTrue(ListEqualityComparer<IList<string>>.Aggressive.Equals(control, equalDifferentType));

            // Lists and arrays are order - sensitive
            Assert.AreNotEqual(ListEqualityComparer<IList<string>>.Aggressive.GetHashCode(control), ListEqualityComparer<IList<string>>.Aggressive.GetHashCode(equalDifferentOrder));
            Assert.IsFalse(ListEqualityComparer<IList<string>>.Aggressive.Equals(control, equalDifferentOrder));

            Assert.AreNotEqual(ListEqualityComparer<IList<string>>.Aggressive.GetHashCode(control), ListEqualityComparer<IList<string>>.Aggressive.GetHashCode(level1EqualLevel2Unequal));
            Assert.IsFalse(ListEqualityComparer<IList<string>>.Aggressive.Equals(control, level1EqualLevel2Unequal));
        }

        [Test]
        public void TestEqualityListShallow_Default()
        {
            var control = new List<int> { 1, 2, 3, 4, 5 };
            var equal = new List<int> { 1, 2, 3, 4, 5 };
            var equalDifferentType = new int[] { 1, 2, 3, 4, 5 };
            var equalDifferentOrder = new List<int> { 1, 2, 3, 5, 4 };

            Assert.AreEqual(ListEqualityComparer<int>.Default.GetHashCode(control), ListEqualityComparer<int>.Default.GetHashCode(control));
            Assert.AreEqual(ListEqualityComparer<int>.Default.GetHashCode(control), ListEqualityComparer<int>.Default.GetHashCode(equal));
            Assert.AreEqual(ListEqualityComparer<int>.Default.GetHashCode(control), ListEqualityComparer<int>.Default.GetHashCode(equalDifferentType));
            // Lists and arrays are order-sensitive
            Assert.AreNotEqual(ListEqualityComparer<int>.Default.GetHashCode(control), ListEqualityComparer<int>.Default.GetHashCode(equalDifferentOrder));
        }

        [Test]
        public void TestEqualityListShallow_Aggressive()
        {
            var control = new List<int> { 1, 2, 3, 4, 5 };
            var equal = new List<int> { 1, 2, 3, 4, 5 };
            var equalDifferentType = new int[] { 1, 2, 3, 4, 5 };
            var equalDifferentOrder = new List<int> { 1, 2, 3, 5, 4 };

            Assert.AreEqual(ListEqualityComparer<int>.Aggressive.GetHashCode(control), ListEqualityComparer<int>.Aggressive.GetHashCode(control));
            Assert.AreEqual(ListEqualityComparer<int>.Aggressive.GetHashCode(control), ListEqualityComparer<int>.Aggressive.GetHashCode(equal));
            Assert.AreEqual(ListEqualityComparer<int>.Aggressive.GetHashCode(control), ListEqualityComparer<int>.Aggressive.GetHashCode(equalDifferentType));
            // Lists and arrays are order-sensitive
            Assert.AreNotEqual(ListEqualityComparer<int>.Aggressive.GetHashCode(control), ListEqualityComparer<int>.Aggressive.GetHashCode(equalDifferentOrder));
        }

        [Test]
        public void TestEqualityListDeep_Aggressive()
        {
            var deepControl = new List<IDictionary<string, string>>
            {
                new Dictionary<string, string> { { "1", "one" }, { "2", "two" }, { "3", "three" } },
                new Dictionary<string, string> { { "4", "four" }, { "5", "five" }, { "6", "six" } },
                new Dictionary<string, string> { { "7", "seven" }, { "8", "eight" }, { "9", "nine" } },
            };
            var deepEqual = new List<IDictionary<string, string>>
            {
                new Dictionary<string, string> { { "1", "one" }, { "2", "two" }, { "3", "three" } },
                new Dictionary<string, string> { { "4", "four" }, { "5", "five" }, { "6", "six" } },
                new Dictionary<string, string> { { "7", "seven" }, { "8", "eight" }, { "9", "nine" } },
            };
            var deepEqualDifferentType = new IDictionary<string, string>[]
            {
                new Dictionary<string, string> { { "1", "one" }, { "2", "two" }, { "3", "three" } },
                new Dictionary<string, string> { { "4", "four" }, { "5", "five" }, { "6", "six" } },
                new Dictionary<string, string> { { "7", "seven" }, { "8", "eight" }, { "9", "nine" } },
            };
            var deepEqualDifferentOrder = new List<IDictionary<string, string>>
            {
                new Dictionary<string, string> { { "7", "seven" }, { "8", "eight" }, { "9", "nine" } },
                new Dictionary<string, string> { { "1", "one" }, { "2", "two" }, { "3", "three" } },
                new Dictionary<string, string> { { "4", "four" }, { "5", "five" }, { "6", "six" } },
            };
            var deepLevel1EqualLevel2Unequal = new List<IDictionary<string, string>>
            {
                new Dictionary<string, string> { { "1", "one" }, { "2", "two" }, { "3", "three" } },
                new Dictionary<string, string> { { "4", "four" }, { "5", "five" }, { "6", "six" } },
                new Dictionary<string, string> { { "7", "seven" }, { "8", "eight" }, { "9", "nine99" } },
            };

            Assert.AreEqual(ListEqualityComparer<IDictionary<string, string>>.Aggressive.GetHashCode(deepControl), ListEqualityComparer<IDictionary<string, string>>.Aggressive.GetHashCode(deepControl));
            Assert.AreEqual(ListEqualityComparer<IDictionary<string, string>>.Aggressive.GetHashCode(deepControl), ListEqualityComparer<IDictionary<string, string>>.Aggressive.GetHashCode(deepEqual));
            Assert.AreEqual(ListEqualityComparer<IDictionary<string, string>>.Aggressive.GetHashCode(deepControl), ListEqualityComparer<IDictionary<string, string>>.Aggressive.GetHashCode(deepEqualDifferentType));
            // Lists and arrays are order-sensitive
            Assert.AreNotEqual(ListEqualityComparer<IDictionary<string, string>>.Aggressive.GetHashCode(deepControl), ListEqualityComparer<IDictionary<string, string>>.Aggressive.GetHashCode(deepEqualDifferentOrder));
            Assert.AreNotEqual(ListEqualityComparer<IDictionary<string, string>>.Aggressive.GetHashCode(deepControl), ListEqualityComparer<IDictionary<string, string>>.Aggressive.GetHashCode(deepLevel1EqualLevel2Unequal));
        }

        


        [Test]
        public void TestEqualityListDeep_IStructuralEquatable_Aggressive()
        {
            var deepControl = new StructuralEquatableList<IDictionary<string, string>>(ListEqualityComparer<IDictionary<string, string>>.Aggressive)
            {
                new StructuralEquatableDictionary<string, string>(DictionaryEqualityComparer<string, string>.Aggressive) { { "1", "one" }, { "2", "two" }, { "3", "three" } },
                new StructuralEquatableDictionary<string, string>(DictionaryEqualityComparer<string, string>.Aggressive) { { "4", "four" }, { "5", "five" }, { "6", "six" } },
                new StructuralEquatableDictionary<string, string>(DictionaryEqualityComparer<string, string>.Aggressive) { { "7", "seven" }, { "8", "eight" }, { "9", "nine" } },
            };
            var deepEqual = new StructuralEquatableList<IDictionary<string, string>>(ListEqualityComparer<IDictionary<string, string>>.Aggressive)
            {
                new StructuralEquatableDictionary<string, string>(DictionaryEqualityComparer<string, string>.Aggressive) { { "1", "one" }, { "2", "two" }, { "3", "three" } },
                new StructuralEquatableDictionary<string, string>(DictionaryEqualityComparer<string, string>.Aggressive) { { "4", "four" }, { "5", "five" }, { "6", "six" } },
                new StructuralEquatableDictionary<string, string>(DictionaryEqualityComparer<string, string>.Aggressive) { { "7", "seven" }, { "8", "eight" }, { "9", "nine" } },
            };
            var deepEqualDifferentType = new IDictionary<string, string>[]
            {
                new StructuralEquatableDictionary<string, string>(DictionaryEqualityComparer<string, string>.Aggressive) { { "1", "one" }, { "2", "two" }, { "3", "three" } },
                new StructuralEquatableDictionary<string, string>(DictionaryEqualityComparer<string, string>.Aggressive) { { "4", "four" }, { "5", "five" }, { "6", "six" } },
                new StructuralEquatableDictionary<string, string>(DictionaryEqualityComparer<string, string>.Aggressive) { { "7", "seven" }, { "8", "eight" }, { "9", "nine" } },
            };
            var deepEqualDifferentOrder = new StructuralEquatableList<IDictionary<string, string>>(ListEqualityComparer<IDictionary<string, string>>.Aggressive)
            {
                new StructuralEquatableDictionary<string, string>(DictionaryEqualityComparer<string, string>.Aggressive) { { "7", "seven" }, { "8", "eight" }, { "9", "nine" } },
                new StructuralEquatableDictionary<string, string>(DictionaryEqualityComparer<string, string>.Aggressive) { { "1", "one" }, { "2", "two" }, { "3", "three" } },
                new StructuralEquatableDictionary<string, string>(DictionaryEqualityComparer<string, string>.Aggressive) { { "4", "four" }, { "5", "five" }, { "6", "six" } },
            };
            var deepLevel1EqualLevel2Unequal = new StructuralEquatableList<IDictionary<string, string>>(ListEqualityComparer<IDictionary<string, string>>.Aggressive)
            {
                new StructuralEquatableDictionary<string, string>(DictionaryEqualityComparer<string, string>.Aggressive) { { "1", "one" }, { "2", "two" }, { "3", "three" } },
                new StructuralEquatableDictionary<string, string>(DictionaryEqualityComparer<string, string>.Aggressive) { { "4", "four" }, { "5", "five" }, { "6", "six" } },
                new StructuralEquatableDictionary<string, string>(DictionaryEqualityComparer<string, string>.Aggressive) { { "7", "seven" }, { "8", "eight" }, { "9", "nine99" } },
            };

            Assert.AreEqual(ListEqualityComparer<IDictionary<string, string>>.Aggressive.GetHashCode(deepControl), ListEqualityComparer<IDictionary<string, string>>.Aggressive.GetHashCode(deepControl));
            Assert.AreEqual(ListEqualityComparer<IDictionary<string, string>>.Aggressive.GetHashCode(deepControl), ListEqualityComparer<IDictionary<string, string>>.Aggressive.GetHashCode(deepEqual));
            Assert.AreEqual(ListEqualityComparer<IDictionary<string, string>>.Aggressive.GetHashCode(deepControl), ListEqualityComparer<IDictionary<string, string>>.Aggressive.GetHashCode(deepEqualDifferentType));
            // Lists and arrays are order-sensitive
            Assert.AreNotEqual(ListEqualityComparer<IDictionary<string, string>>.Aggressive.GetHashCode(deepControl), ListEqualityComparer<IDictionary<string, string>>.Aggressive.GetHashCode(deepEqualDifferentOrder));
            Assert.AreNotEqual(ListEqualityComparer<IDictionary<string, string>>.Aggressive.GetHashCode(deepControl), ListEqualityComparer<IDictionary<string, string>>.Aggressive.GetHashCode(deepLevel1EqualLevel2Unequal));
        }

        [Test]
        public void TestEqualityListDeep_IStructuralEquatable_Default()
        {
            var deepControl = new StructuralEquatableList<IDictionary<string, string>>(ListEqualityComparer<IDictionary<string, string>>.Default)
            {
                new StructuralEquatableDictionary<string, string>(DictionaryEqualityComparer<string, string>.Default) { { "1", "one" }, { "2", "two" }, { "3", "three" } },
                new StructuralEquatableDictionary<string, string>(DictionaryEqualityComparer<string, string>.Default) { { "4", "four" }, { "5", "five" }, { "6", "six" } },
                new StructuralEquatableDictionary<string, string>(DictionaryEqualityComparer<string, string>.Default) { { "7", "seven" }, { "8", "eight" }, { "9", "nine" } },
            };
            var deepEqual = new StructuralEquatableList<IDictionary<string, string>>(ListEqualityComparer<IDictionary<string, string>>.Default)
            {
                new StructuralEquatableDictionary<string, string>(DictionaryEqualityComparer<string, string>.Default) { { "1", "one" }, { "2", "two" }, { "3", "three" } },
                new StructuralEquatableDictionary<string, string>(DictionaryEqualityComparer<string, string>.Default) { { "4", "four" }, { "5", "five" }, { "6", "six" } },
                new StructuralEquatableDictionary<string, string>(DictionaryEqualityComparer<string, string>.Default) { { "7", "seven" }, { "8", "eight" }, { "9", "nine" } },
            };
            var deepEqualDifferentType = new IDictionary<string, string>[]
            {
                new StructuralEquatableDictionary<string, string>(DictionaryEqualityComparer<string, string>.Default) { { "1", "one" }, { "2", "two" }, { "3", "three" } },
                new StructuralEquatableDictionary<string, string>(DictionaryEqualityComparer<string, string>.Default) { { "4", "four" }, { "5", "five" }, { "6", "six" } },
                new StructuralEquatableDictionary<string, string>(DictionaryEqualityComparer<string, string>.Default) { { "7", "seven" }, { "8", "eight" }, { "9", "nine" } },
            };
            var deepEqualDifferentOrder = new StructuralEquatableList<IDictionary<string, string>>(ListEqualityComparer<IDictionary<string, string>>.Default)
            {
                new StructuralEquatableDictionary<string, string>(DictionaryEqualityComparer<string, string>.Default) { { "7", "seven" }, { "8", "eight" }, { "9", "nine" } },
                new StructuralEquatableDictionary<string, string>(DictionaryEqualityComparer<string, string>.Default) { { "1", "one" }, { "2", "two" }, { "3", "three" } },
                new StructuralEquatableDictionary<string, string>(DictionaryEqualityComparer<string, string>.Default) { { "4", "four" }, { "5", "five" }, { "6", "six" } },
            };
            var deepLevel1EqualLevel2Unequal = new StructuralEquatableList<IDictionary<string, string>>(ListEqualityComparer<IDictionary<string, string>>.Default)
            {
                new StructuralEquatableDictionary<string, string>(DictionaryEqualityComparer<string, string>.Default) { { "1", "one" }, { "2", "two" }, { "3", "three" } },
                new StructuralEquatableDictionary<string, string>(DictionaryEqualityComparer<string, string>.Default) { { "4", "four" }, { "5", "five" }, { "6", "six" } },
                new StructuralEquatableDictionary<string, string>(DictionaryEqualityComparer<string, string>.Default) { { "7", "seven" }, { "8", "eight" }, { "9", "nine99" } },
            };

            Assert.AreEqual(ListEqualityComparer<IDictionary<string, string>>.Default.GetHashCode(deepControl), ListEqualityComparer<IDictionary<string, string>>.Default.GetHashCode(deepControl));
            Assert.AreEqual(ListEqualityComparer<IDictionary<string, string>>.Default.GetHashCode(deepControl), ListEqualityComparer<IDictionary<string, string>>.Default.GetHashCode(deepEqual));
            Assert.AreEqual(ListEqualityComparer<IDictionary<string, string>>.Default.GetHashCode(deepControl), ListEqualityComparer<IDictionary<string, string>>.Default.GetHashCode(deepEqualDifferentType));
            // Lists and arrays are order-sensitive
            Assert.AreNotEqual(ListEqualityComparer<IDictionary<string, string>>.Default.GetHashCode(deepControl), ListEqualityComparer<IDictionary<string, string>>.Default.GetHashCode(deepEqualDifferentOrder));
            Assert.AreNotEqual(ListEqualityComparer<IDictionary<string, string>>.Default.GetHashCode(deepControl), ListEqualityComparer<IDictionary<string, string>>.Default.GetHashCode(deepLevel1EqualLevel2Unequal));
        }

        /// <summary>
        /// In Java, float.NaN is always equal to float.NaN.
        /// </summary>
        [Test]
        public void TestEqualityFloatNaN()
        {
            sbyte[] nan1s = { -111, 28, -95, -1 };
            sbyte[] nan2s = { 76, -83, -47, 127 };
            byte[] nan1 = new byte[4];
            byte[] nan2 = new byte[4];
            Buffer.BlockCopy(nan1s, 0, nan1, 0, 4);
            Buffer.BlockCopy(nan2s, 0, nan2, 0, 4);

            var list1 = new List<float> { BitConverter.ToSingle(nan1, 0) };
            var list2 = new List<float> { BitConverter.ToSingle(nan2, 0) };

            assertTrue(ListEqualityComparer<float>.Default.Equals(list1, list2));
            assertEquals(ListEqualityComparer<float>.Default.GetHashCode(list1), ListEqualityComparer<float>.Default.GetHashCode(list2));

            // One dimensional arrays should work
            var list3 = new List<float[]> { new float[] { BitConverter.ToSingle(nan1, 0), BitConverter.ToSingle(nan1, 0), BitConverter.ToSingle(nan2, 0) } };
            var list4 = new List<float[]> { new float[] { BitConverter.ToSingle(nan1, 0), BitConverter.ToSingle(nan1, 0), BitConverter.ToSingle(nan2, 0) } };

            assertTrue(ListEqualityComparer<float[]>.Default.Equals(list3, list4));
            assertEquals(ListEqualityComparer<float[]>.Default.GetHashCode(list3), ListEqualityComparer<float[]>.Default.GetHashCode(list4));

            // Multi dimensional arrays are not (yet) supported
            var list5 = new List<float[,]> { new float[,] { { BitConverter.ToSingle(nan1, 0), BitConverter.ToSingle(nan2, 0) }, { BitConverter.ToSingle(nan2, 0), BitConverter.ToSingle(nan1, 0) } } };
            var list6 = new List<float[,]> { new float[,] { { BitConverter.ToSingle(nan1, 0), BitConverter.ToSingle(nan2, 0) }, { BitConverter.ToSingle(nan2, 0), BitConverter.ToSingle(nan1, 0) } } };

            Assert.Throws<ArgumentException>(() => ListEqualityComparer<float[,]>.Default.Equals(list5, list6));
            Assert.Throws<ArgumentException>(() => ListEqualityComparer<float[,]>.Default.GetHashCode(list5));
        }

        /// <summary>
        /// In Java, double.NaN is always equal to double.NaN.
        /// </summary>
        [Test]
        public void TestEqualityDoubleNaN()
        {
            sbyte[] nan1s = { 5, 84, 21, 61, -39, -20, -8, -1 };
            sbyte[] nan2s = { -65, 78, -21, -6, 47, 123, -4, 127 };
            byte[] nan1 = new byte[8];
            byte[] nan2 = new byte[8];
            Buffer.BlockCopy(nan1s, 0, nan1, 0, 8);
            Buffer.BlockCopy(nan2s, 0, nan2, 0, 8);

            var list1 = new List<double> { BitConverter.ToDouble(nan1, 0) };
            var list2 = new List<double> { BitConverter.ToDouble(nan2, 0) };

            assertTrue(ListEqualityComparer<double>.Default.Equals(list1, list2));
            assertEquals(ListEqualityComparer<double>.Default.GetHashCode(list1), ListEqualityComparer<double>.Default.GetHashCode(list2));

            // One dimensional arrays should work
            var list3 = new List<double[]> { new double[] { BitConverter.ToDouble(nan1, 0), BitConverter.ToDouble(nan1, 0), BitConverter.ToDouble(nan2, 0) } };
            var list4 = new List<double[]> { new double[] { BitConverter.ToDouble(nan1, 0), BitConverter.ToDouble(nan1, 0), BitConverter.ToDouble(nan2, 0) } };

            assertTrue(ListEqualityComparer<double[]>.Default.Equals(list3, list4));
            assertEquals(ListEqualityComparer<double[]>.Default.GetHashCode(list3), ListEqualityComparer<double[]>.Default.GetHashCode(list4));

            // Multi dimensional arrays are not (yet) supported
            var list5 = new List<double[,]> { new double[,] { { BitConverter.ToDouble(nan1, 0), BitConverter.ToDouble(nan2, 0) }, { BitConverter.ToDouble(nan2, 0), BitConverter.ToDouble(nan1, 0) } } };
            var list6 = new List<double[,]> { new double[,] { { BitConverter.ToDouble(nan1, 0), BitConverter.ToDouble(nan2, 0) }, { BitConverter.ToDouble(nan2, 0), BitConverter.ToDouble(nan1, 0) } } };

            Assert.Throws<ArgumentException>(() => ListEqualityComparer<double[,]>.Default.Equals(list5, list6));
            Assert.Throws<ArgumentException>(() => ListEqualityComparer<double[,]>.Default.GetHashCode(list5));
        }

        /// <summary>
        /// In Java, negative and positive 0 are two different values, thus not equal
        /// </summary>
        [Test]
        public void TestEqualityFloatNegiativeZero()
        {
            var list1 = new List<float> { -0.0f };
            var list2 = new List<float> { -0.0f };
            var list3 = new List<float> { 0.0f };

            assertTrue(ListEqualityComparer<float>.Default.Equals(list1, list2));
            assertEquals(ListEqualityComparer<float>.Default.GetHashCode(list1), ListEqualityComparer<float>.Default.GetHashCode(list2));
            assertFalse(ListEqualityComparer<float>.Default.Equals(list1, list3));
            Assert.AreNotEqual(ListEqualityComparer<float>.Default.GetHashCode(list1), ListEqualityComparer<float>.Default.GetHashCode(list3));

            // One dimensional arrays should work
            var list4 = new List<float[]> { new float[] { -0.0f, -0.0f, 0.0f } };
            var list5 = new List<float[]> { new float[] { -0.0f, -0.0f, 0.0f } };
            var list6 = new List<float[]> { new float[] { -0.0f, -0.0f, -0.0f } };

            assertTrue(ListEqualityComparer<float[]>.Default.Equals(list4, list5));
            assertEquals(ListEqualityComparer<float[]>.Default.GetHashCode(list4), ListEqualityComparer<float[]>.Default.GetHashCode(list5));
            assertFalse(ListEqualityComparer<float[]>.Default.Equals(list4, list6));
            Assert.AreNotEqual(ListEqualityComparer<float[]>.Default.GetHashCode(list4), ListEqualityComparer<float[]>.Default.GetHashCode(list6));

            // Multi dimensional arrays are not (yet) supported
            var list10 = new List<float[,]> { new float[,] { { -0.0f, -0.0f }, { 0.0f, -0.0f } } };
            var list11 = new List<float[,]> { new float[,] { { -0.0f, -0.0f }, { 0.0f, -0.0f } } };
            var list12 = new List<float[,]> { new float[,] { { -0.0f, -0.0f }, { -0.0f, -0.0f } } };

            Assert.Throws<ArgumentException>(() => ListEqualityComparer<float[,]>.Default.Equals(list10, list11));
            Assert.Throws<ArgumentException>(() => ListEqualityComparer<float[,]>.Default.GetHashCode(list10));
            Assert.Throws<ArgumentException>(() => ListEqualityComparer<float[,]>.Default.Equals(list10, list12));
            Assert.Throws<ArgumentException>(() => ListEqualityComparer<float[,]>.Default.GetHashCode(list12));
        }

        /// <summary>
        /// In Java, negative and positive 0 are two different values, thus not equal
        /// </summary>
        [Test]
        public void TestEqualityDoubleNegiativeZero()
        {
            var list1 = new List<double> { -0.0d };
            var list2 = new List<double> { -0.0d };
            var list3 = new List<double> { 0.0d };

            assertTrue(ListEqualityComparer<double>.Default.Equals(list1, list2));
            assertEquals(ListEqualityComparer<double>.Default.GetHashCode(list1), ListEqualityComparer<double>.Default.GetHashCode(list2));
            assertFalse(ListEqualityComparer<double>.Default.Equals(list1, list3));
            Assert.AreNotEqual(ListEqualityComparer<double>.Default.GetHashCode(list1), ListEqualityComparer<double>.Default.GetHashCode(list3));

            // One dimensional arrays should work
            var list4 = new List<double[]> { new double[] { -0.0d, -0.0d, 0.0d } };
            var list5 = new List<double[]> { new double[] { -0.0d, -0.0d, 0.0d } };
            var list6 = new List<double[]> { new double[] { -0.0d, -0.0d, -0.0d } };

            assertTrue(ListEqualityComparer<double[]>.Default.Equals(list4, list5));
            assertEquals(ListEqualityComparer<double[]>.Default.GetHashCode(list4), ListEqualityComparer<double[]>.Default.GetHashCode(list5));
            assertFalse(ListEqualityComparer<double[]>.Default.Equals(list4, list6));
            Assert.AreNotEqual(ListEqualityComparer<double[]>.Default.GetHashCode(list4), ListEqualityComparer<double[]>.Default.GetHashCode(list6));

            // Multi dimensional arrays are not (yet) supported
            var list10 = new List<double[,]> { new double[,] { { -0.0d, -0.0d }, { 0.0d, -0.0d } } };
            var list11 = new List<double[,]> { new double[,] { { -0.0d, -0.0d }, { 0.0d, -0.0d } } };
            var list12 = new List<double[,]> { new double[,] { { -0.0d, -0.0d }, { -0.0d, -0.0d } } };

            Assert.Throws<ArgumentException>(() => ListEqualityComparer<double[,]>.Default.Equals(list10, list11));
            Assert.Throws<ArgumentException>(() => ListEqualityComparer<double[,]>.Default.GetHashCode(list10));
            Assert.Throws<ArgumentException>(() => ListEqualityComparer<double[,]>.Default.Equals(list10, list12));
            Assert.Throws<ArgumentException>(() => ListEqualityComparer<double[,]>.Default.GetHashCode(list12));
        }
    }
}
