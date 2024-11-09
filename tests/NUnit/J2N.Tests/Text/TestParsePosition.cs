using NUnit.Framework;

namespace J2N.Text
{
    public class TestParsePosition : TestCase
    {
        ParsePosition pp;

        /**
         * @tests java.text.ParsePosition#ParsePosition(int) 
         */
        [Test]
        public void Test_ConstructorI()
        {
            // Test for method java.text.ParsePosition(int)
            ParsePosition pp1 = new ParsePosition(int.MinValue);
            assertTrue("Initialization failed.", pp1.Index == int.MinValue);
            assertEquals("Initialization failed.", -1, pp1.ErrorIndex);
        }

        /**
         * @tests java.text.ParsePosition#equals(java.lang.Object)
         */
        [Test]
        public void Test_equalsLjava_lang_Object()
        {
            // Test for method boolean
            // java.text.ParsePosition.equals(java.lang.Object)
            ParsePosition pp2 = new ParsePosition(43);
            pp2.ErrorIndex=(56);
            assertTrue("equals failed.", !pp.Equals(pp2));
            pp.ErrorIndex=(56);
            pp.Index=(43);
            assertTrue("equals failed.", pp.Equals(pp2));
        }

        /**
         * @tests java.text.ParsePosition#getErrorIndex()
         */
        [Test]
        public void Test_getErrorIndex()
        {
            // Test for method int java.text.ParsePosition.getErrorIndex()
            pp.ErrorIndex=(56);
            assertEquals("getErrorIndex failed.", 56, pp.ErrorIndex);
        }

        /**
         * @tests java.text.ParsePosition#getIndex()
         */
        [Test]
        public void Test_getIndex()
        {
            // Test for method int java.text.ParsePosition.getIndex()
            assertTrue("getIndex failed.", pp.Index == int.MaxValue);
        }

        /**
         * @tests java.text.ParsePosition#hashCode()
         */
        [Test]
        public void Test_hashCode()
        {
            // Test for method int java.text.ParsePosition.hashCode()
            assertTrue("Wrong hashCode returned", (pp.GetHashCode() == pp.Index
                    + pp.ErrorIndex));
        }

        /**
         * @tests java.text.ParsePosition#setErrorIndex(int)
         */
        [Test]
        public void Test_setErrorIndexI()
        {
            // Test for method void java.text.ParsePosition.setErrorIndex(int)
            pp.ErrorIndex = (4564);
            assertEquals("setErrorIndex failed.", 4564, pp.ErrorIndex);
        }

        /**
         * @tests java.text.ParsePosition#setIndex(int)
         */
        [Test]
        public void Test_setIndexI()
        {
            // Test for method void java.text.ParsePosition.setIndex(int)
            pp.Index=(4564);
            assertEquals("setErrorIndex failed.", 4564, pp.Index);
        }

        /**
         * @tests java.text.ParsePosition#toString()
         */
        [Test]
        public void Test_toString()
        {
            // Test for method java.lang.String java.text.ParsePosition.toString()
            assertEquals("toString failed.",
                    "J2N.Text.ParsePosition[index=2147483647, errorIndex=-1]", pp.ToString());
        }

        /**
         * Sets up the fixture, for example, open a network connection. This method
         * is called before a test is executed.
         */
        public override void SetUp()
        {

            pp = new ParsePosition(int.MaxValue);
        }

        /**
         * Tears down the fixture, for example, close a network connection. This
         * method is called after a test is executed.
         */
        public override void TearDown()
        {
        }
    }
}
