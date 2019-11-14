using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace J2N.Text
{
    [TestFixture]
    public class TestStringExtensions
    {
        [Test]
        public void TestToCharSequence()
        {
            string target = "This is a test";

            var result = target.ToCharSequence();

            Assert.IsNotNull(result);
            Assert.AreEqual(typeof(StringCharSequence), result.GetType());
        }

        [Test]
        public void TestSubsequence()
        {
            string target = "This is a test";

            Assert.AreEqual("This is a test", target.Subsequence(0, target.Length));
            Assert.AreEqual("is a", target.Subsequence(5, 4));
            Assert.AreEqual("", target.Subsequence(4, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => target.Subsequence(-1, 10));
            Assert.Throws<ArgumentOutOfRangeException>(() => target.Subsequence(3, -2));
            Assert.Throws<ArgumentOutOfRangeException>(() => target.Subsequence(target.Length, 1));

            string emptyTarget = string.Empty;

            Assert.Throws<ArgumentOutOfRangeException>(() => emptyTarget.Subsequence(0, 1));

            string nullTarget = null;

            Assert.IsFalse(nullTarget.Subsequence(6, 10).HasValue); // Null target will always return null subsequence
        }

        [Test]
        public void TestCompareToOrdinal()
        {
            string target = null;
            string compareTo = "Alpine";

            Assert.Greater(0, target.CompareToOrdinal(compareTo.ToCharArray()));
            Assert.Greater(0, target.CompareToOrdinal(new StringBuilder(compareTo)));
            Assert.Greater(0, target.CompareToOrdinal(compareTo));
            Assert.Greater(0, target.CompareToOrdinal(new CharArrayCharSequence(compareTo.ToCharArray())));
            Assert.Greater(0, target.CompareToOrdinal(new StringBuilderCharSequence(new StringBuilder(compareTo))));
            Assert.Greater(0, target.CompareToOrdinal(new StringCharSequence(compareTo)));

            target = "Alpha";

            Assert.Greater(0, target.CompareToOrdinal(compareTo.ToCharArray()));
            Assert.Greater(0, target.CompareToOrdinal(new StringBuilder(compareTo)));
            Assert.Greater(0, target.CompareToOrdinal(compareTo));
            Assert.Greater(0, target.CompareToOrdinal(new CharArrayCharSequence(compareTo.ToCharArray())));
            Assert.Greater(0, target.CompareToOrdinal(new StringBuilderCharSequence(new StringBuilder(compareTo))));
            Assert.Greater(0, target.CompareToOrdinal(new StringCharSequence(compareTo)));

            compareTo = "Alpha";

            Assert.AreEqual(0, target.CompareToOrdinal(compareTo.ToCharArray()));
            Assert.AreEqual(0, target.CompareToOrdinal(new StringBuilder(compareTo)));
            Assert.AreEqual(0, target.CompareToOrdinal(compareTo));
            Assert.AreEqual(0, target.CompareToOrdinal(new CharArrayCharSequence(compareTo.ToCharArray())));
            Assert.AreEqual(0, target.CompareToOrdinal(new StringBuilderCharSequence(new StringBuilder(compareTo))));
            Assert.AreEqual(0, target.CompareToOrdinal(new StringCharSequence(compareTo)));

            compareTo = "Alp";

            Assert.Less(0, target.CompareToOrdinal(compareTo.ToCharArray()));
            Assert.Less(0, target.CompareToOrdinal(new StringBuilder(compareTo)));
            Assert.Less(0, target.CompareToOrdinal(compareTo));
            Assert.Less(0, target.CompareToOrdinal(new CharArrayCharSequence(compareTo.ToCharArray())));
            Assert.Less(0, target.CompareToOrdinal(new StringBuilderCharSequence(new StringBuilder(compareTo))));
            Assert.Less(0, target.CompareToOrdinal(new StringCharSequence(compareTo)));


            Assert.Less(0, target.CompareToOrdinal((char[])null));
            Assert.Less(0, target.CompareToOrdinal((StringBuilder)null));
            Assert.Less(0, target.CompareToOrdinal((string)null));
            Assert.Less(0, target.CompareToOrdinal(new CharArrayCharSequence(null)));
            Assert.Less(0, target.CompareToOrdinal(new StringBuilderCharSequence(null)));
            Assert.Less(0, target.CompareToOrdinal(new StringCharSequence(null)));
        }
    }
}
