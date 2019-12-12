using NUnit.Framework;
using System;
using System.Threading;

namespace J2N.Threading.Atomic
{
    public class TestAtomicBoolean : TestCase
    {
        private const int LONG_DELAY_MS = 50 * 50;

        /**
         * fail with message "Unexpected exception"
         */
        public void unexpectedException()
        {
            fail("Unexpected exception");
        }

        /**
         * constructor initializes to given value
         */
        [Test]
        public void TestConstructor()
        {
            AtomicBoolean ai = new AtomicBoolean(true);
            assertEquals(true, ai.Value);
        }

        /**
         * default constructed initializes to false
         */
        [Test]
        public void TestConstructor2()
        {
            AtomicBoolean ai = new AtomicBoolean();
            assertEquals(false, ai.Value);
        }

        /**
         * get returns the last value set
         */
        [Test]
        public void TestGetSet()
        {
            AtomicBoolean ai = new AtomicBoolean(true);
            assertEquals(true, ai);
            ai.Value = (false);
            assertEquals(false, ai);
            ai.Value = (true);
            assertEquals(true, ai);

        }

        /**
         * compareAndSet succeeds in changing value if equal to expected else fails
         */
        [Test]
        public void TestCompareAndSet()
        {
            AtomicBoolean ai = new AtomicBoolean(true);
            assertTrue(ai.CompareAndSet(true, false));
            assertEquals(false, ai.Value);
            assertTrue(ai.CompareAndSet(false, false));
            assertEquals(false, ai.Value);
            assertFalse(ai.CompareAndSet(true, false));
            assertFalse((ai.Value));
            assertTrue(ai.CompareAndSet(false, true));
            assertEquals(true, ai.Value);
        }

#if FEATURE_THREADYIELD
        /**
         * compareAndSet in one thread enables another waiting for value
         * to succeed
         */
        [Test]
        public void TestCompareAndSetInMultipleThreads()
        {
            AtomicBoolean ai = new AtomicBoolean(true);
            Thread t = new Thread(() =>
            {
                while (!ai.CompareAndSet(false, true)) Thread.Yield();
            });
            try
            {
                t.Start();
                assertTrue(ai.CompareAndSet(true, false));
                t.Join(LONG_DELAY_MS);
                assertFalse(t.IsAlive);
            }
            catch (Exception e)
            {
                unexpectedException();
            }
        }
#endif

        ///**
        // * repeated weakCompareAndSet succeeds in changing value when equal
        // * to expected 
        // */
        //[Test]
        //public void TestWeakCompareAndSet()
        //{
        //    AtomicBoolean ai = new AtomicBoolean(true);
        //    while (!ai.weakCompareAndSet(true, false)) ;
        //    assertEquals(false, ai.get());
        //    while (!ai.weakCompareAndSet(false, false)) ;
        //    assertEquals(false, ai.get());
        //    while (!ai.weakCompareAndSet(false, true)) ;
        //    assertEquals(true, ai.get());
        //}

        /**
         * getAndSet returns previous value and sets to given value
         */
        [Test]
        public void TestGetAndSet()
        {
            AtomicBoolean ai = new AtomicBoolean(true);
            assertEquals(true, ai.GetAndSet(false));
            assertEquals(false, ai.GetAndSet(false));
            assertEquals(false, ai.GetAndSet(true));
            assertEquals(true, ai.Value);
        }

#if FEATURE_SERIALIZABLE
        /**
         * a deserialized serialized atomic holds same value
         */
        [Test]
        public void TestSerialization()
        {
            AtomicBoolean l = new AtomicBoolean();

            try
            {
                l.Value = (true);
                //ByteArrayOutputStream bout = new ByteArrayOutputStream(10000);
                //ObjectOutputStream out = new ObjectOutputStream(new BufferedOutputStream(bout));
                //    out.writeObject(l);
                //    out.close();

                //ByteArrayInputStream bin = new ByteArrayInputStream(bout.toByteArray());
                //ObjectInputStream in = new ObjectInputStream(new BufferedInputStream(bin));
                //AtomicBoolean r = (AtomicBoolean) in.readObject();

                AtomicBoolean r = Clone(l);

                assertEquals(l.Value, r.Value);
                assertNotSame(l, r);
            }
            catch (Exception e)
            {
                //e.PrintStackTrace();
                unexpectedException();
            }
        }
#endif

        /**
         * toString returns current value.
         */
        [Test]
        public void TestToString()
        {
            AtomicBoolean ai = new AtomicBoolean();
            assertEquals(ai.ToString(), (false).ToString());
            ai.Value = (true);
            assertEquals(ai.ToString(), (true).ToString());
        }

    }
}
