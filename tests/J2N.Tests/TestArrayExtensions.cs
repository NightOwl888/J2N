using J2N.Collections;
using NUnit.Framework;
using System;

namespace J2N
{
    public class TestArrayExtensions : TestCase
    {
        /**
         * @tests java.util.Arrays#fill(byte[], byte)
         */
        [Test]
        public void Test_fill_BB()
        {
            // Test for method void java.util.Arrays.fill(byte [], byte)

            byte[] d = new byte[1000];
            ArrayExtensions.Fill(d, byte.MaxValue);
            for (int i = 0; i < d.Length; i++)
                assertTrue("Failed to fill byte array correctly",
                        d[i] == byte.MaxValue);
        }

        /**
         * @tests java.util.Arrays#fill(byte[], int, int, byte)
         */
        [Test]
        public void Test_fill_BIIB()
        {
            // Test for method void java.util.Arrays.fill(byte [], int, int, byte)
            byte val = Byte.MaxValue;
            byte[] d = new byte[1000];
            ArrayExtensions.Fill(d, 400, d.Length - 400, val); // J2N: Corrected 3rd parameter
            for (int i = 0; i < 400; i++)
                assertTrue("Filled elements not in range", !(d[i] == val));
            for (int i = 400; i < d.Length; i++)
                assertTrue("Failed to fill byte array correctly", d[i] == val);

            int result;
            try
            {
                ArrayExtensions.Fill(new byte[2], 2, 1 - 2, (byte)27); // J2N: Corrected 3rd parameter
                result = 0;
            }
            catch (ArgumentOutOfRangeException e)
            {
                result = 1;
            }
            catch (ArgumentException e)
            {
                result = 2;
            }
            assertEquals("Wrong exception1", 1, result); // J2N: Changed exception type to ArgumentOutOfRangeException for negative values
            try
            {
                ArrayExtensions.Fill(new byte[2], -1, 1 - -1, (byte)27); // J2N: Corrected 3rd parameter
                result = 0;
            }
            catch (ArgumentOutOfRangeException e)
            {
                result = 1;
            }
            catch (ArgumentException e)
            {
                result = 2;
            }
            assertEquals("Wrong exception2", 1, result);
            try
            {
                ArrayExtensions.Fill(new byte[2], 1, 4 - 1, (byte)27); // J2N: Corrected 3rd parameter
                result = 0;
            }
            catch (ArgumentOutOfRangeException e)
            {
                result = 1;
            }
            catch (ArgumentException e)
            {
                result = 2;
            }
            assertEquals("Wrong exception", 1, result);
        }

        /**
         * @tests java.util.Arrays#fill(short[], short)
         */
        [Test]
        public void Test_fill_SS()
        {
            // Test for method void java.util.Arrays.fill(short [], short)

            short[] d = new short[1000];
            ArrayExtensions.Fill(d, short.MaxValue);
            for (int i = 0; i < d.Length; i++)
                assertTrue("Failed to fill short array correctly",
                        d[i] == short.MaxValue);
        }

        /**
         * @tests java.util.Arrays#fill(short[], int, int, short)
         */
        [Test]
        public void Test_fill_SIIS()
        {
            // Test for method void java.util.ArrayExtensions.fill(short [], int, int, short)
            short val = short.MaxValue;
            short[] d = new short[1000];
            ArrayExtensions.Fill(d, 400, d.Length - 400, val); // J2N: Corrected 3rd parameter
            for (int i = 0; i < 400; i++)
                assertTrue("Filled elements not in range", !(d[i] == val));
            for (int i = 400; i < d.Length; i++)
                assertTrue("Failed to fill short array correctly", d[i] == val);
        }

        /**
         * @tests java.util.Arrays#fill(char[], char)
         */
        [Test]
        public void Test_fill_CC()
        {
            // Test for method void java.util.Arrays.fill(char [], char)

            char[] d = new char[1000];
            ArrayExtensions.Fill(d, 'V');
            for (int i = 0; i < d.Length; i++)
                assertEquals("Failed to fill char array correctly", 'V', d[i]);
        }

