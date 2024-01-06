using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace J2N.Text
{
    [TestFixture]
    public class TestCharArrayExtensions
    {
        [Test]
        public void TestAsCharSequence()
        {
            char[] target = "This is a test".ToCharArray();

            var result = target.AsCharSequence();

            Assert.IsNotNull(result);
            Assert.AreEqual(typeof(CharArrayCharSequence), result.GetType());
        }

        [Test]
        public void TestSubsequence()
        {
            char[] target = "This is a test".ToCharArray();

            Assert.AreEqual("This is a test", target.Subsequence(0, target.Length));
            Assert.AreEqual("is a", target.Subsequence(5, 4));
            Assert.AreEqual("", target.Subsequence(4, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => target.Subsequence(-1, 10));
            Assert.Throws<ArgumentOutOfRangeException>(() => target.Subsequence(3, -2));
            Assert.Throws<ArgumentOutOfRangeException>(() => target.Subsequence(target.Length, 1));

            char[] emptyTarget = new char[0];

            Assert.Throws<ArgumentOutOfRangeException>(() => emptyTarget.Subsequence(0, 1));

            char[] nullTarget = null;

            Assert.IsFalse(nullTarget.Subsequence(6, 10).HasValue); // Null target will always return null subsequence
        }

        [Test]
        public void TestCompareToOrdinal()
        {
            char[] target = null;
            string compareTo = "Alpine";

#if FEATURE_SPAN
            Assert.Greater(0, target.CompareToOrdinal(compareTo.AsSpan()));
#endif
            Assert.Greater(0, target.CompareToOrdinal(compareTo.ToCharArray()));
            Assert.Greater(0, target.CompareToOrdinal(new StringBuilder(compareTo)));
            Assert.Greater(0, target.CompareToOrdinal(compareTo));
            Assert.Greater(0, target.CompareToOrdinal(new CharArrayCharSequence(compareTo.ToCharArray())));
            Assert.Greater(0, target.CompareToOrdinal(new StringBuilderCharSequence(new StringBuilder(compareTo))));
            Assert.Greater(0, target.CompareToOrdinal(new StringCharSequence(compareTo)));

            target = "Alpha".ToCharArray();

#if FEATURE_SPAN
            Assert.Greater(0, target.CompareToOrdinal(compareTo.AsSpan()));
#endif
            Assert.Greater(0, target.CompareToOrdinal(compareTo.ToCharArray()));
            Assert.Greater(0, target.CompareToOrdinal(new StringBuilder(compareTo)));
            Assert.Greater(0, target.CompareToOrdinal(compareTo));
            Assert.Greater(0, target.CompareToOrdinal(new CharArrayCharSequence(compareTo.ToCharArray())));
            Assert.Greater(0, target.CompareToOrdinal(new StringBuilderCharSequence(new StringBuilder(compareTo))));
            Assert.Greater(0, target.CompareToOrdinal(new StringCharSequence(compareTo)));

            compareTo = "Alpha";

#if FEATURE_SPAN
            Assert.AreEqual(0, target.CompareToOrdinal(compareTo.AsSpan()));
#endif
            Assert.AreEqual(0, target.CompareToOrdinal(compareTo.ToCharArray()));
            Assert.AreEqual(0, target.CompareToOrdinal(new StringBuilder(compareTo)));
            Assert.AreEqual(0, target.CompareToOrdinal(compareTo));
            Assert.AreEqual(0, target.CompareToOrdinal(new CharArrayCharSequence(compareTo.ToCharArray())));
            Assert.AreEqual(0, target.CompareToOrdinal(new StringBuilderCharSequence(new StringBuilder(compareTo))));
            Assert.AreEqual(0, target.CompareToOrdinal(new StringCharSequence(compareTo)));

            compareTo = "Alp";

#if FEATURE_SPAN
            Assert.Less(0, target.CompareToOrdinal(compareTo.AsSpan()));
#endif
            Assert.Less(0, target.CompareToOrdinal(compareTo.ToCharArray()));
            Assert.Less(0, target.CompareToOrdinal(new StringBuilder(compareTo)));
            Assert.Less(0, target.CompareToOrdinal(compareTo));
            Assert.Less(0, target.CompareToOrdinal(new CharArrayCharSequence(compareTo.ToCharArray())));
            Assert.Less(0, target.CompareToOrdinal(new StringBuilderCharSequence(new StringBuilder(compareTo))));
            Assert.Less(0, target.CompareToOrdinal(new StringCharSequence(compareTo)));

#if FEATURE_SPAN
            Assert.Less(0, target.CompareToOrdinal((ReadOnlySpan<char>)null));
#endif
            Assert.Less(0, target.CompareToOrdinal((char[])null));
            Assert.Less(0, target.CompareToOrdinal((StringBuilder)null));
            Assert.Less(0, target.CompareToOrdinal((string)null));
            Assert.Less(0, target.CompareToOrdinal(new CharArrayCharSequence(null)));
            Assert.Less(0, target.CompareToOrdinal(new StringBuilderCharSequence(null)));
            Assert.Less(0, target.CompareToOrdinal(new StringCharSequence(null)));

            target = null;

#if FEATURE_SPAN
            Assert.AreEqual(0, target.CompareToOrdinal((ReadOnlySpan<char>)null));
#endif
            Assert.AreEqual(0, target.CompareToOrdinal((char[])null));
            Assert.AreEqual(0, target.CompareToOrdinal((StringBuilder)null));
            Assert.AreEqual(0, target.CompareToOrdinal((string)null));
            Assert.AreEqual(0, target.CompareToOrdinal(new CharArrayCharSequence(null)));
            Assert.AreEqual(0, target.CompareToOrdinal(new StringBuilderCharSequence(null)));
            Assert.AreEqual(0, target.CompareToOrdinal(new StringCharSequence(null)));
        }
    }
}
