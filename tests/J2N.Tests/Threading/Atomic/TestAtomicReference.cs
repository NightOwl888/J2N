using J2N.Util;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace J2N.Threading.Atomic
{
    public class TestAtomicReference : TestCase
    {
        private const int LONG_DELAY_MS = 50 * 50;

        static readonly Integer zero = new Integer(0);
        static readonly Integer one = new Integer(1);
        static readonly Integer two = new Integer(2);
        static readonly Integer three = new Integer(3);
        static readonly Integer seven = new Integer(7);
        static readonly Integer m3 = new Integer(-3);
        static readonly Integer m4 = new Integer(-4);
        static readonly Integer m5 = new Integer(-5);
        static readonly Integer m10 = new Integer(-10);

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
            AtomicReference<Integer> ai = new AtomicReference<Integer>(one);
            assertEquals(one, (Integer)ai);
        }

        /**
         * default constructed initializes to null
         */
        [Test]
        public void TestConstructor2()
        {
            AtomicReference<Integer> ai = new AtomicReference<Integer>();
            assertNull(ai.Value);
        }

        /**
         * get returns the last value set
         */
        [Test]
        public void TestGetSet()
        {
            AtomicReference<Integer> ai = new AtomicReference<Integer>(one);
            assertEquals(one, ai.Value);
            ai.Value = (two);
            assertEquals(two, ai.Value);
            ai.Value = (m3);
            assertEquals(m3, ai.Value);

        }
        /**
         * compareAndSet succeeds in changing value if equal to expected else fails
         */
        [Test]
        public void TestCompareAndSet()
        {
            AtomicReference<Integer> ai = new AtomicReference<Integer>(one);
            assertTrue(ai.CompareAndSet(one, two));
            assertTrue(ai.CompareAndSet(two, m4));
            assertEquals(m4, ai.Value);
            assertFalse(ai.CompareAndSet(m5, seven));
            assertFalse((seven.Equals(ai.Value)));
            assertTrue(ai.CompareAndSet(m4, seven));
            assertEquals(seven, ai.Value);
        }

        /**
         * compareAndSet in one thread enables another waiting for value
         * to succeed
         */
        [Test]
        public void TestCompareAndSetInMultipleThreads()
        {
            AtomicReference<Integer> ai = new AtomicReference<Integer>(one);
            Thread t = new Thread(() =>
            {
                while (!ai.CompareAndSet(two, three)) Thread.Yield();
            });
            try
            {
                t.Start();
                assertTrue(ai.CompareAndSet(one, two));
                t.Join(LONG_DELAY_MS);
                assertFalse(t.IsAlive);
                assertEquals((Integer)ai, three);
            }
            catch (Exception e)
            {
                unexpectedException();
            }
        }

        /////**
        //// * repeated weakCompareAndSet succeeds in changing value when equal
        //// * to expected 
        //// */
        ////[Test]
        ////public void TestWeakCompareAndSet()
        ////{
        ////    AtomicReference<Integer> ai = new AtomicReference<Integer>(one);
        ////    while (!ai.weakCompareAndSet(one, two)) ;
        ////    while (!ai.weakCompareAndSet(two, m4)) ;
        ////    assertEquals(m4, ai.get());
        ////    while (!ai.weakCompareAndSet(m4, seven)) ;
        ////    assertEquals(seven, ai.get());
        ////}

        /**
         * getAndSet returns previous value and sets to given value
         */
        [Test]
        public void TestGetAndSet()
        {
            AtomicReference<Integer> ai = new AtomicReference<Integer>(one);
            assertEquals(one, ai.GetAndSet(zero));
            assertEquals(zero, ai.GetAndSet(m10));
            assertEquals(m10, ai.GetAndSet(one));
        }

#if FEATURE_SERIALIZABLE
        /**
         * a deserialized serialized atomic holds same value
         */
        [Test]
        public void TestSerialization()
        {
            AtomicReference<Integer> l = new AtomicReference<Integer>();

            try
            {
                l.Value = (one);
                //ByteArrayOutputStream bout = new ByteArrayOutputStream(10000);
                //ObjectOutputStream out = new ObjectOutputStream(new BufferedOutputStream(bout));
                //    out.writeObject(l);
                //    out.close();

                //ByteArrayInputStream bin = new ByteArrayInputStream(bout.toByteArray());
                //ObjectInputStream in = new ObjectInputStream(new BufferedInputStream(bin));
                //AtomicReference r = (AtomicReference) in.readObject();

                AtomicReference<Integer> r = Clone(l);

                assertEquals(l.Value, r.Value);
                assertNotSame(l, r);
            }
            catch (Exception e)
            {
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
            AtomicReference<Integer> ai = new AtomicReference<Integer>(one);
            assertEquals(ai.ToString(), one.ToString());
            ai.Value = (two);
            assertEquals(ai.ToString(), two.ToString());
        }
    }
}
