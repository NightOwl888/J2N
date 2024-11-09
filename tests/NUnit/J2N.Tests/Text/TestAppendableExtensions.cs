using NUnit.Framework;
using System;
using System.Text;
#nullable enable

namespace J2N.Text
{
    public class TestAppendableExtensions
    {
        // Helper method to create a fake IAppendable implementation using StringBuilder
        private static IAppendable CreateStringBuilderAppendable()
        {
            return new StringBuilderAppendable(new StringBuilder());
        }

        // Helper method to create a fake IAppendable implementation using StringBuilder
        // that also implements ISpanAppendable
        private static ISpanAppendable CreateSpanStringBuilderAppendable()
        {
            return new SpanStringBuilderAppendable(new StringBuilder());
        }

        // Helper class to implement IAppendable using StringBuilder
        private class StringBuilderAppendable : IAppendable
        {
            private readonly StringBuilder _sb;

            public StringBuilderAppendable(StringBuilder sb)
            {
                _sb = sb;
            }

            public IAppendable Append(char value)
            {
                _sb.Append(value);
                return this;
            }

            public IAppendable Append(string? value)
            {
                _sb.Append(value);
                return this;
            }

            public IAppendable Append(string? value, int startIndex, int count)
            {
                _sb.Append(value, startIndex, count);
                return this;
            }

            public IAppendable Append(StringBuilder? value)
            {
                _sb.Append(value);
                return this;
            }

            public IAppendable Append(StringBuilder? value, int startIndex, int count)
            {
                _sb.Append(value, startIndex, count);
                return this;
            }

            public IAppendable Append(char[]? value)
            {
                _sb.Append(value);
                return this;
            }

            public IAppendable Append(char[]? value, int startIndex, int count)
            {
                _sb.Append(value, startIndex, count);
                return this;
            }

            public IAppendable Append(ICharSequence? value)
            {
                // Implementation for ICharSequence can be added if needed
                throw new NotImplementedException();
            }

            public IAppendable Append(ICharSequence? value, int startIndex, int count)
            {
                // Implementation for ICharSequence can be added if needed
                throw new NotImplementedException();
            }

            public IAppendable Append(ReadOnlySpan<char> value)
            {
                _sb.Append(value);
                return this;
            }

            public override string ToString()
            {
                return _sb.ToString();
            }
        }

        // Helper class to implement IAppendable and ISpanAppendable using StringBuilder
        private class SpanStringBuilderAppendable : StringBuilderAppendable, ISpanAppendable
        {
            public SpanStringBuilderAppendable(StringBuilder sb) : base(sb) { }

            ISpanAppendable ISpanAppendable.Append(ReadOnlySpan<char> value)
            {
                base.Append(value);
                return this;
            }
        }

        [Test]
        public void TestAppendWithIAppendable()
        {
            // Arrange
            var appendable = CreateStringBuilderAppendable();
            var inputString = "Hello, World!";

            // Act
            appendable.Append(inputString.AsSpan());

            // Assert
            Assert.AreEqual(inputString, appendable.ToString());
        }

        [Test]
        public void TestAppendWithISpanAppendable()
        {
            // Arrange
            var appendable = CreateSpanStringBuilderAppendable();
            var inputString = "Hello, World!";

            // Act
            appendable.Append(inputString.AsSpan());

            // Assert
            Assert.AreEqual(inputString, appendable.ToString());
        }

        [Test]
        public void TestAppendSliceWithIAppendable()
        {
            // Arrange
            var appendable = CreateStringBuilderAppendable();
            var inputString = "Hello, World!";
            var startIndex = 0;
            var count = 5;

            // Act
            appendable.Append(inputString.AsSpan(startIndex, count));

            // Assert
            Assert.AreEqual(inputString.Substring(startIndex, count), appendable.ToString());
        }

        [Test]
        public void TestAppendSliceWithISpanAppendable()
        {
            // Arrange
            var appendable = CreateSpanStringBuilderAppendable();
            var inputString = "Hello, World!";
            var startIndex = 0;
            var count = 5;

            // Act
            appendable.Append(inputString.AsSpan(startIndex, count));

            // Assert
            Assert.AreEqual(inputString.Substring(startIndex, count), appendable.ToString());
        }

        [Test]
        public void TestAppendLargeStringWithIAppendable()
        {
            // Arrange
            var appendable = CreateStringBuilderAppendable();
            var inputString = new string('A', 1537);

            // Act
            appendable.Append(inputString.AsSpan());

            // Assert
            Assert.AreEqual(inputString, appendable.ToString());
        }

        [Test]
        public void TestAppendLargeStringWithISpanAppendable()
        {
            // Arrange
            var appendable = CreateSpanStringBuilderAppendable();
            var inputString = new string('A', 1537);

            // Act
            appendable.Append(inputString.AsSpan());

            // Assert
            Assert.AreEqual(inputString, appendable.ToString());
        }
    }
}