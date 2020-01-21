using J2N.Collections;
using J2N.Text;
using NUnit.Framework;
using System;
using System.Globalization;
using System.Text;

namespace J2N
{
    public class TestCharacter : TestCase
    {
        //[Test]
        //public void Test_valueOfC()
        //{
        //    // test the cache range
        //    for (char c = '\u0000'; c < 512; c++)
        //    {
        //        Character e = new Character(c);
        //        Character a = Character.valueOf(c);
        //        assertEquals(e, a);
        //    }
        //    // test the rest of the chars
        //    for (int c = 512; c <= Character.MAX_VALUE; c++)
        //    {
        //        assertEquals(new Character((char)c), Character.valueOf((char)c));
        //    }
        //}

        [Test]
        public void Test_isValidCodePointI()
        {
            assertFalse(Character.IsValidCodePoint(-1));
            assertTrue(Character.IsValidCodePoint(0));
            assertTrue(Character.IsValidCodePoint(1));
            assertFalse(Character.IsValidCodePoint(int.MaxValue));

            for (int c = '\u0000'; c <= 0x10FFFF; c++)
            {
                assertTrue(Character.IsValidCodePoint(c));
            }

            assertFalse(Character.IsValidCodePoint(0x10FFFF + 1));
        }

        [Test]
        public void Test_isSupplementaryCodePointI()
        {
            assertFalse(Character.IsSupplementaryCodePoint(-1));

            for (int c = '\u0000'; c <= '\uFFFF'; c++)
            {
                assertFalse(Character.IsSupplementaryCodePoint(c));
            }

            for (int c = 0xFFFF + 1; c <= 0x10FFFF; c++)
            {
                assertTrue(Character.IsSupplementaryCodePoint(c));
            }

            assertFalse(Character.IsSupplementaryCodePoint(0x10FFFF + 1));
        }

        //[Test]
        //public void Test_isHighSurrogateC()
        //{
        //    // (\uD800-\uDBFF)
        //    assertFalse(Character.IsHighSurrogate((char)('\uD800' - 1)));
        //    for (int c = '\uD800'; c <= '\uDBFF'; c++)
        //    {
        //        assertTrue(Character.IsHighSurrogate((char)c));
        //    }
        //    assertFalse(Character.IsHighSurrogate((char)('\uDBFF' + 1)));
        //    assertFalse(Character.IsHighSurrogate('\uFFFF'));
        //}

        //[Test]
        //public void Test_isLowSurrogateC()
        //{
        //    // (\uDC00-\uDFFF)
        //    assertFalse(Character.IsLowSurrogate((char)('\uDC00' - 1)));
        //    for (int c = '\uDC00'; c <= '\uDFFF'; c++)
        //    {
        //        assertTrue(Character.IsLowSurrogate((char)c));
        //    }
        //    assertFalse(Character.IsLowSurrogate((char)('\uDFFF' + 1)));
        //}

        //[Test]
        //public void Test_isSurrogatePairCC()
        //{
        //    assertFalse(Character.IsSurrogatePair('\u0000', '\u0000'));
        //    assertFalse(Character.IsSurrogatePair('\u0000', '\uDC00'));

        //    assertTrue(Character.IsSurrogatePair('\uD800', '\uDC00'));
        //    assertTrue(Character.IsSurrogatePair('\uD800', '\uDFFF'));
        //    assertTrue(Character.IsSurrogatePair('\uDBFF', '\uDFFF'));

        //    assertFalse(Character.IsSurrogatePair('\uDBFF', '\uF000'));
        //}

        [Test]
        public void Test_charCountI()
        {

            for (int c = '\u0000'; c <= '\uFFFF'; c++)
            {
                assertEquals(1, Character.CharCount(c));
            }

            for (int c = 0xFFFF + 1; c <= 0x10FFFF; c++)
            {
                assertEquals(2, Character.CharCount(c));
            }

            // invalid code points work in this method
            assertEquals(2, Character.CharCount(int.MaxValue));
        }

        [Test]
        public void Test_toCodePointCC()
        {
            int result = Character.ToCodePoint('\uD800', '\uDC00');
            assertEquals(0x00010000, result);

            result = Character.ToCodePoint('\uD800', '\uDC01');
            assertEquals(0x00010001, result);

            result = Character.ToCodePoint('\uD801', '\uDC01');
            assertEquals(0x00010401, result);

            result = Character.ToCodePoint('\uDBFF', '\uDFFF');
            assertEquals(0x00010FFFF, result);
        }

        [Test]
        public void Test_codePointAt_CharSequenceI()
        {

            assertEquals('a', Character.CodePointAt((ICharSequence)"abc".AsCharSequence(), 0));
            assertEquals('b', Character.CodePointAt((ICharSequence)"abc".AsCharSequence(), 1));
            assertEquals('c', Character.CodePointAt((ICharSequence)"abc".AsCharSequence(), 2));
            assertEquals(0x10000, Character.CodePointAt(
                    (ICharSequence)"\uD800\uDC00".AsCharSequence(), 0));
            assertEquals('\uDC00', Character.CodePointAt(
                    (ICharSequence)"\uD800\uDC00".AsCharSequence(), 1));

            try
            {
                Character.CodePointAt((ICharSequence)null, 0);
                fail("No NPE.");
            }
            catch (ArgumentNullException e)
            {
            }

            try
            {
                Character.CodePointAt((ICharSequence)"abc".AsCharSequence(), -1);
                fail("No IOOBE, negative index.");
            }
            catch (ArgumentOutOfRangeException e)
            {
            }

            try
            {
                Character.CodePointAt((ICharSequence)"abc".AsCharSequence(), 4);
                fail("No IOOBE, index too large.");
            }
            catch (ArgumentOutOfRangeException e)
            {
            }
        }

        [Test]
        public void Test_codePointAt_CharArrayI()
        {

            assertEquals('a', Character.CodePointAt((char[])"abc".ToCharArray(), 0));
            assertEquals('b', Character.CodePointAt((char[])"abc".ToCharArray(), 1));
            assertEquals('c', Character.CodePointAt((char[])"abc".ToCharArray(), 2));
            assertEquals(0x10000, Character.CodePointAt(
                    (char[])"\uD800\uDC00".ToCharArray(), 0));
            assertEquals('\uDC00', Character.CodePointAt(
                    (char[])"\uD800\uDC00".ToCharArray(), 1));

            try
            {
                Character.CodePointAt((char[])null, 0);
                fail("No NPE.");
            }
            catch (ArgumentNullException e)
            {
            }

            try
            {
                Character.CodePointAt((char[])"abc".ToCharArray(), -1);
                fail("No IOOBE, negative index.");
            }
            catch (ArgumentOutOfRangeException e)
            {
            }

            try
            {
                Character.CodePointAt((char[])"abc".ToCharArray(), 4);
                fail("No IOOBE, index too large.");
            }
            catch (ArgumentOutOfRangeException e)
            {
            }
        }

        [Test]
        public void Test_codePointAt_StringBuilderI()
        {

            assertEquals('a', Character.CodePointAt((StringBuilder)new StringBuilder("abc"), 0));
            assertEquals('b', Character.CodePointAt((StringBuilder)new StringBuilder("abc"), 1));
            assertEquals('c', Character.CodePointAt((StringBuilder)new StringBuilder("abc"), 2));
            assertEquals(0x10000, Character.CodePointAt(
                    (StringBuilder)new StringBuilder("\uD800\uDC00"), 0));
            assertEquals('\uDC00', Character.CodePointAt(
                    (StringBuilder)new StringBuilder("\uD800\uDC00"), 1));

            try
            {
                Character.CodePointAt((StringBuilder)null, 0);
                fail("No NPE.");
            }
            catch (ArgumentNullException e)
            {
            }

            try
            {
                Character.CodePointAt((StringBuilder)new StringBuilder("abc"), -1);
                fail("No IOOBE, negative index.");
            }
            catch (ArgumentOutOfRangeException e)
            {
            }

            try
            {
                Character.CodePointAt((StringBuilder)new StringBuilder("abc"), 4);
                fail("No IOOBE, index too large.");
            }
            catch (ArgumentOutOfRangeException e)
            {
            }
        }

        [Test]
        public void Test_codePointAt_StringI()
        {

            assertEquals('a', Character.CodePointAt((string)"abc", 0));
            assertEquals('b', Character.CodePointAt((string)"abc", 1));
            assertEquals('c', Character.CodePointAt((string)"abc", 2));
            assertEquals(0x10000, Character.CodePointAt(
                    (string)"\uD800\uDC00", 0));
            assertEquals('\uDC00', Character.CodePointAt(
                    (string)"\uD800\uDC00", 1));

            try
            {
                Character.CodePointAt((string)null, 0);
                fail("No NPE.");
            }
            catch (ArgumentNullException e)
            {
            }

            try
            {
                Character.CodePointAt((string)"abc", -1);
                fail("No IOOBE, negative index.");
            }
            catch (ArgumentOutOfRangeException e)
            {
            }

            try
            {
                Character.CodePointAt((string)"abc", 4);
                fail("No IOOBE, index too large.");
            }
            catch (ArgumentOutOfRangeException e)
            {
            }
        }

        [Test]
        public void Test_codePointAt_CI()
        {

            assertEquals('a', Character.CodePointAt("abc".ToCharArray(), 0));
            assertEquals('b', Character.CodePointAt("abc".ToCharArray(), 1));
            assertEquals('c', Character.CodePointAt("abc".ToCharArray(), 2));
            assertEquals(0x10000, Character.CodePointAt("\uD800\uDC00"
                    .ToCharArray(), 0));
            assertEquals('\uDC00', Character.CodePointAt("\uD800\uDC00"
                    .ToCharArray(), 1));

            try
            {
                Character.CodePointAt((char[])null, 0);
                fail("No NPE.");
            }
            catch (ArgumentNullException e)
            {
            }

            try
            {
                Character.CodePointAt("abc".ToCharArray(), -1);
                fail("No IOOBE, negative index.");
            }
            catch (ArgumentOutOfRangeException e)
            {
            }

            try
            {
                Character.CodePointAt("abc".ToCharArray(), 4);
                fail("No IOOBE, index too large.");
            }
            catch (ArgumentOutOfRangeException e)
            {
            }
        }

        [Test]
        public void Test_codePointAt_CII()
        {

            assertEquals('a', Character.CodePointAt("abc".ToCharArray(), 0, 3));
            assertEquals('b', Character.CodePointAt("abc".ToCharArray(), 1, 3));
            assertEquals('c', Character.CodePointAt("abc".ToCharArray(), 2, 3));
            assertEquals(0x10000, Character.CodePointAt("\uD800\uDC00"
                    .ToCharArray(), 0, 2));
            assertEquals('\uDC00', Character.CodePointAt("\uD800\uDC00"
                    .ToCharArray(), 1, 2));
            assertEquals('\uD800', Character.CodePointAt("\uD800\uDC00"
                    .ToCharArray(), 0, 1));

            try
            {
                Character.CodePointAt((char[])null, 0, 1);
                fail("No NPE.");
            }
            catch (ArgumentNullException e)
            {
            }

            try
            {
                Character.CodePointAt("abc".ToCharArray(), -1, 3);
                fail("No IOOBE, negative index.");
            }
            catch (ArgumentOutOfRangeException e)
            {
            }

            try
            {
                Character.CodePointAt("abc".ToCharArray(), 4, 3);
                fail("No IOOBE, index too large.");
            }
            catch (ArgumentOutOfRangeException e)
            {
            }

            try
            {
                Character.CodePointAt("abc".ToCharArray(), 2, 1);
                fail("No IOOBE, index larger than limit.");
            }
            catch (ArgumentOutOfRangeException e)
            {
            }

            try
            {
                Character.CodePointAt("abc".ToCharArray(), 2, -1);
                fail("No IOOBE, limit is negative.");
            }
            catch (ArgumentOutOfRangeException e)
            {
            }
        }

