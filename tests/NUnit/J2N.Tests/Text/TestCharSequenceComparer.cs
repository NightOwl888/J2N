using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Threading;
#nullable enable

namespace J2N.Text
{
    [TestFixture]
    internal class TestCharSequenceComparer
    {
        private CultureInfo originalCulture = null!;


        [SetUp]
        public virtual void SetUp()
        {
            originalCulture = CultureInfo.CurrentCulture;
#if !FEATURE_CULTUREINFO_CURRENTCULTURE_SETTER
            Thread.CurrentThread.CurrentCulture
#else
            CultureInfo.CurrentCulture
#endif
                 = new CultureInfo("tr-TR");
        }

        [TearDown]
        public virtual void TearDown()
        {
#if !FEATURE_CULTUREINFO_CURRENTCULTURE_SETTER
            Thread.CurrentThread.CurrentCulture
#else
            CultureInfo.CurrentCulture
#endif
                = originalCulture;
        }

        #region Equals

        [Test]
        public void Test_Equals_Object_SynchronizedTextBuilderCharSequence_SynchronizedTextBuilderCharSequence_ShouldNotDeadlock_WhenComparingOppositeDirections()
        {
            const string Value = "abcdefghijklmnopqrstuvwxyz";

            AssertExtensions.AssertNoABDeadlock(
                () => CharSequenceUtil.CreateSynchronizedTextBuilderCharSequence(Value),
                () => CharSequenceUtil.CreateSynchronizedTextBuilderCharSequence(Value),
                (seq1, seq2) =>
                {
                    Assert.IsTrue(CharSequenceComparer.Ordinal.Equals((object?)seq1, (object?)seq2));
                });
        }

        [Test]
        public void Test_Equals_Object_SynchronizedTextBuilderCharSequence_SynchronizedTextBuilder_ShouldNotDeadlock_WhenComparingOppositeDirections()
        {
            const string Value = "abcdefghijklmnopqrstuvwxyz";

            AssertExtensions.AssertNoABDeadlock(
                () => CharSequenceUtil.CreateSynchronizedTextBuilderCharSequence(Value),
                () => CharSequenceUtil.CreateSynchronizedTextBuilder(Value),
                (seq1, seq2) =>
                {
                    Assert.IsTrue(CharSequenceComparer.Ordinal.Equals((object?)seq1, (object?)seq2));
                });
        }

        [Test]
        public void Test_Equals_Object_SynchronizedTextBuilderCharSequence_StringBuffer_ShouldNotDeadlock_WhenComparingOppositeDirections()
        {
            const string Value = "abcdefghijklmnopqrstuvwxyz";

            AssertExtensions.AssertNoABDeadlock(
                () => CharSequenceUtil.CreateSynchronizedTextBuilderCharSequence(Value),
                () => CharSequenceUtil.CreateStringBuffer(Value),
                (seq1, seq2) =>
                {
                    Assert.IsTrue(CharSequenceComparer.Ordinal.Equals((object?)seq1, (object?)seq2));
                });
        }

        [Test]
        public void Test_Equals_Object_StringBuffer_SynchronizedTextBuilderCharSequence_ShouldNotDeadlock_WhenComparingOppositeDirections()
        {
            const string Value = "abcdefghijklmnopqrstuvwxyz";

            AssertExtensions.AssertNoABDeadlock(
                () => CharSequenceUtil.CreateStringBuffer(Value),
                () => CharSequenceUtil.CreateSynchronizedTextBuilderCharSequence(Value),
                (seq1, seq2) =>
                {
                    Assert.IsTrue(CharSequenceComparer.Ordinal.Equals((object?)seq1, (object?)seq2));
                });
        }

        [Test]
        public void Test_Equals_Object_StringBuffer_SynchronizedTextBuilder_ShouldNotDeadlock_WhenComparingOppositeDirections()
        {
            const string Value = "abcdefghijklmnopqrstuvwxyz";

            AssertExtensions.AssertNoABDeadlock(
                () => CharSequenceUtil.CreateStringBuffer(Value),
                () => CharSequenceUtil.CreateSynchronizedTextBuilder(Value),
                (seq1, seq2) =>
                {
                    Assert.IsTrue(CharSequenceComparer.Ordinal.Equals((object?)seq1, (object?)seq2));
                });
        }

        [Test]
        public void Test_Equals_Object_StringBuffer_StringBuffer_ShouldNotDeadlock_WhenComparingOppositeDirections()
        {
            const string Value = "abcdefghijklmnopqrstuvwxyz";

            AssertExtensions.AssertNoABDeadlock(
                () => CharSequenceUtil.CreateStringBuffer(Value),
                () => CharSequenceUtil.CreateStringBuffer(Value),
                (seq1, seq2) =>
                {
                    Assert.IsTrue(CharSequenceComparer.Ordinal.Equals((object?)seq1, (object?)seq2));
                });
        }

        [Test]
        public void Test_Equals_ICharSequence_SynchronizedTextBuilderCharSequence_SynchronizedTextBuilderCharSequence_ShouldNotDeadlock_WhenComparingOppositeDirections()
        {
            const string Value = "abcdefghijklmnopqrstuvwxyz";

            AssertExtensions.AssertNoABDeadlock(
                () => CharSequenceUtil.CreateSynchronizedTextBuilderCharSequence(Value),
                () => CharSequenceUtil.CreateSynchronizedTextBuilderCharSequence(Value),
                (seq1, seq2) =>
                {
                    Assert.IsTrue(CharSequenceComparer.Ordinal.Equals(seq1, seq2));
                });
        }

