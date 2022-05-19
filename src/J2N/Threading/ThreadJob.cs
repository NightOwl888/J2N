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

using System;
#if FEATURE_EXCEPTIONDISPATCHINFO
using System.Runtime.ExceptionServices;
#endif
using System.Threading;


namespace J2N.Threading
{
    /// <summary>
    /// Base class used to handle threads that is
    /// inheritable just like the Thread type in Java.
    /// This class also ensures that when an error is thrown
    /// on a background thread, it will be properly re-thrown
    /// on the calling thread.
    /// <para/>
    /// Usage Note: Where possible, prefer to port Java threads
    /// to use TPL and/or the .NET <see cref="Thread"/> class.
    /// This class is provided as a quick solution to
    /// porting code that subclasses the Thread class in Java, but
    /// the end result is less than ideal, so it is recommended to only
    /// use this class for porting tests. However, it is stable enough
    /// to use in production scenarios.
    /// </summary>
    public class ThreadJob : IEquatable<ThreadJob>, IEquatable<Thread> //: IRunnable
    {
        /// <summary>
        /// The <see cref="ThreadStart"/> delegate.
        /// This object will be used for synchronization.
        /// </summary>
        private readonly ThreadStart threadStart;

        /// <summary>
        /// The instance of <see cref="Thread"/>.
        /// </summary>
        private Thread thread;

        /// <summary>
        /// The name of the thread before it is started.
        /// </summary>
        private string? name;

        /// <summary>
        /// The exception (if any) caught on the running thread
        /// that will be re-thrown on the calling thread after
        /// calling <see cref="Join()"/>, <see cref="Join(long)"/>, 
        /// or <see cref="Join(long, int)"/>.
        /// </summary>
        private Exception? exception;