        [Test]
        public void Test_codePointBefore_ICharSequenceI()
        {

            assertEquals('a', Character.CodePointBefore((ICharSequence)"abc".AsCharSequence(), 1));
            assertEquals('b', Character.CodePointBefore((ICharSequence)"abc".AsCharSequence(), 2));
            assertEquals('c', Character.CodePointBefore((ICharSequence)"abc".AsCharSequence(), 3));
            assertEquals(0x10000, Character.CodePointBefore(
                    (ICharSequence)"\uD800\uDC00".AsCharSequence(), 2));
            assertEquals('\uD800', Character.CodePointBefore(
                    (ICharSequence)"\uD800\uDC00".AsCharSequence(), 1));

            try
            {
                Character.CodePointBefore((ICharSequence)null, 0);
                fail("No NPE.");
            }
            catch (ArgumentNullException e)
            {
            }

            try
            {
                Character.CodePointBefore((ICharSequence)"abc".AsCharSequence(), 0);
                fail("No IOOBE, index below one.");
            }
            catch (ArgumentOutOfRangeException e)
            {
            }

            try
            {
                Character.CodePointBefore((ICharSequence)"abc".AsCharSequence(), 4);
                fail("No IOOBE, index too large.");
            }
            catch (ArgumentOutOfRangeException e)
            {
            }
        }

        [Test]
        public void Test_codePointBefore_CharArrayI()
        {

            assertEquals('a', Character.CodePointBefore((char[])"abc".ToCharArray(), 1));
            assertEquals('b', Character.CodePointBefore((char[])"abc".ToCharArray(), 2));
            assertEquals('c', Character.CodePointBefore((char[])"abc".ToCharArray(), 3));
            assertEquals(0x10000, Character.CodePointBefore(
                    (char[])"\uD800\uDC00".ToCharArray(), 2));
            assertEquals('\uD800', Character.CodePointBefore(
                    (char[])"\uD800\uDC00".ToCharArray(), 1));

            try
            {
                Character.CodePointBefore((char[])null, 0);
                fail("No NPE.");
            }
            catch (ArgumentNullException e)
            {
            }

            try
            {
                Character.CodePointBefore((char[])"abc".ToCharArray(), 0);
                fail("No IOOBE, index below one.");
            }
            catch (ArgumentOutOfRangeException e)
            {
            }

            try
            {
                Character.CodePointBefore((char[])"abc".ToCharArray(), 4);
                fail("No IOOBE, index too large.");
            }
            catch (ArgumentOutOfRangeException e)
            {
            }
        }

        [Test]
        public void Test_codePointBefore_StringBuilderI()
        {

            assertEquals('a', Character.CodePointBefore((StringBuilder)new StringBuilder("abc"), 1));
            assertEquals('b', Character.CodePointBefore((StringBuilder)new StringBuilder("abc"), 2));
            assertEquals('c', Character.CodePointBefore((StringBuilder)new StringBuilder("abc"), 3));
            assertEquals(0x10000, Character.CodePointBefore(
                    (StringBuilder)new StringBuilder("\uD800\uDC00"), 2));
            assertEquals('\uD800', Character.CodePointBefore(
                    (StringBuilder)new StringBuilder("\uD800\uDC00"), 1));

            try
            {
                Character.CodePointBefore((StringBuilder)null, 0);
                fail("No NPE.");
            }
            catch (ArgumentNullException e)
            {
            }

            try
            {
                Character.CodePointBefore((StringBuilder)new StringBuilder("abc"), 0);
                fail("No IOOBE, index below one.");
            }
            catch (ArgumentOutOfRangeException e)
            {
            }

            try
            {
                Character.CodePointBefore((StringBuilder)new StringBuilder("abc"), 4);
                fail("No IOOBE, index too large.");
            }
            catch (ArgumentOutOfRangeException e)
            {
            }
        }

        [Test]
        public void Test_codePointBefore_StringI()
        {

            assertEquals('a', Character.CodePointBefore((string)"abc", 1));
            assertEquals('b', Character.CodePointBefore((string)"abc", 2));
            assertEquals('c', Character.CodePointBefore((string)"abc", 3));
            assertEquals(0x10000, Character.CodePointBefore(
                    (string)"\uD800\uDC00", 2));
            assertEquals('\uD800', Character.CodePointBefore(
                    (string)"\uD800\uDC00", 1));

            try
            {
                Character.CodePointBefore((string)null, 0);
                fail("No NPE.");
            }
            catch (ArgumentNullException e)
            {
            }

            try
            {
                Character.CodePointBefore((string)"abc", 0);
                fail("No IOOBE, index below one.");
            }
            catch (ArgumentOutOfRangeException e)
            {
            }

            try
            {
                Character.CodePointBefore((string)"abc", 4);
                fail("No IOOBE, index too large.");
            }
            catch (ArgumentOutOfRangeException e)
            {
            }
        }





        [Test]
        public void Test_codePointBefore_CI()
        {

            assertEquals('a', Character.CodePointBefore("abc".ToCharArray(), 1));
            assertEquals('b', Character.CodePointBefore("abc".ToCharArray(), 2));
            assertEquals('c', Character.CodePointBefore("abc".ToCharArray(), 3));
            assertEquals(0x10000, Character.CodePointBefore("\uD800\uDC00"
                    .ToCharArray(), 2));
            assertEquals('\uD800', Character.CodePointBefore("\uD800\uDC00"
                    .ToCharArray(), 1));

            try
            {
                Character.CodePointBefore((char[])null, 0);
                fail("No NPE.");
            }
            catch (ArgumentNullException e)
            {
            }

            try
            {
                Character.CodePointBefore("abc".ToCharArray(), -1);
                fail("No IOOBE, negative index.");
            }
            catch (ArgumentOutOfRangeException e)
            {
            }

            try
            {
                Character.CodePointBefore("abc".ToCharArray(), 4);
                fail("No IOOBE, index too large.");
            }
            catch (ArgumentOutOfRangeException e)
            {
            }
        }

        [Test]
        public void Test_codePointBefore_CII()
        {

            assertEquals('a', Character.CodePointBefore("abc".ToCharArray(), 1, 0));
            assertEquals('b', Character.CodePointBefore("abc".ToCharArray(), 2, 0));
            assertEquals('c', Character.CodePointBefore("abc".ToCharArray(), 3, 0));
            assertEquals(0x10000, Character.CodePointBefore("\uD800\uDC00"
                    .ToCharArray(), 2, 0));
            assertEquals('\uDC00', Character.CodePointBefore("\uD800\uDC00"
                    .ToCharArray(), 2, 1));
            assertEquals('\uD800', Character.CodePointBefore("\uD800\uDC00"
                    .ToCharArray(), 1, 0));

            try
            {
                Character.CodePointBefore((char[])null, 1, 0);
                fail("No NPE.");
            }
            catch (ArgumentNullException e)
            {
            }

            try
            {
                Character.CodePointBefore("abc".ToCharArray(), 0, 1);
                fail("No IOOBE, index less than start.");
            }
            catch (ArgumentOutOfRangeException e)
            {
            }

            try
            {
                Character.CodePointBefore("abc".ToCharArray(), 4, 0);
                fail("No IOOBE, index larger than length.");
            }
            catch (ArgumentOutOfRangeException e)
            {
            }

            try
            {
                Character.CodePointBefore("abc".ToCharArray(), 2, -1);
                fail("No IOOBE, start is negative.");
            }
            catch (ArgumentOutOfRangeException e)
            {
            }

            try
            {
                Character.CodePointBefore("abc".ToCharArray(), 2, 4);
                fail("No IOOBE, start larger than length.");
            }
            catch (ArgumentOutOfRangeException e)
            {
            }
        }

        [Test]
        public void Test_toCharsI_CI()
        {
            char[] dst = new char[2];
            int result = Character.ToChars(0x10000, dst, 0);
            assertEquals(2, result);
            assertTrue(Arrays.Equals(new char[] { '\uD800', '\uDC00' }, dst));

            result = Character.ToChars(0x10001, dst, 0);
            assertEquals(2, result);
            assertTrue(Arrays.Equals(new char[] { '\uD800', '\uDC01' }, dst));

            result = Character.ToChars(0x10401, dst, 0);
            assertEquals(2, result);
            assertTrue(Arrays.Equals(new char[] { '\uD801', '\uDC01' }, dst));

            result = Character.ToChars(0x10FFFF, dst, 0);
            assertEquals(2, result);
            assertTrue(Arrays.Equals(new char[] { '\uDBFF', '\uDFFF' }, dst));

            try
            {
                Character.ToChars(int.MaxValue, new char[2], 0);
                fail("No IAE, invalid code point.");
            }
            catch (ArgumentException e)
            {
            }

            try
            {
                Character.ToChars('a', null, 0);
                fail("No NPE, null char[].");
            }
            catch (ArgumentNullException e)
            {
            }

            try
            {
                Character.ToChars('a', new char[1], -1);
                fail("No IOOBE, negative index.");
            }
            catch (ArgumentOutOfRangeException e)
            {
            }

            try
            {
                Character.ToChars('a', new char[1], 1);
                fail("No IOOBE, index equal to length.");
            }
            catch (ArgumentOutOfRangeException e)
            {
            }
        }

        [Test]
        public void Test_toCharsI()
        {
            assertTrue(Arrays.Equals(new char[] { '\uD800', '\uDC00' }, Character
                    .ToChars(0x10000)));
            assertTrue(Arrays.Equals(new char[] { '\uD800', '\uDC01' }, Character
                    .ToChars(0x10001)));
            assertTrue(Arrays.Equals(new char[] { '\uD801', '\uDC01' }, Character
                    .ToChars(0x10401)));
            assertTrue(Arrays.Equals(new char[] { '\uDBFF', '\uDFFF' }, Character
                    .ToChars(0x10FFFF)));

            try
            {
                Character.ToChars(int.MaxValue);
                fail("No IAE, invalid code point.");
            }
            catch (ArgumentException e)
            {
            }
        }

        [Test]
        public void Test_codePointCount_ICharSequenceII()
        {
            assertEquals(1, Character.CodePointCount((ICharSequence)"\uD800\uDC00".AsCharSequence(), 0, 2 - 0)); // end - start
            assertEquals(1, Character.CodePointCount((ICharSequence)"\uD800\uDC01".AsCharSequence(), 0, 2 - 0)); // end - start
            assertEquals(1, Character.CodePointCount((ICharSequence)"\uD801\uDC01".AsCharSequence(), 0, 2 - 0)); // end - start
            assertEquals(1, Character.CodePointCount((ICharSequence)"\uDBFF\uDFFF".AsCharSequence(), 0, 2 - 0)); // end - start

            assertEquals(3, Character.CodePointCount((ICharSequence)"a\uD800\uDC00b".AsCharSequence(), 0, 4 - 0)); // end - start
            assertEquals(4, Character.CodePointCount((ICharSequence)"a\uD800\uDC00b\uD800".AsCharSequence(), 0, 5 - 0)); // end - start

            try
            {
                Character.CodePointCount((ICharSequence)null, 0, 1 - 0); // end - start
                fail("No NPE, null char sequence.");
            }
            catch (ArgumentNullException e)
            {
            }

            try
            {
                Character.CodePointCount((ICharSequence)"abc".AsCharSequence(), -1, 1 - -1); // end - start
                fail("No IOOBE, negative start.");
            }
            catch (ArgumentOutOfRangeException e)
            {
            }

            try
            {
                Character.CodePointCount((ICharSequence)"abc".AsCharSequence(), 0, 4 - 0); // end - start
                fail("No IOOBE, end greater than length.");
            }
            catch (ArgumentOutOfRangeException e)
            {
            }

            try
            {
                Character.CodePointCount((ICharSequence)"abc".AsCharSequence(), 2, 1 - 2); // end - start
                fail("No IOOBE, end greater than start.");
            }
            catch (ArgumentOutOfRangeException e)
            {
            }
        }

