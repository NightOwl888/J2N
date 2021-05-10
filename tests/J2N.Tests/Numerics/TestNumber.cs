using NUnit.Framework;

namespace J2N.Numerics
{
    public class TestNumber : TestCase
    {
        /**
     * @tests java.lang.Number#byteValue()
     */
        [Test]
        public void Test_byteValue()
        {
            int number = 1231243;
            assertTrue("Incorrect byte returned for: " + number,
                    ((byte)new Int32(number).GetInt32Value()) == new Int32(number)
                            .GetByteValue());
            number = 0;
            assertTrue("Incorrect byte returned for: " + number,
                    ((byte)new Int32(number).GetInt32Value()) == new Int32(number)
                            .GetByteValue());
            number = -1;
            assertTrue("Incorrect byte returned for: " + number,
                    ((byte)new Int32(number).GetInt32Value()) == new Int32(number)
                            .GetByteValue());
            number = -84109328;
            assertTrue("Incorrect byte returned for: " + number,
                    ((byte)new Int32(number).GetInt32Value()) == new Int32(number)
                            .GetByteValue());
        }

        /**
         * @tests java.lang.Number#shortValue()
         */
        [Test]
        public void Test_shortValue()
        {
            int number = 1231243;
            assertTrue("Incorrect byte returned for: " + number,
                    ((short)new Int32(number).GetInt32Value()) == new Int32(number)
                            .GetInt16Value());
            number = 0;
            assertTrue("Incorrect byte returned for: " + number,
                    ((short)new Int32(number).GetInt32Value()) == new Int32(number)
                            .GetInt16Value());
            number = -1;
            assertTrue("Incorrect byte returned for: " + number,
                    ((short)new Int32(number).GetInt32Value()) == new Int32(number)
                            .GetInt16Value());
            number = -84109328;
            assertTrue("Incorrect byte returned for: " + number,
                    ((short)new Int32(number).GetInt32Value()) == new Int32(number)
                            .GetInt16Value());

        }
    }
}