        /**
         * @tests java.util.Arrays#fill(char[], int, int, char)
         */
        [Test]
        public void Test_fill_CIIC()
        {
            // Test for method void java.util.Arrays.fill(char [], int, int, char)
            char val = 'T';
            char[] d = new char[1000];
            ArrayExtensions.Fill(d, 400, d.Length - 400, val); // J2N: Corrected 3rd parameter
            for (int i = 0; i < 400; i++)
                assertTrue("Filled elements not in range", !(d[i] == val));
            for (int i = 400; i < d.Length; i++)
                assertTrue("Failed to fill char array correctly", d[i] == val);
        }

        /**
         * @tests java.util.Arrays#fill(int[], int)
         */
        [Test]
        public void Test_fill_II()
        {
            // Test for method void java.util.Arrays.fill(int [], int)

            int[] d = new int[1000];
            ArrayExtensions.Fill(d, int.MaxValue);
            for (int i = 0; i < d.Length; i++)
                assertTrue("Failed to fill int array correctly",
                        d[i] == int.MaxValue);
        }

        /**
         * @tests java.util.Arrays#fill(int[], int, int, int)
         */
        [Test]
        public void Test_fill_IIII()
        {
            // Test for method void java.util.Arrays.fill(int [], int, int, int)
            int val = int.MaxValue;
            int[] d = new int[1000];
            ArrayExtensions.Fill(d, 400, d.Length - 400, val); // J2N: Corrected 3rd parameter
            for (int i = 0; i < 400; i++)
                assertTrue("Filled elements not in range", !(d[i] == val));
            for (int i = 400; i < d.Length; i++)
                assertTrue("Failed to fill int array correctly", d[i] == val);
        }

        /**
         * @tests java.util.Arrays#fill(long[], long)
         */
        [Test]
        public void Test_fill_JJ()
        {
            // Test for method void java.util.Arrays.fill(long [], long)

            long[] d = new long[1000];
            ArrayExtensions.Fill(d, long.MaxValue);
            for (int i = 0; i < d.Length; i++)
                assertTrue("Failed to fill long array correctly",
                        d[i] == long.MaxValue);
        }

        /**
         * @tests java.util.Arrays#fill(long[], int, int, long)
         */
        [Test]
        public void Test_fill_JIIJ()
        {
            // Test for method void java.util.Arrays.fill(long [], int, int, long)
            long[] d = new long[1000];
            ArrayExtensions.Fill(d, 400, d.Length - 400, long.MaxValue); // J2N: Corrected 3rd parameter
            for (int i = 0; i < 400; i++)
                assertTrue("Filled elements not in range", !(d[i] == long.MaxValue));
            for (int i = 400; i < d.Length; i++)
                assertTrue("Failed to fill long array correctly",
                        d[i] == long.MaxValue);
        }

        /**
         * @tests java.util.Arrays#fill(float[], float)
         */
        [Test]
        public void Test_fill_FF()
        {
            // Test for method void java.util.Arrays.fill(float [], float)
            float[] d = new float[1000];
            ArrayExtensions.Fill(d, float.MaxValue);
            for (int i = 0; i < d.Length; i++)
                assertTrue("Failed to fill float array correctly",
                        d[i] == float.MaxValue);
        }

        /**
         * @tests java.util.Arrays#fill(float[], int, int, float)
         */
        [Test]
        public void Test_fill_FIIF()
        {
            // Test for method void java.util.Arrays.fill(float [], int, int, float)
            float val = float.MaxValue;
            float[] d = new float[1000];
            ArrayExtensions.Fill(d, 400, d.Length - 400, val); // J2N: Corrected 3rd parameter
            for (int i = 0; i < 400; i++)
                assertTrue("Filled elements not in range", !(d[i] == val));
            for (int i = 400; i < d.Length; i++)
                assertTrue("Failed to fill float array correctly", d[i] == val);
        }

