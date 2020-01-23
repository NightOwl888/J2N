using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading;

namespace J2N.Text
{
    [TestFixture]
    public class TestCharSequenceComparer
    {
        private static readonly string String1 = "This is a portriat of a Turkish czar";
        private static readonly string String2 = "This is not an equal string";
        private static readonly string String3 = String1 + String2;
        private static readonly string String4 = String1.Substring(0, String1.Length - 10);

        private static readonly char[] CharArray1 = String1.ToCharArray();
        private static readonly char[] CharArray2 = String2.ToCharArray();
        private static readonly char[] CharArray3 = String3.ToCharArray();
        private static readonly char[] CharArray4 = String4.ToCharArray();

        private static readonly StringBuilder StringBuilder1 = new StringBuilder(String1);
        private static readonly StringBuilder StringBuilder2 = new StringBuilder(String2);
        private static readonly StringBuilder StringBuilder3 = new StringBuilder(String3);
        private static readonly StringBuilder StringBuilder4 = new StringBuilder(String4);

        private CultureInfo originalCulture;
        private ICharSequence target;
        private ICharSequence nullTarget;
        private ICharSequence equalTarget;
        private ICharSequence unequalTarget;
        private ICharSequence emptyTarget;

        [SetUp]
        public virtual void SetUp()
        {
            originalCulture = CultureInfo.CurrentCulture;
#if !NETSTANDARD
            Thread.CurrentThread.CurrentCulture
#else
            CultureInfo.CurrentCulture
#endif
                 = new CultureInfo("tr-TR");

            target = new CharArrayCharSequence(CharArray1);
            nullTarget = new CharArrayCharSequence(null);
            equalTarget = new CharArrayCharSequence(CharArray1);
            unequalTarget = new CharArrayCharSequence(CharArray2);
            emptyTarget = new CharArrayCharSequence(string.Empty.ToCharArray());
        }

        [TearDown]
        public virtual void TearDown()
        {
#if !NETSTANDARD
            Thread.CurrentThread.CurrentCulture
#else
            CultureInfo.CurrentCulture
#endif
                = originalCulture;
        }

        [Test]
        public void TestCompare()
        {
            Assert.AreEqual(0, CharSequenceComparer.Ordinal.Compare(target, String1));
            Assert.AreEqual(0, CharSequenceComparer.Ordinal.Compare(target, new StringBuilder(String1)));
            Assert.AreEqual(0, CharSequenceComparer.Ordinal.Compare(target, String1.ToCharArray()));
            Assert.AreEqual(0, CharSequenceComparer.Ordinal.Compare(target, String1.AsCharSequence()));
            Assert.AreEqual(0, CharSequenceComparer.Ordinal.Compare(target, new StringBuilderCharSequence(new StringBuilder(String1))));
            Assert.AreEqual(0, CharSequenceComparer.Ordinal.Compare(target, new CharArrayCharSequence(String1.ToCharArray())));

            Assert.Greater(0, CharSequenceComparer.Ordinal.Compare(target, String2));
            Assert.Greater(0, CharSequenceComparer.Ordinal.Compare(target, new StringBuilder(String2)));
            Assert.Greater(0, CharSequenceComparer.Ordinal.Compare(target, String2.ToCharArray()));
            Assert.Greater(0, CharSequenceComparer.Ordinal.Compare(target, String2.AsCharSequence()));
            Assert.Greater(0, CharSequenceComparer.Ordinal.Compare(target, new StringBuilderCharSequence(new StringBuilder(String2))));
            Assert.Greater(0, CharSequenceComparer.Ordinal.Compare(target, new CharArrayCharSequence(String2.ToCharArray())));

            Assert.Greater(0, CharSequenceComparer.Ordinal.Compare(target, String3));
            Assert.Greater(0, CharSequenceComparer.Ordinal.Compare(target, new StringBuilder(String3)));
            Assert.Greater(0, CharSequenceComparer.Ordinal.Compare(target, String3.ToCharArray()));
            Assert.Greater(0, CharSequenceComparer.Ordinal.Compare(target, String3.AsCharSequence()));
            Assert.Greater(0, CharSequenceComparer.Ordinal.Compare(target, new StringBuilderCharSequence(new StringBuilder(String3))));
            Assert.Greater(0, CharSequenceComparer.Ordinal.Compare(target, new CharArrayCharSequence(String3.ToCharArray())));

            Assert.Less(0, CharSequenceComparer.Ordinal.Compare(target, String4));
            Assert.Less(0, CharSequenceComparer.Ordinal.Compare(target, new StringBuilder(String4)));
            Assert.Less(0, CharSequenceComparer.Ordinal.Compare(target, String4.ToCharArray()));
            Assert.Less(0, CharSequenceComparer.Ordinal.Compare(target, String4.AsCharSequence()));
            Assert.Less(0, CharSequenceComparer.Ordinal.Compare(target, new StringBuilderCharSequence(new StringBuilder(String4))));
            Assert.Less(0, CharSequenceComparer.Ordinal.Compare(target, new CharArrayCharSequence(String4.ToCharArray())));

            Assert.Greater(0, CharSequenceComparer.Ordinal.Compare(nullTarget, String1));
            Assert.Less(0, CharSequenceComparer.Ordinal.Compare(target, (string)null));
            Assert.Less(0, CharSequenceComparer.Ordinal.Compare(target, (StringBuilder)null));
            Assert.Less(0, CharSequenceComparer.Ordinal.Compare(target, (char[])null));
        }

