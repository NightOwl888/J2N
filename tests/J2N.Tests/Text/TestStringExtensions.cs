using J2N.Globalization;
using NUnit.Framework;
using System;
using System.Globalization;
using System.Text;

namespace J2N.Text
{
    public class TestStringExtensions : TestCase
    {
        static TestStringExtensions()
        {
#if NETSTANDARD
            // Support for 8859-1 and IBM01047 encoding. See: https://docs.microsoft.com/en-us/dotnet/api/system.text.codepagesencodingprovider?view=netcore-2.0
            var encodingProvider = System.Text.CodePagesEncodingProvider.Instance;
            System.Text.Encoding.RegisterProvider(encodingProvider);
#endif
        }


        const string hw1 = "HelloWorld";
        const string hw2 = "HelloWorld";

        [Test]
        public void TestCompareToOrdinal()
        {
            string target = null;
            string compareTo = "Alpine";

            Assert.Greater(0, target.CompareToOrdinal(compareTo.ToCharArray()));
            Assert.Greater(0, target.CompareToOrdinal(new StringBuilder(compareTo)));
            Assert.Greater(0, target.CompareToOrdinal(compareTo));
            Assert.Greater(0, target.CompareToOrdinal(new CharArrayCharSequence(compareTo.ToCharArray())));
            Assert.Greater(0, target.CompareToOrdinal(new StringBuilderCharSequence(new StringBuilder(compareTo))));
            Assert.Greater(0, target.CompareToOrdinal(new StringCharSequence(compareTo)));

            target = "Alpha";

            Assert.Greater(0, target.CompareToOrdinal(compareTo.ToCharArray()));
            Assert.Greater(0, target.CompareToOrdinal(new StringBuilder(compareTo)));
            Assert.Greater(0, target.CompareToOrdinal(compareTo));
            Assert.Greater(0, target.CompareToOrdinal(new CharArrayCharSequence(compareTo.ToCharArray())));
            Assert.Greater(0, target.CompareToOrdinal(new StringBuilderCharSequence(new StringBuilder(compareTo))));
            Assert.Greater(0, target.CompareToOrdinal(new StringCharSequence(compareTo)));

            compareTo = "Alpha";

            Assert.AreEqual(0, target.CompareToOrdinal(compareTo.ToCharArray()));
            Assert.AreEqual(0, target.CompareToOrdinal(new StringBuilder(compareTo)));
            Assert.AreEqual(0, target.CompareToOrdinal(compareTo));
            Assert.AreEqual(0, target.CompareToOrdinal(new CharArrayCharSequence(compareTo.ToCharArray())));
            Assert.AreEqual(0, target.CompareToOrdinal(new StringBuilderCharSequence(new StringBuilder(compareTo))));
            Assert.AreEqual(0, target.CompareToOrdinal(new StringCharSequence(compareTo)));

            compareTo = "Alp";

            Assert.Less(0, target.CompareToOrdinal(compareTo.ToCharArray()));
            Assert.Less(0, target.CompareToOrdinal(new StringBuilder(compareTo)));
            Assert.Less(0, target.CompareToOrdinal(compareTo));
            Assert.Less(0, target.CompareToOrdinal(new CharArrayCharSequence(compareTo.ToCharArray())));
            Assert.Less(0, target.CompareToOrdinal(new StringBuilderCharSequence(new StringBuilder(compareTo))));
            Assert.Less(0, target.CompareToOrdinal(new StringCharSequence(compareTo)));


            Assert.Less(0, target.CompareToOrdinal((char[])null));
            Assert.Less(0, target.CompareToOrdinal((StringBuilder)null));
            Assert.Less(0, target.CompareToOrdinal((string)null));
            Assert.Less(0, target.CompareToOrdinal(new CharArrayCharSequence(null)));
            Assert.Less(0, target.CompareToOrdinal(new StringBuilderCharSequence(null)));
            Assert.Less(0, target.CompareToOrdinal(new StringCharSequence(null)));
        }

