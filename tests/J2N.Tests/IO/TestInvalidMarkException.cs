using NUnit.Framework;
using System.IO;

namespace J2N.IO
{
    public class TestInvalidMarkException : TestCase
    {
#if FEATURE_SERIALIZABLE_EXCEPTIONS
        /**
         * @tests serialization/deserialization compatibility.
         */
        [Test]
        public void TestSerializationSelf()
        {

            //SerializationTest.verifySelf(new InvalidMarkException());
            var ex = new InvalidMarkException("testing", new IOException("testing12"));
            var clone = Clone(ex);

            assertNotSame(ex, clone);
            assertNotSame(ex.InnerException, clone.InnerException);
            assertEquals("testing", clone.Message);
            assertEquals("testing12", clone.InnerException.Message);
        }

        /////**
        //// * @tests serialization/deserialization compatibility with RI.
        //// */
        ////[Test]
        ////public void TestSerializationCompatibility()
        ////{

        ////    SerializationTest.verifyGolden(this, new InvalidMarkException());
        ////}
#endif

        /**
         *@tests {@link java.nio.InvalidMarkException#InvalidMarkException()}
         */
        [Test]
        public void Test_Constructor()
        {
            InvalidMarkException exception = new InvalidMarkException();
            assertEquals($"Exception of type '{typeof(InvalidMarkException).FullName}' was thrown.", exception.Message);
            //assertNull(exception.getLocalizedMessage());
            assertNull(exception.InnerException);
        }
    }
}
