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

using J2N.Text;
using NUnit.Framework;
using System;
using System.Collections.Generic;
#nullable enable

namespace J2N.Globalization
{
    internal partial class TestOrdinal
    {
        private const string Value = "abcdefghijklmnopqrstuvwxyz";

        [Test]
        public void Test_Equal_SynchronizedTextBuilderCharSequence_SynchronizedTextBuilder_ShouldNotDeadlock_WhenComparingOppositeDirections()
        {
            AssertExtensions.AssertNoABDeadlock(
                () => new SynchronizedTextBuilderCharSequence(new SynchronizedTextBuilder(Value)),
                () => new SynchronizedTextBuilder(Value),
                (seq1, seq2) =>
                {
                    Assert.IsTrue(Ordinal.Equal(seq1, seq2));
                });
        }

        [Test]
        public void Test_Equal_SynchronizedTextBuilder_SynchronizedTextBuilderCharSequence_ShouldNotDeadlock_WhenComparingOppositeDirections()
        {
            AssertExtensions.AssertNoABDeadlock(
                () => new SynchronizedTextBuilder(Value),
                () => new SynchronizedTextBuilderCharSequence(new SynchronizedTextBuilder(Value)),
                (seq1, seq2) =>
                {
                    Assert.IsTrue(Ordinal.Equal(seq1, seq2));
                });
        }

        [Test]
        public void Test_Equal_SynchronizedTextBuilder_StringBuffer_ShouldNotDeadlock_WhenComparingOppositeDirections()
        {
            AssertExtensions.AssertNoABDeadlock(
                () => new SynchronizedTextBuilder(Value),
                () => new StringBuffer(Value),
                (seq1, seq2) =>
                {
                    Assert.IsTrue(Ordinal.Equal(seq1, seq2));
                });
        }

        [Test]
        public void Test_Equal_StringBuffer_SynchronizedTextBuilder_ShouldNotDeadlock_WhenComparingOppositeDirections()
        {
            AssertExtensions.AssertNoABDeadlock(
                () => new StringBuffer(Value),
                () => new SynchronizedTextBuilder(Value),
                (seq1, seq2) =>
                {
                    Assert.IsTrue(Ordinal.Equal(seq1, seq2));
                });
        }

        [Test]
        public void Test_Equal_SynchronizedTextBuilderCharSequence_StringBuffer_ShouldNotDeadlock_WhenComparingOppositeDirections()
        {
            AssertExtensions.AssertNoABDeadlock(
                () => new SynchronizedTextBuilderCharSequence(new SynchronizedTextBuilder(Value)),
                () => new StringBuffer(Value),
                (seq1, seq2) =>
                {
                    Assert.IsTrue(Ordinal.Equal(seq1, seq2));
                });
        }

        [Test]
        public void Test_Equal_StringBuffer_SynchronizedTextBuilderCharSequence_ShouldNotDeadlock_WhenComparingOppositeDirections()
        {
            AssertExtensions.AssertNoABDeadlock(
                () => new StringBuffer(Value),
                () => new SynchronizedTextBuilderCharSequence(new SynchronizedTextBuilder(Value)),
                (seq1, seq2) =>
                {
                    Assert.IsTrue(Ordinal.Equal(seq1, seq2));
                });
        }


        public static IEnumerable<TestCaseData> Equal_ICharSequence_SynchronizedTextBuilder_TestData()
        {
            var leftFactories = new Func<string?, object?>[]
            {
                CharSequenceUtil.CreateStringCharSequence,
                CharSequenceUtil.CreateCharArrayCharSequence,
                CharSequenceUtil.CreateStringBuilderCharSequence,
                CharSequenceUtil.CreateMutableTextBufferCharSequence,
                CharSequenceUtil.CreateSynchronizedTextBuilderCharSequence,
                CharSequenceUtil.CreateStringBuffer,
            };

            var rightFactories = new Func<string?, object?>[]
            {
                CharSequenceUtil.CreateSynchronizedTextBuilder,
            };

            foreach (var leftFactory in leftFactories)
            {
                foreach (var rightFactory in rightFactories)
                {
                    foreach (var item in CharSequenceUtil.Equals_String_TestData())
                    {
                        // We need a SynchronizedTextBuilder to test with, so skip any cases where the right value is null.
                        if (item[1] is not null)
                        {
                            var leftArg = leftFactory((string?)item[0]);
                            var rightArg = rightFactory((string?)item[1]);

                            yield return new TestCaseData(leftArg, rightArg)
                                .FormatArguments(leftArg, rightArg);
                        }
                    }
                }
            }
        }

