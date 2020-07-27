using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading;

namespace J2N.Text
{
    [TestFixture]
    public abstract class CharSequenceTestBase<T> 
        where T: ICharSequence, 
        IComparable<ICharSequence>, IComparable, 
        IComparable<char[]>, IComparable<StringBuilder>, IComparable<string>
    {
        protected static readonly string String1 = "This is a portriat of a Turkish czar";
        protected static readonly string String2 = "This is not an equal string";
        protected static readonly string String3 = String1 + String2;
        protected static readonly string String4 = String1.Substring(0, String1.Length - 10);

        protected static readonly char[] CharArray1 = String1.ToCharArray();
        protected static readonly char[] CharArray2 = String2.ToCharArray();
        protected static readonly char[] CharArray3 = String3.ToCharArray();
        protected static readonly char[] CharArray4 = String4.ToCharArray();

        protected static readonly StringBuilder StringBuilder1 = new StringBuilder(String1);
        protected static readonly StringBuilder StringBuilder2 = new StringBuilder(String2);
        protected static readonly StringBuilder StringBuilder3 = new StringBuilder(String3);
        protected static readonly StringBuilder StringBuilder4 = new StringBuilder(String4);

        protected CultureInfo originalCulture;
        protected T target;
        protected T nullTarget;
        protected T equalTarget;
        protected T unequalTarget;
        protected T emptyTarget;

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



        [Test]
        public virtual void TestHasValue()
        {
            Assert.IsTrue(target.HasValue);
            Assert.IsTrue(emptyTarget.HasValue);
            Assert.IsFalse(nullTarget.HasValue);
        }

        [Test]
        public virtual void TestIndexer()
        {
            Assert.AreEqual('p', target[10]);

            Assert.Throws<IndexOutOfRangeException>(() => { var x = target[String1.Length + 1]; });
            Assert.Throws<IndexOutOfRangeException>(() => { var x = emptyTarget[0]; });
            Assert.Throws<InvalidOperationException>(() => { var x = nullTarget[10]; });
        }

        [Test]
        public virtual void TestLength()
        {
            Assert.AreEqual(String1.Length, target.Length);

            Assert.AreEqual(0, emptyTarget.Length);
            Assert.AreEqual(0, nullTarget.Length);
        }

        [Test]
        public virtual void TestSubsequence()
        {
            Assert.AreEqual("s a portri", target.Subsequence(6, 10).ToString()); // Substring
            Assert.AreEqual(String1, target.Subsequence(0, String1.Length)); // Full string
            Assert.AreEqual(string.Empty, target.Subsequence(10, 0)); // Zero length
            Assert.Throws<ArgumentOutOfRangeException>(() => target.Subsequence(-1, 10));
            Assert.Throws<ArgumentOutOfRangeException>(() => target.Subsequence(3, -2));
            Assert.Throws<ArgumentOutOfRangeException>(() => target.Subsequence(String1.Length, 1));

            Assert.Throws<ArgumentOutOfRangeException>(() => emptyTarget.Subsequence(0, 1));

            Assert.IsFalse(nullTarget.Subsequence(6, 10).HasValue); // Null target will always return null subsequence
        }

        [Test]
        public virtual void TestToString()
        {
            Assert.AreEqual(String1, target.ToString());
            Assert.AreEqual(string.Empty, emptyTarget.ToString());
            Assert.AreEqual(string.Empty, nullTarget.ToString());
        }

        [Test]
        public virtual void TestEquals()
        {
            Assert.IsTrue(target.Equals(String1));
            Assert.IsTrue(target.Equals(String1.ToCharArray()));
            Assert.IsTrue(target.Equals(new StringBuilder(String1)));
            Assert.IsTrue(target.Equals(new StringCharSequence(String1)));
            Assert.IsTrue(target.Equals(new StringBuilderCharSequence(new StringBuilder(String1))));
            Assert.IsTrue(target.Equals(new CharArrayCharSequence(String1.ToCharArray())));

            Assert.IsFalse(target.Equals(String2));
            Assert.IsFalse(target.Equals(String2.ToCharArray()));
            Assert.IsFalse(target.Equals(new StringBuilder(String2)));
            Assert.IsFalse(target.Equals(new StringCharSequence(String2)));
            Assert.IsFalse(target.Equals(new StringBuilderCharSequence(new StringBuilder(String2))));
            Assert.IsFalse(target.Equals(new CharArrayCharSequence(String2.ToCharArray())));


            Assert.IsTrue(target.Equals((object)String1));
            Assert.IsTrue(target.Equals((object)String1.ToCharArray()));
            Assert.IsTrue(target.Equals((object)new StringBuilder(String1)));
            Assert.IsTrue(target.Equals((object)new StringCharSequence(String1)));
            Assert.IsTrue(target.Equals((object)new StringBuilderCharSequence(new StringBuilder(String1))));
            Assert.IsTrue(target.Equals((object)new CharArrayCharSequence(String1.ToCharArray())));

            Assert.IsFalse(target.Equals((object)String2));
            Assert.IsFalse(target.Equals((object)String2.ToCharArray()));
            Assert.IsFalse(target.Equals((object)new StringBuilder(String2)));
            Assert.IsFalse(target.Equals((object)new StringCharSequence(String2)));
            Assert.IsFalse(target.Equals((object)new StringBuilderCharSequence(new StringBuilder(String2))));
            Assert.IsFalse(target.Equals((object)new CharArrayCharSequence(String2.ToCharArray())));
        }

        [Test]
        public virtual void TestGetHashCode()
        {
            Assert.AreEqual(CharSequenceComparer.Ordinal.GetHashCode(String1), target.GetHashCode());
            Assert.AreEqual(new StringBuilderCharSequence(new StringBuilder(String1)).GetHashCode(), target.GetHashCode());
            Assert.AreEqual(new CharArrayCharSequence(String1.ToCharArray()).GetHashCode(), target.GetHashCode());

            Assert.AreNotEqual(CharSequenceComparer.Ordinal.GetHashCode(String2), target.GetHashCode());
            Assert.AreNotEqual(new StringBuilderCharSequence(new StringBuilder(String2)).GetHashCode(), target.GetHashCode());
            Assert.AreNotEqual(new CharArrayCharSequence(String2.ToCharArray()).GetHashCode(), target.GetHashCode());

            Assert.AreEqual(0, emptyTarget.GetHashCode());
            Assert.AreEqual(int.MaxValue, nullTarget.GetHashCode());
        }

        [Test]
        public virtual void TestCompareTo()
        {
            Assert.AreEqual(0, target.CompareTo(String1));
            Assert.AreEqual(0, target.CompareTo(new StringBuilder(String1)));
            Assert.AreEqual(0, target.CompareTo(String1.ToCharArray()));
            Assert.AreEqual(0, target.CompareTo(String1.AsCharSequence()));
            Assert.AreEqual(0, target.CompareTo(new StringBuilderCharSequence(new StringBuilder(String1))));
            Assert.AreEqual(0, target.CompareTo(new CharArrayCharSequence(String1.ToCharArray())));

            Assert.Greater(0, target.CompareTo(String2));
            Assert.Greater(0, target.CompareTo(new StringBuilder(String2)));
            Assert.Greater(0, target.CompareTo(String2.ToCharArray()));
            Assert.Greater(0, target.CompareTo(String2.AsCharSequence()));
            Assert.Greater(0, target.CompareTo(new StringBuilderCharSequence(new StringBuilder(String2))));
            Assert.Greater(0, target.CompareTo(new CharArrayCharSequence(String2.ToCharArray())));

            Assert.Greater(0, target.CompareTo(String3));
            Assert.Greater(0, target.CompareTo(new StringBuilder(String3)));
            Assert.Greater(0, target.CompareTo(String3.ToCharArray()));
            Assert.Greater(0, target.CompareTo(String3.AsCharSequence()));
            Assert.Greater(0, target.CompareTo(new StringBuilderCharSequence(new StringBuilder(String3))));
            Assert.Greater(0, target.CompareTo(new CharArrayCharSequence(String3.ToCharArray())));

            Assert.Less(0, target.CompareTo(String4));
            Assert.Less(0, target.CompareTo(new StringBuilder(String4)));
            Assert.Less(0, target.CompareTo(String4.ToCharArray()));
            Assert.Less(0, target.CompareTo(String4.AsCharSequence()));
            Assert.Less(0, target.CompareTo(new StringBuilderCharSequence(new StringBuilder(String4))));
            Assert.Less(0, target.CompareTo(new CharArrayCharSequence(String4.ToCharArray())));

            Assert.Greater(0, nullTarget.CompareTo(String1));
            Assert.Less(0, target.CompareTo((string)null));
            Assert.Less(0, target.CompareTo((StringBuilder)null));
            Assert.Less(0, target.CompareTo((char[])null));
        }
    }
}