        /**
         * @tests java.util.Arrays#fill(double[], double)
         */
        [Test]
        public void Test_fill_DD()
        {
            // Test for method void java.util.Arrays.fill(double [], double)

            double[] d = new double[1000];
            ArrayExtensions.Fill(d, double.MaxValue);
            for (int i = 0; i < d.Length; i++)
                assertTrue("Failed to fill double array correctly",
                        d[i] == double.MaxValue);
        }

        /**
         * @tests java.util.Arrays#fill(double[], int, int, double)
         */
        [Test]
        public void Test_fill_DIID()
        {
            // Test for method void java.util.Arrays.fill(double [], int, int,
            // double)
            double val = double.MaxValue;
            double[] d = new double[1000];
            ArrayExtensions.Fill(d, 400, d.Length - 400, val); // J2N: Corrected 3rd parameter
            for (int i = 0; i < 400; i++)
                assertTrue("Filled elements not in range", !(d[i] == val));
            for (int i = 400; i < d.Length; i++)
                assertTrue("Failed to fill double array correctly", d[i] == val);
        }

        /**
         * @tests java.util.Arrays#fill(boolean[], boolean)
         */
        [Test]
        public void Test_fill_ZZ()
        {
            // Test for method void java.util.Arrays.fill(boolean [], boolean)

            bool[] d = new bool[1000];
            ArrayExtensions.Fill(d, true);
            for (int i = 0; i < d.Length; i++)
                assertTrue("Failed to fill boolean array correctly", d[i]);
        }

        /**
         * @tests java.util.Arrays#fill(boolean[], int, int, boolean)
         */
        [Test]
        public void Test_fill_ZIIZ()
        {
            // Test for method void java.util.Arrays.fill(boolean [], int, int,
            // boolean)
            bool val = true;
            bool[] d = new bool[1000];
            ArrayExtensions.Fill(d, 400, d.Length - 400, val); // J2N: Corrected 3rd parameter
            for (int i = 0; i < 400; i++)
                assertTrue("Filled elements not in range", !(d[i] == val));
            for (int i = 400; i < d.Length; i++)
                assertTrue("Failed to fill boolean array correctly", d[i] == val);
        }

        /**
         * @tests java.util.Arrays#fill(java.lang.Object[], java.lang.Object)
         */
        [Test]
        public void Test_fill_Ljava_lang_ObjectLjava_lang_Object()
        {
            // Test for method void java.util.Arrays.fill(java.lang.Object [],
            // java.lang.Object)
            Object val = new Object();
            Object[] d = new Object[1000];
            ArrayExtensions.Fill(d, 0, d.Length - 0, val); // J2N: Corrected 3rd parameter
            for (int i = 0; i < d.Length; i++)
                assertTrue("Failed to fill Object array correctly", d[i] == val);
        }

        /**
         * @tests java.util.Arrays#fill(java.lang.Object[], int, int,
         *        java.lang.Object)
         */
        [Test]
        public void Test_fill_Ljava_lang_ObjectIILjava_lang_Object()
        {
            // Test for method void java.util.Arrays.fill(java.lang.Object [], int,
            // int, java.lang.Object)
            Object val = new Object();
            Object[] d = new Object[1000];
            ArrayExtensions.Fill(d, 400, d.Length - 400, val); // J2N: Corrected 3rd parameter
            for (int i = 0; i < 400; i++)
                assertTrue("Filled elements not in range", !(d[i] == val));
            for (int i = 400; i < d.Length; i++)
                assertTrue("Failed to fill Object array correctly", d[i] == val);

            ArrayExtensions.Fill(d, 400, d.Length - 400, null); // J2N: Corrected 3rd parameter
            for (int i = 400; i < d.Length; i++)
                assertNull("Failed to fill Object array correctly with nulls",
                        d[i]);
        }

    }
}