        [Test]
        public void Test_codePointCount_CharArrayII()
        {
            assertEquals(1, Character.CodePointCount("\uD800\uDC00".ToCharArray(), 0, 2 - 0)); // end - start
            assertEquals(1, Character.CodePointCount("\uD800\uDC01".ToCharArray(), 0, 2 - 0)); // end - start
            assertEquals(1, Character.CodePointCount("\uD801\uDC01".ToCharArray(), 0, 2 - 0)); // end - start
            assertEquals(1, Character.CodePointCount("\uDBFF\uDFFF".ToCharArray(), 0, 2 - 0)); // end - start

            assertEquals(3, Character.CodePointCount("a\uD800\uDC00b".ToCharArray(), 0, 4 - 0)); // end - start
            assertEquals(4, Character.CodePointCount("a\uD800\uDC00b\uD800".ToCharArray(), 0, 5 - 0)); // end - start

            try
            {
                Character.CodePointCount((char[])null, 0, 1 - 0); // end - start
                fail("No NPE, null char sequence.");
            }
            catch (ArgumentNullException e)
            {
            }

            try
            {
                Character.CodePointCount("abc".ToCharArray(), -1, 1 - -1); // end - start
                fail("No IOOBE, negative start.");
            }
            catch (ArgumentOutOfRangeException e)
            {
            }

            try
            {
                Character.CodePointCount("abc".ToCharArray(), 0, 4 - 0); // end - start
                fail("No IOOBE, end greater than length.");
            }
            catch (ArgumentOutOfRangeException e)
            {
            }

            try
            {
                Character.CodePointCount("abc".ToCharArray(), 2, 1 - 2); // end - start
                fail("No IOOBE, end greater than start.");
            }
            catch (ArgumentOutOfRangeException e)
            {
            }
        }

        [Test]
        public void Test_codePointCount_StringBuilderII()
        {
            assertEquals(1, Character.CodePointCount(new StringBuilder("\uD800\uDC00"), 0, 2 - 0)); // end - start
            assertEquals(1, Character.CodePointCount(new StringBuilder("\uD800\uDC01"), 0, 2 - 0)); // end - start
            assertEquals(1, Character.CodePointCount(new StringBuilder("\uD801\uDC01"), 0, 2 - 0)); // end - start
            assertEquals(1, Character.CodePointCount(new StringBuilder("\uDBFF\uDFFF"), 0, 2 - 0)); // end - start

            assertEquals(3, Character.CodePointCount(new StringBuilder("a\uD800\uDC00b"), 0, 4 - 0)); // end - start
            assertEquals(4, Character.CodePointCount(new StringBuilder("a\uD800\uDC00b\uD800"), 0, 5 - 0)); // end - start

            try
            {
                Character.CodePointCount((StringBuilder)null, 0, 1 - 0); // end - start
                fail("No NPE, null char sequence.");
            }
            catch (ArgumentNullException e)
            {
            }

            try
            {
                Character.CodePointCount(new StringBuilder("abc"), -1, 1 - -1); // end - start
                fail("No IOOBE, negative start.");
            }
            catch (ArgumentOutOfRangeException e)
            {
            }

            try
            {
                Character.CodePointCount(new StringBuilder("abc"), 0, 4 - 0); // end - start
                fail("No IOOBE, end greater than length.");
            }
            catch (ArgumentOutOfRangeException e)
            {
            }

            try
            {
                Character.CodePointCount(new StringBuilder("abc"), 2, 1 - 2); // end - start
                fail("No IOOBE, end greater than start.");
            }
            catch (ArgumentOutOfRangeException e)
            {
            }
        }

        [Test]
        public void Test_codePointCount_StringII()
        {
            assertEquals(1, Character.CodePointCount((string)"\uD800\uDC00", 0, 2 - 0)); // end - start
            assertEquals(1, Character.CodePointCount((string)"\uD800\uDC01", 0, 2 - 0)); // end - start
            assertEquals(1, Character.CodePointCount((string)"\uD801\uDC01", 0, 2 - 0)); // end - start
            assertEquals(1, Character.CodePointCount((string)"\uDBFF\uDFFF", 0, 2 - 0)); // end - start

            assertEquals(3, Character.CodePointCount((string)"a\uD800\uDC00b", 0, 4 - 0)); // end - start
            assertEquals(4, Character.CodePointCount((string)"a\uD800\uDC00b\uD800", 0, 5 - 0)); // end - start

            try
            {
                Character.CodePointCount((string)null, 0, 1 - 0); // end - start
                fail("No NPE, null char sequence.");
            }
            catch (ArgumentNullException e)
            {
            }

            try
            {
                Character.CodePointCount((string)"abc", -1, 1 - -1); // end - start
                fail("No IOOBE, negative start.");
            }
            catch (ArgumentOutOfRangeException e)
            {
            }

            try
            {
                Character.CodePointCount((string)"abc", 0, 4 - 0); // end - start
                fail("No IOOBE, end greater than length.");
            }
            catch (ArgumentOutOfRangeException e)
            {
            }

            try
            {
                Character.CodePointCount((string)"abc", 2, 1 - 2); // end - start
                fail("No IOOBE, end greater than start.");
            }
            catch (ArgumentOutOfRangeException e)
            {
            }
        }

        [Test]
        public void Test_offsetByCodePoints_ICharSequenceII()
        {
            int result = Character.OffsetByCodePoints((ICharSequence)"a\uD800\uDC00b".AsCharSequence(), 0, 2);
            assertEquals(3, result);

            result = Character.OffsetByCodePoints((ICharSequence)"abcd".AsCharSequence(), 3, -1);
            assertEquals(2, result);

            result = Character.OffsetByCodePoints((ICharSequence)"a\uD800\uDC00b".AsCharSequence(), 0, 3);
            assertEquals(4, result);

            result = Character.OffsetByCodePoints((ICharSequence)"a\uD800\uDC00b".AsCharSequence(), 3, -1);
            assertEquals(1, result);

            result = Character.OffsetByCodePoints((ICharSequence)"a\uD800\uDC00b".AsCharSequence(), 3, 0);
            assertEquals(3, result);

            result = Character.OffsetByCodePoints((ICharSequence)"\uD800\uDC00bc".AsCharSequence(), 3, 0);
            assertEquals(3, result);

            result = Character.OffsetByCodePoints((ICharSequence)"a\uDC00bc".AsCharSequence(), 3, -1);
            assertEquals(2, result);

            result = Character.OffsetByCodePoints((ICharSequence)"a\uD800bc".AsCharSequence(), 3, -1);
            assertEquals(2, result);

            try
            {
                Character.OffsetByCodePoints((ICharSequence)null, 0, 1);
                fail();
            }
            catch (ArgumentNullException e)
            {
            }

            try
            {
                Character.OffsetByCodePoints((ICharSequence)"abc".AsCharSequence(), -1, 1);
                fail();
            }
            catch (ArgumentOutOfRangeException e)
            {
            }

            try
            {
                Character.OffsetByCodePoints((ICharSequence)"abc".AsCharSequence(), 4, 1);
                fail();
            }
            catch (ArgumentOutOfRangeException e)
            {
            }

            try
            {
                Character.OffsetByCodePoints((ICharSequence)"abc".AsCharSequence(), 1, 3);
                fail();
            }
            catch (ArgumentOutOfRangeException e)
            {
            }

            try
            {
                Character.OffsetByCodePoints((ICharSequence)"abc".AsCharSequence(), 1, -2);
                fail();
            }
            catch (ArgumentOutOfRangeException e)
            {
            }
        }

        [Test]
        public void Test_offsetByCodePoints_CharArrayII()
        {
            int result = Character.OffsetByCodePoints((char[])"a\uD800\uDC00b".ToCharArray(), 0, 2);
            assertEquals(3, result);

            result = Character.OffsetByCodePoints((char[])"abcd".ToCharArray(), 3, -1);
            assertEquals(2, result);

            result = Character.OffsetByCodePoints((char[])"a\uD800\uDC00b".ToCharArray(), 0, 3);
            assertEquals(4, result);

            result = Character.OffsetByCodePoints((char[])"a\uD800\uDC00b".ToCharArray(), 3, -1);
            assertEquals(1, result);

            result = Character.OffsetByCodePoints((char[])"a\uD800\uDC00b".ToCharArray(), 3, 0);
            assertEquals(3, result);

            result = Character.OffsetByCodePoints((char[])"\uD800\uDC00bc".ToCharArray(), 3, 0);
            assertEquals(3, result);

            result = Character.OffsetByCodePoints((char[])"a\uDC00bc".ToCharArray(), 3, -1);
            assertEquals(2, result);

            result = Character.OffsetByCodePoints((char[])"a\uD800bc".ToCharArray(), 3, -1);
            assertEquals(2, result);

            try
            {
                Character.OffsetByCodePoints((char[])null, 0, 1);
                fail();
            }
            catch (ArgumentNullException e)
            {
            }

            try
            {
                Character.OffsetByCodePoints((char[])"abc".ToCharArray(), -1, 1);
                fail();
            }
            catch (ArgumentOutOfRangeException e)
            {
            }

            try
            {
                Character.OffsetByCodePoints((char[])"abc".ToCharArray(), 4, 1);
                fail();
            }
            catch (ArgumentOutOfRangeException e)
            {
            }

            try
            {
                Character.OffsetByCodePoints((char[])"abc".ToCharArray(), 1, 3);
                fail();
            }
            catch (ArgumentOutOfRangeException e)
            {
            }

            try
            {
                Character.OffsetByCodePoints((char[])"abc".ToCharArray(), 1, -2);
                fail();
            }
            catch (ArgumentOutOfRangeException e)
            {
            }
        }

        [Test]
        public void Test_offsetByCodePoints_StringBuilderII()
        {
            int result = Character.OffsetByCodePoints((StringBuilder)new StringBuilder("a\uD800\uDC00b"), 0, 2);
            assertEquals(3, result);

            result = Character.OffsetByCodePoints((StringBuilder)new StringBuilder("abcd"), 3, -1);
            assertEquals(2, result);

            result = Character.OffsetByCodePoints((StringBuilder)new StringBuilder("a\uD800\uDC00b"), 0, 3);
            assertEquals(4, result);

            result = Character.OffsetByCodePoints((StringBuilder)new StringBuilder("a\uD800\uDC00b"), 3, -1);
            assertEquals(1, result);

            result = Character.OffsetByCodePoints((StringBuilder)new StringBuilder("a\uD800\uDC00b"), 3, 0);
            assertEquals(3, result);

            result = Character.OffsetByCodePoints((StringBuilder)new StringBuilder("\uD800\uDC00bc"), 3, 0);
            assertEquals(3, result);

            result = Character.OffsetByCodePoints((StringBuilder)new StringBuilder("a\uDC00bc"), 3, -1);
            assertEquals(2, result);

            result = Character.OffsetByCodePoints((StringBuilder)new StringBuilder("a\uD800bc"), 3, -1);
            assertEquals(2, result);

            try
            {
                Character.OffsetByCodePoints((StringBuilder)null, 0, 1);
                fail();
            }
            catch (ArgumentNullException e)
            {
            }

            try
            {
                Character.OffsetByCodePoints((StringBuilder)new StringBuilder("abc"), -1, 1);
                fail();
            }
            catch (ArgumentOutOfRangeException e)
            {
            }

            try
            {
                Character.OffsetByCodePoints((StringBuilder)new StringBuilder("abc"), 4, 1);
                fail();
            }
            catch (ArgumentOutOfRangeException e)
            {
            }

            try
            {
                Character.OffsetByCodePoints((StringBuilder)new StringBuilder("abc"), 1, 3);
                fail();
            }
            catch (ArgumentOutOfRangeException e)
            {
            }

            try
            {
                Character.OffsetByCodePoints((StringBuilder)new StringBuilder("abc"), 1, -2);
                fail();
            }
            catch (ArgumentOutOfRangeException e)
            {
            }
        }

