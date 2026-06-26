using NUnit.Framework;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
#nullable enable

namespace J2N.Text
{
    public class TestSynchronizedTextBuilderCharSequence : CharSequenceTestBase<SynchronizedTextBuilderCharSequence>
    {
        public override void SetUp()
        {
            base.SetUp();

            target = new SynchronizedTextBuilderCharSequence(new SynchronizedTextBuilder(String1));
            nullTarget = new SynchronizedTextBuilderCharSequence(null);
            equalTarget = new SynchronizedTextBuilderCharSequence(new SynchronizedTextBuilder(String1));
            unequalTarget = new SynchronizedTextBuilderCharSequence(new SynchronizedTextBuilder(String2));
            emptyTarget = new SynchronizedTextBuilderCharSequence(new SynchronizedTextBuilder(string.Empty));
        }

        protected override int CompareToString(SynchronizedTextBuilderCharSequence target, string? value) => target.CompareTo(value);
        protected override int CompareToCharArray(SynchronizedTextBuilderCharSequence target, char[]? value) => target.CompareTo(value);
        protected override int CompareToStringBuilder(SynchronizedTextBuilderCharSequence target, StringBuilder? value) => target.CompareTo(value);
        protected override int CompareToReadOnlySpan(SynchronizedTextBuilderCharSequence target, ReadOnlySpan<char> value) => target.CompareTo(value);
        protected override int CompareToObject(SynchronizedTextBuilderCharSequence target, object? value) => target.CompareTo(value);

        protected override bool EqualsString(SynchronizedTextBuilderCharSequence target, string? value) => target.Equals(value);
        protected override bool EqualsCharArray(SynchronizedTextBuilderCharSequence target, char[]? value) => target.Equals(value);
        protected override bool EqualsStringBuilder(SynchronizedTextBuilderCharSequence target, StringBuilder? value) => target.Equals(value);
        protected override bool EqualsReadOnlySpan(SynchronizedTextBuilderCharSequence target, ReadOnlySpan<char> value) => target.Equals(value);

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

        //    Assert.IsTrue(target == String1);
        //    Assert.IsTrue(String1 == target);

        //    Assert.IsFalse(target == String2);
        //    Assert.IsFalse(String2 == target);

        //    Assert.IsTrue(nullTarget == (string)null);
        //    Assert.IsTrue((string)null == nullTarget);


        //    Assert.IsFalse(target != equalTarget);
        //    Assert.IsFalse(equalTarget != target);

        //    Assert.IsTrue(target != unequalTarget);
        //    Assert.IsTrue(unequalTarget != target);

        //    Assert.IsTrue(target != String2);
        //    Assert.IsTrue(String2 != target);

        //    Assert.IsFalse(target != String1);
        //    Assert.IsFalse(String1 != target);

        //    Assert.IsFalse(nullTarget != (string)null);
        //    Assert.IsFalse((string)null != nullTarget);
        //}

        [Test]
        public void Equals_SynchronizedTextBuilderCharSequence_ShouldNotDeadlock_WhenComparingOppositeDirections()
        {
            var sb1 = new SynchronizedTextBuilder("abcdefghijklmnopqrstuvwxyz");
            var sb2 = new SynchronizedTextBuilder("abcdefghijklmnopqrstuvwxyz");

            var seq1 = new SynchronizedTextBuilderCharSequence(sb1);
            var seq2 = new SynchronizedTextBuilderCharSequence(sb2);

            var barrier = new Barrier(2);

            Exception? ex1 = null;
            Exception? ex2 = null;

            var t1 = Task.Run(() =>
            {
                barrier.SignalAndWait();

                try
                {
                    for (int i = 0; i < 1000; i++)
                        Assert.IsTrue(seq1.Equals(seq2));
                }
                catch (Exception ex)
                {
                    ex1 = ex;
                }
            });

            var t2 = Task.Run(() =>
            {
                barrier.SignalAndWait();

                try
                {
                    for (int i = 0; i < 1000; i++)
                        Assert.IsTrue(seq2.Equals(seq1));
                }
                catch (Exception ex)
                {
                    ex2 = ex;
                }
            });

            Assert.IsTrue(Task.WaitAll(new[] { t1, t2 }, TimeSpan.FromSeconds(4)),
                "Possible deadlock.");

            Assert.IsNull(ex1);
            Assert.IsNull(ex2);
        }

        [Test]
        public void CompareTo_SynchronizedTextBuilderCharSequence_ShouldNotDeadlock_WhenComparingOppositeDirections()
        {
            var sb1 = new SynchronizedTextBuilder("abcdefghijklmnopqrstuvwxyz");
            var sb2 = new SynchronizedTextBuilder("abcdefghijklmnopqrstuvwxyz");

            var seq1 = new SynchronizedTextBuilderCharSequence(sb1);
            var seq2 = new SynchronizedTextBuilderCharSequence(sb2);

            var barrier = new Barrier(2);

            Exception? ex1 = null;
            Exception? ex2 = null;

            var t1 = Task.Run(() =>
            {
                barrier.SignalAndWait();

                try
                {
                    for (int i = 0; i < 1000; i++)
                        Assert.AreEqual(0, seq1.CompareTo(seq2));
                }
                catch (Exception ex)
                {
                    ex1 = ex;
                }
            });

            var t2 = Task.Run(() =>
            {
                barrier.SignalAndWait();

                try
                {
                    for (int i = 0; i < 1000; i++)
                        Assert.AreEqual(0, seq2.CompareTo(seq1));
                }
                catch (Exception ex)
                {
                    ex2 = ex;
                }
            });

            Assert.IsTrue(Task.WaitAll(new[] { t1, t2 }, TimeSpan.FromSeconds(4)),
                "Possible deadlock.");

            Assert.IsNull(ex1);
            Assert.IsNull(ex2);
        }
    }
}
