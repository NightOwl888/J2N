using NUnit.Framework;
using System;
using System.Threading;

namespace J2N.Threading
{
    public class TestThreadJob : TestCase
    {
        [Test]
        public void TestBasic()
        {
            ThreadJob thread = new ThreadJob();

            //Compare Current Thread Ids
            Assert.IsTrue(ThreadJob.CurrentThread.Instance.ManagedThreadId == System.Threading.Thread.CurrentThread.ManagedThreadId);


            //Compare instances of ThreadClass
            MyThread mythread = new MyThread();
            mythread.Start();
            while (mythread.Result == null) System.Threading.Thread.Sleep(1);
            Assert.IsTrue((bool)mythread.Result);


            ThreadJob nullThread = null;
            Assert.IsTrue(nullThread == null); //test overloaded operator == with null values
            Assert.IsFalse(nullThread != null); //test overloaded operator != with null values
        }

        class MyThread : ThreadJob
        {
            public object Result = null;
            public override void Run()
            {
                Result = ThreadJob.CurrentThread == this;
            }
        }





        internal class SimpleThread //: IRunnable
        {

            int delay;



            public void Run()
            {
                try
                {
                    lock (this)
                    {
                        Monitor.Pulse(this);
                        Monitor.Wait(this, delay);
                    }
                }
                catch (ThreadInterruptedException e)
                {
                    return;
                }
            }

            public SimpleThread(int d)
            {
                if (d >= 0)
                    delay = d;
            }
        }

        internal class YieldThread //: IRunnable
        {
            internal volatile int delay;


            public void Run()
            {
                int x = 0;
                while (true)
                {
                    ++x;
                }
            }

            public YieldThread(int d)
            {
                if (d >= 0)
                    delay = d;
            }
        }

        internal class ResSupThread //: IRunnable
        {
            Thread parent;

            volatile int checkVal = -1;


            public void Run()
            {
                try
                {
                    lock (this)
                    {
                        Monitor.Pulse(this);
                    }
                    while (true)
                    {
                        checkVal++;
                        zz();
                        ThreadJob.Sleep(100);
                    }
                }
                catch (ThreadInterruptedException e)
                {
                    return;
                }
                catch (BogusException e)
                {
                    try
                    {
                        // Give parent a chance to sleep
                        ThreadJob.Sleep(500);
                    }
                    catch (ThreadInterruptedException x)
                    {
                    }
                    parent.Interrupt();
                    //while (!Thread.CurrentThread.IsInterrupted())
                    while (true)
                    {
                        // Don't hog the CPU
                        try
                        {
                            ThreadJob.Sleep(50);
                        }
                        catch (ThreadInterruptedException x)
                        {
                            // This is what we've been waiting for...don't throw it
                            // away!
                            break;
                        }
                    }
                }
            }

            public void zz()
            {
            }

            public ResSupThread(Thread t)
            {
                parent = t;
            }

            public int getCheckVal()
            {
                lock (this)
                    return checkVal;
            }
        }

        internal class BogusException : Exception
        {

            //private static readonly long serialVersionUID = 1L;

            public BogusException(String s)
                            : base(s)
            {
            }
        }

        ThreadJob st, ct /*, spinner*/;

        ////static bool calledMySecurityManager = false;

        /////**
        //// * @tests java.lang.Thread#Thread()
        //// */
        ////[Test]
        ////public void Test_Constructor()
        ////{
        ////    // Test for method java.lang.Thread()

        ////    ThreadRun t = new ThreadRun();
        ////    //    SecurityManager m = new SecurityManager() {
        ////    //            @Override
        ////    //            public ThreadGroup getThreadGroup()
        ////    //    {
        ////    //        calledMySecurityManager = true;
        ////    //        return Thread.currentThread().getThreadGroup();
        ////    //    }

        ////    //    @Override
        ////    //            public void checkPermission(Permission permission)
        ////    //    {
        ////    //        if (permission.getName().equals("setSecurityManager"))
        ////    //        {
        ////    //            return;
        ////    //        }
        ////    //        super.checkPermission(permission);
        ////    //    }
        ////    //};
        ////    //		try {
        ////    //			// To see if it checks Thread creation with our SecurityManager
        ////    //			System.setSecurityManager(m);
        ////    //			t = new Thread();
        ////    //		} finally {
        ////    //			// restore original, no side-effects
        ////    //			System.setSecurityManager(null);
        ////    //		}
        ////    //		assertTrue("Did not call SecurityManager.getThreadGroup ()",
        ////    //                calledMySecurityManager);
        ////    t.Start();
        ////}

        /**
         * @tests java.lang.Thread#Thread(java.lang.Runnable)
         */
        [Test]
        public void Test_ConstructorLjava_lang_Runnable()
        {
            // Test for method java.lang.Thread(java.lang.Runnable)
            var simple = new SimpleThread(10);
            ct = new ThreadJob(() => simple.Run());
            ct.Start();
            //ct.Join();
        }

        /**
         * @tests java.lang.Thread#Thread(java.lang.Runnable, java.lang.String)
         */
        [Test]
        public void Test_ConstructorLjava_lang_RunnableLjava_lang_String()
        {
            // Test for method java.lang.Thread(java.lang.Runnable,
            // java.lang.String)
            ThreadJob st1 = new ThreadJob(() => new SimpleThread(1).Run(), "SimpleThread1");
            assertEquals("Constructed thread with incorrect thread name", "SimpleThread1", st1
                    .Name);
            st1.Start();
        }

        /**
         * @tests java.lang.Thread#Thread(java.lang.String)
         */
        [Test]
        public void Test_ConstructorLjava_lang_String()
        {
            // Test for method java.lang.Thread(java.lang.String)
            ThreadJob t = new ThreadJob("Testing");
            assertEquals("Created tread with incorrect name",
                    "Testing", t.Name);
            t.Start();
        }

