using NUnit.Framework;
using System;
using System.Globalization;
using System.Threading;
#nullable enable

namespace J2N.Threading.Atomic
{
    public class TestAtomicInt64 : TestCase
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
            AtomicInt64 ai = new AtomicInt64(1);
            assertEquals(1, ai);
        }

        /**
         * default constructed initializes to zero
         */
        [Test]
        public void TestConstructor2()
        {
            AtomicInt64 ai = new AtomicInt64();
            assertEquals(0, ai.Value);
        }

        /**
         * get returns the last value set
         */
        [Test]
        public void TestGetSet()
        {
            AtomicInt64 ai = new AtomicInt64(1);
            assertEquals(1, ai);
            ai.Value = 2;
            assertEquals(2, ai);
            ai.Value = -3;
            assertEquals(-3, ai);

        }

        /**
         * compareAndSet succeeds in changing value if equal to expected else fails
         */
        [Test]
        public void TestCompareAndSet()
        {
            AtomicInt64 ai = new AtomicInt64(1);
            assertTrue(ai.CompareAndSet(1, 2));
            assertTrue(ai.CompareAndSet(2, -4));
            assertEquals(-4, ai.Value);
            assertFalse(ai.CompareAndSet(-5, 7));
            assertFalse((7 == ai.Value));
            assertTrue(ai.CompareAndSet(-4, 7));
            assertEquals(7, ai.Value);
        }

        /**
         * compareAndSet in one thread enables another waiting for value
         * to succeed
         */
        [Test]
        public void TestCompareAndSetInMultipleThreads()
        {
            AtomicInt64 ai = new AtomicInt64(1);
            Thread t = new Thread(() =>
            {
                while (!ai.CompareAndSet(2, 3)) Thread.Yield();
            });
            try
            {
                t.Start();
                assertTrue(ai.CompareAndSet(1, 2));
                t.Join(LONG_DELAY_MS);
                assertFalse(t.IsAlive);
                assertEquals(ai.Value, 3);
            }
            catch (Exception e)
            {
                unexpectedException();
            }
        }

        //    /**
        //     * repeated weakCompareAndSet succeeds in changing value when equal
        //     * to expected
        //     */
        //[Test]
        //    public void TestWeakCompareAndSet()
        //{
        //    AtomicInt64 ai = new AtomicInt64(1);
        //    while (!ai.WeakCompareAndSet(1, 2)) ;
        //    while (!ai.WeakCompareAndSet(2, -4)) ;
        //    assertEquals(-4, ai.Value);
        //    while (!ai.WeakCompareAndSet(-4, 7)) ;
        //    assertEquals(7, ai.Value);
        //}

        /**
         * getAndSet returns previous value and sets to given value
         */
        [Test]
        public void TestGetAndSet()
        {
            AtomicInt64 ai = new AtomicInt64(1);
            assertEquals(1, ai.GetAndSet(0));
            assertEquals(0, ai.GetAndSet(-10));
            assertEquals(-10, ai.GetAndSet(1));
        }

        /**
         * getAndAdd returns previous value and adds given value
         */
        [Test]
        public void TestGetAndAdd()
        {
            AtomicInt64 ai = new AtomicInt64(1);
            assertEquals(1, ai.GetAndAdd(2));
            assertEquals(3, ai.Value);
            assertEquals(3, ai.GetAndAdd(-4));
            assertEquals(-1, ai.Value);
        }

        /**
         * getAndDecrement returns previous value and decrements
         */
        [Test]
        public void TestGetAndDecrement()
        {
            AtomicInt64 ai = new AtomicInt64(1);
            assertEquals(1, ai.GetAndDecrement());
            assertEquals(0, ai.GetAndDecrement());
            assertEquals(-1, ai.GetAndDecrement());
        }

        /**
         * getAndIncrement returns previous value and increments
         */
        [Test]
        public void TestGetAndIncrement()
        {
            AtomicInt64 ai = new AtomicInt64(1);
            assertEquals(1, ai.GetAndIncrement());
            assertEquals(2, ai.Value);
            ai.Value = -2;
            assertEquals(-2, ai.GetAndIncrement());
            assertEquals(-1, ai.GetAndIncrement());
            assertEquals(0, ai.GetAndIncrement());
            assertEquals(1, ai.Value);
        }

        /**
         * addAndGet adds given value to current, and returns current value
         */
        [Test]
        public void TestAddAndGet()
        {
            AtomicInt64 ai = new AtomicInt64(1);
            assertEquals(3, ai.AddAndGet(2));
            assertEquals(3, ai.Value);
            assertEquals(-1, ai.AddAndGet(-4));
            assertEquals(-1, ai.Value);
        }

        /**
         * decrementAndGet decrements and returns current value
         */
        [Test]
        public void TestDecrementAndGet()
        {
            AtomicInt64 ai = new AtomicInt64(1);
            assertEquals(0, ai.DecrementAndGet());
            assertEquals(-1, ai.DecrementAndGet());
            assertEquals(-2, ai.DecrementAndGet());
            assertEquals(-2, ai.Value);
        }

        /**
         * incrementAndGet increments and returns current value
         */
        [Test]
        public void TestIncrementAndGet()
        {
            AtomicInt64 ai = new AtomicInt64(1);
            assertEquals(2, ai.IncrementAndGet());
            assertEquals(2, ai.Value);
            ai.Value = -2;
            assertEquals(-1, ai.IncrementAndGet());
            assertEquals(0, ai.IncrementAndGet());
            assertEquals(1, ai.IncrementAndGet());
            assertEquals(1, ai.Value);
        }

