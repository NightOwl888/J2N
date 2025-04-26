// Adapted from: https://github.com/SimonCropp/Polyfill/blob/7.27.0/src/Tests/PolyfillTests_Encoding.cs

// MIT License

// Copyright (c) Simon Cropp

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using NUnit.Framework;
using System;
using System.Linq;
using System.Text;

namespace J2N.Text
{
    [TestFixture]
    public class TestEncodingExtensions
    {
        [Test]
        public void TestGetByteCount()
        {
            var encoding = Encoding.UTF8;
            var chars = "Hello, World!".AsSpan();

            var byteCount = EncodingExtensions.GetByteCount(encoding, chars);
            Assert.AreEqual(13, byteCount);
        }

        [Test]
        public void TestGetCharCount()
        {
            var encoding = Encoding.UTF8;
            var utf8Bytes = "Hello, World!"u8.ToArray();
            var byteSpan = new ReadOnlySpan<byte>(utf8Bytes);

            var charCount = EncodingExtensions.GetCharCount(encoding, byteSpan);
            Assert.AreEqual(13, charCount);
        }

        [Test]
        public void TestGetChars()
        {
            // Arrange
            var encoding = Encoding.UTF8;
            var utf8Bytes = "Hello, World!"u8.ToArray();
            var byteSpan = new ReadOnlySpan<byte>(utf8Bytes);
            Span<char> charSpan = stackalloc char[utf8Bytes.Length];

            // Act
            var charCount = EncodingExtensions.GetChars(encoding, byteSpan, charSpan);

            // Assert
            var result = charSpan.Slice(0, charCount).ToString();
            Assert.AreEqual("Hello, World!", result);
        }

        [Test]
        public void TestGetString()
        {
            var array = (ReadOnlySpan<byte>)"value"u8.ToArray().AsSpan();
            var result = EncodingExtensions.GetString(Encoding.UTF8, array);
            Assert.AreEqual("value", result);
        }

        [Test]
        public void TestGetString_Long()
        {
            const string testString = "Café naïve façade über schöne Straße — 🧠🌍💡✨🎉📚🔬🎨🧬🦄🚀🐍🌈🎵⚛️👾";
            var array = (ReadOnlySpan<byte>)Encoding.UTF8.GetBytes(testString).AsSpan();
            var result = EncodingExtensions.GetString(Encoding.UTF8, array);
            Assert.AreEqual(testString, result);
        }

        [Test]
        public void TestGetBytes()
        {
            var encoding = Encoding.UTF8;
            var chars = "Hello, World!".AsSpan();
            Span<byte> bytes = stackalloc byte[encoding.GetByteCount(chars)];

            var byteCount = EncodingExtensions.GetBytes(encoding, chars, bytes);

            Assert.AreEqual(encoding.GetByteCount(chars), byteCount);
            Assert.AreEqual(encoding.GetBytes("Hello, World!"), bytes.ToArray());
        }

        [Test]
        public void TestGetBytes_Char()
        {
            var encoding = Encoding.UTF8;
            var chars = "Hello, World!".AsSpan();
            Span<byte> buffer = stackalloc byte[4];
            var expectedBytes = encoding.GetBytes("Hello, World!");
            int byteIndex = 0;

            foreach (char ch in chars)
            {
                ReadOnlySpan<byte> bytes = EncodingExtensions.GetBytes(encoding, ch, buffer);
                int byteLength = bytes.Length;
                Assert.IsTrue(expectedBytes.AsSpan(byteIndex, byteLength).SequenceEqual(bytes));
                byteIndex += byteLength;
            }
        }

        [Test]
        public void TestGetBytes_Codepoint()
        {
            const string testString = "Café naïve façade über schöne Straße — 🧠🌍💡✨🎉📚🔬🎨🧬🦄🚀🐍🌈🎵⚛️👾";
            var encoding = Encoding.UTF8;
            var chars = testString.AsSpan();
            Span<byte> buffer = stackalloc byte[8];
            var expectedBytes = encoding.GetBytes(testString);
            int codePointIndex = 0;
            int byteIndex = 0;

            while (codePointIndex < chars.Length)
            {
                int codePoint = Character.CodePointAt(chars, codePointIndex);
                int charCount = Character.CharCount(codePoint);

                ReadOnlySpan<byte> bytes = EncodingExtensions.GetBytes(encoding, codePoint, buffer);
                int byteLength = bytes.Length;
                Assert.IsTrue(expectedBytes.AsSpan(byteIndex, byteLength).SequenceEqual(bytes));
                byteIndex += byteLength;

                codePointIndex += charCount;
            }
        }

        [Test]
        public void Test_TryGetBytes_WithValidInput_ReturnsTrue()
        {
            // Arrange
            var encoding = Encoding.UTF8;
            var inputString = "Hello, World!";
            var charSpan = inputString.AsSpan();
            Span<byte> bytes = stackalloc byte[encoding.GetMaxByteCount(inputString.Length)];

            // Act
            var result = EncodingExtensions.TryGetBytes(encoding, charSpan, bytes, out var bytesWritten);

            // Assert
            Assert.IsTrue(result);
            var expectedBytes = encoding.GetBytes(inputString);
            Assert.IsTrue(expectedBytes.AsSpan().SequenceEqual(bytes.Slice(0, bytesWritten)));
        }

        [Test]
        public void Test_TryGetBytes_WithSmallDestination_ReturnsFalse()
        {
            // Arrange
            var encoding = Encoding.UTF8;
            var inputString = "Hello, World!";
            var charSpan = inputString.AsSpan();
            Span<byte> byteSpan = stackalloc byte[5]; // Intentionally too small

            // Act
            var result = EncodingExtensions.TryGetBytes(encoding, charSpan, byteSpan, out var bytesWritten);

            // Assert
            Assert.IsFalse(result);
            Assert.AreEqual(0, bytesWritten);
        }


        [Test]
        public void Test_TryGetChars_WithValidInput_ReturnsTrue()
        {
            // Arrange
            var encoding = Encoding.UTF8;
            var utf8Bytes = "Hello, World!"u8.ToArray();
            var byteSpan = new ReadOnlySpan<byte>(utf8Bytes);
            var charArray = new char[utf8Bytes.Length];
            var charSpan = new Span<char>(charArray);

            // Act
            var result = EncodingExtensions.TryGetChars(encoding, byteSpan, charSpan, out var charsWritten);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual("Hello, World!", charSpan.Slice(0, charsWritten).ToString());
        }

        [Test]
        public void Test_TryGetChars_WithSmallDestination_ReturnsFalse()
        {
            // Arrange
            var encoding = Encoding.UTF8;
            var utf8Bytes = "Hello, World!"u8.ToArray();
            var byteSpan = new ReadOnlySpan<byte>(utf8Bytes);
            var charArray = new char[5]; // Smaller than needed
            var charSpan = new Span<char>(charArray);

            // Act
            var result = EncodingExtensions.TryGetChars(encoding, byteSpan, charSpan, out var charsWritten);

            // Assert
            Assert.IsFalse(result);
            Assert.AreEqual(0, charsWritten);
        }
    }
}
