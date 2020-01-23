using J2N.Collections;
using J2N.Util;
using NUnit.Framework;
using System;
using System.Threading;

namespace J2N.Threading.Atomic
{
    public class TestAtomicReferenceArray : TestCase
    {
        private const int LONG_DELAY_MS = 50 * 50;

        /**
         * The number of elements to place in collections, arrays, etc.
         */
        const int SIZE = 20;

        static readonly Integer zero = new Integer(0);
        static readonly Integer one = new Integer(1);
        static readonly Integer two = new Integer(2);
        static readonly Integer three = new Integer(3);
        static readonly Integer four = new Integer(4);
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
         * constructor creates array of given size with all elements null
         */
        [Test]
        public void TestConstructor()
        {
            AtomicReferenceArray<Integer> ai = new AtomicReferenceArray<Integer>(SIZE);
            for (int i = 0; i < SIZE; ++i)
            {
                assertNull(ai[i]);
            }
        }

        /**
         * constructor with null array throws NPE
         */
        [Test]
        public void TestConstructor2NPE()
        {
            try
            {
                Integer[] a = null;
                AtomicReferenceArray<Integer> ai = new AtomicReferenceArray<Integer>(a);
            }
            catch (ArgumentNullException success)
            {
            }
            catch (Exception ex)
            {
                unexpectedException();
            }
        }

        /**
         * constructor with array is of same size and has all elements
         */
        [Test]
        public void TestConstructor2()
        {
            Integer[] a = { two, one, three, four, seven };
            AtomicReferenceArray<Integer> ai = new AtomicReferenceArray<Integer>(a);
            assertEquals(a.Length, ai.Length);
            for (int i = 0; i < a.Length; ++i)
                assertEquals(a[i], ai[i]);
        }


        /**
         * get and set for out of bound indices throw IndexOutOfBoundsException
         */
        [Test]
        public void TestIndexing()
        {
            AtomicReferenceArray<Integer> ai = new AtomicReferenceArray<Integer>(SIZE);
            try
            {
                var _ = ai[SIZE];
            }
            catch (IndexOutOfRangeException success)
            {
            }
            try
            {
                var _ = ai[-1];
            }
            catch (IndexOutOfRangeException success)
            {
            }
            try
            {
                ai[SIZE] = null;
            }
            catch (IndexOutOfRangeException success)
            {
            }
            try
            {
                ai[-1] = null;
            }
            catch (IndexOutOfRangeException success)
            {
            }
        }

        /**
         * get returns the last value set at index
         */
        [Test]
        public void TestGetSet()
        {
            AtomicReferenceArray<Integer> ai = new AtomicReferenceArray<Integer>(SIZE);
            for (int i = 0; i < SIZE; ++i)
            {
                ai[i] = one;
                assertEquals(one, ai[i]);
                ai[i] = two;
                assertEquals(two, ai[i]);
                ai[i] = m3;
                assertEquals(m3, ai[i]);
            }
        }

        /**
         * compareAndSet succeeds in changing value if equal to expected else fails
         */
        [Test]
        public void TestCompareAndSet()
        {
            AtomicReferenceArray<Integer> ai = new AtomicReferenceArray<Integer>(SIZE);
            for (int i = 0; i < SIZE; ++i)
            {
                ai[i] = one;
                assertTrue(ai.CompareAndSet(i, one, two));
                assertTrue(ai.CompareAndSet(i, two, m4));
                assertEquals(m4, ai[i]);
                assertFalse(ai.CompareAndSet(i, m5, seven));
                assertFalse((seven.Equals(ai[i])));
                assertTrue(ai.CompareAndSet(i, m4, seven));
                assertEquals(seven, ai[i]);
            }
        }

#if FEATURE_THREADYIELD
        /**
         * compareAndSet in one thread enables another waiting for value
         * to succeed
         */
        [Test]
        public void TestCompareAndSetInMultipleThreads()
        {
            AtomicReferenceArray<Integer> a = new AtomicReferenceArray<Integer>(1);
            a[0] = one;
            Thread t = new Thread(() =>
            {
                while (!a.CompareAndSet(0, two, three)) Thread.Yield();
            });
            try
            {
                t.Start();
                assertTrue(a.CompareAndSet(0, one, two));
                t.Join(LONG_DELAY_MS);
                assertFalse(t.IsAlive);
                assertEquals(a[0], three);
            }
            catch (Exception e)
            {
                unexpectedException();
            }
        }
#endif

        /////**
        //// * repeated weakCompareAndSet succeeds in changing value when equal
        //// * to expected 
        //// */
        ////[Test]
        ////public void TestWeakCompareAndSet()
        ////{
        ////    AtomicReferenceArray<Integer> ai = new AtomicReferenceArray<Integer>(SIZE);
        ////    for (int i = 0; i < SIZE; ++i)
        ////    {
        ////        ai[i] = one;
        ////        while (!ai.WeakCompareAndSet(i, one, two)) ;
        ////        while (!ai.WeakCompareAndSet(i, two, m4)) ;
        ////        assertEquals(m4, ai[i]);
        ////        while (!ai.weakCompareAndSet(i, m4, seven)) ;
        ////        assertEquals(seven, ai[i]);
        ////    }
        ////}

        /**
         * getAndSet returns previous value and sets to given value at given index
         */
        [Test]
        public void TestGetAndSet()
        {
            AtomicReferenceArray<Integer> ai = new AtomicReferenceArray<Integer>(SIZE);
            for (int i = 0; i < SIZE; ++i)
            {
                ai[i] = one;
                assertEquals(one, ai.GetAndSet(i, zero));
                assertEquals((Integer)0, ai.GetAndSet(i, m10));
                assertEquals(m10, ai.GetAndSet(i, one));
            }
        }

#if FEATURE_SERIALIZABLE
        /**
         * a deserialized serialized array holds same values
         */
        [Test]
        public void TestSerialization()
        {
            AtomicReferenceArray<Integer> l = new AtomicReferenceArray<Integer>(SIZE);
            for (int i = 0; i < SIZE; ++i)
            {
                l[i] = new Integer(-i);
            }

            try
            {
                AtomicReferenceArray<Integer> r = Clone(l);

                assertNotSame(l, r);
                assertEquals(l.Length, r.Length);
                for (int i = 0; i < SIZE; ++i)
                {
                    assertEquals(r[i], l[i]);
                }
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
            Integer[] a = { two, one, three, four, seven };
            AtomicReferenceArray<Integer> ai = new AtomicReferenceArray<Integer>(a);
            assertEquals(Arrays.ToString(a), ai.ToString());
        }
    }
}
