using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace J2N.Collections.Generic
{
    public class TestSortedDictionary : TestCase
    {
        private SortedDictionary<string, string> dict;


        public override void SetUp()
        {
            base.SetUp();
            dict = new SortedDictionary<string, string>(StringComparer.Ordinal);
        }

        public override void TearDown()
        {
            dict = null;
            base.TearDown();
        }

        [Test]
        public void TestTryGetPredecessor()
        {
            dict.Add("A", "1");
            dict.Add("C", "2");
            dict.Add("E", "3");

            Assert.IsTrue(dict.TryGetPredecessor("B", out KeyValuePair<string, string> res));
            Assert.AreEqual("1", res.Value);
            Assert.IsTrue(dict.TryGetPredecessor("C", out res));
            Assert.AreEqual("1", res.Value);

            Assert.IsFalse(dict.TryGetPredecessor("A", out res));
            Assert.AreEqual(null, res.Key);
            Assert.AreEqual(null, res.Value);
        }

        [Test]
        public void TestTryGetSuccessor()
        {
            dict.Add("A", "1");
            dict.Add("C", "2");
            dict.Add("E", "3");

            Assert.IsTrue(dict.TryGetSuccessor("B", out KeyValuePair<string, string> res));
            Assert.AreEqual("2", res.Value);
            Assert.IsTrue(dict.TryGetSuccessor("C", out res));
            Assert.AreEqual("3", res.Value);

            Assert.IsFalse(dict.TryGetSuccessor("E", out res));
            Assert.AreEqual(null, res.Key);
            Assert.AreEqual(null, res.Value);
        }
    }
}
