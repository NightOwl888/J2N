using ICU4N;
using ICU4N.Globalization;
using ICU4N.Text;
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
        public void Test_codePointAt_ReadOnlySpanI()
        {

            assertEquals('a', Character.CodePointAt("abc".AsSpan(), 0));
            assertEquals('b', Character.CodePointAt("abc".AsSpan(), 1));
            assertEquals('c', Character.CodePointAt("abc".AsSpan(), 2));
            assertEquals(0x10000, Character.CodePointAt(
                    "\uD800\uDC00".AsSpan(), 0));
            assertEquals('\uDC00', Character.CodePointAt(
                    "\uD800\uDC00".AsSpan(), 1));

            // J2N: ReadOnlySpan<char> is a value type - null not allowed

            try
            {
                Character.CodePointAt("abc".AsSpan(), -1);
                fail("No IOOBE, negative index.");
            }
            catch (ArgumentOutOfRangeException e)
            {
            }

            try
            {
                Character.CodePointAt("abc".AsSpan(), 4);
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
        public void Test_codePointBefore_ReadOnlySpanI()
        {

            assertEquals('a', Character.CodePointBefore("abc".AsSpan(), 1));
            assertEquals('b', Character.CodePointBefore("abc".AsSpan(), 2));
            assertEquals('c', Character.CodePointBefore("abc".AsSpan(), 3));
            assertEquals(0x10000, Character.CodePointBefore(
                    "\uD800\uDC00".AsSpan(), 2));
            assertEquals('\uD800', Character.CodePointBefore(
                    "\uD800\uDC00".AsSpan(), 1));

            // J2N: ReadOnlySpan<char> is a value type - null not allowed

            try
            {
                Character.CodePointBefore("abc".AsSpan(), 0);
                fail("No IOOBE, index below one.");
            }
            catch (ArgumentOutOfRangeException e)
            {
            }

            try
            {
                Character.CodePointBefore("abc".AsSpan(), 4);
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
        public void Test_toCharsI_Span_I() // J2N: Added overload to cover Span<char>
        {
            char[] dest = new char[2];
            Span<char> dst = dest;
            int result = Character.ToChars(0x10000, dst, 0);
            assertEquals(2, result);
            assertTrue(dst.SequenceEqual(new char[] { '\uD800', '\uDC00' }));

            result = Character.ToChars(0x10001, dst, 0);
            assertEquals(2, result);
            assertTrue(dst.SequenceEqual(new char[] { '\uD800', '\uDC01' }));

            result = Character.ToChars(0x10401, dst, 0);
            assertEquals(2, result);
            assertTrue(dst.SequenceEqual(new char[] { '\uD801', '\uDC01' }));

            result = Character.ToChars(0x10FFFF, dst, 0);
            assertEquals(2, result);
            assertTrue(dst.SequenceEqual(new char[] { '\uDBFF', '\uDFFF' }));

            try
            {
                Character.ToChars(int.MaxValue, new char[2].AsSpan(), 0);
                fail("No IAE, invalid code point.");
            }
            catch (ArgumentException e)
            {
            }

            //try
            //{
            //    Character.ToChars('a', null, 0);
            //    fail("No NPE, null char[].");
            //}
            //catch (ArgumentNullException e)
            //{
            //}

            try
            {
                Character.ToChars('a', new char[1].AsSpan(), -1);
                fail("No IOOBE, negative index.");
            }
            catch (ArgumentOutOfRangeException e)
            {
            }

            try
            {
                Character.ToChars('a', new char[1].AsSpan(), 1);
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
        public void Test_toCharsI_Span() // J2N: Added overload to return ReadOnlySpan<char> while passing in a buffer on the stack.
        {
            Span<char> buffer = stackalloc char[2];
            assertTrue(System.MemoryExtensions.SequenceEqual(new char[] { '\uD800', '\uDC00' }, Character
                    .ToChars(0x10000, buffer)));
            assertTrue(System.MemoryExtensions.SequenceEqual(new char[] { '\uD800', '\uDC01' }, Character
                    .ToChars(0x10001, buffer)));
            assertTrue(System.MemoryExtensions.SequenceEqual(new char[] { '\uD801', '\uDC01' }, Character
                    .ToChars(0x10401, buffer)));
            assertTrue(System.MemoryExtensions.SequenceEqual(new char[] { '\uDBFF', '\uDFFF' }, Character
                    .ToChars(0x10FFFF, buffer)));

            try
            {
                Character.ToChars(int.MaxValue, buffer);
                fail("No IAE, invalid code point.");
            }
            catch (ArgumentException e)
            {
            }
        }

        [Test]
        public void Test_toCharsI_Out() // J2N: Added overload to avoid heap allocation
        {
            char high, low;
            int length;

            length = Character.ToChars(0x10000, out high, out low);
            assertEquals(2, length);
            assertEquals('\uD800', high);
            assertEquals('\uDC00', low);

            length = Character.ToChars(0x10001, out high, out low);
            assertEquals(2, length);
            assertEquals('\uD800', high);
            assertEquals('\uDC01', low);

            length = Character.ToChars(0x10401, out high, out low);
            assertEquals(2, length);
            assertEquals('\uD801', high);
            assertEquals('\uDC01', low);

            length = Character.ToChars(0x10FFFF, out high, out low);
            assertEquals(2, length);
            assertEquals('\uDBFF', high);
            assertEquals('\uDFFF', low);


            try
            {
                Character.ToChars(int.MaxValue, out high, out low);
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
        public void Test_codePointCount_ReadOnlySpanII()
        {
            assertEquals(1, Character.CodePointCount("\uD800\uDC00".AsSpan(0, 2 - 0))); // end - start
            assertEquals(1, Character.CodePointCount("\uD800\uDC01".AsSpan(0, 2 - 0))); // end - start
            assertEquals(1, Character.CodePointCount("\uD801\uDC01".AsSpan(0, 2 - 0))); // end - start
            assertEquals(1, Character.CodePointCount("\uDBFF\uDFFF".AsSpan(0, 2 - 0))); // end - start

            assertEquals(3, Character.CodePointCount("a\uD800\uDC00b".AsSpan(0, 4 - 0))); // end - start
            assertEquals(4, Character.CodePointCount("a\uD800\uDC00b\uD800".AsSpan(0, 5 - 0))); // end - start
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
        public void Test_offsetByCodePoints_ReadOnlySpanII()
        {
            int result = Character.OffsetByCodePoints("a\uD800\uDC00b".AsSpan(), 0, 2);
            assertEquals(3, result);

            result = Character.OffsetByCodePoints("abcd".AsSpan(), 3, -1);
            assertEquals(2, result);

            result = Character.OffsetByCodePoints("a\uD800\uDC00b".AsSpan(), 0, 3);
            assertEquals(4, result);

            result = Character.OffsetByCodePoints("a\uD800\uDC00b".AsSpan(), 3, -1);
            assertEquals(1, result);

            result = Character.OffsetByCodePoints("a\uD800\uDC00b".AsSpan(), 3, 0);
            assertEquals(3, result);

            result = Character.OffsetByCodePoints("\uD800\uDC00bc".AsSpan(), 3, 0);
            assertEquals(3, result);

            result = Character.OffsetByCodePoints("a\uDC00bc".AsSpan(), 3, -1);
            assertEquals(2, result);

            result = Character.OffsetByCodePoints("a\uD800bc".AsSpan(), 3, -1);
            assertEquals(2, result);

            // J2N: ReadOnlySpan<char> is a value type - null not allowed

            try
            {
                Character.OffsetByCodePoints("abc".AsSpan(), -1, 1);
                fail();
            }
            catch (ArgumentOutOfRangeException e)
            {
            }

            try
            {
                Character.OffsetByCodePoints("abc".AsSpan(), 4, 1);
                fail();
            }
            catch (ArgumentOutOfRangeException e)
            {
            }

            try
            {
                Character.OffsetByCodePoints("abc".AsSpan(), 1, 3);
                fail();
            }
            catch (ArgumentOutOfRangeException e)
            {
            }

            try
            {
                Character.OffsetByCodePoints("abc".AsSpan(), 1, -2);
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

            assertEquals("Returned incorrect digit", 1, Character.Digit('๑', 10));
        }

        [Test]
        public void Test_DigitCI_Against_ICU4N()
        {
            for (int c = Character.MinCodePoint; c <= Character.MaxCodePoint; c++)
            {
                if (c >= Character.MinSupplementaryCodePoint)
                    continue;

                for (int radix = Character.MinRadix; radix <= Character.MaxRadix; radix++)
                {
                    int expected = UChar.Digit(c, radix);
                    int actual = Character.Digit((char)c, radix);

                    assertEquals($"{c} (Hex 0x{c.ToHexString()}) failed to match for radix {radix}.", expected, actual);
                }
            }
        }

        /**
         * @tests java.lang.Character#digit(int, int)
         */
        [Test]
        public void Test_digit_II()
        {
            assertEquals(1, Character.Digit((int)'1', 10));
            assertEquals(15, Character.Digit((int)'F', 16));

            assertEquals(-1, Character.Digit(0x0000, 37));
            assertEquals(-1, Character.Digit(0x0045, 10));

            assertEquals(10, Character.Digit(0x0041, 20));
            assertEquals(10, Character.Digit(0x0061, 20));

            assertEquals(-1, Character.Digit(0x110000, 20));
        }

        [Test]
        //[Ignore("Run Manually - ICU4N's Digit method is slow with surrogates")]
        public void Test_Digit_II_Against_ICU4N()
        {
            for (int c = Character.MinCodePoint; c <= Character.MaxCodePoint; c++)
            {
                for (int radix = Character.MinRadix; radix <= Character.MaxRadix; radix++)
                {
                    int expected = UChar.Digit(c, radix);
                    int actual = Character.Digit(c, radix);

                    assertEquals($"{c} (Hex 0x{c.ToHexString()}) failed to match for radix {radix}.", expected, actual);
                }
            }
        }

        [Test]
        [Ignore("For debugging")]
        public void Test_Digit_II_Against_ICU4N_Debug()
        {
            int c = 0x1d7e2; // 0x1d7d8; // 0x1d7ce; //0x1d7f6; // 0x1d7ec; // 0x1d7e2; // 0x1d7d8;
            int radix = 2;

            int expected = UChar.Digit(c, radix);
            int actual = Character.Digit(c, radix);

            assertEquals($"{c} (Hex 0x{c.ToHexString()}) failed to match for radix {radix}.", expected, actual);
        }

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

        [Test]
        public void Test_GetNumericValueC_Against_ICU4N()
        {
            for (int c = Character.MinCodePoint; c <= Character.MaxCodePoint; c++)
            {
                if (c >= Character.MinSupplementaryCodePoint)
                    continue;

                int expected = UChar.GetNumericValue(c);
                int actual = Character.GetNumericValue((char)c);

                assertEquals($"{c} (Hex 0x{c.ToHexString()}) failed to match.", expected, actual);
            }
        }

        [Test]
        [Ignore("For debugging")]
        public void Test_GetNumericValueC_Against_ICU4N_Debug()
        {
            int c = 0x2187; // 0xd58; //0xc7c; //0x9f9; //0x9f4; //0xb2;

            int expected = UChar.GetNumericValue(c);
            int actual = Character.GetNumericValue((char)c);

            assertEquals($"{c} (Hex 0x{c.ToHexString()}) failed to match.", expected, actual);
        }

        /**
         * @tests java.lang.Character#getNumericValue(int)
         */
        [Test]
        public void Test_getNumericValue_I()
        {
            assertEquals(1, Character.GetNumericValue((int)'1'));
            assertEquals(15, Character.GetNumericValue((int)'F'));
            assertEquals(-1, Character.GetNumericValue((int)'\u221e'));
            assertEquals(-2, Character.GetNumericValue((int)'\u00be'));
            assertEquals(10000, Character.GetNumericValue((int)'\u2182'));
            assertEquals(2, Character.GetNumericValue((int)'\uff12'));
            assertEquals(-1, Character.GetNumericValue(0xFFFF));

            assertEquals(-1, Character.GetNumericValue(0xFFFF));
            assertEquals(0, Character.GetNumericValue(0x1D7CE));
            assertEquals(0, Character.GetNumericValue(0x1D7D8));
            assertEquals(-1, Character.GetNumericValue(0x2F800));
            assertEquals(-1, Character.GetNumericValue(0x10FFFD));
            assertEquals(-1, Character.GetNumericValue(0x110000));

            assertEquals(50, Character.GetNumericValue(0x216C));

            assertEquals(10, Character.GetNumericValue(0x0041));
            assertEquals(35, Character.GetNumericValue(0x005A));
            assertEquals(10, Character.GetNumericValue(0x0061));
            assertEquals(35, Character.GetNumericValue(0x007A));
            assertEquals(10, Character.GetNumericValue(0xFF21));

            //FIXME depends on ICU4J
            //assertEquals(35, Character.GetNumericValue(0xFF3A));

            assertEquals(10, Character.GetNumericValue(0xFF41));
            assertEquals(35, Character.GetNumericValue(0xFF5A));
        }

        [Test]
        public void Test_GetNumericValue_I_Against_ICU4N()
        {
            for (int c = Character.MinCodePoint; c <= Character.MaxCodePoint; c++)
            {
                int expected = UChar.GetNumericValue(c);
                int actual = Character.GetNumericValue(c);

                assertEquals($"{c} (Hex 0x{c.ToHexString()}) failed to match.", expected, actual);
            }
        }

        [Test]
        [Ignore("For debugging")]
        public void Test_GetNumericValue_I_Against_ICU4N_Debug()
        {
            int c = 0x0; //0x10131; // 0xd58; //0xc7c; //0x9f9; //0x9f4; //0xb2;

            int expected = UChar.GetNumericValue(c);
            int actual = Character.GetNumericValue(c);

            assertEquals($"{c} (Hex 0x{c.ToHexString()}) failed to match.", expected, actual);
        }

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

#if FEATURE_UNICODE_DEFINED_0x9FFF
            assertTrue(Character.GetType(0x9FFF) == UnicodeCategory.OtherLetter);
#else
            assertTrue(Character.GetType(0x9FFF) == UnicodeCategory.OtherNotAssigned);
#endif
#if FEATURE_UNICODE_DEFINED_0x30000
            assertTrue(Character.GetType(0x30000) == UnicodeCategory.OtherLetter); // This character is now defined in .NET 5
#else
            assertTrue(Character.GetType(0x30000) == UnicodeCategory.OtherNotAssigned);
#endif
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

#if FEATURE_UNICODE_DEFINED_0x30000
            assertTrue(Character.IsDefined(0x30000)); // This character is now defined in .NET 5
#else
            assertFalse(Character.IsDefined(0x30000));
#endif
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
            int[] javaGoodWhiteSpaceChars = new int[] {
                0x0009,
                0x000a, // '\n' 'T'
                0x000b,
                0x000c,
                0x000d,
                0x001c,
                0x001d,
                0x001e,
                0x001f,
                0x0020,
                0x1680,
                0x2000,
                0x2001,
                0x2002,
                0x2003,
                0x2004,
                0x2005,
                0x2006,
                0x2008,
                0x2009,
                0x200a,
                0x2028,
                0x2029,
                0x205f,
                0x3000,
            };

            int[] javaBadWhiteSpaceChars = new int[] {
                // These are official Unicode whitespace, but Java doesn't recognize them
                0x0085,
                0x00A0,
                0x2007,
                0x202F,

                // From Harmony
                'T',
                0x110000, // Invalid Unicode code point
                0xFEFF,

                // Harmony bug
                0x200b,
            };

            foreach (char c in javaGoodWhiteSpaceChars)
            {
                assertTrue($"0x{c:X4}", Character.IsWhiteSpace(c));
            }

            foreach (char c in javaBadWhiteSpaceChars)
            {
                assertFalse($"0x{c:X4}", Character.IsWhiteSpace(c));
            }

            for (int c = Character.MinCodePoint; c <= Character.MaxCodePoint; c++)
            {
                assertEquals($"0x{c:X4}", UChar.IsWhiteSpace((char)c), Character.IsWhiteSpace((char)c));
            }

            //assertTrue("space returned false", Character.IsWhiteSpace('\n'));
            //assertTrue("non-space returned true", !Character.IsWhiteSpace('T'));
        }

        /**
         * @tests java.lang.Character#isWhitespace(int)
         */
        [Test]
        public void Test_isWhitespace_I()
        {
            // J2N: Added more thorough tests because Harmony had differences from ICU4N

            int[] javaGoodWhiteSpaceChars = new int[] {
                0x0009,
                0x000a, // '\n' 'T'
                0x000b,
                0x000c,
                0x000d,
                0x001c,
                0x001d,
                0x001e,
                0x001f,
                0x0020,
                0x1680,
                0x2000,
                0x2001,
                0x2002,
                0x2003,
                0x2004,
                0x2005,
                0x2006,
                0x2008,
                0x2009,
                0x200a,
                0x2028,
                0x2029,
                0x205f,
                0x3000,
            };

            int[] javaBadWhiteSpaceChars = new int[] {
                // These are official Unicode whitespace, but Java doesn't recognize them
                0x0085,
                0x00A0,
                0x2007,
                0x202F,

                // From Harmony
                'T',
                0x110000, // Invalid Unicode code point
                0xFEFF,

                // Harmony bug
                0x200b,
            };

            foreach (int c in javaGoodWhiteSpaceChars)
            {
                assertTrue($"0x{c:X4}", Character.IsWhiteSpace(c));
            }

            foreach (int c in javaBadWhiteSpaceChars)
            {
                assertFalse($"0x{c:X4}", Character.IsWhiteSpace(c));
            }

            for (int c = Character.MinCodePoint; c <= Character.MaxCodePoint; c++)
            {
                assertEquals($"0x{c:X4}", UChar.IsWhiteSpace(c), Character.IsWhiteSpace(c));
            }

            //assertTrue(Character.IsWhiteSpace((int)'\n'));
            //assertFalse(Character.IsWhiteSpace((int)'T'));

            //assertTrue(Character.IsWhiteSpace(0x0009));
            //assertTrue(Character.IsWhiteSpace(0x000A));
            //assertTrue(Character.IsWhiteSpace(0x000B));
            //assertTrue(Character.IsWhiteSpace(0x000C));
            //assertTrue(Character.IsWhiteSpace(0x000D));
            //assertTrue(Character.IsWhiteSpace(0x001C));
            //assertTrue(Character.IsWhiteSpace(0x001D));
            //assertTrue(Character.IsWhiteSpace(0x001F));
            //assertTrue(Character.IsWhiteSpace(0x001E));

            //assertTrue(Character.IsWhiteSpace(0x2000));
            //assertTrue(Character.IsWhiteSpace(0x200A));

            //assertTrue(Character.IsWhiteSpace(0x2028));
            //assertTrue(Character.IsWhiteSpace(0x2029));

            //assertFalse(Character.IsWhiteSpace(0x00A0));
            //assertFalse(Character.IsWhiteSpace(0x202F));
            //assertFalse(Character.IsWhiteSpace(0x110000));

            //assertFalse(Character.IsWhiteSpace(0xFEFF));

            ////FIXME depend on ICU4J
            ////assertFalse(Character.IsWhiteSpace(0x2007));

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

        /**
         * @tests java.lang.Character#toLowerCase(char)
         */
        [Test]
        public void Test_ToLower_C()
        {
            assertEquals("Failed to change case", 't', Character.ToLower('T', CultureInfo.InvariantCulture));
        }

        /**
         * @tests java.lang.Character#toLowerCase(int)
         */
        [Test]
        public void Test_ToLower_I()
        {
            assertEquals('t', Character.ToLower((int)'T', CultureInfo.InvariantCulture));

            assertEquals(0x10428, Character.ToLower(0x10400, CultureInfo.InvariantCulture));
            assertEquals(0x10428, Character.ToLower(0x10428, CultureInfo.InvariantCulture));

            assertEquals(0x1D504, Character.ToLower(0x1D504, CultureInfo.InvariantCulture));
            assertEquals(0x10FFFD, Character.ToLower(0x10FFFD, CultureInfo.InvariantCulture));
            //assertEquals(0x110000, Character.ToLower(0x110000, CultureInfo.InvariantCulture)); // J2N: To match the behavior of CharUnicodeInfo.GetUnicodeCategory(int), we are throwing an exception rather than passing through invalid codepoints.

            try
            {
                Character.ToLower(0x110000, CultureInfo.InvariantCulture);
                fail("No IOOBE, negative offset");
            }
            catch (ArgumentException e)
            {
            }
        }

        /**
         * @tests java.lang.Character#toLowerCase(char)
         */
        [Test]
        public void Test_ToLowerInvariant_C()
        {
            assertEquals("Failed to change case", 't', Character.ToLowerInvariant('T'));
        }

        /**
         * @tests java.lang.Character#toLowerCase(int)
         */
        [Test]
        public void Test_ToLowerInvariant_I()
        {
            assertEquals('t', Character.ToLowerInvariant((int)'T'));

            assertEquals(0x10428, Character.ToLowerInvariant(0x10400));
            assertEquals(0x10428, Character.ToLowerInvariant(0x10428));

            assertEquals(0x1D504, Character.ToLowerInvariant(0x1D504));
            assertEquals(0x10FFFD, Character.ToLowerInvariant(0x10FFFD));
            //assertEquals(0x110000, Character.ToLowerInvariant(0x110000)); // J2N: To match the behavior of CharUnicodeInfo.GetUnicodeCategory(int), we are throwing an exception rather than passing through invalid codepoints.

            try
            {
                Character.ToLowerInvariant(0x110000);
                fail("No IOOBE, negative offset");
            }
            catch (ArgumentException e)
            {
            }
        }

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

        /**
         * @tests java.lang.Character#toUpperCase(char)
         */
        [Test]
        public void Test_ToUpper_C()
        {
            // Test for method char java.lang.Character.toUpperCase(char)
            assertEquals("Incorrect upper case for a",
                    'A', Character.ToUpper('a', CultureInfo.InvariantCulture));
            assertEquals("Incorrect upper case for A",
                    'A', Character.ToUpper('A', CultureInfo.InvariantCulture));
            assertEquals("Incorrect upper case for 1",
                    '1', Character.ToUpper('1', CultureInfo.InvariantCulture));
        }

        /**
         * @tests java.lang.Character#toUpperCase(int)
         */
        [Test]
        public void Test_ToUpper_I()
        {
            assertEquals('A', Character.ToUpper((int)'a', CultureInfo.InvariantCulture));
            assertEquals('A', Character.ToUpper((int)'A', CultureInfo.InvariantCulture));
            assertEquals('1', Character.ToUpper((int)'1', CultureInfo.InvariantCulture));

            assertEquals(0x10400, Character.ToUpper(0x10428, CultureInfo.InvariantCulture));
            assertEquals(0x10400, Character.ToUpper(0x10400, CultureInfo.InvariantCulture));

            assertEquals(0x10FFFF, Character.ToUpper(0x10FFFF, CultureInfo.InvariantCulture));
            //assertEquals(0x110000, Character.ToUpper(0x110000, CultureInfo.InvariantCulture)); // J2N: To match the behavior of CharUnicodeInfo.GetUnicodeCategory(int), we are throwing an exception rather than passing through invalid codepoints.

            try
            {
                Character.ToUpper(0x110000, CultureInfo.InvariantCulture);
                fail("No IOOBE, negative offset");
            }
            catch (ArgumentException e)
            {
            }
        }

        /**
         * @tests java.lang.Character#toUpperCase(char)
         */
        [Test]
        public void Test_ToUpperInvariant_C()
        {
            // Test for method char java.lang.Character.toUpperCase(char)
            assertEquals("Incorrect upper case for a",
                    'A', Character.ToUpperInvariant('a'));
            assertEquals("Incorrect upper case for A",
                    'A', Character.ToUpperInvariant('A'));
            assertEquals("Incorrect upper case for 1",
                    '1', Character.ToUpperInvariant('1'));
        }

        /**
         * @tests java.lang.Character#toUpperCase(int)
         */
        [Test]
        public void Test_ToUpperInvariant_I()
        {
            assertEquals('A', Character.ToUpperInvariant((int)'a'));
            assertEquals('A', Character.ToUpperInvariant((int)'A'));
            assertEquals('1', Character.ToUpperInvariant((int)'1'));

            assertEquals(0x10400, Character.ToUpperInvariant(0x10428));
            assertEquals(0x10400, Character.ToUpperInvariant(0x10400));

            assertEquals(0x10FFFF, Character.ToUpperInvariant(0x10FFFF));
            //assertEquals(0x110000, Character.ToUpperInvariant(0x110000)); // J2N: To match the behavior of CharUnicodeInfo.GetUnicodeCategory(int), we are throwing an exception rather than passing through invalid codepoints.

            try
            {
                Character.ToUpperInvariant(0x110000);
                fail("No IOOBE, negative offset");
            }
            catch (ArgumentException e)
            {
            }
        }

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


        // J2N: Tess the static Character.ToString() method to ensure it does the same thing as the Java constructor overload of String.
        // Basically, just replaced "new String(" with "Character.ToString(".
        /**
         * @tests java.lang.String#String(int[],int,int)
         */
        [Test]
        public void TestToString_ReadOnlySpanInt32_Int32_Int32() //test_Constructor_III()
        {
            assertEquals("HelloWorld", Character.ToString(new int[] { 'H', 'e', 'l', 'l',
                'o', 'W', 'o', 'r', 'l', 'd' }.AsSpan(), 0, 10));
            assertEquals("Hello", Character.ToString(new int[] { 'H', 'e', 'l', 'l', 'o',
                'W', 'o', 'r', 'l', 'd' }.AsSpan(), 0, 5));
            assertEquals("World", Character.ToString(new int[] { 'H', 'e', 'l', 'l', 'o',
                'W', 'o', 'r', 'l', 'd' }.AsSpan(), 5, 5));
            assertEquals("", Character.ToString(new int[] { 'H', 'e', 'l', 'l', 'o', 'W',
                'o', 'r', 'l', 'd' }.AsSpan(), 5, 0));

            assertEquals("\uD800\uDC00", Character.ToString(new int[] { 0x010000 }.AsSpan(), 0, 1));
            assertEquals("\uD800\uDC00a\uDBFF\uDFFF", Character.ToString(new int[] {
                0x010000, 'a', 0x010FFFF }.AsSpan(), 0, 3));

            //try
            //{
            //    Character.ToString((int[])null, 0, 1);
            //    fail("No NPE");
            //}
            //catch (ArgumentNullException e)
            //{
            //}

            try
            {
                Character.ToString(new int[] { 'a', 'b' }.AsSpan(), -1, 2);
                fail("No IOOBE, negative offset");
            }
            catch (ArgumentOutOfRangeException e)
            {
            }

            try
            {
                Character.ToString(new int[] { 'a', 'b' }.AsSpan(), 0, -1);
                fail("No IOOBE, negative count");
            }
            catch (ArgumentOutOfRangeException e)
            {
            }

            try
            {
                Character.ToString(new int[] { 'a', 'b' }.AsSpan(), 0, -1);
                fail("No IOOBE, negative count");
            }
            catch (ArgumentOutOfRangeException e)
            {
            }

            try
            {
                Character.ToString(new int[] { 'a', 'b' }.AsSpan(), 0, 3);
                fail("No IOOBE, too large");
            }
            catch (ArgumentOutOfRangeException e)
            {
            }
        }

        // J2N: Tess the static Character.ToString() method to ensure it does the same thing as the Java constructor overload of String.
        // Basically, just replaced "new String(" with "Character.ToString(".
        /**
         * @tests java.lang.String#String(int[],int,int)
         */
        [Test]
        public void TestToString_Int32Array_Int32_Int32() //test_Constructor_III()
        {
            assertEquals("HelloWorld", Character.ToString(new int[] { 'H', 'e', 'l', 'l',
                'o', 'W', 'o', 'r', 'l', 'd' }, 0, 10));
            assertEquals("Hello", Character.ToString(new int[] { 'H', 'e', 'l', 'l', 'o',
                'W', 'o', 'r', 'l', 'd' }, 0, 5));
            assertEquals("World", Character.ToString(new int[] { 'H', 'e', 'l', 'l', 'o',
                'W', 'o', 'r', 'l', 'd' }, 5, 5));
            assertEquals("", Character.ToString(new int[] { 'H', 'e', 'l', 'l', 'o', 'W',
                'o', 'r', 'l', 'd' }, 5, 0));

            assertEquals("\uD800\uDC00", Character.ToString(new int[] { 0x010000 }, 0, 1));
            assertEquals("\uD800\uDC00a\uDBFF\uDFFF", Character.ToString(new int[] {
                0x010000, 'a', 0x010FFFF }, 0, 3));

            try
            {
                Character.ToString((int[])null, 0, 1);
                fail("No NPE");
            }
            catch (ArgumentNullException e)
            {
            }

            try
            {
                Character.ToString(new int[] { 'a', 'b' }, -1, 2);
                fail("No IOOBE, negative offset");
            }
            catch (ArgumentOutOfRangeException e)
            {
            }

            try
            {
                Character.ToString(new int[] { 'a', 'b' }, 0, -1);
                fail("No IOOBE, negative count");
            }
            catch (ArgumentOutOfRangeException e)
            {
            }

            try
            {
                Character.ToString(new int[] { 'a', 'b' }, 0, -1);
                fail("No IOOBE, negative count");
            }
            catch (ArgumentOutOfRangeException e)
            {
            }

            try
            {
                Character.ToString(new int[] { 'a', 'b' }, 0, 3);
                fail("No IOOBE, too large");
            }
            catch (ArgumentOutOfRangeException e)
            {
            }
        }

        /// <summary>
        /// Ensures that arrays with lengths below and above thresholds will successfully convert
        /// </summary>
        [Test]
        public void TestToString_ReadOnlySpanInt32_Thresholds()
        {
            // Note: These code points all are defined in Unicode 8.0
            ReadOnlySpan<int> StackThresholdMinusOne = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 80, 81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 91, 92, 93, 94, 95, 96, 97, 98, 99, 100, 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 112, 113, 114, 115, 116, 117, 118, 119, 120, 121, 122, 123, 124, 125, 126, 127, 128, 129, 130, 131, 132, 133, 134, 135, 136, 137, 138, 139, 140, 141, 142, 143, 144, 145, 146, 147, 148, 149, 150, 151, 152, 153, 154, 155, 156, 157, 158, 159, 160, 161, 162, 163, 164, 165, 166, 167, 168, 169, 170, 171, 172, 173, 174, 175, 176, 177, 178, 179, 180, 181, 182, 183, 184, 185, 186, 187, 188, 189, 190, 191, 192, 193, 194, 195, 196, 197, 198, 199, 200, 201, 202, 203, 204, 205, 206, 207, 208, 209, 210, 211, 212, 213, 214, 215, 216, 217, 218, 219, 220, 221, 222, 223, 224, 225, 226, 227, 228, 229, 230, 231, 232, 233, 234, 235, 236, 237, 238, 239, 240, 241, 242, 243, 244, 245, 246, 247, 248, 249, 250, 251, 252, 253, 254, 255 };
            ReadOnlySpan<int> StackThreshold = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 80, 81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 91, 92, 93, 94, 95, 96, 97, 98, 99, 100, 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 112, 113, 114, 115, 116, 117, 118, 119, 120, 121, 122, 123, 124, 125, 126, 127, 128, 129, 130, 131, 132, 133, 134, 135, 136, 137, 138, 139, 140, 141, 142, 143, 144, 145, 146, 147, 148, 149, 150, 151, 152, 153, 154, 155, 156, 157, 158, 159, 160, 161, 162, 163, 164, 165, 166, 167, 168, 169, 170, 171, 172, 173, 174, 175, 176, 177, 178, 179, 180, 181, 182, 183, 184, 185, 186, 187, 188, 189, 190, 191, 192, 193, 194, 195, 196, 197, 198, 199, 200, 201, 202, 203, 204, 205, 206, 207, 208, 209, 210, 211, 212, 213, 214, 215, 216, 217, 218, 219, 220, 221, 222, 223, 224, 225, 226, 227, 228, 229, 230, 231, 232, 233, 234, 235, 236, 237, 238, 239, 240, 241, 242, 243, 244, 245, 246, 247, 248, 249, 250, 251, 252, 253, 254, 255, 256 };
            ReadOnlySpan<int> StackThresholdPlusOne = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 80, 81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 91, 92, 93, 94, 95, 96, 97, 98, 99, 100, 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 112, 113, 114, 115, 116, 117, 118, 119, 120, 121, 122, 123, 124, 125, 126, 127, 128, 129, 130, 131, 132, 133, 134, 135, 136, 137, 138, 139, 140, 141, 142, 143, 144, 145, 146, 147, 148, 149, 150, 151, 152, 153, 154, 155, 156, 157, 158, 159, 160, 161, 162, 163, 164, 165, 166, 167, 168, 169, 170, 171, 172, 173, 174, 175, 176, 177, 178, 179, 180, 181, 182, 183, 184, 185, 186, 187, 188, 189, 190, 191, 192, 193, 194, 195, 196, 197, 198, 199, 200, 201, 202, 203, 204, 205, 206, 207, 208, 209, 210, 211, 212, 213, 214, 215, 216, 217, 218, 219, 220, 221, 222, 223, 224, 225, 226, 227, 228, 229, 230, 231, 232, 233, 234, 235, 236, 237, 238, 239, 240, 241, 242, 243, 244, 245, 246, 247, 248, 249, 250, 251, 252, 253, 254, 255, 256, 257 };

            ReadOnlySpan<int> CountThresholdMinusOne = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 80, 81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 91, 92, 93, 94, 95, 96, 97, 98, 99, 100, 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 112, 113, 114, 115, 116, 117, 118, 119, 120, 121, 122, 123, 124, 125, 126, 127, 128, 129, 130, 131, 132, 133, 134, 135, 136, 137, 138, 139, 140, 141, 142, 143, 144, 145, 146, 147, 148, 149, 150, 151, 152, 153, 154, 155, 156, 157, 158, 159, 160, 161, 162, 163, 164, 165, 166, 167, 168, 169, 170, 171, 172, 173, 174, 175, 176, 177, 178, 179, 180, 181, 182, 183, 184, 185, 186, 187, 188, 189, 190, 191, 192, 193, 194, 195, 196, 197, 198, 199, 200, 201, 202, 203, 204, 205, 206, 207, 208, 209, 210, 211, 212, 213, 214, 215, 216, 217, 218, 219, 220, 221, 222, 223, 224, 225, 226, 227, 228, 229, 230, 231, 232, 233, 234, 235, 236, 237, 238, 239, 240, 241, 242, 243, 244, 245, 246, 247, 248, 249, 250, 251, 252, 253, 254, 255, 256, 257, 258, 259, 260, 261, 262, 263, 264, 265, 266, 267, 268, 269, 270, 271, 272, 273, 274, 275, 276, 277, 278, 279, 280, 281, 282, 283, 284, 285, 286, 287, 288, 289, 290, 291, 292, 293, 294, 295, 296, 297, 298, 299, 300, 301, 302, 303, 304, 305, 306, 307, 308, 309, 310, 311, 312, 313, 314, 315, 316, 317, 318, 319, 320, 321, 322, 323, 324, 325, 326, 327, 328, 329, 330, 331, 332, 333, 334, 335, 336, 337, 338, 339, 340, 341, 342, 343, 344, 345, 346, 347, 348, 349, 350, 351, 352, 353, 354, 355, 356, 357, 358, 359, 360, 361, 362, 363, 364, 365, 366, 367, 368, 369, 370, 371, 372, 373, 374, 375, 376, 377, 378, 379, 380, 381, 382, 383, 384, 385, 386, 387, 388, 389, 390, 391, 392, 393, 394, 395, 396, 397, 398, 399, 400, 401, 402, 403, 404, 405, 406, 407, 408, 409, 410, 411, 412, 413, 414, 415, 416, 417, 418, 419, 420, 421, 422, 423, 424, 425, 426, 427, 428, 429, 430, 431, 432, 433, 434, 435, 436, 437, 438, 439, 440, 441, 442, 443, 444, 445, 446, 447, 448, 449, 450, 451, 452, 453, 454, 455, 456, 457, 458, 459, 460, 461, 462, 463, 464, 465, 466, 467, 468, 469, 470, 471, 472, 473, 474, 475, 476, 477, 478, 479, 480, 481, 482, 483, 484, 485, 486, 487, 488, 489, 490, 491, 492, 493, 494, 495, 496, 497, 498, 499, 500, 501, 502, 503, 504, 505, 506, 507, 508, 509, 510, 511, 512, 513, 514, 515, 516, 517, 518, 519, 520, 521, 522, 523, 524, 525, 526, 527, 528, 529, 530, 531, 532, 533, 534, 535, 536, 537, 538, 539, 540, 541, 542, 543, 544, 545, 546, 547, 548, 549, 550, 551, 552, 553, 554, 555, 556, 557, 558, 559, 560, 561, 562, 563, 564, 565, 566, 567, 568, 569, 570, 571, 572, 573, 574, 575, 576, 577, 578, 579, 580, 581, 582, 583, 584, 585, 586, 587, 588, 589, 590, 591, 592, 593, 594, 595, 596, 597, 598, 599, 600, 601, 602, 603, 604, 605, 606, 607, 608, 609, 610, 611, 612, 613, 614, 615, 616, 617, 618, 619, 620, 621, 622, 623, 624, 625, 626, 627, 628, 629, 630, 631, 632, 633, 634, 635, 636, 637, 638, 639, 640, 641, 642, 643, 644, 645, 646, 647, 648, 649, 650, 651, 652, 653, 654, 655, 656, 657, 658, 659, 660, 661, 662, 663, 664, 665, 666, 667, 668, 669, 670, 671, 672, 673, 674, 675, 676, 677, 678, 679, 680, 681, 682, 683, 684, 685, 686, 687, 688, 689, 690, 691, 692, 693, 694, 695, 696, 697, 698, 699, 700, 701, 702, 703, 704, 705, 706, 707, 708, 709, 710, 711, 712, 713, 714, 715, 716, 717, 718, 719, 720, 721, 722, 723, 724, 725, 726, 727, 728, 729, 730, 731, 732, 733, 734, 735, 736, 737, 738, 739, 740, 741, 742, 743, 744, 745, 746, 747, 748, 749, 750, 751, 752, 753, 754, 755, 756, 757, 758, 759, 760, 761, 762, 763, 764, 765, 766, 767, 768, 769, 770, 771, 772, 773, 774, 775, 776, 777, 778, 779, 780, 781, 782, 783, 784, 785, 786, 787, 788, 789, 790, 791, 792, 793, 794, 795, 796, 797, 798, 799, 800, 801, 802, 803, 804, 805, 806, 807, 808, 809, 810, 811, 812, 813, 814, 815, 816, 817, 818, 819, 820, 821, 822, 823, 824, 825, 826, 827, 828, 829, 830, 831, 832, 833, 834, 835, 836, 837, 838, 839, 840, 841, 842, 843, 844, 845, 846, 847, 848, 849, 850, 851, 852, 853, 854, 855, 856, 857, 858, 859, 860, 861, 862, 863, 864, 865, 866, 867, 868, 869, 870, 871, 872, 873, 874, 875, 876, 877, 878, 879, 880, 881, 882, 883, 884, 885, 886, 887, 890, 891, 892, 893, 894, 895, 900, 901, 902, 903, 904, 905, 906, 908, 910, 911, 912, 913, 914, 915, 916, 917, 918, 919, 920, 921, 922, 923, 924, 925, 926, 927, 928, 929, 931, 932, 933, 934, 935, 936, 937, 938, 939, 940, 941, 942, 943, 944, 945, 946, 947, 948, 949, 950, 951, 952, 953, 954, 955, 956, 957, 958, 959, 960, 961, 962, 963, 964, 965, 966, 967, 968, 969, 970, 971, 972, 973, 974, 975, 976, 977, 978, 979, 980, 981, 982, 983, 984, 985, 986, 987, 988, 989, 990, 991, 992, 993, 994, 995, 996, 997, 998, 999, 1000, 1001, 1002, 1003, 1004, 1005, 1006, 1007, 1008, 1009, 1010, 1011, 1012, 1013, 1014, 1015, 1016, 1017, 1018, 1019, 1020, 1021, 1022, 1023, 1024, 1025, 1026, 1027, 1028, 1029, 1030, 1031, 1032 };
            ReadOnlySpan<int> CountThreshold = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 80, 81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 91, 92, 93, 94, 95, 96, 97, 98, 99, 100, 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 112, 113, 114, 115, 116, 117, 118, 119, 120, 121, 122, 123, 124, 125, 126, 127, 128, 129, 130, 131, 132, 133, 134, 135, 136, 137, 138, 139, 140, 141, 142, 143, 144, 145, 146, 147, 148, 149, 150, 151, 152, 153, 154, 155, 156, 157, 158, 159, 160, 161, 162, 163, 164, 165, 166, 167, 168, 169, 170, 171, 172, 173, 174, 175, 176, 177, 178, 179, 180, 181, 182, 183, 184, 185, 186, 187, 188, 189, 190, 191, 192, 193, 194, 195, 196, 197, 198, 199, 200, 201, 202, 203, 204, 205, 206, 207, 208, 209, 210, 211, 212, 213, 214, 215, 216, 217, 218, 219, 220, 221, 222, 223, 224, 225, 226, 227, 228, 229, 230, 231, 232, 233, 234, 235, 236, 237, 238, 239, 240, 241, 242, 243, 244, 245, 246, 247, 248, 249, 250, 251, 252, 253, 254, 255, 256, 257, 258, 259, 260, 261, 262, 263, 264, 265, 266, 267, 268, 269, 270, 271, 272, 273, 274, 275, 276, 277, 278, 279, 280, 281, 282, 283, 284, 285, 286, 287, 288, 289, 290, 291, 292, 293, 294, 295, 296, 297, 298, 299, 300, 301, 302, 303, 304, 305, 306, 307, 308, 309, 310, 311, 312, 313, 314, 315, 316, 317, 318, 319, 320, 321, 322, 323, 324, 325, 326, 327, 328, 329, 330, 331, 332, 333, 334, 335, 336, 337, 338, 339, 340, 341, 342, 343, 344, 345, 346, 347, 348, 349, 350, 351, 352, 353, 354, 355, 356, 357, 358, 359, 360, 361, 362, 363, 364, 365, 366, 367, 368, 369, 370, 371, 372, 373, 374, 375, 376, 377, 378, 379, 380, 381, 382, 383, 384, 385, 386, 387, 388, 389, 390, 391, 392, 393, 394, 395, 396, 397, 398, 399, 400, 401, 402, 403, 404, 405, 406, 407, 408, 409, 410, 411, 412, 413, 414, 415, 416, 417, 418, 419, 420, 421, 422, 423, 424, 425, 426, 427, 428, 429, 430, 431, 432, 433, 434, 435, 436, 437, 438, 439, 440, 441, 442, 443, 444, 445, 446, 447, 448, 449, 450, 451, 452, 453, 454, 455, 456, 457, 458, 459, 460, 461, 462, 463, 464, 465, 466, 467, 468, 469, 470, 471, 472, 473, 474, 475, 476, 477, 478, 479, 480, 481, 482, 483, 484, 485, 486, 487, 488, 489, 490, 491, 492, 493, 494, 495, 496, 497, 498, 499, 500, 501, 502, 503, 504, 505, 506, 507, 508, 509, 510, 511, 512, 513, 514, 515, 516, 517, 518, 519, 520, 521, 522, 523, 524, 525, 526, 527, 528, 529, 530, 531, 532, 533, 534, 535, 536, 537, 538, 539, 540, 541, 542, 543, 544, 545, 546, 547, 548, 549, 550, 551, 552, 553, 554, 555, 556, 557, 558, 559, 560, 561, 562, 563, 564, 565, 566, 567, 568, 569, 570, 571, 572, 573, 574, 575, 576, 577, 578, 579, 580, 581, 582, 583, 584, 585, 586, 587, 588, 589, 590, 591, 592, 593, 594, 595, 596, 597, 598, 599, 600, 601, 602, 603, 604, 605, 606, 607, 608, 609, 610, 611, 612, 613, 614, 615, 616, 617, 618, 619, 620, 621, 622, 623, 624, 625, 626, 627, 628, 629, 630, 631, 632, 633, 634, 635, 636, 637, 638, 639, 640, 641, 642, 643, 644, 645, 646, 647, 648, 649, 650, 651, 652, 653, 654, 655, 656, 657, 658, 659, 660, 661, 662, 663, 664, 665, 666, 667, 668, 669, 670, 671, 672, 673, 674, 675, 676, 677, 678, 679, 680, 681, 682, 683, 684, 685, 686, 687, 688, 689, 690, 691, 692, 693, 694, 695, 696, 697, 698, 699, 700, 701, 702, 703, 704, 705, 706, 707, 708, 709, 710, 711, 712, 713, 714, 715, 716, 717, 718, 719, 720, 721, 722, 723, 724, 725, 726, 727, 728, 729, 730, 731, 732, 733, 734, 735, 736, 737, 738, 739, 740, 741, 742, 743, 744, 745, 746, 747, 748, 749, 750, 751, 752, 753, 754, 755, 756, 757, 758, 759, 760, 761, 762, 763, 764, 765, 766, 767, 768, 769, 770, 771, 772, 773, 774, 775, 776, 777, 778, 779, 780, 781, 782, 783, 784, 785, 786, 787, 788, 789, 790, 791, 792, 793, 794, 795, 796, 797, 798, 799, 800, 801, 802, 803, 804, 805, 806, 807, 808, 809, 810, 811, 812, 813, 814, 815, 816, 817, 818, 819, 820, 821, 822, 823, 824, 825, 826, 827, 828, 829, 830, 831, 832, 833, 834, 835, 836, 837, 838, 839, 840, 841, 842, 843, 844, 845, 846, 847, 848, 849, 850, 851, 852, 853, 854, 855, 856, 857, 858, 859, 860, 861, 862, 863, 864, 865, 866, 867, 868, 869, 870, 871, 872, 873, 874, 875, 876, 877, 878, 879, 880, 881, 882, 883, 884, 885, 886, 887, 890, 891, 892, 893, 894, 895, 900, 901, 902, 903, 904, 905, 906, 908, 910, 911, 912, 913, 914, 915, 916, 917, 918, 919, 920, 921, 922, 923, 924, 925, 926, 927, 928, 929, 931, 932, 933, 934, 935, 936, 937, 938, 939, 940, 941, 942, 943, 944, 945, 946, 947, 948, 949, 950, 951, 952, 953, 954, 955, 956, 957, 958, 959, 960, 961, 962, 963, 964, 965, 966, 967, 968, 969, 970, 971, 972, 973, 974, 975, 976, 977, 978, 979, 980, 981, 982, 983, 984, 985, 986, 987, 988, 989, 990, 991, 992, 993, 994, 995, 996, 997, 998, 999, 1000, 1001, 1002, 1003, 1004, 1005, 1006, 1007, 1008, 1009, 1010, 1011, 1012, 1013, 1014, 1015, 1016, 1017, 1018, 1019, 1020, 1021, 1022, 1023, 1024, 1025, 1026, 1027, 1028, 1029, 1030, 1031, 1032, 1033 };
            ReadOnlySpan<int> CountThresholdPlusOne = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 80, 81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 91, 92, 93, 94, 95, 96, 97, 98, 99, 100, 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 112, 113, 114, 115, 116, 117, 118, 119, 120, 121, 122, 123, 124, 125, 126, 127, 128, 129, 130, 131, 132, 133, 134, 135, 136, 137, 138, 139, 140, 141, 142, 143, 144, 145, 146, 147, 148, 149, 150, 151, 152, 153, 154, 155, 156, 157, 158, 159, 160, 161, 162, 163, 164, 165, 166, 167, 168, 169, 170, 171, 172, 173, 174, 175, 176, 177, 178, 179, 180, 181, 182, 183, 184, 185, 186, 187, 188, 189, 190, 191, 192, 193, 194, 195, 196, 197, 198, 199, 200, 201, 202, 203, 204, 205, 206, 207, 208, 209, 210, 211, 212, 213, 214, 215, 216, 217, 218, 219, 220, 221, 222, 223, 224, 225, 226, 227, 228, 229, 230, 231, 232, 233, 234, 235, 236, 237, 238, 239, 240, 241, 242, 243, 244, 245, 246, 247, 248, 249, 250, 251, 252, 253, 254, 255, 256, 257, 258, 259, 260, 261, 262, 263, 264, 265, 266, 267, 268, 269, 270, 271, 272, 273, 274, 275, 276, 277, 278, 279, 280, 281, 282, 283, 284, 285, 286, 287, 288, 289, 290, 291, 292, 293, 294, 295, 296, 297, 298, 299, 300, 301, 302, 303, 304, 305, 306, 307, 308, 309, 310, 311, 312, 313, 314, 315, 316, 317, 318, 319, 320, 321, 322, 323, 324, 325, 326, 327, 328, 329, 330, 331, 332, 333, 334, 335, 336, 337, 338, 339, 340, 341, 342, 343, 344, 345, 346, 347, 348, 349, 350, 351, 352, 353, 354, 355, 356, 357, 358, 359, 360, 361, 362, 363, 364, 365, 366, 367, 368, 369, 370, 371, 372, 373, 374, 375, 376, 377, 378, 379, 380, 381, 382, 383, 384, 385, 386, 387, 388, 389, 390, 391, 392, 393, 394, 395, 396, 397, 398, 399, 400, 401, 402, 403, 404, 405, 406, 407, 408, 409, 410, 411, 412, 413, 414, 415, 416, 417, 418, 419, 420, 421, 422, 423, 424, 425, 426, 427, 428, 429, 430, 431, 432, 433, 434, 435, 436, 437, 438, 439, 440, 441, 442, 443, 444, 445, 446, 447, 448, 449, 450, 451, 452, 453, 454, 455, 456, 457, 458, 459, 460, 461, 462, 463, 464, 465, 466, 467, 468, 469, 470, 471, 472, 473, 474, 475, 476, 477, 478, 479, 480, 481, 482, 483, 484, 485, 486, 487, 488, 489, 490, 491, 492, 493, 494, 495, 496, 497, 498, 499, 500, 501, 502, 503, 504, 505, 506, 507, 508, 509, 510, 511, 512, 513, 514, 515, 516, 517, 518, 519, 520, 521, 522, 523, 524, 525, 526, 527, 528, 529, 530, 531, 532, 533, 534, 535, 536, 537, 538, 539, 540, 541, 542, 543, 544, 545, 546, 547, 548, 549, 550, 551, 552, 553, 554, 555, 556, 557, 558, 559, 560, 561, 562, 563, 564, 565, 566, 567, 568, 569, 570, 571, 572, 573, 574, 575, 576, 577, 578, 579, 580, 581, 582, 583, 584, 585, 586, 587, 588, 589, 590, 591, 592, 593, 594, 595, 596, 597, 598, 599, 600, 601, 602, 603, 604, 605, 606, 607, 608, 609, 610, 611, 612, 613, 614, 615, 616, 617, 618, 619, 620, 621, 622, 623, 624, 625, 626, 627, 628, 629, 630, 631, 632, 633, 634, 635, 636, 637, 638, 639, 640, 641, 642, 643, 644, 645, 646, 647, 648, 649, 650, 651, 652, 653, 654, 655, 656, 657, 658, 659, 660, 661, 662, 663, 664, 665, 666, 667, 668, 669, 670, 671, 672, 673, 674, 675, 676, 677, 678, 679, 680, 681, 682, 683, 684, 685, 686, 687, 688, 689, 690, 691, 692, 693, 694, 695, 696, 697, 698, 699, 700, 701, 702, 703, 704, 705, 706, 707, 708, 709, 710, 711, 712, 713, 714, 715, 716, 717, 718, 719, 720, 721, 722, 723, 724, 725, 726, 727, 728, 729, 730, 731, 732, 733, 734, 735, 736, 737, 738, 739, 740, 741, 742, 743, 744, 745, 746, 747, 748, 749, 750, 751, 752, 753, 754, 755, 756, 757, 758, 759, 760, 761, 762, 763, 764, 765, 766, 767, 768, 769, 770, 771, 772, 773, 774, 775, 776, 777, 778, 779, 780, 781, 782, 783, 784, 785, 786, 787, 788, 789, 790, 791, 792, 793, 794, 795, 796, 797, 798, 799, 800, 801, 802, 803, 804, 805, 806, 807, 808, 809, 810, 811, 812, 813, 814, 815, 816, 817, 818, 819, 820, 821, 822, 823, 824, 825, 826, 827, 828, 829, 830, 831, 832, 833, 834, 835, 836, 837, 838, 839, 840, 841, 842, 843, 844, 845, 846, 847, 848, 849, 850, 851, 852, 853, 854, 855, 856, 857, 858, 859, 860, 861, 862, 863, 864, 865, 866, 867, 868, 869, 870, 871, 872, 873, 874, 875, 876, 877, 878, 879, 880, 881, 882, 883, 884, 885, 886, 887, 890, 891, 892, 893, 894, 895, 900, 901, 902, 903, 904, 905, 906, 908, 910, 911, 912, 913, 914, 915, 916, 917, 918, 919, 920, 921, 922, 923, 924, 925, 926, 927, 928, 929, 931, 932, 933, 934, 935, 936, 937, 938, 939, 940, 941, 942, 943, 944, 945, 946, 947, 948, 949, 950, 951, 952, 953, 954, 955, 956, 957, 958, 959, 960, 961, 962, 963, 964, 965, 966, 967, 968, 969, 970, 971, 972, 973, 974, 975, 976, 977, 978, 979, 980, 981, 982, 983, 984, 985, 986, 987, 988, 989, 990, 991, 992, 993, 994, 995, 996, 997, 998, 999, 1000, 1001, 1002, 1003, 1004, 1005, 1006, 1007, 1008, 1009, 1010, 1011, 1012, 1013, 1014, 1015, 1016, 1017, 1018, 1019, 1020, 1021, 1022, 1023, 1024, 1025, 1026, 1027, 1028, 1029, 1030, 1031, 1032, 1033, 1034 };

            string actual, expected;

            actual = Character.ToString(StackThresholdMinusOne);
            expected = "\u0001\u0002\u0003\u0004\u0005\u0006\a\b\t\n\v\f\r\u000e\u000f\u0010\u0011\u0012\u0013\u0014\u0015\u0016\u0017\u0018\u0019\u001a\u001b\u001c\u001d\u001e\u001f !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~\u007f\u0080\u0081\u0082\u0083\u0084\u0085\u0086\u0087\u0088\u0089\u008a\u008b\u008c\u008d\u008e\u008f\u0090\u0091\u0092\u0093\u0094\u0095\u0096\u0097\u0098\u0099\u009a\u009b\u009c\u009d\u009e\u009f ¡¢£¤¥¦§¨©ª«¬­®¯°±²³´µ¶·¸¹º»¼½¾¿ÀÁÂÃÄÅÆÇÈÉÊËÌÍÎÏÐÑÒÓÔÕÖ×ØÙÚÛÜÝÞßàáâãäåæçèéêëìíîïðñòóôõö÷øùúûüýþÿ";
            assertEquals(expected.Length, StackThresholdMinusOne.Length);
            assertEquals(expected, actual);

            actual = Character.ToString(StackThreshold);
            expected = "\u0001\u0002\u0003\u0004\u0005\u0006\a\b\t\n\v\f\r\u000e\u000f\u0010\u0011\u0012\u0013\u0014\u0015\u0016\u0017\u0018\u0019\u001a\u001b\u001c\u001d\u001e\u001f !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~\u007f\u0080\u0081\u0082\u0083\u0084\u0085\u0086\u0087\u0088\u0089\u008a\u008b\u008c\u008d\u008e\u008f\u0090\u0091\u0092\u0093\u0094\u0095\u0096\u0097\u0098\u0099\u009a\u009b\u009c\u009d\u009e\u009f ¡¢£¤¥¦§¨©ª«¬­®¯°±²³´µ¶·¸¹º»¼½¾¿ÀÁÂÃÄÅÆÇÈÉÊËÌÍÎÏÐÑÒÓÔÕÖ×ØÙÚÛÜÝÞßàáâãäåæçèéêëìíîïðñòóôõö÷øùúûüýþÿĀ";
            assertEquals(expected.Length, StackThreshold.Length);
            assertEquals(expected, actual);

            actual = Character.ToString(StackThresholdPlusOne);
            expected = "\u0001\u0002\u0003\u0004\u0005\u0006\a\b\t\n\v\f\r\u000e\u000f\u0010\u0011\u0012\u0013\u0014\u0015\u0016\u0017\u0018\u0019\u001a\u001b\u001c\u001d\u001e\u001f !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~\u007f\u0080\u0081\u0082\u0083\u0084\u0085\u0086\u0087\u0088\u0089\u008a\u008b\u008c\u008d\u008e\u008f\u0090\u0091\u0092\u0093\u0094\u0095\u0096\u0097\u0098\u0099\u009a\u009b\u009c\u009d\u009e\u009f ¡¢£¤¥¦§¨©ª«¬­®¯°±²³´µ¶·¸¹º»¼½¾¿ÀÁÂÃÄÅÆÇÈÉÊËÌÍÎÏÐÑÒÓÔÕÖ×ØÙÚÛÜÝÞßàáâãäåæçèéêëìíîïðñòóôõö÷øùúûüýþÿĀā";
            assertEquals(expected.Length, StackThresholdPlusOne.Length);
            assertEquals(expected, actual);


            actual = Character.ToString(CountThresholdMinusOne);
            expected = "\u0001\u0002\u0003\u0004\u0005\u0006\a\b\t\n\v\f\r\u000e\u000f\u0010\u0011\u0012\u0013\u0014\u0015\u0016\u0017\u0018\u0019\u001a\u001b\u001c\u001d\u001e\u001f !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~\u007f\u0080\u0081\u0082\u0083\u0084\u0085\u0086\u0087\u0088\u0089\u008a\u008b\u008c\u008d\u008e\u008f\u0090\u0091\u0092\u0093\u0094\u0095\u0096\u0097\u0098\u0099\u009a\u009b\u009c\u009d\u009e\u009f ¡¢£¤¥¦§¨©ª«¬­®¯°±²³´µ¶·¸¹º»¼½¾¿ÀÁÂÃÄÅÆÇÈÉÊËÌÍÎÏÐÑÒÓÔÕÖ×ØÙÚÛÜÝÞßàáâãäåæçèéêëìíîïðñòóôõö÷øùúûüýþÿĀāĂăĄąĆćĈĉĊċČčĎďĐđĒēĔĕĖėĘęĚěĜĝĞğĠġĢģĤĥĦħĨĩĪīĬĭĮįİıĲĳĴĵĶķĸĹĺĻļĽľĿŀŁłŃńŅņŇňŉŊŋŌōŎŏŐőŒœŔŕŖŗŘřŚśŜŝŞşŠšŢţŤťŦŧŨũŪūŬŭŮůŰűŲųŴŵŶŷŸŹźŻżŽžſƀƁƂƃƄƅƆƇƈƉƊƋƌƍƎƏƐƑƒƓƔƕƖƗƘƙƚƛƜƝƞƟƠơƢƣƤƥƦƧƨƩƪƫƬƭƮƯưƱƲƳƴƵƶƷƸƹƺƻƼƽƾƿǀǁǂǃǄǅǆǇǈǉǊǋǌǍǎǏǐǑǒǓǔǕǖǗǘǙǚǛǜǝǞǟǠǡǢǣǤǥǦǧǨǩǪǫǬǭǮǯǰǱǲǳǴǵǶǷǸǹǺǻǼǽǾǿȀȁȂȃȄȅȆȇȈȉȊȋȌȍȎȏȐȑȒȓȔȕȖȗȘșȚțȜȝȞȟȠȡȢȣȤȥȦȧȨȩȪȫȬȭȮȯȰȱȲȳȴȵȶȷȸȹȺȻȼȽȾȿɀɁɂɃɄɅɆɇɈɉɊɋɌɍɎɏɐɑɒɓɔɕɖɗɘəɚɛɜɝɞɟɠɡɢɣɤɥɦɧɨɩɪɫɬɭɮɯɰɱɲɳɴɵɶɷɸɹɺɻɼɽɾɿʀʁʂʃʄʅʆʇʈʉʊʋʌʍʎʏʐʑʒʓʔʕʖʗʘʙʚʛʜʝʞʟʠʡʢʣʤʥʦʧʨʩʪʫʬʭʮʯʰʱʲʳʴʵʶʷʸʹʺʻʼʽʾʿˀˁ˂˃˄˅ˆˇˈˉˊˋˌˍˎˏːˑ˒˓˔˕˖˗˘˙˚˛˜˝˞˟ˠˡˢˣˤ˥˦˧˨˩˪˫ˬ˭ˮ˯˰˱˲˳˴˵˶˷˸˹˺˻˼˽˾˿̴̵̶̷̸̡̢̧̨̛̖̗̘̙̜̝̞̟̠̣̤̥̦̩̪̫̬̭̮̯̰̱̲̳̹̺̻̼͇͈͉͍͎̀́̂̃̄̅̆̇̈̉̊̋̌̍̎̏̐̑̒̓̔̽̾̿̀́͂̓̈́͆͊͋͌̕̚ͅ͏͓͔͕͖͙͚͐͑͒͗͛ͣͤͥͦͧͨͩͪͫͬͭͮͯ͘͜͟͢͝͞͠͡ͰͱͲͳʹ͵Ͷͷͺͻͼͽ;Ϳ΄΅Ά·ΈΉΊΌΎΏΐΑΒΓΔΕΖΗΘΙΚΛΜΝΞΟΠΡΣΤΥΦΧΨΩΪΫάέήίΰαβγδεζηθικλμνξοπρςστυφχψωϊϋόύώϏϐϑϒϓϔϕϖϗϘϙϚϛϜϝϞϟϠϡϢϣϤϥϦϧϨϩϪϫϬϭϮϯϰϱϲϳϴϵ϶ϷϸϹϺϻϼϽϾϿЀЁЂЃЄЅІЇЈ";
            assertEquals(expected.Length, CountThresholdMinusOne.Length);
            assertEquals(expected, actual);

            actual = Character.ToString(CountThreshold);
            expected = "\u0001\u0002\u0003\u0004\u0005\u0006\a\b\t\n\v\f\r\u000e\u000f\u0010\u0011\u0012\u0013\u0014\u0015\u0016\u0017\u0018\u0019\u001a\u001b\u001c\u001d\u001e\u001f !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~\u007f\u0080\u0081\u0082\u0083\u0084\u0085\u0086\u0087\u0088\u0089\u008a\u008b\u008c\u008d\u008e\u008f\u0090\u0091\u0092\u0093\u0094\u0095\u0096\u0097\u0098\u0099\u009a\u009b\u009c\u009d\u009e\u009f ¡¢£¤¥¦§¨©ª«¬­®¯°±²³´µ¶·¸¹º»¼½¾¿ÀÁÂÃÄÅÆÇÈÉÊËÌÍÎÏÐÑÒÓÔÕÖ×ØÙÚÛÜÝÞßàáâãäåæçèéêëìíîïðñòóôõö÷øùúûüýþÿĀāĂăĄąĆćĈĉĊċČčĎďĐđĒēĔĕĖėĘęĚěĜĝĞğĠġĢģĤĥĦħĨĩĪīĬĭĮįİıĲĳĴĵĶķĸĹĺĻļĽľĿŀŁłŃńŅņŇňŉŊŋŌōŎŏŐőŒœŔŕŖŗŘřŚśŜŝŞşŠšŢţŤťŦŧŨũŪūŬŭŮůŰűŲųŴŵŶŷŸŹźŻżŽžſƀƁƂƃƄƅƆƇƈƉƊƋƌƍƎƏƐƑƒƓƔƕƖƗƘƙƚƛƜƝƞƟƠơƢƣƤƥƦƧƨƩƪƫƬƭƮƯưƱƲƳƴƵƶƷƸƹƺƻƼƽƾƿǀǁǂǃǄǅǆǇǈǉǊǋǌǍǎǏǐǑǒǓǔǕǖǗǘǙǚǛǜǝǞǟǠǡǢǣǤǥǦǧǨǩǪǫǬǭǮǯǰǱǲǳǴǵǶǷǸǹǺǻǼǽǾǿȀȁȂȃȄȅȆȇȈȉȊȋȌȍȎȏȐȑȒȓȔȕȖȗȘșȚțȜȝȞȟȠȡȢȣȤȥȦȧȨȩȪȫȬȭȮȯȰȱȲȳȴȵȶȷȸȹȺȻȼȽȾȿɀɁɂɃɄɅɆɇɈɉɊɋɌɍɎɏɐɑɒɓɔɕɖɗɘəɚɛɜɝɞɟɠɡɢɣɤɥɦɧɨɩɪɫɬɭɮɯɰɱɲɳɴɵɶɷɸɹɺɻɼɽɾɿʀʁʂʃʄʅʆʇʈʉʊʋʌʍʎʏʐʑʒʓʔʕʖʗʘʙʚʛʜʝʞʟʠʡʢʣʤʥʦʧʨʩʪʫʬʭʮʯʰʱʲʳʴʵʶʷʸʹʺʻʼʽʾʿˀˁ˂˃˄˅ˆˇˈˉˊˋˌˍˎˏːˑ˒˓˔˕˖˗˘˙˚˛˜˝˞˟ˠˡˢˣˤ˥˦˧˨˩˪˫ˬ˭ˮ˯˰˱˲˳˴˵˶˷˸˹˺˻˼˽˾˿̴̵̶̷̸̡̢̧̨̛̖̗̘̙̜̝̞̟̠̣̤̥̦̩̪̫̬̭̮̯̰̱̲̳̹̺̻̼͇͈͉͍͎̀́̂̃̄̅̆̇̈̉̊̋̌̍̎̏̐̑̒̓̔̽̾̿̀́͂̓̈́͆͊͋͌̕̚ͅ͏͓͔͕͖͙͚͐͑͒͗͛ͣͤͥͦͧͨͩͪͫͬͭͮͯ͘͜͟͢͝͞͠͡ͰͱͲͳʹ͵Ͷͷͺͻͼͽ;Ϳ΄΅Ά·ΈΉΊΌΎΏΐΑΒΓΔΕΖΗΘΙΚΛΜΝΞΟΠΡΣΤΥΦΧΨΩΪΫάέήίΰαβγδεζηθικλμνξοπρςστυφχψωϊϋόύώϏϐϑϒϓϔϕϖϗϘϙϚϛϜϝϞϟϠϡϢϣϤϥϦϧϨϩϪϫϬϭϮϯϰϱϲϳϴϵ϶ϷϸϹϺϻϼϽϾϿЀЁЂЃЄЅІЇЈЉ";
            assertEquals(expected.Length, CountThreshold.Length);
            assertEquals(expected, actual);

            actual = Character.ToString(CountThresholdPlusOne);
            expected = "\u0001\u0002\u0003\u0004\u0005\u0006\a\b\t\n\v\f\r\u000e\u000f\u0010\u0011\u0012\u0013\u0014\u0015\u0016\u0017\u0018\u0019\u001a\u001b\u001c\u001d\u001e\u001f !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~\u007f\u0080\u0081\u0082\u0083\u0084\u0085\u0086\u0087\u0088\u0089\u008a\u008b\u008c\u008d\u008e\u008f\u0090\u0091\u0092\u0093\u0094\u0095\u0096\u0097\u0098\u0099\u009a\u009b\u009c\u009d\u009e\u009f ¡¢£¤¥¦§¨©ª«¬­®¯°±²³´µ¶·¸¹º»¼½¾¿ÀÁÂÃÄÅÆÇÈÉÊËÌÍÎÏÐÑÒÓÔÕÖ×ØÙÚÛÜÝÞßàáâãäåæçèéêëìíîïðñòóôõö÷øùúûüýþÿĀāĂăĄąĆćĈĉĊċČčĎďĐđĒēĔĕĖėĘęĚěĜĝĞğĠġĢģĤĥĦħĨĩĪīĬĭĮįİıĲĳĴĵĶķĸĹĺĻļĽľĿŀŁłŃńŅņŇňŉŊŋŌōŎŏŐőŒœŔŕŖŗŘřŚśŜŝŞşŠšŢţŤťŦŧŨũŪūŬŭŮůŰűŲųŴŵŶŷŸŹźŻżŽžſƀƁƂƃƄƅƆƇƈƉƊƋƌƍƎƏƐƑƒƓƔƕƖƗƘƙƚƛƜƝƞƟƠơƢƣƤƥƦƧƨƩƪƫƬƭƮƯưƱƲƳƴƵƶƷƸƹƺƻƼƽƾƿǀǁǂǃǄǅǆǇǈǉǊǋǌǍǎǏǐǑǒǓǔǕǖǗǘǙǚǛǜǝǞǟǠǡǢǣǤǥǦǧǨǩǪǫǬǭǮǯǰǱǲǳǴǵǶǷǸǹǺǻǼǽǾǿȀȁȂȃȄȅȆȇȈȉȊȋȌȍȎȏȐȑȒȓȔȕȖȗȘșȚțȜȝȞȟȠȡȢȣȤȥȦȧȨȩȪȫȬȭȮȯȰȱȲȳȴȵȶȷȸȹȺȻȼȽȾȿɀɁɂɃɄɅɆɇɈɉɊɋɌɍɎɏɐɑɒɓɔɕɖɗɘəɚɛɜɝɞɟɠɡɢɣɤɥɦɧɨɩɪɫɬɭɮɯɰɱɲɳɴɵɶɷɸɹɺɻɼɽɾɿʀʁʂʃʄʅʆʇʈʉʊʋʌʍʎʏʐʑʒʓʔʕʖʗʘʙʚʛʜʝʞʟʠʡʢʣʤʥʦʧʨʩʪʫʬʭʮʯʰʱʲʳʴʵʶʷʸʹʺʻʼʽʾʿˀˁ˂˃˄˅ˆˇˈˉˊˋˌˍˎˏːˑ˒˓˔˕˖˗˘˙˚˛˜˝˞˟ˠˡˢˣˤ˥˦˧˨˩˪˫ˬ˭ˮ˯˰˱˲˳˴˵˶˷˸˹˺˻˼˽˾˿̴̵̶̷̸̡̢̧̨̛̖̗̘̙̜̝̞̟̠̣̤̥̦̩̪̫̬̭̮̯̰̱̲̳̹̺̻̼͇͈͉͍͎̀́̂̃̄̅̆̇̈̉̊̋̌̍̎̏̐̑̒̓̔̽̾̿̀́͂̓̈́͆͊͋͌̕̚ͅ͏͓͔͕͖͙͚͐͑͒͗͛ͣͤͥͦͧͨͩͪͫͬͭͮͯ͘͜͟͢͝͞͠͡ͰͱͲͳʹ͵Ͷͷͺͻͼͽ;Ϳ΄΅Ά·ΈΉΊΌΎΏΐΑΒΓΔΕΖΗΘΙΚΛΜΝΞΟΠΡΣΤΥΦΧΨΩΪΫάέήίΰαβγδεζηθικλμνξοπρςστυφχψωϊϋόύώϏϐϑϒϓϔϕϖϗϘϙϚϛϜϝϞϟϠϡϢϣϤϥϦϧϨϩϪϫϬϭϮϯϰϱϲϳϴϵ϶ϷϸϹϺϻϼϽϾϿЀЁЂЃЄЅІЇЈЉЊ";
            assertEquals(expected.Length, CountThresholdPlusOne.Length);
            assertEquals(expected, actual);
        }

        /// <summary>
        /// Ensures that arrays with lengths below and above thresholds will successfully convert
        /// </summary>
        [Test]
        public void TestToString_Int32Array_Thresholds()
        {
            // Note: These code points all are defined in Unicode 8.0
            int[] StackThresholdMinusOne = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 80, 81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 91, 92, 93, 94, 95, 96, 97, 98, 99, 100, 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 112, 113, 114, 115, 116, 117, 118, 119, 120, 121, 122, 123, 124, 125, 126, 127, 128, 129, 130, 131, 132, 133, 134, 135, 136, 137, 138, 139, 140, 141, 142, 143, 144, 145, 146, 147, 148, 149, 150, 151, 152, 153, 154, 155, 156, 157, 158, 159, 160, 161, 162, 163, 164, 165, 166, 167, 168, 169, 170, 171, 172, 173, 174, 175, 176, 177, 178, 179, 180, 181, 182, 183, 184, 185, 186, 187, 188, 189, 190, 191, 192, 193, 194, 195, 196, 197, 198, 199, 200, 201, 202, 203, 204, 205, 206, 207, 208, 209, 210, 211, 212, 213, 214, 215, 216, 217, 218, 219, 220, 221, 222, 223, 224, 225, 226, 227, 228, 229, 230, 231, 232, 233, 234, 235, 236, 237, 238, 239, 240, 241, 242, 243, 244, 245, 246, 247, 248, 249, 250, 251, 252, 253, 254, 255 };
            int[] StackThreshold = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 80, 81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 91, 92, 93, 94, 95, 96, 97, 98, 99, 100, 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 112, 113, 114, 115, 116, 117, 118, 119, 120, 121, 122, 123, 124, 125, 126, 127, 128, 129, 130, 131, 132, 133, 134, 135, 136, 137, 138, 139, 140, 141, 142, 143, 144, 145, 146, 147, 148, 149, 150, 151, 152, 153, 154, 155, 156, 157, 158, 159, 160, 161, 162, 163, 164, 165, 166, 167, 168, 169, 170, 171, 172, 173, 174, 175, 176, 177, 178, 179, 180, 181, 182, 183, 184, 185, 186, 187, 188, 189, 190, 191, 192, 193, 194, 195, 196, 197, 198, 199, 200, 201, 202, 203, 204, 205, 206, 207, 208, 209, 210, 211, 212, 213, 214, 215, 216, 217, 218, 219, 220, 221, 222, 223, 224, 225, 226, 227, 228, 229, 230, 231, 232, 233, 234, 235, 236, 237, 238, 239, 240, 241, 242, 243, 244, 245, 246, 247, 248, 249, 250, 251, 252, 253, 254, 255, 256 };
            int[] StackThresholdPlusOne = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 80, 81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 91, 92, 93, 94, 95, 96, 97, 98, 99, 100, 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 112, 113, 114, 115, 116, 117, 118, 119, 120, 121, 122, 123, 124, 125, 126, 127, 128, 129, 130, 131, 132, 133, 134, 135, 136, 137, 138, 139, 140, 141, 142, 143, 144, 145, 146, 147, 148, 149, 150, 151, 152, 153, 154, 155, 156, 157, 158, 159, 160, 161, 162, 163, 164, 165, 166, 167, 168, 169, 170, 171, 172, 173, 174, 175, 176, 177, 178, 179, 180, 181, 182, 183, 184, 185, 186, 187, 188, 189, 190, 191, 192, 193, 194, 195, 196, 197, 198, 199, 200, 201, 202, 203, 204, 205, 206, 207, 208, 209, 210, 211, 212, 213, 214, 215, 216, 217, 218, 219, 220, 221, 222, 223, 224, 225, 226, 227, 228, 229, 230, 231, 232, 233, 234, 235, 236, 237, 238, 239, 240, 241, 242, 243, 244, 245, 246, 247, 248, 249, 250, 251, 252, 253, 254, 255, 256, 257 };

            int[] CountThresholdMinusOne = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 80, 81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 91, 92, 93, 94, 95, 96, 97, 98, 99, 100, 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 112, 113, 114, 115, 116, 117, 118, 119, 120, 121, 122, 123, 124, 125, 126, 127, 128, 129, 130, 131, 132, 133, 134, 135, 136, 137, 138, 139, 140, 141, 142, 143, 144, 145, 146, 147, 148, 149, 150, 151, 152, 153, 154, 155, 156, 157, 158, 159, 160, 161, 162, 163, 164, 165, 166, 167, 168, 169, 170, 171, 172, 173, 174, 175, 176, 177, 178, 179, 180, 181, 182, 183, 184, 185, 186, 187, 188, 189, 190, 191, 192, 193, 194, 195, 196, 197, 198, 199, 200, 201, 202, 203, 204, 205, 206, 207, 208, 209, 210, 211, 212, 213, 214, 215, 216, 217, 218, 219, 220, 221, 222, 223, 224, 225, 226, 227, 228, 229, 230, 231, 232, 233, 234, 235, 236, 237, 238, 239, 240, 241, 242, 243, 244, 245, 246, 247, 248, 249, 250, 251, 252, 253, 254, 255, 256, 257, 258, 259, 260, 261, 262, 263, 264, 265, 266, 267, 268, 269, 270, 271, 272, 273, 274, 275, 276, 277, 278, 279, 280, 281, 282, 283, 284, 285, 286, 287, 288, 289, 290, 291, 292, 293, 294, 295, 296, 297, 298, 299, 300, 301, 302, 303, 304, 305, 306, 307, 308, 309, 310, 311, 312, 313, 314, 315, 316, 317, 318, 319, 320, 321, 322, 323, 324, 325, 326, 327, 328, 329, 330, 331, 332, 333, 334, 335, 336, 337, 338, 339, 340, 341, 342, 343, 344, 345, 346, 347, 348, 349, 350, 351, 352, 353, 354, 355, 356, 357, 358, 359, 360, 361, 362, 363, 364, 365, 366, 367, 368, 369, 370, 371, 372, 373, 374, 375, 376, 377, 378, 379, 380, 381, 382, 383, 384, 385, 386, 387, 388, 389, 390, 391, 392, 393, 394, 395, 396, 397, 398, 399, 400, 401, 402, 403, 404, 405, 406, 407, 408, 409, 410, 411, 412, 413, 414, 415, 416, 417, 418, 419, 420, 421, 422, 423, 424, 425, 426, 427, 428, 429, 430, 431, 432, 433, 434, 435, 436, 437, 438, 439, 440, 441, 442, 443, 444, 445, 446, 447, 448, 449, 450, 451, 452, 453, 454, 455, 456, 457, 458, 459, 460, 461, 462, 463, 464, 465, 466, 467, 468, 469, 470, 471, 472, 473, 474, 475, 476, 477, 478, 479, 480, 481, 482, 483, 484, 485, 486, 487, 488, 489, 490, 491, 492, 493, 494, 495, 496, 497, 498, 499, 500, 501, 502, 503, 504, 505, 506, 507, 508, 509, 510, 511, 512, 513, 514, 515, 516, 517, 518, 519, 520, 521, 522, 523, 524, 525, 526, 527, 528, 529, 530, 531, 532, 533, 534, 535, 536, 537, 538, 539, 540, 541, 542, 543, 544, 545, 546, 547, 548, 549, 550, 551, 552, 553, 554, 555, 556, 557, 558, 559, 560, 561, 562, 563, 564, 565, 566, 567, 568, 569, 570, 571, 572, 573, 574, 575, 576, 577, 578, 579, 580, 581, 582, 583, 584, 585, 586, 587, 588, 589, 590, 591, 592, 593, 594, 595, 596, 597, 598, 599, 600, 601, 602, 603, 604, 605, 606, 607, 608, 609, 610, 611, 612, 613, 614, 615, 616, 617, 618, 619, 620, 621, 622, 623, 624, 625, 626, 627, 628, 629, 630, 631, 632, 633, 634, 635, 636, 637, 638, 639, 640, 641, 642, 643, 644, 645, 646, 647, 648, 649, 650, 651, 652, 653, 654, 655, 656, 657, 658, 659, 660, 661, 662, 663, 664, 665, 666, 667, 668, 669, 670, 671, 672, 673, 674, 675, 676, 677, 678, 679, 680, 681, 682, 683, 684, 685, 686, 687, 688, 689, 690, 691, 692, 693, 694, 695, 696, 697, 698, 699, 700, 701, 702, 703, 704, 705, 706, 707, 708, 709, 710, 711, 712, 713, 714, 715, 716, 717, 718, 719, 720, 721, 722, 723, 724, 725, 726, 727, 728, 729, 730, 731, 732, 733, 734, 735, 736, 737, 738, 739, 740, 741, 742, 743, 744, 745, 746, 747, 748, 749, 750, 751, 752, 753, 754, 755, 756, 757, 758, 759, 760, 761, 762, 763, 764, 765, 766, 767, 768, 769, 770, 771, 772, 773, 774, 775, 776, 777, 778, 779, 780, 781, 782, 783, 784, 785, 786, 787, 788, 789, 790, 791, 792, 793, 794, 795, 796, 797, 798, 799, 800, 801, 802, 803, 804, 805, 806, 807, 808, 809, 810, 811, 812, 813, 814, 815, 816, 817, 818, 819, 820, 821, 822, 823, 824, 825, 826, 827, 828, 829, 830, 831, 832, 833, 834, 835, 836, 837, 838, 839, 840, 841, 842, 843, 844, 845, 846, 847, 848, 849, 850, 851, 852, 853, 854, 855, 856, 857, 858, 859, 860, 861, 862, 863, 864, 865, 866, 867, 868, 869, 870, 871, 872, 873, 874, 875, 876, 877, 878, 879, 880, 881, 882, 883, 884, 885, 886, 887, 890, 891, 892, 893, 894, 895, 900, 901, 902, 903, 904, 905, 906, 908, 910, 911, 912, 913, 914, 915, 916, 917, 918, 919, 920, 921, 922, 923, 924, 925, 926, 927, 928, 929, 931, 932, 933, 934, 935, 936, 937, 938, 939, 940, 941, 942, 943, 944, 945, 946, 947, 948, 949, 950, 951, 952, 953, 954, 955, 956, 957, 958, 959, 960, 961, 962, 963, 964, 965, 966, 967, 968, 969, 970, 971, 972, 973, 974, 975, 976, 977, 978, 979, 980, 981, 982, 983, 984, 985, 986, 987, 988, 989, 990, 991, 992, 993, 994, 995, 996, 997, 998, 999, 1000, 1001, 1002, 1003, 1004, 1005, 1006, 1007, 1008, 1009, 1010, 1011, 1012, 1013, 1014, 1015, 1016, 1017, 1018, 1019, 1020, 1021, 1022, 1023, 1024, 1025, 1026, 1027, 1028, 1029, 1030, 1031, 1032 };
            int[] CountThreshold = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 80, 81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 91, 92, 93, 94, 95, 96, 97, 98, 99, 100, 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 112, 113, 114, 115, 116, 117, 118, 119, 120, 121, 122, 123, 124, 125, 126, 127, 128, 129, 130, 131, 132, 133, 134, 135, 136, 137, 138, 139, 140, 141, 142, 143, 144, 145, 146, 147, 148, 149, 150, 151, 152, 153, 154, 155, 156, 157, 158, 159, 160, 161, 162, 163, 164, 165, 166, 167, 168, 169, 170, 171, 172, 173, 174, 175, 176, 177, 178, 179, 180, 181, 182, 183, 184, 185, 186, 187, 188, 189, 190, 191, 192, 193, 194, 195, 196, 197, 198, 199, 200, 201, 202, 203, 204, 205, 206, 207, 208, 209, 210, 211, 212, 213, 214, 215, 216, 217, 218, 219, 220, 221, 222, 223, 224, 225, 226, 227, 228, 229, 230, 231, 232, 233, 234, 235, 236, 237, 238, 239, 240, 241, 242, 243, 244, 245, 246, 247, 248, 249, 250, 251, 252, 253, 254, 255, 256, 257, 258, 259, 260, 261, 262, 263, 264, 265, 266, 267, 268, 269, 270, 271, 272, 273, 274, 275, 276, 277, 278, 279, 280, 281, 282, 283, 284, 285, 286, 287, 288, 289, 290, 291, 292, 293, 294, 295, 296, 297, 298, 299, 300, 301, 302, 303, 304, 305, 306, 307, 308, 309, 310, 311, 312, 313, 314, 315, 316, 317, 318, 319, 320, 321, 322, 323, 324, 325, 326, 327, 328, 329, 330, 331, 332, 333, 334, 335, 336, 337, 338, 339, 340, 341, 342, 343, 344, 345, 346, 347, 348, 349, 350, 351, 352, 353, 354, 355, 356, 357, 358, 359, 360, 361, 362, 363, 364, 365, 366, 367, 368, 369, 370, 371, 372, 373, 374, 375, 376, 377, 378, 379, 380, 381, 382, 383, 384, 385, 386, 387, 388, 389, 390, 391, 392, 393, 394, 395, 396, 397, 398, 399, 400, 401, 402, 403, 404, 405, 406, 407, 408, 409, 410, 411, 412, 413, 414, 415, 416, 417, 418, 419, 420, 421, 422, 423, 424, 425, 426, 427, 428, 429, 430, 431, 432, 433, 434, 435, 436, 437, 438, 439, 440, 441, 442, 443, 444, 445, 446, 447, 448, 449, 450, 451, 452, 453, 454, 455, 456, 457, 458, 459, 460, 461, 462, 463, 464, 465, 466, 467, 468, 469, 470, 471, 472, 473, 474, 475, 476, 477, 478, 479, 480, 481, 482, 483, 484, 485, 486, 487, 488, 489, 490, 491, 492, 493, 494, 495, 496, 497, 498, 499, 500, 501, 502, 503, 504, 505, 506, 507, 508, 509, 510, 511, 512, 513, 514, 515, 516, 517, 518, 519, 520, 521, 522, 523, 524, 525, 526, 527, 528, 529, 530, 531, 532, 533, 534, 535, 536, 537, 538, 539, 540, 541, 542, 543, 544, 545, 546, 547, 548, 549, 550, 551, 552, 553, 554, 555, 556, 557, 558, 559, 560, 561, 562, 563, 564, 565, 566, 567, 568, 569, 570, 571, 572, 573, 574, 575, 576, 577, 578, 579, 580, 581, 582, 583, 584, 585, 586, 587, 588, 589, 590, 591, 592, 593, 594, 595, 596, 597, 598, 599, 600, 601, 602, 603, 604, 605, 606, 607, 608, 609, 610, 611, 612, 613, 614, 615, 616, 617, 618, 619, 620, 621, 622, 623, 624, 625, 626, 627, 628, 629, 630, 631, 632, 633, 634, 635, 636, 637, 638, 639, 640, 641, 642, 643, 644, 645, 646, 647, 648, 649, 650, 651, 652, 653, 654, 655, 656, 657, 658, 659, 660, 661, 662, 663, 664, 665, 666, 667, 668, 669, 670, 671, 672, 673, 674, 675, 676, 677, 678, 679, 680, 681, 682, 683, 684, 685, 686, 687, 688, 689, 690, 691, 692, 693, 694, 695, 696, 697, 698, 699, 700, 701, 702, 703, 704, 705, 706, 707, 708, 709, 710, 711, 712, 713, 714, 715, 716, 717, 718, 719, 720, 721, 722, 723, 724, 725, 726, 727, 728, 729, 730, 731, 732, 733, 734, 735, 736, 737, 738, 739, 740, 741, 742, 743, 744, 745, 746, 747, 748, 749, 750, 751, 752, 753, 754, 755, 756, 757, 758, 759, 760, 761, 762, 763, 764, 765, 766, 767, 768, 769, 770, 771, 772, 773, 774, 775, 776, 777, 778, 779, 780, 781, 782, 783, 784, 785, 786, 787, 788, 789, 790, 791, 792, 793, 794, 795, 796, 797, 798, 799, 800, 801, 802, 803, 804, 805, 806, 807, 808, 809, 810, 811, 812, 813, 814, 815, 816, 817, 818, 819, 820, 821, 822, 823, 824, 825, 826, 827, 828, 829, 830, 831, 832, 833, 834, 835, 836, 837, 838, 839, 840, 841, 842, 843, 844, 845, 846, 847, 848, 849, 850, 851, 852, 853, 854, 855, 856, 857, 858, 859, 860, 861, 862, 863, 864, 865, 866, 867, 868, 869, 870, 871, 872, 873, 874, 875, 876, 877, 878, 879, 880, 881, 882, 883, 884, 885, 886, 887, 890, 891, 892, 893, 894, 895, 900, 901, 902, 903, 904, 905, 906, 908, 910, 911, 912, 913, 914, 915, 916, 917, 918, 919, 920, 921, 922, 923, 924, 925, 926, 927, 928, 929, 931, 932, 933, 934, 935, 936, 937, 938, 939, 940, 941, 942, 943, 944, 945, 946, 947, 948, 949, 950, 951, 952, 953, 954, 955, 956, 957, 958, 959, 960, 961, 962, 963, 964, 965, 966, 967, 968, 969, 970, 971, 972, 973, 974, 975, 976, 977, 978, 979, 980, 981, 982, 983, 984, 985, 986, 987, 988, 989, 990, 991, 992, 993, 994, 995, 996, 997, 998, 999, 1000, 1001, 1002, 1003, 1004, 1005, 1006, 1007, 1008, 1009, 1010, 1011, 1012, 1013, 1014, 1015, 1016, 1017, 1018, 1019, 1020, 1021, 1022, 1023, 1024, 1025, 1026, 1027, 1028, 1029, 1030, 1031, 1032, 1033 };
            int[] CountThresholdPlusOne = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 80, 81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 91, 92, 93, 94, 95, 96, 97, 98, 99, 100, 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 112, 113, 114, 115, 116, 117, 118, 119, 120, 121, 122, 123, 124, 125, 126, 127, 128, 129, 130, 131, 132, 133, 134, 135, 136, 137, 138, 139, 140, 141, 142, 143, 144, 145, 146, 147, 148, 149, 150, 151, 152, 153, 154, 155, 156, 157, 158, 159, 160, 161, 162, 163, 164, 165, 166, 167, 168, 169, 170, 171, 172, 173, 174, 175, 176, 177, 178, 179, 180, 181, 182, 183, 184, 185, 186, 187, 188, 189, 190, 191, 192, 193, 194, 195, 196, 197, 198, 199, 200, 201, 202, 203, 204, 205, 206, 207, 208, 209, 210, 211, 212, 213, 214, 215, 216, 217, 218, 219, 220, 221, 222, 223, 224, 225, 226, 227, 228, 229, 230, 231, 232, 233, 234, 235, 236, 237, 238, 239, 240, 241, 242, 243, 244, 245, 246, 247, 248, 249, 250, 251, 252, 253, 254, 255, 256, 257, 258, 259, 260, 261, 262, 263, 264, 265, 266, 267, 268, 269, 270, 271, 272, 273, 274, 275, 276, 277, 278, 279, 280, 281, 282, 283, 284, 285, 286, 287, 288, 289, 290, 291, 292, 293, 294, 295, 296, 297, 298, 299, 300, 301, 302, 303, 304, 305, 306, 307, 308, 309, 310, 311, 312, 313, 314, 315, 316, 317, 318, 319, 320, 321, 322, 323, 324, 325, 326, 327, 328, 329, 330, 331, 332, 333, 334, 335, 336, 337, 338, 339, 340, 341, 342, 343, 344, 345, 346, 347, 348, 349, 350, 351, 352, 353, 354, 355, 356, 357, 358, 359, 360, 361, 362, 363, 364, 365, 366, 367, 368, 369, 370, 371, 372, 373, 374, 375, 376, 377, 378, 379, 380, 381, 382, 383, 384, 385, 386, 387, 388, 389, 390, 391, 392, 393, 394, 395, 396, 397, 398, 399, 400, 401, 402, 403, 404, 405, 406, 407, 408, 409, 410, 411, 412, 413, 414, 415, 416, 417, 418, 419, 420, 421, 422, 423, 424, 425, 426, 427, 428, 429, 430, 431, 432, 433, 434, 435, 436, 437, 438, 439, 440, 441, 442, 443, 444, 445, 446, 447, 448, 449, 450, 451, 452, 453, 454, 455, 456, 457, 458, 459, 460, 461, 462, 463, 464, 465, 466, 467, 468, 469, 470, 471, 472, 473, 474, 475, 476, 477, 478, 479, 480, 481, 482, 483, 484, 485, 486, 487, 488, 489, 490, 491, 492, 493, 494, 495, 496, 497, 498, 499, 500, 501, 502, 503, 504, 505, 506, 507, 508, 509, 510, 511, 512, 513, 514, 515, 516, 517, 518, 519, 520, 521, 522, 523, 524, 525, 526, 527, 528, 529, 530, 531, 532, 533, 534, 535, 536, 537, 538, 539, 540, 541, 542, 543, 544, 545, 546, 547, 548, 549, 550, 551, 552, 553, 554, 555, 556, 557, 558, 559, 560, 561, 562, 563, 564, 565, 566, 567, 568, 569, 570, 571, 572, 573, 574, 575, 576, 577, 578, 579, 580, 581, 582, 583, 584, 585, 586, 587, 588, 589, 590, 591, 592, 593, 594, 595, 596, 597, 598, 599, 600, 601, 602, 603, 604, 605, 606, 607, 608, 609, 610, 611, 612, 613, 614, 615, 616, 617, 618, 619, 620, 621, 622, 623, 624, 625, 626, 627, 628, 629, 630, 631, 632, 633, 634, 635, 636, 637, 638, 639, 640, 641, 642, 643, 644, 645, 646, 647, 648, 649, 650, 651, 652, 653, 654, 655, 656, 657, 658, 659, 660, 661, 662, 663, 664, 665, 666, 667, 668, 669, 670, 671, 672, 673, 674, 675, 676, 677, 678, 679, 680, 681, 682, 683, 684, 685, 686, 687, 688, 689, 690, 691, 692, 693, 694, 695, 696, 697, 698, 699, 700, 701, 702, 703, 704, 705, 706, 707, 708, 709, 710, 711, 712, 713, 714, 715, 716, 717, 718, 719, 720, 721, 722, 723, 724, 725, 726, 727, 728, 729, 730, 731, 732, 733, 734, 735, 736, 737, 738, 739, 740, 741, 742, 743, 744, 745, 746, 747, 748, 749, 750, 751, 752, 753, 754, 755, 756, 757, 758, 759, 760, 761, 762, 763, 764, 765, 766, 767, 768, 769, 770, 771, 772, 773, 774, 775, 776, 777, 778, 779, 780, 781, 782, 783, 784, 785, 786, 787, 788, 789, 790, 791, 792, 793, 794, 795, 796, 797, 798, 799, 800, 801, 802, 803, 804, 805, 806, 807, 808, 809, 810, 811, 812, 813, 814, 815, 816, 817, 818, 819, 820, 821, 822, 823, 824, 825, 826, 827, 828, 829, 830, 831, 832, 833, 834, 835, 836, 837, 838, 839, 840, 841, 842, 843, 844, 845, 846, 847, 848, 849, 850, 851, 852, 853, 854, 855, 856, 857, 858, 859, 860, 861, 862, 863, 864, 865, 866, 867, 868, 869, 870, 871, 872, 873, 874, 875, 876, 877, 878, 879, 880, 881, 882, 883, 884, 885, 886, 887, 890, 891, 892, 893, 894, 895, 900, 901, 902, 903, 904, 905, 906, 908, 910, 911, 912, 913, 914, 915, 916, 917, 918, 919, 920, 921, 922, 923, 924, 925, 926, 927, 928, 929, 931, 932, 933, 934, 935, 936, 937, 938, 939, 940, 941, 942, 943, 944, 945, 946, 947, 948, 949, 950, 951, 952, 953, 954, 955, 956, 957, 958, 959, 960, 961, 962, 963, 964, 965, 966, 967, 968, 969, 970, 971, 972, 973, 974, 975, 976, 977, 978, 979, 980, 981, 982, 983, 984, 985, 986, 987, 988, 989, 990, 991, 992, 993, 994, 995, 996, 997, 998, 999, 1000, 1001, 1002, 1003, 1004, 1005, 1006, 1007, 1008, 1009, 1010, 1011, 1012, 1013, 1014, 1015, 1016, 1017, 1018, 1019, 1020, 1021, 1022, 1023, 1024, 1025, 1026, 1027, 1028, 1029, 1030, 1031, 1032, 1033, 1034 };

            string actual, expected;

            actual = Character.ToString(StackThresholdMinusOne);
            expected = "\u0001\u0002\u0003\u0004\u0005\u0006\a\b\t\n\v\f\r\u000e\u000f\u0010\u0011\u0012\u0013\u0014\u0015\u0016\u0017\u0018\u0019\u001a\u001b\u001c\u001d\u001e\u001f !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~\u007f\u0080\u0081\u0082\u0083\u0084\u0085\u0086\u0087\u0088\u0089\u008a\u008b\u008c\u008d\u008e\u008f\u0090\u0091\u0092\u0093\u0094\u0095\u0096\u0097\u0098\u0099\u009a\u009b\u009c\u009d\u009e\u009f ¡¢£¤¥¦§¨©ª«¬­®¯°±²³´µ¶·¸¹º»¼½¾¿ÀÁÂÃÄÅÆÇÈÉÊËÌÍÎÏÐÑÒÓÔÕÖ×ØÙÚÛÜÝÞßàáâãäåæçèéêëìíîïðñòóôõö÷øùúûüýþÿ";
            assertEquals(expected.Length, StackThresholdMinusOne.Length);
            assertEquals(expected, actual);

            actual = Character.ToString(StackThreshold);
            expected = "\u0001\u0002\u0003\u0004\u0005\u0006\a\b\t\n\v\f\r\u000e\u000f\u0010\u0011\u0012\u0013\u0014\u0015\u0016\u0017\u0018\u0019\u001a\u001b\u001c\u001d\u001e\u001f !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~\u007f\u0080\u0081\u0082\u0083\u0084\u0085\u0086\u0087\u0088\u0089\u008a\u008b\u008c\u008d\u008e\u008f\u0090\u0091\u0092\u0093\u0094\u0095\u0096\u0097\u0098\u0099\u009a\u009b\u009c\u009d\u009e\u009f ¡¢£¤¥¦§¨©ª«¬­®¯°±²³´µ¶·¸¹º»¼½¾¿ÀÁÂÃÄÅÆÇÈÉÊËÌÍÎÏÐÑÒÓÔÕÖ×ØÙÚÛÜÝÞßàáâãäåæçèéêëìíîïðñòóôõö÷øùúûüýþÿĀ";
            assertEquals(expected.Length, StackThreshold.Length);
            assertEquals(expected, actual);

            actual = Character.ToString(StackThresholdPlusOne);
            expected = "\u0001\u0002\u0003\u0004\u0005\u0006\a\b\t\n\v\f\r\u000e\u000f\u0010\u0011\u0012\u0013\u0014\u0015\u0016\u0017\u0018\u0019\u001a\u001b\u001c\u001d\u001e\u001f !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~\u007f\u0080\u0081\u0082\u0083\u0084\u0085\u0086\u0087\u0088\u0089\u008a\u008b\u008c\u008d\u008e\u008f\u0090\u0091\u0092\u0093\u0094\u0095\u0096\u0097\u0098\u0099\u009a\u009b\u009c\u009d\u009e\u009f ¡¢£¤¥¦§¨©ª«¬­®¯°±²³´µ¶·¸¹º»¼½¾¿ÀÁÂÃÄÅÆÇÈÉÊËÌÍÎÏÐÑÒÓÔÕÖ×ØÙÚÛÜÝÞßàáâãäåæçèéêëìíîïðñòóôõö÷øùúûüýþÿĀā";
            assertEquals(expected.Length, StackThresholdPlusOne.Length);
            assertEquals(expected, actual);


            actual = Character.ToString(CountThresholdMinusOne);
            expected = "\u0001\u0002\u0003\u0004\u0005\u0006\a\b\t\n\v\f\r\u000e\u000f\u0010\u0011\u0012\u0013\u0014\u0015\u0016\u0017\u0018\u0019\u001a\u001b\u001c\u001d\u001e\u001f !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~\u007f\u0080\u0081\u0082\u0083\u0084\u0085\u0086\u0087\u0088\u0089\u008a\u008b\u008c\u008d\u008e\u008f\u0090\u0091\u0092\u0093\u0094\u0095\u0096\u0097\u0098\u0099\u009a\u009b\u009c\u009d\u009e\u009f ¡¢£¤¥¦§¨©ª«¬­®¯°±²³´µ¶·¸¹º»¼½¾¿ÀÁÂÃÄÅÆÇÈÉÊËÌÍÎÏÐÑÒÓÔÕÖ×ØÙÚÛÜÝÞßàáâãäåæçèéêëìíîïðñòóôõö÷øùúûüýþÿĀāĂăĄąĆćĈĉĊċČčĎďĐđĒēĔĕĖėĘęĚěĜĝĞğĠġĢģĤĥĦħĨĩĪīĬĭĮįİıĲĳĴĵĶķĸĹĺĻļĽľĿŀŁłŃńŅņŇňŉŊŋŌōŎŏŐőŒœŔŕŖŗŘřŚśŜŝŞşŠšŢţŤťŦŧŨũŪūŬŭŮůŰűŲųŴŵŶŷŸŹźŻżŽžſƀƁƂƃƄƅƆƇƈƉƊƋƌƍƎƏƐƑƒƓƔƕƖƗƘƙƚƛƜƝƞƟƠơƢƣƤƥƦƧƨƩƪƫƬƭƮƯưƱƲƳƴƵƶƷƸƹƺƻƼƽƾƿǀǁǂǃǄǅǆǇǈǉǊǋǌǍǎǏǐǑǒǓǔǕǖǗǘǙǚǛǜǝǞǟǠǡǢǣǤǥǦǧǨǩǪǫǬǭǮǯǰǱǲǳǴǵǶǷǸǹǺǻǼǽǾǿȀȁȂȃȄȅȆȇȈȉȊȋȌȍȎȏȐȑȒȓȔȕȖȗȘșȚțȜȝȞȟȠȡȢȣȤȥȦȧȨȩȪȫȬȭȮȯȰȱȲȳȴȵȶȷȸȹȺȻȼȽȾȿɀɁɂɃɄɅɆɇɈɉɊɋɌɍɎɏɐɑɒɓɔɕɖɗɘəɚɛɜɝɞɟɠɡɢɣɤɥɦɧɨɩɪɫɬɭɮɯɰɱɲɳɴɵɶɷɸɹɺɻɼɽɾɿʀʁʂʃʄʅʆʇʈʉʊʋʌʍʎʏʐʑʒʓʔʕʖʗʘʙʚʛʜʝʞʟʠʡʢʣʤʥʦʧʨʩʪʫʬʭʮʯʰʱʲʳʴʵʶʷʸʹʺʻʼʽʾʿˀˁ˂˃˄˅ˆˇˈˉˊˋˌˍˎˏːˑ˒˓˔˕˖˗˘˙˚˛˜˝˞˟ˠˡˢˣˤ˥˦˧˨˩˪˫ˬ˭ˮ˯˰˱˲˳˴˵˶˷˸˹˺˻˼˽˾˿̴̵̶̷̸̡̢̧̨̛̖̗̘̙̜̝̞̟̠̣̤̥̦̩̪̫̬̭̮̯̰̱̲̳̹̺̻̼͇͈͉͍͎̀́̂̃̄̅̆̇̈̉̊̋̌̍̎̏̐̑̒̓̔̽̾̿̀́͂̓̈́͆͊͋͌̕̚ͅ͏͓͔͕͖͙͚͐͑͒͗͛ͣͤͥͦͧͨͩͪͫͬͭͮͯ͘͜͟͢͝͞͠͡ͰͱͲͳʹ͵Ͷͷͺͻͼͽ;Ϳ΄΅Ά·ΈΉΊΌΎΏΐΑΒΓΔΕΖΗΘΙΚΛΜΝΞΟΠΡΣΤΥΦΧΨΩΪΫάέήίΰαβγδεζηθικλμνξοπρςστυφχψωϊϋόύώϏϐϑϒϓϔϕϖϗϘϙϚϛϜϝϞϟϠϡϢϣϤϥϦϧϨϩϪϫϬϭϮϯϰϱϲϳϴϵ϶ϷϸϹϺϻϼϽϾϿЀЁЂЃЄЅІЇЈ";
            assertEquals(expected.Length, CountThresholdMinusOne.Length);
            assertEquals(expected, actual);

            actual = Character.ToString(CountThreshold);
            expected = "\u0001\u0002\u0003\u0004\u0005\u0006\a\b\t\n\v\f\r\u000e\u000f\u0010\u0011\u0012\u0013\u0014\u0015\u0016\u0017\u0018\u0019\u001a\u001b\u001c\u001d\u001e\u001f !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~\u007f\u0080\u0081\u0082\u0083\u0084\u0085\u0086\u0087\u0088\u0089\u008a\u008b\u008c\u008d\u008e\u008f\u0090\u0091\u0092\u0093\u0094\u0095\u0096\u0097\u0098\u0099\u009a\u009b\u009c\u009d\u009e\u009f ¡¢£¤¥¦§¨©ª«¬­®¯°±²³´µ¶·¸¹º»¼½¾¿ÀÁÂÃÄÅÆÇÈÉÊËÌÍÎÏÐÑÒÓÔÕÖ×ØÙÚÛÜÝÞßàáâãäåæçèéêëìíîïðñòóôõö÷øùúûüýþÿĀāĂăĄąĆćĈĉĊċČčĎďĐđĒēĔĕĖėĘęĚěĜĝĞğĠġĢģĤĥĦħĨĩĪīĬĭĮįİıĲĳĴĵĶķĸĹĺĻļĽľĿŀŁłŃńŅņŇňŉŊŋŌōŎŏŐőŒœŔŕŖŗŘřŚśŜŝŞşŠšŢţŤťŦŧŨũŪūŬŭŮůŰűŲųŴŵŶŷŸŹźŻżŽžſƀƁƂƃƄƅƆƇƈƉƊƋƌƍƎƏƐƑƒƓƔƕƖƗƘƙƚƛƜƝƞƟƠơƢƣƤƥƦƧƨƩƪƫƬƭƮƯưƱƲƳƴƵƶƷƸƹƺƻƼƽƾƿǀǁǂǃǄǅǆǇǈǉǊǋǌǍǎǏǐǑǒǓǔǕǖǗǘǙǚǛǜǝǞǟǠǡǢǣǤǥǦǧǨǩǪǫǬǭǮǯǰǱǲǳǴǵǶǷǸǹǺǻǼǽǾǿȀȁȂȃȄȅȆȇȈȉȊȋȌȍȎȏȐȑȒȓȔȕȖȗȘșȚțȜȝȞȟȠȡȢȣȤȥȦȧȨȩȪȫȬȭȮȯȰȱȲȳȴȵȶȷȸȹȺȻȼȽȾȿɀɁɂɃɄɅɆɇɈɉɊɋɌɍɎɏɐɑɒɓɔɕɖɗɘəɚɛɜɝɞɟɠɡɢɣɤɥɦɧɨɩɪɫɬɭɮɯɰɱɲɳɴɵɶɷɸɹɺɻɼɽɾɿʀʁʂʃʄʅʆʇʈʉʊʋʌʍʎʏʐʑʒʓʔʕʖʗʘʙʚʛʜʝʞʟʠʡʢʣʤʥʦʧʨʩʪʫʬʭʮʯʰʱʲʳʴʵʶʷʸʹʺʻʼʽʾʿˀˁ˂˃˄˅ˆˇˈˉˊˋˌˍˎˏːˑ˒˓˔˕˖˗˘˙˚˛˜˝˞˟ˠˡˢˣˤ˥˦˧˨˩˪˫ˬ˭ˮ˯˰˱˲˳˴˵˶˷˸˹˺˻˼˽˾˿̴̵̶̷̸̡̢̧̨̛̖̗̘̙̜̝̞̟̠̣̤̥̦̩̪̫̬̭̮̯̰̱̲̳̹̺̻̼͇͈͉͍͎̀́̂̃̄̅̆̇̈̉̊̋̌̍̎̏̐̑̒̓̔̽̾̿̀́͂̓̈́͆͊͋͌̕̚ͅ͏͓͔͕͖͙͚͐͑͒͗͛ͣͤͥͦͧͨͩͪͫͬͭͮͯ͘͜͟͢͝͞͠͡ͰͱͲͳʹ͵Ͷͷͺͻͼͽ;Ϳ΄΅Ά·ΈΉΊΌΎΏΐΑΒΓΔΕΖΗΘΙΚΛΜΝΞΟΠΡΣΤΥΦΧΨΩΪΫάέήίΰαβγδεζηθικλμνξοπρςστυφχψωϊϋόύώϏϐϑϒϓϔϕϖϗϘϙϚϛϜϝϞϟϠϡϢϣϤϥϦϧϨϩϪϫϬϭϮϯϰϱϲϳϴϵ϶ϷϸϹϺϻϼϽϾϿЀЁЂЃЄЅІЇЈЉ";
            assertEquals(expected.Length, CountThreshold.Length);
            assertEquals(expected, actual);

            actual = Character.ToString(CountThresholdPlusOne);
            expected = "\u0001\u0002\u0003\u0004\u0005\u0006\a\b\t\n\v\f\r\u000e\u000f\u0010\u0011\u0012\u0013\u0014\u0015\u0016\u0017\u0018\u0019\u001a\u001b\u001c\u001d\u001e\u001f !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~\u007f\u0080\u0081\u0082\u0083\u0084\u0085\u0086\u0087\u0088\u0089\u008a\u008b\u008c\u008d\u008e\u008f\u0090\u0091\u0092\u0093\u0094\u0095\u0096\u0097\u0098\u0099\u009a\u009b\u009c\u009d\u009e\u009f ¡¢£¤¥¦§¨©ª«¬­®¯°±²³´µ¶·¸¹º»¼½¾¿ÀÁÂÃÄÅÆÇÈÉÊËÌÍÎÏÐÑÒÓÔÕÖ×ØÙÚÛÜÝÞßàáâãäåæçèéêëìíîïðñòóôõö÷øùúûüýþÿĀāĂăĄąĆćĈĉĊċČčĎďĐđĒēĔĕĖėĘęĚěĜĝĞğĠġĢģĤĥĦħĨĩĪīĬĭĮįİıĲĳĴĵĶķĸĹĺĻļĽľĿŀŁłŃńŅņŇňŉŊŋŌōŎŏŐőŒœŔŕŖŗŘřŚśŜŝŞşŠšŢţŤťŦŧŨũŪūŬŭŮůŰűŲųŴŵŶŷŸŹźŻżŽžſƀƁƂƃƄƅƆƇƈƉƊƋƌƍƎƏƐƑƒƓƔƕƖƗƘƙƚƛƜƝƞƟƠơƢƣƤƥƦƧƨƩƪƫƬƭƮƯưƱƲƳƴƵƶƷƸƹƺƻƼƽƾƿǀǁǂǃǄǅǆǇǈǉǊǋǌǍǎǏǐǑǒǓǔǕǖǗǘǙǚǛǜǝǞǟǠǡǢǣǤǥǦǧǨǩǪǫǬǭǮǯǰǱǲǳǴǵǶǷǸǹǺǻǼǽǾǿȀȁȂȃȄȅȆȇȈȉȊȋȌȍȎȏȐȑȒȓȔȕȖȗȘșȚțȜȝȞȟȠȡȢȣȤȥȦȧȨȩȪȫȬȭȮȯȰȱȲȳȴȵȶȷȸȹȺȻȼȽȾȿɀɁɂɃɄɅɆɇɈɉɊɋɌɍɎɏɐɑɒɓɔɕɖɗɘəɚɛɜɝɞɟɠɡɢɣɤɥɦɧɨɩɪɫɬɭɮɯɰɱɲɳɴɵɶɷɸɹɺɻɼɽɾɿʀʁʂʃʄʅʆʇʈʉʊʋʌʍʎʏʐʑʒʓʔʕʖʗʘʙʚʛʜʝʞʟʠʡʢʣʤʥʦʧʨʩʪʫʬʭʮʯʰʱʲʳʴʵʶʷʸʹʺʻʼʽʾʿˀˁ˂˃˄˅ˆˇˈˉˊˋˌˍˎˏːˑ˒˓˔˕˖˗˘˙˚˛˜˝˞˟ˠˡˢˣˤ˥˦˧˨˩˪˫ˬ˭ˮ˯˰˱˲˳˴˵˶˷˸˹˺˻˼˽˾˿̴̵̶̷̸̡̢̧̨̛̖̗̘̙̜̝̞̟̠̣̤̥̦̩̪̫̬̭̮̯̰̱̲̳̹̺̻̼͇͈͉͍͎̀́̂̃̄̅̆̇̈̉̊̋̌̍̎̏̐̑̒̓̔̽̾̿̀́͂̓̈́͆͊͋͌̕̚ͅ͏͓͔͕͖͙͚͐͑͒͗͛ͣͤͥͦͧͨͩͪͫͬͭͮͯ͘͜͟͢͝͞͠͡ͰͱͲͳʹ͵Ͷͷͺͻͼͽ;Ϳ΄΅Ά·ΈΉΊΌΎΏΐΑΒΓΔΕΖΗΘΙΚΛΜΝΞΟΠΡΣΤΥΦΧΨΩΪΫάέήίΰαβγδεζηθικλμνξοπρςστυφχψωϊϋόύώϏϐϑϒϓϔϕϖϗϘϙϚϛϜϝϞϟϠϡϢϣϤϥϦϧϨϩϪϫϬϭϮϯϰϱϲϳϴϵ϶ϷϸϹϺϻϼϽϾϿЀЁЂЃЄЅІЇЈЉЊ";
            assertEquals(expected.Length, CountThresholdPlusOne.Length);
            assertEquals(expected, actual);
        }

        /// <summary>
        /// Ensures that arrays with lengths below and above thresholds will successfully convert
        /// </summary>
        [Test]
        public void TestToString_ReadOnlySpanInt32_Thresholds_Surrogates()
        {
            // Note: These code points all are defined in Unicode 8.0
            ReadOnlySpan<int> StackThresholdMinusOne = new int[] { 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562 };
            ReadOnlySpan<int> StackThreshold = new int[] { 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563 };
            ReadOnlySpan<int> StackThresholdPlusOne = new int[] { 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564 };

            ReadOnlySpan<int> CountThresholdMinusOne = new int[] { 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574 };
            ReadOnlySpan<int> CountThreshold = new int[] { 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575 };
            ReadOnlySpan<int> CountThresholdPlusOne = new int[] { 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66580 };

            string actual, expected;

            actual = Character.ToString(StackThresholdMinusOne);
            expected = "𐐀𐐁𐐂𐐃𐐄𐐅𐐆𐐇𐐈𐐉𐐊𐐋𐐌𐐍𐐎𐐏𐐐𐐑𐐒𐐓𐐔𐐀𐐁𐐂𐐃𐐄𐐅𐐆𐐇𐐈𐐉𐐊𐐋𐐌𐐍𐐎𐐏𐐐𐐑𐐒𐐓𐐔𐐀𐐁𐐂𐐃𐐄𐐅𐐆𐐇𐐈𐐉𐐊𐐋𐐌𐐍𐐎𐐏𐐐𐐑𐐒𐐓𐐔𐐀𐐁𐐂𐐃𐐄𐐅𐐆𐐇𐐈𐐉𐐊𐐋𐐌𐐍𐐎𐐏𐐐𐐑𐐒𐐓𐐔𐐀𐐁𐐂𐐃𐐄𐐅𐐆𐐇𐐈𐐉𐐊𐐋𐐌𐐍𐐎𐐏𐐐𐐑𐐒𐐓𐐔𐐀𐐁𐐂𐐃𐐄𐐅𐐆𐐇𐐈𐐉𐐊𐐋𐐌𐐍𐐎𐐏𐐐𐐑𐐒𐐓𐐔𐐀𐐁𐐂𐐃𐐄𐐅𐐆𐐇𐐈𐐉𐐊𐐋𐐌𐐍𐐎𐐏𐐐𐐑𐐒𐐓𐐔𐐀𐐁𐐂𐐃𐐄𐐅𐐆𐐇𐐈𐐉𐐊𐐋𐐌𐐍𐐎𐐏𐐐𐐑𐐒𐐓𐐔𐐀𐐁𐐂𐐃𐐄𐐅𐐆𐐇𐐈𐐉𐐊𐐋𐐌𐐍𐐎𐐏𐐐𐐑𐐒𐐓𐐔𐐀𐐁𐐂𐐃𐐄𐐅𐐆𐐇𐐈𐐉𐐊𐐋𐐌𐐍𐐎𐐏𐐐𐐑𐐒𐐓𐐔𐐀𐐁𐐂𐐃𐐄𐐅𐐆𐐇𐐈𐐉𐐊𐐋𐐌𐐍𐐎𐐏𐐐𐐑𐐒𐐓𐐔𐐀𐐁𐐂𐐃𐐄𐐅𐐆𐐇𐐈𐐉𐐊𐐋𐐌𐐍𐐎𐐏𐐐𐐑𐐒𐐓𐐔𐐀𐐁𐐂";
            assertEquals(expected.Length, StackThresholdMinusOne.Length * 2);
            assertEquals(expected, actual);

            actual = Character.ToString(StackThreshold);
            expected = "𐐀𐐁𐐂𐐃𐐄𐐅𐐆𐐇𐐈𐐉𐐊𐐋𐐌𐐍𐐎𐐏𐐐𐐑𐐒𐐓𐐔𐐀𐐁𐐂𐐃𐐄𐐅𐐆𐐇𐐈𐐉𐐊𐐋𐐌𐐍𐐎𐐏𐐐𐐑𐐒𐐓𐐔𐐀𐐁𐐂𐐃𐐄𐐅𐐆𐐇𐐈𐐉𐐊𐐋𐐌𐐍𐐎𐐏𐐐𐐑𐐒𐐓𐐔𐐀𐐁𐐂𐐃𐐄𐐅𐐆𐐇𐐈𐐉𐐊𐐋𐐌𐐍𐐎𐐏𐐐𐐑𐐒𐐓𐐔𐐀𐐁𐐂𐐃𐐄𐐅𐐆𐐇𐐈𐐉𐐊𐐋𐐌𐐍𐐎𐐏𐐐𐐑𐐒𐐓𐐔𐐀𐐁𐐂𐐃𐐄𐐅𐐆𐐇𐐈𐐉𐐊𐐋𐐌𐐍𐐎𐐏𐐐𐐑𐐒𐐓𐐔𐐀𐐁𐐂𐐃𐐄𐐅𐐆𐐇𐐈𐐉𐐊𐐋𐐌𐐍𐐎𐐏𐐐𐐑𐐒𐐓𐐔𐐀𐐁𐐂𐐃𐐄𐐅𐐆𐐇𐐈𐐉𐐊𐐋𐐌𐐍𐐎𐐏𐐐𐐑𐐒𐐓𐐔𐐀𐐁𐐂𐐃𐐄𐐅𐐆𐐇𐐈𐐉𐐊𐐋𐐌𐐍𐐎𐐏𐐐𐐑𐐒𐐓𐐔𐐀𐐁𐐂𐐃𐐄𐐅𐐆𐐇𐐈𐐉𐐊𐐋𐐌𐐍𐐎𐐏𐐐𐐑𐐒𐐓𐐔𐐀𐐁𐐂𐐃𐐄𐐅𐐆𐐇𐐈𐐉𐐊𐐋𐐌𐐍𐐎𐐏𐐐𐐑𐐒𐐓𐐔𐐀𐐁𐐂𐐃𐐄𐐅𐐆𐐇𐐈𐐉𐐊𐐋𐐌𐐍𐐎𐐏𐐐𐐑𐐒𐐓𐐔𐐀𐐁𐐂𐐃";
            assertEquals(expected.Length, StackThreshold.Length * 2);
            assertEquals(expected, actual);

            actual = Character.ToString(StackThresholdPlusOne);
            expected = "𐐀𐐁𐐂𐐃𐐄𐐅𐐆𐐇𐐈𐐉𐐊𐐋𐐌𐐍𐐎𐐏𐐐𐐑𐐒𐐓𐐔𐐀𐐁𐐂𐐃𐐄𐐅𐐆𐐇𐐈𐐉𐐊𐐋𐐌𐐍𐐎𐐏𐐐𐐑𐐒𐐓𐐔𐐀𐐁𐐂𐐃𐐄𐐅𐐆𐐇𐐈𐐉𐐊𐐋𐐌𐐍𐐎𐐏𐐐𐐑𐐒𐐓𐐔𐐀𐐁𐐂𐐃𐐄𐐅𐐆𐐇𐐈𐐉𐐊𐐋𐐌𐐍𐐎𐐏𐐐𐐑𐐒𐐓𐐔𐐀𐐁𐐂𐐃𐐄𐐅𐐆𐐇𐐈𐐉𐐊𐐋𐐌𐐍𐐎𐐏𐐐𐐑𐐒𐐓𐐔𐐀𐐁𐐂𐐃𐐄𐐅𐐆𐐇𐐈𐐉𐐊𐐋𐐌𐐍𐐎𐐏𐐐𐐑𐐒𐐓𐐔𐐀𐐁𐐂𐐃𐐄𐐅𐐆𐐇𐐈𐐉𐐊𐐋𐐌𐐍𐐎𐐏𐐐𐐑𐐒𐐓𐐔𐐀𐐁𐐂𐐃𐐄𐐅𐐆𐐇𐐈𐐉𐐊𐐋𐐌𐐍𐐎𐐏𐐐𐐑𐐒𐐓𐐔𐐀𐐁𐐂𐐃𐐄𐐅𐐆𐐇𐐈𐐉𐐊𐐋𐐌𐐍𐐎𐐏𐐐𐐑𐐒𐐓𐐔𐐀𐐁𐐂𐐃𐐄𐐅𐐆𐐇𐐈𐐉𐐊𐐋𐐌𐐍𐐎𐐏𐐐𐐑𐐒𐐓𐐔𐐀𐐁𐐂𐐃𐐄𐐅𐐆𐐇𐐈𐐉𐐊𐐋𐐌𐐍𐐎𐐏𐐐𐐑𐐒𐐓𐐔𐐀𐐁𐐂𐐃𐐄𐐅𐐆𐐇𐐈𐐉𐐊𐐋𐐌𐐍𐐎𐐏𐐐𐐑𐐒𐐓𐐔𐐀𐐁𐐂𐐃𐐄";
            assertEquals(expected.Length, StackThresholdPlusOne.Length * 2);
            assertEquals(expected, actual);

            actual = Character.ToString(CountThresholdMinusOne);
            expected = "\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E";
            assertEquals(expected.Length, CountThresholdMinusOne.Length * 2);
            assertEquals(expected, actual);

            actual = Character.ToString(CountThreshold);
            expected = "\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F";
            assertEquals(expected.Length, CountThreshold.Length * 2);
            assertEquals(expected, actual);

            actual = Character.ToString(CountThresholdPlusOne);
            expected = "\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC14";
            assertEquals(expected.Length, CountThresholdPlusOne.Length * 2);
            assertEquals(expected, actual);
        }

        /// <summary>
        /// Ensures that arrays with lengths below and above thresholds will successfully convert
        /// </summary>
        [Test]
        public void TestToString_Int32Array_Thresholds_Surrogates()
        {
            // Note: These code points all are defined in Unicode 8.0
            int[] StackThresholdMinusOne = new int[] { 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562 };
            int[] StackThreshold = new int[] { 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563 };
            int[] StackThresholdPlusOne = new int[] { 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564 };

            int[] CountThresholdMinusOne = new int[] { 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574 };
            int[] CountThreshold = new int[] { 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575 };
            int[] CountThresholdPlusOne = new int[] { 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66576, 66577, 66578, 66579, 66580, 66560, 66561, 66562, 66563, 66564, 66565, 66566, 66567, 66568, 66569, 66570, 66571, 66572, 66573, 66574, 66575, 66580 };

            string actual, expected;

            actual = Character.ToString(StackThresholdMinusOne);
            expected = "𐐀𐐁𐐂𐐃𐐄𐐅𐐆𐐇𐐈𐐉𐐊𐐋𐐌𐐍𐐎𐐏𐐐𐐑𐐒𐐓𐐔𐐀𐐁𐐂𐐃𐐄𐐅𐐆𐐇𐐈𐐉𐐊𐐋𐐌𐐍𐐎𐐏𐐐𐐑𐐒𐐓𐐔𐐀𐐁𐐂𐐃𐐄𐐅𐐆𐐇𐐈𐐉𐐊𐐋𐐌𐐍𐐎𐐏𐐐𐐑𐐒𐐓𐐔𐐀𐐁𐐂𐐃𐐄𐐅𐐆𐐇𐐈𐐉𐐊𐐋𐐌𐐍𐐎𐐏𐐐𐐑𐐒𐐓𐐔𐐀𐐁𐐂𐐃𐐄𐐅𐐆𐐇𐐈𐐉𐐊𐐋𐐌𐐍𐐎𐐏𐐐𐐑𐐒𐐓𐐔𐐀𐐁𐐂𐐃𐐄𐐅𐐆𐐇𐐈𐐉𐐊𐐋𐐌𐐍𐐎𐐏𐐐𐐑𐐒𐐓𐐔𐐀𐐁𐐂𐐃𐐄𐐅𐐆𐐇𐐈𐐉𐐊𐐋𐐌𐐍𐐎𐐏𐐐𐐑𐐒𐐓𐐔𐐀𐐁𐐂𐐃𐐄𐐅𐐆𐐇𐐈𐐉𐐊𐐋𐐌𐐍𐐎𐐏𐐐𐐑𐐒𐐓𐐔𐐀𐐁𐐂𐐃𐐄𐐅𐐆𐐇𐐈𐐉𐐊𐐋𐐌𐐍𐐎𐐏𐐐𐐑𐐒𐐓𐐔𐐀𐐁𐐂𐐃𐐄𐐅𐐆𐐇𐐈𐐉𐐊𐐋𐐌𐐍𐐎𐐏𐐐𐐑𐐒𐐓𐐔𐐀𐐁𐐂𐐃𐐄𐐅𐐆𐐇𐐈𐐉𐐊𐐋𐐌𐐍𐐎𐐏𐐐𐐑𐐒𐐓𐐔𐐀𐐁𐐂𐐃𐐄𐐅𐐆𐐇𐐈𐐉𐐊𐐋𐐌𐐍𐐎𐐏𐐐𐐑𐐒𐐓𐐔𐐀𐐁𐐂";
            assertEquals(expected.Length, StackThresholdMinusOne.Length * 2);
            assertEquals(expected, actual);

            actual = Character.ToString(StackThreshold);
            expected = "𐐀𐐁𐐂𐐃𐐄𐐅𐐆𐐇𐐈𐐉𐐊𐐋𐐌𐐍𐐎𐐏𐐐𐐑𐐒𐐓𐐔𐐀𐐁𐐂𐐃𐐄𐐅𐐆𐐇𐐈𐐉𐐊𐐋𐐌𐐍𐐎𐐏𐐐𐐑𐐒𐐓𐐔𐐀𐐁𐐂𐐃𐐄𐐅𐐆𐐇𐐈𐐉𐐊𐐋𐐌𐐍𐐎𐐏𐐐𐐑𐐒𐐓𐐔𐐀𐐁𐐂𐐃𐐄𐐅𐐆𐐇𐐈𐐉𐐊𐐋𐐌𐐍𐐎𐐏𐐐𐐑𐐒𐐓𐐔𐐀𐐁𐐂𐐃𐐄𐐅𐐆𐐇𐐈𐐉𐐊𐐋𐐌𐐍𐐎𐐏𐐐𐐑𐐒𐐓𐐔𐐀𐐁𐐂𐐃𐐄𐐅𐐆𐐇𐐈𐐉𐐊𐐋𐐌𐐍𐐎𐐏𐐐𐐑𐐒𐐓𐐔𐐀𐐁𐐂𐐃𐐄𐐅𐐆𐐇𐐈𐐉𐐊𐐋𐐌𐐍𐐎𐐏𐐐𐐑𐐒𐐓𐐔𐐀𐐁𐐂𐐃𐐄𐐅𐐆𐐇𐐈𐐉𐐊𐐋𐐌𐐍𐐎𐐏𐐐𐐑𐐒𐐓𐐔𐐀𐐁𐐂𐐃𐐄𐐅𐐆𐐇𐐈𐐉𐐊𐐋𐐌𐐍𐐎𐐏𐐐𐐑𐐒𐐓𐐔𐐀𐐁𐐂𐐃𐐄𐐅𐐆𐐇𐐈𐐉𐐊𐐋𐐌𐐍𐐎𐐏𐐐𐐑𐐒𐐓𐐔𐐀𐐁𐐂𐐃𐐄𐐅𐐆𐐇𐐈𐐉𐐊𐐋𐐌𐐍𐐎𐐏𐐐𐐑𐐒𐐓𐐔𐐀𐐁𐐂𐐃𐐄𐐅𐐆𐐇𐐈𐐉𐐊𐐋𐐌𐐍𐐎𐐏𐐐𐐑𐐒𐐓𐐔𐐀𐐁𐐂𐐃";
            assertEquals(expected.Length, StackThreshold.Length * 2);
            assertEquals(expected, actual);

            actual = Character.ToString(StackThresholdPlusOne);
            expected = "𐐀𐐁𐐂𐐃𐐄𐐅𐐆𐐇𐐈𐐉𐐊𐐋𐐌𐐍𐐎𐐏𐐐𐐑𐐒𐐓𐐔𐐀𐐁𐐂𐐃𐐄𐐅𐐆𐐇𐐈𐐉𐐊𐐋𐐌𐐍𐐎𐐏𐐐𐐑𐐒𐐓𐐔𐐀𐐁𐐂𐐃𐐄𐐅𐐆𐐇𐐈𐐉𐐊𐐋𐐌𐐍𐐎𐐏𐐐𐐑𐐒𐐓𐐔𐐀𐐁𐐂𐐃𐐄𐐅𐐆𐐇𐐈𐐉𐐊𐐋𐐌𐐍𐐎𐐏𐐐𐐑𐐒𐐓𐐔𐐀𐐁𐐂𐐃𐐄𐐅𐐆𐐇𐐈𐐉𐐊𐐋𐐌𐐍𐐎𐐏𐐐𐐑𐐒𐐓𐐔𐐀𐐁𐐂𐐃𐐄𐐅𐐆𐐇𐐈𐐉𐐊𐐋𐐌𐐍𐐎𐐏𐐐𐐑𐐒𐐓𐐔𐐀𐐁𐐂𐐃𐐄𐐅𐐆𐐇𐐈𐐉𐐊𐐋𐐌𐐍𐐎𐐏𐐐𐐑𐐒𐐓𐐔𐐀𐐁𐐂𐐃𐐄𐐅𐐆𐐇𐐈𐐉𐐊𐐋𐐌𐐍𐐎𐐏𐐐𐐑𐐒𐐓𐐔𐐀𐐁𐐂𐐃𐐄𐐅𐐆𐐇𐐈𐐉𐐊𐐋𐐌𐐍𐐎𐐏𐐐𐐑𐐒𐐓𐐔𐐀𐐁𐐂𐐃𐐄𐐅𐐆𐐇𐐈𐐉𐐊𐐋𐐌𐐍𐐎𐐏𐐐𐐑𐐒𐐓𐐔𐐀𐐁𐐂𐐃𐐄𐐅𐐆𐐇𐐈𐐉𐐊𐐋𐐌𐐍𐐎𐐏𐐐𐐑𐐒𐐓𐐔𐐀𐐁𐐂𐐃𐐄𐐅𐐆𐐇𐐈𐐉𐐊𐐋𐐌𐐍𐐎𐐏𐐐𐐑𐐒𐐓𐐔𐐀𐐁𐐂𐐃𐐄";
            assertEquals(expected.Length, StackThresholdPlusOne.Length * 2);
            assertEquals(expected, actual);

            actual = Character.ToString(CountThresholdMinusOne);
            expected = "\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E";
            assertEquals(expected.Length, CountThresholdMinusOne.Length * 2);
            assertEquals(expected, actual);

            actual = Character.ToString(CountThreshold);
            expected = "\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F";
            assertEquals(expected.Length, CountThreshold.Length * 2);
            assertEquals(expected, actual);

            actual = Character.ToString(CountThresholdPlusOne);
            expected = "\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC10\uD801\uDC11\uD801\uDC12\uD801\uDC13\uD801\uDC14\uD801\uDC00\uD801\uDC01\uD801\uDC02\uD801\uDC03\uD801\uDC04\uD801\uDC05\uD801\uDC06\uD801\uDC07\uD801\uDC08\uD801\uDC09\uD801\uDC0A\uD801\uDC0B\uD801\uDC0C\uD801\uDC0D\uD801\uDC0E\uD801\uDC0F\uD801\uDC14";
            assertEquals(expected.Length, CountThresholdPlusOne.Length * 2);
            assertEquals(expected, actual);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Useful debug utility")]
        private string ConvertToUtf16Escaped(string input) // Utility useful for escaping long strings (because VS cannot attach a debugger when they are too long)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];
                sb.Append('\\');
                sb.Append('u');
                sb.Append(c.ToHexString().ToUpperInvariant());
            }
            return sb.ToString();
        }

        [Test]
        public virtual void TestNewString() // From Lucene.NET TestUnicodeUtil
        {
            int[] codePoints = new int[] { Character.ToCodePoint(Character.MinHighSurrogate, Character.MaxLowSurrogate), Character.ToCodePoint(Character.MaxHighSurrogate, Character.MinLowSurrogate), Character.MaxHighSurrogate, 'A', -1 };

            string cpString = "" + Character.MinHighSurrogate + Character.MaxLowSurrogate + Character.MaxHighSurrogate + Character.MinLowSurrogate + Character.MaxHighSurrogate + 'A';

            int[][] tests = new int[][] { new int[] { 0, 1, 0, 2 }, new int[] { 0, 2, 0, 4 }, new int[] { 1, 1, 2, 2 }, new int[] { 1, 2, 2, 3 }, new int[] { 1, 3, 2, 4 }, new int[] { 2, 2, 4, 2 }, new int[] { 2, 3, 0, -1 }, new int[] { 4, 5, 0, -1 }, new int[] { 3, -1, 0, -1 } };

            for (int i = 0; i < tests.Length; ++i)
            {
                int[] t = tests[i];
                int s = t[0];
                int c = t[1];
                int rs = t[2];
                int rc = t[3];

                try
                {
                    string str = Character.ToString(codePoints, s, c);
                    Assert.IsFalse(rc == -1);
                    Assert.AreEqual(cpString.Substring(rs, rc), str);
                    continue;
                }
                catch (ArgumentException e2)
                {
                    // Ignored.
                }
                Assert.IsTrue(rc == -1);
            }
        }
    }
}
