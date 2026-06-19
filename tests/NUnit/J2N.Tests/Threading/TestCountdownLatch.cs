#region Copyright 2010 by Apache Harmony, Licensed under the Apache License, Version 2.0
/*  Licensed to the Apache Software Foundation (ASF) under one or more
 *  contributor license agreements.  See the NOTICE file distributed with
 *  this work for additional information regarding copyright ownership.
 *  The ASF licenses this file to You under the Apache License, Version 2.0
 *  (the "License"); you may not use this file except in compliance with
 *  the License.  You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 *  Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *  limitations under the License.
 */
#endregion

// Some tests adapted from Apache Harmony: https://github.com/apache/harmony/blob/02970cb7227a335edd2c8457ebdde0195a735733/classlib/modules/concurrent/src/test/java/CountDownLatchTest.java

using NUnit.Framework;
using System;
using System.Threading;

namespace J2N.Threading
{
    public class TestCountdownLatch : JSR166TestCase
    {
        // Delay in milliseconds for waits that are expected to time out, or for
        // giving a background thread a chance to block.
        private const int ShortDelayMilliseconds = 50;

        // Generous upper bound in milliseconds for waits that are expected to complete,
        // so a regression fails within a bounded time instead of hanging the runner.
        private const int MaxWaitMilliseconds = 30000;

        // Joins a worker with a bounded wait so a regression fails instead of hanging;
        // ThreadJob.Join also rethrows any exception the worker stored.
        private static void JoinAndAssertCompleted(ThreadJob thread)
        {
            thread.Join(MaxWaitMilliseconds);
            assertFalse("Worker thread did not complete in time", thread.IsAlive);
        }