        [Test]
        public void Test_Equals_ICharSequence_SynchronizedTextBuilderCharSequence_StringBuffer_ShouldNotDeadlock_WhenComparingOppositeDirections()
        {
            const string Value = "abcdefghijklmnopqrstuvwxyz";

            AssertExtensions.AssertNoABDeadlock(
                () => CharSequenceUtil.CreateSynchronizedTextBuilderCharSequence(Value),
                () => CharSequenceUtil.CreateStringBuffer(Value),
                (seq1, seq2) =>
                {
                    Assert.IsTrue(CharSequenceComparer.Ordinal.Equals(seq1, seq2));
                });
        }

        [Test]
        public void Test_Equals_ICharSequence_StringBuffer_SynchronizedTextBuilderCharSequence_ShouldNotDeadlock_WhenComparingOppositeDirections()
        {
            const string Value = "abcdefghijklmnopqrstuvwxyz";

            AssertExtensions.AssertNoABDeadlock(
                () => CharSequenceUtil.CreateStringBuffer(Value),
                () => CharSequenceUtil.CreateSynchronizedTextBuilderCharSequence(Value),
                (seq1, seq2) =>
                {
                    Assert.IsTrue(CharSequenceComparer.Ordinal.Equals(seq1, seq2));
                });
        }


        [Test]
        public void Test_Equals_ICharSequence_SynchronizedTextBuilderCharSequence_SynchronizesWhileReading()
        {
            const string Value = "abcdefghijklmnopqrstuvwxyz";

            foreach (var leftFactory in CharSequenceUtil.ICharSequenceFactories)
            {
                var sequence = CharSequenceUtil.CreateSynchronizedTextBuilderCharSequence(Value);
                CharSequenceUtil.AssertSynchronizesWhileReading(sequence.Value!, () => Assert.IsTrue(CharSequenceComparer.Ordinal.Equals(leftFactory(Value), sequence)));
            }
        }

        [Test]
        public void Test_Equals_ICharSequence_StringBuffer_SynchronizesWhileReading()
        {
            const string Value = "abcdefghijklmnopqrstuvwxyz";

            foreach (var leftFactory in CharSequenceUtil.ICharSequenceFactories)
            {
                var sequence = CharSequenceUtil.CreateStringBuffer(Value);
                CharSequenceUtil.AssertSynchronizesWhileReading(sequence!, () => Assert.IsTrue(CharSequenceComparer.Ordinal.Equals(leftFactory(Value), sequence)));
            }
        }

        [Test]
        public void Test_Equals_Object_SynchronizedTextBuilderCharSequence_SynchronizesWhileReading()
        {
            const string Value = "abcdefghijklmnopqrstuvwxyz";

            foreach (var leftFactory in CharSequenceUtil.ComparableObjectFactories)
            {
                var sequence = CharSequenceUtil.CreateSynchronizedTextBuilderCharSequence(Value);
                CharSequenceUtil.AssertSynchronizesWhileReading(sequence.Value!, () => Assert.IsTrue(CharSequenceComparer.Ordinal.Equals((object?)leftFactory(Value), (object?)sequence)));
            }
        }

        [Test]
        public void Test_Equals_Object_SynchronizedTextBuilder_SynchronizesWhileReading()
        {
            const string Value = "abcdefghijklmnopqrstuvwxyz";

            foreach (var leftFactory in CharSequenceUtil.ComparableObjectFactories)
            {
                var sequence = CharSequenceUtil.CreateSynchronizedTextBuilder(Value);
                CharSequenceUtil.AssertSynchronizesWhileReading(sequence!, () => Assert.IsTrue(CharSequenceComparer.Ordinal.Equals((object?)leftFactory(Value), (object?)sequence)));
            }
        }

        [Test]
        public void Test_Equals_Object_StringBuffer_SynchronizesWhileReading()
        {
            const string Value = "abcdefghijklmnopqrstuvwxyz";

            foreach (var leftFactory in CharSequenceUtil.ComparableObjectFactories)
            {
                var sequence = CharSequenceUtil.CreateStringBuffer(Value);
                CharSequenceUtil.AssertSynchronizesWhileReading(sequence!, () => Assert.IsTrue(CharSequenceComparer.Ordinal.Equals((object?)leftFactory(Value), (object?)sequence)));
            }
        }

        public static IEnumerable<TestCaseData> Equals_Object_Object_TestData()
        {
            foreach (var leftFactory in CharSequenceUtil.ComparableObjectFactories)
            {
                foreach (var rightFactory in CharSequenceUtil.ComparableObjectFactories)
                {
                    foreach (var item in CharSequenceUtil.Equals_String_TestData())
                    {
                        var leftArg = leftFactory((string?)item[0]);
                        var rightArg = rightFactory((string?)item[1]);
                        var expectedArg = item[2];

                        yield return new TestCaseData(leftArg, rightArg, expectedArg)
                            .FormatArguments(leftArg, rightArg);
                    }
                }
            }
        }