        [Test]
        public void TestEquals()
        {
            Assert.IsTrue(CharSequenceComparer.Ordinal.Equals(target, String1));
            Assert.IsTrue(CharSequenceComparer.Ordinal.Equals(target, String1.ToCharArray()));
            Assert.IsTrue(CharSequenceComparer.Ordinal.Equals(target, new StringBuilder(String1)));
            Assert.IsTrue(CharSequenceComparer.Ordinal.Equals(target, new StringCharSequence(String1)));
            Assert.IsTrue(CharSequenceComparer.Ordinal.Equals(target, new StringBuilderCharSequence(new StringBuilder(String1))));
            Assert.IsTrue(CharSequenceComparer.Ordinal.Equals(target, new CharArrayCharSequence(String1.ToCharArray())));

            Assert.IsFalse(CharSequenceComparer.Ordinal.Equals(target, String2));
            Assert.IsFalse(CharSequenceComparer.Ordinal.Equals(target, String2.ToCharArray()));
            Assert.IsFalse(CharSequenceComparer.Ordinal.Equals(target, new StringBuilder(String2)));
            Assert.IsFalse(CharSequenceComparer.Ordinal.Equals(target, new StringCharSequence(String2)));
            Assert.IsFalse(CharSequenceComparer.Ordinal.Equals(target, new StringBuilderCharSequence(new StringBuilder(String2))));
            Assert.IsFalse(CharSequenceComparer.Ordinal.Equals(target, new CharArrayCharSequence(String2.ToCharArray())));


            Assert.IsTrue(CharSequenceComparer.Ordinal.Equals(target, (object)String1));
            Assert.IsTrue(CharSequenceComparer.Ordinal.Equals(target, (object)String1.ToCharArray()));
            Assert.IsTrue(CharSequenceComparer.Ordinal.Equals(target, (object)new StringBuilder(String1)));
            Assert.IsTrue(CharSequenceComparer.Ordinal.Equals(target, (object)new StringCharSequence(String1)));
            Assert.IsTrue(CharSequenceComparer.Ordinal.Equals(target, (object)new StringBuilderCharSequence(new StringBuilder(String1))));
            Assert.IsTrue(CharSequenceComparer.Ordinal.Equals(target, (object)new CharArrayCharSequence(String1.ToCharArray())));

            Assert.IsFalse(CharSequenceComparer.Ordinal.Equals(target, (object)String2));
            Assert.IsFalse(CharSequenceComparer.Ordinal.Equals(target, (object)String2.ToCharArray()));
            Assert.IsFalse(CharSequenceComparer.Ordinal.Equals(target, (object)new StringBuilder(String2)));
            Assert.IsFalse(CharSequenceComparer.Ordinal.Equals(target, (object)new StringCharSequence(String2)));
            Assert.IsFalse(CharSequenceComparer.Ordinal.Equals(target, (object)new StringBuilderCharSequence(new StringBuilder(String2))));
            Assert.IsFalse(CharSequenceComparer.Ordinal.Equals(target, (object)new CharArrayCharSequence(String2.ToCharArray())));
        }

