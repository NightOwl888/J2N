using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace J2N.Collections.Generic
{
    public class TestSetEqualityComparer : TestCase
    {
        private class MockHashSet<T> : HashSet<T>
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
            var control = new HashSet<IDictionary<string, string>>
                    {
                        new Dictionary<string, string> { { "1", "one" }, { "2", "two" }, { "3", "three" } },
                        new Dictionary<string, string> { { "4", "four" }, { "5", "five" }, { "6", "six" } },
                        new Dictionary<string, string> { { "7", "seven" }, { "8", "eight" }, { "9", "nine" } },
                    };
            var equal = new HashSet<IDictionary<string, string>>
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
            var equalDifferentOrder = new HashSet<IDictionary<string, string>>
                    {
                        new Dictionary<string, string> { { "7", "seven" }, { "8", "eight" }, { "9", "nine" } },
                        new Dictionary<string, string> { { "1", "one" }, { "2", "two" }, { "3", "three" } },
                        new Dictionary<string, string> { { "4", "four" }, { "5", "five" }, { "6", "six" } },
                    };
            var level1EqualLevel2Unequal = new HashSet<IDictionary<string, string>>
                    {
                        new Dictionary<string, string> { { "1", "one" }, { "2", "two" }, { "3", "three" } },
                        new Dictionary<string, string> { { "4", "four" }, { "5", "five" }, { "6", "six" } },
                        new Dictionary<string, string> { { "7", "seven" }, { "8", "eight" }, { "9", "nine99" } },
                    };

            var comparer = SetEqualityComparer<IDictionary<string, string>>.Aggressive;

            Assert.AreEqual(comparer.GetHashCode(control), comparer.GetHashCode(control));
            Assert.IsTrue(comparer.Equals(control, control));

            Assert.AreEqual(comparer.GetHashCode(control), comparer.GetHashCode(equal));
            Assert.IsTrue(comparer.Equals(control, equal));

            Assert.AreEqual(comparer.GetHashCode(control), comparer.GetHashCode(equalDifferentType));
            Assert.IsTrue(comparer.Equals(control, equalDifferentType));

            // Sets are not order-sensitive
            Assert.AreEqual(comparer.GetHashCode(control), comparer.GetHashCode(equalDifferentOrder));
            Assert.IsTrue(comparer.Equals(control, equalDifferentOrder));

            Assert.AreNotEqual(comparer.GetHashCode(control), comparer.GetHashCode(level1EqualLevel2Unequal));
            Assert.IsFalse(comparer.Equals(control, level1EqualLevel2Unequal));
        }
    }
}