        [TestCaseSource(nameof(Equals_Object_Object_TestData))]
        public void Test_Equals_Object_Object(object? leftValue, object? rightValue, bool expected)
        {
            try
            {
#if FEATURE_BROKEN_NULL_CHARSEQUENCE_COMPARISON
                if (leftValue is null)
                {
                    if (rightValue is ICharSequence cs2 && !cs2.HasValue)
                        expected = false; // J2N TODO: Fix broken null comparison
                }
                if (leftValue is ICharSequence cs1 && !cs1.HasValue)
                {
                    if (rightValue is null)
                        expected = false; // J2N TODO: Fix broken null comparison
                }
#endif

                Assert.AreEqual(expected, CharSequenceComparer.Ordinal.Equals(leftValue, rightValue));
            }
            finally
            {
                CharSequenceUtil.Dispose(leftValue);
                CharSequenceUtil.Dispose(rightValue);
            }
        }

#if !FEATURE_BROKEN_CHARSEQENCE_EXCEPTION_HANDLING
        [TestCaseSource(nameof(Object_Object_Invalid_TestData))]
        public void Test_Equals_Object_Object_Invalid(object? leftValue, object? rightValue)
        {
            Assert.Throws<ArgumentException>(() => CharSequenceComparer.Ordinal.Compare(leftValue, rightValue));
        }
#endif

        public static IEnumerable<TestCaseData> Equals_ICharSequence_ICharSequence_TestData()
        {
            foreach (var leftFactory in CharSequenceUtil.ICharSequenceFactories)
            {
                foreach (var rightFactory in CharSequenceUtil.ICharSequenceFactories)
                {
                    foreach (var item in CharSequenceUtil.Equals_String_TestData())
                    {
                        var leftArg = leftFactory((string?)item[0]);
                        var rightArg = rightFactory((string?)item[1]);
                        var expectedArg = item[2];

                        yield return new TestCaseData(leftArg, rightArg, expectedArg)
                            .FormatArguments(leftArg, rightArg);
                    }
                }
            }
        }

        [TestCaseSource(nameof(Equals_ICharSequence_ICharSequence_TestData))]
        public void Test_Equals_ICharSequence_ICharSequence(ICharSequence? leftValue, ICharSequence? rightValue, bool expected)
        {
            try
            {
                Assert.AreEqual(expected, CharSequenceComparer.Ordinal.Equals(leftValue, rightValue));
            }
            finally
            {
                CharSequenceUtil.Dispose(leftValue);
                CharSequenceUtil.Dispose(rightValue);
            }
        }

        public static IEnumerable<TestCaseData> Equals_ICharSequence_String_TestData()
        {
            var rightFactories = new Func<string?, object?>[]
            {
                CharSequenceUtil.CreateString,
            };

            foreach (var leftFactory in CharSequenceUtil.ICharSequenceFactories)
            {
                foreach (var rightFactory in rightFactories)
                {
                    foreach (var item in CharSequenceUtil.Equals_String_TestData())
                    {
                        var leftArg = leftFactory((string?)item[0]);
                        var rightArg = rightFactory((string?)item[1]);
                        var expectedArg = item[2];

                        yield return new TestCaseData(leftArg, rightArg, expectedArg)
                            .FormatArguments(leftArg, rightArg);
                    }
                }
            }
        }

        [TestCaseSource(nameof(Equals_ICharSequence_String_TestData))]
        public void Test_Equals_ICharSequence_String(ICharSequence? leftValue, string? rightValue, bool expected)
        {
            try
            {
                Assert.AreEqual(expected, CharSequenceComparer.Ordinal.Equals(leftValue, rightValue));
            }
            finally
            {
                CharSequenceUtil.Dispose(leftValue);
            }
        }

        public static IEnumerable<TestCaseData> Equals_ICharSequence_CharArray_TestData()
        {
            var rightFactories = new Func<string?, object?>[]
            {
                CharSequenceUtil.CreateCharArray,
            };

            foreach (var leftFactory in CharSequenceUtil.ICharSequenceFactories)
            {
                foreach (var rightFactory in rightFactories)
                {
                    foreach (var item in CharSequenceUtil.Equals_String_TestData())
                    {
                        var leftArg = leftFactory((string?)item[0]);
                        var rightArg = rightFactory((string?)item[1]);
                        var expectedArg = item[2];

                        yield return new TestCaseData(leftArg, rightArg, expectedArg)
                            .FormatArguments(leftArg, rightArg);
                    }
                }
            }
        }

        [TestCaseSource(nameof(Equals_ICharSequence_CharArray_TestData))]
        public void Test_Equals_ICharSequence_CharArray(ICharSequence? leftValue, char[]? rightValue, bool expected)
        {
            try
            {
                Assert.AreEqual(expected, CharSequenceComparer.Ordinal.Equals(leftValue, rightValue));
            }
            finally
            {
                CharSequenceUtil.Dispose(leftValue);
            }
        }

        public static IEnumerable<TestCaseData> Equals_ICharSequence_StringBuilder_TestData()
        {
            var rightFactories = new Func<string?, object?>[]
            {
                CharSequenceUtil.CreateStringBuilder,
            };

            foreach (var leftFactory in CharSequenceUtil.ICharSequenceFactories)
            {
                foreach (var rightFactory in rightFactories)
                {
                    foreach (var item in CharSequenceUtil.Equals_String_TestData())
                    {
                        var leftArg = leftFactory((string?)item[0]);
                        var rightArg = rightFactory((string?)item[1]);
                        var expectedArg = item[2];

                        yield return new TestCaseData(leftArg, rightArg, expectedArg)
                            .FormatArguments(leftArg, rightArg);
                    }
                }
            }
        }

