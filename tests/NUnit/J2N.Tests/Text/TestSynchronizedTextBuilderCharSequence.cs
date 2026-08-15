#region Copyright 2019-2026 by Shad Storhaug, Licensed under the Apache License, Version 2.0
/*  Licensed to the Apache Software Foundation (ASF) under one or more
 *  contributor license agreements.  See the NOTICE file distributed with
 *  this work for additional information regarding copyright ownership.
 *  The ASF licenses this file to You under the Apache License, Version 2.0
 *  (the "License"); you may not use this file except in compliance with
 *  the License.  You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 *  Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *  limitations under the License.
 */
#endregion

using NUnit.Framework;
using System;
using System.Text;
#nullable enable

namespace J2N.Text
{
    internal class TestSynchronizedTextBuilderCharSequence : CharSequenceTestBase<SynchronizedTextBuilderCharSequence>
    {
        public override SynchronizedTextBuilderCharSequence CreateClassUnderTest(string? value)
            => CharSequenceUtil.CreateSynchronizedTextBuilderCharSequence(value);

        [Test]
        public void Test_Value()
        {
            var target = CreateClassUnderTest(String1);

            Assert.IsNotNull(target.Value);
            Assert.AreEqual(String1, target.Value!.ToString());

            Assert.IsNull(CreateClassUnderTest(null).Value);
        }

        //[Test]
        //public virtual void Test_EqualityOperators()
        //{
        //    Assert.IsTrue(target == equalTarget);
        //    Assert.IsTrue(equalTarget == target);

        //    Assert.IsFalse(target == unequalTarget);
        //    Assert.IsFalse(unequalTarget == target);

        //    Assert.IsTrue(target == String1);
        //    Assert.IsTrue(String1 == target);

        //    Assert.IsFalse(target == String2);
        //    Assert.IsFalse(String2 == target);

        //    Assert.IsTrue(nullTarget == (string)null);
        //    Assert.IsTrue((string)null == nullTarget);


        //    Assert.IsFalse(target != equalTarget);
        //    Assert.IsFalse(equalTarget != target);

        //    Assert.IsTrue(target != unequalTarget);
        //    Assert.IsTrue(unequalTarget != target);

        //    Assert.IsTrue(target != String2);
        //    Assert.IsTrue(String2 != target);

        //    Assert.IsFalse(target != String1);
        //    Assert.IsFalse(String1 != target);

        //    Assert.IsFalse(nullTarget != (string)null);
        //    Assert.IsFalse((string)null != nullTarget);
        //}

        [Test]
        public void Test_Equals_ICharSequence_SynchronizedTextBuilderCharSequence_ShouldNotDeadlock_WhenComparingOppositeDirections()
        {
            const string Value = "abcdefghijklmnopqrstuvwxyz";

            AssertExtensions.AssertNoABDeadlock(
                () => CreateClassUnderTest(Value),
                () => CharSequenceUtil.CreateSynchronizedTextBuilderCharSequence(Value),
                (seq1, seq2) =>
                {
                    Assert.IsTrue(seq1.Equals(seq2));
                });
        }

        [Test]
        public void Test_Equals_ICharSequence_StringBuffer_ShouldNotDeadlock_WhenComparingOppositeDirections()
        {
            const string Value = "abcdefghijklmnopqrstuvwxyz";

            AssertExtensions.AssertNoABDeadlock(
                () => CreateClassUnderTest(Value),
                () => CharSequenceUtil.CreateStringBuffer(Value),
                (seq1, seq2) =>
                {
                    Assert.IsTrue(seq1.Equals(seq2));
                });
        }

        [Test]
        public void Test_Equals_Object_SynchronizedTextBuilderCharSequence_ShouldNotDeadlock_WhenComparingOppositeDirections()
        {
            const string Value = "abcdefghijklmnopqrstuvwxyz";

            AssertExtensions.AssertNoABDeadlock(
                () => CreateClassUnderTest(Value),
                () => CharSequenceUtil.CreateSynchronizedTextBuilderCharSequence(Value),
                (seq1, seq2) =>
                {
                    Assert.IsTrue(seq1.Equals((object?)seq2));
                });
        }

