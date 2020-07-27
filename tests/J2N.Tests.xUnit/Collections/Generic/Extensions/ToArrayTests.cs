// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace J2N.Collections.Generic.Extensions
{
    public class ToArrayTests : EnumerableTests
    {
        [Fact]
        public void ToArray_CreateACopyWhenNotEmpty()
        {
            int[] sourceArray = new int[] { 1, 2, 3, 4, 5 };
            int[] resultArray = sourceArray.ToArray();

            Assert.NotSame(sourceArray, resultArray);
            Assert.Equal(sourceArray, resultArray);
        }

        [Fact]
        public void ToArray_UseArrayEmptyWhenEmpty()
        {
            int[] emptySourceArray = Arrays.Empty<int>();

            Assert.Same(emptySourceArray.ToArray(), emptySourceArray.ToArray());
        }


        private void RunToArrayOnAllCollectionTypes<T>(T[] items, Action<T[]> validation)
        {
            validation(Enumerable.ToArray(items));
            validation(Enumerable.ToArray(new List<T>(items)));
            //validation(new TestEnumerable<T>(items).ToArray());
            //validation(new TestReadOnlyCollection<T>(items).ToArray());
            validation(new TestCollection<T>(items).ToArray());
        }


        [Fact]
        public void ToArray_WorkWithEmptyCollection()
        {
            RunToArrayOnAllCollectionTypes(new int[0],
                resultArray =>
                {
                    Assert.NotNull(resultArray);
                    Assert.Equal(0, resultArray.Length);
                });
        }

        [Fact]
        public void ToArray_ProduceCorrectArray()
        {
            int[] sourceArray = new int[] { 1, 2, 3, 4, 5, 6, 7 };
            RunToArrayOnAllCollectionTypes(sourceArray,
                resultArray =>
                {
                    Assert.Equal(sourceArray.Length, resultArray.Length);
                    Assert.Equal(sourceArray, resultArray);
                });

            string[] sourceStringArray = new string[] { "1", "2", "3", "4", "5", "6", "7", "8" };
            RunToArrayOnAllCollectionTypes(sourceStringArray,
                resultStringArray =>
                {
                    Assert.Equal(sourceStringArray.Length, resultStringArray.Length);
                    for (int i = 0; i < sourceStringArray.Length; i++)
                        Assert.Same(sourceStringArray[i], resultStringArray[i]);
                });
        }

        [Fact]
        public void ToArray_FromList()
        {
            Assert.Equal(new int[] { 1, 2, 3, 4, 5, 6, 7 }, ((ISet<int>)new HashSet<int>(Enumerable.Range(1, 7))).ToArray());
            Assert.Equal(
                new string[] { "1", "2", "3", "4", "5", "6", "7", "8" },
                ((IList<string>)new List<string>(Enumerable.Range(1, 8).Select(i => i.ToString()))).ToArray());
        }

        [Fact]
        public void ToArray_FromSet()
        {
            var expected1 = new int[] { 1, 2, 3, 4, 5, 6, 7 };
            var actual1 = ((ISet<int>)new HashSet<int>(Enumerable.Range(1, 7))).ToArray();
            Assert.True(expected1.Length == actual1.Length && !expected1.Except(actual1).Any());

            var expected2 = new string[] { "1", "2", "3", "4", "5", "6", "7", "8" };
            var actual2 = ((IList<string>)new List<string>(Enumerable.Range(1, 8).Select(i => i.ToString()))).ToArray();
            Assert.True(expected2.Length == actual2.Length && !expected2.Except(actual2).Any());
        }

        [Fact]
        public void ToArray_FromDictionary()
        {
            var expectedItems = new KeyValuePair<int, int>[] { new KeyValuePair<int, int>(1, 7), new KeyValuePair<int, int>(2, 6), new KeyValuePair<int, int>(3, 5), new KeyValuePair<int, int>(4, 4), new KeyValuePair<int, int>(5, 3), new KeyValuePair<int, int>(6, 2), new KeyValuePair<int, int>(7, 1) };
            var expectedKeys = new int[] { 1, 2, 3, 4, 5, 6, 7 };
            var expectedValues = new int[] { 7, 6, 5, 4, 3, 2, 1 };

            var dictionary = new Dictionary<int, int>
            {
                [1] = 7, [2] = 6, [3] = 5, [4] = 4, [5] = 3, [6] = 2, [7] = 1,
            };

            var actualItems = dictionary.ToArray();
            var actualKeys = dictionary.Keys.ToArray();
            var actualValues = dictionary.Values.ToArray();

            Assert.True(expectedItems.Length == actualItems.Length && !expectedItems.Except(actualItems).Any());
            Assert.True(expectedKeys.Length == actualKeys.Length && !expectedKeys.Except(actualKeys).Any());
            Assert.True(expectedValues.Length == actualValues.Length && !expectedValues.Except(actualValues).Any());

            var expectedItems2 = new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("1", "8"), new KeyValuePair<string, string>("2", "7"), new KeyValuePair<string, string>("3", "6"), new KeyValuePair<string, string>("4", "5"), new KeyValuePair<string, string>("5", "4"), new KeyValuePair<string, string>("6", "3"), new KeyValuePair<string, string>("7", "2"), new KeyValuePair<string, string>("8", "1") };
            var expectedKeys2 = new string[] { "1", "2", "3", "4", "5", "6", "7", "8" };
            var expectedValues2 = new string[] { "8", "7", "6", "5", "4", "3", "2", "1" };

            var dictionary2 = new Dictionary<string, string>
            {
                ["1"] = "8", ["2"] = "7", ["3"] = "6", ["4"] = "5", ["5"] = "4", ["6"] = "3", ["7"] = "2", ["8"] = "1",
            };

            var actualItems2 = dictionary2.ToArray();
            var actualKeys2 = dictionary2.Keys.ToArray();
            var actualValues2 = dictionary2.Values.ToArray();

            Assert.True(expectedItems2.Length == actualItems2.Length && !expectedItems2.Except(actualItems2).Any());
            Assert.True(expectedKeys2.Length == actualKeys2.Length && !expectedKeys2.Except(actualKeys2).Any());
            Assert.True(expectedValues2.Length == actualValues2.Length && !expectedValues2.Except(actualValues2).Any());
        }

        [Fact]
        public void ToArray_TouchCountWithICollection()
        {
            TestCollection<int> source = new TestCollection<int>(new int[] { 1, 2, 3, 4 });
            var resultArray = source.ToArray();

            Assert.Equal(source, resultArray);
            Assert.Equal(1, source.CountTouched);
        }


        [Fact]
        public void ToArray_ThrowArgumentNullExceptionWhenSourceIsNull()
        {
            int[] source = null;
            AssertExtensions.Throws<ArgumentNullException>("source", () => source.ToArray());
        }

        // Generally the optimal approach. Anything that breaks this should be confirmed as not harming performance.
        [Fact]
        public void ToArray_UseCopyToWithICollection()
        {
            TestCollection<int> source = new TestCollection<int>(new int[] { 1, 2, 3, 4 });
            var resultArray = source.ToArray();

            Assert.Equal(source, resultArray);
            Assert.Equal(1, source.CopyToTouched);
        }

        [Theory]
        [InlineData(new int[] { }, new string[] { })]
        [InlineData(new int[] { 1 }, new string[] { "1" })]
        [InlineData(new int[] { 1, 2, 3 }, new string[] { "1", "2", "3" })]
        public void ToArray_ListWhereSelect(int[] sourceIntegers, string[] convertedStrings)
        {
            Assert.Equal(convertedStrings, ((IList<string>)new List<string>(sourceIntegers.Select(i => i.ToString()))).ToArray());

            Assert.Equal(sourceIntegers, ((IList<int>)new List<int>(sourceIntegers.Where(i => true))).ToArray());
            Assert.Equal(Arrays.Empty<int>(), ((IList<int>)new List<int>(sourceIntegers.Where(i => false))).ToArray());

            Assert.Equal(convertedStrings, ((IList<string>)new List<string>(sourceIntegers.Where(i => true).Select(i => i.ToString()))).ToArray());
            Assert.Equal(Arrays.Empty<string>(), ((IList<string>)new List<string>(sourceIntegers.Where(i => false).Select(i => i.ToString()))).ToArray());

            Assert.Equal(convertedStrings, ((IList<string>)new List<string>(sourceIntegers.Select(i => i.ToString()).Where(s => s != null))).ToArray());
            Assert.Equal(Arrays.Empty<string>(), ((IList<string>)new List<string>(sourceIntegers.Select(i => i.ToString()).Where(s => s == null))).ToArray());
        }

        [Fact]
        public void SameResultsRepeatCallsFromWhereOnIntQuery()
        {
            ICollection<int> q = (from x in new[] { 9999, 0, 888, -1, 66, -777, 1, 2, -12345 }
                    where x > int.MinValue
                    select x).ToList();

            Assert.Equal(q.ToArray(), q.ToArray());
        }

        [Fact]
        public void SameResultsRepeatCallsFromWhereOnStringQuery()
        {
            ICollection<string> q = (from x in new[] { "!@#$%^", "C", "AAA", "", "Calling Twice", "SoS", string.Empty }
                    where !string.IsNullOrEmpty(x)
                    select x).ToList();

            Assert.Equal(q.ToArray(), q.ToArray());
        }

        [Fact]
        public void SameResultsButNotSameObject()
        {
            ICollection<int> qInt = (from x in new[] { 9999, 0, 888, -1, 66, -777, 1, 2, -12345 }
                       where x > int.MinValue
                       select x).ToList();

            ICollection<string> qString = (from x in new[] { "!@#$%^", "C", "AAA", "", "Calling Twice", "SoS", string.Empty }
                          where !string.IsNullOrEmpty(x)
                          select x).ToList();

            Assert.NotSame(qInt.ToArray(), qInt.ToArray());
            Assert.NotSame(qString.ToArray(), qString.ToArray());
        }

        [Fact]
        public void EmptyArraysSameObject()
        {
            //// .NET Core returns the instance as an optimization.
            //// see https://github.com/dotnet/corefx/pull/2401.
            //Assert.True(ReferenceEquals(Enumerable.Empty<int>().ToArray(), Enumerable.Empty<int>().ToArray()));

            var array = new int[0];
            Assert.NotSame(array, array.ToArray());
        }

        [Fact]
        public void SourceIsEmptyICollectionT()
        {
            int[] source = { };

            ICollection<int> collection = source as ICollection<int>;

            Assert.Empty(source.ToArray());
            Assert.Empty(collection.ToArray());
        }

        [Fact]
        public void SourceIsICollectionTWithFewElements()
        {
            int?[] source = { -5, null, 0, 10, 3, -1, null, 4, 9 };
            int?[] expected = { -5, null, 0, 10, 3, -1, null, 4, 9 };

            ICollection<int?> collection = source as ICollection<int?>;

            Assert.Equal(expected, source.ToArray());
            Assert.Equal(expected, collection.ToArray());
        }

        [Fact]
        public void CountPartitionSelectSameTypeToArray()
        {
            ICollection<int> source = Enumerable.Range(0, 100).Select(i => i * 2).Skip(1).Take(5).ToList();
            Assert.Equal(new[] { 2, 4, 6, 8, 10 }, source.ToArray());
        }

        [Fact]
        public void CountPartitionSelectDiffTypeToArray()
        {
            ICollection<string> source = Enumerable.Range(0, 100).Select(i => i.ToString()).Skip(1).Take(5).ToArray();
            Assert.Equal(new[] { "1", "2", "3", "4", "5" }, source.ToArray());
        }

        [Fact]
        public void CountEmptyPartitionSelectSameTypeToArray()
        {
            ICollection<int> source = Enumerable.Range(0, 100).Select(i => i * 2).Skip(1000).ToList();
            Assert.Empty(source.ToArray());
        }

        [Fact]
        public void CountEmptyPartitionSelectDiffTypeToArray()
        {
            ICollection<string> source = Enumerable.Range(0, 100).Select(i => i.ToString()).Skip(1000).ToList();
            Assert.Empty(source.ToArray());
        }
    }
}