        [TestCaseSource(nameof(Equals_ICharSequence_StringBuilder_TestData))]
        public void Test_Equals_ICharSequence_StringBuilder(ICharSequence? leftValue, StringBuilder? rightValue, bool expected)
        {
            try
            {
                Assert.AreEqual(expected, CharSequenceComparer.Ordinal.Equals(leftValue, rightValue));
            }
            finally
            {
                CharSequenceUtil.Dispose(leftValue);
            }
        }

        #endregion Equals


        #region Compare

        [Test]
        public void Test_Compare_Object_SynchronizedTextBuilderCharSequence_SynchronizedTextBuilderCharSequence_ShouldNotDeadlock_WhenComparingOppositeDirections()
        {
            const string Value = "abcdefghijklmnopqrstuvwxyz";

            AssertExtensions.AssertNoABDeadlock(
                () => CharSequenceUtil.CreateSynchronizedTextBuilderCharSequence(Value),
                () => CharSequenceUtil.CreateSynchronizedTextBuilderCharSequence(Value),
                (seq1, seq2) =>
                {
                    Assert.AreEqual(0, CharSequenceComparer.Ordinal.Compare((object?)seq1, (object?)seq2));
                });
        }

        [Test]
        public void Test_Compare_Object_SynchronizedTextBuilderCharSequence_SynchronizedTextBuilder_ShouldNotDeadlock_WhenComparingOppositeDirections()
        {
            const string Value = "abcdefghijklmnopqrstuvwxyz";

            AssertExtensions.AssertNoABDeadlock(
                () => CharSequenceUtil.CreateSynchronizedTextBuilderCharSequence(Value),
                () => CharSequenceUtil.CreateSynchronizedTextBuilder(Value),
                (seq1, seq2) =>
                {
                    Assert.AreEqual(0, CharSequenceComparer.Ordinal.Compare((object?)seq1, (object?)seq2));
                });
        }

        [Test]
        public void Test_Compare_Object_SynchronizedTextBuilderCharSequence_StringBuffer_ShouldNotDeadlock_WhenComparingOppositeDirections()
        {
            const string Value = "abcdefghijklmnopqrstuvwxyz";

            AssertExtensions.AssertNoABDeadlock(
                () => CharSequenceUtil.CreateSynchronizedTextBuilderCharSequence(Value),
                () => CharSequenceUtil.CreateStringBuffer(Value),
                (seq1, seq2) =>
                {
                    Assert.AreEqual(0, CharSequenceComparer.Ordinal.Compare((object?)seq1, (object?)seq2));
                });
        }

        [Test]
        public void Test_Compare_Object_StringBuffer_SynchronizedTextBuilderCharSequence_ShouldNotDeadlock_WhenComparingOppositeDirections()
        {
            const string Value = "abcdefghijklmnopqrstuvwxyz";

            AssertExtensions.AssertNoABDeadlock(
                () => CharSequenceUtil.CreateStringBuffer(Value),
                () => CharSequenceUtil.CreateSynchronizedTextBuilderCharSequence(Value),
                (seq1, seq2) =>
                {
                    Assert.AreEqual(0, CharSequenceComparer.Ordinal.Compare((object?)seq1, (object?)seq2));
                });
        }

        [Test]
        public void Test_Compare_Object_StringBuffer_SynchronizedTextBuilder_ShouldNotDeadlock_WhenComparingOppositeDirections()
        {
            const string Value = "abcdefghijklmnopqrstuvwxyz";

            AssertExtensions.AssertNoABDeadlock(
                () => CharSequenceUtil.CreateStringBuffer(Value),
                () => CharSequenceUtil.CreateSynchronizedTextBuilder(Value),
                (seq1, seq2) =>
                {
                    Assert.AreEqual(0, CharSequenceComparer.Ordinal.Compare((object?)seq1, (object?)seq2));
                });
        }

        [Test]
        public void Test_Compare_Object_StringBuffer_StringBuffer_ShouldNotDeadlock_WhenComparingOppositeDirections()
        {
            const string Value = "abcdefghijklmnopqrstuvwxyz";

            AssertExtensions.AssertNoABDeadlock(
                () => CharSequenceUtil.CreateStringBuffer(Value),
                () => CharSequenceUtil.CreateStringBuffer(Value),
                (seq1, seq2) =>
                {
                    Assert.AreEqual(0, CharSequenceComparer.Ordinal.Compare((object?)seq1, (object?)seq2));
                });
        }

        [Test]
        public void Test_Compare_ICharSequence_SynchronizedTextBuilderCharSequence_SynchronizedTextBuilderCharSequence_ShouldNotDeadlock_WhenComparingOppositeDirections()
        {
            const string Value = "abcdefghijklmnopqrstuvwxyz";

            AssertExtensions.AssertNoABDeadlock(
                () => CharSequenceUtil.CreateSynchronizedTextBuilderCharSequence(Value),
                () => CharSequenceUtil.CreateSynchronizedTextBuilderCharSequence(Value),
                (seq1, seq2) =>
                {
                    Assert.AreEqual(0, CharSequenceComparer.Ordinal.Compare(seq1, seq2));
                });
        }