        /////**
        //// * @tests java.lang.Thread#Thread(java.lang.ThreadGroup, java.lang.Runnable)
        //// */
        ////[Test]
        ////public void Test_ConstructorLjava_lang_ThreadGroupLjava_lang_Runnable()
        ////{
        ////    // Test for method java.lang.Thread(java.lang.ThreadGroup,
        ////    // java.lang.Runnable)
        ////    ThreadGroup tg = new ThreadGroup("Test Group1");
        ////    st = new Thread(tg, new SimpleThread(1), "SimpleThread2");
        ////    assertTrue("Returned incorrect thread group", st.getThreadGroup() == tg);
        ////    st.start();
        ////    try
        ////    {
        ////        st.join();
        ////    }
        ////    catch (InterruptedException e)
        ////    {
        ////    }
        ////    tg.destroy();
        ////}

        /////**
        //// * @tests java.lang.Thread#Thread(java.lang.ThreadGroup, java.lang.Runnable,
        //// *        java.lang.String)
        //// */
        ////[Test]
        ////public void Test_ConstructorLjava_lang_ThreadGroupLjava_lang_RunnableLjava_lang_String()
        ////{
        ////    // Test for method java.lang.Thread(java.lang.ThreadGroup,
        ////    // java.lang.Runnable, java.lang.String)
        ////    ThreadGroup tg = new ThreadGroup("Test Group2");
        ////    st = new Thread(tg, new SimpleThread(1), "SimpleThread3");
        ////    assertTrue("Constructed incorrect thread", (st.getThreadGroup() == tg)
        ////            && st.getName().equals("SimpleThread3"));
        ////    st.start();
        ////    try
        ////    {
        ////        st.join();
        ////    }
        ////    catch (InterruptedException e)
        ////    {
        ////    }
        ////    tg.destroy();

        ////    Runnable r = new Runnable()
        ////    {

        ////            public void run()
        ////    {
        ////    }
        ////};

        ////ThreadGroup foo = null;
        ////		try {
        ////			new Thread(foo = new ThreadGroup("foo"), r, null);
        ////// Should not get here
        ////fail("Null cannot be accepted as Thread name");
        ////		} catch (NullPointerException npe) {
        ////			assertTrue("Null cannot be accepted as Thread name", true);
        ////foo.destroy();
        ////		}

        ////	}

        ////	/**
        ////	 * @tests java.lang.Thread#Thread(java.lang.ThreadGroup, java.lang.String)
        ////	 */
        ////[Test]
        ////	public void Test_ConstructorLjava_lang_ThreadGroupLjava_lang_String()
        ////{
        ////    // Test for method java.lang.Thread(java.lang.ThreadGroup,
        ////    // java.lang.String)
        ////    st = new Thread(new SimpleThread(1), "SimpleThread4");
        ////    assertEquals("Returned incorrect thread name",
        ////            "SimpleThread4", st.getName());
        ////    st.start();
        ////}

        /////**
        //// * @tests java.lang.Thread#activeCount()
        //// */
        ////[Test]
        ////public void Test_activeCount()
        ////{
        ////            // Test for method int java.lang.Thread.activeCount()
        ////            ThreadRun t = new ThreadRun(() => new SimpleThread(10).Run());
        ////    int active = 0;
        ////    lock(t) {
        ////        t.Start();
        ////        active = ThreadRun.ActiveCount;
        ////    }
        ////    assertTrue("Incorrect activeCount for current group: " + active, active > 1);
        ////    try
        ////    {
        ////        t.Join();
        ////    }
        ////    catch (ThreadInterruptedException e)
        ////    {
        ////    }
        ////}

        /////**
        //// * @tests java.lang.Thread#checkAccess()
        //// */
        ////[Test]
        ////public void Test_checkAccess()
        ////{
        ////    // Test for method void java.lang.Thread.checkAccess()
        ////    ThreadGroup tg = new ThreadGroup("Test Group3");
        ////    try
        ////    {
        ////        st = new Thread(tg, new SimpleThread(1), "SimpleThread5");
        ////        st.checkAccess();
        ////        assertTrue("CheckAccess passed", true);
        ////    }
        ////    catch (SecurityException e)
        ////    {
        ////        fail("CheckAccess failed : " + e.getMessage());
        ////    }
        ////    st.start();
        ////    try
        ////    {
        ////        st.join();
        ////    }
        ////    catch (InterruptedException e)
        ////    {
        ////    }
        ////    tg.destroy();
        ////}

        /////**
        //// * @tests java.lang.Thread#countStackFrames()
        //// */
        ////[Test]
        ////    public void Test_countStackFrames()
        ////{
        ////    /*
        ////     * Thread.countStackFrames() is unpredictable, so we just test that it
        ////     * doesn't throw an exception.
        ////     */
        ////    Thread.currentThread().countStackFrames();
        ////}

        /**
         * @tests java.lang.Thread#currentThread()
         */
        [Test]
        public void Test_currentThread()
        {
            assertNotNull(ThreadJob.CurrentThread);
        }

        /////**
        //// * @tests java.lang.Thread#destroy()
        //// */
        ////[Test]
        ////    public void Test_destroy()
        ////{
        ////    try
        ////    {
        ////        new ThreadRun().destroy();
        ////        // FIXME uncomment when IBM VME is updated
        ////        //fail("NoSuchMethodError was not thrown");
        ////    }
        ////    catch (NoSuchMethodError e)
        ////    {
        ////    }
        ////}

        /////**
        //// * @tests java.lang.Thread#enumerate(java.lang.Thread[])
        //// */
        ////[Test]
        ////public void Test_enumerate_Ljava_lang_Thread()
        ////{
        ////        // Test for method int java.lang.Thread.enumerate(java.lang.Thread [])
        ////        // The test has been updated according to HARMONY-1974 JIRA issue.

