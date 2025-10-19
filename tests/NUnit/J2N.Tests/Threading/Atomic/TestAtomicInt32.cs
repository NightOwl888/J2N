using NUnit.Framework;
using System;
using System.Globalization;
using System.Threading;
#nullable enable

namespace J2N.Threading.Atomic
{
    public class TestAtomicInt32 : TestCase
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
            AtomicInt32 ai = new AtomicInt32(1);
            assertEquals(1, ai);
        }

        /**
         * default constructed initializes to zero
         */
        [Test]
        public void TestConstructor2()
        {
            AtomicInt32 ai = new AtomicInt32();
            assertEquals(0, ai.Value);
        }

        /**
         * get returns the last value set
         */
        [Test]
        public void TestGetSet()
        {
            AtomicInt32 ai = new AtomicInt32(1);
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
            AtomicInt32 ai = new AtomicInt32(1);
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
            AtomicInt32 ai = new AtomicInt32(1);
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
        //    AtomicInt32 ai = new AtomicInt32(1);
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
            AtomicInt32 ai = new AtomicInt32(1);
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
            AtomicInt32 ai = new AtomicInt32(1);
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
            AtomicInt32 ai = new AtomicInt32(1);
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
            AtomicInt32 ai = new AtomicInt32(1);
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
            AtomicInt32 ai = new AtomicInt32(1);
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
            AtomicInt32 ai = new AtomicInt32(1);
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
            AtomicInt32 ai = new AtomicInt32(1);
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
            AtomicInt32 l = new AtomicInt32();

            try
            {
                l.Value = 22;
                AtomicInt32 r = Clone(l);
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
            AtomicInt32 ai = new AtomicInt32();
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
            AtomicInt32 ai = new AtomicInt32();
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
            AtomicInt32 ai = new AtomicInt32();
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
            AtomicInt32 ai = new AtomicInt32();
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
            AtomicInt32 ai = new AtomicInt32();
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
            AtomicInt32 ai = new AtomicInt32(6);
            assertTrue(5 < ai);
            assertTrue(9 > ai);
            assertTrue(ai > 4);
            assertTrue(ai < 7);
            assertFalse(ai < 6);
            assertTrue(ai <= 6);
        }

        [Test]
        public void TestOperatorEquals_AtomicInt32_AtomicInt32_BothNull()
        {
            AtomicInt32? ai1 = null;
            AtomicInt32? ai2 = null;
            Assert.IsTrue(ai1 == ai2);
        }

        [Test]
        public void TestOperatorEquals_AtomicInt32_AtomicInt32_OneNull()
        {
            AtomicInt32? ai1 = null;
            AtomicInt32 ai2 = new AtomicInt32(42);
            Assert.IsFalse(ai1 == ai2);
            Assert.IsFalse(ai2 == ai1);
        }

        [Test]
        public void TestOperatorEquals_AtomicInt32_AtomicInt32_BothNonNull()
        {
            AtomicInt32 ai1 = new AtomicInt32(42);
            AtomicInt32 ai2 = new AtomicInt32(42);
            AtomicInt32 ai3 = new AtomicInt32(7);
            Assert.IsTrue(ai1 == ai2);
            Assert.IsFalse(ai1 == ai3);
        }

        [Test]
        public void TestOperatorNotEquals_AtomicInt32_AtomicInt32_BothNull()
        {
            AtomicInt32? ai1 = null;
            AtomicInt32? ai2 = null;
            Assert.IsFalse(ai1 != ai2);
        }

        [Test]
        public void TestOperatorNotEquals_AtomicInt32_AtomicInt32_OneNull()
        {
            AtomicInt32? ai1 = null;
            AtomicInt32 ai2 = new AtomicInt32(42);
            Assert.IsTrue(ai1 != ai2);
            Assert.IsTrue(ai2 != ai1);
        }

        [Test]
        public void TestOperatorNotEquals_AtomicInt32_AtomicInt32_BothNonNull()
        {
            AtomicInt32 ai1 = new AtomicInt32(42);
            AtomicInt32 ai2 = new AtomicInt32(42);
            AtomicInt32 ai3 = new AtomicInt32(7);
            Assert.IsFalse(ai1 != ai2);
            Assert.IsTrue(ai1 != ai3);
        }

        [Test]
        public void TestOperatorEquals_AtomicInt32_Int_LeftNull()
        {
            AtomicInt32? ai1 = null;
            const int i2 = 42;
            Assert.IsFalse(ai1 == i2);
        }

        [Test]
        public void TestOperatorEquals_AtomicInt32_Int_LeftValue()
        {
            AtomicInt32 ai1 = new AtomicInt32(42);
            const int i2 = 42;
            const int i3 = 7;
            Assert.IsTrue(ai1 == i2);
            Assert.IsFalse(ai1 == i3);
        }

        [Test]
        public void TestOperatorNotEquals_AtomicInt32_Int_LeftNull()
        {
            AtomicInt32? ai1 = null;
            const int i2 = 42;
            Assert.IsTrue(ai1 != i2);
        }

        [Test]
        public void TestOperatorNotEquals_AtomicInt32_Int_LeftValue()
        {
            AtomicInt32 ai1 = new AtomicInt32(42);
            const int i2 = 42;
            const int i3 = 7;
            Assert.IsFalse(ai1 != i2);
            Assert.IsTrue(ai1 != i3);
        }

        [Test]
        public void TestOperatorEquals_Int_AtomicInt32_RightNull()
        {
            const int i1 = 42;
            AtomicInt32? ai2 = null;
            Assert.IsFalse(i1 == ai2);
        }

        [Test]
        public void TestOperatorEquals_Int_AtomicInt32_RightValue()
        {
            const int i1 = 42;
            const int i3 = 7;
            AtomicInt32 ai2 = new AtomicInt32(42);
            Assert.IsTrue(i1 == ai2);
            Assert.IsFalse(i3 == ai2);
        }

        [Test]
        public void TestOperatorNotEquals_Int_AtomicInt32_RightNull()
        {
            const int i1 = 42;
            AtomicInt32? ai2 = null;
            Assert.IsTrue(i1 != ai2);
        }

        [Test]
        public void TestOperatorNotEquals_Int_AtomicInt32_RightValue()
        {
            const int i1 = 42;
            const int i3 = 7;
            AtomicInt32 ai2 = new AtomicInt32(42);
            Assert.IsFalse(i1 != ai2);
            Assert.IsTrue(i3 != ai2);
        }

        [Test]
        public void TestOperatorEquals_AtomicInt32_NullableInt_LeftNull()
        {
            AtomicInt32? ai1 = null;
            int? i2 = null;
            int? i3 = 42;
            Assert.IsTrue(ai1 == i2);
            Assert.IsFalse(ai1 == i3);
        }

        [Test]
        public void TestOperatorEquals_AtomicInt32_NullableInt_LeftValue()
        {
            AtomicInt32 ai1 = new AtomicInt32(42);
            int? i2 = null;
            int? i3 = 42;
            int? i4 = 7;
            Assert.IsFalse(ai1 == i2);
            Assert.IsTrue(ai1 == i3);
            Assert.IsFalse(ai1 == i4);
        }

        [Test]
        public void TestOperatorNotEquals_AtomicInt32_NullableInt_LeftNull()
        {
            AtomicInt32? ai1 = null;
            int? i2 = null;
            int? i3 = 42;
            Assert.IsFalse(ai1 != i2);
            Assert.IsTrue(ai1 != i3);
        }

        [Test]
        public void TestOperatorNotEquals_AtomicInt32_NullableInt_LeftValue()
        {
            AtomicInt32 ai1 = new AtomicInt32(42);
            int? i2 = null;
            int? i3 = 42;
            int? i4 = 7;
            Assert.IsTrue(ai1 != i2);
            Assert.IsFalse(ai1 != i3);
            Assert.IsTrue(ai1 != i4);
        }

        [Test]
        public void TestOperatorEquals_NullableInt_AtomicInt32_RightNull()
        {
            int? i1 = null;
            AtomicInt32? ai2 = null;
            AtomicInt32 ai3 = new AtomicInt32(42);
            Assert.IsTrue(i1 == ai2);
            Assert.IsFalse(i1 == ai3);
        }

        [Test]
        public void TestOperatorEquals_NullableInt_AtomicInt32_RightValue()
        {
            int? i1 = 42;
            int? i2 = 7;
            AtomicInt32? ai3 = null;
            AtomicInt32 ai4 = new AtomicInt32(42);
            Assert.IsFalse(i1 == ai3);
            Assert.IsTrue(i1 == ai4);
            Assert.IsFalse(i2 == ai4);
        }

        [Test]
        public void TestOperatorNotEquals_NullableInt_AtomicInt32_RightNull()
        {
            int? i1 = null;
            AtomicInt32? ai2 = null;
            AtomicInt32 ai3 = new AtomicInt32(42);
            Assert.IsFalse(i1 != ai2);
            Assert.IsTrue(i1 != ai3);
        }

        [Test]
        public void TestOperatorNotEquals_NullableInt_AtomicInt32_RightValue()
        {
            int? i1 = 42;
            int? i2 = 7;
            AtomicInt32? ai3 = null;
            AtomicInt32 ai4 = new AtomicInt32(42);
            Assert.IsTrue(i1 != ai3);
            Assert.IsFalse(i1 != ai4);
            Assert.IsTrue(i2 != ai4);
        }
    }
}
