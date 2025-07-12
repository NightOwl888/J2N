using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Integer = J2N.Numerics.Int32;
using JCG = J2N.Collections.Generic;

namespace J2N.Collections
{
    public class Support_CollectionTest : TestCase
    {
        readonly ICollection<Integer> col; // must contain the Integers 0 to 99

        public Support_CollectionTest(/*String p1*/)
            //: base(p1)
        {
        }

        public Support_CollectionTest(String p1, ICollection<Integer> c)
            //: base(p1)
        {
            col = c;
        }

        public void RunTest()
        {
            new Support_UnmodifiableCollectionTest("", col).RunTest();

            // setup
            ICollection<Integer> myCollection = new JCG.SortedSet<Integer>();
            myCollection.Add(new Integer(101));
            myCollection.Add(new Integer(102));
            myCollection.Add(new Integer(103));

            // add
            //assertTrue("CollectionTest - a) add did not work", col.Add(new Integer(
            //        101)));
            col.Add(new Integer(101)); // Does not return in .NET
            assertTrue("CollectionTest - b) add did not work", col
                    .Contains(new Integer(101)));

            // remove
            assertTrue("CollectionTest - a) remove did not work", col
                    .Remove(new Integer(101)));
            assertTrue("CollectionTest - b) remove did not work", !col
                    .Contains(new Integer(101)));

            if (col is ISet<Integer> set)
            {
                // addAll
                //assertTrue("CollectionTest - a) addAll failed", set
                //        .UnionWith(myCollection));
                set.UnionWith(myCollection); // Does not return in .NET
                assertTrue("CollectionTest - b) addAll failed", set
                        .IsSupersetOf(myCollection));

                // containsAll
                assertTrue("CollectionTest - a) containsAll failed", set
                        .IsSupersetOf(myCollection));
                col.Remove(new Integer(101));
                assertTrue("CollectionTest - b) containsAll failed", !set
                        .IsSupersetOf(myCollection));

                // removeAll
                //assertTrue("CollectionTest - a) removeAll failed", set
                //        .ExceptWith(myCollection));
                //assertTrue("CollectionTest - b) removeAll failed", !set
                //        .ExceptWith(myCollection)); // should not change the colletion
                //                                   // the 2nd time around

                set.ExceptWith(myCollection); // Does not return in .NET
                assertTrue("CollectionTest - c) removeAll failed", !set
                        .Contains(new Integer(102)));
                assertTrue("CollectionTest - d) removeAll failed", !set
                        .Contains(new Integer(103)));

                // retianAll
                set.UnionWith(myCollection);
                //assertTrue("CollectionTest - a) retainAll failed", set
                //        .IntersectWith(myCollection));
                //assertTrue("CollectionTest - b) retainAll failed", !set
                //        .IntersectWith(myCollection)); // should not change the colletion
                //                                   // the 2nd time around

                set.IntersectWith(myCollection); // Does not return in .NET
                assertTrue("CollectionTest - c) retainAll failed", set
                        .IsSupersetOf(myCollection));
                assertTrue("CollectionTest - d) retainAll failed", !set
                        .Contains(new Integer(0)));
                assertTrue("CollectionTest - e) retainAll failed", !set
                        .Contains(new Integer(50)));

            }

            // clear
            col.Clear();
            assertTrue("CollectionTest - a) clear failed", col.Count == 0);
            assertTrue("CollectionTest - b) clear failed", !col
                    .Contains(new Integer(101)));

        }

    }
}
