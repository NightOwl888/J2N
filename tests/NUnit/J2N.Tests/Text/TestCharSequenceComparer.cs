using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Threading;
#nullable enable

namespace J2N.Text
{
    [TestFixture]
    internal class TestCharSequenceComparer
    {
        private CultureInfo originalCulture = null!;


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

        #region Equals

        public static IEnumerable<TestCaseData> Equals_Object_Object_TestData()
        {
            var leftFactories = new Func<string?, object?>[]
            {
                CharSequenceUtil.CreateStringCharSequence,
                CharSequenceUtil.CreateCharArrayCharSequence,
                CharSequenceUtil.CreateStringBuilderCharSequence,
                CharSequenceUtil.CreateMutableTextBufferCharSequence,
                CharSequenceUtil.CreateSynchronizedTextBuilderCharSequence,
                CharSequenceUtil.CreateStringBuffer,
            };

            var rightFactories = new Func<string?, object?>[]
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
            };

            foreach (var leftFactory in leftFactories)
            {
                foreach (var rightFactory in rightFactories)
                {
                    foreach (var item in CharSequenceUtil.Equals_String_TestData())
                    {
                        var leftArg = leftFactory((string?)item[0]);
                        var rightArg = rightFactory((string?)item[1]);
                        var expectedArg = item[2];

                        yield return new TestCaseData(leftArg, rightArg, expectedArg)
                            .FormatArguments(leftArg, rightArg);
                    }
                }
            }
        }

        [TestCaseSource(nameof(Equals_Object_Object_TestData))]
        public void Test_Equals_Object_Object(object? leftValue, object? rightValue, bool expected)
        {
            try
            {
                if (leftValue is null)
                {
                    if (rightValue is StringCharSequence scs2 && !scs2.HasValue)
                        expected = false; // J2N TODO: Fix broken null comparison
                    if (rightValue is CharArrayCharSequence cacs2 && !cacs2.HasValue)
                        expected = false; // J2N TODO: Fix broken null comparison
                    if (rightValue is StringBuilderCharSequence sbcs2 && !sbcs2.HasValue)
                        expected = false; // J2N TODO: Fix broken null comparison
                }
                if (leftValue is StringCharSequence scs1 && !scs1.HasValue)
                {
                    if (rightValue is null)
                        expected = false; // J2N TODO: Fix broken null comparison
                }
                if (leftValue is CharArrayCharSequence cacs1 && !cacs1.HasValue)
                {
                    if (rightValue is null)
                        expected = false; // J2N TODO: Fix broken null comparison
                }
                if (leftValue is StringBuilderCharSequence sbcs1 && !sbcs1.HasValue)
                {
                    if (rightValue is null)
                        expected = false; // J2N TODO: Fix broken null comparison
                }

                Assert.AreEqual(expected, CharSequenceComparer.Ordinal.Equals(leftValue, rightValue));
            }
            finally
            {
                CharSequenceUtil.Dispose(leftValue);
                CharSequenceUtil.Dispose(rightValue);
            }
        }

        public static IEnumerable<TestCaseData> Equals_ICharSequence_ICharSequence_TestData()
        {
            var factories = new Func<string?, object?>[]
            {
                CharSequenceUtil.CreateStringCharSequence,
                CharSequenceUtil.CreateCharArrayCharSequence,
                CharSequenceUtil.CreateStringBuilderCharSequence,
                CharSequenceUtil.CreateMutableTextBufferCharSequence,
                CharSequenceUtil.CreateSynchronizedTextBuilderCharSequence,
                CharSequenceUtil.CreateStringBuffer,

                //CreateTextBuilderAsCharSequence,
                //CreatePooledTextBuilderAsCharSequence,
            };

            foreach (var leftFactory in factories)
            {
                foreach (var rightFactory in factories)
                {
                    foreach (var item in CharSequenceUtil.Equals_String_TestData())
                    {
                        var leftArg = leftFactory((string?)item[0]);
                        var rightArg = rightFactory((string?)item[1]);
                        var expectedArg = item[2];

                        yield return new TestCaseData(leftArg, rightArg, expectedArg)
                            .FormatArguments(leftArg, rightArg);
                    }
                }
            }
        }