        [Test]
        public void Test_Compare_ICharSequence_SynchronizedTextBuilderCharSequence_StringBuffer_ShouldNotDeadlock_WhenComparingOppositeDirections()
        {
            const string Value = "abcdefghijklmnopqrstuvwxyz";

            AssertExtensions.AssertNoABDeadlock(
                () => CharSequenceUtil.CreateSynchronizedTextBuilderCharSequence(Value),
                () => CharSequenceUtil.CreateStringBuffer(Value),
                (seq1, seq2) =>
                {
                    Assert.AreEqual(0, CharSequenceComparer.Ordinal.Compare(seq1, seq2));
                });
        }

        [Test]
        public void Test_Compare_ICharSequence_StringBuffer_SynchronizedTextBuilderCharSequence_ShouldNotDeadlock_WhenComparingOppositeDirections()
        {
            const string Value = "abcdefghijklmnopqrstuvwxyz";

            AssertExtensions.AssertNoABDeadlock(
                () => CharSequenceUtil.CreateStringBuffer(Value),
                () => CharSequenceUtil.CreateSynchronizedTextBuilderCharSequence(Value),
                (seq1, seq2) =>
                {
                    Assert.AreEqual(0, CharSequenceComparer.Ordinal.Compare(seq1, seq2));
                });
        }


        [Test]
        public void Test_Compare_ICharSequence_SynchronizedTextBuilderCharSequence_SynchronizesWhileReading()
        {
            const string Value = "abcdefghijklmnopqrstuvwxyz";

            foreach (var leftFactory in CharSequenceUtil.ICharSequenceFactories)
            {
                var sequence = CharSequenceUtil.CreateSynchronizedTextBuilderCharSequence(Value);
                CharSequenceUtil.AssertSynchronizesWhileReading(sequence.Value!, () => Assert.AreEqual(0, CharSequenceComparer.Ordinal.Compare(leftFactory(Value), sequence)));
            }
        }

        [Test]
        public void Test_Compare_ICharSequence_StringBuffer_SynchronizesWhileReading()
        {
            const string Value = "abcdefghijklmnopqrstuvwxyz";

            foreach (var leftFactory in CharSequenceUtil.ICharSequenceFactories)
            {
                var sequence = CharSequenceUtil.CreateStringBuffer(Value);
                CharSequenceUtil.AssertSynchronizesWhileReading(sequence!, () => Assert.AreEqual(0, CharSequenceComparer.Ordinal.Compare(leftFactory(Value), sequence)));
            }
        }

        [Test]
        public void Test_Compare_Object_SynchronizedTextBuilderCharSequence_SynchronizesWhileReading()
        {
            const string Value = "abcdefghijklmnopqrstuvwxyz";

            foreach (var leftFactory in CharSequenceUtil.ComparableObjectFactories)
            {
                var sequence = CharSequenceUtil.CreateSynchronizedTextBuilderCharSequence(Value);
                CharSequenceUtil.AssertSynchronizesWhileReading(sequence.Value!, () => Assert.AreEqual(0, CharSequenceComparer.Ordinal.Compare((object?)leftFactory(Value), (object?)sequence)));
            }
        }

        [Test]
        public void Test_Compare_Object_SynchronizedTextBuilder_SynchronizesWhileReading()
        {
            const string Value = "abcdefghijklmnopqrstuvwxyz";

            foreach (var leftFactory in CharSequenceUtil.ComparableObjectFactories)
            {
                var sequence = CharSequenceUtil.CreateSynchronizedTextBuilder(Value);
                CharSequenceUtil.AssertSynchronizesWhileReading(sequence!, () => Assert.AreEqual(0, CharSequenceComparer.Ordinal.Compare((object?)leftFactory(Value), (object?)sequence)));
            }
        }

        [Test]
        public void Test_Compare_Object_StringBuffer_SynchronizesWhileReading()
        {
            const string Value = "abcdefghijklmnopqrstuvwxyz";

            foreach (var leftFactory in CharSequenceUtil.ComparableObjectFactories)
            {
                var sequence = CharSequenceUtil.CreateStringBuffer(Value);
                CharSequenceUtil.AssertSynchronizesWhileReading(sequence!, () => Assert.AreEqual(0, CharSequenceComparer.Ordinal.Compare((object?)leftFactory(Value), (object?)sequence)));
            }
        }



        public static IEnumerable<TestCaseData> Compare_Object_Object_TestData()
        {
            foreach (var leftFactory in CharSequenceUtil.ComparableObjectFactories)
            {
                foreach (var rightFactory in CharSequenceUtil.ComparableObjectFactories)
                {
                    foreach (var item in CharSequenceUtil.CompareTo_String_TestData())
                    {
                        var leftArg = leftFactory((string?)item[0]);
                        var rightArg = rightFactory((string?)item[1]);
                        var expectedArg = item[2];

                        yield return new TestCaseData(leftArg, rightArg, expectedArg)
                            .FormatArguments(leftArg, rightArg);
                    }
                }
            }
        }

