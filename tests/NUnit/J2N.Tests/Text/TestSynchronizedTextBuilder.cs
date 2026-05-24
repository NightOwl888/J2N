using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace J2N.Text
{
    public partial class TestSynchronizedTextBuilder
    {
        #region IBufferWriter<char> Tests

        [Test]
        public void GetSpan_ShouldProvideWritableBuffer()
        {
            var buffer = new SynchronizedTextBuilder();

            Span<char> span = buffer.GetSpan(5);

            "Hello".AsSpan().CopyTo(span);

            buffer.Advance(5);

            Assert.That(buffer.ToString(), Is.EqualTo("Hello"));
        }

        [Test]
        public void GetSpan_ShouldGrowBuffer()
        {
            var buffer = new SynchronizedTextBuilder(4);

            Span<char> span = buffer.GetSpan(100);

            Assert.That(span.Length, Is.GreaterThanOrEqualTo(100));
        }

        [Test]
        public void Advance_ShouldMovePosition()
        {
            var buffer = new SynchronizedTextBuilder();

            Span<char> span = buffer.GetSpan(3);

            span[0] = 'A';
            span[1] = 'B';
            span[2] = 'C';

            buffer.Advance(3);

            Assert.That(buffer.Length, Is.EqualTo(3));
            Assert.That(buffer.ToString(), Is.EqualTo("ABC"));
        }

        [Test]
        public void GetSpan_ZeroHint_ShouldReturnNonEmptyBuffer()
        {
            var buffer = new SynchronizedTextBuilder();

            Span<char> span = buffer.GetSpan();

            Assert.That(span.Length, Is.GreaterThan(0));
        }

        [Test]
        public void Advance_PastCapacity_ShouldThrow()
        {
            var buffer = new SynchronizedTextBuilder(4);

            Assert.Throws<InvalidOperationException>(() =>
            {
                buffer.Advance(5);
            });
        }

        [Test]
        public void MultipleWrites_ShouldAppendCorrectly()
        {
            var buffer = new SynchronizedTextBuilder();

            {
                Span<char> span = buffer.GetSpan(5);
                "Hello".AsSpan().CopyTo(span);
                buffer.Advance(5);
            }

            {
                Span<char> span = buffer.GetSpan(6);
                " World".AsSpan().CopyTo(span);
                buffer.Advance(6);
            }

            Assert.That(buffer.ToString(), Is.EqualTo("Hello World"));
        }

        [Test]
        public void Append_And_IBufferWriter_ShouldInteroperate()
        {
            var buffer = new SynchronizedTextBuilder();

            buffer.Append("Hello");

            Span<char> span = buffer.GetSpan(6);

            " World".AsSpan().CopyTo(span);

            buffer.Advance(6);

            Assert.That(buffer.ToString(), Is.EqualTo("Hello World"));
        }

        #endregion IBufferWriter<char> Tests
    }
}
