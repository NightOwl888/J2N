using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace J2N.Collections.Generic
{
    public class TestDictionaryEqualityComparer : TestCase
    {
        // NOTE: For Aggressive mode to work right, all collections it uses (including this one)
        // must be declared public.
#if FEATURE_SERIALIZABLE
        [Serializable]
#endif
        public class HashMap<TKey, TValue> : Dictionary<TKey, TValue>
        {
            public HashMap() { }


#if FEATURE_SERIALIZABLE
            protected HashMap(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
#endif

            // TODO: Aggressive mode is currently broken for dictionaries - we shouldn't have
            // to override these methods for this to work with collection key types.
            // However, since Lucene.Net doesn't use aggressive comparison of dictionaries, this has
            // been put off until later.
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
        public void TestEqualityDictionary_Aggressive()
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

            Assert.AreEqual(DictionaryEqualityComparer<string, IDictionary<HashMap<long, double>, string>>.Aggressive.GetHashCode(control), 
                            DictionaryEqualityComparer<string, IDictionary<HashMap<long, double>, string>>.Aggressive.GetHashCode(control));
            Assert.IsTrue(DictionaryEqualityComparer<string, IDictionary<HashMap<long, double>, string>>.Aggressive.Equals(control, control));

            Assert.AreEqual(DictionaryEqualityComparer<string, IDictionary<HashMap<long, double>, string>>.Aggressive.GetHashCode(control), 
                            DictionaryEqualityComparer<string, IDictionary<HashMap<long, double>, string>>.Aggressive.GetHashCode(equal));
            Assert.IsTrue(DictionaryEqualityComparer<string, IDictionary<HashMap<long, double>, string>>.Aggressive.Equals(control, equal));

            Assert.AreEqual(DictionaryEqualityComparer<string, IDictionary<HashMap<long, double>, string>>.Aggressive.GetHashCode(control), 
                            DictionaryEqualityComparer<string, IDictionary<HashMap<long, double>, string>>.Aggressive.GetHashCode(equalDifferentType));
            Assert.IsTrue(DictionaryEqualityComparer<string, IDictionary<HashMap<long, double>, string>>.Aggressive.Equals(control, equalDifferentType));

            Assert.AreEqual(DictionaryEqualityComparer<string, IDictionary<HashMap<long, double>, string>>.Aggressive.GetHashCode(control),
                            DictionaryEqualityComparer<string, IDictionary<HashMap<long, double>, string>>.Aggressive.GetHashCode(equalDifferentOrder));
            Assert.IsTrue(DictionaryEqualityComparer<string, IDictionary<HashMap<long, double>, string>>.Aggressive.Equals(control, equalDifferentOrder));

            Assert.AreNotEqual(DictionaryEqualityComparer<string, IDictionary<HashMap<long, double>, string>>.Aggressive.GetHashCode(control), 
                               DictionaryEqualityComparer<string, IDictionary<HashMap<long, double>, string>>.Aggressive.GetHashCode(level1EqualLevel2EqualLevel3Unequal));
            Assert.IsFalse(DictionaryEqualityComparer<string, IDictionary<HashMap<long, double>, string>>.Aggressive.Equals(control, level1EqualLevel2EqualLevel3Unequal));
        }

        [Test]
        public void TestEqualityDictionaryShallow_Aggressive()
        {
            var control = new Dictionary<string, IDictionary<int, string>>
            {
                { "a", new Dictionary<int, string> { { 9, "qwerty" } } },
                { "z", new Dictionary<int, string> { { 23, "hexagon" } } },
                { "r", new Dictionary<int, string> { { 4, "parasite" } } },
                { "t", new Dictionary<int, string> { { 61, "octopus" } } },
            };
            var equal = new Dictionary<string, IDictionary<int, string>>
            {
                { "a", new Dictionary<int, string> { { 9, "qwerty" } } },
                { "z", new Dictionary<int, string> { { 23, "hexagon" } } },
                { "r", new Dictionary<int, string> { { 4, "parasite" } } },
                { "t", new Dictionary<int, string> { { 61, "octopus" } } },
            };
            var equalDifferentType = new HashMap<string, IDictionary<int, string>>
            {
                { "a", new Dictionary<int, string> { { 9, "qwerty" } } },
                { "z", new Dictionary<int, string> { { 23, "hexagon" } } },
                { "r", new Dictionary<int, string> { { 4, "parasite" } } },
                { "t", new Dictionary<int, string> { { 61, "octopus" } } },
            };
            var equalDifferentOrder = new Dictionary<string, IDictionary<int, string>>
            {
                { "r", new Dictionary<int, string> { { 4, "parasite" } } },
                { "t", new Dictionary<int, string> { { 61, "octopus" } } },
                { "a", new Dictionary<int, string> { { 9, "qwerty" } } },
                { "z", new Dictionary<int, string> { { 23, "hexagon" } } },
            };
            var level1EqualLevel2Unequal = new Dictionary<string, IDictionary<int, string>>
            {
                { "a", new Dictionary<int, string> { { 9, "qwerty" } } },
                { "z", new Dictionary<int, string> { { 23, "hexagon" } } },
                { "r", new Dictionary<int, string> { { 4, "parasite" } } },
                { "t", new Dictionary<int, string> { { 7, "octopus" } } },
            };
            var level1UnequalLevel2Equal = new Dictionary<string, IDictionary<int, string>>
            {
                { "y", new Dictionary<int, string> { { 9, "qwerty" } } },
                { "z", new Dictionary<int, string> { { 23, "hexagon" } } },
                { "r", new Dictionary<int, string> { { 4, "parasite" } } },
                { "t", new Dictionary<int, string> { { 61, "octopus" } } },
            };

            var comparer = DictionaryEqualityComparer<string, IDictionary<int, string>>.Aggressive;

            Assert.AreEqual(comparer.GetHashCode(control), comparer.GetHashCode(control));
            Assert.IsTrue(comparer.Equals(control, control));

            Assert.AreEqual(comparer.GetHashCode(control), comparer.GetHashCode(equal));
            Assert.IsTrue(comparer.Equals(control, equal));

            Assert.AreEqual(comparer.GetHashCode(control), comparer.GetHashCode(equalDifferentType));
            Assert.IsTrue(comparer.Equals(control, equalDifferentType));

            Assert.AreEqual(comparer.GetHashCode(control), comparer.GetHashCode(equalDifferentOrder));
            Assert.IsTrue(comparer.Equals(control, equalDifferentOrder));

            Assert.AreNotEqual(comparer.GetHashCode(control), comparer.GetHashCode(level1EqualLevel2Unequal));
            Assert.IsFalse(comparer.Equals(control, level1EqualLevel2Unequal));

            Assert.AreNotEqual(comparer.GetHashCode(control), comparer.GetHashCode(level1UnequalLevel2Equal));
            Assert.IsFalse(comparer.Equals(control, level1UnequalLevel2Equal));
        }

        [Test]
        public void TestEqualityDictionaryDeep_Aggressive()
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

            var comparer = DictionaryEqualityComparer<string, IDictionary<HashMap<long, double>, string>>.Aggressive;

            Assert.AreEqual(comparer.GetHashCode(control), comparer.GetHashCode(control));
            Assert.AreEqual(comparer.GetHashCode(control), comparer.GetHashCode(equal));
            Assert.AreEqual(comparer.GetHashCode(control), comparer.GetHashCode(equalDifferentType));
            Assert.AreEqual(comparer.GetHashCode(control), comparer.GetHashCode(equalDifferentOrder));
            Assert.AreNotEqual(comparer.GetHashCode(control), comparer.GetHashCode(level1EqualLevel2EqualLevel3Unequal));
        }
    }
}
