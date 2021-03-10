using J2N.Globalization;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace J2N
{
    public class TestPropertyExtensions : TestCase
    {
        private IDictionary<string, string> tProps;

        [Test]
        public void Test_loadLjava_io_InputStream_NPE()
        {
            var p = new Dictionary<string, string>();
            try
            {
                p.LoadProperties((Stream)null);
                fail("should throw ArgumentNullException");
            }
            catch (ArgumentNullException e)
            {
                // Expected
            }
        }

        /**
         * @tests java.util.Properties#getProperty(java.lang.String)
         */
        [Test]
        public void Test_getPropertyLjava_lang_String()
        {
            assertEquals("Did not retrieve property", "this is a test property",
                    tProps.GetProperty("test.prop"));
        }

        /**
         * @tests java.util.Properties#getProperty(java.lang.String,
         *        java.lang.String)
         */
        [Test]
        public void Test_getPropertyLjava_lang_StringLjava_lang_String()
        {
            assertEquals("Did not retrieve property", "this is a test property",
                    tProps.GetProperty("test.prop", "Blarg"));
            assertEquals("Did not return default value", "Gabba", tProps
                    .GetProperty("notInThere.prop", "Gabba"));
        }

        /**
         * @tests java.util.Properties#getProperty(java.lang.String)
         */
        [Test]
        public void Test_getPropertyLjava_lang_String2()
        {
            // regression test for HARMONY-3518
            var props = new Dictionary<string, string>();
            assertNull(props.GetProperty("key"));
        }

        /**
         * @tests java.util.Properties#getProperty(java.lang.String,
         *        java.lang.String)
         */
        [Test]
        public void Test_getPropertyLjava_lang_StringLjava_lang_String2()
        {
            // regression test for HARMONY-3518
            var props = new Dictionary<string, string>();
            assertEquals("defaultValue", props.GetProperty("key", "defaultValue"));
        }


        [Test]
        public void Test_GetPropertyAsBoolean_String()
        {
            assertEquals(false, tProps.GetPropertyAsBoolean("bool.prop.false"));
            assertEquals(true, tProps.GetPropertyAsBoolean("bool.prop.true"));
            assertEquals(false, tProps.GetPropertyAsBoolean("bool.prop.bogus"));
            assertEquals(false, tProps.GetPropertyAsBoolean("bool.prop.unparsable"));
        }

        [Test]
        public void Test_GetPropertyAsBoolean_String_String()
        {
            assertEquals(false, tProps.GetPropertyAsBoolean("bool.prop.false", true));
            assertEquals(true, tProps.GetPropertyAsBoolean("bool.prop.true", false));
            assertEquals(true, tProps.GetPropertyAsBoolean("bool.prop.bogus", true));
            assertEquals(true, tProps.GetPropertyAsBoolean("bool.prop.unparsable", true));
        }

        [Test]
        public void Test_GetPropertyAsInt32_String_Ambient_en_US()
        {
            using (var context = new CultureContext("en-US"))
            {
                assertEquals(int.MaxValue, tProps.GetPropertyAsInt32("int32.prop.max"));
                assertEquals(int.MinValue, tProps.GetPropertyAsInt32("int32.prop.min"));
                assertEquals(-1, tProps.GetPropertyAsInt32("int32.prop.negone"));
                assertEquals(0, tProps.GetPropertyAsInt32("int32.prop.bogus"));
                assertEquals(0, tProps.GetPropertyAsInt32("int32.prop.unparsable"));
            }
        }

        [Test]
        public void Test_GetPropertyAsInt32_String_Ambient_Neg_Q()
        {
            var newCulture = (CultureInfo)CultureInfo.InvariantCulture.Clone();
            newCulture.NumberFormat.NegativeSign = "Q";

            using (var context = new CultureContext(newCulture))
            {
                assertEquals(int.MaxValue, tProps.GetPropertyAsInt32("int32.prop.max"));
                assertEquals(0, tProps.GetPropertyAsInt32("int32.prop.min"));
                assertEquals(0, tProps.GetPropertyAsInt32("int32.prop.negone"));
                assertEquals(0, tProps.GetPropertyAsInt32("int32.prop.bogus"));
                assertEquals(-456, tProps.GetPropertyAsInt32("int32.prop.unparsable"));
            }
        }

        [Test]
        public void Test_GetPropertyAsInt32_String_Int32_Ambient_en_US()
        {
            using (var context = new CultureContext("en-US"))
            {
                assertEquals(int.MaxValue, tProps.GetPropertyAsInt32("int32.prop.max", 5));
                assertEquals(int.MinValue, tProps.GetPropertyAsInt32("int32.prop.min", 5));
                assertEquals(-1, tProps.GetPropertyAsInt32("int32.prop.negone", 5));
                assertEquals(5, tProps.GetPropertyAsInt32("int32.prop.bogus", 5));
                assertEquals(5, tProps.GetPropertyAsInt32("int32.prop.unparsable", 5));
            }
        }

        [Test]
        public void Test_GetPropertyAsInt32_String_Int32_Ambient_Neg_Q()
        {
            var newCulture = (CultureInfo)CultureInfo.InvariantCulture.Clone();
            newCulture.NumberFormat.NegativeSign = "Q";

            using (var context = new CultureContext(newCulture))
            {
                assertEquals(int.MaxValue, tProps.GetPropertyAsInt32("int32.prop.max", 5));
                assertEquals(5, tProps.GetPropertyAsInt32("int32.prop.min", 5));
                assertEquals(5, tProps.GetPropertyAsInt32("int32.prop.negone", 5));
                assertEquals(5, tProps.GetPropertyAsInt32("int32.prop.bogus", 5));
                assertEquals(-456, tProps.GetPropertyAsInt32("int32.prop.unparsable", 5));
            }
        }

        [Test]
        public void Test_GetPropertyAsInt32_IFormatProvider_String_en_US()
        {
            var formatProvider = new CultureInfo("en-US");

            assertEquals(int.MaxValue, tProps.GetPropertyAsInt32(formatProvider, "int32.prop.max"));
            assertEquals(int.MinValue, tProps.GetPropertyAsInt32(formatProvider, "int32.prop.min"));
            assertEquals(-1, tProps.GetPropertyAsInt32(formatProvider, "int32.prop.negone"));
            assertEquals(0, tProps.GetPropertyAsInt32(formatProvider, "int32.prop.bogus"));
            assertEquals(0, tProps.GetPropertyAsInt32(formatProvider, "int32.prop.unparsable"));
        }

        [Test]
        public void Test_GetPropertyAsInt32_IFormatProvider_String_Neg_Q()
        {
            var newCulture = (CultureInfo)CultureInfo.InvariantCulture.Clone();
            newCulture.NumberFormat.NegativeSign = "Q";

            assertEquals(int.MaxValue, tProps.GetPropertyAsInt32(newCulture, "int32.prop.max"));
            assertEquals(0, tProps.GetPropertyAsInt32(newCulture, "int32.prop.min"));
            assertEquals(0, tProps.GetPropertyAsInt32(newCulture, "int32.prop.negone"));
            assertEquals(0, tProps.GetPropertyAsInt32(newCulture, "int32.prop.bogus"));
            assertEquals(-456, tProps.GetPropertyAsInt32(newCulture, "int32.prop.unparsable"));
        }

        [Test]
        public void Test_GetPropertyAsInt32_IFormatProvider_String_Int32_en_US()
        {
            var formatProvider = new CultureInfo("fr-FR");

            assertEquals(int.MaxValue, tProps.GetPropertyAsInt32(formatProvider, "int32.prop.max", 5));
            assertEquals(int.MinValue, tProps.GetPropertyAsInt32(formatProvider, "int32.prop.min", 5));
            assertEquals(-1, tProps.GetPropertyAsInt32(formatProvider, "int32.prop.negone", 5));
            assertEquals(5, tProps.GetPropertyAsInt32(formatProvider, "int32.prop.bogus", 5));
            assertEquals(5, tProps.GetPropertyAsInt32(formatProvider, "int32.prop.unparsable", 5));
        }

        [Test]
        public void Test_GetPropertyAsInt32_IFormatProvider_String_Int32_Neg_Q()
        {
            var newCulture = (CultureInfo)CultureInfo.InvariantCulture.Clone();
            newCulture.NumberFormat.NegativeSign = "Q";

            assertEquals(int.MaxValue, tProps.GetPropertyAsInt32(newCulture, "int32.prop.max", 5));
            assertEquals(5, tProps.GetPropertyAsInt32(newCulture, "int32.prop.min", 5));
            assertEquals(5, tProps.GetPropertyAsInt32(newCulture, "int32.prop.negone", 5));
            assertEquals(5, tProps.GetPropertyAsInt32(newCulture, "int32.prop.bogus", 5));
            assertEquals(-456, tProps.GetPropertyAsInt32(newCulture, "int32.prop.unparsable", 5));
        }


        [Test]
        public void Test_loadLSystem_IO_Stream_ArgumentNullException()
        {
            Dictionary<string, string> p = new Dictionary<string, string>();
            try
            {
                p.LoadProperties((Stream)null);
                fail("should throw ArgumentNullException");
            }
            catch (ArgumentNullException e)
            {
                // Expected
            }
        }

        /**
         * @tests java.util.Properties#load(java.io.InputStream)
         */
        [Test]
        public void Test_loadLSystem_IO_Stream()
        {
            Dictionary<string, string> prop = new Dictionary<string, string>();
            using (Stream @is = new MemoryStream(writeProperties()))
            {
                prop.LoadProperties(@is);
            }
            assertEquals("Failed to load correct properties", "harmony.tests", prop.get("test.pkg"));
            assertNull("Load failed to parse incorrectly", prop
                    .get("commented.entry"));

            prop = new Dictionary<string, string>();
            prop.LoadProperties(new MemoryStream("=".getBytes()));
            assertEquals("Failed to add empty key", "", prop.get(""));

            prop = new Dictionary<string, string>();
            prop.LoadProperties(new MemoryStream(" = ".getBytes()));
            assertEquals("Failed to add empty key2", "", prop.get(""));

            prop = new Dictionary<string, string>();
            prop.LoadProperties(new MemoryStream(" a= b".getBytes()));
            assertEquals("Failed to ignore whitespace", "b", prop.get("a"));

            prop = new Dictionary<string, string>();
            prop.LoadProperties(new MemoryStream(" a b".getBytes()));
            assertEquals("Failed to interpret whitespace as =", "b", prop.get("a"));

            prop = new Dictionary<string, string>();
            prop.LoadProperties(new MemoryStream("#comment\na=value"
                    .getBytes("UTF-8")));
            assertEquals("value", prop.get("a"));

            prop = new Dictionary<string, string>();
            prop.LoadProperties(new MemoryStream("#\u008d\u00d2\na=\u008d\u00d3"
                    .getBytes("ISO-8859-1")));
            assertEquals("Failed to parse chars >= 0x80", "\u008d\u00d3", prop
                    .get("a"));

            prop = new Dictionary<string, string>();
            prop.LoadProperties(new MemoryStream(
                    "#properties file\r\nfred=1\r\n#last comment"
                            .getBytes("ISO-8859-1")));
            assertEquals("Failed to load when last line contains a comment", "1",
                    prop.get("fred"));

            // Regression tests for HARMONY-5414
            prop = new Dictionary<string, string>();
            prop.LoadProperties(new MemoryStream("a=\\u1234z".getBytes()));

            prop = new Dictionary<string, string>();
            try
            {
                prop.LoadProperties(new MemoryStream("a=\\u123".getBytes()));
                fail("should throw ArgumentException");
            }
            catch (ArgumentException e)
            {
                // Expected
            }

            prop = new Dictionary<string, string>();
            try
            {
                prop.LoadProperties(new MemoryStream("a=\\u123z".getBytes()));
                fail("should throw ArgumentException");
            }
            catch (ArgumentException /*expected*/)
            {
                // Expected
            }

            prop = new Dictionary<string, string>();
            Dictionary<string, string> expected = new Dictionary<string, string>();
            expected["a"] = "\u0000";
            prop.LoadProperties(new MemoryStream("a=\\".getBytes()));
            assertEquals("Failed to read trailing slash value", expected, prop);

            prop = new Dictionary<string, string>();
            expected = new Dictionary<string, string>();
            expected["a"] = "\u1234\u0000";
            prop.LoadProperties(new MemoryStream("a=\\u1234\\".getBytes()));
            assertEquals("Failed to read trailing slash value #2", expected, prop);

            prop = new Dictionary<string, string>();
            expected = new Dictionary<string, string>();
            expected["a"] = "q";
            prop.LoadProperties(new MemoryStream("a=\\q".getBytes()));
            assertEquals("Failed to read slash value #3", expected, prop);
        }

        /**
         * @tests java.util.Properties#load(java.io.InputStream)
         */
        [Test]
        public void Test_loadLSystem_IO_Stream_Special()
        {
            // Test for method void java.util.Properties.load(java.io.InputStream)
            Dictionary<string, string> prop = null;
            prop = new Dictionary<string, string>();
            prop.LoadProperties(new MemoryStream("=".getBytes()));
            assertTrue("Failed to add empty key", prop.get("").Equals("", StringComparison.Ordinal));

            prop = new Dictionary<string, string>();
            prop.LoadProperties(new MemoryStream("=\r\n".getBytes()));
            assertTrue("Failed to add empty key", prop.get("").Equals("", StringComparison.Ordinal));

            prop = new Dictionary<string, string>();
            prop.LoadProperties(new MemoryStream("=\n\r".getBytes()));
            assertTrue("Failed to add empty key", prop.get("").Equals("", StringComparison.Ordinal));
        }

        /**
         * @tests java.util.Properties#load(java.io.InputStream)
         */
        [Test]
        public void Test_loadLSystem_IO_Stream_subtest0()
        {
            Dictionary<string, string> props = new Dictionary<string, string>();
            using (Stream input = GetType().getResourceAsStream("hyts_PropertiesTest.properties"))
                props.LoadProperties(input);

            assertEquals("1", "\n \t \f", props.getProperty(" \r"));
            assertEquals("2", "a", props.getProperty("a"));
            assertEquals("3", "bb as,dn   ", props.getProperty("b"));
            assertEquals("4", ":: cu", props.getProperty("c\r \t\nu"));
            assertEquals("5", "bu", props.getProperty("bu"));
            assertEquals("6", "d\r\ne=e", props.getProperty("d"));
            assertEquals("7", "fff", props.getProperty("f"));
            assertEquals("8", "g", props.getProperty("g"));
            assertEquals("9", "", props.getProperty("h h"));
            assertEquals("10", "i=i", props.getProperty(" "));
            assertEquals("11", "   j", props.getProperty("j"));
            assertEquals("12", "   c", props.getProperty("space"));
            assertEquals("13", "\\", props.getProperty("dblbackslash"));
        }

        /**
     * @tests java.util.Properties#store(java.io.OutputStream, java.lang.String)
     */
        [Test]
        public void Test_storeLSystem_IO_StreamLSystem_String()
        {
            Dictionary<string, string> myProps = new Dictionary<string, string>();
            myProps["Property A"] = " aye\\\f\t\n\r\b";
            myProps["Property B"] = "b ee#!=:";
            myProps["Property C"] = "see";

            MemoryStream @out = new MemoryStream();
            myProps.SaveProperties(@out, "A Header");
            @out.Dispose();

            MemoryStream @in = new MemoryStream(@out.ToArray());
            Dictionary<string, string> myProps2 = new Dictionary<string, string>();
            myProps2.LoadProperties(@in);
            @in.Dispose();

            using (var e = myProps.Keys.GetEnumerator())
            {
                String nextKey;
                while (e.MoveNext())
                {
                    nextKey = e.Current;
                    assertTrue("Stored property list not equal to original", myProps2
                        .getProperty(nextKey).Equals(myProps.getProperty(nextKey), StringComparison.Ordinal));
                }
            }
        }

        /**
        * if loading from single line like "hello" without "\n\r" neither "=", it
        * should be same as loading from "hello="
        */
        [Test]
        public void TestLoadSingleLine()
        {
            Dictionary<string, string> props = new Dictionary<string, string>();
            Stream sr = new MemoryStream("hello".getBytes());
            props.LoadProperties(sr);
            assertEquals(1, props.Count);
        }

        private String comment1 = "comment1";

        private String comment2 = "comment2";

        private void validateOutput(String[] expectStrings, byte[] output)
        {
            MemoryStream bais = new MemoryStream(output);
            TextReader br = new StreamReader(bais,
                    Encoding.GetEncoding("ISO-8859-1"));
            foreach (String expectString in expectStrings)
            {
                assertEquals(expectString, br.ReadLine());
            }
            br.ReadLine();
            assertNull(br.ReadLine());
            br.Dispose();
        }

        [Test]
        public void TestStore_scenario0()
        {
            MemoryStream baos = new MemoryStream();
            Dictionary<string, string> props = new Dictionary<string, string>();
            props.SaveProperties(baos, comment1 + '\r' + comment2);
            validateOutput(new String[] { "#comment1", "#comment2" },
                    baos.ToArray());
            baos.Dispose();
        }

        [Test]
        public void TestStore_scenario1()
        {
            MemoryStream baos = new MemoryStream();
            Dictionary<string, string> props = new Dictionary<string, string>();
            props.SaveProperties(baos, comment1 + '\n' + comment2);
            validateOutput(new String[] { "#comment1", "#comment2" },
                    baos.ToArray());
            baos.Dispose();
        }

        [Test]
        public void TestStore_scenario2()
        {
            MemoryStream baos = new MemoryStream();
            Dictionary<string, string> props = new Dictionary<string, string>();
            props.SaveProperties(baos, comment1 + '\r' + '\n' + comment2);
            validateOutput(new String[] { "#comment1", "#comment2" },
                    baos.ToArray());
            baos.Dispose();
        }

        [Test]
        public void TestStore_scenario3()
        {
            MemoryStream baos = new MemoryStream();
            Dictionary<string, string> props = new Dictionary<string, string>();
            props.SaveProperties(baos, comment1 + '\n' + '\r' + comment2);
            validateOutput(new String[] { "#comment1", "#", "#comment2" },
                    baos.ToArray());
            baos.Dispose();
        }

        [Test]
        public void TestStore_scenario4()
        {
            MemoryStream baos = new MemoryStream();
            Dictionary<string, string> props = new Dictionary<string, string>();
            props.SaveProperties(baos, comment1 + '\r' + '#' + comment2);
            validateOutput(new String[] { "#comment1", "#comment2" },
                    baos.ToArray());
            baos.Dispose();
        }

        [Test]
        public void TestStore_scenario5()
        {
            MemoryStream baos = new MemoryStream();
            Dictionary<string, string> props = new Dictionary<string, string>();
            props.SaveProperties(baos, comment1 + '\r' + '!' + comment2);
            validateOutput(new String[] { "#comment1", "!comment2" },
                    baos.ToArray());
            baos.Dispose();
        }

        [Test]
        public void TestStore_scenario6()
        {
            MemoryStream baos = new MemoryStream();
            Dictionary<string, string> props = new Dictionary<string, string>();
            props.SaveProperties(baos, comment1 + '\n' + '#' + comment2);
            validateOutput(new String[] { "#comment1", "#comment2" },
                    baos.ToArray());
            baos.Dispose();
        }

        [Test]
        public void TestStore_scenario7()
        {
            MemoryStream baos = new MemoryStream();
            Dictionary<string, string> props = new Dictionary<string, string>();
            props.SaveProperties(baos, comment1 + '\n' + '!' + comment2);
            validateOutput(new String[] { "#comment1", "!comment2" },
                    baos.ToArray());
            baos.Dispose();
        }

        [Test]
        public void TestStore_scenario8()
        {
            MemoryStream baos = new MemoryStream();
            Dictionary<string, string> props = new Dictionary<string, string>();
            props.SaveProperties(baos, comment1 + '\r' + '\n' + '#' + comment2);
            validateOutput(new String[] { "#comment1", "#comment2" },
                    baos.ToArray());
            baos.Dispose();
        }

        [Test]
        public void TestStore_scenario9()
        {
            MemoryStream baos = new MemoryStream();
            Dictionary<string, string> props = new Dictionary<string, string>();
            props.SaveProperties(baos, comment1 + '\n' + '\r' + '#' + comment2);
            validateOutput(new String[] { "#comment1", "#", "#comment2" },
                    baos.ToArray());
            baos.Dispose();
        }

        [Test]
        public void TestStore_scenario10()
        {
            MemoryStream baos = new MemoryStream();
            Dictionary<string, string> props = new Dictionary<string, string>();
            props.SaveProperties(baos, comment1 + '\r' + '\n' + '!' + comment2);
            validateOutput(new String[] { "#comment1", "!comment2" },
                    baos.ToArray());
            baos.Dispose();
        }

        [Test]
        public void TestStore_scenario11()
        {
            MemoryStream baos = new MemoryStream();
            Dictionary<string, string> props = new Dictionary<string, string>();
            props.SaveProperties(baos, comment1 + '\n' + '\r' + '!' + comment2);
            validateOutput(new String[] { "#comment1", "#", "!comment2" },
                    baos.ToArray());
            baos.Dispose();
        }



        protected byte[] writeProperties()
        {
            MemoryStream bout = new MemoryStream();
            TextWriter ps = new StreamWriter(bout);
            ps.WriteLine("#commented.entry=Bogus");
            ps.WriteLine("test.pkg=harmony.tests");
            ps.WriteLine("test.proj=Automated Tests");
            ps.Dispose();
            return bout.ToArray();
        }


        /**
         * Sets up the fixture, for example, open a network connection. This method
         * is called before a test is executed.
         */
        public override void SetUp()
        {
            tProps = new Dictionary<string, string>();
            tProps["test.prop"] = "this is a test property";
            tProps["bogus.prop"] = "bogus";
            tProps["bool.prop.true"] = "true";
            tProps["bool.prop.false"] = "false";
            tProps["bool.prop.unparsable"] = "ABC";
            tProps["int32.prop.max"] = int.MaxValue.ToString(CultureInfo.InvariantCulture);
            tProps["int32.prop.min"] = int.MinValue.ToString(CultureInfo.InvariantCulture);
            tProps["int32.prop.negone"] = "-1";
            tProps["int32.prop.unparsable"] = "Q456";
        }

        /**
         * Tears down the fixture, for example, close a network connection. This
         * method is called after a test is executed.
         */
        public override void TearDown()
        {
            tProps = null;
        }

    }

    public static class Extensions
    {
        public static byte[] getBytes(this string input)
        {
            return Encoding.UTF8.GetBytes(input);
        }

        public static byte[] getBytes(this string input, string encoding)
        {
            return Encoding.GetEncoding(encoding).GetBytes(input);
        }

        public static string get(this IDictionary<string, string> dict, string key)
        {
            string result;
            dict.TryGetValue(key, out result);
            return result;
        }

        public static string getProperty(this IDictionary<string, string> dict, string key)
        {
            string result;
            dict.TryGetValue(key, out result);
            return result;
        }
    }
}