        [TestCaseSource(nameof(Equal_ICharSequence_SynchronizedTextBuilder_TestData))]
        public void Test_Equal_ICharSequence_SynchronizedTextBuilder_SynchronizesWhileReading(ICharSequence? leftValue, SynchronizedTextBuilder rightValue)
        {
            try
            {
                CharSequenceUtil.AssertSynchronizesWhileReading(rightValue, () => Ordinal.Equal(leftValue, rightValue));
            }
            finally
            {
                CharSequenceUtil.Dispose(leftValue);
            }
        }

        [TestCaseSource(nameof(Equal_ICharSequence_SynchronizedTextBuilder_TestData))]
        public void Test_Equal_SynchronizedTextBuilder_ICharSequence_SynchronizesWhileReading(ICharSequence? leftValue, SynchronizedTextBuilder rightValue)
        {
            try
            {
                CharSequenceUtil.AssertSynchronizesWhileReading(rightValue, () => Ordinal.Equal(rightValue, leftValue));
            }
            finally
            {
                CharSequenceUtil.Dispose(leftValue);
            }
        }

        public static IEnumerable<TestCaseData> Equal_ICharSequence_StringBuffer_TestData()
        {
            var leftFactories = new Func<string?, object?>[]
            {
                CharSequenceUtil.CreateStringCharSequence,
                CharSequenceUtil.CreateCharArrayCharSequence,
                CharSequenceUtil.CreateStringBuilderCharSequence,
                CharSequenceUtil.CreateMutableTextBufferCharSequence,
                CharSequenceUtil.CreateSynchronizedTextBuilderCharSequence,
                CharSequenceUtil.CreateStringBuffer,
            };

            var rightFactories = new Func<string?, object?>[]
            {
                CharSequenceUtil.CreateStringBuffer,
            };

            foreach (var leftFactory in leftFactories)
            {
                foreach (var rightFactory in rightFactories)
                {
                    foreach (var item in CharSequenceUtil.Equals_String_TestData())
                    {
                        // We need a SynchronizedTextBuilder to test with, so skip any cases where the right value is null.
                        if (item[1] is not null)
                        {
                            var leftArg = leftFactory((string?)item[0]);
                            var rightArg = rightFactory((string?)item[1]);

                            yield return new TestCaseData(leftArg, rightArg)
                                .FormatArguments(leftArg, rightArg);
                        }
                    }
                }
            }
        }

        [TestCaseSource(nameof(Equal_ICharSequence_StringBuffer_TestData))]
        public void Test_Equal_ICharSequence_StringBuffer_SynchronizesWhileReading(ICharSequence? leftValue, StringBuffer rightValue)
        {
            try
            {
                CharSequenceUtil.AssertSynchronizesWhileReading(rightValue, () => Ordinal.Equal(leftValue, rightValue));
            }
            finally
            {
                CharSequenceUtil.Dispose(leftValue);
            }
        }

        [TestCaseSource(nameof(Equal_ICharSequence_StringBuffer_TestData))]
        public void Test_Equal_StringBuffer_ICharSequence_SynchronizesWhileReading(ICharSequence? leftValue, StringBuffer rightValue)
        {
            try
            {
                CharSequenceUtil.AssertSynchronizesWhileReading(rightValue, () => Ordinal.Equal(rightValue, leftValue));
            }
            finally
            {
                CharSequenceUtil.Dispose(leftValue);
            }
        }