        // J2N TODO: When any factories that can return naked null value are added, this test causes
        // all tests to run when this class node is run in VS2026 and other quirks like changing
        // grouping depending on which nodes are selected. It is unclear whether this is an NUnit bug,
        // somethign in the test SDK, something in the adapter, or a bug in VS2026. For now, we will leave
        // this in place, but if you need selecting the J2N.Text tests to work right, this test needs to be commented.
        [TestCaseSource(nameof(Compare_Object_Object_TestData))]
        public void Test_Compare_Object_Object(object? leftValue, object? rightValue, int expected)
        {
            try
            {
#if FEATURE_BROKEN_NULL_CHARSEQUENCE_COMPARISON
                if (leftValue is null)
                {
                    if (rightValue is ICharSequence cs2 && !cs2.HasValue)
                        expected = -1; // J2N TODO: Fix broken null comparison
                }
                if (leftValue is ICharSequence cs1 && !cs1.HasValue)
                {
                    if (rightValue is null)
                        expected = 1; // J2N TODO: Fix broken null comparison
                    if (rightValue is ICharSequence cs2)
                    {
                        if (!cs2.HasValue)
                            expected = -1; // J2N TODO: Fix broken null comparison
                        else if (cs2.HasValue && cs2.Length == 0)
                            expected = 0; // J2N TODO: Fix broken null comparison
                    }
                    if (rightValue is ISpannable<char> spannable2)
                    {
                        if (!spannable2.HasValue)
                            expected = -1; // J2N TODO: Fix broken null comparison
                        else if (spannable2.HasValue && spannable2.AsSpan().IsEmpty)
                            expected = 0; // J2N TODO: Fix broken null comparison
                    }
                    if (rightValue is SynchronizedTextBuilder stb)
                    {
                        if (stb.Length == 0)
                            expected = 0; // J2N TODO: Fix broken null comparison
                    }
                }
#endif

                CharSequenceUtil.AssertCompareTo(expected, CharSequenceComparer.Ordinal.Compare(leftValue, rightValue));
            }
            finally
            {
                CharSequenceUtil.Dispose(leftValue);
                CharSequenceUtil.Dispose(rightValue);
            }
        }

#if !FEATURE_BROKEN_CHARSEQENCE_EXCEPTION_HANDLING

        public static IEnumerable<TestCaseData> Object_Object_Invalid_TestData()
        {
            const string Value = "abcdefghijklmnopqrstuvwxyz";

            foreach (var factory in CharSequenceUtil.ComparableObjectFactories)
            {
                var leftArg = factory(Value);
                var rightArg = CharSequenceUtil.CreateInvalidCharSequenceObject(Value);

                yield return new TestCaseData(leftArg, rightArg)
                    .FormatArguments(leftArg, rightArg);
            }

            foreach (var factory in CharSequenceUtil.ComparableObjectFactories)
            {
                var leftArg = CharSequenceUtil.CreateInvalidCharSequenceObject(Value);
                var rightArg = factory(Value);

                yield return new TestCaseData(leftArg, rightArg)
                    .FormatArguments(leftArg, rightArg);
            }

            {
                var leftArg = CharSequenceUtil.CreateInvalidCharSequenceObject(Value);
                var rightArg = CharSequenceUtil.CreateInvalidCharSequenceObject(Value);

                yield return new TestCaseData(leftArg, rightArg)
                    .FormatArguments(leftArg, rightArg);
            }
        }

        [TestCaseSource(nameof(Object_Object_Invalid_TestData))]
        public void Test_Compare_Object_Object_Invalid(object? leftValue, object? rightValue)
        {
            Assert.Throws<ArgumentException>(() => CharSequenceComparer.Ordinal.Compare(leftValue, rightValue));
        }
#endif


        public static IEnumerable<TestCaseData> Compare_ICharSequence_ICharSequence_TestData()
        {
            foreach (var leftFactory in CharSequenceUtil.ICharSequenceFactories)
            {
                foreach (var rightFactory in CharSequenceUtil.ICharSequenceFactories)
                {
                    foreach (var item in CharSequenceUtil.CompareTo_String_TestData())
                    {
                        var leftArg = leftFactory((string?)item[0]);
                        var rightArg = rightFactory((string?)item[1]);
                        var expectedArg = item[2];

                        Debug.Assert(leftArg is null || leftArg is ICharSequence);
                        Debug.Assert(rightArg is null || rightArg is ICharSequence);


                        yield return new TestCaseData(leftArg, rightArg, expectedArg)
                            .FormatArguments(leftArg, rightArg);
                    }
                }
            }
        }

        // J2N TODO: When any factories that can return naked null value are added, this test causes
        // all tests to run when this class node is run in VS2026 and other quirks like changing
        // grouping depending on which nodes are selected. It is unclear whether this is an NUnit bug,
        // somethign in the test SDK, something in the adapter, or a bug in VS2026. For now, we will leave
        // this in place, but if you need selecting the J2N.Text tests to work right, this test needs to be commented.
        [TestCaseSource(nameof(Compare_ICharSequence_ICharSequence_TestData))]
        public void Test_Compare_ICharSequence_ICharSeqeunce(ICharSequence? leftValue, ICharSequence? rightValue, int expected)
        {
            try
            {
#if FEATURE_BROKEN_NULL_CHARSEQUENCE_COMPARISON
                if (leftValue is null)
                {
                    if (rightValue is ICharSequence cs2)
                    {
                        if (!cs2.HasValue)
                            expected = -1; // J2N TODO: Fix broken null comparison
                        else if (cs2.HasValue && cs2.Length == 0)
                            expected = 0; // J2N TODO: Fix broken null comparison
                    }
                }
                if (leftValue is ICharSequence cs1 && !cs1.HasValue)
                {
                    if (rightValue is ICharSequence cs2)
                    {
                        if (!cs2.HasValue)
                            expected = -1; // J2N TODO: Fix broken null comparison
                        else if (cs2.HasValue && cs2.Length == 0)
                            expected = 0; // J2N TODO: Fix broken null comparison
                    }
                }
#endif

                CharSequenceUtil.AssertCompareTo(expected, CharSequenceComparer.Ordinal.Compare(leftValue, rightValue));
            }
            finally
            {
                CharSequenceUtil.Dispose(leftValue);
                CharSequenceUtil.Dispose(rightValue);
            }
        }


