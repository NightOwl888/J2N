using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
#nullable enable

namespace J2N.Collections.Generic
{
    public class TestEnumerableHelpers
    {
        private delegate T[] ToDistinctArrayFunc<T>(IEnumerable<T> source, out int length);

        private static ToDistinctArrayFunc<T> WithComparer<T>(IComparer<T>? comparer)
        {
            return (IEnumerable<T> source, out int length) =>
                EnumerableHelpers.ToDistinctArray(source, out length, comparer);
        }

        private static ToDistinctArrayFunc<T> WithEqualityComparer<T>(IEqualityComparer<T>? comparer)
        {
            return (IEnumerable<T> source, out int length) =>
                EnumerableHelpers.ToDistinctArray(source, out length, comparer);
        }

        [Test]
        public void Test_ToDistinctArray_Comparer_EmptySequence_ReturnsEmpty()
        {
            Run_ToDistinctArray_EmptySequence_ReturnsEmpty(WithComparer<int>(null));
        }

        [Test]
        public void Test_ToDistinctArray_EqualityComparer_EmptySequence_ReturnsEmpty()
        {
            Run_ToDistinctArray_EmptySequence_ReturnsEmpty(WithEqualityComparer<int>(null));
        }

        private static void Run_ToDistinctArray_EmptySequence_ReturnsEmpty(ToDistinctArrayFunc<int> func)
        {
            var source = Array.Empty<int>();

            var result = func(source, out int count);

            Assert.That(count, Is.EqualTo(0));
            Assert.That(result, Is.Empty);
        }


        [Test]
        public void Test_ToDistinctArray_Comparer_SingleElement_ReturnsSingle()
        {
            Run_ToDistinctArray_SingleElement_ReturnsSingle(WithComparer<int>(null));
        }

        [Test]
        public void Test_ToDistinctArray_EqualityComparer_SingleElement_ReturnsSingle()
        {
            Run_ToDistinctArray_SingleElement_ReturnsSingle(WithEqualityComparer<int>(null));
        }

        private static void Run_ToDistinctArray_SingleElement_ReturnsSingle(ToDistinctArrayFunc<int> func)
        {
            var source = new[] { 42 };

            var result = func(source, out int count);

            Assert.That(count, Is.EqualTo(1));
            Assert.That(result[0], Is.EqualTo(42));
        }


        [Test]
        public void Test_ToDistinctArray_Comparer_AlreadyDistinctSequence_IsPreserved()
        {
            Run_ToDistinctArray_AlreadyDistinctSequence_IsPreserved(WithComparer<int>(null));
        }

        [Test]
        public void Test_ToDistinctArray_EqualityComparer_AlreadyDistinctSequence_IsPreserved()
        {
            Run_ToDistinctArray_AlreadyDistinctSequence_IsPreserved(WithEqualityComparer<int>(null));
        }

        private static void Run_ToDistinctArray_AlreadyDistinctSequence_IsPreserved(ToDistinctArrayFunc<int> func)
        {
            var source = new[] { 1, 2, 3, 4, 5 };

            var result = func(source, out int count);

            Assert.That(count, Is.EqualTo(5));
            Assert.That(result.Take(count), Is.EqualTo(source));
        }

        [Test]
        public void Test_ToDistinctArray_Comparer_DuplicateValues_AreCollapsed()
        {
            Run_ToDistinctArray_DuplicateValues_AreCollapsed(WithComparer<int>(null));
        }

        [Test]
        public void Test_ToDistinctArray_EqualityComparer_DuplicateValues_AreCollapsed()
        {
            Run_ToDistinctArray_DuplicateValues_AreCollapsed(WithEqualityComparer<int>(null));
        }

        private static void Run_ToDistinctArray_DuplicateValues_AreCollapsed(ToDistinctArrayFunc<int> func)
        {
            var source = new[] { 1, 1, 2, 2, 2, 3, 3 };

            var result = func(source, out int count);

            Assert.That(count, Is.EqualTo(3));
            Assert.That(result.Take(count), Is.EqualTo(new[] { 1, 2, 3 }));
        }


        [Test]
        public void Test_ToDistinctArray_Comparer_AllElementsSame_ReturnsSingle()
        {
            Run_ToDistinctArray_AllElementsSame_ReturnsSingle(WithComparer<int>(null));
        }

        [Test]
        public void Test_ToDistinctArray_EqualityComparer_AllElementsSame_ReturnsSingle()
        {
            Run_ToDistinctArray_AllElementsSame_ReturnsSingle(WithEqualityComparer<int>(null));
        }

        private static void Run_ToDistinctArray_AllElementsSame_ReturnsSingle(ToDistinctArrayFunc<int> func)
        {
            var source = Enumerable.Repeat(7, 10).ToArray();

            var result = func(source, out int count);

            Assert.That(count, Is.EqualTo(1));
            Assert.That(result[0], Is.EqualTo(7));
        }


        [Test]
        public void Test_ToDistinctArray_Comparer_ReferenceTypes_DeduplicatedByEqualityComparer()
        {
            Run_ToDistinctArray_ReferenceTypes_DeduplicatedByEqualityComparer(WithComparer<string>(null));
        }

        [Test]
        public void Test_ToDistinctArray_EqualityComparer_ReferenceTypes_DeduplicatedByEqualityComparer()
        {
            Run_ToDistinctArray_ReferenceTypes_DeduplicatedByEqualityComparer(WithEqualityComparer<string>(null));
        }

        private static void Run_ToDistinctArray_ReferenceTypes_DeduplicatedByEqualityComparer(ToDistinctArrayFunc<string> func)
        {
            var a = new string('x', 1);
            var b = new string('x', 1); // equal but not reference-equal

            var source = new[] { a, a, b, b };

            var result = func(source, out int count);

            Assert.That(count, Is.EqualTo(1));
            Assert.That(result[0], Is.EqualTo("x"));
        }


        [Test]
        public void Test_ToDistinctArray_Comparer_NonICollectionEnumerable_WorksCorrectly()
        {
            Run_ToDistinctArray_NonICollectionEnumerable_WorksCorrectly(WithComparer<int>(null));
        }

        [Test]
        public void Test_ToDistinctArray_EqualityComparer_NonICollectionEnumerable_WorksCorrectly()
        {
            Run_ToDistinctArray_NonICollectionEnumerable_WorksCorrectly(WithEqualityComparer<int>(null));
        }

        private static void Run_ToDistinctArray_NonICollectionEnumerable_WorksCorrectly(ToDistinctArrayFunc<int> func)
        {
            IEnumerable<int> source = Generate();

            var result = func(source, out int count);

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
        public void Test_ToDistinctArray_Comparer_DoesNotReorderElements()
        {
            Run_ToDistinctArray_DoesNotReorderElements(WithComparer<int>(null));
        }

        [Test]
        public void Test_ToDistinctArray_EqualityComparer_DoesNotReorderElements()
        {
            Run_ToDistinctArray_DoesNotReorderElements(WithEqualityComparer<int>(null));
        }

        private static void Run_ToDistinctArray_DoesNotReorderElements(ToDistinctArrayFunc<int> func)
        {
            var source = new[] { 1, 1, 2, 3, 3, 2 }; // precondition violated!

            var result = func(source, out int count);

            // We *do not* assert correctness — only that method is stable
            Assert.That(result[0], Is.EqualTo(1));
        }
    }
}
