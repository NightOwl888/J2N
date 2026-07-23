// Source: https://github.com/dotnet/runtime/blob/v9.0.0/src/libraries/System.Runtime/tests/System.Runtime.Tests/System/Text/StringBuilderTests.cs
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using J2N.Buffers;
using J2N.Collections;
using J2N.IO;
using J2N.Numerics;
using J2N.TestUtilities;
using J2N.TestUtilities.Xunit;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Xunit;
//using System.Tests;
//using Microsoft.DotNet.RemoteExecutor;
#nullable enable

namespace J2N.Text.Tests
{
    /// <summary>
    /// Contains tests that ensure the correctness (compliance with the BCL) of an <see cref="MutableTextBuffer"/> implementation
    /// including subclasses.
    /// </summary>
    /// <remarks>
    /// J2N: This class does not map exactly to the upstream code. It was refactored to be an abstract class that can be used for testing
    /// multiple implementations of <see cref="MutableTextBuffer"/>. Each implementation is presumed to have the same behavior, but
    /// may have different internal implementations. For example, one implementation may use a buffer that is allocated on the heap,
    /// while another may use a buffer that is allocated from an array pool. The tests in this class are designed to ensure that all
    /// implementations behave correctly and consistently with the BCL.
    /// </remarks>
    public abstract partial class StringBuilder_Tests
    {
        private static readonly string s_noCapacityParamName = "valueCount";

        internal static readonly string s_chunkSplitSource = new string('a', 30);
        //internal static StringBuilder StringBuilderWithMultipleChunks() => new StringBuilder(20).Append(s_chunkSplitSource);

        internal sealed class MutableTextBufferTestOptions
        {
            public bool UseInvariantDefaults { get; init; } = false;
            public bool ClearExposedBuffers { get; init; } = true;
        }

        #region MutableTextBuffer Helper Methods

        /// <summary>
        /// Creates an instance of an <see cref="MutableTextBuffer"/> that can be used for testing.
        /// </summary>
        /// <returns>An instance of <see cref="MutableTextBuffer"/> that can be used for testing.</returns>
        private protected abstract MutableTextBuffer MutableTextBufferFactory(MutableTextBufferTestOptions? options = null);

        /// <summary>
        /// Creates an instance of an <see cref="MutableTextBuffer"/> that can be used for testing.
        /// </summary>
        /// <returns>An instance of <see cref="MutableTextBuffer"/> that can be used for testing.</returns>
        private protected abstract MutableTextBuffer MutableTextBufferFactory(int capacity, MutableTextBufferTestOptions? options = null);

        /// <summary>
        /// Creates an instance of an <see cref="MutableTextBuffer"/> that can be used for testing.
        /// </summary>
        /// <returns>An instance of <see cref="MutableTextBuffer"/> that can be used for testing.</returns>
        private protected abstract MutableTextBuffer MutableTextBufferFactory(int capacity, int maxCapacity, MutableTextBufferTestOptions? options = null);

        /// <summary>
        /// Creates an instance of an <see cref="MutableTextBuffer"/> that can be used for testing.
        /// </summary>
        /// <returns>An instance of <see cref="MutableTextBuffer"/> that can be used for testing.</returns>
        private protected abstract MutableTextBuffer MutableTextBufferFactory(string? value, MutableTextBufferTestOptions? options = null);

        /// <summary>
        /// Creates an instance of an <see cref="MutableTextBuffer"/> that can be used for testing.
        /// </summary>
        /// <returns>An instance of <see cref="MutableTextBuffer"/> that can be used for testing.</returns>
        private protected abstract MutableTextBuffer MutableTextBufferFactory(string? value, int startIndex, int length, int capacity, MutableTextBufferTestOptions? options = null);

        /// <summary>
        /// Creates an instance of an <see cref="MutableTextBuffer"/> that can be used for testing.
        /// </summary>
        /// <returns>An instance of <see cref="MutableTextBuffer"/> that can be used for testing.</returns>
        private protected abstract MutableTextBuffer MutableTextBufferFactory(string? value, int capacity, MutableTextBufferTestOptions? options = null);

        /// <summary>
        /// Creates an instance of an <see cref="MutableTextBuffer"/> that can be used for testing.
        /// </summary>
        /// <returns>An instance of <see cref="MutableTextBuffer"/> that can be used for testing.</returns>
        private protected abstract MutableTextBuffer MutableTextBufferFactory(ReadOnlySpan<char> value, MutableTextBufferTestOptions? options = null);

        /// <summary>
        /// Creates an instance of an <see cref="MutableTextBuffer"/> that can be used for testing.
        /// </summary>
        /// <returns>An instance of <see cref="MutableTextBuffer"/> that can be used for testing.</returns>
        private protected abstract MutableTextBuffer MutableTextBufferFactory(ReadOnlySpan<char> value, int capacity, MutableTextBufferTestOptions? options = null);

        /// <summary>
        /// Creates an instance of an <see cref="MutableTextBuffer"/> that can be used for testing.
        /// </summary>
        /// <returns>An instance of <see cref="MutableTextBuffer"/> that can be used for testing.</returns>
        private protected abstract MutableTextBuffer MutableTextBufferFactory(StringBuilder? value, MutableTextBufferTestOptions? options = null);

        /// <summary>
        /// Creates an instance of an <see cref="MutableTextBuffer"/> that can be used for testing.
        /// </summary>
        /// <returns>An instance of <see cref="MutableTextBuffer"/> that can be used for testing.</returns>
        private protected abstract MutableTextBuffer MutableTextBufferFactory(StringBuilder? value, int capacity, MutableTextBufferTestOptions? options = null);

        /// <summary>
        /// Creates an instance of an <see cref="MutableTextBuffer"/> that can be used for testing.
        /// </summary>
        /// <returns>An instance of <see cref="MutableTextBuffer"/> that can be used for testing.</returns>
        private protected abstract MutableTextBuffer MutableTextBufferFactory(StringBuilder? value, int startIndex, int length, int capacity, MutableTextBufferTestOptions? options = null);

        /// <summary>
        /// Creates an instance of an <see cref="MutableTextBuffer"/> that can be used for testing.
        /// </summary>
        /// <returns>An instance of <see cref="MutableTextBuffer"/> that can be used for testing.</returns>
        private protected abstract MutableTextBuffer MutableTextBufferFactory(ICharSequence? value, MutableTextBufferTestOptions? options = null);

        /// <summary>
        /// Creates an instance of an <see cref="MutableTextBuffer"/> that can be used for testing.
        /// </summary>
        /// <returns>An instance of <see cref="MutableTextBuffer"/> that can be used for testing.</returns>
        private protected abstract MutableTextBuffer MutableTextBufferFactory(string? value, int capacity, IArrayAllocator<char> allocator, MutableTextBufferTestOptions? options = null);

        private static readonly int MaxArrayLength =
            (int)typeof(Arrays)
                .GetField("MaxArrayLength",
                    BindingFlags.Static |
                    BindingFlags.NonPublic)!
                .GetValue(null)!;

        private static StringBuilder CreateOversizeStringBuilder()
        {
            const int ChunkSize = 256 * 1024; // safe for .NET Framework StringBuilder

            int targetLength = MaxArrayLength + 1;

            var sb = new StringBuilder();

            while (targetLength >= ChunkSize)
            {
                sb.Append('a', ChunkSize);
                targetLength -= ChunkSize;
            }

            if (targetLength > 0)
            {
                sb.Append('a', (int)targetLength);
            }

            Assert.Equal(MaxArrayLength + 1, sb.Length);
            return sb;
        }

        #endregion MutableTextBuffer Helper Methods

        #region Constructor Tests

        [Fact]
        public void Ctor_Empty()
        {
            MutableTextBuffer builder = MutableTextBufferFactory();
            Assert.Same(string.Empty, builder.ToString());
            Assert.Equal(string.Empty, builder.ToString(0, 0));
            Assert.Equal(0, builder.Length);
            Assert.Equal(MaxArrayLength, builder.MaxCapacity);
        }

        [Fact]
        public void Ctor_Int()
        {
            MutableTextBuffer builder = MutableTextBufferFactory(42);
            Assert.Same(string.Empty, builder.ToString());
            Assert.Equal(0, builder.Length);

            Assert.True(builder.Capacity >= 42);
            Assert.Equal(MaxArrayLength, builder.MaxCapacity);
        }

        [Fact]
        public void Ctor_Int_NegativeCapacity_ThrowsArgumentOutOfRangeException()
        {
            AssertExtensions.Throws<ArgumentOutOfRangeException>("capacity", () => MutableTextBufferFactory(-1)); // Capacity < 0
        }

        [Fact]
        public void Ctor_Int_Int()
        {
            // The second int parameter is MaxCapacity but in CLR4.0 and later, MutableTextBuffer isn't required to honor it.
            MutableTextBuffer builder = MutableTextBufferFactory(42, 50);
            Assert.Equal("", builder.ToString());
            Assert.Equal(0, builder.Length);

            Assert.InRange(builder.Capacity, 42, builder.MaxCapacity);
            Assert.Equal(50, builder.MaxCapacity);
        }

        [Fact]
        public void Ctor_Int_Int_Invalid()
        {
            AssertExtensions.Throws<ArgumentOutOfRangeException>("capacity", () => MutableTextBufferFactory(-1, 1)); // Capacity < 0
            AssertExtensions.Throws<ArgumentOutOfRangeException>("maxCapacity", () => MutableTextBufferFactory(0, 0)); // MaxCapacity < 1

            AssertExtensions.Throws<ArgumentOutOfRangeException>("capacity", () => MutableTextBufferFactory(2, 1)); // Capacity > maxCapacity
        }

        [Theory]
        [InlineData("Hello")]
        [InlineData("")]
        [InlineData(null)]
        public void Ctor_String(string? value)
        {
            MutableTextBuffer builder = MutableTextBufferFactory(value);

            string expected = value ?? "";
            Assert.Equal(expected, builder.ToString());
            Assert.Equal(expected.Length, builder.Length);
        }

        //// This is a good candidate for [OuterLoop].
        //[Fact] // J2N specific
        //public void Ctor_String_LessThan2GBCharLength_LoadsSuccessfully()
        //{
        //    const int TwoGiBChars = 1_073_741_824;
        //    int maxLength = TwoGiBChars - 64;

        //    char[] array = new char[maxLength];
        //    Span<char> span = array;
        //    span.Fill('a');
        //    MutableTextBuffer builder = MutableTextBufferFactory(span.ToString());
        //    Assert.Equal(maxLength, builder.Length);
        //}

        [Theory]
        [InlineData("Hello")]
        [InlineData("")]
        [InlineData(null)]
        public void Ctor_String_Int(string? value)
        {
            MutableTextBuffer builder = MutableTextBufferFactory(value, 42);

            string expected = value ?? "";
            Assert.Equal(expected, builder.ToString());
            Assert.Equal(expected.Length, builder.Length);

            Assert.True(builder.Capacity >= 42);
        }

        //// This is a good candidate for [OuterLoop].
        //[Fact] // J2N specific
        //public void Ctor_String_Int_LessThan2GBCharLength_LoadsSuccessfully()
        //{
        //    const int TwoGiBChars = 1_073_741_824;
        //    int maxLength = TwoGiBChars - 64;

        //    char[] array = new char[maxLength];
        //    Span<char> span = array;
        //    span.Fill('a');
        //    MutableTextBuffer builder = MutableTextBufferFactory(span.ToString(), 0);
        //    Assert.Equal(maxLength, builder.Length);
        //}

        [Fact] // J2N specific - was Ctor_String_Int_NegativeCapacity_ThrowsArgumentOutOfRangeException()
        public void Ctor_String_Int_Invalid()
        {
            AssertExtensions.Throws<ArgumentOutOfRangeException>("capacity", () => MutableTextBufferFactory("", -1)); // Capacity < 0
            AssertExtensions.Throws<ArgumentOutOfRangeException>("capacity", () => MutableTextBufferFactory("foo", MaxArrayLength + 1)); // Capacity > Array.MaxLength
        }

        [Theory]
        [InlineData("Hello", 0, 5)]
        [InlineData("Hello", 2, 3)]
        [InlineData("", 0, 0)]
        [InlineData(null, 0, 0)]
        public void Ctor_String_Int_Int_Int(string? value, int startIndex, int length)
        {
            MutableTextBuffer builder = MutableTextBufferFactory(value, startIndex, length, 42);

            string expected = value?.Substring(startIndex, length) ?? "";
            Assert.Equal(expected, builder.ToString());
            Assert.Equal(length, builder.Length);
            Assert.Equal(expected.Length, builder.Length);

            Assert.True(builder.Capacity >= 42);
        }

        //// This is a good candidate for [OuterLoop].
        //[Fact] // J2N specific
        //public void Ctor_String_Int_Int_Int_LessThan2GBCharLength_LoadsSuccessfully()
        //{
        //    const int TwoGiBChars = 1_073_741_824;
        //    int maxLength = TwoGiBChars - 64;

        //    char[] array = new char[maxLength];
        //    Span<char> span = array;
        //    span.Fill('a');
        //    MutableTextBuffer builder = MutableTextBufferFactory(span.ToString(), 0, maxLength, 0);
        //    Assert.Equal(maxLength, builder.Length);
        //}

        [Fact]
        public void Ctor_String_Int_Int_Int_Invalid()
        {
            AssertExtensions.Throws<ArgumentOutOfRangeException>("startIndex", () => MutableTextBufferFactory("foo", -1, 0, 0)); // Start index < 0
            AssertExtensions.Throws<ArgumentOutOfRangeException>("length", () => MutableTextBufferFactory("foo", 0, -1, 0)); // Length < 0
            AssertExtensions.Throws<ArgumentOutOfRangeException>("capacity", () => MutableTextBufferFactory("foo", 0, 0, -1)); // Capacity < 0
            AssertExtensions.Throws<ArgumentOutOfRangeException>("capacity", () => MutableTextBufferFactory("foo", 0, 0, MaxArrayLength + 1)); // Capacity > Array.MaxLength
            AssertExtensions.Throws<ArgumentOutOfRangeException>("length", () => MutableTextBufferFactory("foo", 4, 0, 0)); // Start index + length > builder.Length
            AssertExtensions.Throws<ArgumentOutOfRangeException>("length", () => MutableTextBufferFactory("foo", 3, 1, 0)); // Start index + length > builder.Length
        }

        [Theory] // J2N specific
        [InlineData("Hello", 0, 5)]
        [InlineData("Hello", 2, 3)]
        [InlineData("", 0, 0)]
        [InlineData(null, 0, 0)]
        public void Ctor_ReadOnlySpan(string? value, int startIndex, int length)
        {
            MutableTextBuffer builder = MutableTextBufferFactory(value.AsSpan(startIndex, length));

            string expected = value?.Substring(startIndex, length) ?? "";
            Assert.Equal(expected, builder.ToString());
            Assert.Equal(length, builder.Length);
            Assert.Equal(expected.Length, builder.Length);
        }

        //// .NET Framework and unknown platforms may have maximum object size limits that are far less than MaxArrayLength,
        //// so this test is only reliable on .NET Core. This is a good candidate for [OuterLoop].
        //[ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNetCore))] // J2N specific
        //public void Ctor_ReadOnlySpan_Int_MaxArrayLength_LoadsSuccessfully()
        //{
        //    char[] array = new char[MaxArrayLength];
        //    Span<char> span = array;
        //    span.Fill('a');
        //    MutableTextBuffer builder = MutableTextBufferFactory(span);
        //    Assert.Equal(MaxArrayLength, builder.Length);
        //}

        [Fact] // J2N specific
        public unsafe void Ctor_ReadOnlySpan_GreaterThanMaxArrayLength_ThrowsArgumentOutOfRangeException()
        {
            // We create an invalid pointer here instead of a string that is too long because the test would be very slow to run otherwise.
            // The constructor should check the the length against MaxCapacity, so it should throw before it tries to
            // read from the pointer.
            char c = 'a';
            AssertExtensions.Throws<ArgumentOutOfRangeException>("valueCount", () => MutableTextBufferFactory(new ReadOnlySpan<char>(Unsafe.AsPointer(ref c), MaxArrayLength + 1))); // value.Length > Array.MaxLength
        }

        [Theory] // J2N specific
        [InlineData("Hello", 0, 5)]
        [InlineData("Hello", 2, 3)]
        [InlineData("", 0, 0)]
        [InlineData(null, 0, 0)]
        public void Ctor_ReadOnlySpan_Int(string? value, int startIndex, int length)
        {
            MutableTextBuffer builder = MutableTextBufferFactory(value.AsSpan(startIndex, length), 42);

            string expected = value?.Substring(startIndex, length) ?? "";
            Assert.Equal(expected, builder.ToString());
            Assert.Equal(length, builder.Length);
            Assert.Equal(expected.Length, builder.Length);

            Assert.True(builder.Capacity >= 42);
        }

        //// .NET Framework and unknown platforms may have maximum object size limits that are far less than MaxArrayLength,
        //// so this test is only reliable on .NET Core. This is a good candidate for [OuterLoop].
        //[ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNetCore))] // J2N specific
        //public void Ctor_ReadOnlySpan_Int_MaxArrayLength_LoadsSuccessfully()
        //{
        //    char[] array = new char[MaxArrayLength];
        //    Span<char> span = array;
        //    span.Fill('a');
        //    MutableTextBuffer builder = MutableTextBufferFactory(span, 0);
        //    Assert.Equal(MaxArrayLength, builder.Length);
        //}

        [Fact] // J2N specific
        public unsafe void Ctor_ReadOnlySpan_Int_GreaterThanMaxArrayLength_ThrowsArgumentOutOfRangeException()
        {
            // We create an invalid pointer here instead of a string that is too long because the test would be very slow to run otherwise.
            // The constructor should check the the length against MaxCapacity, so it should throw before it tries to
            // read from the pointer.
            char c = 'a';
            AssertExtensions.Throws<ArgumentOutOfRangeException>("valueCount", () => MutableTextBufferFactory(new ReadOnlySpan<char>(Unsafe.AsPointer(ref c), MaxArrayLength + 1), 0)); // value.Length > Array.MaxLength
        }

        [Fact] // J2N specific
        public void Ctor_ReadOnlySpan_Int_Invalid()
        {
            AssertExtensions.Throws<ArgumentOutOfRangeException>("capacity", () => MutableTextBufferFactory("foo".AsSpan(0, 0), -1)); // Capacity < 0
            AssertExtensions.Throws<ArgumentOutOfRangeException>("capacity", () => MutableTextBufferFactory("foo".AsSpan(0, 0), MaxArrayLength + 1)); // Capacity > Array.MaxLength
        }

        [Theory] // J2N specific
        [InlineData("Hello")]
        [InlineData("")]
        [InlineData(null)]
        public void Ctor_StringBuilder(string? value)
        {
            var sb = value is not null ? new StringBuilder(value) : (StringBuilder?)null;
            MutableTextBuffer builder = MutableTextBufferFactory(sb);

            string expected = value ?? "";
            Assert.Equal(expected, builder.ToString());
            Assert.Equal(expected.Length, builder.Length);
        }

        //// This test works on .NET Framework, but it is very slow. This is a good candidate for [OuterLoop]
        //[ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNetCore))] // J2N specific
        //public void Ctor_StringBuilder_GreaterThanMaxArrayLength_ThrowsArgumentOutOfRangeException()
        //{
        //    StringBuilder sb = CreateOversizeStringBuilder();
        //    AssertExtensions.Throws<ArgumentOutOfRangeException>("valueCount", () => MutableTextBufferFactory(sb)); // value.Length > Array.MaxLength
        //}

        [Theory] // J2N specific
        [InlineData("Hello")]
        [InlineData("")]
        [InlineData(null)]
        public void Ctor_StringBuilder_Int(string? value)
        {
            var sb = value is not null ? new StringBuilder(value) : (StringBuilder?)null;
            MutableTextBuffer builder = MutableTextBufferFactory(sb, 42);

            string expected = value ?? "";
            Assert.Equal(expected, builder.ToString());
            Assert.Equal(expected.Length, builder.Length);

            Assert.True(builder.Capacity >= 42);
        }

        [Fact] // J2N specific
        public void Ctor_StringBuilder_Int_NegativeCapacity_ThrowsArgumentOutOfRangeException()
        {
            AssertExtensions.Throws<ArgumentOutOfRangeException>("capacity", () => MutableTextBufferFactory(new StringBuilder(""), -1)); // Capacity < 0
        }

        //// This test works on .NET Framework, but it is very slow. This is a good candidate for [OuterLoop]
        //[ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNetCore))] // J2N specific
        //public void Ctor_StringBuilder_Int_GreaterThanMaxArrayLength_ThrowsArgumentOutOfRangeException()
        //{
        //    StringBuilder sb = CreateOversizeStringBuilder();
        //    AssertExtensions.Throws<ArgumentOutOfRangeException>("length", () => MutableTextBufferFactory(sb, 0)); // value.Length > Array.MaxLength
        //}

        [Theory] // J2N specific
        [InlineData("Hello", 0, 5)]
        [InlineData("Hello", 2, 3)]
        [InlineData("", 0, 0)]
        [InlineData(null, 0, 0)]
        public void Ctor_StringBuilder_Int_Int_Int(string? value, int startIndex, int length)
        {
            var sb = value is not null ? new StringBuilder(value) : (StringBuilder?)null;
            MutableTextBuffer builder = MutableTextBufferFactory(sb, startIndex, length, 42);

            string expected = value?.Substring(startIndex, length) ?? "";
            Assert.Equal(expected, builder.ToString());
            Assert.Equal(length, builder.Length);
            Assert.Equal(expected.Length, builder.Length);

            Assert.True(builder.Capacity >= 42);
        }

        //// .NET Framework and unknown platforms may have maximum object size limits that are far less than MaxArrayLength,
        //// so this test is only reliable on .NET Core.
        //[ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNetCore))] // J2N specific
        //public void Ctor_StringBuilder_Int_Int_Int_MaxArrayLength_LoadsSuccessfully()
        //{
        //    var sb = CreateOversizeStringBuilder();
        //    MutableTextBuffer builder = MutableTextBufferFactory(sb, 0, MaxArrayLength, 0);
        //    Assert.Equal(MaxArrayLength, builder.Length);
        //}

        //// This test works on .NET Framework, but it is very slow. This is a good candidate for [OuterLoop]
        //[ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNetCore))] // J2N specific
        //public void Ctor_StringBuilder_Int_Int_Int_GreaterThanMaxArrayLength_ThrowsArgumentOutOfRangeException()
        //{
        //    StringBuilder sb = CreateOversizeStringBuilder();
        //    AssertExtensions.Throws<ArgumentOutOfRangeException>("length", () => MutableTextBufferFactory(sb, 0, MaxArrayLength + 1, 0)); // length > Array.MaxLength
        //}

        [Fact] // J2N specific
        public void Ctor_StringBuilder_Int_Int_Int_Invalid()
        {
            AssertExtensions.Throws<ArgumentOutOfRangeException>("startIndex", () => MutableTextBufferFactory(new StringBuilder("foo"), -1, 0, 0)); // Start index < 0
            AssertExtensions.Throws<ArgumentOutOfRangeException>("length", () => MutableTextBufferFactory(new StringBuilder("foo"), 0, -1, 0)); // Length < 0
            AssertExtensions.Throws<ArgumentOutOfRangeException>("capacity", () => MutableTextBufferFactory(new StringBuilder("foo"), 0, 0, -1)); // Capacity < 0
            AssertExtensions.Throws<ArgumentOutOfRangeException>("capacity", () => MutableTextBufferFactory(new StringBuilder("foo"), 0, 0, MaxArrayLength + 1)); // Capacity > Array.MaxLength

            AssertExtensions.Throws<ArgumentOutOfRangeException>("length", () => MutableTextBufferFactory(new StringBuilder("foo"), 4, 0, 0)); // Start index + length > builder.Length
            AssertExtensions.Throws<ArgumentOutOfRangeException>("length", () => MutableTextBufferFactory(new StringBuilder("foo"), 3, 1, 0)); // Start index + length > builder.Length
        }

        public static IEnumerable<object?[]> Test_Ctor_ICharSequence_TestData()
        {
            yield return new object?[] { "Hello", "Hello" };
            yield return new object?[] { "", "" };
            yield return new object?[] { null, "" };
        }

        public static IEnumerable<object?[]> Test_Ctor_ICharSequence_Typed_TestData()
        {
            foreach (var testCase in Test_Ctor_ICharSequence_TestData())
            {
                yield return new object?[] { new StringCharSequence((string?)testCase[0]), testCase[1] };
                yield return new object?[] { new StringBuilderCharSequence(new StringBuilder((string?)testCase[0])), testCase[1] };
                yield return new object?[] { new CharArrayCharSequence(((string?)testCase[0])?.ToCharArray()), testCase[1] };

                ReadOnlyMemory<char> memory = ((string?)testCase[0]).AsMemory();
                yield return new object?[] { new MockCharSequence(memory), testCase[1] };
                if (testCase[0] is not null)
                {
                    yield return new object?[] { new StringBuffer((string?)testCase[0]), testCase[1] };
                }
                else
                {
                    yield return new object?[] { (StringCharSequence?)null, testCase[1] };
                    yield return new object?[] { (StringBuilderCharSequence?)null, testCase[1] };
                    yield return new object?[] { (CharArrayCharSequence?)null, testCase[1] };
                    yield return new object?[] { (ICharSequence?)null, testCase[1] };
                }
            }
        }


        [Theory] // J2N specific
        [MemberData(nameof(Test_Ctor_ICharSequence_Typed_TestData))]
        public void Ctor_ICharSequence(ICharSequence? value, string expected)
        {
            MutableTextBuffer builder = MutableTextBufferFactory(value);

            Assert.Equal(expected, builder.ToString());
            Assert.Equal(expected.Length, builder.Length);
        }

        #endregion Constructor Tests


        [Fact]
        public void Item_Get_Set()
        {
            string s = "Hello";
            var builder = MutableTextBufferFactory(s);

            for (int i = 0; i < s.Length; i++)
            {
                Assert.Equal(s[i], builder[i]);

                char c = (char)(i + '0');
                builder[i] = c;
                Assert.Equal(c, builder[i]);
            }
            Assert.Equal("01234", builder.ToString());
        }

        [Fact]
        public void Item_Get_Set_InvalidIndex()
        {
            var builder = MutableTextBufferFactory("Hello");

            Assert.Throws<IndexOutOfRangeException>(() => builder[-1]); // Index < 0
            Assert.Throws<IndexOutOfRangeException>(() => builder[5]); // Index >= string.Length

            AssertExtensions.Throws<ArgumentOutOfRangeException>("index", () => builder[-1] = 'a'); // Index < 0
            AssertExtensions.Throws<ArgumentOutOfRangeException>("index", () => builder[5] = 'a'); // Index >= string.Length
        }

        [Fact]
        public void Capacity_Get_Set()
        {
            var builder = MutableTextBufferFactory("Hello");
            Assert.True(builder.Capacity >= builder.Length);

            builder.Capacity = 10;
            Assert.True(builder.Capacity >= 10);

            builder.Capacity = 5;
            Assert.True(builder.Capacity >= 5);

            // Setting the capacity to the same value does not change anything
            int oldCapacity = builder.Capacity;
            builder.Capacity = 5;
            Assert.Equal(oldCapacity, builder.Capacity);
        }

        [Fact]
        public void Capacity_Set_Invalid_ThrowsArgumentOutOfRangeException()
        {
            var builder = MutableTextBufferFactory(10, 10);
            builder.Append("Hello");
            AssertExtensions.Throws<ArgumentOutOfRangeException>("value", () => builder.Capacity = -1); // Capacity < 0
            AssertExtensions.Throws<ArgumentOutOfRangeException>("value", () => builder.Capacity = builder.MaxCapacity + 1); // Capacity > builder.MaxCapacity
            AssertExtensions.Throws<ArgumentOutOfRangeException>("value", () => builder.Capacity = builder.Length - 1); // Capacity < builder.Length
        }

        [Fact]
        public void Length_Get_Set()
        {
            var builder = MutableTextBufferFactory("Hello");

            builder.Length = 2;
            Assert.Equal(2, builder.Length);
            Assert.Equal("He", builder.ToString());

            builder.Length = 10;
            Assert.Equal(10, builder.Length);
            Assert.Equal("He" + new string((char)0, 8), builder.ToString());
        }

        [Fact]
        public void Length_Set_InvalidValue_ThrowsArgumentOutOfRangeException()
        {
            var builder = MutableTextBufferFactory(10, 10);
            builder.Append("Hello");

            AssertExtensions.Throws<ArgumentOutOfRangeException>("value", () => builder.Length = -1); // Value < 0
            AssertExtensions.Throws<ArgumentOutOfRangeException>("value", () => builder.Length = builder.MaxCapacity + 1); // Value > builder.MaxCapacity
        }

        [Theory]
        [InlineData("Hello", (ushort)0, "Hello0")]
        [InlineData("Hello", (ushort)123, "Hello123")]
        [InlineData("", (ushort)456, "456")]
        public void Append_UShort(string? original, ushort value, string expected)
        {
            var builder = MutableTextBufferFactory(original);
            builder.Append(value);
            Assert.Equal(expected, builder.ToString());
        }

        [Theory]
        [InlineData("Hello", (ushort)1, "J", "Hello1")]
        [InlineData("Hello", (ushort)1, "j", "Hello1")]
        [InlineData("Hello", (ushort)123, "j1", "Hello123")]
        [InlineData("Hello", (ushort)1, "g", "Hello1")]
        [InlineData("Hello", (ushort)123, "g1", "Hello1e+02")]
        public void Append_UShort_Format(string original, ushort value, string format, string expected)
        {
            var builder = MutableTextBufferFactory(original);
            builder.Append(value, format, CultureInfo.InvariantCulture);
            Assert.Equal(expected, builder.ToString());
        }

        [Fact]
        public void Append_UShort_NoSpareCapacity_ThrowsArgumentOutOfRangeException()
        {
            var builder = MutableTextBufferFactory(0, 5);
            builder.Append("Hello");

            AssertExtensions.Throws<ArgumentOutOfRangeException>(s_noCapacityParamName, () => builder.Append((ushort)1));
        }

        // J2N: added a BooleanFormat parameter to specify lowercase vs titlecase, so these tests were changed from upstream