        [Test]
        public void Test_offsetByCodePoints_StringII()
        {
            int result = Character.OffsetByCodePoints((string)"a\uD800\uDC00b", 0, 2);
            assertEquals(3, result);

            result = Character.OffsetByCodePoints((string)"abcd", 3, -1);
            assertEquals(2, result);

            result = Character.OffsetByCodePoints((string)"a\uD800\uDC00b", 0, 3);
            assertEquals(4, result);

            result = Character.OffsetByCodePoints((string)"a\uD800\uDC00b", 3, -1);
            assertEquals(1, result);

            result = Character.OffsetByCodePoints((string)"a\uD800\uDC00b", 3, 0);
            assertEquals(3, result);

            result = Character.OffsetByCodePoints((string)"\uD800\uDC00bc", 3, 0);
            assertEquals(3, result);

            result = Character.OffsetByCodePoints((string)"a\uDC00bc", 3, -1);
            assertEquals(2, result);

            result = Character.OffsetByCodePoints((string)"a\uD800bc", 3, -1);
            assertEquals(2, result);

            try
            {
                Character.OffsetByCodePoints((string)null, 0, 1);
                fail();
            }
            catch (ArgumentNullException e)
            {
            }

            try
            {
                Character.OffsetByCodePoints((string)"abc", -1, 1);
                fail();
            }
            catch (ArgumentOutOfRangeException e)
            {
            }

            try
            {
                Character.OffsetByCodePoints((string)"abc", 4, 1);
                fail();
            }
            catch (ArgumentOutOfRangeException e)
            {
            }

            try
            {
                Character.OffsetByCodePoints((string)"abc", 1, 3);
                fail();
            }
            catch (ArgumentOutOfRangeException e)
            {
            }

            try
            {
                Character.OffsetByCodePoints((string)"abc", 1, -2);
                fail();
            }
            catch (ArgumentOutOfRangeException e)
            {
            }
        }



        [Test]
        public void Test_offsetByCodePoints_CIIII()
        {
            int result = Character.OffsetByCodePoints("a\uD800\uDC00b"
                    .ToCharArray(), 0, 4, 0, 2);
            assertEquals(3, result);

            result = Character.OffsetByCodePoints("a\uD800\uDC00b".ToCharArray(),
                    0, 4, 0, 3);
            assertEquals(4, result);

            result = Character.OffsetByCodePoints("a\uD800\uDC00b\uD800c"
                    .ToCharArray(), 0, 5, 0, 3);
            assertEquals(4, result);

            result = Character
                    .OffsetByCodePoints("abcd".ToCharArray(), 0, 4, 3, -1);
            assertEquals(2, result);

            result = Character
                    .OffsetByCodePoints("abcd".ToCharArray(), 1, 2, 3, -2);
            assertEquals(1, result);

            result = Character.OffsetByCodePoints("a\uD800\uDC00b".ToCharArray(),
                    0, 4, 3, -1);
            assertEquals(1, result);

            result = Character.OffsetByCodePoints("a\uD800\uDC00b".ToCharArray(),
                    0, 2, 2, -1);
            assertEquals(1, result);

            result = Character.OffsetByCodePoints("a\uD800\uDC00b".ToCharArray(),
                    0, 4, 3, 0);
            assertEquals(3, result);

            result = Character.OffsetByCodePoints("\uD800\uDC00bc".ToCharArray(),
                    0, 4, 3, 0);
            assertEquals(3, result);

            result = Character.OffsetByCodePoints("a\uDC00bc".ToCharArray(), 0, 4,
                    3, -1);
            assertEquals(2, result);

            result = Character.OffsetByCodePoints("a\uD800bc".ToCharArray(), 0, 4,
                    3, -1);
            assertEquals(2, result);

            try
            {
                Character.OffsetByCodePoints(null, 0, 4, 1, 1);
                fail();
            }
            catch (ArgumentNullException e)
            {
            }

            try
            {
                Character.OffsetByCodePoints("abcd".ToCharArray(), -1, 4, 1, 1);
                fail();
            }
            catch (ArgumentOutOfRangeException e)
            {
            }

            try
            {
                Character.OffsetByCodePoints("abcd".ToCharArray(), 0, -1, 1, 1);
                fail();
            }
            catch (ArgumentOutOfRangeException e)
            {
            }

            try
            {
                Character.OffsetByCodePoints("abcd".ToCharArray(), 2, 4, 1, 1);
                fail();
            }
            catch (ArgumentOutOfRangeException e)
            {
            }

            try
            {
                Character.OffsetByCodePoints("abcd".ToCharArray(), 1, 3, 0, 1);
                fail();
            }
            catch (ArgumentOutOfRangeException e)
            {
            }

            try
            {
                Character.OffsetByCodePoints("abcd".ToCharArray(), 1, 1, 3, 1);
                fail();
            }
            catch (ArgumentOutOfRangeException e)
            {
            }

            try
            {
                Character.OffsetByCodePoints("abc".ToCharArray(), 0, 3, 1, 3);
                fail();
            }
            catch (ArgumentOutOfRangeException e)
            {
            }

            try
            {
                Character.OffsetByCodePoints("abc".ToCharArray(), 0, 2, 1, 2);
                fail();
            }
            catch (ArgumentOutOfRangeException e)
            {
            }

            try
            {
                Character.OffsetByCodePoints("abc".ToCharArray(), 1, 3, 1, -2);
                fail();
            }
            catch (ArgumentOutOfRangeException e)
            {
            }
        }

        /////**
        //// * @tests java.lang.Character#compareTo(Character)
        //// */
        ////[Test]
        ////public void Test_compareToLjava_lang_Byte()
        ////{
        ////    Character min = new Character(Character.MIN_VALUE);
        ////    Character mid = new Character((char)(Character.MAX_VALUE / 2));
        ////    Character max = new Character(Character.MAX_VALUE);

        ////    assertTrue(max.compareTo(max) == 0);
        ////    assertTrue(min.compareTo(min) == 0);
        ////    assertTrue(mid.compareTo(mid) == 0);

        ////    assertTrue(max.compareTo(mid) > 0);
        ////    assertTrue(max.compareTo(min) > 0);

        ////    assertTrue(mid.compareTo(max) < 0);
        ////    assertTrue(mid.compareTo(min) > 0);

        ////    assertTrue(min.compareTo(mid) < 0);
        ////    assertTrue(min.compareTo(max) < 0);

        ////    try
        ////    {
        ////        min.compareTo(null);
        ////        fail("No NPE");
        ////    }
        ////    catch (ArgumentNullException e)
        ////    {
        ////    }
        ////}

        [Test]
        public void Test_codePointAt_Invalid()
        {

            try
            {
                Character.CodePointAt("abc".ToCharArray(), 6, 4);
                fail("Expected ArgumentOutOfRangeException");
            }
            catch (ArgumentOutOfRangeException e)
            {
                // expected
            }

            try
            {
                Character.CodePointAt(null, 4, 6);
                fail("Expected ArgumentNullException");
            }
            catch (ArgumentNullException e)
            {
                // expected
            }

            try
            {
                Character.CodePointAt("abc".ToCharArray(), 0, 0);
                fail("Expected ArgumentOutOfRangeException");
            }
            catch (ArgumentOutOfRangeException e)
            {
                // expected
            }
        }

        /////**
        //// * @tests java.lang.Character#Character(char)
        //// */
        ////[Test]
        ////public void Test_ConstructorC()
        ////{
        ////    assertEquals("Constructor failed", 'T', new Character('T').charValue());
        ////}

        /////**
        //// * @tests java.lang.Character#charValue()
        //// */
        ////[Test]
        ////public void Test_charValue()
        ////{
        ////    assertEquals("Incorrect char value returned", 'T', new Character('T')
        ////            .charValue());
        ////}

        /////**
        //// * @tests java.lang.Character#compareTo(java.lang.Character)
        //// */
        ////[Test]
        ////public void Test_compareToLjava_lang_Character()
        ////{
        ////    Character c = new Character('c');
        ////    Character x = new Character('c');
        ////    Character y = new Character('b');
        ////    Character z = new Character('d');

        ////    assertEquals("Returned false for same Character", 0, c.compareTo(c));
        ////    assertEquals("Returned false for identical Character",
        ////            0, c.compareTo(x));
        ////    assertTrue("Returned other than less than for lesser char", c
        ////            .compareTo(y) > 0);
        ////    assertTrue("Returned other than greater than for greater char", c
        ////            .compareTo(z) < 0);
        ////}

        /**
         * @tests java.lang.Character#digit(char, int)
         */
        [Test]
        public void Test_digitCI()
        {
            assertEquals("Returned incorrect digit", 1, Character.Digit('1', 10));
            assertEquals("Returned incorrect digit", 15, Character.Digit('F', 16));
        }

        /////**
        //// * @tests java.lang.Character#digit(int, int)
        //// */
        ////[Test]
        ////public void Test_digit_II()
        ////{
        ////    assertEquals(1, Character.Digit((int)'1', 10));
        ////    assertEquals(15, Character.Digit((int)'F', 16));

        ////    assertEquals(-1, Character.Digit(0x0000, 37));
        ////    assertEquals(-1, Character.Digit(0x0045, 10));

        ////    assertEquals(10, Character.Digit(0x0041, 20));
        ////    assertEquals(10, Character.Digit(0x0061, 20));

        ////    assertEquals(-1, Character.Digit(0x110000, 20));
        ////}

        /////**
        //// * @tests java.lang.Character#equals(java.lang.Object)
        //// */
        ////[Test]
        ////public void Test_equalsLjava_lang_Object()
        ////{
        ////    // Test for method boolean java.lang.Character.equals(java.lang.Object)
        ////    assertTrue("Equality test failed", new Character('A')
        ////            .Equals(new Character('A')));
        ////    assertTrue("Equality test failed", !(new Character('A')
        ////            .Equals(new Character('a'))));
        ////}

        /**
         * @tests java.lang.Character#forDigit(int, int)
         */
        [Test]
        public void Test_forDigitII()
        {
            char[] hexChars = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
                'a', 'b', 'c', 'd', 'e', 'f' };
            for (int i = 0; i < hexChars.Length; i++)
            {
                assertTrue("Returned incorrect char for " + i,
                        Character.ForDigit(i, hexChars.Length) == hexChars[i]);
            }

            char[] decimalChars = { '0', '1', '2', '3', '4', '5', '6', '7', '8',
                '9' };
            for (int i = 0; i < decimalChars.Length; i++)
            {
                assertTrue(
                        "Returned incorrect char for " + i,
                        Character.ForDigit(i, decimalChars.Length) == decimalChars[i]);
            }

        }

        /**
         * @tests java.lang.Character#getNumericValue(char)
         */
        [Test]
        public void Test_getNumericValueC()
        {
            assertEquals("Returned incorrect numeric value 1", 1, Character
                    .GetNumericValue('1'));
            assertEquals("Returned incorrect numeric value 2", 15, Character
                    .GetNumericValue('F'));
            assertEquals("Returned incorrect numeric value 3", -1, Character
                    .GetNumericValue('\u221e'));
            assertEquals("Returned incorrect numeric value 4", -2, Character
                    .GetNumericValue('\u00be'));
            assertEquals("Returned incorrect numeric value 5", 10000, Character
                    .GetNumericValue('\u2182'));
            assertEquals("Returned incorrect numeric value 6", 2, Character
                    .GetNumericValue('\uff12'));
        }

        /////**
        //// * @tests java.lang.Character#getNumericValue(int)
        //// */
        ////[Test]
        ////public void Test_getNumericValue_I()
        ////{
        ////    assertEquals(1, Character.GetNumericValue((int)'1'));
        ////    assertEquals(15, Character.GetNumericValue((int)'F'));
        ////    assertEquals(-1, Character.GetNumericValue((int)'\u221e'));
        ////    assertEquals(-2, Character.GetNumericValue((int)'\u00be'));
        ////    assertEquals(10000, Character.GetNumericValue((int)'\u2182'));
        ////    assertEquals(2, Character.GetNumericValue((int)'\uff12'));
        ////    assertEquals(-1, Character.GetNumericValue(0xFFFF));

        ////    assertEquals(-1, Character.GetNumericValue(0xFFFF));
        ////    assertEquals(0, Character.GetNumericValue(0x1D7CE));
        ////    assertEquals(0, Character.GetNumericValue(0x1D7D8));
        ////    assertEquals(-1, Character.GetNumericValue(0x2F800));
        ////    assertEquals(-1, Character.GetNumericValue(0x10FFFD));
        ////    assertEquals(-1, Character.GetNumericValue(0x110000));

        ////    assertEquals(50, Character.GetNumericValue(0x216C));

        ////    assertEquals(10, Character.GetNumericValue(0x0041));
        ////    assertEquals(35, Character.GetNumericValue(0x005A));
        ////    assertEquals(10, Character.GetNumericValue(0x0061));
        ////    assertEquals(35, Character.GetNumericValue(0x007A));
        ////    assertEquals(10, Character.GetNumericValue(0xFF21));

