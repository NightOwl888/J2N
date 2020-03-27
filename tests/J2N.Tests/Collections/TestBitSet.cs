using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace J2N.Collections
{
    public class TestBitSet : TestCase
    {
        BitSet eightbs;

        /**
         * @tests java.util.BitSet#BitSet()
         */
        [Test]
        public void Test_Constructor()
        {
            BitSet bs = new BitSet();
            assertEquals("Create BitSet of incorrect size", 64, bs.Count);
            assertEquals("New BitSet had invalid string representation", "{}",
                         bs.ToString());
        }

        /**
         * @tests java.util.BitSet#BitSet(int)
         */
        [Test]
        public void Test_ConstructorI()
        {
            BitSet bs = new BitSet(128);
            assertEquals("Create BitSet of incorrect size", 128, bs.Count);
            assertEquals("New BitSet had invalid string representation: "
                    + bs.ToString(), "{}", bs.ToString());
            // All BitSets are created with elements of multiples of 64
            bs = new BitSet(89);
            assertEquals("Failed to round BitSet element size", 128, bs.Count);

            try
            {
                bs = new BitSet(-9);
                fail("Failed to throw exception when creating a new BitSet with negative element value");
            }
            catch (ArgumentOutOfRangeException e)
            {
                // Correct behaviour
            }
        }

        /**
         * tests java.util.BitSet#clone()
         */
        [Test]
        public void Test_clone()
        {
            BitSet bs;
            bs = (BitSet)eightbs.Clone();
            assertEquals("clone failed to return equal BitSet", bs, eightbs);
        }

        /**
         * @tests java.util.BitSet#equals(java.lang.Object)
         */
        [Test]
        public void Test_equalsLjava_lang_Object()
        {
            BitSet bs;
            bs = (BitSet)eightbs.Clone();
            assertEquals("Same BitSet returned false", eightbs, eightbs);
            assertEquals("Identical BitSet returned false", bs, eightbs);
            bs.Clear(6);
            assertFalse("Different BitSets returned true", eightbs.Equals(bs));

            bs = (BitSet)eightbs.Clone();
            bs.Set(128);
            assertFalse("Different sized BitSet with higher bit set returned true",
                    eightbs.Equals(bs));
            bs.Clear(128);
            assertTrue(
                    "Different sized BitSet with higher bits not set returned false",
                    eightbs.Equals(bs));
        }

        /**
         * @tests java.util.BitSet#hashCode()
         */
        [Test]
        public void Test_hashCode()
        {
            // Test for method int java.util.BitSet.GetHashCode()
            BitSet bs = (BitSet)eightbs.Clone();
            bs.Clear(2);
            bs.Clear(6);
            assertEquals("BitSet returns wrong hash value", 1129, bs.GetHashCode());
            bs.Set(10);
            bs.Clear(3);
            assertEquals("BitSet returns wrong hash value", 97, bs.GetHashCode());
        }

        /**
         * @tests java.util.BitSet#clear()
         */
        [Test]
        public void Test_clear()
        {
            eightbs.Clear();
            for (int i = 0; i < 8; i++)
            {
                assertFalse("Clear didn't clear bit " + i, eightbs.Get(i));
            }
            assertEquals("Test1: Wrong length", 0, eightbs.Length);

            BitSet bs = new BitSet(3400);
            bs.Set(0, bs.Count - 1); // ensure all bits are 1's
            bs.Set(bs.Count - 1);
            bs.Clear();
            assertEquals("Test2: Wrong length", 0, bs.Length);
            assertTrue("Test2: isEmpty() returned incorrect value", bs.IsEmpty);
            assertEquals("Test2: cardinality() returned incorrect value", 0, bs
                    .Cardinality);
        }

        /**
         * @tests java.util.BitSet#clear(int)
         */
        [Test]
        public void Test_clearI()
        {
            // Test for method void java.util.BitSet.Clear(int)

            eightbs.Clear(7);
            assertFalse("Failed to clear bit", eightbs.Get(7));

            // Check to see all other bits are still set
            for (int i = 0; i < 7; i++)
                assertTrue("Clear cleared incorrect bits", eightbs.Get(i));

            eightbs.Clear(165);
            assertFalse("Failed to clear bit", eightbs.Get(165));
            // Try out of range
            try
            {
                eightbs.Clear(-1);
                fail("Failed to throw out of bounds exception");
            }
            catch (ArgumentOutOfRangeException e)
            {
                // Correct behaviour
            }

            BitSet bs = new BitSet(0);
            assertEquals("Test1: Wrong length,", 0, bs.Length);
            assertEquals("Test1: Wrong size,", 0, bs.Count);

            bs.Clear(0);
            assertEquals("Test2: Wrong length,", 0, bs.Length);
            assertEquals("Test2: Wrong size,", 0, bs.Count);

            bs.Clear(60);
            assertEquals("Test3: Wrong length,", 0, bs.Length);
            assertEquals("Test3: Wrong size,", 0, bs.Count);

            bs.Clear(120);
            assertEquals("Test4: Wrong size,", 0, bs.Count);
            assertEquals("Test4: Wrong length,", 0, bs.Length);

            bs.Set(25);
            assertEquals("Test5: Wrong size,", 64, bs.Count);
            assertEquals("Test5: Wrong length,", 26, bs.Length);

            bs.Clear(80);
            assertEquals("Test6: Wrong size,", 64, bs.Count);
            assertEquals("Test6: Wrong length,", 26, bs.Length);

            bs.Clear(25);
            assertEquals("Test7: Wrong size,", 64, bs.Count);
            assertEquals("Test7: Wrong length,", 0, bs.Length);
        }

        /**
         * @tests java.util.BitSet#clear(int, int)
         */
        [Test]
        public void Test_clearII()
        {
            // Regression for HARMONY-98
            BitSet bitset = new BitSet();
            for (int i = 0; i < 20; i++)
            {
                bitset.Set(i);
            }
            bitset.Clear(10, 10);

            // Test for method void java.util.BitSet.Clear(int, int)
            // pos1 and pos2 are in the same bitset element
            BitSet bs = new BitSet(16);
            int initialSize = bs.Count;
            bs.Set(0, initialSize);
            bs.Clear(5);
            bs.Clear(15);
            bs.Clear(7, 11);
            for (int i = 0; i < 7; i++)
            {
                if (i == 5)
                    assertFalse("Shouldn't have flipped bit " + i, bs.Get(i));
                else
                    assertTrue("Shouldn't have cleared bit " + i, bs.Get(i));
            }
            for (int i = 7; i < 11; i++)
                assertFalse("Failed to clear bit " + i, bs.Get(i));

            for (int i = 11; i < initialSize; i++)
            {
                if (i == 15)
                    assertFalse("Shouldn't have flipped bit " + i, bs.Get(i));
                else
                    assertTrue("Shouldn't have cleared bit " + i, bs.Get(i));
            }

            for (int i = initialSize; i < bs.Count; i++)
            {
                assertFalse("Shouldn't have flipped bit " + i, bs.Get(i));
            }

            // pos1 and pos2 is in the same bitset element, boundry testing
            bs = new BitSet(16);
            initialSize = bs.Count;
            bs.Set(0, initialSize);
            bs.Clear(7, 64);
            assertEquals("Failed to grow BitSet", 64, bs.Count);
            for (int i = 0; i < 7; i++)
                assertTrue("Shouldn't have cleared bit " + i, bs.Get(i));
            for (int i = 7; i < 64; i++)
                assertFalse("Failed to clear bit " + i, bs.Get(i));
            for (int i = 64; i < bs.Count; i++)
            {
                assertTrue("Shouldn't have flipped bit " + i, !bs.Get(i));
            }
            // more boundary testing
            bs = new BitSet(32);
            initialSize = bs.Count;
            bs.Set(0, initialSize);
            bs.Clear(0, 64);
            for (int i = 0; i < 64; i++)
                assertFalse("Failed to clear bit " + i, bs.Get(i));
            for (int i = 64; i < bs.Count; i++)
            {
                assertFalse("Shouldn't have flipped bit " + i, bs.Get(i));
            }

            bs = new BitSet(32);
            initialSize = bs.Count;
            bs.Set(0, initialSize);
            bs.Clear(0, 65);
            for (int i = 0; i < 65; i++)
                assertFalse("Failed to clear bit " + i, bs.Get(i));
            for (int i = 65; i < bs.Count; i++)
            {
                assertFalse("Shouldn't have flipped bit " + i, bs.Get(i));
            }

            // pos1 and pos2 are in two sequential bitset elements
            bs = new BitSet(128);
            initialSize = bs.Count;
            bs.Set(0, initialSize);
            bs.Clear(7);
            bs.Clear(110);
            bs.Clear(9, 74);
            for (int i = 0; i < 9; i++)
            {
                if (i == 7)
                    assertFalse("Shouldn't have flipped bit " + i, bs.Get(i));
                else
                    assertTrue("Shouldn't have cleared bit " + i, bs.Get(i));
            }
            for (int i = 9; i < 74; i++)
                assertFalse("Failed to clear bit " + i, bs.Get(i));
            for (int i = 74; i < initialSize; i++)
            {
                if (i == 110)
                    assertFalse("Shouldn't have flipped bit " + i, bs.Get(i));
                else
                    assertTrue("Shouldn't have cleared bit " + i, bs.Get(i));
            }
            for (int i = initialSize; i < bs.Count; i++)
            {
                assertFalse("Shouldn't have flipped bit " + i, bs.Get(i));
            }

            // pos1 and pos2 are in two non-sequential bitset elements
            bs = new BitSet(256);
            bs.Set(0, 256);
            bs.Clear(7);
            bs.Clear(255);
            bs.Clear(9, 219);
            for (int i = 0; i < 9; i++)
            {
                if (i == 7)
                    assertFalse("Shouldn't have flipped bit " + i, bs.Get(i));
                else
                    assertTrue("Shouldn't have cleared bit " + i, bs.Get(i));
            }

            for (int i = 9; i < 219; i++)
                assertFalse("failed to clear bit " + i, bs.Get(i));

            for (int i = 219; i < 255; i++)
                assertTrue("Shouldn't have cleared bit " + i, bs.Get(i));

            for (int i = 255; i < bs.Count; i++)
                assertFalse("Shouldn't have flipped bit " + i, bs.Get(i));

            // test illegal args
            bs = new BitSet(10);
            try
            {
                bs.Clear(-1, 3);
                fail("Test1: Attempt to flip with  negative index failed to generate exception");
            }
            catch (ArgumentOutOfRangeException e)
            {
                // excepted
            }

            try
            {
                bs.Clear(2, -1);
                fail("Test2: Attempt to flip with negative index failed to generate exception");
            }
            catch (ArgumentOutOfRangeException e)
            {
                // excepted
            }

            bs.Set(2, 4);
            bs.Clear(2, 2);
            assertTrue("Bit got cleared incorrectly ", bs.Get(2));

            try
            {
                bs.Clear(4, 2);
                fail("Test4: Attempt to flip with illegal args failed to generate exception");
            }
            catch (ArgumentOutOfRangeException e)
            {
                // excepted
            }

            bs = new BitSet(0);
            assertEquals("Test1: Wrong length,", 0, bs.Length);
            assertEquals("Test1: Wrong size,", 0, bs.Count);

            bs.Clear(0, 2);
            assertEquals("Test2: Wrong length,", 0, bs.Length);
            assertEquals("Test2: Wrong size,", 0, bs.Count);

            bs.Clear(60, 64);
            assertEquals("Test3: Wrong length,", 0, bs.Length);
            assertEquals("Test3: Wrong size,", 0, bs.Count);

            bs.Clear(64, 120);
            assertEquals("Test4: Wrong length,", 0, bs.Length);
            assertEquals("Test4: Wrong size,", 0, bs.Count);

            bs.Set(25);
            assertEquals("Test5: Wrong length,", 26, bs.Length);
            assertEquals("Test5: Wrong size,", 64, bs.Count);

            bs.Clear(60, 64);
            assertEquals("Test6: Wrong length,", 26, bs.Length);
            assertEquals("Test6: Wrong size,", 64, bs.Count);

            bs.Clear(64, 120);
            assertEquals("Test7: Wrong size,", 64, bs.Count);
            assertEquals("Test7: Wrong length,", 26, bs.Length);

            bs.Clear(80);
            assertEquals("Test8: Wrong size,", 64, bs.Count);
            assertEquals("Test8: Wrong length,", 26, bs.Length);

            bs.Clear(25);
            assertEquals("Test9: Wrong size,", 64, bs.Count);
            assertEquals("Test9: Wrong length,", 0, bs.Length);
        }

        /**
         * @tests java.util.BitSet#get(int)
         */
        [Test]
        public void Test_getI()
        {
            // Test for method boolean java.util.BitSet.Get(int)

            BitSet bs = new BitSet();
            bs.Set(8);
            assertFalse("Get returned true for index out of range", eightbs.Get(99));
            assertTrue("Get returned false for set value", eightbs.Get(3));
            assertFalse("Get returned true for a non set value", bs.Get(0));

            try
            {
                bs.Get(-1);
                fail("Attempt to get at negative index failed to generate exception");
            }
            catch (ArgumentOutOfRangeException e)
            {
                // Correct behaviour
            }

            bs = new BitSet(1);
            assertFalse("Access greater than size", bs.Get(64));

            bs = new BitSet();
            bs.Set(63);
            assertTrue("Test highest bit", bs.Get(63));

            bs = new BitSet(0);
            assertEquals("Test1: Wrong length,", 0, bs.Length);
            assertEquals("Test1: Wrong size,", 0, bs.Count);

            bs.Get(2);
            assertEquals("Test2: Wrong length,", 0, bs.Length);
            assertEquals("Test2: Wrong size,", 0, bs.Count);

            bs.Get(70);
            assertEquals("Test3: Wrong length,", 0, bs.Length);
            assertEquals("Test3: Wrong size,", 0, bs.Count);

        }

        /**
         * @tests java.util.BitSet#get(int, int)
         */
        [Test]
        public void Test_getII()
        {
            BitSet bitset = new BitSet(30);
            bitset.Get(3, 3);

            // Test for method boolean java.util.BitSet.Get(int, int)
            BitSet bs, resultbs, correctbs;
            bs = new BitSet(512);
            bs.Set(3, 9);
            bs.Set(10, 20);
            bs.Set(60, 75);
            bs.Set(121);
            bs.Set(130, 140);

            // pos1 and pos2 are in the same bitset element, at index0
            resultbs = bs.Get(3, 6);
            correctbs = new BitSet(3);
            correctbs.Set(0, 3);
            assertEquals("Test1: Returned incorrect BitSet", correctbs, resultbs);

            // pos1 and pos2 are in the same bitset element, at index 1
            resultbs = bs.Get(100, 125);
            correctbs = new BitSet(25);
            correctbs.Set(21);
            assertEquals("Test2: Returned incorrect BitSet", correctbs, resultbs);

            // pos1 in bitset element at index 0, and pos2 in bitset element at
            // index 1
            resultbs = bs.Get(15, 125);
            correctbs = new BitSet(25);
            correctbs.Set(0, 5);
            correctbs.Set(45, 60);
            correctbs.Set(121 - 15);
            assertEquals("Test3: Returned incorrect BitSet", correctbs, resultbs);

            // pos1 in bitset element at index 1, and pos2 in bitset element at
            // index 2
            resultbs = bs.Get(70, 145);
            correctbs = new BitSet(75);
            correctbs.Set(0, 5);
            correctbs.Set(51);
            correctbs.Set(60, 70);
            assertEquals("Test4: Returned incorrect BitSet", correctbs, resultbs);

            // pos1 in bitset element at index 0, and pos2 in bitset element at
            // index 2
            resultbs = bs.Get(5, 145);
            correctbs = new BitSet(140);
            correctbs.Set(0, 4);
            correctbs.Set(5, 15);
            correctbs.Set(55, 70);
            correctbs.Set(116);
            correctbs.Set(125, 135);
            assertEquals("Test5: Returned incorrect BitSet", correctbs, resultbs);

            // pos1 in bitset element at index 0, and pos2 in bitset element at
            // index 3
            resultbs = bs.Get(5, 250);
            correctbs = new BitSet(200);
            correctbs.Set(0, 4);
            correctbs.Set(5, 15);
            correctbs.Set(55, 70);
            correctbs.Set(116);
            correctbs.Set(125, 135);
            assertEquals("Test6: Returned incorrect BitSet", correctbs, resultbs);

            assertEquals("equality principle 1 ", bs.Get(0, bs.Count), bs);

            // more tests
            BitSet bs2 = new BitSet(129);
            bs2.Set(0, 20);
            bs2.Set(62, 65);
            bs2.Set(121, 123);
            resultbs = bs2.Get(1, 124);
            correctbs = new BitSet(129);
            correctbs.Set(0, 19);
            correctbs.Set(61, 64);
            correctbs.Set(120, 122);
            assertEquals("Test7: Returned incorrect BitSet", correctbs, resultbs);

            // equality principle with some boundary conditions
            bs2 = new BitSet(128);
            bs2.Set(2, 20);
            bs2.Set(62);
            bs2.Set(121, 123);
            bs2.Set(127);
            resultbs = bs2.Get(0, bs2.Count);
            assertEquals("equality principle 2 ", resultbs, bs2);

            bs2 = new BitSet(128);
            bs2.Set(2, 20);
            bs2.Set(62);
            bs2.Set(121, 123);
            bs2.Set(127);
            bs2.Flip(0, 128);
            resultbs = bs2.Get(0, bs.Count);
            assertEquals("equality principle 3 ", resultbs, bs2);

            bs = new BitSet(0);
            assertEquals("Test1: Wrong length,", 0, bs.Length);
            assertEquals("Test1: Wrong size,", 0, bs.Count);

            bs.Get(0, 2);
            assertEquals("Test2: Wrong length,", 0, bs.Length);
            assertEquals("Test2: Wrong size,", 0, bs.Count);

            bs.Get(60, 64);
            assertEquals("Test3: Wrong length,", 0, bs.Length);
            assertEquals("Test3: Wrong size,", 0, bs.Count);

            bs.Get(64, 120);
            assertEquals("Test4: Wrong length,", 0, bs.Length);
            assertEquals("Test4: Wrong size,", 0, bs.Count);

            bs.Set(25);
            assertEquals("Test5: Wrong length,", 26, bs.Length);
            assertEquals("Test5: Wrong size,", 64, bs.Count);

            bs.Get(60, 64);
            assertEquals("Test6: Wrong length,", 26, bs.Length);
            assertEquals("Test6: Wrong size,", 64, bs.Count);

            bs.Get(64, 120);
            assertEquals("Test7: Wrong size,", 64, bs.Count);
            assertEquals("Test7: Wrong length,", 26, bs.Length);

            bs.Get(80);
            assertEquals("Test8: Wrong size,", 64, bs.Count);
            assertEquals("Test8: Wrong length,", 26, bs.Length);

            bs.Get(25);
            assertEquals("Test9: Wrong size,", 64, bs.Count);
            assertEquals("Test9: Wrong length,", 26, bs.Length);

        }

        /**
         * @tests java.util.BitSet#flip(int)
         */
        [Test]
        public void Test_flipI()
        {
            // Test for method void java.util.BitSet.Flip(int)
            BitSet bs = new BitSet();
            bs.Clear(8);
            bs.Clear(9);
            bs.Set(10);
            bs.Flip(9);
            assertFalse("Failed to flip bit", bs.Get(8));
            assertTrue("Failed to flip bit", bs.Get(9));
            assertTrue("Failed to flip bit", bs.Get(10));

            bs.Set(8);
            bs.Set(9);
            bs.Clear(10);
            bs.Flip(9);
            assertTrue("Failed to flip bit", bs.Get(8));
            assertFalse("Failed to flip bit", bs.Get(9));
            assertFalse("Failed to flip bit", bs.Get(10));

            try
            {
                bs.Flip(-1);
                fail("Attempt to flip at negative index failed to generate exception");
            }
            catch (ArgumentOutOfRangeException e)
            {
                // Correct behaviour
            }

            // Try setting a bit on a 64 boundary
            bs.Flip(128);
            assertEquals("Failed to grow BitSet", 192, bs.Count);
            assertTrue("Failed to flip bit", bs.Get(128));

            bs = new BitSet(64);
            for (int i = bs.Count; --i >= 0;)
            {
                bs.Flip(i);
                assertTrue("Test1: Incorrectly flipped bit" + i, bs.Get(i));
                assertEquals("Incorrect length", i + 1, bs.Length);
                for (int j = bs.Count; --j > i;)
                {
                    assertTrue("Test2: Incorrectly flipped bit" + j, !bs.Get(j));
                }
                for (int j = i; --j >= 0;)
                {
                    assertTrue("Test3: Incorrectly flipped bit" + j, !bs.Get(j));
                }
                bs.Flip(i);
            }

            BitSet bs0 = new BitSet(0);
            assertEquals("Test1: Wrong size", 0, bs0.Count);
            assertEquals("Test1: Wrong length", 0, bs0.Length);

            bs0.Flip(0);
            assertEquals("Test2: Wrong size", 64, bs0.Count);
            assertEquals("Test2: Wrong length", 1, bs0.Length);

            bs0.Flip(63);
            assertEquals("Test3: Wrong size", 64, bs0.Count);
            assertEquals("Test3: Wrong length", 64, bs0.Length);

            eightbs.Flip(7);
            assertTrue("Failed to flip bit 7", !eightbs.Get(7));

            // Check to see all other bits are still set
            for (int i = 0; i < 7; i++)
                assertTrue("Flip flipped incorrect bits", eightbs.Get(i));

            eightbs.Flip(127);
            assertTrue("Failed to flip bit 127", eightbs.Get(127));

            eightbs.Flip(127);
            assertTrue("Failed to flip bit 127", !eightbs.Get(127));
        }

        /**
         * @tests java.util.BitSet#clear(int, int)
         */
        [Test]
        public void Test_flipII()
        {
            BitSet bitset = new BitSet();
            for (int i = 0; i < 20; i++)
            {
                bitset.Set(i);
            }
            bitset.Flip(10, 10);

            // Test for method void java.util.BitSet.Flip(int, int)
            // pos1 and pos2 are in the same bitset element
            BitSet bs = new BitSet(16);
            bs.Set(7);
            bs.Set(10);
            bs.Flip(7, 11);
            for (int i = 0; i < 7; i++)
            {
                assertTrue("Shouldn't have flipped bit " + i, !bs.Get(i));
            }
            assertFalse("Failed to flip bit 7", bs.Get(7));
            assertTrue("Failed to flip bit 8", bs.Get(8));
            assertTrue("Failed to flip bit 9", bs.Get(9));
            assertFalse("Failed to flip bit 10", bs.Get(10));
            for (int i = 11; i < bs.Count; i++)
            {
                assertTrue("Shouldn't have flipped bit " + i, !bs.Get(i));
            }

            // pos1 and pos2 is in the same bitset element, boundry testing
            bs = new BitSet(16);
            bs.Set(7);
            bs.Set(10);
            bs.Flip(7, 64);
            assertEquals("Failed to grow BitSet", 64, bs.Count);
            for (int i = 0; i < 7; i++)
            {
                assertTrue("Shouldn't have flipped bit " + i, !bs.Get(i));
            }
            assertFalse("Failed to flip bit 7", bs.Get(7));
            assertTrue("Failed to flip bit 8", bs.Get(8));
            assertTrue("Failed to flip bit 9", bs.Get(9));
            assertFalse("Failed to flip bit 10", bs.Get(10));
            for (int i = 11; i < 64; i++)
            {
                assertTrue("failed to flip bit " + i, bs.Get(i));
            }
            assertFalse("Shouldn't have flipped bit 64", bs.Get(64));

            // more boundary testing
            bs = new BitSet(32);
            bs.Flip(0, 64);
            for (int i = 0; i < 64; i++)
            {
                assertTrue("Failed to flip bit " + i, bs.Get(i));
            }
            assertFalse("Shouldn't have flipped bit 64", bs.Get(64));

            bs = new BitSet(32);
            bs.Flip(0, 65);
            for (int i = 0; i < 65; i++)
            {
                assertTrue("Failed to flip bit " + i, bs.Get(i));
            }
            assertFalse("Shouldn't have flipped bit 65", bs.Get(65));

            // pos1 and pos2 are in two sequential bitset elements
            bs = new BitSet(128);
            bs.Set(7);
            bs.Set(10);
            bs.Set(72);
            bs.Set(110);
            bs.Flip(9, 74);
            for (int i = 0; i < 7; i++)
            {
                assertFalse("Shouldn't have flipped bit " + i, bs.Get(i));
            }
            assertTrue("Shouldn't have flipped bit 7", bs.Get(7));
            assertFalse("Shouldn't have flipped bit 8", bs.Get(8));
            assertTrue("Failed to flip bit 9", bs.Get(9));
            assertFalse("Failed to flip bit 10", bs.Get(10));
            for (int i = 11; i < 72; i++)
            {
                assertTrue("failed to flip bit " + i, bs.Get(i));
            }
            assertFalse("Failed to flip bit 72", bs.Get(72));
            assertTrue("Failed to flip bit 73", bs.Get(73));
            for (int i = 74; i < 110; i++)
            {
                assertFalse("Shouldn't have flipped bit " + i, bs.Get(i));
            }
            assertTrue("Shouldn't have flipped bit 110", bs.Get(110));
            for (int i = 111; i < bs.Count; i++)
            {
                assertFalse("Shouldn't have flipped bit " + i, bs.Get(i));
            }

            // pos1 and pos2 are in two non-sequential bitset elements
            bs = new BitSet(256);
            bs.Set(7);
            bs.Set(10);
            bs.Set(72);
            bs.Set(110);
            bs.Set(181);
            bs.Set(220);
            bs.Flip(9, 219);
            for (int i = 0; i < 7; i++)
            {
                assertFalse("Shouldn't have flipped bit " + i, bs.Get(i));
            }
            assertTrue("Shouldn't have flipped bit 7", bs.Get(7));
            assertFalse("Shouldn't have flipped bit 8", bs.Get(8));
            assertTrue("Failed to flip bit 9", bs.Get(9));
            assertFalse("Failed to flip bit 10", bs.Get(10));
            for (int i = 11; i < 72; i++)
            {
                assertTrue("failed to flip bit " + i, bs.Get(i));
            }
            assertFalse("Failed to flip bit 72", bs.Get(72));
            for (int i = 73; i < 110; i++)
            {
                assertTrue("failed to flip bit " + i, bs.Get(i));
            }
            assertFalse("Failed to flip bit 110", bs.Get(110));
            for (int i = 111; i < 181; i++)
            {
                assertTrue("failed to flip bit " + i, bs.Get(i));
            }
            assertFalse("Failed to flip bit 181", bs.Get(181));
            for (int i = 182; i < 219; i++)
            {
                assertTrue("failed to flip bit " + i, bs.Get(i));
            }
            assertFalse("Shouldn't have flipped bit 219", bs.Get(219));
            assertTrue("Shouldn't have flipped bit 220", bs.Get(220));
            for (int i = 221; i < bs.Count; i++)
            {
                assertTrue("Shouldn't have flipped bit " + i, !bs.Get(i));
            }

            // test illegal args
            bs = new BitSet(10);
            try
            {
                bs.Flip(-1, 3);
                fail("Test1: Attempt to flip with  negative index failed to generate exception");
            }
            catch (ArgumentOutOfRangeException e)
            {
                // correct behavior
            }

            try
            {
                bs.Flip(2, -1);
                fail("Test2: Attempt to flip with negative index failed to generate exception");
            }
            catch (ArgumentOutOfRangeException e)
            {
                // correct behavior
            }

            try
            {
                bs.Flip(4, 2);
                fail("Test4: Attempt to flip with illegal args failed to generate exception");
            }
            catch (ArgumentOutOfRangeException e)
            {
                // correct behavior
            }
        }

        /**
         * @tests java.util.BitSet#set(int)
         */
        [Test]
        public void Test_setI()
        {
            // Test for method void java.util.BitSet.Set(int)

            BitSet bs = new BitSet();
            bs.Set(8);
            assertTrue("Failed to set bit", bs.Get(8));

            try
            {
                bs.Set(-1);
                fail("Attempt to set at negative index failed to generate exception");
            }
            catch (ArgumentOutOfRangeException e)
            {
                // Correct behaviour
            }

            // Try setting a bit on a 64 boundary
            bs.Set(128);
            assertEquals("Failed to grow BitSet", 192, bs.Count);
            assertTrue("Failed to set bit", bs.Get(128));

            bs = new BitSet(64);
            for (int i = bs.Count; --i >= 0;)
            {
                bs.Set(i);
                assertTrue("Incorrectly set", bs.Get(i));
                assertEquals("Incorrect length", i + 1, bs.Length);
                for (int j = bs.Count; --j > i;)
                    assertFalse("Incorrectly set bit " + j, bs.Get(j));
                for (int j = i; --j >= 0;)
                    assertFalse("Incorrectly set bit " + j, bs.Get(j));
                bs.Clear(i);
            }

            bs = new BitSet(0);
            assertEquals("Test1: Wrong length", 0, bs.Length);
            bs.Set(0);
            assertEquals("Test2: Wrong length", 1, bs.Length);
        }

        /**
         * @tests java.util.BitSet#set(int, boolean)
         */
        [Test]
        public void Test_setIZ()
        {
            // Test for method void java.util.BitSet.Set(int, boolean)
            eightbs.Set(5, false);
            assertFalse("Should have set bit 5 to true", eightbs.Get(5));

            eightbs.Set(5, true);
            assertTrue("Should have set bit 5 to false", eightbs.Get(5));
        }

        /**
         * @tests java.util.BitSet#set(int, int)
         */
        [Test]
        public void Test_setII()
        {
            BitSet bitset = new BitSet(30);
            bitset.Set(29, 29);

            // Test for method void java.util.BitSet.Set(int, int)
            // pos1 and pos2 are in the same bitset element
            BitSet bs = new BitSet(16);
            bs.Set(5);
            bs.Set(15);
            bs.Set(7, 11);
            for (int i = 0; i < 7; i++)
            {
                if (i == 5)
                    assertTrue("Shouldn't have flipped bit " + i, bs.Get(i));
                else
                    assertFalse("Shouldn't have set bit " + i, bs.Get(i));
            }
            for (int i = 7; i < 11; i++)
                assertTrue("Failed to set bit " + i, bs.Get(i));
            for (int i = 11; i < bs.Count; i++)
            {
                if (i == 15)
                    assertTrue("Shouldn't have flipped bit " + i, bs.Get(i));
                else
                    assertFalse("Shouldn't have set bit " + i, bs.Get(i));
            }

            // pos1 and pos2 is in the same bitset element, boundry testing
            bs = new BitSet(16);
            bs.Set(7, 64);
            assertEquals("Failed to grow BitSet", 64, bs.Count);
            for (int i = 0; i < 7; i++)
            {
                assertFalse("Shouldn't have set bit " + i, bs.Get(i));
            }
            for (int i = 7; i < 64; i++)
            {
                assertTrue("Failed to set bit " + i, bs.Get(i));
            }
            assertFalse("Shouldn't have set bit 64", bs.Get(64));

            // more boundary testing
            bs = new BitSet(32);
            bs.Set(0, 64);
            for (int i = 0; i < 64; i++)
            {
                assertTrue("Failed to set bit " + i, bs.Get(i));
            }
            assertFalse("Shouldn't have set bit 64", bs.Get(64));

            bs = new BitSet(32);
            bs.Set(0, 65);
            for (int i = 0; i < 65; i++)
            {
                assertTrue("Failed to set bit " + i, bs.Get(i));
            }
            assertFalse("Shouldn't have set bit 65", bs.Get(65));

            // pos1 and pos2 are in two sequential bitset elements
            bs = new BitSet(128);
            bs.Set(7);
            bs.Set(110);
            bs.Set(9, 74);
            for (int i = 0; i < 9; i++)
            {
                if (i == 7)
                    assertTrue("Shouldn't have flipped bit " + i, bs.Get(i));
                else
                    assertFalse("Shouldn't have set bit " + i, bs.Get(i));
            }
            for (int i = 9; i < 74; i++)
            {
                assertTrue("Failed to set bit " + i, bs.Get(i));
            }
            for (int i = 74; i < bs.Count; i++)
            {
                if (i == 110)
                    assertTrue("Shouldn't have flipped bit " + i, bs.Get(i));
                else
                    assertFalse("Shouldn't have set bit " + i, bs.Get(i));
            }

            // pos1 and pos2 are in two non-sequential bitset elements
            bs = new BitSet(256);
            bs.Set(7);
            bs.Set(255);
            bs.Set(9, 219);
            for (int i = 0; i < 9; i++)
            {
                if (i == 7)
                    assertTrue("Shouldn't have set flipped " + i, bs.Get(i));
                else
                    assertFalse("Shouldn't have set bit " + i, bs.Get(i));
            }

            for (int i = 9; i < 219; i++)
            {
                assertTrue("failed to set bit " + i, bs.Get(i));
            }

            for (int i = 219; i < 255; i++)
            {
                assertFalse("Shouldn't have set bit " + i, bs.Get(i));
            }

            assertTrue("Shouldn't have flipped bit 255", bs.Get(255));

            // test illegal args
            bs = new BitSet(10);
            try
            {
                bs.Set(-1, 3);
                fail("Test1: Attempt to flip with  negative index failed to generate exception");
            }
            catch (ArgumentOutOfRangeException e)
            {
                // Correct behavior
            }

            try
            {
                bs.Set(2, -1);
                fail("Test2: Attempt to flip with negative index failed to generate exception");
            }
            catch (ArgumentOutOfRangeException e)
            {
                // Correct behavior
            }

            bs.Set(2, 2);
            assertFalse("Bit got set incorrectly ", bs.Get(2));

            try
            {
                bs.Set(4, 2);
                fail("Test4: Attempt to flip with illegal args failed to generate exception");
            }
            catch (ArgumentOutOfRangeException e)
            {
                // Correct behavior
            }
        }

        /**
         * @tests java.util.BitSet#set(int, int, boolean)
         */
        [Test]
        public void Test_setIIZ()
        {
            // Test for method void java.util.BitSet.Set(int, int, boolean)
            eightbs.Set(3, 6, false);
            assertTrue("Should have set bits 3, 4, and 5 to false", !eightbs.Get(3)
                    && !eightbs.Get(4) && !eightbs.Get(5));

            eightbs.Set(3, 6, true);
            assertTrue("Should have set bits 3, 4, and 5 to true", eightbs.Get(3)
                    && eightbs.Get(4) && eightbs.Get(5));

        }

        /**
         * @tests java.util.BitSet#intersects(java.util.BitSet)
         */
        [Test]
        public void Test_intersectsLjava_util_BitSet()
        {
            // Test for method boolean java.util.BitSet.Intersects(java.util.BitSet)
            BitSet bs = new BitSet(500);
            bs.Set(5);
            bs.Set(63);
            bs.Set(64);
            bs.Set(71, 110);
            bs.Set(127, 130);
            bs.Set(192);
            bs.Set(450);

            BitSet bs2 = new BitSet(8);
            assertFalse("Test1: intersects() returned incorrect value", bs
                    .Intersects(bs2));
            assertFalse("Test1: intersects() returned incorrect value", bs2
                    .Intersects(bs));

            bs2.Set(4);
            assertFalse("Test2: intersects() returned incorrect value", bs
                    .Intersects(bs2));
            assertFalse("Test2: intersects() returned incorrect value", bs2
                    .Intersects(bs));

            bs2.Clear();
            bs2.Set(5);
            assertTrue("Test3: intersects() returned incorrect value", bs
                    .Intersects(bs2));
            assertTrue("Test3: intersects() returned incorrect value", bs2
                    .Intersects(bs));

            bs2.Clear();
            bs2.Set(63);
            assertTrue("Test4: intersects() returned incorrect value", bs
                    .Intersects(bs2));
            assertTrue("Test4: intersects() returned incorrect value", bs2
                    .Intersects(bs));

            bs2.Clear();
            bs2.Set(80);
            assertTrue("Test5: intersects() returned incorrect value", bs
                    .Intersects(bs2));
            assertTrue("Test5: intersects() returned incorrect value", bs2
                    .Intersects(bs));

            bs2.Clear();
            bs2.Set(127);
            assertTrue("Test6: intersects() returned incorrect value", bs
                    .Intersects(bs2));
            assertTrue("Test6: intersects() returned incorrect value", bs2
                    .Intersects(bs));

            bs2.Clear();
            bs2.Set(192);
            assertTrue("Test7: intersects() returned incorrect value", bs
                    .Intersects(bs2));
            assertTrue("Test7: intersects() returned incorrect value", bs2
                    .Intersects(bs));

            bs2.Clear();
            bs2.Set(450);
            assertTrue("Test8: intersects() returned incorrect value", bs
                    .Intersects(bs2));
            assertTrue("Test8: intersects() returned incorrect value", bs2
                    .Intersects(bs));

            bs2.Clear();
            bs2.Set(500);
            assertFalse("Test9: intersects() returned incorrect value", bs
                    .Intersects(bs2));
            assertFalse("Test9: intersects() returned incorrect value", bs2
                    .Intersects(bs));

            try
            {
                bs.Intersects((BitSet)null);
                fail("Expected ArgumentNullException for null BitSet");
            }
            catch (ArgumentNullException e)
            {
                // correct behavior
            }
        }

        /**
         * @tests java.util.BitSet#and(java.util.BitSet)
         */
        [Test]
        public void Test_andLjava_util_BitSet()
        {
            // Test for method void java.util.BitSet.and(java.util.BitSet)
            BitSet bs = new BitSet(128);
            // Initialize the bottom half of the BitSet

            for (int i = 64; i < 128; i++)
                bs.Set(i);
            eightbs.And(bs);
            assertFalse("AND failed to clear bits", eightbs.Equals(bs));
            eightbs.Set(3);
            bs.Set(3);
            eightbs.And(bs);
            assertTrue("AND failed to maintain set bits", bs.Get(3));
            bs.And(eightbs);
            for (int i = 64; i < 128; i++)
            {
                assertFalse("Failed to clear extra bits in the receiver BitSet", bs
                        .Get(i));
            }

            try
            {
                bs.And((BitSet)null);
                fail("Expected ArgumentNullException for null BitSet");
            }
            catch (ArgumentNullException e)
            {
                // correct behavior
            }
        }

        /**
         * @tests java.util.BitSet#andNot(java.util.BitSet)
         */
        [Test]
        public void Test_andNotLjava_util_BitSet()
        {
            BitSet bs = (BitSet)eightbs.Clone();
            bs.Clear(5);
            BitSet bs2 = new BitSet();
            bs2.Set(2);
            bs2.Set(3);
            bs.AndNot(bs2);
            assertEquals("Incorrect bitset after andNot",
                         "{0, 1, 4, 6, 7}", bs.ToString());

            bs = new BitSet(0);
            bs.AndNot(bs2);
            assertEquals("Incorrect size", 0, bs.Count);

            try
            {
                bs.AndNot((BitSet)null);
                fail("Expected ArgumentNullException for null BitSet");
            }
            catch (ArgumentNullException e)
            {
                // correct behavior
            }
        }

        /**
         * @tests java.util.BitSet#or(java.util.BitSet)
         */
        [Test]
        public void Test_orLjava_util_BitSet()
        {
            // Test for method void java.util.BitSet.or(java.util.BitSet)
            BitSet bs = new BitSet(128);
            bs.Or(eightbs);
            for (int i = 0; i < 8; i++)
            {
                assertTrue("OR failed to set bits", bs.Get(i));
            }

            bs = new BitSet(0);
            bs.Or(eightbs);
            for (int i = 0; i < 8; i++)
            {
                assertTrue("OR(0) failed to set bits", bs.Get(i));
            }

            eightbs.Clear(5);
            bs = new BitSet(128);
            bs.Or(eightbs);
            assertFalse("OR set a bit which should be off", bs.Get(5));

            try
            {
                bs.Or((BitSet)null);
                fail("Expected ArgumentNullException for null BitSet");
            }
            catch (ArgumentNullException e)
            {
                // correct behavior
            }
        }

        /**
         * @tests java.util.BitSet#xor(java.util.BitSet)
         */
        [Test]
        public void Test_xorLjava_util_BitSet()
        {
            // Test for method void java.util.BitSet.xor(java.util.BitSet)

            BitSet bs = (BitSet)eightbs.Clone();
            bs.Xor(eightbs);
            for (int i = 0; i < 8; i++)
            {
                assertFalse("XOR failed to clear bits", bs.Get(i));
            }

            bs.Xor(eightbs);
            for (int i = 0; i < 8; i++)
            {
                assertTrue("XOR failed to set bits", bs.Get(i));
            }

            bs = new BitSet(0);
            bs.Xor(eightbs);
            for (int i = 0; i < 8; i++)
            {
                assertTrue("XOR(0) failed to set bits", bs.Get(i));
            }

            bs = new BitSet();
            bs.Set(63);
            assertEquals("Test highest bit", "{63}", bs.ToString());

            try
            {
                bs.Xor((BitSet)null);
                fail("Expected ArgumentNullException for null BitSet");
            }
            catch (ArgumentNullException e)
            {
                // correct behavior
            }
        }

        /**
         * @tests java.util.BitSet#size()
         */
        [Test]
        public void Test_size()
        {
            // Test for method int java.util.BitSet.Count
            assertEquals("Returned incorrect size", 64, eightbs.Count);
            eightbs.Set(129);
            assertTrue("Returned incorrect size", eightbs.Count >= 129);

        }

        /**
         * @tests java.util.BitSet#toString()
         */
        [Test]
        public void Test_toString()
        {
            // Test for method java.lang.String java.util.BitSet.ToString()
            assertEquals("Returned incorrect string representation",
                         "{0, 1, 2, 3, 4, 5, 6, 7}", eightbs.ToString());
            eightbs.Clear(2);
            assertEquals("Returned incorrect string representation",
                         "{0, 1, 3, 4, 5, 6, 7}", eightbs.ToString());
        }

        /**
         * @tests java.util.BitSet#length()
         */
        [Test]
        public void Test_length()
        {
            BitSet bs = new BitSet();
            assertEquals("BitSet returned wrong length", 0, bs.Length);
            bs.Set(5);
            assertEquals("BitSet returned wrong length", 6, bs.Length);
            bs.Set(10);
            assertEquals("BitSet returned wrong length", 11, bs.Length);
            bs.Set(432);
            assertEquals("BitSet returned wrong length", 433, bs.Length);
            bs.Set(300);
            assertEquals("BitSet returned wrong length", 433, bs.Length);
        }

        /**
         * @tests java.util.BitSet#nextSetBit(int)
         */
        [Test]
        public void Test_nextSetBitI()
        {
            // Test for method int java.util.BitSet.NextSetBit()
            BitSet bs = new BitSet(500);
            bs.Set(5);
            bs.Set(32);
            bs.Set(63);
            bs.Set(64);
            bs.Set(71, 110);
            bs.Set(127, 130);
            bs.Set(193);
            bs.Set(450);
            try
            {
                bs.NextSetBit(-1);
                fail("Expected IndexOutOfBoundsException for negative index");
            }
            catch (ArgumentOutOfRangeException e)
            {
                // correct behavior
            }
            assertEquals("nextSetBit() returned the wrong value", 5, bs
                    .NextSetBit(0));
            assertEquals("nextSetBit() returned the wrong value", 5, bs
                    .NextSetBit(5));
            assertEquals("nextSetBit() returned the wrong value", 32, bs
                    .NextSetBit(6));
            assertEquals("nextSetBit() returned the wrong value", 32, bs
                    .NextSetBit(32));
            assertEquals("nextSetBit() returned the wrong value", 63, bs
                    .NextSetBit(33));

            // boundary tests
            assertEquals("nextSetBit() returned the wrong value", 63, bs
                    .NextSetBit(63));
            assertEquals("nextSetBit() returned the wrong value", 64, bs
                    .NextSetBit(64));

            // at bitset element 1
            assertEquals("nextSetBit() returned the wrong value", 71, bs
                    .NextSetBit(65));
            assertEquals("nextSetBit() returned the wrong value", 71, bs
                    .NextSetBit(71));
            assertEquals("nextSetBit() returned the wrong value", 72, bs
                    .NextSetBit(72));
            assertEquals("nextSetBit() returned the wrong value", 127, bs
                    .NextSetBit(110));

            // boundary tests
            assertEquals("nextSetBit() returned the wrong value", 127, bs
                    .NextSetBit(127));
            assertEquals("nextSetBit() returned the wrong value", 128, bs
                    .NextSetBit(128));

            // at bitset element 2
            assertEquals("nextSetBit() returned the wrong value", 193, bs
                    .NextSetBit(130));

            assertEquals("nextSetBit() returned the wrong value", 193, bs
                    .NextSetBit(191));
            assertEquals("nextSetBit() returned the wrong value", 193, bs
                    .NextSetBit(192));
            assertEquals("nextSetBit() returned the wrong value", 193, bs
                    .NextSetBit(193));
            assertEquals("nextSetBit() returned the wrong value", 450, bs
                    .NextSetBit(194));
            assertEquals("nextSetBit() returned the wrong value", 450, bs
                    .NextSetBit(255));
            assertEquals("nextSetBit() returned the wrong value", 450, bs
                    .NextSetBit(256));
            assertEquals("nextSetBit() returned the wrong value", 450, bs
                    .NextSetBit(450));

            assertEquals("nextSetBit() returned the wrong value", -1, bs
                    .NextSetBit(451));
            assertEquals("nextSetBit() returned the wrong value", -1, bs
                    .NextSetBit(511));
            assertEquals("nextSetBit() returned the wrong value", -1, bs
                    .NextSetBit(512));
            assertEquals("nextSetBit() returned the wrong value", -1, bs
                    .NextSetBit(800));
        }

        /**
         * @tests java.util.BitSet#nextClearBit(int)
         */
        [Test]
        public void Test_nextClearBitI()
        {
            // Test for method int java.util.BitSet.NextSetBit()
            BitSet bs = new BitSet(500);
            bs.Set(0, bs.Count - 1); // ensure all the bits from 0 to bs.Count
                                     // -1
            bs.Set(bs.Count - 1); // are set to true
            bs.Clear(5);
            bs.Clear(32);
            bs.Clear(63);
            bs.Clear(64);
            bs.Clear(71, 110);
            bs.Clear(127, 130);
            bs.Clear(193);
            bs.Clear(450);
            try
            {
                bs.NextClearBit(-1);
                fail("Expected IndexOutOfBoundsException for negative index");
            }
            catch (ArgumentOutOfRangeException e)
            {
                // correct behavior
            }
            assertEquals("nextClearBit() returned the wrong value", 5, bs
                    .NextClearBit(0));
            assertEquals("nextClearBit() returned the wrong value", 5, bs
                    .NextClearBit(5));
            assertEquals("nextClearBit() returned the wrong value", 32, bs
                    .NextClearBit(6));
            assertEquals("nextClearBit() returned the wrong value", 32, bs
                    .NextClearBit(32));
            assertEquals("nextClearBit() returned the wrong value", 63, bs
                    .NextClearBit(33));

            // boundary tests
            assertEquals("nextClearBit() returned the wrong value", 63, bs
                    .NextClearBit(63));
            assertEquals("nextClearBit() returned the wrong value", 64, bs
                    .NextClearBit(64));

            // at bitset element 1
            assertEquals("nextClearBit() returned the wrong value", 71, bs
                    .NextClearBit(65));
            assertEquals("nextClearBit() returned the wrong value", 71, bs
                    .NextClearBit(71));
            assertEquals("nextClearBit() returned the wrong value", 72, bs
                    .NextClearBit(72));
            assertEquals("nextClearBit() returned the wrong value", 127, bs
                    .NextClearBit(110));

            // boundary tests
            assertEquals("nextClearBit() returned the wrong value", 127, bs
                    .NextClearBit(127));
            assertEquals("nextClearBit() returned the wrong value", 128, bs
                    .NextClearBit(128));

            // at bitset element 2
            assertEquals("nextClearBit() returned the wrong value", 193, bs
                    .NextClearBit(130));
            assertEquals("nextClearBit() returned the wrong value", 193, bs
                    .NextClearBit(191));

            assertEquals("nextClearBit() returned the wrong value", 193, bs
                    .NextClearBit(192));
            assertEquals("nextClearBit() returned the wrong value", 193, bs
                    .NextClearBit(193));
            assertEquals("nextClearBit() returned the wrong value", 450, bs
                    .NextClearBit(194));
            assertEquals("nextClearBit() returned the wrong value", 450, bs
                    .NextClearBit(255));
            assertEquals("nextClearBit() returned the wrong value", 450, bs
                    .NextClearBit(256));
            assertEquals("nextClearBit() returned the wrong value", 450, bs
                    .NextClearBit(450));

            // bitset has 1 still the end of bs.Count -1, but calling nextClearBit
            // with any index value
            // after the last true bit should return bs.Count,
            assertEquals("nextClearBit() returned the wrong value", 512, bs
                    .NextClearBit(451));
            assertEquals("nextClearBit() returned the wrong value", 512, bs
                    .NextClearBit(511));
            assertEquals("nextClearBit() returned the wrong value", 512, bs
                    .NextClearBit(512));

            // if the index is larger than bs.Count, nextClearBit should return
            // index;
            assertEquals("nextClearBit() returned the wrong value", 513, bs
                    .NextClearBit(513));
            assertEquals("nextClearBit() returned the wrong value", 800, bs
                    .NextClearBit(800));
        }

        /**
         * @tests java.util.BitSet#isEmpty()
         */
        [Test]
        public void Test_isEmpty()
        {
            BitSet bs = new BitSet(500);
            assertTrue("Test: isEmpty() returned wrong value", bs.IsEmpty);

            // at bitset element 0
            bs.Set(3);
            assertFalse("Test0: isEmpty() returned wrong value", bs.IsEmpty);

            // at bitset element 1
            bs.Clear();
            bs.Set(12);
            assertFalse("Test1: isEmpty() returned wrong value", bs.IsEmpty);

            // at bitset element 2
            bs.Clear();
            bs.Set(128);
            assertFalse("Test2: isEmpty() returned wrong value", bs.IsEmpty);

            // boundary testing
            bs.Clear();
            bs.Set(459);
            assertFalse("Test3: isEmpty() returned wrong value", bs.IsEmpty);

            bs.Clear();
            bs.Set(511);
            assertFalse("Test4: isEmpty() returned wrong value", bs.IsEmpty);
        }

        /**
         * @tests java.util.BitSet#cardinality()
         */
        [Test]
        public void Test_cardinality()
        {
            // test for method int java.util.BitSet.Cardinality
            BitSet bs = new BitSet(500);
            bs.Set(5);
            bs.Set(32);
            bs.Set(63);
            bs.Set(64);
            bs.Set(71, 110);
            bs.Set(127, 130);
            bs.Set(193);
            bs.Set(450);
            assertEquals("cardinality() returned wrong value", 48, bs.Cardinality);

            bs.Flip(0, 500);
            assertEquals("cardinality() returned wrong value", 452, bs
                    .Cardinality);

            bs.Clear();
            assertEquals("cardinality() returned wrong value", 0, bs.Cardinality);

            bs.Set(0, 500);
            assertEquals("cardinality() returned wrong value", 500, bs
                    .Cardinality);
        }

#if FEATURE_SERIALIZABLE
        [Test]
        public void Test_Serialization()
        {
            var target = new BitSet();
            target.Set(5);
            target.Set(7);
            target.Set(9);

            var clone = Clone(target);

            assertNotSame(target, clone);
            assertNotSame(target.bits, clone.bits);

            assertTrue(target.Get(5));
            assertFalse(target.Get(6));
            assertTrue(target.Get(7));
            assertFalse(target.Get(8));
            assertTrue(target.Get(9));

            assertEquals(target.Count, clone.Count);
            assertEquals(target.Length, clone.Length);
            assertEquals(target.Cardinality, clone.Cardinality);
        }
#endif

        public override void SetUp()
        {

            eightbs = new BitSet();

            for (int i = 0; i < 8; i++)
                eightbs.Set(i);
        }

        public override void TearDown()
        {
        }


        /// <summary>
        /// This is a simple test class created to run tests on the BitSet class.
        /// </summary>
        public class JDKBSMethods : TestCase
        {
            private static Random generator = new Random();
            //private static bool failure = false;

            //private static void fail(String diagnostic)
            //{
            //    new Error(diagnostic).printStackTrace();
            //    failure = true;
            //}

            private static void check(bool condition)
            {
                check(condition, "something's fishy");
            }

            private static void check(bool condition, String diagnostic)
            {
                if (!condition)
                    fail(diagnostic);
            }

            private static void checkEmpty(BitSet s)
            {
                check(s.IsEmpty, "isEmpty");
                check(s.Length == 0, "length");
                check(s.Cardinality == 0, "cardinality");
                check(s.Equals(new BitSet()), "equals");
                check(s.Equals(new BitSet(0)), "equals");
                check(s.Equals(new BitSet(127)), "equals");
                check(s.Equals(new BitSet(128)), "equals");
                check(s.NextSetBit(0) == -1, "nextSetBit");
                check(s.NextSetBit(127) == -1, "nextSetBit");
                check(s.NextSetBit(128) == -1, "nextSetBit");
                check(s.NextClearBit(0) == 0, "nextClearBit");
                check(s.NextClearBit(127) == 127, "nextClearBit");
                check(s.NextClearBit(128) == 128, "nextClearBit");
                check(s.ToString().Equals("{}"), "toString");
                check(!s.Get(0), "get");
            }

            private static BitSet makeSet(params int[] elts)
            {
                BitSet s = new BitSet();
                foreach (int elt in elts)
                    s.Set(elt);
                return s;
            }

            private static void checkEquality(BitSet s, BitSet t)
            {
                checkSanity(s, t);
                check(s.Equals(t), "equals");
                check(s.ToString().Equals(t.ToString()), "equal strings");
                check(s.Length == t.Length, "equal lengths");
                check(s.Cardinality == t.Cardinality, "equal cardinalities");
            }

            private static void checkSanity(params BitSet[] sets)
            {
                foreach (BitSet s in sets)
                {
                    int len = s.Length;
                    int cardinality1 = s.Cardinality;
                    int cardinality2 = 0;
                    for (int i = s.NextSetBit(0); i >= 0; i = s.NextSetBit(i + 1))
                    {
                        check(s.Get(i));
                        cardinality2++;
                    }
                    check(s.NextSetBit(len) == -1, "last set bit");
                    check(s.NextClearBit(len) == len, "last set bit");
                    check(s.IsEmpty == (len == 0), "emptiness");
                    check(cardinality1 == cardinality2, "cardinalities");
                    check(len <= s.Count, "length <= size");
                    check(len >= 0, "length >= 0");
                    check(cardinality1 >= 0, "cardinality >= 0");
                }
            }

            //public static void main(String[] args)
            //{

            //    //testFlipTime();

            //    // These are the single bit versions
            //    testSetGetClearFlip();

            //    // Test the ranged versions
            //    testClear();

            //    testFlip();
            //    testSet();
            //    testGet();

            //    // BitSet interaction calls
            //    testAndNot();
            //    testAnd();
            //    testOr();
            //    testXor();

            //    // Miscellaneous calls
            //    testLength();
            //    testEquals();
            //    testNextSetBit();
            //    testNextClearBit();
            //    testIntersects();
            //    testCardinality();
            //    testEmpty();
            //    testEmpty2();
            //    testToString();
            //    testLogicalIdentities();

            //    if (failure)
            //        throw new RuntimeException("One or more BitSet failures.");
            //}

            private static void report(String testName, int failCount)
            {
                Console.Error.WriteLine(testName + ": " +
                                   (failCount == 0 ? "Passed" : "Failed(" + failCount + ")"));
                //if (failCount > 0)
                //    failure = true;
            }

            [Test]
            public static void TestFlipTime()
            {
                // Make a fairly random bitset
                BitSet b1 = new BitSet();
                b1.Set(1000);
                long startTime = Time.CurrentTimeMilliseconds();
                for (int x = 0; x < 100000; x++)
                {
                    b1.Flip(100, 900);
                }
                long endTime = Time.CurrentTimeMilliseconds();
                long total = endTime - startTime;
                Console.Out.WriteLine("Multiple word flip Time " + total);

                startTime = Time.CurrentTimeMilliseconds();
                for (int x = 0; x < 100000; x++)
                {
                    b1.Flip(2, 44);
                }
                endTime = Time.CurrentTimeMilliseconds();
                total = endTime - startTime;
                Console.Out.WriteLine("Single word flip Time " + total);
            }

            [Test]
            public static void TestNextSetBit()
            {
                int failCount = 0;

                for (int i = 0; i < 100; i++)
                {
                    int numberOfSetBits = generator.Next(100) + 1;
                    BitSet testSet = new BitSet();
                    int[] history = new int[numberOfSetBits];

                    // Set some random bits and remember them
                    int nextBitToSet = 0;
                    for (int x = 0; x < numberOfSetBits; x++)
                    {
                        nextBitToSet += generator.Next(30) + 1;
                        history[x] = nextBitToSet;
                        testSet.Set(nextBitToSet);
                    }

                    // Verify their retrieval using nextSetBit()
                    int historyIndex = 0;
                    for (int x = testSet.NextSetBit(0); x >= 0; x = testSet.NextSetBit(x + 1))
                    {
                        if (x != history[historyIndex++])
                            failCount++;
                    }

                    checkSanity(testSet);
                }

                report("NextSetBit                  ", failCount);
            }

            [Test]
            public static void TestNextClearBit()
            {
                int failCount = 0;

                for (int i = 0; i < 1000; i++)
                {
                    BitSet b = new BitSet(256);
                    int[] history = new int[10];

                    // Set all the bits
                    for (int x = 0; x < 256; x++)
                        b.Set(x);

                    // Clear some random bits and remember them
                    int nextBitToClear = 0;
                    for (int x = 0; x < 10; x++)
                    {
                        nextBitToClear += generator.Next(24) + 1;
                        history[x] = nextBitToClear;
                        b.Clear(nextBitToClear);
                    }

                    // Verify their retrieval using nextClearBit()
                    int historyIndex = 0;
                    for (int x = b.NextClearBit(0); x < 256; x = b.NextClearBit(x + 1))
                    {
                        if (x != history[historyIndex++])
                            failCount++;
                    }

                    checkSanity(b);
                }

                // regression test for 4350178
                BitSet bs = new BitSet();
                if (bs.NextClearBit(0) != 0)
                    failCount++;
                for (int i = 0; i < 64; i++)
                {
                    bs.Set(i);
                    if (bs.NextClearBit(0) != i + 1)
                        failCount++;
                }

                checkSanity(bs);

                report("NextClearBit                ", failCount);
            }

            [Test]
            public static void TestSetGetClearFlip()
            {
                int failCount = 0;

                for (int i = 0; i < 100; i++)
                {
                    BitSet testSet = new BitSet();
                    HashSet<int?> history = new HashSet<int?>();

                    // Set a random number of bits in random places
                    // up to a random maximum
                    int nextBitToSet = 0;
                    int numberOfSetBits = generator.Next(100) + 1;
                    int highestPossibleSetBit = generator.Next(1000) + 1;
                    for (int x = 0; x < numberOfSetBits; x++)
                    {
                        nextBitToSet = generator.Next(highestPossibleSetBit);
                        history.Add(new int?(nextBitToSet));
                        testSet.Set(nextBitToSet);
                    }

                    // Make sure each bit is set appropriately
                    for (int x = 0; x < highestPossibleSetBit; x++)
                    {
                        if (testSet.Get(x) != history.Contains(new int?(x)))
                            failCount++;
                    }

                    // Clear the bits
                    var setBitIterator = history.GetEnumerator();
                    while (setBitIterator.MoveNext())
                    {
                        var setBit = setBitIterator.Current;
                        testSet.Clear(setBit.Value);
                    }

                    // Verify they were cleared
                    for (int x = 0; x < highestPossibleSetBit; x++)
                        if (testSet.Get(x))
                            failCount++;
                    if (testSet.Length != 0)
                        failCount++;

                    // Set them with set(int, bool)
                    setBitIterator = history.GetEnumerator();
                    while (setBitIterator.MoveNext())
                    {
                        var setBit = setBitIterator.Current;
                        testSet.Set(setBit.Value, true);
                    }

                    // Make sure each bit is set appropriately
                    for (int x = 0; x < highestPossibleSetBit; x++)
                    {
                        if (testSet.Get(x) != history.Contains(new int?(x)))
                            failCount++;
                    }

                    // Clear them with set(int, bool)
                    setBitIterator = history.GetEnumerator();
                    while (setBitIterator.MoveNext())
                    {
                        var setBit = (int?)setBitIterator.Current;
                        testSet.Set(setBit.Value, false);
                    }

                    // Verify they were cleared
                    for (int x = 0; x < highestPossibleSetBit; x++)
                        if (testSet.Get(x))
                            failCount++;
                    if (testSet.Length != 0)
                        failCount++;

                    // Flip them on
                    setBitIterator = history.GetEnumerator();
                    while (setBitIterator.MoveNext())
                    {
                        var setBit = (int?)setBitIterator.Current;
                        testSet.Flip(setBit.Value);
                    }

                    // Verify they were flipped
                    for (int x = 0; x < highestPossibleSetBit; x++)
                    {
                        if (testSet.Get(x) != history.Contains(new int?(x)))
                            failCount++;
                    }

                    // Flip them off
                    setBitIterator = history.GetEnumerator();
                    while (setBitIterator.MoveNext())
                    {
                        var setBit = (int?)setBitIterator.Current;
                        testSet.Flip(setBit.Value);
                    }

                    // Verify they were flipped
                    for (int x = 0; x < highestPossibleSetBit; x++)
                        if (testSet.Get(x))
                            failCount++;
                    if (testSet.Length != 0)
                        failCount++;

                    checkSanity(testSet);
                }

                report("SetGetClearFlip             ", failCount);
            }

            [Test]
            public static void TestAndNot()
            {
                int failCount = 0;

                for (int i = 0; i < 100; i++)
                {
                    BitSet b1 = new BitSet(256);
                    BitSet b2 = new BitSet(256);

                    // Set some random bits in first set and remember them
                    //int nextBitToSet = 0;
                    for (int x = 0; x < 10; x++)
                        b1.Set(generator.Next(255));

                    // Set some random bits in second set and remember them
                    for (int x = 10; x < 20; x++)
                        b2.Set(generator.Next(255));

                    // andNot the sets together
                    BitSet b3 = (BitSet)b1.Clone();
                    b3.AndNot(b2);

                    // Examine each bit of b3 for errors
                    for (int x = 0; x < 256; x++)
                    {
                        bool bit1 = b1.Get(x);
                        bool bit2 = b2.Get(x);
                        bool bit3 = b3.Get(x);
                        if (!(bit3 == (bit1 & (!bit2))))
                            failCount++;
                    }
                    checkSanity(b1, b2, b3);
                }

                report("AndNot                      ", failCount);
            }

            [Test]
            public static void TestAnd()
            {
                int failCount = 0;

                for (int i = 0; i < 100; i++)
                {
                    BitSet b1 = new BitSet(256);
                    BitSet b2 = new BitSet(256);

                    // Set some random bits in first set and remember them
                    //int nextBitToSet = 0;
                    for (int x = 0; x < 10; x++)
                        b1.Set(generator.Next(255));

                    // Set more random bits in second set and remember them
                    for (int x = 10; x < 20; x++)
                        b2.Set(generator.Next(255));

                    // And the sets together
                    BitSet b3 = (BitSet)b1.Clone();
                    b3.And(b2);

                    // Examine each bit of b3 for errors
                    for (int x = 0; x < 256; x++)
                    {
                        bool bit1 = b1.Get(x);
                        bool bit2 = b2.Get(x);
                        bool bit3 = b3.Get(x);
                        if (!(bit3 == (bit1 & bit2)))
                            failCount++;
                    }
                    checkSanity(b1, b2, b3);
                }

                // `and' that happens to clear the last word
                BitSet b4 = makeSet(2, 127);
                b4.And(makeSet(2, 64));
                checkSanity(b4);
                if (!(b4.Equals(makeSet(2))))
                    failCount++;

                report("And                         ", failCount);
            }

            [Test]
            public static void TestOr()
            {
                int failCount = 0;

                for (int i = 0; i < 100; i++)
                {
                    BitSet b1 = new BitSet(256);
                    BitSet b2 = new BitSet(256);
                    int[] history = new int[20];

                    // Set some random bits in first set and remember them
                    int nextBitToSet = 0;
                    for (int x = 0; x < 10; x++)
                    {
                        nextBitToSet = generator.Next(255);
                        history[x] = nextBitToSet;
                        b1.Set(nextBitToSet);
                    }

                    // Set more random bits in second set and remember them
                    for (int x = 10; x < 20; x++)
                    {
                        nextBitToSet = generator.Next(255);
                        history[x] = nextBitToSet;
                        b2.Set(nextBitToSet);
                    }

                    // Or the sets together
                    BitSet b3 = (BitSet)b1.Clone();
                    b3.Or(b2);

                    // Verify the set bits of b3 from the history
                    //int historyIndex = 0;
                    for (int x = 0; x < 20; x++)
                    {
                        if (!b3.Get(history[x]))
                            failCount++;
                    }

                    // Examine each bit of b3 for errors
                    for (int x = 0; x < 256; x++)
                    {
                        bool bit1 = b1.Get(x);
                        bool bit2 = b2.Get(x);
                        bool bit3 = b3.Get(x);
                        if (!(bit3 == (bit1 | bit2)))
                            failCount++;
                    }
                    checkSanity(b1, b2, b3);
                }

                report("Or                          ", failCount);
            }

            [Test]
            public static void TestXor()
            {
                int failCount = 0;

                for (int i = 0; i < 100; i++)
                {
                    BitSet b1 = new BitSet(256);
                    BitSet b2 = new BitSet(256);

                    // Set some random bits in first set and remember them
                    //int nextBitToSet = 0;
                    for (int x = 0; x < 10; x++)
                        b1.Set(generator.Next(255));

                    // Set more random bits in second set and remember them
                    for (int x = 10; x < 20; x++)
                        b2.Set(generator.Next(255));

                    // Xor the sets together
                    BitSet b3 = (BitSet)b1.Clone();
                    b3.Xor(b2);

                    // Examine each bit of b3 for errors
                    for (int x = 0; x < 256; x++)
                    {
                        bool bit1 = b1.Get(x);
                        bool bit2 = b2.Get(x);
                        bool bit3 = b3.Get(x);
                        if (!(bit3 == (bit1 ^ bit2)))
                            failCount++;
                    }
                    checkSanity(b1, b2, b3);
                    b3.Xor(b3); checkEmpty(b3);
                }

                // xor that happens to clear the last word
                BitSet b4 = makeSet(2, 64, 127);
                b4.Xor(makeSet(64, 127));
                checkSanity(b4);
                if (!(b4.Equals(makeSet(2))))
                    failCount++;

                report("Xor                         ", failCount);
            }

            private static void TestEquals()
            {
                int failCount = 0;

                for (int i = 0; i < 100; i++)
                {
                    // Create BitSets of different sizes
                    BitSet b1 = new BitSet(generator.Next(1000) + 1);
                    BitSet b2 = new BitSet(generator.Next(1000) + 1);

                    // Set some random bits
                    int nextBitToSet = 0;
                    for (int x = 0; x < 10; x++)
                    {
                        nextBitToSet += generator.Next(50) + 1;
                        b1.Set(nextBitToSet);
                        b2.Set(nextBitToSet);
                    }

                    // Verify their equality despite different storage sizes
                    if (!b1.Equals(b2))
                        failCount++;
                    checkEquality(b1, b2);
                }

                report("Equals                      ", failCount);
            }

            [Test]
            public static void TestLength()
            {
                int failCount = 0;

                // Test length after set
                for (int i = 0; i < 100; i++)
                {
                    BitSet b1 = new BitSet(256);
                    int highestSetBit = 0;

                    for (int x = 0; x < 100; x++)
                    {
                        int nextBitToSet = generator.Next(255);
                        if (nextBitToSet > highestSetBit)
                            highestSetBit = nextBitToSet;
                        b1.Set(nextBitToSet);
                        if (b1.Length != highestSetBit + 1)
                            failCount++;
                    }
                    checkSanity(b1);
                }

                // Test length after flip
                for (int i = 0; i < 100; i++)
                {
                    BitSet b1 = new BitSet(256);
                    for (int x = 0; x < 100; x++)
                    {
                        // Flip a random range twice
                        int rangeStart = generator.Next(100);
                        int rangeEnd = rangeStart + generator.Next(100);
                        b1.Flip(rangeStart);
                        b1.Flip(rangeStart);
                        if (b1.Length != 0)
                            failCount++;
                        b1.Flip(rangeStart, rangeEnd);
                        b1.Flip(rangeStart, rangeEnd);
                        if (b1.Length != 0)
                            failCount++;
                    }
                    checkSanity(b1);
                }

                // Test length after or
                for (int i = 0; i < 100; i++)
                {
                    BitSet b1 = new BitSet(256);
                    BitSet b2 = new BitSet(256);
                    int bit1 = generator.Next(100);
                    int bit2 = generator.Next(100);
                    int highestSetBit = (bit1 > bit2) ? bit1 : bit2;
                    b1.Set(bit1);
                    b2.Set(bit2);
                    b1.Or(b2);
                    if (b1.Length != highestSetBit + 1)
                        failCount++;
                    checkSanity(b1, b2);
                }

                report("Length                      ", failCount);
            }

            [Test]
            public static void TestClear()
            {
                int failCount = 0;

                for (int i = 0; i < 1000; i++)
                {
                    BitSet b1 = new BitSet();

                    // Make a fairly random bitset
                    int numberOfSetBits = generator.Next(100) + 1;
                    int highestPossibleSetBit = generator.Next(1000) + 1;

                    for (int x = 0; x < numberOfSetBits; x++)
                        b1.Set(generator.Next(highestPossibleSetBit));

                    BitSet b2 = (BitSet)b1.Clone();

                    // Clear out a random range
                    int rangeStart = generator.Next(100);
                    int rangeEnd = rangeStart + generator.Next(100);

                    // Use the clear(int, int) call on b1
                    b1.Clear(rangeStart, rangeEnd);

                    // Use a loop on b2
                    for (int x = rangeStart; x < rangeEnd; x++)
                        b2.Clear(x);

                    // Verify their equality
                    if (!b1.Equals(b2))
                    {
                        Console.Out.WriteLine("rangeStart = " + rangeStart);
                        Console.Out.WriteLine("rangeEnd = " + rangeEnd);
                        Console.Out.WriteLine("b1 = " + b1);
                        Console.Out.WriteLine("b2 = " + b2);
                        failCount++;
                    }
                    checkEquality(b1, b2);
                }

                report("Clear                       ", failCount);
            }

            [Test]
            public static void TestSet()
            {
                int failCount = 0;

                // Test set(int, int)
                for (int i = 0; i < 1000; i++)
                {
                    BitSet b1 = new BitSet();

                    // Make a fairly random bitset
                    int numberOfSetBits = generator.Next(100) + 1;
                    int highestPossibleSetBit = generator.Next(1000) + 1;

                    for (int x = 0; x < numberOfSetBits; x++)
                        b1.Set(generator.Next(highestPossibleSetBit));

                    BitSet b2 = (BitSet)b1.Clone();

                    // Set a random range
                    int rangeStart = generator.Next(100);
                    int rangeEnd = rangeStart + generator.Next(100);

                    // Use the set(int, int) call on b1
                    b1.Set(rangeStart, rangeEnd);

                    // Use a loop on b2
                    for (int x = rangeStart; x < rangeEnd; x++)
                        b2.Set(x);

                    // Verify their equality
                    if (!b1.Equals(b2))
                    {
                        Console.Out.WriteLine("Set 1");
                        Console.Out.WriteLine("rangeStart = " + rangeStart);
                        Console.Out.WriteLine("rangeEnd = " + rangeEnd);
                        Console.Out.WriteLine("b1 = " + b1);
                        Console.Out.WriteLine("b2 = " + b2);
                        failCount++;
                    }
                    checkEquality(b1, b2);
                }

                // Test set(int, int, bool)
                for (int i = 0; i < 100; i++)
                {
                    BitSet b1 = new BitSet();

                    // Make a fairly random bitset
                    int numberOfSetBits = generator.Next(100) + 1;
                    int highestPossibleSetBit = generator.Next(1000) + 1;

                    for (int x = 0; x < numberOfSetBits; x++)
                        b1.Set(generator.Next(highestPossibleSetBit));

                    BitSet b2 = (BitSet)b1.Clone();
                    bool setOrClear = generator.NextBoolean();

                    // Set a random range
                    int rangeStart = generator.Next(100);
                    int rangeEnd = rangeStart + generator.Next(100);

                    // Use the set(int, int, bool) call on b1
                    b1.Set(rangeStart, rangeEnd, setOrClear);

                    // Use a loop on b2
                    for (int x = rangeStart; x < rangeEnd; x++)
                        b2.Set(x, setOrClear);

                    // Verify their equality
                    if (!b1.Equals(b2))
                    {
                        Console.Out.WriteLine("Set 2");
                        Console.Out.WriteLine("b1 = " + b1);
                        Console.Out.WriteLine("b2 = " + b2);
                        failCount++;
                    }
                    checkEquality(b1, b2);
                }

                report("Set                         ", failCount);
            }

            [Test]
            public static void TestFlip()
            {
                int failCount = 0;

                for (int i = 0; i < 1000; i++)
                {
                    BitSet b1 = new BitSet();

                    // Make a fairly random bitset
                    int numberOfSetBits = generator.Next(100) + 1;
                    int highestPossibleSetBit = generator.Next(1000) + 1;

                    for (int x = 0; x < numberOfSetBits; x++)
                        b1.Set(generator.Next(highestPossibleSetBit));

                    BitSet b2 = (BitSet)b1.Clone();

                    // Flip a random range
                    int rangeStart = generator.Next(100);
                    int rangeEnd = rangeStart + generator.Next(100);

                    // Use the flip(int, int) call on b1
                    b1.Flip(rangeStart, rangeEnd);

                    // Use a loop on b2
                    for (int x = rangeStart; x < rangeEnd; x++)
                        b2.Flip(x);

                    // Verify their equality
                    if (!b1.Equals(b2))
                        failCount++;
                    checkEquality(b1, b2);
                }

                report("Flip                        ", failCount);
            }

            [Test]
            public static void TestGet()
            {
                int failCount = 0;

                for (int i = 0; i < 1000; i++)
                {
                    BitSet b1 = new BitSet();

                    // Make a fairly random bitset
                    int numberOfSetBits = generator.Next(100) + 1;
                    int highestPossibleSetBit = generator.Next(1000) + 1;

                    for (int x = 0; x < numberOfSetBits; x++)
                        b1.Set(generator.Next(highestPossibleSetBit));

                    // Get a new set from a random range
                    int rangeStart = generator.Next(100);
                    int rangeEnd = rangeStart + generator.Next(100);

                    BitSet b2 = b1.Get(rangeStart, rangeEnd);

                    BitSet b3 = new BitSet();
                    for (int x = rangeStart; x < rangeEnd; x++)
                        b3.Set(x - rangeStart, b1.Get(x));

                    // Verify their equality
                    if (!b2.Equals(b3))
                    {
                        Console.Out.WriteLine("start=" + rangeStart);
                        Console.Out.WriteLine("end=" + rangeEnd);
                        Console.Out.WriteLine(b1);
                        Console.Out.WriteLine(b2);
                        Console.Out.WriteLine(b3);
                        failCount++;
                    }
                    checkEquality(b2, b3);
                }

                report("Get                         ", failCount);
            }

            [Test]
            public static void TestIntersects()
            {
                int failCount = 0;

                for (int i = 0; i < 100; i++)
                {
                    BitSet b1 = new BitSet(256);
                    BitSet b2 = new BitSet(256);

                    // Set some random bits in first set
                    int nextBitToSet = 0;
                    for (int x = 0; x < 30; x++)
                    {
                        nextBitToSet = generator.Next(255);
                        b1.Set(nextBitToSet);
                    }

                    // Set more random bits in second set
                    for (int x = 0; x < 30; x++)
                    {
                        nextBitToSet = generator.Next(255);
                        b2.Set(nextBitToSet);
                    }

                    // Make sure they intersect
                    nextBitToSet = generator.Next(255);
                    b1.Set(nextBitToSet);
                    b2.Set(nextBitToSet);

                    if (!b1.Intersects(b2))
                        failCount++;

                    // Remove the common set bits
                    b1.AndNot(b2);

                    // Make sure they don't intersect
                    if (b1.Intersects(b2))
                        failCount++;

                    checkSanity(b1, b2);
                }

                report("Intersects                  ", failCount);
            }

            [Test]
            public static void TestCardinality()
            {
                int failCount = 0;

                for (int i = 0; i < 100; i++)
                {
                    BitSet b1 = new BitSet(256);

                    // Set a random number of increasing bits
                    int nextBitToSet = 0;
                    int iterations = generator.Next(20) + 1;
                    for (int x = 0; x < iterations; x++)
                    {
                        nextBitToSet += generator.Next(20) + 1;
                        b1.Set(nextBitToSet);
                    }

                    if (b1.Cardinality != iterations)
                    {
                        Console.Out.WriteLine("Iterations is " + iterations);
                        Console.Out.WriteLine("Cardinality is " + b1.Cardinality);
                        failCount++;
                    }

                    checkSanity(b1);
                }

                report("Cardinality                 ", failCount);
            }

            [Test]
            public static void TestEmpty()
            {
                int failCount = 0;

                BitSet b1 = new BitSet();
                if (!b1.IsEmpty)
                    failCount++;

                int nextBitToSet = 0;
                int numberOfSetBits = generator.Next(100) + 1;
                int highestPossibleSetBit = generator.Next(1000) + 1;
                for (int x = 0; x < numberOfSetBits; x++)
                {
                    nextBitToSet = generator.Next(highestPossibleSetBit);
                    b1.Set(nextBitToSet);
                    if (b1.IsEmpty)
                        failCount++;
                    b1.Clear(nextBitToSet);
                    if (!b1.IsEmpty)
                        failCount++;
                }

                report("Empty                       ", failCount);
            }

            [Test]
            public static void TestEmpty2()
            {
                { BitSet t = new BitSet(); t.Set(100); t.Clear(3, 600); checkEmpty(t); }
                checkEmpty(new BitSet(0));
                checkEmpty(new BitSet(342));
                BitSet s = new BitSet(0);
                checkEmpty(s);
                s.Clear(92); checkEmpty(s);
                s.Clear(127, 127); checkEmpty(s);
                s.Set(127, 127); checkEmpty(s);
                s.Set(128, 128); checkEmpty(s);
                BitSet empty = new BitSet();
                { BitSet t = new BitSet(); t.And(empty); checkEmpty(t); }
                { BitSet t = new BitSet(); t.Or(empty); checkEmpty(t); }
                { BitSet t = new BitSet(); t.Xor(empty); checkEmpty(t); }
                { BitSet t = new BitSet(); t.AndNot(empty); checkEmpty(t); }
                { BitSet t = new BitSet(); t.And(t); checkEmpty(t); }
                { BitSet t = new BitSet(); t.Or(t); checkEmpty(t); }
                { BitSet t = new BitSet(); t.Xor(t); checkEmpty(t); }
                { BitSet t = new BitSet(); t.AndNot(t); checkEmpty(t); }
                { BitSet t = new BitSet(); t.And(makeSet(1)); checkEmpty(t); }
                { BitSet t = new BitSet(); t.And(makeSet(127)); checkEmpty(t); }
                { BitSet t = new BitSet(); t.And(makeSet(128)); checkEmpty(t); }
                { BitSet t = new BitSet(); t.Flip(7); t.Flip(7); checkEmpty(t); }
                { BitSet t = new BitSet(); checkEmpty(t.Get(200, 300)); }
                { BitSet t = makeSet(2, 5); check(t.Get(2, 6).Equals(makeSet(0, 3)), ""); }
            }

            [Test]
            public static void TestToString()
            {
                check(new BitSet().ToString().Equals("{}"));
                check(makeSet(2, 3, 42, 43, 234).ToString().Equals("{2, 3, 42, 43, 234}"));
            }

            [Test]
            public static void TestLogicalIdentities()
            {
                int failCount = 0;

                // Verify that (!b1)|(!b2) == !(b1&b2)
                for (int i = 0; i < 50; i++)
                {
                    // Construct two fairly random bitsets
                    BitSet b1 = new BitSet();
                    BitSet b2 = new BitSet();

                    int numberOfSetBits = generator.Next(100) + 1;
                    int highestPossibleSetBit = generator.Next(1000) + 1;

                    for (int x = 0; x < numberOfSetBits; x++)
                    {
                        b1.Set(generator.Next(highestPossibleSetBit));
                        b2.Set(generator.Next(highestPossibleSetBit));
                    }

                    BitSet b3 = (BitSet)b1.Clone();
                    BitSet b4 = (BitSet)b2.Clone();

                    for (int x = 0; x < highestPossibleSetBit; x++)
                    {
                        b1.Flip(x);
                        b2.Flip(x);
                    }
                    b1.Or(b2);
                    b3.And(b4);
                    for (int x = 0; x < highestPossibleSetBit; x++)
                        b3.Flip(x);
                    if (!b1.Equals(b3))
                        failCount++;
                    checkSanity(b1, b2, b3, b4);
                }

                // Verify that (b1&(!b2)|(b2&(!b1) == b1^b2
                for (int i = 0; i < 50; i++)
                {
                    // Construct two fairly random bitsets
                    BitSet b1 = new BitSet();
                    BitSet b2 = new BitSet();

                    int numberOfSetBits = generator.Next(100) + 1;
                    int highestPossibleSetBit = generator.Next(1000) + 1;

                    for (int x = 0; x < numberOfSetBits; x++)
                    {
                        b1.Set(generator.Next(highestPossibleSetBit));
                        b2.Set(generator.Next(highestPossibleSetBit));
                    }

                    BitSet b3 = (BitSet)b1.Clone();
                    BitSet b4 = (BitSet)b2.Clone();
                    BitSet b5 = (BitSet)b1.Clone();
                    BitSet b6 = (BitSet)b2.Clone();

                    for (int x = 0; x < highestPossibleSetBit; x++)
                        b2.Flip(x);
                    b1.And(b2);
                    for (int x = 0; x < highestPossibleSetBit; x++)
                        b3.Flip(x);
                    b3.And(b4);
                    b1.Or(b3);
                    b5.Xor(b6);
                    if (!b1.Equals(b5))
                        failCount++;
                    checkSanity(b1, b2, b3, b4, b5, b6);
                }
                report("Logical Identities          ", failCount);
            }

        }
    }
}
