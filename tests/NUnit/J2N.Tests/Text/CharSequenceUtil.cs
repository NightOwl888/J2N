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

using J2N.Buffers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
#nullable enable

namespace J2N.Text
{
    internal static class CharSequenceUtil
    {
        #region CharSequence Factories

        public static string? CreateString(string? value)
            => value;

        public static char[]? CreateCharArray(string? value)
            => value is null ? null : value.ToCharArray();

        public static StringBuilder? CreateStringBuilder(string? value)
            => value is null ? null : new StringBuilder(value);

        public static StringCharSequence CreateStringCharSequence(string? value)
            => new(value);

        public static CharArrayCharSequence CreateCharArrayCharSequence(string? value)
            => new(value is null ? null : value.ToCharArray());

        public static StringBuilderCharSequence CreateStringBuilderCharSequence(string? value)
            => new(value is null ? null : new StringBuilder(value));

        public static StringBuffer? CreateStringBuffer(string? value)
            => value is null ? null : new(value);

        public static MutableTextBufferCharSequence CreateMutableTextBufferCharSequence(string? value)
            => value is null
                ? new MutableTextBufferCharSequence(null)
                : new MutableTextBufferCharSequence(new MutableTextBuffer(ArrayAllocator<char>.Default).Initialize(value));

        public static MutableTextBufferCharSequence CreateTextBuilderAsCharSequence(string? value)
            => value is null
                ? new MutableTextBufferCharSequence(null)
                : new MutableTextBufferCharSequence(new TextBuilder(value).buffer);

        public static SynchronizedTextBuilderCharSequence CreateSynchronizedTextBuilderCharSequence(string? value)
            => value is null
                ? new SynchronizedTextBuilderCharSequence(null)
                : new SynchronizedTextBuilderCharSequence(new SynchronizedTextBuilder(value));

        public static MutableTextBufferCharSequence CreatePooledTextBuilderAsCharSequence(string? value)
        {
            if (value is null)
            {
                return new(null);
            }

            // Note this must be disposed after the test by calling the Dispose() method below
            return new(new PooledTextBuilder(value).buffer);
        }


        public static TextBuilder? CreateTextBuilder(string? value)
            => value is null ? null : new TextBuilder(value);

        // Note this must be disposed after the test by calling the Dispose() method below
        public static PooledTextBuilder? CreatePooledTextBuilder(string? value)
            => value is null ? null : new PooledTextBuilder(value);

        public static SynchronizedTextBuilder? CreateSynchronizedTextBuilder(string? value)
            => value is null ? null : new SynchronizedTextBuilder(value);

        public static object CreateInvalidCharSequenceObject(string? value)
            => new object();

        #endregion

        #region CharSequence Factory Common Lists

        public static IReadOnlyList<Func<string?, object?>> ICharSequenceFactories { get; } =
            new Func<string?, object?>[]
            {
                CreateStringCharSequence,
                CreateCharArrayCharSequence,
                CreateStringBuilderCharSequence,
                CreateMutableTextBufferCharSequence,
                CreateSynchronizedTextBuilderCharSequence,
                CreateStringBuffer,
            };

        public static IReadOnlyList<Func<string?, object?>> ComparableObjectFactories { get; } =
            ICharSequenceFactories
                .Concat(new Func<string?, object?>[]
                {
                    CreateString,
                    CreateCharArray,
                    CreateStringBuilder,
                    CreateTextBuilder,
                    CreatePooledTextBuilder,
                    CreateSynchronizedTextBuilder,
                })
                .ToArray();

        #endregion CharSequence Factory Common Lists

        #region Test Data

