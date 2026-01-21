// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using J2N.Collections.Generic;
using System;
using System.Linq;
using Xunit;
using SCG = System.Collections.Generic;

namespace J2N.Collections.Tests
{
    /// <summary>
    /// Contains tests that ensure the correctness of the List class.
    /// </summary>
    public abstract partial class List_Generic_Tests<T> : IList_Generic_Tests<T>
    {
        #region Helpers

        public delegate int IndexOfDelegate(List<T> list, T value);
        public enum IndexOfMethod
        {
            IndexOf_T,
            IndexOf_T_int,
            IndexOf_T_int_int,
            LastIndexOf_T,
            LastIndexOf_T_int,
            LastIndexOf_T_int_int,
        };

        private IndexOfDelegate IndexOfDelegateFromType(IndexOfMethod methodType)
        {
            switch (methodType)
            {
                case (IndexOfMethod.IndexOf_T):
                    return ((List<T> list, T value) => { return list.IndexOf(value); });
                case (IndexOfMethod.IndexOf_T_int):
                    return ((List<T> list, T value) => { return list.IndexOf(value, 0); });
                case (IndexOfMethod.IndexOf_T_int_int):
                    return ((List<T> list, T value) => { return list.IndexOf(value, 0, list.Count); });
                case (IndexOfMethod.LastIndexOf_T):
                    return ((List<T> list, T value) => { return list.LastIndexOf(value); });
                case (IndexOfMethod.LastIndexOf_T_int):
                    return ((List<T> list, T value) => { return list.LastIndexOf(value, list.Count - 1); });
                case (IndexOfMethod.LastIndexOf_T_int_int):
                    return ((List<T> list, T value) => { return list.LastIndexOf(value, list.Count - 1, list.Count); });
                default:
                    throw new Exception("Invalid IndexOfMethod");
            }
        }

        /// <summary>
        /// MemberData for a Theory to test the IndexOf methods for List. To avoid high code reuse of tests for the 6 IndexOf
        /// methods in List, delegates are used to cover the basic behavioral cases shared by all IndexOf methods. A bool
        /// is used to specify the ordering (front-to-back or back-to-front (e.g. LastIndexOf)) that the IndexOf method
        /// searches in.
        /// </summary>
        public static SCG.IEnumerable<object[]> IndexOfTestData()
        {
            foreach (object[] sizes in ValidCollectionSizes())
            {
                int count = (int)sizes[0];
                yield return new object[] { IndexOfMethod.IndexOf_T, count, true };
                yield return new object[] { IndexOfMethod.LastIndexOf_T, count, false };

                if (count > 0) // 0 is an invalid index for IndexOf when the count is 0.
                {
                    yield return new object[] { IndexOfMethod.IndexOf_T_int, count, true };
                    yield return new object[] { IndexOfMethod.LastIndexOf_T_int, count, false };
                    yield return new object[] { IndexOfMethod.IndexOf_T_int_int, count, true };
                    yield return new object[] { IndexOfMethod.LastIndexOf_T_int_int, count, false };
                }
            }
        }

        #endregion

        #region IndexOf

        [Theory]
        [MemberData(nameof(IndexOfTestData))]
        public void IndexOf_NoDuplicates(IndexOfMethod indexOfMethod, int count, bool frontToBackOrder)
        {
            _ = frontToBackOrder;
            List<T> list = GenericListFactory(count);
            List<T> expectedList = list.ToList();
            IndexOfDelegate IndexOf = IndexOfDelegateFromType(indexOfMethod);

            Assert.All(Enumerable.Range(0, count), i =>
            {
                Assert.Equal(i, IndexOf(list, expectedList[i]));
            });
        }

        [Theory]
        [MemberData(nameof(IndexOfTestData))]
        public void IndexOf_NonExistingValues(IndexOfMethod indexOfMethod, int count, bool frontToBackOrder)
        {
            _ = frontToBackOrder;
            List<T> list = GenericListFactory(count);
            SCG.IEnumerable<T> nonexistentValues = CreateEnumerable(EnumerableType.List, list, count: count, numberOfMatchingElements: 0, numberOfDuplicateElements: 0);
            IndexOfDelegate IndexOf = IndexOfDelegateFromType(indexOfMethod);

            Assert.All(nonexistentValues, nonexistentValue =>
            {
                Assert.Equal(-1, IndexOf(list, nonexistentValue));
            });
        }

