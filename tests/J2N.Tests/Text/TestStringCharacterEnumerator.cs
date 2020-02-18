using NUnit.Framework;
using System;

namespace J2N.Text
{

    public class TestStringCharacterEnumerator : TestCase
    {
        /**
         * @tests java.text.StringCharacterIterator.StringCharacterIterator(String,
         *        int)
         */
        [Test]
        public void Test_ConstructorI()
        {
            assertNotNull(new StringCharacterEnumerator("value", 0));
            assertNotNull(new StringCharacterEnumerator("value", "value".Length));
            assertNotNull(new StringCharacterEnumerator("", 0));
            try
            {
                new StringCharacterEnumerator(null, 0);
                fail("Assert 0: no null pointer");
            }
            catch (ArgumentNullException e)
            {
                // expected
            }

            try
            {
                new StringCharacterEnumerator("value", -1);
                fail("Assert 1: no illegal argument");
            }
            catch (ArgumentOutOfRangeException e)
            {
                // expected
            }

            try
            {
                new StringCharacterEnumerator("value", "value".Length + 1);
                fail("Assert 2: no illegal argument");
            }
            catch (ArgumentOutOfRangeException e)
            {
                // expected
            }
        }

        /**
         * @tests java.text.StringCharacterIterator(String, int, int, int)
         */
        [Test]
        public void Test_ConstructorIII()
        {
            assertNotNull(new StringCharacterEnumerator("value", 0, "value".Length,
                    0));
            assertNotNull(new StringCharacterEnumerator("value", 0, "value".Length,
                    1));
            assertNotNull(new StringCharacterEnumerator("", 0, 0, 0));

            try
            {
                new StringCharacterEnumerator(null, 0, 0, 0);
                fail("no null pointer");
            }
            catch (ArgumentNullException e)
            {
                // Expected
            }

            try
            {
                new StringCharacterEnumerator("value", -1, "value".Length, 0);
                fail("no illegal argument: invalid begin");
            }
            catch (ArgumentOutOfRangeException e)
            {
                // Expected
            }

            try
            {
                new StringCharacterEnumerator("value", 0, "value".Length + 1, 0);
                fail("no illegal argument: invalid end");
            }
            catch (ArgumentOutOfRangeException e)
            {
                // Expected
            }

            try
            {
                new StringCharacterEnumerator("value", 2, -1, 0);
                fail("no illegal argument: start greater than end");
            }
            catch (ArgumentOutOfRangeException e)
            {
                // Expected
            }

            try
            {
                new StringCharacterEnumerator("value", 2, -1, 2);
                fail("no illegal argument: start greater than end");
            }
            catch (ArgumentOutOfRangeException e)
            {
                // Expected
            }

            try
            {
                new StringCharacterEnumerator("value", 2, 4, 1);
                fail("no illegal argument: location greater than start");
            }
            catch (ArgumentOutOfRangeException e)
            {
                // Expected
            }

            try
            {
                new StringCharacterEnumerator("value", 0, 2, 3);
                fail("no illegal argument: location greater than start");
            }
            catch (ArgumentOutOfRangeException e)
            {
                // Expected
            }
        }

        /**
         * @tests java.text.StringCharacterIterator.equals(Object)
         */
        [Test]
        public void Test_equalsLjava_lang_Object()
        {
            StringCharacterEnumerator sci0 = new StringCharacterEnumerator("fixture");
            assertEquals(sci0, sci0);
            assertFalse(sci0.Equals(null));
            assertFalse(sci0.Equals("fixture"));

            StringCharacterEnumerator sci1 = new StringCharacterEnumerator("fixture");
            assertEquals(sci0, sci1);

            sci1.MoveNext();
            assertFalse(sci0.Equals(sci1));
            sci0.MoveNext();
            assertEquals(sci0, sci1);

            StringCharacterEnumerator it1 = new StringCharacterEnumerator("testing", 2,
                    4, 4);
            StringCharacterEnumerator it2 = new StringCharacterEnumerator("xxstinx", 2,
                    4, 4);
            assertTrue("Range is equal", !it1.Equals(it2));
            StringCharacterEnumerator it3 = new StringCharacterEnumerator("testing", 2,
                    4, 2);
            it3.Index = 4;
            assertTrue("Not equal", it1.Equals(it3));
        }