        public static IEnumerable<TestCaseData> Compare_ICharSequence_String_TestData()
        {
            var rightFactories = new Func<string?, object?>[]
            {
                CharSequenceUtil.CreateString,
            };

            foreach (var leftFactory in CharSequenceUtil.ICharSequenceFactories)
            {
                foreach (var rightFactory in rightFactories)
                {
                    foreach (var item in CharSequenceUtil.CompareTo_String_TestData())
                    {
                        var leftArg = leftFactory((string?)item[0]);
                        var rightArg = rightFactory((string?)item[1]);
                        var expectedArg = item[2];

                        yield return new TestCaseData(leftArg, rightArg, expectedArg)
                            .FormatArguments(leftArg, rightArg);
                    }
                }
            }
        }

        [TestCaseSource(nameof(Compare_ICharSequence_String_TestData))]
        public void Test_Compare_ICharSequence_String(ICharSequence? leftValue, string? rightValue, int expected)
        {
            try
            {
                CharSequenceUtil.AssertCompareTo(expected, CharSequenceComparer.Ordinal.Compare(leftValue, rightValue));
            }
            finally
            {
                CharSequenceUtil.Dispose(leftValue);
            }
        }

        public static IEnumerable<TestCaseData> Compare_ICharSequence_CharArray_TestData()
        {
            var rightFactories = new Func<string?, object?>[]
            {
                CharSequenceUtil.CreateCharArray,
            };

            foreach (var leftFactory in CharSequenceUtil.ICharSequenceFactories)
            {
                foreach (var rightFactory in rightFactories)
                {
                    foreach (var item in CharSequenceUtil.CompareTo_String_TestData())
                    {
                        var leftArg = leftFactory((string?)item[0]);
                        var rightArg = rightFactory((string?)item[1]);
                        var expectedArg = item[2];

                        yield return new TestCaseData(leftArg, rightArg, expectedArg)
                            .FormatArguments(leftArg, rightArg);
                    }
                }
            }
        }

        [TestCaseSource(nameof(Compare_ICharSequence_CharArray_TestData))]
        public void Test_Compare_ICharSequence_CharArray(ICharSequence? leftValue, char[]? rightValue, int expected)
        {
            try
            {
                CharSequenceUtil.AssertCompareTo(expected, CharSequenceComparer.Ordinal.Compare(leftValue, rightValue));
            }
            finally
            {
                CharSequenceUtil.Dispose(leftValue);
            }
        }

        public static IEnumerable<TestCaseData> Compare_ICharSequence_StringBuilder_TestData()
        {
            var rightFactories = new Func<string?, object?>[]
            {
                CharSequenceUtil.CreateStringBuilder,
            };

            foreach (var leftFactory in CharSequenceUtil.ICharSequenceFactories)
            {
                foreach (var rightFactory in rightFactories)
                {
                    foreach (var item in CharSequenceUtil.CompareTo_String_TestData())
                    {
                        var leftArg = leftFactory((string?)item[0]);
                        var rightArg = rightFactory((string?)item[1]);
                        var expectedArg = item[2];

                        yield return new TestCaseData(leftArg, rightArg, expectedArg)
                            .FormatArguments(leftArg, rightArg);
                    }
                }
            }
        }

        [TestCaseSource(nameof(Compare_ICharSequence_StringBuilder_TestData))]
        public void Test_Compare_ICharSequence_StringBuilder(ICharSequence? leftValue, StringBuilder? rightValue, int expected)
        {
            try
            {
#if FEATURE_BROKEN_NULL_CHARSEQUENCE_COMPARISON
                if (leftValue is null)
                {
                    if (rightValue is null)
                        expected = -1; // J2N TODO: Fix broken null comparison
                }
                if (leftValue is ICharSequence cs1 && !cs1.HasValue)
                {
                    if (rightValue is null)
                        expected = -1; // J2N TODO: Fix broken null comparison
                }

#endif
                CharSequenceUtil.AssertCompareTo(expected, CharSequenceComparer.Ordinal.Compare(leftValue, rightValue));
            }
            finally
            {
                CharSequenceUtil.Dispose(leftValue);
            }
        }


        #endregion Compare


        #region GetHashCode

        [Test]
        public void Test_GetHashCode_ICharSequence_SynchronizedTextBuilderCharSequence_SynchronizesWhileReading()
        {
            const string Value = "abcdefghijklmnopqrstuvwxyz";
            var sequence = CharSequenceUtil.CreateSynchronizedTextBuilderCharSequence(Value);
            CharSequenceUtil.AssertSynchronizesWhileReading(sequence.Value!, () => Assert.AreEqual(958031277, CharSequenceComparer.Ordinal.GetHashCode(sequence)));
        }

        [Test]
        public void Test_GetHashCode_ICharSequence_StringBuffer_SynchronizesWhileReading()
        {
            const string Value = "abcdefghijklmnopqrstuvwxyz";
            var sequence = CharSequenceUtil.CreateStringBuffer(Value);
            CharSequenceUtil.AssertSynchronizesWhileReading(sequence!, () => Assert.AreEqual(958031277, CharSequenceComparer.Ordinal.GetHashCode(sequence)));
        }

