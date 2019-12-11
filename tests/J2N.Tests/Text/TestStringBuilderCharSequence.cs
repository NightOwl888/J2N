using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace J2N.Text
{
    public class TestStringBuilderCharSequence : CharSequenceTestBase<StringBuilderCharSequence>
    {
        public override void SetUp()
        {
            base.SetUp();

            target = new StringBuilderCharSequence(StringBuilder1);
            nullTarget = new StringBuilderCharSequence(null);
            equalTarget = new StringBuilderCharSequence(StringBuilder1);
            unequalTarget = new StringBuilderCharSequence(StringBuilder2);
            emptyTarget = new StringBuilderCharSequence(new StringBuilder());
        }


        [Test]
        public void TestValue()
        {
            Assert.IsNotNull(target.Value);
            Assert.AreEqual(StringBuilder1, target.Value);

            Assert.IsNull(nullTarget.Value);
        }

        [Test]
        public virtual void TestEqualityOperators()
        {
            Assert.IsTrue(target == equalTarget);
            Assert.IsTrue(equalTarget == target);

            Assert.IsFalse(target == unequalTarget);
            Assert.IsFalse(unequalTarget == target);

            Assert.IsTrue(target == StringBuilder1);
            Assert.IsTrue(StringBuilder1 == target);

            Assert.IsFalse(target == StringBuilder2);
            Assert.IsFalse(StringBuilder2 == target);

            Assert.IsTrue(nullTarget == (StringBuilder)null);
            Assert.IsTrue((StringBuilder)null == nullTarget);


            Assert.IsFalse(target != equalTarget);
            Assert.IsFalse(equalTarget != target);

            Assert.IsTrue(target != unequalTarget);
            Assert.IsTrue(unequalTarget != target);

            Assert.IsTrue(target != StringBuilder2);
            Assert.IsTrue(StringBuilder2 != target);

            Assert.IsFalse(target != StringBuilder1);
            Assert.IsFalse(StringBuilder1 != target);

            Assert.IsFalse(nullTarget != (StringBuilder)null);
            Assert.IsFalse((StringBuilder)null != nullTarget);
        }
    }
}