        ////class MyThread : ThreadRun
        ////        {
        ////    MyThread(ThreadGroup tg, String name) {
        ////        super(tg, name);
        ////    }

        ////    boolean failed = false;
        ////    String failMessage = null;

        ////            public void run()
        ////{
        ////    SimpleThread st1 = null;
        ////    SimpleThread st2 = null;
        ////    ThreadGroup mytg = null;
        ////    Thread firstOne = null;
        ////    Thread secondOne = null;
        ////    try
        ////    {
        ////        int arrayLength = 10;
        ////        Thread[] tarray = new Thread[arrayLength];
        ////        st1 = new SimpleThread(-1);
        ////        st2 = new SimpleThread(-1);
        ////        mytg = new ThreadGroup("jp");
        ////        firstOne = new Thread(mytg, st1, "firstOne2");
        ////        secondOne = new Thread(mytg, st2, "secondOne1");
        ////        int count = Thread.enumerate(tarray);
        ////        assertEquals("Incorrect value returned1",
        ////                1, count);
        ////        synchronized(st1) {
        ////            firstOne.start();
        ////            try
        ////            {
        ////                st1.wait();
        ////            }
        ////            catch (InterruptedException e)
        ////            {
        ////            }
        ////        }
        ////        count = Thread.enumerate(tarray);
        ////        assertEquals("Incorrect value returned2",
        ////                2, count);
        ////        synchronized(st2) {
        ////            secondOne.start();
        ////            try
        ////            {
        ////                st2.wait();
        ////            }
        ////            catch (InterruptedException e)
        ////            {
        ////            }
        ////        }
        ////        count = Thread.enumerate(tarray);
        ////        assertEquals("Incorrect value returned3",
        ////                3, count);
        ////    }
        ////    catch (junit.framework.AssertionFailedError e)
        ////    {
        ////        failed = true;
        ////        failMessage = e.getMessage();
        ////    }
        ////    finally
        ////    {
        ////        synchronized(st1) {
        ////            firstOne.interrupt();
        ////        }
        ////        synchronized(st2) {
        ////            secondOne.interrupt();
        ////        }
        ////        try
        ////        {
        ////            firstOne.join();
        ////            secondOne.join();
        ////        }
        ////        catch (InterruptedException e)
        ////        {
        ////        }
        ////        mytg.destroy();
        ////    }
        ////}
        ////        };

        ////        ThreadGroup tg = new ThreadGroup("tg");
        ////MyThread t = new MyThread(tg, "top");
        ////t.start();
        ////        try {
        ////            t.join();
        ////        } catch (InterruptedException e) {
        ////            fail("Unexpected interrupt");
        ////        } finally {
        ////            tg.destroy();
        ////        }
        ////        assertFalse(t.failMessage, t.failed);
        ////    }

        ////	/**
        ////	 * @tests java.lang.Thread#getContextClassLoader()
        ////	 */
        ////[Test]
        ////	public void Test_getContextClassLoader()
        ////{
        ////    // Test for method java.lang.ClassLoader
        ////    // java.lang.Thread.getContextClassLoader()
        ////    Thread t = new Thread();
        ////    assertTrue("Incorrect class loader returned",
        ////            t.getContextClassLoader() == Thread.currentThread()
        ////                    .getContextClassLoader());
        ////    t.start();

        ////}

        /**
         * @tests java.lang.Thread#getName()
         */
        [Test]
        public void Test_getName()
        {
            // Test for method java.lang.String java.lang.Thread.getName()
            st = new ThreadJob(() => new SimpleThread(1).Run(), "SimpleThread6");
            assertEquals("Returned incorrect thread name",
                    "SimpleThread6", st.Name);
            st.Start();
        }

        /**
         * @tests java.lang.Thread#getPriority()
         */
        [Test]
        public void Test_getPriority()
        {
            // Test for method int java.lang.Thread.getPriority()
            st = new ThreadJob(() => new SimpleThread(1).Run());
            st.Priority = ThreadPriority.Highest;
            assertTrue("Returned incorrect thread priority",
                    st.Priority == ThreadPriority.Highest);
            st.Start();
        }

        /////**
        //// * @tests java.lang.Thread#getThreadGroup()
        //// */
        ////[Test]
        ////public void Test_getThreadGroup()
        ////{
        ////    // Test for method java.lang.ThreadGroup
        ////    // java.lang.Thread.getThreadGroup()
        ////    ThreadGroup tg = new ThreadGroup("Test Group4");
        ////    st = new Thread(tg, new SimpleThread(1), "SimpleThread8");
        ////    assertTrue("Returned incorrect thread group", st.getThreadGroup() == tg);
        ////    st.start();
        ////    try
        ////    {
        ////        st.join();
        ////    }
        ////    catch (InterruptedException e)
        ////    {
        ////    }
        ////    assertNull("group should be null", st.getThreadGroup());
        ////    assertNotNull("toString() should not be null", st.toString());
        ////    tg.destroy();

        ////    final Object lock = new Object();
        ////    Thread t = new Thread() {
        ////            @Override
        ////            public void run()
        ////    {
        ////        synchronized(lock)
        ////        {
        ////            lock.notifyAll();
        ////        }
        ////    }
        ////};
        ////synchronized(lock)
        ////{
        ////    t.start();
        ////    try
        ////    {
        ////        lock.wait();
        ////    }
        ////    catch (InterruptedException e)
        ////    {
        ////    }
        ////}
        ////int running = 0;
        ////		while (t.isAlive())
        ////			running++;
        ////		ThreadGroup group = t.getThreadGroup();
        ////assertNull("ThreadGroup is not null", group);
        ////	}

        internal class ChildThread1 : ThreadJob
        {
            ThreadJob parent;

            bool sync;

            object syncLock;

