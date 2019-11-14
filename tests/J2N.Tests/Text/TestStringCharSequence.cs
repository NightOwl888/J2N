using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading;

namespace J2N.Text
{
    public class TestStringCharSequence : CharSequenceTestBase<StringCharSequence>
    {
        public override void SetUp()
        {
            base.SetUp();

            target = new StringCharSequence(String1);
            nullTarget = new StringCharSequence(null);
            equalTarget = new StringCharSequence(String1);
            unequalTarget = new StringCharSequence(String2);
            emptyTarget = new StringCharSequence(string.Empty);
        }


        [Test]
        public void TestValue()
        {
            Assert.IsNotNull(target.Value);
            Assert.AreEqual(String1, target.Value);

            Assert.IsNull(nullTarget.Value);
        }

        [Test]
        public virtual void TestReferenceEquality()
        {
            Assert.IsTrue(target == equalTarget);
            Assert.IsTrue(equalTarget == target);

            Assert.IsFalse(target == unequalTarget);
            Assert.IsFalse(unequalTarget == target);

            Assert.IsTrue(target == String1);
            Assert.IsTrue(String1 == target);

            Assert.IsFalse(target == String2);
            Assert.IsFalse(String2 == target);

            Assert.IsTrue(nullTarget == (string)null);
            Assert.IsTrue((string)null == nullTarget);


            Assert.IsFalse(target != equalTarget);
            Assert.IsFalse(equalTarget != target);

            Assert.IsTrue(target != unequalTarget);
            Assert.IsTrue(unequalTarget != target);

            Assert.IsTrue(target != String2);
            Assert.IsTrue(String2 != target);

            Assert.IsFalse(target != String1);
            Assert.IsFalse(String1 != target);

            Assert.IsFalse(nullTarget != (string)null);
            Assert.IsFalse((string)null != nullTarget);
        }


        //        public static readonly string String1 = "This is a portriat of a Turkish czar";
        //        public static readonly string String2 = "This is not an equal string";
        //        public static readonly string String3 = String1 + String2;
        //        public static readonly string String4 = String1.Substring(0, String1.Length - 10);

        //        private CultureInfo originalCulture;
        //        private StringCharSequence target;
        //        private StringCharSequence nullTarget;
        //        private StringCharSequence equalTarget;
        //        private StringCharSequence unequalTarget;
        //        private StringCharSequence emptyTarget;

        //        [SetUp]
        //        public void SetUp()
        //        {
        //            originalCulture = CultureInfo.CurrentCulture;
        //#if !NETSTANDARD
        //            Thread.CurrentThread.CurrentCulture
        //#else
        //            CultureInfo.CurrentCulture
        //#endif
        //                 = new CultureInfo("tr-TR");

        //            target = new StringCharSequence(String1);
        //            nullTarget = new StringCharSequence(null);
        //            equalTarget = new StringCharSequence(String1);
        //            unequalTarget = new StringCharSequence(String2);
        //            emptyTarget = new StringCharSequence(string.Empty);
        //        }

        //        [TearDown]
        //        public void TearDown()
        //        {
        //#if !NETSTANDARD
        //            Thread.CurrentThread.CurrentCulture
        //#else
        //            CultureInfo.CurrentCulture
        //#endif
        //                = originalCulture;
        //        }



        //        [Test]
        //        public void TestValue()
        //        {
        //            Assert.IsNotNull(target.Value);
        //            Assert.AreEqual(String1, target.Value);

        //            Assert.IsNull(nullTarget.Value);
        //        }

        //        [Test]
        //        public void TestHasValue()
        //        {
        //            Assert.IsTrue(target.HasValue);
        //            Assert.IsTrue(emptyTarget.HasValue);
        //            Assert.IsFalse(nullTarget.HasValue);
        //        }

        //        [Test]
        //        public void TestIndexer()
        //        {
        //            Assert.AreEqual('p', target[10]);

        //            Assert.Throws<IndexOutOfRangeException>(() => { var x = target[String1.Length + 1]; });
        //            Assert.Throws<IndexOutOfRangeException>(() => { var x = emptyTarget[0]; });
        //            Assert.Throws<InvalidOperationException>(() => { var x = nullTarget[10]; });
        //        }