        [Test]
        public void Test_CompareString_SynchronizedTextBuilderCharSequence_SynchronizedTextBuilder_ShouldNotDeadlock_WhenComparingOppositeDirections()
        {
            AssertExtensions.AssertNoABDeadlock(
                () => new SynchronizedTextBuilderCharSequence(new SynchronizedTextBuilder(Value)),
                () => new SynchronizedTextBuilder(Value),
                (seq1, seq2) =>
                {
                    Assert.AreEqual(0, Ordinal.CompareString(seq1, seq2));
                });
        }

        [Test]
        public void Test_CompareString_SynchronizedTextBuilder_SynchronizedTextBuilderCharSequence_ShouldNotDeadlock_WhenComparingOppositeDirections()
        {
            AssertExtensions.AssertNoABDeadlock(
                () => new SynchronizedTextBuilder(Value),
                () => new SynchronizedTextBuilderCharSequence(new SynchronizedTextBuilder(Value)),
                (seq1, seq2) =>
                {
                    Assert.AreEqual(0, Ordinal.CompareString(seq1, seq2));
                });
        }

        [Test]
        public void Test_CompareString_SynchronizedTextBuilder_StringBuffer_ShouldNotDeadlock_WhenComparingOppositeDirections()
        {
            AssertExtensions.AssertNoABDeadlock(
                () => new SynchronizedTextBuilder(Value),
                () => new StringBuffer(Value),
                (seq1, seq2) =>
                {
                    Assert.AreEqual(0, Ordinal.CompareString(seq1, seq2));
                });
        }

        [Test]
        public void Test_CompareString_StringBuffer_SynchronizedTextBuilder_ShouldNotDeadlock_WhenComparingOppositeDirections()
        {
            AssertExtensions.AssertNoABDeadlock(
                () => new StringBuffer(Value),
                () => new SynchronizedTextBuilder(Value),
                (seq1, seq2) =>
                {
                    Assert.AreEqual(0, Ordinal.CompareString(seq1, seq2));
                });
        }

        [Test]
        public void Test_CompareString_SynchronizedTextBuilderCharSequence_StringBuffer_ShouldNotDeadlock_WhenComparingOppositeDirections()
        {
            AssertExtensions.AssertNoABDeadlock(
                () => new SynchronizedTextBuilderCharSequence(new SynchronizedTextBuilder(Value)),
                () => new StringBuffer(Value),
                (seq1, seq2) =>
                {
                    Assert.AreEqual(0, Ordinal.CompareString(seq1, seq2));
                });
        }

        [Test]
        public void Test_CompareString_StringBuffer_SynchronizedTextBuilderCharSequence_ShouldNotDeadlock_WhenComparingOppositeDirections()
        {
            AssertExtensions.AssertNoABDeadlock(
                () => new StringBuffer(Value),
                () => new SynchronizedTextBuilderCharSequence(new SynchronizedTextBuilder(Value)),
                (seq1, seq2) =>
                {
                    Assert.AreEqual(0, Ordinal.CompareString(seq1, seq2));
                });
        }


        public static IEnumerable<TestCaseData> CompareString_ICharSequence_SynchronizedTextBuilder_TestData()
        {
            var leftFactories = new Func<string?, object?>[]
            {
                CharSequenceUtil.CreateStringCharSequence,
                CharSequenceUtil.CreateCharArrayCharSequence,
                CharSequenceUtil.CreateStringBuilderCharSequence,
                CharSequenceUtil.CreateMutableTextBufferCharSequence,
                CharSequenceUtil.CreateSynchronizedTextBuilderCharSequence,
                CharSequenceUtil.CreateStringBuffer,
            };

            var rightFactories = new Func<string?, object?>[]
            {
                CharSequenceUtil.CreateSynchronizedTextBuilder,
            };

            foreach (var leftFactory in leftFactories)
            {
                foreach (var rightFactory in rightFactories)
                {
                    foreach (var item in CharSequenceUtil.CompareTo_String_TestData())
                    {
                        // We need a SynchronizedTextBuilder to test with, so skip any cases where the right value is null.
                        if (item[1] is not null)
                        {
                            var leftArg = leftFactory((string?)item[0]);
                            var rightArg = rightFactory((string?)item[1]);

                            yield return new TestCaseData(leftArg, rightArg)
                                .FormatArguments(leftArg, rightArg);
                        }
                    }
                }
            }
        }