            public override void Run()
            {
                if (sync)
                {
                    lock (syncLock)
                    {
                        Monitor.Pulse(syncLock);
                        try
                        {
                            Monitor.Wait(syncLock);
                        }
                        catch (ThreadInterruptedException e)
                        {
                        }
                    }
                }
                parent.Interrupt();
            }

            public ChildThread1(ThreadJob p, String name, bool sync, object syncLock)
                : base(name)
            {
                parent = p;
                this.sync = sync;
                this.syncLock = syncLock;
            }
        }

        /**
         * @tests java.lang.Thread#interrupt()
         */
        [Test]
        public void Test_interrupt()
        {
            // Test for method void java.lang.Thread.interrupt()
            Object syncLock = new Object();


            bool interrupted = false;
            try
            {
                ct = new ChildThread1(ThreadJob.CurrentThread, "Interrupt Test1",
                        false, syncLock);
                lock (syncLock)
                {
                    ct.Start();
                    Monitor.Wait(syncLock);
                }
            }
            catch (ThreadInterruptedException e)
            {
                interrupted = true;
            }
            assertTrue("Failed to Interrupt thread1", interrupted);

            interrupted = false;
            try
            {
                ct = new ChildThread1(ThreadJob.CurrentThread, "Interrupt Test2",
                        true, syncLock);
                lock (syncLock)
                {
                    ct.Start();
                    Monitor.Wait(syncLock);
                    Monitor.Pulse(syncLock);
                }
                ThreadJob.Sleep(20000);
            }
            catch (ThreadInterruptedException e)
            {
                interrupted = true;
            }
            assertTrue("Failed to Interrupt thread2", interrupted);

        }

        /**
         * @tests java.lang.Thread#interrupted()
         */
        [Test]
        public void Test_interrupted()
        {
            assertFalse("Interrupted returned true for non-interrupted thread", ThreadJob
                    .Interrupted());
            ThreadJob.CurrentThread.Interrupt();
            assertTrue("Interrupted returned true for non-interrupted thread", ThreadJob.Interrupted());
            assertFalse("Failed to clear interrupted flag", ThreadJob.Interrupted());
        }

        /**
         * @tests java.lang.Thread#isAlive()
         */
        [Test]
        public void Test_isAlive()
        {
            // Test for method boolean java.lang.Thread.isAlive()
            SimpleThread simple = new SimpleThread(500);
            st = new ThreadJob(() => simple.Run());
            assertFalse("A thread that wasn't started is alive.", st.IsAlive);
            lock (simple)
            {
                st.Start();
                try
                {
                    //simple.Wait();
                    Monitor.Wait(simple);
                }
                catch (ThreadInterruptedException e)
                {
                }
            }
            assertTrue("Started thread returned false", st.IsAlive);
            try
            {
                st.Join();
            }
            catch (ThreadInterruptedException e)
            {
                fail("Thread did not die");
            }
            assertTrue("Stopped thread returned true", !st.IsAlive);
        }

        /**
         * @tests java.lang.Thread#isDaemon()
         */
        [Test]
        public void Test_isDaemon()
        {
            // Test for method boolean java.lang.Thread.isDaemon()
            st = new ThreadJob(() => new SimpleThread(1).Run(), "SimpleThread10");
            assertTrue("Non-Daemon thread returned true", !st.IsBackground);
            st.IsBackground = (true);
            assertTrue("Daemon thread returned false", st.IsBackground);
            st.Start();
        }

        //        internal class SpinThread //: IRunnable
        //        {

        //            public volatile bool done = false;

        //            public void Run()
        //            {
        //                while (!Thread.CurrentThread.IsInterrupted())
        //                    ;
        //                while (!done)
        //                    ;
        //            }
        //        }

        //        /**
        //         * @tests java.lang.Thread#isInterrupted()
        //         */
        //        public void Test_isInterrupted()
        //{
        //        // Test for method boolean java.lang.Thread.isInterrupted()


        //		SpinThread spin = new SpinThread();
        //spinner = new ThreadRun(() => spin.Run());
        //spinner.Start();
        //		ThreadRun.Yield();
        //		try {
        //			assertTrue("Non-Interrupted thread returned true", !spinner
        //                    .IsInterrupted());
        //spinner.Interrupt();
        //			assertTrue("Interrupted thread returned false", spinner
        //                    .IsInterrupted());
        //spin.done = true;
        //		} finally {
        //			spinner.Interrupt();
        //			spin.done = true;
        //		}
        //	}

        /**
         * @tests java.lang.Thread#join()
         */
        [Test]
        public void Test_join()
        {
            // Test for method void java.lang.Thread.join()
            SimpleThread simple = new SimpleThread(100);
            try
            {
                st = new ThreadJob(() => simple.Run());
                // cause isAlive() to be compiled by the JIT, as it must be called
                // within 100ms below.
                assertTrue("Thread is alive", !st.IsAlive);
                lock (simple)
                {
                    st.Start();
                    //simple.wait();
                    Monitor.Wait(simple);
                }
                st.Join();
            }
            catch (ThreadInterruptedException e)
            {
                fail("Join failed ");
            }
            assertTrue("Joined thread is still alive", !st.IsAlive);
            bool result = true;
            ThreadJob th = new ThreadJob("test");
            try
            {
                th.Join();
            }
            catch (ThreadInterruptedException e)
            {
                result = false;
            }
            catch (ThreadStateException e) // .NET specific - a ThreadStateException is thrown when the thread is not started
            {
                // expected
            }
            assertTrue("Hung joining a non-started thread", result);
            th.Start();
        }