        //        [Test]
        //        public void TestLength()
        //        {
        //            Assert.AreEqual(String1.Length, target.Length);

        //            Assert.AreEqual(0, emptyTarget.Length);
        //            Assert.AreEqual(0, nullTarget.Length);
        //        }

        //        [Test]
        //        public void TestSubsequence()
        //        {
        //            Assert.AreEqual("s a portri", target.Subsequence(6, 10).ToString()); // Substring
        //            Assert.AreEqual(String1, target.Subsequence(0, String1.Length)); // Full string
        //            Assert.AreEqual(string.Empty, target.Subsequence(10, 0)); // Zero length
        //            Assert.Throws<ArgumentOutOfRangeException>(() => target.Subsequence(-1, 10));
        //            Assert.Throws<ArgumentOutOfRangeException>(() => target.Subsequence(3, -2));
        //            Assert.Throws<ArgumentOutOfRangeException>(() => target.Subsequence(String1.Length, 1));

        //            Assert.Throws<ArgumentOutOfRangeException>(() => emptyTarget.Subsequence(0, 1));

        //            Assert.IsFalse(nullTarget.Subsequence(6, 10).HasValue); // Null target will always return null subsequence
        //        }

        //        [Test]
        //        public void TestToString()
        //        {
        //            Assert.AreEqual(String1, target.ToString());
        //            Assert.AreEqual(string.Empty, emptyTarget.ToString());
        //            Assert.AreEqual(string.Empty, nullTarget.ToString());
        //        }

        //        [Test]
        //        public void TestReferenceEquality()
        //        {
        //            Assert.IsTrue(target == equalTarget);
        //            Assert.IsTrue(equalTarget == target);

        //            Assert.IsFalse(target == unequalTarget);
        //            Assert.IsFalse(unequalTarget == target);

        //            Assert.IsTrue(target == String1);
        //            Assert.IsTrue(String1 == target);

        //            Assert.IsFalse(target == String2);
        //            Assert.IsFalse(String2 == target);

        //            Assert.IsTrue(nullTarget == (string)null);
        //            Assert.IsTrue((string)null == nullTarget);


        //            Assert.IsFalse(target != equalTarget);
        //            Assert.IsFalse(equalTarget != target);

        //            Assert.IsTrue(target != unequalTarget);
        //            Assert.IsTrue(unequalTarget != target);

        //            Assert.IsTrue(target != String2);
        //            Assert.IsTrue(String2 != target);

        //            Assert.IsFalse(target != String1);
        //            Assert.IsFalse(String1 != target);

        //            Assert.IsFalse(nullTarget != (string)null);
        //            Assert.IsFalse((string)null != nullTarget);
        //        }

        //        [Test]
        //        public void TestEquals()
        //        {
        //            Assert.IsTrue(target.Equals(String1));
        //            Assert.IsTrue(target.Equals(String1.ToCharArray()));
        //            Assert.IsTrue(target.Equals(new StringBuilder(String1)));
        //            Assert.IsTrue(target.Equals(new StringCharSequence(String1)));
        //            Assert.IsTrue(target.Equals(new StringBuilderCharSequence(new StringBuilder(String1))));
        //            Assert.IsTrue(target.Equals(new CharArrayCharSequence(String1.ToCharArray())));

        //            Assert.IsFalse(target.Equals(String2));
        //            Assert.IsFalse(target.Equals(String2.ToCharArray()));
        //            Assert.IsFalse(target.Equals(new StringBuilder(String2)));
        //            Assert.IsFalse(target.Equals(new StringCharSequence(String2)));
        //            Assert.IsFalse(target.Equals(new StringBuilderCharSequence(new StringBuilder(String2))));
        //            Assert.IsFalse(target.Equals(new CharArrayCharSequence(String2.ToCharArray())));


        //            Assert.IsTrue(target.Equals((object)String1));
        //            Assert.IsTrue(target.Equals((object)String1.ToCharArray()));
        //            Assert.IsTrue(target.Equals((object)new StringBuilder(String1)));
        //            Assert.IsTrue(target.Equals((object)new StringCharSequence(String1)));
        //            Assert.IsTrue(target.Equals((object)new StringBuilderCharSequence(new StringBuilder(String1))));
        //            Assert.IsTrue(target.Equals((object)new CharArrayCharSequence(String1.ToCharArray())));