        public static IEnumerable<object[]> Append_Bool_TestData()
        {
            yield return new object[] { "Hello", true, BooleanFormat.TitleCase, "HelloTrue" };
            yield return new object[] { "Hello", true, BooleanFormat.Lowercase, "Hellotrue" };
            yield return new object[] { "Hello", false, BooleanFormat.TitleCase, "HelloFalse" };
            yield return new object[] { "Hello", false, BooleanFormat.Lowercase, "Hellofalse" };
            yield return new object[] { "", false, BooleanFormat.TitleCase, "False" };
            yield return new object[] { "", false, BooleanFormat.Lowercase, "false" };
        }

        [Fact]
        public void Append_Bool()
        {
            foreach (var testdata in Append_Bool_TestData())
            {
                if (((BooleanFormat)testdata[2]) == BooleanFormat.Lowercase)
                    Test_Append_Bool_Format((string)testdata[0], (bool)testdata[1], null, (string)testdata[3]);
            }
        }

        [Fact]
        public void Append_Bool_Format()
        {
            foreach (var testdata in Append_Bool_TestData())
            {
                Test_Append_Bool_Format((string)testdata[0], (bool)testdata[1], (BooleanFormat)testdata[2], (string)testdata[3]);
            }
        }

        private void Test_Append_Bool_Format(string original, bool value, BooleanFormat? format, string expected)
        {
            var builder = MutableTextBufferFactory(original);
            if (format is null)
                builder.Append(value);
            else
                builder.Append(value, format.Value);
            Assert.Equal(expected, builder.ToString());
        }

        [Fact]
        public void Append_Bool_NoSpareCapacity_ThrowsArgumentOutOfRangeException()
        {
            var builder = MutableTextBufferFactory(0, 5);
            builder.Append("Hello");

            AssertExtensions.Throws<ArgumentOutOfRangeException>(s_noCapacityParamName, () => builder.Append(true));
        }

        public static IEnumerable<object[]> Append_Decimal_TestData()
        {
            yield return new object[] { "Hello", 0m, "Hello0" };
            yield return new object[] { "Hello", 1.23m, "Hello1.23" };
            yield return new object[] { "", -4.56m, "-4.56" };
            yield return new object[] { "", 0.000001m, "0.000001" };
            yield return new object[] { "", 0.0000001m, "1E-7" };
            yield return new object[] { "", 1.2300m, "1.2300" };
        }

        public static IEnumerable<object[]> Append_Decimal_Format_TestData()
        {
            yield return new object[] { "Hello", 1m, "J", "Hello1" };
            yield return new object[] { "Hello", 1m, "j", "Hello1" };
            yield return new object[] { "Hello", 123m, "j1", "Hello123" };
            yield return new object[] { "Hello", 1m, "g", "Hello1" };
            yield return new object[] { "Hello", 123m, "g1", "Hello1e+02" };
        }

        [Fact]
        public void Append_Decimal()
        {
            using (new ThreadCultureChange(CultureInfo.InvariantCulture))
            {
                foreach (var testdata in Append_Decimal_TestData())
                {
                    Test_Append_Decimal((string)testdata[0], (decimal)testdata[1], (string)testdata[2]);
                }
            }
        }

        private void Test_Append_Decimal(string original, decimal value, string expected)
        {
            var builder = MutableTextBufferFactory(original);
            builder.Append(value);
            Assert.Equal(expected, builder.ToString());
        }

        [Fact]
        public void Append_Decimal_Format()
        {
            foreach (var testdata in Append_Decimal_Format_TestData())
            {
                Test_Append_Decimal_Format((string)testdata[0], (decimal)testdata[1], (string)testdata[2], (string)testdata[3]);
            }
        }

        public void Test_Append_Decimal_Format(string original, decimal value, string format, string expected)
        {
            var builder = MutableTextBufferFactory(original);
            builder.Append(value, format, CultureInfo.InvariantCulture);
            Assert.Equal(expected, builder.ToString());
        }

        [Fact]
        public void Append_Decimal_NoSpareCapacity_ThrowsArgumentOutOfRangeException()
        {
            var builder = MutableTextBufferFactory(0, 5);
            builder.Append("Hello");

            AssertExtensions.Throws<ArgumentOutOfRangeException>(s_noCapacityParamName, () => builder.Append((decimal)1));
        }
        public static IEnumerable<object[]> Append_Double_TestData()
        {
            yield return new object[] { "Hello", (double)0, "Hello0.0" }; // J2N: Use the "j" format, which always has at least 1 digit after the decimal
            yield return new object[] { "Hello", 1.23, "Hello1.23" };
            yield return new object[] { "", -4.56, "-4.56" };
        }

        [Fact]
        public void Append_Double()
        {
            using (new ThreadCultureChange(CultureInfo.InvariantCulture))
            {
                foreach (var testdata in Append_Double_TestData())
                {
                    Test_Append_Double((string)testdata[0], (double)testdata[1], (string)testdata[2]);
                }
            }
        }

        private void Test_Append_Double(string original, double value, string expected)
        {
            var builder = MutableTextBufferFactory(original);
            builder.Append(value);
            Assert.Equal(expected, builder.ToString());
        }

        [Theory]
        [InlineData("Hello", 1d, "J", "Hello1.0")]
        [InlineData("Hello", 1d, "j", "Hello1.0")]
        [InlineData("Hello", 123d, "j1", "Hello123.0")]
        [InlineData("Hello", 1d, "g", "Hello1")]
        [InlineData("Hello", 123d, "g1", "Hello1e+02")]
        public void Append_Double_Format(string original, double value, string format, string expected)
        {
            var builder = MutableTextBufferFactory(original);
            builder.Append(value, format, CultureInfo.InvariantCulture);
            Assert.Equal(expected, builder.ToString());
        }

        [Fact]
        public void Append_Double_NoSpareCapacity_ThrowsArgumentOutOfRangeException()
        {
            var builder = MutableTextBufferFactory(0, 5);
            builder.Append("Hello");

            AssertExtensions.Throws<ArgumentOutOfRangeException>(s_noCapacityParamName, () => builder.Append((double)1));
        }

        [Fact]
        public void Append_Double_UsesAmbientCulture_WhenInvariantDefaultsDisabled()
        {
            using var ambientCulture = new ThreadCultureChange("fr-FR");
            var sb = MutableTextBufferFactory("foo", new MutableTextBufferTestOptions { UseInvariantDefaults = false });
            sb.Append(1.5d);
            Assert.Equal("foo1,5", sb.ToString());
        }

        [Fact]
        public void Append_Double_UsesInvariantCulture_WhenInvariantDefaultsEnabled()
        {
            using var ambientCulture = new ThreadCultureChange("fr-FR");
            var sb = MutableTextBufferFactory("foo", new MutableTextBufferTestOptions { UseInvariantDefaults = true });
            sb.Append(1.5d);
            Assert.Equal("foo1.5", sb.ToString());
        }

        [Fact]
        public void Append_Double_ExplicitProviderOverridesInvariantDefaults()
        {
            using var ambientCulture = new ThreadCultureChange("en-US");
            var sb = MutableTextBufferFactory("foo", new MutableTextBufferTestOptions { UseInvariantDefaults = true });
            sb.Append(1.5f, provider: new CultureInfo("fr-FR"));
            Assert.Equal("foo1,5", sb.ToString());
        }

        [Fact]
        public void Append_Double_ExplicitProviderOverridesAmbientCulture()
        {
            using var ambientCulture = new ThreadCultureChange("en-US");
            var sb = MutableTextBufferFactory("foo", new MutableTextBufferTestOptions { UseInvariantDefaults = false });
            sb.Append(1.5f, provider: new CultureInfo("fr-FR"));
            Assert.Equal("foo1,5", sb.ToString());
        }

        [Theory]
        [InlineData("Hello", (short)0, "Hello0")]
        [InlineData("Hello", (short)123, "Hello123")]
        [InlineData("", (short)-456, "-456")]
        public void Append_Short(string? original, short value, string expected)
        {
            var builder = MutableTextBufferFactory(original);
            builder.Append(value);
            Assert.Equal(expected, builder.ToString());
        }

        [Theory]
        [InlineData("Hello", (short)1, "J", "Hello1")]
        [InlineData("Hello", (short)1, "j", "Hello1")]
        [InlineData("Hello", (short)123, "j1", "Hello123")]
        [InlineData("Hello", (short)1, "g", "Hello1")]
        [InlineData("Hello", (short)123, "g1", "Hello1e+02")]
        public void Append_Short_Format(string original, short value, string format, string expected)
        {
            var builder = MutableTextBufferFactory(original);
            builder.Append(value, format, CultureInfo.InvariantCulture);
            Assert.Equal(expected, builder.ToString());
        }

        [Fact]
        public void Append_Short_NoSpareCapacity_ThrowsArgumentOutOfRangeException()
        {
            var builder = MutableTextBufferFactory(0, 5);
            builder.Append("Hello");

            AssertExtensions.Throws<ArgumentOutOfRangeException>(s_noCapacityParamName, () => builder.Append((short)1));
        }

        [Theory]
        [InlineData("Hello", 0, "Hello0")]
        [InlineData("Hello", 123, "Hello123")]
        [InlineData("", -456, "-456")]
        public void Append_Int(string? original, int value, string expected)
        {
            var builder = MutableTextBufferFactory(original);
            builder.Append(value);
            Assert.Equal(expected, builder.ToString());
        }

        [Theory]
        [InlineData("Hello", 1, "J", "Hello1")]
        [InlineData("Hello", 1, "j", "Hello1")]
        [InlineData("Hello", 123, "j1", "Hello123")]
        [InlineData("Hello", 1, "g", "Hello1")]
        [InlineData("Hello", 123, "g1", "Hello1e+02")]
        public void Append_Int_Format(string original, int value, string format, string expected)
        {
            var builder = MutableTextBufferFactory(original);
            builder.Append(value, format, CultureInfo.InvariantCulture);
            Assert.Equal(expected, builder.ToString());
        }

        [Fact]
        public void Append_Int_NoSpareCapacity_ThrowsArgumentOutOfRangeException()
        {
            var builder = MutableTextBufferFactory(0, 5);
            builder.Append("Hello");

            AssertExtensions.Throws<ArgumentOutOfRangeException>(s_noCapacityParamName, () => builder.Append(1));
        }

        [Fact]
        public void Append_Int_UsesAmbientCulture_WhenInvariantDefaultsDisabled()
        {
            using var ambientCulture = new ThreadCultureChange("ar-IQ");
            var sb = MutableTextBufferFactory("foo", new MutableTextBufferTestOptions { UseInvariantDefaults = false });
            sb.Append(-1);
            Assert.Equal("foo\u061C\u002D1", sb.ToString());
        }

        [Fact]
        public void Append_Int_UsesInvariantCulture_WhenInvariantDefaultsEnabled()
        {
            using var ambientCulture = new ThreadCultureChange("ar-IQ");
            var sb = MutableTextBufferFactory("foo", new MutableTextBufferTestOptions { UseInvariantDefaults = true });
            sb.Append(-1);
            Assert.Equal("foo\u002D1", sb.ToString());
        }

        [Fact]
        public void Append_Int_ExplicitProviderOverridesInvariantDefaults()
        {
            using var ambientCulture = new ThreadCultureChange("en-US");
            var sb = MutableTextBufferFactory("foo", new MutableTextBufferTestOptions { UseInvariantDefaults = true });
            sb.Append(-1, provider: new CultureInfo("ar-IQ"));
            Assert.Equal("foo\u061C\u002D1", sb.ToString());
        }

        [Fact]
        public void Append_Int_ExplicitProviderOverridesAmbientCulture()
        {
            using var ambientCulture = new ThreadCultureChange("en-US");
            var sb = MutableTextBufferFactory("foo", new MutableTextBufferTestOptions { UseInvariantDefaults = false });
            sb.Append(-1, provider: new CultureInfo("ar-IQ"));
            Assert.Equal("foo\u061C\u002D1", sb.ToString());
        }

        [Theory]
        [InlineData("Hello", (long)0, "Hello0")]
        [InlineData("Hello", (long)123, "Hello123")]
        [InlineData("", (long)-456, "-456")]
        public void Append_Long(string? original, long value, string expected)
        {
            var builder = MutableTextBufferFactory(original);
            builder.Append(value);
            Assert.Equal(expected, builder.ToString());
        }

        [Theory]
        [InlineData("Hello", 1L, "J", "Hello1")]
        [InlineData("Hello", 1L, "j", "Hello1")]
        [InlineData("Hello", 123L, "j1", "Hello123")]
        [InlineData("Hello", 1L, "g", "Hello1")]
        [InlineData("Hello", 123L, "g1", "Hello1e+02")]
        public void Append_Long_Format(string original, long value, string format, string expected)
        {
            var builder = MutableTextBufferFactory(original);
            builder.Append(value, format, CultureInfo.InvariantCulture);
            Assert.Equal(expected, builder.ToString());
        }

        [Fact]
        public void Append_Long_NoSpareCapacity_ThrowsArgumentOutOfRangeException()
        {
            var builder = MutableTextBufferFactory(0, 5);
            builder.Append("Hello");

            AssertExtensions.Throws<ArgumentOutOfRangeException>(s_noCapacityParamName, () => builder.Append((long)1));
        }

        [Theory]
        [InlineData("Hello", "abc", "Helloabc")]
        [InlineData("Hello", "def", "Hellodef")]
        [InlineData("", "g", "g")]
        [InlineData("Hello", "", "Hello")]
        [InlineData("Hello", null, "Hello")]
        public void Append_Object(string? original, object? value, string expected)
        {
            var builder = MutableTextBufferFactory(original);
            builder.Append(value);
            Assert.Equal(expected, builder.ToString());
        }

        [Fact]
        public void Append_Object_NoSpareCapacity_ThrowsArgumentOutOfRangeException()
        {
            var builder = MutableTextBufferFactory(0, 5);
            builder.Append("Hello");

            AssertExtensions.Throws<ArgumentOutOfRangeException>(s_noCapacityParamName, () => builder.Append(new object()));
        }

        [Theory]
        [InlineData("Hello", (sbyte)0, "Hello0")]
        [InlineData("Hello", (sbyte)123, "Hello123")]
        [InlineData("", (sbyte)-123, "-123")]
        public void Append_SByte(string original, sbyte value, string expected)
        {
            var builder = MutableTextBufferFactory(original);
            builder.Append(value);
            Assert.Equal(expected, builder.ToString());
        }

        [Theory]
        [InlineData("Hello", (sbyte)1, "J", "Hello1")]
        [InlineData("Hello", (sbyte)1, "j", "Hello1")]
        [InlineData("Hello", (sbyte)123, "j1", "Hello123")]
        [InlineData("Hello", (sbyte)1, "g", "Hello1")]
        [InlineData("Hello", (sbyte)123, "g1", "Hello1e+02")]
        public void Append_SByte_Format(string original, sbyte value, string format, string expected)
        {
            var builder = MutableTextBufferFactory(original);
            builder.Append(value, format, CultureInfo.InvariantCulture);
            Assert.Equal(expected, builder.ToString());
        }

        [Fact]
        public void Append_SByte_NoSpareCapacity_ThrowsArgumentOutOfRangeException()
        {
            var builder = MutableTextBufferFactory(0, 5);
            builder.Append("Hello");

            AssertExtensions.Throws<ArgumentOutOfRangeException>(s_noCapacityParamName, () => builder.Append((sbyte)1));
        }

        public static IEnumerable<object[]> Append_Float_TestData()
        {
            yield return new object[] { "Hello", (float)0, "Hello0.0" }; // J2N: Use the "j" format, which always has at least 1 digit after the decimal
            yield return new object[] { "Hello", (float)1.23, "Hello1.23" };
            yield return new object[] { "", (float)-4.56, "-4.56" };
        }

        [Fact]
        public void Append_Float()
        {
            using (new ThreadCultureChange(CultureInfo.InvariantCulture))
            {
                foreach (var testdata in Append_Float_TestData())
                {
                    Test_Append_Float((string)testdata[0], (float)testdata[1], (string)testdata[2]);
                }
            }
        }

        private void Test_Append_Float(string? original, float value, string expected)
        {
            var builder = MutableTextBufferFactory(original);
            builder.Append(value);
            Assert.Equal(expected, builder.ToString());
        }

        [Theory]
        [InlineData("Hello", 1f, "J", "Hello1.0")]
        [InlineData("Hello", 1f, "j", "Hello1.0")]
        [InlineData("Hello", 123f, "j1", "Hello123.0")]
        [InlineData("Hello", 1f, "g", "Hello1")]
        [InlineData("Hello", 123f, "g1", "Hello1e+02")]
        public void Append_Float_Format(string original, float value, string format, string expected)
        {
            var builder = MutableTextBufferFactory(original);
            builder.Append(value, format, CultureInfo.InvariantCulture);
            Assert.Equal(expected, builder.ToString());
        }

        [Fact]
        public void Append_Float_NoSpareCapacity_ThrowsArgumentOutOfRangeException()
        {
            var builder = MutableTextBufferFactory(0, 5);
            builder.Append("Hello");

            AssertExtensions.Throws<ArgumentOutOfRangeException>(s_noCapacityParamName, () => builder.Append((float)1));
        }

        [Theory]
        [InlineData("Hello", (byte)0, "Hello0")]
        [InlineData("Hello", (byte)123, "Hello123")]
        [InlineData("", (byte)123, "123")]
        public void Append_Byte(string? original, byte value, string expected)
        {
            var builder = MutableTextBufferFactory(original);
            builder.Append(value);
            Assert.Equal(expected, builder.ToString());
        }

        [Theory]
        [InlineData("Hello", (byte)1, "J", "Hello1")]
        [InlineData("Hello", (byte)1, "j", "Hello1")]
        [InlineData("Hello", (byte)123, "j1", "Hello123")]
        [InlineData("Hello", (byte)1, "g", "Hello1")]
        [InlineData("Hello", (byte)123, "g1", "Hello1e+02")]
        public void Append_Byte_Format(string original, byte value, string format, string expected)
        {
            var builder = MutableTextBufferFactory(original);
            builder.Append(value, format, CultureInfo.InvariantCulture);
            Assert.Equal(expected, builder.ToString());
        }

        [Fact]
        public void Append_Byte_NoSpareCapacity_ThrowsArgumentOutOfRangeException()
        {
            var builder = MutableTextBufferFactory(0, 5);
            builder.Append("Hello");

            AssertExtensions.Throws<ArgumentOutOfRangeException>(s_noCapacityParamName, () => builder.Append((byte)1));
        }

        [Theory]
        [InlineData("Hello", (uint)0, "Hello0")]
        [InlineData("Hello", (uint)123, "Hello123")]
        [InlineData("", (uint)456, "456")]
        public void Append_UInt(string original, uint value, string expected)
        {
            var builder = MutableTextBufferFactory(original);
            builder.Append(value);
            Assert.Equal(expected, builder.ToString());
        }

        [Theory]
        [InlineData("Hello", (uint)1, "J", "Hello1")]
        [InlineData("Hello", (uint)1, "j", "Hello1")]
        [InlineData("Hello", (uint)123, "j1", "Hello123")]
        [InlineData("Hello", (uint)1, "g", "Hello1")]
        [InlineData("Hello", (uint)123, "g1", "Hello1e+02")]
        public void Append_UInt_Format(string original, uint value, string format, string expected)
        {
            var builder = MutableTextBufferFactory(original);
            builder.Append(value, format, CultureInfo.InvariantCulture);
            Assert.Equal(expected, builder.ToString());
        }

        [Fact]
        public void Append_UInt_NoSpareCapacity_ThrowsArgumentOutOfRangeException()
        {
            var builder = MutableTextBufferFactory(0, 5);
            builder.Append("Hello");

            AssertExtensions.Throws<ArgumentOutOfRangeException>(s_noCapacityParamName, () => builder.Append((uint)1));
        }

        [Theory]
        [InlineData("Hello", (ulong)0, "Hello0")]
        [InlineData("Hello", (ulong)123, "Hello123")]
        [InlineData("", (ulong)456, "456")]
        public void Append_ULong(string original, ulong value, string expected)
        {
            var builder = MutableTextBufferFactory(original);
            builder.Append(value);
            Assert.Equal(expected, builder.ToString());
        }

        [Theory]
        [InlineData("Hello", (ulong)1, "J", "Hello1")]
        [InlineData("Hello", (ulong)1, "j", "Hello1")]
        [InlineData("Hello", (ulong)123, "j1", "Hello123")]
        [InlineData("Hello", (ulong)1, "g", "Hello1")]
        [InlineData("Hello", (ulong)123, "g1", "Hello1e+02")]
        public void Append_ULong_Format(string original, ulong value, string format, string expected)
        {
            var builder = MutableTextBufferFactory(original);
            builder.Append(value, format, CultureInfo.InvariantCulture);
            Assert.Equal(expected, builder.ToString());
        }

        [Fact]
        public void Append_ULong_NoSpareCapacity_ThrowsArgumentOutOfRangeException()
        {
            var builder = MutableTextBufferFactory(0, 5);
            builder.Append("Hello");

            AssertExtensions.Throws<ArgumentOutOfRangeException>(s_noCapacityParamName, () => builder.Append((ulong)1));
        }

        [Theory]
        [InlineData("Hello", '\0', 1, "Hello\0")]
        [InlineData("Hello", 'a', 1, "Helloa")]
        [InlineData("", 'b', 1, "b")]
        [InlineData("Hello", 'c', 2, "Hellocc")]
        [InlineData("Hello", '\0', 0, "Hello")]
        public void Append_Char(string original, char value, int repeatCount, string expected)
        {
            MutableTextBuffer builder;
            if (repeatCount == 1)
            {
                // Use Append(char)
                builder = MutableTextBufferFactory(original);
                builder.Append(value);
                Assert.Equal(expected, builder.ToString());
            }
            // Use Append(char, int)
            builder = MutableTextBufferFactory(original);
            builder.Append(value, repeatCount);
            Assert.Equal(expected, builder.ToString());
        }

        [Fact]
        public void Append_Char_NegativeRepeatCount_ThrowsArgumentOutOfRangeException()
        {
            var builder = MutableTextBufferFactory(0, 5);
            AssertExtensions.Throws<ArgumentOutOfRangeException>("repeatCount", () => builder.Append('a', -1));
        }

        [Fact]
        public void Append_Char_NoSpareCapacity_ThrowsArgumentOutOfRangeException()
        {
            var builder = MutableTextBufferFactory(0, 5);
            builder.Append("Hello");

            AssertExtensions.Throws<ArgumentOutOfRangeException>("requiredLength", () => builder.Append('a'));
            AssertExtensions.Throws<ArgumentOutOfRangeException>("repeatCount", /*"requiredLength",*/ () => builder.Append('a', 1));
        }

        [Theory]
        [InlineData("Hello", new char[] { 'a', 'b', 'c' }, 1, "Helloa")]
        [InlineData("Hello", new char[] { 'a', 'b', 'c' }, 2, "Helloab")]
        [InlineData("Hello", new char[] { 'a', 'b', 'c' }, 3, "Helloabc")]
        [InlineData("", new char[] { 'a' }, 1, "a")]
        [InlineData("", new char[] { 'a' }, 0, "")]
        [InlineData("Hello", new char[0], 0, "Hello")]
        [InlineData("Hello", null, 0, "Hello")]
        public unsafe void Append_CharPointer(string? original, char[]? charArray, int valueCount, string expected)
        {
            _ = charArray; // https://github.com/xunit/xunit/issues/1969
            fixed (char* value = charArray)
            {
                var builder = MutableTextBufferFactory(original);
                builder.Append(value, valueCount);
                Assert.Equal(expected, builder.ToString());
            }
        }

        [Fact]
        public unsafe void Append_CharPointer_Null_ThrowsNullReferenceException()
        {
            var builder = MutableTextBufferFactory();
            Assert.Throws<NullReferenceException>(() => builder.Append(null, 2));
        }

        [Fact]
        public unsafe void Append_CharPointer_NegativeValueCount_ThrowsArgumentOutOfRangeException()
        {
            var builder = MutableTextBufferFactory(0, 5);
            builder.Append("Hello");

            AssertExtensions.Throws<ArgumentOutOfRangeException>("valueCount", () =>
            {
                fixed (char* value = new char[0]) { builder.Append(value, -1); }
            });
        }

        [Fact]
        public unsafe void Append_CharPointer_NoSpareCapacity_ThrowsArgumentOutOfRangeException()
        {
            var builder = MutableTextBufferFactory(0, 5);
            builder.Append("Hello");

            AssertExtensions.Throws<ArgumentOutOfRangeException>(s_noCapacityParamName, () =>
            {
                fixed (char* value = new char[] { 'a' }) { builder.Append(value, 1); }
            });
        }

        [Theory]
        [InlineData(0, 8)]
        [InlineData(0, 4)]
        [InlineData(2, 4)]
        [InlineData(3, 1)]
        [InlineData(7, 1)]
        public unsafe void Append_CharPointer_IsSelf(int sourceIndex, int length)
        {
            const string original = "ABCDEFGH";

            var expected = MutableTextBufferFactory(original);
            expected.Append(original.AsSpan(sourceIndex, length));

            var actual = MutableTextBufferFactory(original);
            fixed (char* p = actual.RawChars)
            {
                actual.Append(p + sourceIndex, length);
            }

            Assert.Equal(expected.ToString(), actual.ToString());
        }

        [Fact]
        public unsafe void Append_CharPointer_SourceIsUnusedBufferInSelf_AppendsCorrectly()
        {
            var sb = MutableTextBufferFactory("12345678", capacity: 32);
            Span<char> source = sb.RawChars.Slice(10, 5);
            "abcde".AsSpan().CopyTo(source);
            fixed (char* p = &MemoryMarshal.GetReference(source))
            {
                sb.Append(p, source.Length);
            }
            Assert.Equal("12345678abcde", sb.ToString());
        }

        [Theory]
        [InlineData(0, 8)]
        [InlineData(0, 4)]
        [InlineData(2, 4)]
        [InlineData(3, 1)]
        public unsafe void Append_CharPointer_IsSelf_GrowingBuffer(int sourceIndex, int length)
        {
            const string original = "ABCDEFGH";

            var expected = MutableTextBufferFactory(original, capacity: original.Length);
            expected.Append(original.AsSpan(sourceIndex, length));

            var actual = MutableTextBufferFactory(original, capacity: original.Length);

            fixed (char* p = actual.RawChars)
            {
                actual.Append(p + sourceIndex, length);
            }

            Assert.Equal(expected.ToString(), actual.ToString());
        }

        [Theory]
        [InlineData("ABCDEFGH", 8, 0, 8)]
        [InlineData("ABCDEFGH", 8, 0, 4)]
        [InlineData("ABCDEFGH", 8, 2, 4)]
        [InlineData("ABCDEFGH", 8, 3, 1)]
        [InlineData("ABCDEFGH", 8, 7, 1)]
        public unsafe void Append_CharPointer_SelfSpan_GrowingBuffer_ShouldNotReadReturnedBuffer(string original, int capacity, int sourceIndex, int length)
        {
            var allocator = new EvilCharArrayAllocator();

            var expected = MutableTextBufferFactory(original);
            expected.Append(original.AsSpan(sourceIndex, length));

            var actual = MutableTextBufferFactory(original, capacity, allocator);

            fixed (char* p = actual.RawChars)
            {
                actual.Append(p + sourceIndex, length);
            }

            Assert.Equal(expected.ToString(), actual.ToString());
        }

        [Theory]
        [InlineData("ABCDEFGH", 8)]
        [InlineData("abcdefghijklmnopqrstuvwxyz", 26)]
        [InlineData("123456789", 9)]
        public unsafe void Append_CharPointer_Self_GrowingBuffer_ShouldNotReadReturnedBuffer(string original, int capacity)
        {
            var allocator = new EvilCharArrayAllocator();

            var expected = MutableTextBufferFactory(original);
            expected.Append(original);

            var actual = MutableTextBufferFactory(original, capacity, allocator);
            fixed (char* p = actual.RawChars)
            {
                actual.Append(p, actual.Length);
            }

            Assert.Equal(expected.ToString(), actual.ToString());
        }

        [Theory]
        [InlineData("12345678", 8, 8, 5, "abcde")]
        [InlineData("ABCDEFGH", 8, 12, 3, "XYZ")]
        [InlineData("Hello", 5, 6, 5, "World")]
        public unsafe void Append_CharPointer_UnusedBuffer_GrowingBuffer_ShouldCopyBeforeGrow(string original, int capacity,
            int sourceOffset, int sourceLength, string sourceValue)
        {
            var allocator = new EvilCharArrayAllocator();

            var builder = MutableTextBufferFactory(original, capacity, allocator);

            sourceValue.AsSpan().CopyTo(
                builder.RawChars.Slice(sourceOffset, sourceLength));

            fixed (char* p = builder.RawChars)
            {
                builder.Append(p + sourceOffset, sourceLength);
            }

            Assert.Equal(original + sourceValue, builder.ToString());
        }


        [Theory]
        [InlineData("Hello", "abc", 0, 3, "Helloabc")]
        [InlineData("Hello", "def", 1, 2, "Helloef")]
        [InlineData("Hello", "def", 2, 1, "Hellof")]
        [InlineData("", "g", 0, 1, "g")]
        [InlineData("Hello", "g", 1, 0, "Hello")]
        [InlineData("Hello", "g", 0, 0, "Hello")]
        [InlineData("Hello", "", 0, 0, "Hello")]
        [InlineData("Hello", null, 0, 0, "Hello")]
        public void Append_String(string? original, string? value, int startIndex, int count, string expected)
        {
            MutableTextBuffer builder;
            if (startIndex == 0 && count == (value?.Length ?? 0))
            {
                // Use Append(string)
                builder = MutableTextBufferFactory(original);
                builder.Append(value);
                Assert.Equal(expected, builder.ToString());
            }
            // Use Append(string, int, int)
            builder = MutableTextBufferFactory(original);
            builder.Append(value, startIndex, count);
            Assert.Equal(expected, builder.ToString());
        }

        [Fact]
        public void Append_String_NullValueNonZeroStartIndexCount_ThrowsArgumentNullException()
        {
            var builder = MutableTextBufferFactory();
            AssertExtensions.Throws<ArgumentNullException>("value", () => builder.Append((string?)null, 1, 1));
        }