        public static IEnumerable<object?[]> Comparison_String_TestData()
        {
            // left value, right value, Equals() expected, CompareTo() expected, GetHashCode() expected
            //yield return new object?[] { null, null, true, 0, int.MaxValue }; // TODO: Remember to test the null case (not just a wrapped null)
            yield return new object?[] { null, null, true, 0, int.MaxValue };
            yield return new object?[] { null, "", false, -1, int.MaxValue };
            yield return new object?[] { "", null, false, 1, 0 };
            yield return new object?[] { "", "", true, 0, 0 };

            yield return new object?[] { "123", "123", true, 0, 48690 };
            yield return new object?[] { "123", "456", false, -3, 48690 };
            yield return new object?[] { "456", "123", false, 3, 51669 };

            // Turkish dotted/dotless I
            yield return new object?[] { "I", "ı", false, -232, 73 };
            yield return new object?[] { "i", "İ", false, -199, 105 };

            // Ordinal should not consider ß == ss
            yield return new object?[] { "straße", "strasse", false, 108, -891990090 };

            // Canonically equivalent Unicode sequences are NOT equal ordinally
            yield return new object?[] { "\u00E9", "e\u0301", false, 132, 233 };

            yield return new object?[] { "abc", "abcd", false, -1, 96354 };
            yield return new object?[] { "abcd", "abc", false, 1, 2987074 };

            yield return new object?[] { "😀", "😀", true, 0, 1772899 };
            yield return new object?[] { "😁", "😀", false, 1, 1772900 };

            yield return new object?[] { "abcdefghijklmnopqrstuvwxyz", "abcdefghijklmnopqrstuvwxyz", true, 0, 958031277 };
            yield return new object?[] { "abcdefghijklmnopqrstuvwxyza", "abcdefghijklmnopqrstuvwxyz", false, 1, -365801388 };
        }

        public static IEnumerable<object?[]> Equals_String_TestData()
        {
            foreach (var item in Comparison_String_TestData())
            {
                // left, right, expected Equals
                yield return new object?[] { item[0], item[1], item[2] };
            }
        }

        public static IEnumerable<TestCaseData> Equals_Object_TestData()
        {
            foreach (var factory in ComparableObjectFactories)
            {
                foreach (var item in Equals_String_TestData())
                {
                    var leftArg = item[0];
                    var rightArgRaw = factory((string?)item[1]);
                    var expectedArg = item[2];

                    yield return new TestCaseData(leftArg, rightArgRaw, expectedArg)
                        .FormatArguments(leftArg, rightArgRaw);
                }
            }
        }

        public static IEnumerable<TestCaseData> Equals_ICharSequence_TestData()
        {
            foreach (var factory in ICharSequenceFactories)
            {
                foreach (var item in Equals_String_TestData())
                {
                    var leftArg = item[0];
                    var rightArgRaw = factory((string?)item[1]);
                    var expectedArg = item[2];

                    yield return new TestCaseData(leftArg, rightArgRaw, expectedArg)
                        .FormatArguments(leftArg, rightArgRaw);
                }
            }
        }

        public static IEnumerable<TestCaseData> Equals_StringCharSequence_TestData()
        {
            var factories = new Func<string?, object?>[]
            {
                CreateStringCharSequence,
            };

            foreach (var factory in factories)
            {
                foreach (var item in Equals_String_TestData())
                {
                    var leftArg = item[0];
                    var rightArgRaw = factory((string?)item[1]);
                    var expectedArg = item[2];

                    yield return new TestCaseData(leftArg, rightArgRaw, expectedArg)
                        .FormatArguments(leftArg, rightArgRaw);
                }
            }
        }

        public static IEnumerable<TestCaseData> Equals_CharArrayCharSequence_TestData()
        {
            var factories = new Func<string?, object?>[]
            {
                CreateCharArrayCharSequence,
            };

            foreach (var factory in factories)
            {
                foreach (var item in Equals_String_TestData())
                {
                    var leftArg = item[0];
                    var rightArgRaw = factory((string?)item[1]);
                    var expectedArg = item[2];

                    yield return new TestCaseData(leftArg, rightArgRaw, expectedArg)
                        .FormatArguments(leftArg, rightArgRaw);
                }
            }
        }

        public static IEnumerable<TestCaseData> Equals_StringBuilderCharSequence_TestData()
        {
            var factories = new Func<string?, object?>[]
            {
                CreateStringBuilderCharSequence,
            };

            foreach (var factory in factories)
            {
                foreach (var item in Equals_String_TestData())
                {
                    var leftArg = item[0];
                    var rightArgRaw = factory((string?)item[1]);
                    var expectedArg = item[2];

                    yield return new TestCaseData(leftArg, rightArgRaw, expectedArg)
                        .FormatArguments(leftArg, rightArgRaw);
                }
            }
        }

        public static IEnumerable<TestCaseData> Equals_CharArray_TestData()
        {
            var factories = new Func<string?, object?>[]
            {
                CreateCharArray,
            };

            foreach (var factory in factories)
            {
                foreach (var item in Equals_String_TestData())
                {
                    var leftArg = item[0];
                    var rightArgRaw = factory((string?)item[1]);
                    var expectedArg = item[2];

                    yield return new TestCaseData(leftArg, rightArgRaw, expectedArg)
                        .FormatArguments(leftArg, rightArgRaw);
                }
            }
        }