        /**
         * @tests java.lang.String#contentEquals(CharSequence cs)
         */
        [Test]
        public void Test_ContentEquals_ICharSequence()
        {
            // Test for method java.lang.String
            // java.lang.String.contentEquals(CharSequence cs)
            assertFalse("Incorrect result of compare", "qwerty".ContentEquals("".AsCharSequence()));
        }

        /**
         * @tests java.lang.String#contentEquals(CharSequence)
         */
        [Test]
        public void Test_ContentEquals_ICharSequence2()
        {
            String s = "abc";
            assertTrue(s.ContentEquals((ICharSequence)new StringBuffer("abc")));
            assertFalse(s.ContentEquals((ICharSequence)new StringBuffer("def")));
            assertFalse(s.ContentEquals((ICharSequence)new StringBuffer("ghij")));

            s = new string("_abc_".ToCharArray(), 1, 3);
            assertTrue(s.ContentEquals((ICharSequence)new StringBuffer("abc")));
            assertFalse(s.ContentEquals((ICharSequence)new StringBuffer("def")));
            assertFalse(s.ContentEquals((ICharSequence)new StringBuffer("ghij")));

            //try
            //{
            //    s.ContentEquals((ICharSequence)null);
            //    fail("No NPE");
            //}
            //catch (ArgumentNullException e)
            //{
            //}

            // J2N: This differs from Java in that ContentEquals never throws an exception.
            // It returns true it both the string and the target are null, otherwise false
            // if the target is null and string is not.
            assertFalse(s.ContentEquals((ICharSequence)null));
            assertTrue(((string)null).ContentEquals((ICharSequence)null));
            assertFalse(((string)null).ContentEquals((ICharSequence)"".AsCharSequence()));
            assertTrue("".ContentEquals((ICharSequence)"".AsCharSequence()));
        }


        /**
         * @tests java.lang.String#contentEquals(CharSequence cs)
         */
        [Test]
        public void Test_ContentEquals_CharArray()
        {
            // Test for method java.lang.String
            // java.lang.String.contentEquals(CharSequence cs)
            assertFalse("Incorrect result of compare", "qwerty".ContentEquals("".ToCharArray()));
        }

        /**
         * @tests java.lang.String#contentEquals(CharSequence)
         */
        [Test]
        public void Test_ContentEquals_CharArray2()
        {
            String s = "abc";
            assertTrue(s.ContentEquals((char[])"abc".ToCharArray()));
            assertFalse(s.ContentEquals((char[])"def".ToCharArray()));
            assertFalse(s.ContentEquals((char[])"ghij".ToCharArray()));

            s = new string("_abc_".ToCharArray(), 1, 3);
            assertTrue(s.ContentEquals((char[])"abc".ToCharArray()));
            assertFalse(s.ContentEquals((char[])"def".ToCharArray()));
            assertFalse(s.ContentEquals((char[])"ghij".ToCharArray()));

            //try
            //{
            //    s.ContentEquals((ICharSequence)null);
            //    fail("No NPE");
            //}
            //catch (ArgumentNullException e)
            //{
            //}

            // J2N: This differs from Java in that ContentEquals never throws an exception.
            // It returns true it both the string and the target are null, otherwise false
            // if the target is null and string is not.
            assertFalse(s.ContentEquals((char[])null));
            assertTrue(((string)null).ContentEquals((char[])null));
            assertFalse(((string)null).ContentEquals((char[])"".ToCharArray()));
            assertTrue("".ContentEquals((char[])"".ToCharArray()));
        }

        /**
         * @tests java.lang.String#contentEquals(CharSequence cs)
         */
        [Test]
        public void Test_ContentEquals_StringBuilder()
        {
            // Test for method java.lang.String
            // java.lang.String.contentEquals(CharSequence cs)
            assertFalse("Incorrect result of compare", "qwerty".ContentEquals(new StringBuilder("")));
        }