        [Theory]
        [InlineData("", -1, 0)]
        [InlineData("hello", 5, 1)]
        [InlineData("hello", 4, 2)]
        public void Append_String_InvalidIndexPlusCount_ThrowsArgumentOutOfRangeException(string value, int startIndex, int count)
        {
            var builder = MutableTextBufferFactory();
            AssertExtensions.Throws<ArgumentOutOfRangeException>("startIndex", () => builder.Append(value, startIndex, count));
        }

        [Fact]
        public void Append_String_NegativeCount_ThrowsArgumentOutOfRangeException()
        {
            var builder = MutableTextBufferFactory();
            AssertExtensions.Throws<ArgumentOutOfRangeException>("count", () => builder.Append("", 0, -1));
        }

        [Fact]
        public void Append_String_NoSpareCapacity_ThrowsArgumentOutOfRangeException()
        {
            var builder = MutableTextBufferFactory(0, 5);
            builder.Append("Hello");

            AssertExtensions.Throws<ArgumentOutOfRangeException>(s_noCapacityParamName, () => builder.Append("a"));
            AssertExtensions.Throws<ArgumentOutOfRangeException>(s_noCapacityParamName, () => builder.Append("a", 0, 1));
        }

        [Theory]
        [InlineData("Hello", new char[] { 'a' }, 0, 1, "Helloa")]
        [InlineData("Hello", new char[] { 'b', 'c', 'd' }, 0, 3, "Hellobcd")]
        [InlineData("Hello", new char[] { 'b', 'c', 'd' }, 1, 2, "Hellocd")]
        [InlineData("Hello", new char[] { 'b', 'c', 'd' }, 2, 1, "Hellod")]
        [InlineData("", new char[] { 'e', 'f', 'g' }, 0, 3, "efg")]
        [InlineData("Hello", new char[] { 'e' }, 1, 0, "Hello")]
        [InlineData("Hello", new char[] { 'e' }, 0, 0, "Hello")]
        [InlineData("Hello", new char[0], 0, 0, "Hello")]
        [InlineData("Hello", null, 0, 0, "Hello")]
        public void Append_CharArray(string? original, char[]? value, int startIndex, int charCount, string expected)
        {
            MutableTextBuffer builder;
            if (startIndex == 0 && charCount == (value?.Length ?? 0))
            {
                // Use Append(char[])
                builder = MutableTextBufferFactory(original);
                builder.Append(value);
                Assert.Equal(expected, builder.ToString());
            }
            // Use Append(char[], int, int)
            builder = MutableTextBufferFactory(original);
            builder.Append(value, startIndex, charCount);
            Assert.Equal(expected, builder.ToString());
        }

        [Fact]
        public void Append_CharArray_Invalid()
        {
            var builder = MutableTextBufferFactory(0, 5);
            builder.Append("Hello");

            AssertExtensions.Throws<ArgumentNullException>("value", () => builder.Append((char[]?)null, 1, 1)); // Value is null, startIndex > 0 and count > 0

            AssertExtensions.Throws<ArgumentOutOfRangeException>("startIndex", () => builder.Append(new char[0], -1, 0)); // Start index < 0
            AssertExtensions.Throws<ArgumentOutOfRangeException>("charCount", () => builder.Append(new char[0], 0, -1)); // Count < 0

            AssertExtensions.Throws<ArgumentOutOfRangeException>("charCount", () => builder.Append(new char[5], 6, 0)); // Start index + count > value.Length
            AssertExtensions.Throws<ArgumentOutOfRangeException>("charCount", () => builder.Append(new char[5], 5, 1)); // Start index + count > value.Length

            AssertExtensions.Throws<ArgumentOutOfRangeException>("valueCount", () => builder.Append(new char[] { 'a' })); // New length > builder.MaxCapacity
            AssertExtensions.Throws<ArgumentOutOfRangeException>("valueCount", () => builder.Append(new char[] { 'a' }, 0, 1)); // New length > builder.MaxCapacity
        }

        public static IEnumerable<object?[]> Test_Append_ICharSequence_TestData()
        {
            yield return new object?[] { "Hello", "a", 0, 1, "Helloa" };
            yield return new object?[] { "Hello", "bcd", 0, 3, "Hellobcd" };
            yield return new object?[] { "Hello", "bcd", 1, 2, "Hellocd" };
            yield return new object?[] { "Hello", "bcd", 2, 1, "Hellod" };
            yield return new object?[] { "", "efg", 0, 3, "efg" };
            yield return new object?[] { "Hello", "e", 1, 0, "Hello" };
            yield return new object?[] { "Hello", "e", 0, 0, "Hello" };
            yield return new object?[] { "Hello", "", 0, 0, "Hello" };
            yield return new object?[] { "Hello", null, 0, 0, "Hello" };
        }

        public static IEnumerable<object?[]> Test_Append_ICharSequence_Typed_TestData()
        {
            foreach (var testCase in Test_Append_ICharSequence_TestData())
            {
                yield return new object?[] { testCase[0], new StringCharSequence((string?)testCase[1]), testCase[2], testCase[3], testCase[4] };
                yield return new object?[] { testCase[0], new StringBuilderCharSequence(new StringBuilder((string?)testCase[1])), testCase[2], testCase[3], testCase[4] };
                yield return new object?[] { testCase[0], new CharArrayCharSequence(((string?)testCase[1])?.ToCharArray()), testCase[2], testCase[3], testCase[4] };
                ReadOnlyMemory<char> memory = ((string?)testCase[1]).AsMemory();
                yield return new object?[] { testCase[0], new MockCharSequence(memory), testCase[2], testCase[3], testCase[4] };
                if (testCase[1] is not null)
                {
                    yield return new object?[] { testCase[0], new StringBuffer((string?)testCase[1]), testCase[2], testCase[3], testCase[4] };
                }
                else
                {
                    yield return new object?[] { testCase[0], (StringCharSequence?)null, testCase[2], testCase[3], testCase[4] };
                    yield return new object?[] { testCase[0], (StringBuilderCharSequence?)null, testCase[2], testCase[3], testCase[4] };
                    yield return new object?[] { testCase[0], (CharArrayCharSequence?)null, testCase[2], testCase[3], testCase[4] };
                    yield return new object?[] { testCase[0], (ICharSequence?)null, testCase[2], testCase[3], testCase[4] };
                }
            }
        }

        [Theory]
        [MemberData(nameof(Test_Append_ICharSequence_Typed_TestData))]
        public void Append_ICharSequence(string? original, ICharSequence? value, int startIndex, int charCount, string expected)
        {
            MutableTextBuffer builder;
            if (startIndex == 0 && charCount == (value?.Length ?? 0))
            {
                // Use Append(char[])
                builder = MutableTextBufferFactory(original);
                builder.Append(value);
                Assert.Equal(expected, builder.ToString());
            }
            // Use Append(char[], int, int)
            builder = MutableTextBufferFactory(original);
            builder.Append(value, startIndex, charCount);
            Assert.Equal(expected, builder.ToString());
        }

        [Fact]
        public void Append_ICharSequence_Invalid()
        {
            var builder = MutableTextBufferFactory(0, 5);
            builder.Append("Hello");

            AssertExtensions.Throws<ArgumentNullException>("value", () => builder.Append((ICharSequence?)null, 1, 1)); // Value is null, startIndex > 0 and count > 0

            AssertExtensions.Throws<ArgumentOutOfRangeException>("startIndex", () => builder.Append(new char[0].AsCharSequence(), -1, 0)); // Start index < 0
            AssertExtensions.Throws<ArgumentOutOfRangeException>("count", () => builder.Append(new char[0].AsCharSequence(), 0, -1)); // Count < 0

            AssertExtensions.Throws<ArgumentOutOfRangeException>("count", () => builder.Append(new char[5].AsCharSequence(), 6, 0)); // Start index + count > value.Length
            AssertExtensions.Throws<ArgumentOutOfRangeException>("count", () => builder.Append(new char[5].AsCharSequence(), 5, 1)); // Start index + count > value.Length

            AssertExtensions.Throws<ArgumentOutOfRangeException>("valueCount", () => builder.Append(new char[] { 'a' }.AsCharSequence())); // New length > builder.MaxCapacity
            AssertExtensions.Throws<ArgumentOutOfRangeException>("valueCount", () => builder.Append(new char[] { 'a' }.AsCharSequence(), 0, 1)); // New length > builder.MaxCapacity
        }

        public static IEnumerable<object[]> Append_Overlapping_TestData()
        {
            yield return new object[] { "abcdefghijklmnopqrstuvwxyz", 0, 5, "abcdefghijklmnopqrstuvwxyzabcde" };
            yield return new object[] { "abcdefghijklmnopqrstuvwxyz", 10, 5, "abcdefghijklmnopqrstuvwxyzklmno" };
            yield return new object[] { "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz", 10, 5, "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzklmno" };
            yield return new object[] { "abcdefghijklmnopqrstuvwxyz", 23, 3, "abcdefghijklmnopqrstuvwxyzxyz" };
            yield return new object[] { "abcdefghijklmnopqrstuvwxyz", 5, 10, "abcdefghijklmnopqrstuvwxyzfghijklmno" };
            yield return new object[] { "abcdefghijklmnopqrstuvwxyz", 8, 6, "abcdefghijklmnopqrstuvwxyzijklmn" };
            yield return new object[] { "abcdefghijklmnopqrstuvwxyz", 0, 26, "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz" };
            yield return new object[] { "abcdefghijklmnopqrstuvwxyz", 5, 0, "abcdefghijklmnopqrstuvwxyz" };
            yield return new object[] { "", 0, 0, "" };
        }

        [Theory]
        [MemberData(nameof(Append_Overlapping_TestData))]
        public void Append_ICharSequence_Overlapping(string value, int sourceIndex, int sourceLength, string expected)
        {
            {
                MutableTextBuffer builder = MutableTextBufferFactory(value);
                ReadOnlyMemory<char> memory = builder.AsMemory(sourceIndex, sourceLength);
                ICharSequence sequence = new SpannableCharSequence(memory);
                builder.Append(sequence);
                Assert.Equal(expected, builder.ToString());
            }

            {
                MutableTextBuffer builder = MutableTextBufferFactory(value);
                ReadOnlyMemory<char> memory = builder.AsMemory(sourceIndex, sourceLength);
                ICharSequence sequence = new CopyableCharSequence(memory);
                builder.Append(sequence);
                Assert.Equal(expected, builder.ToString());
            }

            {
                MutableTextBuffer builder = MutableTextBufferFactory(value);
                ReadOnlyMemory<char> memory = builder.AsMemory(sourceIndex, sourceLength);
                ICharSequence sequence = new SpanCopyableCharSequence(memory);
                builder.Append(sequence);
                Assert.Equal(expected, builder.ToString());
            }

            {
                MutableTextBuffer builder = MutableTextBufferFactory(value);
                ReadOnlyMemory<char> memory = builder.AsMemory(sourceIndex, sourceLength);
                ICharSequence sequence = new SimpleCharSequence(memory);
                builder.Append(sequence);
                Assert.Equal(expected, builder.ToString());
            }
        }

        [Theory]
        [MemberData(nameof(Append_Overlapping_TestData))]
        public void Append_ICharSequence_Int32_Int32_Overlapping(string value, int sourceIndex, int sourceLength, string expected)
        {
            {
                MutableTextBuffer builder = MutableTextBufferFactory(value);
                ReadOnlyMemory<char> memory = builder.AsMemory();
                ICharSequence sequence = new SpannableCharSequence(memory);
                builder.Append(sequence, sourceIndex, sourceLength);
                Assert.Equal(expected, builder.ToString());
            }

            {
                MutableTextBuffer builder = MutableTextBufferFactory(value);
                ReadOnlyMemory<char> memory = builder.AsMemory();
                ICharSequence sequence = new CopyableCharSequence(memory);
                builder.Append(sequence, sourceIndex, sourceLength);
                Assert.Equal(expected, builder.ToString());
            }

            {
                MutableTextBuffer builder = MutableTextBufferFactory(value);
                ReadOnlyMemory<char> memory = builder.AsMemory();
                ICharSequence sequence = new SpanCopyableCharSequence(memory);
                builder.Append(sequence, sourceIndex, sourceLength);
                Assert.Equal(expected, builder.ToString());
            }

            {
                MutableTextBuffer builder = MutableTextBufferFactory(value);
                ReadOnlyMemory<char> memory = builder.AsMemory();
                ICharSequence sequence = new SimpleCharSequence(memory);
                builder.Append(sequence, sourceIndex, sourceLength);
                Assert.Equal(expected, builder.ToString());
            }
        }

#nullable disable

        public static IEnumerable<object[]> AppendFormat_TestData()
        {
            yield return new object[] { "", null, "", new object[0], "" };
            yield return new object[] { "", null, ", ", new object[0], ", " };

            yield return new object[] { "Hello", null, ", Foo {0  }", new object[] { "Bar" }, "Hello, Foo Bar" }; // Ignores whitespace

            yield return new object[] { "Hello", null, ", Foo {0}", new object[] { "Bar" }, "Hello, Foo Bar" };
            yield return new object[] { "Hello", null, ", Foo {0} Baz {1}", new object[] { "Bar", "Foo" }, "Hello, Foo Bar Baz Foo" };
            yield return new object[] { "Hello", null, ", Foo {0} Baz {1} Bar {2}", new object[] { "Bar", "Foo", "Baz" }, "Hello, Foo Bar Baz Foo Bar Baz" };
            yield return new object[] { "Hello", null, ", Foo {0} Baz {1} Bar {2} Foo {3}", new object[] { "Bar", "Foo", "Baz", "Bar" }, "Hello, Foo Bar Baz Foo Bar Baz Foo Bar" };

            // Length is positive
            yield return new object[] { "Hello", null, ", Foo {0,2}", new object[] { "Bar" }, "Hello, Foo Bar" }; // MiValue's length > minimum length (so don't prepend whitespace)
            yield return new object[] { "Hello", null, ", Foo {0,3}", new object[] { "B" }, "Hello, Foo   B" }; // Value's length < minimum length (so prepend whitespace)
            yield return new object[] { "Hello", null, ", Foo {0,     3}", new object[] { "B" }, "Hello, Foo   B" }; // Same as above, but verify AppendFormat ignores whitespace
            yield return new object[] { "Hello", null, ", Foo {0,0}", new object[] { "Bar" }, "Hello, Foo Bar" }; // Minimum length is 0
            yield return new object[] { "Hello", null, ", Foo {0,  2 }", new object[] { "Bar" }, "Hello, Foo Bar" }; // whitespace before and after length

            // Length is negative
            yield return new object[] { "Hello", null, ", Foo {0,-2}", new object[] { "Bar" }, "Hello, Foo Bar" }; // Value's length > |minimum length| (so don't prepend whitespace)
            yield return new object[] { "Hello", null, ", Foo {0,-3}", new object[] { "B" }, "Hello, Foo B  " }; // Value's length < |minimum length| (so append whitespace)
            yield return new object[] { "Hello", null, ", Foo {0,     -3}", new object[] { "B" }, "Hello, Foo B  " }; // Same as above, but verify AppendFormat ignores whitespace
            yield return new object[] { "Hello", null, ", Foo {0,0}", new object[] { "Bar" }, "Hello, Foo Bar" }; // Minimum length is 0
            yield return new object[] { "Hello", null, ", Foo {0, -2  }", new object[] { "Bar" }, "Hello, Foo Bar" }; // whitespace before and after length

            yield return new object[] { "Hello", null, ", Foo {0:D6}", new object[] { 1 }, "Hello, Foo 000001" }; // Custom format
            yield return new object[] { "Hello", null, ", Foo {0     :D6}", new object[] { 1 }, "Hello, Foo 000001" }; // Custom format with ignored whitespace
            yield return new object[] { "Hello", null, ", Foo {0:}", new object[] { 1 }, "Hello, Foo 1" }; // Missing custom format

            yield return new object[] { "Hello", null, ", Foo {0,9:D6}", new object[] { 1 }, "Hello, Foo    000001" }; // Positive minimum length and custom format
            yield return new object[] { "Hello", null, ", Foo {0,-9:D6}", new object[] { 1 }, "Hello, Foo 000001   " }; // Negative length and custom format

            yield return new object[] { "Hello", null, ", Foo {{{0}", new object[] { 1 }, "Hello, Foo {1" }; // Escaped open curly braces
            yield return new object[] { "Hello", null, ", Foo }}{0}", new object[] { 1 }, "Hello, Foo }1" }; // Escaped closed curly braces
            yield return new object[] { "Hello", null, ", Foo {0} {{0}}", new object[] { 1 }, "Hello, Foo 1 {0}" }; // Escaped placeholder


            yield return new object[] { "Hello", null, ", Foo {0}", new object[] { null }, "Hello, Foo " }; // Values has null only
            yield return new object[] { "Hello", null, ", Foo {0} {1} {2}", new object[] { "Bar", null, "Baz" }, "Hello, Foo Bar  Baz" }; // Values has null

            yield return new object[] { "Hello", CultureInfo.InvariantCulture, ", Foo {0,9:D6}", new object[] { 1 }, "Hello, Foo    000001" }; // Positive minimum length, custom format and custom format provider

            yield return new object[] { "", new CustomFormatter(), "{0}", new object[] { 1.2 }, "abc" }; // Custom format provider
            yield return new object[] { "", new CustomFormatter(), "{0:0}", new object[] { 1.2 }, "abc" }; // Custom format provider

            // ISpanFormattable inputs: simple validation of known types that implement the interface
            yield return new object[] { "", CultureInfo.InvariantCulture, "{0}", new object[] { (byte)42 }, "42" };
            yield return new object[] { "", CultureInfo.InvariantCulture, "{0}", new object[] { 'A' }, "A" };
            yield return new object[] { "", CultureInfo.InvariantCulture, "{0:r}", new object[] { DateTime.ParseExact("2021-03-15T14:52:51.5058563Z", "o", null, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal) }, "Mon, 15 Mar 2021 14:52:51 GMT" };
            yield return new object[] { "", CultureInfo.InvariantCulture, "{0:r}", new object[] { DateTimeOffset.ParseExact("2021-03-15T14:52:51.5058563Z", "o", null, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal) }, "Mon, 15 Mar 2021 14:52:51 GMT" };
            yield return new object[] { "", CultureInfo.InvariantCulture, "{0}", new object[] { (decimal)42 }, "42" };
            yield return new object[] { "", CultureInfo.InvariantCulture, "{0}", new object[] { (double)42 }, "42" };
            yield return new object[] { "", CultureInfo.InvariantCulture, "{0}", new object[] { Guid.Parse("68d9cfaf-feab-4d5b-96d8-a3fd889ae89f") }, "68d9cfaf-feab-4d5b-96d8-a3fd889ae89f" };
#if FEATURE_HALF
            yield return new object[] { "", CultureInfo.InvariantCulture, "{0}", new object[] { (Half)42 }, "42" };
#endif
            yield return new object[] { "", CultureInfo.InvariantCulture, "{0}", new object[] { (short)42 }, "42" };
            yield return new object[] { "", CultureInfo.InvariantCulture, "{0}", new object[] { (int)42 }, "42" };
            yield return new object[] { "", CultureInfo.InvariantCulture, "{0}", new object[] { (long)42 }, "42" };
            yield return new object[] { "", CultureInfo.InvariantCulture, "{0}", new object[] { (IntPtr)42 }, "42" };
#if FEATURE_RUNE
            yield return new object[] { "", CultureInfo.InvariantCulture, "{0}", new object[] { new Rune('A') }, "A" };
#endif
            yield return new object[] { "", CultureInfo.InvariantCulture, "{0}", new object[] { (sbyte)42 }, "42" };
            yield return new object[] { "", CultureInfo.InvariantCulture, "{0}", new object[] { (float)42 }, "42" };
            yield return new object[] { "", CultureInfo.InvariantCulture, "{0}", new object[] { TimeSpan.FromSeconds(42) }, "00:00:42" };
            yield return new object[] { "", CultureInfo.InvariantCulture, "{0}", new object[] { (ushort)42 }, "42" };
            yield return new object[] { "", CultureInfo.InvariantCulture, "{0}", new object[] { (uint)42 }, "42" };
            yield return new object[] { "", CultureInfo.InvariantCulture, "{0}", new object[] { (ulong)42 }, "42" };
            yield return new object[] { "", CultureInfo.InvariantCulture, "{0}", new object[] { (UIntPtr)42 }, "42" };
            yield return new object[] { "", CultureInfo.InvariantCulture, "{0}", new object[] { new Version(1, 2, 3, 4) }, "1.2.3.4" };
        }

#nullable enable

        [Theory]
        [MemberData(nameof(AppendFormat_TestData))]
        public void AppendFormat(string original, IFormatProvider provider, string format, object?[]? values, string expected)
        {
            MutableTextBuffer builder;
            if (values != null)
            {
                if (values.Length == 1)
                {
                    // Use AppendFormat(string, object) or AppendFormat(IFormatProvider, string, object)
                    if (provider == null)
                    {
                        // Use AppendFormat(string, object)
                        builder = MutableTextBufferFactory(original);
                        builder.AppendFormat(format, values[0]);
                        Assert.Equal(expected, builder.ToString());
                    }
                    // Use AppendFormat(IFormatProvider, string, object)
                    builder = MutableTextBufferFactory(original);
                    builder.AppendFormat(provider, format, values[0]);
                    Assert.Equal(expected, builder.ToString());
                }
                else if (values.Length == 2)
                {
                    // Use AppendFormat(string, object, object) or AppendFormat(IFormatProvider, string, object, object)
                    if (provider == null)
                    {
                        // Use AppendFormat(string, object, object)
                        builder = MutableTextBufferFactory(original);
                        builder.AppendFormat(format, values[0], values[1]);
                        Assert.Equal(expected, builder.ToString());
                    }
                    // Use AppendFormat(IFormatProvider, string, object, object)
                    builder = MutableTextBufferFactory(original);
                    builder.AppendFormat(provider, format, values[0], values[1]);
                    Assert.Equal(expected, builder.ToString());
                }
                else if (values.Length == 3)
                {
                    // Use AppendFormat(string, object, object, object) or AppendFormat(IFormatProvider, string, object, object, object)
                    if (provider == null)
                    {
                        // Use AppendFormat(string, object, object, object)
                        builder = MutableTextBufferFactory(original);
                        builder.AppendFormat(format, values[0], values[1], values[2]);
                        Assert.Equal(expected, builder.ToString());
                    }
                    // Use AppendFormat(IFormatProvider, string, object, object, object)
                    builder = MutableTextBufferFactory(original);
                    builder.AppendFormat(provider, format, values[0], values[1], values[2]);
                    Assert.Equal(expected, builder.ToString());
                }
            }
            // Use AppendFormat(string, object[]) or AppendFormat(IFormatProvider, string, object[])
            if (provider == null)
            {
                // Use AppendFormat(string, object[])
                builder = MutableTextBufferFactory(original);
                builder.AppendFormat(format, values!);
                Assert.Equal(expected, builder.ToString());
            }
            // Use AppendFormat(IFormatProvider, string, object[])
            builder = MutableTextBufferFactory(original);
            builder.AppendFormat(provider, format, values!);
            Assert.Equal(expected, builder.ToString());

            // Use AppendFormat(string, ReadOnlySpan<object>) or AppendFormat(IFormatProvider, string, ReadOnlySpan<object>)
            if (provider == null)
            {
                // Use AppendFormat(string, ReadOnlySpan<object>)
                builder = MutableTextBufferFactory(original);
                builder.AppendFormat(format, (ReadOnlySpan<object?>)values);
                Assert.Equal(expected, builder.ToString());
            }
            // Use AppendFormat(IFormatProvider, string, ReadOnlySpan<object>)
            builder = MutableTextBufferFactory(original);
            builder.AppendFormat(provider, format, (ReadOnlySpan<object?>)values);
            Assert.Equal(expected, builder.ToString());
        }

#nullable disable

        [Fact]
        public void AppendFormat_Invalid()
        {
            var builder = MutableTextBufferFactory(0, 5);
            builder.Append("Hello");

            IFormatProvider formatter = null;
            var obj1 = new object();
            var obj2 = new object();
            var obj3 = new object();
            var obj4 = new object();
            var objArray = new object[] { obj1, obj2, obj3, obj4 };

            AssertExtensions.Throws<ArgumentNullException>("format", () => builder.AppendFormat(null, obj1)); // Format is null
            AssertExtensions.Throws<ArgumentNullException>("format", () => builder.AppendFormat(null, obj1, obj2, obj3)); // Format is null
            AssertExtensions.Throws<ArgumentNullException>("format", () => builder.AppendFormat(null, obj1, obj2, obj3, obj4)); // Format is null
            AssertExtensions.Throws<ArgumentNullException>("format", () => builder.AppendFormat(null, objArray)); // Format is null
            AssertExtensions.Throws<ArgumentNullException>("format", () => builder.AppendFormat(null, (ReadOnlySpan<object>)objArray)); // Format is null
            AssertExtensions.Throws<ArgumentNullException>("args", () => builder.AppendFormat("", null)); // Args is null
            AssertExtensions.Throws<ArgumentNullException>("format", () => builder.AppendFormat(null, (object[])null)); // Both format and args are null
            AssertExtensions.Throws<ArgumentNullException>("format", () => builder.AppendFormat(formatter, (string)null, obj1)); // Format is null
            AssertExtensions.Throws<ArgumentNullException>("format", () => builder.AppendFormat(formatter, (string)null, obj1, obj2)); // Format is null
            AssertExtensions.Throws<ArgumentNullException>("format", () => builder.AppendFormat(formatter, (string)null, obj1, obj2, obj3)); // Format is null
            AssertExtensions.Throws<ArgumentNullException>("format", () => builder.AppendFormat(formatter, (string)null, obj1, obj2, obj3, obj4)); // Format is null
            AssertExtensions.Throws<ArgumentNullException>("format", () => builder.AppendFormat(formatter, (string)null, objArray)); // Format is null
            AssertExtensions.Throws<ArgumentNullException>("format", () => builder.AppendFormat(formatter, (string)null, (ReadOnlySpan<object>)objArray)); // Format is null
            AssertExtensions.Throws<ArgumentNullException>("args", () => builder.AppendFormat(formatter, "", null)); // Args is null
            AssertExtensions.Throws<ArgumentNullException>("format", () => builder.AppendFormat(formatter, (string)null, null)); // Both format and args are null

            Assert.Throws<FormatException>(() => builder.AppendFormat("{-1}", obj1)); // Format has value < 0
            Assert.Throws<FormatException>(() => builder.AppendFormat("{-1}", obj1, obj2)); // Format has value < 0
            Assert.Throws<FormatException>(() => builder.AppendFormat("{-1}", obj1, obj2, obj3)); // Format has value < 0
            Assert.Throws<FormatException>(() => builder.AppendFormat("{-1}", obj1, obj2, obj3, obj4)); // Format has value < 0
            Assert.Throws<FormatException>(() => builder.AppendFormat("{-1}", objArray)); // Format has value < 0
            Assert.Throws<FormatException>(() => builder.AppendFormat("{-1}", (ReadOnlySpan<object>)objArray)); // Format has value < 0
            Assert.Throws<FormatException>(() => builder.AppendFormat(formatter, "{-1}", obj1)); // Format has value < 0
            Assert.Throws<FormatException>(() => builder.AppendFormat(formatter, "{-1}", obj1, obj2)); // Format has value < 0
            Assert.Throws<FormatException>(() => builder.AppendFormat(formatter, "{-1}", obj1, obj2, obj3)); // Format has value < 0
            Assert.Throws<FormatException>(() => builder.AppendFormat(formatter, "{-1}", obj1, obj2, obj3, obj4)); // Format has value < 0
            Assert.Throws<FormatException>(() => builder.AppendFormat(formatter, "{-1}", objArray)); // Format has value < 0
            Assert.Throws<FormatException>(() => builder.AppendFormat(formatter, "{-1}", (ReadOnlySpan<object>)objArray)); // Format has value < 0
            Assert.Throws<FormatException>(() => builder.AppendFormat("{1}", obj1)); // Format has value >= 1
            Assert.Throws<FormatException>(() => builder.AppendFormat("{2}", obj1, obj2)); // Format has value >= 2
            Assert.Throws<FormatException>(() => builder.AppendFormat("{3}", obj1, obj2, obj3)); // Format has value >= 3
            Assert.Throws<FormatException>(() => builder.AppendFormat("{4}", obj1, obj2, obj3, obj4)); // Format has value >= 4
            Assert.Throws<FormatException>(() => builder.AppendFormat("{4}", objArray)); // Format has value >= 4
            Assert.Throws<FormatException>(() => builder.AppendFormat("{4}", (ReadOnlySpan<object>)objArray)); // Format has value >= 4
            Assert.Throws<FormatException>(() => builder.AppendFormat(formatter, "{1}", obj1)); // Format has value >= 1
            Assert.Throws<FormatException>(() => builder.AppendFormat(formatter, "{2}", obj1, obj2)); // Format has value >= 2
            Assert.Throws<FormatException>(() => builder.AppendFormat(formatter, "{3}", obj1, obj2, obj3)); // Format has value >= 3
            Assert.Throws<FormatException>(() => builder.AppendFormat(formatter, "{4}", obj1, obj2, obj3, obj4)); // Format has value >= 4
            Assert.Throws<FormatException>(() => builder.AppendFormat(formatter, "{4}", objArray)); // Format has value >= 4
            Assert.Throws<FormatException>(() => builder.AppendFormat(formatter, "{4}", (ReadOnlySpan<object>)objArray)); // Format has value >= 4

            Assert.Throws<FormatException>(() => builder.AppendFormat("{", "")); // Format has unescaped {
            Assert.Throws<FormatException>(() => builder.AppendFormat("{a", "")); // Format has unescaped {

            Assert.Throws<FormatException>(() => builder.AppendFormat("}", "")); // Format has unescaped }
            Assert.Throws<FormatException>(() => builder.AppendFormat("}a", "")); // Format has unescaped }
            Assert.Throws<FormatException>(() => builder.AppendFormat("{0:}}", "")); // Format has unescaped }

            Assert.Throws<FormatException>(() => builder.AppendFormat("{\0", "")); // Format has invalid character after {
            Assert.Throws<FormatException>(() => builder.AppendFormat("{a", "")); // Format has invalid character after {

            Assert.Throws<FormatException>(() => builder.AppendFormat("{0     ", "")); // Format with index and spaces is not closed

            Assert.Throws<FormatException>(() => builder.AppendFormat("{1000000", new string[10])); // Format index is too long
            Assert.Throws<FormatException>(() => builder.AppendFormat("{1000000", (ReadOnlySpan<object>)new string[10])); // Format index is too long

            Assert.Throws<FormatException>(() => builder.AppendFormat("{10000000}", new string[10])); // Format index is too long
            Assert.Throws<FormatException>(() => builder.AppendFormat("{10000000}", (ReadOnlySpan<object>)new string[10])); // Format index is too long

            Assert.Throws<FormatException>(() => builder.AppendFormat("{0,", "")); // Format with comma is not closed
            Assert.Throws<FormatException>(() => builder.AppendFormat("{0,   ", "")); // Format with comma and spaces is not closed
            Assert.Throws<FormatException>(() => builder.AppendFormat("{0,-", "")); // Format with comma and minus sign is not closed

            Assert.Throws<FormatException>(() => builder.AppendFormat("{0,-\0", "")); // Format has invalid character after minus sign
            Assert.Throws<FormatException>(() => builder.AppendFormat("{0,-a", "")); // Format has invalid character after minus sign

            Assert.Throws<FormatException>(() => builder.AppendFormat("{0,1000000", new string[10])); // Format length is too long
            Assert.Throws<FormatException>(() => builder.AppendFormat("{0,1000000", (ReadOnlySpan<object>)new string[10])); // Format length is too long

            Assert.Throws<FormatException>(() => builder.AppendFormat("{0,10000000}", new string[10])); // Format length is too long
            Assert.Throws<FormatException>(() => builder.AppendFormat("{0,10000000}", (ReadOnlySpan<object>)new string[10])); // Format length is too long

            Assert.Throws<FormatException>(() => builder.AppendFormat("{0:", new string[10])); // Format with colon is not closed
            Assert.Throws<FormatException>(() => builder.AppendFormat("{0:", (ReadOnlySpan<object>)new string[10])); // Format with colon is not closed

            Assert.Throws<FormatException>(() => builder.AppendFormat("{0:    ", new string[10])); // Format with colon and spaces is not closed
            Assert.Throws<FormatException>(() => builder.AppendFormat("{0:    ", (ReadOnlySpan<object>)new string[10])); // Format with colon and spaces is not closed

            Assert.Throws<FormatException>(() => builder.AppendFormat("{0:{", new string[10])); // Format with custom format contains unescaped {
            Assert.Throws<FormatException>(() => builder.AppendFormat("{0:{", (ReadOnlySpan<object>)new string[10])); // Format with custom format contains unescaped {

            Assert.Throws<FormatException>(() => builder.AppendFormat("{0:{}", new string[10])); // Format with custom format contains unescaped {
            Assert.Throws<FormatException>(() => builder.AppendFormat("{0:{}", (ReadOnlySpan<object>)new string[10])); // Format with custom format contains unescaped {

            Assert.Throws<FormatException>(() => builder.AppendFormat("{0}", new TooManyCharsWrittenSpanFormattable())); // ISpanFormattable that returns more characters than it actually wrote
        }
#nullable enable

