using NUnit.Framework;
using System;
using System.Text;
#nullable enable

namespace J2N.Text
{
    internal class TestCharArrayCharSequence : CharSequenceTestBase<CharArrayCharSequence>
    {
        public override void SetUp()
        {
            base.SetUp();

            target = new CharArrayCharSequence(CharArray1);
            nullTarget = new CharArrayCharSequence(null);
            equalTarget = new CharArrayCharSequence(CharArray1);
            unequalTarget = new CharArrayCharSequence(CharArray2);
            emptyTarget = new CharArrayCharSequence(string.Empty.ToCharArray());
        }

        protected override int CompareToString(CharArrayCharSequence target, string? value) => target.CompareTo(value);
        protected override int CompareToCharArray(CharArrayCharSequence target, char[]? value) => target.CompareTo(value);
        protected override int CompareToStringBuilder(CharArrayCharSequence target, StringBuilder? value) => target.CompareTo(value);
        protected override int CompareToReadOnlySpan(CharArrayCharSequence target, ReadOnlySpan<char> value) => target.CompareTo(value);
        protected override int CompareToObject(CharArrayCharSequence target, object? value) => target.CompareTo(value);

        protected override bool EqualsString(CharArrayCharSequence target, string? value) => target.Equals(value);
        protected override bool EqualsCharArray(CharArrayCharSequence target, char[]? value) => target.Equals(value);
        protected override bool EqualsStringBuilder(CharArrayCharSequence target, StringBuilder? value) => target.Equals(value);
        protected override bool EqualsReadOnlySpan(CharArrayCharSequence target, ReadOnlySpan<char> value) => target.Equals(value);


        [Test]
        public void TestValue()
        {
            Assert.IsNotNull(target.Value);
            Assert.AreEqual(CharArray1, target.Value);

            Assert.IsNull(nullTarget.Value);
        }

        [Test]
        public virtual void TestEqualityOperators()
        {
            Assert.IsTrue(target == equalTarget);
            Assert.IsTrue(equalTarget == target);

            Assert.IsFalse(target == unequalTarget);
            Assert.IsFalse(unequalTarget == target);

            Assert.IsTrue(target == CharArray1);
            Assert.IsTrue(CharArray1 == target);

            Assert.IsFalse(target == CharArray2);
            Assert.IsFalse(CharArray2 == target);

            Assert.IsTrue(nullTarget == (char[]?)null);
            Assert.IsTrue((char[]?)null == nullTarget);


            Assert.IsFalse(target != equalTarget);
            Assert.IsFalse(equalTarget != target);

            Assert.IsTrue(target != unequalTarget);
            Assert.IsTrue(unequalTarget != target);

            Assert.IsTrue(target != CharArray2);
            Assert.IsTrue(CharArray2 != target);

            Assert.IsFalse(target != CharArray1);
            Assert.IsFalse(CharArray1 != target);

            Assert.IsFalse(nullTarget != (char[]?)null);
            Assert.IsFalse((char[]?)null != nullTarget);
        }
    }
}