        /**
         * @tests java.text.StringCharacterIterator.clone()
         */
        [Test]
        public void Test_clone()
        {
            StringCharacterEnumerator sci0 = new StringCharacterEnumerator("fixture");
            assertSame(sci0, sci0);
            StringCharacterEnumerator sci1 = (StringCharacterEnumerator)sci0.Clone();
            assertNotSame(sci0, sci1);
            assertEquals(sci0, sci1);

            StringCharacterEnumerator it = new StringCharacterEnumerator("testing", 2,
                    4, 4);
            StringCharacterEnumerator clone = (StringCharacterEnumerator)it.Clone();
            assertTrue("Clone not equal", it.Equals(clone));
        }

        /**
         * @tests java.text.StringCharacterIterator.Current
         */
        [Test]
        public void Test_current()
        {
            StringCharacterEnumerator fixture = new StringCharacterEnumerator("fixture");
            assertEquals('f', fixture.Current);
            fixture.MoveNext();
            assertEquals('i', fixture.Current);

            StringCharacterEnumerator it =
                new StringCharacterEnumerator("testing", 2, 4, 4);
            assertEquals("Wrong current char", 'i', it.Current);
        }

        /**
         * @tests java.text.StringCharacterIterator.First()
         */
        [Test]
        public void Test_first()
        {
            StringCharacterEnumerator fixture = new StringCharacterEnumerator("fixture");
            assertTrue(fixture.MoveFirst());
            assertEquals('f', fixture.Current);
            fixture.MoveNext();
            assertTrue(fixture.MoveFirst());
            assertEquals('f', fixture.Current);
            fixture = new StringCharacterEnumerator("fixture", 1);
            assertTrue(fixture.MoveFirst());
            assertEquals('f', fixture.Current);
            fixture = new StringCharacterEnumerator("fixture", 1, "fixture".Length - 1,
                    2);
            assertTrue(fixture.MoveFirst());
            assertEquals('i', fixture.Current);

            StringCharacterEnumerator it1 =
                new StringCharacterEnumerator("testing", 2, 4, 4);
            assertTrue(it1.MoveFirst());
            assertEquals("Wrong first char", 's', it1.Current);
            assertTrue(it1.MoveNext());
            assertEquals("Wrong next char", 't', it1.Current);
            it1 = new StringCharacterEnumerator("testing", 2, 0, 2);
            assertTrue("Not DONE", it1.MoveFirst() == false);
        }

        /**
         * @tests java.text.StringCharacterIterator.getBeginIndex()
         */
        [Test]
        public void Test_BeginIndex()
        {
            StringCharacterEnumerator fixture = new StringCharacterEnumerator("fixture");
            assertEquals(0, fixture.StartIndex);
            fixture = new StringCharacterEnumerator("fixture", 1);
            assertEquals(0, fixture.StartIndex);
            fixture = new StringCharacterEnumerator("fixture", 1, "fixture".Length - 1,
                    2);
            assertEquals(1, fixture.StartIndex);

            StringCharacterEnumerator it1 =
                new StringCharacterEnumerator("testing", 2, 4, 4);
            assertEquals("Wrong begin index 2", 2, it1.StartIndex);
        }

