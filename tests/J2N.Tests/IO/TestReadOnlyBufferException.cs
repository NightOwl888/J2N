using NUnit.Framework;
using System.IO;

namespace J2N.IO
{
    public class TestReadOnlyBufferException : TestCase
    {
#if FEATURE_SERIALIZABLE_EXCEPTIONS
        /**
         * @tests serialization/deserialization compatibility.
         */
        [Test]
        public void TestSerializationSelf()
        {

            //SerializationTest.verifySelf(new ReadOnlyBufferException());
            var ex = new ReadOnlyBufferException("testing", new IOException("testing12"));
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

        ////    SerializationTest.verifyGolden(this, new ReadOnlyBufferException());
        ////}
#endif

        /**
         *@tests {@link java.nio.ReadOnlyBufferException#ReadOnlyBufferException()}
         */
        [Test]
        public void Test_Constructor()
        {
            ReadOnlyBufferException exception = new ReadOnlyBufferException();
            assertEquals("Specified method is not supported.", exception.Message);
            //assertNull(exception.getLocalizedMessage());
            assertNull(exception.InnerException);
        }
    }
}