        ////    //FIXME depends on ICU4J
        ////    //assertEquals(35, Character.GetNumericValue(0xFF3A));

        ////    assertEquals(10, Character.GetNumericValue(0xFF41));
        ////    assertEquals(35, Character.GetNumericValue(0xFF5A));
        ////}

        /**
         * @tests java.lang.Character#getType(char)
         */
        [Test]
        public void Test_getTypeC()
        {
            assertTrue("Returned incorrect type for: \n",
                    Character.GetType('\n') == UnicodeCategory.Control);
            assertTrue("Returned incorrect type for: 1",
                    Character.GetType('1') == UnicodeCategory.DecimalDigitNumber);
            assertTrue("Returned incorrect type for: ' '",
                    Character.GetType(' ') == UnicodeCategory.SpaceSeparator);
            assertTrue("Returned incorrect type for: a",
                    Character.GetType('a') == UnicodeCategory.LowercaseLetter);
            assertTrue("Returned incorrect type for: A",
                    Character.GetType('A') == UnicodeCategory.UppercaseLetter);
            assertTrue("Returned incorrect type for: <",
                    Character.GetType('<') == UnicodeCategory.MathSymbol);
            assertTrue("Returned incorrect type for: ;",
                    Character.GetType(';') == UnicodeCategory.OtherPunctuation);
            assertTrue("Returned incorrect type for: _",
                    Character.GetType('_') == UnicodeCategory.ConnectorPunctuation);
            assertTrue("Returned incorrect type for: $",
                    Character.GetType('$') == UnicodeCategory.CurrencySymbol);
            assertTrue("Returned incorrect type for: \u2029", Character
                    .GetType('\u2029') == UnicodeCategory.ParagraphSeparator);

            //assertEquals("Wrong constant for FORMAT", 15, UnicodeCategory.Format);
            //assertEquals("Wrong constant for PRIVATE_USE",
            //        17, UnicodeCategory.PrivateUse);
        }

        /**
         * @tests java.lang.Character#getType(int)
         */
        [Test]
        public void Test_getType_I()
        {
            assertTrue(Character.GetType((int)'\n') == UnicodeCategory.Control);
            assertTrue(Character.GetType((int)'1') == UnicodeCategory.DecimalDigitNumber);
            assertTrue(Character.GetType((int)' ') == UnicodeCategory.SpaceSeparator);
            assertTrue(Character.GetType((int)'a') == UnicodeCategory.LowercaseLetter);
            assertTrue(Character.GetType((int)'A') == UnicodeCategory.UppercaseLetter);
            assertTrue(Character.GetType((int)'<') == UnicodeCategory.MathSymbol);
            assertTrue(Character.GetType((int)';') == UnicodeCategory.OtherPunctuation);
            assertTrue(Character.GetType((int)'_') == UnicodeCategory.ConnectorPunctuation);
            assertTrue(Character.GetType((int)'$') == UnicodeCategory.CurrencySymbol);
            assertTrue(Character.GetType((int)'\u2029') == UnicodeCategory.ParagraphSeparator);

            assertTrue(Character.GetType(0x9FFF) == UnicodeCategory.OtherNotAssigned);
            assertTrue(Character.GetType(0x30000) == UnicodeCategory.OtherNotAssigned);
            assertTrue(Character.GetType(0x110000) == UnicodeCategory.OtherNotAssigned);

            assertTrue(Character.GetType(0x0041) == UnicodeCategory.UppercaseLetter);
            assertTrue(Character.GetType(0x10400) == UnicodeCategory.UppercaseLetter);

            assertTrue(Character.GetType(0x0061) == UnicodeCategory.LowercaseLetter);
            assertTrue(Character.GetType(0x10428) == UnicodeCategory.LowercaseLetter);

            assertTrue(Character.GetType(0x01C5) == UnicodeCategory.TitlecaseLetter);
            assertTrue(Character.GetType(0x1FFC) == UnicodeCategory.TitlecaseLetter);

            assertTrue(Character.GetType(0x02B0) == UnicodeCategory.ModifierLetter);
            assertTrue(Character.GetType(0xFF9F) == UnicodeCategory.ModifierLetter);

            assertTrue(Character.GetType(0x01BB) == UnicodeCategory.OtherLetter);
            assertTrue(Character.GetType(0x2F888) == UnicodeCategory.OtherLetter);

            assertTrue(Character.GetType(0x0F82) == UnicodeCategory.NonSpacingMark);
            assertTrue(Character.GetType(0x1D180) == UnicodeCategory.NonSpacingMark);

            assertTrue(Character.GetType(0x0488) == UnicodeCategory.EnclosingMark);
            assertTrue(Character.GetType(0x20DE) == UnicodeCategory.EnclosingMark);

            assertTrue(Character.GetType(0x1938) == UnicodeCategory.SpacingCombiningMark);
            assertTrue(Character.GetType(0x1D165) == UnicodeCategory.SpacingCombiningMark);

            assertTrue(Character.GetType(0x194D) == UnicodeCategory.DecimalDigitNumber);
            assertTrue(Character.GetType(0x1D7CE) == UnicodeCategory.DecimalDigitNumber);

            assertTrue(Character.GetType(0x2160) == UnicodeCategory.LetterNumber);
            assertTrue(Character.GetType(0x1034A) == UnicodeCategory.LetterNumber);

            assertTrue(Character.GetType(0x00B2) == UnicodeCategory.OtherNumber);
            assertTrue(Character.GetType(0x10120) == UnicodeCategory.OtherNumber);

            assertTrue(Character.GetType(0x0020) == UnicodeCategory.SpaceSeparator);
            assertTrue(Character.GetType(0x3000) == UnicodeCategory.SpaceSeparator);

            assertTrue(Character.GetType(0x2028) == UnicodeCategory.LineSeparator);

            assertTrue(Character.GetType(0x2029) == UnicodeCategory.ParagraphSeparator);

            assertTrue(Character.GetType(0x0000) == UnicodeCategory.Control);
            assertTrue(Character.GetType(0x009F) == UnicodeCategory.Control);

            assertTrue(Character.GetType(0x00AD) == UnicodeCategory.Format);
            assertTrue(Character.GetType(0xE007F) == UnicodeCategory.Format);

            assertTrue(Character.GetType(0xE000) == UnicodeCategory.PrivateUse);
            assertTrue(Character.GetType(0x10FFFD) == UnicodeCategory.PrivateUse);

            assertTrue(Character.GetType(0xD800) == UnicodeCategory.Surrogate);
            assertTrue(Character.GetType(0xDFFF) == UnicodeCategory.Surrogate);

            assertTrue(Character.GetType(0xFE31) == UnicodeCategory.DashPunctuation);
            assertTrue(Character.GetType(0xFF0D) == UnicodeCategory.DashPunctuation);

            assertTrue(Character.GetType(0x0028) == UnicodeCategory.OpenPunctuation);
            assertTrue(Character.GetType(0xFF62) == UnicodeCategory.OpenPunctuation);

            assertTrue(Character.GetType(0x0029) == UnicodeCategory.ClosePunctuation);
            assertTrue(Character.GetType(0xFF63) == UnicodeCategory.ClosePunctuation);

            assertTrue(Character.GetType(0x005F) == UnicodeCategory.ConnectorPunctuation);
            assertTrue(Character.GetType(0xFF3F) == UnicodeCategory.ConnectorPunctuation);

            assertTrue(Character.GetType(0x2034) == UnicodeCategory.OtherPunctuation);
            assertTrue(Character.GetType(0x1039F) == UnicodeCategory.OtherPunctuation);

            assertTrue(Character.GetType(0x002B) == UnicodeCategory.MathSymbol);
            assertTrue(Character.GetType(0x1D6C1) == UnicodeCategory.MathSymbol);

            assertTrue(Character.GetType(0x0024) == UnicodeCategory.CurrencySymbol);
            assertTrue(Character.GetType(0xFFE6) == UnicodeCategory.CurrencySymbol);

            assertTrue(Character.GetType(0x005E) == UnicodeCategory.ModifierSymbol);
            assertTrue(Character.GetType(0xFFE3) == UnicodeCategory.ModifierSymbol);

            assertTrue(Character.GetType(0x00A6) == UnicodeCategory.OtherSymbol);
            assertTrue(Character.GetType(0x1D356) == UnicodeCategory.OtherSymbol);

            assertTrue(Character.GetType(0x00AB) == UnicodeCategory.InitialQuotePunctuation);
            assertTrue(Character.GetType(0x2039) == UnicodeCategory.InitialQuotePunctuation);

            assertTrue(Character.GetType(0x00BB) == UnicodeCategory.FinalQuotePunctuation);
            assertTrue(Character.GetType(0x203A) == UnicodeCategory.FinalQuotePunctuation);
        }

        /////**
        //// * @tests java.lang.Character#hashCode()
        //// */
        ////[Test]
        ////public void Test_hashCode()
        ////{
        ////    assertEquals("Incorrect hash returned",
        ////            89, new Character('Y').hashCode());
        ////}

        /**
         * @tests java.lang.Character#isDefined(char)
         */
        [Test]
        public void Test_isDefinedC()
        {
            assertTrue("Defined character returned false", Character.IsDefined('v'));
            assertTrue("Defined character returned false", Character
                    .IsDefined('\u6039'));
        }

        /**
         * @tests java.lang.Character#isDefined(int)
         */
        [Test]
        public void Test_isDefined_I()
        {
            assertTrue(Character.IsDefined((int)'v'));
            assertTrue(Character.IsDefined((int)'\u6039'));
            assertTrue(Character.IsDefined(0x10300));

            assertFalse(Character.IsDefined(0x30000));
            assertFalse(Character.IsDefined(0x3FFFF));
            assertFalse(Character.IsDefined(0x110000));
        }

        /**
         * @tests java.lang.Character#isDigit(char)
         */
        [Test]
        public void Test_isDigitC()
        {
            assertTrue("Digit returned false", Character.IsDigit('1'));
            assertTrue("Non-Digit returned false", !Character.IsDigit('A'));
        }

        /**
         * @tests java.lang.Character#isDigit(int)
         */
        [Test]
        public void Test_isDigit_I()
        {
            assertTrue(Character.IsDigit((int)'1'));
            assertFalse(Character.IsDigit((int)'A'));

            assertTrue(Character.IsDigit(0x0030));
            assertTrue(Character.IsDigit(0x0035));
            assertTrue(Character.IsDigit(0x0039));

            assertTrue(Character.IsDigit(0x0660));
            assertTrue(Character.IsDigit(0x0665));
            assertTrue(Character.IsDigit(0x0669));

            assertTrue(Character.IsDigit(0x06F0));
            assertTrue(Character.IsDigit(0x06F5));
            assertTrue(Character.IsDigit(0x06F9));

            assertTrue(Character.IsDigit(0x0966));
            assertTrue(Character.IsDigit(0x096A));
            assertTrue(Character.IsDigit(0x096F));

            assertTrue(Character.IsDigit(0xFF10));
            assertTrue(Character.IsDigit(0xFF15));
            assertTrue(Character.IsDigit(0xFF19));

            assertTrue(Character.IsDigit(0x1D7CE));
            assertTrue(Character.IsDigit(0x1D7D8));

            assertFalse(Character.IsDigit(0x2F800));
            assertFalse(Character.IsDigit(0x10FFFD));
            assertFalse(Character.IsDigit(0x110000));
        }

        /**
         * @tests java.lang.Character#isIdentifierIgnorable(char)
         */
        [Test]
        public void Test_isIdentifierIgnorableC()
        {
            assertTrue("Ignorable whitespace returned false", Character
                    .IsIdentifierIgnorable('\u0007'));
            assertTrue("Ignorable non - whitespace  control returned false",
                    Character.IsIdentifierIgnorable('\u000f'));
            assertTrue("Ignorable join control returned false", Character
                    .IsIdentifierIgnorable('\u200e'));

            // the spec is wrong, and our implementation is correct
            assertTrue("Ignorable bidi control returned false", Character
                    .IsIdentifierIgnorable('\u202b'));

            assertTrue("Ignorable format control returned false", Character
                    .IsIdentifierIgnorable('\u206c'));
            assertTrue("Ignorable zero-width no-break returned false", Character
                    .IsIdentifierIgnorable('\ufeff'));

            assertTrue("Non-Ignorable returned true", !Character
                    .IsIdentifierIgnorable('\u0065'));
        }

