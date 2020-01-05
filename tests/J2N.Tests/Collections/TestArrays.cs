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
            ArrayExtensions.Fill(d, double.MaxValue);
            ArrayExtensions.Fill(x, double.MinValue);

            assertTrue("Assert 0: Inequal arrays returned true", !Arrays.Equals(d, x));

            ArrayExtensions.Fill(x, double.MaxValue);
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
            ArrayExtensions.Fill(d, float.MaxValue);
            ArrayExtensions.Fill(x, float.MinValue);

            assertTrue("Assert 0: Inequal arrays returned true", !Arrays.Equals(d, x));

            ArrayExtensions.Fill(x, float.MaxValue);
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
        public void Test_toString_Z()
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
        public void Test_toString_B()
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
        public void Test_toString_C()
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
        public void Test_toString_D()
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
        public void Test_toString_F()
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
        public void Test_toString_I()
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
        public void Test_toString_J()
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
        public void Test_toString_S()
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
        public void Test_toString_LSystem_Object()
        {
            assertEquals("null", Arrays.ToString((Object[])null));
            assertEquals("[]", Arrays.ToString(new Object[] { }));
            assertEquals("[fixture]", Arrays.ToString(new Object[] { "fixture" }));
            assertEquals("[fixture, null]", Arrays.ToString(new Object[] { "fixture", null }));
            assertEquals("[fixture, null, fixture]", Arrays.ToString(new Object[] { "fixture", null, "fixture" }));
        }

        /////**
        //// * @tests java.util.Arrays#deepToString(Object[])
        //// */
        ////public void Test_deepToString_System_Object()
        ////{
        ////    assertEquals("null", Arrays.deepToString((Object[])null));
        ////    assertEquals("[]", Arrays.deepToString(new Object[] { }));
        ////    assertEquals("[fixture]", Arrays.deepToString(new Object[] { "fixture" }));
        ////    assertEquals("[fixture, null]", Arrays.deepToString(new Object[] { "fixture", null }));
        ////    assertEquals("[fixture, null, fixture]", Arrays.deepToString(new Object[] { "fixture", null, "fixture" }));

        ////    Object[] fixture = new Object[1];
        ////    fixture[0] = fixture;
        ////    assertEquals("[[...]]", Arrays.deepToString(fixture));

        ////    fixture = new Object[2];
        ////    fixture[0] = "fixture";
        ////    fixture[1] = fixture;
        ////    assertEquals("[fixture, [...]]", Arrays.DeepToString(fixture));

        ////    fixture = new Object[10];
        ////    fixture[0] = new bool[] { true, false };
        ////    fixture[1] = new byte[] { 0, 1 };
        ////    fixture[2] = new char[] { 'a', 'b' };
        ////    fixture[3] = new double[] { 0.0D, 1.0D };
        ////    fixture[4] = new float[] { 0.0F, 1.0F };
        ////    fixture[5] = new int[] { 0, 1 };
        ////    fixture[6] = new long[] { 0L, 1L };
        ////    fixture[7] = new short[] { 0, 1 };
        ////    fixture[8] = fixture[0];
        ////    fixture[9] = new Object[9];
        ////    ((Object[])fixture[9])[0] = fixture;
        ////    ((Object[])fixture[9])[1] = fixture[1];
        ////    ((Object[])fixture[9])[2] = fixture[2];
        ////    ((Object[])fixture[9])[3] = fixture[3];
        ////    ((Object[])fixture[9])[4] = fixture[4];
        ////    ((Object[])fixture[9])[5] = fixture[5];
        ////    ((Object[])fixture[9])[6] = fixture[6];
        ////    ((Object[])fixture[9])[7] = fixture[7];
        ////    Object[] innerFixture = new Object[4];
        ////    innerFixture[0] = "innerFixture0";
        ////    innerFixture[1] = innerFixture;
        ////    innerFixture[2] = fixture;
        ////    innerFixture[3] = "innerFixture3";
        ////    ((Object[])fixture[9])[8] = innerFixture;

        ////    String expected = "[[true, false], [0, 1], [a, b], [0.0, 1.0], [0.0, 1.0], [0, 1], [0, 1], [0, 1], [true, false], [[...], [0, 1], [a, b], [0.0, 1.0], [0.0, 1.0], [0, 1], [0, 1], [0, 1], [innerFixture0, [...], [...], innerFixture3]]]";

        ////    assertEquals(expected, Arrays.DeepToString(fixture));
        ////}

        /**
         * @tests java.util.Arrays#deepEquals(Object[], Object[])      
         */
        [Test]
        public void Test_deepEquals_Ljava_lang_ObjectLjava_lang_Object()
        {
            int[] a1 = { 1, 2, 3 };
            short[] a2 = { 0, 1 };
            Object[] a3 = { new int?(1), a2 };
            int[] a4 = { 6, 5, 4 };

            int[] b1 = { 1, 2, 3 };
            short[] b2 = { 0, 1 };
            Object[] b3 = { new int?(1), b2 };

            Object[] a = { a1, a2, a3 };
            Object[] b = { b1, b2, b3 };

            assertFalse(Arrays.Equals(a, b));
            assertTrue(Arrays.DeepEquals(a, b));

            a[2] = a4;

            assertFalse(Arrays.DeepEquals(a, b));
        }

        /**
         * @tests java.util.Arrays#deepHashCode(Object[])
         */
        [Test]
        public void Test_deepHashCode_Ljava_lang_Object()
        {
            int[] a1 = { 1, 2, 3 };
            short[] a2 = { 0, 1 };
            Object[] a3 = { new int?(1), a2 };

            int[] b1 = { 1, 2, 3 };
            short[] b2 = { 0, 1 };
            Object[] b3 = { new int?(1), b2 };

            Object[] a = { a1, a2, a3 };
            Object[] b = { b1, b2, b3 };

            int deep_hash_a = Arrays.GetDeepHashCode(a);
            int deep_hash_b = Arrays.GetDeepHashCode(b);

            assertEquals(deep_hash_a, deep_hash_b);
        }

        /**
         * @tests java.util.Arrays#hashCode(boolean[] a)
         */
        [Test]
        public void Test_hashCode_LZ()
        {
            int listHashCode;
            int arrayHashCode;

            bool[] boolArr = { true, false, false, true, false };
            List<bool> listOfBoolean = new List<bool>();
            for (int i = 0; i < boolArr.Length; i++)
            {
                listOfBoolean.Add(boolArr[i]);
            }
            listHashCode = CollectionUtil.GetHashCode(listOfBoolean);
            arrayHashCode = Arrays.GetHashCode(boolArr);
            assertEquals(listHashCode, arrayHashCode);
        }

        /**
         * @tests java.util.Arrays#hashCode(int[] a)
         */
        [Test]
        public void Test_hashCode_LI()
        {
            int listHashCode;
            int arrayHashCode;

            int[] intArr = { 10, 5, 134, 7, 19 };
            List<int> listOfInteger = new List<int>();

            for (int i = 0; i < intArr.Length; i++)
            {
                listOfInteger.Add(intArr[i]);
            }
            listHashCode = CollectionUtil.GetHashCode(listOfInteger);
            arrayHashCode = Arrays.GetHashCode(intArr);
            assertEquals(listHashCode, arrayHashCode);

            int[] intArr2 = { 10, 5, 134, 7, 19 };
            assertEquals(Arrays.GetHashCode(intArr2), Arrays.GetHashCode(intArr));
        }

        /**
         * @tests java.util.Arrays#hashCode(char[] a)
         */
        [Test]
        public void Test_hashCode_LC()
        {
            int listHashCode;
            int arrayHashCode;

            char[] charArr = { 'a', 'g', 'x', 'c', 'm' };
            List<char> listOfCharacter = new List<char>();
            for (int i = 0; i < charArr.Length; i++)
            {
                listOfCharacter.Add(charArr[i]);
            }
            listHashCode = CollectionUtil.GetHashCode(listOfCharacter);
            arrayHashCode = Arrays.GetHashCode(charArr);
            assertEquals(listHashCode, arrayHashCode);
        }

        /**
         * @tests java.util.Arrays#hashCode(byte[] a)
         */
        [Test]
        public void Test_hashCode_LB()
        {
            int listHashCode;
            int arrayHashCode;

            byte[] byteArr = { 5, 9, 7, 6, 17 };
            List<byte> listOfByte = new List<byte>();
            for (int i = 0; i < byteArr.Length; i++)
            {
                listOfByte.Add(byteArr[i]);
            }
            listHashCode = CollectionUtil.GetHashCode(listOfByte);
            arrayHashCode = Arrays.GetHashCode(byteArr);
            assertEquals(listHashCode, arrayHashCode);
        }

        /**
         * @tests java.util.Arrays#hashCode(long[] a)
         */
        [Test]
        public void Test_hashCode_LJ()
        {
            int listHashCode;
            int arrayHashCode;

            long[] longArr = {67890234512L, 97587236923425L, 257421912912L,
                6754268100L, 5};
            List<long> listOfLong = new List<long>();
            for (int i = 0; i < longArr.Length; i++)
            {
                listOfLong.Add(longArr[i]);
            }
            listHashCode = CollectionUtil.GetHashCode(listOfLong);
            arrayHashCode = Arrays.GetHashCode(longArr);
            assertEquals(listHashCode, arrayHashCode);
        }

        /**
         * @tests java.util.Arrays#hashCode(float[] a)
         */
        [Test]
        public void Test_hashCode_LF()
        {
            int listHashCode;
            int arrayHashCode;

            float[] floatArr = { 0.13497f, 0.268934f, 12e-5f, -3e+2f, 10e-4f };
            List<float> listOfFloat = new List<float>();
            for (int i = 0; i < floatArr.Length; i++)
            {
                listOfFloat.Add(floatArr[i]);
            }
            listHashCode = CollectionUtil.GetHashCode(listOfFloat);
            arrayHashCode = Arrays.GetHashCode(floatArr);
            assertEquals(listHashCode, arrayHashCode);

            float[] floatArr2 = { 0.13497f, 0.268934f, 12e-5f, -3e+2f, 10e-4f };
            assertEquals(Arrays.GetHashCode(floatArr2), Arrays.GetHashCode(floatArr));
        }

        /**
         * @tests java.util.Arrays#hashCode(double[] a)
         */
        [Test]
        public void Test_hashCode_LD()
        {
            int listHashCode;
            int arrayHashCode;

            double[] doubleArr = { 0.134945657, 0.0038754, 11e-150, -30e-300, 10e-4 };
            List<double> listOfDouble = new List<double>();
            for (int i = 0; i < doubleArr.Length; i++)
            {
                listOfDouble.Add(doubleArr[i]);
            }
            listHashCode = CollectionUtil.GetHashCode(listOfDouble);
            arrayHashCode = Arrays.GetHashCode(doubleArr);
            assertEquals(listHashCode, arrayHashCode);
        }

        /**
         * @tests java.util.Arrays#hashCode(short[] a)
         */
        [Test]
        public void Test_hashCode_LS()
        {
            int listHashCode;
            int arrayHashCode;

            short[] shortArr = { 35, 13, 45, 2, 91 };
            List<short> listOfShort = new List<short>();
            for (int i = 0; i < shortArr.Length; i++)
            {
                listOfShort.Add(shortArr[i]);
            }
            listHashCode = CollectionUtil.GetHashCode(listOfShort);
            arrayHashCode = Arrays.GetHashCode(shortArr);
            assertEquals(listHashCode, arrayHashCode);
        }

        /**
         * @tests java.util.Arrays#hashCode(Object[] a)
         */
        [Test]
        public void Test_hashCode_Ljava_lang_Object()
        {
            int listHashCode;
            int arrayHashCode;

            Object[] objectArr = { new int?(1), new float?(10e-12f), null };
            List<object> listOfObject = new List<object>();
            for (int i = 0; i < objectArr.Length; i++)
            {
                listOfObject.Add(objectArr[i]);
            }
            listHashCode = CollectionUtil.GetHashCode(listOfObject);
            arrayHashCode = Arrays.GetHashCode(objectArr);
            assertEquals(listHashCode, arrayHashCode);
        }
    }
}
