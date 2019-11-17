using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace J2N.Collections
{
    public class TestArrays : TestCase
    {
        /**
        * @tests java.util.Arrays#equals(double[], double[])
        */
        [Test]
        public void Test_equals_D_D()
        {
            double[] d = new double[100];
            double[] x = new double[100];
            Arrays.Fill(d, double.MaxValue);
            Arrays.Fill(x, double.MinValue);

            assertTrue("Assert 0: Inequal arrays returned true", !Arrays.Equals(d, x));

            Arrays.Fill(x, double.MaxValue);
            assertTrue("Assert 1: equal arrays returned false", Arrays.Equals(d, x));

            assertTrue("Assert 2: should be false",
                    !Arrays.Equals(new double[] { 1.0 }, new double[] { 2.0 }));

            assertTrue("Assert 3: NaN not equals",
                    Arrays.Equals(new double[] { Double.NaN }, new double[] { Double.NaN }));
            assertTrue("Assert 4: 0d equals -0d",
                    !Arrays.Equals(new double[] { 0d }, new double[] { -0d }));
        }

        /**
         * @tests java.util.Arrays#equals(float[], float[])
         */
        [Test]
        public void Test_equals_F_F()
        {
            float[] d = new float[100];
            float[] x = new float[100];
            Arrays.Fill(d, float.MaxValue);
            Arrays.Fill(x, float.MinValue);

            assertTrue("Assert 0: Inequal arrays returned true", !Arrays.Equals(d, x));

            Arrays.Fill(x, float.MaxValue);
            assertTrue("Assert 1: equal arrays returned false", Arrays.Equals(d, x));

            assertTrue("Assert 2: NaN not equals",
                    Arrays.Equals(new float[] { float.NaN }, new float[] { float.NaN }));
            assertTrue("Assert 3: 0f equals -0f",
                    !Arrays.Equals(new float[] { 0f }, new float[] { -0f }));
        }

        /**
         * @tests java.util.Arrays#toString(boolean[])
         */
        [Test]
        public void test_toString_Z()
        {
            assertEquals("null", Arrays.ToString((bool[])null));
            assertEquals("[]", Arrays.ToString(new bool[] { }));
            assertEquals("[true]", Arrays.ToString(new bool[] { true }));
            assertEquals("[true, false]", Arrays.ToString(new bool[] { true, false }));
            assertEquals("[true, false, true]", Arrays.ToString(new bool[] { true, false, true }));
        }

        /**
         * @tests java.util.Arrays#toString(byte[])
         */
        [Test]
        public void test_toString_B()
        {
            assertEquals("null", Arrays.ToString((byte[])null));
            assertEquals("[]", Arrays.ToString(new byte[] { }));
            assertEquals("[0]", Arrays.ToString(new byte[] { 0 }));
            assertEquals("[-1, 0]", Arrays.ToString(new sbyte[] { -1, 0 }));
            assertEquals("[-1, 0, 1]", Arrays.ToString(new sbyte[] { -1, 0, 1 }));
        }

        /**
         * @tests java.util.Arrays#toString(char[])
         */
        [Test]
        public void test_toString_C()
        {
            assertEquals("null", Arrays.ToString((char[])null));
            assertEquals("[]", Arrays.ToString(new char[] { }));
            assertEquals("[a]", Arrays.ToString(new char[] { 'a' }));
            assertEquals("[a, b]", Arrays.ToString(new char[] { 'a', 'b' }));
            assertEquals("[a, b, c]", Arrays.ToString(new char[] { 'a', 'b', 'c' }));
        }

        /**
         * @tests java.util.Arrays#toString(double[])
         */
        [Test]
        public void test_toString_D()
        {
            assertEquals("null", Arrays.ToString((double[])null));
            assertEquals("[]", Arrays.ToString(new double[] { }));
            assertEquals("[0.0]", Arrays.ToString(new double[] { 0.0D }));
            assertEquals("[-1.0, 0.0]", Arrays.ToString(new double[] { -1.0D, 0.0D }));
            assertEquals("[-1.0, 0.0, 1.0]", Arrays.ToString(new double[] { -1.0D, 0.0D, 1.0D }));
        }

        /**
         * @tests java.util.Arrays#toString(float[])
         */
        [Test]
        public void test_toString_F()
        {
            

            assertEquals("null", Arrays.ToString((float[])null));
            assertEquals("[]", Arrays.ToString(new float[] { }));
            assertEquals("[0.0]", Arrays.ToString(new float[] { 0.0F }));
            assertEquals("[-1.0, 0.0]", Arrays.ToString(new float[] { -1.0F, 0.0F }));
            assertEquals("[-1.0, 0.0, 1.0]", Arrays.ToString(new float[] { -1.0F, 0.0F, 1.0F }));
        }

        /**
         * @tests java.util.Arrays#toString(int[])
         */
        [Test]
        public void test_toString_I()
        {
            assertEquals("null", Arrays.ToString((int[])null));
            assertEquals("[]", Arrays.ToString(new int[] { }));
            assertEquals("[0]", Arrays.ToString(new int[] { 0 }));
            assertEquals("[-1, 0]", Arrays.ToString(new int[] { -1, 0 }));
            assertEquals("[-1, 0, 1]", Arrays.ToString(new int[] { -1, 0, 1 }));
        }

        /**
         * @tests java.util.Arrays#toString(long[])
         */
        [Test]
        public void test_toString_J()
        {
            assertEquals("null", Arrays.ToString((long[])null));
            assertEquals("[]", Arrays.ToString(new long[] { }));
            assertEquals("[0]", Arrays.ToString(new long[] { 0 }));
            assertEquals("[-1, 0]", Arrays.ToString(new long[] { -1, 0 }));
            assertEquals("[-1, 0, 1]", Arrays.ToString(new long[] { -1, 0, 1 }));
        }

        /**
         * @tests java.util.Arrays#toString(short[])
         */
        [Test]
        public void test_toString_S()
        {
            assertEquals("null", Arrays.ToString((short[])null));
            assertEquals("[]", Arrays.ToString(new short[] { }));
            assertEquals("[0]", Arrays.ToString(new short[] { 0 }));
            assertEquals("[-1, 0]", Arrays.ToString(new short[] { -1, 0 }));
            assertEquals("[-1, 0, 1]", Arrays.ToString(new short[] { -1, 0, 1 }));
        }

        /**
         * @tests java.util.Arrays#toString(Object[])
         */
        [Test]
        public void test_toString_LSystem_Object()
        {
            assertEquals("null", Arrays.ToString((Object[])null));
            assertEquals("[]", Arrays.ToString(new Object[] { }));
            assertEquals("[fixture]", Arrays.ToString(new Object[] { "fixture" }));
            assertEquals("[fixture, null]", Arrays.ToString(new Object[] { "fixture", null }));
            assertEquals("[fixture, null, fixture]", Arrays.ToString(new Object[] { "fixture", null, "fixture" }));
        }

        ///**
        // * @tests java.util.Arrays#deepToString(Object[])
        // */
        //public void test_deepToString_System_Object()
        //{
        //    assertEquals("null", Arrays.deepToString((Object[])null));
        //    assertEquals("[]", Arrays.deepToString(new Object[] { }));
        //    assertEquals("[fixture]", Arrays.deepToString(new Object[] { "fixture" }));
        //    assertEquals("[fixture, null]", Arrays.deepToString(new Object[] { "fixture", null }));
        //    assertEquals("[fixture, null, fixture]", Arrays.deepToString(new Object[] { "fixture", null, "fixture" }));

        //    Object[] fixture = new Object[1];
        //    fixture[0] = fixture;
        //    assertEquals("[[...]]", Arrays.deepToString(fixture));

        //    fixture = new Object[2];
        //    fixture[0] = "fixture";
        //    fixture[1] = fixture;
        //    assertEquals("[fixture, [...]]", Arrays.DeepToString(fixture));

        //    fixture = new Object[10];
        //    fixture[0] = new bool[] { true, false };
        //    fixture[1] = new byte[] { 0, 1 };
        //    fixture[2] = new char[] { 'a', 'b' };
        //    fixture[3] = new double[] { 0.0D, 1.0D };
        //    fixture[4] = new float[] { 0.0F, 1.0F };
        //    fixture[5] = new int[] { 0, 1 };
        //    fixture[6] = new long[] { 0L, 1L };
        //    fixture[7] = new short[] { 0, 1 };
        //    fixture[8] = fixture[0];
        //    fixture[9] = new Object[9];
        //    ((Object[])fixture[9])[0] = fixture;
        //    ((Object[])fixture[9])[1] = fixture[1];
        //    ((Object[])fixture[9])[2] = fixture[2];
        //    ((Object[])fixture[9])[3] = fixture[3];
        //    ((Object[])fixture[9])[4] = fixture[4];
        //    ((Object[])fixture[9])[5] = fixture[5];
        //    ((Object[])fixture[9])[6] = fixture[6];
        //    ((Object[])fixture[9])[7] = fixture[7];
        //    Object[] innerFixture = new Object[4];
        //    innerFixture[0] = "innerFixture0";
        //    innerFixture[1] = innerFixture;
        //    innerFixture[2] = fixture;
        //    innerFixture[3] = "innerFixture3";
        //    ((Object[])fixture[9])[8] = innerFixture;

        //    String expected = "[[true, false], [0, 1], [a, b], [0.0, 1.0], [0.0, 1.0], [0, 1], [0, 1], [0, 1], [true, false], [[...], [0, 1], [a, b], [0.0, 1.0], [0.0, 1.0], [0, 1], [0, 1], [0, 1], [innerFixture0, [...], [...], innerFixture3]]]";

        //    assertEquals(expected, Arrays.DeepToString(fixture));
        //}
    }
}