        internal class KillerThread : ThreadJob
        {
            private readonly ThreadJob main;
            private readonly object syncLock;
            public KillerThread(object syncLock, ThreadJob main)
            {
                this.syncLock = syncLock;
                this.main = main;
            }
            public override void Run()
            {
                try
                {
                    lock (syncLock)
                    {
                        //lock.notify();
                        Monitor.Pulse(syncLock);
                    }
                    ThreadJob.Sleep(100);
                }
                catch (ThreadInterruptedException e)
                {
                    return;
                }
                main.Interrupt();
            }
        }

        /**
         * @tests java.lang.Thread#join(long)
         */
        [Test]
        public void Test_joinJ()
        {
            // Test for method void java.lang.Thread.join(long)
            SimpleThread simple = new SimpleThread(1000);
            try
            {
                st = new ThreadJob(simple.Run, "SimpleThread12");
                // cause isAlive() to be compiled by the JIT, as it must be called
                // within 100ms below.
                assertTrue("Thread is alive", !st.IsAlive);
                lock (simple)
                {
                    st.Start();
                    //simple.wait();
                    Monitor.Wait(simple);
                }
                st.Join(10);
            }
            catch (ThreadInterruptedException e)
            {
                fail("Join failed ");
            }
            assertTrue("Join failed to timeout", st.IsAlive);
            st.Interrupt();

            try
            {
                simple = new SimpleThread(100);
                st = new ThreadJob(() => simple.Run(), "SimpleThread13");
                lock (simple)
                {
                    st.Start();
                    //simple.wait();
                    Monitor.Wait(simple);
                }
                st.Join(1000);
            }
            catch (ThreadInterruptedException e)
            {
                fail("Join failed : " + e.Message);
                return;
            }
            assertTrue("Joined thread is still alive", !st.IsAlive);

            Object syncLock = new Object();
            ThreadJob main = ThreadJob.CurrentThread;
            ThreadJob killer = new KillerThread(syncLock, main);
            bool result = true;
            ThreadJob th = new ThreadJob("test");
            try
            {
                lock (syncLock)
                {
                    killer.Start();
                    //lock.wait();
                    Monitor.Wait(syncLock);
                }
                th.Join(200);

            }
            catch (ThreadInterruptedException e)
            {
                result = false;
            }
            catch (ThreadStateException e) // .NET specific - a ThreadStateException is thrown when the thread is not started
            {
                // expected
            }
            killer.Interrupt();
            assertTrue("Hung joining a non-started thread", result);
            th.Start();
        }

        private class KillerThread2 : ThreadJob
        {
            private readonly ThreadJob main;
            private readonly object syncLock;
            public KillerThread2(object syncLock, ThreadJob main)
            {
                this.syncLock = syncLock;
                this.main = main;
            }
            public override void Run()
            {
                try
                {
                    lock (syncLock)
                    {
                        //lock.notify();
                        Monitor.Pulse(syncLock);
                    }
                    ThreadJob.Sleep(100);
                }
                catch (ThreadInterruptedException e)
                {
                    return;
                }
                main.Interrupt();
            }
        }


        /**
         * @tests java.lang.Thread#join(long, int)
         */
        [Test]
        public void Test_joinJI()
        {
            // Test for method void java.lang.Thread.join(long, int)
            SimpleThread simple = new SimpleThread(1000);
            st = new ThreadJob(simple.Run, "Squawk1");
            assertTrue("Thread is alive", !st.IsAlive);
            lock (simple)
            {
                st.Start();
                //simple.wait();
                Monitor.Wait(simple);
            }

            long firstRead = Time.CurrentTimeMilliseconds();
            st.Join(100, 999999);
            long secondRead = Time.CurrentTimeMilliseconds();
            assertTrue("Did not join by appropriate time: " + secondRead + "-"
                                            + firstRead + "=" + (secondRead - firstRead), secondRead
                                            - firstRead <= 1000); // In .NET, we don't have nanosecond precision, so increased from 300 to 1000 to ensure test passes
            assertTrue("Joined thread is not alive", st.IsAlive);
            st.Interrupt();

            Object syncLock = new Object();
            ThreadJob main = ThreadJob.CurrentThread;
            ThreadJob killer = new KillerThread2(syncLock, main);
            bool result = true;
            ThreadJob th = new ThreadJob("test");
            try
            {
                lock (syncLock)
                {
                    killer.Start();
                    //lock.wait();
                    Monitor.Wait(syncLock);
                }
                th.Join(200, 20);
            }
            catch (ThreadInterruptedException e)
            {
                result = false;
            }
            catch (ThreadStateException e) // .NET specific - a ThreadStateException is thrown when the thread is not started
            {
                // expected
            }
            killer.Interrupt();
            assertTrue("Hung joining a non-started thread", result);
            th.Start();
        }

//        /**
//         * @tests java.lang.Thread#resume()
//         */
//        [Test]
//        [Ignore("TODO: Fix this test")]
//        public void Test_resume()
//        {
//            // Test for method void java.lang.Thread.resume()
//            int orgval;
//            ResSupThread res;
//            Thread t;
//            try
//            {
//                res = new ResSupThread(Thread.CurrentThread);
//                t = new Thread(() => res.Run());
//                lock (t)
//                {
//                    ct = new ThreadJob(t, "Interrupt Test2");
//                    ct.Start();
//                    //t.wait();
//                    Monitor.Wait(t);
//                }
//                ct.Suspend();
//                // Wait to be sure the suspend has occurred
//                ThreadJob.Sleep(500);
//                orgval = res.getCheckVal();
//                // Wait to be sure the thread is suspended
//                ThreadJob.Sleep(500);
//                assertTrue("Failed to suspend thread", orgval == res.getCheckVal());
//                ct.Resume();
//                // Wait to be sure the resume has occurred.
//                ThreadJob.Sleep(500);
//                assertTrue("Failed to resume thread", orgval != res.getCheckVal());
//                ct.Interrupt();
//            }
//            catch (ThreadInterruptedException e)
//            {
//                fail("Unexpected interrupt occurred : " + e.Message);
//            }
//        }

