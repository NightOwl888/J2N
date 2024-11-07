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
        public virtual void TestEqualityOperators()
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
    }
}