        /**
         * @tests java.text.StringCharacterIterator.getEndIndex()
         */
        [Test]
        public void Test_EndIndex()
        {
            StringCharacterEnumerator fixture = new StringCharacterEnumerator("fixture");
            assertEquals("fixture".Length - 1, fixture.EndIndex);
            fixture = new StringCharacterEnumerator("fixture", 1);
            assertEquals("fixture".Length - 1, fixture.EndIndex);
            fixture = new StringCharacterEnumerator("fixture", 1, "fixture".Length - 1,
                    2);
            assertEquals("fixture".Length - 1, fixture.EndIndex);
            fixture = new StringCharacterEnumerator("fixture", 1, 3, 2);
            assertEquals(3, fixture.EndIndex);

            StringCharacterEnumerator it1 =
                new StringCharacterEnumerator("testing", 2, 4, 4);
            assertEquals("Wrong end index 5", 5, it1.EndIndex);
        }


        /**
         * @tests java.text.StringCharacterIterator.getEndIndex()
         */
        [Test]
        public void Test_Length()
        {
            StringCharacterEnumerator fixture = new StringCharacterEnumerator("fixture");
            assertEquals("fixture".Length, fixture.Length);
            fixture = new StringCharacterEnumerator("fixture", 1);
            assertEquals("fixture".Length, fixture.Length);
            fixture = new StringCharacterEnumerator("fixture", 1, "fixture".Length - 1,
                    2);
            assertEquals("fixture".Length - 1, fixture.Length);
            fixture = new StringCharacterEnumerator("fixture", 1, 4, 2);
            assertEquals(4, fixture.Length);

            StringCharacterEnumerator it1 =
                new StringCharacterEnumerator("testing", 2, 4, 4);
            assertEquals("Wrong length 4", 4, it1.Length);
        }

        /**
         * @tests java.text.StringCharacterIterator.Index
         */
        [Test]
        public void Test_Position()
        {
            StringCharacterEnumerator fixture = new StringCharacterEnumerator("fixture");
            assertEquals(0, fixture.Index);
            fixture = new StringCharacterEnumerator("fixture", 1);
            assertEquals(1, fixture.Index);
            fixture = new StringCharacterEnumerator("fixture", 1, "fixture".Length - 1,
                    2);
            assertEquals(2, fixture.Index);
            fixture = new StringCharacterEnumerator("fixture", 1, 4, 2);
            assertEquals(2, fixture.Index);
        }

        /**
         * @tests java.text.StringCharacterIterator.Last()
         */
        [Test]
        public void TestLast()
        {
            StringCharacterEnumerator fixture = new StringCharacterEnumerator("fixture");
            assertTrue(fixture.MoveLast());
            assertEquals('e', fixture.Current);
            fixture.MoveNext();
            assertTrue(fixture.MoveLast());
            assertEquals('e', fixture.Current);
            fixture = new StringCharacterEnumerator("fixture", 1);
            assertTrue(fixture.MoveLast());
            assertEquals('e', fixture.Current);
            fixture = new StringCharacterEnumerator("fixture", 1, "fixture".Length - 1,
                    2);
            assertTrue(fixture.MoveLast());
            assertEquals('e', fixture.Current);
            fixture = new StringCharacterEnumerator("fixture", 1, 3, 2);
            assertTrue(fixture.MoveLast());
            assertEquals('t', fixture.Current);
        }