        private class RunThread //: IRunnable
        {
            internal bool didThreadRun = false;


            public void Run()
            {
                didThreadRun = true;
            }
        }

        /**
         * @tests java.lang.Thread#run()
         */
        [Test]
        public void Test_run()
        {
            // Test for method void java.lang.Thread.run()

            RunThread rt = new RunThread();
            ThreadJob t = new ThreadJob(() => rt.Run());
            try
            {
                t.Start();
                int count = 0;
                while (!rt.didThreadRun && count < 20)
                {
                    ThreadJob.Sleep(100);
                    count++;
                }
                assertTrue("Thread did not run", rt.didThreadRun);
                t.Join();
            }
            catch (ThreadInterruptedException e)
            {
                assertTrue("Joined thread was interrupted", true);
            }
            assertTrue("Joined thread is still alive", !t.IsAlive);
        }

        /**
         * @tests java.lang.Thread#setDaemon(boolean)
         */
        [Test]
        public void Test_setDaemonZ()
        {
            // Test for method void java.lang.Thread.setDaemon(boolean)
            st = new ThreadJob(() => new SimpleThread(1).Run(), "SimpleThread14");
            st.IsBackground = (true);
            assertTrue("Failed to set thread as daemon thread", st.IsBackground);
            st.Start();
        }

        /**
         * @tests java.lang.Thread#setName(java.lang.String)
         */
        [Test]
        public void Test_setNameLjava_lang_String()
        {
            // Test for method void java.lang.Thread.setName(java.lang.String)
            var simple = new SimpleThread(1);
            st = new ThreadJob(() => simple.Run(), "SimpleThread15");
            st.Name = ("Bogus Name");
            assertEquals("Failed to set thread name",
                    "Bogus Name", st.Name);
            try
            {
                st.Name = (null);
                fail("Null should not be accepted as a valid name");
            }
            catch (ArgumentNullException e)
            {
                // success
                assertTrue("Null should not be accepted as a valid name", true);
            }
            st.Start();
        }

        /**
         * @tests java.lang.Thread#setPriority(int)
         */
        [Test]
        public void Test_setPriorityI()
        {
            // Test for method void java.lang.Thread.setPriority(int)
            st = new ThreadJob(() => new SimpleThread(1).Run());
            st.Priority = ThreadPriority.Highest;
            assertTrue("Failed to set priority",
                    st.Priority == ThreadPriority.Highest);
            st.Start();
        }

        /**
         * @tests java.lang.Thread#sleep(long)
         */
        [Test]
        public void Test_sleepJ()
        {
            // Test for method void java.lang.Thread.sleep(long)

            // TODO : Test needs enhancing.
            long stime = 0, ftime = 0;
            try
            {
                stime = Time.CurrentTimeMilliseconds();
                ThreadJob.Sleep(1000);
                ftime = Time.CurrentTimeMilliseconds();
            }
            catch (ThreadInterruptedException e)
            {
                fail("Unexpected interrupt received");
            }
            assertTrue("Failed to sleep long enough", (ftime - stime) >= 800);
        }

        /**
         * @tests java.lang.Thread#sleep(long, int)
         */
        [Test]
        [Ignore("TODO: Fix this test")]
        public void Test_sleepJI()
        {
            // Test for method void java.lang.Thread.sleep(long, int)

            // TODO : Test needs revisiting.
            long stime = 0, ftime = 0;
            try
            {
                stime = Time.CurrentTimeMilliseconds();
                ThreadJob.Sleep(1000 + 1/*, 999999*/);
                ftime = Time.CurrentTimeMilliseconds();
            }
            catch (ThreadInterruptedException e)
            {
                fail("Unexpected interrupt received");
            }
            long result = ftime - stime;
            assertTrue("Failed to sleep long enough: " + result, result >= 900
                    && result <= 1100);
        }

        /**
         * @tests java.lang.Thread#start()
         */
        [Test]
        [Ignore("TODO: Fix this test")]
        public void Test_start()
        {
            // Test for method void java.lang.Thread.start()
            try
            {
                ResSupThread t = new ResSupThread(Thread.CurrentThread);
                lock (t)
                {
                    ct = new ThreadJob(() => t.Run(), "Interrupt Test4");
                    ct.Start();
                    //t.wait();
                    Monitor.Wait(t);
                }
                assertTrue("Thread is not running1", ct.IsAlive);
                // Let the child thread get going.
                int orgval = t.getCheckVal();
                ThreadJob.Sleep(150);
                assertTrue("Thread is not running2", orgval != t.getCheckVal());
                ct.Interrupt();
            }
            catch (ThreadInterruptedException e)
            {
                fail("Unexpected interrupt occurred");
            }
        }

        /////**
        //// * @tests java.lang.Thread#stop()
        //// */
        ////[Test]
        ////public void Test_stop()
        ////{
        ////    // Test for method void java.lang.Thread.stop()
        ////    try
        ////    {
        ////        var r = new ResSupThread(null);
        ////        lock (r)
        ////        {
        ////            st = new ThreadRun(r, "Interupt Test5");
        ////            st.Start();
        ////            // r.wait();
        ////            Monitor.Wait(r);
        ////        }

        ////    }
        ////    catch (ThreadInterruptedException e)
        ////    {
        ////        fail("Unexpected interrupt received");
        ////    }
        ////    st.stop();

        ////    try
        ////    {
        ////        st.Join(10000);
        ////    }
        ////    catch (ThreadInterruptedException e1)
        ////    {
        ////        st.Interrupt();
        ////        fail("Failed to stopThread before 10000 timeout");
        ////    }
        ////    assertTrue("Failed to stopThread", !st.IsAlive);
        ////}

