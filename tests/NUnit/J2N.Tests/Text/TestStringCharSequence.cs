using NUnit.Framework;
using System.Text;
#nullable enable

namespace J2N.Text
{
    internal class TestStringCharSequence : CharSequenceTestBase<StringCharSequence>
    {
        public override StringCharSequence CreateClassUnderTest(string? value)
            => CharSequenceUtil.CreateStringCharSequence(value);

        [Test]
        public void Test_Value()
        {
            var target = CreateClassUnderTest(String1);

            Assert.IsNotNull(target.Value);
            Assert.AreEqual(String1, target.Value);

            Assert.IsNull(CreateClassUnderTest(null).Value);
        }

        [Test]
        public virtual void Test_EqualityOperators()
        {
            var target = CreateClassUnderTest(String1);
            var equalTarget = CreateClassUnderTest(String1);
            var unequalTarget = CreateClassUnderTest(String2);
            var nullTarget = CreateClassUnderTest(null);

            Assert.IsTrue(target == equalTarget);
            Assert.IsTrue(equalTarget == target);

            Assert.IsFalse(target == unequalTarget);
            Assert.IsFalse(unequalTarget == target);

            Assert.IsTrue(target == String1);
            Assert.IsTrue(String1 == target);

            Assert.IsFalse(target == String2);
            Assert.IsFalse(String2 == target);

            Assert.IsTrue(nullTarget == (string?)null);
            Assert.IsTrue((string?)null == nullTarget);


            Assert.IsFalse(target != equalTarget);
            Assert.IsFalse(equalTarget != target);

            Assert.IsTrue(target != unequalTarget);
            Assert.IsTrue(unequalTarget != target);

            Assert.IsTrue(target != String2);
            Assert.IsTrue(String2 != target);

            Assert.IsFalse(target != String1);
            Assert.IsFalse(String1 != target);

            Assert.IsFalse(nullTarget != (string?)null);
            Assert.IsFalse((string?)null != nullTarget);
        }


        [TestCaseSource(typeof(CharSequenceUtil), nameof(CharSequenceUtil.Equals_Object_TestData))]
        public void Test_Equals_Object(string? leftValue, object? rightValue, bool expected)
        {
            try
            {
                Assert.AreEqual(expected, CreateClassUnderTest(leftValue).Equals(rightValue));
            }
            finally
            {
                CharSequenceUtil.Dispose(rightValue);
            }
        }

        [TestCaseSource(typeof(CharSequenceUtil), nameof(CharSequenceUtil.Equals_ICharSequence_TestData))]
        public void Test_Equals_ICharSequence(string? leftValue, ICharSequence? rightValue, bool expected)
        {
            try
            {
                Assert.AreEqual(expected, CreateClassUnderTest(leftValue).Equals(rightValue));
            }
            finally
            {
                CharSequenceUtil.Dispose(rightValue);
            }
        }

        [TestCaseSource(typeof(CharSequenceUtil), nameof(CharSequenceUtil.Equals_StringCharSequence_TestData))]
        public void Test_Equals_StringCharSequence(string? leftValue, StringCharSequence? rightValue, bool expected)
        {
            try
            {
                Assert.AreEqual(expected, CreateClassUnderTest(leftValue).Equals(rightValue));
            }
            finally
            {
                CharSequenceUtil.Dispose(rightValue);
            }
        }

        [TestCaseSource(typeof(CharSequenceUtil), nameof(CharSequenceUtil.Equals_CharArrayCharSequence_TestData))]
        public void Test_Equals_CharArrayCharSequence(string? leftValue, CharArrayCharSequence? rightValue, bool expected)
        {
            try
            {
                Assert.AreEqual(expected, CreateClassUnderTest(leftValue).Equals(rightValue));
            }
            finally
            {
                CharSequenceUtil.Dispose(rightValue);
            }
        }

        [TestCaseSource(typeof(CharSequenceUtil), nameof(CharSequenceUtil.Equals_StringBuilderCharSequence_TestData))]
        public void Test_Equals_StringBuilderCharSequence(string? leftValue, StringBuilderCharSequence? rightValue, bool expected)
        {
            try
            {
                Assert.AreEqual(expected, CreateClassUnderTest(leftValue).Equals(rightValue));
            }
            finally
            {
                CharSequenceUtil.Dispose(rightValue);
            }
        }


        [TestCaseSource(typeof(CharSequenceUtil), nameof(CharSequenceUtil.Equals_String_TestData))]
        public void Test_Equals_String(string? leftValue, string? rightValue, bool expected)
        {
            try
            {
                Assert.AreEqual(expected, CreateClassUnderTest(leftValue).Equals(rightValue));
            }
            finally
            {
                CharSequenceUtil.Dispose(rightValue);
            }
        }

        [TestCaseSource(typeof(CharSequenceUtil), nameof(CharSequenceUtil.Equals_CharArray_TestData))]
        public void Test_Equals_CharArray(string? leftValue, char[]? rightValue, bool expected)
        {
            try
            {
                Assert.AreEqual(expected, CreateClassUnderTest(leftValue).Equals(rightValue));
            }
            finally
            {
                CharSequenceUtil.Dispose(rightValue);
            }
        }