        [Test]
        public virtual void TestGetHashCode()
        {
            Assert.AreEqual(CharSequenceComparer.Ordinal.GetHashCode(String1), CharSequenceComparer.Ordinal.GetHashCode(target));
            Assert.AreEqual(new StringBuilderCharSequence(new StringBuilder(String1)).GetHashCode(), CharSequenceComparer.Ordinal.GetHashCode(target));
            Assert.AreEqual(new CharArrayCharSequence(String1.ToCharArray()).GetHashCode(), CharSequenceComparer.Ordinal.GetHashCode(target));

            Assert.AreEqual(CharSequenceComparer.Ordinal.GetHashCode(String1), CharSequenceComparer.Ordinal.GetHashCode(CharArray1));
            Assert.AreEqual(new StringBuilderCharSequence(new StringBuilder(String1)).GetHashCode(), CharSequenceComparer.Ordinal.GetHashCode(CharArray1));
            Assert.AreEqual(new CharArrayCharSequence(String1.ToCharArray()).GetHashCode(), CharSequenceComparer.Ordinal.GetHashCode(CharArray1));

            Assert.AreEqual(CharSequenceComparer.Ordinal.GetHashCode(String1), CharSequenceComparer.Ordinal.GetHashCode(StringBuilder1));
            Assert.AreEqual(new StringBuilderCharSequence(new StringBuilder(String1)).GetHashCode(), CharSequenceComparer.Ordinal.GetHashCode(StringBuilder1));
            Assert.AreEqual(new CharArrayCharSequence(String1.ToCharArray()).GetHashCode(), CharSequenceComparer.Ordinal.GetHashCode(StringBuilder1));

            Assert.AreEqual(CharSequenceComparer.Ordinal.GetHashCode(String1), CharSequenceComparer.Ordinal.GetHashCode(String1));
            Assert.AreEqual(new StringBuilderCharSequence(new StringBuilder(String1)).GetHashCode(), CharSequenceComparer.Ordinal.GetHashCode(String1));
            Assert.AreEqual(new CharArrayCharSequence(String1.ToCharArray()).GetHashCode(), CharSequenceComparer.Ordinal.GetHashCode(String1));

            Assert.AreNotEqual(CharSequenceComparer.Ordinal.GetHashCode(String2), CharSequenceComparer.Ordinal.GetHashCode(target));
            Assert.AreNotEqual(new StringBuilderCharSequence(new StringBuilder(String2)).GetHashCode(), CharSequenceComparer.Ordinal.GetHashCode(target));
            Assert.AreNotEqual(new CharArrayCharSequence(String2.ToCharArray()).GetHashCode(), CharSequenceComparer.Ordinal.GetHashCode(target));

            Assert.AreNotEqual(CharSequenceComparer.Ordinal.GetHashCode(String2), CharSequenceComparer.Ordinal.GetHashCode(CharArray1));
            Assert.AreNotEqual(new StringBuilderCharSequence(new StringBuilder(String2)).GetHashCode(), CharSequenceComparer.Ordinal.GetHashCode(CharArray1));
            Assert.AreNotEqual(new CharArrayCharSequence(String2.ToCharArray()).GetHashCode(), CharSequenceComparer.Ordinal.GetHashCode(CharArray1));

            Assert.AreNotEqual(CharSequenceComparer.Ordinal.GetHashCode(String2), CharSequenceComparer.Ordinal.GetHashCode(StringBuilder1));
            Assert.AreNotEqual(new StringBuilderCharSequence(new StringBuilder(String2)).GetHashCode(), CharSequenceComparer.Ordinal.GetHashCode(StringBuilder1));
            Assert.AreNotEqual(new CharArrayCharSequence(String2.ToCharArray()).GetHashCode(), CharSequenceComparer.Ordinal.GetHashCode(StringBuilder1));

            Assert.AreNotEqual(CharSequenceComparer.Ordinal.GetHashCode(String2), CharSequenceComparer.Ordinal.GetHashCode(String1));
            Assert.AreNotEqual(new StringBuilderCharSequence(new StringBuilder(String2)).GetHashCode(), CharSequenceComparer.Ordinal.GetHashCode(String1));
            Assert.AreNotEqual(new CharArrayCharSequence(String2.ToCharArray()).GetHashCode(), CharSequenceComparer.Ordinal.GetHashCode(String1));

            Assert.AreEqual(0, CharSequenceComparer.Ordinal.GetHashCode(emptyTarget));
            Assert.AreEqual(0, CharSequenceComparer.Ordinal.GetHashCode(new char[0]));
            Assert.AreEqual(0, CharSequenceComparer.Ordinal.GetHashCode(new StringBuilder()));
            Assert.AreEqual(0, CharSequenceComparer.Ordinal.GetHashCode(string.Empty));

            Assert.AreEqual(int.MaxValue, CharSequenceComparer.Ordinal.GetHashCode(nullTarget));
            Assert.AreEqual(int.MaxValue, CharSequenceComparer.Ordinal.GetHashCode((char[])null));
            Assert.AreEqual(int.MaxValue, CharSequenceComparer.Ordinal.GetHashCode((StringBuilder)null));
            Assert.AreEqual(int.MaxValue, CharSequenceComparer.Ordinal.GetHashCode((string)null));
        }
    }
}
