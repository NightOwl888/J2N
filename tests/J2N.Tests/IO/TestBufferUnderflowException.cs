using NUnit.Framework;
using System.IO;

namespace J2N.IO
{
    public class TestBufferUnderflowException : TestCase
    {
#if FEATURE_SERIALIZABLE_EXCEPTIONS
        /**
         * @tests serialization/deserialization compatibility.
         */
        [Test]
        public void TestSerializationSelf()
        {

            //SerializationTest.verifySelf(new BufferUnderflowException());

            var ex = new BufferUnderflowException("testing", new IOException("testing12"));
            var clone = Clone(ex);

            assertNotSame(ex, clone);
            assertNotSame(ex.InnerException, clone.InnerException);
            assertEquals("testing", clone.Message);
            assertEquals("testing12", clone.InnerException.Message);
        }

        /////**
        //// * @tests serialization/deserialization compatibility with RI.
        //// */
        ////public void TestSerializationCompatibility()
        ////{

        ////    SerializationTest.verifyGolden(this, new BufferUnderflowException());
        ////}
#endif

        /**
         *@tests {@link java.nio.BufferUnderflowException#BufferUnderflowException()}
         */
        [Test]
        public void Test_Constructor()
        {
            BufferUnderflowException exception = new BufferUnderflowException();
            assertEquals($"Exception of type '{typeof(BufferUnderflowException).FullName}' was thrown.", exception.Message);
            //assertNull(exception.getLocalizedMessage());
            assertNull(exception.InnerException);
        }
    }
}
