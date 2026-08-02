using J2N.Buffers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        #endregion

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

            yield return new object?[] { "abc", "abcd", false, -100, 96354 };
            yield return new object?[] { "abcd", "abc", false, 100, 2987074 };

            yield return new object?[] { "😀", "😀", true, 0, 1772899 };
            yield return new object?[] { "😁", "😀", false, 1, 1772900 };
        }

        public static IEnumerable<object?[]> Equals_String_TestData()
        {
            foreach (var item in Comparison_String_TestData())
            {
                // left, right, expected Equals
                yield return new object?[] { item[0], item[1], item[2] };
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
                Assert.AreEqual(actual, 0);
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
