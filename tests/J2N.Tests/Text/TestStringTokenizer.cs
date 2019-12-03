using NUnit.Framework;
using System;

namespace J2N.Text
{
    public class TestStringTokenizer : TestCase
    {
        /**
	     * @tests java.util.StringTokenizer#StringTokenizer(java.lang.String)
	     */
        [Test]
        public void Test_ConstructorLjava_lang_String()
        {
            // Test for method java.util.StringTokenizer(java.lang.String)
            assertTrue("Used in tests", true);
        }

        /**
         * @tests java.util.StringTokenizer#StringTokenizer(java.lang.String,
         *        java.lang.String)
         */
        [Test]
        public void Test_ConstructorLjava_lang_StringLjava_lang_String()
        {
            // Test for method java.util.StringTokenizer(java.lang.String,
            // java.lang.String)
            StringTokenizer st = new StringTokenizer("This:is:a:test:String", ":");

            assertTrue("Created incorrect tokenizer", st.CountTokens() == 5
                    && st.MoveNext() && (st.Current.Equals("This", StringComparison.Ordinal)));
        }

        /**
         * @tests java.util.StringTokenizer#StringTokenizer(java.lang.String,
         *        java.lang.String, boolean)
         */
        [Test]
        public void Test_ConstructorLjava_lang_StringLjava_lang_StringZ()
        {
            // Test for method java.util.StringTokenizer(java.lang.String,
            // java.lang.String, boolean)
            StringTokenizer st = new StringTokenizer("This:is:a:test:String", ":",
                    true);
            st.MoveNext();
            assertTrue("Created incorrect tokenizer", st.CountTokens() == 8
                    && st.MoveNext() && (st.Current.Equals(":", StringComparison.Ordinal)));
        }

        [Test]
        public void Test_RemainingTokens()
        {
            // Test for method java.util.StringTokenizer(java.lang.String,
            // java.lang.String, boolean)
            StringTokenizer st = new StringTokenizer("This:is:a:test:String", ":",
                    true);
            st.MoveNext();
            assertEquals(8, st.RemainingTokens);

            while (st.MoveNext())
                assertEquals(st.CountTokens(), st.RemainingTokens);
        }

        /**
         * @tests java.util.StringTokenizer#countTokens()
         */
        [Test]
        public void Test_countTokens()
        {
            // Test for method int java.util.StringTokenizer.countTokens()
            StringTokenizer st = new StringTokenizer("This is a test String");

            assertEquals("Incorrect token count returned", 5, st.CountTokens());
        }

        ///**
        // * @tests java.util.StringTokenizer#hasMoreElements()
        // */
        //[Test]
        //public void test_hasMoreElements()
        //{
        //    // Test for method boolean java.util.StringTokenizer.hasMoreElements()

        //    StringTokenizer st = new StringTokenizer("This is a test String");
        //    st.NextToken();
        //    assertTrue("hasMoreElements returned incorrect value", st
        //            .hasMoreElements());
        //    st.NextToken();
        //    st.NextToken();
        //    st.NextToken();
        //    st.NextToken();
        //    assertTrue("hasMoreElements returned incorrect value", !st
        //            .hasMoreElements());
        //}

        /**
         * @tests java.util.StringTokenizer#hasMoreTokens()
         */
        [Test]
        public void Test_hasMoreTokens()
        {
            // Test for method boolean java.util.StringTokenizer.hasMoreTokens()
            StringTokenizer st = new StringTokenizer("This is a test String");
            for (int counter = 0; counter < 5; counter++)
            {
                assertTrue(
                        "StringTokenizer incorrectly reports it has no more tokens",
                        st.HasMoreTokens());
                st.MoveNext();
            }
            assertTrue("StringTokenizer incorrectly reports it has more tokens",
                    !st.HasMoreTokens());
        }

        ///**
        // * @tests java.util.StringTokenizer#nextElement()
        // */
        //[Test]
        //public void test_nextElement()
        //{
        //    // Test for method java.lang.Object
        //    // java.util.StringTokenizer.nextElement()
        //    StringTokenizer st = new StringTokenizer("This is a test String");
        //    assertEquals("nextElement returned incorrect value", "This", ((String)st
        //            .NextToken()));
        //    assertEquals("nextElement returned incorrect value", "is", ((String)st
        //            .NextToken()));
        //    assertEquals("nextElement returned incorrect value", "a", ((String)st
        //            .NextToken()));
        //    assertEquals("nextElement returned incorrect value", "test", ((String)st
        //            .NextToken()));
        //    assertEquals("nextElement returned incorrect value", "String", ((String)st
        //            .NextToken()));
        //    try
        //    {
        //        st.NextToken();
        //        fail(
        //                "nextElement failed to throw a NoSuchElementException when it should have been out of elements");
        //    }
        //    catch (InvalidOperationException e)
        //    {
        //        return;
        //    }
        //}