        [Fact]
        public void AppendFormat_UsesAmbientCulture_WhenInvariantDefaultsDisabled()
        {
            using var ambientCulture = new ThreadCultureChange("de-DE");
            var sb = MutableTextBufferFactory(new MutableTextBufferTestOptions { UseInvariantDefaults = false });
            sb.AppendFormat("{0:N2}", 1234.5);
            Assert.Equal("1.234,50", sb.ToString());
        }

        [Fact]
        public void AppendFormat_UsesInvariantCulture_WhenInvariantDefaultsEnabled()
        {
            using var ambientCulture = new ThreadCultureChange("de-DE");
            var sb = MutableTextBufferFactory(new MutableTextBufferTestOptions { UseInvariantDefaults = true });
            sb.AppendFormat("{0:N2}", 1234.5);
            Assert.Equal("1,234.50", sb.ToString());
        }

        [Fact]
        public void AppendFormat_ExplicitProviderOverridesInvariantDefaults()
        {
            using var ambientCulture = new ThreadCultureChange("en-US");
            var sb = MutableTextBufferFactory(new MutableTextBufferTestOptions { UseInvariantDefaults = true });
            sb.AppendFormat(new CultureInfo("de-DE"), "{0:N2}", 1234.5);
            Assert.Equal("1.234,50", sb.ToString());
        }

        [Fact]
        public void AppendFormat_ExplicitInvariantProviderOverridesAmbientCulture()
        {
            using var ambientCulture = new ThreadCultureChange("de-DE");
            var sb = MutableTextBufferFactory(new MutableTextBufferTestOptions { UseInvariantDefaults = false });
            sb.AppendFormat(CultureInfo.InvariantCulture,"{0:N2}", 1234.5);
            Assert.Equal("1,234.50", sb.ToString());
        }

#if FEATURE_SPANFORMATTABLE
        private readonly struct TooManyCharsWrittenSpanFormattable : ISpanFormattable
        {
            public string ToString(string? format, IFormatProvider? formatProvider) => "abc";
            public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
            {
                "abc".AsSpan().TryCopyTo(destination);
                charsWritten = 1_000_000;
                return true;
            }
        }
#else
        private class TooManyCharsWrittenSpanFormattable : Number
        {
            public override string ToString(string? format, IFormatProvider? provider) => "abc";

            public override bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
            {
                "abc".AsSpan().TryCopyTo(destination);
                charsWritten = 1_000_000;
                return true;
            }
            public override double ToDouble() => throw new NotSupportedException();

            public override int ToInt32() => throw new NotSupportedException();

            public override long ToInt64() => throw new NotSupportedException();

            public override float ToSingle() => throw new NotSupportedException();
        }
#endif


        [Fact]
        public void AppendFormat_NoEscapedBracesInCustomFormatSpecifier()
        {
            // Tests new rule which does not allow escaped braces in the custom format specifier
            var builder = MutableTextBufferFactory();
            builder.AppendFormat("{0:}}}", 0);

            // Previous behavior: first two closing braces would be escaped and passed in as the custom format specifier, thus result = "}"
            // New behavior: first closing brace closes the argument hole and next two are escaped as part of the format, thus result = "0}"
            Assert.Equal("0}", builder.ToString());
            // Previously this would be allowed and escaped brace would be passed into the custom format, now this is unsupported
            Assert.Throws<FormatException>(() => builder.AppendFormat("{0:{{}", 0)); // Format with custom format contains {
        }

        [Fact]
        public void AppendFormat_NewLengthGreaterThanBuilderLength_ThrowsArgumentOutOfRangeException()
        {
            var builder = MutableTextBufferFactory(0, 5);
            IFormatProvider? formatter = null;
            builder.Append("Hello");

            AssertExtensions.Throws<ArgumentOutOfRangeException>(s_noCapacityParamName, () => builder.AppendFormat("{0}", "a"));
            AssertExtensions.Throws<ArgumentOutOfRangeException>(s_noCapacityParamName, () => builder.AppendFormat("{0}", "a", ""));
            AssertExtensions.Throws<ArgumentOutOfRangeException>(s_noCapacityParamName, () => builder.AppendFormat("{0}", "a", "", ""));
            AssertExtensions.Throws<ArgumentOutOfRangeException>(s_noCapacityParamName, () => builder.AppendFormat("{0}", "a", "", "", ""));
            AssertExtensions.Throws<ArgumentOutOfRangeException>(s_noCapacityParamName, () => builder.AppendFormat(formatter, "{0}", "a"));
            AssertExtensions.Throws<ArgumentOutOfRangeException>(s_noCapacityParamName, () => builder.AppendFormat(formatter, "{0}", "a", ""));
            AssertExtensions.Throws<ArgumentOutOfRangeException>(s_noCapacityParamName, () => builder.AppendFormat(formatter, "{0}", "a", "", ""));
            AssertExtensions.Throws<ArgumentOutOfRangeException>(s_noCapacityParamName, () => builder.AppendFormat(formatter, "{0}", "a", "", "", ""));
        }

        public static IEnumerable<object?[]> AppendLine_TestData()
        {
            yield return new object?[] { "Hello", "abc", "Helloabc" + Environment.NewLine };
            yield return new object?[] { "Hello", "", "Hello" + Environment.NewLine };
            yield return new object?[] { "Hello", null, "Hello" + Environment.NewLine };
        }

        [Theory]
        [MemberData(nameof(AppendLine_TestData))]
        public void AppendLine_String(string? original, string? value, string expected)
        {
            MutableTextBuffer builder;
            if (string.IsNullOrEmpty(value))
            {
                // Use AppendLine()
                builder = MutableTextBufferFactory(original);
                builder.AppendLine();
                Assert.Equal(expected, builder.ToString());
            }
            // Use AppendLine(string)
            builder = MutableTextBufferFactory(original);
            builder.AppendLine(value);
            Assert.Equal(expected, builder.ToString());
        }

        [Fact]
        public void AppendLine_String_NoSpareCapacity_ThrowsArgumentOutOfRangeException()
        {
            var builder = MutableTextBufferFactory(0, 5);
            builder.Append("Hello");

            AssertExtensions.Throws<ArgumentOutOfRangeException>(s_noCapacityParamName, () => builder.AppendLine());
            AssertExtensions.Throws<ArgumentOutOfRangeException>(s_noCapacityParamName, () => builder.AppendLine("a"));
        }

        [Theory]
        [MemberData(nameof(AppendLine_TestData))]
        public void AppendLine_ReadOnlySpan(string? original, string? value, string expected)
        {
            MutableTextBuffer builder;
            if (string.IsNullOrEmpty(value))
            {
                // Use AppendLine()
                builder = MutableTextBufferFactory(original);
                builder.AppendLine();
                Assert.Equal(expected, builder.ToString());
            }
            // Use AppendLine(ReadOnlySpan<char>)
            builder = MutableTextBufferFactory(original);
            builder.AppendLine(value.AsSpan());
            Assert.Equal(expected, builder.ToString());
        }

        [Fact]
        public void AppendLine_ReadOnlySpan_NoSpareCapacity_ThrowsArgumentOutOfRangeException()
        {
            var builder = MutableTextBufferFactory(0, 5);
            builder.Append("Hello");

            AssertExtensions.Throws<ArgumentOutOfRangeException>(s_noCapacityParamName, () => builder.AppendLine());
            AssertExtensions.Throws<ArgumentOutOfRangeException>(s_noCapacityParamName, () => builder.AppendLine("a".AsSpan()));
        }

        [Fact]
        public void Clear()
        {
            var builder = MutableTextBufferFactory("Hello");
            builder.Clear();
            Assert.Equal(0, builder.Length);
            Assert.Same(string.Empty, builder.ToString());
        }

        [Fact]
        public void Clear_Empty_CapacityNotZero()
        {
            var builder = MutableTextBufferFactory();
            builder.Clear();
            Assert.NotEqual(0, builder.Capacity);
        }

        [Fact]
        public void Clear_Empty_CapacityStaysUnchanged()
        {
            var sb = MutableTextBufferFactory(14);
            sb.Clear();
            Assert.Equal(14, sb.Capacity);
        }

        [Fact]
        public void Clear_Full_CapacityStaysUnchanged()
        {
            var sb = MutableTextBufferFactory(14);
            sb.Append("Hello World!!!");
            sb.Clear();
            Assert.Equal(14, sb.Capacity);
        }

        [Fact]
        public void Clear_AtMaxCapacity_CapacityStaysUnchanged()
        {
            var builder = MutableTextBufferFactory(14, 14);
            builder.Append("Hello World!!!");
            builder.Clear();
            Assert.Equal(14, builder.Capacity);
        }

        [Theory]
        [InlineData("Hello", 0, new char[] { '\0', '\0', '\0', '\0', '\0' }, 0, 5, new char[] { 'H', 'e', 'l', 'l', 'o' })]
        [InlineData("Hello", 0, new char[] { '\0', '\0', '\0', '\0', '\0', '\0' }, 1, 5, new char[] { '\0', 'H', 'e', 'l', 'l', 'o' })]
        [InlineData("Hello", 0, new char[] { '\0', '\0', '\0', '\0' }, 0, 4, new char[] { 'H', 'e', 'l', 'l' })]
        [InlineData("Hello", 1, new char[] { '\0', '\0', '\0', '\0', '\0', '\0', '\0' }, 2, 4, new char[] { '\0', '\0', 'e', 'l', 'l', 'o', '\0' })]
        public void CopyTo(string value, int sourceIndex, char[] destination, int destinationIndex, int count, char[] expected)
        {
            var builder = MutableTextBufferFactory(value);
            builder.CopyTo(sourceIndex, destination, destinationIndex, count);
            Assert.Equal(expected, destination);
        }

        // J2N TODO: Multiple chunk tests?
        //[Fact]
        //public void CopyTo_StringBuilderWithMultipleChunks()
        //{
        //    MutableTextBuffer builder = StringBuilderWithMultipleChunks();
        //    char[] destination = new char[builder.Length];
        //    builder.CopyTo(0, destination, 0, destination.Length);
        //    Assert.Equal(s_chunkSplitSource.ToCharArray(), destination);
        //}

        [Fact]
        public void CopyTo_Invalid()
        {
            var builder = MutableTextBufferFactory("Hello");
            AssertExtensions.Throws<ArgumentNullException>("destination", () => builder.CopyTo(0, null!, 0, 0)); // Destination is null

            AssertExtensions.Throws<ArgumentOutOfRangeException>("sourceIndex", () => builder.CopyTo(-1, new char[10], 0, 0)); // Source index < 0
            AssertExtensions.Throws<ArgumentOutOfRangeException>("sourceIndex", () => builder.CopyTo(6, new char[10], 0, 0)); // Source index > builder.Length

            AssertExtensions.Throws<ArgumentOutOfRangeException>("destinationIndex", () => builder.CopyTo(0, new char[10], -1, 0)); // Destination index < 0
            AssertExtensions.Throws<ArgumentOutOfRangeException>("count", () => builder.CopyTo(0, new char[10], 0, -1)); // Count < 0

            AssertExtensions.Throws<ArgumentException>(null, () => builder.CopyTo(5, new char[10], 0, 1)); // Source index + count > builder.Length
            AssertExtensions.Throws<ArgumentException>(null, () => builder.CopyTo(4, new char[10], 0, 2)); // Source index + count > builder.Length

            AssertExtensions.Throws<ArgumentException>(null, () => builder.CopyTo(0, new char[10], 10, 1)); // Destination index + count > destinationArray.Length
            AssertExtensions.Throws<ArgumentException>(null, () => builder.CopyTo(0, new char[10], 9, 2)); // Destination index + count > destinationArray.Length
        }

        [Fact]
        public void EnsureCapacity()
        {
            var builder = MutableTextBufferFactory(40);

            builder.EnsureCapacity(20);
            Assert.True(builder.Capacity >= 20);

            builder.EnsureCapacity(20000);
            Assert.True(builder.Capacity >= 20000);

            // Ensuring a capacity less than the current capacity does not change anything
            int oldCapacity = builder.Capacity;
            builder.EnsureCapacity(10);
            Assert.Equal(oldCapacity, builder.Capacity);
        }

        [Fact]
        public void EnsureCapacity_InvalidCapacity_ThrowsArgumentOutOfRangeException()
        {
            var builder = MutableTextBufferFactory("Hello", 10);
            AssertExtensions.Throws<ArgumentOutOfRangeException>("capacity", () => builder.EnsureCapacity(-1)); // Capacity < 0
            AssertExtensions.Throws<ArgumentOutOfRangeException>("capacity", () => builder.EnsureCapacity(unchecked(builder.MaxCapacity + 1))); // Capacity > builder.MaxCapacity
        }

        public IEnumerable<object?[]> Equals_TestData()
        {
            var sb1 = MutableTextBufferFactory("Hello");
            var sb2 = MutableTextBufferFactory("Hello");
            var sb3 = MutableTextBufferFactory("HelloX");

            var sb4 = MutableTextBufferFactory(10, 20);
            var sb5 = MutableTextBufferFactory(10, 20);

            var sb6 = MutableTextBufferFactory(10, 20).Apply((sb) => sb.Append("Hello"));
            var sb7 = MutableTextBufferFactory(10, 20).Apply((sb) => sb.Append("Hello"));
            var sb8 = MutableTextBufferFactory(10, 20).Apply((sb) => sb.Append("HelloX"));

            yield return new object?[] { sb1, sb1, true };
            yield return new object?[] { sb1, sb2, true };
            yield return new object?[] { sb1, sb3, false };

            yield return new object?[] { sb4, sb5, true };

            yield return new object?[] { sb6, sb7, true };
            yield return new object?[] { sb6, sb8, false };

            yield return new object?[] { sb1, null, false };

            // J2N TODO: StringBuilder with multiple chunks?
            //StringBuilder chunkSplitBuilder = StringBuilderWithMultipleChunks();
            //yield return new object?[] { chunkSplitBuilder, StringBuilderWithMultipleChunks(), true };
            //yield return new object?[] { sb1, chunkSplitBuilder, false };
            //yield return new object?[] { chunkSplitBuilder, sb1, false };
            //yield return new object?[] { chunkSplitBuilder, StringBuilderWithMultipleChunks().Append("b"), false };

            yield return new object?[] { MutableTextBufferFactory(), MutableTextBufferFactory(), true };
            yield return new object?[] { MutableTextBufferFactory(), MutableTextBufferFactory().Apply((sb) => sb.Clear()), true };
        }

        [Fact]
        public void Test_Equals()
        {
            foreach (var testData in Equals_TestData())
            {
                Run_Test_Equals((MutableTextBuffer)testData[0]!, (MutableTextBuffer?)testData[1], (bool)testData[2]!);
            }
        }

        private static void Run_Test_Equals(MutableTextBuffer sb1, MutableTextBuffer? sb2, bool expected)
        {
            Assert.Equal(expected, sb1.Equals(sb2));
        }

        [Theory]
        [InlineData("Hello", 0, (uint)0, "0Hello")]
        [InlineData("Hello", 3, (uint)123, "Hel123lo")]
        [InlineData("Hello", 5, (uint)456, "Hello456")]
        public void Insert_UInt(string original, int index, uint value, string expected)
        {
            var builder = MutableTextBufferFactory(original);
            builder.Insert(index, value);
            Assert.Equal(expected, builder.ToString());
        }

        [Theory]
        [InlineData("Hello", 0, (uint)1, "J", "1Hello")]
        [InlineData("Hello", 0, (uint)1, "j", "1Hello")]
        [InlineData("Hello", 0, (uint)123, "j1", "123Hello")]
        [InlineData("Hello", 0, (uint)1, "g", "1Hello")]
        [InlineData("Hello", 0, (uint)123, "g1", "1e+02Hello")]
        public void Insert_UInt_Format(string original, int index, uint value, string format, string expected)
        {
            var builder = MutableTextBufferFactory(original);
            builder.Insert(index, value, format, CultureInfo.InvariantCulture);
            Assert.Equal(expected, builder.ToString());
        }

        [Fact]
        public void Insert_UInt_Invalid()
        {
            var builder = MutableTextBufferFactory(0, 5);
            builder.Append("Hello");

            AssertExtensions.Throws<ArgumentOutOfRangeException>("index", () => builder.Insert(-1, (uint)1)); // Index < 0
            AssertExtensions.Throws<ArgumentOutOfRangeException>("index", () => builder.Insert(builder.Length + 1, (uint)1)); // Index > builder.Length
            Assert.Throws<OutOfMemoryException>(() => builder.Insert(builder.Length, (uint)1)); // New length > builder.MaxCapacity
        }

        // J2N: added a BooleanFormat parameter to specify lowercase vs titlecase, so these tests were changed from upstream

        public static IEnumerable<object[]> Insert_Bool_TestData()
        {
            yield return new object[] { "Hello", 0, true, BooleanFormat.TitleCase, "TrueHello" };
            yield return new object[] { "Hello", 0, true, BooleanFormat.Lowercase, "trueHello" };
            yield return new object[] { "Hello", 3, false, BooleanFormat.TitleCase, "HelFalselo" };
            yield return new object[] { "Hello", 3, false, BooleanFormat.Lowercase, "Helfalselo" };
            yield return new object[] { "Hello", 5, false, BooleanFormat.TitleCase, "HelloFalse" };
            yield return new object[] { "Hello", 5, false, BooleanFormat.Lowercase, "Hellofalse" };
        }

        [Fact]
        public void Insert_Bool()
        {
            foreach (var testdata in Insert_Bool_TestData())
            {
                if (((BooleanFormat)testdata[3]) == BooleanFormat.Lowercase)
                    Test_Insert_Bool_Format((string)testdata[0], (int)testdata[1], (bool)testdata[2], null, (string)testdata[4]);
            }
        }

        [Fact]
        public void Insert_Bool_Format()
        {
            foreach (var testdata in Insert_Bool_TestData())
            {
                Test_Insert_Bool_Format((string)testdata[0], (int)testdata[1], (bool)testdata[2], (BooleanFormat)testdata[3], (string)testdata[4]);
            }
        }

        private void Test_Insert_Bool_Format(string original, int index, bool value, BooleanFormat? format, string expected)
        {
            var builder = MutableTextBufferFactory(original);
            if (format is null)
                builder.Insert(index, value);
            else
                builder.Insert(index, value, format.Value);
            Assert.Equal(expected, builder.ToString());
        }

        [Fact]
        public void Insert_Bool_Invalid()
        {
            var builder = MutableTextBufferFactory(0, 5);
            builder.Append("Hello");

            AssertExtensions.Throws<ArgumentOutOfRangeException>("index", () => builder.Insert(-1, true)); // Index < 0
            AssertExtensions.Throws<ArgumentOutOfRangeException>("index", () => builder.Insert(builder.Length + 1, true)); // Index > builder.Length
            Assert.Throws<OutOfMemoryException>(() => builder.Insert(builder.Length, true)); // New length > builder.MaxCapacity
        }

        [Theory]
        [InlineData("Hello", 0, (byte)0, "0Hello")]
        [InlineData("Hello", 3, (byte)123, "Hel123lo")]
        [InlineData("Hello", 5, (byte)123, "Hello123")]
        public void Insert_Byte(string original, int index, byte value, string expected)
        {
            var builder = MutableTextBufferFactory(original);
            builder.Insert(index, value);
            Assert.Equal(expected, builder.ToString());
        }

        [Theory]
        [InlineData("Hello", 0, (byte)1, "J", "1Hello")]
        [InlineData("Hello", 0, (byte)1, "j", "1Hello")]
        [InlineData("Hello", 0, (byte)123, "j1", "123Hello")]
        [InlineData("Hello", 0, (byte)1, "g", "1Hello")]
        [InlineData("Hello", 0, (byte)123, "g1", "1e+02Hello")]
        public void Insert_Byte_Format(string original, int index, byte value, string format, string expected)
        {
            var builder = MutableTextBufferFactory(original);
            builder.Insert(index, value, format, CultureInfo.InvariantCulture);
            Assert.Equal(expected, builder.ToString());
        }

        [Fact]
        public void Insert_Byte_Invalid()
        {
            var builder = MutableTextBufferFactory(0, 5);
            builder.Append("Hello");

            AssertExtensions.Throws<ArgumentOutOfRangeException>("index", () => builder.Insert(-1, (byte)1)); // Index < 0
            AssertExtensions.Throws<ArgumentOutOfRangeException>("index", () => builder.Insert(builder.Length + 1, (byte)1)); // Index > builder.Length
            Assert.Throws<OutOfMemoryException>(() => builder.Insert(builder.Length, (byte)1)); // New length > builder.MaxCapacity
        }

        [Theory]
        [InlineData("Hello", 0, (ulong)0, "0Hello")]
        [InlineData("Hello", 3, (ulong)123, "Hel123lo")]
        [InlineData("Hello", 5, (ulong)456, "Hello456")]
        public void Insert_ULong(string original, int index, ulong value, string expected)
        {
            var builder = MutableTextBufferFactory(original);
            builder.Insert(index, value);
            Assert.Equal(expected, builder.ToString());
        }

        [Theory]
        [InlineData("Hello", 0, (ulong)1, "J", "1Hello")]
        [InlineData("Hello", 0, (ulong)1, "j", "1Hello")]
        [InlineData("Hello", 0, (ulong)123, "j1", "123Hello")]
        [InlineData("Hello", 0, (ulong)1, "g", "1Hello")]
        [InlineData("Hello", 0, (ulong)123, "g1", "1e+02Hello")]
        public void Insert_ULong_Format(string original, int index, ulong value, string format, string expected)
        {
            var builder = MutableTextBufferFactory(original);
            builder.Insert(index, value, format, CultureInfo.InvariantCulture);
            Assert.Equal(expected, builder.ToString());
        }

        [Fact]
        public void Insert_ULong_Invalid()
        {
            var builder = MutableTextBufferFactory(0, 5);
            builder.Append("Hello");

            AssertExtensions.Throws<ArgumentOutOfRangeException>("index", () => builder.Insert(-1, (ulong)1)); // Index < 0
            AssertExtensions.Throws<ArgumentOutOfRangeException>("index", () => builder.Insert(builder.Length + 1, (ulong)1)); // Index > builder.Length
            Assert.Throws<OutOfMemoryException>(() => builder.Insert(builder.Length, (ulong)1)); // New length > builder.MaxCapacity
        }

        [Theory]
        [InlineData("Hello", 0, (ushort)0, "0Hello")]
        [InlineData("Hello", 3, (ushort)123, "Hel123lo")]
        [InlineData("Hello", 5, (ushort)456, "Hello456")]
        public void Insert_UShort(string original, int index, ushort value, string expected)
        {
            var builder = MutableTextBufferFactory(original);
            builder.Insert(index, value);
            Assert.Equal(expected, builder.ToString());
        }

        [Theory]
        [InlineData("Hello", 0, (ushort)1, "J", "1Hello")]
        [InlineData("Hello", 0, (ushort)1, "j", "1Hello")]
        [InlineData("Hello", 0, (ushort)123, "j1", "123Hello")]
        [InlineData("Hello", 0, (ushort)1, "g", "1Hello")]
        [InlineData("Hello", 0, (ushort)123, "g1", "1e+02Hello")]
        public void Insert_UShort_Format(string original, int index, ushort value, string format, string expected)
        {
            var builder = MutableTextBufferFactory(original);
            builder.Insert(index, value, format, CultureInfo.InvariantCulture);
            Assert.Equal(expected, builder.ToString());
        }

        [Fact]
        public void Insert_UShort_Invalid()
        {
            var builder = MutableTextBufferFactory(0, 5);
            builder.Append("Hello");

            AssertExtensions.Throws<ArgumentOutOfRangeException>("index", () => builder.Insert(-1, (ushort)1)); // Index < 0
            AssertExtensions.Throws<ArgumentOutOfRangeException>("index", () => builder.Insert(builder.Length + 1, (ushort)1)); // Index > builder.Length
            Assert.Throws<OutOfMemoryException>(() => builder.Insert(builder.Length, (ushort)1)); // New length > builder.MaxCapacity
        }

        [Theory]
        [InlineData("Hello", 0, '\0', "\0Hello")]
        [InlineData("Hello", 3, 'a', "Helalo")]
        [InlineData("Hello", 5, 'b', "Hellob")]
        public void Insert_Char(string original, int index, char value, string expected)
        {
            var builder = MutableTextBufferFactory(original);
            builder.Insert(index, value);
            Assert.Equal(expected, builder.ToString());
        }

        [Fact]
        public void Insert_Char_Invalid()
        {
            var builder = MutableTextBufferFactory(0, 5);
            builder.Append("Hello");

            AssertExtensions.Throws<ArgumentOutOfRangeException>("index", () => builder.Insert(-1, '\0')); // Index < 0
            AssertExtensions.Throws<ArgumentOutOfRangeException>("index", () => builder.Insert(builder.Length + 1, '\0')); // Index > builder.Length
            AssertExtensions.Throws<ArgumentOutOfRangeException>("requiredLength", () => builder.Insert(builder.Length, '\0')); // New length > builder.MaxCapacity
        }

        [Theory] // J2N specific
        [InlineData("Hello", 0, new char[] { 'a', 'b', 'c' }, 1, "aHello")]
        [InlineData("Hello", 3, new char[] { 'a', 'b', 'c' }, 2, "Helablo")]
        [InlineData("HelloThere", 7, new char[] { 'a', 'b', 'c' }, 3, "HelloThabcere")]
        [InlineData("", 0, new char[] { 'a' }, 1, "a")]
        [InlineData("", 0, new char[] { 'a' }, 0, "")]
        [InlineData("Hello", 2, new char[0], 0, "Hello")]
        [InlineData("Hello", 3, null, 0, "Hello")]
        public unsafe void Insert_CharPointer(string? original, int index, char[]? charArray, int valueCount, string expected)
        {
            _ = charArray; // https://github.com/xunit/xunit/issues/1969
            fixed (char* value = charArray)
            {
                var builder = MutableTextBufferFactory(original);
                builder.Insert(index, value, valueCount);
                Assert.Equal(expected, builder.ToString());
            }
        }

        [Theory]
        [InlineData("ABCDEFGH", 0, 4)]
        [InlineData("ABCDEFGH", 1, 3)]
        [InlineData("ABCDEFGH", 2, 2)]
        [InlineData("ABCDEFGH", 3, 1)]
        [InlineData("ABCDEFGH", 0, 2)]
        public unsafe void Insert_CharPointer_IsSelf_ShouldMatchInsertSelf(string original, int sourceIndex, int length)
        {
            for (int insertIndex = 0; insertIndex <= original.Length; insertIndex++)
            {
                var expected = MutableTextBufferFactory(original);
                expected.InsertFromSelf(insertIndex, sourceIndex, length);

                var actual = MutableTextBufferFactory(original);
                fixed (char* p = &MemoryMarshal.GetReference(actual.RawChars))
                {
                    actual.Insert(insertIndex, p + sourceIndex, length);
                }

                Assert.Equal(expected.ToString(), actual.ToString());
            }
        }

        [Fact] // J2N specific
        public unsafe void Insert_CharPointer_Null_ThrowsNullReferenceException()
        {
            var builder = MutableTextBufferFactory();
            Assert.Throws<NullReferenceException>(() => builder.Insert(0, (char*)null, 2));
        }

        [Fact] // J2N specific
        public unsafe void Insert_CharPointer_NegativeValueCount_ThrowsArgumentOutOfRangeException()
        {
            var builder = MutableTextBufferFactory(0, 5);
            builder.Append("Hello");

            AssertExtensions.Throws<ArgumentOutOfRangeException>("valueCount", () =>
            {
                fixed (char* value = new char[0]) { builder.Insert(3, value, -1); }
            });
        }

