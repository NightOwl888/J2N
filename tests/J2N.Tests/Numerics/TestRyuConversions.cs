using NUnit.Framework;

namespace J2N.Numerics {
    public class TestRyuConversions : TestCase {
        

        [Test]
        public void DecimalOnly() {
            float f = .123f;

            string s = RyuConversion.FloatToString(f);
            assertEquals(".123", s);
        }

        [Test]
        public void SmallNumber() {
            float f = 123.45f;

            string s = RyuConversion.FloatToString(f);
            assertEquals("123.45", s);
        }


        [Test]
        public void ScientificNotation1() {
            float f = 1e7f;

            string s = RyuConversion.FloatToString(f);
            assertEquals("1.0E7", s);
        }

        [Test]
        public void ScientificNotation2() {
            float f = 1e7f + 1;

            string s = RyuConversion.FloatToString(f);
            assertEquals("1.0000001E7", s);
        }


        [Test]
        public void LargestNonScientificNotation() {
            float f = 1e7f - 1;

            string s = RyuConversion.FloatToString(f);
            assertEquals("9999999", s);
        }

        /**
         * This example was chosed because it's one that failed under the old 
         * float to string approach.
         */
        [Test]
        public void Example1() {
            float f = 12.90898f;

            string s = RyuConversion.FloatToString(f);
            assertEquals("12.90898", s);
        }

        /**
         * This example was chosed because it's one that failed under the old 
         * float to string approach.
         */
        [Test]
        public void Example2() {
            float f = 1.0e19f;

            string s = RyuConversion.FloatToString(f);
            assertEquals("1.0E19", s);
        }

        /**
         * This example was chosed because it's one that failed under the old 
         * float to string approach.
         */
        [Test]
        public void Example3() {
            float f = 1.0E-36f;

            string s = RyuConversion.FloatToString(f);
            assertEquals("1.0E-36", s);
        }

    }
}