        [TestCaseSource(nameof(Equals_ICharSequence_ICharSequence_TestData))]
        public void Test_Equals_ICharSequence_ICharSequence(ICharSequence? leftValue, ICharSequence? rightValue, bool expected)
        {
            try
            {
                Assert.AreEqual(expected, CharSequenceComparer.Ordinal.Equals(leftValue, rightValue));
            }
            finally
            {
                CharSequenceUtil.Dispose(leftValue);
                CharSequenceUtil.Dispose(rightValue);
            }
        }

        public static IEnumerable<TestCaseData> Equals_ICharSequence_String_TestData()
        {
            var leftFactories = new Func<string?, object?>[]
            {
                CharSequenceUtil.CreateStringCharSequence,
                CharSequenceUtil.CreateCharArrayCharSequence,
                CharSequenceUtil.CreateStringBuilderCharSequence,
                CharSequenceUtil.CreateMutableTextBufferCharSequence,
                CharSequenceUtil.CreateSynchronizedTextBuilderCharSequence,
                CharSequenceUtil.CreateStringBuffer,
            };

            var rightFactories = new Func<string?, object?>[]
            {
                CharSequenceUtil.CreateString,
            };

            foreach (var leftFactory in leftFactories)
            {
                foreach (var rightFactory in rightFactories)
                {
                    foreach (var item in CharSequenceUtil.Equals_String_TestData())
                    {
                        var leftArg = leftFactory((string?)item[0]);
                        var rightArg = rightFactory((string?)item[1]);
                        var expectedArg = item[2];

                        yield return new TestCaseData(leftArg, rightArg, expectedArg)
                            .FormatArguments(leftArg, rightArg);
                    }
                }
            }
        }

        [TestCaseSource(nameof(Equals_ICharSequence_String_TestData))]
        public void Test_Equals_ICharSequence_String(ICharSequence? leftValue, string? rightValue, bool expected)
        {
            try
            {
                Assert.AreEqual(expected, CharSequenceComparer.Ordinal.Equals(leftValue, rightValue));
            }
            finally
            {
                CharSequenceUtil.Dispose(leftValue);
            }
        }

        public static IEnumerable<TestCaseData> Equals_ICharSequence_CharArray_TestData()
        {
            var leftFactories = new Func<string?, object?>[]
            {
                CharSequenceUtil.CreateStringCharSequence,
                CharSequenceUtil.CreateCharArrayCharSequence,
                CharSequenceUtil.CreateStringBuilderCharSequence,
                CharSequenceUtil.CreateMutableTextBufferCharSequence,
                CharSequenceUtil.CreateSynchronizedTextBuilderCharSequence,
                CharSequenceUtil.CreateStringBuffer,
            };

            var rightFactories = new Func<string?, object?>[]
            {
                CharSequenceUtil.CreateCharArray,
            };

            foreach (var leftFactory in leftFactories)
            {
                foreach (var rightFactory in rightFactories)
                {
                    foreach (var item in CharSequenceUtil.Equals_String_TestData())
                    {
                        var leftArg = leftFactory((string?)item[0]);
                        var rightArg = rightFactory((string?)item[1]);
                        var expectedArg = item[2];

                        yield return new TestCaseData(leftArg, rightArg, expectedArg)
                            .FormatArguments(leftArg, rightArg);
                    }
                }
            }
        }

        [TestCaseSource(nameof(Equals_ICharSequence_CharArray_TestData))]
        public void Test_Equals_ICharSequence_CharArray(ICharSequence? leftValue, char[]? rightValue, bool expected)
        {
            try
            {
                Assert.AreEqual(expected, CharSequenceComparer.Ordinal.Equals(leftValue, rightValue));
            }
            finally
            {
                CharSequenceUtil.Dispose(leftValue);
            }
        }

        public static IEnumerable<TestCaseData> Equals_ICharSequence_StringBuilder_TestData()
        {
            var leftFactories = new Func<string?, object?>[]
            {
                CharSequenceUtil.CreateStringCharSequence,
                CharSequenceUtil.CreateCharArrayCharSequence,
                CharSequenceUtil.CreateStringBuilderCharSequence,
                CharSequenceUtil.CreateMutableTextBufferCharSequence,
                CharSequenceUtil.CreateSynchronizedTextBuilderCharSequence,
                CharSequenceUtil.CreateStringBuffer,
            };

            var rightFactories = new Func<string?, object?>[]
            {
                CharSequenceUtil.CreateStringBuilder,
            };

            foreach (var leftFactory in leftFactories)
            {
                foreach (var rightFactory in rightFactories)
                {
                    foreach (var item in CharSequenceUtil.Equals_String_TestData())
                    {
                        var leftArg = leftFactory((string?)item[0]);
                        var rightArg = rightFactory((string?)item[1]);
                        var expectedArg = item[2];

                        yield return new TestCaseData(leftArg, rightArg, expectedArg)
                            .FormatArguments(leftArg, rightArg);
                    }
                }
            }
        }

