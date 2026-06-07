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
    public class TestCountdownLatch : TestCase
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
         * a latch constructed with count zero starts open: Await does not block
         * and CountDown has no effect
         */
        [Test]
        public void TestConstructor_ZeroCount()
        {
            using CountdownLatch latch = new CountdownLatch(0);
            assertEquals(0, latch.Count);
            latch.Await(); // returns immediately, the count is already zero
            assertTrue(latch.Await(0));
            assertTrue(latch.Await(TimeSpan.Zero));
            latch.CountDown(); // no-op
            assertEquals(0, latch.Count);
        }

        /**
         * Count returns initial count and decreases after CountDown
         */
        [Test]
        public void TestCount()
        {
            using CountdownLatch latch = new CountdownLatch(2);
            assertEquals(2, latch.Count);
            latch.CountDown();
            assertEquals(1, latch.Count);
        }

        /**
         * CountDown decrements count when positive and has no effect when zero
         */
        [Test]
        public void TestCountDown()
        {
            using CountdownLatch latch = new CountdownLatch(1);
            assertEquals(1, latch.Count);
            latch.CountDown();
            assertEquals(0, latch.Count);
            // Unlike CountdownEvent.Signal(), which throws InvalidOperationException
            // once the count reaches zero, counting down past zero is a no-op,
            // matching Java's CountDownLatch.countDown() semantics.
            latch.CountDown();
            assertEquals(0, latch.Count);
        }

        /**
         * Await returns after CountDown to zero, but not before
         */
        [Test]
        public void TestAwait()
        {
            using CountdownLatch latch = new CountdownLatch(2);
            using ManualResetEventSlim started = new ManualResetEventSlim(false);
            ThreadJob t = new ThreadJob(() =>
            {
                started.Set();
                latch.Await();
                assertEquals(0, latch.Count);
            });
            t.Start();
            assertTrue(started.Wait(MaxWaitMilliseconds));
            Thread.Sleep(ShortDelayMilliseconds); // give the worker a chance to block in Await()
            assertEquals(2, latch.Count);
            latch.CountDown();
            assertEquals(1, latch.Count);
            latch.CountDown();
            assertEquals(0, latch.Count);
            JoinAndAssertCompleted(t);
        }

        /**
         * timed Await returns after CountDown to zero
         */
        [Test]
        public void TestTimedAwait()
        {
            using CountdownLatch latch = new CountdownLatch(2);
            using ManualResetEventSlim started = new ManualResetEventSlim(false);
            ThreadJob t = new ThreadJob(() =>
            {
                started.Set();
                assertTrue(latch.Await(TimeSpan.FromMilliseconds(MaxWaitMilliseconds)));
            });
            t.Start();
            assertTrue(started.Wait(MaxWaitMilliseconds));
            Thread.Sleep(ShortDelayMilliseconds);
            assertEquals(2, latch.Count);
            latch.CountDown();
            assertEquals(1, latch.Count);
            latch.CountDown();
            assertEquals(0, latch.Count);
            JoinAndAssertCompleted(t);
        }

        /**
         * Await throws ThreadInterruptedException if interrupted before counted down
         */
        [Test]
        public void TestAwait_InterruptedException()
        {
            using CountdownLatch latch = new CountdownLatch(1);
            using ManualResetEventSlim started = new ManualResetEventSlim(false);
            ThreadJob t = new ThreadJob(() =>
            {
                try
                {
                    assertTrue(latch.Count > 0);
                    started.Set();
                    latch.Await();
                    fail("Should throw ThreadInterruptedException");
                }
                catch (ThreadInterruptedException)
                {
                    // expected
                }
            });
            t.Start();
            assertTrue(started.Wait(MaxWaitMilliseconds));
            Thread.Sleep(ShortDelayMilliseconds);
            assertEquals(1, latch.Count);
            t.Interrupt();
            JoinAndAssertCompleted(t);
        }

        /**
         * timed Await throws ThreadInterruptedException if interrupted before counted down
         */
        [Test]
        public void TestTimedAwait_InterruptedException()
        {
            using CountdownLatch latch = new CountdownLatch(1);
            using ManualResetEventSlim started = new ManualResetEventSlim(false);
            ThreadJob t = new ThreadJob(() =>
            {
                try
                {
                    assertTrue(latch.Count > 0);
                    started.Set();
                    latch.Await(TimeSpan.FromMilliseconds(MaxWaitMilliseconds));
                    fail("Should throw ThreadInterruptedException");
                }
                catch (ThreadInterruptedException)
                {
                    // expected
                }
            });
            t.Start();
            assertTrue(started.Wait(MaxWaitMilliseconds));
            Thread.Sleep(ShortDelayMilliseconds);
            assertEquals(1, latch.Count);
            t.Interrupt();
            JoinAndAssertCompleted(t);
        }

        /**
         * timed Await times out if not counted down before timeout, leaving the count unchanged
         */
        [Test]
        public void TestAwaitTimeout()
        {
            using CountdownLatch latch = new CountdownLatch(1);
            assertFalse(latch.Await(TimeSpan.FromMilliseconds(ShortDelayMilliseconds)));
            assertEquals(1, latch.Count);
        }

        /**
         * the millisecond overload observes the same timeout and completion semantics
         * as the TimeSpan overload, including zero and infinite timeouts
         */
        [Test]
        public void TestAwait_MillisecondsTimeout()
        {
            using CountdownLatch latch = new CountdownLatch(1);
            assertFalse(latch.Await(0));
            assertFalse(latch.Await(ShortDelayMilliseconds));
            assertEquals(1, latch.Count);
            latch.CountDown();
            assertTrue(latch.Await(0));
            assertTrue(latch.Await(ShortDelayMilliseconds));
            assertTrue(latch.Await(Timeout.Infinite));
            assertTrue(latch.Await(Timeout.InfiniteTimeSpan));
        }

        /**
         * timeouts that are neither non-negative nor -1 milliseconds (infinite) are rejected
         */
        [Test]
        public void TestAwait_OutOfRangeTimeout()
        {
            using CountdownLatch latch = new CountdownLatch(1);
            Assert.Throws<ArgumentOutOfRangeException>(() => latch.Await(-2));
            Assert.Throws<ArgumentOutOfRangeException>(() => latch.Await(TimeSpan.FromMilliseconds(-2)));
            Assert.Throws<ArgumentOutOfRangeException>(() => latch.Await(TimeSpan.MaxValue));
        }

        /**
         * Await(CancellationToken) throws OperationCanceledException when the token
         * is canceled while waiting, leaving the latch closed
         */
        [Test]
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
                    latch.Await(cts.Token);
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
            assertEquals(1, latch.Count);
        }

        /**
         * the timed token overloads throw OperationCanceledException when the token
         * is canceled while waiting
         */
        [Test]
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
                    latch.Await(TimeSpan.FromMilliseconds(MaxWaitMilliseconds), cts.Token);
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
                    latch.Await(MaxWaitMilliseconds, cts.Token);
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
            assertEquals(1, latch.Count);
        }

        /**
         * token-observing waits complete normally when the latch is counted down
         * and the token is never canceled
         */
        [Test]
        public void TestAwait_CancellationToken_NotCanceled()
        {
            using CountdownLatch latch = new CountdownLatch(1);
            using CancellationTokenSource cts = new CancellationTokenSource();
            using CountdownEvent started = new CountdownEvent(2);
            ThreadJob untimedWaiter = new ThreadJob(() =>
            {
                started.Signal();
                latch.Await(cts.Token); // completes normally when the count reaches zero
            });
            ThreadJob timedWaiter = new ThreadJob(() =>
            {
                started.Signal();
                assertTrue(latch.Await(MaxWaitMilliseconds, cts.Token));
            });
            untimedWaiter.Start();
            timedWaiter.Start();
            assertTrue(started.Wait(MaxWaitMilliseconds));
            Thread.Sleep(ShortDelayMilliseconds);
            latch.CountDown();
            JoinAndAssertCompleted(untimedWaiter);
            JoinAndAssertCompleted(timedWaiter);
            assertEquals(0, latch.Count);
        }

        /**
         * an already-canceled token throws OperationCanceledException even when
         * the count is zero, matching CountdownEvent.Wait
         */
        [Test]
        public void TestAwait_PreCanceledToken()
        {
            using CountdownLatch latch = new CountdownLatch(0);
            using CancellationTokenSource cts = new CancellationTokenSource();
            cts.Cancel();
            Assert.Throws<OperationCanceledException>(() => latch.Await(cts.Token));
            Assert.Throws<OperationCanceledException>(() => latch.Await(TimeSpan.FromMilliseconds(ShortDelayMilliseconds), cts.Token));
            Assert.Throws<OperationCanceledException>(() => latch.Await(ShortDelayMilliseconds, cts.Token));
        }

        /**
         * Count never goes negative and never increases, even when concurrent
         * CountDown calls race past the early-return guard
         */
        [Test]
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
                        latch.CountDown();
                        long current = latch.Count;
                        // Count is clamped at zero and the internal counter is
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

            assertEquals(0, latch.Count);
            assertTrue(latch.Await(0));
            assertTrue(latch.ToString().IndexOf("Count = 0", StringComparison.Ordinal) >= 0);
        }

        /**
         * all waiting threads are released together when the count reaches zero
         */
        [Test]
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
                    assertTrue(latch.Await(MaxWaitMilliseconds));
                    Interlocked.Increment(ref released);
                });
            }
            foreach (ThreadJob waiter in waiters)
                waiter.Start();
            assertTrue(started.Wait(MaxWaitMilliseconds));
            Thread.Sleep(ShortDelayMilliseconds);
            assertEquals(0, Volatile.Read(ref released));
            latch.CountDown();
            foreach (ThreadJob waiter in waiters)
                JoinAndAssertCompleted(waiter);
            assertEquals(WaiterCount, released);
        }

        /**
         * a single waiter is released once the count is exhausted by multiple
         * threads each counting down once
         */
        [Test]
        public void TestManyCountersOneWaiter()
        {
            const int CounterCount = 5;

            using CountdownLatch latch = new CountdownLatch(CounterCount);
            ThreadJob[] counters = new ThreadJob[CounterCount];
            for (int i = 0; i < CounterCount; i++)
            {
                counters[i] = new ThreadJob(() => latch.CountDown());
            }
            foreach (ThreadJob counter in counters)
                counter.Start();
            assertTrue(latch.Await(TimeSpan.FromMilliseconds(MaxWaitMilliseconds)));
            assertEquals(0, latch.Count);
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
            latch.CountDown();
            string s1 = latch.ToString();
            assertTrue(s1.IndexOf("Count = 1", StringComparison.Ordinal) >= 0);
            latch.CountDown();
            string s2 = latch.ToString();
            assertTrue(s2.IndexOf("Count = 0", StringComparison.Ordinal) >= 0);
        }

        /**
         * Dispose is idempotent; waiting on or counting down a disposed latch
         * throws ObjectDisposedException, while Count remains readable
         */
        [Test]
        public void TestDispose()
        {
            CountdownLatch latch = new CountdownLatch(1);
            latch.Dispose();
            latch.Dispose(); // double dispose is a no-op
            Assert.Throws<ObjectDisposedException>(() => latch.Await());
            Assert.Throws<ObjectDisposedException>(() => latch.Await(ShortDelayMilliseconds));
            Assert.Throws<ObjectDisposedException>(() => latch.Await(TimeSpan.FromMilliseconds(ShortDelayMilliseconds)));
            Assert.Throws<ObjectDisposedException>(() => latch.CountDown());
            assertEquals(1, latch.Count); // Count stays readable, like CountdownEvent.CurrentCount
        }
    }
}