        /**
         * negative constructor argument throws ArgumentOutOfRangeException
         */
        [Test]
        public void TestConstructor()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new CountdownLatch(-1));
        }

        /**
         * a latch constructed with count zero starts open: Wait does not block
         * and Signal has no effect
         */
        [Test] // J2N specific
        public void TestConstructor_ZeroCount()
        {
            using CountdownLatch latch = new CountdownLatch(0);
            assertEquals(0, latch.CurrentCount);
            latch.Wait(); // returns immediately, the count is already zero
            assertTrue(latch.Wait(0));
            assertTrue(latch.Wait(TimeSpan.Zero));
            latch.Signal(); // no-op
            assertEquals(0, latch.CurrentCount);
        }

        /**
         * CurrentCount returns initial count and decreases after Signal
         */
        [Test]
        public void TestCount()
        {
            using CountdownLatch latch = new CountdownLatch(2);
            assertEquals(2, latch.CurrentCount);
            latch.Signal();
            assertEquals(1, latch.CurrentCount);
        }

        /**
         * Signal decrements count when positive and has no effect when zero
         */
        [Test]
        public void TestCountDown()
        {
            using CountdownLatch latch = new CountdownLatch(1);
            assertEquals(1, latch.CurrentCount);
            latch.Signal();
            assertEquals(0, latch.CurrentCount);
            // Unlike CountdownEvent.Signal(), which throws InvalidOperationException
            // once the count reaches zero, counting down past zero is a no-op,
            // matching Java's CountDownLatch.countDown() semantics.
            latch.Signal();
            assertEquals(0, latch.CurrentCount);
        }

        /**
         * Wait returns after the count reaches zero, but not before
         */
        [Test]
        public void TestAwait()
        {
            using CountdownLatch latch = new CountdownLatch(2);
            ThreadJob t = new ThreadJob(() =>
            {
                try
                {
                    threadAssertTrue(latch.CurrentCount > 0);
                    latch.Wait();
                    threadAssertTrue(latch.CurrentCount == 0);
                }
                catch (ThreadInterruptedException)
                {
                    threadUnexpectedException();
                }
            });
            t.Start();
            try
            {
                Thread.Sleep(ShortDelayMilliseconds); // give the worker a chance to block in Wait()
                assertEquals(2, latch.CurrentCount);
                latch.Signal();
                assertEquals(1, latch.CurrentCount);
                latch.Signal();
                assertEquals(0, latch.CurrentCount);
                JoinAndAssertCompleted(t);
            }
            catch (ThreadInterruptedException)
            {
                unexpectedException();
            }
        }

        /**
         * timed Wait returns after the count reaches zero
         */
        [Test]
        public void TestTimedAwait()
        {
            using CountdownLatch latch = new CountdownLatch(2);
            ThreadJob t = new ThreadJob(() =>
            {
                try
                {
                    threadAssertTrue(latch.CurrentCount > 0);
                    threadAssertTrue(latch.Wait(TimeSpan.FromMilliseconds(MaxWaitMilliseconds)));
                }
                catch (ThreadInterruptedException)
                {
                    threadUnexpectedException();
                }
            });
            t.Start();
            try
            {
                Thread.Sleep(ShortDelayMilliseconds);
                assertEquals(2, latch.CurrentCount);
                latch.Signal();
                assertEquals(1, latch.CurrentCount);
                latch.Signal();
                assertEquals(0, latch.CurrentCount);
                JoinAndAssertCompleted(t);
            }
            catch (ThreadInterruptedException)
            {
                unexpectedException();
            }
        }

        /**
         * Wait throws ThreadInterruptedException if interrupted before counted down
         */
        [Test]
        public void TestAwait_InterruptedException()
        {
            using CountdownLatch latch = new CountdownLatch(1);
            ThreadJob t = new ThreadJob(() =>
            {
                try
                {
                    threadAssertTrue(latch.CurrentCount > 0);
                    latch.Wait();
                    threadShouldThrow();
                }
                catch (ThreadInterruptedException)
                {
                    // expected
                }
            });
            t.Start();
            try
            {
                assertEquals(1, latch.CurrentCount);
                t.Interrupt();
                JoinAndAssertCompleted(t);
            }
            catch (ThreadInterruptedException)
            {
                unexpectedException();
            }
        }

        /**
         * timed Wait throws ThreadInterruptedException if interrupted before counted down
         */
        [Test]
        public void TestTimedAwait_InterruptedException()
        {
            using CountdownLatch latch = new CountdownLatch(1);
            ThreadJob t = new ThreadJob(() =>
            {
                try
                {
                    threadAssertTrue(latch.CurrentCount > 0);
                    latch.Wait(TimeSpan.FromMilliseconds(MaxWaitMilliseconds));
                    threadShouldThrow();
                }
                catch (ThreadInterruptedException)
                {
                    // expected
                }
            });
            t.Start();
            try
            {
                Thread.Sleep(ShortDelayMilliseconds);
                assertEquals(1, latch.CurrentCount);
                t.Interrupt();
                JoinAndAssertCompleted(t);
            }
            catch (ThreadInterruptedException)
            {
                unexpectedException();
            }
        }

        /**
         * timed Wait times out if not counted down before timeout, leaving the count unchanged
         */
        [Test]
        public void TestAwaitTimeout()
        {
            using CountdownLatch latch = new CountdownLatch(1);
            ThreadJob t = new ThreadJob(() =>
            {
                try
                {
                    threadAssertTrue(latch.CurrentCount > 0);
                    threadAssertFalse(latch.Wait(TimeSpan.FromMilliseconds(ShortDelayMilliseconds)));
                    threadAssertTrue(latch.CurrentCount > 0);
                }
                catch (ThreadInterruptedException)
                {
                    threadUnexpectedException();
                }
            });
            t.Start();
            try
            {
                assertEquals(1, latch.CurrentCount);
                JoinAndAssertCompleted(t);
            }
            catch (ThreadInterruptedException)
            {
                unexpectedException();
            }
        }

        /**
         * the millisecond overload observes the same timeout and completion semantics
         * as the TimeSpan overload, including zero and infinite timeouts
         */
        [Test] // J2N specific
        public void TestAwait_MillisecondsTimeout()
        {
            using CountdownLatch latch = new CountdownLatch(1);
            assertFalse(latch.Wait(0));
            assertFalse(latch.Wait(ShortDelayMilliseconds));
            assertEquals(1, latch.CurrentCount);
            latch.Signal();
            assertTrue(latch.Wait(0));
            assertTrue(latch.Wait(ShortDelayMilliseconds));
            assertTrue(latch.Wait(Timeout.Infinite));
            assertTrue(latch.Wait(Timeout.InfiniteTimeSpan));
        }

        /**
         * timeouts that are neither non-negative nor -1 milliseconds (infinite) are rejected
         */
        [Test] // J2N specific
        public void TestAwait_OutOfRangeTimeout()
        {
            using CountdownLatch latch = new CountdownLatch(1);
            Assert.Throws<ArgumentOutOfRangeException>(() => latch.Wait(-2));
            Assert.Throws<ArgumentOutOfRangeException>(() => latch.Wait(TimeSpan.FromMilliseconds(-2)));
            Assert.Throws<ArgumentOutOfRangeException>(() => latch.Wait(TimeSpan.MaxValue));
        }

        /**
         * Wait(CancellationToken) throws OperationCanceledException when the token
         * is canceled while waiting, leaving the latch closed
         */
        [Test] // J2N specific
        public void TestAwait_CancellationToken_Canceled()
        {
            using CountdownLatch latch = new CountdownLatch(1);
            using CancellationTokenSource cts = new CancellationTokenSource();
            using ManualResetEventSlim started = new ManualResetEventSlim(false);
            ThreadJob t = new ThreadJob(() =>
            {
                try
                {
                    started.Set();
                    latch.Wait(cts.Token);
                    fail("Should throw OperationCanceledException");
                }
                catch (OperationCanceledException)
                {
                    // expected
                }
            });
            t.Start();
            assertTrue(started.Wait(MaxWaitMilliseconds));
            Thread.Sleep(ShortDelayMilliseconds);
            cts.Cancel();
            JoinAndAssertCompleted(t);
            assertEquals(1, latch.CurrentCount);
        }

        /**
         * the timed token overloads throw OperationCanceledException when the token
         * is canceled while waiting
         */
        [Test] // J2N specific
        public void TestTimedAwait_CancellationToken_Canceled()
        {
            using CountdownLatch latch = new CountdownLatch(1);
            using CancellationTokenSource cts = new CancellationTokenSource();
            using CountdownEvent started = new CountdownEvent(2);
            ThreadJob timeSpanWaiter = new ThreadJob(() =>
            {
                try
                {
                    started.Signal();
                    latch.Wait(TimeSpan.FromMilliseconds(MaxWaitMilliseconds), cts.Token);
                    fail("Should throw OperationCanceledException");
                }
                catch (OperationCanceledException)
                {
                    // expected
                }
            });
            ThreadJob millisecondsWaiter = new ThreadJob(() =>
            {
                try
                {
                    started.Signal();
                    latch.Wait(MaxWaitMilliseconds, cts.Token);
                    fail("Should throw OperationCanceledException");
                }
                catch (OperationCanceledException)
                {
                    // expected
                }
            });
            timeSpanWaiter.Start();
            millisecondsWaiter.Start();
            assertTrue(started.Wait(MaxWaitMilliseconds));
            Thread.Sleep(ShortDelayMilliseconds);
            cts.Cancel();
            JoinAndAssertCompleted(timeSpanWaiter);
            JoinAndAssertCompleted(millisecondsWaiter);
            assertEquals(1, latch.CurrentCount);
        }

        /**
         * token-observing waits complete normally when the latch is counted down
         * and the token is never canceled
         */
        [Test] // J2N specific
        public void TestAwait_CancellationToken_NotCanceled()
        {
            using CountdownLatch latch = new CountdownLatch(1);
            using CancellationTokenSource cts = new CancellationTokenSource();
            using CountdownEvent started = new CountdownEvent(2);
            ThreadJob untimedWaiter = new ThreadJob(() =>
            {
                started.Signal();
                latch.Wait(cts.Token); // completes normally when the count reaches zero
            });
            ThreadJob timedWaiter = new ThreadJob(() =>
            {
                started.Signal();
                assertTrue(latch.Wait(MaxWaitMilliseconds, cts.Token));
            });
            untimedWaiter.Start();
            timedWaiter.Start();
            assertTrue(started.Wait(MaxWaitMilliseconds));
            Thread.Sleep(ShortDelayMilliseconds);
            latch.Signal();
            JoinAndAssertCompleted(untimedWaiter);
            JoinAndAssertCompleted(timedWaiter);
            assertEquals(0, latch.CurrentCount);
        }

        /**
         * an already-canceled token throws OperationCanceledException even when
         * the count is zero, matching CountdownEvent.Wait
         */
        [Test] // J2N specific
        public void TestAwait_PreCanceledToken()
        {
            using CountdownLatch latch = new CountdownLatch(0);
            using CancellationTokenSource cts = new CancellationTokenSource();
            cts.Cancel();
            Assert.Throws<OperationCanceledException>(() => latch.Wait(cts.Token));
            Assert.Throws<OperationCanceledException>(() => latch.Wait(TimeSpan.FromMilliseconds(ShortDelayMilliseconds), cts.Token));
            Assert.Throws<OperationCanceledException>(() => latch.Wait(ShortDelayMilliseconds, cts.Token));
        }

        /**
         * CurrentCount never goes negative and never increases, even when concurrent
         * Signal calls race past the early-return guard
         */
        [Test] // J2N specific
        public void TestCount_NeverNegative()
        {
            const int ThreadCount = 8;
            const int CountDownsPerThread = 25;

            using CountdownLatch latch = new CountdownLatch(4);
            using ManualResetEventSlim startGate = new ManualResetEventSlim(false);
            ThreadJob[] threads = new ThreadJob[ThreadCount];
            for (int i = 0; i < ThreadCount; i++)
            {
                threads[i] = new ThreadJob(() =>
                {
                    startGate.Wait();
                    long previous = long.MaxValue;
                    for (int j = 0; j < CountDownsPerThread; j++)
                    {
                        latch.Signal();
                        long current = latch.CurrentCount;
                        // CurrentCount is clamped at zero and the internal counter is
                        // decrement-only, so each thread must observe a
                        // non-negative, non-increasing sequence.
                        assertTrue(current >= 0);
                        assertTrue(current <= previous);
                        previous = current;
                    }
                });
            }
            foreach (ThreadJob thread in threads)
                thread.Start();
            startGate.Set();
            foreach (ThreadJob thread in threads)
                JoinAndAssertCompleted(thread);

            assertEquals(0, latch.CurrentCount);
            assertTrue(latch.Wait(0));
            assertTrue(latch.ToString().IndexOf("Count = 0", StringComparison.Ordinal) >= 0);
        }

        /**
         * all waiting threads are released together when the count reaches zero
         */
        [Test] // J2N specific
        public void TestMultipleWaiters()
        {
            const int WaiterCount = 5;

            using CountdownLatch latch = new CountdownLatch(1);
            using CountdownEvent started = new CountdownEvent(WaiterCount);
            int released = 0;
            ThreadJob[] waiters = new ThreadJob[WaiterCount];
            for (int i = 0; i < WaiterCount; i++)
            {
                waiters[i] = new ThreadJob(() =>
                {
                    started.Signal();
                    // bounded so a release regression fails the test instead of hanging it
                    assertTrue(latch.Wait(MaxWaitMilliseconds));
                    Interlocked.Increment(ref released);
                });
            }
            foreach (ThreadJob waiter in waiters)
                waiter.Start();
            assertTrue(started.Wait(MaxWaitMilliseconds));
            Thread.Sleep(ShortDelayMilliseconds);
            assertEquals(0, Volatile.Read(ref released));
            latch.Signal();
            foreach (ThreadJob waiter in waiters)
                JoinAndAssertCompleted(waiter);
            assertEquals(WaiterCount, released);
        }

        /**
         * a single waiter is released once the count is exhausted by multiple
         * threads each counting down once
         */
        [Test] // J2N specific
        public void TestManyCountersOneWaiter()
        {
            const int CounterCount = 5;

            using CountdownLatch latch = new CountdownLatch(CounterCount);
            ThreadJob[] counters = new ThreadJob[CounterCount];
            for (int i = 0; i < CounterCount; i++)
            {
                counters[i] = new ThreadJob(() => latch.Signal());
            }
            foreach (ThreadJob counter in counters)
                counter.Start();
            assertTrue(latch.Wait(TimeSpan.FromMilliseconds(MaxWaitMilliseconds)));
            assertEquals(0, latch.CurrentCount);
            foreach (ThreadJob counter in counters)
                JoinAndAssertCompleted(counter);
        }

        /**
         * ToString indicates current count
         */
        [Test]
        public void TestToString()
        {
            using CountdownLatch latch = new CountdownLatch(2);
            string s0 = latch.ToString();
            assertTrue(s0.IndexOf("Count = 2", StringComparison.Ordinal) >= 0);
            latch.Signal();
            string s1 = latch.ToString();
            assertTrue(s1.IndexOf("Count = 1", StringComparison.Ordinal) >= 0);
            latch.Signal();
            string s2 = latch.ToString();
            assertTrue(s2.IndexOf("Count = 0", StringComparison.Ordinal) >= 0);
        }

        /**
         * Dispose is idempotent; waiting on or counting down a disposed latch
         * throws ObjectDisposedException, while CurrentCount remains readable
         */
        [Test] // J2N specific
        public void TestDispose()
        {
            CountdownLatch latch = new CountdownLatch(1);
            latch.Dispose();
            latch.Dispose(); // double dispose is a no-op
            Assert.Throws<ObjectDisposedException>(() => latch.Wait());
            Assert.Throws<ObjectDisposedException>(() => latch.Wait(ShortDelayMilliseconds));
            Assert.Throws<ObjectDisposedException>(() => latch.Wait(TimeSpan.FromMilliseconds(ShortDelayMilliseconds)));
            Assert.Throws<ObjectDisposedException>(() => latch.Signal());
            assertEquals(1, latch.CurrentCount); // CurrentCount stays readable, like CountdownEvent.CurrentCount
        }
    }
}
