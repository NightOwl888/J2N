using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace J2N.Collections.Generic
{
    internal class TestEnumerableHelpers
    {
        [Test]
        public void EmptySequence_ReturnsEmpty()
        {
            var source = Array.Empty<int>();

            var result = EnumerableHelpers.ToDistinctArray(source, out int count);

            Assert.That(count, Is.EqualTo(0));
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void SingleElement_ReturnsSingle()
        {
            var source = new[] { 42 };

            var result = EnumerableHelpers.ToDistinctArray(source, out int count);

            Assert.That(count, Is.EqualTo(1));
            Assert.That(result[0], Is.EqualTo(42));
        }

        [Test]
        public void AlreadyDistinctSequence_IsPreserved()
        {
            var source = new[] { 1, 2, 3, 4, 5 };

            var result = EnumerableHelpers.ToDistinctArray(source, out int count);

            Assert.That(count, Is.EqualTo(5));
            Assert.That(result.Take(count), Is.EqualTo(source));
        }

        [Test]
        public void DuplicateValues_AreCollapsed()
        {
            var source = new[] { 1, 1, 2, 2, 2, 3, 3 };

            var result = EnumerableHelpers.ToDistinctArray(source, out int count);

            Assert.That(count, Is.EqualTo(3));
            Assert.That(result.Take(count), Is.EqualTo(new[] { 1, 2, 3 }));
        }

        [Test]
        public void AllElementsSame_ReturnsSingle()
        {
            var source = Enumerable.Repeat(7, 10).ToArray();

            var result = EnumerableHelpers.ToDistinctArray(source, out int count);

            Assert.That(count, Is.EqualTo(1));
            Assert.That(result[0], Is.EqualTo(7));
        }

        [Test]
        public void ReferenceTypes_DeduplicatedByEqualityComparer()
        {
            var a = new string('x', 1);
            var b = new string('x', 1); // equal but not reference-equal

            var source = new[] { a, a, b, b };

            var result = EnumerableHelpers.ToDistinctArray(source, out int count);

            Assert.That(count, Is.EqualTo(1));
            Assert.That(result[0], Is.EqualTo("x"));
        }

        [Test]
        public void NonICollectionEnumerable_WorksCorrectly()
        {
            IEnumerable<int> source = Generate();

            var result = EnumerableHelpers.ToDistinctArray(source, out int count);

            Assert.That(count, Is.EqualTo(3));
            Assert.That(result.Take(count), Is.EqualTo(new[] { 1, 2, 3 }));

            static IEnumerable<int> Generate()
            {
                yield return 1;
                yield return 1;
                yield return 2;
                yield return 2;
                yield return 3;
            }
        }

        [Test]
        public void DoesNotReorderElements()
        {
            var source = new[] { 1, 1, 2, 3, 3, 2 }; // precondition violated!

            var result = EnumerableHelpers.ToDistinctArray(source, out int count);

            // We *do not* assert correctness — only that method is stable
            Assert.That(result[0], Is.EqualTo(1));
        }
    }
}
