#region Copyright 2010 by Apache Harmony, Licensed under the Apache License, Version 2.0
/*  Licensed to the Apache Software Foundation (ASF) under one or more
 *  contributor license agreements.  See the NOTICE file distributed with
 *  this work for additional information regarding copyright ownership.
 *  The ASF licenses this file to You under the Apache License, Version 2.0
 *  (the "License"); you may not use this file except in compliance with
 *  the License.  You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 *  Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *  limitations under the License.
 */
#endregion

using J2N.Globalization;
using NUnit.Framework;
using System;
using System.Globalization;

namespace J2N.Text
{
    /// <summary>
    /// Additional tests from Apache Harmony, StringBuffer2Test.java
    /// </summary>
    public partial class TestSynchronizedTextBuilder
    {
        public class StringBufferTest2 : TestCase
        {

            SynchronizedTextBuilder testBuffer;

            /**
             * @tests java.lang.SynchronizedTextBuilder#SynchronizedTextBuilder()
             */
            [Test]
            public void Test_Constructor()
            {
                // Test for method java.lang.SynchronizedTextBuilder()
                new SynchronizedTextBuilder();
                assertTrue("Invalid buffer created", true);
            }

            /**
             * @tests java.lang.SynchronizedTextBuilder#SynchronizedTextBuilder(int)
             */
            [Test]
            public void Test_ConstructorI()
            {
                // Test for method java.lang.SynchronizedTextBuilder(int)
                SynchronizedTextBuilder sb = new SynchronizedTextBuilder(8);
                assertEquals("Newly constructed buffer is of incorrect length", 0, sb
                        .Length);
            }

            /**
             * @tests java.lang.SynchronizedTextBuilder#SynchronizedTextBuilder(java.lang.String)
             */
            [Test]
            public void Test_ConstructorLjava_lang_String()
            {
                // Test for method java.lang.SynchronizedTextBuilder(java.lang.String)

                SynchronizedTextBuilder sb = new SynchronizedTextBuilder("HelloWorld");

                assertTrue("Invalid buffer created", sb.Length == 10
                        && (sb.ToString().Equals("HelloWorld")));

                //bool pass = false;
                //try
                //{
                //    new SynchronizedTextBuilder((string)null);
                //}
                //catch (ArgumentNullException e)
                //{
                //    pass = true;
                //}
                //assertTrue("Should throw NullPointerException", pass);

                assertEquals(string.Empty, new SynchronizedTextBuilder((string)null).ToString()); // J2N: Matching .NET behavior - a null in the constructor is a no-op
            }

            /**
             * @tests java.lang.SynchronizedTextBuilder#append(char[])
             */
            [Test]
            public void Test_append_C()
            {
                // Test for method java.lang.SynchronizedTextBuilder
                // java.lang.SynchronizedTextBuilder.Append(char [])
                char[] buf = new char[4];
                //"char".getChars(0, 4, buf, 0);
                "char".CopyTo(0, buf, 0, 4);
                testBuffer.Append(buf);
                assertEquals("Append of char[] failed",
                        "This is a test bufferchar", testBuffer.ToString());
            }

