using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace J2N.Collections.Generic.Extensions
{
    public class TestListExtensions : TestCase
    {
        private IList<object> ll;

        private static object[] objArray = LoadObjectArray();

        private static object[] LoadObjectArray()
        {
            var objArray = new object[1000];
            for (int i = 0; i < objArray.Length; i++)
            {
                objArray[i] = new int?(i);
            }
            return objArray;
        }

        /**
        * @tests java.util.Collections#shuffle(java.util.List)
        */
        [Test]
        public void Test_shuffleLjava_util_List()
        {
            // Test for method void java.util.Collections.shuffle(java.util.List)
            // Assumes ll is sorted and has no duplicate keys and is large ( > 20
            // elements)

            // test shuffling a Sequential Access List
            try
            {
                ((IList<object>)null).Shuffle();
                fail("Expected ArgumentNullException for null list parameter");
            }
            catch (ArgumentNullException e)
            {
                //Expected
            }
            var al = new List<object>();
            al.AddRange(ll);
            testShuffle(al, "Sequential Access", false);

            // test shuffling a Random Access List
            var ll2 = new List<object>();
            ll2.AddRange(ll);
            testShuffle(ll2, "Random Access", false);
        }

        // J2N: No "Random Access" in .NET, this test is redundant.
        //[Test]
        //public void TestShuffleRandomAccessWithSeededRandom()
        //{
        //    var list = new string[] { "A", "B", "C", "D", "E", "F", "G" }.ToList();
        //    list.Shuffle(new Random(0));
        //    assertTrue(new string[] {"B", "A", "D", "C", "G", "E", "F"}.SequenceEqual(list));
        //}

        [Test]
        public void TestShuffleWithSeededRandom()
        {
            var list = new string[] { "A", "B", "C", "D", "E", "F", "G" }.ToList();
            list.Shuffle(new Randomizer(0));
            assertTrue(new string[] { "B", "A", "D", "C", "G", "E", "F" }.SequenceEqual(list));
        }

        private void testShuffle(IList<object> list, string type, bool random)
        {
            bool sorted = true;
            bool allMatch = true;
            int index = 0;
            int size = list.Count;

            if (random)
                list.Shuffle();
            else
                list.Shuffle(new Random(200));

            for (int counter = 0; counter < size - 1; counter++)
            {
                if (((int)list[counter]).CompareTo((int)list[counter + 1]) > 0)
                {
                    sorted = false;
                }
            }
            assertFalse("Shuffling sorted " + type
                    + " list resulted in sorted list (should be unlikely)", sorted);
            for (int counter = 0; counter < 20; counter++)
            {
                index = 30031 * counter % (size + 1); // 30031 is a large prime
                if (list[index] != ll[index])
                    allMatch = false;
            }
            assertFalse("Too many element positions match in shuffled " + type
                    + " list", allMatch);
        }

        /**
         * @tests java.util.Collections#shuffle(java.util.List, java.util.Random)
         */
        [Test]
        public void Test_shuffleLjava_util_ListLjava_util_Random()
        {
            // Test for method void java.util.Collections.shuffle(java.util.List,
            // java.util.Random)
            // Assumes ll is sorted and has no duplicate keys and is large ( > 20
            // elements)

            // test shuffling a Sequential Access List
            try
            {
                ((IList<object>)null).Shuffle(new Random(200));
                fail("Expected ArgumentNullException for null list parameter");
            }
            catch (ArgumentNullException e)
            {
                //Excepted
            }
            var al = new List<object>();
            al.AddRange(ll);
            testShuffle(al, "Sequential Access", true);

            // test shuffling a Random Access List
            var ll2 = new List<object>();
            ll2.AddRange(ll);
            testShuffle(ll2, "Random Access", true);

            var l = new List<object>();
            l.Add('a');
            l.Add('b');
            l.Add('c');
            l.Shuffle(new Randomizer(12345678921L));
            assertEquals("acb", l[0].ToString() + l[1] + l[2]);
        }

        [Test]
        public void TestShuffle_IsReadOnly()
        {
            var l = Enumerable.Repeat(false, 100).ToList().ToUnmodifiableList();

            Assert.Throws<NotSupportedException>(() => l.Shuffle());
            Assert.Throws<NotSupportedException>(() => l.Shuffle(Random));
        }


        /**
         * @tests java.util.Collections#swap(java.util.List, int, int)
         */
        [Test]
        public void Test_swapLjava_util_ListII()
        {
            // Test for method swap(java.util.List, int, int)

            var smallList = new List<object>();
            for (int i = 0; i < 10; i++)
            {
                smallList.Add(objArray[i]);
            }

            // test exception cases
            try
            {
                smallList.Swap(-1, 6);
                fail("Expected ArgumentOutOfRangeException for -1");
            }
            catch (ArgumentOutOfRangeException e)
            {
                //Expected
            }

            try
            {
                smallList.Swap(6, -1);
                fail("Expected ArgumentOutOfRangeException for -1");
            }
            catch (ArgumentOutOfRangeException e)
            {
                //Expected
            }

            try
            {
                smallList.Swap(6, 11);
                fail("Expected ArgumentOutOfRangeException for 11");
            }
            catch (ArgumentOutOfRangeException e)
            {
                //Expected
            }

            try
            {
                smallList.Swap(11, 6);
                fail("Expected ArgumentOutOfRangeException for 11");
            }
            catch (ArgumentOutOfRangeException e)
            {
                //Expected
            }

            // Ensure a NPE is thrown if the list is NULL
            try
            {
                ((IList<object>)null).Swap(1, 1);
                fail("Expected ArgumentNullException for null list parameter");
            }
            catch (ArgumentNullException e)
            {
                //Expected
            }

            // test with valid parameters
            smallList.Swap(4, 7);
            assertEquals("Didn't Swap the element at position 4 ", new int?(7),
                    smallList[4]);
            assertEquals("Didn't Swap the element at position 7 ", new int?(4),
                    smallList[7]);

            // make sure other elements didn't get swapped by mistake
            for (int i = 0; i < 10; i++)
            {
                if (i != 4 && i != 7)
                    assertEquals("shouldn't have swapped the element at position "
                            + i, new int?(i), smallList[i]);
            }
        }

        [Test]
        public void TestSwap_JDK7()
        {
            int size = 100;

            var l = Enumerable.Repeat(false, 100).ToList();
            l[0] = true;
            for (int i = 0; i < size - 1; i++)
                l.Swap(i, i + 1);

            var l2 = Enumerable.Repeat(false, 100).ToList();
            l2[size - 1] = true;
            assertTrue(l.SequenceEqual(l2));
        }

        [Test]
        public void TestSwap_IsReadOnly()
        {
            var l = Enumerable.Repeat(false, 100).ToList().ToUnmodifiableList();

            Assert.Throws<NotSupportedException>(() => l.Swap(0, 1));
        }


        /**
         * Sets up the fixture, for example, open a network connection. This method
         * is called before a test is executed.
         */
        public override void SetUp()
        {
            ll = new List<object>();
            //myll = new List<object>();
            //s = new HashSet<object>();
            //mys = new HashSet<object>();
            //reversedLinkedList = new LinkedList(); // to be sorted in reverse order
            //myReversedLinkedList = new LinkedList(); // to be sorted in reverse
            // order
            //hm = new Dictionary<object, object>();
            for (int i = 0; i < objArray.Length; i++)
            {
                ll.Add(objArray[i]);
                //myll.Add(myobjArray[i]);
                //s.Add(objArray[i]);
                //mys.Add(myobjArray[i]);
                //reversedLinkedList.Add(objArray[objArray.Length - i - 1]);
                //myReversedLinkedList.Add(myobjArray[myobjArray.Length - i - 1]);
                //hm[objArray[i].ToString()] = objArray[i];
            }
        }

        /**
         * Tears down the fixture, for example, close a network connection. This
         * method is called after a test is executed.
         */
        public override void TearDown()
        {
        }
    }
}