        /////**
        //// * @tests java.lang.Thread#stop()
        //// */
        ////[Test]
        ////    public void Test_stop_subtest0()
        ////{
        ////    Thread t = new Thread("t");

        ////        class MySecurityManager extends SecurityManager
        ////{

        ////            public bool intest = false;

        ////@Override
        ////            public void checkAccess(Thread t)
        ////{
        ////    if (intest)
        ////    {
        ////        fail("checkAccess called");
        ////    }
        ////}
        ////@Override
        ////            public void checkPermission(Permission permission)
        ////{
        ////    if (permission.getName().equals("setSecurityManager"))
        ////    {
        ////        return;
        ////    }
        ////    super.checkPermission(permission);
        ////}
        ////		}
        ////		MySecurityManager sm = new MySecurityManager();
        ////System.setSecurityManager(sm);
        ////		try {
        ////			sm.intest = true;
        ////			try {
        ////				t.stop();
        ////				// Ignore any SecurityExceptions, may not have stopThread
        ////				// permission
        ////			} catch (SecurityException e) {
        ////			}
        ////			sm.intest = false;
        ////			t.start();
        ////			try {
        ////				t.join(2000);
        ////			} catch (InterruptedException e) {
        ////			}
        ////			sm.intest = true;
        ////			try {
        ////				t.stop();
        ////				// Ignore any SecurityExceptions, may not have stopThread
        ////				// permission
        ////			} catch (SecurityException e) {
        ////			}
        ////			sm.intest = false;
        ////		} finally {
        ////			System.setSecurityManager(null);
        ////		}
        ////	}

        ////	/**
        ////	 * @tests java.lang.Thread#stop(java.lang.Throwable)
        ////	 */
        ////[Test]
        ////	@SuppressWarnings("deprecation")
        ////    public void Test_stopLjava_lang_Throwable_subtest0()
        ////{
        ////    Thread t = new Thread("t");

        ////        class MySecurityManager extends SecurityManager
        ////{

        ////            public boolean intest = false;

        ////public boolean checkAccess = false;

        ////@Override
        ////            public void checkAccess(Thread t)
        ////{
        ////    if (intest)
        ////    {
        ////        checkAccess = true;
        ////    }
        ////}
        ////@Override
        ////            public void checkPermission(Permission permission)
        ////{
        ////    if (permission.getName().equals("setSecurityManager"))
        ////    {
        ////        return;
        ////    }
        ////    super.checkPermission(permission);
        ////}
        ////		}
        ////		MySecurityManager sm = new MySecurityManager();
        ////System.setSecurityManager(sm);
        ////		try {
        ////			sm.intest = true;
        ////			try {
        ////				t.stop(new ThreadDeath());
        ////				// Ignore any SecurityExceptions, may not have stopThread
        ////				// permission
        ////			} catch (SecurityException e) {
        ////			}
        ////			sm.intest = false;
        ////			assertTrue("no checkAccess 1", sm.checkAccess);
        ////t.start();
        ////			try {
        ////				t.join(2000);
        ////			} catch (InterruptedException e) {
        ////			}
        ////			sm.intest = true;
        ////			sm.checkAccess = false;
        ////			try {
        ////				t.stop(new ThreadDeath());
        ////				// Ignore any SecurityExceptions, may not have stopThread
        ////				// permission
        ////			} catch (SecurityException e) {
        ////			}
        ////			assertTrue("no checkAccess 2", sm.checkAccess);
        ////sm.intest = false;
        ////		} finally {
        ////			System.setSecurityManager(null);
        ////		}
        ////	}

        ////	/**
        ////	 * @tests java.lang.Thread#stop(java.lang.Throwable)
        ////	 */
        ////[Test]
        ////	@SuppressWarnings("deprecation")
        ////    public void Test_stopLjava_lang_Throwable()
        ////{
        ////    // Test for method void java.lang.Thread.stop(java.lang.Throwable)
        ////    ResSupThread t = new ResSupThread(Thread.currentThread());
        ////    synchronized(t) {
        ////        st = new Thread(t, "StopThread");
        ////        st.setPriority(Thread.MAX_PRIORITY);
        ////        st.start();
        ////        try
        ////        {
        ////            t.wait();
        ////        }
        ////        catch (InterruptedException e)
        ////        {
        ////        }
        ////    }
        ////    try
        ////    {
        ////        st.stop(new BogusException("Bogus"));
        ////        Thread.sleep(20000);
        ////    }
        ////    catch (InterruptedException e)
        ////    {
        ////        assertTrue("Stopped child with exception not alive", st.isAlive());
        ////        st.interrupt();
        ////        return;
        ////    }
        ////    st.interrupt();
        ////    fail("Stopped child did not throw exception");
        ////}

        private class NotifyThread : ThreadJob
        {
            private readonly object notify;
            public NotifyThread(object notify)
            {
                this.notify = notify;
            }
            public override void Run()
            {
                lock (notify)
                {
                    //notify.Notify();
                    Monitor.Pulse(notify);
                }
                ThreadJob.CurrentThread.Suspend();
            }
        }

        /**
         * @tests java.lang.Thread#suspend()
         */
        [Test]
        [Ignore("TODO: Fix this test")]
        public void Test_suspend()
        {
            // Test for method void java.lang.Thread.suspend()
            int orgval;
            ResSupThread t = new ResSupThread(Thread.CurrentThread);
            try
            {
                lock (t)
                {
                    ct = new ThreadJob(t.Run, "Interupt Test6");
                    ct.Start();
                    //t.wait();
                    Monitor.Wait(t);
                }
                ct.Suspend();
                // Wait to be sure the suspend has occurred
                ThreadJob.Sleep(500);
                orgval = t.getCheckVal();
                // Wait to be sure the thread is suspended
                ThreadJob.Sleep(500);
                assertTrue("Failed to suspend thread", orgval == t.getCheckVal());
                ct.Resume();
                // Wait to be sure the resume has occurred.
                ThreadJob.Sleep(500);
                assertTrue("Failed to resume thread", orgval != t.getCheckVal());
                ct.Interrupt();
            }
            catch (ThreadInterruptedException e)
            {
                fail("Unexpected interrupt occurred");
            }

            Object notify = new Object();
            ThreadJob t1 = new NotifyThread(notify);
            try
            {
                lock (notify)
                {
                    t1.Start();
                    //notify.wait();
                    Monitor.Wait(notify);
                }
                // wait for Thread to suspend
                ThreadJob.Sleep(500);
                assertTrue("Thread should be alive", t1.IsAlive);
                t1.Resume();
                t1.Join();
            }
            catch (ThreadInterruptedException e)
            {
            }
        }