        /**
         * @tests java.lang.String#contentEquals(CharSequence)
         */
        [Test]
        public void Test_ContentEquals_StringBuilder2()
        {
            String s = "abc";
            assertTrue(s.ContentEquals((StringBuilder)new StringBuilder("abc")));
            assertFalse(s.ContentEquals((StringBuilder)new StringBuilder("def")));
            assertFalse(s.ContentEquals((StringBuilder)new StringBuilder("ghij")));

            s = new string("_abc_".ToCharArray(), 1, 3);
            assertTrue(s.ContentEquals((StringBuilder)new StringBuilder("abc")));
            assertFalse(s.ContentEquals((StringBuilder)new StringBuilder("def")));
            assertFalse(s.ContentEquals((StringBuilder)new StringBuilder("ghij")));

            //try
            //{
            //    s.ContentEquals((ICharSequence)null);
            //    fail("No NPE");
            //}
            //catch (ArgumentNullException e)
            //{
            //}

            // J2N: This differs from Java in that ContentEquals never throws an exception.
            // It returns true it both the string and the target are null, otherwise false
            // if the target is null and string is not.
            assertFalse(s.ContentEquals((StringBuilder)null));
            assertTrue(((string)null).ContentEquals((StringBuilder)null));
            assertFalse(((string)null).ContentEquals((StringBuilder) new StringBuilder("")));
            assertTrue("".ContentEquals((StringBuilder)new StringBuilder("")));
        }

        /**
         * @tests java.lang.String#contentEquals(CharSequence cs)
         */
        [Test]
        public void Test_ContentEquals_String()
        {
            // Test for method java.lang.String
            // java.lang.String.contentEquals(CharSequence cs)
            assertFalse("Incorrect result of compare", "qwerty".ContentEquals(""));
        }

        /**
         * @tests java.lang.String#contentEquals(CharSequence)
         */
        [Test]
        public void Test_ContentEquals_String2()
        {
            String s = "abc";
            assertTrue(s.ContentEquals("abc"));
            assertFalse(s.ContentEquals("def"));
            assertFalse(s.ContentEquals("ghij"));

            s = new string("_abc_".ToCharArray(), 1, 3);
            assertTrue(s.ContentEquals("abc"));
            assertFalse(s.ContentEquals("def"));
            assertFalse(s.ContentEquals("ghij"));

            //try
            //{
            //    s.ContentEquals((ICharSequence)null);
            //    fail("No NPE");
            //}
            //catch (ArgumentNullException e)
            //{
            //}

            // J2N: This differs from Java in that ContentEquals never throws an exception.
            // It returns true it both the string and the target are null, otherwise false
            // if the target is null and string is not.
            assertFalse(s.ContentEquals((string)null));
            assertTrue(((string)null).ContentEquals((string)null));
            assertFalse(((string)null).ContentEquals((string)""));
            assertTrue("".ContentEquals((string)""));
        }

