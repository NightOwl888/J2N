using NUnit.Framework;
using System;
using System.Threading;
#nullable enable

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

        /////**
        //// * repeated weakCompareAndSet succeeds in changing value when equal
        //// * to expected
        //// */
        ////[Test]
        ////public void TestWeakCompareAndSet()
        ////{
        ////    AtomicBoolean ai = new AtomicBoolean(true);
        ////    while (!ai.weakCompareAndSet(true, false)) ;
        ////    assertEquals(false, ai.get());
        ////    while (!ai.weakCompareAndSet(false, false)) ;
        ////    assertEquals(false, ai.get());
        ////    while (!ai.weakCompareAndSet(false, true)) ;
        ////    assertEquals(true, ai.get());
        ////}

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
            assertEquals(ai.ToString(), "false");
            ai.Value = (true);
            assertEquals(ai.ToString(), "true");
        }

        [Test]
        public void TestImplicitOperatorBool_Null()
        {
            AtomicBoolean? ab = null;
            Assert.Throws<ArgumentNullException>(() =>
            {
                bool x = ab;
                Assert.Fail("Should throw exception");
            });
        }

        [Test]
        public void TestImplicitOperatorBool_Value()
        {
            AtomicBoolean ab = new AtomicBoolean(true);
            bool x = ab;
            Assert.IsTrue(x);
        }

        [Test]
        public void TestImplicitOperatorNullableBool_Null()
        {
            AtomicBoolean? ab = null;
            bool? x = ab;
            Assert.False(x.HasValue);
        }

        [Test]
        public void TestImplicitOperatorNullableBool_Value()
        {
            AtomicBoolean ab = new AtomicBoolean(true);
            bool? x = ab;
            Assert.IsTrue(x);
        }

        [Test]
        public void TestOperatorEquals_AtomicBoolean_AtomicBoolean_BothNull()
        {
            AtomicBoolean? ab1 = null;
            AtomicBoolean? ab2 = null;
            Assert.IsTrue(ab1 == ab2);
        }

        [Test]
        public void TestOperatorEquals_AtomicBoolean_AtomicBoolean_OneNull()
        {
            AtomicBoolean? ab1 = null;
            AtomicBoolean ab2 = new AtomicBoolean(true);
            Assert.IsFalse(ab1 == ab2);
            Assert.IsFalse(ab2 == ab1);
        }

        [Test]
        public void TestOperatorEquals_AtomicBoolean_AtomicBoolean_BothNonNull()
        {
            AtomicBoolean ab1 = new AtomicBoolean(true);
            AtomicBoolean ab2 = new AtomicBoolean(true);
            AtomicBoolean ab3 = new AtomicBoolean(false);
            Assert.IsTrue(ab1 == ab2);
            Assert.IsFalse(ab1 == ab3);
        }

        [Test]
        public void TestOperatorNotEquals_AtomicBoolean_AtomicBoolean_BothNull()
        {
            AtomicBoolean? ab1 = null;
            AtomicBoolean? ab2 = null;
            Assert.IsFalse(ab1 != ab2);
        }

        [Test]
        public void TestOperatorNotEquals_AtomicBoolean_AtomicBoolean_OneNull()
        {
            AtomicBoolean? ab1 = null;
            AtomicBoolean ab2 = new AtomicBoolean(true);
            Assert.IsTrue(ab1 != ab2);
            Assert.IsTrue(ab2 != ab1);
        }

        [Test]
        public void TestOperatorNotEquals_AtomicBoolean_AtomicBoolean_BothNonNull()
        {
            AtomicBoolean ab1 = new AtomicBoolean(true);
            AtomicBoolean ab2 = new AtomicBoolean(true);
            AtomicBoolean ab3 = new AtomicBoolean(false);
            Assert.IsFalse(ab1 != ab2);
            Assert.IsTrue(ab1 != ab3);
        }

        [Test]
        public void TestOperatorEquals_AtomicBoolean_Bool_LeftNull()
        {
            AtomicBoolean? ab1 = null;
            const bool b2 = true;
            Assert.IsFalse(ab1 == b2);
        }

        [Test]
        public void TestOperatorEquals_AtomicBoolean_Bool_LeftValue()
        {
            AtomicBoolean ab1 = new AtomicBoolean(true);
            const bool b2 = true;
            const bool b3 = false;
            Assert.IsTrue(ab1 == b2);
            Assert.IsFalse(ab1 == b3);
        }

        [Test]
        public void TestOperatorNotEquals_AtomicBoolean_Bool_LeftNull()
        {
            AtomicBoolean? ab1 = null;
            const bool b2 = true;
            Assert.IsTrue(ab1 != b2);
        }

        [Test]
        public void TestOperatorNotEquals_AtomicBoolean_Bool_LeftValue()
        {
            AtomicBoolean ab1 = new AtomicBoolean(true);
            const bool b2 = true;
            const bool b3 = false;
            Assert.IsFalse(ab1 != b2);
            Assert.IsTrue(ab1 != b3);
        }

        [Test]
        public void TestOperatorEquals_Bool_AtomicBoolean_RightNull()
        {
            const bool b1 = true;
            AtomicBoolean? ab2 = null;
            Assert.IsFalse(b1 == ab2);
        }

        [Test]
        public void TestOperatorEquals_Bool_AtomicBoolean_RightValue()
        {
            const bool b1 = true;
            const bool b3 = false;
            AtomicBoolean ab2 = new AtomicBoolean(true);
            Assert.IsTrue(b1 == ab2);
            Assert.IsFalse(b3 == ab2);
        }

        [Test]
        public void TestOperatorNotEquals_Bool_AtomicBoolean_RightNull()
        {
            const bool b1 = true;
            AtomicBoolean? ab2 = null;
            Assert.IsTrue(b1 != ab2);
        }

        [Test]
        public void TestOperatorNotEquals_Bool_AtomicBoolean_RightValue()
        {
            const bool b1 = true;
            const bool b3 = false;
            AtomicBoolean ab2 = new AtomicBoolean(true);
            Assert.IsFalse(b1 != ab2);
            Assert.IsTrue(b3 != ab2);
        }

        [Test]
        public void TestOperatorEquals_AtomicBoolean_NullableBool_LeftNull()
        {
            AtomicBoolean? ab1 = null;
            bool? b2 = null;
            bool? b3 = true;
            Assert.IsTrue(ab1 == b2);
            Assert.IsFalse(ab1 == b3);
        }

        [Test]
        public void TestOperatorEquals_AtomicBoolean_NullableBool_LeftValue()
        {
            AtomicBoolean ab1 = new AtomicBoolean(true);
            bool? b2 = null;
            bool? b3 = true;
            bool? b4 = false;
            Assert.IsFalse(ab1 == b2);
            Assert.IsTrue(ab1 == b3);
            Assert.IsFalse(ab1 == b4);
        }

        [Test]
        public void TestOperatorNotEquals_AtomicBoolean_NullableBool_LeftNull()
        {
            AtomicBoolean? ab1 = null;
            bool? b2 = null;
            bool? b3 = true;
            Assert.IsFalse(ab1 != b2);
            Assert.IsTrue(ab1 != b3);
        }

        [Test]
        public void TestOperatorNotEquals_AtomicBoolean_NullableBool_LeftValue()
        {
            AtomicBoolean ab1 = new AtomicBoolean(true);
            bool? b2 = null;
            bool? b3 = true;
            bool? b4 = false;
            Assert.IsTrue(ab1 != b2);
            Assert.IsFalse(ab1 != b3);
            Assert.IsTrue(ab1 != b4);
        }

        [Test]
        public void TestOperatorEquals_NullableBool_AtomicBoolean_RightNull()
        {
            bool? b1 = null;
            AtomicBoolean? ab2 = null;
            AtomicBoolean ab3 = new AtomicBoolean(true);
            Assert.IsTrue(b1 == ab2);
            Assert.IsFalse(b1 == ab3);
        }

        [Test]
        public void TestOperatorEquals_NullableBool_AtomicBoolean_RightValue()
        {
            bool? b1 = true;
            bool? b2 = false;
            AtomicBoolean? ab3 = null;
            AtomicBoolean ab4 = new AtomicBoolean(true);
            Assert.IsFalse(b1 == ab3);
            Assert.IsTrue(b1 == ab4);
            Assert.IsFalse(b2 == ab4);
        }

        [Test]
        public void TestOperatorNotEquals_NullableBool_AtomicBoolean_RightNull()
        {
            bool? b1 = null;
            AtomicBoolean? ab2 = null;
            AtomicBoolean ab3 = new AtomicBoolean(true);
            Assert.IsFalse(b1 != ab2);
            Assert.IsTrue(b1 != ab3);
        }

        [Test]
        public void TestOperatorNotEquals_NullableBool_AtomicBoolean_RightValue()
        {
            bool? b1 = true;
            bool? b2 = false;
            AtomicBoolean? ab3 = null;
            AtomicBoolean ab4 = new AtomicBoolean(true);
            Assert.IsTrue(b1 != ab3);
            Assert.IsFalse(b1 != ab4);
            Assert.IsTrue(b2 != ab4);
        }
    }
}