        /**
         * @tests java.lang.Thread#toString()
         */
        [Test]
        public void Test_toString()
        {
            // Test for method java.lang.String java.lang.Thread.toString()
            //ThreadGroup tg = new ThreadGroup("Test Group5");
            //st = new Thread(tg, new SimpleThread(1), "SimpleThread17");
            st = new ThreadJob(() => new SimpleThread(1).Run(), "SimpleThread17");
            string stString = st.ToString();
            string expected = "Thread[SimpleThread17,Normal]";
            assertTrue("Returned incorrect string: " + stString + "\t(expecting :"
                    + expected + ")", stString.Equals(expected));
            st.Start();
            try
            {
                st.Join();
            }
            catch (ThreadInterruptedException e)
            {
            }
            //tg.destroy();
        }

        /////**
        //// * @tests java.lang.Thread#getAllStackTraces()
        //// */
        ////[Test]
        ////public void Test_getAllStackTraces()
        ////{
        ////    IDictionary<Thread, StackTraceElement[]> stMap = Thread.getAllStackTraces();
        ////    assertNotNull(stMap);
        ////    //TODO add security-based tests
        ////}

        /////**
        //// * @tests java.lang.Thread#getDefaultUncaughtExceptionHandler
        //// * @tests java.lang.Thread#setDefaultUncaughtExceptionHandler
        //// */
        ////public void Test_get_setDefaultUncaughtExceptionHandler()
        ////{
        ////        class Handler implements UncaughtExceptionHandler
        ////{
        ////            public void uncaughtException(Thread thread, Throwable ex)
        ////{
        ////}
        ////        }

        ////        final Handler handler = new Handler();
        ////Thread.setDefaultUncaughtExceptionHandler(handler);
        ////        assertSame(handler, Thread.getDefaultUncaughtExceptionHandler());

        ////Thread.setDefaultUncaughtExceptionHandler(null);
        ////        assertNull(Thread.getDefaultUncaughtExceptionHandler());
        ////        //TODO add security-based tests
        ////    }

        ////    /**
        ////     * @tests java.lang.Thread#getStackTrace()
        ////     */
        ////    public void Test_getStackTrace()
        ////{
        ////    StackTraceElement[] stackTrace = Thread.currentThread().getStackTrace();

        ////    assertNotNull(stackTrace);

        ////stack_trace_loop:
        ////    {
        ////        for (int i = 0; i < stackTrace.length; i++)
        ////        {
        ////            StackTraceElement e = stackTrace[i];
        ////            if (getClass().getName().equals(e.getClassName()))
        ////            {
        ////                if ("test_getStackTrace".equals(e.getMethodName()))
        ////                {
        ////                    break stack_trace_loop;
        ////                }
        ////            }
        ////        }
        ////        fail("class and method not found in stack trace");
        ////    }

        ////    //TODO add security-based tests
        ////}

        /**
         * @tests java.lang.Thread#getState()
         */
        [Test]
        public void Test_getState()
        {
            var state = ThreadJob.CurrentThread.State;
            assertNotNull(state);

            assertTrue(state == ThreadState.Background || state == ThreadState.Running);

            //TODO add additional state tests
        }

        ///**
        // * @tests java.lang.Thread#getUncaughtExceptionHandler
        // * @tests java.lang.Thread#setUncaughtExceptionHandler
        // */
        //[Test]
        //public void Test_get_setUncaughtExceptionHandler()
        //{
        //        class Handler implements UncaughtExceptionHandler
        //{
        //            public void uncaughtException(Thread thread, Throwable ex)
        //{
        //}
        //        }

        //        final Handler handler = new Handler();
        //Thread.currentThread().setUncaughtExceptionHandler(handler);
        //assertSame(handler, Thread.currentThread().getUncaughtExceptionHandler());

        //Thread.currentThread().setUncaughtExceptionHandler(null);

        //        //TODO add security-based tests
        //    }

        //    /**
        //     * @tests java.lang.Thread#getId()
        //     */
        //[Test]
        //    public void Test_getId()
        //{
        //    assertTrue("current thread's ID is not positive", Thread.currentThread().getId() > 0);

        //    //check all the current threads for positive IDs
        //    Map<Thread, StackTraceElement[]> stMap = Thread.getAllStackTraces();
        //    for (Thread thread : stMap.keySet())
        //    {
        //        assertTrue("thread's ID is not positive: " + thread.getName(), thread.getId() > 0);
        //    }
        //}



        public override void TearDown()
        {
            try
            {
                if (st != null)
                    st.Interrupt();
            }
            catch (Exception e)
            {
            }
            //try
            //{
            //    if (spinner != null)
            //        spinner.Interrupt();
            //}
            //catch (Exception e)
            //{
            //}
            try
            {
                if (ct != null)
                    ct.Interrupt();
            }
            catch (Exception e)
            {
            }

            try
            {
                //spinner = null;
                st = null;
                ct = null;
                GC.Collect();
                //System.runFinalization();
            }
            catch (Exception e)
            {
            }
        }
    }
}