        [Fact] // J2N specific
        public unsafe void Insert_CharPointer_NoSpareCapacity_ThrowsArgumentOutOfRangeException()
        {
            var builder = MutableTextBufferFactory(0, 5);
            builder.Append("Hello");

            AssertExtensions.Throws<ArgumentOutOfRangeException>(s_noCapacityParamName, () =>
            {
                fixed (char* value = new char[] { 'a' }) { builder.Insert(1, value, 1); }
            });
        }

        [Fact] // J2N specific
        public unsafe void Insert_CharPointer_NegativeIndex_ThrowsArgumentOutOfRangeException()
        {
            var builder = MutableTextBufferFactory(0, 5);
            builder.Append("Hello");

            AssertExtensions.Throws<ArgumentOutOfRangeException>("index", () =>
            {
                fixed (char* value = new char[] { 'a' }) { builder.Insert(-1, value, 1); }
            });
        }

        public static IEnumerable<object[]> Insert_Float_TestData()
        {
            yield return new object[] { "Hello", 0, (float)0, "0.0Hello" }; // J2N: Use the "j" format, which always has at least 1 digit after the decimal
            yield return new object[] { "Hello", 3, (float)1.23, "Hel1.23lo" };
            yield return new object[] { "Hello", 5, (float)-4.56, "Hello-4.56" };
        }

        [Fact]
        public void Insert_Float()
        {
            using (new ThreadCultureChange(CultureInfo.InvariantCulture))
            {
                foreach (var testdata in Insert_Float_TestData())
                {
                    Test_Insert_Float((string)testdata[0], (int)testdata[1], (float)testdata[2], (string)testdata[3]);
                }
            }
        }

        private void Test_Insert_Float(string original, int index, float value, string expected)
        {
            var builder = MutableTextBufferFactory(original);
            builder.Insert(index, value);
            Assert.Equal(expected, builder.ToString());
        }

        [Theory]
        [InlineData("Hello", 0, 1f, "J", "1.0Hello")]
        [InlineData("Hello", 0, 1f, "j", "1.0Hello")]
        [InlineData("Hello", 0, 123f, "j1", "123.0Hello")]
        [InlineData("Hello", 0, 1f, "g", "1Hello")]
        [InlineData("Hello", 0, 123f, "g1", "1e+02Hello")]
        public void Insert_Float_Format(string original, int index, float value, string format, string expected)
        {
            var builder = MutableTextBufferFactory(original);
            builder.Insert(index, value, format, CultureInfo.InvariantCulture);
            Assert.Equal(expected, builder.ToString());
        }

        [Fact]
        public void Insert_Float_Invalid()
        {
            var builder = MutableTextBufferFactory(0, 5);
            builder.Append("Hello");

            AssertExtensions.Throws<ArgumentOutOfRangeException>("index", () => builder.Insert(-1, (float)1)); // Index < 0
            AssertExtensions.Throws<ArgumentOutOfRangeException>("index", () => builder.Insert(builder.Length + 1, (float)1)); // Index > builder.Length
            Assert.Throws<OutOfMemoryException>(() => builder.Insert(builder.Length, (float)1)); // New length > builder.MaxCapacity
        }

        public static IEnumerable<object?[]> Test_Insert_ICharSequence_TestData()
        {
            yield return new object?[] { "Hello", 0, "\0", 0, 1, "\0Hello" };
            yield return new object?[] { "Hello", 3, "abc", 0, 1, "Helalo" };
            yield return new object?[] { "Hello", 3, "abc", 0, 3, "Helabclo" };
            yield return new object?[] { "Hello", 5, "def", 0, 1, "Hellod" };
            yield return new object?[] { "Hello", 5, "def", 0, 3, "Hellodef" };

            yield return new object?[] { "Hello", 0, "", 0, 0, "Hello" };
            yield return new object?[] { "Hello", 0, null, 0, 0, "Hello" };
            yield return new object?[] { "Hello", 3, "abc", 1, 1, "Helblo" };
            yield return new object?[] { "Hello", 3, "abc", 1, 2, "Helbclo" };
            yield return new object?[] { "Hello", 3, "abc", 0, 2, "Helablo" };
        }

        public static IEnumerable<object?[]> Test_Insert_ICharSequence_Typed_TestData()
        {
            foreach (var testCase in Test_Insert_ICharSequence_TestData())
            {
                yield return new object?[] { testCase[0], testCase[1], new StringCharSequence((string?)testCase[2]), testCase[3], testCase[4], testCase[5] };
                yield return new object?[] { testCase[0], testCase[1], new StringBuilderCharSequence(new StringBuilder((string?)testCase[2])), testCase[3], testCase[4], testCase[5] };
                yield return new object?[] { testCase[0], testCase[1], new CharArrayCharSequence(((string?)testCase[2])?.ToCharArray()), testCase[3], testCase[4], testCase[5] };
                ReadOnlyMemory<char> memory = ((string?)testCase[2]).AsMemory();
                yield return new object?[] { testCase[0], testCase[1], new MockCharSequence(memory), testCase[3], testCase[4], testCase[5] };
                if (testCase[2] is not null)
                {
                    yield return new object?[] { testCase[0], testCase[1], new StringBuffer((string?)testCase[2]), testCase[3], testCase[4], testCase[5] };
                }
                else
                {
                    yield return new object?[] { testCase[0], testCase[1], (StringCharSequence?)null, testCase[3], testCase[4], testCase[5] };
                    yield return new object?[] { testCase[0], testCase[1], (StringBuilderCharSequence?)null, testCase[3], testCase[4], testCase[5] };
                    yield return new object?[] { testCase[0], testCase[1], (CharArrayCharSequence?)null, testCase[3], testCase[4], testCase[5] };
                    yield return new object?[] { testCase[0], testCase[1], (ICharSequence?)null, testCase[3], testCase[4], testCase[5] };
                }
            }
        }

        [Theory]
        [MemberData(nameof(Test_Insert_ICharSequence_Typed_TestData))]
        public void Insert_ICharSequence(string? original, int index, ICharSequence? value, int startIndex, int count, string expected)
        {
            MutableTextBuffer builder;
            if (startIndex == 0 && count == (value?.Length ?? 0))
            {
                // Use Insert(int, ICharSequence)
                builder = MutableTextBufferFactory(original);
                builder.Insert(index, value);
                Assert.Equal(expected, builder.ToString());
            }
            // Use Insert(int, ICharSequence, int, int)
            builder = MutableTextBufferFactory(original);
            builder.Insert(index, value, startIndex, count);
            Assert.Equal(expected, builder.ToString());
        }

        [Fact]
        public void Insert_ICharSequence_Invalid()
        {
            var builder = MutableTextBufferFactory(0, 5);
            builder.Append("Hello");

            AssertExtensions.Throws<ArgumentOutOfRangeException>("index", () => builder.Insert(-1, new char[1].AsCharSequence())); // Index < 0
            AssertExtensions.Throws<ArgumentOutOfRangeException>("index", () => builder.Insert(-1, new char[0].AsCharSequence(), 0, 0)); // Index < 0

            AssertExtensions.Throws<ArgumentOutOfRangeException>("index", () => builder.Insert(builder.Length + 1, new char[1].AsCharSequence())); // Index > builder.Length
            AssertExtensions.Throws<ArgumentOutOfRangeException>("index", () => builder.Insert(builder.Length + 1, new char[0].AsCharSequence(), 0, 0)); // Index > builder.Length

            Assert.Throws<ArgumentNullException>(() => builder.Insert(0, (char[]?)null, 1, 1)); // Value is null (startIndex and count are not zero)
            AssertExtensions.Throws<ArgumentOutOfRangeException>("startIndex", () => builder.Insert(0, new char[0].AsCharSequence(), -1, 0)); // Start index < 0

            AssertExtensions.Throws<ArgumentOutOfRangeException>("startIndex", () => builder.Insert(0, new char[3].AsCharSequence(), 4, 0)); // Start index + char count > value.Length
            AssertExtensions.Throws<ArgumentOutOfRangeException>("startIndex", () => builder.Insert(0, new char[3].AsCharSequence(), 3, 1)); // Start index + char count > value.Length
            AssertExtensions.Throws<ArgumentOutOfRangeException>("startIndex", () => builder.Insert(0, new char[3].AsCharSequence(), 2, 2)); // Start index + char count > value.Length

            AssertExtensions.Throws<ArgumentOutOfRangeException>("requiredLength", () => builder.Insert(builder.Length, new char[1].AsCharSequence())); // New length > builder.MaxCapacity
            AssertExtensions.Throws<ArgumentOutOfRangeException>("requiredLength", () => builder.Insert(builder.Length, new char[] { 'a' }.AsCharSequence(), 0, 1)); // New length > builder.MaxCapacity
        }

        [Fact]
        public void Insert_ICharSequence_InvalidCount()
        {
            var builder = MutableTextBufferFactory(0, 5);
            builder.Append("Hello");
            AssertExtensions.Throws<ArgumentOutOfRangeException>("count", () => builder.Insert(0, new char[0].AsCharSequence(), 0, -1)); // Char count < 0
        }

        public static IEnumerable<object[]> Insert_Overlapping_TestData()
        {
            // source entirely before insertion point
            yield return new object[] { "abcdefghijklmnopqrstuvwxyz", 10, 0, 5, "abcdefghijabcdeklmnopqrstuvwxyz" };
            yield return new object[] { "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz", 10, 0, 5, "abcdefghijabcdeklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz" };

            // source immediately before insertion point
            yield return new object[] { "abcdefghijklmnopqrstuvwxyz", 5, 2, 3, "abcdecdefghijklmnopqrstuvwxyz" };

            // source entirely after insertion point
            yield return new object[] { "abcdefghijklmnopqrstuvwxyz", 5, 10, 5, "abcdeklmnofghijklmnopqrstuvwxyz" };

            // source immediately after insertion point
            yield return new object[] { "abcdefghijklmnopqrstuvwxyz", 5, 5, 3, "abcdefghfghijklmnopqrstuvwxyz" };

            // insert at beginning from middle
            yield return new object[] { "abcdefghijklmnopqrstuvwxyz", 0, 10, 5, "klmnoabcdefghijklmnopqrstuvwxyz" };

            // insert at end from beginning
            yield return new object[] { "abcdefghijklmnopqrstuvwxyz", 26, 0, 5, "abcdefghijklmnopqrstuvwxyzabcde" };

            // source spans insertion point
            yield return new object[] { "abcdefghijklmnopqrstuvwxyz", 10, 8, 6, "abcdefghijijklmnklmnopqrstuvwxyz" };

            // insertion inside source
            yield return new object[] { "abcdefghijklmnopqrstuvwxyz", 8, 5, 10, "abcdefghfghijklmnoijklmnopqrstuvwxyz" };

            // whole buffer
            yield return new object[] { "abcdefghijklmnopqrstuvwxyz", 26, 0, 26, "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz" };

            // empty span
            yield return new object[] { "abcdefghijklmnopqrstuvwxyz", 10, 5, 0, "abcdefghijklmnopqrstuvwxyz" };

            // empty buffer
            yield return new object[] { "", 0, 0, 0, "" };
        }

        [Theory]
        [MemberData(nameof(Insert_Overlapping_TestData))]
        public void Insert_ICharSequence_Overlapping(string value, int index, int sourceIndex, int sourceLength, string expected)
        {
            {
                MutableTextBuffer builder = MutableTextBufferFactory(value);
                ReadOnlyMemory<char> memory = builder.AsMemory(sourceIndex, sourceLength);
                ICharSequence sequence = new SpannableCharSequence(memory);
                builder.Insert(index, sequence);
                Assert.Equal(expected, builder.ToString());
            }

            {
                MutableTextBuffer builder = MutableTextBufferFactory(value);
                ReadOnlyMemory<char> memory = builder.AsMemory(sourceIndex, sourceLength);
                ICharSequence sequence = new CopyableCharSequence(memory);
                builder.Insert(index, sequence);
                Assert.Equal(expected, builder.ToString());
            }

            {
                MutableTextBuffer builder = MutableTextBufferFactory(value);
                ReadOnlyMemory<char> memory = builder.AsMemory(sourceIndex, sourceLength);
                ICharSequence sequence = new SpanCopyableCharSequence(memory);
                builder.Insert(index, sequence);
                Assert.Equal(expected, builder.ToString());
            }

            {
                MutableTextBuffer builder = MutableTextBufferFactory(value);
                ReadOnlyMemory<char> memory = builder.AsMemory(sourceIndex, sourceLength);
                ICharSequence sequence = new SimpleCharSequence(memory);
                builder.Insert(index, sequence);
                Assert.Equal(expected, builder.ToString());
            }
        }

        [Theory]
        [MemberData(nameof(Insert_Overlapping_TestData))]
        public void Insert_ICharSequence_Int32_Int32_Overlapping(string value, int index, int sourceIndex, int sourceLength, string expected)
        {
            {
                MutableTextBuffer builder = MutableTextBufferFactory(value);
                ReadOnlyMemory<char> memory = builder.AsMemory();
                ICharSequence sequence = new SpannableCharSequence(memory);
                builder.Insert(index, sequence, sourceIndex, sourceLength);
                Assert.Equal(expected, builder.ToString());
            }

            {
                MutableTextBuffer builder = MutableTextBufferFactory(value);
                ReadOnlyMemory<char> memory = builder.AsMemory();
                ICharSequence sequence = new CopyableCharSequence(memory);
                builder.Insert(index, sequence, sourceIndex, sourceLength);
                Assert.Equal(expected, builder.ToString());
            }

            {
                MutableTextBuffer builder = MutableTextBufferFactory(value);
                ReadOnlyMemory<char> memory = builder.AsMemory();
                ICharSequence sequence = new SpanCopyableCharSequence(memory);
                builder.Insert(index, sequence, sourceIndex, sourceLength);
                Assert.Equal(expected, builder.ToString());
            }

            {
                MutableTextBuffer builder = MutableTextBufferFactory(value);
                ReadOnlyMemory<char> memory = builder.AsMemory();
                ICharSequence sequence = new SimpleCharSequence(memory);
                builder.Insert(index, sequence, sourceIndex, sourceLength);
                Assert.Equal(expected, builder.ToString());
            }
        }

        [Theory]
        [InlineData("Hello", 0, "\0", "\0Hello")]
        [InlineData("Hello", 3, "abc", "Helabclo")]
        [InlineData("Hello", 5, "def", "Hellodef")]
        [InlineData("Hello", 0, "", "Hello")]
        [InlineData("Hello", 0, null, "Hello")]
        public void Insert_Object(string? original, int index, object? value, string expected)
        {
            var builder = MutableTextBufferFactory(original);
            builder.Insert(index, value);
            Assert.Equal(expected, builder.ToString());
        }

        [Fact]
        public void Insert_Object_Invalid()
        {
            var builder = MutableTextBufferFactory(0, 5);
            builder.Append("Hello");

            AssertExtensions.Throws<ArgumentOutOfRangeException>("index", () => builder.Insert(-1, new object())); // Index < 0
            AssertExtensions.Throws<ArgumentOutOfRangeException>("index", () => builder.Insert(builder.Length + 1, new object())); // Index > builder.Length
            Assert.Throws<OutOfMemoryException>(() => builder.Insert(builder.Length, new object())); // New length > builder.MaxCapacity
        }

        [Theory]
        [InlineData("Hello", 0, (long)0, "0Hello")]
        [InlineData("Hello", 3, (long)123, "Hel123lo")]
        [InlineData("Hello", 5, (long)-456, "Hello-456")]
        public void Insert_Long(string original, int index, long value, string expected)
        {
            var builder = MutableTextBufferFactory(original);
            builder.Insert(index, value);
            Assert.Equal(expected, builder.ToString());
        }

        [Theory]
        [InlineData("Hello", 0, 1L, "J", "1Hello")]
        [InlineData("Hello", 0, 1L, "j", "1Hello")]
        [InlineData("Hello", 0, 123L, "j1", "123Hello")]
        [InlineData("Hello", 0, 1L, "g", "1Hello")]
        [InlineData("Hello", 0, 123L, "g1", "1e+02Hello")]
        public void Insert_Long_Format(string original, int index, long value, string format, string expected)
        {
            var builder = MutableTextBufferFactory(original);
            builder.Insert(index, value, format, CultureInfo.InvariantCulture);
            Assert.Equal(expected, builder.ToString());
        }

        [Fact]
        public void Insert_Long_Invalid()
        {
            var builder = MutableTextBufferFactory(0, 5);
            builder.Append("Hello");

            AssertExtensions.Throws<ArgumentOutOfRangeException>("index", () => builder.Insert(-1, (long)1)); // Index < 0
            AssertExtensions.Throws<ArgumentOutOfRangeException>("index", () => builder.Insert(builder.Length + 1, (long)1)); // Index > builder.Length
            Assert.Throws<OutOfMemoryException>(() => builder.Insert(builder.Length, (long)1)); // New length > builder.MaxCapacity
        }

        [Theory]
        [InlineData("Hello", 0, 0, "0Hello")]
        [InlineData("Hello", 3, 123, "Hel123lo")]
        [InlineData("Hello", 5, -456, "Hello-456")]
        public void Insert_Int(string original, int index, int value, string expected)
        {
            var builder = MutableTextBufferFactory(original);
            builder.Insert(index, value);
            Assert.Equal(expected, builder.ToString());
        }

        [Theory]
        [InlineData("Hello", 0, 1, "J", "1Hello")]
        [InlineData("Hello", 0, 1, "j", "1Hello")]
        [InlineData("Hello", 0, 123, "j1", "123Hello")]
        [InlineData("Hello", 0, 1, "g", "1Hello")]
        [InlineData("Hello", 0, 123, "g1", "1e+02Hello")]
        public void Insert_Int_Format(string original, int index, int value, string format, string expected)
        {
            var builder = MutableTextBufferFactory(original);
            builder.Insert(index, value, format, CultureInfo.InvariantCulture);
            Assert.Equal(expected, builder.ToString());
        }

        [Fact]
        public void Insert_Int_Invalid()
        {
            var builder = MutableTextBufferFactory(0, 5);
            builder.Append("Hello");

            AssertExtensions.Throws<ArgumentOutOfRangeException>("index", () => builder.Insert(-1, 1)); // Index < 0
            AssertExtensions.Throws<ArgumentOutOfRangeException>("index", () => builder.Insert(builder.Length + 1, 1)); // Index > builder.Length
            Assert.Throws<OutOfMemoryException>(() => builder.Insert(builder.Length, 1)); // New length > builder.MaxCapacity
        }

        [Fact]
        public void Insert_Int_UsesAmbientCulture_WhenInvariantDefaultsDisabled()
        {
            using var ambientCulture = new ThreadCultureChange("ar-IQ");
            var sb = MutableTextBufferFactory("foo", new MutableTextBufferTestOptions { UseInvariantDefaults = false });
            sb.Insert(0, -1);
            Assert.Equal("\u061C\u002D1foo", sb.ToString());
        }

        [Fact]
        public void Insert_Int_UsesInvariantCulture_WhenInvariantDefaultsEnabled()
        {
            using var ambientCulture = new ThreadCultureChange("ar-IQ");
            var sb = MutableTextBufferFactory("foo", new MutableTextBufferTestOptions { UseInvariantDefaults = true });
            sb.Insert(0, -1);
            Assert.Equal("\u002D1foo", sb.ToString());
        }

        [Fact]
        public void Insert_Int_ExplicitProviderOverridesInvariantDefaults()
        {
            using var ambientCulture = new ThreadCultureChange("en-US");
            var sb = MutableTextBufferFactory("foo", new MutableTextBufferTestOptions { UseInvariantDefaults = true });
            sb.Insert(0, -1, provider: new CultureInfo("ar-IQ"));
            Assert.Equal("\u061C\u002D1foo", sb.ToString());
        }

        [Fact]
        public void Insert_Int_ExplicitProviderOverridesAmbientCulture()
        {
            using var ambientCulture = new ThreadCultureChange("en-US");
            var sb = MutableTextBufferFactory("foo", new MutableTextBufferTestOptions { UseInvariantDefaults = false });
            sb.Insert(0, -1, provider: new CultureInfo("ar-IQ"));
            Assert.Equal("\u061C\u002D1foo", sb.ToString());
        }

        [Theory]
        [InlineData("Hello", 0, (short)0, "0Hello")]
        [InlineData("Hello", 3, (short)123, "Hel123lo")]
        [InlineData("Hello", 5, (short)-456, "Hello-456")]
        public void Insert_Short(string original, int index, short value, string expected)
        {
            var builder = MutableTextBufferFactory(original);
            builder.Insert(index, value);
            Assert.Equal(expected, builder.ToString());
        }

        [Theory]
        [InlineData("Hello", 0, (short)1, "J", "1Hello")]
        [InlineData("Hello", 0, (short)1, "j", "1Hello")]
        [InlineData("Hello", 0, (short)123, "j1", "123Hello")]
        [InlineData("Hello", 0, (short)1, "g", "1Hello")]
        [InlineData("Hello", 0, (short)123, "g1", "1e+02Hello")]
        public void Insert_Short_Format(string original, int index, short value, string format, string expected)
        {
            var builder = MutableTextBufferFactory(original);
            builder.Insert(index, value, format, CultureInfo.InvariantCulture);
            Assert.Equal(expected, builder.ToString());
        }

        [Fact]
        public void Insert_Short_Invalid()
        {
            var builder = MutableTextBufferFactory(0, 5);
            builder.Append("Hello");

            AssertExtensions.Throws<ArgumentOutOfRangeException>("index", () => builder.Insert(-1, (short)1)); // Index < 0
            AssertExtensions.Throws<ArgumentOutOfRangeException>("index", () => builder.Insert(builder.Length + 1, (short)1)); // Index > builder.Length
            Assert.Throws<OutOfMemoryException>(() => builder.Insert(builder.Length, (short)1)); // New length > builder.MaxCapacity
        }

        public static IEnumerable<object[]> Insert_Double_TestData()
        {
            yield return new object[] { "Hello", 0, (double)0, "0.0Hello" }; // J2N: Use the "j" format, which always has at least 1 digit after the decimal
            yield return new object[] { "Hello", 3, 1.23, "Hel1.23lo" };
            yield return new object[] { "Hello", 5, -4.56, "Hello-4.56" };
        }

        [Fact]
        public void Insert_Double()
        {
            using (new ThreadCultureChange(CultureInfo.InvariantCulture))
            {
                foreach (var testdata in Insert_Double_TestData())
                {
                    Test_Insert_Double((string)testdata[0], (int)testdata[1], (double)testdata[2], (string)testdata[3]);
                }
            }
        }

        private void Test_Insert_Double(string original, int index, double value, string expected)
        {
            var builder = MutableTextBufferFactory(original);
            builder.Insert(index, value);
            Assert.Equal(expected, builder.ToString());
        }

        [Theory]
        [InlineData("Hello", 0, 1d, "J", "1.0Hello")]
        [InlineData("Hello", 0, 1d, "j", "1.0Hello")]
        [InlineData("Hello", 0, 123d, "j1", "123.0Hello")]
        [InlineData("Hello", 0, 1d, "g", "1Hello")]
        [InlineData("Hello", 0, 123d, "g1", "1e+02Hello")]
        public void Insert_Double_Format(string original, int index, double value, string format, string expected)
        {
            var builder = MutableTextBufferFactory(original);
            builder.Insert(index, value, format, CultureInfo.InvariantCulture);
            Assert.Equal(expected, builder.ToString());
        }

        [Fact]
        public void Insert_Double_Invalid()
        {
            var builder = MutableTextBufferFactory(0, 5);
            builder.Append("Hello");

            AssertExtensions.Throws<ArgumentOutOfRangeException>("index", () => builder.Insert(-1, (double)1)); // Index < 0
            AssertExtensions.Throws<ArgumentOutOfRangeException>("index", () => builder.Insert(builder.Length + 1, (double)1)); // Index > builder.Length
            Assert.Throws<OutOfMemoryException>(() => builder.Insert(builder.Length, (double)1)); // New length > builder.MaxCapacity
        }

        [Fact]
        public void Insert_Double_UsesAmbientCulture_WhenInvariantDefaultsDisabled()
        {
            using var ambientCulture = new ThreadCultureChange("fr-FR");
            var sb = MutableTextBufferFactory("foo", new MutableTextBufferTestOptions { UseInvariantDefaults = false });
            sb.Insert(0, 1.5d);
            Assert.Equal("1,5foo", sb.ToString());
        }

        [Fact]
        public void Insert_Double_UsesInvariantCulture_WhenInvariantDefaultsEnabled()
        {
            using var ambientCulture = new ThreadCultureChange("fr-FR");
            var sb = MutableTextBufferFactory("foo", new MutableTextBufferTestOptions { UseInvariantDefaults = true });
            sb.Insert(0, 1.5d);
            Assert.Equal("1.5foo", sb.ToString());
        }

        [Fact]
        public void Insert_Double_ExplicitProviderOverridesInvariantDefaults()
        {
            using var ambientCulture = new ThreadCultureChange("en-US");
            var sb = MutableTextBufferFactory("foo", new MutableTextBufferTestOptions { UseInvariantDefaults = true });
            sb.Insert(0, 1.5f, provider: new CultureInfo("fr-FR"));
            Assert.Equal("1,5foo", sb.ToString());
        }

        [Fact]
        public void Insert_Double_ExplicitProviderOverridesAmbientCulture()
        {
            using var ambientCulture = new ThreadCultureChange("en-US");
            var sb = MutableTextBufferFactory("foo", new MutableTextBufferTestOptions { UseInvariantDefaults = false });
            sb.Insert(0, 1.5f, provider: new CultureInfo("fr-FR"));
            Assert.Equal("1,5foo", sb.ToString());
        }

        public static IEnumerable<object[]> Test_Insert_Decimal_TestData()
        {
            yield return new object[] { "Hello", 0, 0m, "0Hello" };
            yield return new object[] { "Hello", 3, 1.23m, "Hel1.23lo" };
            yield return new object[] { "Hello", 5, -4.56m, "Hello-4.56" };
            yield return new object[] { "", 0, 0.000001m, "0.000001" };
            yield return new object[] { "", 0, 0.0000001m, "1E-7" };
            yield return new object[] { "", 0, 1.2300m, "1.2300" };
        }

        public static IEnumerable<object[]> Test_Insert_Decimal_Format_TestData()
        {
            yield return new object[] { "Hello", 0, 1m, "J", "1Hello" };
            yield return new object[] { "Hello", 0, 1m, "j", "1Hello" };
            yield return new object[] { "Hello", 0, 123m, "j1", "123Hello" };
            yield return new object[] { "Hello", 0, 1m, "g", "1Hello" };
            yield return new object[] { "Hello", 0, 123m, "g1", "1e+02Hello" };
        }

        [Fact]
        public void Insert_Decimal()
        {
            using (new ThreadCultureChange(CultureInfo.InvariantCulture))
            {
                foreach (var testdata in Test_Insert_Decimal_TestData())
                {
                    Test_Insert_Decimal((string)testdata[0], (int)testdata[1], (decimal)testdata[2], (string)testdata[3]);
                }
            }
        }

        private void Test_Insert_Decimal(string original, int index, decimal value, string expected)
        {
            var builder = MutableTextBufferFactory(original);
            builder.Insert(index, value);
            Assert.Equal(expected, builder.ToString());
        }

        [Fact]
        public void Insert_Decimal_Format()
        {
            foreach (var testdata in Test_Insert_Decimal_Format_TestData())
            {
                Test_Insert_Decimal_Format((string)testdata[0], (int)testdata[1], (decimal)testdata[2], (string)testdata[3], (string)testdata[4]);
            }
        }

        public void Test_Insert_Decimal_Format(string original, int index, decimal value, string format, string expected)
        {
            var builder = MutableTextBufferFactory(original);
            builder.Insert(index, value, format, CultureInfo.InvariantCulture);
            Assert.Equal(expected, builder.ToString());
        }

        [Fact]
        public void Insert_Decimal_Invalid()
        {
            var builder = MutableTextBufferFactory(0, 5);
            builder.Append("Hello");

            AssertExtensions.Throws<ArgumentOutOfRangeException>("index", () => builder.Insert(-1, (decimal)1)); // Index < 0
            AssertExtensions.Throws<ArgumentOutOfRangeException>("index", () => builder.Insert(builder.Length + 1, (decimal)1)); // Index > builder.Length
            Assert.Throws<OutOfMemoryException>(() => builder.Insert(builder.Length, (decimal)1)); // New length > builder.MaxCapacity
        }

        [Theory]
        [InlineData("Hello", 0, (sbyte)0, "0Hello")]
        [InlineData("Hello", 3, (sbyte)123, "Hel123lo")]
        [InlineData("Hello", 5, (sbyte)-123, "Hello-123")]
        public void Insert_SByte(string original, int index, sbyte value, string expected)
        {
            var builder = MutableTextBufferFactory(original);
            builder.Insert(index, value);
            Assert.Equal(expected, builder.ToString());
        }

        [Theory]
        [InlineData("Hello", 0, (sbyte)1, "J", "1Hello")]
        [InlineData("Hello", 0, (sbyte)1, "j", "1Hello")]
        [InlineData("Hello", 0, (sbyte)123, "j1", "123Hello")]
        [InlineData("Hello", 0, (sbyte)1, "g", "1Hello")]
        [InlineData("Hello", 0, (sbyte)123, "g1", "1e+02Hello")]
        public void Insert_SByte_Format(string original, int index, sbyte value, string format, string expected)
        {
            var builder = MutableTextBufferFactory(original);
            builder.Insert(index, value, format, CultureInfo.InvariantCulture);
            Assert.Equal(expected, builder.ToString());
        }

        [Fact]
        public void Insert_SByte_Invalid()
        {
            var builder = MutableTextBufferFactory(0, 5);
            builder.Append("Hello");

            AssertExtensions.Throws<ArgumentOutOfRangeException>("index", () => builder.Insert(-1, (sbyte)1)); // Index < 0
            AssertExtensions.Throws<ArgumentOutOfRangeException>("index", () => builder.Insert(builder.Length + 1, (sbyte)1)); // Index > builder.Length
            Assert.Throws<OutOfMemoryException>(() => builder.Insert(builder.Length, (sbyte)1)); // New length > builder.MaxCapacity
        }