        /**
         * @tests java.util.StringTokenizer#nextToken()
         */
        [Test]
        public void Test_nextToken()
        {
            // Test for method java.lang.String
            // java.util.StringTokenizer.nextToken()
            StringTokenizer st = new StringTokenizer("This is a test String");

            st.MoveNext();
            assertEquals("nextToken returned incorrect value",
                    "This", st.Current);

            st.MoveNext();
            assertEquals("nextToken returned incorrect value",
                    "is", st.Current);

            st.MoveNext();
            assertEquals("nextToken returned incorrect value",
                    "a", st.Current);

            st.MoveNext();
            assertEquals("nextToken returned incorrect value",
                    "test", st.Current);

            st.MoveNext();
            assertEquals("nextToken returned incorrect value",
                    "String", st.Current);

            // Not a valid use case - can never happen in .NET
//            try
//            {
//                st.NextToken();
//                fail(
//                        "nextToken failed to throw a NoSuchElementException when it should have been out of elements");
//            }
//#pragma warning disable 168
//            catch (InvalidOperationException e)
//#pragma warning restore 168
//            {
//                return;
//            }
        }

        /**
         * @tests java.util.StringTokenizer#nextToken(java.lang.String)
         */
        [Test]
        public void Test_nextTokenLjava_lang_String()
        {
            // Test for method java.lang.String
            // java.util.StringTokenizer.nextToken(java.lang.String)
            StringTokenizer st = new StringTokenizer("This is a test String");

            st.MoveNext(" ");
            assertEquals("nextToken(String) returned incorrect value with normal token String",
                    "This", st.Current);

            st.MoveNext("tr");
            assertEquals("nextToken(String) returned incorrect value with custom token String",
                    " is a ", st.Current);

            st.MoveNext();
            assertEquals("calling nextToken() did not use the new default delimiter list",
                    "es", st.Current);
        }

        //[Test]
        //public void test_hasMoreElements_NPE()
        //{
        //    StringTokenizer stringTokenizer = new StringTokenizer(new String(),
        //            (String)null, true);
        //    try
        //    {
        //        stringTokenizer.HasMoreElements();
        //        fail("should throw NullPointerException");
        //    }
        //    catch (NullPointerException e)
        //    {
        //        // Expected
        //    }

        //    stringTokenizer = new StringTokenizer(new String(), (String)null);
        //    try
        //    {
        //        stringTokenizer.hasMoreElements();
        //        fail("should throw NullPointerException");
        //    }
        //    catch (NullPointerException e)
        //    {
        //        // Expected
        //    }
        //}

            // Not valid - in .NET we throw exceptions based on null input, we don't
            // throw exceptions based on normal workflow
//        [Test]
//        public void Test_hasMoreTokens_NPE()
//        {
//            StringTokenizer stringTokenizer = new StringTokenizer("",
//                    (String)null, true);
//            try
//            {
//                stringTokenizer.HasMoreTokens();
//                fail("should throw NullPointerException");
//            }
//#pragma warning disable 168
//            catch (ArgumentNullException e)
//#pragma warning restore 168
//            {
//                // Expected
//            }

//            stringTokenizer = new StringTokenizer("", (String)null);
//            try
//            {
//                stringTokenizer.HasMoreTokens();
//                fail("should throw NullPointerException");
//            }
//#pragma warning disable 168
//            catch (ArgumentNullException e)
//#pragma warning restore 168
//            {
//                // Expected
//            }
//        }

        //[Test]
        //public void test_nextElement_NPE()
        //{
        //    StringTokenizer stringTokenizer = new StringTokenizer(new string(),
        //            (String)null, true);
        //    try
        //    {
        //        stringTokenizer.NextToken();
        //        fail("should throw NullPointerException");
        //    }
        //    catch (ArgumentNullException e)
        //    {
        //        // Expected
        //    }

        //    stringTokenizer = new StringTokenizer(new String(), (String)null);
        //    try
        //    {
        //        stringTokenizer.NextToken();
        //        fail("should throw NullPointerException");
        //    }
        //    catch (ArgumentNullException e)
        //    {
        //        // Expected
        //    }
        //}

        [Test]
        public void Test_nextToken_NPE()
        {
            StringTokenizer stringTokenizer;
            try
            {
                stringTokenizer = new StringTokenizer("",
                    (String)null, true);
                //stringTokenizer.NextToken();
                fail("should throw NullPointerException");
            }
#pragma warning disable 168
            catch (ArgumentNullException e)
#pragma warning restore 168
            {
                // Expected
            }

            
            try
            {
                stringTokenizer = new StringTokenizer("", (String)null);
                //stringTokenizer.NextToken();
                fail("should throw NullPointerException");
            }
#pragma warning disable 168
            catch (ArgumentNullException e)
#pragma warning restore 168
            {
                // Expected
            }
        }

        [Test]
        public void Test_nextTokenLjava_lang_String_NPE()
        {
            StringTokenizer stringTokenizer = new StringTokenizer("");
            try
            {
                stringTokenizer.MoveNext(null);
                fail("should throw NullPointerException");
            }
#pragma warning disable 168
            catch (ArgumentNullException e)
#pragma warning restore 168
            {
                // Expected
            }
        }

        /**
         * Sets up the fixture, for example, open a network connection. This method
         * is called before a test is executed.
         */
        public override void SetUp()
        {
            base.SetUp();
        }

        /**
         * Tears down the fixture, for example, close a network connection. This
         * method is called after a test is executed.
         */
        public override void TearDown()
        {
            base.TearDown();
        }

    }
}