        /**
         * @tests java.lang.String#getBytes()
         */
        [Test]
        public void Test_GetBytes()
        {
#if !NETCOREAPP1_0
            // Test for method byte [] java.lang.String.getBytes()
            byte[] sbytes = hw1.GetBytes(Encoding.Default);

            bool isEbcdic = Encoding.Default.Equals(Encoding.GetEncoding("IBM01047"));
            //bool isEbcdic = Charset.defaultCharset().equals(Charset.forName("IBM1047"));
            if (!isEbcdic)
            {
                for (int i = 0; i < hw1.Length; i++)
                    assertTrue("Returned incorrect bytes", sbytes[i] == (byte)hw1[i]);
            }
            else
            {
                // On EBCDIC platforms, getBytes() returns different values
                // Reference values taken from J9 5.0
                byte[] expectedValues;
                unchecked
                {
                    expectedValues = new byte[] {(byte)-56, (byte)-123, (byte)-109, (byte)-109, (byte)-106, (byte)-26, (byte)-106,
                        (byte)-103, (byte)-109, (byte)-124};
                }
                for (int i = 0; i < hw1.Length; i++)
                    assertEquals(expectedValues[i], sbytes[i]);
            }
#endif

            char[] chars = new char[1];
            for (int i = 0; i < 65536; i++)
            {
                // skip surrogates
                if (i == 0xd800)
                    i = 0xe000;
                byte[] result = null;
                chars[0] = (char)i;
                string str = new string(chars);
                try
                {
                    result = str.GetBytes(Encoding.GetEncoding("iso-8859-1",
                        new EncoderReplacementFallback("?"),
                        new DecoderReplacementFallback("?")));
                    if (i < 256)
                    {
                        assertEquals((byte)i, result[0]);
                    }
                    else
                    {
                        /*
                         * Substitute character should be 0x1A [1], but may be '?'
                         * character. [1]
                         * http://en.wikipedia.org/wiki/Substitute_character
                         */
                        assertTrue(result[0] == '?' || result[0] == 0x1a);
                    }
                }
                catch (NotSupportedException e)
                {
                }
                try
                {
                    result = str.GetBytes(Encoding.UTF8);
                    int length = i < 0x80 ? 1 : (i < 0x800 ? 2 : 3);
                    assertTrue("Wrong length UTF8: " + i.ToHexString(),
                            result.Length == length);
                    assertTrue(
                            "Wrong bytes UTF8: " + i.ToHexString(),
                            (i < 0x80 && result[0] == i)
                                    || (i >= 0x80
                                            && i < 0x800
                                            && result[0] == (byte)(0xc0 | ((i & 0x7c0) >> 6)) && result[1] == (byte)(0x80 | (i & 0x3f)))
                                    || (i >= 0x800
                                            && result[0] == (byte)(0xe0 | (i >> 12))
                                            && result[1] == (byte)(0x80 | ((i & 0xfc0) >> 6)) && result[2] == (byte)(0x80 | (i & 0x3f))));
                }
                catch (NotSupportedException e)
                {
                }

                string bytes2 = null;
                try
                {
                    bytes2 = Encoding.UTF8.GetString(result); //new string(result, "UTF8");
                    assertTrue("Wrong UTF8 byte length: " + bytes2.Length + "("
                            + i + ")", bytes2.Length == 1);
                    assertTrue(
                            "Wrong char UTF8: "
                                    + (bytes2[0]).ToHexString() + " ("
                                    + i + ")", bytes2[0] == i);
                }
                catch (NotSupportedException e)
                {
                }
            }

            byte[] bytes = new byte[1];
            for (int i = 0; i < 256; i++)
            {
                bytes[0] = (byte)i;
                string result = null;
                try
                {
                    result = Encoding.GetEncoding("iso-8859-1").GetString(bytes); //new string(bytes, "8859_1");
                    assertEquals("Wrong char length", 1, result.Length);
                    assertTrue("Wrong char value", result[0] == (char)i);
                }
                catch (NotSupportedException e)
                {
                }
            }
        }


        /**
         * @tests java.lang.String#indexOf(int)
         */
        [Test]
        public void Test_IndexOf_Int32()
        {
            // Test for method int java.lang.String.indexOf(int)
            assertEquals("Invalid index returned", 1, hw1.IndexOf((int)'e'));
            assertEquals("Invalid index returned", 1, "a\ud800\udc00".IndexOf(0x10000));

            StringExtensions.IndexOf(TestStringSupplementary, "𠳕".CodePointAt(0));
        }

        private const string TestStringSupplementary = "李红：不，那不是杂志。那是字典。𠳕";

        /**
         * @tests java.lang.String#indexOf(int, int)
         */
        [Test]
        public void Test_IndexOf_Int32_Int32()
        {
            // Test for method int java.lang.String.indexOf(int, int)
            assertEquals("Invalid character index returned", 5, hw1.IndexOf((int)'W', 2));
            assertEquals("Invalid index returned", 2, "ab\ud800\udc00".IndexOf(0x10000, 1));
        }