            /**
             * @tests java.lang.SynchronizedTextBuilder#append(char[], int, int)
             */
            [Test]
            public void Test_append_CII()
            {
                // Test for method java.lang.SynchronizedTextBuilder
                // java.lang.SynchronizedTextBuilder.Append(char [], int, int)
                SynchronizedTextBuilder sb = new SynchronizedTextBuilder();
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
             * @tests java.lang.SynchronizedTextBuilder#append(char)
             */
            [Test]
            public void Test_appendC()
            {
                // Test for method java.lang.SynchronizedTextBuilder
                // java.lang.SynchronizedTextBuilder.Append(char)
                SynchronizedTextBuilder sb = new SynchronizedTextBuilder();
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
             * @tests java.lang.SynchronizedTextBuilder#append(double)
             */
            [Test]
            public void Test_appendD()
            {
                using (var context = new CultureContext(CultureInfo.InvariantCulture))
                {

                    // Test for method java.lang.SynchronizedTextBuilder
                    // java.lang.SynchronizedTextBuilder.Append(double)
                    SynchronizedTextBuilder sb = new SynchronizedTextBuilder();
                    sb.Append(double.MaxValue);

                    /*.NET would normally do "1.7976931348623157E+308"*/

                    assertEquals("Buffer is invalid length after append", J2N.Numerics.Double.ToString(double.MaxValue).Length, sb.Length); // J2N: Exact format of string is dependent upon framework implementation, so we are testing whether the append matches the re-generated string
                    assertEquals("Buffer contains invalid characters",
                            "1.7976931348623157E308", sb.ToString()); ;
                }
            }

            /**
             * @tests java.lang.SynchronizedTextBuilder#append(float)
             */
            [Test]
            public void Test_appendF()
            {
                // Test for method java.lang.SynchronizedTextBuilder
                // java.lang.SynchronizedTextBuilder.Append(float)
                SynchronizedTextBuilder sb = new SynchronizedTextBuilder();
                float floatNum = 900.87654F;
                sb.Append(floatNum);
                assertTrue("Buffer is invalid length after append: " + sb.Length, sb
                        .Length == floatNum.ToString().Length);
                assertTrue("Buffer contains invalid characters", sb.ToString().Equals(
                        floatNum.ToString()));
            }

            /**
             * @tests java.lang.SynchronizedTextBuilder#append(int)
             */
            [Test]
            public void Test_appendI()
            {
                // Test for method java.lang.SynchronizedTextBuilder
                // java.lang.SynchronizedTextBuilder.Append(int)
                SynchronizedTextBuilder sb = new SynchronizedTextBuilder();
                sb.Append(9000);
                assertEquals("Buffer is invalid length after append", 4, sb.Length);
                sb.Append(1000);
                assertEquals("Buffer is invalid length after append", 8, sb.Length);
                assertEquals("Buffer contains invalid characters",
                        "90001000", sb.ToString());
            }

            /**
             * @tests java.lang.SynchronizedTextBuilder#append(long)
             */
            [Test]
            public void Test_appendJ()
            {
                // Test for method java.lang.SynchronizedTextBuilder
                // java.lang.SynchronizedTextBuilder.Append(long)

                SynchronizedTextBuilder sb = new SynchronizedTextBuilder();
                long t = 927654321098L;
                sb.Append(t);
                assertEquals("Buffer is of invlaid length", 12, sb.Length);
                assertEquals("Buffer contains invalid characters",
                        "927654321098", sb.ToString());
            }

            /**
             * @tests java.lang.SynchronizedTextBuilder#append(java.lang.Object)
             */
            [Test]
            public void Test_appendLjava_lang_Object()
            {
                // Test for method java.lang.SynchronizedTextBuilder
                // java.lang.SynchronizedTextBuilder.Append(java.lang.Object)
                SynchronizedTextBuilder sb = new SynchronizedTextBuilder();
                Object obj1 = new Object();
                Object obj2 = new Object();
                sb.Append(obj1);
                sb.Append(obj2);
                assertTrue("Buffer contains invalid characters", sb.ToString().Equals(
                        obj1.ToString() + obj2.ToString()));
            }

            /**
             * @tests java.lang.SynchronizedTextBuilder#append(java.lang.String)
             */
            [Test]
            public void Test_appendLjava_lang_String()
            {
                // Test for method java.lang.SynchronizedTextBuilder
                // java.lang.SynchronizedTextBuilder.Append(java.lang.String)
                SynchronizedTextBuilder sb = new SynchronizedTextBuilder();
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
             * @tests java.lang.SynchronizedTextBuilder#append(boolean)
             */
            [Test]
            public void Test_appendZ()
            {
                // Test for method java.lang.SynchronizedTextBuilder
                // java.lang.SynchronizedTextBuilder.Append(boolean)
                SynchronizedTextBuilder sb = new SynchronizedTextBuilder();
                sb.Append(false);
                assertEquals("Buffer is invalid length after append", 5, sb.Length);
                sb.Append(true);
                assertEquals("Buffer is invalid length after append", 9, sb.Length);
                assertTrue("Buffer is invalid length after append", (sb.ToString()
                        .Equals("falsetrue"))); // J2N: In .NET, the "False" and "True" are title case, but J2N defaults to lower case
            }

            /**
             * @tests java.lang.SynchronizedTextBuilder#capacity()
             */
            [Test]
            public void Test_capacity()
            {
                // Test for method int java.lang.SynchronizedTextBuilder.Capacity
                SynchronizedTextBuilder sb = new SynchronizedTextBuilder(10);
                assertEquals("Returned incorrect capacity", 10, sb.Capacity);
                sb.EnsureCapacity(100);
                assertTrue("Returned incorrect capacity", sb.Capacity >= 100);
            }

            /**
             * @tests java.lang.SynchronizedTextBuilder#charAt(int)
             */
            [Test]
            public void Test_charAtI()
            {
                // Test for method char java.lang.SynchronizedTextBuilder.charAt(int)
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
             * @tests java.lang.SynchronizedTextBuilder#delete(int, int)
             */
            [Test]
            public void Test_deleteII()
            {
                // Test for method java.lang.SynchronizedTextBuilder
                // java.lang.SynchronizedTextBuilder.delete(int, int)
                testBuffer.Delete(7, 7 - 7); // J2N: Corrected 2nd parameter
                assertEquals("Deleted chars when start == end", "This is a test buffer", testBuffer.ToString()
                        );
                testBuffer.Delete(4, 14 - 4); // J2N: Corrected 2nd parameter
                assertEquals("Deleted incorrect chars",
                        "This buffer", testBuffer.ToString());

                testBuffer = new SynchronizedTextBuilder("This is a test buffer");
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

            /**
             * @tests java.lang.SynchronizedTextBuilder#deleteCharAt(int)
             */
            [Test]
            public void Test_deleteCharAtI()
            {
                // Test for method java.lang.SynchronizedTextBuilder
                // java.lang.SynchronizedTextBuilder.deleteCharAt(int)
                testBuffer.RemoveAt(3);
                assertEquals("Deleted incorrect char",
                        "Thi is a test buffer", testBuffer.ToString());
            }

            /**
             * @tests java.lang.SynchronizedTextBuilder#ensureCapacity(int)
             */
            [Test]
            public void Test_ensureCapacityI()
            {
                // Test for method void java.lang.SynchronizedTextBuilder.ensureCapacity(int)
                SynchronizedTextBuilder sb = new SynchronizedTextBuilder(10);

                sb.EnsureCapacity(100);
                assertTrue("Failed to increase capacity", sb.Capacity >= 100);
            }

            /**
             * @tests java.lang.SynchronizedTextBuilder#getChars(int, int, char[], int)
             */
            [Test]
            public void Test_getCharsII_CI()
            {
                // Test for method void java.lang.SynchronizedTextBuilder.getChars(int, int, char
                // [], int)

                char[] buf = new char[10];
                //testBuffer.getChars(4, 8, buf, 2);
                testBuffer.CopyTo(4, buf, 2, 4);
                assertTrue("Returned incorrect chars", new String(buf, 2, 4)
                        .Equals(testBuffer.ToString().Substring(4, 8 - 4))); // J2N: Corrected 2nd parameter

                bool exception = false;
                try
                {
                    SynchronizedTextBuilder buf2 = new SynchronizedTextBuilder("");
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
             * @tests java.lang.SynchronizedTextBuilder#insert(int, char[])
             */
            [Test]
            public void Test_insertI_C()
            {
                // Test for method java.lang.SynchronizedTextBuilder
                // java.lang.SynchronizedTextBuilder.insert(int, char [])
                char[] buf = new char[4];
                //"char".getChars(0, 4, buf, 0);
                "char".CopyTo(0, buf, 0, 4);
                testBuffer.Insert(15, buf);
                assertEquals("Insert test failed",
                        "This is a test charbuffer", testBuffer.ToString());

                bool exception = false;
                SynchronizedTextBuilder buf1 = new SynchronizedTextBuilder("abcd");
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
             * @tests java.lang.SynchronizedTextBuilder#insert(int, char[], int, int)
             */
            [Test]
            public void Test_insertI_CII()
            {
                // Test for method java.lang.SynchronizedTextBuilder
                // java.lang.SynchronizedTextBuilder.insert(int, char [], int, int)
                char[] c = new char[] { 'n', 'o', 't', ' ' };
                testBuffer.Insert(8, c, 0, 4);
                assertEquals("This is not a test buffer", testBuffer.ToString());

                SynchronizedTextBuilder buf1 = new SynchronizedTextBuilder("abcd");
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
             * @tests java.lang.SynchronizedTextBuilder#insert(int, char)
             */
            [Test]
            public void Test_insertIC()
            {
                // Test for method java.lang.SynchronizedTextBuilder
                // java.lang.SynchronizedTextBuilder.insert(int, char)
                testBuffer.Insert(15, 'T');
                assertEquals("Insert test failed",
                        "This is a test Tbuffer", testBuffer.ToString());
            }

            /**
             * @tests java.lang.SynchronizedTextBuilder#insert(int, double)
             */
            [Test]
            public void Test_insertID()
            {
                // Test for method java.lang.SynchronizedTextBuilder
                // java.lang.SynchronizedTextBuilder.insert(int, double)
                testBuffer.Insert(15, double.MaxValue);
                assertTrue("Insert test failed", testBuffer.ToString().Equals(
                        $"This is a test {J2N.Numerics.Double.ToString(double.MaxValue)}buffer"));
            }

            /**
             * @tests java.lang.SynchronizedTextBuilder#insert(int, float)
             */
            [Test]
            public void Test_insertIF()
            {
                // Test for method java.lang.SynchronizedTextBuilder
                // java.lang.SynchronizedTextBuilder.insert(int, float)
                testBuffer.Insert(15, float.MaxValue);
                String testBufferString = testBuffer.ToString();
                String expectedResult = $"This is a test {J2N.Numerics.Single.ToString(float.MaxValue)}buffer";
                assertTrue("Insert test failed, got: " + "\'" + testBufferString + "\'"
                        + " but wanted: " + "\'" + expectedResult + "\'",
                        testBufferString.Equals(expectedResult));
            }

            /**
             * @tests java.lang.SynchronizedTextBuilder#insert(int, int)
             */
            [Test]
            public void Test_insertII()
            {
                // Test for method java.lang.SynchronizedTextBuilder
                // java.lang.SynchronizedTextBuilder.insert(int, int)
                testBuffer.Insert(15, 100);
                assertEquals("Insert test failed",
                        "This is a test 100buffer", testBuffer.ToString());
            }

            /**
             * @tests java.lang.SynchronizedTextBuilder#insert(int, long)
             */
            [Test]
            public void Test_insertIJ()
            {
                // Test for method java.lang.SynchronizedTextBuilder
                // java.lang.SynchronizedTextBuilder.insert(int, long)
                testBuffer.Insert(15, 88888888888888888L);
                assertEquals("Insert test failed",
                        "This is a test 88888888888888888buffer", testBuffer.ToString());
            }

            /**
             * @tests java.lang.SynchronizedTextBuilder#insert(int, java.lang.Object)
             */
            [Test]
            public void Test_insertILjava_lang_Object()
            {
                // Test for method java.lang.SynchronizedTextBuilder
                // java.lang.SynchronizedTextBuilder.insert(int, java.lang.Object)
                Object obj1 = new Object();
                testBuffer.Insert(15, obj1);
                assertTrue("Insert test failed", testBuffer.ToString().Equals(
                        "This is a test " + obj1.ToString() + "buffer"));
            }

            /**
             * @tests java.lang.SynchronizedTextBuilder#insert(int, java.lang.String)
             */
            [Test]
            public void Test_insertILjava_lang_String()
            {
                // Test for method java.lang.SynchronizedTextBuilder
                // java.lang.SynchronizedTextBuilder.insert(int, java.lang.String)

                testBuffer.Insert(15, "STRING ");
                assertEquals("Insert test failed",
                        "This is a test STRING buffer", testBuffer.ToString());
            }

            /**
             * @tests java.lang.SynchronizedTextBuilder#insert(int, boolean)
             */
            [Test]
            public void Test_insertIZ()
            {
                // Test for method java.lang.SynchronizedTextBuilder
                // java.lang.SynchronizedTextBuilder.insert(int, boolean)
                testBuffer.Insert(15, true);
                assertEquals("Insert test failed",
                        "This is a test truebuffer", testBuffer.ToString());  // J2N: In .NET, the "False" and "True" are title case, but J2N's default overload is lower case
            }

            /**
             * @tests java.lang.SynchronizedTextBuilder#length()
             */
            [Test]
            public void Test_length()
            {
                // Test for method int java.lang.SynchronizedTextBuilder.Length
                assertEquals("Incorrect length returned", 21, testBuffer.Length);
            }

            /**
             * @tests java.lang.SynchronizedTextBuilder#replace(int, int, java.lang.String)
             */
            [Test]
            public void Test_replaceIILjava_lang_String()
            {
                // Test for method java.lang.SynchronizedTextBuilder
                // java.lang.SynchronizedTextBuilder.replace(int, int, java.lang.String)
                testBuffer.Replace(5, 9 - 5, "is a replaced"); // J2N: Corrected 2nd parameter
                assertTrue("Replace failed, wanted: " + "\'"
                        + "This is a replaced test buffer" + "\'" + " but got: " + "\'"
                        + testBuffer.ToString() + "\'", testBuffer.ToString().Equals(
                        "This is a replaced test buffer"));
                assertEquals("insert1", "text", new SynchronizedTextBuilder().Replace(0, 0 - 0, "text") // J2N: Corrected 2nd parameter
                        .ToString());
                assertEquals("insert2", "123text", new SynchronizedTextBuilder("123").Replace(3, 3 - 3, "text") // J2N: Corrected 2nd parameter
                        .ToString());
                assertEquals("insert2", "1text23", new SynchronizedTextBuilder("123").Replace(1, 1 - 1, "text") // J2N: Corrected 2nd parameter
                        .ToString());
            }

            private String writeString(String input)
            {
                SynchronizedTextBuilder result = new SynchronizedTextBuilder();
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
                // create non-shared SynchronizedTextBuilder
                SynchronizedTextBuilder sb = new SynchronizedTextBuilder(org);
                sb.Reverse();
                String reversed = sb.ToString();
                assertTrue("reversed surrogate " + id + ": " + writeString(reversed),
                        reversed.Equals(rev));
                // create non-shared SynchronizedTextBuilder
                sb = new SynchronizedTextBuilder(reversed);
                sb.Reverse();
                reversed = sb.ToString();
                assertTrue("reversed surrogate " + id + "a: " + writeString(reversed),
                        reversed.Equals(back));

                // test algorithm when SynchronizedTextBuilder is shared
                sb = new SynchronizedTextBuilder(org);
                String copy = sb.ToString();
                assertEquals(org, copy);
                sb.Reverse();
                reversed = sb.ToString();
                assertTrue("reversed surrogate " + id + ": " + writeString(reversed),
                        reversed.Equals(rev));
                sb = new SynchronizedTextBuilder(reversed);
                copy = sb.ToString();
                assertEquals(rev, copy);
                sb.Reverse();
                reversed = sb.ToString();
                assertTrue("reversed surrogate " + id + "a: " + writeString(reversed),
                        reversed.Equals(back));

            }

            /**
             * @tests java.lang.SynchronizedTextBuilder#reverse()
             */
            [Test]
            public void Test_reverse()
            {
                // Test for method java.lang.SynchronizedTextBuilder
                // java.lang.SynchronizedTextBuilder.reverse()
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
             * @tests java.lang.SynchronizedTextBuilder#setCharAt(int, char)
             */
            [Test]
            public void Test_setCharAtIC()
            {
                // Test for method void java.lang.SynchronizedTextBuilder.setCharAt(int, char)
                SynchronizedTextBuilder s = new SynchronizedTextBuilder("HelloWorld");
                s[4] = 'Z';
                assertEquals("Returned incorrect char", 'Z', s[4]);
            }

            /**
             * @tests java.lang.SynchronizedTextBuilder#setLength(int)
             */
            [Test]
            public void Test_setLengthI()
            {
                // Test for method void java.lang.SynchronizedTextBuilder.setLength(int)
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
             * @tests java.lang.SynchronizedTextBuilder#substring(int)
             */
            [Test]
            public void Test_substringI()
            {
                // Test for method java.lang.String
                // java.lang.SynchronizedTextBuilder.substring(int)
                assertEquals("Returned incorrect substring", "is a test buffer", testBuffer.ToString(5, testBuffer.Length - 5) // J2N: This overload doesn't exist in .NET, so we are improvising
                        );
            }

            /**
             * @tests java.lang.SynchronizedTextBuilder#substring(int, int)
             */
            [Test]
            public void Test_substringII()
            {
                // Test for method java.lang.String
                // java.lang.SynchronizedTextBuilder.substring(int, int)
                assertEquals("Returned incorrect substring", "is", testBuffer.ToString(5, 7 - 5) // J2N: Corrected 2nd parameter
                        );
            }

            /**
             * @tests java.lang.SynchronizedTextBuilder#toString()
             */
            [Test]
            public void Test_toString()
            {
                // Test for method java.lang.String java.lang.SynchronizedTextBuilder.ToString()
                assertEquals("Incorrect string value returned", "This is a test buffer", testBuffer.ToString()
                        );
            }

            public override void SetUp()
            {
                testBuffer = new SynchronizedTextBuilder("This is a test buffer");
            }

        }
    }
}