        [Test]
        public void Test_Equals_Object_SynchronizedTextBuilder_ShouldNotDeadlock_WhenComparingOppositeDirections()
        {
            const string Value = "abcdefghijklmnopqrstuvwxyz";

            AssertExtensions.AssertNoABDeadlock(
                () => CreateClassUnderTest(Value),
                () => CharSequenceUtil.CreateSynchronizedTextBuilder(Value),
                (seq1, sb2) =>
                {
                    Assert.IsTrue(seq1.Equals((object?)sb2));
                });
        }

        [Test]
        public void Test_Equals_Object_StringBuffer_ShouldNotDeadlock_WhenComparingOppositeDirections()
        {
            const string Value = "abcdefghijklmnopqrstuvwxyz";

            AssertExtensions.AssertNoABDeadlock(
                () => CreateClassUnderTest(Value),
                () => CharSequenceUtil.CreateStringBuffer(Value),
                (seq1, sb2) =>
                {
                    Assert.IsTrue(seq1.Equals((object?)sb2));
                });
        }

        [Test]
        public void Test_Equals_ICharSequence_SynchronizedTextBuilderCharSequence_SynchronizesWhileReading()
        {
            var target = CreateClassUnderTest(String1);
            var other = CharSequenceUtil.CreateSynchronizedTextBuilderCharSequence(String1);

            CharSequenceUtil.AssertSynchronizesWhileReading(other.Value!, () => Assert.IsTrue(target.Equals((ICharSequence?)other)));
        }

        [Test]
        public void Test_Equals_ICharSequence_StringBuffer_SynchronizesWhileReading()
        {
            var target = CreateClassUnderTest(String1);
            var other = CharSequenceUtil.CreateStringBuffer(String1)!;

            CharSequenceUtil.AssertSynchronizesWhileReading(other, () => Assert.IsTrue(target.Equals((ICharSequence?)other)));
        }

        [Test]
        public void Test_Equals_Object_SynchronizedTextBuilderCharSequence_SynchronizesWhileReading()
        {
            var target = CreateClassUnderTest(String1);
            var other = CharSequenceUtil.CreateSynchronizedTextBuilderCharSequence(String1);

            CharSequenceUtil.AssertSynchronizesWhileReading(other.Value!, () => Assert.IsTrue(target.Equals((object?)other)));
        }

        [Test]
        public void Test_Equals_Object_SynchronizedTextBuilder_SynchronizesWhileReading()
        {
            var target = CreateClassUnderTest(String1);
            var other = CharSequenceUtil.CreateSynchronizedTextBuilder(String1)!;

            CharSequenceUtil.AssertSynchronizesWhileReading(other, () => Assert.IsTrue(target.Equals((object?)other)));
        }