        public static IEnumerable<TestCaseData> Equals_StringBuilder_TestData()
        {
            var factories = new Func<string?, object?>[]
            {
                CreateStringBuilder,
            };

            foreach (var factory in factories)
            {
                foreach (var item in Equals_String_TestData())
                {
                    var leftArg = item[0];
                    var rightArgRaw = factory((string?)item[1]);
                    var expectedArg = item[2];

                    yield return new TestCaseData(leftArg, rightArgRaw, expectedArg)
                        .FormatArguments(leftArg, rightArgRaw);
                }
            }
        }

        public static IEnumerable<object?[]> CompareTo_String_TestData()
        {
            foreach (var item in Comparison_String_TestData())
            {
                // left, right, expected CompareTo
                yield return new object?[] { item[0], item[1], item[3] };
            }
        }

        public static IEnumerable<TestCaseData> CompareTo_Object_TestData()
        {
            foreach (var factory in ComparableObjectFactories)
            {
                foreach (var item in CompareTo_String_TestData())
                {
                    var leftArg = item[0];
                    var rightArgRaw = factory((string?)item[1]);
                    var expectedArg = item[2];

                    yield return new TestCaseData(leftArg, rightArgRaw, expectedArg)
                        .FormatArguments(leftArg, rightArgRaw);
                }
            }
        }

        public static IEnumerable<TestCaseData> CompareTo_ICharSequence_TestData()
        {
            foreach (var factory in ICharSequenceFactories)
            {
                foreach (var item in CompareTo_String_TestData())
                {
                    var leftArg = item[0];
                    var rightArgRaw = factory((string?)item[1]);
                    var expectedArg = item[2];

                    yield return new TestCaseData(leftArg, rightArgRaw, expectedArg)
                        .FormatArguments(leftArg, rightArgRaw);
                }
            }
        }

        public static IEnumerable<TestCaseData> CompareTo_StringCharSequence_TestData()
        {
            var factories = new Func<string?, object?>[]
            {
                CreateStringCharSequence,
            };

            foreach (var factory in factories)
            {
                foreach (var item in CompareTo_String_TestData())
                {
                    var leftArg = item[0];
                    var rightArgRaw = factory((string?)item[1]);
                    var expectedArg = item[2];

                    yield return new TestCaseData(leftArg, rightArgRaw, expectedArg)
                        .FormatArguments(leftArg, rightArgRaw);
                }
            }
        }

        public static IEnumerable<TestCaseData> CompareTo_CharArrayCharSequence_TestData()
        {
            var factories = new Func<string?, object?>[]
            {
                CreateCharArrayCharSequence,
            };

            foreach (var factory in factories)
            {
                foreach (var item in CompareTo_String_TestData())
                {
                    var leftArg = item[0];
                    var rightArgRaw = factory((string?)item[1]);
                    var expectedArg = item[2];

                    yield return new TestCaseData(leftArg, rightArgRaw, expectedArg)
                        .FormatArguments(leftArg, rightArgRaw);
                }
            }
        }

        public static IEnumerable<TestCaseData> CompareTo_StringBuilderCharSequence_TestData()
        {
            var factories = new Func<string?, object?>[]
            {
                CreateStringBuilderCharSequence,
            };

            foreach (var factory in factories)
            {
                foreach (var item in CompareTo_String_TestData())
                {
                    var leftArg = item[0];
                    var rightArgRaw = factory((string?)item[1]);
                    var expectedArg = item[2];

                    yield return new TestCaseData(leftArg, rightArgRaw, expectedArg)
                        .FormatArguments(leftArg, rightArgRaw);
                }
            }
        }

        public static IEnumerable<TestCaseData> CompareTo_CharArray_TestData()
        {
            var factories = new Func<string?, object?>[]
            {
                CreateCharArray,
            };

            foreach (var factory in factories)
            {
                foreach (var item in CompareTo_String_TestData())
                {
                    var leftArg = item[0];
                    var rightArgRaw = factory((string?)item[1]);
                    var expectedArg = item[2];

                    yield return new TestCaseData(leftArg, rightArgRaw, expectedArg)
                        .FormatArguments(leftArg, rightArgRaw);
                }
            }
        }