        /**
         * @tests java.text.StringCharacterIterator.Next()
         */
        [Test]
        public void Test_next()
        {
            StringCharacterEnumerator fixture = new StringCharacterEnumerator("fixture");
            assertEquals(0, fixture.Index);
            assertTrue(fixture.MoveNext());
            assertEquals('i', fixture.Current);
            assertEquals(1, fixture.Index);
            assertTrue(fixture.MoveNext());
            assertEquals('x', fixture.Current);
            assertEquals(2, fixture.Index);
            assertTrue(fixture.MoveNext());
            assertEquals('t', fixture.Current);
            assertEquals(3, fixture.Index);
            assertTrue(fixture.MoveNext());
            assertEquals('u', fixture.Current);
            assertEquals(4, fixture.Index);
            assertTrue(fixture.MoveNext());
            assertEquals('r', fixture.Current);
            assertEquals(5, fixture.Index);
            assertTrue(fixture.MoveNext());
            assertEquals('e', fixture.Current);
            assertEquals(6, fixture.Index);
            assertFalse(fixture.MoveNext());
            assertEquals(6, fixture.Index);
            assertFalse(fixture.MoveNext());
            assertEquals(6, fixture.Index);
            assertFalse(fixture.MoveNext());
            assertEquals(6, fixture.Index);

            StringCharacterEnumerator it1 = new StringCharacterEnumerator("testing", 2,
                    4, 3);
            assertTrue(it1.MoveNext());
            char result = it1.Current;
            assertEquals("Wrong MoveNext() char1", 'i', result);
            assertTrue(it1.MoveNext());
            assertEquals("Wrong MoveNext() char2", 'n', it1.Current);
            assertTrue("Wrong MoveNext() char3", it1.MoveNext() == false);
            assertTrue("Wrong MoveNext() char4", it1.MoveNext() == false);
            int index = it1.Index;
            assertEquals("Wrong index", 5, index);
            assertTrue("Wrong current char",
                       it1.Current == 'n');
        }

        /**
         * @tests java.text.StringCharacterIterator.Previous()
         */
        [Test]
        public void Test_previous()
        {
            StringCharacterEnumerator fixture = new StringCharacterEnumerator("fixture");
            assertEquals(false, fixture.MovePrevious());
            assertTrue(fixture.MoveNext());
            assertEquals('i', fixture.Current);
            assertTrue(fixture.MoveNext());
            assertEquals('x', fixture.Current);
            assertTrue(fixture.MoveNext());
            assertEquals('t', fixture.Current);
            assertTrue(fixture.MoveNext());
            assertEquals('u', fixture.Current);
            assertTrue(fixture.MoveNext());
            assertEquals('r', fixture.Current);
            assertTrue(fixture.MoveNext());
            assertEquals('e', fixture.Current);
            assertEquals(false, fixture.MoveNext());
            assertEquals(false, fixture.MoveNext());
            assertEquals(false, fixture.MoveNext());
            assertEquals(6, fixture.Index);
            //assertTrue(fixture.MovePrevious());
            assertEquals('e', fixture.Current);
            assertEquals(6, fixture.Index);
            assertTrue(fixture.MovePrevious());
            assertEquals('r', fixture.Current);
            assertEquals(5, fixture.Index);
            assertTrue(fixture.MovePrevious());
            assertEquals('u', fixture.Current);
            assertEquals(4, fixture.Index);
            assertTrue(fixture.MovePrevious());
            assertEquals('t', fixture.Current);
            assertEquals(3, fixture.Index);
            assertTrue(fixture.MovePrevious());
            assertEquals('x', fixture.Current);
            assertEquals(2, fixture.Index);
            assertTrue(fixture.MovePrevious());
            assertEquals('i', fixture.Current);
            assertEquals(1, fixture.Index);
            assertTrue(fixture.MovePrevious());
            assertEquals('f', fixture.Current);
            assertEquals(0, fixture.Index);
            assertEquals(false, fixture.MovePrevious());
            assertEquals(0, fixture.Index);

            StringCharacterEnumerator it1 =
                new StringCharacterEnumerator("testing", 2, 4, 4);
            assertTrue(it1.MovePrevious());
            assertEquals("Wrong previous char1", 't', it1.Current);
            assertTrue(it1.MovePrevious());
            assertEquals("Wrong previous char2", 's', it1.Current);
            assertTrue("Wrong previous char3",
                       it1.MovePrevious() == false);
            assertTrue("Wrong previous char4",
                       it1.MovePrevious() == false);
            assertEquals("Wrong index", 2, it1.Index);
            assertEquals("Wrong current char", 's', it1.Current);
        }