        [Test]
        public void Test_GetHashCode_Object_SynchronizedTextBuilderCharSequence_SynchronizesWhileReading()
        {
            const string Value = "abcdefghijklmnopqrstuvwxyz";
            var sequence = CharSequenceUtil.CreateSynchronizedTextBuilderCharSequence(Value);
            CharSequenceUtil.AssertSynchronizesWhileReading(sequence.Value!, () => Assert.AreEqual(958031277, CharSequenceComparer.Ordinal.GetHashCode((object?)sequence)));
        }

        [Test]
        public void Test_GetHashCode_Object_SynchronizedTextBuilder_SynchronizesWhileReading()
        {
            const string Value = "abcdefghijklmnopqrstuvwxyz";
            var sequence = CharSequenceUtil.CreateSynchronizedTextBuilder(Value);
            CharSequenceUtil.AssertSynchronizesWhileReading(sequence!, () => Assert.AreEqual(958031277, CharSequenceComparer.Ordinal.GetHashCode((object?)sequence)));
        }

        [Test]
        public void Test_GetHashCode_Object_StringBuffer_SynchronizesWhileReading()
        {
            const string Value = "abcdefghijklmnopqrstuvwxyz";
            var sequence = CharSequenceUtil.CreateStringBuffer(Value);
            CharSequenceUtil.AssertSynchronizesWhileReading(sequence!, () => Assert.AreEqual(958031277, CharSequenceComparer.Ordinal.GetHashCode((object?)sequence)));
        }

        public static IEnumerable<TestCaseData> GetHashCode_Object_TestData()
        {
            foreach (var leftFactory in CharSequenceUtil.ComparableObjectFactories)
            {
                foreach (var item in CharSequenceUtil.GetHashCode_String_TestData())
                {
                    var leftArg = leftFactory((string?)item[0]);
                    var expectedArg = item[1];
                    yield return new TestCaseData(leftArg, expectedArg)
                        .FormatArguments(leftArg);
                }
            }
        }

        [TestCaseSource(nameof(GetHashCode_Object_TestData))]
        public void Test_GetHashCode_Object(object? value, int expected)
        {
            Assert.AreEqual(expected, CharSequenceComparer.Ordinal.GetHashCode(value));
        }

#if !FEATURE_BROKEN_CHARSEQENCE_EXCEPTION_HANDLING
        [Test]
        public void Test_GetHashCode_Object_Invalid()
        {
            Assert.Throws<ArgumentException>(() => CharSequenceComparer.Ordinal.GetHashCode(CharSequenceUtil.CreateInvalidCharSequenceObject(null)));
        }
#endif

        public static IEnumerable<TestCaseData> GetHashCode_ICharSequence_TestData()
        {
            foreach (var leftFactory in CharSequenceUtil.ICharSequenceFactories)
            {
                foreach (var item in CharSequenceUtil.GetHashCode_String_TestData())
                {
                    var leftArg = leftFactory((string?)item[0]);
                    var expectedArg = item[1];
                    yield return new TestCaseData(leftArg, expectedArg)
                        .FormatArguments(leftArg);
                }
            }
        }

        [TestCaseSource(nameof(GetHashCode_ICharSequence_TestData))]
        public void Test_GetHashCode_ICharSequence(ICharSequence? value, int expected)
        {
            Assert.AreEqual(expected, CharSequenceComparer.Ordinal.GetHashCode(value));
        }


        [TestCaseSource(typeof(CharSequenceUtil), nameof(CharSequenceUtil.GetHashCode_String_TestData))]
        public void Test_GetHashCode_String(string? value, int expected)
        {
            Assert.AreEqual(expected, CharSequenceComparer.Ordinal.GetHashCode(value));
        }

        public static IEnumerable<TestCaseData> GetHashCode_CharArray_TestData()
        {
            var factories = new Func<string?, object?>[]
            {
                CharSequenceUtil.CreateCharArray,
            };

            foreach (var leftFactory in factories)
            {
                foreach (var item in CharSequenceUtil.GetHashCode_String_TestData())
                {
                    var leftArg = leftFactory((string?)item[0]);
                    var expectedArg = item[1];
                    yield return new TestCaseData(leftArg, expectedArg)
                        .FormatArguments(leftArg);
                }
            }
        }

        [TestCaseSource(nameof(GetHashCode_CharArray_TestData))]
        public void Test_GetHashCode_CharArray(char[]? value, int expected)
        {
            Assert.AreEqual(expected, CharSequenceComparer.Ordinal.GetHashCode(value));
        }

        public static IEnumerable<TestCaseData> GetHashCode_StringBuilder_TestData()
        {
            var factories = new Func<string?, object?>[]
            {
                CharSequenceUtil.CreateStringBuilder,
            };

            foreach (var leftFactory in factories)
            {
                foreach (var item in CharSequenceUtil.GetHashCode_String_TestData())
                {
                    var leftArg = leftFactory((string?)item[0]);
                    var expectedArg = item[1];
                    yield return new TestCaseData(leftArg, expectedArg)
                        .FormatArguments(leftArg);
                }
            }
        }

        [TestCaseSource(nameof(GetHashCode_StringBuilder_TestData))]
        public void Test_GetHashCode_StringBuilder(StringBuilder? value, int expected)
        {
            Assert.AreEqual(expected, CharSequenceComparer.Ordinal.GetHashCode(value));
        }

        #endregion GetHashCode
    }
}