        /**
         * @tests java.lang.Character#isIdentifierIgnorable(int)
         */
        [Test]
        public void Test_isIdentifierIgnorable_I()
        {
            assertTrue(Character.IsIdentifierIgnorable(0x0000));
            assertTrue(Character.IsIdentifierIgnorable(0x0004));
            assertTrue(Character.IsIdentifierIgnorable(0x0008));

            assertTrue(Character.IsIdentifierIgnorable(0x000E));
            assertTrue(Character.IsIdentifierIgnorable(0x0013));
            assertTrue(Character.IsIdentifierIgnorable(0x001B));

            assertTrue(Character.IsIdentifierIgnorable(0x007F));
            assertTrue(Character.IsIdentifierIgnorable(0x008F));
            assertTrue(Character.IsIdentifierIgnorable(0x009F));

            assertTrue(Character.IsIdentifierIgnorable(0x202b));
            assertTrue(Character.IsIdentifierIgnorable(0x206c));
            assertTrue(Character.IsIdentifierIgnorable(0xfeff));
            assertFalse(Character.IsIdentifierIgnorable(0x0065));

            assertTrue(Character.IsIdentifierIgnorable(0x1D173));

            assertFalse(Character.IsIdentifierIgnorable(0x10FFFD));
            assertFalse(Character.IsIdentifierIgnorable(0x110000));
        }

        /////**
        //// * @tests java.lang.Character#isMirrored(char)
        //// */
        ////[Test]
        ////public void Test_isMirrored_C()
        ////{
        ////    assertTrue(Character.IsMirrored('\u0028'));
        ////    assertFalse(Character.IsMirrored('\uFFFF'));
        ////}

        /////**
        //// * @tests java.lang.Character#isMirrored(int)
        //// */
        ////[Test]
        ////public void Test_isMirrored_I()
        ////{
        ////    assertTrue(Character.IsMirrored(0x0028));
        ////    assertFalse(Character.IsMirrored(0xFFFF));
        ////    assertFalse(Character.IsMirrored(0x110000));
        ////}

        /**
         * @tests java.lang.Character#isISOControl(char)
         */
        [Test]
        public void Test_isISOControlC()
        {
            // Test for method boolean java.lang.Character.IsISOControl(char)
            for (int i = 0; i < 32; i++)
                assertTrue("ISOConstrol char returned false", Character
                        .IsISOControl((char)i));

            for (int i = 127; i < 160; i++)
                assertTrue("ISOConstrol char returned false", Character
                        .IsISOControl((char)i));
        }

        /**
         * @tests java.lang.Character#isISOControl(int)
         */
        [Test]
        public void Test_isISOControlI()
        {
            // Test for method boolean java.lang.Character.IsISOControl(char)
            for (int i = 0; i < 32; i++)
                assertTrue("ISOConstrol char returned false", Character
                    .IsISOControl(i));

            for (int i = 127; i < 160; i++)
                assertTrue("ISOConstrol char returned false", Character
                    .IsISOControl(i));

            for (int i = 160; i < 260; i++)
                assertFalse("Not ISOConstrol char returned true", Character
                    .IsISOControl(i));

        }


        //    /**
        //     * @tests java.lang.Character#isJavaIdentifierPart(char)
        //     */
        //[Test]
        //    public void Test_isJavaIdentifierPartC()
        //    {
        //        assertTrue("letter returned false", Character.IsJavaIdentifierPart('l'));
        //        assertTrue("currency returned false", Character
        //                .isJavaIdentifierPart('$'));
        //        assertTrue("digit returned false", Character.IsJavaIdentifierPart('9'));
        //        assertTrue("connecting char returned false", Character
        //                .isJavaIdentifierPart('_'));
        //        assertTrue("ignorable control returned true", !Character
        //                .isJavaIdentifierPart('\u200b'));
        //        assertTrue("semi returned true", !Character.IsJavaIdentifierPart(';'));
        //    }

        //    /**
        //     * @tests java.lang.Character#isJavaIdentifierPart(int)
        //     */
        //[Test]
        //    public void Test_isJavaIdentifierPart_I()
        //    {
        //        assertTrue(Character.IsJavaIdentifierPart((int)'l'));
        //        assertTrue(Character.IsJavaIdentifierPart((int)'$'));
        //        assertTrue(Character.IsJavaIdentifierPart((int)'9'));
        //        assertTrue(Character.IsJavaIdentifierPart((int)'_'));
        //        assertFalse(Character.IsJavaIdentifierPart((int)';'));

        //        assertTrue(Character.IsJavaIdentifierPart(0x0041));
        //        assertTrue(Character.IsJavaIdentifierPart(0x10400));
        //        assertTrue(Character.IsJavaIdentifierPart(0x0061));
        //        assertTrue(Character.IsJavaIdentifierPart(0x10428));
        //        assertTrue(Character.IsJavaIdentifierPart(0x01C5));
        //        assertTrue(Character.IsJavaIdentifierPart(0x1FFC));
        //        assertTrue(Character.IsJavaIdentifierPart(0x02B0));
        //        assertTrue(Character.IsJavaIdentifierPart(0xFF9F));
        //        assertTrue(Character.IsJavaIdentifierPart(0x01BB));
        //        assertTrue(Character.IsJavaIdentifierPart(0x2F888));

        //        assertTrue(Character.IsJavaIdentifierPart(0x0024));
        //        assertTrue(Character.IsJavaIdentifierPart(0xFFE6));

        //        assertTrue(Character.IsJavaIdentifierPart(0x005F));
        //        assertTrue(Character.IsJavaIdentifierPart(0xFF3F));

        //        assertTrue(Character.IsJavaIdentifierPart(0x194D));
        //        assertTrue(Character.IsJavaIdentifierPart(0x1D7CE));
        //        assertTrue(Character.IsJavaIdentifierPart(0x2160));
        //        assertTrue(Character.IsJavaIdentifierPart(0x1034A));

        //        assertTrue(Character.IsJavaIdentifierPart(0x0F82));
        //        assertTrue(Character.IsJavaIdentifierPart(0x1D180));

        //        assertTrue(Character.IsJavaIdentifierPart(0x0000));
        //        assertTrue(Character.IsJavaIdentifierPart(0x0008));
        //        assertTrue(Character.IsJavaIdentifierPart(0x000E));
        //        assertTrue(Character.IsJavaIdentifierPart(0x001B));
        //        assertTrue(Character.IsJavaIdentifierPart(0x007F));
        //        assertTrue(Character.IsJavaIdentifierPart(0x009F));
        //        assertTrue(Character.IsJavaIdentifierPart(0x00AD));
        //        assertTrue(Character.IsJavaIdentifierPart(0xE007F));

        //        //RI fails because 0x200B changes category in Unicode 4.1
        //        assertTrue(Character.IsJavaIdentifierPart(0x200B));
        //    }

        //    /**
        //     * @tests java.lang.Character#isJavaIdentifierStart(char)
        //     */
        //[Test]
        //    public void Test_isJavaIdentifierStartC()
        //    {
        //        assertTrue("letter returned false", Character
        //                .isJavaIdentifierStart('l'));
        //        assertTrue("currency returned false", Character
        //                .isJavaIdentifierStart('$'));
        //        assertTrue("connecting char returned false", Character
        //                .isJavaIdentifierStart('_'));
        //        assertTrue("digit returned true", !Character.IsJavaIdentifierStart('9'));
        //        assertTrue("ignorable control returned true", !Character
        //                .isJavaIdentifierStart('\u200b'));
        //        assertTrue("semi returned true", !Character.IsJavaIdentifierStart(';'));
        //    }

        //    /**
        //     * @tests java.lang.Character#isJavaIdentifierStart(int)
        //     */
        //[Test]
        //    public void Test_isJavaIdentifierStart_I()
        //    {
        //        assertTrue(Character.IsJavaIdentifierStart((int)'l'));
        //        assertTrue(Character.IsJavaIdentifierStart((int)'$'));
        //        assertTrue(Character.IsJavaIdentifierStart((int)'_'));
        //        assertFalse(Character.IsJavaIdentifierStart((int)'9'));
        //        assertFalse(Character.IsJavaIdentifierStart((int)'\u200b'));
        //        assertFalse(Character.IsJavaIdentifierStart((int)';'));

        //        assertTrue(Character.IsJavaIdentifierStart(0x0041));
        //        assertTrue(Character.IsJavaIdentifierStart(0x10400));
        //        assertTrue(Character.IsJavaIdentifierStart(0x0061));
        //        assertTrue(Character.IsJavaIdentifierStart(0x10428));
        //        assertTrue(Character.IsJavaIdentifierStart(0x01C5));
        //        assertTrue(Character.IsJavaIdentifierStart(0x1FFC));
        //        assertTrue(Character.IsJavaIdentifierStart(0x02B0));
        //        assertTrue(Character.IsJavaIdentifierStart(0xFF9F));
        //        assertTrue(Character.IsJavaIdentifierStart(0x01BB));
        //        assertTrue(Character.IsJavaIdentifierStart(0x2F888));

        //        assertTrue(Character.IsJavaIdentifierPart(0x0024));
        //        assertTrue(Character.IsJavaIdentifierPart(0xFFE6));

        //        assertTrue(Character.IsJavaIdentifierPart(0x005F));
        //        assertTrue(Character.IsJavaIdentifierPart(0xFF3F));

        //        assertTrue(Character.IsJavaIdentifierPart(0x2160));
        //        assertTrue(Character.IsJavaIdentifierPart(0x1034A));

        //        assertFalse(Character.IsJavaIdentifierPart(0x110000));
        //    }

        //    /**
        //     * @tests java.lang.Character#isJavaLetter(char)
        //     */
        //[Test]
        //    @SuppressWarnings("deprecation")
        //public void Test_isJavaLetterC()
        //    {
        //        assertTrue("letter returned false", Character.IsJavaLetter('l'));
        //        assertTrue("currency returned false", Character.IsJavaLetter('$'));
        //        assertTrue("connecting char returned false", Character
        //                .isJavaLetter('_'));

        //        assertTrue("digit returned true", !Character.IsJavaLetter('9'));
        //        assertTrue("ignored control returned true", !Character
        //                .isJavaLetter('\u200b'));
        //        assertTrue("semi returned true", !Character.IsJavaLetter(';'));
        //    }

        //    /**
        //     * @tests java.lang.Character#isJavaLetterOrDigit(char)
        //     */
        //[Test]
        //    @SuppressWarnings("deprecation")
        //public void Test_isJavaLetterOrDigitC()
        //    {
        //        assertTrue("letter returned false", Character.IsJavaLetterOrDigit('l'));
        //        assertTrue("currency returned false", Character
        //                .isJavaLetterOrDigit('$'));
        //        assertTrue("digit returned false", Character.IsJavaLetterOrDigit('9'));
        //        assertTrue("connecting char returned false", Character
        //                .isJavaLetterOrDigit('_'));
        //        assertTrue("semi returned true", !Character.IsJavaLetterOrDigit(';'));
        //    }

        /**
         * @tests java.lang.Character#isLetter(char)
         */
        [Test]
        public void Test_isLetterC()
        {
            assertTrue("Letter returned false", Character.IsLetter('L'));
            assertTrue("Non-Letter returned true", !Character.IsLetter('9'));
        }

        /**
         * @tests java.lang.Character#isLetter(int)
         */
        [Test]
        public void Test_isLetter_I()
        {
            assertTrue(Character.IsLetter((int)'L'));
            assertFalse(Character.IsLetter((int)'9'));

            assertTrue(Character.IsLetter(0x1FA9));
            assertTrue(Character.IsLetter(0x1D400));
            assertTrue(Character.IsLetter(0x1D622));
            assertTrue(Character.IsLetter(0x10000));

            assertFalse(Character.IsLetter(0x1012C));
            assertFalse(Character.IsLetter(0x110000));
        }