        [TestCaseSource(nameof(CompareString_ICharSequence_SynchronizedTextBuilder_TestData))]
        public void Test_CompareString_ICharSequence_SynchronizedTextBuilder_SynchronizesWhileReading(ICharSequence? leftValue, SynchronizedTextBuilder rightValue)
        {
            try
            {
                CharSequenceUtil.AssertSynchronizesWhileReading(rightValue, () => Ordinal.CompareString(leftValue, rightValue));
            }
            finally
            {
                CharSequenceUtil.Dispose(leftValue);
            }
        }

        [TestCaseSource(nameof(CompareString_ICharSequence_SynchronizedTextBuilder_TestData))]
        public void Test_CompareString_SynchronizedTextBuilder_ICharSequence_SynchronizesWhileReading(ICharSequence? leftValue, SynchronizedTextBuilder rightValue)
        {
            try
            {
                CharSequenceUtil.AssertSynchronizesWhileReading(rightValue, () => Ordinal.CompareString(rightValue, leftValue));
            }
            finally
            {
                CharSequenceUtil.Dispose(leftValue);
            }
        }

        public static IEnumerable<TestCaseData> CompareString_ICharSequence_StringBuffer_TestData()
        {
            var leftFactories = new Func<string?, object?>[]
            {
                CharSequenceUtil.CreateStringCharSequence,
                CharSequenceUtil.CreateCharArrayCharSequence,
                CharSequenceUtil.CreateStringBuilderCharSequence,
                CharSequenceUtil.CreateMutableTextBufferCharSequence,
                CharSequenceUtil.CreateSynchronizedTextBuilderCharSequence,
                CharSequenceUtil.CreateStringBuffer,
            };

            var rightFactories = new Func<string?, object?>[]
            {
                CharSequenceUtil.CreateStringBuffer,
            };

            foreach (var leftFactory in leftFactories)
            {
                foreach (var rightFactory in rightFactories)
                {
                    foreach (var item in CharSequenceUtil.CompareTo_String_TestData())
                    {
                        // We need a SynchronizedTextBuilder to test with, so skip any cases where the right value is null.
                        if (item[1] is not null)
                        {
                            var leftArg = leftFactory((string?)item[0]);
                            var rightArg = rightFactory((string?)item[1]);

                            yield return new TestCaseData(leftArg, rightArg)
                                .FormatArguments(leftArg, rightArg);
                        }
                    }
                }
            }
        }

        [TestCaseSource(nameof(CompareString_ICharSequence_StringBuffer_TestData))]
        public void Test_CompareString_ICharSequence_StringBuffer_SynchronizesWhileReading(ICharSequence? leftValue, StringBuffer rightValue)
        {
            try
            {
                CharSequenceUtil.AssertSynchronizesWhileReading(rightValue, () => Ordinal.CompareString(leftValue, rightValue));
            }
            finally
            {
                CharSequenceUtil.Dispose(leftValue);
            }
        }

        [TestCaseSource(nameof(CompareString_ICharSequence_StringBuffer_TestData))]
        public void Test_CompareString_StringBuffer_ICharSequence_SynchronizesWhileReading(ICharSequence? leftValue, StringBuffer rightValue)
        {
            try
            {
                CharSequenceUtil.AssertSynchronizesWhileReading(rightValue, () => Ordinal.CompareString(rightValue, leftValue));
            }
            finally
            {
                CharSequenceUtil.Dispose(leftValue);
            }
        }
    }
}
