using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace J2N.Text
{
    public class TestParseException : TestCase
    {
#if FEATURE_SERIALIZABLE_EXCEPTIONS
        /**
         * @tests serialization/deserialization compatibility.
         */
        [Test]
        public void TestSerializationSelf()
        {

            //SerializationTest.verifySelf(new BufferOverflowException());
            var ex = new ParseException("testing", 1234, new IOException("testing12"));
            var clone = Clone(ex);

            assertNotSame(ex, clone);
            assertNotSame(ex.InnerException, clone.InnerException);
            assertEquals("testing", clone.Message);
            assertEquals(1234, clone.ErrorOffset);
            assertEquals("testing12", clone.InnerException.Message);
        }

        /////**
        //// * @tests serialization/deserialization compatibility with RI.
        //// */
        ////public void testSerializationCompatibility() 
        ////{

        ////    SerializationTest.verifyGolden(this, new BufferOverflowException());
        ////}
#endif

        /**
         * @tests java.text.ParseException#ParseException(java.lang.String, int)
         */
        [Test]
        public void test_ConstructorLjava_lang_StringI()
        {
            //try
            //{
            //    DateFormat df = DateFormat.getInstance();
            //    df.parse("HelloWorld");
            //    fail("ParseException not created/thrown.");
            //}
            //catch (ParseException e)
            //{
            //    // expected
            //}

            // J2N: We don't have anything that will throw this, so we just test
            // the basic constructors to verify it works.
            var target = new ParseException("foo", 43);
            assertEquals("foo", target.Message);
            assertEquals(43, target.ErrorOffset);

            var target2 = new ParseException("bar", 67, new InvalidOperationException("Something bad happened."));
            assertEquals("bar", target2.Message);
            assertEquals(67, target2.ErrorOffset);
            assertTrue(target2.InnerException is InvalidOperationException);
            assertEquals("Something bad happened.", target2.InnerException.Message);
        }

        ///**
        // * @tests java.text.ParseException#getErrorOffset()
        // */
        //[Test]
        //public void test_getErrorOffset()
        //{
        //    //try
        //    //{
        //    //    DateFormat df = DateFormat.getInstance();
        //    //    df.parse("1999HelloWorld");
        //    //}
        //    //catch (ParseException e)
        //    //{
        //    //    assertEquals("getErrorOffsetFailed.", 4, e.getErrorOffset());
        //    //}
        //}
    }
}