        public static IEnumerable<TestCaseData> CompareTo_StringBuilder_TestData()
        {
            var factories = new Func<string?, object?>[]
            {
                CreateStringBuilder,
            };

            foreach (var factory in factories)
            {
                foreach (var item in CompareTo_String_TestData())
                {
                    var leftArg = item[0];
                    var rightArgRaw = factory((string?)item[1]);
                    var expectedArg = item[2];

                    yield return new TestCaseData(leftArg, rightArgRaw, expectedArg)
                        .FormatArguments(leftArg, rightArgRaw);
                }
            }
        }

        public static IEnumerable<object?[]> GetHashCode_String_TestData()
        {
            foreach (var item in Comparison_String_TestData())
            {
                // value, expected GetHashCode
                yield return new object?[] { item[0], item[4] };
            }
        }

        #endregion Test Data

        #region Utilities

        public static void AssertSynchronizesWhileReading(SynchronizedTextBuilder builder, Action readOperation, int iterations = 500)
        {
            AssertSynchronizesWhileReading(builder.SyncRoot, readOperation, CreateMutation(builder, builder.ToString()), iterations);
        }

        public static void AssertSynchronizesWhileReading(StringBuffer buffer, Action readOperation, int iterations = 500)
        {
            AssertSynchronizesWhileReading(buffer.SyncRoot, readOperation, CreateMutation(buffer, buffer.ToString()), iterations);
        }

        private static void AssertSynchronizesWhileReading(object syncRoot, Action readOperation, Action mutate, int iterations)
        {
            using var cts = new CancellationTokenSource();

            Task writer = Task.Run(() => MutationWorker(syncRoot, mutate, cts.Token));

            try
            {
                for (int i = 0; i < iterations; i++)
                {
                    readOperation();
                }
            }
            finally
            {
                cts.Cancel();
                writer.Wait();
            }
        }


        private static Action CreateMutation(SynchronizedTextBuilder builder, string original)
        {
            return () =>
            {
                builder.Clear();
                builder.Append('X', original.Length);

                builder.Clear();
                builder.Append('Y', original.Length);

                builder.Clear();
                builder.Append(original);
            };
        }

        private static Action CreateMutation(StringBuffer buffer, string original)
        {
            return () =>
            {
                buffer.Clear();
                buffer.Append('X', original.Length);

                buffer.Clear();
                buffer.Append('Y', original.Length);

                buffer.Clear();
                buffer.Append(original);
            };
        }

        private static void MutationWorker(object syncRoot, Action mutate, CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                lock (syncRoot)
                {
                    mutate();
                }

                Thread.Yield();
            }
        }

        public static void Dispose(object? toDispose)
        {
            if (toDispose is MutableTextBufferCharSequence mtb)
            {
                try
                {
                    mtb.Value?.Dispose();
                }
                catch { /* swallow */ }
            }

            if (toDispose is IDisposable disposable)
                disposable.Dispose();

        }

        public static void AssertCompareTo(int expected, int actual)
        {
            if (expected == 0)
                Assert.AreEqual(0, actual);
            else if (expected < 0)
                Assert.Less(actual, 0);
            else
                Assert.Greater(actual, 0);
        }

        public static TestCaseData FormatArguments(this TestCaseData testCase, params object?[] displayArguments)
        {
            testCase.SetArgDisplayNames(
                displayArguments.Select(FormatArgument).ToArray());

            return testCase;
        }

        private static string FormatArgument(object? value)
        {
            if (value is null)
                return "null";

            if (value is MutableTextBufferCharSequence mtb)
            {
                if (!mtb.HasValue)
                {
                    return $"MutableTextBufferCharSequence(null)";
                }
                return $"MutableTextBufferCharSequence({mtb.Value!.GetType().Name}(\"{mtb.Value}\"))";
            }

            if (value is ICharSequence csq)
            {
                string text = csq.HasValue
                    ? $"\"{csq}\""
                    : "null";

                return $"{value.GetType().Name}({text})";
            }

            if (value is string s)
                return $"\"{s}\"";

            if (value is char[] ca)
                return $"char⟦⟧(\"{new string(ca)}\")";

            if (value is StringBuilder sb)
                return $"StringBuilder(\"{sb}\")";

            return $"{value.GetType().Name}(\"{value}\")";
        }

        #endregion
    }
}
