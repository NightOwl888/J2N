using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace J2N.Text
{
    public class TestCharArrayCharSequence : CharSequenceTestBase<CharArrayCharSequence>
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


        [Test]
        public void TestValue()
        {
            Assert.IsNotNull(target.Value);
            Assert.AreEqual(CharArray1, target.Value);

            Assert.IsNull(nullTarget.Value);
        }

        [Test]
        public virtual void TestReferenceEquality()
        {
            Assert.IsTrue(target == equalTarget);
            Assert.IsTrue(equalTarget == target);

            Assert.IsFalse(target == unequalTarget);
            Assert.IsFalse(unequalTarget == target);

            Assert.IsTrue(target == CharArray1);
            Assert.IsTrue(CharArray1 == target);

            Assert.IsFalse(target == CharArray2);
            Assert.IsFalse(CharArray2 == target);

            Assert.IsTrue(nullTarget == (char[])null);
            Assert.IsTrue((char[])null == nullTarget);


            Assert.IsFalse(target != equalTarget);
            Assert.IsFalse(equalTarget != target);

            Assert.IsTrue(target != unequalTarget);
            Assert.IsTrue(unequalTarget != target);

            Assert.IsTrue(target != CharArray2);
            Assert.IsTrue(CharArray2 != target);

            Assert.IsFalse(target != CharArray1);
            Assert.IsFalse(CharArray1 != target);

            Assert.IsFalse(nullTarget != (char[])null);
            Assert.IsFalse((char[])null != nullTarget);
        }
    }
}