        [TestCaseSource(typeof(CharSequenceUtil), nameof(CharSequenceUtil.Equals_StringBuilder_TestData))]
        public void Test_Equals_StringBuilder(string? leftValue, StringBuilder? rightValue, bool expected)
        {
            try
            {
                Assert.AreEqual(expected, CreateClassUnderTest(leftValue).Equals(rightValue));
            }
            finally
            {
                CharSequenceUtil.Dispose(rightValue);
            }
        }


        [TestCaseSource(typeof(CharSequenceUtil), nameof(CharSequenceUtil.CompareTo_Object_TestData))]
        public void Test_CompareTo_Object(string? leftValue, object? rightValue, int expected)
        {
            try
            {
#if FEATURE_BROKEN_NULL_CHARSEQUENCE_COMPARISON
                if (leftValue is null)
                {
                    if (rightValue is ICharSequence cs2 && !cs2.HasValue)
                        expected = -1; // J2N TODO: Fix broken null comparison (should be 0)
                }
#endif

                CharSequenceUtil.AssertCompareTo(expected, CreateClassUnderTest(leftValue).CompareTo(rightValue));
            }
            finally
            {
                CharSequenceUtil.Dispose(rightValue);
            }
        }

        [TestCaseSource(typeof(CharSequenceUtil), nameof(CharSequenceUtil.CompareTo_ICharSequence_TestData))]
        public void Test_CompareTo_ICharSequence(string? leftValue, ICharSequence? rightValue, int expected)
        {
            try
            {
                CharSequenceUtil.AssertCompareTo(expected, CreateClassUnderTest(leftValue).CompareTo(rightValue));
            }
            finally
            {
                CharSequenceUtil.Dispose(rightValue);
            }
        }

        [TestCaseSource(typeof(CharSequenceUtil), nameof(CharSequenceUtil.CompareTo_StringCharSequence_TestData))]
        public void Test_CompareTo_StringCharSequence(string? leftValue, StringCharSequence? rightValue, int expected)
        {
            try
            {
                CharSequenceUtil.AssertCompareTo(expected, CreateClassUnderTest(leftValue).CompareTo(rightValue));
            }
            finally
            {
                CharSequenceUtil.Dispose(rightValue);
            }
        }

        [TestCaseSource(typeof(CharSequenceUtil), nameof(CharSequenceUtil.CompareTo_CharArrayCharSequence_TestData))]
        public void Test_CompareTo_CharArrayCharSequence(string? leftValue, CharArrayCharSequence? rightValue, int expected)
        {
            try
            {
                CharSequenceUtil.AssertCompareTo(expected, CreateClassUnderTest(leftValue).CompareTo(rightValue));
            }
            finally
            {
                CharSequenceUtil.Dispose(rightValue);
            }
        }

        [TestCaseSource(typeof(CharSequenceUtil), nameof(CharSequenceUtil.CompareTo_StringBuilderCharSequence_TestData))]
        public void Test_CompareTo_StringBuilderCharSequence(string? leftValue, StringBuilderCharSequence? rightValue, int expected)
        {
            try
            {
                CharSequenceUtil.AssertCompareTo(expected, CreateClassUnderTest(leftValue).CompareTo(rightValue));
            }
            finally
            {
                CharSequenceUtil.Dispose(rightValue);
            }
        }

        [TestCaseSource(typeof(CharSequenceUtil), nameof(CharSequenceUtil.CompareTo_String_TestData))]
        public void Test_CompareTo_String(string? leftValue, string? rightValue, int expected)
        {
            try
            {
                CharSequenceUtil.AssertCompareTo(expected, CreateClassUnderTest(leftValue).CompareTo(rightValue));
            }
            finally
            {
                CharSequenceUtil.Dispose(rightValue);
            }
        }

        [TestCaseSource(typeof(CharSequenceUtil), nameof(CharSequenceUtil.CompareTo_CharArray_TestData))]
        public void Test_CompareTo_CharArray(string? leftValue, char[]? rightValue, int expected)
        {
            try
            {
                CharSequenceUtil.AssertCompareTo(expected, CreateClassUnderTest(leftValue).CompareTo(rightValue));
            }
            finally
            {
                CharSequenceUtil.Dispose(rightValue);
            }
        }

        [TestCaseSource(typeof(CharSequenceUtil), nameof(CharSequenceUtil.CompareTo_StringBuilder_TestData))]
        public void Test_CompareTo_StringBuilder(string? leftValue, StringBuilder? rightValue, int expected)
        {
            try
            {
                CharSequenceUtil.AssertCompareTo(expected, CreateClassUnderTest(leftValue).CompareTo(rightValue));
            }
            finally
            {
                CharSequenceUtil.Dispose(rightValue);
            }
        }


        [TestCaseSource(typeof(CharSequenceUtil), nameof(CharSequenceUtil.GetHashCode_String_TestData))]
        public void Test_GetHashCode(string? value, int expected)
        {
            Assert.AreEqual(expected, CreateClassUnderTest(value).GetHashCode());
        }
    }
}