        //            Assert.IsFalse(target.Equals((object)String2));
        //            Assert.IsFalse(target.Equals((object)String2.ToCharArray()));
        //            Assert.IsFalse(target.Equals((object)new StringBuilder(String2)));
        //            Assert.IsFalse(target.Equals((object)new StringCharSequence(String2)));
        //            Assert.IsFalse(target.Equals((object)new StringBuilderCharSequence(new StringBuilder(String2))));
        //            Assert.IsFalse(target.Equals((object)new CharArrayCharSequence(String2.ToCharArray())));
        //        }

        //        [Test]
        //        public void TestGetHashCode()
        //        {
        //            Assert.AreEqual(CharSequenceComparer.Ordinal.GetHashCode(String1), target.GetHashCode());
        //            Assert.AreEqual(new StringBuilderCharSequence(new StringBuilder(String1)).GetHashCode(), target.GetHashCode());
        //            Assert.AreEqual(new CharArrayCharSequence(String1.ToCharArray()).GetHashCode(), target.GetHashCode());

        //            Assert.AreNotEqual(CharSequenceComparer.Ordinal.GetHashCode(String2), target.GetHashCode());
        //            Assert.AreNotEqual(new StringBuilderCharSequence(new StringBuilder(String2)).GetHashCode(), target.GetHashCode());
        //            Assert.AreNotEqual(new CharArrayCharSequence(String2.ToCharArray()).GetHashCode(), target.GetHashCode());

        //            Assert.AreEqual(0, emptyTarget.GetHashCode());
        //            Assert.AreEqual(int.MaxValue, nullTarget.GetHashCode());
        //        }

        //        [Test]
        //        public void TestCompareTo()
        //        {
        //            Assert.AreEqual(0, target.CompareTo(String1));
        //            Assert.AreEqual(0, target.CompareTo(new StringBuilder(String1)));
        //            Assert.AreEqual(0, target.CompareTo(String1.ToCharArray()));
        //            Assert.AreEqual(0, target.CompareTo(String1.ToCharSequence()));
        //            Assert.AreEqual(0, target.CompareTo(new StringBuilderCharSequence(new StringBuilder(String1))));
        //            Assert.AreEqual(0, target.CompareTo(new CharArrayCharSequence(String1.ToCharArray())));

        //            Assert.Greater(0, target.CompareTo(String2));
        //            Assert.Greater(0, target.CompareTo(new StringBuilder(String2)));
        //            Assert.Greater(0, target.CompareTo(String2.ToCharArray()));
        //            Assert.Greater(0, target.CompareTo(String2.ToCharSequence()));
        //            Assert.Greater(0, target.CompareTo(new StringBuilderCharSequence(new StringBuilder(String2))));
        //            Assert.Greater(0, target.CompareTo(new CharArrayCharSequence(String2.ToCharArray())));

        //            Assert.Greater(0, target.CompareTo(String3));
        //            Assert.Greater(0, target.CompareTo(new StringBuilder(String3)));
        //            Assert.Greater(0, target.CompareTo(String3.ToCharArray()));
        //            Assert.Greater(0, target.CompareTo(String3.ToCharSequence()));
        //            Assert.Greater(0, target.CompareTo(new StringBuilderCharSequence(new StringBuilder(String3))));
        //            Assert.Greater(0, target.CompareTo(new CharArrayCharSequence(String3.ToCharArray())));

        //            Assert.Less(0, target.CompareTo(String4));
        //            Assert.Less(0, target.CompareTo(new StringBuilder(String4)));
        //            Assert.Less(0, target.CompareTo(String4.ToCharArray()));
        //            Assert.Less(0, target.CompareTo(String4.ToCharSequence()));
        //            Assert.Less(0, target.CompareTo(new StringBuilderCharSequence(new StringBuilder(String4))));
        //            Assert.Less(0, target.CompareTo(new CharArrayCharSequence(String4.ToCharArray())));

        //            Assert.Greater(0, nullTarget.CompareTo(String1));
        //            Assert.Less(0, target.CompareTo((string)null));
        //            Assert.Less(0, target.CompareTo((StringBuilder)null));
        //            Assert.Less(0, target.CompareTo((char[])null));
        //        }
    }
}
