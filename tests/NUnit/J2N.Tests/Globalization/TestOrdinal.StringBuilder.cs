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
using System.Collections.ObjectModel;
using System.Text;
#nullable enable

namespace J2N.Globalization
{
    internal partial class TestOrdinal
    {
        [TestCaseSource(typeof(CharSequenceUtil), nameof(CharSequenceUtil.Equals_ICharSequence_TestData))]
        public void Test_Equal_StringBuilder_ICharSequence(string? leftValue, ICharSequence? rightValue, bool expected)
        {
            try
            {
                Assert.AreEqual(expected, Ordinal.Equal(CharSequenceUtil.CreateStringBuilder(leftValue), rightValue));
            }
            finally
            {
                CharSequenceUtil.Dispose(rightValue);
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
        public void Test_Equal_ICharSequence_StringBuilder(ICharSequence? leftValue, StringBuilder? rightValue, bool expected)
        {
            try
            {
                Assert.AreEqual(expected, Ordinal.Equal(leftValue, rightValue));
            }
            finally
            {
                CharSequenceUtil.Dispose(leftValue);
            }
        }

        [TestCaseSource(typeof(CharSequenceUtil), nameof(CharSequenceUtil.Equals_String_TestData))]
        public void Test_Equal_StringBuilder_CharSpan(string? leftValue, string? rightValue, bool expected)
        {
            if (rightValue is null)
            {
                Assert.Inconclusive("ReadOnlySpan<char> cannot be null, so this test is inconclusive.");
            }

            Assert.AreEqual(expected, Ordinal.Equal(CharSequenceUtil.CreateStringBuilder(leftValue), rightValue.AsSpan()));
        }

        [TestCaseSource(typeof(CharSequenceUtil), nameof(CharSequenceUtil.Equals_String_TestData))]
        public void Test_Equal_CharSpan_StringBuilder(string? leftValue, string? rightValue, bool expected)
        {
            if (leftValue is null)
            {
                Assert.Inconclusive("ReadOnlySpan<char> cannot be null, so this test is inconclusive.");
            }

            Assert.AreEqual(expected, Ordinal.Equal(leftValue.AsSpan(), CharSequenceUtil.CreateStringBuilder(rightValue)));
        }

        [TestCaseSource(typeof(CharSequenceUtil), nameof(CharSequenceUtil.Equals_String_TestData))]
        public void Test_Equal_StringBuilder_StringBuilder(string? leftValue, string? rightValue, bool expected)
        {
            Assert.AreEqual(expected, Ordinal.Equal(CharSequenceUtil.CreateStringBuilder(leftValue), CharSequenceUtil.CreateStringBuilder(rightValue)));
        }


        [TestCaseSource(nameof(Equal_StringBuilder_String_DifferingChunkSizes_TestData))]
        public void Test_Equal_StringBuilder_ICharSequence_DifferingChunkSizes(StringBuilder leftValue, int leftChunkSize, string rightValue, bool expected)
        {
            Assert.AreEqual(expected, Ordinal.Equal(leftValue, new SimpleStringCharSequence(rightValue)));
        }

        [TestCaseSource(nameof(Equal_String_StringBuilder_DifferingChunkSizes_TestData))]
        public void Test_Equal_ICharSequence_StringBuilder_DifferingChunkSizes(string leftValue, StringBuilder rightValue, int rightChunkSize, bool expected)
        {
            Assert.AreEqual(expected, Ordinal.Equal(new SimpleStringCharSequence(leftValue), rightValue));
        }

        [TestCaseSource(nameof(Equal_StringBuilder_String_DifferingChunkSizes_TestData))]
        public void Test_Equal_StringBuilder_CharSpan_DifferingChunkSizes(StringBuilder leftValue, int leftChunkSize, string rightValue, bool expected)
        {
            Assert.AreEqual(expected, Ordinal.Equal(leftValue, rightValue.AsSpan()));
        }

        [TestCaseSource(nameof(Equal_String_StringBuilder_DifferingChunkSizes_TestData))]
        public void Test_Equal_CharSpan_StringBuilder_DifferingChunkSizes(string leftValue, StringBuilder rightValue, int rightChunkSize, bool expected)
        {
            Assert.AreEqual(expected, Ordinal.Equal(leftValue.AsSpan(), rightValue));
        }

        [TestCaseSource(nameof(Equal_StringBuilder_StringBuilder_DifferingChunkSizes_TestData))]
        public void Test_Equal_StringBuilder_StringBuilder_DifferingChunkSizes(StringBuilder leftValue, int leftChunkSize, StringBuilder rightValue, int rightChunkSize, bool expected)
        {
            Assert.AreEqual(expected, Ordinal.Equal(leftValue, rightValue));
        }



        [TestCaseSource(typeof(CharSequenceUtil), nameof(CharSequenceUtil.CompareTo_ICharSequence_TestData))]
        public void Test_CompareString_StringBuilder_ICharSequence(string? leftValue, ICharSequence? rightValue, int expected)
        {
            try
            {
                Assert.AreEqual(expected, Ordinal.CompareString(CharSequenceUtil.CreateStringBuilder(leftValue), rightValue));
            }
            finally
            {
                CharSequenceUtil.Dispose(rightValue);
            }
        }

        public static IEnumerable<TestCaseData> CompareTo_ICharSequence_StringBuilder_TestData()
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

        [TestCaseSource(nameof(CompareTo_ICharSequence_StringBuilder_TestData))]
        public void Test_CompareString_ICharSequence_StringBuilder(ICharSequence? leftValue, StringBuilder? rightValue, int expected)
        {
            try
            {
                Assert.AreEqual(expected, Ordinal.CompareString(leftValue, rightValue));
            }
            finally
            {
                CharSequenceUtil.Dispose(leftValue);
            }
        }

        [TestCaseSource(typeof(CharSequenceUtil), nameof(CharSequenceUtil.CompareTo_String_TestData))]
        public void Test_CompareString_StringBuilder_CharSpan(string? leftValue, string? rightValue, int expected)
        {
            if (rightValue is null)
            {
                Assert.Inconclusive("ReadOnlySpan<char> cannot be null, so this test is inconclusive.");
            }

            Assert.AreEqual(expected, Ordinal.CompareString(CharSequenceUtil.CreateStringBuilder(leftValue), rightValue.AsSpan()));
        }

        [TestCaseSource(typeof(CharSequenceUtil), nameof(CharSequenceUtil.CompareTo_String_TestData))]
        public void Test_CompareString_CharSpan_StringBuilder(string? leftValue, string? rightValue, int expected)
        {
            if (leftValue is null)
            {
                Assert.Inconclusive("ReadOnlySpan<char> cannot be null, so this test is inconclusive.");
            }

            Assert.AreEqual(expected, Ordinal.CompareString(leftValue.AsSpan(), CharSequenceUtil.CreateStringBuilder(rightValue)));
        }

        [TestCaseSource(typeof(CharSequenceUtil), nameof(CharSequenceUtil.CompareTo_String_TestData))]
        public void Test_CompareString_StringBuilder_StringBuilder(string? leftValue, string? rightValue, int expected)
        {
            Assert.AreEqual(expected, Ordinal.CompareString(CharSequenceUtil.CreateStringBuilder(leftValue), CharSequenceUtil.CreateStringBuilder(rightValue)));
        }


        [TestCaseSource(nameof(CompareString_StringBuilder_String_DifferingChunkSizes_TestData))]
        public void Test_CompareString_StringBuilder_ICharSequence_DifferingChunkSizes(StringBuilder leftValue, int leftChunkSize, string rightValue, int expected)
        {
            Assert.AreEqual(expected, Ordinal.CompareString(leftValue, new SimpleStringCharSequence(rightValue)));
        }

        [TestCaseSource(nameof(CompareString_String_StringBuilder_DifferingChunkSizes_TestData))]
        public void Test_CompareString_ICharSequence_StringBuilder_DifferingChunkSizes(string leftValue, StringBuilder rightValue, int rightChunkSize, int expected)
        {
            Assert.AreEqual(expected, Ordinal.CompareString(new SimpleStringCharSequence(leftValue), rightValue));
        }

        [TestCaseSource(nameof(CompareString_StringBuilder_String_DifferingChunkSizes_TestData))]
        public void Test_CompareString_StringBuilder_CharSpan_DifferingChunkSizes(StringBuilder leftValue, int leftChunkSize, string rightValue, int expected)
        {
            Assert.AreEqual(expected, Ordinal.CompareString(leftValue, rightValue.AsSpan()));
        }

        [TestCaseSource(nameof(CompareString_String_StringBuilder_DifferingChunkSizes_TestData))]
        public void Test_CompareString_CharSpan_StringBuilder_DifferingChunkSizes(string leftValue, StringBuilder rightValue, int rightChunkSize, int expected)
        {
            Assert.AreEqual(expected, Ordinal.CompareString(leftValue.AsSpan(), rightValue));
        }

        [TestCaseSource(nameof(CompareString_StringBuilder_StringBuilder_DifferingChunkSizes_TestData))]
        public void Test_CompareString_StringBuilder_StringBuilder_DifferingChunkSizes(StringBuilder leftValue, int leftChunkSize, StringBuilder rightValue, int rightChunkSize, int expected)
        {
            Assert.AreEqual(expected, Ordinal.CompareString(leftValue, rightValue));
        }

        private static StringBuilder CreateStringBuilderWithMultipleChunks(string value, int chunkSize)
        {
            StringBuilder builder = new(chunkSize);

            for (int i = 0; i < value.Length; i += chunkSize)
            {
                builder.Append(value, i, Math.Min(chunkSize, value.Length - i));
            }

            return builder;
        }

        public static IEnumerable<object[]> Comparison_String_String_TestData()
        {
            // left value, right value, Equals() expected, CompareTo() expected

            yield return new object[]
            {
                "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz",
                "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz",
                true,
                0
            };

            yield return new object[]
            {
                "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwx",
                "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz",
                false,
                -2
            };

            yield return new object[]
            {
                "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwx",
                "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxy",
                false,
                -1
            };

            yield return new object[]
            {
                "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxy",
                "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwx",
                false,
                1
            };

            yield return new object[]
            {
                "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz",
                "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwx",
                false,
                2
            };

            yield return new object[]
            {
                "abc",
                "abcdef",
                false,
                -3
            };

            yield return new object[]
            {
                "abcdef",
                "abc",
                false,
                3
            };

            yield return new object[]
            {
                "abcde",
                "abcde",
                true,
                0
            };

            yield return new object[]
            {
                "abcdefgh",
                "abcdefghi",
                false,
                -1
            };

            yield return new object[]
            {
                "abcdefghi",
                "abcdefgh",
                false,
                1
            };

            yield return new object[]
            {
                "abcdefghijklmnopX",
                "abcdefghijklmnopY",
                false,
                -1
            };

            yield return new object[]
            {
                "abcdefghijklmnopY",
                "abcdefghijklmnopX",
                false,
                1
            };

            yield return new object[]
            {
                "abcdefghijklmnopX",
                "abcdefghijklmnopY",
                false,
                -1
            };

            yield return new object[]
            {
                "abcdefghijklmnopY",
                "abcdefghijklmnopX",
                false,
                1
            };

            yield return new object[]
            {
                "abcdefghijklmnopX",
                "abcdefghijklmnopZ",
                false,
                -2
            };

            yield return new object[]
            {
                "abcdefghijklmnopZ",
                "abcdefghijklmnopX",
                false,
                2
            };

            yield return new object[]
            {
                "abcdefghijkX",
                "abcdefghijkZ",
                false,
                -2
            };

            yield return new object[]
            {
                "abcdefghijkZ",
                "abcdefghijkX",
                false,
                2
            };

            yield return new object[]
            {
                "abcdX",
                "abcdY",
                false,
                -1
            };

            yield return new object[]
            {
                "abcdY",
                "abcdX",
                false,
                1
            };

            yield return new object[]
            {
                "abcdeX",
                "abcdeY",
                false,
                -1
            };

            yield return new object[]
            {
                "abcdeY",
                "abcdeX",
                false,
                1
            };

            yield return new object[]
            {
                "abcdefghijX",
                "abcdefghijZ",
                false,
                -2
            };

            yield return new object[]
            {
                "abcdefghijZ",
                "abcdefghijX",
                false,
                2
            };

            // Large character difference after multiple chunk boundaries.
            yield return new object[]
            {
                "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyza",
                "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzz",
                false,
                -25
            };

            yield return new object[]
            {
                "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzz",
                "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyza",
                false,
                25
            };

            // Large length difference after multiple equal chunks.
            yield return new object[]
            {
                new string('a', 128),
                new string('a', 3),
                false,
                125
            };

            yield return new object[]
            {
                new string('a', 3),
                new string('a', 128),
                false,
                -125
            };
        }

        public static IEnumerable<object[]> Comparison_String_String_DifferingChunkSizes_TestData()
        {
            int leftChunkSize = 4;
            int rightChunkSize = 5;

            foreach (var item in Comparison_String_String_TestData())
            {
                yield return new object[] {
                    item[0],
                    leftChunkSize,
                    item[1],
                    rightChunkSize,
                    item[2],
                    item[3],
                };
            }

            leftChunkSize = 27;
            rightChunkSize = 3;

            foreach (var item in Comparison_String_String_TestData())
            {
                yield return new object[] {
                    item[0],
                    leftChunkSize,
                    item[1],
                    rightChunkSize,
                    item[2],
                    item[3],
                };
            }

            leftChunkSize = 7;
            rightChunkSize = 23;

            foreach (var item in Comparison_String_String_TestData())
            {
                yield return new object[] {
                    item[0],
                    leftChunkSize,
                    item[1],
                    rightChunkSize,
                    item[2],
                    item[3],
                };
            }
        }

        public static IEnumerable<object[]> CompareString_StringBuilder_String_DifferingChunkSizes_TestData()
        {
            foreach (var item in Comparison_String_String_DifferingChunkSizes_TestData())
            {
                yield return new object[] {
                    CreateStringBuilderWithMultipleChunks((string)item[0], (int)item[1]),
                    item[1],
                    (string)item[2],
                    item[5],
                };
            }
        }

        public static IEnumerable<object[]> CompareString_String_StringBuilder_DifferingChunkSizes_TestData()
        {
            foreach (var item in Comparison_String_String_DifferingChunkSizes_TestData())
            {
                yield return new object[] {
                    (string)item[0],
                    CreateStringBuilderWithMultipleChunks((string)item[2], (int)item[3]),
                    item[3],
                    item[5],
                };
            }
        }

        public static IEnumerable<object[]> CompareString_StringBuilder_StringBuilder_DifferingChunkSizes_TestData()
        {
            foreach (var item in Comparison_String_String_DifferingChunkSizes_TestData())
            {
                yield return new object[] {
                    CreateStringBuilderWithMultipleChunks((string)item[0], (int)item[1]),
                    item[1],
                    CreateStringBuilderWithMultipleChunks((string)item[2], (int)item[3]),
                    item[3],
                    item[5],
                };
            }
        }


        public static IEnumerable<object[]> Equal_StringBuilder_String_DifferingChunkSizes_TestData()
        {
            foreach (var item in Comparison_String_String_DifferingChunkSizes_TestData())
            {
                yield return new object[] {
                    CreateStringBuilderWithMultipleChunks((string)item[0], (int)item[1]),
                    item[1],
                    (string)item[2],
                    item[4],
                };
            }
        }

        public static IEnumerable<object[]> Equal_String_StringBuilder_DifferingChunkSizes_TestData()
        {
            foreach (var item in Comparison_String_String_DifferingChunkSizes_TestData())
            {
                yield return new object[] {
                    (string)item[0],
                    CreateStringBuilderWithMultipleChunks((string)item[2], (int)item[3]),
                    item[3],
                    item[4],
                };
            }
        }

        public static IEnumerable<object[]> Equal_StringBuilder_StringBuilder_DifferingChunkSizes_TestData()
        {
            foreach (var item in Comparison_String_String_DifferingChunkSizes_TestData())
            {
                yield return new object[] {
                    CreateStringBuilderWithMultipleChunks((string)item[0], (int)item[1]),
                    item[1],
                    CreateStringBuilderWithMultipleChunks((string)item[2], (int)item[3]),
                    item[3],
                    item[4],
                };
            }
        }


        public sealed class SimpleStringCharSequence : ICharSequence
        {
            private readonly ReadOnlyMemory<char> memory;
            private readonly object reference; // Pin the instance while this class lives

            public SimpleStringCharSequence(string value)
            {
                memory = value.AsMemory();
                reference = value;
            }

            public bool HasValue => true;

            public int Length => memory.Length;

            public char this[int index] => memory.Span[index];

            public ICharSequence Subsequence(int startIndex, int length)
                => new CharArrayCharSequence(memory.ToArray());

            public override string ToString() => memory.Span.ToString();
        }
    }
}
