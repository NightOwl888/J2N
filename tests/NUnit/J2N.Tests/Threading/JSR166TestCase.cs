#region Copyright by Doug Lea (JSR-166), released to the public domain
/*
 * Written by Doug Lea with assistance from members of JCP JSR-166
 * Expert Group and released to the public domain, as explained at
 * http://creativecommons.org/licenses/publicdomain
 * Other contributors include Andrew Wright, Jeffrey Hayes,
 * Pat Fisher, Mike Judd.
 */
#endregion

// Adapted from Apache Lucene.NET's port of the JSR-166 test suite:
// https://github.com/apache/lucenenet/blob/8c57a0f389f1afa783b73a05328e052dea0b768b/src/Lucene.Net.Tests/Support/Threading/JSR166TestCase.cs
// Only the thread-failure propagation helpers are brought over (not the delay
// constants or TaskScheduler executor support; no J2N use yet).

using System;

namespace J2N.Threading
{
    /// <summary>
    /// Base class for J2N tests ported from the JSR-166 / Apache Harmony suite.
    /// Holds the <c>threadAssert</c> helpers, which flag a failure on a worker
    /// thread so it surfaces from the main thread in <see cref="TearDown"/>.
    /// </summary>
    public abstract class JSR166TestCase : TestCase
    {
        /**
         * Flag set true if any threadAssert methods fail
         */
        private volatile bool threadFailed;

        /**
         * Initializes test to indicate that no thread assertions have failed
         */
        public override void SetUp()
        {
            base.SetUp();
            threadFailed = false;
        }

        /**
         * Triggers test case failure if any thread assertions have failed
         */
        public override void TearDown()
        {
            assertFalse(threadFailed);
            base.TearDown();
        }

        /**
         * Fail, also setting status to indicate current testcase should fail
         */
        public void threadFail(string reason)
        {
            threadFailed = true;
            fail(reason);
        }

        /**
         * If expression not true, set status to indicate current testcase
         * should fail
         */
        public void threadAssertTrue(bool b)
        {
            if (!b)
            {
                threadFailed = true;
                assertTrue(b);
            }
        }

        /**
         * If expression not false, set status to indicate current testcase
         * should fail
         */
        public void threadAssertFalse(bool b)
        {
            if (b)
            {
                threadFailed = true;
                assertFalse(b);
            }
        }

        /**
         * If argument not null, set status to indicate current testcase
         * should fail
         */
        public void threadAssertNull(object x)
        {
            if (x != null)
            {
                threadFailed = true;
                assertNull(x);
            }
        }

        /**
         * If arguments not equal, set status to indicate current testcase
         * should fail
         */
        public void threadAssertEquals(long x, long y)
        {
            if (x != y)
            {
                threadFailed = true;
                assertEquals(x, y);
            }
        }

        /**
         * If arguments not equal, set status to indicate current testcase
         * should fail
         */
        public void threadAssertEquals(object x, object y)
        {
            if (!Equals(x, y))
            {
                threadFailed = true;
                assertEquals(x, y);
            }
        }

        /**
         * threadFail with message "should throw exception"
         */
        public void threadShouldThrow()
        {
            threadFailed = true;
            fail("should throw exception");
        }

        /**
         * threadFail with message "Unexpected exception"
         */
        public void threadUnexpectedException()
        {
            threadFailed = true;
            fail("Unexpected exception");
        }

        /**
         * threadFail with message "Unexpected exception", with argument
         */
        public void threadUnexpectedException(Exception ex)
        {
            threadFailed = true;
            fail("Unexpected exception: " + ex);
        }

        /**
         * fail with message "should throw exception"
         */
        public void shouldThrow()
        {
            fail("Should throw exception");
        }

        /**
         * fail with message "Unexpected exception"
         */
        public void unexpectedException()
        {
            fail("Unexpected exception");
        }
    }
}
