using J2N.Collections.Generic;
using J2N.Text;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace J2N.Collections
{
    public class TestCollectionUtil : TestCase
    {
        // NOTE: For Aggressive mode to work right, all collections it uses (including this one)
        // must be declared public.
        public class HashMap<TKey, TValue> : Dictionary<TKey, TValue>
        {
            public override bool Equals(object obj)
            {
                if (obj is IDictionary<TKey, TValue> otherDictionary)
                    return DictionaryEqualityComparer<TKey, TValue>.Aggressive.Equals(this, otherDictionary);
                return false;
            }

            public override int GetHashCode()
            {
                return DictionaryEqualityComparer<TKey, TValue>.Aggressive.GetHashCode(this);
            }
        }

        [Test]
        public void TestEqualsTypeMismatch()
        {
            var list = new List<int> { 1, 2, 3, 4, 5 };
            var set = new System.Collections.Generic.HashSet<int> { 1, 2, 3, 4, 5 };
            var dictionary = new Dictionary<int, int> { { 1, 0 }, { 2, 0 }, { 3, 0 }, { 4, 0 }, { 5, 0 } };
            var array = new int[] { 1, 2, 3, 4, 5 };

            Assert.IsFalse(CollectionUtil.Equals(list, set));
            Assert.IsFalse(CollectionUtil.Equals(list, dictionary));
            Assert.IsTrue(CollectionUtil.Equals(list, array)); // Types are compatible - array implements IList<T>

            Assert.IsFalse(CollectionUtil.Equals(set, dictionary));
            Assert.IsFalse(CollectionUtil.Equals(set, array));
        }

        [Test]
        public void TestEqualityDictionary()
        {
            var control = new Dictionary<string, IDictionary<HashMap<long, double>, string>>
                    {
                        { "a", new Dictionary<HashMap<long, double>, string> { { new HashMap<long, double> { { 123, 9.87 }, { 80, 88 } }, "qwerty" } } },
                        { "z", new Dictionary<HashMap<long, double>, string> { { new HashMap<long, double> { { 456, 9.86 }, { 81, 88 } }, "hexagon" } } },
                        { "r", new Dictionary<HashMap<long, double>, string> { { new HashMap<long, double> { { 789, 9.85 }, { 82, 88 } }, "parasite" } } },
                        { "t", new Dictionary<HashMap<long, double>, string> { { new HashMap<long, double> { { 101, 9.84 }, { 83, 88 } }, "octopus" } } },
                    };
            var equal = new Dictionary<string, IDictionary<HashMap<long, double>, string>>
                    {
                        { "a", new Dictionary<HashMap<long, double>, string> { { new HashMap<long, double> { { 123, 9.87 }, { 80, 88 } }, "qwerty" } } },
                        { "z", new Dictionary<HashMap<long, double>, string> { { new HashMap<long, double> { { 456, 9.86 }, { 81, 88 } }, "hexagon" } } },
                        { "r", new Dictionary<HashMap<long, double>, string> { { new HashMap<long, double> { { 789, 9.85 }, { 82, 88 } }, "parasite" } } },
                        { "t", new Dictionary<HashMap<long, double>, string> { { new HashMap<long, double> { { 101, 9.84 }, { 83, 88 } }, "octopus" } } },
                    };
            var equalDifferentType = new HashMap<string, IDictionary<HashMap<long, double>, string>>
                    {
                        { "a", new Dictionary<HashMap<long, double>, string> { { new HashMap<long, double> { { 123, 9.87 }, { 80, 88 } }, "qwerty" } } },
                        { "z", new Dictionary<HashMap<long, double>, string> { { new HashMap<long, double> { { 456, 9.86 }, { 81, 88 } }, "hexagon" } } },
                        { "r", new Dictionary<HashMap<long, double>, string> { { new HashMap<long, double> { { 789, 9.85 }, { 82, 88 } }, "parasite" } } },
                        { "t", new Dictionary<HashMap<long, double>, string> { { new HashMap<long, double> { { 101, 9.84 }, { 83, 88 } }, "octopus" } } },
                    };
            var equalDifferentOrder = new Dictionary<string, IDictionary<HashMap<long, double>, string>>
                    {
                        { "r", new Dictionary<HashMap<long, double>, string> { { new HashMap<long, double> { { 789, 9.85 }, { 82, 88 } }, "parasite" } } },
                        { "t", new Dictionary<HashMap<long, double>, string> { { new HashMap<long, double> { { 101, 9.84 }, { 83, 88 } }, "octopus" } } },
                        { "a", new Dictionary<HashMap<long, double>, string> { { new HashMap<long, double> { { 123, 9.87 }, { 80, 88 } }, "qwerty" } } },
                        { "z", new Dictionary<HashMap<long, double>, string> { { new HashMap<long, double> { { 456, 9.86 }, { 81, 88 } }, "hexagon" } } },
                    };

            var level1EqualLevel2EqualLevel3Unequal = new Dictionary<string, IDictionary<HashMap<long, double>, string>>
                    {
                        { "a", new Dictionary<HashMap<long, double>, string> { { new HashMap<long, double> { { 123, 9.87 }, { 80, 88.1 } }, "qwerty" } } },
                        { "z", new Dictionary<HashMap<long, double>, string> { { new HashMap<long, double> { { 456, 9.86 }, { 81, 88 } }, "hexagon" } } },
                        { "r", new Dictionary<HashMap<long, double>, string> { { new HashMap<long, double> { { 789, 9.85 }, { 82, 88 } }, "parasite" } } },
                        { "t", new Dictionary<HashMap<long, double>, string> { { new HashMap<long, double> { { 101, 9.84 }, { 83, 88 } }, "octopus" } } },
                    };

            Assert.AreEqual(CollectionUtil.GetHashCode(control), CollectionUtil.GetHashCode(control));
            Assert.IsTrue(CollectionUtil.Equals(control, control));

            Assert.AreEqual(CollectionUtil.GetHashCode(control), CollectionUtil.GetHashCode(equal));
            Assert.IsTrue(CollectionUtil.Equals(control, equal));

            Assert.AreEqual(CollectionUtil.GetHashCode(control), CollectionUtil.GetHashCode(equalDifferentType));
            Assert.IsTrue(CollectionUtil.Equals(control, equalDifferentType));

            Assert.AreEqual(CollectionUtil.GetHashCode(control), CollectionUtil.GetHashCode(equalDifferentOrder));
            Assert.IsTrue(CollectionUtil.Equals(control, equalDifferentOrder));

            Assert.AreNotEqual(CollectionUtil.GetHashCode(control), CollectionUtil.GetHashCode(level1EqualLevel2EqualLevel3Unequal));
            Assert.IsFalse(CollectionUtil.Equals(control, level1EqualLevel2EqualLevel3Unequal));
        }

        [Test]
        public void TestEqualityList()
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

            Assert.AreEqual(CollectionUtil.GetHashCode(control), CollectionUtil.GetHashCode(control));
            Assert.IsTrue(CollectionUtil.Equals(control, control));

            Assert.AreEqual(CollectionUtil.GetHashCode(control), CollectionUtil.GetHashCode(equal));
            Assert.IsTrue(CollectionUtil.Equals(control, equal));

            Assert.AreEqual(CollectionUtil.GetHashCode(control), CollectionUtil.GetHashCode(equalDifferentType));
            Assert.IsTrue(CollectionUtil.Equals(control, equalDifferentType));

            // Lists and arrays are order-sensitive
            Assert.AreNotEqual(CollectionUtil.GetHashCode(control), CollectionUtil.GetHashCode(equalDifferentOrder));
            Assert.IsFalse(CollectionUtil.Equals(control, equalDifferentOrder));

            Assert.AreNotEqual(CollectionUtil.GetHashCode(control), CollectionUtil.GetHashCode(level1EqualLevel2Unequal));
            Assert.IsFalse(CollectionUtil.Equals(control, level1EqualLevel2Unequal));
        }

        [Test]
        public void TestEqualityListSimple()
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

            Assert.AreEqual(CollectionUtil.GetHashCode(control), CollectionUtil.GetHashCode(control));
            Assert.IsTrue(CollectionUtil.Equals(control, control));

            Assert.AreEqual(CollectionUtil.GetHashCode(control), CollectionUtil.GetHashCode(equal));
            Assert.IsTrue(CollectionUtil.Equals(control, equal));

            Assert.AreEqual(CollectionUtil.GetHashCode(control), CollectionUtil.GetHashCode(equalDifferentType));
            Assert.IsTrue(CollectionUtil.Equals(control, equalDifferentType));

            // Lists and arrays are order - sensitive
            Assert.AreNotEqual(CollectionUtil.GetHashCode(control), CollectionUtil.GetHashCode(equalDifferentOrder));
            Assert.IsFalse(CollectionUtil.Equals(control, equalDifferentOrder));

            Assert.AreNotEqual(CollectionUtil.GetHashCode(control), CollectionUtil.GetHashCode(level1EqualLevel2Unequal));
            Assert.IsFalse(CollectionUtil.Equals(control, level1EqualLevel2Unequal));
        }


        private class MockHashSet<T> : System.Collections.Generic.HashSet<T>
        {
            public override int GetHashCode()
            {
                return Random.Next(); // Random garbage to ensure it is not equal
            }

            public override bool Equals(object obj)
            {
                return false;
            }
        }

        [Test]
        public void TestEqualitySet()
        {
            var control = new System.Collections.Generic.HashSet<IDictionary<string, string>>
                    {
                        new Dictionary<string, string> { { "1", "one" }, { "2", "two" }, { "3", "three" } },
                        new Dictionary<string, string> { { "4", "four" }, { "5", "five" }, { "6", "six" } },
                        new Dictionary<string, string> { { "7", "seven" }, { "8", "eight" }, { "9", "nine" } },
                    };
            var equal = new System.Collections.Generic.HashSet<IDictionary<string, string>>
                    {
                        new Dictionary<string, string> { { "1", "one" }, { "2", "two" }, { "3", "three" } },
                        new Dictionary<string, string> { { "4", "four" }, { "5", "five" }, { "6", "six" } },
                        new Dictionary<string, string> { { "7", "seven" }, { "8", "eight" }, { "9", "nine" } },
                    };
            var equalDifferentType = new MockHashSet<IDictionary<string, string>>
                    {
                        new Dictionary<string, string> { { "1", "one" }, { "2", "two" }, { "3", "three" } },
                        new Dictionary<string, string> { { "4", "four" }, { "5", "five" }, { "6", "six" } },
                        new Dictionary<string, string> { { "7", "seven" }, { "8", "eight" }, { "9", "nine" } },
                    };
            var equalDifferentOrder = new System.Collections.Generic.HashSet<IDictionary<string, string>>
                    {
                        new Dictionary<string, string> { { "7", "seven" }, { "8", "eight" }, { "9", "nine" } },
                        new Dictionary<string, string> { { "1", "one" }, { "2", "two" }, { "3", "three" } },
                        new Dictionary<string, string> { { "4", "four" }, { "5", "five" }, { "6", "six" } },
                    };
            var level1EqualLevel2Unequal = new System.Collections.Generic.HashSet<IDictionary<string, string>>
                    {
                        new Dictionary<string, string> { { "1", "one" }, { "2", "two" }, { "3", "three" } },
                        new Dictionary<string, string> { { "4", "four" }, { "5", "five" }, { "6", "six" } },
                        new Dictionary<string, string> { { "7", "seven" }, { "8", "eight" }, { "9", "nine99" } },
                    };

            Assert.AreEqual(CollectionUtil.GetHashCode(control), CollectionUtil.GetHashCode(control));
            Assert.IsTrue(CollectionUtil.Equals(control, control));

            Assert.AreEqual(CollectionUtil.GetHashCode(control), CollectionUtil.GetHashCode(equal));
            Assert.IsTrue(CollectionUtil.Equals(control, equal));

            Assert.AreEqual(CollectionUtil.GetHashCode(control), CollectionUtil.GetHashCode(equalDifferentType));
            Assert.IsTrue(CollectionUtil.Equals(control, equalDifferentType));

            // Sets are not order-sensitive
            Assert.AreEqual(CollectionUtil.GetHashCode(control), CollectionUtil.GetHashCode(equalDifferentOrder));
            Assert.IsTrue(CollectionUtil.Equals(control, equalDifferentOrder));

            Assert.AreNotEqual(CollectionUtil.GetHashCode(control), CollectionUtil.GetHashCode(level1EqualLevel2Unequal));
            Assert.IsFalse(CollectionUtil.Equals(control, level1EqualLevel2Unequal));
        }

        [Test]
        public void TestToString()
        {
            var set = new J2N.Collections.Generic.HashSet<IDictionary<string, string>>
            {
                new Dictionary<string, string> { { "1", "one" }, { "2", "two" }, { "3", "three" } },
                new Dictionary<string, string> { { "4", "four" }, { "5", "five" }, { "6", "six" } },
                new Dictionary<string, string> { { "7", "seven" }, { "8", "eight" }, { "9", "nine" } },
            };
            var setExpected = "[{1=one, 2=two, 3=three}, {4=four, 5=five, 6=six}, {7=seven, 8=eight, 9=nine}]";

            Assert.AreEqual(setExpected, CollectionUtil.ToString(set, StringFormatter.InvariantCulture));


            var set2 = new J2N.Collections.Generic.HashSet<ISet<string>>
            {
                new J2N.Collections.Generic.HashSet<string> { "1", "2", "3" },
                new J2N.Collections.Generic.HashSet<string> { "4", "5", "6" },
                new System.Collections.Generic.HashSet<string> { "7", "8", "9" },
            };
            var set2Expected = "[[1, 2, 3], [4, 5, 6], [7, 8, 9]]";

            Assert.AreEqual(set2Expected, CollectionUtil.ToString(set2, StringFormatter.InvariantCulture));


            var map = new Dictionary<string, IDictionary<int, double>>
            {
                { "first", new Dictionary<int, double> { { 1, 1.23 }, { 2, 2.23 }, { 3, 3.23 } } },
                { "second", new Dictionary<int, double> { { 4, 1.24 }, { 5, 2.24 }, { 6, 3.24 } } },
                { "third", new Dictionary<int, double> { { 7, 1.25 }, { 8, 2.25 }, { 9, 3.25 } } },
            };
            var mapExpectedPortuguese = "{first={1=1,23, 2=2,23, 3=3,23}, second={4=1,24, 5=2,24, 6=3,24}, third={7=1,25, 8=2,25, 9=3,25}}";
            var mapExpectedUSEnglish = "{first={1=1.23, 2=2.23, 3=3.23}, second={4=1.24, 5=2.24, 6=3.24}, third={7=1.25, 8=2.25, 9=3.25}}";

            Assert.AreEqual(mapExpectedPortuguese, CollectionUtil.ToString(map, new StringFormatter(new CultureInfo("pt"))));
            Assert.AreEqual(mapExpectedUSEnglish, CollectionUtil.ToString(map, new StringFormatter(new CultureInfo("en-US"))));

            var array = new List<Dictionary<string, string>>[]
            {
                new List<Dictionary<string, string>> {
                    new Dictionary<string, string> { { "foo", "bar" }, { "foobar", "barfoo" } }
                },
                new List<Dictionary<string, string>> {
                    new Dictionary<string, string> { { "orange", "yellow" }, { "red", "black" } },
                    new Dictionary<string, string> { { "rain", "snow" }, { "sleet", "sunshine" } }
                },
            };
            var arrayExpected = "[[{foo=bar, foobar=barfoo}], [{orange=yellow, red=black}, {rain=snow, sleet=sunshine}]]";

            Assert.AreEqual(arrayExpected, CollectionUtil.ToString(array, StringFormatter.InvariantCulture));
        }



        //[Test]
        //public void TestEqualityDictionaryShallow()
        //{
        //    var control = new Dictionary<string, IDictionary<int, string>>
        //    {
        //        { "a", new Dictionary<int, string> { { 9, "qwerty" } } },
        //        //{ "z", new Dictionary<int, string> { { 23, "hexagon" } } },
        //        //{ "r", new Dictionary<int, string> { { 4, "parasite" } } },
        //        //{ "t", new Dictionary<int, string> { { 61, "octopus" } } },
        //    };
        //    var equal = new Dictionary<string, IDictionary<int, string>>
        //    {
        //        { "a", new Dictionary<int, string> { { 9, "qwerty" } } },
        //        //{ "z", new Dictionary<int, string> { { 23, "hexagon" } } },
        //        //{ "r", new Dictionary<int, string> { { 4, "parasite" } } },
        //        //{ "t", new Dictionary<int, string> { { 61, "octopus" } } },
        //    };
        //    var equalDifferentType = new HashMap<string, IDictionary<int, string>>
        //    {
        //        { "a", new Dictionary<int, string> { { 9, "qwerty" } } },
        //        { "z", new Dictionary<int, string> { { 23, "hexagon" } } },
        //        { "r", new Dictionary<int, string> { { 4, "parasite" } } },
        //        { "t", new Dictionary<int, string> { { 61, "octopus" } } },
        //    };
        //    var equalDifferentOrder = new Dictionary<string, IDictionary<int, string>>
        //    {
        //        { "r", new Dictionary<int, string> { { 4, "parasite" } } },
        //        { "t", new Dictionary<int, string> { { 61, "octopus" } } },
        //        { "a", new Dictionary<int, string> { { 9, "qwerty" } } },
        //        { "z", new Dictionary<int, string> { { 23, "hexagon" } } },
        //    };
        //    var level1EqualLevel2Unequal = new Dictionary<string, IDictionary<int, string>>
        //    {
        //        { "a", new Dictionary<int, string> { { 9, "qwerty" } } },
        //        { "z", new Dictionary<int, string> { { 23, "hexagon" } } },
        //        { "r", new Dictionary<int, string> { { 4, "parasite" } } },
        //        { "t", new Dictionary<int, string> { { 7, "octopus" } } },
        //    };
        //    var level1UnequalLevel2Equal = new Dictionary<string, IDictionary<int, string>>
        //    {
        //        { "y", new Dictionary<int, string> { { 9, "qwerty" } } },
        //        { "z", new Dictionary<int, string> { { 23, "hexagon" } } },
        //        { "r", new Dictionary<int, string> { { 4, "parasite" } } },
        //        { "t", new Dictionary<int, string> { { 61, "octopus" } } },
        //    };

        //    Assert.AreEqual(CollectionUtil.GetHashCode(control), CollectionUtil.GetHashCode(control));
        //    Assert.IsTrue(CollectionUtil.Equals(control, control));

        //    Assert.AreEqual(CollectionUtil.GetHashCode(control), CollectionUtil.GetHashCode(equal));
        //    Assert.IsTrue(CollectionUtil.Equals(control, equal));

        //    Assert.AreEqual(CollectionUtil.GetHashCode(control), CollectionUtil.GetHashCode(equalDifferentType));
        //    Assert.IsTrue(CollectionUtil.Equals(control, equalDifferentType));

        //    Assert.AreEqual(CollectionUtil.GetHashCode(control), CollectionUtil.GetHashCode(equalDifferentOrder));
        //    Assert.IsTrue(CollectionUtil.Equals(control, equalDifferentOrder));

        //    Assert.AreEqual(CollectionUtil.GetHashCode(control), CollectionUtil.GetHashCode(level1EqualLevel2Unequal));
        //    Assert.IsTrue(CollectionUtil.Equals(control, level1EqualLevel2Unequal));

        //    Assert.AreNotEqual(CollectionUtil.GetHashCode(control), CollectionUtil.GetHashCode(level1UnequalLevel2Equal));
        //    Assert.IsFalse(CollectionUtil.Equals(control, level1UnequalLevel2Equal));
        //}

        //[Test]
        //public void TestEqualityDictionaryDeep()
        //{
        //    var control = new Dictionary<string, IDictionary<HashMap<long, double>, string>>
        //    {
        //        { "a", new Dictionary<HashMap<long, double>, string> { { new HashMap<long, double> { { 123, 9.87 }, { 80, 88 } }, "qwerty" } } },
        //        { "z", new Dictionary<HashMap<long, double>, string> { { new HashMap<long, double> { { 456, 9.86 }, { 81, 88 } }, "hexagon" } } },
        //        { "r", new Dictionary<HashMap<long, double>, string> { { new HashMap<long, double> { { 789, 9.85 }, { 82, 88 } }, "parasite" } } },
        //        { "t", new Dictionary<HashMap<long, double>, string> { { new HashMap<long, double> { { 101, 9.84 }, { 83, 88 } }, "octopus" } } },
        //    };
        //    var equal = new Dictionary<string, IDictionary<HashMap<long, double>, string>>
        //    {
        //        { "a", new Dictionary<HashMap<long, double>, string> { { new HashMap<long, double> { { 123, 9.87 }, { 80, 88 } }, "qwerty" } } },
        //        { "z", new Dictionary<HashMap<long, double>, string> { { new HashMap<long, double> { { 456, 9.86 }, { 81, 88 } }, "hexagon" } } },
        //        { "r", new Dictionary<HashMap<long, double>, string> { { new HashMap<long, double> { { 789, 9.85 }, { 82, 88 } }, "parasite" } } },
        //        { "t", new Dictionary<HashMap<long, double>, string> { { new HashMap<long, double> { { 101, 9.84 }, { 83, 88 } }, "octopus" } } },
        //    };
        //    var equalDifferentType = new HashMap<string, IDictionary<HashMap<long, double>, string>>
        //    {
        //        { "a", new Dictionary<HashMap<long, double>, string> { { new HashMap<long, double> { { 123, 9.87 }, { 80, 88 } }, "qwerty" } } },
        //        { "z", new Dictionary<HashMap<long, double>, string> { { new HashMap<long, double> { { 456, 9.86 }, { 81, 88 } }, "hexagon" } } },
        //        { "r", new Dictionary<HashMap<long, double>, string> { { new HashMap<long, double> { { 789, 9.85 }, { 82, 88 } }, "parasite" } } },
        //        { "t", new Dictionary<HashMap<long, double>, string> { { new HashMap<long, double> { { 101, 9.84 }, { 83, 88 } }, "octopus" } } },
        //    };
        //    var equalDifferentOrder = new Dictionary<string, IDictionary<HashMap<long, double>, string>>
        //    {
        //        { "r", new Dictionary<HashMap<long, double>, string> { { new HashMap<long, double> { { 789, 9.85 }, { 82, 88 } }, "parasite" } } },
        //        { "t", new Dictionary<HashMap<long, double>, string> { { new HashMap<long, double> { { 101, 9.84 }, { 83, 88 } }, "octopus" } } },
        //        { "a", new Dictionary<HashMap<long, double>, string> { { new HashMap<long, double> { { 123, 9.87 }, { 80, 88 } }, "qwerty" } } },
        //        { "z", new Dictionary<HashMap<long, double>, string> { { new HashMap<long, double> { { 456, 9.86 }, { 81, 88 } }, "hexagon" } } },
        //    };

        //    var level1EqualLevel2EqualLevel3Unequal = new Dictionary<string, IDictionary<HashMap<long, double>, string>>
        //    {
        //        { "a", new Dictionary<HashMap<long, double>, string> { { new HashMap<long, double> { { 123, 9.87 }, { 80, 88.1 } }, "qwerty" } } },
        //        { "z", new Dictionary<HashMap<long, double>, string> { { new HashMap<long, double> { { 456, 9.86 }, { 81, 88 } }, "hexagon" } } },
        //        { "r", new Dictionary<HashMap<long, double>, string> { { new HashMap<long, double> { { 789, 9.85 }, { 82, 88 } }, "parasite" } } },
        //        { "t", new Dictionary<HashMap<long, double>, string> { { new HashMap<long, double> { { 101, 9.84 }, { 83, 88 } }, "octopus" } } },
        //    };

        //    Assert.AreEqual(CollectionUtil.GetHashCode(control, true), CollectionUtil.GetHashCode(control, true));
        //    Assert.AreEqual(CollectionUtil.GetHashCode(control, true), CollectionUtil.GetHashCode(equal, true));
        //    Assert.AreEqual(CollectionUtil.GetHashCode(control, true), CollectionUtil.GetHashCode(equalDifferentType, true));
        //    Assert.AreEqual(CollectionUtil.GetHashCode(control, true), CollectionUtil.GetHashCode(equalDifferentOrder, true));
        //    Assert.AreNotEqual(CollectionUtil.GetHashCode(control, true), CollectionUtil.GetHashCode(level1EqualLevel2EqualLevel3Unequal, true));
        //}

        //[Test]
        //public void TestEqualityListShallow()
        //{
        //    var control = new List<int> { 1, 2, 3, 4, 5 };
        //    var equal = new List<int> { 1, 2, 3, 4, 5 };
        //    var equalDifferentType = new int[] { 1, 2, 3, 4, 5 };
        //    var equalDifferentOrder = new List<int> { 1, 2, 3, 5, 4 };

        //    Assert.AreEqual(CollectionUtil.GetHashCode(control), CollectionUtil.GetHashCode(control));
        //    Assert.AreEqual(CollectionUtil.GetHashCode(control), CollectionUtil.GetHashCode(equal));
        //    Assert.AreEqual(CollectionUtil.GetHashCode(control), CollectionUtil.GetHashCode(equalDifferentType));
        //    // Lists and arrays are order-sensitive
        //    Assert.AreNotEqual(CollectionUtil.GetHashCode(control), CollectionUtil.GetHashCode(equalDifferentOrder));
        //}

        //[Test]
        //public void TestEqualityListDeep()
        //{
        //    var control = new List<IDictionary<string, string>>
        //    {
        //        new Dictionary<string, string> { { "1", "one" }, { "2", "two" }, { "3", "three" } },
        //        new Dictionary<string, string> { { "4", "four" }, { "5", "five" }, { "6", "six" } },
        //        new Dictionary<string, string> { { "7", "seven" }, { "8", "eight" }, { "9", "nine" } },
        //    };
        //    var equal = new List<IDictionary<string, string>>
        //    {
        //        new Dictionary<string, string> { { "1", "one" }, { "2", "two" }, { "3", "three" } },
        //        new Dictionary<string, string> { { "4", "four" }, { "5", "five" }, { "6", "six" } },
        //        new Dictionary<string, string> { { "7", "seven" }, { "8", "eight" }, { "9", "nine" } },
        //    };
        //    var equalDifferentType = new IDictionary<string, string>[]
        //    {
        //        new Dictionary<string, string> { { "1", "one" }, { "2", "two" }, { "3", "three" } },
        //        new Dictionary<string, string> { { "4", "four" }, { "5", "five" }, { "6", "six" } },
        //        new Dictionary<string, string> { { "7", "seven" }, { "8", "eight" }, { "9", "nine" } },
        //    };
        //    var equalDifferentOrder = new List<IDictionary<string, string>>
        //    {
        //        new Dictionary<string, string> { { "7", "seven" }, { "8", "eight" }, { "9", "nine" } },
        //        new Dictionary<string, string> { { "1", "one" }, { "2", "two" }, { "3", "three" } },
        //        new Dictionary<string, string> { { "4", "four" }, { "5", "five" }, { "6", "six" } },
        //    };
        //    var level1EqualLevel2Unequal = new List<IDictionary<string, string>>
        //    {
        //        new Dictionary<string, string> { { "1", "one" }, { "2", "two" }, { "3", "three" } },
        //        new Dictionary<string, string> { { "4", "four" }, { "5", "five" }, { "6", "six" } },
        //        new Dictionary<string, string> { { "7", "seven" }, { "8", "eight" }, { "9", "nine99" } },
        //    };

        //    Assert.AreEqual(CollectionUtil.GetHashCode(control, true), CollectionUtil.GetHashCode(control, true));
        //    Assert.AreEqual(CollectionUtil.GetHashCode(control, true), CollectionUtil.GetHashCode(equal, true));
        //    Assert.AreEqual(CollectionUtil.GetHashCode(control, true), CollectionUtil.GetHashCode(equalDifferentType, true));
        //    // Lists and arrays are order-sensitive
        //    Assert.AreNotEqual(CollectionUtil.GetHashCode(control, true), CollectionUtil.GetHashCode(equalDifferentOrder, true));
        //    Assert.AreNotEqual(CollectionUtil.GetHashCode(control, true), CollectionUtil.GetHashCode(level1EqualLevel2Unequal, true));
        //}

        //private class MockHashSet<T> : HashSet<T>
        //{
        //    public override int GetHashCode()
        //    {
        //        return Random().nextInt(); // Random garbage to ensure it is not equal
        //    }

        //    public override bool Equals(object obj)
        //    {
        //        return false;
        //    }
        //}

        //[Test]
        //public void TestEqualitySetShallow()
        //{
        //    var control = new HashSet<int> { 1, 2, 3, 4, 5 };
        //    var equal = new HashSet<int> { 1, 2, 3, 4, 5 };
        //    var equalDifferentType = new MockHashSet<int> { 1, 2, 3, 4, 5 };
        //    var equalDifferentOrder = new HashSet<int> { 1, 2, 3, 5, 4 };
        //    var missingItem = new HashSet<int> { 1, 2, 3, 5 };

        //    Assert.AreEqual(CollectionUtil.GetHashCode(control), CollectionUtil.GetHashCode(control));
        //    Assert.IsTrue(CollectionUtil.Equals(control, control));

        //    Assert.AreEqual(CollectionUtil.GetHashCode(control), CollectionUtil.GetHashCode(equal));
        //    Assert.IsTrue(CollectionUtil.Equals(control, equal));

        //    Assert.AreEqual(CollectionUtil.GetHashCode(control), CollectionUtil.GetHashCode(equalDifferentType));
        //    Assert.IsTrue(CollectionUtil.Equals(control, equalDifferentType));

        //    // sets are not order-sensitive
        //    Assert.AreEqual(CollectionUtil.GetHashCode(control), CollectionUtil.GetHashCode(equalDifferentOrder));
        //    Assert.IsTrue(CollectionUtil.Equals(control, equalDifferentOrder));

        //    Assert.AreNotEqual(CollectionUtil.GetHashCode(control), CollectionUtil.GetHashCode(missingItem));
        //    Assert.IsFalse(CollectionUtil.Equals(control, missingItem));
        //}

        //[Test]
        //public void TestEqualitySetDeep()
        //{
        //    var control = new HashSet<IDictionary<string, string>>
        //    {
        //        new Dictionary<string, string> { { "1", "one" }, { "2", "two" }, { "3", "three" } },
        //        new Dictionary<string, string> { { "4", "four" }, { "5", "five" }, { "6", "six" } },
        //        new Dictionary<string, string> { { "7", "seven" }, { "8", "eight" }, { "9", "nine" } },
        //    };
        //    var equal = new HashSet<IDictionary<string, string>>
        //    {
        //        new Dictionary<string, string> { { "1", "one" }, { "2", "two" }, { "3", "three" } },
        //        new Dictionary<string, string> { { "4", "four" }, { "5", "five" }, { "6", "six" } },
        //        new Dictionary<string, string> { { "7", "seven" }, { "8", "eight" }, { "9", "nine" } },
        //    };
        //    var equalDifferentType = new MockHashSet<IDictionary<string, string>>
        //    {
        //        new Dictionary<string, string> { { "1", "one" }, { "2", "two" }, { "3", "three" } },
        //        new Dictionary<string, string> { { "4", "four" }, { "5", "five" }, { "6", "six" } },
        //        new Dictionary<string, string> { { "7", "seven" }, { "8", "eight" }, { "9", "nine" } },
        //    };
        //    var equalDifferentOrder = new HashSet<IDictionary<string, string>>
        //    {
        //        new Dictionary<string, string> { { "7", "seven" }, { "8", "eight" }, { "9", "nine" } },
        //        new Dictionary<string, string> { { "1", "one" }, { "2", "two" }, { "3", "three" } },
        //        new Dictionary<string, string> { { "4", "four" }, { "5", "five" }, { "6", "six" } },
        //    };
        //    var level1EqualLevel2Unequal = new HashSet<IDictionary<string, string>>
        //    {
        //        new Dictionary<string, string> { { "1", "one" }, { "2", "two" }, { "3", "three" } },
        //        new Dictionary<string, string> { { "4", "four" }, { "5", "five" }, { "6", "six" } },
        //        new Dictionary<string, string> { { "7", "seven" }, { "8", "eight" }, { "9", "nine99" } },
        //    };

        //    Assert.AreEqual(CollectionUtil.GetHashCode(control, true), CollectionUtil.GetHashCode(control, true));
        //    Assert.AreEqual(CollectionUtil.GetHashCode(control, true), CollectionUtil.GetHashCode(equal, true));
        //    Assert.AreEqual(CollectionUtil.GetHashCode(control, true), CollectionUtil.GetHashCode(equalDifferentType, true));
        //    // Sets are not order-sensitive
        //    Assert.AreEqual(CollectionUtil.GetHashCode(control, true), CollectionUtil.GetHashCode(equalDifferentOrder, true));
        //    Assert.AreNotEqual(CollectionUtil.GetHashCode(control, true), CollectionUtil.GetHashCode(level1EqualLevel2Unequal, true));
        //}

    }
}