        [Theory]
        [InlineData("Hello", 0, "\0", 0, "Hello")]
        [InlineData("Hello", 0, "\0", 1, "\0Hello")]
        [InlineData("Hello", 3, "abc", 1, "Helabclo")]
        [InlineData("Hello", 5, "def", 1, "Hellodef")]
        [InlineData("Hello", 0, "", 1, "Hello")]
        [InlineData("Hello", 0, null, 1, "Hello")]
        [InlineData("Hello", 3, "abc", 2, "Helabcabclo")]
        [InlineData("Hello", 5, "def", 2, "Hellodefdef")]

        // J2N specific - added tests to stress copy logic
        [InlineData("", 0, "a", 8, "aaaaaaaa")]
        [InlineData("", 0, "ab", 4, "abababab")]
        [InlineData("", 0, "abc", 4, "abcabcabcabc")]
        [InlineData("", 0, "abcd", 3, "abcdabcdabcd")]
        [InlineData("", 0, "abc", 5, "abcabcabcabcabc")]
        [InlineData("Hello", 0, "abc", 5, "abcabcabcabcabcHello")]
        [InlineData("Hello", 2, "abc", 5, "Heabcabcabcabcabcllo")]
        [InlineData("Hello", 5, "abc", 5, "Helloabcabcabcabcabc")]
        [InlineData("", 0, "abcde", 7, "abcdeabcdeabcdeabcdeabcdeabcdeabcde")]
        public void Insert_String_RepeatCount(string? original, int index, string? value, int count, string expected)
        {
            MutableTextBuffer builder;
            if (count == 1)
            {
                // Use Insert(int, string)
                builder = MutableTextBufferFactory(original);
                builder.Insert(index, value);
                Assert.Equal(expected, builder.ToString());
            }
            // Use Insert(int, string, int)
            builder = MutableTextBufferFactory(original);
            builder.Insert(index, value, count);
            Assert.Equal(expected, builder.ToString());
        }

        [Fact]
        public void Insert_String_RepeatCount_Invalid()
        {
            var builder = MutableTextBufferFactory(0, 6);
            builder.Append("Hello");

            AssertExtensions.Throws<ArgumentOutOfRangeException>("index", () => builder.Insert(-1, "")); // Index < 0
            AssertExtensions.Throws<ArgumentOutOfRangeException>("index", () => builder.Insert(-1, "", 0)); // Index < 0

            AssertExtensions.Throws<ArgumentOutOfRangeException>("index", () => builder.Insert(builder.Length + 1, "")); // Index > builder.Length
            AssertExtensions.Throws<ArgumentOutOfRangeException>("index", () => builder.Insert(builder.Length + 1, "", 0)); // Index > builder.Length

            AssertExtensions.Throws<ArgumentOutOfRangeException>("repeatCount", () => builder.Insert(0, "", -1)); // Count < 0

            AssertExtensions.Throws<ArgumentOutOfRangeException>("requiredLength", () => builder.Insert(builder.Length, "aa")); // New length > builder.MaxCapacity
            Assert.Throws<OutOfMemoryException>(() => builder.Insert(builder.Length, "aa", 1)); // New length > builder.MaxCapacity
            Assert.Throws<OutOfMemoryException>(() => builder.Insert(builder.Length, "a", 2)); // New length > builder.MaxCapacity
        }


        [Theory] // J2N specific
        [InlineData("Hello", 0, "\0", 0, "Hello")]
        [InlineData("Hello", 0, "\0", 1, "\0Hello")]
        [InlineData("Hello", 3, "abc", 1, "Helabclo")]
        [InlineData("Hello", 5, "def", 1, "Hellodef")]
        [InlineData("Hello", 0, "", 1, "Hello")]
        [InlineData("Hello", 0, null, 1, "Hello")]
        [InlineData("Hello", 3, "abc", 2, "Helabcabclo")]
        [InlineData("Hello", 5, "def", 2, "Hellodefdef")]
        public void Insert_CharSpan_RepeatCount(string? original, int index, string? value, int count, string expected)
        {
            MutableTextBuffer builder;
            if (count == 1)
            {
                // Use Insert(int, ReadOnlySpan<char>)
                builder = MutableTextBufferFactory(original);
                builder.Insert(index, value.AsSpan());
                Assert.Equal(expected, builder.ToString());
            }
            // Use Insert(int, ReadOnlySpan<char>, int)
            builder = MutableTextBufferFactory(original);
            builder.Insert(index, value.AsSpan(), count);
            Assert.Equal(expected, builder.ToString());
        }

        [Theory]
        [MemberData(nameof(Insert_CharSpan_RepeatCount_Overlapping_TestData))]
        public void Insert_CharSpan_RepeatCount_Overlapping(string value, int index, int sourceIndex, int sourceLength, int repeatCount, string expected)
        {
            MutableTextBuffer builder = MutableTextBufferFactory(value);
            ReadOnlySpan<char> source = builder.AsSpan(sourceIndex, sourceLength);
            builder.Insert(index, source, repeatCount);
            Assert.Equal(expected, builder.ToString());
        }

        [Theory] // J2N specific
        [InlineData(0, 4, 2)]
        [InlineData(1, 3, 2)]
        [InlineData(2, 2, 2)]
        [InlineData(3, 1, 5)]
        [InlineData(0, 2, 3)]
        public void Insert_CharSpan_RepeatCount_IsSelf_GrowingBuffer(int sourceIndex, int length, int repeatCount)
        {
            const string original = "ABCDEFGH";

            ReadOnlySpan<char> source =
                original.AsSpan(sourceIndex, length);

            for (int insertIndex = 0; insertIndex <= original.Length; insertIndex++)
            {
                // Force MakeRoom() to grow.
                var expected = MutableTextBufferFactory(original, capacity: original.Length);

                for (int i = 0; i < repeatCount; i++)
                {
                    expected.Insert(insertIndex + i * length, source);
                }

                var actual = MutableTextBufferFactory(original, capacity: original.Length);

                actual.Insert(
                    insertIndex,
                    actual.AsSpan(sourceIndex, length),
                    repeatCount);

                Assert.Equal(expected.ToString(), actual.ToString());
            }
        }

        [Fact] // J2N specific
        public void Insert_CharSpan_RepeatCount_Invalid()
        {
            var builder = MutableTextBufferFactory(0, 6);
            builder.Append("Hello");

            AssertExtensions.Throws<ArgumentOutOfRangeException>("index", () => builder.Insert(-1, "".AsSpan())); // Index < 0
            AssertExtensions.Throws<ArgumentOutOfRangeException>("index", () => builder.Insert(-1, "".AsSpan(), 0)); // Index < 0

            AssertExtensions.Throws<ArgumentOutOfRangeException>("index", () => builder.Insert(builder.Length + 1, "".AsSpan())); // Index > builder.Length
            AssertExtensions.Throws<ArgumentOutOfRangeException>("index", () => builder.Insert(builder.Length + 1, "".AsSpan(), 0)); // Index > builder.Length

            AssertExtensions.Throws<ArgumentOutOfRangeException>("repeatCount", () => builder.Insert(0, "".AsSpan(), -1)); // Count < 0

            AssertExtensions.Throws<ArgumentOutOfRangeException>("requiredLength", () => builder.Insert(builder.Length, "aa".AsSpan())); // New length > builder.MaxCapacity
            Assert.Throws<OutOfMemoryException>(() => builder.Insert(builder.Length, "aa".AsSpan(), 1)); // New length > builder.MaxCapacity
            Assert.Throws<OutOfMemoryException>(() => builder.Insert(builder.Length, "a".AsSpan(), 2)); // New length > builder.MaxCapacity
        }

        public static IEnumerable<object?[]> Test_Insert_ICharSequence_RepeatCount_TestData()
        {
            yield return new object?[] { "Hello", 0, "\0", 0, "Hello" };
            yield return new object?[] { "Hello", 0, "\0", 1, "\0Hello" };
            yield return new object?[] { "Hello", 3, "abc", 1, "Helabclo" };
            yield return new object?[] { "Hello", 5, "def", 1, "Hellodef" };
            yield return new object?[] { "Hello", 0, "", 1, "Hello" };

            yield return new object?[] { "Hello", 0, null, 1, "Hello" };
            yield return new object?[] { "Hello", 3, "abc", 2, "Helabcabclo" };
            yield return new object?[] { "Hello", 5, "def", 2, "Hellodefdef" };

            // J2N specific - added tests to stress copy logic
            yield return new object?[] { "", 0, "a", 8, "aaaaaaaa" };
            yield return new object?[] { "", 0, "ab", 4, "abababab" };

            yield return new object?[] { "", 0, "abc", 4, "abcabcabcabc" };
            yield return new object?[] { "", 0, "abcd", 3, "abcdabcdabcd" };
            yield return new object?[] { "", 0, "abc", 5, "abcabcabcabcabc" };
            yield return new object?[] { "Hello", 0, "abc", 5, "abcabcabcabcabcHello" };
            yield return new object?[] { "Hello", 2, "abc", 5, "Heabcabcabcabcabcllo" };
            yield return new object?[] { "Hello", 5, "abc", 5, "Helloabcabcabcabcabc" };
            yield return new object?[] { "", 0, "abcde", 7, "abcdeabcdeabcdeabcdeabcdeabcdeabcde" };
        }

        public static IEnumerable<object?[]> Test_Insert_ICharSequence_Typed_RepeatCount_TestData()
        {
            foreach (var testCase in Test_Insert_ICharSequence_RepeatCount_TestData())
            {
                yield return new object?[] { testCase[0], testCase[1], new StringCharSequence((string?)testCase[2]), testCase[3], testCase[4] };
                yield return new object?[] { testCase[0], testCase[1], new StringBuilderCharSequence(new StringBuilder((string?)testCase[2])), testCase[3], testCase[4] };
                yield return new object?[] { testCase[0], testCase[1], new CharArrayCharSequence(((string?)testCase[2])?.ToCharArray()), testCase[3], testCase[4] };
                ReadOnlyMemory<char> memory = ((string?)testCase[2]).AsMemory();
                yield return new object?[] { testCase[0], testCase[1], new MockCharSequence(memory), testCase[3], testCase[4] };
                if (testCase[2] is not null)
                {
                    yield return new object?[] { testCase[0], testCase[1], new StringBuffer((string?)testCase[2]), testCase[3], testCase[4] };
                }
                else
                {
                    yield return new object?[] { testCase[0], testCase[1], (StringCharSequence?)null, testCase[3], testCase[4] };
                    yield return new object?[] { testCase[0], testCase[1], (StringBuilderCharSequence?)null, testCase[3], testCase[4] };
                    yield return new object?[] { testCase[0], testCase[1], (CharArrayCharSequence?)null, testCase[3], testCase[4] };
                    yield return new object?[] { testCase[0], testCase[1], (ICharSequence?)null, testCase[3], testCase[4] };
                }
            }
        }


        [Theory] // J2N specific
        [MemberData(nameof(Test_Insert_ICharSequence_Typed_RepeatCount_TestData))]
        public void Insert_ICharSequence_RepeatCount(string? original, int index, ICharSequence? value, int count, string expected)
        {
            MutableTextBuffer builder;
            if (count == 1)
            {
                // Use Insert(int, ICharSequence)
                builder = MutableTextBufferFactory(original);
                builder.Insert(index, value);
                Assert.Equal(expected, builder.ToString());
            }
            // Use Insert(int, ICharSequence, int)
            builder = MutableTextBufferFactory(original);
            builder.Insert(index, value, count);
            Assert.Equal(expected, builder.ToString());
        }

        [Fact] // J2N specific
        public void Insert_ICharSequence_RepeatCount_Invalid()
        {
            var builder = MutableTextBufferFactory(0, 6);
            builder.Append("Hello");

            AssertExtensions.Throws<ArgumentOutOfRangeException>("index", () => builder.Insert(-1, "".AsCharSequence())); // Index < 0
            AssertExtensions.Throws<ArgumentOutOfRangeException>("index", () => builder.Insert(-1, "".AsCharSequence(), 0)); // Index < 0

            AssertExtensions.Throws<ArgumentOutOfRangeException>("index", () => builder.Insert(builder.Length + 1, "".AsCharSequence())); // Index > builder.Length
            AssertExtensions.Throws<ArgumentOutOfRangeException>("index", () => builder.Insert(builder.Length + 1, "".AsCharSequence(), 0)); // Index > builder.Length

            AssertExtensions.Throws<ArgumentOutOfRangeException>("repeatCount", () => builder.Insert(0, "".AsCharSequence(), -1)); // Count < 0

            AssertExtensions.Throws<ArgumentOutOfRangeException>("requiredLength", () => builder.Insert(builder.Length, "aa".AsCharSequence())); // New length > builder.MaxCapacity
            Assert.Throws<OutOfMemoryException>(() => builder.Insert(builder.Length, "aa".AsCharSequence(), 1)); // New length > builder.MaxCapacity
            Assert.Throws<OutOfMemoryException>(() => builder.Insert(builder.Length, "a".AsCharSequence(), 2)); // New length > builder.MaxCapacity
        }

        public static IEnumerable<object[]> Insert_CharSpan_RepeatCount_Overlapping_TestData()
        {
            // source entirely before insertion point
            yield return new object[] { "abcdefghijklmnopqrstuvwxyz", 10, 0, 5, 2, "abcdefghijabcdeabcdeklmnopqrstuvwxyz" };

            // source immediately before insertion point
            yield return new object[] { "abcdefghijklmnopqrstuvwxyz", 5, 2, 3, 2, "abcdecdecdefghijklmnopqrstuvwxyz" };

            // source entirely after insertion point
            yield return new object[] { "abcdefghijklmnopqrstuvwxyz", 5, 10, 5, 2, "abcdeklmnoklmnofghijklmnopqrstuvwxyz" };

            // source immediately after insertion point
            yield return new object[] { "abcdefghijklmnopqrstuvwxyz", 5, 5, 3, 2, "abcdefghfghfghijklmnopqrstuvwxyz" };

            // insert at beginning
            yield return new object[] { "abcdefghijklmnopqrstuvwxyz", 0, 10, 5, 2, "klmnoklmnoabcdefghijklmnopqrstuvwxyz" };

            // insert at end
            yield return new object[] { "abcdefghijklmnopqrstuvwxyz", 26, 0, 5, 2, "abcdefghijklmnopqrstuvwxyzabcdeabcde" };

            // source spans insertion point
            yield return new object[] { "abcdefghijklmnopqrstuvwxyz", 10, 8, 6, 2, "abcdefghijijklmnijklmnklmnopqrstuvwxyz" };

            // insertion point inside source
            yield return new object[] { "abcdefghijklmnopqrstuvwxyz", 8, 5, 10, 2, "abcdefghfghijklmnofghijklmnoijklmnopqrstuvwxyz" };

            // whole buffer
            yield return new object[] { "abcdefghijklmnopqrstuvwxyz", 26, 0, 26, 2, "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz" };

            // empty span
            yield return new object[] { "abcdefghijklmnopqrstuvwxyz", 10, 5, 0, 5, "abcdefghijklmnopqrstuvwxyz" };

            // repeat count zero
            yield return new object[]
            { "abcdefghijklmnopqrstuvwxyz", 10, 5, 5, 0, "abcdefghijklmnopqrstuvwxyz" };

            // empty builder
            yield return new object[] { "", 0, 0, 0, 5, "" };
        }

        [Theory]
        [MemberData(nameof(Insert_CharSpan_RepeatCount_Overlapping_TestData))]
        public void Insert_ICharSequence_RepeatCount_Overlapping(string value, int index, int sourceIndex, int sourceLength, int repeatCount, string expected)
        {
            {
                MutableTextBuffer builder = MutableTextBufferFactory(value);
                ReadOnlyMemory<char> memory = builder.AsMemory(sourceIndex, sourceLength);
                ICharSequence sequence = new SpannableCharSequence(memory);
                builder.Insert(index, sequence, repeatCount);
                Assert.Equal(expected, builder.ToString());
            }

            {
                MutableTextBuffer builder = MutableTextBufferFactory(value);
                ReadOnlyMemory<char> memory = builder.AsMemory(sourceIndex, sourceLength);
                ICharSequence sequence = new CopyableCharSequence(memory);
                builder.Insert(index, sequence, repeatCount);
                Assert.Equal(expected, builder.ToString());
            }

            {
                MutableTextBuffer builder = MutableTextBufferFactory(value);
                ReadOnlyMemory<char> memory = builder.AsMemory(sourceIndex, sourceLength);
                ICharSequence sequence = new SpanCopyableCharSequence(memory);
                builder.Insert(index, sequence, repeatCount);
                Assert.Equal(expected, builder.ToString());
            }

            {
                MutableTextBuffer builder = MutableTextBufferFactory(value);
                ReadOnlyMemory<char> memory = builder.AsMemory(sourceIndex, sourceLength);
                ICharSequence sequence = new SimpleCharSequence(memory);
                builder.Insert(index, sequence, repeatCount);
                Assert.Equal(expected, builder.ToString());
            }
        }

        public static IEnumerable<object?[]> Test_Insert_StringBuilder_RepeatCount_TestData()
        {
            foreach (var testCase in Test_Insert_ICharSequence_RepeatCount_TestData())
            {
                if (testCase[2] is not null)
                {
                    yield return new object?[] { testCase[0], testCase[1], new StringBuilder((string?)testCase[2]), testCase[3], testCase[4] };
                }
                else
                {
                    yield return new object?[] { testCase[0], testCase[1], (StringBuilder?)null, testCase[3], testCase[4] };
                }
            }
        }


        [Theory] // J2N specific
        [MemberData(nameof(Test_Insert_StringBuilder_RepeatCount_TestData))]
        public void Insert_StringBuilder_RepeatCount(string? original, int index, StringBuilder? value, int count, string expected)
        {
            MutableTextBuffer builder;
            if (count == 1)
            {
                // Use Insert(int, ICharSequence)
                builder = MutableTextBufferFactory(original);
                builder.Insert(index, value);
                Assert.Equal(expected, builder.ToString());
            }
            // Use Insert(int, ICharSequence, int)
            builder = MutableTextBufferFactory(original);
            builder.Insert(index, value, count);
            Assert.Equal(expected, builder.ToString());
        }

        [Fact] // J2N specific
        public void Insert_StringBuilder_RepeatCount_Invalid()
        {
            var builder = MutableTextBufferFactory(0, 6);
            builder.Append("Hello");

            AssertExtensions.Throws<ArgumentOutOfRangeException>("index", () => builder.Insert(-1, new StringBuilder(""))); // Index < 0
            AssertExtensions.Throws<ArgumentOutOfRangeException>("index", () => builder.Insert(-1, new StringBuilder(""), 0)); // Index < 0

            AssertExtensions.Throws<ArgumentOutOfRangeException>("index", () => builder.Insert(builder.Length + 1, new StringBuilder(""))); // Index > builder.Length
            AssertExtensions.Throws<ArgumentOutOfRangeException>("index", () => builder.Insert(builder.Length + 1, new StringBuilder(""), 0)); // Index > builder.Length

            AssertExtensions.Throws<ArgumentOutOfRangeException>("repeatCount", () => builder.Insert(0, new StringBuilder(""), -1)); // Count < 0

            AssertExtensions.Throws<ArgumentOutOfRangeException>("requiredLength", () => builder.Insert(builder.Length, new StringBuilder("aa"))); // New length > builder.MaxCapacity
            Assert.Throws<OutOfMemoryException>(() => builder.Insert(builder.Length, new StringBuilder("aa"), 1)); // New length > builder.MaxCapacity
            Assert.Throws<OutOfMemoryException>(() => builder.Insert(builder.Length, new StringBuilder("a"), 2)); // New length > builder.MaxCapacity
        }


        [Theory]
        [InlineData("Hello", 0, new char[] { '\0' }, 0, 1, "\0Hello")]
        [InlineData("Hello", 3, new char[] { 'a', 'b', 'c' }, 0, 1, "Helalo")]
        [InlineData("Hello", 3, new char[] { 'a', 'b', 'c' }, 0, 3, "Helabclo")]
        [InlineData("Hello", 5, new char[] { 'd', 'e', 'f' }, 0, 1, "Hellod")]
        [InlineData("Hello", 5, new char[] { 'd', 'e', 'f' }, 0, 3, "Hellodef")]
        [InlineData("Hello", 0, new char[0], 0, 0, "Hello")]
        [InlineData("Hello", 0, null!, 0, 0, "Hello")]
        [InlineData("Hello", 3, new char[] { 'a', 'b', 'c' }, 1, 1, "Helblo")]
        [InlineData("Hello", 3, new char[] { 'a', 'b', 'c' }, 1, 2, "Helbclo")]
        [InlineData("Hello", 3, new char[] { 'a', 'b', 'c' }, 0, 2, "Helablo")]
        public void Insert_CharArray(string? original, int index, char[]? value, int startIndex, int charCount, string expected)
        {
            MutableTextBuffer builder;
            if (startIndex == 0 && charCount == (value?.Length ?? 0))
            {
                // Use Insert(int, char[])
                builder = MutableTextBufferFactory(original);
                builder.Insert(index, value);
                Assert.Equal(expected, builder.ToString());
            }
            // Use Insert(int, char[], int, int)
            builder = MutableTextBufferFactory(original);
            builder.Insert(index, value, startIndex, charCount);
            Assert.Equal(expected, builder.ToString());
        }

        [Fact]
        public void Insert_CharArray_Invalid()
        {
            var builder = MutableTextBufferFactory(0, 5);
            builder.Append("Hello");

            AssertExtensions.Throws<ArgumentOutOfRangeException>("index", () => builder.Insert(-1, new char[1])); // Index < 0
            AssertExtensions.Throws<ArgumentOutOfRangeException>("index", () => builder.Insert(-1, new char[0], 0, 0)); // Index < 0

            AssertExtensions.Throws<ArgumentOutOfRangeException>("index", () => builder.Insert(builder.Length + 1, new char[1])); // Index > builder.Length
            AssertExtensions.Throws<ArgumentOutOfRangeException>("index", () => builder.Insert(builder.Length + 1, new char[0], 0, 0)); // Index > builder.Length

            Assert.Throws<ArgumentNullException>(() => builder.Insert(0, (char[]?)null, 1, 1)); // Value is null (startIndex and count are not zero)
            AssertExtensions.Throws<ArgumentOutOfRangeException>("startIndex", () => builder.Insert(0, new char[0], -1, 0)); // Start index < 0

            AssertExtensions.Throws<ArgumentOutOfRangeException>("startIndex", () => builder.Insert(0, new char[3], 4, 0)); // Start index + char count > value.Length
            AssertExtensions.Throws<ArgumentOutOfRangeException>("startIndex", () => builder.Insert(0, new char[3], 3, 1)); // Start index + char count > value.Length
            AssertExtensions.Throws<ArgumentOutOfRangeException>("startIndex", () => builder.Insert(0, new char[3], 2, 2)); // Start index + char count > value.Length

            AssertExtensions.Throws<ArgumentOutOfRangeException>("requiredLength", () => builder.Insert(builder.Length, new char[1])); // New length > builder.MaxCapacity
            AssertExtensions.Throws<ArgumentOutOfRangeException>("requiredLength", () => builder.Insert(builder.Length, new char[] { 'a' }, 0, 1)); // New length > builder.MaxCapacity
        }

        [Fact]
        public void Insert_CharArray_InvalidCount()
        {
            var builder = MutableTextBufferFactory(0, 5);
            builder.Append("Hello");
            AssertExtensions.Throws<ArgumentOutOfRangeException>("charCount", () => builder.Insert(0, new char[0], 0, -1)); // Char count < 0
        }

        [Fact]
        public void Insert_CharArray_InvalidCharCount()
        {
            var builder = MutableTextBufferFactory(0, 5);
            builder.Append("Hello");
            AssertExtensions.Throws<ArgumentOutOfRangeException>("charCount", () => builder.Insert(0, new char[0], 0, -1)); // Char count < 0
        }

        [Theory]
        [InlineData("", 0, 0, "")]
        [InlineData("Hello", 0, 5, "")]
        [InlineData("Hello", 1, 3, "Ho")]
        [InlineData("Hello", 1, 4, "H")]
        [InlineData("Hello", 1, 0, "Hello")]
        [InlineData("Hello", 5, 0, "Hello")]
        [InlineData("Hello", 1, 2, "Hlo")]
        [InlineData("HelloHello", 1, 2, "HloHello")]
        public void Remove(string value, int startIndex, int length, string expected)
        {
            var builder = MutableTextBufferFactory(value);
            builder.Remove(startIndex, length);
            Assert.Equal(expected, builder.ToString());
        }

        // J2N TODO: StringBuilder with multiple chunks?
        //[Theory]
        //[InlineData(1, 29, "a")]
        //[InlineData(0, 29, "a")]
        //[InlineData(20, 10, "aaaaaaaaaaaaaaaaaaaa")]
        //[InlineData(0, 15, "aaaaaaaaaaaaaaa")]
        //public void Remove_StringBuilderWithMultipleChunks(int startIndex, int count, string expected)
        //{
        //    StringBuilder builder = StringBuilderWithMultipleChunks();
        //    builder.Remove(startIndex, count);
        //    Assert.Equal(expected, builder.ToString());
        //}

        [Fact]
        public void Remove_Invalid()
        {
            var builder = MutableTextBufferFactory("Hello");
            AssertExtensions.Throws<ArgumentOutOfRangeException>("startIndex", () => builder.Remove(-1, 0)); // Start index < 0
            AssertExtensions.Throws<ArgumentOutOfRangeException>("length", () => builder.Remove(0, -1)); // Length < 0
            AssertExtensions.Throws<ArgumentOutOfRangeException>("length", () => builder.Remove(6, 0)); // Start index + length > 0
            AssertExtensions.Throws<ArgumentOutOfRangeException>("length", () => builder.Remove(5, 1)); // Start index + length > 0
            AssertExtensions.Throws<ArgumentOutOfRangeException>("length", () => builder.Remove(4, 2)); // Start index + length > 0
        }

        [Theory]
        [InlineData("", 'a', '!', 0, 0, "")]
        [InlineData("aaaabbbbccccdddd", 'a', '!', 0, 16, "!!!!bbbbccccdddd")]
        [InlineData("aaaabbbbccccdddd", 'a', '!', 0, 4, "!!!!bbbbccccdddd")]
        [InlineData("aaaabbbbccccdddd", 'a', '!', 2, 3, "aa!!bbbbccccdddd")]
        [InlineData("aaaabbbbccccdddd", 'a', '!', 4, 1, "aaaabbbbccccdddd")]
        [InlineData("aaaabbbbccccdddd", 'b', '!', 0, 0, "aaaabbbbccccdddd")]
        [InlineData("aaaabbbbccccdddd", 'a', '!', 16, 0, "aaaabbbbccccdddd")]
        [InlineData("aaaabbbbccccdddd", 'e', '!', 0, 16, "aaaabbbbccccdddd")]
        public void Replace_Char(string value, char oldChar, char newChar, int startIndex, int count, string expected)
        {
            MutableTextBuffer builder;
            if (startIndex == 0 && count == value.Length)
            {
                // Use Replace(char, char)
                builder = MutableTextBufferFactory(value);
                builder.Replace(oldChar, newChar);
                Assert.Equal(expected, builder.ToString());
            }
            // Use Replace(char, char, int, int)
            builder = MutableTextBufferFactory(value);
            builder.Replace(oldChar, newChar, startIndex, count);
            Assert.Equal(expected, builder.ToString());
        }

        // J2N TODO: StringBuilder with multiple chunks?
        //[Fact]
        //public void Replace_Char_StringBuilderWithMultipleChunks()
        //{
        //    StringBuilder builder = StringBuilderWithMultipleChunks();
        //    builder.Replace('a', 'b', 0, builder.Length);
        //    Assert.Equal(new string('b', builder.Length), builder.ToString());
        //}

        [Fact]
        public void Replace_Char_Invalid()
        {
            var builder = MutableTextBufferFactory("Hello");
            AssertExtensions.Throws<ArgumentOutOfRangeException>("startIndex", () => builder.Replace('a', 'b', -1, 0)); // Start index < 0
            AssertExtensions.Throws<ArgumentOutOfRangeException>("count", () => builder.Replace('a', 'b', 0, -1)); // Count < 0

            AssertExtensions.Throws<ArgumentOutOfRangeException>("startIndex", () => builder.Replace('a', 'b', 6, 0)); // Count + start index > builder.Length
            AssertExtensions.Throws<ArgumentOutOfRangeException>("count", () => builder.Replace('a', 'b', 5, 1)); // Count + start index > builder.Length
            AssertExtensions.Throws<ArgumentOutOfRangeException>("count", () => builder.Replace('a', 'b', 4, 2)); // Count + start index > builder.Length
        }

        [Theory]
        [InlineData("Hello", 0, 5, "Hello")]
        [InlineData("Hello", 2, 3, "llo")]
        [InlineData("Hello", 2, 2, "ll")]
        [InlineData("Hello", 5, 0, "")]
        [InlineData("Hello", 4, 0, "")]
        [InlineData("Hello", 0, 0, "")]
        [InlineData("", 0, 0, "")]
        public void ToStringTest(string value, int startIndex, int length, string expected)
        {
            var builder = MutableTextBufferFactory(value);
            if (startIndex == 0 && length == value.Length)
            {
                Assert.Equal(expected, builder.ToString());
            }
            Assert.Equal(expected, builder.ToString(startIndex, length));
        }

        // J2N TODO: StringBuilder with multiple chunks?
        //[Fact]
        //public void ToString_StringBuilderWithMultipleChunks()
        //{
        //    StringBuilder builder = StringBuilderWithMultipleChunks();
        //    Assert.Equal(s_chunkSplitSource, builder.ToString());
        //    Assert.Equal(s_chunkSplitSource, builder.ToString(0, builder.Length));
        //    Assert.Equal("a", builder.ToString(0, 1));
        //    Assert.Equal(string.Empty, builder.ToString(builder.Length - 1, 0));
        //}