        /**
         * @tests java.lang.String#intern()
         */
        [Test]
        public void Test_Intern()
        {
            // Test for method java.lang.String java.lang.String.intern()
            assertTrue("Intern returned incorrect result", hw1.Intern() == hw2
                    .Intern());
        }

        /**
         * @tests java.lang.String#lastIndexOf(int)
         */
        [Test]
        public void Test_LastIndexOf_Int32()
        {
            // Test for method int java.lang.String.lastIndexOf(int)
            assertEquals("Failed to return correct index", 5, hw1.LastIndexOf((int)'W'));
            assertEquals("Returned index for non-existent char", -1, hw1
                    .LastIndexOf((int)'Z'));
            assertEquals("Failed to return correct index", 1, "a\ud800\udc00"
                    .LastIndexOf(0x10000));
        }

        /**
         * @tests java.lang.String#lastIndexOf(int, int)
         */
        [Test]
        public void Test_LastIndexOf_Int32_Int32()
        {
            // Test for method int java.lang.String.lastIndexOf(int, int)
            assertEquals("Failed to return correct index", 5, hw1.LastIndexOf((int)'W',
                    6));
            assertEquals("Returned index for char out of specified range", -1, hw1
                    .LastIndexOf((int)'W', 4));
            assertEquals("Returned index for non-existent char", -1, hw1
                    .LastIndexOf((int)'Z', 9));

        }

        /**
         * @tests java.lang.String#regionMatches(int, java.lang.String, int, int)
         */
        [Test]
        public void Test_RegionMatches_String_Int32_ICharSequence_Int32_Int32_StringComparison()
        {
            // Test for method boolean java.lang.String.regionMatches(int,
            // java.lang.String, int, int)
            String bogusString = "xxcedkedkleiorem lvvwr e''' 3r3r 23r";

            assertTrue("identical regions failed comparison", hw1.RegionMatches(2,
                    hw2.AsCharSequence(), 2, 5, StringComparison.Ordinal));
            assertTrue("Different regions returned true", !hw1.RegionMatches(2,
                    bogusString.AsCharSequence(), 2, 5, StringComparison.Ordinal));

            var input = "iiiiiiiiiiiıIiii";
            var test = "İII"; // Turkish capital dotted I test.

            using (var context = new CultureContext("tr"))
            {
                assertTrue("Turkish captial I test failed using StringComparison.CurrentCultureIgnoreCase in tr culture",
                    input.RegionMatches(10, test, 0, 3, StringComparison.CurrentCultureIgnoreCase));
                assertFalse("Turkish captial I test failed using StringComparison.Ordinal in tr culture",
                    input.RegionMatches(10, test, 0, 3, StringComparison.Ordinal));
            }
            using (var context = new CultureContext("en"))
            {
                if (!input.RegionMatches(10, test, 0, 3, StringComparison.CurrentCultureIgnoreCase))
                {
                    assertFalse("Turkish captial I test failed using StringComparison.CurrentCultureIgnoreCase in en culture",
                    input.RegionMatches(10, test, 0, 3, StringComparison.CurrentCultureIgnoreCase));
                    assertFalse("Turkish captial I test failed using StringComparison.Ordinal in en culture",
                        input.RegionMatches(10, test, 0, 3, StringComparison.Ordinal));
                }
            }
        }