        /**
         * @tests java.lang.Character#isLetterOrDigit(char)
         */
        [Test]
        public void Test_isLetterOrDigitC()
        {
            assertTrue("Digit returned false", Character.IsLetterOrDigit('9'));
            assertTrue("Letter returned false", Character.IsLetterOrDigit('K'));
            assertTrue("Control returned true", !Character.IsLetterOrDigit('\n'));
            assertTrue("Punctuation returned true", !Character.IsLetterOrDigit('?'));
        }

        /**
         * @tests java.lang.Character#isLetterOrDigit(int)
         */
        [Test]
        public void Test_isLetterOrDigit_I()
        {
            assertTrue(Character.IsLetterOrDigit((int)'9'));
            assertTrue(Character.IsLetterOrDigit((int)'K'));
            assertFalse(Character.IsLetterOrDigit((int)'\n'));
            assertFalse(Character.IsLetterOrDigit((int)'?'));

            assertTrue(Character.IsLetterOrDigit(0x1FA9));
            assertTrue(Character.IsLetterOrDigit(0x1D400));
            assertTrue(Character.IsLetterOrDigit(0x1D622));
            assertTrue(Character.IsLetterOrDigit(0x10000));

            assertTrue(Character.IsLetterOrDigit(0x1D7CE));
            assertTrue(Character.IsLetterOrDigit(0x1D7D8));

            assertFalse(Character.IsLetterOrDigit(0x10FFFD));
            assertFalse(Character.IsLetterOrDigit(0x1012C));
            assertFalse(Character.IsLetterOrDigit(0x110000));
        }

        /**
         * @tests java.lang.Character#isLowerCase(char)
         */
        [Test]
        public void Test_isLowerCaseC()
        {
            assertTrue("lower returned false", Character.IsLower('a'));
            assertTrue("upper returned true", !Character.IsLower('T'));
        }

        /**
         * @tests java.lang.Character#isLowerCase(int)
         */
        [Test]
        public void Test_isLowerCase_I()
        {
            assertTrue(Character.IsLower((int)'a'));
            assertFalse(Character.IsLower((int)'T'));

            assertTrue(Character.IsLower(0x10428));
            assertTrue(Character.IsLower(0x1D4EA));

            assertFalse(Character.IsLower(0x1D504));
            assertFalse(Character.IsLower(0x30000));
            assertFalse(Character.IsLower(0x110000));
        }

        /**
         * @tests java.lang.Character#isSpace(char)
         */
        [Test]
        public void Test_isSpaceC()
        {
            // Test for method boolean java.lang.Character.IsSpace(char)
            assertTrue("space returned false", Character.IsSpace('\n'));
            assertTrue("non-space returned true", !Character.IsSpace('T'));
        }

        /**
         * @tests java.lang.Character#isSpaceChar(char)
         */
        [Test]
        public void Test_isSpaceCharC()
        {
            assertTrue("space returned false", Character.IsSpaceChar('\u0020'));
            assertTrue("non-space returned true", !Character.IsSpaceChar('\n'));
        }

        /**
         * @tests java.lang.Character#isSpaceChar(int)
         */
        [Test]
        public void Test_isSpaceChar_I()
        {
            assertTrue(Character.IsSpaceChar((int)'\u0020'));
            assertFalse(Character.IsSpaceChar((int)'\n'));

            assertTrue(Character.IsSpaceChar(0x2000));
            assertTrue(Character.IsSpaceChar(0x200A));

            assertTrue(Character.IsSpaceChar(0x2028));
            assertTrue(Character.IsSpaceChar(0x2029));

            assertFalse(Character.IsSpaceChar(0x110000));
        }

        /**
         * @tests java.lang.Character#isTitleCase(char)
         */
        [Test]
        public void Test_isTitleCaseC()
        {
            char[] tChars = { (char) 0x01c5, (char) 0x01c8, (char) 0x01cb,
                (char) 0x01f2, (char) 0x1f88, (char) 0x1f89, (char) 0x1f8a,
                (char) 0x1f8b, (char) 0x1f8c, (char) 0x1f8d, (char) 0x1f8e,
                (char) 0x1f8f, (char) 0x1f98, (char) 0x1f99, (char) 0x1f9a,
                (char) 0x1f9b, (char) 0x1f9c, (char) 0x1f9d, (char) 0x1f9e,
                (char) 0x1f9f, (char) 0x1fa8, (char) 0x1fa9, (char) 0x1faa,
                (char) 0x1fab, (char) 0x1fac, (char) 0x1fad, (char) 0x1fae,
                (char) 0x1faf, (char) 0x1fbc, (char) 0x1fcc, (char) 0x1ffc };
            byte tnum = 0;
            for (char c = (char)0; c < 65535; c++)
            {
                if (Character.IsTitleCase(c))
                {
                    tnum++;
                    int i;
                    for (i = 0; i < tChars.Length; i++)
                        if (tChars[i] == c)
                            i = tChars.Length + 1;
                    if (i < tChars.Length)
                    {
                        fail("Non Title Case char returned true");
                    }
                }
            }
            assertTrue("Failed to find all Title Case chars", tnum == tChars.Length);
        }

        /**
         * @tests java.lang.Character#isTitleCase(int)
         */
        [Test]
        public void Test_isTitleCase_I()
        {
            //all the titlecase characters
            int[] titleCaseCharacters = { 0x01c5, 0x01c8, 0x01cb, 0x01f2, 0x1f88,
                0x1f89, 0x1f8a, 0x1f8b, 0x1f8c, 0x1f8d, 0x1f8e, 0x1f8f, 0x1f98,
                0x1f99, 0x1f9a, 0x1f9b, 0x1f9c, 0x1f9d, 0x1f9e, 0x1f9f, 0x1fa8,
                0x1fa9, 0x1faa, 0x1fab, 0x1fac, 0x1fad, 0x1fae, 0x1faf, 0x1fbc,
                0x1fcc, 0x1ffc };

            for (int i = 0; i < titleCaseCharacters.Length; i++)
            {
                assertTrue(Character.IsTitleCase(titleCaseCharacters[i]));
            }

            assertFalse(Character.IsTitleCase(0x110000));
        }

        /**
         * @tests java.lang.Character#isUnicodeIdentifierPart(char)
         */
        [Test]
        public void Test_isUnicodeIdentifierPartC()
        {
            assertTrue("'a' returned false", Character.IsUnicodeIdentifierPart('a'));
            assertTrue("'2' returned false", Character.IsUnicodeIdentifierPart('2'));
            assertTrue("'+' returned true", !Character.IsUnicodeIdentifierPart('+'));
        }

        /**
         * @tests java.lang.Character#isUnicodeIdentifierPart(int)
         */
        [Test]
        public void Test_isUnicodeIdentifierPart_I()
        {
            assertTrue(Character.IsUnicodeIdentifierPart((int)'a'));
            assertTrue(Character.IsUnicodeIdentifierPart((int)'2'));
            assertFalse(Character.IsUnicodeIdentifierPart((int)'+'));

            assertTrue(Character.IsUnicodeIdentifierPart(0x1FA9));
            assertTrue(Character.IsUnicodeIdentifierPart(0x1D400));
            assertTrue(Character.IsUnicodeIdentifierPart(0x1D622));
            assertTrue(Character.IsUnicodeIdentifierPart(0x10000));

            assertTrue(Character.IsUnicodeIdentifierPart(0x0030));
            assertTrue(Character.IsUnicodeIdentifierPart(0x0035));
            assertTrue(Character.IsUnicodeIdentifierPart(0x0039));

            assertTrue(Character.IsUnicodeIdentifierPart(0x0660));
            assertTrue(Character.IsUnicodeIdentifierPart(0x0665));
            assertTrue(Character.IsUnicodeIdentifierPart(0x0669));

            assertTrue(Character.IsUnicodeIdentifierPart(0x06F0));
            assertTrue(Character.IsUnicodeIdentifierPart(0x06F5));
            assertTrue(Character.IsUnicodeIdentifierPart(0x06F9));

            assertTrue(Character.IsUnicodeIdentifierPart(0x0966));
            assertTrue(Character.IsUnicodeIdentifierPart(0x096A));
            assertTrue(Character.IsUnicodeIdentifierPart(0x096F));

            assertTrue(Character.IsUnicodeIdentifierPart(0xFF10));
            assertTrue(Character.IsUnicodeIdentifierPart(0xFF15));
            assertTrue(Character.IsUnicodeIdentifierPart(0xFF19));

            assertTrue(Character.IsUnicodeIdentifierPart(0x1D7CE));
            assertTrue(Character.IsUnicodeIdentifierPart(0x1D7D8));

            assertTrue(Character.IsUnicodeIdentifierPart(0x16EE));
            assertTrue(Character.IsUnicodeIdentifierPart(0xFE33));
            assertTrue(Character.IsUnicodeIdentifierPart(0xFF10));
            assertTrue(Character.IsUnicodeIdentifierPart(0x1D165));
            assertTrue(Character.IsUnicodeIdentifierPart(0x1D167));
            assertTrue(Character.IsUnicodeIdentifierPart(0x1D173));

            assertFalse(Character.IsUnicodeIdentifierPart(0x10FFFF));
            assertFalse(Character.IsUnicodeIdentifierPart(0x110000));
        }

        /**
         * @tests java.lang.Character#isUnicodeIdentifierStart(char)
         */
        [Test]
        public void Test_isUnicodeIdentifierStartC()
        {
            assertTrue("'a' returned false", Character
                    .IsUnicodeIdentifierStart('a'));
            assertTrue("'2' returned true", !Character
                    .IsUnicodeIdentifierStart('2'));
            assertTrue("'+' returned true", !Character
                    .IsUnicodeIdentifierStart('+'));
        }

        /**
         * @tests java.lang.Character#isUnicodeIdentifierStart(int)
         */
        [Test]
        public void Test_isUnicodeIdentifierStart_I()
        {

            assertTrue(Character.IsUnicodeIdentifierStart((int)'a'));
            assertFalse(Character.IsUnicodeIdentifierStart((int)'2'));
            assertFalse(Character.IsUnicodeIdentifierStart((int)'+'));

            assertTrue(Character.IsUnicodeIdentifierStart(0x1FA9));
            assertTrue(Character.IsUnicodeIdentifierStart(0x1D400));
            assertTrue(Character.IsUnicodeIdentifierStart(0x1D622));
            assertTrue(Character.IsUnicodeIdentifierStart(0x10000));

            assertTrue(Character.IsUnicodeIdentifierStart(0x16EE));

            // number is not a valid start of a Unicode identifier
            assertFalse(Character.IsUnicodeIdentifierStart(0x0030));
            assertFalse(Character.IsUnicodeIdentifierStart(0x0039));
            assertFalse(Character.IsUnicodeIdentifierStart(0x0660));
            assertFalse(Character.IsUnicodeIdentifierStart(0x0669));
            assertFalse(Character.IsUnicodeIdentifierStart(0x06F0));
            assertFalse(Character.IsUnicodeIdentifierStart(0x06F9));

            assertFalse(Character.IsUnicodeIdentifierPart(0x10FFFF));
            assertFalse(Character.IsUnicodeIdentifierPart(0x110000));
        }

        /**
         * @tests java.lang.Character#isUpperCase(char)
         */
        [Test]
        public void Test_isUpperCaseC()
        {
            assertTrue("Incorrect case value", !Character.IsUpper('t'));
            assertTrue("Incorrect case value", Character.IsUpper('T'));
        }

        /**
         * @tests java.lang.Character#isUpperCase(int)
         */
        [Test]
        public void Test_isUpperCase_I()
        {
            assertFalse(Character.IsUpper((int)'t'));
            assertTrue(Character.IsUpper((int)'T'));

            assertTrue(Character.IsUpper(0x1D504));
            assertTrue(Character.IsUpper(0x1D608));

            assertFalse(Character.IsUpper(0x1D656));
            assertFalse(Character.IsUpper(0x10FFFD));
            assertFalse(Character.IsUpper(0x110000));
        }

        /**
         * @tests java.lang.Character#isWhitespace(char)
         */
        [Test]
        public void Test_isWhitespaceC()
        {
            assertTrue("space returned false", Character.IsWhiteSpace('\n'));
            assertTrue("non-space returned true", !Character.IsWhiteSpace('T'));
        }

