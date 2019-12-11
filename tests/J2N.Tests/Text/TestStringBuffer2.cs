using J2N.Globalization;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace J2N.Text
{
    /// <summary>
    /// Additional tests from Apache Harmony, StringBuffer2Test.java
    /// </summary>
    public class TestStringBuffer2 : TestCase
    {
        StringBuffer testBuffer;

        /**
         * @tests java.lang.StringBuffer#StringBuffer()
         */
        [Test]
        public void Test_Constructor()
        {
            // Test for method java.lang.StringBuffer()
            new StringBuffer();
            assertTrue("Invalid buffer created", true);
        }

        /**
         * @tests java.lang.StringBuffer#StringBuffer(int)
         */
        [Test]
        public void Test_ConstructorI()
        {
            // Test for method java.lang.StringBuffer(int)
            StringBuffer sb = new StringBuffer(8);
            assertEquals("Newly constructed buffer is of incorrect length", 0, sb
                    .Length);
        }

        /**
         * @tests java.lang.StringBuffer#StringBuffer(java.lang.String)
         */
        [Test]
        public void Test_ConstructorLjava_lang_String()
        {
            // Test for method java.lang.StringBuffer(java.lang.String)

            StringBuffer sb = new StringBuffer("HelloWorld");

            assertTrue("Invalid buffer created", sb.Length == 10
                    && (sb.ToString().Equals("HelloWorld")));

            //bool pass = false;
            //try
            //{
            //    new StringBuffer((string)null);
            //}
            //catch (ArgumentNullException e)
            //{
            //    pass = true;
            //}
            //assertTrue("Should throw NullPointerException", pass);

            assertEquals(string.Empty, new StringBuffer((string)null).ToString()); // J2N: Matching .NET behavior - a null in the constructor is a no-op
        }

        /**
         * @tests java.lang.StringBuffer#append(char[])
         */
        [Test]
        public void Test_append_C()
        {
            // Test for method java.lang.StringBuffer
            // java.lang.StringBuffer.Append(char [])
            char[] buf = new char[4];
            //"char".getChars(0, 4, buf, 0);
            "char".CopyTo(0, buf, 0, 4);
            testBuffer.Append(buf);
            assertEquals("Append of char[] failed",
                    "This is a test bufferchar", testBuffer.ToString());
        }

        /**
         * @tests java.lang.StringBuffer#append(char[], int, int)
         */
        [Test]
        public void Test_append_CII()
        {
            // Test for method java.lang.StringBuffer
            // java.lang.StringBuffer.Append(char [], int, int)
            StringBuffer sb = new StringBuffer();
            char[] buf1 = { 'H', 'e', 'l', 'l', 'o' };
            char[] buf2 = { 'W', 'o', 'r', 'l', 'd' };
            sb.Append(buf1, 0, buf1.Length);
            assertEquals("Buffer is invalid length after append", 5, sb.Length);
            sb.Append(buf2, 0, buf2.Length);
            assertEquals("Buffer is invalid length after append", 10, sb.Length);
            assertTrue("Buffer contains invalid chars", (sb.ToString()
                    .Equals("HelloWorld")));
        }

        /**
         * @tests java.lang.StringBuffer#append(char)
         */
        [Test]
        public void Test_appendC()
        {
            // Test for method java.lang.StringBuffer
            // java.lang.StringBuffer.Append(char)
            StringBuffer sb = new StringBuffer();
            char buf1 = 'H';
            char buf2 = 'W';
            sb.Append(buf1);
            assertEquals("Buffer is invalid length after append", 1, sb.Length);
            sb.Append(buf2);
            assertEquals("Buffer is invalid length after append", 2, sb.Length);
            assertTrue("Buffer contains invalid chars",
                    (sb.ToString().Equals("HW")));
        }

        /**
         * @tests java.lang.StringBuffer#append(double)
         */
        [Test]
        public void Test_appendD()
        {
            using (var context = new CultureContext(CultureInfo.InvariantCulture))
            {

                // Test for method java.lang.StringBuffer
                // java.lang.StringBuffer.Append(double)
                StringBuffer sb = new StringBuffer();
                sb.Append(double.MaxValue);
                assertEquals("Buffer is invalid length after append", double.MaxValue.ToString().Length, sb.Length); // J2N: Exact format of string is dependent upon framework implementation, so we are testing whether the append matches the re-generated string
                assertEquals("Buffer contains invalid characters",
                        /*"1.7976931348623157E+308"*/ double.MaxValue.ToString(), sb.ToString()); ;
            }
        }

        /**
         * @tests java.lang.StringBuffer#append(float)
         */
        [Test]
        public void Test_appendF()
        {
            // Test for method java.lang.StringBuffer
            // java.lang.StringBuffer.Append(float)
            StringBuffer sb = new StringBuffer();
            float floatNum = 900.87654F;
            sb.Append(floatNum);
            assertTrue("Buffer is invalid length after append: " + sb.Length, sb
                    .Length == floatNum.ToString().Length);
            assertTrue("Buffer contains invalid characters", sb.ToString().Equals(
                    floatNum.ToString()));
        }

        /**
         * @tests java.lang.StringBuffer#append(int)
         */
        [Test]
        public void Test_appendI()
        {
            // Test for method java.lang.StringBuffer
            // java.lang.StringBuffer.Append(int)
            StringBuffer sb = new StringBuffer();
            sb.Append(9000);
            assertEquals("Buffer is invalid length after append", 4, sb.Length);
            sb.Append(1000);
            assertEquals("Buffer is invalid length after append", 8, sb.Length);
            assertEquals("Buffer contains invalid characters",
                    "90001000", sb.ToString());
        }

        /**
         * @tests java.lang.StringBuffer#append(long)
         */
        [Test]
        public void Test_appendJ()
        {
            // Test for method java.lang.StringBuffer
            // java.lang.StringBuffer.Append(long)

            StringBuffer sb = new StringBuffer();
            long t = 927654321098L;
            sb.Append(t);
            assertEquals("Buffer is of invlaid length", 12, sb.Length);
            assertEquals("Buffer contains invalid characters",
                    "927654321098", sb.ToString());
        }

        /**
         * @tests java.lang.StringBuffer#append(java.lang.Object)
         */
        [Test]
        public void Test_appendLjava_lang_Object()
        {
            // Test for method java.lang.StringBuffer
            // java.lang.StringBuffer.Append(java.lang.Object)
            StringBuffer sb = new StringBuffer();
            Object obj1 = new Object();
            Object obj2 = new Object();
            sb.Append(obj1);
            sb.Append(obj2);
            assertTrue("Buffer contains invalid characters", sb.ToString().Equals(
                    obj1.ToString() + obj2.ToString()));
        }

        /**
         * @tests java.lang.StringBuffer#append(java.lang.String)
         */
        [Test]
        public void Test_appendLjava_lang_String()
        {
            // Test for method java.lang.StringBuffer
            // java.lang.StringBuffer.Append(java.lang.String)
            StringBuffer sb = new StringBuffer();
            String buf1 = "Hello";
            String buf2 = "World";
            sb.Append(buf1);
            assertEquals("Buffer is invalid length after append", 5, sb.Length);
            sb.Append(buf2);
            assertEquals("Buffer is invalid length after append", 10, sb.Length);
            assertTrue("Buffer contains invalid chars", (sb.ToString()
                    .Equals("HelloWorld")));
        }

        /**
         * @tests java.lang.StringBuffer#append(boolean)
         */
        [Test]
        public void Test_appendZ()
        {
            // Test for method java.lang.StringBuffer
            // java.lang.StringBuffer.Append(boolean)
            StringBuffer sb = new StringBuffer();
            sb.Append(false);
            assertEquals("Buffer is invalid length after append", 5, sb.Length);
            sb.Append(true);
            assertEquals("Buffer is invalid length after append", 9, sb.Length);
            assertTrue("Buffer is invalid length after append", (sb.ToString()
                    .Equals("FalseTrue"))); // J2N: In .NET, the "False" and "True" are title case
        }

        /**
         * @tests java.lang.StringBuffer#capacity()
         */
        [Test]
        public void Test_capacity()
        {
            // Test for method int java.lang.StringBuffer.Capacity
            StringBuffer sb = new StringBuffer(10);
            assertEquals("Returned incorrect capacity", 10, sb.Capacity);
            sb.EnsureCapacity(100);
            assertTrue("Returned incorrect capacity", sb.Capacity >= 100);
        }

        /**
         * @tests java.lang.StringBuffer#charAt(int)
         */
        [Test]
        public void Test_charAtI()
        {
            // Test for method char java.lang.StringBuffer.charAt(int)
            assertEquals("Returned incorrect char", 's', testBuffer[3]);

            // Test for StringIndexOutOfBoundsException
            bool exception = false;
            try
            {
                var _ = testBuffer[-1];
            }
            catch (IndexOutOfRangeException e)
            {
                exception = true;
            }
            catch (ArgumentOutOfRangeException e)
            {
            }
            assertTrue("Should throw IndexOutOfRangeException", exception);
        }

        /**
         * @tests java.lang.StringBuffer#delete(int, int)
         */
        [Test]
        public void Test_deleteII()
        {
            // Test for method java.lang.StringBuffer
            // java.lang.StringBuffer.delete(int, int)
            testBuffer.Delete(7, 7 - 7); // J2N: Corrected 2nd parameter
            assertEquals("Deleted chars when start == end", "This is a test buffer", testBuffer.ToString()
                    );
            testBuffer.Delete(4, 14 - 4); // J2N: Corrected 2nd parameter
            assertEquals("Deleted incorrect chars",
                    "This buffer", testBuffer.ToString());

            testBuffer = new StringBuffer("This is a test buffer");
            String sharedStr = testBuffer.ToString();
            testBuffer.Delete(0, testBuffer.Length - 0); // J2N: Corrected 2nd parameter
            assertEquals("Didn't clone shared buffer", "This is a test buffer", sharedStr
                    );
            assertTrue("Deleted incorrect chars", testBuffer.ToString().Equals(""));
            testBuffer.Append("more stuff");
            assertEquals("Didn't clone shared buffer 2", "This is a test buffer", sharedStr
                    );
            assertEquals("Wrong contents", "more stuff", testBuffer.ToString());
            try
            {
                testBuffer.Delete(-5, 2 - -5); // J2N: Corrected 2nd parameter
            }
            catch (ArgumentOutOfRangeException e)
            {
            }
            assertEquals("Wrong contents 2",
                    "more stuff", testBuffer.ToString());
        }

        ///**
        // * @tests java.lang.StringBuffer#deleteCharAt(int)
        // */
        //[Test]
        //public void Test_deleteCharAtI()
        //{
        //    // Test for method java.lang.StringBuffer
        //    // java.lang.StringBuffer.deleteCharAt(int)
        //    testBuffer.deleteCharAt(3);
        //    assertEquals("Deleted incorrect char",
        //            "Thi is a test buffer", testBuffer.ToString());
        //}

        /**
         * @tests java.lang.StringBuffer#ensureCapacity(int)
         */
        [Test]
        public void Test_ensureCapacityI()
        {
            // Test for method void java.lang.StringBuffer.ensureCapacity(int)
            StringBuffer sb = new StringBuffer(10);

            sb.EnsureCapacity(100);
            assertTrue("Failed to increase capacity", sb.Capacity >= 100);
        }

        /**
         * @tests java.lang.StringBuffer#getChars(int, int, char[], int)
         */
        [Test]
        public void Test_getCharsII_CI()
        {
            // Test for method void java.lang.StringBuffer.getChars(int, int, char
            // [], int)

            char[] buf = new char[10];
            //testBuffer.getChars(4, 8, buf, 2);
            testBuffer.CopyTo(4, buf, 2, 4);
            assertTrue("Returned incorrect chars", new String(buf, 2, 4)
                    .Equals(testBuffer.ToString().Substring(4, 8 - 4))); // J2N: Corrected 2nd parameter

            bool exception = false;
            try
            {
                StringBuffer buf2 = new StringBuffer("");
                //buf2.getChars(0, 0, new char[5], 2);
                buf2.CopyTo(0, new char[5], 2, 0);
            }
            catch (ArgumentOutOfRangeException e)
            {
                exception = true;
            }
            assertTrue("did not expect ArgumentOutOfRangeException", !exception);
        }

        /**
         * @tests java.lang.StringBuffer#insert(int, char[])
         */
        [Test]
        public void Test_insertI_C()
        {
            // Test for method java.lang.StringBuffer
            // java.lang.StringBuffer.insert(int, char [])
            char[] buf = new char[4];
            //"char".getChars(0, 4, buf, 0);
            "char".CopyTo(0, buf, 0, 4);
            testBuffer.Insert(15, buf);
            assertEquals("Insert test failed",
                    "This is a test charbuffer", testBuffer.ToString());

            bool exception = false;
            StringBuffer buf1 = new StringBuffer("abcd");
            try
            {
                buf1.Insert(-1, (char[])null);
            }
            catch (ArgumentOutOfRangeException e)
            {
                exception = true;
            }
            catch (ArgumentNullException e)
            {
            }
            assertTrue("Should throw ArgumentOutOfRangeException", exception);
        }

        /**
         * @tests java.lang.StringBuffer#insert(int, char[], int, int)
         */
        [Test]
        public void Test_insertI_CII()
        {
            // Test for method java.lang.StringBuffer
            // java.lang.StringBuffer.insert(int, char [], int, int)
            char[] c = new char[] { 'n', 'o', 't', ' ' };
            testBuffer.Insert(8, c, 0, 4);
            assertEquals("This is not a test buffer", testBuffer.ToString());

            StringBuffer buf1 = new StringBuffer("abcd");
            try
            {
                buf1.Insert(-1, (char[])null, 0, 0);
                fail("Should throw ArgumentOutOfRangeException");
            }
            catch (ArgumentOutOfRangeException e)
            {
                //expected
            }

            try
            {
                testBuffer.Insert(testBuffer.Length - 1, c, -1, 1);
                fail("Should throw ArgumentOutOfRangeException");
            }
            catch (ArgumentOutOfRangeException e)
            {
                //expected
            }

        }

        /**
         * @tests java.lang.StringBuffer#insert(int, char)
         */
        [Test]
        public void Test_insertIC()
        {
            // Test for method java.lang.StringBuffer
            // java.lang.StringBuffer.insert(int, char)
            testBuffer.Insert(15, 'T');
            assertEquals("Insert test failed",
                    "This is a test Tbuffer", testBuffer.ToString());
        }

        /**
         * @tests java.lang.StringBuffer#insert(int, double)
         */
        [Test]
        public void Test_insertID()
        {
            // Test for method java.lang.StringBuffer
            // java.lang.StringBuffer.insert(int, double)
            testBuffer.Insert(15, double.MaxValue);
            assertTrue("Insert test failed", testBuffer.ToString().Equals(
                    "This is a test " + double.MaxValue + "buffer"));
        }

        /**
         * @tests java.lang.StringBuffer#insert(int, float)
         */
        [Test]
        public void Test_insertIF()
        {
            // Test for method java.lang.StringBuffer
            // java.lang.StringBuffer.insert(int, float)
            testBuffer.Insert(15, float.MaxValue);
            String testBufferString = testBuffer.ToString();
            String expectedResult = "This is a test "
                    + float.MaxValue + "buffer";
            assertTrue("Insert test failed, got: " + "\'" + testBufferString + "\'"
                    + " but wanted: " + "\'" + expectedResult + "\'",
                    testBufferString.Equals(expectedResult));
        }

        /**
         * @tests java.lang.StringBuffer#insert(int, int)
         */
        [Test]
        public void Test_insertII()
        {
            // Test for method java.lang.StringBuffer
            // java.lang.StringBuffer.insert(int, int)
            testBuffer.Insert(15, 100);
            assertEquals("Insert test failed",
                    "This is a test 100buffer", testBuffer.ToString());
        }

        /**
         * @tests java.lang.StringBuffer#insert(int, long)
         */
        [Test]
        public void Test_insertIJ()
        {
            // Test for method java.lang.StringBuffer
            // java.lang.StringBuffer.insert(int, long)
            testBuffer.Insert(15, 88888888888888888L);
            assertEquals("Insert test failed",
                    "This is a test 88888888888888888buffer", testBuffer.ToString());
        }

        /**
         * @tests java.lang.StringBuffer#insert(int, java.lang.Object)
         */
        [Test]
        public void Test_insertILjava_lang_Object()
        {
            // Test for method java.lang.StringBuffer
            // java.lang.StringBuffer.insert(int, java.lang.Object)
            Object obj1 = new Object();
            testBuffer.Insert(15, obj1);
            assertTrue("Insert test failed", testBuffer.ToString().Equals(
                    "This is a test " + obj1.ToString() + "buffer"));
        }

        /**
         * @tests java.lang.StringBuffer#insert(int, java.lang.String)
         */
        [Test]
        public void Test_insertILjava_lang_String()
        {
            // Test for method java.lang.StringBuffer
            // java.lang.StringBuffer.insert(int, java.lang.String)

            testBuffer.Insert(15, "STRING ");
            assertEquals("Insert test failed",
                    "This is a test STRING buffer", testBuffer.ToString());
        }

        /**
         * @tests java.lang.StringBuffer#insert(int, boolean)
         */
        [Test]
        public void Test_insertIZ()
        {
            // Test for method java.lang.StringBuffer
            // java.lang.StringBuffer.insert(int, boolean)
            testBuffer.Insert(15, true);
            assertEquals("Insert test failed",
                    "This is a test Truebuffer", testBuffer.ToString());  // J2N: In .NET, the "False" and "True" are title case
        }

        /**
         * @tests java.lang.StringBuffer#length()
         */
        [Test]
        public void Test_length()
        {
            // Test for method int java.lang.StringBuffer.Length
            assertEquals("Incorrect length returned", 21, testBuffer.Length);
        }

        /**
         * @tests java.lang.StringBuffer#replace(int, int, java.lang.String)
         */
        [Test]
        public void Test_replaceIILjava_lang_String()
        {
            // Test for method java.lang.StringBuffer
            // java.lang.StringBuffer.replace(int, int, java.lang.String)
            testBuffer.Replace(5, 9 - 5, "is a replaced"); // J2N: Corrected 2nd parameter
            assertTrue("Replace failed, wanted: " + "\'"
                    + "This is a replaced test buffer" + "\'" + " but got: " + "\'"
                    + testBuffer.ToString() + "\'", testBuffer.ToString().Equals(
                    "This is a replaced test buffer"));
            assertEquals("insert1", "text", new StringBuffer().Replace(0, 0 - 0, "text") // J2N: Corrected 2nd parameter
                    .ToString());
            assertEquals("insert2", "123text", new StringBuffer("123").Replace(3, 3 - 3, "text") // J2N: Corrected 2nd parameter
                    .ToString());
            assertEquals("insert2", "1text23", new StringBuffer("123").Replace(1, 1 - 1, "text") // J2N: Corrected 2nd parameter
                    .ToString());
        }

        private String writeString(String input)
        {
            StringBuffer result = new StringBuffer();
            result.Append("\"");
            for (int i = 0; i < input.Length; i++)
            {
                result.Append(" 0x" + (input[i]).ToHexString());
            }
            result.Append("\"");
            return result.ToString();
        }

        private void reverseTest(String id, String org, String rev, String back)
        {
            // create non-shared StringBuffer
            StringBuffer sb = new StringBuffer(org);
            sb.Reverse();
            String reversed = sb.ToString();
            assertTrue("reversed surrogate " + id + ": " + writeString(reversed),
                    reversed.Equals(rev));
            // create non-shared StringBuffer
            sb = new StringBuffer(reversed);
            sb.Reverse();
            reversed = sb.ToString();
            assertTrue("reversed surrogate " + id + "a: " + writeString(reversed),
                    reversed.Equals(back));

            // test algorithm when StringBuffer is shared
            sb = new StringBuffer(org);
            String copy = sb.ToString();
            assertEquals(org, copy);
            sb.Reverse();
            reversed = sb.ToString();
            assertTrue("reversed surrogate " + id + ": " + writeString(reversed),
                    reversed.Equals(rev));
            sb = new StringBuffer(reversed);
            copy = sb.ToString();
            assertEquals(rev, copy);
            sb.Reverse();
            reversed = sb.ToString();
            assertTrue("reversed surrogate " + id + "a: " + writeString(reversed),
                    reversed.Equals(back));

        }

        /**
         * @tests java.lang.StringBuffer#reverse()
         */
        [Test]
        public void Test_reverse()
        {
            // Test for method java.lang.StringBuffer
            // java.lang.StringBuffer.reverse()
            String org;
            org = "a";
            reverseTest("0", org, org, org);

            org = "ab";
            reverseTest("1", org, "ba", org);

            org = "abcdef";
            reverseTest("2", org, "fedcba", org);

            org = "abcdefg";
            reverseTest("3", org, "gfedcba", org);

        }

        /**
         * @tests java.lang.StringBuffer#setCharAt(int, char)
         */
        [Test]
        public void Test_setCharAtIC()
        {
            // Test for method void java.lang.StringBuffer.setCharAt(int, char)
            StringBuffer s = new StringBuffer("HelloWorld");
            s[4] = 'Z';
            assertEquals("Returned incorrect char", 'Z', s[4]);
        }

        /**
         * @tests java.lang.StringBuffer#setLength(int)
         */
        [Test]
        public void Test_setLengthI()
        {
            // Test for method void java.lang.StringBuffer.setLength(int)
            testBuffer.Length = (1000);
            assertEquals("Failed to increase length", 1000, testBuffer.Length);
            assertTrue("Increase in length trashed buffer", testBuffer.ToString()
                    .StartsWith("This is a test buffer", StringComparison.Ordinal));
            testBuffer.Length = (2);
            assertEquals("Failed to decrease length", 2, testBuffer.Length);
            assertEquals("Decrease in length failed",
                    "Th", testBuffer.ToString());
        }

        /**
         * @tests java.lang.StringBuffer#substring(int)
         */
        [Test]
        public void Test_substringI()
        {
            // Test for method java.lang.String
            // java.lang.StringBuffer.substring(int)
            assertEquals("Returned incorrect substring", "is a test buffer", testBuffer.ToString(5, testBuffer.Length - 5) // J2N: This overload doesn't exist in .NET, so we are improvising
                    );
        }

        /**
         * @tests java.lang.StringBuffer#substring(int, int)
         */
        [Test]
        public void Test_substringII()
        {
            // Test for method java.lang.String
            // java.lang.StringBuffer.substring(int, int)
            assertEquals("Returned incorrect substring", "is", testBuffer.ToString(5, 7 - 5) // J2N: Corrected 2nd parameter
                    );
        }

        /**
         * @tests java.lang.StringBuffer#toString()
         */
        [Test]
        public void Test_toString()
        {
            // Test for method java.lang.String java.lang.StringBuffer.ToString()
            assertEquals("Incorrect string value returned", "This is a test buffer", testBuffer.ToString()
                    );
        }

        public override void SetUp()
        {
            testBuffer = new StringBuffer("This is a test buffer");
        }
    }
}
