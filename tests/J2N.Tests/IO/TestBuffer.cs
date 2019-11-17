using NUnit.Framework;
using System;

namespace J2N.IO
{
    [TestFixture]
    public class TestBuffer
    {
        public static void TestBufferInstance(Buffer buf)
        {
            // save state
            int oldPosition = buf.Position;
            int oldLimit = buf.Limit;

            TestCapacity(buf);
            TestClear(buf);
            TestFlip(buf);
            TestHasRemaining(buf);
            TestIsReadOnly(buf);
            TestLimit(buf);
            TestLimitint(buf);
            TestMark(buf);
            TestPosition(buf);
            TestPositionint(buf);
            TestRemaining(buf);
            TestReset(buf);
            TestRewind(buf);

            // check state, should not change
            Assert.AreEqual(oldPosition, buf.Position);
            Assert.AreEqual(oldLimit, buf.Limit);
        }

        public static void TestCapacity(Buffer buf)
        {
            Assert.IsTrue(0 <= buf.Position && buf.Position <= buf.Limit
                    && buf.Limit <= buf.Capacity);
        }

        public static void TestClear(Buffer buf)
        {
            // save state
            int oldPosition = buf.Position;
            int oldLimit = buf.Limit;

            Buffer ret = buf.Clear();
            Assert.AreSame(buf, ret);
            Assert.AreEqual(0, buf.Position);
            Assert.AreEqual(buf.Capacity, buf.Limit);
            try
            {
                buf.Reset();
                Assert.Fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (InvalidMarkException e)
            {
                // expected
            }

            // restore state
            buf.Limit=(oldLimit);
            buf.Position=(oldPosition);
        }

        public static void TestFlip(Buffer buf)
        {
            // save state
            int oldPosition = buf.Position;
            int oldLimit = buf.Limit;

            Buffer ret = buf.Flip();
            Assert.AreSame(buf, ret);
            Assert.AreEqual(0, buf.Position);
            Assert.AreEqual(oldPosition, buf.Limit);
            try
            {
                buf.Reset();
                Assert.Fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (InvalidMarkException e)
            {
                // expected
            }

            // restore state
            buf.Limit=(oldLimit);
            buf.Position=(oldPosition);
        }

        public static void TestHasRemaining(Buffer buf)
        {
            // save state
            int oldPosition = buf.Position;
            int oldLimit = buf.Limit;

            Assert.AreEqual(buf.Position < buf.Limit, buf.HasRemaining);
            buf.Position=(buf.Limit);
            Assert.IsFalse(buf.HasRemaining);

            // restore state
            buf.Limit=(oldLimit);
            buf.Position=(oldPosition);
        }

        public static void TestIsReadOnly(Buffer buf)
        {
            var _ = buf.IsReadOnly;
        }

        /*
         * Class under test for int Limit
         */
        public static void TestLimit(Buffer buf)
        {
            Assert.IsTrue(0 <= buf.Position && buf.Position <= buf.Limit
                    && buf.Limit <= buf.Capacity);
        }

        /*
         * Class under test for Buffer limit(int)
         */
        public static void TestLimitint(Buffer buf)
        {
            // save state
            int oldPosition = buf.Position;
            int oldLimit = buf.Limit;

            Buffer ret = buf.SetLimit(buf.Limit);
            Assert.AreSame(buf, ret);

            buf.Mark();
            buf.Limit=(buf.Capacity);
            Assert.AreEqual(buf.Capacity, buf.Limit);
            // position should not change
            Assert.AreEqual(oldPosition, buf.Position);
            // mark should be valid
            buf.Reset();

            if (buf.Capacity > 0)
            {
                buf.Limit=(buf.Capacity);
                buf.Position=(buf.Capacity);
                buf.Mark();
                buf.Limit=(buf.Capacity - 1);
                // position should be the new limit
                Assert.AreEqual(buf.Limit, buf.Position);
                // mark should be invalid
                try
                {
                    buf.Reset();
                    Assert.Fail("Should throw Exception"); //$NON-NLS-1$
                }
                catch (InvalidMarkException e)
                {
                    // expected
                }
            }

            try
            {
                buf.Limit=(-1);
                Assert.Fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (ArgumentException e)
            {
                // expected
            }
            try
            {
                buf.Limit=(buf.Capacity + 1);
                Assert.Fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (ArgumentException e)
            {
                // expected
            }

            // restore state
            buf.Limit=(oldLimit);
            buf.Position=(oldPosition);
        }

        public static void TestMark(Buffer buf)
        {
            // save state
            int oldPosition = buf.Position;
            int oldLimit = buf.Limit;

            Buffer ret = buf.Mark();
            Assert.AreSame(buf, ret);

            buf.Mark();
            buf.Position=(buf.Limit);
            buf.Reset();
            Assert.AreEqual( oldPosition, buf.Position);

            buf.Mark();
            buf.Position=(buf.Limit);
            buf.Reset();
            Assert.AreEqual( oldPosition, buf.Position);

            // restore state
            buf.Limit=(oldLimit);
            buf.Position=(oldPosition);
        }

        /*
         * Class under test for int Position
         */
        public static void TestPosition(Buffer buf)
        {
            Assert.IsTrue(0 <= buf.Position && buf.Position <= buf.Limit
                    && buf.Limit <= buf.Capacity);
        }

        /*
         * Class under test for Buffer position(int)
         */
        public static void TestPositionint(Buffer buf)
        {
            // save state
            int oldPosition = buf.Position;
            int oldLimit = buf.Limit;

            try
            {
                buf.Position=(-1);
                Assert.Fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (ArgumentException e)
            {
                // expected
            }
            try
            {
                buf.Position=(buf.Limit + 1);
                Assert.Fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (ArgumentException e)
            {
                // expected
            }

            buf.Mark();
            buf.Position=(buf.Position);
            buf.Reset();
            Assert.AreEqual(oldPosition, buf.Position);

            buf.Position=(0);
            Assert.AreEqual(0, buf.Position);
            buf.Position=(buf.Limit);
            Assert.AreEqual(buf.Limit, buf.Position);

            if (buf.Capacity > 0)
            {
                buf.Limit=(buf.Capacity);
                buf.Position=(buf.Limit);
                buf.Mark();
                buf.Position=(buf.Limit - 1);
                Assert.AreEqual(buf.Limit - 1, buf.Position);
                // mark should be invalid
                try
                {
                    buf.Reset();
                    Assert.Fail("Should throw Exception"); //$NON-NLS-1$
                }
                catch (InvalidMarkException e)
                {
                    // expected
                }
            }

            Buffer ret = buf.SetPosition(0);
            Assert.AreSame(buf, ret);

            // restore state
            buf.Limit=(oldLimit);
            buf.Position=(oldPosition);
        }

        public static void TestRemaining(Buffer buf)
        {
            Assert.AreEqual(buf.Limit - buf.Position, buf.Remaining);
        }

        public static void TestReset(Buffer buf)
        {
            // save state
            int oldPosition = buf.Position;
            int oldLimit = buf.Limit;

            buf.Mark();
            buf.Position=(buf.Limit);
            buf.Reset();
            Assert.AreEqual(oldPosition, buf.Position);

            buf.Mark();
            buf.Position=(buf.Limit);
            buf.Reset();
            Assert.AreEqual(oldPosition, buf.Position);

            Buffer ret = buf.Reset();
            Assert.AreSame(buf, ret);

            buf.Clear();
            try
            {
                buf.Reset();
                Assert.Fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (InvalidMarkException e)
            {
                // expected
            }

            // restore state
            buf.Limit=(oldLimit);
            buf.Position=(oldPosition);
        }

        public static void TestRewind(Buffer buf)
        {
            // save state
            int oldPosition = buf.Position;
            int oldLimit = buf.Limit;

            Buffer ret = buf.Rewind();
            Assert.AreEqual(0, buf.Position);
            Assert.AreSame(buf, ret);
            try
            {
                buf.Reset();
                Assert.Fail("Should throw Exception"); //$NON-NLS-1$
            }
            catch (InvalidMarkException e)
            {
                // expected
            }

            // restore state
            buf.Limit=(oldLimit);
            buf.Position=(oldPosition);
        }

        //[Test]
        //public void TestNothing()
        //{
        //    // to remove JUnit warning
        //}

    }
}