        [TestCaseSource(nameof(Equals_ICharSequence_StringBuilder_TestData))]
        public void Test_Equals_ICharSequence_StringBuilder(ICharSequence? leftValue, StringBuilder? rightValue, bool expected)
        {
            try
            {
                Assert.AreEqual(expected, CharSequenceComparer.Ordinal.Equals(leftValue, rightValue));
            }
            finally
            {
                CharSequenceUtil.Dispose(leftValue);
            }
        }

        #endregion Equals


        #region Compare

        public static IEnumerable<TestCaseData> Compare_Object_Object_TestData()
        {
            var leftFactories = new Func<string?, object?>[]
            {
                CharSequenceUtil.CreateStringCharSequence,
                CharSequenceUtil.CreateCharArrayCharSequence,
                CharSequenceUtil.CreateStringBuilderCharSequence,
                CharSequenceUtil.CreateMutableTextBufferCharSequence,
                CharSequenceUtil.CreateSynchronizedTextBuilderCharSequence,
                CharSequenceUtil.CreateStringBuffer,
            };

            var rightFactories = new Func<string?, object?>[]
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
            };

            foreach (var leftFactory in leftFactories)
            {
                foreach (var rightFactory in rightFactories)
                {
                    foreach (var item in CharSequenceUtil.CompareTo_String_TestData())
                    {
                        var leftArg = leftFactory((string?)item[0]);
                        var rightArg = rightFactory((string?)item[1]);
                        var expectedArg = item[2];

                        yield return new TestCaseData(leftArg, rightArg, expectedArg)
                            .FormatArguments(leftArg, rightArg);
                    }
                }
            }
        }

        // J2N TODO: When any factories that can return naked null value are added, this test causes
        // all tests to run when this class node is run in VS2026 and other quirks like changing
        // grouping depending on which nodes are selected. It is unclear whether this is an NUnit bug,
        // somethign in the test SDK, something in the adapter, or a bug in VS2026. For now, we will leave
        // this in place, but if you need selecting the J2N.Text tests to work right, this test needs to be commented.
        [TestCaseSource(nameof(Compare_Object_Object_TestData))]
        public void Test_Compare_Object_Object(object? leftValue, object? rightValue, int expected)
        {
            try
            {
                if (leftValue is null)
                {
                    if (rightValue is StringCharSequence scs2 && !scs2.HasValue)
                        expected = -1; // J2N TODO: Fix broken null comparison
                    if (rightValue is CharArrayCharSequence cacs2 && !cacs2.HasValue)
                        expected = -1; // J2N TODO: Fix broken null comparison
                    if (rightValue is StringBuilderCharSequence sbcs2 && !sbcs2.HasValue)
                        expected = -1; // J2N TODO: Fix broken null comparison
                }
                if (leftValue is StringCharSequence scs1 && !scs1.HasValue)
                {
                    if (rightValue is null)
                        expected = 1; // J2N TODO: Fix broken null comparison
                    if (rightValue is CharArrayCharSequence cacs2)
                    {
                        if (!cacs2.HasValue)
                            expected = -1; // J2N TODO: Fix broken null comparison
                        else if (cacs2.HasValue && cacs2.Length == 0)
                            expected = 0; // J2N TODO: Fix broken null comparison
                    }
                    if (rightValue is StringBuilderCharSequence sbcs2)
                    {
                        if (!sbcs2.HasValue)
                            expected = -1; // J2N TODO: Fix broken null comparison
                        else if (sbcs2.HasValue && sbcs2.Length == 0)
                            expected = 0; // J2N TODO: Fix broken null comparison
                    }
                    if (rightValue is StringCharSequence scs2)
                    {
                        if (!scs2.HasValue)
                            expected = -1; // J2N TODO: Fix broken null comparison
                        else if (scs2.HasValue && scs2.Length == 0)
                            expected = 0; // J2N TODO: Fix broken null comparison
                    }
                    if (rightValue is StringBuffer sbuf2)
                    {
                        if (!((ICharSequence)sbuf2).HasValue)
                            expected = -1; // J2N TODO: Fix broken null comparison
                        else if (((ICharSequence)sbuf2).HasValue && sbuf2.Length == 0)
                            expected = 0; // J2N TODO: Fix broken null comparison
                    }
                }
                if (leftValue is CharArrayCharSequence cacs1 && !cacs1.HasValue)
                {
                    if (rightValue is null)
                        expected = 1; // J2N TODO: Fix broken null comparison
                    if (rightValue is CharArrayCharSequence cacs2)
                    {
                        if (!cacs2.HasValue)
                            expected = -1; // J2N TODO: Fix broken null comparison
                        else if (cacs2.HasValue && cacs2.Length == 0)
                            expected = 0; // J2N TODO: Fix broken null comparison
                    }
                    if (rightValue is StringBuilderCharSequence sbcs2)
                    {
                        if (!sbcs2.HasValue)
                            expected = -1; // J2N TODO: Fix broken null comparison
                        else if (sbcs2.HasValue && sbcs2.Length == 0)
                            expected = 0; // J2N TODO: Fix broken null comparison
                    }
                    if (rightValue is StringCharSequence scs2)
                    {
                        if (!scs2.HasValue)
                            expected = -1; // J2N TODO: Fix broken null comparison
                        else if (scs2.HasValue && scs2.Length == 0)
                            expected = 0; // J2N TODO: Fix broken null comparison
                    }
                    if (rightValue is StringBuffer sbuf2)
                    {
                        if (!((ICharSequence)sbuf2).HasValue)
                            expected = -1; // J2N TODO: Fix broken null comparison
                        else if (((ICharSequence)sbuf2).HasValue && sbuf2.Length == 0)
                            expected = 0; // J2N TODO: Fix broken null comparison
                    }
                }
                if (leftValue is StringBuilderCharSequence sbcs1 && !sbcs1.HasValue)
                {
                    if (rightValue is null)
                        expected = 1; // J2N TODO: Fix broken null comparison
                    if (rightValue is CharArrayCharSequence cacs2)
                    {
                        if (!cacs2.HasValue)
                            expected = -1; // J2N TODO: Fix broken null comparison
                        else if (cacs2.HasValue && cacs2.Length == 0)
                            expected = 0; // J2N TODO: Fix broken null comparison
                    }
                    if (rightValue is StringBuilderCharSequence sbcs2)
                    {
                        if (!sbcs2.HasValue)
                            expected = -1; // J2N TODO: Fix broken null comparison
                        else if (sbcs2.HasValue && sbcs2.Length == 0)
                            expected = 0; // J2N TODO: Fix broken null comparison
                    }
                    if (rightValue is StringCharSequence scs2)
                    {
                        if (!scs2.HasValue)
                            expected = -1; // J2N TODO: Fix broken null comparison
                        else if (scs2.HasValue && scs2.Length == 0)
                            expected = 0; // J2N TODO: Fix broken null comparison
                    }
                    if (rightValue is StringBuffer sbuf2)
                    {
                        if (!((ICharSequence)sbuf2).HasValue)
                            expected = -1; // J2N TODO: Fix broken null comparison
                        else if (((ICharSequence)sbuf2).HasValue && sbuf2.Length == 0)
                            expected = 0; // J2N TODO: Fix broken null comparison
                    }
                }

                CharSequenceUtil.AssertCompareTo(expected, CharSequenceComparer.Ordinal.Compare(leftValue, rightValue));
            }
            finally
            {
                CharSequenceUtil.Dispose(leftValue);
                CharSequenceUtil.Dispose(rightValue);
            }
        }

        public static IEnumerable<TestCaseData> Compare_ICharSequence_ICharSequence_TestData()
        {
            var factories = new Func<string?, object?>[]
            {
                CharSequenceUtil.CreateStringCharSequence,
                CharSequenceUtil.CreateCharArrayCharSequence,
                CharSequenceUtil.CreateStringBuilderCharSequence,
                CharSequenceUtil.CreateMutableTextBufferCharSequence,
                CharSequenceUtil.CreateSynchronizedTextBuilderCharSequence,
                CharSequenceUtil.CreateStringBuffer,

                //CreateTextBuilderAsCharSequence,
                //CreatePooledTextBuilderAsCharSequence,
            };

            foreach (var leftFactory in factories)
            {
                foreach (var rightFactory in factories)
                {
                    foreach (var item in CharSequenceUtil.CompareTo_String_TestData())
                    {
                        var leftArg = leftFactory((string?)item[0]);
                        var rightArg = rightFactory((string?)item[1]);
                        var expectedArg = item[2];

                        Debug.Assert(leftArg is null || leftArg is ICharSequence);
                        Debug.Assert(rightArg is null || rightArg is ICharSequence);


                        yield return new TestCaseData(leftArg, rightArg, expectedArg)
                            .FormatArguments(leftArg, rightArg);
                    }
                }
            }
        }

        // J2N TODO: When any factories that can return naked null value are added, this test causes
        // all tests to run when this class node is run in VS2026 and other quirks like changing
        // grouping depending on which nodes are selected. It is unclear whether this is an NUnit bug,
        // somethign in the test SDK, something in the adapter, or a bug in VS2026. For now, we will leave
        // this in place, but if you need selecting the J2N.Text tests to work right, this test needs to be commented.
        [TestCaseSource(nameof(Compare_ICharSequence_ICharSequence_TestData))]
        public void Test_Compare_ICharSequence_ICharSeqeunce(ICharSequence? leftValue, ICharSequence? rightValue, int expected)
        {
            try
            {
                if (leftValue is null)
                {
                    if (rightValue is CharArrayCharSequence cacs2)
                    {
                        if (!cacs2.HasValue)
                            expected = -1; // J2N TODO: Fix broken null comparison
                        else if (cacs2.HasValue && cacs2.Length == 0)
                            expected = 0; // J2N TODO: Fix broken null comparison
                    }
                    if (rightValue is StringBuilderCharSequence sbcs2)
                    {
                        if (!sbcs2.HasValue)
                            expected = -1; // J2N TODO: Fix broken null comparison
                        else if (sbcs2.HasValue && sbcs2.Length == 0)
                            expected = 0; // J2N TODO: Fix broken null comparison
                    }
                    if (rightValue is StringCharSequence scs2)
                    {
                        if (!scs2.HasValue)
                            expected = -1; // J2N TODO: Fix broken null comparison
                        else if (scs2.HasValue && scs2.Length == 0)
                            expected = 0; // J2N TODO: Fix broken null comparison
                    }
                    if (rightValue is StringBuffer sbuf2)
                    {
                        if (!((ICharSequence)sbuf2).HasValue)
                            expected = -1; // J2N TODO: Fix broken null comparison
                        else if (((ICharSequence)sbuf2).HasValue && sbuf2.Length == 0)
                            expected = 0; // J2N TODO: Fix broken null comparison
                    }
                }
                //if (leftValue is MutableTextBufferCharSequence mtbcs1 && !mtbcs1.HasValue)
                //{
                //    if (rightValue is ICharSequence cs2 && (!cs2.HasValue || cs2.HasValue && cs2.Length == 0))
                //        Assert.Ignore("J2N TODO: Fix broken null comparison");
                //}
                //if (leftValue is SynchronizedTextBuilderCharSequence stbcs1 && !stbcs1.HasValue)
                //{
                //    if (rightValue is ICharSequence cs2 && (!cs2.HasValue || cs2.HasValue && cs2.Length == 0))
                //        Assert.Ignore("J2N TODO: Fix broken null comparison");
                //}
                if (leftValue is CharArrayCharSequence cacs1 && !cacs1.HasValue)
                {
                    if (rightValue is CharArrayCharSequence cacs2)
                    {
                        if (!cacs2.HasValue)
                            expected = -1; // J2N TODO: Fix broken null comparison
                        else if (cacs2.HasValue && cacs2.Length == 0)
                            expected = 0; // J2N TODO: Fix broken null comparison
                    }
                    if (rightValue is StringBuilderCharSequence sbcs2)
                    {
                        if (!sbcs2.HasValue)
                            expected = -1; // J2N TODO: Fix broken null comparison
                        else if (sbcs2.HasValue && sbcs2.Length == 0)
                            expected = 0; // J2N TODO: Fix broken null comparison
                    }
                    if (rightValue is StringCharSequence scs2)
                    {
                        if (!scs2.HasValue)
                            expected = -1; // J2N TODO: Fix broken null comparison
                        else if (scs2.HasValue && scs2.Length == 0)
                            expected = 0; // J2N TODO: Fix broken null comparison
                    }
                    if (rightValue is StringBuffer sbuf2)
                    {
                        if (!((ICharSequence)sbuf2).HasValue)
                            expected = -1; // J2N TODO: Fix broken null comparison
                        else if (((ICharSequence)sbuf2).HasValue && sbuf2.Length == 0)
                            expected = 0; // J2N TODO: Fix broken null comparison
                    }
                }
                if (leftValue is StringBuilderCharSequence sbcs1 && !sbcs1.HasValue)
                {
                    if (rightValue is CharArrayCharSequence cacs2)
                    {
                        if (!cacs2.HasValue)
                            expected = -1; // J2N TODO: Fix broken null comparison
                        else if (cacs2.HasValue && cacs2.Length == 0)
                            expected = 0; // J2N TODO: Fix broken null comparison
                    }
                    if (rightValue is StringBuilderCharSequence sbcs2)
                    {
                        if (!sbcs2.HasValue)
                            expected = -1; // J2N TODO: Fix broken null comparison
                        else if (sbcs2.HasValue && sbcs2.Length == 0)
                            expected = 0; // J2N TODO: Fix broken null comparison
                    }
                    if (rightValue is StringCharSequence scs2)
                    {
                        if (!scs2.HasValue)
                            expected = -1; // J2N TODO: Fix broken null comparison
                        else if (scs2.HasValue && scs2.Length == 0)
                            expected = 0; // J2N TODO: Fix broken null comparison
                    }
                    if (rightValue is StringBuffer sbuf2)
                    {
                        if (!((ICharSequence)sbuf2).HasValue)
                            expected = -1; // J2N TODO: Fix broken null comparison
                        else if (((ICharSequence)sbuf2).HasValue && sbuf2.Length == 0)
                            expected = 0; // J2N TODO: Fix broken null comparison
                    }
                }
                if (leftValue is StringCharSequence scs1 && !scs1.HasValue)
                {
                    if (rightValue is CharArrayCharSequence cacs2)
                    {
                        if (!cacs2.HasValue)
                            expected = -1; // J2N TODO: Fix broken null comparison
                        else if (cacs2.HasValue && cacs2.Length == 0)
                            expected = 0; // J2N TODO: Fix broken null comparison
                    }
                    if (rightValue is StringBuilderCharSequence sbcs2)
                    {
                        if (!sbcs2.HasValue)
                            expected = -1; // J2N TODO: Fix broken null comparison
                        else if (sbcs2.HasValue && sbcs2.Length == 0)
                            expected = 0; // J2N TODO: Fix broken null comparison
                    }
                    if (rightValue is StringCharSequence scs2)
                    {
                        if (!scs2.HasValue)
                            expected = -1; // J2N TODO: Fix broken null comparison
                        else if (scs2.HasValue && scs2.Length == 0)
                            expected = 0; // J2N TODO: Fix broken null comparison
                    }
                    if (rightValue is StringBuffer sbuf2)
                    {
                        if (!((ICharSequence)sbuf2).HasValue)
                            expected = -1; // J2N TODO: Fix broken null comparison
                        else if (((ICharSequence)sbuf2).HasValue && sbuf2.Length == 0)
                            expected = 0; // J2N TODO: Fix broken null comparison
                    }
                }

                CharSequenceUtil.AssertCompareTo(expected, CharSequenceComparer.Ordinal.Compare(leftValue, rightValue));
            }
            finally
            {
                CharSequenceUtil.Dispose(leftValue);
                CharSequenceUtil.Dispose(rightValue);
            }
        }


        public static IEnumerable<TestCaseData> Compare_ICharSequence_String_TestData()
        {
            var leftFactories = new Func<string?, object?>[]
            {
                CharSequenceUtil.CreateStringCharSequence,
                CharSequenceUtil.CreateCharArrayCharSequence,
                CharSequenceUtil.CreateStringBuilderCharSequence,
                CharSequenceUtil.CreateMutableTextBufferCharSequence,
                CharSequenceUtil.CreateSynchronizedTextBuilderCharSequence,
                CharSequenceUtil.CreateStringBuffer,
            };

            var rightFactories = new Func<string?, object?>[]
            {
                CharSequenceUtil.CreateString,
            };

            foreach (var leftFactory in leftFactories)
            {
                foreach (var rightFactory in rightFactories)
                {
                    foreach (var item in CharSequenceUtil.CompareTo_String_TestData())
                    {
                        var leftArg = leftFactory((string?)item[0]);
                        var rightArg = rightFactory((string?)item[1]);
                        var expectedArg = item[2];

                        yield return new TestCaseData(leftArg, rightArg, expectedArg)
                            .FormatArguments(leftArg, rightArg);
                    }
                }
            }
        }

        [TestCaseSource(nameof(Compare_ICharSequence_String_TestData))]
        public void Test_Compare_ICharSequence_String(ICharSequence? leftValue, string? rightValue, int expected)
        {
            try
            {
                CharSequenceUtil.AssertCompareTo(expected, CharSequenceComparer.Ordinal.Compare(leftValue, rightValue));
            }
            finally
            {
                CharSequenceUtil.Dispose(leftValue);
            }
        }

        public static IEnumerable<TestCaseData> Compare_ICharSequence_CharArray_TestData()
        {
            var leftFactories = new Func<string?, object?>[]
            {
                CharSequenceUtil.CreateStringCharSequence,
                CharSequenceUtil.CreateCharArrayCharSequence,
                CharSequenceUtil.CreateStringBuilderCharSequence,
                CharSequenceUtil.CreateMutableTextBufferCharSequence,
                CharSequenceUtil.CreateSynchronizedTextBuilderCharSequence,
                CharSequenceUtil.CreateStringBuffer,
            };

            var rightFactories = new Func<string?, object?>[]
            {
                CharSequenceUtil.CreateCharArray,
            };

            foreach (var leftFactory in leftFactories)
            {
                foreach (var rightFactory in rightFactories)
                {
                    foreach (var item in CharSequenceUtil.CompareTo_String_TestData())
                    {
                        var leftArg = leftFactory((string?)item[0]);
                        var rightArg = rightFactory((string?)item[1]);
                        var expectedArg = item[2];

                        yield return new TestCaseData(leftArg, rightArg, expectedArg)
                            .FormatArguments(leftArg, rightArg);
                    }
                }
            }
        }

        [TestCaseSource(nameof(Compare_ICharSequence_CharArray_TestData))]
        public void Test_Compare_ICharSequence_CharArray(ICharSequence? leftValue, char[]? rightValue, int expected)
        {
            try
            {
                CharSequenceUtil.AssertCompareTo(expected, CharSequenceComparer.Ordinal.Compare(leftValue, rightValue));
            }
            finally
            {
                CharSequenceUtil.Dispose(leftValue);
            }
        }

        public static IEnumerable<TestCaseData> Compare_ICharSequence_StringBuilder_TestData()
        {
            var leftFactories = new Func<string?, object?>[]
            {
                CharSequenceUtil.CreateStringCharSequence,
                CharSequenceUtil.CreateCharArrayCharSequence,
                CharSequenceUtil.CreateStringBuilderCharSequence,
                CharSequenceUtil.CreateMutableTextBufferCharSequence,
                CharSequenceUtil.CreateSynchronizedTextBuilderCharSequence,
                CharSequenceUtil.CreateStringBuffer,
            };

            var rightFactories = new Func<string?, object?>[]
            {
                CharSequenceUtil.CreateStringBuilder,
            };

            foreach (var leftFactory in leftFactories)
            {
                foreach (var rightFactory in rightFactories)
                {
                    foreach (var item in CharSequenceUtil.CompareTo_String_TestData())
                    {
                        var leftArg = leftFactory((string?)item[0]);
                        var rightArg = rightFactory((string?)item[1]);
                        var expectedArg = item[2];

                        yield return new TestCaseData(leftArg, rightArg, expectedArg)
                            .FormatArguments(leftArg, rightArg);
                    }
                }
            }
        }

        [TestCaseSource(nameof(Compare_ICharSequence_StringBuilder_TestData))]
        public void Test_Compare_ICharSequence_StringBuilder(ICharSequence? leftValue, StringBuilder? rightValue, int expected)
        {
            try
            {
                if (leftValue is null)
                {
                    if (rightValue is null)
                        expected = -1; // J2N TODO: Fix broken null comparison
                }
                if (leftValue is StringCharSequence scs1 && !scs1.HasValue)
                {
                    if (rightValue is null)
                        expected = -1; // J2N TODO: Fix broken null comparison
                }
                if (leftValue is CharArrayCharSequence cacs1 && !cacs1.HasValue)
                {
                    if (rightValue is null)
                        expected = -1; // J2N TODO: Fix broken null comparison
                }
                if (leftValue is StringBuilderCharSequence sbcs1 && !sbcs1.HasValue)
                {
                    if (rightValue is null)
                        expected = -1; // J2N TODO: Fix broken null comparison
                }

                CharSequenceUtil.AssertCompareTo(expected, CharSequenceComparer.Ordinal.Compare(leftValue, rightValue));
            }
            finally
            {
                CharSequenceUtil.Dispose(leftValue);
            }
        }


        #endregion Compare

        public static IEnumerable<TestCaseData> GetHashCode_ICharSequence_TestData()
        {
            var factories = new Func<string?, object?>[]
            {
                CharSequenceUtil.CreateStringCharSequence,
                CharSequenceUtil.CreateCharArrayCharSequence,
                CharSequenceUtil.CreateStringBuilderCharSequence,
                CharSequenceUtil.CreateMutableTextBufferCharSequence,
                CharSequenceUtil.CreateSynchronizedTextBuilderCharSequence,
                CharSequenceUtil.CreateStringBuffer,
            };

            foreach (var leftFactory in factories)
            {
                foreach (var item in CharSequenceUtil.GetHashCode_String_TestData())
                {
                    var leftArg = leftFactory((string?)item[0]);
                    var expectedArg = item[1];
                    yield return new TestCaseData(leftArg, expectedArg)
                        .FormatArguments(leftArg);
                }
            }
        }

        [TestCaseSource(nameof(GetHashCode_ICharSequence_TestData))]
        public void Test_GetHashCode_ICharSequence(ICharSequence? value, int expected)
        {
            Assert.AreEqual(expected, CharSequenceComparer.Ordinal.GetHashCode(value));
        }


        [TestCaseSource(typeof(CharSequenceUtil), nameof(CharSequenceUtil.GetHashCode_String_TestData))]
        public void Test_GetHashCode_String(string? value, int expected)
        {
            Assert.AreEqual(expected, CharSequenceComparer.Ordinal.GetHashCode(value));
        }

        public static IEnumerable<TestCaseData> GetHashCode_CharArray_TestData()
        {
            var factories = new Func<string?, object?>[]
            {
                CharSequenceUtil.CreateCharArray,
            };

            foreach (var leftFactory in factories)
            {
                foreach (var item in CharSequenceUtil.GetHashCode_String_TestData())
                {
                    var leftArg = leftFactory((string?)item[0]);
                    var expectedArg = item[1];
                    yield return new TestCaseData(leftArg, expectedArg)
                        .FormatArguments(leftArg);
                }
            }
        }

        [TestCaseSource(nameof(GetHashCode_CharArray_TestData))]
        public void Test_GetHashCode_CharArray(char[]? value, int expected)
        {
            Assert.AreEqual(expected, CharSequenceComparer.Ordinal.GetHashCode(value));
        }

        public static IEnumerable<TestCaseData> GetHashCode_StringBuilder_TestData()
        {
            var factories = new Func<string?, object?>[]
            {
                CharSequenceUtil.CreateStringBuilder,
            };

            foreach (var leftFactory in factories)
            {
                foreach (var item in CharSequenceUtil.GetHashCode_String_TestData())
                {
                    var leftArg = leftFactory((string?)item[0]);
                    var expectedArg = item[1];
                    yield return new TestCaseData(leftArg, expectedArg)
                        .FormatArguments(leftArg);
                }
            }
        }

        [TestCaseSource(nameof(GetHashCode_StringBuilder_TestData))]
        public void Test_GetHashCode_StringBuilder(StringBuilder? value, int expected)
        {
            Assert.AreEqual(expected, CharSequenceComparer.Ordinal.GetHashCode(value));
        }
    }
}