        [Theory]
        [MemberData(nameof(IndexOfTestData))]
        public void IndexOf_DefaultValue(IndexOfMethod indexOfMethod, int count, bool frontToBackOrder)
        {
            _ = frontToBackOrder;
            T defaultValue = default;
            List<T> list = GenericListFactory(count);
            IndexOfDelegate IndexOf = IndexOfDelegateFromType(indexOfMethod);
            while (list.Remove(defaultValue))
                count--;
            list.Add(defaultValue);
            Assert.Equal(count, IndexOf(list, defaultValue));
        }

        [Theory]
        [MemberData(nameof(IndexOfTestData))]
        public void IndexOf_OrderIsCorrect(IndexOfMethod indexOfMethod, int count, bool frontToBackOrder)
        {
            List<T> list = GenericListFactory(count);
            List<T> withoutDuplicates = list.ToList();
            list.AddRange(list);
            IndexOfDelegate IndexOf = IndexOfDelegateFromType(indexOfMethod);

            Assert.All(Enumerable.Range(0, count), i =>
            {
                if (frontToBackOrder)
                    Assert.Equal(i, IndexOf(list, withoutDuplicates[i]));
                else
                    Assert.Equal(count + i, IndexOf(list, withoutDuplicates[i]));
            });
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void IndexOf_int_OrderIsCorrectWithManyDuplicates(int count)
        {
            List<T> list = GenericListFactory(count);
            List<T> withoutDuplicates = list.ToList();
            list.AddRange(list);
            list.AddRange(list);
            list.AddRange(list);

            Assert.All(Enumerable.Range(0, count), i =>
            {
                Assert.All(Enumerable.Range(0, 4), j =>
                {
                    int expectedIndex = (j * count) + i;
                    Assert.Equal(expectedIndex, list.IndexOf(withoutDuplicates[i], (count * j)));
                    Assert.Equal(expectedIndex, list.IndexOf(withoutDuplicates[i], (count * j), count));
                });
            });
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void LastIndexOf_int_OrderIsCorrectWithManyDuplicates(int count)
        {
            List<T> list = GenericListFactory(count);
            List<T> withoutDuplicates = list.ToList();
            list.AddRange(list);
            list.AddRange(list);
            list.AddRange(list);

            Assert.All(Enumerable.Range(0, count), i =>
            {
                Assert.All(Enumerable.Range(0, 4), j =>
                {
                    int expectedIndex = (j * count) + i;
                    Assert.Equal(expectedIndex, list.LastIndexOf(withoutDuplicates[i], (count * (j + 1)) - 1));
                    Assert.Equal(expectedIndex, list.LastIndexOf(withoutDuplicates[i], (count * (j + 1)) - 1, count));
                });
            });
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void IndexOf_int_OutOfRangeExceptions(int count)
        {
            List<T> list = GenericListFactory(count);
            T element = CreateT(234);
            Assert.Throws<ArgumentOutOfRangeException>(() => list.IndexOf(element, count + 1)); //"Expect ArgumentOutOfRangeException for index greater than length of list.."
            Assert.Throws<ArgumentOutOfRangeException>(() => list.IndexOf(element, count + 10)); //"Expect ArgumentOutOfRangeException for index greater than length of list.."
            Assert.Throws<ArgumentOutOfRangeException>(() => list.IndexOf(element, -1)); //"Expect ArgumentOutOfRangeException for negative index."
            Assert.Throws<ArgumentOutOfRangeException>(() => list.IndexOf(element, int.MinValue)); //"Expect ArgumentOutOfRangeException for negative index."
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void IndexOf_int_int_OutOfRangeExceptions(int count)
        {
            List<T> list = GenericListFactory(count);
            T element = CreateT(234);
            Assert.Throws<ArgumentOutOfRangeException>(() => list.IndexOf(element, count, 1)); //"ArgumentOutOfRangeException expected on index larger than array."
            Assert.Throws<ArgumentOutOfRangeException>(() => list.IndexOf(element, count + 1, 1)); //"ArgumentOutOfRangeException expected  on index larger than array."
            Assert.Throws<ArgumentOutOfRangeException>(() => list.IndexOf(element, 0, count + 1)); //"ArgumentOutOfRangeException expected  on count larger than array."
            Assert.Throws<ArgumentOutOfRangeException>(() => list.IndexOf(element, count / 2, count / 2 + 2)); //"ArgumentOutOfRangeException expected.."
            Assert.Throws<ArgumentOutOfRangeException>(() => list.IndexOf(element, 0, count + 1)); //"ArgumentOutOfRangeException expected  on count larger than array."
            Assert.Throws<ArgumentOutOfRangeException>(() => list.IndexOf(element, 0, -1)); //"ArgumentOutOfRangeException expected on negative count."
            Assert.Throws<ArgumentOutOfRangeException>(() => list.IndexOf(element, -1, 1)); //"ArgumentOutOfRangeException expected on negative index."
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void LastIndexOf_int_OutOfRangeExceptions(int count)
        {
            List<T> list = GenericListFactory(count);
            T element = CreateT(234);
            Assert.Throws<ArgumentOutOfRangeException>(() => list.LastIndexOf(element, count)); //"ArgumentOutOfRangeException expected."
            if (count == 0)  // IndexOf with a 0 count List is special cased to return -1.
                Assert.Equal(-1, list.LastIndexOf(element, -1));
            else
                Assert.Throws<ArgumentOutOfRangeException>(() => list.LastIndexOf(element, -1));
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void LastIndexOf_int_int_OutOfRangeExceptions(int count)
        {
            List<T> list = GenericListFactory(count);
            T element = CreateT(234);

            if (count > 0)
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => list.LastIndexOf(element, 0, count + 1)); //"Expected ArgumentOutOfRangeException."
                Assert.Throws<ArgumentOutOfRangeException>(() => list.LastIndexOf(element, count / 2, count / 2 + 2)); //"Expected ArgumentOutOfRangeException."
                Assert.Throws<ArgumentOutOfRangeException>(() => list.LastIndexOf(element, 0, count + 1)); //"Expected ArgumentOutOfRangeException."
                Assert.Throws<ArgumentOutOfRangeException>(() => list.LastIndexOf(element, 0, -1)); //"Expected ArgumentOutOfRangeException."
                Assert.Throws<ArgumentOutOfRangeException>(() => list.LastIndexOf(element, -1, count)); //"Expected ArgumentOutOfRangeException."
                Assert.Throws<ArgumentOutOfRangeException>(() => list.LastIndexOf(element, -1, 1)); //"Expected ArgumentOutOfRangeException."                Assert.Throws<ArgumentOutOfRangeException>(() => list.LastIndexOf(element, count, 0)); //"Expected ArgumentOutOfRangeException."
                Assert.Throws<ArgumentOutOfRangeException>(() => list.LastIndexOf(element, count, 1)); //"Expected ArgumentOutOfRangeException."
            }
            else // IndexOf with a 0 count List is special cased to return -1.
            {
                Assert.Equal(-1, list.LastIndexOf(element, 0, count + 1));
                Assert.Equal(-1, list.LastIndexOf(element, count / 2, count / 2 + 2));
                Assert.Equal(-1, list.LastIndexOf(element, 0, count + 1));
                Assert.Equal(-1, list.LastIndexOf(element, 0, -1));
                Assert.Equal(-1, list.LastIndexOf(element, -1, count));
                Assert.Equal(-1, list.LastIndexOf(element, -1, 1));
                Assert.Equal(-1, list.LastIndexOf(element, count, 0));
                Assert.Equal(-1, list.LastIndexOf(element, count, 1));
            }
        }

        #endregion

        #region Float and Double NaN and Signed Zero Tests

        // These tests ensure that float and double IndexOf/LastIndexOf methods match Java's behavior
        // regarding NaN and signed zero comparisons.

        /// <summary>
        /// Tests IndexOf with float NaN values. In Java/J2N, NaN == NaN, unlike .NET's BCL.
        /// </summary>
        [Fact]
        public void IndexOf_FloatNaN()
        {
            if (typeof(T) != typeof(float))
                return;

            // Test with different NaN bit patterns
            float nanDefault = float.NaN; // 0x7FC00000
            float nanVariant1 = BitConversion.Int32BitsToSingle(0x7FC00001);
            float nanVariant2 = BitConversion.Int32BitsToSingle(unchecked((int)0xFFC00001));

            List<float> list = (List<float>)(object)new List<T> { (T)(object)1.0f, (T)(object)nanDefault, (T)(object)3.0f };

            // NaN should be found at index 1 regardless of NaN bit pattern
            Assert.Equal(1, list.IndexOf(nanDefault));
            Assert.Equal(1, list.IndexOf(nanVariant1));
            Assert.Equal(1, list.IndexOf(nanVariant2));

            Assert.Equal(1, list.IndexOf(nanDefault, 0));
            Assert.Equal(1, list.IndexOf(nanVariant1, 0));
            Assert.Equal(1, list.IndexOf(nanVariant2, 0));

            Assert.Equal(1, list.IndexOf(nanDefault, 0, 3));
            Assert.Equal(1, list.IndexOf(nanVariant1, 0, 3));
            Assert.Equal(1, list.IndexOf(nanVariant2, 0, 3));

            // NaN at different positions with multiple NaN bit patterns
            var list2 = new List<float> { nanVariant1, 1.0f, nanDefault, 3.0f, nanVariant2 };
            Assert.Equal(0, list2.IndexOf(nanDefault));
            Assert.Equal(0, list2.IndexOf(nanVariant1));
            Assert.Equal(0, list2.IndexOf(nanVariant2));

            Assert.Equal(2, list2.IndexOf(nanDefault, 1));
            Assert.Equal(2, list2.IndexOf(nanVariant1, 1));
            Assert.Equal(2, list2.IndexOf(nanVariant2, 1));

            Assert.Equal(2, list2.IndexOf(nanDefault, 2, 3));
            Assert.Equal(2, list2.IndexOf(nanVariant1, 2, 3));
            Assert.Equal(2, list2.IndexOf(nanVariant2, 2, 3));
        }

        /// <summary>
        /// Tests IndexOf with double NaN values. In Java/J2N, NaN == NaN, unlike .NET's BCL.
        /// </summary>
        [Fact]
        public void IndexOf_DoubleNaN()
        {
            if (typeof(T) != typeof(double))
                return;

            // Test with different NaN bit patterns
            double nanDefault = double.NaN; // 0x7FF8000000000000
            double nanVariant1 = BitConversion.Int64BitsToDouble(0x7FF8000000000001);
            double nanVariant2 = BitConversion.Int64BitsToDouble(unchecked((long)0xFFF8000000000001));

            List<double> list = (List<double>)(object)new List<T> { (T)(object)1.0d, (T)(object)nanDefault, (T)(object)3.0d };

            // NaN should be found at index 1 regardless of NaN bit pattern
            Assert.Equal(1, list.IndexOf(nanDefault));
            Assert.Equal(1, list.IndexOf(nanVariant1));
            Assert.Equal(1, list.IndexOf(nanVariant2));

            Assert.Equal(1, list.IndexOf(nanDefault, 0));
            Assert.Equal(1, list.IndexOf(nanVariant1, 0));
            Assert.Equal(1, list.IndexOf(nanVariant2, 0));

            Assert.Equal(1, list.IndexOf(nanDefault, 0, 3));
            Assert.Equal(1, list.IndexOf(nanVariant1, 0, 3));
            Assert.Equal(1, list.IndexOf(nanVariant2, 0, 3));

            // NaN at different positions with multiple NaN bit patterns
            var list2 = new List<double> { nanVariant1, 1.0d, nanDefault, 3.0d, nanVariant2 };
            Assert.Equal(0, list2.IndexOf(nanDefault));
            Assert.Equal(0, list2.IndexOf(nanVariant1));
            Assert.Equal(0, list2.IndexOf(nanVariant2));

            Assert.Equal(2, list2.IndexOf(nanDefault, 1));
            Assert.Equal(2, list2.IndexOf(nanVariant1, 1));
            Assert.Equal(2, list2.IndexOf(nanVariant2, 1));

            Assert.Equal(2, list2.IndexOf(nanDefault, 2, 3));
            Assert.Equal(2, list2.IndexOf(nanVariant1, 2, 3));
            Assert.Equal(2, list2.IndexOf(nanVariant2, 2, 3));
        }

        /// <summary>
        /// Tests LastIndexOf with float NaN values. In Java/J2N, NaN == NaN, unlike .NET's BCL.
        /// </summary>
        [Fact]
        public void LastIndexOf_FloatNaN()
        {
            if (typeof(T) != typeof(float))
                return;

            // Test with different NaN bit patterns
            float nanDefault = float.NaN; // 0x7FC00000
            float nanVariant1 = BitConversion.Int32BitsToSingle(0x7FC00001);
            float nanVariant2 = BitConversion.Int32BitsToSingle(unchecked((int)0xFFC00001));

            List<float> list = (List<float>)(object)new List<T> { (T)(object)1.0f, (T)(object)nanDefault, (T)(object)3.0f };

            // NaN should be found at index 1 regardless of NaN bit pattern
            Assert.Equal(1, list.LastIndexOf(nanDefault));
            Assert.Equal(1, list.LastIndexOf(nanVariant1));
            Assert.Equal(1, list.LastIndexOf(nanVariant2));

            Assert.Equal(1, list.LastIndexOf(nanDefault, 2));
            Assert.Equal(1, list.LastIndexOf(nanVariant1, 2));
            Assert.Equal(1, list.LastIndexOf(nanVariant2, 2));

            Assert.Equal(1, list.LastIndexOf(nanDefault, 2, 3));
            Assert.Equal(1, list.LastIndexOf(nanVariant1, 2, 3));
            Assert.Equal(1, list.LastIndexOf(nanVariant2, 2, 3));

            // NaN at different positions with multiple NaN bit patterns
            var list2 = new List<float> { nanVariant1, 1.0f, nanDefault, 3.0f, nanVariant2 };
            Assert.Equal(4, list2.LastIndexOf(nanDefault));
            Assert.Equal(4, list2.LastIndexOf(nanVariant1));
            Assert.Equal(4, list2.LastIndexOf(nanVariant2));

            Assert.Equal(2, list2.LastIndexOf(nanDefault, 2));
            Assert.Equal(2, list2.LastIndexOf(nanVariant1, 2));
            Assert.Equal(2, list2.LastIndexOf(nanVariant2, 2));

            Assert.Equal(0, list2.LastIndexOf(nanDefault, 1, 2));
            Assert.Equal(0, list2.LastIndexOf(nanVariant1, 1, 2));
            Assert.Equal(0, list2.LastIndexOf(nanVariant2, 1, 2));
        }

        /// <summary>
        /// Tests LastIndexOf with double NaN values. In Java/J2N, NaN == NaN, unlike .NET's BCL.
        /// </summary>
        [Fact]
        public void LastIndexOf_DoubleNaN()
        {
            if (typeof(T) != typeof(double))
                return;

            // Test with different NaN bit patterns
            double nanDefault = double.NaN; // 0x7FF8000000000000
            double nanVariant1 = BitConversion.Int64BitsToDouble(0x7FF8000000000001);
            double nanVariant2 = BitConversion.Int64BitsToDouble(unchecked((long)0xFFF8000000000001));

            List<double> list = (List<double>)(object)new List<T> { (T)(object)1.0d, (T)(object)nanDefault, (T)(object)3.0d };

            // NaN should be found at index 1 regardless of NaN bit pattern
            Assert.Equal(1, list.LastIndexOf(nanDefault));
            Assert.Equal(1, list.LastIndexOf(nanVariant1));
            Assert.Equal(1, list.LastIndexOf(nanVariant2));

            Assert.Equal(1, list.LastIndexOf(nanDefault, 2));
            Assert.Equal(1, list.LastIndexOf(nanVariant1, 2));
            Assert.Equal(1, list.LastIndexOf(nanVariant2, 2));

            Assert.Equal(1, list.LastIndexOf(nanDefault, 2, 3));
            Assert.Equal(1, list.LastIndexOf(nanVariant1, 2, 3));
            Assert.Equal(1, list.LastIndexOf(nanVariant2, 2, 3));

            // NaN at different positions with multiple NaN bit patterns
            var list2 = new List<double> { nanVariant1, 1.0d, nanDefault, 3.0d, nanVariant2 };
            Assert.Equal(4, list2.LastIndexOf(nanDefault));
            Assert.Equal(4, list2.LastIndexOf(nanVariant1));
            Assert.Equal(4, list2.LastIndexOf(nanVariant2));

            Assert.Equal(2, list2.LastIndexOf(nanDefault, 2));
            Assert.Equal(2, list2.LastIndexOf(nanVariant1, 2));
            Assert.Equal(2, list2.LastIndexOf(nanVariant2, 2));

            Assert.Equal(0, list2.LastIndexOf(nanDefault, 1, 2));
            Assert.Equal(0, list2.LastIndexOf(nanVariant1, 1, 2));
            Assert.Equal(0, list2.LastIndexOf(nanVariant2, 1, 2));
        }

        /// <summary>
        /// Tests IndexOf with float signed zero. In Java/J2N, +0.0f != -0.0f, unlike .NET's BCL.
        /// </summary>
        [Fact]
        public void IndexOf_FloatSignedZero()
        {
            if (typeof(T) != typeof(float))
                return;

            float positiveZero = 0.0f;
            float negativeZero = -0.0f;

            List<float> list = (List<float>)(object)new List<T>
            {
                (T)(object)positiveZero,
                (T)(object)1.0f,
                (T)(object)negativeZero,
                (T)(object)3.0f
            };

            // +0.0f should be found at index 0, not at index 2
            Assert.Equal(0, list.IndexOf(positiveZero));
            Assert.Equal(2, list.IndexOf(negativeZero));
        }

        /// <summary>
        /// Tests IndexOf with double signed zero. In Java/J2N, +0.0d != -0.0d, unlike .NET's BCL.
        /// </summary>
        [Fact]
        public void IndexOf_DoubleSignedZero()
        {
            if (typeof(T) != typeof(double))
                return;

            double positiveZero = 0.0d;
            double negativeZero = -0.0d;

            List<double> list = (List<double>)(object)new List<T>
            {
                (T)(object)positiveZero,
                (T)(object)1.0d,
                (T)(object)negativeZero,
                (T)(object)3.0d
            };

            // +0.0d should be found at index 0, not at index 2
            Assert.Equal(0, list.IndexOf(positiveZero));
            Assert.Equal(2, list.IndexOf(negativeZero));
        }

        /// <summary>
        /// Tests LastIndexOf with float signed zero. In Java/J2N, +0.0f != -0.0f, unlike .NET's BCL.
        /// </summary>
        [Fact]
        public void LastIndexOf_FloatSignedZero()
        {
            if (typeof(T) != typeof(float))
                return;

            float positiveZero = 0.0f;
            float negativeZero = -0.0f;

            List<float> list = (List<float>)(object)new List<T>
            {
                (T)(object)positiveZero,
                (T)(object)1.0f,
                (T)(object)negativeZero,
                (T)(object)positiveZero,
                (T)(object)3.0f
            };

            // Searching for +0.0f should return the last occurrence at index 3
            // Searching for -0.0f should return the occurrence at index 2
            Assert.Equal(3, list.LastIndexOf(positiveZero));
            Assert.Equal(2, list.LastIndexOf(negativeZero));
        }

        /// <summary>
        /// Tests LastIndexOf with double signed zero. In Java/J2N, +0.0d != -0.0d, unlike .NET's BCL.
        /// </summary>
        [Fact]
        public void LastIndexOf_DoubleSignedZero()
        {
            if (typeof(T) != typeof(double))
                return;

            double positiveZero = 0.0d;
            double negativeZero = -0.0d;

            List<double> list = (List<double>)(object)new List<T>
            {
                (T)(object)positiveZero,
                (T)(object)1.0d,
                (T)(object)negativeZero,
                (T)(object)positiveZero,
                (T)(object)3.0d
            };

            // Searching for +0.0d should return the last occurrence at index 3
            // Searching for -0.0d should return the occurrence at index 2
            Assert.Equal(3, list.LastIndexOf(positiveZero));
            Assert.Equal(2, list.LastIndexOf(negativeZero));
        }

        #endregion
    }
}