#if FEATURE_SERIALIZABLE
        /**
         * a deserialized serialized atomic holds same value
         */
        [Test]
        public void TestSerialization()
        {
            AtomicInt64 l = new AtomicInt64();

            try
            {
                l.Value = 22;
                AtomicInt64 r = Clone(l);
                assertEquals(l.Value, r.Value);
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
            AtomicInt64 ai = new AtomicInt64();
            Span<char> buffer = stackalloc char[64];

            for (int i = -12; i < 6; ++i)
            {
                string answer = i.ToString(CultureInfo.CurrentCulture);
                string answerInvariant = i.ToString(CultureInfo.InvariantCulture);
                ai.Value = i;
                assertEquals(answer, ai.ToString(CultureInfo.CurrentCulture));
                assertEquals(answerInvariant, ai.ToString(CultureInfo.InvariantCulture));

                assertTrue(ai.TryFormat(buffer, out int charsWritten, ReadOnlySpan<char>.Empty, CultureInfo.CurrentCulture));
                string actual = buffer.Slice(0, charsWritten).ToString();
                assertEquals($"Incorrect String representation want {answer}, got ({actual})", answer, actual);

                assertTrue(ai.TryFormat(buffer, out int charsWrittenInvariant, ReadOnlySpan<char>.Empty, CultureInfo.InvariantCulture));
                string actualInvariant = buffer.Slice(0, charsWrittenInvariant).ToString();
                assertEquals($"Incorrect String representation want {answerInvariant}, got ({actualInvariant})", answerInvariant, actualInvariant);
            }
        }

        /**
         * intValue returns current value.
         */
        [Test]
        public void TestIntValue()
        {
            AtomicInt64 ai = new AtomicInt64();
            for (int i = -12; i < 6; ++i)
            {
                ai.Value = i;
                assertEquals(i, ai);
            }
        }


        /**
         * longValue returns current value.
         */
        [Test]
        public void TestLongValue()
        {
            AtomicInt64 ai = new AtomicInt64();
            for (int i = -12; i < 6; ++i)
            {
                ai.Value = i;
                assertEquals((long)i, Convert.ToInt64(ai));
            }
        }

        /**
         * floatValue returns current value.
         */
        [Test]
        public void TestFloatValue()
        {
            AtomicInt64 ai = new AtomicInt64();
            for (int i = -12; i < 6; ++i)
            {
                ai.Value = i;
                assertEquals((float)i, Convert.ToSingle(ai));
            }
        }

        /**
         * doubleValue returns current value.
         */
        [Test]
        public void TestDoubleValue()
        {
            AtomicInt64 ai = new AtomicInt64();
            for (int i = -12; i < 6; ++i)
            {
                ai.Value = i;
                assertEquals((double)i, Convert.ToDouble(ai));
            }
        }

        /**
        * doubleValue returns current value.
        */
        [Test]
        public void TestComparisonOperators()
        {
            AtomicInt64 ai = new AtomicInt64(6);
            assertTrue(5 < ai);
            assertTrue(9 > ai);
            assertTrue(ai > 4);
            assertTrue(ai < 7);
            assertFalse(ai < 6);
            assertTrue(ai <= 6);
        }

        [Test]
        public void TestImplicitOperatorLong_Null()
        {
            AtomicInt64? ai = null;
            Assert.Throws<ArgumentNullException>(() =>
            {
                long x = ai;
                Assert.Fail("Should throw exception");
            });
        }

        [Test]
        public void TestImplicitOperatorLong_Value()
        {
            AtomicInt64 ai = new AtomicInt64(42L);
            long x = ai;
            Assert.AreEqual(42L, x);
        }

        [Test]
        public void TestImplicitOperatorNullableLong_Null()
        {
            AtomicInt64? ai = null;
            long? x = ai;
            Assert.False(x.HasValue);
        }

        [Test]
        public void TestImplicitOperatorNullableLong_Value()
        {
            AtomicInt64 ai = new AtomicInt64(42L);
            long? x = ai;
            Assert.AreEqual(42L, x);
        }

        [Test]
        public void TestOperatorEquals_AtomicInt64_AtomicInt64_BothNull()
        {
            AtomicInt64? ai1 = null;
            AtomicInt64? ai2 = null;
            Assert.IsTrue(ai1 == ai2);
        }

        [Test]
        public void TestOperatorEquals_AtomicInt64_AtomicInt64_OneNull()
        {
            AtomicInt64? ai1 = null;
            AtomicInt64 ai2 = new AtomicInt64(42L);
            Assert.IsFalse(ai1 == ai2);
            Assert.IsFalse(ai2 == ai1);
        }

        [Test]
        public void TestOperatorEquals_AtomicInt64_AtomicInt64_BothNonNull()
        {
            AtomicInt64 ai1 = new AtomicInt64(42L);
            AtomicInt64 ai2 = new AtomicInt64(42L);
            AtomicInt64 ai3 = new AtomicInt64(7L);
            Assert.IsTrue(ai1 == ai2);
            Assert.IsFalse(ai1 == ai3);
        }

        [Test]
        public void TestOperatorNotEquals_AtomicInt64_AtomicInt64_BothNull()
        {
            AtomicInt64? ai1 = null;
            AtomicInt64? ai2 = null;
            Assert.IsFalse(ai1 != ai2);
        }

        [Test]
        public void TestOperatorNotEquals_AtomicInt64_AtomicInt64_OneNull()
        {
            AtomicInt64? ai1 = null;
            AtomicInt64 ai2 = new AtomicInt64(42L);
            Assert.IsTrue(ai1 != ai2);
            Assert.IsTrue(ai2 != ai1);
        }

        [Test]
        public void TestOperatorNotEquals_AtomicInt64_AtomicInt64_BothNonNull()
        {
            AtomicInt64 ai1 = new AtomicInt64(42L);
            AtomicInt64 ai2 = new AtomicInt64(42L);
            AtomicInt64 ai3 = new AtomicInt64(7L);
            Assert.IsFalse(ai1 != ai2);
            Assert.IsTrue(ai1 != ai3);
        }

        [Test]
        public void TestOperatorEquals_AtomicInt64_Long_LeftNull()
        {
            AtomicInt64? ai1 = null;
            const long i2 = 42L;
            Assert.IsFalse(ai1 == i2);
        }

        [Test]
        public void TestOperatorEquals_AtomicInt64_Long_LeftValue()
        {
            AtomicInt64 ai1 = new AtomicInt64(42L);
            const long i2 = 42L;
            const long i3 = 7L;
            Assert.IsTrue(ai1 == i2);
            Assert.IsFalse(ai1 == i3);
        }

        [Test]
        public void TestOperatorNotEquals_AtomicInt64_Long_LeftNull()
        {
            AtomicInt64? ai1 = null;
            const long i2 = 42L;
            Assert.IsTrue(ai1 != i2);
        }

        [Test]
        public void TestOperatorNotEquals_AtomicInt64_Long_LeftValue()
        {
            AtomicInt64 ai1 = new AtomicInt64(42L);
            const long i2 = 42L;
            const long i3 = 7L;
            Assert.IsFalse(ai1 != i2);
            Assert.IsTrue(ai1 != i3);
        }

        [Test]
        public void TestOperatorEquals_Long_AtomicInt64_RightNull()
        {
            const long i1 = 42L;
            AtomicInt64? ai2 = null;
            Assert.IsFalse(i1 == ai2);
        }

        [Test]
        public void TestOperatorEquals_Long_AtomicInt64_RightValue()
        {
            const long i1 = 42L;
            const long i3 = 7L;
            AtomicInt64 ai2 = new AtomicInt64(42L);
            Assert.IsTrue(i1 == ai2);
            Assert.IsFalse(i3 == ai2);
        }

        [Test]
        public void TestOperatorNotEquals_Long_AtomicInt64_RightNull()
        {
            const long i1 = 42L;
            AtomicInt64? ai2 = null;
            Assert.IsTrue(i1 != ai2);
        }

        [Test]
        public void TestOperatorNotEquals_Long_AtomicInt64_RightValue()
        {
            const long i1 = 42L;
            const long i3 = 7L;
            AtomicInt64 ai2 = new AtomicInt64(42L);
            Assert.IsFalse(i1 != ai2);
            Assert.IsTrue(i3 != ai2);
        }

        [Test]
        public void TestOperatorEquals_AtomicInt64_NullableLong_LeftNull()
        {
            AtomicInt64? ai1 = null;
            long? i2 = null;
            long? i3 = 42L;
            Assert.IsTrue(ai1 == i2);
            Assert.IsFalse(ai1 == i3);
        }

        [Test]
        public void TestOperatorEquals_AtomicInt64_NullableLong_LeftValue()
        {
            AtomicInt64 ai1 = new AtomicInt64(42L);
            long? i2 = null;
            long? i3 = 42L;
            long? i4 = 7L;
            Assert.IsFalse(ai1 == i2);
            Assert.IsTrue(ai1 == i3);
            Assert.IsFalse(ai1 == i4);
        }

        [Test]
        public void TestOperatorNotEquals_AtomicInt64_NullableLong_LeftNull()
        {
            AtomicInt64? ai1 = null;
            long? i2 = null;
            long? i3 = 42L;
            Assert.IsFalse(ai1 != i2);
            Assert.IsTrue(ai1 != i3);
        }

        [Test]
        public void TestOperatorNotEquals_AtomicInt64_NullableLong_LeftValue()
        {
            AtomicInt64 ai1 = new AtomicInt64(42L);
            long? i2 = null;
            long? i3 = 42L;
            long? i4 = 7L;
            Assert.IsTrue(ai1 != i2);
            Assert.IsFalse(ai1 != i3);
            Assert.IsTrue(ai1 != i4);
        }

        [Test]
        public void TestOperatorEquals_NullableLong_AtomicInt64_RightNull()
        {
            long? i1 = null;
            AtomicInt64? ai2 = null;
            AtomicInt64 ai3 = new AtomicInt64(42L);
            Assert.IsTrue(i1 == ai2);
            Assert.IsFalse(i1 == ai3);
        }

        [Test]
        public void TestOperatorEquals_NullableLong_AtomicInt64_RightValue()
        {
            long? i1 = 42L;
            long? i2 = 7L;
            AtomicInt64? ai3 = null;
            AtomicInt64 ai4 = new AtomicInt64(42L);
            Assert.IsFalse(i1 == ai3);
            Assert.IsTrue(i1 == ai4);
            Assert.IsFalse(i2 == ai4);
        }

        [Test]
        public void TestOperatorNotEquals_NullableLong_AtomicInt64_RightNull()
        {
            long? i1 = null;
            AtomicInt64? ai2 = null;
            AtomicInt64 ai3 = new AtomicInt64(42L);
            Assert.IsFalse(i1 != ai2);
            Assert.IsTrue(i1 != ai3);
        }

        [Test]
        public void TestOperatorNotEquals_NullableLong_AtomicInt64_RightValue()
        {
            long? i1 = 42L;
            long? i2 = 7L;
            AtomicInt64? ai3 = null;
            AtomicInt64 ai4 = new AtomicInt64(42L);
            Assert.IsTrue(i1 != ai3);
            Assert.IsFalse(i1 != ai4);
            Assert.IsTrue(i2 != ai4);
        }
    }
}