        /**
         * @tests java.text.StringCharacterIterator.setIndex(int)
         */
        [Test]
        public void Test_setIndex()
        {
            StringCharacterEnumerator fixture = new StringCharacterEnumerator("fixture");
            while (fixture.MoveNext())
            {
                // empty
            }
            assertEquals("fixture".Length - 1, fixture.Index);
            fixture.Index = 0;
            assertEquals(0, fixture.Index);
            assertEquals('f', fixture.Current);
            fixture.Index = "fixture".Length - 1;
            assertEquals('e', fixture.Current);
            try
            {
                fixture.Index = (-1);
                fail("no illegal argument");
            }
            catch (ArgumentOutOfRangeException e)
            {
                // expected
            }

            try
            {
                fixture.Index = ("fixture".Length + 1);
                fail("no illegal argument");
            }
            catch (ArgumentOutOfRangeException e)
            {
                // expected
            }
        }

        /**
         * @tests java.text.StringCharacterIterator.setText(String)
         */
        [Test]
        public void Test_setText()
        {
            StringCharacterEnumerator fixture = new StringCharacterEnumerator("fixture");
            fixture.Reset("fix");
            assertEquals('f', fixture.Current);
            assertTrue(fixture.MoveLast());
            assertEquals('x', fixture.Current);

            try
            {
                fixture.Reset(null);
                fail("no null pointer");
            }
            catch (ArgumentNullException e)
            {
                // expected
            }
        }

        /**
         * @tests java.text.StringCharacterIterator#StringCharacterIterator(java.lang.String)
         */
        [Test]
        public void Test_ConstructorLjava_lang_String()
        {
            assertNotNull(new StringCharacterEnumerator("value"));
            assertNotNull(new StringCharacterEnumerator(""));
            try
            {
                new StringCharacterEnumerator(null);
                fail("Assert 0: no null pointer");
            }
            catch (ArgumentNullException e)
            {
                // expected
            }

            StringCharacterEnumerator it = new StringCharacterEnumerator("testing");
            assertEquals("Wrong begin index", 0, it.StartIndex);
            assertEquals("Wrong end index", 6, it.EndIndex);
            assertEquals("Wrong current index", 0, it.Index);
            assertEquals("Wrong current char", 't', it.Current);
            assertTrue(it.MoveNext());
            assertEquals("Wrong next char", 'e', it.Current);
        }

        /**
         * @tests java.text.StringCharacterIterator#StringCharacterIterator(java.lang.String,
         *        int)
         */
        [Test]
        public void Test_ConstructorLjava_lang_StringI()
        {
            StringCharacterEnumerator it = new StringCharacterEnumerator("testing", 3);
            assertEquals("Wrong begin index", 0, it.StartIndex);
            assertEquals("Wrong end index", 6, it.EndIndex);
            assertEquals("Wrong current index", 3, it.Index);
            assertEquals("Wrong current char", 't', it.Current);
            assertTrue(it.MoveNext());
            assertEquals("Wrong next char", 'i', it.Current);
        }

        /**
         * @tests java.text.StringCharacterIterator#StringCharacterIterator(java.lang.String,
         *        int, int, int)
         */
        [Test]
        public void Test_ConstructorLjava_lang_StringIII()
        {
            StringCharacterEnumerator it = new StringCharacterEnumerator("testing", 2,
                    4, 4);
            assertEquals("Wrong begin index", 2, it.StartIndex);
            assertEquals("Wrong end index", 5, it.EndIndex);
            assertEquals("Wrong current index", 4, it.Index);
            assertEquals("Wrong current char", 'i', it.Current);
            assertTrue(it.MoveNext());
            assertEquals("Wrong next char", 'n', it.Current);
        }

        /**
         * @tests java.text.StringCharacterIterator#getIndex()
         */
        [Test]
        public void Test_getIndex()
        {
            StringCharacterEnumerator it1 = new StringCharacterEnumerator("testing", 2,
                    4, 4);
            assertEquals("Wrong index 4", 4, it1.Index);
            it1.MoveNext();
            assertEquals("Wrong index 5", 5, it1.Index);
            it1.MoveLast();
            assertEquals("Wrong index 4/2", 5, it1.Index);
        }