        [Fact]
        public void ToString_Invalid()
        {
            var builder = MutableTextBufferFactory("Hello");
            AssertExtensions.Throws<ArgumentOutOfRangeException>("startIndex", () => builder.ToString(-1, 0)); // Start index < 0
            AssertExtensions.Throws<ArgumentOutOfRangeException>("length", () => builder.ToString(0, -1)); // Length < 0

            AssertExtensions.Throws<ArgumentOutOfRangeException>("startIndex", () => builder.ToString(6, 0)); // Length + start index > builder.Length
            AssertExtensions.Throws<ArgumentOutOfRangeException>("length", () => builder.ToString(5, 1)); // Length + start index > builder.Length
            AssertExtensions.Throws<ArgumentOutOfRangeException>("length", () => builder.ToString(4, 2)); // Length + start index > builder.Length
        }

        public class CustomFormatter : ICustomFormatter, IFormatProvider
        {
            public string Format(string? format, object? arg, IFormatProvider? formatProvider) => "abc";
            public object? GetFormat(Type? formatType) => this;
        }

        [Fact]
        public void AppendJoin_NullValues_ThrowsArgumentNullException()
        {
            AssertExtensions.Throws<ArgumentNullException>("values", () => MutableTextBufferFactory().AppendJoin('|', (object?[])null!));
            AssertExtensions.Throws<ArgumentNullException>("values", () => MutableTextBufferFactory().AppendJoin('|', (IEnumerable<object?>)null!));
            AssertExtensions.Throws<ArgumentNullException>("values", () => MutableTextBufferFactory().AppendJoin('|', (string?[])null!));
            AssertExtensions.Throws<ArgumentNullException>("values", () => MutableTextBufferFactory().AppendJoin("|", (object?[])null!));
            AssertExtensions.Throws<ArgumentNullException>("values", () => MutableTextBufferFactory().AppendJoin("|", (IEnumerable<object?>)null!));
            AssertExtensions.Throws<ArgumentNullException>("values", () => MutableTextBufferFactory().AppendJoin("|", (string?[])null!));
        }

        [Theory]
        [InlineData(new object?[0], "")]
        [InlineData(new object?[] { null }, "")]
        [InlineData(new object?[] { 10 }, "10")]
        [InlineData(new object?[] { null, null }, "|")]
        [InlineData(new object?[] { null, 20 }, "|20")]
        [InlineData(new object?[] { 10, null }, "10|")]
        [InlineData(new object?[] { 10, 20 }, "10|20")]
        [InlineData(new object?[] { null, null, null }, "||")]
        [InlineData(new object?[] { null, null, 30 }, "||30")]
        [InlineData(new object?[] { null, 20, null }, "|20|")]
        [InlineData(new object?[] { null, 20, 30 }, "|20|30")]
        [InlineData(new object?[] { 10, null, null }, "10||")]
        [InlineData(new object?[] { 10, null, 30 }, "10||30")]
        [InlineData(new object?[] { 10, 20, null }, "10|20|")]
        [InlineData(new object?[] { 10, 20, 30 }, "10|20|30")]
        [InlineData(new object?[] { "" }, "")]
        [InlineData(new object?[] { "", "" }, "|")]
        public void AppendJoin_TestValues(object?[] values, string expected)
        {
            var stringValues = Array.ConvertAll(values, _ => _?.ToString());
            var enumerable = values.Select(_ => _);

            Assert.Equal(expected, MutableTextBufferFactory().Apply((sb) => sb.AppendJoin('|', values)).ToString());
            Assert.Equal(expected, MutableTextBufferFactory().Apply((sb) => sb.AppendJoin('|', (ReadOnlySpan<object?>)values)).ToString());
            Assert.Equal(expected, MutableTextBufferFactory().Apply((sb) => sb.AppendJoin('|', enumerable)).ToString());
            Assert.Equal(expected, MutableTextBufferFactory().Apply((sb) => sb.AppendJoin('|', stringValues)).ToString());
            Assert.Equal(expected, MutableTextBufferFactory().Apply((sb) => sb.AppendJoin('|', (ReadOnlySpan<string?>)stringValues)).ToString());
            Assert.Equal(expected, MutableTextBufferFactory().Apply((sb) => sb.AppendJoin("|", values)).ToString());
            Assert.Equal(expected, MutableTextBufferFactory().Apply((sb) => sb.AppendJoin("|", (ReadOnlySpan<object?>)values)).ToString());
            Assert.Equal(expected, MutableTextBufferFactory().Apply((sb) => sb.AppendJoin("|", enumerable)).ToString());
            Assert.Equal(expected, MutableTextBufferFactory().Apply((sb) => sb.AppendJoin("|", stringValues)).ToString());
            Assert.Equal(expected, MutableTextBufferFactory().Apply((sb) => sb.AppendJoin("|", (ReadOnlySpan<string?>)stringValues)).ToString());
        }

        [Fact]
        public void AppendJoin_NullToStringValues()
        {
            AppendJoin_TestValues(new object[] { new NullToStringObject() }, "");
            AppendJoin_TestValues(new object[] { new NullToStringObject(), new NullToStringObject() }, "|");
        }

        private sealed class NullToStringObject
        {
            public override string ToString() => null!;
        }

        [Theory]
        [InlineData(null, "123")]
        [InlineData("", "123")]
        [InlineData(" ", "1 2 3")]
        [InlineData(", ", "1, 2, 3")]
        public void AppendJoin_TestStringSeparators(string? separator, string expected)
        {
            var values = new object?[] { 1, 2, 3 };
            var stringValues = new string?[] { "1", "2", "3" };

            Assert.Equal(expected, MutableTextBufferFactory().Apply((sb) => sb.AppendJoin(separator, values)).ToString());
            Assert.Equal(expected, MutableTextBufferFactory().Apply((sb) => sb.AppendJoin(separator, (ReadOnlySpan<object?>)values)).ToString());
            Assert.Equal(expected, MutableTextBufferFactory().Apply((sb) => sb.AppendJoin(separator, Enumerable.Range(1, 3))).ToString());
            Assert.Equal(expected, MutableTextBufferFactory().Apply((sb) => sb.AppendJoin(separator, stringValues)).ToString());
            Assert.Equal(expected, MutableTextBufferFactory().Apply((sb) => sb.AppendJoin(separator, (ReadOnlySpan<string?>)stringValues)).ToString());
        }


        private MutableTextBuffer CreateBuilderWithNoSpareCapacity()
        {
            return MutableTextBufferFactory(0, 5).Apply((sb) => sb.Append("Hello"));
        }

        [Theory]
        [InlineData(null, new object?[] { null, null })]
        [InlineData("", new object?[] { "", "" })]
        [InlineData(" ", new object?[] { })]
        [InlineData(", ", new object?[] { "" })]
        public void AppendJoin_NoValues_NoSpareCapacity_DoesNotThrow(string? separator, object?[] values)
        {
            var stringValues = Array.ConvertAll(values, _ => _?.ToString());
            var enumerable = values.Select(_ => _);

            if (separator?.Length == 1)
            {
                CreateBuilderWithNoSpareCapacity().AppendJoin(separator[0], values);
                CreateBuilderWithNoSpareCapacity().AppendJoin(separator[0], (ReadOnlySpan<object?>)values);
                CreateBuilderWithNoSpareCapacity().AppendJoin(separator[0], enumerable);
                CreateBuilderWithNoSpareCapacity().AppendJoin(separator[0], stringValues);
                CreateBuilderWithNoSpareCapacity().AppendJoin(separator[0], (ReadOnlySpan<string?>)stringValues);
            }
            CreateBuilderWithNoSpareCapacity().AppendJoin(separator, values);
            CreateBuilderWithNoSpareCapacity().AppendJoin(separator, (ReadOnlySpan<object?>)values);
            CreateBuilderWithNoSpareCapacity().AppendJoin(separator, enumerable);
            CreateBuilderWithNoSpareCapacity().AppendJoin(separator, stringValues);
            CreateBuilderWithNoSpareCapacity().AppendJoin(separator, (ReadOnlySpan<string?>)stringValues);
        }

        [Theory]
        [InlineData(null, new object?[] { " " })]
        [InlineData(" ", new object?[] { " " })]
        [InlineData(" ", new object?[] { null, null })]
        [InlineData(" ", new object?[] { "", "" })]
        public void AppendJoin_NoSpareCapacity_ThrowsArgumentOutOfRangeException(string? separator, object?[] values)
        {
            // J2N: Stray code from upstream - not used in below test
            //var builder = MutableTextBufferFactory(0, 5);
            //builder.Append("Hello");

            var stringValues = Array.ConvertAll(values, _ => _?.ToString());
            var enumerable = values.Select(_ => _);

            if (separator?.Length == 1)
            {
                AssertExtensions.Throws<ArgumentOutOfRangeException>(s_noCapacityParamName, () => CreateBuilderWithNoSpareCapacity().AppendJoin(separator[0], values));
                AssertExtensions.Throws<ArgumentOutOfRangeException>(s_noCapacityParamName, () => CreateBuilderWithNoSpareCapacity().AppendJoin(separator[0], (ReadOnlySpan<object?>)values));
                AssertExtensions.Throws<ArgumentOutOfRangeException>(s_noCapacityParamName, () => CreateBuilderWithNoSpareCapacity().AppendJoin(separator[0], enumerable));
                AssertExtensions.Throws<ArgumentOutOfRangeException>(s_noCapacityParamName, () => CreateBuilderWithNoSpareCapacity().AppendJoin(separator[0], stringValues));
                AssertExtensions.Throws<ArgumentOutOfRangeException>(s_noCapacityParamName, () => CreateBuilderWithNoSpareCapacity().AppendJoin(separator[0], (ReadOnlySpan<string?>)stringValues));
            }
            AssertExtensions.Throws<ArgumentOutOfRangeException>(s_noCapacityParamName, () => CreateBuilderWithNoSpareCapacity().AppendJoin(separator, values));
            AssertExtensions.Throws<ArgumentOutOfRangeException>(s_noCapacityParamName, () => CreateBuilderWithNoSpareCapacity().AppendJoin(separator, (ReadOnlySpan<object?>)values));
            AssertExtensions.Throws<ArgumentOutOfRangeException>(s_noCapacityParamName, () => CreateBuilderWithNoSpareCapacity().AppendJoin(separator, enumerable));
            AssertExtensions.Throws<ArgumentOutOfRangeException>(s_noCapacityParamName, () => CreateBuilderWithNoSpareCapacity().AppendJoin(separator, stringValues));
            AssertExtensions.Throws<ArgumentOutOfRangeException>(s_noCapacityParamName, () => CreateBuilderWithNoSpareCapacity().AppendJoin(separator, (ReadOnlySpan<string?>)stringValues));
        }

        [Theory]
        [InlineData("Hello", new char[] { 'a' }, "Helloa")]
        [InlineData("Hello", new char[] { 'b', 'c', 'd' }, "Hellobcd")]
        [InlineData("Hello", new char[] { 'b', '\0', 'd' }, "Hellob\0d")]
        [InlineData("", new char[] { 'e', 'f', 'g' }, "efg")]
        [InlineData("Hello", new char[0], "Hello")]
        public void Append_CharSpan(string? original, char[]? value, string expected)
        {
            var builder = MutableTextBufferFactory(original);
            builder.Append(new ReadOnlySpan<char>(value));
            Assert.Equal(expected, builder.ToString());
        }

        [Theory]
        [MemberData(nameof(Append_Overlapping_TestData))]
        public void Append_CharSpan_Overlapping(string value, int sourceIndex, int sourceLength, string expected)
        {
            MutableTextBuffer builder = MutableTextBufferFactory(value);
            ReadOnlySpan<char> source = builder.AsSpan(sourceIndex, sourceLength);
            builder.Append(source);
            Assert.Equal(expected, builder.ToString());
        }

        [Theory]
        [InlineData(0, 8)]
        [InlineData(0, 4)]
        [InlineData(2, 4)]
        [InlineData(3, 1)]
        [InlineData(7, 1)]
        public void Append_CharSpan_IsSelf(int sourceIndex, int length)
        {
            const string original = "ABCDEFGH";

            var expected = MutableTextBufferFactory(original);
            expected.Append(original.AsSpan(sourceIndex, length));

            var actual = MutableTextBufferFactory(original);
            actual.Append(actual.AsSpan(sourceIndex, length));

            Assert.Equal(expected.ToString(), actual.ToString());
        }

        [Fact]
        public void Append_CharSpan_SourceIsUnusedBufferInSelf_AppendsCorrectly()
        {
            var sb = MutableTextBufferFactory("12345678", capacity: 32);
            Span<char> source = sb.RawChars.Slice(10, 5);
            "abcde".AsSpan().CopyTo(source);
            sb.Append(source);
            Assert.Equal("12345678abcde", sb.ToString());
        }

        [Theory]
        [InlineData(0, 8)]
        [InlineData(0, 4)]
        [InlineData(2, 4)]
        [InlineData(3, 1)]
        public void Append_CharSpan_IsSelf_GrowingBuffer(int sourceIndex, int length)
        {
            const string original = "ABCDEFGH";

            var expected = MutableTextBufferFactory(original, capacity: original.Length);
            expected.Append(original.AsSpan(sourceIndex, length));

            var actual = MutableTextBufferFactory(original, capacity: original.Length);
            actual.Append(actual.AsSpan(sourceIndex, length));

            Assert.Equal(expected.ToString(), actual.ToString());
        }

        [Theory]
        [InlineData("ABCDEFGH", 8, 0, 8)]
        [InlineData("ABCDEFGH", 8, 0, 4)]
        [InlineData("ABCDEFGH", 8, 2, 4)]
        [InlineData("ABCDEFGH", 8, 3, 1)]
        [InlineData("ABCDEFGH", 8, 7, 1)]
        public void Append_CharSpan_SelfSpan_GrowingBuffer_ShouldNotReadReturnedBuffer(string original, int capacity, int sourceIndex, int length)
        {
            var allocator = new EvilCharArrayAllocator();

            var expected = MutableTextBufferFactory(original);
            expected.Append(original.AsSpan(sourceIndex, length));

            var actual = MutableTextBufferFactory(original, capacity, allocator);
            actual.Append(actual.AsSpan(sourceIndex, length));

            Assert.Equal(expected.ToString(), actual.ToString());
        }

        [Theory]
        [InlineData("ABCDEFGH", 8)]
        [InlineData("abcdefghijklmnopqrstuvwxyz", 26)]
        [InlineData("123456789", 9)]
        public void Append_CharSpan_Self_GrowingBuffer_ShouldNotReadReturnedBuffer(string original, int capacity)
        {
            var allocator = new EvilCharArrayAllocator();

            var expected = MutableTextBufferFactory(original);
            expected.Append(original);

            var actual = MutableTextBufferFactory(original, capacity, allocator);
            actual.Append(actual.AsSpan());

            Assert.Equal(expected.ToString(), actual.ToString());
        }

        [Theory]
        [InlineData("12345678", 8, 8, 5, "abcde")]
        [InlineData("ABCDEFGH", 8, 12, 3, "XYZ")]
        [InlineData("Hello", 5, 6, 5, "World")]
        public void Append_CharSpan_UnusedBuffer_GrowingBuffer_ShouldCopyBeforeGrow(string original, int capacity,
            int sourceOffset, int sourceLength, string sourceValue)
        {
            var allocator = new EvilCharArrayAllocator();

            var builder = MutableTextBufferFactory(original, capacity, allocator);

            sourceValue.AsSpan().CopyTo(
                builder.RawChars.Slice(sourceOffset, sourceLength));

            builder.Append(builder.RawChars.Slice(sourceOffset, sourceLength));

            Assert.Equal(original + sourceValue, builder.ToString());
        }

        private sealed class EvilCharArrayAllocator : IArrayAllocator<char>
        {
            private char[]? returned;

            public bool GuaranteesClearedArrays => false;

            public char[] Allocate(int minimumLength)
            {
                if (returned != null && returned.Length >= minimumLength)
                {
                    char[] result = returned;
                    returned = null;

                    // Scribble over it a second time.
                    result.AsSpan().Fill('#');

                    return result;
                }

                return new char[minimumLength];
            }

            public void Return(char[] array)
            {
                // Immediately relinquish ownership.
                array.AsSpan().Fill('!');
                returned = array;
            }
        }

        [Theory]
        [InlineData("Hello", new char[] { 'a' }, "Helloa")]
        [InlineData("Hello", new char[] { 'b', 'c', 'd' }, "Hellobcd")]
        [InlineData("Hello", new char[] { 'b', '\0', 'd' }, "Hellob\0d")]
        [InlineData("", new char[] { 'e', 'f', 'g' }, "efg")]
        [InlineData("Hello", new char[0], "Hello")]
        public void Append_CharMemory(string? original, char[]? value, string expected)
        {
            var builder = MutableTextBufferFactory(original);
            builder.Append(value.AsMemory());
            Assert.Equal(expected, builder.ToString());
        }

        // J2N: We don't shrink on Clear() because that would cause another allocation
        //[Theory]
        //[InlineData(1)]
        //[InlineData(10000)]
        //public void Clear_AppendAndInsertBeforeClearManyTimes_CapacityStaysWithinRange(int times)
        //{
        //    var builder = MutableTextBufferFactory();
        //    var originalCapacity = builder.Capacity;
        //    var s = new string(' ', 10);
        //    int oldLength = 0;
        //    for (int i = 0; i < times; i++)
        //    {
        //        builder.Append(s);
        //        builder.Append(s);
        //        builder.Append(s);
        //        builder.Insert(0, s);
        //        builder.Insert(0, s);
        //        oldLength = builder.Length;

        //        builder.Clear();
        //    }
        //    Assert.InRange(builder.Capacity, 1, oldLength * 1.2);
        //}

        // J2N: We don't shrink on Clear() because that would cause another allocation
        //[Fact]
        //public void Clear_InitialCapacityMuchLargerThanLength_CapacityReducedToInitialCapacity()
        //{
        //    var builder = MutableTextBufferFactory(100);
        //    var initialCapacity = builder.Capacity;
        //    builder.Append(new string('a', 40));
        //    builder.Insert(0, new string('a', 10));
        //    builder.Insert(0, new string('a', 10));
        //    builder.Insert(0, new string('a', 10));
        //    var oldCapacity = builder.Capacity;
        //    var oldLength = builder.Length;
        //    builder.Clear();
        //    Assert.NotEqual(oldCapacity, builder.Capacity);
        //    Assert.Equal(initialCapacity, builder.Capacity);
        //    Assert.NotInRange(builder.Capacity, 1, oldLength * 1.2);
        //    Assert.InRange(builder.Capacity, 1, Math.Max(initialCapacity, oldLength * 1.2));
        //}

        // J2N: We don't shrink on Clear() because that would cause another allocation
        //[Fact]
        //public void Clear_StringBuilderHasTwoChunks_OneChunkIsEmpty_ClearReducesCapacity()
        //{
        //    var sb = MutableTextBufferFactory(string.Empty);
        //    int initialCapacity = sb.Capacity;
        //    for (int i = 0; i < initialCapacity; i++)
        //    {
        //        sb.Append('a');
        //    }
        //    sb.Insert(0, 'a');
        //    while (sb.Length > 1)
        //    {
        //        sb.Remove(1, 1);
        //    }
        //    int oldCapacity = sb.Capacity;
        //    sb.Clear();
        //    Assert.Equal(oldCapacity - 1, sb.Capacity);
        //    Assert.Equal(initialCapacity, sb.Capacity);
        //}

        [Theory]
        [InlineData("Hello", 0, new char[] { '\0', '\0', '\0', '\0', '\0' }, 5, new char[] { 'H', 'e', 'l', 'l', 'o' })]
        [InlineData("Hello", 0, new char[] { '\0', '\0', '\0', '\0' }, 4, new char[] { 'H', 'e', 'l', 'l' })]
        [InlineData("Hello", 1, new char[] { '\0', '\0', '\0', '\0', '\0' }, 4, new char[] { 'e', 'l', 'l', 'o', '\0' })]
        public void CopyTo_CharSpan(string value, int sourceIndex, char[] destination, int count, char[] expected)
        {
            var builder = MutableTextBufferFactory(value);
            builder.CopyTo(sourceIndex, new Span<char>(destination), count);
            Assert.Equal(expected, destination);
        }

        // J2N TODO: StringBuilder with multiple chunks?
        //[Fact]
        //public void CopyTo_CharSpan_StringBuilderWithMultipleChunks()
        //{
        //    MutableTextBuffer builder = StringBuilderWithMultipleChunks();
        //    char[] destination = new char[builder.Length];
        //    builder.CopyTo(0, new Span<char>(destination), destination.Length);
        //    Assert.Equal(s_chunkSplitSource.ToCharArray(), destination);
        //}

        [Fact]
        public void CopyTo_CharSpan_Invalid()
        {
            var builder = MutableTextBufferFactory("Hello");

            AssertExtensions.Throws<ArgumentOutOfRangeException>("sourceIndex", () => builder.CopyTo(-1, new Span<char>(new char[10]), 0)); // Source index < 0
            AssertExtensions.Throws<ArgumentOutOfRangeException>("sourceIndex", () => builder.CopyTo(6, new Span<char>(new char[10]), 0)); // Source index > builder.Length

            AssertExtensions.Throws<ArgumentOutOfRangeException>("count", () => builder.CopyTo(0, new Span<char>(new char[10]), -1)); // Count < 0

            AssertExtensions.Throws<ArgumentException>(null, () => builder.CopyTo(5, new Span<char>(new char[10]), 1)); // Source index + count > builder.Length
            AssertExtensions.Throws<ArgumentException>(null, () => builder.CopyTo(4, new Span<char>(new char[10]), 2)); // Source index + count > builder.Length

            AssertExtensions.Throws<ArgumentException>(null, () => builder.CopyTo(0, new Span<char>(new char[10]), 11)); // count > destinationArray.Length
        }

        [Theory]
        [InlineData("Hello", 0, new char[] { '\0' }, "\0Hello")]
        [InlineData("Hello", 3, new char[] { 'a', 'b', 'c' }, "Helabclo")]
        [InlineData("Hello", 5, new char[] { 'd', 'e', 'f' }, "Hellodef")]
        [InlineData("Hello", 0, new char[0], "Hello")]
        public void Insert_CharSpan(string original, int index, char[] value, string expected)
        {
            var builder = MutableTextBufferFactory(original);
            builder.Insert(index, new ReadOnlySpan<char>(value));
            Assert.Equal(expected, builder.ToString());
        }

        [Theory]
        [MemberData(nameof(Insert_Overlapping_TestData))]
        public void Insert_CharSpan_Overlapping(string value, int index, int sourceIndex, int sourceLength, string expected)
        {
            MutableTextBuffer builder = MutableTextBufferFactory(value);
            ReadOnlySpan<char> source = builder.AsSpan(sourceIndex, sourceLength);
            builder.Insert(index, source);
            Assert.Equal(expected, builder.ToString());
        }

        [Theory]
        [InlineData("ABCDEFGH", 0, 4)]
        [InlineData("ABCDEFGH", 1, 3)]
        [InlineData("ABCDEFGH", 2, 2)]
        [InlineData("ABCDEFGH", 3, 1)]
        [InlineData("ABCDEFGH", 0, 2)]
        public void Insert_CharSpan_IsSelf_ShouldMatchInsertSelf(string original, int sourceIndex, int length)
        {
            for (int insertIndex = 0; insertIndex <= original.Length; insertIndex++)
            {
                var expected = MutableTextBufferFactory(original);
                expected.InsertFromSelf(insertIndex, sourceIndex, length);

                var actual = MutableTextBufferFactory(original);
                actual.Insert(insertIndex,
                    actual.AsSpan(sourceIndex, length));

                Assert.Equal(expected.ToString(), actual.ToString());
            }
        }

        [Fact]
        public void Insert_CharSpan_Invalid()
        {
            var builder = MutableTextBufferFactory(0, 5);
            builder.Append("Hello");

            AssertExtensions.Throws<ArgumentOutOfRangeException>("index", () => builder.Insert(-1, new ReadOnlySpan<char>(new char[0]))); // Index < 0
            AssertExtensions.Throws<ArgumentOutOfRangeException>("index", () => builder.Insert(builder.Length + 1, new ReadOnlySpan<char>(new char[0]))); // Index > builder.Length
            AssertExtensions.Throws<ArgumentOutOfRangeException>("requiredLength", () => builder.Insert(builder.Length, new ReadOnlySpan<char>(new char[1]))); // New length > builder.MaxCapacity
        }

        /// <summary>
        /// Stresses the MakeRoom() implementation
        /// </summary>
        [Theory]
        [InlineData("0123456789", 0, "ABC")]
        [InlineData("0123456789", 1, "ABC")]
        [InlineData("0123456789", 5, "ABC")]
        [InlineData("0123456789", 9, "ABC")]
        [InlineData("0123456789", 10, "ABC")]
        public void Insert_CharSpan_InPlaceMove_MatchesStringInsert(string original, int index, string value)
        {
            // Enough spare capacity to guarantee the in-place MakeRoom() path.
            var builder = MutableTextBufferFactory(
                original,
                original.Length + value.Length + 10);

            builder.Insert(index, value.AsSpan());

            Assert.Equal(
                original.Insert(index, value),
                builder.ToString());
        }

        /// <summary>
        /// Stresses the MakeRoom() implementation
        /// </summary>
        [Theory]
        [InlineData("0123456789", 0, "ABC")]
        [InlineData("0123456789", 1, "ABC")]
        [InlineData("0123456789", 5, "ABC")]
        [InlineData("0123456789", 9, "ABC")]
        [InlineData("0123456789", 10, "ABC")]
        public void Insert_CharSpan_ReallocationMove_MatchesStringInsert(string original, int index, string value)
        {
            // No spare capacity. Forces allocation.
            var builder = MutableTextBufferFactory(
                original,
                original.Length);

            builder.Insert(index, value.AsSpan());

            Assert.Equal(
                original.Insert(index, value),
                builder.ToString());
        }

        /// <summary>
        /// Stresses the MakeRoom() implementation
        /// </summary>
        [Fact]
        public void Insert_CharSpan_InPlaceMove_LargeOverlap_MatchesStringInsert()
        {
            string original = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string value = "1234567890";

            var builder = MutableTextBufferFactory(
                original,
                original.Length + value.Length + 10);

            builder.Insert(1, value.AsSpan());

            Assert.Equal(
                original.Insert(1, value),
                builder.ToString());
        }

        [Fact]
        public void Insert_CharSpan_RepeatedFrontInsertions_MatchesStringInsert()
        {
            string expected = "XYZ";

            var builder = MutableTextBufferFactory("XYZ", 64);

            builder.Insert(0, "1".AsSpan());
            expected = expected.Insert(0, "1");

            builder.Insert(0, "2".AsSpan());
            expected = expected.Insert(0, "2");

            builder.Insert(0, "3".AsSpan());
            expected = expected.Insert(0, "3");

            builder.Insert(0, "4".AsSpan());
            expected = expected.Insert(0, "4");

            Assert.Equal(expected, builder.ToString());
        }

        public IEnumerable<object?[]> Append_MutableTextBuffer_TestData()
        {
            string mediumString = new string('a', 30);
            string largeString = new string('b', 1000);

            var sb1 = MutableTextBufferFactory("Hello");
            var sb2 = MutableTextBufferFactory("one");
            var sb3 = MutableTextBufferFactory(20).Apply((sb) => sb.Append(mediumString));

            yield return new object?[] { MutableTextBufferFactory("Hello"), sb1, "HelloHello" };
            yield return new object?[] { MutableTextBufferFactory("Hello"), sb2, "Helloone" };
            yield return new object?[] { MutableTextBufferFactory("Hello"), MutableTextBufferFactory(), "Hello" };

            yield return new object?[] { MutableTextBufferFactory("one"), sb3, "one" + mediumString };

            yield return new object?[] { MutableTextBufferFactory(20).Apply((sb) => sb.Append(mediumString)), sb3, mediumString + mediumString };
            yield return new object?[] { MutableTextBufferFactory(10).Apply((sb) => sb.Append(mediumString)), sb3, mediumString + mediumString };

            yield return new object?[] { MutableTextBufferFactory(20).Apply((sb) => sb.Append(largeString)), sb3, largeString + mediumString };
            yield return new object?[] { MutableTextBufferFactory(10).Apply((sb) => sb.Append(largeString)), sb3, largeString + mediumString };

            yield return new object?[] { MutableTextBufferFactory(10), sb3, mediumString };
            yield return new object?[] { MutableTextBufferFactory(30), sb3, mediumString };
            yield return new object?[] { MutableTextBufferFactory(10), MutableTextBufferFactory(20), string.Empty };

            yield return new object?[] { sb1, null, "Hello" };
            yield return new object?[] { sb1, sb1, "HelloHello" };
        }

        [Fact]
        public void Append_MutableTextBuffer()
        {
            foreach (var testData in Append_MutableTextBuffer_TestData())
            {
                Test_Append_MutableTextBuffer((MutableTextBuffer)testData[0]!, (MutableTextBuffer?)testData[1], (string)testData[2]!);
            }
        }

        private static void Test_Append_MutableTextBuffer(MutableTextBuffer s1, MutableTextBuffer? s2, string s)
        {
            Assert.Equal(s, s1.Apply((sb) => sb.Append(s2)).ToString());
        }