        /**
         * @tests java.lang.Character#isWhitespace(int)
         */
        [Test]
        public void Test_isWhitespace_I()
        {
            assertTrue(Character.IsWhiteSpace((int)'\n'));
            assertFalse(Character.IsWhiteSpace((int)'T'));

            assertTrue(Character.IsWhiteSpace(0x0009));
            assertTrue(Character.IsWhiteSpace(0x000A));
            assertTrue(Character.IsWhiteSpace(0x000B));
            assertTrue(Character.IsWhiteSpace(0x000C));
            assertTrue(Character.IsWhiteSpace(0x000D));
            assertTrue(Character.IsWhiteSpace(0x001C));
            assertTrue(Character.IsWhiteSpace(0x001D));
            assertTrue(Character.IsWhiteSpace(0x001F));
            assertTrue(Character.IsWhiteSpace(0x001E));

            assertTrue(Character.IsWhiteSpace(0x2000));
            assertTrue(Character.IsWhiteSpace(0x200A));

            assertTrue(Character.IsWhiteSpace(0x2028));
            assertTrue(Character.IsWhiteSpace(0x2029));

            assertFalse(Character.IsWhiteSpace(0x00A0));
            assertFalse(Character.IsWhiteSpace(0x202F));
            assertFalse(Character.IsWhiteSpace(0x110000));

            assertFalse(Character.IsWhiteSpace(0xFEFF));

            //FIXME depend on ICU4J
            //assertFalse(Character.IsWhiteSpace(0x2007));

        }

        /**
         * @tests java.lang.Character#reverseBytes(char)
         */
        [Test]
        public void Test_reverseBytesC()
        {
            char[] original = new char[] { (char)0x0000, (char)0x0010, (char)0x00AA, (char)0xB000, (char)0xCC00, (char)0xABCD, (char)0xFFAA };
            char[] reversed = new char[] { (char)0x0000, (char)0x1000, (char)0xAA00, (char)0x00B0, (char)0x00CC, (char)0xCDAB, (char)0xAAFF };
            assertTrue("Test self check", original.Length == reversed.Length);

            for (int i = 0; i < original.Length; i++)
            {
                char origChar = original[i];
                char reversedChar = reversed[i];
                char origReversed = Character.ReverseBytes(origChar);

                assertTrue("java.lang.Character.reverseBytes failed: orig char="
                    + origChar.ToHexString() + ", reversed char="
                    + origReversed.ToHexString(), reversedChar == origReversed);
            }
        }

        /////**
        //// * @tests java.lang.Character#toLowerCase(char)
        //// */
        ////[Test]
        ////public void Test_toLowerCaseC()
        ////{
        ////    assertEquals("Failed to change case", 't', Character.toLowerCase('T'));
        ////}

        /////**
        //// * @tests java.lang.Character#toLowerCase(int)
        //// */
        ////[Test]
        ////public void Test_toLowerCase_I()
        ////{
        ////    assertEquals('t', Character.toLowerCase((int)'T'));

        ////    assertEquals(0x10428, Character.toLowerCase(0x10400));
        ////    assertEquals(0x10428, Character.toLowerCase(0x10428));

        ////    assertEquals(0x1D504, Character.toLowerCase(0x1D504));
        ////    assertEquals(0x10FFFD, Character.toLowerCase(0x10FFFD));
        ////    assertEquals(0x110000, Character.toLowerCase(0x110000));
        ////}

        /////**
        //// * @tests java.lang.Character#toString()
        //// */
        ////[Test]
        ////public void Test_toString()
        ////{
        ////    assertEquals("Incorrect String returned", "T", new Character('T').toString());
        ////}

        /////**
        //// * @tests java.lang.Character#toTitleCase(char)
        //// */
        ////[Test]
        ////public void Test_toTitleCaseC()
        ////{
        ////    assertEquals("Incorrect title case for a",
        ////            'A', Character.toTitleCase('a'));
        ////    assertEquals("Incorrect title case for A",
        ////            'A', Character.toTitleCase('A'));
        ////    assertEquals("Incorrect title case for 1",
        ////            '1', Character.toTitleCase('1'));
        ////}

        /////**
        //// * @tests java.lang.Character#toTitleCase(int)
        //// */
        ////[Test]
        ////public void Test_toTitleCase_I()
        ////{
        ////    assertEquals('A', Character.toTitleCase((int)'a'));
        ////    assertEquals('A', Character.toTitleCase((int)'A'));
        ////    assertEquals('1', Character.toTitleCase((int)'1'));

        ////    assertEquals(0x10400, Character.toTitleCase(0x10428));
        ////    assertEquals(0x10400, Character.toTitleCase(0x10400));

        ////    assertEquals(0x10FFFF, Character.toTitleCase(0x10FFFF));
        ////    assertEquals(0x110000, Character.toTitleCase(0x110000));
        ////}

        /////**
        //// * @tests java.lang.Character#toUpperCase(char)
        //// */
        ////[Test]
        ////public void Test_toUpperCaseC()
        ////{
        ////    // Test for method char java.lang.Character.toUpperCase(char)
        ////    assertEquals("Incorrect upper case for a",
        ////            'A', Character.toUpperCase('a'));
        ////    assertEquals("Incorrect upper case for A",
        ////            'A', Character.toUpperCase('A'));
        ////    assertEquals("Incorrect upper case for 1",
        ////            '1', Character.toUpperCase('1'));
        ////}

        /////**
        //// * @tests java.lang.Character#toUpperCase(int)
        //// */
        ////[Test]
        ////public void Test_toUpperCase_I()
        ////{
        ////    assertEquals('A', Character.toUpperCase((int)'a'));
        ////    assertEquals('A', Character.toUpperCase((int)'A'));
        ////    assertEquals('1', Character.toUpperCase((int)'1'));

        ////    assertEquals(0x10400, Character.toUpperCase(0x10428));
        ////    assertEquals(0x10400, Character.toUpperCase(0x10400));

        ////    assertEquals(0x10FFFF, Character.toUpperCase(0x10FFFF));
        ////    assertEquals(0x110000, Character.toUpperCase(0x110000));
        ////}

        /////**
        //// * @tests java.lang.Character#getDirectionality(int)
        //// */
        ////[Test]
        ////public void Test_isDirectionaliy_I()
        ////{
        ////    assertEquals(Character.DIRECTIONALITY_UNDEFINED, Character
        ////            .getDirectionality(0xFFFE));
        ////    assertEquals(Character.DIRECTIONALITY_UNDEFINED, Character
        ////            .getDirectionality(0x30000));
        ////    assertEquals(Character.DIRECTIONALITY_UNDEFINED, Character
        ////            .getDirectionality(0x110000));
        ////    assertEquals(Character.DIRECTIONALITY_UNDEFINED, Character
        ////            .getDirectionality(-1));

        ////    assertEquals(Character.DIRECTIONALITY_LEFT_TO_RIGHT, Character
        ////            .getDirectionality(0x0041));
        ////    assertEquals(Character.DIRECTIONALITY_LEFT_TO_RIGHT, Character
        ////            .getDirectionality(0x10000));
        ////    assertEquals(Character.DIRECTIONALITY_LEFT_TO_RIGHT, Character
        ////            .getDirectionality(0x104A9));

        ////    assertEquals(Character.DIRECTIONALITY_RIGHT_TO_LEFT, Character
        ////            .getDirectionality(0xFB4F));
        ////    assertEquals(Character.DIRECTIONALITY_RIGHT_TO_LEFT, Character
        ////            .getDirectionality(0x10838));
        ////    // Unicode standard 5.1 changed category of unicode point 0x0600 from AL to AN
        ////    assertEquals(Character.DIRECTIONALITY_ARABIC_NUMBER, Character
        ////            .getDirectionality(0x0600));
        ////    assertEquals(Character.DIRECTIONALITY_RIGHT_TO_LEFT_ARABIC, Character
        ////            .getDirectionality(0xFEFC));

        ////    assertEquals(Character.DIRECTIONALITY_EUROPEAN_NUMBER, Character
        ////            .getDirectionality(0x2070));
        ////    assertEquals(Character.DIRECTIONALITY_EUROPEAN_NUMBER, Character
        ////            .getDirectionality(0x1D7FF));

        ////    //RI fails ,this is non-bug difference between Unicode 4.0 and 4.1
        ////    assertEquals(Character.DIRECTIONALITY_EUROPEAN_NUMBER_SEPARATOR, Character
        ////            .getDirectionality(0x002B));
        ////    assertEquals(Character.DIRECTIONALITY_EUROPEAN_NUMBER_SEPARATOR, Character
        ////            .getDirectionality(0xFF0B));

        ////    assertEquals(Character.DIRECTIONALITY_EUROPEAN_NUMBER_TERMINATOR, Character
        ////            .getDirectionality(0x0023));
        ////    assertEquals(Character.DIRECTIONALITY_EUROPEAN_NUMBER_TERMINATOR, Character
        ////            .getDirectionality(0x17DB));

        ////    assertEquals(Character.DIRECTIONALITY_ARABIC_NUMBER, Character
        ////            .getDirectionality(0x0660));
        ////    assertEquals(Character.DIRECTIONALITY_ARABIC_NUMBER, Character
        ////            .getDirectionality(0x066C));

        ////    assertEquals(Character.DIRECTIONALITY_COMMON_NUMBER_SEPARATOR, Character
        ////            .getDirectionality(0x002C));
        ////    assertEquals(Character.DIRECTIONALITY_COMMON_NUMBER_SEPARATOR, Character
        ////            .getDirectionality(0xFF1A));

        ////    assertEquals(Character.DIRECTIONALITY_NONSPACING_MARK, Character
        ////            .getDirectionality(0x17CE));
        ////    assertEquals(Character.DIRECTIONALITY_NONSPACING_MARK, Character
        ////            .getDirectionality(0xE01DB));

        ////    assertEquals(Character.DIRECTIONALITY_BOUNDARY_NEUTRAL, Character
        ////            .getDirectionality(0x0000));
        ////    assertEquals(Character.DIRECTIONALITY_BOUNDARY_NEUTRAL, Character
        ////            .getDirectionality(0xE007F));

        ////    assertEquals(Character.DIRECTIONALITY_PARAGRAPH_SEPARATOR, Character
        ////            .getDirectionality(0x000A));
        ////    assertEquals(Character.DIRECTIONALITY_PARAGRAPH_SEPARATOR, Character
        ////            .getDirectionality(0x2029));

        ////    assertEquals(Character.DIRECTIONALITY_SEGMENT_SEPARATOR, Character
        ////            .getDirectionality(0x0009));
        ////    assertEquals(Character.DIRECTIONALITY_SEGMENT_SEPARATOR, Character
        ////            .getDirectionality(0x001F));

        ////    assertEquals(Character.DIRECTIONALITY_WHITESPACE, Character
        ////            .getDirectionality(0x0020));
        ////    assertEquals(Character.DIRECTIONALITY_WHITESPACE, Character
        ////            .getDirectionality(0x3000));

        ////    assertEquals(Character.DIRECTIONALITY_OTHER_NEUTRALS, Character
        ////            .getDirectionality(0x2FF0));
        ////    assertEquals(Character.DIRECTIONALITY_OTHER_NEUTRALS, Character
        ////            .getDirectionality(0x1D356));

        ////    assertEquals(Character.DIRECTIONALITY_LEFT_TO_RIGHT_EMBEDDING, Character
        ////            .getDirectionality(0x202A));

        ////    assertEquals(Character.DIRECTIONALITY_LEFT_TO_RIGHT_OVERRIDE, Character
        ////            .getDirectionality(0x202D));

        ////    assertEquals(Character.DIRECTIONALITY_RIGHT_TO_LEFT_EMBEDDING, Character
        ////            .getDirectionality(0x202B));

        ////    assertEquals(Character.DIRECTIONALITY_RIGHT_TO_LEFT_OVERRIDE, Character
        ////            .getDirectionality(0x202E));

        ////    assertEquals(Character.DIRECTIONALITY_POP_DIRECTIONAL_FORMAT, Character
        ////            .getDirectionality(0x202C));
        ////}

    }
}
