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

#nullable enable

        private static char[]? CreateClassUnderTest(string? value) => CharSequenceUtil.CreateCharArray(value);



        [Test]
        public void Test_CompareToOridnal_ICharSequence_SynchronizedTextBuilderCharSequence_SynchronizesWhileReading()
        {
            const string Value = "abcdefghijklmnopqrstuvwxyz";

            var target = CreateClassUnderTest(Value);
            var other = CharSequenceUtil.CreateSynchronizedTextBuilderCharSequence(Value);

            CharSequenceUtil.AssertSynchronizesWhileReading(other.Value!, () => Assert.AreEqual(0, target.CompareToOrdinal((ICharSequence?)other)));
        }

        [Test]
        public void Test_CompareToOrdinal_ICharSequence_StringBuffer_SynchronizesWhileReading()
        {
            const string Value = "abcdefghijklmnopqrstuvwxyz";

            var target = CreateClassUnderTest(Value);
            var other = CharSequenceUtil.CreateStringBuffer(Value)!;

            CharSequenceUtil.AssertSynchronizesWhileReading(other, () => Assert.AreEqual(0, target.CompareToOrdinal((ICharSequence?)other)));
        }

        [TestCaseSource(typeof(CharSequenceUtil), nameof(CharSequenceUtil.CompareTo_ICharSequence_TestData))]
        public void Test_CompareToOrdinal_ICharSequence(string? leftValue, ICharSequence? rightValue, int expected)
        {
            try
            {
                Assert.AreEqual(expected, CreateClassUnderTest(leftValue).CompareToOrdinal(rightValue));
            }
            finally
            {
                CharSequenceUtil.Dispose(rightValue);
            }
        }

        [TestCaseSource(typeof(CharSequenceUtil), nameof(CharSequenceUtil.CompareTo_String_TestData))]
        public void Test_CompareToOrdinal_ReadOnlySpan(string? leftValue, string? rightValue, int expected)
        {
            try
            {
                if (rightValue is null)
                {
                    Assert.Inconclusive("Cannot test null ReadOnlySpan<char> since it is a value type and cannot be null.");
                    return;
                }
#if FEATURE_BROKEN_NULL_CHARSEQUENCE_COMPARISON
                if (leftValue is null)
                {
                    expected = 0; // J2N TODO: Fix broken null comparison (should be -1)
                }
#endif

                Assert.AreEqual(expected, CreateClassUnderTest(leftValue).CompareToOrdinal(rightValue.AsSpan()));
            }
            finally
            {
                CharSequenceUtil.Dispose(rightValue);
            }
        }

        [TestCaseSource(typeof(CharSequenceUtil), nameof(CharSequenceUtil.CompareTo_String_TestData))]
        public void Test_CompareToOrdinal_String(string? leftValue, string? rightValue, int expected)
        {
            try
            {
                Assert.AreEqual(expected, CreateClassUnderTest(leftValue).CompareToOrdinal(rightValue));
            }
            finally
            {
                CharSequenceUtil.Dispose(rightValue);
            }
        }

        [TestCaseSource(typeof(CharSequenceUtil), nameof(CharSequenceUtil.CompareTo_CharArray_TestData))]
        public void Test_CompareToOrdinal_CharArray(string? leftValue, char[]? rightValue, int expected)
        {
            try
            {
                Assert.AreEqual(expected, CreateClassUnderTest(leftValue).CompareToOrdinal(rightValue));
            }
            finally
            {
                CharSequenceUtil.Dispose(rightValue);
            }
        }

        [TestCaseSource(typeof(CharSequenceUtil), nameof(CharSequenceUtil.CompareTo_StringBuilder_TestData))]
        public void Test_CompareToOrdinal_StringBuilder(string? leftValue, StringBuilder? rightValue, int expected)
        {
            try
            {
                Assert.AreEqual(expected, CreateClassUnderTest(leftValue).CompareToOrdinal(rightValue));
            }
            finally
            {
                CharSequenceUtil.Dispose(rightValue);
            }
        }

#nullable restore
    }
}