        /**
         * @tests java.lang.String#regionMatches(boolean, int, java.lang.String,
         *        int, int)
         */
        [Test]
        public void Test_RegionMatches_String_Int32_ICharSequence_Int32_Int32_StringComparison_OrdinalIgnoreCase()
        {
            // Test for method boolean java.lang.String.regionMatches(boolean, int,
            // java.lang.String, int, int)

            String bogusString = "xxcedkedkleiorem lvvwr e''' 3r3r 23r";

            assertTrue("identical regions failed comparison", hw1.RegionMatches(
                    2, hw2.AsCharSequence(), 2, 5, StringComparison.Ordinal));
            assertTrue("identical regions failed comparison with different cases",
                    hw1.RegionMatches(2, hw2.AsCharSequence(), 2, 5, StringComparison.OrdinalIgnoreCase));
            assertTrue("Different regions returned true", !hw1.RegionMatches(
                    2, bogusString.AsCharSequence(), 2, 5, StringComparison.OrdinalIgnoreCase));
            assertTrue("identical regions failed comparison with different cases",
                    hw1.RegionMatches(2, hw2.AsCharSequence(), 2, 5, StringComparison.Ordinal));
        }

        /**
         * @tests java.lang.String#regionMatches(int, java.lang.String, int, int)
         */
        [Test]
        public void Test_RegionMatches_String_Int32_CharArray_Int32_Int32_StringComparison()
        {
            // Test for method boolean java.lang.String.regionMatches(int,
            // java.lang.String, int, int)
            String bogusString = "xxcedkedkleiorem lvvwr e''' 3r3r 23r";

            assertTrue("identical regions failed comparison", hw1.RegionMatches(2,
                    hw2.ToCharArray(), 2, 5, StringComparison.Ordinal));
            assertTrue("Different regions returned true", !hw1.RegionMatches(2,
                    bogusString.ToCharArray(), 2, 5, StringComparison.Ordinal));

            var input = "iiiiiiiiiiiıIiii";
            var test = "İII"; // Turkish capital dotted I test.

            using (var context = new CultureContext("tr"))
            {
                assertTrue("Turkish captial I test failed using StringComparison.CurrentCultureIgnoreCase in tr culture",
                    input.RegionMatches(10, test, 0, 3, StringComparison.CurrentCultureIgnoreCase));
                assertFalse("Turkish captial I test failed using StringComparison.Ordinal in tr culture",
                    input.RegionMatches(10, test, 0, 3, StringComparison.Ordinal));
            }
            using (var context = new CultureContext("en"))
            {
                if (!input.RegionMatches(10, test, 0, 3, StringComparison.CurrentCultureIgnoreCase))
                {
                    assertFalse("Turkish captial I test failed using StringComparison.CurrentCultureIgnoreCase in en culture",
                    input.RegionMatches(10, test, 0, 3, StringComparison.CurrentCultureIgnoreCase));
                    assertFalse("Turkish captial I test failed using StringComparison.Ordinal in en culture",
                        input.RegionMatches(10, test, 0, 3, StringComparison.Ordinal));
                }
            }
        }

        /**
         * @tests java.lang.String#regionMatches(boolean, int, java.lang.String,
         *        int, int)
         */
        [Test]
        public void Test_RegionMatches_String_Int32_CharArray_Int32_Int32_StringComparison_OrdinalIgnoreCase()
        {
            // Test for method boolean java.lang.String.regionMatches(boolean, int,
            // java.lang.String, int, int)

            String bogusString = "xxcedkedkleiorem lvvwr e''' 3r3r 23r";

            assertTrue("identical regions failed comparison", hw1.RegionMatches(
                    2, hw2.ToCharArray(), 2, 5, StringComparison.Ordinal));
            assertTrue("identical regions failed comparison with different cases",
                    hw1.RegionMatches(2, hw2.ToCharArray(), 2, 5, StringComparison.OrdinalIgnoreCase));
            assertTrue("Different regions returned true", !hw1.RegionMatches(
                    2, bogusString.ToCharArray(), 2, 5, StringComparison.OrdinalIgnoreCase));
            assertTrue("identical regions failed comparison with different cases",
                    hw1.RegionMatches(2, hw2.ToCharArray(), 2, 5, StringComparison.Ordinal));
        }

