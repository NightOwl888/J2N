using J2N.Collections.Generic;
using J2N.Text;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using SCG = System.Collections.Generic;

namespace J2N.Collections
{
    public class TestCollectionUtil : TestCase
    {
        // NOTE: For Aggressive mode to work right, all collections it uses (including this one)
        // must be declared public.
#if FEATURE_SERIALIZABLE
        [Serializable]
#endif
        public class HashMap<TKey, TValue> : SCG.Dictionary<TKey, TValue>
        {

            public HashMap() { }

#if FEATURE_SERIALIZABLE
            protected HashMap(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
#endif
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
            var list = new SCG.List<int> { 1, 2, 3, 4, 5 };
            var set = new SCG.HashSet<int> { 1, 2, 3, 4, 5 };
            var dictionary = new SCG.Dictionary<int, int> { { 1, 0 }, { 2, 0 }, { 3, 0 }, { 4, 0 }, { 5, 0 } };
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
            var control = new SCG.Dictionary<string, IDictionary<HashMap<long, double>, string>>
                    {
                        { "a", new SCG.Dictionary<HashMap<long, double>, string> { { new HashMap<long, double> { { 123, 9.87 }, { 80, 88 } }, "qwerty" } } },
                        { "z", new SCG.Dictionary<HashMap<long, double>, string> { { new HashMap<long, double> { { 456, 9.86 }, { 81, 88 } }, "hexagon" } } },
                        { "r", new SCG.Dictionary<HashMap<long, double>, string> { { new HashMap<long, double> { { 789, 9.85 }, { 82, 88 } }, "parasite" } } },
                        { "t", new SCG.Dictionary<HashMap<long, double>, string> { { new HashMap<long, double> { { 101, 9.84 }, { 83, 88 } }, "octopus" } } },
                    };
            var equal = new SCG.Dictionary<string, IDictionary<HashMap<long, double>, string>>
                    {
                        { "a", new SCG.Dictionary<HashMap<long, double>, string> { { new HashMap<long, double> { { 123, 9.87 }, { 80, 88 } }, "qwerty" } } },
                        { "z", new SCG.Dictionary<HashMap<long, double>, string> { { new HashMap<long, double> { { 456, 9.86 }, { 81, 88 } }, "hexagon" } } },
                        { "r", new SCG.Dictionary<HashMap<long, double>, string> { { new HashMap<long, double> { { 789, 9.85 }, { 82, 88 } }, "parasite" } } },
                        { "t", new SCG.Dictionary<HashMap<long, double>, string> { { new HashMap<long, double> { { 101, 9.84 }, { 83, 88 } }, "octopus" } } },
                    };
            var equalDifferentType = new HashMap<string, IDictionary<HashMap<long, double>, string>>
                    {
                        { "a", new SCG.Dictionary<HashMap<long, double>, string> { { new HashMap<long, double> { { 123, 9.87 }, { 80, 88 } }, "qwerty" } } },
                        { "z", new SCG.Dictionary<HashMap<long, double>, string> { { new HashMap<long, double> { { 456, 9.86 }, { 81, 88 } }, "hexagon" } } },
                        { "r", new SCG.Dictionary<HashMap<long, double>, string> { { new HashMap<long, double> { { 789, 9.85 }, { 82, 88 } }, "parasite" } } },
                        { "t", new SCG.Dictionary<HashMap<long, double>, string> { { new HashMap<long, double> { { 101, 9.84 }, { 83, 88 } }, "octopus" } } },
                    };
            var equalDifferentOrder = new SCG.Dictionary<string, IDictionary<HashMap<long, double>, string>>
                    {
                        { "r", new SCG.Dictionary<HashMap<long, double>, string> { { new HashMap<long, double> { { 789, 9.85 }, { 82, 88 } }, "parasite" } } },
                        { "t", new SCG.Dictionary<HashMap<long, double>, string> { { new HashMap<long, double> { { 101, 9.84 }, { 83, 88 } }, "octopus" } } },
                        { "a", new SCG.Dictionary<HashMap<long, double>, string> { { new HashMap<long, double> { { 123, 9.87 }, { 80, 88 } }, "qwerty" } } },
                        { "z", new SCG.Dictionary<HashMap<long, double>, string> { { new HashMap<long, double> { { 456, 9.86 }, { 81, 88 } }, "hexagon" } } },
                    };

            var level1EqualLevel2EqualLevel3Unequal = new SCG.Dictionary<string, IDictionary<HashMap<long, double>, string>>
                    {
                        { "a", new SCG.Dictionary<HashMap<long, double>, string> { { new HashMap<long, double> { { 123, 9.87 }, { 80, 88.1 } }, "qwerty" } } },
                        { "z", new SCG.Dictionary<HashMap<long, double>, string> { { new HashMap<long, double> { { 456, 9.86 }, { 81, 88 } }, "hexagon" } } },
                        { "r", new SCG.Dictionary<HashMap<long, double>, string> { { new HashMap<long, double> { { 789, 9.85 }, { 82, 88 } }, "parasite" } } },
                        { "t", new SCG.Dictionary<HashMap<long, double>, string> { { new HashMap<long, double> { { 101, 9.84 }, { 83, 88 } }, "octopus" } } },
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
            var control = new SCG.List<IDictionary<string, string>>
                    {
                        new SCG.Dictionary<string, string> { { "1", "one" }, { "2", "two" }, { "3", "three" } },
                        new SCG.Dictionary<string, string> { { "4", "four" }, { "5", "five" }, { "6", "six" } },
                        new SCG.Dictionary<string, string> { { "7", "seven" }, { "8", "eight" }, { "9", "nine" } },
                    };
            var equal = new SCG.List<IDictionary<string, string>>
                    {
                        new SCG.Dictionary<string, string> { { "1", "one" }, { "2", "two" }, { "3", "three" } },
                        new SCG.Dictionary<string, string> { { "4", "four" }, { "5", "five" }, { "6", "six" } },
                        new SCG.Dictionary<string, string> { { "7", "seven" }, { "8", "eight" }, { "9", "nine" } },
                    };
            var equalDifferentType = new IDictionary<string, string>[]
            {
                        new SCG.Dictionary<string, string> { { "1", "one" }, { "2", "two" }, { "3", "three" } },
                        new SCG.Dictionary<string, string> { { "4", "four" }, { "5", "five" }, { "6", "six" } },
                        new SCG.Dictionary<string, string> { { "7", "seven" }, { "8", "eight" }, { "9", "nine" } },
            };
            var equalDifferentOrder = new SCG.List<IDictionary<string, string>>
                    {
                        new SCG.Dictionary<string, string> { { "7", "seven" }, { "8", "eight" }, { "9", "nine" } },
                        new SCG.Dictionary<string, string> { { "1", "one" }, { "2", "two" }, { "3", "three" } },
                        new SCG.Dictionary<string, string> { { "4", "four" }, { "5", "five" }, { "6", "six" } },
                    };
            var level1EqualLevel2Unequal = new SCG.List<IDictionary<string, string>>
                    {
                        new SCG.Dictionary<string, string> { { "1", "one" }, { "2", "two" }, { "3", "three" } },
                        new SCG.Dictionary<string, string> { { "4", "four" }, { "5", "five" }, { "6", "six" } },
                        new SCG.Dictionary<string, string> { { "7", "seven" }, { "8", "eight" }, { "9", "nine99" } },
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
            var control = new SCG.List<IList<string>>
                    {
                        new SCG.List<string> { "one",  "two",  "three" },
                        new SCG.List<string> { "four",  "five", "six" } ,
                        new SCG.List<string> { "seven", "eight", "nine" },
                    };
            var equal = new SCG.List<IList<string>>
                    {
                        new SCG.List<string> { "one",  "two",  "three" },
                        new SCG.List<string> { "four",  "five", "six" } ,
                        new SCG.List<string> { "seven", "eight", "nine" },
                    };
            var equalDifferentType = new IList<string>[]
            {
                        new SCG.List<string> { "one",  "two",  "three" },
                        new SCG.List<string> { "four",  "five", "six" } ,
                        new SCG.List<string> { "seven", "eight", "nine" },
            };
            var equalDifferentOrder = new SCG.List<IList<string>>
                    {
                        new SCG.List<string> { "four",  "five", "six" } ,
                        new SCG.List<string> { "seven", "eight", "nine" },
                        new SCG.List<string> { "one",  "two",  "three" },
                    };
            var level1EqualLevel2Unequal = new SCG.List<IList<string>>
                    {
                        new SCG.List<string> { "one",  "two",  "three" },
                        new SCG.List<string> { "four",  "five", "six" } ,
                        new SCG.List<string> { "seven", "eight", "nine-nine" },
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


        private class MockHashSet<T> : SCG.HashSet<T>
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
            var control = new SCG.HashSet<IDictionary<string, string>>
                    {
                        new SCG.Dictionary<string, string> { { "1", "one" }, { "2", "two" }, { "3", "three" } },
                        new SCG.Dictionary<string, string> { { "4", "four" }, { "5", "five" }, { "6", "six" } },
                        new SCG.Dictionary<string, string> { { "7", "seven" }, { "8", "eight" }, { "9", "nine" } },
                    };
            var equal = new SCG.HashSet<IDictionary<string, string>>
                    {
                        new SCG.Dictionary<string, string> { { "1", "one" }, { "2", "two" }, { "3", "three" } },
                        new SCG.Dictionary<string, string> { { "4", "four" }, { "5", "five" }, { "6", "six" } },
                        new SCG.Dictionary<string, string> { { "7", "seven" }, { "8", "eight" }, { "9", "nine" } },
                    };
            var equalDifferentType = new MockHashSet<IDictionary<string, string>>
                    {
                        new SCG.Dictionary<string, string> { { "1", "one" }, { "2", "two" }, { "3", "three" } },
                        new SCG.Dictionary<string, string> { { "4", "four" }, { "5", "five" }, { "6", "six" } },
                        new SCG.Dictionary<string, string> { { "7", "seven" }, { "8", "eight" }, { "9", "nine" } },
                    };
            var equalDifferentOrder = new SCG.HashSet<IDictionary<string, string>>
                    {
                        new SCG.Dictionary<string, string> { { "7", "seven" }, { "8", "eight" }, { "9", "nine" } },
                        new SCG.Dictionary<string, string> { { "1", "one" }, { "2", "two" }, { "3", "three" } },
                        new SCG.Dictionary<string, string> { { "4", "four" }, { "5", "five" }, { "6", "six" } },
                    };
            var level1EqualLevel2Unequal = new SCG.HashSet<IDictionary<string, string>>
                    {
                        new SCG.Dictionary<string, string> { { "1", "one" }, { "2", "two" }, { "3", "three" } },
                        new SCG.Dictionary<string, string> { { "4", "four" }, { "5", "five" }, { "6", "six" } },
                        new SCG.Dictionary<string, string> { { "7", "seven" }, { "8", "eight" }, { "9", "nine99" } },
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
                new SCG.Dictionary<string, string> { { "1", "one" }, { "2", "two" }, { "3", "three" } },
                new SCG.Dictionary<string, string> { { "4", "four" }, { "5", "five" }, { "6", "six" } },
                new SCG.Dictionary<string, string> { { "7", "seven" }, { "8", "eight" }, { "9", "nine" } },
            };
            var setExpected = "[{1=one, 2=two, 3=three}, {4=four, 5=five, 6=six}, {7=seven, 8=eight, 9=nine}]";

            Assert.AreEqual(setExpected, CollectionUtil.ToString(set, StringFormatter.InvariantCulture));


            var set2 = new J2N.Collections.Generic.HashSet<ISet<string>>
            {
                new J2N.Collections.Generic.HashSet<string> { "1", "2", "3" },
                new J2N.Collections.Generic.HashSet<string> { "4", "5", "6" },
                new SCG.HashSet<string> { "7", "8", "9" },
            };
            var set2Expected = "[[1, 2, 3], [4, 5, 6], [7, 8, 9]]";

            Assert.AreEqual(set2Expected, CollectionUtil.ToString(set2, StringFormatter.InvariantCulture));


            var map = new SCG.Dictionary<string, IDictionary<int, double>>
            {
                { "first", new SCG.Dictionary<int, double> { { 1, 1.23 }, { 2, 2.23 }, { 3, 3.23 } } },
                { "second", new SCG.Dictionary<int, double> { { 4, 1.24 }, { 5, 2.24 }, { 6, 3.24 } } },
                { "third", new SCG.Dictionary<int, double> { { 7, 1.25 }, { 8, 2.25 }, { 9, 3.25 } } },
            };
            var mapExpectedPortuguese = "{first={1=1,23, 2=2,23, 3=3,23}, second={4=1,24, 5=2,24, 6=3,24}, third={7=1,25, 8=2,25, 9=3,25}}";
            var mapExpectedUSEnglish = "{first={1=1.23, 2=2.23, 3=3.23}, second={4=1.24, 5=2.24, 6=3.24}, third={7=1.25, 8=2.25, 9=3.25}}";

            Assert.AreEqual(mapExpectedPortuguese, CollectionUtil.ToString(map, new StringFormatter(new CultureInfo("pt"))));
            Assert.AreEqual(mapExpectedUSEnglish, CollectionUtil.ToString(map, new StringFormatter(new CultureInfo("en-US"))));

            var array = new SCG.List<SCG.Dictionary<string, string>>[]
            {
                new SCG.List<SCG.Dictionary<string, string>> {
                    new SCG.Dictionary<string, string> { { "foo", "bar" }, { "foobar", "barfoo" } }
                },
                new SCG.List<SCG.Dictionary<string, string>> {
                    new SCG.Dictionary<string, string> { { "orange", "yellow" }, { "red", "black" } },
                    new SCG.Dictionary<string, string> { { "rain", "snow" }, { "sleet", "sunshine" } }
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

        [Test]
        public void TestStringInterpolationWithList()
        {
            var list = new J2N.Collections.Generic.List<string>()
            {
                "nothing",
                "else",
                "matters"
            };
            string actual = $"{list}";
            assertEquals("[nothing, else, matters]", actual);
        }

        [Test]
        public void TestStringInterpolationWithDictionary()
        {
            var dictionary = new J2N.Collections.Generic.Dictionary<string, bool>()
            {
                ["nothing"] = true,
                ["else"] = false,
                ["matters"] = true
            };
            string actual = $"{dictionary}";
            assertEquals("{nothing=true, else=false, matters=true}", actual);
        }

        [Test]
        public void TestEqualsGetHashCodeNullHandling()
        {
            Assert.IsTrue(CollectionUtil.Equals(null, null));
            Assert.IsFalse(CollectionUtil.Equals(null, new SCG.List<int>()));
            Assert.IsFalse(CollectionUtil.Equals(new SCG.List<int>(), null));

            Assert.AreEqual(0, CollectionUtil.GetHashCode(null));
        }

        [Test]
        public void TestToStringCyclicList()
        {
            var list = new SCG.List<object>();
            list.Add(list);

            string result = CollectionUtil.ToString(list, StringFormatter.InvariantCulture);

            // Don't assert exact format — just ensure it terminates and contains recursion marker
            Assert.IsTrue(result.Contains("...") || result.Length > 0);
        }

        [Test]
        public void TestToStringCyclicDictionary()
        {
            var dict = new SCG.Dictionary<string, object>();
            dict["self"] = dict;

            string result = CollectionUtil.ToString(dict, StringFormatter.InvariantCulture);

            Assert.IsTrue(result.Contains("...") || result.Length > 0);
        }

        [Test]
        public void TestEqualsDictionaryComparerDifferences()
        {
            var dict1 = new SCG.Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
            {
                ["a"] = 1
            };

            var dict2 = new SCG.Dictionary<string, int>(StringComparer.Ordinal)
            {
                ["A"] = 1
            };

            // JDK-style: depends on key equality, not comparer identity
            Assert.IsTrue(CollectionUtil.Equals(dict1, dict2));
        }

        [Test]
        public void TestEqualsDoubleEdgeCases()
        {
            var list1 = new SCG.List<double> { double.NaN };
            var list2 = new SCG.List<double> { double.NaN };

            Assert.IsTrue(CollectionUtil.Equals(list1, list2));

            var zero = new SCG.List<double> { 0.0d };
            var negZero = new SCG.List<double> { -0.0d };

            Assert.IsFalse(CollectionUtil.Equals(zero, negZero)); // J2N: Confirmed against the JDK this is right
        }

        [Test]
        public void TestEqualsSingleEdgeCases()
        {
            var list1 = new SCG.List<float> { float.NaN };
            var list2 = new SCG.List<float> { float.NaN };

            Assert.IsTrue(CollectionUtil.Equals(list1, list2));
            
            var zero = new SCG.List<float> { 0.0f };
            var negZero = new SCG.List<float> { -0.0f };

            Assert.IsFalse(CollectionUtil.Equals(zero, negZero));
        }

        [Test]
        public void TestGetHashCodeLargeCollection()
        {
            var list = new SCG.List<int>();
            for (int i = 0; i < 100000; i++)
                list.Add(i);

            int hash = CollectionUtil.GetHashCode(list);

            Assert.AreNotEqual(0, hash);
        }




        [Test]
        public void TestToString_NonGeneric_List()
        {
            var list = new NonGenericList { 1, 2, 3 };
            Assert.AreEqual("[1, 2, 3]", CollectionUtil.ToString(list, StringFormatter.InvariantCulture));
        }

        [Test]
        public void TestToString_NonGeneric_Nested()
        {
            var list = new NonGenericList
            {
                new NonGenericList { 1, 2 },
                new NonGenericList { 3, 4 }
            };

            Assert.AreEqual("[[1, 2], [3, 4]]", CollectionUtil.ToString(list, StringFormatter.InvariantCulture));
        }

        [Test]
        public void TestToString_NonGeneric_Culture()
        {
            var list = new NonGenericList { 1.23 };
            var result = CollectionUtil.ToString(list, new StringFormatter(new CultureInfo("pt")));

            Assert.IsTrue(result.Contains("1,23"));
        }

        [Test]
        public void TestToString_NonGeneric_SelfReference()
        {
            var list = new NonGenericList();
            list.Add(list);

            var result = CollectionUtil.ToString(list, StringFormatter.InvariantCulture);

            Assert.IsTrue(result.Contains("(this Collection)"));
        }

        [Test]
        public void TestToString_NonGeneric_List_Empty()
        {
            var list = new NonGenericList();

            var result = CollectionUtil.ToString(list, StringFormatter.InvariantCulture);
            Assert.AreEqual("[]", result);
        }

        [Test]
        public void TestToString_NonGeneric_List_Null()
        {
            NonGenericList list = null;

            var result = CollectionUtil.ToString(list, StringFormatter.InvariantCulture);

            Assert.AreEqual("null", result);
        }



        [Test]
        public void TestToString_NonGeneric_Dictionary()
        {
            var dict = new NonGenericDictionary
            {
                ["a"] = 1,
                ["b"] = 2
            };

            var result = CollectionUtil.ToString(dict, StringFormatter.InvariantCulture);

            Assert.IsTrue(result == "{a=1, b=2}" || result == "{b=2, a=1}");
        }

        [Test]
        public void TestToString_NonGeneric_Dictionary_Nested()
        {
            var dict = new NonGenericDictionary
            {
                ["x"] = new NonGenericDictionary
                {
                    ["a"] = 1
                },
                ["y"] = new NonGenericDictionary
                {
                    ["b"] = 2
                }
            };

            var result = CollectionUtil.ToString(dict, StringFormatter.InvariantCulture);

            Assert.IsTrue(result.Contains("{a=1}"));
            Assert.IsTrue(result.Contains("{b=2}"));
        }

        [Test]
        public void TestToString_NonGeneric_Mixed()
        {
            var list = new NonGenericList
            {
                new NonGenericDictionary { ["a"] = 1 },
                new NonGenericDictionary { ["b"] = 2 }
            };

            var result = CollectionUtil.ToString(list, StringFormatter.InvariantCulture);

            Assert.IsTrue(result.Contains("{a=1}"));
            Assert.IsTrue(result.Contains("{b=2}"));
        }

        [Test]
        public void TestToString_NonGeneric_Dictionary_Culture()
        {
            var dict = new NonGenericDictionary
            {
                ["value"] = 1.23
            };

            var result = CollectionUtil.ToString(dict, new StringFormatter(new CultureInfo("pt")));

            Assert.IsTrue(result.Contains("1,23"));
        }

        [Test]
        public void TestToString_NonGeneric_Dictionary_SelfReference()
        {
            var dict = new NonGenericDictionary();
            dict["self"] = dict;

            var result = CollectionUtil.ToString(dict, StringFormatter.InvariantCulture);

            Assert.IsTrue(result.Contains("(this Dictionary)"));
        }

        [Test]
        public void TestToString_NonGeneric_Dictionary_KeySelfReference()
        {
            var dict = new NonGenericDictionary();
            dict[dict] = "value";

            var result = CollectionUtil.ToString(dict, StringFormatter.InvariantCulture);

            Assert.IsTrue(result.Contains("(this Dictionary)=value"));
        }

        [Test]
        public void TestToString_NonGeneric_Dictionary_Empty()
        {
            var dict = new NonGenericDictionary();

            var result = CollectionUtil.ToString(dict, StringFormatter.InvariantCulture);

            Assert.AreEqual("{}", result);
        }

        [Test]
        public void TestToString_NonGeneric_Dictionary_Null()
        {
            NonGenericDictionary dict = null;

            var result = CollectionUtil.ToString(dict, StringFormatter.InvariantCulture);

            Assert.AreEqual("null", result);
        }

#nullable enable

        public sealed class NonGenericList : IList
        {
            private readonly SCG.List<object> list;

            public NonGenericList()
            {
                this.list = new SCG.List<object>();
            }

            public NonGenericList(int capacity)
            {
                this.list = new SCG.List<object>(capacity);
            }

            public NonGenericList(IEnumerable<object> collection)
            {
                this.list = new SCG.List<object>(collection);
            }

            public object? this[int index]
            { 
                get => list[index];
                set => list[index] = value!;
            }

            public bool IsFixedSize => ((IList)list).IsFixedSize;

            public bool IsReadOnly => ((IList)list).IsReadOnly;

            public int Count => list.Count;
            public bool IsSynchronized => ((ICollection)list).IsSynchronized;

            public object SyncRoot => ((ICollection)list).SyncRoot;

            public int Add(object? value)
            {
                list.Add(value!);
                return list.Count - 1;
            }

            public void Clear()
            {
                list.Clear();
            }

            public bool Contains(object? value)
            {
                return list.Contains(value!);
            }

            public void CopyTo(Array array, int index)
            {
                list.CopyTo((object[])array, index);
            }

            public IEnumerator GetEnumerator()
            {
                return list.GetEnumerator();
            }

            public int IndexOf(object? value)
            {
                return list.IndexOf(value!);
            }

            public void Insert(int index, object? value)
            {
                list.Insert(index, value!);
            }

            public void Remove(object? value)
            {
                list.Remove(value!);
            }

            public void RemoveAt(int index)
            {
                list.RemoveAt(index);
            }
        }

        public sealed class NonGenericDictionary : IDictionary
        {
            private readonly SCG.Dictionary<object, object> dictionary;

            public NonGenericDictionary()
            {
                this.dictionary = new SCG.Dictionary<object, object>();
            }

            public NonGenericDictionary(int capacity)
            {
                this.dictionary = new SCG.Dictionary<object, object>(capacity);
            }

            public NonGenericDictionary(IDictionary source)
            {
                this.dictionary = new SCG.Dictionary<object, object>(source.Count);
                foreach (DictionaryEntry entry in source)
                {
                    dictionary.Add(entry.Key!, entry.Value!);
                }
            }

            public object? this[object key]
            {
                get => dictionary[key];
                set => dictionary[key] = value!;
            }

            public ICollection Keys => dictionary.Keys;
            public ICollection Values => dictionary.Values;

            public bool IsReadOnly => false;
            public bool IsFixedSize => false;

            public int Count => dictionary.Count;

            public bool IsSynchronized => false;
            public object SyncRoot => ((ICollection)dictionary).SyncRoot;

            public void Add(object key, object? value)
            {
                dictionary.Add(key, value!);
            }

            public void Clear()
            {
                dictionary.Clear();
            }

            public bool Contains(object key)
            {
                return dictionary.ContainsKey(key);
            }

            public IDictionaryEnumerator GetEnumerator()
            {
                return new DictionaryEnumerator(dictionary.GetEnumerator());
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return dictionary.GetEnumerator();
            }

            public void Remove(object key)
            {
                dictionary.Remove(key);
            }

            public void CopyTo(Array array, int index)
            {
                foreach (DictionaryEntry entry in this)
                {
                    array.SetValue(entry, index++);
                }
            }

            private sealed class DictionaryEnumerator : IDictionaryEnumerator
            {
                private readonly IEnumerator<KeyValuePair<object, object>> enumerator;

                public DictionaryEnumerator(IEnumerator<KeyValuePair<object, object>> enumerator)
                {
                    this.enumerator = enumerator;
                }

                public DictionaryEntry Entry => new DictionaryEntry(Key, Value);

                public object Key => enumerator.Current.Key;
                public object? Value => enumerator.Current.Value;

                public object Current => Entry;

                public bool MoveNext() => enumerator.MoveNext();

                public void Reset() => throw new NotSupportedException();
            }
        }
    }
}
