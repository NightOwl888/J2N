using NUnit.Framework;
using System.IO;

namespace J2N.IO
{
    public class TestBufferOverflowException : TestCase
    {
#if FEATURE_SERIALIZABLE_EXCEPTIONS
        /**
         * @tests serialization/deserialization compatibility.
         */
        [Test]
        public void TestSerializationSelf()
        {

            //SerializationTest.verifySelf(new BufferOverflowException());
            var ex = new BufferOverflowException("testing", new IOException("testing12"));
            var clone = Clone(ex);

            assertNotSame(ex, clone);
            assertNotSame(ex.InnerException, clone.InnerException);
            assertEquals("testing", clone.Message);
            assertEquals("testing12", clone.InnerException.Message);
        }

        ///**
        // * @tests serialization/deserialization compatibility with RI.
        // */
        //public void testSerializationCompatibility() 
        //{

        //    SerializationTest.verifyGolden(this, new BufferOverflowException());
        //}
#endif

        /**
         *@tests {@link java.nio.BufferOverflowException#BufferOverflowException()}
         */
        [Test]
        public void Test_Constructor()
        {
            BufferOverflowException exception = new BufferOverflowException();
            assertEquals($"Exception of type '{typeof(BufferOverflowException).FullName}' was thrown.", exception.Message);
            //assertNull(exception.getLocalizedMessage());
            assertNull(exception.InnerException);
        }
    }
}