        /**
         * @tests java.lang.String#regionMatches(int, java.lang.String, int, int)
         */
        [Test]
        public void Test_RegionMatches_String_Int32_StringBuilder_Int32_Int32_StringComparison()
        {
            // Test for method boolean java.lang.String.regionMatches(int,
            // java.lang.String, int, int)
            String bogusString = "xxcedkedkleiorem lvvwr e''' 3r3r 23r";

            assertTrue("identical regions failed comparison", hw1.RegionMatches(2,
                    new StringBuilder(hw2), 2, 5, StringComparison.Ordinal));
            assertTrue("Different regions returned true", !hw1.RegionMatches(2,
                    new StringBuilder(bogusString), 2, 5, StringComparison.Ordinal));

            var input = "iiiiiiiiiiiıIiii";
            var test = "İII"; // Turkish capital dotted I test.

            using (var context = new CultureContext("tr"))
            {
                assertTrue("Turkish captial I test failed using StringComparison.CurrentCultureIgnoreCase in tr culture",
                    input.RegionMatches(10, test, 0, 3, StringComparison.CurrentCultureIgnoreCase));
                assertFalse("Turkish captial I test failed using StringComparison.Ordinal in tr culture",
                    input.RegionMatches(10, test, 0, 3, StringComparison.Ordinal));
            }
            using (var context = new CultureContext("en"))
            {
                if (!input.RegionMatches(10, test, 0, 3, StringComparison.CurrentCultureIgnoreCase))
                {
                    assertFalse("Turkish captial I test failed using StringComparison.CurrentCultureIgnoreCase in en culture",
                    input.RegionMatches(10, test, 0, 3, StringComparison.CurrentCultureIgnoreCase));
                    assertFalse("Turkish captial I test failed using StringComparison.Ordinal in en culture",
                        input.RegionMatches(10, test, 0, 3, StringComparison.Ordinal));
                }
            }
        }

        /**
         * @tests java.lang.String#regionMatches(boolean, int, java.lang.String,
         *        int, int)
         */
        [Test]
        public void Test_RegionMatches_String_Int32_StringBuilder_Int32_Int32_StringComparison_OrdinalIgnoreCase()
        {
            // Test for method boolean java.lang.String.regionMatches(boolean, int,
            // java.lang.String, int, int)

            String bogusString = "xxcedkedkleiorem lvvwr e''' 3r3r 23r";

            assertTrue("identical regions failed comparison", hw1.RegionMatches(
                    2, new StringBuilder(hw2), 2, 5, StringComparison.Ordinal));
            assertTrue("identical regions failed comparison with different cases",
                    hw1.RegionMatches(2, new StringBuilder(hw2), 2, 5, StringComparison.OrdinalIgnoreCase));
            assertTrue("Different regions returned true", !hw1.RegionMatches(
                    2, new StringBuilder(bogusString), 2, 5, StringComparison.OrdinalIgnoreCase));
            assertTrue("identical regions failed comparison with different cases",
                    hw1.RegionMatches(2, new StringBuilder(hw2), 2, 5, StringComparison.Ordinal));
        }

        /**
         * @tests java.lang.String#regionMatches(int, java.lang.String, int, int)
         */
        [Test]
        public void Test_RegionMatches_String_Int32_String_Int32_Int32_StringComparison()
        {
            // Test for method boolean java.lang.String.regionMatches(int,
            // java.lang.String, int, int)
            String bogusString = "xxcedkedkleiorem lvvwr e''' 3r3r 23r";

            assertTrue("identical regions failed comparison", hw1.RegionMatches(2,
                    hw2, 2, 5, StringComparison.Ordinal));
            assertTrue("Different regions returned true", !hw1.RegionMatches(2,
                    bogusString, 2, 5, StringComparison.Ordinal));

            var input = "iiiiiiiiiiiıIiii";
            var test = "İII"; // Turkish capital dotted I test.

            using (var context = new CultureContext("tr"))
            {
                assertTrue("Turkish captial I test failed using StringComparison.CurrentCultureIgnoreCase in tr culture", 
                    input.RegionMatches(10, test, 0, 3, StringComparison.CurrentCultureIgnoreCase));
                assertFalse("Turkish captial I test failed using StringComparison.Ordinal in tr culture", 
                    input.RegionMatches(10, test, 0, 3, StringComparison.Ordinal));
            }
            using (var context = new CultureContext("en"))
            {
                if (!input.RegionMatches(10, test, 0, 3, StringComparison.CurrentCultureIgnoreCase))
                {
                    assertFalse("Turkish captial I test failed using StringComparison.CurrentCultureIgnoreCase in en culture",
                    input.RegionMatches(10, test, 0, 3, StringComparison.CurrentCultureIgnoreCase));
                    assertFalse("Turkish captial I test failed using StringComparison.Ordinal in en culture",
                        input.RegionMatches(10, test, 0, 3, StringComparison.Ordinal));
                }
            }
        }

