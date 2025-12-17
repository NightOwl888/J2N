// Based on: https://github.com/sestoft/C5/blob/master/C5.Tests/Trees/Dictionary.cs#L72-L126
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
        public void TestTryGetPredecessor_KeyValuePair()
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
        public void TestTryGetSuccessor_KeyValuePair()
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

        [Test]
        public void TestTryGetPredecessor()
        {
            dict.Add("A", "1");
            dict.Add("C", "2");
            dict.Add("E", "3");

            Assert.IsTrue(dict.TryGetPredecessor("B", out _, out string value));
            Assert.AreEqual("1", value);
            Assert.IsTrue(dict.TryGetPredecessor("C", out _, out value));
            Assert.AreEqual("1", value);

            Assert.IsFalse(dict.TryGetPredecessor("A", out string key, out value));
            Assert.AreEqual(null, key);
            Assert.AreEqual(null, value);
        }

        [Test]
        public void TestTryGetSuccessor()
        {
            dict.Add("A", "1");
            dict.Add("C", "2");
            dict.Add("E", "3");

            Assert.IsTrue(dict.TryGetSuccessor("B", out _, out string value));
            Assert.AreEqual("2", value);
            Assert.IsTrue(dict.TryGetSuccessor("C", out _, out value));
            Assert.AreEqual("3", value);

            Assert.IsFalse(dict.TryGetSuccessor("E", out string key, out value));
            Assert.AreEqual(null, key);
            Assert.AreEqual(null, value);
        }

        [Test]
        public void TestTryGetFloor()
        {
            dict.Add("A", "1");
            dict.Add("C", "2");
            dict.Add("E", "3");

            Assert.IsTrue(dict.TryGetFloor("B", out _, out string value));
            Assert.AreEqual("1", value);
            Assert.IsTrue(dict.TryGetFloor("C", out _, out value));
            Assert.AreEqual("2", value);

            Assert.IsFalse(dict.TryGetFloor("@", out string key, out value));
            Assert.AreEqual(null, key);
            Assert.AreEqual(null, value);
        }

        [Test]
        public void TestTryGetCeiling()
        {
            dict.Add("A", "1");
            dict.Add("C", "2");
            dict.Add("E", "3");

            Assert.IsTrue(dict.TryGetCeiling("B", out _, out string value));
            Assert.AreEqual("2", value);
            Assert.IsTrue(dict.TryGetCeiling("C", out _, out value));
            Assert.AreEqual("2", value);

            Assert.IsFalse(dict.TryGetCeiling("F", out string key, out value));
            Assert.AreEqual(null, key);
            Assert.AreEqual(null, value);
        }
    }
}
