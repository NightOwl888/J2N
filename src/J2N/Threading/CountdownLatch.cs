// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

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

// Some code adapted from Apache Harmony: https://github.com/apache/harmony/blob/02970cb7227a335edd2c8457ebdde0195a735733/classlib/modules/concurrent/src/main/java/java/util/concurrent/CountDownLatch.java
// Other aspects adapted from .NET's CountdownEvent: https://github.com/dotnet/runtime/blob/38496302e54e1b6fb11a998b297492f3fdfbfd0c/src/libraries/System.Threading/src/System/Threading/CountdownEvent.cs

using System;
using System.Diagnostics;
using System.Threading;

namespace J2N.Threading
{
    /// <summary>
    /// A synchronization aid that allows one or more threads to wait until
    /// a set of operations being performed in other threads completes.
    /// <para/>
    /// A <see cref="CountdownLatch"/> is initialized with a given <i>count</i>.
    /// The <see cref="Wait()"/> methods block until the current count reaches
    /// zero due to invocations of the <see cref="Signal()"/> method, after which
    /// all waiting threads are released and any subsequent invocations of
    /// <see cref="Wait()"/> return immediately. This is a one-shot phenomenon:
    /// the count cannot be reset.
    /// <para/>
    /// A <see cref="CountdownLatch"/> is a versatile synchronization tool
    /// and can be used for a number of purposes. A <see cref="CountdownLatch"/>
    /// initialized with a count of one serves as a simple on/off latch, or gate:
    /// all threads invoking <see cref="Wait()"/> wait at the gate until it is
    /// opened by a thread invoking <see cref="Signal()"/>. A
    /// <see cref="CountdownLatch"/> initialized to <i>N</i> can be used to make
    /// one thread wait until <i>N</i> threads have completed some action, or
    /// some action has been completed N times.
    /// <para/>
    /// A useful property of a <see cref="CountdownLatch"/> is that it
    /// doesn't require that threads calling <see cref="Signal()"/> wait for
    /// the count to reach zero before proceeding, it simply prevents any
    /// thread from proceeding past an <see cref="Wait()"/> until all
    /// threads could pass.
    /// <para/>
    /// Usage Note: This type is similar to <see cref="CountdownEvent"/>,
    /// but unlike <see cref="CountdownEvent.Signal()"/>, <see cref="Signal()"/>
    /// can be called any number of times after the count reaches zero without
    /// throwing. This matches the semantics of Java's <c>CountDownLatch.countDown()</c>
    /// method, so Java code that counts down past zero can be ported without adding
    /// guard clauses.
    /// </summary>
    /// <remarks>
    /// Memory consistency effects: actions in a thread prior to calling
    /// <see cref="Signal()"/> happen before actions following a successful
    /// return from a corresponding <see cref="Wait()"/> in another thread.
    /// <para/>
    /// Unlike Java's <c>java.util.concurrent.CountDownLatch</c>, which is not
    /// <c>AutoCloseable</c>, this type implements <see cref="IDisposable"/>
    /// because it wraps a <see cref="ManualResetEventSlim"/>, which is itself
    /// disposable. Callers should dispose instances when they are no longer
    /// needed, typically via a <c>using</c> statement. <see cref="Dispose()"/>
    /// is not thread-safe: only dispose the latch once no threads are waiting
    /// on it or counting it down.
    /// </remarks>
    [DebuggerDisplay("Initial Count={InitialCount}, Current Count={CurrentCount}")]
    public sealed class CountdownLatch : IDisposable
    {
        // The current count and ManualResetEventSlim for signaling, instead of Java's Sync class
        private volatile int _currentCount;
        private readonly int _initialCount;
        private readonly ManualResetEventSlim _event;
        private volatile bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="CountdownLatch"/> class
        /// with the given <paramref name="count"/>.
        /// </summary>
        /// <param name="count">The number of times <see cref="Signal()"/> must be invoked
        /// before threads can pass through <see cref="Wait()"/>.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> is negative.</exception>
        public CountdownLatch(int count)
        {
            if (count < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(count, ExceptionArgument.count);

            _currentCount = count;
            _initialCount = count;
            _event = new ManualResetEventSlim(count == 0);
        }

        /// <summary>
        /// Causes the current thread to wait until the latch has counted down to
        /// zero, unless the thread is interrupted.
        /// <para/>
        /// If the current count is zero then this method returns immediately.
        /// <para/>
        /// If the current count is greater than zero then the current
        /// thread becomes disabled for thread scheduling purposes and lies
        /// dormant until one of two things happen:
        /// <list type="bullet">
        ///     <item><description>The count reaches zero due to invocations of the
        ///         <see cref="Signal()"/> method; or</description></item>
        ///     <item><description>Some other thread interrupts the current thread.</description></item>
        /// </list>
        /// <para/>
        /// If the current thread is interrupted while waiting, a
        /// <see cref="ThreadInterruptedException"/> is thrown.
        /// </summary>
        /// <exception cref="ThreadInterruptedException">The current thread is
        /// interrupted while waiting.</exception>
        /// <exception cref="ObjectDisposedException">The current instance has
        /// already been disposed.</exception>
        public void Wait()
        {
            _event.Wait();
        }

        /// <summary>
        /// Causes the current thread to wait until the latch has counted down to
        /// zero, unless the thread is interrupted, while observing a
        /// <see cref="CancellationToken"/>.
        /// <para/>
        /// If <paramref name="cancellationToken"/> is already canceled when this
        /// method is called, an <see cref="OperationCanceledException"/> is thrown
        /// even if the count has already reached zero, matching
        /// <see cref="CountdownEvent.Wait(CancellationToken)"/>.
        /// </summary>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe.</param>
        /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/>
        /// has been canceled.</exception>
        /// <exception cref="ThreadInterruptedException">The current thread is
        /// interrupted while waiting.</exception>
        /// <exception cref="ObjectDisposedException">The current instance has
        /// already been disposed.</exception>
        /// <seealso cref="Wait()"/>
        public void Wait(CancellationToken cancellationToken)
        {
            _event.Wait(cancellationToken);
        }

        /// <summary>
        /// Causes the current thread to wait until the latch has counted down to
        /// zero, unless the thread is interrupted, or the specified waiting time
        /// elapses.
        /// <para/>
        /// If the current count is zero then this method returns immediately
        /// with the value <c>true</c>.
        /// <para/>
        /// If the current count is greater than zero then the current
        /// thread becomes disabled for thread scheduling purposes and lies
        /// dormant until one of three things happen:
        /// <list type="bullet">
        ///     <item><description>The count reaches zero due to invocations of the
        ///         <see cref="Signal()"/> method; or</description></item>
        ///     <item><description>Some other thread interrupts the current thread; or</description></item>
        ///     <item><description>The specified waiting time elapses.</description></item>
        /// </list>
        /// <para/>
        /// If the count reaches zero then the method returns with the
        /// value <c>true</c>.
        /// <para/>
        /// If the current thread is interrupted while waiting, a
        /// <see cref="ThreadInterruptedException"/> is thrown.
        /// <para/>
        /// If the specified waiting time elapses then the value <c>false</c>
        /// is returned. If the time is zero, the method does not block: it
        /// returns <c>true</c> if the count has already reached zero, or
        /// <c>false</c> otherwise. Note that negative timeouts are handled
        /// differently from Java: Java treats any non-positive timeout as an
        /// immediate poll, while this method follows
        /// <see cref="CountdownEvent.Wait(TimeSpan)"/> semantics, where -1
        /// milliseconds means an infinite wait and other negative values throw
        /// <see cref="ArgumentOutOfRangeException"/>.
        /// </summary>
        /// <param name="timeout">The maximum time to wait, or a <see cref="TimeSpan"/> that
        /// represents -1 milliseconds to wait indefinitely.</param>
        /// <returns><c>true</c> if the count reached zero, or <c>false</c>
        /// if the waiting time elapsed before the count reached zero.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="timeout"/> is a negative
        /// number other than -1 milliseconds, which represents an infinite time-out, or
        /// <paramref name="timeout"/> is greater than <see cref="int.MaxValue"/> milliseconds.</exception>
        /// <exception cref="ThreadInterruptedException">The current thread is
        /// interrupted while waiting.</exception>
        /// <exception cref="ObjectDisposedException">The current instance has
        /// already been disposed.</exception>
        public bool Wait(TimeSpan timeout)
        {
            return _event.Wait(timeout);
        }

        /// <summary>
        /// Causes the current thread to wait until the latch has counted down to
        /// zero, unless the thread is interrupted, or the specified waiting time
        /// elapses, while observing a <see cref="CancellationToken"/>.
        /// <para/>
        /// If <paramref name="cancellationToken"/> is already canceled when this
        /// method is called, an <see cref="OperationCanceledException"/> is thrown
        /// even if the count has already reached zero, matching
        /// <see cref="CountdownEvent.Wait(TimeSpan, CancellationToken)"/>.
        /// </summary>
        /// <param name="timeout">The maximum time to wait, or a <see cref="TimeSpan"/> that
        /// represents -1 milliseconds to wait indefinitely.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe.</param>
        /// <returns><c>true</c> if the count reached zero, or <c>false</c>
        /// if the waiting time elapsed before the count reached zero.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="timeout"/> is a negative
        /// number other than -1 milliseconds, which represents an infinite time-out, or
        /// <paramref name="timeout"/> is greater than <see cref="int.MaxValue"/> milliseconds.</exception>
        /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/>
        /// has been canceled.</exception>
        /// <exception cref="ThreadInterruptedException">The current thread is
        /// interrupted while waiting.</exception>
        /// <exception cref="ObjectDisposedException">The current instance has
        /// already been disposed.</exception>
        /// <seealso cref="Wait(TimeSpan)"/>
        public bool Wait(TimeSpan timeout, CancellationToken cancellationToken)
        {
            return _event.Wait(timeout, cancellationToken);
        }

        /// <summary>
        /// Causes the current thread to wait until the latch has counted down to
        /// zero, unless the thread is interrupted, or the specified waiting time
        /// elapses.
        /// <para/>
        /// Note that unlike Java, where a non-positive timeout means an immediate
        /// poll, <see cref="Timeout.Infinite"/> (-1) means an infinite wait and
        /// other negative values throw <see cref="ArgumentOutOfRangeException"/>.
        /// </summary>
        /// <param name="millisecondsTimeout">The number of milliseconds to wait, or
        /// <see cref="Timeout.Infinite"/> (-1) to wait indefinitely.</param>
        /// <returns><c>true</c> if the count reached zero, or <c>false</c>
        /// if the waiting time elapsed before the count reached zero.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="millisecondsTimeout"/> is a
        /// negative number other than -1, which represents an infinite time-out.</exception>
        /// <exception cref="ThreadInterruptedException">The current thread is
        /// interrupted while waiting.</exception>
        /// <exception cref="ObjectDisposedException">The current instance has
        /// already been disposed.</exception>
        /// <seealso cref="Wait(TimeSpan)"/>
        public bool Wait(int millisecondsTimeout)
        {
            return _event.Wait(millisecondsTimeout);
        }

        /// <summary>
        /// Causes the current thread to wait until the latch has counted down to
        /// zero, unless the thread is interrupted, or the specified waiting time
        /// elapses, while observing a <see cref="CancellationToken"/>.
        /// <para/>
        /// If <paramref name="cancellationToken"/> is already canceled when this
        /// method is called, an <see cref="OperationCanceledException"/> is thrown
        /// even if the count has already reached zero, matching
        /// <see cref="CountdownEvent.Wait(int, CancellationToken)"/>.
        /// </summary>
        /// <param name="millisecondsTimeout">The number of milliseconds to wait, or
        /// <see cref="Timeout.Infinite"/> (-1) to wait indefinitely.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe.</param>
        /// <returns><c>true</c> if the count reached zero, or <c>false</c>
        /// if the waiting time elapsed before the count reached zero.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="millisecondsTimeout"/> is a
        /// negative number other than -1, which represents an infinite time-out.</exception>
        /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/>
        /// has been canceled.</exception>
        /// <exception cref="ThreadInterruptedException">The current thread is
        /// interrupted while waiting.</exception>
        /// <exception cref="ObjectDisposedException">The current instance has
        /// already been disposed.</exception>
        /// <seealso cref="Wait(TimeSpan)"/>
        public bool Wait(int millisecondsTimeout, CancellationToken cancellationToken)
        {
            return _event.Wait(millisecondsTimeout, cancellationToken);
        }

        /// <summary>
        /// Decrements the count of the latch, releasing all waiting threads if
        /// the count reaches zero.
        /// <para/>
        /// If the current count is greater than zero then it is decremented.
        /// If the new count is zero then all waiting threads are re-enabled for
        /// thread scheduling purposes.
        /// <para/>
        /// If the current count equals zero then nothing happens.
        /// </summary>
        /// <exception cref="ObjectDisposedException">The current instance has
        /// already been disposed.</exception>
        public void Signal()
        {
            if (_disposed)
                ThrowHelper.ThrowObjectDisposedException(this);

            if (_currentCount <= 0)
            {
                // Try to avoid unnecessary decrementing of the count below zero,
                // but a couple concurrent races below zero are fine
                return;
            }

            int newCount = Interlocked.Decrement(ref _currentCount);
            if (newCount <= 0)
            {
                _event.Set();
            }
        }

        /// <summary>
        /// Gets the number of signals initially required to release the latch.
        /// </summary>
        public int InitialCount => _initialCount;

        /// <summary>
        /// Returns the current count.
        /// <para/>
        /// This property is typically used for debugging and testing purposes.
        /// </summary>
        /// <remarks>
        /// In Java, <c>getCount()</c> returns <c>long</c>, but internally it's
        /// always an int. Here we retain the Java return type. There is no need
        /// to make the internal count 64-bit.
        /// <para/>
        /// The returned value is clamped at zero. The internal counter may
        /// briefly drop below zero under concurrent <see cref="Signal()"/>
        /// calls that race past the early-return guard, but that detail is
        /// hidden here to match Java's <c>getCount()</c> semantics, which never
        /// returns a negative value.
        /// <para/>
        /// The count reaching zero and the release of waiting threads are not a
        /// single atomic transition: a thread may briefly observe this property
        /// as zero before waiters are released. <see cref="CountdownEvent"/>
        /// exhibits the same behavior.
        /// </remarks>
        public long CurrentCount => Math.Max(0, _currentCount);

        /// <summary>
        /// Returns a string identifying this latch, as well as its state.
        /// The state, in brackets, includes the string <c>"Count ="</c>
        /// followed by the current count.
        /// </summary>
        /// <returns>A string identifying this latch, as well as its state.</returns>
        public override string ToString() => $"{base.ToString()}[Count = {CurrentCount}]";

        /// <summary>
        /// Releases all resources used by the current instance of <see cref="CountdownLatch"/>.
        /// </summary>
        /// <remarks>
        /// Java's <c>CountDownLatch</c> is not <c>AutoCloseable</c>
        /// and does not need to be closed. This .NET port wraps a
        /// <see cref="ManualResetEventSlim"/>, which is disposable, so the latch
        /// must be disposed when no longer needed.
        /// <para/>
        /// This method is not thread-safe with respect to the other members of
        /// this class. Only call it once all threads have finished waiting on
        /// and counting down the latch: disposing while a thread is blocked in
        /// one of the <see cref="Wait()"/> overloads leaves that thread waiting
        /// indefinitely.
        /// </remarks>
        public void Dispose()
        {
            _disposed = true;
            _event.Dispose();
        }
    }
}