        /**
         * @tests java.lang.String#regionMatches(boolean, int, java.lang.String,
         *        int, int)
         */
        [Test]
        public void Test_RegionMatches_String_Int32_String_Int32_Int32_StringComparison_OrdinalIgnoreCase()
        {
            // Test for method boolean java.lang.String.regionMatches(boolean, int,
            // java.lang.String, int, int)

            String bogusString = "xxcedkedkleiorem lvvwr e''' 3r3r 23r";

            assertTrue("identical regions failed comparison", hw1.RegionMatches(
                    2, hw2, 2, 5, StringComparison.Ordinal));
            assertTrue("identical regions failed comparison with different cases",
                    hw1.RegionMatches(2, hw2, 2, 5, StringComparison.OrdinalIgnoreCase));
            assertTrue("Different regions returned true", !hw1.RegionMatches(
                    2, bogusString, 2, 5, StringComparison.OrdinalIgnoreCase));
            assertTrue("identical regions failed comparison with different cases",
                    hw1.RegionMatches(2, hw2, 2, 5, StringComparison.Ordinal));
        }

        /**
        * @tests java.lang.String#startsWith(java.lang.String, int)
        */
        [Test]
        public void Test_StartsWith_String_Int32_StringComparison()
        {
            // Test for method boolean java.lang.String.startsWith(java.lang.String,
            // int)
            assertTrue("Failed to find string", hw1.StartsWith("World", 5, StringComparison.Ordinal));
            assertTrue("Found incorrect string", !hw1.StartsWith("Hello", 5, StringComparison.Ordinal));

            Assert.Throws<ArgumentNullException>(() => hw1.StartsWith((string)null, 5, StringComparison.Ordinal));
            Assert.Throws<ArgumentNullException>(() => ((string)null).StartsWith("test", 3, StringComparison.Ordinal));
            Assert.DoesNotThrow(() => "test".StartsWith("test", 6, StringComparison.Ordinal));
        }

        [Test]
        public void TestSubsequence()
        {
            string target = "This is a test";

            assertEquals("This is a test", target.Subsequence(0, target.Length));
            assertEquals("is a", target.Subsequence(5, 4));
            assertEquals("", target.Subsequence(4, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => target.Subsequence(-1, 10));
            Assert.Throws<ArgumentOutOfRangeException>(() => target.Subsequence(3, -2));
            Assert.Throws<ArgumentOutOfRangeException>(() => target.Subsequence(target.Length, 1));

            string emptyTarget = string.Empty;

            Assert.Throws<ArgumentOutOfRangeException>(() => emptyTarget.Subsequence(0, 1));

            string nullTarget = null;

            Assert.IsFalse(nullTarget.Subsequence(6, 10).HasValue); // Null target will always return null subsequence
        }

        [Test]
        public void TestAsCharSequence()
        {
            string target = "This is a test";

            var result = target.AsCharSequence();

            assertNotNull(result);
            assertEquals(typeof(StringCharSequence), result.GetType());
        }
    }
}
