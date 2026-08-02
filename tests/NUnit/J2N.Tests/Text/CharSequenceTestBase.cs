using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading;
#nullable enable

namespace J2N.Text
{
    [TestFixture]
    internal abstract class CharSequenceTestBase<T>
        where T: ICharSequence, IComparable<ICharSequence?>, IEquatable<ICharSequence?>
    {
        protected static readonly string String1 = "This is a portriat of a Turkish czar";
        protected static readonly string String2 = "This is not an equal string";

        protected static readonly char[] CharArray1 = String1.ToCharArray();
        protected static readonly char[] CharArray2 = String2.ToCharArray();

        protected static readonly StringBuilder StringBuilder1 = new(String1);
        protected static readonly StringBuilder StringBuilder2 = new(String2);

        protected CultureInfo originalCulture = null!;

        public abstract T CreateClassUnderTest(string? value);


        [SetUp]
        public virtual void SetUp()
        {
            originalCulture = CultureInfo.CurrentCulture;
#if !FEATURE_CULTUREINFO_CURRENTCULTURE_SETTER
            Thread.CurrentThread.CurrentCulture
#else
            CultureInfo.CurrentCulture
#endif
                 = new CultureInfo("tr-TR");
        }

        [TearDown]
        public virtual void TearDown()
        {
#if !FEATURE_CULTUREINFO_CURRENTCULTURE_SETTER
            Thread.CurrentThread.CurrentCulture
#else
            CultureInfo.CurrentCulture
#endif
                = originalCulture;
        }

        [TestCase("This is a portriat of a Turkish czar", true)]
        [TestCase("", true)]
        [TestCase(null, false)]
        public virtual void Test_HasValue(string? value, bool expected)
        {
            Assert.AreEqual(expected, CreateClassUnderTest(value).HasValue);
        }

        [Test]
        public virtual void Test_Indexer()
        {
            Assert.AreEqual('p', CreateClassUnderTest(String1)[10]);
        }

        [Test]
        public virtual void Test_Indexer_Invalid()
        {
            Assert.Throws<IndexOutOfRangeException>(() => { var x = CreateClassUnderTest(String1)[String1.Length + 1]; });
            Assert.Throws<IndexOutOfRangeException>(() => { var x = CreateClassUnderTest("")[0]; });
            Assert.Throws<InvalidOperationException>(() => { var x = CreateClassUnderTest(null)[10]; });
        }


        [TestCase("This is a portriat of a Turkish czar", 36)]
        [TestCase("", 0)]
        [TestCase(null, 0)]
        public virtual void Test_Length(string? value, int expected)
        {
            Assert.AreEqual(expected, CreateClassUnderTest(value).Length);
        }

        [TestCase("This is a portriat of a Turkish czar", 6, 10, "s a portri")]
        [TestCase("This is a portriat of a Turkish czar", 0, 36, "This is a portriat of a Turkish czar")]
        [TestCase("This is a portriat of a Turkish czar", 10, 0, "")]
        public virtual void Test_Subsequence(string? value, int startIndex, int length, string expected)
        {
            Assert.AreEqual(expected, CreateClassUnderTest(value).Subsequence(startIndex, length).ToString());

            //Assert.IsFalse(nullTarget.Subsequence(6, 10).HasValue); // Null target will always return null subsequence
        }

        [Test]
        public virtual void Test_Subsequence_Invalid()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => CreateClassUnderTest(String1).Subsequence(-1, 10));
            Assert.Throws<ArgumentOutOfRangeException>(() => CreateClassUnderTest(String1).Subsequence(3, -2));
            Assert.Throws<ArgumentOutOfRangeException>(() => CreateClassUnderTest(String1).Subsequence(String1.Length, 1));