        /**
         * @tests java.text.StringCharacterIterator#hashCode()
         */
        [Test]
        public void Test_hashCode()
        {
            StringCharacterEnumerator it1 = new StringCharacterEnumerator("testing", 2,
                    4, 4);
            StringCharacterEnumerator it2 = new StringCharacterEnumerator("xxstinx", 2,
                    4, 4);
            assertTrue("Hash is equal", it1.GetHashCode() != it2.GetHashCode());
            StringCharacterEnumerator it3 = new StringCharacterEnumerator("testing", 2,
                    4, 2);
            assertTrue("Hash equal1", it1.GetHashCode() != it3.GetHashCode());
            it3 = new StringCharacterEnumerator("testing", 0, 6, 4);
            assertTrue("Hash equal2", it1.GetHashCode() != it3.GetHashCode());
            it3 = new StringCharacterEnumerator("testing", 2, 3, 4);
            assertTrue("Hash equal3", it1.GetHashCode() != it3.GetHashCode());
            it3 = new StringCharacterEnumerator("froging", 2, 4, 4);
            assertTrue("Hash equal4", it1.GetHashCode() != it3.GetHashCode());

            StringCharacterEnumerator sci0 = new StringCharacterEnumerator("fixture");
            assertEquals(sci0.GetHashCode(), sci0.GetHashCode());

            StringCharacterEnumerator sci1 = new StringCharacterEnumerator("fixture");
            assertEquals(sci0.GetHashCode(), sci1.GetHashCode());

            sci1.MoveNext();
            sci0.MoveNext();
            assertEquals(sci0.GetHashCode(), sci1.GetHashCode());
        }

        /**
         * @tests java.text.StringCharacterIterator#last()
         */
        [Test]
        public void Test_last()
        {
            StringCharacterEnumerator it1 = new StringCharacterEnumerator("testing", 2,
                    4, 3);
            assertTrue(it1.MoveLast());
            assertEquals("Wrong last char", 'n', it1.Current);
            assertTrue(it1.MovePrevious());
            assertEquals("Wrong previous char", 'i', it1.Current);
            it1 = new StringCharacterEnumerator("testing", 2, 0, 2);
            assertTrue("Not DONE", it1.MoveLast() == false);
        }

        /**
         * @tests java.text.StringCharacterIterator#setIndex(int)
         */
        [Test]
        public void Test_setIndexI()
        {
            StringCharacterEnumerator it1 = new StringCharacterEnumerator("testing", 2,
                    4, 4);
            it1.Index = 2;
            assertEquals("Wrong result1", 's', it1.Current);
            assertTrue(it1.MoveNext());
            char result = it1.Current;
            assertTrue("Wrong next char: " + result, result == 't');
            try
            {
                it1.Index = 6;
                fail("Assert 3: No ArgumentOutOfRangeException");
            }
            catch (ArgumentOutOfRangeException)
            {
                // expected
            }

            assertFalse("Wrong result2", it1.TrySetIndex(6));
            //assertTrue("Wrong result2", it1.SetIndex(6) == CharacterIterator.Done);
            //assertTrue(it1.MovePrevious());
            assertEquals("Wrong previous char", 'n', it1.Current);
            assertTrue(it1.MovePrevious());
            assertEquals("Wrong previous char", 'i', it1.Current);
        }

        /**
         * @tests java.text.StringCharacterIterator#setText(java.lang.String)
         */
        [Test]
        public void Test_setTextLjava_lang_String()
        {
            StringCharacterEnumerator it1 = new StringCharacterEnumerator("testing", 2,
                    4, 4);
            it1.Reset("frog");
            assertEquals("Wrong begin index", 0, it1.StartIndex);
            assertEquals("Wrong end index", 3, it1.EndIndex);
            assertEquals("Wrong current index", 0, it1.Index);
        }
    }

}