        /// <summary>
        /// Initializes a new instance of the <see cref="ThreadJob"/> class.
        /// </summary>
        public ThreadJob()
        {
            this.threadStart = Run;
            this.thread = new Thread(() => SafeRun(this.threadStart));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ThreadJob"/> class
        /// with the provided <paramref name="threadName"/>.
        /// </summary>
        /// <param name="threadName">The name of the thread</param>
        public ThreadJob(string threadName)
        {
            this.threadStart = Run;
            this.thread = new Thread(() => SafeRun(this.threadStart));
            this.name = threadName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ThreadJob"/> class
        /// with the provided <paramref name="threadStart"/> delegate.
        /// </summary>
        /// <param name="threadStart">A <see cref="ThreadStart"/> delegate that 
        /// references the method to be invoked when this thread begins executing.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="threadStart"/> is <c>null</c>.</exception>
        public ThreadJob(ThreadStart threadStart)
        {
            this.threadStart = threadStart ?? throw new ArgumentNullException(nameof(threadStart));
            this.thread = new Thread(() => SafeRun(this.threadStart));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ThreadJob"/> class
        /// with the provided <paramref name="threadStart"/> delegate and
        /// <paramref name="threadName"/>.
        /// </summary>
        /// <param name="threadStart">A ThreadStart delegate that references the method
        /// to be invoked when this thread begins executing.</param>
        /// <param name="threadName">The name of the thread</param>
        /// <exception cref="ArgumentNullException">If <paramref name="threadStart"/> is <c>null</c>.</exception>
        public ThreadJob(ThreadStart threadStart, string threadName)
        {
            this.threadStart = threadStart ?? throw new ArgumentNullException(nameof(threadStart));
            this.thread = new Thread(() => SafeRun(this.threadStart));
            this.name = threadName;
        }

       

        //// Constructor for testing resume
        // TODO: Finish implementation
        //internal ThreadJob(Thread thread, string threadName)
        //{
        //    this.thread = thread ?? throw new ArgumentNullException(nameof(thread));
        //    this.name = threadName;
        //}

        /// <summary>
        /// Safely starts the method passed to <paramref name="start"/> and stores any exception that is
        /// thrown. The first exception will stop execution of the method passed to <paramref name="start"/>
        /// and it will be re-thrown on the calling thread after it calls <see cref="Join()"/>,
        /// <see cref="Join(long)"/>, or <see cref="Join(long, int)"/>.
        /// </summary>
        /// <param name="start">A <see cref="ThreadStart"/> delegate that references the method
        /// to be invoked when this thread begins executing.</param>
        protected virtual void SafeRun(ThreadStart start)
        {
            try
            {
                start.Invoke();
            }
            catch (Exception ex) when (!IsThreadingException(ex))
            {
                // Catch the first excption so it can be re-thrown
                // on the calling thread when Join is called.
                exception = ex;
                exception.Data["OriginalMessage"] = ex.ToString();
            }
        }

        private static bool IsThreadingException(Exception e) // J2N specific - marked static
        {
            // NOTE: We don't swallow ThreadAbortException because we don't want
            // to interfere with normal operation of the thread.
            // We also had ThreadInterruptedExcption here once, but Lucene.NET requires
            // it to be rethrown on the calling thread.
            return
#if FEATURE_THREADABORT
                e.GetType().Equals(typeof(ThreadAbortException)) ||
#endif
                false;
        }

        /// <summary>
        /// Invokes the <see cref="ThreadStart"/> delegate that was passed into the constructor.
        /// If no <see cref="ThreadStart"/> was set, does nothing.
        /// Alternatively, this method may be overridden by subclasses to provide an implementation
        /// for the thread to run.
        /// </summary>
        public virtual void Run()
        {
        }

        /// <summary>
        /// Starts the new <see cref="Thread"/> of execution. The <see cref="Run()"/> method of the
        /// receiver will be called by the receiver <see cref="Thread"/> itself (and not the
        /// <see cref="Thread"/> calling <see cref="Start()"/>).
        /// </summary>
        public virtual void Start()
        {
            if (name != null)
                thread.Name = name;
            thread.Start();
        }

        /// <summary>
        /// Interrupts a thread that is in the <see cref="ThreadState.WaitSleepJoin"/> thread state.
        /// </summary>
        public virtual void Interrupt()
        {
            thread.Interrupt();
        }

        /// <summary>
        /// Gets an object that can be used to synchronize with this instance.
        /// </summary>
        public object SyncRoot => threadStart;

        /// <summary>
        /// Gets the current thread instance.
        /// </summary>
        public Thread Instance
        {
            get => thread;
            private set => thread = value;
        }

        [ThreadStatic]
        private static ThreadJob? thisInstance = null;

        /// <summary>
        /// Gets the currently running thread as a <see cref="ThreadJob"/>.
        /// </summary>
        /// <returns>The currently running thread as a <see cref="ThreadJob"/>.</returns>
        public static ThreadJob CurrentThread
        {
            get
            {
                if (thisInstance is null)
                {
                    thisInstance = new ThreadJob
                    {
                        Instance = Thread.CurrentThread
                    };
                }
                return thisInstance;
            }
        }

        /// <summary>
        /// Gets or sets the name of the thread.
        /// </summary>
        /// <exception cref="ArgumentNullException">When setting the value to <c>null</c>.</exception>
        public string? Name
        {
            get => name;
            set => this.name = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// Gets the current state of the thread. This propery is
        /// useful for monitoring purposes.
        /// </summary>
        public ThreadState State => thread.ThreadState;


        // TODO: Add extension method
        ///// <summary>
        ///// Set if the receiver is a daemon thread or not. This can only be done
        ///// before the thread starts running.
        ///// </summary>
        ///// <param name="isDaemon"></param>
        //public void SetDaemon(bool isDaemon)
        //{
        //    thread.IsBackground = isDaemon;
        //}

        /// <summary>
        /// Gets or sets a value indicating the scheduling priority of a thread.
        /// </summary>
        public ThreadPriority Priority // J2N TODO: Remove try/catch when setting here - we may need feedback. Need to test against Lucene.NET.
        {
            get
            {
                try
                {
                    return thread.Priority;
                }
                catch
                {
                    return ThreadPriority.Normal;
                }
            }
            set
            {
                try
                {
                    thread.Priority = value;
                }
                catch { }
            }
        }

        /// <summary>
        /// Gets a value indicating the execution status of the current thread
        /// </summary>
        public bool IsAlive => thread.IsAlive;

        /// <summary>
        /// Gets or sets a value indicating whether or not a thread is a background thread.
        /// A background thread only runs as long as there are non-background threads running.
        /// When the last non-background thread ends, the whole program ends no matter if it had
        /// background threads running or not.
        /// </summary>
        public bool IsBackground
        {
            get => thread.IsBackground;
            set => thread.IsBackground = value;
        }

        /// <summary>
        /// If <c>true</c> when <see cref="Join()"/>, <see cref="Join(long)"/> or <see cref="Join(long, int)"/> is called,
        /// any original exception and error message will be wrapped into a new <see cref="Exception"/> when thrown, so
        /// debugging tools will show the correct stack trace information.
        /// <para/>
        /// NOTE: This changes the original exception type to <see cref="Exception"/>, so this setting should not be used if
        /// control logic depends on the specific exception type being thrown. An alternative way to get the original
        /// <see cref="Exception.ToString()"/> message is to use <c>exception.Data["OriginalMessage"].ToString()</c>.
        /// </summary>
#if FEATURE_EXCEPTIONDISPATCHINFO
        [Obsolete("This setting no longer has any effect, as we are throwing the exception using ExceptionDispatchInfo so the stack trace is 'always on'.")]
#endif
        public bool IsDebug { get; set; }

        private void RethrowFirstException()
        {
            if (exception != null)
            {
#if FEATURE_EXCEPTIONDISPATCHINFO
                ExceptionDispatchInfo.Capture(exception).Throw();
#else
                if (IsDebug)
                    throw new Exception(exception.Data["OriginalMessage"]!.ToString(), exception);
                else
                    throw exception;
#endif
            }
        }

        /// <summary>
        /// Blocks the calling thread until a thread terminates.
        /// </summary>
        public void Join()
        {
            thread.Join();
            RethrowFirstException();
        }

        /// <summary>
        /// Blocks the calling thread until a thread terminates or the specified time elapses.
        /// </summary>
        /// <param name="milliSeconds">Time of wait in milliseconds</param>
        public void Join(long milliSeconds)
        {
            thread.Join(Convert.ToInt32(milliSeconds));
            RethrowFirstException();
        }

        /// <summary>
        /// Blocks the calling thread until a thread terminates or the specified time elapses.
        /// </summary>
        /// <param name="milliSeconds">Time of wait in milliseconds.</param>
        /// <param name="nanoSeconds">Time of wait in nanoseconds.</param>
        public void Join(long milliSeconds, int nanoSeconds)
        {
            int totalTime = Convert.ToInt32(milliSeconds + (nanoSeconds * 0.000001));

            thread.Join(totalTime);
            RethrowFirstException();
        }

        /// <summary>
        /// Resumes a thread that has been suspended. This is a no-op if the receiver
        /// was never suspended, or suspended and already resumed. If the receiver is
        /// suspended, this method remumes at the point where it was when it was suspended.
        /// </summary>
        [Obsolete("Resume() is not supported in .NET Core. Use Monitor.PulseAll(SyncLock) instead. This method will be removed in J2N 3.0.")]
        public void Resume()
        {
            Monitor.PulseAll(SyncRoot);
        }

#if FEATURE_THREADABORT

        /// <summary>
        /// Raises a <see cref="ThreadAbortException"/> in the thread on which it is invoked,
        /// to begin the process of terminating the thread. Calling this method
        /// usually terminates the thread.
        /// </summary>
        public void Abort()
        {
            thread.Abort();
        }

        /// <summary>
        /// Raises a <see cref="ThreadAbortException"/> in the thread on which it is invoked,
        /// to begin the process of terminating the thread while also providing
        /// exception information about the thread termination.
        /// Calling this method usually terminates the thread.
        /// </summary>
        /// <param name="stateInfo">An object that contains application-specific information, such as state, which can be used by the thread being aborted</param>
        public void Abort(object stateInfo)
        {
            thread.Abort(stateInfo);
        }
#endif

        /// <summary>
        /// Causes the calling <see cref="Thread"/> to yield execution time to another <see cref="Thread"/> that
        /// is ready to run. The actual scheduling is implementation-dependent.
        /// </summary>
        public static void Yield()
        {
            Thread.Yield();
        }

        /// <summary>
        /// Suspends the thread. This is a no-op if the receiver is already suspended.
        /// If the receiver state is <see cref="IsAlive"/>, it will be suspended until
        /// <see cref="Resume()"/> (or <see cref="Monitor.PulseAll(object)"/>) is called.
        /// Suspend requests are not queued, which means that N requests are equivalent to
        /// just one - only one resume request is needed in this case.
        /// </summary>
        [Obsolete("Suspend() is not supported in .NET Core. Use Monitor.Wait(SyncLock) instead. This method will be removed in J2N 3.0.")]
        public void Suspend()
        {
            Monitor.Wait(SyncRoot);
        }

        /// <summary>
        /// Causes the thread which sent this message to sleep for the given interval
        /// of time (given in milliseconds). The precision is not guaranteed - the
        /// <see cref="Thread"/> may sleep more or less than requested.
        /// </summary>
        /// <param name="milliSeconds">The time to sleep in milliseconds.</param>
        /// <seealso cref="Interrupt()"/>
        public static void Sleep(long milliSeconds)
        {
            // casting long ms to int ms could lose resolution, however unlikely
            // that someone would want to sleep for that long...
            Thread.Sleep((int)milliSeconds);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="milliSeconds"></param>
        /// <param name="nanoSeconds"></param>
        public static void Sleep(long milliSeconds, int nanoSeconds)
        {
            int totalTime = Convert.ToInt32(milliSeconds + (nanoSeconds * 0.000001));
            Thread.Sleep(totalTime);
        }

        /// <summary>
        /// Suspends the current thread for the specified amount of time.
        /// </summary>
        /// <param name="timeout">The amount of time for which the thread is suspended.
        /// If the value of the <c>millisecondsTimeout</c> argument is <see cref="TimeSpan.Zero"/>,
        /// the thread relinquishes the remainder of its time slice to any thread of equal
        /// priority that is ready to run. If there are no other threads of equal priority
        /// that are ready to run, execution of the current thread is not suspended.</param>
        /// <exception cref="ArgumentOutOfRangeException">The value of <paramref name="timeout"/> is negative and
        /// is not equal to <see cref="Timeout.Infinite"/> in milliseconds, or is greater than <see cref="int.MaxValue"/>
        /// milliseconds.</exception>
        public static void Sleep(TimeSpan timeout) => Thread.Sleep(timeout);

        /// <summary>
        /// Java has Thread.interrupted() which returns, and clears, the interrupt
        /// flag of the current thread. .NET has no such method, so we're calling
        /// Thread.Sleep to provoke the exception which will also clear the flag.
        /// </summary>
        /// <returns><c>true</c> if the current thread (<see cref="CurrentThread"/>)
        /// has a bending interrupt request; othewise, <c>false</c>.</returns>
        public static bool Interrupted()
        {
            try
            {
                Thread.Sleep(0);
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (ThreadInterruptedException)
            {
                return true;
            }
#pragma warning restore CA1031 // Do not catch general exception types
            return false;
        }

        /// <summary>
        /// Compares <paramref name="t1"/> and <paramref name="t2"/> for equality.
        /// </summary>
        /// <param name="t1">A <see cref="ThreadJob"/> instance.</param>
        /// <param name="t2">A <see cref="ThreadJob"/> or <see cref="Thread"/> instance to compare <paramref name="t1"/> to.</param>
        /// <returns><c>true</c> if the given instances are equal; otherwise, <c>false</c>.</returns>
        public static bool operator ==(ThreadJob t1, object t2)
        {
            if (((object)t1) == null) return t2 == null;
            return t1.Equals(t2);
        }

        /// <summary>
        /// Compares <paramref name="t1"/> and <paramref name="t2"/> for inequality.
        /// </summary>
        /// <param name="t1">A <see cref="ThreadJob"/> instance.</param>
        /// <param name="t2">A <see cref="ThreadJob"/> or <see cref="Thread"/> instance to compare <paramref name="t1"/> to.</param>
        /// <returns><c>true</c> if the given instances are not equal; otherwise, <c>false</c>.</returns>
        public static bool operator !=(ThreadJob t1, object t2)
        {
            return !(t1 == t2);
        }

        /// <summary>
        /// Determines whether <paramref name="other"/> is equal to the current object.
        /// </summary>
        /// <param name="other">The object to compare with the current object.</param>
        /// <returns><c>true</c> if the specified object is equal to the current object; otherwise, <c>false</c>.</returns>
        public bool Equals(Thread? other)
        {
            if (other is null)
                return thread is null;
            return this.thread.Equals(other);
        }

        /// <summary>
        /// Determines whether <paramref name="other"/> is equal to the current object.
        /// </summary>
        /// <param name="other">The object to compare with the current object.</param>
        /// <returns><c>true</c> if the specified object is equal to the current object; otherwise, <c>false</c>.</returns>
        public bool Equals(ThreadJob? other)
        {
            if (other is null)
                return thread is null;
            return this.thread.Equals(other);
        }

        /// <summary>
        /// Determines whether <paramref name="obj"/> is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns><c>true</c> if the specified object is equal to the current object; otherwise, <c>false</c>.</returns>
        public override bool Equals(object? obj)
        {
            if (obj is ThreadJob other)
                return this.thread.Equals(other.thread);
            if (obj is Thread otherThread)
                return this.thread.Equals(otherThread);
            return false;
        }

        /// <summary>
        /// Returns a hash code for the current thread.
        /// </summary>
        /// <returns>An integer hash code value.</returns>
        public override int GetHashCode()
        {
            return this.thread.GetHashCode();
        }

        /// <summary>
        /// Obtain a <see cref="string"/> that represents the current object.
        /// </summary>
        /// <returns>A <see cref="string"/> that represents the current object.</returns>
        public override string ToString()
        {
            return "Thread[" + Name + "," + Priority.ToString() + "]";
        }
    }
}