            Assert.Throws<ArgumentOutOfRangeException>(() => CreateClassUnderTest("").Subsequence(0, 1));
        }

        [TestCase("Hello", "Hello")]
        [TestCase("This is a portriat of a Turkish czar", "This is a portriat of a Turkish czar")]
        [TestCase(null, "")]
        [TestCase("", "")]
        public virtual void Test_ToString(string? value, string expected)
        {
            Assert.AreEqual(expected, CreateClassUnderTest(value).ToString());
        }

        public static IEnumerable<TestCaseData> Equals_Object_TestData()
        {
            var factories = new Func<string?, object?>[]
            {
                CharSequenceUtil.CreateStringCharSequence,
                CharSequenceUtil.CreateCharArrayCharSequence,
                CharSequenceUtil.CreateStringBuilderCharSequence,
                CharSequenceUtil.CreateMutableTextBufferCharSequence,
                CharSequenceUtil.CreateSynchronizedTextBuilderCharSequence,
                CharSequenceUtil.CreateStringBuffer,

                CharSequenceUtil.CreateString,
                CharSequenceUtil.CreateCharArray,
                CharSequenceUtil.CreateStringBuilder,

                CharSequenceUtil.CreateTextBuilder,
                CharSequenceUtil.CreatePooledTextBuilder,
                CharSequenceUtil.CreateSynchronizedTextBuilder,
            };

            foreach (var factory in factories)
            {
                foreach (var item in CharSequenceUtil.Equals_String_TestData())
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
            var factories = new Func<string?, object?>[]
            {
                CharSequenceUtil.CreateStringCharSequence,
                CharSequenceUtil.CreateCharArrayCharSequence,
                CharSequenceUtil.CreateStringBuilderCharSequence,
                CharSequenceUtil.CreateMutableTextBufferCharSequence,
                CharSequenceUtil.CreateSynchronizedTextBuilderCharSequence,
                CharSequenceUtil.CreateStringBuffer,

                //CharSequenceUtil.CreateTextBuilderAsCharSequence,
                //CharSequenceUtil.CreatePooledTextBuilderAsCharSequence,
            };

            foreach (var factory in factories)
            {
                foreach (var item in CharSequenceUtil.Equals_String_TestData())
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
                CharSequenceUtil.CreateStringCharSequence,
            };

            foreach (var factory in factories)
            {
                foreach (var item in CharSequenceUtil.Equals_String_TestData())
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
                CharSequenceUtil.CreateCharArrayCharSequence,
            };

            foreach (var factory in factories)
            {
                foreach (var item in CharSequenceUtil.Equals_String_TestData())
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
                CharSequenceUtil.CreateStringBuilderCharSequence,
            };

            foreach (var factory in factories)
            {
                foreach (var item in CharSequenceUtil.Equals_String_TestData())
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
                CharSequenceUtil.CreateCharArray,
            };

            foreach (var factory in factories)
            {
                foreach (var item in CharSequenceUtil.Equals_String_TestData())
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
                CharSequenceUtil.CreateStringBuilder,
            };

            foreach (var factory in factories)
            {
                foreach (var item in CharSequenceUtil.Equals_String_TestData())
                {
                    var leftArg = item[0];
                    var rightArgRaw = factory((string?)item[1]);
                    var expectedArg = item[2];

                    yield return new TestCaseData(leftArg, rightArgRaw, expectedArg)
                        .FormatArguments(leftArg, rightArgRaw);
                }
            }
        }

        public static IEnumerable<TestCaseData> CompareTo_Object_TestData()
        {
            var factories = new Func<string?, object?>[]
            {
                CharSequenceUtil.CreateStringCharSequence,
                CharSequenceUtil.CreateCharArrayCharSequence,
                CharSequenceUtil.CreateStringBuilderCharSequence,
                CharSequenceUtil.CreateMutableTextBufferCharSequence,
                CharSequenceUtil.CreateSynchronizedTextBuilderCharSequence,
                CharSequenceUtil.CreateStringBuffer,

                CharSequenceUtil.CreateString,
                CharSequenceUtil.CreateCharArray,
                CharSequenceUtil.CreateStringBuilder,

                CharSequenceUtil.CreateTextBuilder,
                CharSequenceUtil.CreatePooledTextBuilder,
                CharSequenceUtil.CreateSynchronizedTextBuilder,
            };

            foreach (var factory in factories)
            {
                foreach (var item in CharSequenceUtil.CompareTo_String_TestData())
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
            var factories = new Func<string?, object?>[]
            {
                CharSequenceUtil.CreateStringCharSequence,
                CharSequenceUtil.CreateCharArrayCharSequence,
                CharSequenceUtil.CreateStringBuilderCharSequence,
                CharSequenceUtil.CreateMutableTextBufferCharSequence,
                CharSequenceUtil.CreateSynchronizedTextBuilderCharSequence,
                CharSequenceUtil.CreateStringBuffer,

                //CharSequenceUtil.CreateTextBuilderAsCharSequence,
                //CharSequenceUtil.CreatePooledTextBuilderAsCharSequence,
            };

            foreach (var factory in factories)
            {
                foreach (var item in CharSequenceUtil.CompareTo_String_TestData())
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
                CharSequenceUtil.CreateStringCharSequence,
            };

            foreach (var factory in factories)
            {
                foreach (var item in CharSequenceUtil.CompareTo_String_TestData())
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
                CharSequenceUtil.CreateCharArrayCharSequence,
            };

            foreach (var factory in factories)
            {
                foreach (var item in CharSequenceUtil.CompareTo_String_TestData())
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
                CharSequenceUtil.CreateStringBuilderCharSequence,
            };

            foreach (var factory in factories)
            {
                foreach (var item in CharSequenceUtil.CompareTo_String_TestData())
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
                CharSequenceUtil.CreateCharArray,
            };

            foreach (var factory in factories)
            {
                foreach (var item in CharSequenceUtil.CompareTo_String_TestData())
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
                CharSequenceUtil.CreateStringBuilder,
            };

            foreach (var factory in factories)
            {
                foreach (var item in CharSequenceUtil.CompareTo_String_TestData())
                {
                    var leftArg = item[0];
                    var rightArgRaw = factory((string?)item[1]);
                    var expectedArg = item[2];

                    yield return new TestCaseData(leftArg, rightArgRaw, expectedArg)
                        .FormatArguments(leftArg, rightArgRaw);
                }
            }
        }

    }
}
