using J2N.Buffers;
using NUnit.Framework;
using System;
using System.Text;
#nullable enable

namespace J2N.Text
{
    public class TestMutableTextBufferCharSequence : CharSequenceTestBase<MutableTextBufferCharSequence>
    {
        public override void SetUp()
        {
            base.SetUp();

            target = new MutableTextBufferCharSequence(new MutableTextBuffer(ArrayAllocator<char>.Default).Initialize(CharArray1));
            nullTarget = new MutableTextBufferCharSequence(null);
            equalTarget = new MutableTextBufferCharSequence(new MutableTextBuffer(ArrayAllocator<char>.Default).Initialize(CharArray1));
            unequalTarget = new MutableTextBufferCharSequence(new MutableTextBuffer(ArrayAllocator<char>.Default).Initialize(CharArray2));
            emptyTarget = new MutableTextBufferCharSequence(new MutableTextBuffer(ArrayAllocator<char>.Default).Initialize());
        }

        protected override int CompareToString(MutableTextBufferCharSequence target, string? value) => target.CompareTo(value);
        protected override int CompareToCharArray(MutableTextBufferCharSequence target, char[]? value) => target.CompareTo(value);
        protected override int CompareToStringBuilder(MutableTextBufferCharSequence target, StringBuilder? value) => target.CompareTo(value);
        protected override int CompareToReadOnlySpan(MutableTextBufferCharSequence target, ReadOnlySpan<char> value) => target.CompareTo(value);
        protected override int CompareToObject(MutableTextBufferCharSequence target, object? value) => target.CompareTo(value);

        protected override bool EqualsString(MutableTextBufferCharSequence target, string? value) => target.Equals(value);
        protected override bool EqualsCharArray(MutableTextBufferCharSequence target, char[]? value) => target.Equals(value);
        protected override bool EqualsStringBuilder(MutableTextBufferCharSequence target, StringBuilder? value) => target.Equals(value);
        protected override bool EqualsReadOnlySpan(MutableTextBufferCharSequence target, ReadOnlySpan<char> value) => target.Equals(value);

        [Test]
        public void TestValue()
        {
            Assert.IsNotNull(target.Value);
            Assert.AreEqual(String1, target.Value!.ToString());

            Assert.IsNull(nullTarget.Value);
        }

        //[Test]
        //public virtual void TestEqualityOperators()
        //{
        //    Assert.IsTrue(target == equalTarget);
        //    Assert.IsTrue(equalTarget == target);

        //    Assert.IsFalse(target == unequalTarget);
        //    Assert.IsFalse(unequalTarget == target);

        //    Assert.IsTrue(target == CharArray1);
        //    Assert.IsTrue(CharArray1 == target);

        //    Assert.IsFalse(target == CharArray2);
        //    Assert.IsFalse(CharArray2 == target);

        //    Assert.IsTrue(nullTarget == (char[])null);
        //    Assert.IsTrue((char[])null == nullTarget);


        //    Assert.IsFalse(target != equalTarget);
        //    Assert.IsFalse(equalTarget != target);

        //    Assert.IsTrue(target != unequalTarget);
        //    Assert.IsTrue(unequalTarget != target);

        //    Assert.IsTrue(target != CharArray2);
        //    Assert.IsTrue(CharArray2 != target);

        //    Assert.IsFalse(target != CharArray1);
        //    Assert.IsFalse(CharArray1 != target);

        //    Assert.IsFalse(nullTarget != (char[])null);
        //    Assert.IsFalse((char[])null != nullTarget);
        //}
    }
}