        [Test]
        public void Test_Equals_Object_WithMatchingFormattedInteger_ReturnsFalse()
        {
            int value = 123;
            var target = CreateClassUnderTest(value.ToString());
            Assert.IsFalse(target.Equals(value));
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


        [Test]
        public void Test_CompareTo_ICharSequence_SynchronizedTextBuilderCharSequence_ShouldNotDeadlock_WhenComparingOppositeDirections()
        {
            const string Value = "abcdefghijklmnopqrstuvwxyz";

            AssertExtensions.AssertNoABDeadlock(
                () => CreateClassUnderTest(Value),
                () => CharSequenceUtil.CreateSynchronizedTextBuilderCharSequence(Value),
                (seq1, seq2) =>
                {
                    Assert.AreEqual(0, seq1.CompareTo(seq2));
                });
        }

        [Test]
        public void Test_CompareTo_ICharSequence_StringBuffer_ShouldNotDeadlock_WhenComparingOppositeDirections()
        {
            const string Value = "abcdefghijklmnopqrstuvwxyz";

            AssertExtensions.AssertNoABDeadlock(
                () => CreateClassUnderTest(Value),
                () => CharSequenceUtil.CreateStringBuffer(Value),
                (seq1, seq2) =>
                {
                    Assert.AreEqual(0, seq1.CompareTo(seq2));
                });
        }

        [Test]
        public void Test_CompareTo_Object_SynchronizedTextBuilderCharSequence_ShouldNotDeadlock_WhenComparingOppositeDirections()
        {
            const string Value = "abcdefghijklmnopqrstuvwxyz";

            AssertExtensions.AssertNoABDeadlock(
                () => CreateClassUnderTest(Value),
                () => CharSequenceUtil.CreateSynchronizedTextBuilderCharSequence(Value),
                (seq1, seq2) =>
                {
                    Assert.AreEqual(0, seq1.CompareTo((object?)seq2));
                });
        }

        [Test]
        public void Test_CompareTo_Object_SynchronizedTextBuilder_ShouldNotDeadlock_WhenComparingOppositeDirections()
        {
            const string Value = "abcdefghijklmnopqrstuvwxyz";

            AssertExtensions.AssertNoABDeadlock(
                () => CreateClassUnderTest(Value),
                () => CharSequenceUtil.CreateSynchronizedTextBuilder(Value),
                (seq1, sb2) =>
                {
                    Assert.AreEqual(0, seq1.CompareTo((object?)sb2));
                });
        }

        [Test]
        public void Test_CompareTo_Object_StringBuffer_ShouldNotDeadlock_WhenComparingOppositeDirections()
        {
            const string Value = "abcdefghijklmnopqrstuvwxyz";

            AssertExtensions.AssertNoABDeadlock(
                () => CreateClassUnderTest(Value),
                () => CharSequenceUtil.CreateStringBuffer(Value),
                (seq1, sb2) =>
                {
                    Assert.AreEqual(0, seq1.CompareTo((object?)sb2));
                });
        }

        [Test]
        public void Test_CompareTo_ICharSequence_SynchronizedTextBuilderCharSequence_SynchronizesWhileReading()
        {
            var target = CreateClassUnderTest(String1);
            var other = CharSequenceUtil.CreateSynchronizedTextBuilderCharSequence(String1);

            CharSequenceUtil.AssertSynchronizesWhileReading(other.Value!, () => Assert.AreEqual(0, target.CompareTo((ICharSequence?)other)));
        }

        [Test]
        public void Test_CompareTo_ICharSequence_StringBuffer_SynchronizesWhileReading()
        {
            var target = CreateClassUnderTest(String1);
            var other = CharSequenceUtil.CreateStringBuffer(String1)!;

            CharSequenceUtil.AssertSynchronizesWhileReading(other, () => Assert.AreEqual(0, target.CompareTo((ICharSequence?)other)));
        }

        [Test]
        public void Test_CompareTo_Object_SynchronizedTextBuilderCharSequence_SynchronizesWhileReading()
        {
            var target = CreateClassUnderTest(String1);
            var other = CharSequenceUtil.CreateSynchronizedTextBuilderCharSequence(String1);

            CharSequenceUtil.AssertSynchronizesWhileReading(other.Value!, () => Assert.AreEqual(0, target.CompareTo((object?)other)));
        }

        [Test]
        public void Test_CompareTo_Object_SynchronizedTextBuilder_SynchronizesWhileReading()
        {
            var target = CreateClassUnderTest(String1);
            var other = CharSequenceUtil.CreateSynchronizedTextBuilder(String1)!;

            CharSequenceUtil.AssertSynchronizesWhileReading(other, () => Assert.AreEqual(0, target.CompareTo((object?)other)));
        }

        [Test]
        public void Test_CompareTo_Object_StringBuffer_SynchronizesWhileReading()
        {
            var target = CreateClassUnderTest(String1);
            var other = CharSequenceUtil.CreateStringBuffer(String1)!;

            CharSequenceUtil.AssertSynchronizesWhileReading(other, () => Assert.AreEqual(0, target.CompareTo((object?)other)));
        }

        [Test]
        public void Test_CompareTo_Object_WithMatchingFormattedInteger_ThrowsArgumentException()
        {
            int value = 123;
            var target = CreateClassUnderTest(value.ToString());
            Assert.Throws<ArgumentException>(() => target.CompareTo(value));
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

                Assert.AreEqual(expected, CreateClassUnderTest(leftValue).CompareTo(rightValue));
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
                Assert.AreEqual(expected, CreateClassUnderTest(leftValue).CompareTo(rightValue));
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
                Assert.AreEqual(expected, CreateClassUnderTest(leftValue).CompareTo(rightValue));
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
                Assert.AreEqual(expected, CreateClassUnderTest(leftValue).CompareTo(rightValue));
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
                Assert.AreEqual(expected, CreateClassUnderTest(leftValue).CompareTo(rightValue));
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
                Assert.AreEqual(expected, CreateClassUnderTest(leftValue).CompareTo(rightValue));
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
                Assert.AreEqual(expected, CreateClassUnderTest(leftValue).CompareTo(rightValue));
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
                Assert.AreEqual(expected, CreateClassUnderTest(leftValue).CompareTo(rightValue));
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