        public IEnumerable<object?[]> Append_MutableTextBuffer_Substring_TestData()
        {
            string mediumString = new string('a', 30);
            string largeString = new string('b', 1000);

            var sb1 = MutableTextBufferFactory("Hello");
            var sb2 = MutableTextBufferFactory("one");
            var sb3 = MutableTextBufferFactory(20).Apply((sb) => sb.Append(mediumString));

            yield return new object?[] { MutableTextBufferFactory("Hello"), sb1, 0, 5, "HelloHello" };
            yield return new object?[] { MutableTextBufferFactory("Hello"), sb1, 0, 0, "Hello" };
            yield return new object?[] { MutableTextBufferFactory("Hello"), sb1, 2, 3, "Hellollo" };
            yield return new object?[] { MutableTextBufferFactory("Hello"), sb1, 2, 2, "Helloll" };
            yield return new object?[] { MutableTextBufferFactory("Hello"), sb1, 2, 0, "Hello" };
            yield return new object?[] { MutableTextBufferFactory("Hello"), MutableTextBufferFactory(), 0, 0, "Hello" };
            yield return new object?[] { MutableTextBufferFactory("Hello"), null, 0, 0, "Hello" };
            yield return new object?[] { MutableTextBufferFactory(), MutableTextBufferFactory("Hello"), 2, 3, "llo" };
            yield return new object?[] { MutableTextBufferFactory("Hello"), sb2, 0, 3, "Helloone" };

            yield return new object?[] { MutableTextBufferFactory("one"), sb3, 5, 25, "one" + new string('a', 25) };
            yield return new object?[] { MutableTextBufferFactory("one"), sb3, 5, 20, "one" + new string('a', 20) };
            yield return new object?[] { MutableTextBufferFactory("one"), sb3, 10, 10, "one" + new string('a', 10) };

            yield return new object?[] { MutableTextBufferFactory(20).Apply((sb) => sb.Append(mediumString)), sb3, 20, 10, new string('a', 40) };
            yield return new object?[] { MutableTextBufferFactory(10).Apply((sb) => sb.Append(mediumString)), sb3, 10, 10, new string('a', 40) };

            yield return new object?[] { MutableTextBufferFactory(20).Apply((sb) => sb.Append(largeString)), MutableTextBufferFactory(20).Apply((sb) => sb.Append(largeString)), 100, 50, largeString + new string('b', 50) };
            yield return new object?[] { MutableTextBufferFactory(10).Apply((sb) => sb.Append(mediumString)), MutableTextBufferFactory(20).Apply((sb) => sb.Append(largeString)), 20, 10, mediumString + new string('b', 10) };
            yield return new object?[] { MutableTextBufferFactory(10).Apply((sb) => sb.Append(mediumString)), MutableTextBufferFactory(20).Apply((sb) => sb.Append(largeString)), 100, 50, mediumString + new string('b', 50) };

            yield return new object?[] { sb1, sb1, 2, 3, "Hellollo" };
            yield return new object?[] { sb2, sb2, 2, 0, "one" };
        }

        [Fact]
        public void Append_MutableTextBuffer_Substring()
        {
            foreach (var testData in Append_MutableTextBuffer_Substring_TestData())
            {
                Test_Append_MutableTextBuffer_Substring((MutableTextBuffer)testData[0]!, (MutableTextBuffer?)testData[1], (int)testData[2]!, (int)testData[3]!, (string)testData[4]!);
            }
        }

        private static void Test_Append_MutableTextBuffer_Substring(MutableTextBuffer s1, MutableTextBuffer? s2, int startIndex, int count, string s)
        {
            Assert.Equal(s, s1.Apply((sb) => sb.Append(s2.AsSpan(startIndex, count))).ToString());
        }

        [Fact]
        public void Append_MutableTextBuffer_InvalidInput()
        {
            MutableTextBuffer sb = MutableTextBufferFactory(5, 5).Apply((sb) => sb.Append("Hello"));

            Assert.Throws<ArgumentOutOfRangeException>(() => sb.Append(sb.AsSpan(-1, 0)));
            Assert.Throws<ArgumentOutOfRangeException>(() => sb.Append(sb.AsSpan(0, -1)));
            Assert.Throws<ArgumentOutOfRangeException>(() => sb.Append(sb.AsSpan(4, 5)));

            Assert.Throws<ArgumentOutOfRangeException>(() => MutableTextBufferFactory(3, 6).Apply((sb) => sb.Append("Hello")).Apply((sb) => sb.Append(sb)));
            Assert.Throws<ArgumentOutOfRangeException>(() => MutableTextBufferFactory(3, 6).Apply((sb) => sb.Append("Hello")).Apply((sb) => sb.Append("Hello")));

            Assert.Throws<ArgumentOutOfRangeException>(() => sb.Append(sb));
        }

        public IEnumerable<object?[]> Append_StringBuilder_TestData()
        {
            string mediumString = new string('a', 30);
            string largeString = new string('b', 1000);

            var sb1 = new StringBuilder("Hello");
            var sb2 = new StringBuilder("one");
            var sb3 = new StringBuilder(20).Append(mediumString);

            yield return new object?[] { MutableTextBufferFactory("Hello"), sb1, "HelloHello" };
            yield return new object?[] { MutableTextBufferFactory("Hello"), sb2, "Helloone" };
            yield return new object?[] { MutableTextBufferFactory("Hello"), new StringBuilder(), "Hello" };

            yield return new object?[] { MutableTextBufferFactory("one"), sb3, "one" + mediumString };

            yield return new object?[] { MutableTextBufferFactory(20).Apply((sb) => sb.Append(mediumString)), sb3, mediumString + mediumString };
            yield return new object?[] { MutableTextBufferFactory(10).Apply((sb) => sb.Append(mediumString)), sb3, mediumString + mediumString };

            yield return new object?[] { MutableTextBufferFactory(20).Apply((sb) => sb.Append(largeString)), sb3, largeString + mediumString };
            yield return new object?[] { MutableTextBufferFactory(10).Apply((sb) => sb.Append(largeString)), sb3, largeString + mediumString };

            yield return new object?[] { MutableTextBufferFactory(10), sb3, mediumString };
            yield return new object?[] { MutableTextBufferFactory(30), sb3, mediumString };
            yield return new object?[] { MutableTextBufferFactory(10), new StringBuilder(20), string.Empty };

            yield return new object?[] { MutableTextBufferFactory("Hello"), null, "Hello" };
            yield return new object?[] { MutableTextBufferFactory("Hello"), sb1, "HelloHello" };
        }

        [Fact]
        public void Append_StringBuilder()
        {
            foreach (var testData in Append_StringBuilder_TestData())
            {
                Test_Append_StringBuilder((MutableTextBuffer)testData[0]!, (StringBuilder?)testData[1], (string)testData[2]!);
            }
        }

        private static void Test_Append_StringBuilder(MutableTextBuffer s1, StringBuilder? s2, string s)
        {
            Assert.Equal(s, s1.Apply((sb) => sb.Append(s2)).ToString());
        }

        public IEnumerable<object?[]> Append_StringBuilder_Substring_TestData()
        {
            string mediumString = new string('a', 30);
            string largeString = new string('b', 1000);

            var sb1 = new StringBuilder("Hello");
            var sb2 = new StringBuilder("one");
            var sb3 = new StringBuilder(20).Append(mediumString);

            yield return new object?[] { MutableTextBufferFactory("Hello"), sb1, 0, 5, "HelloHello" };
            yield return new object?[] { MutableTextBufferFactory("Hello"), sb1, 0, 0, "Hello" };
            yield return new object?[] { MutableTextBufferFactory("Hello"), sb1, 2, 3, "Hellollo" };
            yield return new object?[] { MutableTextBufferFactory("Hello"), sb1, 2, 2, "Helloll" };
            yield return new object?[] { MutableTextBufferFactory("Hello"), sb1, 2, 0, "Hello" };
            yield return new object?[] { MutableTextBufferFactory("Hello"), new StringBuilder(), 0, 0, "Hello" };
            yield return new object?[] { MutableTextBufferFactory("Hello"), null, 0, 0, "Hello" };
            yield return new object?[] { MutableTextBufferFactory(), new StringBuilder("Hello"), 2, 3, "llo" };
            yield return new object?[] { MutableTextBufferFactory("Hello"), sb2, 0, 3, "Helloone" };

            yield return new object?[] { MutableTextBufferFactory("one"), sb3, 5, 25, "one" + new string('a', 25) };
            yield return new object?[] { MutableTextBufferFactory("one"), sb3, 5, 20, "one" + new string('a', 20) };
            yield return new object?[] { MutableTextBufferFactory("one"), sb3, 10, 10, "one" + new string('a', 10) };

            yield return new object?[] { MutableTextBufferFactory(20).Apply((sb) => sb.Append(mediumString)), sb3, 20, 10, new string('a', 40) };
            yield return new object?[] { MutableTextBufferFactory(10).Apply((sb) => sb.Append(mediumString)), sb3, 10, 10, new string('a', 40) };

            yield return new object?[] { MutableTextBufferFactory(20).Apply((sb) => sb.Append(largeString)), new StringBuilder(20).Append(largeString), 100, 50, largeString + new string('b', 50) };
            yield return new object?[] { MutableTextBufferFactory(10).Apply((sb) => sb.Append(mediumString)), new StringBuilder(20).Append(largeString), 20, 10, mediumString + new string('b', 10) };
            yield return new object?[] { MutableTextBufferFactory(10).Apply((sb) => sb.Append(mediumString)), new StringBuilder(20).Append(largeString), 100, 50, mediumString + new string('b', 50) };

            yield return new object?[] { MutableTextBufferFactory("Hello"), sb1, 2, 3, "Hellollo" };
            yield return new object?[] { MutableTextBufferFactory("one"), sb2, 2, 0, "one" };
        }

        [Fact]
        public void Append_StringBuilder_Substring()
        {
            foreach (var testData in Append_StringBuilder_Substring_TestData())
            {
                Test_Append_StringBuilder_Substring((MutableTextBuffer)testData[0]!, (StringBuilder?)testData[1], (int)testData[2]!, (int)testData[3]!, (string)testData[4]!);
            }
        }

        private static void Test_Append_StringBuilder_Substring(MutableTextBuffer s1, StringBuilder? s2, int startIndex, int count, string s)
        {
            Assert.Equal(s, s1.Apply((sb) => sb.Append(s2, startIndex, count)).ToString());
        }

        [Fact]
        public void Append_StringBuilder_InvalidInput()
        {
            MutableTextBuffer mtb = MutableTextBufferFactory(5, 5).Apply((mtb) => mtb.Append("Hello"));
            StringBuilder sb = new StringBuilder(5, 5).Append("Hello");

            Assert.Throws<ArgumentOutOfRangeException>(() => mtb.Append(sb, -1, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => mtb.Append(sb, 0, -1));
            Assert.Throws<ArgumentOutOfRangeException>(() => mtb.Append(sb, 4, 5));

            Assert.Throws<ArgumentNullException>(() => mtb.Append((StringBuilder?)null, 2, 2));
            Assert.Throws<ArgumentNullException>(() => mtb.Append((StringBuilder?)null, 2, 3));
            Assert.Throws<ArgumentOutOfRangeException>(() => MutableTextBufferFactory(3, 6).Apply((sb) => sb.Append("Hello")).Apply((sb) => sb.Append(sb)));
            Assert.Throws<ArgumentOutOfRangeException>(() => MutableTextBufferFactory(3, 6).Apply((sb) => sb.Append("Hello")).Apply((sb) => sb.Append("Hello")));

            Assert.Throws<ArgumentOutOfRangeException>(() => mtb.Append(sb));
        }

        public IEnumerable<object[]> Equals_String_TestData()
        {
            string mediumString = new string('a', 30);
            string largeString = new string('a', 1000);
            string extraLargeString = new string('a', 41000); // 8000 is the maximum chunk size

            var sb1 = MutableTextBufferFactory("Hello");
            var sb2 = MutableTextBufferFactory(20).Apply((sb) => sb.Append(mediumString));
            var sb3 = MutableTextBufferFactory(20).Apply((sb) => sb.Append(largeString));
            var sb4 = MutableTextBufferFactory(20).Apply((sb) => sb.Append(extraLargeString));

            yield return new object[] { sb1, "Hello", true };
            yield return new object[] { sb1, "Hel", false };
            yield return new object[] { sb1, "Hellz", false };
            yield return new object[] { sb1, "Helloz", false };
            yield return new object[] { sb1, "", false };
            yield return new object[] { MutableTextBufferFactory(), "", true };
            yield return new object[] { MutableTextBufferFactory(), "Hello", false };
            yield return new object[] { sb2, mediumString, true };
            yield return new object[] { sb2, "H", false };
            yield return new object[] { sb3, largeString, true };
            yield return new object[] { sb3, "H", false };
            yield return new object[] { sb3, new string('a', 999) + 'b', false };
            yield return new object[] { sb4, extraLargeString, true };
            yield return new object[] { sb4, "H", false };
        }

        [Fact]
        public void Equals_String()
        {
            foreach (var testData in Equals_String_TestData())
            {
                Test_Equals_String((MutableTextBuffer)testData[0]!, (string)testData[1]!, (bool)testData[2]!);
            }
        }

        private static void Test_Equals_String(MutableTextBuffer sb1, string value, bool expected)
        {
            Assert.Equal(expected, sb1.Equals(value.AsSpan()));
        }

        [Fact]
        public void TextBuilder_ForEach()
        {
            // Test on a variety of lengths, at least up to the point of 9 8K chunks = 72K because this is where
            // we start using a different technique for creating the ChunkEnumerator.   200 * 500 = 100K which hits this.
            for (int i = 0; i < 200; i++)
            {
                TextBuilder inBuilder = new TextBuilder();
                for (int j = 0; j < i; j++)
                {
                    // Make some unique strings that are at least 500 bytes long.
                    inBuilder.Append(j);
                    inBuilder.Append("_abcdefghijklmnopqrstuvwxyz01234567890__Abcdefghijklmnopqrstuvwxyz01234567890__ABcdefghijklmnopqrstuvwxyz01_");
                    inBuilder.Append("_abcdefghijklmnopqrstuvwxyz01234567890__Abcdefghijklmnopqrstuvwxyz01234567890__ABcdefghijklmnopqrstuvwxyz0123_");
                    inBuilder.Append("_abcdefghijklmnopqrstuvwxyz01234567890__Abcdefghijklmnopqrstuvwxyz01234567890__ABcdefghijklmnopqrstuvwxyz012345_");
                    inBuilder.Append("_abcdefghijklmnopqrstuvwxyz01234567890__Abcdefghijklmnopqrstuvwxyz01234567890__ABcdefghijklmnopqrstuvwxyz012345678_");
                    inBuilder.Append("_abcdefghijklmnopqrstuvwxyz01234567890__Abcdefghijklmnopqrstuvwxyz01234567890__ABcdefghijklmnopqrstuvwxyz01234567890_");
                }

                // Copy the string out (not using MutableTextBuffer).
                string outStr = "";
                foreach (ReadOnlyMemory<char> chunk in inBuilder.GetChunks())
                    outStr += chunk.Span.ToString();

                // The strings formed by concatenating the chunks should be the same as the value in the MutableTextBuffer.
                Assert.Equal(outStr, inBuilder.ToString());
            }
        }

        [Fact]
        public void PooledTextBuilder_ForEach()
        {
            // Test on a variety of lengths, at least up to the point of 9 8K chunks = 72K because this is where
            // we start using a different technique for creating the ChunkEnumerator.   200 * 500 = 100K which hits this.
            for (int i = 0; i < 200; i++)
            {
                PooledTextBuilder inBuilder = new PooledTextBuilder();
                for (int j = 0; j < i; j++)
                {
                    // Make some unique strings that are at least 500 bytes long.
                    inBuilder.Append(j);
                    inBuilder.Append("_abcdefghijklmnopqrstuvwxyz01234567890__Abcdefghijklmnopqrstuvwxyz01234567890__ABcdefghijklmnopqrstuvwxyz01_");
                    inBuilder.Append("_abcdefghijklmnopqrstuvwxyz01234567890__Abcdefghijklmnopqrstuvwxyz01234567890__ABcdefghijklmnopqrstuvwxyz0123_");
                    inBuilder.Append("_abcdefghijklmnopqrstuvwxyz01234567890__Abcdefghijklmnopqrstuvwxyz01234567890__ABcdefghijklmnopqrstuvwxyz012345_");
                    inBuilder.Append("_abcdefghijklmnopqrstuvwxyz01234567890__Abcdefghijklmnopqrstuvwxyz01234567890__ABcdefghijklmnopqrstuvwxyz012345678_");
                    inBuilder.Append("_abcdefghijklmnopqrstuvwxyz01234567890__Abcdefghijklmnopqrstuvwxyz01234567890__ABcdefghijklmnopqrstuvwxyz01234567890_");
                }

                // Copy the string out (not using MutableTextBuffer).
                string outStr = "";
                foreach (ReadOnlyMemory<char> chunk in inBuilder.GetChunks())
                    outStr += chunk.Span.ToString();

                // The strings formed by concatenating the chunks should be the same as the value in the MutableTextBuffer.
                Assert.Equal(outStr, inBuilder.ToString());
            }
        }

        [Fact]
        public void SynchronizedTextBuilder_ForEach()
        {
            // Test on a variety of lengths, at least up to the point of 9 8K chunks = 72K because this is where
            // we start using a different technique for creating the ChunkEnumerator.   200 * 500 = 100K which hits this.
            for (int i = 0; i < 200; i++)
            {
                SynchronizedTextBuilder inBuilder = new SynchronizedTextBuilder();
                for (int j = 0; j < i; j++)
                {
                    // Make some unique strings that are at least 500 bytes long.
                    inBuilder.Append(j);
                    inBuilder.Append("_abcdefghijklmnopqrstuvwxyz01234567890__Abcdefghijklmnopqrstuvwxyz01234567890__ABcdefghijklmnopqrstuvwxyz01_");
                    inBuilder.Append("_abcdefghijklmnopqrstuvwxyz01234567890__Abcdefghijklmnopqrstuvwxyz01234567890__ABcdefghijklmnopqrstuvwxyz0123_");
                    inBuilder.Append("_abcdefghijklmnopqrstuvwxyz01234567890__Abcdefghijklmnopqrstuvwxyz01234567890__ABcdefghijklmnopqrstuvwxyz012345_");
                    inBuilder.Append("_abcdefghijklmnopqrstuvwxyz01234567890__Abcdefghijklmnopqrstuvwxyz01234567890__ABcdefghijklmnopqrstuvwxyz012345678_");
                    inBuilder.Append("_abcdefghijklmnopqrstuvwxyz01234567890__Abcdefghijklmnopqrstuvwxyz01234567890__ABcdefghijklmnopqrstuvwxyz01234567890_");
                }

                // Copy the string out (not using MutableTextBuffer).
                string outStr = "";
                foreach (ReadOnlyMemory<char> chunk in inBuilder.GetChunks())
                    outStr += chunk.Span.ToString();

                // The strings formed by concatenating the chunks should be the same as the value in the MutableTextBuffer.
                Assert.Equal(outStr, inBuilder.ToString());
            }
        }

        [Fact] // J2N specific
        public void Equals_StringBuilder_IgnoresCapacity()
        {
            var sb1 = MutableTextBufferFactory(5);
            var sb2 = new StringBuilder(10);

            Assert.True(sb1.Equals(sb2));

            sb1.Append("12345");
            sb2.Append("12345");

            Assert.True(sb1.Equals(sb2));
        }

        [Fact]
        public void Equals_MutableTextBuffer_IgnoresCapacity()
        {
            var sb1 = MutableTextBufferFactory(5);
            var sb2 = MutableTextBufferFactory(10);

            Assert.True(sb1.Equals(sb2));

            sb1.Append("12345");
            sb2.Append("12345");

            Assert.True(sb1.Equals(sb2));
        }

        [Fact] // J2N specific
        public void Equals_StringBuilder_IgnoresMaxCapacity()
        {
            var sb1 = MutableTextBufferFactory(5, 5);
            var sb2 = new StringBuilder(5, 10);

            Assert.True(sb1.Equals(sb2));

            sb1.Append("12345");
            sb2.Append("12345");

            Assert.True(sb1.Equals(sb2));
        }

        [Fact]
        public void Equals_MutableTextBuffer_IgnoresMaxCapacity()
        {
            var sb1 = MutableTextBufferFactory(5, 5);
            var sb2 = MutableTextBufferFactory(5, 10);

            Assert.True(sb1.Equals(sb2));

            sb1.Append("12345");
            sb2.Append("12345");

            Assert.True(sb1.Equals(sb2));
        }

        [Fact] // J2N specific
        public void Equals_StringBuilder_MultipleChunks()
        {
            var sb1 = MutableTextBufferFactory(5);
            var sb2 = new StringBuilder(5);

            Assert.True(sb1.Equals(sb2));

            sb1.Append("12345");
            sb2.Append("12345");

            sb1.Append("67890");
            sb2.Append("67890");

            sb1.Append("12345");
            sb2.Append("12345");

            Assert.True(sb1.Equals(sb2));
        }

        // J2N TODO: Finish
        //[ActiveIssue("https://github.com/dotnet/runtime/issues/40625")] // Hangs expanding the SB
        //[ConditionalFact(typeof(RemoteExecutor), nameof(RemoteExecutor.IsSupported))]
        //public unsafe void FailureOnLargeString()
        //{
        //    RemoteExecutor.Invoke(() => // Uses lots of memory
        //    {
        //        AssertExtensions.ThrowsAny<ArgumentOutOfRangeException, OutOfMemoryException>(() =>
        //        {
        //            MutableTextBuffer sb = MutableTextBufferFactory();
        //            sb.Append(new char[2_000_000_000]);
        //            sb.Length--;
        //            string s = new string('x', 500_000_000);
        //            sb.Append(s); // This should throw, not AV
        //        });
        //    }).Dispose();
        //}


        #region IBufferWriter<char> Tests

        [Fact]
        public void GetSpan_DataAppendedCorrectly()
        {
            var expected = new StringBuilder();
            var actual = MutableTextBufferFactory();

            for (int i = 1; i <= 1000; i++)
            {
                string s = i.ToString();

                expected.Append(s);

                Span<char> span = actual.GetSpan(s.Length);
                Assert.Equal(expected.Length - s.Length, actual.Length);

                s.AsSpan().CopyTo(span);

                actual.Advance(s.Length);
            }

            Assert.Equal(expected.Length, actual.Length);
            Assert.Equal(expected.ToString(), actual.ToString());
        }

        [Fact]
        public void GetMemory_DataAppendedCorrectly()
        {
            var expected = new StringBuilder();
            var actual = MutableTextBufferFactory();

            for (int i = 1; i <= 1000; i++)
            {
                string s = i.ToString();

                expected.Append(s);

                Memory<char> memory = actual.GetMemory(s.Length);

                s.AsSpan().CopyTo(memory.Span);

                actual.Advance(s.Length);
            }

            Assert.Equal(expected.ToString(), actual.ToString());
        }

        [Fact]
        public void GetSpan_DoesNotChangeLengthUntilAdvance()
        {
            var builder = MutableTextBufferFactory();

            builder.Append("Hello");

            int length = builder.Length;

            Span<char> span = builder.GetSpan(5);

            Assert.Equal(length, builder.Length);

            "World".AsSpan().CopyTo(span);

            Assert.Equal(length, builder.Length);

            builder.Advance(5);

            Assert.Equal(length + 5, builder.Length);
            Assert.Equal("HelloWorld", builder.ToString());
        }

        [Fact]
        public void Advance_CanAdvanceLessThanRequested()
        {
            var builder = MutableTextBufferFactory();

            Span<char> span = builder.GetSpan(32);

            "Hello".AsSpan().CopyTo(span);

            builder.Advance(5);

            Assert.Equal("Hello", builder.ToString());
        }

        [Fact]
        public void GetSpan_SizeHintZero_ReturnsNonEmptySpan()
        {
            var builder = MutableTextBufferFactory();

            Span<char> span = builder.GetSpan();

            Assert.False(span.IsEmpty);
        }

        [Fact]
        public void GetMemory_SizeHintZero_ReturnsNonEmptySpan()
        {
            var builder = MutableTextBufferFactory();

            Memory<char> memory = builder.GetMemory();

            Assert.False(memory.IsEmpty);
        }

        [Fact]
        public void GetSpan_Invalid()
        {
            var builder = MutableTextBufferFactory();

            AssertExtensions.Throws<ArgumentOutOfRangeException>(
                "sizeHint",
                () => builder.GetSpan(-1));
        }

        [Fact]
        public void GetMemory_Invalid()
        {
            var builder = MutableTextBufferFactory();

            AssertExtensions.Throws<ArgumentOutOfRangeException>(
                "sizeHint",
                () => builder.GetMemory(-1));
        }

        [Fact]
        public void Advance_Negative_Throws()
        {
            var builder = MutableTextBufferFactory();

            AssertExtensions.Throws<ArgumentOutOfRangeException>(
                "count",
                () => builder.Advance(-1));
        }

        [Fact]
        public void Advance_PastCapacity_Throws()
        {
            var builder = MutableTextBufferFactory();

            builder.GetSpan(10);

            Assert.Throws<InvalidOperationException>(
                () => builder.Advance(int.MaxValue));
        }

        [Fact]
        public void Advance_ExactlyRequested_Succeeds()
        {
            var builder = MutableTextBufferFactory();

            Span<char> span = builder.GetSpan(5);

            "Hello".AsSpan().CopyTo(span);

            builder.Advance(5);

            Assert.Equal("Hello", builder.ToString());
        }

        [Fact]
        public void GetSpan_MultipleWrites_WorkCorrectly()
        {
            var builder = MutableTextBufferFactory();

            Span<char> span = builder.GetSpan(5);
            "Hello".AsSpan().CopyTo(span);
            builder.Advance(5);

            span = builder.GetSpan(1);
            span[0] = ' ';
            builder.Advance(1);

            span = builder.GetSpan(5);
            "World".AsSpan().CopyTo(span);
            builder.Advance(5);

            Assert.Equal("Hello World", builder.ToString());
        }

        [Fact]
        public void GetSpan_DoesNotModifyExistingContents()
        {
            var builder = MutableTextBufferFactory();

            builder.Append("Hello");

            Span<char> span = builder.GetSpan(5);

            Assert.Equal("Hello", builder.ToString());

            "World".AsSpan().CopyTo(span);

            Assert.Equal("Hello", builder.ToString());

            builder.Advance(5);

            Assert.Equal("HelloWorld", builder.ToString());
        }

        #endregion


        #region InsertFromSelf Tests

        [Fact]
        public void InsertFromSelf_AppendsRange()
        {
            var buffer = MutableTextBufferFactory("abcdef");

            buffer.InsertFromSelf(6, 1, 3);

            Assert.Equal("abcdefbcd", buffer.ToString());
        }

        [Fact]
        public void InsertFromSelf_InsertAtBeginning()
        {
            var buffer = MutableTextBufferFactory("abcdef");

            buffer.InsertFromSelf(0, 2, 2);

            Assert.Equal("cdabcdef", buffer.ToString());
        }

        [Fact]
        public void InsertFromSelf_InsertInMiddle()
        {
            var buffer = MutableTextBufferFactory("abcdef");

            buffer.InsertFromSelf(3, 1, 2);

            Assert.Equal("abcbcdef", buffer.ToString());
        }

        [Fact]
        public void InsertFromSelf_OverlappingForward()
        {
            var buffer = MutableTextBufferFactory("abcdef");

            buffer.InsertFromSelf(2, 1, 3);

            Assert.Equal("abbcdcdef", buffer.ToString());
        }

        [Fact]
        public void InsertFromSelf_OverlappingBackward()
        {
            var buffer = MutableTextBufferFactory("abcdef");

            buffer.InsertFromSelf(1, 2, 3);

            Assert.Equal("acdebcdef", buffer.ToString());
        }

        [Fact]
        public void InsertFromSelf_ZeroCount_NoChange()
        {
            var buffer = MutableTextBufferFactory("abcdef");

            buffer.InsertFromSelf(2, 3, 0);

            Assert.Equal("abcdef", buffer.ToString());
        }

        [Fact]
        public void InsertFromSelf_StartAtLength_WithZeroCount_IsValid()
        {
            var buffer = MutableTextBufferFactory("abc");

            buffer.InsertFromSelf(0, 3, 0);

            Assert.Equal("abc", buffer.ToString());
        }

        [Fact]
        public void InsertFromSelf_IndexAtLength_Appends()
        {
            var buffer = MutableTextBufferFactory("abc");

            buffer.InsertFromSelf(3, 0, 2);

            Assert.Equal("abcab", buffer.ToString());
        }

        //[Fact]
        //public void InsertFromSelf_InsertEntireBuffer()
        //{
        //    var buffer = MutableTextBufferFactory("abc");

        //    buffer.InsertFromSelf(1);

        //    Assert.Equal("aabcbc", buffer.ToString());
        //}

        [Fact]
        public void InsertFromSelf_NegativeStart_Throws()
        {
            var buffer = MutableTextBufferFactory("abc");

            Assert.Throws<ArgumentOutOfRangeException>(
                () => buffer.InsertFromSelf(0, -1, 1));
        }

        [Fact]
        public void InsertFromSelf_NegativeCount_Throws()
        {
            var buffer = MutableTextBufferFactory("abc");

            Assert.Throws<ArgumentOutOfRangeException>(
                () => buffer.InsertFromSelf(0, 0, -1));
        }

        [Fact]
        public void InsertFromSelf_NegativeIndex_Throws()
        {
            var buffer = MutableTextBufferFactory("abc");

            Assert.Throws<ArgumentOutOfRangeException>(
                () => buffer.InsertFromSelf(-1, 0, 1));
        }

        [Fact]
        public void InsertFromSelf_StartPastLength_Throws()
        {
            var buffer = MutableTextBufferFactory("abc");

            Assert.Throws<ArgumentOutOfRangeException>(
                () => buffer.InsertFromSelf(0, 4, 0));
        }

        [Fact]
        public void InsertFromSelf_IndexPastLength_Throws()
        {
            var buffer = MutableTextBufferFactory("abc");

            Assert.Throws<ArgumentOutOfRangeException>(
                () => buffer.InsertFromSelf(4, 0, 1));
        }

        [Fact]
        public void InsertFromSelf_CountTooLarge_Throws()
        {
            var buffer = MutableTextBufferFactory("abc");

            Assert.Throws<ArgumentException>(
                () => buffer.InsertFromSelf(0, 1, 3));
        }

#if FEATURE_INDEX_RANGE

        [Fact]
        public void InsertFromSelf_Range_Overload_Works()
        {
            var buffer = MutableTextBufferFactory("abcdef");

            buffer.InsertFromSelf(6, 1..4);

            Assert.Equal("abcdefbcd", buffer.ToString());
        }

#endif

        #endregion InsertFromSelf Tests




        /// <summary>
        /// A custom <see cref="ICharSequence"/> implementation used for testing unknown
        /// implementations, since we generally optimize by using the underlying value of
        /// <see cref="CharArrayCharSequence"/>, <see cref="StringCharSequence"/> or 
        /// <see cref="StringBuilderCharSequence"/> rather than the interface itself.
        /// </summary>
        private sealed class MockCharSequence : ICharSequence
        {
            private readonly ReadOnlyMemory<char> value;
            public MockCharSequence(ReadOnlyMemory<char> value)
            {
                this.value = value;
            }

            public char this[int index] => value.Span[index];

            public bool HasValue => true;

            public int Length => value.Length;

            public ICharSequence Subsequence(int startIndex, int length)
            {
                return new MockCharSequence(value.Slice(startIndex, length));
            }

            public override string ToString()
            {
                return value.ToString();
            }
        }
    }
}
